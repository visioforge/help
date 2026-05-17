---
title: Déployer TVFVideoEdit en applications Delphi et ActiveX
description: Déployez TVFVideoEdit dans des applications Delphi et ActiveX avec installateurs automatiques ou configuration manuelle des composants et dépendances.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Encoding
  - Editing
  - IP Camera
  - MP4
  - WebM
  - H.264
  - AAC
primary_api_classes:
  - TVFVideoEdit

---

# Guide de déploiement de la bibliothèque TVFVideoEdit

## Introduction

La bibliothèque TVFVideoEdit fournit de puissantes capacités d'édition vidéo pour vos applications Delphi et ActiveX. Ce guide explique comment déployer correctement tous les composants nécessaires pour que votre application fonctionne sur les systèmes des utilisateurs finaux sans nécessiter l'environnement de développement complet.

## Options de déploiement

Vous disposez de deux méthodes principales pour déployer les composants de la bibliothèque TVFVideoEdit : les installateurs automatiques ou l'installation manuelle. Chaque approche présente des avantages spécifiques selon vos besoins de distribution.

### Installateurs silencieux automatiques

Pour un déploiement simplifié, nous proposons des paquets d'installation silencieuse qui gèrent l'installation de tous les composants nécessaires sans interaction de l'utilisateur :

#### Paquet de base requis

* **Composants de base** (toujours requis) :
  * [Version Delphi](https://files.visioforge.com/redists_delphi/redist_video_edit_base_delphi.exe)
  * [Version ActiveX](https://files.visioforge.com/redists_delphi/redist_video_edit_base_ax.exe)

#### Paquets de fonctionnalités facultatives

* **Paquet FFMPEG** (requis pour la prise en charge des fichiers et des caméras IP (uniquement pour le moteur source FFMPEG)) :
  * [Architecture x86](https://files.visioforge.com/redists_delphi/redist_video_edit_ffmpeg.exe)

* **Paquet de sortie MP4** (pour la création de vidéo MP4) :
  * [Architecture x86](https://files.visioforge.com/redists_delphi/redist_video_edit_mp4.exe)

### Processus d'installation manuelle

Dans les situations où vous avez besoin d'un contrôle précis sur le déploiement des composants, suivez ces étapes détaillées :

1. **Installer les dépendances Visual C++**
   * Installez le redistribuable VC++ 2010 SP1 :
     * [Version x86](https://files.visioforge.com/shared/vcredist_2010_x86.exe)
     * [Version x64](https://files.visioforge.com/shared/vcredist_2010_x64.exe)

2. **Déployer les composants centraux Media Foundation**
   * Copiez toutes les DLL MFP du répertoire `Redist\Filters` vers le dossier de votre application

3. **Enregistrer les filtres DirectShow**
   * Copiez et enregistrez en COM ces filtres DirectShow essentiels à l'aide de [regsvr32.exe](https://support.microsoft.com/en-us/topic/how-to-use-the-regsvr32-tool-and-troubleshoot-regsvr32-error-messages-a98d960a-7392-e6fe-d90a-3f4e0cb543e5).
   * **Architecture :** le nom de fichier nu correspond au filtre **x86 (32 bits)** ; le filtre **x64 (64 bits)** correspondant porte le suffixe `_x64` (par exemple `VisioForge_Video_Effects_Pro_x64.ax`). Les deux sont livrés dans le redistribuable ; déployez et enregistrez la variante qui correspond à l'architecture cible de votre application. Les applications 64 bits doivent enregistrer les variantes `_x64`.
     * `VisioForge_Audio_Effects_4.ax`
     * `VisioForge_Dump.ax`
     * `VisioForge_RGB2YUV.ax`
     * `VisioForge_Video_Effects_Pro.ax`
     * `VisioForge_Video_Mixer.ax`
     * `VisioForge_Video_Resize.ax`
     * `VisioForge_WavDest.ax`
     * `VisioForge_YUV2RGB.ax`
     * `VisioForge_FFMPEG_Source.ax`

   La fonctionnalité de capture d'écran (auparavant livrée sous le nom `VisioForge_Screen_Capture.ax`) fait désormais partie des filtres de base et ne nécessite plus d'étape d'enregistrement distincte.

4. **Configurer les paramètres de chemin**
   * Ajoutez le dossier contenant ces filtres à la variable d'environnement système `PATH` si l'exécutable de votre application réside dans un répertoire différent

## Installation de composants supplémentaires

### Intégration FFMPEG

Pour activer la prise en charge avancée des formats multimédias :

* Copiez tous les fichiers du dossier `Redist\FFMPEG`
* Ajoutez ce dossier à la variable `PATH` du système Windows
* Enregistrez tous les fichiers .ax du dossier `Redist\FFMPEG`

### Prise en charge VLC

Pour une compatibilité de formats étendue :

* Copiez tous les fichiers du dossier `Redist\VLC`
* Enregistrez en COM le fichier .ax à l'aide de regsvr32.exe
* Créez une variable d'environnement nommée `VLC_PLUGIN_PATH`
* Définissez sa valeur de manière à pointer vers le dossier `VLC\plugins`

### Prise en charge de la sortie audio

Pour les capacités d'encodage MP3 :

* Copiez le fichier lame.ax depuis le dossier `Redist\Formats`
* Enregistrez le fichier lame.ax à l'aide de regsvr32.exe

### Prise en charge du format WebM

Pour l'encodage et le décodage WebM :

* Installez les codecs gratuits nécessaires disponibles sur le [site xiph.org](https://www.xiph.org/dshow/)

### Prise en charge du conteneur Matroska

Pour la compatibilité avec le format MKV :

* Installez [Haali Matroska Splitter](https://haali.net/mkv/) pour un encodage et un décodage corrects

### Sortie MP4 H264/AAC — encodeur moderne

Pour la création de MP4 de haute qualité avec des codecs modernes :

* Copiez les fichiers `libmfxsw32.dll` / `libmfxsw64.dll`
* Enregistrez ces filtres DirectShow :
  * `VisioForge_H264_Encoder.ax`
  * `VisioForge_MP4_Muxer.ax`
  * `VisioForge_AAC_Encoder.ax`
  * `VisioForge_Video_Resize.ax`

### Sortie MP4 H264/AAC — encodeur hérité

Pour la compatibilité avec les systèmes plus anciens :

* Copiez les fichiers `libmfxxp32.dll` / `libmfxxp64.dll`
* Enregistrez ces filtres DirectShow :
  * `VisioForge_H264_Encoder_XP.ax`
  * `VisioForge_MP4_Muxer_XP.ax`
  * `VisioForge_AAC_Encoder_XP.ax`
  * `VisioForge_Video_Resize.ax`

## Utilitaire d'enregistrement en lot

Pour simplifier le processus d'enregistrement de plusieurs filtres DirectShow :

* Placez l'utilitaire `reg_special.exe` du paquet redistribuable dans le dossier contenant vos filtres
* Exécutez-le avec des privilèges d'administrateur pour enregistrer tous les filtres compatibles dans ce répertoire

## Conseils de dépannage

Les problèmes courants lors du déploiement incluent souvent :

* Des dépendances manquantes
* Un enregistrement incorrect des composants COM
* Des problèmes de configuration des chemins
* Des permissions utilisateur insuffisantes

Assurez-vous que tous les fichiers requis sont correctement déployés et enregistrés avant de lancer votre application.

---
Veuillez contacter [notre équipe de support](https://support.visioforge.com/) si vous rencontrez le moindre problème avec ce processus de déploiement. Visitez notre [dépôt GitHub](https://github.com/visioforge/) pour des exemples de code supplémentaires et des exemples d'implémentation.