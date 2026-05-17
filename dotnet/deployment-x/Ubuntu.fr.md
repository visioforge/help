---
title: Déployer du .NET sur Ubuntu avec GStreamer et VisioForge
description: Applications multimédias .NET sur Ubuntu Linux — configuration GStreamer, configuration matérielle et performances multiplateformes.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Linux
  - GStreamer
  - NuGet

---

# Guide de déploiement Ubuntu pour les applications SDK VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Le déploiement d'applications .NET avec les SDK VisioForge sur Ubuntu Linux offre de nombreux avantages, notamment la compatibilité multiplateforme, l'accès à du matériel spécifique à Linux et la possibilité d'exécuter vos applications multimédias dans des environnements allant de l'infrastructure serveur aux appareils en périphérie. Ce guide complet vous accompagnera tout au long du processus de configuration de votre environnement Ubuntu, d'installation des dépendances nécessaires et de déploiement de votre application .NET propulsée par VisioForge.

La famille de SDK VisioForge fonctionne sur Ubuntu et d'autres distributions Linux qui prennent en charge les bibliothèques `GStreamer`. Les plateformes prises en charge incluent également les appareils `Nvidia Jetson` et `Raspberry Pi`, ce qui le rend parfaitement adapté à un large éventail d'applications, des logiciels multimédias de bureau aux solutions IoT.

## Configuration requise

Avant de déployer votre application, assurez-vous que votre environnement Ubuntu satisfait à ces exigences minimales :

- Ubuntu 20.04 LTS ou ultérieur (22.04 LTS et ultérieur recommandés)
- Runtime .NET 7.0 ou ultérieur
- Au moins 4 Go de RAM (8 Go recommandés pour le traitement vidéo)
- Architecture x86_64 ou ARM64
- Connexion Internet pour l'installation des paquets

## Installation et configuration

### Installation de .NET

Téléchargez le dernier paquet d'[installation .NET](https://dotnet.microsoft.com/en-us/download/dotnet) depuis le site Microsoft et suivez les instructions d'installation.

## Installation de GStreamer

GStreamer constitue l'épine dorsale multimédia des SDK VisioForge sur les plateformes Linux. Il fournit des fonctionnalités essentielles pour la capture, le traitement et la lecture audio et vidéo.

### Paquets GStreamer requis

Installez les paquets GStreamer suivants à l'aide d'apt-get. Nous exigeons la v1.22.0 ou ultérieure, mais la v1.24.0+ est fortement recommandée pour accéder aux dernières fonctionnalités et optimisations :

- `gstreamer1.0-plugins-base` : plugins de base essentiels
- `gstreamer1.0-plugins-good` : plugins de haute qualité, bien testés
- `gstreamer1.0-plugins-bad` : plugins plus récents de qualité variable
- `gstreamer1.0-alsa` : prise en charge audio ALSA
- `gstreamer1.0-gl` : prise en charge du rendu OpenGL
- `gstreamer1.0-pulseaudio` : intégration PulseAudio
- `libges-1.0-0` : GStreamer Editing Services
- `gstreamer1.0-libav` : intégration FFMPEG (FACULTATIF mais recommandée pour une prise en charge étendue des formats)

### Script d'installation complet

Les commandes suivantes mettront à jour vos dépôts de paquets et installeront tous les composants GStreamer requis :

```bash
sudo apt update
```

```bash
sudo apt install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-alsa gstreamer1.0-gl gstreamer1.0-pulseaudio gstreamer1.0-libav libges-1.0-0
```

### Exigences supplémentaires pour Raspberry Pi

Pour Raspberry Pi, vous devez en outre installer les paquets suivants :

```bash
sudo apt install gstreamer1.0-libcamera
```

### Vérification de l'installation de GStreamer

Après installation, vérifiez votre configuration GStreamer en exécutant :

```bash
gst-inspect-1.0 --version
```

Cette commande doit afficher la version de GStreamer installée. Assurez-vous qu'elle satisfait à l'exigence minimale (1.22.0+) ou qu'elle affiche idéalement 1.24.0 ou une version plus récente.

## Paquets NuGet requis

Lors du déploiement de votre application .NET sur Ubuntu, vous devez inclure des paquets NuGet spécifiques à la plateforme qui fournissent les bibliothèques natives et les bindings nécessaires.

### Paquet Linux principal supplémentaire

Le paquet [VisioForge.CrossPlatform.Core.Linux.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Linux.x64) contient les bibliothèques natives et les bindings essentiels pour la plateforme Linux .NET. Ce paquet est obligatoire pour tous les déploiements de SDK VisioForge sur Ubuntu.

### Environnement de développement

Vous pouvez utiliser Rider pour développer votre projet sous Linux. Consultez la page d'installation de [Rider](../install/rider.md) pour plus d'informations.

## Déploiement de l'application

Suivez ces étapes pour déployer votre application sur Ubuntu :

### Publication de votre application

Pour créer un déploiement autonome qui inclut toutes les dépendances du runtime .NET :

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

Pour des déploiements plus légers lorsque la machine cible a déjà .NET installé :

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

### Structure de déploiement

Votre dossier de déploiement doit contenir :

- L'exécutable de votre application
- Les DLL de l'application
- Les assemblies du SDK VisioForge
- Les bibliothèques natives Linux issues des paquets NuGet VisioForge

### Définition des permissions d'exécution

Assurez-vous que l'exécutable de votre application dispose des permissions appropriées :

```bash
chmod +x ./YourApplicationName
```

## Considérations matérielles

### Prise en charge des caméras

Ubuntu prend en charge divers types de caméras :

- **Webcams USB** : la plupart des webcams USB fonctionnent dès le départ
- **Caméras IP** : prises en charge via les flux RTSP et HTTP
- **Caméras professionnelles** : de nombreuses caméras professionnelles disposant de pilotes Linux sont prises en charge
- **Périphériques virtuels** : v4l2loopback peut être utilisé pour créer une caméra virtuelle

Pour lister les caméras disponibles :

```bash
v4l2-ctl --list-devices
```

### Périphériques audio

La capture et la lecture audio sont prises en charge via :

- ALSA (Advanced Linux Sound Architecture)
- PulseAudio

Pour lister les périphériques audio disponibles :

```bash
arecord -L  # Périphériques d'enregistrement
aplay -L    # Périphériques de lecture
```

## Dépannage

### Problèmes de permissions

Les problèmes d'accès à la caméra ou aux périphériques audio peuvent souvent être résolus en ajoutant votre utilisateur aux groupes appropriés :

```bash
sudo usermod -a -G video,audio $USER
```

N'oubliez pas de vous déconnecter puis de vous reconnecter pour que les modifications de groupe prennent effet.

### Optimisation des performances

Pour des performances optimales sous Ubuntu :

- Utilisez la dernière version de GStreamer (1.24.0+)
- Activez l'accélération matérielle lorsqu'elle est disponible
- Pour les GPU NVIDIA, installez les paquets CUDA et nvcodec appropriés
- Ajustez la priorité du processus à l'aide de `nice` pour les applications gourmandes en ressources

## Conclusion

Le déploiement d'applications SDK VisioForge sur Ubuntu fournit un environnement puissant et flexible pour les applications multimédias. En suivant ce guide, vous pouvez vous assurer que votre application .NET tire pleinement parti des capacités de l'écosystème SDK VisioForge sur les plateformes Linux.

Pour des scénarios de déploiement spécifiques ou une assistance au dépannage, consultez la documentation complète disponible sur le site VisioForge ou contactez notre équipe de support technique.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
