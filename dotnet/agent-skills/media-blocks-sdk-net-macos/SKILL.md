---
name: media-blocks-sdk-net-macos
description: Integrate VisioForge Media Blocks SDK into a native .NET for macOS application. Covers the graph-based pipeline model on macOS, the cross-platform NuGet package, Info.plist NSCameraUsageDescription / NSMicrophoneUsageDescription, code signing entitlements, license registration, and the most common macOS pitfalls (missing usage descriptions, hardened runtime + missing entitlements, Apple Silicon vs Intel native libs, trial-period expiry / unlicensed build). Use for native .NET for macOS pipelines (capture, transcode, stream, record) — for cross-OS MAUI use media-blocks-sdk-net-maui.
---

# Media Blocks SDK .NET — native .NET for macOS integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a **native .NET for macOS** application (`net10.0-macos`, AppKit / Storyboards / `NSApplication`). Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `H264EncoderBlock`, `MP4SinkBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. On macOS the host UI is AppKit (`NSViewController`, `NSWindow`) and the preview surface is `VideoView` from `VisioForge.Core.UI.Apple`.

Pinned NuGet versions: wrapper **`2026.5.4`**, macOS native redist **`2025.9.1`** (matches the [official Simple Video Capture MB sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/macOS/SimpleVideoCaptureMBMac)). The macOS redist version lags the wrapper version on purpose — pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not bump the redist to match the wrapper.

## When to use this skill

- Native .NET for macOS (AppKit) pipelines using `NSApplication` + Storyboards / `.xib`, **not** MAUI and **not** Mac Catalyst.
- Custom pipeline topologies on macOS the high-level capture/playback SDKs don't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capturing → encoding → muxing to MP4 / MOV / MPEG-TS / WebM with explicit control over each stage on macOS.
- Network streaming sinks (RTSP server, SRT, RIST, NDI, WebRTC WHIP, YouTube/Facebook RTMP) wired into a custom graph on macOS.
- Apple Silicon (arm64) and Intel (x64) macOS — the redist is a fat binary; one project covers both.
- Sharing pipeline code with companion apps on Windows / Linux / iOS / Android — the block API is identical across platforms; only the host control and the redist change.

## When NOT to use this skill

- **MAUI app on macOS** (Mac Catalyst TFM, `UseMaui`, `<my:VideoView />` from `VisioForge.Core.UI.MAUI`): use `media-blocks-sdk-net-maui` — different host (Mac Catalyst, sandboxed, `Entitlements.plist` lives in `Platforms/MacCatalyst/`), different control, different runtime layout. Do **not** mix native .NET for macOS and MAUI in one project.
- **Plain webcam capture** (preview + record, no custom topology) on macOS: `video-capture-sdk-x-macos` is dramatically less code — `VideoCaptureCoreX` is the high-level wrapper around exactly the same engine.
- **Playback only** (play a file or network stream, no capture / no custom graph) on macOS: `media-player-sdk-x-macos`.
- **Cross-platform host other than native macOS**: same SDK, different UI shell — `media-blocks-sdk-net-wpf` (Windows WPF), `media-blocks-sdk-net-avalonia` (cross-platform desktop), `media-blocks-sdk-net-maui` (incl. Mac Catalyst).

## Project setup

### Target framework

`net10.0-macos`. The csproj uses the plain **`Microsoft.NET.Sdk`** with `<TargetFramework>net10.0-macos</TargetFramework>` and `<RuntimeIdentifiers>osx-arm64;osx-x64</RuntimeIdentifiers>`. The macOS-specific workload (installed via `dotnet workload install macos`) brings in the AppKit/Foundation bindings. `<SupportedOSPlatformVersion>15.0</SupportedOSPlatformVersion>` matches the upstream sample; bump higher only if you actually require API from a newer macOS.

```bash
dotnet workload install macos
```

Without it, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: macos`.

### NuGet packages

Two packages — the .NET wrapper plus a single macOS native redist. There is no separate "libav" redist on macOS; the macOS Core package already contains the FFmpeg/libav payload (this differs from the Windows Media Blocks setup, which requires the libav package on top of Core):

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
</ItemGroup>
```

The macOS redist is a **fat binary** that contains both `arm64` (Apple Silicon) and `x86_64` (Intel) native libraries — one PackageReference covers both architectures. Do **not** look for a `.macOS.arm64` / `.macOS.x64` pair like on Windows; they don't exist for this SDK. Mismatches between wrapper and redist (jumping the redist to match the wrapper version, or vice versa) are undefined behaviour and surface as `DllNotFoundException` / `dlopen` failures or `Element 'X' not found` errors at pipeline start.

### Full minimal csproj

See `references/Sample.csproj`. Adapted verbatim from the official Simple Video Capture MB sample at `_SETUP/GitHub/Media Blocks SDK/macOS/SimpleVideoCaptureMBMac/`.

## Info.plist — usage descriptions (mandatory)

`Info.plist` MUST contain non-empty user-facing strings for every hardware/data class the app touches — macOS terminates the app process on first hardware access without a matching usage-description key. There is no exception, no crash log entry from your code; the process just dies. The minimum for capture pipelines:

```xml
<key>NSCameraUsageDescription</key>
<string>Camera usage required</string>
<key>NSMicrophoneUsageDescription</key>
<string>Mic usage required</string>
```

The full `references/Info.plist` matches the upstream sample (also includes `NSPrincipalClass=NSApplication`, `NSMainStoryboardFile=Main`, `XSAppIconAssets`, and a `com.apple.security.device.camera` user-facing string). Copy it as-is, replace `CFBundleName` / `CFBundleIdentifier` with your app's values.

You must also call `AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, …)` early (e.g. in `ViewDidLoad`) to actually trigger the macOS permission prompt — the Info.plist key alone gates *whether macOS will ask*; the API call is what *makes it ask*. The bundled `references/ViewController.cs` does this on load.

For pipelines that don't capture (file-to-file transcode, file playback, network-source-only) the camera/microphone keys are unnecessary — but if any block in the graph might enumerate or touch a hardware device, include them anyway: `DeviceEnumerator.Shared.VideoSourcesAsync()` itself does not trigger the prompt, but the first frame from `SystemVideoSourceBlock` does, and a missing key kills the process at that exact moment.

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

## Pipeline model

This is the core mental shift from the higher-level Video Capture SDK X. There is no `VideoCaptureCoreX.StartAsync()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario; multiple pipelines per process are fine.
2. **Source blocks** (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `RTSPSourceBlock`, `UniversalSourceBlock` for files, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock`, `AACEncoderBlock`, `VPXEncoderBlock`, `TeeBlock`, `VideoMixerBlock`, `AudioMixerBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** — terminate the graph. **Renderer sinks** drive the UI / speakers (`VideoRendererBlock` bound to a macOS `VideoView`, `AudioRendererBlock` bound to a system audio output). **File / network sinks** write/transmit (`MP4SinkBlock`, `MPEGTSSinkBlock`, `WebMSinkBlock`, `RTSPServerBlock`, `SRTMPEGTSSinkBlock`, …). Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `StartAsync`; reconnecting at runtime is supported by specific blocks (live-source-switch, bridge) but not the general case.

Topology example (preview-only capture, video + audio):

```text
SystemVideoSourceBlock  --video-->  VideoRendererBlock (VideoView)
SystemAudioSourceBlock  --audio-->  AudioRendererBlock (system output)
```

For preview + MP4 record on macOS, splice a `TeeBlock` on each path (one branch to the renderer, one to an `H264EncoderBlock` / `AACEncoderBlock` → `MP4SinkBlock`).

Engine boot on macOS is implicit — the X engine auto-initialises on the first `MediaBlocksPipeline` constructor call, there is no explicit `await VisioForgeX.InitSDKAsync()` in the upstream macOS sample (that's a Windows/Linux pattern). What you **do** need is a clean shutdown, wired through an `NSWindowDelegate`:

```csharp
public class CustomWindowDelegate : NSWindowDelegate
{
    public override bool WindowShouldClose(NSObject sender)
    {
        VisioForgeX.DestroySDK();
        return true;
    }
}

// In ViewDidLoad after the window is available:
View.Window.Delegate = new CustomWindowDelegate();
```

Skipping `VisioForgeX.DestroySDK()` is benign for one-shot apps but leaves the GStreamer pipeline running on close in long-lived processes — eventually leading to GPU resource exhaustion if the app is relaunched many times in a single session. Always wire the window delegate. Tear down each `MediaBlocksPipeline` instance with `await _pipeline.StopAsync(); await _pipeline.DisposeAsync();` *before* the window delegate fires; the delegate only handles the SDK-wide teardown.

The first `MediaBlocksPipeline` construction on a fresh machine builds the GStreamer plugin-registry cache (~2-5 s on Apple Silicon, ~3-7 s on Intel, one-time, blocking). Construct on the main thread inside `InvokeOnMainThread` so the UI does not appear hung; subsequent launches are instant.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();

var cert = System.IO.File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
await _pipeline.SetLicenseCertificateAsync(cert);
```

Ship the `.vflicense` as a bundle resource (add `<None Include="license.vflicense"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>` to the csproj, or use Xcode's "Copy Bundle Resources" build phase). Resolve the runtime path with `NSBundle.MainBundle.PathForResource(...)` — the working directory inside an `.app` bundle is **not** the resources directory, so `File.ReadAllBytes("license.vflicense")` will throw `FileNotFoundException` once the app is installed.

The certificate-bytes form is the only public licensing API as of `2026.5.2` — older `SetLicenseCertificate(string filePath)` / `SetLicenseCertificate(Stream)` overloads were removed. For multi-pipeline apps, every `MediaBlocksPipeline` instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`.

The bundled `references/ViewController.cs` is the upstream sample, which runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence in the bundled reference, add the two lines above into `StartAsync()` right after `_pipeline = new MediaBlocksPipeline();`.

## Hello-World pipeline (native macOS)

Drop the bundled `references/` files into a fresh `dotnet new macos` project, wire a Storyboard with an `NSView videoViewHost` and a "Start" / "Stop" button, and you have webcam + microphone preview through a 2-source pipeline. The minimum viable wiring (trimmed from `references/ViewController.cs`):

```csharp
using AVFoundation;
using ObjCRuntime;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.Apple;

public partial class ViewController : NSViewController
{
    private MediaBlocksPipeline _pipeline;
    private SystemVideoSourceBlock _videoSource;
    private VideoRendererBlock _videoRenderer;
    private VideoView _videoView;

    protected ViewController(NativeHandle handle) : base(handle) { }

    public override void ViewDidLoad()
    {
        base.ViewDidLoad();

        // Triggers the macOS permission prompt — the Info.plist key alone is not enough.
        AVCaptureDevice.RequestAccessForMediaType(AVAuthorizationMediaType.Video, granted => { });

        InvokeOnMainThread(() =>
        {
            // Wire shutdown so VisioForgeX.DestroySDK() runs on close.
            View.Window.Delegate = new CustomWindowDelegate();

            // Construct VideoView with the host's bounds, NOT a fixed size — bounds are
            // zero before layout, so this must run after the storyboard has been realised.
            _videoView = new VideoView(new CGRect(0, 0, videoViewHost.Bounds.Width, videoViewHost.Bounds.Height));
            videoViewHost.AddSubview(_videoView);
        });
    }

    private async Task StartAsync()
    {
        _pipeline = new MediaBlocksPipeline();

        // For a purchased licence:
        //   var cert = File.ReadAllBytes(NSBundle.MainBundle.PathForResource("license", "vflicense"));
        //   await _pipeline.SetLicenseCertificateAsync(cert);

        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0) return;

        // Let the SDK pick the device's default format. Setting Format = device.VideoFormats[0]
        // breaks on virtual cameras / IP shims that enumerate with an empty format list.
        var settings = new VideoCaptureDeviceSourceSettings(devices[0]);
        _videoSource = new SystemVideoSourceBlock(settings);

        // VideoRendererBlock binds to the macOS VideoView at construction time. IsSync = false
        // is the standard preview setting — frames render as fast as they arrive instead of
        // being clock-throttled (which can stutter on capture sources whose timestamps drift).
        _videoRenderer = new VideoRendererBlock(_pipeline, _videoView as IVideoView) { IsSync = false };

        _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);

        await _pipeline.StartAsync();
    }
}
```

`references/ViewController.cs` (paired with `references/AppDelegate.cs` and the Storyboard outlets in `ViewController.designer.cs`) ships the full sample with device/format/frame-rate combo boxes, the audio source + audio renderer path, `StopAsync`/`DisposeAsync` on a button, and `CustomWindowDelegate` calling `VisioForgeX.DestroySDK()` on close. Use it as a copy-paste starting template; trim the branches you don't need.

## Common deployment failures

These are the five most common production issues — flag any of them on first run.

### 1. App silently terminates as soon as the pipeline starts

**Cause**: missing `NSCameraUsageDescription` and/or `NSMicrophoneUsageDescription` in `Info.plist`. macOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no crash log entry from your code. The kill happens at the moment a `SystemVideoSourceBlock` / `SystemAudioSourceBlock` produces its first frame, which often *looks* like a `pipeline.StartAsync()` failure.

**Fix**: add both keys with non-empty user-facing strings (see "Info.plist — usage descriptions" above).

### 2. `AVCaptureDevice.RequestAccessForMediaType` returns `granted=false` instantly with no prompt

**Cause**: the build is signed with hardened runtime (`codesign --options runtime`) but `Entitlements.plist` does not grant `com.apple.security.device.camera` / `com.apple.security.device.audio-input`. Hardened runtime denies the access silently; no system prompt is shown.

**Fix**: add `Entitlements.plist` with the camera / audio-input / usb entitlements (see "Entitlements.plist" above) and ensure the codesign command references it: `codesign --entitlements Entitlements.plist --options runtime …`.

### 3. `DllNotFoundException` / `dlopen` failure / pipeline `OnError` "Element 'X' not found"

**Cause A — missing redist**: `VisioForge.CrossPlatform.Core.macOS` is not referenced (build succeeds against the managed wrapper alone; the `dlopen` happens on first `MediaBlocksPipeline` use).
**Cause B — hand-assembled bundle**: the `.app` bundle was assembled by hand and the `.dylib` files under `bin/.../net10.0-macos/<RID>/` were not copied into the bundle's `Contents/MonoBundle/`.
**Cause C — wrapper/redist drift**: someone bumped the wrapper version without bumping the redist (or vice versa) past the ABI boundary; the same symptom surfaces as a missing element from the GStreamer plugin registry rather than a `dlopen` error.

**Fix**: confirm the redist PackageReference is present and pinned to the value used by the upstream sample for your wrapper version (see "NuGet packages"). The redist is a fat binary — one reference covers both Apple Silicon and Intel. If you use `dotnet publish -r osx-arm64` / `osx-x64`, `dotnet publish` handles the native copy; for hand-assembled bundles, mirror the layout that `dotnet publish` produces.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded on the pipeline instance — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started.

**Fix**: read the `.vflicense` file as bytes and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Resolve the bundle path with `NSBundle.MainBundle.PathForResource("license", "vflicense")` — `File.ReadAllBytes("license.vflicense")` resolves against the working directory and fails inside an installed `.app`.

### 5. Preview is black / frozen on first frame

**Cause A — VideoView constructed too early**: `VideoView` was constructed before the host `NSView videoViewHost` had a valid frame, so its bounds were zero at construction time.
**Cause B — wrong thread**: pipeline was started off the main thread.
**Cause C — UI thread blocking on stop**: calling `_pipeline.Stop()` (synchronous overload) or `Dispose()` from the main thread while `VideoRendererBlock` is still pumping frames deadlocks because the renderer needs the main thread to release the last frame.

**Fix**: defer `VideoView` construction and `StartAsync` to a `ViewDidLoad` continuation wrapped in `InvokeOnMainThread`, after the window has laid out (see `references/ViewController.cs`). Construct `VideoView` with `videoViewHost.Bounds`, not a fixed size — the host view's bounds are zero before layout. On stop, always use `await _pipeline.StopAsync()` and `await _pipeline.DisposeAsync()`, never the synchronous overloads.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet workload install macos` is installed; `dotnet build` succeeds with the bundled `references/` files copied into a fresh `net10.0-macos` project (no missing-DLL warnings during build).
- [ ] On first run a macOS system prompt asks for Camera and Microphone access (the `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` strings appear verbatim in the prompt).
- [ ] After granting access, the webcam preview appears in `videoViewHost` within ~2-5 s on Apple Silicon, ~3-7 s on Intel (registry build, one-time); subsequent runs are instant.
- [ ] Stopping and restarting the pipeline from the UI does not leak (always call `await _pipeline.StopAsync(); await _pipeline.DisposeAsync(); _pipeline = null;` on stop, then construct a fresh `MediaBlocksPipeline` on the next start).
- [ ] On clean window close, `CustomWindowDelegate.WindowShouldClose` runs `VisioForgeX.DestroySDK()`.
- [ ] If recording to a file sink: output file is finalised correctly when the window closes (`StopAsync` runs to completion before `DisposeAsync`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance before its `StartAsync` (otherwise the app runs in 30-day trial mode).
- [ ] For a hardened-runtime-signed release build: `codesign -d --entitlements - YourApp.app` shows the camera / audio-input / usb entitlements; `AVCaptureDevice.RequestAccessForMediaType` actually shows a prompt rather than returning `granted=false`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `dotnet new macos` project alongside a Storyboard with the matching outlets, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-macos` csproj, version-pinned to the same NuGet release as the prose (see the "Pinned NuGet versions" line in the intro paragraph).
- `references/Info.plist` — bundle metadata + `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` / `com.apple.security.device.camera` user-facing string. Replace `CFBundleName` / `CFBundleIdentifier` with your app's values.
- `references/Entitlements.plist` — hardened-runtime entitlements: `com.apple.security.device.camera`, `com.apple.security.device.audio-input`, `com.apple.security.device.usb`. Required for signed/notarised release builds.
- `references/AppDelegate.cs` — minimal `NSApplicationDelegate` (DidFinishLaunching / WillTerminate) — no SDK code; the SDK lives in `ViewController`.
- `references/ViewController.cs` — full code-behind: `AVCaptureDevice.RequestAccessForMediaType` to trigger the prompt, `DeviceEnumerator` wiring, video / audio source + format + frame-rate selection combo boxes, `MediaBlocksPipeline` construction, `SystemVideoSourceBlock` + `SystemAudioSourceBlock` + `VideoRendererBlock` + `AudioRendererBlock` graph, `StartAsync`/`StopAsync`/`DisposeAsync`, `CustomWindowDelegate` calling `VisioForgeX.DestroySDK()` on close. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/macOS>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-wpf` — same SDK on Windows WPF host (single-OS Windows build, no Apple plumbing).
    - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (cross-platform desktop incl. macOS).
    - `media-blocks-sdk-net-maui` — for Mac Catalyst (the MAUI macOS target), use this instead of native .NET for macOS.
    - `video-capture-sdk-x-macos` — high-level capture-and-record API on native macOS; use this if you don't need a custom pipeline.
    - `media-player-sdk-x-macos` — playback-only API on native macOS.
