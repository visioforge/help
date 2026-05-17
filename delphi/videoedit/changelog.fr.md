---
title: Journal des modifications TVFVideoEdit — SDK Delphi
description: Historique des versions TVFVideoEdit de 2.1 à 10.0 avec fonctionnalités, corrections, intégration FFMPEG, prise en charge Windows 8 et effets vidéo.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Streaming
  - Editing
primary_api_classes:
  - TVFVideoEdit

---

# Bibliothèque TVFVideoEdit : historique complet des versions

## Version 10.0 — Dernière version

### Améliorations principales

- **Compatibilité multimédia améliorée :** ajout d'un composant splitter MP3 dédié pour résoudre les problèmes de lecture des fichiers MP3 problématiques qui échouent avec le splitter par défaut
- **Améliorations du traitement audio :** extraction d'informations et lecture des métadonnées des fichiers audio Speex nettement améliorées
- **Optimisation des performances :** correction d'une fuite de mémoire critique dans l'implémentation de la source FFMPEG, pour une meilleure gestion des ressources
- **Prise en charge étendue des formats :** le filtre YUV2RGB prend désormais entièrement en charge le format HDYC pour les flux de travail vidéo professionnels

## Version 8.7 — Mises à jour des moteurs

### Améliorations techniques

- **Intégration VLC :** mise à jour du moteur VLC vers la dernière version stable (libVLC 2.2.1.0) pour une meilleure prise en charge des codecs
- **Capacités de décodage :** implémentation de la dernière version du moteur FFMPEG avec une compatibilité de formats étendue

## Version 8.6 — Améliorations de stabilité

### Corrections de bogues et ajouts

- **Gestion de la mémoire :** résolution d'une fuite de mémoire critique affectant les applications en exécution prolongée
- **Gestion des fichiers :** correction des problèmes liés à la fermeture incorrecte des fichiers d'entrée et de sortie qui provoquaient un verrouillage des ressources
- **Prise en charge WebM :** ajout de nouveaux filtres WebM hautes performances basés sur les spécifications officielles du projet WebM

## Version 8.4 — Extension de plateforme

### Prise en charge des environnements de développement

- **Delphi moderne :** intégration et compatibilité complètes ajoutées pour Delphi XE8
- **Extension d'architecture :** introduction des implémentations 64 bits (x64) pour Delphi et ActiveX

## Version 8.3.1 — Mise à jour de compatibilité

### Outils de développement

- **Prise en charge de l'IDE :** compatibilité et intégration complètes ajoutées pour Delphi XE7

## Version 8.3 — Version de performance

### Améliorations principales

- **Mise à jour du décodeur :** implémentation du décodeur FFMPEG sensiblement améliorée
- **Stabilité :** correction de plusieurs bogues affectant la fiabilité et les performances

## Version 8.0 — Mise à niveau majeure du moteur

### Fonctionnalités clés

- **Architecture de lecture :** intégration du moteur VLC pour des capacités de lecture vidéo/audio enrichies
- **Fiabilité :** résolution de plusieurs bogues critiques affectant les performances

## Version 7.15 — Fonctionnalités de sécurité

### Protection des médias

- **Sécurité du contenu :** ajout de la fonctionnalité de lecture de fichiers vidéo chiffrés
- **Stabilité :** mise en œuvre de corrections mineures pour une meilleure fiabilité

## Version 7.2 — Effets et performances

### Améliorations visuelles

- **Implémentation FFMPEG :** mise à jour du décodeur FFMPEG pour une meilleure prise en charge des formats
- **Effets vidéo :** ajout de capacités professionnelles d'effet pan/zoom vidéo
- **Fiabilité :** correction de bogues mineurs pour une meilleure stabilité

## Version 7.0 — Windows 8 et FFMPEG

### Prise en charge de la plateforme

- **Système d'exploitation :** ajout de la prise en charge complète de Windows 8 RTM
- **Gestion des médias :** intégration de capacités complètes de décodage FFMPEG
- **Effets visuels :** amélioration sensible de la qualité du traitement des effets vidéo

## Version 6.0 — Aperçu Windows 8

### Extension de plateforme

- **Adoption précoce :** compatibilité ajoutée avec Windows 8 Developer Preview
- **Traitement visuel :** qualité et performances des effets vidéo améliorées

## Version 3.4 — Version de maintenance

### Améliorations de stabilité

- **Corrections de bogues :** résolution de plusieurs problèmes affectant la fiabilité

## Version 3.3 — Prise en charge de Delphi XE2

### Outils de développement

- **Compatibilité de l'IDE :** prise en charge et intégration complètes ajoutées pour Delphi XE2
- **Stabilité :** mise en œuvre de diverses corrections de bogues pour une meilleure fiabilité

## Version 3.2 — Effets et démos

### Capacités améliorées

- **Effets visuels :** traitement des effets vidéo nettement amélioré
- **Ressources pour développeurs :** ajout d'applications de démonstration supplémentaires pour une implémentation plus aisée

## Version 3.1 — Mise à niveau des effets

### Traitement visuel

- **Moteur d'effets :** capacités de traitement des effets vidéo enrichies
- **Stabilité :** correction de plusieurs bogues pour une meilleure fiabilité

## Version 3.0 — Extension des fonctionnalités

### Améliorations majeures

- **Système d'effets :** fonctionnalité du filtre d'effets sensiblement améliorée
- **Streaming :** ajout de la prise en charge de la lecture de flux MMS / WMV
- **Analyse vidéo :** mise en œuvre de capacités de détection de mouvement
- **Composition :** ajout de la fonctionnalité professionnelle d'incrustation par chrominance
- **Performances du noyau :** moteur sous-jacent significativement amélioré

## Version 2.2 — Mise à jour des effets

### Traitement visuel

- **Qualité des effets :** implémentation du filtre d'effets améliorée pour de meilleurs résultats visuels

## Version 2.1 — Effets initiaux

### Premières implémentations

- **Traitement visuel :** introduction des capacités initiales du filtre d'effets
