---
title: Superposition d'image avec positionnement en C# .NET
description: Mettez en œuvre des superpositions d'image avec VisioForge Video Edit SDK .NET. Guide pas à pas pour positionnement, transparence et minutage.
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
  - C#
primary_api_classes:
  - ImageOverlayVideoEffect
  - VideoEffectImageLogo

---

# Ajouter des superpositions d'image aux vidéos en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introduction aux superpositions d'image

Notre SDK .NET fournit une fonctionnalité puissante pour ajouter des superpositions d'image à vos projets vidéo. Avec cette fonctionnalité, les développeurs peuvent intégrer sans heurts des logos, filigranes, graphiques et autres éléments visuels dans le contenu vidéo. Le SDK offre de vastes options de personnalisation, notamment le positionnement précis, le réglage de la transparence et le contrôle du minutage.

## Formats de fichiers image pris en charge

Le SDK est compatible avec tous les formats d'image standard utilisés en production vidéo professionnelle :

- BMP (Bitmap)
- GIF (Graphics Interchange Format)
- JPEG/JPG (Joint Photographic Experts Group)
- PNG (Portable Network Graphics)
- TIFF (Tagged Image File Format)

## Guide d'implémentation

Vous trouverez ci-dessous des exemples de code détaillés démontrant comment mettre en œuvre des superpositions d'image dans vos applications de traitement vidéo à l'aide de notre SDK.

### Utiliser le moteur VideoEditCoreX

L'exemple de code suivant illustre comment ajouter une superposition d'image avec positionnement personnalisé, transparence et minutage à l'aide du moteur VideoEditCoreX :

```cs
// Ajouter une superposition d'image aux effets de la source vidéo à partir d'un fichier PNG
var imageOverlay = new ImageOverlayVideoEffect("logo.png");

// Définir la position
imageOverlay.X = 50;
imageOverlay.Y = 50;

// Définir l'alpha
imageOverlay.Alpha = 0.5;

// Définir l'heure de début et l'heure de fin
imageOverlay.StartTime = TimeSpan.FromSeconds(0);
imageOverlay.StopTime = TimeSpan.FromSeconds(5);

// Ajouter la source vidéo à la timeline
VideoEdit1.Video_Effects.Add(imageOverlay);
```

### Utiliser le moteur VideoEditCore

Pour les développeurs travaillant avec le moteur VideoEditCore, voici comment obtenir la même fonctionnalité :

```cs
var effect = new VideoEffectImageLogo(true, "Logo1");

// Pointer vers le fichier image (ou assigner effect.MemoryBitmap pour des données d'image en mémoire)
effect.Filename = "logo.png";

// Définir la position
effect.Left = 50;
effect.Top = 50;

// Définir le niveau global de transparence (0 = entièrement opaque, 255 = entièrement transparent)
effect.TransparencyLevel = 127;

// Définir l'heure de début et l'heure de fin
effect.StartTime = TimeSpan.FromSeconds(5);
effect.StopTime = TimeSpan.FromSeconds(15);

VideoEdit1.Video_Effects_Add(effect);
```

## Options de configuration avancées

Lors de la mise en œuvre de superpositions d'image, prenez en compte ces options de configuration supplémentaires :

- **Positionnement** : ajustez les valeurs X/Y ou Left/Top pour placer votre superposition avec précision
- **Transparence** : configurez Alpha ou TransparencyLevel pour contrôler l'opacité de la superposition
- **Minutage** : définissez StartTime et StopTime pour déterminer quand la superposition apparaît et disparaît
- **Taille** : vous pouvez redimensionner les superpositions pour les adapter à vos besoins spécifiques

## Ressources supplémentaires

Pour davantage d'exemples de code et des conseils de mise en œuvre, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
