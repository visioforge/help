---
title: ONVIF IP Camera Integration - Complete Guide
description: Comprehensive ONVIF IP camera integration in .NET covering discovery, connection, PTZ control, recording, streaming, and computer vision.
---

# ONVIF IP Camera Integration - Complete Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Table of Contents

- [ONVIF IP Camera Integration - Complete Guide](#onvif-ip-camera-integration-complete-guide)
  - [Table of Contents](#table-of-contents)
  - [What is ONVIF?](#what-is-onvif)
  - [Benefits of ONVIF Integration](#benefits-of-onvif-integration)
  - [Camera Discovery and Enumeration](#camera-discovery-and-enumeration)
    - [Discovering ONVIF Cameras on Your Network](#discovering-onvif-cameras-on-your-network)
    - [Querying Camera Capabilities](#querying-camera-capabilities)
  - [Connecting to ONVIF Cameras](#connecting-to-onvif-cameras)
    - [Basic Connection](#basic-connection)
  - [Working with Media Profiles](#working-with-media-profiles)
  - [Video Preview](#video-preview)
    - [Basic Preview Setup](#basic-preview-setup)
    - [Low-Latency Preview](#low-latency-preview)
  - [PTZ (Pan-Tilt-Zoom) Control](#ptz-pan-tilt-zoom-control)
    - [Basic PTZ Operations](#basic-ptz-operations)
    - [PTZ Presets](#ptz-presets)
    - [Absolute Positioning](#absolute-positioning)
  - [Camera Actions and Capabilities](#camera-actions-and-capabilities)
    - [Query Camera Capabilities](#query-camera-capabilities)
    - [Reboot Camera](#reboot-camera)
    - [Get System Date and Time](#get-system-date-and-time)
  - [Turn Local Camera into ONVIF Source](#turn-local-camera-into-onvif-source)
  - [Best Practices](#best-practices)
    - [Connection Management](#connection-management)
    - [Performance Optimization](#performance-optimization)
    - [Network Considerations](#network-considerations)
    - [Security](#security)
    - [Error Handling](#error-handling)
  - [Troubleshooting](#troubleshooting)
    - [Common Issues](#common-issues)
      - ["Unable to connect to ONVIF camera"](#unable-to-connect-to-onvif-camera)
      - ["No cameras discovered"](#no-cameras-discovered)
      - ["Stream won't play"](#stream-wont-play)
      - ["High CPU usage during recording"](#high-cpu-usage-during-recording)
      - ["PTZ commands not working"](#ptz-commands-not-working)
    - [Diagnostic Tools](#diagnostic-tools)
      - [Enable Debug Logging](#enable-debug-logging)
      - [Test RTSP Connection](#test-rtsp-connection)
    - [Getting Help](#getting-help)
    - [Related Demos](#related-demos)

## What is ONVIF?

ONVIF (Open Network Video Interface Forum) is an industry standard protocol that enables seamless interoperability between network video products from different manufacturers. This protocol defines a common interface for IP-based security devices including cameras, NVRs (Network Video Recorders), and access control systems.

**Key Benefits:**
- **Vendor Independence**: Work with cameras from different manufacturers using a unified API
- **Standardized Communication**: Consistent methods for device discovery, streaming, and control
- **Future-Proof**: New ONVIF-compliant devices work with existing applications
- **Rich Feature Set**: Access to profiles, media streams, events, PTZ, and more

## Benefits of ONVIF Integration

- **Vendor Neutrality**: Build applications that work with cameras from multiple manufacturers
- **Future-Proof Development**: As new ONVIF-compliant cameras enter the market, your application will support them
- **Standardized Communication**: Consistent methods for device discovery, video streaming, and PTZ controls
- **Reduced Development Time**: No need to implement proprietary APIs for each camera brand
- **Advanced Features**: Access to profiles, media streams, events, and device management

## Camera Discovery and Enumeration

### Discovering ONVIF Cameras on Your Network

The first step in working with ONVIF cameras is discovering them on your local network using the WS-Discovery protocol.

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFDiscovery.Models;

private Discovery _onvifDiscovery = new Discovery();
private CancellationTokenSource _cts;

// Discover cameras for 5 seconds
_cts = new CancellationTokenSource();

try
{
    await _onvifDiscovery.Discover(5, (device) =>
    {
        if (device.XAdresses?.Any() == true)
        {
            var address = device.XAdresses.FirstOrDefault();
            if (!string.IsNullOrEmpty(address))
            {
                Console.WriteLine($"Found camera at: {address}");
                // Add to your UI list, etc.
            }
        }
    }, _cts.Token);
}
catch (OperationCanceledException)
{
    // Discovery cancelled
}
```

**Key Features:**
- **WS-Discovery Protocol**: Automatically discovers ONVIF-compliant cameras on the local network
- **Timeout Control**: Specify discovery duration in seconds
- **Async Callback**: Receive discovered devices in real-time as they respond
- **Cancellation Support**: Cancel discovery using CancellationToken

### Querying Camera Capabilities

Once discovered, you can connect to a camera and query its capabilities:

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
var result = await onvifClient.ConnectAsync(cameraUrl, username, password);

if (result)
{
    // Get device information
    var deviceInfo = onvifClient.DeviceInformation;
    Console.WriteLine($"Camera: {deviceInfo?.Model}, S/N: {deviceInfo?.SerialNumber}");
    
    // Get available profiles
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles != null)
    {
        foreach (var profile in profiles)
        {
            var mediaUri = await onvifClient.GetStreamUriAsync(profile);
            if (mediaUri != null)
            {
                Console.WriteLine($"Profile: {profile.Name}, URI: {mediaUri.Uri}");
            }
        }
    }
}
```

## Connecting to ONVIF Cameras

### Basic Connection

```cs
using VisioForge.Core.ONVIFX;

// Connect to ONVIF camera
var onvifClient = new ONVIFClientX();
var connected = await onvifClient.ConnectAsync(
    "http://192.168.1.100:80/onvif/device_service", 
    "admin", 
    "password");

if (connected)
{
    Console.WriteLine("Successfully connected to camera");
}
else
{
    Console.WriteLine("Connection failed");
}
```

## Working with Media Profiles

ONVIF cameras typically provide multiple media profiles with different resolutions, codecs, and frame rates.

```cs
// Get all available profiles
var profiles = await onvifClient.GetProfilesAsync();

if (profiles != null && profiles.Length > 0)
{
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name}");
        Console.WriteLine($"Token: {profile.token}");
        
        // Get stream URI for this profile
        var mediaUri = await onvifClient.GetStreamUriAsync(profile);
        if (mediaUri != null)
        {
            Console.WriteLine($"  Stream URI: {mediaUri.Uri}");
        }
    }
    
    // Use the first profile
    var selectedProfile = profiles[0];
    var streamUri = await onvifClient.GetStreamUriAsync(selectedProfile);
}
```

## Video Preview

### Basic Preview Setup

=== "Media Blocks SDK"

    
    ```cs
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoRendering;
    using VisioForge.Core.MediaBlocks.AudioRendering;
    using VisioForge.Core.Types.X.Sources;
    
    // Create pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Get RTSP URL from ONVIF profile
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Create RTSP source
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // enable audio
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    
    // Create video renderer
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Connect blocks
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    
    // Optional: Add audio rendering
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);
    
    // Start preview
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.VideoCaptureX;
    
    // Create video capture engine
    var videoCapture = new VideoCaptureCoreX(videoView);
    
    // Get stream URI from ONVIF
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Create RTSP source settings
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // enable audio
    
    // Set video source
    videoCapture.Video_Source = rtspSettings;
    
    // Configure audio
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    // Start preview
    await videoCapture.StartAsync();
    ```
    


### Low-Latency Preview

For real-time surveillance and monitoring applications, enable low-latency mode:

=== "Media Blocks SDK"

    
    ```cs
    // Create RTSP source with low latency
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Enable low latency mode (60-120ms total latency)
    rtspSettings.LowLatencyMode = true;
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Disable sync for even lower latency
    videoRenderer.IsSync = false;
    
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // Create RTSP source
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Enable low latency mode
    rtspSettings.LowLatencyMode = true;
    
    videoCapture.Video_Source = rtspSettings;
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    await videoCapture.StartAsync();
    ```
    


## PTZ (Pan-Tilt-Zoom) Control

### Basic PTZ Operations

```cs
using VisioForge.Core.ONVIFX;
using VisioForge.Core.ONVIFX.PTZ;

// Connect to camera
var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Get profile token
var profiles = await onvifClient.GetProfilesAsync();
var profileToken = profiles[0].token;

// Pan right
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0.5f, y = 0 }, 
    null);

// Pan left
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = -0.5f, y = 0 }, 
    null);

// Tilt up
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0, y = 0.5f }, 
    null);

// Tilt down
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    new Vector2D { x = 0, y = -0.5f }, 
    null);

// Zoom in
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    null, 
    new Vector1D { x = 0.5f });

// Zoom out
await onvifClient.ContinuousMoveAsync(
    profileToken, 
    null, 
    new Vector1D { x = -0.5f });

// Stop movement
await onvifClient.StopAsync(profileToken, true, true);
```

### PTZ Presets

```cs
// Get available presets
var presets = await onvifClient.GetPresetsAsync(profileToken);

foreach (var preset in presets)
{
    Console.WriteLine($"Preset: {preset.Name}, Token: {preset.token}");
}

// Go to preset (home position)
if (presets != null && presets.Length > 0)
{
    await onvifClient.GoToPresetAsync(
        profileToken, 
        presets[0].token, 
        1.0f,  // pan/tilt speed
        1.0f,  // zoom speed
        1.0f); // time
}

// Set current position as preset
await onvifClient.SetPresetAsync(profileToken, "MyPreset");
```

### Absolute Positioning

```cs
// Move to absolute position
await onvifClient.AbsoluteMoveAsync(
    profileToken,
    new PTZVector
    {
        PanTilt = new Vector2D { x = 0.5f, y = 0.3f },
        Zoom = new Vector1D { x = 0.7f }
    },
    1.0f); // speed
```

## Camera Actions and Capabilities

### Query Camera Capabilities

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Get device information
var deviceInfo = onvifClient.DeviceInformation;
Console.WriteLine($"Manufacturer: {deviceInfo?.Manufacturer}");
Console.WriteLine($"Model: {deviceInfo?.Model}");
Console.WriteLine($"Firmware: {deviceInfo?.FirmwareVersion}");
Console.WriteLine($"Serial Number: {deviceInfo?.SerialNumber}");
Console.WriteLine($"Hardware ID: {deviceInfo?.HardwareId}");

// Get service capabilities
var capabilities = await onvifClient.GetCapabilitiesAsync();

// Check for PTZ support
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ supported");
}

// Check for analytics
if (capabilities?.Analytics != null)
{
    Console.WriteLine("Analytics supported");
}

// Check for events
if (capabilities?.Events != null)
{
    Console.WriteLine("Events supported");
}
```

### Reboot Camera

```cs
await onvifClient.SystemRebootAsync();
```

### Get System Date and Time

```cs
var dateTime = await onvifClient.GetSystemDateAndTimeAsync();
Console.WriteLine($"Camera time: {dateTime}");
```

## Turn Local Camera into ONVIF Source

Convert your local USB/webcam into an IP stream that ONVIF clients can consume. The `RTSP Webcam Server` console demo included with the Media Blocks SDK exposes a DirectShow camera as an RTSP endpoint that ONVIF recorders and VMS software can ingest.

=== "Media Blocks SDK"

    
    ```cs
    using System;
    using System.Threading;
    using VisioForge.Core;
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sinks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoEncoders;
    using VisioForge.Core.Types.X.Output;
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.Types.X.VideoCapture;
    
    Console.WriteLine("Initializing VisioForge SDK.");
    VisioForgeX.InitSDK();
    
    var cameras = DeviceEnumerator.Shared.VideoSources();
    Console.WriteLine("Select the web camera");
    for (int i = 0; i < cameras.Length; i++)
    {
        Console.WriteLine($"{i + 1}: {cameras[i].DisplayName}");
    }
    
    Console.Write("Enter the number of the camera: ");
    VideoCaptureDeviceInfo cameraInfo = null;
    if (int.TryParse(Console.ReadLine(), out int cameraIndex) && cameraIndex > 0 && cameraIndex <= cameras.Length)
    {
        cameraInfo = cameras[cameraIndex - 1];
        Console.WriteLine($"Selected camera: {cameraInfo.DisplayName}");
    }
    else
    {
        Console.WriteLine("Invalid selection. Exiting.");
    
        VisioForgeX.DestroySDK();
        return;
    }
    
    var pipeline = new MediaBlocksPipeline();
    
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(cameraInfo);
    videoSourceSettings.Format = cameraInfo.GetHDVideoFormatAndFrameRate(out var frameRate).ToFormat();
    videoSourceSettings.Format.FrameRate = frameRate;
    
    var cameraSource = new SystemVideoSourceBlock(videoSourceSettings);
    
    var rtspServerSettings = new RTSPServerSettings(H264EncoderBlock.GetDefaultSettings(), null)
    {
        Port = 8554,
    };
    
    var rtspBlock = new RTSPServerBlock(rtspServerSettings);
    
    Console.WriteLine("RTSP Server URL: " + rtspBlock.Settings.URL);
    
    pipeline.Connect(cameraSource.Output, rtspBlock.Input);
    
    Console.WriteLine("Starting the pipeline...");
    
    new Thread(() =>
    {
        pipeline.Start();
    }).Start();
    
    Console.WriteLine("Pipeline started. Press any key to stop the server and exit.");
    Console.ReadKey();
    
    Console.WriteLine("Stopping the pipeline...");
    
    pipeline.Stop();
    
    Console.WriteLine("Pipeline stopped.");
    
    pipeline.Dispose();
    
    VisioForgeX.DestroySDK();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // ONVIF/RTSP server functionality is provided by the Media Blocks SDK.
    ```
    


## Best Practices

### Connection Management

1. **Always dispose of ONVIF clients properly:**
   ```cs
   using (var onvifClient = new ONVIFClientX())
   {
       await onvifClient.ConnectAsync(url, username, password);
       // ... use client ...
   } // Automatically disposed
   ```

2. **Handle connection failures gracefully:**
   ```cs
   var maxRetries = 3;
   var retryCount = 0;
   
   while (retryCount < maxRetries)
   {
       try
       {
           var connected = await onvifClient.ConnectAsync(url, user, pass);
           if (connected)
               break;
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Connection attempt {retryCount + 1} failed: {ex.Message}");
       }
       
       retryCount++;
       await Task.Delay(2000); // Wait before retry
   }
   ```

3. **Use cancellation tokens for discovery:**
   ```cs
   var cts = new CancellationTokenSource();
   cts.CancelAfter(TimeSpan.FromSeconds(10));
   
   await _onvifDiscovery.Discover(10, callback, cts.Token);
   ```

### Performance Optimization

1. **Use RTSPRAWSourceBlock for recording without re-encoding** - significantly reduces CPU usage
2. **Enable low-latency mode only when needed** - trades stability for speed
3. **Limit concurrent streams** based on your hardware capabilities
4. **Use hardware decoders** when available:
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```

### Network Considerations

1. **Use TCP for reliable connections:**
   ```cs
   rtspSettings.Transport = RTSPTransport.TCP;
   ```

2. **Configure appropriate timeouts:**
   ```cs
   rtspSettings.Timeout = TimeSpan.FromSeconds(30);
   ```

3. **Monitor for disconnections:**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       if (e.Message.Contains("disconnect"))
       {
           // Attempt reconnection
       }
   };
   ```

### Security

1. **Never hardcode credentials** - use configuration files or secure storage
2. **Use HTTPS for web streaming** when possible
3. **Implement authentication** for streaming endpoints
4. **Validate user input** when constructing URLs
5. **Keep credentials in memory for minimal time**

### Error Handling

1. **Log all errors for diagnostics:**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Logger.Error($"Pipeline error: {e.Message}");
   };
   ```

2. **Enable debug mode during development:**
   ```cs
   pipeline.Debug_Mode = true;
   pipeline.Debug_Dir = @"C:\Logs\VisioForge";
   ```

3. **Handle stream interruptions:**
   ```cs
   // Implement auto-reconnect logic
   var reconnectAttempts = 0;
   const int maxReconnects = 5;
   
   pipeline.OnError += async (sender, e) =>
   {
       if (reconnectAttempts < maxReconnects)
       {
           reconnectAttempts++;
           await Task.Delay(5000);
           await pipeline.StopAsync();
           await pipeline.StartAsync();
       }
   };
   ```

## Troubleshooting

### Common Issues

#### "Unable to connect to ONVIF camera"

**Possible causes:**
- Incorrect URL format (should be: `http://IP:PORT/onvif/device_service`)
- Wrong credentials
- Network firewall blocking connection
- Camera's ONVIF service disabled

**Solutions:**
```cs
// Try different URL formats
var urls = new[]
{
    "http://192.168.1.100:80/onvif/device_service",
    "http://192.168.1.100:8080/onvif/device_service",
    "http://192.168.1.100/onvif/device_service"
};

foreach (var url in urls)
{
    if (await onvifClient.ConnectAsync(url, user, pass))
    {
        Console.WriteLine($"Connected using: {url}");
        break;
    }
}
```

#### "No cameras discovered"

**Possible causes:**
- Cameras on different subnet
- Multicast blocked by network
- Firewall blocking WS-Discovery

**Solutions:**
1. Check network configuration
2. Try direct connection with known IP
3. Verify camera ONVIF support is enabled
4. Increase discovery timeout

#### "Stream won't play"

**Possible causes:**
- Codec not supported
- Network bandwidth insufficient
- Incorrect stream URL

**Solutions:**
```cs
// Get stream info before playing
var info = rtspSettings.GetInfo();
if (info == null)
{
    Console.WriteLine("Cannot get stream info - check URL");
    return;
}

Console.WriteLine($"Video codec: {info.VideoStreams[0].CodecName}");
Console.WriteLine($"Resolution: {info.VideoStreams[0].Width}x{info.VideoStreams[0].Height}");
Console.WriteLine($"Bitrate: {info.VideoStreams[0].Bitrate}");
```

#### "High CPU usage during recording"

**Possible causes:**
- Re-encoding when not necessary
- Too many concurrent streams
- Software decoding instead of hardware

**Solutions:**
1. Use `RTSPRAWSourceBlock` for recording without re-encoding
2. Enable hardware decoder:
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```
3. Limit concurrent streams
4. Lower resolution/bitrate at camera

#### "PTZ commands not working"

**Possible causes:**
- Camera doesn't support PTZ
- Wrong profile selected
- PTZ service not enabled

**Solutions:**
```cs
// Check PTZ capabilities
var capabilities = await onvifClient.GetCapabilitiesAsync();
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ is supported");
    
    // Get PTZ configuration
    var profiles = await onvifClient.GetProfilesAsync();
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name}, Token: {profile.token}");
    }
}
else
{
    Console.WriteLine("PTZ not supported by this camera");
}
```

### Diagnostic Tools

#### Enable Debug Logging

=== "Media Blocks SDK"

    
    ```cs
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    

=== "Video Capture SDK"

    
    ```cs
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    


#### Test RTSP Connection

```cs
// Test if RTSP URL is valid
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), 
    username, 
    password, 
    true);

var info = rtspSettings.GetInfo();
if (info != null)
{
    Console.WriteLine("✓ RTSP connection successful");
    Console.WriteLine($"  Video streams: {info.VideoStreams.Count}");
    Console.WriteLine($"  Audio streams: {info.AudioStreams.Count}");
    
    foreach (var stream in info.VideoStreams)
    {
        Console.WriteLine($"  Video: {stream.CodecName} {stream.Width}x{stream.Height}");
    }
}
else
{
    Console.WriteLine("✗ RTSP connection failed");
}
```

### Getting Help

- **Sample Code**: [GitHub Samples Repository](https://github.com/visioforge/.Net-SDK-s-samples)
- **Documentation**: [VisioForge Documentation](https://www.visioforge.com/help/)
- **Support Forum**: [VisioForge Support](https://support.visioforge.com)

### Related Demos

- [RTSP MultiView Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) - Recording multiple cameras without re-encoding
- [RTSP Preview Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) - Preview and recording with postprocessing
- [IP Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) - Comprehensive IP camera integration with PTZ control
