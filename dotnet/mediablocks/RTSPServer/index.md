---
title: RTSP Server Streaming in C# .NET — Developer Guide
description: Create RTSP streaming servers for low-latency audio and video delivery with H.264/H.265 codec support using VisioForge Media Blocks SDK.
sidebar_label: RTSP Server
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - RTSPServerBlock
  - RTSPServerSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - SystemVideoSourceBlock

---

# RTSP Server Block - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    The `RTSPServerBlock` runs on **Windows, macOS, and Linux** via GStreamer (requires the `gst-rtsp-server` plugin). See the [platform support matrix](../../platform-matrix.md) for codec and hardware-acceleration details, and the [Linux deployment guide](../../deployment-x/Ubuntu.md) for Ubuntu / NVIDIA Jetson / Raspberry Pi setup.

The RTSP Server block creates an RTSP (Real-Time Streaming Protocol) server endpoint for streaming audio/video content over networks. Clients can connect to receive live or recorded media streams with low latency.

## Overview

The RTSPServerBlock provides a complete RTSP streaming server implementation with the following capabilities:

- **Multiple Client Support**: Handles simultaneous connections from multiple RTSP clients
- **Standard Compliance**: Compatible with VLC, FFmpeg, GStreamer, and IP camera viewers
- **Video Codecs**: H.264, H.265, and other compressed formats
- **Audio Codecs**: AAC, MP3, Opus, and other formats
- **Transport Protocols**: RTP/RTCP for reliable media delivery
- **Authentication**: Optional RTSP authentication support
- **Configurable**: Custom port binding and mount points
- **Low Latency**: Optimized for real-time streaming applications

## Block Info

Name: RTSPServerBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | compressed video (H.264, H.265) | 1 |
| Input audio | compressed audio (AAC, MP3, etc.) | 1 |

## The Sample Pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->H264EncoderBlock;
    SystemAudioSourceBlock-->AACEncoderBlock;
    H264EncoderBlock-->RTSPServerBlock;
    AACEncoderBlock-->RTSPServerBlock;
```

## Settings

The RTSPServerBlock is configured using `RTSPServerSettings`:

### RTSPServerSettings Properties

- `Port` (`int`): The TCP port for the RTSP server. Default `8554`.
- `Point` (`string`): The URL path under which the stream is served (for example `/live`). Default `/live`.
- `Username` (`string`): Optional username for basic RTSP authentication.
- `Password` (`string`): Optional password for basic RTSP authentication.
- `Address` (`string`): Address the server binds to. Default `127.0.0.1`; set to `0.0.0.0` to bind all interfaces.
- `Name` (`string`): Server display name announced to clients. Default `"VisioForge RTSP Server"`.
- `Description` (`string`): Human-readable description. Default `"VisioForge RTSP Server"`.
- `Latency` (`TimeSpan`): Buffering latency applied by the server. Default `250 ms`.
- `URL` (`string`, read-only): Computed `rtsp://{Address}:{Port}{Point}` URL — use this after constructing the settings to log the client-facing URL.
- `VideoEncoder` (`IVideoEncoder`): Video encoder settings. Pass `null` to disable the video stream.
- `AudioEncoder` (`IAudioEncoder`): Audio encoder settings. Pass `null` to disable the audio stream.

`RTSPServerSettings` has **no parameterless constructor**. Use one of:

- `new RTSPServerSettings(IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)`
- `new RTSPServerSettings(Uri uri, IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)` — parses host / port / point from the URI.

## Sample Code

### Basic RTSP Server

```csharp
var pipeline = new MediaBlocksPipeline();

// Create video source
var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
var videoFormat = videoDevice.VideoFormats[0];
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
{
    Format = videoFormat.ToFormat()
};
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Create audio source
var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
var audioFormat = audioDevice.Formats[0];
var audioSettings = audioDevice.CreateSourceSettings(audioFormat.ToFormat());
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Create video encoder (concrete settings class — use NVENC/QSV/AMF where available, OpenH264 as the portable fallback)
var h264Settings = new OpenH264EncoderSettings
{
    Bitrate = 2000 // kbps
};
var h264Encoder = new H264EncoderBlock(h264Settings);
pipeline.Connect(videoSource.Output, h264Encoder.Input);

// Create audio encoder (cross-platform AAC)
var aacSettings = new VOAACEncoderSettings
{
    Bitrate = 128 // kbps
};
var aacEncoder = new AACEncoderBlock(aacSettings);
pipeline.Connect(audioSource.Output, aacEncoder.Input);

// Create RTSP server (settings ctor takes videoEncoder + audioEncoder).
// Since we pre-encode above, pass null for both here — RTSPServerBlock just forwards the encoded
// streams. Pass concrete encoders only when you want the server to encode internally.
var rtspSettings = new RTSPServerSettings(videoEncoder: null, audioEncoder: null)
{
    Port = 8554,
    Point = "/live",
    Address = "0.0.0.0"   // listen on all interfaces; defaults to 127.0.0.1
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(h264Encoder.Output, rtspServer.VideoInput);
pipeline.Connect(aacEncoder.Output, rtspServer.AudioInput);

// Start streaming
await pipeline.StartAsync();

// rtspSettings.URL returns the client-facing URL:
Console.WriteLine($"RTSP server started at {rtspSettings.URL}");
Console.WriteLine($"Connect with: vlc {rtspSettings.URL}");
```

### Letting the server encode internally

If you'd rather feed raw camera frames straight into the RTSP server and let the server do its own H.264 encoding, construct `RTSPServerSettings` with concrete encoder settings and skip the standalone `H264EncoderBlock` / `AACEncoderBlock`:

```csharp
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: null)
{
    Port = 8554,
    Point = "/live",
    Address = "0.0.0.0"
};

var rtspServer = new RTSPServerBlock(rtspSettings);

// Connect raw (unencoded) sources — the server will encode.
pipeline.Connect(videoSource.Output, rtspServer.VideoInput);

await pipeline.StartAsync();
Console.WriteLine(rtspSettings.URL);
```

### RTSP Server with Authentication

```csharp
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/secure",
    Username = "admin",
    Password = "password123"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Configure encoders and connect as above
// ...

await pipeline.StartAsync();

// Clients must authenticate: rtsp://admin:password123@localhost:8554/secure
Console.WriteLine($"Secure RTSP server started at {rtspSettings.URL}");
```

### File Streaming over RTSP

```csharp
var pipeline = new MediaBlocksPipeline();

// Use file as source
var fileSettings = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
var fileSource = new UniversalSourceBlock(fileSettings);

// Create RTSP server — let it encode both streams internally
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/vod"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Connect raw video and audio from file
pipeline.Connect(fileSource.VideoOutput, rtspServer.VideoInput);
pipeline.Connect(fileSource.AudioOutput, rtspServer.AudioInput);

await pipeline.StartAsync();

// Stream file content via RTSP
Console.WriteLine($"Video-on-Demand RTSP server started at {rtspSettings.URL}");
```

## Client Connection

Clients can connect to the RTSP server using the URL format:

```
rtsp://hostname:port/mountpoint
```

Examples:
- `rtsp://localhost:8554/live`
- `rtsp://192.168.1.100:8554/stream1`
- `rtsp://username:password@server.com:8554/secure`

### VLC Player

```bash
vlc rtsp://localhost:8554/live
```

### FFmpeg

```bash
ffplay rtsp://localhost:8554/live
ffmpeg -i rtsp://localhost:8554/live -c copy output.mp4
```

### GStreamer

```bash
gst-launch-1.0 rtspsrc location=rtsp://localhost:8554/live ! decodebin ! autovideosink
```

## Use Cases

- **Live Streaming**: Broadcast live camera feeds over networks
- **Security Systems**: Stream surveillance cameras to monitoring stations
- **Video Distribution**: Distribute media content to multiple clients
- **Remote Monitoring**: Enable remote viewing of industrial processes
- **Broadcasting**: Create low-latency streaming solutions
- **Video Conferencing**: Stream participant feeds to viewers
- **IoT Cameras**: Provide RTSP endpoint for smart camera devices

## Performance Considerations

- **Encoding**: Video/audio must be encoded before streaming
- **Bandwidth**: Monitor network bandwidth for multiple clients
- **Latency**: Optimize encoder settings for low-latency requirements
- **Clients**: Each client consumes server resources and bandwidth
- **Port**: Ensure firewall allows traffic on the configured port
- **Multicast**: Use for efficient one-to-many streaming on local networks

## Remarks

- Video and audio streams should be compressed/encoded before feeding to RTSP server
- The server supports H.264/H.265 for video and AAC/MP3/Opus for audio
- Port 8554 is the standard RTSP port but can be changed
- Mount point defines the URL path for accessing the stream
- Server continues streaming as long as the pipeline is running
- Multiple RTSPServerBlock instances can run on different ports simultaneously
- Use authentication for secure streaming environments

## Platforms

Windows, macOS, Linux.

Note: Requires GStreamer with RTSP server support (gst-rtsp-server plugin).

## Sample Applications

- [RTSP Webcam Server](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Webcam%20Server)

## Related Blocks

- [H264EncoderBlock](../VideoEncoders/index.md#h264-encoder) - H.264 video encoding
- [H265EncoderBlock](../VideoEncoders/index.md#hevch265-encoder) - H.265 video encoding
- [AACEncoderBlock](../AudioEncoders/index.md#aac-encoder) - AAC audio encoding
- [SystemVideoSourceBlock](../Sources/index.md#system-video-source) - Camera capture
- [SystemAudioSourceBlock](../Sources/index.md#system-audio-source) - Audio capture

## Related Guides

- [RTSP protocol deep-dive](../../general/network-streaming/rtsp.md) — how RTSP works under the hood
- [RTSP camera source configuration](../../videocapture/video-sources/ip-cameras/rtsp.md) — consume RTSP streams in your own applications
- [Media Blocks RTSP player](../Guides/rtsp-player-csharp.md) — a client pipeline for RTSP streams (pair with this server)
- [Save RTSP stream without re-encoding](../Guides/rtsp-save-original-stream.md) — archive streams from an RTSP source to disk
- [RTSP reconnection and fallback switch](../../general/network-sources/reconnection-and-fallback.md) — handle upstream source drops with reconnect events and `FallbackSwitch` when you feed this server from an RTSP camera
