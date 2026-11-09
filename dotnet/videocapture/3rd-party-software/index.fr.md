---
title: Diffuser vers OBS, FFmpeg et Zoom via caméra virtuelle en C#
description: Diffusez la vidéo vers OBS et FFMPEG depuis le VisioForge Video Capture SDK. Configuration de la caméra virtuelle DirectShow pour WinForms, WPF et Console.
sidebar_label: Utilisation de logiciels tiers
order: 4
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - Streaming

---

# Intégration de logiciels tiers avec Video Capture SDK

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Le Video Capture SDK .NET offre des capacités robustes d'intégration avec diverses applications logicielles tierces. Cette intégration étend les fonctionnalités de vos applications et permet une plus grande flexibilité dans les flux de travail de traitement vidéo.

## Comment fonctionne l'intégration

Le SDK utilise le Virtual Camera SDK comme pont entre notre Video Capture SDK et les applications tierces. Ce pont crée un périphérique de caméra virtuelle qui peut être détecté et utilisé par toute application compatible DirectShow dans votre environnement de développement.

Le Virtual Camera SDK fait l'objet d'une licence distincte : sans sa clé de licence, indiquée dans `Virtual_Camera_Output_LicenseKey`, la caméra virtuelle affiche une bande TRIAL, et une licence Video Capture SDK .NET ne la supprime pas.

### Pont vidéo

La technologie de caméra virtuelle permet aux flux vidéo capturés d'être transmis de manière transparente vers des applications externes sans perte de qualité ni impact significatif sur les performances.

### Pont audio

En plus de la vidéo, un pont audio est également fourni, permettant une intégration audiovisuelle complète avec les logiciels externes.

## Applications compatibles

La caméra virtuelle fonctionne avec de nombreuses applications compatibles DirectShow, notamment :

- OBS (Open Broadcaster Software)
- FFMPEG
- VLC Media Player
- Zoom et autres logiciels de visioconférence compatibles avec les caméras DirectShow
- Applications DirectShow personnalisées

## Tutoriels détaillés

Nos tutoriels pas à pas vous guident dans le processus d'intégration avec les applications populaires :

- [Intégration streaming FFMPEG](ffmpeg-streaming.md) — apprenez à utiliser FFMPEG avec le SDK pour de puissantes capacités de streaming
- [Configuration streaming OBS](obs-streaming.md) — guide détaillé pour l'intégration avec Open Broadcaster Software
  
## Ressources de développement

Nous fournissons une documentation et des exemples étendus pour vous aider à implémenter ces intégrations dans vos projets logiciels. L'intégration fonctionne sur toutes les plateformes prises en charge :

- Applications WinForms
- Applications WPF (Windows Presentation Foundation)
- Applications console

---

Pour des exemples d'implémentation et des exemples de code supplémentaires, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
