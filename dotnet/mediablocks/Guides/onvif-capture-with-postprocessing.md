---
title: Capture MP4 from ONVIF Camera with Postprocessing
description: Capture video from ONVIF IP cameras and apply video processing effects like resizing, brightness adjustment, and filters before saving to MP4.
---

# Capture MP4 from ONVIF Camera with Postprocessing

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!!info Demo Samples
For complete working examples, see:
- [RTSP Preview Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) - Shows ONVIF camera preview with postprocessing
- [IP Capture Demo (Video Capture SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) - Alternative using Video Capture SDK

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

Resize video from ONVIF camera before saving to MP4:

```cs
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source from ONVIF camera
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";
rtspSource.Transport = RTSPTransport.TCP;

// Video resize block - downscale to 1280x720
var videoResize = new VideoResizeBlock(1280, 720);
videoResize.Mode = VideoResizeMode.Stretch; // Or Fit, Fill, Crop

// H.264 encoder
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 2000000; // 2 Mbps
h264Encoder.Framerate = 25;
h264Encoder.Profile = H264Profile.High;
h264Encoder.Level = H264Level.Level41;

// AAC audio encoder
var aacEncoder = new AACEncoderBlock();
aacEncoder.Bitrate = 128000; // 128 kbps

// MP4 sink
var mp4Sink = new MP4SinkBlock("output_resized.mp4");

// Connect video pipeline
rtspSource.VideoOutput.Connect(videoResize.Input);
videoResize.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Connect audio pipeline
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Add blocks to pipeline
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(videoResize);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

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

Apply brightness, contrast, hue, and saturation adjustments:

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// Video balance block - adjust brightness, contrast, saturation, hue
var videoBalance = new VideoBalanceBlock();
videoBalance.Brightness = 0.2; // Range: -1.0 to 1.0 (0.2 = 20% brighter)
videoBalance.Contrast = 1.15;   // Range: 0.0 to 2.0 (1.15 = 15% more contrast)
videoBalance.Saturation = 1.3;  // Range: 0.0 to 2.0 (1.3 = 30% more saturation)
videoBalance.Hue = 0.0;         // Range: -1.0 to 1.0 (0 = no hue shift)

// Color effects block - apply preset color effects
var colorEffects = new ColorEffectsBlock();
colorEffects.Preset = ColorEffectsPreset.Sepia; // Try: None, Heat, Sepia, XRay, etc.

// H.264 encoder
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000; // 3 Mbps for higher quality
h264Encoder.Framerate = 25;

// AAC audio
var aacEncoder = new AACEncoderBlock();
aacEncoder.Bitrate = 128000;

// MP4 output
var mp4Sink = new MP4SinkBlock("output_enhanced.mp4");

// Connect video pipeline with effects
rtspSource.VideoOutput.Connect(videoBalance.Input);
videoBalance.Output.Connect(colorEffects.Input);
colorEffects.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Connect audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Add all blocks
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(videoBalance);
await pipeline.AddAsync(colorEffects);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

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
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// CVFaceBlur block - automatic face detection and blurring
var faceBlurSettings = new CVFaceBlurSettings();
faceBlurSettings.ScaleFactor = 1.25;        // Detection scale factor
faceBlurSettings.MinNeighbors = 3;          // Minimum neighbors for detection
faceBlurSettings.MinSize = new Size(30, 30); // Minimum face size
faceBlurSettings.MainCascadeFile = "haarcascade_frontalface_default.xml";

var faceBlur = new CVFaceBlurBlock(faceBlurSettings);

// H.264 encoder
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000;
h264Encoder.Framerate = 25;

// AAC audio
var aacEncoder = new AACEncoderBlock();

// MP4 output
var mp4Sink = new MP4SinkBlock("output_face_blur.mp4");

// Connect video pipeline
rtspSource.VideoOutput.Connect(faceBlur.Input);
faceBlur.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Add blocks
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(faceBlur);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

// Start
await pipeline.StartAsync();
```

## Example 4: Watermark and Logo Overlay

Add a watermark or logo to the video:

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Create pipeline
var pipeline = new MediaBlocksPipeline();

// RTSP source
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// Load logo/watermark
var logoOverlay = new ImageOverlayBlock();
logoOverlay.ImagePath = "watermark.png";
logoOverlay.X = 10;            // 10 pixels from left
logoOverlay.Y = 10;            // 10 pixels from top
logoOverlay.Opacity = 0.7f;     // 70% opaque

// Text overlay for timestamp
var textOverlay = new TextOverlayBlock();
textOverlay.Text = "Camera 1 - {timestamp}";
textOverlay.FontSize = 24;
textOverlay.FontColor = Color.White;
textOverlay.X = 10;
textOverlay.Y = -50;            // 50 pixels from bottom
textOverlay.UpdateInterval = TimeSpan.FromSeconds(1); // Update every second

// H.264 encoder
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000;

// AAC audio
var aacEncoder = new AACEncoderBlock();

// MP4 output
var mp4Sink = new MP4SinkBlock("output_watermarked.mp4");

// Connect video pipeline
rtspSource.VideoOutput.Connect(logoOverlay.Input);
logoOverlay.Output.Connect(textOverlay.Input);
textOverlay.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Add blocks
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(logoOverlay);
await pipeline.AddAsync(textOverlay);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

// Start
await pipeline.StartAsync();
```

## Performance Considerations

1. **CPU Usage**: Video processing is CPU-intensive. Each effect adds overhead:
   - Simple resize: ~10-20% CPU per stream
   - Color correction: ~5-15% CPU
   - Face detection: ~30-50% CPU (depends on resolution)
   - Multiple effects: Additive CPU usage

2. **GPU Acceleration**: Use hardware-accelerated encoders when available:
   ```cs
   // NVIDIA GPU encoding
   var nvencEncoder = new NVENCEncoderBlock();
   nvencEncoder.Bitrate = 4000000;
   nvencEncoder.Preset = NVENCPreset.P4; // Balance quality/performance
   ```

3. **Memory Usage**: Higher resolutions require more memory. Monitor usage:
   ```cs
   pipeline.MemoryWarning += (sender, e) =>
   {
       Console.WriteLine($"Memory warning: {e.UsageMB} MB used");
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

2. **Choose Appropriate Bitrates**:
   - 720p: 1-2 Mbps
   - 1080p: 2-4 Mbps
   - 4K: 8-15 Mbps

3. **Handle Errors Gracefully**:
   ```cs
   pipeline.Error += (sender, e) =>
   {
       Console.WriteLine($"Pipeline error: {e.Message}");
       // Attempt recovery or cleanup
   };
   ```

4. **Dispose Resources**:
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

5. **Monitor Pipeline State**:
   ```cs
   pipeline.StateChanged += (sender, e) =>
   {
       Console.WriteLine($"Pipeline state: {e.State}");
   };
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
- Verify block connections
- Check effect parameters are valid
- Ensure blocks are added to pipeline
- Review pipeline order (effects chain)

---
For simpler recording without postprocessing, see [Save RTSP Stream without Re-encoding](./rtsp-save-original-stream.md).
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for complete working examples.