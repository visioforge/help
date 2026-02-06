---
title: Blackmagic Decklink Integration for .NET Developers
description: Integrate Blackmagic Decklink devices for professional SDI and HDMI capture and rendering with multi-device support in .NET applications.
sidebar_label: Blackmagic Decklink

---

# Blackmagic Decklink Integration with Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Decklink Integration

The VisioForge Media Blocks SDK for .NET provides robust support for Blackmagic Decklink devices, enabling developers to implement professional-grade audio and video functionality in their applications. This integration allows for seamless capture and rendering operations using Decklink's high-quality hardware.

Our SDK includes specialized blocks designed specifically for Decklink devices, giving you full access to their capabilities including SDI, HDMI, and other inputs/outputs. These blocks are optimized for performance and offer a straightforward API for implementing complex media workflows.

### Key Capabilities

- **Audio Capture and Rendering**: Capture and output audio through Decklink devices
- **Video Capture and Rendering**: Capture and output video in various formats and resolutions
- **Multiple Device Support**: Work with multiple Decklink devices simultaneously
- **Professional I/O Options**: Utilize SDI, HDMI, and other professional interfaces
- **High-Quality Processing**: Maintain professional video/audio quality throughout the pipeline
- **Combined Audio/Video Blocks**: Simplified handling of synchronized audio and video streams with dedicated source and sink blocks.

## System Requirements

Before using the Decklink blocks, ensure your system meets these requirements:

- **Hardware**: Compatible Blackmagic Decklink device
- **Software**: Blackmagic Decklink SDK or drivers installed

## Decklink Block Types

The SDK provides several block types for working with Decklink devices:

1. **Audio Sink Block**: For audio output to Decklink devices.
2. **Audio Source Block**: For audio capture from Decklink devices.
3. **Video Sink Block**: For video output to Decklink devices.
4. **Video Source Block**: For video capture from Decklink devices.
5. **Video + Audio Sink Block**: For synchronized video and audio output to Decklink devices using a single block.
6. **Video + Audio Source Block**: For synchronized video and audio capture from Decklink devices using a single block.

Each block type is designed to handle specific media operations while maintaining synchronization and quality.

## Working with Decklink Audio Sink Block

The Decklink Audio Sink block enables audio output to Blackmagic Decklink devices. This block handles the complexities of audio timing and device interfacing.

### Device Enumeration

Before creating an audio sink block, you'll need to enumerate available devices:

```csharp
var devices = await DecklinkAudioSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Found device: {item.Name}, Device Number: {item.DeviceNumber}");
}
```

This code retrieves all available Decklink devices that support audio output functionality.

### Block Creation and Configuration

Once you've identified the target device, you can create and configure the audio sink block:

```csharp
// Get the first available device
var deviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Create settings for the selected device
DecklinkAudioSinkSettings audioSinkSettings = null;
if (deviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(deviceInfo);
    // Example: audioSinkSettings.DeviceNumber = deviceInfo.DeviceNumber; (already set by constructor)
    // Further configuration:
    // audioSinkSettings.BufferTime = TimeSpan.FromMilliseconds(100);
    // audioSinkSettings.IsSync = true;
}

// Create the block with configured settings
var decklinkAudioSink = new DecklinkAudioSinkBlock(audioSinkSettings);
```

### Key Audio Sink Settings

The `DecklinkAudioSinkSettings` class includes properties like:

- `DeviceNumber`: The output device instance to use.
- `BufferTime`: Minimum latency reported by the sink (default: 50ms).
- `AlignmentThreshold`: Timestamp alignment threshold (default: 40ms).
- `DiscontWait`: Time to wait before creating a discontinuity (default: 1s).
- `IsSync`: Enables synchronization (default: true).

### Connecting to the Pipeline

The audio sink block includes an `Input` pad that accepts audio data from other blocks in your pipeline:

```csharp
// Example: Connect an audio source/encoder to the Decklink audio sink
audioEncoder.Output.Connect(decklinkAudioSink.Input);
```

## Working with Decklink Audio Source Block

The Decklink Audio Source block enables capturing audio from Blackmagic Decklink devices. It supports various audio formats and configurations.

### Device Enumeration

Enumerate available audio source devices:

```csharp
var devices = await DecklinkAudioSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Available audio source: {item.Name}, Device Number: {item.DeviceNumber}");
}
```

### Block Creation and Configuration

Create and configure the audio source block:

```csharp
// Get the first available device
var deviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Create settings for the selected device
DecklinkAudioSourceSettings audioSourceSettings = null;
if (deviceInfo != null)
{
    // create settings object
    audioSourceSettings = new DecklinkAudioSourceSettings(deviceInfo);
    // Further configuration:
    // audioSourceSettings.Channels = DecklinkAudioChannels.Ch2;
    // audioSourceSettings.Connection = DecklinkAudioConnection.Embedded;
    // audioSourceSettings.Format = DecklinkAudioFormat.S16LE; // SampleRate is fixed at 48000
}

// Create the block with the configured settings
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);
```

### Key Audio Source Settings

The `DecklinkAudioSourceSettings` class includes properties like:

- `DeviceNumber`: The input device instance to use.
- `Channels`: Audio channels to capture (e.g., `DecklinkAudioChannels.Ch2`, `Ch8`, `Ch16`). Default `Ch2`.
- `Format`: Audio sample format (e.g., `DecklinkAudioFormat.S16LE`). Default `S16LE`. Sample rate is fixed at 48000 Hz.
- `Connection`: Audio connection type (e.g., `DecklinkAudioConnection.Embedded`, `AES`, `Analog`). Default `Auto`.
- `BufferSize`: Internal buffer size in frames (default: 5).
- `DisableAudioConversion`: Set to `true` to disable internal audio conversion. Default `false`.

### Connecting to the Pipeline

The audio source block provides an `Output` pad that can connect to other blocks:

```csharp
// Example: Connect the audio source to an audio encoder or processor
audioSource.Output.Connect(audioProcessor.Input);
```

## Working with Decklink Video Sink Block

The Decklink Video Sink block enables video output to Blackmagic Decklink devices, supporting various video formats and resolutions.

### Device Enumeration

Find available video sink devices:

```csharp
var devices = await DecklinkVideoSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Available video output device: {item.Name}, Device Number: {item.DeviceNumber}");
}
```

### Block Creation and Configuration

Create and configure the video sink block:

```csharp
// Get the first available device
var deviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Create settings for the selected device
// Note: Mode is required and must be specified in the constructor
DecklinkVideoSinkSettings videoSinkSettings = null;
if (deviceInfo != null)
{
    // Mode is required - specify the output video resolution and frame rate
    videoSinkSettings = new DecklinkVideoSinkSettings(deviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_10bit,
        // Optional: Additional configuration
        // KeyerMode = DecklinkKeyerMode.Internal,
        // KeyerLevel = 128,
        // Profile = DecklinkProfileID.Default,
        // TimecodeFormat = DecklinkTimecodeFormat.RP188Any
    };
}

// Create the block with the configured settings
var decklinkVideoSink = new DecklinkVideoSinkBlock(videoSinkSettings);
```

### Key Video Sink Settings

The `DecklinkVideoSinkSettings` class includes properties like:

- `DeviceNumber`: The output device instance to use (read-only, set via constructor).
- `Mode`: Specifies the video resolution and frame rate (e.g., `DecklinkMode.HD1080i60`, `HD720p60`). **Required** - must be specified in the constructor.
- `VideoFormat`: Defines the pixel format using `DecklinkVideoFormat` enum (e.g., `DecklinkVideoFormat.YUV_8bit`, `YUV_10bit`). Default `YUV_8bit`.
- `KeyerMode`: Controls keying/compositing options using `DecklinkKeyerMode` (if supported by the device). Default `Off`.
- `KeyerLevel`: Sets the keyer level (0-255). Default `255`.
- `Profile`: Specifies the Decklink profile to use with `DecklinkProfileID`.
- `TimecodeFormat`: Specifies the timecode format for playback using `DecklinkTimecodeFormat`. Default `RP188Any`.
- `CustomVideoSize`: Optional resize effect to apply before output.
- `CustomFrameRate`: Optional frame rate conversion before output.
- `IsSync`: Enables synchronization (default: true).

**Important**: The `Mode` parameter is required and determines the output frame rate and resolution. If not specified correctly, the Decklink hardware may output at an unexpected frame rate.

## Working with Decklink Video Source Block

The Decklink Video Source block allows capturing video from Blackmagic Decklink devices, supporting various input formats and resolutions.

### Device Enumeration

Enumerate video capture devices:

```csharp
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Available video capture device: {item.Name}, Device Number: {item.DeviceNumber}");
}
```

### Block Creation and Configuration

Create and configure the video source block:

```csharp
// Get the first available device
var deviceInfo = (await DecklinkVideoSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Create settings for the selected device
DecklinkVideoSourceSettings videoSourceSettings = null;
if (deviceInfo != null)
{
    videoSourceSettings = new DecklinkVideoSourceSettings(deviceInfo);
    
    // Configure video input format and mode
    videoSourceSettings.Mode = DecklinkMode.HD1080i60;
    videoSourceSettings.Connection = DecklinkConnection.SDI; 
    // videoSourceSettings.VideoFormat = DecklinkVideoFormat.Auto; // Often used with Mode=Auto
}

// Create the block with configured settings
var videoSourceBlock = new DecklinkVideoSourceBlock(videoSourceSettings);
```

### Key Video Source Settings

The `DecklinkVideoSourceSettings` class includes properties like:

- `DeviceNumber`: The input device instance to use.
- `Mode`: Specifies the expected input resolution and frame rate (e.g., `DecklinkMode.HD1080i60`). Default `Unknown`.
- `Connection`: Defines which physical input to use, using `DecklinkConnection` enum (e.g., `DecklinkConnection.HDMI`, `DecklinkConnection.SDI`). Default `Auto`.
- `VideoFormat`: Specifies the video format type for input, using `DecklinkVideoFormat` enum. Default `Auto` (especially when `Mode` is `Auto`).
- `Profile`: Specifies the Decklink profile using `DecklinkProfileID`. Default `Default`.
- `DropNoSignalFrames`: If `true`, drops frames marked as having no input signal. Default `false`.
- `OutputAFDBar`: If `true`, extracts and outputs AFD/Bar data as Meta. Default `false`.
- `OutputCC`: If `true`, extracts and outputs Closed Captions as Meta. Default `false`.
- `TimecodeFormat`: Specifies the timecode format using `DecklinkTimecodeFormat`. Default `RP188Any`.
- `DisableVideoConversion`: Set to `true` to disable internal video conversion. Default `false`.

## Working with Decklink Video + Audio Source Block

The `DecklinkVideoAudioSourceBlock` simplifies capturing synchronized video and audio streams from a single Decklink device.

### Device Enumeration and Configuration

Device selection is managed through `DecklinkVideoSourceSettings` and `DecklinkAudioSourceSettings`. You would typically enumerate video devices using `DecklinkVideoSourceBlock.GetDevicesAsync()` and audio devices using `DecklinkAudioSourceBlock.GetDevicesAsync()`, then configure the respective settings objects for the chosen device. The `DecklinkVideoAudioSourceBlock` itself also provides `GetDevicesAsync()` which enumerates video sources.

```csharp
// Enumerate video devices (for video part of the combined source)
var videoDeviceInfo = (await DecklinkVideoAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(); // or DecklinkVideoSourceBlock.GetDevicesAsync()
var audioDeviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoDeviceInfo.DeviceNumber); // Example: match by device number

DecklinkVideoSourceSettings videoSettings = null;
if (videoDeviceInfo != null)
{
    videoSettings = new DecklinkVideoSourceSettings(videoDeviceInfo);
    videoSettings.Mode = DecklinkMode.HD1080i60;
    videoSettings.Connection = DecklinkConnection.SDI;
}

DecklinkAudioSourceSettings audioSettings = null;
if (audioDeviceInfo != null)
{
    audioSettings = new DecklinkAudioSourceSettings(audioDeviceInfo);
    audioSettings.Channels = DecklinkAudioChannels.Ch2;
}

// Create the block with configured settings
if (videoSettings != null && audioSettings != null)
{
    var decklinkVideoAudioSource = new DecklinkVideoAudioSourceBlock(videoSettings, audioSettings);

    // Connect outputs
    // decklinkVideoAudioSource.VideoOutput.Connect(videoProcessor.Input);
    // decklinkVideoAudioSource.AudioOutput.Connect(audioProcessor.Input);
}
```

### Block Creation and Configuration

You instantiate `DecklinkVideoAudioSourceBlock` by providing pre-configured `DecklinkVideoSourceSettings` and `DecklinkAudioSourceSettings` objects.

```csharp
// Assuming videoSourceSettings and audioSourceSettings are configured as above
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);
```

### Connecting to the Pipeline

The block provides separate `VideoOutput` and `AudioOutput` pads:

```csharp
// Example: Connect to video and audio processors/encoders
videoAudioSource.VideoOutput.Connect(videoEncoder.Input);
videoAudioSource.AudioOutput.Connect(audioEncoder.Input);
```

## Working with Decklink Video + Audio Sink Block

The `DecklinkVideoAudioSinkBlock` simplifies sending synchronized video and audio streams to a single Decklink device.

### Device Enumeration and Configuration

Similar to the combined source, device selection is managed via `DecklinkVideoSinkSettings` and `DecklinkAudioSinkSettings`. Enumerate devices using `DecklinkVideoSinkBlock.GetDevicesAsync()` and `DecklinkAudioSinkBlock.GetDevicesAsync()`.

```csharp
var videoSinkDeviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();
var audioSinkDeviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoSinkDeviceInfo.DeviceNumber); // Example match

DecklinkVideoSinkSettings videoSinkSettings = null;
if (videoSinkDeviceInfo != null)
{
    // Mode is required - specify the output video resolution and frame rate
    videoSinkSettings = new DecklinkVideoSinkSettings(videoSinkDeviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_8bit
    };
}

DecklinkAudioSinkSettings audioSinkSettings = null;
if (audioSinkDeviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(audioSinkDeviceInfo);
}

// Create the block
if (videoSinkSettings != null && audioSinkSettings != null)
{
    var decklinkVideoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
    
    // Connect inputs
    // videoEncoder.Output.Connect(decklinkVideoAudioSink.VideoInput);
    // audioEncoder.Output.Connect(decklinkVideoAudioSink.AudioInput);
}
```

### Block Creation and Configuration

Instantiate `DecklinkVideoAudioSinkBlock` with configured `DecklinkVideoSinkSettings` and `DecklinkAudioSinkSettings`.

```csharp
// Assuming videoSinkSettings and audioSinkSettings are configured
var videoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
```

### Connecting to the Pipeline

The block provides separate `VideoInput` and `AudioInput` pads:

```csharp
// Example: Connect from video and audio encoders
videoEncoder.Output.Connect(videoAudioSink.VideoInput);
audioEncoder.Output.Connect(videoAudioSink.AudioInput);
```

## Advanced Usage Examples

### Synchronized Audio/Video Capture

**Using separate source blocks:**

```csharp
// Assume videoSourceSettings and audioSourceSettings are configured for the same device/timing
var videoSource = new DecklinkVideoSourceBlock(videoSourceSettings);
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);

// Create an MP4 encoder
var mp4Settings = new MP4SinkSettings("output.mp4");
var sink = new MP4SinkBlock(mp4Settings);

// Create video encoder
var videoEncoder = new H264EncoderBlock();

// Create audio encoder
var audioEncoder = new AACEncoderBlock();

// Connect video and audio sources
pipeline.Connect(videoSource.Output, videoEncoder.Input);
pipeline.Connect(audioSource.Output, audioEncoder.Input);

// Connect video encoder to sink
pipeline.Connect(videoEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connect audio encoder to sink
pipeline.Connect(audioEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start the pipeline
await pipeline.StartAsync();
```

**Using `DecklinkVideoAudioSourceBlock` for simplified synchronized capture:**
If you use `DecklinkVideoAudioSourceBlock` (as configured in its dedicated section), the source setup becomes:

```csharp
// Assuming videoSourceSettings and audioSourceSettings are configured for the same device
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);

// ... (encoders and sink setup as above) ...

// Connect video and audio from the combined source
pipeline.Connect(videoAudioSource.VideoOutput, videoEncoder.Input);
pipeline.Connect(videoAudioSource.AudioOutput, audioEncoder.Input);

// ... (connect encoders to sink and start pipeline as above) ...
```

This ensures that audio and video are sourced from the Decklink device in a synchronized manner by the SDK.

## Troubleshooting Tips

- **No Devices Found**: Ensure Blackmagic drivers/SDK are installed and up-to-date. Check if the device is recognized by Blackmagic Desktop Video Setup.
- **Format Mismatch**: Verify the device supports your selected video/audio mode, format, and connection type. For sources with `Mode = DecklinkMode.Unknown` (auto-detect), ensure a stable signal is present.
- **Performance Issues**: Check system resources (CPU, RAM, disk I/O). Consider lowering resolution/framerate if issues persist.
- **Signal Detection**: For input devices, check cable connections and ensure the source device is outputting a valid signal.
- **"Unable to build ...Block" errors**: Double-check that all settings are valid for the selected device and mode. Ensure the correct `DeviceNumber` is used if multiple Decklink cards are present.

## Sample Applications

For complete working examples, refer to these sample applications:

- [Decklink Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Decklink%20Demo)

## Conclusion

The Blackmagic Decklink blocks in the VisioForge Media Blocks SDK provide a powerful and flexible way to integrate professional video and audio hardware into your .NET applications. By leveraging the specific source and sink blocks, including the combined audio/video blocks, you can efficiently implement complex capture and playback workflows. Always refer to the specific settings classes for detailed configuration options.
