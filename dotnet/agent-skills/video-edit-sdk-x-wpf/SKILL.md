---
name: video-edit-sdk-x-wpf
description: Integrate VisioForge Video Edit SDK X (cross-platform editor edition) into a Windows WPF application. Covers the timeline model, the cross-platform NuGet package layout, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want non-linear editing on WPF with an API that ports cleanly to Avalonia, Console, WinForms — for Windows-only with the legacy DirectShow stack, use video-edit-sdk-net-wpf instead.
---

# Video Edit SDK X — WPF integration

This skill helps you add **VisioForge Video Edit SDK X** — the cross-platform "X" edition of the non-linear editor (NLE) — to a Windows WPF application. The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level NLE god-object (`VideoEditCoreX`) that mirrors the legacy `VideoEditCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux — the only thing that changes between platforms is the UI host (WPF here, Avalonia / WinForms / Console elsewhere) and the per-OS native redist NuGet package.

The SDK is a non-linear editor: it cuts, trims, joins, transcodes, and applies effects to **existing** video and audio files. It does **not** capture from cameras or screen — for live capture see `video-capture-sdk-x-wpf`.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the [official Video Join Demo X sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/WPF/CSharp/Video%20Join%20Demo%20X)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Joining (concatenating) multiple videos / audio / images into one output **with an API that ports unchanged to other platforms** (Avalonia, WinForms, Console).
- Transcoding from one container/codec to another (MP4, WebM, AVI, MKV, WMV, MP3, M4A, FLAC, …).
- Building a slideshow from images + audio.
- Trimming or cutting segments of existing files (the timeline accepts in/out times per input).
- Sharing edit/transcode code between a Windows WPF "main" app and one or more cross-platform companion apps.
- Pipeline introspection via `Debug_Mode = true` (logs go to `Debug_Dir`) — useful for support tickets.

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist, includes `FastEdit_CutFileAsync` stream-copy path): use `video-edit-sdk-net-wpf`. The two SDKs ship side-by-side and can coexist in one app.
- **Live capture** from webcam / IP camera / screen: `video-capture-sdk-x-wpf` (cross-platform) or `video-capture-sdk-net-wpf` (legacy Windows).
- **Custom pipeline topology** (per-block decoder/filter/sink control): use `media-blocks-sdk-net-wpf` — `VideoEditCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without editing): `media-player-sdk-net-wpf`.
- **Cross-platform host instead of WPF**: same SDK, different UI shell → `video-edit-sdk-x-avalonia`, `video-edit-sdk-x-winforms`, `video-edit-sdk-x-console`. The `VideoEditCoreX` API is identical across hosts.

## Project setup

### Target framework

Video Edit SDK X 2026.x supports `net472`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on the WPF host. Pick the highest your toolchain supports. The csproj uses the plain **`Microsoft.NET.Sdk`** (not `Microsoft.NET.Sdk.WindowsDesktop`) with `<UseWPF>true</UseWPF>` — same convention as Media Blocks and Video Capture X, intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly. Don't switch to `WindowsDesktop`.

### NuGet packages

Three packages are required for a Windows WPF edit-and-transcode scenario — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoEdit` is the **same wrapper package** the legacy SDK uses — both `VideoEditCore` (legacy) and `VideoEditCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]`) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. The `.UPX` suffix is a UPX-compressed variant (smaller download, slightly slower first-load); the non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` works identically. Pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Video Join Demo X sample (`x:/MediaFrameworkDotNet/_SETUP/GitHub/Video Edit SDK X/WPF/CSharp/Video Join Demo X/`). Changes vs upstream: the per-target-framework csproj fan-out (net472/net5/net6/net7/net8/net9/net10) is collapsed to a single `net10.0-windows` row; the upstream icon resource (`<Resource Include="visioforge_main_icon.ico" />`) and the matching `<Window Icon="..." />` attribute were stripped — that's the SDK's own branding icon and shouldn't ship into a user's app via this skill. The bundled file builds standalone against the public NuGet packages.

### Project platform

Video Edit SDK X WPF samples use `<PlatformTarget>x64</PlatformTarget>` — this differs from the legacy SDK (which uses AnyCPU). The reason is the redist packages are split per-architecture; referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous.

## Mandatory engine boot

Before any `VideoEditCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// MainWindow.xaml.cs — Window_Loaded
private async void Window_Loaded(object sender, RoutedEventArgs e)
{
    Title += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
    this.IsEnabled = false;
    await VisioForgeX.InitSDKAsync();
    this.IsEnabled = true;
    Title = Title.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

    CreateEngine();   // now safe to construct VideoEditCoreX
}

// Window_Closing
private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    VideoEdit1?.Stop();
    DestroyEngine();              // disposes VideoEditCoreX
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/MainWindow.xaml.cs` shows the canonical placement.

## Timeline model

`VideoEditCoreX` exposes a single editing path: a list of input files, each added with one of the `Input_Add*` methods, concatenated in insertion order, then transcoded to the format set in `Output_Format`. There is **no** `FastEdit_CutFileAsync` stream-copy path on the X engine — every render decodes and re-encodes (this is the cost of the cross-platform engine). For a stream-copy-only cut on Windows, use the legacy `video-edit-sdk-net-wpf` skill.

The three input-add methods on `VideoEditCoreX`:

- `Input_AddAudioVideoFile(filename)` — for video files (with or without audio).
- `Input_AddAudioFile(filename)` — for audio-only files.
- `Input_AddImageFile(filename, TimeSpan duration)` — for stills (slideshow path); `duration` is how long the image stays on screen.

Concrete timeline build (multi-file join to MP4):

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.UI;
using VisioForge.Core.VideoEditX;

await VisioForgeX.InitSDKAsync();   // mandatory — see "Mandatory engine boot"

var core = new VideoEditCoreX(VideoView1 as IVideoView);
core.OnError    += (s, e) => Console.WriteLine($"{e.Message} ({e.CallSite})");
core.OnProgress += (s, e) => Console.WriteLine($"Progress: {e.Progress}%");
core.OnStop     += (s, e) => Console.WriteLine($"Done. Successful={e.Successful}");

// Optional: force a target frame size and rate before adding files.
core.Output_VideoSize      = new VisioForge.Core.Types.Size(1280, 720);
core.Output_VideoFrameRate = new VideoFrameRate(30);

// Three sources concatenated in insertion order:
core.Input_AddAudioVideoFile(@"C:\clips\a.mp4");
core.Input_AddImageFile(@"C:\clips\title.png", TimeSpan.FromSeconds(3));
core.Input_AddAudioVideoFile(@"C:\clips\b.mp4");

core.Output_Format = new MP4Output(@"C:\out\joined.mp4");

core.Start();   // OnProgress fires; OnStop fires when finished
```

Key types: `MP4Output` / `WebMOutput` / `AVIOutput` / `MKVOutput` / `WMVOutput` / `DVOutput` / `WAVOutput` / `MP3Output` / `M4AOutput` / `WMAOutput` / `OGGVorbisOutput` / `FLACOutput` / `SpeexOutput` (all under `VisioForge.Core.Types.X.Output`). To **preview** the timeline in the `VideoView` rather than render to a file, set `Output_Format = null` before `Start()`. Clear the timeline between runs with `Input_Clear_List()`.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoEdit1.SetLicenseCertificateAsync(certBytes)` on every `VideoEditCoreX` instance, after the constructor and before `Start()`:

```csharp
VideoEdit1 = new VideoEditCoreX(VideoView1 as IVideoView);
VideoEdit1.OnError += VideoEdit1_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await VideoEdit1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-window apps, every `VideoEditCoreX` instance needs its own `SetLicenseCertificateAsync` call before `Start()`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoEdit1 = new VideoEditCoreX(...)`.

## Hello-World join

The bundled `references/` is the full hello-world: pick multiple files (video, audio, images), pick output format and filename, click Start, the SDK joins them and writes the output. For a minimal preview-only snippet you can drop into a fresh WPF project, copy the timeline build above into a button click, ensure `Window_Loaded` runs `await VisioForgeX.InitSDKAsync()` before `new VideoEditCoreX(...)`, and add `<wpf:VideoView x:Name="VideoView1" Background="Black" />` to your XAML (declare `xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` on the `<Window>` root).

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoEditCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await VideoEdit1.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start()` (see "License registration" above). Every `VideoEditCoreX` instance in the process needs its own call.

### 3. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the chosen `Output_Format` depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]` covers MP4 (libav h264), AAC, MPEG-TS, MOV, AVI, MKV, WMV, MP3, FLAC, Ogg Vorbis, Speex, and WebM out of the box. Less common codecs (HAP, DNxHD, ProRes via plugin variants) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist, add it with the same version pin as the others.

### 4. Output file is 0 bytes / corrupt after `OnStop` fires

**Cause**: the app exited / disposed `VideoEditCoreX` before `OnStop` had a chance to flush the muxer. `Start()` returns to the caller as soon as the pipeline starts — completion is signalled asynchronously through `OnStop`.

**Fix**: do not call `Dispose()` (or close the window without cancelling) until `OnStop` has fired with `e.Successful == true`. The bundled `references/MainWindow.xaml.cs` calls `_core.Stop()` then `Dispose()` from `Window_Closing` — for a long-running render, hold the close (cancel `e.Cancel = true` on first close attempt) until you receive the `OnStop` callback.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows the "[FIRST TIME LOAD, BUILDING THE REGISTRY...]" title for ~2-5 s (the registry build), then opens to the editor UI within ~1 s on subsequent launches.
- [ ] Adding a small MP4, picking MP4 output, and clicking Start writes a playable output file and `OnStop` fires with `e.Successful == true`.
- [ ] `OnProgress` ticks 0..100 during the run; the output file plays in any standard player.
- [ ] Stopping and restarting a render from the UI does not leak `VideoEditCoreX` (always call `VideoEdit1.Stop(); VideoEdit1.Dispose();` after `OnStop`).
- [ ] On clean shutdown, `Window_Closing` runs `Stop → DestroyEngine → VisioForgeX.DestroySDK()` in that order.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCoreX` instance before its `Start()` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window. Declares `xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<wpf:VideoView x:Name="VideoView1" />` along with input file list, resize / frame-rate controls, output-format selection, debug / telemetry checkboxes, progress bar, and log textbox.
- `references/MainWindow.xaml.cs` — full code-behind with `InitSDKAsync` boot, `VideoEditCoreX` engine lifecycle (`CreateEngine` / `DestroyEngine`), input-file enumeration via `FilenameHelper.IsImageFile` / `IsAudioFile`, output-format selection (MP4 / WebM / AVI / MKV / WMV / DV / WAV / MP3 / M4A / WMA / OGG Vorbis / FLAC / Speex), preview vs. convert mode toggle, and `OnError` / `OnProgress` / `OnStop` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.) The upstream sample's `Window` `Icon="visioforge_main_icon.ico"` attribute and the matching `<Resource Include>` in `Sample.csproj` were deliberately stripped — the SDK's own branding icon shouldn't ship into a user's app via this skill.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-net-wpf` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist, includes `FastEdit_CutFileAsync` stream-copy path).
    - `video-capture-sdk-x-wpf` — same engine for live capture/recording (when you need to record from a webcam / IP camera / screen, not edit existing files).
    - `media-blocks-sdk-net-wpf` — same engine, lower-level graph-based API for custom pipeline topologies.
    - **Cross-platform hosts**:
        - `video-edit-sdk-x-avalonia` — same X SDK on Avalonia.
        - `video-edit-sdk-x-winforms` — same X SDK on WinForms.
        - `video-edit-sdk-x-console` — same X SDK headless.
