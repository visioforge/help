---
name: media-player-sdk-net-winui
description: Integrate VisioForge Media Player SDK .NET (file/stream playback) into a WinUI 3 (Windows App SDK) application. Covers the WinUI-specific VideoView control, the single NuGet package, project setup, license registration, MSIX packaging quirks, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, unsupported format, trial-period expiry / unlicensed build). Use for WinUI 3 playback — for WPF use media-player-sdk-net-wpf, for WinForms use media-player-sdk-net-winforms.
---

# Media Player SDK .NET — WinUI 3 integration

This skill helps you add **VisioForge Media Player SDK .NET** to a Windows App SDK / WinUI 3 desktop application. It covers playback of local files and network streams (HTTP/RTSP/UDP/file-based MMS) with seek, pause/resume, audio output control, and a position timer. WinUI 3 is the right host when you want native Windows 10/11 look-and-feel, MSIX/Store deployment, and the modern Fluent control set; for the traditional WPF stack use `media-player-sdk-net-wpf`, for WinForms use `media-player-sdk-net-winforms`. The SDK is Windows-only (DirectShow / Media Foundation under the hood).

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Media Player WinUI sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinUI/CSharp/Simple%20Media%20Player%20WinUI)). Newer 2026.x.x patch versions are drop-in compatible. Pinned Windows App SDK: **`Microsoft.WindowsAppSDK 1.8.251106002`**.

## When to use this skill

- Playing back local video/audio files (MP4, MKV, AVI, MOV, WMV, MP3, WAV, FLAC, AAC, etc.) in a WinUI 3 desktop app.
- Streaming HTTP, HTTPS, RTSP, UDP, or file-server URLs into a WinUI 3 surface.
- Hosting the video output in a WinUI 3 window using `VisioForge.Core.UI.WinUI.VideoView` over a Win2D `CanvasControl`.
- Adding a position slider, play/pause/resume/stop transport, and async duration/position queries.
- Shipping as MSIX (Store / sideload) or as an unpackaged WinUI 3 app.

## When NOT to use this skill

- **WPF host** → `media-player-sdk-net-wpf` (different `VideoView` namespace and project SDK).
- **WinForms host** → `media-player-sdk-net-winforms`.
- **Capture / recording from a webcam or screen** → `video-capture-sdk-net-winui`.
- **Editing / trimming / converting files instead of playback** → `video-edit-sdk-net-wpf`.
- **Cross-platform playback** (macOS, iOS, Android, Linux) → `media-blocks-sdk-net-{maui,avalonia,uno}`. Media Player SDK is Windows-only.
- **Newer "X" engine on WinUI** → `media-blocks-sdk-net-*` is the cross-process / modernised pipeline; no Media Player X skill exists yet.

## Project setup

### Target framework and platform

WinUI 3 desktop apps use the plain **`Microsoft.NET.Sdk`** SDK (not `WindowsDesktop`) plus `<UseWinUI>true</UseWinUI>`. The bundled `references/Sample.csproj` targets:

```xml
<TargetFramework>net10.0-windows10.0.19041.0</TargetFramework>
<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
<Platforms>x64</Platforms>
<UseWinUI>true</UseWinUI>
<EnablePreviewMsixTooling>true</EnablePreviewMsixTooling>
<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>
```

WinUI 3 apps are commonly pinned to **x64** rather than AnyCPU — both because Windows App SDK ships separate x64/x86/arm64 native binaries and because the redist packages used here are `.x64`-only. If you need x86 or arm64, swap the redist suffixes accordingly and add the matching RID to `<Platforms>` / `<RuntimeIdentifiers>`. `<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>` bundles the WinAppSDK runtime so end users don't have to install it separately. Note that the Media Player WinUI sample uses `<EnablePreviewMsixTooling>` (not `<EnableMsixTooling>`); both work, but match what's in the bundled `Sample.csproj` to keep diffs clean against upstream.

### NuGet packages

WinUI requires **two** VisioForge packages — the SDK itself and the WinUI-specific `VideoView` host — plus the playback redists:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.MediaPlayer.x64" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="2026.5.4" />
</ItemGroup>
```

Unlike a transitive-only setup, the WinUI sample references the redist packages **explicitly** — the WinUI host has historically had less-reliable transitive resolution under MSIX, and pinning the redist set avoids surprise "filter not registered" errors at runtime. Pin every redist to the same version as `VisioForge.DotNet.MediaPlayer`; version drift between main and redist is undefined behaviour. The `LAV` redist is what powers `MediaPlayerSourceMode.LAV` — the recommended source mode for the broadest format coverage (covers virtually every container and codec the LAV Filters project supports: MP4, MKV, AVI, MOV, WMV, FLV, MPEG-TS, MP3, AAC, FLAC, WAV, etc.).

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Simple Media Player WinUI demo (`_SETUP/GitHub/Media Player SDK/WinUI/CSharp/Simple Media Player WinUI/`). Changes vs upstream: the SDK icon asset (`Assets\visioforge_main_icon.ico`), its `<ApplicationIcon>` property, and its `<Content Update>` block are removed; everything else is byte-identical including the MSIX tooling block and the explicit Windows App SDK / SDK build-tools pin.

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
// MainWindow.xaml.cs — inside the constructor (after InitializeComponent)
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.MediaPlayer;
using VisioForge.Core.UI.WinUI;

_videoView = new VideoView(canvasControl);
MediaPlayer1 = new MediaPlayerCore(_videoView);
MediaPlayer1.Audio_PlayAudio = true;
MediaPlayer1.Source_Mode = MediaPlayerSourceMode.LAV;
```

Unlike the Video Capture WinUI host (which gates initialisation on `Window_Activated` because the capture engine wants a settled HWND), the Media Player WinUI sample constructs `VideoView` and `MediaPlayerCore` directly in the `MainWindow` constructor — the `CanvasControl`'s swapchain is not exercised until `PlayAsync()` runs, so the constructor timing is safe. This is a structural difference from WPF (where `<my:VideoView/>` is itself the XAML element). If you copy/paste from a WPF skill, you will get "type not found" XAML errors — there is no `<vfwinui:VideoView/>` element to drop into the XAML tree.

`Source_Mode = MediaPlayerSourceMode.LAV` is the recommended default. The other modes (`DirectShow_Auto`, `DirectShow_Manual`, `MediaFoundation`) exist for legacy filter graphs or hardware-accelerated MF playback, but they require knowing exactly which DirectShow filters are installed on the target machine — fragile in a redist scenario. Stick with LAV unless you have a specific reason to switch.

## License registration

The SDK ships with a 30-day trial — the bundled `references/MainWindow.xaml.cs` runs in trial mode by design. To register a purchased licence, add two lines after the `MediaPlayerCore` constructor:

```csharp
// references/MainWindow.xaml.cs — MainWindow constructor
MediaPlayer1 = new MediaPlayerCore(_videoView);

// Add these two lines (sync-over-async is fine inside the WinUI ctor;
// for cleaner code, call SetLicenseCertificateAsync from an async init method):
var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
MediaPlayer1.SetLicenseCertificateAsync(cert).GetAwaiter().GetResult();

MediaPlayer1.Audio_PlayAudio = true;
MediaPlayer1.Source_Mode = MediaPlayerSourceMode.LAV;
MediaPlayer1.OnError += MediaPlayer1_OnError;
```

Or, in async code (e.g. an `InitAsync` called from `OnLaunched` before showing the window):

```csharp
MediaPlayer1 = new MediaPlayerCore(_videoView);
var cert = await File.ReadAllBytesAsync("path/to/your.vflicense");
await MediaPlayer1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-window apps, every `MediaPlayerCore` instance needs its own `SetLicenseCertificateAsync` call before `PlayAsync`.

**MSIX note**: when the `.vflicense` is bundled as Content in the package, load it via `Package.Current.InstalledLocation.Path` rather than a relative path — the working directory under MSIX is not the package install root.

## Hello-World playback

Self-contained `MainWindow` you can drop into a fresh WinUI 3 desktop project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>`:

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
        <win2d:CanvasControl x:Name="canvasControl" Background="Black" Grid.Row="0"/>
        <StackPanel Grid.Row="1" Orientation="Horizontal" Margin="12" Spacing="8">
            <TextBox x:Name="edFilename" Width="500" PlaceholderText="File path or URL"/>
            <Button x:Name="btPlay" Content="Play" Click="btPlay_Click"/>
            <Button x:Name="btStop" Content="Stop" Click="btStop_Click"/>
        </StackPanel>
    </Grid>
</Window>
```

```csharp
// MainWindow.xaml.cs
using System;
using Microsoft.UI.Xaml;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.MediaPlayer;
using VisioForge.Core.UI.WinUI;

namespace YourApp;

public sealed partial class MainWindow : Window
{
    private MediaPlayerCore MediaPlayer1;
    private VideoView _videoView;

    public MainWindow()
    {
        InitializeComponent();

        _videoView = new VideoView(canvasControl);
        MediaPlayer1 = new MediaPlayerCore(_videoView);
        MediaPlayer1.Audio_PlayAudio = true;
        MediaPlayer1.Source_Mode = MediaPlayerSourceMode.LAV;
        MediaPlayer1.OnError += (_, e) => System.Diagnostics.Debug.WriteLine(e.Message);

        Closed += (_, _) => MediaPlayer1?.Dispose();
    }

    private async void btPlay_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers must catch — otherwise the exception
        // escapes to AppDomain.UnhandledException and silently terminates
        // the app. Realistic escape paths: missing native DLLs
        // (DllNotFoundException), unsupported file/URL (the engine raises
        // OnError but PlayAsync also throws if the source can't be opened),
        // or a malformed URL (UriFormatException).
        try
        {
            MediaPlayer1.Playlist_Clear();
            MediaPlayer1.Playlist_Add(edFilename.Text);
            await MediaPlayer1.PlayAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Play failed: {ex.Message}");
        }
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        try { await MediaPlayer1.StopAsync(); }
        catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.Message); }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `MainWindow.xaml`) ships the full pattern with a file picker (`Windows.Storage.Pickers.FileOpenPicker` + `WinRT.Interop.InitializeWithWindow.Initialize` for HWND wiring), pause/resume, position slider, a 500 ms `DispatcherTimer` for duration/position display, and `OnError` event wiring. The position-slider feedback loop uses a `_timerTag` flag so a programmatic `slPosition.Value = ...` from the timer doesn't trigger a redundant `Position_Set_TimeAsync` from the slider's `ValueChanged` handler.

## Supported input formats

`MediaPlayerSourceMode.LAV` (recommended default) covers:

- **Video containers**: MP4, MKV, MOV, AVI, WMV, ASF, FLV, MPEG-TS, MPEG-PS, WebM, OGM, 3GP, MXF.
- **Audio formats**: MP3, AAC, FLAC, WAV, OGG, WMA, AC3, DTS, ALAC, Opus, Monkey's Audio.
- **Network sources**: HTTP, HTTPS, RTSP (TCP/UDP transport), UDP unicast/multicast, MMSH (`mms://`), local file paths, UNC paths.

For protocols not covered by LAV (HLS `.m3u8`, DASH `.mpd`, SRT, NDI, WebRTC), use Media Blocks SDK on a different host — Media Player SDK's source modes don't include those engines. The `OnError` handler will surface "source unsupported" / "no demuxer found" messages for unsupported inputs.

## Common deployment failures

Six most common production issues in WinUI 3 — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'" (unpackaged)

**Cause**: in unpackaged mode the native DLLs from `VisioForge.DotNet.Core.Redist.*.x64` need to be in the executable's directory. WinUI 3 unpackaged apps published with `dotnet publish -r win-x64` sometimes leave the redist natives in a `runtimes/win-x64/native/` subfolder that the loader doesn't search.

**Fix**: publish self-contained with `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None`. That flattens the natives into the publish root. Verify by listing the publish folder — `VisioForge_*.dll` files must sit next to the .exe.

### 2. Native DLLs missing on end-user machine (MSIX)

**Cause**: MSIX strictly bounds the package contents — only what's declared as Content or comes via NuGet `runtimes/` is shipped. The redist packages register their natives correctly, but a hand-edited `.appxmanifest` or a `<Content Remove>` in the csproj can drop them.

**Fix**: do not exclude `runtimes/**` from MSIX content. After packaging, open the `.msix` (it's a ZIP), look under `\runtimes\win-x64\native\` for the VisioForge DLLs. If they're missing, the redist NuGets aren't being honoured — re-add them at the project level (don't rely on transitive only, see `references/Sample.csproj`).

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `MediaPlayerCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await MediaPlayer1.SetLicenseCertificateAsync(certBytes)` after the `MediaPlayerCore` constructor and before `PlayAsync` (see "License registration"). For multi-window apps, every `MediaPlayerCore` instance needs its own call. Under MSIX, load the certificate from `Package.Current.InstalledLocation.Path` — relative paths break.

### 4. `OnError` fires with "no demuxer found" / "filter not registered" / "Codec not found"

**Cause**: the source format isn't covered by the redists currently referenced. The pinned `Sample.csproj` includes `MediaPlayer` and `LAV`, which together cover the full LAV format matrix — but a project that drops `LAV` (or pins a stale version) will fail on most modern files. Some legacy formats (e.g. RealMedia, certain WMV3 variants, encrypted ASF) still aren't handled by LAV and need the DirectShow source mode plus the right third-party filter installed on the machine.

**Fix**: keep `VisioForge.DotNet.Core.Redist.LAV.x64` referenced and pinned to the same version as the main package. If LAV genuinely doesn't cover the format, switch `Source_Mode` to `DirectShow_Auto` — but then you depend on the end user having compatible filters, which voids the redist guarantees.

### 5. URL plays in VLC but `OnError` fires "source unsupported" in the SDK

**Cause**: HLS (`.m3u8`) and DASH (`.mpd`) live streams are not in the LAV source set. Media Player SDK's source modes don't ship an HLS/DASH demuxer.

**Fix**: for HLS/DASH/SRT/NDI/WebRTC, use Media Blocks SDK on a different host (`media-blocks-sdk-net-wpf` or similar) — its GStreamer-based pipeline handles those protocols natively. Don't try to force `MediaPlayerCore` onto these sources; the engine will surface "source unsupported" and abort.

### 6. Preview is black or `CanvasControl` artefacts; `MediaPlayerCore` constructed too early or `Dispose` not called

**Cause #1**: `VideoView(canvasControl)` was called on a control that hasn't been laid out yet (rare in the constructor pattern shown above; common if you stash the constructor in `OnLaunched` *before* showing the window). **Cause #2**: the player wasn't disposed on window close, so the previous Win2D swapchain is still bound when a new instance starts.

**Fix**: stick with the bundled pattern — construct `VideoView` and `MediaPlayerCore` directly in the `MainWindow` constructor after `InitializeComponent()`, and call `MediaPlayer1.Dispose()` from `Window.Closed`. The bundled `references/MainWindow.xaml.cs` does both. If you must defer initialisation (e.g. picking the file before constructing the engine), gate it on `Window_Activated` with an `_isInitiated` flag like the Video Capture WinUI sample.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (after adding `Assets/` PNGs from any WinUI 3 desktop template).
- [ ] `VisioForge_*.dll` natives from the `MediaPlayer` and `LAV` redists are present next to the .exe (unpackaged) or under `\runtimes\win-x64\native\` inside the `.msix` (packaged). Missing natives produce `DllNotFoundException` on the first `PlayAsync`.
- [ ] WindowsAppSDK runtime is bundled (`<WindowsAppSDKSelfContained>true</WindowsAppSDKSelfContained>`) or installed on the target machine — otherwise the app fails to launch with a "Windows App Runtime" missing dialog before any SDK code runs.
- [ ] First playback of a typical MP4 / MKV file works without an `OnError` "no demuxer found" / "filter not registered" message — confirms the LAV redist resolved correctly.
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCore` (always call `MediaPlayer1.Dispose()` on `Window.Closed`).
- [ ] Position slider updates smoothly while playing and lets you seek without freezing the UI (the bundled sample uses a 500 ms `DispatcherTimer` and a `_timerTag` flag to suppress feedback loops between slider events and timer ticks — keep that pattern).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCore` instance before its `PlayAsync` (otherwise the app runs in 30-day trial mode and aborts after 30 days with `"SDK TRIAL period (30 days) is over."`). Under MSIX, the certificate path is resolved via `Package.Current.InstalledLocation.Path`.
- [ ] If publishing unpackaged: `dotnet publish -c Release -r win-x64 --self-contained true /p:WindowsAppSDKSelfContained=true /p:WindowsPackageType=None` — verify `VisioForge_*.dll` files land in the publish root, not buried under `runtimes/win-x64/native/`.
- [ ] If publishing MSIX: open the `.msix` ZIP and confirm `\runtimes\win-x64\native\` contains the VisioForge + LAV DLLs and that `Package.appxmanifest` declares the `runFullTrust` capability.

## Bundled references

The `references/` folder is a faithful copy of the official sample with the SDK icon stripped. Copy all of it into a fresh project folder; you'll also need the `Assets/` PNGs from any WinUI 3 desktop template (or the upstream sample) for the package to build:

- `references/Sample.csproj` — minimal working WinUI 3 csproj, version-pinned to the same NuGet release as the prose. References `VisioForge.DotNet.MediaPlayer`, `VisioForge.DotNet.Core.UI.WinUI`, and the `MediaPlayer` + `LAV` redists.
- `references/App.xaml` + `references/App.xaml.cs` — Application entry point.
- `references/MainWindow.xaml` — XAML with `<win2d:CanvasControl x:Name="canvasControl"/>` for the playback surface plus filename input, file-picker button, transport buttons, and a position slider.
- `references/MainWindow.xaml.cs` — full code-behind with `MediaPlayerCore` construction, file picker, play/pause/resume/stop, 500 ms `DispatcherTimer` for duration/position display, position slider with `_timerTag` debounce, and `OnError` wiring. Runs in trial mode by design; add `SetLicenseCertificateAsync` yourself when integrating a purchased licence. The `SetIcon()` helper from the upstream sample is removed — there is no `visioforge_main_icon.ico` to set.
- `references/Package.appxmanifest` — MSIX manifest (declares `runFullTrust` capability, required for Win32 P/Invoke into the SDK natives).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinUI>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-net-wpf` — same SDK on WPF (different `VideoView` namespace, different csproj SDK).
    - `media-player-sdk-net-winforms` — same SDK on WinForms.
    - `video-capture-sdk-net-winui` — webcam / screen / IP-camera capture on the same WinUI host.
    - `video-edit-sdk-net-wpf` — non-linear editing / trimming / format conversion (different SDK).
    - `media-blocks-sdk-net-wpf` — newer cross-platform engine; required for HLS, DASH, SRT, NDI, WebRTC sources.
