---
title: Screen Capture Source for .NET Video SDK
description: Implement screen capture in .NET apps to capture full screens, windows, or custom areas with DirectX integration and cursor support.
---

# Screen Capture Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction to Screen Capture

Screen capture technology enables developers to programmatically record and stream visual content displayed on a computer monitor. This powerful functionality serves as the foundation for numerous applications including:

- Remote support and technical assistance tools
- Software demonstration and tutorial creation
- Gameplay recording and streaming
- Webinar and presentation systems
- Quality assurance and testing automation

Video Capture SDK .Net provides developers with robust tools for capturing screen content with high performance and flexibility. The SDK supports capturing entire screens, individual application windows, or custom-defined screen regions.

## Platform Support and Technology Overview

### Windows Implementation

On Windows platforms, the SDK leverages the power of DirectX technologies to achieve optimal performance. Developers can choose between:

- **DirectX 9**: Legacy support for older systems
- **DirectX 11/12**: Modern implementation offering superior performance and efficiency

DirectX 11 is particularly recommended for window capture scenarios due to its improved handling of window composition and superior performance characteristics.

=== "VideoCaptureCore"

    
    ### Core Capture Configuration
    
    The VideoCaptureCore implementation provides straightforward configuration options to control the capture process:
    
    - `AllowCaptureMouseCursor`: Enable or disable cursor visibility in the captured content
    - `DisplayIndex`: Select which display to capture in multi-monitor setups (zero-indexed)
    - `ScreenPreview` / `ScreenCapture`: Set the operational mode for viewing or recording
    

=== "VideoCaptureCoreX"

    
    ### Advanced Capture Configuration
    
    VideoCaptureCoreX offers more granular control through dedicated configuration classes:
    
    - `ScreenCaptureDX9SourceSettings`: Configure DirectX 9 based capture
    - `ScreenCaptureD3D11SourceSettings`: Configure DirectX 11 based capture with enhanced performance
    


## Full Screen and Region Capture Implementation

Capturing either a complete screen or a defined screen region is a common requirement for many applications. Below are the implementation approaches for both VideoCaptureCore and VideoCaptureCoreX.

=== "VideoCaptureCore"

    
    ### Configuring Full Screen and Region Capture
    
    The following code demonstrates how to configure screen capture settings for either full screen mode or a specific rectangular region:
    
    ```csharp
    // Set screen capture source settings
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
         // Set to true to capture the full screen
        FullScreen = false,
    
         // Set the left position of the screen area
        Left = 0,
    
        // Set the top position of the screen area
        Top = 0, 
    
        // Set the width of the screen area
        Width = 640, 
    
        // Set the height of the screen area
        Height = 480, 
    
        // Set the display index
        DisplayIndex = 0, 
    
        // Set the frame rate
        FrameRate = new VideoFrameRate(25), 
    
         // Set to true to capture the mouse cursor
        AllowCaptureMouseCursor = true
    };
    ```
    
    When `FullScreen` is set to `true`, the `Left`, `Top`, `Width`, and `Height` properties are ignored, and the entire screen specified by `DisplayIndex` is captured.
    
    For multi-monitor setups, the `DisplayIndex` property identifies which monitor to capture, with 0 representing the primary display.
    

=== "VideoCaptureCoreX"

    
    ### Advanced Screen Capture with DirectX 11
    
    VideoCaptureCoreX provides a more powerful implementation using DirectX 11 technology:
    
    ```cs
    // Display index
    var screenID = 0;
    
    // Create a new screen capture source using DirectX 11
    var source = new ScreenCaptureD3D11SourceSettings(); 
    
    // Set the capture API
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Set the frame rate
    source.FrameRate = new VideoFrameRate(25);
    
    // Set the screen area or full screen mode
    if (fullscreen)
    {
        // Enumerate all screens and set the screen area
        for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
        {
            if (i == screenID)
            {
                source.Rectangle = new VisioForge.Core.Types.Rect(System.Windows.Forms.Screen.AllScreens[i].Bounds);
            }
        }
    }
    else
    {
        // Set the screen area
        source.Rectangle = new VisioForge.Core.Types.Rect(0, 0, 1280, 720); 
    }
    
    // Set to true to capture the mouse cursor
    source.CaptureCursor = true; 
    
    // Set the monitor index
    source.MonitorIndex = screenID; 
    
    // Set the screen capture source
    VideoCapture1.Video_Source = source; 
    ```
    
    The Windows Graphics Capture (WGC) API option provides excellent performance on Windows 10 and higher. This approach also demonstrates the use of `System.Windows.Forms.Screen.AllScreens` to programmatically determine the bounds of available displays.
    


## Window Capture Implementation

Capturing specific application windows allows for targeted recording of individual applications without including other desktop content. This is particularly useful for:

- Application-specific tutorials
- Software demos
- Support scenarios where only a single application is relevant

=== "VideoCaptureCore"

    
    ### Basic Window Capture
    
    To capture a specific window with VideoCaptureCore:
    
    ```csharp
    // Set screen capture source settings
    VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
    {
        // Disable full screen capture
        FullScreen = false, 
    
        // Set the window handle
        WindowHandle = windowHandle, 
    
         // Set the frame rate
        FrameRate = new VideoFrameRate(25),
    
         // Set to true to capture the mouse cursor
        AllowCaptureMouseCursor = true
    };
    ```
    
    The `windowHandle` parameter should contain a valid handle to the target window. This can be obtained using Windows API functions like `FindWindow` or by using UI automation libraries.
    

=== "VideoCaptureCoreX"

    
    ### Enhanced Window Capture
    
    VideoCaptureCoreX provides an optimized window capture implementation:
    
    ```cs
    // Create Direct3D11 source
    var source = new ScreenCaptureD3D11SourceSettings();
    
    // Set the capture API
    source.API = D3D11ScreenCaptureAPI.WGC; 
    
    // Set frame rate
    source.FrameRate = new VideoFrameRate(25);
    
    // Set the window handle
    source.WindowHandle = windowHandle;
    
    VideoCapture1.Video_Source = source; // Set the screen capture source
    ```
    
    The DirectX 11 implementation offers better performance, particularly for capturing applications that use hardware acceleration.
    


## Performance Optimization Techniques

Optimizing screen capture performance is crucial for maintaining high frame rates while minimizing CPU and memory usage. Consider the following best practices:

### Frame Rate Management

Carefully select an appropriate frame rate based on your application requirements:

- For general purpose recording: 15-30 FPS is typically sufficient
- For gaming or motion-intensive content: 30-60 FPS may be necessary
- For static or document-based content: 5-10 FPS can reduce resource usage significantly

### Resolution Considerations

Higher resolution captures require more processing power and memory. Consider:

- Capturing at a lower resolution and scaling up if appropriate
- Using region capture instead of full screen when only part of the screen is relevant
- Implementing resolution switching based on content type

### Hardware Acceleration

When available, using DirectX 11/12 with hardware acceleration can significantly improve performance:

- Reduces CPU load by leveraging the GPU
- Provides better frame rates, especially with high-resolution content
- Allows for more efficient encoding when combined with hardware accelerated video encoders

## Advanced Implementation Scenarios

### Multi-Monitor Configuration

Working with multi-monitor setups requires special consideration:

```csharp
// Detect all available monitors
var screens = System.Windows.Forms.Screen.AllScreens;

// Create a list to present to the user
var screenOptions = new List<string>();
for (int i = 0; i < screens.Length; i++)
{
    screenOptions.Add($"Monitor {i+1}: {screens[i].Bounds.Width} x {screens[i].Bounds.Height}");
}

// Once a selection is made, set the appropriate DisplayIndex/MonitorIndex
```

### Application Window Selection

Providing users with the ability to select a window to capture:

```csharp
// Get all open windows
var openWindows = GetOpenWindows(); // Implementation depends on your approach

// Present the list to the user for selection
// Once selected, get the window handle

// Configure the capture with the selected window handle
VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    WindowHandle = selectedWindowHandle,
    // Additional configuration...
};
```

### Dynamic Region Selection

Allowing users to interactively select a screen region to capture:

```csharp
// Create a form with transparent background
var selectionForm = new Form
{
    FormBorderStyle = FormBorderStyle.None,
    WindowState = FormWindowState.Maximized,
    Opacity = 0.3,
    BackColor = Color.Black
};

// Add mouse event handlers to track selection rectangle
// Once selection is complete

// Configure capture with the selected region
VideoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
{
    Left = selection.Left,
    Top = selection.Top,
    Width = selection.Width,
    Height = selection.Height,
    // Additional configuration...
};
```

## Troubleshooting Common Issues

### Blank or Black Capture

If the captured content appears blank or black:

- Verify that you have appropriate permissions for the window or screen
- Check if the application uses hardware acceleration that might conflict with capture
- Try alternate DirectX versions (9 vs 11/12)
- For protected content (like DRM video), capture may be blocked by security mechanisms

### Performance Issues

If experiencing slow or stuttering capture:

- Reduce capture resolution and/or frame rate
- Use DirectX 11/12 instead of DirectX 9 when available
- Close unnecessary background applications
- Verify that hardware acceleration is enabled when applicable

## Conclusion

Screen capture functionality enables developers to create powerful applications for demonstration, education, support, and entertainment purposes. The Video Capture SDK .Net provides a robust framework for implementing this functionality with minimal development effort.

By leveraging the appropriate configuration options for your specific requirements, you can implement high-performance screen capture features in your .NET applications.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.