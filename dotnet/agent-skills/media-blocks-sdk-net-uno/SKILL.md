---
name: media-blocks-sdk-net-uno
description: Integrate VisioForge Media Blocks SDK into an Uno Platform application. Covers the graph-based pipeline model, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (camera permissions per OS, WebAssembly limitations, missing native libs, trial-period expiry / unlicensed build). Use for Uno cross-OS pipelines (Windows, Android, iOS, macOS) — for MAUI use media-blocks-sdk-net-maui, for Avalonia use media-blocks-sdk-net-avalonia.
---

# Media Blocks SDK .NET — Uno Platform integration

This skill helps you add **VisioForge Media Blocks SDK .NET** — the graph-based pipeline SDK — to an **Uno Platform** application that targets Windows, Android, and Mac Catalyst (and optionally iOS) from a single codebase. Media Blocks composes a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. Compared to the higher-level `VideoCaptureCoreX` god-object, Media Blocks gives you full control over the topology — splitting streams with tees, mixing sources, transcoding without preview, swapping sinks at runtime — at the cost of having to wire every edge yourself.

The same C# pipeline code runs unchanged on every Uno target; what differs is the per-OS native redist NuGet, the permission paperwork, and the `Uno.Sdk` MSBuild SDK plus the `VisioForge.DotNet.Core.UI.Uno` `VideoView` control. Under the hood the engine is GStreamer-based and shared with `VideoCaptureCoreX`.

Pinned NuGet versions (match the bundled `references/Sample.csproj` and the upstream Uno `VideoCaptureUnoX` sample): wrapper **`2026.5.4`**, Uno UI **`2026.5.4`**, Windows redists **`2026.4.29`**, Android redist **`2026.4.18.0`**, iOS redist **`2025.0.16`**, Mac Catalyst redist **`2025.9.1`**. Newer 2026.x.x patch versions are usually drop-in compatible — keep the wrapper and `VisioForge.DotNet.Core.UI.Uno` on the same version, and pin the per-OS redists to the values from the upstream csproj for your wrapper version. Mismatches between wrapper and redist are undefined behaviour and surface as `DllNotFoundException` or `Element 'X' not found` errors at pipeline start.

## When to use this skill

- Single Uno codebase running a **custom pipeline** that the high-level `VideoCaptureCoreX` doesn't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capture → encode → mux to MP4 / MPEG-TS / WebM with explicit control over each stage, on Windows / Android / Mac Catalyst (and iOS) from one project.
- Network streaming sinks (RTSP server, SRT, RIST, NDI, WebRTC WHIP, YouTube/Facebook RTMP) wired into a custom graph on cross-platform Uno heads.
- Sharing the same `MediaBlocksPipeline` graph code between an Uno app and other VisioForge hosts (WPF, MAUI, Avalonia, WinUI) — the block API is identical across all of them; only the `VideoView` and the host SDK differ.

## When NOT to use this skill

- **Plain webcam capture** on Uno (preview + record to MP4, no custom topology): `video-capture-sdk-x-uno` is dramatically less code, with the same multi-target setup.
- **MAUI host** (XAML + handlers, MAUI Essentials permissions): `media-blocks-sdk-net-maui`. Block API is identical — only the UI host package and permission helpers differ.
- **Avalonia host** (Linux as a first-class target, no iOS): `media-blocks-sdk-net-avalonia`.
- **WPF host** (Windows-only): `media-blocks-sdk-net-wpf`.
- **WebAssembly target**: not currently supported by the Media Blocks engine. The native runtime is GStreamer-based and there is no WASM build of the redist. Uno's `-browserwasm` head builds compile (the wrapper is managed) but `VisioForgeX.InitSDKAsync()` fails at runtime because the native library isn't there. Don't add a `-browserwasm` TFM to the pipeline project — split web UI off into a separate Uno head that doesn't reference Media Blocks.

## NuGet packages (cross-platform layout)

Two managed packages always, plus per-OS native redists conditionally. The conditional `<ItemGroup>` blocks are not optional — every target framework you build needs its matching native runtime, otherwise the app builds but crashes the first time `VisioForgeX.InitSDKAsync()` runs.

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed wrapper (`MediaBlocksPipeline`, all blocks) | Always |
| `VisioForge.DotNet.Core.UI.Uno` | Uno `VideoView` control | Always |
| `Microsoft.WindowsAppSDK` | WinAppSDK runtime for the Windows head | `-windows` |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav muxers + encoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` is bound through a small companion csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

Unlike the WPF Media Blocks layout (where `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` ships compressed binaries), the Uno sample uses the un-UPX'd `VisioForge.CrossPlatform.Libav.Windows.x64`. UPX-compressed natives interact badly with some MSIX packaging flows on the WinAppSDK head — keep the non-UPX variant for Uno.

The Android target additionally needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` (a small companion project that ships in the samples repo under `AndroidDependency/`) — that's how the `.aar` is bound. Copy that folder into your repo and reference it as shown in the bundled csproj. There's no NuGet equivalent today.

## Project setup

### Uno workload

Make sure the Uno templates and workloads are installed. The Uno project type uses `Sdk="Uno.Sdk"` (not `Microsoft.NET.Sdk`), and that SDK pulls in the right per-OS workloads automatically when present. If you're starting fresh:

```bash
dotnet workload install android ios maccatalyst maui  # workloads needed for each TFM
dotnet new install Uno.Templates                       # Uno project templates
```

Without the matching workload, `dotnet build -f net10.0-android` (etc.) fails with `error NETSDK1147: To build this project, the following workloads must be installed`.

### Multi-target csproj

The csproj declares target frameworks **conditionally per host OS**: Android always; Mac Catalyst only on macOS hosts; Windows only on Windows hosts. iOS is intentionally absent from the upstream Uno sample (Uno's iOS head usually lives in a separate solution; if you do need it, add `net10.0-ios` to the macOS condition and reference `VisioForge.CrossPlatform.Core.iOS`). That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights: `<Project Sdk="Uno.Sdk">`, `<UnoSingleProject>true</UnoSingleProject>`, `<RunAOTCompilation>false</RunAOTCompilation>`, conditional `<TargetFrameworks>` per host OS, and conditional `<ItemGroup>` blocks pulling in the right per-OS native redists.

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure.

`<RunAOTCompilation>false</RunAOTCompilation>` is required — the native-interop layer is reflection-heavy in places and doesn't survive AOT trimming today. The upstream sample explicitly turns it off; so should you.

### Uno "single project" structure

The upstream Media Blocks Uno sample uses **`UnoSingleProject` mode** which collapses the per-OS heads into a single multi-targeted csproj. That's the layout reflected in `references/Sample.csproj`. If your existing solution still uses the older multi-head layout, add the VisioForge package references and conditional redist blocks to **the head project that builds for that target**, not to the shared library.

Uno's `App` class is plain WinUI / WinAppSDK — there is **no MAUI-style handler registration**. The `<vf:VideoView />` control wires itself up through its own `xmlns` declaration in XAML, so nothing needs to be done in `App.xaml.cs` beyond what the Uno template generates. See `references/App.xaml.cs` for the unchanged template.

## Pipeline model

This is the core mental shift from `VideoCaptureCoreX`. There is no `_core.StartAsync()` god-object — you build a directed graph of blocks and run it. Four concepts:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, …) produce media on output pads. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, …) accept on input pads, produce on output pads.
3. **Sink blocks** terminate the graph — renderer sinks drive UI/speakers (`VideoRendererBlock` bound to an Uno `VideoView`, `AudioRendererBlock`); file/network sinks write/transmit (`MP4SinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, …). Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
4. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `StartAsync`.

Topology used by the bundled `references/MainPage.xaml.cs` capture path:

```text
SystemVideoSourceBlock  --video-->  TeeBlock(video)  --[0]-->  VideoRendererBlock (VideoView)
                                                     --[1]-->  H264EncoderBlock --> MP4SinkBlock(video pad)
SystemAudioSourceBlock  --audio-->  TeeBlock(audio)  --[0]-->  AudioRendererBlock (desktop only)
                                                     --[1]-->  AACEncoderBlock  --> MP4SinkBlock(audio pad)
```

`MP4SinkBlock` is the same instance on both paths — its dynamic inputs collect the streams to mux. On mobile (`__ANDROID__`, `__IOS__ && !__MACCATALYST__`) the `AudioRendererBlock` branch is disabled to avoid feedback loops.

## Per-platform permissions

Cross-platform pipelines mean cross-platform permission paperwork. Each OS has its own dialect — same as for Video Capture SDK X.

### Android

`Platforms/Android/AndroidManifest.xml` must declare `CAMERA` and `RECORD_AUDIO`. Storage entries are needed only if you save recordings to public folders on older API levels:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.WRITE_EXTERNAL_STORAGE" android:maxSdkVersion="28" />
<uses-permission android:name="android.permission.READ_EXTERNAL_STORAGE" android:maxSdkVersion="32" />
<uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

Uno does not ship a MAUI-Essentials-style `Permissions.RequestAsync<>` helper. Use AndroidX directly (`ContextCompat.CheckSelfPermission` + `ActivityCompat.RequestPermissions`) — `references/MainPage.xaml.cs` shows the canonical `EnsureAndroidPermissionsAsync` shape. On Android 10+ (API 29+) write recordings to the app-specific external dir (`GetExternalFilesDir(DirectoryMovies)`) — no storage permission needed under scoped storage.

### iOS

`Platforms/iOS/Info.plist` MUST contain non-empty user-facing strings. App Store review rejects builds without them, and **iOS terminates the app** the first time you touch the camera/mic without the matching key:

```xml
<key>NSCameraUsageDescription</key><string>Camera access is required to capture video.</string>
<key>NSMicrophoneUsageDescription</key><string>Microphone access is required to capture audio.</string>
<key>NSPhotoLibraryAddUsageDescription</key><string>Photo library access is required to save captured videos.</string>
```

Request the runtime permission via `AVCaptureDevice.RequestAccessForMediaTypeAsync` — see the `EnsureApplePermissionsAsync` method in `references/MainPage.xaml.cs`.

### Mac Catalyst

`Platforms/MacCatalyst/Info.plist` needs the same `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` strings as iOS. **Plus** `Platforms/MacCatalyst/Entitlements.plist` must enable the camera/microphone entitlements **and disable the App Sandbox** if you write recordings outside the container (the upstream sample writes to `~/Movies`):

```xml
<!-- Entitlements.plist -->
<key>com.apple.security.app-sandbox</key>          <false/>
<key>com.apple.security.device.camera</key>        <true/>
<key>com.apple.security.device.audio-input</key>   <true/>
```

Sandbox-on Mac Catalyst is supported too — write recordings to `Environment.SpecialFolder.MyVideos` (which lives inside the app container) instead of `~/Movies`.

### Windows

No project-side permission paperwork for the Windows head. The system camera privacy switch in **Settings → Privacy & security → Camera** is enforced by Windows itself. The upstream sample additionally calls `MediaCapture.RequestAccessAsync` once at startup to surface the Windows consent UI — see `EnsureWindowsPermissionsAsync` in `references/MainPage.xaml.cs`.

## Mandatory engine boot

Before any block construction (and before any `DeviceEnumerator` query), call `VisioForgeX.InitSDK()` (or `await VisioForgeX.InitSDKAsync()`) exactly once per process. The upstream `references/MainPage.xaml.cs` uses the synchronous `InitSDK()` from the page constructor — which is fine on Uno because the boot is fast on a warm registry and the constructor runs on the UI thread before the first awaited `Loaded` handler — and mirrors it with `VisioForgeX.DestroySDK()` from `Unloaded` (after `StopAllAsync`).

The `MediaBlocksPipeline` itself is created lazily inside the start handlers (`btStartPreview_Clicked` / `btStartCapture_Clicked` → `CreateEngine()`), not in `Loaded` — that lets the user re-arm a fresh pipeline after Stop without leaking the previous instance. The Loaded handler should run the platform permission check **first** (`EnsureApplePermissionsAsync` / `EnsureAndroidPermissionsAsync` / `EnsureWindowsPermissionsAsync`) and only then enumerate devices via `DeviceEnumerator.Shared.VideoSourcesAsync()` etc. — `DeviceEnumerator` probes hardware on iOS / mac and will return empty on a cold first launch without permission.

Skipping `InitSDK` is the #1 source of "DLL not found" / "no element X" failures on first run. First boot on a fresh machine takes 2-5 s on desktop / up to ~10 s on Android; subsequent launches are instant.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += Core_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _pipeline.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. Every `MediaBlocksPipeline` instance in the process needs its own call before its `StartAsync` — and because the bundled sample re-creates the pipeline each time you click PREVIEW/CAPTURE (via `CreateEngine()`), the licence call needs to live inside `CreateEngine()`, not in `Loaded`.

The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes` works on Windows / Mac Catalyst (sandbox off), but on iOS / Android the working directory is the app bundle, not your dev machine. The portable approach for an Uno head is to ship the licence as a **`Content`** item with `CopyToOutputDirectory=PreserveNewest` and read it with `Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///license.vflicense"))` — that URI scheme works on every Uno target including Android / iOS. (On the Windows head you can also use `Package.Current.InstalledLocation`.) Where the bytes come from (asset, env var, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

The bundled `references/MainPage.xaml.cs` runs in trial mode by design; add the two lines above into `CreateEngine()` right after `_pipeline = new MediaBlocksPipeline();` to register a purchased licence.

## Hello-World pipeline

The XAML is a single-line `<vf:VideoView />` plus a Start button (see `references/MainPage.xaml`). The minimum viable preview-only handler — assuming `VisioForgeX.InitSDK()` already ran from the page constructor and the platform permission helper has returned `true`:

```csharp
private async void StartButton_Click(object sender, RoutedEventArgs e)
{
    // async-void handlers MUST catch — otherwise an exception escapes to
    // AppDomain.UnhandledException and silently terminates the app. Common
    // triggers on first run: permission denied, trial expired, missing
    // native libs, no devices.
    try
    {
        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0) { /* "no camera" UI */ return; }

        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, args) => System.Diagnostics.Debug.WriteLine(args.Message);

        // For a purchased licence, add these two lines here:
        //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
        //   await _pipeline.SetLicenseCertificateAsync(cert);

        // VideoRendererBlock binds to the Uno VideoView at construction time.
        // IsSync = false is the standard preview setting — frames render as
        // fast as they arrive instead of being clock-throttled.
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView) { IsSync = false };

        var device = devices[0];
        var format = device.GetHDVideoFormatAndFrameRate(out var frameRate);
        var settings = new VideoCaptureDeviceSourceSettings(device) { Format = format.ToFormat() };
        settings.Format.FrameRate = frameRate;

        _videoSource = new SystemVideoSourceBlock(settings);
        _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);

        await _pipeline.StartAsync();
    }
    catch (Exception ex) { System.Diagnostics.Debug.WriteLine($"Start failed: {ex.Message}"); }
}
```

`references/MainPage.xaml.cs` extends this with per-OS permission helpers, audio source/output, the tee + encoder + MP4 muxer record path, per-platform output paths (Android scoped storage, iOS Library, Mac Catalyst `~/Movies`, Windows `MyVideos`), gallery save on iOS, pause/resume, and `OnError` wiring. Use it as a copy-paste starting template; trim the branches you don't need.

## Common cross-platform pitfalls

### 1. Black `<vf:VideoView />`, no errors logged

**Cause**: pipeline started before the Uno `VideoView` element has a valid handle (e.g. `StartAsync` from the page constructor instead of a button click), **or** `VideoRendererBlock` was constructed before `VisioForgeX.InitSDK` returned, **or** the wrong native redist was referenced for the build's TFM.

**Fix**: defer `VideoRendererBlock` construction and `pipeline.StartAsync` to a button click (or at minimum to `Loaded`), the way the bundled sample does in `btStartPreview_Clicked`. Keep `InitSDK` in the page constructor.

### 2. `DllNotFoundException` / `dlopen` failure / `OnError` "Element 'X' not found" on first start

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`, or the encoder/muxer block in the graph needs a redist that isn't referenced for the current TFM. Common slips:

- Windows head: forgot `VisioForge.CrossPlatform.Core.Windows.x64` or `VisioForge.CrossPlatform.Libav.Windows.x64` — `MP4SinkBlock` / `H264EncoderBlock` / `AACEncoderBlock` all need the libav package; most "missing element" errors trace back to it. `WebMSinkBlock` + `VPXEncoderBlock` + `VorbisEncoderBlock` need both core and libav.
- Android head: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough.
- Mac Catalyst head: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional `<ItemGroup>` and the Mac Catalyst `<Target>` matters. For specialised sinks (NDI, Decklink, WHIP) check the matching upstream sample's csproj for additional redist packages.

### 3. `VisioForgeX.InitSDK` works locally but fails in `-browserwasm` head

**Cause**: WebAssembly is not a supported target for the Media Blocks engine. There is no WASM build of the GStreamer-based native runtime, so `InitSDK` cannot find `libgstcore` (or its WASM equivalent) and throws `DllNotFoundException`.

**Fix**: split the WebAssembly UI off into a separate Uno head that does **not** reference `VisioForge.DotNet.MediaBlocks` / `VisioForge.DotNet.Core.UI.Uno`. The pipeline project should multi-target only `-windows`, `-android`, `-ios`, and `-maccatalyst`. If you accidentally added `net10.0-browserwasm` to `<TargetFrameworks>`, remove it.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started. The bundled sample re-creates the pipeline on every PREVIEW/CAPTURE click — if you put the licence call in `Loaded` it will be applied to a stale pipeline and the next click will run unlicensed.

**Fix**: read the `.vflicense` (shipped as `Content` / `ms-appx:///` resource on iOS / Android, see "License registration") and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` inside `CreateEngine()` after `_pipeline = new MediaBlocksPipeline();` and before any `StartAsync`. Every pipeline instance in the process needs its own call.

### 5. iOS first run silently terminates as soon as a source block opens the camera

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Platforms/iOS/Info.plist`. iOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no managed crash log entry from your code.

**Fix**: add both keys with non-empty user-facing strings (see "Per-platform permissions"). For Mac Catalyst, also confirm `Entitlements.plist` enables `com.apple.security.device.camera` and `com.apple.security.device.audio-input`.

### 6. Pipeline deadlocks on stop / disposal crashes mid-flush

**Cause**: calling `_pipeline.Stop()` (synchronous overload) or `Dispose()` from the UI thread deadlocks because the `VideoRendererBlock` needs the UI thread to release the last frame. Or: disposing source / encoder / sink blocks **before** `await _pipeline.DisposeAsync()` returns crashes the worker threads mid-flush.

**Fix**: copy the `StopAllAsync` shape from `references/MainPage.xaml.cs` verbatim — `pipeline.StopAsync` → `pipeline.DisposeAsync` → null the pipeline → dispose the individual block instances last. Always use the async forms.

## Verification checklist

- [ ] Uno workloads installed: `dotnet workload list` shows `android`, plus `maccatalyst` on macOS / `wasm-tools` if you have a (separate) WASM head.
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041` succeeds against the bundled `references/`.
- [ ] On Windows host: `dotnet build -f net10.0-android` succeeds.
- [ ] On macOS host: `dotnet build -f net10.0-maccatalyst` succeeds; the `.app/Contents/MonoBundle/` folder contains `.dylib` files after the build.
- [ ] First run on a fresh machine takes 2-5 s during `VisioForgeX.InitSDK()` (registry build); second run is instant. Android first launch can take ~10 s.
- [ ] App launches on each target OS and shows the camera preview within ~2 s after permission grant.
- [ ] Stopping and restarting the pipeline does not leak — `StopAllAsync` follows the `StopAsync → DisposeAsync → null pipeline → dispose blocks` order.
- [ ] If recording: the MP4 is finalised when `StopAsync` runs to completion before `DisposeAsync` (verify the file plays back in another player).
- [ ] On clean shutdown, `Unloaded` runs `StopAllAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called inside `CreateEngine()` on every `MediaBlocksPipeline` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `Uno.Sdk` project folder, set up `Platforms/` per the standard Uno template, and `dotnet build` succeeds with no extra files needed (apart from the `AndroidDependency/VisioForge.Core.Android.X10.csproj` companion project, which lives a few directory levels up — adjust the `<ProjectReference>` path to match your repo layout).

- `references/Sample.csproj` — multi-target Uno csproj with all per-OS conditional package references and the Mac Catalyst native-copy `<Target>`. Version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro).
- `references/App.xaml` + `references/App.xaml.cs` — Uno Application entry point (unchanged from the Uno template; no VisioForge-specific registration needed).
- `references/MainPage.xaml` — XAML with `<vf:VideoView />` and camera/mic/speaker/preview/capture/stop buttons.
- `references/MainPage.xaml.cs` — full code-behind: per-OS permission helpers (Android, iOS, Mac Catalyst, Windows), `InitSDK` boot, `DeviceEnumerator` wiring, `MediaBlocksPipeline` construction (`CreateEngine`), preview-only graph (`ConfigurePreviewAsync`), tee + encoder + MP4 muxer capture graph (`ConfigureCapture`), per-platform output-path logic (Android scoped storage, iOS Library, Mac Catalyst `~/Movies`, Windows `MyVideos`), gallery save on iOS, pause/resume, `StopAllAsync` shutdown, and `OnError` wiring. Runs in 30-day trial mode by design — add a `SetLicenseCertificateAsync` call inside `CreateEngine()` when integrating a purchased licence.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Uno>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-uno` — high-level capture-and-record API on Uno; use this if you don't need a custom pipeline.
    - `media-blocks-sdk-net-wpf` — same Media Blocks SDK on WPF (Windows-only host, single TFM, no per-OS conditionals).
    - **Other cross-platform hosts**:
        - `media-blocks-sdk-net-maui` — same Media Blocks SDK on .NET MAUI.
        - `media-blocks-sdk-net-avalonia` — same Media Blocks SDK on Avalonia (broader Linux support, no iOS).
