---
title: Comparaison d'empreintes vidéo — détection de doublons
description: Comparez les empreintes vidéo pour la similarité avec l'outil CLI VisioForge vfp_compare — seuils configurables, détection de doublons.
tags:
  - Video Fingerprinting SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Fingerprinting
  - MP4
  - C#

---

# vfp_compare — outil de comparaison d'empreintes vidéo

📦 **Code source** : [Voir sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_compare)

## Vue d'ensemble

`vfp_compare` est un outil en ligne de commande qui compare deux fichiers d'empreinte vidéo pour déterminer si les vidéos sont similaires ou identiques. Il est utile pour détecter les vidéos en doublon, trouver du contenu similaire ou vérifier l'intégrité d'une vidéo.

## Fonctionnalités

- Comparer des empreintes vidéo pré-générées
- Comparaison rapide sans retraitement des vidéos
- Seuil de similarité configurable
- Retourne un score de différence numérique

## Utilisation

```bash
vfp_compare -f "fingerprint1.vsigx" -s "fingerprint2.vsigx" [options]
```

### Paramètres requis

- `-f, --f1` : chemin du premier fichier d'empreinte
- `-s, --f2` : chemin du second fichier d'empreinte

### Paramètres optionnels

- `-d, --md` : différence maximale acceptable (par défaut : 500)
- `-l, --license` : clé de licence VisioForge (par défaut : « TRIAL »)

## Exemples

### Comparaison de base
```bash
vfp_compare -f "video1.vsigx" -s "video2.vsigx"
```

### Comparaison avec seuil personnalisé
```bash
vfp_compare -f "original.vsigx" -s "copy.vsigx" -d 100
```

### Utilisation d'une clé de licence
```bash
vfp_compare -f "file1.vsigx" -s "file2.vsigx" -l "YOUR-LICENSE-KEY"
```

## Sortie

L'outil affiche :
- Score de différence (plus bas = plus similaire)
- Résultat de comparaison basé sur le seuil
- Temps de traitement

### Comprendre les scores de différence

- **0-5** : vidéos presque identiques (même contenu, différences mineures d'encodage)
- **5-15** : vidéos très similaires (même contenu, qualité/compression différente)
- **15-30** : vidéos similaires (même contenu avec modifications, logos ou filigranes)
- **30-100** : contenu lié avec des différences significatives
- **100-300** : vidéos différentes avec quelques scènes similaires
- **300+** : vidéos complètement différentes

## Cas d'usage

1. **Détection de doublons** : trouver des copies exactes de vidéos dans différents formats
2. **Comparaison de qualité** : comparer différents encodages de la même vidéo
3. **Détection de modifications** : identifier si une vidéo a été modifiée
4. **Vérification des droits d'auteur** : vérifier si le contenu correspond à la source originale

## Exemple de flux de travail

1. Générer les empreintes pour les vidéos :
```bash
vfp_gen -i "original.mp4" -o "original.vsigx" -t compare
vfp_gen -i "suspect.mp4" -o "suspect.vsigx" -t compare
```

2. Comparer les empreintes :
```bash
vfp_compare -f "original.vsigx" -s "suspect.vsigx"
```

## Notes de performance

- La comparaison est quasi instantanée
- L'utilisation mémoire est minimale (chargement des empreintes uniquement)
- Aucun décodage vidéo requis

## Gestion des erreurs

L'outil quittera avec une erreur si :
- L'un des fichiers d'empreinte n'existe pas
- Les fichiers d'empreinte sont corrompus
- Les empreintes ont été générées avec des paramètres incompatibles

## Bonnes pratiques

- Utilisez des empreintes de type « compare » (générées avec `-t compare`) pour de meilleurs résultats
- Conservez les empreintes avec leurs vidéos sources pour référence
- Documentez le seuil de différence utilisé pour votre cas d'usage

## Outils associés

- `vfp_gen` : générer des empreintes à partir de fichiers vidéo
- `vfp_search` : rechercher des fragments dans des vidéos
- `DVS` : outil graphique pour trouver des vidéos en doublon dans des dossiers
