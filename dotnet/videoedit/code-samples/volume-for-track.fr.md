---
title: Volume d'une piste audio dans un éditeur C# .NET VisioForge
description: Contrôlez les volumes audio individuels par piste avec VisioForge Video Edit SDK .NET. Mixage par piste avec des exemples de code C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - Effects
  - C#
  - NuGet
primary_api_classes:
  - AudioVolumeEnvelopeEffect
  - AudioSource

---

# Définir des niveaux de volume personnalisés pour les pistes audio dans les applications C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Vue d'ensemble

La gestion des niveaux de volume audio est un aspect essentiel des applications de production et d'édition vidéo. Ce guide montre comment implémenter des contrôles de volume individuels pour des pistes audio distinctes dans votre application .NET.

## Détails d'implémentation

Définir des niveaux de volume personnalisés pour les pistes audio offre à vos utilisateurs un contrôle plus précis sur leur mixage audio. Chaque piste peut avoir son propre réglage de volume indépendant, permettant un équilibrage audio de qualité professionnelle.

## Exemple d'implémentation

L'exemple C# suivant montre comment appliquer un effet d'enveloppe de volume à une piste audio :

```cs
// AudioVolumeEnvelopeEffect(level) définit un volume constant pour la piste.
// Les propriétés facultatives StartTime / StopTime (TimeSpan) restreignent l'effet à une fenêtre temporelle.
var volume = new AudioVolumeEnvelopeEffect(level: 10);

// La surcharge à 5 arguments Input_AddAudioFile prend un AudioSource (pas une chaîne),
// donc enveloppez le chemin du fichier avant de le passer avec le tableau d'effets.
// Signature : Input_AddAudioFile(AudioSource, TimeSpan? timelineInsertTime = null,
//   int targetStreamIndex = 0, AudioTrackEffect[] effects = null,
//   TimelineAudioTrackCustomSettings = null)
var audioSource = new AudioSource(audioFile);
VideoEdit1.Input_AddAudioFile(audioSource, null, 0, new[] { volume });
```

## Comprendre les paramètres

- `AudioVolumeEnvelopeEffect(level: 10)` : enveloppe à volume constant. Level est un `int` ; utilisez les propriétés `StartTime`/`StopTime` (toutes deux `TimeSpan`) pour limiter l'effet à une fenêtre temporelle.
- `audioFile` (string) doit être enveloppé dans un `new AudioSource(...)` — la surcharge avec tableau d'effets de `Input_AddAudioFile` accepte uniquement un `AudioSource`, pas un nom de fichier brut.
- `Input_AddAudioFile` : ajoute le fichier audio à la timeline avec les effets donnés appliqués au flux audio choisi.

## Dépendances requises

Pour implémenter cette fonctionnalité, vous aurez besoin des paquets redistribuables suivants :

- Redistribuables Video Edit SDK :
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Informations de déploiement

Pour des informations sur l'installation ou le déploiement des composants requis sur les systèmes de vos utilisateurs finaux, veuillez consulter notre [guide de déploiement](../deployment.md).

---
## Ressources supplémentaires
Pour plus d'exemples de code et de techniques d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) avec des projets d'exemple complets.
