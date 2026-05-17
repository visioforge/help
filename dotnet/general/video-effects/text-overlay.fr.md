---
title: Superposition de texte vidéo en C# — polices et fondu
description: Créez des superpositions de texte dynamiques avec contrôle de police, couleur, position, rotation et animation pour horodatages, légendes et branding.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
  - Effects
  - C#
primary_api_classes:
  - TextOverlayVideoEffect
  - FontSettings
  - VideoEffectTextLogo

---

# Implémentation de superpositions de texte dans les flux vidéo

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [MediaPlayerCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introduction

Les superpositions de texte offrent un moyen puissant d'enrichir les flux vidéo avec des informations dynamiques, du branding, des légendes ou des horodatages. Ce guide explore comment implémenter des superpositions de texte entièrement personnalisables avec un contrôle précis de l'apparence, du positionnement et des animations.

## Implémentation avec le moteur classique

Nos moteurs classiques (VideoCaptureCore, MediaPlayerCore, VideoEditCore) offrent une API directe pour ajouter du texte aux flux vidéo.

### Implémentation basique de superposition de texte

L'exemple suivant montre une superposition de texte simple avec positionnement personnalisé :

```csharp
var effect = new VideoEffectTextLogo(true, "textoverlay");

// définir la position
effect.Left = 20;
effect.Top = 20;

// définir la police (System.Drawing.Font)
effect.Font = new Font("Arial", 40);

// définir le texte
effect.Text = "Hello, world!";

// définir la couleur du texte
effect.FontColor = Color.Yellow;

MediaPlayer1.Video_Effects_Add(effect);
```

### Options d'affichage d'informations dynamiques

#### Affichage d'horodatage et de date

Vous pouvez afficher automatiquement la date courante, l'heure ou les informations d'horodatage vidéo en utilisant des modes spécialisés :

```csharp
// définir le mode et le masque
effect.Mode = TextLogoMode.DateTime;
effect.DateTimeMask = "yyyy-MM-dd. hh:mm:ss";
```

Le SDK prend en charge des masques de formatage personnalisés pour les horodatages et les dates, permettant un contrôle précis du format d'affichage des informations. L'affichage du numéro d'image ne nécessite aucune configuration supplémentaire.

### Effets d'animation et de transition

#### Implémentation des effets de fondu

Créez des apparitions et disparitions de texte fluides avec des effets de fondu personnalisables :

```csharp
// ajouter le fondu d'entrée
effect.FadeIn = true; 
effect.FadeInDuration = TimeSpan.FromMilliseconds(5000);

// ajouter le fondu de sortie
effect.FadeOut = true;
effect.FadeOutDuration = TimeSpan.FromMilliseconds(5000);
```

### Options de rotation du texte

Faites pivoter votre superposition de texte selon vos exigences de design :

```csharp
// définir le mode de rotation
effect.RotationMode = TextRotationMode.Rm90;
```

### Transformations de retournement du texte

Appliquez des effets miroir à votre texte pour des présentations créatives :

```csharp
// définir le mode de retournement
effect.FlipMode = TextFlipMode.XAndY;
```

## Implémentation avec le moteur X

Nos nouveaux moteurs X (VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX) fournissent une API enrichie avec des fonctionnalités supplémentaires.

### Superposition de texte basique avec le moteur X

```csharp
// superposition de texte
var textOverlay = new TextOverlayVideoEffect() { Text = "Hello World!" };
 
// définir la position
textOverlay.XPad = 20;
textOverlay.YPad = 20;

textOverlay.HorizontalAlignment = TextOverlayHAlign.Left;
textOverlay.VerticalAlignment = TextOverlayVAlign.Top;

// définir la police — en utilisant l'initialiseur d'objet
textOverlay.Font = new FontSettings
{
    Name = "Arial",
    Size = 24,
    Weight = FontWeight.Bold
};

// Alternative : en utilisant le constructeur avec une chaîne de fonte
// textOverlay.Font = new FontSettings("Arial", "Bold", 24);

// définir le texte
textOverlay.Text = "Hello, world!";

// définir la couleur du texte
textOverlay.Color = SKColors.Yellow;

// ajouter l'effet
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Affichage avancé de contenu dynamique

#### Intégration de l'horodatage vidéo

Affichez la position courante dans la vidéo :

```csharp
// superposition de texte
var textOverlay = new TextOverlayVideoEffect();
  
// définir le texte
textOverlay.Text = "Timestamp: ";

// définir le mode horodatage
textOverlay.Mode = TextOverlayMode.Timestamp;

// ajouter l'effet
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

#### Intégration de l'heure système

Affichez l'heure système courante en parallèle de votre contenu vidéo :

```csharp
// superposition de texte
var textOverlay = new TextOverlayVideoEffect();
 
// définir le texte
textOverlay.Text = "Time: ";

// définir le mode heure système
textOverlay.Mode = TextOverlayMode.SystemTime;

// ajouter l'effet
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

## Bonnes pratiques pour les superpositions de texte

- Tenez compte de la lisibilité sur différents arrière-plans
- Utilisez des tailles de police appropriées à la résolution d'affichage cible
- Implémentez des effets de fondu pour des superpositions moins intrusives
- Testez l'impact sur les performances des effets de texte complexes

---
Pour plus d'exemples de code et de détails d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).