---
title: Guide de référence des multiplexeurs et formats DirectShow
description: Référence des formats de conteneurs pour les multiplexeurs DirectShow VisioForge — MP4, MKV, WebM, MPEG-TS, AVI, compatibilité codecs et streaming.
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - Decoding
  - Metadata
  - HLS
  - MPEG-DASH
  - WebRTC
  - MP4
  - MKV
  - WebM
  - AVI
  - FLV
  - TS
  - OGG
  - H.264
  - H.265
  - VP8
  - VP9
  - MPEG-2
  - AAC
  - MP3
  - Opus
  - FLAC
  - Vorbis
  - WAV
  - AC-3
  - Speex

---

# Pack de filtres d'encodage — référence des multiplexeurs

## Vue d'ensemble

Ce document fournit des informations complètes sur tous les formats de conteneurs (multiplexeurs) pris en charge par le pack de filtres d'encodage DirectShow. Les multiplexeurs combinent les flux vidéo et audio dans des fichiers conteneurs pour le stockage et la diffusion.

---
## Conteneur MP4
### Vue d'ensemble
MPEG-4 Part 14 (MP4) est le format de conteneur le plus utilisé pour la distribution vidéo.
**Extensions de fichiers** : `.mp4`, `.m4v`, `.m4a` (audio uniquement)
**Type MIME** : `video/mp4`, `audio/mp4`
### Codecs pris en charge
#### Codecs vidéo
- H.264/AVC ✓ (principal)
- H.265/HEVC ✓
- MPEG-4 Part 2 ✓
- MPEG-2 ✗ (utilisez MPEG-TS à la place)
- VP8/VP9 ✗ (utilisez WebM à la place)
#### Codecs audio
- AAC ✓ (principal)
- MP3 ✓
- Opus ✗
- Vorbis ✗
- FLAC ✓
- PCM ✓
### Fonctionnalités
**Prise en charge du streaming** :
- **Téléchargement progressif** : ✓ (avec placement correct de l'atome moov)
- **Streaming adaptatif** : ✓ (DASH, HLS avec MP4 fragmenté)
- **Diffusion en direct** : ✓ (MP4 fragmenté)
**Prise en charge des métadonnées** :
- **Étiquettes de base** : titre, artiste, album, année
- **Pochette** : ✓
- **Chapitres** : ✓
- **Sous-titres** : ✓ (VTT, SRT, divers formats texte)
**Caractéristiques techniques** :
- **Pistes audio multiples** : ✓
- **Pistes de sous-titres multiples** : ✓
- **Démarrage rapide** : ✓ (atome moov en début)
- **MP4 fragmenté** : ✓ (pour le streaming)
### Bonnes pratiques
**Pour le téléchargement progressif (web)** :
```
- Placer l'atome moov au début (démarrage rapide)
- Utiliser le profil H.264 Baseline/Main
- Audio AAC-LC
- Intervalle d'images-clés : 2-4 secondes
```
**Pour la lecture locale** :
```
- Profil H.264 High ou H.265
- AAC-LC ou HE-AAC
- Tout intervalle d'images-clés
```
**Pour le streaming (DASH/HLS)** :
```
- MP4 fragmenté
- Profil H.264 Main/High
- Audio AAC-LC
- Fragments courts (2-6 secondes)
```
### Compatibilité
| Plateforme/Appareil | Compatibilité |
|----------------|---------------|
| **Windows Media Player** | ✓ |
| **VLC** | ✓ |
| **Navigateurs web** | ✓ |
| **iOS/iPhone** | ✓ |
| **Android** | ✓ |
| **TV connectées** | ✓ |
| **Consoles de jeu** | ✓ |
### Problèmes courants
**Problème** : vidéo non navigable sur le web
- **Solution** : activer le démarrage rapide (moov en début)
**Problème** : problèmes de synchronisation audio
- **Solution** : utiliser une fréquence d'images constante, vérifier la fréquence d'échantillonnage audio
---

## Conteneur MKV (Matroska)

### Vue d'ensemble

Matroska est un format de conteneur ouvert et riche en fonctionnalités.

**Extensions de fichiers** : `.mkv` (vidéo), `.mka` (audio), `.mks` (sous-titres)

**Type MIME** : `video/x-matroska`, `audio/x-matroska`

### Codecs pris en charge

#### Codecs vidéo
- H.264/AVC ✓
- H.265/HEVC ✓
- VP8 ✓
- VP9 ✓
- MPEG-4 Part 2 ✓
- MPEG-2 ✓
- AV1 ✓

#### Codecs audio
- AAC ✓
- MP3 ✓
- Opus ✓
- Vorbis ✓
- FLAC ✓
- DTS ✓
- AC-3 ✓
- PCM ✓

### Fonctionnalités

**Fonctionnalités avancées** :
- **Pistes vidéo multiples** : ✓
- **Pistes audio multiples** : ✓ (illimitées)
- **Pistes de sous-titres multiples** : ✓ (illimitées)
- **Pièces jointes** : ✓ (polices, pochettes)
- **Chapitres** : ✓ (avec imbrication)
- **Étiquettes/métadonnées** : ✓ (étendues)
- **Segmentation** : ✓ (segments liés)

**Capacités techniques** :
- **Fréquence d'images variable** : ✓
- **Audio sans perte** : ✓
- **3D/stéréoscopique** : ✓
- **Métadonnées HDR** : ✓

### Bonnes pratiques

**Pour l'archivage** :
```
- Utiliser FLAC ou PCM pour l'audio sans perte
- Inclure toutes les pistes audio/sous-titres
- Ajouter des repères de chapitres
- Inclure les étiquettes de métadonnées
```

**Pour la distribution** :
```
- Vidéo H.264/H.265
- Audio AAC (le plus compatible)
- Sous-titres incorporés modifiables
- Taille de fichier raisonnable
```

**Pour le streaming** :
```
- Pas idéal pour le streaming
- Envisager MP4 ou WebM à la place
- Si utilisé : désactiver les fonctionnalités complexes
```

### Compatibilité

| Plateforme/Appareil | Compatibilité |
|----------------|---------------|
| **Windows Media Player** | ✗ (pack de codecs requis) |
| **VLC** | ✓ |
| **Navigateurs web** | ✗ (pas de prise en charge native) |
| **iOS/iPhone** | ✗ (applis tierces uniquement) |
| **Android** | Limitée (selon l'application) |
| **TV connectées** | Limitée (selon le modèle) |
| **Lecteurs multimédias** | ✓ (Kodi, Plex, etc.) |

### Problèmes courants

**Problème** : navigation lente
- **Solution** : activer les cues (index) lors du multiplexage

**Problème** : lecture saccadée avec audio haute qualité
- **Solution** : vérifier les performances du décodeur, envisager AAC plutôt que sans perte

---
## Conteneur WebM
### Vue d'ensemble
WebM est un format ouvert et libre de redevances conçu pour le web.
**Extensions de fichiers** : `.webm`
**Type MIME** : `video/webm`, `audio/webm`
### Codecs pris en charge
#### Codecs vidéo
- VP8 ✓ (WebM 1.0)
- VP9 ✓ (WebM 2.0)
- AV1 ✓ (expérimental)
- H.264 ✗
- H.265 ✗
#### Codecs audio
- Vorbis ✓ (principal)
- Opus ✓ (recommandé)
- AAC ✗
- MP3 ✗
### Fonctionnalités
**Optimisé pour le web** :
- **Vidéo HTML5** : ✓ (prise en charge native par le navigateur)
- **Streaming** : ✓
- **Streaming adaptatif** : ✓ (DASH)
- **Faible latence** : ✓
**Prise en charge des métadonnées** :
- **Étiquettes de base** : ✓
- **Chapitres** : ✓
- **Sous-titres** : ✓ (WebVTT)
### Bonnes pratiques
**Pour YouTube/web** :
```
- Codec vidéo VP9
- Codec audio Opus (96-160 kbps)
- Intervalle d'images-clés : 2-4 secondes
- Encodage en deux passes pour la meilleure qualité
```
**Pour la diffusion en direct** :
```
- VP8 pour de meilleures performances d'encodage
- Audio Opus (mode faible latence)
- Débit CBR
- GOP court
```
**Pour la haute qualité** :
```
- VP9 avec débit élevé
- Opus 128-256 kbps
- Encodage en deux passes
- Contrôle du débit basé sur la qualité
```
### Compatibilité
| Plateforme/Appareil | Compatibilité |
|----------------|---------------|
| **Chrome** | ✓ |
| **Firefox** | ✓ |
| **Edge** | ✓ |
| **Safari** | Limitée (VP8 uniquement) |
| **Android** | ✓ |
| **iOS** | Limitée |
### Problèmes courants
**Problème** : Safari ne lit pas le WebM
- **Solution** : fournir une solution de repli MP4 avec H.264
**Problème** : encodage trop lent
- **Solution** : utiliser VP8 au lieu de VP9, ou VP9 accéléré matériellement si disponible
---

## MPEG-TS (Transport Stream)

### Vue d'ensemble

MPEG Transport Stream est conçu pour la diffusion et le streaming, en particulier lorsque la résistance aux erreurs est importante.

**Extensions de fichiers** : `.ts`, `.mts`, `.m2ts`

**Type MIME** : `video/mp2t`

### Codecs pris en charge

#### Codecs vidéo
- H.264/AVC ✓
- H.265/HEVC ✓
- MPEG-2 ✓
- VP8/VP9 ✗

#### Codecs audio
- AAC ✓
- MP3 ✓
- AC-3 ✓
- PCM ✓

### Fonctionnalités

**Fonctionnalités de diffusion** :
- **Résistance aux erreurs** : ✓ (récupération d'erreurs intégrée)
- **Décalage temporel** : ✓
- **Multiplexage de programmes** : ✓ (plusieurs programmes dans un flux)
- **Chiffrement** : ✓ (accès conditionnel)

**Fonctionnalités de streaming** :
- **Streaming HLS** : ✓ (Apple HTTP Live Streaming)
- **Diffusion DVB** : ✓
- **IPTV** : ✓

### Bonnes pratiques

**Pour le streaming HLS** :
```
- Vidéo H.264
- Audio AAC
- Durée de segment : 6-10 secondes
- Encodage CBR
- GOP fermé
```

**Pour la diffusion** :
```
- MPEG-2 ou H.264
- Audio AC-3 ou AAC
- Débit binaire constant
- Taille de paquet fixe (188 octets)
```

### Compatibilité

| Plateforme/Appareil | Compatibilité |
|----------------|---------------|
| **Lecteurs HLS** | ✓ |
| **Box TV** | ✓ |
| **TV connectées** | ✓ |
| **VLC** | ✓ |
| **Navigateurs web** | Via prise en charge HLS |

---
## FLV (Flash Video)
### Vue d'ensemble
Format hérité autrefois utilisé pour la vidéo sur le web (YouTube, lecteurs Flash).
**Extensions de fichiers** : `.flv`, `.f4v`
**Type MIME** : `video/x-flv`
**Statut** : ⚠️ Déprécié — utilisez MP4 ou WebM à la place
### Codecs pris en charge
#### Codecs vidéo
- H.264 ✓
- VP6 ✓ (hérité)
- Sorenson Spark ✓ (hérité)
#### Codecs audio
- AAC ✓
- MP3 ✓
- Speex ✓
### Fonctionnalités
- **Streaming** : ✓ (RTMP)
- **Métadonnées** : basiques (onMetaData)
- **Points de repère** : ✓
**Non recommandé** : la fin de vie de Flash Player (2020) rend FLV obsolète
---

## Conteneur OGG

### Vue d'ensemble

Conteneur open source principalement pour l'audio Vorbis.

**Extensions de fichiers** : `.ogg`, `.oga` (audio), `.ogv` (vidéo)

**Type MIME** : `audio/ogg`, `video/ogg`

### Codecs pris en charge

#### Codecs vidéo
- Theora ✓ (qualité héritée)
- VP8 ✗ (utilisez WebM)

#### Codecs audio
- Vorbis ✓ (principal)
- Opus ✓
- FLAC ✓
- Speex ✓

### Fonctionnalités

- **Streaming** : ✓
- **Chaînage** : ✓ (plusieurs fichiers en séquence)
- **Métadonnées** : ✓ (commentaires Vorbis)

### Bonnes pratiques

**Pour l'audio** :
```
- Codec Vorbis ou Opus
- Encodage basé sur la qualité
- Commentaires Vorbis pour les métadonnées
```

**Pour la vidéo** :
```
- Non recommandé
- Utiliser WebM (VP8/VP9) à la place
```

### Compatibilité

| Plateforme/Appareil | Compatibilité |
|----------------|---------------|
| **Firefox** | ✓ |
| **Chrome** | ✓ |
| **VLC** | ✓ |
| **La plupart des appareils mobiles** | Limitée |

---
## AVI (Audio Video Interleave)
### Vue d'ensemble
Format de conteneur Microsoft hérité.
**Extensions de fichiers** : `.avi`
**Type MIME** : `video/x-msvideo`
**Statut** : ⚠️ Hérité — utilisez MP4 ou MKV pour les nouveaux projets
### Codecs pris en charge
#### Codecs vidéo
- H.264 ✓ (prise en charge limitée)
- MPEG-4 Part 2 ✓
- MPEG-2 ✓
- Divers codecs hérités ✓
#### Codecs audio
- MP3 ✓
- PCM ✓
- AC-3 ✓
- AAC Limitée
### Limitations
- **Taille maximale de fichier** : 2 Go (sans OpenDML)
- **Métadonnées limitées** : très basiques
- **Pas de streaming** : non conçu pour le streaming
- **Pas de chapitres** : non pris en charge
### Quand l'utiliser
- Compatibilité avec systèmes hérités
- Capture depuis du matériel ancien
- Exigences logicielles spécifiques
**Recommandation** : utilisez MP4 ou MKV pour les nouveaux projets
---

## Conteneur WAV

### Vue d'ensemble

Conteneur audio non compressé.

**Extensions de fichiers** : `.wav`

**Type MIME** : `audio/wav`, `audio/x-wav`

### Fonctionnalités

- **Sans perte** : ✓ (PCM)
- **Compressé** : ✓ (MP3, AAC dans un wrapper WAV)
- **Métadonnées** : limitées (étiquettes RIFF)

### Formats courants

- **PCM 44,1 kHz 16 bits** : qualité CD
- **PCM 48 kHz 24 bits** : audio professionnel
- **PCM 96 kHz 24 bits** : audio haute résolution

### Bonnes pratiques

**Pour la production audio** :
```
- 48 kHz, 24 bits PCM
- Mono ou stéréo
- Éviter la compression
```

**Pour la distribution** :
```
- Utiliser FLAC ou AAC à la place
- Les fichiers WAV sont volumineux
```

---
## Guide de sélection des conteneurs
### Pour la diffusion web
**Principal** : MP4 (H.264 + AAC)
- **Raison** : compatibilité universelle
- **Solution de repli** : WebM (VP9 + Opus) pour les navigateurs modernes
### Pour l'archivage professionnel
**Principal** : MKV (H.265 + FLAC)
- **Raison** : riche en fonctionnalités, prise en charge audio sans perte
- **Alternative** : MP4 (H.265 + AAC) pour une meilleure compatibilité
### Pour la diffusion/IPTV
**Principal** : MPEG-TS (H.264 + AAC)
- **Raison** : résistance aux erreurs, standard de l'industrie
- **Alternative** : MPEG-TS (MPEG-2 + AC-3) pour les systèmes hérités
### Pour la diffusion en direct
**HLS** : segments MPEG-TS (H.264 + AAC)
**DASH** : MP4 fragmenté (H.264 + AAC)
**WebRTC** : audio Opus, vidéo VP8/H.264
### Pour l'audio seulement
**Haute qualité** : FLAC (.flac) ou MP3 VBR (.mp3)
**Streaming** : AAC dans MP4 (.m4a) ou Opus dans WebM
**Voix** : Opus dans OGG ou Speex
---

## Tableau comparatif des formats

| Caractéristique | MP4 | MKV | WebM | MPEG-TS | FLV | OGG |
|---------|-----|-----|------|---------|-----|-----|
| **Compatibilité web** | ★★★★★ | ★☆☆☆☆ | ★★★★☆ | ★★☆☆☆ | ☆☆☆☆☆ | ★★☆☆☆ |
| **Compatibilité mobile** | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★★★☆ | ☆☆☆☆☆ | ★☆☆☆☆ |
| **Prise en charge du streaming** | ★★★★★ | ★★☆☆☆ | ★★★★★ | ★★★★★ | ★★★☆☆ | ★★★☆☆ |
| **Richesse fonctionnelle** | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★☆☆☆ |
| **Prise en charge des codecs** | ★★★★☆ | ★★★★★ | ★★☆☆☆ | ★★★☆☆ | ★★☆☆☆ | ★★★☆☆ |
| **Efficacité de taille** | ★★★★☆ | ★★★★☆ | ★★★★★ | ★★★☆☆ | ★★★☆☆ | ★★★★☆ |
| **Résistance aux erreurs** | ★★☆☆☆ | ★★☆☆☆ | ★★☆☆☆ | ★★★★★ | ★★★☆☆ | ★★☆☆☆ |

---
## Spécifications techniques
### Structure MP4
```
ftyp (type de fichier)
moov (metadonnees - placer au debut pour un demarrage rapide)
  ├── mvhd (en-tete du film)
  ├── trak (piste video)
  ├── trak (piste audio)
  └── udta (donnees utilisateur/metadonnees)
mdat (donnees medias)
```
### MP4 fragmenté (pour le streaming)
```
ftyp
moov
  └── mvex (extensions du film)
moof (fragment de film)
  └── traf (fragment de piste)
mdat (donnees de fragment)
[repeter moof/mdat pour chaque fragment]
```
### Structure MKV
```
En-tete EBML
Segment
  ├── SeekHead (index)
  ├── Info (informations du segment)
  ├── Tracks (definitions des pistes)
  ├── Chapters (optionnel)
  ├── Attachments (optionnel)
  ├── Tags (metadonnees)
  └── Cluster (donnees medias)
```
---

## Voir aussi

- [Présentation du pack de filtres d'encodage](index.md)
- [Référence des codecs](codecs-reference.md)
- [Exemples de code](examples.md)
- [Référence de l'interface NVENC](interfaces/nvenc.md)
