---
title: Lecture vidéo et RTSP dans Unity avec Media Blocks SDK
description: Ajoutez la lecture vidéo et le RTSP à Unity 6 avec VisioForge Media Blocks SDK .NET — .unitypackage pour Windows, Android, macOS et iOS.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Android
  - macOS
  - iOS
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
  - VisioForgeEnvironment
---

# Lecture vidéo et streaming RTSP dans Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Le **Media Blocks SDK .NET** est livré avec un **`.unitypackage`** multiplateforme prêt à
importer qui apporte la lecture de fichiers vidéo, le streaming RTSP / IP en direct et le
reste du pipeline Media Blocks dans **Unity 6**. Le même paquet cible quatre
plateformes — **Windows x64**, **Android (IL2CPP arm64)**, **macOS Standalone
(Universel arm64+x86_64)** et **iOS Standalone (appareil arm64)**. Importez une fois, basculez
Build Target, compilez.

Pour installer le paquet, consultez
**[Installer Media Blocks SDK dans Unity](../../install/unity.md)**. Pour le raccourci en cinq
étapes, consultez le **[Guide rapide](getting-started.md)**.

!!! tip "Agents de programmation IA : utilisez le serveur MCP VisioForge"
    Vous le construisez avec **Claude Code**, **Cursor** ou un autre agent de programmation
    IA ? Connectez-vous au
    [serveur MCP public VisioForge](../mcp-server-usage.md) à `https://mcp.visioforge.com/mcp`
    pour des recherches structurées d'API et des exemples de code vérifiés.

!!! info "Le SDK complet est disponible — les exemples ne sont qu'un point de départ"
    Le paquet contient le **Media Blocks SDK .NET complet**. Les scènes incluses
    sont des exemples pour démarrer rapidement — vos scripts
    ont accès à l'**API Media Blocks complète** : capture et types de sources multiples,
    décodeurs et encodeurs, traitement et effets audio/vidéo, mixage et composition,
    enregistrement vers fichier et sortie de streaming réseau. Construisez n'importe quel
    pipeline dont votre app a besoin. Voir la
    [documentation Media Blocks SDK .NET](../../mediablocks/index.md) pour le catalogue complet
    de blocs.

## Packaging cumulatif par plateforme

Le `.unitypackage` livré est **cumulatif** — un fichier porte les assemblies managés plus
chaque runtime natif, et les métadonnées `PluginImporter` par fichier d'Unity choisissent la
bonne copie quand vous basculez Build Target. Il n'y a pas de téléchargement par plateforme.

| Plateforme | Runtime natif | Architecture | Profil de build |
|---|---|---|---|
| Windows | installation GStreamer à plat dans `StreamingAssets/VisioForge/x64/` | x86_64 | [Compilation pour Windows](windows.md) |
| Android | `libgstreamer_android.so` monolithique + AAR Java | arm64-v8a | [Compilation pour Android](android.md) |
| macOS | ~300 dylibs séparés dans `Plugins/macOS/` | Universel arm64 + x86_64 | [Compilation pour macOS](macos.md) |
| iOS | `GStreamerX.framework` embarqué (plugins enregistrés statiquement) | appareil arm64 | [Compilation pour iOS](ios.md) |

Les quatre déclinaisons partagent la même surface managée — `MediaBlocksPipeline`,
`BufferSinkBlock`, `RTSPSourceBlock`, `UniversalSourceBlock`, chaque effet, chaque encoder,
chaque sink. La seule chose spécifique à la plateforme que votre script voit est la valeur
`Application.platform` à l'exécution.

## Exemples

Le paquet livre des scènes prêtes sous `Assets/Scenes/`. Ouvrez-en une dans la fenêtre
**Project** (double-clic — ne restez pas sur la scène par défaut vide) et appuyez sur
**▶ Play** :

- **[Lire un fichier multimédia](simple-player.md)** — deux scènes : la `SimplePlayer` de bas niveau
  (`MediaBlocksPipeline`) et la `MediaPlayerX` de haut niveau (`MediaPlayerCoreX`, avec
  navigation/pause/volume).
- **[Voir une caméra RTSP](rtsp-viewer.md)** — deux scènes : la `RTSPViewer` de bas niveau
  (`MediaBlocksPipeline`) et la `IPCameraX` de haut niveau (`VideoCaptureCoreX`, avec enregistrement
  optionnel).
- **[Capturer une webcam](video-capture-x.md)** — la scène `VideoCaptureX` : webcam + microphone
  local avec enregistrement MP4 (`VideoCaptureCoreX`, Windows / macOS).
- **[Monter et rendre](video-edit-x.md)** — la scène `VideoEditX` : un montage multi-clips
  prévisualisé en direct, puis rendu en MP4 (`VideoEditCoreX`).

Les scènes de bas niveau utilisent l'API `MediaBlocksPipeline` ; les scènes CoreX de haut niveau
rendent dans un `RawImage` via l'événement propre à Unity `OnVideoFrameUnity` (RGBA32 compact, prêt
pour `Texture2D`).

!!! tip "Le RawImage paraît vide jusqu'à ce que vous appuyiez sur Play"
    La texture vidéo est créée à l'exécution, donc le `RawImage` est vide (blanc) en mode
    édition. C'est attendu — rien n'est dessiné jusqu'à ce que le pipeline démarre.

## Comment fonctionne le rendu

Deux helpers partagés gèrent la configuration et le rendu pour vous, donc chaque script
player n'est que le pipeline Media Blocks :

1. **`VisioForgeEnvironment.Configure()`** s'exécute automatiquement avant le chargement de
   la première scène et prépare le runtime natif fourni pour la plateforme courante — chemin
   de recherche DLL Windows, hints du chargeur dylib macOS, bootstrap Java Android ou
   démarrage du framework iOS. Vous ne gérez aucune dépendance native ni chemin. L'histoire
   complète est dans [Démarrage et cycle de vie](bootstrap.md).
2. **`VisioForgeEnvironment.InitializeSdk()`** initialise le SDK une fois. Appelez-le avant
   de construire un pipeline (les players d'exemple l'appellent dans `Start()`).
3. Chaque player construit un pipeline qui se termine par un
   **`BufferSinkBlock(VideoFormatX.RGBA)`** ; son événement `OnVideoFrameBuffer` remet les
   frames vidéo à **`VisioForgeVideoView`**.
4. **`VisioForgeVideoView`** uploade chaque frame dans un `Texture2D` Unity sur le thread
   principal et applique le mode d'aspect, donc la vidéo apparaît dans votre `RawImage`. Vous
   n'écrivez aucun code de texture — attachez-le simplement (les players d'exemple le font
   pour vous).

### Cycle de vie Éditeur

Le SDK exécute sa boucle principale GLib de GStreamer sur un thread d'arrière-plan qu'Unity
ne peut pas abandonner. Deux points en découlent :

- **Vous n'avez pas besoin de désactiver le Domain Reload.** Le comportement par défaut
  d'Unity à l'entrée du mode Play (Domain + Scene Reload) est entièrement pris en charge — le
  guard de rechargement du paquet arrête le thread de la boucle principale avant chaque
  rechargement, donc Play/Stop se termine sans blocage.
- Les players d'exemple ne libèrent que le pipeline par-Play sur Stop (`StopAsync`) et
  **intentionnellement** ne coupent pas tout le SDK — gardez ce motif dans vos propres
  scripts.

Si vous rencontrez de l'instabilité après une recompilation de script, redémarrez l'Éditeur.
Voir [Démarrage et cycle de vie](bootstrap.md#cycle-de-vie-editeur) pour la justification.

## Foire aux questions

### Ai-je le Media Blocks SDK complet ou seulement la lecture ?

Le SDK complet. Les scènes d'exemple sont des points de départ ; vos scripts ont accès
à toute l'API Media Blocks SDK .NET — capture, types de sources multiples, décodeurs et
encodeurs, traitement et effets audio/vidéo, mixage et composition, enregistrement vers
fichier et sortie de streaming réseau.

### Puis-je rendre la vidéo dans ma propre UI au lieu des scènes d'exemple ?

Oui. Ajoutez un `RawImage`, attachez `MediaBlocksPlayer` (fichier) ou `RTSPViewerPlayer`
(RTSP), ou construisez votre propre pipeline et alimentez un `BufferSinkBlock` à
`VisioForgeVideoView`. L'upload de texture, la gestion du ratio et le flip sont gérés pour
vous.

### Le même paquet est-il utilisé pour chaque plateforme ?

Oui — un `.unitypackage` cumulatif contient les runtimes natifs Windows, Android, macOS et
iOS plus un seul jeu d'assemblies managés `netstandard2.1`. Unity choisit le bon slot au
moment du build à partir des métadonnées `PluginImporter` par fichier ; vous n'importez pas
un paquet séparé par plateforme.

### Puis-je voir quel chemin de plateforme s'exécute ?

Oui. `VisioForgeEnvironment.Configure()` journalise l'une de
`[VisioForge] Environment configured. Natives: <path>` (Windows / macOS),
`[VisioForge] Android GStreamer bootstrap complete.` ou
`[VisioForge] iOS environment configured (GStreamerX.framework via @rpath).` dans la Console.
Utilisez cette ligne pour confirmer quelle branche a tourné.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration complète,
  étape par étape
- [Guide rapide](getting-started.md) — le chemin en cinq étapes jusqu'à une vidéo qui joue
- [Démarrage et cycle de vie](bootstrap.md) — ce que font `Configure()` et `InitializeSdk()`
- [Lire un fichier multimédia dans Unity](simple-player.md) — l'exemple de lecture de
  fichiers
- [Voir une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple RTSP / caméra IP en direct
- [Capturer une webcam avec VideoCaptureCoreX](video-capture-x.md) · [Monter et rendre avec VideoEditCoreX](video-edit-x.md) — les exemples de moteurs CoreX autonomes
- [Compilation pour Windows](windows.md) · [Android](android.md) · [macOS](macos.md) · [iOS](ios.md)
- [Matrice des plateformes](platform-matrix.md) — prise en charge des fonctionnalités par
  plateforme Unity
- [Dépannage](troubleshooting.md) — erreurs courantes et solutions
- [Aperçu du Media Blocks SDK .NET](../../mediablocks/index.md) — le catalogue complet de
  blocs
- [Répertoire des marques de caméras IP](../../camera-brands/index.md) — URLs testées et
  réglages
