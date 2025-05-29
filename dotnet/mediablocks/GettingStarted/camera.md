---
title: Creating Camera Applications with Media Blocks SDK
description: Learn how to build powerful camera viewing applications with Media Blocks SDK .Net. This step-by-step tutorial covers device enumeration, format selection, camera configuration, pipeline creation, and video rendering for desktop and mobile platforms.
sidebar_label: Camera Applications

---

# Building Camera Applications with Media Blocks SDK

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## Introduction

This comprehensive guide demonstrates how to create a fully functional camera viewing application using the Media Blocks SDK .Net. The SDK provides a robust framework for capturing, processing, and displaying video streams across multiple platforms including Windows, macOS, iOS, and Android.

## Architecture Overview

To create a camera viewer application, you'll need to understand two fundamental components:

1. **System Video Source** - Captures the video stream from connected camera devices
2. **Video Renderer** - Displays the captured video on screen with configurable settings

These components work together within a pipeline architecture that manages media processing.

## Essential Media Blocks

To build a camera application, you need to add the following blocks to your pipeline:

- **[System Video Source Block](../Sources/index.md)** - Connects to and reads from camera devices
- **[Video Renderer Block](../VideoRendering/index.md)** - Displays the video with configurable rendering options

## Setting Up the Pipeline

### Creating the Base Pipeline

First, create a pipeline object that will manage the media flow:

```csharp
using VisioForge.Core.MediaBlocks;

// Initialize the pipeline
var pipeline = new MediaBlocksPipeline();

// Add error handling
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Pipeline error: {args.Message}");
};
```

### Camera Device Enumeration

Before adding a camera source, you need to enumerate the available devices and select one:

```csharp
// Get all available video devices asynchronously
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();

// Display available devices (useful for user selection)
foreach (var device in videoDevices)
{
    Console.WriteLine($"Device: {device.Name} [{device.API}]");
}

// Select the first available device
var selectedDevice = videoDevices[0];
```

### Camera Format Selection

Each camera supports different resolutions and frame rates. You can enumerate and select the optimal format:

```csharp
// Display available formats for the selected device
foreach (var format in selectedDevice.VideoFormats)
{
    Console.WriteLine($"Format: {format.Width}x{format.Height} {format.Format}");
    
    // Display available frame rates for this format
    foreach (var frameRate in format.FrameRateList)
    {
        Console.WriteLine($"  Frame Rate: {frameRate}");
    }
}

// Select the optimal format (in this example, we look for HD resolution)
var hdFormat = selectedDevice.GetHDVideoFormatAndFrameRate(out var frameRate);
var formatToUse = hdFormat ?? selectedDevice.VideoFormats[0];
```

## Configuring Camera Settings

### Creating Source Settings

Configure the camera source settings with your selected device and format:

```csharp
// Create camera settings with the selected device and format
var videoSourceSettings = new VideoCaptureDeviceSourceSettings(selectedDevice)
{
    Format = formatToUse.ToFormat()
};

// Set the desired frame rate (selecting the highest available)
if (formatToUse.FrameRateList.Count > 0)
{
    videoSourceSettings.Format.FrameRate = formatToUse.FrameRateList.Max();
}

// Optional: Enable force frame rate to maintain consistent timing
videoSourceSettings.Format.ForceFrameRate = true;

// Platform-specific settings
#if __ANDROID__
// Android-specific settings
videoSourceSettings.VideoStabilization = true;
#elif __IOS__ && !__MACCATALYST__
// iOS-specific settings
videoSourceSettings.Position = IOSVideoSourcePosition.Back;
videoSourceSettings.Orientation = IOSVideoSourceOrientation.Portrait;
#endif
```

### Creating the Video Source Block

Now create the system video source block with your configured settings:

```csharp
// Create the video source block
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
```

## Setting Up Video Display

### Creating the Video Renderer

Add a video renderer to display the captured video:

```csharp
// Create the video renderer and connect it to your UI component
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Optional: Configure renderer settings
videoRenderer.Settings.IsSync = true;
```

### Advanced Renderer Configuration

For more control over video rendering, you can customize renderer settings:

```csharp
// Enable snapshot capabilities
videoRenderer.Settings.EnableSnapshot = true;

// Configure subtitle overlay if needed
videoRenderer.SubtitleEnabled = false;
```

## Connecting the Pipeline

Connect the video source to the renderer to establish the media flow:

```csharp
// Connect the output of the video source to the input of the renderer
pipeline.Connect(videoSource.Output, videoRenderer.Input);
```

## Managing the Pipeline Lifecycle

### Starting the Pipeline

Start the pipeline to begin capturing and displaying video:

```csharp
// Start the pipeline asynchronously
await pipeline.StartAsync();
```

### Taking Snapshots

Capture still images from the video stream:

```csharp
// Take a snapshot and save it as a JPEG file
await videoRenderer.Snapshot_SaveAsync("camera_snapshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);

// Or get the snapshot as a bitmap for further processing
var bitmap = await videoRenderer.Snapshot_GetAsync();
```

### Stopping the Pipeline

When finished, properly stop the pipeline:

```csharp
// Stop the pipeline asynchronously
await pipeline.StopAsync();
```

## Platform-Specific Considerations

The Media Blocks SDK supports cross-platform development with specific optimizations:

- **Windows**: Supports both Media Foundation and Kernel Streaming APIs
- **macOS/iOS**: Utilizes AVFoundation for optimal performance
- **Android**: Provides access to camera features like stabilization and orientation

## Error Handling and Troubleshooting

Implement proper error handling to ensure a stable application:

```csharp
try
{
    // Pipeline operations
    await pipeline.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error starting pipeline: {ex.Message}");
    // Handle the exception appropriately
}
```

## Complete Implementation Example

This example demonstrates a complete camera viewer implementation:

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public class CameraViewerExample
{
    private MediaBlocksPipeline _pipeline;
    private SystemVideoSourceBlock _videoSource;
    private VideoRendererBlock _videoRenderer;
    
    public async Task InitializeAsync(IVideoView videoView)
    {
        // Create pipeline
        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, e) => Console.WriteLine(e.Message);
        
        // Enumerate devices
        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0)
        {
            throw new Exception("No camera devices found");
        }
        
        // Select device and format
        var device = devices[0];
        var format = device.GetHDOrAnyVideoFormatAndFrameRate(out var frameRate);
        
        // Create settings
        var settings = new VideoCaptureDeviceSourceSettings(device);
        if (format != null)
        {
            settings.Format = format.ToFormat();
            if (frameRate != null && !frameRate.IsEmpty)
            {
                settings.Format.FrameRate = frameRate;
            }
        }
        
        // Create blocks
        _videoSource = new SystemVideoSourceBlock(settings);
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Build pipeline
        _pipeline.AddBlock(_videoSource);
        _pipeline.AddBlock(_videoRenderer);
        _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);
        
        // Start pipeline
        await _pipeline.StartAsync();
    }
    
    public async Task StopAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            _pipeline.Dispose();
        }
    }
    
    public async Task<bool> TakeSnapshotAsync(string filename)
    {
        return await _videoRenderer.Snapshot_SaveAsync(filename, 
            SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
    }
}
```

## Conclusion

With Media Blocks SDK .Net, building powerful camera applications becomes straightforward. The component-based architecture provides flexibility and performance across platforms while abstracting the complexities of camera device integration.

For complete source code examples, please visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo).
