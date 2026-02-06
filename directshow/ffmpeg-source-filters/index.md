---
title: FFMPEG Source DirectShow Filter for Multimedia Apps
description: FFMPEG DirectShow filter for decoding 100+ formats including MP4, MKV, H.265 with hardware acceleration in C++, C#, and Delphi apps.
sidebar_label: FFMPEG Source DirectShow Filter
order: 10
---

# FFMPEG Source DirectShow Filter

## Introduction

The FFMPEG Source DirectShow filter enables developers to seamlessly integrate advanced media decoding and playback capabilities into any DirectShow-compatible application. This powerful component bridges the gap between complex multimedia formats and your software development needs, providing a robust foundation for building media-rich applications.

---

## Installation

Before using the code samples and integrating the filter into your application, you must first install the FFMPEG Source DirectShow Filter from the [product page](https://www.visioforge.com/ffmpeg-source-directshow-filter).

**Installation Steps**:

1. Download the SDK installer from the product page
2. Run the installer with administrative privileges
3. The installer will register the FFMPEG Source filter and deploy all necessary FFMPEG DLLs
4. Sample applications and source code will be available in the installation directory

**Note**: The filter must be properly registered on the system before it can be used in your applications. The installer handles this automatically.

---

## Key Features and Capabilities

Our filter comes bundled with all necessary FFMPEG DLLs and provides a feature-rich DirectShow filter interface that supports:

- **Extensive Format Compatibility**: Handle a wide range of video and audio formats including MP4, MKV, AVI, MOV, WMV, FLV, and many others without additional codec installations
- **Network Stream Support**: Connect to RTSP, RTMP, HTTP, UDP, and TCP streams for live media integration
- **Multiple Stream Management**: Select between video and audio streams in multi-stream media files
- **Advanced Seeking Capabilities**: Implement precise seeking functionality in your applications
- **GPU Acceleration**: Utilize hardware acceleration for optimal performance

## Implementation Examples

The SDK includes comprehensive sample applications for multiple development environments:

### Delphi Integration (Primary)

```delphi
// Initialize the FFMPEG Source filter in Delphi using DSPack
procedure TMainForm.InitializeFFMPEGSource;
var
  FFMPEGFilter: IBaseFilter;
  FileSource: IFileSourceFilter;
begin
  // Create FFMPEG Source filter instance
  // IMPORTANT: Ensure proper COM initialization before this call
  CoCreateInstance(CLSID_FFMPEGSource, nil, CLSCTX_INPROC_SERVER, 
                  IID_IBaseFilter, FFMPEGFilter);
  
  // Query for file source interface
  FFMPEGFilter.QueryInterface(IID_IFileSourceFilter, FileSource);
  
  // Load media file - can be local or network URL
  FileSource.Load('C:\media\sample.mp4', nil);
  
  // Add to filter graph for rendering
  FilterGraph.AddFilter(FFMPEGFilter, 'FFMPEG Source');
  
  // Connect to appropriate renderers or processing filters
  // FilterGraph.RenderStream(...);
end;
```

### C# Integration (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Initialize the FFMPEG Source filter in C# using DirectShowLib
public class FFMPEGSourcePlayer
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaSeeking mediaSeeking;
    private IMediaEventEx mediaEventEx;
    private IBaseFilter sourceFilter;
    private IBaseFilter videoRenderer;

    public void Initialize(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Create the filter graph manager
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;
            mediaSeeking = (IMediaSeeking)filterGraph;
            mediaEventEx = (IMediaEventEx)filterGraph;

            // Attach the filter graph to the capture graph
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Create the FFMPEG Source filter using the correct CLSID
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Optional: Register purchased version
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("your-license-key-here");

            // Configure filter settings
            var filterConfig = sourceFilter as IFFMPEGSourceSettings;
            if (filterConfig != null)
            {
                // Set buffering mode (AUTO, ON, or OFF)
                filterConfig.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);

                // Enable hardware acceleration (GPU decoding)
                filterConfig.SetHWAccelerationEnabled(true);

                // Set connection timeout (milliseconds)
                filterConfig.SetLoadTimeOut(30000);
            }

            // Load the media file or network stream
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Create video renderer (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configure EVR
            var evrConfig = videoRenderer as MediaFoundation.EVR.IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Set video window for rendering
            var getService = videoRenderer as MediaFoundation.IMFGetService;
            if (getService != null)
            {
                getService.GetService(
                    MediaFoundation.MFServices.MR_VIDEO_RENDER_SERVICE,
                    typeof(MediaFoundation.IMFVideoDisplayControl).GUID,
                    out var videoDisplayControlObj);

                var videoDisplayControl = videoDisplayControlObj as MediaFoundation.IMFVideoDisplayControl;
                videoDisplayControl?.SetVideoWindow(videoWindowHandle);
            }

            // Render the streams
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
            // Note: Audio rendering errors are not critical for video-only playback

            // Start playback
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing FFMPEG Source: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        // Stop playback
        if (mediaControl != null)
        {
            mediaControl.StopWhenReady();
            mediaControl.Stop();
        }

        // Stop receiving events
        mediaEventEx?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);

        // Remove all filters
        FilterGraphTools.RemoveAllFilters(filterGraph);

        // Release DirectShow interfaces
        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
        }

        if (mediaSeeking != null)
        {
            Marshal.ReleaseComObject(mediaSeeking);
            mediaSeeking = null;
        }

        if (mediaEventEx != null)
        {
            Marshal.ReleaseComObject(mediaEventEx);
            mediaEventEx = null;
        }

        if (sourceFilter != null)
        {
            Marshal.ReleaseComObject(sourceFilter);
            sourceFilter = null;
        }

        if (videoRenderer != null)
        {
            Marshal.ReleaseComObject(videoRenderer);
            videoRenderer = null;
        }

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }
    }
}
```

**Key CLSIDs and GUIDs:**

```csharp
// FFMPEG Source Filter CLSID
public static readonly Guid CLSID_VFFFMPEGSource = new Guid("C5255DE3-50A7-4714-B763-D99E96E4CD52");

// IFFMPEGSourceSettings Interface IID
[Guid("1974D893-83E4-4F89-9908-795C524CC17E")]
public interface IFFMPEGSourceSettings { /* ... */ }

// IVFRegister Interface IID (for licensing)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Required NuGet Packages:**

- `VisioForge.DirectShowAPI` - DirectShow wrapper library
- `MediaFoundation.Net` - Media Foundation wrapper for EVR renderer

**Stream Selection Example:**

```csharp
// Select specific video or audio streams in multi-stream files
var streamSelect = sourceFilter as IAMStreamSelect;
if (streamSelect != null)
{
    streamSelect.Count(out int streamCount);

    for (int i = 0; i < streamCount; i++)
    {
        streamSelect.Info(i, out var mediaType, out _, out _, out _, out var name, out _, out _);

        if (mediaType.majorType == MediaType.Video)
        {
            // Enable the first video stream
            streamSelect.Enable(i, AMStreamSelectEnableFlags.Enable);
            break;
        }
    }
}
```

### C++ Integration Example

```cpp
// Initialize the FFMPEG Source filter in C++ using DirectShow
HRESULT InitializeFFMPEGSource()
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pFFMPEGSource = NULL;
    IFileSourceFilter* pFileSource = NULL;
    
    // Initialize COM
    CoInitialize(NULL);
    
    // Create the filter graph manager
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    
    // Create the FFMPEG Source filter
    hr = CoCreateInstance(CLSID_FFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pFFMPEGSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Add the filter to the graph
    hr = pGraph->AddFilter(pFFMPEGSource, L"FFMPEG Source");
    if (FAILED(hr))
        goto cleanup;
    
    // Get the IFileSourceFilter interface
    hr = pFFMPEGSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Load the media file
    hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
    if (FAILED(hr))
        goto cleanup;
    
    // Render the output pins of the FFMPEG Source filter
    hr = pGraph->Render(GetPin(pFFMPEGSource, PINDIR_OUTPUT, 0));
    
    // Get the media control interface for playback control
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Start playback
        hr = pControl->Run();
        // ... handle playback as needed
    }
    
cleanup:
    // Release interfaces
    if (pControl) pControl->Release();
    if (pFileSource) pFileSource->Release();
    if (pFFMPEGSource) pFFMPEGSource->Release();
    if (pGraph) pGraph->Release();
    
    return hr;
}

// Helper function to get pins from a filter
IPin* GetPin(IBaseFilter* pFilter, PIN_DIRECTION PinDir, int nPin)
{
    IEnumPins* pEnum = NULL;
    IPin* pPin = NULL;
    
    if (pFilter)
    {
        pFilter->EnumPins(&pEnum);
        if (pEnum)
        {
            while (pEnum->Next(1, &pPin, NULL) == S_OK)
            {
                PIN_DIRECTION PinDirThis;
                pPin->QueryDirection(&PinDirThis);
                if (PinDir == PinDirThis)
                {
                    if (nPin == 0)
                        break;
                    nPin--;
                }
                pPin->Release();
                pPin = NULL;
            }
            pEnum->Release();
        }
    }
    
    return pPin;
}
```

## Integration with Processing Filters

Enhance your media pipeline by connecting the FFMPEG Source filter with additional processing components:

- Apply real-time video effects and transformations
- Process audio streams for custom sound manipulation
- Implement specialized media analysis features

Our [Processing Filters pack](https://www.visioforge.com/processing-filters-pack) offers additional capabilities, or you can integrate with any standard DirectShow-compatible filters.

## Technical Specifications

### Supported DirectShow Interfaces

The filter implements these standard DirectShow interfaces for maximum compatibility:

- **IAMStreamSelect**: Select between multiple video and audio streams
- **IAMStreamConfig**: Configure video and audio settings
- **IFileSourceFilter**: Set filename or streaming URL
- **IMediaSeeking**: Implement precise seeking functionality
- **ISpecifyPropertyPages**: Access configuration through property pages

## Version History and Updates

### Version 15.0

- Enhanced FFMPEG libraries with latest codecs
- Added GPU decoding support for improved performance
- Optimized memory management for large files

### Version 12.0

- Updated FFMPEG libraries
- Improved compatibility with Windows 10/11

### Version 11.0

- Updated FFMPEG libraries
- Fixed seeking issues with certain file formats

### Version 10.0

- Updated FFMPEG libraries
- Added support for additional container formats

### Version 9.0

- Updated FFMPEG libraries
- Performance optimizations

### Version 8.0

- Updated FFMPEG libraries
- Improved error handling

### Version 7.0

- Initial release as an independent product
- Core functionality established

## Additional Resources

- Explore our [product page](https://www.visioforge.com/ffmpeg-source-directshow-filter) for detailed specifications
- View our [End User License Agreement](../../eula.md) for licensing details
- Check our developer documentation for advanced implementation scenarios
