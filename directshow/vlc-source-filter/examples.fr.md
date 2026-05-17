---
title: Exemples du filtre source DirectShow VLC — code C++/C#
description: Exemples de code pour le filtre source VLC avec audio multi-piste, sous-titres, vidéo 360° et paramètres VLC personnalisés dans DirectShow.
tags:
  - DirectShow
  - C++
  - Windows
  - WinForms
  - Playback
  - Streaming
  - Decoding
  - RTSP
  - HLS
  - MP4
  - MKV
  - AVI
  - MOV
  - C#
  - VB.NET
primary_api_classes:
  - IBaseFilter
  - IVFRegister

---

# Exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code pratiques pour utiliser le filtre source VLC dans les applications DirectShow. Le filtre source VLC prend en charge plusieurs pistes audio, les sous-titres, la vidéo 360° et des options de ligne de commande VLC personnalisées.

---
## Prérequis
### Projets C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "ivlcsrc.h"      // En-tete unique du SDK — declare IVlcSrc, IVlcSrc2, IVlcSrc3 + GUID CLSID/IID
#pragma comment(lib, "strmiids.lib")
```
### Projets C#
```csharp
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
using System.Runtime.InteropServices;
using System.Text;
```
**Paquets NuGet** :
- VisioForge.DirectShowAPI
- MediaFoundationCore
---

## Exemple 1 : lecture de fichier de base

Lire un fichier multimédia local avec le filtre source VLC.

### Implémentation C++

```cpp
#include <dshow.h>
#include "ivlcsrc.h"

// CLSID du filtre source VLC
DEFINE_GUID(CLSID_VFVLCSource,
    0x3fc97748, 0x7cb6, 0x4195, 0x89, 0xde, 0x07, 0x17, 0x58, 0x2a, 0x48, 0x63);

HRESULT PlayVLCFile(LPCWSTR filename, HWND hVideoWindow)
{
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pSourceFilter = NULL;
    IVlcSrc* pVlcSrc = NULL;

    HRESULT hr = S_OK;

    // Creer le graphe de filtres
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Creer le filtre source VLC
    hr = CoCreateInstance(CLSID_VFVLCSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Ajouter le filtre au graphe
    hr = pGraph->AddFilter(pSourceFilter, L"VLC Source");
    if (FAILED(hr)) goto cleanup;

    // Obtenir l'interface VLC
    hr = pSourceFilter->QueryInterface(IID_IVlcSrc, (void**)&pVlcSrc);
    if (FAILED(hr)) goto cleanup;

    // Charger le fichier
    hr = pVlcSrc->SetFile((WCHAR*)filename);
    if (FAILED(hr)) goto cleanup;

    // Construire et rendre le graphe
    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Rendre video et audio
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Demarrer le graphe
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

### Implémentation C#

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;

            // Creer et ajouter le filtre source VLC
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Charger le fichier via l'interface IVlcSrc
            var vlcSrc = sourceFilter as IVlcSrc;
            if (vlcSrc != null)
            {
                int hr = vlcSrc.SetFile(filename);
                DsError.ThrowExceptionForHR(hr);
            }

            // Rendre les flux
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            int result = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(result);

            captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

            // Configurer la fenetre video
            videoWindow.put_Owner(videoWindowHandle);
            videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
            videoWindow.put_Visible(OABool.True);

            // Demarrer le graphe
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

### Implémentation VB.NET

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
            ' Creer le graphe de filtres
            filterGraph = DirectCast(New FilterGraph(), IFilterGraph2)
            mediaControl = DirectCast(filterGraph, IMediaControl)
            videoWindow = DirectCast(filterGraph, IVideoWindow)

            ' Creer et ajouter le filtre source VLC
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source")

            ' Charger le fichier
            Dim vlcSrc = DirectCast(sourceFilter, IVlcSrc)
            If vlcSrc IsNot Nothing Then
                Dim hr As Integer = vlcSrc.SetFile(filename)
                DsError.ThrowExceptionForHR(hr)
            End If

            ' Rendre les flux
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            captureGraph.SetFiltergraph(filterGraph)

            captureGraph.RenderStream(Nothing, MediaType.Video, sourceFilter, Nothing, Nothing)
            captureGraph.RenderStream(Nothing, MediaType.Audio, sourceFilter, Nothing, Nothing)

            ' Configurer la fenetre video
            videoWindow.put_Owner(videoWindowHandle)
            videoWindow.put_WindowStyle(WindowStyle.Child Or WindowStyle.ClipSiblings)
            videoWindow.put_Visible(OABool.True)

            ' Demarrer le graphe
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
## Exemple 2 : sélection de piste audio
Lister et sélectionner les pistes audio des fichiers multi-audio.
### Gestion des pistes audio en C#
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
        // Creer le filtre source VLC
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFVLCSource,
            "VLC Source");
        // Charger le fichier
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
            // Obtenir le nombre de pistes audio
            int audioCount = 0;
            vlcSrc.GetAudioTracksCount(out audioCount);
            Console.WriteLine($"Total audio tracks: {audioCount}");
            // Lister toutes les pistes audio
            for (int i = 0; i < audioCount; i++)
            {
                int trackId;
                StringBuilder trackName = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);
                Console.WriteLine($"Track {i}: ID={trackId}, Name={trackName}");
            }
            // Obtenir la piste active courante
            int currentTrackId;
            vlcSrc.GetAudioTrack(out currentTrackId);
            Console.WriteLine($"Currently active track ID: {currentTrackId}");
            // Selectionner une piste specifique (par ex. index 1)
            if (audioCount > 1)
            {
                int desiredTrackId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
                vlcSrc.SetAudioTrack(desiredTrackId);
                Console.WriteLine($"Switched to track: {name}");
            }
        }
        // Construire et lancer le graphe
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
    // Methode pour changer de piste audio pendant la lecture
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
### Gestion des pistes audio en C++
```cpp
void ListAndSelectAudioTracks(IVlcSrc* pVlcSrc)
{
    int audioCount = 0;
    pVlcSrc->GetAudioTracksCount(&audioCount);
    wprintf(L"Total audio tracks: %d\n", audioCount);
    // Lister toutes les pistes
    for (int i = 0; i < audioCount; i++)
    {
        int trackId = 0;
        WCHAR trackName[256] = {0};
        pVlcSrc->GetAudioTrackInfo(i, &trackId, trackName);
        wprintf(L"Track %d: ID=%d, Name=%s\n", i, trackId, trackName);
    }
    // Obtenir la piste courante
    int currentId = 0;
    pVlcSrc->GetAudioTrack(&currentId);
    wprintf(L"Current track ID: %d\n", currentId);
    // Selectionner la piste par index (par ex. piste 1)
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

## Exemple 3 : gestion des sous-titres

Sélectionner et gérer les pistes de sous-titres.

### Gestion des sous-titres en C#

```csharp
public class VLCSubtitleExample
{
    private IBaseFilter sourceFilter;

    public void ManageSubtitles(string filename)
    {
        // On suppose que le filtre est deja cree et ajoute au graphe
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);

            // Obtenir le nombre de sous-titres
            int subtitleCount = 0;
            vlcSrc.GetSubtitlesCount(out subtitleCount);

            Console.WriteLine($"Total subtitle tracks: {subtitleCount}");

            // Lister tous les sous-titres
            for (int i = 0; i < subtitleCount; i++)
            {
                int subtitleId;
                StringBuilder subtitleName = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(i, out subtitleId, subtitleName);

                Console.WriteLine($"Subtitle {i}: ID={subtitleId}, Name={subtitleName}");
            }

            // Activer un sous-titre (par ex. index 0)
            if (subtitleCount > 0)
            {
                int subtitleId;
                StringBuilder name = new StringBuilder(256);
                vlcSrc.GetSubtitleInfo(0, out subtitleId, name);

                vlcSrc.SetSubtitle(subtitleId);
                Console.WriteLine($"Enabled subtitle: {name}");
            }

            // Desactiver les sous-titres (utiliser ID -1)
            // vlcSrc.SetSubtitle(-1);
        }
    }

    // Methode pour changer les sous-titres pendant la lecture
    public void SwitchSubtitle(int subtitleIndex)
    {
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            if (subtitleIndex < 0)
            {
                // Desactiver les sous-titres
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

### Gestion des sous-titres en C++

```cpp
void ManageSubtitles(IVlcSrc* pVlcSrc)
{
    int subtitleCount = 0;
    pVlcSrc->GetSubtitlesCount(&subtitleCount);

    wprintf(L"Total subtitles: %d\n", subtitleCount);

    // Lister tous les sous-titres
    for (int i = 0; i < subtitleCount; i++)
    {
        int subtitleId = 0;
        WCHAR subtitleName[256] = {0};
        pVlcSrc->GetSubtitleInfo(i, &subtitleId, subtitleName);

        wprintf(L"Subtitle %d: ID=%d, Name=%s\n", i, subtitleId, subtitleName);
    }

    // Activer le premier sous-titre
    if (subtitleCount > 0)
    {
        int id = 0;
        WCHAR name[256] = {0};
        pVlcSrc->GetSubtitleInfo(0, &id, name);

        pVlcSrc->SetSubtitle(id);
        wprintf(L"Enabled subtitle: %s\n", name);
    }

    // Desactiver les sous-titres
    // pVlcSrc->SetSubtitle(-1);
}
```

---
## Exemple 4 : options de ligne de commande VLC personnalisées
Transmettre des paramètres VLC personnalisés via l'interface IVlcSrc2.
### Paramètres VLC personnalisés en C#
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
        // Definir des options VLC personnalisees via IVlcSrc2
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            // Creer la liste des parametres de ligne de commande VLC
            var parameters = new List<string>();
            parameters.Add("--avcodec-hw=any");          // Activer le decodage materiel
            parameters.Add("--network-caching=1000");    // Tampon reseau de 1 seconde
            parameters.Add("--live-caching=300");        // 300 ms pour flux en direct
            parameters.Add("--file-caching=300");        // 300 ms pour fichiers
            parameters.Add("--sout-mux-caching=2000");   // Tampon de multiplexage de sortie
            parameters.Add("--vout=direct3d11");         // Utiliser le moteur Direct3D11
            parameters.Add("--verbose=2");               // Niveau de journalisation
            // Convertir les chaines en tableau IntPtr UTF-8 natif
            var array = new IntPtr[parameters.Count];
            for (int i = 0; i < parameters.Count; i++)
            {
                array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
            }
            try
            {
                // Appeler SetCustomCommandLine avec le tableau IntPtr
                int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Count);
                DsError.ThrowExceptionForHR(hr);
            }
            finally
            {
                // Liberer la memoire non manageed allouee
                for (int i = 0; i < array.Length; i++)
                {
                    Marshal.FreeHGlobal(array[i]);
                }
            }
        }
        // Charger le fichier via IVlcSrc
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }
        // Construire et lancer le graphe
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
### Options VLC courantes
Voici des combinaisons de paramètres VLC courantes. Pensez à utiliser le motif de marshalling approprié montré ci-dessus.
```csharp
// Options de streaming reseau
var networkOptions = new List<string>
{
    "--network-caching=3000",      // 3 secondes pour flux reseau
    "--rtsp-tcp",                  // Forcer TCP pour RTSP
    "--http-reconnect"             // Reconnexion auto des flux HTTP
};
// Options de decodage materiel
var hwDecodeOptions = new List<string>
{
    "--avcodec-hw=any"             // Detection automatique du materiel
    // Options alternatives :
    // "--avcodec-hw=dxva2"        // Utiliser DXVA2
    // "--avcodec-hw=d3d11va"      // Utiliser D3D11VA
    // "--avcodec-hw=nvdec"        // Utiliser NVIDIA NVDEC
};
// Options de faible latence
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
// Options video 360°
var video360Options = new List<string>
{
    "--video-filter=transform",
    "--transform-type=hflip",
    "--vout-filter=rotate"
};
// Options de sous-titres
var subtitleOptions = new List<string>
{
    "--sub-autodetect-file",       // Detection auto des fichiers de sous-titres
    "--sub-language=eng",          // Langue de sous-titres preferee
    "--freetype-fontsize=20"       // Taille de police des sous-titres
};
// Options audio
var audioOptions = new List<string>
{
    "--audio-desync=0",            // Synchronisation audio
    "--audiotrack-language=eng",   // Langue audio preferee
    "--audio-filter=normvol"       // Normalisation du volume
};
// Exemple complet avec plusieurs options
var completeOptions = new List<string>
{
    "--avcodec-hw=any",
    "--network-caching=1000",
    "--rtsp-tcp",
    "--sub-autodetect-file",
    "--verbose=1"
};
// Methode utilitaire pour appliquer les parametres VLC
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

## Exemple 5 : surcharge de la fréquence d'images

Surcharger la fréquence d'images source via l'interface IVlcSrc3.

### Surcharge de fréquence d'images en C#

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

        // Definir une frequence d'images personnalisee via IVlcSrc3
        var vlcSrc3 = sourceFilter as IVlcSrc3;
        if (vlcSrc3 != null)
        {
            // Surcharger la frequence d'images (par ex. 30.0, 25.0, 60.0)
            int hr = vlcSrc3.SetDefaultFrameRate(fps);
            DsError.ThrowExceptionForHR(hr);

            Console.WriteLine($"Frame rate set to: {fps} fps");
        }

        // Charger le fichier
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            vlcSrc.SetFile(filename);
        }

        // Construire et lancer le graphe
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

### Surcharge de fréquence d'images en C++

```cpp
#include "ivlcsrc.h"

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
## Exemple 6 : streaming réseau (RTSP/HLS)
Diffuser depuis des sources réseau.
### Streaming réseau en C#
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
        // Configurer pour le streaming RTSP
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--rtsp-tcp",                         // Utiliser le transport TCP
                "--network-caching=300",              // Faible latence (300 ms)
                "--rtsp-frame-buffer-size=500000",    // Taille du tampon d'images
                "--drop-late-frames",                 // Supprimer les images en retard
                "--skip-frames"                       // Sauter des images si necessaire
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
        // Charger le flux RTSP
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Exemple : "rtsp://camera.example.com:554/stream"
            vlcSrc.SetFile(rtspUrl);
        }
        // Construire et lancer le graphe
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
        // Configurer pour le streaming HLS
        var vlcSrc2 = sourceFilter as IVlcSrc2;
        if (vlcSrc2 != null)
        {
            var parameters = new List<string>
            {
                "--http-reconnect",                // Reconnexion auto
                "--network-caching=3000",          // Tampon de 3 secondes pour HLS
                "--hls-segment-threads=3",         // Telechargement parallele de segments
                "--avcodec-hw=any"                 // Decodage materiel
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
        // Charger le flux HLS
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            // Exemple : "https://example.com/stream/playlist.m3u8"
            vlcSrc.SetFile(hlsUrl);
        }
        // Construire et lancer le graphe
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

## Exemple 7 : activation de licence

Activer une licence achetée.

### Activation de licence en C#

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    var filterGraph = (IFilterGraph2)new FilterGraph();

    var sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFVLCSource,
        "VLC Source");

    // Activer la licence
    var registration = sourceFilter as IVFRegister;
    if (registration != null)
    {
        int hr = registration.SetLicenseKey(licenseKey);
        if (hr != 0)
        {
            throw new Exception("License activation failed");
        }
    }

    // Charger et lire le fichier
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
## Exemple 8 : lecteur multi-piste complet
Lecteur multimédia complet avec toutes les fonctionnalités du filtre source VLC.
### Exemple complet en C#
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
        // Enregistrer la licence
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Definir des options VLC personnalisees
        if (!string.IsNullOrEmpty(vlcOptions))
        {
            var vlcSrc2 = sourceFilter as IVlcSrc2;
            if (vlcSrc2 != null)
            {
                // Analyser les options separees par des espaces en liste
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
        // Surcharger la frequence d'images
        if (frameRate.HasValue)
        {
            var vlcSrc3 = sourceFilter as IVlcSrc3;
            vlcSrc3?.SetDefaultFrameRate(frameRate.Value);
        }
        // Charger le fichier
        var vlcSrc = sourceFilter as IVlcSrc;
        if (vlcSrc != null)
        {
            int hr = vlcSrc.SetFile(filename);
            DsError.ThrowExceptionForHR(hr);
            // Enumerer les pistes audio
            LoadAudioTracks(vlcSrc);
            // Enumerer les sous-titres
            LoadSubtitles(vlcSrc);
        }
        // Construire le graphe
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
                vlcSrc.SetSubtitle(-1); // Desactiver
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
### Exemple d'utilisation
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
            // Options VLC personnalisees pour le decodage materiel
            string vlcOptions = "--avcodec-hw=any --network-caching=1000";
            player.LoadFile(dlg.FileName, vlcOptions: vlcOptions);
            player.SetVideoWindow(panelVideo.Handle, panelVideo.Width, panelVideo.Height);
            // Remplir la combo box de pistes audio
            comboAudioTracks.Items.Clear();
            foreach (var track in player.AudioTracks)
            {
                comboAudioTracks.Items.Add(track.Name);
            }
            if (comboAudioTracks.Items.Count > 0)
            {
                comboAudioTracks.SelectedIndex = 0;
            }
            // Remplir la combo box de sous-titres
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
        player.SelectSubtitle(comboSubtitles.SelectedIndex - 1); // -1 a cause de l'item "None"
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

## Dépannage

### Problème : erreur « Class not registered »

**Solution** : vérifiez que le filtre source VLC est enregistré :
```bash
regsvr32 VisioForge_VLC_Source.ax
```

### Problème : le nombre de pistes audio/sous-titres renvoie 0

**Cause** : les pistes ne sont pas encore disponibles (le graphe de filtres n'a pas été construit)

**Solution** : les informations sur l'audio et les sous-titres ne sont disponibles qu'après le chargement du fichier et la construction du graphe. Appelez les méthodes d'énumération de pistes après `SetFile()` et la construction du graphe.

```cpp
// Charger le fichier d'abord
pVlcSrc->SetFile(L"movie.mkv");

// Construire le graphe de filtres
pGraph->RenderFile(L"movie.mkv", nullptr);

// MAINTENANT interroger les pistes
int count = 0;
pVlcSrc->GetAudioTracksCount(&count);
```

### Problème : les options VLC personnalisées ne fonctionnent pas

**Solution** : assurez-vous que les options VLC sont définies avant de charger le fichier, et utilisez le marshalling correct :

```csharp
// Ordre et usage corrects :
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
        vlcSrc2.SetCustomCommandLine(array, parameters.Count);  // 1. Definir les options d'abord
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
vlcSrc?.SetFile(filename);  // 2. Puis charger le fichier
// Construire et lancer le graphe...
```

### Problème : le flux RTSP a une latence élevée

**Solution** : configurez VLC pour une mise en tampon réseau minimale et utilisez le transport TCP :

```cpp
// Exemple C++
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
    
    // Convertir en chaines larges puis en equivalents IntPtr
    // (en C++, vous pouvez passer des chaines ANSI/UTF-8 directement dans certains cas,
    // mais pour la coherence avec l'interface, utilisez une conversion appropriee)
    
    pVlcSrc2->Release();
}
```

```csharp
// Exemple C#
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

### Problème : l'accélération matérielle ne fonctionne pas

**Solution** : spécifiez explicitement le décodeur matériel à utiliser :

```cpp
// Exemple C++ - essayer un decodeur materiel explicite
IVlcSrc2* pVlcSrc2 = nullptr;
hr = pFilter->QueryInterface(IID_IVlcSrc2, (void**)&pVlcSrc2);
if (SUCCEEDED(hr))
{
    const char* params[] = {
        "--avcodec-hw=d3d11va"  // Pour Windows 8+
        // ou "--avcodec-hw=dxva2" pour Windows 7
    };
    
    // Conversion et appel appropries requis
    
    pVlcSrc2->Release();
}
```

```csharp
// Exemple C#
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var hwOptions = new List<string>
    {
        "--avcodec-hw=dxva2"  // ou d3d11va, nvdec, any
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
## Voir aussi
### Documentation
- [Référence des interfaces](interface-reference.md) — référence API complète
- [Guide de déploiement](../deployment/index.md) — déploiement du filtre
### Ressources externes
- [Documentation de ligne de commande VLC](https://www.videolan.org/doc/)
- [Guide de programmation DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)
