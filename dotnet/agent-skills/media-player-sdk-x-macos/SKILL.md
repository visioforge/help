---
name: media-player-sdk-x-macos
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a native .NET for macOS application. Covers the macOS-specific VideoView control, the cross-platform NuGet package, Info.plist usage descriptions, code signing entitlements, license registration, and the most common macOS pitfalls (hardened runtime, network entitlements for streaming, Apple Silicon vs Intel native libs, trial-period expiry / unlicensed build). Use for native .NET for macOS playback apps — for cross-OS MAUI use media-player-sdk-x-maui.
---

# Media Player SDK X — native .NET for macOS integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the playback SDK — to a **native .NET for macOS** application (`net10.0-macos`, AppKit / Storyboards / `NSApplication`). The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes the high-level `MediaPlayerCoreX` god-object with the same API across all platforms; on macOS the host UI is AppKit (`NSViewController`, `NSWindow`) and the rendering surface is `VideoView` from `VisioForge.Core.UI.Apple`.

Pinned NuGet versions: wrapper **`2026.5.4`**, macOS native redist **`2025.9.1`** (matches the [official Simple Media Player sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/macOS/SimpleMediaPlayer)). The macOS redist version lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not bump the redist to match the wrapper.

## When to use this skill

- Native .NET for macOS (AppKit) playback apps using `NSApplication` + Storyboards / `.xib`, **not** MAUI and **not** Mac Catalyst.
- Local file playback (MP4, MOV, MKV, WebM, MP3, WAV, AAC, FLAC) into an `NSView` host with a single `VideoView` rendering surface.
- HTTP/HTTPS, RTSP, HLS, RTMP network streaming playback.
- Position seek / pause / volume / playback-rate control through `MediaPlayerCoreX`.
- Apple Silicon (arm64) and Intel (x64) macOS — the redist is a fat binary; one project covers both.
- Sharing playback code with companion apps on Windows / Linux / iOS / Android — the `MediaPlayerCoreX` API is identical across platforms.

## When NOT to use this skill

- **MAUI app on macOS** (Mac Catalyst TFM, `UseMaui`, `<my:VideoView />` from `VisioForge.Core.UI.MAUI`): use `media-player-sdk-x-maui` — same `MediaPlayerCoreX` engine, different host (Mac Catalyst, sandboxed, `Entitlements.plist` lives in `Platforms/MacCatalyst/`), different control, different runtime layout. Do **not** mix native .NET for macOS and MAUI in one project.
- **Cross-platform host other than native macOS**: same SDK, different UI shell — `media-player-sdk-x-wpf`, `media-player-sdk-x-winui`, `media-player-sdk-x-winforms`, `media-player-sdk-x-avalonia`, `media-player-sdk-x-maui`, `media-player-sdk-x-uno`.
- **Custom pipeline topology** (split-with-tee, multi-source mix, custom decoders, runtime sink swap): use `media-blocks-sdk-net-{maui,avalonia,uno,wpf}` — `MediaPlayerCoreX` is the high-level wrapper around exactly the same engine.
- **Capture, recording, or screen capture** (webcam, USB UVC, screen, IP camera as a *source* you record): `video-capture-sdk-x-macos`.
- **Non-linear video editing / timeline** (multi-clip composition, transitions, effects pipeline as an editor): `video-edit-sdk-x-*` — the playback X SDK does not ship a timeline.

## Project setup

### Target framework

`net10.0-macos`. The csproj uses the plain **`Microsoft.NET.Sdk`** with `<TargetFramework>net10.0-macos</TargetFramework>` — the macOS-specific workload (installed via `dotnet workload install macos`) brings in the AppKit/Foundation bindings. `<SupportedOSPlatformVersion>14.00</SupportedOSPlatformVersion>` matches the upstream sample; bump higher only if you actually require API from a newer macOS.

```bash
dotnet workload install macos
```

Without it, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: macos`.

### NuGet packages

Two packages — the .NET wrapper plus a single macOS native redist. There is no separate "libav" redist on macOS; the macOS Core package already contains the FFmpeg/libav payload:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>
```

`VisioForge.DotNet.MediaPlayer` is the **same wrapper package** the legacy Windows-only SDK uses — both `MediaPlayerCore` (legacy DirectShow) and `MediaPlayerCoreX` (cross-platform) ship in it. On `net10.0-macos` only the X engine is reachable; the redist (`VisioForge.CrossPlatform.Core.macOS`) is what makes it actually run.

The macOS redist is a **fat binary** that contains both `arm64` (Apple Silicon) and `x86_64` (Intel) native libraries — one PackageReference covers both architectures. Do **not** look for a `.macOS.arm64` / `.macOS.x64` pair like on Windows; they don't exist for this SDK.

### Full minimal csproj

See `references/Sample.csproj`. Adapted verbatim from the official Simple Media Player sample at `_SETUP/GitHub/Media Player SDK X/macOS/SimpleMediaPlayer/`.

## Info.plist — playback-only baseline

A pure playback app does **not** need `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` — those only matter when the app accesses capture hardware. The minimum `Info.plist` for Media Player X is the standard AppKit bundle metadata (`NSPrincipalClass=NSApplication`, `NSMainStoryboardFile=Main`, `XSAppIconAssets`, `CFBundleName`, `CFBundleIdentifier`); the upstream `references/Info.plist` is exactly this.

When **does** the player touch usage-description-gated APIs? Two cases:

- **`NSAppleMusicUsageDescription`** — only if you load tracks from the user's Apple Music / iTunes library via media-library APIs. Not needed for plain file paths or HTTP URLs.
- **`NSDocumentsFolderUsageDescription` / `NSDownloadsFolderUsageDescription` / `NSDesktopFolderUsageDescription`** — only triggered when your code reads files from those specific locations *outside* of an `NSOpenPanel` user-selected file. The bundled `ViewController.cs` opens files via `NSOpenPanel` (which grants per-file access automatically), so these aren't required for the upstream sample.

If you ship a player that auto-loads `~/Movies/intro.mp4` on launch (no user file picker), add `NSDocumentsFolderUsageDescription` with a non-empty user-facing string — otherwise sandboxed/hardened-runtime builds will fail to read the file silently.

## Entitlements.plist — sandbox / hardened runtime

The upstream playback sample ships an **empty** `Entitlements.plist` (`<dict></dict>`) — pure local-file playback against an `NSOpenPanel`-selected file does not need any entitlements beyond the implicit "user-selected file" grant that comes from the open panel itself. The bundled `references/Entitlements.plist` matches.

Add entitlements only when you actually need the corresponding capability:

```xml
<!-- Streaming playback (HTTP/HTTPS, RTSP, HLS, RTMP) under App Sandbox -->
<key>com.apple.security.network.client</key>
<true/>

<!-- Playing files outside the user-selected sandbox (e.g. auto-loading a path) -->
<key>com.apple.security.files.user-selected.read-only</key>
<true/>
```

`com.apple.security.network.client` is the one that bites most often — without it, sandboxed builds fail to open *any* network URL with a misleading "file not found" / "could not open source" error. Streaming playback always needs this entitlement on a sandboxed (App Store) build. Hardened-runtime-only builds (notarised but not sandboxed) get network access for free.

For local development (`dotnet run`, ad-hoc signed) you can usually skip the entitlements file — the hardened runtime + sandbox are not enforced until `codesign --options runtime` and an explicit sandbox profile are applied during release packaging. Add entitlements before your first signed build, not after.

## Mandatory engine boot

Unlike the capture SDK, Media Player X **requires an explicit `VisioForgeX.InitSDK()` call** before constructing the first `MediaPlayerCoreX`. Skipping it produces a `NullReferenceException` deep inside the GStreamer pipeline on the first frame. The upstream sample calls it from `ViewDidLoad`:

```csharp
public override void ViewDidLoad()
{
    base.ViewDidLoad();

    _videoView = new VideoView(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
    videoViewHost.AddSubview(_videoView);

    VisioForgeX.InitSDK();   // mandatory, before any MediaPlayerCoreX construction
}
```

You also need a clean shutdown via the window delegate:

```csharp
public class CustomWindowDelegate : NSWindowDelegate
{
    private readonly ViewController _viewController;
    public CustomWindowDelegate(ViewController vc) { _viewController = vc; }

    public override bool WindowShouldClose(NSObject sender)
    {
        // Stop the player BEFORE destroying the SDK so the pipeline drains cleanly.
        _viewController.StopAsync().GetAwaiter().GetResult();
        VisioForgeX.DestroySDK();
        return true;
    }
}

// Wire it up in StartAsync (or ViewDidLoad after the window is available):
View.Window.Delegate = new CustomWindowDelegate(this);
```

Skipping `VisioForgeX.DestroySDK()` is benign for one-shot apps but leaves the GStreamer pipeline running on close in long-lived processes — eventually leading to GPU resource exhaustion if the app is relaunched many times in a single session. Always wire the window delegate.

The first `InitSDK()` on a fresh machine builds the GStreamer plugin-registry cache (~2-5 s, one-time, blocking). Call it on the main thread inside `ViewDidLoad` so the UI does not appear hung; subsequent launches are instant.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and **before `OpenAsync`**:

```csharp
_player = new MediaPlayerCoreX(_videoView);

var cert = System.IO.File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
await _player.SetLicenseCertificateAsync(cert);

await _player.OpenAsync(sourceSettings);
```

Ship the `.vflicense` as a bundle resource (add `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` to the csproj, or use Xcode's "Copy Bundle Resources" build phase). Resolve the runtime path with `NSBundle.MainBundle.PathForResource(...)` — the working directory inside an `.app` bundle is **not** the resources directory.

The certificate-bytes form is the only public licensing API in current 2026.x. For multi-window apps, every `MediaPlayerCoreX` instance needs its own `SetLicenseCertificateAsync` call before its `OpenAsync`.

The bundled `references/ViewController.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `StartAsync()` right after `_player = new MediaPlayerCoreX(_videoView)`.

## Hello-World playback (native macOS)

Drop the bundled `references/` files into a fresh `dotnet new macos` project, wire a Storyboard with an `NSView videoViewHost`, an `NSTextField edFilename`, an `NSSlider slPosition`, and "Open" / "Start" / "Stop" buttons, and you have file playback. The minimum viable wiring (trimmed from `references/ViewController.cs`):

```csharp
using ObjCRuntime;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Apple;

public partial class ViewController : NSViewController
{
    private MediaPlayerCoreX _player;
    private VideoView _videoView;

    protected ViewController(NativeHandle handle) : base(handle) { }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        _videoView = new VideoView(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
        videoViewHost.AddSubview(_videoView);

        VisioForgeX.InitSDK();
    }

    private async Task StartAsync()
    {
        if (View.Window.Delegate == null)
            View.Window.Delegate = new CustomWindowDelegate(this);

        var sourceSettings = await UniversalSourceSettings.CreateAsync(edFilename.StringValue);
        if (sourceSettings == null) return;

        _player = new MediaPlayerCoreX(_videoView);

        // For a purchased licence:
        //   var cert = File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
        //   await _player.SetLicenseCertificateAsync(cert);

        await _player.OpenAsync(sourceSettings);
        await _player.PlayAsync();
    }

    public async Task StopAsync()
    {
        if (_player == null) return;
        await _player.StopAsync();
        await _player.DisposeAsync();
        _player = null;
    }
}
```

`references/ViewController.cs` (paired with `references/AppDelegate.cs` and the Storyboard outlets in `ViewController.designer.cs`) ships the full sample with `NSOpenPanel`-based file picker (filtered to MP4 / MOV / MP3 / WAV via `UTTypes`), a position-update timer that drives `slPosition` from `Position_GetAsync` / `DurationAsync`, and seek-on-slider-change via `Position_SetAsync`. Use it as a copy-paste template; trim the branches you don't need.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. `NullReferenceException` on first frame inside the GStreamer pipeline

**Cause**: `VisioForgeX.InitSDK()` was never called. Capture SDK X auto-initialises on first `VideoCaptureCoreX` construction; **Media Player SDK X does not** — `InitSDK()` is mandatory.

**Fix**: call `VisioForgeX.InitSDK()` once on the main thread in `ViewDidLoad`, before any `MediaPlayerCoreX` constructor. Pair it with `VisioForgeX.DestroySDK()` from the window-close delegate.

### 2. Streaming URL fails with "could not open source" / "file not found"

**Cause**: the build is sandboxed (App Store distribution) but `Entitlements.plist` does not grant `com.apple.security.network.client`. Sandboxed apps cannot open *any* outbound socket without this entitlement; the GStreamer source returns a misleading file-style error rather than a network one.

**Fix**: add `<key>com.apple.security.network.client</key><true/>` to `Entitlements.plist` and re-sign. Hardened-runtime-only (notarised, non-sandbox) builds get network access for free and don't need this.

### 3. `DllNotFoundException` / `dlopen` failure on first playback

**Cause**: `VisioForge.CrossPlatform.Core.macOS` is not referenced (build succeeds against the managed wrapper alone; the `dlopen` happens on first `MediaPlayerCoreX` use). Or: the `.app` bundle was assembled by hand and the `.dylib` files under `bin/.../net10.0-macos/<RID>/` were not copied into the bundle's `Contents/MonoBundle/`.

**Fix**: confirm the redist PackageReference is present and pinned (see "NuGet packages"). For Apple Silicon vs Intel, the redist is a fat binary — one reference covers both. If you use `dotnet publish -r osx-arm64` / `osx-x64`, `dotnet publish` handles the native copy; for hand-assembled bundles, mirror the layout that `dotnet publish` produces.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the player instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` file as bytes and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` (see "License registration" above). Resolve the bundle path with `NSBundle.MainBundle.PathForResource("license", "vflicense")` — `File.ReadAllBytes("license.vflicense")` resolves against the working directory and fails inside an installed `.app`.

### 5. Video is black / frozen on first frame (audio plays normally)

**Cause**: `VideoView` was constructed before the host `NSView videoViewHost` had a valid frame, **or** `OpenAsync` / `PlayAsync` was called off the main thread.

**Fix**: construct `VideoView` from inside `ViewDidLoad` using `videoViewHost.Bounds` (not a fixed size — the host view's bounds are zero before layout). Wrap the click handlers' `StartAsync` / `StopAsync` calls in `InvokeOnMainThread` (the bundled `references/ViewController.cs` already does this). If the host view is laid out via Auto Layout *after* `ViewDidLoad`, defer `VideoView` creation to `ViewDidAppear` instead.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet workload install macos` is installed; `dotnet build` succeeds with the bundled `references/` files copied into a fresh `net10.0-macos` project (no missing-DLL warnings during build).
- [ ] On first launch, a local MP4 file selected via `NSOpenPanel` plays in `videoViewHost` within ~1-3 s on Apple Silicon, ~2-5 s on Intel (registry build, one-time).
- [ ] `slPosition` advances smoothly during playback; dragging the slider seeks via `Position_SetAsync` without stuttering or feedback loops.
- [ ] Stopping and restarting playback from the UI does not leak `MediaPlayerCoreX` (always call `await _player.StopAsync(); await _player.DisposeAsync(); _player = null;` on stop).
- [ ] On clean window close, `CustomWindowDelegate.WindowShouldClose` runs `StopAsync` first, then `VisioForgeX.DestroySDK()`.
- [ ] If playing network URLs (HLS / RTSP / HTTP) on a sandboxed build: `com.apple.security.network.client` is set in `Entitlements.plist` and the URL opens.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode).
- [ ] For a hardened-runtime-signed release build: `codesign -d --entitlements - YourApp.app` shows the entitlements you expect; network playback works under the sandbox if applicable.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `dotnet new macos` project alongside a Storyboard with the matching outlets, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-macos` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/Info.plist` — bundle metadata only (`NSPrincipalClass=NSApplication`, `NSMainStoryboardFile=Main`, `XSAppIconAssets`, `CFBundleName`, `CFBundleIdentifier`). Replace `CFBundleName` / `CFBundleIdentifier` with your app's values. No `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` — playback does not touch capture hardware.
- `references/Entitlements.plist` — empty `<dict>` (matches the upstream sample). Add `com.apple.security.network.client` if you do streaming playback on a sandboxed build; add `com.apple.security.files.user-selected.read-only` if you bypass `NSOpenPanel`.
- `references/AppDelegate.cs` — minimal `NSApplicationDelegate` (DidFinishLaunching only) — no SDK code; the SDK lives in `ViewController`.
- `references/ViewController.cs` — full code-behind: `VisioForgeX.InitSDK()` in `ViewDidLoad`, `NSOpenPanel`-based file picker filtered via `UTTypes`, `MediaPlayerCoreX` construction + `OpenAsync` + `PlayAsync`, position-update timer driving `slPosition`, seek-on-slider-change via `Position_SetAsync`, `StopAsync`/`DisposeAsync`, `CustomWindowDelegate` that stops the player and calls `VisioForgeX.DestroySDK()` on close. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/macOS>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-x-wpf` — same X SDK on Windows WPF host (single-OS Windows build, no Apple plumbing).
    - `media-player-sdk-x-winui` — same X SDK on Windows / WinUI 3.
    - `media-player-sdk-x-winforms` — same X SDK on Windows / WinForms.
    - `media-player-sdk-x-avalonia` — same X SDK on Avalonia (cross-OS desktop).
    - `video-capture-sdk-x-macos` — capture/recording counterpart on the same macOS host.
    - **Cross-platform hosts**:
        - `media-player-sdk-x-maui` — same X SDK on .NET MAUI (incl. Mac Catalyst — use this for the MAUI macOS target instead of native .NET for macOS).
        - `media-player-sdk-x-uno` — same X SDK on Uno Platform.
