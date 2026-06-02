---
title: Build a Unity App with Video Playback for iOS (device arm64)
description: Build settings, Xcode workflow, Info.plist permissions, and troubleshooting for the VisioForge Media Blocks SDK .NET in Unity 6 on iOS Standalone.
sidebar_label: Build for iOS
order: 56
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - iOS
  - IL2CPP
  - RTSP
  - Xcode
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Build for iOS

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

The iOS flavor ships as a single `GStreamerX.framework` (~68 MB on disk, arm64 device only) with every
GStreamer plugin statically registered inside the framework, plus the package's managed
assemblies built against `netstandard2.1`. There are no separate plugin files and no plugin scan
at runtime — dyld loads the framework via `@rpath` when the first `[DllImport]` fires. This page
covers the Xcode export workflow, the Info.plist entries, and the iOS-specific gotchas; for the
bring-up sequence see [Bootstrap and lifecycle](bootstrap.md).

The iOS flavor is bundled into the `.unitypackage` when the build pipeline is run with both
`-IncludeMacOS` and `-IncludeIOS`. iOS development happens on a Mac host, so the macOS flavor is
required alongside iOS — without it, the Editor on the Mac would have no native runtime to load
when opening the package. The result is one cumulative package that holds Windows, Android,
macOS, and iOS together.

## Player Settings

| Setting | Value | Where |
|---|---|---|
| Target Platform | **iOS** | File → Build Profiles → iOS |
| Target SDK | **Device SDK** | File → Build Profiles → iOS |
| Target minimum iOS Version | **15.0** or later | Project Settings → Player → Other Settings → Configuration |
| Architecture | **ARM64** | Project Settings → Player → Other Settings → Configuration |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(mandatory — Mono is not supported on iOS by Unity itself)* | Project Settings → Player → Other Settings → Configuration |
| Target Device | **iPhone + iPad** | Project Settings → Player → Other Settings → Configuration |
| Enable Bitcode | **Off** *(removed from Xcode 14+)* | Project Settings → Player → Other Settings → Configuration |

The Editor automatically applies `Api Compatibility Level = .NET Standard 2.1` for iOS when the
package's one-time setup dialog runs. If you skipped that dialog, set it manually.

## Required Info.plist entries

These are appended to the generated Xcode project's `Info.plist`. Edit them either through
Unity's **Player Settings** UI or via a post-process script in your Editor scripts:

| Key | Value | Required for |
|---|---|---|
| `NSCameraUsageDescription` | Reason you need camera access | Camera capture sources |
| `NSMicrophoneUsageDescription` | Reason you need microphone access | Microphone capture sources |
| `NSLocalNetworkUsageDescription` | Reason you need LAN access | RTSP streams on the local network (`192.168.*`, mDNS discovery) |
| `NSAppTransportSecurity` → `NSAllowsArbitraryLoads` = `YES` | (optional) | Plain `http://` URLs and self-signed `https://` / `rtsps://` certificates |

Without `NSLocalNetworkUsageDescription`, iOS 14+ silently blocks the first connection attempt to
any local-network address — `RTSPSourceBlock` then surfaces a connect timeout that looks like a
camera-side error. Set the string to something the App Store reviewers will accept (for example,
"This app streams video from local IP cameras on your network.").

If you stream only public `https://` / `rtsps://` URLs from internet hosts with valid CA-signed
certificates, you can skip the ATS exception entirely — App Transport Security accepts those by
default.

## On-disk layout

The `deploy-unity-natives.ps1 -Platform iOS` step stages the iOS runtime as:

| Path | Contents |
|---|---|
| `Assets/Plugins/iOS/GStreamerX.framework/GStreamerX` | The Mach-O arm64 binary — every GStreamer plugin statically registered. |
| `Assets/Plugins/iOS/GStreamerX.framework/Info.plist` | Framework metadata. |
| `Assets/Plugins/iOS/GStreamerX.framework/Modules/module.modulemap` | Swift / Objective-C module map. |
| `Assets/Plugins/iOS/libVisioForge_Core.a` | The SDK's iOS-flavor static archive. |
| `Assets/VisioForge/link.xml` | IL2CPP preservation rules (shared across mobile flavors). |
| `Assets/Plugins/iOS/Managed/` | Managed assemblies built against `netstandard2.1` with `UNITY_NS21_IOS` defined. |

`PluginImporter` metadata on the framework folder marks it `Add To Embedded Binaries = YES`, so
Unity embeds it into the generated Xcode project automatically. Dyld resolves
`@rpath/GStreamerX.framework/GStreamerX` when the first `[DllImport]` from the SDK fires — no
search path setup is required.

The CA bundle is **not** a separate file on iOS — it is an embedded managed resource inside
`VisioForge.Core.dll` (`VisioForge.Core.ResourcesData.ca-certificates.crt`).
`VisioForgeEnvironment.Configure()` extracts it to `Application.persistentDataPath/ssl/certs/` at
startup and points `SSL_CERT_FILE` there.

## Xcode workflow

1. **File → Build Profiles → iOS → Build** — Unity produces an Xcode project, not a final `.ipa`.
2. Open the generated `.xcworkspace` (or `.xcodeproj`) in Xcode on the same Mac.
3. **Signing & Capabilities** tab on the Unity-iPhone target — set your Apple Developer team and
   a bundle identifier you own.
4. Connect the iPhone (or simulator stub — see the Simulator note below), pick it as the Run
   target, and press **▶ Run**.

The first build takes a few minutes — Xcode is compiling IL2CPP-generated C++ into device arm64.
Incremental builds are seconds.

!!! note "Simulator is not supported"
    `GStreamerX.framework` ships device-arm64 only. The iOS Simulator (x86_64 on Intel Macs,
    arm64-sim on Apple silicon) cannot load it — Xcode aborts the build with
    `Could not find module 'GStreamerX' for target 'arm64-apple-ios-simulator'`. Test on a real
    iPhone or iPad. If you have a hard Simulator requirement, contact support.

## Build size

The iOS flavor adds roughly **40 MB** to the final `.ipa`:

- `GStreamerX.framework/GStreamerX` — ~38 MB device-arm64 binary after link-time thinning (from the ~68 MB on-disk framework)
- `libVisioForge_Core.a` — linked into the IL2CPP binary, ~2 MB delta
- Managed assemblies — ~3 MB

The compressed `.ipa` is typically smaller after App Store thinning.

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| Xcode aborts: `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX` | `Add To Embedded Binaries` was not applied to the framework PluginImporter slot. | Confirm `Assets/Plugins/iOS/GStreamerX.framework`'s `.meta` has `AddToEmbeddedBinaries: 1`. If you replaced it manually, re-import the package. |
| `UnityException: get_dataPath can only be called from the main thread` from a GStreamer callback | The first reader of `NativesPath` ran on a background thread before `Configure()`'s main-thread prime. | Confirm `Configure()` completed — it prints `[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` in the Console. If absent, the bootstrap failed before priming. |
| `MissingMethodException` from `GLib.SignalArgs` or `SignalClosure.MarshalCallback` | `link.xml` was removed or IL2CPP managed stripping is on without it. | Confirm `Assets/VisioForge/link.xml` exists. Re-import the package if missing. |
| RTSP stream times out before connecting on iOS 14+ | `NSLocalNetworkUsageDescription` is missing — iOS blocks the first LAN connection. | Add the key to `Info.plist` with a user-facing reason. |
| RTSPS / HTTPS fails with TLS error on first request | CA bundle extraction failed silently. | Check Console for `[VisioForge] CA cert extraction failed`. The embedded resource ships in `VisioForge.Core.dll` — confirm the DLL was not stripped. |
| App rejected from App Store review for "missing privacy reason" | A capture source needs `NSCameraUsageDescription` or `NSMicrophoneUsageDescription`. | Add the matching keys with user-facing reasons. |
| Crash on the second `Play` in Xcode | `VisioForgeX.DestroySDK()` was called in `OnDestroy`. | Don't call it — see [Bootstrap and lifecycle](bootstrap.md#the-editor-lifecycle). |

## Frequently Asked Questions

### Can I use Mono on iOS?

No. Unity itself does not support Mono on iOS — IL2CPP is the only backend for iOS Standalone
builds. The SDK matches that constraint.

### Does the iOS flavor work in the iOS Simulator?

No. `GStreamerX.framework` is device-arm64 only — see the note above. Test on real hardware.

### Why is the Xcode build the slow step?

IL2CPP transpiles every managed assembly (your scripts + Unity engine + the SDK) into C++, then
Xcode compiles that C++ for device arm64. The first cold build is ~3 — 5 minutes; incremental
builds are seconds because Xcode caches almost everything.

### Does the SDK upload data to VisioForge servers?

No. The SDK runs entirely in-process — no telemetry, no licensing call-home, no usage analytics.
The `NSLocalNetworkUsageDescription` requirement is purely about your app's outgoing RTSP /
HTTP connections, which iOS treats as user-visible.

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — package setup
- [Bootstrap and lifecycle](bootstrap.md) — how `Configure()` brings up the iOS runtime
- [Build for macOS](macos.md) — the matching macOS host you need to build iOS
- [View an RTSP camera in Unity](rtsp-viewer.md) — the `RTSPViewer` sample
- [Troubleshooting](troubleshooting.md) — cross-platform error reference
