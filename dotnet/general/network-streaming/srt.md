---
title: SRT Streaming in C# .NET - Send and Receive Video over IP
description: Stream and receive video over SRT protocol in C# .NET with caller/listener modes, AES encryption, and MPEG-TS multiplexing. Includes SDK code examples.
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
  - Webcam
  - IP Camera
  - NDI
  - SRT
  - TS
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - SRTSinkSettings
  - SRTMPEGTSSinkBlock
  - MediaBlocksPipeline
  - SRTSourceSettings
  - SRTSourceBlock

---

# SRT Streaming Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## What is SRT?

SRT (Secure Reliable Transport) is a streaming protocol designed for low-latency, high-quality video delivery across unreliable networks. It provides built-in error recovery, AES encryption, and firewall traversal — making it ideal for:

- Live broadcasting over the internet
- Contribution feeds between production facilities
- Remote camera backhaul over cellular or satellite links
- Secure point-to-point video transport
- Cloud-based video ingest and distribution

The VisioForge .NET SDKs support both sending and receiving SRT streams across Windows, macOS, and Linux. SRT streams use MPEG-TS multiplexing to carry video and audio together.

You can check SRT availability at runtime:

```csharp
bool srtAvailable = SRTSinkBlock.IsAvailable(); // for sending
bool srtSourceAvailable = SRTSourceBlock.IsAvailable(); // for receiving
```

## SRT Connection Modes

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

SRT supports three connection modes via the `SRTConnectionMode` enum:

| Mode | Description | Use Case |
| --- | --- | --- |
| **Caller** | Connects to a remote listener | Client connecting to a server |
| **Listener** | Waits for incoming connections on a port | Server accepting connections |
| **Rendezvous** | Both sides connect simultaneously | Peer-to-peer, firewall traversal |

### Listener (Server) Mode

The listener waits for incoming SRT connections on a specified port:

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener
};
```

### Caller (Client) Mode

The caller connects to a remote SRT listener:

```csharp
// SRTSourceSettings has a private ctor — use the CreateAsync factory. Uri is System.Uri, not string.
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
sourceSettings.Mode = SRTConnectionMode.Caller;
```

### Rendezvous Mode

Both endpoints connect simultaneously — useful when both sides are behind firewalls:

```csharp
var settings = new SRTSinkSettings
{
    Uri = "srt://remote-host:8888",
    Mode = SRTConnectionMode.Rendezvous,
    LocalPort = 8888
};
```

## Basic SRT Output

### Video Capture SDK

```csharp
// Initialize SRT output with destination URL
var srtOutput = new SRTOutput("srt://streaming-server:1234");

// Add the configured SRT output to your capture engine
videoCapture.Outputs_Add(srtOutput, true);  // videoCapture is a VideoCaptureCoreX instance
```

### Media Blocks SDK

The `SRTMPEGTSSinkBlock` multiplexes video and audio into an MPEG-TS container and sends over SRT:

```csharp
// Create an SRT MPEG-TS sink in listener mode
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings { Uri = "srt://:8888" });

// Connect video encoder output to the SRT sink
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connect audio encoder output to the SRT sink
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Streaming a Camera to SRT

[MediaBlocksPipeline](#){ .md-button }

This complete example captures from a webcam and microphone, encodes to H.264/AAC, and streams over SRT:

### Pipeline Architecture

```text
SystemVideoSourceBlock → H264EncoderBlock → SRTMPEGTSSinkBlock (video input)
SystemAudioSourceBlock → AACEncoderBlock  → SRTMPEGTSSinkBlock (audio input)
```

### Code Example

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

// Initialize SDK once at startup
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Enumerate devices
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Video source (first camera)
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Audio source (first microphone)
var audioSource = new SystemAudioSourceBlock(
    new AudioCaptureDeviceSourceSettings(audioDevices[0]));

// Video encoder — H.264 with hardware fallback
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());

// Audio encoder — AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings());

// SRT output in listener mode on port 8888
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Latency = TimeSpan.FromMilliseconds(125)
});

// Build pipeline: camera → encoder → SRT
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

Receivers can connect using `ffplay srt://your-ip:8888` or any SRT-compatible player.

## Receiving an SRT Stream

[MediaBlocksPipeline](#){ .md-button }

Use `SRTSourceBlock` to receive and play an SRT stream with automatic decoding:

```csharp
var pipeline = new MediaBlocksPipeline();

// Connect to an SRT sender (caller mode by default)
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
var srtSource = new SRTSourceBlock(sourceSettings);

// Video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(srtSource.VideoOutput, videoRenderer.Input);

// Audio renderer
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(srtSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

For passthrough recording without decoding (e.g., saving the raw MPEG-TS stream), use `SRTRAWSourceBlock` instead.

## Encryption

SRT supports AES encryption with 128, 192, or 256-bit keys. Both sender and receiver must use the same passphrase and key length.

### Sender (Encrypted)

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Passphrase = "my-secret-passphrase",  // minimum 10 characters
    PbKeyLen = SRTKeyLength.Length32       // 256-bit AES
};
```

### Receiver (Encrypted)

```csharp
// Use the async factory — SRTSourceSettings has a private ctor; Uri is System.Uri, not string.
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
sourceSettings.Mode = SRTConnectionMode.Caller;
sourceSettings.Passphrase = "my-secret-passphrase";
sourceSettings.PbKeyLen = SRTKeyLength.Length32;
```

Available key lengths: `SRTKeyLength.NoKey` (disabled), `Length16` (128-bit), `Length24` (192-bit), `Length32` (256-bit).

## Latency Configuration

The `Latency` property controls the SRT receiver buffer size (default: 125ms). Lower values reduce delay but increase sensitivity to network jitter:

```csharp
// Low-latency for local network
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(50)
};

// Higher latency for unreliable networks (internet streaming)
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(500)
};
```

| Network | Recommended Latency | Notes |
| --- | --- | --- |
| Local LAN | 20–80ms | Minimal jitter |
| Reliable internet | 125ms (default) | Good balance |
| Unreliable/long-distance | 250–1000ms | Prevents drops |

## Video Encoding Options

### Software Encoders

- **OpenH264** — Default cross-platform H.264 encoder

### Hardware-Accelerated Encoders

- **NVIDIA NVENC** (H.264/HEVC) — GPU-accelerated encoding on NVIDIA cards
- **Intel Quick Sync** (H.264/HEVC) — Intel integrated GPU acceleration
- **AMD AMF** (H.264/HEVC) — AMD GPU acceleration
- **Microsoft Media Foundation HEVC** — Windows-only hardware encoder

### Encoder Selection with Fallback

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    srtOutput.Video = new NVENCH264EncoderSettings();
}
else
{
    srtOutput.Video = new OpenH264EncoderSettings();
}
```

## Audio Encoding

SRT streams typically use AAC audio. The SDK provides multiple encoders:

- **VO-AAC** — Cross-platform, consistent performance
- **AVENC AAC** — FFmpeg-based with extensive options
- **MF AAC** — Windows-only, Microsoft Media Foundation

The SDK auto-selects the best available encoder per platform (MF AAC on Windows, VO AAC elsewhere).

## Troubleshooting

### Unable to Establish SRT Connection

**Symptom:** Connection times out or is refused.

**Solutions:**

- Verify the SRT URL format: `srt://host:port` for caller, `srt://:port` for listener
- Ensure the port is open in firewalls on both sides
- Confirm both sides use matching connection modes (one caller, one listener)
- Check that passphrases match if encryption is enabled

### High CPU Usage or Dropped Frames

**Symptom:** Performance degrades during streaming.

**Solutions:**

- Switch to hardware-accelerated encoders (NVENC, QSV, AMF)
- Reduce resolution or bitrate
- Increase the `Latency` value to give more buffer room

### Encoder Fails to Initialize

**Symptom:** Exception when starting the pipeline.

**Solutions:**

- Use `IsAvailable()` to check encoder support before creating it
- Verify GPU drivers are up to date for hardware encoders
- Fall back to OpenH264 as a universal software encoder

## Frequently Asked Questions

### What is the difference between SRT caller and listener mode?

The **listener** binds to a port and waits for incoming connections — it acts as the server. The **caller** initiates the connection to a listener's address and port — it acts as the client. For firewall traversal where both sides are behind NAT, use **rendezvous** mode where both endpoints connect simultaneously.

### How do I encrypt an SRT stream?

Set the `Passphrase` property (minimum 10 characters) and `PbKeyLen` on both `SRTSinkSettings` and `SRTSourceSettings`. Both sender and receiver must use identical values. Available key lengths are 128-bit (`Length16`), 192-bit (`Length24`), and 256-bit (`Length32`). See the [Encryption](#encryption) section for code examples.

### How do I receive and play an SRT stream in C#?

Create `SRTSourceSettings` with the sender's URL, then pass it to `SRTSourceBlock`. Connect `VideoOutput` to a `VideoRendererBlock` and `AudioOutput` to an `AudioRendererBlock`. The source block handles MPEG-TS demuxing and decoding automatically. See the [Receiving an SRT Stream](#receiving-an-srt-stream) section for the complete example.

### What video codecs does SRT support?

SRT itself is codec-agnostic — it transports any data over the wire. When using `SRTMPEGTSSinkBlock`, the stream is multiplexed as MPEG-TS, which supports H.264, HEVC (H.265), MPEG-2, and AV1 video codecs. H.264 is the most widely compatible choice for SRT streaming.

### How do I reduce SRT streaming latency?

Lower the `Latency` property on both sender and receiver settings (default is 125ms). For local networks, values as low as 20–50ms work well. For internet streaming, keep at least 125ms to handle jitter. Also ensure your encoder is configured for low-latency mode and that you're using hardware acceleration to minimize encoding delay.

## See Also

- [NDI Streaming Integration](ndi.md) — NDI video-over-IP streaming
- [RTSP Stream Viewer and IP Camera Player](../../mediablocks/Guides/rtsp-player-csharp.md) — RTSP camera streaming guide
- [MPEG-TS Output Format](../output-formats/mpegts.md) — MPEG-TS container configuration
- [Deployment Guide](../../deployment-x/index.md) — platform-specific runtime packages
- [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — SRT source demo
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — product page and downloads
