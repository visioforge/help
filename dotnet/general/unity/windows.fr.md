---
title: Compiler un jeu Unity avec lecture vidéo pour Windows x64
description: Réglages de build et dépannage pour VisioForge Media Blocks SDK .NET dans Unity 6 sur Windows x64 — Éditeur et Standalone.
sidebar_label: Compilation pour Windows
order: 53
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Standalone Player
  - C#
primary_api_classes:
  - VisioForgeEnvironment
  - MediaBlocksPipeline
---

# Compilation pour Windows

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }
[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button target="_blank" }
[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button target="_blank" }
[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button target="_blank" }

Windows x64 est la cible de base du paquet Unity — il est livré dans chaque variante
`.unitypackage` que produit le pipeline de build. Cette page couvre les Player Settings,
l'organisation sur disque et les problèmes propres à Windows. Pour le reste, consultez
[Démarrage et cycle de vie](bootstrap.md).

## Player Settings

| Réglage | Valeur | Où |
|---|---|---|
| Architecture | **x86_64** | File → Build Profiles → Windows |
| Target Platform | **Windows** | File → Build Profiles → Windows |
| Api Compatibility Level | **.NET Standard 2.1** | Project Settings → Player → Other Settings → Configuration |
| Scripting Backend | **Mono** *(IL2CPP fonctionne aussi ; Mono est par défaut sur Windows)* | Project Settings → Player → Other Settings → Configuration |
| Enter Play Mode → Reload Domain | **Off** | Project Settings → Editor → Enter Play Mode Settings |

Si vous avez importé le paquet via le dialogue **Apply**, les deux réglages de projet
obligatoires (`.NET Standard 2.1` et Disable Domain Reload) sont déjà en place.

## Organisation sur disque

L'étape de build `deploy-unity-natives.ps1` met en place le runtime Windows dans votre projet
comme suit :

| Chemin | Contenu |
|---|---|
| `Assets/StreamingAssets/VisioForge/x64/` | Layout plat : libs core GStreamer, chaque DLL de plugin, modules GIO, `ca-certificates.crt`. ~300 fichiers. |
| `Assets/Plugins/` | Assemblies managés (`VisioForge.Core.dll`, `VisioForge.Libs.dll`, `GstSharp.dll`, `GLibSharp.dll`, etc.) avec leurs `.meta`. |
| `Assets/Scripts/` | Les helpers partagés (`VisioForgeEnvironment.cs`, `VisioForgeVideoView.cs`) plus les six scripts d'exemple (`MediaBlocksPlayer.cs`, `RTSPViewerPlayer.cs`, `MediaPlayerXPlayer.cs`, `IPCameraXViewer.cs`, `VideoCaptureXRecorder.cs`, `VideoEditXRenderer.cs`). |
| `Assets/Scenes/` | Les six scènes d'exemple : `SimplePlayer.unity`, `RTSPViewer.unity`, `MediaPlayerX.unity`, `IPCameraX.unity`, `VideoCaptureX.unity`, `VideoEditX.unity`. |

`StreamingAssets` (et non `Plugins/x64`) est utilisé parce qu'Unity copie le dossier
littéralement dans un build Standalone, ce qui est exactement où `VisioForgeEnvironment.Configure()`
pointe ensuite le chargeur DLL. Le même chemin résout à la fois dans l'Éditeur et dans le
player — `Application.streamingAssetsPath` retourne le `Assets/StreamingAssets` du projet
dans l'Éditeur et `<game>_Data/StreamingAssets` dans le player.

## Build Standalone Player

**File → Build Profiles → Windows → x86_64 → Build** produit un player autonome. Sans étapes
supplémentaires :

- Unity copie `Assets/StreamingAssets/VisioForge/x64/` vers
  `<game>_Data/StreamingAssets/VisioForge/x64/` automatiquement.
- Les plugins managés de `Assets/Plugins/` sont stagés dans `<game>_Data/Managed/`.
- `VisioForgeEnvironment.Configure()` s'exécute `BeforeSceneLoad` et pointe `SetDllDirectoryW`
  vers le dossier de natifs stagé.

Le `<game>.exe` résultant et son dossier `_Data/` constituent tout l'artefact livrable.

## Taille sur disque

Le runtime Windows ajoute environ **50 Mo** de bibliothèques natives à votre build (jusqu'à
~30 Mo si vous excluez libav avec `-SkipLibav` lors de la reconstruction du paquet). Les
assemblies managés ajoutent ~5 Mo supplémentaires.

## Dépannage

| Symptôme | Cause | Solution |
|---|---|---|
| `DllNotFoundException: gstreamer-1.0-0` sur Play | `Assets/StreamingAssets/VisioForge/x64/` manque ou est vide. | Réimportez le `.unitypackage` avec tous les éléments cochés, ou inspectez la ligne `[VisioForge] Native runtime not found at …` de la Console pour le chemin résolu. |
| Le pipeline se bloque au démarrage, le log montre deux appels `gst_init` | Une installation GStreamer dans le `PATH` système charge une deuxième copie de `gstreamer-1.0-0.dll`. | `Configure()` nettoie déjà le `PATH` — confirmez en inspectant la Console : le nombre d'entrées supprimées est journalisé. Si le compteur est 0 mais le symptôme persiste, un lanceur externe réinjecte GStreamer après `Configure()`. |
| `TypeLoadException` au premier appel SDK | Api Compatibility Level est `.NET Framework` au lieu de `.NET Standard 2.1`. | Réglez-le sur `.NET Standard 2.1` (Project Settings → Player → Other Settings → Configuration → Api Compatibility Level). |
| Les streams RTSPS / HTTPS échouent à la connexion avec erreur TLS | `SSL_CERT_FILE` ne pointe pas vers le bundle CA fourni. | `Configure()` le règle quand `ca-certificates.crt` est présent dans le dossier des natifs. Un bundle CA absent est journalisé en avertissement — restagez le runtime via `deploy-unity-natives.ps1`. |
| L'Éditeur se bloque sur "Reloading domain" après Play/Stop | Disable Domain Reload a été remis sur off. | Réactivez-le dans Project Settings → Editor → Enter Play Mode Settings (réglez **When entering Play Mode** sur une option qui ne recharge pas). |

## Foire aux questions

### Puis-je utiliser IL2CPP sur Windows ?

Oui — ça compile et ça tourne. Mono est par défaut et c'est ce que la matrice CI du paquet
exerce. Passez à IL2CPP seulement si vous avez une raison à l'échelle du projet (autres
plugins qui le requièrent, surface de déploiement plus petite). Le même `link.xml` que le
paquet fournit préserve les types managés du SDK contre le stripping IL2CPP sur n'importe
quel backend.

### Windows 11 ARM64 fonctionne-t-il ?

Pas depuis ce `.unitypackage`. Le runtime natif fourni est x86_64 seulement — l'exécuter via
émulation x64 sous ARM64 n'est pas pris en charge. Un build natif ARM64 ne fait pas partie du
paquet Unity actuel.

### Le SDK a-t-il besoin de droits administrateur ?

Non. Tout tourne depuis le dossier du projet / le dossier `_Data` du player. Le runtime ne
touche aucune clé de registre, n'installe aucun pilote global et n'écrit que dans
`Application.persistentDataPath` (pour les logs / fichiers temporaires quand activés).

## Voir aussi

- [Installer Media Blocks SDK dans Unity](../../install/unity.md) — configuration du paquet
- [Démarrage et cycle de vie](bootstrap.md) — comment `Configure()` démarre le runtime Windows
- [Lire un fichier multimédia dans Unity](simple-player.md) — l'exemple `SimplePlayer`
- [Voir une caméra RTSP dans Unity](rtsp-viewer.md) — l'exemple `RTSPViewer`
- [Dépannage](troubleshooting.md) — erreurs runtime courantes sur toutes les plateformes
