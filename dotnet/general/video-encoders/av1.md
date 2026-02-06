---
title: AV1 encoders usage in VisioForge .Net SDKs
description: Configure AV1 encoders in cross-platform Video Capture, Video Edit, and Media Blocks SDKs for efficient next-generation video compression.
---

# AV1 Encoders

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

VisioForge supports multiple AV1 encoder implementations, each with its own unique features and capabilities. This document covers the available encoders and their configuration options.

Currently, AV1 encoder are supported in the cross-platform engines: `VideoCaptureCoreX`, `VideoEditCoreX`, and `Media Blocks SDK`.

## Available Encoders

1. [AMD AMF AV1 Encoder (AMF)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AMFAV1EncoderSettings.html)
2. [NVIDIA NVENC AV1 Encoder (NVENC)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.NVENCAV1EncoderSettings.html)
3. [Intel QuickSync AV1 Encoder (QSV)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.QSVAV1EncoderSettings.html)
4. [AOM AV1 Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AOMAV1EncoderSettings.html)
5. [RAV1E Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.RAV1EEncoderSettings.html)

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

The Alliance for Open Media (AOM) AV1 encoder is a software-based reference implementation.

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

### Sample Usage

```csharp
var encoderSettings = new AOMAV1EncoderSettings
{
    BufferInitialSize = TimeSpan.FromMilliseconds(4000),
    BufferOptimalSize = TimeSpan.FromMilliseconds(5000),
    BufferSize = TimeSpan.FromMilliseconds(6000),
    CPUUsed = 4,                                   // CPU usage level
    DropFrame = 0,                                 // Disable frame dropping
    RateControl = AOMAV1EncoderEndUsageMode.VBR,   // Variable Bitrate mode
    TargetBitrate = 256,                           // 256 Kbps
    Threads = 0,                                   // Auto thread count
    UseRowMT = true,                               // Enable row-based threading
    SuperResMode = AOMAV1SuperResolutionMode.None  // No super-resolution
};
```

## RAV1E Encoder

RAV1E is a fast and safe AV1 encoder written in Rust.

### Features

- Speed preset control
- Quantizer settings
- Key frame interval control
- Low latency mode
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
    SpeedPreset = 6,                              // Speed preset (0-10)
    Tune = RAV1EEncoderTune.Psychovisual          // Psychovisual tuning
};
```

## General Usage Notes

1. All encoders implement the `IAV1EncoderSettings` interface, providing a consistent way to create encoder blocks.
2. Each encoder has its own specific set of optimizations and trade-offs.
3. Hardware encoders (AMF, NVENC, QSV) generally provide better performance but may have specific hardware requirements.
4. Software encoders (AOM, RAV1E) offer more flexibility but may require more CPU resources.

## Recommendations

- For AMD GPUs: Use AMF encoder
- For NVIDIA GPUs: Use NVENC encoder
- For Intel GPUs: Use QSV encoder
- For maximum quality: Use AOM encoder
- For CPU-efficient encoding: Use RAV1E encoder

## Best Practices

1. Always check encoder availability before using it
2. Set appropriate bitrates based on your target resolution and framerate
3. Use appropriate GOP sizes based on your content type
4. Consider the trade-off between quality and encoding speed
5. Test different rate control modes to find the best fit for your use case