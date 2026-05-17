---
title: Exemples DirectShow de caméra virtuelle — C++, C#, VB.NET
description: Diffusez et capturez via caméras virtuelles DirectShow. Exemples de rendu d'images, effets temps réel et instances multiples.
tags:
  - Virtual Camera SDK
  - DirectShow
  - C++
  - Windows
  - WinForms
  - Streaming
  - Virtual Camera
  - Webcam
  - C#
primary_api_classes:
  - IBaseFilter
  - IVFLiveVideoSource
  - IVFVirtualCameraSink
  - IVFVirtualCameraSource

---

# Virtual Camera SDK — exemples de code

## Vue d'ensemble

Cette page fournit des exemples de code pratiques pour utiliser le Virtual Camera SDK. Le SDK vous permet de :

- **Écrire VERS une caméra virtuelle** : diffuser de la vidéo depuis des fichiers, des caméras réelles ou des images individuelles vers une caméra virtuelle
- **Lire DEPUIS une caméra virtuelle** : capturer de la vidéo depuis une caméra virtuelle (apparaît comme une webcam classique pour les applications)
- Appliquer des effets vidéo et un traitement en temps réel
- Prendre en charge plusieurs instances de caméra virtuelle

La caméra virtuelle apparaît comme une webcam standard pour les applications comme Zoom, Teams, OBS et autres logiciels de visioconférence.

---
## Présentation de l'architecture
Le Virtual Camera SDK fournit trois types principaux de filtres :
1. **CLSID_VFVirtualCameraSource** : lit DEPUIS la caméra virtuelle (agit comme source de capture vidéo)
2. **CLSID_VFVirtualCameraSink** : écrit VERS la caméra virtuelle (agit comme moteur de rendu)
3. **CLSID_VFVideoPushSource** : source push pour le rendu image par image (séquences d'images, rendu personnalisé)
**Flux typiques** :
- Fichier/Caméra → VirtualCameraSink → caméra virtuelle → autres applications
- Caméra virtuelle → VirtualCameraSource → votre application
- PushSource (images) → VirtualCameraSink → caméra virtuelle → autres applications
---

## Prérequis

### Projets C#

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;
```

**Paquets NuGet requis** :

- `VisioForge.DirectShowAPI` — bibliothèque wrapper DirectShow

**CLSID clés** :

```csharp
// CLSID de filtres (disponibles dans la classe Consts)
public static readonly Guid CLSID_VFVirtualCameraSource = new Guid("AA4DA14E-644B-487a-A7CB-517A390B4BB8"); // Lire depuis la camera virtuelle
public static readonly Guid CLSID_VFVirtualCameraSink = new Guid("AA6AB4DF-9670-4913-88BB-2CB381C19340"); // Ecrire vers la camera virtuelle
public static readonly Guid CLSID_VFVirtualAudioCardSource = new Guid("B5A463DF-4016-4C34-AA4F-48EC1B51C73F"); // Source audio
public static readonly Guid CLSID_VFVirtualAudioCardSink = new Guid("1A2673B0-553E-4027-AECC-839405468950"); // Puits audio

// Source push pour le rendu image par image
public static readonly Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
```

### Projets C++

```cpp
#include <dshow.h>
#include <streams.h>
#include "ivirtualcamera.h"

#pragma comment(lib, "strmiids.lib")

// CLSID de filtres
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
## Exemple 1 : diffuser un fichier vidéo vers la caméra virtuelle
Cet exemple démontre la diffusion d'un fichier vidéo vers une caméra virtuelle.
### Implémentation C#
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
            // Creer le graphe de filtres
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            mediaEventExSource = (IMediaEventEx)filterGraphSource;
            // Attacher le graphe de filtres au graphe de capture
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Ajouter le Virtual Camera Sink pour la video
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            // Optionnel : definir la cle de licence pour la version achetee
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("YOUR-LICENSE-KEY"); // Utiliser "TRIAL" pour la version d'essai
            // Ajouter le Virtual Camera Sink pour l'audio
            sinkAudioFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualAudioCardSink,
                "VisioForge Virtual Camera Sink - Audio");
            // Ajouter le filtre source pour le fichier video
            // DirectShow selectionne automatiquement le filtre source approprie
            filterGraphSource.AddSourceFilter(videoFile, "Source file", out sourceVideoFilter);
            // Rendre le flux video : Source -> Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Rendre le flux audio : Source -> Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkAudioFilter);
            // Remarque : les erreurs audio ne sont pas critiques, il est preferable de verifier si l'audio est disponible
            // Demarrer la lecture
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
        // Arreter la lecture
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Arreter la reception d'evenements
        mediaEventExSource?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
        // Retirer tous les filtres
        FilterGraphTools.RemoveAllFilters(filterGraphSource);
        // Liberer les interfaces DirectShow
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
### Implémentation C++
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
    // Initialiser COM
    CoInitialize(NULL);
    // Creer le gestionnaire de graphe de filtres
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    // Creer le Capture Graph Builder
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;
    // Definir le graphe de filtres
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;
    // Creer le filtre Virtual Camera Sink pour la video
    hr = CoCreateInstance(CLSID_VFVirtualCameraSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkVideoFilter, L"VisioForge Virtual Camera Sink - Video");
    if (FAILED(hr))
        goto cleanup;
    // Creer le filtre Virtual Camera Sink pour l'audio
    hr = CoCreateInstance(CLSID_VFVirtualAudioCardSink, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pSinkAudioFilter);
    if (FAILED(hr))
        goto cleanup;
    hr = pGraph->AddFilter(pSinkAudioFilter, L"VisioForge Virtual Camera Sink - Audio");
    if (FAILED(hr))
        goto cleanup;
    // Ajouter le filtre source pour le fichier
    hr = pGraph->AddSourceFilter(videoFile, L"Source File", &pSourceFilter);
    if (FAILED(hr))
        goto cleanup;
    // Rendre le flux video
    hr = pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkVideoFilter);
    if (FAILED(hr))
        goto cleanup;
    // Rendre le flux audio (erreurs non critiques)
    pBuild->RenderStream(NULL, NULL, pSourceFilter, NULL, pSinkAudioFilter);
    // Obtenir l'interface media control
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Demarrer la lecture
        hr = pControl->Run();
    }
cleanup:
    // Liberer les interfaces
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

## Exemple 2 : diffuser une caméra physique vers la caméra virtuelle

Cet exemple démontre la capture depuis une webcam physique et sa diffusion vers une caméra virtuelle.

### Implémentation C#

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            // Attacher le graphe de filtres au graphe de capture
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Ajouter le filtre de la camera physique
            cameraFilter = FilterGraphTools.AddFilterByName(
                filterGraph,
                FilterCategory.VideoInputDevice,
                physicalCameraName);

            if (cameraFilter == null)
            {
                throw new Exception($"Camera '{physicalCameraName}' not found");
            }

            // Ajouter le Virtual Camera Sink
            virtualCameraSink = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSink,
                "Virtual Camera Sink");

            // Optionnel : definir la licence
            var sinkIntf = virtualCameraSink as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");

            // Rendre le flux : Camera physique -> Virtual Camera Sink
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                cameraFilter,
                null,
                virtualCameraSink);
            DsError.ThrowExceptionForHR(hr);

            // Demarrer la capture
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
## Exemple 3 : diffuser une séquence d'images vers la caméra virtuelle (image par image)
Cet exemple démontre le rendu d'images individuelles (séquence d'images ou diaporama) vers une caméra virtuelle.
### Implémentation C#
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
            // Charger les images en memoire
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
            // Creer le graphe de filtres
            filterGraphSource = (IFilterGraph2)new FilterGraph();
            captureGraphSource = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControlSource = (IMediaControl)filterGraphSource;
            int hr = captureGraphSource.SetFiltergraph(filterGraphSource);
            DsError.ThrowExceptionForHR(hr);
            // Ajouter le Virtual Camera Sink
            sinkVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                Consts.CLSID_VFVirtualCameraSink,
                "VisioForge Virtual Camera Sink - Video");
            var sinkIntf = sinkVideoFilter as IVFVirtualCameraSink;
            sinkIntf?.set_license("TRIAL");
            // Ajouter le filtre push source
            Guid CLSID_VFVideoPushSource = new Guid("38D15306-BBC6-4D6C-A89C-9621604D9FC1");
            sourceVideoFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraphSource,
                CLSID_VFVideoPushSource,
                "VisioForge Video Push Source");
            if (sourceVideoFilter == null)
            {
                throw new Exception("Unable to create VisioForge Push Source filter.");
            }
            // Obtenir l'interface IVFLiveVideoSource
            pushSource = sourceVideoFilter as IVFLiveVideoSource;
            if (pushSource == null)
            {
                throw new Exception("Unable to get IVFLiveVideoSource interface.");
            }
            // Configurer le format bitmap
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
            // Connecter les filtres : Push Source -> Virtual Camera Sink
            hr = captureGraphSource.RenderStream(null, null, sourceVideoFilter, null, sinkVideoFilter);
            DsError.ThrowExceptionForHR(hr);
            // Demarrer le graphe
            hr = mediaControlSource.Run();
            DsError.ThrowExceptionForHR(hr);
            // Configurer un timer pour pousser les images
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
        // Obtenir l'image courante
        Bitmap frame = frames[currentFrameIndex];
        // Verrouiller les donnees bitmap
        BitmapData bmpData = frame.LockBits(
            new Rectangle(0, 0, frame.Width, frame.Height),
            ImageLockMode.ReadOnly,
            PixelFormat.Format24bppRgb);
        try
        {
            // AddFrame prend un AVFrameData (LPStruct), PAS un IntPtr brut.
            // Construisez-le a partir du bitmap verrouille avant de pousser.
            int frameSize = bmpData.Stride * frame.Height;
            long durationTicks = (long)(10_000_000.0 / frameRate); // unites de 100 ns
            var avFrame = new AVFrameData
            {
                Data = bmpData.Scan0,
                Size = frameSize,
                StartTime = currentFrameIndex * durationTicks,
                StopTime = (currentFrameIndex + 1) * durationTicks
            };
            pushSource.AddFrame(avFrame);
        }
        finally
        {
            frame.UnlockBits(bmpData);
        }
        // Passer a l'image suivante (en boucle)
        currentFrameIndex = (currentFrameIndex + 1) % frames.Length;
    }
    private int GetStrideRGB24(int width)
    {
        return ((width * 24 + 31) / 32) * 4;
    }
    public void Cleanup()
    {
        // Arreter le timer
        if (framePushTimer != null)
        {
            framePushTimer.Stop();
            framePushTimer.Dispose();
            framePushTimer = null;
        }
        // Arreter la lecture
        if (mediaControlSource != null)
        {
            mediaControlSource.Stop();
        }
        // Liberer les images
        if (frames != null)
        {
            foreach (var frame in frames)
            {
                frame?.Dispose();
            }
            frames = null;
        }
        // Liberer les interfaces DirectShow
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

## Exemple 4 : lire depuis la caméra virtuelle

Cet exemple démontre la capture de vidéo depuis une caméra virtuelle (utile pour les tests ou la surveillance de ce qui est envoyé vers la caméra virtuelle).

### Implémentation C#

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
            // Creer le graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;

            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Ajouter le filtre Virtual Camera Source
            virtualCameraSource = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVirtualCameraSource,
                "VisioForge Virtual Camera");

            if (virtualCameraSource == null)
            {
                throw new Exception("Unable to create Virtual Camera Source filter");
            }

            // REMARQUE : IVFVirtualCameraSource n'expose que SetCustomVideoSize() et
            // FixResolution(). La gestion des licences se trouve cote SINK (IVFVirtualCameraSink::set_license)
            // car c'est le filtre qui publie du contenu vers la camera virtuelle ; le filtre source
            // montre ici ne fait que le consommer.

            // Creer Enhanced Video Renderer (EVR)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurer EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Definir la fenetre video
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

            // Rendre le flux : Virtual Camera Source -> EVR
            hr = captureGraph.RenderStream(
                null,
                MediaType.Video,
                virtualCameraSource,
                null,
                videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            // Demarrer la lecture
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
## Ressources supplémentaires
Pour des informations plus détaillées, consultez :
- [Page produit du Virtual Camera SDK](https://www.visioforge.com/virtual-camera-sdk)
- [Contrat de licence utilisateur final](../../eula.md)
- [Référentiel d'exemples de code](https://github.com/visioforge/directshow-samples/tree/main/Virtual%20Camera%20SDK)
## Support
- **Support technique** : https://support.visioforge.com/
- **Communauté Discord** : https://discord.com/invite/yvXUG56WCH
