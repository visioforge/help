---
name: video-capture-sdk-net-wpf
description: Integrate VisioForge Video Capture SDK .NET into a Windows WPF application. Covers the single NuGet package, project setup, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when adding webcam, IP camera, screen, or DV capture to a WPF app on Windows.
---

# Video Capture SDK .NET — WPF integration

This skill helps you add **VisioForge Video Capture SDK .NET** to a Windows WPF application. It covers webcam, IP camera, screen, and DV-camera capture with preview, recording, and snapshot. The SDK is Windows-only (DirectShow / Media Foundation under the hood) — for cross-platform capture (macOS, iOS, Android, Linux), use one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills instead.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Video Capture sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Simple%20Video%20Capture)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Adding webcam, IP camera, screen, or DV capture to a Windows WPF app.
- Recording captured video/audio to MP4, AVI, MOV, MPEG-TS, WebM, or WMV.
- Taking still snapshots from a live capture stream.
- Showing the preview in a WPF surface using `VideoView`.

## When NOT to use this skill

- **Cross-platform**: target macOS, iOS, Android, or Linux → use `media-blocks-sdk-net-{maui,avalonia,uno}` instead. Video Capture SDK is Windows-only.
- **Editing instead of capture**: cut, merge, transcode existing files → `video-edit-sdk-net-wpf`.
- **Playback only**: play files / streams without capturing → `media-player-sdk-net-wpf`.
- **WinForms instead of WPF**: same SDK, different UI host → `video-capture-sdk-net-winforms`.

## Project setup

### Target framework

Video Capture SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj must use the **`Microsoft.NET.Sdk.WindowsDesktop`** SDK with `<UseWPF>true</UseWPF>`.

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for a basic capture-and-record scenario.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
```

If you add codec-heavy outputs (WebM, certain GPU-accelerated MP4 paths), add the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture sample (`_DEMOS/Video Capture SDK/WPF/CSharp/Simple Video Capture/`). Changes vs upstream: the in-repo `<ProjectReference>` is swapped for a `<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />`; demo-only properties are removed (`<BaseIntermediateOutputPath>`, `<TreatWarningsAsErrors>`, per-TFM `<NoWarn>` blocks, `<DefaultItemExcludes>obj/**`, the analyzer block `<EnableNETAnalyzers>` / `<AnalysisLevel>` / `<NoWarn>S2325</NoWarn>`); the dead `Condition='net472'` ItemGroup and the legacy `<ProductVersion>` / `<ProjectTypeGuids>` fields (ignored by the modern SDK) are dropped; `<AssemblyName>` is shortened to `WPF Simple Video Capture`. The bundled file builds standalone against the public NuGet package.

### Project platform

Use `<Platforms>AnyCPU</Platforms>` (the default). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below.

## License registration

The SDK ships with a 30-day trial — the bundled `references/Window1.xaml.cs` runs in trial mode by design (the upstream demo never sets a licence anywhere). To register a purchased licence in the bundled reference, add two lines to its existing `CreateEngineAsync()` method right after the `CreateAsync` call:

```csharp
// references/Window1.xaml.cs — CreateEngineAsync(), at line ~109
private async Task CreateEngineAsync()
{
    VideoCapture1 = await VideoCaptureCore.CreateAsync(VideoView1 as IVideoView);

    // Add these two lines:
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    await VideoCapture1.SetLicenseCertificateAsync(cert);

    VideoCapture1.OnError += VideoCapture1_OnError;
}
```

If you're starting from scratch without the bundled reference, the same pattern applies in your own `MainWindow.xaml.cs` — call `await videoCapture.SetLicenseCertificateAsync(certBytes)` after `await VideoCaptureCore.CreateAsync(...)` and before `StartAsync`. (See the Hello-World snippet below for a full standalone `MainWindow`.)

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-window apps, every `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World capture

Minimum viable capture-and-preview snippet — a self-contained `MainWindow` you can drop into a fresh WPF project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` below with your project's `<RootNamespace>` from the csproj:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <my:VideoView x:Name="VideoView1" Background="Black" />
        <!-- add a Button x:Name="StartButton" Click="StartButton_Click" anywhere -->
    </Grid>
</Window>
```

Code-behind:

```csharp
// MainWindow.xaml.cs
using System;
using System.Windows;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI.WPF;
using VisioForge.Core.VideoCapture;

namespace YourApp
{
    public partial class MainWindow : Window
    {
        private VideoCaptureCore _videoCapture;
        // (The bundled `references/Window1.xaml.cs` calls this field `VideoCapture1`;
        //  use the same name there if you start from the bundled template, otherwise
        //  the snippet below won't merge cleanly.)

        public MainWindow() => InitializeComponent();

        private async void StartButton_Click(object sender, RoutedEventArgs e)
        {
            // async-void event handlers must catch — otherwise the exception
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Realistic escape paths from this snippet: missing
            // native DLLs (DllNotFoundException), COM init failure
            // (COMException), or unexpected init exceptions during CreateAsync.
            // Note: trial-expired, device-busy, and codec-not-found don't
            // throw — Start() returns false; surface those via the StartAsync
            // return value and the OnError event (see references/Window1.xaml.cs).
            try
            {
                // VisioForge.Core.UI.WPF.VideoView already implements IVideoView,
                // so passing VideoView1 directly is fine — no cast needed.
                _videoCapture = await VideoCaptureCore.CreateAsync(VideoView1);

                // Note: Video_CaptureDevices() is synchronous — there is no Async overload.
                var devices = _videoCapture.Video_CaptureDevices();
                if (devices.Count == 0)
                {
                    MessageBox.Show("No video capture devices found.");
                    _videoCapture.Dispose();
                    _videoCapture = null;
                    return;
                }

                _videoCapture.Video_CaptureDevice = new VideoCaptureSource(devices[0].Name);
                // Let the SDK pick a default format. Setting Format = devices[0].VideoFormats[0].Name
                // crashes on devices that enumerate with no formats (some virtual cameras / IP shims).
                _videoCapture.Audio_RecordAudio = false;

                // Preview-only, no recording. For recording, set Mode = VideoCapture
                // and Output_Format / Output_Filename appropriately — see references/Window1.xaml.cs.
                _videoCapture.Mode = VideoCaptureMode.VideoPreview;

                await _videoCapture.StartAsync();
            }
            catch (Exception ex)
            {
                // Dispose the half-built engine on failure so the next click
                // starts from a clean slate (otherwise the field stays non-null
                // and the next CreateAsync overwrites it without disposing).
                if (_videoCapture != null)
                {
                    try { _videoCapture.Dispose(); } catch (Exception) { /* Log the disposal failure here, e.g. via your logger. */ }
                    _videoCapture = null;
                }
                MessageBox.Show($"Capture failed: {ex.Message}");
            }
        }
    }
}
```

`references/Window1.xaml.cs` (paired with `Window1.xaml`) ships the full pattern with output format dialogs (MP4, AVI, MOV, MPEG-TS, GIF), recording-time display, and `OnError` event wiring.

## Optional codec packages

You only need these if your output format actually uses them. Default MP4 (Media Foundation hardware encoder) does not require any explicit redist — it ships in the main package.

| Output / source | Add to csproj |
|---|---|
| WebM (VP8/VP9 + Vorbis/Opus) | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| FFmpeg-based output (custom muxers, network sinks) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| LAV-based decoding (uncommon legacy formats) | `VisioForge.DotNet.Core.Redist.LAV.x64` |
| VLC-based source | `VisioForge.DotNet.Core.Redist.VLC.x64` |
| Xiph (Vorbis/Theora) standalone codecs | `VisioForge.DotNet.Core.Redist.XIPH.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.VideoCapture`** — version drift between main and redist is undefined behaviour.

For a 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist package you need.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder.

**Fix**: keep `<Platforms>AnyCPU</Platforms>` (default). If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported, the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `VideoCaptureCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoCapture.SetLicenseCertificateAsync(certBytes)` after the `CreateAsync` call and before `StartAsync` (see "License registration" above). For multi-window apps, every `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call. The SDK never shows a blocking "License required" dialog. For `AudioCapture` / `AudioPreview` modes only, it surfaces the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`; for any mode it aborts startup with `"SDK TRIAL period (30 days) is over."` once the 30-day window elapses.

### 3. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: output format depends on a redist not referenced by the project (see "Optional codec packages" above).

**Fix**: add the corresponding redist NuGet package and rebuild. For example, recording to WebM without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

### 4. Preview is black / frozen on first frame

**Cause**: capture started before the WPF `VideoView1` element has a valid HWND. Most often happens when `StartAsync` runs from a constructor or `Loaded`-handler too early.

**Fix**: defer to `ContentRendered` (or hook a button click) before calling `StartAsync`. The official sample uses an explicit "Start" button — copy that pattern.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Running the app shows a webcam preview within ~1 s on a machine with a real or virtual webcam.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCore` (always call `await _videoCapture.StopAsync(); _videoCapture.Dispose();` on close).
- [ ] If recording to MP4: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `Dispose`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCore` instance before `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="Window1.xaml"`).
- `references/Window1.xaml` — XAML for the main window. Declares `xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<my:VideoView Name="VideoView1" />`.
- `references/Window1.xaml.cs` — full code-behind with capture, recording, snapshot, output-format dialogs, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-winforms` — same SDK on WinForms.
    - `video-capture-sdk-x-wpf` — newer "X" line on WPF (cross-process capture, modernised pipeline; covers a different feature set).
    - `media-blocks-sdk-net-wpf` — alternative when you need a custom media pipeline rather than the high-level capture-and-record API.
