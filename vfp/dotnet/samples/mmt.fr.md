---
title: Détection de fragments vidéo dans des diffusions avec .NET
description: Détectez publicités, intros et clips dans les enregistrements de diffusion avec Media Monitoring Tool de VisioForge — recherche par empreinte.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Editing
  - Fingerprinting
  - C#

---

# Media Monitoring Tool

📦 **Code source** :
- [MMT Windows Forms sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT)
- [MMT MAUI (multiplateforme) sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20MAUI)

## Vue d'ensemble

MMT (Media Monitoring Tool) est une application de bureau Windows conçue pour trouver des fragments vidéo dans des vidéos plus grandes. Elle est principalement utilisée pour détecter des publicités, des annonces, des intros, des outros ou tout segment vidéo spécifique dans des enregistrements de diffusion ou des collections vidéo. Contrairement à DVS qui compare des vidéos entières, MMT se spécialise dans la localisation d'apparitions de clips spécifiques dans un contenu plus long.

## Fonctionnalités clés

- **Détection de fragments** : trouver des clips vidéo courts dans des enregistrements longs
- **Traitement par lots** : rechercher plusieurs fragments dans plusieurs vidéos
- **Surveillance publicitaire** : détecter et suivre les apparitions de publicités
- **Visualisation chronologique** : voir exactement quand les fragments apparaissent
- **Aperçu média** : lecteur intégré pour examiner les détections
- **Capacités d'export** : enregistrer les résultats au format CSV
- **Prise en charge de base de données** : construire des bibliothèques de fragments pour des recherches répétées

## Interface utilisateur

### Composants principaux

1. **Lecteur multimédia** : prévisualiser les vidéos et les détections
2. **Onglet Broadcast Dump** : gérer les vidéos dans lesquelles rechercher
3. **Onglet Ads/Fragments** : gérer les fragments vidéo à rechercher
4. **Onglet Résultats** : afficher les résultats de détection et les statistiques
5. **Onglet Paramètres** : configurer les paramètres de recherche
6. **Barre d'état** : surveiller la progression du traitement

## Comment l'utiliser

### Flux de travail de base

1. **Ajouter du contenu de diffusion** :
   - Utiliser l'onglet « Broadcast dump »
   - Ajouter des fichiers ou dossiers contenant des vidéos longues
   - Ce sont les vidéos dans lesquelles vous chercherez

2. **Ajouter des fragments** :
   - Passer à l'onglet « Ads/fragments »
   - Ajouter des publicités, intros ou clips à trouver
   - Possibilité d'ajouter des fichiers individuels ou des dossiers

3. **Configurer les paramètres** :
   - Définir la sensibilité de détection
   - Choisir les options de traitement
   - Configurer les préférences de sortie

4. **Traiter** :
   - Cliquer sur « Process » pour démarrer l'analyse
   - MMT génère les empreintes pour tout le contenu
   - Recherche chaque fragment dans chaque diffusion

5. **Examiner les résultats** :
   - Voir les détections dans l'onglet Résultats
   - Consulter horodatages et scores de confiance
   - Prévisualiser les correspondances dans le lecteur

## Cas d'usage

### 1. Détection de publicités

- Surveiller les enregistrements TV pour des publicités spécifiques
- Suivre la fréquence et le placement des publicités
- Générer des rapports pour l'analyse publicitaire

### 2. Surveillance de contenu

- Trouver du contenu protégé dans les téléchargements
- Détecter l'utilisation non autorisée de clips vidéo
- Surveiller les apparitions de marque

### 3. Analyse de diffusion

- Localiser les segments de programme (intros, outros)
- Trouver des segments d'actualité dans plusieurs diffusions
- Suivre le contenu récurrent

### 4. Contrôle qualité

- Vérifier l'insertion publicitaire dans les diffusions
- Rechercher les segments manquants
- Garantir la conformité du contenu

## Paramètres et options

### Paramètres de détection

- **Sensibilité** : ajuster le seuil de correspondance (1 à 100)
  - Plus élevé = correspondance plus stricte (moins de faux positifs)
  - Plus bas = correspondance plus lâche (peut manquer des variations)

- **Décalage temporel** : déplacement temporel maximum autorisé
- **Zones d'exclusion** : définir des régions à exclure (logos, bandeaux)

### Options de traitement

- **Multithreading** : utiliser plusieurs cœurs CPU
- **Limite de mémoire** : contrôler l'utilisation de la RAM
- **Mise en cache des empreintes** : enregistrer pour un retraitement plus rapide

## Comprendre les résultats

### Informations de résultat

Chaque détection affiche :

- **Nom du fragment** : quel clip a été trouvé
- **Fichier de diffusion** : où il a été trouvé
- **Horodatage** : position exacte (HH:MM:SS)
- **Durée** : longueur de la correspondance
- **Confiance** : qualité de correspondance (pourcentage)

### Scores de confiance

- **95-100 %** : correspondance exacte
- **85-95 %** : confiance élevée (différences mineures)
- **70-85 %** : correspondance probable (quelques modifications)
- **Inférieur à 70 %** : correspondance possible (examen nécessaire)

## Fonctionnalités avancées

### Base de fragments

- Construire des bibliothèques de fragments courants
- Enregistrer les empreintes pour réutilisation
- Organiser par catégories (publicités, intros, etc.)

### Analyse par lots

- Traiter plusieurs diffusions pendant la nuit
- Mettre en file de grands ensembles de fragments
- Générer des rapports complets

### Export CSV

Les résultats peuvent être exportés avec :

- Horodatages de détection
- Chemins de fichiers
- Scores de confiance
- Détails des fragments

## Bonnes pratiques

### Préparation des fragments

1. **Longueur optimale** : 10 à 60 secondes fonctionne le mieux
2. **Coupes nettes** : éviter les images partielles au début/fin
3. **Qualité maximale** : utiliser la source de qualité la plus élevée disponible
4. **Pas de modifications** : ne pas rogner ni éditer les fragments

### Fichiers de diffusion

1. **Format cohérent** : encodage similaire préféré
2. **Fichiers complets** : éviter les enregistrements corrompus
3. **Longueur raisonnable** : diviser les enregistrements très longs

### Optimisation des performances

1. **Pré-générer les empreintes** : économiser du temps de traitement
2. **Traiter par lots** : ne pas surcharger la mémoire
3. **Utiliser le stockage SSD** : accès aux fichiers plus rapide
4. **Fermer les autres applis** : maximiser les ressources disponibles

## Dépannage

### Aucune détection trouvée

- Vérifier la qualité et la longueur du fragment
- Vérifier que la diffusion contient le fragment
- Ajuster les paramètres de sensibilité
- Assurer la plage temporelle appropriée

### Trop de faux positifs

- Augmenter le seuil de sensibilité
- Vérifier les éléments communs (images noires)
- Utiliser des fragments plus longs
- Définir des zones d'exclusion

### Problèmes de performance

- Réduire le nombre de fichiers simultanés
- Activer la mise en cache des empreintes
- Vérifier l'espace disque disponible
- Surveiller l'utilisation de la RAM

## Flux de travail typiques

### Surveillance des publicités

1. Enregistrer les flux de diffusion
2. Créer une bibliothèque de fragments de publicités
3. Exécuter MMT quotidiennement/hebdomadairement
4. Exporter des rapports pour les clients

### Vérification de contenu

1. Préparer des clips de contenu autorisé
2. Surveiller les plateformes vidéo
3. Détecter les utilisations non autorisées
4. Documenter les infractions

## Configuration système requise

- Windows 7 ou ultérieur (64 bits)
- .NET Framework 8.0
- 8 Go de RAM recommandés
- Stockage rapide pour les fichiers vidéo
- CPU multi-cœur bénéfique

## Outils associés

- `vfp_search` : recherche de fragments en ligne de commande
- `MMT Live` : version de surveillance en temps réel
- `DVS` : trouver des vidéos complètes en doublon
