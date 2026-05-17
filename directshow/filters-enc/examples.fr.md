---
title: Exemples de filtres d'encodage DirectShow — C++, C#, VB.NET
description: Exemples de code pour l'encodage DirectShow — NVENC GPU, H.264/H.265 logiciel, audio AAC/MP3/Opus et configuration des multiplexeurs MP4/MKV/WebM.
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

# Pack de filtres d'encodage — exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code pratiques pour encoder de la vidéo et de l'audio avec le pack de filtres d'encodage. Elle couvre :

- **Encodeur NVENC** — encodage matériel NVIDIA (H.264/H.265)
- **Encodeurs logiciels** — H.264, H.265, VP8, VP9, MPEG-2
- **Encodeurs audio** — AAC, MP3, Opus, Vorbis, FLAC
- **Multiplexeurs** — MP4, MKV, WebM, MPEG-TS, AVI

---
## Prérequis
### Projets C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "INVEncConfig.h"  // Interface NVENC
#pragma comment(lib, "strmiids.lib")
```
### Projets C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
```
**Paquets NuGet** :
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Exemples d'encodage matériel NVENC

> **GUID de préréglages et profils NVENC** — Les méthodes `SetPreset(Guid)` et `SetProfile(Guid)` acceptent
> des constantes GUID du SDK NVIDIA NVENC. Préréglages réels : `NV_ENC_PRESET_DEFAULT_GUID`,
> `NV_ENC_PRESET_HP_GUID` (hautes performances), `NV_ENC_PRESET_HQ_GUID` (haute qualité),
> `NV_ENC_PRESET_LOW_LATENCY_DEFAULT_GUID`, `NV_ENC_PRESET_LOW_LATENCY_HQ_GUID`,
> `NV_ENC_PRESET_LOW_LATENCY_HP_GUID`, `NV_ENC_PRESET_LOSSLESS_DEFAULT_GUID`,
> `NV_ENC_PRESET_BD_GUID`. Profils H.264 : `NV_ENC_H264_PROFILE_BASELINE_GUID`,
> `NV_ENC_H264_PROFILE_MAIN_GUID`, `NV_ENC_H264_PROFILE_HIGH_GUID`.
> HEVC : `NV_ENC_HEVC_PROFILE_MAIN_GUID`.
> Ces GUID sont définis dans le SDK NVIDIA NVENC (`nvEncodeAPI.h`).

### Exemple 1 : encodage NVENC H.264 de base

Encodez de la vidéo avec l'accélération matérielle NVIDIA.

#### Encodage NVENC H.264 en C#

```csharp
using VisioForge.Core.Types.Output;

// ...

var nvenc = encoderFilter as INVEncConfig;
if (nvenc != null)
{
    nvenc.SetCodec(NVENCEncoder.H264);
    nvenc.SetRateControl(NVENCRateControlMode.CBR);
    nvenc.SetBitrate(5000000);                    // 5 Mbps
    nvenc.SetPreset(NV_ENC_PRESET_DEFAULT_GUID);   // Équilibre qualité/vitesse
    nvenc.SetGOP(60);                              // Image-clé toutes les 60 images
    nvenc.SetBFrames(2);                           // Images B
    nvenc.SetProfile(NV_ENC_H264_PROFILE_HIGH_GUID);
    nvenc.SetLevel(NVENCEncoderLevel.H264_41);     // Niveau 4.1
}
```

#### Encodage NVENC H.264 en C++

```cpp
hr = pEncoder->QueryInterface(IID_INVEncConfig, (void**)&pNVEnc);
if (SUCCEEDED(hr))
{
    pNVEnc->SetCodec(0);                              // H264
    pNVEnc->SetRateControl(2);                        // CBR
    pNVEnc->SetBitrate(5000000);                      // 5 Mbps
    pNVEnc->SetPreset(NV_ENC_PRESET_DEFAULT_GUID);    // Équilibre
    pNVEnc->SetGOP(60);                               // Intervalle d'image-clé
    pNVEnc->SetBFrames(2);                            // Images B
    pNVEnc->SetProfile(NV_ENC_H264_PROFILE_HIGH_GUID);
    pNVEnc->SetLevel(41);                             // Niveau 4.1

    pNVEnc->Release();
}
```

---
### Exemple 2 : encodage NVENC H.265 (HEVC)
Encodez avec H.265 pour une meilleure compression.
#### NVENC H.265 en C#
```csharp
public void EncodeH265(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Ajouter l'encodeur NVENC
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    var nvenc = encoderFilter as INVEncConfig;
    if (nvenc != null)
    {
        nvenc.SetCodec(NVENCEncoder.HEVC);
        nvenc.SetRateControl(NVENCRateControlMode.CONST_QP);
        nvenc.SetQp(23);                            // QP 23 : bon équilibre
        nvenc.SetPreset(NV_ENC_PRESET_HQ_GUID);      // Haute qualité
        nvenc.SetProfile(NV_ENC_HEVC_PROFILE_MAIN_GUID);
        nvenc.SetLevel(NVENCEncoderLevel.H264_41);   // Niveau 4.1
        nvenc.SetGOP(120);                           // 4 secondes a 30 fps
        nvenc.SetBFrames(3);
    }
    // Ajouter le multiplexeur
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connecter et encoder...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Exemple 3 : modes de contrôle de débit NVENC

Différentes stratégies de contrôle de débit pour divers cas d'usage.

#### Exemples de contrôle de débit en C#

```csharp
using VisioForge.Core.Types.Output;

public void ConfigureRateControl(INVEncConfig nvenc, NVENCRateControlMode mode)
{
    switch (mode)
    {
        case NVENCRateControlMode.CBR:
            nvenc.SetRateControl(NVENCRateControlMode.CBR);
            nvenc.SetBitrate(5000000);       // Cible 5 Mbps
            nvenc.SetVbvBitrate(5000000);    // Identique à la cible pour CBR
            nvenc.SetVbvSize(10000000);      // Tampon de 10 Mb
            break;

        case NVENCRateControlMode.VBR:
            nvenc.SetRateControl(NVENCRateControlMode.VBR);
            nvenc.SetBitrate(5000000);       // Moyenne 5 Mbps
            nvenc.SetVbvBitrate(8000000);    // Pic 8 Mbps
            nvenc.SetVbvSize(10000000);
            break;

        case NVENCRateControlMode.CONST_QP:
            nvenc.SetRateControl(NVENCRateControlMode.CONST_QP);
            nvenc.SetQp(23);                 // QP : plus bas = meilleure qualité
            break;
    }
}
```

---
### Exemple 4 : préréglages de qualité NVENC
Équilibre entre vitesse et qualité.
#### Préréglages de qualité en C#
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

## Exemples d'encodage logiciel

### Exemple 5 : encodeur H.264 logiciel

Utilisez l'encodage H.264 basé sur le CPU.

#### H.264 logiciel en C#

```csharp
public void EncodeSoftwareH264(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Ajouter l'encodeur H.264 logiciel
    var encoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFH264Encoder,  // Encodeur logiciel
        "H.264 Encoder");

    // Configurer l'encodeur (si l'interface est disponible)
    // var h264Config = encoderFilter as IH264EncoderConfig;
    // Configurer le débit binaire, la qualité, etc.

    // Ajouter le multiplexeur
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Connecter et encoder
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, encoderFilter, muxerFilter);

    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

---
## Exemples d'encodage audio
### Exemple 6 : encodage audio AAC
Encodez l'audio au format AAC.
#### Encodage AAC en C#
```csharp
public void EncodeAACAudio(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Ajouter l'encodeur vidéo (par exemple NVENC)
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
    // Ajouter l'encodeur audio AAC
    var audioEncoderFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Configurer AAC (si l'interface est disponible)
    // var aacConfig = audioEncoderFilter as IAACEncoderConfig;
    // aacConfig?.SetBitrate(192000);  // 192 kbps
    // aacConfig?.SetProfile(AAC_PROFILE_LC);
    // Ajouter le multiplexeur
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connecter les filtres
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    // Chemin vidéo
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, videoEncoderFilter, muxerFilter);
    // Chemin audio
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, audioEncoderFilter, muxerFilter);
    // Lancer l'encodage
    var mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
---

### Exemple 7 : prise en charge de plusieurs codecs audio

Prise en charge de différents codecs audio.

#### Sélection de codec audio en C#

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
## Exemples de multiplexage de conteneurs
### Exemple 8 : conteneur MP4
Multiplexez vidéo et audio au format MP4.
#### Multiplexage MP4 en C#
```csharp
public void CreateMP4(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Ajouter l'encodeur vidéo
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");
    // Ajouter l'encodeur audio
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");
    // Ajouter le multiplexeur MP4
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMP4Muxer,
        "MP4 Muxer");
    // Définir le fichier de sortie
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Configurer le multiplexeur (si nécessaire)
    // var mp4Config = muxerFilter as IMP4MuxerConfig;
    // mp4Config?.SetFastStart(true);  // Activer le demarrage rapide pour le web
    // Connecter les filtres
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

### Exemple 9 : conteneur MKV

Créez des fichiers Matroska (MKV).

#### Multiplexage MKV en C#

```csharp
public void CreateMKV(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

    // Ajouter les encodeurs
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFNVENCEncoder,
        "NVENC Encoder");

    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFAACEncoder,
        "AAC Encoder");

    // Ajouter le multiplexeur MKV
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFMKVMuxer,
        "MKV Muxer");

    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);

    // Connecter et encoder
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
### Exemple 10 : conteneur WebM
Créez des fichiers WebM pour la diffusion web.
#### Multiplexage WebM en C#
```csharp
public void CreateWebM(string inputFile, string outputFile)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();
    // Ajouter la source
    filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);
    // Ajouter l'encodeur vidéo VP9 (standard WebM)
    var videoEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVP9Encoder,
        "VP9 Encoder");
    // Ajouter l'encodeur audio Opus (standard WebM)
    var audioEncoder = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFOpusEncoder,
        "Opus Encoder");
    // Ajouter le multiplexeur WebM
    var muxerFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFWebMMuxer,
        "WebM Muxer");
    var fileSink = muxerFilter as IFileSinkFilter;
    fileSink?.SetFileName(outputFile, null);
    // Connecter et encoder
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

## Pipeline d'encodage complet

### Exemple 11 : encodeur complet

Application d'encodage complète avec toutes les fonctionnalités.

#### Encodeur complet en C#

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
        // Ajouter la source
        filterGraph.AddSourceFilter(inputFile, "Source", out IBaseFilter sourceFilter);

        // Créer l'encodeur vidéo
        IBaseFilter videoEncoder = CreateVideoEncoder(videoCodec);
        ConfigureVideoEncoder(videoEncoder, videoCodec, videoBitrate);

        // Créer l'encodeur audio
        IBaseFilter audioEncoder = CreateAudioEncoder(audioCodec);
        ConfigureAudioEncoder(audioEncoder, audioCodec, audioBitrate);

        // Créer le multiplexeur
        IBaseFilter muxer = CreateMuxer(container);

        // Définir le fichier de sortie
        var fileSink = muxer as IFileSinkFilter;
        fileSink?.SetFileName(outputFile, null);

        // Connecter le pipeline
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
                throw new ArgumentException("Unsupported vidéo codec");
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
        // Configurer d'autres encodeurs...
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
        // Configurer l'encodeur audio selon le type de codec
        // (Configuration specifique a l'interface)
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
## Dépannage
### Problème : NVENC non disponible
**Solution** : vérifiez la compatibilité du GPU :
```csharp
var nvenc2 = encoder as INVEncConfig2;
if (nvenc2 != null)
{
    int hr = nvenc2.CheckNVENCAvailable(out bool available, out int status);
    if (hr != 0 || !available)
    {
        Console.WriteLine("NVENC not available - GPU may not support it");
        // Solution de repli vers l'encodeur logiciel
    }
}
```
### Problème : encodage trop lent
**Solution** : ajustez le préréglage de qualité :
```csharp
// Utiliser un preset plus rapide
nvenc.SetPreset(NV_ENC_PRESET_LOW_LATENCY_HP_GUID);
nvenc.SetBFrames(0);  // Desactiver les images B
```
### Problème : fichier de sortie trop volumineux
**Solution** : ajustez le débit binaire ou utilisez un meilleur codec :
```csharp
// Reduire le débit binaire
nvenc.SetBitrate(2000000);  // 2 Mbps au lieu de 5
// Ou utiliser H.265 pour une meilleure compression
nvenc.SetCodec(NVENCEncoder.HEVC);
```
---

## Voir aussi

### Documentation

- [Interface NVENC](interfaces/nvenc.md) — API NVENC complète
- [Référence des codecs](codecs-reference.md) — tous les codecs vidéo/audio
- [Référence des multiplexeurs](muxers-reference.md) — formats de conteneurs

### Ressources externes

- [Documentation NVIDIA NVENC](https://developer.nvidia.com/video-codec-sdk)
- [Spécification H.264](https://www.itu.int/rec/T-REC-H.264)
