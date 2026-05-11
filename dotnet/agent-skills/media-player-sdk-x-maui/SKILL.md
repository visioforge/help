---
name: media-player-sdk-x-maui
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a .NET MAUI cross-platform app (Windows, Android, iOS, macOS). Covers the MAUI-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, supported input formats, and the most common cross-platform pitfalls (network permissions per OS, missing native libs, AOT JIT-only ExecutionEngineException, trial-period expiry / unlicensed build). Use when building playback apps that must run on multiple OSes from one MAUI codebase.
---

# Media Player SDK X — .NET MAUI integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to a .NET MAUI application that targets **Windows, Android, iOS, and Mac Catalyst** from a single codebase. The X SDK shares its native runtime with Video Capture X and Media Blocks (GStreamer-backed under the hood) and exposes a high-level playback god-object (`MediaPlayerCoreX`) that mirrors the legacy `MediaPlayerCore` API but runs on the cross-platform engine. Same C# code targets every OS — only the platform handler glue and per-OS redist NuGets change between TFMs.

Pinned NuGet versions: wrapper **`2026.5.4`**, MAUI handlers **`2026.5.4`**, plus per-OS native redists at the versions shown in the csproj below — these match the official [Media Player X MAUI samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI). Newer 2026.x.x patch versions are drop-in compatible; keep `VisioForge.DotNet.MediaPlayer` and `VisioForge.DotNet.Core.UI.MAUI` pinned to the same wrapper version. The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin to the value shipped in the upstream csproj for your wrapper version; do not blindly bump.

## When to use this skill

- One MAUI codebase that runs on **Windows, Android, iOS, and Mac Catalyst** with playback of local files or network streams.
- Universal source playback — `UniversalSourceSettings.CreateAsync(Uri)` autodetects local files, HTTP/HTTPS, RTSP, RTMP, HLS / DASH manifests, file:// URIs, etc.
- Mobile-friendly playback surface using `VisioForge.Core.UI.MAUI.VideoView`.
- Sharing playback code between a Windows desktop "main" app and one or more cross-platform companion apps — the `MediaPlayerCoreX` API is identical across hosts (WPF / MAUI / Avalonia / Uno).

## When NOT to use this skill

- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode-while-playing, runtime sink swap, audio-only fanout): use `media-blocks-sdk-net-maui` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.
- **Capture / recording** (webcam → MP4, screen capture, IP camera record): use `video-capture-sdk-x-maui`.
- **Windows-only desktop app** (WPF / WinForms): `media-player-sdk-x-wpf` is simpler — single TFM, no per-OS redists, no permission paperwork.
- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): `media-player-sdk-net-wpf`.
- **Different cross-platform host**: `media-player-sdk-x-avalonia` / `media-player-sdk-x-uno` — same SDK, different UI shell.

## NuGet packages (cross-platform layout)

Media Player X for MAUI is **not** a single meta-package. You add the SDK plus the **MAUI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaPlayer` | Managed wrapper (`MediaPlayerCoreX`, types) | Always |
| `VisioForge.DotNet.Core.UI.MAUI` | MAUI `VideoView` control + handlers | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav demuxers + decoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` ships through a small bundled csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

`VisioForge.DotNet.MediaPlayer` is the **same wrapper package** the legacy SDK uses — both `MediaPlayerCore` (legacy, Windows-only) and `MediaPlayerCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist set plus the MAUI handler registration below.

The Android target also needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` — a small companion project that ships in the samples repo under `Media Player SDK X/MAUI/AndroidDependency/`. That's how the `.aar` is bound. Copy that folder into your repo alongside your MAUI app and reference it as shown in the csproj below; there is no NuGet equivalent today.

## Project setup

### MAUI workload

```bash
dotnet workload install maui
```

Without this, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: maui-android maui-ios maui-maccatalyst maui-windows`.

### Multi-target csproj

This is the core of the skill — get this right and the rest follows. The csproj declares target frameworks **conditionally per host OS**: Android always; iOS + Mac Catalyst only on macOS hosts; Windows only on Windows hosts. That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights:

```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">$(TargetFrameworks);net10.0-maccatalyst;net10.0-ios</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>

<UseMaui>true</UseMaui>
<SingleProject>true</SingleProject>

<!-- iOS / Mac Catalyst MUST run with the Mono interpreter, otherwise MAUI XAML
     hits a JIT-only path inside Styles.xaml load and throws ExecutionEngineException. -->
<UseInterpreter Condition="...== 'ios'">true</UseInterpreter>
<UseInterpreter Condition="...== 'maccatalyst'">true</UseInterpreter>
```

The conditional `<ItemGroup>` blocks pull in the right per-OS native packages:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.5.4" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>
```

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure on first `MediaPlayerCoreX` construction.

### MauiProgram registration

In `MauiProgram.cs` register the VisioForge MAUI handlers and SkiaSharp (used internally by the renderer):

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Forgetting `AddVisioForgeHandlers()` is the most common silent failure — `<my:VideoView />` renders as a blank `Grid`-shaped void, no errors logged.

## Engine boot model — different from WPF

Unlike the WPF host (which requires an explicit `await VisioForgeX.InitSDKAsync()` before constructing `MediaPlayerCoreX`), the MAUI handler chain initializes the native runtime when `AddVisioForgeHandlers()` is registered — so you do **not** call `InitSDKAsync` yourself in MAUI. Construction order is just:

1. MAUI app starts; `MauiProgram.CreateMauiApp()` runs (`AddVisioForgeHandlers` triggers native init).
2. `MainPage_Loaded` constructs `_player = new MediaPlayerCoreX(videoView.GetVideoView())` and wires events.
3. User picks a file (or URL is set programmatically), then `await _player.OpenAsync(await UniversalSourceSettings.CreateAsync(uri)); await _player.PlayAsync();`.

On shutdown, mirror with `VisioForgeX.DestroySDK()` (in `Window.Destroying` or `Page.Unloaded`):

```csharp
private async void Window_Destroying(object? sender, EventArgs e)
{
    if (_player != null)
    {
        _player.OnError -= _player_OnError;
        await _player.StopAsync();
        await _player.DisposeAsync();
        _player = null;
    }
    VisioForgeX.DestroySDK();
}
```

`MediaPlayerCoreX.DisposeAsync` is the right call (not synchronous `Dispose`) — it gives the engine a chance to flush the audio renderer cleanly before the native runtime tears down.

## Supported input formats

`UniversalSourceSettings.CreateAsync(Uri)` autodetects the input type from URI scheme + content. The X engine covers:

- **Container/local files**: MP4 / MOV / M4V / 3GP, MKV / WebM, AVI, MPEG-TS / M2TS, FLV, OGG / OGV, WAV, MP3, AAC, FLAC, Opus.
- **Codecs**: H.264 / AVC, H.265 / HEVC, VP8, VP9, AV1 (software decode), MPEG-2, MPEG-4, ProRes, AAC, AC-3, MP3, Opus, Vorbis, FLAC, PCM.
- **Network streams**: HTTP / HTTPS progressive, HLS (`.m3u8`), DASH (`.mpd`), RTSP, RTMP, MMS, file:// URIs, custom GStreamer source URIs.
- **Subtitles**: embedded (MKV / MP4) and external SRT / VTT (set via the player's subtitles API — see `MediaPlayerCoreX.Subtitles_*`).

Format coverage on each OS depends on which native plugins ship in the redist for that OS — H.264/H.265/AAC are universal; some legacy / niche codecs are Windows + libav only. If a particular file fails to open, `OnError` will surface the underlying GStreamer error message ("no decoder for type ...") — that's the signal you've hit a missing-codec gap, not a wrapper bug.

## Per-platform permissions

Pure playback has a much smaller permission surface than capture — no camera/microphone keys are needed. The two things that bite cross-platform are **network access on Android** and the **App Transport Security** relaxation that iOS / Mac Catalyst need to play HTTP (non-HTTPS) streams. See `references/platform-permissions.md` for the full set; the essentials:

### Android

`Platforms/Android/AndroidManifest.xml` must declare `INTERNET` for streaming and (on API 32 and below, if you let the user pick from shared storage) the legacy storage permissions:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
<!-- API 33+ for picking videos from gallery: -->
<uses-permission android:name="android.permission.READ_MEDIA_VIDEO" />
```

For **plain HTTP streams** (not HTTPS / RTSP / RTMP) on API 28+, opt in to cleartext traffic on the `<application>` element:

```xml
<application android:usesCleartextTraffic="true" ...>
```

Otherwise Android blocks the request silently and `OnError` reports a generic connect failure.

### iOS

For pure playback, no usage-description keys are required. The thing that surprises people is **App Transport Security**: iOS blocks `http://` (non-HTTPS) streams at the network stack by default. To allow plain HTTP playback for development or known internal hosts, relax ATS in `Platforms/iOS/Info.plist`:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key><true/>
</dict>
```

For App Store submissions, scope the exemption to specific domains via `NSExceptionDomains` (see `references/platform-permissions.md`).

If you also pick local files from the Photos library, add `NSPhotoLibraryUsageDescription`.

### Mac Catalyst

`Platforms/MacCatalyst/Info.plist` needs the same ATS rules as iOS (above) for HTTP. **Plus** `Platforms/MacCatalyst/Entitlements.plist` must enable network-client and (for `FilePicker`) user-selected file read entitlements — sandboxed Mac Catalyst apps don't get them by default:

```xml
<!-- Entitlements.plist -->
<key>com.apple.security.network.client</key><true/>
<key>com.apple.security.files.user-selected.read-only</key><true/>
```

Without `network.client`, network streams fail with a generic connect error and no useful exception.

### Windows

No project-level permission paperwork. Firewall prompts may appear on first run for low-level protocols (RTSP / RTMP) — that's a runtime user dialog, not something declared in the project.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before the first `OpenAsync` / `PlayAsync`. The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes("path/to/your.vflicense")` works on Windows, but on iOS / Android / Mac Catalyst the working directory is the app bundle, not your dev machine. The portable approach is to ship the licence as a `MauiAsset` and load via `FileSystem.OpenAppPackageFileAsync`:

```csharp
// In csproj:
// <ItemGroup>
//   <MauiAsset Include="Resources\Raw\license.vflicense" />
// </ItemGroup>

using var stream = await FileSystem.OpenAppPackageFileAsync("license.vflicense");
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
await _player.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-page apps, every `MediaPlayerCoreX` instance needs its own `SetLicenseCertificateAsync` call before its first `OpenAsync`. Where the bytes come from (asset, env var, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/MainPage.xaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the snippet above into `MainPage_Loaded` right after `_player = new MediaPlayerCoreX(vv)`.

## Hello-World playback

A file-→-preview pipeline with `MediaPlayerCoreX` is three lines of wiring:

```xml
<!-- MainPage.xaml -->
<ContentPage xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI" ...>
    <Grid>
        <my:VideoView x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand" />
    </Grid>
</ContentPage>
```

```csharp
// MainPage.xaml.cs
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

public partial class MainPage : ContentPage
{
    private MediaPlayerCoreX _player;

    public MainPage() { InitializeComponent(); Loaded += async (_, __) => await StartAsync(); }

    private async Task StartAsync()
    {
        _player = new MediaPlayerCoreX(videoView.GetVideoView());

        var result = await FilePicker.Default.PickAsync();
        if (result == null) return;

        await _player.OpenAsync(await UniversalSourceSettings.CreateAsync(new Uri(result.FullPath)));
        await _player.PlayAsync();
    }
}
```

The bundled `references/MainPage.xaml.cs` extends this with audio output device selection, position/duration UI (seek slider, time labels, 500 ms timer), play/pause/stop, volume, playback speed (0.5× / 1× / 2×), `OnError` / `OnStart` / `OnStop` wiring, and clean shutdown via `Window.Destroying`. Use it as a copy-paste template; trim the branches you don't need.

## Common cross-platform pitfalls

These are the five most common production issues — flag any of them on first run.

### 1. Black `VideoView`, no errors logged

**Cause**: `AddVisioForgeHandlers()` was not called in `MauiProgram.CreateMauiApp`. The view registers fine but renders nothing because no platform handler is bound — and the native runtime is not initialised.

**Fix**: add `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers())` to the builder chain (see "MauiProgram registration" above).

### 2. `DllNotFoundException` / `dlopen` failure on first playback (per-OS)

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`, **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`). Common slips:

- Windows: forgot `VisioForge.CrossPlatform.Core.Windows.x64` (build succeeds but `MediaPlayerCoreX` construction fails).
- Android: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough; the companion csproj binds the `.aar`.
- Mac Catalyst: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional ItemGroup matters. Pin redist versions to the values shipped in the upstream csproj for your wrapper version; do not bump.

### 3. iOS / Mac Catalyst app crashes during MAUI XAML load

**Cause**: `<UseInterpreter>` is not enabled for those TFMs. MAUI XAML's `TypeConversionExtensions` uses `DynamicMethod` to invoke implicit operators, and `DynamicMethod` requires JIT. On iOS / Mac Catalyst the runtime is AOT-only by default, so the call throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode` from inside `App.InitializeComponent()`.

**Fix**: set `<UseInterpreter>true</UseInterpreter>` on `-ios` and `-maccatalyst` (see csproj). Tiny size/startup cost, fixes XAML load.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded on the player instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: ship the `.vflicense` as a `MauiAsset` and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 5. HTTP stream silently fails to open on Android / iOS / Mac Catalyst

**Cause**: cleartext-HTTP is blocked by default on Android API 28+ (`android:usesCleartextTraffic="false"` is the default), and on iOS / Mac Catalyst by App Transport Security. The engine surfaces this as a generic connect error in `OnError`, not as a permission-denied error, so it looks like the URL is wrong.

**Fix**: HTTPS works out of the box on every OS — switch the source URL where possible. For HTTP-only sources, set `android:usesCleartextTraffic="true"` on the `<application>` element (or scope it via `network-security-config.xml`), and add an `NSAppTransportSecurity` exception in iOS / Mac Catalyst `Info.plist`. Mac Catalyst additionally needs `com.apple.security.network.client` in `Entitlements.plist` regardless of HTTP vs HTTPS — without it, **all** outbound network is blocked. See `references/platform-permissions.md`.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet workload install maui` is installed; `dotnet build -f net10.0-android` succeeds against the bundled `references/`.
- [ ] On macOS host: `dotnet build -f net10.0-ios` and `dotnet build -f net10.0-maccatalyst` succeed (TFMs only show up on macOS).
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041.0` succeeds.
- [ ] `MauiProgram.CreateMauiApp()` calls `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers()).UseSkiaSharp()` (otherwise `VideoView` is blank everywhere).
- [ ] App launches on each target OS and plays a local sample MP4 within ~1 s of `PlayAsync`.
- [ ] On Android / iOS / Mac Catalyst: an HTTPS test stream (e.g. an HLS `.m3u8`) plays; if you also need HTTP, the cleartext / ATS exemptions are in place.
- [ ] Stopping and reopening playback from the UI does not leak `MediaPlayerCoreX` (always call `await _player.StopAsync(); await _player.DisposeAsync();` on close, then `VisioForgeX.DestroySDK()`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its first `OpenAsync` (otherwise the app runs in 30-day trial mode and aborts after day 30).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh MAUI project folder, set up `Platforms/` and `Resources/` per the standard MAUI template, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target MAUI csproj with all per-OS conditional package references and the Mac Catalyst native-copy target.
- `references/MauiProgram.cs` — entry point with `AddVisioForgeHandlers()` + `UseSkiaSharp()`.
- `references/App.xaml` — MAUI Application entry point.
- `references/MainPage.xaml` — XAML with `<my:VideoView />`, seek slider, play/pause/stop/speed buttons.
- `references/MainPage.xaml.cs` — full code-behind: audio output device selection, file-picker open, position/duration timer, play/pause/stop/seek, volume, 0.5× / 1× / 2× rate, `OnError` / `OnStart` / `OnStop` wiring, clean shutdown via `Window.Destroying`. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/platform-permissions.md` — concise notes on Android `AndroidManifest.xml` (INTERNET + cleartext), iOS `Info.plist` (ATS), Mac Catalyst `Info.plist` + `Entitlements.plist` (network-client + user-selected files).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-maui` — same engine, capture/recording on MAUI (webcam, IP camera, screen → MP4).
    - `media-blocks-sdk-net-maui` — same engine, lower-level graph-based API for custom playback / transcode topologies on MAUI.
    - **Other cross-platform hosts**:
        - `media-player-sdk-x-wpf` — same X SDK on WPF (Windows-only host, no per-OS redists, no permission paperwork).
        - `media-player-sdk-x-avalonia` — same X SDK on Avalonia.
        - `media-player-sdk-x-uno` — same X SDK on Uno Platform.
