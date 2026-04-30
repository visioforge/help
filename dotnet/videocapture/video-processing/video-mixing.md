---
title: Video Mixing and Picture-in-Picture Layout in C# .NET
description: Implement Picture-in-Picture with multiple video sources in .NET using C# code examples for mixing streams with custom layouts.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Mixing
  - IP Camera
  - Screen Capture
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - ScreenCaptureSourceSettings
  - VideoMixerSourceSettings
  - IPCameraSourceSettings
  - VideoCaptureDeviceSourceSettings

---

# Mixing Multiple Video Streams in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Video Stream Mixing

The SDK provides powerful capabilities for mixing several video streams with different resolutions and frame rates. This functionality is essential for creating professional video applications that require Picture-in-Picture effects, multi-camera views, or combined display outputs.

## Key Features of Video Mixing

- Combine multiple video sources in a single output
- Support for different video resolutions and frame rates
- Multiple layout options (horizontal, vertical, grid, custom)
- Real-time position and parameter adjustments
- Transparency and flip controls
- Support for diverse input sources (cameras, screens, IP cameras)

## VideoCaptureCore engine

The implementation process involves three main steps:

1. Setting up the video mixing mode
2. Adding multiple video sources
3. Configuring and adjusting source parameters

Let's explore each step in detail with code examples.

### Setting Video Mixing Mode

The SDK supports various predefined layouts as well as custom configurations. Select the appropriate mode based on your application requirements.

#### Horizontal Stack Layout

This mode arranges all video sources horizontally in a row:

```cs
VideoCapture1.PIP_Mode = PIPMode.Horizontal;
```

#### Vertical Stack Layout

This mode arranges all video sources vertically in a column:

```cs
VideoCapture1.PIP_Mode = PIPMode.Vertical;
```

#### Grid Layout (2×2)

This mode organizes up to four video sources in a square grid:

```cs
VideoCapture1.PIP_Mode = PIPMode.Mode2x2;
```

#### Custom Layout with Specific Resolution

For advanced scenarios, you can define a custom layout with precise output dimensions:

```cs
VideoCapture1.PIP_Mode = PIPMode.Custom;
VideoCapture1.PIP_CustomOutputSize_Set(1920, 1080);
```

### Adding Video Sources

The first configured source serves as the main input, while additional sources can be added using the PIP API.

#### Adding a Video Capture Device

To incorporate a camera or other video capture device:

```cs
// Signature: (string deviceName, string format, bool useBestFormat, VideoFrameRate frameRate,
//             string crossbarInput, int left, int top, int width, int height) → bool
VideoCapture1.PIP_Sources_Add_VideoCaptureDevice(
    deviceName:     "USB Camera",                     // name from Video_CaptureDevices()
    format:         "1280x720 30fps",                 // format string, or empty + useBestFormat=true
    useBestFormat:  false,
    frameRate:      new VideoFrameRate(30),
    input:          null,                             // crossbar input name, or null
    left: 100, top: 100, width: 320, height: 240);
```

#### Adding an IP Camera Source

For network-based cameras:

```cs
var ipCameraSource = new IPCameraSourceSettings
{
    URL = new Uri("camera url") // URL is Uri, not string
};

// Set additional IP camera parameters as needed
// Authentication, protocol, buffering, etc.

VideoCapture1.PIP_Sources_Add_IPCamera(
    ipCameraSource,
    left,
    top,
    width,
    height);
```

#### Adding a Screen Capture Source

To include desktop or application windows:

```cs
ScreenCaptureSourceSettings screenSource = new ScreenCaptureSourceSettings();
screenSource.Mode = ScreenCaptureMode.Screen;
screenSource.FullScreen = true;
VideoCapture1.PIP_Sources_Add_ScreenSource(
    screenSource,
    left,
    top,
    width,
    height);
```

### Dynamic Source Adjustments

A major advantage of the SDK is the ability to adjust source parameters in real-time during operation.

#### Repositioning Sources

You can modify the position and dimensions of any source:

```cs
// SetSourcePositionAsync takes (int index, Rectangle rect).
await VideoCapture1.PIP_Sources_SetSourcePositionAsync(
    index,                                 // 0 is main source, 1+ are additional sources
    new System.Drawing.Rectangle(left, top, width, height));
```

#### Adjusting Visual Properties

Fine-tune the appearance with transparency and orientation controls:

```cs
int transparency = 127; // Range: 0-255
bool flipX = false;
bool flipY = false;

// SetSourceSettingsAsync takes (int index, int transparency, bool flipX, bool flipY, bool disabled = false).
await VideoCapture1.PIP_Sources_SetSourceSettingsAsync(
    index,
    transparency,
    flipX,
    flipY);
```

### Required Dependencies

To use video mixing functionality, ensure you include the appropriate redistributable packages:

- Video capture redistributables:
  - [x86 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## VideoCaptureCoreX engine

The VideoCaptureCoreX engine is a newer version of the VideoCaptureCore engine that provides additional features and improvements.

### Video Mixing with VideoCaptureCoreX

The VideoCaptureCoreX engine offers a more flexible and powerful approach to video mixing through the `VideoMixerSourceSettings` class. This allows for precise control over multiple video sources and their layout.

#### Basic Setup

```cs
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.MediaInfo;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;

// Create a video mixer with 1080p resolution at 30fps
var videoMixer = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Initialize the VideoCaptureCoreX engine
var captureX = new VideoCaptureCoreX();
captureX.Video_Source = videoMixer;
```

#### Adding Multiple Sources

You can add various video sources to the mixer, positioning each one exactly where needed:

```cs
// Rect uses (left, top, right, bottom) — NOT (left, top, width, height). Width and
// Height are computed as Right-Left / Bottom-Top, so passing width/height here yields
// negative dimensions for any non-origin rectangle.

// Add a camera as the first source (full-screen background, 1920×1080 canvas).
var cameraSource = new VideoCaptureDeviceSourceSettings();

// Configure camera settings
// ...

videoMixer.Add(cameraSource, new Rect(0, 0, 1920, 1080));

// Add a screen capture source in the top-right corner, 640×480 region.
var screenSource = new ScreenCaptureDX9SourceSettings();
screenSource.CaptureCursor = true;
screenSource.FrameRate = VideoFrameRate.FPS_30;

// left = 1280, top = 0, right = 1280+640 = 1920, bottom = 0+480 = 480.
videoMixer.Add(screenSource, new Rect(1280, 0, 1920, 480));
```

#### Reconfiguring sources before Start

`VideoMixerSourceSettings` is configuration — its sources are baked in at `StartAsync`. To change the layout you mutate the settings list (via `Get`/`RemoveAt`/`Add`) **before** starting the pipeline:

```cs
// Inspect a source at index 1 (returns a Tuple<IVideoMixerSource, Rect, ChromaKeySettingsX>).
var (source, currentRect, chromaKey) = videoMixer.Get(1);

// Swap its rectangle by removing and re-adding in place.
videoMixer.RemoveAt(1);
videoMixer.Add(source, new Rect(0, 0, 1280, 720), chromaKey);
```

For true **runtime** layout changes (update position while the pipeline is live), drop down to Media Blocks: build your pipeline around a `VideoMixerBlock` and use its `Input_Get(Guid)` / `Input_Update(VideoMixerStream)` methods to mutate stream position, size, alpha, or z-order without restarting. See [Media Blocks video-processing reference](../../mediablocks/VideoProcessing/index.md) for the `VideoMixerBlock` API.

#### Output Configuration

Configure the output destination for your mixed video:

```cs
// Set up MP4 recording
captureX.Outputs_Add(new MP4Output("output.mp4")); 

// Start capturing
await captureX.StartAsync();

// Stop after some time
await Task.Delay(TimeSpan.FromMinutes(5));
await captureX.StopAsync();
```

### Comparison with VideoCaptureCore Engine

| Feature | VideoCaptureCore | VideoCaptureCoreX |
|---------|------------------|-------------------|
| API Style | Method-based approach | Object-oriented with settings classes |
| Flexibility | Predefined layouts | Fully customizable source positioning |
| Source Management | Fixed API for adding sources | Add any source implementing IVideoMixerSource |
| Performance | Good | Enhanced with optimized rendering pipeline |
| Real-time Changes | Limited | Comprehensive source manipulation |

### Required Dependencies

Check the main [Deployment](../deployment.md) page for the latest dependencies.

## Advanced Usage Scenarios

The video mixing capabilities enable numerous application scenarios:

- Multi-camera security monitoring
- Video conferencing with screen sharing
- Broadcasting with commentator overlay
- Educational presentations with multiple inputs
- Gaming streams with player camera

## Troubleshooting Tips

- Ensure hardware resources are sufficient for the number of streams
- Monitor CPU and memory usage during operation
- Consider lower resolutions for smoother performance with many sources
- Test different layout configurations for optimal visual results

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for additional code samples and implementation examples.