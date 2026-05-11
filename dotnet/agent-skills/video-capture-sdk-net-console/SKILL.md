---
name: video-capture-sdk-net-console
description: Integrate VisioForge Video Capture SDK .NET into a .NET console application (no UI). Covers the single NuGet package, project setup, license registration, headless capture/recording, and the most common deployment pitfalls (DLL not found, missing codecs, trial-period expiry / unlicensed build). Use when capturing or recording from webcam, IP camera, or screen on Windows from a service, scheduled task, or batch script — for an interactive UI use video-capture-sdk-net-{wpf,winforms} instead.
---

# Video Capture SDK .NET — Console integration

This skill helps you add **VisioForge Video Capture SDK .NET** to a .NET console application — no window, no UI thread, no preview surface. The host process drives the SDK directly: enumerate devices, configure inputs, start recording, keep the process alive while frames flow, stop and dispose. The SDK is Windows-only (DirectShow / Media Foundation under the hood) — for cross-platform headless capture (macOS, iOS, Android, Linux) use one of the `media-blocks-sdk-net-{maui,avalonia,uno}` skills.

Pinned NuGet version: **`2026.5.4`** (matches the [official Video Capture Console sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/Console/Video%20Capture%20Demo)). Newer 2026.x.x patch versions are drop-in compatible.

## When to use this skill

- Recording webcam, IP camera, screen, or DV input from a console host — scheduled tasks, batch scripts, CLI tools, CI runners.
- Embedding capture-and-record into an existing console pipeline that already has its own argument parsing and lifecycle.
- Producing MP4 or AVI output files headless (no preview, no `VideoView`).

## When NOT to use this skill

- **Interactive UI**: live preview surface, buttons, file dialogs → use [`video-capture-sdk-net-wpf`](../video-capture-sdk-net-wpf/SKILL.md) or [`video-capture-sdk-net-winforms`](../video-capture-sdk-net-winforms/SKILL.md) instead.
- **Cross-platform**: target macOS, iOS, Android, or Linux → use `media-blocks-sdk-net-{maui,avalonia,uno}`. Video Capture SDK is Windows-only.
- **Editing instead of capture**: cut, merge, transcode existing files → `video-edit-sdk-net-console`.

## Project setup

### Target framework

Video Capture SDK .NET 2026.x supports `net472`, `netcoreapp3.1`, `net5.0-windows`, `net6.0-windows`, `net7.0-windows`, `net8.0-windows`, `net9.0-windows`, and `net10.0-windows`. Pick the highest your toolchain supports. The csproj uses the standard **`Microsoft.NET.Sdk`** SDK — *not* `Microsoft.NET.Sdk.WindowsDesktop` (no WPF, no WinForms). On .NET 5+ the TFM must still carry the `-windows` suffix because the underlying redist NuGets resolve native DLLs from `runtimes/win-*/native/` and the Windows-only API surface lives behind the OS guard.

### NuGet packages

The SDK ships as a single meta-package. The redist packages (Core, MP4, FFMPEG, codec runtime DLLs) come in transitively — you do **not** need to reference them explicitly for a basic capture-and-record scenario.

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
```

If you add codec-heavy outputs (WebM, certain GPU-accelerated MP4 paths), add the matching redist explicitly — see "Optional codec packages" below.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Video Capture Console sample (`_DEMOS/Video Capture SDK/Console/Video Capture Demo/`). Changes vs upstream: the seven per-TFM upstream csprojs (`net472`, `net5..net10`) are collapsed into a single multi-target file with `<TargetFrameworks>net472;net10.0-windows</TargetFrameworks>`; demo-only properties are removed (`<RuntimeIdentifiers>`, `<PlatformTarget>`, the per-Configuration `<DebugType>` blocks, the ClickOnce publish settings — `<PublishUrl>`, `<Install>`, `<UpdateEnabled>`, `<BootstrapperEnabled>`, etc. — `<AssemblyTitle>` / `<Product>` / `<Copyright>` / `<AssemblyVersion>` / `<FileVersion>`, `<OutputPath>`); the `<ApplicationIcon>` and the matching `<Content Include="visioforge_main_icon.ico" />` are deliberately dropped — the SDK's branding icon shouldn't ship into a user's app via this skill, and a console app doesn't render one anyway. The bundled file builds standalone against the public NuGet package.

### Project platform

Use `<Platforms>AnyCPU</Platforms>` (the default). The transitive redist NuGet packages contain native DLLs for x64 *and* x86 and resolve at runtime via the `runtimes/<rid>/native/` convention. **Do not** set `<PlatformTarget>x64</PlatformTarget>` alone — that's a common cause of the "DLL not found" failure mode below. (Note: the upstream `net472.csproj` does pin `<PlatformTarget>x64</PlatformTarget>` and `<RuntimeIdentifiers>win7-x64</RuntimeIdentifiers>` — that's a legacy demo choice, the bundled `Sample.csproj` removes both on purpose.)

## License registration

The SDK ships with a 30-day trial — the bundled `references/Program.cs` runs in trial mode by design (the upstream demo never sets a licence anywhere). To register a purchased licence, load the `.vflicense` file as bytes and call `SetLicenseCertificateAsync` immediately after constructing the engine and before any device enumeration or `Start()`:

```csharp
// references/Program.cs — at the top of Main, right after `new VideoCaptureCore()`
var videoCapture = new VideoCaptureCore();
videoCapture.OnError += VideoCapture_OnError;

// Add these two lines:
var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await videoCapture.SetLicenseCertificateAsync(cert);
```

Note that `SetLicenseCertificateAsync` is async, so `Main` must be `async Task Main(string[] args)` (C# 7.1+) — the bundled `Program.cs` uses synchronous `void Main` because it runs in trial mode and never awaits anything. Switch the signature when you add the licence call:

```csharp
static async Task Main(string[] args)
{
    var videoCapture = new VideoCaptureCore();
    var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
    await videoCapture.SetLicenseCertificateAsync(cert);
    // ... rest of Main
}
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — that release removed the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads across shared licensing, the public SDK wrappers, and the legacy Windows wrappers. Applications must load `.vflicense` files into memory and call `SetLicenseCertificateAsync(byte[])`. For long-running console hosts that recycle the engine (one capture session per request), every new `VideoCaptureCore` instance needs its own `SetLicenseCertificateAsync` call before `Start`. Where the bytes come from (env var, embedded resource, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World capture

Minimum viable headless record-to-file snippet — drop into a fresh console project as `Program.cs`. Records 10 seconds from the first webcam to MP4, no preview, no UI thread. (For the full interactive prompt-driven pattern, copy `references/` into your project and skip this section.) Replace `YourApp` below with your project's `<RootNamespace>` from the csproj:

```csharp
// Program.cs
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;

namespace YourApp
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            var videoCapture = new VideoCaptureCore();
            videoCapture.OnError += (s, e) => Console.WriteLine("Error: " + e.Message);

            // Headless: no window means no place to render preview. Always set this.
            videoCapture.Video_Renderer.VideoRenderer = VideoRendererMode.None;

            var devices = videoCapture.Video_CaptureDevices();
            if (devices.Count == 0)
            {
                Console.WriteLine("No video capture devices found.");
                return;
            }

            videoCapture.Video_CaptureDevice = new VideoCaptureSource(devices[0].Name);
            // Let the SDK pick a default format. Setting Format = devices[0].VideoFormats[0].Name
            // crashes on devices that enumerate with no formats (some virtual cameras / IP shims).
            videoCapture.Video_CaptureDevice.Format_UseBest = true;
            videoCapture.Video_CaptureDevice.IsAudioSource = false;

            var outputFile = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            videoCapture.Output_Filename = outputFile;
            videoCapture.Output_Format = new MP4HWOutput();
            videoCapture.Mode = VideoCaptureMode.VideoCapture;

            videoCapture.Start();
            Console.WriteLine($"Recording to {outputFile} for 10 s...");

            // Console hosts have no message pump — the process must stay alive
            // and pinned on a thread while the engine drives capture. A blocking
            // wait (Task.Delay/Thread.Sleep on the entry thread) is fine; the
            // engine runs on its own internal threads.
            await Task.Delay(TimeSpan.FromSeconds(10));

            videoCapture.Stop();
            videoCapture.Dispose();
            Console.WriteLine("Done.");
        }
    }
}
```

`references/Program.cs` ships the full interactive pattern with device/format/frame-rate enumeration prompts, AVI-vs-MP4 mode selection, ESC-to-stop loop, and `OnError` event wiring (with the trial-mode `"FULL SDK"` message filtered out). Use as a copy-paste starting template.

## Optional codec packages

You only need these if your output format actually uses them. Default MP4 (Media Foundation hardware encoder, `MP4HWOutput`) does not require any explicit redist — it ships in the main package. Same for AVI.

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

These are the most common production issues — flag any of them on first run.

### 1. `DllNotFoundException` / "Unable to load DLL 'VisioForge_*'"

**Cause**: project's `<PlatformTarget>` is set to `x64` or `x86` alone, *or* the build output is targeting a runtime identifier (`<RuntimeIdentifiers>win-x64</RuntimeIdentifiers>`) that doesn't match the redist NuGet's native folder, *or* the TFM is bare `net10.0` instead of `net10.0-windows` so the `runtimes/win-*/native/` folder is never picked up.

**Fix**: keep `<Platforms>AnyCPU</Platforms>` (default) and use a `-windows`-suffixed TFM on .NET 5+. If you must target a specific RID, ensure the redist NuGet contains a matching `runtimes/<rid>/native/` folder — for non-`win-x64` / `win-x86` RIDs this currently is **not** supported, the SDK is Windows-x86/x64 only.

### 2. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."` via `OnError`). Or the certificate was loaded on a *different* `VideoCaptureCore` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await videoCapture.SetLicenseCertificateAsync(certBytes)` after `new VideoCaptureCore()` and before `Start` (see "License registration" above). For console hosts that recycle engines per request, every new instance needs its own call. The SDK never shows a blocking dialog from a console host — the trial-expiry message comes through `OnError` only, so make sure you've subscribed to that event before `Start` or the failure will look like a silent no-op.

### 3. `OnError` fires with "Codec not found" / "Filter not registered"

**Cause**: output format depends on a redist not referenced by the project (see "Optional codec packages" above).

**Fix**: add the corresponding redist NuGet package and rebuild. For example, recording to WebM without `VisioForge.DotNet.Core.Redist.WebM.x64` triggers this on the WebM filter graph instantiation.

### 4. Process exits before the file is finalised — output is empty / unplayable

**Cause**: console-host-specific. Console apps have no message pump and no `Application.Run` blocking the entry thread, so if `Main` returns immediately after `Start()` the engine is torn down (or the process exits) mid-record. The MP4/AVI muxer hasn't written its trailer yet, so the file is unplayable. Symptom: 0-byte output or a truncated file that VLC opens but seeks weirdly.

**Fix**: keep the entry thread blocked while capture is running. The bundled `Program.cs` polls `Console.KeyAvailable` for ESC; the Hello-World snippet uses `await Task.Delay(...)`. Whatever you pick, `Stop()` must run to completion **before** `Dispose()` (or before the process exits) — `Stop` flushes the muxer trailer synchronously. Letting the process unwind via Ctrl-C without a console-cancel handler skips both, so wire `Console.CancelKeyPress` to call `Stop` + `Dispose` for graceful shutdown if you accept signal termination.

### 5. No video device found / capture starts but no frames flow

**Cause**: console hosts run in the same desktop session as the user that launched them — fine for an interactive console. But running under a scheduled task with "Run whether user is logged on or not" (session 0) cuts off most webcam access; the SDK enumerates zero devices or hangs on `Start`. Same trap for invocation from a Windows Service host.

**Fix**: for unattended operation either run the task only while a user is logged on (the same desktop the camera is attached to), or switch to a network source (RTSP/IP camera) which doesn't have this problem. True Windows Service hosting works for network sources only — local USB devices need a logged-on user session.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] Running the app prompts for a device, records to `%USERPROFILE%\Videos\output.mp4` (or `.avi`), and the file plays cleanly in VLC after pressing ESC.
- [ ] `Stop()` runs to completion before `Dispose()` (and before the process exits) — output file has a valid muxer trailer.
- [ ] `OnError` is wired *before* `Start()` so trial-expiry / codec-missing errors surface instead of becoming a silent no-op.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCore` instance before `Start` (otherwise the app runs in 30-day trial mode).
- [ ] `Video_Renderer.VideoRenderer = VideoRendererMode.None` is set — no preview surface in a console host.

## Bundled references

The `references/` folder is self-contained — copy both files into a fresh project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working console csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet version" line in the intro paragraph). Multi-targets `net472;net10.0-windows`.
- `references/Program.cs` — full interactive console app: device + audio enumeration, format/frame-rate selection prompts, AVI-vs-MP4 output mode, ESC-to-stop loop, `OnError` wiring with the trial `"FULL SDK"` message filtered. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence — see "License registration" above for where it goes.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - [`video-capture-sdk-net-wpf`](../video-capture-sdk-net-wpf/SKILL.md) — same SDK on WPF (with preview).
    - [`video-capture-sdk-net-winforms`](../video-capture-sdk-net-winforms/SKILL.md) — same SDK on WinForms (with preview).
    - `media-blocks-sdk-net-console` — alternative when you need a custom media pipeline rather than the high-level capture-and-record API.
