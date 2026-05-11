---
name: video-edit-sdk-x-avalonia
description: Integrate VisioForge Video Edit SDK X (cross-platform editor edition) into an Avalonia UI application. Covers the timeline model, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (missing native libs, file path conventions, trial-period expiry / unlicensed build). Use when building a non-linear editor that must run on Windows, Linux, and macOS from one codebase — for WPF / WinForms hosts use video-edit-sdk-x-{wpf,winforms}, for headless batch use video-edit-sdk-x-console.
---

# Video Edit SDK X — Avalonia integration

This skill helps you add **VisioForge Video Edit SDK X** — the cross-platform "X" edition of the editor SDK — to an Avalonia UI application that targets **Windows, Linux, and macOS** from a single codebase. Video Edit SDK X exposes a high-level non-linear editor god-object (`VideoEditCoreX`) with a list-of-clips timeline (`Input_AddAudioVideoFile`, `Input_AddImageFile`, `Input_AddAudioFile`) and a single configurable `Output_Format` sink. Same C# code targets all three desktop OSes — only the UI host (Avalonia here) and the per-OS native redist NuGet package change between platforms.

Pinned NuGet versions: wrapper **`2026.5.4`**, Windows redists **`2026.4.29`**, macOS redist **`2025.9.1`** (matches the [official VideoJoin Avalonia sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/Avalonia)). The redist versions track the underlying GStreamer rebuild cadence per OS and lag the wrapper version on purpose — pin every redist to the value shipped in the upstream csproj for the wrapper version you're using; do not blindly bump.

## When to use this skill

- Building a non-linear video/audio editor in **Avalonia** that must run on Windows, Linux, and macOS from a single codebase.
- Joining / concatenating multiple media files (video + audio + image clips) into one output (MP4, WebM, AVI, MKV, WMV, MP3, M4A, WMA, OGG Vorbis, FLAC, Speex, WAV).
- Sharing edit/render code between an Avalonia "main" app and one or more cross-platform companion apps (MAUI mobile, WPF Windows-only, console batch).
- Time-trimmed inputs with optional resize/frame-rate normalisation across heterogeneous source files.

## When NOT to use this skill

- **Windows-only WPF editor**: `video-edit-sdk-x-wpf` drops the per-OS conditionals and uses a single TFM. `video-edit-sdk-x-winforms` does the same on WinForms.
- **Headless / batch transcoding** (no UI shell, server-side rendering): `video-edit-sdk-x-console`.
- **Capture / recording instead of editing** (webcam → file): `video-capture-sdk-x-avalonia` — `VideoCaptureCoreX` is the capture sibling of `VideoEditCoreX`.
- **Custom pipeline topology** (split-with-tee, multi-source mix, runtime sink swap): use `media-blocks-sdk-net-avalonia` — `VideoEditCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without editing): `media-player-sdk-x-avalonia`.

## NuGet packages (cross-platform layout)

Video Edit SDK X for Avalonia is **not** a single meta-package. You add the SDK plus the **Avalonia UI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed SDK (engine + `VideoEditCoreX` lives here, despite the package name) | Always |
| `VisioForge.DotNet.Core.UI.Avalonia` | Avalonia `VideoView` control | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64.UPX` | FFmpeg/libav runtime on Windows (UPX-compressed; non-UPX `VisioForge.CrossPlatform.Libav.Windows.x64` is interchangeable) | `-windows` |
| `VisioForge.CrossPlatform.Core.macOS` | Native runtime on macOS | `-macos` |
| (none — uses system-wide GStreamer) | Native runtime on Linux | Linux |

Linux is the odd one out — there is no NuGet redist. The Linux target uses the **system-installed GStreamer** (`gstreamer1.0`, `gstreamer1.0-plugins-base`, `gstreamer1.0-plugins-good`, `gstreamer1.0-plugins-bad`, `gstreamer1.0-plugins-ugly`, `gstreamer1.0-libav` on Debian/Ubuntu; equivalent packages on Fedora/Arch). If GStreamer is not installed system-wide on the Linux target, the app launches but `VideoEditCoreX` operations fail with element-not-found errors at the first `Start()` call.

## Project setup

### Multi-target csproj

This is the core of the skill — get the per-OS conditionals right and the rest follows. The csproj declares `<TargetFramework>` (singular) **conditionally per host OS**: Windows (`net10.0-windows` + `WinExe`) on Windows hosts; macOS (`net10.0-macos` + `Exe`) on macOS hosts; plain `net10.0` + `Exe` on Linux. This is different from the MAUI multi-target pattern (which uses `<TargetFrameworks>` plural and emits one binary per OS) — the Avalonia sample emits **one binary per OS**, with the active TFM picked at build time by the host OS detection.

The full minimal csproj is in `references/Sample.csproj`. Adapted from the official VideoJoin sample (`Video Edit SDK X/Avalonia/VideoJoin/VideoJoinA.csproj`). Highlights:

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

The Avalonia base packages (`Avalonia`, `Avalonia.Desktop`, `Avalonia.Themes.Fluent`, `Avalonia.Fonts.Inter`) are unconditional — they ship managed-only and resolve their per-OS native bits at runtime. The VisioForge wrapper + Avalonia UI packages (`VisioForge.DotNet.MediaBlocks`, `VisioForge.DotNet.Core.UI.Avalonia`) are also unconditional.

For 32-bit Windows deployment, swap `.x64` for `.x86` on both Windows redists. To support both Windows architectures from a single AnyCPU build, reference both `.x64` and `.x86` and drop `<PlatformTarget>` from the csproj.

### Avalonia entry point

`Program.cs` is the standard Avalonia bootstrap — `[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime(args)`. `App.axaml` declares `<FluentTheme />` under `<Application.Styles>`. `App.axaml.cs` instantiates `MainWindow` in `OnFrameworkInitializationCompleted`. See `references/Program.cs`, `references/App.axaml`, `references/App.axaml.cs` — all three are unmodified copies of the upstream sample and need no SDK-specific changes.

> **Note**: Avalonia uses `.axaml` (not `.xaml`) for both the markup file and its `.axaml.cs` code-behind. If you're porting from a WPF skill, rename every `.xaml` reference accordingly — the build will silently skip files with the wrong extension.

## Engine boot and lifecycle

`VideoEditCoreX` is the god-object. **Unlike the capture SDK X**, the editor sample does not call `await VisioForgeX.InitSDKAsync()` before constructing — `VideoEditCoreX`'s constructor performs the necessary initialisation lazily. You **must** still call `VisioForgeX.DestroySDK()` on shutdown (after `VideoEdit1.Stop()` and `VideoEdit1.Dispose()`); skipping it leaks native handles and can prevent clean process exit on Windows.

The upstream sample does its initialisation in the `MainWindow.Activated` handler with an `_initialized` guard (because `Activated` fires every time the window comes back to the foreground), and tears down in `Closing`:

```csharp
// MainWindow.axaml.cs
public MainWindow()
{
    Activated += MainWindow_Activated;
    Closing  += MainWindow_Closing;
    DataContext = this;
    InitializeComponent();
}

private void MainWindow_Activated(object sender, EventArgs e)
{
    if (_initialized) return;
    _initialized = true;
    InitControls();
    CreateEngine();          // new VideoEditCoreX() + event subscriptions
    Title += $" (SDK v{VideoEditCoreX.SDK_Version})";
}

private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
{
    VideoEdit1.Stop();
    DestroyEngine();         // unsubscribe + Dispose() + null out
    VisioForgeX.DestroySDK();
}
```

`Activated`-with-guard is the canonical pattern for Avalonia + VisioForge. Doing it in the constructor instead works on Windows/macOS but races with native-control parenting on Linux. See `references/MainWindow.axaml.cs` for the full code.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoEdit1.SetLicenseCertificateAsync(certBytes)` on every `VideoEditCoreX` instance, after the constructor and before `Start()`:

```csharp
VideoEdit1 = new VideoEditCoreX();
VideoEdit1.OnError += VideoEdit1_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await VideoEdit1.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2.

The cross-platform wrinkle is **where the bytes come from**. On Windows / macOS / Linux, `File.ReadAllBytes("license.vflicense")` resolves relative to the OS-specific working directory:

- **Windows**: typically the `.exe` directory.
- **macOS**: the `.app/Contents/MacOS/` directory inside the bundle — **not** `Contents/Resources/` where you might expect Mac-style data files. Either ship the licence next to the binary, or use `AppContext.BaseDirectory` and put it under `Contents/Resources/` with a `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` entry in the csproj.
- **Linux**: the directory where `dotnet ./YourApp.dll` was launched, **not** the binary directory. Use `AppContext.BaseDirectory` to make this deterministic.

The portable approach is to embed the licence as an `EmbeddedResource` and load via `Assembly.GetManifestResourceStream(...)`. The bundled `references/MainWindow.axaml.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `CreateEngine()` right after `VideoEdit1 = new VideoEditCoreX()`.

## Hello-World render

A minimal join-and-render — three input files concatenated to MP4 — is six lines of SDK code. The full file is in `references/MainWindow.axaml.cs`; the minimum viable wiring:

```csharp
// MainWindow.axaml.cs
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.VideoEditX;

private VideoEditCoreX _edit;

private void StartJoin()
{
    _edit = new VideoEditCoreX();
    _edit.OnError    += (s, e) => Console.WriteLine($"Edit error: {e.Message}");
    _edit.OnProgress += (s, e) => Dispatcher.UIThread.Post(() => pbProgress.Value = e.Progress);
    _edit.OnStop     += (s, e) => Dispatcher.UIThread.Post(() =>
        Console.WriteLine(e.Successful ? "Done" : "Stopped with error"));

    // Add inputs in render order. Image clips need an explicit duration;
    // audio/video clips read their own duration from the file.
    _edit.Input_AddAudioVideoFile(@"clip1.mp4");
    _edit.Input_AddAudioVideoFile(@"clip2.mp4");
    _edit.Input_AddImageFile(@"logo.png", TimeSpan.FromSeconds(3));

    _edit.Output_Format = new MP4Output(@"output.mp4");
    _edit.Start();
}
```

`Start()` is fire-and-forget — it returns immediately, the render runs on a background thread, and you watch progress via `OnProgress` and completion via `OnStop`. `OnProgress` and `OnStop` fire on a non-UI thread, so marshal back via `Dispatcher.UIThread.Post(...)` (or `InvokeAsync`) before touching Avalonia controls — touching them off-UI throws on Linux and intermittently on macOS. `OnError` may fire on the UI thread depending on where the error originates; the safe rule is to marshal anything that touches UI.

The bundled `references/MainWindow.axaml.cs` extends this with file-picker integration (`TopLevel.GetTopLevel(this).StorageProvider.OpenFilePickerAsync` / `SaveFilePickerAsync` — Avalonia's cross-platform replacement for `OpenFileDialog`), a 12-format output dropdown (MP4 / WebM / AVI / MKV / WMV / WAV / MP3 / M4A / WMA / OGG / FLAC / Speex), debug-log capture, and Avalonia message boxes (via `MsBox.Avalonia`).

## Common cross-platform pitfalls

These are the cross-platform pitfalls that bite first.

### 1. `DllNotFoundException` / "no element X" on Windows or macOS

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`. Common slips:

- Windows build but `VisioForge.CrossPlatform.Core.Windows.x64` was added unconditionally and no `Condition="$([MSBuild]::IsOsPlatform('Windows'))"` wraps it — works on Windows, but `dotnet build` on a macOS / Linux host fails with NuGet restore errors because the Windows redist has no macOS / Linux RID.
- Wrapper / redist version drift (e.g. wrapper `2026.5.4` paired with redist `2026.5.x` instead of `2026.4.29` Windows / `2025.9.1` macOS).

**Fix**: cross-check against `references/Sample.csproj` — every per-OS `ItemGroup` needs the matching `Condition`. Pin every redist to the value shipped in the upstream csproj for your wrapper version; do not bump.

### 2. Linux: app launches but `Start()` errors with "no element X"

**Cause**: GStreamer is not installed system-wide. There is no NuGet redist for Linux; the Linux build relies on the system package manager.

**Fix**: install the GStreamer base + plugin packages for the distro:

- Debian / Ubuntu: `sudo apt install gstreamer1.0-tools gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-plugins-ugly gstreamer1.0-libav`
- Fedora: `sudo dnf install gstreamer1 gstreamer1-plugins-base gstreamer1-plugins-good gstreamer1-plugins-bad-free gstreamer1-plugins-ugly-free gstreamer1-libav`
- Arch: `sudo pacman -S gstreamer gst-plugins-base gst-plugins-good gst-plugins-bad gst-plugins-ugly gst-libav`

`gst-inspect-1.0 --version` should print 1.18 or newer.

### 3. Trial-mode message or "SDK TRIAL period (30 days) is over" on `Start()`

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoEditCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await VideoEdit1.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start()` (see "License registration" above). Every `VideoEditCoreX` instance in the process needs its own call.

### 4. macOS: `File.ReadAllBytes("license.vflicense")` throws `FileNotFoundException`

**Cause**: the working directory inside an `.app` bundle on macOS is `.app/Contents/MacOS/`, not the directory you launched from. Relative paths to data files outside that directory don't resolve.

**Fix**: use `Path.Combine(AppContext.BaseDirectory, "license.vflicense")` and copy the licence to the output via the csproj:

```xml
<ItemGroup>
  <None Include="license.vflicense">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

The same pattern works on Linux (avoids the working-directory-vs-binary-directory ambiguity) and on Windows (no-op — they're already the same).

### 5. UI freezes or throws when `OnProgress` fires

**Cause**: `OnProgress` and `OnStop` fire on a non-UI thread. Touching `pbProgress.Value`, `mmLog.Items`, etc. from those handlers throws on Linux (X11 is single-threaded) and intermittently corrupts state on macOS.

**Fix**: wrap UI mutations in `Dispatcher.UIThread.Post(...)` or `Dispatcher.UIThread.InvokeAsync(...)` — see the upstream sample's `VideoEdit1_OnProgress` and `VideoEdit1_OnStop`.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build` succeeds on each target OS with the bundled `references/` files copied into a fresh project folder (no missing-DLL warnings during build).
- [ ] On Linux: `gst-inspect-1.0 --version` reports 1.18 or newer; the GStreamer base + good + bad + ugly + libav plugin sets are installed.
- [ ] App launches and the first `Start()` runs to completion with `OnStop.Successful == true` for a two-clip MP4 join on each target OS.
- [ ] Output file is finalised correctly (verify it plays back in another player) — `Start()` is async, so wait for `OnStop` before reading the file.
- [ ] `MainWindow_Closing` runs `VideoEdit1.Stop() → DestroyEngine() → VisioForgeX.DestroySDK()` in that order; clean exit on each OS (no zombie process, no native-handle leak warnings).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoEditCoreX` instance before its `Start()` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh Avalonia project folder and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target Avalonia csproj with all per-OS conditional package references (Windows + macOS NuGet redists; Linux uses system GStreamer).
- `references/Program.cs` — Avalonia entry point (`[STAThread]`, `BuildAvaloniaApp().UsePlatformDetect().StartWithClassicDesktopLifetime`).
- `references/App.axaml` + `references/App.axaml.cs` — Avalonia `Application` with `<FluentTheme />` and `MainWindow` instantiation.
- `references/MainWindow.axaml` — Avalonia XAML with input-files list, output-format dropdown (12 formats), output-path picker, debug/telemetry/licensing checkboxes, progress bar, log list, Start/Stop buttons. Declares `xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"` for the `VideoView` namespace (commented-out preview panel; the join sample renders without preview).
- `references/MainWindow.axaml.cs` — full code-behind: `Activated`-with-guard initialisation, `DestroyEngine` + `VisioForgeX.DestroySDK` on close, `TopLevel.StorageProvider`-based file pickers, 12-format output switch, `OnError` / `OnProgress` / `OnStop` wiring with `Dispatcher.UIThread` marshalling, and `MsBox.Avalonia` message boxes. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-x-wpf` — same SDK, Windows-only WPF host with a single TFM (no per-OS conditionals).
    - `video-edit-sdk-x-winforms` — same SDK on WinForms.
    - `video-edit-sdk-x-console` — same SDK in a headless / batch console host.
    - `video-capture-sdk-x-avalonia` — capture sibling (`VideoCaptureCoreX`) for the same Avalonia host.
    - `media-blocks-sdk-net-avalonia` — same engine, lower-level graph-based API for custom pipeline topologies.
    - `media-player-sdk-x-avalonia` — high-level playback API; use when a full edit/render is overkill.
