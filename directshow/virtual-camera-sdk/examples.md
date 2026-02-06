---
title: Virtual Camera SDK - Code Examples
description: Virtual Camera SDK code examples for streaming to virtual camera devices and reading from virtual cameras with frame-by-frame rendering.
---

# Virtual Camera SDK - Code Examples

## Overview

This page provides practical code examples for using the Virtual Camera SDK. The SDK enables you to:

- **Write TO virtual camera**: Stream video from files, real cameras, or individual frames to virtual camera devices
- **Read FROM virtual camera**: Capture video from virtual camera devices (appears as regular webcam to applications)
- Apply real-time video effects and processing
- Support multiple virtual camera instances

The virtual camera appears as a standard webcam to applications like Zoom, Teams, OBS, and other video conferencing software.

---
## Architecture Overview
The Virtual Camera SDK provides three main filter types:
1. **CLSID_VFVirtualCameraSource**: Reads FROM virtual camera device (acts as a video capture source)
2. **CLSID_VFVirtualCameraSink**: Writes TO virtual camera device (acts as a renderer)
3. **CLSID_VFVideoPushSource**: Push source for frame-by-frame rendering (image sequences, custom rendering)
**Typical workflows**:
- File/Camera → VirtualCameraSink → Virtual Camera Device → Other Applications
- Virtual Camera Device → VirtualCameraSource → Your Application
- PushSource (frames) → VirtualCameraSink → Virtual Camera Device → Other Applications
---

## Prerequisites

### C# Projects

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```

**Required NuGet Packages**:

- `VisioForge.DirectShowAPI` - DirectShow wrapper library

**Key CLSIDs**:

```csharp
// Filter CLSIDs (available in Consts class)
public static readonly Guid CLSID_VFVirtualCameraSource = new Guid("AA4DA14E-644B-487a-A7CB-517A390B4BB8"); // Read from virtual camera
public static readonly Guid CLSID_VFVirtualCameraSink = new Guid("AA6AB4DF-9670-4913-88BB-2CB381C19340"); // Write to virtual camera
public static readonly Guid CLSID_VFVirtualAudioCardSource = new Guid("B5A463DF-4016-4C34-AA4F-48EC1B51C73F"); // Audio source
public static readonly Guid CLSID_VFVirtualAudioCardSink = new Guid("1A2673B0-553E-4027-AECC-839405468950"); // Audio sink

// Push source for frame-by-frame rendering
public static readonly Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
```

### C++ Projects

```cpp
#include <dshow.h>
#include <streams.h>
#include "ivirtualcamera.h"

#pragma comment(lib, "strmiids.lib")

// Filter CLSIDs
DEFINE_GUID(CLSID_VFVirtualCameraSource,
    0xAA4DA14E, 0x644B, 0x487a, 0xA7, 0xCB, 0x51, 0x7A, 0x39, 0x0B, 0x4B, 0xB8);

DEFINE_GUID(CLSID_VFVirtualCameraSink,
    0xAA6AB4DF, 0x9670, 0x4913, 0x88, 0xBB, 0x2C, 0xB3, 0x81, 0xC1, 0x93, 0x40);

DEFINE_GUID(CLSID_VFVirtualAudioCardSource,
    0xB5A463DF, 0x4016, 0x4C34, 0xAA, 0x4F, 0x48, 0xEC, 0x1B, 0x51, 0xC7, 0x3F);

DEFINE_GUID(CLSID_VFVirtualAudioCardSink,
    0x1A2673B0, 0x553E, 0x4027, 0xAE, 0xCC, 0x83, 0x94, 0x05, 0x46, 0x89, 0x50);

DEFINE_GUID(CLSID_VFVideoPushSource,
    0x38D15306, 0xBBC6, 0x4D6C, 0xA8, 0x9C, 0x96, 0x21, 0x60, 0x4D, 0x9F, 0xC1);
```

---
## Example 1: Stream Video File to Virtual Camera
This example demonstrates streaming a video file to a virtual camera device.
### C# Implementation
```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VirtualCameraFileStreaming
{
    private IFilterGraph2 filterGraphSource;
    private ICaptureGraphBuilder2 captureGraphSource;
    private IMediaControl mediaControlSource;
    private IMediaEventEx mediaEventExSource;
    private IBaseFilter sourceVideoFilter;
    private IBaseFilter sinkVideoFilter;
    private IBaseFilter sinkAudioFilter;
    public void StreamFileToVirtualCamera(string videoFile)
    {
        try
        {
            // Create filter graph
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            mediaEventExSource = (IMediaEventEx)filterGraphSource;
            // Attach the filter graph to the capture graph
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Add Virtual Camera Sink for video
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            // Optional: Set license key for purchased version
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("YOUR-LICENSE-KEY"); // Use "TRIAL" for trial version
            // Add Virtual Camera Sink for audio
            sinkAudioFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualAudioCardSink,
                "VisioForge Virtual Camera Sink - Audio");
            // Add source filter for the video file
            // DirectShow automatically selects appropriate source filter
            filterGraphSource.AddSourceFilter(videoFile, "Source file", out sourceVideoFilter);
            // Render video stream: Source → Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Render audio stream: Source → Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkAudioFilter);
            // Note: Audio errors are not critical, better to check if audio is available
            // Start playback
            hr = mediaControlSource.Run();
            DsError.ThrowExceptionForHR(hr);
            Console.WriteLine("Streaming to virtual camera. Press any key to stop...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }
    public void Cleanup()
    {
        // Stop playback
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Stop receiving events
        mediaEventExSource?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
        // Remove all filters
        FilterGraphTools.RemoveAllFilters(filterGraphSource);
        // Release DirectShow interfaces
        if (mediaControlSource != null)
        {
            Marshal.ReleaseComObject(mediaControlSource);
            mediaControlSource = null;
        }
        if (mediaEventExSource != null)
        {
            Marshal.ReleaseComObject(mediaEventExSource);
            mediaEventExSource = null;
        }
        if (sourceVideoFilter != null)
        {
            Marshal.ReleaseComObject(sourceVideoFilter);
            sourceVideoFilter = null;
        }
        if (sinkVideoFilter != null)
        {
            Marshal.ReleaseComObject(sinkVideoFilter);
            sinkVideoFilter = null;
        }
        if (sinkAudioFilter != null)
        {
            Marshal.ReleaseComObject(sinkAudioFilter);
            sinkAudioFilter = null;
        }
        if (captureGraphSource != null)
        {
            Marshal.ReleaseComObject(captureGraphSource);
            captureGraphSource = null;
        }
        if (filterGraphSource != null)
        {
            Marshal.ReleaseComObject(filterGraphSource);
            filterGraphSource = null;
        }
    }
}
```
### C++ Implementation
```cpp
#include <dshow.h>
#include "ivirtualcamera.h"
HRESULT StreamFileToVirtualCamera(LPCWSTR videoFile)
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    ICaptureGraphBuilder2* pBuild = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IBaseFilter* pSinkVideoFilter = NULL;
    IBaseFilter* pSinkAudioFilter = NULL;
    // Initialize COM
    CoInitialize(NULL);
    // Create the filter graph manager
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    // Create the Capture Graph Builder
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;
    // Set the filter graph
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;
    // Create Virtual Camera Sink filter for video
    hr = CoCreateInstance(CLSID_VFVirtualCameraSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkVideoFilter, L"VisioForge Virtual Camera Sink - Video");
    if (FAILED(hr))
        goto cleanup;
    // Create Virtual Camera Sink filter for audio
    hr = CoCreateInstance(CLSID_VFVirtualAudioCardSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkAudioFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkAudioFilter, L"VisioForge Virtual Camera Sink - Audio");
    if (FAILED(hr))
        goto cleanup;
    // Add source filter for the file
    hr = pGraph->AddSourceFilter(videoFile, L"Source File", &pSourceFilter);
    if (FAILED(hr))
        goto cleanup;
    // Render video stream
    hr = pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    // Render audio stream (errors not critical)
    pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkAudioFilter);
    // Get media control interface
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Start playback
        hr = pControl->Run();
    }
cleanup:
    // Release interfaces
    if (pControl) pControl->Release();
    if (pSinkAudioFilter) pSinkAudioFilter->Release();
    if (pSinkVideoFilter) pSinkVideoFilter->Release();
    if (pSourceFilter) pSourceFilter->Release();
    if (pBuild) pBuild->Release();
    if (pGraph) pGraph->Release();
    return hr;
}
```
---

## Example 2: Stream Physical Camera to Virtual Camera

This example demonstrates capturing from a physical webcam and streaming it to a virtual camera device.

### C# Implementation

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class VirtualCameraFromPhysicalCamera
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IBaseFilter cameraFilter;
    private IBaseFilter virtualCameraSink;

    public void StreamCameraToVirtualCamera(string physicalCameraName)
    {
        try
        {
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            // Attach filter graph to capture graph
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Add physical camera filter
            cameraFilter = FilterGraphTools.AddFilterByName(
                filterGraph,
                FilterCategory.VideoInputDevice,
                physicalCameraName);

            if (cameraFilter == null)
            {
                throw new Exception($"Camera '{physicalCameraName}' not found");
            }

            // Add Virtual Camera Sink
            virtualCameraSink = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSink,
                "Virtual Camera Sink");

            // Optional: Set license
            var sinkIntf = virtualCameraSink as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");

            // Render stream: Physical Camera → Virtual Camera Sink
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                cameraFilter,
                null,
                virtualCameraSink);
            DsError.ThrowExceptionForHR(hr);

            // Start capture
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Streaming physical camera to virtual camera...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (cameraFilter != null)
        {
            Marshal.ReleaseComObject(cameraFilter);
            cameraFilter = null;
        }

        if (virtualCameraSink != null)
        {
            Marshal.ReleaseComObject(virtualCameraSink);
            virtualCameraSink = null;
        }

        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
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

---
## Example 3: Stream Image Sequence to Virtual Camera (Frame-by-Frame)
This example demonstrates rendering individual frames (image sequence or slideshow) to a virtual camera device.
### C# Implementation
```csharp
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VirtualCameraFrameByFrame
{
    private IFilterGraph2 filterGraphSource;
    private ICaptureGraphBuilder2 captureGraphSource;
    private IMediaControl mediaControlSource;
    private IBaseFilter sourceVideoFilter;
    private IBaseFilter sinkVideoFilter;
    private IVFLiveVideoSource pushSource;
    private System.Windows.Forms.Timer framePushTimer;
    private int currentFrameIndex = 0;
    private Bitmap[] frames;
    public void StreamImageSequenceToVirtualCamera(string[] imageFiles, float frameRate = 10)
    {
        try
        {
            // Load images into memory
            frames = new Bitmap[imageFiles.Length];
            for (int i = 0; i < imageFiles.Length; i++)
            {
                frames[i] = new Bitmap(imageFiles[i]);
            }
            if (frames.Length == 0)
            {
                throw new Exception("No images to display");
            }
            int width = frames[0].Width;
            int height = frames[0].Height;
            // Create filter graph
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Add Virtual Camera Sink
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");
            // Add push source filter
            Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
            sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                CLSID_VFVideoPushSource,
                "VisioForge Video Push Source");
            if (sourceVideoFilter == null)
            {
                throw new Exception("Unable to create VisioForge Push Source filter.");
            }
            // Get IVFLiveVideoSource interface
            pushSource = sourceVideoFilter as IVFLiveVideoSource;
            if (pushSource == null)
            {
                throw new Exception("Unable to get IVFLiveVideoSource interface.");
            }
            // Configure bitmap format
            var bmiHeader = new BitmapInfoHeader
            {
                BitCount = 24,
                Compression = 0,
                Width = width,
                Height = height,
                Planes = 1,
                Size = Marshal.SizeOf(typeof(BitmapInfoHeader)),
                ImageSize = GetStrideRGB24(width) * height
            };
            pushSource.SetBitmapInfo(bmiHeader);
            pushSource.SetFrameRate(frameRate);
            // Connect filters: Push Source → Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Start the graph
            hr = mediaControlSource.Run();
            DsError.ThrowExceptionForHR(hr);
            // Setup timer to push frames
            framePushTimer = new System.Windows.Forms.Timer();
            framePushTimer.Interval = (int)(1000 / frameRate);
            framePushTimer.Tick += PushFrame;
            framePushTimer.Start();
            Console.WriteLine("Streaming image sequence to virtual camera...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }
    private void PushFrame(object sender, EventArgs e)
    {
        if (frames == null || frames.Length == 0 || pushSource == null)
            return;
        // Get current frame
        Bitmap frame = frames[currentFrameIndex];
        // Lock bitmap data
        BitmapData bmpData = frame.LockBits(
            new Rectangle(0, 0, frame.Width, frame.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);
        try
        {
            // Push frame to virtual camera
            pushSource.AddFrame(bmpData.Scan0);
        }
        finally
        {
            frame.UnlockBits(bmpData);
        }
        // Move to next frame (loop)
        currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
    }
    private int GetStrideRGB24(int width)
    {
        return ((width * 24 + 31) / 32) * 4;
    }
    public void Cleanup()
    {
        // Stop timer
        if (framePushTimer != null)
        {
            framePushTimer.Stop();
            framePushTimer.Dispose();
            framePushTimer = null;
        }
        // Stop playback
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Release frames
        if (frames != null)
        {
            foreach (var frame in frames)
            {
                frame?.Dispose();
            }
            frames = null;
        }
        // Release DirectShow interfaces
        pushSource = null;
        FilterGraphTools.RemoveAllFilters(filterGraphSource);
        if (sourceVideoFilter != null)
        {
            Marshal.ReleaseComObject(sourceVideoFilter);
            sourceVideoFilter = null;
        }
        if (sinkVideoFilter != null)
        {
            Marshal.ReleaseComObject(sinkVideoFilter);
            sinkVideoFilter = null;
        }
        if (mediaControlSource != null)
        {
            Marshal.ReleaseComObject(mediaControlSource);
            mediaControlSource = null;
        }
        if (captureGraphSource != null)
        {
            Marshal.ReleaseComObject(captureGraphSource);
            captureGraphSource = null;
        }
        if (filterGraphSource != null)
        {
            Marshal.ReleaseComObject(filterGraphSource);
            filterGraphSource = null;
        }
    }
}
```
---

## Example 4: Read From Virtual Camera

This example demonstrates capturing video from a virtual camera device (useful for testing or monitoring what's being sent to the virtual camera).

### C# Implementation

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using MediaFoundation;
using MediaFoundation.EVR;

public class VirtualCameraCapture
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IBaseFilter virtualCameraSource;
    private IBaseFilter videoRenderer;

    public void CaptureFromVirtualCamera(IntPtr videoWindowHandle)
    {
        try
        {
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Add Virtual Camera Source filter
            virtualCameraSource = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSource,
                "VisioForge Virtual Camera");

            if (virtualCameraSource == null)
            {
                throw new Exception("Unable to create Virtual Camera Source filter");
            }

            // Optional: Set license
            var cameraIntf = virtualCameraSource as IVFVirtualCameraSource;
            cameraIntf?.set_license("TRIAL");

            // Create Enhanced Video Renderer (EVR)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configure EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Set video window
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

            // Render stream: Virtual Camera Source → EVR
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                virtualCameraSource,
                null,
                videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            // Start playback
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine("Capturing from virtual camera...");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (virtualCameraSource != null)
        {
            Marshal.ReleaseComObject(virtualCameraSource);
            virtualCameraSource = null;
        }

        if (videoRenderer != null)
        {
            Marshal.ReleaseComObject(videoRenderer);
            videoRenderer = null;
        }

        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
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

---
## Additional Resources
For more detailed information, see:
- [Virtual Camera SDK Product Page](https://www.visioforge.com/virtual-camera-sdk)
- [End User License Agreement](../../eula.md)
- [Sample Code Repository](https://github.com/visioforge/directshow-samples/tree/main/Virtual%20Camera%20SDK)
## Support
- **Technical Support**: https://support.visioforge.com/
- **Discord Community**: https://discord.com/invite/yvXUG56WCH