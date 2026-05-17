---
title: Éditeur vidéo iOS en C# .NET — rognage, effets et timeline
description: Intégrez l'édition vidéo professionnelle dans vos applications iOS avec VisioForge Video Edit SDK .NET. Rognage, filtres, transitions et effets.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - Effects
  - MP4
  - C#
primary_api_classes:
  - VideoEditCoreX
  - IVideoView
  - VideoBalanceVideoEffect
  - MP4Output
  - ProgressEventArgs

---

# Éditeur vidéo iOS pour une édition vidéo intégrée et fluide

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introduction à l'édition vidéo sur iOS

Créer une application d'édition vidéo professionnelle pour iPhone et iPad nécessite un SDK robuste qui offre des performances natives avec des fonctionnalités personnalisables. Le VisioForge Video Edit SDK fournit les outils pour créer des applications d'édition impressionnantes qui rivalisent avec Adobe Premiere ou DaVinci Resolve sur les appareils Apple.

Notre Video Edit SDK iOS intègre efficacement des capacités avancées d'édition vidéo dans votre application iOS. Construisez une application photo-vidéo, des outils de création de contenu ou une application d'éditeur vidéo pro avec les fonctionnalités que les utilisateurs attendent des applications d'édition modernes.

## Fonctionnalités principales

Le SDK fournit des fonctionnalités complètes d'édition vidéo pour le développement d'applications iOS :

- **Rognage** : rognage vidéo à la précision de l'image avec des contrôles adaptés au tactile
- **Timeline** : éditez plusieurs pistes vidéo et audio simultanément
- **Transitions** : effets fluides incluant fondus et balayages entre clips
- **Effets vidéo** : appliquez des filtres et une correction colorimétrique à vos vidéos
- **Mélange audio** : contrôlez le volume et mélangez plusieurs sources audio
- **Superpositions de texte** : ajoutez des titres et filigranes personnalisables

Bien qu'optimisé pour iOS, notre framework prend en charge Android via .NET MAUI, vous permettant de créer des solutions d'édition multiplateformes.

## Prise en main avec VideoEditCoreX

### Initialisation du SDK

Initialisez le moteur d'édition vidéo dans votre application iOS :

```csharp
using System.Drawing;                              // Size
using VisioForge.Core;                             // VisioForgeX
using VisioForge.Core.Types;                       // IVideoView, VideoFrameRate
using VisioForge.Core.Types.X.VideoEdit;           // VideoTransition, VideoTransitionType
using VisioForge.Core.VideoEditX;                  // VideoEditCoreX

await VisioForgeX.InitSDKAsync();

var videoEdit = new VideoEditCoreX(VideoView1 as IVideoView);
videoEdit.OnError += VideoEdit_OnError;
videoEdit.OnProgress += VideoEdit_OnProgress;
videoEdit.OnStop += VideoEdit_OnStop;
```

### Ajouter du contenu vidéo

Ajoutez des fichiers vidéo à votre timeline :

```csharp
// Ajouter un fichier vidéo complet
videoEdit.Input_AddVideoFile("input.mp4", null);

// Ou ajouter une vidéo avec des heures de début et de fin spécifiques
videoEdit.Input_AddAudioVideoFile(
    "input.mp4",
    TimeSpan.FromMilliseconds(0),
    TimeSpan.FromMilliseconds(10000),
    insertTime: null);
```

### Appliquer des effets

Améliorez les vidéos avec des effets que les utilisateurs choisissent pour leur contenu :

```csharp
using VisioForge.Core.Types.X.VideoEffects;

var balance = new VideoBalanceVideoEffect();
balance.Brightness = 0.1;
balance.Contrast = 1.0;
videoEdit.Video_Effects.Add(balance);
```

### Configurer la sortie

Exportez des vidéos optimisées pour YouTube ou conformes à la politique de l'App Store :

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;

videoEdit.Output_VideoSize = new Size(1920, 1080);
videoEdit.Output_VideoFrameRate = new VideoFrameRate(30);

var mp4Output = new MP4Output("output.mp4");
videoEdit.Output_Format = mp4Output;
videoEdit.Start();
```

### Gestion des événements

Surveillez la progression de l'édition :

```csharp
private void VideoEdit_OnProgress(object sender, ProgressEventArgs e)
{
    Console.WriteLine($"Progress: {e.Progress}%");
}

private void VideoEdit_OnStop(object sender, StopEventArgs e)
{
    Console.WriteLine(e.Successful ? "Completed" : "Error");
}
```

## Options avancées

### API de superposition de texte

Ajoutez des superpositions de texte à l'aide de l'API de rendu native :

```csharp
using VisioForge.Core.Types.X.VideoEdit;

var textOverlay = new TextOverlay("Your Title");
videoEdit.Video_TextOverlays.Add(textOverlay);
```

### Transitions vidéo

Créez des transitions fluides entre les clips :

```csharp
var transition = new VideoTransition(
    VideoTransitionType.Crossfade,
    TimeSpan.FromMilliseconds(1000),
    TimeSpan.FromMilliseconds(2000));
videoEdit.Video_Transitions.Add(transition);
```

## Déploiement iOS

Pour des instructions détaillées de déploiement iOS, notamment les paquets NuGet, les permissions et les bonnes pratiques, consultez notre [Guide de déploiement iOS](../../deployment-x/iOS.md).

## Pourquoi choisir VisioForge

- **API professionnelle** : contrôle complet sur l'édition vidéo
- **Interface personnalisable** : créez votre propre interface
- **Performances natives** : encodage accéléré par GPU sur les appareils Apple

---
Explorez les exemples d'édition vidéo iOS sur notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) ou contactez le [support](https://support.visioforge.com/) pour des ressources.