---
title: Process Video Frames in C# .NET — OnVideoFrameBuffer Event
description: Access the raw video frame buffer in C# / .NET via the OnVideoFrameBuffer event. Modify pixels, draw images, apply custom blends — full frame control.
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
  - Encoding
  - C#
primary_api_classes:
  - VideoFrameXBufferEventArgs
  - VideoCaptureCoreX
  - MediaPlayerCoreX
  - VideoCaptureCore
  - VideoFrameBufferEventArgs

---

# Drawing Images with OnVideoFrameBuffer in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The `OnVideoFrameBuffer` event gives direct, pixel-level access to every video frame as it passes through the pipeline. Event handlers receive a raw buffer and can inspect, modify, or overwrite pixels before the frame continues to the next stage (preview, encoder, file output). Drawing an image onto the frame — for a watermark, logo, debug overlay, or computer-vision annotation — is the most common use case and the one this guide walks through.

!!! tip "Looking for the high-level overlay feature?"
    If you just need to drop a static or animated image on the video (PNG / JPG / GIF / BMP), use the dedicated [image overlay effect](../video-effects/image-overlay.md) — one line of code via `Video_Effects_Add(new VideoEffectImageLogo(...))`. Use `OnVideoFrameBuffer` (this page) when you need **pixel-level control**: custom blend modes, per-frame logic, CV annotations, or integration with third-party imaging libraries.

### Supported engines

The `OnVideoFrameBuffer` event is exposed on both engine families:

| Engine | Event args type | Frame pixel format |
|---|---|---|
| `VideoCaptureCore` (DirectShow, Windows) | `VideoFrameBufferEventArgs` | RGB24 / RGB32 |
| `VideoCaptureCoreX` (GStreamer, cross-platform) | `VideoFrameXBufferEventArgs` | BGRA (most common) |
| `MediaPlayerCoreX` (GStreamer, cross-platform) | `VideoFrameXBufferEventArgs` | BGRA (most common) |

Both engines follow the same pattern — subscribe to the event, read `e.Frame.Data` (an `IntPtr`) along with `Width` / `Height` / `Stride`, optionally modify the buffer in place, and set `e.UpdateData = true` to have the changes propagated downstream.

## Understanding the process

When working with video frames you need to:

1. Load your image (logo, watermark, etc.) into memory.
2. Convert the image to a compatible buffer format (RGB24/RGB32 for the legacy engine, BGRA for the X engines).
3. Subscribe to the `OnVideoFrameBuffer` event.
4. Draw the image onto each video frame as it is processed.
5. Set `e.UpdateData = true` so the modified frame replaces the original downstream.

## VideoCaptureCore (DirectShow) example

Let's walk through the implementation step by step:

### Step 1: Load Your Image

First, load the image file you want to overlay on the video:

```cs
// Bitmap loading from file
private Bitmap logoImage = new Bitmap(@"logo24.jpg");
// You can also use PNG with alpha channel for transparency
//private Bitmap logoImage = new Bitmap(@"logo32.png");
```

### Step 2: Prepare Memory Buffers

Initialize pointers for the image buffer:

```cs
// Logo RGB24/RGB32 buffer
private IntPtr logoImageBuffer = IntPtr.Zero;
private int logoImageBufferSize = 0;
```

### Step 3: Implement the OnVideoFrameBuffer Event Handler

The full event handler implementation:

```cs
private void VideoCapture1_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
{
    // Create logo buffer if not allocated or have zero size
    if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
    {
        if (logoImageBuffer == IntPtr.Zero)
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }
        else
        {
            if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
            else
            {
                logoImageBufferSize = ImageHelper.GetStrideRGB24(logoImage.Width) * logoImage.Height;

                Marshal.FreeCoTaskMem(logoImageBuffer);
                logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
            }
        }

        if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format32bppArgb);
        }
        else
        {
            BitmapHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format24bppRgb);
        }
    }

    // Draw image — the classic VideoFrame struct keeps Width/Height/Stride inside Frame.Info
    if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
    {
        FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }
    else
    {
        FastImageProcessing.Draw_RGB24OnRGB24Old(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Info.Width,
            e.Frame.Info.Height, 0, 0);
    }

    e.UpdateData = true;
}
```

## Detailed Explanation

### Memory Management

The code handles both 24-bit and 32-bit image formats. Here's what happens:

1. **Buffer Initialization Check**: The code first checks if the logo buffer needs to be created or recreated.

2. **Format Detection**: It determines whether to use RGB24 or RGB32 format based on the loaded image:
   - RGB24: Standard 24-bit color (8 bits each for R, G, B)
   - RGB32: 32-bit color with alpha channel for transparency (8 bits each for R, G, B, A)

3. **Memory Allocation**: Allocates unmanaged memory using `Marshal.AllocCoTaskMem()` to store the image data.

4. **Image Conversion**: Converts the Bitmap to raw pixel data in the allocated buffer using `BitmapHelper.BitmapToIntPtr()`.

### Drawing Process

Once the buffer is prepared, drawing takes place:

1. **Format-Specific Drawing**: The code selects the appropriate drawing method based on the image format:
   - `FastImageProcessing.Draw_RGB32OnRGB24()` for 32-bit images with transparency
   - `FastImageProcessing.Draw_RGB24OnRGB24Old()` for standard 24-bit images (8-arg form) or `Draw_RGB24OnRGB24S()` when source/destination strides are known

2. **Position Parameters**: The `0, 0` parameters specify where to draw the image (top-left corner in this example).

3. **Frame Update**: Setting `e.UpdateData = true` ensures the modified frame data is used for display or further processing.

## VideoCaptureCoreX / MediaPlayerCoreX (X engines) example

On the cross-platform X engines the event signature changes to `VideoFrameXBufferEventArgs` and the frame buffer typically arrives in **BGRA** format (4 bytes per pixel). The same pattern applies — subscribe, inspect, modify, flag updates. The example below uses SkiaSharp to wrap the raw buffer and draw a PNG logo on top; SkiaSharp is already a transitive dependency of the X engines, so no extra NuGet package is needed.

```cs
using SkiaSharp;

// Load logo once (PNG with alpha works well for watermarks)
private SKBitmap _logo = SKBitmap.Decode(@"logo.png");

// Subscribe after constructing VideoCaptureCoreX / MediaPlayerCoreX
_videoCapture.OnVideoFrameBuffer += VideoCapture_OnVideoFrameBuffer;

private void VideoCapture_OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
{
    if (e.Frame == null || e.Frame.Data == IntPtr.Zero)
    {
        return;
    }

    // Wrap the raw BGRA buffer in a SkiaSharp canvas (no extra allocation)
    var info = new SKImageInfo(e.Frame.Width, e.Frame.Height, SKColorType.Bgra8888, SKAlphaType.Premul);

    using (var pixmap = new SKPixmap(info, e.Frame.Data, e.Frame.Stride))
    using (var surface = SKSurface.Create(pixmap))
    {
        var canvas = surface.Canvas;

        // Draw the logo at bottom-right with 16px padding
        var x = e.Frame.Width - _logo.Width - 16;
        var y = e.Frame.Height - _logo.Height - 16;
        canvas.DrawBitmap(_logo, x, y);
        canvas.Flush();
    }

    // Propagate the modified frame downstream
    e.UpdateData = true;
}
```

**Why BGRA matters.** The X engines request BGRA by default for frame callbacks because it maps 1:1 to SkiaSharp, System.Drawing, and most GPU-friendly interop paths. If you need a different format (I420, NV12, RGB24), request a format conversion block upstream of your handler rather than converting on every frame.

**Alternative imaging stacks.** You can also use `System.Drawing.Bitmap` via `new Bitmap(width, height, stride, PixelFormat.Format32bppArgb, data)` on Windows, or manual byte writes via `Marshal.Copy` / `Span<byte>` for maximum control. SkiaSharp is the recommended option on macOS / Linux / iOS / Android.

**Engine-level parity.** Everything documented in the [Memory Management](#memory-management), [Error Handling](#error-handling), and [Performance Optimization](#performance-optimization) sections below applies equally to the X engines — the event fires on a processing thread, `UpdateData` toggles whether the buffer is re-used downstream, and heavy work should be offloaded to avoid dropping frames.

## Best Practices for Image Overlay

For optimal performance when overlaying images on video frames:

1. **Memory Management**: Always free allocated memory when it's no longer needed to prevent memory leaks.

2. **Buffer Reuse**: Create the buffer once and reuse it for subsequent frames rather than recreating it for each frame.

3. **Image Size Considerations**: Use appropriately sized images; overlaying large images can impact performance.

4. **Format Selection**:
   - Use PNG (RGB32) when you need transparency
   - Use JPG (RGB24) when transparency isn't required (more efficient)

5. **Position Calculation**: For dynamic positioning, calculate coordinates based on frame dimensions. On the classic engine (`VideoFrameBufferEventArgs`) Width/Height live on `e.Frame.Info`; on the X engines (`VideoFrameXBufferEventArgs`) they're flat on `e.Frame`.

   ```cs
   // Classic engine — Width/Height live on e.Frame.Info
   int xPos = e.Frame.Info.Width - logoImage.Width - 10;
   int yPos = e.Frame.Info.Height - logoImage.Height - 10;
   ```

## Error Handling

When implementing this functionality, consider adding error handling:

```cs
try 
{
    // Your existing implementation
}
catch (OutOfMemoryException ex)
{
    // Handle memory allocation failures
    Console.WriteLine("Failed to allocate memory: " + ex.Message);
}
catch (Exception ex)
{
    // Handle other exceptions
    Console.WriteLine("Error during frame processing: " + ex.Message);
}
finally 
{
    // Optional cleanup code
}
```

## Performance Optimization

For high-performance applications, consider these optimizations:

1. **Buffer Pre-allocation**: Initialize buffers during application startup rather than during video processing.

2. **Conditional Processing**: Only process frames that need the overlay (e.g., skip processing for certain frames).

3. **Parallel Processing**: For complex operations, consider using parallel processing techniques.

## Conclusion

The `OnVideoFrameBuffer` event gives direct access to every raw frame on both the legacy `VideoCaptureCore` engine (RGB24/RGB32 via `VideoFrameBufferEventArgs`) and the cross-platform X engines (`VideoCaptureCoreX` / `MediaPlayerCoreX`, BGRA via `VideoFrameXBufferEventArgs`). This is the right tool when you need pixel-level control — custom blend modes, per-frame CV annotations, or integration with third-party imaging libraries.

For static or animated image overlays without writing a per-frame handler, the one-line [image overlay effect](../video-effects/image-overlay.md) is usually the better choice.

## Related documentation

- [Image overlay effect](../video-effects/image-overlay.md) — high-level, declarative watermark / logo overlay without writing a callback.
- [Text overlay via OnVideoFrameBuffer](text-onvideoframebuffer.md) — same technique applied to text instead of images.
- [Drawing video in a PictureBox](draw-video-picturebox.md) — WinForms rendering pattern that often pairs with pixel-level frame work.

---

Looking for more code samples? Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for additional examples and resources.