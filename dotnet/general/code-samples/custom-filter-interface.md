---
title: Working with Custom DirectShow Filter Interfaces in .NET
description: Learn how to implement and use custom DirectShow filter interfaces in .NET applications. This guide provides step-by-step examples for accessing and manipulating DirectShow components through the IBaseFilter interface in your multimedia applications.
sidebar_label: Custom Filter Interface Usage
---

# Working with Custom DirectShow Filter Interfaces in .NET

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

*Note: The API shown in this guide is the same across all our SDK products, including Video Capture SDK .Net, Video Edit SDK .Net, and Media Player SDK .Net.*

DirectShow is a powerful multimedia framework that allows developers to perform complex operations on media streams. One of its key strengths is the ability to work with custom filter interfaces, giving you precise control over media processing. This guide will walk you through implementing and utilizing custom DirectShow filter interfaces in your .NET applications.

## Understanding DirectShow Filters

DirectShow uses a filter-based architecture where each filter performs a specific operation on the media stream. These filters are connected in a graph, creating a pipeline for media processing.

### Key DirectShow Components

- **Filter**: A component that processes media data
- **Pin**: Connection points between filters
- **Filter Graph**: The complete pipeline of connected filters
- **IBaseFilter**: The fundamental interface that all DirectShow filters implement

## Getting Started with Custom Filter Interfaces

To work with DirectShow filters in .NET, you'll need to:

1. Add the proper references to your project
2. Access the filter through appropriate events
3. Cast the filter to the interface you need
4. Implement your custom logic

### Required Project References

To access DirectShow functionality, include the appropriate package in your project:

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="X.X.X" />
```

You can also add the `VisioForge.Core` assembly reference directly to your project.

## Implementing Custom Filter Interface Access

Our SDK provides several events that give you access to filters as they're added to the filter graph. Here's how to use them effectively:

### Accessing Filters in Video Capture SDK

The Video Capture SDK offers the `OnFilterAdded` event that fires whenever a filter is added to the graph. This event provides access to each filter through its event arguments.

```cs
// Subscribe to the OnFilterAdded event
videoCaptureCore.OnFilterAdded += VideoCaptureCore_OnFilterAdded;

// Event handler implementation
private void VideoCaptureCore_OnFilterAdded(object sender, FilterAddedEventArgs eventArgs)
{
    // Access the DirectShow filter interface
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    // Now you can work with the filter through the IBaseFilter interface
    if (baseFilter != null)
    {
        // Custom filter manipulation code goes here
    }
}
```

## Working with IBaseFilter Interface

The `IBaseFilter` interface is the foundation of DirectShow filters. Here's what you can do with it:

### Retrieving Filter Information

```cs
private void GetFilterInfo(IBaseFilter filter)
{
    FilterInfo filterInfo = new FilterInfo();
    int hr = filter.QueryFilterInfo(out filterInfo);
    
    if (hr >= 0)
    {
        Console.WriteLine($"Filter Name: {filterInfo.achName}");
        
        // Don't forget to release the reference to the filter graph
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

### Enumerating Filter Pins

```cs
private void EnumerateFilterPins(IBaseFilter filter)
{
    IEnumPins enumPins;
    int hr = filter.EnumPins(out enumPins);
    
    if (hr >= 0 && enumPins != null)
    {
        IPin[] pins = new IPin[1];
        int fetched;
        
        while (enumPins.Next(1, pins, out fetched) == 0 && fetched > 0)
        {
            PinInfo pinInfo = new PinInfo();
            pins[0].QueryPinInfo(out pinInfo);
            
            Console.WriteLine($"Pin Name: {pinInfo.name}, Direction: {pinInfo.dir}");
            
            // Release pin and info
            if (pinInfo.filter != null)
                Marshal.ReleaseComObject(pinInfo.filter);
                
            Marshal.ReleaseComObject(pins[0]);
        }
        
        Marshal.ReleaseComObject(enumPins);
    }
}
```

## Identifying the Right Filter

When working with the `OnFilterAdded` event, remember that it can be called multiple times as various filters are added to the graph. To work with a specific filter, you'll need to identify it correctly:

```cs
private void VideoCaptureCore_OnFilterAdded(object sender, FilterAddedEventArgs eventArgs)
{
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    if (baseFilter != null)
    {
        FilterInfo filterInfo = new FilterInfo();
        baseFilter.QueryFilterInfo(out filterInfo);
        
        // Check if this is the filter we're looking for
        if (filterInfo.achName == "Video Capture Device")
        {
            // This is our target filter, perform specific operations
            ConfigureVideoCaptureFilter(baseFilter);
        }
        
        // Release the filter graph reference
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

## Advanced Filter Configuration

Once you have access to the filter interface, you can perform advanced configurations:

### Setting Filter Properties

```cs
private void SetFilterProperty(IBaseFilter filter, Guid propertySet, int propertyId, object propertyValue)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    
    if (propertySetInterface != null)
    {
        // Convert property value to byte array
        byte[] propertyData = ConvertToByteArray(propertyValue);
        
        // Set the property
        int hr = propertySetInterface.Set(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length
        );
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
}
```

### Retrieving Filter Properties

```cs
private object GetFilterProperty(IBaseFilter filter, Guid propertySet, int propertyId, Type propertyType)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    object result = null;
    
    if (propertySetInterface != null)
    {
        int dataSize = Marshal.SizeOf(propertyType);
        byte[] propertyData = new byte[dataSize];
        int returnedDataSize;
        
        // Get the property
        int hr = propertySetInterface.Get(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length,
            out returnedDataSize
        );
        
        if (hr >= 0)
        {
            result = ConvertFromByteArray(propertyData, propertyType);
        }
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
    
    return result;
}
```

## Common Use Cases for Custom Filter Interfaces

### Video Processing Filters

When working with video, you might need to access specific properties of camera devices:

```cs
private void ConfigureVideoCaptureFilter(IBaseFilter captureFilter)
{
    // Access and set camera properties
    IAMCameraControl cameraControl = captureFilter as IAMCameraControl;
    
    if (cameraControl != null)
    {
        // Set exposure
        cameraControl.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Manual);
        
        // Set focus
        cameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Manual);
        
        Marshal.ReleaseComObject(cameraControl);
    }
}
```

### Audio Processing Filters

For audio processing, you might want to adjust volume or audio quality settings:

```cs
private void ConfigureAudioFilter(IBaseFilter audioFilter)
{
    // Access volume interface
    IBasicAudio basicAudio = audioFilter as IBasicAudio;
    
    if (basicAudio != null)
    {
        // Set volume (0 to -10000, where 0 is max and -10000 is min)
        basicAudio.put_Volume(-2000); // 80% volume
        
        Marshal.ReleaseComObject(basicAudio);
    }
}
```

## Handling Resources Properly

When working with DirectShow interfaces, it's crucial to properly release COM objects to prevent memory leaks:

```cs
private void ReleaseComObject(object comObject)
{
    if (comObject != null)
    {
        Marshal.ReleaseComObject(comObject);
    }
}
```

## Complete Example

Here's a more complete example that demonstrates finding and configuring a video capture filter:

```cs
using System;
using System.Runtime.InteropServices;
using VisioForge.Core.DirectShow;

public class CustomFilterExample
{
    private VideoCaptureCore captureCore;
    
    public void Initialize()
    {
        captureCore = new VideoCaptureCore();
        captureCore.OnFilterAdded += CaptureCore_OnFilterAdded;
        
        // Configure source
        // ...
        
        // Start capture
        captureCore.Start();
    }
    
    private void CaptureCore_OnFilterAdded(object sender, FilterAddedEventArgs eventArgs)
    {
        IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
        
        if (baseFilter != null)
        {
            // Get filter information
            FilterInfo filterInfo = new FilterInfo();
            baseFilter.QueryFilterInfo(out filterInfo);
            
            Console.WriteLine($"Filter added: {filterInfo.achName}");
            
            // Check if this is the video capture filter
            if (filterInfo.achName.Contains("Video Capture"))
            {
                ConfigureVideoCaptureFilter(baseFilter);
            }
            
            // Release filter graph reference
            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }
        }
    }
    
    private void ConfigureVideoCaptureFilter(IBaseFilter captureFilter)
    {
        // Your filter configuration code here
    }
    
    public void Cleanup()
    {
        if (captureCore != null)
        {
            captureCore.Stop();
            captureCore.OnFilterAdded -= CaptureCore_OnFilterAdded;
            captureCore.Dispose();
            captureCore = null;
        }
    }
}
```

## Required System Components

To use DirectShow functionality in your application, ensure your end-users have the following components installed:

- DirectX Runtime (included with Windows)
- SDK redistributable components

## Conclusion

Working with custom DirectShow filter interfaces gives you powerful capabilities for media processing in your .NET applications. By following the patterns described in this guide, you can access and manipulate the underlying DirectShow components to achieve precise control over your multimedia applications.

For additional assistance with implementing these techniques, please contact our support team. Visit our GitHub repository for more code samples and implementation examples.
