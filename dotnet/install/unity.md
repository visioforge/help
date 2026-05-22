---
title: Install Media Blocks SDK in Unity 6 — Windows Setup
description: Step-by-step setup for the VisioForge Media Blocks SDK .NET in Unity 6 on Windows — import the self-contained .unitypackage, apply project settings, and run.
sidebar_label: Unity
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Install the Media Blocks SDK in Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This guide walks through installing the **Media Blocks SDK .NET** in **Unity 6** on **Windows
x64**. The SDK ships as a ready-to-import **`.unitypackage`** that is fully self-contained — you do
not build anything from source, you do not need NuGet, and there are no external dependencies to
install. After importing, open a sample scene and press **Play**.

Once it's installed, see the usage guides: [Play a media file in Unity](../general/unity/simple-player.md)
and [View an RTSP camera in Unity](../general/unity/rtsp-viewer.md).

## Requirements

| | |
|---|---|
| Unity | **6 (6000.x)** — verified on `6000.4.6f1` |
| Platform | **Windows x64** (Editor and standalone player) |

!!! warning "Use a short path on NTFS — not a Dev Drive / ReFS volume"
    Importing the package writes thousands of small native files, and Unity's import/compile is
    heavy small-file I/O. On a Dev Drive (ReFS) that is **dramatically slower** (a cold import can
    take many minutes instead of seconds) and is more prone to the `EPERM rename` race. Keep the
    project on a plain **NTFS** drive with a short root path, e.g. `C:\unity\MyApp`. Unity's
    package cache also produces deep paths that can overflow Windows' 260-character `MAX_PATH`.

## Download

Download the latest package:

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

## Step 1 — Create or open a Unity project

Use an existing Unity 6 project or create a new one (any template). Keep the project root on a
short NTFS path (see the warning above).

![Creating a Unity 6 project on a short NTFS path in Unity Hub](unity-new-project.webp)

## Step 2 — Import the package

In the Editor: **Assets → Import Package → Custom Package…**, select the downloaded
`.unitypackage`, and click **Import** (keep all items checked).

![Import Unity Package dialog showing the VisioForge package contents](unity-import-package.webp)

The package adds:

| Content | Location | Purpose |
|---|---|---|
| Managed SDK (`net48`) + dependencies | `Assets/Plugins/` | the Media Blocks SDK .NET assemblies |
| Native SDK libraries runtime incl. FFmpeg/libav | `Assets/StreamingAssets/VisioForge/x64/` | the media engine; copied verbatim into standalone builds |
| Reusable scripts | `Assets/Scripts/` | `VisioForgeEnvironment`, `VisioForgeVideoView`, and the two players |
| Two ready scenes | `Assets/Scenes/` | `SimplePlayer` (file) and `RTSPViewer` (RTSP) |
| One-time setup helper | `Assets/VisioForge/Editor/` | applies the two required project settings |

## Step 3 — Apply the required project settings

On first import the setup helper shows a dialog asking to apply two required project settings.
Click **Apply** — both settings are configured for you.

![VisioForge Media Blocks SDK setup dialog with Apply and Skip buttons](unity-apply-dialog.webp)

These two settings are **mandatory** — the SDK will not work without them:

- **Api Compatibility Level = .NET Framework** — Unity 6 defaults to `.NET Standard 2.1`, which
  cannot load this `net48` SDK build (symptom: `TypeLoadException` on play).
- **Disable Domain Reload** — the SDK initializes once per process and is reused across Play/Stop
  sessions; with Domain Reload enabled the Editor can hang when leaving Play mode.

## Step 4 — Set the settings manually (only if you clicked Skip)

If you clicked **Skip**, set both by hand:

1. **Api Compatibility Level = .NET Framework**
   *Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level*.

   ![Player settings with Api Compatibility Level set to .NET Framework](unity-apicompat.webp)

2. **Disable Domain Reload**
   *Edit → Project Settings → Editor → Enter Play Mode Settings* → set **When entering Play Mode**
   to an option that does **not** reload the domain — either **Reload Scene only** (matches what
   **Apply** does) or **Do not reload Domain or Scene**.

   ![Editor Enter Play Mode Settings with domain reload disabled](unity-domain-reload.webp)

## Step 5 — Run a sample scene

In the **Project** window open `Assets/Scenes/SimplePlayer.unity` (double-click it — do not stay
on the empty default scene), select the **RawImage** GameObject, set its **File Path** in the
Inspector, and press **▶ Play**. Video renders in the Game view and audio plays through the system
default device.

![The SimplePlayer scene playing video in the Unity Game view](unity-sample-play.webp)

!!! tip "The RawImage looks empty until you press Play"
    The video texture is created at runtime, so the `RawImage` is blank (white) in edit mode.

Next, read the usage guides:

- [Play a media file in Unity](../general/unity/simple-player.md) — the `SimplePlayer` sample.
- [View an RTSP camera in Unity](../general/unity/rtsp-viewer.md) — the `RTSPViewer` sample.

## Standalone builds

*File → Build Settings → Windows x64 → Build* produces a standalone player that works without any
extra steps: the native runtime in `Assets/StreamingAssets/VisioForge/x64` is copied verbatim by
Unity into `<game>_Data/StreamingAssets/VisioForge/x64`, and the managed assemblies in
`Assets/Plugins` are staged automatically. The same load path resolves in both the Editor and the
standalone build.

## Uninstalling or upgrading the package

A `.unitypackage` has no uninstaller — remove the files manually.

1. **Close the Unity Editor** first — it locks the native DLLs and the `Library/` cache.
2. Delete the VisioForge content from `Assets/`:
   - `Assets/StreamingAssets/VisioForge/` — the native runtime (~300 files).
   - `Assets/VisioForge/` — the one-time setup helper.
   - The four reusable scripts in `Assets/Scripts/`: `VisioForgeEnvironment.cs`,
     `VisioForgeVideoView.cs`, `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs` (plus their `.meta`)
     — keep any scripts of your own that live in the same folder.
   - The sample scenes `Assets/Scenes/SimplePlayer.unity` and `Assets/Scenes/RTSPViewer.unity`.
   - The VisioForge assemblies in `Assets/Plugins/` (`VisioForge.*.dll`, `GstSharp.dll`,
     `GLibSharp.dll`, and their dependencies, each with its `.meta`). Delete the whole
     `Assets/Plugins/` folder only if it contains nothing but VisioForge assemblies.
3. Delete the project's `Library/` folder (next to `Assets/`) to clear the cached import state.
   Unity regenerates it on the next open (the first launch is slower).

**Upgrading:** import the new `.unitypackage` over the old one — the managed-plugin GUIDs are
deterministic, so Unity overwrites the existing assets in place and references are preserved. If
you are coming from a much older package or see duplicated DLLs in `Assets/Plugins/`, do a clean
removal (steps above) first, then import the new package.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `TypeLoadException` on play | Api Compatibility Level is `.NET Standard 2.1` | Set it to `.NET Framework`, or re-import and click **Apply** |
| Editor hangs on "Reloading domain" on Play/Stop | Domain Reload is enabled | Keep Disable Domain Reload on |
| Editor crashes on the 2nd Play | The SDK was shut down on Stop and re-initialized | Don't shut the SDK down on Stop; keep Disable Domain Reload on |
| Native runtime not found | Package imported partially | Re-import the package with all items checked |
| No video, errors in the Console after import | The Editor needs a clean reload after the runtime is staged | Restart the Editor |
| Editor becomes unstable after a long editing session | Repeated script recompiles | Restart the Editor |

## Known limitations

- **Windows x64 only** — the bundled native runtime is Windows x64. Other platforms are not yet
  supported by the package.

## Frequently Asked Questions

### Can I install the SDK in Unity via NuGet?

No. Unity does not run NuGet restore, and the SDK ships ~300 native files that NuGet would not lay
out for Unity. The `.unitypackage` bundles everything — managed assemblies, native runtime, scripts,
and scenes — so you import a single file instead.

### Do I need to install GStreamer or any other system dependency?

No. The package is fully self-contained; everything the SDK needs is inside it. A system GStreamer
install on your machine is not required and is not used by the bundled runtime.

### Do other VisioForge SDKs work in Unity?

Today the Unity package ships the **Media Blocks SDK .NET** runtime, which covers playback, capture,
processing, and streaming. Other VisioForge SDKs will follow.

## See Also

- [Using VisioForge in Unity](../general/unity/index.md) — overview of how the integration works
- [Play a media file in Unity](../general/unity/simple-player.md) — the file-playback sample
- [View an RTSP camera in Unity](../general/unity/rtsp-viewer.md) — the RTSP sample
- [Media Blocks SDK .NET overview](../mediablocks/index.md) — the full block catalog
- [Installation guide](index.md) — install the SDK in other .NET project types
