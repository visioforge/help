---
title: Integrating RTSP Cameras in Video Capture SDK .Net
description: Implement and configure RTSP camera streams in .NET with low-latency options, sample code, and best practices for UDP and TCP.
---

# Integrating RTSP Camera Streams in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Setting Up Standard RTSP Camera Sources

Implementing RTSP camera streams in your .NET applications provides flexible access to network cameras and video streams. This powerful capability allows real-time monitoring, surveillance features, and video processing directly within your application.

=== "VideoCaptureCore"

    For additional connection options and alternative protocols, please reference our detailed [IP cameras](index.md) documentation which covers a wide range of camera integration approaches.

=== "VideoCaptureCoreX"

    
    ```cs
    // Create RTSP source settings object
    var rtsp = await RTSPSourceSettings.CreateAsync(new Uri("url"), "login", "password", true /*capture audio?*/);
    
    // Set source to the VideoCaptureCoreX object
    VideoCapture1.Video_Source = rtsp;
    ```
    


## Optimizing for Low-Latency RTSP Streaming

Low latency is critical for many real-time applications including security monitoring, interactive systems, and live broadcasting. Our SDK provides specialized configurations to minimize delay between capture and display.

=== "VideoCaptureCore"

    
    Our SDK includes a dedicated mode specifically engineered for low-latency RTSP streaming. When properly configured, this mode typically achieves latency under 250 milliseconds, making it ideal for time-sensitive applications.
    
    ```cs
    // Create the source settings object.
    var settings = new IPCameraSourceSettings();
    
    // Configure IP address, username, password, etc.
    // ...
    
    // Set RTSP LowLatency mode.
    settings.Type = IPSourceEngine.RTSP_LowLatency;
    
    // Set UDP or TCP mode.
    settings.RTSP_LowLatency_UseUDP = false; // true to use UDP, false to use TCP
    
    // Set source to the VideoCaptureCore object.
    VideoCapture1.IP_Camera_Source = settings;
    ```
    

=== "VideoCaptureCoreX"

    
    **NEW: Ultra-Low Latency Mode (60-120ms)**
    
    VideoCaptureCoreX now includes a dedicated low latency mode that achieves 60-120ms total latency - up to 10x faster than standard mode. Perfect for real-time surveillance, interactive monitoring, and security applications.
    
    ```cs
    // Create RTSP source settings
    var rtsp = await RTSPSourceSettings.CreateAsync(
        new Uri("rtsp://192.168.1.100:554/stream"), 
        "admin", 
        "password", 
        true); // enable audio
    
    // Enable low latency mode - optimizes for minimal delay (60-120ms)
    rtsp.LowLatencyMode = true;
    
    // Set source to VideoCaptureCoreX
    VideoCapture1.Video_Source = rtsp;
    ```
    
    **How It Works:**
    - Sets RTSP jitter buffer to 80ms (vs. default 1000ms)
    - Optimizes internal queue buffering (2 frames max)
    - Disables packet reordering for minimal delay
    - Trade-off: Optimizes speed over stability
    
    **When to Use:**
    - ✓ Real-time surveillance and monitoring
    - ✓ Live security systems
    - ✓ Interactive video applications
    - ✓ Remote control systems
    - ✗ Recording on unstable networks (use default)
    - ✗ Long-term archival capture (use default)
    
    **Advanced Configuration:**
    
    For fine-tuned control, you can manually configure latency parameters:
    
    ```cs
    var rtsp = await RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled);
    
    // Manual low latency configuration
    rtsp.Latency = TimeSpan.FromMilliseconds(50);  // Custom buffer size
    rtsp.BufferMode = RTSPBufferMode.None;          // No jitter buffering
    rtsp.DropOnLatency = false;                     // Don't drop frames
    ```
    


## Implementation Examples and Reference Applications

These sample projects demonstrate practical RTSP implementation patterns and can serve as starting points for your own development. Reviewing these examples will help you understand best practices for RTSP integration.

=== "VideoCaptureCore"

    
    - [Main SDK sample (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [RTSP and other source sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/IP_Capture)
    - [Main SDK sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)

=== "VideoCaptureCoreX"

    - [RTSP source sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)


## Troubleshooting RTSP Connections

When working with RTSP cameras, you may encounter connectivity issues related to network configuration, firewall settings, or authentication. Here are key factors to consider:

- Verify network connectivity between your application and the camera
- Ensure correct authentication credentials are provided
- Check if firewalls are blocking required ports (typically 554 for RTSP)
- Consider using TCP instead of UDP if experiencing packet loss
- Test camera streams with VLC or similar tools to isolate application-specific issues
