---
title: Multiple Output Video Screens in WPF Applications
description: Create multi-display video applications in WPF with Image controls, event handling, memory management, and performance optimization techniques.
---

# Implementing Multiple Video Output Screens in WPF Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

When developing WPF applications that require handling multiple video feeds simultaneously, developers often face challenges with performance, synchronization, and resource management. This guide provides a comprehensive approach to implementing multiple video output screens in your WPF applications using C# and the Image control.

## Getting Started with Multiple Video Screens

Check the installation guide for WPF [here](../../install/index.md).

To begin implementing multiple video outputs in your WPF application, you'll need to:

1. Add the appropriate Video View control to your application
2. Set up event handling for video frame processing
3. Configure your rendering pipeline for optimal performance

### Setting Up Your WPF Project

First, place the `VisioForge.Core.UI.WPF.VideoView` control on your WPF window. It's recommended to give this control a descriptive name, such as `videoView`, for clarity in your code. This control will serve as your primary video display element.

### Handling Video Frames

The key to creating multiple output screens is proper event handling. You'll need to subscribe to the "OnVideoFrameBuffer" event for your SDK control. This event provides access to the raw video frame data that you can then distribute to multiple display elements.

## Implementing the Video Frame Handler

Below is a sample implementation of the video frame handler that captures incoming frames and renders them to a video view:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    videoView.RenderFrame(e);
}
```

This simple handler receives video frames through the `VideoFrameBufferEventArgs` parameter and passes them to the `RenderFrame` method of your video view control.

## Advanced Implementation Techniques

### Creating Dynamic Video Views

For applications requiring a variable number of video outputs, you can dynamically create video view controls:

```cs
private List<VisioForge.Core.UI.WPF.VideoView> videoViews = new List<VisioForge.Core.UI.WPF.VideoView>();

private void CreateVideoView(Grid container, int row, int column)
{
    var videoView = new VisioForge.Core.UI.WPF.VideoView();
    Grid.SetRow(videoView, row);
    Grid.SetColumn(videoView, column);
    
    container.Children.Add(videoView);
    videoViews.Add(videoView);
}

// Usage example:
// CreateVideoView(mainGrid, 0, 0);
// CreateVideoView(mainGrid, 0, 1);
```

### Distributing Video Frames to Multiple Views

When working with multiple video views, you need to distribute incoming frames to all active views:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Render to all video views
    foreach (var view in videoViews)
    {
        view.RenderFrame(e);
    }
}
```

### Memory Management Considerations

When working with multiple video outputs, memory management becomes a critical concern. Video frames can consume significant memory, especially at higher resolutions. Consider implementing a frame pooling mechanism:

```cs
private ConcurrentQueue<VideoFrame> framePool = new ConcurrentQueue<VideoFrame>();
private const int MaxPoolSize = 10;

private VideoFrame GetFrameFromPool()
{
    if (framePool.TryDequeue(out var frame))
    {
        return frame;
    }
    
    return new VideoFrame();
}

private void ReturnFrameToPool(VideoFrame frame)
{
    frame.Clear();
    
    if (framePool.Count < MaxPoolSize)
    {
        framePool.Enqueue(frame);
    }
}
```

## Performance Optimization Strategies

### Reducing Render Load

For multiple video views, consider these optimization techniques:

1. **Adaptive resolution**: Scale down the resolution for secondary displays
2. **Frame skipping**: Not every view needs to update at full frame rate
3. **Asynchronous rendering**: Offload rendering to background threads

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Primary view gets full resolution, full frame rate
    primaryVideoView.RenderFrame(e);
    
    // Secondary views get every second frame
    if (frameCounter % 2 == 0)
    {
        foreach (var view in secondaryVideoViews)
        {
            Task.Run(() => view.RenderFrameScaled(e, 0.5)); // Scaled down by 50%
        }
    }
    
    frameCounter++;
}
```

## Practical Example: Four-Camera Security System

Here's a more complete example of implementing a four-camera security system:

```cs
public partial class SecurityMonitorWindow : Window
{
    private List<VisioForge.Core.UI.WPF.VideoView> cameraViews = new List<VisioForge.Core.UI.WPF.VideoView>();
    private List<VideoCapture> cameras = new List<VideoCapture>();
    
    public SecurityMonitorWindow()
    {
        InitializeComponent();
        
        // Set up 2x2 grid of camera views
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++)
            {
                var view = new VisioForge.Core.UI.WPF.VideoView();
                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                mainGrid.Children.Add(view);
                cameraViews.Add(view);
                
                // Create and configure camera
                var camera = new VideoCapture();
                camera.OnVideoFrameBuffer += (s, e) => view.RenderFrame(e);
                cameras.Add(camera);
            }
        }
    }
    
    public async Task StartCamerasAsync()
    {
        for (int i = 0; i < cameras.Count; i++)
        {
            cameras[i].VideoSource = VideoSource.CameraSource;
            cameras[i].CameraDevice = new CameraDevice(i); // Assuming cameras are indexed 0-3
            await cameras[i].StartAsync();
        }
    }
}
```

## Troubleshooting Common Issues

### Handling Frame Synchronization

If you experience frame timing issues across multiple displays:

```cs
private readonly object syncLock = new object();

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    lock (syncLock)
    {
        foreach (var view in videoViews)
        {
            view.RenderFrame(e);
        }
    }
}
```

---
For more code samples and advanced implementation techniques, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).