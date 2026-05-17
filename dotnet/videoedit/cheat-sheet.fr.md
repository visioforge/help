---
title: VisioForge Video Edit SDK en C# .NET — Aide-mémoire
description: Référence d'une page du Video Edit SDK — paquets NuGet, API VideoEditCoreX, exemple de timeline, prise en charge des plateformes et pièges.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - VideoEditCore
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
  - DirectShow
  - Editing
  - Encoding
  - Effects
  - Mixing
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoEditCoreX
  - VideoEditCore
  - MP4Output
  - VideoSource
  - VideoFileSource
---

# VisioForge Video Edit SDK .Net — Aide-mémoire

Le Video Edit SDK assemble des timelines, applique des transitions/effets/superpositions et exporte vers MP4/MKV/WebM. `VideoEditCoreX` est le moteur multiplateforme (GStreamer) ; `VideoEditCore` est le moteur historique exclusivement Windows basé sur DirectShow. Choisissez `VideoEditCoreX` pour les nouveaux projets.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prise en charge des plateformes

- `VideoEditCoreX` (multiplateforme, GStreamer) : Windows, macOS (Intel + Apple Silicon), Linux (Ubuntu/Debian/CentOS), Android (via MAUI), iOS (via MAUI).
- `VideoEditCore` (historique, DirectShow) : Windows x64 uniquement.
- Frameworks d'interface : WinForms, WPF, MAUI, Avalonia, Uno, Console.
- Matrice complète moteur × plateforme × interface : [platform-matrix.md](../platform-matrix.md).

## Paquets NuGet

Moteur multiplateforme (recommandé pour les nouveaux projets) :

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEditX" Version="*" />
```

Moteur historique exclusivement Windows :

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="*" />
<PackageReference Include="VisioForge.DotNet.VideoEdit" Version="*" />
```

Intégration de l'interface utilisateur (choisissez celle qui correspond à votre pile UI) :

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.WinUI" Version="*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Uno" Version="*" />
```

Les redistribuables natifs spécifiques à chaque plateforme suivent le même schéma que les autres SDK VisioForge (les paquets redist par OS embarquent les binaires natifs GStreamer). Consultez le [guide d'installation](../install/index.md) pour la liste complète.

## Classes principales de l'API

| Classe | Rôle | Voir aussi |
|---|---|---|
| `VideoEditCoreX` | Moteur de timeline multiplateforme (basé sur GStreamer) | [getting-started.md](./getting-started.md) |
| `VideoEditCore` | Moteur historique exclusivement Windows (DirectShow) — pour maintenir les applications existantes en fonctionnement | [getting-started.md](./getting-started.md) |
| `MP4Output` | Configuration de sortie MP4 encodée (H.264/H.265 + AAC) | [getting-started.md](./getting-started.md) |
| `GIFOutput` | Sortie GIF animé (vidéo uniquement, sans audio, palette 256 couleurs) — `gifenc` écrit le fichier directement sans conteneur | [getting-started.md](./getting-started.md#rendering-to-animated-gif) |
| `VideoSource` / `VideoFileSource` | Descripteur d'entrée vidéo par segment (`VideoSource` pour `VideoEditCore`, `VideoFileSource` pour `VideoEditCoreX`) | [code-samples/several-segments.md](./code-samples/several-segments.md) |
| `AudioFileSource` | Entrée audio depuis fichier pour mélanger dans la timeline | [code-samples/volume-for-track.md](./code-samples/volume-for-track.md) |

## Exemple minimum canonique

Fusion de timeline à deux clips, sortie MP4 H.264, multiplateforme avec `VideoEditCoreX` :

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// 1. Initialiser le SDK (requis pour les moteurs multiplateformes).
await VisioForgeX.InitSDKAsync();

// 2. Créer l'éditeur. Passez un IVideoView pour l'aperçu, ou null pour mode sans interface/console.
var editor = new VideoEditCoreX(videoView as IVideoView);

// 3. Brancher les événements.
editor.OnError    += (s, e) => Console.WriteLine($"Error: {e.Message}");
editor.OnProgress += (s, e) => Console.WriteLine($"Progress: {e.Progress}%");
editor.OnStop     += (s, e) => Console.WriteLine("Editing completed");

// 4. Définir les paramètres de sortie de la timeline AVANT d'ajouter des sources.
editor.Output_VideoSize      = new Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// 5. Ajouter des sources à la timeline (audio + vidéo en un seul appel).
editor.Input_AddAudioVideoFile("/abs/path/intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("/abs/path/main.mp4",  null, null, null);

// 6. Configurer la sortie MP4.
editor.Output_Format = new MP4Output("/abs/path/output.mp4");

// 7. Démarrer le traitement. OnStop se déclenche à la fin du rendu.
editor.Start();

// ... à l'intérieur de votre gestionnaire OnStop, ou après avoir attendu un signal de fin :
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK(); // synchrone uniquement — pas de variante async
```

Pour le rognage, utilisez plutôt `Input_AddVideoFile` avec un `VideoFileSource(filename, startTime, stopTime, streamNumber, rate)` — les valeurs `startTime`/`stopTime` délimitent la plage d'entrée.

## Flux de travail typique

1. Initialiser le SDK : `await VisioForgeX.InitSDKAsync()` (moteur multiplateforme uniquement — `VideoEditCore` historique l'omet).
2. Instancier `VideoEditCoreX` (ou `VideoEditCore` pour l'historique Windows), en passant un `IVideoView` ou `null` pour le mode sans interface.
3. Définir la résolution de sortie, la fréquence d'images et l'encodeur (`Output_VideoSize`, `Output_VideoFrameRate`).
4. Ajouter les sources d'entrée (fichiers, segments, images, audio) — **avant** `Start`.
5. Définir le fichier de sortie via `MP4Output` (ou `MKVOutput` / `WebMOutput` / `GIFOutput` pour GIF animé) sur `Output_Format`.
6. `Start()` (CoreX) ou `await StartAsync()` (historique) → attendre `OnStop` → `Dispose()` + `DestroySDK()`.

## Pièges courants

- **Définissez la résolution / fréquence d'images de sortie de la timeline avant d'ajouter des sources.** Modifier `Output_VideoSize` ou `Output_VideoFrameRate` après `Input_Add*` peut reconfigurer silencieusement le pipeline ou provoquer une dérive de synchronisation.
- **Ne mélangez pas les API des moteurs.** `VideoEditCore` utilise `VideoSource` + `Input_AddVideoFileAsync` ; `VideoEditCoreX` utilise `VideoFileSource` + `Input_AddVideoFile` (synchrone). Les espaces de noms de types diffèrent également (`Types.VideoEdit` vs `Types.X.VideoEdit`).
- **L'initialisation ne s'applique qu'au moteur X.** `VideoEditCoreX` requiert `VisioForgeX.InitSDKAsync()` / `DestroySDK()`. `VideoEditCore` (historique) non — appeler InitSDK pour des applications utilisant uniquement l'historique est inutile.
- **Utilisez des chemins de fichiers absolus.** Les chemins relatifs se résolvent par rapport au répertoire de travail du processus et se comportent de manière incohérente selon les plateformes (en particulier MAUI/iOS/Android où le CWD n'est pas votre dossier de projet).
- **`Start` n'est pas bloquant ; attendez `OnStop`.** Ne supposez pas que le rendu est terminé lorsque `Start()` retourne. Utilisez `OnStop` (ou enveloppez-le avec `await`) pour déclencher le post-traitement, puis `Dispose()` et `DestroySDK()`.

## Voir aussi

- **Prise en main**
    - [Guide de prise en main](./getting-started.md) — visite guidée complète pour les deux moteurs.
    - [Index des exemples de code](./code-samples/index.md) — superpositions, transitions, audio, composition.
- **Tâches spécifiques**
    - Fusionner / concaténer des clips → [output-file-from-multiple-sources.md](./code-samples/output-file-from-multiple-sources.md)
    - Rogner / multi-segments depuis un seul fichier → [several-segments.md](./code-samples/several-segments.md)
    - Transitions entre clips → [transition-video.md](./code-samples/transition-video.md), [référence des transitions](./transitions.md)
    - Superposition de texte → [add-text-overlay.md](./code-samples/add-text-overlay.md)
    - Superposition d'image / logo → [add-image-overlay.md](./code-samples/add-image-overlay.md)
    - Image dans l'image → [picture-in-picture.md](./code-samples/picture-in-picture.md)
    - Diaporama à partir d'images → [video-images-console.md](./code-samples/video-images-console.md)
    - Mélange audio / enveloppe de volume → [audio-envelope.md](./code-samples/audio-envelope.md), [volume-for-track.md](./code-samples/volume-for-track.md)
    - Application d'édition iOS → [ios-video-editor.md](./code-samples/ios-video-editor.md)
- **Déploiement** — [Windows / macOS / Ubuntu / Android / iOS](../deployment-x/index.md)
- **Installation et matrice** — [Guide d'installation](../install/index.md) · [Matrice des plateformes](../platform-matrix.md)

## FAQ

### `VideoEditCoreX` vs `VideoEditCore` — lequel utiliser ?

Utilisez `VideoEditCoreX` pour les nouveaux projets. Il fonctionne sur Windows, macOS, Linux, Android et iOS sur un backend GStreamer. Utilisez `VideoEditCore` uniquement pour maintenir en fonctionnement des applications Windows DirectShow existantes.

### Puis-je ajouter des transitions entre clips ?

Oui. `VideoEditCoreX` prend en charge plus de 100 transitions SMPTE de type wipe entre segments consécutifs — voir [transition-video.md](./code-samples/transition-video.md) et la [référence des transitions](./transitions.md).

### Fonctionne-t-il sur macOS / Linux ?

Oui — `VideoEditCoreX` fonctionne sur macOS (Intel + Apple Silicon) et Linux (Ubuntu/Debian/CentOS) grâce aux binaires natifs GStreamer fournis. `VideoEditCore` est exclusivement Windows.

### Comment rogner un fichier sans le rajouter plusieurs fois ?

Passez un tableau `FileSegment[]` au constructeur de `VideoFileSource` — chaque segment devient une plage rognée du même fichier source sur la timeline de sortie.
