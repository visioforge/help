---
title: Draw Text on Video Frames in C# .NET — OnVideoFrameBuffer
description: Draw dynamic text on video frames in C# / .NET using the OnVideoFrameBuffer event. Timestamps, sensor data, custom fonts — pixel-level rendering.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - C#
primary_api_classes:
  - VideoEffectTextLogo
  - VideoFrameBufferEventArgs
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX

---

# Creating Custom Text Overlays with OnVideoFrameBuffer in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The `OnVideoFrameBuffer` event gives direct, pixel-level access to every video frame as it passes through the pipeline. Drawing text onto the frame — timestamps, sensor readings, debug telemetry, or custom branding — is one of the most common uses. This guide shows how to render text on raw frames with full control over font, color, position, and per-frame logic.

!!! tip "Looking for the high-level text overlay feature?"
    If you just need a static, animated, or clock-driven text overlay with standard positioning, use the dedicated [text overlay effect](../video-effects/text-overlay.md) — one line of code via `Video_Effects_Add(new VideoEffectTextLogo(...))`. Use `OnVideoFrameBuffer` (this page) when you need **pixel-level control**: custom fonts, advanced layout, per-frame dynamic content, or integration with third-party text/graphics libraries.

### Supported engines

The `OnVideoFrameBuffer` event is exposed on both engine families:

| Engine | Event args type | Frame pixel format |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, cross-platform) | `VideoFrameXBufferEventArgs` | BGRA (most common) |
| `MediaPlayerCoreX` (GStreamer, cross-platform) | `VideoFrameXBufferEventArgs` | BGRA (most common) |

Both engines follow the same pattern — subscribe to the event, read `e.Frame.Data` (an `IntPtr`) with `Width` / `Height` / `Stride`, render into the buffer in place, and set `e.UpdateData = true` to propagate changes downstream.

## Understanding the OnVideoFrameBuffer Event

The OnVideoFrameBuffer event is a powerful hook that gives developers direct access to the video frame buffer during processing. This event fires for each frame of video, providing an opportunity to modify the frame data before it's displayed or encoded.

Key benefits of using OnVideoFrameBuffer for text overlays include:

- **Frame-level access**: Modify individual frames with pixel-perfect precision
- **Dynamic content**: Update text based on real-time data or timestamps
- **Custom styling**: Apply custom fonts, colors, and effects beyond what built-in APIs offer
- **Performance optimizations**: Implement efficient rendering techniques for high-performance applications

## Implementation Overview

The technique presented here uses the following components:

1. An event handler for OnVideoFrameBuffer that processes each video frame
2. A VideoEffectTextLogo object to define text properties
3. The FastImageProcessing API to render text onto the frame buffer

This approach is particularly useful when you need to:

- Display dynamic data like timestamps, metadata, or sensor readings
- Create animated text effects
- Position text with pixel-perfect accuracy
- Apply custom styling not available through standard APIs

## Sample Code Implementation

The following C# example demonstrates how to implement a basic text overlay system using the OnVideoFrameBuffer event:

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;

        InitTextLogo();
    }

    // AddTextLogo(context, pixels, pixels32bit, pixels32tmp, frameWidth, frameHeight,
    //             ref textLogo, timeStamp, frameNumber)
    // Pass pixels32bit: false + pixels32tmp: IntPtr.Zero for RGB24 frames.
    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth: e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}

private bool logoInitiated = false;

private VideoEffectTextLogo textLogo = null;

private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "Hello world!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

## Detailed Code Explanation

Let's break down the key components of this implementation:

### The Event Handler

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
```

This method is triggered for each video frame. The VideoFrameBufferEventArgs provides access to:

- Frame data (pixel buffer)
- Frame dimensions (width and height)
- Timestamp information

### Initialization Logic

```cs
if (!logoInitiated)
{
    logoInitiated = true;
    InitTextLogo();
}
```

This code ensures the text logo is only initialized once, preventing unnecessary object creation for each frame. This pattern is important for performance when processing video at high frame rates.

### Text Logo Setup

```cs
private void InitTextLogo()
{
    textLogo = new VideoEffectTextLogo(true);
    textLogo.Text = "Hello world!";
    textLogo.Left = 50;
    textLogo.Top = 50;
}
```

The VideoEffectTextLogo class is used to define the properties of the text overlay:

- The text content ("Hello world!")
- Position coordinates (50 pixels from both left and top)

### Rendering the Text Overlay

```cs
FastImageProcessing.AddTextLogo(
    context: null,
    pixels: e.Frame.Data,
    pixels32bit: false,        // true when the engine delivers RGB32
    pixels32tmp: IntPtr.Zero,  // optional scratch buffer; IntPtr.Zero lets the helper allocate on demand
    frameWidth:  e.Frame.Info.Width,
    frameHeight: e.Frame.Info.Height,
    textLogo: ref textLogo,
    timeStamp: e.Frame.Timestamp,
    frameNumber: 0);
```

The 9-arg signature mirrors `FastImageProcessing.AddTextLogo` exactly. Width/height live inside `e.Frame.Info` on the classic `VideoFrame` struct; the timestamp lives on `e.Frame.Timestamp`. Pass `pixels32bit: true` when your source is RGB32.

## Advanced Customization Options

While the basic example demonstrates a simple static text overlay, the VideoEffectTextLogo class supports numerous customization options:

### Text Formatting

```cs
// Font is a full System.Drawing.Font — any typeface + style combo works.
textLogo.Font = new System.Drawing.Font("Arial", 24, System.Drawing.FontStyle.Bold);
textLogo.FontColor = System.Drawing.Color.White;
textLogo.TransparencyLevel = 200;   // 0 (fully transparent) - 255 (opaque)
```

### Background and Borders

```cs
textLogo.BackgroundTransparent = false;
textLogo.BackgroundColor = System.Drawing.Color.Black;

// Border ring is configured via BorderMode + per-ring colors and sizes (inner/outer).
// TextEffectMode values: None, Inner, Outer, InnerAndOuter, Embossed, Outline, FilledOutline, Halo.
textLogo.BorderMode = TextEffectMode.InnerAndOuter;
textLogo.BorderInnerColor = System.Drawing.Color.Yellow;
textLogo.BorderInnerSize = 2;
textLogo.BorderOuterColor = System.Drawing.Color.Black;
textLogo.BorderOuterSize = 1;
```

### Animation and Dynamic Content

For dynamic content that changes per frame:

```cs
private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logoInitiated)
    {
        logoInitiated = true;
        InitTextLogo();
    }

    // Timestamp lives on e.Frame.Timestamp (TimeSpan)
    textLogo.Text = $"Timestamp: {e.Frame.Timestamp:hh\\:mm\\:ss\\.fff}";

    // Animate position
    textLogo.Left = 50 + (int)(Math.Sin(e.Frame.Timestamp.TotalSeconds) * 50);

    FastImageProcessing.AddTextLogo(
        context: null,
        pixels: e.Frame.Data,
        pixels32bit: false,
        pixels32tmp: IntPtr.Zero,
        frameWidth:  e.Frame.Info.Width,
        frameHeight: e.Frame.Info.Height,
        textLogo: ref textLogo,
        timeStamp: e.Frame.Timestamp,
        frameNumber: 0);
}
```

## Performance Considerations

When implementing custom text overlays, consider these performance best practices:

1. **Initialize objects once**: Create the VideoEffectTextLogo object only once, not per frame
2. **Minimize text changes**: Update text content only when necessary
3. **Use efficient fonts**: Simple fonts render faster than complex ones
4. **Consider resolution**: Higher resolution videos require more processing power
5. **Test on target hardware**: Ensure your implementation performs well on production systems

## Multiple Text Elements

To display multiple text elements on the same frame:

```cs
private VideoEffectTextLogo titleLogo = null;
private VideoEffectTextLogo timestampLogo = null;

private void InitTextLogos()
{
    titleLogo = new VideoEffectTextLogo(true);
    titleLogo.Text = "Camera Feed";
    titleLogo.Left = 50;
    titleLogo.Top = 50;
    
    timestampLogo = new VideoEffectTextLogo(true);
    timestampLogo.Left = 50;
    timestampLogo.Top = 100;
}

private void SDK_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    if (!logosInitiated)
    {
        logosInitiated = true;
        InitTextLogos();
    }
    
    // Update dynamic content
    timestampLogo.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

    // Render both text elements
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref titleLogo, e.Frame.Timestamp, 0);
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, false, IntPtr.Zero,
        e.Frame.Info.Width, e.Frame.Info.Height, ref timestampLogo, e.Frame.Timestamp, 0);
}
```

## VideoCaptureCoreX / MediaPlayerCoreX (X engines) example

On the cross-platform X engines the event signature is `VideoFrameXBufferEventArgs` and the frame buffer typically arrives in **BGRA** format (4 bytes per pixel). The example below uses SkiaSharp to wrap the raw buffer and draw text on top; SkiaSharp is a transitive dependency of the X engines, so no extra NuGet package is needed.

```cs
using SkiaSharp;

// Create paints once, reuse across frames
private SKPaint _textPaint = new SKPaint
{
    Color = SKColors.White,
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

private SKPaint _shadowPaint = new SKPaint
{
    Color = SKColors.Black.WithAlpha(160),
    TextSize = 32,
    IsAntialias = true,
    Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
};

// Subscribe after constructing VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // Wrap the raw BGRA buffer in a SkiaSharp surface (no extra allocation)
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Dynamic content built per frame
        var timestamp = e.Frame.Timestamp.ToString(@"hh\:mm\:ss\.fff");
        var line = $"REC  {timestamp}";

        // Draw shadow first, then main text for legibility on any background
        canvas.DrawText(line, 18, 42, _shadowPaint);
        canvas.DrawText(line, 16, 40, _textPaint);
        canvas.Flush();
    }

    // Propagate the modified frame downstream
    e.UpdateData = true;
}
```

**Why BGRA matters.** The X engines request BGRA by default for frame callbacks because it maps 1:1 to SkiaSharp, System.Drawing, and most GPU-friendly interop paths. If you need a different format, request a format conversion block upstream rather than converting on every frame.

**Measuring and positioning text.** Use `_textPaint.MeasureText(line)` to compute width for right- or center-alignment. SkiaSharp also exposes `SKFontMetrics` via `_textPaint.FontMetrics` for baseline / ascent / descent to position text precisely against frame edges.

**Alternative imaging stacks.** You can also use `System.Drawing.Graphics` wrapping a `Bitmap` constructed over the raw buffer on Windows, or direct byte writes with `Marshal.Copy` / `Span<byte>` for full control. SkiaSharp is the recommended option on macOS / Linux / iOS / Android.

**Engine-level parity.** Everything in [Performance Considerations](#performance-considerations) and [Multiple Text Elements](#multiple-text-elements) applies equally to the X engines — the event fires on a processing thread, `UpdateData` propagates changes, and heavy work should be offloaded to avoid dropping frames.

## Required Components

To implement this solution, you'll need:

- SDK redist package installed in your application (NuGet on the X engines, installer on `VideoCaptureCore`).
- Reference to the appropriate SDK (Video Capture SDK, Video Edit SDK, or Media Player SDK — X or classic).
- For the X-engine sample: a transitive SkiaSharp reference (already pulled in by the SDK) or your own text-rendering library.

## Conclusion

The `OnVideoFrameBuffer` event gives direct access to every raw frame on both the classic `VideoCaptureCore` engine (RGB24/RGB32 via `VideoFrameBufferEventArgs` + `FastImageProcessing`) and the cross-platform X engines (`VideoCaptureCoreX` / `MediaPlayerCoreX`, BGRA via `VideoFrameXBufferEventArgs` + SkiaSharp). This is the right tool when you need pixel-level text rendering — custom fonts, per-frame dynamic content, anti-aliasing you control, or integration with third-party text/graphics libraries.

For static or clock-driven text overlays without writing a per-frame handler, the one-line [text overlay effect](../video-effects/text-overlay.md) is usually the better choice.

## Related documentation

- [Text overlay effect](../video-effects/text-overlay.md) — high-level, declarative text overlay without writing a callback.
- [Image drawing via OnVideoFrameBuffer](image-onvideoframebuffer.md) — same technique applied to images instead of text.
- [Drawing video in a PictureBox](draw-video-picturebox.md) — WinForms rendering pattern that often pairs with pixel-level frame work.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.