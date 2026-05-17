---
title: Rechercher des fragments vidéo par empreinte en .NET
description: Recherchez des fragments vidéo dans des fichiers plus grands avec l'outil CLI VisioForge vfp_search — sortie d'horodatages et seuils configurables.
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

# vfp_search — outil de recherche de fragments vidéo

📦 **Code source** : [Voir sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/Console/vfp_search)

## Vue d'ensemble

`vfp_search` est un outil en ligne de commande qui recherche un fragment vidéo (comme une publicité, une intro ou une scène spécifique) dans une vidéo plus grande. Il utilise des empreintes pré-générées pour localiser rapidement où le fragment apparaît dans la vidéo principale.

## Fonctionnalités

- Trouver des fragments vidéo dans des vidéos longues
- Détecter des publicités dans des enregistrements de diffusion
- Localiser des scènes ou clips spécifiques
- Recherche rapide sans retraitement des vidéos
- Retourne les horodatages exacts des correspondances

## Utilisation

```bash
vfp_search -f "fragment.vsigx" -m "main_video.vsigx" [options]
```

### Paramètres requis

- `-f, --fragment` : chemin du fichier d'empreinte du fragment (le segment vidéo à rechercher)
- `-m, --main` : chemin du fichier d'empreinte de la vidéo principale (où chercher)

### Paramètres optionnels

- `-d, --md` : différence maximale acceptable (par défaut : 500)
- `-l, --license` : clé de licence VisioForge (par défaut : « TRIAL »)

## Exemples

### Rechercher une publicité dans un enregistrement TV
```bash
vfp_search -f "commercial.vsigx" -m "tv_recording.vsigx"
```

### Recherche avec correspondance plus stricte
```bash
vfp_search -f "intro.vsigx" -m "movie.vsigx" -d 50
```

### Utilisation d'une clé de licence
```bash
vfp_search -f "scene.vsigx" -m "full_movie.vsigx" -l "YOUR-LICENSE-KEY"
```

## Sortie

L'outil affiche :
- Nombre de correspondances trouvées
- Horodatage pour chaque correspondance (format : HH:MM:SS)
- Score de différence pour chaque correspondance
- Temps de traitement total

Exemple de sortie :
```
Starting analyze.
Analyze finished. Elapsed time: 0:00:01.234
Search results: 3
00:05:32
01:23:45
02:15:18
```

## Exemple de flux de travail

1. Générer l'empreinte du fragment (par exemple, publicité de 30 secondes) :
```bash
vfp_gen -i "commercial.mp4" -o "commercial.vsigx" -t search
```

2. Générer l'empreinte de la vidéo complète :
```bash
vfp_gen -i "broadcast.mp4" -o "broadcast.vsigx" -t compare
```

3. Rechercher la publicité dans la diffusion :
```bash
vfp_search -f "commercial.vsigx" -m "broadcast.vsigx"
```

## Cas d'usage

1. **Détection de publicités** : trouver et sauter les publicités dans les enregistrements TV
2. **Surveillance de contenu** : détecter l'apparition de contenu spécifique dans les diffusions
3. **Localisation de scènes** : trouver des scènes spécifiques dans plusieurs fichiers vidéo
4. **Détection d'intro/outro** : localiser les segments récurrents dans des séries
5. **Surveillance des droits d'auteur** : trouver l'utilisation non autorisée de clips vidéo

## Bonnes pratiques

- Utilisez des empreintes de type « search » pour les fragments (`-t search` dans vfp_gen)
- Utilisez des empreintes de type « compare » pour les vidéos principales (`-t compare` dans vfp_gen)
- Les fragments doivent durer au moins 5 à 10 secondes pour une détection fiable
- Seuils de différence plus bas (< 100) pour des correspondances exactes
- Seuils plus élevés (100 à 500) pour du contenu similaire avec modifications

## Notes de performance

- La vitesse de recherche dépend de la longueur de la vidéo principale
- L'utilisation mémoire est proportionnelle à la taille des empreintes
- Traite généralement des heures de vidéo en quelques secondes

## Gestion des erreurs

L'outil quittera avec une erreur si :
- L'un des fichiers d'empreinte n'existe pas
- Le fragment est plus long que la vidéo principale
- Les fichiers d'empreinte sont corrompus
- Les types d'empreinte sont incompatibles

## Limitations

- Le fragment doit être continu (pas de coupes ni de modifications)
- Les fragments très courts (< 5 secondes) peuvent produire des faux positifs
- Le contenu fortement modifié peut ne pas être détecté

## Outils associés

- `vfp_gen` : générer des empreintes à partir de fichiers vidéo
- `vfp_compare` : comparer deux vidéos complètes
- `MMT` : outil graphique de surveillance média avec recherche de fragments
