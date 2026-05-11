---
name: video-capture-sdk-x-macos
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a native .NET for macOS application. Covers the macOS-specific VideoView control, the cross-platform NuGet package, Info.plist NSCameraUsageDescription / NSMicrophoneUsageDescription, code signing entitlements, license registration, and the most common macOS pitfalls (missing Info.plist usage descriptions, hardened runtime + missing entitlements, Apple Silicon vs Intel native libs, trial-period expiry / unlicensed build). Use for native .NET for macOS (NOT MAUI / MacCatalyst) capture apps — for cross-OS MAUI use video-capture-sdk-x-maui.
---

# Video Capture SDK X — native .NET for macOS integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a **native .NET for macOS** application (`net10.0-macos`, AppKit / Storyboards / `NSApplication`). The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes the high-level `VideoCaptureCoreX` god-object with the same API across all platforms; on macOS the host UI is AppKit (`NSViewController`, `NSWindow`) and the preview surface is `VideoViewGL` from `VisioForge.Core.UI.Apple`.

Pinned NuGet versions: wrapper **`2026.5.4`**, macOS native redist **`2025.9.1`** (matches the [official Simple Video Capture sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/macOS/SimpleVideoCapture)). The macOS redist version lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not bump the redist to match the wrapper.

## When to use this skill

- Native .NET for macOS (AppKit) capture/recording apps using `NSApplication` + Storyboards / `.xib`, **not** MAUI and **not** Mac Catalyst.
- Webcam, USB UVC, screen, IP camera, NDI capture into an `NSView` host with a single `VideoViewGL` preview.
- Recording captured video/audio to MP4 (incl. fragmented), MOV, MPEG-TS, WebM, AVI.
- Apple Silicon (arm64) and Intel (x64) macOS — the redist is a fat binary; one project covers both.
- Sharing capture/recording code with companion apps on Windows / Linux / iOS / Android — the `VideoCaptureCoreX` API is identical across platforms.

## When NOT to use this skill

- **MAUI app on macOS** (Mac Catalyst TFM, `UseMaui`, `<my:VideoView />` from `VisioForge.Core.UI.MAUI`): use `video-capture-sdk-x-maui` — same `VideoCaptureCoreX` engine, different host (Mac Catalyst, sandboxed, `Entitlements.plist` lives in `Platforms/MacCatalyst/`), different control, different runtime layout. Do **not** mix native .NET for macOS and MAUI in one project.
- **Cross-platform host other than native macOS**: same SDK, different UI shell — `video-capture-sdk-x-wpf` (Windows WPF), `video-capture-sdk-x-maui` (.NET MAUI), `video-capture-sdk-x-avalonia` (Avalonia), `video-capture-sdk-x-uno` (Uno Platform).
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): use `media-blocks-sdk-net-{maui,avalonia,uno,wpf}` — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play files / streams without capturing): `media-player-sdk-x-macos`.

## Project setup

### Target framework

`net10.0-macos`. The csproj uses the plain **`Microsoft.NET.Sdk`** with `<TargetFramework>net10.0-macos</TargetFramework>` — the macOS-specific workload (`maui-maccatalyst`'s sibling, installed via `dotnet workload install macos`) brings in the AppKit/Foundation bindings. `<SupportedOSPlatformVersion>14.00</SupportedOSPlatformVersion>` matches the upstream sample; bump higher only if you actually require API from a newer macOS.

```bash
dotnet workload install macos
```

Without it, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: macos`.

### NuGet packages

Two packages — the .NET wrapper plus a single macOS native redist. There is no separate "libav" redist on macOS; the macOS Core package already contains the FFmpeg/libav payload:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy Windows-only SDK uses — both `VideoCaptureCore` (legacy DirectShow) and `VideoCaptureCoreX` (cross-platform) ship in it. On `net10.0-macos` only the X engine is reachable; the redist (`VisioForge.CrossPlatform.Core.macOS`) is what makes it actually run.

The macOS redist is a **fat binary** that contains both `arm64` (Apple Silicon) and `x86_64` (Intel) native libraries — one PackageReference covers both architectures. Do **not** look for a `.macOS.arm64` / `.macOS.x64` pair like on Windows; they don't exist for this SDK.

### Full minimal csproj

See `references/Sample.csproj`. Adapted verbatim from the official Simple Video Capture sample at `_SETUP/GitHub/Video Capture SDK X/macOS/SimpleVideoCapture/`.

## Info.plist — usage descriptions (mandatory)

`Info.plist` MUST contain non-empty user-facing strings for every hardware/data class the app touches — macOS terminates the app process on first hardware access without a matching usage-description key. There is no exception, no crash log entry from your code; the process just dies. The minimum for capture:

```xml
<key>NSCameraUsageDescription</key>
<string>Camera usage required</string>
<key>NSMicrophoneUsageDescription</key>
<string>Mic usage required</string>
```

The full `references/Info.plist` matches the upstream sample (also includes `NSPrincipalClass=NSApplication`, `NSMainStoryboardFile=Main`, `XSAppIconAssets`, and a `com.apple.security.device.camera` user-facing string). Copy it as-is, replace `CFBundleName` / `CFBundleIdentifier` with your app's values.

You must also call `AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, …)` early (e.g. in `ViewDidLoad`) to actually trigger the macOS permission prompt — the Info.plist key alone gates *whether macOS will ask*; the API call is what *makes it ask*. The bundled `references/ViewController.cs` does this on load.

## Entitlements.plist — sandbox / hardened runtime

For App Store distribution and for hardened-runtime-signed builds (the default for notarised apps), `Entitlements.plist` MUST grant the camera/microphone/USB entitlements explicitly. Without these, the hardened runtime denies hardware access **silently** — `AVCaptureDevice.RequestAccessForMediaType` returns `granted=false` immediately with no prompt:

```xml
<!-- Entitlements.plist -->
<key>com.apple.security.device.audio-input</key>
<true/>
<key>com.apple.security.device.camera</key>
<true/>
<key>com.apple.security.device.usb</key>
<true/>
```

`com.apple.security.device.usb` is required for USB UVC / capture-card devices that surface as USB rather than the built-in camera. Drop it only if you have audited that no target device class needs it.

For local development (`dotnet run`, ad-hoc signed) you can usually skip the entitlements file and the prompt still works — the hardened runtime is not enforced until `codesign --options runtime` is applied during release packaging. Add `Entitlements.plist` before your first signed build, not after.

The bundled `references/Entitlements.plist` is the upstream sample's file verbatim.

## Mandatory engine boot

The X SDK auto-initialises on the first `VideoCaptureCoreX` constructor call on macOS — there is no explicit `await VisioForgeX.InitSDKAsync()` in the upstream macOS sample (it's a Windows/Linux pattern). What you **do** need is a clean shutdown:

```csharp
// Custom Window delegate to close the SDK
public class CustomWindowDelegate : NSWindowDelegate
{
    public override bool WindowShouldClose(NSObject sender)
    {
        VisioForgeX.DestroySDK();
        return true;
    }
}

// Wire it up in ViewDidLoad (after the window is available):
View.Window.Delegate = new CustomWindowDelegate();
```

Skipping `VisioForgeX.DestroySDK()` is benign for one-shot apps but leaves the GStreamer pipeline running on close in long-lived processes — eventually leading to GPU resource exhaustion if the app is relaunched many times in a single session. Always wire the window delegate.

The first `VideoCaptureCoreX` construction on a fresh machine builds the GStreamer plugin-registry cache (~2-5 s, one-time, blocking). Construct on the main thread inside `InvokeOnMainThread` so the UI does not appear hung; subsequent launches are instant.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _core.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
_core = new VideoCaptureCoreX(_videoView);

var cert = System.IO.File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
await _core.SetLicenseCertificateAsync(cert);
```

Ship the `.vflicense` as a bundle resource (add `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` to the csproj, or use Xcode's "Copy Bundle Resources" build phase). Resolve the runtime path with `NSBundle.MainBundle.PathForResource(...)` — the working directory inside an `.app` bundle is **not** the resources directory.

The certificate-bytes form is the only public licensing API in current 2026.x. For multi-window apps, every `VideoCaptureCoreX` instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`.

The bundled `references/ViewController.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `StartAsync()` right after `_core = new VideoCaptureCoreX(_videoView)`.

## Hello-World capture (native macOS)

Drop the bundled `references/` files into a fresh `dotnet new macos` project, wire a Storyboard with an `NSView videoViewHost` and a "Start" / "Stop" button, and you have webcam preview. The minimum viable wiring (trimmed from `references/ViewController.cs`):

```csharp
using AVFoundation;
using ObjCRuntime;
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Apple;
using VisioForge.Core.VideoCaptureX;

public partial class ViewController : NSViewController
{
    private VideoCaptureCoreX _core;
    private VideoViewGL _videoView;

    protected ViewController(NativeHandle handle) : base(handle) { }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Triggers the macOS permission prompt — Info.plist key alone is not enough.
        AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted => { });

        InvokeOnMainThread(async () =>
        {
            // Wire shutdown so VisioForgeX.DestroySDK() runs on close.
            View.Window.Delegate = new CustomWindowDelegate();
        });
    }

    private async Task StartAsync()
    {
        _videoView = new VideoViewGL(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
        videoViewHost.AddSubview(_videoView);

        _core = new VideoCaptureCoreX(_videoView);

        // For a purchased licence:
        //   var cert = File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
        //   await _core.SetLicenseCertificateAsync(cert);

        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0) return;

        var device = devices[0];
        var fmt = device.VideoFormats.First();
        _core.Video_Source = new VideoCaptureDeviceSourceSettings(device) { Format = fmt.ToFormat() };

        // Preview-only — no Outputs_Add call. For recording, see references/ViewController.cs.
        await _core.StartAsync();
    }
}
```

`references/ViewController.cs` (paired with `references/AppDelegate.cs` and the Storyboard outlets in `ViewController.designer.cs`) ships the full sample with device/format/frame-rate combo boxes, audio source + audio output enumeration, and `StopAsync`/`DisposeAsync` on a button. Use it as a copy-paste template; trim the branches you don't need.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. App silently terminates as soon as capture starts

**Cause**: missing `NSCameraUsageDescription` and/or `NSMicrophoneUsageDescription` in `Info.plist`. macOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no crash log entry from your code.

**Fix**: add both keys with non-empty user-facing strings (see "Info.plist — usage descriptions" above).

### 2. `AVCaptureDevice.RequestAccessForMediaType` returns `granted=false` instantly with no prompt

**Cause**: the build is signed with hardened runtime (`codesign --options runtime`) but `Entitlements.plist` does not grant `com.apple.security.device.camera` / `com.apple.security.device.audio-input`. Hardened runtime denies the access silently; no system prompt is shown.

**Fix**: add `Entitlements.plist` with the camera / audio-input / usb entitlements (see "Entitlements.plist" above) and ensure the codesign command references it: `codesign --entitlements Entitlements.plist --options runtime …`.

### 3. `DllNotFoundException` / `dlopen` failure on first capture

**Cause**: `VisioForge.CrossPlatform.Core.macOS` is not referenced (build succeeds against the managed wrapper alone; the `dlopen` happens on first `VideoCaptureCoreX` use). Or: the `.app` bundle was assembled by hand and the `.dylib` files under `bin/.../net10.0-macos/<RID>/` were not copied into the bundle's `Contents/MonoBundle/`.

**Fix**: confirm the redist PackageReference is present and pinned (see "NuGet packages"). For Apple Silicon vs Intel, the redist is a fat binary — one reference covers both. If you use `dotnet publish -r osx-arm64` / `osx-x64`, `dotnet publish` handles the native copy; for hand-assembled bundles, mirror the layout that `dotnet publish` produces.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Resolve the bundle path with `NSBundle.MainBundle.PathForResource("license", "vflicense")` — `File.ReadAllBytes("license.vflicense")` resolves against the working directory and fails inside an installed `.app`.

### 5. Preview is black / frozen on first frame

**Cause**: `VideoViewGL` was constructed before the host `NSView videoViewHost` had a valid frame, **or** capture was started off the main thread.

**Fix**: defer `VideoViewGL` construction and `StartAsync` to a `ViewDidLoad` continuation wrapped in `InvokeOnMainThread`, after the window has laid out (see `references/ViewController.cs`). Construct `VideoViewGL` with `videoViewHost.Bounds`, not a fixed size — the host view's bounds are zero before layout.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet workload install macos` is installed; `dotnet build` succeeds with the bundled `references/` files copied into a fresh `net10.0-macos` project (no missing-DLL warnings during build).
- [ ] On first run a macOS system prompt asks for Camera and Microphone access (the `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` strings appear verbatim in the prompt).
- [ ] After granting access, the webcam preview appears in `videoViewHost` within ~1-3 s on Apple Silicon, ~2-5 s on Intel (registry build, one-time).
- [ ] Stopping and restarting capture from the UI does not leak `VideoCaptureCoreX` (always call `await _core.StopAsync(); await _core.DisposeAsync();` on stop).
- [ ] On clean window close, `CustomWindowDelegate.WindowShouldClose` runs `VisioForgeX.DestroySDK()`.
- [ ] If recording to MP4: the output file is finalised correctly when the window closes (`StopAsync` runs to completion before `DisposeAsync`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).
- [ ] For a hardened-runtime-signed release build: `codesign -d --entitlements - YourApp.app` shows the camera / audio-input / usb entitlements; `AVCaptureDevice.RequestAccessForMediaType` actually shows a prompt rather than returning `granted=false`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `dotnet new macos` project alongside a Storyboard with the matching outlets, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-macos` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/Info.plist` — bundle metadata + `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` / `com.apple.security.device.camera` user-facing string. Replace `CFBundleName` / `CFBundleIdentifier` with your app's values.
- `references/Entitlements.plist` — hardened-runtime entitlements: `com.apple.security.device.camera`, `com.apple.security.device.audio-input`, `com.apple.security.device.usb`. Required for signed/notarised release builds.
- `references/AppDelegate.cs` — minimal `NSApplicationDelegate` (DidFinishLaunching / WillTerminate) — no SDK code; the SDK lives in `ViewController`.
- `references/ViewController.cs` — full code-behind: `AVCaptureDevice.RequestAccessForMediaType` to trigger the prompt, `DeviceEnumerator` wiring, video / audio source + format + frame-rate selection combo boxes, `VideoViewGL` construction, `StartAsync`/`StopAsync`/`DisposeAsync`, `CustomWindowDelegate` calling `VisioForgeX.DestroySDK()` on close. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/macOS>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-wpf` — same X SDK on Windows WPF host (single-OS Windows build, no Apple plumbing).
    - `video-capture-sdk-x-winui` — same X SDK on Windows / WinUI 3.
    - **Cross-platform hosts**:
        - `video-capture-sdk-x-maui` — same X SDK on .NET MAUI (incl. Mac Catalyst — use this for the MAUI macOS target instead of native .NET for macOS).
        - `video-capture-sdk-x-avalonia` — same X SDK on Avalonia (incl. macOS desktop).
        - `video-capture-sdk-x-uno` — same X SDK on Uno Platform.
