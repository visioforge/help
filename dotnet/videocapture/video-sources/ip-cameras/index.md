---
title: IP Camera Integration for .NET Video Applications
description: Implement IP cameras, RTSP/RTMP streams, ONVIF devices, and network video sources in .NET with code samples and best practices.
sidebar_label: IP Cameras & Network Sources
order: 19
---

# Complete Guide to IP Cameras and Network Sources Integration

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction to Network Video Sources

Modern video applications often require integration with various network video sources. The Video Capture SDK for .NET provides robust support for diverse IP camera types and network video streams, allowing developers to easily incorporate live network video into .NET applications.

This comprehensive guide covers all supported network sources and provides clear implementation examples for both VideoCaptureCore and VideoCaptureCoreX frameworks.

## Supported IP Camera Types and Network Sources

The SDK offers extensive compatibility with various network video sources, including:

* [ONVIF-compliant cameras](onvif.md) - Industry standard for IP-based security products
* [RTSP cameras](rtsp.md) - Real-Time Streaming Protocol cameras
* HTTP MJPEG cameras - Motion JPEG streaming over HTTP
* UDP cameras and streams - User Datagram Protocol-based streams
* [NDI cameras](ndi.md) - Network Device Interface technology cameras
* SRT servers and cameras - Secure Reliable Transport protocol
* VNC servers - Virtual Network Computing for screen capture
* RTMP streams - Real-Time Messaging Protocol sources
* HLS streams - HTTP Live Streaming sources
* HTTP video sources - Various HTTP-based video streams

Each protocol offers specific advantages depending on your application requirements, from low-latency needs to high-quality video transmission.

## Universal Source Implementation for Network Protocols

Our SDK provides a universal approach to handling most network video sources including RTSP, RTMP, HTTP, and others. This flexibility allows developers to focus on application logic rather than protocol-specific implementation details.

=== "VideoCaptureCore"

    
    ### Implementing Universal Source in VideoCaptureCore
    
    For VideoCaptureCore applications, you can use the IPCameraSourceSettings class to define your network video source:
    
    ```cs
    // Create and configure the network source
    VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
    {
        URL = "rtsp://192.168.1.100:554/stream1", // The stream URL
        Login = "admin", // Optional authentication credentials
        Password = "password123",
        AudioCapture = true, // Set to true to include audio from the source
        Type = VFIPSource.Auto_VLC // The processing engine to use
    };
    ```
    
    #### Available Engine Types
    
    The SDK supports multiple underlying engines for processing network streams, providing flexibility for different scenarios:
    
    * `Auto_VLC` - Uses the VLC engine, offering broad protocol support and compatibility
    * `Auto_FFMPEG` - Uses the FFMPEG engine, providing extensive format support and customization
    * `Auto_LAV` - Uses the LAV engine, optimized for Windows environments
    
    ### Customizing FFMPEG Settings for Advanced Users
    
    The SDK allows fine-grained control over FFMPEG settings when using the FFMPEG engine. This provides advanced users with extensive customization options:
    
    ```cs
    // Configure custom FFMPEG parameters
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("rtsp_transport", "tcp"); // Force TCP transport
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("timeout", "3000000"); // Set timeout in microseconds
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("buffer_size", "1000000"); // Adjust buffer size
    VideoCapture1.IP_Camera_Source.FFMPEG_CustomOptions.Add("max_delay", "500000"); // Set maximum allowed delay
    ```
    
    These parameters are passed directly to the avformat_open_input function in FFMPEG, providing deep customization options for network streaming performance.
    

=== "VideoCaptureCoreX"

    
    ### Implementing Universal Source in VideoCaptureCoreX
    
    For VideoCaptureCoreX applications, the approach uses the modern async patterns:
    
    ```cs
    // Prepare the source URL with authentication if needed
    var uri = new Uri("rtsp://192.168.1.100:554/stream1");
    if (!string.IsNullOrEmpty(login) && !string.IsNullOrEmpty(password))
    {
        uri = new UriBuilder(uri) { UserName = login, Password = password }.Uri;
    }
    
    // Create the universal source with desired settings
    var source = await UniversalSourceSettings.CreateAsync(
        uri,
        renderAudio: true, // Include audio stream
    );
    
    // Apply the source to the capture object
    VideoCapture1.Video_Source = source;
    ```
    
    The VideoCaptureCoreX approach provides a more modern, task-based asynchronous pattern, making it ideal for responsive UI applications.
    


## Low-Latency MJPEG Implementation

For applications requiring minimal latency, such as security monitoring or real-time control systems, the SDK offers a specialized low-latency MJPEG implementation with typical latency under 100ms.

=== "VideoCaptureCore"

    
    ### Configuring Low-Latency MJPEG in VideoCaptureCore
    
    ```cs
    // Create settings object
    var settings = new IPCameraSourceSettings
    {
        URL = "http://192.168.1.100/video.mjpg",
        Login = "admin",
        Password = "pass123",
        // Use the dedicated low-latency MJPEG engine
        Type = IPSourceEngine.HTTP_MJPEG_LowLatency
    };
    
    // Apply settings to the VideoCaptureCore object
    VideoCapture1.IP_Camera_Source = settings;
    ```
    
    This specialized mode bypasses unnecessary processing to minimize latency, making it ideal for time-sensitive applications.
    

=== "VideoCaptureCoreX"

    
    ### Configuring Low-Latency MJPEG in VideoCaptureCoreX
    
    ```cs
    // Create specialized HTTP MJPEG source settings
    var mjpeg = await HTTPMJPEGSourceSettings.CreateAsync(
        new Uri("http://192.168.1.100/video.mjpg"),
        "admin", // Username
        "pass123"
    );
    
    // Apply settings to the VideoCaptureCoreX object
    VideoCapture1.Video_Source = mjpeg;
    ```
    
    The low-latency MJPEG implementation is particularly useful for surveillance systems, remote monitoring, and industrial applications where minimizing delay is critical.
    


## Secure Reliable Transport (SRT) Implementation

SRT is a modern protocol designed for reliable video streaming over unpredictable networks. It's especially valuable for maintaining quality in challenging network conditions.

=== "VideoCaptureCoreX"

    
    ### Implementing SRT Source in VideoCaptureCoreX
    
    ```cs
    // Create SRT source settings with the server URL
    var srt = await SRTSourceSettings.CreateAsync("srt://streaming-server.example.com:7001");
    
    // Apply the SRT source to the capture object
    VideoCapture1.Video_Source = srt;
    ```
    
    SRT provides significant advantages for reliable streaming across challenging networks, offering built-in security, error correction, and congestion control.
    


## Network Disconnect Handling

Robust network source implementations must handle connection interruptions gracefully. The SDK provides built-in mechanisms for detecting and responding to network disconnections.

=== "VideoCaptureCore"

    
    ### Implementing Network Disconnect Handling in VideoCaptureCore
    
    ```cs
    // Enable network disconnect detection
    VideoCapture1.DisconnectEventInterval = TimeSpan.FromSeconds(5); // Check every 5 seconds
    
    // Register the disconnect event handler
    VideoCapture1.OnNetworkSourceDisconnect += VideoCapture1_OnNetworkSourceDisconnect;
    
    // Implement the disconnect event handler
    private void VideoCapture1_OnNetworkSourceDisconnect(object sender, EventArgs e)
    {
        Invoke((Action)(
            async () =>
            {
                await VideoCapture1.StopAsync();
                
                // Notify the user
                MessageBox.Show(this, "Network source disconnected!");
            }));
    }
    ```
    
    Implementing proper disconnect handling improves application reliability and user experience during network fluctuations.
    


## VNC Source Implementation

Virtual Network Computing (VNC) allows capturing remote desktop screens as video sources, useful for screen recording and remote assistance applications.

=== "VideoCaptureCoreX"

    
    ### Implementing VNC Source in VideoCaptureCoreX
    
    ```cs
    // Create VNC source settings object
    var vncSettings = new VNCSourceSettings();
    
    // Configure using host and port
    vncSettings.Host = "remote-server.example.com";
    vncSettings.Port = 5900; // Default VNC port
    
    // Or configure using URI format
    // vncSettings.Uri = "vnc://remote-server.example.com:5900";
    
    // Set authentication credentials
    vncSettings.Password = "secure-password";
    
    // Optional: Configure advanced VNC settings
    vncSettings.EnableCursor = true; // Capture mouse cursor
    vncSettings.CompressionLevel = 5; // 0-9, higher values = more compression
    vncSettings.QualityLevel = 8; // 0-9, higher values = better quality
    vncSettings.UpdateInterval = 100; // Update interval in milliseconds
    
    // Apply the settings to the capture object
    VideoCapture1.Video_Source = vncSettings;
    ```
    
    The VNC source implementation provides a complete solution for remote desktop capture with customizable quality and performance settings.
    
    * [Complete VNC source sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/VNC%20Source%20Demo)
    


## Best Practices for Network Sources

For optimal performance when working with network video sources:

1. **Buffer Management**: Adjust buffering based on source stability and latency requirements
2. **Error Handling**: Implement comprehensive error handling for network interruptions
3. **Authentication**: Always use secure credentials storage for camera authentication
4. **Connection Pooling**: Reuse connections when accessing multiple streams from the same device
5. **Bandwidth Consideration**: Monitor and manage bandwidth consumption, especially with multiple sources

## Conclusion

The Video Capture SDK for .NET provides comprehensive support for IP cameras and network sources, enabling developers to build sophisticated video applications. With support for multiple protocols and flexible configuration options, it accommodates a wide range of use cases from security surveillance to media streaming applications.

For additional implementation examples and advanced usage scenarios, explore our GitHub repository with complete code samples.

---

Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for more comprehensive code samples and demo applications.
