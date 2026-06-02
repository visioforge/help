---
title: Install VisioForge media SDKs in Unity 6 — Setup Guide
description: Set up VisioForge media SDKs in Unity 6 — one cumulative .unitypackage for video playback, capture and editing on Windows, Android, macOS, iOS.
sidebar_label: Unity
tags:
  - Media Blocks SDK
  - Media Player SDK
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
---

# Install VisioForge media SDKs in Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

This guide walks through installing the VisioForge **media SDKs** in **Unity 6**. A single,
fully self-contained **`.unitypackage`** brings four products into Unity at once — the
**Media Blocks SDK .NET** pipeline plus the high-level **Media Player SDK .NET**
(`MediaPlayerCoreX`), **Video Capture SDK .NET** (`VideoCaptureCoreX`), and **Video Edit SDK .NET**
(`VideoEditCoreX`) engines. The package bundles every supported native runtime in one file —
Windows x64, Android, macOS Standalone, and iOS Standalone — and lets Unity pick the right one
per Build Target at build time. You do not build anything from source, you do not need NuGet, and
there are no external dependencies to install.

The package targets **`netstandard2.1`** managed assemblies. For projects pinned to the older
Mono Unity LTS, a legacy `net48` Windows-only flavor is still published — see the spoiler at the
bottom of this page.

For what's inside and how to use it, see **[Using VisioForge in Unity](../general/unity/index.md)** —
the overview with the full product and sample catalog. For the five-step shortcut, see the
**[Quickstart](../general/unity/getting-started.md)**.

## Requirements

| | |
|---|---|
| Unity | **6 (6000.x)** — verified on `6000.4.6f1` |
| Build targets shipped | **Windows x64**, **Android arm64**, **macOS Universal arm64+x86_64**, **iOS device arm64** |
| Managed TFM | **`netstandard2.1`** |
| Mandatory Editor settings | `Api Compatibility Level = .NET Standard 2.1` and Disable Domain Reload |

!!! warning "Use a short path on NTFS — not a Dev Drive / ReFS volume"
    Importing the package writes thousands of small native files, and Unity's import/compile is
    heavy small-file I/O. On a Dev Drive (ReFS) that is **dramatically slower** (a cold import can
    take many minutes instead of seconds) and is more prone to the `EPERM rename` race. Keep the
    project on a plain **NTFS** drive with a short root path, e.g. `C:\unity\MyApp`. Unity's
    package cache also produces deep paths that can overflow Windows' 260-character `MAX_PATH`.

## Download

Download the latest cumulative package — Windows + Android + macOS + iOS in one file:

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
| Managed SDK (`netstandard2.1`) + dependencies | `Assets/Plugins/` (+ `Android/`, `macOS/`, `iOS/Managed/` subfolders) | the Media Blocks SDK .NET assemblies, per platform |
| Windows native runtime | `Assets/StreamingAssets/VisioForge/x64/` | Windows GStreamer libs and plugins |
| Android native runtime | `Assets/Plugins/Android/libs/arm64-v8a/` | monolithic `libgstreamer_android.so` + Java AAR |
| macOS native runtime | `Assets/Plugins/macOS/` | universal dylibs (arm64+x86_64) |
| iOS native runtime | `Assets/Plugins/iOS/GStreamerX.framework/` | embedded framework (device arm64) |
| IL2CPP preservation | `Assets/VisioForge/link.xml` | type / member preservation for Android & iOS |
| Reusable scripts | `Assets/Scripts/` | `VisioForgeEnvironment` and `VisioForgeVideoView` helpers plus the six sample scripts |
| Six sample scenes | `Assets/Scenes/` | `SimplePlayer`, `RTSPViewer`, `MediaPlayerX`, `IPCameraX`, `VideoCaptureX`, `VideoEditX` — see the [samples overview](../general/unity/index.md#samples) |
| One-time setup helper | `Assets/VisioForge/Editor/` | applies the two required project settings |

Per-flavor `PluginImporter` metadata on every native file tells Unity which Build Target each
binary belongs to — switching the Build Target in Build Profiles automatically picks the right
slot at build time.

## Step 3 — Apply the required project settings

On first import the setup helper shows a dialog asking to apply two required project settings.
Click **Apply** — both settings are configured for you.

![VisioForge Media Blocks SDK setup dialog with Apply and Skip buttons](unity-apply-dialog.webp)

These two settings are **mandatory** — the SDK will not work without them:

- **Api Compatibility Level = .NET Standard 2.1** — the SDK ships as `netstandard2.1`
  assemblies; the legacy `.NET Framework` setting cannot load them.
- **Disable Domain Reload** — the SDK initializes once per process and is reused across
  Play/Stop sessions; with Domain Reload enabled the Editor can hang when leaving Play mode.

For mobile targets, also switch **Scripting Backend** to **IL2CPP** — Mono is not supported on
Android or iOS by Unity itself. See [Build for Android](../general/unity/android.md) and
[Build for iOS](../general/unity/ios.md) for the per-target Build Profile checklists.

## Step 4 — Set the settings manually (only if you clicked Skip)

If you clicked **Skip**, set both by hand:

1. **Api Compatibility Level = .NET Standard 2.1**
   *Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility Level*.

   ![Player settings with Api Compatibility Level set to .NET Standard 2.1](unity-apicompat.webp)

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

- [Quickstart](../general/unity/getting-started.md) — the five-step path from import to playback.
- [Using VisioForge in Unity](../general/unity/index.md) — the overview with the full product and
  sample catalog: file playback, RTSP / IP camera, webcam capture, and timeline editing.

## Build for a target platform

The cumulative `.unitypackage` contains every supported platform, but each Build Target has its
own settings and gotchas. Read the matching page:

- [Build for Windows](../general/unity/windows.md) — x86_64 Editor + Standalone Player.
- [Build for Android](../general/unity/android.md) — IL2CPP arm64, AndroidManifest permissions.
- [Build for macOS](../general/unity/macos.md) — Universal arm64+x86_64, code-signing.
- [Build for iOS](../general/unity/ios.md) — Xcode export workflow, Info.plist permissions.

## Uninstalling or upgrading the package

A `.unitypackage` has no uninstaller — remove the files manually.

1. **Close the Unity Editor** first — it locks the native DLLs and the `Library/` cache.
2. Delete the VisioForge content from `Assets/`:
   - `Assets/StreamingAssets/VisioForge/` — the Windows native runtime.
   - `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so`, `libVisioForge_Core.so`,
     and `Assets/Plugins/Android/visioforge-gstreamer.aar` — the Android runtime.
   - `Assets/Plugins/macOS/*.dylib` and `Assets/Plugins/macOS/ca-certificates.crt` — the macOS
     runtime.
   - `Assets/Plugins/iOS/GStreamerX.framework/` and `Assets/Plugins/iOS/libVisioForge_Core.a` —
     the iOS runtime.
   - `Assets/Plugins/` (with `Android/`, `macOS/`, `iOS/Managed/` subfolders) — the managed assemblies, per platform.
   - `Assets/VisioForge/` — the one-time setup helper and `link.xml`.
   - The scripts in `Assets/Scripts/`: the `VisioForgeEnvironment.cs` and `VisioForgeVideoView.cs`
     helpers plus the six sample scripts — `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs`,
     `MediaPlayerXPlayer.cs`, `IPCameraXViewer.cs`, `VideoCaptureXRecorder.cs`,
     `VideoEditXRenderer.cs` (plus their `.meta`) — keep any scripts of your own that live in the
     same folder.
   - The sample scenes in `Assets/Scenes/`: `SimplePlayer.unity`, `RTSPViewer.unity`,
     `MediaPlayerX.unity`, `IPCameraX.unity`, `VideoCaptureX.unity`, `VideoEditX.unity`.
3. Delete the project's `Library/` folder (next to `Assets/`) to clear the cached import state.
   Unity regenerates it on the next open (the first launch is slower).

**Upgrading:** import the new `.unitypackage` over the old one — the managed-plugin GUIDs are
deterministic, so Unity overwrites the existing assets in place and references are preserved. If
you are coming from a much older package or see duplicated DLLs in `Assets/Plugins/`, do a clean
removal (steps above) first, then import the new package.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `TypeLoadException` on play | Api Compatibility Level is `.NET Framework`, not `.NET Standard 2.1` | Set it to `.NET Standard 2.1`, or re-import and click **Apply** |
| Editor hangs on "Reloading domain" on Play/Stop | Domain Reload is enabled | Keep Disable Domain Reload on |
| Editor crashes on the 2nd Play | The SDK was shut down on Stop and re-initialized | Don't shut the SDK down on Stop; keep Disable Domain Reload on |
| Native runtime not found | Package imported partially, or wrong Build Target's flavor missing from the package | Re-import the package with all items checked; confirm the package contains the platform you targeted |
| No video, errors in the Console after import | The Editor needs a clean reload after the runtime is staged | Restart the Editor |
| `DllNotFoundException` on Android | Scripting Backend is Mono | Switch to IL2CPP |

For the full per-symptom reference, see [Troubleshooting](../general/unity/troubleshooting.md).

## Legacy `net48` Windows-only flavor

??? note "I have an older Unity LTS pinned to Mono — what about the net48 build?"

    The original Windows-only build of the package targets **`.NET Framework 4.8`** managed
    assemblies and is still produced for projects that cannot move to `.NET Standard 2.1` (for
    example, Unity 2019.4 LTS without the modern Api Compatibility option). It ships as a
    separate `.unitypackage` with `NET48` in the filename, contains only the Windows-x64 native
    runtime, and uses `.NET Framework` as the Api Compatibility Level. New projects should use
    the `netstandard2.1` package described above — it covers the same Windows-x64 use case plus
    every other platform, and Unity 6 defaults to it. If you have a hard requirement for the
    `net48` build, contact support for the latest download link.

## Frequently Asked Questions

### Can I install the SDK in Unity via NuGet?

No. Unity does not run NuGet restore, and the SDK ships hundreds of native files that NuGet
would not lay out for Unity. The `.unitypackage` bundles everything — managed assemblies, every
platform's native runtime, scripts, and scenes — so you import a single file instead.

### Do I need to install GStreamer or any other system dependency?

No. The package is fully self-contained; everything the SDK needs is inside it. A system
GStreamer install on your machine is not required and is not used by the bundled runtime — on
the contrary, `VisioForgeEnvironment.Configure()` actively prunes any system GStreamer from the
process search path to avoid a double-init.

### Which VisioForge SDKs are included?

The package ships four products from one shared `netstandard2.1` managed surface: the
**Media Blocks SDK .NET** pipeline and the high-level **Media Player SDK .NET**
(`MediaPlayerCoreX`), **Video Capture SDK .NET** (`VideoCaptureCoreX`), and **Video Edit SDK .NET**
(`VideoEditCoreX`) engines. Each ships one or more ready sample scenes — see the
[samples overview](../general/unity/index.md#samples).

### Does the same package work on Windows ARM64?

The package's Windows native runtime is x86_64 only — there is no ARM64 native build today.
Run it via x64 emulation only at your own risk; production use on Windows 11 ARM64 is not
exercised.

### Can I open the same package in the Mac-host Editor?

Yes — if the package was built with `-IncludeMacOS`. The cumulative variant published at
`files.visioforge.com/unity/` always contains the macOS flavor. A Windows-only variant opened in
a Mac Editor surfaces a clear `[VisioForge] Native runtime folder not found at '…' for runtime
platform OSXEditor` message; see [Bootstrap and lifecycle](../general/unity/bootstrap.md).

## See Also

- [Using VisioForge in Unity](../general/unity/index.md) — overview of how the integration works
- [Quickstart](../general/unity/getting-started.md) — five-step path to a playing video
- [Bootstrap and lifecycle](../general/unity/bootstrap.md) — what `Configure()` and
  `InitializeSdk()` do
- [Play a media file in Unity](../general/unity/simple-player.md) — the file-playback sample
- [View an RTSP camera in Unity](../general/unity/rtsp-viewer.md) — the RTSP sample
- [Capture a webcam](../general/unity/video-capture-x.md) · [Edit and render](../general/unity/video-edit-x.md) — the CoreX engine samples
- [Platform matrix](../general/unity/platform-matrix.md) — feature support by Unity platform
- [Media Blocks SDK .NET overview](../mediablocks/index.md) — the full block catalog
- [Installation guide](index.md) — install the SDK in other .NET project types
