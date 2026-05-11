---
name: media-player-sdk-x-winui
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a WinUI 3 (Windows App SDK) application. Covers the WinUI-specific VideoView control, the cross-platform NuGet package layout, project setup, license registration, MSIX packaging quirks, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use for WinUI 3 with the X-family API — for the legacy DirectShow stack use media-player-sdk-net-winui.
---

# Media Player SDK X — WinUI 3 integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the playback SDK — to a Windows App SDK / WinUI 3 desktop application. The X SDK shares its runtime with Media Blocks and Video Capture X (GStreamer-backed under the hood) and exposes a high-level player god-object (`MediaPlayerCoreX`) that mirrors the legacy `MediaPlayerCore` API but runs on the cross-platform engine. The same C# code targets Windows / macOS / Linux / iOS / Android — only the UI host (WinUI 3 here, MAUI / Avalonia / Uno / native elsewhere) and the per-OS native redist NuGet package change.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`**, Windows App SDK **`Microsoft.WindowsAppSDK 1.8.251106002`** (matches the [official Simple Media Player WinUIX sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinUI)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding video / audio playback (local file, HTTP/HLS/DASH/RTSP/RTMP stream, memory stream) to a Windows WinUI 3 desktop app **with an API that ports unchanged to other platforms** (MAUI, Avalonia, Uno, Android, iOS, macOS).
- Frame-accurate seeking, variable-rate playback, audio output device selection, volume / balance, basic video effects, and screenshot / frame-grab from a running pipeline.
- Shipping as MSIX (Store / sideload) or as an unpackaged WinUI 3 app while sharing the same `MediaPlayerCoreX` code with cross-platform companion apps.
- Pipeline introspection: `MediaPlayerCoreX.GetDiagramAsImage()` returns a SkiaSharp bitmap of the live GStreamer graph (handy for support tickets).

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist) on WinUI: use `media-player-sdk-net-winui`. The two SDKs ship side-by-side and can coexist in one app.
- **WPF host** instead of WinUI 3: same X SDK → `media-player-sdk-x-wpf`.
- **Capture / recording** (webcam, IP camera, screen, NDI): use `video-capture-sdk-x-winui`. `MediaPlayerCoreX` is playback-only; it does not have device enumeration or `Outputs_Add`.
- **Non-linear editing / timeline composition** (cuts, transitions, render to file): use `video-edit-sdk-x-*` skills.
- **Cross-platform host instead of WinUI 3**: same SDK, different UI shell → `media-player-sdk-x-{maui,avalonia,uno}`.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): the high-level `MediaPlayerCoreX` doesn't expose the underlying graph — drop down to `media-blocks-sdk-net-wpf` (the WinUI WindowsAppSDK host pattern works the same way for the engine itself; only the `VideoView` wiring differs).

## Project setup

### Target framework and platform

WinUI 3 desktop apps use the plain **`Microsoft.NET.Sdk`** SDK (not `WindowsDesktop`) plus `<UseWinUI>true</UseWinUI>`. The bundled `references/Sample.csproj` targets:

```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
<Platforms>x64</Platforms>
<RuntimeIdentifiers>win10-x64</RuntimeIdentifiers>
<UseWinUI>true</UseWinUI>
<EnablePreviewMsixTooling>true</EnablePreviewMsixTooling>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

Note the bundled csproj uses `net10.0-windows10.0.19041.0` — the upstream sample was on `net7.0-windows10.0.19041.0` (.NET 7 reached end-of-support in May 2024). Bumping to `net10.0` is the only change vs upstream beyond the SDK-icon strip; if you need to reproduce the upstream sample byte-for-byte (e.g. to reproduce an issue), revert the `TargetFramework` line. The wrapper / redist NuGets resolve identically against both.

WinUI 3 X-SDK apps are pinned to **x64** (not AnyCPU): the WinAppSDK ships separate x64/x86/arm64 native binaries, and the X redist NuGets are split per-architecture. Pinning `<Platforms>x64</Platforms>` and matching the redist suffix makes runtime resolution unambiguous. For x86, swap the redist suffix accordingly (`.x86`); no arm64 redist exists today on the X line — Windows-on-ARM is not supported by the cross-platform engine on Windows yet. `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` bundles the WinAppSDK runtime so end users don't need a separate install.

`<EnablePreviewMsixTooling>true</EnablePreviewMsixTooling>` is the upstream sample's choice — the Video Capture sibling skill uses `<EnableMsixTooling>true</EnableMsixTooling>` (the non-preview switch). Both work; keep whichever the upstream sample uses for the SDK you're integrating, because the matching `<ProjectCapability Include="Msix" />` block at the bottom of the csproj is gated on the same property.

### NuGet packages

Four packages are required for a WinUI 3 playback scenario — the .NET wrapper, the WinUI-specific `VideoView` host, plus two native redist packages (Core runtime + libav demuxers/decoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.MediaPlayer` is the **same wrapper package** the legacy SDK uses — both `MediaPlayerCore` (legacy DirectShow/MF) and `MediaPlayerCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]`) plus constructing `MediaPlayerCoreX` (not `MediaPlayerCore`). The upstream WinUI sample uses `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` — that's a UPX-compressed variant (smaller download, slightly slower first-load). The non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable; pick one and stay consistent within the project.

`VisioForge.DotNet.Core.UI.WinUI` provides the `VisioForge.Core.UI.WinUI.VideoView` host needed to render the X engine into a Win2D `CanvasControl`. WinUI 3 needs this companion package — it is not pulled in transitively by the main wrapper.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Simple Media Player WinUIX sample (`_SETUP/GitHub/Media Player SDK X/WinUI/Simple Media Player WinUI/`). Changes vs upstream: the SDK icon asset (`Assets\visioforge_main_icon.ico`), its `<ApplicationIcon>` element, and its `<Content Update>` entry are removed; `TargetFramework` is bumped from `net7.0` to `net10.0`. Everything else is byte-identical including the MSIX tooling block and the explicit Windows App SDK / SDK build-tools pin.

## Engine boot — the upstream pattern vs the safer pattern

Unlike the Video Capture X sibling, the upstream Media Player WinUI sample **does not** call `await VisioForgeX.InitSDKAsync()` and constructs `VideoView` + `MediaPlayerCoreX` directly in the `MainWindow` constructor. That works because the player engine lazy-builds the GStreamer plugin registry on the first `OpenAsync` call rather than on engine construction — the player can survive a missing `InitSDKAsync` where capture (which queries `DeviceEnumerator` immediately) cannot.

The bundled `references/MainWindow.xaml.cs` keeps the upstream pattern verbatim. If you hit "DLL not found" / "no element X" errors on first run (see Common deployment failures #1), switch to the safer Activated-gated pattern below — same one used by the Video Capture skill:

```csharp
// MainWindow.xaml — add: Activated="Window_Activated"
// MainWindow.xaml.cs — replace ctor body that constructs VideoView/Player
private bool _isInitiated;

private async void Window_Activated(object sender, WindowActivatedEventArgs args)
{
    if (_isInitiated) return;
    _isInitiated = true;

    await VisioForgeX.InitSDKAsync();          // explicit boot

    _videoView = new VideoView(canvasControl);
    MediaPlayer1 = new MediaPlayerCoreX(_videoView);
    MediaPlayer1.Audio_Play = true;
    MediaPlayer1.OnError += MediaPlayer1_OnError;
    Title = $"... v{MediaPlayerCoreX.SDK_Version}";

    InitTimer();
}
```

`InitSDKAsync` builds the GStreamer plugin-registry cache on first call (~2-5 s on a fresh machine, instant thereafter). Mirror it with `VisioForgeX.DestroySDK()` on shutdown — the upstream sample already does this in `MainWindow_Closed` after `await MediaPlayer1.DisposeAsync()`. The Activated event fires on every focus change, so the `_isInitiated` flag is mandatory to avoid double-construction.

## VideoView in WinUI 3

The WinUI VideoView is **not a XAML control** the way the WPF one is — it's a thin `IVideoView` wrapper around a Win2D `CanvasControl`. Layout the `CanvasControl` in XAML and pass it to the `VideoView` constructor in code:

```xml
<!-- MainWindow.xaml -->
xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml"
...
<win2d:CanvasControl x:Name="canvasControl" HorizontalAlignment="Stretch" VerticalAlignment="Stretch"/>
```

```csharp
// MainWindow.xaml.cs — once, before any OpenAsync call
using VisioForge.Core.UI.WinUI;

_videoView = new VideoView(canvasControl);
MediaPlayer1 = new MediaPlayerCoreX(_videoView);
```

This is a structural difference from WPF (where `<my:VideoView/>` is itself the XAML element). If you copy/paste from a WPF X skill, you will get "type not found" XAML errors — there is no `<vfwinui:VideoView/>` element to drop into the XAML tree. Note the upstream `MainWindow.xaml` declares `xmlns:winui="using:VisioForge.Core.UI.WinUI"` but never uses it — that namespace alias is harmless and can be removed.

## Supported input

`MediaPlayerCoreX` opens sources via `UniversalSourceSettings.CreateAsync(uriOrPath)`, which auto-detects the right demuxer based on the URI scheme and the file's container:

- **Local files**: anything the bundled libav redist can demux — MP4, MKV, MOV, AVI, MPEG-TS, WebM, FLV, MPEG-PS, plus most audio-only containers (MP3, M4A, FLAC, OGG, WAV).
- **HTTP / HTTPS progressive**: MP4 / MKV / WebM / MPEG-TS over plain HTTP.
- **HLS** (`.m3u8`) and **MPEG-DASH** (`.mpd`).
- **RTSP** (`rtsp://`), **RTMP** (`rtmp://`), **SRT** (`srt://`), **MMS / MMSH**.
- **In-memory streams** via the appropriate `*MemorySourceSettings` types in `VisioForge.Core.Types.X.Sources`.

For codecs requiring a different plugin family (HAP, DNxHD, ProRes via plugin variants), the default `Libav.Windows.x64[.UPX]` redist may not be enough — see Common deployment failures #4.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await MediaPlayer1.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync`:

```csharp
MediaPlayer1 = new MediaPlayerCoreX(_videoView);
MediaPlayer1.OnError += MediaPlayer1_OnError;

var cert = await File.ReadAllBytesAsync("path/to/your.vflicense");
await MediaPlayer1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-window apps, every `MediaPlayerCoreX` instance needs its own `SetLicenseCertificateAsync` call before `OpenAsync`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

**MSIX note**: when the `.vflicense` is bundled as Content in the package, load it via `Package.Current.InstalledLocation.Path` rather than a relative path — the working directory under MSIX is not the package install root.

The bundled `references/MainWindow.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `MainWindow()` right after `MediaPlayer1 = new MediaPlayerCoreX(...)`.

## Hello-World playback

Minimum viable open-and-play snippet — a self-contained `MainWindow` you can drop into a fresh WinUI 3 desktop project. (For the full feature set including pause/resume/seek/position display, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:win2d="using:Microsoft.Graphics.Canvas.UI.Xaml">
    <Grid>
        <Grid.RowDefinitions>
            <RowDefinition Height="*"/>
            <RowDefinition Height="Auto"/>
        </Grid.RowDefinitions>
        <win2d:CanvasControl x:Name="canvasControl" Background="Black"/>
        <Button Grid.Row="1" x:Name="btPlay" Content="Play sample" Click="btPlay_Click" Margin="12"/>
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System;
using System.Linq;
using Microsoft.UI.Xaml;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.WinUI;

namespace YourApp;

public sealed partial class MainWindow : Window
{
    private MediaPlayerCoreX _player;
    private VideoView _videoView;

    public MainWindow()
    {
        InitializeComponent();
        Closed += (_, _) => Shutdown();

        _videoView = new VideoView(canvasControl);
        _player = new MediaPlayerCoreX(_videoView);
        _player.Audio_Play = true;
        _player.OnError += (_, e) => System.Diagnostics.Debug.WriteLine(e.Message);

        // For a purchased licence:
        //   var cert = await System.IO.File.ReadAllBytesAsync("your.vflicense");
        //   await _player.SetLicenseCertificateAsync(cert);
    }

    private async void btPlay_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers must catch — otherwise an exception
        // escapes to AppDomain.UnhandledException and silently terminates
        // the app. Common triggers on first run: trial expired, missing
        // native DLLs, registry not built, codec unsupported, file not found.
        try
        {
            var sink = (await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound))
                       .FirstOrDefault();
            if (sink != null)
                _player.Audio_OutputDevice = new AudioRendererSettings(sink);

            // File path or URL: HTTP/HLS/DASH/RTSP/RTMP all work here.
            var src = await UniversalSourceSettings.CreateAsync(@"C:\sample\test.mp4");
            await _player.OpenAsync(src);
            await _player.PlayAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Play failed: {ex.Message}");
        }
    }

    private async void Shutdown()
    {
        try
        {
            if (_player != null)
            {
                await _player.StopAsync();
                await _player.DisposeAsync();
            }
            VisioForgeX.DestroySDK();
        }
        catch { /* swallow on shutdown */ }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `references/MainWindow.xaml`) ships the full pattern with file picker, pause/resume/stop, position slider, time display, and `OnError` wiring.

## Common deployment failures

Five most common production issues in WinUI 3 — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X" (unpackaged)

**Cause**: the X redist NuGets aren't being honoured (the WinUI loader sometimes leaves natives in `runtimes/win-x64/native/` and won't search there for unpackaged builds), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`), **or** the registry build hadn't completed before the first `OpenAsync` (rare but possible on very slow disks).

**Fix**: For unpackaged builds, publish self-contained: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None` — that flattens the natives into the publish root. Verify by listing the publish folder: the GStreamer DLLs from `VisioForge.CrossPlatform.Core.Windows.x64` must sit next to (or be loadable from) the .exe. Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump. If errors persist, switch to the explicit `await VisioForgeX.InitSDKAsync()` boot pattern (see "Engine boot") so the registry is fully built before any `OpenAsync` call.

### 2. Native DLLs missing on end-user machine (MSIX)

**Cause**: MSIX strictly bounds the package contents — only what's declared as Content or comes via NuGet `runtimes/` is shipped. The X redists register their natives correctly, but a hand-edited `.appxmanifest` or a `<Content Remove>` in the csproj can drop them.

**Fix**: do not exclude `runtimes/**` from MSIX content. After packaging, open the `.msix` (it's a ZIP), look under `\runtimes\win-x64\native\` for the GStreamer / libav DLLs. If they're missing, the redist NuGets aren't being honoured — re-add them at the project level (don't rely on transitive only, see `references/Sample.csproj`).

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration"). Every `MediaPlayerCoreX` instance in the process needs its own call. Under MSIX, load the certificate from `Package.Current.InstalledLocation.Path` — relative paths break.

### 4. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the input file uses a codec or container the referenced redist can't demux/decode. The default `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]` covers the common-case formats (H.264 / H.265 in MP4 / MKV / MOV / TS, AAC / MP3 / Opus / Vorbis, WebM with VP8/VP9). Less common codecs (HAP, DNxHD, ProRes via plugin variants) need additional redists; some codec names on the error string come from libav rather than GStreamer, which is normal.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist, add it with the same version pin as the others. For HLS / DASH playback the default redist is sufficient — if those error out, the issue is usually network / DRM-related rather than codec-related.

### 5. Preview is black; player runs but no video shows

**Cause**: `VideoView(canvasControl)` was called from the constructor before the `CanvasControl` had a render target, **or** `Audio_Play = true` is implicit but the engine had no audio output device assigned and stalled the pipeline waiting for one, **or** `OpenAsync` succeeded against a video-only file but the wrong `VideoView` instance was passed (a stale reference from a previous Window).

**Fix**: the upstream pattern (construct `VideoView` + `MediaPlayerCoreX` in the constructor) works for the common case, but if the canvas swap-chain isn't ready yet, the first frame is dropped. Move construction into the `Activated` handler gated by `_isInitiated` (see "Engine boot — the safer pattern"). Always assign an audio output device explicitly before `PlayAsync`:

```csharp
var sink = (await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)).FirstOrDefault();
if (sink != null) _player.Audio_OutputDevice = new AudioRendererSettings(sink);
```

If the source has no audio track, set `_player.Audio_Play = false` before `OpenAsync` to skip audio negotiation entirely.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (after adding `Assets/` PNGs from any WinUI 3 desktop template).
- [ ] First run shows a brief delay (~2-5 s, the GStreamer registry build) before the first `OpenAsync` returns; subsequent launches are instant.
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCoreX` (always call `await _player.StopAsync(); await _player.DisposeAsync();` on close).
- [ ] On clean shutdown, `MainWindow_Closed` runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] Position slider updates smoothly while playing and lets you seek without freezing the UI (the bundled sample uses a 500 ms `DispatcherTimer` and a `_timerTag` flag to suppress feedback loops between slider events and timer ticks — keep that pattern).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode). Under MSIX, the certificate path is resolved via `Package.Current.InstalledLocation.Path`.
- [ ] If publishing unpackaged: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsPackageType=None` — verify the GStreamer / libav DLLs land next to the .exe.
- [ ] If publishing MSIX: open the `.msix` ZIP and verify `\runtimes\win-x64\native\` contains the X engine DLLs.

## Bundled references

The `references/` folder is a faithful copy of the official sample with the SDK icon stripped and the target framework bumped to `net10.0`. Copy all of it into a fresh project folder; you'll also need the `Assets/` PNGs from any WinUI 3 desktop template (or the upstream sample) for the package to build:

- `references/Sample.csproj` — minimal working WinUI 3 csproj, version-pinned to the same NuGet release as the prose (wrapper `2026.5.4`, redist `2026.4.29`, WindowsAppSDK `1.8.251106002`).
- `references/App.xaml` + `references/App.xaml.cs` — Application entry point.
- `references/MainWindow.xaml` — XAML with `<win2d:CanvasControl x:Name="canvasControl"/>` for the playback surface, file/URL textbox + browse, and Play / Pause / Resume / Stop / position-slider transport bar.
- `references/MainWindow.xaml.cs` — full code-behind with `MediaPlayerCoreX` construction in the ctor, file picker, `UniversalSourceSettings.CreateAsync` open, `DispatcherTimer`-driven position display, audio output device pick, `OnError` wiring, and `MainWindow_Closed → StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` shutdown. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Package.appxmanifest` — MSIX manifest (declares `runFullTrust` capability, required for Win32 P/Invoke into the SDK natives).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-net-winui` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist).
    - `media-player-sdk-x-wpf` — same X SDK on WPF (different `VideoView` namespace, XAML element rather than Win2D-canvas wrapper).
    - `video-capture-sdk-x-winui` — same X engine for capture / recording in a WinUI 3 host (sibling skill, near-identical project setup).
    - **Cross-platform hosts**:
        - `media-player-sdk-x-maui` — same X SDK on .NET MAUI.
        - `media-player-sdk-x-avalonia` — same X SDK on Avalonia.
        - `media-player-sdk-x-uno` — same X SDK on Uno Platform.
