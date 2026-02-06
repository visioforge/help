---
title: Code Examples
description: Code examples for FFMPEG Source Filter in C++, C#, and VB.NET with DirectShow graphs, hardware acceleration, and network streaming.
---

# Code Examples

## Overview

This page provides practical code examples for using the FFMPEG Source Filter in DirectShow applications. Examples are provided in C++, C#, and VB.NET.

## Complete Working Samples

**Official GitHub Repository**: All examples shown on this page are available as complete, working Visual Studio projects in our GitHub samples repository:

🔗 **[DirectShow Samples Repository](https://github.com/visioforge/directshow-samples)**

### FFMPEG Source Filter Samples

- **[C# Sample](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** - Full-featured media player with all filter capabilities
- **[VB.NET Sample](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** - VB.NET implementation
- **[C++Builder Sample](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** - C++ implementation

Each sample includes:
- Complete Visual Studio/C++Builder project files
- Working code for playback, stream selection, and configuration
- Hardware acceleration examples
- Network streaming (RTSP/HLS) examples

---
## Prerequisites
### C++ Projects
```cpp
#include <dshow.h>
#include <streams.h>
#include "IFFMPEGSourceSettings.h"  // From SDK
#pragma comment(lib, "strmiids.lib")
```
### C# Projects
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
```
**NuGet Packages**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Example 1: Basic File Playback

Play a local media file with default settings.

### C++ Implementation

```cpp
#include <dshow.h>
#include <windows.h>
#include "IFFMPEGSourceSettings.h"

// CLSID for FFMPEG Source Filter
DEFINE_GUID(CLSID_VFFFMPEGSource,
    0x1974d893, 0x83e4, 0x4f89, 0x99, 0x8, 0x79, 0x5c, 0x52, 0x4c, 0xc1, 0x7e);

HRESULT PlayFile(LPCWSTR filename, HWND hVideoWindow)
{
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IMediaEventEx* pEvent = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IFileSourceFilter* pFileSource = NULL;
    IVideoWindow* pVideoWindow = NULL;

    HRESULT hr = S_OK;

    // Create Filter Graph
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Create FFMPEG Source Filter
    hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Add filter to graph
    hr = pGraph->AddFilter(pSourceFilter, L"FFMPEG Source");
    if (FAILED(hr)) goto cleanup;

    // Load file
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr)) goto cleanup;

    hr = pFileSource->Load(filename, NULL);
    if (FAILED(hr)) goto cleanup;

    // Render streams automatically
    hr = pGraph->QueryInterface(IID_IGraphBuilder, (void**)&pGraph);

    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Render video stream
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);

        // Render audio stream
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Set video window
    hr = pGraph->QueryInterface(IID_IVideoWindow, (void**)&pVideoWindow);
    if (SUCCEEDED(hr))
    {
        pVideoWindow->put_Owner((OAHWND)hVideoWindow);
        pVideoWindow->put_WindowStyle(WS_CHILD | WS_CLIPSIBLINGS);

        RECT rc;
        GetClientRect(hVideoWindow, &rc);
        pVideoWindow->SetWindowPosition(0, 0, rc.right, rc.bottom);
    }

    // Get control interface
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (FAILED(hr)) goto cleanup;

    // Run the graph
    hr = pControl->Run();

cleanup:
    if (pFileSource) pFileSource->Release();
    if (pVideoWindow) pVideoWindow->Release();
    if (pControl) pControl->Release();
    if (pSourceFilter) pSourceFilter->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

### C# Implementation

```csharp
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class FFMPEGSourceBasicExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IVideoWindow videoWindow;
    private IBaseFilter sourceFilter;

    public void PlayFile(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Create filter graph
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;

            // Create and add FFMPEG Source filter
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Load file
            var fileSource = sourceFilter as IFileSourceFilter;
            int hr = fileSource.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Render streams
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Render video
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            // Render audio
            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

            // Set video window
            videoWindow.put_Owner(videoWindowHandle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.put_Visible(OABool.True);

            // Run graph
            mediaControl.Run();

            Marshal.ReleaseComObject(captureGraph);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Playback Error");
        }
    }

    public void Stop()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }

        if (videoWindow != null)
        {
            videoWindow.put_Visible(OABool.False);
            videoWindow.put_Owner(IntPtr.Zero);
        }

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (sourceFilter != null) Marshal.ReleaseComObject(sourceFilter);
        if (videoWindow != null) Marshal.ReleaseComObject(videoWindow);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```

### VB.NET Implementation

```vbnet
Imports System.Runtime.InteropServices
Imports VisioForge.DirectShowAPI
Imports VisioForge.DirectShowLib

Public Class FFMPEGSourceBasicExample
    Private filterGraph As IFilterGraph2
    Private mediaControl As IMediaControl
    Private videoWindow As IVideoWindow
    Private sourceFilter As IBaseFilter

    Public Sub PlayFile(filename As String, videoWindowHandle As IntPtr)
        Try
            ' Create filter graph
            filterGraph = DirectCast(New FilterGraph(), IFilterGraph2)
            mediaControl = DirectCast(filterGraph, IMediaControl)
            videoWindow = DirectCast(filterGraph, IVideoWindow)

            ' Create and add FFMPEG Source filter
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source")

            ' Load file
            Dim fileSource = DirectCast(sourceFilter, IFileSourceFilter)
            Dim hr As Integer = fileSource.Load(filename, Nothing)
            DsError.ThrowExceptionForHR(hr)

            ' Render streams
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            hr = captureGraph.SetFiltergraph(filterGraph)
            DsError.ThrowExceptionForHR(hr)

            ' Render video and audio
            captureGraph.RenderStream(Nothing, MediaType.Video, sourceFilter, Nothing, Nothing)
            captureGraph.RenderStream(Nothing, MediaType.Audio, sourceFilter, Nothing, Nothing)

            ' Set video window
            videoWindow.put_Owner(videoWindowHandle)
            videoWindow.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipSiblings)
            videoWindow.put_Visible(OABool.True)

            ' Run graph
            mediaControl.Run()

            Marshal.ReleaseComObject(captureGraph)
        Catch ex As Exception
            MessageBox.Show($"Error: {ex.Message}", "Playback Error")
        End Try
    End Sub

    Public Sub [Stop]()
        If mediaControl IsNot Nothing Then
            mediaControl.Stop()
        End If

        If videoWindow IsNot Nothing Then
            videoWindow.put_Visible(OABool.False)
            videoWindow.put_Owner(IntPtr.Zero)
        End If

        FilterGraphTools.RemoveAllFilters(filterGraph)

        If sourceFilter IsNot Nothing Then Marshal.ReleaseComObject(sourceFilter)
        If videoWindow IsNot Nothing Then Marshal.ReleaseComObject(videoWindow)
        If mediaControl IsNot Nothing Then Marshal.ReleaseComObject(mediaControl)
        If filterGraph IsNot Nothing Then Marshal.ReleaseComObject(filterGraph)
    End Sub
End Class
```

---
## Example 2: Hardware Acceleration
Enable GPU decoding for better performance.
### C++ with Hardware Acceleration
```cpp
HRESULT PlayFileWithGPU(LPCWSTR filename)
{
    IBaseFilter* pSourceFilter = NULL;
    IFFMPEGSourceSettings* pSettings = NULL;
    // Create source filter
    HRESULT hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) return hr;
    // Get configuration interface
    hr = pSourceFilter->QueryInterface(IID_IFFMPEGSourceSettings, (void**)&pSettings);
    if (FAILED(hr))
    {
        pSourceFilter->Release();
        return hr;
    }
    // Enable hardware acceleration (NVDEC/QuickSync/DXVA)
    hr = pSettings->SetHWAccelerationEnabled(TRUE);
    if (FAILED(hr))
    {
        pSettings->Release();
        pSourceFilter->Release();
        return hr;
    }
    // Load file
    IFileSourceFilter* pFileSource = NULL;
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr))
    {
        hr = pFileSource->Load(filename, NULL);
        pFileSource->Release();
    }
    // Continue building graph...
    // (Add to graph, render streams, etc.)
    pSettings->Release();
    pSourceFilter->Release();
    return hr;
}
```
### C# with Hardware Acceleration
```csharp
public void PlayFileWithHardwareAcceleration(string filename, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    // Create source filter
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Configure hardware acceleration
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Enable GPU decoding
        int hr = settings.SetHWAccelerationEnabled(true);
        DsError.ThrowExceptionForHR(hr);
    }
    // Load file
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Build and run graph...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Example 3: Network Streaming (RTSP/HLS)

Stream video from network sources.

### C# RTSP Streaming

```csharp
public void PlayRTSPStream(string rtspUrl, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    // Configure for network streaming
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Set buffering mode for network streams
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Set connection timeout (in seconds)
        settings.SetLoadTimeOut(30);

        // Enable hardware decoding for performance
        settings.SetHWAccelerationEnabled(true);
    }

    // Load RTSP stream
    // Example: "rtsp://camera.example.com:554/stream"
    var fileSource = sourceFilter as IFileSourceFilter;
    int hr = fileSource.Load(rtspUrl, null);
    DsError.ThrowExceptionForHR(hr);

    // Build graph
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    // Setup video window
    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
    videoWindow.put_Visible(OABool.True);

    // Run
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

### C# HLS Streaming

```csharp
public void PlayHLSStream(string hlsUrl, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // HLS streams benefit from buffering
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Longer timeout for HLS playlist loading
        settings.SetLoadTimeOut(60);

        // Hardware acceleration
        settings.SetHWAccelerationEnabled(true);
    }

    // Load HLS stream
    // Example: "https://example.com/stream/playlist.m3u8"
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(hlsUrl, null);

    // Build and run graph (same as RTSP example)
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Example 4: Custom FFmpeg Options
Pass custom FFmpeg demuxer/decoder options.
### C# with Custom Options
```csharp
public void PlayWithCustomOptions(string filename, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Set custom FFmpeg options
        // Format: "key=value" for each option
        // Example 1: Set buffer size for network streams
        settings.SetCustomOption("buffer_size", "1024000");
        // Example 2: Enable low latency mode
        settings.SetCustomOption("fflags", "nobuffer");
        // Example 3: Set analyzer duration (microseconds)
        settings.SetCustomOption("analyzeduration", "1000000");
        // Example 4: Set probe size
        settings.SetCustomOption("probesize", "5000000");
        // Example 5: RTSP transport protocol
        settings.SetCustomOption("rtsp_transport", "tcp");
        // Example 6: Set timeout (microseconds)
        settings.SetCustomOption("timeout", "5000000");
    }
    // Load file
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Build graph...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
### Common FFmpeg Options
```csharp
// Network streaming options
settings.SetCustomOption("rtsp_transport", "tcp");        // Use TCP for RTSP
settings.SetCustomOption("rtsp_flags", "prefer_tcp");     // Prefer TCP over UDP
settings.SetCustomOption("timeout", "10000000");          // 10 second timeout
settings.SetCustomOption("stimeout", "5000000");          // 5 second socket timeout
// Buffer and probing options
settings.SetCustomOption("buffer_size", "2097152");       // 2MB buffer
settings.SetCustomOption("analyzeduration", "2000000");   // 2 seconds analysis
settings.SetCustomOption("probesize", "10000000");        // 10MB probe size
// Low latency options
settings.SetCustomOption("fflags", "nobuffer");           // Disable buffering
settings.SetCustomOption("flags", "low_delay");           // Low delay flag
settings.SetCustomOption("framedrop", "1");               // Allow frame dropping
// HTTP options
settings.SetCustomOption("user_agent", "MyApp/1.0");     // Custom user agent
settings.SetCustomOption("headers", "Authorization: Bearer token");
// Clear all custom options
settings.ClearCustomOptions();
```
---

## Example 5: Buffering Mode Configuration

Control buffering behavior for different scenarios.

### C# Buffering Examples

```csharp
public enum BufferingScenario
{
    LocalFile,
    NetworkStream,
    LowLatency
}

public void PlayWithBuffering(string source, BufferingScenario scenario, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        switch (scenario)
        {
            case BufferingScenario.LocalFile:
                // Auto mode - let filter decide
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);
                break;

            case BufferingScenario.NetworkStream:
                // Enable buffering for smooth playback
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);
                settings.SetLoadTimeOut(30);  // 30 second timeout
                break;

            case BufferingScenario.LowLatency:
                // Disable buffering for minimal latency
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_OFF);
                settings.SetCustomOption("fflags", "nobuffer");
                settings.SetCustomOption("flags", "low_delay");
                break;
        }
    }

    // Load source
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(source, null);

    // Build and run graph...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Example 6: Multi-Stream Selection
Select specific video and audio tracks.
### C# Stream Selection
```csharp
public void PlayWithStreamSelection(string filename, int videoStreamIndex, int audioStreamIndex, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Load file first
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Get available streams
    var streamSelect = sourceFilter as IAMStreamSelect;
    if (streamSelect != null)
    {
        streamSelect.Count(out int streamCount);
        List<int> videoStreams = new List<int>();
        List<int> audioStreams = new List<int>();
        // Enumerate streams
        for (int i = 0; i < streamCount; i++)
        {
            streamSelect.Info(i, out AMMediaType mt, out _, out _, out _,
                             out string name, out _, out _);
            if (mt.majorType == MediaType.Video)
            {
                videoStreams.Add(i);
                Console.WriteLine($"Video Stream {videoStreams.Count - 1}: {name}");
            }
            else if (mt.majorType == MediaType.Audio)
            {
                audioStreams.Add(i);
                Console.WriteLine($"Audio Stream {audioStreams.Count - 1}: {name}");
            }
            DsUtils.FreeAMMediaType(mt);
        }
        // Enable selected streams
        if (videoStreamIndex >= 0 && videoStreamIndex < videoStreams.Count)
        {
            streamSelect.Enable(videoStreams[videoStreamIndex],
                               AMStreamSelectEnable.Enable);
        }
        if (audioStreamIndex >= 0 && audioStreamIndex < audioStreams.Count)
        {
            streamSelect.Enable(audioStreams[audioStreamIndex],
                               AMStreamSelectEnable.Enable);
        }
    }
    // Build graph
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Example 7: Video/Audio Data Callback

Capture raw video and audio frames.

### C# Data Callback Implementation

```csharp
// Callback delegate
public delegate void DataCallbackDelegate(
    int streamType,  // 0 = video, 1 = audio
    IntPtr buffer,
    int bufferSize,
    long timestamp);

public class FFMPEGDataCallbackExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter sourceFilter;
    private DataCallbackDelegate dataCallback;

    public void PlayWithCallback(string filename, DataCallbackDelegate callback)
    {
        this.dataCallback = callback;

        filterGraph = (IFilterGraph2)new FilterGraph();

        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");

        var settings = sourceFilter as IFFMPEGSourceSettings;
        if (settings != null)
        {
            // Set data callback
            settings.SetDataCallback(OnDataReceived);
        }

        // Load file
        var fileSource = sourceFilter as IFileSourceFilter;
        fileSource.Load(filename, null);

        // Build graph (without renderers if you only want callbacks)
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Option 1: Render normally + get callbacks
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        // Option 2: No renderers - callback only
        // (Don't call RenderStream)

        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }

    private void OnDataReceived(int streamType, IntPtr buffer, int bufferSize, long timestamp)
    {
        // streamType: 0 = video, 1 = audio

        if (streamType == 0)
        {
            // Video frame received
            // Buffer contains raw video data (format depends on codec)
            byte[] videoData = new byte[bufferSize];
            Marshal.Copy(buffer, videoData, 0, bufferSize);

            // Process video data...
            ProcessVideoFrame(videoData, timestamp);
        }
        else if (streamType == 1)
        {
            // Audio frame received
            byte[] audioData = new byte[bufferSize];
            Marshal.Copy(buffer, audioData, 0, bufferSize);

            // Process audio data...
            ProcessAudioFrame(audioData, timestamp);
        }
    }

    private void ProcessVideoFrame(byte[] data, long timestamp)
    {
        // Custom video processing
        Console.WriteLine($"Video frame: {data.Length} bytes at {timestamp}ms");

        // Save to file, encode, analyze, etc.
    }

    private void ProcessAudioFrame(byte[] data, long timestamp)
    {
        // Custom audio processing
        Console.WriteLine($"Audio frame: {data.Length} bytes at {timestamp}ms");
    }
}
```

---
## Example 8: Timestamp Callback
Monitor playback timing.
### C# Timestamp Callback
```csharp
public delegate void TimestampCallbackDelegate(long videoTimestamp, long audioTimestamp);
public void PlayWithTimestampCallback(string filename, TimestampCallbackDelegate callback)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Set timestamp callback
        settings.SetTimestampCallback((videoTs, audioTs) =>
        {
            // Called periodically with current timestamps
            callback(videoTs, audioTs);
            // Update UI, sync external systems, etc.
            Console.WriteLine($"Video: {videoTs}ms, Audio: {audioTs}ms");
        });
    }
    // Load and play file...
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Example 9: License Activation

Activate purchased license.

### C# License Activation

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    // Activate license
    var registration = sourceFilter as IVFRegister;
    if (registration != null)
    {
        int hr = registration.SetLicenseKey(licenseKey);
        if (hr != 0)
        {
            throw new Exception("License activation failed");
        }
    }

    // Configure and play...
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        settings.SetHWAccelerationEnabled(true);
    }

    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);

    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Example 10: Complete Media Player
Full-featured media player with all features.
### C# Complete Example
```csharp
using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class FFMPEGMediaPlayer : IDisposable
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaSeeking mediaSeeking;
    private IVideoWindow videoWindow;
    private IMediaEventEx mediaEventEx;
    private IBaseFilter sourceFilter;
    private const int WM_GRAPHNOTIFY = 0x8000 + 1;
    public event EventHandler PlaybackComplete;
    public void Initialize(IntPtr windowHandle, IntPtr notifyHandle)
    {
        // Create filter graph
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;
        videoWindow = (IVideoWindow)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        // Setup event notifications
        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);
        // Attach capture graph
        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }
    public void LoadFile(string filename, bool enableGPU, bool enableBuffering, string licenseKey = null)
    {
        // Create source filter
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");
        // Register license if provided
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Configure filter
        var settings = sourceFilter as IFFMPEGSourceSettings;
        if (settings != null)
        {
            settings.SetHWAccelerationEnabled(enableGPU);
            if (enableBuffering)
            {
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);
            }
            else
            {
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);
            }
            settings.SetLoadTimeOut(30);
        }
        // Load file
        var fileSource = sourceFilter as IFileSourceFilter;
        int hr = fileSource.Load(filename, null);
        DsError.ThrowExceptionForHR(hr);
        // Render streams
        hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    }
    public void SetVideoWindow(IntPtr handle, int width, int height)
    {
        if (videoWindow != null)
        {
            videoWindow.put_Owner(handle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.SetWindowPosition(0, 0, width, height);
            videoWindow.put_Visible(OABool.True);
        }
    }
    public void Play()
    {
        mediaControl?.Run();
    }
    public void Pause()
    {
        mediaControl?.Pause();
    }
    public void Stop()
    {
        mediaControl?.Stop();
    }
    public void Seek(long timeInSeconds)
    {
        if (mediaSeeking != null)
        {
            long duration;
            mediaSeeking.GetDuration(out duration);
            long seekPos = timeInSeconds * 10000000; // Convert to 100-nanosecond units
            if (seekPos <= duration)
            {
                mediaSeeking.SetPositions(ref seekPos, AMSeekingSeekingFlags.AbsolutePositioning,
                                         IntPtr.Zero, AMSeekingSeekingFlags.NoPositioning);
            }
        }
    }
    public long GetPosition()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetCurrentPosition(out long position);
            return position / 10000000; // Convert to seconds
        }
        return 0;
    }
    public long GetDuration()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetDuration(out long duration);
            return duration / 10000000; // Convert to seconds
        }
        return 0;
    }
    public void HandleGraphEvent()
    {
        if (mediaEventEx != null)
        {
            while (mediaEventEx.GetEvent(out EventCode eventCode, out IntPtr param1,
                                          out IntPtr param2, 0) == 0)
            {
                mediaEventEx.FreeEventParams(eventCode, param1, param2);
                if (eventCode == EventCode.Complete)
                {
                    PlaybackComplete?.Invoke(this, EventArgs.Empty);
                }
            }
        }
    }
    public void Dispose()
    {
        if (mediaControl != null)
        {
            mediaControl.Stop();
        }
        if (mediaEventEx != null)
        {
            mediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
        }
        if (videoWindow != null)
        {
            videoWindow.put_Visible(OABool.False);
            videoWindow.put_Owner(IntPtr.Zero);
        }
        FilterGraphTools.RemoveAllFilters(filterGraph);
        if (sourceFilter != null) Marshal.ReleaseComObject(sourceFilter);
        if (videoWindow != null) Marshal.ReleaseComObject(videoWindow);
        if (mediaSeeking != null) Marshal.ReleaseComObject(mediaSeeking);
        if (mediaEventEx != null) Marshal.ReleaseComObject(mediaEventEx);
        if (captureGraph != null) Marshal.ReleaseComObject(captureGraph);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```
### Usage Example
```csharp
public partial class MainForm : Form
{
    private FFMPEGMediaPlayer player;
    public MainForm()
    {
        InitializeComponent();
        player = new FFMPEGMediaPlayer();
    }
    private void btnLoad_Click(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = "Video Files|*.mp4;*.mkv;*.avi;*.mov|All Files|*.*";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            player.Initialize(panelVideo.Handle, this.Handle);
            player.LoadFile(dlg.FileName, enableGPU: true, enableBuffering: true);
            player.SetVideoWindow(panelVideo.Handle, panelVideo.Width, panelVideo.Height);
            player.PlaybackComplete += Player_PlaybackComplete;
            player.Play();
        }
    }
    private void Player_PlaybackComplete(object sender, EventArgs e)
    {
        MessageBox.Show("Playback complete");
    }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x8000 + 1) // WM_GRAPHNOTIFY
        {
            player?.HandleGraphEvent();
        }
        base.WndProc(ref m);
    }
    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        player?.Dispose();
    }
}
```
---

## Troubleshooting

### Issue: "Class not registered" Error

**Solution**: Ensure filter is registered:

```bash
regsvr32 VisioForge_FFMPEG_Source_x64.ax
```

### Issue: Hardware Acceleration Not Working

**Solution**: Check GPU support and drivers:

```csharp
var settings = sourceFilter as IFFMPEGSourceSettings;
bool isEnabled;
settings.GetHWAccelerationEnabled(out isEnabled);
Console.WriteLine($"HW Acceleration: {isEnabled}");
```

### Issue: Network Stream Connection Fails

**Solution**: Increase timeout and use custom options:

```csharp
settings.SetLoadTimeOut(60);
settings.SetCustomOption("timeout", "30000000");  // 30 seconds
settings.SetCustomOption("rtsp_transport", "tcp");
```

---
## See Also
### Documentation
- [Interface Reference](interface-reference.md) - Complete API reference
- [Deployment Guide](../deployment/index.md) - Filter deployment
### Code Samples
- **[GitHub Samples Repository](https://github.com/visioforge/directshow-samples)** - Complete working examples
- **[C# Sample Project](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** - Full-featured C# implementation
- **[VB.NET Sample Project](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** - VB.NET implementation
- **[C++ Sample Project](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** - C++Builder implementation
### External Resources
- [FFmpeg Documentation](https://ffmpeg.org/documentation.html)
- [DirectShow Programming Guide](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)
### Support
- [Technical Support](https://support.visioforge.com) - Get help from VisioForge team
- [Discord Community](https://discord.com/invite/yvXUG56WCH) - Join our community