---
name: video-edit-sdk-x-winforms
description: Integrate VisioForge Video Edit SDK X (cross-platform editor edition) into a Windows Forms application. Covers the timeline model, the cross-platform NuGet package layout, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when you want non-linear editing on WinForms with an API that ports cleanly to Avalonia, Console, WPF — for Windows-only with the legacy DirectShow stack, use video-edit-sdk-net-winforms instead.
---

# Video Edit SDK X — WinForms integration

This skill helps you add **VisioForge Video Edit SDK X** — the cross-platform "X" edition of the editor SDK — to a Windows Forms application. The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes a high-level non-linear-editor god-object (`VideoEditCoreX`) that mirrors the legacy `VideoEditCore` API but runs on the cross-platform engine. Same C# code targets Windows / macOS / Linux / iOS / Android — the only thing that changes between platforms is the UI host (WinForms here, Avalonia / Console / WPF / MAUI elsewhere) and the per-OS native redist NuGet package.

Pinned NuGet versions: wrapper **`2026.5.4`**, redist **`2026.4.29`** (matches the upstream Main Demo X sample at `_DEMOS/Video Edit SDK X/WinForms/CSharp/Main Demo X/`). The redist version tracks the underlying GStreamer rebuild cadence and lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redists to match the wrapper.

## When to use this skill

- Cutting / trimming / merging existing video files in a Windows WinForms app **with an API that ports unchanged to other platforms** (Avalonia, Console, WPF, MAUI).
- Transcoding from one container/codec to another (MP4, WebM, MKV, WMV, MP3, M4A, OGG, FLAC, …).
- Applying effects (crop, rotate/flip, brightness/contrast/hue/saturation, grayscale, sepia, text/image overlays, deinterlace) to existing files.
- Building a slideshow from images and audio with optional transitions (`Video_Transitions_Names()` enumerates the available presets).
- Sharing edit/transcode code between a Windows WinForms "main" app and one or more cross-platform companion apps.

## When NOT to use this skill

- **Live capture** from webcam / IP camera / screen → `video-capture-sdk-x-winforms` for the X engine, or `video-capture-sdk-net-winforms` for the Windows-only legacy stack. Video Edit SDK X is offline-only — it operates on existing files on disk.
- **Windows-only legacy stack** (DirectShow / Media Foundation, smaller deploy footprint, no GStreamer redist): use `video-edit-sdk-net-winforms`. The two SDKs ship side-by-side and can coexist in one app.
- **Custom pipeline topology** (per-block control over decoders, filters, sinks, multi-track mixing without re-encoding the whole timeline): use `media-blocks-sdk-net-winforms` — `VideoEditCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without editing): `media-player-sdk-net-winforms`.
- **WPF instead of WinForms**: same SDK, different UI shell → `video-edit-sdk-x-wpf`.
- **Cross-platform host instead of WinForms**: same SDK, different UI shell → `video-edit-sdk-x-avalonia`, `video-edit-sdk-x-console`.

## Project setup

### Target framework

Video Edit SDK X 2026.x supports `net472`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows` on the WinForms host. Pick the highest your toolchain supports. The csproj uses the plain **`Microsoft.NET.Sdk`** (not `Microsoft.NET.Sdk.WindowsDesktop`) with `<UseWindowsForms>true</UseWindowsForms>` — same convention as Media Blocks, intentional and required for the cross-platform `VisioForge.Core` reference graph to resolve correctly.

### NuGet packages

Three packages are required for a Windows WinForms edit-and-transcode scenario — the .NET wrapper plus two native redist packages (Core runtime + libav muxers/encoders). The redists are **not** transitive; you must reference them explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>
```

`VisioForge.DotNet.VideoEdit` is the **same wrapper package** the legacy SDK uses — both `VideoEditCore` (legacy) and `VideoEditCoreX` (cross-platform) ship in it. What switches you to the X engine is the redist pair (`VisioForge.CrossPlatform.Core.Windows.x64` + `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]`) plus the mandatory `VisioForgeX.InitSDKAsync()` boot below. The bundled csproj uses the `.UPX` (UPX-compressed) variant for a smaller download at the cost of a slightly slower first-load; the non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable. Pick one and stay consistent within the project.

For 32-bit deployment, swap `.x64` for `.x86` on both redists. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist and drop `<PlatformTarget>` from the csproj.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the upstream Main Demo X sample (`_DEMOS/Video Edit SDK X/WinForms/CSharp/Main Demo X/`) — the seven per-TFM upstream csprojs (`net472`, `net5..net10`) are collapsed into a single multi-target file with `<TargetFrameworks>net472;net10.0-windows</TargetFrameworks>`; demo-only properties are removed (`<ProductVersion>`, `<AssemblyVersion>` / `<FileVersion>`, `<PostBuildEvent>`, the per-Configuration `<DebugType>` blocks, the `Properties\Resources.*` / `Properties\Settings.*` Compile/EmbeddedResource entries that the modern SDK auto-includes); the `<ApplicationIcon>` and `<ApplicationManifest>` references are dropped (the default WinForms manifest is sufficient on Windows 10/11 and the bundled reference does not ship an icon file). The bundled file builds standalone against the public NuGet packages.

### Project platform

Video Edit SDK X WinForms samples use `<PlatformTarget>x64</PlatformTarget>` — this differs from the legacy SDK (which uses AnyCPU). The reason is the redist packages are split per-architecture; referencing only `.x64` and pinning `<PlatformTarget>` to match makes the runtime resolution unambiguous.

## Mandatory engine boot

Before any `VideoEditCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown:

```csharp
// Form1.cs — Form1_Load
private async void Form1_Load(object sender, EventArgs e)
{
    Text += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
    this.Enabled = false;
    await VisioForgeX.InitSDKAsync();
    this.Enabled = true;
    Text = Text.Replace(" [FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");

    // ...now safe to construct VideoEditCoreX, populate transition lists, etc.
    CreateEngine();
}

// Form1_FormClosing
private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    VideoEdit1?.Stop();
    VisioForgeX.DestroySDK();
}
```

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run. The bundled `references/Form1.cs` shows the canonical placement.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoEdit1.SetLicenseCertificateAsync(certBytes)` on every `VideoEditCoreX` instance, after the constructor and before `Start()`:

```csharp
VideoEdit1 = new VideoEditCoreX(VideoView1 as IVideoView);
VideoEdit1.OnError += VideoEdit1_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await VideoEdit1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. For multi-form apps, every `VideoEditCoreX` instance needs its own `SetLicenseCertificateAsync` call before `Start()`. Where the bytes come from (env var, embedded resource, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/Form1.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoEdit1 = new VideoEditCoreX(...)`.

## Timeline model

`VideoEditCoreX` is a non-linear editor: the ordered list of input segments — populated through `Input_AddVideoFile`, `Input_AddAudioVideoFile`, `Input_AddAudioFile`, `Input_AddImageFile` — is concatenated into a single output stream that re-encodes through the format set in `Output_Format`. Each `Input_Add*` call accepts a sub-clip range (`startTime`, `stopTime`) and an `insertTime` that places the segment at an explicit absolute timeline position; passing `null` for `insertTime` chains it after the previous file ("insert after previous file" in the demo UI). For a single-file fast-cut without re-encode use the legacy `VideoEditCore` (DirectShow) — `VideoEditCoreX` always decodes and re-encodes through the cross-platform pipeline.

Concrete timeline build (image + audio slideshow + transition + MP4 transcode):

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.UI;
using VisioForge.Core.VideoEditX;

await VisioForgeX.InitSDKAsync();
var core = new VideoEditCoreX(VideoView1 as IVideoView);

core.Output_VideoSize       = new Size(1920, 1080);
core.Output_VideoFrameRate  = new VideoFrameRate(30);

// 2-second still image, then a video clip trimmed to 5..15s.
core.Input_AddImageFile(@"C:\photos\title.jpg", TimeSpan.FromSeconds(2), insertTime: null);
core.Input_AddAudioVideoFile(
    @"C:\clips\b.mp4",
    TimeSpan.FromSeconds(5),
    TimeSpan.FromSeconds(15),
    insertTime: null);

// Optional: insert a 1s crossfade between the two segments.
// The available preset names come from VideoEditCoreX.Video_Transitions_Names().
core.Video_Transitions.Add(new VideoTransition(
    "Fade",
    TimeSpan.FromSeconds(2),
    TimeSpan.FromSeconds(3)));

core.Output_Format = new MP4Output(@"C:\out\slideshow.mp4");
core.Start();   // OnProgress fires; OnStop fires when finished
```

`Mode = 0` in the bundled demo's `cbMode` selects "Convert" (re-encode through `Output_Format`); `Mode = 1` runs preview without writing a file. The demo defaults to preview — flip the combo to record.

## Hello-World edit

Minimum viable cut-and-transcode snippet — a self-contained `Form1` you can drop into a fresh WinForms project. (For the full feature set, copy `references/` into your project and skip this section.) In the designer, drop a `VisioForge.Core.UI.WinForms.VideoView` from the toolbox (auto-populates after first build), name it `VideoView1`, then add a `Button` named `StartButton` wired to `StartButton_Click`. Replace `YourApp` with your project's `<RootNamespace>`:

```csharp
// Form1.cs
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.UI;
using VisioForge.Core.VideoEditX;

namespace YourApp
{
    public partial class Form1 : Form
    {
        private VideoEditCoreX _videoEdit;

        public Form1() => InitializeComponent();

        private async void Form1_Load(object sender, EventArgs e)
        {
            // Mandatory engine boot — see "Mandatory engine boot" above.
            await VisioForgeX.InitSDKAsync();
            _videoEdit = new VideoEditCoreX(VideoView1 as IVideoView);
            // For a purchased licence:
            //   await _videoEdit.SetLicenseCertificateAsync(File.ReadAllBytes("your.vflicense"));
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            // Catch — an unhandled exception in a WinForms event handler
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, registry not built (forgot InitSDKAsync), bad
            // input path.
            try
            {
                var output = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                    "output.mp4");

                // Trim 5..15 seconds out of the source.
                _videoEdit.Input_AddAudioVideoFile(
                    @"C:\clips\source.mp4",
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(15),
                    insertTime: null);

                _videoEdit.Output_Format = new MP4Output(output);
                _videoEdit.Start();   // OnProgress / OnStop fire on the engine thread
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Edit failed: {ex.Message}");
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _videoEdit?.Stop();
            _videoEdit?.Dispose();
            VisioForgeX.DestroySDK();
        }
    }
}
```

`references/Form1.cs` (paired with `references/Form1.Designer.cs`) ships the full pattern with multi-input timeline (video / audio / image), output format dialogs (MP4, WebM, MKV, WMV, WAV, MP3, M4A, WMA, OGG Vorbis, FLAC, Speex), audio effects (amplify, 10-band equalizer), video effects (deinterlace, balance, grayscale, sepia, flip, text/image overlay), crop, rotate, transitions, progress bar, and `OnError` / `OnStart` / `OnStop` / `OnProgress` wiring.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL" / "no element X"

**Cause**: forgot the `await VisioForgeX.InitSDKAsync()` boot, **or** the redist NuGet for the build's RID is missing (`VisioForge.CrossPlatform.Core.Windows.x64` not referenced for an x64 build), **or** wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29`).

**Fix**: confirm `InitSDKAsync` runs before any other SDK call (see "Mandatory engine boot"). Confirm the redist NuGet matches the build platform (`x64` redist for x64, `x86` redist for x86, both for AnyCPU). Pin the redist version to the value shipped in the upstream csproj for your wrapper version — do not bump.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoEditCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoEdit.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start()` (see "License registration" above). Every `VideoEditCoreX` instance in the process needs its own call.

### 3. `OnError` fires with "Codec not found" / "Element 'X' not found"

**Cause**: the output format depends on a GStreamer plugin not present in the referenced redist. The default `VisioForge.CrossPlatform.Libav.Windows.x64[.UPX]` covers MP4 (libav h264), AAC, MPEG-TS, MOV, AVI, WebM, MP3, M4A, OGG Vorbis, and FLAC out of the box. Less common codecs (Speex, WMA/WMV via plugin variants, HAP, DNxHD, ProRes) may need a different redist family.

**Fix**: check the error string against the upstream sample's csproj for the format you need; if it references an additional redist, add it with the same version pin as the others. The bundled `references/Form1.cs` exposes WMV / WMA / Speex options in the output combo — those work on Windows but require the matching codec plugins to be present on the build's RID.

### 4. `Output_Format = null` produced no output file

**Cause**: in the bundled demo, `cbMode.SelectedIndex == 1` (the default — "Preview") sets `Output_Format = null` on `Start()`, which runs the timeline through the preview window without writing a file. Easy to miss when copy-pasting from the demo: the `Output_Format = new MP4Output(...)` block only runs in the `cbMode.SelectedIndex == 0` ("Convert") branch.

**Fix**: when adapting the bundled `references/Form1.cs` for your own app, either flip the mode combo to "Convert" before `Start()`, or remove the `if (cbMode.SelectedIndex == 0)` guard and assign `Output_Format` unconditionally.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] First run shows the "[FIRST TIME LOAD, BUILDING THE REGISTRY...]" title for ~2-5 s (the registry build), then `OnProgress` updates within ~1 s on subsequent launches.
- [ ] `OnStop` fires with `e.Successful == true` for a basic MP4 transcode of an existing file.
- [ ] On clean shutdown, `Form1_FormClosing` runs `Stop()` before `VisioForgeX.DestroySDK()`.
- [ ] If recording to MP4: output file is finalised correctly when `OnStop` fires (do not delete the file mid-render — the muxer writes the moov atom on close).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCoreX` instance before its `Start()` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WinForms csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/Program.cs` — WinForms entry point with the standard `[STAThread] Application.Run(new Form1())` boilerplate.
- `references/Form1.cs` — full code-behind with `InitSDKAsync` boot, timeline assembly (video / audio / image inputs with sub-clipping and explicit insert times), output-format selection (MP4 / WebM / MKV / WMV / WAV / MP3 / M4A / WMA / OGG Vorbis / FLAC / Speex), audio effects (amplify, 10-band equalizer), video effects (deinterlace, balance, grayscale, sepia, flip, text/image overlay), crop, rotate, transitions, progress bar, subtitles support, and `OnError` / `OnStart` / `OnStop` / `OnProgress` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Form1.Designer.cs` — the matching designer file (icon reference stripped — the bundled reference does not ship a `.ico`).
- `references/Form1.resx` — the matching resx (icon resource stripped to match).

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-x-wpf` — same scenario on WPF (concurrently authored).
    - `video-edit-sdk-x-avalonia` — same SDK on Avalonia for cross-platform desktop hosting.
    - `media-blocks-sdk-net-winforms` — same engine, lower-level graph-based API for custom pipeline topologies.
    - `video-edit-sdk-net-winforms` — same scenario on the Windows-only DirectShow / Media Foundation stack (smaller deploy footprint, no GStreamer redist).
    - `video-capture-sdk-x-winforms` — same X engine for live capture instead of file editing.
    - `video-edit-sdk-x-console` — same X edit SDK for headless / batch transcoding.
