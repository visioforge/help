---
name: media-player-sdk-net-wpf
description: Integrate VisioForge Media Player SDK .NET (file/stream playback) into a Windows WPF application. Covers the single NuGet package, project setup, license registration, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, unsupported format, trial-period expiry / unlicensed build). Use when adding video/audio file or network-stream playback to a WPF app on Windows — for capture from a camera use video-capture-sdk-net-wpf, for editing use video-edit-sdk-net-wpf.
---

# Media Player SDK .NET — WPF integration

This skill helps you add **VisioForge Media Player SDK .NET** to a Windows WPF application. It covers playback of local video/audio files, network streams (HTTP, RTSP, UDP, RTMP), and DVD/Blu-ray, with seek, pause/resume, audio device selection, volume control, and a bindable timeline. The SDK is Windows-only (DirectShow / Media Foundation under the hood) — for cross-platform playback (macOS, iOS, Android, Linux), use one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills instead.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Player Demo sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Simple%20Player%20Demo)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Playing local video/audio files (MP4, MKV, MOV, AVI, WebM, MP3, WAV, FLAC, etc.) in a Windows WPF app.
- Playing HTTP / RTSP / UDP / RTMP network streams.
- Showing the video surface in WPF using `VideoView`.
- Implementing standard transport controls (play/pause/resume/stop, seek, volume, audio output selection).

## When NOT to use this skill

- **Cross-platform**: target macOS, iOS, Android, or Linux → use `media-blocks-sdk-net-{maui,avalonia,uno}` instead. Media Player SDK is Windows-only.
- **Capture from a camera**: webcam, IP camera, screen, DV → use `video-capture-sdk-net-wpf` instead.
- **Editing instead of playback**: cut, merge, transcode, render timeline → use `video-edit-sdk-net-wpf`.
- **WinForms instead of WPF**: same SDK, different UI host → `media-player-sdk-net-winforms`.
- **Custom pipeline / encoders / filters**: build your own GStreamer-style graph → `media-blocks-sdk-net-wpf`.

## Project setup

### Target framework

Media Player SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj must use the **`Microsoft.NET.Sdk.WindowsDesktop`** SDK with `<UseWPF>true</UseWPF>`.

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for a basic file-playback scenario.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
</ItemGroup>
```

If your inputs need codec-heavy decoding (WebM/VP9, certain HEVC paths, exotic containers), add the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Player Demo sample (`_DEMOS/Media Player SDK/WPF/CSharp/Simple Player Demo/`). Changes vs upstream: the per-TFM file matrix (`net472`/`net5`/`net6`/`net7`/`net8`/`net9`/`net10`) is collapsed to a single `<TargetFramework>net10.0-windows</TargetFramework>`; `<PlatformTarget>x64</PlatformTarget>` is removed (AnyCPU is the supported default — see "Project platform" below); the analyzer block (`<EnableNETAnalyzers>`, `<AnalysisLevel>`, `<NoWarn>S2325</NoWarn>`) is dropped; the `visioforge_main_icon.ico` reference and `<Content Include="visioforge_main_icon.ico" />` are removed (the bundled `references/` folder ships **no** icon — you supply your own application icon if needed); `<AssemblyName>` is shortened to `WPF Simple Player`. The bundled file builds standalone against the public NuGet package.

### Project platform

Use `<Platforms>AnyCPU</Platforms>` (the default). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below. The upstream Simple Player Demo csproj forces `x64` for convenience; the bundled `references/Sample.csproj` removes it.

## License registration

The SDK ships with a 30-day trial — the bundled `references/MainWindow.xaml.cs` runs in trial mode by design (the upstream demo never sets a licence anywhere). To register a purchased licence in the bundled reference, add two lines to its existing `CreateEngine()` method right after the `MediaPlayerCore` constructor:

```csharp
// references/MainWindow.xaml.cs — CreateEngine(), at line ~102
private void CreateEngine()
{
    _player = new MediaPlayerCore(VideoView1);

    // Add these two lines (CreateEngine becomes async — update its callers
    // and the method signature to `private async Task CreateEngineAsync()`):
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    await _player.SetLicenseCertificateAsync(cert);

    _player.OnError += Player_OnError;
    _player.OnStop += Player_OnStop;
    // ...
}
```

If you're starting from scratch without the bundled reference, the same pattern applies in your own `MainWindow.xaml.cs` — call `await _player.SetLicenseCertificateAsync(certBytes)` after `new MediaPlayerCore(VideoView1)` and before `PlayAsync`. (See the Hello-World snippet below for a full standalone `MainWindow`.)

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-window apps, every `MediaPlayerCore` instance needs its own `SetLicenseCertificateAsync` call before `PlayAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World playback

Minimum viable load-and-play snippet — a self-contained `MainWindow` you can drop into a fresh WPF project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` below with your project's `<RootNamespace>` from the csproj:

```xml
<!-- MainWindow.xaml -->
<Window x:Class="YourApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:my="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="MainWindow" Height="450" Width="800">
    <Grid>
        <my:VideoView x:Name="VideoView1" Background="Black" />
        <!-- add a Button x:Name="PlayButton" Click="PlayButton_Click" anywhere -->
    </Grid>
</Window>
```

Code-behind:

```csharp
// MainWindow.xaml.cs
using System;
using System.Windows;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.Events;

namespace YourApp
{
    public partial class MainWindow : Window
    {
        private MediaPlayerCore _player;
        // (The bundled `references/MainWindow.xaml.cs` calls this field `_player`;
        //  use the same name there if you start from the bundled template, otherwise
        //  the snippet below won't merge cleanly.)

        public MainWindow() => InitializeComponent();

        private async void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            // async-void event handlers must catch — otherwise the exception
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Realistic escape paths from this snippet: missing
            // native DLLs (DllNotFoundException), COM init failure
            // (COMException), or unexpected init exceptions during construction.
            // Note: trial-expired, unsupported-format, file-not-found, and
            // network errors don't throw — they surface via the OnError event
            // (see references/MainWindow.xaml.cs).
            try
            {
                // VisioForge.Core.UI.WPF.VideoView already implements the
                // expected interface, so passing VideoView1 directly is fine.
                _player = new MediaPlayerCore(VideoView1);
                _player.OnError += (s, args) =>
                    Dispatcher.Invoke(() => MessageBox.Show($"Player error: {args.Message}"));

                _player.Playlist_Clear();
                _player.Playlist_Add(@"C:\samples\video.mp4"); // local file, or http://, rtsp://, udp://, rtmp:// URL

                await _player.PlayAsync();
            }
            catch (Exception ex)
            {
                // Dispose the half-built engine on failure so the next click
                // starts from a clean slate (otherwise the field stays non-null
                // and the next constructor call overwrites it without disposing).
                if (_player != null)
                {
                    try { await _player.DisposeAsync(); } catch { }
                    _player = null;
                }
                MessageBox.Show($"Playback failed: {ex.Message}");
            }
        }
    }
}
```

`references/MainWindow.xaml.cs` (paired with `MainWindow.xaml`) ships the full pattern with audio output device selection, volume slider, seekable timeline, debug-mode logging, and `OnError` / `OnStop` event wiring.

## Supported input formats

Out-of-the-box (no additional redist needed): MP4 / MOV / M4V / M4A (H.264, H.265 via Media Foundation), AVI, WMV / ASF, MP3, WAV, AAC, FLAC, MPEG-TS, MPEG-PS. Network protocols: `http://`, `https://`, `rtsp://`, `udp://`, `rtmp://`, `mms://`.

For **WebM (VP8/VP9 + Vorbis/Opus)** and many MKV variants you need the WebM redist. For exotic containers and codecs (e.g. DV, Theora, HEVC HDR via FFmpeg, raw network sinks), add the FFMPEG or LAV redist — see the table in the next section.

## Optional codec packages

You only need these if your inputs actually use them. Plain MP4/H.264, WMV, AVI, MP3, WAV, FLAC playback does not require any explicit redist — it ships in the main package.

| Input format / source | Add to csproj |
|---|---|
| WebM (VP8/VP9 + Vorbis/Opus) | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| FFmpeg-based decoding (exotic containers, custom protocols) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| LAV-based decoding (uncommon legacy formats, robust MKV) | `VisioForge.DotNet.Core.Redist.LAV.x64` |
| VLC-based source (when LAV/FFmpeg can't open a stream) | `VisioForge.DotNet.Core.Redist.VLC.x64` |
| Xiph (Vorbis/Theora) standalone codecs | `VisioForge.DotNet.Core.Redist.XIPH.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.MediaPlayer`** — version drift between main and redist is undefined behaviour.

For a 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist package you need.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder. The upstream Simple Player Demo csproj forces `<PlatformTarget>x64</PlatformTarget>` — when you copy that into a project that publishes for a different RID, the native DLLs no longer resolve.

**Fix**: keep `<Platforms>AnyCPU</Platforms>` (default) and remove any explicit `<PlatformTarget>`. If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported, the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `MediaPlayerCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the `MediaPlayerCore` constructor and before `PlayAsync` (see "License registration" above). For multi-window apps, every `MediaPlayerCore` instance needs its own `SetLicenseCertificateAsync` call. The SDK never shows a blocking "License required" dialog — it surfaces the trial info string via `IVideoView.ShowMessage`, and once the 30-day window elapses it aborts startup with `"SDK TRIAL period (30 days) is over."`.

### 3. `OnError` fires with "Codec not found" / "Unsupported format" / "No suitable decoder"

**Cause**: input file or stream uses a codec/container not covered by the default redist set. Most common offenders: WebM/VP9, MKV with HEVC, DV in AVI, Theora/Vorbis in OGG.

**Fix**: add the corresponding redist NuGet package (see "Optional codec packages" above) and rebuild. For WebM/VP9 input add `VisioForge.DotNet.Core.Redist.WebM.x64`; for stubborn MKVs and exotic codecs add `VisioForge.DotNet.Core.Redist.LAV.x64` or `VisioForge.DotNet.Core.Redist.FFMPEG.x64`. If the source is a flaky network stream that LAV can't open, fall back to `VisioForge.DotNet.Core.Redist.VLC.x64`.

### 4. Network stream stalls or drops every few seconds

**Cause**: the default network buffer is sized for HTTP progressive download. RTSP/UDP/RTMP feeds with bursty bitrate or high jitter need a larger buffer; pure-LAN UDP needs almost none.

**Fix**: hook `_player.OnError` early (before `PlayAsync`) so transport errors get logged instead of swallowed silently — the bundled `references/MainWindow.xaml.cs` already does this. For RTSP cameras, prefer TCP transport (set in the source URL or `Source_*` properties) to dodge UDP packet loss. For HTTP/HLS playback, ensure `https://` URLs go through a working TLS stack (Windows 7 with no recent updates is a known black-hole). Verify with VLC first — if VLC can't open the URL, neither can the SDK.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Running the app and pointing it at a local MP4 file plays video+audio within ~1 s.
- [ ] Seeking via the timeline slider works without freezing the UI thread.
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCore` (always call `await _player.StopAsync(); await _player.DisposeAsync();` on close).
- [ ] `OnError` is wired before `PlayAsync` so codec/network errors surface instead of being swallowed silently.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCore` instance before `PlayAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window. Declares `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and hosts `<WPF:VideoView x:Name="VideoView1" />`.
- `references/MainWindow.xaml.cs` — full code-behind with playback (Play/Pause/Resume/Stop), audio output device selection, volume control, seekable timeline, debug-mode logging, `OnError` / `OnStop` wiring, and proper async dispose on window close. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-wpf` — capture from webcam / IP camera / screen / DV (companion SDK on the same UI host).
    - `media-player-sdk-net-winforms` — same SDK on WinForms.
    - `video-edit-sdk-net-wpf` — when you need to cut/merge/transcode rather than just play back.
    - `media-blocks-sdk-net-wpf` — alternative when you need a custom playback pipeline (custom decoders, multi-output rendering) rather than the high-level `MediaPlayerCore` API.
