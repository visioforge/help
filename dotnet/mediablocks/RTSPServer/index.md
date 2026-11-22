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
  - UniversalSourceBlock
video:
  id: YP5rS11zoNc
  upload_date: 2026-11-22T09:36:39Z
  duration: PT46S

---

# RTSP Server Block - VisioForge Media Blocks SDK .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    The `RTSPServerBlock` runs on **Windows, macOS, and Linux** via GStreamer (requires the `gst-rtsp-server` plugin). See the [platform support matrix](../../platform-matrix.md) for codec and hardware-acceleration details, and the [Linux deployment guide](../../deployment-x/Ubuntu.md) for Ubuntu / NVIDIA Jetson / Raspberry Pi setup.

The RTSP Server block creates an RTSP (Real-Time Streaming Protocol) server endpoint for streaming audio/video content over networks. Clients can connect to receive live or recorded media streams with low latency.

## Video Tutorial: Stream an MP4 File as an RTSP Server in a Few Lines of C\#

A console app that serves a video file as an RTSP stream: `UniversalSourceBlock` reads the file, and `RTSPServerBlock` encodes it to H.264, plus AAC when the file has audio, and serves it at `rtsp://127.0.0.1:8554/live`.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YP5rS11zoNc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Its code: [rtsp-server-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/rtsp-server-short-console) on GitHub, and [File Streaming over RTSP](#file-streaming-over-rtsp) below.

## Overview

The RTSPServerBlock provides a complete RTSP streaming server implementation with the following capabilities:

- **Multiple Client Support**: Handles simultaneous connections from multiple RTSP clients, which share one encoded stream
- **Standard Compliance**: Compatible with VLC, FFmpeg, GStreamer, and IP camera viewers
- **Built-in Encoding**: The block takes uncompressed video and audio and encodes them itself
- **Video Codecs**: H.264, H.265 (HEVC) and VP9
- **Audio Codecs**: AAC, MP3, Opus and Vorbis
- **Transport Protocols**: RTP/RTCP for reliable media delivery
- **Authentication**: Optional RTSP authentication support
- **Configurable**: Custom port, mount point and listening address
- **Low Latency**: Optimized for real-time streaming applications

## Block Info

Name: RTSPServerBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | uncompressed video, encoded with `VideoEncoder` | 1 |
| Input audio | uncompressed audio, encoded with `AudioEncoder` | 1 |

## The Sample Pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->RTSPServerBlock;
    SystemAudioSourceBlock-->RTSPServerBlock;
```

## Settings

The RTSPServerBlock is configured using `RTSPServerSettings`:

### RTSPServerSettings Properties

- `Port` (`int`): The TCP port for the RTSP server. Default `8554`.
- `Point` (`string`): The URL path under which the stream is served (for example `/live`). Default `/live`.
- `Username` (`string`): Optional username for basic RTSP authentication.
- `Password` (`string`): Optional password for basic RTSP authentication.
- `Address` (`string`): The host name or IP address clients use to reach the server. It only sets the host part of `URL`; it does not choose the interfaces the server listens on. Default `127.0.0.1`.
- `ListenAddress` (`string`): The local IP address the server listens on. Default `0.0.0.0`, every IPv4 interface; `127.0.0.1` accepts only clients on the same machine. When you set a specific address here, set `Address` too, so `URL` names it.
- `Name` (`string`): Server display name announced to clients. Default `"VisioForge RTSP Server"`.
- `Description` (`string`): Human-readable description. Default `"VisioForge RTSP Server"`.
- `Latency` (`TimeSpan`): Buffering latency applied by the server. Default `250 ms`.
- `URL` (`string`, read-only): Computed `rtsp://{Address}:{Port}{Point}` URL — use this after constructing the settings to log the client-facing URL.
- `VideoEncoder` (`IVideoEncoder`): Video encoder settings: H.264, H.265 or VP9. Pass `null` to disable the video stream.
- `AudioEncoder` (`IAudioEncoder`): Audio encoder settings: AAC, MP3, Opus or Vorbis. Pass `null` to disable the audio stream.

`RTSPServerSettings` has **no parameterless constructor**. Use one of:

- `new RTSPServerSettings(IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)`
- `new RTSPServerSettings(Uri uri, IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)` — parses host / port / point from the URI; it does not set `ListenAddress`, so for an IPv6 host set `ListenAddress` to an IPv6 address too.

## Sample Code

### Basic RTSP Server

Connect the camera and the microphone straight to the server: it encodes them with the encoders named in its settings.

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

// Create RTSP server: it encodes the video to H.264 and the audio to AAC
var rtspSettings = new RTSPServerSettings(
    H264EncoderBlock.GetDefaultSettings(),
    AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/live"
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(videoSource.Output, rtspServer.VideoInput);
pipeline.Connect(audioSource.Output, rtspServer.AudioInput);

// Start streaming
await pipeline.StartAsync();

// rtspSettings.URL returns the client-facing URL:
Console.WriteLine($"RTSP server started at {rtspSettings.URL}");
Console.WriteLine($"Connect with: vlc {rtspSettings.URL}");
```

The server listens on every network interface by default, so other machines reach it at this machine's IP address. To keep it on this machine only, set `ListenAddress = "127.0.0.1"`.

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

// Connect the sources as above
// ...

await pipeline.StartAsync();

// Clients must authenticate: rtsp://admin:password123@localhost:8554/secure
Console.WriteLine($"Secure RTSP server started at {rtspSettings.URL}");
```

### File Streaming over RTSP

A complete console app that serves an MP4 file (or any other video file `UniversalSourceBlock` plays) at `rtsp://127.0.0.1:8554/live` until Enter is pressed. Pass the file path as the first argument. The AAC encoder is set only when the file has an audio track, so a video-only file gives a video-only stream.

<!-- vf-compile -->
```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();
await using var p = new MediaBlocksPipeline();
p.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var file = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(
        args[0]));
var aac = file.AudioOutput == null ? null
    : AACEncoderBlock.GetDefaultSettings();
var rtsp = new RTSPServerBlock(
    new RTSPServerSettings(
        H264EncoderBlock.GetDefaultSettings(),
        aac));
p.Connect(file.VideoOutput, rtsp.VideoInput);
if (aac != null)
    p.Connect(file.AudioOutput, rtsp.AudioInput);
if (!await p.StartAsync()) return;
Console.WriteLine(rtsp.Settings.URL);
Console.ReadLine();
await p.StopAsync();
```

The file plays once, in real time, from the moment the pipeline starts: a client that connects later joins it at that point. When the file ends, connected clients receive black frames until the app stops.

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

- **Encoding**: The server encodes once for all clients; a hardware encoder (NVENC, QSV, AMF) keeps the CPU free
- **Bandwidth**: Monitor network bandwidth for multiple clients
- **Latency**: Optimize encoder settings for low-latency requirements
- **Clients**: Each client consumes server resources and bandwidth
- **Port**: Ensure firewall allows traffic on the configured port

## Remarks

- Connect uncompressed video and audio; the server encodes them with `VideoEncoder` and `AudioEncoder`
- The server supports H.264, H.265 and VP9 for video and AAC, MP3, Opus and Vorbis for audio
- Port 8554 is the usual alternative to the standard RTSP port 554, which needs root on Linux; any free port works
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

## Video Transcript

Serve a video file as an RTSP stream, in a few lines of C#. Load the SDK and create a pipeline. The source is the video file. `RTSPServerBlock` encodes it to H.264, and to AAC when the file has sound, and serves it on port 8554. Run it with a file. It prints the URL, and any RTSP player can open it. The film, received over RTSP. This page adds a camera and a password: see [Basic RTSP Server](#basic-rtsp-server) and [RTSP Server with Authentication](#rtsp-server-with-authentication).

The video file is Big Buck Bunny, (c) copyright 2008, Blender Foundation, www.bigbuckbunny.org, CC BY 3.0.
