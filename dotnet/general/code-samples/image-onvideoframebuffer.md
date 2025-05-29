---
title: Drawing Images with OnVideoFrameBuffer in .NET
description: Learn how to implement image drawing using the OnVideoFrameBuffer event in .NET applications. This step-by-step guide with C# code samples shows you how to efficiently overlay images on video frames in real-time for video processing applications.
sidebar_label: Drawing Images with OnVideoFrameBuffer

---

# Drawing Images with OnVideoFrameBuffer in .NET

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

## Introduction

The `OnVideoFrameBuffer` event provides a powerful way to manipulate video frames in real-time. This guide demonstrates how to overlay images on video frames using this event in .NET applications. This technique is useful for adding watermarks, logos, or other visual elements to video content during processing or playback.

## Understanding the Process

When working with video frames in .NET, you need to:

1. Load your image (logo, watermark, etc.) into memory
2. Convert the image to a compatible buffer format
3. Listen for the `OnVideoFrameBuffer` event
4. Draw the image onto each video frame as it's processed
5. Update the frame data to display the changes

## Code Implementation

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
            ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format32bppArgb);
        }
        else
        {
            ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                PixelFormat.Format24bppRgb);
        }
    }

    // Draw image
    if (logoImage.PixelFormat == PixelFormat.Format32bppArgb)
    {
        FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width,
            e.Frame.Height, 0, 0);
    }
    else
    {
        FastImageProcessing.Draw_RGB24OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width,
            e.Frame.Height, 0, 0);
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

4. **Image Conversion**: Converts the Bitmap to raw pixel data in the allocated buffer using `ImageHelper.BitmapToIntPtr()`.

### Drawing Process

Once the buffer is prepared, drawing takes place:

1. **Format-Specific Drawing**: The code selects the appropriate drawing method based on the image format:
   - `FastImageProcessing.Draw_RGB32OnRGB24()` for 32-bit images with transparency
   - `FastImageProcessing.Draw_RGB24OnRGB24()` for standard 24-bit images

2. **Position Parameters**: The `0, 0` parameters specify where to draw the image (top-left corner in this example).

3. **Frame Update**: Setting `e.UpdateData = true` ensures the modified frame data is used for display or further processing.

## Best Practices for Image Overlay

For optimal performance when overlaying images on video frames:

1. **Memory Management**: Always free allocated memory when it's no longer needed to prevent memory leaks.

2. **Buffer Reuse**: Create the buffer once and reuse it for subsequent frames rather than recreating it for each frame.

3. **Image Size Considerations**: Use appropriately sized images; overlaying large images can impact performance.

4. **Format Selection**:
   - Use PNG (RGB32) when you need transparency
   - Use JPG (RGB24) when transparency isn't required (more efficient)

5. **Position Calculation**: For dynamic positioning, calculate coordinates based on frame dimensions:

   ```cs
   // Example: Position logo at bottom-right corner with 10px padding
   int xPos = e.Frame.Width - logoImage.Width - 10;
   int yPos = e.Frame.Height - logoImage.Height - 10;
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

The `OnVideoFrameBuffer` event provides a powerful way to manipulate video frames in real-time. By following this guide, you can efficiently overlay images on video content for watermarking, branding, or visual enhancement purposes.

The technique demonstrated here works across multiple SDK products and can be adapted for various video processing scenarios in your .NET applications.

---

Looking for more code samples? Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for additional examples and resources.
