---
title: Reproductor de video Android con HLS y RTSP en C# .NET
description: Cree apps de reproductor Android con reproducción de video, streaming, aceleración por hardware y soporte de múltiples formatos con Media Player SDK .NET.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - RTSP
  - HLS
  - MP4
  - C#
primary_api_classes:
  - MediaPlayerCoreX

---

# Media Player SDK .NET para Android - Solución profesional de reproducción de video

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

El Media Player SDK .NET de VisioForge para Android permite a los desarrolladores integrar reproducción de video profesional, streaming y efectos en tiempo real en aplicaciones nativas de Android. Construido sobre GStreamer, proporciona una API completa para aplicaciones ricas en funcionalidades.

El SDK soporta extensos formatos de video, códecs y protocolos de streaming.

## Características principales

### Reproducción de video y streaming

En Android, el Media Player SDK .NET ofrece reproducción potente con aceleración por hardware, asegurando un rendimiento óptimo para contenido de alta resolución. Los desarrolladores integran el reproductor usando una API intuitiva con soporte para MP4, MKV, AVI, WebM y otros formatos.

El reproductor proporciona control preciso con reproducir, pausar, buscar y navegación. Velocidades de reproducción variables y navegación fotograma a fotograma dan control completo sobre la experiencia de visualización.

Transmita contenido desde varias fuentes incluyendo HTTP Live Streaming (HLS), RTSP y RTMP. El streaming de tasa de bits adaptativa ajusta la calidad basada en el ancho de banda para usuarios móviles.

### Efectos y superposiciones de video

Aplique efectos en tiempo real durante la reproducción, incluyendo ajustes de brillo, contraste y saturación.

Superponga texto, imágenes y gráficos SVG con control sobre posicionamiento y transparencia para picture-in-picture, marcas de agua y elementos interactivos.

### Compatibilidad nativa con Android y multiplataforma

La aplicación se desarrolla en C# con .NET for Android (Visual Studio o Rider). El componente VideoView se incrusta en cualquier layout de Android.

El SDK también soporta .NET MAUI y Avalonia para desarrollo multiplataforma, permitiendo compartir código entre Android, iOS, Windows, macOS y Linux.

## Capacidades técnicas

### Códecs y formatos compatibles

El SDK soporta extensos códecs de video con decodificación acelerada por hardware para H.264, H.265/HEVC, VP8 y VP9. La reproducción de audio soporta AAC, MP3, Opus y Vorbis.

### API y rendimiento

Nuestra referencia de API proporciona documentación detallada. Los ejemplos de código demuestran casos de uso comunes. Los eventos y callbacks proporcionan notificaciones en tiempo real.

El SDK está optimizado para móviles con atención a la batería y memoria. La aceleración por hardware asegura una reproducción fluida.

## Primeros pasos

### Instalación y configuración

Integre el Media Player SDK .NET de VisioForge usando NuGet. Añada la referencia del paquete a su proyecto. Para .NET MAUI, configure para usar el control VideoView.

Las instrucciones de configuración están en nuestra documentación.

### Ejemplo de código de inicio rápido

Aquí está cómo crear un reproductor multimedia básico:

#### Añadir VideoView al layout

```xml
<VisioForge.Core.UI.Android.VideoViewTX
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:id="@+id/videoView" />
```

#### Inicializar el reproductor

```csharp
using VisioForge.Core.MediaPlayerX;

public class MainActivity : Activity
{
    private MediaPlayerCoreX _player;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        var videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView);
        _player = new MediaPlayerCoreX(videoView);
    }

    protected override void OnDestroy()
    {
        VisioForgeX.DestroySDK();
        base.OnDestroy();
    }
}
```

#### Controles de reproducción

```csharp
private async void PlayVideo()
{
    await _player.OpenAsync(new Uri("https://example.com/video.mp4"));
    await _player.PlayAsync();
}

private async void PauseVideo() => await _player.PauseAsync();
private async void ResumeVideo() => await _player.ResumeAsync();
private async void StopVideo() => await _player.StopAsync();
```

### Aplicaciones de ejemplo

Los ejemplos de GitHub demuestran las capacidades del SDK: [Ejemplo de Media Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer) con reproducción y streaming.

### Alternativa: Media Blocks SDK

El [Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android/MediaPlayer) proporciona una API de nivel más bajo para pipelines personalizados.

## Casos de uso

En Android, el Media Player SDK .NET es ideal para:

- **Apps de Streaming de Video**: Soporte de streaming adaptativo
- **Plataformas Educativas**: Lecciones en video y e-learning
- **Reproductores Multimedia**: Apps de reproductor nativas con soporte de subtítulos
- **Redes Sociales**: Reproducción de contenido generado por usuarios
- **Seguridad**: Apps de vigilancia con streaming en vivo

El SDK soporta Android TV y modo picture-in-picture en Android 8.0+.

## Licenciamiento

El Media Player SDK .NET está disponible bajo una licencia comercial. Una sola licencia cubre todas las plataformas soportadas. Hay versiones de prueba disponibles.

## Conclusión

El Media Player SDK .NET de VisioForge proporciona reproducción de video profesional para aplicaciones Android. Con streaming, efectos en tiempo real y superposiciones, los desarrolladores pueden crear aplicaciones multimedia potentes rápidamente.

Para más información, visite nuestra [página de producto](https://www.visioforge.com/media-player-sdk-net) o [documentación de API](https://api.visioforge.org/dotnet/api/index.html).

## Recursos relacionados

- [Guía de implementación en Android](../../deployment-x/Android.md) - Instrucciones detalladas de despliegue para Android
- [Ejemplos de código](../code-samples/index.md) - Ejemplos funcionales y fragmentos
- [Guía del reproductor multiplataforma con Avalonia](../guides/avalonia-player.md) - Construyendo apps de video multiplataforma
- [Registro de cambios](../../changelog.md) - Últimas actualizaciones y versiones
- [Acuerdo de licencia de usuario final](../../../eula.md) - Información de licenciamiento
