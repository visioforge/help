---
title: FFMPEG Source DirectShow Filter — décode 100+ formats
description: Filtre source DirectShow basé sur FFmpeg pour décoder MP4, MKV, H.265 et plus de 100 formats avec accélération matérielle. Interface COM pour C++, C# et Delphi.
sidebar_label: FFMPEG Source DirectShow Filter
order: 10
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
primary_api_classes:
  - IBaseFilter
  - IFileSourceFilter
  - FFMPEGFilter
  - FileSource
  - IFFMPEGSourceSettings

---

# FFMPEG Source DirectShow Filter

## Introduction

Le FFMPEG Source DirectShow Filter permet aux développeurs d'intégrer en toute transparence des capacités avancées de décodage et de lecture multimédia dans n'importe quelle application compatible DirectShow. Ce composant puissant comble le fossé entre les formats multimédias complexes et vos besoins de développement logiciel, fournissant une base robuste pour créer des applications riches en médias.

---

## Installation

Avant d'utiliser les exemples de code et d'intégrer le filtre dans votre application, vous devez d'abord installer le FFMPEG Source DirectShow Filter depuis la [page produit](https://www.visioforge.com/ffmpeg-source-directshow-filter).

**Étapes d'installation** :

1. Téléchargez l'installeur du SDK depuis la page produit
2. Exécutez l'installeur avec des privilèges administrateur
3. L'installeur enregistrera le FFMPEG Source Filter et déploiera toutes les DLL FFMPEG nécessaires
4. Les applications d'exemple et le code source seront disponibles dans le répertoire d'installation

**Remarque** : le filtre doit être correctement enregistré sur le système avant de pouvoir être utilisé dans vos applications. L'installeur s'en charge automatiquement.

---

## Fonctionnalités et capacités clés

Notre filtre est livré avec toutes les DLL FFMPEG nécessaires et fournit une interface de filtre DirectShow riche en fonctionnalités prenant en charge :

- **Vaste compatibilité de formats** : prise en charge d'un large éventail de formats vidéo et audio, dont MP4, MKV, AVI, MOV, WMV, FLV et bien d'autres, sans installation supplémentaire de codecs
- **Prise en charge des flux réseau** : connexion aux flux RTSP, RTMP, HTTP, UDP et TCP pour l'intégration de médias en direct
- **Gestion de plusieurs flux** : sélection entre les flux vidéo et audio dans les fichiers multimédias multi-flux
- **Capacités de positionnement avancées** : implémentez des fonctionnalités de positionnement précises dans vos applications
- **Accélération GPU** : utilisez l'accélération matérielle pour des performances optimales

## Exemples d'implémentation

Le SDK inclut des applications d'exemple complètes pour plusieurs environnements de développement :

### Intégration Delphi (principale)

```delphi
// Initialiser le FFMPEG Source Filter en Delphi avec DSPack
procedure TMainForm.InitializeFFMPEGSource;
var
  FFMPEGFilter: IBaseFilter;
  FileSource: IFileSourceFilter;
begin
  // Creer une instance du FFMPEG Source Filter
  // IMPORTANT : assurez-vous d'une initialisation COM correcte avant cet appel
  CoCreateInstance(CLSID_FFMPEGSource, nil, CLSCTX_INPROC_SERVER, 
                  IID_IBaseFilter, FFMPEGFilter);
  
  // Interroger l'interface du filtre source
  FFMPEGFilter.QueryInterface(IID_IFileSourceFilter, FileSource);
  
  // Charger le fichier media - peut etre local ou une URL reseau
  FileSource.Load('C:\media\sample.mp4', nil);
  
  // Ajouter au graphe de filtres pour le rendu
  FilterGraph.AddFilter(FFMPEGFilter, 'FFMPEG Source');
  
  // Connecter aux moteurs de rendu ou filtres de traitement appropries
  // FilterGraph.RenderStream(...);
end;
```

### Intégration C# (.NET)

```csharp
using System;
using System.Runtime.InteropServices;
using VisioForge.DirectShowAPI;
using VisioForge.DirectShowLib;

// Initialiser le FFMPEG Source Filter en C# avec DirectShowLib
public class FFMPEGSourcePlayer
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

            // Creer le FFMPEG Source Filter avec le CLSID correct
            sourceFilter = FilterGraphTools.AddFilterFromClsid(
                filterGraph,
                Consts.CLSID_VFFFMPEGSource,
                "FFMPEG Source");

            // Optionnel : enregistrer la version achetee
            // var reg = sourceFilter as IVFRegister;
            // reg?.SetLicenseKey("your-license-key-here");

            // Configurer les parametres du filtre
            var filterConfig = sourceFilter as IFFMPEGSourceSettings;
            if (filterConfig != null)
            {
                // Definir le mode de mise en tampon (AUTO, ON ou OFF)
                filterConfig.SetBufferingMode(FFMPEG_SOURCE_BUFFERING_MODE.FFMPEG_SOURCE_BUFFERING_MODE_AUTO);

                // Activer l'acceleration materielle (decodage GPU)
                filterConfig.SetHWAccelerationEnabled(true);

                // Definir le delai d'attente de connexion (millisecondes)
                filterConfig.SetLoadTimeOut(30000);
            }

            // Charger le fichier media ou le flux reseau
            var sourceFilterIntf = sourceFilter as IFileSourceFilter;
            hr = sourceFilterIntf.Load(filename, null);
            DsError.ThrowExceptionForHR(hr);

            // Creer le moteur de rendu video (EVR - Enhanced Video Renderer)
            Guid CLSID_EVR = new Guid("FA10746C-9B63-4B6C-BC49-FC300EA5F256");
            videoRenderer = FilterGraphTools.AddFilterFromClsid(filterGraph, CLSID_EVR, "EVR");

            // Configurer EVR
            var evrConfig = videoRenderer as MediaFoundation.EVR.IEVRFilterConfig;
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

            // Effectuer le rendu des flux
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
            Console.WriteLine($"Error initializing FFMPEG Source: {ex.Message}");
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

        if (filterGraph != null)
        {
            Marshal.ReleaseComObject(filterGraph);
            filterGraph = null;
        }

        if (captureGraph != null)
        {
            Marshal.ReleaseComObject(captureGraph);
            captureGraph = null;
        }
    }
}
```

**CLSID et GUID clés :**

```csharp
// CLSID du FFMPEG Source Filter — meme valeur utilisee dans les exemples C# / C++
// et dans l'enregistrement registre dans ../deployment/filter-registration.md.
public static readonly Guid CLSID_VFFFMPEGSource = new Guid("1974D893-83E4-4F89-9908-795C524CC17E");

// Interface IFFMPEGSourceSettings — pour l'IID canonique, voir l'en-tete de l'interface
// (`IFFmpegSourceSettings.h` / `IFFmpegSourceSettings.cs`) reference depuis
// [interface-reference.md](./interface-reference.md). L'IID de l'interface est distinct
// du CLSID du filtre ci-dessus (un seul GUID ne peut servir les deux roles).
public interface IFFMPEGSourceSettings { /* ... */ }

// IID de l'interface IVFRegister (pour la gestion des licences)
[Guid("59E82754-B531-4A8E-A94D-57C75F01DA30")]
public interface IVFRegister { /* ... */ }
```

**Paquets NuGet requis :**

- `VisioForge.DirectShowAPI` — bibliothèque wrapper DirectShow
- `MediaFoundation.Net` — wrapper Media Foundation pour le moteur de rendu EVR

**Exemple de sélection de flux :**

```csharp
// Selectionner des flux video ou audio specifiques dans des fichiers multi-flux
var streamSelect = sourceFilter as IAMStreamSelect;
if (streamSelect != null)
{
    streamSelect.Count(out int streamCount);

    for (int i = 0; i < streamCount; i++)
    {
        streamSelect.Info(i, out var mediaType, out _, out _, out _, out var name, out _, out _);

        if (mediaType.majorType == MediaType.Video)
        {
            // Activer le premier flux video
            streamSelect.Enable(i, AMStreamSelectEnableFlags.Enable);
            break;
        }
    }
}
```

### Exemple d'intégration C++

```cpp
// Initialiser le FFMPEG Source Filter en C++ avec DirectShow
HRESULT InitializeFFMPEGSource()
{
    HRESULT hr = S_OK;
    IGraphBuilder* pGraph = NULL;
    IMediaControl* pControl = NULL;
    IBaseFilter* pFFMPEGSource = NULL;
    IFileSourceFilter* pFileSource = NULL;
    
    // Initialiser COM
    CoInitialize(NULL);
    
    // Creer le gestionnaire de graphe de filtres
    hr = CoCreateInstance(CLSID_FilterGraph, NULL, CLSCTX_INPROC_SERVER, 
                         IID_IGraphBuilder, (void**)&pGraph);
    if (FAILED(hr))
        return hr;
    
    // Creer le FFMPEG Source Filter
    hr = CoCreateInstance(CLSID_FFMPEGSource, NULL, CLSCTX_INPROC_SERVER,
                         IID_IBaseFilter, (void**)&pFFMPEGSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Ajouter le filtre au graphe
    hr = pGraph->AddFilter(pFFMPEGSource, L"FFMPEG Source");
    if (FAILED(hr))
        goto cleanup;
    
    // Obtenir l'interface IFileSourceFilter
    hr = pFFMPEGSource->QueryInterface(IID_IFileSourceFilter, (void**)&pFileSource);
    if (FAILED(hr))
        goto cleanup;
    
    // Charger le fichier media
    hr = pFileSource->Load(L"C:\\media\\sample.mp4", NULL);
    if (FAILED(hr))
        goto cleanup;
    
    // Effectuer le rendu des pins de sortie du FFMPEG Source Filter
    hr = pGraph->Render(GetPin(pFFMPEGSource, PINDIR_OUTPUT, 0));
    
    // Obtenir l'interface media control pour le controle de la lecture
    hr = pGraph->QueryInterface(IID_IMediaControl, (void**)&pControl);
    if (SUCCEEDED(hr))
    {
        // Demarrer la lecture
        hr = pControl->Run();
        // ... gerer la lecture selon les besoins
    }
    
cleanup:
    // Liberer les interfaces
    if (pControl) pControl->Release();
    if (pFileSource) pFileSource->Release();
    if (pFFMPEGSource) pFFMPEGSource->Release();
    if (pGraph) pGraph->Release();
    
    return hr;
}

// Fonction utilitaire pour obtenir les pins d'un filtre
IPin* GetPin(IBaseFilter* pFilter, PIN_DIRECTION PinDir, int nPin)
{
    IEnumPins* pEnum = NULL;
    IPin* pPin = NULL;
    
    if (pFilter)
    {
        pFilter->EnumPins(&pEnum);
        if (pEnum)
        {
            while (pEnum->Next(1, &pPin, NULL) == S_OK)
            {
                PIN_DIRECTION PinDirThis;
                pPin->QueryDirection(&PinDirThis);
                if (PinDir == PinDirThis)
                {
                    if (nPin == 0)
                        break;
                    nPin--;
                }
                pPin->Release();
                pPin = NULL;
            }
            pEnum->Release();
        }
    }
    
    return pPin;
}
```

## Intégration avec les filtres de traitement

Enrichissez votre pipeline multimédia en connectant le FFMPEG Source Filter avec des composants de traitement supplémentaires :

- Appliquer des effets vidéo et transformations en temps réel
- Traiter les flux audio pour une manipulation sonore personnalisée
- Implémenter des fonctionnalités d'analyse multimédia spécialisées

Notre [pack de filtres de traitement](https://www.visioforge.com/processing-filters-pack) propose des capacités supplémentaires, ou vous pouvez intégrer tout filtre compatible DirectShow standard.

## Spécifications techniques

### Interfaces DirectShow prises en charge

Le filtre implémente ces interfaces DirectShow standard pour une compatibilité maximale :

- **IAMStreamSelect** : sélectionner entre plusieurs flux vidéo et audio
- **IAMStreamConfig** : configurer les paramètres vidéo et audio
- **IFileSourceFilter** : définir le nom de fichier ou l'URL de streaming
- **IMediaSeeking** : implémenter une fonctionnalité de positionnement précise
- **ISpecifyPropertyPages** : accéder à la configuration via les pages de propriétés

## Historique des versions et mises à jour

### Version 15.0

- Bibliothèques FFMPEG améliorées avec les derniers codecs
- Ajout de la prise en charge du décodage GPU pour de meilleures performances
- Gestion mémoire optimisée pour les fichiers volumineux

### Version 12.0

- Mise à jour des bibliothèques FFMPEG
- Compatibilité améliorée avec Windows 10/11

### Version 11.0

- Mise à jour des bibliothèques FFMPEG
- Correction des problèmes de positionnement avec certains formats de fichiers

### Version 10.0

- Mise à jour des bibliothèques FFMPEG
- Ajout de la prise en charge de formats de conteneur supplémentaires

### Version 9.0

- Mise à jour des bibliothèques FFMPEG
- Optimisations de performance

### Version 8.0

- Mise à jour des bibliothèques FFMPEG
- Gestion d'erreurs améliorée

### Version 7.0

- Première version en tant que produit indépendant
- Fonctionnalités de base établies

## Ressources supplémentaires

- Explorez notre [page produit](https://www.visioforge.com/ffmpeg-source-directshow-filter) pour des spécifications détaillées
- Consultez notre [contrat de licence utilisateur final](../../eula.md) pour les détails de licence
- Consultez notre documentation développeur pour des scénarios d'implémentation avancés
