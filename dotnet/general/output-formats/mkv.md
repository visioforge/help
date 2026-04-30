---
title: MKV Container Format - .NET Video Encoding and Recording
description: Implement MKV output in .NET with hardware-accelerated encoding, multiple audio tracks, and flexible Matroska container support for video apps.
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
  - Recording
  - Encoding
  - Editing
  - MKV
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - MKVOutput
  - VideoCaptureCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline
  - NVENCH264EncoderSettings

---

# MKV Output in VisioForge .NET SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

## Introduction to MKV Format

MKV (Matroska Video) is a flexible, open-standard container format that can hold an unlimited number of video, audio, and subtitle tracks in one file. The VisioForge SDKs provide robust support for MKV output with various encoding options to meet diverse development requirements.

This format is particularly valuable for developers working on applications that require:

- Multiple audio tracks or languages
- High-quality video with multiple codec options
- Cross-platform compatibility
- Support for metadata and chapters

## Getting Started with MKV Output

The `MKVOutput` class serves as the primary interface for generating MKV files in VisioForge SDKs. You can initialize it with default settings or specify custom encoders to match your application's needs.

### Basic Implementation

```csharp
// Create MKV output with default encoders
var mkvOutput = new MKVOutput("output.mkv");
```

Or specify custom encoders during initialization (the second and third constructor parameters are `IVideoEncoder` / `IAudioEncoder`, both nullable):

```csharp
var videoEncoder = new NVENCH264EncoderSettings();
var audioEncoder = new MFAACEncoderSettings();
var mkvOutput = new MKVOutput("output.mkv", videoEncoder, audioEncoder);
```

## Video Encoding Options

The MKV format supports multiple video codecs, giving developers flexibility in balancing quality, performance, and compatibility. VisioForge SDKs offer both software and hardware-accelerated encoders.

### H.264 Encoder Options

H.264 remains one of the most widely supported video codecs, providing excellent compression and quality. For detailed configuration options, see the [H.264 encoder documentation](../video-encoders/h264.md).

- **OpenH264**: Software-based encoder, used as default when hardware acceleration isn't available
- **NVENC H.264**: NVIDIA GPU-accelerated encoding for superior performance
- **QSV H.264**: Intel Quick Sync Video technology for hardware acceleration
- **AMF H.264**: AMD GPU-accelerated encoding option

### HEVC (H.265) Encoder Options

For applications requiring higher compression efficiency or 4K content, see the [HEVC encoder documentation](../video-encoders/hevc.md) for detailed settings:

- **MF HEVC**: Windows Media Foundation implementation (Windows-only)
- **NVENC HEVC**: NVIDIA GPU acceleration for H.265
- **QSV HEVC**: Intel Quick Sync implementation for H.265
- **AMF HEVC**: AMD GPU acceleration for H.265 encoding

### Setting a Video Encoder

```csharp
mkvOutput.Video = new NVENCH264EncoderSettings();
```

## Audio Encoding Options

Audio quality is equally important for most applications. VisioForge SDKs provide several audio encoder options for MKV output:

### Supported Audio Codecs

- **AAC Encoders** - See [AAC encoder documentation](../audio-encoders/aac.md):
  - **VO AAC**: Default choice for non-Windows platforms
  - **AVENC AAC**: FFMPEG AAC implementation
  - **MF AAC**: Windows Media Foundation implementation (default on Windows)
  
- **Alternative Audio Formats**:
  - **[MP3](../audio-encoders/mp3.md)**: Common format with wide compatibility
  - **[Vorbis](../audio-encoders/vorbis.md)**: Open source audio codec
  - **[OPUS](../audio-encoders/opus.md)**: Modern codec with excellent quality-to-size ratio

### Configuring Audio Encoding

```csharp
// Platform-specific audio encoder selection
#if NET_WINDOWS
    var aacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 48000
    };
    mkvOutput.Audio = aacSettings;
#else
    var aacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 44100
    };
    mkvOutput.Audio = aacSettings;
#endif

// Or use OPUS for better quality at lower bitrates (Bitrate is in Kbit/s; channel count
// is inferred from the input caps, so there is no Channels property on OPUSEncoderSettings)
var opusSettings = new OPUSEncoderSettings
{
    Bitrate = 128
};
mkvOutput.Audio = opusSettings;
```

## Advanced MKV Configuration

### Custom Video and Audio Processing

For applications that require special processing, you can integrate custom MediaBlock processors:

```csharp
// Add a video processor for effects or transformations
var textOverlayBlock = new TextOverlayBlock(new TextOverlaySettings("Hello world!"));
mkvOutput.CustomVideoProcessor = textOverlayBlock;

// Add audio processing
var volumeBlock = new VolumeBlock() { Level = 1.2 }; // Boost volume by 20%
mkvOutput.CustomAudioProcessor = volumeBlock;
```

### Sink Settings Management

Control output file properties through the sink settings:

```csharp
// Change output filename
mkvOutput.Sink.Filename = "processed_output.mkv";

// Get current filename
string currentFile = mkvOutput.GetFilename();

// Update filename with timestamp
string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
mkvOutput.SetFilename($"recording_{timestamp}.mkv");
```

## Integration with VisioForge SDK Components

### With Video Capture SDK

```csharp
// Initialize capture core
var captureCore = new VideoCaptureCoreX();

// Configure video and audio source
// ...

// Add MKV output to recording pipeline
var mkvOutput = new MKVOutput("capture.mkv");
captureCore.Outputs_Add(mkvOutput, true);

// Start recording
await captureCore.StartAsync();
```

### With Video Edit SDK

```csharp
// Initialize editing core
var editCore = new VideoEditCoreX();

// Add input sources
// ...

// Configure MKV output with hardware acceleration
var h265Encoder = new NVENCHEVCEncoderSettings
{
    Bitrate = 10000
};
var mkvOutput = new MKVOutput("edited.mkv", h265Encoder);
editCore.Output_Format = mkvOutput;

// Process the file
await editCore.StartAsync();
```

### With Media Blocks SDK

`MKVOutputBlock` takes encoder **settings** (not encoder blocks) in its constructor and builds the encoder chain internally. Connect source audio/video directly to its input pads.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Outputs;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types;

// Pipeline.
var pipeline = new MediaBlocksPipeline();

// Source (any file — will be decoded into separate video and audio outputs).
var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri("input.mp4"));
var sourceBlock = new UniversalSourceBlock(sourceSettings);

// MKV sink — the constructor accepts encoder settings, not encoder blocks.
var mkvSinkSettings = new MKVSinkSettings("processed.mkv");
var mkvOutput = new MKVOutputBlock(
    mkvSinkSettings,
    videoSettings: new OpenH264EncoderSettings(),
    audioSettings: new VOAACEncoderSettings());

// Wire source pads directly to the MKV output — it handles encoding internally.
pipeline.Connect(sourceBlock.VideoOutput, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(sourceBlock.AudioOutput, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

## Hardware Acceleration Benefits

Hardware-accelerated encoding offers significant advantages for developers building real-time or batch processing applications:

1. **Reduced CPU Load**: Offloads encoding to dedicated hardware
2. **Faster Processing**: Up to 5-10x performance improvement
3. **Power Efficiency**: Lower energy consumption, important for mobile apps
4. **Higher Quality**: Some hardware encoders provide better quality-per-bitrate

## Best Practices for Developers

When implementing MKV output in your applications, consider these recommendations:

1. **Always check hardware availability** before using GPU-accelerated encoders
2. **Select appropriate bitrates** based on content type and resolution
3. **Use platform-specific encoders** where possible for optimal performance
4. **Test on target platforms** to ensure compatibility
5. **Consider quality-size trade-offs** based on your application's needs

## Conclusion

The MKV format provides developers with a flexible, robust container for video content in .NET applications. With VisioForge SDKs, you can leverage hardware acceleration, advanced encoding options, and custom processing to create high-performance video applications.

By understanding the available encoders and configuration options, you can optimize your implementation for specific hardware platforms while maintaining cross-platform compatibility where needed.
