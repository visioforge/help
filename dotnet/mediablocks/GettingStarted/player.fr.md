---
title: Construire un lecteur vidéo en C# .NET — Guide pas à pas
description: Tutoriel C# pour construire un lecteur vidéo avec VisioForge Media Blocks SDK — blocs source, rendu audio/vidéo et contrôles de lecture.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceSettings
  - UniversalSourceBlock
  - VideoRendererBlock
  - DeviceEnumerator

---

# Construire un lecteur vidéo riche en fonctionnalités avec Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce tutoriel détaillé vous accompagne dans le processus de création d'une application de lecteur vidéo de niveau professionnel à l'aide du Media Blocks SDK .Net. En suivant ces instructions, vous comprendrez comment mettre en œuvre des fonctionnalités clés telles que le chargement de médias, le contrôle de la lecture et le rendu audio-vidéo.

## Composants essentiels pour votre application de lecteur

Pour construire un lecteur vidéo entièrement fonctionnel, le pipeline de votre application nécessite ces briques essentielles :

- [Source universelle](../Sources/index.md) — ce composant polyvalent gère l'entrée multimédia provenant de diverses sources, permettant à votre lecteur de lire et traiter des fichiers vidéo depuis un stockage local ou des flux réseau.
- [Moteur de rendu vidéo](../VideoRendering/index.md) — le composant visuel responsable de l'affichage des images vidéo à l'écran avec un timing et un formatage corrects.
- [Moteur de rendu audio](../AudioRendering/index.md) — gère la sortie sonore, garantissant une lecture audio synchronisée avec votre contenu vidéo.

## Mise en place du pipeline multimédia

### Création des fondations

La première étape du développement de votre lecteur consiste à établir le pipeline multimédia — le framework central qui gère le flux de données entre les composants.

```csharp
using VisioForge.Core.MediaBlocks;

var pipeline = new MediaBlocksPipeline();
```

### Mise en œuvre de la gestion d'erreurs

Une gestion d'erreurs robuste est essentielle pour une application de lecteur fiable. Abonnez-vous aux événements d'erreur du pipeline pour capturer et réagir aux exceptions.

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine(args.Message);
    // Une logique supplémentaire de gestion d'erreurs peut être implémentée ici
};
```

### Mise en place des écouteurs d'événements

Pour un contrôle complet du cycle de vie de votre lecteur, implémentez des gestionnaires d'événements pour les changements d'état critiques :

```csharp
pipeline.OnStart += (sender, args) => 
{
    // Exécuter du code lorsque le pipeline démarre
    Console.WriteLine("Lecture démarrée");
};

pipeline.OnStop += (sender, args) => 
{
    // Exécuter du code lorsque le pipeline s'arrête
    Console.WriteLine("Lecture arrêtée");
};
```

## Configuration des blocs multimédias

### Initialisation du bloc source

Le bloc Universal Source sert de point d'entrée pour le contenu multimédia. Configurez-le avec le chemin de votre fichier multimédia :

```csharp
var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri(filePath));
var fileSource = new UniversalSourceBlock(sourceSettings);
```

Pendant l'initialisation, le SDK analyse automatiquement le fichier pour extraire des métadonnées essentielles sur les flux vidéo et audio, permettant une configuration correcte des composants en aval.

### Mise en place de l'affichage vidéo

Pour faire le rendu du contenu vidéo à l'écran, créez et configurez un bloc moteur de rendu vidéo :

```csharp
var videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
```

Le moteur de rendu nécessite deux paramètres : une référence à votre pipeline et le contrôle d'UI sur lequel les images vidéo seront affichées.

### Configuration de la sortie audio

Pour la lecture audio, vous devrez sélectionner et initialiser un périphérique de sortie audio approprié :

```csharp
var audioRenderers = await DeviceEnumerator.Shared.AudioOutputsAsync();
var audioRenderer = new AudioRendererBlock(audioRenderers[0]);
```

Ce code récupère les périphériques de sortie audio disponibles et configure la première option disponible pour la lecture.

## Établissement des connexions entre composants

Une fois tous les blocs configurés, vous devez établir les connexions entre eux pour créer un flux multimédia cohérent :

```csharp
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);
```

Ces connexions définissent le chemin que prennent les données à travers votre application :

- Les données vidéo circulent de la source vers le moteur de rendu vidéo
- Les données audio circulent de la source vers le moteur de rendu audio

Pour des fichiers contenant uniquement de la vidéo ou de l'audio, vous pouvez ne connecter sélectivement que les sorties pertinentes.

### Validation du contenu multimédia

Avant la lecture, vous pouvez inspecter les flux disponibles à l'aide d'Universal Source Settings :

```csharp
var mediaInfo = await sourceSettings.ReadInfoAsync();
bool hasVideo = mediaInfo.VideoStreams.Count > 0;
bool hasAudio = mediaInfo.AudioStreams.Count > 0;
```

## Contrôle de la lecture multimédia

### Démarrage de la lecture

Pour démarrer la lecture multimédia, appelez la méthode asynchrone de démarrage du pipeline :

```csharp
await pipeline.StartAsync();
```

Une fois exécutée, votre application commencera le rendu des images vidéo et la lecture audio via les sorties configurées.

### Gestion de l'état de lecture

Pour arrêter la lecture, invoquez la méthode d'arrêt du pipeline :

```csharp
await pipeline.StopAsync();
```

Cela termine gracieusement tout le traitement multimédia et libère les ressources associées.

## Implémentation avancée

Pour un exemple d'implémentation complet avec des fonctionnalités supplémentaires comme la recherche, le contrôle du volume et la prise en charge du plein écran, consultez notre code source complet sur [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Player%20Demo%20WPF).

Le dépôt contient des démonstrations fonctionnelles pour diverses plateformes, notamment WPF, Windows Forms et des applications .NET multiplateformes.

## Voir aussi

- [Lecteur vidéo C# (WinForms / WPF)](../../mediaplayer/guides/video-player-csharp.md) — l'alternative Media Player SDK avec une configuration en une ligne pour les applications de bureau Windows
- [Lecteur multiplateforme Avalonia](../../mediaplayer/guides/avalonia-player.md) — la même approche pipeline pour les cibles de bureau Windows, macOS et Linux
- [Lecteur .NET MAUI](../../mediaplayer/guides/maui-player.md) — mobile + bureau depuis un seul code source (iOS, Android, macOS, Windows)
