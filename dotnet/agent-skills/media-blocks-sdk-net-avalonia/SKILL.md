---
name: media-blocks-sdk-net-avalonia
description: Integrate VisioForge Media Blocks SDK into an Avalonia UI application. Covers the graph-based pipeline model (MediaBlocksPipeline, source/sink/transform blocks), multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (missing native libs, file path conventions, X11/Wayland on Linux, ALSA/PulseAudio audio, trial-period expiry / unlicensed build). Use when building custom media pipelines (capture, transcode, mix, stream, record) on Windows, Linux, and macOS — for native iOS/Android/macOS use media-blocks-sdk-net-{android,ios,macos}, for MAUI use media-blocks-sdk-net-maui.
---

# Media Blocks SDK .Net — Avalonia integration

This skill helps you add **VisioForge Media Blocks SDK .Net** to an Avalonia UI application that targets **Windows, Linux, and macOS** from a single codebase. Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4OutputBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring them with `pipeline.Connect(producer, consumer)`, then calling `await pipeline.StartAsync()`. Compared to the higher-level Video Capture SDK X (a single `VideoCaptureCoreX` god-object), Media Blocks gives you full control over the topology — splitting streams with tees, mixing sources, transcoding without preview, swapping sinks at runtime — at the cost of having to wire every edge yourself.

Pinned NuGet versions: wrapper **`2026.5.4`**, Windows redists **`2026.4.29`**, macOS redist **`2025.9.1`** (matches the [official Simple Video Capture Avalonia sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Avalonia)). Newer 2026.x.x patch versions of the wrapper are drop-in compatible — but the redist versions track the underlying GStreamer/libav rebuild cadence per OS and lag the wrapper on purpose. Pin every redist to the value shipped in the upstream csproj for the wrapper version you're using; do not blindly bump.

## When to use this skill

- Building a **custom pipeline** that the high-level capture/playback SDKs don't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capturing → encoding → muxing to MP4 / MPEG-TS / WebM with explicit control over each stage, on Windows, Linux, and macOS from one codebase.
- Network streaming sinks (RTSP server, SRT, RIST, NDI, WebRTC WHIP, YouTube/Facebook RTMP) wired into a custom graph that must run cross-platform.
- Sharing pipeline construction code between an Avalonia "main" app and one or more cross-platform companion apps (MAUI mobile, WPF Windows-only, console batch).
- Pipelines you want to inspect — `pipeline.GetDiagramAsImage()` returns a SkiaSharp bitmap of the live graph.

## When NOT to use this skill

- **Plain webcam capture** (preview + record to MP4, no custom topology): `video-capture-sdk-x-avalonia` is dramatically less code. `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play a file or network stream, no capture / no custom graph): `media-player-sdk-x-avalonia`.
- **Windows-only WPF host** (single TFM, no per-OS conditionals): `media-blocks-sdk-net-wpf`. Same engine, same block API.
- **Native .NET-for-mobile/Apple hosts**: `media-blocks-sdk-net-android`, `media-blocks-sdk-net-ios`, `media-blocks-sdk-net-macos`.
- **MAUI host** (multi-target one binary per OS, mobile-first): `media-blocks-sdk-net-maui`.
- **Headless / server-side** (no UI shell, daemon process): `media-blocks-sdk-net-console`.

## NuGet packages (cross-platform layout)

Media Blocks SDK for Avalonia is **not** a single meta-package. You add the SDK plus the **Avalonia UI handler package**, then per-OS native runtime redists conditionally:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed SDK (engine + all blocks) | Always |
| `VisioForge.DotNet.Core.UI.Avalonia` | Avalonia `VideoView` control | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` | FFmpeg/libav runtime on Windows (UPX-compressed; non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable) | `-windows` |
| `VisioForge.CrossPlatform.Core.macOS` | Native runtime on macOS (libav is bundled inside Core on macOS — no separate libav redist) | `-macos` |
| (none — uses system-wide GStreamer) | Native runtime on Linux | Linux |

Linux is the odd one out — there is no NuGet redist. The Linux target uses the **system-installed GStreamer** (`gstreamer1.0`, `gstreamer1.0-plugins-base`, `gstreamer1.0-plugins-good`, `gstreamer1.0-plugins-bad`, `gstreamer1.0-plugins-ugly`, `gstreamer1.0-libav` on Debian/Ubuntu; equivalent packages on Fedora/Arch). If GStreamer is not installed system-wide on the Linux target, the app launches but `pipeline.StartAsync()` errors with `"no element X"` for every block in your graph.

> **Note on Windows redists vs WPF host**: the Avalonia layout requires both Core and Libav redists on Windows for any pipeline that mux es to MP4/MPEG-TS/WebM (most realistic graphs). They're declared explicitly in the conditional `<ItemGroup>` — don't rely on transitive resolution.

## Project setup

### Multi-target csproj

This is the core of the skill — get the per-OS conditionals right and the rest follows. The csproj declares `<TargetFramework>` (singular) **conditionally per host OS**: Windows (`net10.0-windows` + `WinExe`) on Windows hosts; macOS (`net10.0-macos` + `Exe`) on macOS hosts; plain `net10.0` + `Exe` on Linux. This is different from the MAUI multi-target pattern (which uses `<TargetFrameworks>` plural and emits one binary per OS) — the Avalonia sample emits **one binary per OS**, with the active TFM picked at build time by the host OS detection.

The full minimal csproj is in `references/Sample.csproj`. Adapted from the official Simple Video Capture sample (`Media Blocks SDK/Avalonia/Simple Video Capture/SimpleVideoCaptureAMB.csproj`). Highlights:

```xml
<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <TargetFramework>net10.0-windows</TargetFramework>
  <OutputType>WinExe</OutputType>
</PropertyGroup>
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('Windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2026.4.29" />
</ItemGroup>

<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <TargetFramework>net10.0-macos</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
<ItemGroup Condition="$([MSBuild]::IsOsPlatform('OSX'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>

<PropertyGroup Condition="$([MSBuild]::IsOsPlatform('Linux'))">
  <TargetFramework>net10.0</TargetFramework>
  <OutputType>Exe</OutputType>
</PropertyGroup>
```

Avalonia base packages (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`) are unconditional — they ship managed-only and resolve their per-OS native bits at runtime. The VisioForge wrapper + Avalonia UI packages (`VisioForge.DotNet.MediaBlocks`, `VisioForge.DotNet.Core.UI.Avalonia`) are also unconditional.

For 32-bit Windows deployment, swap `.x64` for `.x86` on both Windows redists. To support both Windows architectures from a single AnyCPU build, reference both `.x64` and `.x86` and drop `<PlatformTarget>` from the csproj.

### Avalonia entry point

`Program.cs` is the standard Avalonia bootstrap — `[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime(args)`. `App.axaml` declares `<FluentTheme />` under `<Application.Styles>`. `App.axaml.cs` instantiates `MainWindow` in `OnFrameworkInitializationCompleted`. See `references/Program.cs`, `references/App.axaml`, `references/App.axaml.cs` — all three are unmodified copies of the upstream sample and need no SDK-specific changes.

> **Note**: Avalonia uses `.axaml` (not `.xaml`) for both the markup file and its `.axaml.cs` code-behind. If you're porting from the WPF Media Blocks skill, rename every `.xaml` reference accordingly — the build will silently skip files with the wrong extension.

## Pipeline model

This is the core mental shift from Video Capture SDK X. There is no `VideoCaptureCoreX.StartAsync()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `VPXEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, `AudioMixerBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** — terminate the graph. **Renderer sinks** drive the UI / speakers (`VideoRendererBlock` bound to an Avalonia `VideoView`, `AudioRendererBlock` bound to a system audio output). **File / network sinks** write/transmit (`MP4OutputBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, …). Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — the convenient `pipeline.Connect(producer, sink)` overload creates the right input pad for you.
5. **Connections** — `pipeline.Connect(producer, consumer)`. Connections must be made before `StartAsync`; reconnecting at runtime is supported by specific blocks (live-source-switch, bridge) but not the general case.

Topology example (Simple Video Capture demo, video preview + MP4 record):

```text
SystemVideoSourceBlock  --video-->  TeeBlock(video)  --[0]-->  VideoRendererBlock (VideoView1)
                                                     --[1]-->  MP4OutputBlock (video pad)
SystemAudioSourceBlock  --audio-->  TeeBlock(audio)  --[0]-->  AudioRendererBlock (speakers)
                                                     --[1]-->  MP4OutputBlock (audio pad)
```

`MP4OutputBlock` is the same instance on both audio and video paths — its dynamic inputs collect the streams to mux.

## Mandatory engine boot

Before any block construction or `DeviceEnumerator` query, call `VisioForgeX.InitSDK()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown.

The upstream sample places `InitSDK()` in the `MainWindow` constructor; heavy work — device enumeration, populating combo boxes — is deferred to `Activated` with an `_initialized` guard, because `Activated` fires on every foreground transition, and on Linux the native `VideoView` is only fully parented at first activation, not at constructor time. The actual `MediaBlocksPipeline` is constructed even later, in the Start-button click handler — that way the user controls when the graph runs and the `VideoView`'s native handle is guaranteed to exist.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before any `Connect()` / `StartAsync()`:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += Pipeline_OnError;

var cert = System.IO.File.ReadAllBytes(
    Path.Combine(AppContext.BaseDirectory, "your.vflicense"));
await _pipeline.SetLicenseCertificateAsync(cert);

// ...now build the graph and StartAsync.
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2.

**Where the bytes come from** is the cross-platform wrinkle. The portable approach is `Path.Combine(AppContext.BaseDirectory, "license.vflicense")` plus `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` in the csproj — that resolves correctly inside an `.app` bundle on macOS (where the working directory is `.app/Contents/MacOS/`, not the launch directory) and avoids working-directory-vs-binary-directory ambiguity on Linux. Alternatively, embed the licence as an `EmbeddedResource` and load via `Assembly.GetManifestResourceStream(...)`. See pitfall #8 for the macOS-specific `FileNotFoundException` symptom.

The bundled `references/MainWindow.axaml.cs` runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call) — the comment marker in `btStart_Click` shows exactly where to drop the two lines. Multi-pipeline apps need one call per `MediaBlocksPipeline` instance.

## Hello-World pipeline

The minimum viable preview-only pipeline is six lines of SDK code on top of an Avalonia window with a `<avalonia:VideoView x:Name="VideoView1" />`. The wiring in the Start-button click handler:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += (_, args) =>
    Dispatcher.UIThread.Post(() => Console.WriteLine(args.Message));

// For a purchased licence, add these two lines here:
//   var cert = System.IO.File.ReadAllBytes(
//       Path.Combine(AppContext.BaseDirectory, "your.vflicense"));
//   await _pipeline.SetLicenseCertificateAsync(cert);

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
if (devices.Length == 0) return;

_videoSource   = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(devices[0]));
_videoRenderer = new VideoRendererBlock(_pipeline, this.FindControl<VideoView>("VideoView1"));

_pipeline.Connect(_videoSource, _videoRenderer);
await _pipeline.StartAsync();
```

`async-void` event handlers must `try/catch` — an exception otherwise escapes to `AppDomain.UnhandledException` and silently terminates the app on Linux. `OnError` may fire on a non-UI thread; marshal back via `Dispatcher.UIThread.Post(...)` before touching Avalonia controls (touching them off-UI throws on Linux and intermittently corrupts state on macOS).

`references/MainWindow.axaml.cs` (paired with `MainWindow.axaml`) is the full upstream sample — it adds device / format / frame-rate dropdowns, the audio path, the tee + MP4 muxer record path, recording-time display, Avalonia file pickers (`TopLevel.GetTopLevel(this).StorageProvider.SaveFilePickerAsync` — Avalonia's cross-platform replacement for `SaveFileDialog`), pause/resume, and `OnError` wired through `Dispatcher.UIThread`. Use it as a copy-paste starting template when you outgrow the snippet above.

## Common cross-platform pitfalls

These are the cross-platform pitfalls that bite first.

### 1. `DllNotFoundException` / "no element X" on Windows or macOS

**Cause**: the matching per-OS native runtime package is missing or mismatched, **or** `VisioForgeX.InitSDK()` was not called before the first block construction / `DeviceEnumerator` query. Common slips:

- Windows redist added unconditionally (no `Condition="$([MSBuild]::IsOsPlatform('Windows'))"`) — works on Windows but breaks NuGet restore on a macOS / Linux host because the Windows redist has no matching RID.
- Windows pipeline mux es to MP4 / MPEG-TS / WebM but only `VisioForge.CrossPlatform.Core.Windows.x64` is referenced — the libav redist is a separate NuGet and is what most "no element" errors trace back to.
- Wrapper / redist version drift (wrapper `2026.5.4` paired with Windows redist `2026.5.x` instead of `2026.4.29`, or macOS redist `2026.x.x` instead of `2025.9.1`).
- `VisioForgeX.InitSDK()` placed after `new MediaBlocksPipeline()` instead of before it.

**Fix**: cross-check against `references/Sample.csproj`. On Windows reference both Core and Libav. Pin every redist to the upstream-sample value for your wrapper version. Confirm `InitSDK` is the very first SDK call in `MainWindow`'s constructor.

### 2. Linux: app launches but `StartAsync()` errors with "no element X" / "Element 'h264parse' not found"

**Cause**: GStreamer is not installed system-wide, or the specific plugin family the block needs is missing. There is no NuGet redist for Linux; the Linux build relies on the system package manager. `H264EncoderBlock`, `MP4OutputBlock`, `WebMSinkBlock`, etc. each pull from a different plugin set — `gstreamer1.0-libav` is what most "missing element" errors trace back to.

**Fix**: install the GStreamer base + plugin packages for the distro:

- Debian / Ubuntu: `sudo apt install gstreamer1.0-tools gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav`
- Fedora: `sudo dnf install gstreamer1 gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free gstreamer1-libav`
- Arch: `sudo pacman -S gstreamer gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav`

`gst-inspect-1.0 --version` should print 1.18 or newer. Verify the specific element your graph needs: `gst-inspect-1.0 h264parse`, `gst-inspect-1.0 avenc_aac`, etc.

### 3. Linux: no devices on Wayland; no audio under PulseAudio container

**Cause**: GStreamer's video / audio source elements need access to the underlying display server and audio daemon.

- **X11 vs Wayland**: `v4l2src` (USB cameras) works under both, but screen-capture (`ximagesrc`) is X11-only. Under a pure Wayland session, screen capture needs PipeWire (`pipewiresrc`, `gstreamer1.0-plugins-bad` ≥ 1.20) plus a portal-aware desktop. If your graph uses `ScreenSourceBlock`, force-prefer X11 by launching with `XDG_SESSION_TYPE=x11` or accept a Wayland-only PipeWire path.
- **ALSA vs PulseAudio**: `pulsesrc`/`pulsesink` is the default on most desktops; it falls through to ALSA when PulseAudio isn't running. Inside a sandboxed container (Flatpak, Snap, Docker without `--device /dev/snd`) audio devices may be invisible to `DeviceEnumerator.Shared.AudioSourcesAsync()` — fix the container's audio passthrough rather than the SDK.

**Fix**: verify with the GStreamer probes before blaming the SDK:

```bash
gst-inspect-1.0 v4l2src                 # does the source element exist?
gst-device-monitor-1.0 Video/Source     # can the engine see the camera?
gst-device-monitor-1.0 Audio/Source     # can it see the microphone?
```

If the device-monitor output is empty, the SDK's enumerator will be empty too — the issue is below the SDK.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on `StartAsync()`

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` after the constructor and before any `Connect()` / `StartAsync()` (see "License registration" above). Every `MediaBlocksPipeline` instance in the process needs its own call.

### 5. macOS: camera prompt never appears / capture fails silently

**Cause**: macOS 10.14+ requires explicit user consent (`NSCameraUsageDescription` in `Info.plist`) and an `AVCaptureDevice.RequestAccessForMediaType` call before the first capture. Without consent, `DeviceEnumerator` returns the camera but `StartAsync` produces a black frame and no error.

**Fix**: add the usage-description key to the `.app` bundle's `Info.plist` (and `NSMicrophoneUsageDescription` if you record audio) and call `AVFoundation.AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted => {...})` once during `Activated`. The bundled `references/MainWindow.axaml.cs` shows the pattern under `#if __MACOS__`.

### 6. `await pipeline.StartAsync()` deadlocks / preview is black on first frame

**Cause A — UI thread blocking**: calling synchronous `_pipeline.Stop()` or `Dispose()` from the UI thread while a renderer block is still pumping frames to a `VideoView` deadlocks because the renderer needs the UI thread to release the last frame. Always use the async forms.

**Cause B — VideoView has no native handle yet**: `VideoRendererBlock` was constructed and `pipeline.StartAsync` ran before the Avalonia `VideoView1` had a valid native handle. Most often happens when start logic runs from the constructor or `Loaded`-handler too early.

**Fix**: use `await _pipeline.StopAsync()` everywhere, defer pipeline construction and start to a button click (the upstream sample uses an explicit Start button — copy that pattern), and always create `VideoRendererBlock` *after* the `VideoView` has been activated at least once (i.e. after `MainWindow_Activated` has fired).

### 7. UI freezes or throws when `OnError` / progress callbacks fire

**Cause**: `OnError` and the `Position_GetAsync` continuations fire on a non-UI thread. Touching `edLog.Text`, `lbTimestamp.Text`, etc. from those handlers throws on Linux (X11 is single-threaded) and intermittently corrupts state on macOS.

**Fix**: wrap UI mutations in `Dispatcher.UIThread.Post(...)` or `Dispatcher.UIThread.InvokeAsync(...)`. The bundled `references/MainWindow.axaml.cs` uses `Dispatcher.UIThread.InvokeAsync` inside `Log()` and `UpdateRecordingTimeAsync`.

### 8. macOS: `File.ReadAllBytes("license.vflicense")` throws `FileNotFoundException`

**Cause**: the working directory inside an `.app` bundle on macOS is `.app/Contents/MacOS/`, not the directory you launched from. Relative paths to data files outside that directory don't resolve.

**Fix**: use `Path.Combine(AppContext.BaseDirectory, "license.vflicense")` and copy the licence to the output via the csproj:

```xml
<ItemGroup>
  <None Include="license.vflicense">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

The same pattern works on Linux (avoids working-directory-vs-binary-directory ambiguity) and on Windows (no-op — they're already the same).

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds on each target OS with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] On Linux: `gst-inspect-1.0 --version` reports 1.18 or newer; the GStreamer base + good + bad + ugly + libav plugin sets are installed; `gst-device-monitor-1.0 Video/Source` lists the camera; the specific elements your graph uses (`gst-inspect-1.0 h264parse`, `gst-inspect-1.0 avenc_aac`) all resolve.
- [ ] On macOS: `Info.plist` contains `NSCameraUsageDescription` (and `NSMicrophoneUsageDescription` if recording audio); first launch shows the permission prompt.
- [ ] First run on a fresh machine takes 2-5 s during `VisioForgeX.InitSDK()` (registry build); second run is instant.
- [ ] Webcam preview appears within ~1 s after clicking Start on a machine with a real or virtual webcam.
- [ ] Stopping and restarting the pipeline does not leak — always `await _pipeline.StopAsync()` then `_pipeline.Dispose()` before reuse.
- [ ] On clean shutdown, `Window_Closing` runs `StopAsync → Dispose → VisioForgeX.DestroySDK()` in that order.
- [ ] If recording to a file sink: output file is finalised correctly when the app exits cleanly (`StopAsync` runs to completion before `Dispose`); verify it plays back in another player.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh Avalonia project folder and `dotnet build` succeeds on each host OS with no extra files needed:

- `references/Sample.csproj` — multi-target Avalonia csproj with all per-OS conditional package references (Windows + macOS NuGet redists; Linux uses system GStreamer).
- `references/Program.cs` — Avalonia entry point (`[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime`).
- `references/App.axaml` + `references/App.axaml.cs` — Avalonia `Application` with `<FluentTheme />` and `MainWindow` instantiation.
- `references/MainWindow.axaml` — Avalonia XAML with device / format / frame-rate dropdowns, output-path picker, debug log, Preview/Capture radio toggle, Start/Stop/Pause/Resume buttons, recording-time text, and the `VideoView` preview panel. Declares `xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"` for the `VideoView` namespace.
- `references/MainWindow.axaml.cs` — full code-behind: `InitSDK`-in-constructor + `Activated`-with-guard for `DeviceEnumerator` queries, `MediaBlocksPipeline` constructed in the Start-button click handler with explicit graph wiring (preview-only and tee+MP4 capture topologies), `TopLevel.StorageProvider`-based file picker, pause/resume, recording-time display via `Position_GetAsync`, `OnError` wired through `Dispatcher.UIThread.InvokeAsync`, and the macOS camera-permission `#if __MACOS__` block. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence — the comment marker in `btStart_Click` shows where.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-wpf` — same SDK on Windows-only WPF, single TFM, no per-OS conditionals.
    - `video-capture-sdk-x-avalonia` — high-level capture API on the same Avalonia host; use this if you don't need a custom pipeline.
    - `video-edit-sdk-x-avalonia` — non-linear editor sibling (`VideoEditCoreX`) on the same Avalonia host.
    - `media-player-sdk-x-avalonia` — high-level playback API on the same Avalonia host.
    - `media-blocks-sdk-net-winforms` — same SDK on WinForms.
    - `media-blocks-sdk-net-maui` — same SDK on MAUI (mobile-first multi-target).
    - `media-blocks-sdk-net-uno` — same SDK on Uno (Windows + macOS + iOS + Android + Linux + browser).
    - `media-blocks-sdk-net-android` / `media-blocks-sdk-net-ios` / `media-blocks-sdk-net-macos` — native mobile / Apple hosts.
