---
title: VLC Source Filter - Code Examples
description: Code examples for VLC Source Filter with multi-track audio, subtitles, 360° video, and custom VLC parameters in DirectShow.
---

# Code Examples

## Overview

This page provides practical code examples for using the VLC Source Filter in DirectShow applications. The VLC Source Filter supports multi-audio tracks, subtitles, 360° video, and custom VLC command-line options.

---
## Prerequisites
### C++ Projects
```cpp
#include <dshow.h>
#include <streams.h>
#include "IVlcSrc.h"      // From SDK
#include "IVlcSrc2.h"     // For custom parameters
#include "IVlcSrc3.h"     // For frame rate override
#pragma comment(lib, "strmiids.lib")
```
### C# Projects
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Text;
```
**NuGet Packages**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Example 1: Basic File Playback

Play a local media file with VLC Source Filter.

### C++ Implementation

```cpp
#include <dshow.h>
#include "IVlcSrc.h"

// CLSID for VLC Source Filter
DEFINE_GUID(CLSID_VFVLCSource,
    0x9f73dcd4, 0x2fc8, 0x4e89, 0x8f, 0xc3, 0x2d, 0xf1, 0x69, 0x3c, 0xa0, 0x3e);

HRESULT PlayVLCFile(LPCWSTR filename, HWND hVideoWindow)
{
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IVlcSrc* pVlcSrc = NULL;

    HRESULT hr = S_OK;

    // Create Filter Graph
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Create VLC Source Filter
    hr = CoCreateInstance(CLSID_VFVLCSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Add filter to graph
    hr = pGraph->AddFilter(pSourceFilter, L"VLC Source");
    if (FAILED(hr)) goto cleanup;

    // Get VLC interface
    hr = pSourceFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr)) goto cleanup;

    // Load file
    hr = pVlcSrc->SetFile((WCHAR*)filename);
    if (FAILED(hr)) goto cleanup;

    // Build and render graph
    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Render video and audio
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Run graph
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        hr = pControl->Run();
    }

cleanup:
    if (pVlcSrc) pVlcSrc->Release();
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
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class VLCSourceBasicExample
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

            // Create and add VLC Source filter
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Load file using IVlcSrc interface
            var vlcSrc = sourceFilter as IVlcSrc;
            if (vlcSrc != null)
            {
                int hr = vlcSrc.SetFile(filename);
                DsError.ThrowExceptionForHR(hr);
            }

            // Render streams
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            int result = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(result);

            captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

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
            Console.WriteLine($"Error: {ex.Message}");
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

Public Class VLCSourceBasicExample
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

            ' Create and add VLC Source filter
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source")

            ' Load file
            Dim vlcSrc = DirectCast(sourceFilter, IVlcSrc)
            If vlcSrc IsNot Nothing Then
                Dim hr As Integer = vlcSrc.SetFile(filename)
                DsError.ThrowExceptionForHR(hr)
            End If

            ' Render streams
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            captureGraph.SetFiltergraph(filterGraph)

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
            Console.WriteLine($"Error: {ex.Message}")
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
## Example 2: Audio Track Selection
List and select audio tracks from multi-audio files.
### C# Audio Track Management
```csharp
public class VLCAudioTrackExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;
    private IBaseFilter sourceFilter;
    public void PlayWithAudioTrackSelection(string filename, IntPtr videoWindowHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        mediaControl = (IMediaControl)filterGraph;
        // Create VLC Source filter
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Load file
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
            // Get audio track count
            int audioCount = 0;
            vlcSrc.GetAudioTracksCount(out audioCount);
            Console.WriteLine($"Total audio tracks: {audioCount}");
            // List all audio tracks
            for (int i = 0; i < audioCount; i++)
            {
                int trackId;
                StringBuilder trackName = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);
                Console.WriteLine($"Track {i}: ID={trackId}, Name={trackName}");
            }
            // Get currently active track
            int currentTrackId;
            vlcSrc.GetAudioTrack(out currentTrackId);
            Console.WriteLine($"Currently active track ID: {currentTrackId}");
            // Select a specific track (e.g., track index 1)
            if (audioCount > 1)
            {
                int desiredTrackId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
                vlcSrc.SetAudioTrack(desiredTrackId);
                Console.WriteLine($"Switched to track: {name}");
            }
        }
        // Build and run graph
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
    // Method to switch audio track during playback
    public void SwitchAudioTrack(int trackIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            int trackId;
            StringBuilder trackName = new StringBuilder(256);
            vlcSrc.GetAudioTrackInfo(trackIndex, out trackId, trackName);
            vlcSrc.SetAudioTrack(trackId);
            Console.WriteLine($"Switched to audio track: {trackName}");
        }
    }
}
```
### C++ Audio Track Management
```cpp
void ListAndSelectAudioTracks(IVlcSrc* pVlcSrc)
{
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);
    wprintf(L"Total audio tracks: %d\n", audioCount);
    // List all tracks
    for (int i = 0; i < audioCount; i++)
    {
        int trackId = 0;
        WCHAR trackName[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &trackId, trackName);
        wprintf(L"Track %d: ID=%d, Name=%s\n", i, trackId, trackName);
    }
    // Get current track
    int currentId = 0;
    pVlcSrc->GetAudioTrack(&currentId);
    wprintf(L"Current track ID: %d\n", currentId);
    // Select track by index (e.g., track 1)
    if (audioCount > 1)
    {
        int trackId = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetAudioTrackInfo(1, &trackId, name);
        pVlcSrc->SetAudioTrack(trackId);
        wprintf(L"Switched to track: %s\n", name);
    }
}
```
---

## Example 3: Subtitle Management

Select and manage subtitle tracks.

### C# Subtitle Management

```csharp
public class VLCSubtitleExample
{
    private IBaseFilter sourceFilter;

    public void ManageSubtitles(string filename)
    {
        // Assume filter is already created and added to graph
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);

            // Get subtitle count
            int subtitleCount = 0;
            vlcSrc.GetSubtitlesCount(out subtitleCount);

            Console.WriteLine($"Total subtitle tracks: {subtitleCount}");

            // List all subtitles
            for (int i = 0; i < subtitleCount; i++)
            {
                int subtitleId;
                StringBuilder subtitleName = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(i, out subtitleId, subtitleName);

                Console.WriteLine($"Subtitle {i}: ID={subtitleId}, Name={subtitleName}");
            }

            // Enable subtitle (e.g., subtitle index 0)
            if (subtitleCount > 0)
            {
                int subtitleId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(0, out subtitleId, name);

                vlcSrc.SetSubtitle(subtitleId);
                Console.WriteLine($"Enabled subtitle: {name}");
            }

            // Disable subtitles (use ID -1)
            // vlcSrc.SetSubtitle(-1);
        }
    }

    // Method to switch subtitles during playback
    public void SwitchSubtitle(int subtitleIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            if (subtitleIndex < 0)
            {
                // Disable subtitles
                vlcSrc.SetSubtitle(-1);
                Console.WriteLine("Subtitles disabled");
            }
            else
            {
                int subtitleId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(subtitleIndex, out subtitleId, name);

                vlcSrc.SetSubtitle(subtitleId);
                Console.WriteLine($"Switched to subtitle: {name}");
            }
        }
    }
}
```

### C++ Subtitle Management

```cpp
void ManageSubtitles(IVlcSrc* pVlcSrc)
{
    int subtitleCount = 0;
    pVlcSrc->GetSubtitlesCount(&subtitleCount);

    wprintf(L"Total subtitles: %d\n", subtitleCount);

    // List all subtitles
    for (int i = 0; i < subtitleCount; i++)
    {
        int subtitleId = 0;
        WCHAR subtitleName[256] = {0};
        pVlcSrc->GetSubtitleInfo(i, &subtitleId, subtitleName);

        wprintf(L"Subtitle %d: ID=%d, Name=%s\n", i, subtitleId, subtitleName);
    }

    // Enable first subtitle
    if (subtitleCount > 0)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetSubtitleInfo(0, &id, name);

        pVlcSrc->SetSubtitle(id);
        wprintf(L"Enabled subtitle: %s\n", name);
    }

    // Disable subtitles
    // pVlcSrc->SetSubtitle(-1);
}
```

---
## Example 4: Custom VLC Command-Line Options
Pass custom VLC parameters using IVlcSrc2 interface.
### C# Custom VLC Parameters
```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using VisioForge.Core.Helpers;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VLCCustomOptionsExample
{
    public void PlayWithCustomVLCOptions(string filename, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Set custom VLC command-line options using IVlcSrc2
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            // Create list of VLC command-line parameters
            var parameters = new List<string>();
            parameters.Add("--avcodec-hw=any");          // Enable hardware decoding
            parameters.Add("--network-caching=1000");    // 1 second network cache
            parameters.Add("--live-caching=300");        // 300ms for live streams
            parameters.Add("--file-caching=300");        // 300ms for files
            parameters.Add("--sout-mux-caching=2000");   // Output muxing cache
            parameters.Add("--vout=direct3d11");         // Use Direct3D11 renderer
            parameters.Add("--verbose=2");               // Logging level
            // Convert strings to native UTF-8 IntPtr array
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                // Call SetCustomCommandLine with IntPtr array
                int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Count);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                // Free allocated unmanaged memory
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Load file using IVlcSrc
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }
        // Build and run graph
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
### Common VLC Options
Here are common VLC parameter combinations. Remember to use the proper marshaling pattern shown above.
```csharp
// Network streaming options
var networkOptions = new List<string>
{
    "--network-caching=3000",      // 3 seconds for network streams
    "--rtsp-tcp",                  // Force TCP for RTSP
    "--http-reconnect"             // Auto-reconnect HTTP streams
};
// Hardware decoding options
var hwDecodeOptions = new List<string>
{
    "--avcodec-hw=any"             // Auto-detect hardware
    // Alternative options:
    // "--avcodec-hw=dxva2"        // Use DXVA2
    // "--avcodec-hw=d3d11va"      // Use D3D11VA
    // "--avcodec-hw=nvdec"        // Use NVIDIA NVDEC
};
// Low latency options
var lowLatencyOptions = new List<string>
{
    "--network-caching=0",
    "--live-caching=0",
    "--file-caching=0",
    "--sout-mux-caching=0",
    "--clock-jitter=0",
    "--drop-late-frames",
    "--skip-frames"
};
// 360° video options
var video360Options = new List<string>
{
    "--video-filter=transform",
    "--transform-type=hflip",
    "--vout-filter=rotate"
};
// Subtitle options
var subtitleOptions = new List<string>
{
    "--sub-autodetect-file",       // Auto-detect subtitle files
    "--sub-language=eng",          // Preferred subtitle language
    "--freetype-fontsize=20"       // Subtitle font size
};
// Audio options
var audioOptions = new List<string>
{
    "--audio-desync=0",            // Audio synchronization
    "--audiotrack-language=eng",   // Preferred audio language
    "--audio-filter=normvol"       // Volume normalization
};
// Complete example with multiple options
var completeOptions = new List<string>
{
    "--avcodec-hw=any",
    "--network-caching=1000",
    "--rtsp-tcp",
    "--sub-autodetect-file",
    "--verbose=1"
};
// Helper method to apply VLC parameters
private void ApplyVLCParameters(IVlcSrc2 vlcSrc2, List<string> parameters)
{
    if (vlcSrc2 == null || parameters == null || parameters.Count == 0)
        return;
    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }
    try
    {
        int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Count);
        DsError.ThrowExceptionForHR(hr);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```
---

## Example 5: Frame Rate Override

Override source frame rate using IVlcSrc3 interface.

### C# Frame Rate Override

```csharp
public class VLCFrameRateExample
{
    public void PlayWithCustomFrameRate(string filename, double fps, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();

        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");

        // Set custom frame rate using IVlcSrc3
        var vlcSrc3 = sourceFilter as IVlcSrc3;
        if (vlcSrc3 != null)
        {
            // Override frame rate (e.g., 30.0, 25.0, 60.0)
            int hr = vlcSrc3.SetDefaultFrameRate(fps);
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine($"Frame rate set to: {fps} fps");
        }

        // Load file
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }

        // Build and run graph
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}
```

### C++ Frame Rate Override

```cpp
#include "IVlcSrc3.h"

void SetCustomFrameRate(IBaseFilter* pFilter, double fps)
{
    IVlcSrc3* pVlcSrc3 = nullptr;
    HRESULT hr = pFilter->QueryInterface(IID_IVlcSrc3, (void**)&pVlcSrc3);

    if (SUCCEEDED(hr))
    {
        hr = pVlcSrc3->SetDefaultFrameRate(fps);
        if (SUCCEEDED(hr))
        {
            wprintf(L"Frame rate set to: %.2f fps\n", fps);
        }

        pVlcSrc3->Release();
    }
}
```

---
## Example 6: Network Streaming (RTSP/HLS)
Stream from network sources.
### C# Network Streaming
```csharp
public class VLCNetworkStreamingExample
{
    public void PlayRTSPStream(string rtspUrl, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Configure for RTSP streaming
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--rtsp-tcp",                         // Use TCP transport
                "--network-caching=300",              // Low latency (300ms)
                "--rtsp-frame-buffer-size=500000",    // Frame buffer size
                "--drop-late-frames",                 // Drop late frames
                "--skip-frames"                       // Skip frames if needed
            };
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                vlcSrc2.SetCustomCommandLine(array, parameters.Count);
            }
            finally
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Load RTSP stream
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Example: "rtsp://camera.example.com:554/stream"
            vlcSrc.SetFile(rtspUrl);
        }
        // Build and run graph
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
    public void PlayHLSStream(string hlsUrl, IntPtr videoWindowHandle)
    {
        var filterGraph = (IFilterGraph2)new FilterGraph();
        var sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Configure for HLS streaming
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--http-reconnect",                // Auto-reconnect
                "--network-caching=3000",          // 3 second buffer for HLS
                "--hls-segment-threads=3",         // Parallel segment download
                "--avcodec-hw=any"                 // Hardware decoding
            };
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                vlcSrc2.SetCustomCommandLine(array, parameters.Count);
            }
            finally
            {
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Load HLS stream
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Example: "https://example.com/stream/playlist.m3u8"
            vlcSrc.SetFile(hlsUrl);
        }
        // Build and run graph
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
        var videoWindow = (IVideoWindow)filterGraph;
        videoWindow.put_Owner(videoWindowHandle);
        videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();
        Marshal.ReleaseComObject(captureGraph);
    }
}
```
---

## Example 7: License Activation

Activate purchased license.

### C# License Activation

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    var sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVLCSource,
        "VLC Source");

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

    // Load and play file
    var vlcSrc = sourceFilter as IVlcSrc;
    if (vlcSrc != null)
    {
        vlcSrc.SetFile(filename);
    }

    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    var videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Example 8: Complete Multi-Track Player
Full-featured media player with all VLC Source features.
### C# Complete Example
```csharp
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
public class VLCMediaPlayer : IDisposable
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
    public List<AudioTrackInfo> AudioTracks { get; private set; } = new List<AudioTrackInfo>();
    public List<SubtitleInfo> Subtitles { get; private set; } = new List<SubtitleInfo>();
    public class AudioTrackInfo
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class SubtitleInfo
    {
        public int Index { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public void Initialize(IntPtr windowHandle, IntPtr notifyHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;
        videoWindow = (IVideoWindow)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);
        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }
    public void LoadFile(string filename, string licenseKey = null,
                        string vlcOptions = null, double? frameRate = null)
    {
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Register license
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Set custom VLC options
        if (!string.IsNullOrEmpty(vlcOptions))
        {
            var vlcSrc2 = sourceFilter as IVlcSrc2;
            if (vlcSrc2 != null)
            {
                // Parse space-separated options into list
                var parameters = new List<string>(vlcOptions.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
                var array = new IntPtr[parameters.Count];
                for (int i = 0; i < parameters.Count; i++)
                {
                    array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
                }
                try
                {
                    vlcSrc2.SetCustomCommandLine(array, parameters.Count);
                }
                finally
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        Marshal.FreeHGlobal(array[i]);
                    }
                }
            }
        }
        // Set frame rate override
        if (frameRate.HasValue)
        {
            var vlcSrc3 = sourceFilter as IVlcSrc3;
            vlcSrc3?.SetDefaultFrameRate(frameRate.Value);
        }
        // Load file
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            int hr = vlcSrc.SetFile(filename);
            DsError.ThrowExceptionForHR(hr);
            // Enumerate audio tracks
            LoadAudioTracks(vlcSrc);
            // Enumerate subtitles
            LoadSubtitles(vlcSrc);
        }
        // Build graph
        int result = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        result = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    }
    private void LoadAudioTracks(IVlcSrc vlcSrc)
    {
        AudioTracks.Clear();
        int count = 0;
        vlcSrc.GetAudioTracksCount(out count);
        for (int i = 0; i < count; i++)
        {
            int id;
            StringBuilder name = new StringBuilder(256);
            vlcSrc.GetAudioTrackInfo(i, out id, name);
            AudioTracks.Add(new AudioTrackInfo
            {
                Index = i,
                Id = id,
                Name = name.ToString()
            });
        }
    }
    private void LoadSubtitles(IVlcSrc vlcSrc)
    {
        Subtitles.Clear();
        int count = 0;
        vlcSrc.GetSubtitlesCount(out count);
        for (int i = 0; i < count; i++)
        {
            int id;
            StringBuilder name = new StringBuilder(256);
            vlcSrc.GetSubtitleInfo(i, out id, name);
            Subtitles.Add(new SubtitleInfo
            {
                Index = i,
                Id = id,
                Name = name.ToString()
            });
        }
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
    public void Play() => mediaControl?.Run();
    public void Pause() => mediaControl?.Pause();
    public void Stop() => mediaControl?.Stop();
    public void SelectAudioTrack(int trackIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null && trackIndex >= 0 && trackIndex < AudioTracks.Count)
        {
            vlcSrc.SetAudioTrack(AudioTracks[trackIndex].Id);
        }
    }
    public void SelectSubtitle(int subtitleIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            if (subtitleIndex < 0)
            {
                vlcSrc.SetSubtitle(-1); // Disable
            }
            else if (subtitleIndex < Subtitles.Count)
            {
                vlcSrc.SetSubtitle(Subtitles[subtitleIndex].Id);
            }
        }
    }
    public void Seek(long timeInSeconds)
    {
        if (mediaSeeking != null)
        {
            long seekPos = timeInSeconds * 10000000;
            mediaSeeking.SetPositions(ref seekPos, AMSeekingSeekingFlags.AbsolutePositioning,
                                     IntPtr.Zero, AMSeekingSeekingFlags.NoPositioning);
        }
    }
    public long GetPosition()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetCurrentPosition(out long position);
            return position / 10000000;
        }
        return 0;
    }
    public long GetDuration()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetDuration(out long duration);
            return duration / 10000000;
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
    private VLCMediaPlayer player;
    public MainForm()
    {
        InitializeComponent();
        player = new VLCMediaPlayer();
    }
    private void btnLoad_Click(object sender, EventArgs e)
    {
        OpenFileDialog dlg = new OpenFileDialog();
        dlg.Filter = "Video Files|*.mp4;*.mkv;*.avi;*.mov|All Files|*.*";
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            player.Initialize(panelVideo.Handle, this.Handle);
            // Custom VLC options for hardware decoding
            string vlcOptions = "--avcodec-hw=any --network-caching=1000";
            player.LoadFile(dlg.FileName, vlcOptions: vlcOptions);
            player.SetVideoWindow(panelVideo.Handle, panelVideo.Width, panelVideo.Height);
            // Populate audio track combo box
            comboAudioTracks.Items.Clear();
            foreach (var track in player.AudioTracks)
            {
                comboAudioTracks.Items.Add(track.Name);
            }
            if (comboAudioTracks.Items.Count > 0)
            {
                comboAudioTracks.SelectedIndex = 0;
            }
            // Populate subtitle combo box
            comboSubtitles.Items.Clear();
            comboSubtitles.Items.Add("None");
            foreach (var subtitle in player.Subtitles)
            {
                comboSubtitles.Items.Add(subtitle.Name);
            }
            comboSubtitles.SelectedIndex = 0;
            player.Play();
        }
    }
    private void comboAudioTracks_SelectedIndexChanged(object sender, EventArgs e)
    {
        player.SelectAudioTrack(comboAudioTracks.SelectedIndex);
    }
    private void comboSubtitles_SelectedIndexChanged(object sender, EventArgs e)
    {
        player.SelectSubtitle(comboSubtitles.SelectedIndex - 1); // -1 because of "None" item
    }
    protected override void WndProc(ref Message m)
    {
        if (m.Msg == 0x8000 + 1)
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

**Solution**: Ensure VLC Source filter is registered:
```bash
regsvr32 VisioForge_VLC_Source.ax
```

### Issue: Audio/Subtitle Track Count Returns 0

**Cause**: Tracks not yet available (filter graph not built yet)

**Solution**: Audio and subtitle information is only available after the file is loaded and the graph is built. Call track enumeration methods after `SetFile()` and building the graph.

```cpp
// Load file first
pVlcSrc->SetFile(L"movie.mkv");

// Build filter graph
pGraph->RenderFile(L"movie.mkv", nullptr);

// NOW query tracks
int count = 0;
pVlcSrc->GetAudioTracksCount(&count);
```

### Issue: Custom VLC Options Not Working

**Solution**: Ensure VLC options are set before loading the file, and use the correct marshaling:

```csharp
// Correct order and usage:
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var parameters = new List<string>
    {
        "--network-caching=1000",
        "--rtsp-tcp"
    };

    var array = new IntPtr[parameters.Count];
    for (int i = 0; i < parameters.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);  // 1. Set options first
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}

var vlcSrc = sourceFilter as IVlcSrc;
vlcSrc?.SetFile(filename);  // 2. Then load file
// Build and run graph...
```

### Issue: RTSP Stream Has High Latency

**Solution**: Configure VLC for minimal network buffering and use TCP transport:

```cpp
// C++ example
IVlcSrc2* pVlcSrc2 = nullptr;
hr = pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);
if (SUCCEEDED(hr))
{
    const char* params[] = {
        "--network-caching=50",
        "--live-caching=50",
        "--rtsp-tcp",
        "--no-audio-time-stretch"
    };
    
    // Convert to wide strings and then to IntPtr equivalents
    // (In C++, you can pass ANSI/UTF-8 strings directly in some cases,
    // but for consistency with the interface, use proper conversion)
    
    pVlcSrc2->Release();
}
```

```csharp
// C# example
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var lowLatencyOptions = new List<string>
    {
        "--network-caching=50",
        "--live-caching=50",
        "--rtsp-tcp",
        "--no-audio-time-stretch"
    };

    var array = new IntPtr[lowLatencyOptions.Count];
    for (int i = 0; i < lowLatencyOptions.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(lowLatencyOptions[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, lowLatencyOptions.Count);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

### Issue: Hardware Acceleration Not Working

**Solution**: Explicitly specify the hardware decoder to use:

```cpp
// C++ example - Try explicit hardware decoder
IVlcSrc2* pVlcSrc2 = nullptr;
hr = pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);
if (SUCCEEDED(hr))
{
    const char* params[] = {
        "--avcodec-hw=d3d11va"  // For Windows 8+
        // or "--avcodec-hw=dxva2" for Windows 7
    };
    
    // Proper conversion and calling required
    
    pVlcSrc2->Release();
}
```

```csharp
// C# example
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var hwOptions = new List<string>
    {
        "--avcodec-hw=dxva2"  // or d3d11va, nvdec, any
    };

    var array = new IntPtr[hwOptions.Count];
    for (int i = 0; i < hwOptions.Count; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(hwOptions[i]);
    }

    try
    {
        vlcSrc2.SetCustomCommandLine(array, hwOptions.Count);
    }
    finally
    {
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }
}
```

---
## See Also
### Documentation
- [Interface Reference](interface-reference.md) - Complete API reference
- [Deployment Guide](../deployment/index.md) - Filter deployment
### External Resources
- [VLC Command-Line Documentation](https://www.videolan.org/doc/)
- [DirectShow Programming Guide](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)