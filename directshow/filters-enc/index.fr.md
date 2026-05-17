---
title: Filtres d'encodage DirectShow — H.264, H.265, NVENC et plus
description: Encodeurs DirectShow vidéo/audio avec H.264, H.265, VP8, VP9, AAC, MP3 et accélération GPU (NVENC, QuickSync, AMF) pour applications Windows.
sidebar_label: DirectShow Encoding Filters Pack
order: 8
tags:
  - DirectShow
  - C++
  - Windows
  - Streaming

---

# DirectShow Encoding Filters Pack

## Introduction

Le DirectShow Encoding Filters Pack fournit un ensemble puissant de composants d'encodage multimédia conçus spécifiquement pour les développeurs de logiciels qui créent des applications multimédias professionnelles. Cette boîte à outils permet l'intégration transparente de capacités d'encodage hautes performances pour les flux audio et vidéo dans une large variété de formats populaires.

---

## Installation

Avant d'utiliser les exemples de code et d'intégrer les filtres dans votre application, vous devez d'abord installer le DirectShow Encoding Filters Pack depuis la [page produit](https://www.visioforge.com/encoding-filters-pack).

**Étapes d'installation** :

1. Téléchargez l'installeur du Encoding Filters Pack depuis la page produit
2. Exécutez l'installeur avec des privilèges administrateur
3. L'installeur enregistrera tous les filtres d'encodage et de multiplexage
4. Les applications d'exemple et le code source seront disponibles dans le répertoire d'installation

**Remarque** : tous les filtres doivent être correctement enregistrés sur le système avant de pouvoir être utilisés dans vos applications. L'installeur s'en charge automatiquement.

---

## Fonctionnalités clés

### Prise en charge d'encodage multiformat

Le Encoding Filters Pack prend en charge de nombreux formats standard de l'industrie, notamment :

- **Conteneur MP4** avec codecs H264, HEVC et AAC
- Flux **MPEG-TS**
- Conteneurs **MKV** (Matroska)
- Format **WebM** avec codecs vidéo VP8/VP9
- Plusieurs formats audio, dont **Vorbis**, **MP3**, **FLAC** et **Opus**

### Accélération matérielle

Les développeurs peuvent tirer parti de l'accélération GPU pour de meilleures performances d'encodage :

- Technologie **Intel** QuickSync
- Accélération matérielle **AMD/ATI**
- Prise en charge de l'encodage **Nvidia** NVENC

Cette optimisation matérielle améliore considérablement les vitesses d'encodage tout en réduisant la charge CPU dans vos applications.

### Options d'implémentation flexibles

Le pack inclut :

- Encodeurs H264/AAC autonomes exploitant les ressources CPU
- Composants multiplexeurs spécialisés avec encodeurs vidéo et audio intégrés
- Options pour les chemins d'encodage basés sur CPU et GPU

## Capacités techniques

Les composants de filtres s'intègrent parfaitement dans les pipelines d'applications DirectShow, fournissant aux développeurs :

- Encodage vidéo de haute qualité à différents débits et résolutions
- Compression audio efficace avec paramètres de qualité configurables
- Prise en charge de formats de conteneurs avancés avec paramètres personnalisables
- Compatibilité avec le graphe de filtres DirectShow pour une implémentation simple

Pour les spécifications détaillées et une liste complète de tous les encodeurs vidéo/audio et formats de sortie pris en charge, consultez la [page produit](https://www.visioforge.com/encoding-filters-pack).

## Historique des versions

### Version 11.4

- Composants de filtres mis à jour pour correspondre aux implémentations actuelles des SDK .Net
- Encodeurs AMD AMF H264/H265 améliorés avec les dernières optimisations
- Encodeurs Intel QuickSync H265 améliorés pour de meilleures performances
- Applications d'exemple rafraîchies avec de nouveaux exemples de code

### Version 11.0

- Filtres synchronisés avec les versions actuelles des SDK .Net
- Encodeurs Nvidia NVENC H264/H265 mis à niveau pour une meilleure qualité
- Introduction d'un nouveau composant de filtre multiplexeur SSF

### Version 10.0

- Tous les filtres mis à jour pour s'aligner sur les implémentations des SDK .Net
- Encodeurs Media Foundation améliorés (H264, H265, AAC)
- Ajout d'un filtre d'encodeur vidéo NVENC dédié en remplacement de l'encodeur CUDA

### Version 9.0

- Conteneur MP4 optimisé avec sortie H264/AAC
- Prise en charge étendue du format WebM avec capacités d'encodage VP9
- Performances du filtre encodeur H265 améliorées
- Encodeurs Intel QuickSync H264 améliorés

### Version 8.6

- Filtre puits RTSP implémenté pour les applications de streaming
- Filtre puits RTMP ajouté en statut BETA
- Filtre encodeur AAC mis à niveau avec des améliorations de qualité

### Version 8.5 — version initiale

- Première publication incluant les filtres des SDK .Net
- Composants principaux : encodeur AAC, encodeurs H264 (CPU/GPU)
- Encodeurs supplémentaires : H265 (CPU/GPU), VP8, Vorbis
- Prise en charge de conteneurs : multiplexeur MP4, multiplexeur WebM
- Capacités de streaming : source RTSP, source RTMP

---

## Ressources

- [Page produit](https://www.visioforge.com/encoding-filters-pack) — achat, licences et informations produit
- [Exemples de code](https://github.com/visioforge/directshow-samples/tree/main/Encoding%20Filters%20Pack) — applications d'exemple et exemples d'implémentation

---

## Voir aussi

- [Référence des codecs](codecs-reference.md) — documentation complète des codecs vidéo et audio
- [Référence des multiplexeurs](muxers-reference.md) — spécifications des formats de conteneurs
- [Interface NVENC](interfaces/nvenc.md) — API de l'encodeur matériel NVIDIA
- [Exemples de code](examples.md) — exemples pratiques d'encodage
