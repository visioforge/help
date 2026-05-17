---
title: Édition vidéo — timeline, transitions, superpositions C#
description: Créez des applications d'édition vidéo avec VisioForge Video Edit SDK .NET. Timeline, transitions, superpositions, conversion et encodage accéléré GPU.
sidebar_label: Video Edit SDK .NET
order: 12
tags:
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
primary_api_classes:
  - VideoEditCoreX
  - MP4Output

---

# Video Edit SDK pour C# .NET — API d'édition vidéo par timeline

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Video Edit SDK pour .NET est une bibliothèque C# d'édition vidéo qui vous permet de créer des applications d'édition vidéo basées sur une timeline. Ajoutez des fichiers vidéo et audio à une timeline, rognez des segments, appliquez des transitions et des effets, superposez du texte et des images, et générez le résultat au format MP4, AVI, MKV, WebM, GIF animé ou d'autres formats — le tout depuis votre code .NET.

Le SDK fournit deux moteurs : **VideoEditCore** (Windows uniquement, basé sur DirectShow) et **VideoEditCoreX** (multiplateforme, fonctionne sur Windows, macOS, Linux, Android et iOS). Les deux moteurs partagent le même modèle de timeline — ajouter des sources, configurer la sortie et commencer l'édition.

## Démarrage rapide

### 1. Installer le paquet NuGet

```bash
dotnet add package VisioForge.DotNet.VideoEdit
```

Pour les dépendances spécifiques à chaque plateforme et la configuration des frameworks d'interface, consultez le [Guide d'installation](../install/index.md).

### 2. Exemple minimal d'édition vidéo

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// Initialiser le SDK
VisioForgeX.InitSDK();

// Créer l'éditeur avec un aperçu vidéo
var editor = new VideoEditCoreX(videoView);

// Définir la résolution de sortie et la fréquence d'images
editor.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// Ajouter des fichiers vidéo à la timeline
editor.Input_AddAudioVideoFile("intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("main.mp4", null, null, null);

// Définir le format de sortie
editor.Output_Format = new MP4Output("output.mp4");

// Démarrer l'édition
editor.Start();

// ... une fois terminé :
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK();
```

Pour le guide d'implémentation complet avec gestion d'événements, sources audio, sources d'image et configuration avancée de la timeline, consultez le [Guide de prise en main](getting-started.md).

## Cas d'usage courants

### Combiner plusieurs fichiers vidéo

Fusionnez plusieurs clips vidéo dans un seul fichier de sortie. Ajoutez les fichiers à la timeline dans l'ordre, définissez le format de sortie et lancez le rendu. Prend en charge le mélange de formats source différents — combinez des fichiers MP4, AVI et MOV dans une seule sortie.

Voir : [Créer des vidéos à partir de plusieurs sources](code-samples/output-file-from-multiple-sources.md)

### Rogner et couper des segments vidéo

Extrayez des plages temporelles spécifiques de fichiers vidéo en définissant les temps de début et de fin sur chaque source. Combinez plusieurs segments du même fichier ou de fichiers différents en un montage final.

Voir : [Travailler avec des segments vidéo](code-samples/several-segments.md)

### Ajouter des superpositions de texte et d'image

Insérez des titres, sous-titres, logos et filigranes par-dessus le contenu vidéo. Positionnez, mettez à l'échelle et temporisez les superpositions sur la timeline.

Voir : [Mise en œuvre d'une superposition de texte](code-samples/add-text-overlay.md) | [Ajout de superpositions d'image](code-samples/add-image-overlay.md)

### Appliquer des transitions entre clips

Ajoutez des fondus enchaînés, balayages, glissements, fondus et plus de 100 transitions standard SMPTE entre segments vidéo. Contrôlez la durée, le style de bordure et la direction de la transition.

Voir : [Effets de transition entre fragments vidéo](code-samples/transition-video.md) | [Référence des transitions](transitions.md)

### Créer un diaporama à partir d'images

Construisez des diaporamas vidéo à partir d'images JPG, PNG, BMP et GIF avec une durée d'affichage configurable par image, des transitions entre diapositives et une musique d'arrière-plan.

Voir : [Générer des vidéos à partir de séquences d'images](code-samples/video-images-console.md)

### Ajouter de la musique d'arrière-plan et mélanger l'audio

Mélangez plusieurs pistes audio avec le contenu vidéo. Contrôlez le volume par piste, appliquez des effets d'enveloppe audio pour les fondus en entrée/sortie et synchronisez l'audio avec la vidéo.

Voir : [Effets d'enveloppe de volume audio](code-samples/audio-envelope.md) | [Contrôle de volume personnalisé](code-samples/volume-for-track.md)

### Composition image dans l'image

Superposez plusieurs sources vidéo avec contrôle de position et de taille pour des mises en page image dans l'image, des vidéos de réaction ou des compositions multi-caméras.

Voir : [Effets image dans l'image](code-samples/picture-in-picture.md)

## Formats pris en charge

| Catégorie | Formats |
| -------- | ------- |
| Conteneurs vidéo | MP4, AVI, MOV, WMV, MKV, WebM, TS, FLV |
| Codecs vidéo | H.264, H.265/HEVC, VP9, AV1, MPEG-2 |
| Formats audio | AAC, MP3, WMA, OPUS, Vorbis, FLAC, WAV |
| Formats d'image | JPG, PNG, BMP, GIF (entrée), GIF animé (sortie) |

## Prise en charge des plateformes

| Plateforme | Frameworks d'interface | Moteur | Notes |
| -------- | ------------- | ------ | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | VideoEditCore, VideoEditCoreX | Ensemble complet de fonctionnalités, y compris les ponts DirectShow |
| macOS | MAUI, Avalonia, Console | VideoEditCoreX | Intel et Apple Silicon |
| Linux x64 | Avalonia, Console | VideoEditCoreX | Ubuntu, Debian, CentOS |
| Android | MAUI | VideoEditCoreX | Via l'intégration MAUI |
| iOS | MAUI | VideoEditCoreX | Via l'intégration MAUI |

## Documentation développeur

### Guides

* [Guide de prise en main](getting-started.md) — Tutoriel d'implémentation complet avec les deux moteurs
* [Exemples de code](code-samples/index.md) — Exemples prêts à l'emploi pour les superpositions, transitions, audio et composition
* [Guide de déploiement](deployment.md) — Paquets NuGet, programmes d'installation et installation manuelle
* [Référence des transitions](transitions.md) — Plus de 100 codes de transition SMPTE et propriétés

### iOS

* [Éditeur vidéo iOS](code-samples/ios-video-editor.md) — Création d'applications d'édition vidéo pour iPhone et iPad

## Ressources développeur

* [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples)
* [Référence de l'API](https://api.visioforge.org/dotnet/api/index.html)
* [Journal des modifications](../changelog.md)
* [Contrat de licence utilisateur final](../../eula.md)
* [Informations de licence](../../../licensing.md)
