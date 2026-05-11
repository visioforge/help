---
name: video-capture-sdk-x-android
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a native .NET for Android application. Covers the Android-specific VideoView control, the cross-platform NuGet package, AndroidDependency project reference, runtime camera/audio permissions, license registration, and the most common Android pitfalls (CAMERA permission denied, RECORD_AUDIO permission denied, AAB vs APK packaging, ABI filters, trial-period expiry / unlicensed build). Use for native .NET for Android (NOT MAUI / Xamarin) capture apps — for cross-OS MAUI use video-capture-sdk-x-maui.
---

# Video Capture SDK X — native .NET for Android integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a **native .NET for Android** application (TFM `net10.0-android`, Activity-based, NOT MAUI and NOT classic Xamarin.Android). The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level capture-and-record god-object (`VideoCaptureCoreX`) that mirrors the legacy `VideoCaptureCore` API. The same C# code works unchanged on WPF / MAUI / Avalonia / Uno / iOS / macOS — only the UI host swaps (`VideoViewGL` here, `<my:VideoView />` on MAUI, etc.) and the per-OS native redist NuGet.

Pinned NuGet versions: wrapper **`2026.5.4`**, Android redist **`2026.4.18.0`** (matches the [official Simple Video Capture Android sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Android/Simple%20Video%20Capture)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding webcam (front/back), audio, or screen capture to a native .NET for Android app (`net10.0-android`, Activity-based).
- Recording captured video/audio to MP4 on-device, with `StartCaptureAsync` / `StopCaptureAsync` toggling recording while preview keeps running.
- Sharing a single C# capture/recording layer across an Android app and one or more desktop / mobile companions (the `VideoCaptureCoreX` API is identical across platforms — only the host control changes).
- Live camera switching (front ↔ back) without tearing down the pipeline.

## When NOT to use this skill

- **MAUI app that targets Android + other OSes from one codebase**: use `media-blocks-sdk-net-maui` (graph-based, lower-level) — the conditional multi-target csproj and `AddVisioForgeHandlers()` registration are MAUI-specific and not covered here.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap on Android): use `media-blocks-sdk-net-android` — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Classic Xamarin.Android** (`MonoAndroid12.0` TFM, not `net10.0-android`): unsupported — Xamarin.Android went out of support in May 2024, migrate to `net8.0-android` or newer first.
- **Playback only** (play files / streams without capturing): `media-player-sdk-x-android`.
- **iOS / macOS host instead of Android**: same SDK, different host → `video-capture-sdk-x-ios` / `video-capture-sdk-x-maui`.

## Project setup

### Target framework

Video Capture SDK X 2026.x supports `net8.0-android`, `net9.0-android`, and `net10.0-android` for the native Android host. The upstream Simple Video Capture sample pins **`net10.0-android`** with `<SupportedOSPlatformVersion>28.0</SupportedOSPlatformVersion>` (Android 9.0 / API level 28 minimum) — match these unless you have a reason not to. API 28 is the floor because the underlying GStreamer redist drops below that level.

### NuGet packages + AndroidDependency

A native Android capture project needs **two NuGet packages plus one ProjectReference**:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <PackageReference Include="Xamarin.Essentials" Version="1.8.1" />
</ItemGroup>
<ItemGroup>
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy SDK uses — both `VideoCaptureCore` (legacy, Windows-only) and `VideoCaptureCoreX` (cross-platform) ship in it. What switches you to the X engine on Android is the redist (`VisioForge.CrossPlatform.Core.Android`) plus the `AndroidDependency` project reference.

The `AndroidDependency` project is **not on NuGet** — it's a small companion csproj (`VisioForge.Core.Android.X10.csproj` for `net10.0-android`, or `X9.csproj` for `net9.0-android`) that ships in the samples repo and binds the underlying `.aar` plus a handful of Java callback classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`). Copy the entire `AndroidDependency/` folder from `https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency` next to your app and adjust the relative path in `<ProjectReference Include="...">`. The package alone is **not** enough — without the project reference, builds succeed but `VideoCaptureCoreX` aborts at construction time with a missing-Java-class error.

`Xamarin.Essentials 1.8.1` is used by the upstream sample for the `OnRequestPermissionsResult` plumbing. On `net10.0-android` you can substitute the built-in `AndroidX` permissions API if you prefer fewer dependencies — the SDK itself does not require Xamarin.Essentials.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture sample (`Video Capture SDK X/Android/Simple Video Capture/Simple Video Capture.csproj`) — kept verbatim except for stripping demo-only metadata; the bundled file builds standalone against the public NuGet packages plus the `AndroidDependency/` project copied in alongside.

### AndroidManifest permissions

`Properties/AndroidManifest.xml` (or root-level `AndroidManifest.xml`) MUST declare camera, microphone, and Internet:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:label="@string/app_name" android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.CAMERA" />
    <uses-permission android:name="android.permission.RECORD_AUDIO" />
</manifest>
```

`INTERNET` is required even for offline capture — the GStreamer redist binds a few network helpers internally. Omitting it produces a delayed, cryptic `SocketException` from inside `_core.StartAsync()`. Storage permissions (`READ_EXTERNAL_STORAGE` / `WRITE_EXTERNAL_STORAGE`) are NOT required if you write recordings under `GetExternalFilesDir(...)` (app-private storage) — this is the recommended path on API 29+ where scoped storage applies.

### Layout — `VideoViewGL`

The Android-specific renderer is `VisioForge.Core.UI.Android.VideoViewGL` (an OpenGL `SurfaceView`-based control), referenced by full type name in the layout XML. Drop it into your activity layout exactly like a stock view:

```xml
<!-- Resources/layout/activity_main.xml -->
<?xml version="1.0" encoding="utf-8"?>
<FrameLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent">

    <VisioForge.Core.UI.Android.VideoViewGL
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:id="@+id/videoView" />
</FrameLayout>
```

Resolve it in code with `FindViewById<VisioForge.Core.UI.Android.VideoViewGL>(Resource.Id.videoView)`. There is **no** Android-specific XML namespace declaration — the fully-qualified type name in the element is enough; the Mono/.NET-for-Android resource compiler binds it automatically.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _core.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
_core = new VideoCaptureCoreX(videoView);
_core.OnError += _core_OnError;

// Ship the licence as an Android asset (Build Action: AndroidAsset, file under Assets/license.vflicense).
using var assetStream = Assets.Open("license.vflicense");
using var ms = new MemoryStream();
await assetStream.CopyToAsync(ms);
await _core.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (Android asset, `getFilesDir()`, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper. If your app constructs multiple `VideoCaptureCoreX` instances (rare on Android — one per Activity is typical), each instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`.

The bundled `references/MainActivity.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the snippet above into `CreateEngineAsync()` right after `_core = new VideoCaptureCoreX(videoView)`.

## Runtime permissions

Declaring `CAMERA` / `RECORD_AUDIO` in the manifest is necessary but **not sufficient** on API 23+. You MUST also request them at runtime, and you MUST defer pipeline construction until both grants land. The canonical pattern from the upstream sample:

```csharp
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    SetContentView(Resource.Layout.activity_main);

    RequestPermissions(new[]
    {
        Manifest.Permission.Camera,
        Manifest.Permission.Internet,
        Manifest.Permission.RecordAudio,
        Manifest.Permission.ModifyAudioSettings
    }, requestCode: 1004);

    videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewGL>(Resource.Id.videoView);
    // ...wire buttons...
}

public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
    [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

    if (_previewStarted) return;                       // guard: only start once
    if (CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted) return;
    if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted) return;

    _previewStarted = true;
    Task.Run(StartPreviewAsync);
}
```

The `_previewStarted` guard matters: `OnRequestPermissionsResult` can fire more than once (the user can re-prompt), and constructing two `VideoCaptureCoreX` instances against the same `VideoViewGL` is undefined behaviour.

## Hello-World capture

Minimum viable capture-and-preview snippet — a self-contained `MainActivity` you can drop into a fresh `net10.0-android` project. (For the full feature set incl. recording and live camera switch, copy `references/` into your project and skip this section.)

```csharp
// MainActivity.cs
using Android;
using Android.Runtime;
using Android.Util;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace YourApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true,
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
              Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private VisioForge.Core.UI.Android.VideoViewGL videoView;
        private VideoCaptureCoreX _core;
        private bool _previewStarted;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            RequestPermissions(new[] {
                Manifest.Permission.Camera,
                Manifest.Permission.Internet,
                Manifest.Permission.RecordAudio,
                Manifest.Permission.ModifyAudioSettings
            }, 1004);

            videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewGL>(Resource.Id.videoView);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (_previewStarted) return;
            if (CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted) return;
            if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted) return;
            _previewStarted = true;

            Task.Run(async () =>
            {
                try
                {
                    var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
                    if (cameras.Length == 0) return;

                    _core = new VideoCaptureCoreX(videoView);
                    _core.OnError += (_, e) => Log.Error("MainActivity", e.Message);
                    // For a purchased licence, add here:
                    //   using var s = Assets.Open("license.vflicense");
                    //   using var ms = new MemoryStream(); await s.CopyToAsync(ms);
                    //   await _core.SetLicenseCertificateAsync(ms.ToArray());

                    _core.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[0]);
                    _core.Video_Play = true;

                    await _core.StartAsync();
                }
                catch (Exception ex) { Log.Error("MainActivity", ex.ToString()); }
            });
        }

        protected override async void OnDestroy()
        {
            try { if (_core != null) { await _core.DisposeAsync(); _core = null; } }
            catch (Exception ex) { Log.Error("MainActivity", ex.ToString()); }
            VisioForgeX.DestroySDK();
            base.OnDestroy();
        }
    }
}
```

Note: unlike WPF / MAUI, the Android host does **not** require an explicit `await VisioForgeX.InitSDKAsync()` before constructing `VideoCaptureCoreX` — the Android wrapper boots the engine on first use. You DO still need `VisioForgeX.DestroySDK()` on `OnDestroy` to release native resources cleanly between Activity recreations.

`references/MainActivity.cs` (paired with `references/Resources/layout/activity_main.xml`) ships the full pattern with MP4 recording (pre-configured with `Outputs_Add` + `autoStart: false` so preview can run while record toggles), front/back camera switching with live `Video_Source_SwitchCamera` + full-restart fallback, gallery save via `PhotoGalleryHelper.AddVideoToGalleryAsync`, and `OnError` logging.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. `CAMERA` / `RECORD_AUDIO` permission denied — preview is black, no error

**Cause**: the manifest declares the permission but the runtime request was either skipped or denied, **or** preview construction fired before the user tapped "Allow". On API 23+ Android holds back the hardware until both the manifest declaration AND a runtime grant exist; the SDK gets back a null camera handle and silently produces a black preview.

**Fix**: use the `OnRequestPermissionsResult` + `_previewStarted` guard pattern shown above. Do NOT construct `VideoCaptureCoreX` from `OnCreate` — defer it until the grants land. If the user denied the permission, prompt again from a settings UI; "Don't ask again" requires sending the user to system Settings via `Intent.ActionApplicationDetailsSettings`.

### 2. `Java.Lang.ClassNotFoundException: org.freedesktop.gstreamer.GStreamer` (or one of the `Gst*Callback` classes)

**Cause**: the `AndroidDependency` `<ProjectReference>` is missing from the csproj. The `VisioForge.CrossPlatform.Core.Android` NuGet ships the native `.so` files but **not** the four Java glue classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`) — those live in the companion `AndroidDependency` project, which is `<AndroidJavaSource>`-compiled into your APK.

**Fix**: copy the `AndroidDependency/` folder from the samples repo (`https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency`) next to your app, add `<ProjectReference Include="...\AndroidDependency\VisioForge.Core.Android.X10.csproj" />` (or `X9.csproj` if you target `net9.0-android`), and rebuild. `dotnet build` regenerates the binding stubs from the Java sources automatically.

### 3. AAB upload to Play Console fails / device install errors out with missing-`.so` for an ABI

**Cause**: the `VisioForge.CrossPlatform.Core.Android` redist ships native libraries for `arm64-v8a` and `armeabi-v7a` (and `x86_64` for emulator). When packaging as **AAB** (Android App Bundle, the default for Play Store) Google Play does per-device split delivery — but **only if every ABI present in `<RuntimeIdentifiers>` has a complete set of `.so` files**. If your csproj sets `<RuntimeIdentifiers>android-arm64;android-arm;android-x86;android-x64</RuntimeIdentifiers>` you'll trip on `android-x86` (32-bit Intel) which the redist does not provide and which Play Store will reject with "Bundle contains native code, but does not contain code for ...".

**Fix**: drop `android-x86` from `<RuntimeIdentifiers>` (32-bit Intel Android devices are essentially extinct; the Play Store no longer requires it). Keep `android-arm64;android-arm;android-x64`. For local APK testing on a `x86_64` emulator, `android-x64` is enough; for Play Store production, `android-arm64;android-arm` is enough.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."` surfaced via `OnError`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: ship the licence as an Android asset (place under `Assets/license.vflicense`, Build Action `AndroidAsset`), open with `Assets.Open(...)`, copy to `byte[]`, and call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every `VideoCaptureCoreX` instance in the process needs its own call.

### 5. App crashes on `OnDestroy` / second launch shows a blank screen

**Cause**: `VideoCaptureCoreX` was not disposed before the Activity tore down, **or** `VisioForgeX.DestroySDK()` was called while another Activity instance still held a live `VideoCaptureCoreX`. Both leak native handles, and on the next launch the GStreamer registry is in an inconsistent state.

**Fix**: in `OnDestroy`, run `await _core.DisposeAsync()` first (sets `_core = null`), then `VisioForgeX.DestroySDK()`. Wrap both in `try/catch` and log — exceptions thrown from `OnDestroy` kill the process. The bundled `references/MainActivity.cs` shows the canonical pattern.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -f net10.0-android` succeeds with the bundled `references/` files copied in plus the `AndroidDependency/` folder in place (no missing `.so` warnings, no `ClassNotFoundException` at runtime).
- [ ] `dotnet build -t:Run -f net10.0-android` deploys to a connected device or emulator and shows a webcam preview within ~1 s after the user grants `CAMERA` + `RECORD_AUDIO`.
- [ ] Toggling Record / Stop produces an MP4 under `GetExternalFilesDir(Android.OS.Environment.DirectoryMovies)` that opens correctly in another player.
- [ ] Switching front ↔ back camera during preview does not crash (live switch + full-restart fallback both work).
- [ ] `OnDestroy` runs `DisposeAsync → DestroySDK` cleanly; relaunching the app shows a fresh preview, not a black screen.
- [ ] AAB packaging targets only `android-arm64;android-arm` (and optionally `android-x64` for emulator); `android-x86` is NOT in `<RuntimeIdentifiers>`.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode and aborts after 30 days).

## Bundled references

The `references/` folder is a faithful copy of the upstream Simple Video Capture sample — copy all of it into a fresh `net10.0-android` project folder, copy the `AndroidDependency/` folder from the samples repo into the parent solution, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-android` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph). Includes the `<ProjectReference>` to `AndroidDependency`.
- `references/AndroidManifest.xml` — minimal manifest with `INTERNET`, `CAMERA`, and `RECORD_AUDIO` declared.
- `references/MainActivity.cs` — full code-behind: device enumeration, runtime permission flow with `_previewStarted` guard, MP4 output pre-configured with `autoStart: false`, `StartCaptureAsync` / `StopCaptureAsync` toggle, front/back live camera switch with `Video_Source_SwitchCamera` plus full-restart fallback, gallery save via `PhotoGalleryHelper.AddVideoToGalleryAsync`, and `OnError` logging. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Resources/layout/activity_main.xml` — Activity layout with `<VisioForge.Core.UI.Android.VideoViewGL />` plus Record and Switch-camera `ImageButton` controls.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Android>
- **AndroidDependency project (required `<ProjectReference>`)**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-wpf` — same X SDK on a Windows WPF host.
    - `video-capture-sdk-x-winui` — same X SDK on a Windows WinUI 3 host.
    - **Cross-platform / mobile**:
        - `video-capture-sdk-x-ios` — same X SDK on a native iOS host.
        - `video-capture-sdk-x-macos` — same X SDK on a native macOS host.
        - `video-capture-sdk-x-maui` — same X SDK on .NET MAUI (one codebase across Android + iOS + Windows + Mac Catalyst).
        - `media-blocks-sdk-net-android` — lower-level graph-based engine on Android for custom pipeline topologies.
