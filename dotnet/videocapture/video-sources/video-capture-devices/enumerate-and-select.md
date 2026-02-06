---
title: Manage Video Capture Devices in .NET Applications
description: Detect, enumerate, and configure video capture devices in .NET with code examples for listing devices, formats, and frame rates.
---

# Working with Video Capture Devices in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [VideoCaptureCore](#){ .md-button }

## Introduction to Video Device Management

The Video Capture SDK .Net provides robust support for any video capture device recognized by your operating system. This guide demonstrates how to discover available devices, inspect their capabilities, and integrate them into your applications.

## Enumerating Available Video Capture Devices

Before you can use a capture device, you need to identify which ones are connected to the system. The following code examples show how to retrieve a list of available devices and display them in a user interface component:

=== "VideoCaptureCore"

    
    ```csharp
    // Iterate through all available video capture devices connected to the system
    foreach (var device in VideoCapture1.Video_CaptureDevices())
    {
        // Add each device name to a dropdown selection control
        cbVideoInputDevice.Items.Add(device.Name);
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Asynchronously retrieve all video sources using the shared DeviceEnumerator
    var devices = DeviceEnumerator.Shared.VideoSourcesAsync();
    
    // Iterate through each available device
    foreach (var device in await devices)
    {
        // Add the device's friendly name to the dropdown selection control
        cbVideoInputDevice.Items.Add(device.DisplayName);
    }
    ```
    


## Discovering Video Format Capabilities

After identifying a capture device, you can examine its supported video formats and frame rates. This allows you to offer users appropriate configuration options:

=== "VideoCaptureCore"

    
    ```csharp
    // Locate a specific device by its display name
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Iterate through all video formats supported by this device
    foreach (var format in deviceItem.VideoFormats)
    {
        // Add each format to the format selection dropdown
        cbVideoInputFormat.Items.Add(format);
    
        // For each format, iterate through its supported frame rates
        foreach (var frameRate in format.FrameRates)
        {
            // Add each frame rate option to the frame rate selection dropdown
            cbVideoInputFrameRate.Items.Add(frameRate.ToString(CultureInfo.CurrentCulture));
        }
    }
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Locate a specific device by its display name
    var deviceItem = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
    
    // Iterate through all video formats supported by this device
    foreach (var format in deviceItem.VideoFormats)
    {
        // Add the format's display name to the format selection dropdown
        cbVideoInputFormat.Items.Add(format.Name);
    
        // For each format, retrieve and display available frame rates
        foreach (var frameRate in format.FrameRateList)
        {
            // Add each frame rate value to the selection dropdown
            cbVideoInputFrameRate.Items.Add(frameRate.ToString());
        }
    }
    ```
    


## Configuring and Activating a Video Capture Device

Once you've selected a device and identified your preferred format settings, you can initialize the capture source with these parameters:

=== "VideoCaptureCore"

    
    ```csharp
    // Find the selected device in the device list
    var deviceItem = VideoCapture1.Video_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
    
    // Create a new video capture source using the selected device
    VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(deviceItem.Name);
    
    // Configure the default video format from the first available option
    VideoCapture1.Video_CaptureDevice.Format = deviceItem.VideoFormats[0].ToString();
    
    // Set the default frame rate from the first available option for the selected format
    VideoCapture1.Video_CaptureDevice.FrameRate = deviceItem.VideoFormats[0].FrameRates[0];
    
    // Note: After this setup, use VideoPreview or VideoCapture mode to start streaming
    ```
    

=== "VideoCaptureCoreX"

    
    ```cs
    // Initialize the video source settings variable
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;
    
    // Find the selected device by its display name
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault(x => x.DisplayName == deviceName);
    if (device != null)
    {
        // Locate the selected format by name
        var formatItem = device.VideoFormats.FirstOrDefault(x => x.Name == format);
        if (formatItem != null)
        {
            // Create configuration settings using the selected device
            videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
            {
                // Convert the format representation to the required Format object
                Format = formatItem.ToFormat()
            };
    
            // Set the desired frame rate from the dropdown selection
            videoSourceSettings.Format.FrameRate = new VideoFrameRate(Convert.ToDouble(cbVideoInputFrameRate.Text));
        }
    }
    
    // Apply the configured settings to the video capture component
    VideoCapture1.Video_Source = videoSourceSettings;
    ```
    


## Additional Resources and Code Examples

For more advanced usage scenarios and complete implementation examples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) containing comprehensive demonstration projects.

## Troubleshooting Device Detection

If your application cannot detect expected devices, consider these common issues:

1. Ensure the device is properly connected and powered on
2. Verify that device drivers are correctly installed
3. Check that no other application is currently using the device
4. Restart the application after connecting new devices
