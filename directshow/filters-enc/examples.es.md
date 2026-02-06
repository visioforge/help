---
title: Ejemplos de Código: Filtros de Codificación
description: Ejemplos de NVENC hardware, codificadores H.264/H.265/VP8, códecs AAC/MP3 y multiplexación MP4/MKV en DirectShow con código.
---

# Paquete de Filtros de Codificación - Ejemplos de Código

## Descripción General

Esta página proporciona ejemplos prácticos de código para codificar video y audio utilizando el Paquete de Filtros de Codificación. Cubre:

- **Codificador NVENC** - Codificación de hardware NVIDIA (H.264/H.265)
- **Codificadores de Software** - H.264, H.265, VP8, VP9, MPEG-2
- **Codificadores de Audio** - AAC, MP3, Opus, Vorbis, FLAC
- **Multiplexores** - MP4, MKV, WebM, MPEG-TS, AVI

---
## Prerrequisitos
### Proyectos C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "INVEncConfig.h"  // Interfaz NVENC
#pragma comment(lib, "strmiids.lib")
```
### Proyectos C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
```
**Paquetes NuGet**:
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Ejemplos de Codificación de Hardware NVENC

### Ejemplo 1: Codificación Básica NVENC H.264

Codificar video con aceleración de hardware NVIDIA.

#### Codificación NVENC H.264 en C#

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

        // Agregar filtro de fuente
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Agregar Codificador NVENC
        var encoderFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFNVENCEncoder,
            "NVENC Encoder");

        // Configurar NVENC
        var nvenc = encoderFilter as INVEncConfig;
        if (nvenc != null)
        {
            // Establecer códec (H.264)
            nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_H264);

            // Establecer modo de control de tasa
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);

            // Establecer tasa de bits (5 Mbps)
            nvenc.SetBitrate(5000000);

            // Establecer preajuste de calidad
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);  // Equilibrado

            // Establecer tamaño GOP (intervalo de fotogramas clave)
            nvenc.SetGOP(60);  // Cada 2 segundos a 30fps

            // Establecer B-frames (GPUs Turing+)
            nvenc.SetBFrames(2);

            // Establecer perfil
            nvenc.SetProfile(NVENC_H264_PROFILE.NVENC_H264_PROFILE_HIGH);

            // Establecer nivel
            nvenc.SetLevel(NVENC_H264_LEVEL.NVENC_H264_LEVEL_41);
        }

        // Agregar multiplexor MP4
        var muxerFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFMP4Muxer,
            "MP4 Muxer");

        // Establecer archivo de salida
        var fileSink = muxerFilter as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Conectar filtros: Fuente → NVENC → Multiplexor
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Conectar ruta de video
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);

        // Conectar ruta de audio (paso directo o codificar)
        // captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, muxerFilter);

        // Ejecutar codificación
        mediaControl.Run();

        // Esperar finalización...
        // (Monitorear IMediaEventEx para EC_COMPLETE)

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

#### Codificación NVENC H.264 en C++

```cpp
HRESULT EncodeWithNVENC(LPCWSTR inputFile, LPCWSTR outputFile)
{
    IGraphBuilder* pGraph = NULL;
    IBaseFilter* pSource = NULL;
    IBaseFilter* pEncoder = NULL;
    IBaseFilter* pMuxer = NULL;
    INVEncConfig* pNVEnc = NULL;

    // Crear gráfico
    HRESULT hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Agregar fuente
    hr = pGraph->AddSourceFilter(inputFile, L"Source", &pSource);
    if (FAILED(hr)) goto cleanup;

    // Agregar codificador NVENC
    hr = CoCreateInstance(CLSID_VFNVENCEncoder, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pEncoder);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pEncoder, L"NVENC Encoder");
    if (FAILED(hr)) goto cleanup;

    // Configurar NVENC
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

    // Agregar multiplexor
    hr = CoCreateInstance(CLSID_VFMP4Muxer, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pMuxer);
    if (FAILED(hr)) goto cleanup;

    hr = pGraph->AddFilter(pMuxer, L"MP4 Muxer");
    if (FAILED(hr)) goto cleanup;

    // Establecer archivo de salida
    IFileSinkFilter* pFileSink = NULL;
    hr = pMuxer->QueryInterface(IID_IFileSinkFilter, (void**)&pFileSink);
    if (SUCCEEDED(hr))
    {
        pFileSink->SetFileName(outputFile, NULL);
        pFileSink->Release();
    }

    // Conectar filtros y ejecutar...
    // (Usar ICaptureGraphBuilder2::RenderStream)

cleanup:
    if (pMuxer) pMuxer->Release();
    if (pEncoder) pEncoder->Release();
    if (pSource) pSource->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

---
### Ejemplo 2: Codificación NVENC H.265 (HEVC)
Codificar con H.265 para una mejor compresión.
#### NVENC H.265 en C#
```csharp
public void EncodeH265(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Agregar codificador NVENC
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = encoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        // Códec H.265/HEVC
        nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_HEVC);
        // Control de tasa: CQP (Calidad Constante)
        nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CQP);
        nvenc.SetCQP(23);  // Calidad: 0=sin pérdida, 51=peor
        // Preajuste de calidad
        nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P5);  // Mayor calidad
        // Perfil H.265
        nvenc.SetHEVCProfile(NVENC_HEVC_PROFILE.NVENC_HEVC_PROFILE_MAIN);
        // Nivel H.265
        nvenc.SetHEVCLevel(NVENC_HEVC_LEVEL.NVENC_HEVC_LEVEL_41);
        // GOP y B-frames
        nvenc.SetGOP(120);  // 4 segundos a 30fps
        nvenc.SetBFrames(3);
    }
    // Agregar multiplexor
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

#### Ejemplos de Control de Tasa en C#

```csharp
public enum RateControlMode
{
    CBR,     // Tasa de Bits Constante
    VBR,     // Tasa de Bits Variable
    CQP      // Calidad Constante
}

public void ConfigureRateControl(INVEncConfig nvenc, RateControlMode mode)
{
    switch (mode)
    {
        case RateControlMode.CBR:
            // Mejor para transmisión (tasa de bits predecible)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CBR);
            nvenc.SetBitrate(5000000);      // 5 Mbps
            nvenc.SetMaxBitrate(5000000);   // Igual que la tasa de bits para CBR
            nvenc.SetVBVBufferSize(10000000); // Búfer de 10 Mb
            break;

        case RateControlMode.VBR:
            // Mejor para grabación (mejor calidad)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_VBR);
            nvenc.SetBitrate(5000000);      // Promedio 5 Mbps
            nvenc.SetMaxBitrate(8000000);   // Pico 8 Mbps
            nvenc.SetVBVBufferSize(10000000);
            break;

        case RateControlMode.CQP:
            // Mejor para archivo (calidad consistente)
            nvenc.SetRateControl(NVENC_RATE_CONTROL.NVENC_RC_CQP);
            nvenc.SetCQP(23);  // Valor QP: menor = mejor calidad
            // No se necesitan configuraciones de tasa de bits para CQP
            break;
    }
}
```

---
### Ejemplo 4: Preajustes de Calidad NVENC
Equilibrio entre velocidad y calidad.
#### Preajustes de Calidad en C#
```csharp
public void SetQualityPreset(INVEncConfig nvenc, string useCase)
{
    switch (useCase.ToLower())
    {
        case "realtime":
            // Codificación más rápida (transmisión en vivo)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P1);
            nvenc.SetBFrames(0);  // Sin B-frames para baja latencia
            break;
        case "fast":
            // Codificación rápida
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P2);
            nvenc.SetBFrames(1);
            break;
        case "balanced":
            // Velocidad/calidad equilibrada (recomendado)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
            nvenc.SetBFrames(2);
            break;
        case "quality":
            // Mayor calidad
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P6);
            nvenc.SetBFrames(3);
            break;
        case "maximum":
            // Mejor calidad (más lento)
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P7);
            nvenc.SetBFrames(4);
            break;
        default:
            // Por defecto a equilibrado
            nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P4);
            nvenc.SetBFrames(2);
            break;
    }
}
```
---

## Ejemplos de Codificación por Software

### Ejemplo 5: Codificador H.264 por Software

Usar codificación H.264 basada en CPU.

#### H.264 por Software en C#

```csharp
public void EncodeSoftwareH264(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Agregar codificador H.264 por software
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFH264Encoder,  // Codificador por software
        "H.264 Encoder");

    // Configurar codificador (si la interfaz está disponible)
    // var h264Config = encoderFilter as IH264EncoderConfig;
    // Configurar tasa de bits, calidad, etc.

    // Agregar multiplexor
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
Codificar audio a formato AAC.
#### Codificación AAC en C#
```csharp
public void EncodeAACAudio(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Agregar codificador de video (ej. NVENC)
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
    // Agregar codificador de audio AAC
    var audioEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAAEncoder,
        "AAC Encoder");
    // Configurar AAC (si la interfaz está disponible)
    // var aacConfig = audioEncoderFilter as IAACEncoderConfig;
    // aacConfig?.SetBitrate(192000);  // 192 kbps
    // aacConfig?.SetProfile(AAC_PROFILE_LC);
    // Agregar multiplexor
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
    // Ejecutar codificación
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Ejemplo 7: Soporte para Múltiples Códecs de Audio

Soporte para diferentes códecs de audio.

#### Selección de Códec de Audio en C#

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
## Ejemplos de Multiplexación de Contenedores
### Ejemplo 8: Contenedor MP4
Multiplexar video y audio a formato MP4.
#### Multiplexación MP4 en C#
```csharp
public void CreateMP4(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Agregar codificador de video
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    // Agregar codificador de audio
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Agregar multiplexor MP4
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    // Establecer archivo de salida
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Configurar multiplexor (si es necesario)
    // var mp4Config = muxerFilter as IMP4MuxerConfig;
    // mp4Config?.SetFastStart(true);  // Habilitar inicio rápido para web
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

Crear archivos Matroska (MKV).

#### Multiplexación MKV en C#

```csharp
public void CreateMKV(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Agregar codificadores
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");

    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");

    // Agregar multiplexor MKV
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
Crear archivos WebM para entrega web.
#### Multiplexación WebM en C#
```csharp
public void CreateWebM(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Agregar fuente
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Agregar codificador de video VP9 (estándar WebM)
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVP9Encoder,
        "VP9 Encoder");
    // Agregar codificador de audio Opus (estándar WebM)
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFOpusEncoder,
        "Opus Encoder");
    // Agregar multiplexor WebM
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

## Tubería de Codificación Completa

### Ejemplo 11: Codificador con Todas las Funciones

Aplicación de codificación completa con todas las características.

#### Codificador Completo en C#

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
        // Agregar fuente
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Crear codificador de video
        IBaseFilter videoEncoder = CreateVideoEncoder(videoCodec);
        ConfigureVideoEncoder(videoEncoder, videoCodec, videoBitrate);

        // Crear codificador de audio
        IBaseFilter audioEncoder = CreateAudioEncoder(audioCodec);
        ConfigureAudioEncoder(audioEncoder, audioCodec, audioBitrate);

        // Crear multiplexor
        IBaseFilter muxer = CreateMuxer(container);

        // Establecer archivo de salida
        var fileSink = muxer as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Conectar tubería
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
                throw new ArgumentException("Unsupported audio codec");
        }

        return FilterGraphTools.AddFilterFromClsid(filterGraph, clsid, name);
    }

    private void ConfigureAudioEncoder(IBaseFilter encoder, AudioCodec codec, int bitrate)
    {
        // Configurar codificador de audio basado en el tipo de códec
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
var nvenc = encoder as INVEncConfig;
if (nvenc != null)
{
    // Intentar establecer códec
    int hr = nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_H264);
    if (hr != 0)
    {
        Console.WriteLine("NVENC not available - GPU may not support it");
        // Recurrir al codificador por software
    }
}
```
### Problema: Codificación Demasiado Lenta
**Solución**: Ajuste el preajuste de calidad:
```csharp
// Usar preajuste más rápido
nvenc.SetPreset(NVENC_PRESET.NVENC_PRESET_P1);  // Más rápido
nvenc.SetBFrames(0);  // Deshabilitar B-frames
```
### Problema: Tamaño de Archivo de Salida Demasiado Grande
**Solución**: Ajuste la tasa de bits o use un mejor códec:
```csharp
// Bajar tasa de bits
nvenc.SetBitrate(2000000);  // 2 Mbps en lugar de 5
// O usar H.265 para mejor compresión
nvenc.SetCodec(NVENC_CODEC.NVENC_CODEC_HEVC);
```
---

## Ver También

### Documentación

- [Interfaz NVENC](interfaces/nvenc.md) - API completa de NVENC
- [Referencia de Códecs](codecs-reference.md) - Todos los códecs de video/audio
- [Referencia de Multiplexores](muxers-reference.md) - Formatos de contenedor

### Recursos Externos

- [Documentación de NVIDIA NVENC](https://developer.nvidia.com/video-codec-sdk)
- [Especificación H.264](https://www.itu.int/rec/T-REC-H.264)
