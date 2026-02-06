---
title: MPEG-TS File Output Guide for .NET
description: Implement MPEG Transport Stream output in .NET with video and audio encoding, hardware acceleration, and cross-platform streaming support.
---

# MPEG-TS Output

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The MPEG-TS (Transport Stream) output module in VisioForge SDK functionality for creating MPEG transport stream files with various video and audio encoding options. This guide explains how to configure and use the `MPEGTSOutput` class effectively.

## Cross-platform MPEG-TS output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

To create a new MPEG-TS output, use the following constructor:

```csharp
// Initialize with AAC audio (recommended)
var output = new MPEGTSOutput("output.ts", useAAC: true);
```

You can also use MP3 audio instead of AAC:

```csharp
// Initialize with MP3 audio instead of AAC
var output = new MPEGTSOutput("output.ts", useAAC: false);
```

### Video Encoding Options

The [MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) supports multiple video encoders through the `Video` property. Available encoders include:

**[H.264 Encoders](../video-encoders/h264.md)**

- OpenH264 (Software-based)
- NVENC H.264 (NVIDIA GPU acceleration)
- QSV H.264 (Intel Quick Sync)
- AMF H.264 (AMD GPU acceleration)

**[H.265/HEVC Encoders](../video-encoders/hevc.md)**

- MF HEVC (Windows Media Foundation, Windows only)
- NVENC HEVC (NVIDIA GPU acceleration)
- QSV HEVC (Intel Quick Sync)
- AMF H.265 (AMD GPU acceleration)

Example of setting a specific video encoder:

```csharp
// Check if NVIDIA encoder is available
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else
{
    // Fall back to OpenH264
    output.Video = new OpenH264EncoderSettings();
}
```

### Audio Encoding Options

The following audio encoders are supported through the `Audio` property:

**[AAC Encoders](../audio-encoders/aac.md)**

- VO-AAC (Cross-platform)
- AVENC AAC
- MF AAC (Windows only)
  
**[MP3 Encoder](../audio-encoders/mp3.md)**:

- MP3EncoderSettings

Example of setting an audio encoder:

```csharp
// For Windows platforms
output.Audio = new MFAACEncoderSettings();
```

```csharp
// For cross-platform compatibility
output.Audio = new VOAACEncoderSettings();
```

```csharp
// Using MP3 instead of AAC
output.Audio = new MP3EncoderSettings();
```

### File Management

You can get or set the output filename after initialization:

```csharp
// Get current filename
string currentFile = output.GetFilename();

// Change output filename
output.SetFilename("new_output.ts");
```

### File Splitting

The `MPEGTSSplitSinkSettings` class enables automatic splitting of MPEG-TS output into multiple files based on size, duration, or timecode. This is useful for:

- Creating segmented files for HLS streaming
- Managing storage by limiting file sizes
- Recording time-lapse videos
- Implementing rolling buffer recording

#### Configuration Options

```csharp
using VisioForge.Core.Types.X.Sinks;

// Create split settings with filename pattern
// %05d will be replaced with segment number (00000, 00001, etc.)
var splitSettings = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    // Split by duration (e.g., every 5 minutes)
    SplitDuration = TimeSpan.FromMinutes(5),
    
    // Split by file size (e.g., 100 MB, 0 = disabled)
    SplitFileSize = 100 * 1024 * 1024,
    
    // Maximum number of files to keep (older files deleted, 0 = unlimited)
    SplitMaxFiles = 10,
    
    // Split by timecode difference (optional)
    SplitMaxSizeTimecode = "01:00:00:00", // 1 hour
    
    // Starting index for segment numbering
    StartIndex = 0,
    
    // M2TS mode (Blu-ray format with 192-byte packets)
    M2TSMode = false
};

// Apply to output
output.Sink = splitSettings;
```

#### Split Triggers

Files will be split when any of these conditions are met:

1. **Duration-based**: `SplitDuration` - New file created after specified time
2. **Size-based**: `SplitFileSize` - New file created when size limit reached
3. **Timecode-based**: `SplitMaxSizeTimecode` - New file when timecode difference reached

#### Filename Pattern

The filename pattern uses printf-style formatting for segment numbers:

```csharp
// Examples of filename patterns
"recording_%02d.ts"   // recording_00.ts, recording_01.ts, ...
"stream_%05d.ts"      // stream_00000.ts, stream_00001.ts, ...
"output_%d.ts"        // output_0.ts, output_1.ts, ...
```

#### Rolling Buffer Recording

To implement a rolling buffer that keeps only the last N segments:

```csharp
var settings = new MPEGTSSplitSinkSettings("buffer_%03d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(1),  // 1 minute segments
    SplitMaxFiles = 60  // Keep last 60 minutes
};
```

#### Usage Example

```csharp
// Complete example with split settings
var output = new MPEGTSOutput("video_%05d.ts", useAAC: true);

// Configure split settings
output.Sink = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(5),
    SplitMaxFiles = 12,  // Keep last hour (12 x 5 minutes)
    M2TSMode = false
};

// Configure encoders
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}

// Use with VideoCaptureCoreX or MediaBlocksPipeline
// The filename pattern will be used automatically
```

### Advanced Features

#### Custom Processing

The MPEGTSOutput supports custom video and audio processing through MediaBlocks:

```csharp
// Add custom video processing
output.CustomVideoProcessor = new YourCustomVideoProcessor();

// Add custom audio processing
output.CustomAudioProcessor = new YourCustomAudioProcessor();
```

#### Sink Settings

The output uses MP4SinkSettings for configuration:

```csharp
// Access sink settings
output.Sink.Filename = "modified_output.ts";
```

### Platform Considerations

- Some encoders (MF AAC, MF HEVC) are only available on Windows platforms
- Cross-platform applications should use platform-agnostic encoders like VO-AAC for audio

### Best Practices

1. **Hardware Acceleration**: When available, prefer hardware-accelerated encoders (NVENC, QSV, AMF) over software encoders for better performance.

2. **Audio Codec Selection**: Use AAC for better compatibility and quality unless you have specific requirements for MP3.

3. **Error Handling**: Always check for encoder availability before using hardware-accelerated options:

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    // Use NVIDIA encoder
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    // Fall back to Intel Quick Sync
}
else
{
    // Fall back to software encoding
}
```

**Cross-Platform Compatibility**: For cross-platform applications, ensure you're using encoders available on all target platforms or implement appropriate fallbacks.

### Implementation Example

Here's a complete example showing how to create and configure an MPEG-TS output:

```csharp
var output = new MPEGTSOutput("output.ts", useAAC: true);

// Configure video encoder
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    output.Video = new QSVH264EncoderSettings();
}
else
{
    output.Video = new OpenH264EncoderSettings();
}

// Configure audio encoder based on platform
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new VOAACEncoderSettings();
#endif

// Optional: Add custom processing
output.CustomVideoProcessor = new YourCustomVideoProcessor();
output.CustomAudioProcessor = new YourCustomAudioProcessor();
```

## Windows-only MPEG-TS output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The `MPEGTSOutput` class provides configuration settings for MPEG Transport Stream (MPEG-TS) output in the VisioForge video processing framework. This class inherits from `MFBaseOutput` and implements the `IVideoCaptureBaseOutput` interface, enabling it to be used specifically for video capture scenarios with MPEG-TS formatting.

### Class Hierarchy

```text
MFBaseOutput
    └── MPEGTSOutput
```

### Inherited Video Settings

The [MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) class inherits video encoding capabilities from MFBaseOutput, which includes:

**Video Encoding Configuration**: Through the `Video` property, supporting:

- Multiple codec options (H.264/H.265) with hardware acceleration support
- Bitrate control (CBR/VBR)
- Quality settings
- Frame type and GOP structure configuration
- Interlacing options
- Resolution and aspect ratio controls

### Inherited Audio Settings

Audio configuration is handled through the inherited `Audio` property of type M4AOutput, which includes:

AAC audio encoding with configurable:

- Version (default: MPEG-4)
- Object type (default: AAC-LC)
- Bitrate (default: 128 kbps)
- Output format (default: RAW)

### Usage

#### Basic Implementation

```csharp
// Create VideoCaptureCore instance
var core = new VideoCaptureCore();

// Set output filename
core.Output_Filename = "output.ts";

// Create MPEG-TS output
var mpegtsOutput = new MPEGTSOutput();

// Configure video settings
mpegtsOutput.Video.Codec = MFVideoEncoder.MS_H264;
mpegtsOutput.Video.AvgBitrate = 2000; // 2 Mbps
mpegtsOutput.Video.RateControl = MFCommonRateControlMode.CBR;

// Configure audio settings
mpegtsOutput.Audio.Bitrate = 128; // 128 kbps
mpegtsOutput.Audio.Version = AACVersion.MPEG4;

core.Output_Format = mpegtsOutput;
```

#### Serialization Support

The class provides built-in JSON serialization support for saving and loading configurations:

```csharp
// Save configuration
string jsonConfig = mpegtsOutput.Save();

// Load configuration
MPEGTSOutput loadedConfig = MPEGTSOutput.Load(jsonConfig);
```

### Default Configuration

The `MPEGTSOutput` class initializes with these default settings:

#### Video Defaults (inherited from MFBaseOutput)

- Average Bitrate: 2000 kbps
- Codec: Microsoft H.264
- Profile: Main
- Level: 4.2
- Rate Control: CBR
- Quality vs Speed: 85
- Maximum Reference Frames: 2
- GOP Size: 50 frames
- B-Picture Count: 0
- Low Latency Mode: Disabled
- CABAC: Disabled
- Interlace Mode: Progressive

#### Audio Defaults

- Bitrate: 128 kbps
- AAC Version: MPEG-4
- AAC Object Type: Low Complexity (LC)
- Output Format: RAW

### Best Practices

1. **Bitrate Configuration**:
   - For streaming applications, ensure the combined video and audio bitrates are within your target bandwidth
   - Consider using VBR for storage scenarios and CBR for streaming

2. **Hardware Acceleration**:
   - When available, use hardware-accelerated encoders (QSV, NVENC, AMD) for better performance
   - Fall back to MS_H264/MS_H265 when hardware acceleration is unavailable

3. **Quality Optimization**:
   - For higher quality at the cost of performance, increase the `QualityVsSpeed` value
   - Enable CABAC for better compression efficiency in non-low-latency scenarios
   - Adjust `MaxKeyFrameSpacing` based on your specific use case (lower values for better seeking, higher values for better compression)

### Technical Notes

1. **MPEG-TS Characteristics**:
   - Suitable for streaming and broadcasting applications
   - Provides error resilience through packet-based structure
   - Supports multiple programs and elementary streams

2. **Performance Considerations**:
   - Low latency mode trades quality for reduced encoding delay
   - B-frames improve compression but increase latency
   - Hardware acceleration can significantly reduce CPU usage

### Required redists  

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.