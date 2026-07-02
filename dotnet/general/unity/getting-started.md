---
title: Get Started with Media Blocks SDK in Unity 6 — Quickstart
description: Five-step quickstart from a fresh Unity 6 project to a playing video — import the VisioForge .unitypackage, apply Player Settings, run the sample.
sidebar_label: Getting started
order: 51
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Quickstart
  - Windows
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - BufferSinkBlock
---

# Getting started

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

This page is the five-step path from a fresh Unity 6 project to a playing video. For installation
details, deep platform notes, and the bootstrap explanation, follow the links at the end.

## What you need

- **Unity 6 LTS** (`6000.x`) — verified on `6000.4.6f1`. Older Unity LTS (2022 / 2023) may also work (untested),
  provided they offer `.NET Standard 2.1` as an Api Compatibility Level option.
- **A short NTFS project path** on Windows — for example, `C:\unity\MyApp`. Avoid Dev Drive
  (ReFS) and very long paths; Unity's package cache can overflow Windows' 260-character
  `MAX_PATH`.
- For the mobile / Apple targets, the matching Unity module installed via Unity Hub
  (Android Build Support, iOS Build Support, macOS Build Support).

## Step 1 — Download the cumulative `.unitypackage`

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

The package is self-contained — managed assemblies, every supported native runtime, sample
scenes, and the one-time setup helper are all inside it. No NuGet restore, no external GStreamer
install, no per-platform downloads.

## Step 2 — Import the package

In Unity: **Assets → Import Package → Custom Package…**, select the downloaded `.unitypackage`,
and click **Import** with all items checked.

The published cumulative package adds all four platform runtimes (a custom private build only includes the platforms its `-Include*` switches opted into):

- `Assets/StreamingAssets/VisioForge/x64/` — Windows native runtime
- `Assets/Plugins/Android/` — Android native runtime
- `Assets/Plugins/macOS/` — macOS native runtime
- `Assets/Plugins/iOS/GStreamerX.framework/` — iOS native runtime
- `Assets/Plugins/` — managed SDK assemblies
- `Assets/Scripts/` — the shared `VisioForgeEnvironment`, `VisioForgeVideoView`, and two sample
  player components
- `Assets/Scenes/SimplePlayer.unity` and `Assets/Scenes/RTSPViewer.unity` — sample scenes
- `Assets/VisioForge/` — the one-time setup helper and (on mobile / macOS builds)
  `link.xml`

## Step 3 — Apply the project settings

On first import the setup helper offers to apply the mandatory setting. Click **Apply** and it
is configured for you:

| Setting | Value | Why |
|---|---|---|
| Api Compatibility Level | `.NET Standard 2.1` | The SDK ships as `netstandard2.1` assemblies. The legacy `.NET Framework` setting cannot load them. |

Unity's default **Enter Play Mode** behavior (Domain + Scene Reload) is fully supported — you do
**not** need to disable Domain Reload. The SDK survives the Editor's Play/Stop reloads.

If you click **Skip**, set the Api Compatibility Level manually under **Edit → Project Settings
→ Player → Other Settings → Configuration → Api Compatibility Level**.

For mobile targets (Android, iOS), set **Scripting Backend = IL2CPP** in the same Configuration
section. Mono is not supported on Android or iOS by Unity itself.

## Step 4 — Run the sample scene

1. In the **Project** window open `Assets/Scenes/SimplePlayer.unity` (double-click — do not stay
   on the empty default scene).
2. Select the **RawImage** GameObject in the **Hierarchy**.
3. In the **Inspector** set **File Path** to an absolute path to a local media file.
4. Press **▶ Play**.

Video renders in the Game view; audio plays through the system default device.

If you have a local RTSP camera, open `Assets/Scenes/RTSPViewer.unity` instead, set **Rtsp Url**
(plus **Login** / **Password** if the camera requires authentication), and press **Play**.

## Step 5 — Adapt to your own scene

You do not have to use the sample scenes. To play a video into your own UI:

1. Add a **Canvas → Raw Image** (*GameObject → UI → Raw Image*).
2. Select the **Raw Image** and **Add Component →** `MediaBlocksPlayer` (or
   `RTSPViewerPlayer` for RTSP).
3. Set the **File Path** (or **Rtsp Url**) field and press **▶ Play**.

The aspect handling, vertical flip, and `Texture2D` upload are handled by the bundled
`VisioForgeVideoView`. Your script is just the pipeline — see
[Play a media file in Unity](simple-player.md) for the C# breakdown.

## Build for a target platform

When you are ready to ship:

- [Windows x64](windows.md) — Editor and Standalone Player baseline.
- [Android](android.md) — IL2CPP arm64, AndroidManifest permissions, build size notes.
- [macOS](macos.md) — Universal arm64+x86_64, code-signing and notarization.
- [iOS](ios.md) — Xcode export, Info.plist permissions, IL2CPP arm64 device only.

The cumulative `.unitypackage` contains every platform you opted into when it was built; Unity
picks the right runtime per Build Target via the per-file `PluginImporter` metadata.

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — installation reference
- [Using VisioForge in Unity](index.md) — architecture and rendering overview
- [Bootstrap and lifecycle](bootstrap.md) — what `Configure()` and `InitializeSdk()` do
- [Platform matrix](platform-matrix.md) — feature availability by platform
- [Troubleshooting](troubleshooting.md) — common errors and fixes
