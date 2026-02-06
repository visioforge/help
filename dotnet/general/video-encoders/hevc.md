---
title: HEVC Encoding with VisioForge .Net SDKs
description: Implement hardware-accelerated HEVC (H.265) encoding with AMD, NVIDIA, and Intel GPUs for efficient video compression in .NET applications.
---

# HEVC Hardware Encoding in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

This guide explores hardware-accelerated HEVC (H.265) encoding options available in VisioForge .NET SDKs. We'll cover implementation details for AMD, NVIDIA, and Intel GPU encoders, helping you choose the right solution for your video processing needs.

For Windows-specific output formats, refer to our [MP4 output documentation](../output-formats/mp4.md).

## Hardware HEVC Encoders Overview

Modern GPUs offer powerful hardware encoding capabilities that significantly outperform software-based solutions. VisioForge SDKs support three major hardware HEVC encoders:

- **AMD AMF** - For AMD Radeon GPUs
- **NVIDIA NVENC** - For NVIDIA GeForce and professional GPUs
- **Intel QuickSync** - For Intel CPUs with integrated graphics

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

## Quality Presets for Simplified Configuration

All encoders support standardized quality presets through the `VideoQuality` enum, providing a simplified configuration approach:

- **Low**: 1 Mbps target, 2 Mbps max (for basic streaming)
- **Normal**: 3 Mbps target, 5 Mbps max (for standard content)
- **High**: 6 Mbps target, 10 Mbps max (for detailed content)
- **Very High**: 15 Mbps target, 25 Mbps max (for premium quality)

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
        // Fall back to software encoder if no hardware is available
        return new SoftwareHEVCEncoderSettings(VideoQuality.High);
    }
}
```

## Best Practices for HEVC Encoding

### 1. Encoder Selection

- **AMD GPUs**: Best for applications where you know users have AMD hardware
- **NVIDIA GPUs**: Provides consistent quality across generations, ideal for professional applications
- **Intel QuickSync**: Great universal option when a dedicated GPU isn't guaranteed

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
