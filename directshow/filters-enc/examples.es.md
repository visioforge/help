---
title: Codificación DirectShow: NVENC, H.264/H.265 y Muxers MP4/MKV
description: Ejemplos de codificación DirectShow: NVENC GPU, H.264/H.265 software, AAC/MP3/Opus audio y multiplexación MP4/MKV/WebM con código C# y C++.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - MP4
  - MKV
  - WebM
  - TS
  - H.264
  - H.265
  - VP9
  - AAC
  - MP3
  - Opus
  - FLAC
  - Vorbis
  - C#
primary_api_classes:
  - IBaseFilter
  - IFileSinkFilter
  - ProgressEventArgs

---

# Encoding Filters Pack - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos para codificar video y audio usando el Encoding Filters Pack. Cubre:

- **NVENC Encoder** - Codificación hardware NVIDIA (H.264/H.265)
- **Codificadores Software** - H.264, H.265, VP8, VP9, MPEG-2
- **Codificadores de Audio** - AAC, MP3, Opus, Vorbis, FLAC
- **Muxers** - MP4, MKV, WebM, MPEG-TS, AVI

---
## Requisitos Previos
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

## Ejemplos de Codificación Hardware NVENC

> **GUIDs de Preset y Perfil NVENC** — Los métodos `SetPreset(Guid)` y `SetProfile(Guid)` aceptan
> constantes GUID del SDK NVIDIA NVENC. Presets reales: `NV_ENC_PRESET_DEFAULT_GUID`,
> `NV_ENC_PRESET_HP_GUID` (alto rendimiento), `NV_ENC_PRESET_HQ_GUID` (alta calidad),
> `NV_ENC_PRESET_LOW_LATENCY_DEFAULT_GUID`, `NV_ENC_PRESET_LOW_LATENCY_HQ_GUID`,
> `NV_ENC_PRESET_LOW_LATENCY_HP_GUID`, `NV_ENC_PRESET_LOSSLESS_DEFAULT_GUID`,
> `NV_ENC_PRESET_BD_GUID`. H.264 profiles: `NV_ENC_H264_PROFILE_BASELINE_GUID`,
> `NV_ENC_H264_PROFILE_MAIN_GUID`, `NV_ENC_H264_PROFILE_HIGH_GUID`.
> HEVC: `NV_ENC_HEVC_PROFILE_MAIN_GUID`.
> Estos GUIDs están definidos en el SDK NVIDIA NVENC (`nvEncodeAPI.h`).

### Ejemplo 1: Codificación Básica NVENC H.264

Codifique video con aceleración hardware NVIDIA.

#### C# NVENC H.264

```csharp
using VisioForge.Core.Types.Output;

// ...

var nvenc = encoderFilter as INVEncConfig;
if (nvenc != null)
{
    nvenc.SetCodec(NVENCEncoder.H264);
    nvenc.SetRateControl(NVENCRateControlMode.CBR);
    nvenc.SetBitrate(5000000);                    // 5 Mbps
    nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);   // Equilibrio entre calidad y velocidad
    nvenc.SetGOP(60);                              // Fotograma clave cada 60 cuadros
    nvenc.SetBFrames(2);                           // B-frames
    nvenc.SetProfile(NV_ENC_H264_PROFILE_HIGH_GUID);
    nvenc.SetLevel(NVENCEncoderLevel.H264_41);     // Nivel 4.1
}
```

#### C++ NVENC H.264

```cpp
hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
if (SUCCEEDED(hr))
{
    pNVEnc->SetCodec(0);                              // H264
    pNVEnc->SetRateControl(2);                        // CBR
    pNVEnc->SetBitrate(5000000);                      // 5 Mbps
    pNVEnc->SetPreset(NV_ENC_PRESET_DEFAULT_GUID);    // Equilibrado
    pNVEnc->SetGOP(60);                               // Intervalo de fotogramas clave
    pNVEnc->SetBFrames(2);                            // B-frames
    pNVEnc->SetProfile(NV_ENC_H264_PROFILE_HIGH_GUID);
    pNVEnc->SetLevel(41);                             // Nivel 4.1

    pNVEnc->Release();
}
```

---
### Ejemplo 2: Codificación NVENC H.265 (HEVC)
Codifique con H.265 para mejor compresión.
#### C# NVENC H.265
```csharp
public void EncodeH265(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Añadir codificador NVENC
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = encoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        nvenc.SetCodec(NVENCEncoder.HEVC);
        nvenc.SetRateControl(NVENCRateControlMode.CONST_QP);
        nvenc.SetQp(23);                            // QP 23: buen equilibrio
        nvenc.SetPreset(NV_ENC_PRESET_HQ_GUID);      // Alta calidad
        nvenc.SetProfile(NV_ENC_HEVC_PROFILE_MAIN_GUID);
        nvenc.SetLevel(NVENCEncoderLevel.H264_41);   // Nivel 4.1
        nvenc.SetGOP(120);                           // 4 segundos a 30 fps
        nvenc.SetBFrames(3);
    }
    // Añadir muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Conectar y codificar...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Ejemplo 3: Modos de Control de Tasa NVENC

Diferentes estrategias de control de tasa para varios casos de uso.

#### C# Control de Tasa

```csharp
using VisioForge.Core.Types.Output;

public void ConfigureRateControl(INVEncConfig nvenc, NVENCRateControlMode mode)
{
    switch (mode)
    {
        case NVENCRateControlMode.CBR:
            nvenc.SetRateControl(NVENCRateControlMode.CBR);
            nvenc.SetBitrate(5000000);       // 5 Mbps target
            nvenc.SetVbvBitrate(5000000);    // Same as target for CBR
            nvenc.SetVbvSize(10000000);      // 10 Mb buffer
            break;

        case NVENCRateControlMode.VBR:
            nvenc.SetRateControl(NVENCRateControlMode.VBR);
            nvenc.SetBitrate(5000000);       // Average 5 Mbps
            nvenc.SetVbvBitrate(8000000);    // Peak 8 Mbps
            nvenc.SetVbvSize(10000000);
            break;

        case NVENCRateControlMode.CONST_QP:
            nvenc.SetRateControl(NVENCRateControlMode.CONST_QP);
            nvenc.SetQp(23);                 // QP: lower = better quality
            break;
    }
}
```

---
### Ejemplo 4: Presets de Calidad NVENC
Equilibrio entre velocidad y calidad.
#### C# Presets de Calidad
```csharp
public void SetQualityPreset(INVEncConfig nvenc, string useCase)
{
    switch (useCase.ToLower())
    {
        case "realtime":
            nvenc.SetPreset(NV_ENC_PRESET_LOW_LATENCY_HP_GUID);
            nvenc.SetBFrames(0);
            break;
        case "fast":
            nvenc.SetPreset(NV_ENC_PRESET_LOW_LATENCY_DEFAULT_GUID);
            nvenc.SetBFrames(1);
            break;
        case "balanced":
            nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);
            nvenc.SetBFrames(2);
            break;
        case "quality":
            nvenc.SetPreset(NV_ENC_PRESET_HQ_GUID);
            nvenc.SetBFrames(3);
            break;
        default:
            nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);
            nvenc.SetBFrames(2);
            break;
    }
}
```
---

## Ejemplos de Codificación Software

### Ejemplo 5: Codificador H.264 por Software

Utilice codificación H.264 basada en CPU.

#### C# H.264 por Software

```csharp
public void EncodeSoftwareH264(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Añadir codificador H.264 por software
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFH264Encoder,  // Codificador por software
        "H.264 Encoder");

    // Configurar el codificador (si la interfaz está disponible)
    // var h264Config = encoderFilter as IH264EncoderConfig;
    // Configurar bitrate, calidad, etc.

    // Añadir muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Conectar y codificar
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Ejemplos de Codificación de Audio
### Ejemplo 6: Codificación de Audio AAC
Codifique el audio en formato AAC.
#### C# Audio AAC
```csharp
public void EncodeAACAudio(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Añadir codificador de video (por ejemplo, NVENC)
    var videoEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = videoEncoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        nvenc.SetCodec(NVENCEncoder.H264);
        nvenc.SetRateControl(NVENCRateControlMode.CBR);
        nvenc.SetBitrate(5000000);
        nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);
    }
    // Añadir codificador de audio AAC
    var audioEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Configurar AAC (si la interfaz está disponible)
    // var aacConfig = audioEncoderFilter as IAACEncoderConfig;
    // aacConfig?.SetBitrate(192000);  // 192 kbps
    // aacConfig?.SetProfile(AAC_PROFILE_LC);
    // Añadir muxer
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Conectar filtros
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    // Ruta de video
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoderFilter, muxerFilter);
    // Ruta de audio
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoderFilter, muxerFilter);
    // Iniciar la codificación
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Ejemplo 7: Selección de Códec de Audio

Soporte para diferentes códecs de audio.

#### C# Selección de Códec

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
            throw new ArgumentException("Códec de audio no compatible");
    }

    return FilterGraphTools.AddFilterFromClsid(filterGraph, encoderCLSID, encoderName);
}
```

---
## Ejemplos de Contenedores (Muxing)
### Ejemplo 8: Contenedor MP4
Multiplexe video y audio a formato MP4.
#### C# MP4 Muxing
```csharp
public void CreateMP4(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Añadir codificador de video
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    // Añadir codificador de audio
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Añadir muxer MP4
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    // Establecer archivo de salida
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Configurar el muxer (si es necesario)
    // var mp4Config = muxerFilter as IMP4MuxerConfig;
    // mp4Config?.SetFastStart(true);  // Activar inicio rápido para web
    // Conectar filtros
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

### Ejemplo 9: Contenedor MKV

Cree archivos Matroska (MKV).

#### C# MKV Muxing

```csharp
public void CreateMKV(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Añadir codificadores
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");

    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");

    // Añadir muxer MKV
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMKVMuxer,
        "MKV Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Conectar y codificar
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
### Ejemplo 10: Contenedor WebM
Cree archivos WebM para entrega web.
#### C# WebM Muxing
```csharp
public void CreateWebM(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Añadir fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Añadir codificador de video VP9 (estándar WebM)
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVP9Encoder,
        "VP9 Encoder");
    // Añadir codificador de audio Opus (estándar WebM)
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFOpusEncoder,
        "Opus Encoder");
    // Añadir muxer WebM
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFWebMMuxer,
        "WebM Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Conectar y codificar
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

## Pipeline de Codificación Completo

### Ejemplo 11: Codificador Completo

Aplicación de codificación completa con todas las funciones.

#### C# Codificador Completo

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
        // Añadir fuente
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Crear codificador de video
        IBaseFilter videoEncoder = CreateVideoEncoder(videoCodec);
        ConfigureVideoEncoder(videoEncoder, videoCodec, videoBitrate);

        // Crear codificador de audio
        IBaseFilter audioEncoder = CreateAudioEncoder(audioCodec);
        ConfigureAudioEncoder(audioEncoder, audioCodec, audioBitrate);

        // Crear muxer
        IBaseFilter muxer = CreateMuxer(container);

        // Establecer archivo de salida
        var fileSink = muxer as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Conectar la canalización
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
                nvenc.SetCodec(codec == VideoCodec.H264_NVENC ? NVENCEncoder.H264 : NVENCEncoder.HEVC);
                nvenc.SetRateControl(NVENCRateControlMode.CBR);
                nvenc.SetBitrate(bitrate);
                nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);
                nvenc.SetGOP(60);
                nvenc.SetBFrames(2);
            }
        }
        // Configurar otros codificadores...
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
                throw new ArgumentException("Códec de audio no compatible");
        }

        return FilterGraphTools.AddFilterFromClsid(filterGraph, clsid, name);
    }

    private void ConfigureAudioEncoder(IBaseFilter encoder, AudioCodec codec, int bitrate)
    {
        // Configurar el codificador de audio según el tipo de códec
        // (Configuración específica de la interfaz)
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
## Solución de Problemas
### Problema: NVENC No Disponible
**Solución**: Verifique la compatibilidad de la GPU:
```csharp
var nvenc2 = encoder as INVEncConfig2;
if (nvenc2 != null)
{
    int hr = nvenc2.CheckNVENCAvailable(out bool available, out int status);
    if (hr != 0 || !available)
    {
        Console.WriteLine("NVENC not available - GPU may not support it");
        // Fall back to software encoder
    }
}
```
### Problema: Codificación Demasiado Lenta
**Solución**: Ajuste el preset de calidad:
```csharp
// Use un preset más rápido
nvenc.SetPreset(NV_ENC_PRESET_LOW_LATENCY_HP_GUID);
nvenc.SetBFrames(0);  // Desactivar B-frames
```
### Problema: Tamaño de Archivo Demasiado Grande
**Solución**: Ajuste el bitrate o use un códec mejor:
```csharp
// Reducir bitrate
nvenc.SetBitrate(2000000);  // 2 Mbps en vez de 5
// O usar H.265 para mejor compresión
nvenc.SetCodec(NVENCEncoder.HEVC);
```
---

## Ver También

### Documentación

- [Interfaz NVENC](interfaces/nvenc.md) - API NVENC completa
- [Referencia de Códecs](codecs-reference.md) - Todos los códecs de video/audio
- [Referencia de Muxers](muxers-reference.md) - Formatos de contenedor

### Recursos Externos

- [Documentación NVIDIA NVENC](https://developer.nvidia.com/video-codec-sdk)
- [Especificación H.264](https://www.itu.int/rec/T-REC-H.264)
