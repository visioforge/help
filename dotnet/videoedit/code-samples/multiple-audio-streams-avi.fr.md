---
title: Pistes audio multilingues dans un AVI en C# .NET VisioForge
description: Intégrez plusieurs pistes audio dans des fichiers AVI avec VisioForge Video Edit SDK .NET. Prise en charge multilingue et commentaires en C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - AVI
  - C#
  - NuGet
primary_api_classes:
  - AudioSource
  - VideoSource

---

# Ajouter plusieurs flux audio à des fichiers AVI en .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

Les flux audio multiples vous permettent d'inclure différentes pistes linguistiques, des commentaires ou des options musicales dans un seul fichier vidéo. Cette fonctionnalité est essentielle pour créer du contenu multilingue ou fournir des expériences audio alternatives aux spectateurs.

## Détails d'implémentation

Lors de la création de plusieurs flux audio dans un fichier AVI, vous devez ajouter chaque source audio à la timeline en utilisant des paramètres de ciblage spécifiques. Cette approche garantit que chaque flux audio est correctement indexé et accessible pendant la lecture.

## Exemple de code

L'exemple C# suivant montre comment ajouter deux flux audio différents à un fichier AVI :

```cs
var videoSource = new VideoSource("video1.avi");
var audioSource1 = new AudioSource("video1.avi");
var audioSource2 = new AudioSource("audio2.mp3"); 

VideoEdit1.Input_Clear_List();
VideoEdit1.Input_AddVideoFile(videoSource);
VideoEdit1.Input_AddAudioFile(audioSource1, targetStreamIndex: 0);
VideoEdit1.Input_AddAudioFile(audioSource2, targetStreamIndex: 1);
```

## Explication des paramètres clés

- `targetStreamIndex` : définit à quel index de flux audio la source sera attribuée
- Le premier flux audio utilise l'index 0, le second utilise l'index 1, etc.
- Vous pouvez ajouter autant de flux audio que nécessaire en utilisant des valeurs d'index incrémentielles

## Dépendances requises

Pour mettre en œuvre cette fonctionnalité, vous aurez besoin de :

- Redistribuables Video Edit SDK :
  - [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Informations de déploiement

Pour les détails sur l'installation ou le déploiement des dépendances requises sur les systèmes des utilisateurs finaux, consultez notre [guide de déploiement](../deployment.md).

---
Trouvez d'autres exemples de code et détails d'implémentation sur notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).