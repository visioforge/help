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

---

# Media Blocks SDK .Net - Guide de démarrage rapide pour développeurs

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Ce guide propose un parcours complet pour intégrer le Media Blocks SDK .Net dans vos applications. Le SDK s'articule autour d'une architecture de pipeline modulaire qui vous permet de créer, connecter et gérer des blocs de traitement multimédia pour la vidéo, l'audio et plus encore. Que vous construisiez des outils de traitement vidéo, des solutions de streaming ou des applications multimédias, ce guide vous aidera à démarrer rapidement et correctement.

## Processus d'installation du SDK

Le SDK est distribué sous forme de paquet NuGet pour une intégration facile dans vos projets .Net. Installez-le avec :

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

Pour les exigences spécifiques à chaque plateforme et des détails d'installation supplémentaires, consultez le [guide d'installation détaillé](../../install/index.md).

## Concepts principaux et architecture

### MediaBlocksPipeline

- La classe centrale pour gérer le flux des données multimédias entre les blocs de traitement.
- Gère l'ajout de blocs, les connexions, la gestion d'état et la gestion d'événements.
- Implémente `IMediaBlocksPipeline` et expose des événements comme `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop` et `OnLoop`.

### MediaBlock et interfaces

- Chaque unité de traitement est un `MediaBlock` (ou une classe dérivée), implémentant l'interface `IMediaBlock`.
- Interfaces clés :
  - `IMediaBlock` : interface de base pour tous les blocs. Définit les propriétés `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs` ainsi que des méthodes pour le contexte de pipeline et l'export YAML.
  - `IMediaBlockDynamicInputs` : pour les blocs qui prennent en charge la création dynamique d'entrées (par ex. les mélangeurs).
  - `IMediaBlockInternals`/`IMediaBlockInternals2` : pour la gestion interne du pipeline, la construction et la logique post-connexion.
  - `IMediaBlockRenderer` : pour les blocs qui font le rendu du média (par ex. moteurs de rendu vidéo/audio), avec une propriété pour contrôler la synchronisation des flux.
  - `IMediaBlockSink`/`IMediaBlockSource` : pour les blocs qui agissent comme puits (sorties) ou sources (entrées).
  - `IMediaBlockSettings` : pour les objets de paramètres capables de créer des blocs.

### Pads et types de médias

- Les blocs sont connectés via des objets `MediaBlockPad`, qui possèdent une direction (`In`/`Out`) et un type de média (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Les pads peuvent être connectés/déconnectés, et leur état peut être interrogé.

### Types de blocs

- Le SDK fournit un large éventail de types de blocs intégrés (voir l'enum `MediaBlockType` dans le code source) pour les sources, puits, moteurs de rendu, effets, et plus.

## Création et gestion d'un pipeline

### 1. Initialiser le SDK (si nécessaire)

```csharp
using VisioForge.Core;

// Initialiser le SDK au démarrage de l'application
VisioForgeX.InitSDK();
```

### 2. Créer un pipeline et des blocs

```csharp
using VisioForge.Core.MediaBlocks;

// Créer une nouvelle instance de pipeline
var pipeline = new MediaBlocksPipeline();

// Exemple : créer une source vidéo virtuelle et un moteur de rendu vidéo
var virtualSource = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // VideoView1 est votre contrôle UI

// Ajouter les blocs au pipeline
pipeline.AddBlock(virtualSource);
pipeline.AddBlock(videoRenderer);
```

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

### 6. Nettoyage du SDK (si nécessaire)

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

## Exemple : configuration minimale d'un pipeline

```csharp
using VisioForge.Core.MediaBlocks;

var pipeline = new MediaBlocksPipeline();
var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var renderer = new VideoRendererBlock(pipeline, videoViewControl);

pipeline.AddBlock(source);
pipeline.AddBlock(renderer);
pipeline.Connect(source.Output, renderer.Input);
await pipeline.StartAsync();
// ...
await pipeline.StopAsync();
pipeline.Dispose();
```

## Référence : interfaces clés

- `IMediaBlock` : interface de base pour tous les blocs.
- `IMediaBlockDynamicInputs` : pour les blocs avec prise en charge d'entrées dynamiques.
- `IMediaBlockInternals`, `IMediaBlockInternals2` : pour la logique interne du pipeline.
- `IMediaBlockRenderer` : pour les blocs moteurs de rendu.
- `IMediaBlockSink`, `IMediaBlockSource` : pour les blocs puits/sources.
- `IMediaBlockSettings` : pour les objets de paramètres de blocs.
- `IMediaBlocksPipeline` : interface principale du pipeline.
- `MediaBlockPad`, `MediaBlockPadDirection`, `MediaBlockPadMediaType` : pour la gestion des pads.

## Lectures et exemples complémentaires

- [Implémentation complète d'un pipeline](pipeline.md)
- [Guide de développement d'un lecteur multimédia](player.md)
- [Tutoriel d'application de visualisation de caméra](camera.md)
- [Dépôt GitHub avec des exemples de code](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

Pour une liste complète des types de blocs et une utilisation avancée, consultez la référence d'API du SDK et le code source.
