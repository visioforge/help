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
  - D3D12InterlaceBlock
  - D3D12InterlaceSettings
  - D3D12FisheyeDewarpBlock
  - D3D12FisheyeDewarpSettings
  - D3D12RemapBlock
  - D3D12RemapSettings
  - D3D12UVRemapMap
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

### Traitement Direct3D 12

- **D3D12InterlaceBlock** : conversion sur GPU d'une vidéo progressive en vidéo entrelacée, pour la diffusion et les autres sorties qui exigent un ordre de trames explicite

C'est l'inverse du désentrelacement (`DeinterlaceBlock`) : l'entrée doit être progressive, et la sortie est une vidéo entrelacée (interleaved). `D3D12InterlaceSettings` définit :

- `FieldPattern` : `OneToOne` fusionne deux images progressives en une image entrelacée et divise la fréquence d'images par deux (60p vers 60i, 30 images par seconde) ; `TwoToTwo` scinde chaque image progressive en deux trames et conserve la fréquence d'images (30p vers 60i, 30 images par seconde)
- `TopFieldFirst` : trame supérieure en premier si `true`, trame inférieure en premier si `false` (valeur par défaut)
- `Engine` : le moteur GPU (`Auto`, `ThreeD`, `Compute`)
- `AdapterIndex` : l'adaptateur GPU, `-1` pour l'adaptateur par défaut

Le bloc transfère lui-même les images vers le GPU et depuis celui-ci : son entrée comme sa sortie acceptent la mémoire système, D3D11 ou D3D12, selon le bloc voisin, et la mémoire D3D12 de l'adaptateur GPU du bloc est transmise sans copie en entrée comme en sortie. Il nécessite un GPU compatible Direct3D 12. Le consommateur en aval doit accepter la vidéo entrelacée (par exemple, l'encodeur MPEG-2) : un consommateur qui n'accepte que la vidéo progressive ne peut pas être lié au bloc, et le pipeline signale l'échec de la liaison via `OnError`. Un adaptateur GPU inexistant fait échouer le démarrage du pipeline.

- **D3D12FisheyeDewarpBlock** : redressement sur GPU d'une image fisheye circulaire en vue équirectangulaire, panoramique ou en perspective

`D3D12FisheyeDewarpSettings` définit :

- `Projection` : `Equirectangular` (valeur par défaut), `Panorama`, `Perspective` ou `Passthrough`
- `CenterX`, `CenterY`, `RadiusX`, `RadiusY` : le cercle fisheye dans l'image d'entrée, normalisé de 0 à 1, le rayon au moins égal à 1e-6 (par défaut : centré, rayon 0,5)
- `FisheyeFOV` : le champ de vision de l'objectif en degrés (180 par défaut)
- `HorizontalFOV`, `VerticalFOV` : le champ de vision de la sortie en degrés (90 par défaut) ; non utilisés par `Panorama`. L'élément réduit les angles modulo 360 ; une fois réduit, un angle doit être compris entre 0,001 et 359,999 degrés, et ne pas dépasser 179,999 pour `Perspective`. `FisheyeFOV` suit la plage de 0,001 à 359,999 dans toutes les projections. Une image `Equirectangular` couvre `HorizontalFOV` sur `VerticalFOV` : une image large prend par exemple 300 sur 180
- `RotationX` (tangage), `RotationY` (lacet), `RotationZ` (roulis) en degrés, avec `RotationOrder` et `RotationSpace` (`Local`, relatif à la caméra, ou `World`, axes fixes) ; seul le roulis s'applique à `Panorama`
- `InnerRadius` : la partie centrale retirée d'un `Panorama`, normalisée (0,3 par défaut)
- `RoiX`, `RoiY`, `RoiWidth`, `RoiHeight` : la région d'intérêt dans l'espace de l'image de sortie, normalisée, largeur et hauteur au moins égales à 1e-6 ; l'élément ne la borne pas. Au-delà de l'image horizontalement, elle fait tourner la vue (un panorama franchit sa jointure) ; verticalement, un panorama affiche des lignes noires et une image équirectangulaire se replie au-delà du pôle
- `ViewportX`, `ViewportY`, `ViewportWidth`, `ViewportHeight` : l'emplacement du résultat dans l'image de sortie, normalisé, largeur et hauteur au moins égales à 1e-6
- `AdapterIndex` : l'adaptateur GPU, `-1` pour l'adaptateur par défaut

Tous les réglages sauf `AdapterIndex` peuvent être modifiés pendant l'exécution du pipeline : modifiez `Settings` puis appelez `Update()`, qui renvoie `false` lorsqu'une valeur est hors plage et, une fois le bloc dans un pipeline, signale la cause via `OnError`. Une seule caméra fisheye devient ainsi une caméra PTZ virtuelle. La sortie conserve la taille, le format et la fréquence d'images de l'entrée ; placez un `VideoResizeBlock` après le bloc pour une autre taille, par exemple 2:1 pour une image équirectangulaire. Comme le bloc d'entrelacement, il transfère lui-même les images vers le GPU et depuis celui-ci, et nécessite un GPU compatible Direct3D 12. Sur le CPU, `CVDewarpBlock` (OpenCV) est l'alternative.

- **D3D12RemapBlock** : remappage de la vidéo sur le GPU selon une carte UV, pour la calibration d'objectif, la correction de distorsion et d'autres transformations géométriques

`D3D12RemapSettings` règle :

- `Map` : une `D3D12UVRemapMap`, ou `null` pour laisser passer la vidéo inchangée
- `AdapterIndex` : l'adaptateur GPU, `-1` pour l'adaptateur par défaut

Une `D3D12UVRemapMap` se crée à partir de sa largeur et de sa hauteur (chacune de 1 à 16384) et d'un tableau de valeurs `ushort`, quatre par point de la carte, ligne par ligne depuis le coin supérieur gauche : U, V, inutilisé et masque. U et V sont la position d'entrée affichée par le point, de 0 à 65535 pour 0 à 1 de la largeur et de la hauteur de l'entrée. Un masque de 32768 ou plus y affiche l'entrée ; un masque inférieur y affiche du noir. La carte est étirée sur l'image de sortie, sa taille n'a donc pas à correspondre à celle de la vidéo : le point (x, y) de la carte correspond à la position de sortie ((x + 0,5) / largeur, (y + 0,5) / hauteur). La carte est copiée à sa création et occupe 8 octets par point ; une carte de la taille de la vidéo suffit largement. Un pipeline enregistré (`ToJson`, `SaveJsonAsync`) contient la carte, jusqu'à 80 millions de points (8192x8192 en compte 67 millions) ; un pipeline avec une carte plus grande est enregistré sans les paramètres du bloc, et `ToDocument` le signale. Pour la remplacer ou la retirer pendant l'exécution du pipeline, modifiez `Settings.Map` puis appelez `Update()` ; l'image suivante affiche le changement. Le bloc envoie la carte sur le GPU où se trouvent les images vidéo et possède cette copie, l'application ne manipule donc jamais de ressource Direct3D ; l'image qui reçoit une nouvelle carte attend son envoi. Un échec de l'envoi est signalé via `OnError`, et la carte précédente reste en usage (s'il n'y en a pas, ou si elle a été faite pour un autre GPU, la vidéo passe inchangée) jusqu'au prochain `Update()`, qui envoie de nouveau la carte, ou jusqu'à ce que la vidéo passe sur un autre GPU. Comme le bloc d'entrelacement, il transfère lui-même les images vers le GPU et depuis celui-ci, et nécessite un GPU compatible Direct3D 12.

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

// Le décodeur attend le flux encodé : démultiplexer le MP4 et analyser son H.264 au lieu de le décoder dans un bloc source (un autre conteneur demande son propre démultiplexeur)
var fileSource = new BasicFileSourceBlock("video.mp4");
var demux = new QTDemuxBlock(renderVideo: true, renderAudio: false);
var parse = new H264ParseBlock();
pipeline.Connect(fileSource.Output, demux.Input);
pipeline.Connect(demux.VideoOutput, parse.Input);

// Décodeur matériel D3D11
var d3d11Decoder = new D3D11H264DecoderBlock();
pipeline.Connect(parse.Output, d3d11Decoder.Input);

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

// Décodage logiciel : un décodeur matériel produit déjà de la mémoire GPU, et le téléversement n'a alors rien à faire
var settings = await UniversalSourceSettings.CreateAsync("video.mp4");
settings.DisableHWDecoders = true;
var fileSource = new UniversalSourceBlock(settings);

// Téléverser vers le GPU
var d3d11Upload = new D3D11UploadBlock();
pipeline.Connect(fileSource.VideoOutput, d3d11Upload.Input);

// Conversion colorimétrique GPU, appliquée quand le bloc suivant attend un autre format
var d3d11Converter = new D3D11VideoConverterBlock();
pipeline.Connect(d3d11Upload.Output, d3d11Converter.Input);

// Récupérer depuis le GPU
var d3d11Download = new D3D11DownloadBlock();
pipeline.Connect(d3d11Converter.Output, d3d11Download.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(d3d11Download.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Progressif vers entrelacé sur le GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// Une source progressive 25p
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// 25p vers 50i, trame supérieure en premier
var interlace = new D3D12InterlaceBlock(new D3D12InterlaceSettings
{
    FieldPattern = D3D12InterlacePattern.TwoToTwo,
    TopFieldFirst = true
});
pipeline.Connect(fileSource.VideoOutput, interlace.Input);

// MPEG-2 conserve l'entrelacement et l'ordre des trames dans le flux
var encoder = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(interlace.Output, encoder.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));
pipeline.Connect(encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Redressement fisheye sur le GPU

```csharp
var pipeline = new MediaBlocksPipeline();

// Un enregistrement fisheye circulaire
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("fisheye.mp4"));

// Une vue en perspective de 90 degrés, tournée de 30 degrés autour de l'axe vertical (lacet)
var dewarp = new D3D12FisheyeDewarpBlock(new D3D12FisheyeDewarpSettings
{
    Projection = D3D12FisheyeProjection.Perspective,
    RotationY = 30
});
pipeline.Connect(fileSource.VideoOutput, dewarp.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(dewarp.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Plus tard, pendant la lecture : faire pivoter la vue
dewarp.Settings.RotationY = -45;
dewarp.Update();
```

### Remappage vidéo sur le GPU

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video.mp4"));

// Votre propre carte, par exemple issue d'une calibration d'objectif : 1920 x 1080 points, quatre valeurs chacun (U, V, inutilisé, masque)
ushort[] calibration = LoadCalibrationMap();
var remap = new D3D12RemapBlock(new D3D12RemapSettings
{
    Map = new D3D12UVRemapMap(1920, 1080, calibration)
});
pipeline.Connect(fileSource.VideoOutput, remap.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(remap.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Plus tard, pendant la lecture : passer à une autre calibration
remap.Settings.Map = new D3D12UVRemapMap(1920, 1080, LoadOtherCalibrationMap());
remap.Update();
```

### Composition vidéo

```csharp
var pipeline = new MediaBlocksPipeline();

var source1 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video1.mp4"));
var source2 = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("video2.mp4"));

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
