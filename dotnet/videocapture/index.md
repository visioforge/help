---
title: Video Capture API for C# .NET — Webcam, Screen, RTSP
description: Record webcam, screen, and IP camera (RTSP/ONVIF) to MP4 with VisioForge Video Capture SDK. Cross-platform async API with GPU encoding for WinForms, WPF, MAUI.
sidebar_label: Video Capture SDK .NET
order: 15

---

# Video Capture SDK for C# .NET — Webcam, Screen, and IP Camera Capture API

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The Video Capture SDK for .NET is a C# video capture API that lets you record from webcams, IP cameras (RTSP/ONVIF), and screens in your .NET applications. It replaces low-level DirectShow video capture code with a modern async API — enumerate devices, configure sources, and start recording in a few lines of C#.

The SDK handles device enumeration, format negotiation, GPU-accelerated encoding, and file output across Windows, macOS, and Linux. Whether you need to save an RTSP stream to file, capture photos from a webcam, or build a screen capture tool, the API covers it.

## Quick Start

### 1. Install NuGet Packages

```bash
dotnet add package VisioForge.DotNet.VideoCapture
```

Add platform-specific native dependencies:

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

For the full list of packages and UI framework support (WinForms, WPF, MAUI, Avalonia), see the [Installation Guide](../install/index.md).

### 2. Initialize the SDK

Call `InitSDKAsync()` once at application startup before using any capture functionality:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. C# Webcam Capture to MP4 (Minimal Example)

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// Create capture instance linked to a VideoView control
var capture = new VideoCaptureCoreX(videoView);

// Enumerate available devices
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Configure video and audio sources
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// Add MP4 output (H.264 + AAC, GPU-accelerated when available)
capture.Outputs_Add(new MP4Output("recording.mp4"));

// Start capture
await capture.StartAsync();

// ... when done:
await capture.StopAsync();
await capture.DisposeAsync();
```

### 4. Cleanup at Shutdown

```csharp
VisioForgeX.DestroySDK();
```

## Core Workflow

Every capture application follows the same pattern:

1. **Initialize SDK** — `VisioForgeX.InitSDKAsync()` (once per app lifetime)
2. **Enumerate devices** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`
3. **Create capture object** — `new VideoCaptureCoreX(videoView)` for preview, or without a view for headless recording
4. **Configure source** — Set `Video_Source` to a webcam, IP camera, screen, or other source
5. **Configure output** — Add one or more outputs: `MP4Output`, `WebMOutput`, `AVIOutput`, etc.
6. **Start** — `await capture.StartAsync()`
7. **Stop** — `await capture.StopAsync()` finalizes the output file
8. **Dispose** — `await capture.DisposeAsync()` releases all resources

## Common C# Video Capture Scenarios

### Record Webcam Video in C#

Record from a USB webcam or built-in camera to MP4 with H.264/AAC:

```csharp
capture.Video_Source = new VideoCaptureDeviceSourceSettings(device);
capture.Outputs_Add(new MP4Output("webcam.mp4"));
```

See the full tutorial: [Save Webcam Video in C#](guides/save-webcam-video.md)

### Save RTSP Stream to File in C#

Connect to IP cameras and save RTSP streams to MP4 files. Supports RTSP, ONVIF, and HTTP protocols:

```csharp
capture.Video_Source = new RTSPSourceSettings(
    new Uri("rtsp://192.168.1.100:554/stream"))
{
    Login = "admin",
    Password = "password"
};
capture.Outputs_Add(new MP4Output("ipcam.mp4"));
```

See: [IP Camera to MP4](video-tutorials/ip-camera-capture-mp4.md) | [RTSP Guide](video-sources/ip-cameras/rtsp.md) | [ONVIF Guide](video-sources/ip-cameras/onvif.md)

### C# Screen Capture to MP4

Record the desktop or a specific screen region to MP4:

```csharp
capture.Video_Source = new ScreenCaptureSourceSettings()
{
    FrameRate = new VideoFrameRate(30),
    CaptureCursor = true
};
capture.Outputs_Add(new MP4Output("screen.mp4"));
```

See: [Screen Capture to MP4](video-tutorials/screen-capture-mp4.md)

### Audio-Only Recording

Record from a microphone without video:

```csharp
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;
capture.Video_Record = false;
capture.Outputs_Add(new MP4Output("audio.mp4"));
```

### Capture Photo from Webcam

Grab a still image from the live webcam video feed:

```csharp
capture.Snapshot_Grabber_Enabled = true;
await capture.StartAsync();

// Later, save a frame:
await capture.Snapshot_SaveAsync("photo.jpg", SKEncodedImageFormat.Jpeg, 85);
```

See: [Webcam Photo Capture](guides/make-photo-using-webcam.md)

## Supported Input Devices

### Cameras and Capture Hardware

* USB webcams and capture devices (up to 4K)
* DV and HDV MPEG-2 camcorders
* Professional PCI capture cards
* Television tuners (with and without MPEG encoder)

### Network and IP Cameras

* RTSP-enabled IP cameras
* JPEG/MJPEG HTTP cameras
* RTMP streaming sources
* ONVIF-compatible cameras
* SRT and NDI sources

Browse our [IP camera brands directory](../camera-brands/index.md) for brand-specific RTSP URLs and connection guides covering 60+ manufacturers.

### Professional and Industrial Equipment

* Blackmagic Decklink capture devices
* Microsoft Kinect and Kinect 2
* GenICam / GigE Vision / USB3 Vision industrial cameras

### Audio Sources

* System audio capture devices and sound cards
* Professional ASIO audio devices
* Audio loopback (system sound) capture

## Output Formats and Encoders

The SDK supports multiple output containers and codecs. GPU-accelerated encoding (NVIDIA NVENC, AMD AMF, Intel Quick Sync) is used automatically when available.

| Format | Video Codecs | Audio Codecs | Use Case |
| ------ | ------------ | ------------ | -------- |
| MP4 | H.264, H.265 (HEVC) | AAC | General purpose, widest compatibility |
| WebM | VP8, VP9, AV1 | Vorbis, Opus | Web delivery, royalty-free |
| AVI | MJPEG, custom codecs | PCM, MP3 | Legacy compatibility |
| MKV | H.264, H.265, VP9 | AAC, Vorbis, FLAC | Flexible container, multiple tracks |
| GIF | Animated GIF | — | Short clips, previews |

For detailed codec configuration, see [Output Formats](../general/output-formats/mp4.md) and [Video Encoders](../general/video-encoders/h264.md).

## Platform Support

| Platform | UI Frameworks | Notes |
| -------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | Full feature set including DirectShow sources |
| macOS | MAUI, Avalonia, Console | AVFoundation camera access |
| Linux x64 | Avalonia, Console | V4L2 camera access |
| Android | MAUI | Via MAUI camera integration |
| iOS | MAUI | Via MAUI camera integration |

## Migrating from DirectShow Video Capture

If you're currently using DirectShow (DirectShow.NET) for video capture in C#, the Video Capture SDK provides a modern replacement. Instead of manually building filter graphs with `IGraphBuilder`, connecting pins, and managing COM objects, you use typed settings classes and async methods.

| DirectShow Concept | Video Capture SDK Equivalent |
| ------------------ | ---------------------------- |
| `IGraphBuilder` filter graph | Managed automatically by `VideoCaptureCoreX` |
| `ICaptureGraphBuilder2` | Not needed — source and output configured via properties |
| Device enumeration via `DsDevice` | `DeviceEnumerator.Shared.VideoSourcesAsync()` |
| `ISampleGrabber` for frame access | `Snapshot_SaveAsync()` or `OnVideoFrameBuffer` event |
| Manual pin connection | Automatic — or use [Media Blocks SDK](../mediablocks/GettingStarted/index.md) for explicit pipelines |
| COM cleanup / `Marshal.ReleaseComObject` | Standard `IAsyncDisposable` pattern |

For a detailed migration guide with side-by-side code examples, see [Migrate from DirectShow.NET](https://www.visioforge.com/compare/migrate-from-directshow-net).

## Developer Documentation

### Core Functionality

* [Configuring Video Sources](video-sources/index.md)
* [Configuring Audio Sources](audio-sources/index.md)
* [Video Processing and Effects](video-processing/index.md)
* [Audio Rendering](audio-rendering/index.md)

### Advanced Features

* [Video Capture Implementation](video-capture/index.md)
* [Audio Capture Implementation](audio-capture/index.md)
* [Motion Detection](motion-detection/index.md)
* [Network Streaming](network-streaming/index.md)
* [Pre-Event Recording](guides/pre-event-recording.md)
* [Synchronizing Multiple Captures](guides/start-in-sync.md)

### Tutorials (Step-by-Step with Code)

* [Webcam to MP4](video-tutorials/video-capture-webcam-mp4.md) | [to AVI](video-tutorials/video-capture-camera-avi.md) | [to WMV](video-tutorials/video-capture-webcam-wmv.md)
* [Screen Capture to MP4](video-tutorials/screen-capture-mp4.md) | [to AVI](video-tutorials/screen-capture-avi.md) | [to WMV](video-tutorials/screen-capture-wmv.md)
* [IP Camera Preview](video-tutorials/ip-camera-preview.md) | [IP Camera to MP4](video-tutorials/ip-camera-capture-mp4.md)
* [Text Overlay on Webcam](video-tutorials/webcam-capture-text-overlay.md)
* [Camera Video Preview](video-tutorials/camera-video-preview.md)
* [All Video Tutorials](video-tutorials/index.md)

### Guides

* [Save Webcam Video in C#](guides/save-webcam-video.md)
* [Record Webcam in VB.NET](guides/record-webcam-vb-net.md)
* [Screen Capture in VB.NET](guides/screen-capture-vb-net.md)
* [Webcam Photo Capture](guides/make-photo-using-webcam.md)
* [All Guides](guides/index.md)

### Integration

* [Third-Party Software Integration](3rd-party-software/index.md)
* [Computer Vision (Face Detection)](computer-vision/index.md)
* [MAUI Camera Recording](maui/camera-recording-maui.md)

## Developer Resources

* [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Deployment Guidelines](deployment.md)
* [API Reference](https://api.visioforge.org/dotnet/api/index.html)
* [Changelog](../changelog.md)
* [End User License Agreement](../../eula.md)
* [Licensing Information](../../../licensing.md)
