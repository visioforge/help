---
name: video-capture-sdk-net-winforms
description: Integrate VisioForge Video Capture SDK .NET into a Windows Forms application. Covers the single NuGet package, project setup, license registration, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when adding webcam, IP camera, screen, or DV capture to a WinForms app on Windows.
---

# Video Capture SDK .NET — WinForms integration

This skill helps you add **VisioForge Video Capture SDK .NET** to a Windows Forms application. It covers webcam, IP camera, screen, and DV-camera capture with preview, recording, and snapshot. The SDK is Windows-only (DirectShow / Media Foundation under the hood) — for cross-platform capture (macOS, iOS, Android, Linux), use one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills instead.

Pinned NuGet version: **`2026.5.4`** (matches the [official Simple Video Capture sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Simple%20VideoCapture)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Adding webcam, IP camera, screen, or DV capture to a Windows Forms app.
- Recording captured video/audio to MP4, AVI, MOV, MPEG-TS, WebM, or WMV.
- Taking still snapshots from a live capture stream.
- Hosting the preview in a WinForms surface using the `VideoView` control.

## When NOT to use this skill

- **Cross-platform**: target macOS, iOS, Android, or Linux → use `media-blocks-sdk-net-{maui,avalonia,uno}` instead. Video Capture SDK is Windows-only.
- **Editing instead of capture**: cut, merge, transcode existing files → `video-edit-sdk-net-winforms`.
- **Playback only**: play files / streams without capturing → `media-player-sdk-net-winforms`.
- **WPF instead of WinForms**: same SDK, different UI host → [`video-capture-sdk-net-wpf`](../video-capture-sdk-net-wpf/SKILL.md).

## Project setup

### Target framework

Video Capture SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** SDK with `<UseWindowsForms>true</UseWindowsForms>` — note this is *not* `Microsoft.NET.Sdk.WindowsDesktop` (that's the WPF SDK).

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for a basic capture-and-record scenario.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
```

If you add codec-heavy outputs (WebM, certain GPU-accelerated MP4 paths), add the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture sample (`_DEMOS/Video Capture SDK/WinForms/CSharp/Simple VideoCapture/`). Changes vs upstream: the seven per-TFM upstream csprojs (`net472`, `net5..net10`) are collapsed into a single multi-target file with `<TargetFrameworks>net472;net10.0-windows</TargetFrameworks>`; demo-only properties are removed (`<ProductVersion>`, `<IsWebBootstrapper>`, `<RuntimeIdentifiers>`, `<PlatformTarget>`, the per-Configuration `<DebugType>` blocks, `<PostBuildEvent>`, `<AssemblyVersion>` / `<FileVersion>`, the `Properties\Resources.*` and `Properties\Settings.*` Compile/EmbeddedResource entries that the modern SDK auto-includes); the redundant `app.manifest` reference is dropped (the default WinForms manifest is sufficient on Windows 10/11). The bundled file builds standalone against the public NuGet package.

### Project platform

Use `<Platforms>AnyCPU</Platforms>` (the default). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below. (Note: the upstream `net472.csproj` does pin `<PlatformTarget>x64</PlatformTarget>` — that's a legacy demo choice, the bundled `Sample.csproj` removes it on purpose.)

## License registration

The SDK ships with a 30-day trial — the bundled `references/Form1.cs` runs in trial mode by design (the upstream demo never sets a licence anywhere). To register a purchased licence in the bundled reference, add two lines to its existing `CreateEngineAsync()` method right after the `CreateAsync` call:

```csharp
// references/Form1.cs — CreateEngineAsync(), at line ~117
private async Task CreateEngineAsync()
{
    VideoCapture1 = await VideoCaptureCore.CreateAsync(VideoView1 as IVideoView);

    // Add these two lines:
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    await VideoCapture1.SetLicenseCertificateAsync(cert);

    VideoCapture1.OnError += VideoCapture1_OnError;
}
```

If you're starting from scratch without the bundled reference, the same pattern applies in your own `Form1.cs` — call `await videoCapture.SetLicenseCertificateAsync(certBytes)` after `await VideoCaptureCore.CreateAsync(...)` and before `StartAsync`. (See the Hello-World snippet below for a full standalone `Form1`.)

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For multi-form apps, every `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call before `StartAsync`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World capture

Minimum viable capture-and-preview snippet — a self-contained `Form1` you can drop into a fresh WinForms project. (For the full feature set, copy `references/` into your project and skip this section.) Replace `YourApp` below with your project's `<RootNamespace>` from the csproj:

In the designer, drop a `VisioForge.Core.UI.WinForms.VideoView` from the toolbox onto the form (after first build the toolbox auto-populates), name it `VideoView1`, then add a `Button` named `StartButton` with a `Click` handler. The minimal designer wiring looks like:

```csharp
// Form1.Designer.cs — the bits that matter
this.VideoView1 = new VisioForge.Core.UI.WinForms.VideoView();
this.VideoView1.Dock = System.Windows.Forms.DockStyle.Fill;
this.VideoView1.BackColor = System.Drawing.Color.Black;
this.Controls.Add(this.VideoView1);

this.StartButton = new System.Windows.Forms.Button();
this.StartButton.Text = "Start";
this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
this.Controls.Add(this.StartButton);
```

Code-behind:

```csharp
// Form1.cs
using System;
using System.Windows.Forms;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;

namespace YourApp
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore _videoCapture;
        // (The bundled `references/Form1.cs` calls this field `VideoCapture1`;
        //  use the same name there if you start from the bundled template, otherwise
        //  the snippet below won't merge cleanly.)

        public Form1() => InitializeComponent();

        private async void StartButton_Click(object sender, EventArgs e)
        {
            // async-void event handlers must catch — an exception otherwise
            // escapes to AppDomain.UnhandledException and silently terminates
            // the app. Common triggers on first run: trial expired, missing
            // native DLLs, no DirectShow stack, COM init failure, device busy.
            try
            {
                // VisioForge.Core.UI.WinForms.VideoView already implements IVideoView,
                // so passing VideoView1 directly is fine — no cast needed.
                _videoCapture = await VideoCaptureCore.CreateAsync(VideoView1);

                // Note: Video_CaptureDevices() is synchronous — there is no Async overload.
                var devices = _videoCapture.Video_CaptureDevices();
                if (devices.Count == 0)
                {
                    MessageBox.Show("No video capture devices found.");
                    return;
                }

                _videoCapture.Video_CaptureDevice = new VideoCaptureSource(devices[0].Name);
                // Let the SDK pick a default format. Setting Format = devices[0].VideoFormats[0].Name
                // crashes on devices that enumerate with no formats (some virtual cameras / IP shims).
                _videoCapture.Audio_RecordAudio = false;

                // Preview-only, no recording. For recording, set Mode = VideoCapture
                // and Output_Format / Output_Filename appropriately — see references/Form1.cs.
                _videoCapture.Mode = VideoCaptureMode.VideoPreview;

                await _videoCapture.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Capture failed: {ex.Message}");
            }
        }
    }
}
```

`references/Form1.cs` (paired with `Form1.Designer.cs`) ships the full pattern with output format dialogs (MP4, AVI, MOV, MPEG-TS, GIF), audio effects, video effects, recording-time display, and `OnError` event wiring.

## Optional codec packages

You only need these if your output format actually uses them. Default MP4 (Media Foundation hardware encoder) does not require any explicit redist — it ships in the main package.

| Output / source | Add to csproj |
|---|---|
| WebM (VP8/VP9 + Vorbis/Opus) | `VisioForge.DotNet.Core.Redist.WebM.x64` |
| FFmpeg-based output (custom muxers, network sinks) | `VisioForge.DotNet.Core.Redist.FFMPEG.x64` |
| LAV-based decoding (uncommon legacy formats) | `VisioForge.DotNet.Core.Redist.LAV.x64` |
| VLC-based source | `VisioForge.DotNet.Core.Redist.VLC.x64` |
| Xiph (Vorbis/Theora) standalone codecs | `VisioForge.DotNet.Core.Redist.XIPH.x64` |

Pin all redist packages to **the same version as `VisioForge.DotNet.VideoCapture`** — version drift between main and redist is undefined behaviour.

For a 32-bit deployment swap `.x64` for `.x86`. To support both architectures with a single AnyCPU build, reference both `.x64` and `.x86` of every redist package you need.

## Common deployment failures

These are the four most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder.

**Fix**: keep `<Platforms>AnyCPU</Platforms>` (default). If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported, the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all (trial mode shows the info string `"TRIAL version of SDK without restrictions."` via `IVideoView.ShowMessage`), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`). Or the certificate was loaded on a *different* `VideoCaptureCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _videoCapture.SetLicenseCertificateAsync(certBytes)` after the `CreateAsync` call and before `StartAsync` (see "License registration" above). For multi-form apps, every `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call. The SDK never shows a blocking "License required" dialog; it surfaces the trial-mode info string via `IVideoView.ShowMessage` and aborts startup with `"SDK TRIAL period (30 days) is over."` once the 30-day window elapses.

### 3. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: output format depends on a redist not referenced by the project (see "Optional codec packages" above).

**Fix**: add the corresponding redist NuGet package and rebuild. For example, recording to WebM without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

### 4. Preview is black / frozen on first frame

**Cause**: capture started before the WinForms `VideoView1` control has a created HWND. Most often happens when `StartAsync` runs from the form constructor — at that point the underlying handle hasn't been created yet (`Control.IsHandleCreated == false`).

**Fix**: defer to the `Form.Load` event (or hook a button click) before calling `StartAsync`. The bundled `references/Form1.cs` uses `Form1_Load` for engine creation and an explicit "Start" button for `StartAsync` — copy that pattern.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Running the app shows a webcam preview within ~1 s on a machine with a real or virtual webcam.
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCore` (always call `await _videoCapture.StopAsync(); _videoCapture.Dispose();` on form close — the bundled `Form1.Designer.cs::Dispose` already does this).
- [ ] If recording to MP4: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `Dispose`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCore` instance before `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working WinForms csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph). Multi-targets `net472;net10.0-windows`.
- `references/Program.cs` — WinForms application entry point (`Application.Run(new Form1())`).
- `references/Form1.cs` — full code-behind with capture, recording, snapshot, output-format dialogs (MP4 / AVI / WMV / MOV / MPEG-TS / GIF), audio + video effects, and `OnError` wiring. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/Form1.Designer.cs` — auto-generated UI layout for `Form1`. Hosts the `VisioForge.Core.UI.WinForms.VideoView` control (`VideoView1`) plus all the upstream demo controls. Its `Dispose` override correctly tears down both the engine and every settings dialog.
- `references/Form1.resx` — RESX for `Form1` (paired with `Form1.Designer.cs`). Trimmed: the upstream form's `<data name="$this.Icon">` block (and the matching `<ApplicationIcon>` in `Sample.csproj` plus `this.Icon = ...` line in `Form1.Designer.cs`) was deliberately removed — the SDK's own branding icon shouldn't ship into a user's app via this skill.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - [`video-capture-sdk-net-wpf`](../video-capture-sdk-net-wpf/SKILL.md) — same SDK on WPF.
    - `video-capture-sdk-x-winforms` — newer "X" line on WinForms (cross-process capture, modernised pipeline; covers a different feature set).
    - `media-blocks-sdk-net-winforms` — alternative when you need a custom media pipeline rather than the high-level capture-and-record API.
