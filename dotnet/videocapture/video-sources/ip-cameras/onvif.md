---
title: ONVIF IP Camera in C# .NET — Discovery & PTZ Control
description: Auto-discover ONVIF cameras via WS-Discovery, control PTZ presets, and record streams using VisioForge Video Capture SDK. C# examples and troubleshooting.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Decoding
  - Webcam
  - IP Camera
  - NDI Source
  - RTSP
  - ONVIF
  - NDI
  - H.264
  - C#
primary_api_classes:
  - RTSPSourceSettings
  - ONVIFClientX
  - VideoCaptureCoreX
  - MediaBlocksPipeline
  - RTSPSourceBlock

---

# ONVIF IP Camera Integration - Complete Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    ONVIF discovery and PTZ control work on both `VideoCaptureCore` (Windows-only, classic DirectShow) and `VideoCaptureCoreX` / Media Blocks SDK (cross-platform: **Windows, macOS, Linux, Android, and iOS** via GStreamer). See the [platform support matrix](../../../platform-matrix.md) and the [Linux deployment guide](../../../deployment-x/Ubuntu.md).

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

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

// The lightweight scalar overload of ContinuousMoveAsync takes pan/tilt/zoom
// speed floats directly (range -1.0..1.0; 0 = no motion on that axis).

// Pan right
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0.5f, tilt: 0f, zoom: 0f);

// Pan left
await onvifClient.ContinuousMoveAsync(profileToken, pan: -0.5f, tilt: 0f, zoom: 0f);

// Tilt up
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0.5f, zoom: 0f);

// Tilt down
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: -0.5f, zoom: 0f);

// Zoom in
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0f, zoom: 0.5f);

// Zoom out
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0f, zoom: -0.5f);

// Stop movement on both axes
await onvifClient.StopMoveAsync(profileToken, panTilt: true, zoom: true);
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
        panSpeed:  1.0f,
        tiltSpeed: 1.0f,
        zoomSpeed: 1.0f);
}

// Set current position as preset
await onvifClient.SetPresetAsync(profileToken, "MyPreset");
```

### Absolute Positioning

```cs
// Move to absolute pan + tilt + zoom position with per-axis speeds
await onvifClient.AbsoluteMoveAsync(
    profileToken,
    pan:       0.5f,
    tilt:      0.3f,
    zoom:      0.7f,
    panSpeed:  1.0f,
    tiltSpeed: 1.0f,
    zoomSpeed: 1.0f);
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

1. **Use TCP for reliable connections** (`AllowedProtocols` is a `RTSPSourceProtocol` flag-enum — set it to force a specific transport):
   ```cs
   rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;
   ```

2. **Tune jitter-buffer latency** (there is no standalone `Timeout` property; `Latency` controls the RTSP jitter-buffer — higher values trade latency for stability over lossy networks):
   ```cs
   rtspSettings.Latency = TimeSpan.FromMilliseconds(1000);
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
// Read and cache stream info before starting the pipeline. ReadInfoAsync
// probes the camera; GetInfo() returns the cached MediaFileInfo afterwards.
var info = await rtspSettings.ReadInfoAsync();
if (info == null)
{
    Console.WriteLine("Cannot get stream info - check URL");
    return;
}

Console.WriteLine($"Video codec: {info.VideoStreams[0].Codec}");
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

var info = await rtspSettings.ReadInfoAsync();
if (info != null)
{
    Console.WriteLine("✓ RTSP connection successful");
    Console.WriteLine($"  Video streams: {info.VideoStreams.Count}");
    Console.WriteLine($"  Audio streams: {info.AudioStreams.Count}");

    foreach (var stream in info.VideoStreams)
    {
        Console.WriteLine($"  Video: {stream.Codec} {stream.Width}x{stream.Height}");
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
- [IP Camera Brands Directory](../../../camera-brands/index.md) - RTSP URLs and connection guides for 60+ camera manufacturers

### Related Documentation

- [RTSP camera source configuration](rtsp.md) — `IPCameraSourceSettings` and `RTSPSourceSettings` reference with UDP/TCP tuning
- [RTSP protocol deep-dive](../../../general/network-streaming/rtsp.md) — protocol internals and streaming architecture
- [NDI source integration](ndi.md) — professional video-over-IP alternative to ONVIF/RTSP
- [IP camera live preview tutorial](../../video-tutorials/ip-camera-preview.md) — video walkthrough with a minimal C# example
- [Record RTSP to MP4 tutorial](../../video-tutorials/ip-camera-capture-mp4.md) — capture ONVIF-discovered cameras to file
- [Media Blocks RTSP player](../../../mediablocks/Guides/rtsp-player-csharp.md) — pipeline-based RTSP playback
- [Multi-camera RTSP grid (NVR wall)](../../../mediablocks/Guides/multi-camera-rtsp-grid.md) — 4×4 live preview wall for WPF and MAUI, with ONVIF-discovered cameras
- [RTSP reconnection and fallback switch](../../../general/network-sources/reconnection-and-fallback.md) — handle camera drops with disconnect events and automatic `FallbackSwitch`
