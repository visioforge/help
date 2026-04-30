---
title: Professional MXF Integration for .NET Applications
description: Generate broadcast MXF files in .NET with hardware acceleration, codec optimization, and professional workflows for broadcast production.
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
  - Encoding
  - Editing
  - MXF
  - H.264
  - H.265
  - AAC
  - MP3
  - C#
primary_api_classes:
  - QSVH264EncoderSettings
  - MXFOutput
  - NVENCH264EncoderSettings
  - AMFH264EncoderSettings
  - MFAACEncoderSettings

---

# MXF Output in VisioForge .NET SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Material Exchange Format (MXF) is an industry-standard container format designed for professional video applications. It's widely adopted in broadcast environments, post-production workflows, and archival systems. VisioForge SDKs provide robust cross-platform MXF output capabilities that allow developers to integrate this professional format into their applications.

## Understanding MXF Format

MXF serves as a wrapper that can contain various types of video and audio data along with metadata. The format was designed to address interoperability issues in professional video workflows:

- **Industry Standard**: Adopted by major broadcasters worldwide
- **Professional Metadata**: Supports extensive technical and descriptive metadata
- **Versatile Container**: Compatible with numerous audio and video codecs
- **Cross-Platform**: Supported across Windows, macOS, and Linux

## Getting Started with MXF Output

Two code paths cover 99% of uses:

- **`MXFOutput`** (class in `VisioForge.Core.Types.X.Output`) is a settings object consumed by `VideoCaptureCoreX.Outputs_Add(...)` or set as `VideoEditCoreX.Output_Format`.
- **`MXFSinkBlock`** + **`MXFSinkSettings`** are the Media Blocks path when you drive the pipeline by hand.

### Basic implementation

Here's the foundational code to create an MXF output:

```csharp
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);
```

This creates a valid MXF output with default encoding settings. For professional applications, you'll typically want to customize the encoding parameters.

## Video Encoding Options for MXF

The quality and compatibility of your MXF output largely depends on your choice of video encoder. VisioForge SDKs support multiple encoder options to balance performance, quality, and compatibility. For detailed configuration options, see the [H.264 encoder documentation](../video-encoders/h264.md) and [HEVC encoder documentation](../video-encoders/hevc.md).

> Video encoder `Bitrate` properties on the X namespace are in **Kbps** (so 8000 = 8 Mbps). Don't pass raw bits per second.

### Hardware-Accelerated Encoders

For optimal performance in real-time applications, hardware-accelerated encoders are recommended:

#### NVIDIA NVENC Encoders

```csharp
// Check availability first
if (NVENCH264EncoderSettings.IsAvailable())
{
    var nvencSettings = new NVENCH264EncoderSettings
    {
        Bitrate = 8000, // 8 Mbps (Kbps)
    };

    mxfOutput.Video = nvencSettings;
}
```

#### Intel Quick Sync Video (QSV) Encoders

```csharp
if (QSVH264EncoderSettings.IsAvailable())
{
    var qsvSettings = new QSVH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = qsvSettings;
}
```

#### AMD Advanced Media Framework (AMF) Encoders

```csharp
if (AMFH264EncoderSettings.IsAvailable())
{
    var amfSettings = new AMFH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = amfSettings;
}
```

### Software-Based Encoders

When hardware acceleration isn't available, software encoders provide reliable alternatives:

#### OpenH264 Encoder

```csharp
var openH264Settings = new OpenH264EncoderSettings
{
    Bitrate = 8000,
};

mxfOutput.Video = openH264Settings;
```

### High-Efficiency Video Coding (HEVC/H.265)

For applications requiring higher compression efficiency:

```csharp
// NVIDIA HEVC encoder
if (NVENCHEVCEncoderSettings.IsAvailable())
{
    var nvencHevcSettings = new NVENCHEVCEncoderSettings
    {
        Bitrate = 5000, // Lower bitrate possible with HEVC
    };

    mxfOutput.Video = nvencHevcSettings;
}
```

## Audio Encoding for MXF Files

While video often gets the most attention, proper audio encoding is crucial for professional MXF outputs. VisioForge SDKs offer multiple audio encoder options. For detailed configuration options, see the [AAC encoder documentation](../audio-encoders/aac.md) and [MP3 encoder documentation](../audio-encoders/mp3.md).

> Audio encoder `Bitrate` on the X namespace is also in **Kbps** (so 192 = 192 kbps). `MFAACEncoderSettings` and `VOAACEncoderSettings` do expose a `SampleRate` property (default 48000); only `MP3EncoderSettings` has no sample-rate setter and follows the upstream source's audio format. Channel layout on all three follows the upstream audio unless reshaped upstream (e.g., via `AudioResamplerBlock`).

### AAC Encoders

AAC is the preferred codec for most professional applications:

```csharp
// Media Foundation AAC (Windows-only)
#if NET_WINDOWS
    var mfAacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192, // kbps
    };

    mxfOutput.Audio = mfAacSettings;
#else
    // Cross-platform AAC alternative
    var voAacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
    };

    mxfOutput.Audio = voAacSettings;
#endif
```

### MP3 Encoder

For maximum compatibility:

```csharp
var mp3Settings = new MP3EncoderSettings
{
    Bitrate = 320,         // Kbps — must be one of 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320
    ForceMono = false      // Default; set true to downmix to mono
};

mxfOutput.Audio = mp3Settings;
```

## Advanced MXF Configuration

### Custom Processing Pipelines

One of the powerful features of VisioForge SDKs is the ability to add custom processing to your MXF output chain:

```csharp
// Add custom video processing
mxfOutput.CustomVideoProcessor = yourVideoProcessingBlock;

// Add custom audio processing
mxfOutput.CustomAudioProcessor = yourAudioProcessingBlock;
```

### Sink Configuration

Fine-tune your MXF output with sink settings:

```csharp
// Access sink settings (MXFSinkSettings)
mxfOutput.Sink.Filename = "new_output.mxf";
```

## Cross-Platform Considerations

Building applications that work across different platforms requires careful planning:

```csharp
// Platform-specific encoder selection
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

#if NET_WINDOWS
    if (QSVH264EncoderSettings.IsAvailable())
    {
        mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
        mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
    }
#elif NET_MACOS
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new MP3EncoderSettings { Bitrate = 320 };
#endif
```

## Error Handling and Validation

Robust MXF implementations require proper error handling:

```csharp
try
{
    // Create MXF output
    var mxfOutput = new MXFOutput(
        filename: Path.Combine(outputDirectory, "output.mxf"),
        videoStreamType: MXFVideoStreamType.H264,
        audioStreamType: MXFAudioStreamType.MPEG
    );

    // Validate encoder availability
    if (!OpenH264EncoderSettings.IsAvailable())
    {
        throw new ApplicationException("No compatible H.264 encoder found");
    }

    // Validate output directory
    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(mxfOutput.Sink.Filename));
    if (!directoryInfo.Exists)
    {
        Directory.CreateDirectory(directoryInfo.FullName);
    }

    // Attach MXFOutput as a VideoCaptureCoreX output
    videoCapture.Outputs_Add(mxfOutput, autostart: true);
    await videoCapture.StartAsync();
}
catch (Exception ex)
{
    logger.LogError($"MXF output error: {ex.Message}");
    // Implement fallback strategy
}
```

## Performance Optimization

For optimal MXF output performance:

1. **Prioritize Hardware Acceleration**: Always check for and use hardware encoders first
2. **Buffer Management**: Adjust buffer sizes based on system capabilities
3. **Parallel Processing**: Utilize multi-threading where appropriate
4. **Preset Selection**: Choose encoder presets based on quality vs. speed requirements

## Complete Implementation Example — VideoCaptureCoreX

Here's a full example demonstrating MXF implementation with fallback options:

```csharp
// Create MXF output with specific stream types
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

// Configure video encoder with prioritized fallback chain (bitrate in Kbps)
if (NVENCH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new NVENCH264EncoderSettings { Bitrate = 8000 };
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
}
else if (AMFH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new AMFH264EncoderSettings { Bitrate = 8000 };
}
else
{
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
}

// Configure platform-optimized audio (Kbps)
#if NET_WINDOWS
    mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#endif

// Attach to VideoCaptureCoreX (or VideoEditCoreX: videoEdit.Output_Format = mxfOutput;)
videoCapture.Outputs_Add(mxfOutput, autostart: true);

await videoCapture.StartAsync();
```

## Complete Implementation Example — MediaBlocksPipeline

When you're driving the pipeline by hand, use `MXFSinkBlock` + `MXFSinkSettings` instead of `MXFOutput`:

```csharp
var pipeline = new MediaBlocksPipeline();

var mxfSettings = new MXFSinkSettings("output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG);

var mxfSink = new MXFSinkBlock(mxfSettings);

// videoEncoder / audioEncoder are existing H264EncoderBlock / AACEncoderBlock instances
pipeline.Connect(videoEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

By following this guide, you can implement professional-grade MXF output in your applications using VisioForge .NET SDKs, ensuring compatibility with broadcast workflows and post-production systems.
