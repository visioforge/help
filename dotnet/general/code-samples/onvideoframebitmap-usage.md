---
title: OnVideoFrameBitmap Real-Time Frame Guide
description: Access and modify video frames in real-time with OnVideoFrameBitmap events for advanced video manipulation in C# applications.
---

# OnVideoFrameBitmap Real-Time Frame Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

The `OnVideoFrameBitmap` event is a powerful feature in .NET video processing libraries that allows developers to access and modify video frames in real-time. This guide explores the practical applications, implementation techniques, and performance considerations when working with bitmap frame manipulation in C# applications.

## Understanding OnVideoFrameBitmap Events

The `OnVideoFrameBitmap` event provides a direct interface to access video frames as they're processed by the SDK. This capability is essential for applications that require:

- Real-time video analysis
- Frame-by-frame manipulation
- Dynamic overlay implementation
- Custom video effects
- Computer vision integration

When the event fires, it delivers a bitmap representation of the current video frame, allowing for pixel-level access and manipulation before the frame continues through the processing pipeline.

## Basic Implementation

To begin working with the `OnVideoFrameBitmap` event, you'll need to subscribe to it in your code:

```csharp
// Subscribe to the OnVideoFrameBitmap event
videoProcessor.OnVideoFrameBitmap += VideoProcessor_OnVideoFrameBitmap;

// Implement the event handler
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Frame manipulation code will go here
    // e.Frame contains the current frame as a Bitmap
}
```

## Manipulating Video Frames

### Simple Bitmap Overlay Example

The following example demonstrates how to overlay an image on each video frame:

```csharp
Bitmap bmp = new Bitmap(@"c:\samples\pics\1.jpg");

using (Graphics g = Graphics.FromImage(e.Frame))
{
    g.DrawImage(bmp, 0, 0, bmp.Width, bmp.Height);
    e.UpdateData = true;
}

bmp.Dispose();
```

In this code:

1. We create a `Bitmap` object from an image file
2. We use the `Graphics` class to draw onto the frame bitmap
3. We set `e.UpdateData = true` to inform the SDK that we've modified the frame
4. We dispose of our resources properly to prevent memory leaks

> **Important:** Always set `e.UpdateData = true` when you modify the frame bitmap. This signals the SDK to use your modified frame instead of the original.

### Adding Text Overlays

Text overlays are commonly used for timestamps, captions, or informational displays:

```csharp
using (Graphics g = Graphics.FromImage(e.Frame))
{
    // Create a semi-transparent background for text
    using (SolidBrush brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
    {
        g.FillRectangle(brush, 10, 10, 200, 30);
    }
    
    // Add text overlay
    using (Font font = new Font("Arial", 12))
    using (SolidBrush textBrush = new SolidBrush(Color.White))
    {
        g.DrawString(DateTime.Now.ToString(), font, textBrush, new PointF(15, 15));
    }
    
    e.UpdateData = true;
}
```

## Performance Considerations

When working with `OnVideoFrameBitmap`, it's crucial to optimize your code for performance. Each frame processing operation must complete quickly to maintain smooth video playback.

### Resource Management

Proper resource management is essential:

```csharp
// Poor performance approach
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap overlay = new Bitmap(@"c:\logo.png");
    Graphics g = Graphics.FromImage(e.Frame);
    g.DrawImage(overlay, 0, 0);
    e.UpdateData = true;
    // Memory leak! Graphics and Bitmap not disposed
}

// Optimized approach
private Bitmap _cachedOverlay;

private void InitializeResources()
{
    _cachedOverlay = new Bitmap(@"c:\logo.png");
}

private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        g.DrawImage(_cachedOverlay, 0, 0);
        e.UpdateData = true;
    }
}

private void CleanupResources()
{
    _cachedOverlay?.Dispose();
}
```

### Optimizing Processing Time

To maintain smooth video playback:

1. **Pre-compute where possible**: Prepare resources before processing begins
2. **Cache frequently used objects**: Avoid creating new objects for each frame
3. **Process only when necessary**: Add conditional logic to skip frames or perform less intensive operations when needed
4. **Use efficient drawing operations**: Choose appropriate GDI+ methods based on your needs

```csharp
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Only process every second frame
    if (_frameCounter % 2 == 0)
    {
        using (Graphics g = Graphics.FromImage(e.Frame))
        {
            // Your frame processing code
            e.UpdateData = true;
        }
    }
    _frameCounter++;
}
```

## Advanced Frame Manipulation Techniques

### Applying Filters and Effects

You can implement custom image processing filters:

```csharp
private void ApplyGrayscaleFilter(Bitmap bitmap)
{
    Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
    BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, bitmap.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
    byte[] rgbValues = new byte[bytes];
    
    Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Process pixel data
    for (int i = 0; i < rgbValues.Length; i += 4)
    {
        byte gray = (byte)(0.299 * rgbValues[i + 2] + 0.587 * rgbValues[i + 1] + 0.114 * rgbValues[i]);
        rgbValues[i] = gray;     // Blue
        rgbValues[i + 1] = gray; // Green
        rgbValues[i + 2] = gray; // Red
    }
    
    Marshal.Copy(rgbValues, 0, ptr, bytes);
    bitmap.UnlockBits(bmpData);
}
```

## Integration with Computer Vision Libraries

The `OnVideoFrameBitmap` event can be combined with popular computer vision libraries:

```csharp
// Example using a hypothetical computer vision library
private void VideoProcessor_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Convert bitmap to format needed by CV library
    byte[] imageData = ConvertBitmapToByteArray(e.Frame);
    
    // Process with CV library
    var results = _computerVisionProcessor.DetectFaces(imageData, e.Frame.Width, e.Frame.Height);
    
    // Draw results back onto frame
    using (Graphics g = Graphics.FromImage(e.Frame))
    {
        foreach (var face in results)
        {
            g.DrawRectangle(new Pen(Color.Yellow, 2), face.X, face.Y, face.Width, face.Height);
        }
        
        e.UpdateData = true;
    }
}
```

## Troubleshooting Common Issues

### Memory Leaks

If you experience memory growth during prolonged video processing:

1. Ensure all `Graphics` objects are disposed
2. Properly dispose of any temporary `Bitmap` objects
3. Avoid capturing large objects in lambda expressions

### Performance Degradation

If frame processing becomes sluggish:

1. Profile your event handler to identify bottlenecks
2. Consider reducing processing frequency
3. Optimize GDI+ operations or consider DirectX for performance-critical applications

## SDK Integration

The `OnVideoFrameBitmap` event is available in the following SDKs:

## Required Dependencies

To use the functionality described in this guide, you'll need:

- SDK redistribution package
- System.Drawing (included in .NET Framework)
- Windows GDI+ support

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and projects demonstrating these techniques in action.