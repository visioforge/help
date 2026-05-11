---
name: video-capture-sdk-x-winui
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a WinUI 3 (Windows App SDK) application. Covers the WinUI-specific VideoView control, the cross-platform NuGet package layout, project setup, license registration, MSIX packaging quirks, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want capture/recording on WinUI 3 with an API that ports to MAUI, Avalonia, Uno — for the legacy DirectShow stack, use video-capture-sdk-net-winui.
---

# Video Capture SDK X — WinUI 3 integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a Windows App SDK / WinUI 3 desktop application. The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level capture-and-record god-object (`VideoCaptureCoreX`) that mirrors the legacy `VideoCaptureCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux / iOS / Android — the only thing that changes between platforms is the UI host (WinUI 3 here, MAUI / Avalonia / Uno / native elsewhere) and the per-OS native redist NuGet package.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`**, Windows App SDK **`Microsoft.WindowsAppSDK 1.8.251106002`** (matches the [official Simple Video Capture WinUIX sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinUI)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding webcam, IP camera, screen, or NDI capture to a Windows WinUI 3 desktop app **with an API that ports unchanged to other platforms** (MAUI, Avalonia, Uno, Android, iOS, macOS).
- Recording captured video/audio to MP4 (incl. fragmented), AVI, MOV, MPEG-TS, or WebM.
- Shipping as MSIX (Store / sideload) or as an unpackaged WinUI 3 app while sharing the same `VideoCaptureCoreX` code with cross-platform companion apps.
- Pipeline introspection: `VideoCaptureCoreX.GetDiagramAsImage()` returns a SkiaSharp bitmap of the live GStreamer graph (handy for support tickets).

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist) on WinUI: use `video-capture-sdk-net-winui`. The two SDKs ship side-by-side and can coexist in one app.
- **WPF host** instead of WinUI 3: same X SDK → `video-capture-sdk-x-wpf` (different `VideoView` element, different csproj).
- **Playback only** (play files / streams without capturing): `media-player-sdk-net-winui`.
- **Cross-platform host instead of WinUI 3**: same SDK, different UI shell → `video-capture-sdk-x-{maui,avalonia,uno}`.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): the high-level `VideoCaptureCoreX` doesn't expose the underlying graph — drop down to `media-blocks-sdk-net-wpf` (the WinUI WindowsAppSDK host pattern works the same way for the engine itself; only the `VideoView` wiring differs).

## Project setup

### Target framework and platform

WinUI 3 desktop apps use the plain **`Microsoft.NET.Sdk`** SDK (not `WindowsDesktop`) plus `<UseWinUI>true</UseWinUI>`. The bundled `references/Sample.csproj` targets:

```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
<Platforms>x64</Platforms>
<UseWinUI>true</UseWinUI>
<EnableMsixTooling>true</EnableMsixTooling>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

WinUI 3 X-SDK apps are pinned to **x64** (not AnyCPU): the WinAppSDK ships separate x64/x86/arm64 native binaries, and the X redist NuGets are split per-architecture. Pinning `<Platforms>x64</Platforms>` and matching the redist suffix makes runtime resolution unambiguous. For x86 or arm64, swap the redist suffix accordingly (`.x86` / no arm64 redist exists today on the X line — Windows-on-ARM is not supported by the cross-platform engine on Windows yet). `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` bundles the WinAppSDK runtime so end users don't need a separate install.

### NuGet packages

Four packages are required for a WinUI 3 capture-and-record scenario — the .NET wrapper, the WinUI-specific `VideoView` host, plus two native redist packages (Core runtime + libav muxers/encoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy SDK uses — both `VideoCaptureCore` (legacy) and `VideoCaptureCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]`) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. The upstream WinUI sample uses `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` — that's a UPX-compressed variant (smaller download, slightly slower first-load). The non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable; pick one and stay consistent within the project.

`VisioForge.DotNet.Core.UI.WinUI` provides the `VisioForge.Core.UI.WinUI.VideoView` host needed to render the X engine into a Win2D `CanvasControl`. WinUI 3 needs this companion package — it is not pulled in transitively by the main wrapper.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Simple Video Capture WinUIX sample (`_SETUP/GitHub/Video Capture SDK X/WinUI/CSharp/Simple Video Capture Demo WinUIX/`). Changes vs upstream: the SDK icon asset (`Assets\visioforge_main_icon.ico`) and its `<None Remove>` / `<Content Update>` entries are removed; everything else is byte-identical including the MSIX tooling block and the explicit Windows App SDK / SDK build-tools pin.

## Mandatory engine boot

Before any `VideoCaptureCoreX` instance is constructed (and before any `DeviceEnumerator` query), call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// MainWindow.xaml.cs — Window_Activated, gated by _isInitiated
private async void Window_Activated(object sender, WindowActivatedEventArgs args)
{
    if (_isInitiated) return;
    _isInitiated = true;

    await VisioForgeX.InitSDKAsync();

    _videoView = new VideoView(canvasControl);
    VideoCapture1 = new VideoCaptureCoreX(_videoView);
    // ...now safe to query DeviceEnumerator, configure sources, etc.
}

// Window_Closed
private void Window_Closed(object sender, WindowEventArgs args)
{
    VideoCapture1?.Dispose();
    VideoCapture1 = null;
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/MainWindow.xaml.cs` shows the canonical placement (inside `Window_Activated`, gated by `_isInitiated` because Activated fires repeatedly on focus changes).

## VideoView in WinUI 3

The WinUI VideoView is **not a XAML control** the way the WPF one is — it's a thin `IVideoView` wrapper around a Win2D `CanvasControl`. Layout the `CanvasControl` in XAML and pass it to the `VideoView` constructor in code:

```xml
<!-- MainWindow.xaml -->
xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
...
<win2d:CanvasControl x:Name="canvasControl" Background="Black"
                     HorizontalAlignment="Stretch" VerticalAlignment="Stretch"/>
```

```csharp
// MainWindow.xaml.cs — inside Window_Activated, AFTER InitSDKAsync, ONCE
using VisioForge.Core.UI.WinUI;

_videoView = new VideoView(canvasControl);
VideoCapture1 = new VideoCaptureCoreX(_videoView);
```

This is a structural difference from WPF (where `<my:VideoView/>` is itself the XAML element). If you copy/paste from a WPF X skill, you will get "type not found" XAML errors — there is no `<vfwinui:VideoView/>` element to drop into the XAML tree.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoCapture1.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
VideoCapture1 = new VideoCaptureCoreX(_videoView);
VideoCapture1.OnError += VideoCapture1_OnError;

var cert = await File.ReadAllBytesAsync("path/to/your.vflicense");
await VideoCapture1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-window apps, every `VideoCaptureCoreX` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

**MSIX note**: when the `.vflicense` is bundled as Content in the package, load it via `Package.Current.InstalledLocation.Path` rather than a relative path — the working directory under MSIX is not the package install root.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoCapture1 = new VideoCaptureCoreX(...)`.

## Hello-World capture

Minimum viable capture-and-preview snippet — a self-contained `MainWindow` you can drop into a fresh WinUI 3 desktop project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
        Activated="Window_Activated">
    <Grid>
        <win2d:CanvasControl x:Name="canvasControl" Background="Black"/>
        <Button x:Name="btStart" Content="Start" Click="btStart_Click"
                VerticalAlignment="Bottom" HorizontalAlignment="Left" Margin="12"/>
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System;
using System.Linq;
using Microsoft.UI.Xaml;
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.WinUI;
using VisioForge.Core.VideoCaptureX;

namespace YourApp;

public sealed partial class MainWindow : Window
{
    private VideoCaptureCoreX _videoCapture;
    private VideoView _videoView;
    private bool _isInitiated;

    public MainWindow() => InitializeComponent();

    // VideoView must be constructed AFTER the Window has an HWND AND
    // AFTER VisioForgeX.InitSDKAsync has returned. Window_Activated is
    // the official sample's hook; gate with _isInitiated because Activated
    // fires repeatedly on focus changes.
    private async void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (_isInitiated) return;
        _isInitiated = true;

        await VisioForgeX.InitSDKAsync();   // mandatory engine boot

        _videoView = new VideoView(canvasControl);
        _videoCapture = new VideoCaptureCoreX(_videoView);
        _videoCapture.OnError += (_, e) => System.Diagnostics.Debug.WriteLine(e.Message);

        // For a purchased licence:
        //   var cert = await System.IO.File.ReadAllBytesAsync("your.vflicense");
        //   await _videoCapture.SetLicenseCertificateAsync(cert);
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers must catch — otherwise an exception
        // escapes to AppDomain.UnhandledException and silently terminates
        // the app. Common triggers on first run: trial expired, missing
        // native DLLs, registry not built (forgot InitSDKAsync), no device.
        try
        {
            var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
            if (devices.Count == 0) return;

            var device = devices[0];
            var format = device.VideoFormats.First();
            _videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(device)
            {
                Format = format.ToFormat()
            };
            _videoCapture.Audio_Play = false;
            _videoCapture.Audio_Record = false;

            // Preview-only — no Outputs_Add call. For recording, see
            // references/MainWindow.xaml.cs (MP4Output, etc.).
            await _videoCapture.StartAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Capture failed: {ex.Message}");
        }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `references/MainWindow.xaml`) ships the full pattern with device/format/frame-rate selection, audio routing, MP4 recording, recording-time display, and `OnError` wiring.

## Common deployment failures

Five most common production issues in WinUI 3 — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X" (unpackaged)

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the X redist NuGets aren't being honoured (the WinUI loader sometimes leaves natives in `runtimes/win-x64/native/` and won't search there for unpackaged builds), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). For unpackaged builds, publish self-contained: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None` — that flattens the natives into the publish root. Verify by listing the publish folder: the GStreamer DLLs from `VisioForge.CrossPlatform.Core.Windows.x64` must sit next to (or be loadable from) the .exe. Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Native DLLs missing on end-user machine (MSIX)

**Cause**: MSIX strictly bounds the package contents — only what's declared as Content or comes via NuGet `runtimes/` is shipped. The X redists register their natives correctly, but a hand-edited `.appxmanifest` or a `<Content Remove>` in the csproj can drop them.

**Fix**: do not exclude `runtimes/**` from MSIX content. After packaging, open the `.msix` (it's a ZIP), look under `\runtimes\win-x64\native\` for the GStreamer / libav DLLs. If they're missing, the redist NuGets aren't being honoured — re-add them at the project level (don't rely on transitive only, see `references/Sample.csproj`).

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoCapture.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration"). Every `VideoCaptureCoreX` instance in the process needs its own call. Under MSIX, load the certificate from `Package.Current.InstalledLocation.Path` — relative paths break.

### 4. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the output format depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]` covers MP4 (libav h264), AAC, MPEG-TS, MOV, AVI, and WebM out of the box. Less common codecs (HAP, DNxHD, ProRes via plugin variants) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist (e.g. for VP9 hardware encode), add it with the same version pin as the others.

### 5. Preview is black; `VideoView` constructed too early

**Cause**: `VideoView(canvasControl)` was called from the constructor or before the window has an HWND, **or** before `VisioForgeX.InitSDKAsync()` returned. The `CanvasControl` only has a render target after `Window.Activated` has fired at least once.

**Fix**: construct the `VideoView` and `VideoCaptureCoreX` in the `Activated` handler, gated by an `_isInitiated` flag (Activated fires every time focus changes), AFTER `await VisioForgeX.InitSDKAsync()` returns. Both the bundled `references/MainWindow.xaml.cs` and the Hello-World snippet above use this pattern. Calling `StartAsync` from a button click — not from `Activated` itself — also avoids races where the engine starts before the canvas swapchain is ready.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (after adding `Assets/` PNGs from any WinUI 3 desktop template).
- [ ] First run shows a brief delay (~2-5 s, the GStreamer registry build) before the preview appears; subsequent launches are instant.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCoreX` (always call `await _videoCapture.StopAsync(); _videoCapture.Dispose();` on close).
- [ ] On clean shutdown, `Window_Closed` runs `Dispose → VisioForgeX.DestroySDK()` in that order.
- [ ] If recording to MP4: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `Dispose`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode). Under MSIX, the certificate path is resolved via `Package.Current.InstalledLocation.Path`.
- [ ] If publishing unpackaged: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsPackageType=None` — verify the GStreamer / libav DLLs land next to the .exe.
- [ ] If publishing MSIX: open the `.msix` ZIP and verify `\runtimes\win-x64\native\` contains the X engine DLLs.

## Bundled references

The `references/` folder is a faithful copy of the official sample with the SDK icon stripped. Copy all of it into a fresh project folder; you'll also need the `Assets/` PNGs from any WinUI 3 desktop template (or the upstream sample) for the package to build:

- `references/Sample.csproj` — minimal working WinUI 3 csproj, version-pinned to the same NuGet release as the prose (wrapper `2026.5.4`, redist `2026.4.29`, WindowsAppSDK `1.8.251106002`).
- `references/App.xaml` + `references/App.xaml.cs` — Application entry point.
- `references/MainWindow.xaml` — XAML with `<win2d:CanvasControl x:Name="canvasControl"/>` for the preview surface plus the device/output/log Pivot UI.
- `references/MainWindow.xaml.cs` — full code-behind with `InitSDKAsync` boot, `DeviceEnumerator` wiring, MP4 recording, audio routing, recording-time display, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Package.appxmanifest` — MSIX manifest (declares `runFullTrust` capability, required for Win32 P/Invoke into the SDK natives).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-winui` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist).
    - `video-capture-sdk-x-wpf` — same X SDK on WPF (different `VideoView` namespace, XAML element rather than Win2D-canvas wrapper).
    - **Cross-platform hosts**:
        - `video-capture-sdk-x-maui` — same X SDK on .NET MAUI.
        - `video-capture-sdk-x-avalonia` — same X SDK on Avalonia.
        - `video-capture-sdk-x-uno` — same X SDK on Uno Platform.
