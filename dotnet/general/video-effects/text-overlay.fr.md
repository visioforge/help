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
  - OverlayManagerText
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

### Texte qui change à chaque image

`TextOverlayVideoEffect` est conçu pour du texte qui change rarement, et il n'offre aucune fenêtre
temporelle. Pour un affichage en direct - une valeur de capteur, le numéro d'image, l'horloge -
utilisez `OverlayManagerText` via `Video_Overlay_Add` et fournissez-lui un `TextProvider`. Il est
interrogé une fois par image :

```csharp
// Le gestionnaire de superpositions n'est inséré dans le pipeline que si ceci vaut
// true, et il faut le définir avant Start/StartAsync.
videoCapture1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 40, y: 40);
text.Color = SKColors.Yellow;
text.Font.Size = 28;

// Appelé une fois par image sur le thread de streaming. L'argument est l'horodatage
// de l'image, compté depuis le démarrage du pipeline.
text.TextProvider = ts => "Camera 1\nOperator: demo\nREC\n"
    + $"Sensor {_sensorValue:F1}   {DateTime.Now:HH:mm:ss}   {ts:hh\\:mm\\:ss}";

videoCapture1.Video_Overlay_Add(text);
```

Les lignes statiques et dynamiques tiennent dans une seule chaîne : un bloc de légende comportant une
seule ligne vivante ne coûte donc qu'un appel par image. Renvoyer la même chaîne que la fois
précédente est peu coûteux : la mise en page du texte n'est remesurée que si la chaîne diffère
réellement.

Le callback s'exécute sur le thread de streaming, sous le même verrou que `Video_Overlay_Add` et
`Video_Overlay_Remove`. Gardez-le court et n'y bloquez pas - appeler `Dispatcher.Invoke` depuis
l'intérieur pour lire une valeur d'interface peut provoquer un interblocage, et pas seulement la
perte d'une image. Lisez plutôt un champ que le thread d'interface a déjà écrit. Un callback qui lève
une exception est journalisé une fois puis n'est plus appelé, et l'élément revient à son `Text` ;
réaffecter `TextProvider` le réactive.

`OverlayManagerText` respecte également `StartTime` et `EndTime`. Chaque borne fonctionne seule - un
`StartTime` nul signifie « depuis le début » et un `EndTime` nul signifie « sans fin » - et toutes
deux sont comparées à l'horodatage de l'image, non à l'horloge système.

Consultez la [page OverlayManagerBlock](../../mediablocks/VideoProcessing/OverlayManagerBlock.md)
pour la liste complète des éléments de superposition.

`X` et `Y` sont le coin supérieur gauche du texte, en pixels. `(0, 0)` est le coin supérieur gauche
de l'image et la première ligne d'une chaîne multiligne y est entièrement visible.

### Superpositions aperçu uniquement dans MediaPlayerCoreX

La même API `Video_Overlay_*` est disponible sur `MediaPlayerCoreX`. Définissez
`Video_Overlay_Enabled` avant `OpenAsync` / `PlayAsync`. La superposition est insérée uniquement sur
la branche du rendu, après le sample grabber et après toute sortie vidéo personnalisée, de sorte que
le fichier sur disque, les instantanés et les exports ne sont pas modifiés. Les éléments ajoutés avec
`Video_Overlay_Add` survivent à `Stop` et à l'ouverture d'un autre fichier :

```csharp
mediaPlayer1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 0, y: 0);
text.TextProvider = ts => $"T {ts:hh\\:mm\\:ss}";
mediaPlayer1.Video_Overlay_Add(text);
```

## Bonnes pratiques pour les superpositions de texte

- Tenez compte de la lisibilité sur différents arrière-plans
- Utilisez des tailles de police appropriées à la résolution d'affichage cible
- Implémentez des effets de fondu pour des superpositions moins intrusives
- Testez l'impact sur les performances des effets de texte complexes

---
Pour plus d'exemples de code et de détails d'implémentation, visitez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).