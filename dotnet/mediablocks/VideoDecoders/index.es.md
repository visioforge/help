---
title: Decodificadores de Video en C# .NET - H.264, HEVC, AV1
description: Decodifique H.264, HEVC, VP9, AV1 y MJPEG con aceleración por hardware usando VisioForge Media Blocks SDK. Decodificación GPU para .NET.
sidebar_label: Decodificadores de Video
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - UniversalDemuxBlock
  - VideoRendererBlock
  - BasicFileSourceBlock
  - MediaBlocksPipeline
  - MediaInfoReaderX
---

# Bloques Decodificadores de Video - SDK de VisioForge Media Blocks .Net

[SDK de Media Blocks .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Los bloques Decodificadores de Video son componentes esenciales en un pipeline de medios, responsables de descomprimir streams de video codificados en frames de video raw que pueden procesarse o renderizarse posteriormente. El SDK de VisioForge Media Blocks .Net ofrece una variedad de bloques decodificadores de video soportando numerosos códecs y tecnologías de aceleración de hardware.

## Bloques Decodificadores de Video Disponibles

### Bloque Decodificador H264

Decodifica streams de video H.264 (AVC). Este es uno de los estándares de compresión de video más utilizados. El bloque puede utilizar diferentes implementaciones de decodificador subyacentes como FFMPEG, OpenH264, o decodificadores acelerados por hardware si están disponibles.

#### Información del bloque

Nombre: `H264DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video H.264 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### Ajustes

El `H264DecoderBlock` está configurado usando ajustes que implementan `IH264DecoderSettings`. Las clases de ajustes disponibles incluyen:
- `FFMPEGH264DecoderSettings`
- `OpenH264DecoderSettings`
- `NVH264DecoderSettings` (para aceleración GPU NVIDIA)
- `VAAPIH264DecoderSettings` (para aceleración VA-API en Linux)

Un constructor sin parámetros intentará seleccionar un decodificador disponible automáticamente.

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.264 --> H264DecoderBlock;
    H264DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador H264
var h264Decoder = new H264DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

// Obtener información de medios usando MediaInfoReaderX
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Puede verificar implementaciones de decodificador específicas usando `H264DecoderBlock.IsAvailable(H264DecoderType decoderType)`.
`H264DecoderType` incluye `FFMPEG`, `OpenH264`, `GPU_Nvidia_H264`, `VAAPI_H264`, etc.

#### Plataformas

Windows, macOS, Linux. (Decodificadores específicos de hardware como NVH264Decoder requieren hardware y drivers específicos).

### Bloque Decodificador JPEG

Decodifica streams de video JPEG (Motion JPEG) o imágenes JPEG individuales en frames de video raw.

#### Información del bloque

Nombre: `JPEGDecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video/imágenes JPEG codificadas | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    HTTPSourceBlock -- Stream MJPEG --> JPEGDecoderBlock;
    JPEGDecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador JPEG
var jpegDecoder = new JPEGDecoderBlock();

// Ejemplo: Crear fuente HTTP para cámara MJPEG y renderizador de video
var httpSettings = new HTTPSourceSettings(new Uri("http://mjpegcamera:8080"));
var httpSource = new HTTPSourceBlock(httpSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Asumiendo VideoView1

// Conectar bloques
pipeline.Connect(httpSource.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Puede verificar si el decodificador NVIDIA JPEG subyacente (si aplica) está disponible usando `NVJPEGDecoderBlock.IsAvailable()`. La funcionalidad genérica de decodificador JPEG generalmente está disponible.

#### Plataformas

Windows, macOS, Linux. (Implementación específica de NVIDIA requiere hardware NVIDIA).

### Bloque Decodificador H.264 NVIDIA (NVH264DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video H.264 (AVC) usando tecnología NVDEC de NVIDIA. Esto ofrece alto rendimiento y eficiencia en sistemas con GPUs NVIDIA compatibles.

#### Información del bloque

Nombre: `NVH264DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video H.264 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.264 --> NVH264DecoderBlock;
    NVH264DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador H.264 NVIDIA
var nvH264Decoder = new NVH264DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH264Decoder.Input);
pipeline.Connect(nvH264Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVH264DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador H.265 NVIDIA (NVH265DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video H.265 (HEVC) usando tecnología NVDEC de NVIDIA. H.265 ofrece eficiencia de compresión mejorada en comparación con H.264.

#### Información del bloque

Nombre: `NVH265DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video H.265/HEVC codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.265 --> NVH265DecoderBlock;
    NVH265DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador H.265 NVIDIA
var nvH265Decoder = new NVH265DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH265Decoder.Input);
pipeline.Connect(nvH265Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVH265DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para H.265 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador JPEG NVIDIA (NVJPEGDecoderBlock)

Proporciona decodificación acelerada por hardware de imágenes JPEG o streams Motion JPEG (MJPEG) usando biblioteca NVJPEG de NVIDIA. Esto es particularmente útil para streams MJPEG de alta resolución o alta tasa de frames.
(Nota: El pipeline de muestra para JPEG con BasicFileSourceBlock podría no ser común en comparación con HTTPSource para MJPEG. El ejemplo a continuación está adaptado pero considere casos de uso típicos.)

#### Información del bloque

Nombre: `NVJPEGDecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video/imágenes JPEG codificadas | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw MJPEG --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video JPEG --> NVJPEGDecoderBlock;
    NVJPEGDecoderBlock -- Video Decodificado --> VideoRendererBlock;
```
Para streams MJPEG en vivo, `HTTPSourceBlock --> NVJPEGDecoderBlock` es más típico.

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador JPEG NVIDIA
var nvJpegDecoder = new NVJPEGDecoderBlock();

// Ejemplo: Crear fuente de archivo básica para archivo MJPEG, demuxer, y renderizador
// Asegurar que "test.mjpg" contenga un stream Motion JPEG.
var basicFileSource = new BasicFileSourceBlock("test.mjpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.mjpg");
var mediaInfo = reader.Info;
if (mediaInfo == null || mediaInfo.VideoStreams.Count == 0 || !mediaInfo.VideoStreams[0].Codec.Contains("jpeg"))
{
    Console.WriteLine("Falló al obtener información de medios MJPEG o no es un archivo MJPEG.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvJpegDecoder.Input);
pipeline.Connect(nvJpegDecoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVJPEGDecoderBlock.IsAvailable()`. Requiere GPU NVIDIA y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador MPEG-1 NVIDIA (NVMPEG1DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video MPEG-1 usando tecnología NVDEC de NVIDIA.

#### Información del bloque

Nombre: `NVMPEG1DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video MPEG-1 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video MPEG-1 --> NVMPEG1DecoderBlock;
    NVMPEG1DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador MPEG-1 NVIDIA
var nvMpeg1Decoder = new NVMPEG1DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_mpeg1.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg1.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg1Decoder.Input);
pipeline.Connect(nvMpeg1Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVMPEG1DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para MPEG-1 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador MPEG-2 NVIDIA (NVMPEG2DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video MPEG-2 usando tecnología NVDEC de NVIDIA. Comúnmente usado para video DVD y algunas transmisiones digitales de televisión.

#### Información del bloque

Nombre: `NVMPEG2DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video MPEG-2 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video MPEG-2 --> NVMPEG2DecoderBlock;
    NVMPEG2DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador MPEG-2 NVIDIA
var nvMpeg2Decoder = new NVMPEG2DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg2Decoder.Input);
pipeline.Connect(nvMpeg2Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVMPEG2DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para MPEG-2 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador MPEG-4 NVIDIA (NVMPEG4DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video MPEG-4 Part 2 (a menudo encontrados en archivos AVI, ej. DivX/Xvid) usando tecnología NVDEC de NVIDIA. Nota que esto es diferente de MPEG-4 Part 10 (H.264/AVC).

#### Información del bloque

Nombre: `NVMPEG4DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video MPEG-4 Part 2 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video MPEG-4 --> NVMPEG4DecoderBlock;
    NVMPEG4DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador MPEG-4 NVIDIA
var nvMpeg4Decoder = new NVMPEG4DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_mpeg4.avi");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg4.avi");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg4Decoder.Input);
pipeline.Connect(nvMpeg4Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVMPEG4DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para MPEG-4 Part 2 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador VP8 NVIDIA (NVVP8DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video VP8 usando tecnología NVDEC de NVIDIA. VP8 es un formato de video abierto, a menudo usado con WebM.

#### Información del bloque

Nombre: `NVVP8DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video VP8 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP8 --> NVVP8DecoderBlock;
    NVVP8DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador VP8 NVIDIA
var nvVp8Decoder = new NVVP8DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp8Decoder.Input);
pipeline.Connect(nvVp8Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVVP8DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para VP8 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador VP9 NVIDIA (NVVP9DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video VP9 usando tecnología NVDEC de NVIDIA. VP9 es un estándar de codificación de video abierto y sin royalties desarrollado por Google, a menudo usado para streaming web (ej. YouTube).

#### Información del bloque

Nombre: `NVVP9DecoderBlock`.

| Dirección del pin | Tipo de medio | Conteo de pines |
| --- | :---: | :---: |
| Entrada de video | video VP9 codificado | 1 |
| Salida de video | video sin comprimir | 1 |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP9 --> NVVP9DecoderBlock;
    NVVP9DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador VP9 NVIDIA
var nvVp9Decoder = new NVVP9DecoderBlock();

// Ejemplo: Crear fuente de archivo básica, demuxer, y renderizador
var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Conectar bloques
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp9Decoder.Input);
pipeline.Connect(nvVp9Decoder.Output, videoRenderer.Input);

// Iniciar pipeline
await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVVP9DecoderBlock.IsAvailable()`. Requiere GPU NVIDIA que soporte NVDEC para VP9 y drivers apropiados.

#### Plataformas

Windows, Linux (con drivers NVIDIA).

### Bloque Decodificador H.264 VAAPI (VAAPIH264DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video H.264 (AVC) usando VA-API (Video Acceleration API). Disponible en sistemas Linux con hardware y drivers compatibles.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video H.264 codificado  | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.264 --> VAAPIH264DecoderBlock;
    VAAPIH264DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiH264Decoder = new VAAPIH264DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiH264Decoder.Input);
pipeline.Connect(vaapiH264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar con `VAAPIH264DecoderBlock.IsAvailable()`. Requiere soporte VA-API y redist SDK correcto.

#### Plataformas

Linux (con drivers VA-API).

---

### Bloque Decodificador HEVC VAAPI (VAAPIHEVCDecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video H.265 (HEVC) usando VA-API. Disponible en sistemas Linux con hardware y drivers compatibles.

#### Información del bloque

| Dirección del pin | Tipo de medio            | Conteo de pines |
|-------------------|--------------------------|-----------------|
| Entrada de video  | video H.265/HEVC codificado | 1               |
| Salida de video   | video sin comprimir       | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.265 --> VAAPIHEVCDecoderBlock;
    VAAPIHEVCDecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiHevcDecoder = new VAAPIHEVCDecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiHevcDecoder.Input);
pipeline.Connect(vaapiHevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar con `VAAPIHEVCDecoderBlock.IsAvailable()`. Requiere soporte VA-API y redist SDK correcto.

#### Plataformas

Linux (con drivers VA-API).

---

### Bloque Decodificador JPEG VAAPI (VAAPIJPEGDecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video JPEG/MJPEG usando VA-API. Disponible en sistemas Linux con hardware y drivers compatibles.

#### Información del bloque

| Dirección del pin | Tipo de medio                | Conteo de pines |
|-------------------|------------------------------|-----------------|
| Entrada de video  | video/imágenes JPEG codificadas | 1               |
| Salida de video   | video sin comprimir          | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    HTTPSourceBlock -- Stream MJPEG --> VAAPIJPEGDecoderBlock;
    VAAPIJPEGDecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiJpegDecoder = new VAAPIJPEGDecoderBlock();
var httpSettings = new HTTPSourceSettings(new Uri("http://your-mjpeg-camera/stream"));
var httpSource = new HTTPSourceBlock(httpSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(httpSource.Output, vaapiJpegDecoder.Input);
pipeline.Connect(vaapiJpegDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar con `VAAPIJPEGDecoderBlock.IsAvailable()`. Requiere soporte VA-API y redist SDK correcto.

#### Plataformas

Linux (con drivers VA-API).

---

### Bloque Decodificador VC1 VAAPI (VAAPIVC1DecoderBlock)

Proporciona decodificación acelerada por hardware de streams de video VC-1 usando VA-API. Disponible en sistemas Linux con hardware y drivers compatibles.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video VC-1 codificado   | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VC-1 --> VAAPIVC1DecoderBlock;
    VAAPIVC1DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiVc1Decoder = new VAAPIVC1DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_vc1.wmv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vc1.wmv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiVc1Decoder.Input);
pipeline.Connect(vaapiVc1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar con `VAAPIVC1DecoderBlock.IsAvailable()`. Requiere soporte VA-API y redist SDK correcto.

#### Plataformas

Linux (con drivers VA-API).

---

## Bloques Decodificadores de Video Direct3D 11/DXVA

Los bloques decodificadores Direct3D 11/DXVA proporcionan decodificación acelerada por hardware en sistemas Windows con GPUs y drivers compatibles. Estos bloques son útiles para pipelines de reproducción y procesamiento de video de alto rendimiento en Windows.

### Bloque Decodificador AV1 D3D11

Decodifica streams de video AV1 usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11AV1DecoderBlock`.

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video AV1 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video AV1 --> D3D11AV1DecoderBlock;
    D3D11AV1DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador AV1 D3D11
var d3d11Av1Decoder = new D3D11AV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_av1.mkv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_av1.mkv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Av1Decoder.Input);
pipeline.Connect(d3d11Av1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11AV1DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (D3D11/DXVA requerido).

---

### Bloque Decodificador H.264 D3D11

Decodifica streams de video H.264 (AVC) usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11H264DecoderBlock`.

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video H.264 codificado  | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.264 --> D3D11H264DecoderBlock;
    D3D11H264DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador H.264 D3D11
var d3d11H264Decoder = new D3D11H264DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H264Decoder.Input);
pipeline.Connect(d3d11H264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11H264DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (D3D11/DXVA requerido).

---

### Bloque Decodificador H.265 D3D11

Decodifica streams de video H.265 (HEVC) usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11H265DecoderBlock`.

| Dirección del pin | Tipo de medio            | Conteo de pines |
|-------------------|--------------------------|-----------------|
| Entrada de video  | video H.265/HEVC codificado | 1               |
| Salida de video   | video sin comprimir       | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video H.265 --> D3D11H265DecoderBlock;
    D3D11H265DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador H.265 D3D11
var d3d11H265Decoder = new D3D11H265DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H265Decoder.Input);
pipeline.Connect(d3d11H265Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11H265DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (D3D11/DXVA requerido).

---

### Bloque Decodificador MPEG-2 D3D11

Decodifica streams de video MPEG-2 usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11MPEG2DecoderBlock`.

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video MPEG-2 codificado | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video MPEG-2 --> D3D11MPEG2DecoderBlock;
    D3D11MPEG2DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador MPEG-2 D3D11
var d3d11Mpeg2Decoder = new D3D11MPEG2DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Mpeg2Decoder.Input);
pipeline.Connect(d3d11Mpeg2Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11MPEG2DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (D3D11/DXVA requerido).

---

### Bloque Decodificador VP8 D3D11

Decodifica streams de video VP8 usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11VP8DecoderBlock`.

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video VP8 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP8 --> D3D11VP8DecoderBlock;
    D3D11VP8DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador VP8 D3D11
var d3d11Vp8Decoder = new D3D11VP8DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp8Decoder.Input);
pipeline.Connect(d3d11Vp8Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11VP8DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (D3D11/DXVA requerido).

---

### Bloque Decodificador VP9 D3D11

Decodifica streams de video VP9 usando aceleración hardware Direct3D 11/DXVA.

#### Información del bloque

Nombre: `D3D11VP9DecoderBlock`.

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video VP9 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP9 --> D3D11VP9DecoderBlock;
    D3D11VP9DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear bloque Decodificador VP9 D3D11
var d3d11Vp9Decoder = new D3D11VP9DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp9Decoder.Input);
pipeline.Connect(d3d11Vp9Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `D3D11VP9DecoderBlock.IsAvailable()`. Requiere Windows con soporte D3D11/DXVA y redist SDK correcto.

#### Plataformas

Windows (se requiere D3D11/DXVA).

---

### Bloque Decodificador HEVC

El `HEVCDecoderBlock` decodifica streams de video H.265/HEVC con selección automática del mejor backend disponible: NVIDIA NVDEC → Intel Quick Sync (QSV) → AMD AMF → D3D11/DXVA (Windows) → VAAPI (Linux) → FFmpeg (software). También se puede seleccionar un backend explícito mediante las clases de configuración correspondientes.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video HEVC codificado   | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### Configuración

**Selección automática (recomendado):**

```csharp
var hevcDecoder = new HEVCDecoderBlock();
```

**Selección explícita de backend:**

```csharp
// FFmpeg (CPU, software)
var hevcDecoder = new HEVCDecoderBlock(new FFMPEGHEVCDecoderSettings());

// NVIDIA NVDEC (GPU)
var hevcDecoder = new HEVCDecoderBlock(new NVHEVCDecoderSettings());

// Intel Quick Sync
var hevcDecoder = new HEVCDecoderBlock(new QSVHEVCDecoderSettings());

// AMD AMF
var hevcDecoder = new HEVCDecoderBlock(new AMFHEVCDecoderSettings());

// D3D11/DXVA (solo Windows)
var hevcDecoder = new HEVCDecoderBlock(new D3D11HEVCDecoderSettings());

// VAAPI (solo Linux)
var hevcDecoder = new HEVCDecoderBlock(new VAAPIHEVCDecoderSettings());
```

Todas las clases de configuración implementan la interfaz `IHEVCDecoderSettings`.

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video HEVC --> HEVCDecoderBlock;
    HEVCDecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Selección automática del mejor decodificador disponible
var hevcDecoder = new HEVCDecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test.hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.hevc.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), hevcDecoder.Input);
pipeline.Connect(hevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `HEVCDecoderBlock.IsAvailable(HEVCDecoderType decoder)`. El decodificador de software FFmpeg generalmente siempre está disponible; los backends de hardware requieren los drivers y hardware GPU correspondientes.

#### Plataformas

Windows, macOS, Linux.

---

### Bloque Decodificador AV1

El `AV1DecoderBlock` decodifica streams de video AV1 con soporte para backends de software y hardware. Admite resoluciones de hasta 8K, color de 10 bits y contenido HDR. Los backends disponibles incluyen dav1d, av1dec, NVIDIA NVDEC, D3D11 (Windows) y VAAPI (Linux), con selección automática por defecto.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video AV1 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### Configuración

La clase `AV1DecoderSettings` acepta un valor de la enumeración `AV1DecoderType` para seleccionar el backend:

| Valor de AV1DecoderType | Descripción |
|--------------------------|-------------|
| `Auto`                  | Selección automática del mejor backend disponible |
| `Dav1d`                 | Decodificador dav1d de alto rendimiento (software) |
| `AOM`                   | Decodificador de referencia AOM (software) |
| `NVIDIA`                | NVIDIA NVDEC (hardware, GPU RTX 30xx o más reciente recomendado) |
| `Intel_QSV`             | Intel Quick Sync Video (hardware, Arc o más reciente) |
| `D3D11`                 | Aceleración D3D11/DXVA (solo Windows) |
| `VAAPI`                 | Aceleración VAAPI (solo Linux) |

```csharp
// Selección automática (recomendado)
var av1Decoder = new AV1DecoderBlock();

// Explícito: dav1d
var av1Decoder = new AV1DecoderBlock(new AV1DecoderSettings { DecoderType = AV1DecoderType.Dav1d });

// Explícito: NVIDIA NVDEC
var av1Decoder = new AV1DecoderBlock(new AV1DecoderSettings { DecoderType = AV1DecoderType.NVIDIA });
```

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video AV1 --> AV1DecoderBlock;
    AV1DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Selección automática del mejor decodificador AV1 disponible
var av1Decoder = new AV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test.av1.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.av1.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), av1Decoder.Input);
pipeline.Connect(av1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `AV1DecoderBlock.IsAvailable(AV1DecoderType decoderType = AV1DecoderType.Auto)`. El backend `dav1d` generalmente siempre está disponible; los backends de hardware requieren los drivers y hardware GPU correspondientes.

#### Plataformas

Windows, macOS, Linux.

---

### Bloque Decodificador NVAV1

El `NVAV1DecoderBlock` decodifica streams de video AV1 exclusivamente usando el hardware NVDEC de NVIDIA. Requiere una GPU de la serie RTX 30xx o más reciente con soporte de decodificación AV1 por hardware. Admite resoluciones de hasta 8K a 60 fps con soporte HDR10/HDR10+.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video AV1 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### Configuración

Este bloque no requiere parámetros de configuración:

```csharp
var nvAv1Decoder = new NVAV1DecoderBlock();
```

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video AV1 --> NVAV1DecoderBlock;
    NVAV1DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificador AV1 hardware NVIDIA (se requiere GPU RTX 30xx o más reciente)
var nvAv1Decoder = new NVAV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test.av1.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.av1.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvAv1Decoder.Input);
pipeline.Connect(nvAv1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `NVAV1DecoderBlock.IsAvailable()`. Requiere una GPU NVIDIA con soporte de decodificación AV1 por hardware (RTX 30xx o más reciente) y los drivers CUDA/NVDEC correctos.

#### Plataformas

Windows, Linux (se requiere GPU NVIDIA con soporte NVDEC AV1).

---

### Bloque Decodificador VP8

El `VP8DecoderBlock` decodifica streams de video VP8 usados en WebM y WebRTC. Selecciona automáticamente el mejor backend disponible: VPX → FFmpeg → NVIDIA NVDEC → VAAPI (Linux) → D3D11 (Windows). También se puede seleccionar explícitamente un backend mediante las clases de configuración correspondientes.

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video VP8 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### Configuración

**Selección automática (recomendado):**

```csharp
var vp8Decoder = new VP8DecoderBlock();
```

**Selección explícita de backend:**

```csharp
// VPX (software, alta compatibilidad)
var vp8Decoder = new VP8DecoderBlock(new VPXVP8DecoderSettings());

// FFmpeg (software)
var vp8Decoder = new VP8DecoderBlock(new FFMPEGVP8DecoderSettings());

// NVIDIA NVDEC (hardware)
var vp8Decoder = new VP8DecoderBlock(new NVVP8DecoderSettings());

// VAAPI (solo Linux, hardware)
var vp8Decoder = new VP8DecoderBlock(new VAAPIVP8DecoderSettings());

// D3D11/DXVA (solo Windows, hardware)
var vp8Decoder = new VP8DecoderBlock(new D3D11VP8DecoderSettings());
```

Todas las clases de configuración implementan la interfaz `IVP8DecoderSettings`.

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP8 --> VP8DecoderBlock;
    VP8DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Selección automática del mejor decodificador VP8 disponible
var vp8Decoder = new VP8DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test.vp8.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp8Decoder.Input);
pipeline.Connect(vp8Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `VP8DecoderBlock.IsAvailable(VP8DecoderType decoder)`. El backend VPX generalmente siempre está disponible; los backends de hardware requieren los drivers y hardware GPU correspondientes.

#### Plataformas

Windows, macOS, Linux.

---

### Bloque Decodificador VP9

El `VP9DecoderBlock` decodifica streams de video VP9 usados en YouTube 4K/8K, WebM y otros contenedores. Admite color de 10 bits y 12 bits. Selecciona automáticamente el mejor backend disponible: VPX → FFmpeg → NVIDIA NVDEC → VAAPI (Linux) → D3D11 (Windows).

#### Información del bloque

| Dirección del pin | Tipo de medio           | Conteo de pines |
|-------------------|-------------------------|-----------------|
| Entrada de video  | video VP9 codificado    | 1               |
| Salida de video   | video sin comprimir     | 1               |

#### Configuración

**Selección automática (recomendado):**

```csharp
var vp9Decoder = new VP9DecoderBlock();
```

**Selección explícita de backend:**

```csharp
// VPX (software, alta compatibilidad)
var vp9Decoder = new VP9DecoderBlock(new VPXVP9DecoderSettings());

// FFmpeg (software)
var vp9Decoder = new VP9DecoderBlock(new FFMPEGVP9DecoderSettings());

// NVIDIA NVDEC (hardware)
var vp9Decoder = new VP9DecoderBlock(new NVVP9DecoderSettings());

// VAAPI (solo Linux, hardware)
var vp9Decoder = new VP9DecoderBlock(new VAAPIVP9DecoderSettings());

// D3D11/DXVA (solo Windows, hardware)
var vp9Decoder = new VP9DecoderBlock(new D3D11VP9DecoderSettings());
```

Todas las clases de configuración implementan la interfaz `IVP9DecoderSettings`.

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP9 --> VP9DecoderBlock;
    VP9DecoderBlock -- Video Decodificado --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Selección automática del mejor decodificador VP9 disponible
var vp9Decoder = new VP9DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test.vp9.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp9Decoder.Input);
pipeline.Connect(vp9Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `VP9DecoderBlock.IsAvailable(VP9DecoderType decoder)`. El backend VPX generalmente siempre está disponible; los backends de hardware requieren los drivers y hardware GPU correspondientes.

#### Plataformas

Windows, macOS, Linux.

---

### Bloque Decode Bin VP8 con Alpha

El `VP8AlphaDecodeBinBlock` decodifica streams de video VP8 que incluyen un canal de transparencia alpha. Este bloque es ideal para contenido WebM/MKV con capas de transparencia. La salida es video sin comprimir en formato RGBA o YUVA, listo para composición o superposición.

#### Información del bloque

| Dirección del pin | Tipo de medio                               | Conteo de pines |
|-------------------|---------------------------------------------|-----------------|
| Entrada de video  | video VP8 con alpha codificado              | 1               |
| Salida de video   | video sin comprimir con alpha (RGBA/YUVA)   | 1               |

#### Configuración

Este bloque no requiere parámetros de configuración:

```csharp
var vp8AlphaDecoder = new VP8AlphaDecodeBinBlock();
```

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP8 con Alpha --> VP8AlphaDecodeBinBlock;
    VP8AlphaDecodeBinBlock -- Video Decodificado con Alpha --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificador VP8 con soporte de canal alpha
var vp8AlphaDecoder = new VP8AlphaDecodeBinBlock();

var basicFileSource = new BasicFileSourceBlock("test.vp8_alpha.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.vp8_alpha.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp8AlphaDecoder.Input);
pipeline.Connect(vp8AlphaDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `VP8AlphaDecodeBinBlock.IsAvailable()`. Requiere el plugin `codecalpha` de GStreamer y el paquete de redistribución SDK correcto.

#### Plataformas

Windows, macOS, Linux.

---

### Bloque Decode Bin VP9 con Alpha

El `VP9AlphaDecodeBinBlock` decodifica streams de video VP9 que incluyen un canal de transparencia alpha. Admite resoluciones de hasta 4K. La salida es video sin comprimir en formato RGBA o YUVA, adecuado para composición, superposición y efectos visuales avanzados.

#### Información del bloque

| Dirección del pin | Tipo de medio                               | Conteo de pines |
|-------------------|---------------------------------------------|-----------------|
| Entrada de video  | video VP9 con alpha codificado              | 1               |
| Salida de video   | video sin comprimir con alpha (RGBA/YUVA)   | 1               |

#### Configuración

Este bloque no requiere parámetros de configuración:

```csharp
var vp9AlphaDecoder = new VP9AlphaDecodeBinBlock();
```

#### El pipeline de muestra

```mermaid
graph LR;
    BasicFileSourceBlock -- Datos Raw --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Stream de Video VP9 con Alpha --> VP9AlphaDecodeBinBlock;
    VP9AlphaDecodeBinBlock -- Video Decodificado con Alpha --> VideoRendererBlock;
```

#### Código de muestra

```csharp
var pipeline = new MediaBlocksPipeline();

// Decodificador VP9 con soporte de canal alpha (hasta 4K)
var vp9AlphaDecoder = new VP9AlphaDecodeBinBlock();

var basicFileSource = new BasicFileSourceBlock("test.vp9_alpha.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.vp9_alpha.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Falló al obtener información de medios.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp9AlphaDecoder.Input);
pipeline.Connect(vp9AlphaDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilidad

Verificar disponibilidad usando `VP9AlphaDecodeBinBlock.IsAvailable()`. Requiere el plugin `codecalpha` de GStreamer y el paquete de redistribución SDK correcto.

#### Plataformas

Windows, macOS, Linux.

Windows (D3D11/DXVA requerido).