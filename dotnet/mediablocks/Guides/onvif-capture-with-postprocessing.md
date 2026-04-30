---
title: ONVIF IP Camera Capture to MP4 with Video Effects in C#
description: Capture ONVIF IP camera video and apply resize, brightness, and filter effects before saving to MP4 using VisioForge Media Blocks SDK for .NET.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Recording
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - RTSPSourceBlock
  - RTSPSourceSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - MP4SinkBlock

---

# Capture MP4 from ONVIF Camera with Postprocessing

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    The Media Blocks SDK runs on **Windows, macOS, Linux, Android, and iOS** via GStreamer. See the [platform support matrix](../../platform-matrix.md) for codec and hardware-acceleration details, and the [Linux deployment guide](../../deployment-x/Ubuntu.md) for Ubuntu / NVIDIA Jetson / Raspberry Pi setup.

!!!info Demo Samples
For complete working examples, see:
- [RTSP Preview Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) — Shows ONVIF camera preview with postprocessing
- [IP Capture Demo (Video Capture SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) — Alternative using Video Capture SDK

For comprehensive ONVIF documentation, see the [ONVIF IP Camera Integration Guide](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Table of Contents

- [Capture MP4 from ONVIF Camera with Postprocessing](#capture-mp4-from-onvif-camera-with-postprocessing)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [When to Use Postprocessing](#when-to-use-postprocessing)
  - [Prerequisites](#prerequisites)
  - [Basic Setup: ONVIF Discovery and Connection](#basic-setup-onvif-discovery-and-connection)
  - [Example 1: Resize Video](#example-1-resize-video)
  - [Example 2: Apply Video Effects](#example-2-apply-video-effects)
  - [Example 3: Real-Time Face Blur](#example-3-real-time-face-blur)
  - [Example 4: Watermark and Logo Overlay](#example-4-watermark-and-logo-overlay)
  - [Performance Considerations](#performance-considerations)
  - [Best Practices](#best-practices)
  - [Troubleshooting](#troubleshooting)

## Overview

This guide demonstrates how to capture video from ONVIF IP cameras while applying various postprocessing effects before encoding to MP4. Unlike pass-through recording which preserves the original stream, postprocessing requires decoding the video, applying transformations, and re-encoding.

This approach is useful when you need to:
- Resize or crop video
- Apply brightness, contrast, or color corrections
- Add watermarks or logos
- Blur faces for privacy
- Apply artistic effects or filters
- Combine multiple processing steps

## When to Use Postprocessing

**Use postprocessing when:**
- You need to resize video (e.g., from 4K to 1080p)
- You want to apply video effects (brightness, contrast, etc.)
- You need to add overlays or watermarks
- Privacy requirements mandate face blurring
- You're combining multiple camera feeds
- You need to apply AI/CV algorithms

**Use pass-through (no postprocessing) when:**
- You want to preserve original video quality
- You need to minimize CPU usage
- Storage space is not a concern
- Recording duration is long (hours/days)

For pass-through recording, see [Save RTSP Stream without Re-encoding](./rtsp-save-original-stream.md).

## Prerequisites

1. **VisioForge Media Blocks SDK .NET** installed
2. **ONVIF Camera** accessible on your network
3. **Valid camera credentials** (username and password)
4. **Basic understanding of**:
   - C# async/await
   - ONVIF protocol basics
   - Video encoding parameters

## Basic Setup: ONVIF Discovery and Connection

First, discover and connect to your ONVIF camera:

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Discover ONVIF cameras
var discovery = new Discovery();
var cts = new CancellationTokenSource();
string cameraUrl = null;

await discovery.Discover(5, (device) =>
{
    if (device.XAdresses?.Any() == true)
    {
        cameraUrl = device.XAdresses.FirstOrDefault();
        Console.WriteLine($"Found camera: {cameraUrl}");
    }
}, cts.Token);

if (string.IsNullOrEmpty(cameraUrl))
{
    Console.WriteLine("No ONVIF cameras found");
    return;
}

// Connect to camera
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(cameraUrl, "admin", "password");

if (!connected)
{
    Console.WriteLine("Failed to connect to camera");
    return;
}

// Get RTSP stream URL
var profiles = await onvifClient.GetProfilesAsync();
var streamUri = await onvifClient.GetStreamUriAsync(profiles[0]);
string rtspUrl = streamUri.Uri;

Console.WriteLine($"RTSP URL: {rtspUrl}");
```

## Example 1: Resize Video

Resize video from ONVIF camera before saving to MP4. The Media Blocks API takes
settings objects, not fluent properties — `RTSPSourceBlock` is constructed from
an `RTSPSourceSettings`, the encoder from its settings object, the MP4 sink hands
out input pads via `CreateNewInput(...)`, and links are declared on the pipeline.

```cs
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source from ONVIF camera. Credentials and latency live on the settings
// object; use the async factory so the settings discover codec info up front.
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Video resize block — downscale to 1280x720. The (width, height) ctor is a
// shortcut for `new VideoResizeBlock(new ResizeVideoEffect(w, h))`.
var videoResize = new VideoResizeBlock(1280, 720);

// H.264 encoder. Pick a concrete settings class — Bitrate is in Kbit/s (2000 = 2 Mbps).
// OpenH264EncoderSettings works on every platform; swap to NVENC / QSV / AMF / MFH264 for GPU acceleration.
var h264Settings = new OpenH264EncoderSettings { Bitrate = 2000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio encoder (Bitrate is in Kbit/s — 128 = 128 kbps).
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// MP4 sink — file-path ctor is the shortest path to a valid MP4 writer.
var mp4Sink = new MP4SinkBlock("output_resized.mp4");

// Add every block to the pipeline before wiring links
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoResize);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Wire the video path (RTSP video → resize → H.264 → MP4 sink video pad)
pipeline.Connect(rtspSource.VideoOutput, videoResize.Input);
pipeline.Connect(videoResize.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Wire the audio path (RTSP audio → AAC → MP4 sink audio pad)
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start recording
await pipeline.StartAsync();

Console.WriteLine("Recording with resize... Press Enter to stop.");
Console.ReadLine();

// Stop and cleanup
await pipeline.StopAsync();
await pipeline.DisposeAsync();

Console.WriteLine("Recording complete: output_resized.mp4");
```

## Example 2: Apply Video Effects

Apply brightness, contrast, hue, and saturation adjustments. Video-processing
blocks take a settings object in the ctor; the settings carry the knobs.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Video-balance block — knobs live on the settings object.
// Brightness: -1.0..1.0 (0.2 = slightly brighter)
// Contrast:    0.0..2.0 (1.15 = +15% contrast)
// Saturation:  0.0..2.0 (1.3  = +30% saturation)
// Hue:        -1.0..1.0 (0.0  = no shift)
var balanceSettings = new VideoBalanceVideoEffect
{
    Brightness = 0.2,
    Contrast   = 1.15,
    Saturation = 1.3,
    Hue        = 0.0,
};
var videoBalance = new VideoBalanceBlock(balanceSettings);

// Color-effects block takes a preset directly in the ctor
var colorEffects = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

// H.264 encoder (3 Mbps)
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// MP4 output
var mp4Sink = new MP4SinkBlock("output_enhanced.mp4");

// Add everything to the pipeline, then wire
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoBalance);
pipeline.AddBlock(colorEffects);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Video chain: RTSP → balance → color-effects → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, videoBalance.Input);
pipeline.Connect(videoBalance.Output, colorEffects.Input);
pipeline.Connect(colorEffects.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Audio chain
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start
await pipeline.StartAsync();

Console.WriteLine("Recording with enhancements...");
await Task.Delay(TimeSpan.FromMinutes(5)); // Record for 5 minutes

await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Example 3: Real-Time Face Blur

Apply face detection and blurring for privacy protection:

```cs
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// CVFaceBlurBlock — automatic face detection and blurring via OpenCV
var faceBlurSettings = new CVFaceBlurSettings
{
    ScaleFactor      = 1.25,
    MinNeighbors     = 3,
    MinSize          = new Size(30, 30),
    MainCascadeFile  = "haarcascade_frontalface_default.xml",
};
var faceBlur = new CVFaceBlurBlock(faceBlurSettings);

// H.264 encoder
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// MP4 output
var mp4Sink = new MP4SinkBlock("output_face_blur.mp4");

// Add + wire
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(faceBlur);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

pipeline.Connect(rtspSource.VideoOutput, faceBlur.Input);
pipeline.Connect(faceBlur.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start
await pipeline.StartAsync();
```

## Example 4: Watermark and Logo Overlay

Add a watermark logo and a timestamp overlay. `ImageOverlayBlock` takes either a
filename or an `ImageOverlaySettings`; position/opacity live on the settings.
`TextOverlayBlock` always takes a `TextOverlaySettings`.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;
using SkiaSharp;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Logo / watermark: file-path ctor loads the image. Position knobs live on the
// settings object; transparency is Alpha (0..1, not Opacity).
var logoOverlay = new ImageOverlayBlock(new ImageOverlaySettings("watermark.png")
{
    X     = 10,   // 10 px from left
    Y     = 10,   // 10 px from top
    Alpha = 0.7,  // 0..1
});

// Static text overlay. TextOverlaySettings carries Text, position, Color (SKColor),
// and font knobs — see the OverlayManagerText reference for every option.
var textOverlay = new TextOverlayBlock(new TextOverlaySettings("Camera 1")
{
    Color = SKColors.White,
});

// H.264 encoder
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// MP4 output
var mp4Sink = new MP4SinkBlock("output_watermarked.mp4");

// Add + wire
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(logoOverlay);
pipeline.AddBlock(textOverlay);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Video chain: RTSP → logo → text → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, logoOverlay.Input);
pipeline.Connect(logoOverlay.Output, textOverlay.Input);
pipeline.Connect(textOverlay.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Audio chain
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Start
await pipeline.StartAsync();
```

## Performance Considerations

1. **CPU Usage**: Video processing is CPU-intensive. Each effect adds overhead:
   - Simple resize: ~10-20% CPU per stream
   - Color correction: ~5-15% CPU
   - Face detection: ~30-50% CPU (depends on resolution)
   - Multiple effects: Additive CPU usage

2. **GPU Acceleration**: Use hardware-accelerated encoders when available.
   `H264EncoderBlock.GetDefaultSettings()` already prefers NVENC / QSV / AMF when
   the platform supports it, but you can force a specific backend:

   ```cs
   // NVIDIA NVENC H.264 encoder (Bitrate in Kbit/s — 4000 = 4 Mbps)
   var nvencSettings = new NVENCH264EncoderSettings { Bitrate = 4000 };
   var h264Encoder   = new H264EncoderBlock(nvencSettings);
   ```

3. **Error handling**: Subscribe to `OnError` to learn about pipeline failures:

   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Console.WriteLine($"Pipeline error: {e.Message}");
   };
   ```

4. **Encoding Settings Balance**:
   - **Quality**: Higher bitrate, slower preset = better quality, more CPU
   - **Performance**: Lower bitrate, faster preset = lower quality, less CPU
   - **File Size**: Bitrate directly affects file size

## Best Practices

1. **Test Performance First**:
   - Start with simple pipeline
   - Add effects one at a time
   - Monitor CPU/memory usage
   - Adjust settings based on hardware

2. **Choose Appropriate Bitrates** (all values in Kbit/s):
   - 720p: 1000-2000
   - 1080p: 2000-4000
   - 4K: 8000-15000

3. **Dispose Resources**:

   ```cs
   try
   {
       await pipeline.StartAsync();
       // ... recording ...
   }
   finally
   {
       await pipeline.StopAsync();
       await pipeline.DisposeAsync();
       onvifClient?.Dispose();
   }
   ```

4. **Observe pipeline lifecycle events**:

   ```cs
   pipeline.OnStart  += (s, e) => Console.WriteLine("Pipeline started");
   pipeline.OnStop   += (s, e) => Console.WriteLine("Pipeline stopped");
   pipeline.OnPause  += (s, e) => Console.WriteLine("Pipeline paused");
   pipeline.OnResume += (s, e) => Console.WriteLine("Pipeline resumed");
   ```

## Troubleshooting

**High CPU Usage:**
- Reduce video resolution
- Lower encoder preset (faster encoding)
- Remove unnecessary effects
- Use GPU acceleration if available

**Dropped Frames:**
- Check if CPU is bottlenecked
- Reduce frame rate
- Lower bitrate
- Simplify processing pipeline

**Poor Video Quality:**
- Increase bitrate
- Use slower encoder preset
- Check source video quality
- Verify network bandwidth for RTSP

**Memory Leaks:**
- Ensure proper disposal of blocks
- Check for circular references
- Monitor long-running recordings

**Effects Not Applied:**
- Verify block connections via `pipeline.Connect(outPad, inPad)`
- Check effect parameters are valid
- Ensure every block is registered via `pipeline.AddBlock(...)` before `StartAsync`
- Review pipeline order (effects chain)

---
For simpler recording without postprocessing, see [Save RTSP Stream without Re-encoding](./rtsp-save-original-stream.md).
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for complete working examples.
