---
title: Editor de video iOS | Cree apps de video más rápido
description: Integre edición de video profesional en su app iOS con Video Edit SDK. Configuración rápida, UI personalizable y soporte para filtros y transiciones.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - Effects
  - MP4
  - C#
primary_api_classes:
  - VideoEditCoreX
  - IVideoView
  - VideoBalanceVideoEffect
  - MP4Output
  - ProgressEventArgs

---

# Editor de video iOS para edición de video integrada

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introducción a la edición de video en iOS

Construir una aplicación de edición de video profesional para iPhone y iPad requiere un SDK robusto que entregue rendimiento nativo con características personalizables. El VisioForge Video Edit SDK proporciona herramientas para crear impresionantes aplicaciones de edición que rivalizan con Adobe Premiere o DaVinci Resolve en dispositivos Apple.

Nuestro Video Edit SDK para iOS integra capacidades avanzadas de edición de video en su app iOS de manera eficiente. Construya una aplicación de foto y video, herramientas de creación de contenido o una aplicación de editor de video profesional con las características que los usuarios esperan de las aplicaciones de edición modernas.

## Características principales

El SDK proporciona características completas de edición de video para el desarrollo de aplicaciones iOS:

- **Recorte**: Recorte de video preciso a nivel de fotograma con controles táctiles
- **Línea de tiempo**: Edite múltiples pistas de video y audio simultáneamente  
- **Transiciones**: Efectos suaves incluyendo fundidos y cortinas entre clips
- **Efectos de video**: Aplique filtros y corrección de color a sus videos
- **Mezcla de audio**: Controle el volumen y mezcle múltiples fuentes de audio
- **Superposiciones de texto**: Añada títulos y marcas de agua personalizables

Aunque optimizado para iOS, nuestro framework soporta Android a través de .NET MAUI, permitiéndole crear soluciones de edición multiplataforma.

## Comenzar con VideoEditCoreX

### Inicialización del SDK

Inicialice el motor de edición de video en su app iOS:

```csharp
using System.Drawing;                              // Size
using VisioForge.Core;                             // VisioForgeX
using VisioForge.Core.Types;                       // IVideoView, VideoFrameRate
using VisioForge.Core.Types.X.VideoEdit;           // VideoTransition, VideoTransitionType
using VisioForge.Core.VideoEditX;                  // VideoEditCoreX

await VisioForgeX.InitSDKAsync();

var videoEdit = new VideoEditCoreX(VideoView1 as IVideoView);
videoEdit.OnError += VideoEdit_OnError;
videoEdit.OnProgress += VideoEdit_OnProgress;
videoEdit.OnStop += VideoEdit_OnStop;
```

### Añadir contenido de video

Añada archivos de video a su línea de tiempo:

```csharp
// Añadir archivo de video completo
videoEdit.Input_AddVideoFile("input.mp4", null);

// O añadir video con tiempos de inicio y fin específicos
videoEdit.Input_AddAudioVideoFile(
    "input.mp4",
    TimeSpan.FromMilliseconds(0),
    TimeSpan.FromMilliseconds(10000),
    insertTime: null);
```

### Aplicar efectos

Mejore los videos con efectos que los usuarios eligen para su contenido:

```csharp
using VisioForge.Core.Types.X.VideoEffects;

var balance = new VideoBalanceVideoEffect();
balance.Brightness = 0.1;
balance.Contrast = 1.0;
videoEdit.Video_Effects.Add(balance);
```

### Configurar la salida

Exporte videos optimizados para YouTube o cumplimiento con políticas de App Store:

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;

videoEdit.Output_VideoSize = new Size(1920, 1080);
videoEdit.Output_VideoFrameRate = new VideoFrameRate(30);

var mp4Output = new MP4Output("output.mp4");
videoEdit.Output_Format = mp4Output;
videoEdit.Start();
```

### Manejo de eventos

Monitoree el progreso de la edición:

```csharp
private void VideoEdit_OnProgress(object sender, ProgressEventArgs e)
{
    Console.WriteLine($"Progreso: {e.Progress}%");
}

private void VideoEdit_OnStop(object sender, StopEventArgs e)
{
    Console.WriteLine(e.Successful ? "Completado" : "Error");
}
```

## Opciones avanzadas

### API de superposición de texto

Añada superposiciones de texto usando la API de renderizado nativa:

```csharp
using VisioForge.Core.Types.X.VideoEdit;

var textOverlay = new TextOverlay("Tu Título");
videoEdit.Video_TextOverlays.Add(textOverlay);
```

### Transiciones de video

Cree transiciones suaves entre clips:

```csharp
var transition = new VideoTransition(
    VideoTransitionType.Crossfade,
    TimeSpan.FromMilliseconds(1000),
    TimeSpan.FromMilliseconds(2000));
videoEdit.Video_Transitions.Add(transition);
```

## Despliegue en iOS

Para instrucciones detalladas de despliegue en iOS, incluyendo paquetes NuGet, permisos y mejores prácticas, consulte nuestra [Guía de despliegue iOS](../../deployment-x/iOS.md).

## Por qué elegir VisioForge

- **API profesional**: Control completo sobre la edición de video
- **UI personalizable**: Construya su propia interfaz
- **Rendimiento nativo**: Codificación acelerada por GPU en dispositivos Apple

---
Explore muestras de edición de video iOS en nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples) o contacte a [soporte](https://helpdesk.visioforge.com/helpdesk/my-tickets/new) para recursos.