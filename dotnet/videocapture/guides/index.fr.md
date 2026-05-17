---
title: Guides Video Capture — multi-caméras et enregistrement C#
description: Guides étape par étape pour le VisioForge Video Capture SDK : sync multi-caméras, photos webcam, pré-enregistrement et capture d'écran en C#.
sidebar_label: Guides supplémentaires
order: 1
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - Windows
  - Capture
primary_api_classes:
  - VideoCaptureCore

---

# Guides et tutoriels avancés Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble

Explorez des techniques d'implémentation avancées, des guides d'utilisation spécialisés et des tutoriels pour le Video Capture SDK .Net. Ces ressources traitent de scénarios de développement spécifiques qui nécessitent des approches personnalisées, notamment la synchronisation de plusieurs objets de capture, l'intégration webcam, les techniques de capture DirectShow et plus encore.

## Guides disponibles

Cette collection sélectionnée de guides aborde des fonctionnalités avancées spécifiques au sein du Video Capture SDK .Net. Chaque guide fournit des instructions pratiques et des informations pour vous aider à implémenter efficacement des fonctionnalités complexes.

### Guides de prise en main

* [**Enregistrer la vidéo d'une webcam en C#**](save-webcam-video.md) — Guide complet pour capturer et enregistrer la vidéo d'une webcam vers MP4 ou WebM avec C#, avec encodage accéléré par GPU, capture d'instantanés et déploiement multiplateforme.

* [**Enregistrer la vidéo d'une webcam en VB.NET**](record-webcam-vb-net.md) — Guide VB.NET complet pour enregistrer la vidéo d'une webcam vers des fichiers MP4, incluant énumération des périphériques, sélection de format, capture d'écran et configuration de sortie avec des exemples Visual Basic complets.

* [**Capture d'écran en VB.NET**](screen-capture-vb-net.md) — Guide VB.NET complet pour enregistrer l'écran du bureau vers MP4, incluant capture plein écran et de région, prise en charge multi-moniteurs, audio système et enregistrement loopback, avec des exemples Visual Basic complets.

### Techniques de synchronisation

* [**Synchroniser plusieurs objets de capture**](start-in-sync.md) — Dans de nombreuses applications vidéo professionnelles, telles que la couverture multi-caméras d'événements, les systèmes de surveillance avancés ou l'enregistrement vidéo immersif à 360 degrés, la capacité de synchroniser précisément plusieurs instances de capture vidéo est primordiale. Ce guide explore les méthodologies d'initialisation et de coordination de plusieurs objets `VideoCaptureCore`, garantissant qu'ils démarrent, s'arrêtent et enregistrent à l'unisson. Il aborde les défis potentiels tels que l'alignement des horodatages et la gestion des ressources, en proposant des solutions pour parvenir à une capture multi-sources synchronisée et sans accroc. Implémenter une synchronisation robuste est essentiel pour produire du contenu vidéo de qualité professionnelle où le timing et la cohérence entre différents angles ou sources sont critiques.

### Intégration de caméras et techniques de capture

Explorez des guides spécialisés sur l'intégration de diverses fonctionnalités de caméra et la maîtrise de différentes technologies de capture.

* [**Implémentation de la capture photo via webcam**](make-photo-using-webcam.md) — Au-delà de l'enregistrement vidéo continu, la possibilité de capturer des images fixes de haute qualité avec des webcams est une exigence fréquente dans diverses applications. Ce guide étape par étape détaille comment implémenter une fonctionnalité de capture photo robuste. Il couvre la sélection des périphériques, la configuration de la résolution, les choix de format d'image (comme JPEG, PNG, BMP) et l'enregistrement des images capturées. Les cas d'usage courants incluent l'intégration de la capture de photo de profil dans les formulaires d'inscription utilisateur, le développement d'utilitaires simples de numérisation de documents, ou l'ajout de capacités d'instantané aux applications de sécurité et de surveillance. Le guide simplifie le processus, permettant aux développeurs d'ajouter rapidement des fonctionnalités de capture d'images fixes utiles.

* [**Pré-enregistrement d'événement**](pre-event-recording.md) — Implémentez un enregistrement à tampon circulaire qui capture continuellement la vidéo et écrit des clips d'événements sur le disque lors d'un déclenchement, y compris les séquences antérieures à l'événement.

## Ressources supplémentaires

Au-delà des guides spécifiques listés ci-dessus, nous offrons une mine de ressources complémentaires pour soutenir votre parcours de développement avec le Video Capture SDK .Net.

### Exemples de code

Notre vaste [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) est une mine d'or d'exemples d'implémentation pratiques. Ces exemples ne sont pas seulement des extraits, mais souvent des mini-applications complètes démontrant diverses capacités du SDK sur différents frameworks .NET tels que WPF, WinForms et les applications console.

### Support technique

Si vous rencontrez des difficultés lors de l'implémentation, notre documentation technique fournit des solutions détaillées aux questions de développement courantes.
