---
title: MKV Container Format for .NET Video Applications
description: Learn how to implement MKV output in .NET applications with hardware-accelerated encoding, multiple audio tracks, and custom video processing. Master video and audio encoding options for high-performance multimedia applications.
sidebar_label: MKV (Matroska)

---

# MKV Output in VisioForge .NET SDKs

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

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

// Or specify custom encoders during initialization
var videoEncoder = new NVENCH264EncoderSettings();
var audioEncoder = new MFAACEncoderSettings();
var mkvOutput = new MKVOutput("output.mkv", videoEncoder, audioEncoder);
```

## Video Encoding Options

The MKV format supports multiple video codecs, giving developers flexibility in balancing quality, performance, and compatibility. VisioForge SDKs offer both software and hardware-accelerated encoders.

### H.264 Encoder Options

H.264 remains one of the most widely supported video codecs, providing excellent compression and quality:

- **OpenH264**: Software-based encoder, used as default when hardware acceleration isn't available
- **NVENC H.264**: NVIDIA GPU-accelerated encoding for superior performance
- **QSV H.264**: Intel Quick Sync Video technology for hardware acceleration
- **AMF H.264**: AMD GPU-accelerated encoding option

### HEVC (H.265) Encoder Options

For applications requiring higher compression efficiency or 4K content:

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

- **AAC Encoders**:
  - **VO AAC**: Default choice for non-Windows platforms
  - **AVENC AAC**: FFMPEG AAC implementation
  - **MF AAC**: Windows Media Foundation implementation (default on Windows)
  
- **Alternative Audio Formats**:
  - **MP3**: Common format with wide compatibility
  - **Vorbis**: Open source audio codec
  - **OPUS**: Modern codec with excellent quality-to-size ratio

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

// Or use OPUS for better quality at lower bitrates
var opusSettings = new OPUSEncoderSettings
{
    Bitrate = 128,
    Channels = 2
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

```csharp
// Create a pipeline
var pipeline = new MediaBlocksPipeline();

// Add source block
var sourceBlock = // some block

## Interface Implementation

// Configure MKV output
var aacEncoder = new VOAACEncoderSettings();
var h264Encoder = new OpenH264EncoderSettings();
var mkvSinkSettings = new MKVSinkSettings("processed.mkv");
var mkvOutput = new MKVOutputBlock(mkvSinkSettings, h264Encoder, aacEncoder);

// Connect blocks and run the pipeline
pipeline.Connect(sourceBlock.VideoOutput, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(sourceBlock.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Audio));
pipeline.Connect(mkvOutput.Output, pipeline.Sink);

// Start the pipeline
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
