---
title: NDI Integration for Video Capture in .NET
description: Implement NDI video sources in .NET SDK with complete guide to enumerate, connect, and capture high-quality video from NDI cameras in C#.
---

# Implementing NDI Video Sources in .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction to NDI Technology

Network Device Interface (NDI) is a high-performance standard for IP-based production workflows, developed by NewTek. It allows video-compatible products to communicate, deliver, and receive broadcast-quality video over a standard network connection with low latency.

Our SDK provides robust support for NDI sources, enabling your .NET applications to seamlessly integrate with NDI cameras and NDI-enabled software. This makes it ideal for live production environments, streaming applications, video conferencing solutions, and any system requiring high-quality network video integration.

### Prerequisites for NDI Integration

Before implementing NDI functionality in your application, you'll need to install one of the following:

- [NDI SDK](https://ndi.video/for-developers/ndi-sdk/#download) - Recommended for developers building professional applications
- NDI Tools - Sufficient for basic testing and development

These tools provide the necessary runtime components required for NDI communication. After installation, your system will be able to discover and connect to NDI sources on your network.

## Discovering NDI Sources on Your Network

The first step in working with NDI is to enumerate available sources. Our SDK makes this process straightforward with dedicated methods to scan your network for NDI-enabled devices and applications.

### Enumerating Available NDI Sources

=== "VideoCaptureCore"

    
    ```cs
    var lst = await VideoCapture1.IP_Camera_NDI_ListSourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var lst = await DeviceEnumerator.Shared.NDISourcesAsync();
    foreach (var uri in lst)
    {
        cbIPCameraURL.Items.Add(uri.URL);
    }
    ```
    


The asynchronous enumeration methods scan your network and return a list of available NDI sources. Each source has a unique identifier that you'll use to establish a connection. The enumeration process typically takes a few seconds, depending on network conditions and the number of available sources.

## Connecting to NDI Sources

Once you've identified the NDI sources on your network, the next step is to establish a connection. This involves creating the appropriate settings object and configuring it for your specific requirements.

### Configuring NDI Source Settings

=== "VideoCaptureCore"

    
    ```cs
    // Create an IP camera source settings object
    settings = new IPCameraSourceSettings
    {
        URL = new Uri("NDI source URL")
    };
    
    // Set the source type to NDI
    settings.Type = IPSourceEngine.NDI;
    
    // Enable or disable audio capture
    settings.AudioCapture = false; 
    
    // Set login information if needed
    settings.Login = "username";
    settings.Password = "password";
    
    // Set the source to the VideoCaptureCore object
    VideoCapture1.IP_Camera_Source = settings;
    ```
    
    After setting up the source, you'll need to use the `IPPreview` or `IPCapture` mode to preview or capture video from the device.
    

=== "VideoCaptureCoreX"

    
    In VideoCaptureCoreX, you have two options for creating NDI source settings:
    
    **Option 1: Using the NDI source URL**
    
    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), null, "NDI URL");
    ```
    
    **Option 2: Using the NDI source name**
    
    ```cs
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), cbIPURL.Text, null);
    ```
    
    Finally, set the source to the VideoCaptureCoreX object:
    
    ```cs
    VideoCapture1.Video_Source = ndiSettings;
    ```  
    


## Advanced NDI Configuration Options

### Optimizing Performance

When working with NDI sources, performance considerations are important, especially in professional environments. Here are some tips to optimize your NDI implementation:

1. **Network Bandwidth**: Ensure your network has sufficient bandwidth for NDI streams. A typical HD NDI stream requires approximately 100-150 Mbps.

2. **Hardware Acceleration**: Enable hardware acceleration when available to reduce CPU usage and improve performance.

3. **Frame Rate Control**: Consider limiting the frame rate if you don't need the maximum quality, which can reduce network load.

4. **Resolution Settings**: Choose appropriate resolution settings based on your application's needs and available bandwidth.

### Managing Multiple NDI Sources

For applications that need to handle multiple NDI sources simultaneously:

1. Create separate capture instances for each NDI source
2. Implement resource pooling to efficiently manage system resources
3. Consider using a producer/consumer pattern for processing multiple streams
4. Monitor system performance and adjust settings as needed

## Error Handling and Troubleshooting

When implementing NDI functionality, it's important to handle potential issues gracefully:

### Common NDI Connection Issues

1. **Source Not Found**: Verify that the NDI source is active and on the same network
2. **Connection Timeout**: Check network configuration and firewall settings
3. **Authentication Failure**: Ensure credentials are correct if authentication is required
4. **Performance Issues**: Monitor CPU and network usage during capture

### Implementing Robust Error Handling

```cs
try 
{
    // Attempt to connect to NDI source
    var ndiSettings = await NDISourceSettings.CreateAsync(VideoCapture1.GetContext(), null, ndiUrl);
    VideoCapture1.Video_Source = ndiSettings;
}
catch (NDISourceNotFoundException ex)
{
    // Handle source not found
    Log.Error($"NDI source not found: {ex.Message}");
    // Implement retry logic or user notification
}
catch (NDIConnectionException ex)
{
    // Handle connection issues
    Log.Error($"Failed to connect to NDI source: {ex.Message}");
    // Implement fallback strategy
}
```

## Integration with Video Processing Workflows

NDI sources can be seamlessly integrated with other components of your video processing pipeline:

1. **Recording**: Capture NDI streams to various file formats
2. **Live Streaming**: Forward NDI content to streaming services
3. **Video Processing**: Apply filters and effects to NDI sources in real-time
4. **Multi-view Composition**: Combine multiple NDI sources into a single output

## Sample Applications and Code References

To help you get started with NDI implementation, we provide several sample applications that demonstrate different aspects of NDI functionality.

=== "VideoCaptureCore"

    
    - [NDI source capture to MP4 format (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/NDI%20Source)
    - [Main SDK sample (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [NDI and other source sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/IP_Capture)
    - [Main SDK sample (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    

=== "VideoCaptureCoreX"

    
    - [NDI source preview (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/NDI%20Source%20Demo)
    - [NDI (and other sources) preview and capture (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture)
    


## Best Practices for NDI Implementation

To ensure optimal performance and reliability when working with NDI sources:

1. **Perform Regular Source Enumeration**: Network conditions and available sources can change; re-enumerate sources periodically.

2. **Implement Connection Retry Logic**: Network disruptions can occur; implement automatic reconnection attempts.

3. **Monitor Stream Health**: Track frame rates, latency, and connection stability to detect potential issues.

4. **Handle Source Disconnections Gracefully**: Implement event handlers for unexpected disconnections.

5. **Test With Various NDI Sources**: Different NDI implementations may have slight variations; test with various sources.

## Conclusion

NDI technology offers powerful capabilities for network video integration in .NET applications. With our SDK, you can easily incorporate high-quality, low-latency video from NDI sources into your software projects. Whether you're building a live production system, a video conferencing application, or any solution requiring network video, our NDI implementation provides the tools you need for success.

The code samples provided demonstrate the essential patterns for working with NDI sources, from enumeration and connection to capture and processing. By following these patterns and best practices, you can create robust NDI-enabled applications that deliver exceptional performance and reliability.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.