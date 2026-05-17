---
title: Filtre source DirectShow VLC — plus de 200 formats et flux
description: Filtre source DirectShow basé sur libVLC pour lire plus de 200 formats, vidéo 4K/8K et flux réseau RTSP/HLS/UDP avec décodage matériel. C++, C#, Delphi.
sidebar_label: Filtre source DirectShow VLC
order: 9
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Editing
primary_api_classes:
  - IFileSourceFilter
  - IBaseFilter
  - IVFRegister

---

# Filtre source DirectShow VLC

## Vue d'ensemble

Le filtre source DirectShow VLC permet aux développeurs d'intégrer en toute transparence des capacités avancées de lecture multimédia dans n'importe quelle application basée sur DirectShow. Ce composant puissant assure une lecture fluide de divers fichiers vidéo et flux réseau sur de multiples formats et protocoles.

Notre paquet SDK fournit une solution complète avec toutes les DLL du lecteur VLC nécessaires, accompagnées d'un filtre DirectShow flexible. Le paquet propose à la fois des interfaces standard de sélection de fichier et de nombreuses options de configuration personnalisée du filtre pour répondre à vos exigences de développement.

Pour les détails complets du produit et les options de licence, consultez la [page produit](https://www.visioforge.com/vlc-source-directshow-filter).

---

## Installation

Avant d'utiliser les exemples de code et d'intégrer le filtre dans votre application, vous devez d'abord installer le filtre source DirectShow VLC depuis la [page produit](https://www.visioforge.com/vlc-source-directshow-filter).

**Étapes d'installation** :

1. Téléchargez l'installeur du SDK depuis la page produit
2. Exécutez l'installeur avec des privilèges administrateur
3. L'installeur enregistrera le filtre source VLC et déploiera toutes les DLL VLC nécessaires
4. Les applications d'exemple et le code source seront disponibles dans le répertoire d'installation

**Remarque** : le filtre doit être correctement enregistré sur le système avant de pouvoir être utilisé dans vos applications. L'installeur s'en charge automatiquement.

---

## Spécifications techniques

### Interfaces DirectShow prises en charge

Le filtre implémente ces interfaces DirectShow standard pour une compatibilité maximale :

- **IAMStreamSelect** — capacités complètes de sélection de flux vidéo et audio
- **IAMStreamConfig** — paramètres avancés de configuration vidéo et audio
- **IFileSourceFilter** — spécification flexible des sources par nom de fichier ou URL
- **IMediaSeeking** — prise en charge robuste du positionnement sur la timeline

### Fonctionnalités clés

- Décodage accéléré matériellement pour des performances optimales
- Prise en charge de la lecture vidéo 4K et 8K
- Vaste compatibilité de formats, y compris les codecs modernes
- Gestion des flux réseau (RTSP, HLS, DASH, etc.)
- Rendu et gestion des sous-titres
- Prise en charge des pistes audio multilingues
- Capacités de lecture vidéo 360°
- Prise en charge du contenu HDR

## Exemples d'implémentation

### Exemple d'intégration C++

```cpp
#include <dshow.h>
#include <mfapi.h>
#include <evr.h>
#include "ivlcsrc.h"

// CLSID du filtre source VLC
DEFINE_GUID(CLSID_VlcSource,
    0x3fc97748, 0x7cb6, 0x4195, 0x89, 0xde, 0x07, 0x17, 0x58, 0x2a, 0x48, 0x63);

HRESULT InitializeVLCSource(HWND hVideoWindow)
{
    HRESULT hr = S_OK;
    IFilterGraph2* pGraph = NULL;
    ICaptureGraphBuilder2* pBuild = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pVLCSource = NULL;
    IBaseFilter* pVideoRenderer = NULL;
    IFileSourceFilter* pFileSource = NULL;

    // Initialiser COM
    CoInitialize(NULL);

    // Creer le gestionnaire de graphe de filtres
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER,
                         IID_IFilterGraph2, (void**)&pGraph);
    if (FAILED(hr))
        return hr;

    // Creer le Capture Graph Builder
    hr = CoCreateInstance(CLSID_CaptureGraphBuilder2, NULL, CLSCTX_INPROC_SERVER,
                         IID_ICaptureGraphBuilder2, (void**)&pBuild);
    if (FAILED(hr))
        goto cleanup;

    // Definir le graphe de filtres pour le capture graph builder
    hr = pBuild->SetFiltergraph(pGraph);
    if (FAILED(hr))
        goto cleanup;

    // Creer le filtre source VLC
    hr = CoCreateInstance(CLSID_VlcSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVLCSource);
    if (FAILED(hr))
        goto cleanup;

    // Ajouter le filtre au graphe
    hr = pGraph->AddFilter(pVLCSource, L"VLC Source");
    if (FAILED(hr))
        goto cleanup;

    // Charger le fichier media via l'interface IFileSourceFilter
    hr = pVLCSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (SUCCEEDED(hr) && pFileSource)
    {
        hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
        pFileSource->Release();
        if (FAILED(hr))
            goto cleanup;
    }

    // Creer le moteur Enhanced Video Renderer (EVR)
    CLSID CLSID_EVR = { 0xFA10746C, 0x9B63, 0x4B6C,
        { 0xBC, 0x49, 0xFC, 0x30, 0x0E, 0xA5, 0xF2, 0x56 } };
    hr = CoCreateInstance(CLSID_EVR, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pGraph->AddFilter(pVideoRenderer, L"EVR");
    if (FAILED(hr))
        goto cleanup;

    // Configurer EVR
    IEVRFilterConfig* pConfig = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IEVRFilterConfig, (void**)&pConfig);
    if (SUCCEEDED(hr) && pConfig)
    {
        pConfig->SetNumberOfStreams(1);
        pConfig->Release();
    }

    // Configurer la fenetre video
    IMFGetService* pGetService = NULL;
    hr = pVideoRenderer->QueryInterface(IID_IMFGetService, (void**)&pGetService);
    if (SUCCEEDED(hr) && pGetService)
    {
        IMFVideoDisplayControl* pDisplayControl = NULL;
        hr = pGetService->GetService(MR_VIDEO_RENDER_SERVICE,
                                     IID_IMFVideoDisplayControl,
                                     (void**)&pDisplayControl);
        if (SUCCEEDED(hr) && pDisplayControl)
        {
            pDisplayControl->SetVideoWindow(hVideoWindow);
            pDisplayControl->Release();
        }
        pGetService->Release();
    }

    // Rendre les flux
    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Video, pVLCSource, NULL, pVideoRenderer);
    if (FAILED(hr))
        goto cleanup;

    hr = pBuild->RenderStream(NULL, &MEDIATYPE_Audio, pVLCSource, NULL, NULL);
    // Les erreurs audio ne sont pas critiques

    // Obtenir l'interface media control pour le controle de la lecture
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Demarrer la lecture
        hr = pControl->Run();
    }

cleanup:
    // Liberer les interfaces
    if (pControl) pControl->Release();
    if (pVideoRenderer) pVideoRenderer->Release();
    if (pVLCSource) pVLCSource->Release();
    if (pBuild) pBuild->Release();
    if (pGraph) pGraph->Release();

    return hr;
}
```

### Intégration C# (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using MediaFoundation;
using MediaFoundation.EVR;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Initialiser le filtre source VLC en C# avec DirectShowLib
public class VLCSourcePlayer
{
    private IFilterGraph2 filterGraph;
    private ICaptureGraphBuilder2 captureGraph;
    private IMediaControl mediaControl;
    private IMediaSeeking mediaSeeking;
    private IMediaEventEx mediaEventEx;
    private IBaseFilter sourceFilter;
    private IBaseFilter videoRenderer;

    public void Initialize(string filename, IntPtr videoWindowHandle)
    {
        try
        {
            // Creer le gestionnaire de graphe de filtres
            filterGraph = (IFilterGraph2)new FilterGraph();
            captureGraph = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            mediaControl = (IMediaControl)filterGraph;
            mediaSeeking = (IMediaSeeking)filterGraph;
            mediaEventEx = (IMediaEventEx)filterGraph;

            // Attacher le graphe de filtres au graphe de capture
            int hr = captureGraph.SetFiltergraph(filterGraph);
            DsError.ThrowExceptionForHR(hr);

            // Creer le filtre source VLC avec le CLSID correct
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFVLCSource,
                "VLC Source");

            // Optionnel : enregistrer la version achetee
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("your-license-key-here");

            // Charger le fichier media ou l'URL via l'interface IFileSourceFilter
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Creer le moteur de rendu video (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurer EVR
            var evrConfig = videoRenderer as IEVRFilterConfig;
            evrConfig?.SetNumberOfStreams(1);

            // Definir la fenetre video pour le rendu
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

            // Rendre les flux
            hr = captureGraph.RenderStream(null, MediaType.Video, sourceFilter, null, videoRenderer);
            DsError.ThrowExceptionForHR(hr);

            hr = captureGraph.RenderStream(null, MediaType.Audio, sourceFilter, null, null);
            // Remarque : les erreurs de rendu audio ne sont pas critiques pour une lecture video seule

            // Demarrer la lecture
            hr = mediaControl.Run();
            DsError.ThrowExceptionForHR(hr);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error initializing VLC Source: {ex.Message}");
            Cleanup();
            throw;
        }
    }

    public void Cleanup()
    {
        // Arreter la lecture
        if (mediaControl != null)
        {
            mediaControl.StopWhenReady();
            mediaControl.Stop();
        }

        // Arreter la reception des evenements
        mediaEventEx?.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);

        // Retirer tous les filtres
        FilterGraphTools.RemoveAllFilters(filterGraph);

        // Liberer les interfaces DirectShow
        if (mediaControl != null)
        {
            Marshal.ReleaseComObject(mediaControl);
            mediaControl = null;
        }

        if (mediaSeeking != null)
        {
            Marshal.ReleaseComObject(mediaSeeking);
            mediaSeeking = null;
        }

        if (mediaEventEx != null)
        {
            Marshal.ReleaseComObject(mediaEventEx);
            mediaEventEx = null;
        }

        if (sourceFilter != null)
        {
            Marshal.ReleaseComObject(sourceFilter);
            sourceFilter = null;
        }

        if (videoRenderer != null)
        {
            Marshal.ReleaseComObject(videoRenderer);
            videoRenderer = null;
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

**CLSID et GUID clés :**

```csharp
// CLSID du filtre source VLC
public static readonly Guid CLSID_VFVLCSource = new Guid("3FC97748-7CB6-4195-89DE-0717582A4863");

// IID de l'interface IVlcSrc
[Guid("77493EB7-6D00-41C5-9535-7C593824E892")]
public interface IVlcSrc { /* ... */ }

// IID de l'interface IVlcSrc2 (pour les parametres de ligne de commande personnalises)
[Guid("CCE122C0-172C-4626-B4B6-42B039E541CB")]
public interface IVlcSrc2 : IVlcSrc { /* ... */ }

// IID de l'interface IVlcSrc3 (pour la surcharge de frequence d'images)
[Guid("3DFBED0C-E4A8-401C-93EF-CBBFB65223DD")]
public interface IVlcSrc3 : IVlcSrc2 { /* ... */ }

// IID de l'interface IVFRegister (pour la gestion des licences)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Paquets NuGet requis :**

- `VisioForge.DirectShowAPI` — bibliothèque wrapper DirectShow
- `MediaFoundation.Net` — wrapper Media Foundation pour le moteur de rendu EVR

**Exemple de sélection de piste audio :**

```csharp
// Enumerer et selectionner des pistes audio
var vlcSrc = sourceFilter as IVlcSrc;
if (vlcSrc != null)
{
    int audioCount = 0;
    vlcSrc.GetAudioTracksCount(out audioCount);

    for (int i = 0; i < audioCount; i++)
    {
        int trackId;
        var trackName = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(i, out trackId, trackName);

        Console.WriteLine($"Track {i}: {trackName} (ID: {trackId})");
    }

    // Selectionner une piste audio specifique
    if (audioCount > 1)
    {
        int desiredTrackId;
        var name = new StringBuilder(256);
        vlcSrc.GetAudioTrackInfo(1, out desiredTrackId, name);
        vlcSrc.SetAudioTrack(desiredTrackId);
    }
}
```

**Exemple d'options VLC personnalisées :**

`IVlcSrc2::SetCustomCommandLine` prend un tableau de chaînes natives UTF-8 (`char* params[]` dans `ivlcsrc.h`), et non un `string[]` managé. Le wrapper C# marshale les paramètres comme `IntPtr[]` de tampons UTF-8 non managés — allouez avec `StringHelper.NativeUtf8FromString()` et libérez avec `Marshal.FreeHGlobal()` après l'appel. Passer un `string[]` brut produirait un marshalling en ANSI et corromprait les options non-ASCII.

```csharp
// Configurer VLC pour un streaming RTSP faible latence
var vlcSrc2 = sourceFilter as IVlcSrc2;
if (vlcSrc2 != null)
{
    var parameters = new[]
    {
        "--network-caching=300",     // Tampon reseau faible
        "--rtsp-tcp",                // Forcer TCP pour RTSP
        "--avcodec-hw=any",          // Activer le decodage materiel
        "--live-caching=300"         // Tampon faible pour flux en direct
    };

    // Convertir les chaines managees en tableau IntPtr UTF-8 natif
    var array = new IntPtr[parameters.Length];
    for (int i = 0; i < parameters.Length; i++)
    {
        array[i] = StringHelper.NativeUtf8FromString(parameters[i]);
    }

    try
    {
        int hr = vlcSrc2.SetCustomCommandLine(array, parameters.Length);
        DsError.ThrowExceptionForHR(hr);
    }
    finally
    {
        // Liberer les tampons UTF-8 non manages
        for (int i = 0; i < array.Length; i++)
        {
            Marshal.FreeHGlobal(array[i]);
        }
    }

    // Puis charger le flux
    var vlcSrc = vlcSrc2 as IVlcSrc;
    vlcSrc?.SetFile("rtsp://192.168.1.100/stream");
}
```

Consultez [examples.md](examples.md) pour le motif d'aide canonique réutilisé dans plusieurs scénarios (Exemple 4 — clé binaire avec options personnalisées, variantes basse latence, etc.).

## Historique des versions

### Version 2026.3.4

- Mise à jour vers le cœur VLC v3.0.23
- Mise à niveau de la chaîne de build vers MSVC v143 (Visual Studio 2022)
- Correction du plantage lors du démontage du graphe pendant `libvlc_media_player_stop()`
- Remplacement des dépendances Boost par des équivalents de la bibliothèque standard C++

### Version 15.0

- Qualité de lecture améliorée pour de nombreux formats
- Moteur de rendu de sous-titres amélioré
- Mise à jour des implémentations de codecs dont dav1d, ffmpeg et libvpx
- Ajout de la mise à l'échelle Super Resolution avec accélération GPU nVidia et Intel

### Version 14.0

- Mise à jour vers le cœur VLC v3.0.18
- Correction des problèmes de compatibilité DxVA/D3D11 avec le contenu HEVC
- Résolution des problèmes de redimensionnement OpenGL pour une lecture plus fluide

### Version 12.0

- Mise à niveau vers le moteur VLC v3.0.16
- Ajout de la prise en charge de nouveaux formats Fourcc (E-AC3 et AV1)
- Correction des problèmes de stabilité avec les flux VP9

### Version 11.1

- Intégration de VLC v3.0.11
- Optimisation du mécanisme de mise à jour des playlists HLS
- Gestion et affichage améliorés des sous-titres WebVTT

### Version 11.0

- Construit sur la base VLC v3.0.10
- Correction de problèmes de régression critiques avec les flux HLS

### Version 10.4

- Mise à jour majeure vers l'architecture VLC 3.0
- Décodage matériel activé par défaut pour le contenu 4K et 8K
- Ajout de la profondeur de couleur 10 bits et du HDR
- Implémentation des capacités vidéo 360 degrés et audio 3D
- Introduction de la prise en charge des menus Blu-Ray Java

### Version 10.0

- Première version en tant que filtre DirectShow autonome
- Pour l'historique antérieur, consultez le journal des modifications du Video Capture SDK .Net

## Ressources supplémentaires

- [Contrat de licence utilisateur final](../../eula.md)
- [Exemples de code](https://github.com/visioforge/)
