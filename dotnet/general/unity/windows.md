---
title: Build a Unity Game with Video Playback for Windows x64
description: Build settings, native runtime layout, and troubleshooting for the VisioForge Media Blocks SDK .NET in Unity 6 on Windows x64 — Editor and Standalone Player.
sidebar_label: Build for Windows
order: 53
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Standalone Player
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Build for Windows

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Windows x64 is the baseline target for the Unity package — it ships in every
`.unitypackage` variant the build pipeline produces. This page covers the Player Settings, the
on-disk layout, and the issues you might hit on Windows specifically. For the rest, see
[Bootstrap and lifecycle](bootstrap.md).

## Player Settings

| Setting | Value | Where |
|---|---|---|
| Architecture | **x86_64** | File → Build Profiles → Windows |
| Target Platform | **Windows** | File → Build Profiles → Windows |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *(IL2CPP also works; Mono is the default on Windows)* | Project Settings → Player → Other Settings → Configuration |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

If you imported the package with the **Apply** dialog, the two mandatory project settings
(`.NET Standard 2.1` plus Disable Domain Reload) are already in place.

## On-disk layout

The `deploy-unity-natives.ps1` build step stages the Windows runtime into your project as
follows:

| Path | Contents |
|---|---|
| `Assets/StreamingAssets/VisioForge/x64/` | Flat layout: core GStreamer libs, every plugin DLL, GIO modules, `ca-certificates.crt`. ~300 files. |
| `Assets/Plugins/` | Managed assemblies (`VisioForge.Core.dll`, `VisioForge.Libs.dll`, `GstSharp.dll`, `GLibSharp.dll`, etc.) with their `.meta`. |
| `Assets/Scripts/` | The shared helpers: `VisioForgeEnvironment.cs`, `VisioForgeVideoView.cs`, `MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs`. |
| `Assets/Scenes/` | The two sample scenes: `SimplePlayer.unity` and `RTSPViewer.unity`. |

`StreamingAssets` (not `Plugins/x64`) is used because Unity copies the folder verbatim into a
Standalone build, which is exactly what `VisioForgeEnvironment.Configure()` then points the
DLL loader at. The same path resolves in both the Editor and the player —
`Application.streamingAssetsPath` returns the project's `Assets/StreamingAssets` in the Editor
and `<game>_Data/StreamingAssets` in the player.

## Standalone Player build

**File → Build Profiles → Windows → x86_64 → Build** produces a self-contained player. No extra
steps:

- Unity copies `Assets/StreamingAssets/VisioForge/x64/` into `<game>_Data/StreamingAssets/VisioForge/x64/` automatically.
- The managed plugins from `Assets/Plugins/` are staged into `<game>_Data/Managed/`.
- `VisioForgeEnvironment.Configure()` runs `BeforeSceneLoad` and points `SetDllDirectoryW` at the
  staged natives folder.

The resulting `<game>.exe` plus its `_Data/` folder is the entire shippable artifact.

## On-disk size

The Windows runtime adds roughly **50 MB** of native libraries to your build (down to ~30 MB if
you exclude libav with `-SkipLibav` when rebuilding the package). The managed assemblies add
another ~5 MB.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `DllNotFoundException: gstreamer-1.0-0` on Play | `Assets/StreamingAssets/VisioForge/x64/` is missing or empty. | Re-import the `.unitypackage` with all items checked, or check the `[VisioForge] Native runtime not found at …` Console line for the resolved path. |
| Pipeline hangs at startup, log shows two `gst_init` calls | A system GStreamer install on `PATH` is loading a second copy of `gstreamer-1.0-0.dll`. | `Configure()` already prunes `PATH` — confirm by inspecting the Console: stripped-count is logged. If the count is 0 but you still see the symptom, an external launcher is re-injecting GStreamer after `Configure()`. |
| `TypeLoadException` at first SDK call | Api Compatibility Level is `.NET Framework` instead of `.NET Standard 2.1`. | Set it to `.NET Standard 2.1` (Project Settings → Player → Other Settings → Configuration → Api Compatibility Level). |
| RTSPS / HTTPS streams fail to connect with TLS error | `SSL_CERT_FILE` not pointing at the bundled CA bundle. | `Configure()` sets it when `ca-certificates.crt` is present in the natives folder. A missing CA bundle is logged as a warning — re-stage the runtime via `deploy-unity-natives.ps1`. |
| Editor hangs on "Reloading domain" after Play/Stop | Disable Domain Reload was turned back on. | Re-enable it under Project Settings → Editor → Enter Play Mode Settings (set **When entering Play Mode** to a non-reloading option). |

## Frequently Asked Questions

### Can I use IL2CPP on Windows?

Yes — it compiles and runs. Mono is the default and is what the package's CI build matrix
exercises. Switch to IL2CPP only if you have a project-wide reason (other plugins that require
it, smaller deployment surface). The same `link.xml` that ships with the package preserves the
SDK's managed types from IL2CPP stripping on every backend.

### Does Windows 11 ARM64 work?

Not from this `.unitypackage`. The bundled native runtime is x86_64 only — running it under
x64 emulation on ARM64 is unsupported. An ARM64 native build is not part of the current Unity
package.

### Does the SDK need administrator rights?

No. Everything runs from the project folder / player `_Data` folder. The runtime touches no
registry keys, installs no global drivers, and writes only to `Application.persistentDataPath`
(for logs / temp files when enabled).

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — package setup
- [Bootstrap and lifecycle](bootstrap.md) — how `Configure()` brings up the Windows runtime
- [Play a media file in Unity](simple-player.md) — the `SimplePlayer` sample
- [View an RTSP camera in Unity](rtsp-viewer.md) — the `RTSPViewer` sample
- [Troubleshooting](troubleshooting.md) — common runtime errors across platforms
