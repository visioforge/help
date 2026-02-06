---
title: USB3 Vision, GigE & GenICam Integration Guide
description: Integrate USB3 Vision, GigE, and GenICam industrial cameras with DirectShow drivers and cross-platform machine vision support.
sidebar_label: USB3 Vision, GigE, and GenICam devices
order: 15
---

# USB3 Vision, GigE, and GenICam Camera Integration

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button }

## Overview

Industrial cameras using USB3 Vision, GigE Vision, and GenICam standards provide superior image quality and performance for machine vision applications. Our SDK enables seamless integration with these professional camera types through various connectivity options.

## GigE Vision Protocol

GigE Vision is an industrial camera interface standard based on Gigabit Ethernet technology. It offers several advantages for machine vision applications:

- **High-speed data transfer**: Supports up to 1 Gbps on standard GigE networks and 10+ Gbps on modern 10GigE networks
- **Long cable length**: Can operate at distances up to 100 meters using standard Ethernet cabling
- **Network architecture**: Multiple cameras can share the same network infrastructure
- **Power over Ethernet (PoE)**: Cameras can receive power through the same Ethernet cable (when using PoE-enabled switches)
- **Device discovery**: Automatic detection of GigE Vision cameras on the network
- **Multicast capabilities**: Allows streaming to multiple clients simultaneously

GigE Vision combines the GenICam programming interface with GigE transport layer, providing consistent command structures across different manufacturers' cameras.

## USB3 Vision Protocol

USB3 Vision is a camera interface standard that leverages the high-speed USB 3.0 interface for industrial imaging applications:

- **High bandwidth**: Up to 5 Gbps theoretical transfer rate, enabling high resolution and frame rates
- **Plug-and-play**: Simple connectivity without specialized interface cards
- **Hot-swappable**: Devices can be connected or disconnected without system reboot
- **Cable length**: Typically supports distances up to 5 meters (can be extended with active cables)
- **Power delivery**: Up to 4.5W provided directly through the USB connection
- **Standard driver architecture**: Uses standard USB drivers from operating systems

USB3 Vision works alongside the GenICam standard to provide consistent camera control across different manufacturers.

## GenTL (Generic Transport Layer) Protocol Support

VisioForge provides comprehensive support for the **GenICam GenTL (Generic Transport Layer)** standard, which is a key component of industrial machine vision systems. GenTL defines a standardized interface for accessing cameras through various transport protocols while maintaining vendor-neutral compatibility.

### What is GenTL?

GenTL (Generic Transport Layer) is a standardized interface specification that provides:

- **Transport-agnostic access**: Unified API for cameras regardless of physical transport layer (GigE, USB3, CoaXPress, Camera Link, etc.)
- **Vendor neutrality**: Consistent interface across different camera manufacturers
- **Modular architecture**: Separates transport-specific implementations from application logic
- **Producer/Consumer model**: GenTL Producers handle transport specifics, while GenTL Consumers (applications) use standardized interfaces

### VisioForge GenTL Implementation

Our SDK includes full GenTL support through:

#### 1. **Automatic Protocol Detection**

The system automatically detects when a camera is connected via GenTL and sets the protocol accordingly.

#### 2. **GenTL Environment Setup**

Support for standard GenTL environment variables:

- **GENICAM_GENTL64_PATH**: Path to GenTL producer libraries (64-bit)
- Automatic discovery of installed GenTL producers

#### 3. **Comprehensive Error Handling**

Full support for GenTL-specific error codes including:

- System initialization errors
- Transport layer communication issues
- Device access and resource management
- Buffer and streaming errors
- Timeout and abort conditions

#### 4. **Advanced Features**

- **Device enumeration**: Discovery of GenTL-compatible devices across all available transport layers
- **Stream management**: High-performance streaming with GenTL buffer management
- **Feature access**: Full GenICam feature tree access through GenTL interface
- **Multi-transport support**: Simultaneous access to cameras on different transport layers

### GenTL Producer Compatibility

VisioForge's GenTL implementation is compatible with producers from major manufacturers:

- **Camera Link**: High-speed frame grabber interfaces
- **CoaXPress**: Long-distance, high-bandwidth connections
- **10 GigE**: Ultra-high-speed Ethernet connections
- **Custom transport layers**: Vendor-specific transport implementations
- **Multi-interface systems**: Mixed transport environments

### Integration Benefits

Using GenTL with VisioForge provides several advantages:

1. **Future-proof architecture**: Support for new transport layers without application changes
2. **Simplified development**: Single API for all supported transport types
3. **Enhanced performance**: Optimized transport-specific implementations
4. **Broader camera support**: Access to cameras not available through native interfaces
5. **Professional features**: Advanced triggering, synchronization, and control capabilities

### Configuration Requirements

To use GenTL cameras with VisioForge:

1. Install the appropriate GenTL producer from your camera manufacturer
2. Set the `GENICAM_GENTL64_PATH` environment variable to point to the producer library
3. Ensure cameras are properly connected and recognized by the GenTL producer
4. Use standard VisioForge GenICam enumeration methods to discover GenTL devices

The system automatically handles GenTL initialization, device discovery, and transport layer management.

## DirectShow Driver Support

Most industrial camera manufacturers include DirectShow-compatible drivers with their development kits. These drivers create a bridge between the camera's native interface and the DirectShow framework, allowing our SDK to access and control these specialized devices.

Key benefits:

- Simplified integration path
- Full access to camera streams
- Compatibility with existing DirectShow workflows

## Cross-Platform GenICam Support

For developers working in multi-platform environments, our SDK's cross-platform engine supports cameras implementing the unified GenICam interface standard. This provides consistent access to camera features across different operating systems.

## Prerequisites

### macOS

Install the `Aravis` package using Homebrew:

```bash
brew install aravis
```

### Linux

Install the `Aravis` package using the package manager:

```bash
sudo apt-get install libaravis-0.8-dev
```

### Windows

Install the `VisioForge.CrossPlatform.GenICam.Windows.x64` package to your project using NuGet.

#### USB Driver Installation on Windows

By default on Windows, USB3 Vision cameras may not have the appropriate USB3 driver installed, which can prevent them from appearing in device enumeration lists. This is a common issue with industrial USB cameras that require specific driver support.

#### Driver Installation Solutions

##### Option 1: Generic USB Driver Installation with Zadig

For cameras without manufacturer-specific drivers, you can install generic USB drivers using [Zadig](https://zadig.akeo.ie/), a Windows application that simplifies USB driver installation:

1. **Download and run Zadig** from [https://zadig.akeo.ie/](https://zadig.akeo.ie/)
2. **Connect your USB3 Vision camera** to the computer
3. **Select the camera device** from the device list in Zadig
4. **Choose the appropriate driver**:
   - **WinUSB**: Recommended for most GenICam applications
   - **libusb-win32**: For legacy libusb-based applications
   - **libusbK**: Alternative high-performance USB driver
5. **Install the driver** by clicking "Install Driver" or "Replace Driver"

After installation, the camera should appear in VisioForge's device enumeration and be accessible through the GenICam interface.

##### Option 2: Manufacturer SDK with GenTL Bridge

If you have a camera SDK from the device vendor, the camera can be connected using the **GenTL bridge** approach:

1. **Install the manufacturer's SDK** (e.g., Basler pylon, FLIR Spinnaker)
2. **Set up the GenTL environment** by configuring the `GENICAM_GENTL64_PATH` environment variable
3. **Use the GenTL producer** provided by the manufacturer's SDK
4. **Access the camera** through VisioForge's GenTL support

This approach provides access to vendor-specific features and optimizations while maintaining compatibility with VisioForge's unified GenICam interface.

## Compatible SDKs from Major Manufacturers

The following manufacturer SDKs are known to work well with our integration:

- [Basler pylon SDK](https://www.baslerweb.com/en/software/pylon/sdk/) - Comprehensive toolkit for Basler cameras
- [FLIR/Teledyne Spinnaker SDK](https://www.teledynevisionsolutions.com/) - Advanced imaging solution for FLIR and Teledyne cameras

## Code Examples

The following examples demonstrate practical implementation of GenICam, USB3 Vision, and GigE cameras using VisioForge's Video Capture SDK with GenICam integration.

### Basic Camera Discovery and Information

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Initialize the SDK
await VisioForgeX.InitSDKAsync();

// Discover available GenICam cameras
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

Console.WriteLine($"Found {devices.Length} GenICam devices");

foreach (var device in devices)
{
    Console.WriteLine($"Camera: {device.CameraName}");
    Console.WriteLine($"Device ID: {device.DeviceId}");
    Console.WriteLine($"Address: {device.Address}");
    Console.WriteLine();
}

// Get detailed information about a specific camera
if (devices.Length > 0)
{
    var cameraDeviceId = devices[0].DeviceId;
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    
    if (camera != null && GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        camera.ReadInfo();
        
        Console.WriteLine($"Connected to: {camera.VendorName} {camera.ModelName}");
        Console.WriteLine($"Serial Number: {camera.SerialNumber}");
        Console.WriteLine($"Protocol: {camera.Protocol}");
        Console.WriteLine($"Sensor Size: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
        Console.WriteLine($"Available Pixel Formats: {string.Join(", ", camera.AvailablePixelFormats)}");
        
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
```

### Live Preview with VideoCaptureCoreX

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Initialize SDK
await VisioForgeX.InitSDKAsync();

// Create VideoCaptureCoreX instance (assumes you have a video view control)
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Discover cameras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0]; // Use first camera
    Console.WriteLine($"Using camera: {selectedDevice.CameraName}");

    // Configure camera before starting capture
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Configure camera settings
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(10000); // 10ms exposure
        }
        
        if (camera.GainAvailable)
        {
            camera.SetGain(0.0); // Minimum gain
        }
        
        // Get camera resolution and frame rate
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Create GenICam source
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Start preview
        await videoCapture.StartAsync();
        
        Console.WriteLine("Live preview started. Press any key to stop...");
        Console.ReadKey();
        
        await videoCapture.StopAsync();
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Recording to MP4 File

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Initialize SDK
await VisioForgeX.InitSDKAsync();

// Create VideoCaptureCoreX instance
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Configure debug mode
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Discover and select camera
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0];
    Console.WriteLine($"Recording from camera: {selectedDevice.CameraName}");

    // Configure camera settings
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Set camera parameters
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(5000); // 5ms exposure
        }
        
        if (camera.FrameRateAvailable)
        {
            var targetFps = Math.Min(30.0, camera.FrameRateBounds.Max);
            camera.SetFrameRate(new VideoFrameRate(targetFps));
        }

        // Get camera resolution and frame rate
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Create GenICam source
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Configure MP4 output
        string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4Output(outputFilename, H264EncoderBlock.GetDefaultSettings(), null);
        videoCapture.Outputs_Add(mp4Output);
        
        // Start recording
        await videoCapture.StartAsync();
        
        Console.WriteLine($"Recording started to: {outputFilename}");
        Console.WriteLine("Press any key to stop recording...");
        Console.ReadKey();
        
        // Stop recording
        await videoCapture.StopAsync();
        Console.WriteLine($"Recording saved to: {outputFilename}");
        
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Advanced Camera Configuration

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core.Types;
using System;
using System.Linq;
using System.Threading;

// Discover and connect to camera
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
if (devices.Length == 0) return;

var camera = GenICamCameraManager.GetCamera(devices[0].DeviceId);

if (camera != null && GenICamCameraManager.OpenCamera(devices[0].DeviceId))
{
    camera.ReadInfo();
    
    // Display camera capabilities
    Console.WriteLine($"Camera: {camera}");
    Console.WriteLine($"Available pixel formats: {string.Join(", ", camera.AvailablePixelFormats)}");
    
    // Configure pixel format
    if (camera.AvailablePixelFormats.Contains("Mono8"))
    {
        camera.SetPixelFormat("Mono8");
        Console.WriteLine("Set pixel format to Mono8");
    }
    
    // Configure exposure with auto mode
    if (camera.IsExposureAutoAvailable)
    {
        // First try auto exposure
        camera.SetExposureAuto(GenICamAuto.Once);
        Thread.Sleep(1000); // Wait for auto exposure to complete
        
        // Then switch to manual and read the auto-calculated value
        camera.SetExposureAuto(GenICamAuto.Off);
        var autoExposure = camera.GetExposureTime();
        Console.WriteLine($"Auto exposure calculated: {autoExposure:F2} μs");
        
        // Fine-tune manually if needed
        camera.SetExposureTime(autoExposure * 1.2); // 20% longer exposure
    }
    
    // Configure gain
    if (camera.IsGainAutoAvailable)
    {
        camera.SetGainAuto(GenICamAuto.Continuous);
        Console.WriteLine("Enabled continuous auto gain");
    }
    
    // Configure binning for higher frame rates
    if (camera.BinningAvailable)
    {
        camera.SetBinning(2, 2); // 2x2 binning
        Console.WriteLine("Set 2x2 binning for higher sensitivity and frame rate");
    }
    
    // Configure software triggering
    if (camera.SoftwareTriggerSupported)
    {
        camera.SetStringFeature("TriggerMode", "On");
        camera.SetStringFeature("TriggerSource", "Software");
        camera.SetAcquisitionMode(GenICamAcquisitionMode.Continuous);
        
        Console.WriteLine("Configured for software triggering");
        
        // Note: When using with VideoCaptureCoreX, software triggering would be
        // integrated into the capture pipeline rather than called directly
    }
    
    // Read and display advanced features
    camera.ReadAvailableFeatures();
    Console.WriteLine($"Camera has {camera.AvailableStringFeatures.Length + camera.AvailableIntegerFeatures.Length + camera.AvailableFloatFeatures.Length + camera.AvailableBooleanFeatures.Length} features");
    Console.WriteLine($"Advanced features available: {camera.HasAdvancedFeatures}");
    
    GenICamCameraManager.CloseCamera(devices[0].DeviceId);
}
```

### Using GenICamSourceBlock with Media Blocks Pipeline

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Initialize SDK
await VisioForgeX.InitSDKAsync();

// Create Media Blocks Pipeline
var pipeline = new MediaBlocksPipeline();

try
{
    // Configure debug mode
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Discover cameras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0];
    string cameraDeviceId = selectedDevice.DeviceId;

    // Configure camera
    if (GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
        camera?.ReadInfo();

        // Create GenICam source block
        var sourceSettings = new GenICamSourceSettings(cameraDeviceId);
        var sourceBlock = new GenICamSourceBlock(sourceSettings);

        // Create video renderer for preview
        var videoRenderer = new VideoRendererBlock(pipeline, yourVideoViewControl) { IsSync = false };

        // Create tee block for splitting the stream
        var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);

        // Create MP4 output block
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4OutputBlock(new MP4SinkSettings(outputFile), H264EncoderBlock.GetDefaultSettings(), aacSettings: null);

        // Connect the pipeline
        pipeline.Connect(sourceBlock.Output, videoTee.Input);
        pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
        
        var videoInput = mp4Output.CreateNewInput(MediaBlockPadMediaType.Video);
        pipeline.Connect(videoTee.Outputs[1], videoInput);

        // Start the pipeline
        await pipeline.StartAsync();

        Console.WriteLine($"Recording to: {outputFile}");
        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        // Stop the pipeline
        await pipeline.StopAsync();
        Console.WriteLine($"Recording saved to: {outputFile}");

        // Cleanup
        mp4Output.Dispose();
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Error Handling and Recovery

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

// Initialize SDK
await VisioForgeX.InitSDKAsync();

string cameraDeviceId = null;
VideoCaptureCoreX videoCapture = null;

try
{
    // Discover cameras with retry logic
    int maxDiscoveryRetries = 3;
    var devices = new GenICamSourceInfo[0];
    
    for (int attempt = 1; attempt <= maxDiscoveryRetries; attempt++)
    {
        try
        {
            GenICamCameraManager.UpdateDeviceList();
            devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
            
            if (devices.Length > 0)
            {
                Console.WriteLine($"Found {devices.Length} cameras on attempt {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Discovery attempt {attempt} failed: {ex.Message}");
            if (attempt < maxDiscoveryRetries)
            {
                Thread.Sleep(2000); // Wait before retry
            }
        }
    }
    
    if (devices.Length == 0)
    {
        Console.WriteLine("No cameras found after all attempts");
        return;
    }
    
    cameraDeviceId = devices[0].DeviceId;
    
    // Camera connection with retry logic
    int maxRetries = 3;
    bool connected = false;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            connected = GenICamCameraManager.OpenCamera(cameraDeviceId);
            if (connected)
            {
                Console.WriteLine($"Connected to camera on attempt {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection attempt {attempt} failed: {ex.Message}");
            if (attempt < maxRetries)
            {
                Thread.Sleep(2000); // Wait before retry
            }
        }
    }
    
    if (!connected)
    {
        Console.WriteLine("Failed to connect after all attempts");
        return;
    }
    
    // Configure camera
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    camera?.ReadInfo();
    
    // Create VideoCaptureCoreX with error handling
    videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
    
    // Set up error event handler
    videoCapture.OnError += (sender, e) =>
    {
        Console.WriteLine($"Capture error: {e.Message}");
    };
    
    // Configure source
    var sourceSettings = new GenICamSourceSettings(
        cameraDeviceId,
        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
        camera.GetFrameRate(),
        GenICamPixelFormat.Default
    );
    
    videoCapture.Video_Source = sourceSettings;
    
    // Start capture with monitoring
    await videoCapture.StartAsync();
    
    Console.WriteLine("Capture started. Monitoring for errors...");
    
    // Monitor for 30 seconds
    var startTime = DateTime.Now;
    while ((DateTime.Now - startTime).TotalSeconds < 30)
    {
        Thread.Sleep(1000);
        
        // Check capture state
        if (videoCapture.State != VisioForge.Core.Types.PlaybackState.Play)
        {
            Console.WriteLine("Capture stopped unexpectedly. Attempting restart...");
            
            try
            {
                await videoCapture.StopAsync();
                await Task.Delay(1000);
                await videoCapture.StartAsync();
                Console.WriteLine("Capture restarted successfully");
            }
            catch (Exception restartEx)
            {
                Console.WriteLine($"Failed to restart capture: {restartEx.Message}");
                break;
            }
        }
    }
    
    await videoCapture.StopAsync();
    Console.WriteLine("Capture monitoring completed");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
finally
{
    // Clean up
    if (videoCapture != null)
    {
        await videoCapture.DisposeAsync();
    }
    
    if (!string.IsNullOrEmpty(cameraDeviceId))
    {
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
    
    Console.WriteLine("Resources cleaned up");
}
```

### Working with GenTL Cameras

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// For GenTL cameras, ensure the environment variable is set
// GENICAM_GENTL64_PATH should point to the GenTL producer library

// Example: Set in your application startup or environment
Environment.SetEnvironmentVariable("GENICAM_GENTL64_PATH", @"C:\Program Files\Basler\pylon 7\Runtime\x64");

// Initialize SDK
await VisioForgeX.InitSDKAsync();

// Discover GenTL cameras (they will appear alongside other GenICam devices)
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in devices)
{
    // Check camera information
    var camera = GenICamCameraManager.GetCamera(device.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(device.DeviceId))
    {
        camera.ReadInfo();
        
        // Check if this is a GenTL device
        if (camera.Protocol == "GenTL")
        {
            Console.WriteLine($"Found GenTL camera: {camera}");
            
            try
            {
                // Configure GenTL-specific features for maximum performance
                if (camera.IsFeatureAvailable("StreamBufferCountMode"))
                {
                    camera.SetStringFeature("StreamBufferCountMode", "Manual");
                }
                
                if (camera.IsFeatureAvailable("StreamBufferCountManual"))
                {
                    camera.SetIntegerFeature("StreamBufferCountManual", 20); // More buffers
                }
                
                // Set acquisition parameters
                if (camera.ExposureTimeAvailable)
                {
                    camera.SetExposureTime(1000); // 1ms exposure
                }
                
                // Use with VideoCaptureCoreX
                var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
                
                try
                {
                    var sourceSettings = new GenICamSourceSettings(
                        device.DeviceId,
                        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
                        camera.GetFrameRate(),
                        GenICamPixelFormat.Default
                    );
                    
                    videoCapture.Video_Source = sourceSettings;
                    
                    // Start preview
                    await videoCapture.StartAsync();
                    Console.WriteLine($"GenTL camera preview started: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
                    
                    // Let it run for a few seconds
                    await Task.Delay(3000);
                    
                    await videoCapture.StopAsync();
                    Console.WriteLine("GenTL camera preview stopped");
                }
                finally
                {
                    await videoCapture.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error using GenTL camera: {ex.Message}");
            }
        }
        
        GenICamCameraManager.CloseCamera(device.DeviceId);
    }
}
```

## Sample Projects

For complete integration examples and sample projects, explore these specific GenICam implementations:

- **[GenICam Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/GenICam%20Capture)** - Complete WPF application demonstrating GenICam camera integration with VideoCaptureCoreX
- **[Media Blocks GenICam Source Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)** - Advanced Media Blocks pipeline implementation using GenICam sources

For additional integration examples and sample projects, visit our [GitHub samples repository](https://github.com/visioforge/.Net-SDK-s-samples) to explore more code samples across different platforms and use cases.
