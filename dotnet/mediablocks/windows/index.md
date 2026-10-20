---
title: Direct3D 11 GPU Video Decoding and Effects in C# .NET
description: Use Direct3D 11 hardware-accelerated video decoding, effects, and DirectShow integration on Windows with VisioForge Media Blocks SDK for .NET.
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
  - D3D12InterlaceBlock
  - D3D12InterlaceSettings
  - D3D12FisheyeDewarpBlock
  - D3D12FisheyeDewarpSettings
  - D3D12RemapBlock
  - D3D12RemapSettings
  - D3D12UVRemapMap
  - UniversalSourceBlock
  - UniversalSourceSettings

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

### Direct3D 12 Processing

- **D3D12InterlaceBlock**: GPU conversion of progressive video to interlaced video, for broadcast and other outputs that need an explicit field order

This is the opposite of deinterlacing (`DeinterlaceBlock`): the input must be progressive, and the output is interleaved interlaced video. `D3D12InterlaceSettings` sets:

- `FieldPattern`: `OneToOne` turns two progressive frames into one interlaced frame, halving the frame rate (60p to 60i, 30 frames per second); `TwoToTwo` splits each progressive frame into two fields, keeping the frame rate (30p to 60i, 30 frames per second)
- `TopFieldFirst`: top field first when `true`, bottom field first when `false` (the default)
- `Engine`: the GPU engine (`Auto`, `ThreeD`, `Compute`)
- `AdapterIndex`: the GPU adapter, `-1` for the default one

The block moves frames to and from the GPU itself: its input and its output each take system, D3D11 or D3D12 memory, whichever the neighbouring block uses, and D3D12 memory on the block's own GPU adapter passes through without a copy on both sides. It needs a Direct3D 12 capable GPU. The downstream consumer must accept interlaced video (for example, the MPEG-2 encoder): one that takes progressive video only cannot be linked to the block, and the pipeline reports the failed link through `OnError`. A GPU adapter that does not exist fails the pipeline start.

- **D3D12FisheyeDewarpBlock**: GPU dewarping of a circular fisheye image into an equirectangular, panorama or perspective view

`D3D12FisheyeDewarpSettings` sets:

- `Projection`: `Equirectangular` (the default), `Panorama`, `Perspective` or `Passthrough`
- `CenterX`, `CenterY`, `RadiusX`, `RadiusY`: the fisheye circle in the input image, normalized from 0 to 1, the radius at least 1e-6 (defaults: centred, radius 0.5)
- `FisheyeFOV`: the lens field of view in degrees (180 by default)
- `HorizontalFOV`, `VerticalFOV`: the output field of view in degrees (90 by default); not used by `Panorama`. The element reduces angles modulo 360; reduced, an angle must be from 0.001 to 359.999 degrees, and up to 179.999 for `Perspective`. `FisheyeFOV` takes the 0.001 to 359.999 range in every projection. An `Equirectangular` frame spans `HorizontalFOV` by `VerticalFOV`, so a wide one takes, for example, 300 by 180
- `RotationX` (pitch), `RotationY` (yaw), `RotationZ` (roll) in degrees, with `RotationOrder` and `RotationSpace` (`Local` camera-relative or `World` fixed axes); only the roll applies to `Panorama`
- `InnerRadius`: the centre part cropped out of a `Panorama`, normalized (0.3 by default)
- `RoiX`, `RoiY`, `RoiWidth`, `RoiHeight`: the region of interest in output image space, normalized, width and height at least 1e-6; the element does not clamp it. Past the frame horizontally it turns the view (a panorama crosses its seam); past it vertically a panorama shows black rows and an equirectangular frame folds over the pole
- `ViewportX`, `ViewportY`, `ViewportWidth`, `ViewportHeight`: where the result is drawn in the output frame, normalized, width and height at least 1e-6
- `AdapterIndex`: the GPU adapter, `-1` for the default one

Every setting but `AdapterIndex` can be changed while the pipeline runs: change `Settings` and call `Update()`, which returns `false` when a value is out of range and, once the block is in a pipeline, reports the cause through `OnError`. This turns one fisheye camera into a virtual pan-tilt-zoom camera. The output has the input's size, format and frame rate; put a `VideoResizeBlock` after the block for another size, for example 2:1 for an equirectangular frame. Like the interlace block, it moves frames to and from the GPU itself and needs a Direct3D 12 capable GPU. On the CPU, `CVDewarpBlock` (OpenCV) is the alternative.

- **D3D12RemapBlock**: GPU remapping of video through a UV map, for lens calibration, distortion correction and other geometric transformations

`D3D12RemapSettings` sets:

- `Map`: a `D3D12UVRemapMap`, or `null` to pass the video through unchanged
- `AdapterIndex`: the GPU adapter, `-1` for the default one

A `D3D12UVRemapMap` is created from its width and height (each from 1 to 16384) and an array of `ushort` values, four per map point, row by row from the top left: U, V, unused and mask. U and V are the input position the point shows, 0 to 65535 for 0 to 1 of the input's width and height. A mask of 32768 or more shows the input there; a smaller one shows black. The map is stretched over the output frame, so its size does not have to match the video's: map point (x, y) stands for the output position ((x + 0.5) / width, (y + 0.5) / height). The map is copied when it is created and takes 8 bytes per point; a map the size of the video is plenty. A saved pipeline (`ToJson`, `SaveJsonAsync`) carries the map, up to 80 million points (8192x8192 is 67 million); a pipeline with a larger map is saved without the block's settings, and `ToDocument` reports it. To replace or remove it while the pipeline runs, change `Settings.Map` and call `Update()`; the next frame shows the change. The block uploads the map to the GPU the video frames are on and owns that copy, so the application never handles a Direct3D resource; the frame that takes a new map waits for its upload. A failed upload is reported through `OnError`, and the previous map stays in use (if there is none, or it was made for another GPU, the video passes through unchanged) until the next `Update()`, which uploads the map again, or until the video moves to another GPU. Like the interlace block, it moves frames to and from the GPU itself and needs a Direct3D 12 capable GPU.

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

// The decoder takes the encoded stream: demux the MP4 and parse its H.264 instead of decoding it in a source block (another container needs its own demuxer)
var fileSource = new BasicFileSourceBlock("video.mp4");
var demux = new QTDemuxBlock(renderVideo: true, renderAudio: false);
var parse = new H264ParseBlock();
pipeline.Connect(fileSource.Output, demux.Input);
pipeline.Connect(demux.VideoOutput, parse.Input);

// D3D11 hardware decoder
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(parse.Output, d3d11Decoder.Input);

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

// Software decoding: a hardware decoder already outputs GPU memory, which leaves the upload nothing to do
var settings = await UniversalSourceSettings.CreateAsync("video.mp4");
settings.DisableHWDecoders = true;
var fileSource = new UniversalSourceBlock(settings);

// Upload to GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// GPU color conversion, applied when the next block needs another format
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Download from GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Progressive to Interlaced on the GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// A 25p progressive source
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// 25p to 50i, top field first
var interlace = new D3D12InterlaceBlock(new D3D12InterlaceSettings
{
    FieldPattern = D3D12InterlacePattern.TwoToTwo,
    TopFieldFirst = true
});
pipeline.Connect(fileSource.VideoOutput, interlace.Input);

// MPEG-2 keeps the interlacing and the field order in the stream
var encoder = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(interlace.Output, encoder.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));
pipeline.Connect(encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Fisheye Dewarping on the GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// A circular fisheye recording
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("fisheye.mp4"));

// A 90-degree perspective view, turned 30 degrees around the vertical axis (yaw)
var dewarp = new D3D12FisheyeDewarpBlock(new D3D12FisheyeDewarpSettings
{
    Projection = D3D12FisheyeProjection.Perspective,
    RotationY = 30
});
pipeline.Connect(fileSource.VideoOutput, dewarp.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(dewarp.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Later, while playing: pan the view
dewarp.Settings.RotationY = -45;
dewarp.Update();
```

### Remapping Video on the GPU

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// Your own map, for example from a lens calibration: 1920 x 1080 points, four values each (U, V, unused, mask)
ushort[] calibration = LoadCalibrationMap();
var remap = new D3D12RemapBlock(new D3D12RemapSettings
{
    Map = new D3D12UVRemapMap(1920, 1080, calibration)
});
pipeline.Connect(fileSource.VideoOutput, remap.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(remap.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Later, while playing: switch to another calibration
remap.Settings.Map = new D3D12UVRemapMap(1920, 1080, LoadOtherCalibrationMap());
remap.Update();
```

### Video Composition

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video1.mp4"));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video2.mp4"));

// Upload both to GPU
var upload1 = new D3D11UploadBlock();
var upload2 = new D3D11UploadBlock();
pipeline.Connect(source1.VideoOutput, upload1.Input);
pipeline.Connect(source2.VideoOutput, upload2.Input);

// Configure the compositor — two streams on a 1920x1080 @ 30 fps canvas
var compositorSettings = new D3D11VideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(0, 0, 960, 1080), zorder: 0));
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(960, 0, 1920, 1080), zorder: 1));

// Composite on GPU — each added stream creates one input pad; access them via Inputs[].
var compositor = new D3D11VideoCompositorBlock(compositorSettings);
pipeline.Connect(upload1.Output, compositor.Inputs[0]);
pipeline.Connect(upload2.Output, compositor.Inputs[1]);

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
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Platforms

Windows 7 SP1 or later (Windows 10/11 recommended for best hardware support).

## Related Documentation

- [VideoDecoders](../VideoDecoders/index.md) - Hardware decoding blocks
- [VideoProcessing](../VideoProcessing/index.md) - GPU processing blocks
- [Special](../Special/index.md) - Platform-specific utilities
