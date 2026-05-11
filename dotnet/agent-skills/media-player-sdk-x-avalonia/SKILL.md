---
name: media-player-sdk-x-avalonia
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into an Avalonia UI application. Covers the Avalonia-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, supported input formats, and the most common cross-platform pitfalls (missing native libs, X11/Wayland on Linux, ALSA/PulseAudio audio, trial-period expiry / unlicensed build). Use when building playback apps that must run on Windows, Linux, and macOS from one codebase — for native iOS/Android/macOS use media-player-sdk-x-{android,ios,macos}, for MAUI use media-player-sdk-x-maui.
---

# Media Player SDK X — Avalonia integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to an Avalonia UI application that targets **Windows, Linux, and macOS** from a single codebase. Media Player SDK X exposes a high-level playback god-object (`MediaPlayerCoreX`) — local files, HTTP(S), HLS, RTSP, RTMP, MMS, image sequences — with seek, pause/resume, rate control, volume, audio-device selection, frame-stepping, tag reading, and `MediaInfoReaderX` stream introspection. Same C# code targets all three desktop OSes — only the UI host (Avalonia here) and the per-OS native redist NuGet package change between platforms.

Pinned NuGet versions: wrapper **`2026.5.4`**, Windows redists **`2026.4.29`**, macOS redist **`2025.9.1`** (matches the [official Simple Media Player Avalonia sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia)). The redist versions track the underlying GStreamer rebuild cadence per OS and lag the wrapper version on purpose — pin every redist to the value shipped in the upstream csproj for the wrapper version you're using; do not blindly bump.

## When to use this skill

- Adding playback (local file, HTTP(S), HLS, RTSP, RTMP, MMS) to an Avalonia app that must run on Windows, Linux, and macOS from one codebase.
- Building a media player UI with seek, pause/resume, volume, playback-rate control (-2.5x to 3.5x), audio-device routing, and per-frame stepping.
- Reading container/codec metadata (`MediaInfoReaderX`) and ID3/iTunes/Vorbis tags (`Tags_Read`) before or during playback.
- Sharing playback code between an Avalonia "main" app and one or more cross-platform companion apps (MAUI mobile, WPF Windows-only, console batch).

## When NOT to use this skill

- **Windows-only WPF host** (single TFM, no per-OS conditionals): use `media-player-sdk-x-wpf`. Same SDK, same `MediaPlayerCoreX` API.
- **Native .NET-for-mobile/Apple hosts**: `media-player-sdk-x-android`, `media-player-sdk-x-ios`, `media-player-sdk-x-macos`.
- **MAUI host** (multi-target one binary per OS, mobile-first): `media-player-sdk-x-maui`.
- **Capture / recording instead of playback** (webcam → file): use `video-capture-sdk-x-avalonia`.
- **Editing / joining clips** (non-linear editor, multi-clip render): use `video-edit-sdk-x-avalonia`.
- **Custom pipeline topology** (multi-output tee, mid-stream sink swap, audio-only pipelines with custom mixer): use `media-blocks-sdk-net-avalonia` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.

## NuGet packages (cross-platform layout)

Media Player SDK X for Avalonia is **not** a single meta-package. You add the SDK plus the **Avalonia UI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed SDK (engine + `MediaPlayerCoreX` lives here, despite the package name) | Always |
| `VisioForge.DotNet.Core.UI.Avalonia` | Avalonia `VideoView` control | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` | FFmpeg/libav runtime on Windows (UPX-compressed; non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable) | `-windows` |
| `VisioForge.CrossPlatform.Core.macOS` | Native runtime on macOS | `-macos` |
| (none — uses system-wide GStreamer) | Native runtime on Linux | Linux |

> **Why `VisioForge.DotNet.MediaBlocks` and not `VisioForge.DotNet.MediaPlayer`?** On the WPF host the wrapper package is `VisioForge.DotNet.MediaPlayer` (which contains both the legacy `MediaPlayerCore` and the cross-platform `MediaPlayerCoreX`). The Avalonia host uses `VisioForge.DotNet.MediaBlocks` instead — it ships the same `MediaPlayerCoreX` type but without the WPF-only legacy core, and pulls in the Avalonia-friendly transitive dependencies. **Don't reference `VisioForge.DotNet.MediaPlayer` from an Avalonia csproj** — it pulls Windows-only WPF dependencies that fail to restore on Linux/macOS hosts.

Linux is the odd one out — there is no NuGet redist. The Linux target uses the **system-installed GStreamer** (`gstreamer1.0`, `gstreamer1.0-plugins-base`, `gstreamer1.0-plugins-good`, `gstreamer1.0-plugins-bad`, `gstreamer1.0-plugins-ugly`, `gstreamer1.0-libav` on Debian/Ubuntu; equivalent packages on Fedora/Arch). If GStreamer is not installed system-wide on the Linux target, the app launches but `MediaPlayerCoreX.OpenAsync()` / `PlayAsync()` fail with element-not-found errors.

## Supported input formats

`MediaPlayerCoreX` accepts the union of formats handled by GStreamer's container-demuxers + libav fallback. Practically:

- **Containers**: MP4 / M4A / M4V / MOV, MKV / WebM, AVI, WMV / ASF, FLV, MPEG-PS / MPEG-TS, OGG, FLAC, WAV, AIFF, MP3 / AAC / Opus / Vorbis / Speex (raw streams).
- **Video codecs**: H.264 / H.265 / AV1 / VP8 / VP9 / MPEG-2 / MPEG-4 / WMV / Theora / ProRes (HW decode where the OS exposes it: NVDEC / D3D11 / VAAPI / VideoToolbox).
- **Audio codecs**: AAC / MP3 / Opus / Vorbis / FLAC / ALAC / AC-3 / E-AC-3 / DTS / WMA / Speex / PCM.
- **Network protocols**: HTTP / HTTPS, HLS (m3u8), DASH (mpd), RTSP, RTMP, MMS, file://.
- **Image sequences**: PNG / JPEG / BMP via the `ImageSequenceSourceSettings` overload (rotated through at a fixed FPS).

Wrap everything you'd normally pass as a path or URL in `await UniversalSourceSettings.CreateAsync(new Uri(...))` — the SDK dispatches to the right source backend. Use `new Uri("file:///path/...")` (or just `new Uri(absolutePath)`) for files, `new Uri("https://...")` for HTTP streams, `new Uri("rtsp://...")` for RTSP. For Windows paths with backslashes, `new Uri(@"C:\path\to\file.mp4")` works directly.

## Project setup

### Multi-target csproj

This is the core of the skill — get the per-OS conditionals right and the rest follows. The csproj declares `<TargetFramework>` (singular) **conditionally per host OS**: Windows (`net10.0-windows` + `WinExe`) on Windows hosts; macOS (`net10.0-macos` + `Exe`) on macOS hosts; plain `net10.0` + `Exe` on Linux. This is different from the MAUI multi-target pattern (which uses `<TargetFrameworks>` plural and emits one binary per OS) — the Avalonia sample emits **one binary per OS**, with the active TFM picked at build time by the host OS detection.

The full minimal csproj is in `references/Sample.csproj`. Adapted from the official Simple Media Player sample (`Media Player SDK X/Avalonia/Simple Media Player/Simple Media Player.csproj`). Highlights:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <TargetFramework>net10.0-windows</TargetFramework>
  <OutputType>WinExe</OutputType>
</PropertyGroup>
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>

<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <TargetFramework>net10.0-macos</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>

<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

The Avalonia base packages (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`, `System.Reactive`) are unconditional — they ship managed-only and resolve their per-OS native bits at runtime. The VisioForge wrapper + Avalonia UI packages (`VisioForge.DotNet.MediaBlocks`, `VisioForge.DotNet.Core.UI.Avalonia`) are also unconditional.

For 32-bit Windows deployment, swap `.x64` for `.x86` on both Windows redists. To support both Windows architectures from a single AnyCPU build, reference both `.x64` and `.x86` and drop `<PlatformTarget>` from the csproj.

### Avalonia entry point

`Program.cs` is the standard Avalonia bootstrap — `[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime(args)`. `App.axaml` declares `<FluentTheme />` under `<Application.Styles>`. `App.axaml.cs` instantiates `MainWindow` in `OnFrameworkInitializationCompleted`. See `references/Program.cs`, `references/App.axaml`, `references/App.axaml.cs` — all three are unmodified copies of the upstream sample and need no SDK-specific changes.

> **Note**: Avalonia uses `.axaml` (not `.xaml`) for both the markup file and its `.axaml.cs` code-behind. If you're porting from a WPF skill, rename every `.xaml` reference accordingly — the build will silently skip files with the wrong extension.

## Engine boot and lifecycle

`MediaPlayerCoreX` is the god-object. **Like the editor SDK and unlike the capture SDK**, the player sample does not call `VisioForgeX.InitSDK()` before constructing — `MediaPlayerCoreX`'s constructor performs the necessary initialisation lazily. You **must** still call `VisioForgeX.DestroySDK()` on shutdown (after `Player.StopAsync()` and `await Player.DisposeAsync()`); skipping it leaks native handles and can prevent clean process exit on Windows.

The upstream sample does its initialisation in the `MainWindow.Activated` handler with an `_initialized` guard (because `Activated` fires every time the window comes back to the foreground, and on Linux the native `VideoView` is only fully parented at first activation, not at constructor time), and tears down in `Closing`:

```csharp
public MainWindow()
{
    InitializeComponent();
    Activated += MainWindow_Activated;
    Closing  += MainWindow_Closing;
    DataContext = this;
}

private async void MainWindow_Activated(object sender, EventArgs e)
{
    if (_initialized) return;
    _initialized = true;

    InitControls();
    InitPlayer();                   // new MediaPlayerCoreX(VideoView1) + event subscriptions
    Title += $" (SDK v{MediaPlayerCoreX.SDK_Version})";
    // ...audio-output device enumeration, default selection, etc.
}

private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    _tmPosition?.Stop(); _tmPosition?.Dispose();
    if (_player != null)
    {
        await _player.StopAsync();
        await DestroyEngineAsync();   // unsubscribe + DisposeAsync + null out
    }
    VideoView1?.Dispose(); VideoView1 = null;
    VisioForgeX.DestroySDK();
}
```

`Activated`-with-guard is the canonical pattern for Avalonia + Media Player SDK X. See `references/MainWindow.axaml.cs` for the full code.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await Player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync` / `PlayAsync`:

```csharp
_player = new MediaPlayerCoreX(VideoView1);
_player.OnError += Player_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2.

The cross-platform wrinkle is **where the bytes come from**. On Windows / macOS / Linux, `File.ReadAllBytes("license.vflicense")` resolves relative to the OS-specific working directory:

- **Windows**: typically the `.exe` directory.
- **macOS**: the `.app/Contents/MacOS/` directory inside the bundle — **not** `Contents/Resources/` where you might expect Mac-style data files. Either ship the licence next to the binary, or use `AppContext.BaseDirectory` and put it under `Contents/Resources/` with a `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` entry in the csproj.
- **Linux**: the directory where `dotnet ./YourApp.dll` was launched, **not** the binary directory. Use `AppContext.BaseDirectory` to make this deterministic.

The portable approach is to embed the licence as an `EmbeddedResource` and load via `Assembly.GetManifestResourceStream(...)`. The bundled `references/MainWindow.axaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `InitPlayer()` right after `_player = new MediaPlayerCoreX(VideoView1)`.

## Hello-World playback

The minimum viable playback snippet — open a file or URL, play, attach error/stop handlers — is six lines of SDK code on top of an Avalonia window with a `VideoView`. The full file is in `references/MainWindow.axaml.cs`; the minimum viable wiring:

```csharp
// MainWindow.axaml.cs (excerpt)
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Avalonia;

private MediaPlayerCoreX _player;

private async void MainWindow_Activated(object sender, EventArgs e)
{
    if (_initialized) return; _initialized = true;

    _player = new MediaPlayerCoreX(VideoView1);
    _player.OnError += (_, err) => Dispatcher.UIThread.Post(() => Console.WriteLine(err.Message));
    _player.OnStop  += (_, _)   => Dispatcher.UIThread.Post(() => VideoView1.Refresh());

    // Pick the default audio output (per-OS API enum: DirectSound on Windows, null elsewhere).
    var outputs = await _player.Audio_OutputDevicesAsync(null);
    if (outputs.Length > 0)
        _player.Audio_OutputDevice = new AudioRendererSettings(outputs[0]);

    // UniversalSourceSettings dispatches: file paths, http(s), HLS, RTSP, RTMP, MMS.
    await _player.OpenAsync(await UniversalSourceSettings.CreateAsync(
        new Uri(@"https://example.com/sample.mp4")));
    await _player.PlayAsync();
}
```

`OpenAsync` builds the demuxer/decoder pipeline and pre-rolls the first frame; `PlayAsync` releases the clock. Both are awaitable. `OnError` and `OnStop` may fire on a non-UI thread — marshal back via `Dispatcher.UIThread.Post(...)` before touching Avalonia controls (touching them off-UI throws on Linux and intermittently corrupts state on macOS).

The bundled `references/MainWindow.axaml.cs` extends this with Avalonia file pickers (`StorageProvider.OpenFilePickerAsync` — Avalonia's cross-platform replacement for `OpenFileDialog`), audio-output device dropdown, volume slider (0–100 mapped to `Audio_OutputDevice_Volume` 0.0–1.0), playback-rate slider (-2.5x to 3.5x via `Rate_SetAsync`), seek slider driven by `Position_Get` / `Position_SetAsync`, frame-step (`NextFrame`), tag reading (`Tags_Read`), `MediaInfoReaderX` stream-info dump, and a debug log.

## Common cross-platform pitfalls

These are the cross-platform pitfalls that bite first.

### 1. `DllNotFoundException` / "no element X" on Windows or macOS

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`. Common slips:

- Windows build but `VisioForge.CrossPlatform.Core.Windows.x64` was added unconditionally and no `Condition="$([MSBuild]::IsOsPlatform('Windows'))"` wraps it — works on Windows, but `dotnet build` on a macOS / Linux host fails NuGet restore because the Windows redist has no macOS / Linux RID.
- Wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29` Windows / `2025.9.1` macOS).

**Fix**: cross-check against `references/Sample.csproj` — every per-OS `ItemGroup` needs the matching `Condition`. Pin every redist to the value shipped in the upstream csproj for your wrapper version.

### 2. Linux: app launches but `OpenAsync()` / `PlayAsync()` errors with "no element X"

**Cause**: GStreamer is not installed system-wide. There is no NuGet redist for Linux; the Linux build relies on the system package manager.

**Fix**: install the GStreamer base + plugin packages for the distro:

- Debian / Ubuntu: `sudo apt install gstreamer1.0-tools gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav`
- Fedora: `sudo dnf install gstreamer1 gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free gstreamer1-libav`
- Arch: `sudo pacman -S gstreamer gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav`

`gst-inspect-1.0 --version` should print 1.18 or newer.

### 3. Linux: video plays but no audio (PulseAudio container, ALSA-only host)

**Cause**: `MediaPlayerCoreX.Audio_OutputDevicesAsync(null)` lists devices visible to GStreamer's audio backend. `pulsesink` is the default on most desktops and falls through to ALSA when PulseAudio isn't running. Inside a sandboxed container (Flatpak, Snap, Docker without `--device /dev/snd` or PulseAudio socket passthrough) audio devices may be invisible to the enumerator — fix the container's audio passthrough rather than the SDK.

**Fix**: verify with the GStreamer probes before blaming the SDK:

```bash
# does the audio sink element exist?
gst-inspect-1.0 pulsesink
gst-inspect-1.0 alsasink
# can the engine see the output device?
gst-device-monitor-1.0 Audio/Sink
# bypass test: play a file straight through GStreamer
gst-play-1.0 /path/to/sample.mp4
```

If `gst-play-1.0` is silent too, the issue is below the SDK.

### 4. Linux: native VideoView is black or window misaligns under Wayland

**Cause**: Avalonia's `VideoView` embeds a native GStreamer video sink. Under a pure Wayland session some sinks (older `xvimagesink`-based paths) need an X11 surface; the SDK negotiates to a Wayland-compatible sink (`gtkwaylandsink` / `glimagesink`) when available, but the negotiation requires `gstreamer1.0-plugins-bad` ≥ 1.20 and a desktop with a working OpenGL context.

**Fix**: ensure `gstreamer1.0-plugins-bad` ≥ 1.20 and `gstreamer1.0-libav` are installed, or force XWayland by launching with `GDK_BACKEND=x11` / `QT_QPA_PLATFORM=xcb` (Avalonia honours the platform env vars its hosting toolkit reads). If the panel renders but the video region is black, it's almost always a missing OpenGL/GStreamer integration package, not an SDK bug.

### 5. Trial-mode message or "SDK TRIAL period (30 days) is over" on `OpenAsync()`

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 6. macOS: video window is black until you click / resize

**Cause**: macOS's `VideoToolbox` / `glimagesink` path occasionally needs a layout pass before the first frame draws — a known interaction between Avalonia's native-control hosting and AppKit layer-backed views.

**Fix**: the bundled sample calls `VideoView1.Refresh()` from `OnStop`'s `Dispatcher.UIThread.InvokeAsync` to force a redraw. For the live-playback case, calling `VideoView1.Refresh()` once after `await PlayAsync()` returns is the canonical workaround until Avalonia's native-host layer matures.

### 7. macOS: `File.ReadAllBytes("license.vflicense")` throws `FileNotFoundException`

**Cause**: the working directory inside an `.app` bundle on macOS is `.app/Contents/MacOS/`, not the directory you launched from. Relative paths to data files outside that directory don't resolve.

**Fix**: use `Path.Combine(AppContext.BaseDirectory, "license.vflicense")` and copy the licence to the output via the csproj:

```xml
<ItemGroup>
  <None Include="license.vflicense">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

The same pattern works on Linux (avoids working-directory-vs-binary-directory ambiguity) and on Windows (no-op — they're already the same).

### 8. UI freezes or throws when `OnStop` / `OnError` / position-timer fires

**Cause**: `OnStop`, `OnError`, and any custom timer that calls `Position_Get` fire on a non-UI thread. Touching `tbTimeline.Value`, `lbTimeline.Text`, `mmLog.Items`, etc. from those handlers throws on Linux (X11 is single-threaded) and intermittently corrupts state on macOS.

**Fix**: wrap UI mutations in `Dispatcher.UIThread.Post(...)` or `Dispatcher.UIThread.InvokeAsync(...)`. The bundled `references/MainWindow.axaml.cs` uses `Dispatcher.UIThread.InvokeAsync` inside `tmPosition_Elapsed` and `Player_OnStop`, and `Dispatcher.UIThread.Post` inside `Player_OnError`.

### 9. RTSP stream opens but freezes or stutters

**Cause**: RTSP defaults to UDP transport; UDP delivery is unreliable across NAT and Wi-Fi. The SDK exposes per-source overrides via `UniversalSourceSettings` but the default is "let GStreamer pick". If the network drops UDP packets, playback freezes.

**Fix**: pass an explicit `RTSPSourceSettings` with `Protocols = RTSPProtocols.TCP` (or `Protocols.TCP | Protocols.UDP` for fallback) to `OpenAsync` instead of `UniversalSourceSettings`. For protected streams set `Login` / `Password` on the same settings object. See `MediaPlayerCoreX.OpenAsync(IVideoCaptureBaseVideoSourceSettings)` overload.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds on each target OS with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] On Linux: `gst-inspect-1.0 --version` reports 1.18 or newer; the GStreamer base + good + bad + ugly + libav plugin sets are installed; `gst-device-monitor-1.0 Audio/Sink` lists at least one output.
- [ ] First run shows the SDK-version suffix in the window title once `Activated` fires.
- [ ] A short MP4 plays start-to-finish on each OS: `OnStop` fires once, `Position_Get` reaches `Duration`, `VideoView1.Refresh()` clears the last frame.
- [ ] Seeking via the slider works while playing and while paused (`Position_SetAsync` is awaited; the seek slider doesn't loop-feedback on its own change event — see `_tbTimelineApplyingValue` guard in the reference).
- [ ] Pause / Resume / Stop / Next-frame buttons all work without leaking — `await DestroyEngineAsync()` is called before re-creating the player on full restart.
- [ ] `MainWindow_Closing` runs `StopAsync → DestroyEngineAsync → VisioForgeX.DestroySDK` in that order; clean exit on each OS (no zombie process, no native-handle leak warnings).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh Avalonia project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target Avalonia csproj with all per-OS conditional package references (Windows + macOS NuGet redists; Linux uses system GStreamer).
- `references/Program.cs` — Avalonia entry point (`[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime`).
- `references/App.axaml` + `references/App.axaml.cs` — Avalonia `Application` with `<FluentTheme />` and `MainWindow` instantiation.
- `references/MainWindow.axaml` — Avalonia XAML with file/URL textbox + browse button, debug-mode checkbox, audio-output device combo, play-audio toggle, volume slider, seek slider with timeline label, playback-speed slider, Start / Resume / Pause / Stop / Next-frame buttons, info / log tabs, and the `VideoView` panel. Declares `xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"` for the `VideoView` namespace.
- `references/MainWindow.axaml.cs` — full code-behind: `Activated`-with-guard `InitPlayer`, `DestroyEngineAsync` + `VisioForgeX.DestroySDK` on close, `StorageProvider`-based file picker, audio-device enumeration, `UniversalSourceSettings.CreateAsync`-driven `OpenAsync` + `PlayAsync`, position-update timer with `Dispatcher.UIThread.InvokeAsync` marshalling, `Rate_SetAsync` / `Position_SetAsync` / `Audio_OutputDevice_Volume` wiring, `NextFrame`, `Tags_Read`, and `MediaInfoReaderX`-based stream introspection. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-x-wpf` — same SDK, Windows-only WPF host with a single TFM (no per-OS conditionals).
    - `media-player-sdk-x-macos` — same SDK, native .NET-for-macOS host without Avalonia.
    - `media-player-sdk-x-android` / `media-player-sdk-x-ios` — same SDK on native mobile hosts.
    - `media-player-sdk-x-maui` — same SDK on .NET MAUI (mobile-first multi-target).
    - `video-capture-sdk-x-avalonia` — capture sibling (`VideoCaptureCoreX`) for the same Avalonia host.
    - `video-edit-sdk-x-avalonia` — non-linear editor sibling (`VideoEditCoreX`) for the same Avalonia host.
    - `media-blocks-sdk-net-avalonia` — same engine, lower-level graph-based API for custom pipeline topologies.
