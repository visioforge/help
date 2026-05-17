---
title: SDK d'effets vidéo pour .NET — superpositions et traitement
description: Implémentez des effets vidéo professionnels, des superpositions texte/image et le traitement vidéo personnalisé avec des outils visuels pour .NET.
sidebar_label: Effets vidéo et traitement

order: 15
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
primary_api_classes:
  - VideoEffect
  - GPUVideoEffect
  - VideoBalanceVideoEffect
  - ColorEffectsVideoEffect
  - GrayscaleVideoEffect

---

# Effets vidéo et traitement pour les applications .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Nos SDK .Net offrent aux développeurs deux implémentations distinctes d'effets vidéo pour répondre à vos exigences de plateforme et de performance :

### Effets classiques (Windows uniquement)
Disponibles dans **VideoCaptureCore**, **MediaPlayerCore** et **VideoEditCore** :
- Effets basés sur le CPU (`VideoEffect*`)
- Effets accélérés par le GPU (`GPUVideoEffect*`) utilisant DirectX
- Effets boostés par IA (`Maxine*`) exploitant la technologie NVIDIA
- Windows uniquement, optimisés pour les performances bureau

### Effets multiplateformes
Disponibles dans **VideoCaptureCoreX**, **MediaPlayerCoreX**, **VideoEditCoreX** et le **Media Blocks SDK** :
- Implémentation multiplateforme (classes `*VideoEffect`)
- Fonctionne sur Windows, Linux, macOS, Android et iOS
- Accélération matérielle via les plugins multimédias spécifiques à la plateforme
- Système de superposition étendu et fonctionnalités avancées
- Mieux adapté au déploiement mobile et multiplateforme

## Référence complète des effets

**[Consulter la référence complète des effets vidéo →](effects-reference.md)**

Notre référence exhaustive fournit des informations détaillées sur les 100+ effets vidéo disponibles dans les deux implémentations :
- **Effets classiques** pour les applications Windows uniquement
- **Effets multiplateformes** pour une compatibilité universelle
- Paramètres des effets, exemples d'utilisation et disponibilité par SDK
- Fonctionnalités et capacités spécifiques à chaque plateforme

## Catégories d'effets vidéo disponibles

Les catégories d'effets suivantes sont disponibles dans les implémentations Classique et Multiplateforme (le cas échéant) :

### Ajustement de couleurs et d'image

* **Luminosité, obscurité, contraste** — contrôle de la luminance et de la plage tonale
  - Classique : `VideoEffect*` / `GPUVideoEffect*`
  - Multiplateforme : `VideoBalanceVideoEffect`, `GammaVideoEffect`
* **Saturation et étalonnage colorimétrique** — ajustement de l'intensité des couleurs et corrections colorimétriques
  - Classique : `VideoEffectSaturation` / `GPUVideoEffectSaturation`
  - Multiplateforme : `VideoBalanceVideoEffect`, `ColorEffectsVideoEffect`, `LUTVideoEffect`
* **Clarté** — ajustement de la luminosité basé sur HSL préservant les relations de couleurs
  - Classique : `VideoEffectLightness`
* **Filtres de couleur** — isoler ou rehausser des canaux de couleur spécifiques (Rouge, Vert, Bleu)
  - Classique : `VideoEffectRed/Green/Blue`, `VideoEffectFilterRed/Green/Blue`

### Effets créatifs et artistiques

* **Niveaux de gris et monochrome** — conversions noir et blanc avec teintage optionnel
  - Classique : `VideoEffectGrayscale` / `GPUVideoEffectGrayscale`
  - Multiplateforme : `GrayscaleVideoEffect`, `ColorEffectsVideoEffect`
* **Inversion et solarisation** — effets de négatif photographique et d'inversion partielle
  - Classique : `VideoEffectInvert` / `GPUVideoEffectInvert`, `VideoEffectSolorize`
* **Vieux film et vision nocturne** — simulations de pellicule ancienne et de caméra de surveillance
  - Classique : `GPUVideoEffectOldMovie`, `GPUVideoEffectNightVision`
  - Multiplateforme : `AgingVideoEffect`, `OpticalAnimationBWVideoEffect`
* **Gaufrage et contour** — détection de contours et effets en relief
  - Classique : `GPUVideoEffectEmboss`, `GPUVideoEffectContour`
  - Multiplateforme : `EdgeVideoEffect`
* **Pixellisation et mosaïque** — effets stylisés en blocs et tuiles
  - Classique : `GPUVideoEffectPixelate`, `VideoEffectMosaic`
  - Multiplateforme : effets de distorsion variés

### Amélioration d'image

* **Netteté** — accentuation de la définition des contours et des fins détails
  - Classique : `VideoEffectSharpen` / `GPUVideoEffectSharpen`
* **Flou et lissage** — réduction du détail, du bruit et création d'effets de mise au point douce
  - Classique : `VideoEffectBlur` / `GPUVideoEffectBlur`, `VideoEffectSmooth`
  - Multiplateforme : `GaussianBlurVideoEffect`, `SmoothVideoEffect`
* **Débruitage** — algorithmes de suppression du bruit adaptatif, CAST et moustique
  - Classique : `VideoEffectDenoiseAdaptive/CAST/Mosquito` / `GPUVideoEffectDenoise`
* **Désentrelacement** — conversion vidéo entrelacée vers progressive (méthodes Blend, CAVT, Triangle)
  - Classique : `VideoEffectDeinterlaceBlend/CAVT/Triangle` / `GPUVideoEffectDeinterlaceBlend`
  - Multiplateforme : `DeinterlaceVideoEffect`, `AutoDeinterlaceSettings`

### Transformations géométriques

* **Rotation** — pivotement à n'importe quel angle avec options d'étirement ou sans rognage
  - Classique : `VideoEffectRotate`
  - Multiplateforme : `FlipRotateVideoEffect`
* **Retournement et miroir** — retournement/miroir horizontal et vertical
  - Classique : `VideoEffectFlip*`, `VideoEffectMirror*`
  - Multiplateforme : `FlipRotateVideoEffect`
* **Zoom et panoramique** — focalisation sur des régions spécifiques avec contrôle de mise à l'échelle
  - Classique : `VideoEffectZoom`, `VideoEffectPan`
  - Multiplateforme : `ResizeVideoEffect`, `CropVideoEffect`, `AspectRatioCropVideoEffect`, `BoxVideoEffect`
* **Traitement vidéo 360°** — projections équirectangulaires et cubemap pour la vidéo VR
  - Classique : `GPUVideoEffectEquirectangular360`, `GPUVideoEffectEquiangularCubemap360`

### Effets de distorsion artistique (multiplateforme uniquement)

L'implémentation multiplateforme inclut de nombreux effets de distorsion artistique absents de la version Classique :

* **Effets d'objectif** — œil-de-poisson, sphère, tournoiement, renflement, pincement
* **Effets de motif** — kaléidoscope, marbre, quark, dés
* **Effets de mouvement** — flou de mouvement, écho de mouvement, écho de zoom en mouvement
* **Effets de déformation** — ondulation, ondulation aquatique, déformation, étirement, tunnel
* **Effets de perspective** — Pseudo3D, carré, cercle, perspective
* **Transitions SMPTE** — volets et transitions de style diffusion professionnelle

### Effets de transition

* **Fondu en entrée/sortie** — transitions douces vers/depuis le noir
  - Classique : `VideoEffectFadeIn`, `VideoEffectFadeOut`
* **Transitions animées** — interpolation temporelle de la valeur d'effet pour des changements dynamiques
  - Classique : pris en charge via le paramètre ValueStop
  - Multiplateforme : temporel avec StartTime/StopTime

## Capacités de superposition

* [**Superposition de texte**](text-overlay.md) — ajoutez du texte personnalisable avec contrôle de la police, taille, couleur, rotation et animation
  - Classique : `VideoEffectTextLogo`, `VideoEffectScrollingTextLogo`
  - Multiplateforme : `TextOverlayVideoEffect`, `OverlayManagerText`, `OverlayManagerScrollingText`, `OverlayManagerDateTime`
* [**Superposition d'image**](image-overlay.md) — intégrez logos, filigranes et éléments graphiques avec prise en charge de la transparence
  - Classique : `VideoEffectImageLogo`
  - Multiplateforme : `ImageOverlayVideoEffect`, `ImageOverlayCairoVideoEffect`, `OverlayManagerImage`, `OverlayManagerGIF`
  * Prise en charge des formats PNG, JPG, BMP, GIF animés
  * Prise en charge de la transparence et du canal alpha
  * Contrôle du positionnement et de l'alignement

### Fonctionnalités de superposition avancées (multiplateforme uniquement)

L'implémentation multiplateforme inclut un système OverlayManager exhaustif :

* **Superpositions vidéo** — image dans l'image, sources vidéo Decklink, NDI
* **Superpositions de formes** — cercles, rectangles, étoiles, triangles, lignes
* **Couches d'arrière-plan** — images, couleurs unies, formes géométriques
* **Animations** — effets de panoramique, zoom, fondu, squeezeback
* **Gestion de groupe** — contrôle synchronisé de plusieurs superpositions
* **Prise en charge SVG** — superpositions graphiques vectorielles
* **QR codes** — génération et superposition de QR codes

## Incrustation par chrominance et détection de mouvement (multiplateforme uniquement)

* **ChromaKeySettings** — incrustation écran vert / écran bleu pour la suppression d'arrière-plan
* **MotionDetectionProcessor** — détection de mouvement en temps réel avec sensibilité configurable

## Amélioration vidéo boostée par IA (Classique uniquement)

Effets IA NVIDIA Maxine (nécessite un GPU NVIDIA RTX prenant en charge le SDK Maxine) :

* **Débruitage IA** — réduction intelligente du bruit préservant les fins détails
* **Réduction d'artefacts** — suppression des artefacts de compression et de la dégradation vidéo
* **Super-résolution** — surdimensionnement IA pour résolution accrue
* **Écran vert IA** — suppression d'arrière-plan sans véritable écran vert physique
* **Surdimensionnement** — amélioration avancée de la qualité par IA

## Fonctionnalités de traitement vidéo
  
### Traitement en temps réel

* Appliquez des effets durant la capture, la lecture ou l'édition vidéo
* Chaînez plusieurs effets pour des pipelines de traitement complexes
* Accélération GPU pour traitement haute résolution en temps réel (Classique)
* Accélération matérielle pour performances multiplateformes (multiplateforme)
* Solution de repli CPU pour compatibilité universelle

### Traitement avancé

* Application d'effets sur une chronologie (heures de début/fin)
* Transitions d'effets animées (interpolation entre valeurs — Classique)
* Contrôle dynamique des effets pendant la lecture
* [**Capteur d'échantillons vidéo**](video-sample-grabber.md) — extraire les images et traiter les données vidéo en temps réel

## Optimisation des performances

### Effets classiques (Windows uniquement)

- **Effets GPU** (`GPUVideoEffect*`) : meilleures performances pour vidéo haute résolution sur Windows
- **Effets CPU** (`VideoEffect*`) : compatibilité Windows universelle, performances modérées
- **Effets IA** (`Maxine*`) : qualité de pointe, nécessite un GPU NVIDIA RTX
- Utilisez les effets accélérés par GPU pour le traitement HD/4K en temps réel
- Accélération matérielle DirectX pour des performances Windows optimales

### Effets multiplateformes (Multiplateforme)

- **Accélération matérielle** : varie selon la plateforme via les plugins multimédias
  - Windows : DirectX, DXVA
  - Linux : VA-API, VDPAU, OpenGL
  - macOS : VideoToolbox, Metal
  - Android/iOS : codecs matériels
- **Performances** : généralement bonnes sur toutes les plateformes
- **Optimisation mobile** : mieux adapté au mobile que les effets Classiques
- **Système de superposition** : rendu multi-superposition efficace

### Bonnes pratiques

* Choisissez les effets GPU/accélérés pour les vidéos haute résolution
* Tenez compte des combinaisons d'effets et de leur impact sur les performances
* Appliquez des effets limités dans le temps lorsque c'est approprié pour réduire la charge de traitement
* Testez les performances sur les configurations matérielles cibles
* Utilisez les effets multiplateformes pour le déploiement multiplateforme

## Prise en charge des plateformes

### Effets classiques

| Plateforme | Effets CPU | Effets GPU | IA Maxine | SDK |
|----------|-------------|-------------|-----------|-----|
| Windows Desktop | ✅ Complet | ✅ Complet | ✅ GPU RTX | VideoCaptureCore<br/>MediaPlayerCore<br/>VideoEditCore |
| Linux    | ❌ | ❌ | ❌ | N/A |
| macOS    | ❌ | ❌ | ❌ | N/A |
| Android  | ❌ | ❌ | ❌ | N/A |
| iOS      | ❌ | ❌ | ❌ | N/A |

### Effets multiplateformes

| Plateforme | Prise en charge | Accélération matérielle | SDK |
|----------|---------|---------------------|-----|
| Windows  | ✅ Complet | ✅ DirectX, DXVA | VideoCaptureCoreX<br/>MediaPlayerCoreX<br/>VideoEditCoreX<br/>Media Blocks SDK |
| Linux    | ✅ Complet | ✅ VA-API, VDPAU, OpenGL | ✓ |
| macOS    | ✅ Complet | ✅ VideoToolbox, Metal | ✓ |
| Android  | ✅ Complet | ✅ Codecs matériels | ✓ |
| iOS      | ✅ Complet | ✅ VideoToolbox | ✓ |

## Méthodes d'intégration

Nos SDK fournissent deux API distinctes pour l'intégration des effets vidéo selon le moteur SDK que vous utilisez.

### Exemple d'utilisation basique (effets classiques)

```csharp
// Windows uniquement : effet basé sur le CPU
var bwEffect = new VideoEffectGrayscale(
    enabled: true,
    name: "BlackAndWhite"
);
videoCapture.Video_Effects_Add(bwEffect);

// Windows uniquement : effet accéléré par le GPU
var brighten = new GPUVideoEffectBrightness(
    enabled: true,
    value: 180,
    name: "Brighten"
);
videoCapture.Video_Effects_Add(brighten);
```

### Exemple d'utilisation basique (effets multiplateformes)

```csharp
// Multiplateforme : effet niveaux de gris
var grayscale = new GrayscaleVideoEffect("bw_effect");
await videoCaptureX.Video_Effects_AddOrUpdateAsync(grayscale);

// Multiplateforme : balance vidéo (luminosité, contraste, saturation)
var balance = new VideoBalanceVideoEffect("color_adjust")
{
    Brightness = 0.2,    // Plage : -1.0 à 1.0
    Contrast = 1.2,      // Plage : 0.0 à 2.0
    Saturation = 1.5,    // Plage : 0.0 à 2.0
    Hue = 0.0            // Plage : -1.0 à 1.0
};
await videoCaptureX.Video_Effects_AddOrUpdateAsync(balance);
```

### Exemple d'effet animé (Classique)

```csharp
// Fondu au noir sur 3 secondes (Windows uniquement)
var fadeOut = new VideoEffectDarkness(
    enabled: true,
    value: 128,        // Démarrer à la normale
    valueStop: 255,    // Finir au noir
    name: "FadeOut",
    startTime: TimeSpan.FromSeconds(57),
    stopTime: TimeSpan.FromSeconds(60)
);
mediaPlayer.Video_Effects_Add(fadeOut);
```

### Exemple d'effet limité dans le temps (multiplateforme)

```csharp
// Appliquer un flou gaussien sur une plage temporelle spécifique (multiplateforme)
// Ctor : GaussianBlurVideoEffect(double sigma, string name = "gaussian_blur")
var blur = new GaussianBlurVideoEffect(5.0, "scene_blur")
{
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromSeconds(15)
};
await mediaPlayerX.Video_Effects_AddOrUpdateAsync(blur);
```

### Exemple de superposition avancée (multiplateforme)

```csharp
// Superposition de texte complexe avec ombre portée (multiplateforme)
var textOverlay = new OverlayManagerText
{
    Text = "Breaking News",
    Font = new FontSettings
    {
        Name = "Arial",
        Size = 48,
        Weight = FontWeight.Bold
    },
    Color = SKColors.Yellow,
    X = 50,
    Y = 100,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        BlurRadius = 5
    }
};
videoCaptureX.Video_Overlay_Add(textOverlay);
```

## Ressources de documentation

* **[Référence complète des effets](effects-reference.md)** — guide exhaustif de tous les effets disponibles
* **[Guide de superposition de texte](text-overlay.md)** — implémentation détaillée des superpositions de texte
* **[Guide de superposition d'image](image-overlay.md)** — techniques de superposition d'images et de logos
* **[Capteur d'échantillons vidéo](video-sample-grabber.md)** — extraction et traitement d'images
* **[Comment ajouter des effets](add.md)** — guide général d'application des effets

## Plus d'informations

De nombreux effets vidéo et fonctionnalités de traitement supplémentaires sont disponibles dans les SDK. Veuillez consulter la [Référence complète des effets](effects-reference.md) pour des informations détaillées sur tous les effets, ou visitez la documentation du SDK spécifique que vous utilisez pour les exemples d'implémentation et les références d'API.
  
---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour accéder à plus d'exemples de code et d'implémentation.
