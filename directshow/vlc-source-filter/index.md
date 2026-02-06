---
title: VLC Source DirectShow Filter for Media Playback
description: VLC-powered DirectShow filter for playing 200+ formats, 4K/8K video, RTSP/HLS streams with hardware decoding for media applications.
sidebar_label: VLC Source DirectShow Filter
order: 9
---

# VLC Source DirectShow Filter

## Overview

The VLC Source DirectShow filter empowers developers to seamlessly integrate advanced media playback capabilities into any DirectShow-based application. This powerful component enables smooth playback of various video files and network streams across multiple formats and protocols. 

Our SDK package delivers a complete solution with all necessary VLC player DLLs bundled alongside a flexible DirectShow filter. The package provides both standard file-selection interfaces and extensive options for custom filter configurations to match your specific development requirements.

For complete product details and licensing options, visit the [product page](https://www.visioforge.com/vlc-source-directshow-filter).

---

## Installation

Before using the code samples and integrating the filter into your application, you must first install the VLC Source DirectShow Filter from the [product page](https://www.visioforge.com/vlc-source-directshow-filter).

**Installation Steps**:

1. Download the SDK installer from the product page
2. Run the installer with administrative privileges
3. The installer will register the VLC Source filter and deploy all necessary VLC DLLs
4. Sample applications and source code will be available in the installation directory

**Note**: The filter must be properly registered on the system before it can be used in your applications. The installer handles this automatically.

---

## Technical Specifications

### Supported DirectShow Interfaces

The filter implements these standard DirectShow interfaces for maximum compatibility:

- **IAMStreamSelect** - Comprehensive video and audio stream selection capabilities
- **IAMStreamConfig** - Advanced video and audio configuration settings
- **IFileSourceFilter** - Flexible specification of filename or URL sources
- **IMediaSeeking** - Robust timeline seeking and positioning support

### Key Features

- Hardware-accelerated decoding for optimal performance
- Support for 4K and 8K video playback
- Extensive format compatibility including modern codecs
- Network stream handling (RTSP, HLS, DASH, etc.)
- Subtitle rendering and management
- Multi-language audio track support
- 360° video playback capabilities
- HDR content support

## Implementation Examples

### C++ Integration Example

```cpp
#include <dshow.h>
#include <mfapi.h>
#include <evr.h>
#include "ivlcsrc.h"

// VLC Source Filter CLSID
DEFINE_GUID(CLSID_VlcSource,
    0x9f73dcd4, 0x2fc8, 0x4e89, 0x8f, 0xc3, 0x2d, 0xf1, 0x69, 0x3c, 0xa0, 0x3e);

HRESULT InitializeVLCSource(HWND hVideoWindow)
{
    HRESULT hr = S_OK;
    IFilterGraph2* pGraph = NULL;
    ICaptureGraphBuilder2* pBuild = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pVLCSource = NULL;
    IBaseFilter* pVideoRenderer = NULL;
    IFileSourceFilter* pFileSource = NULL;

    // Initialize COM
    CoInitialize(NULL);

    // Create the filter graph manager
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IFilterGraph2, (void**)&pGraph);
    if (FAILED(hr))
        return hr;

    // Create the Capture Graph Builder
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;

    // Set the filter graph to the capture graph builder
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;

    // Create the VLC Source filter
    hr = CoCreateInstance(CLSID_VlcSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVLCSource);
    if (FAILED(hr))
        goto cleanup;

    // Add the filter to the graph
    hr = pGraph->AddFilter(pVLCSource, L"VLC Source");
    if (FAILED(hr))
        goto cleanup;

    // Load the media file using IFileSourceFilter interface
    hr = pVLCSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr) && pFileSource)
    {
        hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
        pFileSource->Release();
        if (FAILED(hr))
            goto cleanup;
    }

    // Create Enhanced Video Renderer (EVR)
    CLSID CLSID_EVR = { 0xFA10746C, 0x9B63, 0x4B6C,
        { 0xBC, 0x49, 0xFC, 0x30, 0x0E, 0xA5, 0xF2, 0x56 } };
    hr = CoCreateInstance(CLSID_EVR, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pGraph->AddFilter(pVideoRenderer, L"EVR");
    if (FAILED(hr))
        goto cleanup;

    // Configure EVR
    IEVRFilterConfig* pConfig = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IEVRFilterConfig, (void**)&pConfig);
    if (SUCCEEDED(hr) && pConfig)
    {
        pConfig->SetNumberOfStreams(1);
        pConfig->Release();
    }

    // Set video window
    IMFGetService* pGetService = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IMFGetService, (void**)&pGetService);
    if (SUCCEEDED(hr) && pGetService)
    {
        IMFVideoDisplayControl* pDisplayControl = NULL;
        hr = pGetService->GetService(MR_VIDEO_RENDER_SERVICE,
                                     IID_IMFVideoDisplayControl,
                                     (void**)&pDisplayControl);
        if (SUCCEEDED(hr) && pDisplayControl)
        {
            pDisplayControl->SetVideoWindow(hVideoWindow);
            pDisplayControl->Release();
        }
        pGetService->Release();
    }

    // Render the streams
    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pVLCSource, NULL, pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pVLCSource, NULL, NULL);
    // Audio errors are not critical

    // Get the media control interface for playback control
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Start playback
        hr = pControl->Run();
    }

cleanup:
    // Release interfaces
    if (pControl) pControl->Release();
    if (pVideoRenderer) pVideoRenderer->Release();
    if (pVLCSource) pVLCSource->Release();
    if (pBuild) pBuild->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

### C# Integration (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using MediaFoundation;
using MediaFoundation.EVR;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Initialize the VLC Source filter in C# using DirectShowLib
public class VLCSourcePlayer
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

            // Create the VLC Source filter using the correct CLSID
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Optional: Register purchased version
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("your-license-key-here");

            // Load the media file or URL using IFileSourceFilter interface
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Create video renderer (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configure EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
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
            Console.WriteLine($"Error initializing VLC Source: {ex.Message}");
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

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }
    }
}
```

**Key CLSIDs and GUIDs:**

```csharp
// VLC Source Filter CLSID
public static readonly Guid CLSID_VFVLCSource = new Guid("9F73DCD4-2FC8-4E89-8FC3-2DF1693CA03E");

// IVlcSrc Interface IID
[Guid("77493EB7-6D00-41C5-9535-7C593824E892")]
public interface IVlcSrc { /* ... */ }

// IVlcSrc2 Interface IID (for custom command-line parameters)
[Guid("CCE122C0-172C-4626-B4B6-42B039E541CB")]
public interface IVlcSrc2 : IVlcSrc { /* ... */ }

// IVlcSrc3 Interface IID (for frame rate override)
[Guid("3DFBED0C-E4A8-401C-93EF-CBBFB65223DD")]
public interface IVlcSrc3 : IVlcSrc2 { /* ... */ }

// IVFRegister Interface IID (for licensing)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Required NuGet Packages:**

- `VisioForge.DirectShowAPI` - DirectShow wrapper library
- `MediaFoundation.Net` - Media Foundation wrapper for EVR renderer

**Audio Track Selection Example:**

```csharp
// Enumerate and select audio tracks
var vlcSrc = sourceFilter as IVlcSrc;
if (vlcSrc != null)
{
    int audioCount = 0;
    vlcSrc.GetAudioTracksCount(out audioCount);

    for (int i = 0; i < audioCount; i++)
    {
        int trackId;
        var trackName = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);

        Console.WriteLine($"Track {i}: {trackName} (ID: {trackId})");
    }

    // Select specific audio track
    if (audioCount > 1)
    {
        int desiredTrackId;
        var name = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
        vlcSrc.SetAudioTrack(desiredTrackId);
    }
}
```

**Custom VLC Options Example:**

```csharp
// Configure VLC for low-latency RTSP streaming
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    string[] parameters = new string[]
    {
        "--network-caching=300",     // Low network buffer
        "--rtsp-tcp",                // Force TCP for RTSP
        "--avcodec-hw=any",          // Enable hardware decoding
        "--live-caching=300"         // Low live stream buffer
    };

    vlcSrc2.SetCustomCommandLine(parameters, parameters.Length);

    // Then load the stream
    var vlcSrc = vlcSrc2 as IVlcSrc;
    vlcSrc?.SetFile("rtsp://192.168.1.100/stream");
}
```

## Version History

### Version 15.0

- Enhanced playback quality across numerous formats
- Improved subtitle rendering engine
- Updated codec implementations including dav1d, ffmpeg, and libvpx
- Added Super Resolution scaling with nVidia and Intel GPU acceleration

### Version 14.0

- Updated to VLC v3.0.18 core
- Fixed DxVA/D3D11 compatibility issues with HEVC content
- Resolved OpenGL resizing problems for smoother playback

### Version 12.0

- Upgraded to VLC v3.0.16 engine
- Added support for new Fourcc formats (E-AC3 and AV1)
- Fixed stability issues with VP9 streams

### Version 11.1

- Incorporated VLC v3.0.11
- Optimized HLS playlist update mechanism
- Enhanced WebVTT subtitle handling and display

### Version 11.0

- Built on VLC v3.0.10 foundation
- Fixed critical regression issues with HLS streams

### Version 10.4

- Major update to VLC 3.0 architecture
- Enabled hardware decoding by default for 4K and 8K content
- Added 10-bit color depth and HDR support
- Implemented 360-degree video and 3D audio capabilities
- Introduced Blu-Ray Java menu support

### Version 10.0

- Initial release as a standalone DirectShow filter
- For earlier version history, please refer to Video Capture SDK .Net changelog

## Additional Resources

- [End User License Agreement](../../eula.md)
- [Code Samples](https://github.com/visioforge/)
