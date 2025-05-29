---
title: Creating Custom Video Effects in C# Applications
description: Learn how to implement custom video effects in C# applications using OnVideoFrameBitmap and OnVideoFrameBuffer events. Discover practical code examples for real-time video processing including text overlays, grayscale conversion, brightness adjustments, and timestamp watermarks.
sidebar_label: Custom Video Effects with Frame Events

---

# Creating Custom Real-time Video Effects in C# Applications

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

## Introduction to Video Frame Processing

When developing video applications, you often need to apply custom effects or overlays to video streams in real-time. The .NET SDK provides two powerful events for this purpose: `OnVideoFrameBitmap` and `OnVideoFrameBuffer`. These events give you direct access to each video frame, allowing you to modify pixels before they're rendered or encoded.

## Implementation Methods

There are two primary approaches to implementing custom video effects:

1. **Using OnVideoFrameBitmap**: Process frames as Bitmap objects with GDI+ - easier to use but with moderate performance
2. **Using OnVideoFrameBuffer**: Manipulate raw RGB24 image buffer directly - offers better performance but requires more low-level code

## Code Examples for Custom Video Effects

### Text Overlay Implementation

Adding text overlays to video is useful for watermarking, displaying information, or creating captions. This example demonstrates how to add simple text to your video frames:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);

    grf.DrawString("Hello!", new Font(FontFamily.GenericSansSerif, 20), new SolidBrush(Color.White), 20, 20);
    grf.Dispose();

    e.UpdateData = true;
}
```

### Grayscale Effect Implementation

Converting video to grayscale is a fundamental image processing technique. This example shows how to access and modify individual pixel values:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Apply standard luminance formula (0.3R + 0.59G + 0.11B) for accurate grayscale conversion
    for (int i = 0; i < rgbValues.Length; i += 3)
    {
        int gray = (int)(rgbValues[i] * 0.3 + rgbValues[i + 1] * 0.59 + rgbValues[i + 2] * 0.11);
        rgbValues[i] = (byte)gray;
        rgbValues[i + 1] = (byte)gray;
        rgbValues[i + 2] = (byte)gray;
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Brightness Adjustment Implementation

This example demonstrates how to adjust the brightness of video frames - a common requirement in video processing applications:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    float brightness = 1.2f; // Values > 1 increase brightness, < 1 decrease it
    
    Bitmap bmp = e.Frame;
    Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
    System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
    
    IntPtr ptr = bmpData.Scan0;
    int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
    byte[] rgbValues = new byte[bytes];
    
    System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
    
    // Apply brightness adjustment to each color channel
    for (int i = 0; i < rgbValues.Length; i++)
    {
        int newValue = (int)(rgbValues[i] * brightness);
        rgbValues[i] = (byte)Math.Min(255, Math.Max(0, newValue));
    }
    
    System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
    bmp.UnlockBits(bmpData);
    
    e.UpdateData = true;
}
```

### Timestamp Overlay Implementation

Adding timestamps to video frames is essential for surveillance and logging applications. This example shows how to create a professional-looking timestamp with a semi-transparent background:

```cs
private void VideoCapture1_OnVideoFrameBitmap(object sender, VideoFrameBitmapEventArgs e)
{
    Graphics grf = Graphics.FromImage(e.Frame);
    
    // Create a semi-transparent background for better readability
    Rectangle textBackground = new Rectangle(10, e.Frame.Height - 50, 250, 40);
    grf.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), textBackground);
    
    // Display current date and time
    string dateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    grf.DrawString(dateTime, new Font(FontFamily.GenericSansSerif, 16), 
                  new SolidBrush(Color.White), 15, e.Frame.Height - 45);
    
    grf.Dispose();
    
    e.UpdateData = true;
}
```

## Performance Optimization Tips

### Working with Raw Buffer Data

For high-performance applications, processing raw buffer data offers significant speed advantages:

```cs
// OnVideoFrameBuffer event example (pseudo-code)
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // e.Buffer contains raw RGB24 data
    // Each pixel uses 3 bytes in RGB order
    // Process directly for maximum performance
}
```

### Best Practices for Frame Processing

* **Memory Management**: Always dispose Graphics objects and unlock bitmapped data
* **Performance Considerations**: For real-time processing, keep operations lightweight
* **Buffer Processing**: We strongly recommend processing RAW data in the OnVideoFrameBuffer event for optimal performance
* **External Libraries**: Consider using Intel IPP or other optimized image-processing libraries for complex operations

---

## Additional Resources

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to access more code samples and complete project examples.
