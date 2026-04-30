---
title: Multiple Video Screens in WPF — C# .NET Multi-Display Guide
description: Create multi-display video applications in WPF with Image controls, event handling, memory management, and performance optimization techniques.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows
  - WPF
  - C#
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoFrameBufferEventArgs
  - MultiscreenVideoView
  - VideoView

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

### Two supported patterns

To show the same video feed on multiple views in WPF, pick one of these two real patterns:

1. **`MultiscreenVideoView` + `OnVideoFrameBuffer`** — one SDK engine pushes each frame into as many `MultiscreenVideoView` controls as you like. Use this when a single capture/playback engine drives several on-screen copies.
2. **One engine per `VideoView`** — each display gets its own `VideoCaptureCore` / `MediaPlayerCore` / `VideoEditCore` instance bound to its own regular `VideoView`. Use this when the displays show **different** sources (e.g. a four-camera security grid). See the [Four-Camera Security System](#practical-example-four-camera-security-system) example below.

Regular `VisioForge.Core.UI.WPF.VideoView` does not expose a `RenderFrame` method — it is driven automatically by the engine it's bound to via `CreateAsync(IVideoView)`. Frame fan-out requires `MultiscreenVideoView`.

### Setting Up Your WPF Project for Frame Fan-out

Drop one or more `VisioForge.Core.UI.WPF.MultiscreenVideoView` controls on your WPF window. Give them descriptive names (e.g. `multiView1`, `multiView2`). These are the controls that accept pushed frames.

### Handling Video Frames

Subscribe to the SDK engine's `OnVideoFrameBuffer` event. The event args carry a `VideoFrameBufferEventArgs` that each `MultiscreenVideoView` can render.

## Implementing the Video Frame Handler

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    multiView1.RenderFrame(e);
}
```

`MultiscreenVideoView.RenderFrame(VideoFrameBufferEventArgs)` copies the frame into its own internal surface, so the engine's buffer can be released when the handler returns.

## Advanced Implementation Techniques

### Creating Dynamic MultiscreenVideoViews

For applications requiring a variable number of video outputs, dynamically create `MultiscreenVideoView` controls:

```cs
private List<VisioForge.Core.UI.WPF.MultiscreenVideoView> multiViews = 
    new List<VisioForge.Core.UI.WPF.MultiscreenVideoView>();

private void CreateMultiView(Grid container, int row, int column)
{
    var view = new VisioForge.Core.UI.WPF.MultiscreenVideoView();
    Grid.SetRow(view, row);
    Grid.SetColumn(view, column);
    
    container.Children.Add(view);
    multiViews.Add(view);
}

// Usage example:
// CreateMultiView(mainGrid, 0, 0);
// CreateMultiView(mainGrid, 0, 1);
```

### Distributing Video Frames to Multiple Views

Fan out incoming frames to every registered `MultiscreenVideoView`:

```cs
private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Render to all MultiscreenVideoView instances
    foreach (var view in multiViews)
    {
        view.RenderFrame(e);
    }
}
```

## Performance Optimization Strategies

### Reducing Render Load

For multiple video views, consider these optimization techniques:

1. **Frame skipping**: Not every view needs to update at full frame rate
2. **Hide off-screen views**: WPF culls rendering for collapsed controls — use `Visibility.Collapsed` on views that aren't visible

```cs
private int frameCounter;

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    // Primary view gets every frame
    primaryMultiView.RenderFrame(e);
    
    // Secondary views get every second frame
    if (frameCounter % 2 == 0)
    {
        foreach (var view in secondaryMultiViews)
        {
            view.RenderFrame(e);
        }
    }
    
    frameCounter++;
}
```

## Practical Example: Four-Camera Security System

Here's a more complete example of implementing a four-camera security system using the Video Capture SDK's `VideoCaptureCore` engine. Each camera gets its own engine instance bound to a dedicated `VideoView`.

```cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI.WPF;
using VisioForge.Core.VideoCapture;

public partial class SecurityMonitorWindow : Window
{
    private readonly List<VideoView> _cameraViews = new List<VideoView>();
    private readonly List<VideoCaptureCore> _cameras = new List<VideoCaptureCore>();

    public SecurityMonitorWindow()
    {
        InitializeComponent();
    }

    public async Task InitializeCamerasAsync(IEnumerable<string> deviceNames)
    {
        // Enumerate the first four capture devices if names aren't provided.
        var names = deviceNames?.Take(4).ToList();

        // Build a 2x2 grid of VideoView controls paired with VideoCaptureCore engines.
        int i = 0;
        for (int row = 0; row < 2; row++)
        {
            for (int col = 0; col < 2; col++, i++)
            {
                var view = new VideoView();
                Grid.SetRow(view, row);
                Grid.SetColumn(view, col);
                mainGrid.Children.Add(view);
                _cameraViews.Add(view);

                // The classic engine uses a constructor; CreateAsync is the X-engine pattern.
                // For VideoCaptureCoreX use `await VideoCaptureCoreX.CreateAsync(view)` instead.
                var camera = new VideoCaptureCore(view);
                _cameras.Add(camera);

                if (names != null && i < names.Count)
                {
                    camera.Video_CaptureDevice = new VideoCaptureSource(names[i]);
                    camera.Video_CaptureDevice.Format_UseBest = true;
                    camera.Mode = VideoCaptureMode.VideoPreview;
                    camera.Audio_PlayAudio = false;
                    camera.Audio_RecordAudio = false;
                }
            }
        }
    }

    public async Task StartCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StartAsync();
        }
    }

    public async Task StopCamerasAsync()
    {
        foreach (var camera in _cameras)
        {
            await camera.StopAsync();
        }

        foreach (var camera in _cameras)
        {
            camera.Dispose();
        }
    }
}
```

Enumerate available devices with `camera.Video_CaptureDevices()` (or its async variant) to populate the `deviceNames` argument at runtime — see [the device enumeration guide](../../videocapture/video-sources/video-capture-devices/enumerate-and-select.md).

## Troubleshooting Common Issues

### Handling Frame Synchronization

If you experience frame timing issues across multiple displays:

```cs
private readonly object syncLock = new object();

private void VideoCapture1_OnVideoFrameBuffer(object sender, VideoFrameBufferEventArgs e)
{
    lock (syncLock)
    {
        foreach (var view in multiViews)
        {
            view.RenderFrame(e);
        }
    }
}
```

---
For more code samples and advanced implementation techniques, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).