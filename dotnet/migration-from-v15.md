---
title: Migration Guide — Upgrade VisioForge .NET SDK v15 to v2026
description: Step-by-step guide for upgrading from VisioForge .NET SDK v15 to v2026, covering both DirectShow and cross-platform X-engine migration paths.
sidebar_label: Migration from v15
order: 25
---

# Migration Guide: v15 to v2026

This guide helps you upgrade your application from VisioForge .NET SDK v15.x to v2026.x. The v2026 SDK contains **two engine types**, and you can choose the migration path that best fits your needs:

- **Path A: Stay on DirectShow** — Minimal code changes, same Windows-only architecture. Best for production applications that need a quick, low-risk upgrade.
- **Path B: Migrate to X-engines** — Significant API changes, but you gain cross-platform support, modern codecs, and new features. Best for applications planning a major update.

> **Important:** The legacy DirectShow classes (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) are **still fully supported and actively updated** in v2026. You do not need to migrate to X-engines immediately. We recommend Path A first, then migrate to X-engines when your schedule allows.

## Target framework changes

The following table shows supported target frameworks in v2026:

| Framework | DirectShow engines | X-engines |
|-----------|-------------------|-----------|
| .NET Framework 4.6.1+ | Supported | Supported |
| .NET Core 3.1 | Supported | Supported |
| .NET 5.0 / 6.0 / 7.0 | Supported | Supported |
| .NET 8.0 | Supported | Supported |
| .NET 9.0 | Supported | Supported |
| .NET 10.0 | Supported | Supported |

> **Note:** DirectShow classes require a Windows TFM (e.g., `net10.0-windows`) or .NET Framework. For X-engines, platform-specific TFMs control the target: `net10.0-windows` for Windows, `net10.0-macos` for macOS, `net10.0-ios` for iOS, `net10.0-android` for Android. Plain `net10.0` (no platform suffix) targets Linux only.

---

## Path A: DirectShow v15 → v2026 DirectShow (Minimal Impact)

This path keeps your existing DirectShow-based code with minimal changes.

### NuGet package changes

| v15 Package | v2026 Package |
|-------------|---------------|
| `VisioForge.DotNet.Core` | `VisioForge.DotNet.Core` (same name, new version) |

Product-specific packages are also available:

- `VisioForge.DotNet.VideoCapture`
- `VisioForge.DotNet.VideoEdit`
- `VisioForge.DotNet.MediaPlayer`

### Namespace changes

**No namespace changes required.** All DirectShow namespaces remain the same:

| Namespace | Status |
|-----------|--------|
| `VisioForge.Core.VideoCapture` | Unchanged |
| `VisioForge.Core.VideoEdit` | Unchanged |
| `VisioForge.Core.MediaPlayer` | Unchanged |
| `VisioForge.Core.Types` | Unchanged |
| `VisioForge.Core.Types.VideoCapture` | Unchanged |
| `VisioForge.Core.Types.Output` | Unchanged |
| `VisioForge.Core.Types.VideoEffects` | Unchanged |
| `VisioForge.Core.Types.AudioEffects` | Unchanged |
| `VisioForge.Core.Types.Events` | Unchanged |
| `VisioForge.Core.Types.MediaPlayer` | Unchanged |
| `VisioForge.Core.Types.VideoEdit` | Unchanged |

### API changes

The DirectShow API is largely unchanged from v15 to v2026:

- **No SDK initialization required** (same as v15)
- Same class names: `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`
- Same creation patterns: `new VideoCaptureCore(videoView)`, `new MediaPlayerCore(videoView)`, `new VideoEditCore(videoView)`
- Same device enumeration: `VideoCapture1.Video_CaptureDevices()`, `.Audio_CaptureDevices()`
- Same output configuration: `VideoCapture1.Output_Format = mp4Output;`
- Same mode setting: `VideoCapture1.Mode = VideoCaptureMode.VideoCapture;`
- Same event names: `OnError`, `OnStop`, etc.

### Troubleshooting

| Problem | Solution |
|---------|----------|
| `VisioForge.Core.Types` namespace not found | Verify the correct NuGet package is installed (`VisioForge.DotNet.Core` or product-specific package) |
| Types or classes missing | For .NET 8+, ensure your TFM includes `-windows` (e.g., `net8.0-windows`, not `net8.0`). For .NET Framework, use `net461` or `net472`. |
| WPF `VideoView` not found | Project must target a Windows TFM (e.g., `net8.0-windows`) with `<UseWPF>true</UseWPF>` |

---

## Path B: Migrate to X-Engines (Cross-Platform)

This path migrates from DirectShow classes to the new cross-platform X-engine classes. This is a more significant change but provides major benefits.

### Why migrate to X-engines?

- **Cross-platform**: Windows, macOS, Linux, iOS, Android
- **Modern codecs**: AV1, HEVC with hardware encoding (NVIDIA NVENC, AMD AMF, Intel QSV)
- **Multiple simultaneous outputs**: Record to MP4 + stream to RTMP at the same time
- **New streaming protocols**: SRT, RIST, WebRTC WHIP, HLS, DASH
- **Async-first API**: Full async/await support throughout
- **MediaBlocks pipeline**: Build custom media processing pipelines with composable blocks

### NuGet packages

You need additional packages for X-engines:

```xml
<!-- Core SDK -->
<PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />

<!-- Platform runtime (required for X-engines) -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />

<!-- UI package (choose one based on your UI framework) -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WPF" Version="2026.*" />
<!-- OR -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WinForms" Version="2026.*" />
<!-- OR -->
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<!-- OR -->
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

### SDK initialization (required for X-engines)

X-engines require explicit initialization at application startup and cleanup at shutdown. **This is not required for legacy DirectShow classes.**

```csharp
// At application startup (e.g., Window_Loaded, OnStartup)
await VisioForgeX.InitSDKAsync();

// At application shutdown (e.g., Window_Closing, OnExit)
VisioForgeX.DestroySDK();
```

> **Warning:** If the SDK is not properly deinitialized, the application may hang on exit.

### Namespace migration

| v15 (DirectShow) Namespace | v2026 (X-Engine) Namespace |
|---|---|
| `VisioForge.Core.VideoCapture` | `VisioForge.Core.VideoCaptureX` |
| `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| `VisioForge.Core.MediaPlayer` | `VisioForge.Core.MediaPlayerX` |
| `VisioForge.Core.Types.VideoCapture` | `VisioForge.Core.Types.X.Sources` + `VisioForge.Core.Types.X.VideoCapture` |
| `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| `VisioForge.Core.Types.AudioEffects` | `VisioForge.Core.Types.X.AudioEffects` |
| `VisioForge.Core.Types.Events` | `VisioForge.Core.Types.Events` (unchanged) |
| `VisioForge.Core.Types.MediaPlayer` | `VisioForge.Core.Types.X.Sources` |
| `VisioForge.Core.Types.VideoEdit` | `VisioForge.Core.Types.X.VideoEdit` |
| — (new) | `VisioForge.Core.Types.X.AudioRenderers` |
| — (new) | `VisioForge.Core.Types.X.VideoEncoders` |
| — (new) | `VisioForge.Core.Types.X.AudioEncoders` |
| — (new) | `VisioForge.Core.Types.X.Sinks` |
| — (new) | `VisioForge.Core.MediaBlocks` |

### Class migration

| v15 (DirectShow) | v2026 (X-Engine) | Notes |
|---|---|---|
| `VideoCaptureCore` | `VideoCaptureCoreX` | Direct constructor instead of factory |
| `VideoEditCore` | `VideoEditCoreX` | |
| `MediaPlayerCore` | `MediaPlayerCoreX` | |
| `VideoCaptureSource` | `VideoCaptureDeviceSourceSettings` | Strongly typed |
| `AudioCaptureSource` | via `device.CreateSourceSettingsVC()` | Device-based creation |
| `MP4Output` | `MP4Output` | Same name, different namespace |
| `MP4HWOutput` | `MP4Output` with HW encoder | See encoder section |
| `AVIOutput` | `AVIOutput` | Same name, different namespace |
| `WMVOutput` | `WMVOutput` | Same name, different namespace |
| `MOVOutput` | `MOVOutput` | Same name, different namespace |
| `MPEGTSOutput` | `MPEGTSOutput` | Same name, different namespace |
| `WebMOutput` | `WebMOutput` | Same name, different namespace |
| `VideoCaptureMode` | Not needed | Preview is default; add outputs for recording |

---

### Video Capture: Side-by-side migration

#### Engine creation

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoCapture;

VideoCaptureCore VideoCapture1;
VideoCapture1 = await VideoCaptureCore.CreateAsync(VideoView1 as IVideoView);

// v2026 (X-Engine)
using VisioForge.Core.VideoCaptureX;

VideoCaptureCoreX VideoCapture1;
await VisioForgeX.InitSDKAsync();  // One-time at app startup
VideoCapture1 = new VideoCaptureCoreX(VideoView1 as IVideoView);
```

#### Device enumeration

```csharp
// v15 (DirectShow) — synchronous, instance method
foreach (var device in VideoCapture1.Video_CaptureDevices())
{
    cbVideoInputDevice.Items.Add(device.Name);
}

foreach (var device in VideoCapture1.Audio_CaptureDevices())
{
    cbAudioInputDevice.Items.Add(device.Name);
}

foreach (string device in VideoCapture1.Audio_OutputDevices())
{
    cbAudioOutputDevice.Items.Add(device);
}
```

```csharp
// v2026 (X-Engine) — async, shared singleton
using VisioForge.Core.MediaBlocks;

// Option 1: One-time enumeration
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
foreach (var device in videoDevices)
{
    cbVideoInputDevice.Items.Add(device.DisplayName);  // Note: DisplayName, not Name
}

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
foreach (var device in audioDevices)
{
    cbAudioInputDevice.Items.Add(device.DisplayName);
}

var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
foreach (var device in audioOutputs)
{
    cbAudioOutputDevice.Items.Add(device.DisplayName);
}

// Option 2: Hot-plug monitoring (new feature)
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) =>
{
    cbVideoInputDevice.Items.Add(device.DisplayName);
};
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();
```

#### Video source configuration

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbVideoInputDevice.Text);
VideoCapture1.Video_CaptureDevice.Format = cbVideoInputFormat.Text;
VideoCapture1.Video_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Video_CaptureDevice.FrameRate = new VideoFrameRate(30.0);
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Sources;

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var deviceItem = devices.FirstOrDefault(d => d.DisplayName == cbVideoInputDevice.Text);

var videoSourceSettings = new VideoCaptureDeviceSourceSettings(deviceItem);

// Set format (typed, not string-based)
var formatItem = deviceItem.VideoFormats
    .FirstOrDefault(f => f.Name == cbVideoInputFormat.Text);
if (formatItem != null)
{
    videoSourceSettings.Format = formatItem.ToFormat();
    videoSourceSettings.Format.FrameRate = new VideoFrameRate(30.0);
}

VideoCapture1.Video_Source = videoSourceSettings;
```

#### Audio source configuration

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(cbAudioInputDevice.Text);
VideoCapture1.Audio_CaptureDevice.Format = cbAudioInputFormat.Text;
VideoCapture1.Audio_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Audio_OutputDevice = cbAudioOutputDevice.Text;
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioRenderers;

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var deviceItem = audioDevices.FirstOrDefault(d => d.DisplayName == cbAudioInputDevice.Text);

var formatItem = deviceItem.Formats
    .FirstOrDefault(f => f.Name == cbAudioInputFormat.Text);
IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(formatItem?.ToFormat());
VideoCapture1.Audio_Source = audioSource;

// Audio output device
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var outputDevice = audioOutputs.FirstOrDefault(d => d.DisplayName == cbAudioOutputDevice.Text);
VideoCapture1.Audio_OutputDevice = new AudioRendererSettings(outputDevice);
```

#### Output and recording

```csharp
// v15 (DirectShow) — single output
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
VideoCapture1.Output_Filename = "output.mp4";

var mp4Output = new MP4Output();
// configure mp4Output...
VideoCapture1.Output_Format = mp4Output;

await VideoCapture1.StartAsync();
```

```csharp
// v2026 (X-Engine) — multiple simultaneous outputs
using VisioForge.Core.Types.X.Output;

// No Mode property needed — preview is default
// Add output(s) for recording
VideoCapture1.Outputs_Add(new MP4Output("output.mp4"), false);

// You can add multiple outputs simultaneously:
// VideoCapture1.Outputs_Add(new WebMOutput("output.webm"), false);

await VideoCapture1.StartAsync();
```

#### Preview only (no recording)

```csharp
// v15 (DirectShow)
VideoCapture1.Mode = VideoCaptureMode.VideoPreview;
await VideoCapture1.StartAsync();

// v2026 (X-Engine) — just don't add any outputs
await VideoCapture1.StartAsync();
```

#### Hardware-accelerated encoding

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.Output;

var mp4Output = new MP4HWOutput();
VideoCapture1.Output_Format = mp4Output;
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;

var mp4Output = new MP4Output("output.mp4");
mp4Output.Video = new NVENCH264EncoderSettings();  // NVIDIA
// or: mp4Output.Video = new QSVH264EncoderSettings();  // Intel
// or: mp4Output.Video = new AMFH264EncoderSettings();  // AMD
VideoCapture1.Outputs_Add(mp4Output, false);
```

#### Volume control

```csharp
// v15 (DirectShow)
VideoCapture1.Audio_OutputDevice_Volume_Set(70);
VideoCapture1.Audio_OutputDevice_Balance_Set(0);

// v2026 (X-Engine)
VideoCapture1.Audio_OutputDevice_Volume = 0.7f;  // 0.0 to 1.0
```

#### Cleanup

```csharp
// v15 (DirectShow)
VideoCapture1.Dispose();

// v2026 (X-Engine)
await VideoCapture1.DisposeAsync();
VisioForgeX.DestroySDK();  // At app shutdown
```

---

### Media Player: Side-by-side migration

#### Engine creation and playback

```csharp
// v15 (DirectShow)
using VisioForge.Core.MediaPlayer;

MediaPlayerCore _player;
_player = new MediaPlayerCore(VideoView1 as IVideoView);
_player.Audio_OutputDevice = cbAudioOutput.Text;
_player.Playlist_Clear();
_player.Playlist_Add("video.mp4");
await _player.PlayAsync();
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();  // One-time at app startup

MediaPlayerCoreX _player;
_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri("video.mp4"));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

#### Key differences

| Feature | v15 (DirectShow) | v2026 (X-Engine) |
|---------|-------------------|-------------------|
| Source setup | `Playlist_Add("file.mp4")` | `OpenAsync(UniversalSourceSettingsV2)` |
| Position | `_player.Position` (property) | `await _player.Position_GetAsync()` |
| Duration | `_player.Duration` (property) | `await _player.DurationAsync()` |
| Version | `_player.SDK_Version()` (instance) | `MediaPlayerCoreX.SDK_Version` (static) |
| Loop | `_player.Looping` | `_player.Loop` |

---

### Video Edit: Side-by-side migration

#### Engine creation

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

VideoEditCore VideoEdit1;
VideoEdit1 = new VideoEditCore(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Filename = "output.mp4";
VideoEdit1.Output_Format = new MP4Output();
await VideoEdit1.StartAsync();
```

```csharp
// v2026 (X-Engine)
using VisioForge.Core.VideoEditX;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;

await VisioForgeX.InitSDKAsync();  // One-time at app startup

VideoEditCoreX VideoEdit1;
VideoEdit1 = new VideoEditCoreX(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Format = new MP4Output("output.mp4");
await VideoEdit1.StartAsync();
```

#### Key differences

| Feature | v15 (DirectShow) | v2026 (X-Engine) |
|---------|-------------------|-------------------|
| Namespace | `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| Output types | `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| Output filename | Separate `Output_Filename` property | Included in output constructor: `new MP4Output("path")` |
| Effects | `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| Video size | `VisioForge.Core.Types.Size` | `VisioForge.Core.Types.Size` (unchanged) |

---

### Events

Most events keep the same names across both engines:

| Event | v15 | v2026 X-Engine | Notes |
|-------|-----|----------------|-------|
| `OnError` | Yes | Yes | `ErrorsEventArgs` (same) |
| `OnStop` | Yes | Yes | |
| `OnStart` | Yes | Yes | |
| `OnAudioVUMeter` | — | Yes | New in X-engine |
| `OnOutputStarted` | — | Yes | New — per-output events |
| `OnOutputStopped` | — | Yes | New — per-output events |
| `OnBarcodeDetected` | — | Yes | New |
| `OnFaceDetected` | — | Yes | New |
| `OnMotionDetection` | Yes | Yes | |
| `OnVideoFrameBuffer` | Yes | Yes | |

---

## FAQ

### "VisioForge.Core.Types namespace is no more available"

This depends on which engine you're using:

- **DirectShow classes**: `VisioForge.Core.Types` still exists. Make sure you have the correct NuGet package installed. For .NET 8+, ensure your TFM includes `-windows`.
- **X-engine classes**: Use `VisioForge.Core.Types.X.*` sub-namespaces (e.g., `VisioForge.Core.Types.X.Output`, `VisioForge.Core.Types.X.Sources`).

### "Can I keep using DirectShow classes (VideoCaptureCore, etc.)?"

Yes. DirectShow classes are fully supported and actively updated in v2026. You can upgrade the NuGet package and keep your existing code with minimal changes.

### "MP4HWOutput is not found"

In X-engines, `MP4HWOutput` is replaced by `MP4Output` with a specific hardware encoder:

```csharp
var mp4 = new MP4Output("output.mp4");
mp4.Video = new NVENCH264EncoderSettings();  // or QSVH264, AMFH264, etc.
```

### "VideoCaptureMode is not found"

X-engines don't use a `Mode` property. Preview is the default behavior. To record, add outputs with `Outputs_Add()`.

### "Audio_CaptureDevices() method is not found"

In X-engines, device enumeration uses a shared async singleton:

```csharp
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
```

### "What is the recommended migration strategy for production applications?"

1. **First**: Upgrade to v2026 keeping DirectShow classes (Path A) — low risk, minimal code changes
2. **Test**: Verify your application works correctly with the new package version
3. **Then**: When ready, migrate to X-engines (Path B) — module by module if needed
4. **Both engines can coexist** in the same application during the transition period

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
