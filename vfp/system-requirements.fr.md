---
title: Configuration requise du VisioForge Video Fingerprinting SDK
description: Exigences matérielles et logicielles pour exécuter le VisioForge Video Fingerprinting SDK sur les plateformes Windows, Linux et macOS.
tags:
  - Video Fingerprinting SDK
  - .NET
  - DirectShow
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting

---

# Configuration système requise

Cette page décrit la configuration système requise pour exécuter le Video Fingerprinting SDK sur les plateformes prises en charge. Ces exigences s'appliquent à la fois aux versions .NET et C++ du SDK.

## Plateformes prises en charge

### Windows

- **Système d'exploitation** : Windows 10 version 1903+ ou Windows 11
- **Architecture** : x86, x64 ou ARM64 (ARM64 uniquement pour le SDK .NET)
- **Runtime** : Microsoft Visual C++ Redistributables 2019 ou ultérieur
- **Bibliothèques supplémentaires** : filtres DirectShow pour une prise en charge étendue des codecs (optionnel)

### Linux

- **Distributions** : Ubuntu 20.04+, Debian 11+, RHEL 8+, CentOS 8+, Fedora 34+ (Fedora pour le SDK C++)
- **Architecture** : x64 ou ARM64
- **Dépendances** : GStreamer 1.18+ avec les plugins base et good
- **Serveur d'affichage** : X11 ou Wayland (pour le SDK .NET avec GUI)

### macOS

- **Système d'exploitation** : macOS 12 (Monterey) ou ultérieur
- **Architecture** : Intel x64 ou Apple Silicon (M1/M2/M3)
- **Dépendances** : aucune dépendance runtime supplémentaire requise

## Configuration matérielle requise

### Configuration minimale

- **Processeur** : CPU double cœur (Intel Core i3 ou équivalent AMD)
- **Mémoire** : 
  - SDK .NET : 4 Go de RAM
  - SDK C++ : 2 Go de RAM disponibles pour l'application
- **Stockage** : 
  - SDK .NET : 500 Mo d'espace disque libre pour le SDK
  - SDK C++ : 100 Mo pour les fichiers du SDK + espace pour le traitement vidéo
- **GPU** : toute carte graphique compatible DirectX 9 (Windows uniquement)

### Configuration recommandée

- **Processeur** : CPU quadricœur (Intel Core i5 ou AMD Ryzen 5)
- **Mémoire** : 
  - SDK .NET : 8 Go de RAM ou plus
  - SDK C++ : 4 Go de RAM ou plus disponibles pour l'application
- **Stockage** : 
  - SDK .NET : 2 Go d'espace disque libre pour le SDK et les fichiers temporaires
  - SDK C++ : 500 Mo pour le SDK + espace de traitement temporaire
- **GPU** : carte graphique dédiée avec prise en charge de l'accélération matérielle

### Considérations de performance

- **Vitesse de traitement** : évolue linéairement avec la durée de la vidéo et le nombre de cœurs CPU
- **Utilisation mémoire** : augmente avec la résolution de la vidéo
- **Stockage** : le stockage SSD améliore significativement la vitesse de traitement (opérations d'E/S 2 à 3 fois plus rapides)
- **Parallélisation** : plusieurs cœurs CPU permettent un traitement parallèle
- **Accélération matérielle** : le décodage vidéo matériel peut fournir une accélération de 3 à 5x (SDK C++)

## Exigences de l'environnement de développement

### SDK .NET

- **Version de .NET** : 
  - Windows : .NET Framework 4.6.1+ ou .NET 6.0+
  - Linux/macOS : .NET 6.0+
- **IDE** : Visual Studio 2019+ (Windows), Visual Studio Code ou JetBrains Rider

### SDK C++

- **Compilateur Windows** : Visual Studio 2019+ (recommandé) ou MinGW-w64
- **Compilateur Linux** : GCC 7+ ou Clang 6+
- **Compilateur macOS** : Xcode 12+ avec Command Line Tools
- **Outils de build** : CMake 3.10+ (optionnel mais recommandé pour Linux/macOS)

## Notes supplémentaires spécifiques à chaque plateforme

### Windows
- Les filtres DirectShow peuvent enrichir la prise en charge des codecs pour les formats hérités
- Windows Media Foundation est utilisé pour l'accélération matérielle lorsqu'elle est disponible

### Linux
- Vérifiez que les plugins GStreamer sont correctement installés : `sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good`
- Pour les applications GUI, un serveur d'affichage X11 ou Wayland est requis

### macOS
- Les processeurs Apple Silicon (M1/M2/M3) sont entièrement pris en charge avec des performances natives
- La traduction Rosetta 2 est prise en charge pour les binaires Intel sur Apple Silicon

## Exigences réseau

Pour le stockage et la comparaison d'empreintes basés sur le cloud :
- **Bande passante** : minimum 1 Mbit/s pour l'envoi/téléchargement d'empreintes
- **Latence** : < 100 ms pour les scénarios de traitement en temps réel
- **Protocoles** : HTTPS pour la communication API, protocole wire MongoDB pour les opérations de base de données

## Prise en charge de la virtualisation et des conteneurs

- **Docker** : entièrement pris en charge sur les hôtes Linux
- **Machines virtuelles** : pris en charge avec une surcharge de performance (20 à 30 % plus lent)
- **WSL2** : pris en charge pour le SDK Linux sous Windows
- **Plateformes cloud** : compatible avec AWS EC2, Azure VMs, Google Cloud Compute

## Étapes suivantes

- [Prise en main du SDK .NET](dotnet/getting-started.md) — configurer le SDK .NET
- [Prise en main du SDK C++](cpp/getting-started.md) — configurer le SDK C++
- [Comprendre l'empreinte vidéo](understanding-video-fingerprinting.md) — apprendre comment fonctionne la technologie
