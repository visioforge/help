---
title: Journal des modifications du Video Capture SDK Delphi
description: Historique complet des versions du Video Capture SDK Delphi (TVFVideoCapture) — accélération GPU, streaming, nouveaux formats et correctifs de la 4.1 à 11.0.
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming
primary_api_classes:
  - TVFVideoCapture
  - TVFMediaPlayer

---

# Historique des versions de TVFVideoCapture

## Version 11.00 — Encodage GPU amélioré et prise en charge des Delphi modernes

- **Compatibilité framework étendue** : ajout de la prise en charge des environnements de développement Delphi 10.4 et 11.0
- **Accélération GPU AMD avancée** : implémentation de l'encodage vidéo MP4 (H264/AAC) utilisant les unités de traitement graphique AMD
- **Encodage matériel GPU Intel** : ajout de l'encodage vidéo MP4 (H264/AAC) via les GPU intégrés et discrets Intel
- **Accélération NVIDIA CUDA** : introduction de l'encodage vidéo MP4 (H264/AAC) propulsé par le matériel graphique NVIDIA
- **Améliorations du format conteneur** : sortie MKV améliorée avec des performances et une fiabilité optimisées
- **Nouveau format de sortie** : ajout de la prise en charge du conteneur MOV pour la compatibilité avec l'écosystème Apple

## Version 10.0 — Optimisations de performances et prise en charge multiplateforme

- **Amélioration MP4** : capacités de sortie MP4 entièrement mises à jour et améliorées
- **Améliorations du streaming** : mise à jour du VLC Source Filter avec une prise en charge améliorée de RTMP et HTTPS
- **Gestion de la mémoire** : correction d'une fuite mémoire critique dans l'encodeur CUDA pour un encodage stable de longue durée
- **Optimisation des ressources** : résolution d'une fuite mémoire dans la source FFMPEG pour une stabilité applicative améliorée
- **Capture audio** : filtre What You Hear amélioré pour un enregistrement audio système supérieur
- **Architecture 64 bits** : ajout d'un VLC Source x64 pour TVFMediaPlayer et TVFVideoCapture (Delphi et ActiveX)
- **Prise en charge de formats étendue** : filtre YUV2RGB amélioré avec prise en charge du format HDYC
- **Encodage audio** : encodeur LAME mis à jour avec correctif pour les problèmes audio mono à faible débit
- **Environnement de développement** : ajout de la prise en charge de Delphi 10 et 10.1 pour les flux de travail modernes

## Version 8.7 — Mises à jour du moteur principal

- **Intégration VLC** : moteur VLC mis à jour vers libVLC 2.2.1.0 pour des capacités de streaming améliorées
- **Amélioration du décodeur** : moteur FFMPEG mis à jour pour une meilleure compatibilité et de meilleures performances de formats

## Version 8.6 — Améliorations de fiabilité et prise en charge de formats

- **Gestion des ressources** : correction d'une fuite mémoire critique pour une stabilité applicative améliorée
- **Gestion des fichiers** : résolution des problèmes de fichiers d'entrée et de sortie mal fermés
- **Nouvelle prise en charge de format** : ajout de filtres WebM personnalisés basés sur les spécifications du projet WebM

## Version 8.4 — Extension de l'architecture

- **Delphi moderne** : ajout de la prise en charge de Delphi XE8 pour les environnements de développement les plus récents
- **Architecture 64 bits** : introduction des versions x64 Delphi et ActiveX pour des performances sur les systèmes modernes

## Version 8.31 — Mise à jour de l'environnement de développement

- **Compatibilité framework** : ajout de la prise en charge de Delphi XE7 pour des options de développement étendues

## Version 8.3 — Améliorations d'API et de performances

- **Amélioration de l'interface** : API ActiveX mise à jour pour une meilleure expérience développeur
- **Optimisation du décodeur** : décodeur FFMPEG amélioré pour de meilleures performances et une meilleure prise en charge des formats
- **Stabilité** : plusieurs correctifs critiques et améliorations de performances implémentés

## Version 8.0 — Capacités de streaming

- **Diffusion réseau** : introduction du moteur VLC pour les capacités de capture vidéo IP
- **Fiabilité** : correction de plusieurs bogues pour une stabilité améliorée sur l'ensemble des composants

## Version 7.15 — Options de sortie avancées et sécurité

- **Capture réseau** : moteur de capture IP amélioré pour une meilleure stabilité de connexion et de meilleures performances
- **Prise en charge de format moderne** : ajout de la sortie MP4 avec H264/AAC pour la compatibilité standard de l'industrie
- **Fonctionnalité de sécurité** : implémentation du chiffrement vidéo pour les flux de contenu protégé
- **Intégration système** : ajout d'une sortie caméra virtuelle pour les scénarios d'intégration logicielle
- **Stabilité** : de multiples petits correctifs pour une fiabilité améliorée

## Version 7.0 — Améliorations du moteur de capture

- **Performances réseau** : moteur de capture IP amélioré avec un meilleur débit et une meilleure fiabilité
- **Capture de bureau** : moteur de capture d'écran mis à jour pour de meilleures performances et une meilleure qualité
- **Options de sortie** : sortie FFMPEG améliorée pour une prise en charge étendue de formats
- **Effets visuels** : ajout de l'effet vidéo Pan/Zoom pour une manipulation vidéo avancée
- **Fiabilité** : implémentation de plusieurs petits correctifs pour une stabilité améliorée

## Version 6.0 — Multi-source et compatibilité Windows 8

- **Composition avancée** : Picture-In-Picture amélioré avec prise en charge de toute source vidéo, y compris la capture d'écran et les caméras IP
- **Protocole de streaming** : prise en charge des sources RTSP améliorée pour une meilleure intégration vidéo réseau
- **Mode de capture spécial** : ajout de la prise en charge de la capture d'écran de fenêtres en couches pour l'enregistrement d'interfaces complexes
- **Prise en charge matérielle** : implémentation de la prise en charge des caméras iCube pour les applications d'imagerie spécialisées
- **Compatibilité OS** : ajout de la prise en charge de Windows 8 Developer Preview pour la compatibilité future
- **Traitement visuel** : effets vidéo améliorés avec de nouvelles options et de meilleures performances
- **Gestion audio** : introduction de la prise en charge de plusieurs flux audio pour les sorties AVI et WMV

## Version 5.5 — Améliorations de stabilité et de fonctionnalités

- **Traitement visuel** : effets vidéo améliorés avec une meilleure qualité et de meilleures performances
- **Vidéo réseau** : prise en charge des caméras IP améliorée pour une meilleure connectivité et compatibilité
- **Fiabilité** : correction de plusieurs bogues pour une stabilité globale améliorée

## Version 5.4 — Prise en charge des Delphi modernes

- **Environnement de développement** : ajout de la prise en charge de Delphi XE2 pour le développement d'applications modernes
- **Stabilité** : plusieurs correctifs implémentés pour une fiabilité améliorée

## Version 5.3 — Améliorations du traitement vidéo

- **Effets visuels** : effets vidéo améliorés avec options supplémentaires et de meilleures performances
- **Vidéo réseau** : prise en charge des caméras IP améliorée pour une compatibilité plus large
- **Fiabilité** : correction de multiples bogues pour un fonctionnement plus stable

## Version 5.2 — Améliorations du traitement d'images

- **Effets visuels** : amélioration des effets vidéo et de la fonctionnalité de capteur d'images vidéo
- **Stabilité** : plusieurs correctifs pour une fiabilité accrue

## Version 5.1 — Améliorations vidéo réseau et effets

- **Intégration caméra IP** : prise en charge des caméras IP améliorée pour une meilleure connectivité
- **Traitement visuel** : qualité et performances des effets vidéo améliorées
- **Fiabilité** : correction de divers problèmes pour une meilleure stabilité

## Version 5.0 — Extension majeure de la prise en charge des formats

- **Vidéo réseau** : ajout de la prise en charge des caméras IP RTSP/HTTP (MJPEG/MPEG-4/H264 avec ou sans audio)
- **Format moderne** : implémentation de la sortie WebM pour la compatibilité avec les standards web ouverts
- **Flexibilité de format** : ajout des sorties MPEG-1/2/4 et FLV via l'intégration FFMPEG

## Version 4.22 — Améliorations de la capture d'écran

- **Enregistrement de bureau** : correction de bogues dans le filtre de capture d'écran pour une meilleure qualité d'enregistrement

## Version 4.21 — Améliorations de la capture d'écran

- **Enregistrement de bureau** : implémentation de multiples correctifs et améliorations dans le filtre de capture d'écran

## Version 4.2 — Amélioration du traitement audio

- **Effets sonores** : filtre d'effets audio amélioré avec une meilleure qualité et de meilleures performances

## Version 4.1 — Intégration Delphi moderne

- **Environnement de développement** : ajout de la prise en charge de Delphi 2010 pour l'édition Delphi
- **Stabilité** : plusieurs correctifs pour une fiabilité améliorée
