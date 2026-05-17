---
title: Déployer le Video Edit SDK .NET sur Windows — guide complet
description: Configurez paquets NuGet, installateurs silencieux et dépendances runtime pour VisioForge Video Edit SDK .NET sur cibles Windows x86 et x64.
tags:
  - Video Edit SDK
  - .NET
  - DirectShow
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Decoding
  - Editing
  - IP Camera
  - RTSP
  - MP4
  - WebM
  - OGG
  - FLAC
  - Vorbis
  - NuGet

---

# Guide complet de déploiement pour Video Edit SDK .Net

## Introduction au déploiement de VideoEditCore

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

Le VisioForge Video Edit SDK pour .Net fournit un ensemble puissant d'outils pour le traitement, l'édition et l'analyse vidéo en environnement Windows. Ce guide complet détaille les options de déploiement pour garantir le bon fonctionnement du SDK sur les systèmes cibles.

Pour les applications construites avec la configuration AnyCPU, vous devez déployer à la fois les redistribuables x86 et x64 pour garantir la compatibilité entre différentes architectures de processeur. Ce guide couvre toutes les méthodes de déploiement, depuis les simples paquets NuGet jusqu'aux installations manuelles détaillées.

## Vue d'ensemble des options de déploiement

Le SDK propose trois approches principales de déploiement :

1. **Paquets NuGet** : la méthode la plus simple, ne nécessitant pas de privilèges administratifs
2. **Installateurs automatiques** : installation silencieuse avec droits administratifs
3. **Installation manuelle** : déploiement personnalisé avec contrôle granulaire sur les composants

Chaque approche présente des avantages distincts selon les exigences de votre application, votre méthode de distribution et les contraintes de l'environnement cible.

## Déploiement multiplateforme avec VideoEditCoreX

Pour les développeurs recherchant une compatibilité multiplateforme, VisioForge propose le moteur VideoEditCoreX. Cette implémentation moderne prend en charge les environnements Windows, macOS et Linux.

Pour des instructions détaillées sur le déploiement de la version multiplateforme, veuillez consulter notre [guide de déploiement multiplateforme](../deployment-x/index.md) dédié. Le reste de ce document se concentre sur le moteur VideoEditCore spécifique à Windows.

## Moteur VideoEditCore (Windows uniquement)

Le moteur VideoEditCore spécifique à Windows fournit de vastes capacités d'édition vidéo optimisées pour les environnements Windows. Les options complètes de déploiement disponibles sont présentées ci-dessous.

### Déploiement par paquet NuGet (aucun droit administratif requis)

Les paquets NuGet offrent la méthode de déploiement la plus simple, ne nécessitant pas de privilèges administratifs sur le système cible. Cette approche copie automatiquement les fichiers nécessaires dans le dossier de votre application pendant le processus de compilation.

#### Paquets NuGet requis

**Composants de base (toujours requis)** :

- Paquet de base du SDK : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
- Paquet Video Edit SDK : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

**Composants spécifiques au format** :

- Sortie MP4 : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- Sortie WebM : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)
- Formats XIPH (Ogg, Vorbis, FLAC) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

**Composants de sources multimédias** :

- FFMPEG (sortie fichier / diffusion réseau) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
- Source VLC (fichier / caméra IP) : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)
- Filtres LAV : [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) | [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

La mise en œuvre est simple : ajoutez les paquets requis à votre projet d'application, et après la compilation, les fichiers redistribuables nécessaires seront automatiquement inclus dans le dossier de votre application.

### Installateurs silencieux automatiques (droits administratifs requis)

Pour les scénarios où des droits administratifs sont disponibles, les installateurs silencieux fournissent une solution de déploiement rationalisée. Ces installateurs peuvent être intégrés au processus d'installation de votre application pour un déploiement transparent du SDK.

**Composants de base** :

- Paquet de base (toujours requis) : [x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

**Composants de sources multimédias** :

- Paquet FFMPEG : [x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)
- Paquet source VLC : [x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)
- Filtres LAV : [x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

**Composants spécifiques au format** :

- Formats XIPH (Ogg, Vorbis, FLAC) : [x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe) | [x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

**Installation et désinstallation** :

- Pour installer : exécutez l'exécutable approprié avec des privilèges administratifs
- Pour désinstaller : exécutez l'exécutable avec des privilèges administratifs et les paramètres « /x // »
- Les assemblages .NET peuvent être installés dans le Global Assembly Cache (GAC) ou utilisés directement depuis un dossier local

### Installation manuelle (avancée)

L'installation manuelle offre le plus haut niveau de contrôle sur le processus de déploiement. Cette approche est recommandée pour les scénarios avancés où des composants spécifiques doivent être personnalisés ou pour des environnements de déploiement avec des contraintes uniques.

#### Processus d'installation manuelle étape par étape

1. **Dépendances runtime** :
   - Pour les applications avec privilèges administratifs : installez le runtime VC++ 2022 (v143) (x86/x64) et les DLL runtime OpenMP à l'aide de redistribuables exécutables ou de modules MSM
   - Pour les applications sans privilèges administratifs : copiez directement le runtime VC++ 2022 (v143) (x86/x64) et les DLL runtime OpenMP dans le dossier de l'application

2. **Composants de base** :
   - Copiez les DLL VisioForge_MFP/VisioForge_MFPX (ou versions x64) de Redist\Filters dans le dossier de votre application
   - Copiez les assemblages .NET dans le dossier de l'application ou installez-les dans le Global Assembly Cache (GAC)

3. **Filtres DirectShow** :
   - Copiez et enregistrez via COM les filtres DirectShow du SDK à l'aide de [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5) ou d'une méthode équivalente
   - Si l'exécutable de votre application se trouve dans un dossier différent, ajoutez le dossier contenant les filtres à la variable d'environnement système PATH

## Référence des filtres DirectShow essentiels

### Filtres de fonctionnalités principales

**Traitement vidéo de base** :

- VisioForge_Video_Effects_Pro.ax - Traitement d'effets vidéo de base
- VisioForge_Audio_Mixer.ax - Mélange et traitement audio
- VisioForge_MP3_Splitter.ax - Gestion du format MP3
- VisioForge_H264_Decoder.ax - Décodage vidéo H.264

**Traitement audio** :

- VisioForge_Audio_Effects_4.ax - Traitement historique d'effets audio

### Filtres de streaming

**Streaming RTSP** :

- VisioForge_RTSP_Sink.ax - Sortie streaming RTSP
- Tous les filtres MP4 (historique/moderne) sauf le multiplexeur

**Streaming SSF** :

- VisioForge_SSF_Muxer.ax - Multiplexeur format SSF
- Tous les filtres MP4 (historique/moderne) sauf le multiplexeur

**Sources RTSP/RTMP/HTTP** :

- VisioForge_RTSP_Source.ax - Entrée flux RTSP
- VisioForge_RTSP_Source_Live555.ax - RTSP avec bibliothèque Live555
- VisioForge_IP_HTTP_Source.ax - Entrée source HTTP
- Filtres FFMPEG, VLC ou LAV selon les besoins

### Filtres de sources multimédias

**Source VLC** :

- VisioForge_VLC_Source.ax - Entrée multimédia basée sur VLC
- Le déploiement complet nécessite :
  - La copie de tous les fichiers du dossier Redist\VLC
  - L'enregistrement COM des fichiers .ax
  - L'ajout de la variable d'environnement VLC_PLUGIN_PATH pointant vers le dossier VLC\plugins

**Source FFMPEG** :

- VisioForge_FFMPEG_Source.ax - Entrée multimédia basée sur FFMPEG
- Copiez tous les fichiers du dossier Redist\FFMPEG et ajoutez-les au PATH Windows

**Source mémoire** :

- VisioForge_AsyncEx.ax - Entrée source basée sur la mémoire

**Source LAV** :

- Copiez tous les fichiers de Redist\LAV\x86(x64)
- Enregistrez tous les fichiers .ax

### Filtres spécifiques aux formats

**Décodage WebM** :

- VisioForge_WebM_Ogg_Source.ax - Prise en charge des conteneurs WebM/Ogg
- VisioForge_WebM_Source.ax - Source format WebM
- VisioForge_WebM_Split.ax - Démultiplexage WebM
- VisioForge_WebM_Vorbis_Decoder.ax - Décodeur audio Vorbis
- VisioForge_WebM_VP8_Decoder.ax - Décodeur vidéo VP8
- VisioForge_WebM_VP9_Decoder.ax - Décodeur vidéo VP9

**Source FLAC** :

- VisioForge_Xiph_FLAC_Source.ax - Prise en charge du format audio FLAC

**Source Ogg Vorbis** :

- VisioForge_Xiph_Ogg_Demux2.ax - Démultiplexeur de conteneur Ogg
- VisioForge_Xiph_Vorbis_Decoder.ax - Décodeur audio Vorbis

### Filtres de fonctionnalités avancées

**Chiffrement vidéo** :

- VisioForge_Encryptor_v8.ax - Chiffrement version 8
- VisioForge_Encryptor_v9.ax - Chiffrement version 9

**Accélération GPU** :

- VisioForge_DXP.dll / VisioForge_DXP64.dll - Effets GPU DirectX 11

### Enregistrement simplifié des filtres

Pour un enregistrement pratique de plusieurs filtres DirectShow, placez l'utilitaire `reg_special.exe` provenant du redistribuable du SDK dans le dossier contenant les filtres et exécutez-le avec des privilèges administrateur.

## Ressources supplémentaires

Pour des exemples de code et des exemples d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Pour le support technique, les mises à jour de la documentation et les discussions communautaires, visitez le [portail développeur VisioForge](https://support.visioforge.com/).
