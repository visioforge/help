---
title: SDK .NET VisioForge — configuration requise et plateformes
description: Windows 10/11, macOS 12+, Ubuntu 22.04+, iOS 12+, Android 10+. .NET 6 à 9, ARM64, conseils mémoire 4K. WPF, WinForms, MAUI, Avalonia compatibles.
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

# Configuration requise pour les SDK .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Ce guide détaille la configuration système et la compatibilité de plateforme requises pour la suite de SDK .NET VisioForge, conçue pour des applications de traitement et de lecture vidéo hautes performances.

## Vue d'ensemble

Débloquez de puissantes capacités vidéo multiplateformes avec les SDK .NET VisioForge, entièrement compatibles avec Windows, Linux, macOS, Android et iOS. Nos SDK offrent une prise en charge robuste de .NET Framework, .NET Core et du .NET moderne 5+ (y compris .NET 8 LTS et .NET 9), permettant une intégration fluide avec WinForms, WPF, WinUI 3, Avalonia, .NET MAUI et Xamarin. Développez des applications vidéo hautes performances avec des paradigmes C# familiers sur tous les principaux systèmes d'exploitation et frameworks d'interface utilisateur.

> **Note importante** : alors que les utilisateurs Windows bénéficient de notre programme d'installation dédié, les développeurs travaillant sur d'autres plateformes doivent privilégier la méthode de distribution par paquet NuGet pour l'implémentation.

## Configuration requise pour l'environnement de développement

Les sections suivantes décrivent les exigences spécifiques pour configurer votre environnement de développement lors de l'utilisation de nos SDK.

### Systèmes d'exploitation pour le développement

Le développement d'applications utilisant nos SDK est pris en charge sur les plateformes suivantes :

#### Windows

* Windows 10 (toutes éditions)
* Windows 11 (toutes éditions)
* Recommandé : dernière mise à jour de fonctionnalités avec les correctifs de sécurité actuels

#### Linux

* Ubuntu 22.04 LTS ou plus récent
* Debian 11 ou plus récent
* Les autres distributions disposant de bibliothèques équivalentes peuvent fonctionner mais ne sont pas officiellement prises en charge

#### macOS

* macOS 12 (Monterey) ou plus récent
* Processeurs Apple Silicon (M1/M2/M3) et Intel pris en charge

### Configuration matérielle requise

Pour une expérience de développement optimale, nous recommandons :

* Processeur : 4 cœurs ou plus, 2,5 GHz ou supérieur
* Mémoire vive : 8 Go minimum, 16 Go recommandés pour les projets complexes
* Stockage : SSD avec au moins 10 Go d'espace libre
* Graphique : GPU compatible DirectX 11 (Windows) ou GPU compatible Metal (macOS)

## Plateformes de déploiement cibles

Nos SDK peuvent être déployés sur une variété de plateformes, permettant une large distribution de vos applications.

### Plateformes de bureau

#### Windows

* Windows 10 (version 1809 ou plus récente)
* Windows 11 (toutes versions)
* Architectures x86 et x64 prises en charge
* Prise en charge ARM64 pour les appareils Windows on ARM

#### Linux

* Ubuntu 22.04 LTS ou plus récent
* Les autres distributions requièrent des bibliothèques et dépendances équivalentes
* Architectures x64 et ARM64 prises en charge

#### macOS

* macOS 12 (Monterey) ou plus récent
* Architectures Intel et Apple Silicon prises en charge nativement
* Rosetta 2 non requis pour les appareils Apple Silicon

### Plateformes mobiles

#### Android

* Android 10 (niveau d'API 29) ou plus récent
* Architectures ARM, ARM64 et x86 prises en charge
* Compatible avec le Google Play Store
* Rendu accéléré matériellement recommandé

#### iOS

* iOS 12 ou versions plus récentes
* Compatible avec iPhone, iPad et iPod Touch
* Prise en charge des architectures ARMv7 et ARM64
* Compatible avec la distribution sur l'App Store

## Compatibilité .NET Framework

Nos SDK offrent une compatibilité étendue avec diverses implémentations .NET :

### .NET Framework

* .NET Framework 4.6.1
* .NET Framework 4.7.x
* .NET Framework 4.8
* .NET Framework 4.8.1

### .NET moderne

* .NET Core 3.1 (LTS)
* .NET 5.0
* .NET 6.0 (LTS)
* .NET 7.0
* .NET 8.0 (LTS)
* .NET 9.0

### Xamarin (hérité)

* Xamarin.iOS 12.0+
* Xamarin.Android 9.0+
* Xamarin.Mac 5.0+

## Intégration de frameworks d'interface utilisateur

Les SDK s'intègrent à un large éventail de frameworks d'interface utilisateur, permettant un développement d'applications flexible :

### Frameworks spécifiques à Windows

* Windows Forms (WinForms)
  * .NET Framework 4.6.1+ et .NET Core 3.1+
  * Options de rendu hautes performances
  * Prend en charge l'intégration au concepteur

* Windows Presentation Foundation (WPF)
  * .NET Framework 4.6.1+ et .NET Core 3.1+
  * Rendu accéléré matériellement
  * Mise en page basée sur XAML avec prise en charge du binding

* Windows UI Library 3 (WinUI 3)
  * Applications de bureau uniquement
  * Composants modernes Fluent Design
  * Intégration au Windows App SDK

### Frameworks multiplateformes

* .NET MAUI
  * Développement unifié pour Windows, macOS, iOS et Android
  * Code d'interface utilisateur partagé entre plateformes
  * Performance native avec base de code partagée

* Avalonia UI
  * Framework UI véritablement multiplateforme
  * Basé sur XAML avec des paradigmes familiers
  * Compatible Windows, Linux, macOS

### Frameworks spécifiques au mobile

* Interface utilisateur native iOS
  * Intégration UIKit
  * Couche de compatibilité SwiftUI
  * Prise en charge des Storyboards et XIB

* macOS / Mac Catalyst
  * Intégration AppKit et UIKit
  * Mac Catalyst pour adapter des applications iPad
  * Éléments d'interface utilisateur macOS natifs

* Interface utilisateur native Android
  * Intégration avec la boîte à outils UI Android
  * Prise en charge des Activities et Fragments
  * Compatibilité avec les composants Material Design

## Méthodes de distribution

### Paquets NuGet

Nos SDK sont disponibles sous forme de paquets NuGet, simplifiant l'intégration dans votre workflow de développement.

### Programme d'installation Windows

Pour les développeurs Windows, nous proposons un programme d'installation dédié qui inclut :

* Binaires et dépendances du SDK
* Documentation et projets d'exemples
* Composants d'intégration à Visual Studio
* Outils et utilitaires pour développeurs

## Considérations de performance

### Besoins en mémoire

* Empreinte mémoire de base : ~50 Mo
* Traitement vidéo : 100 à 500 Mo supplémentaires selon la résolution et la complexité
* Traitement vidéo 4K : 1 Go ou plus recommandé

### Utilisation CPU

* Capture vidéo : 10 à 30 % sur un processeur quadricœur moderne
* Effets en temps réel : 10 à 40 % supplémentaires selon la complexité
* Accélération matérielle recommandée pour les environnements de production

### Besoins en stockage

* Installation du SDK : ~250 Mo
* Cache d'exécution : ~100 Mo
* Fichiers temporaires de traitement : jusqu'à plusieurs Go selon la charge de travail

## Licences et déploiement

Consultez notre page [Licences](../../licensing.md) pour plus d'informations sur les différentes options de licence disponibles pour nos SDK.

## Ressources de support technique

Nous fournissons des ressources étendues pour garantir une implémentation réussie :

* Documentation API avec exemples de code
* Guides d'implémentation pour diverses plateformes
* Conseils de dépannage et d'optimisation
* Canaux de support directs pour les développeurs sous licence

## Exemples de code

Visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour une vaste collection d'exemples de code illustrant les fonctionnalités du SDK et les schémas d'implémentation sur les plateformes prises en charge.

## Mises à jour et maintenance

* Mises à jour régulières du SDK avec de nouvelles fonctionnalités et optimisations
* Correctifs de sécurité et corrections de bugs
* Considérations de compatibilité ascendante
* Guides de migration pour les transitions entre versions

---
Ce document de spécification technique décrit la configuration système et la matrice de compatibilité pour notre Video Capture SDK .Net et les produits associés. Pour des détails d'implémentation spécifiques ou des scénarios d'intégration sur mesure, veuillez contacter notre équipe de support développeur.
