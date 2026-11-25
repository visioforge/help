---
title: Video Renderer in C# .NET - DirectX, OpenGL, Metal
description: Display video in WPF, WinForms, Avalonia, and MAUI apps with hardware-accelerated rendering using VisioForge Media Blocks SDK for .NET.
sidebar_label: Video Renderer
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - VideoRendererBlock
  - UniversalSourceBlock
  - VideoView
  - MediaBlocksPipeline
  - UniversalSourceSettings

---

# Video Renderer Block

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

The Video Renderer block is an essential component designed for developers who need to display video streams in their applications. This powerful tool enables you to render video content on specific areas of windows or screens across various platforms and UI frameworks.

The block utilizes a platform-specific visual control called `VideoView` which leverages DirectX technology on Windows systems and typically implements OpenGL rendering on other platforms. The SDK fully supports cross-platform development with compatibility for both Avalonia and MAUI UI frameworks.

One of the key advantages of this block is its flexibility - developers can implement multiple video views and renderers to display the same video stream in different locations simultaneously, whether in separate sections of a window or across multiple windows.

## Rendering Technologies

### DirectX Integration

On Windows platforms, the Video Renderer Block seamlessly integrates with DirectX for high-performance hardware-accelerated rendering. This integration provides several benefits:

- **Hardware acceleration**: Utilizes the GPU for efficient video processing and rendering
- **Low-latency playback**: Minimizes delay between frame processing and display
- **Direct3D surface sharing**: Enables efficient memory management and reduced copying of video data
- **Multiple display support**: Handles rendering across various display configurations
- **Support for High DPI**: Ensures crisp rendering on high-resolution displays

The renderer automatically selects the appropriate DirectX version based on your system capabilities, supporting DirectX 11 and DirectX 12 where available.

### OpenGL Implementation

For cross-platform compatibility, the Video Renderer uses OpenGL on Linux and older macOS systems:

- **Consistent rendering API**: Provides a unified approach across different operating systems
- **Shader-based processing**: Enables advanced video effects and color transformations
- **Texture mapping optimization**: Efficiently handles video frame presentation
- **Framebuffer objects support**: Allows for off-screen rendering and complex composition
- **Hardware-accelerated scaling**: Delivers high-quality resizing with minimal performance impact

OpenGL ES variants are utilized on mobile platforms to ensure optimal performance while maintaining compatibility with the core rendering pipeline.

### Metal Framework Support

On newer Apple platforms (macOS, iOS, iPadOS), the Video Renderer can leverage Metal - Apple's modern graphics and compute API:

- **Native Apple integration**: Optimized specifically for Apple hardware
- **Reduced CPU overhead**: Minimizes processing bottlenecks compared to OpenGL
- **Enhanced parallel execution**: Better utilizes multi-core processors
- **Improved memory bandwidth**: More efficient video frame handling
- **Integration with Apple's video toolchain**: Seamless interoperability with AV Foundation and Core Video

The renderer automatically selects Metal when available on Apple platforms, falling back to OpenGL when necessary on older versions.

## Technical Specifications

### Block Information

Name: VideoRendererBlock

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input video | uncompressed video | one or more |
| Input subtitle | text or subpicture subtitles | one (optional) |

## Implementation Guide

### Setting Up Your Video View

The Video View component serves as the visual element where your video content will be displayed. It needs to be properly integrated into your application's UI layout.

### Creating a Basic Pipeline

Below is a visual representation of a simple pipeline implementation:

```mermaid
graph LR;
    UniversalSourceBlock-->VideoRendererBlock;
```

This diagram illustrates how a source block connects directly to the video renderer to create a functional video playback system.

### Code Implementation Example

The following sample demonstrates how to implement a basic video rendering pipeline:

```csharp
// Create a pipeline
var pipeline = new MediaBlocksPipeline();

// create a source block
var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(filename));

// create a video renderer block
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connect the blocks
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);

// start the pipeline
await pipeline.StartAsync();
```

### Displaying Embedded Subtitles

Connect a source's subtitle output to `SubtitleInput` and set `SubtitleEnabled` to draw the subtitle track of the file over the video - text (SRT, SSA/ASS, tx3g) and bitmap (DVD, DVB, PGS) subtitles. It works with every video view. To draw an external file instead, set `SubtitleExternalFile` and leave `SubtitleInput` unconnected.

```csharp
var fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(filename, renderSubtitle: true));

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
// SubtitleOutput is null when the file has no subtitle track
if (fileSource.SubtitleOutput != null)
{
    videoRenderer.SubtitleEnabled = true;
    pipeline.Connect(fileSource.SubtitleOutput, videoRenderer.SubtitleInput);
}
```

### Keeping the Last Frame After Stop

By default the view clears to its background when the media ends or the pipeline stops. Set `KeepLastFrame` to keep the last frame on the view instead, for example to show where a film ended:

```csharp
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1)
{
    KeepLastFrame = true,
};
```

The X engines read the same setting from the view: `VideoView1.RendererSettings.KeepLastFrame = true` before the start.

- The default is `false`, which keeps the behaviour of earlier versions.
- The frame stays until the next start. Letterbox applies to it as to the video. One exception: on Android, `VideoViewTX` (which the MAUI and Uno views use there) clears it when the view is resized or its surface is recreated while stopped, for example on rotation.
- While the pipeline is stopped, `Snapshot_Get` and `Snapshot_GetAsync` return that frame, so an app can save it or use it as a cover.
- `LiveVideoCompositor` with `KeepLastFrame` on its main view stops its main pipeline before its inputs, so the view keeps the last composed picture rather than the compositor's background.
- The classic DirectShow engines (`VideoCaptureCore`, `MediaPlayerCore`, `VideoEditCore`) have their own renderer settings and keep clearing on stop.

| Platform | Views | Checked on (off and on) |
|----------|-------|-------------------------|
| Windows | WinForms, WPF, Avalonia | Windows 11 desktop |
| Windows | WinUI, MAUI (`VideoViewX`, `SkiaVideoView`) | Windows 11 desktop |
| Windows | Uno | Not checked yet |
| Linux | Avalonia | Ubuntu 24.04, X11 |
| Android | `VideoView`, `VideoViewGL`, `VideoViewTX` | Android 16 phone, Android emulator |
| Android | MAUI, Uno, Avalonia | Not checked yet |
| iOS | Apple views, MAUI | Not checked on a device yet |
| Mac Catalyst | Apple views, MAUI | macOS 27 desktop |
| macOS | Apple views, Avalonia | macOS 27 desktop |

## Platform Compatibility

The Video Renderer block offers wide compatibility across multiple operating systems and devices:

- Windows
- macOS
- Linux
- iOS
- Android

This makes it an ideal solution for developers building cross-platform applications that require consistent video rendering capabilities.
