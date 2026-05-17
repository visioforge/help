---
title: Exemples de code essentiels du SDK .NET pour le multimédia
description: Exemples d'implémentation pour les filtres DirectShow, le traitement audio/vidéo, le rendu et la manipulation des médias dans les applications du SDK .NET.
sidebar_label: Exemples de code

order: -4
tags:
  - .NET
  - DirectShow
  - Editing

---

# Exemples de code du SDK .NET : guide pratique d'implémentation

Dans ce guide, vous trouverez une collection d'exemples de code pratiques et de techniques d'implémentation pour travailler avec nos SDK .NET. Ces exemples abordent des scénarios de développement courants et montrent comment tirer parti efficacement de nos bibliothèques pour des applications de traitement multimédia.

## Implémentation de filtres DirectShow

DirectShow fournit un framework puissant pour gérer les flux multimédias. Nos SDK simplifient le travail avec ces composants grâce à des interfaces bien conçues et des méthodes utilitaires.

### Indexation des médias et gestion des formats

- [Indexation des fichiers ASF et WMV](asf-wmv-files-indexing.md) — Apprenez les techniques pour indexer correctement les formats Windows Media afin de permettre la recherche et le contrôle efficace de la position de lecture. Cet exemple montre comment établir des points de navigation précis dans les fichiers multimédias et gérer efficacement les contenus ASF/WMV volumineux.

### Intégration de filtres personnalisés

- [Utilisation de l'interface de filtre DirectShow personnalisé](custom-filter-interface.md) — Ce tutoriel décrit le processus d'implémentation et de connexion de filtres DirectShow personnalisés dans votre application. Vous apprendrez à créer des interfaces de filtre qui s'intègrent harmonieusement à l'architecture DirectShow existante tout en ajoutant vos propres fonctionnalités spécialisées.

### Intégration tierce

- [Intégration de filtres de traitement vidéo tiers](3rd-party-video-effects.md) — Découvrez comment incorporer des composants de traitement vidéo externes dans votre graphe de filtres DirectShow. Cet exemple montre l'enregistrement correct des filtres, les méthodes de connexion et la configuration des paramètres pour les effets et transformations vidéo tiers.

### Gestion des filtres

- [Désinstallation manuelle des filtres DirectShow](uninstall-directshow-filter.md) — Ce guide explique les entrées de registre, l'enregistrement des objets COM et les répertoires système impliqués dans la suppression complète des filtres DirectShow lorsque la désinstallation standard n'est pas suffisante ou pas disponible.

- [Exclusion de filtres DirectShow spécifiques](exclude-filters.md) — Apprenez les techniques pour contourner sélectivement certains filtres DirectShow dans la construction de votre graphe de filtres. Cet exemple montre comment exclure des décodeurs, encodeurs ou filtres de traitement spécifiques tout en maintenant une gestion correcte des médias.

## Techniques de traitement audio et vidéo

La manipulation des flux audio et vidéo est une exigence fondamentale pour de nombreuses applications multimédias. Ces exemples démontrent différentes approches pour accéder aux données multimédias et les modifier.

### Effets vidéo en temps réel

- [Effets vidéo personnalisés via les événements d'image](custom-video-effects.md) — Apprenez deux approches puissantes pour implémenter des effets vidéo en temps réel via les événements OnVideoFrameBitmap et OnVideoFrameBuffer. Cet exemple complet montre comment accéder aux images vidéo, appliquer des effets et optimiser les performances.

### Techniques avancées de superposition

- [Dessin de superposition multi-texte](draw-multitext-onvideoframebuffer.md) — Cet exemple montre des techniques pour rendre plusieurs éléments de texte sur des images vidéo avec un positionnement précis et un contrôle du style. Vous apprendrez à gérer la mise en forme du texte, le mélange alpha et l'optimisation des performances.

- [Implémentation de superposition de texte](text-onvideoframebuffer.md) — Un tutoriel ciblé sur l'ajout d'annotations textuelles dynamiques au contenu vidéo. Cet exemple couvre la sélection de la police, le positionnement et les mises à jour en temps réel du texte de superposition.

- [Intégration de superposition d'image](image-onvideoframebuffer.md) — Apprenez à composer des images sur des images vidéo avec une mise à l'échelle, un mélange alpha et un positionnement appropriés. Cet exemple présente des techniques pour le tatouage numérique, le placement de logos et les superpositions d'images dynamiques.

### Transformation vidéo

- [Implémentation manuelle de l'effet de zoom](zoom-onvideoframebuffer.md) — Cet exemple détaillé montre comment implémenter une fonctionnalité de zoom personnalisée en manipulant directement les tampons d'image vidéo. Vous apprendrez les techniques de sélection de région, les algorithmes de mise à l'échelle et les transitions fluides entre les niveaux de zoom.

### Traitement d'image basé sur Bitmap

- [Utilisation de l'événement OnVideoFrameBitmap](onvideoframebitmap-usage.md) — Ce guide explore l'approche basée sur Bitmap pour le traitement des images vidéo, qui offre un accès simplifié aux données d'image via des objets compatibles GDI+. Apprenez en quoi cette approche diffère du traitement basé sur tampon et quand choisir chacune.

## Solutions de rendu vidéo

Afficher du contenu vidéo avec flexibilité et performance nécessite de comprendre diverses techniques de rendu. Ces exemples démontrent différentes approches pour la présentation visuelle.

### Intégration Windows Forms

- [Rendu vidéo dans un PictureBox](draw-video-picturebox.md) — Cet exemple montre comment rendre correctement du contenu vidéo dans un contrôle PictureBox Windows Forms standard. Vous apprendrez le minutage des images, la préservation du rapport d'aspect et les considérations de performance.

### Fonctionnalité multi-écrans

- [Configuration de zoom sur plusieurs moteurs de rendu](zoom-video-multiple-renderer.md) — Apprenez les techniques pour contrôler indépendamment les niveaux de zoom sur plusieurs moteurs de rendu vidéo. Cet exemple est essentiel pour les applications nécessitant des sorties vidéo synchronisées mais visuellement distinctes.

- [Sortie vidéo multi-écrans en WPF](multiple-screens-wpf.md) — Cet exemple montre comment implémenter plusieurs surfaces d'affichage vidéo indépendantes au sein d'une application WPF. Vous apprendrez l'initialisation correcte des contrôles, la gestion des ressources et les techniques de synchronisation.

### Sélection et personnalisation du moteur de rendu

- [Sélection du moteur de rendu vidéo (WinForms)](select-video-renderer-winforms.md) — Ce tutoriel explique comment choisir et configurer le moteur de rendu vidéo le plus adapté à votre application Windows Forms. Vous comprendrez les compromis entre EVR, VMR9 et d'autres types de moteurs de rendu.

### Interaction utilisateur

- [Intégration des événements de molette de la souris](mouse-wheel-usage.md) — Apprenez à gérer les événements de la molette de la souris pour des affichages vidéo interactifs. Cet exemple présente le contrôle du zoom, le défilement temporel et d'autres interactions basées sur la molette.

- [Vue vidéo avec image personnalisée](video-view-set-custom-image.md) — Ce guide montre comment remplacer l'image vidéo standard par une image personnalisée pour des scénarios comme la perte de connexion, les états de mise en tampon ou les messages spécifiques à l'application.

## Informations et visualisation des médias

Ces exemples démontrent comment extraire des informations des fichiers multimédias et créer des visualisations utiles.

### Analyse de fichier

- [Extraction d'informations de fichiers multimédias](read-file-info.md) — Apprenez les techniques pour lire les métadonnées détaillées, les propriétés de flux et les informations de format des fichiers multimédias. Cet exemple montre comment accéder à la durée, au débit binaire, aux informations de codec et à d'autres propriétés multimédias essentielles.

### Visualisation audio

- [VU-mètre et visualisation de forme d'onde](vu-meters.md) — Cet exemple complet montre comment créer des visualisations audio en temps réel, y compris des VU-mètres et des affichages de forme d'onde. Vous apprendrez l'analyse du niveau audio, les techniques de dessin et la synchronisation avec la lecture.

## Optimisation des performances

Chaque exemple de cette collection est conçu avec les considérations de performance à l'esprit. Vous y trouverez des techniques pour la gestion efficace des tampons, la gestion de la mémoire et des optimisations de traitement qui vous aident à créer des applications multimédias réactives, même lorsque vous travaillez avec du contenu haute résolution ou que vous appliquez des effets complexes.

## Considérations multiplateformes

Bien que ces exemples se concentrent sur les implémentations .NET, beaucoup des concepts démontrés s'appliquent également à d'autres plateformes. Lorsque c'est pertinent, nous avons noté les considérations spécifiques à chaque plateforme et les approches alternatives pour les scénarios de développement multiplateformes.

## Prise en main

Pour utiliser ces exemples efficacement, nous vous recommandons de consulter la documentation du SDK adaptée à votre version de produit. Chaque exemple inclut les références et le code d'initialisation nécessaires, mais peut nécessiter une configuration adaptée à votre environnement de développement et à votre plateforme cible.

Ces exemples de code servent de blocs de construction pour vos applications multimédias et offrent des modèles d'implémentation éprouvés que vous pouvez adapter et étendre selon vos besoins spécifiques.
