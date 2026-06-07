---
title: Mixage vidéo et mise en page image dans l'image en C# .NET
description: Implémentez l'image dans l'image avec plusieurs sources vidéo en .NET avec des exemples C# pour mixer des flux avec des mises en page personnalisées.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Mixing
  - IP Camera
  - Screen Capture
  - MP4
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - ScreenCaptureSourceSettings
  - VideoMixerSourceSettings
  - IPCameraSourceSettings
  - VideoCaptureDeviceSourceSettings

---

# Mixage de plusieurs flux vidéo dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au mixage de flux vidéo

Le SDK offre de puissantes capacités pour mixer plusieurs flux vidéo avec des résolutions et des fréquences d'images différentes. Cette fonctionnalité est essentielle pour créer des applications vidéo professionnelles nécessitant des effets d'image dans l'image, des vues multi-caméras ou des sorties d'affichage combinées.

## Fonctionnalités clés du mixage vidéo

- Combiner plusieurs sources vidéo dans une seule sortie
- Prise en charge de différentes résolutions vidéo et fréquences d'images
- Plusieurs options de mise en page (horizontale, verticale, grille, personnalisée)
- Ajustements de position et de paramètres en temps réel
- Contrôles de transparence et de retournement
- Prise en charge de sources d'entrée variées (caméras, écrans, caméras IP)

## Moteur VideoCaptureCore

Le processus d'implémentation comprend trois étapes principales :

1. Configuration du mode de mixage vidéo
2. Ajout de plusieurs sources vidéo
3. Configuration et ajustement des paramètres des sources

Explorons chaque étape en détail avec des exemples de code.

### Définition du mode de mixage vidéo

Le SDK prend en charge diverses mises en page prédéfinies ainsi que des configurations personnalisées. Sélectionnez le mode approprié en fonction des exigences de votre application.

#### Mise en page horizontale

Ce mode dispose toutes les sources vidéo horizontalement sur une rangée :

```cs
VideoCapture1.PIP_Mode = PIPMode.Horizontal;
```

#### Mise en page verticale

Ce mode dispose toutes les sources vidéo verticalement en colonne :

```cs
VideoCapture1.PIP_Mode = PIPMode.Vertical;
```

#### Mise en page en grille (2×2)

Ce mode organise jusqu'à quatre sources vidéo dans une grille carrée :

```cs
VideoCapture1.PIP_Mode = PIPMode.Mode2x2;
```

#### Mise en page personnalisée avec résolution spécifique

Pour les scénarios avancés, vous pouvez définir une mise en page personnalisée avec des dimensions de sortie précises :

```cs
VideoCapture1.PIP_Mode = PIPMode.Custom;
VideoCapture1.PIP_CustomOutputSize_Set(1920, 1080);
```

### Ajout de sources vidéo

La première source configurée sert d'entrée principale, tandis que des sources supplémentaires peuvent être ajoutées via l'API PIP.

#### Ajout d'un périphérique de capture vidéo

Pour incorporer une caméra ou un autre périphérique de capture vidéo :

```cs
// Signature : (string deviceName, string format, bool useBestFormat, VideoFrameRate frameRate,
//             string crossbarInput, int left, int top, int width, int height) → bool
VideoCapture1.PIP_Sources_Add_VideoCaptureDevice(
    deviceName:     "USB Camera",                     // nom depuis Video_CaptureDevices()
    format:         "1280x720 30fps",                 // chaîne de format, ou vide + useBestFormat=true
    useBestFormat:  false,
    frameRate:      new VideoFrameRate(30),
    input:          null,                             // nom d'entrée crossbar, ou null
    left: 100, top: 100, width: 320, height: 240);
```

#### Ajout d'une source caméra IP

Pour les caméras basées sur le réseau :

```cs
var ipCameraSource = new IPCameraSourceSettings
{
    URL = new Uri("rtsp://192.168.1.1:554/live") // URL est Uri, pas string
};

// Définir des paramètres supplémentaires de caméra IP si nécessaire
// Authentification, protocole, mise en mémoire tampon, etc.

VideoCapture1.PIP_Sources_Add_IPCamera(
    ipCameraSource,
    left,
    top,
    width,
    height);
```

#### Ajout d'une source de capture d'écran

Pour inclure le bureau ou des fenêtres d'application :

```cs
ScreenCaptureSourceSettings screenSource = new ScreenCaptureSourceSettings();
screenSource.Mode = ScreenCaptureMode.Screen;
screenSource.FullScreen = true;
VideoCapture1.PIP_Sources_Add_ScreenSource(
    screenSource,
    left,
    top,
    width,
    height);
```

### Ajustements dynamiques des sources

Un avantage majeur du SDK est la capacité d'ajuster les paramètres des sources en temps réel pendant le fonctionnement.

#### Repositionnement des sources

Vous pouvez modifier la position et les dimensions de toute source :

```cs
// SetSourcePositionAsync prend (int index, Rectangle rect).
await VideoCapture1.PIP_Sources_SetSourcePositionAsync(
    index,                                 // 0 est la source principale, 1+ sont des sources supplémentaires
    new System.Drawing.Rectangle(left, top, width, height));
```

#### Ajustement des propriétés visuelles

Affinez l'apparence avec des contrôles de transparence et d'orientation :

```cs
int transparency = 127; // Plage : 0-255
bool flipX = false;
bool flipY = false;

// SetSourceSettingsAsync prend (int index, int transparency, bool flipX, bool flipY, bool disabled = false).
await VideoCapture1.PIP_Sources_SetSourceSettingsAsync(
    index,
    transparency,
    flipX,
    flipY);
```

### Dépendances requises

Pour utiliser la fonctionnalité de mixage vidéo, assurez-vous d'inclure les paquets redistribuables appropriés :

- Redistribuables Video Capture :
  - [Paquet x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [Paquet x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Moteur VideoCaptureCoreX

Le moteur VideoCaptureCoreX est une version plus récente du moteur VideoCaptureCore qui offre des fonctionnalités et améliorations supplémentaires.

### Mixage vidéo avec VideoCaptureCoreX

Le moteur VideoCaptureCoreX offre une approche plus flexible et puissante du mixage vidéo via la classe `VideoMixerSourceSettings`. Cela permet un contrôle précis sur plusieurs sources vidéo et leur mise en page.

#### Configuration de base

```cs
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.MediaInfo;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoCapture;

// Créer un mélangeur vidéo avec une résolution 1080p à 30 fps
var videoMixer = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Initialiser le moteur VideoCaptureCoreX
var captureX = new VideoCaptureCoreX();
captureX.Video_Source = videoMixer;
```

#### Ajout de plusieurs sources

Vous pouvez ajouter diverses sources vidéo au mélangeur, en positionnant chacune exactement là où c'est nécessaire :

```cs
// Rect utilise (left, top, right, bottom) — PAS (left, top, width, height). Width et
// Height sont calculés comme Right-Left / Bottom-Top, donc passer width/height ici
// produit des dimensions négatives pour tout rectangle hors origine.

// Ajouter une caméra comme première source (arrière-plan plein écran, toile 1920×1080).
var cameraSource = new VideoCaptureDeviceSourceSettings();

// Configurer les paramètres de la caméra
// ...

videoMixer.Add(cameraSource, new Rect(0, 0, 1920, 1080));

// Ajouter une source de capture d'écran dans le coin supérieur droit, région 640×480.
var screenSource = new ScreenCaptureDX9SourceSettings();
screenSource.CaptureCursor = true;
screenSource.FrameRate = VideoFrameRate.FPS_30;

// left = 1280, top = 0, right = 1280+640 = 1920, bottom = 0+480 = 480.
videoMixer.Add(screenSource, new Rect(1280, 0, 1920, 480));
```

#### Reconfiguration des sources avant Start

`VideoMixerSourceSettings` est de la configuration — ses sources sont figées lors de `StartAsync`. Pour modifier la mise en page, vous mutez la liste des paramètres (via `Get`/`RemoveAt`/`Add`) **avant** de démarrer le pipeline :

```cs
// Inspecter une source à l'index 1 (retourne un Tuple<IVideoMixerSource, Rect, ChromaKeySettingsX>).
var (source, currentRect, chromaKey) = videoMixer.Get(1);

// Échanger son rectangle en supprimant et en réajoutant à la place.
videoMixer.RemoveAt(1);
videoMixer.Add(source, new Rect(0, 0, 1280, 720), chromaKey);
```

Pour de véritables changements de mise en page **à l'exécution** (mettre à jour la position pendant que le pipeline est actif), descendez vers Media Blocks : construisez votre pipeline autour d'un `VideoMixerBlock` et utilisez ses méthodes `Input_Get(Guid)` / `Input_Update(VideoMixerStream)` pour muter la position, la taille, l'alpha ou l'ordre z du flux sans redémarrer. Consultez la [référence Media Blocks video-processing](../../mediablocks/VideoProcessing/index.md) pour l'API `VideoMixerBlock`.

#### Configuration de la sortie

Configurez la destination de sortie pour votre vidéo mixée :

```cs
// Configurer l'enregistrement MP4
captureX.Outputs_Add(new MP4Output("output.mp4")); 

// Démarrer la capture
await captureX.StartAsync();

// Arrêter après un certain temps
await Task.Delay(TimeSpan.FromMinutes(5));
await captureX.StopAsync();
```

### Comparaison avec le moteur VideoCaptureCore

| Fonctionnalité | VideoCaptureCore | VideoCaptureCoreX |
|---------|------------------|-------------------|
| Style d'API | Approche basée sur des méthodes | Orienté objet avec classes de paramètres |
| Flexibilité | Mises en page prédéfinies | Positionnement de source entièrement personnalisable |
| Gestion des sources | API fixe pour ajouter des sources | Ajouter toute source implémentant IVideoMixerSource |
| Performance | Bonne | Améliorée avec pipeline de rendu optimisé |
| Changements en temps réel | Limité | Manipulation complète des sources |

### Dépendances requises

Consultez la page principale [Déploiement](../deployment.md) pour les dernières dépendances.

## Scénarios d'utilisation avancés

Les capacités de mixage vidéo permettent de nombreux scénarios d'application :

- Surveillance de sécurité multi-caméras
- Visioconférence avec partage d'écran
- Diffusion avec superposition de commentateur
- Présentations éducatives avec plusieurs entrées
- Flux de jeux vidéo avec caméra du joueur

## Conseils de dépannage

- Assurez-vous que les ressources matérielles sont suffisantes pour le nombre de flux
- Surveillez l'utilisation du processeur et de la mémoire pendant le fonctionnement
- Envisagez des résolutions plus basses pour des performances plus fluides avec de nombreuses sources
- Testez différentes configurations de mise en page pour des résultats visuels optimaux

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour des exemples de code supplémentaires et des exemples d'implémentation.
