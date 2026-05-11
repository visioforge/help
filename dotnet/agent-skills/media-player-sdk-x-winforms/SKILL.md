---
name: media-player-sdk-x-winforms
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a Windows Forms application. Covers the cross-platform NuGet package layout, project setup, license registration, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want playback on WinForms with an API that ports cleanly to MAUI, Avalonia, Uno — for the legacy DirectShow stack use media-player-sdk-net-winforms.
---

# Media Player SDK X — WinForms integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the playback SDK — to a Windows Forms application. The X SDK shares its runtime with Media Blocks and Video Capture X (GStreamer-backed under the hood) and exposes a high-level player god-object (`MediaPlayerCoreX`) that mirrors the legacy `MediaPlayerCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux / iOS / Android — the only thing that changes between platforms is the UI host (WinForms here, MAUI / Avalonia / Uno / native elsewhere) and the per-OS native redist NuGet package.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the upstream "Main Demo" sample at `_SETUP/GitHub/Media Player SDK X/WinForms/Main Demo/`). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Adding video and audio playback to a Windows WinForms app **with an API that ports unchanged to other platforms** (MAUI, Avalonia, Uno, Android, iOS, macOS).
- Playing local files (MP4, MOV, AVI, MKV, WebM, MPEG-TS, FLV, MP3, AAC, FLAC, OGG, WAV, ...) and network sources (HTTP(S), HLS, DASH, RTSP) through the unified `UniversalSourceSettings` entry point.
- RTSP cameras with explicit transport / latency / buffer tuning via `RTSPSourceSettings.CreateAsync(...)`.
- Live audio/video effects on the playback chain (resize, deinterlace, color balance, gaussian blur, LUT, image overlay, text overlay, equalizer, amplify, balance, echo) — `MediaPlayerCoreX.Audio_Effects_AddOrUpdate` / `Video_Effects_AddOrUpdateAsync` work identically on every host.
- Reading subtitles, multi-audio-stream switching, snapshot grabbing, and frame-step navigation.
- Sharing playback code between a Windows WinForms "main" app and one or more cross-platform companion apps.

## When NOT to use this skill

- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): use `media-player-sdk-net-winforms`. The two SDKs ship side-by-side and can coexist in one app.
- **Custom pipeline topology** (split-with-tee for simultaneous playback + recording, multi-source mix, runtime sink swap, RTSP-to-file transcode without preview): use `media-blocks-sdk-net-winforms` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.
- **Capture / recording** (webcam, IP camera, screen, NDI) instead of playback: use [`video-capture-sdk-x-winforms`](../video-capture-sdk-x-winforms/SKILL.md).
- **WPF instead of WinForms**: same SDK, different UI shell → [`media-player-sdk-x-wpf`](../media-player-sdk-x-wpf/SKILL.md).
- **Cross-platform host instead of WinForms**: same SDK, different UI shell → `media-player-sdk-x-{maui,avalonia,uno}`. The `MediaPlayerCoreX` API is identical across platforms.

## Project setup

### Target framework

Media Player SDK X 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on the WinForms host. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** SDK with `<UseWindowsForms>true</UseWindowsForms>` — same convention as Video Capture X and Media Blocks, intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly.

### NuGet packages

Three packages are required for a Windows WinForms playback scenario — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/demuxers/decoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.MediaPlayer` is the **same wrapper package** the legacy SDK uses — both `MediaPlayerCore` (legacy) and `MediaPlayerCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + a libav redist) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. The bundled `references/Sample.csproj` uses `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` — a UPX-compressed variant (smaller download, slightly slower first-load); the non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` (which the upstream Main Demo uses) is interchangeable. Either works; pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the upstream Main Demo (`_DEMOS/Media Player SDK X/WinForms/Main Demo/`). Changes vs upstream: the `<ApplicationIcon>visioforge_main_icon.ico</ApplicationIcon>` property and the matching content item are dropped (the SDK's branding icon shouldn't ship into a user's app via this skill); all the demo-only metadata (`<AssemblyTitle>`, `<Product>`, `<Copyright>`, `<AssemblyVersion>`, `<FileVersion>`, the `Properties\Resources.*` and `Properties\Settings.*` Compile/EmbeddedResource entries, the `Serilog` package reference, and the multi-target `<TargetFrameworks>` list) is stripped down to a single-target `net10.0-windows` build. The bundled file builds standalone against the public NuGet packages.

### Project platform

Media Player SDK X WinForms samples use `<PlatformTarget>x64</PlatformTarget>` — this differs from the legacy SDK (which uses AnyCPU). The reason is the redist packages are split per-architecture; referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous.

## Mandatory engine boot

Before any `MediaPlayerCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// Form1.cs — Form1_Load
private async void Form1_Load(object sender, EventArgs e)
{
    Text += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
    Enabled = false;
    await VisioForgeX.InitSDKAsync();
    Enabled = true;
    Text = Text.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

    // ...now safe to construct MediaPlayerCoreX, query Audio_OutputDevicesAsync, etc.
    _player = new MediaPlayerCoreX(VideoView1);
    _player.OnError += Player_OnError;
    _player.OnStop += Player_OnStop;
}

// Form1.cs — FormClosing handler
private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    if (_player != null)
    {
        await _player.StopAsync();
        await _player.DisposeAsync();
        _player = null;
    }
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/Form1.cs` shows the canonical placement.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync`:

```csharp
_player = new MediaPlayerCoreX(VideoView1);
_player.OnError += Player_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-form apps, every `MediaPlayerCoreX` instance needs its own `SetLicenseCertificateAsync` call before `OpenAsync`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/Form1.cs` runs in 30-day trial mode by design (the `SetLicenseCertificateAsync` call is commented out). To register a purchased licence in the bundled reference, uncomment the two lines in `Form1_Load` right after `_player = new MediaPlayerCoreX(...)`.

## Hello-World playback

The bundled `references/Form1.cs` is itself the minimal copy-paste host — drop the `references/` folder into a fresh project and `dotnet build`. The load-bearing source-open + play call is just three lines (the `try`/`catch` wrapper around them is mandatory because `async void` event handlers swallow exceptions to `AppDomain.UnhandledException` otherwise):

```csharp
// Inside an async event handler, after Form1_Load has run InitSDKAsync
// and constructed _player = new MediaPlayerCoreX(VideoView1):
var uri = new Uri(@"C:\path\to\video.mp4");
var source = await UniversalSourceSettings.CreateAsync(uri);  // local files, http(s), HLS, DASH
await _player.OpenAsync(source);
await _player.PlayAsync();
```

For RTSP-specific tuning (latency, transport selection, RTP block size) use `RTSPSourceSettings.CreateAsync(uri, user, password, audioStream: true)` instead of `UniversalSourceSettings`. In the WinForms designer, drop a `VisioForge.Core.UI.WinForms.VideoView` from the toolbox onto the form (after first build the toolbox auto-populates) and name it `VideoView1` — the bundled `references/Form1.Designer.cs` shows the full layout with timeline, volume, audio output picker, and log pane wired up.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 3. `OnError` fires with "Codec not found" / "Element 'X' not found" / "no decoder for ..."

**Cause**: the input format depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64(.UPX)` covers H.264, H.265 (HEVC), AAC, MP3, AC-3, FLAC, Vorbis, Opus, and the standard MP4/MOV/MKV/WebM/MPEG-TS/AVI demuxers out of the box. Less common codecs (HAP, DNxHD, ProRes via plugin variants) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist, add it with the same version pin as the others.

### 4. Preview is black / no audio on first frame

**Cause**: `OpenAsync` was called before the WinForms `VideoView` control had a created HWND, **or** `MediaPlayerCoreX` was constructed before `await VisioForgeX.InitSDKAsync()` returned, **or** `Audio_Play` is `false` and the user expected sound. Most often happens when `OpenAsync` runs from the form constructor — at that point the underlying handle hasn't been created yet (`Control.IsHandleCreated == false`).

**Fix**: defer `InitSDKAsync` and `MediaPlayerCoreX` construction to `Form1_Load` (or later — a button click is also fine), and gate `OpenAsync` / `PlayAsync` behind an explicit user action. Verify `_player.Audio_Play = true` and that `_player.Audio_OutputDevice` is set to a valid device from `Audio_OutputDevicesAsync(...)`. The bundled `references/Form1.cs` uses `Form1_Load` for `InitSDKAsync` + engine creation and an explicit "Play" button for `OpenAsync` / `PlayAsync` — copy that pattern.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows the "[FIRST TIME LOAD, BUILDING THE REGISTRY...]" title for ~2-5 s (the registry build), then video plays within ~1 s on subsequent launches.
- [ ] Stopping and reopening playback from the UI does not leak `MediaPlayerCoreX` (always call `await _player.StopAsync(); await _player.DisposeAsync();` on form close).
- [ ] On clean shutdown, the form's `FormClosing` handler runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] Timeline trackbar updates from `tmPosition_Tick` reflect `Position_GetAsync` and seeking via `tbTimeline_Scroll → Position_Set` works mid-playback.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WinForms csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph). Targets `net10.0-windows`.
- `references/Program.cs` — WinForms application entry point (`Application.Run(new Form1())`).
- `references/Form1.cs` — minimal playback host distilled from the upstream Main Demo: `InitSDKAsync` boot, `UniversalSourceSettings`-based open, play / pause / resume / stop, timeline trackbar with `Position_GetAsync` / `Position_Set`, audio output device selection via `Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)`, volume control, and `OnError` / `OnStop` wiring. Use as a copy-paste starting template — extend with audio/video effects, RTSP tuning, snapshot grabbing, or subtitle handling per the upstream Main Demo. (Runs in trial mode by design; uncomment the `SetLicenseCertificateAsync` lines in `Form1_Load` when integrating a purchased licence.)
- `references/Form1.Designer.cs` — auto-generated UI layout for `Form1`. Hosts a `VisioForge.Core.UI.WinForms.VideoView` (`VideoView1`) plus the file/URL textbox, transport buttons, timeline + volume trackbars, audio-output picker, "Play audio" checkbox, and a read-only log pane. No icon resource is referenced.
- `references/Form1.resx` — RESX for `Form1` (paired with `Form1.Designer.cs`). Tray-only metadata for `openFileDialog1` and `tmPosition`; no embedded icon.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - [`media-player-sdk-x-wpf`](../media-player-sdk-x-wpf/SKILL.md) — same X SDK on WPF.
    - [`video-capture-sdk-x-winforms`](../video-capture-sdk-x-winforms/SKILL.md) — capture / recording on WinForms with the same X engine.
    - `media-player-sdk-net-winforms` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist).
    - `media-blocks-sdk-net-winforms` — same engine, lower-level graph-based API for custom pipeline topologies.
    - `media-player-sdk-x-maui` — same X SDK on .NET MAUI.
    - `media-player-sdk-x-avalonia` — same X SDK on Avalonia.
    - `media-player-sdk-x-uno` — same X SDK on Uno Platform.
