---
title: Extraire et combiner des segments vidéo d'un fichier en C#
description: Extrayez et combinez plusieurs segments d'un seul fichier vidéo avec VisioForge Video Edit SDK .NET. Compilations et assemblage de clips en C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - C#
  - NuGet
primary_api_classes:
  - VideoSource
  - FileSegment
  - AudioSource

---

# Ajouter plusieurs segments d'un seul fichier vidéo en C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

Lors du développement d'applications d'édition vidéo, vous devez souvent extraire des portions spécifiques d'un fichier vidéo et les combiner en une nouvelle composition. Cette technique est essentielle pour créer des compilations, supprimer des sections indésirables ou assembler une compilation de moments clés à partir d'une vidéo plus longue.

Ce guide montre comment extraire et combiner par programme plusieurs segments du même fichier vidéo en C#. Vous apprendrez le processus étape par étape avec des exemples de code fonctionnels que vous pourrez implémenter dans vos propres applications.

## Pourquoi extraire plusieurs segments ?

Extraire des segments spécifiques de vidéos sert de nombreux objectifs pratiques :

- Créer des compilations à partir d'enregistrements plus longs
- Supprimer des sections indésirables (publicités, erreurs, contenu non pertinent)
- Assembler une compilation de moments clés
- Créer des bandes-annonces ou des aperçus à partir d'un contenu complet
- Générer des clips plus courts pour les réseaux sociaux à partir de vidéos plus longues

## Vue d'ensemble de l'implémentation

L'implémentation comporte trois étapes clés :

1. Définir les segments temporels que vous souhaitez extraire
2. Créer une source vidéo incluant ces segments spécifiés
3. Ajouter le fichier segmenté à votre timeline d'édition

Détaillons chaque étape avec des exemples de code et des explications détaillés.

## Implémentation détaillée

### Étape 1 : définir vos segments

D'abord, vous devez spécifier les heures de début et de fin de chaque segment. Chaque segment est défini par un point de départ et une durée, mesurés en millisecondes.

```cs
// Définir plusieurs segments à partir d'un seul fichier vidéo
FileSegment[] segments = new[] { 
    new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)),  // Les 5 premières secondes
    new FileSegment(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000))  // De 3s à 13s
};
```

Dans cet exemple, nous avons défini deux segments :

- Le premier segment commence au début de la vidéo (0 ms) et dure 5 secondes
- Le second segment commence à la marque des 3 secondes et continue pendant 10 secondes

Notez que les segments peuvent se chevaucher, comme indiqué dans cet exemple où le second segment commence avant la fin du premier. Cela peut être utile pour créer des transitions fluides ou lorsque vous souhaitez que certaines portions apparaissent plusieurs fois.

### Étape 2 : créer une source vidéo avec des segments

Ensuite, créez un objet VideoSource qui incorpore les segments que vous avez définis :

```cs
// Créer une source vidéo qui inclut les segments spécifiés
VideoSource videoFile = new VideoSource(
    videoFileName,   // Chemin de votre fichier vidéo
    segments,        // Tableau de segments défini ci-dessus
    VideoEditStretchMode.Letterbox,  // Comment gérer les différences de rapport d'aspect
    0,               // streamNumber — quel flux vidéo lire dans un fichier multi-flux
    1.0);            // rate — vitesse de lecture (1.0 = normale ; 2.0 = 2× ; 0.5 = demi-vitesse)
```

Le constructeur VideoSource prend plusieurs paramètres :

- `videoFileName` : le chemin de votre fichier vidéo source
- `segments` : le tableau d'objets FileSegment que vous avez défini à l'étape 1
- `VideoEditStretchMode` : comment gérer les différences de rapport d'aspect (Letterbox, Stretch, Crop)
- `streamNumber` : index commençant à zéro du flux vidéo à utiliser dans un fichier multi-flux (pas la rotation)
- `rate` : multiplicateur de vitesse de lecture — 1.0 = normal, 0.5 = ralenti, 2.0 = avance rapide

### Étape 3 : ajouter à la timeline

Enfin, ajoutez la source vidéo segmentée à votre timeline d'édition :

```cs
// Ajouter le fichier segmenté à la timeline.
// Signature : Input_AddVideoFile(VideoSource fileSource, TimeSpan? timelineInsertTime = null,
//   int targetVideoStream = 0, int customWidth = 0, int customHeight = 0).
// Passez uniquement la source pour ajouter à la fin actuelle de la timeline.
VideoEdit1.Input_AddVideoFile(videoFile);

// Ou insérez la source à une position spécifique de la timeline :
// VideoEdit1.Input_AddVideoFile(videoFile, TimeSpan.FromSeconds(5));
```

La méthode `Input_AddVideoFile` prend la `VideoSource` plus une position optionnelle d'insertion dans la timeline (`TimeSpan?`, et non un numéro de piste `int`). Des paramètres optionnels supplémentaires permettent de choisir quel flux vidéo consommer dans une source multi-flux et de remplacer la largeur/hauteur personnalisée.

## Travailler avec des segments audio

La même approche fonctionne pour les fichiers audio. Utilisez simplement AudioSource au lieu de VideoSource :

```cs
// Définir vos segments audio. FileSegment(startTime, stopTime) — stop doit être supérieur à start.
FileSegment[] audioSegments = new[] {
    new FileSegment(TimeSpan.FromMilliseconds(0),     TimeSpan.FromMilliseconds(8000)),   // 0 → 8s
    new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(27000))   // 15s → 27s
};

// Créer la source audio avec des segments.
// Signature : AudioSource(string filename, FileSegment[] segments, string fileToSync = null,
//   int streamNumber = 0, double rate = 1.0).
// La position 3 est fileToSync (une *string*), pas un facteur de vitesse — utilisez un arg nommé pour rate.
AudioSource audioFile = new AudioSource(
    audioFileName,
    audioSegments,
    rate: 1.0);

// Ajouter à la timeline.
VideoEdit1.Input_AddAudioFile(audioFile);
```

## Scénarios d'utilisation avancés

### Segments à vitesse variable

Vous pouvez créer des effets intéressants en variant le facteur de vitesse pour différents segments :

```cs
// Créer des segments avec des vitesses différentes. FileSegment(start, stop) — stop doit être > start.
VideoSource slowMotionSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(8000)) },  // 5s → 8s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    0.5);   // rate — demi-vitesse (ralenti)

VideoSource fastForwardSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(15000)) }, // 10s → 15s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    2.0);   // rate — double vitesse

// Ajouter les segments à la timeline. L'argument de position est TimeSpan? (point d'insertion), pas un int de piste.
VideoEdit1.Input_AddVideoFile(slowMotionSegment);
VideoEdit1.Input_AddVideoFile(fastForwardSegment, TimeSpan.FromMilliseconds(3000));
```

### Combiner plusieurs fichiers avec des segments

Vous pouvez combiner des segments de différents fichiers en créant plusieurs objets VideoSource :

```cs
// Créer des segments à partir de différents fichiers. FileSegment(start, stop) — stop doit être > start.
VideoSource file1Segments = new VideoSource(
    videoFileName1,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)) },  // 0 → 5s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    1.0);   // rate

VideoSource file2Segments = new VideoSource(
    videoFileName2,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(6000)) }, // 2s → 6s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    1.0);   // rate

// Ajouter à la timeline dans l'ordre. L'argument de position est TimeSpan?, pas un int.
VideoEdit1.Input_AddVideoFile(file1Segments);
VideoEdit1.Input_AddVideoFile(file2Segments, TimeSpan.FromMilliseconds(5000));
```

## Dépendances requises

Pour utiliser cette fonctionnalité, vous devrez installer les paquets redistribuables appropriés :

- Redistribuables Video Edit SDK :
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

Pour des informations sur l'installation ou le déploiement de ces redistribuables sur les PC de vos utilisateurs, consultez le [guide de déploiement](../deployment.md).

## Conclusion

Extraire et combiner plusieurs segments d'un fichier vidéo est une technique puissante pour créer du contenu vidéo dynamique dans vos applications. En suivant les étapes décrites dans ce guide, vous pouvez mettre en œuvre cette fonctionnalité dans vos applications C# avec un effort minimal.

Cette approche vous donne un contrôle fin sur les portions d'une vidéo qui sont incluses dans votre sortie finale, permettant des possibilités d'édition créatives sans nécessiter d'outils manuels complexes d'édition vidéo.

---
Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour plus d'exemples de code et d'exemples.