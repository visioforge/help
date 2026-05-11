---
name: media-blocks-sdk-net-maui
description: Integrate VisioForge Media Blocks SDK .NET into a .NET MAUI cross-platform app (Windows, Android, iOS, macOS). Covers the graph-based pipeline model, multi-target NuGet packages (per-OS native dependencies), license registration, and the most common cross-platform pitfalls (camera permissions, missing native libs, trial-period expiry / unlicensed build). Use when building custom media pipelines (capture, transcode, stream, record) that must run on multiple OSes from one MAUI codebase.
---

# Media Blocks SDK .NET — .NET MAUI integration

This skill helps you add **VisioForge Media Blocks SDK .NET** to a .NET MAUI application that targets **Windows, Android, iOS, and macOS (Mac Catalyst)** from a single codebase. Media Blocks is a *graph-based* pipeline SDK — you build a `MediaBlocksPipeline` by connecting `MediaBlock` nodes (sources → transforms → encoders → sinks/renderers) similar to GStreamer or DirectShow. That's the primary trade-off vs the higher-level capture/edit/player SDKs: more flexibility, more wiring code.

Pinned NuGet version: **`2026.5.4`** for the MAUI SDK packages, with platform-specific redists at the versions shown in the csproj below — these match the official [Simple Capture MAUI sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimpleCapture). Newer 2026.x.x patch versions are drop-in compatible; keep `VisioForge.DotNet.MediaBlocks` and `VisioForge.DotNet.Core.UI.MAUI` pinned to the same version.

## When to use this skill

- One MAUI codebase that runs on **Windows, Android, iOS, and macOS (Mac Catalyst)** with a custom media pipeline.
- Custom capture/transcode/stream/record graphs that the high-level Capture/Edit/Player SDKs do not expose directly.
- Mobile-friendly preview using `VisioForge.Core.UI.MAUI.VideoView`.
- Mixing pipeline elements (tees, encoders, multiple sinks) — for example, preview + record-to-MP4 from a single source.

## When NOT to use this skill

- **Windows-only desktop app** (WPF / WinForms): `media-blocks-sdk-net-wpf` / `media-blocks-sdk-net-winforms` is simpler — single TFM, no per-OS redists.
- **MAUI does not fit the project** (no XAML, custom rendering needs, .NET 6/7 still pinned): use **Avalonia** → `media-blocks-sdk-net-avalonia`, or **Uno Platform** → `media-blocks-sdk-net-uno`.
- **High-level capture only** (webcam → MP4, no custom graph): `video-capture-sdk-net-wpf` is shorter on Windows; for cross-platform high-level capture there is no equivalent — fall back to Media Blocks.
- **Playback only** (file/stream playback without a custom graph): `media-player-sdk-x-maui`.

## NuGet packages (cross-platform layout)

Media Blocks for MAUI is **not** a single meta-package. You add the SDK plus the **MAUI handler package**, then per-OS native runtime redists conditionally. The full set:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.MediaBlocks` | Managed SDK (pipeline, blocks, types) | Always |
| `VisioForge.DotNet.Core.UI.MAUI` | MAUI `VideoView` control + handlers | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` ships through a small bundled csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

The Android target also needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj` (a small companion project that ships in the samples repo under `MAUI/AndroidDependency/`) — that's how the `.aar` is bound. Copy that folder into your repo alongside your MAUI app and reference it as shown in the csproj below; there is no NuGet equivalent today.

## Project setup

### MAUI workload

Make sure the MAUI workload is installed:

```bash
dotnet workload install maui
```

Without this, `dotnet build` fails with `error NETSDK1147: To build this project, the following workloads must be installed: maui-android maui-ios maui-maccatalyst maui-windows`.

### Multi-target csproj

This is the core of the skill — get this right and the rest follows. The csproj declares target frameworks **conditionally per host OS**: Android always; iOS + Mac Catalyst only on macOS hosts; Windows only on Windows hosts. That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Adapted from the official Simple Capture MAUI sample (`MAUI/SimpleCapture/SimpleCaptureMB.csproj`). Highlights:

```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">$(TargetFrameworks);net10.0-maccatalyst;net10.0-ios</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>

<UseMaui>true</UseMaui>
<SingleProject>true</SingleProject>

<!-- iOS / Mac Catalyst MUST run with the Mono interpreter, otherwise MAUI XAML
     hits a JIT-only path inside Styles.xaml load and throws ExecutionEngineException. -->
<UseInterpreter Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'ios'">true</UseInterpreter>
<UseInterpreter Condition="$([MSBuild]::GetTargetPlatformIdentifier('$(TargetFramework)')) == 'maccatalyst'">true</UseInterpreter>
```

The conditional `<ItemGroup>` blocks pull in the right per-OS native packages:

```xml
<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.4.18.0" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>
```

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into the `.app/Contents/MonoBundle/` folder. This is included verbatim in `references/Sample.csproj`. Without it, the bundled `.app` cannot find the native runtime at launch and crashes with a `dlopen` failure.

### MauiProgram registration

In `MauiProgram.cs` register the VisioForge MAUI handlers and SkiaSharp (used internally by the renderer):

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Forgetting `AddVisioForgeHandlers()` is the most common silent failure — `<my:VideoView />` renders as a blank `Grid`-shaped void, no errors logged.

## Per-platform permissions

Cross-platform capture means cross-platform permission paperwork. Each OS has its own dialect — see `references/platform-permissions.md` for the full set; the essentials:

### Android

`Platforms/Android/AndroidManifest.xml` must declare `CAMERA` and `RECORD_AUDIO`. Storage permissions are needed to write recorded MP4 files to gallery on older API levels:

```xml
<uses-permission android:name="android.permission.CAMERA" />
<uses-permission android:name="android.permission.RECORD_AUDIO" />
<uses-permission android:name="android.permission.INTERNET" />
<uses-feature android:name="android.hardware.camera" android:required="false" />
```

At runtime, request the permission with the MAUI Essentials API before the pipeline starts:

```csharp
await Permissions.RequestAsync<Permissions.Camera>();
await Permissions.RequestAsync<Permissions.Microphone>();
```

### iOS

`Platforms/iOS/Info.plist` MUST contain non-empty user-facing strings — App Store review rejects builds without them, and **iOS terminates the app** the first time you touch the camera/mic without the matching key:

```xml
<key>NSCameraUsageDescription</key>
<string>Camera usage required</string>
<key>NSMicrophoneUsageDescription</key>
<string>Mic usage required</string>
<key>NSPhotoLibraryUsageDescription</key>
<string>To save recorded videos</string>
```

### Mac Catalyst

`Platforms/MacCatalyst/Info.plist` needs the same usage descriptions as iOS, **plus** `Platforms/MacCatalyst/Entitlements.plist` must enable the camera/microphone entitlements (sandboxed Mac Catalyst apps don't get these by default):

```xml
<!-- Entitlements.plist -->
<key>com.apple.security.device.camera</key>
<true/>
<key>com.apple.security.device.audio-input</key>
<true/>
```

### Windows

No extra permission paperwork for desktop Windows MAUI — the system camera privacy switch in Settings is enforced by Windows itself, not declared in the project.

## License registration

Same `SetLicenseCertificateAsync(byte[])` API as the Desktop SDK — the contract is identical across all VisioForge .NET SDKs as of `2026.5.2`. It is a per-instance method on `MediaBlocksPipeline`; call it on every pipeline instance after construction and before `StartAsync`. The cross-platform wrinkle is **where the bytes come from**: `File.ReadAllBytes("path/to/your.vflicense")` works on Windows, but on iOS / Android / Mac Catalyst the working directory is the app bundle, not your dev machine. The portable approach is to ship the licence as a `MauiAsset` and load it via `FileSystem.OpenAppPackageFileAsync`:

```csharp
// In csproj:
// <ItemGroup>
//   <MauiAsset Include="Resources\Raw\license.vflicense" />
// </ItemGroup>

// In MainPage.xaml.cs after you create the pipeline, before StartAsync:
using var stream = await FileSystem.OpenAppPackageFileAsync("license.vflicense");
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
await _pipeline.SetLicenseCertificateAsync(ms.ToArray());
```

The certificate-bytes form is the only public licensing API as of `2026.5.2` — the older `SetLicenseCertificate(string filePath)` and `SetLicenseCertificate(Stream)` overloads were removed in that release. Every `MediaBlocksPipeline` instance needs its own `SetLicenseCertificateAsync` call before its `StartAsync`. Where the bytes come from (asset, env var, secrets manager, …) is your application's choice — there is no built-in licence-loader helper class in the SDK.

## Hello-World pipeline

A camera-→-preview pipeline is three blocks: source, renderer, and the pipeline that owns them. The full file is in `references/MainPage.xaml.cs`; the minimum viable wiring:

```xml
<!-- MainPage.xaml -->
<ContentPage xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI" ...>
    <Grid>
        <my:VideoView x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand" />
    </Grid>
</ContentPage>
```

```csharp
// MainPage.xaml.cs
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public partial class MainPage : ContentPage
{
    private MediaBlocksPipeline _pipeline;

    public MainPage()
    {
        InitializeComponent();
        VisioForgeX.InitSDK();
        Loaded += async (_, __) => await StartPreviewAsync();
    }

    private async Task StartPreviewAsync()
    {
        await Permissions.RequestAsync<Permissions.Camera>();

        var cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (cameras.Length == 0) return;

        var device = cameras[0];
        var fmt = device.GetVideoFormatAndFrameRate(1280, 720, out var frameRate);
        var settings = new VideoCaptureDeviceSourceSettings(device) { Format = fmt.ToFormat() };
        settings.Format.FrameRate = frameRate;

        _pipeline = new MediaBlocksPipeline();
        var source = new SystemVideoSourceBlock(settings);
        var renderer = new VideoRendererBlock(_pipeline, videoView.GetVideoView()) { IsSync = false };

        _pipeline.Connect(source.Output, renderer.Input);
        await _pipeline.StartAsync();
    }
}
```

The bundled `references/MainPage.xaml.cs` extends this with audio source, audio renderer, a `TeeBlock`-based preview-+-record graph, and per-OS gallery-save logic. Use it as a copy-paste template; trim the branches you don't need.

## Common cross-platform pitfalls

### 1. Black `VideoView`, no errors logged

**Cause**: `AddVisioForgeHandlers()` was not called in `MauiProgram.CreateMauiApp`. The view registers fine but renders nothing because no platform handler is bound.

**Fix**: add `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers())` to the builder chain (see "MauiProgram registration" above).

### 2. `DllNotFoundException` / `dlopen` failure on first capture (per-OS)

**Cause**: the matching per-OS native runtime package is missing from the conditional `<ItemGroup>`. Common slips:

- Windows: forgot `VisioForge.CrossPlatform.Core.Windows.x64` (build succeeds but startup fails on first `MediaBlocksPipeline` use).
- Android: missing `<ProjectReference Include="...AndroidDependency\VisioForge.Core.Android.X10.csproj" />` — the package alone is not enough.
- Mac Catalyst: the `CopyNativeLibrariesToMonoBundle` `<Target>` was deleted, so `.dylib` files never reach `.app/Contents/MonoBundle/`.

**Fix**: cross-check against `references/Sample.csproj` — every conditional ItemGroup matters.

### 3. iOS / Mac Catalyst app crashes during MAUI XAML load

**Cause**: `<UseInterpreter>` is not enabled for those TFMs. MAUI XAML's `TypeConversionExtensions` uses `DynamicMethod` to invoke implicit operators, and `DynamicMethod` requires JIT. On iOS / Mac Catalyst the runtime is AOT-only by default, so the call throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode` from inside `App.InitializeComponent()`.

**Fix**: set `<UseInterpreter>true</UseInterpreter>` on `-ios` and `-maccatalyst` (see csproj). Tiny size/startup cost, fixes XAML load.

### 4. Trial-mode message or "SDK TRIAL period (30 days) is over" on app start

**Cause**: no `.vflicense` certificate has been loaded — either nothing was loaded at all, or `SetLicenseCertificateAsync` was called *after* `pipeline.StartAsync()`. Once the 30-day trial elapses the engine refuses to start.

**Fix**: call `await _pipeline.SetLicenseCertificateAsync(certBytes)` on every `MediaBlocksPipeline` instance, after the constructor and before `StartAsync`. On MAUI, ship the `.vflicense` as a `MauiAsset` and load with `FileSystem.OpenAppPackageFileAsync` (see "License registration" above).

### 5. iOS first run silently terminates as soon as capture starts

**Cause**: missing `NSCameraUsageDescription` / `NSMicrophoneUsageDescription` in `Platforms/iOS/Info.plist`. iOS terminates the app process on the first hardware access without a matching usage-description key — there is no exception, no crash log entry from your code.

**Fix**: add both keys with non-empty user-facing strings (see "Per-platform permissions").

## Verification checklist

- [ ] `dotnet workload install maui` is installed; `dotnet build -f net10.0-android` succeeds against the bundled `references/`.
- [ ] On macOS host: `dotnet build -f net10.0-ios` and `dotnet build -f net10.0-maccatalyst` succeed (TFMs only show up on macOS).
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041.0` succeeds.
- [ ] App launches on each target OS and shows the camera preview within ~2 s after permission grant.
- [ ] If recording: the MP4 is finalised when `StopAsync` runs to completion before `Dispose` (verify the file plays back in another player).
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every `MediaBlocksPipeline` instance before its `StartAsync`.

## Bundled references

The `references/` folder is self-contained — copy all of it into a fresh MAUI project folder, set up `Platforms/` and `Resources/` per the standard MAUI template, and `dotnet build` succeeds with no extra files needed:

- `references/Sample.csproj` — multi-target MAUI csproj with all per-OS conditional package references and the Mac Catalyst native-copy target.
- `references/MauiProgram.cs` — entry point with `AddVisioForgeHandlers()` + `UseSkiaSharp()`.
- `references/App.xaml` — MAUI Application entry point.
- `references/MainPage.xaml` — XAML with `<my:VideoView />` and capture/preview/stop buttons.
- `references/MainPage.xaml.cs` — full code-behind: device enumeration, runtime permission requests, preview-only graph, and a tee-based preview+record graph that writes MP4. Use as a copy-paste starting template. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call on every `MediaBlocksPipeline` instance when integrating a purchased licence.)
- `references/platform-permissions.md` — concise notes on Android `AndroidManifest.xml`, iOS `Info.plist`, Mac Catalyst `Info.plist` + `Entitlements.plist`.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/mediablocks/>
- **Product page**: <https://www.visioforge.com/media-blocks-sdk-net>
- **Official samples on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `media-blocks-sdk-net-avalonia` — same SDK on Avalonia (broader Linux support, no iOS).
    - `media-blocks-sdk-net-uno` — same SDK on Uno Platform (WebAssembly target).
    - `media-blocks-sdk-net-wpf` — Windows-only desktop with a single TFM (simpler when iOS / Android / macOS aren't needed).
    - `media-player-sdk-x-maui` — high-level playback API; use when a full custom graph is overkill.
