---
title: ONVIF IP Camera Integration for Video Capture
description: Learn how to implement ONVIF IP camera integration in your .NET applications. Complete guide covering camera discovery, connection, profile management, streaming, PTZ controls, and troubleshooting for developers.
sidebar_label: ONVIF Cameras
order: 19
---

# ONVIF IP Camera Integration

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoCaptureCore"]

## What is ONVIF?

ONVIF (Open Network Video Interface Forum) is an industry standard protocol that enables seamless interoperability between network video products from different manufacturers. This protocol defines a common interface for IP-based security devices including cameras, NVRs (Network Video Recorders), and access control systems. Using ONVIF-compliant devices allows developers to create applications that work with equipment from multiple vendors without needing custom integrations for each device.

## Benefits of ONVIF Integration

- **Vendor Neutrality**: Build applications that work with cameras from multiple manufacturers
- **Future-Proof Development**: As new ONVIF-compliant cameras enter the market, your application will support them
- **Standardized Communication**: Consistent methods for device discovery, video streaming, and PTZ controls
- **Reduced Development Time**: No need to implement proprietary APIs for each camera brand
- **Advanced Features**: Access to profiles, media streams, events, and device management

## Getting Started with ONVIF

Before connecting to ONVIF cameras, it's important to understand your requirements. Will your application need to discover cameras automatically? Are PTZ controls necessary? Which profiles and streams will you need to access?

### ONVIF Device Discovery

Discovering ONVIF devices on your network is typically the first step in integration. The SDK provides methods to scan the network and identify available ONVIF cameras.

+++ VideoCaptureCore

```cs
// List all ONVIF sources on the network
// The first parameter is a timeout (null = default)
// The second parameter is a network interface IP (null = all interfaces)
var uris = await VideoCapture1.IP_Camera_ONVIF_ListSourcesAsync(null, null);
foreach (var uri in uris)
{
    // Each URI represents an ONVIF device endpoint
    Console.WriteLine($"Found ONVIF device: {uri}");
    cbIPCameraURL.Items.Add(uri);
}
```

+++ VideoCaptureCoreX

```cs
// List all ONVIF sources with a 2-second timeout
// The second parameter is a network interface IP (null = all interfaces)
var uris = await DeviceEnumerator.Shared.ONVIF_ListSourcesAsync(TimeSpan.FromSeconds(2), null);
foreach (var uri in uris)
{
    // Each URI represents an ONVIF device endpoint
    Console.WriteLine($"Found ONVIF device: {uri}");
    cbIPCameraURL.Items.Add(uri);
}
```

+++

### Connecting to ONVIF Cameras

Once you've discovered your ONVIF cameras or have their service endpoints, you can establish a connection to access their features and capabilities.

+++ VideoCaptureCore

```cs
// Create an ONVIF control object
var onvifControl = new ONVIFControl();

// Connect to the camera using its device service endpoint
// Don't forget to provide valid credentials
var result = await onvifControl.ConnectAsync("http://192.168.1.2/onvif/device_service", "username", "password");
if (!result)
{
    onvifControl = null;
    Console.WriteLine("Unable to connect to the ONVIF camera.");
    return;
}

Console.WriteLine("Successfully connected to ONVIF camera");
```

+++ VideoCaptureCoreX

```cs
// Create an ONVIF device object
var onvifDevice = new ONVIFDeviceX();

// Connect to the camera using its device service endpoint
// Don't forget to provide valid credentials
var result = await onvifDevice.ConnectAsync("http://192.168.1.2/onvif/device_service", "username", "password");
if (!result)
{
    onvifDevice = null;
    Console.WriteLine("Unable to connect to the ONVIF camera.");
    return;
}

Console.WriteLine("Successfully connected to ONVIF camera");
```

+++

## Retrieving Camera Information and Capabilities

After establishing a connection, you can query the camera for its information and capabilities. This is useful for identifying the camera model, firmware version, and supported features.

+++ VideoCaptureCore

```cs
// Get basic device information
var deviceInfo = await onvifControl.GetDeviceInformationAsync();
if (deviceInfo != null)
{
    Console.WriteLine($"Model: {deviceInfo.Model}");
    Console.WriteLine($"Firmware Version: {deviceInfo.Firmware}");
    Console.WriteLine($"Serial Number: {deviceInfo.SerialNumber}");
}
```

+++ VideoCaptureCoreX

```cs
// Get basic device information
if (onvifDevice.IsConnected())
{
    Console.WriteLine($"Model: {onvifDevice.Model}");
    Console.WriteLine($"Firmware Version: {onvifDevice.Firmware}");
    Console.WriteLine($"Serial Number: {onvifDevice.SerialNumber}");
    Console.WriteLine($"Hardware ID: {onvifDevice.Hardware}");
}
```

+++

## Working with Media Profiles

ONVIF devices organize their media configurations into profiles. A profile typically includes video and audio encoder configurations, PTZ settings, and other parameters. Cameras often have multiple profiles for different quality levels or use cases.

+++ VideoCaptureCore

```cs
// Get all available profiles
var profiles = await onvifControl.GetProfilesAsync();
if (profiles != null && profiles.Length > 0)
{
    Console.WriteLine($"Found {profiles.Length} media profiles:");
    
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name} (Token: {profile.Token})");
        Console.WriteLine($"   - Video Encoder: {profile.VideoEncoderConfiguration?.Encoding}");
        Console.WriteLine($"   - Resolution: {profile.VideoEncoderConfiguration?.Resolution.Width}x{profile.VideoEncoderConfiguration?.Resolution.Height}");
        Console.WriteLine($"   - Framerate: {profile.VideoEncoderConfiguration?.RateControl?.FrameRateLimit}");
        Console.WriteLine($"   - Has PTZ: {profile.PTZConfiguration != null}");
    }
}
```

+++ VideoCaptureCoreX

```cs
// Get all available profiles
var profiles = await onvifDevice.GetProfilesAsync();
if (profiles != null && profiles.Length > 0)
{
    Console.WriteLine($"Found {profiles.Length} media profiles:");
    
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name})");
        
        // Get detailed profile information
        Console.WriteLine($"   - Video Encoder: {profile.VideoEncoderName}");
        Console.WriteLine($"   - Resolution: {profile.VideoEncoderWidth}x{profile.VideoEncoderHeight}");
        Console.WriteLine($"   - Framerate: {profile.VideoEncoderFrameRate}");
        Console.WriteLine($"   - Has PTZ: {profile.PTZConfiguration != null}");
    }
}
```

+++

## Obtaining RTSP Stream URLs

To capture video from an ONVIF camera, you'll need to obtain the RTSP stream URL for the desired profile.

+++ VideoCaptureCore

```cs
// Get streaming URL for the first profile (index 0)
var url = await onvifControl.GetVideoURLAsync(0);
if (!string.IsNullOrEmpty(url))
{
    Console.WriteLine($"RTSP stream URL: {url}");
}
```

+++ VideoCaptureCoreX

```cs
// Get RTSP URL
var uri = onvifDevice.GetVideoURL(0);
if (!string.IsNullOrEmpty(uri))
{
    Console.WriteLine($"RTSP stream URL: {uri}");
}
```

+++

## PTZ (Pan-Tilt-Zoom) Controls

Many ONVIF cameras support PTZ functionality, allowing you to programmatically control the camera's position and zoom level.

+++ VideoCaptureCore

```cs
// First, get PTZ configuration ranges
var ranges = await onvifControl.PTZ_GetRangesAsync();
if (ranges != null)
{
    Console.WriteLine($"X Range: {ranges.MinX} to {ranges.MaxX}");
    Console.WriteLine($"Y Range: {ranges.MinY} to {ranges.MaxY}");
    Console.WriteLine($"Zoom Range: {ranges.ZoomMin} to {ranges.ZoomMax}");
}

// Go to absolute position
// Parameters: Pan position, Tilt position, Zoom position
await onvifControl.PTZ_SetAbsoluteAsync(0.25, 0.5, 0.1);

// Go to home position
await onvifControl.PTZ_SetHomeAsync();
```

+++ VideoCaptureCoreX

```cs
// Go to absolute position
// Parameters: Pan position, Tilt position, Zoom position
onvifDevice.PTZ_AbsoluteMove(0.25, 0.5, 0.1);

// Go to home position
onvifDevice.PTZ_GoHome();
```

+++

## Troubleshooting ONVIF Connections

When working with ONVIF cameras, you may encounter various issues. Here are some common problems and solutions:

### Connection Issues

- Verify the camera is on the same network and reachable (ping test)
- Check that username and password are correct
- Ensure the ONVIF service endpoint URL is correct
- Some cameras require specific ONVIF ports to be open

### Stream Problems

- Check if the RTSP stream URL is correct
- Verify network bandwidth is sufficient for the selected profile
- Try a lower resolution profile if experiencing lag
- Some networks may block RTSP traffic

### PTZ Control Failures

- Verify the camera actually supports PTZ (not all IP cameras do)
- Check if the selected profile has PTZ configuration
- Some cameras have limited PTZ ranges or capabilities

## Best Practices

1. **Cache Device Information**: Store discovered devices and their capabilities to reduce startup time
2. **Handle Authentication Securely**: Never store credentials in plain text
3. **Implement Timeouts**: Network operations should have reasonable timeouts
4. **Provide Fallbacks**: If an ONVIF feature isn't available, have alternative methods
5. **Profile Selection**: Choose the appropriate profile based on your application's needs

## Conclusion

ONVIF integration provides a powerful and standardized way to interact with IP cameras in your applications. By following the guidelines and code examples in this documentation, you can build robust camera management solutions that work with a wide range of devices without vendor lock-in.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
