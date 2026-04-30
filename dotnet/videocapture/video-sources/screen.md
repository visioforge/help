---
title: Screen Capture Source in C# .NET — Full Desktop or Region
description: Capture full screen, specific windows, or custom regions using VisioForge Video Capture SDK with DirectX 11/12 and Windows Graphics Capture APIs.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - Webcam
  - IP Camera
  - Screen Capture
  - C#
primary_api_classes:
  - ScreenCaptureD3D11SourceSettings
  - ScreenCaptureSourceSettings
  - ScreenCaptureDX9SourceSettings

---

# Screen Capture Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

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
    
    - `GrabMouseCursor`: Enable or disable cursor visibility in the captured content
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
    
         // Set the left edge of the screen area (absolute pixel X)
        Left = 0,
    
        // Set the top edge of the screen area (absolute pixel Y)
        Top = 0, 
    
        // Set the right edge (absolute pixel X, not width)
        Right = 640, 
    
        // Set the bottom edge (absolute pixel Y, not height)
        Bottom = 480, 
    
        // Set the display index
        DisplayIndex = 0, 
    
        // Set the frame rate
        FrameRate = new VideoFrameRate(25), 
    
         // Set to true to capture the mouse cursor
        GrabMouseCursor = true
    };
    ```
    
    When `FullScreen` is set to `true`, the `Left`, `Top`, `Right`, and `Bottom` properties are ignored, and the entire screen specified by `DisplayIndex` is captured. Note the rectangle is specified as absolute pixel corner coordinates — `Right` and `Bottom` are the far-corner coordinates, not the width/height of the region.
    
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
        GrabMouseCursor = true
    };
    ```
    
    The `windowHandle` parameter must be a valid HWND for the target window. Get it via `FindWindow` (P/Invoke):

    ```csharp
    using System.Runtime.InteropServices;

    [DllImport("user32.dll", CharSet = CharSet.Unicode)]
    private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

    // Pass null for lpClassName to match by title only.
    IntPtr windowHandle = FindWindow(null, "Untitled - Notepad");
    if (windowHandle == IntPtr.Zero) throw new InvalidOperationException("Target window not found.");
    ```
    

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

Enumerate top-level windows with `EnumWindows` + `GetWindowText` and let the user pick one:

```csharp
using System.Runtime.InteropServices;
using System.Text;

private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

[DllImport("user32.dll")]
private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

[DllImport("user32.dll", CharSet = CharSet.Unicode)]
private static extern int GetWindowText(IntPtr hWnd, StringBuilder buf, int nMaxCount);

[DllImport("user32.dll")]
private static extern bool IsWindowVisible(IntPtr hWnd);

public static Dictionary<IntPtr, string> GetOpenWindows()
{
    var result = new Dictionary<IntPtr, string>();
    EnumWindows((hWnd, _) =>
    {
        if (!IsWindowVisible(hWnd)) return true;
        var buf = new StringBuilder(256);
        if (GetWindowText(hWnd, buf, buf.Capacity) > 0)
        {
            result[hWnd] = buf.ToString();
        }
        return true;
    }, IntPtr.Zero);
    return result;
}

// Present result to the user, then:
IntPtr selectedWindowHandle = /* chosen entry.Key */;

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
    Right = selection.Right,   // absolute pixel X, not width
    Bottom = selection.Bottom, // absolute pixel Y, not height
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

## Frequently Asked Questions

### What is the difference between DirectX 9 and DirectX 11/12 screen capture?

DirectX 11/12 uses GPU-accelerated Desktop Duplication API or Windows Graphics Capture (WGC), delivering higher frame rates and lower CPU usage than DirectX 9's GDI-based approach. DirectX 9 is only needed for legacy systems running Windows 7 or earlier. For all modern Windows 10/11 applications, use `ScreenCaptureD3D11SourceSettings` with `D3D11ScreenCaptureAPI.WGC` for the best performance.

### Can I capture a specific window instead of the full screen in C#?

Yes. Set the `WindowHandle` property on `ScreenCaptureSourceSettings` (VideoCaptureCore) or `ScreenCaptureD3D11SourceSettings` (VideoCaptureCoreX) to the target window's handle. Obtain the handle using `FindWindow`, UI Automation, or by enumerating open windows. DirectX 11 with WGC provides the most reliable window capture, including for hardware-accelerated applications.

### How do I capture the mouse cursor during screen recording?

Set `GrabMouseCursor = true` in `ScreenCaptureSourceSettings` (VideoCaptureCore) or `CaptureCursor = true` in `ScreenCaptureD3D11SourceSettings` (VideoCaptureCoreX). The cursor is included in the captured frame data at its current position. Disable this property when recording tutorials where cursor rendering will be added in post-production.

### Does the screen capture SDK work on multiple monitors?

Yes. Use `DisplayIndex` (VideoCaptureCore) or `MonitorIndex` (VideoCaptureCoreX) to select which monitor to capture. Enumerate available monitors with `System.Windows.Forms.Screen.AllScreens` and present the list to the user. Each monitor is captured independently — to record all monitors simultaneously, create separate capture instances for each display.

## See Also

- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — complete Visual Basic guide with full screen, region capture, and audio recording
- [Screen Capture to MP4 Tutorial](../video-tutorials/screen-capture-mp4.md) — C# tutorial for recording desktop to MP4 with video walkthrough
- [Save Webcam Video in C#](../guides/save-webcam-video.md) — capture from webcam instead of screen
- [IP Camera Capture](ip-cameras/index.md) — record from network cameras using RTSP
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads