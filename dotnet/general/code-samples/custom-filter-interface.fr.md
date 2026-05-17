---
title: Interfaces de filtre DirectShow personnalisées en .NET
description: Implémentez des interfaces de filtre DirectShow personnalisées en .NET avec accès et manipulation d'IBaseFilter pour applications multimédias.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - Windows
  - Capture
  - C#
  - NuGet
primary_api_classes:
  - IBaseFilter
  - FilterEventArgs
  - VideoCaptureCore

---

# Utiliser des interfaces de filtre DirectShow personnalisées

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

*Remarque : l'API présentée dans ce guide est la même pour tous nos produits SDK, y compris Video Capture SDK .Net, Video Edit SDK .Net et Media Player SDK .Net.*

DirectShow est un framework multimédia puissant qui permet aux développeurs d'effectuer des opérations complexes sur les flux multimédias. L'une de ses forces clés est la capacité à travailler avec des interfaces de filtre personnalisées, vous donnant un contrôle précis sur le traitement des médias. Ce guide vous accompagne dans l'implémentation et l'utilisation d'interfaces de filtre DirectShow personnalisées dans vos applications .NET.

## Comprendre les filtres DirectShow

DirectShow utilise une architecture basée sur les filtres où chaque filtre effectue une opération spécifique sur le flux multimédia. Ces filtres sont connectés dans un graphe, créant un pipeline pour le traitement des médias.

### Composants clés de DirectShow

- **Filtre** : un composant qui traite les données multimédias
- **Pin** : points de connexion entre les filtres
- **Graphe de filtres** : le pipeline complet de filtres connectés
- **IBaseFilter** : l'interface fondamentale que tous les filtres DirectShow implémentent

## Prise en main des interfaces de filtre personnalisées

Pour travailler avec les filtres DirectShow en .NET, vous devrez :

1. Ajouter les références appropriées à votre projet
2. Accéder au filtre via les événements appropriés
3. Convertir le filtre vers l'interface dont vous avez besoin
4. Implémenter votre logique personnalisée

### Références de projet requises

Pour accéder à la fonctionnalité DirectShow, incluez le paquet approprié dans votre projet :

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="X.X.X" />
```

Vous pouvez aussi ajouter la référence à l'assembly `VisioForge.Core` directement dans votre projet.

## Implémenter l'accès à une interface de filtre personnalisé

Notre SDK fournit plusieurs événements qui vous donnent accès aux filtres au fur et à mesure qu'ils sont ajoutés au graphe de filtres. Voici comment les utiliser efficacement :

### Accéder aux filtres dans Video Capture SDK

Le Video Capture SDK propose l'événement `OnFilterAdded` qui est déclenché chaque fois qu'un filtre est ajouté au graphe. Cet événement fournit l'accès à chaque filtre via ses arguments d'événement.

```cs
// S'abonner à l'événement OnFilterAdded
videoCaptureCore.OnFilterAdded += VideoCaptureCore_OnFilterAdded;

// Implémentation du gestionnaire d'événements
private void VideoCaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
{
    // Accéder à l'interface du filtre DirectShow
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    // Vous pouvez maintenant travailler avec le filtre via l'interface IBaseFilter
    if (baseFilter != null)
    {
        // Le code de manipulation du filtre personnalisé va ici
    }
}
```

## Travailler avec l'interface IBaseFilter

L'interface `IBaseFilter` est la base des filtres DirectShow. Voici ce que vous pouvez en faire :

### Récupérer les informations du filtre

```cs
private void GetFilterInfo(IBaseFilter filter)
{
    FilterInfo filterInfo = new FilterInfo();
    int hr = filter.QueryFilterInfo(out filterInfo);
    
    if (hr >= 0)
    {
        Console.WriteLine($"Filter Name: {filterInfo.achName}");
        
        // N'oubliez pas de libérer la référence au graphe de filtres
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

### Énumérer les pins du filtre

```cs
private void EnumerateFilterPins(IBaseFilter filter)
{
    IEnumPins enumPins;
    int hr = filter.EnumPins(out enumPins);
    
    if (hr >= 0 && enumPins != null)
    {
        IPin[] pins = new IPin[1];
        int fetched;
        
        while (enumPins.Next(1, pins, out fetched) == 0 && fetched > 0)
        {
            PinInfo pinInfo = new PinInfo();
            pins[0].QueryPinInfo(out pinInfo);
            
            Console.WriteLine($"Pin Name: {pinInfo.name}, Direction: {pinInfo.dir}");
            
            // Libérer le pin et l'info
            if (pinInfo.filter != null)
                Marshal.ReleaseComObject(pinInfo.filter);
                
            Marshal.ReleaseComObject(pins[0]);
        }
        
        Marshal.ReleaseComObject(enumPins);
    }
}
```

## Identifier le bon filtre

Lorsque vous travaillez avec l'événement `OnFilterAdded`, gardez à l'esprit qu'il peut être appelé plusieurs fois au fur et à mesure que divers filtres sont ajoutés au graphe. Pour travailler avec un filtre spécifique, vous devrez l'identifier correctement :

```cs
private void VideoCaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
{
    IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
    
    if (baseFilter != null)
    {
        FilterInfo filterInfo = new FilterInfo();
        baseFilter.QueryFilterInfo(out filterInfo);
        
        // Vérifier si c'est le filtre que nous cherchons
        if (filterInfo.achName == "Video Capture Device")
        {
            // C'est notre filtre cible, effectuer les opérations spécifiques
            ConfigureVideoCaptureFilter(baseFilter);
        }
        
        // Libérer la référence au graphe de filtres
        if (filterInfo.pGraph != null)
        {
            Marshal.ReleaseComObject(filterInfo.pGraph);
        }
    }
}
```

## Configuration avancée du filtre

Une fois que vous avez accès à l'interface du filtre, vous pouvez effectuer des configurations avancées :

### Définir les propriétés du filtre

```cs
private void SetFilterProperty(IBaseFilter filter, Guid propertySet, int propertyId, object propertyValue)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    
    if (propertySetInterface != null)
    {
        // Convertir la valeur de propriété en tableau d'octets
        byte[] propertyData = ConvertToByteArray(propertyValue);
        
        // Définir la propriété
        int hr = propertySetInterface.Set(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length
        );
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
}
```

### Récupérer les propriétés du filtre

```cs
private object GetFilterProperty(IBaseFilter filter, Guid propertySet, int propertyId, Type propertyType)
{
    IKsPropertySet propertySetInterface = filter as IKsPropertySet;
    object result = null;
    
    if (propertySetInterface != null)
    {
        int dataSize = Marshal.SizeOf(propertyType);
        byte[] propertyData = new byte[dataSize];
        int returnedDataSize;
        
        // Obtenir la propriété
        int hr = propertySetInterface.Get(
            propertySet,
            propertyId,
            IntPtr.Zero,
            0,
            propertyData,
            propertyData.Length,
            out returnedDataSize
        );
        
        if (hr >= 0)
        {
            result = ConvertFromByteArray(propertyData, propertyType);
        }
        
        Marshal.ReleaseComObject(propertySetInterface);
    }
    
    return result;
}
```

## Cas d'usage courants des interfaces de filtre personnalisées

### Filtres de traitement vidéo

Lorsque vous travaillez avec la vidéo, vous pourriez avoir besoin d'accéder à des propriétés spécifiques des périphériques caméra :

```cs
private void ConfigureVideoCaptureFilter(IBaseFilter captureFilter)
{
    // Accéder aux propriétés de la caméra et les définir
    IAMCameraControl cameraControl = captureFilter as IAMCameraControl;
    
    if (cameraControl != null)
    {
        // Définir l'exposition
        cameraControl.Set(CameraControlProperty.Exposure, 0, CameraControlFlags.Manual);
        
        // Définir la mise au point
        cameraControl.Set(CameraControlProperty.Focus, 0, CameraControlFlags.Manual);
        
        Marshal.ReleaseComObject(cameraControl);
    }
}
```

### Filtres de traitement audio

Pour le traitement audio, vous voudrez peut-être ajuster le volume ou les paramètres de qualité audio :

```cs
private void ConfigureAudioFilter(IBaseFilter audioFilter)
{
    // Accéder à l'interface de volume
    IBasicAudio basicAudio = audioFilter as IBasicAudio;
    
    if (basicAudio != null)
    {
        // Définir le volume (0 à -10000, où 0 est le max et -10000 le min)
        basicAudio.put_Volume(-2000); // 80% du volume
        
        Marshal.ReleaseComObject(basicAudio);
    }
}
```

## Gérer correctement les ressources

Lorsque vous travaillez avec des interfaces DirectShow, il est crucial de libérer correctement les objets COM pour éviter les fuites de mémoire :

```cs
private void ReleaseComObject(object comObject)
{
    if (comObject != null)
    {
        Marshal.ReleaseComObject(comObject);
    }
}
```

## Exemple complet

Voici un exemple plus complet qui montre comment trouver et configurer un filtre de capture vidéo :

```cs
using System;
using System.Runtime.InteropServices;
using VisioForge.Libs.DirectShowLib;  // Namespace public — IBaseFilter, FilterInfo, IPin, etc.

public class CustomFilterExample
{
    private VideoCaptureCore captureCore;
    
    public void Initialize()
    {
        captureCore = new VideoCaptureCore();
        captureCore.OnFilterAdded += CaptureCore_OnFilterAdded;
        
        // Configurer la source
        // ...
        
        // Démarrer la capture
        captureCore.Start();
    }
    
    private void CaptureCore_OnFilterAdded(object sender, FilterEventArgs eventArgs)
    {
        IBaseFilter baseFilter = eventArgs.Filter as IBaseFilter;
        
        if (baseFilter != null)
        {
            // Obtenir les informations du filtre
            FilterInfo filterInfo = new FilterInfo();
            baseFilter.QueryFilterInfo(out filterInfo);
            
            Console.WriteLine($"Filter added: {filterInfo.achName}");
            
            // Vérifier si c'est le filtre de capture vidéo
            if (filterInfo.achName.Contains("Video Capture"))
            {
                ConfigureVideoCaptureFilter(baseFilter);
            }
            
            // Libérer la référence au graphe de filtres
            if (filterInfo.pGraph != null)
            {
                Marshal.ReleaseComObject(filterInfo.pGraph);
            }
        }
    }
    
    private void ConfigureVideoCaptureFilter(IBaseFilter captureFilter)
    {
        // Votre code de configuration de filtre ici
    }
    
    public void Cleanup()
    {
        if (captureCore != null)
        {
            captureCore.Stop();
            captureCore.OnFilterAdded -= CaptureCore_OnFilterAdded;
            captureCore.Dispose();
            captureCore = null;
        }
    }
}
```

## Composants système requis

Pour utiliser la fonctionnalité DirectShow dans votre application, assurez-vous que vos utilisateurs finaux disposent des composants suivants :

- DirectX Runtime (inclus avec Windows)
- Composants redistribuables du SDK

## Conclusion

Travailler avec des interfaces de filtre DirectShow personnalisées vous donne des capacités puissantes pour le traitement multimédia dans vos applications .NET. En suivant les modèles décrits dans ce guide, vous pouvez accéder aux composants DirectShow sous-jacents et les manipuler pour obtenir un contrôle précis sur vos applications multimédias.

Pour une aide supplémentaire sur l'implémentation de ces techniques, veuillez contacter notre équipe de support. Visitez notre dépôt GitHub pour plus d'exemples de code et d'exemples d'implémentation.
