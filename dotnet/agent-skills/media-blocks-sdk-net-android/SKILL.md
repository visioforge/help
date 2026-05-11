---
name: media-blocks-sdk-net-android
description: Integrate VisioForge Media Blocks SDK into a native .NET for Android application. Covers the graph-based pipeline model on Android, the cross-platform NuGet package, AndroidDependency project reference, runtime camera/audio permissions, license registration, and the most common Android pitfalls (CAMERA permission denied, RECORD_AUDIO permission denied, AAB packaging ABI filters, hardware encoder support, trial-period expiry / unlicensed build). Use for native .NET for Android pipelines (capture, transcode, stream, record) — for cross-OS MAUI use media-blocks-sdk-net-maui.
---

# Media Blocks SDK .NET — native .NET for Android integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a **native .NET for Android** application (TFM `net10.0-android`, Activity-based, NOT MAUI and NOT classic Xamarin.Android). Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. The same C# block code runs unchanged on WPF / MAUI / Avalonia / Uno / iOS / macOS — only the UI host swaps (`VideoViewGL` here, `<my:VideoView />` on MAUI, etc.) and the per-OS native redist NuGet.

Pinned NuGet versions: wrapper **`2026.5.4`**, Android redist **`2026.4.18.0`** (matches the [official Simple Video Capture Android sample for Media Blocks](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android/Simple%20Video%20Capture)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Building a **custom pipeline** on Android that the high-level capture SDK doesn't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capturing → encoding → muxing to MP4 / MPEG-TS / WebM on-device with explicit control over each stage.
- Network streaming sinks (RTSP server, SRT, RIST, RTMP) wired into a custom graph on a phone or tablet.
- Live camera switching (front ↔ back) inside a graph that includes a tee + encoder + sink — `_videoSource.SwitchCamera(newSettings)` swaps the source pad without tearing down the recording sink (live-switch path), with a full pipeline rebuild as fallback when format/fps don't match.

## When NOT to use this skill

- **Plain webcam capture** (preview + record to MP4, no custom topology) on Android: `video-capture-sdk-x-android` is dramatically less code (`VideoCaptureCoreX` is the high-level wrapper around the same engine).
- **MAUI app that targets Android + other OSes from one codebase**: use `media-blocks-sdk-net-maui` — the conditional multi-target csproj and `AddVisioForgeHandlers()` registration are MAUI-specific.
- **Classic Xamarin.Android** (`MonoAndroid12.0` TFM, not `net10.0-android`): unsupported — Xamarin.Android went out of support in May 2024, migrate to `net8.0-android` or newer first.
- **Playback only** (play files / streams without capturing or custom graph): `media-player-sdk-x-android`.
- **iOS / macOS host instead of Android**: same SDK, different host → `media-blocks-sdk-net-ios` / `media-blocks-sdk-net-maui`.

## Project setup

### Target framework

Media Blocks SDK 2026.x supports `net8.0-android`, `net9.0-android`, and `net10.0-android` for the native Android host. The upstream Simple Video Capture sample pins **`net10.0-android`** with `<SupportedOSPlatformVersion>28.0</SupportedOSPlatformVersion>` (Android 9.0 / API level 28 minimum) — match these unless you have a reason not to. API 28 is the floor because the underlying GStreamer redist drops below that level.

### NuGet packages + AndroidDependency

A native Android Media Blocks project needs **two NuGet packages plus one ProjectReference** — note that the wrapper package is `VisioForge.DotNet.MediaBlocks`, not the `VisioForge.DotNet.VideoCapture` used by the X SDK:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <PackageReference Include="Xamarin.Essentials" Version="1.8.1" />
</ItemGroup>
<ItemGroup>
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

The Android redist (`VisioForge.CrossPlatform.Core.Android`) is the **single** redist needed — unlike the WPF host (which needs Core + Libav redists separately), the Android redist bundles everything into one package. There is no `Libav.Android` to add.

The `AndroidDependency` project is **not on NuGet** — it's a small companion csproj (`VisioForge.Core.Android.X10.csproj` for `net10.0-android`, or `X9.csproj` for `net9.0-android`) that ships in the samples repo and binds the underlying `.aar` plus a handful of Java callback classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`). Copy the entire `AndroidDependency/` folder from `https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency` next to your app and adjust the relative path in `<ProjectReference Include="...">`. The package alone is **not** enough — without the project reference, builds succeed but `MediaBlocksPipeline` aborts at first block construction with a missing-Java-class error.

`Xamarin.Essentials 1.8.1` is used by the upstream sample for the `OnRequestPermissionsResult` plumbing. On `net10.0-android` you can substitute the built-in `AndroidX` permissions API if you prefer fewer dependencies — the SDK itself does not require Xamarin.Essentials.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture sample (`Media Blocks SDK/Android/Simple Video Capture/Simple Video Capture.csproj`) — kept verbatim except for stripping demo-only metadata; the bundled file builds standalone against the public NuGet packages plus the `AndroidDependency/` project copied in alongside.

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

`INTERNET` is required even for offline capture — the GStreamer redist binds a few network helpers internally. Omitting it produces a delayed, cryptic `SocketException` from inside `_pipeline.StartAsync()`. Storage permissions (`READ_EXTERNAL_STORAGE` / `WRITE_EXTERNAL_STORAGE`) are NOT required if you write recordings under `GetExternalFilesDir(...)` (app-private storage) — this is the recommended path on API 29+ where scoped storage applies.

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

Resolve it in code with `FindViewById<VisioForge.Core.UI.Android.VideoViewGL>(Resource.Id.videoView)`. There is **no** Android-specific XML namespace declaration — the fully-qualified type name in the element is enough; the Mono/.NET-for-Android resource compiler binds it automatically. The same `VideoViewGL` is used by both Media Blocks (passed to `VideoRendererBlock(_pipeline, videoView)`) and the X capture SDK (passed to `new VideoCaptureCoreX(videoView)`) — they share the renderer surface.

## Pipeline model

This is the core mental shift from the high-level `VideoCaptureCoreX`. There is no `Core.Start()` / `Core.Stop()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock` for Camera2 cameras, `SystemAudioSourceBlock` configured with `AndroidAudioSourceSettings`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, `AudioMixerBlock`, …) — accept on input pads, produce on output pads. On Android, encoders default to the platform hardware encoder (MediaCodec) when the device exposes one.
4. **Sink blocks** — terminate the graph. **Renderer sinks** drive the UI / speakers (`VideoRendererBlock` bound to a `VideoViewGL`, `AudioRendererBlock` bound to a system audio output device). **File / network sinks** write/transmit (`MP4SinkBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, …). Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `StartAsync`; live source switching at runtime is supported by `SystemVideoSourceBlock.SwitchCamera(newSettings)` when the new format matches the current resolution and fps.

Topology example (Simple Video Capture, preview + MP4 record):

```text
SystemVideoSourceBlock  --video-->  TeeBlock(video)  --[0]-->  VideoRendererBlock (VideoViewGL)
                                                     --[1]-->  H264EncoderBlock --> MP4SinkBlock(video pad)
SystemAudioSourceBlock  --audio-->  TeeBlock(audio)  --[0]-->  AudioRendererBlock (speakers)
                                                     --[1]-->  AACEncoderBlock  --> MP4SinkBlock(audio pad)
```

`MP4SinkBlock` is the same instance on both audio and video paths — its dynamic inputs collect the streams to mux. For preview-only, skip the tees and connect `_videoSource.Output → _videoRenderer.Input` directly (the upstream `StartPreviewAsync` does exactly this; the tee path is built only when toggling into record mode).

Unlike WPF / MAUI, the Android host does **not** require an explicit `await VisioForgeX.InitSDKAsync()` before constructing `MediaBlocksPipeline` — the Android wrapper boots the engine on first use. You DO still need `VisioForgeX.DestroySDK()` on `OnDestroy` to release native resources cleanly between Activity recreations.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += _pipeline_OnError;

// Ship the licence as an Android asset (Build Action: AndroidAsset, file under Assets/license.vflicense).
using var assetStream = Assets.Open("license.vflicense");
using var ms = new MemoryStream();
await assetStream.CopyToAsync(ms);
await _pipeline.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (Android asset, `getFilesDir()`, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper. If your app constructs and disposes pipelines repeatedly (e.g. the upstream sample tears down and rebuilds the pipeline on every record toggle and camera switch fallback), each fresh `MediaBlocksPipeline` instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`.

The bundled `references/MainActivity.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the snippet above into `CreateEngineAsync()` right after `_pipeline = new MediaBlocksPipeline();` and before `_pipeline.OnError += _pipeline_OnError;`.

## Runtime permissions

Declaring `CAMERA` / `RECORD_AUDIO` in the manifest is necessary but **not sufficient** on API 23+. You MUST also request them at runtime, and you MUST defer pipeline construction until both grants land. The upstream sample uses an `Interlocked.CompareExchange` guard so the start path runs at most once even if `OnRequestPermissionsResult` fires repeatedly:

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

private int _previewStarted; // 0 = not started, 1 = started

private void CheckPermissionsAndStartPreview()
{
    if (Interlocked.CompareExchange(ref _previewStarted, 1, 0) != 0) return;

    if (CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
    {
        Interlocked.Exchange(ref _previewStarted, 0);
        return;
    }
    if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted)
    {
        Interlocked.Exchange(ref _previewStarted, 0);
        return;
    }

    Task.Run(async () =>
    {
        try { await StartPreviewAsync(); }
        catch (Exception ex) { Log.Error("MainActivity", ex.ToString()); }
    });
}

public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
    [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    CheckPermissionsAndStartPreview();
}
```

Constructing two `MediaBlocksPipeline` instances against the same `VideoViewGL` is undefined behaviour — the `Interlocked` guard exists specifically to prevent that race when the user re-prompts mid-construction.

## Hello-World pipeline

Minimum viable preview-only pipeline (no recording) — drop into a fresh `net10.0-android` project. For the full feature set incl. tee + recording and live camera switch, copy `references/` into your project and skip this section.

```csharp
// MainActivity.cs
using Android;
using Android.Runtime;
using Android.Util;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;
using Activity = Android.App.Activity;

namespace YourApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true,
              ScreenOrientation = Android.Content.PM.ScreenOrientation.Portrait,
              Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private VisioForge.Core.UI.Android.VideoViewGL videoView;
        private MediaBlocksPipeline _pipeline;
        private int _previewStarted;

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

            if (Interlocked.CompareExchange(ref _previewStarted, 1, 0) != 0) return;
            if (CheckSelfPermission(Manifest.Permission.Camera) != Android.Content.PM.Permission.Granted)
            { Interlocked.Exchange(ref _previewStarted, 0); return; }
            if (CheckSelfPermission(Manifest.Permission.RecordAudio) != Android.Content.PM.Permission.Granted)
            { Interlocked.Exchange(ref _previewStarted, 0); return; }

            Task.Run(async () =>
            {
                try
                {
                    var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
                    if (cameras.Length == 0) return;

                    _pipeline = new MediaBlocksPipeline();
                    _pipeline.OnError += (_, e) => Log.Error("MainActivity", e.Message);

                    // For a purchased licence, add here:
                    //   using var s = Assets.Open("license.vflicense");
                    //   using var ms = new MemoryStream(); await s.CopyToAsync(ms);
                    //   await _pipeline.SetLicenseCertificateAsync(ms.ToArray());

                    var src = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(cameras[0]));
                    var renderer = new VideoRendererBlock(_pipeline, videoView) { IsSync = false };
                    _pipeline.Connect(src.Output, renderer.Input);

                    await _pipeline.StartAsync();
                }
                catch (Exception ex) { Log.Error("MainActivity", ex.ToString()); }
            });
        }

        protected override async void OnDestroy()
        {
            try
            {
                if (_pipeline != null)
                {
                    await _pipeline.StopAsync();
                    await _pipeline.DisposeAsync();
                    _pipeline = null;
                }
            }
            catch (Exception ex) { Log.Error("MainActivity", ex.ToString()); }
            VisioForgeX.DestroySDK();
            base.OnDestroy();
        }
    }
}
```

`references/MainActivity.cs` (paired with `references/Resources/layout/activity_main.xml`) ships the full pattern: `CreateEngineAsync()` builds the source/renderer/audio chain, `StartPreviewAsync()` connects source → renderer directly for preview, `btStartRecord_Click` rebuilds the graph with `TeeBlock` + `H264EncoderBlock` + `AACEncoderBlock` + `MP4SinkBlock` and writes recordings to `GetExternalFilesDir(Android.OS.Environment.DirectoryMovies)`, `btSwitchCam_Click` does live `_videoSource.SwitchCamera(...)` first with a full pipeline restart fallback when the new camera doesn't expose the current resolution/fps, and `OnDestroy` runs `DisposeAsync → DestroySDK`. Use it as a copy-paste starting template when you outgrow the snippet above.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. `CAMERA` / `RECORD_AUDIO` permission denied — preview is black, no error

**Cause**: the manifest declares the permission but the runtime request was either skipped or denied, **or** pipeline construction fired before the user tapped "Allow". On API 23+ Android holds back the hardware until both the manifest declaration AND a runtime grant exist; the SDK gets back a null camera handle and silently produces a black preview (the `OnError` event does not fire because no GStreamer-equivalent error was raised — the source just yields no frames).

**Fix**: use the `OnRequestPermissionsResult` + `Interlocked` guard pattern shown above. Do NOT construct `MediaBlocksPipeline` from `OnCreate` — defer it until the grants land. If the user denied the permission, prompt again from a settings UI; "Don't ask again" requires sending the user to system Settings via `Intent.ActionApplicationDetailsSettings`.

### 2. `Java.Lang.ClassNotFoundException: org.freedesktop.gstreamer.GStreamer` (or one of the `Gst*Callback` classes)

**Cause**: the `AndroidDependency` `<ProjectReference>` is missing from the csproj. The `VisioForge.CrossPlatform.Core.Android` NuGet ships the native `.so` files but **not** the four Java glue classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`) — those live in the companion `AndroidDependency` project, which is `<AndroidJavaSource>`-compiled into your APK.

**Fix**: copy the `AndroidDependency/` folder from the samples repo (`https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency`) next to your app, add `<ProjectReference Include="...\AndroidDependency\VisioForge.Core.Android.X10.csproj" />` (or `X9.csproj` if you target `net9.0-android`), and rebuild. `dotnet build` regenerates the binding stubs from the Java sources automatically.

### 3. AAB upload to Play Console fails / device install errors out with missing-`.so` for an ABI

**Cause**: the `VisioForge.CrossPlatform.Core.Android` redist ships native libraries for `arm64-v8a` and `armeabi-v7a` (and `x86_64` for emulator). When packaging as **AAB** (Android App Bundle, the default for Play Store) Google Play does per-device split delivery — but **only if every ABI present in `<RuntimeIdentifiers>` has a complete set of `.so` files**. If your csproj sets `<RuntimeIdentifiers>android-arm64;android-arm;android-x86;android-x64</RuntimeIdentifiers>` you'll trip on `android-x86` (32-bit Intel) which the redist does not provide and which Play Store will reject with "Bundle contains native code, but does not contain code for ...".

**Fix**: drop `android-x86` from `<RuntimeIdentifiers>` (32-bit Intel Android devices are essentially extinct; the Play Store no longer requires it). Keep `android-arm64;android-arm;android-x64`. For local APK testing on a `x86_64` emulator, `android-x64` is enough; for Play Store production, `android-arm64;android-arm` is enough.

### 4. Pipeline `OnError` fires with "Element 'X' not found" or recording produces a 0-byte MP4

**Cause A — encoder picked a software path the redist doesn't ship**: `H264EncoderBlock` and `AACEncoderBlock` default to MediaCodec hardware encoders on Android. On a small minority of devices (older Snapdragon 4xx, some emulators) MediaCodec H.264 is unavailable and the engine falls back to a software encoder element which may not be in the Android redist's pruned plugin set.

**Cause B — `MP4SinkBlock` finalised before all encoders flushed**: stopping the pipeline with `StopAsync(force: true)` while a recording is active corrupts the MP4 moov atom because the encoder queues weren't drained. The upstream sample stops with `StopAsync(force: false)` specifically when ending a recording, and `force: true` only when reconfiguring before a fresh start.

**Fix**: for cause A, on devices that fail check `device.VideoFormats` enumeration in `Log.Info` to confirm the camera enumerates, then test on a different device — most modern Android devices (API 28+) ship MediaCodec H.264. For cause B, use `StopAsync(force: false)` when stopping a recording so the encoder/muxer gets a chance to drain, then `await PhotoGalleryHelper.AddVideoToGalleryAsync(filename)` to publish to the gallery (the sample shows this exact sequence in `btStartRecord_Click`'s stop branch).

### 5. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine surfaces `"SDK TRIAL period (30 days) is over."` via `OnError`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started. The latter trips up Android apps in particular because the upstream sample disposes and rebuilds the pipeline on every record toggle and camera switch fallback.

**Fix**: ship the licence as an Android asset (place under `Assets/license.vflicense`, Build Action `AndroidAsset`) and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` inside `CreateEngineAsync()` — every fresh `MediaBlocksPipeline` instance needs its own call before its `StartAsync`. Centralising the licence load in `CreateEngineAsync()` (rather than `OnCreate`) ensures every rebuild path stays licensed.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -f net10.0-android` succeeds with the bundled `references/` files copied in plus the `AndroidDependency/` folder in place (no missing `.so` warnings, no `ClassNotFoundException` at runtime).
- [ ] `dotnet build -t:Run -f net10.0-android` deploys to a connected device or emulator and shows a webcam preview within ~1 s after the user grants `CAMERA` + `RECORD_AUDIO`.
- [ ] Toggling Record / Stop produces an MP4 under `GetExternalFilesDir(Android.OS.Environment.DirectoryMovies)` that opens correctly in another player (validates `StopAsync(force: false)` finalised the moov atom).
- [ ] Switching front ↔ back camera during preview does not crash (live `SwitchCamera` + full-restart fallback both work).
- [ ] `OnDestroy` runs `DisposeAsync → DestroySDK` cleanly; relaunching the app shows a fresh preview, not a black screen.
- [ ] AAB packaging targets only `android-arm64;android-arm` (and optionally `android-x64` for emulator); `android-x86` is NOT in `<RuntimeIdentifiers>`.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called inside `CreateEngineAsync()` so every pipeline rebuild stays licensed (otherwise the app runs in 30-day trial mode and aborts after 30 days).

## Bundled references

The `references/` folder is a faithful copy of the upstream Simple Video Capture sample for Media Blocks — copy all of it into a fresh `net10.0-android` project folder, copy the `AndroidDependency/` folder from the samples repo into the parent solution, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-android` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph). Includes the `<ProjectReference>` to `AndroidDependency`.
- `references/AndroidManifest.xml` — minimal manifest with `INTERNET`, `CAMERA`, and `RECORD_AUDIO` declared.
- `references/MainActivity.cs` — full code-behind: `CreateEngineAsync()` builds source + renderer + audio source + audio renderer; `StartPreviewAsync()` wires source → renderer directly; `btStartRecord_Click` rebuilds the graph with `TeeBlock` + `H264EncoderBlock` + `AACEncoderBlock` + `MP4SinkBlock` (writing under `GetExternalFilesDir(DirectoryMovies)`); `btSwitchCam_Click` attempts a live `SwitchCamera` and falls back to a full pipeline restart on format mismatch; `OnDestroy` runs `DisposeAsync → DestroySDK`; permission gating uses `Interlocked.CompareExchange` so the start path runs at most once even on repeated `OnRequestPermissionsResult` callbacks. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Resources/layout/activity_main.xml` — Activity layout with `<VisioForge.Core.UI.Android.VideoViewGL />` plus Record and Switch-camera `ImageButton` controls.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android>
- **AndroidDependency project (required `<ProjectReference>`)**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-wpf` — same Media Blocks SDK on a Windows WPF host.
    - `video-capture-sdk-x-android` — high-level capture-and-record API on the same Android engine; use this if you don't need a custom pipeline topology.
    - **Cross-platform / mobile**:
        - `media-blocks-sdk-net-ios` — same SDK on a native iOS host.
        - `media-blocks-sdk-net-macos` — same SDK on a native macOS host.
        - `media-blocks-sdk-net-maui` — same SDK on .NET MAUI (one codebase across Android + iOS + Windows + Mac Catalyst).
        - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (cross-platform).
        - `media-blocks-sdk-net-uno` — same SDK on Uno (Windows + macOS + iOS + Android + Linux + browser).
