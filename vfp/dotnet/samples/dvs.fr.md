---
title: Application d'exemple Duplicate Video Scanner pour .NET
description: Analysez les dossiers à la recherche de vidéos en doublon avec l'exemple DVS de VisioForge — hachage perceptuel, traitement par lots, multi-format.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - C#

---

# DVS — Duplicate Video Scanner

📦 **Code source** :

- [DVS Windows Forms sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/DVS)
- [DVS MAUI (multiplateforme) sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/DVS%20MAUI)

## Vue d'ensemble

DVS (Duplicate Video Scanner) est une application de bureau Windows qui analyse les dossiers pour trouver des vidéos en doublon ou similaires. Elle utilise la technologie d'empreinte vidéo pour comparer les vidéos en fonction de leur contenu plutôt que des propriétés de fichier, ce qui la rend efficace pour trouver des doublons même lorsque les vidéos présentent des formats, des résolutions ou des débits binaires différents.

## Fonctionnalités

- **Traitement par lots** : analyse simultanée de plusieurs dossiers
- **Comparaison intelligente** : trouve les doublons même avec des encodages différents
- **Prise en charge des formats** : fonctionne avec AVI, WMV, MP4, MPG, MOV, TS, FLV, MKV et plus
- **Aperçu visuel** : lecteur multimédia intégré pour examiner les doublons détectés
- **Paramètres flexibles** : seuils de similarité et options d'analyse configurables
- **Suivi de progression** : barres de progression et mises à jour de statut en temps réel
- **Export des résultats** : enregistrement des résultats d'analyse pour examen ultérieur

## Interface utilisateur

### Composants de la fenêtre principale

1. **Panneau des dossiers source** : ajouter/supprimer des dossiers à analyser
2. **Panneau des paramètres** : configurer les paramètres d'analyse
3. **Panneau des résultats** : afficher et gérer les doublons trouvés
4. **Lecteur multimédia** : prévisualiser les vidéos avant action
5. **Barre d'état** : surveiller le statut de l'opération en cours

## Comment l'utiliser

### Flux de travail de base

1. **Ajouter des dossiers** :
   - Cliquez sur le bouton « Add » pour sélectionner des dossiers contenant des vidéos
   - Ajoutez plusieurs dossiers pour une analyse complète
   - Utilisez « Remove » pour supprimer des dossiers de la liste

2. **Configurer les paramètres** :
   - Sélectionnez les formats vidéo à inclure dans l'analyse
   - Définissez la sensibilité de comparaison (1 à 100 %)
   - Choisissez les options de traitement

3. **Démarrer l'analyse** :
   - Cliquez sur « Process » pour commencer l'analyse
   - DVS génère les empreintes pour toutes les vidéos
   - Les vidéos sont comparées par paires pour détecter les doublons

4. **Examiner les résultats** :
   - Les groupes de doublons apparaissent dans le panneau des résultats
   - Cliquez sur les vidéos pour les prévisualiser
   - Utilisez le menu contextuel pour les opérations sur les fichiers

### Options de paramètres

- **Formats pris en charge** : cocher/décocher les formats vidéo à inclure
- **Taille minimale de fichier** : ignorer les petits fichiers vidéo
- **Inclure les sous-dossiers** : analyser récursivement les sous-répertoires
- **Seuil de comparaison** : ajuster la sensibilité (plus bas = correspondance plus stricte)

## Comprendre les résultats

### Scores de similarité

- **95-100 %** : presque identiques (même vidéo, encodage différent)
- **85-95 %** : très similaires (modifications mineures, logos ajoutés)
- **70-85 %** : contenu similaire (modifications significatives)
- **Inférieur à 70 %** : vidéos différentes

### Regroupement des résultats

DVS regroupe les doublons par similarité :

- Chaque groupe contient des vidéos qui correspondent entre elles
- La première vidéo d'un groupe est la « référence »
- Les détails du fichier affichent la taille, la durée et le chemin

## Cas d'usage

1. **Nettoyage du stockage** : trouvez et supprimez les vidéos en doublon pour libérer de l'espace
2. **Organisation des médias** : identifiez plusieurs copies dans différents dossiers
3. **Gestion de la qualité** : conservez la version de meilleure qualité des doublons
4. **Maintenance d'archive** : assurez qu'aucun doublon ne se trouve dans les sauvegardes
5. **Vérification de contenu** : vérifiez si les vidéos sont vraiment différentes

## Fonctionnalités avancées

### Mise en cache des empreintes

- DVS peut enregistrer les empreintes pour des analyses ultérieures plus rapides
- Activer l'option « Save signatures »
- Les empreintes en cache sont stockées avec les vidéos

### Opérations par lots

- Sélectionnez plusieurs vidéos pour des actions en masse
- Supprimez les doublons tout en conservant une copie
- Déplacez les doublons vers un dossier séparé
- Exportez les listes de fichiers pour un traitement externe

### Zones d'exclusion personnalisées

- Définissez des régions à ignorer (logos, horodatages)
- Utile pour les enregistrements de diffusion
- Améliore la précision pour le contenu avec filigrane

## Conseils de performance

1. **Analyse initiale** : la première analyse est la plus lente (génère toutes les empreintes)
2. **Analyses ultérieures** : beaucoup plus rapides avec les empreintes en cache
3. **Grandes bibliothèques** : traiter par lots pour une meilleure utilisation mémoire
4. **Lecteurs réseau** : copier sur un lecteur local pour un traitement plus rapide

## Dépannage

### Problèmes courants

1. **Aucun doublon trouvé** :
   - Vérifier le réglage du seuil (essayer de l'augmenter)
   - Vérifier que les formats vidéo sont sélectionnés
   - Vérifier que les dossiers contiennent des fichiers vidéo

2. **Trop de faux positifs** :
   - Diminuer le seuil de comparaison
   - Vérifier si les vidéos ont des intros/outros communes
   - Utiliser les zones d'exclusion pour les logos

3. **Performance lente** :
   - Traiter moins de fichiers à la fois
   - Vérifier l'espace disque disponible
   - Fermer les autres applications

## Configuration système requise

- Windows 7 ou ultérieur (64 bits)
- .NET Framework 8.0 ou ultérieur
- 4 Go de RAM minimum (8 Go recommandés)
- Stockage suffisant pour le cache d'empreintes

## Gestion des fichiers

### Suppression sûre des doublons

1. Toujours prévisualiser avant de supprimer
2. Conserver la version de meilleure qualité
3. Envisager de conserver différents formats
4. Utiliser « Move to folder » plutôt que la suppression

### Organisation des résultats

- Trier par taille de fichier pour trouver les économies d'espace
- Trier par similarité pour examiner les correspondances les plus proches
- Grouper par dossier pour voir la distribution

## Bonnes pratiques

1. **Tester d'abord** : exécuter sur un petit dossier pour vérifier les paramètres
2. **Sauvegarder les fichiers importants** : avant les suppressions massives
3. **Examiner attentivement** : certains « doublons » peuvent être intentionnels
4. **Utiliser des seuils appropriés** : ajuster selon le type de contenu
5. **Analyses régulières** : les analyses périodiques empêchent l'accumulation de doublons

## Outils associés

- `vfp_compare` : outil en ligne de commande pour comparer deux vidéos
- `Image Comparer` : outil similaire pour trouver les images en doublon
- `MMT` : outil de surveillance média pour l'analyse de diffusion
