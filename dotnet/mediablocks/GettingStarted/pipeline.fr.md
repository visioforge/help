---
title: Pipeline multimédia et connexions de blocs en C# .NET
description: Pipelines multimédias pour lecture, enregistrement et streaming avec des blocs modulaires et gestion des ressources dans VisioForge Media Blocks.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Recording
  - Encoding
  - MP4
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - MediaBlockPadMediaType
  - IMediaBlock
  - MP4SinkBlock
  - MediaBlockType

---

# Pipeline Media Blocks : fonctionnalités principales

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble de la structure du pipeline et des blocs

Le Media Blocks SDK s'articule autour de la classe `MediaBlocksPipeline`, qui gère une collection de blocs de traitement modulaires. Chaque bloc implémente l'interface `IMediaBlock` ou l'une de ses variantes spécialisées. Les blocs sont connectés via des pads d'entrée et de sortie, ce qui permet de créer des chaînes de traitement multimédia flexibles.

### Principales interfaces de blocs

- **IMediaBlock** : interface de base pour tous les blocs. Expose les propriétés de nom, type, pads d'entrée/sortie et des méthodes pour la conversion YAML et la récupération du contexte du pipeline.
- **IMediaBlockDynamicInputs** : pour les blocs (comme les multiplexeurs) qui peuvent créer de nouvelles entrées dynamiquement. Méthodes : `CreateNewInput(mediaType)` et `GetInput(mediaType)`.
- **IMediaBlockInternals** : méthodes internes pour l'intégration au pipeline (par ex. `SetContext`, `Build`, `CleanUp`, `GetElement`, `GetCore`).
- **IMediaBlockInternals2** : pour la logique post-connexion (`PostConnect()`).
- **IMediaBlockRenderer** : pour les blocs moteurs de rendu, expose la propriété `IsSync`.
- **IMediaBlockSettings** : pour les objets de paramètres/configuration capables de créer un bloc (`CreateBlock()`).
- **IMediaBlockSink** : pour les blocs puits, expose un getter/setter de nom de fichier/URL.
- **IMediaBlockSource** : pour les blocs sources (actuellement uniquement des accesseurs de pads commentés).

### Pads et types de médias

- **MediaBlockPad** : représente un point de connexion (entrée/sortie) sur un bloc. Possède une direction (`In`/`Out`), un type de média (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`) et une logique de connexion.
- **Connexion de pads** : utilisez `pipeline.Connect(outputPad, inputPad)` ou `pipeline.Connect(block1.Output, block2.Input)`. Pour les entrées dynamiques, utilisez `CreateNewInput()` sur le bloc puits.

## Mise en place de votre environnement de pipeline

### Création d'une nouvelle instance de pipeline

La première étape pour travailler avec Media Blocks est d'instancier un objet pipeline :

```csharp
using VisioForge.Core.MediaBlocks;

// Créer une instance de pipeline standard
var pipeline = new MediaBlocksPipeline();

// Optionnellement, vous pouvez attribuer un nom à votre pipeline pour faciliter son identification
pipeline.Name = "MainVideoPlayer";
```

### Mise en œuvre d'une gestion d'erreurs robuste

Les applications multimédias doivent gérer divers scénarios d'erreur qui peuvent survenir pendant l'exécution. Mettre en place une gestion d'erreurs appropriée garantit la stabilité de votre application :

```csharp
// S'abonner aux événements d'erreur pour capturer et gérer les exceptions
pipeline.OnError += (sender, args) =>
{
    // Journaliser le message d'erreur
    Debug.WriteLine($"Erreur du pipeline : {args.Message}");
    
    // Mettre en œuvre une récupération d'erreur appropriée selon le message
    if (args.Message.Contains("Access denied"))
    {
        // Gérer les problèmes de permissions
    }
    else if (args.Message.Contains("File not found"))
    {
        // Gérer les erreurs de fichier manquant
    }
};
```

## Gestion du timing et de la navigation multimédia

### Récupération des informations de durée et de position

Un contrôle précis du timing est essentiel pour les applications multimédias :

```csharp
// Obtenir la durée totale du média (renvoie TimeSpan.Zero pour les flux en direct)
var duration = await pipeline.DurationAsync();
Console.WriteLine($"Durée du média : {duration.TotalSeconds} secondes");

// Obtenir la position de lecture courante
var position = await pipeline.Position_GetAsync();
Console.WriteLine($"Position actuelle : {position.TotalSeconds} secondes");
```

### Mise en œuvre de la fonctionnalité de recherche

Permettez à vos utilisateurs de naviguer dans le contenu multimédia avec des opérations de recherche :

```csharp
// Recherche basique à une position temporelle spécifique
await pipeline.Position_SetAsync(TimeSpan.FromSeconds(10));

// Recherche alignée sur une image clé pour une navigation plus efficace
await pipeline.Position_SetAsync(TimeSpan.FromMinutes(2), seekToKeyframe: true);

// Relire la position courante (par ex. pour une barre de progression)
var current = await pipeline.Position_GetAsync();
```

## Contrôle du flux d'exécution du pipeline

### Démarrage de la lecture multimédia

Contrôlez la lecture du média avec ces méthodes essentielles :

```csharp
// Démarrer la lecture immédiatement
await pipeline.StartAsync();

// Précharger le média sans démarrer la lecture (utile pour réduire le délai au démarrage)
await pipeline.StartAsync(onlyPreload: true);
await pipeline.ResumeAsync(); // Démarrer le pipeline préchargé quand vous êtes prêt
```

### Gestion des états de lecture

Surveillez et contrôlez l'état d'exécution actuel du pipeline :

```csharp
// Vérifier l'état actuel du pipeline
var state = pipeline.State;
if (state == PlaybackState.Play)
{
    Console.WriteLine("Le pipeline est en cours de lecture");
}

// S'abonner aux événements importants de changement d'état
pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("La lecture du pipeline a démarré");
    UpdateUIForPlaybackState();
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("La lecture du pipeline est arrêtée");
    Console.WriteLine($"Arrêté à la position : {args.Position.TotalSeconds} secondes");
    ResetPlaybackControls();
};

pipeline.OnPause += (sender, args) =>
{
    Console.WriteLine("La lecture du pipeline est en pause");
    UpdatePauseButtonState();
};

pipeline.OnResume += (sender, args) =>
{
    Console.WriteLine("La lecture du pipeline a repris");
    UpdatePlayButtonState();
};
```

### Mise en pause et reprise des opérations

Mettez en œuvre les fonctionnalités de pause et de reprise pour une meilleure expérience utilisateur :

```csharp
// Mettre en pause la lecture courante
await pipeline.PauseAsync();

// Reprendre la lecture depuis l'état de pause
await pipeline.ResumeAsync();
```

### Arrêt de l'exécution du pipeline

Terminez correctement les opérations du pipeline :

```csharp
// Opération d'arrêt standard
await pipeline.StopAsync();

// Arrêt forcé dans les scénarios sensibles au temps (peut affecter l'intégrité du fichier de sortie)
await pipeline.StopAsync(force: true);
```

## Construction de chaînes de traitement multimédia

### Connexion des blocs de traitement multimédia

La véritable puissance du Media Blocks SDK vient de la connexion de blocs spécialisés pour créer des chaînes de traitement :

```csharp
// Connexion basique entre deux blocs
pipeline.Connect(block1.Output, block2.Input);

// Connecter des blocs avec des types de médias spécifiques
pipeline.Connect(videoSource.GetOutputPadByType(MediaBlockPadMediaType.Video), 
                 videoEncoder.GetInputPadByType(MediaBlockPadMediaType.Video));
```

Différents blocs peuvent avoir plusieurs entrées et sorties spécialisées :

- E/S standard : propriétés `Input` et `Output`
- E/S spécifiques au média : `VideoOutput`, `AudioOutput`, `VideoInput`, `AudioInput`
- Tableaux d'E/S : `Inputs[]` et `Outputs[]` pour les blocs complexes

### Utilisation des blocs à entrées dynamiques

Certains blocs puits avancés créent des entrées à la demande :

```csharp
// Créer un multiplexeur MP4 spécialisé pour l'enregistrement — le constructeur prend le nom de fichier de sortie (ou MP4SinkSettings)
var mp4Muxer = new MP4SinkBlock("output_recording.mp4");

// Demander une nouvelle entrée vidéo au multiplexeur
var videoInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Video);

// Connecter une source vidéo à l'entrée nouvellement créée
pipeline.Connect(videoSource.Output, videoInput);

// De même pour l'audio
var audioInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Audio);
pipeline.Connect(audioSource.Output, audioInput);
```

Cette flexibilité permet des scénarios de traitement multimédia complexes avec plusieurs flux d'entrée.

## Gestion appropriée des ressources

### Libération des ressources du pipeline

Les applications multimédias peuvent consommer des ressources système significatives. Libérez toujours correctement les objets pipeline :

```csharp
// Modèle de libération synchrone
try
{
    // Utiliser le pipeline
}
finally
{
    pipeline.Dispose();
}
```

Pour les applications modernes, utilisez le modèle asynchrone afin d'éviter le gel de l'UI :

```csharp
// Libération asynchrone (à privilégier pour les applications UI)
try
{
    // Utiliser le pipeline
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Utilisation des instructions « using » pour un nettoyage automatique

Tirez parti des fonctionnalités du langage C# pour la gestion automatique des ressources :

```csharp
// Libération automatique avec l'instruction « using »
using (var pipeline = new MediaBlocksPipeline())
{
    // Configurer et utiliser le pipeline
    await pipeline.StartAsync();
    // Le pipeline sera automatiquement libéré à la sortie de ce bloc
}

// Déclaration « using » C# 8.0+
using var pipeline = new MediaBlocksPipeline();
// Le pipeline sera libéré à la sortie de la méthode englobante
```

## Fonctionnalités avancées du pipeline

### Contrôle de la vitesse de lecture

Ajustez la vitesse de lecture pour des effets de ralenti ou d'avance rapide :

```csharp
// Obtenir la vitesse de lecture courante
double currentRate = await pipeline.Rate_GetAsync();

// Définir la vitesse de lecture (1.0 = vitesse normale)
await pipeline.Rate_SetAsync(0.5);  // Ralenti (demi-vitesse)
await pipeline.Rate_SetAsync(2.0);  // Vitesse double
```

### Configuration de la lecture en boucle

Mettez en œuvre une lecture en continu :

```csharp
// Activer la boucle pour une lecture continue
pipeline.Loop = true;

// Écouter les événements de boucle
pipeline.OnLoop += (sender, args) =>
{
    Console.WriteLine("Le média a bouclé au début");
    UpdateLoopCounter();
};
```

### Mode débogage pour le développement

Activez les fonctionnalités de débogage pendant le développement :

```csharp
// Activer le mode débogage pour une journalisation plus détaillée
pipeline.Debug_Mode = true;
pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.MyDocuments), "PipelineDebugLogs");
```

## Référence des types de blocs

Le SDK fournit un large éventail de types de blocs pour les sources, le traitement et les puits. Consultez l'enum `MediaBlockType` dans le code source pour la liste complète des types de blocs disponibles.

## Notes

- Le pipeline prend en charge à la fois des méthodes synchrones et asynchrones pour démarrer, arrêter et libérer. Privilégiez les méthodes asynchrones dans les applications UI ou de longue durée.
- Des événements sont disponibles pour la gestion d'erreurs, les changements d'état et les informations de flux.
- Utilisez la bonne interface pour chaque type de bloc afin d'accéder aux fonctionnalités spécialisées (par ex. entrées dynamiques, rendu, paramètres).
