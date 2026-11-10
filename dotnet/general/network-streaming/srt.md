---
title: SRT Streaming in C# .NET - Send and Receive Video over IP
description: Stream and receive video over SRT protocol in C# .NET with caller/listener modes, AES encryption, and MPEG-TS multiplexing. Includes SDK code examples.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
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

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## What is SRT?

SRT (Secure Reliable Transport) is a streaming protocol designed for low-latency, high-quality video delivery across unreliable networks. It provides built-in error recovery, AES encryption, and firewall traversal — making it ideal for:

- Live broadcasting over the internet
- Contribution feeds between production facilities
- Remote camera backhaul over cellular or satellite links
- Secure point-to-point video transport
- Cloud-based video ingest and distribution

The VisioForge .NET SDKs support both sending and receiving SRT streams on Windows, macOS, Mac Catalyst, Linux, iOS and Android. SRT streams use MPEG-TS multiplexing to carry video and audio together.

You can check SRT availability at runtime:

```csharp
bool srtAvailable = SRTSinkBlock.IsAvailable(); // for sending
bool srtSourceAvailable = SRTSourceBlock.IsAvailable(); // for receiving
```

On Windows, macOS and Mac Catalyst, the SRT plugin ships in the `Adds` runtime package, not in the main one: `VisioForge.CrossPlatform.Core.Windows.Adds.x64` (or its x86 and arm64 variants), `VisioForge.CrossPlatform.Core.macOS.Adds` or `VisioForge.CrossPlatform.Core.macCatalyst.Adds`. Add it next to the main runtime package, or `IsAvailable()` returns `false`. The iOS and Android runtime packages include SRT. On Linux the SDK uses the system GStreamer; on Ubuntu and Debian, SRT is part of `gstreamer1.0-plugins-bad`.

## SRT Connection Modes

[VideoCaptureCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

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
// CreateAsync reads the stream's tracks before it returns, so the sender must be running. Uri is System.Uri, not string.
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
sourceSettings.Mode = SRTConnectionMode.Caller;
```

`CreateAsync` connects with the URI alone, so whatever that first connection needs goes in the URI query: a receiver in listener mode uses `srt://0.0.0.0:8888?mode=listener`, and a server that requires a stream ID gets `?streamid=...`.

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

In Video Capture SDK .NET, `SRTOutput` requires the Premium edition, and an SRT stream used as `Video_Source` requires the Professional edition.

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
    Passphrase = "my-secret-passphrase",  // 10 to 79 bytes in UTF-8
    PbKeyLen = SRTKeyLength.Length32       // 256-bit AES
};
```

### Receiver (Encrypted)

`SRTSourceSettings.CreateAsync` opens the stream with the URI alone to read its tracks before it returns, so when you use it for an encrypted stream, put the passphrase in the URI as `passphrase` and `pbkeylen` query parameters. A passphrase is 10 to 79 bytes in UTF-8, which is 10 to 79 characters for Latin letters and digits; in a URI, use letters, digits, `-`, `_`, `.` and `~`:

```csharp
// CreateAsync reads the stream with the URI alone, so the passphrase and key length go in the query
var sourceSettings = await SRTSourceSettings.CreateAsync(
    new Uri("srt://192.168.1.100:8888?passphrase=my-secret-passphrase&pbkeylen=32"));
```

With Media Blocks SDK .NET you can skip `CreateAsync` and set the passphrase as a property instead. It then stays out of the URI, so it can contain any characters. `SRTSourceBlock` does not know the tracks in advance, so it creates both a video and an audio output:

```csharp
var sourceSettings = new SRTSourceSettings
{
    Uri = new Uri("srt://192.168.1.100:8888"),
    Passphrase = "my secret passphrase!",
    PbKeyLen = SRTKeyLength.Length32
};
var srtSource = new SRTSourceBlock(sourceSettings);
```

For a video-only stream, pass `renderAudio: false`.

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

- **OpenH264** — Cross-platform H.264 encoder, the default on Linux and on Windows when no GPU encoder is available

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

By default the SDK uses AVENC AAC on every platform.

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

### Stopping Hangs After the Other Side Disconnects

**Symptom:** `StopAsync` does not return after the remote peer has become unreachable.

**Solutions:**

- On Windows, macOS, Mac Catalyst, iOS and Android, use the VisioForge packages (on Windows, macOS and Mac Catalyst, the `Adds` package): they ship a patched SRT plugin that re-reads its `auto-reconnect` property before each reconnect attempt. The SDK clears that property when you stop, so stopping returns even when the peer is gone
- On Linux the SDK uses the distribution's SRT plugin, which does not carry this patch. If a stop hangs there, set `AutoReconnect = false` on the settings: a lost peer then ends the stream with an error instead of a reconnect loop

## Frequently Asked Questions

### What is the difference between SRT caller and listener mode?

The **listener** binds to a port and waits for incoming connections — it acts as the server. The **caller** initiates the connection to a listener's address and port — it acts as the client. For firewall traversal where both sides are behind NAT, use **rendezvous** mode where both endpoints connect simultaneously.

### How do I encrypt an SRT stream?

On the sender, set `Passphrase` (10 to 79 bytes in UTF-8) and `PbKeyLen` on `SRTSinkSettings`. On the receiver, pass the same values in the URI given to `SRTSourceSettings.CreateAsync` (`srt://host:8888?passphrase=...&pbkeylen=32`), because `CreateAsync` opens the stream with the URI alone; in a URI, use letters, digits, `-`, `_`, `.` and `~`. With Media Blocks SDK .NET you can instead set `Passphrase` and `PbKeyLen` on a new `SRTSourceSettings` and skip `CreateAsync`. Both sides must use identical values. Available key lengths are 128-bit (`Length16`), 192-bit (`Length24`), and 256-bit (`Length32`). See the [Encryption](#encryption) section for code examples.

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
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) — product page and downloads
