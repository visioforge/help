---
name: media-blocks-sdk-net-ios
description: Integrate VisioForge Media Blocks SDK into a native .NET for iOS application. Covers the graph-based pipeline model on iOS, the cross-platform NuGet package, Info.plist NSCameraUsageDescription / NSMicrophoneUsageDescription requirements, license registration, AOT JIT-only ExecutionEngineException, and the most common iOS pitfalls (missing usage descriptions, no camera in simulator, App Store reviewer rejecting missing privacy strings, trial-period expiry / unlicensed build). Use for native .NET for iOS pipelines (capture, transcode, stream, record) — for cross-OS MAUI use media-blocks-sdk-net-maui.
---

# Media Blocks SDK .NET — native .NET for iOS integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a **native .NET for iOS** application (`net10.0-ios` TFM, UIKit, no MAUI/Xamarin shell). Media Blocks is a graph-based pipeline SDK (think GStreamer-style filter chains) — you compose a pipeline by instantiating individual blocks (`SystemVideoSourceBlock`, `SystemAudioSourceBlock`, `H264EncoderBlock`, `AACEncoderBlock`, `MP4SinkBlock`, `VideoRendererBlock`, `TeeBlock`, …), wiring their pads with `pipeline.Connect(output, input)`, then calling `await pipeline.StartAsync()`. Compared to the higher-level Video Capture SDK X (a single `VideoCaptureCoreX` god-object), Media Blocks gives you full control over the topology — splitting streams with tees, mixing sources, transcoding without preview, swapping sinks at runtime — at the cost of having to wire every edge yourself.

Pinned NuGet versions: wrapper **`VisioForge.DotNet.MediaBlocks` 2026.5.4**, native iOS redist **`VisioForge.CrossPlatform.Core.iOS` 2025.0.16** (matches the [official Simple Video Capture MB iOS sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/iOS/SimpleVideoCapture)). The iOS redist version trails the wrapper version on purpose — it tracks the underlying GStreamer-iOS rebuild cadence, which has slower release tempo than the managed wrapper. Pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redist to match the wrapper.

## When to use this skill

- Building a **custom pipeline** on native iOS that the high-level capture SDK doesn't expose: split-with-tee preview + record, multi-source mix, transcode without preview, dynamic source switch, stream + record with separate encoders.
- Capturing iOS camera + microphone → encoding (VideoToolbox H.264 via `AppleMediaH264EncoderSettings`, AAC) → muxing to MP4 in the app sandbox.
- Network streaming (RTMP, SRT, RIST, WebRTC WHIP) wired into a custom graph from native UIKit.
- Sharing pipeline code between native iOS and Windows/macOS/Android/MAUI companions — block instantiation and `pipeline.Connect` calls are identical across hosts; only the renderer's `IVideoView` host differs.

## When NOT to use this skill

- **Plain webcam capture** on iOS (preview + MP4 record, no custom topology): `video-capture-sdk-x-ios` is dramatically less code — one `VideoCaptureCoreX` instance instead of seven blocks plus tee.
- **MAUI / Xamarin** (one C# project shared across iOS + Android + Windows + Mac Catalyst): `media-blocks-sdk-net-maui` — the per-OS NuGet wiring and `<UseInterpreter>` placement are different.
- **macOS native (`net10.0-macos`)** with AppKit/NSWindow: `media-blocks-sdk-net-macos` — different redist, no `MtouchInterpreter`.
- **Android native (`net10.0-android`)**: `media-blocks-sdk-net-android` — different redist plus the small `VisioForge.Core.Android.X10.csproj` companion `<ProjectReference>`.
- **Playback only** (play files / streams without capturing or building a custom graph): `media-player-sdk-x-ios`.

## Project setup

### Target framework

Use `net10.0-ios` with `<SupportedOSPlatformVersion>14.0</SupportedOSPlatformVersion>` — that's the floor the SDK is tested against. iOS 13 may load but `AppleMediaH264EncoderSettings` (the VideoToolbox H.264 path the iOS redist ships) was finalised at iOS 14, and the GStreamer-iOS plugin set assumes the iOS 14 AVFoundation surface.

Set `<RuntimeIdentifier>ios-arm64</RuntimeIdentifier>` for device builds. The redist NuGet bundles both Apple Silicon device and simulator slices in one package, but the simulator slice is intended for non-camera UI work only — see "No camera devices" below.

### NuGet packages

Two packages — the .NET wrapper plus the iOS native redist. The redist is **not** transitive; you must reference it explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
```

There is no `.x64` / `.x86` split on iOS — Apple Silicon device + simulator slices are bundled in the single redist NuGet. There is no separate `Libav` redist on iOS either; on Windows the H.264/AAC/MP4 path needs `VisioForge.CrossPlatform.Libav.Windows.x64.UPX`, but on iOS those codecs come from VideoToolbox and `AudioToolbox` via the platform redist alone.

### Mandatory Mono interpreter

Media Blocks SDK uses dynamic codegen paths (`DynamicMethod`, late-bound generic instantiations in the GStreamer glue) that the iOS AOT compiler refuses to emit. Without the Mono interpreter enabled, the very first `VisioForgeX.InitSDK()` call throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode`:

```xml
<PropertyGroup Condition=" '$(Configuration)' == 'Debug' ">
  <MtouchLink>SdkOnly</MtouchLink>
  <MtouchInterpreter>all</MtouchInterpreter>
</PropertyGroup>
<PropertyGroup Condition=" '$(Configuration)' == 'Release' ">
  <MtouchLink>SdkOnly</MtouchLink>
  <MtouchInterpreter>all</MtouchInterpreter>
</PropertyGroup>
```

`MtouchInterpreter=all` is the native .NET for iOS equivalent of MAUI's `<UseInterpreter>true</UseInterpreter>`. It applies to both Debug and Release — Release builds (including TestFlight / App Store binaries) are not exempt. Tiny startup cost, fixes the JIT failure.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture MB iOS sample (`_DEMOS/Media Blocks SDK/iOS/SimpleVideoCapture/`) — the upstream csproj uses `<ProjectReference>` to internal `VisioForge.Core.csproj` / `VisioForge.Libs.csproj` checkouts; the bundled `Sample.csproj` swaps those for a single `VisioForge.DotNet.MediaBlocks` `<PackageReference>` so it builds standalone against public NuGet.

## Pipeline model

This is the core mental shift from Video Capture SDK X. There is no `VideoCaptureCoreX.Start()` god-object — instead you build a directed graph of blocks and run it.

The five concepts you need:

1. **`MediaBlocksPipeline`** — the container. Holds the GStreamer-equivalent runtime, bus, clock, error events. One pipeline per logical scenario.
2. **Source blocks** (`SystemVideoSourceBlock` for camera, `SystemAudioSourceBlock` with `IOSAudioSourceSettings` for the microphone, `UniversalSourceBlock` for files, …) — produce media on output pads.
3. **Transform blocks** (`H264EncoderBlock(new AppleMediaH264EncoderSettings())` for VideoToolbox encoding, `AACEncoderBlock`, `TeeBlock`, …) — accept on input pads, produce on output pads.
4. **Sink blocks** — terminate the graph. **Renderer sinks**: `VideoRendererBlock(pipeline, videoView as IVideoView)` drives the on-screen `VisioForge.Core.UI.Apple.VideoView`. **File sinks**: `MP4SinkBlock`, `MPEGTSSinkBlock` write to the app sandbox. Multi-stream sinks (any muxer) implement `IMediaBlockDynamicInputs` — call `(sink as MP4SinkBlock).CreateNewInput(MediaBlockPadMediaType.Video)` to create the video input pad, again for audio.
5. **Connections** — `pipeline.Connect(producer.Output, consumer.Input)`. For tees and dynamic-input muxers you address `block.Outputs[i]` / the pad created by `CreateNewInput`. Connections must be made before `StartAsync`; reconnecting at runtime is supported by specific blocks (live-source-switch, bridge) but not the general case.

Topology example (preview + MP4 record, the bundled `AppDelegate.cs` build):

```text
SystemVideoSourceBlock  --video-->  TeeBlock(2, Video)  --[0]-->  VideoRendererBlock (UIView VideoView)
                                                         --[1]-->  H264EncoderBlock --> MP4SinkBlock(video pad)
SystemAudioSourceBlock(IOSAudioSourceSettings)  --audio-->  AACEncoderBlock --> MP4SinkBlock(audio pad)
```

`MP4SinkBlock` is the same instance on both audio and video paths — its dynamic inputs collect the streams to mux. Note the audio path has no preview tee — there's no audio renderer in the upstream sample because the iPhone's own AVAudioSession routing is enough; add an `AudioRendererBlock` only if you want monitor-through-headphones playback during capture.

## Mandatory engine boot

Before any `MediaBlocksPipeline` instance is constructed (and before any `DeviceEnumerator` query), call `VisioForgeX.InitSDK()` exactly once per process — synchronous on iOS, unlike the `InitSDKAsync()` used on WPF/MAUI. On a fresh install the first call builds the GStreamer plugin-registry cache inside the app sandbox (~1-3 s, one-time); subsequent launches are instant.

```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    Window = new UIWindow(UIScreen.MainScreen.Bounds);
    var vc = new UIViewController();

    CreateVideoView(vc.View);
    AddButtons(vc.View);

    Window.RootViewController = vc;
    Window.MakeKeyAndVisible();

    VisioForgeX.InitSDK();   // mandatory; before any MediaBlocksPipeline ctor

    InvokeOnMainThread(async () =>
    {
        try { await StartPreview(); }
        catch (Exception ex) { Debug.WriteLine($"Preview start failed: {ex}"); }
    });

    return true;
}
```

There is no public `DestroySDK()` for the iOS redist as of `2025.0.16` — disposing the active `MediaBlocksPipeline` with `await _pipeline.DisposeAsync()` is sufficient at app shutdown. The bundled `references/AppDelegate.cs` shows the canonical placement.

Skipping `InitSDK()` is the #1 source of "Element 'X' not found" failures on first run.

## Info.plist privacy strings (mandatory)

iOS terminates the app process the first time `AVCaptureDevice` / `AVAudioSession` / `Photos` is touched without a matching usage-description key in `Info.plist`. There is no exception, no crash log entry from your code — just an instant kill. App Store reviewers also reject builds where these strings are missing or empty:

```xml
<key>NSCameraUsageDescription</key>
<string>We need access to your camera to record video.</string>
<key>NSMicrophoneUsageDescription</key>
<string>We need access to your microphone to record audio.</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>We save recorded videos to your photo library.</string>
```

Keep the strings user-facing — Apple reviewers read them verbatim. "Required" or "Used by SDK" gets bounced. The bundled `references/Info.plist` ships these three keys with reviewer-safe phrasing; reuse them as-is or rewrite for your app's wording.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += _pipeline_OnError;

// Ship the .vflicense as a bundle resource (Build action: BundleResource).
var path = NSBundle.MainBundle.PathForResource("license", "vflicense");
var cert = File.ReadAllBytes(path);
await _pipeline.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (bundle resource, env var, secrets manager) is your application's choice — there is no built-in licence-loader helper. **Every `MediaBlocksPipeline` instance** in the process needs its own call — the bundled `AppDelegate.cs` constructs a fresh pipeline inside `CreateEngineAsync` on every camera flip / capture toggle, so a real licensed app calls `SetLicenseCertificateAsync` inside that method, not just once at app launch.

The bundled `references/AppDelegate.cs` runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence, add the three lines above into `CreateEngineAsync` right after `_pipeline = new MediaBlocksPipeline();` and before the `_videoSource = ...` block.

## Hello-World pipeline

The bundled `references/AppDelegate.cs` is the full template — preview-only and record graph builders, front/back camera flip, capture-button state machine, photo-library save via `PHAssetCreationRequest`. Drop it in alongside `references/Main.cs` / `references/Toast.cs` / `references/Sample.csproj`, set the three Info.plist keys, and the project builds + runs on a physical iPhone.

The minimal preview-only wiring inside `CreateEngineAsync(false)` is:

```csharp
_pipeline = new MediaBlocksPipeline();
_pipeline.OnError += _pipeline_OnError;

var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
var device = cameras[_cameraIndex];
var fmt = device.GetHDVideoFormatAndFrameRate(out var frameRate);
var settings = new VideoCaptureDeviceSourceSettings(device) { Format = fmt.ToFormat() };
settings.Format.FrameRate = frameRate;

_videoSource = new SystemVideoSourceBlock(settings);
_videoRenderer = new VideoRendererBlock(_pipeline, _videoView as IVideoView) { IsSync = false };

_pipeline.Connect(_videoSource.Output, _videoRenderer.Input);

await _pipeline.StartAsync();
```

`IsSync = false` is the standard iOS preview setting — frames render as fast as they arrive instead of being clock-throttled (which can stutter on capture sources whose timestamps drift against the system clock). To extend to record, insert a `TeeBlock(2, MediaBlockPadMediaType.Video)` between source and renderer, send `_videoTee.Outputs[1]` into an `H264EncoderBlock(new AppleMediaH264EncoderSettings())` → `(sink as MP4SinkBlock).CreateNewInput(MediaBlockPadMediaType.Video)`, and add a `SystemAudioSourceBlock(new IOSAudioSourceSettings())` → `AACEncoderBlock` → audio pad on the same `MP4SinkBlock`. The full version is in the bundled `AppDelegate.cs`.

## Common iOS failures

These are the five most common production issues — flag any of them on first run.

### 1. App silently terminates the first time the camera is touched

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Info.plist`. iOS kills the process on the first hardware access without a matching key. No exception, no crash log entry from your code, no clue in the Visual Studio debug output — just an instant exit.

**Fix**: add both keys with non-empty, user-facing strings (see "Info.plist privacy strings" above). Re-deploy — iOS does NOT re-prompt once the key was already missing on a previous launch; you may need to delete the app from the device first.

### 2. `ExecutionEngineException: Attempting to JIT compile method ... in aot-only mode`

**Cause**: `<MtouchInterpreter>all</MtouchInterpreter>` is missing from the active build configuration. Media Blocks SDK's `VisioForgeX.InitSDK()` path uses `DynamicMethod` and late-bound generic instantiation, both of which the iOS AOT compiler refuses. Throws from the very first SDK call.

**Fix**: set `<MtouchInterpreter>all</MtouchInterpreter>` and `<MtouchLink>SdkOnly</MtouchLink>` on **both** Debug and Release `<PropertyGroup>` blocks (see csproj snippet above). Release builds are not exempt — same error fires on a TestFlight build too.

### 3. No camera devices, even on a fresh launch

**Cause A — simulator**: the iOS Simulator does not expose a camera device. `DeviceEnumerator.Shared.VideoSourcesAsync()` returns an empty array. There is no workaround in the SDK — iOS Simulator has never supported AVCaptureDevice video.

**Fix**: deploy to a physical iPhone or iPad. Use the simulator only for non-camera UI work.

**Cause B — privacy denied**: the user denied the camera prompt on a previous launch and the app didn't re-prompt. iOS does not show the prompt twice; once denied, the app gets an empty device list until the user re-enables it manually in Settings → Privacy & Security → Camera.

**Fix**: when `VideoSourcesAsync()` returns empty on a physical device, surface a UI message linking to `UIApplication.OpenSettingsUrlString` rather than silently failing.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded — either nothing was loaded at all (trial mode runs silently for the first 30 days), or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaBlocksPipeline` instance than the one being started. The bundled sample creates a fresh pipeline on every camera flip / capture toggle, so a single one-time licence call in `FinishedLaunching` won't cover later pipelines.

**Fix**: ship the `.vflicense` as a bundle resource (Build Action = `BundleResource`), read it via `NSBundle.MainBundle.PathForResource("license", "vflicense")` + `File.ReadAllBytes`, and call `await _pipeline.SetLicenseCertificateAsync(certBytes)` inside `CreateEngineAsync` after the constructor and before any `Connect` calls (see "License registration" above).

### 5. Pipeline `OnError` fires with "Element 'X' not found" / preview is black

**Cause A — SDK not initialised**: forgot `VisioForgeX.InitSDK()` in `FinishedLaunching` before pipeline construction. The plugin registry isn't built, so encoder/sink blocks fail to find their backing GStreamer elements when `StartAsync` runs.

**Cause B — VideoView attached too late**: `VideoRendererBlock` was constructed and `pipeline.StartAsync` ran before the `VideoView` (`VisioForge.Core.UI.Apple.VideoView`) was added as a subview of a `UIViewController` whose view was already realised. `loadView` hasn't fired, the Metal drawable has no backing surface, and `AutoresizingMask` resizing the UIView later does NOT recover the surface.

**Cause C — `IsSync = true` on a drifting source**: leaving `VideoRendererBlock.IsSync` at its default (true) clock-throttles renderer frames against the pipeline clock. iOS camera sources whose timestamps drift against `clock_gettime` cause the renderer to drop or freeze frames waiting for clock sync — the LED is on, the encoder writes a valid MP4, but preview stays black.

**Fix A**: call `VisioForgeX.InitSDK()` exactly once in `FinishedLaunching` *before* any `MediaBlocksPipeline` constructor.

**Fix B**: assign `Window.RootViewController = vc` and call `Window.MakeKeyAndVisible()` **before** building the subview hierarchy that contains your `VideoView`. The bundled `AppDelegate.cs` shows the correct order.

**Fix C**: set `_videoRenderer = new VideoRendererBlock(_pipeline, _videoView as IVideoView) { IsSync = false };` on every renderer in iOS pipelines that pull from a live capture source.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -c Debug` succeeds with the bundled `references/` files copied into a fresh `net10.0-ios` project (no missing-DLL warnings).
- [ ] `Info.plist` contains all three privacy keys (`NSCameraUsageDescription`, `NSMicrophoneUsageDescription`, `NSPhotoLibraryUsageDescription`) with reviewer-safe strings.
- [ ] First run on a physical iPhone shows the camera/mic permission prompts, then a working preview within ~2 s after grant.
- [ ] Camera-flip and capture toggle do not leak — every transition calls `await _pipeline.StopAsync()` → `await _pipeline.DisposeAsync()` → fresh `CreateEngineAsync` (the bundled `StopCamera` / `DestroyEngineAsync` show this).
- [ ] On clean app suspend / terminate, recordings finalise correctly (verify the MP4 plays back in the Photos app or another player).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called inside `CreateEngineAsync` (NOT just once in `FinishedLaunching`) so every fresh `MediaBlocksPipeline` gets registered before `StartAsync`.
- [ ] `MtouchInterpreter=all` is set on both Debug and Release configurations — confirm by checking the build log for `--interpreter`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder (alongside an `Assets.xcassets`, `LaunchScreen.storyboard`, and `Entitlements.plist` from the standard .NET for iOS template) and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-ios` csproj, version-pinned to the same NuGet release as the prose, with `MtouchInterpreter=all` on both Debug and Release.
- `references/Info.plist` — bundle metadata with the three mandatory privacy-usage keys filled in with reviewer-safe wording.
- `references/AppDelegate.cs` — full code-behind: `VisioForgeX.InitSDK` boot, `DeviceEnumerator` wiring, source → tee → renderer + H.264/AAC → MP4 graph builders, front/back camera flip, capture-button state machine, photo-library save via `PHAssetCreationRequest`. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence — see "License registration" for the exact insertion point inside `CreateEngineAsync`.) This sample builds the root `UIViewController` inline inside `FinishedLaunching` rather than a separate `MainViewController.cs` file — the camera preview and controls live directly on `vc.View`.
- `references/Toast.cs` — small UIKit toast helper used by `AppDelegate.cs` for permission-denied / no-device messages.
- `references/Main.cs` — top-level `UIApplication.Main(args, null, typeof(AppDelegate))` entry point.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/iOS>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-ios` — high-level capture-and-record API on native iOS; use this if you don't need a custom pipeline.
    - `media-blocks-sdk-net-android` — same SDK on native Android (`net10.0-android`).
    - `media-blocks-sdk-net-maui` — same SDK on .NET MAUI (one project shared across iOS / Android / Windows / Mac Catalyst).
    - `media-blocks-sdk-net-macos` — same SDK on native macOS (AppKit / `net10.0-macos`).
    - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (cross-platform desktop + mobile).
    - `media-blocks-sdk-net-uno` — same SDK on Uno Platform.
