---
name: media-blocks-sdk-net-console
description: Integrate VisioForge Media Blocks SDK into a .NET console application for batch processing — build custom pipelines (transcode, mux, stream, record) without UI. Covers the graph-based pipeline model, the single NuGet package, license registration, headless block wiring, and the most common deployment pitfalls (DLL not found, missing codecs, no preview block, trial-period expiry / unlicensed build). Use for scripts, scheduled jobs, CI pipelines that need full pipeline control — for an interactive UI use media-blocks-sdk-net-{wpf,winforms}.
---

# Media Blocks SDK .NET — Console integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a .NET console application — no window, no UI thread, no preview surface. Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`UniversalSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, `RTSPSinkBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `pipeline.Start()` (or `await pipeline.StartAsync()`). Compared to the higher-level Video Capture SDK (a single `VideoCaptureCore` god-object), Media Blocks gives you full control over the topology — multi-source mix, transcode without preview, network streaming sinks, dynamic source switch — at the cost of having to wire every edge yourself.

In a console host the trade-off is simpler than on WPF/WinForms: there is no `VideoView` and therefore no `VideoRendererBlock`. Every running pipeline must terminate on a non-renderer sink (file sink, network sink, app-sink with custom delivery). The host process drives the pipeline directly and must keep the entry thread blocked until the pipeline reports stopped, or the muxer trailer never gets written.

Pinned NuGet version: **`2026.5.4`** (matches the [official FileConvert sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/FileConvert)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- **Headless transcoding** — convert a source file to MP4 / MPEG-TS / WebM with explicit encoder choice and bitrate control (the bundled sample is exactly this).
- **Batch pipelines** — scheduled jobs, CI runners, watch-folder workers that consume an input file or stream and emit a re-encoded output.
- **Network streaming sinks from a console host** — RTSP server, SRT, RIST, NDI, WebRTC WHIP, RTMP push, HLS publisher running unattended.
- **CLI media tooling** — small command-line utilities that wrap a custom pipeline (probe + transcode, mux, segment, remux) without a UI shell.

## When NOT to use this skill

- **Interactive UI** (live preview surface, buttons, file dialogs): use [`media-blocks-sdk-net-wpf`](../media-blocks-sdk-net-wpf/SKILL.md) — the WPF host has the same block API plus the `VideoView` / `VideoRendererBlock` pair for preview. `media-blocks-sdk-net-winforms` for WinForms.
- **Plain webcam capture and record** (no custom topology): [`video-capture-sdk-net-console`](../video-capture-sdk-net-console/SKILL.md) is dramatically less code — one `VideoCaptureCore` object, no graph wiring.
- **Cross-platform** (macOS, iOS, Android, Linux): same SDK family, different host → `media-blocks-sdk-net-{maui,avalonia,uno,android,ios,macos}`. The block API is identical across platforms; only the redist NuGet family differs (`VisioForge.CrossPlatform.Core.macOS`, `.Linux`, etc. instead of `.Windows.x64`).

## Project setup

### Target framework

Media Blocks SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The project SDK is the standard **`Microsoft.NET.Sdk`** with `<OutputType>Exe</OutputType>` — no `<UseWPF>`, no `<UseWindowsForms>`. On .NET 5+ the TFM must still carry the `-windows` suffix because the underlying redist NuGets resolve native DLLs from `runtimes/win-*/native/` and the Windows-only API surface lives behind the OS guard.

### NuGet packages

Three packages are required for a Windows console pipeline — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). Unlike Video Capture SDK, the redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

The redist version (`2026.4.29` here) tracks the underlying GStreamer/libav rebuild cadence and lags the wrapper version (`2026.5.4`) on purpose — pin both to the values shipped in the upstream sample's csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper. Mismatches between wrapper and redist in either direction are undefined behaviour and surface as `DllNotFoundException` or `Element 'X' not found` errors at pipeline start.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official FileConvert sample (`_DEMOS/Media Blocks SDK/Console/FileConvert/`). Changes vs upstream: the upstream csproj uses cross-platform `<TargetFramework Condition>` blocks for macOS / Linux / Windows (with their respective redist packages) — the bundled file targets Windows only with `<TargetFramework>net10.0-windows</TargetFramework>` to match this skill's scope, and pins `<PlatformTarget>x64</PlatformTarget>` for the same reason as the WPF sibling skill (see "Project platform" below). The bundled file builds standalone against the public NuGet packages.

### Project platform

The bundled `Sample.csproj` uses `<PlatformTarget>x64</PlatformTarget>` because the redist packages are split per-architecture (`VisioForge.CrossPlatform.Core.Windows.x64` vs `.x86`); referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous. To support both architectures from a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop the `<PlatformTarget>` line. The SDK is Windows-only on this host platform; for macOS / iOS / Android / Linux you switch to a different redist package family and a different host (Avalonia/MAUI/Uno).

## Pipeline model

This is the core mental shift from Video Capture SDK. There is no `Core.Start()` / `Core.Stop()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need (with the console-specific note on renderers):

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`UniversalSourceBlock` for files / network URIs, `SystemVideoSourceBlock` for webcams, `RTSPSourceBlock`, `ScreenCaptureSourceBlock`, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `VPXEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, `AudioMixerBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** — terminate the graph. **In a console host you do not use renderer sinks** (`VideoRendererBlock`, `AudioRendererBlock`) because there is no `VideoView` to bind to and no audio-output device association you want from a batch job — the constructors require host-UI handles you don't have. Use **file / network sinks** instead: `MP4SinkBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, `RTMPSinkBlock`, `HLSSinkBlock`, …. Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `Start`; reconnecting at runtime is supported by specific blocks (live-source-switch, bridge) but not the general case.

Topology example (FileConvert sample — read a media file, transcode video to H.264 and audio to AAC, mux to MP4):

```text
UniversalSourceBlock  --VideoOutput--> H264EncoderBlock --> MP4SinkBlock(video pad created via CreateNewInput)
                      --AudioOutput--> AACEncoderBlock  --> MP4SinkBlock(audio pad created via CreateNewInput)
```

`MP4SinkBlock` is the same instance on both audio and video paths — its dynamic inputs collect the streams to mux.

The bundled `Program.cs` calls `pipeline.Start()` / `pipeline.Stop()` and `VisioForgeX.DestroySDK()` on shutdown. Note that the upstream sample skips the explicit `await VisioForgeX.InitSDKAsync()` engine-boot call that the WPF skill flags — the SDK lazily initialises on first block construction in this version, but for consistency across hosts (and to surface registry-build errors deterministically) calling `await VisioForgeX.InitSDKAsync()` once before the first `new MediaBlocksPipeline()` is recommended. The first call on a fresh machine builds a plugin-registry cache (~2-5 s); subsequent launches are instant.

## License registration

The SDK ships with a 30-day trial — the bundled `references/Program.cs` runs in trial mode by design (the upstream sample never sets a licence). To register a purchased licence, load the `.vflicense` file as bytes and call `SetLicenseCertificateAsync` on every `MediaBlocksPipeline` instance, after the constructor and before `Start`:

```csharp
// references/Program.cs — right after `new MediaBlocksPipeline()`
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += Pipeline_OnError;

// Add these two lines:
var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await pipeline.SetLicenseCertificateAsync(cert);
```

`Main` is already `async Task Main` in the bundled sample (the upstream `UniversalSourceSettings.CreateAsync` call requires it), so the `await` drops in directly.

The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For long-running batch hosts that recycle the engine (one pipeline per input file), every new `MediaBlocksPipeline` instance needs its own call before its `Start`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World pipeline

Minimum viable headless transcode snippet — drop into a fresh console project as `Program.cs`. Reads `args[0]` as a source media file and writes `<sourcename>_new.mp4` next to it via H.264 + AAC + MP4 mux, no preview, no UI thread. (For the full FileConvert reference pattern — same code, fully commented — copy `references/` into your project.) Replace `YourApp` with your project's `<RootNamespace>` from the csproj:

```csharp
// Program.cs
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace YourApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length == 0 || !File.Exists(args[0]))
            {
                Console.WriteLine("Usage: app <source-file>");
                return;
            }
            var sourceFile = args[0];

            // Build the pipeline. Subscribe to OnError before Start, otherwise
            // trial-expiry / "Element X not found" errors become silent no-ops.
            var pipeline = new MediaBlocksPipeline();
            pipeline.OnError += (s, e) => Console.WriteLine("Error: " + e.Message);

            // For a purchased licence, add these two lines here:
            //   var cert = System.IO.File.ReadAllBytes("your.vflicense");
            //   await pipeline.SetLicenseCertificateAsync(cert);

            // Source: UniversalSourceBlock auto-detects container/codecs and exposes
            // VideoOutput + AudioOutput pads. CreateAsync probes the file once.
            var sourceSettings = await UniversalSourceSettings.CreateAsync(sourceFile);
            var source = new UniversalSourceBlock(sourceSettings);

            // Encoders with default settings — H.264 + AAC is the universally
            // playable choice for MP4. Override settings for explicit bitrate.
            var h264 = new H264EncoderBlock();
            var aac = new AACEncoderBlock();

            // MP4 muxer sink. Dynamic inputs: one per stream to mux.
            var outputFile = Path.Combine(
                Path.GetDirectoryName(sourceFile)!,
                Path.GetFileNameWithoutExtension(sourceFile) + "_new.mp4");
            var mp4 = new MP4SinkBlock(new MP4SinkSettings(outputFile));

            // Wire the graph. Connections must be in place before Start.
            pipeline.Connect(source.VideoOutput, h264.Input);
            pipeline.Connect(h264.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Video));
            pipeline.Connect(source.AudioOutput, aac.Input);
            pipeline.Connect(aac.Output, mp4.CreateNewInput(MediaBlockPadMediaType.Audio));

            Console.WriteLine($"Transcoding -> {outputFile}");
            pipeline.Start();

            // Console hosts have no message pump — block the entry thread
            // until the pipeline finishes processing the input file. The engine
            // runs on its own internal threads. State transitions Play -> Free
            // when the source EOS reaches the sink.
            Thread.Sleep(2000);
            while (pipeline.State == VisioForge.Core.Types.PlaybackState.Play)
                Thread.Sleep(500);

            // Stop -> Dispose flushes the muxer trailer. Skipping Stop produces
            // a truncated MP4 that VLC may open but seek weirdly through.
            pipeline.Stop();
            pipeline.Dispose();
            VisioForgeX.DestroySDK();
            Console.WriteLine("Done.");
        }
    }
}
```

`references/Program.cs` is the full upstream FileConvert sample with `OnStop` event wiring and the same transcode topology. Use as a copy-paste starting template.

## Common deployment failures

These are the most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*' " or "libgstreamer-*.dll not found"

**Cause**: missing redist NuGet (`VisioForge.CrossPlatform.Core.Windows.x64`, `VisioForge.CrossPlatform.Libav.Windows.x64.UPX`), or `<PlatformTarget>` doesn't match the redist's architecture (e.g. `<PlatformTarget>x86</PlatformTarget>` with the `.x64` redist), or the wrapper and redist versions drifted apart enough that the native ABI changed, or the TFM is bare `net10.0` instead of `net10.0-windows` so the `runtimes/win-*/native/` folder is never picked up.

**Fix**: reference both redist packages from the "NuGet packages" section, set `<PlatformTarget>x64</PlatformTarget>` to match, use a `-windows`-suffixed TFM, and pin the redist version to the value used by the upstream sample for your wrapper version (do not blindly bump). For 32-bit deployment swap `.x64` for `.x86` in both redist names and set `<PlatformTarget>x86</PlatformTarget>`.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."` via `OnError`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await pipeline.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start` (see "License registration" above). Every pipeline instance in the process needs its own call. The SDK never shows a blocking dialog from a console host — the trial-expiry message comes through `OnError` only, so make sure you've subscribed to that event before `Start` or the failure will look like a silent no-op.

### 3. Pipeline `OnError` fires with "Element 'X' not found" / "no element 'h264parse'" / "Codec not found"

**Cause**: an encoder/muxer/sink block in the graph requires native plugins from a redist that isn't referenced. For example, `WebMSinkBlock` + `VPXEncoderBlock` + `VorbisEncoderBlock` need both core and libav redists; some H.264 hardware paths also need the libav redist; AAC encoders pull from libav.

**Fix**: ensure both `VisioForge.CrossPlatform.Core.Windows.x64` and `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` are referenced (the libav package is what most "missing element" errors trace back to). For specialised sinks (NDI, Decklink, WHIP) check the matching upstream sample's csproj for additional redist packages.

### 4. Adding a `VideoRendererBlock` in a console app — `ArgumentNullException` on construction or silent no-op

**Cause**: console-specific. `VideoRendererBlock` and `AudioRendererBlock` are designed to terminate a graph at a UI surface (`VideoView` for WPF/WinForms/MAUI/Avalonia/Uno). Constructing them in a console host either throws on the missing host argument or constructs a renderer with no surface to draw into — frames flow until the renderer's queue saturates, then the upstream tee/encoder back-pressures and the pipeline appears to hang.

**Fix**: do not use renderer blocks in a console host. Terminate the graph on a file sink (`MP4SinkBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, …) or a network sink (`RTSPServerBlock`, `RTMPSinkBlock`, `SRTMPEGTSSinkBlock`, `HLSSinkBlock`, `WHIPSinkBlock`, …). If you genuinely need to consume frames in code from a console host, use `AppSinkBlock` / `VideoSampleGrabberBlock` and process the buffers in your callback instead of rendering them.

### 5. Process exits before the file is finalised — output is empty / unplayable

**Cause**: console-host-specific. Console apps have no message pump and no `Application.Run` blocking the entry thread, so if `Main` returns immediately after `Start()` the pipeline is torn down (or the process exits) mid-record. The MP4/MPEG-TS/WebM muxer hasn't written its trailer yet, so the file is unplayable. Symptom: 0-byte output or a truncated file that VLC opens but seeks weirdly through.

**Fix**: keep the entry thread blocked while the pipeline is running. The bundled `Program.cs` polls `pipeline.State == PlaybackState.Play` until the source EOS propagates. For long-running streaming pipelines (no EOS — a webcam-to-RTSP server, for example), block on a `ManualResetEvent` set from `Console.CancelKeyPress` or `OnStop`. Whatever you pick, `pipeline.Stop()` must run to completion **before** `pipeline.Dispose()` — `Stop` flushes the muxer trailer synchronously. Letting the process unwind via Ctrl-C without a console-cancel handler skips both, so wire `Console.CancelKeyPress` to call `Stop` + `Dispose` for graceful shutdown if you accept signal termination.

### 6. No video device found / capture starts but no frames flow (only when capturing from a webcam)

**Cause**: console hosts run in the same desktop session as the user that launched them — fine for an interactive console. But running under a scheduled task with "Run whether user is logged on or not" (session 0) cuts off most webcam access; the SDK enumerates zero devices or hangs on `Start`. Same trap for invocation from a Windows Service host. (Does not apply to file-source or network-source pipelines.)

**Fix**: USB webcams are bound to a logged-on user's desktop session and cannot be reached from session 0 — packaging this as a Windows Service does not bridge that gap. For unattended capture, either keep the user-session console running (autostart on logon, scheduled task with "Run only when user is logged on"), or switch the source to a network camera (RTSP/IP camera) or file — both of those work fine from session 0 and from a Windows Service host. File sources don't have this problem.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run on a fresh machine takes 2-5 s during the implicit (or explicit `await VisioForgeX.InitSDKAsync()`) registry build; second run is instant.
- [ ] Running the app with a sample input file produces `<sourcename>_new.mp4` next to it, and the file plays cleanly in VLC.
- [ ] `pipeline.Stop()` runs to completion before `pipeline.Dispose()` (and before the process exits) — output file has a valid muxer trailer.
- [ ] `OnError` is wired *before* `Start` so trial-expiry / codec-missing errors surface instead of becoming a silent no-op.
- [ ] No `VideoRendererBlock` / `AudioRendererBlock` in the graph — every sink is a file sink, network sink, or app sink.
- [ ] `VisioForgeX.DestroySDK()` is called on shutdown (after `Dispose`) — skipping it leaks the plugin registry.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance before its `Start` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy both files into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working console csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph). Targets `net10.0-windows`.
- `references/Program.cs` — full upstream FileConvert sample: file-to-MP4 transcode via `UniversalSourceBlock` + `H264EncoderBlock` + `AACEncoderBlock` + `MP4SinkBlock`, with `OnError` and `OnStop` wiring and the `pipeline.State == Play` blocking loop. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence — see "License registration" above for where it goes.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - [`media-blocks-sdk-net-wpf`](../media-blocks-sdk-net-wpf/SKILL.md) — same SDK on WPF (with preview).
    - [`video-capture-sdk-net-console`](../video-capture-sdk-net-console/SKILL.md) — high-level capture-and-record API on console; use this if you don't need a custom pipeline.
    - `media-blocks-sdk-net-winforms` — same SDK on WinForms.
    - `media-blocks-sdk-net-{maui,avalonia,uno,android,ios,macos}` — same SDK on cross-platform hosts.
