---
title: Implementing Dynamic Text Overlays on Video Frames
description: Learn how to create, position, and update multiple text overlays on video frames using the OnVideoFrameBuffer event in .NET. This detailed guide with code examples shows you how to customize text properties, handle dynamic updates, and optimize performance.
sidebar_label: Draw Multiple Text Overlays Using OnVideoFrameBuffer Event

---

# Implementing Dynamic Text Overlays on Video Frames in .NET

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

## Introduction

Adding text overlays to video content has become essential for various applications, from adding watermarks and timestamps to creating informative annotations and captions. While many SDKs offer built-in text overlay capabilities, these functions might not always provide the level of customization or flexibility required for advanced projects.

This guide demonstrates how to implement custom text overlays using the `OnVideoFrameBuffer` event. This approach gives you full control over the text appearance, position, and behavior, allowing for more sophisticated overlay implementations than what's possible with standard API methods.

## Why Use Custom Text Overlays?

Standard text overlay APIs often have limitations in areas such as:

- Number of concurrent text elements
- Font customization options
- Dynamic text updates
- Animation capabilities
- Precise positioning control
- Alpha channel management

By leveraging the `OnVideoFrameBuffer` event and working directly with bitmap data, you can overcome these limitations and implement exactly what your application needs.

## Understanding the Approach

The technique demonstrated in this article involves:

1. Creating a transparent bitmap with the same dimensions as the video frame
2. Drawing text elements onto this bitmap using GDI+ (System.Drawing)
3. Converting the bitmap to a memory buffer
4. Overlaying this buffer onto the video frame data
5. Optionally updating text elements dynamically

This provides a powerful method for text overlay creation while maintaining good performance.

## Basic Implementation

The following code sample shows a straightforward implementation for drawing multiple text overlays on video frames:

```cs
        // Image
        private Bitmap logoImage = null;

        // Image RGB32 buffer
        private IntPtr logoImageBuffer = IntPtr.Zero;
        private int logoImageBufferSize = 0;

        private string text1 = "Hello World";
        private string text2 = "Hey-hey";
        private string text3 = "Ocean of pancakes";

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            // draw text to image
            if (logoImage == null)
            {
                logoImage = new Bitmap(e.Frame.Width, e.Frame.Height, PixelFormat.Format32bppArgb);

                using (var grf = Graphics.FromImage(logoImage))
                {
                    // antialiasing mode
                    grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                    // drawing mode
                    grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    // smoothing mode
                    grf.SmoothingMode = SmoothingMode.HighQuality;

                    // text 1
                    var brush1 = new SolidBrush(Color.Blue);
                    var font1 = new Font("Arial", 30, FontStyle.Regular);
                    grf.DrawString(text1, font1, brush1, 100, 100);

                    // text 2
                    var brush2 = new SolidBrush(Color.Red);
                    var font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
                    grf.DrawString(text2, font2, brush2, e.Frame.Width / 2, e.Frame.Height / 2);

                    // text 3
                    var brush3 = new SolidBrush(Color.Green);
                    var font3 = new Font("Verdana", 40, FontStyle.Italic);
                    grf.DrawString(text3, font3, brush3, 200, 200);
                }
            }

            // create image buffer if not allocated or have zero size
            if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
            {
                if (logoImageBuffer == IntPtr.Zero)
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }
                else
                {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(logoImage.Width) * logoImage.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                }

                ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
            }

            // Draw image
            FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width, e.Frame.Height, 0, 0);

            e.UpdateData = true;
        }
```

### Key Components Explained

1. **Bitmap Creation**: We create a 32-bit bitmap (with alpha channel) matching the video frame dimensions
2. **Graphics Settings**: We configure anti-aliasing, interpolation, and smoothing for high-quality text rendering
3. **Text Configuration**: Each text element gets its own font, color, and position
4. **Memory Management**: We allocate unmanaged memory for the bitmap buffer
5. **Bitmap to Buffer Conversion**: We convert the bitmap to a memory buffer using `ImageHelper.BitmapToIntPtr`
6. **Buffer Overlay**: We draw the RGBA buffer onto the video frame using `FastImageProcessing.Draw_RGB32OnRGB24`
7. **Frame Update Flag**: We set `e.UpdateData = true` to inform the SDK that the frame data has been modified

## Advanced Implementation with Dynamic Updates

For more interactive applications, you might need to update text overlays dynamically. The following implementation supports on-the-fly updates of text content, fonts, and colors:

```cs
        // Image
        Bitmap logoImage = null;

        // Image RGB32 buffer
        IntPtr logoImageBuffer = IntPtr.Zero;
        int logoImageBufferSize = 0;

        // text settings
        string text1 = "Hello World";
        Font font1 = new Font("Arial", 30, FontStyle.Regular);
        SolidBrush brush1 = new SolidBrush(Color.Blue);

        string text2 = "Hey-hey";
        Font font2 = new Font("Times New Roman", 35, FontStyle.Strikeout);
        SolidBrush brush2 = new SolidBrush(Color.Red);

        string text3 = "Ocean of pancakes";
        Font font3 = new Font("Verdana", 40, FontStyle.Italic);
        SolidBrush brush3 = new SolidBrush(Color.Green);

        // update flag
        bool textUpdate = false;
        object textLock = new object();

        // Update text overlay, index is [1..3]
        void UpdateText(int index, string text, Font font, SolidBrush brush)
        {
            lock (textLock)
            {
                textUpdate = true;
            }

            switch (index)
            {
                case 1:
                    text1 = text;
                    font1 = font;
                    brush1 = brush;
                    break;
                case 2:
                    text2 = text;
                    font2 = font;
                    brush2 = brush;
                    break;
                case 3:
                    text3 = text;
                    font3 = font;
                    brush3 = brush;
                    break;
                default:
                    return;
            }
        }

        private void SDK_OnVideoFrameBuffer(Object sender, VideoFrameBufferEventArgs e)
        {
            lock (textLock)
            {
                if (textUpdate)
                {
                    logoImage.Dispose();
                    logoImage = null;
                }

                // draw text to image
                if (logoImage == null)
                {
                    logoImage = new Bitmap(e.Frame.Width, e.Frame.Height, PixelFormat.Format32bppArgb);

                    using (var grf = Graphics.FromImage(logoImage))
                    {
                        // antialiasing mode
                        grf.TextRenderingHint = TextRenderingHint.AntiAlias;

                        // drawing mode
                        grf.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        // smoothing mode
                        grf.SmoothingMode = SmoothingMode.HighQuality;

                        // text 1
                        grf.DrawString(text1, font1, brush1, 100, 100);

                        // text 2
                        grf.DrawString(text2, font2, brush2, e.Frame.Width / 2, e.Frame.Height / 2);

                        // text 3
                        grf.DrawString(text3, font3, brush3, 200, 200);
                    }
                }

                // create image buffer if not allocated or have zero size
                if (logoImageBuffer == IntPtr.Zero || logoImageBufferSize == 0)
                {
                    if (logoImageBuffer == IntPtr.Zero)
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Width) * e.Frame.Height;
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }
                    else
                    {
                        logoImageBufferSize = ImageHelper.GetStrideRGB32(e.Frame.Width) * e.Frame.Height;

                        Marshal.FreeCoTaskMem(logoImageBuffer);
                        logoImageBuffer = Marshal.AllocCoTaskMem(logoImageBufferSize);
                    }

                    ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                if (textUpdate)
                {
                    textUpdate = false;
                    ImageHelper.BitmapToIntPtr(logoImage, logoImageBuffer, logoImage.Width, logoImage.Height,
                        PixelFormat.Format32bppArgb);
                }

                // Draw image
                FastImageProcessing.Draw_RGB32OnRGB24(logoImageBuffer, logoImage.Width, logoImage.Height, e.Frame.Data, e.Frame.Width,
                e.Frame.Height, 0, 0);

                e.UpdateData = true;
            }
        }

        private void btUpdateText1_Click(object sender, EventArgs e)
        {
            UpdateText(1, "Hello world", new Font("Arial", 48, FontStyle.Underline),
                new SolidBrush(Color.Aquamarine));
        }
```

### New Features in the Advanced Implementation

1. **Thread Safety**: We use a lock object to prevent concurrent access to shared resources
2. **Update Mechanism**: The `UpdateText` method provides a clean interface for changing text properties
3. **Text Property Storage**: Each text element has its own variables for content, font, and color
4. **Change Detection**: We use a flag (`textUpdate`) to indicate when text properties have changed
5. **Resource Management**: We dispose of the old bitmap when text properties change
6. **Buffer Update**: We update the memory buffer when text properties change
7. **UI Integration**: A sample button click handler demonstrates how to trigger text updates

## Performance Optimization Tips

When implementing text overlays with this method, consider these performance optimizations:

1. **Minimize Bitmap Recreations**: Only recreate the bitmap when necessary (text changes, resolution changes)
2. **Cache Font Objects**: Font creation is expensive; create fonts once and reuse them
3. **Use Memory Efficiently**: Free unmanaged memory when it's no longer needed
4. **Optimize Drawing Operations**: Use hardware acceleration when available
5. **Consider Update Frequency**: For frequent updates, consider double-buffering techniques
6. **Profile Your Code**: Use performance profiling tools to identify bottlenecks

## Advanced Features to Consider

This basic implementation can be extended with additional features:

1. **Text Animation**: Implement text movement, fading, or other animations
2. **Text Formatting**: Add support for rich text formatting (bold, italic, etc.)
3. **Text Effects**: Implement shadows, outlines, or glow effects
4. **Text Alignment**: Add support for different text alignment options
5. **Multi-Line Text**: Implement proper handling of multi-line text with wrapping
6. **Localization**: Add support for different languages and text directions
7. **Performance Monitoring**: Add diagnostics to monitor rendering performance

## Memory Management Considerations

When working with unmanaged memory, it's crucial to handle resource cleanup properly:

1. Implement the `IDisposable` pattern in your class
2. Free unmanaged memory in the `Dispose` method
3. Consider using `SafeHandle` or similar constructs for safer resource management
4. Set buffer pointers to `IntPtr.Zero` after freeing them
5. Use structured exception handling around memory operations

## Cleanup Example

```cs
protected override void Dispose(bool disposing)
{
    if (disposing)
    {
        // Dispose managed resources
        if (logoImage != null)
        {
            logoImage.Dispose();
            logoImage = null;
        }
    }
    
    // Free unmanaged resources
    if (logoImageBuffer != IntPtr.Zero)
    {
        Marshal.FreeCoTaskMem(logoImageBuffer);
        logoImageBuffer = IntPtr.Zero;
        logoImageBufferSize = 0;
    }
    
    base.Dispose(disposing);
}
```

## Required Dependencies

- SDK redistributable components

## Conclusion

Implementing custom text overlays using the `OnVideoFrameBuffer` event provides a powerful and flexible solution for applications that require advanced text display capabilities. While it requires more code than using built-in API methods, the additional flexibility and control make it worthwhile for sophisticated video applications.

By following the patterns demonstrated in this guide, you can create dynamic, high-quality text overlays that can be updated in real-time, providing a rich user experience in your video applications.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
