---
title: Unity Video Playback & RTSP with Media Blocks SDK .NET
description: Add video playback and RTSP camera streaming to Unity 6 with the VisioForge Media Blocks SDK .NET — a self-contained, ready-to-import .unitypackage.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
---

# Video playback and RTSP streaming in Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The **Media Blocks SDK .NET** ships a ready-to-import **`.unitypackage`** that brings video file
playback and live RTSP / IP camera streaming into **Unity 6** on **Windows x64**. Import it, press
**Play**, and video renders into a Unity `RawImage`.

To install the package, see **[Install the Media Blocks SDK in Unity](../../install/unity.md)**.
This guide focuses on how the integration works and how to use the two bundled samples.

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
   prepares the bundled native runtime — you don't manage any native dependencies or paths.
2. **`VisioForgeEnvironment.InitializeSdk()`** initializes the SDK once. Call it before you build a
   pipeline (the sample players call it in `Start()`).
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

If you hit instability after a script recompile, restart the Editor.

## Frequently Asked Questions

### Do I get the full Media Blocks SDK, or only playback?

The full SDK. The two sample scenes are starting points; your scripts have access to the entire
Media Blocks SDK .NET API — capture, multiple source types, decoders and encoders, audio/video
processing and effects, mixing and compositing, recording to file, and network streaming.

### Can I render video into my own UI instead of the sample scenes?

Yes. Add a `RawImage`, attach `MediaBlocksPlayer` (file) or `RTSPViewerPlayer` (RTSP), or build
your own pipeline and feed a `BufferSinkBlock` into `VisioForgeVideoView`. The texture upload,
aspect handling, and flip are handled for you.

## See Also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — full setup, step by step
- [Play a media file in Unity](simple-player.md) — the file-playback sample
- [View an RTSP camera in Unity](rtsp-viewer.md) — the live RTSP / IP camera sample
- [Media Blocks SDK .NET overview](../../mediablocks/index.md) — the full block catalog and pipeline guide
- [RTSP streaming guide](../network-streaming/rtsp.md) — RTSP across the VisioForge .NET SDKs
- [IP camera brands directory](../../camera-brands/index.md) — tested camera URLs and settings
