---
name: video-edit-sdk-net-console
description: Integrate VisioForge Video Edit SDK .NET (non-linear editor) into a .NET console application for batch processing — cut, trim, merge, transcode, apply effects to existing video files headlessly. Covers the timeline model, the SDK and redist NuGet packages, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use for scripts, scheduled jobs, CI pipelines that process video files — for an interactive editor use video-edit-sdk-net-wpf or video-edit-sdk-net-winforms.
---

# Video Edit SDK .NET — Console integration

This skill helps you add **VisioForge Video Edit SDK .NET** to a headless .NET console application — for scripts, scheduled jobs, server-side processing, and CI pipelines that cut, trim, merge, transcode, or apply effects to existing video files. The SDK is a non-linear editor (NLE): it does **not** capture from cameras or screen — for that see `video-capture-sdk-net-wpf`. The SDK is Windows-only (DirectShow / Media Foundation under the hood); for cross-platform batch editing (macOS, Linux containers) see the [Video Edit SDK X product page](https://www.visioforge.com/video-edit-sdk-net).

Pinned NuGet version: **`2026.11.23`** (matches the official Main Demo CLI sample at `_SETUP/GitHub/Video Edit SDK/Console/CSharp/Main Demo CLI/`). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- A batch script that cuts / trims / merges / transcodes a folder of video files.
- A scheduled job (Task Scheduler, cron-on-WSL-with-Windows-target, Hangfire) that processes uploaded videos.
- A CI/CD step that re-encodes assets to a target format.
- A server-side worker that picks jobs from a queue and writes results back to disk / blob storage.

## When NOT to use this skill

- **Interactive editor with preview** → `video-edit-sdk-net-wpf` or `video-edit-sdk-net-winforms`.
- **Live capture** from webcam / IP camera / screen → `video-capture-sdk-net-wpf`.
- **Cross-platform** batch editing on macOS / Linux → `video-edit-sdk-x-console` or `media-blocks-sdk-net-console`. Video Edit SDK .NET is Windows-only.
- **Custom media pipeline** (per-block control over decoders, filters, sinks) → `media-blocks-sdk-net-console`.

## Project setup

### Target framework

Video Edit SDK .NET 2026.x supports `net472`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** with `<OutputType>Exe</OutputType>`. The `-windows` TFM suffix is required (the native DLLs only ship for Windows RIDs) — a plain `net8.0` target will resolve at NuGet restore but fail at runtime with `DllNotFoundException`.

### NuGet packages

`VisioForge.DotNet.VideoEdit` is the managed SDK only: it depends on `VisioForge.DotNet.Core` and `VisioForge.Libs.External`, and no native redist comes in with it. Reference the redist packages explicitly, pinned to the same version. The bundled sample uses the set of the Main Demo CLI sample:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.VideoEdit.x64" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.FFMPEG.x64" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.MP4.x64" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.XIPH.x64" Version="[2026.11.23]"/>
  <PackageReference Include="VisioForge.DotNet.Core.Redist.WebM.x64" Version="[2026.11.23]"/>
</ItemGroup>
```

`FastEdit_*Async` runs `vfffmpeg64.exe` from `VisioForge.DotNet.Core.Redist.FFMPEG.x64`. Without that package, on a machine without the SDK installer, the call fails through `OnError` with `vfffmpeg64.exe not found. Looked in: …`. See "Optional codec packages" below for what each package is for.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Main Demo CLI sample ([`Video Edit SDK/Console/CSharp/Main Demo CLI/`](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/Console/CSharp/Main%20Demo%20CLI) in the public samples repository). Changes vs upstream: the demo's `<PlatformTarget>x64</PlatformTarget>` and its six `.x64` redist packages are kept (see "Project platform" below); the `CommandLineParser` NuGet dependency and the demo's branding metadata (`<AssemblyTitle>`, `<Product>`, `<Copyright>`, `<AssemblyVersion>`, `<ApplicationIcon>`, the `visioforge_main_icon.ico` resource) are dropped — the bundled `references/Program.cs` parses two positional arguments by hand and needs no NuGet package beyond the SDK and its redists. The bundled file builds standalone against the public NuGet package.

### Project platform

Set `<PlatformTarget>x64</PlatformTarget>`, matching the `.x64` redist packages. Their build targets copy the native files into the build output (next to the exe or into its `x64` folder), and the SDK loads the files of the process architecture. For a 32-bit build, swap every `.x64` package for its `.x86` one and set `<PlatformTarget>x86</PlatformTarget>`.

## Timeline model

`VideoEditCore` exposes two distinct editing paths:

**1. Fast-edit (stream-copy, no re-encode)** — `FastEdit_CutFileAsync(source, start, stop, output)`. Stream-copies a single segment of an MP4/MOV/M4A or compatible MPEG-TS-family file (`.ts`, `.m2ts`, or `.mts`) without touching the codec. Fast (I/O-bound) and lossless, but limited to one input. The default seek mode is `FastEditSeekMode.Input`: the cut starts on the keyframe at or before the start position, so it may begin slightly earlier than requested. Pass `FastEditSeekMode.Output` as the fifth argument for output-side seeking - with stream copy it drops video up to the first keyframe after the start, so a cut shorter than one GOP can come out without video. With the default mode a zero `stop` cuts from `start` to the end of the file; with `FastEditSeekMode.Output` it copies the whole file. A non-zero `stop` must be after `start`, neither may be negative, and a `start` at or past the end of the file is refused too: each fails the call with `OnError`. For a batch script that just needs to chop a chunk off the end of an MP4, this is the path: no `Output_Format` / no `Mode` / no event-driven completion ceremony beyond awaiting the call.

**2. Timeline (decode → re-encode)** — multiple `Input_Add*FileAsync` calls populate an ordered list of input segments (video, audio, image), each with an in/out `TimeSpan` for sub-clipping. The engine concatenates them into a single output stream that re-encodes through the format set in `Output_Format`. This is the path for merging multiple files, transcoding, applying effects, building a slideshow, or cutting from a container fast-edit does not cover (see above).

For the timeline path in a console host, you must:

- Construct `VideoEditCore` with the **no-argument** constructor — there is no UI thread to attach to.
- Set `core.Video_Renderer.VideoRenderer = VideoRendererMode.None` so the engine does not try to spin up a video renderer.
- Set `core.Mode = VideoEditMode.Convert`.
- Await an `OnStop` / `OnError` signal before disposing (see "Common deployment failures" #4).

Key types: `VideoSource(filename, startTime, stopTime, stretchMode)`, `AudioSource(filename, startTime, stopTime, fileToSync)`, `VideoEditMode.Convert`, `VideoEditStretchMode.{Letterbox,Stretch,Crop}`, `MP4Output` / `MKVv2Output` / `WebMOutput` / `WMVOutput` / etc. Clear the timeline between runs (long-running worker reusing one core) with `Input_Clear_List()` and `Video_Transition_Clear()`.

## License registration

For commercial license types, scope, updates, support, and trial terms, see the [canonical VisioForge licensing page](https://www.visioforge.com/licensing).

The SDK ships with a 30-day trial — the bundled `references/Program.cs` runs in trial mode by design. To register a purchased licence, load the `.vflicense` bytes and call `SetLicenseCertificateAsync` once per `VideoEditCore` instance after construction and before the first `StartAsync` / `FastEdit_*Async` call:

```csharp
// references/Program.cs — Main(), right after `var core = new VideoEditCore();`
var cert = File.ReadAllBytes("path/to/your.vflicense");
await core.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed across the shared licensing core, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For a long-running worker that constructs / disposes a `VideoEditCore` per job, every instance needs its own `SetLicenseCertificateAsync` call. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World batch transcode

Cut seconds 5..15 out of an input MP4 and re-encode the result to a new MP4. Full code in `references/Program.cs`; the load-bearing pieces are:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;

static async Task<int> Main(string[] args)
{
    var input = args[0];
    var output = args[1];

    var core = new VideoEditCore();
    core.Mode = VideoEditMode.Convert;
    core.Video_Renderer.VideoRenderer = VideoRendererMode.None;   // headless

    var cutStart = TimeSpan.FromSeconds(5);
    var cutStop  = TimeSpan.FromSeconds(15);

    await core.Input_AddVideoFileAsync(
        new VideoSource(input, cutStart, cutStop, VideoEditStretchMode.Letterbox),
        timelineInsertTime: null, targetVideoStream: 0, customWidth: 0, customHeight: 0);
    await core.Input_AddAudioFileAsync(
        new AudioSource(input, cutStart, cutStop, string.Empty),
        timelineInsertTime: null, targetStreamIndex: 0);

    core.Output_Filename = output;
    core.Output_Format   = new MP4Output();

    // Wait for OnStop before exiting — StartAsync only kicks off the pipeline.
    var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    core.OnProgress += (_, e) => Console.Out.WriteLine($"Progress: {e.Progress}%");
    core.OnStop     += (_, e) => done.TrySetResult(e.Successful);
    core.OnError    += (_, e) => { Console.Error.WriteLine(e.Message); done.TrySetResult(false); };

    await core.StartAsync();
    var ok = await done.Task;

    core.Stop();
    core.Dispose();
    return ok ? 0 : 1;
}
```

For a pure FastEdit cut (MP4 / MOV / MPEG-TS input, no re-encode), the same shape applies but you replace the timeline calls + `StartAsync` with a single `await core.FastEdit_CutFileAsync(input, cutStart, cutStop, output);` — `OnStop` still fires when the muxer finishes, so keep the `done`/`await` pattern even though the call returns quickly.

## Optional codec packages

The bundled set (see "NuGet packages" above) already carries every package below. Keep the FFMPEG package in every project: `FastEdit_*Async` runs its `vfffmpeg64.exe`, and the timeline opens every video and audio input except WMV, ASF, WMA, WAV and MP3 through its FFMPEG Source filter when LAV Filters are not registered on the machine (an app deployed with NuGet packages only) or are blacklisted, or the timeline has transitions (they disable LAV). Add the others only if your output format or inputs use them.

| Output / source | Add to csproj |
|---|---|
| WebM (VP8/VP9 + Vorbis/Opus) | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| `FastEdit_*Async` (cut, join); timeline inputs other than WMV, ASF, WMA, WAV and MP3 without registered LAV Filters, with them blacklisted, or with transitions; FFmpeg-based output (custom muxers, network sinks) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| LAV Filters decoding, only where LAV Filters are registered on the machine (the package does not register them) | `VisioForge.DotNet.Core.Redist.LAV.x64` |
| Xiph (Vorbis/Theora/FLAC/Speex) standalone codecs | `VisioForge.DotNet.Core.Redist.XIPH.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.VideoEdit`** — version drift between main and redist is undefined behaviour. For a 32-bit deployment swap `.x64` for `.x86` and set `<PlatformTarget>x86</PlatformTarget>`.

## Common deployment failures (console-specific)

These are the four most common production issues for headless hosts — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'" / "vfffmpeg64.exe not found" / "vfffmpeg.exe not found"

**Cause**: TFM is `net8.0` (no `-windows` suffix), *or* a redist package the code path needs is not referenced (the SDK package brings none), *or* the process architecture does not match the redist packages — a 32-bit process (an `x86` build, AnyCPU with `Prefer32Bit` on .NET Framework, or an x86 runtime identifier or `dotnet` host on .NET) or an ARM64 process with only `.x64` packages, *or* a non-Windows runtime identifier (`linux-x64`, `osx-arm64`, …).

**Fix**: target `netN.0-windows` (e.g. `net10.0-windows`), reference the redist set from "NuGet packages" above and set `<PlatformTarget>x64</PlatformTarget>` (or `.x86` packages with `x86`). `FastEdit_*Async` needs `VisioForge.DotNet.Core.Redist.FFMPEG.x64`; a 32-bit process looks for `vfffmpeg.exe` from `.x86` instead and reports `vfffmpeg.exe not found`. On ARM64 Windows build `x64` explicitly; it runs under emulation. The SDK is Windows x86/x64 only; non-Windows RIDs are not supported. For Docker / containers, base the image on `mcr.microsoft.com/windows/servercore` (not Linux) and ensure the Media Foundation feature is installed — Server Core editions strip it by default.

### 2. Process exits while encoding is still in flight (output is 0 bytes / corrupt)

**Cause**: `StartAsync` returns to the caller as soon as the pipeline starts — completion is signalled asynchronously through `OnStop`. In a console host, if `Main` returns immediately after `await core.StartAsync()`, the process tears down before the muxer flushes. `FastEdit_*Async` is similarly fire-and-forget for completion.

**Fix**: hold the process open with a `TaskCompletionSource<bool>` that is set in `OnStop` (success) and `OnError` (failure), and `await done.Task` before calling `core.Stop()` / `core.Dispose()` and returning from `Main`. The bundled `references/Program.cs` is the canonical pattern.

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial-mode info string surfaces via the `OnError` channel in console hosts since there is no `IVideoView`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over. Buy a license: https://www.visioforge.com/buy?utm_source=sdk-trial"`). Or the certificate was loaded on a *different* `VideoEditCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` / `FastEdit_*Async` (see "License registration" above). For a worker that builds one core per job, every instance needs its own call. The SDK never shows a blocking dialog — in a console host the trial / expiry message appears in `OnError` only.

### 4. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: the chosen `Output_Format` depends on a redist not referenced by the project, *or* the input file uses an exotic codec not in the default decoder set (e.g., HEVC in MOV on older Windows versions).

**Fix (output)**: add the corresponding redist NuGet package and rebuild — for example, `core.Output_Format = new WebMOutput()` without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

**Fix (input)**: add `VisioForge.DotNet.Core.Redist.FFMPEG.x64`. Without registered LAV Filters (or with them blacklisted), or on a timeline with transitions, the timeline opens every input except WMV, ASF, WMA, WAV and MP3 through its FFMPEG Source filter, and writes the warning `add the VisioForge.DotNet.Core.Redist.FFMPEG package` to the SDK log (not to `OnError`) when it is missing. `VisioForge.DotNet.Core.Redist.LAV.x64` helps only where LAV Filters are registered on the machine; the package does not register them. Or transcode the input through `ffmpeg` to a known-good format first.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] `dotnet run -- <small.mp4> <out.mp4>` writes a ~10 s MP4 and exits with code 0 within a few seconds.
- [ ] `OnProgress` ticks 0..100 then `OnStop` fires with `e.Successful == true` *before* the process exits — no truncated output.
- [ ] On error, the exit code is non-zero and the error message reached stderr (so a calling shell / job runner can detect failure).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCore` instance before the first `StartAsync` / `FastEdit_*Async` call (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy both files into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working console csproj, version-pinned to the same NuGet release as the prose (see "Pinned NuGet version" in the intro).
- `references/Program.cs` — full hello-world: parse `<input> <output>` from `args`, cut seconds 5..15, transcode to MP4, await `OnStop` before disposing. Use as a copy-paste starting template. (Runs in trial mode by design; uncomment the two-line `SetLicenseCertificateAsync` block for a purchased licence — see "License registration".) The upstream Main Demo CLI's `CommandLineParser` dependency and branding metadata (`<ApplicationIcon>`, `visioforge_main_icon.ico`, `<AssemblyTitle>` / `<Copyright>` / version stamps) were deliberately stripped — the SDK's own branding icon shouldn't ship into a user's app via this skill, and the simpler code is easier to extend.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK/Console>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-net-wpf` — same SDK with a WPF preview / interactive UI.
    - `video-capture-sdk-net-wpf` — capture from webcam / IP camera / screen / DV (when you need to record live, not edit existing files).
    - `video-edit-sdk-net-winforms` — same SDK on WinForms.
    - `video-edit-sdk-x-console` — newer "X" line on console (cross-process, modernised pipeline; also Windows-only today, cross-platform on the roadmap).
    - `media-blocks-sdk-net-console` — alternative when you need a custom media pipeline rather than the high-level NLE API.
