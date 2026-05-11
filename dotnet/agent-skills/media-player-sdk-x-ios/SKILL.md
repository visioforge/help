---
name: media-player-sdk-x-ios
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into a native .NET for iOS application. Covers the iOS-specific VideoView control, the cross-platform NuGet package, Info.plist usage descriptions, license registration, AOT JIT-only ExecutionEngineException, and the most common iOS pitfalls (App Transport Security for HTTP streams, format support, trial-period expiry / unlicensed build). Use for native .NET for iOS playback apps — for cross-OS MAUI use media-player-sdk-x-maui.
---

# Media Player SDK X — native .NET for iOS integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to a **native .NET for iOS** application (`net10.0-ios` TFM, UIKit, no MAUI/Xamarin shell). The X SDK shares its runtime with Media Blocks and Video Capture X (GStreamer-backed under the hood) and exposes the high-level `MediaPlayerCoreX` god-object that mirrors the legacy `MediaPlayerCore` API. Same C# playback code ports to Windows / macOS / Android / MAUI / Avalonia / Uno — only the UI host changes.

Pinned NuGet versions: wrapper **`VisioForge.DotNet.MediaPlayer` 2026.5.4**, native iOS redist **`VisioForge.CrossPlatform.Core.iOS` 2025.0.16** (matches the upstream Media Player SDK X iOS sample at `_DEMOS/Media Player SDK X/iOS/MediaPlayer/`). The iOS redist version trails the wrapper version on purpose — it tracks the underlying GStreamer-iOS rebuild cadence, which has slower release tempo than the managed wrapper. Pin both to the values shipped in the upstream csproj for the wrapper version you're using; do not blindly bump the redist to match the wrapper.

## When to use this skill

- Adding video / audio file playback to a **native .NET for iOS** app (`net10.0-ios`) with UIKit.
- Playing local media (MP4, MKV, MOV, M4A, MP3, AAC, WAV, FLAC, OGG, …) selected via `UIDocumentPickerViewController`.
- Playing network streams (HTTPS, HLS, DASH, RTSP, SRT) via the `UniversalSourceSettings` URI source. Plain-HTTP streams need an Info.plist ATS exception (see "App Transport Security" below).
- Multi-stream titles: enumerate via `Video_Streams` / `Audio_Streams`, switch live with `Video_Stream_Select` / `Audio_Stream_Select`.
- Sharing playback code between an iOS native app and Windows/Android/MAUI companions — `MediaPlayerCoreX` is API-identical across hosts.

## When NOT to use this skill

- **MAUI / Xamarin** (one C# project shared across iOS + Android + Windows + Mac Catalyst): use `media-player-sdk-x-maui` — the per-OS NuGet wiring and `<UseInterpreter>` placement are different, and MAUI ships its own `VideoView` handler.
- **macOS native (`net10.0-macos`)** with AppKit/NSWindow: use `media-player-sdk-x-macos` — different redist (`VisioForge.CrossPlatform.Core.macOS`), different `VideoView` namespace, no `MtouchInterpreter`.
- **Android native (`net10.0-android`)**: use `media-player-sdk-x-android` — Android also requires a `<ProjectReference>` to a small `VisioForge.Core.Android.X10.csproj` companion project.
- **Custom pipeline topology** (split-with-tee, multi-source mix, transcode-while-playing, runtime sink swap): use `media-blocks-sdk-net-maui` — `MediaPlayerCoreX` is the high-level wrapper; for full graph control go one layer down.
- **Capture / recording** instead of playback: use `video-capture-sdk-x-ios`. The two SDKs ship side-by-side on iOS (same redist, different wrapper assembly) and can coexist in one app.

## Project setup

### Target framework

Use `net10.0-ios` with `<SupportedOSPlatformVersion>14.0</SupportedOSPlatformVersion>` — that's the floor the SDK is tested against. The upstream sample csproj currently sets `13.0`, but the H.264 / HEVC VideoToolbox decode path the iOS redist relies on was finalised at iOS 14 and 13.x slices may surface stutter on first decode. Bump to 14.0 unless you have a hard requirement for older devices.

### NuGet packages

Two packages — the .NET wrapper plus the iOS native redist. The redist is **not** transitive; you must reference it explicitly:

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
</ItemGroup>
<ItemGroup>
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
```

`VisioForge.DotNet.MediaPlayer` is the wrapper that ships `MediaPlayerCoreX` and the iOS-flavour `VisioForge.Core.UI.Apple.VideoView`. What switches you to the X engine on iOS is the `VisioForge.CrossPlatform.Core.iOS` redist plus the `VisioForgeX.InitSDK()` boot below.

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

`MtouchInterpreter=all` is the native iOS equivalent of MAUI's `<UseInterpreter>true</UseInterpreter>`. It applies to both Debug and Release — Release builds are not exempt, and TestFlight builds compiled without it will throw the same `ExecutionEngineException` on first launch. Tiny startup cost, fixes the JIT failure.

### Full minimal csproj

See `references/Sample.csproj`. Adapted from the official Media Player SDK X iOS sample (`_DEMOS/Media Player SDK X/iOS/MediaPlayer/MediaPlayerX.csproj`) — kept verbatim except for replacing the source-tree `<ProjectReference>`s with public `<PackageReference>`s and bumping `SupportedOSPlatformVersion` from 13.0 to 14.0. The bundled file builds standalone against the public NuGet packages.

## Mandatory engine boot

Before any `MediaPlayerCoreX` instance is constructed, call `VisioForgeX.InitSDK()` exactly once per process — synchronous on iOS, unlike the `InitSDKAsync()` used on WPF/MAUI. On a fresh install the first call builds the GStreamer plugin-registry cache inside the app sandbox (~1-3 s, one-time); subsequent launches are instant.

```csharp
public override bool FinishedLaunching(UIApplication application, NSDictionary launchOptions)
{
    Window = new UIWindow(UIScreen.MainScreen.Bounds);

    // CustomViewController.ctor builds the VideoView at full-window size.
    _vc = new CustomViewController(Window, out _videoView);

    // ...wire up button handlers...

    Window.RootViewController = _vc;
    Window.MakeKeyAndVisible();

    VisioForgeX.InitSDK();   // <-- mandatory; before any MediaPlayerCoreX

    return true;
}
```

There is no public `DestroySDK()` for the iOS redist as of 2025.0.16 — disposing the `MediaPlayerCoreX` instance with `await _player.DisposeAsync()` is sufficient at app shutdown. The bundled `references/AppDelegate.cs` shows the canonical placement.

Skipping `InitSDK()` is the #1 source of "Element 'X' not found" failures on first run.

## Info.plist privacy strings

The Media Player SDK itself does not touch the camera, microphone, or location, so the only privacy string needed is the one for the **media source**:

```xml
<key>NSPhotoLibraryUsageDescription</key>
<string>We need access to your photo library to play media files.</string>
```

This is required because:

- the file picker is allowed to return Photos URLs;
- the bundled `AppDelegate.cs` calls `PHPhotoLibrary.RequestAuthorization` at launch.

If your app reads media exclusively via `UIDocumentPickerViewController` from the Files app and never touches `PHPhotoLibrary`, the key is technically optional — but App Store reviewers occasionally flag the missing key on apps that link Photos at all, so leaving it in (with reviewer-safe wording) is the safer default.

The bundled `references/Info.plist` ships this key with reviewer-safe phrasing; reuse as-is or rewrite for your app's wording.

## App Transport Security (HTTP streams)

iOS blocks plain-HTTP network connections by default. Playing an HTTPS / HLS-over-HTTPS / RTSP / SRT URL works out of the box. Playing a plain `http://` URL fails silently (no exception, no `OnError`, just a never-firing `OnStart` and a black surface) until you add an ATS exception:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSAllowsArbitraryLoads</key>
    <true/>
</dict>
```

App Store reviewers flag `NSAllowsArbitraryLoads=true` and request a justification ("legacy server X cannot be reached via HTTPS"). For production, prefer per-domain `NSExceptionDomains` over a blanket allow:

```xml
<key>NSAppTransportSecurity</key>
<dict>
    <key>NSExceptionDomains</key>
    <dict>
        <key>legacy.example.com</key>
        <dict>
            <key>NSExceptionAllowsInsecureHTTPLoads</key>
            <true/>
        </dict>
    </dict>
</dict>
```

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync` / `PlayAsync`:

```csharp
_player = new MediaPlayerCoreX(_videoView as IVideoView);
_player.OnError += PlayerOnError;

// Ship the .vflicense as a bundle resource (Build action: BundleResource).
// NSBundle.MainBundle.PathForResource gives a file path inside the .app sandbox.
var path = NSBundle.MainBundle.PathForResource("license", "vflicense");
var cert = File.ReadAllBytes(path);
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x — older `SetLicenseCertificate(string)` / `SetLicenseCertificate(Stream)` overloads were removed in 2026.5.2. Where the bytes come from (bundle resource, env var, secrets manager) is your application's choice — there is no built-in licence-loader helper.

The bundled `references/AppDelegate.cs` runs in 30-day trial mode by design (no `SetLicenseCertificateAsync` call). To register a purchased licence, add the `PathForResource` + `ReadAllBytes` + `SetLicenseCertificateAsync` lines into `CreateEngineAsync()` right after `_player = new MediaPlayerCoreX(...)` and before `await _player.OpenAsync(sourceSettings)`.

## Hello-World playback

The bundled `references/AppDelegate.cs` + `references/CustomViewController.cs` are the full template — `UIDocumentPickerViewController` file picker, `UniversalSourceSettings` URL probe, video / audio stream auto-detection, position-slider seeking with reentrancy guard, `OnStart` / `OnStop` / `OnError` wiring. Drop them in alongside `references/Main.cs` (the `UIApplication.Main(...)` entry point) and `references/Sample.csproj`, set the Info.plist key, and the project builds + runs on a physical iPhone or the simulator (audio-only files in the simulator).

The minimal playback wiring inside `CreateEngineAsync` is four statements:

```csharp
_player = new MediaPlayerCoreX(_videoView as IVideoView);

// Probe the URL up front so we only enable Video_Play / Audio_Play for streams
// that actually exist. On audio-only files, leaving Video_Play=true wires a
// dead video tee — harmless but wastes time on first-frame.
var sourceSettings = await UniversalSourceSettings.CreateAsync(uri);
_player.Video_Play = sourceSettings.GetInfo().VideoStreams.Count > 0;
_player.Audio_Play = sourceSettings.GetInfo().AudioStreams.Count > 0;

await _player.OpenAsync(sourceSettings);
await _player.PlayAsync();
```

Seek with `await _player.Position_SetAsync(TimeSpan.FromSeconds(t), seekToKeyframe: true)`. The full pattern is in `references/AppDelegate.cs`.

## Common iOS failures

These are the five most common production issues — flag any of them on first run.

### 1. `ExecutionEngineException: Attempting to JIT compile method ... in aot-only mode`

**Cause**: `<MtouchInterpreter>all</MtouchInterpreter>` is missing from the active build configuration. The X SDK's `VisioForgeX.InitSDK()` path uses `DynamicMethod` and late-bound generic instantiation, both of which the iOS AOT compiler refuses. Throws from the very first SDK call.

**Fix**: set `<MtouchInterpreter>all</MtouchInterpreter>` and `<MtouchLink>SdkOnly</MtouchLink>` on **both** Debug and Release `<PropertyGroup>` blocks (see csproj snippet above). Release builds are not exempt — same error fires on a TestFlight build too.

### 2. Black surface, no `OnStart`, no `OnError` — silent stall on first PlayAsync

**Cause A — plain-HTTP URL with default ATS**: iOS swallows the connection at the network layer. The pipeline starts the source, the source never connects, and `MediaPlayerCoreX` never reaches the prerolled state. No exception fires.

**Fix A**: switch the URL to HTTPS, or add `NSExceptionAllowsInsecureHTTPLoads` for the specific host (preferred over blanket `NSAllowsArbitraryLoads`). See "App Transport Security" above.

**Cause B — VideoView built before window was key-and-visible**: the VideoView's `MTKView` grabs its Metal drawable lazily; if you build it on an orphan UIViewController (one not yet assigned to `Window.RootViewController`), `loadView` fires too early, the drawable never gets a backing surface, and `AutoresizingMask` resizing the UIView later does NOT recover the surface.

**Fix B**: assign `Window.RootViewController = vc` and call `Window.MakeKeyAndVisible()` **before** constructing `MediaPlayerCoreX(_videoView as IVideoView)`. The bundled `AppDelegate.cs` shows the correct order.

**Cause C — forgot `Video_Play = true` on a video file**: unlike capture, `MediaPlayerCoreX` defaults `Video_Play` to `true`, so this is rare on player. But if you explicitly set `Video_Play = false` (e.g. you copy-pasted from a VideoCaptureCoreX sample), the audio plays through but the surface stays black. Re-check the auto-detection from `UniversalSourceSettings.GetInfo()`.

### 3. Trial-mode message (or "SDK TRIAL period (30 days) is over") on startup

**Cause**: no `.vflicense` certificate has been loaded — either nothing was loaded, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being started.

**Fix**: ship the `.vflicense` as a bundle resource (Build Action = `BundleResource`), read it via `NSBundle.MainBundle.PathForResource("license", "vflicense")` + `File.ReadAllBytes`, and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync` / `PlayAsync` (see "License registration" above). Every `MediaPlayerCoreX` instance in the process needs its own call.

### 4. "Element 'X' not found" / `OnError` fires immediately on `OpenAsync`

**Cause A — `InitSDK()` skipped**: the GStreamer plugin-registry cache was never built, so element factories like `decodebin3` / `videoconvert` / `glimagesink` cannot be created.

**Fix A**: call `VisioForgeX.InitSDK()` exactly once in `FinishedLaunching` after `Window.MakeKeyAndVisible()` and before any `MediaPlayerCoreX` construction. See "Mandatory engine boot" above.

**Cause B — exotic codec the iOS redist doesn't ship**: the iOS redist is a slimmed GStreamer build (no x265, no patent-encumbered audio codecs beyond what VideoToolbox provides). H.264 / H.265 / AAC / Opus / Vorbis / MP3 / FLAC all work; rare codecs (AV1 in some containers, Dolby TrueHD, DTS:X) may not.

**Fix B**: re-mux/transcode the source on the server side or in a desktop preprocessing step — the iOS redist's codec set is not user-configurable.

### 5. File picker returns a URL that throws on `UniversalSourceSettings.CreateAsync`

**Cause**: `UIDocumentPickerViewController(..., asCopy: false)` returns a security-scoped URL that requires `NSUrl.StartAccessingSecurityScopedResource()` before the underlying file is readable from the SDK's GStreamer source. The bundled sample uses `asCopy: false` and never calls `StartAccessingSecurityScopedResource`, which works for files inside the app's own sandbox but fails on iCloud Drive / external Files providers.

**Fix**: either pass `asCopy: true` to the picker (iOS copies the file into the app's `tmp/` folder, no scope hassle, slower for large files) or wrap the open with `if (url.StartAccessingSecurityScopedResource()) { try { /* CreateAsync + OpenAsync */ } finally { url.StopAccessingSecurityScopedResource(); } }`. The picker URL must remain valid for the entire lifetime of the `MediaPlayerCoreX` — release the scope only on stop/dispose.

## Verification checklist

Run through these after first integration:

- [ ] `dotnet build -c Debug` succeeds with the bundled `references/` files copied into a fresh `net10.0-ios` project (no missing-DLL warnings).
- [ ] `Info.plist` contains `NSPhotoLibraryUsageDescription` with reviewer-safe wording.
- [ ] First run on a physical iPhone shows the photo-library prompt (only on Photos picker), then a working preview within ~2 s after `PlayAsync`.
- [ ] Position slider seeks without stutter and without re-entrant `Position_SetAsync` overlap (the bundled `_isSeeking` flag handles this).
- [ ] `OnStop` fires at end-of-stream and the UI button text resets to "PLAY" without leaking `MediaPlayerCoreX` (every Play→Stop cycle calls `await _player.DisposeAsync()` and reassigns to `null`).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app silently runs in 30-day trial mode).
- [ ] `MtouchInterpreter=all` is set on both Debug and Release configurations — confirm by checking the build log for `--interpreter`.
- [ ] If your app plays plain-HTTP streams: ATS exception (`NSAllowsArbitraryLoads` or per-domain `NSExceptionAllowsInsecureHTTPLoads`) is in `Info.plist`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh project folder (alongside an `Assets.xcassets`, `LaunchScreen.storyboard`, and `Entitlements.plist` from the standard .NET for iOS template) and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — minimal working `net10.0-ios` csproj, version-pinned to the same NuGet release as the prose, with `MtouchInterpreter=all` on both Debug and Release.
- `references/Info.plist` — bundle metadata with the `NSPhotoLibraryUsageDescription` privacy key filled in with reviewer-safe wording, and a commented-out ATS exception block ready to enable for HTTP streams.
- `references/AppDelegate.cs` — full code-behind: `VisioForgeX.InitSDK` boot, `UniversalSourceSettings` URL probe, video / audio stream auto-detection, `OnStart` / `OnStop` / `OnError` wiring, position-slider seek with reentrancy guard. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.)
- `references/CustomViewController.cs` — root `UIViewController` that builds the `VisioForge.Core.UI.Apple.VideoView` at full-window size, hosts the SELECT FILE / PLAY buttons + position slider, and acts as the `IUIDocumentPickerDelegate` for picking the media file.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-x-macos` — same SDK on native macOS (AppKit / `net10.0-macos`).
    - `media-player-sdk-x-android` — same SDK on native Android (`net10.0-android`).
    - `media-player-sdk-x-maui` — same SDK on .NET MAUI (one project shared across iOS / Android / Windows / Mac Catalyst).
    - `media-player-sdk-x-wpf` — same SDK on Windows WPF (desktop playback).
    - `video-capture-sdk-x-ios` — capture / recording counterpart on the same iOS host (same redist, side-by-side wrapper).
    - **Lower-level pipeline**:
        - `media-blocks-sdk-net-ios` — same engine, lower-level graph-based API for custom pipeline topologies on native iOS.
