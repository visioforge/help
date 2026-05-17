---
title: Blocs de décodeurs vidéo en C# .NET — H.264, HEVC, AV1
description: Décodez les vidéos H.264, HEVC, VP9, AV1 et MJPEG avec accélération matérielle grâce à VisioForge Media Blocks SDK. Décodage accéléré GPU pour .NET.
sidebar_label: Décodeurs vidéo
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - UniversalDemuxBlock
  - VideoRendererBlock
  - BasicFileSourceBlock
  - MediaBlocksPipeline
  - MediaInfoReaderX

---

# Blocs de décodeurs vidéo — VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Les blocs de décodage vidéo sont des composants essentiels dans un pipeline multimédia, responsables de la décompression des flux vidéo encodés en images vidéo brutes pouvant être traitées ou rendues ultérieurement. VisioForge Media Blocks SDK .Net offre une variété de blocs de décodage vidéo prenant en charge de nombreux codecs et technologies d'accélération matérielle.

## Blocs de décodeurs vidéo disponibles

### Bloc décodeur H264

Décode les flux vidéo H.264 (AVC). C'est l'un des standards de compression vidéo les plus utilisés. Le bloc peut utiliser différentes implémentations de décodeur sous-jacentes comme FFMPEG, OpenH264 ou des décodeurs accélérés matériellement s'ils sont disponibles.

#### Informations sur le bloc

Nom : `H264DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée H.264 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Paramètres

Le `H264DecoderBlock` est configuré à l'aide de paramètres qui implémentent `IH264DecoderSettings`. Les classes de paramètres disponibles incluent :
- `FFMPEGH264DecoderSettings`
- `OpenH264DecoderSettings`
- `NVH264DecoderSettings` (pour l'accélération GPU NVIDIA)
- `VAAPIH264DecoderSettings` (pour l'accélération VA-API sur Linux)

Un constructeur sans paramètres tente de sélectionner automatiquement un décodeur disponible.

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.264 --> H264DecoderBlock;
    H264DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur H264
var h264Decoder = new H264DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

// Obtenir les informations multimédias avec MediaInfoReaderX
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant VideoView1

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vous pouvez vérifier les implémentations de décodeur spécifiques avec `H264DecoderBlock.IsAvailable(H264DecoderType decoderType)`.
`H264DecoderType` inclut `FFMPEG`, `OpenH264`, `GPU_Nvidia_H264`, `VAAPI_H264`, etc.

#### Plateformes

Windows, macOS, Linux. (Les décodeurs spécifiques au matériel comme NVH264Decoder nécessitent un matériel et des pilotes spécifiques.)

### Bloc décodeur JPEG

Décode les flux vidéo JPEG (Motion JPEG) ou les images JPEG individuelles en images vidéo brutes.

#### Informations sur le bloc

Nom : `JPEGDecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo/images encodées JPEG | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    HTTPSourceBlock -- Flux MJPEG --> JPEGDecoderBlock;
    JPEGDecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur JPEG
var jpegDecoder = new JPEGDecoderBlock();

// Exemple : créer une source HTTP pour une caméra MJPEG et un moteur de rendu vidéo
var httpSettings = new HTTPSourceSettings(new Uri("http://your-mjpeg-camera/stream"));
var httpSource = new HTTPSourceBlock(httpSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // En supposant VideoView1

// Connecter les blocs
pipeline.Connect(httpSource.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vous pouvez vérifier si le décodeur JPEG NVIDIA sous-jacent (le cas échéant) est disponible avec `NVJPEGDecoderBlock.IsAvailable()`. La fonctionnalité générique de décodeur JPEG est généralement disponible.

#### Plateformes

Windows, macOS, Linux. (L'implémentation spécifique NVIDIA nécessite du matériel NVIDIA.)

### Bloc décodeur H.264 NVIDIA (NVH264DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo H.264 (AVC) à l'aide de la technologie NVDEC de NVIDIA. Cela offre des performances et une efficacité élevées sur les systèmes équipés de GPU NVIDIA compatibles.

#### Informations sur le bloc

Nom : `NVH264DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée H.264 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.264 --> NVH264DecoderBlock;
    NVH264DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur H.264 NVIDIA
var nvH264Decoder = new NVH264DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH264Decoder.Input);
pipeline.Connect(nvH264Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVH264DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur H.265 NVIDIA (NVH265DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo H.265 (HEVC) à l'aide de la technologie NVDEC de NVIDIA. H.265 offre une meilleure efficacité de compression que H.264.

#### Informations sur le bloc

Nom : `NVH265DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée H.265/HEVC | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.265 --> NVH265DecoderBlock;
    NVH265DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur H.265 NVIDIA
var nvH265Decoder = new NVH265DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvH265Decoder.Input);
pipeline.Connect(nvH265Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVH265DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour H.265 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur JPEG NVIDIA (NVJPEGDecoderBlock)

Fournit un décodage accéléré matériellement des images JPEG ou des flux Motion JPEG (MJPEG) à l'aide de la bibliothèque NVJPEG de NVIDIA. Cela est particulièrement utile pour les flux MJPEG en haute résolution ou à haute fréquence d'images.
(Remarque : l'exemple de pipeline pour JPEG avec BasicFileSourceBlock peut être moins courant que HTTPSource pour MJPEG. L'exemple ci-dessous est adapté mais tenez compte des cas d'usage typiques.)

#### Informations sur le bloc

Nom : `NVJPEGDecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo/images encodées JPEG | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données MJPEG brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo JPEG --> NVJPEGDecoderBlock;
    NVJPEGDecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```
Pour les flux MJPEG en direct, `HTTPSourceBlock --> NVJPEGDecoderBlock` est plus typique.

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur JPEG NVIDIA
var nvJpegDecoder = new NVJPEGDecoderBlock();

// Exemple : créer une source fichier basique pour un fichier MJPEG, un démultiplexeur et un moteur de rendu
// Assurez-vous que « test.mjpg » contient un flux Motion JPEG.
var basicFileSource = new BasicFileSourceBlock("test.mjpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test.mjpg");
var mediaInfo = reader.Info;
if (mediaInfo == null || mediaInfo.VideoStreams.Count == 0 || !mediaInfo.VideoStreams[0].Codec.Contains("jpeg"))
{
    Console.WriteLine("Échec de l'obtention des infos MJPEG ou ce n'est pas un fichier MJPEG.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvJpegDecoder.Input);
pipeline.Connect(nvJpegDecoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVJPEGDecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur MPEG-1 NVIDIA (NVMPEG1DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo MPEG-1 à l'aide de la technologie NVDEC de NVIDIA.

#### Informations sur le bloc

Nom : `NVMPEG1DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée MPEG-1 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo MPEG-1 --> NVMPEG1DecoderBlock;
    NVMPEG1DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur MPEG-1 NVIDIA
var nvMpeg1Decoder = new NVMPEG1DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_mpeg1.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg1.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg1Decoder.Input);
pipeline.Connect(nvMpeg1Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVMPEG1DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour MPEG-1 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur MPEG-2 NVIDIA (NVMPEG2DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo MPEG-2 à l'aide de la technologie NVDEC de NVIDIA. Couramment utilisé pour la vidéo DVD et certaines diffusions télévisuelles numériques.

#### Informations sur le bloc

Nom : `NVMPEG2DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée MPEG-2 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo MPEG-2 --> NVMPEG2DecoderBlock;
    NVMPEG2DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur MPEG-2 NVIDIA
var nvMpeg2Decoder = new NVMPEG2DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg2Decoder.Input);
pipeline.Connect(nvMpeg2Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVMPEG2DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour MPEG-2 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur MPEG-4 NVIDIA (NVMPEG4DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo MPEG-4 Part 2 (souvent trouvés dans les fichiers AVI, par ex. DivX/Xvid) à l'aide de la technologie NVDEC de NVIDIA. Notez que cela est différent de MPEG-4 Part 10 (H.264/AVC).

#### Informations sur le bloc

Nom : `NVMPEG4DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée MPEG-4 Part 2 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo MPEG-4 --> NVMPEG4DecoderBlock;
    NVMPEG4DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur MPEG-4 NVIDIA
var nvMpeg4Decoder = new NVMPEG4DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_mpeg4.avi");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg4.avi");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvMpeg4Decoder.Input);
pipeline.Connect(nvMpeg4Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVMPEG4DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour MPEG-4 Part 2 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur VP8 NVIDIA (NVVP8DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo VP8 à l'aide de la technologie NVDEC de NVIDIA. VP8 est un format vidéo ouvert, souvent utilisé avec WebM.

#### Informations sur le bloc

Nom : `NVVP8DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée VP8 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP8 --> NVVP8DecoderBlock;
    NVVP8DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur VP8 NVIDIA
var nvVp8Decoder = new NVVP8DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp8Decoder.Input);
pipeline.Connect(nvVp8Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVVP8DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour VP8 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur VP9 NVIDIA (NVVP9DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo VP9 à l'aide de la technologie NVDEC de NVIDIA. VP9 est un format de codage vidéo ouvert et libre de redevances développé par Google, souvent utilisé pour le streaming web (par ex. YouTube).

#### Informations sur le bloc

Nom : `NVVP9DecoderBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo en entrée | vidéo encodée VP9 | 1 |
| Vidéo en sortie | vidéo non compressée | 1 |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP9 --> NVVP9DecoderBlock;
    NVVP9DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur VP9 NVIDIA
var nvVp9Decoder = new NVVP9DecoderBlock();

// Exemple : créer une source fichier basique, un démultiplexeur et un moteur de rendu
var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");

var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// Connecter les blocs
pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvVp9Decoder.Input);
pipeline.Connect(nvVp9Decoder.Output, videoRenderer.Input);

// Démarrer le pipeline
await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVVP9DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA prenant en charge NVDEC pour VP9 et les pilotes appropriés.

#### Plateformes

Windows, Linux (avec pilotes NVIDIA).

### Bloc décodeur H.264 VAAPI (VAAPIH264DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo H.264 (AVC) à l'aide de VA-API (Video Acceleration API). Disponible sur les systèmes Linux avec matériel et pilotes compatibles.

#### Informations sur le bloc

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée H.264 | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.264 --> VAAPIH264DecoderBlock;
    VAAPIH264DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiH264Decoder = new VAAPIH264DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiH264Decoder.Input);
pipeline.Connect(vaapiH264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez avec `VAAPIH264DecoderBlock.IsAvailable()`. Nécessite la prise en charge VA-API et le redistribuable correct du SDK.

#### Plateformes

Linux (avec pilotes VA-API).

---

### Bloc décodeur HEVC VAAPI (VAAPIHEVCDecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo H.265/HEVC à l'aide de VA-API. Disponible sur les systèmes Linux avec matériel et pilotes compatibles.

#### Informations sur le bloc

| Direction du pin | Type de média            | Nombre de pins |
|---------------|----------------------|------------|
| Vidéo en entrée   | encodé H.265/HEVC   | 1          |
| Vidéo en sortie  | vidéo non compressée   | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.265 --> VAAPIHEVCDecoderBlock;
    VAAPIHEVCDecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiHevcDecoder = new VAAPIHEVCDecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiHevcDecoder.Input);
pipeline.Connect(vaapiHevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez avec `VAAPIHEVCDecoderBlock.IsAvailable()`. Nécessite la prise en charge VA-API et le redistribuable correct du SDK.

#### Plateformes

Linux (avec pilotes VA-API).

---

### Bloc décodeur JPEG VAAPI (VAAPIJPEGDecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo JPEG/MJPEG à l'aide de VA-API. Disponible sur les systèmes Linux avec matériel et pilotes compatibles.

#### Informations sur le bloc

| Direction du pin | Type de média                | Nombre de pins |
|---------------|--------------------------|------------|
| Vidéo en entrée   | vidéo/images encodées JPEG | 1          |
| Vidéo en sortie  | vidéo non compressée        | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    HTTPSourceBlock -- Flux MJPEG --> VAAPIJPEGDecoderBlock;
    VAAPIJPEGDecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiJpegDecoder = new VAAPIJPEGDecoderBlock();
var httpSettings = new HTTPSourceSettings(new Uri("http://your-mjpeg-camera/stream"));
var httpSource = new HTTPSourceBlock(httpSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(httpSource.Output, vaapiJpegDecoder.Input);
pipeline.Connect(vaapiJpegDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez avec `VAAPIJPEGDecoderBlock.IsAvailable()`. Nécessite la prise en charge VA-API et le redistribuable correct du SDK.

#### Plateformes

Linux (avec pilotes VA-API).

---

### Bloc décodeur VC1 VAAPI (VAAPIVC1DecoderBlock)

Fournit un décodage accéléré matériellement des flux vidéo VC-1 à l'aide de VA-API. Disponible sur les systèmes Linux avec matériel et pilotes compatibles.

#### Informations sur le bloc

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée VC-1  | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VC-1 --> VAAPIVC1DecoderBlock;
    VAAPIVC1DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();
var vaapiVc1Decoder = new VAAPIVC1DecoderBlock();
var basicFileSource = new BasicFileSourceBlock("test_vc1.wmv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vc1.wmv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vaapiVc1Decoder.Input);
pipeline.Connect(vaapiVc1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez avec `VAAPIVC1DecoderBlock.IsAvailable()`. Nécessite la prise en charge VA-API et le redistribuable correct du SDK.

#### Plateformes

Linux (avec pilotes VA-API).

---

## Blocs de décodeurs vidéo Direct3D 11/DXVA

Les blocs décodeurs Direct3D 11/DXVA (D3D11) fournissent un décodage vidéo accéléré matériellement sur les systèmes Windows avec des GPU et pilotes compatibles. Ces blocs sont utiles pour les pipelines de lecture et de traitement vidéo haute performance sous Windows.

### Bloc décodeur AV1 D3D11

Décode les flux vidéo AV1 à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11AV1DecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée AV1   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo AV1 --> D3D11AV1DecoderBlock;
    D3D11AV1DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur AV1 D3D11
var d3d11Av1Decoder = new D3D11AV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_av1.mkv");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_av1.mkv");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Av1Decoder.Input);
pipeline.Connect(d3d11Av1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11AV1DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur H.264 D3D11

Décode les flux vidéo H.264 (AVC) à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11H264DecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée H.264 | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.264 --> D3D11H264DecoderBlock;
    D3D11H264DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur H.264 D3D11
var d3d11H264Decoder = new D3D11H264DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h264.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h264.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H264Decoder.Input);
pipeline.Connect(d3d11H264Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11H264DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur H.265 D3D11

Décode les flux vidéo H.265 (HEVC) à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11H265DecoderBlock`.

| Direction du pin | Type de média            | Nombre de pins |
|---------------|----------------------|------------|
| Vidéo en entrée   | encodé H.265/HEVC   | 1          |
| Vidéo en sortie  | vidéo non compressée   | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo H.265 --> D3D11H265DecoderBlock;
    D3D11H265DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur H.265 D3D11
var d3d11H265Decoder = new D3D11H265DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_h265.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_h265.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11H265Decoder.Input);
pipeline.Connect(d3d11H265Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11H265DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur MPEG-2 D3D11

Décode les flux vidéo MPEG-2 à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11MPEG2DecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée MPEG-2| 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo MPEG-2 --> D3D11MPEG2DecoderBlock;
    D3D11MPEG2DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur MPEG-2 D3D11
var d3d11Mpeg2Decoder = new D3D11MPEG2DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_mpeg2.mpg");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_mpeg2.mpg");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Mpeg2Decoder.Input);
pipeline.Connect(d3d11Mpeg2Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11MPEG2DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur VP8 D3D11

Décode les flux vidéo VP8 à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11VP8DecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée VP8   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP8 --> D3D11VP8DecoderBlock;
    D3D11VP8DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur VP8 D3D11
var d3d11Vp8Decoder = new D3D11VP8DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp8Decoder.Input);
pipeline.Connect(d3d11Vp8Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11VP8DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur VP9 D3D11

Décode les flux vidéo VP9 à l'aide de l'accélération matérielle Direct3D 11/DXVA.

#### Informations sur le bloc

Nom : `D3D11VP9DecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée VP9   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP9 --> D3D11VP9DecoderBlock;
    D3D11VP9DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur VP9 D3D11
var d3d11Vp9Decoder = new D3D11VP9DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), d3d11Vp9Decoder.Input);
pipeline.Connect(d3d11Vp9Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `D3D11VP9DecoderBlock.IsAvailable()`. Nécessite Windows avec prise en charge D3D11/DXVA et le redistribuable correct du SDK.

#### Plateformes

Windows (D3D11/DXVA requis).

---

### Bloc décodeur HEVC

Décode les flux vidéo H.265/HEVC à l'aide du meilleur décodeur disponible sur le système. Prend en charge le décodage logiciel via FFmpeg et le décodage accéléré matériellement via NVIDIA NVDEC, Intel Quick Sync, AMD AMF, D3D11/DXVA (Windows) et VAAPI (Linux). Lorsqu'il est instancié sans arguments, le bloc sélectionne automatiquement le décodeur le plus rapide disponible dans l'ordre : NVIDIA NVDEC → Intel QSV → AMD AMF → D3D11 (Windows) → VAAPI (Linux) → FFmpeg.

#### Informations sur le bloc

Nom : `HEVCDecoderBlock`.

| Direction du pin | Type de média           | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée HEVC  | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Paramètres

`HEVCDecoderBlock` peut sélectionner automatiquement le meilleur backend ou en utiliser un explicite. Toutes les classes de configuration implémentent l'interface `IHEVCDecoderSettings`.

```csharp
// Sélection automatique (recommandé)
var hevcDecoder = new HEVCDecoderBlock();

// Sélection explicite du backend :
// FFmpeg (CPU, logiciel)
var hevcDecoder = new HEVCDecoderBlock(new FFMPEGHEVCDecoderSettings());

// NVIDIA NVDEC (GPU)
var hevcDecoder = new HEVCDecoderBlock(new NVHEVCDecoderSettings());

// Intel Quick Sync
var hevcDecoder = new HEVCDecoderBlock(new QSVHEVCDecoderSettings());

// AMD AMF
var hevcDecoder = new HEVCDecoderBlock(new AMFHEVCDecoderSettings());

// D3D11/DXVA (Windows uniquement)
var hevcDecoder = new HEVCDecoderBlock(new D3D11HEVCDecoderSettings());

// VAAPI (Linux uniquement)
var hevcDecoder = new HEVCDecoderBlock(new VAAPIHEVCDecoderSettings());
```

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo HEVC --> HEVCDecoderBlock;
    HEVCDecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Sélection automatique du meilleur décodeur HEVC disponible
var hevcDecoder = new HEVCDecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), hevcDecoder.Input);
pipeline.Connect(hevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité d'un type de décodeur spécifique avec `HEVCDecoderBlock.IsAvailable(HEVCDecoderType decoder)`. Assurez-vous que le paquet de redistribution SDK approprié est inclus dans votre projet.

#### Plateformes

Windows, macOS, Linux.

---

### Bloc décodeur AV1

Décode les flux vidéo AV1 à l'aide d'un décodage logiciel ou matériel. Prend en charge dav1d (décodeur logiciel rapide), le décodeur de référence AOM (av1dec) et les décodeurs accélérés matériellement via NVIDIA NVDEC, D3D11/DXVA (Windows) et VAAPI (Linux). AV1 offre une efficacité de compression supérieure à H.264 et VP9, avec prise en charge jusqu'à 8K, couleur 10 bits et contenu HDR.

#### Informations sur le bloc

Nom : `AV1DecoderBlock`.

| Direction du pin | Type de média          | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée AV1   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Paramètres

Le `AV1DecoderBlock` est configuré à l'aide de `AV1DecoderSettings`, qui spécifie le type de décodeur via l'enum `AV1DecoderType` :

- `AV1DecoderType.Auto` — sélectionne automatiquement le meilleur décodeur disponible (par défaut)
- `AV1DecoderType.Dav1d` — décodeur logiciel dav1d (rapide, recommandé pour le décodage CPU)
- `AV1DecoderType.AOM` — décodeur de référence AOM (Alliance for Open Media) (logiciel)
- `AV1DecoderType.NVIDIA` — décodage matériel NVIDIA NVDEC (série RTX 30 ou plus récent)
- `AV1DecoderType.Intel_QSV` — décodage matériel Intel Quick Sync Video (série Arc ou plus récent)
- `AV1DecoderType.D3D11` — décodage matériel D3D11/DXVA (Windows)
- `AV1DecoderType.VAAPI` — décodage matériel VAAPI (Linux)

Un constructeur sans paramètres sélectionne automatiquement le meilleur décodeur disponible.

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo AV1 --> AV1DecoderBlock;
    AV1DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Sélection automatique du meilleur décodeur AV1 disponible
var av1Decoder = new AV1DecoderBlock();

// Ou spécifier explicitement un décodeur :
// var av1Decoder = new AV1DecoderBlock(new AV1DecoderSettings { DecoderType = AV1DecoderType.Dav1d });

var basicFileSource = new BasicFileSourceBlock("test_av1.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_av1.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), av1Decoder.Input);
pipeline.Connect(av1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité d'un type de décodeur spécifique avec `AV1DecoderBlock.IsAvailable(AV1DecoderType decoderType)`. Passez `AV1DecoderType.Auto` pour vérifier si un décodeur AV1 est disponible. Assurez-vous que le paquet de redistribution SDK approprié est inclus dans votre projet.

#### Plateformes

Windows, macOS, Linux.

---

### Bloc décodeur NVAV1

Décode les flux vidéo AV1 à l'aide de l'accélération matérielle NVIDIA NVDEC exclusivement. Nécessite un GPU NVIDIA avec prise en charge AV1 NVDEC (série RTX 30xx ou plus récent). Capable de décoder 4K@120fps ou 8K@60fps selon le modèle de GPU, avec prise en charge du contenu HDR10 et HDR10+.

#### Informations sur le bloc

Nom : `NVAV1DecoderBlock`.

| Direction du pin | Type de média          | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée AV1   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo AV1 --> NVAV1DecoderBlock;
    NVAV1DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer le bloc décodeur NVAV1 (nécessite un GPU NVIDIA avec prise en charge AV1 NVDEC)
var nvav1Decoder = new NVAV1DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_av1.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_av1.mp4");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), nvav1Decoder.Input);
pipeline.Connect(nvav1Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `NVAV1DecoderBlock.IsAvailable()`. Nécessite un GPU NVIDIA avec prise en charge AV1 NVDEC et le paquet de redistribution NVIDIA Video Codec SDK.

#### Plateformes

Windows, Linux (GPU NVIDIA avec prise en charge AV1 NVDEC requis).

---

### Bloc décodeur VP8

Décode les flux vidéo VP8 couramment utilisés dans les conteneurs WebM et les communications vidéo WebRTC. Prend en charge le décodage logiciel via VPX et FFmpeg, et le décodage accéléré matériellement via NVIDIA NVDEC, VAAPI (Linux) et D3D11/DXVA (Windows). Lorsqu'il est instancié sans arguments, le bloc sélectionne automatiquement parmi les décodeurs disponibles dans l'ordre : VPX → FFmpeg → NVIDIA NVDEC → VAAPI (Linux) → D3D11 (Windows).

#### Informations sur le bloc

Nom : `VP8DecoderBlock`.

| Direction du pin | Type de média          | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée VP8   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Paramètres

`VP8DecoderBlock` peut sélectionner automatiquement le meilleur backend ou en utiliser un explicite. Toutes les classes de configuration implémentent l'interface `IVP8DecoderSettings`.

```csharp
// Sélection automatique (recommandé)
var vp8Decoder = new VP8DecoderBlock();

// Sélection explicite du backend :
// VPX (logiciel, large compatibilité)
var vp8Decoder = new VP8DecoderBlock(new VPXVP8DecoderSettings());

// FFmpeg (logiciel)
var vp8Decoder = new VP8DecoderBlock(new FFMPEGVP8DecoderSettings());

// NVIDIA NVDEC (matériel)
var vp8Decoder = new VP8DecoderBlock(new NVVP8DecoderSettings());

// VAAPI (Linux uniquement, matériel)
var vp8Decoder = new VP8DecoderBlock(new VAAPIVP8DecoderSettings());

// D3D11/DXVA (Windows uniquement, matériel)
var vp8Decoder = new VP8DecoderBlock(new D3D11VP8DecoderSettings());
```

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP8 --> VP8DecoderBlock;
    VP8DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Sélection automatique du meilleur décodeur VP8 disponible
var vp8Decoder = new VP8DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp8.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp8Decoder.Input);
pipeline.Connect(vp8Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité d'un type de décodeur spécifique avec `VP8DecoderBlock.IsAvailable(VP8DecoderType decoder)`. Assurez-vous que le paquet de redistribution SDK approprié est inclus dans votre projet.

#### Plateformes

Windows, macOS, Linux.

---

### Bloc décodeur VP9

Décode les flux vidéo VP9 utilisés dans les contenus YouTube 4K/8K et les conteneurs WebM. Prend en charge le décodage logiciel via VPX et FFmpeg, et le décodage accéléré matériellement via NVIDIA NVDEC, VAAPI (Linux) et D3D11/DXVA (Windows). Prend en charge les profondeurs de couleur 10 bits et 12 bits. Lorsqu'il est instancié sans arguments, le bloc sélectionne automatiquement parmi les décodeurs disponibles dans l'ordre : VPX → FFmpeg → NVIDIA NVDEC → VAAPI (Linux) → D3D11 (Windows).

#### Informations sur le bloc

Nom : `VP9DecoderBlock`.

| Direction du pin | Type de média          | Nombre de pins |
|---------------|---------------------|------------|
| Vidéo en entrée   | vidéo encodée VP9   | 1          |
| Vidéo en sortie  | vidéo non compressée  | 1          |

#### Paramètres

`VP9DecoderBlock` peut sélectionner automatiquement le meilleur backend ou en utiliser un explicite. Toutes les classes de configuration implémentent l'interface `IVP9DecoderSettings`.

```csharp
// Sélection automatique (recommandé)
var vp9Decoder = new VP9DecoderBlock();

// Sélection explicite du backend :
// VPX (logiciel, large compatibilité)
var vp9Decoder = new VP9DecoderBlock(new VPXVP9DecoderSettings());

// FFmpeg (logiciel)
var vp9Decoder = new VP9DecoderBlock(new FFMPEGVP9DecoderSettings());

// NVIDIA NVDEC (matériel)
var vp9Decoder = new VP9DecoderBlock(new NVVP9DecoderSettings());

// VAAPI (Linux uniquement, matériel)
var vp9Decoder = new VP9DecoderBlock(new VAAPIVP9DecoderSettings());

// D3D11/DXVA (Windows uniquement, matériel)
var vp9Decoder = new VP9DecoderBlock(new D3D11VP9DecoderSettings());
```

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP9 --> VP9DecoderBlock;
    VP9DecoderBlock -- Vidéo décodée --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Sélection automatique du meilleur décodeur VP9 disponible
var vp9Decoder = new VP9DecoderBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp9.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp9Decoder.Input);
pipeline.Connect(vp9Decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité d'un type de décodeur spécifique avec `VP9DecoderBlock.IsAvailable(VP9DecoderType decoder)`. Assurez-vous que le paquet de redistribution SDK approprié est inclus dans votre projet.

#### Plateformes

Windows, macOS, Linux.

---

### Bloc VP8 Alpha Decode Bin

Décode les flux vidéo VP8 qui incluent un canal alpha de transparence, produisant une sortie RGBA ou YUVA adaptée à la composition et aux effets de superposition. Gère automatiquement le démultiplexage et le décodage des canaux couleur et alpha. Destiné aux fichiers WebM ou MKV encodés avec la prise en charge de VP8 alpha.

#### Informations sur le bloc

Nom : `VP8AlphaDecodeBinBlock`.

| Direction du pin | Type de média                       | Nombre de pins |
|---------------|----------------------------------|------------|
| Vidéo en entrée   | vidéo VP8 avec canal alpha     | 1          |
| Vidéo en sortie  | vidéo non compressée (RGBA/YUVA)   | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP8+Alpha --> VP8AlphaDecodeBinBlock;
    VP8AlphaDecodeBinBlock -- Vidéo RGBA --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var vp8AlphaDecoder = new VP8AlphaDecodeBinBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp8_alpha.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp8_alpha.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp8AlphaDecoder.Input);
pipeline.Connect(vp8AlphaDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `VP8AlphaDecodeBinBlock.IsAvailable()`. Nécessite le plugin GStreamer codecalpha et le paquet de redistribution SDK approprié.

#### Plateformes

Windows, macOS, Linux.

---

### Bloc VP9 Alpha Decode Bin

Décode les flux vidéo VP9 qui incluent un canal alpha de transparence, produisant une sortie RGBA ou YUVA. Prend en charge jusqu'à la résolution 4K. Gère automatiquement le démultiplexage et le décodage des canaux couleur et alpha. Destiné aux fichiers WebM ou MKV encodés avec la prise en charge de VP9 alpha.

#### Informations sur le bloc

Nom : `VP9AlphaDecodeBinBlock`.

| Direction du pin | Type de média                       | Nombre de pins |
|---------------|----------------------------------|------------|
| Vidéo en entrée   | vidéo VP9 avec canal alpha     | 1          |
| Vidéo en sortie  | vidéo non compressée (RGBA/YUVA)   | 1          |

#### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock -- Données brutes --> UniversalDemuxBlock;
    UniversalDemuxBlock -- Flux vidéo VP9+Alpha --> VP9AlphaDecodeBinBlock;
    VP9AlphaDecodeBinBlock -- Vidéo RGBA --> VideoRendererBlock;
```

#### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var vp9AlphaDecoder = new VP9AlphaDecodeBinBlock();

var basicFileSource = new BasicFileSourceBlock("test_vp9_alpha.webm");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_vp9_alpha.webm");
var mediaInfo = reader.Info;
if (mediaInfo == null)
{
    Console.WriteLine("Échec de l'obtention des informations multimédias.");
    return;
}
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), vp9AlphaDecoder.Input);
pipeline.Connect(vp9AlphaDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Disponibilité

Vérifiez la disponibilité avec `VP9AlphaDecodeBinBlock.IsAvailable()`. Nécessite le plugin GStreamer codecalpha et le paquet de redistribution SDK approprié.

#### Plateformes

Windows, macOS, Linux.
