---
title: Setting Zoom Parameters for Multiple Video Renderers
description: Configure independent zoom and position parameters for multiple video renderers across multi-screen displays in .NET multimedia applications.
---

# Configuring Zoom Settings for Multiple Video Renderers in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

When developing multimedia applications that utilize multiple video renderers, controlling the zoom and position parameters independently for each display is essential for creating professional-quality user interfaces. This guide covers the implementation details, parameter configurations, and best practices for setting up multiple video renderers with customized zoom settings in your .NET applications.

## Understanding Multiple Renderer Configurations

Multiple renderer support (also known as multiscreen functionality) allows your application to display video content across different display areas simultaneously. Each renderer can be configured with its own:

- Zoom ratio (magnification level)
- Horizontal shift (X-axis positioning)  
- Vertical shift (Y-axis positioning)

This capability is particularly valuable for applications such as:

- Video surveillance systems displaying multiple camera feeds
- Media production software with preview and output windows
- Medical imaging applications requiring different zoom levels for analysis
- Multi-display kiosk systems with synchronized content

## Implementing the MultiScreen_SetZoom Method

The SDK provides the `MultiScreen_SetZoom` method which takes four key parameters:

1. **Screen Index** (zero-based): Identifies which renderer to configure
2. **Zoom Ratio**: Controls the magnification percentage
3. **Shift X**: Adjusts the horizontal positioning (pixels or percentage)
4. **Shift Y**: Adjusts the vertical positioning (pixels or percentage)

### Method Signature and Parameters

```cs
// Method signature
void MultiScreen_SetZoom(int screenIndex, int zoomRatio, int shiftX, int shiftY);
```

| Parameter | Description | Valid Range |
|-----------|-------------|-------------|
| screenIndex | Zero-based index of the target renderer | 0 to (number of renderers - 1) |
| zoomRatio | Magnification percentage | 1 to 1000 (%) |
| shiftX | Horizontal offset | -1000 to 1000 |
| shiftY | Vertical offset | -1000 to 1000 |

## Code Sample: Configuring Multiple Renderers

The following example demonstrates how to set different zoom and positioning values for three separate renderers:

```cs
// Configure the primary renderer (index 0)
// 50% zoom with no horizontal or vertical shift
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);

// Configure the secondary renderer (index 1)
// 20% zoom with slight horizontal and vertical shift
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);

// Configure the tertiary renderer (index 2)
// 30% zoom with no horizontal shift but significant vertical shift
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);
```

## Best Practices for Multiple Renderer Management

When implementing multiple renderer configurations, consider these best practices:

### 1. Initialize All Renderers Before Setting Zoom

Always ensure that all renderers are properly initialized before applying zoom settings:

```cs
// Initialize multiple renderers
VideoCapture1.MultiScreen_Enabled = true;

// Add 3 renderers
VideoCapture1.MultiScreen_AddScreen(videoView1, 1280, 720);
VideoCapture1.MultiScreen_AddScreen(videoView2, 1920, 1080);
VideoCapture1.MultiScreen_AddScreen(videoView3, 1280, 720);

// Now safe to configure zoom settings
VideoCapture1.MultiScreen_SetZoom(0, 50, 0, 0);
VideoCapture1.MultiScreen_SetZoom(1, 20, 10, 20);
VideoCapture1.MultiScreen_SetZoom(2, 30, 0, 30);

// Additional configurations...
```

### 2. Handle Resolution Changes Appropriately

When the input source resolution changes, you may need to recalculate zoom values:

```cs
private void VideoCapture1_OnVideoSourceResolutionChanged(object sender, EventArgs e)
{
    // Recalculate and apply zoom settings based on new resolution
    int newZoom = CalculateOptimalZoom(VideoCapture1.VideoSource_ResolutionX, 
                                       VideoCapture1.VideoSource_ResolutionY);
    
    // Apply to all renderers
    for (int i = 0; i < VideoCapture1.MultiScreen_Count; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, newZoom, 0, 0);
    }
}
```

### 3. Provide User Controls for Zoom Adjustment

For interactive applications, consider implementing UI controls that allow users to adjust zoom settings:

```cs
private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
{
    int selectedRenderer = rendererComboBox.SelectedIndex;
    int zoomValue = zoomTrackBar.Value;
    int shiftX = horizontalShiftTrackBar.Value;
    int shiftY = verticalShiftTrackBar.Value;
    
    // Apply new zoom settings to selected renderer
    VideoCapture1.MultiScreen_SetZoom(selectedRenderer, zoomValue, shiftX, shiftY);
}
```

## Advanced Zoom Configurations

### Dynamic Zoom Transitions

For smooth zoom transitions, consider implementing gradual zoom changes:

```cs
async Task AnimateZoomAsync(int screenIndex, int startZoom, int targetZoom, int duration)
{
    int steps = 30; // Number of animation steps
    int delay = duration / steps; // Milliseconds between steps
    
    for (int i = 0; i <= steps; i++)
    {
        // Calculate intermediate zoom value
        int currentZoom = startZoom + ((targetZoom - startZoom) * i / steps);
        
        // Apply current zoom value
        VideoCapture1.MultiScreen_SetZoom(screenIndex, currentZoom, 0, 0);
        
        // Wait for next step
        await Task.Delay(delay);
    }
}

// Usage
await AnimateZoomAsync(0, 50, 100, 1000); // Animate from 50% to 100% over 1 second
```

## Optimizing Performance with Multiple Renderers

When working with multiple renderers, be mindful of performance implications:

1. **Limit Frequent Updates**: Avoid rapidly changing zoom settings as it can impact performance
2. **Consider Hardware Acceleration**: Enable hardware acceleration when available
3. **Monitor Memory Usage**: Multiple high-resolution renderers can consume significant memory

```cs
// Enable hardware acceleration for better performance
VideoCapture1.Video_Renderer = VideoRendererType.EVR;
VideoCapture1.Video_Renderer_EVR_Mode = EVRMode.Optimal;
```

## Troubleshooting Common Issues

### Issue: Renderers Show Black Screen After Zoom Changes

This can occur when zoom values exceed valid ranges or when renderers aren't properly initialized:

```cs
// Reset zoom settings to default for all renderers
public void ResetZoomSettings()
{
    for (int i = 0; i < VideoCapture1.MultiScreen_Count; i++)
    {
        VideoCapture1.MultiScreen_SetZoom(i, 100, 0, 0); // 100% zoom, no shift
    }
}
```

### Issue: Distorted Image After Zoom

Extreme zoom values can cause distortion. Implement boundaries for zoom values:

```cs
public void SetSafeZoom(int screenIndex, int requestedZoom, int shiftX, int shiftY)
{
    // Clamp values to safe ranges
    int safeZoom = Math.Clamp(requestedZoom, 10, 200); // 10% to 200%
    int safeShiftX = Math.Clamp(shiftX, -100, 100);
    int safeShiftY = Math.Clamp(shiftY, -100, 100);
    
    VideoCapture1.MultiScreen_SetZoom(screenIndex, safeZoom, safeShiftX, safeShiftY);
}
```

## Conclusion

Properly configured multiple video renderers with independent zoom settings can significantly enhance the user experience in multimedia applications. By following the guidelines and best practices outlined in this document, you can implement sophisticated video display configurations tailored to your specific application requirements.

For additional code examples and implementation guidance, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
