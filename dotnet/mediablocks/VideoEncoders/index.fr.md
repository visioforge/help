---
title: Blocs d'encodeurs vidéo — H.264, HEVC, AV1, VP9 en C# .NET
description: Encodez la vidéo en H.264, HEVC, AV1 et VP9 avec accélération GPU grâce à VisioForge Media Blocks SDK. Débit, qualité et codecs configurables.
sidebar_label: Encodeurs vidéo
order: 18
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
  - UniversalSourceBlock
  - MediaBlocksPipeline
  - UniversalSourceSettings
  - MediaBlockPadMediaType
  - MP4SinkBlock

---

# Encodage vidéo

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

L'encodage vidéo est le processus de conversion des données vidéo brutes en un format compressé. Ce processus est essentiel pour réduire la taille des fichiers vidéo, ce qui les rend plus faciles à stocker et à diffuser sur Internet. VisioForge Media Blocks SDK fournit une large gamme d'encodeurs vidéo prenant en charge divers formats et codecs.

Pour certains encodeurs vidéo, le SDK peut utiliser l'accélération GPU pour accélérer le processus d'encodage. Cette fonctionnalité est particulièrement utile lorsque vous travaillez avec des fichiers vidéo en haute résolution ou lorsque vous encodez plusieurs vidéos simultanément.

Les GPU NVidia, Intel et AMD sont pris en charge pour l'accélération matérielle.

## Encodeur AV1

`AV1 (AOMedia Video 1)` : développé par l'Alliance for Open Media, AV1 est un format de codage vidéo ouvert et libre de redevances conçu pour les transmissions vidéo sur Internet. Il est connu pour sa haute efficacité de compression et une meilleure qualité à des débits plus bas que ses prédécesseurs, ce qui en fait un format bien adapté aux applications de streaming vidéo haute résolution.

Utilisez les classes qui implémentent l'interface `IAV1EncoderSettings` pour définir les paramètres.

#### Encodeurs CPU

##### AOMAV1EncoderSettings

Paramètres d'encodeur AOM AV1. Encodeur CPU.

**Plateformes :** Windows, Linux, macOS.

##### RAV1EEncoderSettings

Paramètres d'encodeur RAV1E AV1. Encodeur CPU.

- **Propriétés clés** :
  - `Bitrate` (entier) : débit binaire cible en kilobits par seconde.
  - `LowLatency` (booléen) : active ou désactive le mode faible latence. Par défaut `false`.
  - `MaxKeyFrameInterval` (ulong) : intervalle maximal entre images clés. Par défaut `240`.
  - `MinKeyFrameInterval` (ulong) : intervalle minimal entre images clés. Par défaut `12`.
  - `MinQuantizer` (uint) : valeur minimale du quantificateur (plage 0-255). Par défaut `0`.
  - `Quantizer` (uint) : valeur du quantificateur (plage 0-255). Par défaut `100`.
  - `SpeedPreset` (int) : préréglage de vitesse d'encodage (10 le plus rapide, 0 le plus lent). Par défaut `6`.
  - `Tune` (`RAV1EEncoderTune`) : réglage de tune pour l'encodeur. Par défaut `RAV1EEncoderTune.Psychovisual`.

**Plateformes :** Windows, Linux, macOS.

###### Enum RAV1EEncoderTune

Spécifie l'option de tune pour l'encodeur RAV1E.

- `PSNR` (0) : tune pour le meilleur PSNR (rapport signal/bruit de crête).
- `Psychovisual` (1) : tune pour la qualité psychovisuelle.

#### Encodeurs GPU

##### AMFAV1EncoderSettings

Encodeur vidéo AV1 GPU AMD.

**Plateformes :** Windows, Linux, macOS.

##### NVENCAV1EncoderSettings

Encodeur vidéo AV1 GPU Nvidia.

**Plateformes :** Windows, Linux, macOS.

##### QSVAV1EncoderSettings

Encodeur vidéo AV1 GPU Intel.

**Plateformes :** Windows, Linux, macOS.

*Remarque : les encodeurs Intel QSV peuvent également utiliser des énumérations communes comme `QSVCodingOption` (`On`, `Off`, `Unknown`) pour configurer des fonctionnalités matérielles spécifiques.*

### Informations sur le bloc

Nom : AV1EncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | AV1 | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AV1EncoderBlock;
    AV1EncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new AV1EncoderBlock(new QSVAV1EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(videoEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur DV

`DV (Digital Video)` : un format pour le stockage de vidéo numérique introduit dans les années 1990, principalement utilisé dans les caméscopes numériques grand public. DV utilise une compression intra-image pour offrir une vidéo de haute qualité sur bandes numériques, ce qui le rend adapté aux vidéos amateur ainsi qu'aux productions semi-professionnelles.

### Informations sur le bloc

Nom : DVEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | video/x-dv | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->DVEncoderBlock;
    DVEncoderBlock-->AVISinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new DVEncoderBlock(new DVVideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var sinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, sinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur vidéo personnalisé

Le bloc CustomVideoEncoder fournit un cadre flexible pour intégrer des encodeurs vidéo tiers ou spécialisés qui ne sont pas directement pris en charge par le SDK. Cela permet l'intégration de codecs propriétaires, d'encodeurs expérimentaux ou de solutions d'encodage spécifiques à une plateforme.

### Informations sur le bloc

Nom : CustomVideoEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | Vidéo compressée | 1

### Paramètres

#### CustomVideoEncoderSettings

```csharp
public class CustomVideoEncoderSettings : IVideoEncoder
{
    // Nom de l'élément encodeur GStreamer (par ex. "x264enc", "nvh264enc").
    // Défini via le constructeur ; le setter est privé.
    public string Name { get; private set; }

    // Dictionnaire des propriétés spécifiques à l'encodeur (setter privé — remplir via indexeur / Add).
    public Dictionary<string, object> Properties { get; private set; }

    public CustomVideoEncoderSettings(string name);
}
```

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->CustomVideoEncoderBlock;
    CustomVideoEncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

// Configurer l'encodeur personnalisé (exemple avec x264enc — Name est défini via le ctor ; remplir
// le dictionnaire Properties via son indexeur car la propriété a un setter privé).
var customSettings = new CustomVideoEncoderSettings("x264enc");
customSettings.Properties["bitrate"] = 2000;
customSettings.Properties["speed-preset"] = "medium";
var customEncoder = new CustomVideoEncoderBlock(customSettings);
pipeline.Connect(fileSource.VideoOutput, customEncoder.Input);

var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(customEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux (nécessite les plugins GStreamer appropriés).

## Encodeur GIF

Le bloc encodeur GIF crée des images GIF animées à partir de flux vidéo, adaptées au contenu web, aux réseaux sociaux et à la documentation.

### Informations sur le bloc

Nom : GIFEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | GIF | 1

### Paramètres

#### GIFEncoderSettings

```csharp
public class GIFEncoderSettings
{
    // Nombre de répétitions supplémentaires : -1 = boucle infinie, 0..n = répétitions finies. Par défaut 0.
    public uint Repeat { get; set; }

    // Vitesse d'encodage [1..30]. Plus élevé = plus rapide (qualité moindre). Par défaut 10.
    public int Speed { get; set; }
}
```

> La fréquence d'images de sortie d'un `GIFEncoderBlock` suit les caps en amont — définissez la fréquence d'images souhaitée sur la source (ou insérez un filtre de débit / `VideoFrameRateBlock` en amont) plutôt que sur `GIFEncoderSettings`.

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->GIFEncoderBlock;
    GIFEncoderBlock-->FileSink;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var gifSettings = new GIFEncoderSettings
{
    Repeat = 0,    // 0 = pas de boucle supplémentaire ; -1 = boucle infinie
    Speed  = 10    // [1..30] — plus élevé = encodage plus rapide (qualité moindre)
};
var gifEncoder = new GIFEncoderBlock(gifSettings, "output.gif");
pipeline.Connect(fileSource.VideoOutput, gifEncoder.Input);

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur H264 { #h264-encoder }

Le bloc encodeur H264 est utilisé pour encoder des fichiers aux formats MP4, MKV, et quelques autres, ainsi que pour le streaming réseau via RTSP et HLS.

Utilisez les classes qui implémentent l'interface IH264EncoderSettings pour définir les paramètres.

### Paramètres

#### NVENCH264EncoderSettings

Encodeur vidéo H264 GPU Nvidia.

**Plateformes :** Windows, Linux, macOS.

#### AMFH264EncoderSettings

Encodeur vidéo H264 GPU AMD/ATI.

**Plateformes :** Windows, Linux, macOS.

#### QSVH264EncoderSettings

Encodeur vidéo H264 GPU Intel.

**Plateformes :** Windows, Linux, macOS.

#### OpenH264EncoderSettings

Encodeur H264 logiciel CPU.

**Plateformes :** Windows, macOS, Linux, iOS, Android.

#### CustomH264EncoderSettings

Permet d'utiliser un élément GStreamer personnalisé pour l'encodage H264. Vous pouvez spécifier le nom de l'élément GStreamer et configurer ses propriétés.

**Plateformes :** Windows, Linux, macOS.

### Informations sur le bloc

Nom : H264EncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | H264 | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var h264EncoderBlock = new H264EncoderBlock(new NVENCH264EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

#### Exemples d'applications

- [Simple Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)
- [Screen Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur HEVC/H265 { #hevch265-encoder }

L'encodeur HEVC est utilisé pour encoder des fichiers aux formats MP4, MKV, et quelques autres, ainsi que pour le streaming réseau via RTSP et HLS.

Utilisez les classes qui implémentent l'interface IHEVCEncoderSettings pour définir les paramètres.

### Paramètres

#### MFHEVCEncoderSettings

Encodeur HEVC Microsoft Media Foundation. Encodeur CPU.

**Plateformes :** Windows.

#### NVENCHEVCEncoderSettings

Encodeur vidéo HEVC GPU Nvidia.

**Plateformes :** Windows, Linux, macOS.

#### AMFHEVCEncoderSettings

Encodeur vidéo HEVC GPU AMD/ATI.

**Plateformes :** Windows, Linux, macOS.

#### QSVHEVCEncoderSettings

Encodeur vidéo HEVC GPU Intel.

**Plateformes :** Windows, Linux, macOS.

#### CustomHEVCEncoderSettings

Permet d'utiliser un élément GStreamer personnalisé pour l'encodage HEVC. Vous pouvez spécifier le nom de l'élément GStreamer et configurer ses propriétés.

**Plateformes :** Windows, Linux, macOS.

### Informations sur le bloc

Nom : HEVCEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | HEVC | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->HEVCEncoderBlock;
    HEVCEncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var hevcEncoderBlock = new HEVCEncoderBlock(new NVENCHEVCEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur MJPEG

`MJPEG (Motion JPEG)` : un format de compression vidéo dans lequel chaque image de la vidéo est compressée séparément en image JPEG. Cette technique est simple et n'a pas de compression inter-image, ce qui la rend idéale dans les situations où le montage ou l'accès image par image est requis, comme dans la vidéosurveillance et l'imagerie médicale.
Utilisez les classes qui implémentent l'interface IH264EncoderSettings pour définir les paramètres.

### Paramètres

#### MJPEGEncoderSettings

Encodeur MJPEG par défaut. Encodeur CPU.

- **Propriétés clés** :
  - `Quality` (int) : niveau de qualité JPEG (10-100). Par défaut `85`.
- **Type d'encodeur** : `MJPEGEncoderType.CPU`.

**Plateformes :** Windows, Linux, macOS, iOS, Android.

#### QSVMJPEGEncoderSettings

Encodeur MJPEG GPU Intel.

- **Propriétés clés** :
  - `Quality` (uint) : niveau de qualité JPEG (10-100). Par défaut `85`.
- **Type d'encodeur** : `MJPEGEncoderType.GPU_Intel_QSV_MJPEG`.

**Plateformes :** Windows, Linux, macOS.

#### Enum MJPEGEncoderType

Spécifie le type d'encodeur MJPEG.

- `CPU` : encodeur basé CPU par défaut.
- `GPU_Intel_QSV_MJPEG` : encodeur MJPEG basé GPU Intel QuickSync.

### Informations sur le bloc

Nom : MJPEGEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | MJPEG | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MJPEGEncoderBlock;
    MJPEGEncoderBlock-->AVISinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var videoEncoderBlock = new MJPEGEncoderBlock(new MJPEGEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, videoEncoderBlock.Input);

var aviSinkBlock = new AVISinkBlock(new AVISinkSettings(@"output.avi"));
pipeline.Connect(videoEncoderBlock.Output, aviSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur Theora

L'encodeur [Theora](https://www.theora.org/) est utilisé pour encoder des fichiers vidéo au format WebM.

### Paramètres

#### TheoraEncoderSettings

Fournit les paramètres pour l'encodeur Theora.

- **Propriétés clés** :
  - `Bitrate` (kbps)
  - `CapOverflow`, `CapUnderflow` (plafonnement du réservoir de bits)
  - `DropFrames` (autoriser/interdire le rejet d'images)
  - `KeyFrameAuto` (détection automatique des images clés)
  - `KeyFrameForce` (intervalle pour forcer une image clé toutes les N images)
  - `KeyFrameFrequency` (fréquence des images clés)
  - `MultipassCacheFile` (chemin de chaîne pour le cache multipasse)
  - `MultipassMode` (utilisant l'enum `TheoraMultipassMode` : `SinglePass`, `FirstPass`, `SecondPass`)
  - `Quality` (valeur entière, typiquement 0-63 pour libtheora, la signification peut varier)
  - `RateBuffer` (taille du tampon de contrôle de débit en unités d'images, 0 = auto)
  - `SpeedLevel` (niveau de recherche de vecteurs de mouvement, 0-2 ou plus selon l'implémentation)
  - `VP3Compatible` (booléen pour activer la compatibilité VP3)
- **Disponibilité** : peut être vérifiée à l'aide de `TheoraEncoderSettings.IsAvailable()`.

### Informations sur le bloc

Nom : TheoraEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | video/x-theora | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->TheoraEncoderBlock;
    TheoraEncoderBlock-->WebMSinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var theoraEncoderBlock = new TheoraEncoderBlock(new TheoraEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, theoraEncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(theoraEncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur VPX

Le bloc encodeur VPX est utilisé pour encoder des fichiers aux formats WebM, MKV ou OGG. L'encodeur VPX est un ensemble de codecs vidéo pour l'encodage aux formats VP8 et VP9.

Le bloc encodeur VPX utilise des classes de paramètres qui implémentent l'interface `IVPXEncoderSettings`. Les classes de paramètres clés incluent :

### Paramètres

La classe de base commune pour les paramètres d'encodeurs CPU VP8 et VP9 est `VPXEncoderSettings`. Elle fournit un large éventail de propriétés partagées pour ajuster le processus d'encodage, telles que :

- `ARNRMaxFrames`, `ARNRStrength`, `ARNRType` (réduction de bruit AltRef)
- `BufferInitialSize`, `BufferOptimalSize`, `BufferSize` (paramètres de tampon client)
- `CPUUsed`, `CQLevel` (qualité contrainte)
- `Deadline` (délai d'encodage par image)
- `DropframeThreshold`
- `RateControl` (utilisant l'enum `VPXRateControl`)
- `ErrorResilient` (utilisant l'enum `VPXErrorResilientFlags`)
- `HorizontalScalingMode`, `VerticalScalingMode` (utilisant l'enum `VPXScalingMode`)
- `KeyFrameMaxDistance`, `KeyFrameMode` (utilisant l'enum `VPXKeyFrameMode`)
- `MinQuantizer`, `MaxQuantizer`
- `MultipassCacheFile`, `MultipassMode` (utilisant l'enum `VPXMultipassMode`)
- `NoiseSensitivity`
- `TargetBitrate` (en Kbits/s)
- `NumOfThreads`
- `TokenPartitions` (utilisant l'enum `VPXTokenPartitions`)
- `Tuning` (utilisant l'enum `VPXTuning`)

#### VP8EncoderSettings

Encodeur CPU pour VP8. Hérite de `VPXEncoderSettings`.

- **Propriétés clés** : tire parti des propriétés de `VPXEncoderSettings` adaptées à VP8.
- **Type d'encodeur** : `VPXEncoderType.VP8`.
- **Disponibilité** : peut être vérifiée à l'aide de `VP8EncoderSettings.IsAvailable()`.

#### VP9EncoderSettings

Encodeur CPU pour VP9. Hérite de `VPXEncoderSettings`.

- **Propriétés clés** : en plus des propriétés `VPXEncoderSettings`, inclut des paramètres spécifiques à VP9 :
  - `AQMode` (mode de quantification adaptative, utilisant l'enum `VPXAdaptiveQuantizationMode`)
  - `FrameParallelDecoding` (autorise le traitement parallèle)
  - `RowMultithread` (encodage de lignes multithread)
  - `TileColumns`, `TileRows` (valeurs log2)
- **Type d'encodeur** : `VPXEncoderType.VP9`.
- **Disponibilité** : peut être vérifiée à l'aide de `VP9EncoderSettings.IsAvailable()`.

#### QSVVP9EncoderSettings

Encodeur Intel QSV (accéléré GPU) pour VP9.

- **Propriétés clés** :
  - `LowLatency`
  - `TargetUsage` (1 : meilleure qualité, 4 : équilibré, 7 : meilleure vitesse)
  - `Bitrate` (Kbit/sec)
  - `GOPSize`
  - `ICQQuality` (qualité constante intelligente)
  - `MaxBitrate` (Kbit/sec)
  - `QPI`, `QPP` (quantificateur constant pour images I et P)
  - `Profile` (0-3)
  - `RateControl` (utilisant l'enum `QSVVP9EncRateControl`)
  - `RefFrames`
- **Type d'encodeur** : `VPXEncoderType.QSV_VP9`.
- **Disponibilité** : peut être vérifiée à l'aide de `QSVVP9EncoderSettings.IsAvailable()`.

#### CustomVPXEncoderSettings

Permet d'utiliser un élément GStreamer personnalisé pour l'encodage VPX.

- **Propriétés clés** :
  - `ElementName` (chaîne pour spécifier le nom de l'élément GStreamer)
  - `Properties` (Dictionary<string, object> pour configurer l'élément)
  - `VideoFormat` (format vidéo requis comme `VideoFormatX.NV12`)
- **Type d'encodeur** : `VPXEncoderType.CustomEncoder`.

### Énumérations VPX

Plusieurs énumérations sont disponibles pour configurer les encodeurs VPX :

- `VPXAdaptiveQuantizationMode` : définit les modes de quantification adaptative (par ex. `Off`, `Variance`, `Complexity`, `CyclicRefresh`, `Equator360`, `Perceptual`, `PSNR`, `Lookahead`).
- `VPXErrorResilientFlags` : indicateurs pour les fonctionnalités de résilience aux erreurs (par ex. `None`, `Default`, `Partitions`).
- `VPXKeyFrameMode` : définit les stratégies de placement des images clés (par ex. `Auto`, `Disabled`).
- `VPXMultipassMode` : modes pour l'encodage multipasse (par ex. `OnePass`, `FirstPass`, `LastPass`).
- `VPXRateControl` : modes de contrôle de débit (par ex. `VBR`, `CBR`, `CQ`).
- `VPXScalingMode` : modes de mise à l'échelle (par ex. `Normal`, `_4_5`, `_3_5`, `_1_2`).
- `VPXTokenPartitions` : nombre de partitions de tokens (par ex. `One`, `Two`, `Four`, `Eight`).
- `VPXTuning` : options de tune pour l'encodeur (par ex. `PSNR`, `SSIM`).
- `VPXEncoderType` : spécifie la variante d'encodeur VPX (par ex. `VP8`, `VP9`, `QSV_VP9`, `CustomEncoder` et celles spécifiques à une plateforme comme `OMXExynosVP8Encoder`).
- `QSVVP9EncRateControl` : modes de contrôle de débit spécifiques à `QSVVP9EncoderSettings` (par ex. `CBR`, `VBR`, `CQP`, `ICQ`).

### Informations sur le bloc

Nom : VPXEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | VP8/VP9 | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->VPXEncoderBlock;
    VPXEncoderBlock-->WebMSinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var vp8EncoderBlock = new VPXEncoderBlock(new VP8EncoderSettings());
pipeline.Connect(fileSource.VideoOutput, vp8EncoderBlock.Input);

var webmSinkBlock = new WebMSinkBlock(new WebMSinkSettings(@"output.webm"));
pipeline.Connect(vp8EncoderBlock.Output, webmSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur MPEG2

`MPEG-2` : un standard largement utilisé pour la compression vidéo et audio, couramment présent dans les DVD, les diffusions télévisuelles numériques (comme DVB et ATSC) et les SVCD. Il offre une bonne qualité à des débits relativement faibles pour le contenu en définition standard.

### Informations sur le bloc

Nom : MPEG2EncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | video/mpeg | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG2EncoderBlock;
    MPEG2EncoderBlock-->MPEGTSSinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg2EncoderBlock = new MPEG2EncoderBlock(new MPEG2VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg2EncoderBlock.Input);

// Exemple : utilisation d'un MPEGTSSinkBlock pour les fichiers .ts
var mpegtsSinkBlock = new MPEGTSSinkBlock(new MPEGTSSinkSettings(@"output.ts"));
pipeline.Connect(mpeg2EncoderBlock.Output, mpegtsSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux.

## Encodeur MPEG4

`MPEG-4 Part 2 Visual` (souvent simplement appelé MPEG-4 vidéo) est un standard de compression vidéo qui fait partie de la suite MPEG-4. Il est utilisé dans diverses applications, notamment le streaming vidéo, la visioconférence et les disques optiques comme DivX et Xvid.

### Informations sur le bloc

Nom : MPEG4EncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | video/mpeg, mpegversion=4 | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->MPEG4EncoderBlock;
    MPEG4EncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4"; // Fichier d'entrée
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var mpeg4EncoderBlock = new MPEG4EncoderBlock(new MPEG4VideoEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, mpeg4EncoderBlock.Input);

// Exemple : utilisation d'un MP4SinkBlock pour les fichiers .mp4
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output_mpeg4.mp4"));
pipeline.Connect(mpeg4EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux.

## Encodeur PNG

Le bloc encodeur PNG fournit une compression d'image sans perte avec une haute qualité, adaptée à l'archivage, aux captures d'écran et aux applications nécessitant une qualité d'image parfaite avec prise en charge de la transparence.

### Informations sur le bloc

Nom : PNGEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | PNG | 1

### Paramètres

#### PNGEncoderSettings

```csharp
public class PNGEncoderSettings
{
    // Enum de niveau de compression (None / Minimal / Low / Light / Medium / MediumHigh /
    // High / VeryHigh / Maximum). Par défaut MediumHigh.
    public PNGEncoderCompressionLevel CompressionLevel { get; set; }

    // Filtre de pré-compression (None / Sub / Up / Average / Paeth / All). Par défaut None.
    public PNGEncoderFilterType Filter { get; set; }

    // Écrire ou non les chunks d'info. Par défaut true.
    public bool WriteInfo { get; set; }
}
```

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->PNGEncoderBlock;
    PNGEncoderBlock-->FileSink;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4"; // Fichier d'entrée
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var pngSettings = new PNGEncoderSettings
{
    CompressionLevel = PNGEncoderCompressionLevel.MediumHigh, // équilibré
    Filter           = PNGEncoderFilterType.None
};
var pngEncoder = new PNGEncoderBlock(pngSettings, "frame.png");
pipeline.Connect(fileSource.VideoOutput, pngEncoder.Input);

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux, iOS, Android.

## Encodeur Apple ProRes { #apple-prores-encoder }

`Apple ProRes` : un format de compression vidéo avec perte de haute qualité développé par Apple Inc., largement utilisé dans les flux de travail de production et post-production vidéo professionnels pour son excellent équilibre entre qualité d'image et performance.

### Informations sur le bloc

Nom : AppleProResEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | ProRes | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AppleProResEncoderBlock;
    AppleProResEncoderBlock-->MOVSinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var proResEncoderBlock = new AppleProResEncoderBlock(new AppleProResEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, proResEncoderBlock.Input);

var movSinkBlock = new MOVSinkBlock(new MOVSinkSettings(@"output.mov"));
pipeline.Connect(proResEncoderBlock.Output, movSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

macOS, iOS.

### Disponibilité

Vous pouvez vérifier si l'encodeur Apple ProRes est disponible dans votre environnement avec :

```csharp
bool available = AppleProResEncoderBlock.IsAvailable();
```

## Encodeur WMV

### Vue d'ensemble

Le bloc encodeur WMV encode la vidéo au format WMV.

### Informations sur le bloc

Nom : WMVEncoderBlock.

Direction du pin | Type de média | Nombre de pins
--- | :---: | :---:
Entrée | Vidéo non compressée | 1
Sortie | video/x-wmv | 1

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->WMVEncoderBlock;
    WMVEncoderBlock-->ASFSinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline(false);

var filename = "test.mp4";
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri(filename)));

var wmvEncoderBlock = new WMVEncoderBlock(new WMVEncoderSettings());
pipeline.Connect(fileSource.VideoOutput, wmvEncoderBlock.Input);

var asfSinkBlock = new ASFSinkBlock(new ASFSinkSettings(@"output.wmv"));
pipeline.Connect(wmvEncoderBlock.Output, asfSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Windows, macOS, Linux.
