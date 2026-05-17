---
title: Déployer une app Video Capture en .NET — NuGet et redists
description: Déployez le VisioForge Video Capture SDK via NuGet, installateur silencieux ou DLL manuel. Guide x86/x64 et moteur X multiplateforme.
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - IP Camera
  - Screen Capture
  - MP4
  - WebM
  - OGG
  - FLAC
  - NuGet

---

# Guide complet de déploiement pour Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Lors du déploiement du Video Capture SDK .Net vers des systèmes où le SDK n'est pas pré-installé, le déploiement correct des composants est essentiel au bon fonctionnement. Pour les applications AnyCPU, les redistribuables x86 et x64 doivent être déployés afin d'assurer la compatibilité avec différentes architectures système.

## Vue d'ensemble des options de moteur

### Moteur VideoCaptureCoreX (compatibilité multiplateforme)

Pour les scénarios de déploiement multiplateforme, consultez notre [guide de déploiement complet](../deployment-x/index.md) qui détaille les exigences spécifiques à chaque plateforme et les options de configuration.

### Moteur VideoCaptureCore (plateforme Windows)

Le moteur VideoCaptureCore est optimisé spécifiquement pour les environnements Windows et offre plusieurs approches de déploiement en fonction des besoins de votre application et des contraintes de l'environnement cible.

## Méthodes de déploiement

### Distribution par paquets NuGet (sans privilèges administrateur)

L'approche par paquets NuGet propose une méthode de déploiement simplifiée qui ne nécessite pas de privilèges administrateur, ce qui la rend idéale pour les environnements restreints ou pour le déploiement vers plusieurs systèmes sans accès élevé.

Ajoutez les paquets NuGet requis à votre projet d'application : après la compilation, les fichiers redistribuables nécessaires seront automatiquement inclus dans le dossier de votre application. Cette méthode simplifie la gestion des dépendances tout en garantissant la disponibilité de tous les composants requis.

#### Paquets NuGet essentiels

**Composants de base (obligatoires) :**

- Paquet de base du SDK : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64)
- Video Capture SDK : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64)

**Paquets spécifiques à certaines fonctionnalités :**

- Intégration FFMPEG (pour la sortie fichier / le streaming réseau) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64)
- Prise en charge de la sortie MP4 : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64)
- Intégration source VLC (pour les sources fichier / caméra IP) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64)
- Format de sortie WebM : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86)
- Prise en charge des formats XIPH (Ogg, Vorbis, FLAC) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64)
- Filtres LAV : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64)
- Prise en charge de la caméra virtuelle : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x86) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VirtualCamera.x64)

> **Note :** lorsque vous utilisez le paquet Virtual Camera, l'enregistrement supplémentaire des fichiers de caméra est requis tel que décrit dans la section Installation manuelle si vous souhaitez que la caméra virtuelle soit accessible depuis des applications externes.

### Déploiement par installateur silencieux (privilèges administrateur requis)

Pour les scénarios où l'accès administrateur est disponible, les installateurs silencieux offrent une approche de déploiement simplifiée qui gère automatiquement l'enregistrement des composants.

**Composants de base :**

- Paquet de base (obligatoire) : [x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)
- Assemblies .NET : peuvent être installés dans le Global Assembly Cache (GAC) ou utilisés depuis un dossier local

**Installateurs spécifiques à certaines fonctionnalités :**

- Intégration FFMPEG : [x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- Prise en charge de la sortie MP4 : [x86](https://files.visioforge.com/redists_net/redist_dotnet_mp4_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_mp4_x64.exe)
- Intégration source VLC : [x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- Prise en charge de formats supplémentaires : WebM ([x86](https://files.visioforge.com/redists_net/redist_dotnet_webm_x86.exe)) et formats XIPH ([x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe))
- Filtres LAV : [x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Note de désinstallation :** pour supprimer le paquet, exécutez l'exécutable d'installation avec les privilèges administrateur en utilisant le paramètre `/x //`.

### Processus d'installation manuelle

Pour un contrôle complet sur le processus de déploiement ou dans les environnements avec des exigences spécifiques, l'installation manuelle offre la plus grande flexibilité :

1. **Dépendances d'exécution :** installez ou copiez les DLL d'exécution VC++ 2022 (v143) (x86/x64) et OpenMP. Avec des droits administrateur, utilisez les redists exe ou les modules MSM ; sinon, copiez-les directement dans le dossier de l'application.

2. **Composants de base :** copiez les DLL `VisioForge_MFP`/`VisioForge_MFPX` (ou versions x64) du dossier `Redist\Filters` vers le répertoire de votre application.

3. **Assemblies .NET :** copiez les assemblies dans le dossier de votre application ou installez-les dans le GAC.

4. **Filtres DirectShow :** copiez les filtres DirectShow du SDK soit dans le dossier de votre application, soit dans un dossier redist dédié (configuré via la propriété `CustomRedist_Path`).

5. **Configuration :** définissez la propriété `CustomRedist_Enabled` à `true` dans l'événement Load de la fenêtre.

6. **Gestion de l'architecture :** pour les filtres LAV (qui utilisent des noms identiques pour les versions x64 et x86), utilisez des dossiers redist distincts pour chaque architecture.

7. **Configuration du chemin :** si votre exécutable d'application réside dans un emplacement différent, ajoutez le dossier des filtres à la variable d'environnement système `PATH`.

#### Composants de base

**Fonctionnalités de base :**

- Filtres de base : VisioForge_BaseFilters.ax / VisioForge_BaseFilters_x64.ax
- Effets vidéo : VisioForge_Video_Effects_Pro.ax / VisioForge_Video_Effects_Pro_x64.ax
- Traitement audio : VisioForge_MP3_Splitter.ax / VisioForge_MP3_Splitter_x64.ax, VisioForge_Audio_Mixer.ax / VisioForge_Audio_Mixer_x64.ax

**Effets audio hérités :**

- VisioForge_Audio_Effects_4.ax / VisioForge_Audio_Effects_4_x64.ax

#### Composants spécifiques à certains formats

**Sortie MP3 :**

- VisioForge_LAME.ax / VisioForge_LAME_x64.ax

**Sortie MP4/M4A :**

- Version héritée : VisioForge_AAC_Encoder.ax, VisioForge_H264_Encoder_XP.ax, VisioForge_MP4_Muxer.ax avec bibliothèques de support
- Version 10 : VisioForge_AAC_Encoder_v10.ax, VisioForge_H264_Encoder.ax, VisioForge_MP4_Muxer_v10.ax avec bibliothèques de support
- Version 11 / Encodage matériel : VisioForge_MFT.dll, VisioForge_MF_Mux.ax (avec variantes x64)

**Sortie WebM :**

- Multiplexeur : VisioForge_WebM_Mux.ax / VisioForge_WebM_Mux_x64.ax
- Encodeurs : VisioForge_WebM_Vorbis_Encoder.ax, VisioForge_WebM_VP8_Encoder.ax
- Amélioration audio : VisioForge_Audio_Enhancer.ax / VisioForge_Audio_Enhancer_x64.ax

**Prise en charge Ogg/FLAC :**

- FLAC : VisioForge_Xiph_FLAC_Encoder.ax / VisioForge_Xiph_FLAC_Encoder_x64.ax
- Ogg Vorbis : VisioForge_Xiph_Ogg_Mux.ax, VisioForge_Xiph_Vorbis_Encoder.ax (avec variantes x64)

#### Composants de streaming et de sources

**Streaming RTSP :**

- VisioForge_RTSP_Sink.ax / VisioForge_RTSP_Sink_x64.ax
- Filtres MP4 (à l'exception du multiplexeur)

**Intégration source VLC :**

- VisioForge_VLC_Source.ax / VisioForge_VLC_Source_x64.ax
- Nécessite de copier tous les fichiers du dossier Redist\VLC, l'enregistrement COM et la configuration appropriée des variables d'environnement

**Intégration FFMPEG :**

- VisioForge_FFMPEG_Source.ax / VisioForge_FFMPEG_Source_x64.ax
- Nécessite tous les fichiers du dossier Redist\FFMPEG et la mise à jour de la variable PATH

**Prise en charge des sources RTSP/RTMP/HTTP :**

- VisioForge_RTSP_Source.ax, VisioForge_RTSP_Source_Live555.ax
- Nécessite FFMPEG, VLC ou les filtres LAV

#### Composants spécialisés

**Capture d'écran :**

- VisioForge_Screen_Capture_DD.ax / VisioForge_Screen_Capture_DD_x64.ax

**Capture audio :**

- VisioForge_WhatYouHear_Source.ax / VisioForge_WhatYouHear_Source_x64.ax

**Caméra virtuelle :**

- VisioForge_Virtual_Camera.ax / VisioForge_Virtual_Camera_x64.ax
- VisioForge_Virtual_Audio_Card.ax / VisioForge_Virtual_Audio_Card_x64.ax

**Traitement vidéo :**

- Push Source : VisioForge_Push_Video_Source.ax / VisioForge_Push_Video_Source_x64.ax
- Streaming réseau : VisioForge_Network_Streamer_Audio.ax, VisioForge_Network_Streamer_Video.ax
- Chiffrement vidéo : plusieurs composants incluant déchiffreurs, encodeurs et bibliothèques de support
- Image dans l'image : VisioForge_Video_Mixer.ax / VisioForge_Video_Mixer_x64.ax

#### Enregistrement des filtres

Pour l'enregistrement COM de tous les filtres DirectShow dans un dossier spécifique, vous pouvez déployer l'utilitaire `reg_special.exe` du SDK dans le répertoire des filtres et l'exécuter avec les privilèges administrateur pour automatiser le processus d'enregistrement.
