---
title: Mastering Video Encoders in .NET SDK
description: Unlock high-performance video encoding in .NET projects. This guide covers various video encoders, codecs like AV1, H264, HEVC, and GPU acceleration techniques.
sidebar_label: Video Encoders
order: 18
---

# Video encoding

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

Video encoding is the process of converting raw video data into a compressed format. This process is essential for reducing the size of video files, making them easier to store and stream over the internet. VisioForge Media Blocks SDK provides a wide range of video encoders that support various formats and codecs.

For some video encoders, SDK can use GPU acceleration to speed up the encoding process. This feature is especially useful when working with high-resolution video files or when encoding multiple videos simultaneously.

NVidia, Intel, and AMD GPUs are supported for hardware acceleration.

## AV1 encoder

`AV1 (AOMedia Video 1)`: Developed by the Alliance for Open Media, AV1 is an open, royalty-free video coding format designed for video transmissions over the Internet. It is known for its high compression efficiency and better quality at lower bit rates compared to its predecessors, making it well-suited for high-resolution video streaming applications.

Use classes that implement the `IAV1EncoderSettings` interface to set the parameters.

#### CPU Encoders

##### AOMAV1EncoderSettings

AOM AV1 encoder settings. CPU encoder.

**Platforms:** Windows, Linux, macOS.

##### RAV1EEncoderSettings

RAV1E AV1 encoder settings. CPU encoder.

- **Key Properties**:
  - `Bitrate` (integer): Target bitrate in kilobits per second.
  - `LowLatency` (boolean): Enables or disables low latency mode. Default is `false`.
  - `MaxKeyFrameInterval` (ulong): Maximum interval between keyframes. Default is `240`.
  - `MinKeyFrameInterval` (ulong): Minimum interval between keyframes. Default is `12`.
  - `MinQuantizer` (uint): Minimum quantizer value (range 0-255). Default is `0`.
  - `Quantizer` (uint): Quantizer value (range 0-255). Default is `100`.
  - `SpeedPreset` (int): Encoding speed preset (10 fastest, 0 slowest). Default is `6`.
  - `Tune` (`RAV1EEncoderTune`): Tune setting for the encoder. Default is `RAV1EEncoderTune.Psychovisual`.

**Platforms:** Windows, Linux, macOS.

###### RAV1EEncoderTune Enum

Specifies the tuning option for the RAV1E encoder.

- `PSNR` (0): Tune for best PSNR (Peak Signal-to-Noise Ratio).
- `Psychovisual` (1): Tune for psychovisual quality.

#### GPU Encoders

##### AMFAV1EncoderSettings

AMD GPU AV1 video encoder.

**Platforms:** Windows, Linux, macOS.

##### NVENCAV1EncoderSettings

Nvidia GPU AV1 video encoder.

**Platforms:** Windows, Linux, macOS.

##### QSVAV1EncoderSettings

Intel GPU AV1 video encoder.

**Platforms:** Windows, Linux, macOS.

*Note: Intel QSV encoders may also utilize common enumerations like `QSVCodingOption` (`On`, `Off`, `Unknown`) for configuring specific hardware features.*

### Block info

Name: AV1EncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | AV1 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AV1EncoderBlock;
    AV1EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new AV1EncoderBlock(new QSVAV1EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(videoEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## DV encoder

`DV (Digital Video)`: A format for storing digital video introduced in the 1990s, primarily used in consumer digital camcorders. DV employs intra-frame compression to deliver high-quality video on digital tapes, making it suitable for home videos as well as semi-professional productions.

### Block info

Name: DVEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | video/x-dv | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->DVEncoderBlock;
    DVEncoderBlock-->AVISinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new DVEncoderBlock(new DVVideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var sinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, sinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## H264 encoder

The H264 encoder block is used for encoding files in MP4, MKV, and some other formats, as well as for network streaming using RTSP and HLS.

Use classes that implement the IH264EncoderSettings interface to set the parameters.

### Settings

#### NVENCH264EncoderSettings

Nvidia GPUs H264 video encoder.

**Platforms:** Windows, Linux, macOS.

#### AMFHEVCEncoderSettings

AMD/ATI GPUs H264 video encoder.

**Platforms:** Windows, Linux, macOS.

#### QSVH264EncoderSettings

Intel GPU H264 video encoder.

**Platforms:** Windows, Linux, macOS.

#### OpenH264EncoderSettings

Software CPU H264 encoder.

**Platforms:** Windows, macOS, Linux, iOS, Android.

#### CustomH264EncoderSettings

Allows using a custom GStreamer element for H264 encoding. You can specify the GStreamer element name and configure its properties.

**Platforms:** Windows, Linux, macOS.

### Block info

Name: H264EncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | H264 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var h264EncoderBlock = new H264EncoderBlock(new NVENCH264EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Sample applications

- [Simple Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)
- [Screen Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)

### Platforms

Windows, macOS, Linux, iOS, Android.

## HEVC/H265 encoder

HEVC encoder is used for encoding files in MP4, MKV, and some other formats, as well as for network streaming using RTSP and HLS.

Use classes that implement the IHEVCEncoderSettings interface to set the parameters.

### Settings

#### MFHEVCEncoderSettings

Microsoft Media Foundation HEVC encoder. CPU encoder.

**Platforms:** Windows.

#### NVENCHEVCEncoderSettings

Nvidia GPUs HEVC video encoder.

**Platforms:** Windows, Linux, macOS.

#### AMFHEVCEncoderSettings

AMD/ATI GPUs HEVC video encoder.

**Platforms:** Windows, Linux, macOS.

#### QSVHEVCEncoderSettings

Intel GPU HEVC video encoder.

**Platforms:** Windows, Linux, macOS.

#### CustomHEVCEncoderSettings

Allows using a custom GStreamer element for HEVC encoding. You can specify the GStreamer element name and configure its properties.

**Platforms:** Windows, Linux, macOS.

### Block info

Name: HEVCEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | HEVC | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->HEVCEncoderBlock;
    HEVCEncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var hevcEncoderBlock = new HEVCEncoderBlock(new NVENCHEVCEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## MJPEG encoder

`MJPEG (Motion JPEG)`: A video compression format where each frame of video is separately compressed into a JPEG image. This technique is straightforward and results in no interframe compression, making it ideal for situations where frame-specific editing or access is required, such as in surveillance and medical imaging.
Use classes that implement the IH264EncoderSettings interface to set the parameters.

### Settings

#### MJPEGEncoderSettings

Default MJPEG encoder. CPU encoder.

- **Key Properties**:
  - `Quality` (int): JPEG quality level (10-100). Default is `85`.
- **Encoder Type**: `MJPEGEncoderType.CPU`.

**Platforms:** Windows, Linux, macOS, iOS, Android.

#### QSVMJPEGEncoderSettings

Intel GPUs MJPEG encoder.

- **Key Properties**:
  - `Quality` (uint): JPEG quality level (10-100). Default is `85`.
- **Encoder Type**: `MJPEGEncoderType.GPU_Intel_QSV_MJPEG`.

**Platforms:** Windows, Linux, macOS.

#### MJPEGEncoderType Enum

Specifies the type of MJPEG encoder.

- `CPU`: Default CPU-based encoder.
- `GPU_Intel_QSV_MJPEG`: Intel QuickSync GPU-based MJPEG encoder.

### Block info

Name: MJPEGEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | MJPEG | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MJPEGEncoderBlock;
    MJPEGEncoderBlock-->AVISinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new MJPEGEncoderBlock(new MJPEGEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var aviSinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, aviSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## Theora encoder

The [Theora](https://www.theora.org/) encoder is used to encode video files in WebM format.

### Settings

#### TheoraEncoderSettings

Provides settings for the Theora encoder.

- **Key Properties**:
  - `Bitrate` (kbps)
  - `CapOverflow`, `CapUnderflow` (bit reservoir capping)
  - `DropFrames` (allow/disallow frame dropping)
  - `KeyFrameAuto` (automatic keyframe detection)
  - `KeyFrameForce` (interval to force keyframe every N frames)
  - `KeyFrameFrequency` (keyframe frequency)
  - `MultipassCacheFile` (string path for multipass cache)
  - `MultipassMode` (using `TheoraMultipassMode` enum: `SinglePass`, `FirstPass`, `SecondPass`)
  - `Quality` (integer value, typically 0-63 for libtheora, meaning can vary)
  - `RateBuffer` (size of rate control buffer in units of frames, 0 = auto)
  - `SpeedLevel` (amount of motion vector searching, 0-2 or higher depending on implementation)
  - `VP3Compatible` (boolean to enable VP3 compatibility)
- **Availability**: Can be checked using `TheoraEncoderSettings.IsAvailable()`.

### Block info

Name: TheoraEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | video/x-theora | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->TheoraEncoderBlock;
    TheoraEncoderBlock-->WebMSinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var theoraEncoderBlock = new TheoraEncoderBlock(new TheoraEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, theoraEncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(theoraEncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## VPX encoder

VPX encoder block is used to encode files in WebM, MKV, or OGG files. VPX encoder is a set of video codecs for encoding in VP8 and VP9 formats.

The VPX encoder block utilizes settings classes that implement the `IVPXEncoderSettings` interface. Key settings classes include:

### Settings

The common base class for VP8 and VP9 CPU encoder settings is `VPXEncoderSettings`. It provides a wide range of shared properties for tuning the encoding process, such as:

- `ARNRMaxFrames`, `ARNRStrength`, `ARNRType` (AltRef noise reduction)
- `BufferInitialSize`, `BufferOptimalSize`, `BufferSize` (client buffer settings)
- `CPUUsed`, `CQLevel` (constrained quality)
- `Deadline` (encoding deadline per frame)
- `DropframeThreshold`
- `RateControl` (using `VPXRateControl` enum)
- `ErrorResilient` (using `VPXErrorResilientFlags` enum)
- `HorizontalScalingMode`, `VerticalScalingMode` (using `VPXScalingMode` enum)
- `KeyFrameMaxDistance`, `KeyFrameMode` (using `VPXKeyFrameMode` enum)
- `MinQuantizer`, `MaxQuantizer`
- `MultipassCacheFile`, `MultipassMode` (using `VPXMultipassMode` enum)
- `NoiseSensitivity`
- `TargetBitrate` (in Kbits/s)
- `NumOfThreads`
- `TokenPartitions` (using `VPXTokenPartitions` enum)
- `Tuning` (using `VPXTuning` enum)

#### VP8EncoderSettings

CPU encoder for VP8. Inherits from `VPXEncoderSettings`.

- **Key Properties**: Leverages properties from `VPXEncoderSettings` tailored for VP8.
- **Encoder Type**: `VPXEncoderType.VP8`.
- **Availability**: Can be checked using `VP8EncoderSettings.IsAvailable()`.

#### VP9EncoderSettings

CPU encoder for VP9. Inherits from `VPXEncoderSettings`.

- **Key Properties**: In addition to `VPXEncoderSettings` properties, includes VP9-specific settings:
  - `AQMode` (Adaptive Quantization mode, using `VPXAdaptiveQuantizationMode` enum)
  - `FrameParallelDecoding` (allow parallel processing)
  - `RowMultithread` (multi-threaded row encoding)
  - `TileColumns`, `TileRows` (log2 values)
- **Encoder Type**: `VPXEncoderType.VP9`.
- **Availability**: Can be checked using `VP9EncoderSettings.IsAvailable()`.

#### QSVVP9EncoderSettings

Intel QSV (GPU accelerated) encoder for VP9.

- **Key Properties**:
  - `LowLatency`
  - `TargetUsage` (1: Best quality, 4: Balanced, 7: Best speed)
  - `Bitrate` (Kbit/sec)
  - `GOPSize`
  - `ICQQuality` (Intelligent Constant Quality)
  - `MaxBitrate` (Kbit/sec)
  - `QPI`, `QPP` (constant quantizer for I and P frames)
  - `Profile` (0-3)
  - `RateControl` (using `QSVVP9EncRateControl` enum)
  - `RefFrames`
- **Encoder Type**: `VPXEncoderType.QSV_VP9`.
- **Availability**: Can be checked using `QSVVP9EncoderSettings.IsAvailable()`.

#### CustomVPXEncoderSettings

Allows using a custom GStreamer element for VPX encoding.

- **Key Properties**:
  - `ElementName` (string to specify the GStreamer element name)
  - `Properties` (Dictionary<string, object> to configure the element)
  - `VideoFormat` (required video format like `VideoFormatX.NV12`)
- **Encoder Type**: `VPXEncoderType.CustomEncoder`.

### VPX Enumerations

Several enumerations are available to configure VPX encoders:

- `VPXAdaptiveQuantizationMode`: Defines adaptive quantization modes (e.g., `Off`, `Variance`, `Complexity`, `CyclicRefresh`, `Equator360`, `Perceptual`, `PSNR`, `Lookahead`).
- `VPXErrorResilientFlags`: Flags for error resilience features (e.g., `None`, `Default`, `Partitions`).
- `VPXKeyFrameMode`: Defines keyframe placement strategies (e.g., `Auto`, `Disabled`).
- `VPXMultipassMode`: Modes for multipass encoding (e.g., `OnePass`, `FirstPass`, `LastPass`).
- `VPXRateControl`: Rate control modes (e.g., `VBR`, `CBR`, `CQ`).
- `VPXScalingMode`: Scaling modes (e.g., `Normal`, `_4_5`, `_3_5`, `_1_2`).
- `VPXTokenPartitions`: Number of token partitions (e.g., `One`, `Two`, `Four`, `Eight`).
- `VPXTuning`: Tuning options for the encoder (e.g., `PSNR`, `SSIM`).
- `VPXEncoderType`: Specifies the VPX encoder variant (e.g., `VP8`, `VP9`, `QSV_VP9`, `CustomEncoder`, and platform-specific ones like `OMXExynosVP8Encoder`).
- `QSVVP9EncRateControl`: Rate control modes specific to `QSVVP9EncoderSettings` (e.g., `CBR`, `VBR`, `CQP`, `ICQ`).

### Block info

Name: VPXEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | VP8/VP9 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->VPXEncoderBlock;
    VPXEncoderBlock-->WebMSinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var vp8EncoderBlock = new VPXEncoderBlock(new VP8EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, vp8EncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(vp8EncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux, iOS, Android.

## MPEG2 encoder

`MPEG-2`: A widely used standard for video and audio compression, commonly found in DVDs, digital television broadcasts (like DVB and ATSC), and SVCDs. It offers good quality at relatively low bitrates for standard definition content.

### Block info

Name: MPEG2EncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | video/mpeg | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG2EncoderBlock;
    MPEG2EncoderBlock-->MPEGTSSinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg2EncoderBlock = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg2EncoderBlock.Input);

// Example: Using an MPGSinkBlock for .mpg or .ts files
var mpgSinkBlock = new MPGSinkBlock(new MPGSinkSettings(@"output.mpg"));
pipeline.Connect(mpeg2EncoderBlock.Output, mpgSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

## MPEG4 encoder

`MPEG-4 Part 2 Visual` (often referred to simply as MPEG-4 video) is a video compression standard that is part of the MPEG-4 suite. It is used in various applications, including streaming video, video conferencing, and optical discs like DivX and Xvid.

### Block info

Name: MPEG4EncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | video/mpeg, mpegversion=4 | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG4EncoderBlock;
    MPEG4EncoderBlock-->MP4SinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4"; // Input file
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg4EncoderBlock = new MPEG4EncoderBlock(new MPEG4VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg4EncoderBlock.Input);

// Example: Using an MP4SinkBlock for .mp4 files
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output_mpeg4.mp4"));
pipeline.Connect(mpeg4EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

## Apple ProRes encoder

`Apple ProRes`: A high-quality, lossy video compression format developed by Apple Inc., widely used in professional video production and post-production workflows for its excellent balance of image quality and performance.

### Block info

Name: AppleProResEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | ProRes | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AppleProResEncoderBlock;
    AppleProResEncoderBlock-->MOVSinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var proResEncoderBlock = new AppleProResEncoderBlock(new AppleProResEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, proResEncoderBlock.Input);

var movSinkBlock = new MOVSinkBlock(new MOVSinkSettings(@"output.mov"));
pipeline.Connect(proResEncoderBlock.Output, movSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

macOS, iOS.

### Availability

You can check if the Apple ProRes encoder is available in your environment using:

```csharp
bool available = AppleProResEncoderBlock.IsAvailable();
```

## WMV encoder

### Overview

WMV encoder block encodes video in WMV format.

### Block info

Name: WMVEncoderBlock.

Pin direction | Media type | Pins count
--- | :---: | :---:
Input | Uncompressed video | 1
Output | video/x-wmv | 1

### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->WMVEncoderBlock;
    WMVEncoderBlock-->ASFSinkBlock;
```

### Sample code

```csharp
var pipeline = new MediaBlocksPipeline(false);

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var wmvEncoderBlock = new WMVEncoderBlock(new WMVEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, wmvEncoderBlock.Input);

var asfSinkBlock = new ASFSinkBlock(new ASFSinkSettings(@"output.wmv"));
pipeline.Connect(wmvEncoderBlock.Output, asfSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Platforms

Windows, macOS, Linux.

## General Video Settings Considerations

While specific encoder settings classes provide detailed control, some general concepts or enumerations might be relevant across different encoders or for understanding video quality options.
