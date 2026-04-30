---
title: UDP Video Streaming with MPEG-TS Container in C# .NET
description: Stream H.264/HEVC video over UDP in C# / .NET: multicast, point-to-point, low-latency configs. Full send/receive code samples with bitrate tuning.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - UDP
  - MP4
  - TS
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - FFMPEGEXEOutput
  - BasicVideoSettings
  - MediaBlockPadMediaType
  - MediaBlocksPipeline
  - UDPMPEGTSSinkBlock
  - UDPSinkSettings
  - MultiUDPMPEGTSSinkBlock

---

# UDP Streaming with VisioForge SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction to UDP Streaming

The User Datagram Protocol (UDP) is a lightweight, connectionless transport protocol that provides a simple interface between network applications and the underlying IP network. Unlike TCP, UDP offers minimal overhead and doesn't guarantee packet delivery, making it ideal for real-time applications where speed is crucial and occasional packet loss is acceptable.

VisioForge SDKs offer robust support for UDP streaming, enabling developers to implement high-performance, low-latency streaming solutions for various applications, including live broadcasts, video surveillance, and real-time communication systems.

## Key Features and Capabilities

The VisioForge SDK suite provides comprehensive UDP streaming functionality with the following key features:

### Video and Audio Codec Support

- **Video Codecs**: Full support for H.264 (AVC) and H.265 (HEVC), offering excellent compression efficiency while maintaining high video quality.
- **Audio Codec**: Advanced Audio Coding (AAC) support, providing superior audio quality at lower bitrates compared to older audio codecs.

### MPEG Transport Stream (MPEG-TS)

The SDK utilizes MPEG-TS as the container format for UDP streaming. MPEG-TS offers several advantages:

- Designed specifically for transmission over potentially unreliable networks
- Built-in error correction capabilities
- Support for multiplexing multiple audio and video streams
- Low latency characteristics ideal for live streaming

### FFMPEG Integration

VisioForge SDKs leverage the power of FFMPEG for UDP streaming, ensuring:

- High performance encoding and streaming
- Wide compatibility with various networks and receiving clients
- Reliable packet handling and stream management

### Unicast and Multicast Support

- **Unicast**: Point-to-point transmission from a single sender to a single receiver
- **Multicast**: Efficient distribution of the same content to multiple recipients simultaneously without duplicating bandwidth at the source

## Technical Implementation Details

UDP streaming in VisioForge SDKs involves several key technical components:

1. **Video Encoding**: Source video is compressed using H.264 or HEVC encoders with configurable parameters for bitrate, resolution, and frame rate.

2. **Audio Encoding**: Audio streams are processed through AAC encoders with adjustable quality settings.

3. **Multiplexing**: Video and audio streams are combined into a single MPEG-TS container.

4. **Packetization**: The MPEG-TS stream is divided into UDP packets of appropriate size for network transmission.

5. **Transmission**: Packets are sent over the network to specified unicast or multicast addresses.

The implementation prioritizes low latency while maintaining sufficient quality for professional applications. Advanced buffering mechanisms help manage network jitter and ensure smooth playback at the receiving end.

## Windows-only UDP Output Implementation

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

### Step 1: Enable Network Streaming

The first step is to enable network streaming functionality in your application. This is done by setting the `Network_Streaming_Enabled` property to true:

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

### Step 2: Configure Audio Streaming (Optional)

If your application requires audio streaming alongside video, enable it with:

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

### Step 3: Set the Streaming Format

Specify UDP as the streaming format by setting the `Network_Streaming_Format` property to `UDP_FFMPEG_EXE`:

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.UDP_FFMPEG_EXE;
```

### Step 4: Configure the UDP Stream URL

Set the destination URL for your UDP stream. For a basic unicast stream to localhost:

```cs
VideoCapture1.Network_Streaming_URL = "udp://127.0.0.1:10000?pkt_size=1316";
```

The `pkt_size` parameter defines the UDP packet size. The value 1316 is optimized for most network environments, allowing for efficient transmission while minimizing fragmentation.

### Step 5: Multicast Configuration (Optional)

For multicast streaming to multiple receivers, use a multicast address (typically in the range 224.0.0.0 to 239.255.255.255):

```cs
VideoCapture1.Network_Streaming_URL = "udp://239.101.101.1:1234?ttl=1&pkt_size=1316";
```

The additional parameters include:
- **ttl**: Time-to-live value that determines how many network hops the packets can traverse
- **pkt_size**: Packet size as explained above

### Step 6: Configure Output Settings

Finally, configure the streaming output parameters using the `FFMPEGEXEOutput` class:

```cs
var ffmpegOutput = new FFMPEGEXEOutput();

ffmpegOutput.FillDefaults(DefaultsProfile.MP4_H264_AAC, true);
ffmpegOutput.OutputMuxer = OutputMuxer.MPEGTS;

VideoCapture1.Network_Streaming_Output = ffmpegOutput;
```

This code:
1. Creates a new FFMPEG output configuration
2. Applies default settings for H.264 video and AAC audio
3. Specifies MPEG-TS as the container format
4. Assigns this configuration to the streaming output

## Cross-platform UDP Output with Media Blocks

[MediaBlocksPipeline](#){ .md-button }

The Media Blocks SDK provides cross-platform UDP streaming support using GStreamer-based blocks. These blocks work on Windows, macOS, Linux, iOS, and Android.

### Single-destination MPEG-TS Streaming

Use `UDPMPEGTSSinkBlock` to multiplex audio and video into MPEG-TS and send over UDP to a single destination:

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("input.mp4")));

var videoEncoder = new H264EncoderBlock(new OpenH264EncoderSettings());
var audioEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings() { Bitrate = 192 });

pipeline.Connect(fileSource.VideoOutput, videoEncoder.Input);
pipeline.Connect(fileSource.AudioOutput, audioEncoder.Input);

var udpSettings = new UDPSinkSettings
{
    Host = "192.168.1.100",
    Port = 5004,
    TTL = 64
};

var udpSink = new UDPMPEGTSSinkBlock(udpSettings);
pipeline.Connect(videoEncoder.Output, udpSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, udpSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

### Multi-destination MPEG-TS Streaming

Use `MultiUDPMPEGTSSinkBlock` to send the same MPEG-TS stream to multiple receivers simultaneously:

```csharp
var multiUdpSettings = new MultiUDPSinkSettings();
multiUdpSettings.AddClient("192.168.1.100", 5004);
multiUdpSettings.AddClient("192.168.1.101", 5004);

var multiUdpSink = new MultiUDPMPEGTSSinkBlock(multiUdpSettings);
pipeline.Connect(videoEncoder.Output, multiUdpSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, multiUdpSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

### Multicast Streaming

For multicast delivery, set the `Host` to a multicast address (224.0.0.0 – 239.255.255.255):

```csharp
var udpSettings = new UDPSinkSettings
{
    Host = "239.101.101.1",
    Port = 5004,
    MulticastTTL = 4,
    AutoMulticast = true
};
```

### Receiving UDP Streams

You can verify the stream using GStreamer command-line tools:

```bash
gst-launch-1.0 udpsrc port=5004 ! tsdemux ! decodebin ! autovideosink
```

Or receive with VLC:

```
vlc udp://@:5004
```

## Advanced Configuration Options

### Bitrate Management

For optimal streaming performance, adjust the video and audio bitrates to match
your network capacity. `FFMPEGEXEOutput` exposes the encoder knobs via `.Video`
and `.Audio` (not `VideoSettings`/`AudioSettings`), and the underlying
`BasicVideoSettings` / `BasicAudioSettings` store bitrate in **kbps**:

```cs
ffmpegOutput.Video.Bitrate = 2500; // 2.5 Mbps for video (kbps)
ffmpegOutput.Audio.Bitrate = 128;  // 128 kbps for audio
```

### Resolution and Frame Rate

Lower resolutions reduce bandwidth. Set the target size inside
`VideoCapture1.Video_Resize` (the classic engine exposes it as an
`IVideoResizeSettings` object, not flat properties on the core), and enable
the resize stage with `Video_ResizeOrCrop_Enabled`:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width  = 1280,   // 720p resolution
    Height = 720,
    Mode   = VideoResizeMode.Letterbox,
};

// Frame rate is configured on the capture device format, not the core — pick
// a 30 fps device format via Video_CaptureDevice_Format / _FrameRate.
```

### Buffer Size Configuration

Latency vs. stability for FFMPEG-based streaming is controlled on the output
object, not on the core. Milliseconds:

```cs
ffmpegOutput.VideoBufferSize = 5000; // 5 s buffer for smoother streaming
```

## Best Practices for UDP Streaming

### Network Considerations

1. **Bandwidth Assessment**: Ensure sufficient bandwidth for your target quality. As a guideline:
   - SD quality (480p): 1-2 Mbps
   - HD quality (720p): 2.5-4 Mbps
   - Full HD (1080p): 4-8 Mbps

2. **Network Stability**: UDP doesn't guarantee packet delivery. In unstable networks, consider:
   - Reducing resolution or bitrate
   - Implementing application-level error recovery
   - Using forward error correction when available

3. **Firewall Configuration**: Ensure that UDP ports are open on both sender and receiver firewalls.

### Performance Optimization

1. **Hardware Acceleration / Keyframes / Preset**: `FFMPEGEXEOutput` does not expose
   first-class properties for HW accel, keyframe interval, or x264 presets — instead
   inject them as FFMPEG CLI flags via `Custom_AdditionalVideoArgs`. FFMPEG then
   applies them to the video encoder invocation.

```cs
// NVENC hardware encoder + 2-second keyframe interval (60 frames @ 30 fps)
// + ultrafast preset (lowest latency).
ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_nvenc -g 60 -preset p1";

// Intel QuickSync instead:
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_qsv -g 60";

// Software x264 with a quality/speed trade-off:
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v libx264 -g 60 -preset ultrafast";
```

2. **Pipe-based transport** (avoids a temp file between SDK and FFMPEG) generally
   reduces latency:

```cs
ffmpegOutput.UsePipe = true;
```

## Troubleshooting Common Issues

1. **Stream Not Receiving**: Verify network connectivity, port availability, and firewall settings.

2. **High Latency**: Check network congestion, reduce bitrate, or adjust buffer sizes.

3. **Poor Quality**: Increase bitrate, adjust encoding settings, or check for network packet loss.

4. **Audio/Video Sync Issues**: Ensure proper timestamp synchronization in your application.

## Conclusion

UDP streaming with VisioForge SDKs provides a powerful solution for real-time video and audio transmission with minimal latency. By leveraging H.264/HEVC video codecs, AAC audio, and MPEG-TS packaging, developers can create robust streaming applications suitable for a wide range of use cases.

The flexibility of the SDK allows for fine-tuning of all streaming parameters, enabling optimization for specific network conditions and quality requirements. Whether implementing a simple point-to-point stream or a complex multicast distribution system, VisioForge's UDP streaming capabilities provide the necessary tools for success.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and working demonstrations of UDP streaming implementations.