---
title: Décodage et effets vidéo GPU Direct3D 11 en C# .NET
description: Décodage vidéo accéléré Direct3D 11, effets et intégration DirectShow sous Windows avec VisioForge Media Blocks pour .NET.
sidebar_label: Plateforme Windows
tags:
  - Media Blocks SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - D3D11H264DecoderBlock
  - D3D11UploadBlock
  - D3D11DownloadBlock
  - UniversalSourceBlock
  - UniversalSourceSettings

---

# Blocs plateforme Windows - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette section couvre les MediaBlocks spécifiquement optimisés pour les plateformes Windows.

## Blocs disponibles

### Décodeurs matériels Direct3D 11

Windows fournit un décodage vidéo accéléré matériellement via Direct3D 11 :

- **D3D11H264DecoderBlock** : décodage matériel H.264/AVC
- **D3D11H265DecoderBlock** : décodage matériel H.265/HEVC
- **D3D11VP8DecoderBlock** : décodage matériel VP8
- **D3D11VP9DecoderBlock** : décodage matériel VP9
- **D3D11AV1DecoderBlock** : décodage matériel AV1
- **D3D11MPEG2DecoderBlock** : décodage matériel MPEG-2

Voir la [documentation des décodeurs vidéo](../VideoDecoders/index.md)

### Traitement Direct3D 11

- **D3D11UploadBlock** : téléverser la vidéo depuis la mémoire système vers le GPU
- **D3D11DownloadBlock** : récupérer la vidéo du GPU vers la mémoire système
- **D3D11VideoConverterBlock** : conversion d'espace colorimétrique accélérée par GPU

Voir la [documentation du traitement vidéo](../VideoProcessing/index.md#d3d11-video-converter)

### Composition Direct3D 11

- **D3D11VideoCompositorBlock** : composition et mélange vidéo accélérés par GPU

### Effets vidéo Windows

- **VideoEffectsWinBlock** : effets vidéo spécifiques à Windows
- **VR360ProcessorBlock** : traitement vidéo VR 360 degrés

### Blocs spéciaux

Voir la [documentation des blocs spéciaux](../Special/index.md)

## Exigences de plateforme

- **Windows** : Windows 7 SP1 ou ultérieur
- **Direct3D 11** : GPU prenant en charge D3D11
- **Décodage matériel** : GPU prenant en charge le décodage vidéo matériel

## Fonctionnalités

- **Accélération matérielle** : exploitation du GPU pour l'encodage, le décodage et le traitement
- **Intégration Direct3D 11** : traitement vidéo efficace sur GPU
- **Faible consommation CPU** : déchargement du traitement vers du matériel dédié
- **Hautes performances** : prise en charge simultanée de plusieurs flux HD/4K
- **Efficacité énergétique** : réduction de la consommation grâce à l'accélération matérielle

## Exemple de code

### Décodage matériel H.264

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// Décodeur matériel D3D11
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Decoder.Input);

// Traiter sur le GPU ou récupérer en mémoire système
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Decoder.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Pipeline de traitement vidéo GPU

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video.mp4")));

// Téléverser vers le GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// Conversion colorimétrique GPU
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Récupérer depuis le GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Composition vidéo

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video1.mp4")));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("video2.mp4")));

// Téléverser les deux vers le GPU
var upload1 = new D3D11UploadBlock();
var upload2 = new D3D11UploadBlock();
pipeline.Connect(source1.VideoOutput, upload1.Input);
pipeline.Connect(source2.VideoOutput, upload2.Input);

// Configurer le compositeur — deux flux sur un canevas 1920x1080 à 30 i/s
var compositorSettings = new D3D11VideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_30);
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(0, 0, 960, 1080), zorder: 0));
compositorSettings.Streams.Add(new VideoMixerStream(new Rect(960, 0, 1920, 1080), zorder: 1));

// Composer sur le GPU — chaque flux ajouté crée un pad d'entrée ; on y accède via Inputs[].
var compositor = new D3D11VideoCompositorBlock(compositorSettings);
pipeline.Connect(upload1.Output, compositor.Inputs[0]);
pipeline.Connect(upload2.Output, compositor.Inputs[1]);

// Récupérer le résultat
var download = new D3D11DownloadBlock();
pipeline.Connect(compositor.Output, download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Conseils de performance

- Conservez la vidéo en mémoire GPU entre les opérations pour éviter la surcharge des téléversements/récupérations
- Utilisez les décodeurs matériels lorsque c'est possible pour de meilleures performances
- Chaînez les opérations GPU avant de revenir en mémoire système
- Surveillez l'utilisation de la mémoire GPU lors du traitement de plusieurs flux
- Vérifiez la prise en charge matérielle avant d'utiliser les blocs D3D11

## Vérification de la prise en charge matérielle

```csharp
// Vérifier si le décodeur D3D11 H.264 est disponible
if (D3D11H264DecoderBlock.IsAvailable())
{
    // Utiliser le décodeur matériel
    var decoder = new D3D11H264DecoderBlock();
}
else
{
    // Solution de repli vers le décodeur logiciel
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Plateformes

Windows 7 SP1 ou ultérieur (Windows 10/11 recommandé pour la meilleure prise en charge matérielle).

## Documentation connexe

- [VideoDecoders](../VideoDecoders/index.md) - Blocs de décodage matériel
- [VideoProcessing](../VideoProcessing/index.md) - Blocs de traitement GPU
- [Special](../Special/index.md) - Utilitaires spécifiques à la plateforme
