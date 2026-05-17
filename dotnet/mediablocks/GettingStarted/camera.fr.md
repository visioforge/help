---
title: Construire une application de capture caméra en C# .NET
description: Construisez des applications caméra avec VisioForge Media Blocks SDK — énumération, sélection de format, rendu vidéo et capture multiplateforme.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Webcam
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - SystemVideoSourceBlock
  - VideoRendererBlock
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings

---

# Construire des applications caméra avec Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Ce guide complet montre comment créer une application de visualisation de caméra entièrement fonctionnelle à l'aide du Media Blocks SDK .Net. Le SDK fournit un framework robuste pour capturer, traiter et afficher des flux vidéo sur de multiples plateformes, notamment Windows, macOS, iOS et Android.

## Vue d'ensemble de l'architecture

Pour créer une application de visualisation de caméra, vous devez comprendre deux composants fondamentaux :

1. **Source vidéo système** — capture le flux vidéo des caméras connectées
2. **Moteur de rendu vidéo** — affiche la vidéo capturée à l'écran avec des paramètres configurables

Ces composants fonctionnent ensemble au sein d'une architecture de pipeline qui gère le traitement multimédia.

## Blocs multimédias essentiels

Pour construire une application caméra, vous devez ajouter les blocs suivants à votre pipeline :

- **[Bloc de source vidéo système](../Sources/index.md)** — se connecte aux périphériques caméra et les lit
- **[Bloc moteur de rendu vidéo](../VideoRendering/index.md)** — affiche la vidéo avec des options de rendu configurables

## Mise en place du pipeline

### Création du pipeline de base

Créez d'abord un objet pipeline qui gérera le flux multimédia :

```csharp
using VisioForge.Core.MediaBlocks;

// Initialiser le pipeline
var pipeline = new MediaBlocksPipeline();

// Ajouter la gestion des erreurs
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Erreur du pipeline : {args.Message}");
};
```

### Énumération des périphériques caméra

Avant d'ajouter une source caméra, vous devez énumérer les périphériques disponibles et en sélectionner un :

```csharp
// Obtenir tous les périphériques vidéo disponibles de façon asynchrone
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();

// Afficher les périphériques disponibles (utile pour la sélection par l'utilisateur)
foreach (var device in videoDevices)
{
    Console.WriteLine($"Périphérique : {device.Name} [{device.API}]");
}

// Sélectionner le premier périphérique disponible
var selectedDevice = videoDevices[0];
```

### Sélection du format caméra

Chaque caméra prend en charge différentes résolutions et fréquences d'images. Vous pouvez les énumérer et sélectionner le format optimal :

```csharp
// Afficher les formats disponibles pour le périphérique sélectionné
foreach (var format in selectedDevice.VideoFormats)
{
    Console.WriteLine($"Format : {format.Width}x{format.Height} {format.Format}");
    
    // Afficher les fréquences d'images disponibles pour ce format
    foreach (var frameRate in format.FrameRateList)
    {
        Console.WriteLine($"  Fréquence d'images : {frameRate}");
    }
}

// Sélectionner le format optimal (dans cet exemple, on recherche la résolution HD)
var hdFormat = selectedDevice.GetHDVideoFormatAndFrameRate(out var frameRate);
var formatToUse = hdFormat ?? selectedDevice.VideoFormats[0];
```

## Configuration des paramètres caméra

### Création des paramètres de source

Configurez les paramètres de la source caméra avec votre périphérique et votre format sélectionnés :

```csharp
// Créer les paramètres caméra avec le périphérique et le format sélectionnés
var videoSourceSettings = new VideoCaptureDeviceSourceSettings(selectedDevice)
{
    Format = formatToUse.ToFormat()
};

// Définir la fréquence d'images souhaitée (en choisissant la plus élevée disponible)
if (formatToUse.FrameRateList.Count > 0)
{
    videoSourceSettings.Format.FrameRate = formatToUse.FrameRateList.Max();
}

// Optionnel : forcer la fréquence d'images pour conserver un timing constant
videoSourceSettings.Format.ForceFrameRate = true;

// Paramètres spécifiques aux plateformes
#if __ANDROID__
// Paramètres spécifiques à Android
videoSourceSettings.VideoStabilization = true;
#elif __IOS__ && !__MACCATALYST__
// Paramètres spécifiques à iOS
videoSourceSettings.Position = IOSVideoSourcePosition.Back;
videoSourceSettings.Orientation = IOSVideoSourceOrientation.Portrait;
#endif
```

### Création du bloc de source vidéo

Créez maintenant le bloc de source vidéo système avec vos paramètres configurés :

```csharp
// Créer le bloc de source vidéo
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
```

## Configuration de l'affichage vidéo

### Création du moteur de rendu vidéo

Ajoutez un moteur de rendu vidéo pour afficher la vidéo capturée :

```csharp
// Créer le moteur de rendu vidéo et le connecter à votre composant d'UI
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Optionnel : garder le moteur de rendu synchronisé avec l'horloge du pipeline
videoRenderer.IsSync = true;
```

### Configuration avancée du moteur de rendu

`VideoRendererBlock` expose une poignée de propriétés à plat — pas de bag `Settings` imbriqué :

```csharp
// Activer la superposition de sous-titres si le pipeline transporte un flux de sous-titres
videoRenderer.SubtitleEnabled = true;

// Désactiver la synchronisation d'horloge pour obtenir les images aussi vite qu'elles arrivent (utile pour les scénarios d'enregistrement)
videoRenderer.IsSync = false;
```

## Connexion du pipeline

Connectez la source vidéo au moteur de rendu pour établir le flux multimédia :

```csharp
// Connecter la sortie de la source vidéo à l'entrée du moteur de rendu
pipeline.Connect(videoSource.Output, videoRenderer.Input);
```

## Gestion du cycle de vie du pipeline

### Démarrage du pipeline

Démarrez le pipeline pour commencer la capture et l'affichage vidéo :

```csharp
// Démarrer le pipeline de manière asynchrone
await pipeline.StartAsync();
```

### Prise de captures d'écran

Capturez des images fixes depuis le flux vidéo :

```csharp
// Prendre une capture d'écran et l'enregistrer en JPEG
await videoRenderer.Snapshot_SaveAsync("camera_snapshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);

// Ou obtenir la capture sous forme de bitmap pour un traitement ultérieur
var bitmap = await videoRenderer.Snapshot_GetAsync();
```

### Arrêt du pipeline

Une fois terminé, arrêtez correctement le pipeline :

```csharp
// Arrêter le pipeline de manière asynchrone
await pipeline.StopAsync();
```

## Considérations spécifiques aux plateformes

Le Media Blocks SDK prend en charge le développement multiplateforme avec des optimisations spécifiques :

- **Windows** : prend en charge à la fois les API Media Foundation et Kernel Streaming
- **macOS/iOS** : utilise AVFoundation pour des performances optimales
- **Android** : fournit l'accès à des fonctionnalités caméra comme la stabilisation et l'orientation

## Gestion des erreurs et dépannage

Mettez en place une gestion d'erreurs appropriée pour garantir une application stable :

```csharp
try
{
    // Opérations sur le pipeline
    await pipeline.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Erreur lors du démarrage du pipeline : {ex.Message}");
    // Gérer l'exception de manière appropriée
}
```

## Exemple d'implémentation complète

Cet exemple présente une implémentation complète de visualisateur de caméra :

```csharp
using System;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

public class CameraViewerExample
{
    private MediaBlocksPipeline _pipeline;
    private SystemVideoSourceBlock _videoSource;
    private VideoRendererBlock _videoRenderer;
    
    public async Task InitializeAsync(IVideoView videoView)
    {
        // Créer le pipeline
        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, e) => Console.WriteLine(e.Message);
        
        // Énumérer les périphériques
        var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
        if (devices.Length == 0)
        {
            throw new Exception("Aucun périphérique caméra trouvé");
        }
        
        // Sélectionner le périphérique et le format
        var device = devices[0];
        var format = device.GetHDOrAnyVideoFormatAndFrameRate(out var frameRate);
        
        // Créer les paramètres
        var settings = new VideoCaptureDeviceSourceSettings(device);
        if (format != null)
        {
            settings.Format = format.ToFormat();
            if (frameRate != null && !frameRate.IsEmpty)
            {
                settings.Format.FrameRate = frameRate;
            }
        }
        
        // Créer les blocs
        _videoSource = new SystemVideoSourceBlock(settings);
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView);
        
        // Construire le pipeline
        _pipeline.AddBlock(_videoSource);
        _pipeline.AddBlock(_videoRenderer);
        _pipeline.Connect(_videoSource.Output, _videoRenderer.Input);
        
        // Démarrer le pipeline
        await _pipeline.StartAsync();
    }
    
    public async Task StopAsync()
    {
        if (_pipeline != null)
        {
            await _pipeline.StopAsync();
            _pipeline.Dispose();
        }
    }
    
    public async Task<bool> TakeSnapshotAsync(string filename)
    {
        return await _videoRenderer.Snapshot_SaveAsync(filename, 
            SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
    }
}
```

## Conclusion

Avec Media Blocks SDK .Net, construire des applications caméra puissantes devient simple. L'architecture à composants offre flexibilité et performance sur toutes les plateformes tout en masquant la complexité de l'intégration des périphériques caméra.

Pour des exemples de code source complets, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo).
