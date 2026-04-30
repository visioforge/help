---
title: VisioForge Video Capture SDK in C# .NET — Cheat Sheet
description: One-page Video Capture SDK reference with NuGet packages, VideoCaptureCoreX APIs, MP4 recording flow, platform support, and pitfalls.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - Capture
  - Recording
  - Streaming
  - Encoding
  - Webcam
  - IP Camera
  - Screen Capture
  - RTSP
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureDeviceSourceSettings
  - DeviceEnumerator
  - VideoView
  - MP4Output
---

# VisioForge Video Capture SDK .Net — Cheat Sheet

Video Capture SDK .Net captures from webcams, IP cameras (RTSP/ONVIF), screens, Decklink, and GenICam/GigE Vision devices on all 5 .NET platforms (Windows, macOS, Linux, Android, iOS). `VideoCaptureCoreX` is the cross-platform engine; sources are configured via typed settings classes such as `VideoCaptureDeviceSourceSettings`, and devices are listed asynchronously with `DeviceEnumerator`.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Platform support

- **Windows x64 / x86** — WinForms, WPF, MAUI, Avalonia, Console. Full DirectShow sources + NVENC / AMF / Intel Quick Sync hardware encoding.
- **macOS / macCatalyst** — MAUI, Avalonia, Console. AVFoundation camera sources + VideoToolbox hardware encoding.
- **Linux x64** — Avalonia, Console. V4L2 camera sources + NVENC / VA-API hardware encoding.
- **Android** — MAUI. Native camera sources + MediaCodec hardware encoding.
- **iOS** — MAUI. AVFoundation camera sources + VideoToolbox hardware encoding.

For the full codec × source × platform matrix see [platform-matrix.md](../platform-matrix.md).

## NuGet packages

Main SDK package (required, all platforms):

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.*" />
```

Platform-specific native runtimes — add the ones you target:

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- Windows x86 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86" Version="2025.11.0" />

<!-- macOS / macCatalyst -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.2.15" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />

<!-- Android / iOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.24" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
```

UI integration (pick the one that matches your UI stack):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

WinForms and WPF `VideoView` controls ship inside the main package. For the complete package list (including redistributables for the Windows-only `VideoCaptureCore` engine) see the [Installation Guide](../install/index.md).

## Primary API classes

| Class | Role | See also |
|---|---|---|
| `VideoCaptureCoreX` | Cross-platform capture engine — owns the pipeline, source, outputs, preview | [Quick Start](./index.md) |
| `VideoCaptureDeviceSourceSettings` | Configures a webcam / USB / DirectShow camera source | [Enumerate and select device](./video-sources/video-capture-devices/enumerate-and-select.md) |
| `DeviceEnumerator` | Async enumeration of video sources, audio sources, and audio outputs via `DeviceEnumerator.Shared` | [Video sources overview](./video-sources/index.md) |
| `VideoView` | WinForms / WPF / MAUI / Avalonia preview control bound to the capture engine | [Installation — video controls](../install/index.md) |
| `MP4Output` | Muxes captured streams to `.mp4` with H.264 / H.265 + AAC (GPU-accelerated when available) | [Webcam → MP4 tutorial](./video-tutorials/video-capture-webcam-mp4.md) |

Additional source settings classes: `RTSPSourceSettings` (IP cameras — construct via `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)`, not `new`), `ScreenCaptureD3D11SourceSettings` / `ScreenCaptureGDISourceSettings` / `ScreenCaptureDX9SourceSettings` (Windows screen), `ScreenCaptureMacOSSourceSettings` (macOS), `ScreenCaptureXDisplaySourceSettings` (Linux), `DecklinkVideoSourceSettings`, `GenICamSourceSettings`, `NDISourceSettings`.

## Canonical minimum example

Record the first webcam + default microphone to `recording.mp4`:

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// 1. Init SDK once at application startup.
await VisioForgeX.InitSDKAsync();

// 2. Enumerate connected devices (async — never block the UI thread).
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

if (videoDevices.Length == 0)
    throw new InvalidOperationException("No video capture devices found.");

// 3. Create the capture engine, bound to a VideoView for live preview.
//    Pass null instead of videoView for headless recording.
var capture = new VideoCaptureCoreX(videoView);

// 4. Configure video + audio sources.
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// 5. Add the MP4 output BEFORE calling StartAsync.
capture.Outputs_Add(new MP4Output("recording.mp4"));

// 6. Start capture — preview renders into videoView immediately.
await capture.StartAsync();

// ... later, when the user clicks Stop:
await capture.StopAsync();     // Finalizes and closes the MP4 file.
await capture.DisposeAsync();  // Releases the pipeline.

// 7. On application shutdown:
VisioForgeX.DestroySDK();
```

Swap `VideoCaptureDeviceSourceSettings(...)` for `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` for IP cameras, or `new ScreenCaptureD3D11SourceSettings()` for desktop recording (Windows) — the rest of the pipeline is identical.

## Typical workflow

1. **Init** — `await VisioForgeX.InitSDKAsync()` (once per app).
2. **Enumerate** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`.
3. **Configure source** — pick a settings class (`VideoCaptureDeviceSourceSettings`, `RTSPSourceSettings`, `ScreenCaptureD3D11SourceSettings` / `ScreenCaptureMacOSSourceSettings` / `ScreenCaptureXDisplaySourceSettings`, `DecklinkVideoSourceSettings`, ...) and assign to `capture.Video_Source`.
4. **Create engine** — `new VideoCaptureCoreX(videoView)` for preview, or `new VideoCaptureCoreX(null)` for headless.
5. **Add outputs** — `capture.Outputs_Add(new MP4Output(path))`, or `MKVOutput` / `WebMOutput` / `AVIOutput` / `RTMPOutput` / `HLSOutput`.
6. **Run** — `StartAsync()` → `StopAsync()` → `DisposeAsync()`, then `VisioForgeX.DestroySDK()` on shutdown.

## Common pitfalls

- **Device enumeration is async.** Always `await DeviceEnumerator.Shared.VideoSourcesAsync()` — calling `.Result` or `.Wait()` on the UI thread can deadlock. Audio uses its own `AudioSourcesAsync()` / `AudioOutputsAsync()` calls; the video enumerator does not cover them.
- **Outputs must be added before `StartAsync`.** `Outputs_Add(...)` during an active capture is not supported — stop, add the output, and start again.
- **Virtual Camera needs extra registration.** The `VirtualCamera` redist package must be registered (see the [deployment guide](./deployment.md)) before the virtual camera becomes visible to external applications like Zoom or Teams.
- **AnyCPU apps need both x86 and x64 redistributables.** On Windows, ship both architectures or force a specific one via `<PlatformTarget>` — partial deployment silently fails at source init time.
- **Mobile permissions must be granted before enumeration.** On Android and iOS, camera and microphone permissions must be requested and granted before `DeviceEnumerator` runs — otherwise the returned list is empty. See the [MAUI camera recording guide](./maui/camera-recording-maui.md) for per-platform permission setup.

## See also

- **Video sources**
    - Webcam / USB camera → [Enumerate and select a device](./video-sources/video-capture-devices/enumerate-and-select.md)
    - IP camera → [RTSP](./video-sources/ip-cameras/rtsp.md), [ONVIF](./video-sources/ip-cameras/onvif.md), [NDI](./video-sources/ip-cameras/ndi.md)
    - Screen capture → [screen.md](./video-sources/screen.md)
    - Decklink / GenICam → [decklink.md](./video-sources/decklink.md), [USB3 Vision / GigE / GenICam](./video-sources/usb3v-gige-genicam/index.md)
- **Outputs & streaming**
    - MP4 recording → [Webcam → MP4](./video-tutorials/video-capture-webcam-mp4.md), [IP camera → MP4](./video-tutorials/ip-camera-capture-mp4.md)
    - Network streaming (RTMP / RTSP / HLS / SRT / NDI) → [network-streaming](./network-streaming/index.md)
- **Platform & deployment**
    - Windows / macOS / Ubuntu / Android / iOS → [../deployment-x/](../deployment-x/index.md)
    - Install & target frameworks → [../install/index.md](../install/index.md)
    - Full codec × source matrix → [../platform-matrix.md](../platform-matrix.md)
- **MAUI**
    - Cross-platform camera recording → [MAUI camera recording](./maui/camera-recording-maui.md)

## FAQ

### How do I list connected cameras?

`var cams = await DeviceEnumerator.Shared.VideoSourcesAsync();` — returns a list of `VideoCaptureDeviceInfo` with friendly names, supported formats, and frame rates.

### Does this SDK support IP cameras?

Yes. Build settings via the async factory — `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` — and pass to `VideoCaptureCoreX.Video_Source`. The constructor is private, so `new RTSPSourceSettings(...)` will not compile. ONVIF and HTTP-JPEG cameras are also supported. See the [IP camera brands directory](../camera-brands/index.md) for vendor-specific URLs.

### What file formats can I record to?

MP4 (H.264 / H.265 + AAC), MKV, WebM (VP8 / VP9 / AV1 + Opus / Vorbis), AVI, GIF, plus live RTMP / RTSP / HLS / SRT / NDI streaming. See the output-format tables in [the SDK overview](./index.md).
