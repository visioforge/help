---
name: media-player-sdk-x-android
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a native .NET for Android application. Covers the Android-specific VideoView control, the cross-platform NuGet package, AndroidDependency project reference, license registration, and the most common Android pitfalls (missing INTERNET permission for streaming, AAB vs APK packaging, ABI filters, hardware decoder support, trial-period expiry / unlicensed build). Use for native .NET for Android playback apps — for cross-OS MAUI use media-player-sdk-x-maui.
---

# Media Player SDK X — native .NET for Android integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to a **native .NET for Android** application (TFM `net10.0-android`, Activity-based, NOT MAUI and NOT classic Xamarin.Android). The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level playback god-object (`MediaPlayerCoreX`) that mirrors the legacy `MediaPlayerCore` API. The same C# code works unchanged on WPF / MAUI / Avalonia / Uno / iOS / macOS — only the UI host swaps (`VideoViewTX` here, `<my:VideoView />` on MAUI, etc.) and the per-OS native redist NuGet.

Pinned NuGet versions: wrapper **`2026.5.4`**, Android redist **`2026.4.18.0`** (matches the [official MediaPlayer Android sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding local-file or network-stream (HTTP / HTTPS / RTSP / HLS / DASH) playback to a native .NET for Android app (`net10.0-android`, Activity-based).
- Implementing transport controls (play / pause / resume / stop), timeline scrubbing via `Position_Get` / `Position_Set` / `Duration`, and a bound `SeekBar` from a single `MediaPlayerCoreX` instance.
- Sharing a single C# playback layer across an Android app and one or more desktop / mobile companions (the `MediaPlayerCoreX` API is identical across platforms — only the host control changes).
- Hardware-accelerated H.264 / H.265 / VP8 / VP9 / AV1 decode via Android `MediaCodec` (the X engine binds the platform's `amcviddec-*` GStreamer elements automatically — no explicit configuration needed).

## When NOT to use this skill

- **MAUI app that targets Android + other OSes from one codebase**: use `media-blocks-sdk-net-maui` (graph-based, lower-level) — the conditional multi-target csproj and `AddVisioForgeHandlers()` registration are MAUI-specific and not covered here.
- **Custom pipeline topology** (multi-source mix, transcode without preview, runtime sink swap on Android, source-/sink-pad muxing with side audio): use `media-blocks-sdk-net-android` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.
- **Classic Xamarin.Android** (`MonoAndroid12.0` TFM, not `net10.0-android`): unsupported — Xamarin.Android went out of support in May 2024, migrate to `net8.0-android` or newer first.
- **Capturing from camera / microphone** (recording, not playback): `video-capture-sdk-x-android`.
- **iOS / macOS host instead of Android**: same SDK, different host → `media-player-sdk-x-ios` / `media-player-sdk-x-maui`.

## Project setup

### Target framework

Media Player SDK X 2026.x supports `net8.0-android`, `net9.0-android`, and `net10.0-android` for the native Android host. The upstream MediaPlayer sample pins **`net10.0-android`** with `<SupportedOSPlatformVersion>21</SupportedOSPlatformVersion>` (Android 5.0 / API level 21 minimum) — match these unless you have a reason not to. The player can target a lower minimum than the capture SDK (which floors at API 28) because playback uses fewer of the GStreamer elements that bumped their minimum SDK level over time.

### NuGet packages + AndroidDependency

A native Android playback project needs **two NuGet packages plus one ProjectReference**:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <PackageReference Include="Xamarin.Essentials" Version="1.8.0" />
</ItemGroup>
<ItemGroup>
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
```

`VisioForge.DotNet.MediaPlayer` is the wrapper package — both `MediaPlayerCore` (legacy, Windows-only) and `MediaPlayerCoreX` (cross-platform) ship in it. What switches you to the X engine on Android is the redist (`VisioForge.CrossPlatform.Core.Android`) plus the `AndroidDependency` project reference.

The `AndroidDependency` project is **not on NuGet** — it's a small companion csproj (`VisioForge.Core.Android.X10.csproj` for `net10.0-android`, or `X9.csproj` for `net9.0-android`) that ships in the samples repo and binds the underlying `.aar` plus a handful of Java callback classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`). Copy the entire `AndroidDependency/` folder from `https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency` next to your app and adjust the relative path in `<ProjectReference Include="...">`. The package alone is **not** enough — without the project reference, builds succeed but `MediaPlayerCoreX` aborts at construction time with a missing-Java-class error.

`Xamarin.Essentials 1.8.0` is used by the upstream sample for `FilePicker.PickAsync()` (the open-file dialog) and `Platform.Init` / `Platform.OnRequestPermissionsResult` plumbing. On `net10.0-android` you can substitute the built-in `AndroidX` storage-access framework if you prefer fewer dependencies — the SDK itself does not require Xamarin.Essentials.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official MediaPlayer sample (`Media Player SDK X/Android/MediaPlayer/MediaPlayer.csproj`) — kept verbatim except for stripping demo-only metadata; the bundled file builds standalone against the public NuGet packages plus the `AndroidDependency/` project copied in alongside.

### AndroidManifest permissions

`Properties/AndroidManifest.xml` (or root-level `AndroidManifest.xml`) MUST declare `INTERNET` (for network playback) and the appropriate storage / media permissions for the API levels you target:

```xml
<?xml version="1.0" encoding="utf-8"?>
<manifest xmlns:android="http://schemas.android.com/apk/res/android" package="com.example.player">
    <application android:allowBackup="true" android:icon="@mipmap/appicon" android:label="@string/app_name" android:supportsRtl="true"></application>
    <uses-permission android:name="android.permission.INTERNET" />
    <uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" />
    <uses-permission android:name="android.permission.READ_MEDIA_AUDIO" />
    <uses-permission android:name="android.permission.READ_MEDIA_IMAGES" />
    <uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
    <uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" />
</manifest>
```

`INTERNET` is required even for offline playback — the GStreamer redist binds a few network helpers internally. Omitting it produces a delayed, cryptic `SocketException` from inside `_player.OpenAsync()`. The granular `READ_MEDIA_AUDIO` / `READ_MEDIA_IMAGES` / `READ_MEDIA_VIDEO` permissions are required on API 33+ (Android 13); the legacy `READ_EXTERNAL_STORAGE` covers API 32 and below. Declare both — the system silently ignores the irrelevant ones per platform version.

### Layout — `VideoViewTX`

The Android-specific renderer for the player is `VisioForge.Core.UI.Android.VideoViewTX` (a `TextureView`-based control — distinct from `VideoViewGL` used by Video Capture SDK X). Drop it into your activity layout exactly like a stock view:

```xml
<!-- Resources/layout/activity_main.xml -->
<?xml version="1.0" encoding="utf-8"?>
<FrameLayout xmlns:android="http://schemas.android.com/apk/res/android"
    android:layout_width="match_parent"
    android:layout_height="match_parent"
    android:background="#000000">

    <VisioForge.Core.UI.Android.VideoViewTX
        android:layout_width="match_parent"
        android:layout_height="match_parent"
        android:layout_gravity="center"
        android:id="@+id/videoView" />
</FrameLayout>
```

Resolve it in code with `FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView)`. There is **no** Android-specific XML namespace declaration — the fully-qualified type name in the element is enough; the Mono/.NET-for-Android resource compiler binds it automatically. Note: `VideoViewTX` (TextureView-backed) is the correct host for **playback**; `VideoViewGL` (SurfaceView-backed OpenGL renderer) is for **capture** — they are not interchangeable across the two SDKs.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync` / `PlayAsync`:

```csharp
_player = new MediaPlayerCoreX(videoView);
_player.OnStart += _player_OnStart;

// Ship the licence as an Android asset (Build Action: AndroidAsset, file under Assets/license.vflicense).
using var assetStream = Assets.Open("license.vflicense");
using var ms = new MemoryStream();
await assetStream.CopyToAsync(ms);
await _player.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (Android asset, `getFilesDir()`, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper. If your app constructs multiple `MediaPlayerCoreX` instances (rare on Android — one per Activity is typical), each instance needs its own `SetLicenseCertificateAsync` call before its first `OpenAsync`.

The bundled `references/MainActivity.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the snippet above into `OnCreate` right after `_player = new MediaPlayerCoreX(videoView)`.

## Runtime permissions

For network-only playback (HTTP / HTTPS / RTSP / HLS / DASH) the manifest `INTERNET` declaration is sufficient — `INTERNET` is a normal permission and granted automatically. For **local-file** playback on API 23+ you DO need a runtime grant for storage / media access, requested via the standard `RequestPermissions` flow:

```csharp
protected override void OnCreate(Bundle? savedInstanceState)
{
    base.OnCreate(savedInstanceState);
    SetContentView(Resource.Layout.activity_main);
    Platform.Init(this, savedInstanceState);

    videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView);
    _player = new MediaPlayerCoreX(videoView);
    // ...wire buttons...
}

public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
    [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
{
    Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
}
```

Constructing `MediaPlayerCoreX` from `OnCreate` (without waiting for permission grants) is safe — unlike the capture SDK, the player only touches the file system when you call `OpenAsync(new Uri(filePath))`. If the user has not granted storage access at that point, `OpenAsync` throws and `OnError` fires; catch and prompt for permission. The simplest UX is to prompt for storage on the file-picker button click and only invoke `OpenAsync` after the grant lands.

`Xamarin.Essentials.FilePicker.PickAsync()` (used by the upstream sample's open-file button) handles the storage-permission prompt internally on API 33+ via the Android Photo Picker — no manual `RequestPermissions` call needed if you go through `FilePicker`.

## Hello-World playback

Minimum viable open-and-play snippet — a self-contained `MainActivity` you can drop into a fresh `net10.0-android` project. (For the full feature set incl. play / pause / stop / seek, copy `references/` into your project and skip this section.)

```csharp
// MainActivity.cs
using Android.Runtime;
using Android.Widget;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using Xamarin.Essentials;

namespace YourApp
{
    [Activity(Label = "@string/app_name", MainLauncher = true,
              Theme = "@android:style/Theme.NoTitleBar.Fullscreen")]
    public class MainActivity : Activity
    {
        private VisioForge.Core.UI.Android.VideoViewTX videoView;
        private MediaPlayerCoreX _player;

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Platform.Init(this, savedInstanceState);

            videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView);
            _player = new MediaPlayerCoreX(videoView);
            // For a purchased licence, add here:
            //   using var s = Assets.Open("license.vflicense");
            //   using var ms = new MemoryStream(); await s.CopyToAsync(ms);
            //   await _player.SetLicenseCertificateAsync(ms.ToArray());

            FindViewById<Button>(Resource.Id.btPlay).Click += async (_, _) =>
            {
                try
                {
                    await _player.OpenAsync(new Uri("https://media.example.com/sample.mp4"));
                    await _player.PlayAsync();
                }
                catch (Exception ex) { Android.Util.Log.Error("MainActivity", ex.ToString()); }
            };
        }

        protected override async void OnDestroy()
        {
            try { if (_player != null) { await _player.StopAsync(); await _player.DisposeAsync(); _player = null; } }
            catch (Exception ex) { Android.Util.Log.Error("MainActivity", ex.ToString()); }
            VisioForgeX.DestroySDK();
            base.OnDestroy();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions,
            [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}
```

Note: unlike WPF / MAUI, the Android host does **not** require an explicit `await VisioForgeX.InitSDKAsync()` before constructing `MediaPlayerCoreX` — the Android wrapper boots the engine on first use. You DO still need `VisioForgeX.DestroySDK()` on `OnDestroy` to release native resources cleanly between Activity recreations.

`references/MainActivity.cs` (paired with `references/Resources/layout/activity_main.xml`) ships the full pattern with file picker, play / pause toggle, stop, seekbar scrubbing via `Position_Set`, periodic position polling on a `System.Timers.Timer`, and `OnStart` event handling.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. Network playback fails with `SocketException` despite manifest `INTERNET`

**Cause**: Android 9.0+ (API 28) blocks **cleartext HTTP** by default — only HTTPS works. If you call `_player.OpenAsync(new Uri("http://..."))` against a non-TLS endpoint, the Android network stack rejects the connection before the GStreamer source element ever opens it, and the player surfaces a `java.net.UnknownServiceException: CLEARTEXT communication ... not permitted` wrapped as a `SocketException`.

**Fix**: prefer HTTPS sources. If you genuinely need HTTP (RTSP-over-HTTP tunnelling, intranet test stream), add `android:usesCleartextTraffic="true"` to the `<application>` element in `AndroidManifest.xml`, or ship a `network-security-config.xml` that whitelists only the specific cleartext hosts you trust. Note: RTSP itself (`rtsp://`) is NOT cleartext-blocked because it doesn't use the Android HTTP stack — only HTTP / HTTPS URLs are subject to this rule.

### 2. `Java.Lang.ClassNotFoundException: org.freedesktop.gstreamer.GStreamer` (or one of the `Gst*Callback` classes)

**Cause**: the `AndroidDependency` `<ProjectReference>` is missing from the csproj. The `VisioForge.CrossPlatform.Core.Android` NuGet ships the native `.so` files but **not** the four Java glue classes (`GStreamer.java`, `GstAhcCallback.java`, `GstAhsCallback.java`, `GstAmcOnFrameAvailableListener.java`) — those live in the companion `AndroidDependency` project, which is `<AndroidJavaSource>`-compiled into your APK.

**Fix**: copy the `AndroidDependency/` folder from the samples repo (`https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency`) next to your app, add `<ProjectReference Include="...\AndroidDependency\VisioForge.Core.Android.X10.csproj" />` (or `X9.csproj` if you target `net9.0-android`), and rebuild. `dotnet build` regenerates the binding stubs from the Java sources automatically.

### 3. AAB upload to Play Console fails / device install errors out with missing-`.so` for an ABI

**Cause**: the `VisioForge.CrossPlatform.Core.Android` redist ships native libraries for `arm64-v8a` and `armeabi-v7a` (and `x86_64` for emulator). When packaging as **AAB** (Android App Bundle, the default for Play Store) Google Play does per-device split delivery — but **only if every ABI present in `<RuntimeIdentifiers>` has a complete set of `.so` files**. If your csproj sets `<RuntimeIdentifiers>android-arm64;android-arm;android-x86;android-x64</RuntimeIdentifiers>` you'll trip on `android-x86` (32-bit Intel) which the redist does not provide and which Play Store will reject with "Bundle contains native code, but does not contain code for ...".

**Fix**: drop `android-x86` from `<RuntimeIdentifiers>` (32-bit Intel Android devices are essentially extinct; the Play Store no longer requires it). Keep `android-arm64;android-arm;android-x64`. For local APK testing on a `x86_64` emulator, `android-x64` is enough; for Play Store production, `android-arm64;android-arm` is enough.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."` surfaced via `OnError`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being started.

**Fix**: ship the licence as an Android asset (place under `Assets/license.vflicense`, Build Action `AndroidAsset`), open with `Assets.Open(...)`, copy to `byte[]`, and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before the first `OpenAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 5. Hardware-decoder fallback to software produces stutter on H.265 / HEVC content

**Cause**: GStreamer auto-selects a decoder via `decodebin` rank ordering. The Android hardware decoders (`amcviddec-omxgooglehevcdecoder`, `amcviddec-c2googlehevcdecoder`, vendor-specific ones like `amcviddec-omxqcomvideodecoderhevc`) ship with the X redist but their availability depends on the device's Codec2 / OMX implementation. On older devices missing a HEVC HW decoder, `decodebin` falls back to the software `avdec_h265` (libav) — playable but CPU-bound and prone to drops on 4K content.

**Fix**: this is a device-capability limitation, not a fixable bug — there is no way for the app to *create* a hardware decoder that the device does not expose. To detect and degrade gracefully, subscribe to `OnError` (or `OnStart` followed by frame-rate sampling) and either downgrade the source variant (HLS bitrate ladder) or warn the user. To force software for testing, use `MediaPlayerCoreX.Debug_Mode = true` and inspect the logged element graph for `amcviddec-*` vs `avdec_*` selection. Avoid setting per-element ranks at runtime on Android — the ranks are global and can break unrelated pipelines in the same process.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -f net10.0-android` succeeds with the bundled `references/` files copied in plus the `AndroidDependency/` folder in place (no missing `.so` warnings, no `ClassNotFoundException` at runtime).
- [ ] `dotnet build -t:Run -f net10.0-android` deploys to a connected device or emulator and plays the upstream sample's selected file end-to-end.
- [ ] Network playback works against an HTTPS source (e.g. `https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4`) without `SocketException`.
- [ ] Local-file playback works after `FilePicker.PickAsync()` returns a path (storage permission was implicitly granted by the picker on API 33+).
- [ ] Seekbar scrubbing via `Position_Set` produces a near-instant frame update (not a multi-second stall) — confirms the source supports seek and `decodebin` chose the correct demuxer.
- [ ] `OnDestroy` runs `StopAsync → DisposeAsync → DestroySDK` cleanly; relaunching the app shows a fresh `VideoViewTX`, not a black screen.
- [ ] AAB packaging targets only `android-arm64;android-arm` (and optionally `android-x64` for emulator); `android-x86` is NOT in `<RuntimeIdentifiers>`.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on the `MediaPlayerCoreX` instance before the first `OpenAsync` (otherwise the app runs in 30-day trial mode and aborts after 30 days).

## Bundled references

The `references/` folder is a faithful copy of the upstream MediaPlayer Android sample — copy all of it into a fresh `net10.0-android` project folder, copy the `AndroidDependency/` folder from the samples repo into the parent solution, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-android` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph). Includes the `<ProjectReference>` to `AndroidDependency`.
- `references/AndroidManifest.xml` — minimal manifest with `INTERNET`, legacy storage, and granular `READ_MEDIA_*` (API 33+) permissions declared.
- `references/MainActivity.cs` — full code-behind: file picker via `Xamarin.Essentials.FilePicker`, play / pause toggle, stop, seekbar scrubbing via `Position_Set`, periodic position polling on a `System.Timers.Timer`, system-bar inset handling for edge-to-edge layouts, and `OnStart` event wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Resources/layout/activity_main.xml` — Activity layout with `<VisioForge.Core.UI.Android.VideoViewTX />` plus URL `EditText`, open-file `ImageButton`, play / stop `ImageButton` controls, timeline `SeekBar`, and a position `TextView`.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android>
- **AndroidDependency project (required `<ProjectReference>`)**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/AndroidDependency>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-x-wpf` — same X SDK on a Windows WPF host.
    - `media-player-sdk-x-winui` — same X SDK on a Windows WinUI 3 host.
    - `media-player-sdk-x-winforms` — same X SDK on a Windows WinForms host.
    - `media-player-sdk-x-avalonia` — same X SDK on an Avalonia host (Windows / Linux / macOS).
    - `video-capture-sdk-x-android` — sibling capture SDK on the same Android host (different `VideoView` control: `VideoViewGL` instead of `VideoViewTX`).
    - **Cross-platform / mobile**:
        - `media-player-sdk-x-ios` — same X SDK on a native iOS host.
        - `media-player-sdk-x-macos` — same X SDK on a native macOS host.
        - `media-player-sdk-x-maui` — same X SDK on .NET MAUI (one codebase across Android + iOS + Windows + Mac Catalyst).
        - `media-blocks-sdk-net-android` — lower-level graph-based engine on Android for custom pipeline topologies.
