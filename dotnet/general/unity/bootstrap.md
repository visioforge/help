---
title: Unity SDK Bootstrap — Configure the Native Runtime
description: How VisioForgeEnvironment.Configure starts the native GStreamer runtime in Unity 6 on Windows, Android, macOS and iOS plus the Editor lifecycle.
sidebar_label: Bootstrap & lifecycle
order: 52
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Bootstrap
  - Windows
  - Android
  - macOS
  - iOS
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - VisioForgeX
---

# Bootstrap and lifecycle

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The Unity package ships a static helper, `VisioForgeEnvironment`, that brings up the bundled native
runtime before the first scene loads. You do not call it from your scripts — Unity invokes it
automatically through `RuntimeInitializeOnLoadMethod`. This page explains what it does on each
platform and the lifecycle rules your scripts have to follow.

If you only need to ship the SDK, you can skip this page: drop `MediaBlocksPlayer` or
`RTSPViewerPlayer` into a scene and press **Play**. Come back here when you build your own scripts,
hit a runtime error, or want to understand why the Editor settings are mandatory.

## The two entry points

`VisioForgeEnvironment` has exactly two public methods your code interacts with:

| Method | When it runs | What it does |
|---|---|---|
| `Configure()` | Automatically, **before the first scene loads** (`[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]`). | Prepares the native runtime for the current platform — DLL search path, environment variables, CA bundle, Java bootstrap. Idempotent. |
| `InitializeSdk()` | You call it once before building a pipeline. The sample players do this in `Start()`. | Calls `VisioForgeX.InitSDK()` and scans the bundled plugin registry. Idempotent. |

Both methods are static. Both flag themselves complete only after every step succeeds — so a
recoverable failure (for example, the Android Java bootstrap firing before `currentActivity` is
ready) leaves the flag unset and a later call retries instead of silently no-op-ing into a broken
state.

## What `Configure()` does on each platform

`Configure()` dispatches on compile-time symbols (`UNITY_STANDALONE_WIN`, `UNITY_STANDALONE_OSX`,
`UNITY_ANDROID`, `UNITY_IOS`) — Unity defines exactly one of these per build target. The body of
each branch is the minimum the platform needs for GStreamer to find its plugins, locate its TLS
roots, and resolve the rest of the bundled runtime via its native loader.

=== "Windows"

    1. Validate the bundled natives directory exists (`StreamingAssets/VisioForge/x64`). Refuse to
       mutate any process state if not — a missing folder must not poison the process `PATH`.
    2. Strip any system GStreamer entry from the **process** `PATH` (a homebrew / system install
       on `PATH` would load a second physical copy of `gstreamer-1.0-0.dll`, double-register GLib
       types and hang the pipeline).
    3. Point the Win32 DLL loader at the bundled natives via `SetDllDirectoryW`.
    4. Prepend the natives folder to the process `PATH` so each plugin's transitive core-lib
       dependencies resolve (`SetDllDirectory` alone is not enough for those).
    5. Set `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` to the same flat folder.
    6. Set `SSL_CERT_FILE` and `CA_CERTIFICATES` to the bundled `ca-certificates.crt` so RTSPS and
       HTTPS verify peers.

    The user / system `PATH` is never touched — only the live process copy.

=== "macOS"

    1. Resolve the natives path by probing the Unity-known layouts: `Plugins/macOS` (Editor and
       some Standalone Player versions), then `Contents/PlugIns`, `Contents/PlugIns/macOS`,
       `Contents/Frameworks`, and `Contents/Resources/Data/Plugins/macOS` for a Standalone `.app`.
       The first folder that contains `libgstreamer-1.0.0.dylib` wins. The result is cached.
    2. Prune any system / homebrew GStreamer from `DYLD_LIBRARY_PATH` (`/opt/homebrew/lib`,
       `/usr/local/lib`) — same double-init failure mode as the Windows `PATH` prune.
    3. Set `GST_PLUGIN_PATH` / `GST_PLUGIN_SYSTEM_PATH` to the natives folder so GStreamer can
       enumerate the bundled plugins.
    4. Set `GIO_MODULE_DIR` so GIO finds `libgioopenssl.so` (the TLS backend that RTSPS / HTTPS
       verify peers through).
    5. Set `SSL_CERT_FILE` and `CA_CERTIFICATES` to the bundled CA bundle.

    Each bundled `.dylib` has its `@rpath` / `@loader_path` baked in by the NuGet pack, so once
    dyld has loaded one through the first `[DllImport]`, the others resolve siblings
    automatically — no `SetDllDirectory` equivalent is needed.

=== "Android"

    1. Acquire `com.unity3d.player.UnityPlayer.currentActivity` through JNI. If the field is null
       — Wear OS / Android TV variants, Unity-as-a-library hosts that have not assigned it yet,
       very early `BeforeSceneLoad` on some Unity 6 startup paths — fail fast with a descriptive
       exception so the customer sees something better than an opaque `NullReferenceException`
       from deep inside JNI.
    2. Resolve `getFilesDir()` and `getCacheDir()` from the Activity.
    3. Capture the previous values of `TMP`, `TEMP`, `TMPDIR`, `XDG_RUNTIME_DIR`,
       `XDG_CACHE_HOME`, `HOME`, `GST_REGISTRY`, `SSL_CERT_FILE`, `CA_CERTIFICATES` so a later
       failure can roll them back.
    4. Point GLib at the app-private writable dirs by setting the variables above (only the
       app-private dirs are writable on Android).
    5. Extract the embedded `ca-certificates.crt` resource into `filesDir/ssl/certs/` and point
       `SSL_CERT_FILE` / `CA_CERTIFICATES` at it.
    6. Call `org.freedesktop.gstreamer.GStreamer.init(activity)` — this loads
       `libgstreamer_android.so`, captures the JavaVM in `JNI_OnLoad`, runs `gst_init`, and
       registers every plugin statically linked into the monolith.
    7. If any of those throws, restore the captured environment and rethrow.

=== "iOS"

    1. Extract the embedded `ca-certificates.crt` resource into `Application.persistentDataPath/ssl/certs/`.
    2. Set `SSL_CERT_FILE` / `CA_CERTIFICATES` to that path so GIO's OpenSSL backend can verify
       RTSPS / HTTPS peers.
    3. Prime the `NativesPath` cache on the main thread (`s_cachedNativesPath =
       Application.dataPath.Replace('\\', '/')`). Without this prime, a background-thread reader — the GStreamer bus,
       an async log callback, a pad-added callback — would later hit the lazy getter and call
       `Application.dataPath` off the main thread, which throws `UnityException`. The compute
       formula matches the lazy getter byte-for-byte.

    iOS does not need a plugin scan: every GStreamer plugin is statically registered inside
    `GStreamerX.framework` at `gst_plugin_register_static()` time, and dyld resolves
    `@rpath/GStreamerX.framework/GStreamerX` automatically when the first `[DllImport]` fires.

=== "Other"

    `Configure()` sets the success flag and logs a warning. No native runtime exists for the
    current platform, so any later `[DllImport]` would throw `DllNotFoundException`. Build for
    one of the four supported platforms instead.

## What `InitializeSdk()` does

After `Configure()` has prepared the runtime, `InitializeSdk()` finishes the bring-up:

1. Refuse to run if `Configure()` never succeeded — failing fast here surfaces an actionable
   error instead of a `DllNotFoundException` deep inside the SDK.
2. Refuse to run on an unsupported `Application.platform` value at runtime (Windows /
   Android / macOS / iOS are admitted; everything else short-circuits with a warning).
3. On Windows and macOS, before calling the native SDK, double-check that the resolved
   natives folder exists on disk. This catches the cross-flavor mismatch (Windows-only
   `.unitypackage` imported into a macOS host, or the opposite) with a clear message instead of
   an opaque `DllNotFoundException`. Android and iOS skip this check (no folder to probe).
4. Call `VisioForgeX.InitSDK()`. Catch and log on failure; leave the flag unset so a later
   retry can succeed.
5. On Windows and macOS, explicitly scan the bundled plugin folder with
   `Gst.Registry.Get().ScanPath(NativesPath)`. Unity's in-process plugin scan does not reliably
   honour `GST_PLUGIN_PATH` on either platform; the explicit scan is what makes blocks like
   `BufferSinkBlock` (which depends on `appsink`) load. Android registers plugins statically in
   `GStreamer.init`; iOS registers them statically in the framework — both skip the scan.
6. Set the success flag.

The sample players (`MediaBlocksPlayer`, `RTSPViewerPlayer`) call `InitializeSdk()` from their
`Start()` method, before they build a pipeline. Your scripts should follow the same pattern.

## The Editor lifecycle

The SDK initializes once per Editor process and is reused across **Play → Stop → Play**
sessions. Two consequences:

- **Disable Domain Reload is mandatory.** With it enabled, leaving Play mode triggers a domain
  reload while the SDK's background GLib main-loop thread is still running, which can hang the
  Editor. The Editor settings dialog the package shows on first import configures this for you;
  set it manually under **Edit → Project Settings → Editor → Enter Play Mode Settings** if you
  skipped that dialog.
- **Do not call `VisioForgeX.DestroySDK()` on Stop or in `OnDestroy`.** GStreamer's `gst_deinit`
  cannot be re-initialized in the same process — destroying the SDK on Stop and trying to use it
  again on the next Play crashes inside the native registry. The sample players follow this
  rule: their `OnDestroy` disposes only the per-play pipeline. The SDK stays alive for the rest
  of the process lifetime.

There is one Editor-only guard the package installs automatically: a
`VisioForgeEditorReloadGuard` that calls `VisioForgeX.StopMainLoop()` on `beforeAssemblyReload`
and `EditorApplication.quitting`. The GLib main-loop runs on a dedicated background thread,
blocked inside a native call Unity cannot abort — without this guard, the domain reload that
follows a script recompile would hang. The guard does **not** call `DestroySDK` (see above); it
stops only the loop thread, and the next Play rebuilds the loop. This guard is internal — your
scripts should ignore it.

## Frequently Asked Questions

### Do I have to call `Configure()` manually?

No. Unity's `[RuntimeInitializeOnLoadMethod(BeforeSceneLoad)]` attribute runs it for you before
the first scene loads. The only time you would call it again is from a custom recovery path when
an earlier attempt failed — and `Configure()` is idempotent, so a redundant call is harmless.

### Why does `Configure()` mutate environment variables instead of passing arguments?

GLib reads `GST_PLUGIN_PATH`, `GIO_MODULE_DIR`, `SSL_CERT_FILE`, `HOME`, etc. from the C
`environ` directly during `gst_init` and again at first TLS use. The SDK has no API to override
them — the only correct way to point the runtime at the bundled assets is to set the variables
before any pipeline is built. The mutations are scoped to the process; the user and system
environments are untouched.

### What happens if I call `InitializeSdk()` before `Configure()` has succeeded?

It logs an error and returns. The success flag stays unset so a later retry can succeed once
`Configure()` works. This guard exists because `InitSDK()` will otherwise crash deep inside
native code with a far less actionable error.

### Can I run two pipelines side by side?

Yes. `InitializeSdk()` brings the SDK up once per process; after that you can construct as many
`MediaBlocksPipeline` instances as you want. Each is independent — the sample multi-camera RTSP
pattern is to attach one `RTSPViewerPlayer` per `RawImage`, and each builds and tears down its
own pipeline.

## See also

- [Using VisioForge in Unity](index.md) — package overview and how rendering works
- [Build for Windows](windows.md) — Windows Editor and Standalone build settings
- [Build for Android](android.md) — Android IL2CPP build settings
- [Build for macOS](macos.md) — macOS Standalone build settings
- [Build for iOS](ios.md) — iOS device build settings
- [Troubleshooting](troubleshooting.md) — common bootstrap and runtime errors
