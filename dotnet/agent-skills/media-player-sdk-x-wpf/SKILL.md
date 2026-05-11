---
name: media-player-sdk-x-wpf
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a Windows WPF application. Covers the cross-platform NuGet package layout, project setup, license registration, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want playback on WPF with an API that ports cleanly to MAUI, Avalonia, Uno, Android, iOS, macOS — for Windows-only with the legacy DirectShow stack, use media-player-sdk-net-wpf.
---

# Media Player SDK X — WPF integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to a Windows WPF application. The X SDK shares its runtime with Media Blocks and Video Capture X (GStreamer-backed under the hood) and exposes a high-level playback god-object (`MediaPlayerCoreX`) that mirrors the legacy `MediaPlayerCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux / iOS / Android — the only thing that changes between platforms is the UI host (WPF here, MAUI / Avalonia / Uno / native elsewhere) and the per-OS native redist NuGet package.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the [official Simple Player Demo X sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding video / audio file or network-stream playback to a Windows WPF app **with an API that ports unchanged to other platforms** (MAUI, Avalonia, Uno, Android, iOS, macOS).
- Playing local media (MP4, MKV, MOV, AVI, WebM, MPEG-TS, MP3, FLAC, AAC, WAV, OGG, …) and network streams (HTTP, HTTPS, HLS, DASH, RTSP, RTMP, UDP, SRT) via the `UniversalSourceSettingsV2` URI source.
- External subtitles (SRT/SSA/ASS) via `Subtitles_ExternalFile` + `SubtitleOverlaySettings`.
- Multi-stream titles: enumerate via `Video_Streams` / `Audio_Streams`, switch live with `Video_Stream_Select` / `Audio_Stream_Select`.
- Sharing playback code between a Windows WPF "main" app and one or more cross-platform companion apps.

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): use `media-player-sdk-net-wpf`. The two SDKs ship side-by-side and can coexist in one app.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode-while-playing, runtime sink swap): use `media-blocks-sdk-net-wpf` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.
- **Capture / recording** instead of playback: `video-capture-sdk-x-wpf`.
- **Cross-platform host instead of WPF**: same SDK, different UI shell → `media-player-sdk-x-{maui,avalonia,uno}`, or use `MediaPlayerCoreX` directly inside those hosts (the API is identical across platforms).

## Project setup

### Target framework

Media Player SDK X 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on the WPF host. Pick the highest your toolchain supports. The csproj uses the plain **`Microsoft.NET.Sdk`** (not `Microsoft.NET.Sdk.WindowsDesktop`) with `<UseWPF>true</UseWPF>` — same convention as Media Blocks and Video Capture X, intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly. Don't switch to `WindowsDesktop`.

### NuGet packages

Three packages are required for a Windows WPF playback scenario — the .NET wrapper plus two native redist packages (Core runtime + libav demuxers/decoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the wrapper package the upstream Simple Player Demo X uses — `MediaPlayerCoreX` ships in it alongside `VideoCaptureCoreX` (a single wrapper assembly covers the whole X family on the Windows side). What switches you to playback is which god-object you instantiate (`MediaPlayerCoreX` here) plus the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64.UPX`) and the mandatory `VisioForgeX.InitSDKAsync()` boot below. The `.UPX` suffix on the libav redist is a UPX-compressed variant (smaller download, slightly slower first-load); the non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` works equally well — pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Player Demo X sample (`_DEMOS/Media Player SDK X/WPF/Simple Player Demo/`) — kept verbatim except for stripping demo-only metadata (icon, etc.); the bundled file builds standalone against the public NuGet packages.

### Project platform

Media Player SDK X WPF samples use `<PlatformTarget>x64</PlatformTarget>`. The reason is the redist packages are split per-architecture; referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous.

## Mandatory engine boot

Before any `MediaPlayerCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// MainWindow.xaml.cs — Window_Loaded
private async void Window_Loaded(object sender, RoutedEventArgs e)
{
    Title += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
    this.IsEnabled = false;
    await VisioForgeX.InitSDKAsync();
    this.IsEnabled = true;
    Title = Title.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

    // ...now safe to construct MediaPlayerCoreX, enumerate audio output devices, etc.
}

// Window_Closing
private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    await DestroyEngineAsync();   // disposes MediaPlayerCoreX
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/MainWindow.xaml.cs` shows the canonical placement.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync`:

```csharp
_player = new MediaPlayerCoreX(VideoView1);
_player.OnError += Player_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-window apps, every `MediaPlayerCoreX` instance needs its own `SetLicenseCertificateAsync` call before `OpenAsync`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `_player = new MediaPlayerCoreX(...)`.

## Hello-World playback

Minimum viable open-and-play snippet — a self-contained `MainWindow` you can drop into a fresh WPF project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="MainWindow" Height="450" Width="800" Loaded="Window_Loaded">
    <Grid>
        <my:VideoView x:Name="VideoView1" Background="Black" />
        <!-- add a Button x:Name="PlayButton" Click="PlayButton_Click" anywhere -->
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

namespace YourApp
{
    public partial class MainWindow : Window
    {
        private MediaPlayerCoreX _player;

        public MainWindow() => InitializeComponent();

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Mandatory engine boot — see "Mandatory engine boot" above.
            await VisioForgeX.InitSDKAsync();
            _player = new MediaPlayerCoreX(VideoView1);
            // For a purchased licence, add these two lines here:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await _player.SetLicenseCertificateAsync(cert);
        }

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // async-void event handlers must catch — otherwise an exception
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, registry not built (forgot InitSDKAsync), bad URI,
            // codec missing for the file.
            try
            {
                var source = await UniversalSourceSettingsV2.CreateAsync(
                    new Uri(@"C:\samples\!video.mp4"));
                await _player.OpenAsync(source);
                await _player.PlayAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Playback failed: {ex.Message}");
            }
        }
    }
}
```

`UniversalSourceSettingsV2.CreateAsync(Uri)` accepts both local paths (`new Uri(@"C:\path\file.mp4")`) and network URIs (`new Uri("https://…/stream.m3u8")`, `new Uri("rtsp://…")`). It probes the source up-front so codec / container detection happens before `OpenAsync` returns.

`references/MainWindow.xaml.cs` (paired with `references/MainWindow.xaml`) ships the full pattern with file picker, subtitle file picker, audio output device selection, volume slider, video / audio stream selection, loop checkbox, position-and-duration timer + seek slider, debug-mode toggle, and `OnError` / `OnStop` wiring.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 3. `OnError` fires with "Codec not found" / "Element 'X' not found" when opening a file

**Cause**: the input file uses a codec / container not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64` (or `.UPX`) covers the common set out of the box: H.264, H.265/HEVC, AAC, MP3, FLAC, Opus, Vorbis, VP8/VP9, AV1, MPEG-TS, MP4, MKV, MOV, AVI, WebM, FLV, OGG, WAV. Less common codecs (HAP, DNxHD, ProRes via plugin variants, professional audio codecs) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist, add it with the same version pin as the others. For protected streams (DRM-encrypted HLS / MSS / DASH) the X engine does not include a CDM — those streams are out of scope.

### 4. Playback shows black video / freezes on first frame

**Cause**: `OpenAsync` was called before the WPF `VideoView1` element had a valid HWND, **or** `MediaPlayerCoreX` was constructed before `await VisioForgeX.InitSDKAsync()` returned, **or** the URI passed to `UniversalSourceSettingsV2.CreateAsync` does not resolve / the source is video-less (audio-only file with no video track) and the app expects a frame.

**Fix**: defer construction and `OpenAsync` to `Window_Loaded` (or later — `ContentRendered` / a button click is also fine). The bundled sample uses `Window_Loaded` for `InitSDKAsync` + `CreateEngine` and an explicit "Start" button for `OpenAsync` + `PlayAsync` — copy that pattern. For audio-only files, this is expected (there is no video frame to render); the audio track plays normally.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows the "[FIRST TIME LOAD, BUILDING THE REGISTRY...]" title for ~2-5 s (the registry build), then a video frame appears within ~1 s of clicking Start on subsequent launches.
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCoreX` (always call `await _player.StopAsync(); await _player.DisposeAsync();` on close — the bundled sample's `DestroyEngineAsync` does both).
- [ ] On clean shutdown, `Window_Closing` runs `DestroyEngineAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] Seeking with the timeline slider works smoothly while `_timer` updates the position label (re-entry guarded by `_timerFlag`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window. Declares `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<WPF:VideoView x:Name="VideoView1" />` along with file / subtitle pickers, transport buttons (Start/Resume/Pause/Stop), a seek slider, audio output device combo, volume slider, video / audio stream combos, and a loop checkbox.
- `references/MainWindow.xaml.cs` — full code-behind with `InitSDKAsync` boot, `MediaPlayerCoreX` lifecycle (`CreateEngine` / `DestroyEngineAsync`), `UniversalSourceSettingsV2` source creation, external subtitles via `Subtitles_ExternalFile`, audio output device selection, video / audio stream enumeration via `OnStreamsInfoAvailable` and live switching via `Video_Stream_Select` / `Audio_Stream_Select`, position / duration timer with seek slider, loop, debug-mode toggle, and `OnError` / `OnStop` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-net-wpf` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist).
    - `media-blocks-sdk-net-wpf` — same engine, lower-level graph-based API for custom pipeline topologies.
    - `video-capture-sdk-x-wpf` — same X engine, capture-and-record god-object instead of playback.
    - **Cross-platform hosts**:
        - `media-player-sdk-x-maui` — same X SDK on .NET MAUI.
        - `media-player-sdk-x-avalonia` — same X SDK on Avalonia.
        - `media-player-sdk-x-uno` — same X SDK on Uno Platform.
