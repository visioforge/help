---
title: ".NET Blackmagic Decklink Integration Guide"
description: Implement professional video capture in .NET with Blackmagic Decklink devices using code examples for broadcast-quality applications.
---

# Integrating Blackmagic Decklink Devices with .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## What Are Decklink Devices?

Blackmagic Design's Decklink devices represent industry-standard capture and playback cards designed for professional video production environments. These high-performance hardware solutions deliver exceptional capabilities for developers integrating video capture functionality into .NET applications.

Decklink cards are renowned for their superior technical specifications:

- Support for high-resolution video formats including 4K, 8K, and HD
- Multiple input/output connection options (SDI, HDMI) for flexible integration
- Low-latency performance crucial for real-time broadcasting applications
- Cross-compatibility with major video production software platforms
- Developer-friendly APIs that integrate seamlessly with various programming languages

For .NET developers building professional video applications, Decklink integration provides a reliable foundation for handling broadcast-quality video signal processing.

## Programmatic Device Detection

The first step in implementing Decklink functionality is proper device enumeration. The following code samples demonstrate how to detect available Decklink hardware in your .NET application.

### Enumerating Available Devices

=== "VideoCaptureCore"

    
    ```csharp
    foreach (var device in (await VideoCapture1.Decklink_CaptureDevicesAsync()))
    {   
        cbDecklinkCaptureDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    var videoCaptureDevices = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();
    if (videoCaptureDevices.Length > 0)
    {
        foreach (var item in videoCaptureDevices)
        {
            cbVideoInput.Items.Add(item.Name);
        }
    
        cbVideoInput.SelectedIndex = 0;
    }
    ```
    
    When working with VideoCaptureCoreX, you'll also need to enumerate audio sources separately:
    
    ```cs
    var audioCaptureDevices = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();
    if (audioCaptureDevices.Length > 0)
    {
        foreach (var item in audioCaptureDevices)
        {
            cbAudioInput.Items.Add(item.Name);
        }
    
        cbAudioInput.SelectedIndex = 0;
    }
    ```
    


## Working with Video Formats and Frame Rates

After enumerating devices, the next step involves determining available video formats and frame rates. Decklink cards support numerous professional formats, but the specific options depend on the connected input source.

### Retrieving Format Information

=== "VideoCaptureCore"

    
    ```csharp
    // Enumerate and filter by device name
    var deviceItem = (await VideoCapture1.Decklink_CaptureDevicesAsync()).Find(device => device.Name == cbDecklinkCaptureDevice.Text);
    if (deviceItem != null)
    {
        // Read video formats and add to combobox
        foreach (var format in (await deviceItem.GetVideoFormatsAsync()))
        {
            cbDecklinkCaptureVideoFormat.Items.Add(format.Name);
        }
    
        // If format does not exist, add a message
        if (cbDecklinkCaptureVideoFormat.Items.Count == 0)
        {
            cbDecklinkCaptureVideoFormat.Items.Add("No input connected");
        }
    }
    ```
    
    This approach provides valuable diagnostic information. If no formats are returned, it typically indicates that no active input is connected to the Decklink device. Implementing this check helps users troubleshoot connection issues directly from your application interface.
    

=== "VideoCaptureCoreX"

    
    With VideoCaptureCoreX, you'll work with predefined modes from the DecklinkMode enumeration:
    
    ```cs
    var decklinkModes = Enum.GetValues(typeof(DecklinkMode));
    foreach (var item in decklinkModes)
    {
        cbVideoMode.Items.Add(item.ToString());
    }
    ```
    
    This approach uses standardized mode settings that must be configured on your Decklink hardware.
    


## Configuring Decklink as Video Source

Once you've detected devices and identified available formats, you need to configure the Decklink hardware as your capture source. This critical step establishes the connection between hardware and software.

### Setting Source Parameters

=== "VideoCaptureCore"

    
    ```csharp
    VideoCapture1.Decklink_Source = new DecklinkSourceSettings
    {
        Name = cbDecklinkCaptureDevice.Text,
        VideoFormat = cbDecklinkCaptureVideoFormat.Text
    };
    ```
    
    When using VideoCaptureCore, you'll need to specify either `DecklinkSourcePreview` or `DecklinkSourceCapture` mode depending on your application's requirements:
    
    - `DecklinkSourcePreview`: Optimized for real-time monitoring with minimal processing
    - `DecklinkSourceCapture`: Configured for high-quality recording with enhanced buffering
    

=== "VideoCaptureCoreX"

    
    VideoCaptureCoreX requires separate configuration of video and audio sources:
    
    ```cs
    // Create settings for the video source
    DecklinkVideoSourceSettings videoSourceSettings = null;
    
    // Device name from the combo box
    var deviceName = cbVideoInput.Text;
    
    // Mode from the combobox
    var mode = cbVideoMode.Text;
    if (!string.IsNullOrEmpty(deviceName) && !string.IsNullOrEmpty(mode))
    {
        // Find device
        var device = (await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Create video source settings using device and mode
            videoSourceSettings = new DecklinkVideoSourceSettings(device)
            {
                Mode = (DecklinkMode)Enum.Parse(typeof(DecklinkMode), mode, true)
            };
        }
    }
    
    // Set the video source to the VideoCaptureCoreX object
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    
    For audio configuration:
    
    ```cs
    // Create settings for the audio source
    DecklinkAudioSourceSettings audioSourceSettings = null;
    
    // Device name from the combobox
    deviceName = cbAudioInput.Text;
    if (!string.IsNullOrEmpty(deviceName))
    {
        // Find device
        var device = (await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync()).FirstOrDefault(x => x.Name == deviceName);
        if (device != null)
        {
            // Create settings for the audio source using device
            audioSourceSettings = new DecklinkAudioSourceSettings(device);
        }
    }
    
    // Set the audio source to the VideoCaptureCoreX object
    VideoCapture1.Audio_Source = audioSourceSettings;
    ```
    
    This separation offers greater flexibility for advanced scenarios where you might need to process video and audio independently.
    


## Performance Considerations

When implementing Decklink capture in production environments, consider these performance factors:

1. **Buffer management:** Professional video formats require substantial memory allocation, especially for 4K+ resolutions
2. **CPU utilization:** Real-time encoding of Decklink streams can be processor-intensive
3. **Disk I/O:** When capturing to storage, ensure your write speeds support the data rate of your selected format
4. **Memory bandwidth:** High-resolution uncompressed streams demand significant system resources

Implementing proper error handling around device connection and format detection will improve your application's resilience in production environments.

## Sample Applications and Implementation Examples

Examining working examples provides valuable insights into effective implementation patterns. The SDK includes numerous sample applications demonstrating Decklink integration.

### Reference Applications

=== "VideoCaptureCore"

    
    - [Main sample with DeckLink input, video/audio processing and many output formats (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Main_Demo)
    - [Main sample for WinForms](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Main%20Demo)
    - [Simple sample with DeckLink input and many output formats (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WinForms/CSharp/Decklink%20Demo)
    

=== "VideoCaptureCoreX"

    
    - [Video preview and capture to MP4 or WebM file (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Decklink%20Demo%20X)
    


## Best Practices for Integration

Based on extensive field testing and customer implementation experiences, we recommend these best practices:

1. **Always verify device connectivity:** Check for available formats to confirm proper signal connection
2. **Implement graceful fallbacks:** Provide meaningful error messages when expected devices are unavailable
3. **Test with multiple frame rates:** Some applications may behave differently with varied input formats
4. **Manage memory effectively:** High-resolution capture requires proper resource management
5. **Monitor CPU usage:** Encoding operations can be processor-intensive during capture
6. **Provide format details to users:** Give clear information about detected formats and connection status

These recommendations help ensure robust implementations that perform reliably across different hardware configurations and operating conditions.

## Conclusion

Integrating Blackmagic Decklink devices with .NET applications provides powerful capabilities for professional video capture scenarios. By following the implementation patterns outlined in this guide, developers can create stable, high-performance applications that leverage the full capabilities of Decklink hardware.

The Video Capture SDK offers a streamlined approach to working with these professional devices, abstracting much of the complexity while providing the flexibility needed for advanced customization.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to access additional code samples and implementation resources.