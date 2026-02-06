---
title: .Net Media Video Decoder Blocks Guide
description: Decompress encoded video streams with hardware-accelerated decoder blocks supporting H.264, HEVC, VP9, and more codecs in Media Blocks SDK.
sidebar_label: Video Decoders
---

# Video Decoder Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Video Decoder blocks are essential components in a media pipeline, responsible for decompressing encoded video streams into raw video frames that can be further processed or rendered. VisioForge Media Blocks SDK .Net offers a variety of video decoder blocks supporting numerous codecs and hardware acceleration technologies.

## Available Video Decoder Blocks

### H264 Decoder Block

Decodes H.264 (AVC) video streams. This is one of the most widely used video compression standards. The block can utilize different underlying decoder implementations like FFMPEG, OpenH264, or hardware-accelerated decoders if available.

#### Block info

Name: `H264DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | H.264 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### Settings

The `H264DecoderBlock` is configured using settings that implement `IH264DecoderSettings`. Available settings classes include:
- `FFMPEGH264DecoderSettings`
- `OpenH264DecoderSettings`
- `NVH264DecoderSettings` (for NVIDIA GPU acceleration)
- `VAAPIH264DecoderSettings` (for VA-API acceleration on Linux)

A constructor without parameters will attempt to select an available decoder automatically.

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.264 Video Stream --> H264DecoderBlock;
    H264DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create H264 Decoder block
var h264Decoder = new H264DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

// Get media info using MediaInfoReaderX
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

You can check for specific decoder implementations using `H264Decoder.IsAvailable(H264DecoderType decoderType)`.
`H264DecoderType` includes `FFMPEG`, `OpenH264`, `GPU_Nvidia_H264`, `VAAPI_H264`, etc.

#### Platforms

Windows, macOS, Linux. (Hardware-specific decoders like NVH264Decoder require specific hardware and drivers).

### JPEG Decoder Block

Decodes JPEG (Motion JPEG) video streams or individual JPEG images into raw video frames.

#### Block info

Name: `JPEGDecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | JPEG encoded video/images | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    HTTPSourceBlock -- MJPEG Stream --> JPEGDecoderBlock;
    JPEGDecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create JPEG Decoder block
var jpegDecoder = new JPEGDecoderBlock();

// Example: Create an HTTP source for an MJPEG camera and a video renderer
var httpSettings = new HTTPSourceSettings(new Uri("http://your-mjpeg-camera/stream"));
var httpSource = new HTTPSourceBlock(httpSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1

// Connect blocks
pipeline.Connect(httpSource.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

You can check if the underlying NVIDIA JPEG decoder (if applicable) is available using `NVJPEGDecoder.IsAvailable()`. The generic JPEG decoder functionality is generally available.

#### Platforms

Windows, macOS, Linux. (NVIDIA specific implementation requires NVIDIA hardware).

### NVIDIA H.264 Decoder Block (NVH264DecoderBlock)

Provides hardware-accelerated decoding of H.264 (AVC) video streams using NVIDIA's NVDEC technology. This offers high performance and efficiency on systems with compatible NVIDIA GPUs.

#### Block info

Name: `NVH264DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | H.264 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.264 Video Stream --> NVH264DecoderBlock;
    NVH264DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA H.264 Decoder block
var nvH264Decoder = new NVH264DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH264Decoder.Input);
pipeline.Connect(nvH264Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVH264Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA H.265 Decoder Block (NVH265DecoderBlock)

Provides hardware-accelerated decoding of H.265 (HEVC) video streams using NVIDIA's NVDEC technology. H.265 offers better compression efficiency than H.264.

#### Block info

Name: `NVH265DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | H.265/HEVC encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.265 Video Stream --> NVH265DecoderBlock;
    NVH265DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA H.265 Decoder block
var nvH265Decoder = new NVH265DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH265Decoder.Input);
pipeline.Connect(nvH265Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVH265Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for H.265 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA JPEG Decoder Block (NVJPEGDecoderBlock)

Provides hardware-accelerated decoding of JPEG images or Motion JPEG (MJPEG) streams using NVIDIA's NVJPEG library. This is particularly useful for high-resolution or high-framerate MJPEG streams.
(Note: The sample pipeline for JPEG with BasicFileSourceBlock might be less common than HTTPSource for MJPEG. The example below is adapted but consider typical use cases.)

#### Block info

Name: `NVJPEGDecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | JPEG encoded video/images | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw MJPEG Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- JPEG Video Stream --> NVJPEGDecoderBlock;
    NVJPEGDecoderBlock -- Decoded Video --> VideoRendererBlock;
```
For live MJPEG streams, `HTTPSourceBlock --> NVJPEGDecoderBlock` is more typical.

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA JPEG Decoder block
var nvJpegDecoder = new NVJPEGDecoderBlock();

// Example: Create a basic file source for an MJPEG file, demuxer, and renderer
// Ensure "test.mjpg" contains a Motion JPEG stream.
var basicFileSource = new BasicFileSourceBlock("test.mjpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.mjpg");
var mediaInfo = reader.Info;
if (mediaInfo == null || mediaInfo.VideoStreams.Count == 0 || !mediaInfo.VideoStreams[0].Codec.Contains("jpeg"))
{
    Console.WriteLine("Failed to get MJPEG media info or not an MJPEG file.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvJpegDecoder.Input);
pipeline.Connect(nvJpegDecoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVJPEGDecoder.IsAvailable()`. Requires an NVIDIA GPU and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA MPEG-1 Decoder Block (NVMPEG1DecoderBlock)

Provides hardware-accelerated decoding of MPEG-1 video streams using NVIDIA's NVDEC technology.

#### Block info

Name: `NVMPEG1DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | MPEG-1 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- MPEG-1 Video Stream --> NVMPEG1DecoderBlock;
    NVMPEG1DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA MPEG-1 Decoder block
var nvMpeg1Decoder = new NVMPEG1DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_mpeg1.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg1.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg1Decoder.Input);
pipeline.Connect(nvMpeg1Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVMPEG1Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for MPEG-1 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA MPEG-2 Decoder Block (NVMPEG2DecoderBlock)

Provides hardware-accelerated decoding of MPEG-2 video streams using NVIDIA's NVDEC technology. Commonly used for DVD video and some digital television broadcasts.

#### Block info

Name: `NVMPEG2DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | MPEG-2 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- MPEG-2 Video Stream --> NVMPEG2DecoderBlock;
    NVMPEG2DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA MPEG-2 Decoder block
var nvMpeg2Decoder = new NVMPEG2DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg2Decoder.Input);
pipeline.Connect(nvMpeg2Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVMPEG2Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for MPEG-2 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA MPEG-4 Decoder Block (NVMPEG4DecoderBlock)

Provides hardware-accelerated decoding of MPEG-4 Part 2 video streams (often found in AVI files, e.g., DivX/Xvid) using NVIDIA's NVDEC technology. Note that this is different from MPEG-4 Part 10 (H.264/AVC).

#### Block info

Name: `NVMPEG4DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | MPEG-4 Part 2 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- MPEG-4 Video Stream --> NVMPEG4DecoderBlock;
    NVMPEG4DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA MPEG-4 Decoder block
var nvMpeg4Decoder = new NVMPEG4DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_mpeg4.avi");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg4.avi");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg4Decoder.Input);
pipeline.Connect(nvMpeg4Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVMPEG4Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for MPEG-4 Part 2 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA VP8 Decoder Block (NVVP8DecoderBlock)

Provides hardware-accelerated decoding of VP8 video streams using NVIDIA's NVDEC technology. VP8 is an open video format, often used with WebM.

#### Block info

Name: `NVVP8DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | VP8 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- VP8 Video Stream --> NVVP8DecoderBlock;
    NVVP8DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA VP8 Decoder block
var nvVp8Decoder = new NVVP8DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp8Decoder.Input);
pipeline.Connect(nvVp8Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVVP8Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for VP8 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### NVIDIA VP9 Decoder Block (NVVP9DecoderBlock)

Provides hardware-accelerated decoding of VP9 video streams using NVIDIA's NVDEC technology. VP9 is an open and royalty-free video coding format developed by Google, often used for web streaming (e.g., YouTube).

#### Block info

Name: `NVVP9DecoderBlock`.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | VP9 encoded video | 1 |
| Output video | Uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- VP9 Video Stream --> NVVP9DecoderBlock;
    NVVP9DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create NVIDIA VP9 Decoder block
var nvVp9Decoder = new NVVP9DecoderBlock();

// Example: Create a basic file source, demuxer, and renderer
var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connect blocks
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp9Decoder.Input);
pipeline.Connect(nvVp9Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Availability

Check availability using `NVVP9Decoder.IsAvailable()`. Requires an NVIDIA GPU that supports NVDEC for VP9 and appropriate drivers.

#### Platforms

Windows, Linux (with NVIDIA drivers).

### VAAPI H.264 Decoder Block (VAAPIH264DecoderBlock)

Provides hardware-accelerated decoding of H.264 (AVC) video streams using VA-API (Video Acceleration API). Available on Linux systems with compatible hardware and drivers.

#### Block info

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | H.264 encoded video | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.264 Video Stream --> VAAPIH264DecoderBlock;
    VAAPIH264DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiH264Decoder = new VAAPIH264DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiH264Decoder.Input);
pipeline.Connect(vaapiH264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check with `VAAPIH264DecoderBlock.IsAvailable()`. Requires VA-API support and correct SDK redist.

#### Platforms

Linux (with VA-API drivers).

---

### VAAPI HEVC Decoder Block (VAAPIHEVCDecoderBlock)

Provides hardware-accelerated decoding of H.265/HEVC video streams using VA-API. Available on Linux systems with compatible hardware and drivers.

#### Block info

| Pin direction | Media type            | Pins count |
|---------------|----------------------|------------|
| Input video   | H.265/HEVC encoded   | 1          |
| Output video  | Uncompressed video   | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.265 Video Stream --> VAAPIHEVCDecoderBlock;
    VAAPIHEVCDecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiHevcDecoder = new VAAPIHEVCDecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiHevcDecoder.Input);
pipeline.Connect(vaapiHevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check with `VAAPIHEVCDecoderBlock.IsAvailable()`. Requires VA-API support and correct SDK redist.

#### Platforms

Linux (with VA-API drivers).

---

### VAAPI JPEG Decoder Block (VAAPIJPEGDecoderBlock)

Provides hardware-accelerated decoding of JPEG/MJPEG video streams using VA-API. Available on Linux systems with compatible hardware and drivers.

#### Block info

| Pin direction | Media type                | Pins count |
|---------------|--------------------------|------------|
| Input video   | JPEG encoded video/images | 1          |
| Output video  | Uncompressed video        | 1          |

#### The sample pipeline

```mermaid
graph LR;
    HTTPSourceBlock -- MJPEG Stream --> VAAPIJPEGDecoderBlock;
    VAAPIJPEGDecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

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

#### Availability

Check with `VAAPIJPEGDecoderBlock.IsAvailable()`. Requires VA-API support and correct SDK redist.

#### Platforms

Linux (with VA-API drivers).

---

### VAAPI VC1 Decoder Block (VAAPIVC1DecoderBlock)

Provides hardware-accelerated decoding of VC-1 video streams using VA-API. Available on Linux systems with compatible hardware and drivers.

#### Block info

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | VC-1 encoded video  | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- VC-1 Video Stream --> VAAPIVC1DecoderBlock;
    VAAPIVC1DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiVc1Decoder = new VAAPIVC1DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_vc1.wmv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vc1.wmv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiVc1Decoder.Input);
pipeline.Connect(vaapiVc1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check with `VAAPIVC1DecoderBlock.IsAvailable()`. Requires VA-API support and correct SDK redist.

#### Platforms

Linux (with VA-API drivers).

---

## Direct3D 11/DXVA Video Decoder Blocks

Direct3D 11/DXVA (D3D11) decoder blocks provide hardware-accelerated video decoding on Windows systems with compatible GPUs and drivers. These blocks are useful for high-performance video playback and processing pipelines on Windows.

### D3D11 AV1 Decoder Block

Decodes AV1 video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11AV1DecoderBlock`.

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | AV1 encoded video   | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- AV1 Video Stream --> D3D11AV1DecoderBlock;
    D3D11AV1DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 AV1 Decoder block
var d3d11Av1Decoder = new D3D11AV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_av1.mkv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_av1.mkv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Av1Decoder.Input);
pipeline.Connect(d3d11Av1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11AV1DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).

---

### D3D11 H.264 Decoder Block

Decodes H.264 (AVC) video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11H264DecoderBlock`.

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | H.264 encoded video | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.264 Video Stream --> D3D11H264DecoderBlock;
    D3D11H264DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 H.264 Decoder block
var d3d11H264Decoder = new D3D11H264DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H264Decoder.Input);
pipeline.Connect(d3d11H264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11H264DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).

---

### D3D11 H.265 Decoder Block

Decodes H.265 (HEVC) video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11H265DecoderBlock`.

| Pin direction | Media type            | Pins count |
|---------------|----------------------|------------|
| Input video   | H.265/HEVC encoded   | 1          |
| Output video  | Uncompressed video   | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- H.265 Video Stream --> D3D11H265DecoderBlock;
    D3D11H265DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 H.265 Decoder block
var d3d11H265Decoder = new D3D11H265DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H265Decoder.Input);
pipeline.Connect(d3d11H265Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11H265DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).

---

### D3D11 MPEG-2 Decoder Block

Decodes MPEG-2 video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11MPEG2DecoderBlock`.

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | MPEG-2 encoded video| 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- MPEG-2 Video Stream --> D3D11MPEG2DecoderBlock;
    D3D11MPEG2DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 MPEG-2 Decoder block
var d3d11Mpeg2Decoder = new D3D11MPEG2DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Mpeg2Decoder.Input);
pipeline.Connect(d3d11Mpeg2Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11MPEG2DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).

---

### D3D11 VP8 Decoder Block

Decodes VP8 video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11VP8DecoderBlock`.

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | VP8 encoded video   | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- VP8 Video Stream --> D3D11VP8DecoderBlock;
    D3D11VP8DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 VP8 Decoder block
var d3d11Vp8Decoder = new D3D11VP8DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp8Decoder.Input);
pipeline.Connect(d3d11Vp8Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11VP8DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).

---

### D3D11 VP9 Decoder Block

Decodes VP9 video streams using Direct3D 11/DXVA hardware acceleration.

#### Block info

Name: `D3D11VP9DecoderBlock`.

| Pin direction | Media type           | Pins count |
|---------------|---------------------|------------|
| Input video   | VP9 encoded video   | 1          |
| Output video  | Uncompressed video  | 1          |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Raw Data --> UniversalDemuxBlock;
    UniversalDemuxBlock -- VP9 Video Stream --> D3D11VP9DecoderBlock;
    D3D11VP9DecoderBlock -- Decoded Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create D3D11 VP9 Decoder block
var d3d11Vp9Decoder = new D3D11VP9DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Failed to get media info.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp9Decoder.Input);
pipeline.Connect(d3d11Vp9Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

Check availability using `D3D11VP9DecoderBlock.IsAvailable()`. Requires Windows with D3D11/DXVA support and correct SDK redist.

#### Platforms

Windows (D3D11/DXVA required).