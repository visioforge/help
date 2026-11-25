---
title: Preguntas frecuentes de Media Player SDK .NET — WPF
description: Respuestas sobre Media Player SDK .NET — URL de red, fotogramas tras una búsqueda, velocidad, paso a paso, inicio sincronizado, WPF y karaoke.
sidebar_label: FAQ
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - Windows
  - Playback
  - RTSP
  - WPF
  - Troubleshooting
primary_api_classes:
  - MediaPlayerCore
  - MediaPlayerCoreX
  - CDGSourceSettings
  - MediaInfoReader
  - MediaInfoReaderX

---

# Preguntas frecuentes de Media Player SDK .NET

Respuestas a las preguntas que los desarrolladores hacen con más frecuencia sobre Media Player SDK .NET, para los dos motores: `MediaPlayerCore` (Windows, DirectShow) y `MediaPlayerCoreX` (multiplataforma). Los errores de despliegue están reunidos en [Solución de problemas](../general/troubleshooting.md).

## Fuentes de red

### "Unable to load source file or stream": un flujo de red no se abre

`MediaPlayerCore` informa este mensaje, o `Unable to set URL to network VLC source filter` con el modo de fuente `HTTP_RTSP_VLC`, cuando el motor de la fuente no pudo abrir la URL. Compruebe, en este orden:

1. La URL es la dirección del propio flujo (por ejemplo `rtsp://host:554/path` o `http://host/stream.m3u8`), no la de una página web ni solo la dirección IP de la cámara.
2. La misma URL se abre en VLC en el mismo equipo. Si no se abre, el problema es la URL, la red o un firewall, no el reproductor.
3. Las credenciales están incluidas: `rtsp://login:password@host:port/path`.
4. El puerto es accesible desde el equipo que ejecuta la aplicación, no solo desde el equipo de desarrollo.

Para cámaras IP que necesitan algo más que una URL, como el descubrimiento ONVIF o una conexión de baja latencia, consulte las [Preguntas frecuentes de Video Capture SDK](../videocapture/faq.md#camaras-ip).

## Búsqueda, velocidad y fotogramas

### Frame_GetCurrent devuelve el fotograma anterior justo después de una búsqueda

`Position_Set_Time` retorna cuando se solicita la búsqueda, antes de que el nuevo fotograma se haya decodificado y renderizado, así que un `Frame_GetCurrent()` inmediato todavía ve el anterior. Tome en su lugar el fotograma del evento `OnVideoFrameBitmap` después de la búsqueda: active un indicador al buscar y guarde una copia de `e.Frame` (`new Bitmap(e.Frame)`) la primera vez que se ejecute el controlador después de eso; el mapa de bits del propio evento pertenece al SDK. Para leer fotogramas de un archivo sin reproducirlo, consulte [Obtener un fotograma de un archivo de video](code-samples/get-frame-from-video-file.md).

### ¿Cómo reproduzco más rápido o más lento, fotograma a fotograma o hacia atrás?

Con `MediaPlayerCore`:

- `SetSpeedAsync(2.0)` reproduce a doble velocidad; los valores inferiores a 1.0 la reducen.
- `NextFrame()` y `PreviousFrame()` pausan la reproducción y avanzan o retroceden un fotograma.
- La reproducción continua hacia atrás usa el modo de reproducción inversa; consulte [Reproducción inversa de video](code-samples/reverse-video-playback.md).

Con `MediaPlayerCoreX`, use `Rate_SetAsync(rate)` y `NextFrame()`.

Retroceder fotograma a fotograma es costoso con formatos que usan fotogramas clave (H.264, HEVC y la mayoría de los demás): el decodificador tiene que volver al fotograma clave anterior y decodificar de nuevo hacia delante. Buscar con `Position_Set_Time` una posición anterior es la forma general de volver atrás.

### ¿Cómo inicio la reproducción en un momento exacto, o varios reproductores a la vez?

Precargue el archivo e inícielo más tarde. `PlayAsync(true)` construye y prepara el grafo de reproducción sin iniciarlo; después, `PlayDelayed()` inicia la reproducción casi sin retraso:

```csharp
await player1.PlayAsync(true);
await player2.PlayAsync(true);

// en el momento que necesite
player1.PlayDelayed();
player2.PlayDelayed();
```

Así se sincroniza el video con un reloj externo o una pista de audio en Windows. Media Player SDK .NET admite aplicaciones WinForms, WPF, WinUI 3, MAUI, Avalonia y Uno; las aplicaciones UWP no son compatibles.

## WPF

### ¿Puedo colocar controles WPF sobre el video o capturarlo con un VisualBrush?

Sí. El `VideoView` de WPF es un elemento WPF, no una ventana de WinForms alojada, así que participa en el diseño, las transformaciones y el orden Z de WPF cuando renderiza en un modo componible. Use el renderizador componible D3D11 para renderizar por GPU, o el modo de software. Solo los renderizadores de ventana nativa dibujan por encima de todo; consulte [Superponer controles WPF sobre el video](../general/code-samples/wpf-controls-over-video.md).

## Archivos y formatos

### ¿Cómo reproduzco archivos de karaoke CDG (MP3+G o ZIP) con cambio de tono?

`MediaPlayerCoreX` los abre con `CDGSourceSettings`, para un par de archivos CDG y de audio o para un archivo ZIP que contenga ambos:

```csharp
var settings = new CDGSourceSettings("song.zip");
settings.EnablePitchShifting = true;
settings.PitchSemitones = -2;
await player.OpenAsync(settings);
await player.PlayAsync();
```

`Rate_SetAsync` también cambia el tempo de los archivos de karaoke. La [Karaoke Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Karaoke%20Demo) muestra un reproductor de karaoke completo.

### ¿Cómo reproduzco un disco Blu-ray?

Establezca `Source_Mode` en `MediaPlayerSourceMode.BluRay` y abra el archivo `BDMV\index.bdmv` del disco. El reproductor reproduce el archivo `.m2ts` más grande de la carpeta `BDMV\STREAM` del disco, que en la mayoría de los discos es la película principal. Usa los mismos decodificadores que el modo FFMPEG, por lo que la aplicación necesita el paquete `VisioForge.DotNet.Core.Redist.FFMPEG.x64` (`.x86` para aplicaciones de 32 bits).

```csharp
player.Source_Mode = MediaPlayerSourceMode.BluRay;
player.Playlist_Add(@"E:\BDMV\index.bdmv");
await player.PlayAsync();
```

No se admiten:

- Imágenes ISO. Monte la imagen en Windows y abra `BDMV\index.bdmv` en la nueva unidad.
- Archivos de lista de reproducción (`.mpls`). Abra `BDMV\index.bdmv` en su lugar.
- Títulos divididos en varios clips. Solo se reproduce el clip más grande.
- Menús del disco.
- Discos cifrados (AACS), lo que incluye la mayoría de los discos comerciales de películas.

Una imagen ISO, un archivo de lista de reproducción, un disco cifrado, un disco sin carpeta `BDMV\STREAM` o un disco que no se puede leer no se reproduce: `PlayAsync` devuelve `false` y `OnError` indica el motivo.

## Decodificación por hardware

### ¿Qué decodificador por hardware debe usar el motor de origen FFMPEG?

Con `Source_Mode` establecido en `MediaPlayerSourceMode.FFMPEG`, establezca `Source_FFMPEG_HWDecoder` antes de `PlayAsync`. Para cámaras IP en Video Capture SDK .NET, la misma opción es `FFMPEG_HWDecoder` en `IPCameraSourceSettings` con el motor `Auto_FFMPEG`. `DXVA2Native` no está disponible ahí.

```csharp
player.Source_Mode = MediaPlayerSourceMode.FFMPEG;
player.Source_FFMPEG_HWDecoder = FFMPEGHWDecoder.Direct3D11;
```

| Valor | Úselo cuando |
|---|---|
| `Auto` (predeterminado) | Desea el comportamiento de las versiones anteriores: decodificación por software. |
| `Software` | El video debe decodificarse en la CPU, por ejemplo para evitar un problema del controlador. |
| `NvidiaCUDA` | El equipo tiene una GPU NVIDIA. |
| `IntelQuickSync` | El equipo tiene gráficos Intel. |
| `Direct3D11` | Cualquier GPU. Una buena primera opción cuando no conoce el fabricante de la GPU. |
| `DXVA2CopyBack` | Cualquier GPU, incluidos controladores antiguos sin decodificación de video Direct3D 11. |
| `DXVA2Native` | El menor uso de CPU, con EVR o madVR y sin efectos de video ni el sample grabber. |

`NvidiaCUDA`, `IntelQuickSync`, `Direct3D11` y `DXVA2CopyBack` copian los fotogramas decodificados a la memoria del sistema, por lo que todos los renderizadores, los efectos de video y el sample grabber funcionan con ellos.

Todos los valores excepto `Auto` son estrictos. Cuando el equipo no tiene esa GPU, la GPU no puede decodificar el video (códec, perfil, profundidad de bits o tamaño de fotograma) o `DXVA2Native` no tiene un renderizador que acepte sus superficies, la reproducción no se inicia y `OnError` indica el decodificador y el motivo. El SDK nunca recurre a la decodificación por software en silencio: gestione `OnError` y elija otro decodificador, o `Auto`.
