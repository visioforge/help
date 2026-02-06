---
title: UDP Video and Audio Streaming in .NET
description: Stream video with UDP protocol for low-latency broadcasts, surveillance, and multicast transmission with minimal overhead in .NET applications.
---

# UDP Streaming with VisioForge SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

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

## Advanced Configuration Options

### Bitrate Management

For optimal streaming performance, consider adjusting the video and audio bitrates based on your network capacity:

```cs
ffmpegOutput.VideoSettings.Bitrate = 2500000; // 2.5 Mbps for video
ffmpegOutput.AudioSettings.Bitrate = 128000;  // 128 kbps for audio
```

### Resolution and Frame Rate

Lower resolutions and frame rates reduce bandwidth requirements:

```cs
VideoCapture1.Video_Resize_Enabled = true;
VideoCapture1.Video_Resize_Width = 1280;    // 720p resolution
VideoCapture1.Video_Resize_Height = 720;
VideoCapture1.Video_FrameRate = 30;         // 30 fps
```

### Buffer Size Configuration

Adjusting buffer sizes can help manage latency vs. stability trade-offs:

```cs
VideoCapture1.Network_Streaming_BufferSize = 8192; // in KB
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

1. **Hardware Acceleration**: When available, enable hardware acceleration for encoding:

```cs
ffmpegOutput.VideoSettings.HWAcceleration = HWAcceleration.Auto;
```

2. **Keyframe Intervals**: For lower latency, reduce keyframe (I-frame) intervals:

```cs
ffmpegOutput.VideoSettings.KeyframeInterval = 60; // One keyframe every 2 seconds at 30 fps
```

3. **Preset Selection**: Choose encoding presets based on your CPU capacity and latency requirements:

```cs
ffmpegOutput.VideoSettings.EncoderPreset = H264EncoderPreset.Ultrafast; // Lowest latency, higher bitrate
// or
ffmpegOutput.VideoSettings.EncoderPreset = H264EncoderPreset.Medium; // Balance between quality and CPU load
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