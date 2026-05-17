---
title: Redimensionner, recadrer et mettre à l'échelle la vidéo C#
description: Redimensionnez et recadrez la vidéo en direct depuis webcams, écrans et caméras IP avec le SDK VisioForge. Contrôle du ratio d'aspect et région d'intérêt en C#.
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - NuGet
primary_api_classes:
  - VideoResizeSettings
  - VideoCropSettings

---

# Opérations de redimensionnement et de recadrage vidéo pour les développeurs .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au traitement vidéo

Lorsque vous travaillez avec des flux vidéo dans des applications .NET, contrôler les dimensions et la zone de focus de votre vidéo est essentiel pour créer des applications professionnelles. Ce guide explique comment implémenter des opérations de redimensionnement et de recadrage sur les flux vidéo provenant de webcams, captures d'écran, caméras IP et autres sources.

## Implémentation du redimensionnement vidéo

Le redimensionnement vous permet de standardiser les dimensions vidéo entre différentes sources vidéo, ce qui est particulièrement utile lorsque vous travaillez avec plusieurs entrées de caméra ou lorsque vous ciblez des formats de sortie spécifiques.

### Étape 1 : activer la fonctionnalité de redimensionnement

Activez d'abord la fonctionnalité de redimensionnement ou de recadrage dans votre application :

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Étape 2 : configurer les paramètres de redimensionnement

Définissez la largeur et la hauteur souhaitées, et déterminez s'il faut maintenir le ratio d'aspect avec un letterboxing :

```cs
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width = 640,
    Height = 480,
    LetterBox = true
};
```

### Étape 3 : sélectionner l'algorithme de redimensionnement approprié

Choisissez l'algorithme qui correspond le mieux à vos exigences de performance et de qualité :

```cs
// Video_Resize est typé comme l'interface marqueur IVideoResizeSettings ;
// Mode vit sur la classe concrète VideoResizeSettings, donc castez avant d'assigner.
var resize = (VideoResizeSettings)VideoCapture1.Video_Resize;
switch (cbResizeMode.SelectedIndex)
{
  case 0: resize.Mode = VideoResizeMode.NearestNeighbor; break;
  case 1: resize.Mode = VideoResizeMode.Bilinear; break;
  case 2: resize.Mode = VideoResizeMode.Bicubic; break;
  case 3: resize.Mode = VideoResizeMode.Lancroz; break;
}
```

## Implémentation du recadrage vidéo

Le recadrage vous permet de vous concentrer sur des régions d'intérêt spécifiques dans votre flux vidéo, en supprimant les zones indésirables de l'image.

### Étape 1 : activer la fonctionnalité de recadrage

De manière similaire au redimensionnement, activez d'abord la fonctionnalité de recadrage :

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Étape 2 : définir la région de recadrage

Spécifiez la région de recadrage en définissant les marges à supprimer de chaque bord de l'image vidéo :

```cs
VideoCapture1.Video_Crop = new VideoCropSettings(40, 0, 40, 0);
```

## Considérations de performance

Lors de l'implémentation des opérations de redimensionnement et de recadrage dans des applications en production, considérez ce qui suit :

- Les opérations de redimensionnement nécessitent des ressources CPU, en particulier aux résolutions plus élevées
- Les algorithmes plus complexes (Bicubic, Lanczos) fournissent une meilleure qualité mais nécessitent plus de puissance de traitement
- Pour les applications temps réel, équilibrez qualité et performance en fonction de votre matériel cible

## Dépendances requises

Assurez-vous que votre projet inclut les paquets redistribuables nécessaires :

- Redist Video Capture [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Ressources supplémentaires

Pour des implémentations plus avancées et des exemples de code, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant de nombreux exemples pour les développeurs .NET.
