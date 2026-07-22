---
title: Bloque de estabilización de vídeo en tiempo real en C# .NET
description: Elimine el temblor de la cámara en vídeo en directo o grabado en C# con VideoStabilizationBlock (deshake de OpenCV) del Media Blocks SDK.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - GStreamer
  - OpenCV
  - Effects
  - Playback
  - C#
primary_api_classes:
  - VideoStabilizationBlock
  - VideoStabilizationSettings
  - UniversalSourceBlock
  - VideoRendererBlock
  - MediaBlocksPipeline

---

# Bloque de estabilización de vídeo

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

El `VideoStabilizationBlock` elimina el temblor de la cámara de un flujo de vídeo en directo o grabado en tiempo real. Estima el movimiento global entre fotogramas (traslación y rotación) con flujo óptico disperso, suaviza la trayectoria resultante de la cámara con una ventana causal de media móvil y deforma cada fotograma para devolverlo a la trayectoria suavizada. Un pequeño zoom central (relación de recorte) oculta los bordes expuestos por la compensación.

El bloque se apoya en el elemento GStreamer `vfdeshake` de OpenCV, por lo que requiere el redistribuible de OpenCV del SDK. Actualmente está disponible en Windows.

La latencia es cero: solo se usan fotogramas pasados para el suavizado, por lo que el bloque puede utilizarse en pipelines en directo.

## Características principales

- Estimación de movimiento global (traslación + rotación) mediante `goodFeaturesToTrack` + flujo óptico piramidal de Lucas-Kanade y un ajuste afín parcial con RANSAC.
- Suavizado causal de la trayectoria con un radio de ventana configurable, sin latencia de fotogramas añadida.
- Zoom central automático (`CropRatio`) para mantener los bordes ocultos.
- Límites de corrección por fotograma (`MaxShift`, `MaxAngle`) como limitadores de seguridad.
- Reajuste en vivo: cualquier propiedad puede cambiarse mientras el pipeline está en marcha.
- Modo de paso directo (`Enabled = false`) para una comparación instantánea de antes y después.

## Configuración

`VideoStabilizationSettings` se corresponde uno a uno con las propiedades del elemento:

| Propiedad | Tipo | Predeterminado | Descripción |
|-----------|------|----------------|-------------|
| `Enabled` | `bool` | `true` | Cuando es `false`, el bloque deja pasar los fotogramas sin modificar. |
| `SmoothingRadius` | `int` | `15` | Número de fotogramas pasados promediados al suavizar la trayectoria de la cámara. Mayor = más estable y de reacción más lenta. Rango `1` - `1000`. |
| `CropRatio` | `double` | `0.9` | Fracción del fotograma que se mantiene visible. El fotograma se amplía por `1 / CropRatio` alrededor de su centro para ocultar los bordes expuestos. `1.0` desactiva el zoom. Rango `0.5` - `1.0`. |
| `MaxShift` | `int` | `100` | Corrección máxima de traslación por fotograma en píxeles (limitador de seguridad). Mínimo `0`. |
| `MaxAngle` | `double` | `15` | Corrección máxima de rotación por fotograma en grados (limitador de seguridad). Rango `0` - `90`. |

Todas las propiedades **se ajustan silenciosamente** a su rango al asignarlas: un valor fuera de rango se corrige, no se rechaza, así que vuelva a leer la propiedad si necesita saber qué valor se aplicó.

## Disponibilidad

`IsAvailable()` busca el elemento `vfdeshake` en el registro de GStreamer, así que llámelo **después** de inicializar el SDK:

```csharp
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Añada el paquete NuGet VisioForge.CrossPlatform.OpenCV.Windows.x64.
}
```

## Código de ejemplo - previsualizar un archivo estabilizado

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;

// Inicialice el SDK una vez al arrancar la aplicación antes de construir cualquier pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    Enabled = true,
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

// VideoView1 es un control VideoView de VisioForge en su formulario/ventana.
var renderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, renderer.Input);

await pipeline.StartAsync();
```

## Código de ejemplo - estabilizar y guardar en MP4

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Inicialice el SDK una vez al arrancar la aplicación antes de construir cualquier pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// El archivo se reproduce hasta el final y el pipeline finaliza el MP4 al llegar a EOS; use OnStop para saberlo.
// Para terminar la grabación ANTES, llame a StopAsync(): envía EOS para que el muxer escriba el átomo moov.
// Matar el proceso en su lugar deja un archivo que no se puede reproducir.
// await pipeline.StopAsync();
```

## Código de ejemplo - previsualizar y grabar la salida estabilizada

Para ver el vídeo estabilizado y grabarlo al mismo tiempo, divida la salida del estabilizador con un `TeeBlock`: una rama alimenta el renderizador y la otra el codificador H264 y el sumidero MP4.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Inicialice el SDK una vez al iniciar la aplicación, antes de construir cualquier pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
var renderer = new VideoRendererBlock(pipeline, VideoView1);
var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], renderer.Input);
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// El archivo se reproduce hasta el final y el pipeline finaliza el MP4 al llegar a EOS; use OnStop para saberlo.
// Para terminar la grabación ANTES, llame a StopAsync(): envía EOS para que el muxer escriba el átomo moov.
// Matar el proceso en su lugar deja un archivo que no se puede reproducir.
// await pipeline.StopAsync();
```

## Uso en los motores X

`VideoStabilizationBlock` implementa `IVideoProcessingBlock`, por lo que puede insertarse directamente en los motores X de alto nivel - `VideoCaptureCoreX` y `MediaPlayerCoreX` - mediante `Video_Processing_AddBlock()`. El motor conecta el bloque en su ruta de vídeo por usted, lo que significa que puede estabilizar una **cámara en directo** y no solo un archivo.

Añada el bloque **antes de que el motor construya su pipeline**. Para `VideoCaptureCoreX` eso significa antes de `StartAsync()`; para `MediaPlayerCoreX`, antes de `OpenAsync()`, porque el reproductor construye su grafo durante Open: un bloque añadido después se ignora (solo genera una advertencia en el registro).

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Inicialice el SDK una vez al arrancar la aplicación antes de crear cualquier motor.
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Añada el paquete NuGet VisioForge.CrossPlatform.OpenCV.Windows.x64.
    return;
}

// Enumere las cámaras.
var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
if (cameras.Length == 0)
{
    return;
}

// VideoView1 es un control VideoView de VisioForge en su formulario/ventana.
var core = new VideoCaptureCoreX(VideoView1);

// Este constructor elige un formato HD (o el mejor disponible) y la velocidad de fotogramas del dispositivo.
core.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[0]);

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

core.Video_Processing_AddBlock(stabilizer);

// StartAsync devuelve false si un bloque no se pudo construir; no lanza una excepción.
if (!await core.StartAsync())
{
    // Gestione el fallo: el redistribuible de OpenCV o la cámara pueden no estar disponibles.
}
```

`ApplySettings()` funciona exactamente igual dentro de un motor: cambie los valores en `stabilizer.Settings` y llame a `stabilizer.ApplySettings()` para reajustar el efecto mientras la cámara sigue funcionando.

!!! warning "El motor es el propietario del bloque"

    Cuando pasa un bloque a `Video_Processing_AddBlock()`, el motor pasa a ser su propietario: lo construye al iniciar y **lo libera cuando la captura o la reproducción se detiene**. No lo libere usted mismo y no reutilice la misma instancia para una segunda sesión: cree un bloque nuevo en cada inicio. `ApplySettings()` se puede llamar en cualquier momento (no hace nada una vez que el bloque ha sido liberado).

## Reajuste en vivo

El elemento `vfdeshake` vuelve a leer sus propiedades en cada fotograma. Cambie cualquier valor del objeto `Settings` y llame a `ApplySettings()` para reajustar el efecto sin reconstruir el pipeline:

```csharp
stabilizer.Settings.Enabled = false;      // conmutación instantánea antes/después
stabilizer.Settings.SmoothingRadius = 30; // más estable
stabilizer.Settings.CropRatio = 0.85;     // amplía un poco más
stabilizer.ApplySettings();
```

## Consejos

- Aumente `SmoothingRadius` para obtener resultados más estables ante un temblor continuo; redúzcalo si la estabilización reacciona con demasiada lentitud a los movimientos intencionados de la cámara.
- Reduzca `CropRatio` (por ejemplo `0.85`) cuando el temblor sea grande, de modo que la compensación más amplia quede oculta tras el zoom. Los valores más altos conservan más fotograma pero pueden exponer los bordes.
- La estimación de movimiento global funciona mejor cuando la escena está dominada por el movimiento de la cámara. Las escenas con grandes objetos en primer plano en movimiento son más difíciles de estabilizar.

## Demos

- **[Demo de estabilización de vídeo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Demo)** - pipeline de Media Blocks: abra un archivo, previsualice la reproducción estabilizada, grábela opcionalmente en MP4 y ajuste la configuración en vivo.
- **[Captura con estabilización de vídeo X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Video%20Stabilization%20Capture%20X)** - estabilización de una cámara en directo con `VideoCaptureCoreX` y `Video_Processing_AddBlock()`.
- **[Demo de estabilización de cámara](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Camera%20Demo)** - la misma cámara en directo, pero construida a mano como un `MediaBlocksPipeline`: fuente de cámara -> estabilizador -> renderizador, con ajuste en vivo.
