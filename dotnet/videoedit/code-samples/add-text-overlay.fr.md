---
title: Superposition de texte dynamique avec style en C# .NET
description: Mettez en œuvre des superpositions de texte dynamiques avec VisioForge Video Edit SDK .NET. Contrôlez police, couleur, position, minutage et animations.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Editing
  - C#
primary_api_classes:
  - VideoEditCoreX
  - TextOverlay

---

# Mise en œuvre de superpositions de texte dans les projets vidéo

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introduction aux superpositions de texte

Les superpositions de texte sont des composants essentiels dans l'édition vidéo professionnelle. Elles vous permettent d'ajouter des titres, des sous-titres, des filigranes, des légendes et d'autres éléments textuels importants à vos vidéos. Avec le Video Edit SDK pour .NET, vous pouvez créer des superpositions de texte sophistiquées avec un contrôle précis sur l'apparence, le positionnement et le minutage.

## Fonctionnalités et capacités principales

Le SDK fournit de vastes options de personnalisation pour les superpositions de texte, notamment :

- Sélection de polices personnalisées parmi les polices installées sur le système
- Contrôle complet de la taille, du poids et du style de la police
- Options de couleur flexibles pour le texte et l'arrière-plan
- Positionnement précis avec plusieurs options d'alignement
- Contrôle du minutage pour définir l'apparition et la disparition du texte
- Paramètres de transparence et d'opacité

## Exemple d'implémentation

Le code suivant illustre comment créer et configurer une superposition de texte dans votre application .NET :

```cs
// Initialiser l'objet VideoEditCoreX (supposé déjà créé)
// var videoEdit = new VideoEditCoreX();

// Créer un nouvel objet de superposition de texte avec le texte souhaité
var textOverlay = new VisioForge.Core.Types.X.VideoEdit.TextOverlay("Hello world!");

// Définir quand le texte doit apparaître et pendant combien de temps
// Cet exemple : le texte apparaît à 2 secondes dans la vidéo et reste pendant 5 secondes
textOverlay.Start = TimeSpan.FromMilliseconds(2000);
textOverlay.Duration = TimeSpan.FromMilliseconds(5000);

// Définir la position du texte sur l'image vidéo
// Les coordonnées X et Y sont mesurées en pixels depuis le coin supérieur gauche
textOverlay.X = 50;
textOverlay.Y = 50;

// Configurer les propriétés de police du texte
textOverlay.FontFamily = "Arial";  // Définir la famille de police
textOverlay.FontSize = 40;         // Définir la taille de police en points
textOverlay.FontWidth = SkiaSharp.SKFontStyleWidth.Normal;   // Caractères de largeur normale
textOverlay.FontSlant = SkiaSharp.SKFontStyleSlant.Italic;   // Appliquer le style italique
textOverlay.FontWeight = SkiaSharp.SKFontStyleWeight.Bold;   // Appliquer le poids gras

// Définir la couleur du texte en rouge
textOverlay.Color = SkiaSharp.SKColors.Red;

// Définir un arrière-plan transparent derrière le texte
// Vous pouvez utiliser n'importe quelle couleur avec canal alpha pour une semi-transparence
textOverlay.BackgroundColor = SkiaSharp.SKColors.Transparent;

// Ajouter la superposition de texte configurée à votre projet vidéo
videoEdit.Video_TextOverlays.Add(textOverlay);
```

## Options de positionnement

Le SDK utilise les coordonnées X et Y pour le positionnement absolu. X et Y représentent des coordonnées en pixels depuis le coin supérieur gauche de l'image vidéo :

```cs
// Positionner le texte à des coordonnées spécifiques (en pixels)
textOverlay.X = 50;   // Position horizontale depuis le bord gauche
textOverlay.Y = 50;   // Position verticale depuis le bord supérieur

// Vous pouvez également positionner le texte dans d'autres zones :
// Coin inférieur droit (en supposant une vidéo 1920x1080) :
// textOverlay.X = 1820;  // 1920 - 100 pour une certaine marge
// textOverlay.Y = 980;   // 1080 - 100 pour une certaine marge

// Centré (en supposant une vidéo 1920x1080 et en mesurant la taille du texte) :
// textOverlay.X = 960;   // Moitié de la largeur de la vidéo
// textOverlay.Y = 540;   // Moitié de la hauteur de la vidéo
```

## Travailler avec les polices

Le SDK exploite la bibliothèque SkiaSharp pour des capacités puissantes de rendu de texte. Ceci donne accès à toutes les polices système et aux fonctionnalités typographiques avancées.

### Récupérer les familles de polices disponibles

Vous pouvez récupérer dynamiquement la liste des polices disponibles sur le système actuel :

```cs
// Obtenir toutes les polices disponibles sur le système actuel
// Utile pour créer des listes déroulantes de sélection de police dans votre UI
var availableFonts = videoEdit.Fonts;

// Vous pouvez maintenant parcourir les polices ou les lier à un contrôle UI
foreach (var font in availableFonts)
{
    // Utiliser les informations de police selon les besoins
    Console.WriteLine(font);
}
```

## Techniques de style avancées

Pour des effets de texte plus sophistiqués, envisagez de combiner les superpositions de texte avec d'autres fonctionnalités du SDK :

- Appliquer des effets d'animation pour faire défiler le texte à l'écran
- Utiliser plusieurs superpositions de texte avec des minutages différents pour un affichage séquentiel
- Combiner avec des superpositions de formes pour créer des zones de texte avec arrière-plans personnalisés
- Intégrer avec des transitions vidéo pour des effets d'entrée et de sortie dynamiques du texte

## Ressources pour développeurs

Pour davantage d'exemples de code et des techniques d'implémentation avancées, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples) contenant des exemples complets du SDK .NET.
