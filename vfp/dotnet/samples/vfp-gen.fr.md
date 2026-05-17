---
title: Générateur d'empreintes vidéo — outil CLI vfp_gen pour .NET
description: Générez des empreintes vidéo en ligne de commande avec l'outil VisioForge vfp_gen pour .NET — modes signature search et compare.
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

# vfp_gen — générateur d'empreintes vidéo

📦 **Code source** : [Voir sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_gen)

## Vue d'ensemble

`vfp_gen` est un outil en ligne de commande qui génère des empreintes vidéo (signatures) à partir de fichiers vidéo. Ces empreintes peuvent être utilisées pour la comparaison vidéo, la détection de doublons ou la recherche de fragments.

## Fonctionnalités

- Générer des empreintes optimisées soit pour la comparaison, soit pour la recherche
- Traiter des vidéos entières ou des durées spécifiques
- Prise en charge de tous les principaux formats vidéo (MP4, AVI, MKV, MOV, etc.)
- Compatibilité multiplateforme (Windows x64)

## Utilisation

```bash
vfp_gen -i "input_video.mp4" -o "output.vsigx" [options]
```

### Paramètres requis

- `-i, --input` : chemin du fichier vidéo d'entrée
- `-o, --output` : chemin où le fichier d'empreinte sera enregistré (typiquement avec l'extension .vsigx)

### Paramètres optionnels

- `-t, --type` : type d'empreinte (par défaut : « search »)
  - `search` : optimisé pour trouver cette vidéo comme fragment dans d'autres vidéos
  - `compare` : optimisé pour comparer des vidéos entières
- `-d, --duration` : durée à analyser en millisecondes (par défaut : 0 = fichier complet)
- `-l, --license` : clé de licence VisioForge (par défaut : « TRIAL »)

## Exemples

### Générer une empreinte de recherche pour la vidéo entière
```bash
vfp_gen -i "commercial.mp4" -o "commercial.vsigx"
```

### Générer une empreinte de comparaison
```bash
vfp_gen -i "movie.mp4" -o "movie_compare.vsigx" -t compare
```

### Générer l'empreinte pour les 30 premières secondes uniquement
```bash
vfp_gen -i "video.mp4" -o "video_30s.vsigx" -d 30000
```

### Utiliser avec une clé de licence
```bash
vfp_gen -i "video.mp4" -o "output.vsigx" -l "YOUR-LICENSE-KEY"
```

## Sortie

L'outil génère un fichier d'empreinte binaire (`.vsigx`) contenant :
- Données d'empreinte
- Métadonnées vidéo (durée, dimensions, fréquence d'images)
- Référence du nom de fichier source
- Identifiant unique

## Cas d'usage

1. **Identification de contenu** : générer des empreintes pour une bibliothèque vidéo afin d'identifier les doublons
2. **Détection de publicités** : créer des empreintes de publicités pour les trouver dans les diffusions
3. **Détection de scènes** : générer des empreintes de scènes spécifiques pour les localiser dans des films complets
4. **Protection des droits d'auteur** : créer des empreintes de contenu protégé pour surveillance

## Notes de performance

- La génération d'empreintes est intensive en CPU
- Le temps de traitement dépend de la durée et de la résolution de la vidéo
- Les fichiers d'empreinte générés sont généralement petits (de quelques Ko à quelques Mo)
- L'outil affiche le pourcentage de progression pendant le traitement

## Gestion des erreurs

L'outil quittera avec un message d'erreur si :
- Le fichier d'entrée n'existe pas
- Le fichier de sortie ne peut pas être créé/écrasé
- Le format vidéo n'est pas pris en charge
- La mémoire est insuffisante pour le traitement

## Outils associés

- `vfp_compare` : comparer deux fichiers d'empreinte
- `vfp_search` : rechercher une empreinte dans une autre empreinte
