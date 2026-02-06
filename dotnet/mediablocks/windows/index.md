---
title: Windows Platform Blocks
description: Windows-optimized MediaBlocks with Direct3D 11 hardware acceleration, video effects, DirectShow decoders, and platform-specific features.
sidebar_label: Windows Platform
---

# Windows Platform Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This section covers MediaBlocks specifically optimized for Windows platforms.

## Available Blocks

### Direct3D 11 Hardware Decoders

Windows provides hardware-accelerated video decoding through Direct3D 11:

- **D3D11H264DecoderBlock**: Hardware H.264/AVC decoding
- **D3D11H265DecoderBlock**: Hardware H.265/HEVC decoding
- **D3D11VP8DecoderBlock**: Hardware VP8 decoding
- **D3D11VP9DecoderBlock**: Hardware VP9 decoding
- **D3D11AV1DecoderBlock**: Hardware AV1 decoding
- **D3D11MPEG2DecoderBlock**: Hardware MPEG-2 decoding

See [Video Decoders Documentation](../VideoDecoders/index.md)

### Direct3D 11 Processing

- **D3D11UploadBlock**: Upload video from system memory to GPU
- **D3D11DownloadBlock**: Download video from GPU to system memory
- **D3D11VideoConverterBlock**: GPU-accelerated color space conversion

See [Video Processing Documentation](../VideoProcessing/index.md#d3d11-video-converter)

### Direct3D 11 Composition

- **D3D11VideoCompositorBlock**: GPU-accelerated video compositing and mixing

### Windows Video Effects

- **VideoEffectsWinBlock**: Windows-specific video effects
- **VR360ProcessorBlock**: 360-degree VR video processing

### Special Blocks

See [Special Blocks Documentation](../Special/index.md)

## Platform Requirements

- **Windows**: Windows 7 SP1 or later
- **Direct3D 11**: GPU with D3D11 support
- **Hardware Decode**: GPU with hardware video decode support

## Features

- **Hardware Acceleration**: Leverage GPU for encoding, decoding, and processing
- **Direct3D 11 Integration**: Efficient video processing on GPU
- **Low CPU Usage**: Offload processing to dedicated hardware
- **High Performance**: Handle multiple HD/4K streams simultaneously
- **Power Efficiency**: Reduce power consumption with hardware acceleration

## Sample Code

### Hardware H.264 Decoding

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// D3D11 hardware decoder
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Decoder.Input);

// Process on GPU or download to system memory
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Decoder.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### GPU Video Processing Pipeline

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// Upload to GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// GPU color conversion
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Download from GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Video Composition

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video1.mp4")));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video2.mp4")));

// Upload both to GPU
var upload1 = new D3D11UploadBlock();
var upload2 = new D3D11UploadBlock();
pipeline.Connect(source1.VideoOutput, upload1.Input);
pipeline.Connect(source2.VideoOutput, upload2.Input);

// Composite on GPU
var compositor = new D3D11VideoCompositorBlock(compositorSettings);
pipeline.Connect(upload1.Output, compositor.Input1);
pipeline.Connect(upload2.Output, compositor.Input2);

// Download result
var download = new D3D11DownloadBlock();
pipeline.Connect(compositor.Output, download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Performance Tips

- Keep video in GPU memory between operations to avoid upload/download overhead
- Use hardware decoders when available for best performance
- Chain GPU operations together before downloading to system memory
- Monitor GPU memory usage when processing multiple streams
- Check hardware support before using D3D11 blocks

## Checking Hardware Support

```csharp
// Check if D3D11 H.264 decoder is available
if (D3D11H264DecoderBlock.IsAvailable())
{
    // Use hardware decoder
    var decoder = new D3D11H264DecoderBlock();
}
else
{
    // Fall back to software decoder
    var decoder = new UniversalDecoderBlock();
}
```

## Platforms

Windows 7 SP1 or later (Windows 10/11 recommended for best hardware support).

## Related Documentation

- [VideoDecoders](../VideoDecoders/index.md) - Hardware decoding blocks
- [VideoProcessing](../VideoProcessing/index.md) - GPU processing blocks
- [Special](../Special/index.md) - Platform-specific utilities
