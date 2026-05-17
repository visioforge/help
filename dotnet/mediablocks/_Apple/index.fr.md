---
title: Blocs Media plateforme Apple en C# .NET — iOS, macOS
description: Applications multimédias iOS et macOS avec encodage ProRes, accélération VideoToolbox et blocs audio natifs grâce à VisioForge Media Blocks.
sidebar_label: Plateforme Apple
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - MetalVideoCompositorBlock
  - MediaBlocksPipeline
  - VideoRendererBlock
  - OSXAudioSourceBlock
  - OSXAudioSinkBlock

---

# Blocs plateforme Apple - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette section couvre les MediaBlocks spécifiquement optimisés pour les plateformes Apple (iOS, macOS, tvOS).

## Blocs disponibles

### Sources audio

- **OSXAudioSourceBlock** : capture audio macOS via Core Audio
  - Voir la [documentation des sources audio](../Sources/index.md#system-audio-source)
  
- **IOSAudioSourceBlock** : capture audio iOS
  - Voir la [documentation des sources audio](../Sources/index.md#system-audio-source)

### Puits audio

- **OSXAudioSinkBlock** : lecture audio macOS
  - Voir la [documentation du rendu audio](../AudioRendering/index.md)
  
- **IOSAudioSinkBlock** : lecture audio iOS
  - Voir la [documentation du rendu audio](../AudioRendering/index.md)

### Sources vidéo

- **IOSVideoSourceBlock** : capture caméra iOS
  - Voir la [documentation des sources vidéo](../Sources/index.md#system-video-source)

### Encodeurs vidéo

- **AppleProResEncoderBlock** : codec vidéo professionnel Apple ProRes
  - Voir la [documentation de l'encodeur ProRes](../VideoEncoders/index.md#apple-prores-encoder)

### Traitement vidéo

- **MetalVideoCompositorBlock** : compositeur vidéo multi-entrées accéléré par GPU utilisant Apple Metal

## Compositeur vidéo Metal

### Bloc Metal Video Compositor

Le `MetalVideoCompositorBlock` compose plusieurs flux vidéo en temps réel à l'aide du framework GPU Apple Metal. Chaque flux d'entrée dispose d'une position, d'une taille, d'un z-order, d'un alpha et d'un opérateur de mélange configurables. Le bloc produit une sortie vidéo BGRA unique.

#### Informations sur le bloc

Nom : MetalVideoCompositorBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Entrée vidéo | Vidéo non compressée | N (un par flux) |
| Sortie vidéo | Vidéo non compressée | 1 |

#### Paramètres

Le bloc prend une instance `MetalVideoCompositorSettings` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Width` | `int` | 1920 | Largeur de sortie en pixels |
| `Height` | `int` | 1080 | Hauteur de sortie en pixels |
| `FrameRate` | `VideoFrameRate` | FPS_30 | Fréquence d'images de sortie |
| `Background` | `VideoMixerBackground` | Transparent | Mode d'arrière-plan |
| `Streams` | `List<VideoMixerStream>` | Vide | Configurations des flux d'entrée |

Chaque flux d'entrée est un `MetalVideoMixerStream` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Rectangle` | `Rect` | requis | Position et taille dans l'image de sortie |
| `ZOrder` | `uint` | requis | Ordre d'empilement (plus élevé = au premier plan) |
| `Alpha` | `double` | 1.0 | Opacité (0.0 transparent – 1.0 opaque) |
| `BlendOperator` | `MetalVideoMixerBlendOperator` | Over | Mode de mélange : Source, Over ou Add |
| `KeepAspectRatio` | `bool` | false | Préserver le rapport d'aspect source lors de la mise à l'échelle |

#### Pipeline d'exemple

```mermaid
graph LR;
    VideoSource1-->MetalVideoCompositorBlock;
    VideoSource2-->MetalVideoCompositorBlock;
    MetalVideoCompositorBlock-->VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configurer le compositeur : 1920x1080 à 30 i/s
var settings = new MetalVideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);

// Premier flux : moitié gauche de l'écran
settings.AddStream(new MetalVideoMixerStream(
    rectangle: new Rect(0, 0, 960, 1080),
    zorder: 0));

// Deuxième flux : moitié droite de l'écran
// Le constructeur de Rect est (left, top, right, bottom). Pour la moitié droite
// d'un canevas 1920x1080, utilisez right=1920 et bottom=1080 — la forme
// précédente (960, 0, 960, 1080) a left==right et produit une boîte de largeur nulle.
settings.AddStream(new MetalVideoMixerStream(
    rectangle: new Rect(960, 0, 1920, 1080),
    zorder: 1));

var compositor = new MetalVideoCompositorBlock(settings);

// Effectuer le rendu de la sortie composée
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(compositor.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Temps réel : faire disparaître progressivement le flux 0 sur 2 secondes
compositor.StartFadeOut(settings.Streams[0].ID, TimeSpan.FromSeconds(2));
```

#### Disponibilité

```csharp
bool available = MetalVideoCompositorBlock.IsAvailable();
```

Renvoie `true` si le plugin GStreamer `vfmetalcompositor` est disponible sur le système courant.

#### Plateformes

macOS, iOS.

## Exigences de plateforme

- **iOS** : iOS 12.0 ou ultérieur
- **macOS** : macOS 10.13 ou ultérieur
- **tvOS** : tvOS 12.0 ou ultérieur

## Fonctionnalités

- Intégration native avec les frameworks Apple (AVFoundation, Core Audio, Core Video)
- Traitement accéléré matériellement sur Apple Silicon et Mac Intel
- Optimisé pour une faible consommation d'énergie sur les appareils mobiles
- Prise en charge de l'encodage ProRes haute qualité
- Intégration avec les autorisations caméra et microphone iOS

## Exemple de code

### Capture caméra iOS

```csharp
var pipeline = new MediaBlocksPipeline();

// Source vidéo iOS
var videoSource = new IOSVideoSourceBlock(videoSettings);

// Traiter et afficher
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Capture et lecture audio macOS

```csharp
var pipeline = new MediaBlocksPipeline();

// Source audio macOS
var audioSource = new OSXAudioSourceBlock(audioSettings);

// Puits audio macOS
var audioSink = new OSXAudioSinkBlock();
pipeline.Connect(audioSource.Output, audioSink.Input);

await pipeline.StartAsync();
```

### Encodage ProRes

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("input.mp4")));

// Encodeur Apple ProRes
// AppleProResEncoderSettings expose Quality (double 0.0-1.0), Bitrate, MaxKeyframeInterval,
// MaxKeyFrameIntervalDuration, AllowFrameReordering, PreserveAlpha, Realtime — pas une énumération de profils nommée.
var proresSettings = new AppleProResEncoderSettings
{
    Quality = 0.8
};
var proresEncoder = new AppleProResEncoderBlock(proresSettings);
pipeline.Connect(fileSource.VideoOutput, proresEncoder.Input);

// Sortie vers un fichier MOV
var movSink = new MOVSinkBlock(new MOVSinkSettings("output.mov"));
pipeline.Connect(proresEncoder.Output, movSink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

## Plateformes

iOS, macOS, tvOS.

## Documentation connexe

- [Sources](../Sources/index.md) - Tous les blocs source, y compris ceux spécifiques à Apple
- [VideoEncoders](../VideoEncoders/index.md) - Encodage vidéo dont ProRes
- [AudioRendering](../AudioRendering/index.md) - Lecture audio
