---
title: Filtres de traitement vidéo temps réel pour DirectShow
description: Plus de 35 effets vidéo temps réel, mélangeur multi-source (2-16 entrées), chroma key, désentrelacement et débruitage. Filtres COM VisioForge.
sidebar_label: Pack de filtres de traitement
order: 7
tags:
  - DirectShow
  - C++
  - Windows

---

# Filtres de traitement DirectShow pour applications multimédias

## Introduction aux filtres de traitement DirectShow

Le pack de filtres de traitement DirectShow propose une puissante collection de filtres spécialisés conçus pour la manipulation audio et vidéo avancée dans les applications Windows. Ces filtres permettent aux développeurs d'implémenter des capacités de traitement multimédia de niveau professionnel sans avoir à développer des algorithmes complexes à partir de zéro.

Conçue pour les développeurs souhaitant enrichir leurs applications avec des fonctionnalités multimédias avancées, cette boîte à outils offre une approche rationalisée pour mettre en œuvre des fonctionnalités audio-visuelles robustes avec un minimum de code.

---

## Installation

Avant d'utiliser les exemples de code et d'intégrer les filtres dans votre application, vous devez d'abord installer le pack de filtres de traitement DirectShow depuis la [page produit](https://www.visioforge.com/processing-filters-pack).

**Étapes d'installation** :

1. Téléchargez l'installeur du pack de filtres de traitement depuis la page produit
2. Exécutez l'installeur avec des privilèges administrateur
3. L'installeur enregistrera tous les filtres de traitement
4. Les applications d'exemple et le code source seront disponibles dans le répertoire d'installation

**Remarque** : tous les filtres doivent être correctement enregistrés sur le système avant de pouvoir être utilisés dans vos applications. L'installeur s'en charge automatiquement.

---

## Capacités et avantages clés

### Capacités de traitement vidéo

#### Effets visuels avancés

- **Traitement d'effets dynamiques** : appliquez des effets en temps réel aux flux vidéo, dont le flou, le renforcement, le sépia, le niveau de gris et de nombreux filtres artistiques
- **Chaînage d'effets personnalisés** : combinez plusieurs effets séquentiellement pour des transformations visuelles complexes
- **Paramètres ajustables** : ajustez finement l'intensité et les caractéristiques des effets pour un contrôle précis

#### Mélange vidéo professionnel

- **Mélange multi-source** : combinez de façon transparente plusieurs flux vidéo en une sortie unifiée
- **Effets de transition** : implémentez des transitions fluides entre sources vidéo
- **Image dans l'image** : créez des configurations de superposition avec positionnement et mise à l'échelle personnalisables

#### Système de superposition d'images et de texte

- **Rendu de texte dynamique** : superposez du texte personnalisable avec contrôle de police et animation
- **Intégration d'images** : ajoutez des logos, filigranes et graphiques informatifs au contenu vidéo
- **Prise en charge du canal alpha** : conservez les informations de transparence pour une composition professionnelle

#### Redimensionnement de haute qualité

- **Plusieurs algorithmes** : choisissez entre les algorithmes du plus proche voisin, bilinéaire, bicubique et Lanczos
- **Contrôle du rapport d'aspect** : conservez ou ajustez le rapport d'aspect selon vos besoins
- **Optimisation de la résolution** : redimensionnez le contenu selon des exigences de sortie spécifiques tout en préservant la qualité

#### Outils de manipulation vidéo

- **Rotation et rognage** : ajustez l'orientation et le cadrage de la vidéo avec un contrôle précis
- **Options de désentrelacement** : plusieurs modes disponibles pour convertir le contenu entrelacé
- **Débruitage** : algorithmes avancés pour améliorer la netteté et la qualité de la vidéo

### Capacités de traitement audio

#### Suite d'amélioration audio

- **Traitement d'effets** : appliquez divers effets audio pour l'amélioration sonore et la manipulation créative
- **Gestion des canaux** : contrôlez l'image stéréo et les configurations multicanaux

#### Contrôles audio avancés

- **Optimisation du volume** : ajustement précis du volume avec options de normalisation
- **Réglage de la balance** : ajustez finement la balance gauche/droite pour une distribution sonore optimale
- **Modification de hauteur tonale** : modifiez la hauteur tout en conservant ou en changeant le tempo
- **Mise en œuvre de delay** : ajoutez des effets de retard personnalisables avec contrôle de réinjection

#### Effets sonores professionnels

- **Génération d'écho** : créez des effets d'écho spatial aux paramètres ajustables
- **Système d'égaliseur** : égalisation multibande pour le réglage des fréquences
- **Effets chorus** : ajoutez de la richesse et de la profondeur aux flux audio
- **Traitement flanger** : créez des effets audio psychédéliques et balayés

## Configuration système requise

### Systèmes d'exploitation compatibles

- Windows 11, 10, 8.1, 8 et 7 (versions 32 bits et 64 bits)

### Prise en charge de l'environnement de développement

- **Microsoft Visual Studio** : versions 2022, 2019, 2017, 2015, 2013, 2012 et 2010
- **Outils Embarcadero** : compatible avec Delphi et C++ Builder
- **Environnements supplémentaires** : fonctionne avec toute plateforme de développement prenant en charge les filtres DirectShow

### Prérequis techniques

- Installation de DirectX 9 ou ultérieur
- 4 Go de RAM minimum (8 Go ou plus recommandés pour le traitement haute résolution)
- Processeur multicœur recommandé pour des performances optimales

## Ressources supplémentaires

- [Informations complètes sur le produit](https://www.visioforge.com/processing-filters-pack)
- [Documentation de l'API](https://api.visioforge.org/proc_filters/api/index.html)
- [Informations de licence](../../eula.md)

## Historique des versions et mises à jour

### Améliorations de la version 15.1

- Intégration à l'architecture des SDK .Net 15.1
- Améliorations significatives des moteurs de mélange audio et vidéo
- Prise en charge renforcée du multithreading pour de meilleures performances sur les systèmes multicœurs
- Bibliothèque d'effets vidéo enrichie avec de nouvelles options de traitement
- Résolution des artefacts de clics audio dans le composant mélangeur
- Prise en charge optimisée du traitement de contenu ultra haute définition 4K et 8K

### Améliorations de la version 15.0

- Alignement complet avec le framework des SDK .Net 15.0
- Traitement haute résolution optimisé pour les filtres de luminosité, contraste, saturation et teinte

### Mises à jour de la version 14.0

- Compatibilité complète avec les SDK .Net 14.0
- Optimisation des performances des opérations de redimensionnement vidéo
- Algorithme bicubique de redimensionnement vidéo amélioré pour une qualité supérieure

### Améliorations de la version 12.0

- Intégration à l'infrastructure des SDK .Net 12.0
- Mélangeur audio redessiné avec performances améliorées
- Correction des problèmes de stabilité lors de l'utilisation de rognage ou redimensionnement avec des paramètres incorrects

### Fonctionnalités de la version 11.0

- Mise à jour pour correspondre aux spécifications des SDK .Net 11.0
- Algorithmes améliorés de manipulation de tempo et de hauteur tonale audio
- Performances optimisées de la balance vidéo pour un traitement plus fluide

### Développements de la version 10.0

- Alignement avec l'architecture des SDK .Net 10.0
- Composant Video Mixer entièrement repensé

### Avancées de la version 9.0

- Intégration au framework des SDK .Net 9.2
- Bibliothèque d'effets vidéo enrichie
- Optimisations spécifiques au traitement de contenu 4K

### Version 8.5 — version initiale

- Première version publique, intégrant les filtres des SDK .Net 8.5
- Introduction de la prise en charge Lanczos dans le filtre de redimensionnement vidéo pour une mise à l'échelle de qualité supérieure
