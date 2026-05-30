---
title: Build a Unity App with Video Playback for Android arm64
description: Build settings, IL2CPP, AndroidManifest permissions and troubleshooting for the VisioForge Media Blocks SDK .NET in Unity 6 on Android.
sidebar_label: Build for Android
order: 54
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Android
  - IL2CPP
  - RTSP
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - RTSPSourceBlock
---

# Build for Android

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The Android flavor ships as a monolithic `libgstreamer_android.so` with every GStreamer plugin
statically linked in, plus the package's managed assemblies built against `netstandard2.1`. This
page covers the Build Profile settings, the manifest permissions, and the Android-specific
gotchas. For the bring-up sequence shared across platforms, see
[Bootstrap and lifecycle](bootstrap.md).

The Android flavor is bundled into the `.unitypackage` when the build pipeline is run with
`-IncludeAndroid`. The result is one cumulative package that holds the Windows, Android, and any
opt-in Apple runtimes together — Unity then picks the correct one at build time from the per-file
`PluginImporter` metadata.

## Player Settings

| Setting | Value | Where |
|---|---|---|
| Target Platform | **Android** | File → Build Profiles → Android |
| Texture Compression | **ASTC** *(recommended; default in Unity 6)* | File → Build Profiles → Android |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **IL2CPP** *(mandatory — Mono is not supported on Android by Unity itself)* | Project Settings → Player → Other Settings → Configuration |
| Target Architectures | **ARM64** *(ARMv7 not shipped — uncheck it)* | Project Settings → Player → Other Settings → Configuration |
| Internet Access | **Require** *(needed for RTSP / HTTPS sources)* | Project Settings → Player → Other Settings → Configuration |
| Write Permission | **External (SDCard)** if you write or record media to external storage | Project Settings → Player → Other Settings → Configuration |
| Minimum API Level | **24 (Android 7.0)** or later | Project Settings → Player → Other Settings → Identification |

Mono cannot load `libgstreamer_android.so` correctly through Unity's Android runtime — only
IL2CPP is exercised in CI and supported in production.

## Required AndroidManifest entries

Unity generates `AndroidManifest.xml` for you. The settings above translate into the standard
entries; if you need a custom manifest, make sure it contains:

```xml
<uses-permission android:name="android.permission.INTERNET" />

<!-- Only if your app uses RTSP discovery / streams audio-out via UDP on the local segment -->
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />

<!-- Only if your app uses the microphone or camera as a source -->
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.CAMERA" />

<!-- Only if you read media from external storage -->
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE"
                 android:maxSdkVersion="32" />
<uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
<uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />
```

`READ_MEDIA_VIDEO` / `READ_MEDIA_AUDIO` replace the legacy `READ_EXTERNAL_STORAGE` on Android 13+
(API 33+); declare both forms so older devices keep working.

## What the package adds for Android

The `deploy-unity-natives.ps1` step stages the Android runtime into your project as follows:

| Path | Contents |
|---|---|
| `Assets/Plugins/Android/libs/arm64-v8a/libgstreamer_android.so` | Monolithic GStreamer runtime — all plugins statically linked. |
| `Assets/Plugins/Android/libs/arm64-v8a/libVisioForge_Core.so` | The SDK's Android-flavor native shim. |
| `Assets/Plugins/Android/visioforge-gstreamer.aar` | The Java archive that exposes `org.freedesktop.gstreamer.GStreamer.init(Context)`. |
| `Assets/VisioForge/link.xml` | Type / member preservation rules for IL2CPP. |
| `Assets/Plugins/Android/` | Managed assemblies built against `netstandard2.1` with `UNITY_NS21_ANDROID` defined. |

The `link.xml` is mandatory. Without it, IL2CPP's managed-code stripping removes types the SDK
references through reflection — `GLib.SignalArgs` subclasses, `SignalClosure.MarshalCallback` — and
the first delegate firing throws `MissingMethodException`. The package ships a tested
`link.xml`; do not remove it from `Assets/`.

## Build size

The Android flavor adds roughly **35 MB** to the APK / AAB:

- `libgstreamer_android.so` — ~30 MB (arm64-v8a, stripped)
- `libVisioForge_Core.so` — ~2 MB
- Managed assemblies — ~3 MB

If you ship ARMv7 too (not currently included by the package, but if you stage it manually),
expect to double the native libraries.

## Standalone build

**File → Build Profiles → Android → Build** (or **Build And Run**) produces an APK / AAB.

Tested on:

- Unity 6 (`6000.x`) with the Android Build Support module installed
- Android 9 through Android 15 devices
- Pixel 6 / 7 / 8 / 9, Galaxy S22 / S23 / S24

## Troubleshooting

| Symptom | Cause | Fix |
|---|---|---|
| `DllNotFoundException: libgstreamer_android` at scene start | Scripting Backend is Mono, not IL2CPP. | Switch to IL2CPP under Project Settings → Player → Other Settings. |
| `MissingMethodException` from `GLib.SignalArgs` or `SignalClosure.MarshalCallback` | `link.xml` is missing or was stripped. | Confirm `Assets/VisioForge/link.xml` exists. Re-import the package if it is not there. |
| `InvalidOperationException: Unity Android bootstrap requires com.unity3d.player.UnityPlayer.currentActivity to be available` | Wear OS / Android TV / Unity-as-a-library host where the field is null at `BeforeSceneLoad`. | Defer `VisioForgeEnvironment.Configure()` to the first Activity event you can observe and call it manually from there. |
| Java init fails with `failed to find getFilesDir` | Activity is not a `UnityPlayerActivity` and does not expose the standard Android Context API. | Confirm the host Activity inherits from a real Android `Activity`. |
| RTSPS / HTTPS streams fail with a TLS error | CA bundle extraction failed silently. | Look in the Logcat for `[VisioForge] CA cert extraction failed`. Re-import the package if the embedded resource is missing. |
| App crashes on the second `Awake` after returning from background | `VisioForgeX.DestroySDK()` was called in `OnDestroy`. | Don't call it — see [Bootstrap and lifecycle](bootstrap.md#the-editor-lifecycle). |

## Frequently Asked Questions

### Why is ARMv7 not shipped?

Modern Android devices (API 24 / Android 7.0 baseline) are predominantly arm64. Shipping the
~30 MB monolithic GStreamer for both ABIs would double the package size for a vanishingly small
device share. If you have a hard ARMv7 requirement, contact support.

### Can I use the SDK in a non-Unity Android project?

Yes — the underlying SDK ships as a standalone `VisioForge.CrossPlatform.Core.Android` NuGet
package for raw .NET Android apps. The Unity package wraps that runtime plus the Java bootstrap
and `link.xml`; the wrapper is Unity-specific.

### Does the SDK work in the Android Editor (Project Player → Run In Editor) mode?

Run-in-Editor for Android targets is not exercised; build and deploy to a real device. The
Editor itself runs the **Windows** flavor of the SDK on a Windows host — switching the Build
Target to Android in Build Profiles does not change which native runtime the Editor loads.

### Which sources work over RTSP on Android?

The same `RTSPSourceBlock` used on Windows. Auto-reconnect, optional credentials, video-only and
video+audio streams, and the standard RTSP transports (TCP, UDP, UDP multicast) are all
supported. The Android flavor uses the GStreamer `rtspsrc` element internally — the same one as
the Windows and macOS flavors.

## See also

- [Install the Media Blocks SDK in Unity](../../install/unity.md) — package setup
- [Bootstrap and lifecycle](bootstrap.md) — the Android Java bootstrap explained
- [View an RTSP camera in Unity](rtsp-viewer.md) — the `RTSPViewer` sample
- [Troubleshooting](troubleshooting.md) — cross-platform error reference
- [Platform matrix](platform-matrix.md) — feature support by Unity platform
