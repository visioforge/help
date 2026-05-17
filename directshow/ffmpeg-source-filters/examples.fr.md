---
title: Exemples du filtre source FFmpeg — C++, C#, VB.NET
description: Graphes DirectShow avec le filtre source FFmpeg — lecture de fichiers, décodage matériel, streaming et tampon en C++, C# et VB.NET.
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
  - IFileSourceFilter
  - IFFMPEGSourceSettings
  - IBaseFilter
  - IVFRegister

---

# Exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code pratiques pour utiliser le filtre source FFMPEG dans les applications DirectShow. Les exemples sont fournis en C++, C# et VB.NET.

## Exemples fonctionnels complets

**Référentiel GitHub officiel** : tous les exemples présentés sur cette page sont disponibles sous forme de projets Visual Studio complets et fonctionnels dans notre référentiel d'exemples GitHub :

🔗 **[Référentiel d'exemples DirectShow](https://github.com/visioforge/directshow-samples)**

### Exemples du filtre source FFMPEG

- **[Exemple C#](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** — lecteur multimédia complet avec toutes les capacités du filtre
- **[Exemple VB.NET](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** — implémentation VB.NET
- **[Exemple C++Builder](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** — implémentation C++

Chaque exemple inclut :
- Fichiers de projet Visual Studio/C++Builder complets
- Code fonctionnel pour la lecture, la sélection de flux et la configuration
- Exemples d'accélération matérielle
- Exemples de streaming réseau (RTSP/HLS)

---
## Prérequis
### Projets C++
```cpp
#include <dshow.h>
#include <streams.h>
#include "IFFMPEGSourceSettings.h"  // Issu du SDK
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

## Exemple 1 : lecture de fichier de base

Lire un fichier multimédia local avec les paramètres par défaut.

### Implémentation C++

```cpp
#include <dshow.h>
#include <windows.h>
#include "IFFMPEGSourceSettings.h"

// CLSID du filtre source FFMPEG
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

    // Creer le graphe de filtres
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr)) return hr;

    // Creer le filtre source FFMPEG
    hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) goto cleanup;

    // Ajouter le filtre au graphe
    hr = pGraph->AddFilter(pSourceFilter, L"FFMPEG Source");
    if (FAILED(hr)) goto cleanup;

    // Charger le fichier
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr)) goto cleanup;

    hr = pFileSource->Load(filename, NULL);
    if (FAILED(hr)) goto cleanup;

    // Effectuer le rendu des flux automatiquement
    hr = pGraph->QueryInterface(IID_IGraphBuilder, (void**)&pGraph);

    ICaptureGraphBuilder2* pBuild = NULL;
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (SUCCEEDED(hr))
    {
        hr = pBuild->SetFiltergraph(pGraph);

        // Rendre le flux video
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pSourceFilter, NULL, NULL);

        // Rendre le flux audio
        hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pSourceFilter, NULL, NULL);

        pBuild->Release();
    }

    // Configurer la fenetre video
    hr = pGraph->QueryInterface(IID_IVideoWindow, (void**)&pVideoWindow);
    if (SUCCEEDED(hr))
    {
        pVideoWindow->put_Owner((OAHWND)hVideoWindow);
        pVideoWindow->put_WindowStyle(WS_CHILD | WS_CLIPSIBLINGS);

        RECT rc;
        GetClientRect(hVideoWindow, &rc);
        pVideoWindow->SetWindowPosition(0, 0, rc.right, rc.bottom);
    }

    // Obtenir l'interface de controle
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (FAILED(hr)) goto cleanup;

    // Demarrer le graphe
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

### Implémentation C#

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            mediaControl = (IMediaControl)filterGraph;
            videoWindow = (IVideoWindow)filterGraph;

            // Creer et ajouter le filtre source FFMPEG
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Charger le fichier
            var fileSource = sourceFilter as IFileSourceFilter;
            int hr = fileSource.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Rendre les flux
            ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Rendre la video
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
            // Rendre l'audio
            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

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

### Implémentation VB.NET

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
            ' Creer le graphe de filtres
            filterGraph = DirectCast(New FilterGraph(), IFilterGraph2)
            mediaControl = DirectCast(filterGraph, IMediaControl)
            videoWindow = DirectCast(filterGraph, IVideoWindow)

            ' Creer et ajouter le filtre source FFMPEG
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source")

            ' Charger le fichier
            Dim fileSource = DirectCast(sourceFilter, IFileSourceFilter)
            Dim hr As Integer = fileSource.Load(filename, Nothing)
            DsError.ThrowExceptionForHR(hr)

            ' Rendre les flux
            Dim captureGraph As ICaptureGraphBuilder2 = DirectCast(New CaptureGraphBuilder2(), ICaptureGraphBuilder2)
            hr = captureGraph.SetFiltergraph(filterGraph)
            DsError.ThrowExceptionForHR(hr)

            ' Rendre video et audio
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
## Exemple 2 : accélération matérielle
Activer le décodage GPU pour de meilleures performances.
### C++ avec accélération matérielle
```cpp
HRESULT PlayFileWithGPU(LPCWSTR filename)
{
    IBaseFilter* pSourceFilter = NULL;
    IFFMPEGSourceSettings* pSettings = NULL;
    // Creer le filtre source
    HRESULT hr = CoCreateInstance(CLSID_VFFFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                                  IID_IBaseFilter, (void**)&pSourceFilter);
    if (FAILED(hr)) return hr;
    // Obtenir l'interface de configuration
    hr = pSourceFilter->QueryInterface(IID_IFFMPEGSourceSettings, (void**)&pSettings);
    if (FAILED(hr))
    {
        pSourceFilter->Release();
        return hr;
    }
    // Activer l'acceleration materielle (NVDEC/QuickSync/DXVA)
    hr = pSettings->SetHWAccelerationEnabled(TRUE);
    if (FAILED(hr))
    {
        pSettings->Release();
        pSourceFilter->Release();
        return hr;
    }
    // Charger le fichier
    IFileSourceFilter* pFileSource = NULL;
    hr = pSourceFilter->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr))
    {
        hr = pFileSource->Load(filename, NULL);
        pFileSource->Release();
    }
    // Continuer la construction du graphe...
    // (Ajouter au graphe, rendre les flux, etc.)
    pSettings->Release();
    pSourceFilter->Release();
    return hr;
}
```
### C# avec accélération matérielle
```csharp
public void PlayFileWithHardwareAcceleration(string filename, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    // Creer le filtre source
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Configurer l'acceleration materielle
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Activer le decodage GPU
        int hr = settings.SetHWAccelerationEnabled(true);
        DsError.ThrowExceptionForHR(hr);
    }
    // Charger le fichier
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Construire et lancer le graphe...
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

## Exemple 3 : streaming réseau (RTSP/HLS)

Diffuser de la vidéo depuis des sources réseau.

### Streaming RTSP en C#

```csharp
public void PlayRTSPStream(string rtspUrl, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

    // Configurer pour le streaming reseau
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        // Definir le mode de mise en tampon pour les flux reseau
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Definir le delai d'attente de connexion (en secondes)
        settings.SetLoadTimeOut(30);

        // Activer le decodage materiel pour les performances
        settings.SetHWAccelerationEnabled(true);
    }

    // Charger le flux RTSP
    // Exemple : "rtsp://camera.example.com:554/stream"
    var fileSource = sourceFilter as IFileSourceFilter;
    int hr = fileSource.Load(rtspUrl, null);
    DsError.ThrowExceptionForHR(hr);

    // Construire le graphe
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);

    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

    // Configurer la fenetre video
    videoWindow = (IVideoWindow)filterGraph;
    videoWindow.put_Owner(videoWindowHandle);
    videoWindow.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipSiblings);
    videoWindow.put_Visible(OABool.True);

    // Demarrer
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();

    Marshal.ReleaseComObject(captureGraph);
}
```

### Streaming HLS en C#

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
        // Les flux HLS beneficient de la mise en tampon
        settings.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);

        // Delai plus long pour le chargement de la playlist HLS
        settings.SetLoadTimeOut(60);

        // Acceleration materielle
        settings.SetHWAccelerationEnabled(true);
    }

    // Charger le flux HLS
    // Exemple : "https://example.com/stream/playlist.m3u8"
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(hlsUrl, null);

    // Construire et lancer le graphe (idem exemple RTSP)
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
## Exemple 4 : options FFmpeg personnalisées
Transmettre des options de démultiplexeur/décodeur FFmpeg personnalisées.
### C# avec options personnalisées
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
        // Definir des options FFmpeg personnalisees
        // Format : "cle=valeur" pour chaque option
        // Exemple 1 : definir la taille du tampon pour les flux reseau
        settings.SetCustomOption("buffer_size", "1024000");
        // Exemple 2 : activer le mode faible latence
        settings.SetCustomOption("fflags", "nobuffer");
        // Exemple 3 : definir la duree d'analyse (microsecondes)
        settings.SetCustomOption("analyzeduration", "1000000");
        // Exemple 4 : definir la taille de sondage
        settings.SetCustomOption("probesize", "5000000");
        // Exemple 5 : protocole de transport RTSP
        settings.SetCustomOption("rtsp_transport", "tcp");
        // Exemple 6 : definir le delai (microsecondes)
        settings.SetCustomOption("timeout", "5000000");
    }
    // Charger le fichier
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Construire le graphe...
    ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
    captureGraph.SetFiltergraph(filterGraph);
    captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
    captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
    mediaControl = (IMediaControl)filterGraph;
    mediaControl.Run();
    Marshal.ReleaseComObject(captureGraph);
}
```
### Options FFmpeg courantes
```csharp
// Options de streaming reseau
settings.SetCustomOption("rtsp_transport", "tcp");        // Utiliser TCP pour RTSP
settings.SetCustomOption("rtsp_flags", "prefer_tcp");     // Preferer TCP a UDP
settings.SetCustomOption("timeout", "10000000");          // Delai de 10 secondes
settings.SetCustomOption("stimeout", "5000000");          // Delai socket de 5 secondes
// Options de tampon et de sondage
settings.SetCustomOption("buffer_size", "2097152");       // Tampon de 2 Mo
settings.SetCustomOption("analyzeduration", "2000000");   // Analyse de 2 secondes
settings.SetCustomOption("probesize", "10000000");        // Sondage de 10 Mo
// Options de faible latence
settings.SetCustomOption("fflags", "nobuffer");           // Desactiver la mise en tampon
settings.SetCustomOption("flags", "low_delay");           // Indicateur de faible latence
settings.SetCustomOption("framedrop", "1");               // Autoriser la perte d'images
// Options HTTP
settings.SetCustomOption("user_agent", "MyApp/1.0");     // User agent personnalise
settings.SetCustomOption("headers", "Authorization: Bearer token");
// Effacer toutes les options personnalisees
settings.ClearCustomOptions();
```
---

## Exemple 5 : configuration du mode de mise en tampon

Contrôler le comportement de mise en tampon pour différents scénarios.

### Exemples de mise en tampon en C#

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
                // Mode auto - laisser le filtre decider
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);
                break;

            case BufferingScenario.NetworkStream:
                // Activer la mise en tampon pour une lecture fluide
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_ON);
                settings.SetLoadTimeOut(30);  // Delai de 30 secondes
                break;

            case BufferingScenario.LowLatency:
                // Desactiver la mise en tampon pour une latence minimale
                settings.SetBufferingMode(
                    FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_OFF);
                settings.SetCustomOption("fflags", "nobuffer");
                settings.SetCustomOption("flags", "low_delay");
                break;
        }
    }

    // Charger la source
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(source, null);

    // Construire et lancer le graphe...
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
## Exemple 6 : sélection multi-flux
Sélectionner des pistes vidéo et audio spécifiques.
### Sélection de flux en C#
```csharp
public void PlayWithStreamSelection(string filename, int videoStreamIndex, int audioStreamIndex, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    // Charger le fichier d'abord
    var fileSource = sourceFilter as IFileSourceFilter;
    fileSource.Load(filename, null);
    // Obtenir les flux disponibles
    var streamSelect = sourceFilter as IAMStreamSelect;
    if (streamSelect != null)
    {
        streamSelect.Count(out int streamCount);
        List<int> videoStreams = new List<int>();
        List<int> audioStreams = new List<int>();
        // Enumerer les flux
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
        // Activer les flux selectionnes
        if (videoStreamIndex >= 0 && videoStreamIndex < videoStreams.Count)
        {
            streamSelect.Enable(videoStreams[videoStreamIndex],
                               AMStreamSelectEnableFlags.Enable);
        }
        if (audioStreamIndex >= 0 && audioStreamIndex < audioStreams.Count)
        {
            streamSelect.Enable(audioStreams[audioStreamIndex],
                               AMStreamSelectEnableFlags.Enable);
        }
    }
    // Construire le graphe
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

## Exemple 7 : rappel de données conteneur (par ex. métadonnées SMPTE KLV)

Recevoir des tampons de données hors-bande bruts transportés dans le conteneur (tels que des paquets de métadonnées SMPTE KLV dans les flux MPEG-TS). Le rappel est déclenché une fois par paquet de données et expose les octets du paquet ainsi que ses horodatages de présentation/fin.

### Implémentation du rappel de données en C#

```csharp
// Signature reelle (selon IFFmpegSourceSettings.h) :
//   HRESULT (BYTE* buffer, int bufferLen, int dataType, LONGLONG startTime, LONGLONG stopTime)
// dataType est une enumeration VF_DATA_TYPE :
//   0 = inconnu, 1 = SMPTE_KLV
public delegate int FFMPEGDataCallbackDelegate(
    IntPtr buffer,
    int bufferLen,
    int dataType,
    long startTime,
    long stopTime);

public class FFMPEGDataCallbackExample
{
    private IFilterGraph2 filterGraph;
    private IBaseFilter sourceFilter;
    private FFMPEGDataCallbackDelegate dataCallback;

    public void PlayWithCallback(string filename, Action<byte[], int, long, long> onPacket)
    {
        // Conserver une reference manageed au delegate pour eviter sa collecte par le GC
        // pendant que le code natif detient un pointeur de fonction vers lui.
        this.dataCallback = (buffer, bufferLen, dataType, startTime, stopTime) =>
        {
            byte[] managed = new byte[bufferLen];
            Marshal.Copy(buffer, managed, 0, bufferLen);
            onPacket(managed, dataType, startTime, stopTime);
            return 0; // S_OK
        };

        filterGraph = (IFilterGraph2)new FilterGraph();

        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");

        var settings = sourceFilter as IFFMPEGSourceSettings;
        if (settings != null)
        {
            // Brancher le rappel de donnees
            settings.SetDataCallback(this.dataCallback);
        }

        // Charger le fichier
        var fileSource = sourceFilter as IFileSourceFilter;
        fileSource.Load(filename, null);

        // Construire le graphe (sans renderers si vous ne voulez que les rappels)
        ICaptureGraphBuilder2 captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        captureGraph.SetFiltergraph(filterGraph);

        // Rendre normalement ; le rappel de donnees se declenche en parallele de la lecture.
        captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, null);
        captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);

        var mediaControl = (IMediaControl)filterGraph;
        mediaControl.Run();

        Marshal.ReleaseComObject(captureGraph);
    }
}

// Utilisation :
//   example.PlayWithCallback("input.ts", (bytes, dataType, startTime, stopTime) =>
//   {
//       const int VF_DATA_TYPE_SMPTE_KLV = 1;
//       if (dataType == VF_DATA_TYPE_SMPTE_KLV)
//       {
//           // Decoder le paquet de metadonnees KLV
//           Console.WriteLine($"KLV packet: {bytes.Length} bytes, {startTime}–{stopTime}");
//       }
//   });
```

---
## Exemple 8 : rappel d'horodatage
Surveiller le cadencement du démultiplexeur/flux par type de média.
### Rappel d'horodatage en C#
```csharp
// Signature reelle (selon IFFmpegSourceSettings.h) :
//   HRESULT (int mediaType, __int64 demuxerStartTime,
//            __int64 streamStartTime, __int64 timestamp)
// mediaType selectionne entre flux audio et video.
public delegate int FFMPEGTimestampCallbackDelegate(
    int mediaType,
    long demuxerStartTime,
    long streamStartTime,
    long timestamp);

private FFMPEGTimestampCallbackDelegate timestampCallback;

public void PlayWithTimestampCallback(string filename)
{
    // Conserver le delegate pour qu'il ne soit pas collecte par le GC pendant que le code natif l'appelle.
    this.timestampCallback = (mediaType, demuxerStart, streamStart, timestamp) =>
    {
        Console.WriteLine(
            $"mediaType={mediaType} demuxerStart={demuxerStart} " +
            $"streamStart={streamStart} timestamp={timestamp}");
        return 0; // S_OK
    };

    filterGraph = (IFilterGraph2)new FilterGraph();
    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");
    var settings = sourceFilter as IFFMPEGSourceSettings;
    if (settings != null)
    {
        settings.SetTimestampCallback(this.timestampCallback);
    }
    // Charger et lire le fichier...
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

## Exemple 9 : activation de licence

Activer une licence achetée.

### Activation de licence en C#

```csharp
public void PlayWithLicense(string filename, string licenseKey, IntPtr videoWindowHandle)
{
    filterGraph = (IFilterGraph2)new FilterGraph();

    sourceFilter = FilterGraphTools.AddFilterFromClsid(
        filterGraph,
        Consts.CLSID_VFFFMPEGSource,
        "FFMPEG Source");

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

    // Configurer et lire...
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
## Exemple 10 : lecteur multimédia complet
Lecteur multimédia complet avec toutes les fonctionnalités.
### Exemple complet en C#
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
        // Creer le graphe de filtres
        filterGraph = (IFilterGraph2)new FilterGraph();
        captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
        mediaControl = (IMediaControl)filterGraph;
        mediaSeeking = (IMediaSeeking)filterGraph;
        videoWindow = (IVideoWindow)filterGraph;
        mediaEventEx = (IMediaEventEx)filterGraph;
        // Configurer les notifications d'evenements
        int hr = mediaEventEx.SetNotifyWindow(notifyHandle, WM_GRAPHNOTIFY, IntPtr.Zero);
        DsError.ThrowExceptionForHR(hr);
        // Attacher le graphe de capture
        hr = captureGraph.SetFiltergraph(filterGraph);
        DsError.ThrowExceptionForHR(hr);
    }
    public void LoadFile(string filename, bool enableGPU, bool enableBuffering, string licenseKey = null)
    {
        // Creer le filtre source
        sourceFilter = FilterGraphTools.AddFilterFromClsid(
            filterGraph,
            Consts.CLSID_VFFFMPEGSource,
            "FFMPEG Source");
        // Enregistrer la licence si fournie
        if (!string.IsNullOrEmpty(licenseKey))
        {
            var registration = sourceFilter as IVFRegister;
            registration?.SetLicenseKey(licenseKey);
        }
        // Configurer le filtre
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
        // Charger le fichier
        var fileSource = sourceFilter as IFileSourceFilter;
        int hr = fileSource.Load(filename, null);
        DsError.ThrowExceptionForHR(hr);
        // Rendre les flux
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
            long seekPos = timeInSeconds * 10000000; // Convertir en unites de 100 nanosecondes
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
            return position / 10000000; // Convertir en secondes
        }
        return 0;
    }
    public long GetDuration()
    {
        if (mediaSeeking != null)
        {
            mediaSeeking.GetDuration(out long duration);
            return duration / 10000000; // Convertir en secondes
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

## Dépannage

### Problème : erreur « Class not registered »

**Solution** : vérifiez que le filtre est enregistré :

```bash
regsvr32 VisioForge_FFMPEG_Source_x64.ax
```

### Problème : l'accélération matérielle ne fonctionne pas

**Solution** : vérifiez la prise en charge GPU et les pilotes :

```csharp
var settings = sourceFilter as IFFMPEGSourceSettings;
bool isEnabled;
settings.GetHWAccelerationEnabled(out isEnabled);
Console.WriteLine($"HW Acceleration: {isEnabled}");
```

### Problème : la connexion au flux réseau échoue

**Solution** : augmentez le délai et utilisez des options personnalisées :

```csharp
settings.SetLoadTimeOut(60);
settings.SetCustomOption("timeout", "30000000");  // 30 secondes
settings.SetCustomOption("rtsp_transport", "tcp");
```

---
## Voir aussi
### Documentation
- [Référence des interfaces](interface-reference.md) — référence API complète
- [Guide de déploiement](../deployment/index.md) — déploiement du filtre
### Exemples de code
- **[Référentiel d'exemples GitHub](https://github.com/visioforge/directshow-samples)** — exemples fonctionnels complets
- **[Projet d'exemple C#](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/cs)** — implémentation C# complète
- **[Projet d'exemple VB.NET](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/dotnet/vbnet)** — implémentation VB.NET
- **[Projet d'exemple C++](https://github.com/visioforge/directshow-samples/tree/master/FFMPEG%20Source%20Filter/cpp_builder)** — implémentation C++Builder
### Ressources externes
- [Documentation FFmpeg](https://ffmpeg.org/documentation.html)
- [Guide de programmation DirectShow](https://learn.microsoft.com/en-us/windows/win32/DirectShow/directshow)
### Support
- [Support technique](https://support.visioforge.com) — obtenez de l'aide de l'équipe VisioForge
- [Communauté Discord](https://discord.com/invite/yvXUG56WCH) — rejoignez notre communauté
