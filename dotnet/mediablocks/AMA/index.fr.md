---
title: Encodage et décodage vidéo matériel AMD AMA en C# .NET
description: Encodez et décodez H.264, HEVC et AV1 sur les accélérateurs AMD Alveo avec l'AMA Video SDK dans VisioForge Media Blocks SDK pour .NET sous Linux.
sidebar_label: AMA
tags:
  - Media Blocks SDK
  - .NET
  - Linux
  - Streaming
  - AV1
  - H.265
primary_api_classes:
  - AMAH264EncoderSettings
  - AMAHEVCEncoderSettings
  - AMAAV1EncoderSettings
  - AMAH264DecoderSettings
  - AMAHEVCDecoderSettings
  - AMAAV1DecoderSettings
  - AMAScalerBlock
  - AMAScalerSettings

---

# Blocs AMD AMA - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

L'**AMD AMA Video SDK** est la plateforme d'accélération multimédia matérielle d'AMD pour les accélérateurs multimédias AMD Alveo. Elle fournit une famille d'éléments GStreamer (`ama_h264enc`, `ama_h265enc`, `ama_av1enc`, `ama_h264dec`, `ama_h265dec`, `ama_av1dec`, `ama_scaler`, `ama_upload`, `ama_download`) qui exécutent l'encodage, le décodage et la mise à l'échelle de H.264, HEVC et AV1 entièrement sur l'accélérateur. VisioForge Media Blocks SDK encapsule cette plateforme afin que vous puissiez décharger le traitement vidéo vers le matériel AMD depuis un pipeline C# standard. La plateforme AMA est distincte d'AMD AMF (qui pilote les GPU Radeon) : AMA cible des accélérateurs multimédias Alveo dédiés, tels que l'**Alveo MA35D**, l'appareil actuellement pris en charge.

**Points clés :**

- **Plateforme :** AMD AMA Video SDK sur les accélérateurs multimédias AMD Alveo (actuellement l'Alveo MA35D).
- **Codecs :** H.264, HEVC (H.265) et AV1 — encodage et décodage matériels.
- **Système d'exploitation :** Linux x86_64 uniquement. Il n'existe pas de prise en charge AMA sous Windows ou macOS.
- **Sélection du périphérique :** la propriété `Device` choisit l'accélérateur — `-1` (par défaut) effectue une sélection automatique, `0`/`1`/`2`… fixent une carte précise lorsque plusieurs périphériques AMD sont présents.
- **Modèle de mémoire :** l'encodage, le décodage et la mise à l'échelle s'exécutent dans la mémoire du périphérique ; le SDK insère automatiquement `ama_upload` / `ama_download` lorsque les données passent depuis ou vers la mémoire système.
- **Distribution :** VisioForge fournit uniquement le wrapper managé. Le runtime AMA (pilote noyau + plugins GStreamer) s'installe séparément depuis AMD ; aucun binaire AMD n'est fourni avec le SDK.

## Prérequis

Avant de pouvoir utiliser les blocs AMA, la machine cible doit disposer de :

- Linux x86_64 (Ubuntu, Alma/RHEL ou Debian selon la matrice de compatibilité de l'AMD AMA SDK).
- Un accélérateur multimédia AMD Alveo installé et provisionné (pilote noyau, huge pages, IOMMU / décodage au-dessus de 4 Go activé dans le firmware de l'hôte).
- L'AMD AMA Video SDK installé, y compris ses plugins GStreamer, afin que les éléments `ama_*` soient détectables par GStreamer.

Chaque classe de configuration et de bloc AMA expose une méthode statique `IsAvailable()` qui vérifie la présence des éléments `ama_*` requis. Appelez-la avant de construire un pipeline et repliez-vous sur un encodeur logiciel ou d'un autre fournisseur lorsqu'elle renvoie `false` :

```csharp
if (AMAH264EncoderSettings.IsAvailable())
{
    // L'encodeur H.264 AMA est présent : utilisez l'encodage matériel.
}
```

## Encodeur H.264 AMA

Encodeur H.264 matériel reposant sur l'élément `ama_h264enc`. Configurez-le avec `AMAH264EncoderSettings` (implémente `IH264EncoderSettings`) et passez-le à un `H264EncoderBlock` standard.

### Informations sur le bloc

Nom : H264EncoderBlock (avec `AMAH264EncoderSettings`).

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

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// Encodeur H.264 matériel AMD AMA (Alveo). Device = -1 sélectionne automatiquement l'accélérateur.
var h264Settings = new AMAH264EncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    RateControl = AMARateControl.CBR
};
var h264EncoderBlock = new H264EncoderBlock(h264Settings);
pipeline.Connect(fileSource.VideoOutput, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Linux (x86_64) avec l'AMD AMA Video SDK.

## Encodeur HEVC AMA

Encodeur HEVC (H.265) matériel reposant sur l'élément `ama_h265enc`. Configurez-le avec `AMAHEVCEncoderSettings` (implémente `IHEVCEncoderSettings`) et passez-le à un `HEVCEncoderBlock` standard.

### Informations sur le bloc

Nom : HEVCEncoderBlock (avec `AMAHEVCEncoderSettings`).

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

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var hevcSettings = new AMAHEVCEncoderSettings
{
    Device = -1,
    Bitrate = 6000, // Kbps
    TuneMetrics = AMATuneMetrics.VMAF
};
var hevcEncoderBlock = new HEVCEncoderBlock(hevcSettings);
pipeline.Connect(fileSource.VideoOutput, hevcEncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(hevcEncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Linux (x86_64) avec l'AMD AMA Video SDK.

## Encodeur AV1 AMA

Encodeur AV1 matériel reposant sur l'élément `ama_av1enc`. Configurez-le avec `AMAAV1EncoderSettings` (implémente `IAV1EncoderSettings`) et passez-le à un `AV1EncoderBlock` standard. AV1 ajoute une propriété `DeviceType` (`AMAAV1DeviceType`) qui sélectionne le moteur AV1.

### Informations sur le bloc

Nom : AV1EncoderBlock (avec `AMAAV1EncoderSettings`).

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

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

var av1Settings = new AMAAV1EncoderSettings
{
    Device = -1,
    DeviceType = AMAAV1DeviceType.Type1,
    Bitrate = 6000 // Kbps
};
var av1EncoderBlock = new AV1EncoderBlock(av1Settings);
pipeline.Connect(fileSource.VideoOutput, av1EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(av1EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Linux (x86_64) avec l'AMD AMA Video SDK.

## Configuration commune de l'encodeur

Les trois classes de configuration des encodeurs AMA partagent les mêmes propriétés principales (AV1 expose en plus `DeviceType`, et sa plage de QP est 0–255 au lieu de 0–51) :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Accélérateur sur lequel encoder. −1 sélectionne automatiquement. |
| `Slice` | int | −1 | Sous-moteur (slice) du périphérique. −1 = automatique. |
| `Bitrate` | uint | 5000 | Débit cible en Kbps. |
| `MaxBitrate` | int | −1 | Débit maximal en Kbps pour VBR/CVBR. −1 = automatique. |
| `RateControl` | `AMARateControl` | Auto | Mode de contrôle de débit. |
| `QPMode` | `AMAQPMode` | Auto | Mode de contrôle de QP. |
| `QP` | int | −1 | QP fixe en mode QP constant. −1 = désactivé. |
| `MinQP` / `MaxQP` | int | 0 / 51 (255 pour AV1) | Plage de QP autorisée. |
| `BFrames` | int | −1 | Images B entre les images P. −1 = automatique. |
| `GOPLength` | int | −1 | Distance maximale entre images I. −1 = automatique. |
| `Tier` | `AMATier` | Auto | Tier d'encodage. |
| `TuneMetrics` | `AMATuneMetrics` | VQ | Métrique de qualité objective à optimiser. |
| `SpatialAQ` / `TemporalAQ` | bool | true | Commutateurs de quantification adaptative (avec `*Gain` 0–255). |
| `LookaheadDepth` | int | −1 | Profondeur de look-ahead. −1 = automatique. |
| `LatencyMs` | int | −1 | Latence cible de l'encodeur (ms). −1 = automatique. |

## Décodeurs AMA

Décodeurs matériels pour H.264 (`ama_h264dec`), HEVC (`ama_h265dec`) et AV1 (`ama_av1dec`). Configurez-les avec `AMAH264DecoderSettings` / `AMAHEVCDecoderSettings` / `AMAAV1DecoderSettings` (qui implémentent `IH264/HEVC/AV1DecoderSettings`) et passez-les au bloc décodeur correspondant. Lorsqu'un décodeur alimente un consommateur en mémoire système, le SDK ajoute automatiquement `ama_download`.

### Informations sur le bloc

Nom : `H264DecoderBlock` / `HEVCDecoderBlock` / `AV1DecoderBlock` (avec un objet de configuration AMA).

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo d'entrée | Vidéo encodée | 1 |
| Vidéo de sortie | Vidéo non compressée | 1 |

### Configuration

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Device` | int | −1 | Accélérateur sur lequel décoder. −1 sélectionne automatiquement. |
| `LowLatency` | bool | false | Active le décodage à faible latence. |
| `LatencyLogging` | bool | false | Journalise les informations de latence dans syslog. |
| `AllowDownscaling` | bool | false | Autorise la réduction d'échelle au niveau du décodeur. |

### Exemple de pipeline

```mermaid
graph LR;
    BasicFileSourceBlock-->UniversalDemuxBlock;
    UniversalDemuxBlock-->HEVCDecoderBlock;
    HEVCDecoderBlock-->VideoRendererBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

// Décodeur HEVC matériel AMD AMA (Alveo).
var hevcDecoder = new HEVCDecoderBlock(new AMAHEVCDecoderSettings { Device = -1 });

var basicFileSource = new BasicFileSourceBlock("test_hevc.mp4");
var reader = new MediaInfoReaderX();
await reader.OpenAsync("test_hevc.mp4");
var mediaInfo = reader.Info;
var universalDemux = new UniversalDemuxBlock(mediaInfo, renderVideo: true, renderAudio: false);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(basicFileSource.Output, universalDemux.Input);
pipeline.Connect(universalDemux.GetVideoOutput(), hevcDecoder.Input);
pipeline.Connect(hevcDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

### Plateformes

Linux (x86_64) avec l'AMD AMA Video SDK.

## Redimensionneur AMA

Le `AMAScalerBlock` redimensionne la vidéo (et, en option, divise par deux la fréquence d'images) sur l'accélérateur à l'aide de l'élément `ama_scaler`. Le bloc encapsule la chaîne `ama_upload ! ama_scaler ! capsfilter ! ama_download`, de sorte qu'il accepte des images en mémoire système en entrée et émet des images en mémoire système en aval : vous pouvez l'insérer dans n'importe quel pipeline sans gérer vous-même la mémoire du périphérique.

### Informations sur le bloc

Nom : `AMAScalerBlock`.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Vidéo d'entrée | Vidéo (mémoire système) | 1 |
| Vidéo de sortie | Vidéo (mémoire système) | 1 |

### Configuration

`AMAScalerSettings` :

| Propriété | Type | Par défaut | Description |
| --- | --- | :---: | --- |
| `Width` / `Height` | int | 0 | Taille de sortie cible en pixels (144–7580). 0 laisse la dimension sans contrainte. |
| `Device` | int | −1 | Accélérateur sur lequel redimensionner. −1 sélectionne automatiquement. |
| `Framerate` | `AMAScalerFramerate` | Auto | Mode de fréquence d'images de sortie (Auto / Full / Half). |

### Exemple de pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->AMAScalerBlock;
    AMAScalerBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

### Exemple de code

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("test.mp4"));

// Redimensionner à 1280x720 sur l'accélérateur AMD.
var scaler = new AMAScalerBlock(new AMAScalerSettings(1280, 720));
pipeline.Connect(fileSource.VideoOutput, scaler.Input);

var h264EncoderBlock = new H264EncoderBlock(new AMAH264EncoderSettings());
pipeline.Connect(scaler.Output, h264EncoderBlock.Input);

var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();
```

### Plateformes

Linux (x86_64) avec l'AMD AMA Video SDK.

## Transcodage matériel complet

Un décodeur AMA, le redimensionneur AMA et un encodeur AMA chaînés exécutent chaque étape — décodage, mise à l'échelle et encodage — sur l'accélérateur AMD. Chaque bloc est autonome : il accepte et émet des images en mémoire système, de sorte que le SDK déplace les données vers et depuis le périphérique (`ama_download` / `ama_upload`) à chaque limite de bloc, tandis que le travail de décodage, de mise à l'échelle et d'encodage s'exécute sur l'accélérateur :

```mermaid
graph LR;
    Source-->AMADecoder-->AMAScaler-->AMAEncoder-->Sink;
```

Enchaînez dans l'ordre un `H264DecoderBlock` (avec `AMAH264DecoderSettings`), un `AMAScalerBlock` et un `HEVCEncoderBlock` (avec `AMAHEVCEncoderSettings`) pour transcoder H.264 → HEVC redimensionné, chaque étape s'exécutant sur l'accélérateur AMD.

## Référence des énumérations

| Énumération | Membres |
| --- | --- |
| `AMARateControl` | Auto, CQP, CBR, VBR, CVBR |
| `AMAQPMode` | Auto, RelativeLoad, Uniform |
| `AMATier` | Auto, Main, High |
| `AMATuneMetrics` | VQ, PSNR, SSIM, VMAF |
| `AMAAV1DeviceType` | Any, Type1, Type2 |
| `AMAScalerFramerate` | Auto, Full, Half |

## Sélection du périphérique

Chaque objet de configuration AMA (`AMA*EncoderSettings`, `AMA*DecoderSettings` et `AMAScalerSettings`) expose une propriété `Device` qui décide quel accélérateur AMD exécute le travail — définissez-la sur l'objet de configuration que vous passez au bloc (les blocs eux-mêmes n'ont pas de propriété `Device`) :

- `Device = -1` (par défaut) : le runtime AMA sélectionne automatiquement un accélérateur disponible.
- `Device = 0` / `1` / `2`… : fixe le travail à une carte précise, ce qui vous permet de choisir un accélérateur Alveo lorsque plusieurs périphériques AMD sont installés dans le même hôte.

Comme AMA cible des accélérateurs multimédias Alveo dédiés plutôt qu'un GPU Radeon, la sélection d'une classe de configuration AMA dirige déjà le travail vers l'accélérateur, et non vers un quelconque graphique AMD intégré.

## Limitations

- **Linux x86_64 uniquement.** Les éléments AMA n'existent pas sous Windows ou macOS ; les classes de configuration ne sont compilées que dans la version Linux du SDK.
- **Pipeline Media Blocks uniquement.** Les encodeurs AMA s'exécutent sur le pipeline Media Blocks direct. Ils ne sont pas pris en charge sur la voie de sortie encodebin de `VideoEditCoreX` et y lèvent une `NotSupportedException` : construisez plutôt l'encodage AMA avec `H264EncoderBlock` / `HEVCEncoderBlock` / `AV1EncoderBlock`.
- Le runtime AMA doit être installé séparément (voir [Prérequis](#prerequis)) ; le SDK ne fournit aucun binaire AMD.

## Foire aux questions

### VisioForge prend-il en charge l'encodage matériel AMD AMA et Alveo ?

Oui. VisioForge Media Blocks SDK encapsule l'AMD AMA Video SDK pour fournir l'encodage et le décodage matériels de H.264, HEVC et AV1, ainsi qu'un redimensionneur matériel, sur les accélérateurs multimédias AMD Alveo. La prise en charge est réservée à Linux x86_64 et nécessite l'installation de l'AMD AMA Video SDK.

### Comment choisir sur quel périphérique AMD l'encodeur AMA s'exécute ?

Définissez la propriété `Device` sur l'objet de configuration AMA. `-1` (la valeur par défaut) sélectionne automatiquement un accélérateur disponible ; `0`, `1`, `2`, etc., fixent l'encodage, le décodage ou la mise à l'échelle à une carte précise lorsque plusieurs périphériques AMD sont présents dans l'hôte.

### L'accélération matérielle AMA est-elle disponible sous Windows ou macOS ?

Non. L'AMD AMA Video SDK est réservé à Linux x86_64, de sorte que les blocs encodeur, décodeur et redimensionneur AMA ne sont disponibles que sous Linux. Sous Windows ou macOS, utilisez plutôt les encodeurs AMF, NVENC, QSV ou logiciels.

### Quelle est la différence entre les encodeurs AMD AMF et AMD AMA ?

AMD AMF pilote les GPU Radeon et est disponible sous Windows, Linux et macOS via des classes telles que `AMFH264EncoderSettings`. AMD AMA est une plateforme distincte qui pilote des accélérateurs multimédias Alveo dédiés (comme le MA35D) sous Linux, exposés via les classes `AMA*`. Ils ciblent un matériel différent et se configurent avec des classes de configuration distinctes.

### Comment exécuter un transcodage complet sur l'accélérateur AMD ?

Enchaînez un décodeur AMA, le `AMAScalerBlock` et un encodeur AMA. Chaque étape s'exécute sur l'accélérateur AMD ; les blocs échangent les images via la mémoire système, et le SDK déplace automatiquement les données vers et depuis le périphérique (`ama_download` / `ama_upload`) à chaque limite.

## Voir aussi

- [Encodeurs vidéo](../VideoEncoders/index.md) : tous les blocs encodeurs H.264, HEVC, AV1 et VP9, y compris les encodeurs matériels AMD AMF, NVENC et QSV.
- [Décodeurs vidéo](../VideoDecoders/index.md) : blocs décodeurs matériels et logiciels pour tous les codecs pris en charge.
- [Blocs spécifiques à Linux](../_Linux/index.md) : blocs et fonctionnalités disponibles dans les versions Linux du SDK.
- [Guide d'encodage HEVC](../../general/video-encoders/hevc.md) : encodage HEVC matériel sur AMD, NVIDIA et Intel.
- [Guide d'encodage AV1](../../general/video-encoders/av1.md) : options d'encodage AV1 matérielles et logicielles.
