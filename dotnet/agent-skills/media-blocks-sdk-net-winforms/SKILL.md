---
name: media-blocks-sdk-net-winforms
description: Integrate VisioForge Media Blocks SDK .NET into a Windows Forms application. Covers the graph-based pipeline model (MediaBlocksPipeline, source/sink/transform blocks), the single NuGet package, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when building custom media pipelines (capture, transcode, mix, stream, record) on a WinForms app — for simpler webcam-only capture, use video-capture-sdk-net-winforms instead.
---

# Media Blocks SDK .NET — WinForms integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a Windows Forms application. Unlike Video Capture SDK's high-level "set device + click record" API, Media Blocks is a **graph-based pipeline SDK**: you instantiate `MediaBlocksPipeline`, then construct source / transform / sink blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, …) and wire their pads together with `pipeline.Connect(outPad, inPad)`. This is the right tool when you need a custom topology — multi-source mixing, branched recording-plus-streaming via `TeeBlock`, on-the-fly transcoding, sample-grabber callbacks, etc. For a plain "show webcam, hit record" scenario, prefer `video-capture-sdk-net-winforms`.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Video Capture demo for Media Blocks WinForms](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/Simple%20Video%20Capture%20Demo)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Adding custom media pipelines to a WinForms app (capture + record + stream simultaneously, mixing, transcoding, network sinks).
- Building cross-platform-ready code paths (Media Blocks is the cross-platform line — same API surface backs MAUI / Avalonia / Uno hosts).
- Inserting custom processing via `VideoSampleGrabberBlock` / `AudioSampleGrabberBlock` callbacks.
- Recording, RTMP/RTSP/HLS/SRT streaming, file-to-file transcoding, multi-camera mixing in a WinForms shell.

## When NOT to use this skill

- **Plain webcam capture, no custom pipeline**: simpler API → use `video-capture-sdk-net-winforms`. Same vendor, much less wiring.
- **WPF instead of WinForms**: same SDK, different UI host → `media-blocks-sdk-net-wpf`.
- **Cross-platform host (MAUI / Avalonia / Uno)**: same SDK, different host → `media-blocks-sdk-net-{maui,avalonia,uno}`.
- **Playback only** (no capture / pipeline assembly): consider `media-player-sdk-net-winforms`, or just use a `UniversalSourceBlock` -> renderer pair from this skill if you need the Media Blocks pipeline anyway.

## Project setup

### Target framework

Media Blocks SDK .NET 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj must use the standard `Microsoft.NET.Sdk` and set `<UseWindowsForms>true</UseWindowsForms>` plus a `-windows` TFM.

### NuGet packages

The SDK ships as a single .NET meta-package plus two native runtime packages (the Media Blocks engine wraps GStreamer; the native binaries do not flow through the .NET package transitively the way Video Capture SDK's classic engine does — you must reference them explicitly):

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.CrossPlatform.Core.Windows.x64` is the GStreamer core + VisioForge plugins. `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` is the FFmpeg/libav redist (needed for H.264/AAC encode and most decode paths). Pin the runtime packages to the latest 2026.x release available — version drift between the .NET wrapper and native runtime is undefined behaviour but minor patch-version skew (as shown above: `2026.5.4` wrapper + `2026.4.29` runtime) matches the official sample and works in practice.

For 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Simple Video Capture Demo (`_DEMOS/Media Blocks SDK/WinForms/CSharp/Simple Video Capture Demo/`). Changes vs upstream: `<PlatformTarget>x64</PlatformTarget>` is dropped (stays AnyCPU — see "Project platform" below), the per-Configuration `<Optimize>` blocks are dropped (defaults are correct), and the project name is shortened to `Sample.csproj`.

### Project platform

Use AnyCPU (the default — no `<PlatformTarget>` line). The native runtime NuGet packages (`VisioForge.CrossPlatform.Core.Windows.x64` etc.) ship binaries under `runtimes/win-x64/native/` and resolve at runtime via the standard RID convention. The official sample explicitly sets `<PlatformTarget>x64</PlatformTarget>`; that works on x64-only machines but breaks if your build agent or end-user is on a different architecture or you later add x86 runtime packages. Drop it unless you have a specific reason.

## Pipeline model in 60 seconds

A pipeline is a directed graph of **blocks** wired together:

- **Source blocks** produce data: `SystemVideoSourceBlock` (camera), `SystemAudioSourceBlock` (mic), `UniversalSourceBlock` (file/URL), `RTSPSourceBlock`, `ScreenCaptureSourceBlock`, …
- **Transform blocks** process it: encoders (`H264EncoderBlock`, `AACEncoderBlock`), `TeeBlock` (split N-ways), `VideoMixerBlock`, sample grabbers (`VideoSampleGrabberBlock`).
- **Sink blocks** consume it: renderers (`VideoRendererBlock`, `AudioRendererBlock`), file muxers (`MP4SinkBlock`, `WebMSinkBlock`), network sinks (`RTMPSinkBlock`, `HLSSinkBlock`).

Every block exposes `Input`, `Output`, `Inputs[]`, or `Outputs[]` *pads* (you'll see `block.Output`, `tee.Outputs[0]`, `muxer.CreateNewInput(...)`). You connect them with `pipeline.Connect(srcPad, dstPad)`, then call `await pipeline.StartAsync()`. That's the whole API.

A typical capture-and-record graph:

```
SystemVideoSourceBlock ──► TeeBlock(2) ──► VideoRendererBlock          (preview)
                                       └─► H264EncoderBlock ──► MP4SinkBlock
SystemAudioSourceBlock ──► TeeBlock(2) ──► AudioRendererBlock          (monitor)
                                       └─► AACEncoderBlock ──► (same MP4SinkBlock, audio input)
```

That graph is exactly what `references/MainForm.cs` builds.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, load the `.vflicense` file as bytes and call `await pipeline.SetLicenseCertificateAsync(certBytes)` on **every** `MediaBlocksPipeline` instance, after construction and before `StartAsync` — there is no global "set once" helper:

```csharp
// references/MainForm.cs — btStart_Click, after `_pipeline = new MediaBlocksPipeline();`
var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _pipeline.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed in that release. For multi-pipeline apps every `MediaBlocksPipeline` instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK. Without this call, `MediaBlocksPipeline.StartAsync()` succeeds in trial mode for the first 30 days, then aborts with a "trial expired" error event on `OnError`.

## Hello-World pipeline

Minimum viable capture-and-preview snippet — a self-contained `MainForm` you can drop into a fresh WinForms project. (For the full reference with audio, recording, format selection, and OnError wiring, copy `references/` into your project and skip this section.) Replace `YourApp` with your project's `<RootNamespace>` from the csproj.

`Program.cs`:

```csharp
using System;
using System.Windows.Forms;

namespace YourApp
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
```

`MainForm.Designer.cs` (drop a `VideoView` control + a Start button on the form via the Designer; the relevant generated bits):

```csharp
using VisioForge.Core.UI.WinForms;
// ...
private VideoView VideoView1;          // VisioForge.Core.UI.WinForms.VideoView
private System.Windows.Forms.Button btStart;
```

`MainForm.cs` code-behind:

```csharp
using System;
using System.Linq;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;

namespace YourApp
{
    public partial class MainForm : Form
    {
        private MediaBlocksPipeline _pipeline;

        public MainForm() => InitializeComponent();

        // Initialise the SDK from Form_Load, NOT the constructor — the form's
        // HWND is not realised in the ctor, and InitSDKAsync's first run can
        // take ~5 s building the GStreamer plugin registry.
        private async void MainForm_Load(object sender, EventArgs e)
        {
            await VisioForgeX.InitSDKAsync();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // async-void event handlers must catch — an exception otherwise
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native runtime DLLs, no camera connected, device busy.
            try
            {
                _pipeline = new MediaBlocksPipeline();
                // var cert = File.ReadAllBytes("your.vflicense");
                // await _pipeline.SetLicenseCertificateAsync(cert);   // per-pipeline, before StartAsync
                _pipeline.OnError += (s, ev) => Debug.WriteLine(ev.Message);

                var devices = await SystemVideoSourceBlock.GetDevicesAsync();
                if (devices.Count == 0) { MessageBox.Show("No camera"); return; }

                var settings = new VideoCaptureDeviceSourceSettings(devices[0]);
                // Letting the SDK pick the default format is safer than
                // settings.Format = devices[0].VideoFormats[0].ToFormat() —
                // some virtual cameras enumerate with no formats and crash
                // the property setter.

                var src = new SystemVideoSourceBlock(settings);
                var renderer = new VideoRendererBlock(_pipeline, VideoView1);

                _pipeline.Connect(src.Output, renderer.Input);
                await _pipeline.StartAsync();
            }
            catch (Exception ex) { MessageBox.Show($"Pipeline failed: {ex.Message}"); }
        }

        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_pipeline != null)
            {
                await _pipeline.StopAsync();
                await _pipeline.DisposeAsync();
            }
            VisioForgeX.DestroySDK();
        }
    }
}
```

`references/MainForm.cs` ships the full pattern with audio, optional MP4 recording via `TeeBlock` branching, format/frame-rate enumeration, and `OnError` wiring.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'libgstreamer-1.0-0'" / "gstreamer-1.0 not found"

**Cause**: the native runtime NuGet (`VisioForge.CrossPlatform.Core.Windows.x64`) was not referenced, or `<PlatformTarget>` is set to a value (`x86`, `arm64`) that doesn't match the runtime package architecture.

**Fix**: reference both `VisioForge.CrossPlatform.Core.Windows.x64` and `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` explicitly (the .NET wrapper does NOT bring them in transitively for the cross-platform line). Keep `<Platforms>AnyCPU</Platforms>` or align `<PlatformTarget>` with the redist RID.

### 2. Trial-mode message / pipeline aborts with "trial period is over"

**Cause**: no `.vflicense` was loaded — either nothing was loaded at all, or `SetLicenseCertificateAsync` was called *after* `pipeline.StartAsync()`, or the call was skipped in a non-debug build path.

**Fix**: `await _pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`. The call is per-pipeline — multi-pipeline apps need one call per instance.

### 3. `OnError` fires with "no element ..." / "could not link" / "missing plugin"

**Cause**: a block's underlying GStreamer element is in a redist not referenced by the project. The most common case is encoder blocks (`H264EncoderBlock`, `AACEncoderBlock`) without `VisioForge.CrossPlatform.Libav.Windows.x64.UPX`, or `WebMSinkBlock` without a WebM-aware redist.

**Fix**: reference `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` for any encode/decode work. For exotic codecs (some VPx, AV1) reference the matching cross-platform codec redist.

### 4. Black / frozen preview, no `OnError`

**Cause**: `pipeline.StartAsync()` was called before the `VideoView` control had a valid HWND — most often because the start logic was wired to the form constructor or `Form_Load` *before* the view's `OnHandleCreated` fires for the first time.

**Fix**: defer `StartAsync()` to a button click, or to `Form.Shown` rather than `Form.Load`. The official sample pattern (and `references/MainForm.cs`) uses an explicit Start button — copy that pattern. If you must auto-start, hook `VideoView1.HandleCreated` and start from there.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing native runtime warnings).
- [ ] First run shows the "[LOADING...]" banner for ~5 s while the GStreamer plugin registry builds, then a webcam preview appears within a second after clicking Start.
- [ ] Stop / Start cycle leaves no leaks — `await _pipeline.StopAsync(); await _pipeline.DisposeAsync(); _pipeline = null;` runs cleanly.
- [ ] On `Form.FormClosing`, `VisioForgeX.DestroySDK()` is called after the pipeline is disposed (tearing the SDK down before stopping the pipeline crashes the GStreamer worker thread).
- [ ] If recording to MP4 with the capture branch enabled: output file is finalised correctly when Stop is clicked (the muxer needs the EOS event from `StopAsync` to write the moov atom).
- [ ] If a purchased licence is in use: `await _pipeline.SetLicenseCertificateAsync(certBytes)` is called on every `MediaBlocksPipeline` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WinForms csproj, version-pinned to the same NuGet release as the prose.
- `references/Program.cs` — `Application.Run(new MainForm())` entry point.
- `references/MainForm.cs` — full code-behind with capture, optional MP4 recording via `TeeBlock`, device enumeration, format selection, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call on every `MediaBlocksPipeline` instance when integrating a purchased licence — the spot is marked with a comment in `btStart_Click`.)
- `references/MainForm.Designer.cs` — Designer-generated UI: tabbed source/output config, transport buttons, and the `VisioForge.Core.UI.WinForms.VideoView` preview surface.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-wpf` — same SDK on WPF.
    - `media-blocks-sdk-net-maui` / `-avalonia` / `-uno` — same SDK on cross-platform hosts.
    - `video-capture-sdk-net-winforms` — simpler high-level capture API for the "webcam + record" case where you don't need a custom pipeline.
