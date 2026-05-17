---
title: Journal des modifications du Video Fingerprinting SDK
description: Suivez les mises à jour du VisioForge Video Fingerprinting SDK : nouvelles fonctionnalités, améliorations de performance et changements d'API .NET et C++.
tags:
  - Video Fingerprinting SDK
  - .NET
  - C++
  - Windows
  - macOS
  - Linux
  - Capture
  - Fingerprinting

---

# Historique des versions du Video Fingerprinting SDK

## Version 12.1 — améliorations de performance et de fonctionnalités

### Améliorations du framework .NET

* **Nouvelle capacité d'empreinte** : introduction de la classe `VFPFingerprintFromFrames` permettant aux développeurs de générer des empreintes vidéo directement à partir de séquences d'images RGB24
* **Modernisation de l'API** : implémentation entièrement repensée de l'API async/await pour un meilleur traitement asynchrone
* **Optimisation du moteur** : performance du moteur d'empreinte principal significativement améliorée grâce à des algorithmes de traitement enrichis

## Version 12.0 — intégration de base de données et accélération matérielle

### Mises à jour du framework .NET

* **Stockage multi-empreintes** : ajout de la nouvelle classe `VFPFingerPrintDB` pour stocker efficacement plusieurs empreintes dans un seul fichier binaire
* **Intégration de Media Monitoring Tool** : mise à jour de l'application Media Monitoring Tool pour tirer parti des nouvelles capacités de base de données
* **Dépendances mises à jour** : intégration de la dernière version de FFMPEG pour des capacités améliorées de traitement vidéo
* **Modification d'exigence framework** : exigence minimale .NET Framework portée à la version 4.7.2
* **Journalisation externe** : ajout de NLog en tant que dépendance externe pour des capacités de journalisation enrichies
* **Accélération GPU** : prise en charge améliorée de l'accélération matérielle via les décodeurs vidéo GPU Nvidia, Intel et AMD

## Version 11.0 — modernisation du moteur

### Implémentation .NET

* **Installation autonome** : publication en tant que paquet d'installation indépendant ne nécessitant pas d'autres installations de SDK .NET
* **Moteur de source vidéo** : implémentation d'un nouveau moteur pour traiter la vidéo provenant de fichiers et de sources réseau
* **Prise en charge des périphériques de capture** : développement d'un nouveau moteur pour gérer la vidéo provenant des périphériques de capture
* **Améliorations principales** : moteur d'empreinte mis à jour avec les algorithmes les plus récents

### Prise en charge C++ sous Linux

* **Résolution de bugs** : correction de plusieurs problèmes affectant les implémentations Linux
* **Mises à jour du moteur** : moteur d'empreinte amélioré avec des optimisations spécifiques à la plateforme

## Version 10.0 — fonctionnalités de personnalisation

### Améliorations .NET

* **Contrôle de la résolution** : ajout d'options de résolution personnalisée pour la vidéo source
* **Fonctionnalité de rognage** : implémentation de capacités de rognage personnalisé pour le matériau source
* **Mises à jour du moteur** : mise à niveau des moteurs de décodage et d'empreinte

### Améliorations C++ sous Linux

* **Application de démonstration** : mise à jour de la démo Media Monitoring Tool avec la dernière compatibilité FFMPEG
* **Améliorations de stabilité** : résolution de divers bugs affectant les performances

## Version 3.1 — version d'optimisation

### Améliorations générales

* **Correctifs de bugs** : prise en compte de problèmes mineurs affectant la stabilité globale
* **Mises à jour du moteur** : moteur de traitement amélioré pour l'implémentation .NET
* **Changement de licence** : les outils Media Monitoring Tool (Live) et Duplicates Video Search sont désormais disponibles pour un usage commercial gratuit

## Version 3.0 — première version publique

### Fonctionnalités clés

* Première version publique du Video Fingerprinting SDK
* Introduction des capacités principales d'empreinte pour l'identification du contenu vidéo
* Établissement des fondations pour le développement multiplateforme
