---
name: video-edit-sdk-net-console
description: Integrate VisioForge Video Edit SDK .NET (non-linear editor) into a .NET console application for batch processing — cut, trim, merge, transcode, apply effects to existing video files headlessly. Covers the timeline model, the single NuGet package, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use for scripts, scheduled jobs, CI pipelines that process video files — for an interactive editor use video-edit-sdk-net-wpf or video-edit-sdk-net-winforms.
---

# Video Edit SDK .NET — Console integration

This skill helps you add **VisioForge Video Edit SDK .NET** to a headless .NET console application — for scripts, scheduled jobs, server-side processing, and CI pipelines that cut, trim, merge, transcode, or apply effects to existing video files. The SDK is a non-linear editor (NLE): it does **not** capture from cameras or screen — for that see `video-capture-sdk-net-wpf`. The SDK is Windows-only (DirectShow / Media Foundation under the hood); for cross-platform batch editing (macOS, Linux containers) see the [Video Edit SDK X product page](https://www.visioforge.com/video-edit-sdk-net).

Pinned NuGet version: **`2026.5.4`** (matches the official Main Demo CLI sample at `_SETUP/GitHub/Video Edit SDK/Console/CSharp/Main Demo CLI/`). Newer 2026.x.x patch versions are drop-in compatible.

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

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for cut / merge / MP4 transcode scenarios.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />
</ItemGroup>
```

Add codec-heavy outputs (WebM, FFMPEG-based custom containers) by adding the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Main Demo CLI sample (`x:/MediaFrameworkDotNet/_SETUP/GitHub/Video Edit SDK/Console/CSharp/Main Demo CLI/`). Changes vs upstream: the demo's hard-coded `<PlatformTarget>x64</PlatformTarget>` is removed (default AnyCPU works — see "Project platform" below); the `CommandLineParser` NuGet dependency, the explicit redist packages, and the demo's branding metadata (`<AssemblyTitle>`, `<Product>`, `<Copyright>`, `<AssemblyVersion>`, `<ApplicationIcon>`, the `visioforge_main_icon.ico` resource) are dropped — the bundled `references/Program.cs` parses two positional arguments by hand and only depends on the Video Edit meta-package. The bundled file builds standalone against the public NuGet package.

### Project platform

Use AnyCPU (the default — no `<Platform>` or `<PlatformTarget>` element required). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below.

## Timeline model

`VideoEditCore` exposes two distinct editing paths:

**1. Fast-edit (stream-copy, no re-encode)** — `FastEdit_CutFileAsync(source, start, stop, output)`. Stream-copies a single segment of an MP4/MOV/M4A without touching the codec. Fast (I/O-bound) and lossless, but limited to one input and supports MP4-family containers only. For a batch script that just needs to chop a chunk off the end of an MP4, this is the path: no `Output_Format` / no `Mode` / no event-driven completion ceremony beyond awaiting the call.

**2. Timeline (decode → re-encode)** — multiple `Input_Add*FileAsync` calls populate an ordered list of input segments (video, audio, image), each with an in/out `TimeSpan` for sub-clipping. The engine concatenates them into a single output stream that re-encodes through the format set in `Output_Format`. This is the path for merging multiple files, transcoding, applying effects, building a slideshow, or cutting from a non-MP4 container.

For the timeline path in a console host, you must:

- Construct `VideoEditCore` with the **no-argument** constructor — there is no UI thread to attach to.
- Set `core.Video_Renderer.VideoRenderer = VideoRendererMode.None` so the engine does not try to spin up a video renderer.
- Set `core.Mode = VideoEditMode.Convert`.
- Await an `OnStop` / `OnError` signal before disposing (see "Common deployment failures" #4).

Key types: `VideoSource(file, start, stop, stretch)`, `AudioSource(file, start, stop, customStream)`, `VideoEditMode.Convert`, `VideoEditStretchMode.{Letterbox,Stretch,Crop}`, `MP4Output` / `MKVv1Output` / `WebMOutput` / `WMVOutput` / etc. Clear the timeline between runs (long-running worker reusing one core) with `Input_Clear_List()` and `Video_Transition_Clear()`.

## License registration

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
        customStream: null, streamIndex: 0, customWidth: 0, customHeight: 0);
    await core.Input_AddAudioFileAsync(
        new AudioSource(input, cutStart, cutStop, string.Empty),
        customStream: null, streamIndex: 0);

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

For a pure FastEdit cut (MP4 / MOV input, no re-encode), the same shape applies but you replace the timeline calls + `StartAsync` with a single `await core.FastEdit_CutFileAsync(input, cutStart, cutStop, output);` — `OnStop` still fires when the muxer finishes, so keep the `done`/`await` pattern even though the call returns quickly.

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

## Common deployment failures (console-specific)

These are the four most common production issues for headless hosts — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: TFM is `net8.0` (no `-windows` suffix), *or* `<PlatformTarget>` is set to `x64` / `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>linux-x64</RuntimeIdentifiers>`, `osx-arm64`, …) that doesn't match the redist NuGet's native folder.

**Fix**: target `netN.0-windows` (e.g. `net10.0-windows`) and keep AnyCPU (default — remove any `<PlatformTarget>x64</PlatformTarget>` carried over from the demo csproj). The SDK is Windows-x86/x64 only; non-Windows RIDs are not supported. For Docker / containers, base the image on `mcr.microsoft.com/windows/servercore` (not Linux) and ensure the Media Foundation feature is installed — Server Core editions strip it by default.

### 2. Process exits while encoding is still in flight (output is 0 bytes / corrupt)

**Cause**: `StartAsync` returns to the caller as soon as the pipeline starts — completion is signalled asynchronously through `OnStop`. In a console host, if `Main` returns immediately after `await core.StartAsync()`, the process tears down before the muxer flushes. `FastEdit_*Async` is similarly fire-and-forget for completion.

**Fix**: hold the process open with a `TaskCompletionSource<bool>` that is set in `OnStop` (success) and `OnError` (failure), and `await done.Task` before calling `core.Stop()` / `core.Dispose()` and returning from `Main`. The bundled `references/Program.cs` is the canonical pattern.

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial-mode info string surfaces via the `OnError` channel in console hosts since there is no `IVideoView`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `VideoEditCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` / `FastEdit_*Async` (see "License registration" above). For a worker that builds one core per job, every instance needs its own call. The SDK never shows a blocking dialog — in a console host the trial / expiry message appears in `OnError` only.

### 4. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: the chosen `Output_Format` depends on a redist not referenced by the project, *or* the input file uses an exotic codec not in the default decoder set (e.g., HEVC in MOV without the LAV redist on older Windows versions).

**Fix (output)**: add the corresponding redist NuGet package and rebuild — for example, `core.Output_Format = new WebMOutput()` without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

**Fix (input)**: add `VisioForge.DotNet.Core.Redist.LAV.x64` for broader decoder coverage, or transcode the input through `ffmpeg` to a known-good format first.

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
- `references/Program.cs` — full hello-world: parse `<input> <output>` from `args`, cut seconds 5..15, transcode to MP4, await `OnStop` before disposing. Use as a copy-paste starting template. (Runs in trial mode by design; uncomment the two-line `SetLicenseCertificateAsync` block for a purchased licence — see "License registration".) The upstream Main Demo CLI's `CommandLineParser` dependency, explicit redist packages, branding metadata (`<ApplicationIcon>`, `visioforge_main_icon.ico`, `<AssemblyTitle>` / `<Copyright>` / version stamps) were deliberately stripped — the SDK's own branding icon shouldn't ship into a user's app via this skill, and the simpler code is easier to extend.

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
