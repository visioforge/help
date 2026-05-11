---
name: video-edit-sdk-x-console
description: Integrate VisioForge Video Edit SDK X (cross-platform editor edition) into a .NET console application for batch processing — cut, trim, merge, transcode, apply effects to existing video files headlessly. Covers the timeline model, the cross-platform NuGet package layout, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use for scripts, scheduled jobs, CI pipelines that process video files with the X-family API — for an interactive editor use video-edit-sdk-x-wpf or video-edit-sdk-x-winforms.
---

# Video Edit SDK X — Console integration

This skill helps you add **VisioForge Video Edit SDK X** — the cross-platform "X" edition of the editor SDK — to a headless .NET console application: scripts, scheduled jobs, server-side workers, and CI pipelines that cut, trim, merge, transcode, or apply effects to existing video files. The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level non-linear-editor god-object (`VideoEditCoreX`) that mirrors the legacy `VideoEditCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux — the only thing that changes between platforms is the per-OS native redist NuGet package. Like every editor SDK, X **does not** capture from cameras or screen — for that see `video-capture-sdk-x-wpf` (or its console-/service-host siblings on the roadmap).

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the [official Video From Images X CLI sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/Console)). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- A batch script that cuts / trims / merges / transcodes a folder of video files with an API that ports cleanly to other platforms (macOS, Linux containers).
- A scheduled job (Task Scheduler, cron, Hangfire) that processes uploaded videos on a server.
- A CI/CD step that re-encodes assets to a target format.
- A long-running worker that picks jobs off a queue and writes results back to disk / blob storage.
- Sharing edit/transcode code between a Windows console worker and a cross-platform companion app (MAUI / Avalonia / Uno) — the `VideoEditCoreX` API is identical across hosts.

## When NOT to use this skill

- **Live capture** from webcam / IP camera / screen → `video-capture-sdk-x-wpf` (or its console-host sibling on the roadmap).
- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist) → `video-edit-sdk-net-console`. The two SDKs ship side-by-side and can coexist in one process.
- **Custom pipeline topology** (per-block control over decoders, filters, sinks; non-linear graphs) → `media-blocks-sdk-net-console`. `VideoEditCoreX` is the high-level NLE wrapper around exactly the same engine.

## Project setup

### Target framework

Video Edit SDK X 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on a Windows console host. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** with `<OutputType>Exe</OutputType>`. The `-windows` TFM suffix is required on Windows builds (the redist resolves native DLLs through Windows-specific RIDs); for macOS / Linux builds, swap the TFM and redist to the matching cross-platform package — see "Cross-platform builds" below.

### NuGet packages

Three packages are required for a Windows console scenario — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoEdit` is the **same wrapper package** the legacy SDK uses — both `VideoEditCore` (legacy) and `VideoEditCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64`) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. Some samples reference `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` — that's a UPX-compressed variant (smaller download, slightly slower first-load). Either works; pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Video From Images X CLI sample (`_SETUP/GitHub/Video Edit SDK X/Console/CSharp/Video From Images X CLI/`). Changes vs upstream: stripped the `CommandLineParser` NuGet dependency (the bundled `references/Program.cs` parses two positional arguments by hand), the `<UseWindowsForms>true</UseWindowsForms>` flag (not needed in a pure console host), and the demo's branding metadata (`<AssemblyTitle>` / `<Product>` / `<Copyright>` / `<AssemblyVersion>` / `<ApplicationIcon>` / the `visioforge_main_icon.ico` resource). Kept upstream's `Condition="$([MSBuild]::IsOsPlatform('...'))"` blocks so a single csproj builds on Windows, Linux, and macOS — see "Cross-platform builds" below for the per-OS TFM and redist mapping. The bundled file builds standalone against the public NuGet packages.

### Cross-platform builds

The X engine itself is cross-platform — to ship a Linux or macOS console worker, swap the redist pair and TFM:

| Platform | TFM | Core redist | Libav redist |
|---|---|---|---|
| Windows x64 | `net10.0-windows` | `VisioForge.CrossPlatform.Core.Windows.x64` | `VisioForge.CrossPlatform.Libav.Windows.x64` |
| macOS (arm64 + x64) | `net10.0` | `VisioForge.CrossPlatform.Core.macOS` | (libav bundled in Core on macOS) |
| Linux x64 | `net10.0` | `VisioForge.CrossPlatform.Core.Linux.x64` | (libav bundled in Core on Linux) |

Linux builds also need `SkiaSharp.NativeAssets.Linux` for diagram/snapshot rendering. The C# code (the `references/Program.cs` body) is identical across platforms — only the csproj `<ItemGroup>` blocks switch via `Condition="$([MSBuild]::IsOsPlatform('...'))"` if you want one csproj that builds everywhere. See the upstream Video From Images X CLI csproj for the multi-OS conditional pattern.

## Mandatory engine boot

Before any `VideoEditCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown — the bundled `references/Program.cs` shows the canonical placement at the top and bottom of `Main`.

```csharp
static async Task<int> Main(string[] args)
{
    await VisioForgeX.InitSDKAsync();
    // ... new VideoEditCoreX(), do work, await OnStop, dispose ...
    VisioForgeX.DestroySDK();
    return 0;
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run.

## Timeline model

`VideoEditCoreX` is a non-linear editor: build an ordered timeline of input segments, set an output format, hit `Start()`. The X engine **does not** expose a separate fast-edit (stream-copy) path equivalent to the legacy `FastEdit_CutFileAsync` — every `VideoEditCoreX` job decodes and re-encodes through the format set in `Output_Format`. For a pure stream-copy cut on MP4, fall back to `video-edit-sdk-net-console` (the legacy SDK still has `FastEdit_*Async`); for everything else (multi-input merge, transcode, effects, slideshows, cuts on non-MP4 containers) the timeline path is what you want.

Key types and methods:

- `VideoFileSource(filename, start, stop)` — adds a video stream segment with an in/out `TimeSpan` for sub-clipping. Pass `TimeSpan.Zero` for both to take the whole file.
- `AudioFileSource(filename, start, stop)` — same for an audio stream. Add an `AudioFileSource` with the same start/stop alongside every `VideoFileSource` if you want sound on the output.
- `core.Input_AddVideoFile(VideoFileSource)` / `core.Input_AddAudioFile(AudioFileSource)` — the timeline append calls. Append in the order you want them concatenated.
- `core.Input_AddImageFile(path, duration, insertTime)` — adds a still image as a video segment of length `duration` at offset `insertTime` (slideshow / titles / outros).
- `core.Output_Format = new MP4Output(filename)` — output format god-object. Filename is passed to the constructor (no separate `Output_Filename` property — that's a legacy-SDK convention). Ships in `VisioForge.Core.Types.X.Output`: `MP4Output`, `WebMOutput`, `AVIOutput`, `MKVOutput`, `WMVOutput`, `MOVOutput`, plus audio-only formats (`MP3Output`, `M4AOutput`, `OGGVorbisOutput`, `FLACOutput`, `SpeexOutput`).
- `core.Output_VideoSize = new Size(1920, 1080)` and `core.Output_VideoFrameRate = new VideoFrameRate(25)` — only set these when you want to override the source's intrinsic size/rate (slideshows, mismatched-input merges).
- `core.ConsoleUsage = true` — tells the engine it is running in a console host (no message pump). Set this **before** `Start()`.
- `core.Start()` (synchronous fire-and-forget) — kicks off the pipeline; completion is signalled later via `OnStop` / `OnError`. Unlike the legacy SDK, the X engine's `Start` is *not* awaitable.
- `core.OnProgress` / `core.OnStop` / `core.OnError` — wire all three before calling `Start`.

Long-running worker reusing one core across jobs: clear the timeline between runs with `core.Input_Clear_List()` before adding the next batch of inputs.

## License registration

The SDK ships with a 30-day trial — the bundled `references/Program.cs` runs in trial mode by design. To register a purchased licence, load the `.vflicense` bytes and call `SetLicenseCertificateAsync` once per `VideoEditCoreX` instance after construction and before the first `Start()` call:

```csharp
// references/Program.cs — Main(), right after `var core = new VideoEditCoreX();`
var cert = File.ReadAllBytes("path/to/your.vflicense");
await core.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For a long-running worker that constructs / disposes a `VideoEditCoreX` per job, every instance needs its own `SetLicenseCertificateAsync` call. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

## Hello-World batch transcode

Cut seconds 5..15 out of an input video and re-encode the result to a new MP4. Full code in `references/Program.cs`; the load-bearing pieces:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

static async Task<int> Main(string[] args)
{
    var input = args[0];
    var output = args[1];

    // Mandatory engine boot — see "Mandatory engine boot" above.
    await VisioForgeX.InitSDKAsync();

    var core = new VideoEditCoreX();

    var cutStart = TimeSpan.FromSeconds(5);
    var cutStop  = TimeSpan.FromSeconds(15);

    core.Input_AddVideoFile(new VideoFileSource(input, cutStart, cutStop));
    core.Input_AddAudioFile(new AudioFileSource(input, cutStart, cutStop));

    core.Output_Format = new MP4Output(output);
    core.ConsoleUsage  = true;

    // Wait for OnStop before exiting — Start() only kicks off the pipeline.
    var done = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
    core.OnProgress += (_, e) => Console.Out.WriteLine($"Progress: {e.Progress}%");
    core.OnStop     += (_, e) => done.TrySetResult(e.Successful);
    core.OnError    += (_, e) => { Console.Error.WriteLine(e.Message); done.TrySetResult(false); };

    core.Start();
    var ok = await done.Task;

    core.Stop();
    core.Dispose();
    VisioForgeX.DestroySDK();
    return ok ? 0 : 1;
}
```

## Common deployment failures (console-specific)

These are the four most common production issues for headless X hosts — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`), **or** the TFM is plain `net8.0` (no `-windows` suffix) on a Windows build.

**Fix**: confirm `InitSDKAsync` runs before any `VideoEditCoreX` constructor. Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU; `Linux.x64` / `macOS` for cross-platform builds). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Process exits while encoding is still in flight (output is 0 bytes / corrupt)

**Cause**: `Start()` returns to the caller as soon as the pipeline starts — completion is signalled asynchronously through `OnStop`. In a console host, if `Main` returns immediately after `core.Start()`, the process tears down before the muxer flushes.

**Fix**: hold the process open with a `TaskCompletionSource<bool>` that is set in `OnStop` (success) and `OnError` (failure), and `await done.Task` before calling `core.Stop()` / `core.Dispose()` / `VisioForgeX.DestroySDK()` and returning from `Main`. The bundled `references/Program.cs` is the canonical pattern. Also set `core.ConsoleUsage = true` before `Start()` so callbacks dispatch directly instead of trying to marshal onto a non-existent UI synchronisation context.

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (the trial-mode info string surfaces via `OnError` in console hosts since there is no UI surface), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoEditCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start()` (see "License registration" above). For a worker that builds one core per job, every instance needs its own call. The SDK never shows a blocking dialog — in a console host the trial / expiry message appears in `OnError` only.

### 4. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the chosen `Output_Format` depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64` covers MP4 (libav h264), AAC, MPEG-TS, MOV, AVI, MKV, and WebM out of the box. Less common codecs (HAP, DNxHD, ProRes via plugin variants) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist (e.g. for VP9 hardware encode), add it with the same version pin as the others.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows a 2-5 s pause on the very first `InitSDKAsync` call (the registry build), then is instant on subsequent launches.
- [ ] `dotnet run -- <small.mp4> <out.mp4>` writes a ~10 s MP4 and exits with code 0 within a few seconds.
- [ ] `OnProgress` ticks 0..100 then `OnStop` fires with `e.Successful == true` *before* the process exits — no truncated output.
- [ ] On error, the exit code is non-zero and the error message reached stderr (so a calling shell / job runner can detect failure).
- [ ] Shutdown order is `core.Stop() → core.Dispose() → VisioForgeX.DestroySDK()`. Reversing the last two leaks native handles.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCoreX` instance before its first `Start()` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy both files into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working console csproj, version-pinned to the same NuGet release as the prose (see "Pinned NuGet versions" in the intro). Multi-OS via `Condition="$([MSBuild]::IsOsPlatform('...'))"` blocks: Windows builds against `net10.0-windows` + the `Windows.x64` Core/Libav redists; Linux against `net10.0` + `Core.Linux.x64` + `SkiaSharp.NativeAssets.Linux`; macOS against `net10.0-macos15.0` + `Core.macOS`. See "Cross-platform builds" above for the table.
- `references/Program.cs` — full hello-world: parse `<input> <output>` from `args`, cut seconds 5..15, transcode to MP4, await `OnStop` before disposing, run `VisioForgeX.DestroySDK()` on the way out. Use as a copy-paste starting template. (Runs in trial mode by design; uncomment the two-line `SetLicenseCertificateAsync` block for a purchased licence — see "License registration".) The upstream Video From Images X CLI's `CommandLineParser` dependency, branding metadata (`<ApplicationIcon>`, `visioforge_main_icon.ico`, `<AssemblyTitle>` / `<Copyright>` / version stamps), and the `<UseWindowsForms>true</UseWindowsForms>` flag were deliberately stripped — the SDK's own branding icon shouldn't ship into a user's app via this skill, and the simpler code is easier to extend.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-net-console` — same scenario on the legacy Windows-only DirectShow/MF stack (smaller deploy footprint, no GStreamer redist; also has the FastEdit stream-copy cut path that X does not expose).
    - `video-capture-sdk-x-wpf` — capture from webcam / IP camera / screen / NDI (when you need to record live, not edit existing files); same X engine.
    - **Cross-platform / interactive hosts**:
        - `video-edit-sdk-x-wpf` — same X SDK with a WPF preview / interactive UI.
        - `video-edit-sdk-x-winforms` — same X SDK on WinForms.
        - `video-edit-sdk-x-avalonia` — same X SDK on Avalonia (cross-platform desktop).
        - `media-blocks-sdk-net-console` — alternative when you need a custom media pipeline rather than the high-level NLE API.
