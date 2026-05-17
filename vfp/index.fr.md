---
title: Video Fingerprinting SDK — détection et correspondance
description: Détectez les contenus piratés, trouvez les doublons et faites correspondre les vidéos transformées avec le VisioForge Video Fingerprinting SDK pour .NET et C++.
sidebar_label: Video Fingerprinting SDK
order: 17
tags:
  - Video Fingerprinting SDK
  - .NET
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting

---

# Video Fingerprinting SDK

## Qu'est-ce que l'empreinte vidéo ?

Notre technologie d'empreinte vidéo de pointe crée des signatures numériques uniques du contenu vidéo en analysant plusieurs dimensions des données visuelles. Le système emploie des algorithmes sophistiqués qui se concentrent sur :

- **Analyse de scènes** — détection des transitions, coupes et de la composition
- **Reconnaissance d'objets** — identification et suivi des éléments visuels clés
- **Détection de mouvement** — analyse des motifs de mouvement et des trajectoires
- **Distribution des couleurs** — cartographie des palettes visuelles et des variations tonales
- **Motifs temporels** — examen de la façon dont les éléments visuels évoluent dans le temps

Ces éléments se combinent pour former une empreinte distinctive qui identifie de manière unique chaque vidéo de votre base de données.

## Capacités et bénéfices clés

Le SDK peut faire correspondre des vidéos avec précision malgré des transformations importantes, parmi lesquelles :

- Changements de résolution (du SD au 4K et au-delà)
- Variations de débit binaire et de qualité d'encodage
- Différentes techniques de compression
- Conversion entre formats de fichier (MP4, AVI, MOV, etc.)
- Correspondance partielle de contenu (identification de segments)
- Vidéos incrustées dans d'autres contenus
- Présence de superpositions, de filigranes ou de sous-titres

Cette robustesse rend la technologie idéale pour la vérification de contenu, la protection des droits d'auteur et les applications de surveillance des médias.

## Prise en charge des plateformes et intégration

Le SDK offre une compatibilité multiplateforme avec :

- **Windows** — prise en charge complète de Windows 10/11 et des environnements serveur
- **Linux** — compatible avec les principales distributions
- **macOS** — prise en charge complète des versions récentes

Les développeurs peuvent intégrer le SDK à l'aide de plusieurs langages de programmation :

- [C# et .NET](#net-sdk-documentation) — code managé avec des fonctionnalités riches
- [C++](#c-sdk-documentation) — performance et contrôle natifs
- VB.NET — compatibilité .NET complète
- Delphi — via interopérabilité COM
- Autres langages via bindings

Pour en savoir plus sur le SDK, consultez la [page produit](https://www.visioforge.com/video-fingerprinting-sdk).

## Applications d'exemple

Nous fournissons deux puissantes applications d'exemple construites avec notre SDK :

### Media Monitoring Tool

Une application Windows conçue pour détecter les publicités et les segments de contenu spécifiques dans les flux vidéo enregistrés ou en direct. Idéal pour :

- Surveillance de chaînes TV et DVB
- Suivi des publicités
- Vérification de la conformité de diffusion
- Analyse de contenu pour les sociétés de médias

### Duplicates Video Finder

Un outil Windows spécialisé pour identifier les contenus vidéo en doublon dans de grandes collections. L'application peut détecter les correspondances même lorsque les vidéos présentent :

- Différentes résolutions et rapports d'aspect
- Débits binaires et niveaux de qualité variés
- Différents formats de fichier et codecs
- Filigranes ou sous-titres ajoutés
- Modifications mineures ou rognages

## Choisissez votre SDK

### Documentation du SDK .NET { #net-sdk-documentation }

Le SDK .NET fournit une solution en code managé avec des fonctionnalités riches et un développement rapide :

- [Prise en main de .NET](dotnet/getting-started.md) — installation et configuration complètes
- [Référence de l'API .NET](dotnet/api.md) — documentation complète de l'API managée
- [Intégration de base de données](dotnet/database-integration.md) — prise en charge MongoDB intégrée
- [Applications d'exemple](dotnet/samples/index.md) — outils GUI et CLI

### Documentation du SDK C++ { #c-sdk-documentation }

Le SDK C++ offre une performance native et un contrôle fin :

- [Prise en main de C++](cpp/getting-started.md) — guides de configuration spécifiques par plateforme
- [Référence de l'API C++](cpp/api.md) — documentation de l'API native
- [Présentation du SDK C++](cpp/index.md) — fonctionnalités et capacités

### Concepts fondamentaux (les deux SDK)

- [Configuration système requise](system-requirements.md) — exigences de plateforme et de matériel pour les deux SDK
- [Comprendre l'empreinte vidéo](understanding-video-fingerprinting.md) — comment la technologie fonctionne
- [Types d'empreinte expliqués](fingerprint-types.md) — empreintes de comparaison vs de recherche (s'applique à .NET et C++)

## Comparaison des SDK { #sdk-comparison }

### Tableau de comparaison rapide

| Fonctionnalité | SDK .NET | SDK C++ |
|---------|----------|---------|
| **Performance** | Excellente performance managée | Performance native maximale |
| **Vitesse de développement** | Développement rapide, API simple | Plus complexe, contrôle total |
| **Gestion de la mémoire** | Automatique (GC) | Manuelle (RAII) |
| **Prise en charge GUI** | WPF, WinForms, MAUI | Qt, MFC, wxWidgets |
| **Intégration de base de données** | MongoDB intégré | Implémentation personnalisée |
| **Applications d'exemple** | GUI et CLI étendues | Centré sur la ligne de commande |
| **Courbe d'apprentissage** | Plus facile pour les développeurs .NET | Plus raide, plus de contrôle |
| **Déploiement** | Runtime .NET requis | Binaires autonomes |

### Choisir le bon SDK

**Choisissez le SDK .NET si vous :**

- Avez besoin d'un développement applicatif rapide
- Voulez une intégration de base de données intégrée
- Préférez la gestion automatique de la mémoire
- Construisez des applications GUI
- Avez une infrastructure .NET existante

**Choisissez le SDK C++ si vous :**

- Exigez une performance maximale
- Avez besoin d'un contrôle fin de la mémoire
- Intégrez avec du code natif
- Déployez sur des systèmes embarqués
- Voulez des dépendances minimales

## Tutoriels et guides

### Tutoriels pas à pas

- [Comment comparer deux fichiers vidéo](dotnet/samples/how-to-compare-two-video-files.md) — guide de comparaison vidéo (.NET)
- [Comment trouver un fragment vidéo dans un autre](dotnet/samples/how-to-search-one-video-fragment-in-another.md) — guide de recherche de fragments (.NET)

### Guides d'intégration

- [Intégration de base de données .NET](dotnet/database-integration.md) — MongoDB avec le SDK .NET
- [Exemples en ligne de commande .NET](dotnet/samples/index.md) — utilitaires CLI et exemples
- [Exemples en ligne de commande C++](cpp/samples/index.md) — exemples CLI natifs
- [Modèles d'intégration C++](cpp/index.md#integration-patterns) — exemples d'intégration native

## Cas d'usage et applications

- [Cas d'usage réels](use-cases.md) — applications et scénarios sectoriels

## Applications d'exemple

### Applications Windows .NET

- [Media Monitoring Tool (MMT)](dotnet/samples/mmt.md) — surveillance TV et flux
- [MMT Live Edition](dotnet/samples/mmt-live.md) — analyse de flux en temps réel
- [Duplicate Video Scanner (DVS)](dotnet/samples/dvs.md) — trouver les vidéos en doublon

### Outils en ligne de commande

- [Outils CLI .NET](dotnet/samples/index.md) — VFP Generator, Compare, Search
- [Exemples C++](cpp/samples/index.md) — utilitaires natifs en ligne de commande

### Exemples de code

- [Exemples de code .NET](dotnet/samples/index.md) — exemples .NET complets
- [Exemples de code C++](cpp/samples/index.md) — exemples natifs C++

## Aide et support

### Ressources essentielles

- **[FAQ](faq.md)** — questions fréquentes avec réponses détaillées

### Documentation de référence

- [Référence complète de l'API .NET](https://api.visioforge.org/vfpnet/)
- [Journal des modifications du SDK](changelog.md)

## Ressources supplémentaires

- [Référence complète de l'API .NET](https://api.visioforge.org/vfpnet/)
- [Journal des modifications du SDK](changelog.md)
- [Contrat de licence utilisateur final](../eula.md)
- [Informations produit](https://www.visioforge.com/video-fingerprinting-sdk)
