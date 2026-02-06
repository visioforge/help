---
title: Encoding Filters Pack - Code Examples
description: Code examples for NVENC hardware encoding, H.264/H.265/VP8 software encoders, AAC/MP3 audio codecs, and MP4/MKV muxing in DirectShow.
---

# Encoding Filters Pack - Code Examples

## Overview

This page provides practical code examples for encoding video and audio using the Encoding Filters Pack. Covers:

- **NVENC Encoder** - NVIDIA hardware encoding (H.264/H.265)
- **Software Encoders** - H.264, H.265, VP8, VP9, MPEG-2
- **Audio Encoders** - AAC, MP3, Opus, Vorbis, FLAC
- **Muxers** - MP4, MKV, WebM, MPEG-TS, AVI

---
## Prerequisites
### C++ Projects
```cpp
#include <dshow.h>
#include <streams.h>
#include "INVEncConfig.h"  // NVENC interface
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

## NVENC Hardware Encoding Examples

### Example 1: Basic NVENC H.264 Encoding

Encode video with NVIDIA hardware acceleration.

#### C# NVENC H.264 Encoding

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class NVENCBasicExample
{
    private IFilterGraph2 filterGraph;
    private IMediaControl mediaControl;

    public void EncodeWithNVENC(string inputFile, string outputFile)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        mediaControl = (IMediaControl)filterGraph;

        // Add source filter
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Add NVENC Encoder
        var encoderFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFNVENCEncoder,
            "NVENC Encoder");

        // Configure NVENC
        var nvenc = encoderFilter as INVEncConfig;
        if (nvenc != null)
        {
            // Set codec (H.264)
            nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_H264);

            // Set rate control mode
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);

            // Set bitrate (5 Mbps)
            nvenc.SetBitrate(5000000);

            // Set quality preset
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);  // Balanced

            // Set GOP size (keyframe interval)
            nvenc.SetGOP(60);  // Every 2 seconds at 30fps

            // Set B-frames (Turing+ GPUs)
            nvenc.SetBFrames(2);

            // Set profile
            nvenc.SetProfile(NVENC_H264_PROFILE.NVENC_H264_PROFILE_HIGH);

            // Set level
            nvenc.SetLevel(NVENC_H264_LEVEL.NVENC_H264_LEVEL_41);
        }

        // Add MP4 muxer
        var muxerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFMP4Muxer,
            "MP4 Muxer");

        // Set output file
        var fileSink = muxerFilter as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Connect filters: Source → NVENC → Muxer
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Connect video path
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);

        // Connect audio path (passthrough or encode)
        // captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, muxerFilter);

        // Run encoding
        mediaControl.Run();

        // Wait for completion...
        // (Monitor IMediaEventEx for EC_COMPLETE)

        Marshal.ReleaseComObject(captureGraph);
    }

    public void Stop()
    {
        mediaControl?.Stop();

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}
```

#### C++ NVENC H.264 Encoding

```cpp
HRESULT EncodeWithNVENC(LPCWSTR inputFile, LPCWSTR outputFile)
{
    IGraphBuilder* pGraph = NULL;
    IBaseFilter* pSource = NULL;
    IBaseFilter* pEncoder = NULL;
    IBaseFilter* pMuxer = NULL;
    INVEncConfig* pNVEnc = NULL;

    // Create graph
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Add source
    hr = pGraph->AddSourceFilter(inputFile, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Add NVENC encoder
    hr = CoCreateInstance(CLSID_VFNVENCEncoder, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEncoder);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEncoder, L"NVENC Encoder");
    if (FAILED(hr)) goto cleanup;

    // Configure NVENC
    hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
    if (SUCCEEDED(hr))
    {
        pNVEnc->SetCodec(NVENC_CODEC_H264);
        pNVEnc->SetRateControl(NVENC_RC_CBR);
        pNVEnc->SetBitrate(5000000);  // 5 Mbps
        pNVEnc->SetPreset(NVENC_PRESET_P4);
        pNVEnc->SetGOP(60);

        pNVEnc->Release();
    }

    // Add muxer
    hr = CoCreateInstance(CLSID_VFMP4Muxer, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pMuxer);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pMuxer, L"MP4 Muxer");
    if (FAILED(hr)) goto cleanup;

    // Set output file
    IFileSinkFilter* pFileSink = NULL;
    hr = pMuxer->QueryInterface(IID_IFileSinkFilter, (void**)&pFileSink);
    if (SUCCEEDED(hr))
    {
        pFileSink->SetFileName(outputFile, NULL);
        pFileSink->Release();
    }

    // Connect filters and run...
    // (Use ICaptureGraphBuilder2::RenderStream)

cleanup:
    if (pMuxer) pMuxer->Release();
    if (pEncoder) pEncoder->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Example 2: NVENC H.265 (HEVC) Encoding
Encode with H.265 for better compression.
#### C# NVENC H.265
```csharp
public void EncodeH265(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Add NVENC encoder
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = encoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        // H.265/HEVC codec
        nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_HEVC);
        // Rate control: CQP (Constant Quality)
        nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CQP);
        nvenc.SetCQP(23);  // Quality: 0=lossless, 51=worst
        // Quality preset
        nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P5);  // Higher quality
        // H.265 profile
        nvenc.SetHEVCProfile(NVENC_HEVC_PROFILE.NVENC_HEVC_PROFILE_MAIN);
        // H.265 level
        nvenc.SetHEVCLevel(NVENC_HEVC_LEVEL.NVENC_HEVC_LEVEL_41);
        // GOP and B-frames
        nvenc.SetGOP(120);  // 4 seconds at 30fps
        nvenc.SetBFrames(3);
    }
    // Add muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connect and encode...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Example 3: NVENC Rate Control Modes

Different rate control strategies for various use cases.

#### C# Rate Control Examples

```csharp
public enum RateControlMode
{
    CBR,     // Constant Bitrate
    VBR,     // Variable Bitrate
    CQP      // Constant Quality
}

public void ConfigureRateControl(INVEncConfig nvenc, RateControlMode mode)
{
    switch (mode)
    {
        case RateControlMode.CBR:
            // Best for streaming (predictable bitrate)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);
            nvenc.SetBitrate(5000000);      // 5 Mbps
            nvenc.SetMaxBitrate(5000000);   // Same as bitrate for CBR
            nvenc.SetVBVBufferSize(10000000); // 10 Mb buffer
            break;

        case RateControlMode.VBR:
            // Best for recording (better quality)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_VBR);
            nvenc.SetBitrate(5000000);      // Average 5 Mbps
            nvenc.SetMaxBitrate(8000000);   // Peak 8 Mbps
            nvenc.SetVBVBufferSize(10000000);
            break;

        case RateControlMode.CQP:
            // Best for archival (consistent quality)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CQP);
            nvenc.SetCQP(23);  // QP value: lower = better quality
            // No bitrate settings needed for CQP
            break;
    }
}
```

---
### Example 4: NVENC Quality Presets
Balance between speed and quality.
#### C# Quality Presets
```csharp
public void SetQualityPreset(INVEncConfig nvenc, string useCase)
{
    switch (useCase.ToLower())
    {
        case "realtime":
            // Fastest encoding (live streaming)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P1);
            nvenc.SetBFrames(0);  // No B-frames for low latency
            break;
        case "fast":
            // Fast encoding
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P2);
            nvenc.SetBFrames(1);
            break;
        case "balanced":
            // Balanced speed/quality (recommended)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
            nvenc.SetBFrames(2);
            break;
        case "quality":
            // Higher quality
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P6);
            nvenc.SetBFrames(3);
            break;
        case "maximum":
            // Best quality (slowest)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P7);
            nvenc.SetBFrames(4);
            break;
        default:
            // Default to balanced
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
            nvenc.SetBFrames(2);
            break;
    }
}
```
---

## Software Encoding Examples

### Example 5: Software H.264 Encoder

Use CPU-based H.264 encoding.

#### C# Software H.264

```csharp
public void EncodeSoftwareH264(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Add software H.264 encoder
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFH264Encoder,  // Software encoder
        "H.264 Encoder");

    // Configure encoder (if interface available)
    // var h264Config = encoderFilter as IH264EncoderConfig;
    // Configure bitrate, quality, etc.

    // Add muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Connect and encode
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Audio Encoding Examples
### Example 6: AAC Audio Encoding
Encode audio to AAC format.
#### C# AAC Encoding
```csharp
public void EncodeAACAudio(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Add video encoder (e.g., NVENC)
    var videoEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = videoEncoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_H264);
        nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);
        nvenc.SetBitrate(5000000);
        nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
    }
    // Add AAC audio encoder
    var audioEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAAEncoder,
        "AAC Encoder");
    // Configure AAC (if interface available)
    // var aacConfig = audioEncoderFilter as IAACEncoderConfig;
    // aacConfig?.SetBitrate(192000);  // 192 kbps
    // aacConfig?.SetProfile(AAC_PROFILE_LC);
    // Add muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connect filters
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    // Video path
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoderFilter, muxerFilter);
    // Audio path
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoderFilter, muxerFilter);
    // Run encoding
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Example 7: Multiple Audio Codec Support

Support for different audio codecs.

#### C# Audio Codec Selection

```csharp
public enum AudioCodec
{
    AAC,
    MP3,
    Opus,
    Vorbis,
    FLAC
}

public IBaseFilter CreateAudioEncoder(IFilterGraph2 filterGraph, AudioCodec codec)
{
    Guid encoderCLSID;
    string encoderName;

    switch (codec)
    {
        case AudioCodec.AAC:
            encoderCLSID = Consts.CLSID_VFAACEncoder;
            encoderName = "AAC Encoder";
            break;

        case AudioCodec.MP3:
            encoderCLSID = Consts.CLSID_VFMP3Encoder;
            encoderName = "MP3 Encoder";
            break;

        case AudioCodec.Opus:
            encoderCLSID = Consts.CLSID_VFOpusEncoder;
            encoderName = "Opus Encoder";
            break;

        case AudioCodec.Vorbis:
            encoderCLSID = Consts.CLSID_VFVorbisEncoder;
            encoderName = "Vorbis Encoder";
            break;

        case AudioCodec.FLAC:
            encoderCLSID = Consts.CLSID_VFFLACEncoder;
            encoderName = "FLAC Encoder";
            break;

        default:
            throw new ArgumentException("Unsupported audio codec");
    }

    return FilterGraphTools.AddFilterFromClsid(filterGraph, encoderCLSID, encoderName);
}
```

---
## Container Muxing Examples
### Example 8: MP4 Container
Mux video and audio to MP4 format.
#### C# MP4 Muxing
```csharp
public void CreateMP4(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Add video encoder
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    // Add audio encoder
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Add MP4 muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    // Set output file
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Configure muxer (if needed)
    // var mp4Config = muxerFilter as IMP4MuxerConfig;
    // mp4Config?.SetFastStart(true);  // Enable fast start for web
    // Connect filters
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, muxerFilter);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, muxerFilter);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Example 9: MKV Container

Create Matroska (MKV) files.

#### C# MKV Muxing

```csharp
public void CreateMKV(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Add encoders
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");

    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");

    // Add MKV muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMKVMuxer,
        "MKV Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Connect and encode
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, muxerFilter);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, muxerFilter);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
### Example 10: WebM Container
Create WebM files for web delivery.
#### C# WebM Muxing
```csharp
public void CreateWebM(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Add source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Add VP9 video encoder (WebM standard)
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVP9Encoder,
        "VP9 Encoder");
    // Add Opus audio encoder (WebM standard)
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFOpusEncoder,
        "Opus Encoder");
    // Add WebM muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFWebMMuxer,
        "WebM Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connect and encode
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, muxerFilter);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, muxerFilter);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

## Complete Encoding Pipeline

### Example 11: Full-Featured Encoder

Complete encoding application with all features.

#### C# Complete Encoder

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

public class CompleteEncoder : IDisposable
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaEventEx mediaEventEx;
    private IMediaSeeking mediaSeeking;

    private const int WM_GRAPHNOTIFY = 0x8000 + 1;

    public event EventHandler EncodingComplete;
    public event EventHandler<ProgressEventArgs> ProgressChanged;

    public class ProgressEventArgs : EventArgs
    {
        public long CurrentPosition { get; set; }
        public long Duration { get; set; }
        public double PercentComplete { get; set; }
    }

    public void Initialize(IntPtr notifyHandle)
    {
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;

        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);

        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }

    public void ConfigureEncoding(
        string inputFile,
        string outputFile,
        VideoCodec videoCodec,
        AudioCodec audioCodec,
        ContainerFormat container,
        int videoBitrate = 5000000,
        int audioBitrate = 192000)
    {
        // Add source
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Create video encoder
        IBaseFilter videoEncoder = CreateVideoEncoder(videoCodec);
        ConfigureVideoEncoder(videoEncoder, videoCodec, videoBitrate);

        // Create audio encoder
        IBaseFilter audioEncoder = CreateAudioEncoder(audioCodec);
        ConfigureAudioEncoder(audioEncoder, audioCodec, audioBitrate);

        // Create muxer
        IBaseFilter muxer = CreateMuxer(container);

        // Set output file
        var fileSink = muxer as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Connect pipeline
        int hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoder, muxer);
        hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoder, muxer);
    }

    private IBaseFilter CreateVideoEncoder(VideoCodec codec)
    {
        Guid clsid;
        string name;

        switch (codec)
        {
            case VideoCodec.H264_NVENC:
                clsid = Consts.CLSID_VFNVENCEncoder;
                name = "NVENC H.264";
                break;

            case VideoCodec.H265_NVENC:
                clsid = Consts.CLSID_VFNVENCEncoder;
                name = "NVENC H.265";
                break;

            case VideoCodec.H264_Software:
                clsid = Consts.CLSID_VFH264Encoder;
                name = "Software H.264";
                break;

            default:
                throw new ArgumentException("Unsupported video codec");
        }

        return FilterGraphTools.AddFilterFromClsid(filterGraph, clsid, name);
    }

    private void ConfigureVideoEncoder(IBaseFilter encoder, VideoCodec codec, int bitrate)
    {
        if (codec == VideoCodec.H264_NVENC || codec == VideoCodec.H265_NVENC)
        {
            var nvenc = encoder as INVEncConfig;
            if (nvenc != null)
            {
                nvenc.SetCodec(codec == VideoCodec.H264_NVENC ?
                              NVENC_CODEC.NVENC_CODEC_H264 :
                              NVENC_CODEC.NVENC_CODEC_HEVC);

                nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);
                nvenc.SetBitrate(bitrate);
                nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
                nvenc.SetGOP(60);
                nvenc.SetBFrames(2);
            }
        }
        // Configure other encoders...
    }

    private IBaseFilter CreateAudioEncoder(AudioCodec codec)
    {
        Guid clsid;
        string name;

        switch (codec)
        {
            case AudioCodec.AAC:
                clsid = Consts.CLSID_VFAACEncoder;
                name = "AAC Encoder";
                break;

            case AudioCodec.MP3:
                clsid = Consts.CLSID_VFMP3Encoder;
                name = "MP3 Encoder";
                break;

            default:
                throw new ArgumentException("Unsupported audio codec");
        }

        return FilterGraphTools.AddFilterFromClsid(filterGraph, clsid, name);
    }

    private void ConfigureAudioEncoder(IBaseFilter encoder, AudioCodec codec, int bitrate)
    {
        // Configure audio encoder based on codec type
        // (Interface-specific configuration)
    }

    private IBaseFilter CreateMuxer(ContainerFormat format)
    {
        Guid clsid;
        string name;

        switch (format)
        {
            case ContainerFormat.MP4:
                clsid = Consts.CLSID_VFMP4Muxer;
                name = "MP4 Muxer";
                break;

            case ContainerFormat.MKV:
                clsid = Consts.CLSID_VFMKVMuxer;
                name = "MKV Muxer";
                break;

            case ContainerFormat.WebM:
                clsid = Consts.CLSID_VFWebMMuxer;
                name = "WebM Muxer";
                break;

            default:
                throw new ArgumentException("Unsupported container format");
        }

        return FilterGraphTools.AddFilterFromClsid(filterGraph, clsid, name);
    }

    public void StartEncoding()
    {
        mediaControl?.Run();
    }

    public void Stop()
    {
        mediaControl?.Stop();
    }

    public double GetProgress()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetCurrentPosition(out long position);
            mediaSeeking.GetDuration(out long duration);

            if (duration > 0)
            {
                return (double)position / duration * 100.0;
            }
        }

        return 0.0;
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
                    EncodingComplete?.Invoke(this, EventArgs.Empty);
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

        FilterGraphTools.RemoveAllFilters(filterGraph);

        if (mediaEventEx != null) Marshal.ReleaseComObject(mediaEventEx);
        if (mediaSeeking != null) Marshal.ReleaseComObject(mediaSeeking);
        if (captureGraph != null) Marshal.ReleaseComObject(captureGraph);
        if (mediaControl != null) Marshal.ReleaseComObject(mediaControl);
        if (filterGraph != null) Marshal.ReleaseComObject(filterGraph);
    }
}

public enum VideoCodec
{
    H264_NVENC,
    H265_NVENC,
    H264_Software,
    VP8,
    VP9
}

public enum ContainerFormat
{
    MP4,
    MKV,
    WebM,
    MPEG_TS,
    AVI
}
```

---
## Troubleshooting
### Issue: NVENC Not Available
**Solution**: Check GPU compatibility:
```csharp
var nvenc = encoder as INVEncConfig;
if (nvenc != null)
{
    // Try to set codec
    int hr = nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_H264);
    if (hr != 0)
    {
        Console.WriteLine("NVENC not available - GPU may not support it");
        // Fall back to software encoder
    }
}
```
### Issue: Encoding Too Slow
**Solution**: Adjust quality preset:
```csharp
// Use faster preset
nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P1);  // Fastest
nvenc.SetBFrames(0);  // Disable B-frames
```
### Issue: Output File Size Too Large
**Solution**: Adjust bitrate or use better codec:
```csharp
// Lower bitrate
nvenc.SetBitrate(2000000);  // 2 Mbps instead of 5
// Or use H.265 for better compression
nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_HEVC);
```
---

## See Also

### Documentation

- [NVENC Interface](interfaces/nvenc.md) - Complete NVENC API
- [Codecs Reference](codecs-reference.md) - All video/audio codecs
- [Muxers Reference](muxers-reference.md) - Container formats

### External Resources

- [NVIDIA NVENC Documentation](https://developer.nvidia.com/video-codec-sdk)
- [H.264 Specification](https://www.itu.int/rec/T-REC-H.264)
