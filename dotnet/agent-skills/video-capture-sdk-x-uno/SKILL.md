---
name: video-capture-sdk-x-uno
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into an Uno Platform application. Covers the Uno-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (camera permissions per OS, WebAssembly limitations, missing native libs, trial-period expiry / unlicensed build). Use for Uno cross-OS apps (Windows, Android, iOS, macOS, WebAssembly) — for MAUI use video-capture-sdk-x-maui, for Avalonia use video-capture-sdk-x-avalonia.
---

# Video Capture SDK X — Uno Platform integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to an **Uno Platform** application that targets Windows, Android, iOS, and macOS (Mac Catalyst) from a single codebase. The same C# capture/recording code runs unchanged on every target; what differs is the per-OS native redist NuGet, the permission paperwork, and (in Uno's case) the `Uno.Sdk` MSBuild SDK plus the `VisioForge.DotNet.Core.UI.Uno` `VideoView` control.

`VideoCaptureCoreX` is the high-level capture-and-record god-object — same API as on the WPF/MAUI/WinUI hosts, just bound to Uno's `VideoView`. Under the hood it shares the GStreamer-backed engine with Media Blocks.

Pinned NuGet versions (match the bundled `references/Sample.csproj` and the official Uno Simple Capture sample): wrapper **`2026.5.4`**, Uno UI **`2026.5.4`**, Windows redists **`2026.4.29`**, Android redist **`2026.4.18.0`**, iOS redist **`2025.0.16`**, Mac Catalyst redist **`2025.9.1`**. Newer 2026.x.x patch versions are usually drop-in compatible — keep the wrapper and `VisioForge.DotNet.Core.UI.Uno` on the same version, and pin the per-OS redists to the values from the upstream csproj for your wrapper version.

## When to use this skill

- Single Uno codebase that runs on **Windows (WinAppSDK / WinUI 3), Android, iOS, and Mac Catalyst** with webcam / IP-camera / screen capture and recording.
- Recording captured video/audio to MP4 (incl. fragmented), MOV, MPEG-TS, AVI, or WebM from one set of source files.
- Sharing capture/recording code between an Uno app and other VisioForge X hosts (WPF, MAUI, Avalonia, WinUI) — `VideoCaptureCoreX` is identical across all of them.
- Uno-specific UX: native `Uno.Sdk` "single-project" head, XAML-based UI, hot-reload via `MainWindow.UseStudio()`.

## When NOT to use this skill

- **MAUI host** (XAML + handlers, MAUI Essentials permissions): use `video-capture-sdk-x-maui`. API is identical — only the UI host package and permission helpers differ.
- **Avalonia host** (Linux as a first-class target, no iOS): use `video-capture-sdk-x-avalonia`.
- **WinUI 3 host without Uno** (Windows-only WinAppSDK, single TFM): use `video-capture-sdk-x-winui` — simpler csproj, no per-OS conditionals.
- **WPF host** (Windows-only, .NET Framework / .NET on Windows): use `video-capture-sdk-x-wpf`.
- **WebAssembly target**: not currently supported by the X engine. The native runtime is GStreamer-based and there is no WASM build of the redist. Uno's `-browserwasm` head builds compile (the wrapper is managed) but `VisioForgeX.InitSDKAsync()` fails at runtime because the native library isn't there. Don't add a `-browserwasm` TFM to the capture project — split web UI off into a separate Uno head that doesn't reference the capture SDK.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): use `media-blocks-sdk-net-uno` — `VideoCaptureCoreX` wraps the same engine behind a fixed-shape graph.

## NuGet packages (cross-platform layout)

Two managed packages always, plus per-OS native redists conditionally. The conditional `<ItemGroup>` blocks are not optional — every target framework you build needs its matching native runtime, otherwise the app builds but crashes the first time `VisioForgeX.InitSDKAsync()` runs.

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.VideoCapture` | Managed wrapper (`VideoCaptureCoreX`, types) | Always |
| `VisioForge.DotNet.Core.UI.Uno` | Uno `VideoView` control | Always |
| `Microsoft.WindowsAppSDK` | WinAppSDK runtime for the Windows head | `-windows` |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav muxers + encoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` is bound through a small companion csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

The Android target additionally needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` (a small companion project that ships in the samples repo under `AndroidDependency/`) — that's how the `.aar` is bound. Copy that folder into your repo and reference it as shown in the bundled csproj. There's no NuGet equivalent today.

## Project setup

### Uno workload

Make sure the Uno templates and workloads are installed. The Uno project type uses `Sdk="Uno.Sdk"` (not `Microsoft.NET.Sdk`), and that SDK pulls in the right per-OS workloads automatically when present. If you're starting fresh:

```bash
dotnet workload install android ios maccatalyst maui  # workloads needed for each TFM
dotnet new install Uno.Templates                        # Uno project templates
```

Without the matching workload, `dotnet build -f net10.0-android` (etc.) fails with `error NETSDK1147: To build this project, the following workloads must be installed`.

### Multi-target csproj

The csproj declares target frameworks **conditionally per host OS**: Android always; Mac Catalyst only on macOS hosts; Windows only on Windows hosts. iOS is intentionally absent from the upstream Uno sample (Uno's iOS head usually lives in a separate solution; if you do need it, add `net10.0-ios` to the macOS condition and reference `VisioForge.CrossPlatform.Core.iOS`). That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-maccatalyst;net10.0-android</TargetFrameworks>
    <UnoSingleProject>true</UnoSingleProject>
    <RunAOTCompilation>false</RunAOTCompilation>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="VisioForge.DotNet.VideoCapture"   Version="2026.5.4" />
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno"    Version="2026.5.4" />
  </ItemGroup>
</Project>
```

The conditional native-redist `<ItemGroup>` blocks pull in the right per-OS packages — see `references/Sample.csproj` for the full set.

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure.

`<RunAOTCompilation>false</RunAOTCompilation>` is required — the native-interop layer is reflection-heavy in places and doesn't survive AOT trimming today. Uno's default for some heads is AOT-on; the upstream sample explicitly turns it off, and so should you.

### Uno "single project" structure

Unlike MAUI, an Uno app is conventionally split into multiple **head projects** — one per target OS (a Windows head, an Android head, an iOS head, …) — each consuming a shared library project. The upstream Simple Capture Uno sample uses the newer **`UnoSingleProject` mode** (`<UnoSingleProject>true</UnoSingleProject>`) which collapses the heads into a single multi-targeted csproj. That's the layout reflected in `references/Sample.csproj`. If your existing solution still uses the older multi-head layout, add the VisioForge package references and conditional redist blocks to **the head project that builds for that target**, not to the shared library.

### App.xaml.cs registration

Uno's `App` class is plain WinUI / WinAppSDK. There is **no MAUI-style handler registration** — the `<vf:VideoView />` control wires itself up through its own `xmlns` declaration in XAML, so nothing needs to be done in `App.xaml.cs` beyond what the Uno template generates. See `references/App.xaml.cs` for the unchanged template.

## Per-platform permissions

Cross-platform capture means cross-platform permission paperwork. Each OS has its own dialect:

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

Sandbox-on Mac Catalyst is supported too — just write recordings to `Environment.SpecialFolder.MyVideos` (which lives inside the app container) instead of `~/Movies`.

### Windows

No project-side permission paperwork for the Windows head. The system camera privacy switch in **Settings → Privacy & security → Camera** is enforced by Windows itself. The upstream sample additionally calls `MediaCapture.InitializeAsync` once at startup to surface the Windows consent UI — see `EnsureWindowsPermissionsAsync` in `references/MainPage.xaml.cs`.

## Mandatory engine boot

Before any `VideoCaptureCoreX` instance is constructed (and before any `DeviceEnumerator` query), call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time on desktop; up to ~10 s on Android first launch); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on page unload / app shutdown:

```csharp
private async void MainPage_Loaded(object sender, RoutedEventArgs e)
{
    // 1. ensure permissions FIRST — InitSDKAsync probes devices and will fail
    //    on iOS / mac if the user hasn't granted access yet.
    var permissionsOk = await EnsureApplePermissionsAsync();   // or Android / Windows variant
    if (!permissionsOk) { ShowStatus("Permissions required", "#FF0000"); return; }

    // 2. boot the engine
    await VisioForgeX.InitSDKAsync();

    // 3. now safe to construct the core and query devices
    _core = new VideoCaptureCoreX(videoView);
    _core.OnError += Core_OnError;
}

private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
{
    if (_core != null)
    {
        await _core.StopAsync();
        _core.Dispose();
        _core = null;
    }
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. Calling it **before** the platform permission check on iOS / mac doesn't crash, but you can hit a spurious "no devices" result on a cold first launch — request permissions first.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _core.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
_core = new VideoCaptureCoreX(videoView);
_core.OnError += Core_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _core.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x. The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes` works on Windows / Mac Catalyst (sandbox off), but on iOS / Android the working directory is the app bundle, not your dev machine. The portable approach for an Uno head is to ship the licence as a **`Content`** item with `CopyToOutputDirectory=PreserveNewest` and read it with `Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///license.vflicense"))` — that URI scheme works on every Uno target including Android / iOS. (On the Windows head you can also use `Package.Current.InstalledLocation`.) Where the bytes come from (asset, env var, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

The bundled `references/MainPage.xaml.cs` runs in trial mode by design; add the two lines above into `MainPage_Loaded` right after `_core = new VideoCaptureCoreX(...)` to register a purchased licence.

## Hello-World capture

A camera-→-preview pipeline on Uno is three lines: init, construct, configure-and-start. The full file is in `references/MainPage.xaml.cs`; the minimum viable XAML + code-behind:

```xml
<!-- MainPage.xaml -->
<Page x:Class="YourApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:vf="using:VisioForge.Core.UI.Uno"
      Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Grid>
        <vf:VideoView x:Name="videoView"
                      HorizontalAlignment="Stretch"
                      VerticalAlignment="Stretch" />
        <!-- add a Button x:Name="StartButton" Click="StartButton_Click" anywhere -->
    </Grid>
</Page>
```

```csharp
// MainPage.xaml.cs
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public sealed partial class MainPage : Page
{
    private VideoCaptureCoreX? _core;

    public MainPage()
    {
        InitializeComponent();
        Loaded += async (_, __) =>
        {
            // Permission requests omitted for brevity — see references/MainPage.xaml.cs.
            await VisioForgeX.InitSDKAsync();
            _core = new VideoCaptureCoreX(videoView);
        };
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers MUST catch — otherwise an exception escapes
        // to AppDomain.UnhandledException and silently terminates the app.
        // Common triggers on first run: permission denied, trial expired,
        // missing native libs, or no device.
        try
        {
            var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
            if (devices.Length == 0) { /* show "no camera" */ return; }

            var device = devices[0];
            var format = device.GetHDVideoFormatAndFrameRate(out var frameRate);
            _core!.Video_Source = new VideoCaptureDeviceSourceSettings(device)
            {
                Format = format.ToFormat()
            };
            _core.Video_Source.Format.FrameRate = frameRate;
            _core.Audio_Play = false;
            _core.Audio_Record = false;

            // Preview-only — no Outputs_Add. For recording, see references/MainPage.xaml.cs.
            await _core.StartAsync();
        }
        catch (Exception ex) { /* surface to UI */ }
    }
}
```

`references/MainPage.xaml.cs` extends this with per-OS permission helpers, audio source/output, MP4 recording (`H264EncoderBlock` + `MP3EncoderSettings` → `MP4Output`), per-platform output paths (Android scoped storage, iOS Documents, Mac Catalyst `~/Movies`, Windows `MyVideos`), gallery save on mobile, and `OnError` wiring. Use it as a copy-paste template; trim the branches you don't need.

## Common cross-platform pitfalls

### 1. Black `<vf:VideoView />`, no errors logged

**Cause**: capture started before the Uno `VideoView` element has a valid handle (e.g. construction in the page constructor instead of `Loaded`), **or** `VideoCaptureCoreX` was constructed before `await VisioForgeX.InitSDKAsync()` returned, **or** the wrong native redist was referenced for the build's TFM.

**Fix**: defer construction and `StartAsync` to `Loaded`. The bundled sample uses `MainPage_Loaded` for `InitSDKAsync` + core construction and an explicit "Preview" button for `StartAsync` — copy that pattern.

### 2. `DllNotFoundException` / `dlopen` failure on first capture (per-OS)

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`. Common slips:

- Windows head: forgot `VisioForge.CrossPlatform.Core.Windows.x64` (build succeeds but startup fails on first `VideoCaptureCoreX` use).
- Android head: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough.
- Mac Catalyst head: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional `<ItemGroup>` and the Mac Catalyst `<Target>` matters.

### 3. `VisioForgeX.InitSDKAsync` works locally but fails in `-browserwasm` head

**Cause**: WebAssembly is not a supported target for the X engine. There is no WASM build of the GStreamer-based native runtime, so `InitSDKAsync` cannot find `libgstcore` (or its WASM equivalent) and throws `DllNotFoundException`.

**Fix**: split the WebAssembly UI off into a separate Uno head that does **not** reference `VisioForge.DotNet.VideoCapture` / `VisioForge.DotNet.Core.UI.Uno`. The capture project should multi-target only `-windows`, `-android`, `-ios`, and `-maccatalyst`. If you accidentally added `net10.0-browserwasm` to `<TargetFrameworks>`, remove it.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: read the `.vflicense` (shipped as `Content` / `ms-appx:///` resource on iOS / Android, see "License registration") and call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync`. Every `VideoCaptureCoreX` instance in the process needs its own call.

### 5. iOS first run silently terminates as soon as capture starts

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Platforms/iOS/Info.plist`. iOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no managed crash log entry from your code.

**Fix**: add both keys with non-empty user-facing strings (see "Per-platform permissions"). For Mac Catalyst, also confirm `Entitlements.plist` enables `com.apple.security.device.camera` and `com.apple.security.device.audio-input`.

### 6. Mac Catalyst app launches but `<vf:VideoView />` is empty and `OnError` reports `dlopen` failure

**Cause**: the `CopyNativeLibrariesToMonoBundle` target was deleted from the csproj, or the build configuration has `<UseInterpreter>` off and AOT trimmed the native-interop layer.

**Fix**: keep the `CopyNativeLibrariesToMonoBundle` `<Target>` from `references/Sample.csproj`, and keep `<RunAOTCompilation>false</RunAOTCompilation>` at the project level.

## Verification checklist

- [ ] Uno workloads installed: `dotnet workload list` shows `android`, plus `maccatalyst` on macOS / `wasm-tools` if you have a (separate) WASM head.
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041` succeeds against the bundled `references/`.
- [ ] On Windows host: `dotnet build -f net10.0-android` succeeds.
- [ ] On macOS host: `dotnet build -f net10.0-maccatalyst` succeeds; the `.app/Contents/MonoBundle/` folder contains `.dylib` files after the build.
- [ ] App launches on each target OS and shows the camera preview within ~2 s after permission grant.
- [ ] If recording: the MP4 is finalised when `StopCaptureAsync` runs to completion before `Dispose` (verify the file plays back in another player).
- [ ] On clean shutdown, the page's `Unloaded` runs `StopAsync → Dispose → VisioForgeX.DestroySDK()` in that order.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `Uno.Sdk` project folder, set up `Platforms/` per the standard Uno template, and `dotnet build` succeeds with no extra files needed (apart from the `AndroidDependency/VisioForge.Core.Android.X10.csproj` companion project, which lives a few directory levels up — adjust the `<ProjectReference>` path to match your repo layout).

- `references/Sample.csproj` — multi-target Uno csproj with all per-OS conditional package references and the Mac Catalyst native-copy `<Target>`.
- `references/App.xaml` + `references/App.xaml.cs` — Uno Application entry point (unchanged from the Uno template; no VisioForge-specific registration needed).
- `references/MainPage.xaml` — XAML with `<vf:VideoView />` and camera/mic/speaker/preview/capture buttons.
- `references/MainPage.xaml.cs` — full code-behind: per-OS permission helpers (Android, iOS, Mac Catalyst, Windows), `InitSDKAsync` boot, `DeviceEnumerator` wiring, MP4 recording, per-platform output-path logic (Android scoped storage, iOS Library, Mac Catalyst `~/Movies`, Windows `MyVideos`), gallery save on mobile, and `OnError` wiring. Runs in 30-day trial mode by design — add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Uno>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-wpf` — same X SDK on WPF (Windows-only host, single TFM, no per-OS conditionals).
    - `video-capture-sdk-x-winui` — same X SDK on WinUI 3 / WinAppSDK (Windows-only) without Uno.
    - **Other cross-platform hosts**:
        - `video-capture-sdk-x-maui` — same X SDK on .NET MAUI.
        - `video-capture-sdk-x-avalonia` — same X SDK on Avalonia (broader Linux support, no iOS).
        - `media-blocks-sdk-net-uno` — same engine on Uno, lower-level graph-based API for custom pipeline topologies.
