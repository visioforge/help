---
title: Text Overlay Implementation with OnVideoFrameBuffer
description: Create custom text overlays on video frames with OnVideoFrameBuffer event for dynamic text rendering and professional video processing in .NET.
---

# Creating Custom Text Overlays with OnVideoFrameBuffer in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Text Overlays in Video Processing

Adding text overlays to video content is a common requirement in many professional applications, from video editing software to security camera feeds, broadcasting tools, and educational applications. While the standard video effect APIs provide basic text overlay capabilities, developers often need more control over how text appears on video frames.

This guide demonstrates how to manually implement custom text overlays using the OnVideoFrameBuffer event available in VideoCaptureCore, VideoEditCore, and MediaPlayerCore engines. By intercepting video frames during processing, you can apply custom text and graphics with precise control over positioning, formatting, and animation.

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

    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
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
FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
```

This line does the actual work of rendering the text onto the frame:

- It takes the frame data buffer as input
- Uses the frame dimensions to properly position the text
- References the textLogo object containing text properties
- Can utilize the timestamp for dynamic content

## Advanced Customization Options

While the basic example demonstrates a simple static text overlay, the VideoEffectTextLogo class supports numerous customization options:

### Text Formatting

```cs
textLogo.FontName = "Arial";
textLogo.FontSize = 24;
textLogo.FontBold = true;
textLogo.FontItalic = false;
textLogo.Color = System.Drawing.Color.White;
textLogo.Opacity = 0.8f;
```

### Background and Borders

```cs
textLogo.BackgroundEnabled = true;
textLogo.BackgroundColor = System.Drawing.Color.Black;
textLogo.BackgroundOpacity = 0.5f;
textLogo.BorderEnabled = true;
textLogo.BorderColor = System.Drawing.Color.Yellow;
textLogo.BorderThickness = 2;
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
    
    // Update text based on timestamp
    textLogo.Text = $"Timestamp: {e.Timestamp.ToString("HH:mm:ss.fff")}";
    
    // Animate position
    textLogo.Left = 50 + (int)(Math.Sin(e.Timestamp.TotalSeconds) * 50);
    
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref textLogo, e.Timestamp, 0);
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
    timestampLogo.Text = e.Timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff");
    
    // Render both text elements
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref titleLogo, e.Timestamp, 0);
    FastImageProcessing.AddTextLogo(null, e.Frame.Data, e.Frame.Width, e.Frame.Height, ref timestampLogo, e.Timestamp, 0);
}
```

## Required Components

To implement this solution, you'll need:

- SDK redist package installed in your application
- Reference to the appropriate SDK (.NET Video Capture, Video Edit, or Media Player)
- Basic understanding of video frame processing concepts

## Conclusion

The OnVideoFrameBuffer event provides a powerful mechanism for implementing custom text overlays in video applications. By directly accessing the frame buffer, developers can create sophisticated text effects with precise control over appearance and behavior.

This approach is particularly valuable when standard text overlay APIs don't provide the flexibility or features required for your application. With the techniques demonstrated in this guide, you can implement professional-quality text overlays for a wide range of video processing scenarios.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.