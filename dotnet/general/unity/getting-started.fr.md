---
title: Démarrer avec Media Blocks SDK dans Unity 6 — Guide rapide
description: Guide rapide en cinq étapes pour passer d'un projet Unity 6 neuf à une vidéo qui joue — importez le .unitypackage VisioForge, appliquez les réglages et lancez.
sidebar_label: Prise en main
order: 51
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Quickstart
  - Windows
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
  - BufferSinkBlock
---

# Prise en main

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Cette page est le chemin en cinq étapes d'un projet Unity 6 neuf à une vidéo qui joue. Pour les
détails d'installation, les notes par plateforme et l'explication du bootstrap, suivez les
liens en fin de page.

## Ce dont vous avez besoin

- **Unity 6 LTS** (`6000.x`) — vérifié sur `6000.4.6f1`. Les Unity LTS antérieures
  (2022 / 2023) peuvent aussi fonctionner (non testées), du moment qu'elles offrent `.NET Standard 2.1` comme
  option d'Api Compatibility Level.
- **Un chemin de projet NTFS court** sous Windows — par exemple, `C:\unity\MyApp`. Évitez
  Dev Drive (ReFS) et les chemins très longs ; le cache de paquets d'Unity peut dépasser la
  limite de 260 caractères `MAX_PATH` de Windows.
- Pour les targets mobiles / Apple, le module Unity correspondant installé via Unity Hub
  (Android Build Support, iOS Build Support, macOS Build Support).

## Étape 1 — Téléchargez le `.unitypackage` cumulatif

[**VisioForge.MediaBlocks.Unity.unitypackage**](https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage)

```text
https://files.visioforge.com/unity/VisioForge.MediaBlocks.Unity.unitypackage
```

Le paquet est autonome — assemblies managés, chaque runtime natif pris en charge, scènes
d'exemple et l'assistant de configuration unique, tout est à l'intérieur. Pas de restauration
NuGet, pas d'installation GStreamer externe, pas de téléchargement par plateforme.

## Étape 2 — Importez le paquet

Dans Unity : **Assets → Import Package → Custom Package…**, sélectionnez le `.unitypackage`
téléchargé et cliquez sur **Import** avec tous les éléments cochés.

Le paquet cumulatif publié ajoute les quatre runtimes de plateforme (un build privé personnalisé n'inclut que les plateformes activées par ses switches `-Include*`) :

- `Assets/StreamingAssets/VisioForge/x64/` — runtime natif Windows
- `Assets/Plugins/Android/` — runtime natif Android
- `Assets/Plugins/macOS/` — runtime natif macOS
- `Assets/Plugins/iOS/GStreamerX.framework/` — runtime natif iOS
- `Assets/Plugins/` — assemblies managés du SDK
- `Assets/Scripts/` — `VisioForgeEnvironment`, `VisioForgeVideoView` et les deux composants
  player partagés
- `Assets/Scenes/SimplePlayer.unity` et `Assets/Scenes/RTSPViewer.unity` — scènes d'exemple
- `Assets/VisioForge/` — l'assistant de configuration unique et (sur les builds mobiles /
  macOS) `link.xml`

## Étape 3 — Appliquez les réglages du projet

Au premier import, l'assistant de configuration propose d'appliquer le réglage obligatoire.
Cliquez sur **Apply** et il est configuré pour vous :

| Réglage | Valeur | Pourquoi |
|---|---|---|
| Api Compatibility Level | `.NET Standard 2.1` | Le SDK est distribué en assemblies `netstandard2.1`. Le réglage legacy `.NET Framework` ne peut pas les charger. |

Si vous cliquez sur **Skip**, réglez l'Api Compatibility Level manuellement dans
**Edit → Project Settings → Player → Other Settings → Configuration → Api Compatibility
Level**.

Le comportement par défaut d'Unity à l'entrée du mode Play (Domain + Scene Reload) est
entièrement pris en charge — vous n'avez pas besoin de désactiver le Domain Reload. Le SDK
survit aux rechargements Play/Stop de l'Éditeur.

Pour les targets mobiles (Android, iOS), réglez aussi **Scripting Backend = IL2CPP** dans la
même section Configuration. Mono n'est pas pris en charge sur Android ou iOS par Unity
lui-même.

## Étape 4 — Exécutez la scène d'exemple

1. Dans la fenêtre **Project** ouvrez `Assets/Scenes/SimplePlayer.unity` (double-clic — ne
   restez pas sur la scène par défaut vide).
2. Sélectionnez le GameObject **RawImage** dans la **Hierarchy**.
3. Dans l'**Inspector** réglez **File Path** sur un chemin absolu vers un fichier média local.
4. Appuyez sur **▶ Play**.

La vidéo s'affiche dans la vue Game ; l'audio joue via le périphérique audio par défaut du
système.

Si vous avez une caméra RTSP locale, ouvrez `Assets/Scenes/RTSPViewer.unity` à la place,
réglez **Rtsp Url** (plus **Login** / **Password** si la caméra nécessite une
authentification) et appuyez sur **Play**.

## Étape 5 — Adaptez à votre propre scène

Vous n'êtes pas obligé d'utiliser les scènes d'exemple. Pour lire une vidéo dans votre propre
UI :

1. Ajoutez un **Canvas → Raw Image** (*GameObject → UI → Raw Image*).
2. Sélectionnez le **Raw Image** et **Add Component →** `MediaBlocksPlayer` (ou
   `RTSPViewerPlayer` pour RTSP).
3. Réglez le champ **File Path** (ou **Rtsp Url**) et appuyez sur **▶ Play**.

La gestion du ratio, le flip vertical et l'upload `Texture2D` sont gérés par le
`VisioForgeVideoView` fourni. Votre script n'est que le pipeline — voir
[Lire un fichier multimédia dans Unity](simple-player.md) pour le détail C#.

## Compilez pour une plateforme cible

Quand vous êtes prêt à déployer :

- [Windows x64](windows.md) — base Éditeur et Standalone Player.
- [Android](android.md) — IL2CPP arm64, permissions AndroidManifest, notes de taille.
- [macOS](macos.md) — Universel arm64+x86_64, signature de code et notarisation.
- [iOS](ios.md) — export Xcode, permissions Info.plist, IL2CPP arm64 appareil seulement.

Le `.unitypackage` cumulatif contient chaque plateforme pour laquelle il a été opté lors de sa
construction ; Unity choisit le bon runtime par Build Target via les métadonnées
`PluginImporter` par fichier.

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — référence d'installation
- [Utiliser VisioForge dans Unity](index.md) — vue d'ensemble de l'architecture et du rendu
- [Démarrage et cycle de vie](bootstrap.md) — ce que font `Configure()` et `InitializeSdk()`
- [Matrice des plateformes](platform-matrix.md) — disponibilité des fonctionnalités par
  plateforme
- [Dépannage](troubleshooting.md) — erreurs courantes et solutions
