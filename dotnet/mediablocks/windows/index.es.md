---
title: Decodificación de video por GPU con Direct3D 11 en C# .NET
description: Use decodificación de video acelerada por hardware con Direct3D 11, efectos e integración con DirectShow en Windows con VisioForge Media Blocks SDK para .NET.
sidebar_label: Windows Platform
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - D3D11H264DecoderBlock
  - D3D11UploadBlock
  - D3D11DownloadBlock
  - UniversalSourceBlock
  - UniversalSourceSettings

---

# Bloques de plataforma Windows - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta sección cubre los MediaBlocks específicamente optimizados para plataformas Windows.

## Bloques disponibles

### Decodificadores de hardware Direct3D 11

Windows proporciona decodificación de video acelerada por hardware mediante Direct3D 11:

- **D3D11H264DecoderBlock**: decodificación H.264/AVC por hardware
- **D3D11H265DecoderBlock**: decodificación H.265/HEVC por hardware
- **D3D11VP8DecoderBlock**: decodificación VP8 por hardware
- **D3D11VP9DecoderBlock**: decodificación VP9 por hardware
- **D3D11AV1DecoderBlock**: decodificación AV1 por hardware
- **D3D11MPEG2DecoderBlock**: decodificación MPEG-2 por hardware

Consulte la [documentación de decodificadores de video](../VideoDecoders/index.md)

### Procesamiento Direct3D 11

- **D3D11UploadBlock**: sube video desde la memoria del sistema a la GPU
- **D3D11DownloadBlock**: descarga video desde la GPU a la memoria del sistema
- **D3D11VideoConverterBlock**: conversión de espacio de color acelerada por GPU

Consulte la [documentación de procesamiento de video](../VideoProcessing/index.md#d3d11-video-converter)

### Composición Direct3D 11

- **D3D11VideoCompositorBlock**: composición y mezcla de video acelerada por GPU

### Efectos de video de Windows

- **VideoEffectsWinBlock**: efectos de video específicos de Windows
- **VR360ProcessorBlock**: procesamiento de video VR 360 grados

### Bloques especiales

Consulte la [documentación de bloques especiales](../Special/index.md)

## Requisitos de plataforma

- **Windows**: Windows 7 SP1 o superior
- **Direct3D 11**: GPU compatible con D3D11
- **Decodificación por hardware**: GPU con soporte de decodificación de video por hardware

## Características

- **Aceleración por hardware**: aproveche la GPU para codificación, decodificación y procesamiento
- **Integración con Direct3D 11**: procesamiento de video eficiente en la GPU
- **Bajo uso de CPU**: descarga el procesamiento al hardware dedicado
- **Alto rendimiento**: gestiona simultáneamente varios flujos HD/4K
- **Eficiencia energética**: reduce el consumo de energía con aceleración por hardware

## Código de ejemplo

### Decodificación H.264 por hardware

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// Decodificador D3D11 por hardware
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Decoder.Input);

// Procesar en GPU o descargar a la memoria del sistema
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Decoder.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Pipeline de procesamiento de video en GPU

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// Subir a la GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// Conversión de color en GPU
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Descargar desde la GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Composición de video

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video1.mp4")));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video2.mp4")));

// Subir ambos a la GPU
var upload1 = new D3D11UploadBlock();
var upload2 = new D3D11UploadBlock();
pipeline.Connect(source1.VideoOutput, upload1.Input);
pipeline.Connect(source2.VideoOutput, upload2.Input);

// Configurar el compositor — dos flujos en un canvas 1920x1080 @ 30 fps
var compositorSettings = new D3D11VideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(0, 0, 960, 1080), zorder: 0));
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(960, 0, 1920, 1080), zorder: 1));

// Componer en GPU — cada flujo añadido crea un pad de entrada; acceda mediante Inputs[].
var compositor = new D3D11VideoCompositorBlock(compositorSettings);
pipeline.Connect(upload1.Output, compositor.Inputs[0]);
pipeline.Connect(upload2.Output, compositor.Inputs[1]);

// Descargar resultado
var download = new D3D11DownloadBlock();
pipeline.Connect(compositor.Output, download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Consejos de rendimiento

- Mantenga el video en memoria GPU entre operaciones para evitar el coste de upload/download
- Use decodificadores por hardware cuando estén disponibles para mejor rendimiento
- Encadene operaciones en GPU antes de descargar a la memoria del sistema
- Monitorice el uso de memoria GPU al procesar varios flujos
- Compruebe el soporte de hardware antes de usar bloques D3D11

## Comprobar el soporte de hardware

```csharp
// Comprobar si el decodificador D3D11 H.264 está disponible
if (D3D11H264DecoderBlock.IsAvailable())
{
    // Usar decodificador por hardware
    var decoder = new D3D11H264DecoderBlock();
}
else
{
    // Recurrir a decodificador por software
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Plataformas

Windows 7 SP1 o superior (Windows 10/11 recomendado para mejor soporte de hardware).

## Documentación relacionada

- [VideoDecoders](../VideoDecoders/index.md) — bloques de decodificación por hardware
- [VideoProcessing](../VideoProcessing/index.md) — bloques de procesamiento por GPU
- [Special](../Special/index.md) — utilidades específicas de plataforma
