---
title: NDI Video and Audio Streaming over IP Network in C# .NET
description: Stream video and audio to NDI from cameras, files, and capture devices in C# .NET. Setup guide with SDK examples, audio resampling, and troubleshooting.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Editing
  - Webcam
  - IP Camera
  - NDI Source
  - NDI
  - MP4
  - C#
primary_api_classes:
  - NDISinkBlock
  - AudioResamplerBlock
  - NDIOutput
  - MediaBlockPadMediaType
  - MediaBlocksPipeline

---

# Network Device Interface (NDI) Streaming Integration

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## What is NDI?

Network Device Interface (NDI) is an industry standard for live video production over IP networks. It enables high-quality, low-latency video and audio streaming over standard Ethernet — replacing expensive SDI cabling with software-based workflows. Common use cases include:

- Live broadcasting and streaming
- Professional video conferencing
- Multi-camera production setups
- Remote production workflows
- Playout server applications

The VisioForge SDK provides full NDI output support across Windows, macOS, and Linux, letting you stream from cameras, files, or any video source to NDI receivers on your network.

## Installation Requirements

To use NDI streaming, install one of the following official NDI packages:

1. **[NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/)** - Recommended for developers
2. **[NDI Tools](https://ndi.video/tools/)** - Suitable for testing

These provide the runtime components that enable NDI communication. You can verify NDI availability in code:

```csharp
bool ndiAvailable = NDISinkBlock.IsAvailable();
```

## Cross-Platform NDI Output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### NDIOutput Class

The `NDIOutput` class provides NDI output for VideoCaptureCoreX and VideoEditCoreX engines:

```csharp
public class NDIOutput : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

#### Configuration

| Property | Type | Description |
|----------|------|-------------|
| `Sink` | `NDISinkSettings` | NDI output configuration (stream name, compression, network settings) |
| `CustomVideoProcessor` | `MediaBlock` | Optional custom video processing before NDI transmission |
| `CustomAudioProcessor` | `MediaBlock` | Optional custom audio processing before NDI transmission |

#### Constructors

```csharp
// Create with stream name
var output = new NDIOutput("My Stream");

// Create with pre-configured settings
var output = new NDIOutput(new NDISinkSettings("My Stream"));
```

## Implementation Examples

### Media Blocks SDK

```cs
// Create an NDI output block with a descriptive stream name
var ndiSink = new NDISinkBlock("VisioForge Production Stream");

// Connect video source to the NDI output
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connect audio source to the NDI output
pipeline.Connect(audioSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

### Video Capture SDK

```cs
// Initialize NDI output with a network-friendly stream name
var ndiOutput = new NDIOutput("VisioForge_Studio_Output");

// Add the configured NDI output to the video capture pipeline
core.Outputs_Add(ndiOutput); // core represents the VideoCaptureCoreX instance
```

## Streaming a Camera to NDI

[MediaBlocksPipeline](#){ .md-button }

The most common use case is streaming a local webcam and microphone to NDI. This example uses the Media Blocks SDK to capture from system devices and send to NDI with proper audio resampling.

### Pipeline Architecture

```text
SystemVideoSourceBlock → NDISinkBlock (video input)
SystemAudioSourceBlock → AudioResamplerBlock (48kHz, F32LE, stereo) → NDISinkBlock (audio input)
```

### Code Example

```cs
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.AudioProcessing;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioEncoders;

// Initialize SDK once at startup
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Enumerate available devices
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Set up video source (first available camera)
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Set up audio source (first available microphone)
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Create NDI output
var ndiSink = new NDISinkBlock("My Camera Stream");

// Connect video directly to NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Resample audio to 48kHz F32LE stereo (required by NDI)
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(audioSource.Output, audioResampler.Input);
pipeline.Connect(audioResampler.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

## Windows-Specific NDI Implementation

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

For Windows-specific implementations, the SDK provides additional configuration options through the VideoCaptureCore or VideoEditCore components.

### Step-by-Step Implementation Guide

#### 1. Enable Network Streaming

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

#### 2. Configure Audio Streaming

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

#### 3. Select NDI Protocol

```csharp
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.NDI;
```

#### 4. Create and Configure NDI Output

```cs
var streamName = "VisioForge NDI Streamer";
var ndiOutput = new NDIOutput(streamName);
```

#### 5. Assign the Output

```cs
VideoCapture1.Network_Streaming_Output = ndiOutput;
```

#### 6. Generate the NDI URL (Optional)

```cs
string ndiUrl = $"ndi://{System.Net.Dns.GetHostName()}/{streamName}";
Debug.WriteLine(ndiUrl);
```

## File Playback to NDI Output

The Media Blocks SDK can stream local media files (MP4, MKV, AVI, etc.) directly to NDI without any local rendering — ideal for playout server applications.

### Recommended Pipeline

```text
UniversalSourceBlock (file)
  VideoOutput → NDISinkBlock (video input)
  AudioOutput → AudioResamplerBlock (48kHz, F32LE, stereo) → NDISinkBlock (audio input)
```

### Code Example

```cs
var pipeline = new MediaBlocksPipeline();

// File source with automatic format detection
var fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(new Uri("file:///path/to/video.mp4")));

// NDI output
var ndiSink = new NDISinkBlock("My NDI Stream");

// Connect video directly to NDI
pipeline.Connect(fileSource.VideoOutput,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Resample audio to 48kHz F32LE stereo for NDI compatibility
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(fileSource.AudioOutput, audioResampler.Input);
pipeline.Connect(audioResampler.Output,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Optional: enable loop playback
pipeline.Loop = true;

await pipeline.StartAsync();
```

## Audio Format Requirements

NDI requires **48kHz, 32-bit float (F32LE), interleaved** audio. When streaming from sources that may contain audio at other sample rates (e.g., 44.1kHz AAC in MP4 files, or varying microphone rates), always include an `AudioResamplerBlock` to convert to 48kHz. Without resampling, audio may stutter, glitch, or lose synchronization with video.

```cs
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
```

## NDI Source Playback (Receiving)

To receive and play an NDI stream locally with proper audio/video synchronization, use `IsSync = true` (default) on both video and audio renderers. This ensures GStreamer's pipeline clock synchronizes both streams correctly.

```cs
var ndiSource = new NDISourceBlock(ndiSettings);

// Both renderers use IsSync = true (default) for proper A/V sync
var videoRenderer = new VideoRendererBlock(pipeline, videoView);
var audioRenderer = new AudioRendererBlock(audioOutputSettings);

pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);
```

For complete NDI source enumeration, connection, and capture details, see the [NDI Video Source Reference](../../videocapture/video-sources/ip-cameras/ndi.md).

## Troubleshooting

### Audio Stuttering or Glitching on NDI Output

**Symptom:** Audio is not smooth when streaming from file sources — stutters, glitches, or lip-sync issues.

**Cause:** The source file contains audio at a sample rate other than 48kHz (e.g., 44.1kHz AAC in MP4 files). NDI expects 48kHz audio.

**Solution:** Insert an `AudioResamplerBlock` configured for 48kHz F32LE stereo between the file source and the NDI sink, as shown in the file playback example above.

### Video/Audio Out of Sync on NDI Receiver

**Symptom:** Video plays ahead of or behind audio when receiving an NDI stream.

**Cause:** The video renderer's `IsSync` property is set to `false`, causing it to render frames immediately without clock synchronization.

**Solution:** Ensure both `VideoRendererBlock` and `AudioRendererBlock` have `IsSync = true` (the default value).

## Frequently Asked Questions

### How do I stream video from a camera to NDI in C#?

Use the Media Blocks SDK to create a pipeline with `SystemVideoSourceBlock` for the camera, `SystemAudioSourceBlock` for the microphone, and `NDISinkBlock` as the output. Connect audio through an `AudioResamplerBlock` set to 48kHz F32LE stereo, which NDI requires. See the [Streaming a Camera to NDI](#streaming-a-camera-to-ndi) section for complete code.

### What audio format does NDI require?

NDI requires 48kHz, 32-bit float (F32LE), interleaved stereo audio. Always include an `AudioResamplerBlock(new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2))` in your pipeline between the audio source and the NDI sink. Without proper resampling, you may experience audio stuttering, glitches, or A/V sync issues.

### Can I stream a video file to NDI for playout?

Yes. Use `UniversalSourceBlock` to read the file, connect video directly to `NDISinkBlock`, and route audio through `AudioResamplerBlock` for 48kHz conversion. Enable `pipeline.Loop = true` for continuous playout. This pattern is ideal for broadcast playout servers with zero local rendering overhead.

### What are the system requirements for NDI streaming in .NET?

You need the [NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/) or [NDI Tools](https://ndi.video/tools/) installed for runtime NDI support. The VisioForge SDK supports Windows, macOS, and Linux. Check NDI availability at runtime with `NDISinkBlock.IsAvailable()`. Network bandwidth requirements depend on resolution and framerate — a typical HD NDI stream uses approximately 100-150 Mbps.

### How do I check if NDI is available on the system?

Call `NDISinkBlock.IsAvailable()` before creating NDI pipeline components. This static method checks whether the NDI runtime libraries are installed and accessible. If it returns `false`, prompt the user to install the NDI SDK or NDI Tools package.

## See Also

- [NDI Video Source Reference](../../videocapture/video-sources/ip-cameras/ndi.md) — receiving and capturing NDI sources in .NET
- [RTSP Stream Viewer and IP Camera Player](../../mediablocks/Guides/rtsp-player-csharp.md) — similar IP streaming guide for RTSP cameras
- [Deployment Guide](../../deployment-x/index.md) — platform-specific runtime packages for Windows, macOS, Linux
- [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — NDI streamer and source demos
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — product page and downloads
