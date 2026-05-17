---
title: SDK d'effets vidéo — ajouter et configurer en .NET C#
description: Ajoutez et configurez des effets vidéo dans les SDK .NET pour la capture, la lecture et l'édition avec gestion des paramètres et exemples C# pratiques.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - C#
primary_api_classes:
  - VideoEffectLightness

---

# Implémentation d'effets vidéo dans les applications SDK .NET

Les effets vidéo peuvent améliorer significativement la qualité visuelle et l'expérience utilisateur de vos applications multimédias. Ce guide montre comment implémenter et gérer correctement les effets vidéo dans divers environnements SDK .NET.

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble de l'implémentation

Lorsque vous travaillez avec le traitement vidéo dans des applications .NET, vous devrez souvent appliquer divers effets pour améliorer ou modifier le contenu vidéo. Les sections suivantes expliquent ce processus étape par étape.

## Implémentation en code C#

### Exemple : effet de clarté dans le Media Player SDK

Cet exemple détaillé montre comment implémenter un effet de clarté, une technique courante d'amélioration vidéo. La même approche d'implémentation s'applique aux environnements Video Edit SDK .Net et Video Capture SDK .Net.

### Étape 1 : déclarer l'interface de l'effet

Tout d'abord, vous devez déclarer l'interface appropriée pour l'effet souhaité :

```cs
IVideoEffectLightness lightness;
```

### Étape 2 : récupérer ou créer l'instance d'effet

Chaque effet requiert un identifiant unique. Le code suivant vérifie si l'effet existe déjà dans le composant SDK :

```cs
var effect = MediaPlayer1.Video_Effects_Get("Lightness");
```

### Étape 3 : ajouter l'effet s'il n'est pas présent

Si l'effet n'existe pas encore, vous devrez l'instancier et l'ajouter à votre pipeline de traitement vidéo :

```cs
if (effect == null) 
{ 
    lightness = new VideoEffectLightness(true, 100);
    MediaPlayer1.Video_Effects_Add(lightness); 
}
```

### Étape 4 : mettre à jour les paramètres d'un effet existant

Si l'effet est déjà présent, vous pouvez modifier ses paramètres pour atteindre le résultat visuel souhaité :

```cs
else
{
   lightness = effect as IVideoEffectLightness;
   if (lightness != null)
   {
      lightness.Value = 100;
   }
}
```

## Notes importantes d'implémentation

Pour un fonctionnement correct, assurez-vous d'activer le traitement des effets avant de démarrer la lecture ou la capture vidéo :

* Définissez la propriété `Video_Effects_Enabled` sur `true` avant d'appeler toute méthode `Play()` ou `Start()`
* Les effets ne seront pas appliqués si cette propriété n'est pas activée
* La modification des paramètres d'effet pendant la lecture mettra à jour la sortie visuelle en temps réel

## Configuration système requise

Pour implémenter avec succès les effets vidéo dans votre application .NET, vous aurez besoin de :

* Les paquets redistribuables du SDK correctement installés
* Des ressources système suffisantes pour le traitement vidéo en temps réel
* Une version appropriée du framework .NET

## Ressources supplémentaires

Pour des implémentations plus avancées et des exemples de techniques d'effets vidéo :

---
Visitez notre dépôt [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code supplémentaires et des projets complets.