---
title: Paquets NuGet VisioForge Computer Vision pour .NET
description: Déployez VisioForge CV et CVD pour la vision par ordinateur .NET sur Windows, Linux et macOS. Détection de visages et d'objets.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows

---

# Guide d'implémentation de la vision par ordinateur

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }, [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }, [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Vue d'ensemble des paquets disponibles

Notre SDK propose deux paquets NuGet qui apportent des capacités de vision par ordinateur à vos applications :

1. **[VisioForge.DotNet.Core.CV](https://www.nuget.org/packages/VisioForge.DotNet.Core.CV/)** — paquet CV exclusif à Windows.
2. **[VisioForge.DotNet.Core.CVD](https://www.nuget.org/packages/VisioForge.DotNet.Core.CVD/)** — paquet CV multiplateforme pour Windows, Linux et macOS.

Les deux paquets fournissent une API cohérente pour intégrer directement les fonctionnalités de vision par ordinateur dans vos applications .NET.

## Exigences de déploiement

### Paquet CV spécifique à Windows

#### Processus d'installation

Le paquet CV spécifique à Windows est conçu pour une intégration sans heurts :

- Installez simplement le paquet NuGet via le gestionnaire de paquets de votre choix
- Aucune étape de déploiement supplémentaire n'est nécessaire
- Prêt à l'emploi immédiatement après l'installation

### Paquet CVD multiplateforme

Notre paquet CVD multiplateforme requiert des configurations spécifiques en fonction de votre système d'exploitation :

#### Configuration sous Windows

Lors du déploiement sur des systèmes Windows :

- Installez le paquet NuGet via Visual Studio ou la CLI .NET
- Aucune dépendance ou configuration supplémentaire n'est requise
- Fonctionne immédiatement avec les installations Windows standard

#### Configuration Ubuntu Linux

Pour les systèmes Ubuntu Linux, installez les dépendances suivantes :

```bash
sudo apt-get install libgdiplus libopenblas-dev libx11-6
```

Ces paquets fournissent des fonctionnalités essentielles :

- `libgdiplus` : assure la compatibilité avec System.Drawing
- `libopenblas-dev` : optimise les opérations matricielles pour les algorithmes de vision par ordinateur
- `libx11-6` : gère la bibliothèque cliente du protocole X Window System

#### Instructions de configuration macOS

Pour les environnements macOS, utilisez Homebrew pour installer les dépendances requises :

```bash
brew install --cask xquartz
brew install mono-libgdiplus
```

Ces composants permettent :

- XQuartz : fournit la fonctionnalité X11 sur macOS
- mono-libgdiplus : assure la compatibilité avec System.Drawing

## Ressources supplémentaires

Pour des exemples d'implémentation et des conseils techniques :

- Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour de nombreux exemples de code
- Explorez les implémentations pratiques dans divers cas d'usage
- Accédez aux exemples et solutions contribués par la communauté

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
