---
title: RTSP Stream Viewer and IP Camera Player in C# .NET
description: Build an RTSP stream viewer and IP camera player in C# with VisioForge Media Blocks SDK — live preview, ONVIF discovery, and passthrough recording.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Recording
  - Encoding
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - UDP
  - MP4
  - TS
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - RTSPSourceSettings
  - RTSPSourceBlock
  - MediaBlocksPipeline
  - VideoRendererBlock
  - RTSPRAWSourceBlock

---

# RTSP Stream Viewer and IP Camera Player in C#

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction

This guide walks you through building an RTSP stream viewer and IP camera player application in C# using the VisioForge Media Blocks SDK. You will learn how to connect to IP cameras via RTSP, display live video with audio, discover cameras using ONVIF, and record streams to file — with or without re-encoding. The Media Blocks SDK runs on Windows, macOS, and Linux, so the same code works across platforms.

Common use cases include surveillance dashboards, NVR (network video recorder) applications, camera management tools, and any project that needs to display or record IP camera feeds programmatically.

!!!info Demo Samples
    For complete working examples, see the [RTSP demos on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK): RTSP Preview Demo, RTSP RAW Capture Demo, and RTSP MultiView Demo.
!!!

## Prerequisites

Add the Media Blocks SDK NuGet package to your project:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

You also need the platform-specific runtime packages. For Windows:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

For other platforms (macOS, Linux, Android, iOS), see the [Deployment Guide](../../deployment-x/index.md).

## Live RTSP Preview

The core pattern connects an RTSP source to video and audio renderers through a `MediaBlocksPipeline`. Initialize the SDK once at application startup, then create pipelines as needed.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// Initialize SDK once at startup
await VisioForgeX.InitSDKAsync();

// Create the pipeline
var pipeline = new MediaBlocksPipeline();

// Connect to the RTSP camera
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/Streaming/Channels/101"),
    "admin",          // login
    "password",       // password
    true);            // audio enabled

var rtspSource = new RTSPSourceBlock(rtspSettings);

// Create video renderer (VideoView1 is an IVideoView control on your form)
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

// Create audio renderer (optional)
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

// Start playback
await pipeline.StartAsync();
```

To stop playback and release resources:

```csharp
await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## RTSP Authentication

Most IP cameras require credentials to access the video stream. You can provide authentication in two ways.

**Credentials as parameters** — pass username and password to `RTSPSourceSettings.CreateAsync()`:

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin",      // username
    "mypassword", // password
    true);        // audio enabled
```

**Credentials in the URL** — embed them directly in the RTSP URI:

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://admin:mypassword@192.168.1.21:554/stream"),
    null, null, true);
```

For ONVIF cameras, authenticate through `ONVIFClientX` to retrieve the stream URI automatically — see the [ONVIF Camera Discovery](#onvif-camera-discovery) section below. Most cameras use digest authentication by default. If you encounter connection failures, verify that your camera's authentication mode matches (digest vs basic) and that the RTSP port (typically 554) is accessible.

## Low-Latency Mode

For real-time monitoring or PTZ control, enable low-latency mode to reduce stream delay from the default ~250ms down to 60–120ms:

```csharp
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

rtspSettings.LowLatencyMode = true;

var rtspSource = new RTSPSourceBlock(rtspSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
videoRenderer.IsSync = false; // Disable A/V sync for lowest latency

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
```

For detailed buffer tuning and UDP vs TCP transport configuration, see the [RTSP Protocol Configuration](../../videocapture/video-sources/ip-cameras/rtsp.md) reference.

## ONVIF Camera Discovery

Automatically discover IP cameras on the local network using the ONVIF discovery protocol, then retrieve stream URIs and camera information.

```csharp
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Discover cameras on the network (5-second timeout)
var discovery = new Discovery();
var cameras = await discovery.Discover(5);

foreach (var camera in cameras)
{
    Console.WriteLine($"Found camera: {camera.XAdresses?.FirstOrDefault()}");
}

// Connect to a specific camera via ONVIF
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(
    "http://192.168.1.21/onvif/device_service",
    "admin",
    "password");

if (connected)
{
    // Get camera info
    var info = onvifClient.DeviceInformation;
    Console.WriteLine($"Camera: {info?.Model}, Serial: {info?.SerialNumber}");

    // Get available profiles and stream URI
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles?.Length > 0)
    {
        var mediaUri = await onvifClient.GetStreamUriAsync(profiles[0]);
        Console.WriteLine($"Stream URI: {mediaUri.Uri}");
    }
}
```

For PTZ control, multiple profiles, and advanced ONVIF features, see the [ONVIF IP Camera Integration](../../videocapture/video-sources/ip-cameras/onvif.md) guide.

## Recording Options

There are two approaches to recording RTSP streams, each suited for different use cases:

| | Passthrough (No Re-encoding) | Re-encoding |
| --- | --- | --- |
| CPU usage | Minimal | High |
| Video quality | Original (lossless) | Re-compressed |
| File size | Same as source | Configurable bitrate |
| Video processing | No overlays, resize, or effects | Full editing capability |
| Best for | Surveillance archival, NVR | Streaming, post-processing |

### Recording with Passthrough (No Re-encoding)

Passthrough recording saves the original compressed stream directly to a file without decoding or re-encoding the video. This approach uses `RTSPRAWSourceBlock` instead of `RTSPSourceBlock` — the video data passes straight from the camera to the file sink with zero CPU overhead for video processing.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;

// Create a separate pipeline for recording
var recordPipeline = new MediaBlocksPipeline();

// Create RAW RTSP source (receives compressed stream directly)
var rawSettings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

var rawSource = new RTSPRAWSourceBlock(rawSettings);

// Create MP4 file sink
var muxer = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));

// Connect video — no decoding, no re-encoding
var videoPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Video);
recordPipeline.Connect(rawSource.VideoOutput, videoPad);

// Connect audio — re-encode to AAC for MP4 compatibility
var audioPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Audio);

var decodeBin = new DecodeBinBlock(false, true, false);
var aacEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

recordPipeline.Connect(rawSource.AudioOutput, decodeBin.Input);
recordPipeline.Connect(decodeBin.AudioOutput, aacEncoder.Input);
recordPipeline.Connect(aacEncoder.Output, audioPad);

await recordPipeline.StartAsync();
```

**Container format choice:** Use MP4 for broad playback compatibility. Use MPEG-TS (`MPEGTSSinkBlock`) for crash-safe recording — if the process terminates unexpectedly, all data written up to that point is preserved. MPEG-TS is preferred for 24/7 surveillance recording.

For a complete implementation with error handling, state management, and resource cleanup, see the [Save RTSP Stream Without Re-encoding](rtsp-save-original-stream.md) guide.

### Recording with Re-encoding

When you need to resize the video, add overlays or watermarks, change the codec, or reduce the bitrate, use `RTSPSourceBlock` (which decodes the stream) followed by encoder blocks:

```csharp
// RTSPSourceBlock decodes the stream, allowing processing
var rtspSource = new RTSPSourceBlock(rtspSettings);
var mp4Sink = new MP4SinkBlock("output.mp4");

// Video: decoded → (optional processing) → encode to H.264 → mux
// Audio: decoded → encode to AAC → mux

pipeline.Connect(rtspSource.VideoOutput, /* encoder/processing blocks */);
```

For detailed code examples of recording with video processing (resize, effects, face detection), see the [ONVIF Capture with Postprocessing](onvif-capture-with-postprocessing.md) guide. For a simpler re-encoding example, see the [IP Camera Capture to MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) tutorial.

## Multi-Camera View

To display multiple IP cameras simultaneously, create independent `MediaBlocksPipeline` instances — one per camera. Each pipeline runs in its own thread and can be started and stopped independently.

```csharp
// Create separate pipelines for each camera
var pipeline1 = new MediaBlocksPipeline();
var pipeline2 = new MediaBlocksPipeline();

// Camera 1
var source1 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"), "admin", "pass1", true));
var renderer1 = new VideoRendererBlock(pipeline1, VideoView1);
pipeline1.Connect(source1.VideoOutput, renderer1.Input);

// Camera 2
var source2 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/stream"), "admin", "pass2", true));
var renderer2 = new VideoRendererBlock(pipeline2, VideoView2);
pipeline2.Connect(source2.VideoOutput, renderer2.Input);

// Start both
await pipeline1.StartAsync();
await pipeline2.StartAsync();
```

The [RTSP MultiView Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) on GitHub shows a grid layout with per-camera record controls and passthrough recording.

## Frequently Asked Questions

### How do I authenticate with an RTSP IP camera in C#?

Embed credentials directly in the RTSP URL (`rtsp://user:pass@ip:554/path`) or pass them as separate parameters to `RTSPSourceSettings.CreateAsync()`. For ONVIF cameras, connect via `ONVIFClientX` first with username and password, then retrieve the authenticated stream URI using `GetStreamUriAsync()`. Most cameras support digest authentication by default.

### Should I use passthrough or re-encoding when recording RTSP streams?

Use passthrough for surveillance archival and NVR applications — it requires zero CPU for video processing and preserves the original camera quality. Use re-encoding when you need to resize, add overlays, change codec or bitrate, or stream to a different format. Most professional recording applications use passthrough to minimize server load.

### How do I reduce RTSP stream latency below 100ms?

Enable `LowLatencyMode = true` on `RTSPSourceSettings` and disable video renderer sync with `IsSync = false`. Use UDP transport when your network supports it. Expected latency: 60–120ms vs the default 250ms. See the [RTSP protocol guide](../../videocapture/video-sources/ip-cameras/rtsp.md) for advanced buffer tuning options.

### Can I view and record from multiple IP cameras simultaneously?

Yes. Create separate `MediaBlocksPipeline` instances for each camera — each pipeline runs independently with its own RTSP connection, decoder, and renderer. You can add per-camera recording by creating additional recording pipelines using `RTSPRAWSourceBlock` for passthrough capture. The RTSP MultiView Demo on GitHub shows a complete implementation with grid layout and individual record controls.

### Which container format should I use for passthrough recording — MP4 or MPEG-TS?

Use MP4 for standard playback compatibility across devices and players. Use MPEG-TS for crash-safe recording — if the application or system crashes during recording, all data written up to the failure point is preserved. For 24/7 surveillance or mission-critical recording, MPEG-TS is the recommended choice.

## See Also

- [Save RTSP Stream Without Re-encoding](rtsp-save-original-stream.md) — detailed passthrough recording with full RTSPRecorder class implementation
- [ONVIF IP Camera Integration](../../videocapture/video-sources/ip-cameras/onvif.md) — ONVIF discovery, profiles, PTZ control
- [RTSP Protocol Configuration](../../videocapture/video-sources/ip-cameras/rtsp.md) — UDP vs TCP, buffer settings, low-latency tuning
- [IP Camera Capture to MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) — recording with re-encoding tutorial
- [ONVIF Capture with Postprocessing](onvif-capture-with-postprocessing.md) — resize, effects, face blur during recording
- [Multi-camera RTSP grid (NVR wall)](multi-camera-rtsp-grid.md) — scale from a single player to a 4×4 live preview wall on WPF or MAUI
- [RTSP reconnection and fallback switch](../../general/network-sources/reconnection-and-fallback.md) — handle camera drops with reconnect events and declarative `FallbackSwitch`
- [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — RTSP preview, capture, and multi-view demos
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — product page and downloads
