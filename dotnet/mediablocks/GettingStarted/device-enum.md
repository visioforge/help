---
title: Complete Guide to Media Device Enumeration in .NET
description: Enumerate video cameras, audio inputs/outputs, Decklink devices, NDI sources, and GenICam cameras in .NET with practical code examples.
---

# Complete Guide to Media Device Enumeration in .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The Media Blocks SDK provides a powerful and efficient way to discover and work with various media devices in your .NET applications. This guide will walk you through the process of enumerating different types of media devices using the SDK's `DeviceEnumerator` class.

## Introduction to Device Enumeration

Device enumeration is a critical first step when developing applications that interact with media hardware. The `DeviceEnumerator` class provides a centralized way to detect and list all available media devices connected to your system.

The SDK uses a singleton pattern for device enumeration, making it easy to access the functionality from anywhere in your code:

```csharp
// Access the shared DeviceEnumerator instance
var enumerator = DeviceEnumerator.Shared;
```

## Discovering Video Input Devices

### Standard Video Sources

To list all available video input devices (webcams, capture cards, virtual cameras):

```csharp
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

foreach (var device in videoSources)
{
    Debug.WriteLine($"Video device found: {device.Name}");
    // You can access additional properties here if needed
}
```

The `VideoCaptureDeviceInfo` objects returned provide detailed information about each device, including device name, internal identifiers, and API type.

## Working with Audio Devices

### Enumerating Audio Input Sources

To discover microphones and other audio input devices:

```csharp
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

foreach (var device in audioSources)
{
    Debug.WriteLine($"Audio input device found: {device.Name}");
    // Additional device information can be accessed here
}
```

You can also filter audio devices by their API type:

```csharp
// Get only audio sources for a specific API
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound);
```

### Finding Audio Output Devices

For speakers, headphones, and other audio output destinations:

```csharp
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();

foreach (var device in audioOutputs)
{
    Debug.WriteLine($"Audio output device found: {device.Name}");
    // Process device information as needed
}
```

Similar to audio sources, you can filter outputs by API:

```csharp
// Get only audio outputs for a specific API
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound);
```

## Professional Blackmagic Decklink Integration

### Decklink Video Input Sources

For professional video workflows using Blackmagic hardware:

```csharp
var decklinkVideoSources = await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync();

foreach (var device in decklinkVideoSources)
{
    Debug.WriteLine($"Decklink video input: {device.Name}");
    // You can work with specific Decklink properties here
}
```

### Decklink Audio Input Sources

To access audio channels from Decklink devices:

```csharp
var decklinkAudioSources = await DeviceEnumerator.Shared.DecklinkAudioSourcesAsync();

foreach (var device in decklinkAudioSources)
{
    Debug.WriteLine($"Decklink audio input: {device.Name}");
    // Process Decklink audio device information
}
```

### Decklink Video Output Destinations

For sending video to Decklink output devices:

```csharp
var decklinkVideoOutputs = await DeviceEnumerator.Shared.DecklinkVideoSinksAsync();

foreach (var device in decklinkVideoOutputs)
{
    Debug.WriteLine($"Decklink video output: {device.Name}");
    // Access output device properties as needed
}
```

### Decklink Audio Output Destinations

For routing audio to Decklink hardware outputs:

```csharp
var decklinkAudioOutputs = await DeviceEnumerator.Shared.DecklinkAudioSinksAsync();

foreach (var device in decklinkAudioOutputs)
{
    Debug.WriteLine($"Decklink audio output: {device.Name}");
    // Work with audio output configuration here
}
```

## Network Device Integration

### NDI Sources Discovery

To find NDI sources available on your network:

```csharp
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();

foreach (var device in ndiSources)
{
    Debug.WriteLine($"NDI source discovered: {device.Name}");
    // Process NDI-specific properties and information
}
```

### ONVIF Network Camera Discovery

To find IP cameras supporting the ONVIF protocol:

```csharp
// Set a timeout for discovery (2 seconds in this example)
var timeout = TimeSpan.FromSeconds(2);
var onvifDevices = await DeviceEnumerator.Shared.ONVIF_ListSourcesAsync(timeout, null);

foreach (var deviceUri in onvifDevices)
{
    Debug.WriteLine($"ONVIF camera found at: {deviceUri}");
    // Connect to the camera using the discovered URI
}
```

## Industrial Camera Support

### Basler Industrial Cameras

For applications requiring Basler industrial cameras:

```csharp
var baslerCameras = await DeviceEnumerator.Shared.BaslerSourcesAsync();

foreach (var device in baslerCameras)
{
    Debug.WriteLine($"Basler camera detected: {device.Name}");
    // Access Basler-specific camera features
}
```

### Allied Vision Industrial Cameras

To work with Allied Vision cameras in your application:

```csharp
var alliedCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();

foreach (var device in alliedCameras)
{
    Debug.WriteLine($"Allied Vision camera found: {device.Name}");
    // Configure Allied Vision specific parameters
}
```

### Spinnaker SDK Compatible Cameras

For cameras supporting the Spinnaker SDK (Windows only):

```csharp
#if NET_WINDOWS
var spinnakerCameras = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();

foreach (var device in spinnakerCameras)
{
    Debug.WriteLine($"Spinnaker SDK camera: {device.Name}");
    Debug.WriteLine($"Model: {device.Model}, Vendor: {device.Vendor}");
    Debug.WriteLine($"Resolution: {device.WidthMax}x{device.HeightMax}");
    // Work with camera-specific properties
}
#endif
```

### Generic GenICam Standard Cameras

For other industrial cameras supporting the GenICam standard:

```csharp
var genicamCameras = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in genicamCameras)
{
    Debug.WriteLine($"GenICam compatible device: {device.Name}");
    Debug.WriteLine($"Model: {device.Model}, Vendor: {device.Vendor}");
    Debug.WriteLine($"Protocol: {device.Protocol}, Serial: {device.SerialNumber}");
    // Work with standard GenICam features
}
```

## Device Monitoring

The SDK also supports monitoring device connections and disconnections:

```csharp
// Start monitoring for video device changes
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();

// Start monitoring for audio device changes
await DeviceEnumerator.Shared.StartAudioSourceMonitorAsync();
await DeviceEnumerator.Shared.StartAudioSinkMonitorAsync();

// Subscribe to device change events
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) => 
{
    Debug.WriteLine($"New video device connected: {device.Name}");
};

DeviceEnumerator.Shared.OnVideoSourceRemoved += (sender, device) => 
{
    Debug.WriteLine($"Video device disconnected: {device.Name}");
};
```

## Platform-Specific Considerations

### Windows

On Windows, the SDK can detect USB device connection and removal events at the system level:

```csharp
#if NET_WINDOWS
// Subscribe to system-wide device events
DeviceEnumerator.Shared.OnDeviceAdded += (sender, args) => 
{
    // Refresh device lists when new hardware is connected
    RefreshDeviceLists();
};

DeviceEnumerator.Shared.OnDeviceRemoved += (sender, args) => 
{
    // Update UI when hardware is disconnected
    RefreshDeviceLists();
};
#endif
```

By default, Media Foundation device enumeration is disabled to avoid duplication with DirectShow devices. You can enable it if needed:

```csharp
#if NET_WINDOWS
// Enable Media Foundation device enumeration if required
DeviceEnumerator.Shared.IsEnumerateMediaFoundationDevices = true;
#endif
```

### iOS and Android

On mobile platforms, the SDK handles the required permission requests when enumerating devices:

```csharp
#if __IOS__ || __ANDROID__
// This will automatically request camera permissions if needed
var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

// This will automatically request microphone permissions if needed
var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();
#endif
```

## Best Practices for Device Enumeration

When working with device enumeration in production applications:

1. Always handle cases where no devices are found
2. Consider caching device lists when appropriate to improve performance
3. Implement proper exception handling for device access failures
4. Provide clear user feedback when required devices are missing
5. Use the async methods to avoid blocking the UI thread during enumeration
6. Clean up resources by calling `Dispose()` when you're done with the DeviceEnumerator

```csharp
// Proper cleanup when done
DeviceEnumerator.Shared.Dispose();
```
