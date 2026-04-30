---
title: VisioForge Media Blocks SDK in C# .NET — Cheat Sheet
description: One-page Media Blocks SDK reference with NuGet packages, pipeline APIs, a canonical example, platform support, and common pitfalls.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - Playback
  - Capture
  - Recording
  - Streaming
  - Encoding
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
  - SystemVideoSourceBlock
  - H264EncoderBlock
  - MP4SinkBlock
  - DeviceEnumerator
---

# VisioForge Media Blocks SDK in C# .NET — Cheat Sheet

Media Blocks SDK is the most flexible VisioForge .NET SDK — build arbitrary media pipelines by composing blocks (sources, encoders, renderers, sinks). Choose Media Blocks when you need custom pipelines that the higher-level Media Player / Video Capture / Video Edit SDKs can't express.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Platform support

- Runs on Windows (x64 / x86), macOS, Linux x64, Android, and iOS.
- UI frameworks: WinForms, WPF, MAUI, Avalonia, Uno, plus console.
- Cross-platform processing is powered by a bundled GStreamer backend on macOS/Android/iOS and by the system GStreamer 1.22+ install on Linux.
- For the full codec × platform matrix, see [platform-matrix.md](../platform-matrix.md).

## NuGet packages

Main SDK package (all platforms):

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Windows x64 runtime (pick x86 for 32-bit apps):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<!-- Optional: extra decoders/encoders via Libav -->
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Windows x86:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86.UPX" Version="2025.4.9" />
```

macOS (native and MAUI / macCatalyst):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.4.9" />
```

Linux x64 (also requires GStreamer 1.22+ installed on the system):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.4.9" />
```

Android and iOS:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.4.9" />
```

UI integration packages (optional, per framework):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.4.9" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2025.4.9" />
```

Full install walkthrough: [install/index.md](../install/index.md).

## Primary API classes

| Class | Role | See also |
| --- | --- | --- |
| `MediaBlocksPipeline` | Root pipeline object. Connects blocks via `Connect(output, input)`. Exposes `StartAsync` / `StopAsync` / `DisposeAsync` and the `OnError`, `OnStart`, `OnStop` events. | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `UniversalSourceBlock` | Opens a file, URL, or stream as input. Analyzes streams automatically and exposes `VideoOutput` / `AudioOutput` pads. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `VideoRendererBlock` | Binds pipeline video output to an `IVideoView` control (WinForms / WPF / MAUI / Avalonia). Supports snapshots. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `SystemVideoSourceBlock` | Webcam / USB / built-in camera input. Configured via `VideoCaptureDeviceSourceSettings`. | [GettingStarted/camera.md](./GettingStarted/camera.md) |
| `H264EncoderBlock` | H.264 encoder with software and hardware (NVENC / AMF / Quick Sync) backends. | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `MP4SinkBlock` | Writes encoded video + audio to an `.mp4` file. Add inputs via `IMediaBlockDynamicInputs.CreateNewInput`. | [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md) |
| `DeviceEnumerator` | Lists cameras, microphones, and audio outputs asynchronously via `DeviceEnumerator.Shared.VideoSourcesAsync()` etc. | [GettingStarted/device-enum.md](./GettingStarted/device-enum.md) |

## Canonical minimum example

The simplest useful pipeline — load a file, render video + audio, clean up. Lift and adapt from [GettingStarted/player.md](./GettingStarted/player.md).

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// 1. Initialize SDK once at application startup
await VisioForgeX.InitSDKAsync();

// 2. Create the pipeline and subscribe to errors
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

// 3. Source: open a media file (URL or local path via URI)
var sourceSettings = await UniversalSourceSettings.CreateAsync(
    new Uri("file:///C:/Videos/sample.mp4"));
var fileSource = new UniversalSourceBlock(sourceSettings);

// 4. Video renderer — VideoView1 is an IVideoView on your form/page
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// 5. Audio renderer — pick the first system audio output
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var audioRenderer = new AudioRendererBlock(audioOutputs[0]);

// 6. Connect output pads → input pads
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);

// 7. Play
await pipeline.StartAsync();

// ... later, on user stop / app exit:
await pipeline.StopAsync();
await pipeline.DisposeAsync();
VisioForgeX.DestroySDK(); // sync only — no async variant
```

Swap `UniversalSourceBlock` for `SystemVideoSourceBlock` (camera) or `RTSPSourceBlock` (IP camera) without changing the rest of the wiring.

## Typical workflow

1. Init SDK — `await VisioForgeX.InitSDKAsync()`.
2. Create `MediaBlocksPipeline` and subscribe to `OnError`.
3. Instantiate source blocks (`UniversalSourceBlock`, `SystemVideoSourceBlock`, `RTSPSourceBlock`, …).
4. Instantiate processing blocks — encoders, mixers, overlays, effects (optional).
5. Instantiate sink / renderer blocks (`VideoRendererBlock`, `AudioRendererBlock`, `MP4SinkBlock`, …).
6. Wire blocks with `pipeline.Connect(output, input)` — every data path must be connected explicitly.
7. `StartAsync` → `StopAsync` → `DisposeAsync`, then `DestroySDK` at app exit.

## Common pitfalls

- **No implicit dataflow.** Blocks must be explicitly wired with `pipeline.Connect(output, input)`; adding a block to the pipeline alone does not route media through it.
- **Overlay / dynamic-input order.** Overlays on `OverlayManagerBlock` and dynamic pads on sinks (`MP4SinkBlock` via `IMediaBlockDynamicInputs.CreateNewInput`) must be added before the pipeline starts — adding them after `StartAsync` silently fails or throws.
- **Awaited enumeration only.** `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioOutputsAsync()` must be awaited — there is no synchronous variant. Using `.Result` from a UI thread can deadlock.
- **Linux needs system GStreamer.** `VisioForge.CrossPlatform.Core.Linux.x64` expects GStreamer 1.22+ already installed (`apt install gstreamer1.0-*`); the Libav NuGet is supplementary, not a replacement. See [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md).
- **Lifecycle hygiene.** Always `await pipeline.StopAsync()` and `await pipeline.DisposeAsync()` before creating another pipeline on the same engine. Skipping dispose leaks native handles and can hang on shutdown.

## See also

- **Getting started**
    - Video file player — [GettingStarted/player.md](./GettingStarted/player.md)
    - Camera viewer — [GettingStarted/camera.md](./GettingStarted/camera.md)
    - Complete pipeline walkthrough — [GettingStarted/pipeline.md](./GettingStarted/pipeline.md)
    - Device enumeration — [GettingStarted/device-enum.md](./GettingStarted/device-enum.md)
- **Specific pipelines**
    - RTSP / IP camera player — [Guides/rtsp-player-csharp.md](./Guides/rtsp-player-csharp.md)
    - Save RTSP stream (passthrough) — [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md)
    - Multi-camera grid — [Guides/multi-camera-rtsp-grid.md](./Guides/multi-camera-rtsp-grid.md)
- **Deployment**
    - Windows — [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS — [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Ubuntu — [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android — [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS — [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Install & platform matrix** — [../install/index.md](../install/index.md), [../platform-matrix.md](../platform-matrix.md)

## FAQ

### How does Media Blocks differ from Media Player SDK?

Media Player SDK is a high-level, single-class player (`MediaPlayerCoreX`) with built-in playback controls — optimal when you just need to play a file or stream. Media Blocks exposes the underlying pipeline so you can insert encoders, effects, multi-output sinks, or custom processing. If you find yourself fighting Media Player SDK to add a step it doesn't expose, switch to Media Blocks.

### Can I run on Linux without GStreamer?

No. The Linux x64 runtime NuGet is a thin bridge to the system GStreamer 1.22+ stack. Install `gstreamer1.0-plugins-base`, `-good`, `-bad`, `-ugly`, and `-libav` from your distro. See [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md) for the full package list. macOS / Android / iOS do ship a bundled GStreamer via the platform NuGet.

### How do I add a custom video effect?

Use `GLShaderBlock` for a custom GLSL fragment shader (GPU-accelerated) or the pre-built `GL*Block` family (e.g. `GLBlurBlock`, `GLColorBalanceBlock`) for common effects. For CV-based processing, use the `CV*Block` family (e.g. `CVFaceDetectBlock`, `CVEdgeDetectBlock`, `CVDewarpBlock`). All slot between a decoder and renderer like any other block. Full recipe: [Guides/custom-video-effects-csharp.md](./Guides/custom-video-effects-csharp.md).
