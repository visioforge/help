---
title: Fusionner sources vidéo et audio en C# .NET — Video Edit
description: Combinez plusieurs fichiers vidéo et audio en une seule sortie sans réencodage avec VisioForge Video Edit SDK .NET. Guide de fusion en C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - AVI
  - C#
  - NuGet

---

# Créer de nouveaux fichiers à partir de plusieurs sources sans réencodage

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

Lors du développement d'applications multimédias, vous pouvez avoir besoin de combiner du contenu provenant de différents fichiers. Ce guide explique comment fusionner des flux provenant de plusieurs sources vidéo et audio dans un seul fichier de sortie sans perte de qualité due au réencodage.

## Avantages du travail avec plusieurs sources

- Préserver la qualité originale de tous les fichiers sources
- Combiner des pistes audio de sources différentes
- Ajouter de la musique d'arrière-plan à des fichiers vidéo
- Créer du contenu multilingue avec différentes pistes audio
- Économiser du temps de traitement en évitant un réencodage inutile

## Implémentation étape par étape

### 1. Initialiser la collection de flux

D'abord, créez une liste pour contenir toutes les références de flux :

```cs
var streams = new List<FFMPEGStream>();
```

### 2. Ajouter un flux vidéo

Ajoutez un flux vidéo à partir de votre premier fichier source. L'ID « v » désigne ce flux comme composant vidéo :

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video.avi",
                ID = "v"
});
```

### 3. Ajouter le flux audio principal

Incorporez un flux audio à partir d'un fichier MP3. L'ID « a » identifie ce flux comme composant audio :

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!sophie.mp3",
                ID = "a"
});
```

### 4. Ajouter des flux audio supplémentaires

Vous pouvez ajouter d'autres flux audio à partir d'autres fichiers vidéo. À nouveau, utilisez l'ID « a » pour spécifier qu'il s'agit d'un composant audio :

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video2.avi",
                ID = "a"
});
```

### 5. Traiter et générer la sortie

Enfin, combinez tous les flux dans un seul fichier de sortie. Définir le second paramètre à « true » garantit que la durée de sortie correspond au flux le plus court, évitant les problèmes de lecture :

```cs
await VideoEdit1.FastEdit_MuxStreamsAsync(streams, true, outputFile);
```

## Considérations techniques importantes

Lorsque vous combinez des flux provenant de plusieurs sources, gardez à l'esprit :

- Les formats sources doivent être compatibles avec le format de conteneur de sortie
- La compatibilité du codec audio doit être vérifiée au préalable
- La synchronisation des flux peut nécessiter une configuration supplémentaire dans des scénarios complexes
- Certains lecteurs peuvent rencontrer des problèmes si les durées des flux varient considérablement

## Dépendances requises

Pour mettre en œuvre cette fonctionnalité, vous devrez référencer :

- Redist du SDK
- Redist FFMPEG [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)

Pour plus d'informations sur le déploiement de ces dépendances aux utilisateurs finaux, consultez [notre guide de déploiement](../deployment.md).

## Ressources supplémentaires

Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code et des exemples d'implémentation supplémentaires.
