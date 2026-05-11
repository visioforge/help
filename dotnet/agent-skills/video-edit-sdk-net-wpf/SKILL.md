---
name: video-edit-sdk-net-wpf
description: Integrate VisioForge Video Edit SDK .NET (non-linear editor) into a Windows WPF application. Covers the timeline model, the single NuGet package, project setup, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when adding cut/trim/merge/transcode/effects to existing video files on a WPF app — for live capture from a camera, use video-capture-sdk-net-wpf instead.
---

# Video Edit SDK .NET — WPF integration

This skill helps you add **VisioForge Video Edit SDK .NET** to a Windows WPF application. The SDK is a non-linear editor (NLE): it cuts, trims, merges, transcodes, and applies effects to **existing** video and audio files. It does **not** capture from cameras or screen — for live capture see `video-capture-sdk-net-wpf`. The SDK is Windows-only (DirectShow / Media Foundation under the hood); for cross-platform editing (macOS, iOS, Android, Linux), use `video-edit-sdk-x-wpf` or one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills.

Pinned NuGet version: **`2026.5.4`** (matches the official Cut Video File and Video Join Demo samples). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Cutting / trimming a segment out of an existing video file on disk.
- Merging (joining) multiple videos into one with optional transitions.
- Transcoding from one container/codec to another (MP4, AVI, MKV, MOV, MPEG-TS, WMV, WebM, FLAC, MP3, …).
- Applying effects (resize, crop, rotate, brightness/contrast, fades) to existing files.
- Building a slideshow from images and audio.

## When NOT to use this skill

- **Live capture** from webcam / IP camera / screen → `video-capture-sdk-net-wpf`.
- **Playback only**: play files / streams without editing → `media-player-sdk-net-wpf`.
- **Cross-platform** editing on macOS / iOS / Android / Linux → `video-edit-sdk-x-wpf` or `media-blocks-sdk-net-{maui,avalonia,uno}`.
- **WinForms instead of WPF** → `video-edit-sdk-net-winforms`.
- **Custom media pipeline** (per-block control over decoders, filters, sinks) → `media-blocks-sdk-net-wpf`.

## Project setup

### Target framework

Video Edit SDK .NET 2026.x supports `net472`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** with `<UseWPF>true</UseWPF>` — the WPF target framework moniker (`-windows` suffix) is what unlocks the WPF assemblies, you don't need `Microsoft.NET.Sdk.WindowsDesktop` explicitly.

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for cut / merge / MP4 transcode scenarios.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />
</ItemGroup>
```

Add codec-heavy outputs (WebM, FFMPEG-based custom containers) by adding the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Cut Video File sample (`x:/MediaFrameworkDotNet/_SETUP/GitHub/Video Edit SDK/WPF/CSharp/Cut Video File/`). Changes vs upstream: the in-repo `<ProjectReference>` is replaced by `<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />`; the demo's hard-coded `<Platform>x64</Platform>` is removed (default AnyCPU works — see "Project platform" below); `<AssemblyName>` is set to `WPF Cut Video File` for clarity. The bundled file builds standalone against the public NuGet package.

### Project platform

Use AnyCPU (the default — no `<Platform>` or `<PlatformTarget>` element required). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below.

## Timeline model

`VideoEditCore` exposes two distinct editing paths:

**1. Fast-edit (stream-copy, no re-encode)** — `FastEdit_CutFileAsync(source, start, stop, output)`. Stream-copies a single segment of an MP4/MOV/M4A without touching the codec. Fast (I/O-bound) and lossless, but limited to one input and supports MP4-family containers only. Used by the bundled `references/MainWindow.xaml.cs`.

**2. Timeline (decode → re-encode)** — multiple `Input_Add*FileAsync` calls populate an ordered list of input segments (video, audio, image), each with an in/out `TimeSpan` for sub-clipping. The engine concatenates them into a single output stream that re-encodes through the format set in `Output_Format`. This is the path for merging multiple files, transcoding, applying effects, or building a slideshow.

Concrete timeline build (multi-file merge to MP4):

```csharp
using System;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

var core = new VideoEditCore(VideoView1 as IVideoView);
core.Mode = VideoEditMode.Convert;
core.Video_FrameRate = new VideoFrameRate(30);

// Input 1: full clip A.
await core.Input_AddVideoFileAsync(
    new VideoSource(@"C:\clips\a.mp4", TimeSpan.Zero, TimeSpan.Zero, VideoEditStretchMode.Letterbox),
    customStream: null, streamIndex: 0, customWidth: 0, customHeight: 0);
await core.Input_AddAudioFileAsync(
    new AudioSource(@"C:\clips\a.mp4", TimeSpan.Zero, TimeSpan.Zero, string.Empty),
    customStream: null, streamIndex: 0);

// Input 2: B trimmed to 5..15s.
await core.Input_AddVideoFileAsync(
    new VideoSource(@"C:\clips\b.mp4", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), VideoEditStretchMode.Letterbox),
    null, 0, 0, 0);
await core.Input_AddAudioFileAsync(
    new AudioSource(@"C:\clips\b.mp4", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(15), string.Empty),
    null, 0);

// Optional: insert a 1s crossfade transition between segments.
// core.Video_Transition_Add(...);  // see Video_Transitions_Names() / the Main Demo

core.Output_Filename = @"C:\out\merged.mp4";
core.Output_Format   = new MP4Output();   // re-encodes to H.264 + AAC

await core.StartAsync();   // OnProgress fires; OnStop fires when finished
```

Key types: `VideoSource(file, start, stop, stretch)`, `AudioSource(file, start, stop, customStream)`, `VideoEditMode.Convert`, `VideoEditStretchMode.{Letterbox,Stretch,Crop}`, `MP4Output` / `MKVv1Output` / `WebMOutput` / `WMVOutput` / etc. Clear the timeline between runs with `Input_Clear_List()` and `Video_Transition_Clear()`.

## License registration

The SDK ships with a 30-day trial — the bundled `references/MainWindow.xaml.cs` runs in trial mode by design. To register a purchased licence, load the `.vflicense` bytes and call `SetLicenseCertificateAsync` once per `VideoEditCore` instance after construction and before the first `StartAsync` / `FastEdit_*Async` call:

```csharp
// references/MainWindow.xaml.cs — Window_Loaded(), right after _core = new VideoEditCore();
private async void Window_Loaded(object sender, RoutedEventArgs e)
{
    _core = new VideoEditCore();

    // Add these two lines:
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    await _core.SetLicenseCertificateAsync(cert);

    _core.OnError    += VideoEdit1_OnError;
    _core.OnStop     += VideoEdit1_OnStop;
    _core.OnProgress += VideoEdit1_OnProgress;
}
```

Note `Window_Loaded` becomes `async void` — keep it `async void` only for event handlers; never make it `async Task`. The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed across the shared licensing core, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-window apps, every `VideoEditCore` instance needs its own `SetLicenseCertificateAsync` call. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World cut

`references/MainWindow.xaml.cs` is the full hello-world: load a source file, set start / stop seconds, click Start, the SDK stream-copies the segment to an MP4 with no re-encoding via `FastEdit_CutFileAsync`. No `VideoView` is needed for this path (no preview is shown — the engine writes the output file directly and fires `OnProgress` + `OnStop`).

For a hello-world that exercises the timeline model and previews the result, copy the merge snippet from "Timeline model" above into a `Window_Loaded`-driven button click in a fresh WPF window with a `<wpf:VideoView x:Name="VideoView1" />` element (declare `xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` on the `<Window>` root) and pass the `VideoView1` to the `VideoEditCore` constructor.

## Optional codec packages

You only need these if your output format actually uses them. Default MP4 (Media Foundation hardware encoder) and the FastEdit cut path do not require any explicit redist — they ship in the main package.

| Output / source | Add to csproj |
|---|---|
| WebM (VP8/VP9 + Vorbis/Opus) | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| FFmpeg-based output (custom muxers, network sinks) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| LAV-based decoding (uncommon legacy formats) | `VisioForge.DotNet.Core.Redist.LAV.x64` |
| VLC-based source | `VisioForge.DotNet.Core.Redist.VLC.x64` |
| Xiph (Vorbis/Theora/FLAC/Speex) standalone codecs | `VisioForge.DotNet.Core.Redist.XIPH.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.VideoEdit`** — version drift between main and redist is undefined behaviour. For a 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist package you need.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder.

**Fix**: keep AnyCPU (default — remove any `<Platform>x64</Platform>` carried over from the demo csproj). If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported; the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial-mode info string surfaces via `IVideoView.ShowMessage` or the `OnError` channel), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `VideoEditCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` / `FastEdit_*Async` (see "License registration" above). For multi-window apps, every `VideoEditCore` instance needs its own call. The SDK never shows a blocking "License required" dialog.

### 3. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: the chosen `Output_Format` depends on a redist not referenced by the project, *or* the input file uses an exotic codec not in the default decoder set (e.g., HEVC in MOV without the LAV redist on older Windows versions).

**Fix (output)**: add the corresponding redist NuGet package and rebuild — for example, `VideoEdit1.Output_Format = new WebMOutput()` without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

**Fix (input)**: add `VisioForge.DotNet.Core.Redist.LAV.x64` for broader decoder coverage, or transcode the input through `ffmpeg` to a known-good format first.

### 4. Output file is 0 bytes / corrupt after `OnStop` fires

**Cause**: the app exited / disposed `VideoEditCore` before `OnStop` had a chance to flush the muxer. `StartAsync` returns to the caller as soon as the pipeline starts — completion is signalled asynchronously through `OnStop`.

**Fix**: do not call `Dispose()` (or close the window without cancelling) until `OnStop` has fired with `e.Successful == true`. The bundled `references/MainWindow.xaml.cs` calls `_core.Stop()` then `Dispose()` from `Window_Closing` — for a long-running encode, hold the close (cancel `e.Cancel = true` on first close attempt) until you receive the `OnStop` callback.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Picking a small MP4, setting start=0 / stop=5, clicking Start produces a ~5 s output MP4 within ~1 s (the FastEdit path is I/O-bound).
- [ ] For a timeline run: `OnProgress` ticks 0..100 then `OnStop` fires with `e.Successful == true` and the output file plays in any standard player.
- [ ] Closing the window between runs does not leak `VideoEditCore` (always call `_core.Stop(); _core.Dispose();` after `OnStop` — see the bundled `Window_Closing`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCore` instance before the first `StartAsync` / `FastEdit_*Async` call (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WPF csproj, version-pinned to the same NuGet release as the prose (see "Pinned NuGet version" in the intro).
- `references/App.xaml` + `references/App.xaml.cs` — WPF Application entry point (`StartupUri="MainWindow.xaml"`).
- `references/MainWindow.xaml` — XAML for the main window: source/output file pickers, start/stop second textboxes, Start button, progress bar, log textbox.
- `references/MainWindow.xaml.cs` — full code-behind for the FastEdit cut path with `OnError` / `OnProgress` / `OnStop` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence — see "License registration".) The upstream sample's `Window` `Icon="visioforge_main_icon.ico"` attribute and the matching `<Resource Include>` in `Sample.csproj` were deliberately stripped — the SDK's own branding icon shouldn't ship into a user's app via this skill.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-net-wpf` — capture from webcam / IP camera / screen / DV (when you need to record live, not edit existing files).
    - `media-blocks-sdk-net-wpf` — alternative when you need a custom media pipeline rather than the high-level NLE API.
    - `video-edit-sdk-net-winforms` — same SDK on WinForms.
    - `video-edit-sdk-x-wpf` — newer "X" line on WPF (cross-process, modernised pipeline).
    - `media-player-sdk-net-wpf` — playback-only sibling.
