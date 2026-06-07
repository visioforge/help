---
title: Accélération vidéo VA-API sous Linux en C# .NET VisioForge
description: Encodage et décodage vidéo accélérés par GPU via VA-API sous Linux avec VisioForge Media Blocks. Ubuntu et Debian pris en charge pour .NET.
sidebar_label: Plateforme Linux
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
primary_api_classes:
  - VAAPIH264DecoderBlock
  - UniversalSourceBlock
  - UniversalSourceSettings
  - VideoRendererBlock
  - VAAPIHEVCDecoderBlock

---

# Blocs plateforme Linux - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Cette section couvre les MediaBlocks spécifiquement optimisés pour les plateformes Linux.

## Blocs disponibles

### Décodeurs matériels VA-API

Linux fournit un décodage vidéo accéléré matériellement via VA-API (Video Acceleration API) :

- **VAAPIH264DecoderBlock** : décodage matériel H.264/AVC
- **VAAPIHEVCDecoderBlock** : décodage matériel H.265/HEVC
- **VAAPIJPEGDecoderBlock** : décodage matériel JPEG
- **VAAPIVC1DecoderBlock** : décodage matériel VC-1

Voir la [documentation des décodeurs vidéo](../VideoDecoders/index.md)

## Exigences de plateforme

- **Linux** : toute distribution Linux moderne
- **VA-API** : libva et le pilote approprié pour votre GPU
- **Matériel** : GPU Intel, AMD ou autre prenant en charge VA-API

## Fonctionnalités

- **Accélération matérielle** : décodage vidéo sur GPU
- **Faible consommation CPU** : déchargement du décodage vers du matériel dédié
- **Large compatibilité** : fonctionne avec les GPU Intel, AMD et autres
- **Efficacité énergétique** : consommation électrique réduite
- **Multiflux** : prise en charge de plusieurs flux HD/4K

## Exemple de code

### Décodage matériel H.264

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// Décodeur matériel VA-API
if (VAAPIH264DecoderBlock.IsAvailable())
{
    var vaapiDecoder = new VAAPIH264DecoderBlock();
    pipeline.Connect(fileSource.VideoOutput, vaapiDecoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(vaapiDecoder.Output, videoRenderer.Input);
}
else
{
    // Solution de repli vers le décodeur logiciel
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
    pipeline.Connect(fileSource.VideoOutput, decoder.Input);
    
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
    pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);
}

await pipeline.StartAsync();
```

### Plusieurs décodeurs matériels

```csharp
var pipeline = new MediaBlocksPipeline();

// Décoder le flux H.264
var h264Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("h264.mp4"));
var h264Decoder = new VAAPIH264DecoderBlock();
pipeline.Connect(h264Source.VideoOutput, h264Decoder.Input);

// Décoder le flux H.265
var h265Source = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("h265.mp4"));
var h265Decoder = new VAAPIHEVCDecoderBlock();
pipeline.Connect(h265Source.VideoOutput, h265Decoder.Input);

// Mélanger les deux flux (exemple)
var mixer = new VideoMixerBlock(mixerSettings);
pipeline.Connect(h264Decoder.Output, mixer.Inputs[0]);
pipeline.Connect(h265Decoder.Output, mixer.Inputs[1]);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

## Installation et configuration

### Installation de VA-API

**Ubuntu/Debian :**
```bash
sudo apt-get install libva2 libva-drm2 vainfo
```

**Fedora/RHEL :**
```bash
sudo dnf install libva libva-utils
```

### Vérification de la prise en charge de VA-API

```bash
vainfo
```

Cette commande affiche les profils et points d'entrée VA-API disponibles.

### Installation du pilote

**GPU Intel :**
```bash
# Ubuntu/Debian
sudo apt-get install intel-media-va-driver

# Fedora/RHEL
sudo dnf install intel-media-driver
```

**GPU AMD :**
```bash
# Ubuntu/Debian
sudo apt-get install mesa-va-drivers

# Fedora/RHEL
sudo dnf install mesa-va-drivers
```

## Vérification de la prise en charge matérielle dans le code

```csharp
// Vérifier si le décodeur VA-API H.264 est disponible
if (VAAPIH264DecoderBlock.IsAvailable())
{
    Console.WriteLine("Le décodage matériel VA-API H.264 est disponible");
    var decoder = new VAAPIH264DecoderBlock();
}
else
{
    Console.WriteLine("VA-API indisponible, utilisation du décodeur logiciel");
    var decoder = new UniversalDecoderBlock(MediaBlockPadMediaType.Video);
}
```

## Conseils de performance

- Assurez-vous que les pilotes VA-API sont correctement installés
- Utilisez les décodeurs matériels lorsque c'est possible pour de meilleures performances
- Vérifiez la disponibilité du décodeur avant de créer des blocs
- Surveillez l'utilisation de la mémoire GPU lors du traitement de plusieurs flux
- Utilisez la commande `vainfo` pour vérifier la prise en charge matérielle

## Dépannage

**VA-API ne fonctionne pas :**
1. Vérifiez l'installation du pilote avec `vainfo`
2. Vérifiez les autorisations utilisateur (vous devrez peut-être appartenir au groupe `video` ou `render`)
3. Assurez-vous que les plugins GStreamer VA-API sont installés : `gstreamer1.0-vaapi`

**Problèmes de permissions :**
```bash
# Ajouter l'utilisateur au groupe video
sudo usermod -a -G video $USER
# Déconnectez-vous puis reconnectez-vous pour appliquer les changements
```

## Plateformes

Linux (Ubuntu, Debian, Fedora, RHEL, Arch et autres distributions).

Nécessite la prise en charge de VA-API avec les pilotes appropriés.

## Documentation connexe

- [VideoDecoders](../VideoDecoders/index.md) - Blocs de décodage matériel
- [Nvidia](../Nvidia/index.md) - Accélération GPU Nvidia (fonctionne aussi sous Linux)
