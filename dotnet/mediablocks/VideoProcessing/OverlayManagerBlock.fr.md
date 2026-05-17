---
title: Gestionnaire de superpositions vidéo — texte, image, PiP
description: Ajoutez texte, images, formes et superpositions PiP à la vidéo en direct avec VisioForge OverlayManagerBlock et une gestion temps réel.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - Effects
  - Decklink
  - NDI Source
  - RTSP
  - NDI
  - MP4
  - GIF
  - C#
primary_api_classes:
  - OverlayManagerBlock
  - OverlayManagerShadowSettings
  - VideoView
  - FontSettings
  - IVideoView

---

# Guide d'utilisation du bloc Overlay Manager

## Vue d'ensemble

L'`OverlayManagerBlock` est un puissant composant MediaBlocks qui fournit la composition et la gestion dynamiques de superpositions vidéo multicouches. Il vous permet d'ajouter divers éléments de superposition (images, texte, formes, animations) au-dessus du contenu vidéo avec mises à jour en temps réel, gestion des couches et fonctionnalités avancées comme les ombres, la rotation et le contrôle d'opacité.

## Fonctionnalités clés

- **Types de superposition multiples** : Texte, texte défilant, images, séquences d'images, GIF, SVG, formes (rectangles, cercles, triangles, étoiles, lignes), fichiers/URL vidéo, vidéo en direct (NDI, Decklink), contenu web WebView2 (Windows), contrôles WPF (Windows)
- **Superpositions de fichiers vidéo** : Lecture de fichiers vidéo ou d'URL de flux comme superpositions avec contrôle complet de la lecture et sortie audio optionnelle
- **Superpositions de contrôles WPF** : Rendu d'éléments WPF en direct avec animations et liaison de données comme superpositions vidéo (Windows uniquement)
- **Groupes de superpositions** : Synchronisation de plusieurs superpositions pour démarrage/arrêt coordonné avec prise en charge du préchargement
- **Effets Squeezeback** : Mise à l'échelle de la vidéo vers un rectangle personnalisé avec image de superposition par-dessus (style diffusion)
- **Transformations vidéo** : Effets de zoom et de panoramique qui transforment toute la trame vidéo
- **Prise en charge des animations** : Animation de position/échelle vidéo avec fonctions d'assouplissement
- **Effets de fondu** : Fondu enchaîné/sortant pour la vidéo et les éléments de superposition
- **Gestion des couches** : Ordre par Z-index pour un empilement correct des superpositions
- **Effets avancés** : Ombres, rotation, opacité, positionnement personnalisé
- **Mises à jour en temps réel** : Modification dynamique des superpositions pendant la lecture
- **Affichage temporisé** : Afficher/masquer les superpositions à des horodatages spécifiques
- **Dessin personnalisé** : Prise en charge de rappel pour les opérations de dessin Cairo personnalisées
- **Sources vidéo en direct** : Prise en charge des sources réseau NDI et des cartes de capture Decklink
- **Multiplateforme** : Fonctionne sous Windows, Linux, macOS, iOS et Android

## Référence de classe

### OverlayManagerBlock

**Espace de noms** : `VisioForge.Core.MediaBlocks.VideoProcessing`

```csharp
public class OverlayManagerBlock : MediaBlock, IMediaBlockInternals
```

#### Propriétés

| Propriété | Type | Description |
|----------|------|-------------|
| `Type` | `MediaBlockType` | Renvoie `MediaBlockType.OverlayManager` |
| `Input` | `MediaBlockPad` | Pad d'entrée vidéo |
| `Output` | `MediaBlockPad` | Pad de sortie vidéo avec superpositions |

#### Méthodes

##### Méthodes statiques

```csharp
public static bool IsAvailable()
```

Vérifie si le gestionnaire de superpositions est disponible dans l'environnement courant (nécessite la prise en charge de superposition Cairo).

##### Méthodes d'instance

```csharp
public void Video_Overlay_Add(IOverlayManagerElement overlay)
```

Ajoute un nouvel élément de superposition à la composition vidéo.

```csharp
public void Video_Overlay_Remove(IOverlayManagerElement overlay)
```

Supprime un élément de superposition spécifique.

```csharp
public void Video_Overlay_RemoveAt(int index)
```

Supprime une superposition à l'index spécifié.

```csharp
public void Video_Overlay_Clear()
```

Supprime tous les éléments de superposition.

```csharp
public void Video_Overlay_Update(IOverlayManagerElement overlay)
```

Met à jour une superposition existante (supprime et réajoute avec de nouvelles propriétés).

## Types d'éléments de superposition

### Propriétés communes (IOverlayManagerElement)

Tous les éléments de superposition implémentent l'interface `IOverlayManagerElement` avec ces propriétés communes :

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Name` | `string` | - | Nom optionnel pour identification |
| `Enabled` | `bool` | `true` | Activer/désactiver la superposition |
| `StartTime` | `TimeSpan` | `Zero` | Quand commencer à afficher (optionnel) |
| `EndTime` | `TimeSpan` | `Zero` | Quand arrêter d'afficher (optionnel) |
| `Opacity` | `double` | `1.0` | Transparence (0.0-1.0) |
| `Rotation` | `double` | `0.0` | Angle de rotation en degrés (0-360) |
| `ZIndex` | `int` | `0` | Ordre des couches (plus élevé = au-dessus) |
| `Shadow` | `OverlayManagerShadowSettings` | - | Configuration de l'ombre |

### OverlayManagerText

Affiche du texte avec arrière-plan et formatage optionnels.

```csharp
public class OverlayManagerText : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "Hello!!!" | Texte à afficher |
| `X` | `int` | `100` | Position X |
| `Y` | `int` | `100` | Position Y |
| `Font` | `FontSettings` | Système par défaut | Configuration de la police |
| `Color` | `SKColor` | `Red` | Couleur du texte |
| `Background` | `IOverlayManagerBackground` | `null` | Arrière-plan optionnel |
| `CustomWidth` | `int` | `0` | Largeur de cadrage personnalisée (0 = auto) |
| `CustomHeight` | `int` | `0` | Hauteur de cadrage personnalisée (0 = auto) |

**Exemple :**

```csharp
var text = new OverlayManagerText("Bonjour le monde !", 100, 100);
text.Color = SKColors.White;
text.Font.Size = 48;
text.Font.Name = "Arial";
text.Shadow = new OverlayManagerShadowSettings(true, depth: 5, direction: 45);
overlayManager.Video_Overlay_Add(text);
```

### OverlayManagerImage

Affiche des images statiques avec modes d'étirement.

```csharp
public class OverlayManagerImage : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur d'affichage (0 = originale) |
| `Height` | `int` | - | Hauteur d'affichage (0 = originale) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `None` | Mode de mise à l'échelle de l'image |

**Modes d'étirement :**

- `None` — Taille originale
- `Stretch` — Remplit la zone cible (peut déformer)
- `Letterbox` — S'adapte à la zone (préserve le ratio d'aspect)
- `CropToFill` — Remplit la zone par recadrage (préserve le ratio d'aspect)

**Constructeurs :**

```csharp
// Depuis un fichier
new OverlayManagerImage(string filename, int x, int y, double alpha = 1.0)

// Depuis un bitmap SkiaSharp
new OverlayManagerImage(SKBitmap image, int x, int y, double alpha = 1.0)

// Depuis System.Drawing.Bitmap (Windows uniquement)
new OverlayManagerImage(System.Drawing.Bitmap image, int x, int y, double alpha = 1.0)
```

**Exemple :**

```csharp
var image = new OverlayManagerImage("logo.png", 10, 10);
image.StretchMode = OverlayManagerImageStretchMode.Letterbox;
image.Width = 200;
image.Height = 100;
overlayManager.Video_Overlay_Add(image);
```

### OverlayManagerGIF

Affiche des images GIF animées.

```csharp
public class OverlayManagerGIF : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Position` | `SKPoint` | Position du GIF |
| `AnimationLength` | `TimeSpan` | Durée totale de l'animation |

**Exemple :**

```csharp
var gif = new OverlayManagerGIF("animation.gif", new SKPoint(150, 150));
overlayManager.Video_Overlay_Add(gif);
```

### OverlayManagerImageSequence

Affiche une séquence d'images, chacune affichée pendant une durée spécifiée, avec prise en charge de la boucle, des animations, des effets de fondu et de toutes les propriétés de superposition standard.

```csharp
public class OverlayManagerImageSequence : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | `0` | Largeur d'affichage (0 = originale) |
| `Height` | `int` | `0` | Hauteur d'affichage (0 = originale) |
| `Loop` | `bool` | `true` | Redémarrer la séquence après la dernière image |
| `StretchMode` | `OverlayManagerImageStretchMode` | `None` | Mode de mise à l'échelle de l'image |
| `AnimationLength` | `TimeSpan` | - | Durée totale de toutes les images (lecture seule) |
| `FrameCount` | `int` | - | Nombre d'images chargées (lecture seule) |

**Propriétés d'animation :**

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `AnimationEnabled` | `bool` | `false` | Activer l'animation de position/taille |
| `TargetX` | `int` | `0` | Cible X de l'animation |
| `TargetY` | `int` | `0` | Cible Y de l'animation |
| `TargetWidth` | `int` | `0` | Largeur cible de l'animation (0 = garder la courante) |
| `TargetHeight` | `int` | `0` | Hauteur cible de l'animation (0 = garder la courante) |
| `AnimationStartTime` | `TimeSpan` | `Zero` | Heure de début d'animation |
| `AnimationEndTime` | `TimeSpan` | `Zero` | Heure de fin d'animation |
| `Easing` | `OverlayManagerPanEasing` | `Linear` | Assouplissement d'animation de position |

**Propriétés de fondu :**

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `FadeEnabled` | `bool` | `false` | Activer l'animation de fondu |
| `FadeType` | `OverlayManagerFadeType` | `FadeIn` | Direction du fondu |
| `FadeStartTime` | `TimeSpan` | `Zero` | Heure de début du fondu |
| `FadeEndTime` | `TimeSpan` | `Zero` | Heure de fin du fondu |
| `FadeEasing` | `OverlayManagerPanEasing` | `Linear` | Fonction d'assouplissement de fondu |

**Constructeurs :**

```csharp
// Basique : position uniquement
new OverlayManagerImageSequence(IEnumerable<ImageSequenceItem> items, int x, int y)

// Complet : position, taille et mode d'étirement
new OverlayManagerImageSequence(
    IEnumerable<ImageSequenceItem> items,
    int x, int y,
    int width, int height,
    OverlayManagerImageStretchMode stretchMode = None)
```

**Méthodes :**

```csharp
// Ajouter dynamiquement une image pendant la lecture
void AddFrame(string filename, TimeSpan duration)

// Démarrer l'animation de position/taille
void StartAnimation(int targetX, int targetY, int targetWidth, int targetHeight,
    TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)

// Obtenir la position/taille interpolée à l'instant donné
(int X, int Y, int Width, int Height) GetCurrentRect(TimeSpan currentTime)

// Effets de fondu
void StartFadeIn(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
void StartFadeOut(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
double GetCurrentOpacity(TimeSpan currentTime)
```

**Exemple — Séquence d'images basique :**

```csharp
// Définir les images avec des durées par image
var items = new List<ImageSequenceItem>
{
    new ImageSequenceItem("slide1.png", TimeSpan.FromSeconds(3)),
    new ImageSequenceItem("slide2.png", TimeSpan.FromSeconds(2)),
    new ImageSequenceItem("slide3.png", TimeSpan.FromSeconds(4))
};

// Créer la séquence à la position (100, 100), mise à l'échelle à 320x240
var sequence = new OverlayManagerImageSequence(items, 100, 100, 320, 240)
{
    Loop = true,
    Opacity = 0.9,
    ZIndex = 5,
    Name = "SlideShow"
};

overlayManager.Video_Overlay_Add(sequence);
```

**Exemple — Avec animation et fondu :**

```csharp
// Animer la séquence du coin supérieur gauche au centre sur 3 secondes
sequence.StartAnimation(
    targetX: 400, targetY: 200,
    targetWidth: 640, targetHeight: 480,
    startTime: TimeSpan.FromSeconds(2),
    duration: TimeSpan.FromSeconds(3),
    easing: OverlayManagerPanEasing.EaseInOut);

// Fondu entrant sur les 1,5 premières secondes
sequence.StartFadeIn(
    startTime: TimeSpan.Zero,
    duration: TimeSpan.FromSeconds(1.5),
    easing: OverlayManagerPanEasing.EaseOut);
```

**Exemple — Ajouter des images dynamiquement :**

```csharp
// Ajouter une nouvelle image à une séquence en cours d'exécution
sequence.AddFrame("slide4.png", TimeSpan.FromSeconds(2.5));
```

**Méthodes utilitaires sur OverlayManagerBlock :**

```csharp
// Ajouter une superposition de séquence d'images
var element = overlayManager.Video_Overlay_AddImageSequence(
    items, x: 100, y: 100, width: 320, height: 240,
    loop: true, name: "SlideShow");

// Mettre à jour la position
overlayManager.Video_Overlay_UpdateImageSequencePosition(
    "SlideShow", x: 200, y: 150, width: 400, height: 300);

// Animer position/taille
overlayManager.Video_Overlay_AnimateImageSequence(
    "SlideShow", targetX: 500, targetY: 300, targetWidth: 640, targetHeight: 480,
    startTime: currentPosition, duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut);

// Effets de fondu
overlayManager.Video_Overlay_ImageSequenceFadeIn(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
overlayManager.Video_Overlay_ImageSequenceFadeOut(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
```

#### ImageSequenceItem

Représente une image unique dans une séquence d'images.

```csharp
public class ImageSequenceItem
```

| Propriété   | Type       | Description                     |
|------------|------------|---------------------------------|
| `Filename` | `string`   | Chemin complet vers le fichier image     |
| `Duration` | `TimeSpan` | Durée d'affichage de cette image |

```csharp
// Constructeurs
new ImageSequenceItem()
new ImageSequenceItem(string filename, TimeSpan duration)
```

### OverlayManagerDateTime

Affiche la date/heure actuelle avec un formatage personnalisé.

```csharp
public class OverlayManagerDateTime : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "[DATETIME]" | Modèle de texte |
| `Format` | `string` | "MM/dd/yyyy HH:mm:ss" | Format DateTime |
| `X` | `int` | `100` | Position X |
| `Y` | `int` | `100` | Position Y |
| `Font` | `FontSettings` | Système par défaut | Configuration de la police |
| `Color` | `SKColor` | `Red` | Couleur du texte |

**Exemple :**

```csharp
var dateTime = new OverlayManagerDateTime();
dateTime.Format = "yyyy-MM-dd HH:mm:ss";
dateTime.X = 10;
dateTime.Y = 30;
overlayManager.Video_Overlay_Add(dateTime);
```

### OverlayManagerScrollingText

Affiche du texte défilant qui se déplace à travers la vidéo dans une direction spécifiée.

```csharp
public class OverlayManagerScrollingText : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "Scrolling Text" | Texte à afficher |
| `X` | `int` | `0` | Position X de la zone de défilement |
| `Y` | `int` | `100` | Position Y de la zone de défilement |
| `Width` | `int` | `0` | Largeur de la zone de défilement (0 = utilise DefaultWidth) |
| `Height` | `int` | `0` | Hauteur de la zone de défilement (0 = auto selon la police) |
| `DefaultWidth` | `int` | `1920` | Largeur par défaut quand Width est 0 (à régler à la largeur vidéo) |
| `DefaultHeight` | `int` | `1080` | Hauteur par défaut quand Height est 0 pour défilement vertical |
| `Speed` | `int` | `5` | Vitesse de défilement en pixels par image |
| `Direction` | `ScrollDirection` | `RightToLeft` | Direction du défilement |
| `Font` | `FontSettings` | Système par défaut | Configuration de la police |
| `Color` | `SKColor` | `White` | Couleur du texte |
| `BackgroundTransparent` | `bool` | `true` | Si l'arrière-plan est transparent |
| `BackgroundColor` | `SKColor` | `Black` | Couleur d'arrière-plan (quand non transparent) |
| `Infinite` | `bool` | `true` | Boucle infinie de défilement |
| `TextRestarted` | `EventHandler` | `null` | Appelé quand le texte revient au début |

**Enum ScrollDirection :**

- `LeftToRight` — Le texte défile de gauche à droite
- `RightToLeft` — Le texte défile de droite à gauche
- `BottomToTop` — Le texte défile du bas vers le haut
- `TopToBottom` — Le texte défile du haut vers le bas

**Exemple :**

```csharp
// Créer un texte défilant de style téléscripteur de nouvelles
var scrollingText = new OverlayManagerScrollingText(
    "Dernières nouvelles : VisioForge Media Framework prend désormais en charge les superpositions de texte défilant !",
    x: 0,
    y: 50,
    speed: 3,
    direction: ScrollDirection.RightToLeft);

scrollingText.Font.Size = 24;
scrollingText.Color = SKColors.Yellow;
scrollingText.BackgroundTransparent = false;
scrollingText.BackgroundColor = SKColors.DarkBlue;

// Définir la largeur par défaut pour correspondre à votre résolution vidéo
// Elle est utilisée lorsque Width n'est pas explicitement défini
scrollingText.DefaultWidth = 1920; // Largeur Full HD

// Ou définir Width directement pour une largeur de zone de défilement spécifique
// scrollingText.Width = 1920;

// Ajouter un gestionnaire d'événement pour quand le texte boucle
scrollingText.TextRestarted += (sender, e) => {
    Console.WriteLine("Le texte défilant a redémarré");
};

overlayManager.Video_Overlay_Add(scrollingText);

// Pour réinitialiser la position de défilement
scrollingText.Reset();

// Pour mettre à jour après modification du texte ou de la police
scrollingText.Text = "Texte de nouvelles mis à jour...";
scrollingText.Update();
```

### Superpositions vidéo

Le gestionnaire de superpositions prend en charge les superpositions vidéo provenant de plusieurs sources, notamment des fichiers/URL vidéo, des cartes de capture Decklink et des sources réseau NDI. Les superpositions vidéo sont lues dans la composition de superposition avec un contrôle complet de la lecture.

#### OverlayManagerVideo

Lit des fichiers vidéo ou des URL de flux comme superpositions sur la composition vidéo. Chaque superposition vidéo exécute son propre pipeline de lecture interne avec sortie audio optionnelle.

```csharp
public class OverlayManagerVideo : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Source` | `string` | - | Chemin du fichier source vidéo ou URL |
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur de superposition |
| `Height` | `int` | - | Hauteur de superposition |
| `Loop` | `bool` | `true` | Si la vidéo boucle |
| `PlaybackRate` | `double` | `1.0` | Vitesse de lecture (1.0 = normale) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Comment ajuster la vidéo |
| `VideoView` | `IVideoView` | `null` | Fenêtre d'aperçu vidéo externe optionnelle |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Paramètres du moteur de rendu pour VideoView |
| `AudioOutput` | `AudioOutputDeviceInfo` | `null` | Périphérique de sortie audio (null = ignorer l'audio) |
| `AudioOutput_Volume` | `double` | `1.0` | Volume audio (0.0-1.0+, au-dessus de 1.0 amplifie) |
| `AudioOutput_Mute` | `bool` | `false` | Couper la sortie audio |

**Méthodes :**

- `Initialize(bool autoStart)` — Initialise le pipeline vidéo. Si `autoStart` est true, commence la lecture immédiatement ; si false, précharge à l'état PAUSED.
- `Play()` — Démarre ou reprend la lecture vidéo
- `Pause()` — Met en pause la lecture vidéo
- `Stop()` — Arrête la lecture vidéo
- `Seek(TimeSpan position)` — Recherche à une position spécifique dans la vidéo
- `UpdateSource(string source)` — Modifie dynamiquement la source vidéo
- `Dispose()` — Nettoie les ressources

**Exemple — Superposition de fichier vidéo :**

```csharp
// Créer une superposition de fichier vidéo
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 100,
    y: 100,
    width: 640,
    height: 360)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Activer optionnellement la sortie audio
var audioOutputs = await AudioRendererBlock.GetDevicesAsync(AudioOutputDeviceAPI.DirectSound);
videoOverlay.AudioOutput = audioOutputs[0];
videoOverlay.AudioOutput_Volume = 0.5;

// Initialiser et ajouter au gestionnaire de superpositions
if (videoOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(videoOverlay);
}

// Contrôler la lecture à l'exécution
videoOverlay.Pause();
videoOverlay.Seek(TimeSpan.FromSeconds(10));
videoOverlay.Play();

// Changer la source dynamiquement
videoOverlay.UpdateSource("outro.mp4");

// Nettoyer une fois terminé
videoOverlay.Stop();
videoOverlay.Dispose();
```

**Exemple — Image dans l'image :**

```csharp
// Créer une petite vidéo PiP dans le coin
var pipVideo = new OverlayManagerVideo(
    source: "camera_feed.mp4",
    x: 20,
    y: 20,
    width: 240,
    height: 135)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 100,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.DarkGray,
        Opacity = 0.7,
        BlurRadius = 8,
        Depth = 3,
        Direction = 45
    }
};

if (pipVideo.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(pipVideo);
}
```

**Exemple — Superposition d'URL de flux :**

```csharp
// Superposer un flux réseau
var streamOverlay = new OverlayManagerVideo(
    source: "rtsp://192.168.1.21:554/Streaming/Channels/101",
    x: 400,
    y: 50,
    width: 320,
    height: 240)
{
    Loop = false,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 10
};

if (streamOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(streamOverlay);
}
```

#### OverlayManagerDecklinkVideo

Capture et affiche la vidéo depuis des cartes de capture Blackmagic Decklink.

```csharp
public class OverlayManagerDecklinkVideo : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `DecklinkSettings` | `DecklinkVideoSourceSettings` | - | Configuration du périphérique Decklink |
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur de superposition |
| `Height` | `int` | - | Hauteur de superposition |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Comment ajuster la vidéo |
| `VideoView` | `IVideoView` | `null` | Aperçu vidéo optionnel |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Paramètres du moteur de rendu |

**Exemple :**

```csharp
// Obtenir les périphériques Decklink
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
var decklinkSettings = new DecklinkVideoSourceSettings(devices[0]);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

// Créer la superposition Decklink
var decklinkOverlay = new OverlayManagerDecklinkVideo(
    decklinkSettings,
    x: 10,
    y: 10,
    width: 640,
    height: 360);

// Initialiser et ajouter au gestionnaire de superpositions
if (decklinkOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(decklinkOverlay);
}

// Nettoyer une fois terminé
decklinkOverlay.Stop();
decklinkOverlay.Dispose();
```

#### OverlayManagerNDIVideo

Capture et affiche la vidéo depuis des sources NDI (Network Device Interface).

```csharp
public class OverlayManagerNDIVideo : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `NDISettings` | `NDISourceSettings` | - | Configuration de la source NDI |
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur de superposition |
| `Height` | `int` | - | Hauteur de superposition |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Comment ajuster la vidéo |
| `VideoView` | `IVideoView` | `null` | Aperçu vidéo optionnel |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Paramètres du moteur de rendu |

**Exemple :**

```csharp
// Découvrir les sources NDI sur le réseau
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSettings = await NDISourceSettings.CreateAsync(
    null,
    ndiSources[0].Name,
    ndiSources[0].URL);

// Créer la superposition NDI
var ndiOverlay = new OverlayManagerNDIVideo(
    ndiSettings,
    x: 10,
    y: 10,
    width: 640,
    height: 360);

// Initialiser et ajouter au gestionnaire de superpositions
if (ndiOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(ndiOverlay);
}

// Nettoyer une fois terminé
ndiOverlay.Stop();
ndiOverlay.Dispose();
```

**Méthodes communes pour les superpositions vidéo :**

- `Initialize(bool autoStart)` — Initialise le pipeline vidéo
- `Play()` — Démarre ou reprend la lecture/capture vidéo
- `Pause()` — Met en pause la lecture/capture vidéo
- `Stop()` — Arrête la lecture/capture vidéo
- `Dispose()` — Nettoie les ressources

**Méthodes supplémentaires (OverlayManagerVideo uniquement) :**

- `Seek(TimeSpan position)` — Recherche à une position spécifique
- `UpdateSource(string source)` — Modifie dynamiquement la source vidéo

### OverlayManagerWPFControl (Windows uniquement)

Rend un `FrameworkElement` WPF comme superposition vidéo. Cela permet d'utiliser tout arbre visuel WPF — y compris les contrôles avec animations Storyboard, liaison de données et dispositions complexes — comme superposition. L'élément est capturé périodiquement à une fréquence configurable.

> **Note** : ce type de superposition n'est disponible que sous Windows (cible de build `NET_WINDOWS`).

```csharp
public class OverlayManagerWPFControl : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `ElementFactory` | `Func<FrameworkElement>` | - | Fabrique qui crée l'élément WPF sur le thread STA interne |
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur de superposition (doit être > 0) |
| `Height` | `int` | - | Hauteur de superposition (doit être > 0) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Stretch` | Comment ajuster le contrôle rendu |
| `RefreshRate` | `int` | `15` | Captures par seconde (1-60) |
| `Dpi` | `double` | `96` | DPI pour le rendu |

**Méthodes :**

- `Initialize()` — Initialise la superposition de contrôle WPF et démarre le rendu périodique
- `InvokeOnUIThread(Action action)` — Exécute une action sur le thread STA WPF pour des mises à jour sûres à l'exécution
- `InvokeOnUIThread<T>(Func<T> func)` — Exécute une fonction sur le thread STA WPF et renvoie le résultat
- `Dispose()` — Nettoie les ressources

**Méthode utilitaire sur OverlayManagerBlock :**

```csharp
public OverlayManagerWPFControl Video_Overlay_AddWPFControl(
    Func<FrameworkElement> elementFactory,
    int x, int y, int width, int height,
    int refreshRate = 15,
    string name = null)
```

Crée, initialise et ajoute une superposition de contrôle WPF en un seul appel. Renvoie l'instance de superposition, ou `null` si l'initialisation a échoué.

**Exemple — Utilisation de la méthode utilitaire :**

```csharp
// Ajouter une superposition de texte avec cycle de couleurs via la méthode utilitaire
var wpfOverlay = overlayManager.Video_Overlay_AddWPFControl(
    elementFactory: () =>
    {
        var border = new Border
        {
            Width = 350,
            Height = 60,
            Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15, 5, 15, 5)
        };

        var text = new TextBlock
        {
            Text = "VisioForge Media Framework",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var brush = new SolidColorBrush(Colors.Red);
        text.Foreground = brush;
        border.Child = text;

        // Animer la couleur du texte
        var colorAnim = new ColorAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromSeconds(5),
            RepeatBehavior = RepeatBehavior.Forever
        };
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(0.0)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Gold, KeyTime.FromPercent(0.25)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Cyan, KeyTime.FromPercent(0.5)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Magenta, KeyTime.FromPercent(0.75)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(1.0)));
        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

        return border;
    },
    x: 50, y: 300, width: 350, height: 60,
    refreshRate: 30, name: "ColorText");

if (wpfOverlay == null)
{
    // Échec de l'initialisation
}
```

**Exemple — Création manuelle avec horloge animée :**

```csharp
// Créer manuellement une superposition d'horloge analogique WPF
var clockOverlay = new OverlayManagerWPFControl(
    elementFactory: () =>
    {
        var canvas = new Canvas { Width = 200, Height = 200 };

        // Cadran de l'horloge
        var face = new Ellipse
        {
            Width = 180, Height = 180,
            Stroke = Brushes.White, StrokeThickness = 3
        };
        Canvas.SetLeft(face, 10);
        Canvas.SetTop(face, 10);
        canvas.Children.Add(face);

        // Aiguille des secondes avec animation de rotation
        var secondHand = new Line
        {
            X1 = 100, Y1 = 100, X2 = 100, Y2 = 25,
            Stroke = Brushes.Red, StrokeThickness = 1
        };
        var secondRotate = new RotateTransform(DateTime.Now.Second * 6, 100, 100);
        secondHand.RenderTransform = secondRotate;
        canvas.Children.Add(secondHand);

        var anim = new DoubleAnimation
        {
            From = DateTime.Now.Second * 6,
            To = DateTime.Now.Second * 6 + 360,
            Duration = TimeSpan.FromSeconds(60),
            RepeatBehavior = RepeatBehavior.Forever
        };
        secondRotate.BeginAnimation(RotateTransform.AngleProperty, anim);

        return canvas;
    },
    x: 20, y: 20, width: 200, height: 200, refreshRate: 30);

if (clockOverlay.Initialize())
{
    overlayManager.Video_Overlay_Add(clockOverlay);
}

// Mettre à jour le contrôle WPF à l'exécution (thread-safe)
clockOverlay.InvokeOnUIThread(() =>
{
    // Sûr de modifier les éléments WPF ici
});

// Nettoyer
clockOverlay.Dispose();
```

### OverlayManagerWebView2Video (Windows uniquement)

Rend du contenu web en direct (HTML, CSS, JavaScript) comme superposition vidéo à l'aide de Microsoft WebView2. Cela permet d'afficher des pages web, des tableaux de bord HTML dynamiques, du contenu web animé, des téléscripteurs de nouvelles ou tout contenu rendu par navigateur comme superposition sur votre vidéo. La page web est rendue hors écran et capturée comme images vidéo à la fréquence de rafraîchissement native du navigateur.

> **Note** : ce type de superposition n'est disponible que sous Windows (cible de build `NET_WINDOWS`). Nécessite le runtime Microsoft WebView2 et le plugin GStreamer WebView2 (`webview2src`).

```csharp
public class OverlayManagerWebView2Video : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `Location` | `string` | `"about:blank"` | URL à afficher dans la superposition |
| `JavaScript` | `string` | `null` | Code JavaScript à exécuter après chaque navigation terminée |
| `Adapter` | `int` | `-1` | Index d'adaptateur DXGI pour la sélection GPU (-1 = tout périphérique disponible) |
| `UserDataFolder` | `string` | `null` | Chemin absolu vers le dossier de données utilisateur WebView2 pour le cache et le profil |
| `X` | `int` | - | Position X |
| `Y` | `int` | - | Position Y |
| `Width` | `int` | - | Largeur de superposition |
| `Height` | `int` | - | Hauteur de superposition |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | Comment ajuster le contenu rendu |
| `VideoView` | `IVideoView` | `null` | Fenêtre d'aperçu vidéo externe optionnelle |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Paramètres du moteur de rendu pour VideoView |

**Méthodes :**

- `Initialize(bool autoStart = true)` — Initialise le pipeline de rendu WebView2. Si `autoStart` est true, commence le rendu immédiatement ; si false, précharge à l'état PAUSED. Renvoie `true` en cas de succès.
- `Play()` — Démarre ou reprend le rendu de la page web
- `Pause()` — Met en pause le rendu de la page web
- `Stop()` — Arrête le rendu de la page web
- `UpdateLocation(string location)` — Modifie dynamiquement l'URL affichée
- `Dispose()` — Nettoie les ressources

**Exemple — Superposition de page web basique :**

```csharp
// Afficher une page web comme superposition vidéo
var webOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/dashboard",
    x: 50,
    y: 50,
    width: 640,
    height: 480)
{
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Initialiser et ajouter au gestionnaire de superpositions
if (webOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(webOverlay);
}
else
{
    webOverlay.Dispose();
}
```

**Exemple — Superposition web avec injection JavaScript :**

```csharp
// Superposer une page web et injecter du JavaScript pour la personnaliser
var tickerOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/ticker",
    x: 0,
    y: 680,
    width: 1920,
    height: 40)
{
    // JavaScript exécuté après chaque navigation terminée
    JavaScript = "document.body.style.background = 'transparent';",
    Opacity = 0.8,
    ZIndex = 10,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        Opacity = 0.5,
        BlurRadius = 5,
        Depth = 5,
        Direction = 45
    }
};

if (tickerOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(tickerOverlay);
}
```

**Exemple — Mise à jour dynamique d'URL à l'exécution :**

```csharp
// Modifier la page affichée à l'exécution
webOverlay.UpdateLocation("https://example.com/new-page");

// Contrôler le rendu
webOverlay.Pause();
webOverlay.Play();

// Nettoyer une fois terminé
webOverlay.Stop();
webOverlay.Dispose();
```

### Superpositions de formes

#### OverlayManagerLine

```csharp
public class OverlayManagerLine : IOverlayManagerElement
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Start` | `SKPoint` | Point de départ de la ligne |
| `End` | `SKPoint` | Point d'arrivée de la ligne |
| `Color` | `SKColor` | Couleur de la ligne |

#### OverlayManagerRectangle

```csharp
public class OverlayManagerRectangle : IOverlayManagerElement
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Rectangle` | `SKRect` | Limites du rectangle |
| `Color` | `SKColor` | Couleur de remplissage/contour |
| `Fill` | `bool` | Remplissage ou contour seulement |

#### OverlayManagerCircle

```csharp
public class OverlayManagerCircle : IOverlayManagerElement
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Center` | `SKPoint` | Centre du cercle |
| `Radius` | `double` | Rayon du cercle |
| `Color` | `SKColor` | Couleur de remplissage/contour |
| `Fill` | `bool` | Remplissage ou contour seulement |

#### OverlayManagerTriangle

```csharp
public class OverlayManagerTriangle : IOverlayManagerElement
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Point1` | `SKPoint` | Premier sommet |
| `Point2` | `SKPoint` | Deuxième sommet |
| `Point3` | `SKPoint` | Troisième sommet |
| `Color` | `SKColor` | Couleur de remplissage/contour |
| `Fill` | `bool` | Remplissage ou contour seulement |

#### OverlayManagerStar

```csharp
public class OverlayManagerStar : IOverlayManagerElement
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Center` | `SKPoint` | Centre de l'étoile |
| `OuterRadius` | `double` | Rayon des pointes externes |
| `InnerRadius` | `double` | Rayon des pointes internes |
| `StrokeColor` | `SKColor` | Couleur du contour |
| `FillColor` | `SKColor` | Couleur de remplissage |

### OverlayManagerSVG

Affiche des graphiques vectoriels SVG.

```csharp
public class OverlayManagerSVG : IOverlayManagerElement, IDisposable
```

| Propriété | Type | Description |
|----------|------|-------------|
| `X` | `int` | Position X |
| `Y` | `int` | Position Y |
| `Width` | `int` | Largeur d'affichage |
| `Height` | `int` | Hauteur d'affichage |

### OverlayManagerCallback

Dessin personnalisé à l'aide de graphismes Cairo.

```csharp
public class OverlayManagerCallback : IOverlayManagerElement
```

**Événement :**

```csharp
public event EventHandler<OverlayManagerCallbackEventArgs> OnDraw;
```

**Exemple :**

```csharp
var callback = new OverlayManagerCallback();
callback.OnDraw += (sender, e) => {
    var ctx = e.Context;
    ctx.SetSourceRGB(1, 0, 0);
    ctx.Arc(200, 200, 50, 0, 2 * Math.PI);
    ctx.Fill();
};
overlayManager.Video_Overlay_Add(callback);
```

### OverlayManagerGroup

Regroupe plusieurs superpositions pour une gestion synchronisée du cycle de vie. Ceci est particulièrement utile lorsque vous devez précharger plusieurs superpositions vidéo et les démarrer exactement en même temps.

```csharp
public class OverlayManagerGroup : IOverlayManagerElement, IDisposable
```

| Propriété   | Type                            | Par défaut    | Description                         |
|------------|----------------------------------|------------|-------------------------------------|
| `Overlays` | `List<IOverlayManagerElement>`   | Liste vide | Superpositions dans ce groupe (lecture seule)  |

> **Note** : les propriétés standard `IOverlayManagerElement` (`Opacity`, `Rotation`, `ZIndex`, `Shadow`) sont présentes sur le groupe mais ne sont pas appliquées au niveau du groupe — les propriétés individuelles des superpositions dans le groupe sont utilisées pour le rendu.

**Méthodes :**

- `Add(IOverlayManagerElement overlay)` — Ajoute une superposition au groupe. Lève `InvalidOperationException` si déjà initialisé.
- `Remove(IOverlayManagerElement overlay)` — Supprime une superposition du groupe. Lève `InvalidOperationException` si déjà initialisé.
- `Initialize()` — Précharge toutes les superpositions `OverlayManagerVideo` du groupe à l'état PAUSED. Renvoie `true` si toutes ont réussi. Les autres types de superposition (Decklink, NDI) doivent être initialisés manuellement.
- `Play()` — Démarre toutes les superpositions `OverlayManagerVideo` de manière synchrone. Doit appeler `Initialize()` en premier. Les autres types de superpositions vidéo doivent appeler `Play()` individuellement.
- `Pause()` — Met en pause toutes les superpositions `OverlayManagerVideo` du groupe.
- `Stop()` — Arrête toutes les superpositions `OverlayManagerVideo` du groupe.
- `GetRenderableOverlays()` — Renvoie toutes les superpositions activées du groupe (utilisé en interne).
- `Dispose()` — Arrête et libère toutes les superpositions du groupe.

> **Important** : vous ne pouvez pas ajouter ou supprimer de superpositions après avoir appelé `Initialize()`.

**Pourquoi ajouter des superpositions non-vidéo à un groupe ?** Bien que `Initialize()`, `Play()`, `Pause()` et `Stop()` ne contrôlent que les instances `OverlayManagerVideo`, ajouter d'autres types de superpositions (Decklink, NDI, texte, images) à un groupe fournit un regroupement organisationnel pour le rendu et une libération centralisée — `Dispose()` nettoie **toutes** les superpositions du groupe quel que soit leur type.

**Exemple — Groupe synchronisé vidéo + Decklink :**

```csharp
// Créer un groupe pour les superpositions qui doivent démarrer simultanément
var group = new OverlayManagerGroup("SyncGroup");

// Ajouter une superposition de fichier vidéo
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 10, y: 10, width: 640, height: 360)
{
    Loop = true,
    ZIndex = 10
};
group.Add(videoOverlay);

// Ajouter une superposition de capture Decklink
var decklinkSettings = new DecklinkVideoSourceSettings(deviceNumber);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

var decklinkOverlay = new OverlayManagerDecklinkVideo(
    settings: decklinkSettings,
    x: 660, y: 10, width: 640, height: 360)
{
    ZIndex = 11
};

// Initialiser Decklink manuellement (le groupe ne gère qu'OverlayManagerVideo)
decklinkOverlay.Initialize(autoStart: false);
group.Add(decklinkOverlay);

// Vous pouvez aussi mélanger des superpositions non-vidéo
var label = new OverlayManagerText("Caméra 1", 10, 380);
label.Font.Size = 14;
label.Color = SKColors.White;
group.Add(label);

// Ajouter le groupe au gestionnaire de superpositions
overlayManager.Video_Overlay_Add(group);

// Démarrer toutes les superpositions OverlayManagerVideo du groupe
group.Play();
// Decklink doit être démarré séparément (group.Play() ne gère qu'OverlayManagerVideo)
decklinkOverlay.Play();

// Plus tard, mettre en pause/arrêter tout en même temps
group.Pause();
group.Stop();

// Nettoyer
group.Dispose();
```

## Effets de transformation vidéo

L'OverlayManagerBlock prend en charge des effets avancés de transformation vidéo qui modifient toute la trame vidéo, pas seulement les superpositions par-dessus.

### OverlayManagerZoom

Applique un effet de zoom à la trame vidéo, mettant à l'échelle depuis un point central.

```csharp
public class OverlayManagerZoom : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `ZoomFactor` | `double` | `1.0` | Niveau de zoom (1.0 = pas de zoom, 2.0 = zoom 2x) |
| `CenterX` | `double` | `0.5` | Centre horizontal (0.0-1.0, relatif à la trame) |
| `CenterY` | `double` | `0.5` | Centre vertical (0.0-1.0, relatif à la trame) |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Compromis qualité/vitesse |

**Exemple :**

```csharp
// Créer un zoom 1,5x centré sur la trame
var zoom = new OverlayManagerZoom
{
    ZoomFactor = 1.5,
    CenterX = 0.5,
    CenterY = 0.5,
    Name = "VideoZoom"
};
zoom.ZIndex = -1000; // Traiter avant les autres superpositions
overlayManager.Video_Overlay_Add(zoom);

// Animer le zoom au fil du temps
zoom.ZoomFactor = 2.0; // Mise à jour dynamique
```

### OverlayManagerPan

Applique un effet de panoramique (translation) à la trame vidéo.

```csharp
public class OverlayManagerPan : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `OffsetX` | `double` | `0.0` | Décalage horizontal en pixels |
| `OffsetY` | `double` | `0.0` | Décalage vertical en pixels |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Compromis qualité/vitesse |

**Exemple :**

```csharp
// Panoramique de la vidéo de 100 pixels à droite et 50 pixels vers le bas
var pan = new OverlayManagerPan
{
    OffsetX = 100,
    OffsetY = 50,
    Name = "VideoPan"
};
pan.ZIndex = -1000; // Traiter avant les autres superpositions
overlayManager.Video_Overlay_Add(pan);
```

### OverlayManagerFade

Applique un effet de fondu à toute la trame vidéo.

```csharp
public class OverlayManagerFade : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `FadeMode` | `OverlayManagerFadeMode` | `None` | Type de fondu (None, FadeIn, FadeOut) |
| `StartTime` | `TimeSpan` | `Zero` | Quand l'effet de fondu commence |
| `Duration` | `TimeSpan` | `1 seconde` | Combien de temps dure le fondu |
| `MinOpacity` | `double` | `0.0` | Opacité minimale (entièrement fondu) |
| `MaxOpacity` | `double` | `1.0` | Opacité maximale (entièrement visible) |

**Exemple :**

```csharp
// Fondu entrant de la vidéo sur 2 secondes à partir de la position de lecture
var fade = new OverlayManagerFade
{
    FadeMode = OverlayManagerFadeMode.FadeIn,
    StartTime = TimeSpan.Zero,
    Duration = TimeSpan.FromSeconds(2),
    Name = "VideoFade"
};
overlayManager.Video_Overlay_Add(fade);
```

### OverlayManagerSqueezeback

Crée un effet « squeezeback » de style diffusion où la vidéo est mise à l'échelle vers un rectangle personnalisé et une image de superposition (généralement PNG avec transparence alpha) est dessinée par-dessus. Ceci est couramment utilisé pour les bandeaux inférieurs, les cadres et les graphismes de diffusion.

```csharp
public class OverlayManagerSqueezeback : IOverlayManagerElement
```

| Propriété | Type | Par défaut | Description |
|----------|------|---------|-------------|
| `BackgroundImageFilename` | `string` | - | Chemin de l'image de superposition (PNG avec alpha recommandé) |
| `VideoRect` | `Rect` | - | Où la vidéo est mise à l'échelle et positionnée |
| `BackgroundRect` | `Rect` | `null` | Position de l'image de superposition (null = trame complète) |
| `VideoOnTop` | `bool` | `false` | Ordre des couches (false = vidéo en dessous, image au-dessus) |
| `VideoOpacity` | `double` | `1.0` | Opacité de la couche vidéo |
| `BackgroundOpacity` | `double` | `1.0` | Opacité de l'image de superposition |

**Propriétés d'animation :**

| Propriété | Type | Description |
|----------|------|-------------|
| `VideoAnimationEnabled` | `bool` | Activer l'animation de position vidéo |
| `VideoAnimationStartRect` | `Rect` | Position de début de l'animation |
| `VideoAnimationTargetRect` | `Rect` | Position de fin de l'animation |
| `VideoAnimationStartTimeMs` | `double` | Heure de début de l'animation (ms) |
| `VideoAnimationDurationMs` | `double` | Durée de l'animation (ms) |
| `VideoAnimationEasing` | `OverlayManagerPanEasing` | Fonction d'assouplissement |

**Propriétés de fondu :**

| Propriété | Type | Description |
|----------|------|-------------|
| `VideoFadeMode` | `OverlayManagerFadeMode` | Type de fondu vidéo |
| `VideoFadeStartTimeMs` | `double` | Heure de début du fondu vidéo |
| `VideoFadeDurationMs` | `double` | Durée du fondu vidéo |
| `BackgroundFadeMode` | `OverlayManagerFadeMode` | Type de fondu d'arrière-plan |
| `BackgroundFadeStartTimeMs` | `double` | Heure de début du fondu d'arrière-plan |
| `BackgroundFadeDurationMs` | `double` | Durée du fondu d'arrière-plan |

**Exemple — Squeezeback basique :**

```csharp
// Créer un squeezeback avec la vidéo dans le coin inférieur droit
var squeezeback = new OverlayManagerSqueezeback
{
    BackgroundImageFilename = "frame.png", // PNG avec centre transparent
    VideoRect = new Rect(960, 540, 1920, 1080), // Quadrant inférieur droit
    Name = "Squeezeback"
};
squeezeback.ZIndex = -2000; // Traiter en premier
overlayManager.Video_Overlay_Add(squeezeback);
```

**Exemple — Squeezeback animé :**

```csharp
// Obtenir la position de lecture courante
var position = await pipeline.Position_GetAsync();

// Animer la vidéo de son VideoRect courant vers un coin sur 2 secondes.
// AnimateVideo n'a pas de startRect — la position de début est implicite (le VideoRect courant de l'élément),
// donc configurez-le au préalable si vous avez besoin d'un point de départ spécifique.
var squeezeback = overlayManager.Video_Overlay_GetByName("Squeezeback") as OverlayManagerSqueezeback;
squeezeback.VideoRect = new Rect(0, 0, 1920, 1080); // Point de départ plein écran
squeezeback.AnimateVideo(
    targetRect: new Rect(1280, 720, 1920, 1080), // Coin inférieur droit
    startTime: position,
    duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut
);
```

**Exemple — Effets de fondu :**

```csharp
var position = await pipeline.Position_GetAsync();

// Fondu sortant de la vidéo sur 1,5 secondes
squeezeback.StartVideoFadeOut(position, TimeSpan.FromSeconds(1.5));

// Plus tard, fondu entrant
squeezeback.StartVideoFadeIn(position, TimeSpan.FromSeconds(1.5));

// Fondu de l'image d'arrière-plan indépendamment
squeezeback.StartBackgroundFadeOut(position, TimeSpan.FromSeconds(1));
```

**Méthodes utilitaires sur OverlayManagerBlock :**

```csharp
// Ajouter un squeezeback
var element = overlayManager.Video_Overlay_AddSqueezeback(
    backgroundImageFilename: "frame.png",
    videoRect: new Rect(960, 540, 1920, 1080),
    backgroundRect: null,  // Trame complète
    name: "Squeezeback"
);

// Mettre à jour la position vidéo
overlayManager.Video_Overlay_Squeezeback_UpdateVideoPosition("Squeezeback", newRect);

// Animer la vidéo (pas de startRect — le départ est le VideoRect courant de l'élément).
overlayManager.Video_Overlay_Squeezeback_AnimateVideo(
    "Squeezeback", targetRect, startTime, duration, easing);

// Contrôles de fondu
overlayManager.Video_Overlay_Squeezeback_VideoFadeIn("Squeezeback", startTime, duration);
overlayManager.Video_Overlay_Squeezeback_VideoFadeOut("Squeezeback", startTime, duration);

// Ordre des couches
overlayManager.Video_Overlay_Squeezeback_SetVideoOnTop("Squeezeback");
overlayManager.Video_Overlay_Squeezeback_SetBackgroundOnTop("Squeezeback");
```

### Fonctions d'assouplissement

Options d'assouplissement d'animation disponibles via `OverlayManagerPanEasing` :

| Valeur | Description |
|-------|-------------|
| `Linear` | Vitesse constante |
| `EaseIn` | Début lent, fin rapide |
| `EaseOut` | Début rapide, fin lente |
| `EaseInOut` | Début et fin lents |
| `EaseInCubic` | Début cubique lent |
| `EaseOutCubic` | Fin cubique lente |
| `EaseInOutCubic` | Début et fin cubiques lents |

### Modes d'interpolation

Compromis qualité/performance pour la mise à l'échelle via `OverlayManagerInterpolationMode` :

| Valeur | Description |
|-------|-------------|
| `Nearest` | Le plus rapide, bords pixelisés |
| `Bilinear` | Bon équilibre qualité/vitesse |
| `Gaussian` | Qualité supérieure, plus lent |

## Paramètres d'ombre

Configurer les ombres portées pour les éléments de superposition :

```csharp
public class OverlayManagerShadowSettings
```

| Propriété | Type | Plage | Par défaut | Description |
|----------|------|-------|---------|-------------|
| `Enabled` | `bool` | - | `false` | Activer les ombres |
| `Depth` | `double` | 0-30 | `5.0` | Distance de décalage de l'ombre |
| `Direction` | `double` | 0-360° | `45.0` | Direction de l'ombre |
| `Opacity` | `double` | 0-1 | `0.5` | Transparence de l'ombre |
| `BlurRadius` | `double` | 0-10 | `2.0` | Quantité de flou de l'ombre |
| `Color` | `SKColor` | - | `Black` | Couleur de l'ombre |

**Référence de direction :**

- 0° = Droite
- 90° = Bas
- 180° = Gauche
- 270° = Haut

## Arrière-plans de texte

Les superpositions de texte peuvent avoir diverses formes d'arrière-plan :

### OverlayManagerBackgroundRectangle

```csharp
var text = new OverlayManagerText("Info", 100, 100);
text.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.Black.WithAlpha(128),
    Fill = true,
    Margin = new Rect(5, 3, 5, 3)   // Margin est un VisioForge.Core.Types.Rect (Left/Top/Right/Bottom)
};
```

### OverlayManagerBackgroundSquare

Similaire au rectangle mais maintient un ratio d'aspect carré.

### OverlayManagerBackgroundImage

Utilise une image comme arrière-plan de texte avec modes d'étirement.

### OverlayManagerBackgroundTriangle/Star

Arrière-plans de forme personnalisée pour le texte.

## Paramètres de police

Configurer l'apparence du texte :

```csharp
public class FontSettings
```

| Propriété | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Nom de la famille de polices |
| `Size` | `int` | Taille de police en points |
| `Style` | `FontStyle` | Normal, Italic, Oblique |
| `Weight` | `FontWeight` | Normal, Bold, Light, etc. |

## Exemple complet

```csharp
// Créer le pipeline et les blocs
var pipeline = new MediaBlocksPipeline();
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(videoUri));
var overlayManager = new OverlayManagerBlock();
var videoRenderer = new VideoRendererBlock(pipeline, videoView);

// Connecter le pipeline
pipeline.Connect(fileSource.VideoOutput, overlayManager.Input);
pipeline.Connect(overlayManager.Output, videoRenderer.Input);

// Ajouter un filigrane logo
var logo = new OverlayManagerImage("logo.png", 10, 10);
logo.Opacity = 0.5;
logo.ZIndex = 10; // Au-dessus
overlayManager.Video_Overlay_Add(logo);

// Ajouter un horodatage — positionner contre la hauteur de sortie connue (définie lors de la configuration du bloc gestionnaire de superpositions)
const int frameHeight = 720;
var timestamp = new OverlayManagerDateTime();
timestamp.X = 10;
timestamp.Y = frameHeight - 30;
timestamp.Font.Size = 16;
timestamp.Color = SKColors.White;
timestamp.Shadow = new OverlayManagerShadowSettings(true);
overlayManager.Video_Overlay_Add(timestamp);

// Ajouter un titre animé (apparaît après 5 secondes)
var title = new OverlayManagerText("Bienvenue !", 100, 100);
title.Font.Size = 72;
title.Color = SKColors.Yellow;
title.StartTime = TimeSpan.FromSeconds(5);
title.EndTime = TimeSpan.FromSeconds(10);
title.Rotation = -10; // Légère inclinaison
title.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.DarkBlue,
    Fill = true
};
overlayManager.Video_Overlay_Add(title);

// Démarrer la lecture
await pipeline.StartAsync();

// Mettre à jour les superpositions dynamiquement
title.Text = "Texte mis à jour !";
overlayManager.Video_Overlay_Update(title);
```

## Application exemple

Pour un exemple fonctionnel complet démontrant tous les types de superposition, consultez :

- [Overlay Manager Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Overlay%20Manager%20Demo)

## Considérations de performance

1. **Ordre par Z-index** : les éléments sont triés par Z-index avant le rendu. Utilisez des valeurs appropriées pour minimiser la charge de tri.

2. **Formats d'image** : utilisez des images au format RGBA8888 lorsque possible pour éviter la conversion de couleur.

3. **Effets d'ombre** : les ombres avec flou sont coûteuses en calcul. À utiliser avec parcimonie pour les applications temps réel.

4. **Mises à jour** : utilisez `Video_Overlay_Update()` pour les éléments existants plutôt que des opérations supprimer/ajouter.

5. **Gestion des ressources** : libérez les superpositions d'image, GIF et séquence d'images lorsqu'elles ne sont plus nécessaires pour libérer la mémoire.

6. **Superpositions vidéo** : chaque superposition `OverlayManagerVideo` exécute son propre pipeline GStreamer interne. Limitez le nombre de superpositions vidéo simultanées pour éviter une utilisation excessive du CPU et de la mémoire.

7. **Superpositions de contrôles WPF** : des valeurs `RefreshRate` plus élevées augmentent l'utilisation du CPU. Utilisez la fréquence de rafraîchissement minimale nécessaire pour des mises à jour visuelles fluides — 15 fps est suffisant pour la plupart du contenu statique ou qui change lentement.

8. **Superpositions WebView2** : chaque superposition `OverlayManagerWebView2Video` exécute son propre pipeline de rendu interne avec un navigateur hors écran. Limitez le nombre de superpositions WebView2 simultanées pour éviter une utilisation excessive du CPU, du GPU et de la mémoire.

9. **Groupes de superpositions** : utilisez `OverlayManagerGroup` pour précharger les superpositions vidéo. Cela évite les heures de démarrage échelonnées lorsque plusieurs superpositions vidéo doivent commencer simultanément.

## Notes sur les plateformes

- **Windows** : prend en charge System.Drawing.Bitmap en plus de SkiaSharp
- **Windows (WPF)** : prend en charge `OverlayManagerWPFControl` pour rendre des éléments visuels WPF comme superpositions. Nécessite la cible de build `NET_WINDOWS`.
- **Windows (WebView2)** : prend en charge `OverlayManagerWebView2Video` pour rendre du contenu web en direct (HTML/CSS/JS) comme superpositions. Nécessite le runtime Microsoft WebView2 et le plugin GStreamer WebView2 (`webview2src`).
- **iOS** : la police par défaut est « System-ui »
- **Android** : la police par défaut est « System-ui »
- **Linux/macOS** : énumère les polices disponibles à l'exécution

## Sécurité de thread

Le gestionnaire de superpositions utilise un verrouillage interne pour les opérations thread-safe. Vous pouvez ajouter, supprimer ou mettre à jour des superpositions en toute sécurité depuis n'importe quel thread.

## Dépannage

1. **Superposition non visible** : vérifiez la propriété `Enabled`, `StartTime`/`EndTime` et l'ordre `ZIndex`.

2. **Le texte apparaît flou** : assurez-vous que la taille de police est appropriée pour la résolution vidéo.

3. **Utilisation de la mémoire** : libérez les superpositions image/GIF/séquence d'images inutilisées et utilisez des tailles d'image appropriées.

4. **La superposition vidéo n'affiche aucune image** : assurez-vous que `Initialize()` renvoie `true` avant l'ajout au gestionnaire de superpositions. Vérifiez que le chemin du fichier source est valide et accessible, et que GStreamer dispose des codecs requis.

5. **La superposition WPF ne se met pas à jour** : vérifiez que `RefreshRate` est approprié pour votre contenu. Utilisez `InvokeOnUIThread()` pour toutes les modifications d'éléments WPF afin d'éviter les exceptions inter-threads.

6. **La superposition WebView2 ne s'affiche pas** : assurez-vous que le runtime Microsoft WebView2 est installé sur la machine cible. Vérifiez que `Initialize()` renvoie `true` avant l'ajout au gestionnaire de superpositions. Le plugin GStreamer WebView2 (`webview2src`) doit être disponible.

7. **Les superpositions de groupe ne démarrent pas ensemble** : assurez-vous que toutes les superpositions sont ajoutées au groupe avant d'appeler `Initialize()`. Les superpositions ne peuvent pas être ajoutées après l'initialisation.

## Foire aux questions

### Comment superposer un fichier vidéo sur une autre vidéo en C# ?

Utilisez `OverlayManagerVideo` pour lire un fichier vidéo ou une URL de flux comme superposition. Créez une instance avec le chemin source, la position et les dimensions, puis appelez `Initialize()` et ajoutez-la à l'`OverlayManagerBlock`. Vous obtenez un contrôle complet de la lecture avec les méthodes `Play()`, `Pause()`, `Stop()` et `Seek()`, ainsi qu'une sortie audio optionnelle. Consultez la section [OverlayManagerVideo](#overlaymanagervideo) pour des exemples.

### Puis-je utiliser des contrôles WPF comme superpositions vidéo en direct ?

Oui. `OverlayManagerWPFControl` rend tout `FrameworkElement` WPF comme superposition vidéo, y compris les contrôles avec animations Storyboard, liaison de données et arbres visuels complexes. L'élément est capturé périodiquement à une fréquence configurable (1-60 fps). Ceci est Windows-uniquement et nécessite la cible de build `NET_WINDOWS`. Utilisez la méthode utilitaire `Video_Overlay_AddWPFControl()` pour la configuration la plus simple. Consultez la section [OverlayManagerWPFControl](#overlaymanagerwpfcontrol-windows-uniquement).

### Comment synchroniser plusieurs superpositions vidéo pour qu'elles démarrent en même temps ?

Utilisez `OverlayManagerGroup` pour regrouper les superpositions qui nécessitent un cycle de vie coordonné. Ajoutez toutes les superpositions au groupe avant d'appeler `Initialize()`, qui précharge les superpositions vidéo à l'état PAUSED. Puis appelez `Play()` pour les démarrer toutes simultanément. Ceci est particulièrement utile pour les compositions multi-caméras. Consultez la section [OverlayManagerGroup](#overlaymanagergroup).

### Puis-je lire l'audio d'une superposition de fichier vidéo ?

Oui. Définissez la propriété `AudioOutput` sur `OverlayManagerVideo` à un périphérique de sortie audio avant d'appeler `Initialize()`. Contrôlez le volume avec `AudioOutput_Volume` (0.0-1.0+) et coupez avec `AudioOutput_Mute`. Si `AudioOutput` est null (par défaut), l'audio du fichier vidéo est ignoré.

### Quels types de superposition l'OverlayManagerBlock prend-il en charge ?

L'OverlayManagerBlock prend en charge : texte (`OverlayManagerText`), date/heure (`OverlayManagerDateTime`), texte défilant (`OverlayManagerScrollingText`), images (`OverlayManagerImage`), GIF animés (`OverlayManagerGIF`), séquences d'images (`OverlayManagerImageSequence`), graphiques SVG (`OverlayManagerSVG`), formes (rectangle, cercle, triangle, étoile, ligne), fichiers/URL vidéo (`OverlayManagerVideo`), cartes de capture Decklink (`OverlayManagerDecklinkVideo`), sources réseau NDI (`OverlayManagerNDIVideo`), contenu web WebView2 (`OverlayManagerWebView2Video`, Windows uniquement), contrôles WPF (`OverlayManagerWPFControl`, Windows uniquement), groupes de superpositions (`OverlayManagerGroup`), dessin Cairo personnalisé (`OverlayManagerCallback`) et effets de transformation vidéo (zoom, panoramique, fondu, squeezeback).

### Puis-je rendre du contenu web en direct comme superposition vidéo ?

Oui. `OverlayManagerWebView2Video` rend toute page web (HTML, CSS, JavaScript) comme superposition vidéo à l'aide de Microsoft WebView2. Vous pouvez afficher des tableaux de bord, du contenu web animé, des téléscripteurs ou tout contenu rendu par navigateur. Il prend en charge l'injection JavaScript après navigation pour personnaliser la page affichée. Ceci est Windows-uniquement et nécessite le runtime Microsoft WebView2. Consultez la section [OverlayManagerWebView2Video](#overlaymanagerwebview2video-windows-uniquement).
