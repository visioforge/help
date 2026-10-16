---
name: video-capture-sdk-x-ios
description: Integrate VisioForge Video Capture SDK X (cross-platform edition) into a native .NET for iOS application. Covers the iOS-specific VideoView control, the cross-platform NuGet package, Info.plist NSCameraUsageDescription / NSMicrophoneUsageDescription requirements, license registration, and the most common iOS pitfalls (Info.plist missing usage descriptions, no camera in simulator, AOT JIT-only ExecutionEngineException, App Store reviewer rejecting missing privacy strings, trial-period expiry / unlicensed build). Use for native .NET for iOS (NOT MAUI / Xamarin) capture apps — for cross-OS MAUI use video-capture-sdk-x-maui.
---

# Video Capture SDK X — native .NET for iOS integration

This skill helps you add **VisioForge Video Capture SDK X** — the cross-platform "X" edition of the capture SDK — to a **native .NET for iOS** application (`net10.0-ios` TFM, UIKit, no MAUI/Xamarin shell). The X SDK shares its runtime with Media Blocks (GStreamer-backed under the hood) and exposes the high-level `VideoCaptureCoreX` god-object that mirrors the legacy `VideoCaptureCore` API. Same C# capture/recording code ports to Windows / macOS / Android / MAUI / Avalonia / Uno — only the UI host changes.

Pinned NuGet versions: wrapper **`VisioForge.DotNet.VideoCapture` 2026.10.15**, native iOS redist at the version shown in the csproj below (matches the [official Simple Video Capture X iOS sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/iOS/SimpleVideoCapture)). Keep the wrapper pinned to one version and pin each redist to the newest version published for that package at or before your wrapper's release - the redists are built on their own cadence, so check nuget.org rather than assuming the wrapper's number exists for them, and never let a redist run ahead of the wrapper.

## When to use this skill

- Adding webcam (front/back), screen, or photo capture to a **native .NET for iOS** app (`net10.0-ios`) with UIKit.
- Recording captured video/audio to MP4 directly in the app sandbox, with optional copy to the Photos library.
- Sharing capture/recording code between an iOS native app and Windows/Android/MAUI companions — `VideoCaptureCoreX` is API-identical across hosts.

## When NOT to use this skill

- **MAUI / Xamarin** (one C# project shared across iOS + Android + Windows + Mac Catalyst): use `video-capture-sdk-x-maui` — the per-OS NuGet wiring and `<UseInterpreter>` placement are different, and MAUI ships its own `VideoView` handler.
- **macOS native (`net10.0-macos`)** with AppKit/NSWindow: use `video-capture-sdk-x-macos` — different redist (`VisioForge.CrossPlatform.Core.macOS`), different `VideoView` namespace, no `MtouchInterpreter`.
- **Android native (`net10.0-android`)**: use `video-capture-sdk-x-android` — Android also requires a `<ProjectReference>` to a small `VisioForge.Core.Android.X10.csproj` companion project.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode without preview, runtime sink swap): use `media-blocks-sdk-net-maui` — `VideoCaptureCoreX` is the high-level wrapper; for full graph control go one layer down.
- **Playback only** (play files / streams without capturing): `media-player-sdk-x-ios`.

## Project setup

### Target framework

Use `net10.0-ios` with `<SupportedOSPlatformVersion>16.4</SupportedOSPlatformVersion>` — VisioForge's native iOS libraries (`VisioForge.CrossPlatform.Core.iOS`) are built for iOS 16.4 and do not load on anything older (the .NET 10 iOS workload itself refuses anything below 15.0), and the upstream sample csproj is at 16.4 as well.

### NuGet packages

Two packages — the .NET wrapper plus the iOS native redist. The redist is **not** transitive; you must reference it explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.10.15" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2026.10.14" />
</ItemGroup>
```

`VisioForge.DotNet.VideoCapture` is the **same wrapper package** the legacy SDK uses — both `VideoCaptureCore` (legacy, Windows-only) and `VideoCaptureCoreX` (cross-platform) ship in it, but only `VideoCaptureCoreX` is reachable from `net10.0-ios`. What switches you to the X engine on iOS is the `VisioForge.CrossPlatform.Core.iOS` redist plus the `VisioForgeX.InitSDK()` boot below.

There is no `.x64` / `.x86` split on iOS — Apple Silicon device + simulator slices are bundled in the single redist NuGet.

### Mandatory Mono interpreter

The X SDK uses dynamic codegen paths (`DynamicMethod`, late-bound generic instantiations in the GStreamer glue) that the iOS AOT compiler refuses to emit. Without the Mono interpreter enabled, the very first `VisioForgeX.InitSDK()` call throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode` — exact same root cause as MAUI iOS, but the property names differ for native .NET for iOS:

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

`MtouchInterpreter=all` is the native iOS equivalent of MAUI's `<UseInterpreter>true</UseInterpreter>`. It applies to both Debug and Release — Release builds are not exempt. Tiny startup cost, fixes the JIT failure.

### Full minimal csproj

See `references/Sample.csproj` in the bundled archive. Adapted from the official Simple Video Capture X iOS sample (`_DEMOS/Video Capture SDK X/iOS/SimpleVideoCapture/`) — kept verbatim except for stripping demo-only metadata; the bundled file builds standalone against the public NuGet packages.

## Mandatory engine boot

Before any `VideoCaptureCoreX` instance is constructed (and before any `DeviceEnumerator` query), call `VisioForgeX.InitSDK()` exactly once per process — synchronous on iOS, unlike the `InitSDKAsync()` used on WPF/MAUI. On a fresh install the first call builds the GStreamer plugin-registry cache inside the app sandbox (~1-3 s, one-time); subsequent launches are instant.

iOS 27 requires the scene lifecycle: `Info.plist` needs the `UIApplicationSceneManifest` pointing at `SceneDelegate` (bundled `references/Info.plist` and `references/SceneDelegate.cs`), and an app that still builds its window in `FinishedLaunching` without the manifest is killed at launch on iOS 27 before any UI appears. The window therefore comes from the connecting `UIWindowScene`, and the UI plus the SDK boot live in `AppDelegate.SetupWindow`:

```csharp
// SceneDelegate.cs — iOS hands the app a UIWindowScene; the window is built from it.
[Export("scene:willConnectToSession:options:")]
public void WillConnect(UIScene scene, UISceneSession session, UISceneConnectionOptions connectionOptions)
{
    if (scene is not UIWindowScene windowScene)
    {
        return;
    }

    var app = (AppDelegate)UIApplication.SharedApplication.Delegate;
    Window = app.Window ?? new UIWindow(windowScene);
    if (app.Window is null)
    {
        app.SetupWindow(Window);
    }
    else
    {
        // A scene that reconnects after a disconnect gets the existing window back.
        Window.WindowScene = windowScene;
        Window.MakeKeyAndVisible();
    }
}

// AppDelegate.cs
public void SetupWindow(UIWindow window)
{
    Window = window;
    var vc = new UIViewController();
    Window.RootViewController = vc;
    Window.MakeKeyAndVisible();
    vc.View.LayoutIfNeeded();   // see "Black preview" pitfall below

    CreateVideoView(vc.View);
    AddButtons(vc.View);

    VisioForgeX.InitSDK();      // <-- mandatory; before any VideoCaptureCoreX

    InvokeOnMainThread(async () =>
    {
        try { await StartAsync(); }
        catch (Exception ex) { Debug.WriteLine($"Capture start failed: {ex}"); }
    });
}
```

At app shutdown, stop and dispose the `VideoCaptureCoreX` instance, then call `VisioForgeX.DestroySDK()` once. `WillTerminate` is synchronous, so use the sync `Stop()` / `Dispose()` overloads there — an `await` in it never completes before the process exits. The bundled `references/AppDelegate.cs` has no shutdown hook; add one in `WillTerminate`.

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

For commercial license types, scope, updates, support, and trial terms, see the [canonical VisioForge licensing page](https://www.visioforge.com/licensing).

The SDK ships with a 30-day trial. To register a purchased licence, call `await VideoCapture1.SetLicenseCertificateAsync(certBytes)` on every `VideoCaptureCoreX` instance, after the constructor and before `StartAsync`:

```csharp
_player = new VideoCaptureCoreX(_videoView as IVideoView);
_player.OnError += PlayerOnError;

// Ship the .vflicense as a bundle resource (Build action: BundleResource).
// NSBundle.MainBundle.PathForResource gives a file path inside the .app sandbox.
var path = NSBundle.MainBundle.PathForResource("license", "vflicense");
var cert = File.ReadAllBytes(path);
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (bundle resource, env var, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/AppDelegate.cs` runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence, add the `PathForResource` + `ReadAllBytes` + `SetLicenseCertificateAsync` lines into `CreateEngineAsync()` right after `_player = new VideoCaptureCoreX(...)`.

## Hello-World capture

The bundled `references/AppDelegate.cs` is the full template — front/back camera flip, MP4 recording with VideoToolbox H.264, photo-library save, capture-button state machine. Drop it in alongside `references/Main.cs` and `references/Sample.csproj`, set the three Info.plist keys, and the project builds + runs on a physical iPhone.

The minimal capture wiring inside `CreateEngineAsync` is three statements:

```csharp
_player = new VideoCaptureCoreX(_videoView as IVideoView);

// VideoCaptureCoreX defaults Video_Play to false. Without this the pipeline
// runs (capture session is up, camera LED lights) but the video tee is never
// connected to the renderer bound to _videoView, so preview stays black.
_player.Video_Play = true;

var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
var device = cameras[0];
var fmt = device.GetHDVideoFormatAndFrameRate(out var frameRate);
var settings = new VideoCaptureDeviceSourceSettings(device) { Format = fmt.ToFormat() };
settings.Format.FrameRate = frameRate;
_player.Video_Source = settings;

await _player.StartAsync();
```

For recording add an `MP4Output` to `Outputs_Add` before `StartAsync`, then toggle with `StartCaptureAsync(0, filename)` / `StopCaptureAsync(0)`. The full pattern is in `references/AppDelegate.cs`.

## Common iOS failures

These are the five most common production issues — flag any of them on first run.

### 1. App silently terminates the first time the camera is touched

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Info.plist`. iOS kills the process on the first hardware access without a matching key. No exception, no crash log entry from your code, no clue in the Visual Studio debug output — just an instant exit.

**Fix**: add both keys with non-empty, user-facing strings (see "Info.plist privacy strings" above). Re-deploy — iOS does NOT re-prompt once the key was already missing on a previous launch; you may need to delete the app from the device first.

### 2. `ExecutionEngineException: Attempting to JIT compile method ... in aot-only mode`

**Cause**: `<MtouchInterpreter>all</MtouchInterpreter>` is missing from the active build configuration. The X SDK's `VisioForgeX.InitSDK()` path uses `DynamicMethod` and late-bound generic instantiation, both of which the iOS AOT compiler refuses. Throws from the very first SDK call.

**Fix**: set `<MtouchInterpreter>all</MtouchInterpreter>` and `<MtouchLink>SdkOnly</MtouchLink>` on **both** Debug and Release `<PropertyGroup>` blocks (see csproj snippet above). Release builds are not exempt — same error fires on a TestFlight build too.

### 3. No camera devices, even on a fresh launch

**Cause A — simulator**: the iOS Simulator does not expose a camera device. `DeviceEnumerator.Shared.VideoSourcesAsync()` returns an empty array. There is no workaround in the SDK — iOS Simulator has never supported AVCaptureDevice video.

**Fix**: deploy to a physical iPhone or iPad. Use the simulator only for non-camera UI work.

**Cause B — privacy denied**: the user denied the camera prompt on a previous launch and the app didn't re-prompt. iOS does not show the prompt twice; once denied, the app gets an empty device list until the user re-enables it manually in Settings → Privacy & Security → Camera.

**Fix**: when `VideoSourcesAsync()` returns empty on a physical device, surface a UI message linking to `UIApplication.OpenSettingsUrlString` rather than silently failing.

### 4. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded — either nothing was loaded, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `VideoCaptureCoreX` instance than the one being started.

**Fix**: ship the `.vflicense` as a bundle resource (Build Action = `BundleResource`), read it via `NSBundle.MainBundle.PathForResource("license", "vflicense")` + `File.ReadAllBytes`, and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `StartAsync` (see "License registration" above). Every `VideoCaptureCoreX` instance in the process needs its own call.

### 5. Camera LED lights up but preview stays black

**Cause A**: forgot `_player.Video_Play = true` after the constructor. `VideoCaptureCoreX.Video_Play` defaults to `false` on iOS — the pipeline runs (capture session is up), but the video tee is never connected to the `IVideoView` renderer.

**Cause B — VideoView not sized to the window**: the `VideoView` (`VisioForge.Core.UI.Apple.VideoView`) was created with a frame other than the area it is shown in — for example a fixed or zero `CGRect`, or on a view that was later resized without `AutoresizingMask`/constraints on the `VideoView`. Check `_videoView.Bounds` and `DrawableSize` after the window appears; both should match the visible area.

**Fix A**: set `_player.Video_Play = true` immediately after `new VideoCaptureCoreX(...)`.

**Fix B**: create the `VideoView` from `vc.View.Bounds` with `AutoresizingMask = FlexibleWidth | FlexibleHeight` (or pin it with constraints). The bundled `AppDelegate.cs` installs the VC (`Window.RootViewController = vc`, `Window.MakeKeyAndVisible()`, `vc.View.LayoutIfNeeded()`) before building the hierarchy — the recommended order.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -c Debug` succeeds with the bundled `references/` files copied into a fresh `net10.0-ios` project (no missing-DLL warnings).
- [ ] `Info.plist` contains all three privacy keys (`NSCameraUsageDescription`, `NSMicrophoneUsageDescription`, `NSPhotoLibraryUsageDescription`) with reviewer-safe strings.
- [ ] `Info.plist` has `UIApplicationSceneManifest` → `SceneDelegate` (iOS 27 kills a scene-less app at launch).
- [ ] First run on a physical iPhone shows the camera/mic permission prompts, then a working preview within ~2 s after grant.
- [ ] Camera-flip button toggles between front / back without leaking `VideoCaptureCoreX` (every flip calls `StopCaptureAsync(0)` → `StopAsync()` → `DisposeAsync()` → fresh `CreateEngineAsync`).
- [ ] On clean app suspend / terminate, recordings finalise correctly (verify the MP4 plays back in the Photos app or another player).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `VideoCaptureCoreX` instance before its `StartAsync` (otherwise the app silently runs in 30-day trial mode).
- [ ] `MtouchInterpreter=all` is set on both Debug and Release configurations — confirm by checking the build log for `--interpreter`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder (alongside an `Assets.xcassets`, `LaunchScreen.storyboard`, and `Entitlements.plist` from the standard .NET for iOS template) and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-ios` csproj, version-pinned to the same NuGet release as the prose, with `MtouchInterpreter=all` on both Debug and Release.
- `references/Info.plist` — bundle metadata with the `UIApplicationSceneManifest` that points iOS at `SceneDelegate` (required — iOS 27 kills a scene-less app at launch), and the three mandatory privacy-usage keys filled in with reviewer-safe wording.
- `references/AppDelegate.cs` — full code-behind: `VisioForgeX.InitSDK` boot, `DeviceEnumerator` wiring, MP4 recording with VideoToolbox H.264, front/back camera flip, capture-button state machine, photo-library save via `PHAssetCreationRequest`. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.) This sample builds the root `UIViewController` inline inside `SetupWindow` rather than a separate `MainViewController.cs` file — the camera preview and controls live directly on `vc.View`.
- `references/SceneDelegate.cs` — creates the window from the connecting `UIWindowScene` and calls `AppDelegate.SetupWindow`; the scene lifecycle iOS 27 requires.
- `references/Main.cs` — top-level `UIApplication.Main(args, null, typeof(AppDelegate))` entry point.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videocapture/>
- **Product page**: <https://www.visioforge.com/video-capture-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/iOS>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-capture-sdk-x-macos` — same SDK on native macOS (AppKit / `net10.0-macos`).
    - `video-capture-sdk-x-android` — same SDK on native Android (`net10.0-android`).
    - `video-capture-sdk-x-maui` — same SDK on .NET MAUI (one project shared across iOS / Android / Windows / Mac Catalyst).
    - `video-capture-sdk-x-wpf` — same SDK on Windows WPF (desktop preview / record).
    - **Lower-level pipeline**:
        - `media-blocks-sdk-net-ios` — same engine, lower-level graph-based API for custom pipeline topologies on native iOS.
