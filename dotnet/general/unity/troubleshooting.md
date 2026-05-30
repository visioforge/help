---
title: Troubleshoot the Media Blocks SDK in Unity — Common Errors
description: Common errors and fixes for the VisioForge Media Blocks SDK .NET in Unity 6 — bootstrap, missing natives, IL2CPP, TLS and Editor lifecycle.
sidebar_label: Troubleshooting
order: 60
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Troubleshooting
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Troubleshooting

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This page collects the symptoms you are most likely to hit and the root cause for each. Errors
are grouped by category. The per-platform pages
([Windows](windows.md), [Android](android.md), [macOS](macos.md), [iOS](ios.md)) also have a
target-specific Troubleshooting table — check both.

## Bootstrap and initialization

### `[VisioForge] Native runtime not found at <path>`

`VisioForgeEnvironment.Configure()` could not find the bundled natives folder on disk. Causes:

- The `.unitypackage` import was partial. Re-import with all items checked.
- On Standalone macOS, the Build Target did not include the macOS flavor — the package was
  built without `-IncludeMacOS`. Re-build the package or import the cumulative variant.
- On Android, the per-Build-Target flavor staging step did not run. Open
  `Assets/Plugins/Android/libs/arm64-v8a/` in the Project window and confirm
  `libgstreamer_android.so` is present.

### `[VisioForge] InitializeSdk() called before Configure() succeeded`

A platform branch of `Configure()` failed and left `s_envConfigured = false`. The earlier
Console line (`[VisioForge] Android GStreamer init failed: …`,
`[VisioForge] SetDllDirectoryW failed (Win32 error …)`, etc.) explains why. Fix the underlying
issue and let `Configure()` retry on the next scene load.

### `UnityException: get_dataPath can only be called from the main thread`

A background thread inside the SDK or your script read `Application.dataPath` (or
`Application.streamingAssetsPath`, or `Application.platform`) without going through the cached
path. The fix:

- On iOS, `Configure()` primes `s_cachedNativesPath` on the main thread — confirm the Console
  shows `[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).`. If absent,
  the bootstrap aborted before priming and the next reader hits the lazy path off the main
  thread.
- In your own code, do not call Unity API from a `Task.Run`, GStreamer pad-added callback, or
  bus signal handler. Marshal the call back to the main thread with `UnitySynchronizationContext`
  or by setting a flag the `Update()` method checks.

### `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available`

The Android Java bootstrap could not get a non-null `currentActivity` at `BeforeSceneLoad`.
Happens on Wear OS, Android TV variants without `UnityPlayerActivity`, and Unity-as-a-library
hosts that have not assigned the field yet. Defer `Configure()` to your first observable Activity
event:

```csharp
private void Start()
{
    if (!VisioForgeIsConfigured())
        VisioForgeEnvironment.Configure();   // re-run after Activity is ready
    VisioForgeEnvironment.InitializeSdk();
}
```

`Configure()` is idempotent — a redundant call after a successful one is harmless.

## Missing libraries

### `DllNotFoundException: gstreamer-1.0-0`

Windows: the natives folder is missing from `Assets/StreamingAssets/VisioForge/x64/`. Re-import
the package. If you are running a Standalone build, confirm `<game>_Data/StreamingAssets/VisioForge/x64/` is also populated — Unity copies it verbatim, so a missing folder in the build
means it was missing in the project.

### `DllNotFoundException: libgstreamer_android`

Android: Scripting Backend is set to **Mono**. Switch to **IL2CPP** under Project Settings →
Player → Other Settings → Configuration. Mono is not supported on Android by Unity itself.

### `DllNotFoundException: libgstreamer-1.0.0` *(macOS)*

The `.app` bundle does not contain `libgstreamer-1.0.0.dylib` at any of the probed locations
(`Contents/PlugIns`, `Contents/PlugIns/macOS`, `Contents/Frameworks`,
`Contents/Resources/Data/Plugins/macOS`). Re-build the package with `-IncludeMacOS` and re-export
the Standalone build.

### `dyld: Library not loaded: @rpath/GStreamerX.framework/GStreamerX`

iOS: the framework's PluginImporter slot did not get **Add To Embedded Binaries = YES**. Re-import
the package; the package's `.meta` file marks the framework correctly. If you replaced the
framework manually, restore the `.meta` from a fresh import.

## IL2CPP / managed stripping

### `MissingMethodException: GLib.SignalArgs..ctor` *(or similar GStreamer / GLib types)*

IL2CPP stripped a managed type the SDK references through reflection. `Assets/VisioForge/link.xml` preserves these types — confirm the file exists in your project. If you accidentally
deleted it, re-import the `.unitypackage`. Do **not** edit `link.xml` to remove rules; the
package ships a tested set.

### `MarshalDirectiveException` at the first SDK call

A `[DllImport]` signature was stripped or its delegate marshalling failed. Same root cause as
`MissingMethodException` — confirm `link.xml` is in place and IL2CPP is not configured with an
extra-aggressive stripping level that overrides it.

## TLS / network

### RTSP stream times out before connecting *(iOS 14+)*

iOS blocks the first connection attempt to any local-network address until your `Info.plist`
declares why your app needs LAN access. Add:

```xml
<key>NSLocalNetworkUsageDescription</key>
<string>This app streams video from local IP cameras on your network.</string>
```

App Store reviewers expect a user-facing reason, not boilerplate.

### RTSPS / HTTPS fails with TLS / certificate verification error

GIO's OpenSSL TLS backend could not find a CA bundle:

- Windows / macOS: `Configure()` sets `SSL_CERT_FILE` to the bundled `ca-certificates.crt`. If
  missing, the Console logs `[VisioForge] CA certificate bundle not found at …`. Re-stage the
  natives via `deploy-unity-natives.ps1` and re-build.
- Android / iOS: the CA bundle is an embedded managed resource extracted to
  `<filesDir>/ssl/certs/`. If extraction fails, the Console logs `[VisioForge] CA cert
  extraction failed`. Confirm `VisioForge.Core.dll` is in `Assets/Plugins/Android/` (Android) or `Assets/Plugins/iOS/Managed/` (iOS).

For self-signed certificates, either install them into the system trust store (out of scope for
the SDK) or — for testing only — disable peer verification on the source block. The block-level
property name varies; see [View an RTSP camera in Unity](rtsp-viewer.md) for the RTSP example.

### Plain `http://` URLs fail on iOS

App Transport Security (ATS) blocks `http://` by default. Either move to `https://` or, if you
must keep `http://`, add `NSAppTransportSecurity → NSAllowsArbitraryLoads = YES` to your
`Info.plist`. Be aware that App Store reviewers may ask why you need it.

## Editor lifecycle

### Editor hangs on "Reloading domain" after Play / Stop

Disable Domain Reload is off. Re-enable it under Project Settings → Editor → Enter Play Mode
Settings → set **When entering Play Mode** to **Reload Scene only** or **Do not reload Domain
or Scene**. The package's one-time setup dialog configures this for you; if you clicked **Skip**,
set it manually.

### Editor crashes on the second Play

The SDK was shut down on Stop and re-initialized on Play. The fix:

- Do not call `VisioForgeX.DestroySDK()` in `OnDestroy` or anywhere else. The SDK is
  process-global and reused across Play sessions.
- The sample players (`MediaBlocksPlayer`, `RTSPViewerPlayer`) follow this pattern — copy their
  teardown shape (dispose only the per-play pipeline; never destroy the SDK).

See [Bootstrap and lifecycle](bootstrap.md#the-editor-lifecycle) for the full explanation.

### Editor becomes unstable after a long editing session

Repeated script recompiles accumulate GStreamer state across domain reloads. Restart the Editor
to recover. This is mostly cosmetic — Standalone Player builds do not exhibit it.

## Build / packaging

### Bundle launched from Finder shows "Damaged app" *(macOS)*

The `.app` is unsigned and downloaded with the quarantine flag set. For distribution, sign and
notarize the bundle (see [Build for macOS](macos.md#code-signing-and-notarization)). For local
testing only, run `xattr -d com.apple.quarantine <app-bundle>` once.

### App rejected from App Store review for "missing privacy reason" *(iOS)*

A capture source needs an explicit `Info.plist` key:

- `NSCameraUsageDescription` if your app captures from the camera
- `NSMicrophoneUsageDescription` if your app captures from the microphone
- `NSLocalNetworkUsageDescription` if your app talks to a LAN device

The user-facing string should describe the actual use, not "Required by SDK".

### `[VisioForge] Native runtime folder not found at '…' for runtime platform …`

The `.unitypackage` you imported does not contain a flavor for the current Build Target. For
example, a Windows-only package opened in a Mac-host Editor surfaces this on the first
`InitializeSdk()` call. Re-build (or re-download) the package with the matching `-Include*`
switch, or import the cumulative variant that contains every platform.

## When all else fails

Collect a log and contact support:

1. Editor or Player Console export (`Window → General → Console`, right-click → Save All / via
   Logcat / via Xcode → Devices → Open Console).
2. The `.unitypackage` file name and its build date.
3. Unity version, Build Target, Scripting Backend, Api Compatibility Level.
4. A minimal reproducible scene if possible.

Open an issue at <https://support.visioforge.com/>.

## See also

- [Bootstrap and lifecycle](bootstrap.md) — how the runtime is brought up on each platform
- [Build for Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md) —
  per-platform troubleshooting tables
- [Platform matrix](platform-matrix.md) — feature support by Unity platform
