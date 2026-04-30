---
title: VisioForge Video Edit SDK in C# .NET — Cheat Sheet
description: One-page Video Edit SDK reference with NuGet packages, VideoEditCoreX APIs, a timeline example, platform support, and pitfalls.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - VideoEditCore
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
  - DirectShow
  - Editing
  - Encoding
  - Effects
  - Mixing
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoEditCore
  - MP4Output
  - VideoSource
  - VideoFileSource
---

# VisioForge Video Edit SDK .Net — Cheat Sheet

Video Edit SDK assembles timelines, applies transitions/effects/overlays, and exports to MP4/MKV/WebM. `VideoEditCoreX` is the cross-platform engine (GStreamer); `VideoEditCore` is the Windows-only DirectShow legacy engine. Choose `VideoEditCoreX` for new projects.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Platform support

- `VideoEditCoreX` (cross-platform, GStreamer): Windows, macOS (Intel + Apple Silicon), Linux (Ubuntu/Debian/CentOS), Android (via MAUI), iOS (via MAUI).
- `VideoEditCore` (legacy, DirectShow): Windows x64 only.
- UI frameworks: WinForms, WPF, MAUI, Avalonia, Uno, Console.
- Full engine × platform × UI matrix: [platform-matrix.md](../platform-matrix.md).

## NuGet packages

Cross-platform engine (recommended for new projects):

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEditX" Version="*" />
```

Legacy Windows-only engine:

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="*" />
```

UI integration (pick the framework you target):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="*" />
```

Platform-specific native redistributables follow the same pattern as other VisioForge SDKs (per-OS redist packages pull in the GStreamer natives). See the [installation guide](../install/index.md) for the full list.

## Primary API classes

| Class | Role | See also |
|---|---|---|
| `VideoEditCoreX` | Cross-platform timeline engine (GStreamer-backed) | [getting-started.md](./getting-started.md) |
| `VideoEditCore` | Legacy Windows-only engine (DirectShow) — keep existing apps running | [getting-started.md](./getting-started.md) |
| `MP4Output` | Encoded MP4 output configuration (H.264/H.265 + AAC) | [getting-started.md](./getting-started.md) |
| `GIFOutput` | Animated GIF output (video-only, no audio, 256-color palette) — `gifenc` writes the file directly with no container | [getting-started.md](./getting-started.md#rendering-to-animated-gif) |
| `VideoSource` / `VideoFileSource` | Per-segment video input descriptor (`VideoSource` for `VideoEditCore`, `VideoFileSource` for `VideoEditCoreX`) | [code-samples/several-segments.md](./code-samples/several-segments.md) |
| `AudioFileSource` | File-backed audio input for mixing into the timeline | [code-samples/volume-for-track.md](./code-samples/volume-for-track.md) |

## Canonical minimum example

Two-clip timeline merge, H.264 MP4 output, cross-platform `VideoEditCoreX`:

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// 1. Initialize SDK (cross-platform engines require this).
await VisioForgeX.InitSDKAsync();

// 2. Create editor. Pass an IVideoView for preview, or null for headless/console.
var editor = new VideoEditCoreX(videoView as IVideoView);

// 3. Wire up events.
editor.OnError    += (s, e) => Console.WriteLine($"Error: {e.Message}");
editor.OnProgress += (s, e) => Console.WriteLine($"Progress: {e.Progress}%");
editor.OnStop     += (s, e) => Console.WriteLine("Editing completed");

// 4. Set timeline output parameters BEFORE adding sources.
editor.Output_VideoSize      = new Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// 5. Add sources to the timeline (audio + video in one call).
editor.Input_AddAudioVideoFile("/abs/path/intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("/abs/path/main.mp4",  null, null, null);

// 6. Configure MP4 output.
editor.Output_Format = new MP4Output("/abs/path/output.mp4");

// 7. Start processing. OnStop fires when rendering finishes.
editor.Start();

// ... inside your OnStop handler, or after awaiting a completion signal:
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK(); // sync only — no async variant
```

For trimming, use `Input_AddVideoFile` with a `VideoFileSource(filename, startTime, stopTime, streamNumber, rate)` instead — the `startTime`/`stopTime` clip the input range.

## Typical workflow

1. Init SDK: `await VisioForgeX.InitSDKAsync()` (cross-platform engine only — `VideoEditCore` legacy skips this).
2. Instantiate `VideoEditCoreX` (or `VideoEditCore` for Windows-legacy), passing an `IVideoView` or `null` for headless.
3. Set output resolution, framerate, encoder (`Output_VideoSize`, `Output_VideoFrameRate`).
4. Add input sources (files, segments, images, audio) — **before** `Start`.
5. Set output file via `MP4Output` (or `MKVOutput` / `WebMOutput` / `GIFOutput` for animated GIF) on `Output_Format`.
6. `Start()` (CoreX) or `await StartAsync()` (legacy) → wait for `OnStop` → `Dispose()` + `DestroySDK()`.

## Common pitfalls

- **Set timeline output resolution / framerate before adding sources.** Changing `Output_VideoSize` or `Output_VideoFrameRate` after `Input_Add*` can silently reconfigure the pipeline or drift sync.
- **Don't mix engine APIs.** `VideoEditCore` uses `VideoSource` + `Input_AddVideoFileAsync`; `VideoEditCoreX` uses `VideoFileSource` + `Input_AddVideoFile` (synchronous). The type namespaces also differ (`Types.VideoEdit` vs `Types.X.VideoEdit`).
- **Init only applies to the X engine.** `VideoEditCoreX` requires `VisioForgeX.InitSDKAsync()` / `DestroySDK()`. `VideoEditCore` (legacy) does not — calling InitSDK for legacy-only apps is unnecessary.
- **Use absolute file paths.** Relative paths resolve against the process CWD and behave inconsistently across platforms (especially MAUI/iOS/Android where CWD is not your project folder).
- **`Start` is non-blocking; wait on `OnStop`.** Don't assume rendering is complete when `Start()` returns. Use `OnStop` (or `await`-wrap it) to trigger post-processing, then `Dispose()` and `DestroySDK()`.

## See also

- **Getting started**
    - [Getting Started Guide](./getting-started.md) — full walkthrough for both engines.
    - [Code Samples index](./code-samples/index.md) — overlays, transitions, audio, composition.
- **Specific tasks**
    - Merge / concatenate clips → [output-file-from-multiple-sources.md](./code-samples/output-file-from-multiple-sources.md)
    - Trim / multi-segment from one file → [several-segments.md](./code-samples/several-segments.md)
    - Transitions between clips → [transition-video.md](./code-samples/transition-video.md), [transitions reference](./transitions.md)
    - Text overlay → [add-text-overlay.md](./code-samples/add-text-overlay.md)
    - Image / logo overlay → [add-image-overlay.md](./code-samples/add-image-overlay.md)
    - Picture-in-picture → [picture-in-picture.md](./code-samples/picture-in-picture.md)
    - Slideshow from images → [video-images-console.md](./code-samples/video-images-console.md)
    - Audio mixing / volume envelope → [audio-envelope.md](./code-samples/audio-envelope.md), [volume-for-track.md](./code-samples/volume-for-track.md)
    - iOS editor app → [ios-video-editor.md](./code-samples/ios-video-editor.md)
- **Deployment** — [Windows / macOS / Ubuntu / Android / iOS](../deployment-x/index.md)
- **Install & matrix** — [Installation guide](../install/index.md) · [Platform matrix](../platform-matrix.md)

## FAQ

### `VideoEditCoreX` vs `VideoEditCore` — which do I use?

Use `VideoEditCoreX` for new projects. It runs on Windows, macOS, Linux, Android, and iOS on a GStreamer backend. Use `VideoEditCore` only to keep existing Windows DirectShow apps running.

### Can I add transitions between clips?

Yes. `VideoEditCoreX` supports 100+ SMPTE wipe transitions between consecutive segments — see [transition-video.md](./code-samples/transition-video.md) and the [transitions reference](./transitions.md).

### Does it run on macOS / Linux?

Yes — `VideoEditCoreX` runs on macOS (Intel + Apple Silicon) and Linux (Ubuntu/Debian/CentOS) via the bundled GStreamer natives. `VideoEditCore` is Windows-only.

### How do I trim a file without re-adding it multiple times?

Pass a `FileSegment[]` array to the `VideoFileSource` constructor — each segment becomes a clipped range from the same source on the output timeline.
