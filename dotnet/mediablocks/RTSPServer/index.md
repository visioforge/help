---
title: RTSP Server Block for .Net
description: Create RTSP streaming servers for low-latency audio and video delivery over networks with H.264/H.265 codec support in Media Blocks SDK.
sidebar_label: RTSP Server
---

# RTSP Server Block - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

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

- `Port` (`int`): The TCP port for the RTSP server (default: 8554)
- `MountPoint` (`string`): The URL path for the stream (e.g., "/stream1")
- `Username` (`string`): Optional username for authentication
- `Password` (`string`): Optional password for authentication
- `Multicast` (`bool`): Enable multicast streaming
- `MulticastAddress` (`string`): Multicast group address
- `TTL` (`int`): Time-to-live for multicast packets

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

// Create video encoder
var h264Settings = new H264EncoderSettings
{
    Bitrate = 2000,
    Profile = H264Profile.Main
};
var h264Encoder = new H264EncoderBlock(h264Settings);
pipeline.Connect(videoSource.Output, h264Encoder.Input);

// Create audio encoder
var aacSettings = new AACEncoderSettings
{
    Bitrate = 128
};
var aacEncoder = new AACEncoderBlock(aacSettings);
pipeline.Connect(audioSource.Output, aacEncoder.Input);

// Create RTSP server
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/live"
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(h264Encoder.Output, rtspServer.VideoInput);
pipeline.Connect(aacEncoder.Output, rtspServer.AudioInput);

// Start streaming
await pipeline.StartAsync();

// Server is now accessible at rtsp://localhost:8554/live
Console.WriteLine("RTSP server started at rtsp://localhost:8554/live");
Console.WriteLine("Connect with: vlc rtsp://localhost:8554/live");
```

### RTSP Server with Authentication

```csharp
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/secure",
    Username = "admin",
    Password = "password123"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Configure encoders and connect as above
// ...

await pipeline.StartAsync();

// Clients must authenticate: rtsp://admin:password123@localhost:8554/secure
Console.WriteLine("Secure RTSP server started at rtsp://localhost:8554/secure");
```

### File Streaming over RTSP

```csharp
var pipeline = new MediaBlocksPipeline();

// Use file as source
var fileSettings = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
var fileSource = new UniversalSourceBlock(fileSettings);

// Create RTSP server
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/vod"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Connect video and audio from file
pipeline.Connect(fileSource.VideoOutput, rtspServer.VideoInput);
pipeline.Connect(fileSource.AudioOutput, rtspServer.AudioInput);

await pipeline.StartAsync();

// Stream file content via RTSP
Console.WriteLine("Video-on-Demand RTSP server started at rtsp://localhost:8554/vod");
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
