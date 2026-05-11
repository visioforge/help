---
name: media-player-sdk-x-uno
description: Integrate VisioForge Media Player SDK X (cross-platform edition) into an Uno Platform application. Covers the Uno-specific VideoView control, multi-target NuGet packages (per-OS native dependencies), license registration, supported input formats, and the most common cross-platform pitfalls (network access per OS, WebAssembly limitations, missing native libs, trial-period expiry / unlicensed build). Use for Uno cross-OS apps — for MAUI use media-player-sdk-x-maui, for Avalonia use media-player-sdk-x-avalonia.
---

# Media Player SDK X — Uno Platform integration

This skill helps you add **VisioForge Media Player SDK X** — the cross-platform "X" edition of the player SDK — to an **Uno Platform** application that targets Windows, Android, iOS, and Mac Catalyst from a single codebase. The same C# playback code runs unchanged on every target; what differs is the per-OS native redist NuGet, the `Uno.Sdk` MSBuild SDK, and the Uno-specific `VisioForge.Core.UI.Uno.VideoView` control.

`MediaPlayerCoreX` is the high-level open/play/seek god-object — same API as on the WPF/MAUI/WinUI hosts, just bound to Uno's `VideoView`. Under the hood it shares the GStreamer-backed engine with Media Blocks. It opens local files, HTTP(S)/HLS/DASH/RTSP streams, and any other URL the underlying engine can demux.

Pinned NuGet versions (match the bundled `references/Sample.csproj` and the official Uno Simple Player sample): wrapper **`2026.5.4`**, Uno UI **`2026.5.4`**, Windows redists **`2026.4.29`**, Android redist **`2026.4.18.0`**, iOS redist **`2025.0.16`**, Mac Catalyst redist **`2025.9.1`**. Newer 2026.x.x patch versions are usually drop-in compatible — keep the wrapper and `VisioForge.DotNet.Core.UI.Uno` on the same version, and pin the per-OS redists to the values from the upstream csproj for your wrapper version.

## When to use this skill

- Single Uno codebase that runs on **Windows (WinAppSDK / WinUI 3), Android, iOS, and Mac Catalyst** and plays local files plus network streams (HTTP(S), HLS, DASH, RTSP) from one set of source files.
- Sharing playback code between an Uno app and other VisioForge X hosts (WPF, MAUI, Avalonia, WinUI) — `MediaPlayerCoreX` is identical across all of them.
- Uno-specific UX: native `Uno.Sdk` "single-project" head, XAML-based UI, hot-reload via `MainWindow.UseStudio()`.

## When NOT to use this skill

- **MAUI host** (XAML + handlers, MAUI Essentials helpers): use `media-player-sdk-x-maui`. API is identical — only the UI host package and lifecycle hooks differ.
- **Avalonia host** (Linux as a first-class target, no iOS): use `media-player-sdk-x-avalonia`.
- **WinUI 3 host without Uno** (Windows-only WinAppSDK, single TFM): use `media-player-sdk-x-winui` — simpler csproj, no per-OS conditionals.
- **WPF host** (Windows-only, .NET Framework / .NET on Windows): use `media-player-sdk-x-wpf`.
- **WebAssembly target**: not currently supported by the X engine. The native runtime is GStreamer-based and there is no WASM build of the redist. Uno's `-browserwasm` head builds compile (the wrapper is managed) but `VisioForgeX.InitSDKAsync()` fails at runtime because the native library isn't there. Don't add a `-browserwasm` TFM to the player project — split web UI off into a separate Uno head that doesn't reference the player SDK.
- **Custom pipeline topology** (split-with-tee, multi-renderer fan-out, transcode-while-playing, runtime sink swap): use `media-blocks-sdk-net-uno` — `MediaPlayerCoreX` wraps the same engine behind a fixed-shape graph.

## NuGet packages (cross-platform layout)

Two managed packages always, plus per-OS native redists conditionally. The conditional `<ItemGroup>` blocks are not optional — every target framework you build needs its matching native runtime, otherwise the app builds but crashes the first time `VisioForgeX.InitSDKAsync()` runs.

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaPlayer` | Managed wrapper (`MediaPlayerCoreX`, types) | Always |
| `VisioForge.DotNet.Core.UI.Uno` | Uno `VideoView` control | Always |
| `Microsoft.WindowsAppSDK` | WinAppSDK runtime for the Windows head | `-windows` |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav demuxers + decoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` is bound through a small companion csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

The Android target additionally needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` (a small companion project that ships in the samples repo under `AndroidDependency/`) — that's how the `.aar` is bound. Copy that folder into your repo and reference it as shown in the bundled csproj. There's no NuGet equivalent today.

## Project setup

### Uno workload

Make sure the Uno templates and workloads are installed. The Uno project type uses `Sdk="Uno.Sdk"` (not `Microsoft.NET.Sdk`), and that SDK pulls in the right per-OS workloads automatically when present. If you're starting fresh:

```bash
dotnet workload install android ios maccatalyst maui  # workloads needed for each TFM
dotnet new install Uno.Templates                        # Uno project templates
```

Without the matching workload, `dotnet build -f net10.0-android` (etc.) fails with `error NETSDK1147: To build this project, the following workloads must be installed`.

### Multi-target csproj

The csproj declares target frameworks **conditionally per host OS**: Android always; Mac Catalyst only on macOS hosts; Windows only on Windows hosts. iOS is intentionally absent from the upstream Uno sample (Uno's iOS head usually lives in a separate solution; if you do need it, add `net10.0-ios` to the macOS condition and reference `VisioForge.CrossPlatform.Core.iOS`). That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights:

```xml
<Project Sdk="Uno.Sdk">
  <PropertyGroup>
    <ManagePackageVersionsCentrally>false</ManagePackageVersionsCentrally>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">net10.0-windows10.0.19041;net10.0-android</TargetFrameworks>
    <TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">net10.0-maccatalyst;net10.0-android</TargetFrameworks>
    <UnoSingleProject>true</UnoSingleProject>
    <RunAOTCompilation>false</RunAOTCompilation>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.5.4" />
    <PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="2026.5.4" />
  </ItemGroup>
</Project>
```

The conditional native-redist `<ItemGroup>` blocks pull in the right per-OS packages — see `references/Sample.csproj` for the full set.

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure.

`<RunAOTCompilation>false</RunAOTCompilation>` is required — the native-interop layer is reflection-heavy in places and doesn't survive AOT trimming today. Uno's default for some heads is AOT-on; the upstream sample explicitly turns it off, and so should you.

### Uno "single project" structure

Unlike MAUI, an Uno app is conventionally split into multiple **head projects** — one per target OS — each consuming a shared library project. The upstream Simple Player Uno sample uses the newer **`UnoSingleProject` mode** (`<UnoSingleProject>true</UnoSingleProject>`) which collapses the heads into a single multi-targeted csproj. That's the layout reflected in `references/Sample.csproj`. If your existing solution still uses the older multi-head layout, add the VisioForge package references and conditional redist blocks to **the head project that builds for that target**, not to the shared library.

### App.xaml.cs registration

Uno's `App` class is plain WinUI / WinAppSDK. There is **no MAUI-style handler registration** — the `<vf:VideoView />` control wires itself up through its own `xmlns` declaration in XAML, so nothing needs to be done in `App.xaml.cs` beyond what the Uno template generates. See `references/App.xaml.cs` for the unchanged template.

## Per-platform considerations

A media player needs less paperwork than a capture app — there's no camera/microphone privilege gate — but a few per-OS items still matter when network sources (HLS/DASH/RTSP/HTTP(S)) or writable storage are involved.

### Android

- **Network access.** Plain HTTP URLs (not HTTPS) are blocked by default on Android API 28+. If you must play `http://` sources, set `android:usesCleartextTraffic="true"` in the `<application>` element of `Platforms/Android/AndroidManifest.xml`, or use a network-security config. HTTPS / HLS-over-HTTPS / DASH-over-HTTPS work out of the box.
- **INTERNET permission** is granted implicitly for debug builds but should be declared explicitly for release: `<uses-permission android:name="android.permission.INTERNET" />`.
- **Storage**: only needed if you open files from public folders on older API levels. On Android 10+ (API 29+) prefer the app-specific external dir (`GetExternalFilesDir(...)`) — no permission required under scoped storage.

### iOS

- **App Transport Security (ATS).** Plain `http://` URLs are blocked by ATS unless you add `NSAllowsArbitraryLoads` (or a per-domain exception) to `Platforms/iOS/Info.plist`. HTTPS URLs work as-is. RTSP is not subject to ATS but may be blocked by default on some networks — test on cellular and Wi-Fi separately.
- No camera/microphone usage descriptions are required for a player that doesn't capture. If you also do recording in the same app, see `media-player-sdk-x-maui` / `video-capture-sdk-x-uno` for the full set of `NS*UsageDescription` keys.

### Mac Catalyst

- **App Sandbox network**. Mac Catalyst inherits ATS rules from iOS. If your app is sandboxed, also enable the network-client entitlement in `Platforms/MacCatalyst/Entitlements.plist`:

  ```xml
  <key>com.apple.security.network.client</key><true/>
  ```

- Sandbox-off Catalyst (the default for the upstream sample, with `com.apple.security.app-sandbox` set to `<false/>`) lifts both ATS-host and sandbox restrictions but disqualifies the app from the Mac App Store.

### Windows

- No project-side paperwork for the Windows head. WinAppSDK has no per-app network gate; firewall prompts on first outbound connection are a Windows-level concern.

## Mandatory engine boot

Before any `MediaPlayerCoreX` instance is constructed, call `await VisioForgeX.InitSDKAsync()` exactly once per process. On a fresh machine the first call builds the GStreamer plugin-registry cache (~2-5 s, one-time on desktop; up to ~10 s on Android first launch); subsequent launches are instant. Mirror it with `VisioForgeX.DestroySDK()` on page unload / app shutdown:

```csharp
private async void MainPage_Loaded(object sender, RoutedEventArgs e)
{
    await VisioForgeX.InitSDKAsync();

    _player = new MediaPlayerCoreX(videoView);
    _player.OnError += Player_OnError;
    _player.OnStart += Player_OnStart;
    _player.OnStop += Player_OnStop;

#if !__IOS__ || __MACCATALYST__
    var audioOutputs = await _player.Audio_OutputDevicesAsync();
    if (audioOutputs.Length > 0)
        _player.Audio_OutputDevice = new AudioRendererSettings(audioOutputs[0]);
#endif
}

private async void MainPage_Unloaded(object sender, RoutedEventArgs e)
{
    if (_player != null)
    {
        await _player.StopAsync();
        await _player.DisposeAsync();
        _player = null;
    }
    VisioForgeX.DestroySDK();
}
```

The `#if !__IOS__ || __MACCATALYST__` guard around `Audio_OutputDevicesAsync` is necessary because on plain iOS (non-Catalyst) the audio output is fixed by the OS — the API returns no devices and you don't need to set `Audio_OutputDevice` at all. Mac Catalyst and Windows / Android expose the regular device list.

Skipping `InitSDKAsync` is the #1 source of "DLL not found" / "no element X" failures on first run.

## License registration

The SDK ships with a 30-day trial. To register a purchased licence, call `await _player.SetLicenseCertificateAsync(certBytes)` on every `MediaPlayerCoreX` instance, after the constructor and before `OpenAsync`:

```csharp
_player = new MediaPlayerCoreX(videoView);
_player.OnError += Player_OnError;

var cert = System.IO.File.ReadAllBytes("path/to/your.vflicense");
await _player.SetLicenseCertificateAsync(cert);
```

The certificate-bytes form is the only public licensing API in current 2026.x. The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes` works on Windows / Mac Catalyst (sandbox off), but on iOS / Android the working directory is the app bundle, not your dev machine. The portable approach for an Uno head is to ship the licence as a **`Content`** item with `CopyToOutputDirectory=PreserveNewest` and read it with `Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///license.vflicense"))` — that URI scheme works on every Uno target including Android / iOS. (On the Windows head you can also use `Package.Current.InstalledLocation`.) Where the bytes come from (asset, env var, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

The bundled `references/MainPage.xaml.cs` runs in trial mode by design; add the two lines above into `MainPage_Loaded` right after `_player = new MediaPlayerCoreX(...)` to register a purchased licence.

## Hello-World playback

Open-and-play on Uno is four lines: init, construct, open, play. The full file is in `references/MainPage.xaml.cs`; the minimum viable XAML + code-behind:

```xml
<!-- MainPage.xaml -->
<Page x:Class="YourApp.MainPage"
      xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
      xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
      xmlns:vf="using:VisioForge.Core.UI.Uno"
      Background="{ThemeResource ApplicationPageBackgroundThemeBrush}">
    <Grid>
        <vf:VideoView x:Name="videoView"
                      HorizontalAlignment="Stretch"
                      VerticalAlignment="Stretch" />
        <!-- add a Button x:Name="PlayButton" Click="PlayButton_Click" anywhere -->
    </Grid>
</Page>
```

```csharp
// MainPage.xaml.cs
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

public sealed partial class MainPage : Page
{
    private MediaPlayerCoreX? _player;

    public MainPage()
    {
        InitializeComponent();
        Loaded += async (_, __) =>
        {
            await VisioForgeX.InitSDKAsync();
            _player = new MediaPlayerCoreX(videoView);
        };
    }

    private async void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        // async-void event handlers MUST catch — otherwise an exception escapes
        // to AppDomain.UnhandledException and silently terminates the app.
        // Common triggers on first run: trial expired, missing native libs, or
        // a network URL blocked by ATS / cleartext-traffic policy.
        try
        {
            const string url = "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4";
            var settings = await UniversalSourceSettings.CreateAsync(new Uri(url));
            await _player!.OpenAsync(settings);
            await _player.PlayAsync();
        }
        catch (Exception ex) { /* surface to UI */ }
    }
}
```

`references/MainPage.xaml.cs` extends this with a position-update `DispatcherTimer`, seek slider wiring (`Position_GetAsync` / `Position_SetAsync`), volume slider (`Audio_OutputDevice_Volume`), per-OS source-settings creation (NSUrl on iOS, regular `Uri` everywhere else), and full lifecycle (`OnStart` / `OnStop` / `OnError`). Use it as a copy-paste template; trim the branches you don't need.

## Common cross-platform pitfalls

### 1. Black `<vf:VideoView />`, no errors logged

**Cause**: playback started before the Uno `VideoView` element has a valid handle (e.g. construction in the page constructor instead of `Loaded`), **or** `MediaPlayerCoreX` was constructed before `await VisioForgeX.InitSDKAsync()` returned, **or** the wrong native redist was referenced for the build's TFM.

**Fix**: defer construction and `OpenAsync` to `Loaded`. The bundled sample uses `MainPage_Loaded` for `InitSDKAsync` + player construction and an explicit "Play" button for `OpenAsync` / `PlayAsync` — copy that pattern.

### 2. `DllNotFoundException` / `dlopen` failure on first playback (per-OS)

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`. Common slips:

- Windows head: forgot `VisioForge.CrossPlatform.Core.Windows.x64` (build succeeds but startup fails on first `MediaPlayerCoreX` use).
- Android head: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough.
- Mac Catalyst head: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional `<ItemGroup>` and the Mac Catalyst `<Target>` matters.

### 3. Network URL plays on Windows but fails on Android / iOS

**Cause**: cleartext HTTP is blocked by default. On Android API 28+, `http://` URLs need `android:usesCleartextTraffic="true"` (or a network-security config). On iOS / Mac Catalyst, ATS rejects `http://` unless you add `NSAllowsArbitraryLoads` or a per-domain `NSExceptionDomains` entry to `Info.plist`. RTSP works on iOS over Wi-Fi but is sometimes filtered on cellular.

**Fix**: prefer HTTPS sources (HLS-over-HTTPS, DASH-over-HTTPS, MP4 via HTTPS). If you can't avoid plain HTTP, opt-in explicitly in the manifest / Info.plist of the target OS.

### 4. `VisioForgeX.InitSDKAsync` works locally but fails in `-browserwasm` head

**Cause**: WebAssembly is not a supported target for the X engine. There is no WASM build of the GStreamer-based native runtime, so `InitSDKAsync` cannot find `libgstcore` (or its WASM equivalent) and throws `DllNotFoundException`.

**Fix**: split the WebAssembly UI off into a separate Uno head that does **not** reference `VisioForge.DotNet.MediaPlayer` / `VisioForge.DotNet.Core.UI.Uno`. The player project should multi-target only `-windows`, `-android`, `-ios`, and `-maccatalyst`. If you accidentally added `net10.0-browserwasm` to `<TargetFrameworks>`, remove it.

### 5. Trial-mode banner or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded on the engine instance — either nothing was loaded at all, or the trial 30-day window has elapsed (the engine aborts startup with `"SDK TRIAL period (30 days) is over."`), or the certificate was loaded on a *different* `MediaPlayerCoreX` instance than the one being opened.

**Fix**: read the `.vflicense` (shipped as `Content` / `ms-appx:///` resource on iOS / Android, see "License registration") and call `await _player.SetLicenseCertificateAsync(certBytes)` after the constructor and before `OpenAsync`. Every `MediaPlayerCoreX` instance in the process needs its own call.

### 6. iOS: no audio output, `Audio_OutputDevicesAsync` returns empty

**Not a bug.** On plain iOS (non-Catalyst), the audio routing is owned by the OS / `AVAudioSession`; the SDK doesn't expose a device list and `Audio_OutputDevice` should not be set. The bundled `references/MainPage.xaml.cs` guards the device-list call with `#if !__IOS__ || __MACCATALYST__` for exactly this reason. On Mac Catalyst, Android, and Windows the regular device list is available.

### 7. Mac Catalyst app launches but `<vf:VideoView />` is empty and `OnError` reports `dlopen` failure

**Cause**: the `CopyNativeLibrariesToMonoBundle` target was deleted from the csproj, or the build configuration has `<UseInterpreter>` off and AOT trimmed the native-interop layer.

**Fix**: keep the `CopyNativeLibrariesToMonoBundle` `<Target>` from `references/Sample.csproj`, and keep `<RunAOTCompilation>false</RunAOTCompilation>` at the project level.

## Verification checklist

- [ ] Uno workloads installed: `dotnet workload list` shows `android`, plus `maccatalyst` on macOS / `wasm-tools` if you have a (separate) WASM head.
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041` succeeds against the bundled `references/`.
- [ ] On Windows host: `dotnet build -f net10.0-android` succeeds.
- [ ] On macOS host: `dotnet build -f net10.0-maccatalyst` succeeds; the `.app/Contents/MonoBundle/` folder contains `.dylib` files after the build.
- [ ] App launches on each target OS and shows the first frame within ~2 s of `PlayAsync`.
- [ ] Seek slider tracks playback position; setting the slider value calls `Position_SetAsync` and the video jumps.
- [ ] Volume slider changes audio level (skip on plain iOS — no device list).
- [ ] Network URL playback works over HTTPS on every OS; cleartext HTTP works only where the manifest opts in.
- [ ] On clean shutdown, the page's `Unloaded` runs `StopAsync → DisposeAsync → VisioForgeX.DestroySDK()` in that order.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaPlayerCoreX` instance before its `OpenAsync` (otherwise the app runs in 30-day trial mode).

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh `Uno.Sdk` project folder, set up `Platforms/` per the standard Uno template, and `dotnet build` succeeds with no extra files needed (apart from the `AndroidDependency/VisioForge.Core.Android.X10.csproj` companion project, which lives a few directory levels up — adjust the `<ProjectReference>` path to match your repo layout).

- `references/Sample.csproj` — multi-target Uno csproj with all per-OS conditional package references and the Mac Catalyst native-copy `<Target>`.
- `references/App.xaml` + `references/App.xaml.cs` — Uno Application entry point (unchanged from the Uno template; no VisioForge-specific registration needed).
- `references/MainPage.xaml` — XAML with `<vf:VideoView />`, seek slider, position/duration labels, play/stop button, URL textbox, and volume slider.
- `references/MainPage.xaml.cs` — full code-behind: `InitSDKAsync` boot, `MediaPlayerCoreX` lifecycle, `OnStart` / `OnStop` / `OnError` wiring, `DispatcherTimer`-based position updates, slider seek (`Position_SetAsync`), volume control, per-OS source-settings creation (`NSUrl` on iOS, `Uri` everywhere else), and audio-device selection guarded for plain iOS. Runs in 30-day trial mode by design — add a `SetLicenseCertificateAsync` call yourself when integrating a purchased licence.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediaplayer/>
- **Product page**: <https://www.visioforge.com/media-player-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Uno>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-player-sdk-x-wpf` — same X SDK on WPF (Windows-only host, single TFM, no per-OS conditionals).
    - `media-player-sdk-x-winui` — same X SDK on WinUI 3 / WinAppSDK (Windows-only) without Uno.
    - `video-capture-sdk-x-uno` — capture / recording counterpart on the same Uno host (camera, IP cameras, screen → MP4).
    - **Other cross-platform hosts**:
        - `media-player-sdk-x-maui` — same X SDK on .NET MAUI.
        - `media-player-sdk-x-avalonia` — same X SDK on Avalonia (broader Linux support, no iOS).
        - `media-blocks-sdk-net-uno` — same engine on Uno, lower-level graph-based API for custom pipeline topologies.
