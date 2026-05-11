---
name: video-capture-sdk-x-wpf
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a Windows WPF application. Covers the cross-platform NuGet package layout, project setup, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want capture/recording on WPF with an API that ports cleanly to MAUI, Avalonia, Uno, Android, iOS, macOS — for Windows-only with the legacy DirectShow stack, use video-capture-sdk-net-wpf instead.
---

# Video Capture SDK X — WPF integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a Windows WPF application. The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level capture-and-record god-object (`VideoCaptureCoreX`) that mirrors the legacy `VideoCaptureCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux / iOS / Android — the only thing that changes between platforms is the UI host (WPF here, MAUI / Avalonia / Uno / native elsewhere) and the per-OS native redist NuGet package.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the [official Simple Video Capture X sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Simple%20Video%20Capture)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding webcam, IP camera, screen, or NDI capture to a Windows WPF app **with an API that ports unchanged to other platforms** (MAUI, Avalonia, Uno, Android, iOS, macOS).
- Recording captured video/audio to MP4 (incl. fragmented), AVI, MOV, MPEG-TS, or WebM.
- Sharing capture/recording code between a Windows WPF "main" app and one or more cross-platform companion apps.
- Pipeline introspection: `VideoCaptureCoreX.GetDiagramAsImage()` returns a SkiaSharp bitmap of the live GStreamer graph (handy for support tickets).

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): use `video-capture-sdk-net-wpf`. The two SDKs ship side-by-side and can coexist in one app.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): use `media-blocks-sdk-net-wpf` — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without capturing): `media-player-sdk-net-wpf`.
- **Cross-platform host instead of WPF**: same SDK, different UI shell → `media-blocks-sdk-net-{maui,avalonia,uno}`, or use the X capture wrapper directly inside those hosts (the `VideoCaptureCoreX` API is identical across platforms).

## Project setup

### Target framework

Video Capture SDK X 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on the WPF host. Pick the highest your toolchain supports. The csproj uses the plain **`Microsoft.NET.Sdk`** (not `Microsoft.NET.Sdk.WindowsDesktop`) with `<UseWPF>true</UseWPF>` — same convention as Media Blocks, intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly. Don't switch to `WindowsDesktop`.

### NuGet packages

Three packages are required for a Windows WPF capture-and-record scenario — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy SDK uses — both `VideoCaptureCore` (legacy) and `VideoCaptureCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64`) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. Some samples reference `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` — that's a UPX-compressed variant (smaller download, slightly slower first-load). Either works; pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture X sample (`_DEMOS/Video Capture SDK X/WPF/CSharp/Simple Video Capture/`) — kept verbatim except for stripping demo-only metadata; the bundled file builds standalone against the public NuGet packages.

### Project platform

Video Capture SDK X WPF samples use `<PlatformTarget>x64</PlatformTarget>` — this differs from the legacy SDK (which uses AnyCPU). The reason is the redist packages are split per-architecture; referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous.

## Mandatory engine boot

Before any `VideoCaptureCoreX` instance is constructed (and before any `DeviceEnumerator` query), call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// MainWindow.xaml.cs — Window_Loaded
private async void Window_Loaded(object sender, RoutedEventArgs e)
{
    Title += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
    this.IsEnabled = false;
    await VisioForgeX.InitSDKAsync();
    this.IsEnabled = true;
    Title = Title.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

    // ...now safe to construct VideoCaptureCoreX, query DeviceEnumerator, etc.
}

// Window_Closing
private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    await DestroyEngineAsync();   // disposes VideoCaptureCoreX
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/MainWindow.xaml.cs` shows the canonical placement.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoCapture1.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
VideoCapture1 = new VideoCaptureCoreX(VideoView1 as IVideoView);
VideoCapture1.OnError += VideoCapture1_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await VideoCapture1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-window apps, every `VideoCaptureCoreX` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoCapture1 = new VideoCaptureCoreX(...)`.

## Hello-World capture

Minimum viable capture-and-preview snippet — a self-contained `MainWindow` you can drop into a fresh WPF project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="MainWindow" Height="450" Width="800" Loaded="Window_Loaded">
    <Grid>
        <my:VideoView x:Name="VideoView1" Background="Black" />
        <!-- add a Button x:Name="StartButton" Click="StartButton_Click" anywhere -->
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System.Linq;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCaptureX;

namespace YourApp
{
    public partial class MainWindow : Window
    {
        private VideoCaptureCoreX _videoCapture;

        public MainWindow() => InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Mandatory engine boot — see "Mandatory engine boot" above.
            await VisioForgeX.InitSDKAsync();
            _videoCapture = new VideoCaptureCoreX(VideoView1 as IVideoView);
            // For a purchased licence, add these two lines here:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await _videoCapture.SetLicenseCertificateAsync(cert);
        }

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // async-void event handlers must catch — otherwise an exception
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, registry not built (forgot InitSDKAsync), no device.
            try
            {
                var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
                if (devices.Count == 0)
                {
                    MessageBox.Show("No video capture devices found.");
                    return;
                }

                var device = devices[0];
                var format = device.VideoFormats.First();
                _videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(device)
                {
                    Format = format.ToFormat()
                };
                _videoCapture.Audio_Play = false;
                _videoCapture.Audio_Record = false;

                // Preview-only — no Outputs_Add call. For recording, see
                // references/MainWindow.xaml.cs (MP4Output, MPEGTSOutput, …).
                await _videoCapture.StartAsync();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Capture failed: {ex.Message}");
            }
        }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `references/MainWindow.xaml`) ships the full pattern with output format dialogs (MP4, MP4-fragmented, AVI, WebM, MPEG-TS, MOV), VU meter, video effects (grayscale, sepia, flip, balance, text/image overlay, LUT), recording-time display, and `OnError` wiring.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoCapture.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every `VideoCaptureCoreX` instance in the process needs its own call.

### 3. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the output format depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64` covers MP4 (libav h264), AAC, MPEG-TS, MOV, AVI, and WebM out of the box. Less common codecs (HAP, DNxHD, ProRes via plugin variants) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist (e.g. for VP9 hardware encode), add it with the same version pin as the others.

### 4. Preview is black / frozen on first frame

**Cause**: capture started before the WPF `VideoView1` element has a valid HWND, **or** `VideoCaptureCoreX` was constructed before `await VisioForgeX.InitSDKAsync()` returned.

**Fix**: defer construction and `StartAsync` to `Window_Loaded` (or later — `ContentRendered` / a button click is also fine). The bundled sample uses `Window_Loaded` for `InitSDKAsync` + `CreateEngine` and an explicit "Start" button for `StartAsync` — copy that pattern.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows the "[FIRST TIME LOAD, BUILDING THE REGISTRY...]" title for ~2-5 s (the registry build), then a webcam preview within ~1 s on subsequent launches.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCoreX` (always call `await _videoCapture.StopAsync(); await _videoCapture.DisposeAsync();` on close).
- [ ] On clean shutdown, `Window_Closing` runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] If recording to MP4: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `DisposeAsync`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window. Declares `xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<my:VideoView x:Name="VideoView1" />` along with device-/format-/output-selection controls and a VU meter.
- `references/MainWindow.xaml.cs` — full code-behind with `InitSDKAsync` boot, `DeviceEnumerator` wiring, output-format selection (MP4 / MP4-fragmented / AVI / WebM / MPEG-TS / MOV), video effects (grayscale, sepia, flip, balance, text/image overlay, LUT), VU meter, snapshot capture, pipeline-diagram export, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-wpf` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist).
    - `media-blocks-sdk-net-wpf` — same engine, lower-level graph-based API for custom pipeline topologies.
    - **Cross-platform hosts**:
        - `video-capture-sdk-x-maui` — same X SDK on .NET MAUI.
        - `video-capture-sdk-x-avalonia` — same X SDK on Avalonia.
        - `video-capture-sdk-x-uno` — same X SDK on Uno Platform.
