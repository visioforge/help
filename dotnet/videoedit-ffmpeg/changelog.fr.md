---
title: Journal des modifications du SDK FFmpeg Video Editor .NET
description: Historique des versions et notes de publication pour VisioForge Video Edit SDK FFmpeg .NET. Nouvelles fonctionnalités, performances et changements d'API.
tags:
  - Video Edit SDK FFmpeg
  - .NET
primary_api_classes:
  - MediaInfoReader
  - FFMPEGEXEOutput

---

# Video Edit SDK FFmpeg .NET : historique complet des versions

## Nouveautés de la version 12.1

Notre dernière version apporte des améliorations significatives à la flexibilité de déploiement et à la compatibilité avec les frameworks :

### Mise à niveau de .Net Framework

* Migration complète vers .Net 4.6 garantissant de meilleures performances et une compatibilité avec les systèmes modernes
* Fiabilité d'exécution améliorée grâce à la mise à jour des composants principaux

### Modèle de distribution simplifié

* Programme d'installation unifié pour les versions TRIAL et FULL, simplifiant le processus de déploiement
* Paquets NuGet identiques entre les niveaux de licence, éliminant la confusion entre versions

### Développement multiplateforme

* Consolidation des paquets .Net Core et .Net Framework en une seule distribution unifiée
* Gestion simplifiée des dépendances entre différentes plateformes cibles

### Améliorations du déploiement

* Ajout de paquets NuGet redists pour faciliter la gestion des dépendances
* Processus de déploiement simplifié avec gestion automatique des références
* Complexité de configuration réduite pour les applications d'entreprise

## Points forts de la version 11.3

Cette version s'est concentrée sur les capacités audio principales et la prise en charge multiplateforme :

### Amélioration audio

* Effets de fondu d'entrée/sortie audio entièrement repensés pour des transitions plus douces
* Performances d'algorithme améliorées sur les processeurs multicœurs
* Stabilité améliorée du pipeline de traitement audio

### Mises à jour du framework

* Ajout de la prise en charge complète de .Net Core pour le développement multiplateforme
* Rétrocompatibilité maintenue avec les implémentations existantes de .Net Framework
* Optimisations de performances pour les deux environnements d'exécution

### Améliorations techniques

* Mise à jour du sérialiseur JSON intégré avec une meilleure gestion des objets complexes
* Gestion de la mémoire améliorée pour les tâches volumineuses de traitement multimédia
* Correction de problèmes de threading dans les environnements multiprocesseurs

## Mise à jour majeure de la version 10.0

Une mise à jour importante avec de nombreuses nouvelles fonctionnalités et améliorations architecturales :

### Gestion multimédia avancée

* Lecteur d'informations multimédias entièrement repensé avec une meilleure prise en charge des formats
* Le composant `MediaInfoNV` est renommé en `MediaInfoReader`, plus intuitif
* Capacités d'extraction de métadonnées améliorées pour une plus large gamme de formats

### Système d'étiquetage multimédia

* Ajout de la prise en charge complète des étiquettes standards pour divers formats :
  * Fichiers vidéo : MP4, WMV et autres formats de conteneur
  * Fichiers audio : MP3, AAC, M4A, Ogg Vorbis et formats audio supplémentaires
* Prise en charge de la lecture des étiquettes dans Media Player SDK
* Capacités d'écriture des étiquettes dans Video Capture SDK et Video Edit SDK

### Améliorations de synchronisation

* Implémentation de la fonctionnalité de démarrage retardé dans tous les composants du SDK
* Nouvelle propriété `Start_DelayEnabled` permettant l'initialisation quasi simultanée de plusieurs contrôles du SDK
* Synchronisation améliorée entre les pipelines de traitement audio et vidéo

### Architecture de traitement audio

* Effets audio réécrits en C# pour la compatibilité avec les applications x64
* API d'effets héritée maintenue pour la rétrocompatibilité
* Performances améliorées et latence réduite dans le traitement en temps réel

### Expérience développeur

* Ajout du suivi des erreurs dans la fenêtre Output de Visual Studio
* Surveillance des erreurs en temps réel à partir des événements OnError
* Sérialisation des paramètres basée sur JSON pour une gestion de configuration plus facile

### Ajouts de formats de sortie

* Prise en charge de la sortie GIF dans Video Edit SDK .Net et Video Capture SDK .Net
* Séparateur MP3 personnalisé répondant aux problèmes de lecture avec les fichiers MP3 problématiques

### Changements structurels

* Les assemblies `VisioForge.Controls.WinForms` et `VisioForge.Controls.WPF` sont consolidées en un assembly unifié `VisioForge.Controls.UI`
* Ajout de la propriété `ExecutableFilename` à `VFFFMPEGEXEOutput` pour la spécification d'un exécutable FFMPEG personnalisé
* Optimisation significative des effets vidéo pour les architectures Intel récentes
* Prise en charge multithreading améliorée pour une meilleure utilisation des multicœurs

## Fonctionnalités de la version 9.0

Cette version a introduit plusieurs nouvelles capacités pour améliorer la présentation multimédia :

### Améliorations visuelles

* Ajout de la prise en charge des GIF animés comme superpositions de logos d'image
* Pipeline de rendu amélioré pour des animations plus fluides
* Meilleure gestion du canal alpha pour les superpositions transparentes

### Accès aux informations du SDK

* Nouvelle propriété `SDK_Version` pour accéder par programme aux versions des assemblies
* Ajout de la propriété `SDK_State` pour vérifier les informations d'enregistrement et de licence
* Capacités de diagnostic améliorées pour le dépannage

### Améliorations de licence

* Implémentation d'un système d'événements de licence dédié pour vérifier l'édition requise du SDK
* Messages d'erreur plus clairs pour les problèmes de licence
* Processus de validation de licence amélioré

## Mise à jour de la version 8.6

Une version de maintenance axée sur la stabilité :

### Améliorations de stabilité

* Correction de fuites de mémoire dans les opérations de traitement de longue durée
* Résolution des problèmes de threading avec les opérations multimédias concurrentes
* Gestion des exceptions améliorée dans les composants principaux

## Version 8.5

Cette mise à jour a apporté des améliorations au moteur principal :

### Mises à jour FFMPEG

* Mise à jour des composants principaux FFMPEG vers la dernière version stable
* Prise en charge des codecs étendue pour les formats multimédias récents
* Améliorations de performances dans les opérations de transcodage

### Corrections de bogues

* Résolution des problèmes de synchronisation audio/vidéo dans des formats spécifiques
* Correction de problèmes de compatibilité avec les formats de conteneur
* Stabilité améliorée lors des opérations de conversion de format

## Version initiale 7.0

La version fondatrice qui a établi les fonctionnalités principales :

### Fonctionnalités clés

* Capacités d'édition vidéo haute performance
* Prise en charge complète des formats pour les flux de travail professionnels
* Conception d'API flexible pour l'intégration dans diverses applications
* Considérations de compatibilité multiplateforme
* Fondations pour le développement et l'amélioration futurs

## Compatibilité et exigences

Lors de la mise à niveau entre versions, veuillez tenir compte des points suivants :

* La version 12.1 nécessite .Net Framework 4.6 ou supérieur
* La version 11.3 et au-delà prennent en charge à la fois .Net Core et .Net Framework
* La version 10.0 a introduit des changements de rupture dans la structure des assemblies
* Les paquets NuGet offrent la voie de mise à niveau la plus simple entre versions

Notre développement continu vise à améliorer les fonctionnalités tout en préservant la compatibilité dans la mesure du possible. Les changements d'API sont documentés en détail pour faciliter la planification de la migration.

## Prise en main

Pour les développeurs novices au SDK, nous recommandons de commencer avec la dernière version pour bénéficier de toutes les améliorations et optimisations. Le programme d'installation unifié et les paquets NuGet facilitent l'intégration dans des projets nouveaux comme existants.
