---
name: media-player-sdk-net-winforms
description: Integrate VisioForge Media Player SDK .NET (file/stream playback) into a Windows Forms application. Covers the single NuGet package, project setup, license registration, supported input formats, and the most common playback pitfalls (DLL not found, missing codecs, unsupported format, trial-period expiry / unlicensed build). Use when adding video/audio file or network-stream playback to a WinForms app on Windows — for capture use video-capture-sdk-net-winforms, for editing use video-edit-sdk-net-winforms.
---

# Media Player SDK .NET — WinForms integration

This skill helps you add **VisioForge Media Player SDK .NET** to a Windows Forms application. It covers playing local video/audio files and network streams (HTTP, RTSP, UDP, HLS, MMS) with frame-accurate seek, variable playback speed, looping, multi-output volume/balance, and frame-step navigation. The SDK is Windows-only (DirectShow / Media Foundation under the hood) — for cross-platform playback (macOS, iOS, Android, Linux), use one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills instead.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Video Player sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Simple%20Video%20Player)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Playing local video/audio files (MP4, AVI, MKV, MOV, WebM, MP3, WAV, FLAC, AAC, …) inside a WinForms app.
- Playing network streams (HTTP/HTTPS progressive, HLS / `.m3u8`, RTSP, RTMP, UDP, MMS) with the LAV / FFMPEG / VLC source backends.
- Building a custom playback UI with timeline scrubbing, pause/resume, frame-step, variable speed, and per-output volume/balance.
- Hosting the video surface in a WinForms surface using the `VideoView` control.

## When NOT to use this skill

- **Cross-platform**: target macOS, iOS, Android, or Linux → use `media-blocks-sdk-net-{maui,avalonia,uno}` instead. Media Player SDK is Windows-only.
- **Capture instead of playback**: webcam, IP camera, screen, or DV capture → [`video-capture-sdk-net-winforms`](../video-capture-sdk-net-winforms/SKILL.md).
- **Editing instead of playback**: cut, merge, transcode existing files → `video-edit-sdk-net-winforms`.
- **WPF instead of WinForms**: same SDK, different UI host → `media-player-sdk-net-wpf`.

## Project setup

### Target framework

Media Player SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** SDK with `<UseWindowsForms>true</UseWindowsForms>` — note this is *not* `Microsoft.NET.Sdk.WindowsDesktop` (that's the WPF SDK).

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for a basic file-playback scenario.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
</ItemGroup>
```

If you need codec-heavy formats (WebM VP8/VP9, certain legacy LAV-only inputs) or VLC-backed network sources, add the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Player sample (`_DEMOS/Media Player SDK/WinForms/CSharp/Simple Video Player/`). Changes vs upstream: the seven per-TFM upstream csprojs (`net472`, `net5..net10`) are collapsed into a single multi-target file with `<TargetFrameworks>net472;net10.0-windows</TargetFrameworks>`; demo-only properties are removed (`<ProductVersion>`, `<PlatformTarget>`, `<AssemblyVersion>` / `<FileVersion>`, the per-Configuration `<DebugType>` blocks, the `<ApplicationIcon>` / `<ApplicationManifest>` references, and the `Properties\Resources.*` and `Properties\Settings.*` Compile/EmbeddedResource entries that the modern SDK auto-includes); the SDK's own `visioforge_main_icon.ico` is deliberately not bundled into the references folder. The bundled file builds standalone against the public NuGet package.

### Project platform

Use `<Platforms>AnyCPU</Platforms>` (the default). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below. (Note: the upstream `net10.csproj` does pin `<PlatformTarget>x64</PlatformTarget>` — that's a legacy demo choice, the bundled `Sample.csproj` removes it on purpose.)

## License registration

The SDK ships with a 30-day trial — the bundled `references/Form1.cs` runs in trial mode by design (the upstream demo never sets a licence anywhere). To register a purchased licence in the bundled reference, add two lines to its existing `CreateEngine()` method right after the constructor call:

```csharp
// references/Form1.cs — CreateEngine(), at line ~28
private void CreateEngine()
{
    MediaPlayer1 = new MediaPlayerCore(VideoView1 as IVideoView);

    // Add these two lines:
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    MediaPlayer1.SetLicenseCertificateAsync(cert).GetAwaiter().GetResult();

    MediaPlayer1.OnError += MediaPlayer1_OnError;
    MediaPlayer1.OnStop += MediaPlayer1_OnStop;
}
```

If you're starting from scratch without the bundled reference, the same pattern applies in your own `Form1.cs` — call `await mediaPlayer.SetLicenseCertificateAsync(certBytes)` after the `new MediaPlayerCore(...)` constructor and before `PlayAsync`. (See the Hello-World snippet below for a full standalone `Form1`.)

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-form apps, every `MediaPlayerCore` instance needs its own `SetLicenseCertificateAsync` call before `PlayAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World playback

Minimum viable file-playback snippet — a self-contained `Form1` you can drop into a fresh WinForms project. (For the full feature set including timeline scrubbing, frame-step, speed control, and per-output volume/balance, copy `references/` into your project and skip this section.) Replace `YourApp` below with your project's `<RootNamespace>` from the csproj. In the designer, drop a `VisioForge.Core.UI.WinForms.VideoView` (toolbox auto-populates after first build), name it `VideoView1`, and add a `Button` named `PlayButton` wired to `PlayButton_Click`.

```csharp
// Form1.cs
using System;
using System.Windows.Forms;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;
using VisioForge.Core.Types.MediaPlayer;

namespace YourApp
{
    public partial class Form1 : Form
    {
        private MediaPlayerCore _mediaPlayer;
        // (The bundled `references/Form1.cs` calls this field `MediaPlayer1`;
        //  use the same name there if you start from the bundled template, otherwise
        //  the snippet below won't merge cleanly.)

        public Form1() => InitializeComponent();

        private async void PlayButton_Click(object sender, EventArgs e)
        {
            // async-void event handlers must catch — an exception otherwise
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, no DirectShow stack, file not found, unsupported
            // format on the chosen Source_Mode.
            try
            {
                if (_mediaPlayer == null)
                {
                    // VisioForge.Core.UI.WinForms.VideoView already implements IVideoView,
                    // so passing VideoView1 directly is fine — no cast needed.
                    // MediaPlayerCore is a regular constructor (no CreateAsync factory),
                    // unlike VideoCaptureCore. That's intentional — playback engine init
                    // is synchronous.
                    _mediaPlayer = new MediaPlayerCore(VideoView1);
                }

                _mediaPlayer.Source_Mode = MediaPlayerSourceMode.LAV;  // default; works for most files + network streams
                _mediaPlayer.Playlist_Clear();
                _mediaPlayer.Playlist_Add(@"C:\samples\video.mp4");

                _mediaPlayer.Audio_PlayAudio = true;
                _mediaPlayer.Loop = false;

                // Auto-pick the best video renderer for the host (EVR on Win10/11).
                _mediaPlayer.Video_Renderer_SetAuto();

                await _mediaPlayer.PlayAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Playback failed: {ex.Message}");
            }
        }
    }
}
```

`references/Form1.cs` (paired with `Form1.Designer.cs`) ships the full pattern with timeline scrubbing, pause/resume, frame-step (next/previous frame), variable playback speed (0.5×–2.5×), per-output volume/balance, source-mode selector (LAV / DirectShow / FFMPEG / VLC), debug logging toggle, and `OnError` / `OnStop` event wiring.

## Supported input formats

The set of decodable formats depends on the **`Source_Mode`** selected:

| `Source_Mode` | Backend | Best for |
|---|---|---|
| `LAV` *(default)* | LAV Filters (FFmpeg-based DirectShow) | Most local files (MP4, MKV, AVI, MOV, WebM, WMV, FLV, MP3, AAC, FLAC, WAV) — recommended starting point. |
| `File_DS` | System DirectShow codecs | Legacy formats handled by codecs already installed on the host (e.g. K-Lite Codec Pack environments). |
| `FFMPEG` | FFmpeg | Network streams (HTTP, HTTPS, HLS, RTSP, RTMP, UDP, MMS), exotic containers, and anything LAV won't decode. Add `VisioForge.DotNet.Core.Redist.FFMPEG.x64`. |
| `File_VLC` | VLC | Fallback for unusual formats VLC handles natively, plus alternative network-stream backend. Add `VisioForge.DotNet.Core.Redist.VLC.x64`. |

Common extensions decoded out-of-the-box with `LAV`: `.mp4`, `.m4v`, `.mov`, `.mkv`, `.webm`, `.avi`, `.wmv`, `.asf`, `.flv`, `.ts`, `.mts`, `.m2ts`, `.mp3`, `.aac`, `.m4a`, `.flac`, `.wav`, `.ogg`, `.opus`. DRM-protected content is not supported.

## Optional codec packages

You only need these if your `Source_Mode` (or the input format) actually uses them. Default LAV-based file playback does not require any explicit redist — it ships in the main package.

| Source / format | Add to csproj |
|---|---|
| `Source_Mode = FFMPEG` (network streams, exotic containers) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| `Source_Mode = File_VLC` | `VisioForge.DotNet.Core.Redist.VLC.x64` |
| WebM (VP8/VP9 + Vorbis/Opus) standalone decode path | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| Xiph (Vorbis/Theora) standalone decoders | `VisioForge.DotNet.Core.Redist.XIPH.x64` |
| Uncommon legacy formats via LAV explicit redist | `VisioForge.DotNet.Core.Redist.LAV.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.MediaPlayer`** — version drift between main and redist is undefined behaviour.

For a 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist package you need.

## Common playback failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder.

**Fix**: keep `<Platforms>AnyCPU</Platforms>` (default). If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported, the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `MediaPlayerCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _mediaPlayer.SetLicenseCertificateAsync(certBytes)` after the `new MediaPlayerCore(...)` constructor and before `PlayAsync` (see "License registration" above). For multi-form apps, every `MediaPlayerCore` instance needs its own `SetLicenseCertificateAsync` call. The SDK never shows a blocking "License required" dialog; it surfaces the trial-mode info string via `IVideoView.ShowMessage` and aborts startup with `"SDK TRIAL period (30 days) is over."` once the 30-day window elapses.

### 3. `OnError` fires with "Codec not found" / "Filter not registered" / "Unsupported format"

**Cause**: input file uses a codec the chosen `Source_Mode` cannot decode — most often a non-LAV format played with `Source_Mode = LAV`, or a network stream played with `Source_Mode = LAV` (LAV is local-file-oriented; switch to `FFMPEG` for HTTP/RTSP/HLS/UDP).

**Fix**: switch `Source_Mode` (`FFMPEG` is the most permissive — covers nearly everything LAV does plus all network protocols) and add the matching redist NuGet (see "Optional codec packages" above). If the file plays in VLC but not the SDK, try `Source_Mode = File_VLC` plus the VLC redist.

### 4. Video surface is black / frozen on first frame

**Cause**: playback started before the WinForms `VideoView1` control has a created HWND. Most often happens when `PlayAsync` runs from the form constructor — at that point the underlying handle hasn't been created yet (`Control.IsHandleCreated == false`).

**Fix**: defer to the `Form.Load` event (or hook a button click) before calling `PlayAsync`. The bundled `references/Form1.cs` uses `Form1_Load` for engine creation and an explicit "Start" button for `PlayAsync` — copy that pattern. Also call `MediaPlayer1.Video_Renderer_SetAuto()` before `PlayAsync` so the EVR renderer is bound to the `VideoView` HWND.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Running the app and pressing Start with a valid `C:\samples\!video.mp4` shows a video frame within ~1 s.
- [ ] Timeline scrubbing seeks correctly during playback (the `tbTimeline_Scroll` → `Position_Set_TimeAsync` path).
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCore` (always call `await _mediaPlayer.StopAsync(); _mediaPlayer.Dispose();` on form close — the bundled `Form1.Designer.cs::Dispose` already does this).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCore` instance before `PlayAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WinForms csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph). Multi-targets `net472;net10.0-windows`.
- `references/Program.cs` — WinForms application entry point (`Application.Run(new Form1())`).
- `references/Form1.cs` — full code-behind with file open, playback, pause/resume, stop, frame-step (next/previous), timeline scrubbing, variable speed, per-output volume/balance, source-mode selector (LAV / DirectShow / FFMPEG / VLC), debug-mode toggle, and `OnError` / `OnStop` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.) The upstream demo's `linkLabel1` "Watch video tutorials" handler (which depended on `VisioForge.Core.UI.HelpLinks`) and the `cbLicensing` info checkbox are deliberately removed — the SDK's branding shouldn't ship into a user's app via this skill.
- `references/Form1.Designer.cs` — auto-generated UI layout for `Form1`. Hosts the `VisioForge.Core.UI.WinForms.VideoView` control (`VideoView1`) plus all the upstream demo controls (minus the dropped link label and licence-info checkbox). Its `Dispose` override correctly tears down the engine.
- `references/Form1.resx` — RESX for `Form1` (paired with `Form1.Designer.cs`). Trimmed: the upstream form's `<data name="$this.Icon">` block (and the matching `<ApplicationIcon>` in `Sample.csproj` plus `this.Icon = ...` line in `Form1.Designer.cs`) was deliberately removed — the SDK's own branding icon shouldn't ship into a user's app via this skill.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-net-wpf` — same SDK on WPF.
    - [`video-capture-sdk-net-winforms`](../video-capture-sdk-net-winforms/SKILL.md) — when you need to capture (webcam, IP camera, screen) instead of play back.
    - `video-edit-sdk-net-winforms` — when you need to cut, merge, or transcode files instead of just play them.
    - `media-blocks-sdk-net-winforms` — alternative when you need a custom media pipeline rather than the high-level playback API.
