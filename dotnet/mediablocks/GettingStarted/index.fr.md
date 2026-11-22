---
title: Démarrage rapide du pipeline multimédia en C# .NET
description: Démarrez avec VisioForge Media Blocks SDK — installation, architecture du pipeline, connexion de blocs et tutoriels de traitement multimédia.
sidebar_label: Prise en main
order: 20
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - MediaBlocksPipeline
  - IMediaBlock
  - MediaBlockPad
  - IMediaBlocksPipeline
  - MediaBlock
  - SystemVideoSourceBlock
  - VideoRendererBlock
  - DeviceEnumerator

---

# Media Blocks SDK .NET - Guide de démarrage rapide pour développeurs

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Ce guide propose un parcours complet pour intégrer le Media Blocks SDK .NET dans vos applications. Le SDK s'articule autour d'une architecture de pipeline modulaire qui vous permet de créer, connecter et gérer des blocs de traitement multimédia pour la vidéo, l'audio et plus encore. Que vous construisiez des outils de traitement vidéo, des solutions de streaming ou des applications multimédias, ce guide vous aidera à démarrer rapidement et correctement.

## Processus d'installation du SDK

Le SDK est distribué sous forme de paquet NuGet pour une intégration facile dans vos projets .NET. Installez-le avec :

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

Ce paquet ne contient que le SDK managé. Le moteur natif — GStreamer et son jeu de plugins — est fourni dans un paquet redistribuable distinct que vous ajoutez à côté. Une application qui ne référence que le paquet managé se compile, puis s'arrête au démarrage : `VisioForgeX.InitSDKAsync` lève `DllNotFoundException` avant l'ouverture de la moindre fenêtre.

Pour une application Windows x64 :

```bash
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

Le premier contient le moteur ; toute application en a besoin. Le second ajoute les décodeurs et encodeurs FFmpeg (libav) et est recommandé pour toute application.

Utilisez l'architecture du processus en cours d'exécution : `.x86` pour une compilation 32 bits et `.arm64` pour une compilation Windows on Arm (NDI n'y est pas disponible) ; un redistribuable d'une autre architecture ne se charge pas dans le processus. Une application AnyCPU s'exécute généralement en x64 sur un Windows 64 bits, et en ARM64 sur Windows on Arm.

Un paquet redistribuable doit être la version la plus récente à la date de votre version de `VisioForge.DotNet.MediaBlocks` ou avant : jamais postérieure, jamais très en retard. Épinglez-le avec `--version` si nécessaire : `dotnet add package VisioForge.CrossPlatform.Core.Windows.x64 --version <version>`.

Les autres plateformes utilisent leur propre paquet redistribuable :

- **macOS, Mac Catalyst** - `VisioForge.CrossPlatform.Core.macOS` ; Mac Catalyst a en plus besoin d'une cible de build qui copie les bibliothèques natives dans le bundle de l'application ([déploiement macOS](../../deployment-x/macOS.md))
- **iOS** - `VisioForge.CrossPlatform.Core.iOS` ([déploiement iOS](../../deployment-x/iOS.md))
- **Android** - `VisioForge.CrossPlatform.Core.Android`, et la bibliothèque de bindings Java ([déploiement Android](../../deployment-x/Android.md))
- **Linux** - `VisioForge.CrossPlatform.Core.Linux.x64` ou `.arm64`, et GStreamer 1.20 ou ultérieur depuis la distribution ([déploiement Ubuntu](../../deployment-x/Ubuntu.md))

Pour les paquets nécessaires, consultez le [guide d'installation détaillé](../../install/index.md) ; les paquets optionnels sont listés dans le [guide de déploiement](../../deployment-x/index.md).

## Votre premier pipeline

Le pipeline utile le plus simple comporte deux blocs : une source et un moteur de rendu. Celui-ci affiche la première caméra détectée sur la machine :

```csharp
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

// À appeler depuis le thread d'interface une fois la fenêtre créée.
async Task<MediaBlocksPipeline> StartCameraAsync(IVideoView videoView)
{
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];

    var pipeline = new MediaBlocksPipeline();
    var source = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(device));
    var renderer = new VideoRendererBlock(pipeline, videoView);

    pipeline.AddBlock(source);
    pipeline.AddBlock(renderer);
    pipeline.Connect(source.Output, renderer.Input);

    await pipeline.StartAsync();

    return pipeline;
}
```

- Le SDK doit d'abord être initialisé avec `await VisioForgeX.InitSDKAsync();` — voir l'étape 1 ci-dessous.
- Aucune caméra connectée ? Omettez la ligne `device` et utilisez la mire : `var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());` à la place de `SystemVideoSourceBlock`. Les lignes de connexion et de démarrage sont identiques.
- Conservez le pipeline renvoyé dans un champ et arrêtez-le à la fermeture de la fenêtre : `await pipeline.StopAsync(); pipeline.Dispose();`.
- Sélection du périphérique et du format, captures et particularités par plateforme : [Tutoriel d'application de visualisation de caméra](camera.md).

## Concepts principaux et architecture

### MediaBlocksPipeline

- La classe centrale pour gérer le flux des données multimédias entre les blocs de traitement.
- Gère l'ajout de blocs, les connexions, la gestion d'état et la gestion d'événements.
- Implémente `IMediaBlocksPipeline` et expose des événements comme `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop` et `OnLoop`.

### MediaBlock et interfaces

- Chaque unité de traitement est un `MediaBlock` (ou une classe dérivée), implémentant l'interface `IMediaBlock`.
- Interfaces clés :
  - `IMediaBlock` : interface de base pour tous les blocs. Définit les propriétés `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs` ainsi qu'une méthode pour le contexte de pipeline du bloc.
  - `IMediaBlockDynamicInputs` : pour les blocs qui prennent en charge la création dynamique d'entrées (par ex. les mélangeurs).
  - `IMediaBlockRenderer` : pour les blocs qui font le rendu du média (par ex. moteurs de rendu vidéo/audio), avec une propriété pour contrôler la synchronisation des flux.
  - `IMediaBlockSettings` : pour les objets de paramètres capables de créer des blocs.

### Pads et types de médias

- Les blocs sont connectés via des objets `MediaBlockPad`, qui possèdent une direction (`In`/`Out`) et un type de média (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Les pads peuvent être connectés/déconnectés, et leur état peut être interrogé.

### Types de blocs

- Le SDK fournit un large éventail de types de blocs intégrés (répertoriés dans l'énumération `MediaBlockType`) pour les sources, les puits, les moteurs de rendu, les effets, etc.
- Catalogue des blocs : [Sources](../Sources/index.md), [Rendu vidéo](../VideoRendering/index.md), [Puits](../Sinks/index.md), [Traitement vidéo](../VideoProcessing/index.md).

## Création et gestion d'un pipeline

### 1. Initialiser le SDK

```csharp
using VisioForge.Core;

// Initialiser le SDK au démarrage de l'application (await rend la main une fois les bibliothèques natives chargées)
await VisioForgeX.InitSDKAsync();
```

### 2. Créer un pipeline et des blocs

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types.X.Sources;

// Créer une nouvelle instance de pipeline
var pipeline = new MediaBlocksPipeline();

// Exemple : créer une source vidéo virtuelle et un moteur de rendu vidéo
var virtualSource = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // VideoView1 est votre contrôle UI

// Ajouter les blocs au pipeline
pipeline.AddBlock(virtualSource);
pipeline.AddBlock(videoRenderer);
```

`VideoView1` est le contrôle `VisioForge.Core.UI.WPF.VideoView` d'une application WPF. Déclarez-le en XAML avec `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` et `<WPF:VideoView x:Name="VideoView1" />`. Windows Forms utilise `VisioForge.Core.UI.WinForms.VideoView` du même assembly ; MAUI, Avalonia, WinUI et Uno fournissent leur propre `VideoView` dans leurs paquets d'UI.

### 3. Connecter les blocs

```csharp
// Connecter la sortie de la source à l'entrée du moteur de rendu
pipeline.Connect(virtualSource.Output, videoRenderer.Input);
```

- Vous pouvez aussi utiliser `pipeline.Connect(sourceBlock, targetBlock)` pour connecter les pads par défaut, ou connecter plusieurs pads pour des graphes complexes.
- Pour les blocs prenant en charge les entrées dynamiques, utilisez l'interface `IMediaBlockDynamicInputs`.

### 4. Démarrer et arrêter le pipeline

```csharp
// Démarrer le pipeline de manière asynchrone
await pipeline.StartAsync();

// ... plus tard, arrêter le traitement
await pipeline.StopAsync();
```

### 5. Nettoyage des ressources

```csharp
// Libérer le pipeline lorsque vous avez terminé
pipeline.Dispose();
```

### 6. Nettoyage du SDK

```csharp
// Libérer toutes les ressources du SDK à la fermeture de l'application
VisioForgeX.DestroySDK();
```

## Gestion des erreurs et événements

- Abonnez-vous aux événements du pipeline pour une gestion robuste des erreurs et de l'état :

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Erreur du pipeline : {args.Message}");
    // Implémentez ici votre logique de gestion d'erreurs
};

pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("Pipeline démarré");
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("Pipeline arrêté");
};
```

## Fonctionnalités avancées

- **Ajout/suppression dynamique de blocs :** vous pouvez ajouter ou supprimer des blocs à l'exécution selon vos besoins.
- **Gestion des pads :** utilisez les méthodes de `MediaBlockPad` pour interroger et gérer les connexions de pads.
- **Sélection de décodeur matériel/logiciel :** utilisez les méthodes utilitaires de `MediaBlocksPipeline` pour l'accélération matérielle.
- **Lecture par segment :** définissez les propriétés `StartPosition` et `StopPosition` pour une lecture partielle.
- **Débogage :** exportez les graphes de pipeline pour le débogage à l'aide des méthodes fournies.

## Lectures et exemples complémentaires

- [Implémentation complète d'un pipeline](pipeline.md)
- [Guide de développement d'un lecteur multimédia](player.md)
- [Tutoriel d'application de visualisation de caméra](camera.md)
- [Dépôt GitHub avec des exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

Pour chaque propriété et valeur par défaut d'un bloc, consultez la [référence d'API](https://api.visioforge.org/dotnet/api/index.html).
