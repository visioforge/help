---
title: Déployer le lecteur vidéo .NET — paquets NuGet et runtime
description: Configurez les paquets NuGet et les dépendances runtime pour les apps VisioForge Media Player SDK .NET sur Windows et environnements multiplateformes.
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming
  - Decoding
  - IP Camera
  - MP4
  - WebM
  - OGG
  - FLAC
  - Vorbis
  - NuGet

---

# Guide de déploiement de Media Player SDK .Net

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce guide complet couvre tous les scénarios de déploiement pour le Media Player SDK .Net, garantissant que vos applications fonctionnent correctement dans différents environnements. Que vous développiez des applications multiplateformes ou des solutions spécifiques à Windows, ce guide fournit les étapes nécessaires à un déploiement réussi.

## Vue d'ensemble des types de moteur

Le Media Player SDK .Net offre deux types principaux de moteur, chacun conçu pour des scénarios de déploiement spécifiques :

### Moteur MediaPlayerCoreX (multiplateforme)

MediaPlayerCoreX est notre solution multiplateforme qui fonctionne sur plusieurs systèmes d'exploitation. Pour des instructions de déploiement détaillées propres à ce moteur, consultez le [Guide de déploiement multiplateforme](../deployment-x/index.md) principal.

### Moteur MediaPlayerCore (Windows uniquement)

Le moteur MediaPlayerCore est optimisé spécifiquement pour les environnements Windows. Lors du déploiement d'applications qui utilisent ce moteur sur des ordinateurs sans le SDK préinstallé, vous devez inclure les composants SDK nécessaires avec votre application.

> **Important** : pour les applications AnyCPU, vous devez déployer à la fois les redistribuables x86 et x64 pour garantir la compatibilité entre les différentes architectures système.

## Options de déploiement

Il existe trois méthodes principales pour déployer les composants Media Player SDK .Net :

1. Utilisation des paquets NuGet (recommandé pour la plupart des scénarios)
2. Utilisation des programmes d'installation silencieux automatiques (nécessite des privilèges administratifs)
3. Installation manuelle (pour un contrôle total du processus de déploiement)

## Déploiement par paquets NuGet

Les paquets NuGet fournissent la méthode de déploiement la plus simple, gérant automatiquement l'inclusion des fichiers nécessaires dans le dossier de votre application durant le processus de build.

### Paquets NuGet requis

#### Paquets principaux (toujours requis)

* **Paquet de base du SDK** :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.Base.x64/)
* **Paquet Media Player SDK** :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/)

#### Paquets propres aux fonctionnalités (ajouter selon les besoins)

##### Prise en charge des formats multimédias

* **Paquet FFMPEG** (pour la lecture de fichiers en mode source FFMPEG) :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)
* **Paquet de sortie MP4** :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
* **Paquet de sortie WebM** :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.WebM.x86/)

##### Prise en charge des sources

* **Paquet source VLC** (pour les sources fichier / caméra IP) :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VLC.x64/)

##### Prise en charge des formats audio

* **Paquet de formats XIPH** (sortie/source Ogg, Vorbis, FLAC) :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.XIPH.x64/)

##### Prise en charge des filtres

* **Paquet de filtres LAV** :
  * [Version x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/)
  * [Version x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Programmes d'installation silencieux automatiques

Pour les scénarios où vous préférez un déploiement basé sur un installeur, le SDK propose des installeurs silencieux automatiques qui nécessitent des privilèges administratifs.

### Installeurs disponibles

#### Composants principaux

* **Paquet de base** (toujours requis) :
  * [Installeur x86](https://files.visioforge.com/redists_net/redist_dotnet_base_x86.exe)
  * [Installeur x64](https://files.visioforge.com/redists_net/redist_dotnet_base_x64.exe)

#### Prise en charge des formats multimédias

* **Paquet FFMPEG** (pour les sources fichier / caméra IP) :
  * [Installeur x86](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x86.exe)
  * [Installeur x64](https://files.visioforge.com/redists_net/redist_dotnet_ffmpeg_x64.exe)

#### Prise en charge des sources

* **Paquet source VLC** (pour les sources fichier / caméra IP) :
  * [Installeur x86](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x86.exe)
  * [Installeur x64](https://files.visioforge.com/redists_net/redist_dotnet_vlc_x64.exe)

#### Prise en charge des formats audio

* **Paquet de formats XIPH** (sortie/source Ogg, Vorbis, FLAC) :
  * [Installeur x86](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x86.exe)
  * [Installeur x64](https://files.visioforge.com/redists_net/redist_dotnet_xiph_x64.exe)

#### Prise en charge des filtres

* **Paquet de filtres LAV** :
  * [Installeur x86](https://files.visioforge.com/redists_net/redist_dotnet_lav_x86.exe)
  * [Installeur x64](https://files.visioforge.com/redists_net/redist_dotnet_lav_x64.exe)

> **Note** : pour désinstaller un paquet installé, exécutez l'exécutable avec des privilèges administratifs en utilisant les paramètres : `/x //`

## Installation manuelle

Pour les scénarios de déploiement avancés nécessitant un contrôle précis de l'installation des composants, suivez ces étapes :

### Étape 1 : Dépendances runtime

* **Avec privilèges administratifs** : installez le runtime VC++ 2022 (v143) (x86/x64) et les DLL runtime OpenMP à l'aide d'exécutables redistribuables ou de modules MSM.
* **Sans privilèges administratifs** : copiez le runtime VC++ 2022 (v143) (x86/x64) et les DLL runtime OpenMP directement dans le dossier de votre application.

### Étape 2 : Composants principaux

* Copiez les DLL VisioForge_MFP/VisioForge_MFPX (ou versions x64) depuis le répertoire Redist\Filters dans le dossier de votre application.

### Étape 3 : Assemblies .NET

* Copiez les assemblies .NET dans le dossier de votre application ou installez-les dans le Global Assembly Cache (GAC).

### Étape 4 : Filtres DirectShow

* Copiez et enregistrez via COM les filtres DirectShow du SDK en utilisant [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5) ou une autre méthode appropriée.

### Étape 5 : Configuration de l'environnement

* Ajoutez le dossier contenant les filtres à la variable d'environnement système PATH si votre exécutable d'application se trouve dans un répertoire différent.

## Configuration des filtres DirectShow

Le SDK utilise divers filtres DirectShow pour des fonctionnalités spécifiques. Voici une liste complète organisée par catégorie de fonctionnalité :

### Filtres de fonctionnalités de base

* VisioForge_Video_Effects_Pro.ax
* VisioForge_MP3_Splitter.ax
* VisioForge_H264_Decoder.ax
* VisioForge_Audio_Mixer.ax

### Filtres d'effets audio

* VisioForge_Audio_Effects_4.ax (effets audio hérités)

### Filtres de prise en charge du streaming

#### Streaming RTSP

* VisioForge_RTSP_Sink.ax
* Filtres MP4 (hérités/modernes, hors multiplexeur)

#### Streaming SSF

* VisioForge_SSF_Muxer.ax
* Filtres MP4 (hérités/modernes, hors multiplexeur)

### Filtres source

#### Source VLC

* VisioForge_VLC_Source.ax
* Dossier complet Redist\VLC avec enregistrement COM
* Variable d'environnement VLC_PLUGIN_PATH pointant vers le dossier VLC\plugins

#### Source FFMPEG

* VisioForge_FFMPEG_Source.ax
* Dossier complet Redist\FFMPEG, ajouté à la variable PATH de Windows

#### Source mémoire

* VisioForge_AsyncEx.ax

#### Décodage WebM

* VisioForge_WebM_Ogg_Source.ax
* VisioForge_WebM_Source.ax
* VisioForge_WebM_Split.ax
* VisioForge_WebM_Vorbis_Decoder.ax
* VisioForge_WebM_VP8_Decoder.ax
* VisioForge_WebM_VP9_Decoder.ax

#### Sources de streaming réseau

* VisioForge_RTSP_Source.ax
* VisioForge_RTSP_Source_Live555.ax
* Filtres FFMPEG, VLC ou LAV

#### Sources de format audio

* VisioForge_Xiph_FLAC_Source.ax (source FLAC)
* VisioForge_Xiph_Ogg_Demux2.ax (source Ogg Vorbis)
* VisioForge_Xiph_Vorbis_Decoder.ax (source Ogg Vorbis)

### Filtres de fonctionnalités spéciales

#### Chiffrement vidéo

* VisioForge_Encryptor_v8.ax
* VisioForge_Encryptor_v9.ax

#### Accélération GPU

* VisioForge_DXP.dll / VisioForge_DXP64.dll (effets vidéo GPU DirectX 11)

#### Source LAV

* Contenu complet de redist\LAV\x86(x64), avec tous les fichiers .ax enregistrés

### Astuce pour l'enregistrement des filtres

Pour simplifier le processus d'enregistrement COM de tous les filtres DirectShow d'un répertoire, placez le fichier « reg_special.exe » du redist du SDK dans le dossier des filtres et exécutez-le avec des privilèges administratifs.

---
Pour plus d'exemples de code, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
