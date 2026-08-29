---
title: HEVC Hardware Encoding on AMD, NVIDIA, Intel and Apple
description: Hardware-accelerated HEVC (H.265) encoding with AMD, NVIDIA and Intel GPUs and Apple VideoToolbox, including 10-bit and alpha, in .NET.
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
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - Webcam
  - H.265
  - C#
primary_api_classes:
  - AMFHEVCEncoderSettings
  - NVENCHEVCEncoderSettings
  - QSVHEVCEncoderSettings
  - AppleMediaHEVCEncoderSettings
  - IHEVCEncoderSettings
  - SVTHEVCEncoderSettings

---

# HEVC Hardware Encoding in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

This guide explores hardware-accelerated HEVC (H.265) encoding options available in VisioForge .NET SDKs. We'll cover implementation details for AMD, NVIDIA, and Intel GPU encoders plus Apple VideoToolbox, helping you choose the right solution for your video processing needs.

For Windows-specific output formats, refer to our [MP4 output documentation](../output-formats/mp4.md).

## Hardware HEVC Encoders Overview

Modern GPUs offer powerful hardware encoding capabilities that significantly outperform software-based solutions. VisioForge SDKs support four hardware HEVC encoders:

- **AMD AMF** - For AMD Radeon GPUs
- **NVIDIA NVENC** - For NVIDIA GeForce and professional GPUs
- **Intel QuickSync** - For Intel CPUs with integrated graphics
- **Apple VideoToolbox** - For macOS, Mac Catalyst and iOS

Each encoder provides unique features and optimization options. Let's explore their capabilities and implementation details.

## AMD AMF HEVC Encoder

AMD's Advanced Media Framework (AMF) delivers hardware-accelerated HEVC encoding on compatible Radeon GPUs. It balances encoding speed, quality, and efficiency for various scenarios.

### Key Features and Settings

- **Rate Control Methods**:
  - `CQP` (Constant QP) for fixed quality settings
  - `LCVBR` (Latency Constrained VBR) for streaming
  - `VBR` (Variable Bitrate) for offline encoding
  - `CBR` (Constant Bitrate) for reliable bandwidth usage

- **Usage Profiles**:
  - Transcoding (highest quality)
  - Ultra Low Latency (for real-time applications)
  - Low Latency (for interactive streaming)
  - Web Camera (optimized for webcam sources)

- **Quality Presets**: Balance between encoding speed and output quality

### Implementation Example

```csharp
var encoder = new AMFHEVCEncoderSettings
{
    Bitrate = 3000, // 3 Mbps target bitrate
    MaxBitrate = 5000, // 5 Mbps peak bitrate
    RateControl = AMFHEVCEncoderRateControl.CBR,
    
    // Quality optimization
    Preset = AMFHEVCEncoderPreset.Quality,
    Usage = AMFHEVCEncoderUsage.Transcoding,
    
    // GOP and frame settings
    GOPSize = 30, // Keyframe interval
    QP_I = 22, // I-frame quantization parameter
    QP_P = 22, // P-frame quantization parameter
    
    RefFrames = 1 // Reference frames count
};
```

## NVIDIA NVENC HEVC Encoder

NVIDIA's NVENC technology provides dedicated encoding hardware on GeForce and professional GPUs, offering excellent performance and quality across various bitrates.

### Key Capabilities

- **Multiple Profile Support**:
  - Main (8-bit)
  - Main10 (10-bit HDR)
  - Main444 (high color precision)
  - Extended bit depth options (12-bit)

- **Advanced Encoding Features**:
  - B-frame support with adaptive placement
  - Temporal Adaptive Quantization
  - Weighted Prediction
  - Look-ahead rate control

- **Performance Presets**: From quality-focused to ultra-fast encoding

### Implementation Example

```csharp
var encoder = new NVENCHEVCEncoderSettings
{
    // Bitrate configuration
    Bitrate = 3000, // 3 Mbps target
    MaxBitrate = 5000, // 5 Mbps maximum
    
    // Profile settings
    Profile = NVENCHEVCProfile.Main,
    Level = NVENCHEVCLevel.Level5_1,
    
    // Quality enhancement options
    BFrames = 2, // Number of B-frames
    BAdaptive = true, // Adaptive B-frame placement
    TemporalAQ = true, // Temporal adaptive quantization
    WeightedPrediction = true, // Improves quality for fades
    RCLookahead = 20, // Frames to analyze for rate control
    
    // Buffer settings
    VBVBufferSize = 0 // Use default buffer size
};
```

## Intel QuickSync HEVC Encoder

Intel QuickSync leverages the integrated GPU present in modern Intel processors for efficient hardware encoding, making it accessible without a dedicated graphics card.

### Key Features

- **Versatile Rate Control Options**:
  - `CBR` (Constant Bitrate)
  - `VBR` (Variable Bitrate)
  - `CQP` (Constant Quantizer)
  - `ICQ` (Intelligent Constant Quality)
  - `VCM` (Video Conferencing Mode)
  - `QVBR` (Quality-defined VBR)

- **Optimization Settings**:
  - Target Usage parameter (quality vs speed balance)
  - Low-latency mode for streaming
  - HDR conformance controls
  - Closed caption insertion options

- **Profile Support**:
  - Main (8-bit)
  - Main10 (10-bit HDR)

### Implementation Example

```csharp
var encoder = new QSVHEVCEncoderSettings
{
    // Bitrate settings
    Bitrate = 3000, // 3 Mbps target
    MaxBitrate = 5000, // 5 Mbps peak
    RateControl = QSVHEVCEncRateControl.VBR,
    
    // Quality tuning
    TargetUsage = 4, // 1=Best quality, 7=Fastest encoding
    
    // Stream structure
    GOPSize = 30, // Keyframe interval
    RefFrames = 2, // Reference frames
    
    // Feature configuration
    Profile = QSVHEVCEncProfile.Main,
    LowLatency = false, // Enable for streaming
    
    // Advanced options
    CCInsertMode = QSVHEVCEncSEIInsertMode.Insert,
    DisableHRDConformance = false
};
```

## Apple VideoToolbox HEVC Encoder

VideoToolbox is the hardware encoder on macOS, Mac Catalyst and iOS. It is available on
Apple Silicon and on Intel Macs with QuickSync-capable graphics.

### Key Features

- **Rate Control**:
  - `ABR` (Average Bitrate, the default)
  - `CBR` (Constant Bitrate)
  - Data rate limits - a bitrate ceiling averaged over a sliding window, in `ABR` mode

- **Profile Support**:
  - Main (8-bit)
  - Main10 (10-bit)

- **Alpha channel** - `PreserveAlpha` switches to the alpha-capable encoder

### Implementation Example

```csharp
// Constant bitrate, for a fixed-bandwidth endpoint.
var cbr = new AppleMediaHEVCEncoderSettings
{
    Bitrate = 3000,                              // 3 Mbps
    RateControl = AppleMediaRateControl.CBR,
    Profile = AppleMediaHEVCProfile.Main,
    Realtime = true,
    Quality = 0.5
};

// Average bitrate with a hard ceiling: 3 Mbps target, never more than 4.5 Mbps
// averaged over any one second.
var cappedAbr = new AppleMediaHEVCEncoderSettings
{
    Bitrate = 3000,
    RateControl = AppleMediaRateControl.ABR,     // the default
    DataRateLimitBitrate = 4500,
    DataRateLimitDuration = 1.0,
    Profile = AppleMediaHEVCProfile.Main
};
```

### CBR and the data rate limits are alternatives

Do not set both. With `CBR` selected the encoder ignores the data rate limits outright and says
so in its log (`Ignoring data-rate-limits property, CBR mode is enabled`) — constant bitrate is
already a ceiling. The limits apply in `ABR` mode, where they turn "aim for this bitrate" into
"aim for this bitrate and never exceed that one over any window of this length".

### CBR is not constant everywhere

True constant bitrate is a VideoToolbox capability, not an SDK one: it requires macOS 13+ or
iOS 16+ on Apple Silicon. On older systems and on Intel Macs, VideoToolbox emulates CBR
through data rate limits of its own, derived from `Bitrate`, and reports it in its own log
(`CBR is unsupported on your system, emulating with custom data rate limits`). The encoded
output is then near-constant rather than constant, which matters if you are sizing a fixed
transport budget around it.

### Platform notes

- **10-bit** (`AppleMediaHEVCProfile.Main10`), the **alpha** encoder and rate control all need the
  GStreamer 1.28.6 runtime, which ships with the macOS and Mac Catalyst packages. The iOS package
  still bundles 1.24.9, whose VideoToolbox encoder has none of them — rate control is discarded
  there with a warning rather than silently applied, and 10-bit fails to negotiate.
- `ForceHWUsage` pins the encoder to the hardware-only element, failing rather than falling back
  to a software implementation. It is not available on iOS.
- On the MediaBlocks pipeline path the SDK always inserts `h265parse` after this encoder:
  VideoToolbox emits `hvc1` only, while the MPEG-TS muxer behind SRT/UDP/RIST output accepts
  byte-stream only. The RTSP path needs no parser — `rtph265pay` takes `hvc1` directly.

## Quality Presets for Simplified Configuration

The AMD, NVIDIA and Intel encoders support standardized quality presets through the `VideoQuality` enum, providing a simplified configuration approach:

- **Low**: 1 Mbps target, 2 Mbps max (for basic streaming)
- **Normal**: 3 Mbps target, 5 Mbps max (for standard content)
- **High**: 6 Mbps target, 10 Mbps max (for detailed content)
- **Very High**: 15 Mbps target, 25 Mbps max (for premium quality)


`AppleMediaHEVCEncoderSettings` has no `VideoQuality` constructor — set `Bitrate` and `Quality` on it directly, as the VideoToolbox example above does.
### Using Quality Presets

```csharp
// For AMD AMF
var amfEncoder = new AMFHEVCEncoderSettings(VideoQuality.High);

// For NVIDIA NVENC
var nvencEncoder = new NVENCHEVCEncoderSettings(VideoQuality.High);

// For Intel QuickSync
var qsvEncoder = new QSVHEVCEncoderSettings(VideoQuality.High);
```

## Hardware Detection and Fallback Strategy

A robust implementation should check for encoder availability and implement appropriate fallbacks:

```csharp
// Create the most appropriate encoder for the current system
IHEVCEncoderSettings GetOptimalHEVCEncoder()
{
    if (AMFHEVCEncoderSettings.IsAvailable())
    {
        return new AMFHEVCEncoderSettings(VideoQuality.High);
    }
    else if (NVENCHEVCEncoderSettings.IsAvailable())
    {
        return new NVENCHEVCEncoderSettings(VideoQuality.High);
    }
    else if (QSVHEVCEncoderSettings.IsAvailable())
    {
        return new QSVHEVCEncoderSettings(VideoQuality.High);
    }
    else
    {
#if __MACOS__ || __MACCATALYST__ || __IOS__
        // Apple VideoToolbox.
        return new AppleMediaHEVCEncoderSettings { Bitrate = 6000 };
#elif NET_LINUX
        // Fall back to the SVT-HEVC software encoder on Linux (Linux-only — see SVTHEVCEncoderSettings).
        return new SVTHEVCEncoderSettings();
#else
        // The X engine does NOT ship a typed IHEVCEncoderSettings software fallback for Windows.
        // Let the caller decide: keep H.264 output, install a GPU driver enabling QSV/NVENC/AMF, or
        // use the classic engine's FFMPEG-EXE pipeline.
        throw new NotSupportedException("No HEVC encoder available on this platform. Install a GPU driver or switch to H.264.");
#endif
    }
}
```

## Best Practices for HEVC Encoding

### 1. Encoder Selection

- **AMD GPUs**: Best for applications where you know users have AMD hardware
- **NVIDIA GPUs**: Provides consistent quality across generations, ideal for professional applications
- **Intel QuickSync**: Great universal option when a dedicated GPU isn't guaranteed
- **Apple VideoToolbox**: The only hardware HEVC encoder on macOS, Mac Catalyst and iOS

### 2. Rate Control Selection

- **Streaming**: Use CBR for consistent bandwidth utilization
- **VoD Content**: VBR provides better quality at the same file size
- **Archival**: CQP ensures consistent quality regardless of content complexity

### 3. Performance Optimization

- Lower the reference frames count for faster encoding
- Adjust GOP size based on content type (smaller for high motion, larger for static scenes)
- Consider disabling B-frames for ultra-low latency applications

### 4. Quality Enhancement

- Enable adaptive quantization features for content with varying complexity
- Use weighted prediction for content with fades or gradual transitions
- Implement look-ahead when encoding quality is more important than latency

## Common Troubleshooting

1. **Encoder unavailability**: Ensure GPU drivers are up-to-date
2. **Lower than expected quality**: Check if quality presets match your content type
3. **Performance issues**: Monitor GPU utilization and adjust settings accordingly
4. **Compatibility problems**: Verify target devices support the selected HEVC profile

## Conclusion

Hardware-accelerated HEVC encoding offers significant performance advantages for .NET applications dealing with video processing. By leveraging AMD AMF, NVIDIA NVENC, or Intel QuickSync through VisioForge SDKs, you can achieve optimal balance between quality, speed, and efficiency.

Choose the right encoder and settings based on your specific requirements, target audience, and content type to deliver the best possible experience in your applications.

Start by detecting available hardware encoders, implementing appropriate quality settings, and testing across various content types to ensure optimal results.
