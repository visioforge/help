---
title: Guide de déploiement multiplateforme du SDK VisioForge .NET
description: Déployez des applications .NET sur Windows, macOS, iOS, Android et Linux avec les bibliothèques natives, les dépendances et l'intégration des frameworks UI.
sidebar_label: Déploiement
order: 17
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS

---

# Guide de déploiement multiplateforme pour le SDK VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au déploiement du SDK VisioForge

La suite SDK VisioForge offre de puissantes capacités multimédias pour les applications .NET, prenant en charge la capture vidéo, l'édition, la lecture et le traitement multimédia avancé sur plusieurs plateformes. Un déploiement correct est essentiel pour garantir que vos applications fonctionnent comme prévu et tirent pleinement parti du potentiel de ces SDK.

Ce guide complet présente le processus de déploiement des applications créées avec les SDK .NET multiplateformes de VisioForge, et vous aide à naviguer parmi les exigences propres à chaque système d'exploitation pris en charge.

## Vue d'ensemble du déploiement

Le déploiement d'applications créées avec les SDK VisioForge requiert une attention particulière aux dépendances et configurations spécifiques à chaque plateforme. Le processus de déploiement varie sensiblement selon votre plateforme cible en raison de différences au niveau de :

- Exigences en bibliothèques natives
- Dépendances vis-à-vis des frameworks multimédias
- Mécanismes d'accès au matériel
- Méthodes de distribution des paquets

### Considérations clés pour le déploiement

Avant de commencer le processus de déploiement, prenez en compte ces facteurs critiques :

1. **Architecture de la plateforme cible** : assurez-vous de sélectionner l'architecture appropriée (x86, x64, ARM64) pour votre plateforme de déploiement
2. **Dépendances requises** : certaines plateformes nécessitent des bibliothèques supplémentaires non incluses dans les paquets NuGet
3. **Compatibilité du framework** : vérifiez la compatibilité entre votre version de .NET et le système d'exploitation cible
4. **Intégration des bibliothèques natives** : comprenez comment les bibliothèques natives sont intégrées et chargées sur chaque plateforme
5. **Choix du framework UI** : choisissez le paquet d'intégration UI approprié pour votre framework

## Déploiement spécifique à chaque plateforme

### Déploiement Windows

Le déploiement Windows est le plus simple, avec une prise en charge complète des paquets NuGet couvrant toutes les dépendances :

- **Distribution des paquets** : tous les composants sont disponibles via NuGet
- **Prise en charge des architectures** : les architectures x86 et x64 sont entièrement prises en charge
- **Bibliothèques natives** : déployées automatiquement avec votre application
- **Options de framework UI** : Windows Forms, WPF, WinUI, Avalonia, Uno et MAUI sont pris en charge

Pour des instructions détaillées sur le déploiement Windows, consultez le [guide de déploiement Windows](Windows.md).

### Déploiement Android

Le déploiement Android nécessite une configuration spécifique pour l'extraction des bibliothèques natives et les permissions :

- **Distribution des paquets** : composants principaux disponibles via NuGet
- **Prise en charge des architectures** : ARM64, ARMv7 et x86_64 prises en charge
- **Bibliothèques natives** : nécessitent une configuration appropriée pour leur extraction au bon emplacement
- **Permissions** : les permissions de caméra, de microphone et de stockage doivent être demandées explicitement
- **Intégration UI** : contrôles de vue vidéo spécifiques à Android requis

Les applications Android utilisent une seule bibliothèque native qui doit être correctement déployée. Consultez le [guide de déploiement Android](Android.md) pour les instructions complètes.

### Déploiement macOS

Le déploiement macOS nécessite l'installation supplémentaire de la bibliothèque GStreamer :

- **Distribution des paquets** : composants principaux disponibles via NuGet, GStreamer nécessite une installation manuelle
- **Prise en charge des architectures** : Intel (x64) et Apple Silicon (ARM64) prises en charge
- **Bibliothèques natives** : plusieurs bibliothèques non managées requises
- **Options de framework** : macOS natif, MAUI et Avalonia pris en charge
- **Intégration au bundle** : une attention particulière est nécessaire pour une structure correcte du bundle d'application

Les déploiements macOS peuvent nécessiter des configurations d'entitlements et de permissions spécifiques. Consultez le [guide de déploiement macOS](macOS.md) pour des instructions détaillées.

### Déploiement iOS

Le déploiement iOS présente des défis particuliers liés aux restrictions de la plateforme Apple :

- **Distribution des paquets** : composants principaux disponibles via NuGet
- **Prise en charge des architectures** : architecture ARM64 prise en charge
- **Règles de l'App Store** : considérations particulières pour les soumissions à l'App Store
- **Bibliothèques natives** : une seule bibliothèque binaire non managée à déployer
- **Intégration UI** : contrôles de vue vidéo spécifiques à iOS requis

Les applications iOS exigent des profils de provisionnement et des entitlements appropriés. Le [guide de déploiement iOS](iOS.md) fournit des instructions complètes.

### Déploiement Ubuntu/Linux

Le déploiement Linux requiert l'installation manuelle des dépendances GStreamer :

- **Distribution des paquets** : composants principaux disponibles via NuGet, GStreamer nécessite des paquets système
- **Prise en charge des architectures** : architecture x64 principalement prise en charge
- **Dépendances système** : les paquets requis doivent être installés sur le système cible
- **Considérations de distribution** : différentes distributions Linux peuvent nécessiter des paquets de dépendances différents
- **Options UI** : principalement le framework Avalonia UI est pris en charge

Le déploiement Linux implique souvent une gestion des paquets spécifique à la distribution. Le [guide de déploiement Ubuntu](Ubuntu.md) fournit des instructions pour les distributions basées sur Ubuntu.

### Déploiement Uno Platform

Uno Platform permet de déployer des applications à partir d'un code source unique vers plusieurs plateformes :

- **Distribution des paquets** : composants principaux disponibles via NuGet avec des redistribuables spécifiques à chaque plateforme
- **Plateformes prises en charge** : Windows, Android, iOS, macOS (Catalyst), WebAssembly et Linux Desktop
- **Prise en charge des architectures** : varie selon la plateforme cible
- **Bibliothèques natives** : redistribuables spécifiques à chaque plateforme requis pour chaque cible
- **Intégration UI** : le contrôle `VideoView` spécifique à Uno s'adapte à chaque plateforme

Uno Platform simplifie le développement multiplateforme tout en tirant parti des capacités natives. Consultez le [guide de déploiement Uno Platform](uno.md) pour des instructions complètes.

### Exigences d'exécution

Les appareils cibles doivent satisfaire à ces exigences minimales :

- **Windows** : Windows 7 ou ultérieur (x86 ou x64)
- **macOS** : macOS 10.15 (Catalina) ou ultérieur (x64 ou ARM64)
- **iOS** : iOS 14.0 ou ultérieur (ARM64)
- **Android** : Android 7.0 (API niveau 24) ou ultérieur
- **Linux** : Ubuntu 20.04 LTS ou ultérieur (x64 ou ARM64)

## Difficultés courantes de déploiement

### Problèmes de chargement des bibliothèques natives

L'un des problèmes de déploiement les plus fréquents concerne les échecs de chargement des bibliothèques natives :

- **Symptômes** : exceptions à l'exécution mentionnant `DllNotFoundException` ou similaire
- **Causes** : architecture incorrecte, dépendances manquantes ou extraction inappropriée
- **Solutions** : vérifiez les références de paquets, contrôlez la configuration de déploiement, assurez-vous que les bibliothèques sont au bon emplacement

### Contraintes de permissions et de sécurité

Les systèmes d'exploitation modernes appliquent des politiques de sécurité strictes :

- **Accès à la caméra** : nécessite une permission explicite sur toutes les plateformes mobiles
- **Accès au stockage** : les restrictions sur le système de fichiers varient selon la plateforme
- **Utilisation du réseau** : peut nécessiter des entitlements ou des entrées de manifeste spécifiques
- **Exécution en arrière-plan** : règles spécifiques à chaque plateforme pour le traitement multimédia en arrière-plan

### Considérations de performance

Le traitement multimédia peut être très consommateur de ressources :

- **Utilisation du CPU** : mettez en œuvre un threading approprié pour éviter le gel de l'interface utilisateur
- **Gestion de la mémoire** : surveillez et optimisez l'utilisation de la mémoire pour les fichiers multimédias volumineux
- **Consommation d'énergie** : équilibrez les paramètres de qualité avec les considérations d'autonomie de la batterie

## Liste de contrôle de déploiement

Utilisez cette liste de contrôle pour garantir un déploiement réussi :

- ✅ Paquets NuGet appropriés sélectionnés pour la plateforme et l'architecture cibles
- ✅ Dépendances spécifiques à la plateforme installées et configurées
- ✅ SDK correctement initialisé et nettoyé
- ✅ Contrôles de vue vidéo appropriés intégrés
- ✅ Permissions requises demandées et justifiées
- ✅ Application testée sur la plateforme cible dans des conditions réalistes
- ✅ Métriques de performance validées pour une expérience utilisateur acceptable
- ✅ Gestion des erreurs implémentée pour une récupération en douceur

## Déploiement de la vision par ordinateur

Le SDK de vision par ordinateur est un paquet NuGet distinct. Consultez le [guide de déploiement de la vision par ordinateur](computer-vision.md) pour plus d'informations.

## Ressources supplémentaires

- [Dépôt GitHub VisioForge](https://github.com/visioforge/.Net-SDK-s-samples) — exemples de code et projets d'exemple
- [Documentation de l'API](https://api.visioforge.org/dotnet/) — référence complète de l'API
- [Portail de support](https://support.visioforge.com/) — support technique et base de connaissances
