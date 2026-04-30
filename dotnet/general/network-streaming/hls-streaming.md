---
title: HLS Live Streaming in C# .NET — m3u8 Playlist & Segments
description: Generate .m3u8 playlists and .ts segments with built-in HTTP server. HLS.js, iOS AVPlayer, and Android ExoPlayer integration. VisioForge SDK examples.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - HLS
  - TS
  - H.264
  - AAC
  - C#
primary_api_classes:
  - HLSOutput
  - HLSSinkSettings
  - MediaBlockPadMediaType
  - HLSPlaylistType
  - H264EncoderBlock

---

# Complete Guide to HLS Network Streaming Implementation in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## What is HTTP Live Streaming (HLS)?

HTTP Live Streaming (HLS) is an adaptive bitrate streaming communications protocol designed and developed by Apple Inc. First introduced in 2009, it has since become one of the most widely adopted streaming protocols across various platforms and devices. HLS works by breaking the overall stream into a sequence of small HTTP-based file downloads, each containing a short segment of the overall content.

### Key Features of HLS Streaming

- **Adaptive Bitrate Streaming**: HLS automatically adjusts video quality based on the viewer's network conditions, ensuring optimal playback quality without buffering.
- **Cross-Platform Compatibility**: Works across iOS, macOS, Android, Windows, and most modern web browsers.
- **HTTP-Based Delivery**: Leverages standard web server infrastructure, allowing content to pass through firewalls and proxy servers.
- **Media Encryption and Authentication**: Supports content protection through encryption and various authentication methods.
- **Live and On-Demand Content**: Can be used for both live broadcasting and pre-recorded media.

### HLS Technical Structure

HLS content delivery relies on three key components:

1. **Manifest File (.m3u8)**: A playlist file that contains metadata about the various streams available
2. **Segment Files (.ts)**: The actual media content, divided into small chunks (typically 2-10 seconds each)
3. **HTTP Server**: Responsible for delivering both manifest and segment files

Since HLS is entirely HTTP-based, you'll need either a dedicated HTTP server or can use the lightweight internal server provided by our SDKs.

## Implementing HLS Streaming with Media Blocks SDK

The Media Blocks SDK offers a comprehensive approach to HLS streaming through its pipeline architecture, giving developers granular control over each aspect of the streaming process.

### Creating a Basic HLS Stream

The following example demonstrates how to set up an HLS stream using Media Blocks SDK:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialise the cross-platform engine once per process.
VisioForgeX.InitSDK();

const string URL = "http://localhost:8088/";

// Pipeline (attach error handler before adding blocks).
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");

// Video source: first enumerated capture device.
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Audio source: first enumerated audio capture device.
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// H.264 and AAC encoders.
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
var aacEncoder = new AACEncoderBlock();

// HLS sink.
var settings = new HLSSinkSettings
{
    Location             = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles             = 10,
    PlaylistLength       = 5,
    PlaylistLocation     = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot         = URL.TrimEnd('/'),
    SendKeyframeRequests = true,
    TargetDuration       = TimeSpan.FromSeconds(5),  // TimeSpan, not int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port    = 8088
};

var hlsSink = new HLSSinkBlock(settings);

// Wire the graph: source → encoder → HLS sink (separate pads for video/audio).
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(h264Encoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(aacEncoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start. Open http://localhost:8088/playlist.m3u8 in a player or browser.
await pipeline.StartAsync();
```

### Advanced Configuration Options

The Media Blocks SDK offers several advanced configuration options for HLS streaming:

- **Multiple Bitrate Variants**: Create different quality levels for adaptive streaming
- **Custom Segment Duration**: Optimize for different types of content and viewing environments
- **Server-Side Options**: Configure cache control headers and other server behaviors
- **Security Features**: Implement token-based authentication or encryption

You can use this SDK to stream both live video capture and existing media files to HLS. The flexible pipeline architecture allows for extensive customization of the media processing workflow.

## HLS Streaming with Video Capture SDK .NET

Video Capture SDK .NET provides a streamlined approach to HLS streaming specifically designed for live video sources like webcams, capture cards, and other input devices.

### VideoCaptureCoreX Implementation

The VideoCaptureCoreX engine offers a modern, object-oriented approach to video capture and streaming:

```csharp
// Create HLS sink settings
var settings = new HLSSinkSettings
{
    Location = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles = 10,
    PlaylistLength = 5,
    PlaylistLocation = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot = edStreamingKey.Text,
    SendKeyframeRequests = true,
    TargetDuration = TimeSpan.FromSeconds(5), // TimeSpan, not int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port = new Uri(edStreamingKey.Text).Port
};

// Create HLS output
var hlsOutput = new HLSOutput(settings);

// Create video and audio encoders with default settings
hlsOutput.Video = new OpenH264EncoderSettings();
hlsOutput.Audio = new VOAACEncoderSettings();

// Add HLS output to video capture object
videoCapture.Outputs_Add(hlsOutput, true);
```

### VideoCaptureCore Implementation

For those working with the traditional VideoCaptureCore engine, the implementation is slightly different but equally straightforward:

```csharp
VideoCapture1.Network_Streaming_Enabled = true;
VideoCapture1.Network_Streaming_Audio_Enabled = true;
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Segment duration in seconds
        NumSegments = 5,                        // Number of segments in playlist
        OutputFolder = "c:\\hls\\",             // Output folder
        PlaylistType = HLSPlaylistType.Live,    // Playlist type
        Custom_HTTP_Server_Enabled = true,      // Use internal HTTP server
        Custom_HTTP_Server_Port = 8088          // Port for internal HTTP server
    }
};

VideoCapture1.Network_Streaming_Output = hls;
```

### Performance Considerations

When streaming with Video Capture SDK, consider these performance optimization techniques:

- Keep segment durations between 2-10 seconds (10 seconds is optimal for most use cases)
- Adjust the number of segments based on expected viewing patterns
- Use hardware acceleration when available for encoding
- Configure appropriate bitrates based on your target audience's connection speeds

## Converting Media Files to HLS with Video Edit SDK .NET

The Video Edit SDK .NET enables developers to convert existing media files into HLS format for streaming, ideal for video-on-demand applications.

### VideoEditCore Implementation

```csharp
VideoEdit1.Network_Streaming_Enabled = true;
VideoEdit1.Network_Streaming_Audio_Enabled = true;
VideoEdit1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Segment duration in seconds
        NumSegments = 5,                        // Number of segments in playlist
        OutputFolder = "c:\\hls\\",             // Output folder
        PlaylistType = HLSPlaylistType.Live,    // Playlist type
        Custom_HTTP_Server_Enabled = true,      // Use internal HTTP server
        Custom_HTTP_Server_Port = 8088          // Port for internal HTTP server
    }
};

VideoEdit1.Network_Streaming_Output = hls;
```

### File Format Considerations

When converting files to HLS, consider these factors:

- Not all input formats are equally efficient for conversion
- MP4, MOV, and MKV files typically provide the best results
- Highly compressed formats may require more processing power
- Consider pre-transcoding very large files to an intermediate format

## Playback and Integration

### HTML5 Player Integration

All applications implementing HLS streaming should include an HTML file with a video player. Modern HTML5 players like HLS.js, Video.js, or JW Player provide excellent support for HLS streams.

Here's a basic example using HLS.js:

```html
<!DOCTYPE html>
<html>
<head>
    <title>HLS Player</title>
    <script src="https://cdn.jsdelivr.net/npm/hls.js@latest"></script>
</head>
<body>
    <video id="video" controls></video>
    <script>
      var video = document.getElementById('video');
      var videoSrc = 'http://localhost:8088/playlist.m3u8';
      
      if (Hls.isSupported()) {
        var hls = new Hls();
        hls.loadSource(videoSrc);
        hls.attachMedia(video);
      } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = videoSrc;
      }
    </script>
</body>
</html>
```

For a complete example player, refer to our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/blob/master/Media%20Blocks%20SDK/Console/HLS%20Streamer/index.htm).

### Mobile App Integration

Our SDKs also support integration with mobile applications through:

- Native iOS playback using AVPlayer
- Android playback via ExoPlayer
- Cross-platform options like Xamarin or MAUI

## Troubleshooting Common Issues

### CORS Configuration

When serving HLS content to web browsers, you may encounter Cross-Origin Resource Sharing (CORS) issues. Ensure your server is configured to send the proper CORS headers:

```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, HEAD, OPTIONS
Access-Control-Allow-Headers: Range
Access-Control-Expose-Headers: Accept-Ranges, Content-Encoding, Content-Length, Content-Range
```

### Latency Optimization

HLS inherently introduces some latency. To minimize this:

- Use shorter segment durations (2-4 seconds) for lower latency
- Consider enabling Low-Latency HLS (LL-HLS) if supported
- Optimize your network infrastructure for minimal delays

## Conclusion

HLS streaming provides a robust, cross-platform solution for delivering both live and on-demand video content to a wide range of devices. With VisioForge's .NET SDKs, implementing HLS in your applications becomes straightforward, allowing you to focus on creating compelling content rather than wrestling with technical details.

For more code samples and advanced implementations, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).

---
## Additional Resources
- [HLS Specification](https://developer.apple.com/streaming/)