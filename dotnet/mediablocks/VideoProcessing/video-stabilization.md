---
title: Video Stabilization SDK for .NET — VideoStabilizationBlock
description: Remove camera shake from live or recorded video in C# with VideoStabilizationBlock (OpenCV deshake) from the VisioForge Media Blocks SDK.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - GStreamer
  - OpenCV
  - Effects
  - Playback
  - C#
primary_api_classes:
  - VideoStabilizationBlock
  - VideoStabilizationSettings
  - UniversalSourceBlock
  - VideoRendererBlock
  - MediaBlocksPipeline

---

# Video Stabilization Block

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

The `VideoStabilizationBlock` removes camera shake from a live or recorded video stream in real time. It estimates the global inter-frame motion (translation and rotation) with sparse optical flow, smooths the resulting camera trajectory with a causal moving-average window, and warps every frame back onto the smoothed path. A small centre zoom (crop ratio) hides the borders exposed by the compensation.

The block is backed by the OpenCV `vfdeshake` GStreamer element, so it requires the OpenCV SDK redistributable. It is currently available on Windows.

Latency is zero: only past frames are used for smoothing, so the block can be used in live pipelines.

## Key Features

- Global motion estimation (translation + rotation) via `goodFeaturesToTrack` + pyramidal Lucas-Kanade optical flow and a partial-affine RANSAC fit.
- Causal trajectory smoothing with a configurable window radius - no added frame latency.
- Automatic centre zoom (`CropRatio`) to keep the borders hidden.
- Per-frame correction limits (`MaxShift`, `MaxAngle`) as safety limiters.
- Live retuning: every property can be changed while the pipeline runs.
- Passthrough mode (`Enabled = false`) for an instant before/after comparison.

## Settings

`VideoStabilizationSettings` maps one-to-one to the element properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Enabled` | `bool` | `true` | When `false` the block passes frames through untouched. |
| `SmoothingRadius` | `int` | `15` | Number of past frames averaged when smoothing the camera trajectory. Larger = steadier, slower-reacting. Range `1` - `1000`. |
| `CropRatio` | `double` | `0.9` | Fraction of the frame kept visible. The frame is zoomed by `1 / CropRatio` around its centre to hide the exposed borders. `1.0` disables the zoom. Range `0.5` - `1.0`. |
| `MaxShift` | `int` | `100` | Maximum per-frame translation correction in pixels (safety limiter). Minimum `0`. |
| `MaxAngle` | `double` | `15` | Maximum per-frame rotation correction in degrees (safety limiter). Range `0` - `90`. |

Every property **clamps silently** to its range on assignment - an out-of-range value is corrected, not rejected, so read the property back if you need to know what was applied.

## Availability

`IsAvailable()` looks the `vfdeshake` element up in the GStreamer registry, so call it **after** the SDK has been initialized:

```csharp
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Add the VisioForge.CrossPlatform.OpenCV.Windows.x64 NuGet package.
}
```

## Sample code - preview a stabilized file

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;

// Initialize the SDK once at application startup before building any pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    Enabled = true,
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

// VideoView1 is a VisioForge VideoView control on your form/window.
var renderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, renderer.Input);

await pipeline.StartAsync();
```

## Sample code - stabilize and save to MP4

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialize the SDK once at application startup before building any pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// The file plays to its end and the pipeline finalizes the MP4 on EOS - handle OnStop to know when.
// To end the recording EARLY, call StopAsync(): it pushes EOS so the muxer writes the moov atom.
// Killing the process instead leaves an unplayable file.
// await pipeline.StopAsync();
```

## Sample code - preview and record the stabilized output

To watch the stabilized video and record it at the same time, split the stabilizer output with a `TeeBlock`: one branch feeds the renderer, the other the H264 encoder and the MP4 sink.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialize the SDK once at application startup before building any pipeline.
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

var source = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(@"C:\Videos\shaky.mp4", renderVideo: true, renderAudio: false));

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings { CropRatio = 0.9 });

var tee = new TeeBlock(2, MediaBlockPadMediaType.Video);
var renderer = new VideoRendererBlock(pipeline, VideoView1);
var h264 = new H264EncoderBlock(new OpenH264EncoderSettings());
var mp4 = new MP4SinkBlock(new MP4SinkSettings(@"C:\Videos\stabilized.mp4"));

pipeline.Connect(source.VideoOutput, stabilizer.Input);
pipeline.Connect(stabilizer.Output, tee.Input);
pipeline.Connect(tee.Outputs[0], renderer.Input);
pipeline.Connect(tee.Outputs[1], h264.Input);
pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// The file plays to its end and the pipeline finalizes the MP4 on EOS - handle OnStop to know when.
// To end the recording EARLY, call StopAsync(): it pushes EOS so the muxer writes the moov atom.
// Killing the process instead leaves an unplayable file.
// await pipeline.StopAsync();
```

## Use in the X engines

`VideoStabilizationBlock` implements `IVideoProcessingBlock`, so it can be inserted directly into the high-level X engines - `VideoCaptureCoreX` and `MediaPlayerCoreX` - with `Video_Processing_AddBlock()`. The engine wires the block into its video path for you, which means you can stabilize a **live camera** and not only a file.

Add the block **before the engine builds its pipeline**. For `VideoCaptureCoreX` that means before `StartAsync()`; for `MediaPlayerCoreX` it means before `OpenAsync()`, because the player builds its graph during Open - a block added after that is ignored (it only produces a warning in the log).

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Initialize the SDK once at application startup before creating any engine.
await VisioForgeX.InitSDKAsync();

if (!VideoStabilizationBlock.IsAvailable())
{
    // Add the VisioForge.CrossPlatform.OpenCV.Windows.x64 NuGet package.
    return;
}

// Enumerate the cameras.
var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
if (cameras.Length == 0)
{
    return;
}

// VideoView1 is a VisioForge VideoView control on your form/window.
var core = new VideoCaptureCoreX(VideoView1);

// This constructor picks an HD (or the best available) format and frame rate for the device.
core.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[0]);

var stabilizer = new VideoStabilizationBlock(new VideoStabilizationSettings
{
    SmoothingRadius = 20,
    CropRatio = 0.9,
});

core.Video_Processing_AddBlock(stabilizer);

// StartAsync returns false if a block failed to build - it does not throw.
if (!await core.StartAsync())
{
    // Handle the failure: the OpenCV redistributable or the camera may be unavailable.
}
```

`ApplySettings()` works exactly the same way inside an engine: change the values on `stabilizer.Settings` and call `stabilizer.ApplySettings()` to retune the effect while the camera keeps running.

!!! warning "The engine owns the block"

    Once you pass a block to `Video_Processing_AddBlock()`, the engine owns it: it builds the block on start and **disposes it when capture or playback stops**. Do not dispose it yourself, and do not reuse the same instance for a second session - create a new block for each start. `ApplySettings()` is safe to call at any time (it is a no-op once the block has been disposed).

## Live retuning

The `vfdeshake` element re-reads its properties every frame. Change any value on the `Settings` object and call `ApplySettings()` to retune the effect without rebuilding the pipeline:

```csharp
stabilizer.Settings.Enabled = false;      // instant before/after toggle
stabilizer.Settings.SmoothingRadius = 30; // steadier
stabilizer.Settings.CropRatio = 0.85;     // zoom in a little more
stabilizer.ApplySettings();
```

## Tips

- Increase `SmoothingRadius` for steadier results on continuous shake; decrease it if the stabilization reacts too slowly to intentional camera moves.
- Lower `CropRatio` (for example `0.85`) when the shake is large, so the wider compensation stays hidden behind the zoom. Higher values keep more of the frame but may expose the edges.
- Global motion estimation works best when the scene is dominated by the camera motion. Scenes with large moving foreground objects are harder to stabilize.

## Demos

- **[Video Stabilization Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Demo)** - Media Blocks pipeline: open a file, preview the stabilized playback, optionally record it to MP4, and tune the settings live.
- **[Video Stabilization Capture X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Video%20Stabilization%20Capture%20X)** - live camera stabilization with `VideoCaptureCoreX` and `Video_Processing_AddBlock()`.
- **[Video Stabilization Camera Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Video%20Stabilization%20Camera%20Demo)** - the same live camera, but built by hand as a `MediaBlocksPipeline`: camera source -> stabilizer -> renderer, with live tuning.
