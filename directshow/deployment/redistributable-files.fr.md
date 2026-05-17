---
title: Fichiers redistribuables DirectShow pour déploiement
description: Liste complète des fichiers redistribuables pour les SDK DirectShow VisioForge avec dépendances, fichiers par architecture et exigences de déploiement.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - Decoding
  - Mixing
  - Screen Capture
  - WebM
  - H.264
  - MP3

---

# SDK DirectShow — Référence des fichiers redistribuables

## Vue d'ensemble

Ce document fournit la liste complète des fichiers requis pour redistribuer chaque SDK DirectShow avec votre application. Tous ces fichiers doivent être inclus dans votre installeur ou paquet de déploiement.

---
## FFMPEG Source DirectShow Filter { #ffmpeg-source-filter }
### Fichiers principaux
#### x86 (32 bits)
**Filtre** :
- `VisioForge_FFMPEG_Source.ax` — filtre DirectShow principal
**Bibliothèques FFmpeg** (requises) :
- `avcodec-58.dll` — bibliothèque de codecs vidéo/audio
- `avdevice-58.dll` — gestion des périphériques
- `avfilter-7.dll` — filtrage audio/vidéo
- `avformat-58.dll` — gestion des formats de conteneur
- `avutil-56.dll` — fonctions utilitaires
- `swresample-3.dll` — rééchantillonnage audio
- `swscale-5.dll` — mise à l'échelle vidéo et conversion colorimétrique
**Taille totale** : ~80-100 Mo
#### x64 (64 bits)
**Filtre** :
- `VisioForge_FFMPEG_Source_x64.ax` — filtre DirectShow principal (64 bits)
**Bibliothèques FFmpeg** (requises) :
- `avcodec-58.dll` — version 64 bits
- `avdevice-58.dll` — version 64 bits
- `avfilter-7.dll` — version 64 bits
- `avformat-58.dll` — version 64 bits
- `avutil-56.dll` — version 64 bits
- `swresample-3.dll` — version 64 bits
- `swscale-5.dll` — version 64 bits
**Taille totale** : ~90-110 Mo
### Structure de répertoire d'installation { #installation-directory-structure }
```
YourApp\
├── VisioForge_FFMPEG_Source.ax          (x86)
├── VisioForge_FFMPEG_Source_x64.ax      (x64)
├── avcodec-58.dll
├── avdevice-58.dll
├── avfilter-7.dll
├── avformat-58.dll
├── avutil-56.dll
├── swresample-3.dll
└── swscale-5.dll
```
### Fichiers de licence
- `license.rtf` — contrat de licence du SDK (à inclure dans l'installeur)
### Dépendances
- **Redistribuable Visual C++ 2015-2022** (x86 ou x64)
  - Téléchargement : https://aka.ms/vs/17/release/vc_redist.x64.exe
---

## VLC Source DirectShow Filter

### Fichiers principaux

#### x86 (32 bits) uniquement

**Filtre** :
- `VisioForge_VLC_Source.ax` — filtre DirectShow principal

**Bibliothèques VLC** (requises) :
- `libvlc.dll` — bibliothèque cœur VLC
- `libvlccore.dll` — fonctionnalités cœur de VLC

**Répertoire de plugins VLC** (requis) :
- `plugins\` — dossier complet des plugins VLC (~100+ DLL de plugins)
  - `plugins\access\` — protocoles d'entrée
  - `plugins\audio_filter\` — traitement audio
  - `plugins\audio_mixer\` — mixage audio
  - `plugins\audio_output\` — sortie audio
  - `plugins\codec\` — codecs
  - `plugins\control\` — interfaces de contrôle
  - `plugins\demux\` — démultiplexeurs
  - `plugins\misc\` — divers
  - `plugins\packetizer\` — packetiseurs
  - `plugins\services_discovery\` — découverte de services
  - `plugins\stream_filter\` — filtres de flux
  - `plugins\stream_out\` — sortie de flux
  - `plugins\text_renderer\` — rendu de texte
  - `plugins\video_chroma\` — conversion colorimétrique
  - `plugins\video_filter\` — filtres vidéo
  - `plugins\video_output\` — sortie vidéo
  - `plugins\visualization\` — visualisations

**Répertoires de données VLC** :
- `locale\` — fichiers de localisation (optionnel, ~50+ dossiers de langues)
- `lua\` — scripts Lua pour les listes de lecture et extensions
- `hrtfs\` — fichiers audio HRTF
  - `dodeca_and_7channel_3DSL_HRTF.sofa`

**Taille totale** : ~150-200 Mo (avec tous les plugins et locales)

### Structure de répertoire d'installation

```
YourApp\
├── VisioForge_VLC_Source.ax
├── libvlc.dll
├── libvlccore.dll
├── plugins\
│   ├── access\
│   ├── audio_filter\
│   ├── codec\
│   └── ... (tous les repertoires de plugins)
├── locale\           (optionnel)
├── lua\
└── hrtfs\
```

### Fichiers de licence

- `license.rtf` — contrat de licence du SDK

### Dépendances

- **Redistribuable Visual C++ 2015-2022** (x86)

### Remarques importantes

- **Tous les plugins VLC doivent être inclus** — l'absence de plugins entraînera des échecs de lecture pour certains formats
- **Conservez la structure de répertoire** — VLC attend les plugins dans le sous-répertoire `plugins\`
- **Pas de version x64** — le VLC Source Filter est uniquement 32 bits

---
## Processing Filters Pack
### Filtres principaux
#### x86 (32 bits)
**Traitement vidéo** :
- `VisioForge_Video_Effects_Pro.ax` — filtre d'effets vidéo (35+ effets)
- `VisioForge_Video_Mixer.ax` — mélangeur vidéo multi-source
- `VisioForge_Screen_Capture_DD.ax` — capture d'écran DirectDraw
**Traitement audio** :
- `VisioForge_Audio_Enhancer.ax` — filtre d'amélioration audio
- `VisioForge_Audio_Effects_4.ax` — effets audio (optionnel)
- `VisioForge_Audio_Mixer.ax` — mélangeur audio
**Filtres de base** (requis) :
- `VisioForge_BaseFilters.ax` — bibliothèque de filtres de base
- `VisioForge_AsyncEx.ax` — lecteur de fichier asynchrone (optionnel)
**Bibliothèques utilitaires** :
- `VisioForge_MFP.dll` — utilitaire Media Foundation
- `VisioForge_MFPX.dll` — fonctions MF étendues
#### x64 (64 bits)
**Traitement vidéo** :
- `VisioForge_Video_Effects_Pro_x64.ax`
- `VisioForge_Video_Mixer_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Traitement audio** :
- `VisioForge_Audio_Enhancer_x64.ax`
- `VisioForge_Audio_Mixer_x64.ax`
**Filtres de base** (requis) :
- `VisioForge_BaseFilters_x64.ax`
- `VisioForge_AsyncEx_x64.ax` (optionnel)
**Bibliothèques utilitaires** :
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### LAV Filters (optionnels mais recommandés)
LAV Filters fournit la prise en charge de codecs supplémentaires et est inclus avec le Processing Filters Pack.
#### x86
**LAV Filters** :
- `LAVSplitter.ax` — séparateur source
- `LAVVideo.ax` — décodeur vidéo
- `LAVAudio.ax` — décodeur audio
**Bibliothèques FFmpeg pour LAV** :
- `avcodec-lav-58.dll`
- `avformat-lav-58.dll`
- `avfilter-lav-7.dll`
- `avresample-lav-4.dll`
- `avutil-lav-56.dll`
- `swscale-lav-5.dll`
**Bibliothèques supplémentaires** :
- `libbluray.dll` — prise en charge Blu-ray
- `IntelQuickSyncDecoder.dll` — décodage matériel Intel QuickSync
**Manifeste** :
- `LAVFilters.Dependencies.manifest`
**Licence** :
- `COPYING` — licence LAV Filters (LGPL)
#### x64
Mêmes fichiers que x86 mais en versions 64 bits.
### Structure de répertoire d'installation
```
YourApp\
├── Filters\
│   ├── VisioForge_Video_Effects_Pro.ax
│   ├── VisioForge_Video_Effects_Pro_x64.ax
│   ├── VisioForge_Video_Mixer.ax
│   ├── VisioForge_Video_Mixer_x64.ax
│   ├── VisioForge_Audio_Enhancer.ax
│   ├── VisioForge_Audio_Enhancer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   └── VisioForge_MFPX64.dll
└── LAV\
    ├── x86\
    │   ├── LAVSplitter.ax
    │   ├── LAVVideo.ax
    │   ├── LAVAudio.ax
    │   ├── avcodec-lav-58.dll
    │   └── ... (autres fichiers LAV)
    └── x64\
        ├── LAVSplitter.ax
        ├── LAVVideo.ax
        └── ... (autres fichiers LAV)
```
### Fichiers de licence
- `license.rtf` — licence du SDK VisioForge
- `VisioForge_AsyncEx_license.htm` — licence du filtre Async
- `VisioForge_Audio_Effects_4_note.txt` — notes sur les effets audio
- `COPYING` — licence LAV Filters (dans le répertoire LAV)
### Taille totale
- **Sans LAV Filters** : ~20-30 Mo
- **Avec LAV Filters** : ~80-100 Mo
---

## Encoding Filters Pack

### Filtres principaux

#### x86 (32 bits)

**Encodeurs vidéo** :
- `VisioForge_NVENC.ax` — encodeur matériel NVIDIA
- `VisioForge_H264_Encoder.ax` — encodeur logiciel H.264
- `VisioForge_H264_Encoder_v9.ax` — encodeur H.264 v9
- `VisioForge_H264_Decoder.ax` — décodeur H.264
- `VisioForge_WebM_VP8_Encoder.ax` — encodeur VP8
- `VisioForge_WebM_VP9_Encoder.ax` — encodeur VP9 (dans x64)
- `VisioForge_WebM_VP8_Decoder.ax` — décodeur VP8
- `VisioForge_WebM_VP9_Decoder.ax` — décodeur VP9

**Encodeurs audio** :
- `VisioForge_AAC_Encoder.ax` — encodeur AAC
- `VisioForge_AAC_Encoder_v10.ax` — encodeur AAC v10
- `VisioForge_LAME.ax` — encodeur MP3 (LAME)
- `VisioForge_WebM_Vorbis_Encoder.ax` — encodeur Vorbis
- `VisioForge_WebM_Vorbis_Decoder.ax` — décodeur Vorbis

**Multiplexeurs/démultiplexeurs** :
- `VisioForge_MP4_Muxer.ax` — multiplexeur de conteneur MP4
- `VisioForge_MP4_Muxer_v10.ax` — multiplexeur MP4 v10
- `VisioForge_MF_Mux.ax` — multiplexeur Media Foundation
- `VisioForge_WebM_Mux.ax` — multiplexeur WebM
- `VisioForge_WebM_Split.ax` — séparateur WebM
- `VisioForge_WebM_Source.ax` — source WebM
- `VisioForge_WebM_Ogg_Source.ax` — source Ogg
- `VisioForge_SSF_Muxer.ax` — multiplexeur SSF

**Réseau** :
- `VisioForge_RTSP_Sink.ax` — puits RTSP
- `VisioForge_RTSP_Source_Live555.ax` — source RTSP

**Filtres de base** (requis) :
- `VisioForge_BaseFilters.ax`

**Bibliothèques utilitaires** (requises) :
- `VisioForge_MFP.dll` — utilitaire Media Foundation
- `VisioForge_MFP64.dll` — utilitaire MF 64 bits
- `VisioForge_MFPX.dll` — fonctions MF étendues
- `VisioForge_MFPX64.dll` — MF étendu 64 bits
- `VisioForge_MFT.dll` — Media Foundation Transform

**Intel QuickSync** (optionnel) :
- `libmfxsw32.dll` — bibliothèque logicielle QuickSync
- `libmfxxp32.dll` — bibliothèque QuickSync XP

#### x64 (64 bits)

**Encodeurs vidéo** :
- `VisioForge_NVENC_x64.ax`
- `VisioForge_H264_Encoder_x64.ax`
- `VisioForge_H264_Encoder_v9_x64.ax`
- `VisioForge_H264_Decoder_x64.ax`
- `VisioForge_WebM_VP8_Encoder_x64.ax`
- `VisioForge_WebM_VP9_Encoder_x64.ax`
- `VisioForge_WebM_VP8_Decoder_x64.ax`
- `VisioForge_WebM_VP9_Decoder_x64.ax`

**Encodeurs audio** :
- `VisioForge_AAC_Encoder_x64.ax`
- `VisioForge_AAC_Encoder_v10_x64.ax`
- `VisioForge_LAME_x64.ax`
- `VisioForge_WebM_Vorbis_Encoder_x64.ax`
- `VisioForge_WebM_Vorbis_Decoder_x64.ax`

**Multiplexeurs/démultiplexeurs** :
- `VisioForge_MP4_Muxer_x64.ax`
- `VisioForge_MP4_Muxer_v10_x64.ax`
- `VisioForge_MF_Mux_x64.ax`
- `VisioForge_WebM_Mux_x64.ax`
- `VisioForge_WebM_Split_x64.ax`
- `VisioForge_WebM_Source_x64.ax`
- `VisioForge_WebM_Ogg_Source_x64.ax`
- `VisioForge_SSF_Muxer_x64.ax`

**Réseau** :
- `VisioForge_RTSP_Sink_x64.ax`
- `VisioForge_RTSP_Sink_X_x64.ax`
- `VisioForge_RTSP_Source_Live555_x64.ax`

**Filtres de base** (requis) :
- `VisioForge_BaseFilters_x64.ax`

**Bibliothèques utilitaires** (identiques à x86) :
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
- `VisioForge_MFT64.dll`

**Intel QuickSync** (optionnel) :
- `libmfxsw64.dll`
- `libmfxxp64.dll`

### Encodeur FFMPEG

L'encodeur FFMPEG dispose de son propre jeu de bibliothèques FFmpeg :

#### x86

**Filtre** :
- `VisioForge_FFMPEG_Encoder.ax`

**Bibliothèques FFmpeg** :
- `avcodec-58.dll`
- `avdevice-58.dll`
- `avfilter-7.dll`
- `avformat-58.dll`
- `avutil-56.dll`
- `swresample-3.dll`
- `swscale-5.dll`
- `ffmedia.dll` — wrapper VisioForge pour FFmpeg

**Info** :
- `vfffmpeg_info.txt` — informations de build FFmpeg

#### x64

Mêmes fichiers que x86 mais en versions 64 bits.

### Structure de répertoire d'installation

```
YourApp\
├── Filters\
│   ├── VisioForge_NVENC.ax
│   ├── VisioForge_NVENC_x64.ax
│   ├── VisioForge_H264_Encoder.ax
│   ├── VisioForge_H264_Encoder_x64.ax
│   ├── VisioForge_AAC_Encoder.ax
│   ├── VisioForge_AAC_Encoder_x64.ax
│   ├── VisioForge_MP4_Muxer.ax
│   ├── VisioForge_MP4_Muxer_x64.ax
│   ├── VisioForge_BaseFilters.ax
│   ├── VisioForge_BaseFilters_x64.ax
│   ├── VisioForge_MFP.dll
│   ├── VisioForge_MFP64.dll
│   ├── VisioForge_MFPX.dll
│   ├── VisioForge_MFPX64.dll
│   ├── VisioForge_MFT.dll
│   ├── VisioForge_MFT64.dll
│   ├── libmfxsw32.dll        (QuickSync)
│   ├── libmfxsw64.dll        (QuickSync)
│   └── ... (autres filtres)
└── FFMPEG\
    ├── x86\
    │   ├── VisioForge_FFMPEG_Encoder.ax
    │   ├── avcodec-58.dll
    │   ├── avformat-58.dll
    │   ├── ffmedia.dll
    │   └── ... (autres DLL FFmpeg)
    └── x64\
        ├── VisioForge_FFMPEG_Encoder_x64.ax
        └── ... (DLL FFmpeg)
```

### Fichiers de licence

- `license.rtf` — licence du SDK

### Taille totale

- **Filtres principaux uniquement** : ~40-60 Mo
- **Avec encodeur FFMPEG** : ~120-150 Mo
- **Pack complet** : ~150-180 Mo

### Exigences matérielles

- **NVENC** : nécessite un GPU NVIDIA (GeForce GTX 600+ ou Quadro K+) et ses pilotes
- **QuickSync** : nécessite un CPU Intel avec graphique intégré (4e génération ou ultérieur)

---
## Virtual Camera SDK { #virtual-camera-sdk }
### Fichiers principaux
#### x86 (32 bits)
**Pilotes de caméra virtuelle** :
- `VisioForge_Virtual_Camera.ax` — pilote de périphérique caméra virtuelle
- `VisioForge_Virtual_Audio_Card.ax` — pilote de périphérique audio virtuel
**Filtres source** :
- `VisioForge_Push_Video_Source.ax` — source push pour diffuser vers la caméra virtuelle
- `VisioForge_Screen_Capture_DD.ax` — capture d'écran DirectDraw
**Traitement** (inclus) :
- `VisioForge_Video_Effects_Pro.ax` — effets vidéo
**Filtres de base** (requis) :
- `VisioForge_BaseFilters.ax`
**Bibliothèques utilitaires** (requises) :
- `VisioForge_MFP.dll`
- `VisioForge_MFPX.dll`
**Runtime** (requis) :
- `vcomp140.dll` — runtime OpenMP Visual C++
#### x64 (64 bits)
**Pilotes de caméra virtuelle** :
- `VisioForge_Virtual_Camera_x64.ax`
- `VisioForge_Virtual_Audio_Card_x64.ax`
**Filtres source** :
- `VisioForge_Push_Video_Source_x64.ax`
- `VisioForge_Screen_Capture_DD_x64.ax`
**Traitement** :
- `VisioForge_Video_Effects_Pro_x64.ax`
**Filtres de base** (requis) :
- `VisioForge_BaseFilters_x64.ax`
**Bibliothèques utilitaires** (requises) :
- `VisioForge_MFP64.dll`
- `VisioForge_MFPX64.dll`
### Structure de répertoire d'installation
```
YourApp\
├── VisioForge_Virtual_Camera.ax
├── VisioForge_Virtual_Camera_x64.ax
├── VisioForge_Virtual_Audio_Card.ax
├── VisioForge_Virtual_Audio_Card_x64.ax
├── VisioForge_Push_Video_Source.ax
├── VisioForge_Push_Video_Source_x64.ax
├── VisioForge_Screen_Capture_DD.ax
├── VisioForge_Screen_Capture_DD_x64.ax
├── VisioForge_Video_Effects_Pro.ax
├── VisioForge_Video_Effects_Pro_x64.ax
├── VisioForge_BaseFilters.ax
├── VisioForge_BaseFilters_x64.ax
├── VisioForge_MFP.dll
├── VisioForge_MFP64.dll
├── VisioForge_MFPX.dll
├── VisioForge_MFPX64.dll
└── vcomp140.dll
```
### Fichiers de licence
- `license.rtf` — licence du SDK
### Taille totale
~15-20 Mo
### Remarques importantes
- Les périphériques de caméra virtuelle apparaissent dans les applications de visioconférence (Zoom, Teams, Skype, etc.)
- Prend en charge jusqu'à 4 instances de caméra virtuelle
- Nécessite l'installation de pilote (incluse dans l'installeur)
---

## Dépendances communes

### Redistribuables Visual C++

Tous les SDK nécessitent le redistribuable Visual C++ 2015-2022.

**Liens de téléchargement** :
- x86 : https://aka.ms/vs/17/release/vc_redist.x86.exe
- x64 : https://aka.ms/vs/17/release/vc_redist.x64.exe

**Vérification d'installation** (par programmation) :
```cpp
// Verifier si le redistribuable VC++ est installe
bool IsVCRedistInstalled()
{
    HKEY hKey;
    LONG result = RegOpenKeyEx(HKEY_LOCAL_MACHINE,
        L"SOFTWARE\\Microsoft\\VisualStudio\\14.0\\VC\\Runtimes\\x64",
        0, KEY_READ, &hKey);

    if (result == ERROR_SUCCESS)
    {
        RegCloseKey(hKey);
        return true;
    }
    return false;
}
```

### Utilitaire d'enregistrement

Tous les SDK incluent :
- `reg_special.exe` — utilitaire d'enregistrement personnalisé

Cet outil peut être utilisé à la place de `regsvr32` pour l'enregistrement des filtres.

---
## Liste de vérification de déploiement
### Fichiers minimum requis
Pour chaque SDK, vous devez inclure :
1. ✅ **Fichiers de filtres** — tous les fichiers .ax pour votre architecture (x86/x64)
2. ✅ **Filtres de base** — VisioForge_BaseFilters.ax (si requis par le SDK)
3. ✅ **DLL utilitaires** — VisioForge_MFP*.dll, VisioForge_MFPX*.dll
4. ✅ **Dépendances** — DLL FFmpeg, bibliothèques VLC, etc.
5. ✅ **Fichier de licence** — license.rtf (afficher dans l'installeur)
6. ✅ **Redistribuable VC++** — inclure ou télécharger dans l'installeur
### Fichiers optionnels
- 📄 **LAV Filters** — prise en charge de codecs renforcée (Processing Filters Pack)
- 📄 **DLL QuickSync** — encodage matériel Intel (Encoding Filters Pack)
- 📄 **Locales VLC** — prise en charge multilingue (VLC Source Filter)
- 📄 **Utilitaire d'enregistrement** — reg_special.exe (alternative à regsvr32)
### Considérations d'architecture
**Application 32 bits** :
- Inclure uniquement les fichiers x86 (.ax)
- Pas besoin des versions x64
**Application 64 bits** :
- Inclure uniquement les fichiers x64 (_x64.ax)
- Pas besoin des versions x86
**Application AnyCPU/.NET** :
- Inclure les versions x86 et x64
- Enregistrer les deux lors de l'installation
- L'application utilisera l'architecture appropriée à l'exécution
---

## Récapitulatif des tailles de fichiers

| SDK | Taille minimum | Avec toutes les options |
|-----|--------------|------------------|
| **FFMPEG Source** | ~80 Mo (x86) | ~190 Mo (les deux archi) |
| **VLC Source** | ~150 Mo | ~200 Mo (avec locales) |
| **Processing Filters** | ~20 Mo | ~180 Mo (avec LAV) |
| **Encoding Filters** | ~40 Mo | ~300 Mo (complet) |
| **Virtual Camera** | ~15 Mo | ~35 Mo (les deux archi) |

---
## Tester le paquet de déploiement
Avant publication, vérifiez que tous les fichiers sont inclus :
```batch
@echo off
echo Test de l'enregistrement des filtres...
REM Tester chaque filtre
regsvr32 /s "VisioForge_FFMPEG_Source_x64.ax"
if %errorLevel% neq 0 (
    echo ERREUR : echec d'enregistrement de FFMPEG Source
    echo Verifier la presence de toutes les DLL FFmpeg
    exit /b 1
)
REM Tester la creation du filtre
YourTestApp.exe
echo Tous les filtres ont ete enregistres avec succes !
```
---

## Voir aussi

- [Enregistrement des filtres](filter-registration.md) — comment enregistrer les filtres
- [Intégration avec l'installeur](installer-integration.md) — création d'installeurs
- [Vue d'ensemble du déploiement](index.md) — guide principal de déploiement
