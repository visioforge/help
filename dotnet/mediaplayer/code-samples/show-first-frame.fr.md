---
title: Afficher la première image d'une vidéo en C# .NET VisioForge
description: Affichez la première image vidéo dans WinForms, WPF et Console avec C# en utilisant VisioForge Media Player SDK .NET pour des aperçus miniatures.
tags:
  - Media Player SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - Capture
  - Playback
  - Streaming
  - C#

---

# Afficher la première image de fichiers vidéo dans des applications .NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Vue d'ensemble

Lors du développement d'applications multimédias, il est souvent nécessaire de prévisualiser le contenu vidéo sans lire le fichier entier. Cette technique est particulièrement utile pour créer des galeries de miniatures, des écrans de sélection vidéo ou pour offrir aux utilisateurs un aperçu visuel avant de regarder une vidéo.

## Guide d'implémentation

Le Media Player SDK .NET fournit un moyen simple mais puissant d'afficher la première image de n'importe quel fichier vidéo. Cela est obtenu grâce à la propriété `Play_PauseAtFirstFrame` qui, lorsqu'elle est définie à `true`, demande au lecteur de se mettre en pause immédiatement après le chargement de la première image.

### Comment cela fonctionne

Lorsque la propriété `Play_PauseAtFirstFrame` est activée :

1. Le lecteur charge le fichier vidéo
2. Effectue le rendu de la première image sur la surface d'affichage vidéo
3. Met automatiquement la lecture en pause
4. Conserve la première image à l'écran jusqu'à une action ultérieure de l'utilisateur

Si cette propriété n'est pas activée (définie à `false`), le lecteur poursuit la lecture normale après le chargement.

## Implémentation du code

### Exemple de base

```cs
// créer le lecteur et configurer le nom du fichier
// ...

// définir la propriété à true
MediaPlayer1.Play_PauseAtFirstFrame = true;

// lire le fichier
await MediaPlayer1.PlayAsync();
```

Reprendre la lecture depuis la première image :

```cs
// reprendre la lecture
await MediaPlayer1.ResumeAsync();
```

## Applications pratiques

Cette fonctionnalité est particulièrement utile pour :

- Fournir des capacités d'aperçu dans les applications d'édition vidéo
- Générer des images de couverture vidéo pour les applications de streaming
- Implémenter des fonctionnalités « survoler pour prévisualiser » dans les navigateurs multimédias

## Composants requis

Pour implémenter cette fonctionnalité dans votre application, vous aurez besoin de :

- Paquet redistribuable de base
- Paquet redistribuable du SDK

Pour plus d'informations sur la distribution de ces composants avec votre application, consultez : [Comment les redistribuables requis peuvent-ils être installés ou déployés sur le PC de l'utilisateur ?](../deployment.md)

## Ressources supplémentaires

Trouvez d'autres exemples de code et exemples d'implémentation dans notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

## Considérations techniques

Lors de l'implémentation de cette fonctionnalité, gardez à l'esprit :

- L'affichage de la première image est quasi instantané pour la plupart des formats vidéo
- L'utilisation des ressources est minimale car le lecteur ne met pas en tampon au-delà de la première image
- Fonctionne avec tous les formats vidéo pris en charge, notamment MP4, MOV, AVI, etc.
