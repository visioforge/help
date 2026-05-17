---
title: API de lecteur vidéo C# .NET — Lire, streamer, intégrer
description: Intégrez la lecture vidéo et audio dans votre app avec VisioForge Media Player SDK .NET. MP4, MKV, RTSP, HLS sur Windows, macOS, Linux et mobile.
sidebar_label: Media Player SDK .NET
order: 13
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming
primary_api_classes:
  - UniversalSourceSettings
  - MediaPlayerCoreX
  - VideoView

---

# Media Player SDK pour C# .NET — Lecteur vidéo, lecteur audio et API de streaming

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Le Media Player SDK pour .NET est une API de lecteur vidéo en C# qui vous permet de lire des fichiers vidéo et audio, des flux réseau (RTSP, HLS, MPEG-DASH) et des contenus spéciaux comme les vidéos à 360 degrés dans vos applications .NET. Il remplace le code de lecture DirectShow bas niveau et l'intégration du SDK Windows Media Player par une API async moderne — ouvrez un fichier, démarrez la lecture, et contrôlez le positionnement et le volume en quelques lignes de C#.

Le SDK utilise un décodage basé sur GStreamer avec accélération matérielle et s'exécute sur Windows, macOS, Linux, Android et iOS. Que vous deviez créer un lecteur vidéo de bureau, intégrer la lecture multimédia dans une application kiosque, ou diffuser des flux RTSP depuis des caméras IP, l'API couvre tout cela.

## Démarrage rapide

### 1. Installer les paquets NuGet

```bash
dotnet add package VisioForge.DotNet.MediaPlayer
```

Ajoutez les dépendances natives spécifiques à la plateforme :

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

Pour la liste complète des paquets et la prise en charge des frameworks UI (WinForms, WPF, MAUI, Avalonia), consultez le [Guide d'installation](../install/index.md).

### 2. Initialiser le SDK

Appelez `InitSDKAsync()` une seule fois au démarrage de l'application avant d'utiliser toute fonctionnalité de lecture :

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Lire une vidéo en C# (exemple minimal)

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Créer une instance du lecteur liée à un contrôle VideoView
var player = new MediaPlayerCoreX(videoView);

// Ouvrir un fichier vidéo
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("C:\\Videos\\sample.mp4"));
await player.OpenAsync(source);

// Démarrer la lecture
await player.PlayAsync();

// ... une fois terminé :
await player.StopAsync();
await player.DisposeAsync();
```

### 4. Nettoyage à l'arrêt

```csharp
VisioForgeX.DestroySDK();
```

## Flux de travail principal

Chaque application de lecteur multimédia suit le même schéma :

1. **Initialiser le SDK** — `VisioForgeX.InitSDKAsync()` (une fois par durée de vie de l'application)
2. **Créer le lecteur** — `new MediaPlayerCoreX(videoView)` pour la vidéo, ou sans vue pour de l'audio seul
3. **Ouvrir le média** — `await player.OpenAsync(source)` avec un chemin de fichier ou une URL de flux
4. **Lire** — `await player.PlayAsync()`
5. **Contrôler la lecture** — se positionner avec `Position_SetAsync()`, mettre en pause avec `PauseAsync()`, ajuster le volume
6. **Arrêter** — `await player.StopAsync()`
7. **Libérer** — `await player.DisposeAsync()` libère toutes les ressources
8. **Détruire le SDK** — `VisioForgeX.DestroySDK()` à l'arrêt de l'application

## Scénarios courants de lecteur vidéo C#

### Lire un fichier vidéo en C# (MP4, AVI, MKV)

Ouvrez et lisez n'importe quel fichier vidéo pris en charge avec détection automatique du codec :

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("movie.mp4"));
await player.OpenAsync(source);
await player.PlayAsync();
```

Voir le tutoriel complet : [Construire un lecteur vidéo en C#](guides/video-player-csharp.md)

### Lire un flux RTSP en C#

Lisez de la vidéo en direct depuis des caméras IP et des sources RTSP. Prend en charge RTSP, HTTP, HLS et MPEG-DASH :

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("rtsp://admin:password@192.168.1.100:554/stream"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Lecture audio seule

Lisez des fichiers audio (MP3, AAC, FLAC, WAV) sans vue vidéo :

```csharp
var player = new MediaPlayerCoreX(); // pas de VideoView nécessaire
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("music.mp3"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Extraire une image depuis la vidéo

Capturez une image fixe à partir de la position de lecture actuelle :

```csharp
// Enregistrer l'image actuelle directement sur le disque — la voie la plus simple.
bool saved = await player.Snapshot_SaveAsync("screenshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, quality: 85);

// Ou récupérer le VideoFrameX brut pour traitement en mémoire (l'appelant possède le tampon de pixels).
var frame = await player.Snapshot_GetAsync();
if (frame != null)
{
    // frame.Data (IntPtr), frame.DataSize, frame.Width, frame.Height, frame.Stride, frame.Format.
    // Libérer le tampon non managé une fois terminé :
    frame.Free();
}
```

Voir : [Obtenir une image spécifique depuis un fichier vidéo](code-samples/get-frame-from-video-file.md)

### Positionnement, volume et contrôle de vitesse

```csharp
// Se positionner à un instant précis
await player.Position_SetAsync(TimeSpan.FromSeconds(30));

// Obtenir la position actuelle et la durée
var position = await player.Position_GetAsync();
var duration = await player.DurationAsync();

// Ajuster le volume (0.0 à 1.0)
player.Audio_OutputDevice_Volume = 0.8;

// Changer la vitesse de lecture (0.25x à 4.0x)
await player.Rate_SetAsync(1.5);
```

### Lecture en boucle et lecture par segments

Lisez un segment spécifique d'un fichier ou bouclez en continu :

```csharp
// Activer le mode boucle
player.Loop = true;

// Lire une plage de temps spécifique
player.Segment_Start = TimeSpan.FromSeconds(10);
player.Segment_Stop = TimeSpan.FromSeconds(60);
```

Voir : [Mode boucle et lecture par plage de position](guides/loop-and-position-range.md)

## Formats pris en charge

| Catégorie | Formats |
| --------- | ------- |
| Conteneurs vidéo | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Formats audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs vidéo | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Protocoles de streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Prise en charge des plateformes

| Plateforme | Frameworks UI | Notes |
| ---------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, WinUI, MAUI, Avalonia, Console | Ensemble complet de fonctionnalités, y compris le moteur DirectShow |
| macOS | MAUI, Avalonia, Console | Rendu basé sur AVFoundation |
| Linux x64 | Avalonia, Console | Décodage basé sur GStreamer |
| Android | MAUI | Via l'intégration MAUI |
| iOS | MAUI | Via l'intégration MAUI |

Pour les implémentations multiplateformes, consultez le [Guide du lecteur Avalonia](guides/avalonia-player.md) (bureau, inclut Linux) ou le [Guide du lecteur .NET MAUI](guides/maui-player.md) (mobile + bureau).

## Documentation pour les développeurs

### Guides

* [Construire un lecteur vidéo en C# (WinForms / WPF)](guides/video-player-csharp.md)
* [Construire un lecteur vidéo en VB.NET](guides/video-player-vb-net.md)
* [Implémentation du lecteur Avalonia](guides/avalonia-player.md)
* [Lecteur .NET MAUI](guides/maui-player.md)
* [Implémentation du lecteur Android](guides/android-player.md)
* [Mode boucle et plage de position](guides/loop-and-position-range.md)
* [Tous les guides](guides/index.md)

### Exemples de code

* [Obtenir une image depuis un fichier vidéo](code-samples/get-frame-from-video-file.md)
* [Lire un fragment d'un fichier](code-samples/play-fragment-file.md)
* [Afficher la première image](code-samples/show-first-frame.md)
* [Lecture depuis la mémoire](code-samples/memory-playback.md)
* [API de liste de lecture](code-samples/playlist-api.md)
* [Lecture vidéo inversée](code-samples/reverse-video-playback.md)
* [Tous les exemples de code](code-samples/index.md)

### Déploiement

* [Guide de déploiement](deployment.md)

## Ressources pour les développeurs

* [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Référence de l'API](https://api.visioforge.org/dotnet/api/index.html)
* [Journal des modifications](../changelog.md)
* [Contrat de licence utilisateur final](../../eula.md)
* [Informations de licence](../../../licensing.md)
