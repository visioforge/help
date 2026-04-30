---
title: Video Renderer in C# .NET — EVR, Direct2D, WPF, MadVR
description: Configure video rendering in C# / .NET across WinForms, WPF, and WinUI 3. EVR, Direct2D, madVR, native HWND, callback modes — setup and hardware acceleration.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - C#
primary_api_classes:
  - VideoRenderer
  - VideoRendererMode
  - VideoView
  - VideoRendererStretchMode
  - VideoCaptureCoreX

---

# Video Renderer Options in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The classic engines (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) expose **10 video renderer modes** through the `VideoRendererMode` enum. Choosing the right one controls how frames reach the screen: raw DirectShow filters, Direct2D GPU surfaces, native HWND embedded in WPF, frame callbacks for custom rendering, WinUI 3 controls, or the third-party madVR renderer. This guide walks through each mode with minimal enable code, platform availability, and a decision guide at the top so you can skip straight to the mode your app needs.

!!! note "Classic engines only"
    This page covers the DirectShow-based classic engines. The cross-platform `VideoCaptureCoreX` / `MediaPlayerCoreX` engines use a `VideoView` control with GStreamer sinks and do not expose a `VideoRendererMode` enum — rendering there is handled automatically by the UI control binding.

## Quick pick — which renderer for which app?

| Mode | UI framework | Best for |
|---|---|---|
| `VideoRenderer` (legacy GDI) | WinForms | Maximum compatibility on very old hardware |
| `VMR9` | WinForms | Windows XP / Vista, software + light HW accel |
| `EVR` | WinForms | Default pick on modern Windows (Vista+) |
| `Direct2D` | WinForms, WPF | GPU-accelerated 2D, 4K+ content, modern apps |
| `Direct2DManaged` | WPF | Managed Direct2D with WPF-aware pause on minimize |
| `WPF_NativeHWND` | WPF | Native HWND embedded in WPF for higher perf than pure WPF |
| `WPF_WinUI_Callback` (`FrameCallback`) | WPF, WinUI, custom | Per-frame callbacks for CV, AI, custom rendering |
| `WinUI` | WinUI 3 | Native WinUI 3 apps (Windows 10/11) |
| `MadVR` | WinForms | Reference-grade scaling + color, needs external madVR install |
| `None` | any | Headless / audio-only / file conversion without preview |

## Understanding Available Video Renderer Options

The detailed sections below describe each mode, starting with the three classic DirectShow renderers.

### Legacy Video Renderer (GDI-based)

The Video Renderer is the oldest option in the DirectShow ecosystem. It relies on GDI (Graphics Device Interface) for drawing operations.

**Key characteristics:**

- Software-based rendering without hardware acceleration
- Compatible with older systems and configurations
- Lower performance ceiling compared to modern alternatives
- Simple implementation with minimal configuration options

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
```

**When to use:**

- Compatibility is the primary concern
- Application targets older hardware or operating systems
- Minimal video processing requirements
- Troubleshooting issues with newer renderers

### Video Mixing Renderer 9 (VMR9)

VMR9 represents a significant improvement over the legacy renderer, introducing support for hardware acceleration and advanced features.

**Key characteristics:**

- Hardware-accelerated rendering through DirectX 9
- Support for multiple video streams mixing
- Advanced deinterlacing options
- Alpha blending and compositing capabilities
- Custom video effects processing

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
```

**When to use:**

- Modern applications requiring good performance
- Video editing or composition features are needed
- Multiple video stream scenarios
- Applications that need to balance performance and compatibility

### Enhanced Video Renderer (EVR)

EVR is the most advanced option, available in Windows Vista and later operating systems. It leverages the Media Foundation framework rather than pure DirectShow.

**Key characteristics:**

- Latest hardware acceleration technologies
- Superior video quality and performance
- Enhanced color space processing
- Better multi-monitor support
- More efficient CPU usage
- Improved synchronization mechanisms

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
```

**When to use:**

- Modern applications targeting Windows Vista or later
- Maximum performance and quality are required
- Applications handling HD or 4K content
- When advanced synchronization is important
- Multiple display environments

### Direct2D Renderer

Direct2D provides high-performance 2D rendering with GPU acceleration. It is available on both WinForms and WPF hosts and is the recommended modern choice when you need hardware-accelerated rendering with simple rotation, flip, and stretch controls.

**Key characteristics:**

- Hardware-accelerated via Direct2D / Direct3D 11
- Works on both WinForms and WPF
- Supports rotation (0 / 90 / 180 / 270), horizontal and vertical flip
- Clean stretch mode integration (`Stretch` / `Letterbox`)
- Low CPU overhead, scales well to 4K / 8K content

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2D;
VideoCapture1.Video_Renderer.RotationAngle = 0;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer.Flip_Horizontal = false;
VideoCapture1.Video_Renderer.Flip_Vertical = false;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**When to use:**

- Modern WinForms or WPF apps that want GPU-accelerated rendering
- 4K / 8K sources where CPU-based paths would bottleneck
- Apps that need run-time rotation or flip controls

### Direct2DManaged Renderer (WPF)

A WPF-specific managed variant of Direct2D. Integrates more cleanly with the WPF object model and automatically pauses rendering when the window is minimized — useful for long-running playback apps where you don't want GPU work on hidden windows.

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.Direct2DManaged;
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

Rotation, flip, and stretch options are shared with the regular `Direct2D` mode. Pause-on-minimize is handled automatically by the WPF `VideoView` control.

**When to use:**

- WPF apps where you want Direct2D performance with WPF-friendly lifecycle
- Multi-window dashboards where inactive windows should not consume GPU cycles

### WPF Native HWND Renderer

Hosts a native Win32 HWND inside the WPF `VideoView` control. Gives you raw DirectShow renderer performance in a WPF layout at the cost of standard WPF-render-chain quirks (airspace issues with overlapping controls).

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**When to use:**

- WPF apps that need maximum DirectShow rendering performance
- Apps embedding legacy filters that expect a HWND target
- You don't need to overlay WPF controls on top of the video surface

### FrameCallback Renderer (WPF_WinUI_Callback)

A callback-based rendering mode. Instead of drawing frames directly, the engine delivers each frame to your code via events, letting you render with any library (SkiaSharp, `System.Drawing`, custom OpenGL/DirectX, WriteableBitmap) or feed a non-visual pipeline (computer vision, AI inference, streaming to a remote endpoint).

`FrameCallback` is an alias for `WPF_WinUI_Callback` — the same mode with a more self-descriptive name.

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
await VideoCapture1.Video_Renderer_UpdateAsync();

// Subscribe to frame events
VideoCapture1.OnVideoFrameBitmap += (sender, e) =>
{
    // e.Frame is a System.Drawing.Bitmap — render to a PictureBox, WriteableBitmap, etc.
};

VideoCapture1.OnVideoFrameBuffer += (sender, e) =>
{
    // e.Frame.Data is IntPtr — wrap with SkiaSharp / Marshal.Copy for pixel-level work
};
```

See [Image drawing via OnVideoFrameBuffer](image-onvideoframebuffer.md) and [Text drawing via OnVideoFrameBuffer](text-onvideoframebuffer.md) for detailed per-frame processing examples.

**When to use:**

- Computer-vision / ML pipelines that consume raw frames
- Custom rendering with SkiaSharp, DirectX, or OpenGL
- WPF / WinUI / MAUI apps that render into a `WriteableBitmap` manually
- Apps with no preview surface at all (frames shipped to a server, encoder, etc.)

### WinUI 3 Renderer

Native rendering for WinUI 3 apps on Windows 10/11. Use this mode when your shell is `Microsoft.UI.Xaml` and you host a `VisioForge.Core.UI.WinUI.VideoView` control.

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.WinUI;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**When to use:**

- WinUI 3 apps (Windows App SDK, not the old WinUI 2 / UWP)
- You want native look-and-feel consistency with other WinUI content

### madVR Renderer (third-party)

[madVR](https://www.madvr.com/) is a reference-quality external video renderer popular with home-theatre PCs and high-end video software. It delivers superior scaling algorithms, color management, and deinterlacing at the cost of higher GPU load. Supported only on WinForms hosts; requires a separate madVR installation on the target machine (the CLSID-registered DirectShow filter must be present).

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.MadVR;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**Runtime requirement:** ensure madVR is installed on the target system. If the filter is missing, `Video_Renderer_UpdateAsync` will fail — use the fallback pattern shown in [Renderer Compatibility Problems](#renderer-compatibility-problems) below to degrade gracefully to EVR.

**When to use:**

- Reference-grade video quality for mastering, HTPC, or media server UIs
- Audiences with GPUs that can absorb the extra rendering cost
- You can ship / document a separate madVR install step

### None (headless)

Disables rendering entirely. The capture / edit / playback graph still runs — frames flow to encoders, file outputs, streaming endpoints, or callbacks — but no preview surface is allocated.

**Implementation example:**

```cs
VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;
await VideoCapture1.Video_Renderer_UpdateAsync();
```

**When to use:**

- Audio-only capture (microphone-to-file) when the SDK has both audio and video branches
- File conversion / transcoding without a preview window
- Server-side rendering pipelines
- Unit tests and headless CI runs

## Advanced Configuration Options

Beyond just selecting a renderer, the SDK provides various configuration options to fine-tune video presentation.

### Working with Deinterlacing Modes

When displaying interlaced video content (common in broadcast sources), proper deinterlacing improves visual quality significantly. The SDK supports various deinterlacing algorithms depending on the renderer chosen.

First, retrieve the available deinterlacing modes. `Video_Renderer_Deinterlace_Modes()` returns the VMR-9 mode names auto-discovered from the current driver:

```cs
// Populate a dropdown with the available VMR-9 modes
foreach (string deinterlaceMode in VideoCapture1.Video_Renderer_Deinterlace_Modes())
{
  cbDeinterlaceModes.Items.Add(deinterlaceMode);
}
```

Deinterlacing is configured on the two renderers separately. VMR-9 takes a mode-name string; EVR takes a `VideoRendererEVRDeinterlaceMode` enum value:

```cs
// VMR-9 — set the mode string selected by the user
VideoCapture1.Video_Renderer.Deinterlace_VMR9_Mode = cbDeinterlaceModes.SelectedItem.ToString();
VideoCapture1.Video_Renderer.Deinterlace_VMR9_UseDefault = false;

// EVR — use the enum instead
// VideoCapture1.Video_Renderer.Deinterlace_EVR_Mode = VideoRendererEVRDeinterlaceMode.Auto;

VideoCapture1.Video_Renderer_Update();
```

VMR9 and EVR support various deinterlacing algorithms including:

- Bob (simple line doubling)
- Weave (field interleaving)
- Motion adaptive
- Motion compensated (highest quality)

The availability of specific algorithms depends on the video card capabilities and driver implementation.

### Managing Aspect Ratio and Stretch Modes

When displaying video in a window or control that doesn't match the source's native aspect ratio, you need to decide how to handle this discrepancy. The SDK provides multiple stretch modes to address different scenarios.

#### Stretch Mode

This mode stretches the video to fill the entire display area, potentially distorting the image:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Stretch;
VideoCapture1.Video_Renderer_Update();
```

**Use cases:**

- When aspect ratio is not critical
- Filling the entire display area is more important than proportions
- Source and display have similar aspect ratios
- User interface constraints require full-area usage

#### Letterbox Mode

This mode preserves the original aspect ratio by adding black borders as needed:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.Letterbox;
VideoCapture1.Video_Renderer_Update();
```

**Use cases:**

- Maintaining correct proportions is essential
- Professional video applications
- Content where distortion would be noticeable or problematic
- Cinema or broadcast content viewing

#### LetterboxToFill Mode

This mode fills the display area while preserving aspect ratio, cropping any overflow on one axis:

```cs
VideoCapture1.Video_Renderer.StretchMode = VideoRendererStretchMode.LetterboxToFill;
VideoCapture1.Video_Renderer_Update();
```

**Use cases:**

- Consumer video applications where filling the screen is preferred
- Content where edges are less important than center
- Social media-style video display
- When trying to eliminate letterboxing in already letterboxed content

### Aspect-Ratio Override

To force a specific display aspect ratio (e.g. show 4:3 content letterboxed inside a 16:9 container), enable the override and set the ratio components:

```cs
VideoCapture1.Video_Renderer.Aspect_Ratio_Override = true;
VideoCapture1.Video_Renderer.Aspect_Ratio_X = 4;
VideoCapture1.Video_Renderer.Aspect_Ratio_Y = 3;
VideoCapture1.Video_Renderer_Update();
```

### Zoom and Pan

`VideoRendererSettings` exposes zoom/shift properties useful for digital PTZ on a preview:

```cs
VideoCapture1.Video_Renderer.Zoom_Ratio  = 150; // 150%
VideoCapture1.Video_Renderer.Zoom_ShiftX = 0;
VideoCapture1.Video_Renderer.Zoom_ShiftY = 0;
VideoCapture1.Video_Renderer_Update();
```

### Flip and Rotation

```cs
VideoCapture1.Video_Renderer.Flip_Horizontal = true;
VideoCapture1.Video_Renderer.Flip_Vertical   = false;
// RotationAngle is only respected by the Direct2D renderer and accepts 0, 90, 180, or 270.
VideoCapture1.Video_Renderer.RotationAngle   = 90;
VideoCapture1.Video_Renderer_Update();
```

## Troubleshooting Common Issues

### Renderer Compatibility Problems

If you encounter issues with a specific renderer, try falling back to a more compatible option:

```cs
try
{
    // Try using EVR first
    VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.EVR;
    VideoCapture1.Video_Renderer_Update();
}
catch
{
    try 
    {
        // Fall back to VMR9
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VMR9;
        VideoCapture1.Video_Renderer_Update();
    }
    catch
    {
        // Last resort - legacy renderer
        VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.VideoRenderer;
        VideoCapture1.Video_Renderer_Update();
    }
}
```

## Best Practices and Recommendations

1. **Choose the right renderer for your target environment**:
   - For modern Windows: EVR
   - For broad compatibility: VMR9
   - For legacy systems: Video Renderer

2. **Test on various hardware configurations**: Video rendering can behave differently across GPU vendors and driver versions.

3. **Implement renderer fallback logic**: Always have a backup plan if the preferred renderer fails.

4. **Consider your video content**: Higher resolution or interlaced content will benefit more from advanced renderers.

5. **Balance quality vs. performance**: The highest quality settings might not always deliver the best user experience if they impact performance.

## Required Dependencies

To ensure proper functionality of these renderers, make sure to include:

- SDK redistributable packages
- DirectX End-User Runtime (latest version recommended)
- .NET Framework runtime appropriate for your application

## Conclusion

The classic engines offer 10 renderer modes covering WinForms, WPF, and WinUI 3. **EVR** is the safe default for WinForms, **Direct2D** for modern GPU-accelerated rendering on either WinForms or WPF, **FrameCallback** for custom pipelines (CV / AI / bespoke rendering), **WinUI** for WinUI 3 shells, and **madVR** for reference-quality scenarios that can accommodate the external install. `None` is the right mode when there is no preview at all.

For applications built on the cross-platform `VideoCaptureCoreX` / `MediaPlayerCoreX` engines, renderer choice is handled by the `VideoView` control and does not use this enum.

## Related documentation

- [Image drawing via OnVideoFrameBuffer](image-onvideoframebuffer.md) — pixel-level frame processing, the canonical use case for `FrameCallback`.
- [Text drawing via OnVideoFrameBuffer](text-onvideoframebuffer.md) — text overlays with `FrameCallback`.
- [Rendering video in a PictureBox](draw-video-picturebox.md) — WinForms rendering pattern that pairs well with `FrameCallback`.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.