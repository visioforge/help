---
title: SDK de Reproductor Multimedia para Android
description: Crea apps de reproductor Android con reproducción de video, streaming, aceleración por hardware y soporte de múltiples formatos con el SDK de Media Player.
---

# SDK de Reproductor Multimedia para Android - Solución Profesional de Reproducción de Video

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción General

El SDK de VisioForge para Android permite a los desarrolladores integrar reproducción de video profesional, streaming y edición en aplicaciones nativas de Android. Construido sobre GStreamer, proporciona una API completa para aplicaciones ricas en funcionalidades.

El SDK soporta extensos formatos de video, códecs y protocolos de streaming.

## Características Principales

### Reproducción de Video y Streaming

Nuestro SDK de reproductor Android ofrece reproducción potente con aceleración por hardware, asegurando un rendimiento óptimo para contenido de alta resolución. Los desarrolladores integran el reproductor usando una API intuitiva con soporte para MP4, MKV, AVI, WebM y otros formatos.

El reproductor proporciona control preciso con reproducir, pausar, buscar y navegación. Velocidades de reproducción variables y navegación fotograma a fotograma dan control completo sobre la experiencia de visualización.

Transmite contenido desde varias fuentes incluyendo HTTP Live Streaming (HLS), RTSP y RTMP. El streaming de tasa de bits adaptativa ajusta la calidad basada en el ancho de banda para usuarios móviles.

### Edición de Video y Efectos

El SDK incluye capacidades de edición de video para crear aplicaciones de editor. Aplica efectos en tiempo real incluyendo ajustes de brillo, contraste y saturación.

Superpone texto, imágenes y gráficos SVG con control sobre posicionamiento y transparencia para picture-in-picture, marcas de agua y elementos interactivos.

### Soporte Nativo Android y Multiplataforma

El SDK se integra perfectamente con Android Studio, soportando desarrollo en Java y Kotlin. El componente VideoView se incrusta en cualquier layout de Android.

El SDK también soporta .NET MAUI y Avalonia para desarrollo multiplataforma, permitiendo compartir código entre Android, iOS, Windows, macOS y Linux.

## Capacidades Técnicas

### Soporte de Códecs y Formatos

El SDK soporta extensos códecs de video con decodificación acelerada por hardware para H.264, H.265/HEVC, VP8 y VP9. La reproducción de audio soporta AAC, MP3, Opus y Vorbis.

### API y Rendimiento

Nuestra referencia de API proporciona documentación detallada. Los ejemplos de código demuestran casos de uso comunes. Los eventos y callbacks proporcionan notificaciones en tiempo real.

El SDK está optimizado para móviles con atención a la batería y memoria. La aceleración por hardware asegura una reproducción fluida.

## Primeros Pasos

### Instalación y Configuración

Integra el SDK de Reproductor Android de VisioForge usando NuGet. Añade la referencia del paquete a tu proyecto. Para .NET MAUI, configura para usar el control VideoView.

Las instrucciones de configuración están en nuestra documentación.

### Ejemplo de Código de Inicio Rápido

Aquí está cómo crear un reproductor multimedia básico:

#### Añadir VideoView al Layout

```xml
<VisioForge.Core.UI.Android.VideoViewTX
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:id="@+id/videoView" />
```

#### Inicializar el Reproductor

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

#### Controles de Reproducción

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

### Aplicaciones de Ejemplo

Los ejemplos de GitHub demuestran las capacidades del SDK: [Ejemplo de Media Player](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer) con reproducción, streaming y edición.

### Alternativa: Media Blocks SDK

El [Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android/MediaPlayer) proporciona una API de nivel más bajo para pipelines personalizados.

## Casos de Uso

El SDK de Reproductor Android es ideal para:

- **Apps de Streaming de Video**: Soporte de streaming adaptativo
- **Plataformas Educativas**: Lecciones en video y e-learning
- **Reproductores Multimedia**: Apps de reproductor nativas con soporte de subtítulos
- **Redes Sociales**: Reproducción de contenido generado por usuarios
- **Editores de Video**: Edición móvil con vista previa en tiempo real
- **Seguridad**: Apps de vigilancia con streaming en vivo

El SDK soporta Android TV y modo picture-in-picture en Android 8.0+.

## Licenciamiento

El SDK de Reproductor Android está disponible bajo una licencia comercial. Una sola licencia cubre todas las plataformas soportadas. Hay versiones de prueba disponibles.

## Conclusión

El SDK de Reproductor Android de VisioForge proporciona reproducción de video profesional para aplicaciones Android. Con streaming, edición y características avanzadas, los desarrolladores pueden crear aplicaciones multimedia potentes rápidamente.

Para más información, visita nuestra [página de producto](https://www.visioforge.com/media-player-sdk-net) o [documentación de API](https://api.visioforge.org/dotnet/api/index.html).

## Recursos Relacionados

- [Guía de Implementación Android](../../deployment-x/Android.md) - Instrucciones detalladas de despliegue para Android
- [Ejemplos de Código](../code-samples/index.md) - Ejemplos funcionales y fragmentos
- [Guía de Reproductor Avalonia Multiplataforma](avalonia-player.md) - Construyendo apps de video multiplataforma
- [Registro de Cambios](../../changelog.md) - Últimas actualizaciones y versiones
- [Acuerdo de Licencia de Usuario Final](../../../eula.md) - Información de licenciamiento
