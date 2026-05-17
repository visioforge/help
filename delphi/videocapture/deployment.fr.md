---
title: Déploiement de la bibliothèque TVFVideoCapture dans Delphi
description: Déployez TVFVideoCapture dans Delphi — installez les composants, enregistrez les filtres DirectShow et configurez l'environnement pour un déploiement réussi.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Capture
  - Encoding
  - IP Camera
  - MP4
  - MP3
primary_api_classes:
  - TVFVideoCapture

---

# Guide complet de déploiement de la bibliothèque TVFVideoCapture

Lorsque vous distribuez des applications construites avec la bibliothèque TVFVideoCapture, vous devrez déployer plusieurs composants du framework pour garantir un bon fonctionnement sur les systèmes des utilisateurs finaux. Ce guide couvre tous les scénarios de déploiement pour vous aider à créer des installations fiables.

## Vue d'ensemble des options de déploiement

Vous disposez de deux approches principales pour déployer les composants nécessaires : des programmes d'installation automatiques pour un déploiement plus simple, ou une installation manuelle pour des configurations plus personnalisées.

## Programmes d'installation silencieux automatiques (droits d'administrateur requis)

Ces programmes d'installation préconfigurés gèrent automatiquement les dépendances et peuvent être intégrés au processus d'installation de votre application :

### Composants essentiels

- **Paquet de base** (obligatoire pour tous les déploiements) 
  - [Version Delphi](https://files.visioforge.com/redists_delphi/redist_video_capture_base_delphi.exe)
  - [Version ActiveX](https://files.visioforge.com/redists_delphi/redist_video_capture_base_ax.exe)

### Composants de fonctionnalités optionnels

- **Paquet FFMPEG** (requis pour les sources de fichiers ou de caméras IP)
  - [Architecture x86](https://files.visioforge.com/redists_delphi/redist_video_capture_ffmpeg.exe)

- **Prise en charge de la sortie MP4**
  - [Architecture x86](https://files.visioforge.com/redists_delphi/redist_video_capture_mp4.exe)

- **Paquet source VLC** (option alternative pour les sources de fichiers ou de caméras IP)
  - [Architecture x86](https://files.visioforge.com/redists_delphi/redist_video_capture_vlc.exe)

## Processus d'installation manuelle (droits d'administrateur requis)

Pour un meilleur contrôle du processus de déploiement, suivez ces étapes détaillées :

### Étape 1 : installer les dépendances requises

1. Déployer les redistribuables Visual C++ 2010 SP1 :
   - [Architecture x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
   - [Architecture x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

### Étape 2 : déployer les composants principaux

1. Copiez toutes les DLL de Media Foundation Platform (MFP) du répertoire `Redist\Filters` vers le dossier de votre application
2. Pour les implémentations ActiveX : copiez et enregistrez le fichier OCX à l'aide de [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5)

### Étape 3 : enregistrer les filtres DirectShow

À l'aide de [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5), enregistrez ces filtres DirectShow essentiels :

- `VisioForge_Audio_Effects_4.ax`
- `VisioForge_Dump.ax`
- `VisioForge_RGB2YUV.ax`
- `VisioForge_Screen_Capture.ax`
- `VisioForge_Video_Effects_Pro.ax`
- `VisioForge_Video_Mixer.ax`
- `VisioForge_Video_Resize.ax`
- `VisioForge_WavDest.ax`
- `VisioForge_YUV2RGB.ax`
- `VisioForge_FFMPEG_Source.ax`

> **Important :** ajoutez le répertoire des filtres à la variable d'environnement système PATH si l'exécutable de votre application réside dans un dossier différent.

## Installation avancée de composants

### Intégration FFMPEG

1. Copiez tous les fichiers du dossier `Redist\FFMPEG` vers votre déploiement
2. Ajoutez le dossier FFMPEG à la variable PATH du système Windows
3. Enregistrez tous les fichiers .ax du dossier FFMPEG

### Intégration VLC

1. Copiez tous les fichiers du dossier `Redist\VLC`
2. Enregistrez le fichier .ax inclus à l'aide de regsvr32.exe
3. Créez une variable d'environnement nommée `VLC_PLUGIN_PATH` pointant vers le répertoire `VLC\plugins`

### Prise en charge de la sortie audio (LAME)

1. Copiez `lame.ax` du dossier `Redist\Formats`
2. Enregistrez le fichier `lame.ax` à l'aide de regsvr32.exe

### Prise en charge des formats de conteneur

- **Prise en charge WebM :** installez les codecs libres depuis [xiph.org](https://www.xiph.org)
- **Prise en charge Matroska :** déployez le `Haali Matroska Splitter`

### Configuration de la sortie MP4

#### Configuration de l'encodeur moderne

1. Copiez les fichiers de bibliothèque appropriés :
   - `libmfxsw32.dll` (pour les déploiements 32 bits)
   - `libmfxsw64.dll` (pour les déploiements 64 bits)
2. Enregistrez les composants requis :
   - `VisioForge_H264_Encoder.ax`
   - `VisioForge_MP4_Muxer.ax`
   - `VisioForge_AAC_Encoder.ax`
   - `VisioForge_Video_Resize.ax`

#### Configuration de l'encodeur hérité (pour les systèmes plus anciens)

1. Copiez les fichiers de bibliothèque appropriés :
   - `libmfxxp32.dll` (pour les déploiements 32 bits)
   - `libmfxxp64.dll` (pour les déploiements 64 bits)
2. Enregistrez les composants requis :
   - `VisioForge_H264_Encoder_XP.ax`
   - `VisioForge_MP4_Muxer_XP.ax`
   - `VisioForge_AAC_Encoder_XP.ax`
   - `VisioForge_Video_Resize.ax`

## Utilitaire d'enregistrement en masse

Pour simplifier l'enregistrement des filtres DirectShow, vous pouvez utiliser l'utilitaire `reg_special.exe` issu de la configuration du framework. Placez cet exécutable dans votre répertoire de filtres et exécutez-le avec des privilèges d'administrateur pour enregistrer tous les filtres en une seule fois.

---
Pour des exemples de code et d'implémentation supplémentaires, visitez notre [dépôt GitHub](https://github.com/visioforge/). Si vous rencontrez des difficultés lors du déploiement, veuillez contacter le [support technique](https://support.visioforge.com/) pour une assistance personnalisée.
