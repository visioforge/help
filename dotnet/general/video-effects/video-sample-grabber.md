---
title: Video sample grabber usage
description: Extract RAW video frames from Video Capture, Media Player, and Video Edit SDKs with managed memory access and bitmap conversion in C#.
---

# Video sample grabber usage

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Getting RAW video frames as unmanaged memory pointer inside the structure

=== "X-engines"

    
    ```csharp
    // Subscribe to the video frame buffer event
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Process the VideoFrameX object
        ProcessFrame(e.Frame);
        
        // If you've modified the frame and want to update the video stream
        e.UpdateData = true;
    }
    
    // Example of processing a VideoFrameX frame - adjusting brightness
    private void ProcessFrame(VideoFrameX frame)
    {
        // Only process RGB/BGR/RGBA/BGRA formats
        if (frame.Format != VideoFormatX.RGB && 
            frame.Format != VideoFormatX.BGR && 
            frame.Format != VideoFormatX.RGBA && 
            frame.Format != VideoFormatX.BGRA)
        {
            return;
        }
        
        // Get the data as a byte array for manipulation
        byte[] data = frame.ToArray();
        
        // Determine the pixel size based on format
        int pixelSize = (frame.Format == VideoFormatX.RGB || frame.Format == VideoFormatX.BGR) ? 3 : 4;
        
        // Brightness factor (1.2 = 20% brighter, 0.8 = 20% darker)
        float brightnessFactor = 1.2f;
        
        // Process each pixel
        for (int i = 0; i < data.Length; i += pixelSize)
        {
            // Adjust R, G, B channels
            for (int j = 0; j < 3; j++)
            {
                int newValue = (int)(data[i + j] * brightnessFactor);
                data[i + j] = (byte)Math.Min(255, newValue);
            }
        }
        
        // Copy the modified data back to the frame
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Classic engines"

    
    ```csharp
    // Subscribe to the video frame buffer event
    VideoCapture1.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
    {
        // Process the VideoFrame structure
        ProcessFrame(e.Frame);
        
        // If you've modified the frame and want to update the video stream
        e.UpdateData = true;
    }
    
    // Example of processing a VideoFrame - adjusting brightness
    private void ProcessFrame(VideoFrame frame)
    {
        // Only process RGB format for this example
        if (frame.Info.Colorspace != RAWVideoColorSpace.RGB24)
        {
            return;
        }
        
        // Get the data as a byte array for manipulation
        byte[] data = frame.ToArray();
        
        // Brightness factor (1.2 = 20% brighter, 0.8 = 20% darker)
        float brightnessFactor = 1.2f;
        
        // Process each pixel (RGB24 format = 3 bytes per pixel)
        for (int i = 0; i < data.Length; i += 3)
        {
            // Adjust R, G, B channels
            for (int j = 0; j < 3; j++)
            {
                int newValue = (int)(data[i + j] * brightnessFactor);
                data[i + j] = (byte)Math.Min(255, newValue);
            }
        }
        
        // Copy the modified data back to the frame
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    

=== "Media Blocks SDK"

    
    ```csharp
    // Create and set up video sample grabber block
    var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.RGB);
    videoSampleGrabberBlock.OnVideoFrameBuffer += OnVideoFrameBuffer;
    
    private void OnVideoFrameBuffer(object sender, VideoFrameXBufferEventArgs e)
    {
        // Process the VideoFrameX object
        ProcessFrame(e.Frame);
        
        // If you've modified the frame and want to update the video stream
        e.UpdateData = true;
    }
    
    // Example of processing a VideoFrameX frame - adjusting brightness
    private void ProcessFrame(VideoFrameX frame)
    {
        if (frame.Format != VideoFormatX.RGB)
        {
            return;
        }
        
        // Get the data as a byte array for manipulation
        byte[] data = frame.ToArray();
        
        // Brightness factor (1.2 = 20% brighter, 0.8 = 20% darker)
        float brightnessFactor = 1.2f;
        
        // Process each pixel (RGB format = 3 bytes per pixel)
        for (int i = 0; i < data.Length; i += 3)
        {
            // Adjust R, G, B channels
            for (int j = 0; j < 3; j++)
            {
                int newValue = (int)(data[i + j] * brightnessFactor);
                data[i + j] = (byte)Math.Min(255, newValue);
            }
        }
        
        // Copy the modified data back to the frame
        Marshal.Copy(data, 0, frame.Data, data.Length);
    }
    ```
    


## Working with bitmap frames

If you need to work with managed Bitmap objects instead of raw memory pointers, you can use the `OnVideoFrameBitmap` event of the `core` classes or the SampleGrabberBlock:

```csharp
// Subscribe to the bitmap frame event
VideoCapture1.OnVideoFrameBitmap += OnVideoFrameBitmap;

private void OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    // Process the Bitmap object
    ProcessBitmap(e.Frame);
    
    // If you've modified the bitmap and want to update the video stream
    e.UpdateData = true;
}

// Example of processing a Bitmap - adjusting brightness
private void ProcessBitmap(Bitmap bitmap)
{
    // Use Bitmap methods or Graphics to manipulate the image
    // This example uses ColorMatrix for brightness adjustment
    
    // Create a graphics object from the bitmap
    using (Graphics g = Graphics.FromImage(bitmap))
    {
        // Create a color matrix for brightness adjustment
        float brightnessFactor = 1.2f; // 1.0 = no change, >1.0 = brighter, <1.0 = darker
        ColorMatrix colorMatrix = new ColorMatrix(new float[][]
        {
            new float[] {brightnessFactor, 0, 0, 0, 0},
            new float[] {0, brightnessFactor, 0, 0, 0},
            new float[] {0, 0, brightnessFactor, 0, 0},
            new float[] {0, 0, 0, 1, 0},
            new float[] {0, 0, 0, 0, 1}
        });
        
        // Create an ImageAttributes object and set the color matrix
        using (ImageAttributes attributes = new ImageAttributes())
        {
            attributes.SetColorMatrix(colorMatrix);
            
            // Draw the image with the brightness adjustment
            g.DrawImage(bitmap, 
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                0, 0, bitmap.Width, bitmap.Height,
                GraphicsUnit.Pixel, attributes);
        }
    }
}
```

## Working with SkiaSharp for cross-platform applications

For cross-platform applications, the VideoSampleGrabberBlock provides the ability to work with SkiaSharp, a high-performance 2D graphics API for .NET. This is especially useful for applications targeting multiple platforms including mobile and web.

### Using the OnVideoFrameSKBitmap event

```csharp
// First, add the SkiaSharp NuGet package to your project
// Install-Package SkiaSharp

// Import necessary namespaces
using SkiaSharp;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.Events;

// Create a VideoSampleGrabberBlock with RGBA or BGRA format
// Note: OnVideoFrameSKBitmap event works only with RGBA or BGRA formats
var videoSampleGrabberBlock = new VideoSampleGrabberBlock(VideoFormatX.BGRA);

// Enable the SaveLastFrame property if you want to take snapshots later
videoSampleGrabberBlock.SaveLastFrame = true;

// Subscribe to the SkiaSharp bitmap event
videoSampleGrabberBlock.OnVideoFrameSKBitmap += OnVideoFrameSKBitmap;

// Event handler for SkiaSharp bitmap frames
private void OnVideoFrameSKBitmap(object sender, VideoFrameSKBitmapEventArgs e)
{
    // Process the SKBitmap
    ProcessSKBitmap(e.Frame);
    
    // Note: Unlike VideoFrameBitmapEventArgs, VideoFrameSKBitmapEventArgs does not have
    // an UpdateData property as it's designed for frame viewing/analysis only
}

// Example of processing an SKBitmap - adjusting brightness
private void ProcessSKBitmap(SKBitmap bitmap)
{
    // Create a new bitmap to hold the processed image
    using (var surface = SKSurface.Create(new SKImageInfo(bitmap.Width, bitmap.Height)))
    {
        var canvas = surface.Canvas;
        
        // Set up a paint with a color filter for brightness adjustment
        using (var paint = new SKPaint())
        {
            // Create a brightness filter (1.2 = 20% brighter)
            float brightnessFactor = 1.2f;
            var colorMatrix = new float[]
            {
                brightnessFactor, 0, 0, 0, 0,
                0, brightnessFactor, 0, 0, 0,
                0, 0, brightnessFactor, 0, 0,
                0, 0, 0, 1, 0
            };
            
            paint.ColorFilter = SKColorFilter.CreateColorMatrix(colorMatrix);
            
            // Draw the original bitmap with the brightness filter applied
            canvas.DrawBitmap(bitmap, 0, 0, paint);
            
            // If you need to get the result as a new SKBitmap:
            var processedImage = surface.Snapshot();
            using (var processedBitmap = SKBitmap.FromImage(processedImage))
            {
                // Use processedBitmap for further operations or display
                // For example, display it in a SkiaSharp view
                // mySkiaView.SKBitmap = processedBitmap.Copy();
            }
        }
    }
}
```

### Taking snapshots with SkiaSharp

```csharp
// Create a method to capture and save a snapshot
private void CaptureSnapshot(string filePath)
{
    // Make sure SaveLastFrame was enabled on the VideoSampleGrabberBlock
    if (videoSampleGrabberBlock.SaveLastFrame)
    {
        // Get the last frame as an SKBitmap
        using (var bitmap = videoSampleGrabberBlock.GetLastFrameAsSKBitmap())
        {
            if (bitmap != null)
            {
                // Save the bitmap to a file
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.OpenWrite(filePath))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}
```

### Advantages of using SkiaSharp

1. **Cross-platform compatibility**: Works on Windows, macOS, Linux, iOS, Android, and WebAssembly
2. **Performance**: Provides high-performance graphics processing
3. **Modern API**: Offers a comprehensive set of drawing, filtering, and transformation functions
4. **Memory efficiency**: More efficient memory management compared to System.Drawing
5. **No platform dependencies**: No dependency on platform-specific imaging libraries

## Frame processing information

You can get video frames from live sources or files using the `OnVideoFrameBuffer` and `OnVideoFrameBitmap` events.

The `OnVideoFrameBuffer` event is faster and provides the unmanaged memory pointer for the decoded frame. The `OnVideoFrameBitmap` event is slower, but you get the decoded frame as the `Bitmap` class object.

### Understanding the frame objects

- **VideoFrameX** (X-engines): Contains frame data, dimensions, format, timestamp, and methods for manipulating raw video data
- **VideoFrame** (Classic engines): Similar structure but with a different memory layout
- **Common properties**:
  - Width/Height: Frame dimensions
  - Format/Colorspace: Pixel format (RGB, BGR, RGBA, etc.)
  - Stride: Number of bytes per scan line
  - Timestamp: Frame's position in the video timeline
  - Data: Pointer to unmanaged memory with pixel data

### Important considerations

1. The frame's pixel format affects how you process the data:
   - RGB/BGR: 3 bytes per pixel
   - RGBA/BGRA/ARGB: 4 bytes per pixel (with alpha channel)
   - YUV formats: Different component arrangements

2. Set `e.UpdateData = true` if you've modified the frame data and want the changes to be visible in the video stream.

3. For processing that requires multiple frames or complex operations, consider using a buffer or queue to store frames.

4. When using `OnVideoFrameSKBitmap`, select either RGBA or BGRA as the frame format when creating the VideoSampleGrabberBlock.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.