---
title: Lecture vidéo et RTSP dans Unity avec Media Blocks SDK .NET
description: Ajoutez la lecture vidéo et le streaming de caméras RTSP à Unity 6 avec le VisioForge Media Blocks SDK .NET — un .unitypackage autonome et prêt à importer.
sidebar_label: Unity
order: 50
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - RTSP
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - BufferSinkBlock
  - UniversalSourceBlock
  - RTSPSourceBlock
  - AudioRendererBlock
---

# Lecture vidéo et streaming RTSP dans Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le **Media Blocks SDK .NET** est livré avec un **`.unitypackage`** prêt à importer qui apporte la
lecture de fichiers vidéo et le streaming en direct de caméras RTSP / IP dans **Unity 6** sur
**Windows x64**. Importez-le, appuyez sur **Play**, et la vidéo s'affiche dans un `RawImage` Unity.

Pour installer le paquet, consultez **[Installer Media Blocks SDK dans Unity](../../install/unity.md)**.
Ce guide se concentre sur le fonctionnement de l'intégration et sur l'utilisation des deux exemples fournis.

!!! tip "Assistants de codage IA : utilisez le serveur MCP VisioForge"
    Vous construisez cela avec **Claude Code**, **Cursor** ou un autre assistant de codage IA ?
    Connectez-vous au [serveur MCP VisioForge](../mcp-server-usage.md) public à l'adresse
    `https://mcp.visioforge.com/mcp` pour des recherches d'API structurées et des exemples de code
    vérifiés.

!!! info "Le SDK complet est disponible — les exemples ne sont qu'un point de départ"
    Le paquet embarque le **Media Blocks SDK .NET complet**. Les deux scènes incluses
    (`SimplePlayer` et `RTSPViewer`) sont des exemples pour vous lancer rapidement — vos scripts ont
    accès à l'**intégralité de l'API Media Blocks** : capture et types de sources multiples,
    décodeurs et encodeurs, traitement et effets audio/vidéo, mixage et compositing, enregistrement
    vers fichier et sortie en streaming réseau. Construisez le pipeline dont votre application a
    besoin. Consultez la [documentation du Media Blocks SDK .NET](../../mediablocks/index.md) pour le
    catalogue complet des blocs.

## Exemples

Le paquet est livré avec deux scènes prêtes à l'emploi dans `Assets/Scenes/`. Ouvrez-en une dans la
fenêtre **Project** (double-cliquez dessus — ne restez pas sur la scène par défaut vide) et appuyez
sur **▶ Play** :

- **[Lire un fichier multimédia](simple-player.md)** — la scène `SimplePlayer`, lecture d'un fichier vidéo local.
- **[Afficher une caméra RTSP](rtsp-viewer.md)** — la scène `RTSPViewer`, flux RTSP / caméra IP en direct.

!!! tip "Le RawImage semble vide tant que vous n'avez pas appuyé sur Play"
    La texture vidéo est créée à l'exécution, le `RawImage` est donc vierge (blanc) en mode édition.
    C'est normal — rien n'est dessiné tant que le pipeline n'a pas démarré.

## Fonctionnement du rendu

Deux assistants partagés gèrent la configuration et le rendu à votre place, de sorte que chaque
script de lecteur se résume au pipeline Media Blocks :

1. **`VisioForgeEnvironment.Configure()`** s'exécute automatiquement avant le chargement de la
   première scène et prépare le runtime natif fourni — vous ne gérez aucune dépendance ni aucun
   chemin natif.
2. **`VisioForgeEnvironment.InitializeSdk()`** initialise le SDK une seule fois. Appelez-le avant de
   construire un pipeline (les lecteurs d'exemple l'appellent dans `Start()`).
3. Chaque lecteur construit un pipeline se terminant par un **`BufferSinkBlock(VideoFormatX.RGBA)`** ;
   son événement `OnVideoFrameBuffer` transmet les images vidéo à **`VisioForgeVideoView`**.
4. **`VisioForgeVideoView`** charge chaque image dans un `Texture2D` Unity sur le thread principal et
   applique le mode d'aspect, de sorte que la vidéo apparaît dans votre `RawImage`. Vous n'écrivez
   aucun code de texture — il suffit de l'attacher (les lecteurs d'exemple le font pour vous).

### Cycle de vie dans l'Éditeur

Le SDK s'initialise une fois par processus et est réutilisé d'une session Play/Stop à l'autre dans
l'Éditeur. Deux conséquences en découlent :

- **Conservez Disable Domain Reload activé** pour que le retour en mode Play réutilise le SDK
  initialisé. S'il est désactivé, l'Éditeur peut se figer en quittant le mode Play.
- Les lecteurs d'exemple ne libèrent que le pipeline propre à chaque lecture au Stop (`StopAsync`)
  et **n'arrêtent** volontairement **pas** tout le SDK — conservez ce modèle dans vos propres
  scripts.

Si vous rencontrez de l'instabilité après une recompilation de script, redémarrez l'Éditeur.

## Foire aux questions

### Ai-je accès au Media Blocks SDK complet, ou seulement à la lecture ?

Au SDK complet. Les deux scènes d'exemple sont des points de départ ; vos scripts ont accès à
l'intégralité de l'API du Media Blocks SDK .NET — capture, types de sources multiples, décodeurs et
encodeurs, traitement et effets audio/vidéo, mixage et compositing, enregistrement vers fichier et
streaming réseau.

### Puis-je afficher la vidéo dans ma propre interface plutôt que dans les scènes d'exemple ?

Oui. Ajoutez un `RawImage`, attachez `MediaBlocksPlayer` (fichier) ou `RTSPViewerPlayer` (RTSP), ou
construisez votre propre pipeline et alimentez un `BufferSinkBlock` vers `VisioForgeVideoView`. Le
chargement de la texture, la gestion de l'aspect et le retournement sont pris en charge pour vous.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration complète, pas à pas
- [Lire un fichier multimédia dans Unity](simple-player.md) — l'exemple de lecture de fichier
- [Afficher une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple de flux RTSP / caméra IP en direct
- [Présentation du Media Blocks SDK .NET](../../mediablocks/index.md) — le catalogue complet des blocs et le guide des pipelines
- [Guide du streaming RTSP](../network-streaming/rtsp.md) — le RTSP à travers les SDK VisioForge .NET
- [Répertoire des marques de caméras IP](../../camera-brands/index.md) — URLs et réglages de caméras testés
