---
title: Codecs d'encodage DirectShow — H.264, H.265, AAC et autres
description: Référence des codecs DirectShow avec vidéo H.264/H.265/VP8/VP9, audio AAC/MP3/Opus et accélération matérielle (NVENC, QuickSync, AMF).
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming
  - Encoding
  - MKV
  - WebM
  - TS
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
  - Speex

---

# Pack de filtres d'encodage — référence des codecs

## Vue d'ensemble

Ce document fournit une référence complète de tous les codecs vidéo et audio pris en charge par le pack de filtres d'encodage DirectShow. Le pack comprend des encodeurs logiciels et accélérés matériellement pour l'encodage multimédia professionnel.

---
## Codecs vidéo
### H.264/AVC (MPEG-4 Part 10)
Le codec vidéo le plus largement utilisé pour le streaming, la diffusion et le stockage de fichiers.
#### Options d'encodeur
| Type d'encodeur | Description | Prise en charge matérielle | Performances | Qualité |
|--------------|-------------|------------------|-------------|---------|
| **Logiciel (x264)** | Encodeur H.264 basé sur CPU | Aucune | Modérée | Excellente |
| **NVENC** | Encodeur GPU NVIDIA | GPU NVIDIA (Kepler+) | Très rapide | Bonne-Excellente |
| **QuickSync** | Graphiques intégrés Intel | CPU Intel (2e gén.+) | Rapide | Bonne |
| **AMD AMF** | Encodeur GPU AMD | GPU AMD (GCN+) | Rapide | Bonne |
| **Media Foundation** | Encodeur MF Windows | Variable (selon l'OS) | Modérée | Bonne |
#### Profils et niveaux
**Profils** :
- **Baseline** — fonctionnalités de base, appareils mobiles
- **Main** — fonctionnalités standard, la plupart des applications
- **High** — fonctionnalités avancées, contenu HD, Blu-ray
**Niveaux courants** :
- **Level 3.0** — SD (720x480 @ 30 fps)
- **Level 3.1** — 720p (1280x720 @ 30 fps)
- **Level 4.0** — 1080p (1920x1080 @ 30 fps)
- **Level 4.1** — 1080p @ 60 fps
- **Level 5.0** — 4K (3840x2160 @ 30 fps)
- **Level 5.1** — 4K @ 60 fps
#### Modes de contrôle du débit
| Mode | Description | Cas d'usage | Comportement de débit |
|------|-------------|----------|------------------|
| **CBR** | Débit binaire constant | Streaming, diffusion | Débit fixe |
| **VBR** | Débit binaire variable | Stockage de fichiers | Varie selon la complexité |
| **CQP** | Quantification constante | Archivage haute qualité | Varie fortement |
#### Paramètres recommandés
**Streaming (1080p @ 30fps)** :
- Débit binaire : 4-6 Mbps
- Profil : High
- Niveau : 4.0
- Taille de GOP : 60 (2 secondes)
- Images B : 2
**Enregistrement (1080p @ 60fps)** :
- Débit binaire : 8-12 Mbps
- Profil : High
- Niveau : 4.1
- Taille de GOP : 120 (2 secondes)
- Images B : 3
**Streaming basse latence** :
- Débit binaire : 2-4 Mbps
- Profil : Main
- Niveau : 3.1
- Taille de GOP : 30 (1 seconde)
- Images B : 0
---

### H.265/HEVC (High Efficiency Video Coding)

Codec de nouvelle génération offrant une compression 40 à 50 % meilleure que H.264.

#### Options d'encodeur

| Type d'encodeur | Description | Prise en charge matérielle | Performances | Qualité |
|--------------|-------------|------------------|-------------|---------|
| **Logiciel (x265)** | Encodeur HEVC basé sur CPU | Aucune | Lente | Excellente |
| **NVENC** | Encodeur GPU NVIDIA | GPU NVIDIA (Maxwell+) | Rapide | Bonne-Excellente |
| **QuickSync** | Graphiques intégrés Intel | CPU Intel (6e gén.+) | Rapide | Bonne |
| **AMD AMF** | Encodeur GPU AMD | GPU AMD (Fiji+) | Rapide | Bonne |

#### Profils et tiers

**Profils** :
- **Main** — 8 bits, 4:2:0, usage standard
- **Main 10** — 10 bits, prise en charge HDR
- **Main Still Picture** — images uniques

**Tiers** :
- **Main Tier** — applications standard
- **High Tier** — diffusion/professionnel

**Niveaux courants** :
- **Level 3.1** — 720p @ 30 fps
- **Level 4.0** — 1080p @ 30 fps
- **Level 4.1** — 1080p @ 60 fps
- **Level 5.0** — 4K @ 30 fps
- **Level 5.1** — 4K @ 60 fps

#### Paramètres recommandés

**Streaming 4K (2160p @ 30fps)** :
- Débit binaire : 15-20 Mbps
- Profil : Main
- Niveau : 5.0
- Taille de GOP : 60
- Encodage par tuiles : activé

**1080p haute qualité** :
- Débit binaire : 3-5 Mbps
- Profil : Main ou Main 10
- Niveau : 4.0
- Taille de GOP : 90

---
### VP8
Codec vidéo open source de Google, principalement pour les conteneurs WebM.
#### Caractéristiques
- **Licence** : libre de redevances, open source
- **Conteneur** : WebM (préféré), MKV
- **Prise en charge matérielle** : limitée
- **Qualité** : bonne aux débits moyens à élevés
- **Complexité** : temps d'encodage modéré
#### Paramètres recommandés
**Streaming WebM (720p)** :
- Débit binaire : 1-2 Mbps
- Taille de GOP : 120
- Qualité : bonne (échelle 0-63, plus bas est meilleur)
- Threads : auto
---

### VP9

Successeur de VP8 avec une efficacité de compression significativement améliorée.

#### Caractéristiques

- **Licence** : libre de redevances, open source
- **Conteneur** : WebM (principal), MKV
- **Prise en charge matérielle** : GPU récents (Intel, NVIDIA, AMD)
- **Qualité** : comparable à H.265
- **Complexité** : très élevée (logiciel), modérée (matériel)

#### Profils

- **Profile 0** — 8 bits, 4:2:0 (le plus courant)
- **Profile 1** — 8 bits, 4:2:2/4:4:4
- **Profile 2** — 10/12 bits, 4:2:0
- **Profile 3** — 10/12 bits, 4:2:2/4:4:4

#### Paramètres recommandés

**YouTube/WebM (1080p @ 30fps)** :
- Débit binaire : 2-4 Mbps
- Qualité/vitesse : 1 (plus rapide), 0 (plus lent/meilleur)
- Taille de GOP : 60
- Colonnes de tuiles : 2

---
### MPEG-2
Codec hérité encore utilisé pour les DVD et la diffusion.
#### Caractéristiques
- **Licence** : nécessite une licence
- **Conteneur** : MPEG-PS, MPEG-TS, VOB
- **Prise en charge matérielle** : universelle
- **Qualité** : bonne mais nécessite des débits plus élevés
- **Cas d'usage** : création de DVD, diffusion
#### Variantes courantes
- **MPEG-2 DVD** — 4-8 Mbps, 720x480/720x576
- **MPEG-2 SVCD** — 2.5 Mbps, 480x480/480x576
- **MPEG-2 HD** — 15-25 Mbps, 1920x1080
#### Paramètres recommandés
**DVD vidéo (NTSC)** :
- Résolution : 720x480
- Débit binaire : 6 Mbps
- Taille de GOP : 15 (NTSC) ou 12 (PAL)
- Rapport d'aspect : 16:9 ou 4:3
---

### MPEG-4 Part 2

Ancien codec MPEG-4 Visual (époque DivX/Xvid).

#### Caractéristiques

- **Licence** : nécessite une licence
- **Conteneur** : AVI, MP4, MKV
- **Qualité** : modérée
- **Cas d'usage** : contenu hérité, appareils à faible puissance

#### Paramètres recommandés

**Définition standard** :
- Résolution : 640x480
- Débit binaire : 1-2 Mbps
- Taille de GOP : 250

---
## Codecs audio
### AAC (Advanced Audio Coding)
Codec audio standard de l'industrie pour la plupart des applications.
#### Options d'encodeur
| Type d'encodeur | Description | Qualité | Performances |
|--------------|-------------|---------|-------------|
| **FFmpeg AAC** | Encodeur logiciel | Bonne | Rapide |
| **Media Foundation AAC** | Intégré à Windows | Bonne | Rapide |
| **FAAC** | Encodeur open source | Modérée | Rapide |
#### Profils
- **AAC-LC (Low Complexity)** — standard, le plus compatible
- **HE-AAC (High Efficiency)** — meilleur à faible débit
- **HE-AAC v2** — encore meilleur à très faible débit
#### Débits recommandés
| Qualité | Stéréo | Surround 5.1 |
|---------|--------|--------------|
| **Basse** | 64-96 kbps | 192 kbps |
| **Moyenne** | 128 kbps | 256 kbps |
| **Haute** | 192 kbps | 384 kbps |
| **Très haute** | 256-320 kbps | 448-640 kbps |
#### Fréquences d'échantillonnage
- **44,1 kHz** — qualité CD, musique
- **48 kHz** — audio professionnel, vidéo
- **32 kHz** — qualité inférieure (voix)
---

### MP3 (MPEG-1/2 Audio Layer III)

Codec audio hérité mais encore largement utilisé.

#### Options d'encodeur

- **LAME** — excellent encodeur open source de qualité
- **FFmpeg MP3** — encodeur intégré

#### Modes de débit

| Mode | Description | Taille de fichier | Qualité |
|------|-------------|-----------|---------|
| **CBR** | Débit binaire constant | Prévisible | Constante |
| **VBR** | Débit binaire variable | Plus petite | Meilleure |
| **ABR** | Débit binaire moyen | Équilibrée | Bonne |

#### Paramètres recommandés

**Musique (haute qualité)** :
- Mode : VBR
- Qualité : V0-V2 (échelle LAME)
- Débit approximatif : 190-245 kbps
- Fréquence d'échantillonnage : 44,1 kHz

**Podcast/parole** :
- Mode : CBR
- Débit binaire : 96-128 kbps
- Fréquence d'échantillonnage : 44,1 kHz

**Faible bande passante** :
- Mode : VBR
- Qualité : V5-V6
- Débit approximatif : 120-150 kbps

---
### Vorbis
Alternative open source à MP3 et AAC.
#### Caractéristiques
- **Licence** : totalement libre, sans brevet
- **Conteneur** : OGG (principal), WebM, MKV
- **Qualité** : excellente, particulièrement à débit faible à moyen
- **Compatibilité** : bonne mais non universelle
#### Paramètres recommandés
**Musique (haute qualité)** :
- Qualité : 6-8 (échelle 0-10)
- Débit approximatif : 192-256 kbps
- Fréquence d'échantillonnage : 44,1 kHz ou 48 kHz
**Streaming** :
- Qualité : 4-5
- Débit approximatif : 128-160 kbps
---

### Opus

Codec moderne et très efficace pour la voix et la musique.

#### Caractéristiques

- **Licence** : libre de redevances, normalisé (RFC 6716)
- **Conteneur** : WebM, MKV, OGG
- **Latence** : extrêmement faible (5-66,5 ms)
- **Plage de débit** : 6-510 kbps
- **Qualité** : supérieure à MP3, AAC, Vorbis

#### Applications

- **VoIP/chat vocal** : 8-24 kbps
- **Streaming musical** : 64-128 kbps
- **Haute fidélité** : 128-256 kbps

#### Paramètres recommandés

**Chat vocal** :
- Débit binaire : 16-24 kbps
- Fréquence d'échantillonnage : 16 kHz ou 48 kHz
- Application : VoIP

**Musique** :
- Débit binaire : 96-160 kbps
- Fréquence d'échantillonnage : 48 kHz
- Application : Audio

---
### FLAC (Free Lossless Audio Codec)
Compression audio sans perte.
#### Caractéristiques
- **Licence** : open source, libre de redevances
- **Compression** : généralement 40-60 % de l'original
- **Qualité** : sans perte bit à bit
- **Compatibilité** : bonne et en amélioration
#### Niveaux de compression
- **Niveau 0** — le plus rapide, ~50 % de compression
- **Niveau 5** — par défaut, ~55 % de compression
- **Niveau 8** — le plus lent, ~60 % de compression
#### Paramètres recommandés
**Archivage** :
- Niveau de compression : 5-8
- Fréquence d'échantillonnage : d'origine (généralement 44,1 ou 48 kHz)
- Profondeur de bits : d'origine (16 ou 24 bits)
**Streaming** :
- Niveau de compression : 0-3
- Fréquence d'échantillonnage : 44,1 ou 48 kHz
---

### Speex

Codec spécialisé pour la compression vocale.

#### Caractéristiques

- **Licence** : open source (BSD)
- **Objectif** : compression de la parole (pas de musique)
- **Débit binaire** : 2-44 kbps
- **Qualité** : optimisée pour la voix

#### Modes

- **Bande étroite** (8 kHz) — qualité téléphonique, 2,15-24,6 kbps
- **Bande large** (16 kHz) — meilleure clarté, 4-44 kbps
- **Ultra-large bande** (32 kHz) — spectre complet de la parole

#### Paramètres recommandés

**VoIP** :
- Mode : bande large
- Qualité : 6-8 (échelle 0-10)
- Débit binaire : ~15-20 kbps

---
## Aperçu de l'accélération matérielle
### NVIDIA NVENC
**Codecs pris en charge** :
- H.264/AVC (tous les GPU NVIDIA depuis la génération Kepler)
- H.265/HEVC (génération Maxwell et plus récente)
**Générations** :
- **Kepler** (GTX 600/700) — 1re gén., H.264 de base
- **Maxwell** (GTX 900) — 2e gén., prise en charge HEVC
- **Pascal** (GTX 10XX) — 3e gén., qualité améliorée
- **Turing/Ampere** (RTX 20XX/30XX) — 7e/8e gén., excellente qualité
**Performances** : jusqu'à 8K @ 30 fps (selon le GPU)
**Paramètres de qualité** :
- Préréglage : P1 (le plus rapide) à P7 (le plus lent, meilleure qualité)
- Recommandé : P4-P6 pour un équilibre qualité/vitesse
---

### Intel QuickSync

**Codecs pris en charge** :
- H.264/AVC (Core 2e gén. et plus récent)
- H.265/HEVC (Core 6e gén. et plus récent)
- VP9 (Core 9e gén. et plus récent)

**Générations** :
- **Sandy Bridge** (2e gén.) — prise en charge H.264
- **Skylake** (6e gén.) — prise en charge HEVC
- **Ice Lake** (10e gén. mobile) — qualité améliorée
- **Rocket Lake** (11e gén.) — fonctionnalités enrichies

**Performances** : jusqu'à 4K @ 60 fps

**Qualité** : bonne, en amélioration à chaque génération

---
### AMD AMF (Advanced Media Framework)
**Codecs pris en charge** :
- H.264/AVC (GCN 1.0 et plus récent)
- H.265/HEVC (Fiji/Polaris et plus récents)
**Générations** :
- **GCN 1-4** (R7/R9, RX 400/500) — H.264 uniquement
- **Vega** (RX Vega) — prise en charge HEVC
- **RDNA** (RX 5000/6000) — qualité améliorée
**Performances** : jusqu'à 4K @ 60 fps
**Qualité** : bonne, compétitive avec QuickSync
---

## Guide de sélection des codecs

### Pour le streaming (en direct)

**Recommandé** : H.264 (NVENC/QuickSync)
- **Raison** : compatibilité universelle, faible latence, accélération matérielle
- **Solution de repli** : H.264 (logiciel)

**Paramètres** :
- 1080p @ 30fps : 4-6 Mbps
- 720p @ 30fps : 2,5-4 Mbps
- Faible latence : désactiver les images B

---
### Pour l'enregistrement (haute qualité)
**Recommandé** : H.265 (NVENC/QuickSync) ou H.264 (logiciel)
- **Raison** : meilleur rapport qualité/taille
- **Alternative** : HEVC logiciel pour une qualité maximale
**Paramètres** :
- 4K @ 30fps : 15-25 Mbps (HEVC) ou 35-50 Mbps (H.264)
- 1080p @ 60fps : 8-15 Mbps (HEVC) ou 15-25 Mbps (H.264)
---

### Pour la diffusion web

**Recommandé** : VP9 ou H.264
- **Raison** : compatibilité navigateur, libre de redevances (VP9)

**Paramètres** :
- VP9 : 1080p @ 2-4 Mbps
- H.264 : 1080p @ 4-6 Mbps

---
### Pour l'audio
**Musique** : AAC (128-192 kbps) ou Opus (96-160 kbps)
**Voix** : Opus (16-32 kbps) ou Speex (15-20 kbps)
**Archivage** : FLAC (sans perte)
**Podcast** : MP3 VBR (V4-V2, ~130-190 kbps) ou AAC (128 kbps)
---

## Matrice de compatibilité

| Codec | MP4 | MKV | AVI | WebM | OGG | MPEG-TS |
|-------|-----|-----|-----|------|-----|---------|
| **H.264** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **H.265** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **VP8** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **VP9** | ✗ | ✓ | ✗ | ✓ | ✗ | ✗ |
| **MPEG-2** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **AAC** | ✓ | ✓ | ✗ | ✗ | ✗ | ✓ |
| **MP3** | ✓ | ✓ | ✓ | ✗ | ✗ | ✓ |
| **Vorbis** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **Opus** | ✗ | ✓ | ✗ | ✓ | ✓ | ✗ |
| **FLAC** | ✓ | ✓ | ✗ | ✓ | ✓ | ✗ |

---
## Voir aussi
- [Présentation du pack de filtres d'encodage](index.md)
- [Référence des multiplexeurs](muxers-reference.md)
- [Référence de l'interface NVENC](interfaces/nvenc.md)
- [Exemples de code](examples.md)
