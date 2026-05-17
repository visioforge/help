---
title: VisioForge Media Blocks SDK en C# .NET — Aide-mémoire
description: Référence d'une page Media Blocks SDK avec paquets NuGet, API du pipeline, exemple canonique, prise en charge des plateformes et pièges courants.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - Playback
  - Capture
  - Recording
  - Streaming
  - Encoding
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
  - SystemVideoSourceBlock
  - H264EncoderBlock
  - MP4SinkBlock
  - DeviceEnumerator
---

# VisioForge Media Blocks SDK en C# .NET — Aide-mémoire

Media Blocks SDK est le SDK .NET le plus flexible de VisioForge — construisez des pipelines multimédias arbitraires en composant des blocs (sources, encodeurs, moteurs de rendu, puits). Choisissez Media Blocks lorsque vous avez besoin de pipelines personnalisés que les SDK de plus haut niveau (Media Player / Video Capture / Video Edit) ne peuvent pas exprimer.

!!! tip "Agents IA de programmation : utilisez le serveur MCP VisioForge"

    Vous construisez avec **Claude Code**, **Cursor** ou un autre agent IA de programmation ?
    Connectez-vous au [serveur MCP public VisioForge](../general/mcp-server-usage.md)
    à `https://mcp.visioforge.com/mcp` pour des recherches d'API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis qu'un grep
    sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prise en charge des plateformes

- Fonctionne sous Windows (x64 / x86), macOS, Linux x64, Android et iOS.
- Frameworks d'interface utilisateur : WinForms, WPF, MAUI, Avalonia, Uno, plus la console.
- Le traitement multiplateforme est assuré par un backend GStreamer fourni sous macOS/Android/iOS et par l'installation système de GStreamer 1.22+ sous Linux.
- Pour la matrice complète codec × plateforme, voir [platform-matrix.md](../platform-matrix.md).

## Paquets NuGet

Paquet SDK principal (toutes les plateformes) :

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Runtime Windows x64 (choisissez x86 pour les applications 32 bits) :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<!-- Optionnel : décodeurs/encodeurs supplémentaires via Libav -->
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Windows x86 :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86.UPX" Version="2025.4.9" />
```

macOS (natif et MAUI / macCatalyst) :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.4.9" />
```

Linux x64 (nécessite également GStreamer 1.22+ installé sur le système) :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.4.9" />
```

Android et iOS :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.4.9" />
```

Paquets d'intégration UI (optionnels, par framework) :

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.4.9" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2025.4.9" />
```

Procédure d'installation complète : [install/index.md](../install/index.md).

## Classes d'API principales

| Classe | Rôle | Voir aussi |
| --- | --- | --- |
| `MediaBlocksPipeline` | Objet pipeline racine. Connecte les blocs via `Connect(output, input)`. Expose `StartAsync` / `StopAsync` / `DisposeAsync` et les événements `OnError`, `OnStart`, `OnStop`. | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `UniversalSourceBlock` | Ouvre un fichier, une URL ou un flux en entrée. Analyse les flux automatiquement et expose les pads `VideoOutput` / `AudioOutput`. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `VideoRendererBlock` | Lie la sortie vidéo du pipeline à un contrôle `IVideoView` (WinForms / WPF / MAUI / Avalonia). Prend en charge les captures d'écran. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `SystemVideoSourceBlock` | Entrée webcam / USB / caméra intégrée. Se configure via `VideoCaptureDeviceSourceSettings`. | [GettingStarted/camera.md](./GettingStarted/camera.md) |
| `H264EncoderBlock` | Encodeur H.264 avec backends logiciels et matériels (NVENC / AMF / Quick Sync). | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `MP4SinkBlock` | Écrit la vidéo + l'audio encodés dans un fichier `.mp4`. Ajoutez des entrées via `IMediaBlockDynamicInputs.CreateNewInput`. | [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md) |
| `DeviceEnumerator` | Liste les caméras, micros et sorties audio de façon asynchrone via `DeviceEnumerator.Shared.VideoSourcesAsync()`, etc. | [GettingStarted/device-enum.md](./GettingStarted/device-enum.md) |

## Exemple minimal canonique

Le pipeline utile le plus simple — charger un fichier, faire le rendu vidéo + audio, nettoyer les ressources. Reprenez et adaptez depuis [GettingStarted/player.md](./GettingStarted/player.md).

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// 1. Initialiser le SDK une seule fois au démarrage de l'application
await VisioForgeX.InitSDKAsync();

// 2. Créer le pipeline et s'abonner aux erreurs
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

// 3. Source : ouvrir un fichier multimédia (URL ou chemin local via URI)
var sourceSettings = await UniversalSourceSettings.CreateAsync(
    new Uri("file:///C:/Videos/sample.mp4"));
var fileSource = new UniversalSourceBlock(sourceSettings);

// 4. Moteur de rendu vidéo — VideoView1 est un IVideoView sur votre formulaire/page
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// 5. Moteur de rendu audio — choisir la première sortie audio système
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var audioRenderer = new AudioRendererBlock(audioOutputs[0]);

// 6. Connecter les pads de sortie → pads d'entrée
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);

// 7. Lecture
await pipeline.StartAsync();

// ... plus tard, à l'arrêt utilisateur / sortie de l'application :
await pipeline.StopAsync();
await pipeline.DisposeAsync();
VisioForgeX.DestroySDK(); // synchrone uniquement — pas de variante asynchrone
```

Remplacez `UniversalSourceBlock` par `SystemVideoSourceBlock` (caméra) ou `RTSPSourceBlock` (caméra IP) sans changer le reste du câblage.

## Flux de travail typique

1. Initialiser le SDK — `await VisioForgeX.InitSDKAsync()`.
2. Créer `MediaBlocksPipeline` et s'abonner à `OnError`.
3. Instancier les blocs de source (`UniversalSourceBlock`, `SystemVideoSourceBlock`, `RTSPSourceBlock`, …).
4. Instancier les blocs de traitement — encodeurs, mélangeurs, superpositions, effets (optionnel).
5. Instancier les blocs puits / moteurs de rendu (`VideoRendererBlock`, `AudioRendererBlock`, `MP4SinkBlock`, …).
6. Câbler les blocs avec `pipeline.Connect(output, input)` — chaque chemin de données doit être connecté explicitement.
7. `StartAsync` → `StopAsync` → `DisposeAsync`, puis `DestroySDK` à la sortie de l'application.

## Pièges courants

- **Pas de flux de données implicite.** Les blocs doivent être câblés explicitement avec `pipeline.Connect(output, input)` ; ajouter un bloc au pipeline seul ne suffit pas pour y faire transiter le média.
- **Ordre des superpositions / entrées dynamiques.** Les superpositions sur `OverlayManagerBlock` et les pads dynamiques sur les puits (`MP4SinkBlock` via `IMediaBlockDynamicInputs.CreateNewInput`) doivent être ajoutés avant le démarrage du pipeline — les ajouter après `StartAsync` échoue silencieusement ou lève une exception.
- **Énumération uniquement avec await.** `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioOutputsAsync()` doivent être attendus avec await — il n'existe pas de variante synchrone. Utiliser `.Result` depuis un thread UI peut provoquer un interblocage.
- **Linux nécessite GStreamer système.** `VisioForge.CrossPlatform.Core.Linux.x64` attend que GStreamer 1.22+ soit déjà installé (`apt install gstreamer1.0-*`) ; le NuGet Libav est complémentaire, pas un remplacement. Voir [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md).
- **Hygiène du cycle de vie.** Appelez toujours `await pipeline.StopAsync()` et `await pipeline.DisposeAsync()` avant de créer un autre pipeline sur le même moteur. Sauter le dispose laisse fuir des handles natifs et peut bloquer à l'arrêt.

## Voir aussi

- **Prise en main**
    - Lecteur de fichiers vidéo — [GettingStarted/player.md](./GettingStarted/player.md)
    - Visualisateur de caméra — [GettingStarted/camera.md](./GettingStarted/camera.md)
    - Tutoriel complet de pipeline — [GettingStarted/pipeline.md](./GettingStarted/pipeline.md)
    - Énumération des périphériques — [GettingStarted/device-enum.md](./GettingStarted/device-enum.md)
- **Pipelines spécifiques**
    - Lecteur RTSP / caméra IP — [Guides/rtsp-player-csharp.md](./Guides/rtsp-player-csharp.md)
    - Enregistrer un flux RTSP (passthrough) — [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md)
    - Grille multicaméra — [Guides/multi-camera-rtsp-grid.md](./Guides/multi-camera-rtsp-grid.md)
- **Déploiement**
    - Windows — [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS — [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Ubuntu — [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android — [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS — [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Installation et matrice de plateformes** — [../install/index.md](../install/index.md), [../platform-matrix.md](../platform-matrix.md)

## FAQ

### En quoi Media Blocks diffère-t-il du Media Player SDK ?

Media Player SDK est un lecteur de haut niveau à classe unique (`MediaPlayerCoreX`) avec des contrôles de lecture intégrés — optimal lorsque vous avez juste besoin de lire un fichier ou un flux. Media Blocks expose le pipeline sous-jacent, ce qui vous permet d'insérer des encodeurs, des effets, des puits multi-sorties ou un traitement personnalisé. Si vous vous battez avec Media Player SDK pour ajouter une étape qu'il n'expose pas, passez à Media Blocks.

### Puis-je exécuter sous Linux sans GStreamer ?

Non. Le NuGet runtime Linux x64 est un pont fin vers la pile GStreamer 1.22+ du système. Installez `gstreamer1.0-plugins-base`, `-good`, `-bad`, `-ugly` et `-libav` depuis votre distribution. Voir [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md) pour la liste complète des paquets. macOS / Android / iOS embarquent un GStreamer via le NuGet de plateforme.

### Comment ajouter un effet vidéo personnalisé ?

Utilisez `GLShaderBlock` pour un fragment shader GLSL personnalisé (accéléré GPU) ou la famille préfabriquée `GL*Block` (par ex. `GLBlurBlock`, `GLColorBalanceBlock`) pour les effets courants. Pour un traitement basé sur CV, utilisez la famille `CV*Block` (par ex. `CVFaceDetectBlock`, `CVEdgeDetectBlock`, `CVDewarpBlock`). Tous se placent entre un décodeur et un moteur de rendu comme n'importe quel autre bloc. Recette complète : [Guides/custom-video-effects-csharp.md](./Guides/custom-video-effects-csharp.md).
