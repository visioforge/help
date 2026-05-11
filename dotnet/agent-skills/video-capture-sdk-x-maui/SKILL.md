---
name: video-capture-sdk-x-maui
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a .NET MAUI cross-platform app (Windows, Android, iOS, macOS). Covers the MAUI-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (camera permissions, missing native libs, AOT JIT-only ExecutionEngineException, trial-period expiry / unlicensed build). Use when building capture apps that must run on multiple OSes from one MAUI codebase — for graph-based pipelines use media-blocks-sdk-net-maui.
---

# Video Capture SDK X — .NET MAUI integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a .NET MAUI application that targets **Windows, Android, iOS, and Mac Catalyst** from a single codebase. The X SDK shares its native runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level capture-and-record god-object (`VideoCaptureCoreX`) that mirrors the legacy `VideoCaptureCore` API but runs on the cross-platform engine. Same C# code targets every OS — only the platform handler glue and per-OS redist NuGets change between TFMs.

Pinned NuGet versions: wrapper **`2026.5.4`**, MAUI handlers **`2026.5.4`**, plus per-OS native redists at the versions shown in the csproj below — these match the official [Video Capture X MAUI samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI). Newer 2026.x.x patch versions are drop-in compatible; keep `VisioForge.DotNet.VideoCapture` and `VisioForge.DotNet.Core.UI.MAUI` pinned to the same wrapper version. The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin to the value shipped in the upstream csproj for your wrapper version; do not blindly bump.

## When to use this skill

- One MAUI codebase that runs on **Windows, Android, iOS, and Mac Catalyst** with webcam, IP camera, screen, or NDI capture and recording.
- Recording captured video/audio to MP4 (incl. fragmented), MOV, MPEG-TS, AVI, or WebM from a single high-level API.
- Mobile-friendly preview using `VisioForge.Core.UI.MAUI.VideoView`.
- Sharing capture/recording code between a Windows desktop "main" app and one or more cross-platform companion apps — the `VideoCaptureCoreX` API is identical across hosts (WPF / MAUI / Avalonia / Uno).

## When NOT to use this skill

- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): use `media-blocks-sdk-net-maui` — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Windows-only desktop app** (WPF / WinForms): `video-capture-sdk-x-wpf` is simpler — single TFM, no per-OS redists, no permission paperwork.
- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): `video-capture-sdk-net-wpf`.
- **Playback only** (file/stream playback without capturing): `media-player-sdk-x-maui`.
- **Different cross-platform host**: `video-capture-sdk-x-avalonia` / `video-capture-sdk-x-uno` — same SDK, different UI shell.

## NuGet packages (cross-platform layout)

Video Capture X for MAUI is **not** a single meta-package. You add the SDK plus the **MAUI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.VideoCapture` | Managed wrapper (`VideoCaptureCoreX`, types) | Always |
| `VisioForge.DotNet.Core.UI.MAUI` | MAUI `VideoView` control + handlers | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav muxers + encoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` ships through a small bundled csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy SDK uses — both `VideoCaptureCore` (legacy, Windows-only) and `VideoCaptureCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist set plus the MAUI handler registration below.

The Android target also needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` — a small companion project that ships in the samples repo under `Video Capture SDK X/MAUI/AndroidDependency/`. That's how the `.aar` is bound. Copy that folder into your repo alongside your MAUI app and reference it as shown in the csproj below; there is no NuGet equivalent today.

## Project setup

### MAUI workload

```bash
dotnet workload install maui
```

Without this, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: maui-android maui-ios maui-maccatalyst maui-windows`.

### Multi-target csproj

This is the core of the skill — get this right and the rest follows. The csproj declares target frameworks **conditionally per host OS**: Android always; iOS + Mac Catalyst only on macOS hosts; Windows only on Windows hosts. That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights:

```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">$(TargetFrameworks);net10.0-maccatalyst;net10.0-ios</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>

<UseMaui>true</UseMaui>
<SingleProject>true</SingleProject>

<!-- iOS / Mac Catalyst MUST run with the Mono interpreter, otherwise MAUI XAML
     hits a JIT-only path inside Styles.xaml load and throws ExecutionEngineException. -->
<UseInterpreter Condition="...== 'ios'">true</UseInterpreter>
<UseInterpreter Condition="...== 'maccatalyst'">true</UseInterpreter>
```

The conditional `<ItemGroup>` blocks pull in the right per-OS native packages:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.5.4" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>
```

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure on first `VideoCaptureCoreX` construction.

### MauiProgram registration

In `MauiProgram.cs` register the VisioForge MAUI handlers and SkiaSharp (used internally by the renderer):

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Forgetting `AddVisioForgeHandlers()` is the most common silent failure — `<my:VideoView />` renders as a blank `Grid`-shaped void, no errors logged.

## Engine boot model — different from WPF

Unlike the WPF host (which requires an explicit `await VisioForgeX.InitSDKAsync()` before constructing `VideoCaptureCoreX`), the MAUI handler chain initializes the native runtime when `AddVisioForgeHandlers()` is registered — so you do **not** call `InitSDKAsync` yourself in MAUI. Construction order is just:

1. MAUI app starts; `MauiProgram.CreateMauiApp()` runs (`AddVisioForgeHandlers` triggers native init).
2. `MainPage_Loaded` requests permissions, then `_core = new VideoCaptureCoreX(videoView.GetVideoView())`.
3. Configure sources / outputs, then `await _core.StartAsync()`.

On shutdown, mirror with `VisioForgeX.DestroySDK()` (in `Window.Destroying` and `Page.Unloaded`):

```csharp
private async void Window_Destroying(object? sender, EventArgs e)
{
    if (_core != null)
    {
        _core.OnError -= Core_OnError;
        await _core.StopAsync();
        _core.Dispose();
        _core = null;
    }
    VisioForgeX.DestroySDK();
}
```

## Per-platform permissions

Cross-platform capture means cross-platform permission paperwork. Each OS has its own dialect — see `references/platform-permissions.md` for the full set; the essentials:

### Android

`Platforms/Android/AndroidManifest.xml` must declare `CAMERA` and `RECORD_AUDIO`. Storage permissions are needed to write recorded MP4 files to gallery on older API levels:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

At runtime, request the permission with the MAUI Essentials API before the engine starts:

```csharp
await Permissions.RequestAsync<Permissions.Camera>();
await Permissions.RequestAsync<Permissions.Microphone>();
// API 33+ for gallery save:
await Permissions.RequestAsync<Permissions.Media>();
```

### iOS

`Platforms/iOS/Info.plist` MUST contain non-empty user-facing strings — App Store review rejects builds without them, and **iOS terminates the app** the first time you touch the camera/mic without the matching key:

```xml
<key>NSCameraUsageDescription</key><string>Camera usage required</string>
<key>NSMicrophoneUsageDescription</key><string>Mic usage required</string>
<key>NSPhotoLibraryUsageDescription</key><string>To save recorded videos</string>
```

For Photos-library save (used by `StopCaptureAsync` in the bundled sample), call `Photos.PHPhotoLibrary.RequestAuthorization(...)` directly — there's no MAUI Essentials wrapper.

### Mac Catalyst

`Platforms/MacCatalyst/Info.plist` needs the same usage descriptions as iOS, **plus** `Platforms/MacCatalyst/Entitlements.plist` must enable the camera/microphone entitlements (sandboxed Mac Catalyst apps don't get these by default):

```xml
<!-- Entitlements.plist -->
<key>com.apple.security.device.camera</key><true/>
<key>com.apple.security.device.audio-input</key><true/>
<!-- USB external webcams: -->
<key>com.apple.security.device.usb</key><true/>
```

### Windows

No extra permission paperwork — the system camera privacy switch in Settings is enforced by Windows itself, not declared in the project. If the user has denied access globally, `DeviceEnumerator.Shared.VideoSourcesAsync()` returns an empty list.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _core.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`. The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes("path/to/your.vflicense")` works on Windows, but on iOS / Android / Mac Catalyst the working directory is the app bundle, not your dev machine. The portable approach is to ship the licence as a `MauiAsset` and load via `FileSystem.OpenAppPackageFileAsync`:

```csharp
// In csproj:
// <ItemGroup>
//   <MauiAsset Include="Resources\Raw\license.vflicense" />
// </ItemGroup>

using var stream = await FileSystem.OpenAppPackageFileAsync("license.vflicense");
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
await _core.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-page apps, every `VideoCaptureCoreX` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`. Where the bytes come from (asset, env var, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/MainPage.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the snippet above into `MainPage_Loaded` right after `_core = new VideoCaptureCoreX(vv)`.

## Hello-World capture

A camera-→-preview pipeline with `VideoCaptureCoreX` is three lines of wiring:

```xml
<!-- MainPage.xaml -->
<ContentPage xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI" ...>
    <Grid>
        <my:VideoView x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand" />
    </Grid>
</ContentPage>
```

```csharp
// MainPage.xaml.cs
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public partial class MainPage : ContentPage
{
    private VideoCaptureCoreX _core;

    public MainPage() { InitializeComponent(); Loaded += async (_, __) => await StartAsync(); }

    private async Task StartAsync()
    {
        await Permissions.RequestAsync<Permissions.Camera>();

        _core = new VideoCaptureCoreX(videoView.GetVideoView());

        var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (cameras.Length == 0) return;

        var device = cameras[0];
        var fmt = device.GetHDVideoFormatAndFrameRate(out var frameRate);
        var settings = new VideoCaptureDeviceSourceSettings(device) { Format = fmt.ToFormat() };
        settings.Format.FrameRate = frameRate;

        _core.Video_Source = settings;
        _core.Audio_Play = false;
        _core.Audio_Record = false;

        // Preview-only — no Outputs_Add. For recording, see references/MainPage.xaml.cs.
        await _core.StartAsync();
    }
}
```

The bundled `references/MainPage.xaml.cs` extends this with audio source/renderer, MP4 recording (H.264 + MP3 muxed via `Outputs_Add(new MP4Output(...))`), per-OS gallery save, runtime device cycling, and an explicit Start/Stop UI. Use it as a copy-paste template; trim the branches you don't need.

## Common cross-platform pitfalls

These are the five most common production issues — flag any of them on first run.

### 1. Black `VideoView`, no errors logged

**Cause**: `AddVisioForgeHandlers()` was not called in `MauiProgram.CreateMauiApp`. The view registers fine but renders nothing because no platform handler is bound — and the native runtime is not initialised.

**Fix**: add `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers())` to the builder chain (see "MauiProgram registration" above).

### 2. `DllNotFoundException` / `dlopen` failure on first capture (per-OS)

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`, **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`). Common slips:

- Windows: forgot `VisioForge.CrossPlatform.Core.Windows.x64` (build succeeds but `VideoCaptureCoreX` construction fails).
- Android: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough; the companion csproj binds the `.aar`.
- Mac Catalyst: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional ItemGroup matters. Pin redist versions to the values shipped in the upstream csproj for your wrapper version; do not bump.

### 3. iOS / Mac Catalyst app crashes during MAUI XAML load

**Cause**: `<UseInterpreter>` is not enabled for those TFMs. MAUI XAML's `TypeConversionExtensions` uses `DynamicMethod` to invoke implicit operators, and `DynamicMethod` requires JIT. On iOS / Mac Catalyst the runtime is AOT-only by default, so the call throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode` from inside `App.InitializeComponent()`.

**Fix**: set `<UseInterpreter>true</UseInterpreter>` on `-ios` and `-maccatalyst` (see csproj). Tiny size/startup cost, fixes XAML load.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: ship the `.vflicense` as a `MauiAsset` and call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every `VideoCaptureCoreX` instance in the process needs its own call.

### 5. iOS first run silently terminates as soon as capture starts

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Platforms/iOS/Info.plist`. iOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no crash log entry from your code.

**Fix**: add both keys with non-empty user-facing strings (see "Per-platform permissions" and `references/platform-permissions.md`). Mac Catalyst additionally needs the matching entries in `Entitlements.plist`.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet workload install maui` is installed; `dotnet build -f net10.0-android` succeeds against the bundled `references/`.
- [ ] On macOS host: `dotnet build -f net10.0-ios` and `dotnet build -f net10.0-maccatalyst` succeed (TFMs only show up on macOS).
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041.0` succeeds.
- [ ] `MauiProgram.CreateMauiApp()` calls `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers()).UseSkiaSharp()` (otherwise `VideoView` is blank everywhere).
- [ ] App launches on each target OS and shows the camera preview within ~2 s after permission grant.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCoreX` (always call `await _core.StopAsync(); _core.Dispose();` on close, then `VisioForgeX.DestroySDK()`).
- [ ] If recording: the MP4 is finalised when `StopCaptureAsync` runs to completion before `Dispose` (verify the file plays back in another player).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode and aborts after day 30).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh MAUI project folder, set up `Platforms/` and `Resources/` per the standard MAUI template, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target MAUI csproj with all per-OS conditional package references and the Mac Catalyst native-copy target.
- `references/MauiProgram.cs` — entry point with `AddVisioForgeHandlers()` + `UseSkiaSharp()`.
- `references/App.xaml` — MAUI Application entry point.
- `references/MainPage.xaml` — XAML with `<my:VideoView />` and device-cycle / preview / capture buttons.
- `references/MainPage.xaml.cs` — full code-behind: device enumeration, runtime permission requests (Android/iOS/Mac Catalyst), preview, MP4 recording (H.264 + MP3 via `MP4Output`), per-OS gallery save, `OnError` wiring, clean shutdown via `Window.Destroying` + `Page.Unloaded`. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/platform-permissions.md` — concise notes on Android `AndroidManifest.xml`, iOS `Info.plist`, Mac Catalyst `Info.plist` + `Entitlements.plist`.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/MAUI>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-wpf` — same X SDK on WPF (Windows-only host, no per-OS redists, no permission paperwork).
    - `media-blocks-sdk-net-maui` — same engine, lower-level graph-based API for custom pipeline topologies on MAUI.
    - **Other cross-platform hosts**:
        - `video-capture-sdk-x-avalonia` — same X SDK on Avalonia.
        - `video-capture-sdk-x-uno` — same X SDK on Uno Platform.
