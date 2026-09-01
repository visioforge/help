---
title: AV1 Video Encoding in C# .NET - Configuration Guide
description: Configure AV1 encoders for cross-platform video capture, editing, and media pipelines. Next-gen compression with VisioForge SDK C# code examples.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - AV1
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoEditCoreX
  - AMFAV1EncoderSettings
  - NVENCAV1EncoderSettings
  - QSVAV1EncoderSettings

---

# AV1 Encoders

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

VisioForge supports multiple AV1 encoder implementations, each with its own unique features and capabilities. This document covers the available encoders and their configuration options.

Currently, AV1 encoder are supported in the cross-platform engines: `VideoCaptureCoreX`, `VideoEditCoreX`, and `Media Blocks SDK`.

!!! note "Not available on iOS"

    AV1 encoding is not available on iOS. Apple ships no AV1 hardware encoder on any chip, and the
    software AV1 encoders are not part of the iOS redistributable. AV1 *decoding* works on iOS -
    through VideoToolbox on A17 Pro and later, and through the software `dav1d` decoder everywhere
    else.

## Available Encoders

1. [AMD AMF AV1 Encoder (AMF)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AMFAV1EncoderSettings.html)
2. [NVIDIA NVENC AV1 Encoder (NVENC)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.NVENCAV1EncoderSettings.html)
3. [Intel QuickSync AV1 Encoder (QSV)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.QSVAV1EncoderSettings.html)
4. [AOM AV1 Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AOMAV1EncoderSettings.html)
5. [RAV1E Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.RAV1EEncoderSettings.html)
6. [SVT-AV1 Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.SVTAV1EncoderSettings.html)

The SVT-AV1 encoder is delivered in the additional runtime package for your platform - the `VisioForge.CrossPlatform.Core.Windows.Adds.*` package for your architecture, `VisioForge.CrossPlatform.Core.macOS.Adds` (2026.8.25 or newer) or `VisioForge.CrossPlatform.Core.macCatalyst.Adds` (2026.8.26 or newer) - which is referenced beside the core package. Without it `AV1EncoderBlock.IsAvailable(new SVTAV1EncoderSettings())` returns `false`.

The AOM AV1 encoder is available on macOS, macCatalyst, and Linux. It is a quality-first software encoder intended for offline encoding; for the usual software AV1 workflow, prefer SVT-AV1 because it is substantially faster and scales better across CPU cores.

The `VisioForge.CrossPlatform.Core.macOS.Adds` and `VisioForge.CrossPlatform.Core.macCatalyst.Adds` packages include the required AOM runtime from 2026.8.30 onwards. On Linux, install a GStreamer runtime that provides the `av1enc` element; use `AOMAV1EncoderSettings.IsAvailable()` to check the active runtime.

You can use AV1 encoder with [WebM output](../output-formats/webm.md) or for network streaming.

## AMD AMF AV1 Encoder

The AMD AMF AV1 encoder provides hardware-accelerated encoding using AMD graphics cards.

### Features

- Multiple quality presets
- Variable bitrate control modes
- GOP size control
- QP (Quantization Parameter) control
- Smart Access Video support

### Rate Control Modes

- `Default`: Depends on Usage
- `CQP`: Constant QP
- `LCVBR`: Latency Constrained VBR
- `VBR`: Peak Constrained VBR
- `CBR`: Constant Bitrate

### Sample Usage

```csharp
var encoderSettings = new AMFAV1EncoderSettings
{
    Bitrate = 3000,                              // 3 Mbps
    GOPSize = 30,                                // GOP size of 30 frames
    Preset = AMFAV1EncoderPreset.Quality,        // Quality preset
    RateControl = AMFAV1RateControlMode.VBR,     // Variable Bitrate mode
    Usage = AMFAV1EncoderUsage.Transcoding,      // Transcoding usage
    MaxBitrate = 5000,                           // 5 Mbps max bitrate
    QpI = 26,                                    // I-frame QP
    QpP = 26,                                    // P-frame QP
    RefFrames = 1,                               // Number of reference frames
    SmartAccessVideo = false                     // Smart Access Video disabled
};
```

## NVIDIA NVENC AV1 Encoder

NVIDIA's NVENC AV1 encoder provides hardware-accelerated encoding using NVIDIA GPUs.

### Features

- Multiple encoding presets
- Adaptive B-frame support
- Temporal AQ (Adaptive Quantization)
- VBV (Video Buffering Verifier) buffer control
- Spatial AQ support

### Rate Control Modes

- `Default`: Default mode
- `ConstQP`: Constant Quantization Parameter
- `CBR`: Constant Bitrate
- `VBR`: Variable Bitrate
- `CBR_LD_HQ`: Low-delay CBR, high quality
- `CBR_HQ`: CBR, high quality (slower)
- `VBR_HQ`: VBR, high quality (slower)

### Sample Usage

```csharp
var encoderSettings = new NVENCAV1EncoderSettings
{
    Bitrate = 3000,                          // 3 Mbps
    Preset = NVENCPreset.HighQuality,        // High quality preset
    RateControl = NVENCRateControl.VBR,      // Variable Bitrate mode
    GOPSize = 75,                            // GOP size of 75 frames
    MaxBitrate = 5000,                       // 5 Mbps max bitrate
    BFrames = 2,                             // 2 B-frames between I and P
    RCLookahead = 8,                         // 8 frames lookahead
    TemporalAQ = true,                       // Enable temporal AQ
    Tune = NVENCTune.HighQuality,            // High quality tuning
    VBVBufferSize = 6000                     // 6000k VBV buffer
};
```

## Intel QuickSync AV1 Encoder

Intel's QuickSync AV1 encoder provides hardware-accelerated encoding using Intel GPUs.

### Features

- Low latency mode support
- Configurable target usage
- Reference frame control
- Flexible GOP size settings

### Rate Control Modes

- `CBR`: Constant Bitrate
- `VBR`: Variable Bitrate
- `CQP`: Constant Quantizer

### Sample Usage

```csharp
var encoderSettings = new QSVAV1EncoderSettings
{
    Bitrate = 2000,                              // 2 Mbps
    LowLatency = false,                          // Standard latency mode
    TargetUsage = 4,                             // Balanced quality/speed
    GOPSize = 30,                                // GOP size of 30 frames
    MaxBitrate = 4000,                           // 4 Mbps max bitrate
    QPI = 26,                                    // I-frame QP
    QPP = 28,                                    // P-frame QP
    RateControl = QSVAV1EncRateControl.VBR,      // Variable Bitrate mode
    RefFrames = 1                                // Number of reference frames
};
```

## AOM AV1 Encoder

The Alliance for Open Media (AOM) AV1 encoder is a software-based reference implementation. It prioritizes compression efficiency and detailed AV1 controls over encoding speed, so it is best suited to offline output rather than live capture or streaming.

### Features

- Buffer control settings
- CPU usage optimization
- Frame dropping support
- Multi-threading capabilities
- Super-resolution support

### Rate Control Modes

- `VBR`: Variable Bit Rate Mode
- `CBR`: Constant Bit Rate Mode
- `CQ`: Constrained Quality Mode
- `Q`: Constant Quality Mode

Rate control only works while `MaxQuantizer` leaves the encoder room to spend fewer bits. It defaults to 63, the highest quantizer AV1 has; lowering it puts a quality floor under the stream, and at 0 the encoder ignores `TargetBitrate` and every buffer and overshoot setting. `TargetBitrate` defaults to 0, which means the encoder scales its own default to the frame size (256 Kbps at 320x240, about 6900 Kbps at 1920x1080); set it in kilobits per second to pin it instead.

### Sample Usage

```csharp
var encoderSettings = new AOMAV1EncoderSettings
{
    BufferInitialSize = TimeSpan.FromMilliseconds(4000),
    BufferOptimalSize = TimeSpan.FromMilliseconds(5000),
    BufferSize = TimeSpan.FromMilliseconds(6000),
    CPUUsed = 4,                                   // CPU usage level, 0-9
    DropFrame = 0,                                 // Disable frame dropping
    RateControl = AOMAV1EncoderEndUsageMode.VBR,   // Variable Bitrate mode
    MaxQuantizer = 63,                             // Quantizer ceiling - keep at 63 for rate control
    TargetBitrate = 0,                             // 0 = scale the encoder default to the frame size
    Threads = 0,                                   // Auto thread count
    UseRowMT = true,                               // Enable row-based threading
    SuperResMode = AOMAV1SuperResolutionMode.None  // No super-resolution
};
```

## RAV1E Encoder

RAV1E is a fast and safe AV1 encoder written in Rust.

`Tiles` is what lets it use more than one core: a single tile encodes at the same speed on 4 cores and on 32. The default of 16 encodes 8 seconds of 720p30 in about 29 seconds against 84 seconds untiled, for roughly 1% more bitrate. Set it to 0 to encode the frame as a single tile.

### Features

- Speed preset control
- Quantizer settings
- Key frame interval control
- Low latency mode
- Tile-based multi-core encoding
- Psychovisual tuning

### Sample Usage

```csharp
var encoderSettings = new RAV1EEncoderSettings
{
    Bitrate = 3000,                               // 3 Mbps
    LowLatency = false,                           // Standard latency mode
    MaxKeyFrameInterval = 240,                    // Maximum keyframe interval
    MinKeyFrameInterval = 12,                     // Minimum keyframe interval
    MinQuantizer = 0,                             // Minimum quantizer value
    Quantizer = 100,                              // Base quantizer value
    SpeedPreset = 10,                             // Speed preset (0-10)
    Tiles = 16,                                   // Tiles the frame is split into (0 - one tile)
    Tune = RAV1EEncoderTune.Psychovisual          // Psychovisual tuning
};
```

## SVT-AV1 Encoder

SVT-AV1 (Scalable Video Technology for AV1) is a high-performance software AV1 encoder that scales across CPU cores. It is considerably faster than RAV1E at comparable quality.

### Features

- Speed/quality preset control (0-13)
- Three mutually exclusive rate-control modes: CQP, CRF, and CBR/VBR by bitrate
- Intra frame period and tiling control
- Explicit logical-core count

### Rate Control Modes

Set exactly one of the three - the settings are applied in this order and the first one present wins:

1. `CQP` - constant quantization parameter (1-63)
2. `TargetBitrate` (with the optional `MaxBitrate` ceiling for VBR) - both in kbits/sec
3. `CRF` - constant rate factor (1-63, default 35), used when neither of the above is set

### Sample Usage

```csharp
var encoderSettings = new SVTAV1EncoderSettings
{
    TargetBitrate = 3000,                         // 3 Mbps (kbits/sec), enables CBR/VBR
    MaxBitrate = 4500,                            // VBR ceiling (kbits/sec)
    Preset = 8,                                   // Speed preset (0-13), lower is slower and better
    IntraPeriodLength = 240,                      // Intra frame period, -2 for automatic
    TileColumns = 1,                              // log2: two tile columns
    TileRows = 1                                  // log2: two tile rows
};
```

## General Usage Notes

1. All encoders implement the `IAV1EncoderSettings` interface, providing a consistent way to create encoder blocks.
2. Each encoder has its own specific set of optimizations and trade-offs.
3. Hardware encoders (AMF, NVENC, QSV) generally provide better performance but may have specific hardware requirements.
4. Software encoders (AOM, RAV1E, SVT-AV1) offer more flexibility but may require more CPU resources.

## Recommendations

- For AMD GPUs: Use AMF encoder
- For NVIDIA GPUs: Use NVENC encoder
- For Intel GPUs: Use QSV encoder
- For maximum quality: Use AOM encoder
- For CPU-efficient encoding: Use RAV1E encoder
- For fast software encoding: Use SVT-AV1 encoder

## Best Practices

1. Always check encoder availability before using it
2. Set appropriate bitrates based on your target resolution and framerate
3. Use appropriate GOP sizes based on your content type
4. Consider the trade-off between quality and encoding speed
5. Test different rate control modes to find the best fit for your use case
