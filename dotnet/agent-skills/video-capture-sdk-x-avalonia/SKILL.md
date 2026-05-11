---
name: video-capture-sdk-x-avalonia
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into an Avalonia UI application. Covers the Avalonia-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (missing native libs, file path conventions, X11/Wayland on Linux, trial-period expiry / unlicensed build). Use when building capture apps that must run on Windows, Linux, and macOS from one codebase — for native .NET for Android/iOS/macOS use video-capture-sdk-x-{android,ios,macos}, for MAUI use video-capture-sdk-x-maui.
---

# Video Capture SDK X — Avalonia integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to an Avalonia UI application that targets **Windows, Linux, and macOS** from a single codebase. Video Capture SDK X exposes a high-level capture-and-record god-object (`VideoCaptureCoreX`) — webcam / IP camera / screen / NDI sources, optional MP4 / AVI / MOV / MPEG-TS / WebM recording, snapshots, pause/resume. Same C# code targets all three desktop OSes — only the UI host (Avalonia here) and the per-OS native redist NuGet package change between platforms.

Pinned NuGet versions: wrapper **`2026.5.4`**, Windows redists **`2026.4.29`**, macOS redist **`2025.9.1`** (matches the [official Simple Video Capture Avalonia sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/Avalonia)). The redist versions track the underlying GStreamer rebuild cadence per OS and lag the wrapper version on purpose — pin every redist to the value shipped in the upstream csproj for the wrapper version you're using; do not blindly bump.

## When to use this skill

- Adding webcam, IP camera, screen, or NDI capture to an Avalonia app that must run on Windows, Linux, and macOS from one codebase.
- Recording captured video/audio to MP4 (incl. fragmented), AVI, MOV, MPEG-TS, or WebM with a single switch between preview-only and capture-to-disk.
- Sharing capture/recording code between an Avalonia "main" app and one or more cross-platform companion apps (MAUI mobile, WPF Windows-only, console batch).
- Pause/resume recording, on-the-fly snapshot capture, audio playback during capture.

## When NOT to use this skill

- **Windows-only WPF host** (single TFM, no per-OS conditionals): use `video-capture-sdk-x-wpf`. Same SDK, same `VideoCaptureCoreX` API.
- **Native .NET-for-mobile/Apple hosts**: `video-capture-sdk-x-macos` (native Mac Catalyst), `video-capture-sdk-x-ios`, `video-capture-sdk-x-android`.
- **MAUI host** (multi-target one binary per OS, mobile-first): `video-capture-sdk-x-maui`.
- **Custom pipeline topology** (split-with-tee, multi-source mix, runtime sink swap): use `media-blocks-sdk-net-avalonia` — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without capturing): `media-player-sdk-x-avalonia`.

## NuGet packages (cross-platform layout)

Video Capture SDK X for Avalonia is **not** a single meta-package. You add the SDK plus the **Avalonia UI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed SDK (engine + `VideoCaptureCoreX` lives here, despite the package name) | Always |
| `VisioForge.DotNet.Core.UI.Avalonia` | Avalonia `VideoView` control | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` | FFmpeg/libav runtime on Windows (UPX-compressed; non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable) | `-windows` |
| `VisioForge.CrossPlatform.Core.macOS` | Native runtime on macOS | `-macos` |
| (none — uses system-wide GStreamer) | Native runtime on Linux | Linux |

> **Why `VisioForge.DotNet.MediaBlocks` and not `VisioForge.DotNet.VideoCapture`?** On the WPF host the wrapper package is `VisioForge.DotNet.VideoCapture` (which contains both the legacy `VideoCaptureCore` and the cross-platform `VideoCaptureCoreX`). The Avalonia host uses `VisioForge.DotNet.MediaBlocks` instead — it ships the same `VideoCaptureCoreX` type but without the WPF-only legacy core, and pulls in the Avalonia-friendly transitive dependencies. **Don't reference `VisioForge.DotNet.VideoCapture` from an Avalonia csproj** — it pulls Windows-only WPF dependencies that fail to restore on Linux/macOS hosts.

Linux is the odd one out — there is no NuGet redist. The Linux target uses the **system-installed GStreamer** (`gstreamer1.0`, `gstreamer1.0-plugins-base`, `gstreamer1.0-plugins-good`, `gstreamer1.0-plugins-bad`, `gstreamer1.0-plugins-ugly`, `gstreamer1.0-libav` on Debian/Ubuntu; equivalent packages on Fedora/Arch). If GStreamer is not installed system-wide on the Linux target, the app launches but `VideoCaptureCoreX.StartAsync()` fails with element-not-found errors.

## Project setup

### Multi-target csproj

This is the core of the skill — get the per-OS conditionals right and the rest follows. The csproj declares `<TargetFramework>` (singular) **conditionally per host OS**: Windows (`net10.0-windows` + `WinExe`) on Windows hosts; macOS (`net10.0-macos26.2` + `Exe`) on macOS hosts; plain `net10.0` + `Exe` on Linux. This is different from the MAUI multi-target pattern (which uses `<TargetFrameworks>` plural and emits one binary per OS) — the Avalonia sample emits **one binary per OS**, with the active TFM picked at build time by the host OS detection.

The full minimal csproj is in `references/Sample.csproj`. Adapted from the official Simple Video Capture sample (`Video Capture SDK X/Avalonia/Simple Video Capture/SimpleVideoCaptureA.csproj`). Highlights:

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
  <TargetFramework>net10.0-macos26.2</TargetFramework>
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

The Avalonia base packages (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`) are unconditional — they ship managed-only and resolve their per-OS native bits at runtime. The VisioForge wrapper + Avalonia UI packages (`VisioForge.DotNet.MediaBlocks`, `VisioForge.DotNet.Core.UI.Avalonia`) are also unconditional.

For 32-bit Windows deployment, swap `.x64` for `.x86` on both Windows redists. To support both Windows architectures from a single AnyCPU build, reference both `.x64` and `.x86` and drop `<PlatformTarget>` from the csproj.

### Avalonia entry point

`Program.cs` is the standard Avalonia bootstrap — `[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime(args)`. `App.axaml` declares `<FluentTheme />` under `<Application.Styles>`. `App.axaml.cs` instantiates `MainWindow` in `OnFrameworkInitializationCompleted`. See `references/Program.cs`, `references/App.axaml`, `references/App.axaml.cs` — all three are unmodified copies of the upstream sample and need no SDK-specific changes.

> **Note**: Avalonia uses `.axaml` (not `.xaml`) for both the markup file and its `.axaml.cs` code-behind. If you're porting from a WPF skill, rename every `.xaml` reference accordingly — the build will silently skip files with the wrong extension.

## Mandatory engine boot

Before any `VideoCaptureCoreX` instance is constructed (and before any `DeviceEnumerator` query), call `VisioForgeX.InitSDK()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on shutdown.

**Unlike the editor SDK** (`VideoEditCoreX`, which can boot lazily from its constructor), the capture SDK X requires the explicit `InitSDK` call before any `DeviceEnumerator` query — skipping it surfaces as `DllNotFoundException` or "no element X" on the very first device-enumeration await.

The upstream sample places `InitSDK()` in the `MainWindow` constructor and tears down in `Closing`:

```csharp
public MainWindow()
{
    InitializeComponent();

    // Mandatory — runs once per process. First call on a fresh machine builds
    // the GStreamer plugin registry (~2-5s); subsequent launches are instant.
    VisioForgeX.InitSDK();

    InitControls();

    Activated += MainWindow_Activated;
    Closing  += Window_Closing;

    DataContext = this;
}

private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    // Avalonia's Closing does not await async-void handlers — cancel first close,
    // run async cleanup, then call Close() once cleanup is done.
    if (_closingHandled) return;
    e.Cancel = true; _closingHandled = true;

    await VideoCapture1.StopAsync();
    await DestroyEngineAsync();             // unsubscribe + DisposeAsync + null out
    VisioForgeX.DestroySDK();
    Close();
}
```

The actual `VideoCaptureCoreX` instance is constructed in `Activated`-with-guard (because `Activated` fires on every foreground transition, and on Linux the native VideoView control is only fully parented at first activation, not at constructor time):

```csharp
private void MainWindow_Activated(object sender, EventArgs e)
{
    if (_initialized) return;
    _initialized = true;

#if __MACOS__
    // Camera permission prompt — required for capture on macOS 10.14+.
    AVFoundation.AVCaptureDevice.RequestAccessForMediaType(
        AVFoundation.AVAuthorizationMediaType.Video,
        granted => Debug.WriteLine($"Camera access: {granted}"));
#endif

    CreateEngine();   // new VideoCaptureCoreX(VideoView1) + event subscriptions
    // ...DeviceEnumerator queries, default selections, etc.
}
```

`InitSDK`-in-constructor + `CreateEngine`-in-Activated-with-guard is the canonical pattern for Avalonia + Video Capture SDK X. See `references/MainWindow.axaml.cs` for the full code.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoCapture1.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
VideoCapture1 = new VideoCaptureCoreX(VideoView1);
VideoCapture1.OnError += VideoCapture1_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await VideoCapture1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2.

The cross-platform wrinkle is **where the bytes come from**. On Windows / macOS / Linux, `File.ReadAllBytes("license.vflicense")` resolves relative to the OS-specific working directory:

- **Windows**: typically the `.exe` directory.
- **macOS**: the `.app/Contents/MacOS/` directory inside the bundle — **not** `Contents/Resources/` where you might expect Mac-style data files. Either ship the licence next to the binary, or use `AppContext.BaseDirectory` and put it under `Contents/Resources/` with a `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` entry in the csproj.
- **Linux**: the directory where `dotnet ./YourApp.dll` was launched, **not** the binary directory. Use `AppContext.BaseDirectory` to make this deterministic.

The portable approach is to embed the licence as an `EmbeddedResource` and load via `Assembly.GetManifestResourceStream(...)`. The bundled `references/MainWindow.axaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoCapture1 = new VideoCaptureCoreX(VideoView1)`.

## Hello-World capture

The minimum viable capture-and-preview snippet is six lines of SDK code on top of an Avalonia window with a `VideoView`. The full file is in `references/MainWindow.axaml.cs`; the minimum viable wiring:

```csharp
// MainWindow.axaml.cs (excerpt)
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Avalonia;
using VisioForge.Core.VideoCaptureX;

private VideoCaptureCoreX _videoCapture;

public MainWindow()
{
    InitializeComponent();
    VisioForgeX.InitSDK();         // mandatory boot
    Activated += MainWindow_Activated;
    Closing   += Window_Closing;
}

private async void MainWindow_Activated(object sender, EventArgs e)
{
    if (_initialized) return; _initialized = true;

    _videoCapture = new VideoCaptureCoreX(VideoView1);
    _videoCapture.OnError += (_, err) => Console.WriteLine($"Capture error: {err.Message}");

    var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
    if (devices.Count == 0) { Console.WriteLine("No camera"); return; }

    var device = devices[0];
    _videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = device.VideoFormats.First().ToFormat()
    };
    _videoCapture.Audio_Play = false;
    _videoCapture.Audio_Record = false;

    // Preview-only — no Outputs_Add. For recording, attach an MP4Output:
    //   _videoCapture.Outputs_Add(new MP4Output("output.mp4"), autostart: true);
    await _videoCapture.StartAsync();
}
```

`StartAsync` is awaitable — preview begins as soon as it returns. `OnError` may fire on a non-UI thread; marshal back via `Dispatcher.UIThread.Post(...)` before touching Avalonia controls (touching them off-UI throws on Linux and intermittently corrupts state on macOS).

The bundled `references/MainWindow.axaml.cs` extends this with Avalonia file pickers (`TopLevel.GetTopLevel(this).StorageProvider.SaveFilePickerAsync` — Avalonia's cross-platform replacement for `SaveFileDialog`), device-/format-/frame-rate dropdowns populated lazily from `DeviceEnumerator.Shared`, audio playback toggle, MP4 recording, pause/resume, recording-time display, and a debug log.

## Common cross-platform pitfalls

These are the cross-platform pitfalls that bite first.

### 1. `DllNotFoundException` / "no element X" on Windows or macOS

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`, **or** `VisioForgeX.InitSDK()` was not called before the first `DeviceEnumerator` query / `VideoCaptureCoreX` constructor. Common slips:

- Windows build but `VisioForge.CrossPlatform.Core.Windows.x64` was added unconditionally and no `Condition="$([MSBuild]::IsOsPlatform('Windows'))"` wraps it — works on Windows, but `dotnet build` on a macOS / Linux host fails NuGet restore because the Windows redist has no macOS / Linux RID.
- Wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29` Windows / `2025.9.1` macOS).
- `VisioForgeX.InitSDK()` placed after `new VideoCaptureCoreX(...)` instead of before it.

**Fix**: cross-check against `references/Sample.csproj` — every per-OS `ItemGroup` needs the matching `Condition`. Pin every redist to the value shipped in the upstream csproj for your wrapper version. Confirm `InitSDK` is the very first SDK call in `MainWindow`'s constructor.

### 2. Linux: app launches but `StartAsync()` errors with "no element X"

**Cause**: GStreamer is not installed system-wide. There is no NuGet redist for Linux; the Linux build relies on the system package manager.

**Fix**: install the GStreamer base + plugin packages for the distro:

- Debian / Ubuntu: `sudo apt install gstreamer1.0-tools gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav`
- Fedora: `sudo dnf install gstreamer1 gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free gstreamer1-libav`
- Arch: `sudo pacman -S gstreamer gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav`

`gst-inspect-1.0 --version` should print 1.18 or newer.

### 3. Linux: no devices on Wayland; no audio under PulseAudio container

**Cause**: GStreamer's video / audio source elements need access to the underlying display server and audio daemon.

- **X11 vs Wayland**: `v4l2src` (USB cameras) works under both, but screen capture (`ximagesrc`) is X11-only. Under a pure Wayland session, screen capture needs PipeWire (`pipewiresrc`, `gstreamer1.0-plugins-bad` ≥ 1.20) plus a portal-aware desktop. If you're capturing the screen, force-prefer X11 by launching with `XDG_SESSION_TYPE=x11` or accept a Wayland-only PipeWire path.
- **ALSA vs PulseAudio**: `pulsesrc`/`pulsesink` is the default on most desktops; it falls through to ALSA when PulseAudio isn't running. Inside a sandboxed container (Flatpak, Snap, Docker without `--device /dev/snd`) audio devices may be invisible to `DeviceEnumerator.Shared.AudioSourcesAsync()` — fix the container's audio passthrough rather than the SDK.

**Fix**: verify with the GStreamer probes before blaming the SDK:

```bash
# does the source element exist?
gst-inspect-1.0 v4l2src
# can the engine see the camera?
gst-device-monitor-1.0 Video/Source
# can it see the microphone?
gst-device-monitor-1.0 Audio/Source
```

If the device-monitor output is empty, the SDK's enumerator will be empty too — the issue is below the SDK.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on `StartAsync()`

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await VideoCapture1.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every `VideoCaptureCoreX` instance in the process needs its own call.

### 5. macOS: camera prompt never appears / capture fails silently

**Cause**: macOS 10.14+ requires explicit user consent (`NSCameraUsageDescription` in `Info.plist`) and an `AVCaptureDevice.RequestAccessForMediaType` call before the first capture. Without consent, `DeviceEnumerator` returns the camera but `StartAsync` produces a black frame and no error.

**Fix**: add the usage-description key to the `.app` bundle's `Info.plist` and call `AVFoundation.AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted => {...})` once during `Activated`. The bundled `references/MainWindow.axaml.cs` shows the pattern under `#if __MACOS__`.

### 6. macOS: `File.ReadAllBytes("license.vflicense")` throws `FileNotFoundException`

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

### 7. UI freezes or throws when `OnError` / progress callbacks fire

**Cause**: `OnError` and the snapshot/duration helpers fire on a non-UI thread. Touching `mmLog.Items`, `lbTimestamp.Text`, etc. from those handlers throws on Linux (X11 is single-threaded) and intermittently corrupts state on macOS.

**Fix**: wrap UI mutations in `Dispatcher.UIThread.Post(...)` or `Dispatcher.UIThread.InvokeAsync(...)`. The bundled `references/MainWindow.axaml.cs` uses `Dispatcher.UIThread.Post` inside `VideoCapture1_OnError` and `Dispatcher.UIThread.InvokeAsync` inside `UpdateRecordingTimeAsync`.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds on each target OS with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] On Linux: `gst-inspect-1.0 --version` reports 1.18 or newer; the GStreamer base + good + bad + ugly + libav plugin sets are installed; `gst-device-monitor-1.0 Video/Source` lists the camera.
- [ ] On macOS: `Info.plist` contains `NSCameraUsageDescription` (and `NSMicrophoneUsageDescription` if recording audio); first launch shows the permission prompt.
- [ ] First run shows the SDK-version suffix in the window title once `Activated` fires; preview appears within ~1 s on subsequent launches (the GStreamer registry was built on the first run).
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCoreX` (`StopAsync` then re-`StartAsync` works, and `Window_Closing` runs `StopAsync → DestroyEngineAsync → DestroySDK` in that order).
- [ ] If recording to MP4 (Capture mode + `Outputs_Add(new MP4Output(...))`): output file is finalised correctly when the app exits cleanly (verify it plays back in another player).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh Avalonia project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target Avalonia csproj with all per-OS conditional package references (Windows + macOS NuGet redists; Linux uses system GStreamer).
- `references/Program.cs` — Avalonia entry point (`[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime`).
- `references/App.axaml` + `references/App.axaml.cs` — Avalonia `Application` with `<FluentTheme />` and `MainWindow` instantiation.
- `references/MainWindow.axaml` — Avalonia XAML with device-/format-/frame-rate dropdowns, output-path picker, debug log, Preview/Capture radio toggle, Start/Stop/Pause/Resume buttons, recording-time text, and the `VideoView` preview panel. Declares `xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"` for the `VideoView` namespace.
- `references/MainWindow.axaml.cs` — full code-behind: `InitSDK`-in-constructor + `Activated`-with-guard `CreateEngine`, `DestroyEngineAsync` + `VisioForgeX.DestroySDK` on close, `TopLevel.StorageProvider`-based file pickers, MP4 recording with `Outputs_Add`, pause/resume, snapshot capture, `OnError` wired through `Dispatcher.UIThread.Post`, and the macOS camera-permission `#if __MACOS__` block. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-wpf` — same SDK, Windows-only WPF host with a single TFM (no per-OS conditionals).
    - `video-capture-sdk-x-macos` — same SDK, native .NET-for-macOS (Mac Catalyst) host without Avalonia.
    - `video-capture-sdk-x-winui` — same SDK, WinUI 3 / Windows App SDK host.
    - `video-edit-sdk-x-avalonia` — non-linear editor sibling (`VideoEditCoreX`) for the same Avalonia host.
    - `video-capture-sdk-x-maui` — same SDK on .NET MAUI (mobile-first multi-target).
    - `video-capture-sdk-x-android` / `video-capture-sdk-x-ios` — native mobile hosts.
    - `media-blocks-sdk-net-avalonia` — same engine, lower-level graph-based API.
    - `media-player-sdk-x-avalonia` — high-level playback API.
