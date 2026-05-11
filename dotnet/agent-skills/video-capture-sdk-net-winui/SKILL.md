---
name: video-capture-sdk-net-winui
description: Integrate VisioForge Video Capture SDK .NET into a WinUI 3 (Windows App SDK) application. Covers the WinUI-specific VideoView control, the single NuGet package, project setup, license registration, MSIX packaging quirks, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when adding webcam, IP camera, screen, or DV capture to a WinUI 3 app — for WPF use video-capture-sdk-net-wpf, for WinForms use video-capture-sdk-net-winforms.
---

# Video Capture SDK .NET — WinUI 3 integration

This skill helps you add **VisioForge Video Capture SDK .NET** to a Windows App SDK / WinUI 3 desktop application. It covers webcam, IP camera, screen, and DV-camera capture with preview, recording, and snapshot. WinUI 3 is the right host when you want native Windows 10/11 look-and-feel, MSIX/Store deployment, and the modern Fluent control set; for the traditional WPF stack use `video-capture-sdk-net-wpf`, for WinForms use `video-capture-sdk-net-winforms`. The SDK is Windows-only (DirectShow / Media Foundation under the hood).

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Video Capture WinUI sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinUI/CSharp/Simple%20Video%20Capture%20Demo%20WinUI)). Newer 2026.x.x patch versions are drop-in compatible. Pinned Windows App SDK: **`Microsoft.WindowsAppSDK 1.8.251106002`**.

## When to use this skill

- Adding webcam, IP camera, screen, or DV capture to a WinUI 3 desktop app.
- Recording captured video/audio to MP4, AVI, MOV, MPEG-TS, WMV, or animated GIF.
- Hosting the preview in a WinUI 3 surface using `VisioForge.Core.UI.WinUI.VideoView` over a Win2D `CanvasControl`.
- Shipping as MSIX (Store / sideload) or as an unpackaged WinUI 3 app.

## When NOT to use this skill

- **WPF host** → `video-capture-sdk-net-wpf` (different `VideoView` namespace and project SDK).
- **WinForms host** → `video-capture-sdk-net-winforms`.
- **Headless / background capture** → `video-capture-sdk-net-console`.
- **Cross-platform** (macOS, iOS, Android, Linux) → `media-blocks-sdk-net-{maui,avalonia,uno}`. Video Capture SDK is Windows-only.
- **Editing existing files instead of capture** → `video-edit-sdk-net-wpf`.
- **Newer "X" line on WinUI** → `video-capture-sdk-x-wpf` covers the cross-process X engine on WPF; no WinUI-specific X skill exists yet.

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

WinUI 3 apps are commonly pinned to **x64** rather than AnyCPU — both because Windows App SDK ships separate x64/x86/arm64 native binaries and because the redist packages used here are `.x64`-only. If you need x86 or arm64, swap the redist suffixes accordingly and add the matching RID to `<Platforms>` / `<RuntimeIdentifiers>`. `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` bundles the WinAppSDK runtime so end users don't have to install it separately.

### NuGet packages

WinUI requires **two** VisioForge packages — the SDK itself and the WinUI-specific `VideoView` host:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.VideoCapture.x64" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.MP4.x64" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="2026.5.4" />
</ItemGroup>
```

Unlike the WPF skill, the WinUI sample references the redist packages **explicitly** — the WinUI sample has historically had less-reliable transitive resolution under MSIX, and pinning the redist set avoids surprise "filter not registered" errors at runtime. Pin every redist to the same version as `VisioForge.DotNet.VideoCapture`; version drift between main and redist is undefined behaviour.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Simple Video Capture WinUI demo (`_SETUP/GitHub/Video Capture SDK/WinUI/CSharp/Simple Video Capture Demo WinUI/`). Changes vs upstream: the SDK icon asset (`Assets\visioforge_main_icon.ico`) and its `<None Remove>` / `<Content Update>` entries are removed; everything else is byte-identical including the MSIX tooling block and the explicit Windows App SDK / SDK build-tools pin.

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
// MainWindow.xaml.cs — inside Window_Activated, ONCE
using VisioForge.Core.UI.WinUI;

_videoView = new VideoView(canvasControl);
VideoCapture1 = new VideoCaptureCore(_videoView as IVideoView);
```

This is a structural difference from WPF (where `<my:VideoView/>` is itself the XAML element). If you copy/paste from a WPF skill, you will get "type not found" XAML errors — there is no `<vfwinui:VideoView/>` element to drop into the XAML tree.

## License registration

The SDK ships with a 30-day trial — the bundled `references/MainWindow.xaml.cs` runs in trial mode by design. To register a purchased licence, add two lines to `CreateEngine()`:

```csharp
// references/MainWindow.xaml.cs — CreateEngine()
private void CreateEngine()
{
    VideoCapture1 = new VideoCaptureCore(_videoView as IVideoView);

    // Add these two lines:
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    VideoCapture1.SetLicenseCertificateAsync(cert).GetAwaiter().GetResult();

    VideoCapture1.OnError += VideoCapture1_OnError;
}
```

Or, in async code (e.g. wrapping `CreateEngine` as `CreateEngineAsync` and calling it from `Window_Activated`):

```csharp
VideoCapture1 = new VideoCaptureCore(_videoView as IVideoView);
var cert = await File.ReadAllBytesAsync("path/to/your.vflicense");
await VideoCapture1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-window apps, every `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`.

**MSIX note**: when the `.vflicense` is bundled as Content in the package, load it via `Package.Current.InstalledLocation.Path` rather than a relative path — the working directory under MSIX is not the package install root.

## Hello-World capture

Self-contained `MainWindow` you can drop into a fresh WinUI 3 desktop project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

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
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI.WinUI;
using VisioForge.Core.VideoCapture;

namespace YourApp;

public sealed partial class MainWindow : Window
{
    private VideoCaptureCore _videoCapture;
    private VideoView _videoView;
    private bool _isInitiated;

    public MainWindow() => InitializeComponent();

    // VideoView must be constructed AFTER the Window has an HWND.
    // Window_Activated is the official sample's hook; gate with _isInitiated
    // because Activated fires repeatedly on focus changes.
    private void Window_Activated(object sender, WindowActivatedEventArgs args)
    {
        if (_isInitiated) return;
        _isInitiated = true;

        _videoView = new VideoView(canvasControl);
        _videoCapture = new VideoCaptureCore(_videoView as IVideoView);
        _videoCapture.OnError += (_, e) => System.Diagnostics.Debug.WriteLine(e.Message);
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers must catch — otherwise the exception
        // escapes to AppDomain.UnhandledException and silently terminates
        // the app. Realistic escape paths: missing native DLLs
        // (DllNotFoundException), COM init failure (COMException), or
        // unexpected init exceptions during StartAsync. Note: trial-expired,
        // device-busy, and codec-not-found don't throw — Start() returns false;
        // surface those via the StartAsync return value and the OnError event.
        try
        {
            var devices = _videoCapture.Video_CaptureDevices();
            if (devices.Count == 0) return;

            _videoCapture.Video_CaptureDevice = new VideoCaptureSource(devices[0].Name);
            // Let the SDK pick a default format. Setting Format = devices[0].VideoFormats[0].Name
            // crashes on devices that enumerate with no formats (some virtual cameras / IP shims).
            _videoCapture.Audio_RecordAudio = false;
            _videoCapture.Mode = VideoCaptureMode.VideoPreview;

            await _videoCapture.StartAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Capture failed: {ex.Message}");
        }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `MainWindow.xaml`) ships the full pattern with output format selection (AVI, WMV, MP4 CPU/GPU, GIF, MPEG-TS, MOV), recording-time display, audio routing, video effects (lightness/contrast/saturation/grayscale/invert/flip + text/image logos), and `OnError` event wiring.

## Common deployment failures

Five most common production issues in WinUI 3 — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'" (unpackaged)

**Cause**: in unpackaged mode the native DLLs from `VisioForge.DotNet.Core.Redist.*.x64` need to be in the executable's directory. Unlike WPF's `runtimes/<rid>/native/` resolution, WinUI 3 unpackaged apps published with `dotnet publish -r win-x64` sometimes leave the redist natives in a `runtimes/win-x64/native/` subfolder that the loader doesn't search.

**Fix**: publish self-contained with `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None`. That flattens the natives into the publish root. Verify by listing the publish folder — `VisioForge_*.dll` files must sit next to the .exe.

### 2. Native DLLs missing on end-user machine (MSIX)

**Cause**: MSIX strictly bounds the package contents — only what's declared as Content or comes via NuGet `runtimes/` is shipped. The redist packages register their natives correctly, but a hand-edited `.appxmanifest` or a `<Content Remove>` in the csproj can drop them.

**Fix**: do not exclude `runtimes/**` from MSIX content. After packaging, open the `.msix` (it's a ZIP), look under `\runtimes\win-x64\native\` for the VisioForge DLLs. If they're missing, the redist NuGets aren't being honoured — re-add them at the project level (don't rely on transitive only, see `references/Sample.csproj`).

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `VideoCaptureCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoCapture.SetLicenseCertificateAsync(certBytes)` after the `VideoCaptureCore` constructor and before `StartAsync` (see "License registration"). For multi-window apps, every `VideoCaptureCore` instance needs its own call. Under MSIX, load the certificate from `Package.Current.InstalledLocation.Path` — relative paths break.

### 4. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: output format depends on a redist not referenced by the project (e.g. `VisioForge.DotNet.Core.Redist.WebM.x64` for WebM, `VisioForge.DotNet.Core.Redist.FFMPEG.x64` for FFMPEG-based muxers).

**Fix**: add the corresponding redist NuGet package, pin it to the same version as the main package, and rebuild. The bundled `Sample.csproj` already includes VideoCapture, MP4, and LAV; that covers MP4 (CPU and GPU), AVI, WMV, MOV, MPEG-TS, and animated GIF.

### 5. Preview is black; `VideoView` constructed too early

**Cause**: `VideoView(canvasControl)` was called from the constructor or before the window has an HWND. The `CanvasControl` only has a render target after `Window.Activated` has fired at least once.

**Fix**: construct the `VideoView` and `VideoCaptureCore` in the `Activated` handler, gated by an `_isInitiated` flag (Activated fires every time focus changes). Both the bundled `references/MainWindow.xaml.cs` and the Hello-World snippet above use this pattern. Calling `StartAsync` from a button click — not from `Activated` itself — also avoids races where the engine starts before the canvas swapchain is ready.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (after adding `Assets/` PNGs from any WinUI 3 desktop template).
- [ ] `VisioForge_*.dll` natives from the `VideoCapture`, `MP4`, and `LAV` redists are present next to the .exe (unpackaged) or under `\runtimes\win-x64\native\` inside the `.msix` (packaged). Missing natives produce `DllNotFoundException` on the first `StartAsync`.
- [ ] WindowsAppSDK runtime is bundled (`<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>`) or installed on the target machine — otherwise the app fails to launch with a "Windows App Runtime" missing dialog before any SDK code runs.
- [ ] `Video_CaptureDevices()` returns at least one device on the target machine and preview shows a non-black frame within ~1-2 s of `StartAsync` — confirms DirectShow / Media Foundation device enumeration works under the WinUI process.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCore` (always call `VideoCapture1.Dispose()` on `Window.Closed`).
- [ ] If recording: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `Dispose`); the chosen output format's redist is referenced (MP4 needs `MP4.x64`, WebM needs `WebM.x64`, etc.).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCore` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode and aborts after 30 days with `"SDK TRIAL period (30 days) is over."`). Under MSIX, the certificate path is resolved via `Package.Current.InstalledLocation.Path`.
- [ ] If publishing unpackaged: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None` — verify `VisioForge_*.dll` files land in the publish root, not buried under `runtimes/win-x64/native/`.
- [ ] If publishing MSIX: open the `.msix` ZIP and confirm `\runtimes\win-x64\native\` contains the VisioForge + redist DLLs and that `Package.appxmanifest` declares the `runFullTrust` capability.

## Bundled references

The `references/` folder is a faithful copy of the official sample with the SDK icon stripped. Copy all of it into a fresh project folder; you'll also need the `Assets/` PNGs from any WinUI 3 desktop template (or the upstream sample) for the package to build:

- `references/Sample.csproj` — minimal working WinUI 3 csproj, version-pinned to the same NuGet release as the prose.
- `references/App.xaml` + `references/App.xaml.cs` — Application entry point.
- `references/MainWindow.xaml` — XAML with `<win2d:CanvasControl x:Name="canvasControl"/>` for the preview surface plus the device/output/effects/log Pivot UI.
- `references/MainWindow.xaml.cs` — full code-behind with capture, recording, output-format selection, video effects, recording-time display, and `OnError` wiring. Runs in trial mode by design; add `SetLicenseCertificateAsync` yourself when integrating a purchased licence.
- `references/Package.appxmanifest` — MSIX manifest (declares `runFullTrust` capability, required for Win32 P/Invoke into the SDK natives).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinUI>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-wpf` — same SDK on WPF (different `VideoView` namespace, different csproj SDK).
    - `video-capture-sdk-net-winforms` — same SDK on WinForms.
    - `video-capture-sdk-net-console` — headless capture.
    - `video-capture-sdk-x-wpf` — newer "X" line (cross-process capture, modernised pipeline) on WPF.
