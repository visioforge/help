---
title: Video Playback & RTSP in Unity with Media Blocks SDK .NET
description: Add video playback and live RTSP to Unity 6 with the VisioForge Media Blocks SDK .NET — cross-platform .unitypackage for Windows, Android, macOS, iOS.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
  - VisioForgeEnvironment
---

# Video playback and RTSP streaming in Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The **Media Blocks SDK .NET** ships a cross-platform, ready-to-import **`.unitypackage`** that
brings video-file playback, live RTSP / IP-camera streaming, and the rest of the Media Blocks
pipeline into **Unity 6**. The same package targets four platforms — **Windows x64**,
**Android (IL2CPP arm64)**, **macOS Standalone (Universal arm64+x86_64)**, and
**iOS Standalone (device arm64)**. Import once, switch Build Target, build.

To install the package, see **[Install the Media Blocks SDK in Unity](../../install/unity.md)**.
For the five-step shortcut, see **[Quickstart](getting-started.md)**.

!!! tip "AI coding agents: use the VisioForge MCP server"
    Building this with **Claude Code**, **Cursor**, or another AI coding agent? Connect to the
    public [VisioForge MCP server](../mcp-server-usage.md) at `https://mcp.visioforge.com/mcp` for
    structured API lookups and verified code examples.

!!! info "The full SDK is available — the samples are just a starting point"
    The package bundles the **complete Media Blocks SDK .NET**. The two included scenes
    (`SimplePlayer` and `RTSPViewer`) are examples to get you running quickly — your scripts have
    access to the **entire Media Blocks API**: capture and multiple source types, decoders and
    encoders, audio/video processing and effects, mixing and compositing, recording to file, and
    network streaming output. Build whatever pipeline your app needs. See the
    [Media Blocks SDK .NET documentation](../../mediablocks/index.md) for the full block catalog.

## Cumulative platform packaging

The shipped `.unitypackage` is **cumulative** — one file carries the managed assemblies plus
every native runtime, and Unity's per-file `PluginImporter` metadata picks the right copy when
you switch Build Target. There is no per-platform download.

| Platform | Native runtime | Architecture | Build profile |
|---|---|---|---|
| Windows | flat GStreamer install in `StreamingAssets/VisioForge/x64/` | x86_64 | [Build for Windows](windows.md) |
| Android | monolithic `libgstreamer_android.so` + Java AAR | arm64-v8a | [Build for Android](android.md) |
| macOS | ~300 separate dylibs in `Plugins/macOS/` | Universal arm64 + x86_64 | [Build for macOS](macos.md) |
| iOS | embedded `GStreamerX.framework` (statically registered plugins) | device arm64 | [Build for iOS](ios.md) |

All four flavors share the same managed surface — `MediaBlocksPipeline`, `BufferSinkBlock`,
`RTSPSourceBlock`, `UniversalSourceBlock`, every effect, every encoder, every sink. The only
per-platform thing your script ever sees is the `Application.platform` value at runtime.

## Samples

The package ships two ready scenes under `Assets/Scenes/`. Open one in the **Project** window
(double-click it — do not stay on the empty default scene) and press **▶ Play**:

- **[Play a media file](simple-player.md)** — the `SimplePlayer` scene, local video file playback.
- **[View an RTSP camera](rtsp-viewer.md)** — the `RTSPViewer` scene, live RTSP / IP camera stream.

!!! tip "The RawImage looks empty until you press Play"
    The video texture is created at runtime, so the `RawImage` is blank (white) in edit mode.
    That is expected — nothing is drawn until the pipeline starts.

## How rendering works

Two shared helpers handle setup and rendering for you, so each player script is just the Media
Blocks pipeline:

1. **`VisioForgeEnvironment.Configure()`** runs automatically before the first scene loads and
   prepares the bundled native runtime for the current platform — Windows DLL search path,
   macOS dylib loader hints, Android Java bootstrap, or iOS framework bring-up. You don't manage
   any native dependencies or paths. The full story is in
   [Bootstrap and lifecycle](bootstrap.md).
2. **`VisioForgeEnvironment.InitializeSdk()`** initializes the SDK once. Call it before you build
   a pipeline (the sample players call it in `Start()`).
3. Each player builds a pipeline ending in a **`BufferSinkBlock(VideoFormatX.RGBA)`**; its
   `OnVideoFrameBuffer` event hands video frames to **`VisioForgeVideoView`**.
4. **`VisioForgeVideoView`** uploads each frame into a Unity `Texture2D` on the main thread and
   applies the aspect mode, so the video shows up in your `RawImage`. You don't write any texture
   code — just attach it (the sample players do this for you).

### Editor lifecycle

The SDK initializes once per process and is reused across Play/Stop sessions in the Editor. Two
points follow from that:

- **Keep Disable Domain Reload on** so re-entering Play mode reuses the initialized SDK. With it
  off, the Editor can hang when leaving Play mode.
- The sample players dispose only the per-play pipeline on Stop (`StopAsync`) and intentionally
  **do not** shut the whole SDK down — keep that pattern in your own scripts.

If you hit instability after a script recompile, restart the Editor. See
[Bootstrap and lifecycle](bootstrap.md#the-editor-lifecycle) for the rationale.

## Frequently Asked Questions

### Do I get the full Media Blocks SDK, or only playback?

The full SDK. The two sample scenes are starting points; your scripts have access to the entire
Media Blocks SDK .NET API — capture, multiple source types, decoders and encoders, audio/video
processing and effects, mixing and compositing, recording to file, and network streaming.

### Can I render video into my own UI instead of the sample scenes?

Yes. Add a `RawImage`, attach `MediaBlocksPlayer` (file) or `RTSPViewerPlayer` (RTSP), or build
your own pipeline and feed a `BufferSinkBlock` into `VisioForgeVideoView`. The texture upload,
aspect handling, and flip are handled for you.

### Is the same package used for every platform?

Yes — one cumulative `.unitypackage` contains the Windows, Android, macOS, and iOS native
runtimes plus a single set of `netstandard2.1` managed assemblies. Unity picks the matching
slot at build time from per-file `PluginImporter` metadata; you do not import a separate package
per platform.

### Can I see which platform code path is running?

Yes. `VisioForgeEnvironment.Configure()` logs one of `[VisioForge] Environment configured.
Natives: <path>` (Windows / macOS), `[VisioForge] Android GStreamer bootstrap complete.`, or
`[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` in the Console.
Use that line to confirm which branch ran.

## See Also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — full setup, step by step
- [Quickstart](getting-started.md) — the five-step path to a playing video
- [Bootstrap and lifecycle](bootstrap.md) — what `Configure()` and `InitializeSdk()` do
- [Play a media file in Unity](simple-player.md) — the file-playback sample
- [View an RTSP camera in Unity](rtsp-viewer.md) — the live RTSP / IP camera sample
- [Build for Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Platform matrix](platform-matrix.md) — feature support by Unity platform
- [Troubleshooting](troubleshooting.md) — common errors and fixes
- [Media Blocks SDK .NET overview](../../mediablocks/index.md) — the full block catalog
- [IP camera brands directory](../../camera-brands/index.md) — tested camera URLs and settings
