---
name: video-edit-sdk-x-maui
description: Integrate VisioForge Video Edit SDK X (cross-platform edition) into a .NET MAUI app (Windows, Android, iOS, Mac Catalyst). Covers the VideoEditCoreX timeline API, the MAUI VideoView control, multi-target NuGet packages (per-OS native dependencies), SDK initialization, license registration, and the most common cross-platform pitfalls (missing InitSDKAsync, media-picker paths that are not filesystem paths, a bare MP4Output that silently encodes in software on iOS, Mac Catalyst and Windows, AOT JIT-only ExecutionEngineException, trial-period expiry). Use when joining, trimming, overlaying or rendering video from one MAUI codebase — for capture use video-capture-sdk-x-maui, for playback use media-player-sdk-x-maui.
---

# Video Edit SDK X — .NET MAUI integration

This skill helps you add **VisioForge Video Edit SDK X** — the cross-platform "X" edition of the editing SDK — to a .NET MAUI application targeting **Windows, Android, iOS, and Mac Catalyst** from a single codebase. `VideoEditCoreX` is a timeline engine on the GStreamer backend: you append clips, images and audio tracks, optionally add effects and transitions, then either preview the timeline into a `VideoView` or render it to a file. The same C# runs on every OS — only the per-TFM redist NuGets change.

Pinned NuGet versions: wrapper **`2026.8.16`**, MAUI handlers **`2026.8.16`**, plus per-OS native redists at the versions in the csproj below — these match the official [Video Edit X MAUI sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit). Newer 2026.x.x patch versions are drop-in compatible; keep `VisioForge.DotNet.VideoEdit` and `VisioForge.DotNet.Core.UI.MAUI` pinned to the same wrapper version. The redist versions track the underlying GStreamer rebuild cadence and lag the wrapper on purpose — **the iOS redist has no 2026.x release at all**; pin to the value in the upstream csproj, do not blindly bump.

## When to use this skill

- One MAUI codebase that joins, trims, overlays or re-encodes video on **Windows, Android, iOS, and Mac Catalyst**.
- Timeline work: multiple clips in sequence, transitions between them, text or image overlays, audio mixing and volume envelopes.
- Rendering the result to MP4, WebM, MKV, AVI or an audio-only container.
- Previewing the timeline in-app through `VisioForge.Core.UI.MAUI.VideoView`.

## When NOT to use this skill

- **Capturing** from a camera, screen or IP camera: `video-capture-sdk-x-maui`.
- **Playing back** a single file or stream with no editing: `media-player-sdk-x-maui`.
- **Custom pipeline topology** (tee, multi-source mix, runtime sink swap): `media-blocks-sdk-net-maui` — the same engine, one level down.
- **Windows-only desktop app**: `video-edit-sdk-x-wpf` is simpler — single TFM, no per-OS redists.
- **Different cross-platform host**: `video-edit-sdk-x-avalonia` — same SDK, different UI shell.

## NuGet packages (cross-platform layout)

Video Edit X for MAUI is **not** a single meta-package. You add the SDK, the MAUI handler package, then per-OS native redists conditionally:

| Package | Role | Condition |
|---|---|---|
| `VisioForge.DotNet.VideoEdit` | Managed wrapper (`VideoEditCoreX`, types) | Always |
| `VisioForge.DotNet.Core.UI.MAUI` | MAUI `VideoView` control + handlers | Always |
| `VisioForge.CrossPlatform.Core.Windows.x64` | Native runtime on Windows | `-windows` |
| `VisioForge.CrossPlatform.Libav.Windows.x64` | FFmpeg/libav muxers + encoders on Windows | `-windows` |
| `VisioForge.CrossPlatform.Core.Android` + AndroidDependency project ref | Native runtime on Android (the `.aar` ships through a small bundled csproj) | `-android` |
| `VisioForge.CrossPlatform.Core.iOS` | Native runtime on iOS | `-ios` |
| `VisioForge.CrossPlatform.Core.macCatalyst` | Native runtime on Mac Catalyst | `-maccatalyst` |

`VisioForge.DotNet.Core.UI.MAUI` supplies only the `VideoView` control. On its own it cannot compile `VideoEditCoreX` — `VisioForge.DotNet.VideoEdit` is what brings the engine in. Adding the UI package alone is a common mistake that produces a project that builds the XAML but not the code-behind.

`VisioForge.DotNet.VideoEdit` is the **same wrapper package** the legacy SDK uses — both `VideoEditCore` (legacy, Windows-only, DirectShow) and `VideoEditCoreX` (cross-platform, GStreamer) ship in it. The redist set plus the MAUI handler registration is what puts you on the X engine.

The Android target also needs a `<ProjectReference>` to `VisioForge.Core.Android.X10.csproj`, a small companion project in the samples repo under `AndroidDependency/`. That is how the `.aar` is bound; there is no NuGet equivalent. The folder ships one `VisioForge.Core.Android.X{N}.csproj` per supported .NET version — pick the one matching your TFM.

## Project setup

### MAUI workload

```bash
dotnet workload install maui
```

### Multi-target csproj

The csproj declares target frameworks **conditionally per host OS**: Android always; iOS + Mac Catalyst only on macOS hosts; Windows only on Windows hosts. That keeps `dotnet build` working on any developer machine.

The full minimal csproj is in `references/Sample.csproj`. Highlights:

```xml
<TargetFrameworks>net10.0-android</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('osx'))">$(TargetFrameworks);net10.0-maccatalyst;net10.0-ios</TargetFrameworks>
<TargetFrameworks Condition="$([MSBuild]::IsOSPlatform('windows'))">$(TargetFrameworks);net10.0-windows10.0.19041.0</TargetFrameworks>

<UseMaui>true</UseMaui>
<SingleProject>true</SingleProject>

<!-- iOS / Mac Catalyst MUST run with the Mono interpreter, otherwise MAUI XAML
     hits a JIT-only path inside Styles.xaml load and throws ExecutionEngineException. -->
<UseInterpreter Condition="...== 'ios'">true</UseInterpreter>
<UseInterpreter Condition="...== 'maccatalyst'">true</UseInterpreter>
```

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.VideoEdit" Version="2026.8.16" />
  <PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.8.16" />
</ItemGroup>

<ItemGroup Condition="$(TargetFramework.Contains('-windows'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.4.29" />
  <PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2026.4.29" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-android'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2026.7.27" />
  <ProjectReference Include="..\..\..\AndroidDependency\VisioForge.Core.Android.X10.csproj" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-ios'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.12.0" />
</ItemGroup>
<ItemGroup Condition="$(TargetFramework.Contains('-maccatalyst'))">
  <PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
</ItemGroup>
```

The Mac Catalyst target additionally needs an `AfterTargets="Build"` step (`CopyNativeLibrariesToMonoBundle`) that copies `.dylib` / `.so` files into `.app/Contents/MonoBundle/`. It is in `references/Sample.csproj` verbatim. Without it the bundled `.app` cannot find the native runtime and crashes with a `dlopen` failure on first `VideoEditCoreX` construction.

### MauiProgram registration

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

builder
    .UseMauiApp<App>()
    .UseSkiaSharp()
    .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());
```

Forgetting `AddVisioForgeHandlers()` is the most common silent failure — `<my:VideoView />` renders as a blank `Grid`-shaped void, no errors logged.

## Engine boot model

`AddVisioForgeHandlers()` only registers the `VideoView` handlers — `VisioForge.Core.UI.MAUI` never touches the native runtime. You must call `await VisioForgeX.InitSDKAsync()` yourself before the first `VideoEditCoreX` is constructed, or the constructor throws `DllNotFoundException` on a clean machine. Construction order:

1. MAUI app starts; `MauiProgram.CreateMauiApp()` runs (`AddVisioForgeHandlers` registers the view handlers).
2. `MainPage_Loaded` calls `await VisioForgeX.InitSDKAsync()`, then `_core = new VideoEditCoreX(videoView.GetVideoView())` and wires events.
3. Append clips with `Input_Add*`, set `Output_Format`, then `_core.Start()`.

The first `InitSDKAsync` builds the GStreamer plugin registry and can take hundreds of milliseconds, so use the async form off the UI thread. It is idempotent — repeat calls return immediately.

On shutdown, mirror with `VisioForgeX.DestroySDK()`:

```csharp
private void MainPage_Unloaded(object? sender, EventArgs e)
{
    if (_core != null)
    {
        _core.OnError -= Core_OnError;
        _core.Stop();
        _core.Dispose();
        _core = null;
    }

    VisioForgeX.DestroySDK();
}
```

## Timeline API

Each `Input_Add*` appends to the end of the timeline unless you pass an explicit insert time:

| Call | Use |
|---|---|
| `Input_AddAudioVideoFile(filename)` | A normal clip, video + audio |
| `Input_AddVideoFile(filename)` | Video track only |
| `Input_AddAudioFile(filename)` | Audio track only |
| `Input_AddImageFile(filename, TimeSpan)` | A still, held for the given duration |
| `Input_Clear_List()` | Empty the timeline |

The trimming overloads take explicit boundaries: `Input_AddAudioVideoFile(filename, startTime, stopTime, insertTime)`.

`Output_VideoSize` and `Output_VideoFrameRate` set the render target; leave them unset to inherit from the first clip.

## Preview vs render

One switch: `Output_Format`. Null plays the timeline into the `VideoView`; a format object renders to a file.

```csharp
// Preview
_core.Output_Format = null;
_core.Start();

// Render
_core.Output_Format = new MP4Output(
    Path.Combine(FileSystem.Current.AppDataDirectory, "joined.mp4"),
    H264EncoderBlock.GetDefaultSettings());
_core.Start();
```

Other formats live in `VisioForge.Core.Types.X.Output`: `WebMOutput`, `MKVOutput`, `AVIOutput`, `MP3Output`, `M4AOutput`, `OGGVorbisOutput`, `FLACOutput`, `WAVOutput`.

`OnProgress` reports 0–100 and `OnStop` reports success; both fire on a background thread, so marshal to the UI thread with `MainThread.BeginInvokeOnMainThread` before touching controls.

## License registration

For license types, scope, updates, support and trial terms, see the [canonical VisioForge licensing page](https://www.visioforge.com/licensing).

The SDK ships with a 30-day trial; after it expires `Start()` returns `false` and logs `"SDK TRIAL period (30 days) is over."`. To register a purchased licence, call `await _core.SetLicenseCertificateAsync(certBytes)` after the constructor and before `Start()`. The cross-platform wrinkle is **where the bytes come from**: on iOS / Android / Mac Catalyst the working directory is the app bundle, not your dev machine. Ship the licence as a `MauiAsset`:

```csharp
// In csproj:
// <ItemGroup>
//   <MauiAsset Include="Resources\Raw\license.vflicense" />
// </ItemGroup>

using var stream = await FileSystem.OpenAppPackageFileAsync("license.vflicense");
using var ms = new MemoryStream();
await stream.CopyToAsync(ms);
await _core.SetLicenseCertificateAsync(ms.ToArray());
```

Every `VideoEditCoreX` instance needs its own call before its `Start()`.

## Common cross-platform pitfalls

### 1. `DllNotFoundException` on the first `VideoEditCoreX`

**Cause**: `VisioForgeX.InitSDKAsync()` was never called. `AddVisioForgeHandlers()` does not do it — see "Engine boot model" above.

**Fix**: `await VisioForgeX.InitSDKAsync();` before the constructor.

### 2. The render silently uses a software encoder on iOS, Mac Catalyst and Windows

**Cause**: the single-argument `new MP4Output(path)` was used. Its built-in default picks `AppleMediaH264EncoderSettings` under `__MACOS__` and calls `H264EncoderBlock.GetDefaultSettings()` under `__ANDROID__`, but iOS, Mac Catalyst and Windows all fall through to `OpenH264EncoderSettings` — software H.264. On mobile that shows up as dropped frames and battery drain on longer renders.

**Fix**: pass `H264EncoderBlock.GetDefaultSettings()` as the video encoder. It is the SDK's platform selector — VideoToolbox on Apple, MediaCodec on Android, then NVENC / AMF / QSV, software last. Despite the `VisioForge.Core.MediaBlocks.VideoEncoders` namespace it is a shared helper, not a Media Blocks pipeline; `MP4Output` calls it itself. Leave the audio encoder unset so `MP4Output` picks AAC on Windows and MP3 elsewhere.

### 3. `Input_AddAudioVideoFile` gets a path that does not exist (iOS)

**Cause**: `PHPicker` hands MAUI an `NSItemProvider`; only the original file name survives in `FileResult.FullPath`. On Android `MediaPicker` copies into a cache path, so `FullPath` is real — the same code works there and silently fails on iOS.

**Fix**: check `Path.IsPathRooted` + `File.Exists`, and otherwise copy `OpenReadAsync()` into `FileSystem.Current.CacheDirectory`. `references/MainPage.xaml.cs` has the helper.

### 4. Rendered file cannot be written (Android)

**Cause**: writing to the shared media store without the permission, or on API 30+ without scoped-storage handling.

**Fix**: render into `FileSystem.Current.AppDataDirectory`, which needs no permission on any platform, and copy into the gallery afterwards only if the app requires it.

### 5. iOS / Mac Catalyst app crashes during MAUI XAML load

**Cause**: `<UseInterpreter>` not enabled for those TFMs. MAUI XAML's `TypeConversionExtensions` uses `DynamicMethod`, which requires JIT; the runtime is AOT-only by default, so it throws `ExecutionEngineException: Attempting to JIT compile method ... while running in aot-only mode` from `App.InitializeComponent()`.

**Fix**: `<UseInterpreter>true</UseInterpreter>` on `-ios` and `-maccatalyst`.

### 6. `NU1102: Unable to find package VisioForge.CrossPlatform.Core.iOS`

**Cause**: the iOS redist was floated to `2026.*` to match the wrapper. There is no 2026.x on nuget.org — the newest is `2025.12.0`.

**Fix**: pin `2025.12.0`. The iOS redist tracks the GStreamer-iOS rebuild cadence, not the wrapper release.

## Verification checklist

- [ ] `dotnet build -f net10.0-android` succeeds against the bundled `references/`.
- [ ] On macOS host: `dotnet build -f net10.0-ios` and `-f net10.0-maccatalyst` succeed.
- [ ] On Windows host: `dotnet build -f net10.0-windows10.0.19041.0` succeeds.
- [ ] `MauiProgram.CreateMauiApp()` calls `.ConfigureMauiHandlers(h => h.AddVisioForgeHandlers()).UseSkiaSharp()`.
- [ ] `await VisioForgeX.InitSDKAsync()` runs before the first `VideoEditCoreX` construction.
- [ ] Preview (`Output_Format = null`) shows the timeline in the `VideoView`.
- [ ] Render produces a file that plays back in another player, and `OnStop` reports `Successful == true`.
- [ ] `VisioForgeX.DestroySDK()` runs on page unload / window destroy.
- [ ] If a purchased licence is in use: `SetLicenseCertificateAsync` is called on every instance before `Start()`.

## Bundled references

- `references/Sample.csproj` — multi-target MAUI csproj with all per-OS conditional package references and the Mac Catalyst native-copy target.
- `references/MauiProgram.cs` — entry point with `AddVisioForgeHandlers()` + `UseSkiaSharp()`.
- `references/App.xaml` — MAUI Application entry point.
- `references/MainPage.xaml` — XAML with `<my:VideoView />`, a progress bar and the timeline buttons.
- `references/MainPage.xaml.cs` — full code-behind: `InitSDKAsync`, gallery picking with the iOS path fix, timeline building, preview, MP4 render, progress and completion handling, clean shutdown. (Runs in trial mode by design; add a `SetLicenseCertificateAsync` call when integrating a purchased licence.)
- `references/platform-permissions.md` — Android manifest, iOS / Mac Catalyst `Info.plist`.

## Related

- **Help**: <https://www.visioforge.com/help/docs/dotnet/videoedit/maui/video-editing-maui/>
- **Product page**: <https://www.visioforge.com/video-edit-sdk-net>
- **Official sample on GitHub**: <https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X/MAUI/SimpleEdit>
- **MCP server** (queryable API + class lookup): see `/.well-known/mcp.json` for the `search_api`, `get_class_info`, `get_code_example`, and `get_deployment_guide` tools.
- **Adjacent skills**:
    - `video-edit-sdk-x-wpf` — same X SDK on WPF (Windows-only host, no per-OS redists).
    - `video-edit-sdk-x-avalonia` — same X SDK on Avalonia.
    - `video-capture-sdk-x-maui` — capture on MAUI.
    - `media-player-sdk-x-maui` — playback on MAUI.
    - `media-blocks-sdk-net-maui` — same engine, lower-level graph API.
