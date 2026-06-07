---
title: Bloques de plataforma Apple en C# .NET — iOS, macOS
description: Cree aplicaciones multimedia para iOS y macOS con codificación ProRes, aceleración VideoToolbox y bloques de audio nativos con VisioForge Media Blocks SDK.
sidebar_label: Apple Platform
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - MetalVideoCompositorBlock
  - MediaBlocksPipeline
  - VideoRendererBlock
  - OSXAudioSourceBlock
  - OSXAudioSinkBlock

---

# Bloques de plataforma Apple - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre los MediaBlocks específicamente optimizados para plataformas Apple (iOS, macOS, tvOS).

## Bloques disponibles

### Fuentes de audio

- **OSXAudioSourceBlock**: captura de audio en macOS mediante Core Audio
  - Consulte la [documentación de fuentes de audio](../Sources/index.md#system-audio-source)
  
- **IOSAudioSourceBlock**: captura de audio en iOS
  - Consulte la [documentación de fuentes de audio](../Sources/index.md#system-audio-source)

### Salidas de audio

- **OSXAudioSinkBlock**: reproducción de audio en macOS
  - Consulte la [documentación de renderizado de audio](../AudioRendering/index.md)
  
- **IOSAudioSinkBlock**: reproducción de audio en iOS
  - Consulte la [documentación de renderizado de audio](../AudioRendering/index.md)

### Fuentes de video

- **IOSVideoSourceBlock**: captura desde la cámara de iOS
  - Consulte la [documentación de fuentes de video](../Sources/index.md#system-video-source)

### Codificadores de video

- **AppleProResEncoderBlock**: códec profesional de video Apple ProRes
  - Consulte la [documentación del codificador ProRes](../VideoEncoders/index.md#apple-prores-encoder)

### Procesamiento de video

- **MetalVideoCompositorBlock**: compositor de video multi-entrada acelerado por GPU mediante Apple Metal
- **MetalConvertScaleBlock**: conversión de formato de píxel y escalado en una sola pasada de GPU mediante Apple Metal
- **MetalDeinterlaceBlock**: desentrelazado acelerado por GPU mediante Apple Metal
- **MetalOverlayBlock**: composición de imágenes superpuestas en GPU mediante Apple Metal
- **MetalTransformBlock**: operaciones de volteo, rotación y recorte en GPU mediante Apple Metal

## Metal Video Compositor

### Metal Video Compositor Block

El `MetalVideoCompositorBlock` compone varios flujos de video en tiempo real usando el framework de GPU Apple Metal. Cada flujo de entrada tiene posición, tamaño, z-order, alfa y operador de mezcla configurables. El bloque produce una única salida de video BGRA.

#### Información del bloque

Nombre: MetalVideoCompositorBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | Video sin comprimir | N (uno por flujo) |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `MetalVideoCompositorSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Width` | `int` | 1920 | Ancho de salida en píxeles |
| `Height` | `int` | 1080 | Alto de salida en píxeles |
| `FrameRate` | `VideoFrameRate` | FPS_30 | Tasa de fotogramas de salida |
| `Background` | `VideoMixerBackground` | Transparent | Modo de fondo |
| `Streams` | `List<VideoMixerStream>` | Vacío | Configuraciones de los flujos de entrada |

Cada flujo de entrada es un `MetalVideoMixerStream`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Rectangle` | `Rect` | requerido | Posición y tamaño dentro del fotograma de salida |
| `ZOrder` | `uint` | requerido | Orden de apilamiento (mayor = al frente) |
| `Alpha` | `double` | 1.0 | Opacidad (0.0 transparente – 1.0 opaco) |
| `BlendOperator` | `MetalVideoMixerBlendOperator` | Over | Modo de mezcla: Source, Over o Add |
| `KeepAspectRatio` | `bool` | false | Preservar la relación de aspecto de la fuente durante el escalado |

#### El pipeline de ejemplo

```mermaid
graph LR;
    VideoSource1-->MetalVideoCompositorBlock;
    VideoSource2-->MetalVideoCompositorBlock;
    MetalVideoCompositorBlock-->VideoRendererBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurar el compositor: 1920x1080 @ 30 fps
var settings = new MetalVideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);

// Primer flujo: mitad izquierda de la pantalla
settings.AddStream(new MetalVideoMixerStream(
    rectangle: new Rect(0, 0, 960, 1080),
    zorder: 0));

// Segundo flujo: mitad derecha de la pantalla
// El constructor de Rect es (left, top, right, bottom). Para la mitad derecha de
// un canvas 1920x1080 use right=1920 y bottom=1080 — la forma anterior
// (960, 0, 960, 1080) tiene left==right y produce una caja de ancho cero.
settings.AddStream(new MetalVideoMixerStream(
    rectangle: new Rect(960, 0, 1920, 1080),
    zorder: 1));

var compositor = new MetalVideoCompositorBlock(settings);

// Renderizar la salida compuesta
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(compositor.Output, videoRenderer.Input);

await pipeline.StartAsync();

// En tiempo real: desvanecer el flujo 0 durante 2 segundos
compositor.StartFadeOut(settings.Streams[0].ID, TimeSpan.FromSeconds(2));
```

#### Disponibilidad

```csharp
bool available = MetalVideoCompositorBlock.IsAvailable();
```

Devuelve `true` si el plugin GStreamer `vfmetalcompositor` está disponible en el sistema actual.

#### Plataformas

macOS, iOS.

## Metal Convert/Scale

### Metal Convert/Scale Block

El `MetalConvertScaleBlock` realiza la conversión de formato de píxel y el escalado en una sola pasada de GPU usando el framework Apple Metal. Opcionalmente puede añadir bordes de letterbox/pillarbox para preservar la relación de aspecto de la fuente.

#### Información del bloque

Nombre: MetalConvertScaleBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | Video sin comprimir | 1 |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `MetalConvertScaleSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Method` | `MetalScalingMethod` | Bilinear | Interpolación de escalado: `Bilinear` o `Nearest` |
| `AddBorders` | `bool` | false | Añadir bordes de letterbox/pillarbox para preservar la relación de aspecto |
| `BorderColor` | `uint` | 0xFF000000 | Color del borde en formato ARGB (negro opaco) |

#### El pipeline de ejemplo

```mermaid
graph LR;
    VideoSource-->MetalConvertScaleBlock;
    MetalConvertScaleBlock-->VideoRendererBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new IOSVideoSourceBlock(videoSettings);

// Escalado bilineal con bordes de letterbox
var settings = new MetalConvertScaleSettings
{
    Method = MetalScalingMethod.Bilinear,
    AddBorders = true,
    BorderColor = 0xFF000000
};
var convertScale = new MetalConvertScaleBlock(settings);
pipeline.Connect(videoSource.Output, convertScale.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(convertScale.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = MetalConvertScaleBlock.IsAvailable();
```

Devuelve `true` si el plugin GStreamer `vfmetalconvertscale` está disponible en el sistema actual.

#### Plataformas

macOS, iOS.

## Metal Deinterlace

### Metal Deinterlace Block

El `MetalDeinterlaceBlock` elimina los artefactos de entrelazado en la GPU usando el framework Apple Metal. Admite los algoritmos bob, weave, lineal y greedy-H (adaptativo al movimiento).

#### Información del bloque

Nombre: MetalDeinterlaceBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | Video sin comprimir | 1 |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `MetalDeinterlaceSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Method` | `MetalDeinterlaceMethod` | Bob | Algoritmo: `Bob`, `Weave`, `Linear` o `GreedyH` |
| `FieldLayout` | `MetalDeinterlaceFieldLayout` | Auto | Orden de campos: `Auto`, `TopFieldFirst` o `BottomFieldFirst` |
| `MotionThreshold` | `double` | 0.1 | Umbral de detección de movimiento para el algoritmo greedy-H (0.0–1.0) |

#### El pipeline de ejemplo

```mermaid
graph LR;
    VideoSource-->MetalDeinterlaceBlock;
    MetalDeinterlaceBlock-->VideoRendererBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new IOSVideoSourceBlock(videoSettings);

// Desentrelazado greedy-H adaptativo al movimiento
var settings = new MetalDeinterlaceSettings
{
    Method = MetalDeinterlaceMethod.GreedyH,
    FieldLayout = MetalDeinterlaceFieldLayout.Auto,
    MotionThreshold = 0.1
};
var deinterlace = new MetalDeinterlaceBlock(settings);
pipeline.Connect(videoSource.Output, deinterlace.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(deinterlace.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = MetalDeinterlaceBlock.IsAvailable();
```

Devuelve `true` si el plugin GStreamer `vfmetaldeinterlace` está disponible en el sistema actual.

#### Plataformas

macOS, iOS.

## Metal Overlay

### Metal Overlay Block

El `MetalOverlayBlock` compone una imagen PNG o JPEG sobre los fotogramas de video en la GPU usando el framework Apple Metal. La superposición puede posicionarse en píxeles absolutos o como una fracción del tamaño del fotograma, con opacidad ajustable.

#### Información del bloque

Nombre: MetalOverlayBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | Video sin comprimir | 1 |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `MetalOverlaySettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Location` | `string` | null | Ruta a la imagen de superposición (PNG o JPEG); null deshabilita la superposición |
| `X` | `int` | 0 | Posición X en píxeles (ignorada cuando `RelativeX` no es -1) |
| `Y` | `int` | 0 | Posición Y en píxeles (ignorada cuando `RelativeY` no es -1) |
| `Width` | `int` | 0 | Ancho de la superposición en píxeles (0 = ancho de la imagen original) |
| `Height` | `int` | 0 | Alto de la superposición en píxeles (0 = alto de la imagen original) |
| `Alpha` | `double` | 1.0 | Opacidad (0.0 transparente – 1.0 opaco) |
| `RelativeX` | `double` | -1.0 | X relativa como fracción del ancho del video; -1.0 usa el `X` en píxeles |
| `RelativeY` | `double` | -1.0 | Y relativa como fracción del alto del video; -1.0 usa el `Y` en píxeles |

#### El pipeline de ejemplo

```mermaid
graph LR;
    VideoSource-->MetalOverlayBlock;
    MetalOverlayBlock-->VideoRendererBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new IOSVideoSourceBlock(videoSettings);

// Logo en la esquina superior izquierda con 80% de opacidad
var settings = new MetalOverlaySettings
{
    Location = "logo.png",
    X = 20,
    Y = 20,
    Alpha = 0.8
};
var overlay = new MetalOverlayBlock(settings);
pipeline.Connect(videoSource.Output, overlay.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(overlay.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = MetalOverlayBlock.IsAvailable();
```

Devuelve `true` si el plugin GStreamer `vfmetaloverlay` está disponible en el sistema actual.

#### Plataformas

macOS, iOS.

## Metal Transform

### Metal Transform Block

El `MetalTransformBlock` aplica operaciones de volteo, rotación y recorte en la GPU usando el framework Apple Metal.

#### Información del bloque

Nombre: MetalTransformBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | Video sin comprimir | 1 |
| Salida de video | Video sin comprimir | 1 |

#### Configuración

El bloque acepta una instancia de `MetalTransformSettings`:

| Propiedad | Tipo | Predeterminado | Descripción |
| --- | --- | :---: | --- |
| `Method` | `MetalTransformMethod` | None | Rotación/volteo: `None`, `Clockwise`, `Rotate180`, `CounterClockwise`, `HorizontalFlip`, `VerticalFlip`, `UpperLeftDiagonal` o `UpperRightDiagonal` |
| `CropTop` | `int` | 0 | Píxeles a recortar desde arriba |
| `CropBottom` | `int` | 0 | Píxeles a recortar desde abajo |
| `CropLeft` | `int` | 0 | Píxeles a recortar desde la izquierda |
| `CropRight` | `int` | 0 | Píxeles a recortar desde la derecha |

#### El pipeline de ejemplo

```mermaid
graph LR;
    VideoSource-->MetalTransformBlock;
    MetalTransformBlock-->VideoRendererBlock;
```

#### Código de ejemplo

```csharp
var pipeline = new MediaBlocksPipeline();

var videoSource = new IOSVideoSourceBlock(videoSettings);

// Rotar 90 grados en sentido horario y recortar 10px de cada lado
var settings = new MetalTransformSettings
{
    Method = MetalTransformMethod.Clockwise,
    CropLeft = 10,
    CropRight = 10
};
var transform = new MetalTransformBlock(settings);
pipeline.Connect(videoSource.Output, transform.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(transform.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

```csharp
bool available = MetalTransformBlock.IsAvailable();
```

Devuelve `true` si el plugin GStreamer `vfmetaltransform` está disponible en el sistema actual.

#### Plataformas

macOS, iOS.

## Requisitos de plataforma

- **iOS**: iOS 12.0 o superior
- **macOS**: macOS 10.13 o superior
- **tvOS**: tvOS 12.0 o superior

## Características

- Integración nativa con los frameworks de Apple (AVFoundation, Core Audio, Core Video)
- Procesamiento acelerado por hardware en Apple Silicon y Macs Intel
- Optimizado para bajo consumo en dispositivos móviles
- Soporte para codificación ProRes de alta calidad
- Integración con los permisos de cámara y micrófono de iOS

## Código de ejemplo

### Captura desde la cámara de iOS

```csharp
var pipeline = new MediaBlocksPipeline();

// Fuente de video iOS
var videoSource = new IOSVideoSourceBlock(videoSettings);

// Procesar y mostrar
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Captura y reproducción de audio en macOS

```csharp
var pipeline = new MediaBlocksPipeline();

// Fuente de audio macOS
var audioSource = new OSXAudioSourceBlock(audioSettings);

// Salida de audio macOS
var audioSink = new OSXAudioSinkBlock();
pipeline.Connect(audioSource.Output, audioSink.Input);

await pipeline.StartAsync();
```

### Codificación ProRes

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("input.mp4"));

// Codificador Apple ProRes
// AppleProResEncoderSettings expone Quality (double 0.0-1.0), Bitrate, MaxKeyframeInterval,
// MaxKeyFrameIntervalDuration, AllowFrameReordering, PreserveAlpha, Realtime — no es un enum de perfiles.
var proresSettings = new AppleProResEncoderSettings
{
    Quality = 0.8
};
var proresEncoder = new AppleProResEncoderBlock(proresSettings);
pipeline.Connect(fileSource.VideoOutput, proresEncoder.Input);

// Salida a archivo MOV
var movSink = new MOVSinkBlock(new MOVSinkSettings("output.mov"));
pipeline.Connect(proresEncoder.Output, movSink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

## Plataformas

iOS, macOS, tvOS.

## Documentación relacionada

- [Sources](../Sources/index.md) — todos los bloques de fuente, incluidos los específicos de Apple
- [VideoEncoders](../VideoEncoders/index.md) — codificación de video, incluida ProRes
- [AudioRendering](../AudioRendering/index.md) — reproducción de audio
