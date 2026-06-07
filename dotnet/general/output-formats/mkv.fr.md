---
title: Format conteneur MKV — encodage et enregistrement vidéo .NET
description: Implémentez la sortie MKV en .NET avec encodage accéléré matériellement, pistes audio multiples et conteneur Matroska flexible pour vos apps.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Recording
  - Encoding
  - Editing
  - MKV
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - MKVOutput
  - VideoCaptureCoreX
  - VideoEditCoreX
  - MediaBlocksPipeline
  - NVENCH264EncoderSettings

---

# Sortie MKV dans les SDK VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

## Introduction au format MKV

MKV (Matroska Video) est un format de conteneur flexible et standard ouvert qui peut contenir un nombre illimité de pistes vidéo, audio et de sous-titres dans un seul fichier. Les SDK VisioForge offrent une prise en charge robuste de la sortie MKV avec diverses options d'encodage pour répondre à des exigences de développement variées.

Ce format est particulièrement précieux pour les développeurs travaillant sur des applications nécessitant :

- Plusieurs pistes audio ou langues
- Vidéo haute qualité avec plusieurs options de codec
- Compatibilité multiplateforme
- Prise en charge des métadonnées et des chapitres

## Premiers pas avec la sortie MKV

La classe `MKVOutput` sert d'interface principale pour générer des fichiers MKV dans les SDK VisioForge. Vous pouvez l'initialiser avec les paramètres par défaut ou spécifier des encodeurs personnalisés pour répondre aux besoins de votre application.

### Implémentation de base

```csharp
// Créer une sortie MKV avec les encodeurs par défaut
var mkvOutput = new MKVOutput("output.mkv");
```

Ou spécifiez des encodeurs personnalisés lors de l'initialisation (les deuxième et troisième paramètres du constructeur sont `IVideoEncoder` / `IAudioEncoder`, tous deux nullables) :

```csharp
var videoEncoder = new NVENCH264EncoderSettings();
var audioEncoder = new MFAACEncoderSettings();
var mkvOutput = new MKVOutput("output.mkv", videoEncoder, audioEncoder);
```

## Options d'encodage vidéo

Le format MKV prend en charge plusieurs codecs vidéo, offrant aux développeurs une flexibilité pour équilibrer qualité, performance et compatibilité. Les SDK VisioForge offrent à la fois des encodeurs logiciels et accélérés matériellement.

### Options d'encodeur H.264

H.264 reste l'un des codecs vidéo les plus largement pris en charge, offrant une excellente compression et qualité. Pour les options de configuration détaillées, consultez la [documentation de l'encodeur H.264](../video-encoders/h264.md).

- **OpenH264** : Encodeur logiciel, utilisé par défaut lorsque l'accélération matérielle n'est pas disponible
- **NVENC H.264** : Encodage accéléré par GPU NVIDIA pour des performances supérieures
- **QSV H.264** : Technologie Intel Quick Sync Video pour l'accélération matérielle
- **AMF H.264** : Option d'encodage accéléré par GPU AMD

### Options d'encodeur HEVC (H.265)

Pour les applications nécessitant une efficacité de compression supérieure ou du contenu 4K, consultez la [documentation de l'encodeur HEVC](../video-encoders/hevc.md) pour des paramètres détaillés :

- **MF HEVC** : Implémentation Windows Media Foundation (Windows uniquement)
- **NVENC HEVC** : Accélération GPU NVIDIA pour H.265
- **QSV HEVC** : Implémentation Intel Quick Sync pour H.265
- **AMF HEVC** : Accélération GPU AMD pour l'encodage H.265

### Définir un encodeur vidéo

```csharp
mkvOutput.Video = new NVENCH264EncoderSettings();
```

## Options d'encodage audio

La qualité audio est tout aussi importante pour la plupart des applications. Les SDK VisioForge fournissent plusieurs options d'encodeur audio pour la sortie MKV :

### Codecs audio pris en charge

- **Encodeurs AAC** — Consultez la [documentation de l'encodeur AAC](../audio-encoders/aac.md) :
  - **VO AAC** : Choix par défaut pour les plateformes non Windows
  - **AVENC AAC** : Implémentation AAC FFMPEG
  - **MF AAC** : Implémentation Windows Media Foundation (par défaut sur Windows)
  
- **Formats audio alternatifs** :
  - **[MP3](../audio-encoders/mp3.md)** : Format courant largement compatible
  - **[Vorbis](../audio-encoders/vorbis.md)** : Codec audio open source
  - **[OPUS](../audio-encoders/opus.md)** : Codec moderne avec un excellent rapport qualité/taille

### Configuration de l'encodage audio

```csharp
// Sélection d'encodeur audio spécifique à la plateforme
#if NET_WINDOWS
    var aacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 48000
    };
    mkvOutput.Audio = aacSettings;
#else
    var aacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 44100
    };
    mkvOutput.Audio = aacSettings;
#endif

// Ou utiliser OPUS pour une meilleure qualité à des débits inférieurs (Bitrate en Kbit/s ; le nombre de canaux
// est déduit des caps d'entrée, il n'y a donc pas de propriété Channels sur OPUSEncoderSettings)
var opusSettings = new OPUSEncoderSettings
{
    Bitrate = 128
};
mkvOutput.Audio = opusSettings;
```

## Configuration MKV avancée

### Traitement vidéo et audio personnalisé

Pour les applications nécessitant un traitement spécial, vous pouvez intégrer des processeurs MediaBlock personnalisés :

```csharp
// Ajouter un processeur vidéo pour des effets ou transformations
var textOverlayBlock = new TextOverlayBlock(new TextOverlaySettings("Hello world!"));
mkvOutput.CustomVideoProcessor = textOverlayBlock;

// Ajouter un traitement audio
var volumeBlock = new VolumeBlock() { Level = 1.2 }; // Augmenter le volume de 20 %
mkvOutput.CustomAudioProcessor = volumeBlock;
```

### Gestion des paramètres de puits

Contrôlez les propriétés du fichier de sortie via les paramètres du puits :

```csharp
// Changer le nom du fichier de sortie
mkvOutput.Sink.Filename = "processed_output.mkv";

// Obtenir le nom de fichier actuel
string currentFile = mkvOutput.GetFilename();

// Mettre à jour le nom du fichier avec un horodatage
string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
mkvOutput.SetFilename($"recording_{timestamp}.mkv");
```

## Intégration avec les composants du SDK VisioForge

### Avec Video Capture SDK

```csharp
// Initialiser le core de capture
var captureCore = new VideoCaptureCoreX();

// Configurer la source vidéo et audio
// ...

// Ajouter la sortie MKV au pipeline d'enregistrement
var mkvOutput = new MKVOutput("capture.mkv");
captureCore.Outputs_Add(mkvOutput, true);

// Démarrer l'enregistrement
await captureCore.StartAsync();
```

### Avec Video Edit SDK

```csharp
// Initialiser le core d'édition
var editCore = new VideoEditCoreX();

// Ajouter les sources d'entrée
// ...

// Configurer la sortie MKV avec accélération matérielle
var h265Encoder = new NVENCHEVCEncoderSettings
{
    Bitrate = 10000
};
var mkvOutput = new MKVOutput("edited.mkv", h265Encoder);
editCore.Output_Format = mkvOutput;

// Traiter le fichier
await editCore.StartAsync();
```

### Avec Media Blocks SDK

`MKVOutputBlock` prend des **paramètres** d'encodeur (et non des blocs d'encodeur) dans son constructeur et construit la chaîne d'encodage en interne. Connectez la source audio/vidéo directement à ses pads d'entrée.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Outputs;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types;

// Pipeline.
var pipeline = new MediaBlocksPipeline();

// Source (n'importe quel fichier — il sera décodé en sorties vidéo et audio séparées).
var sourceSettings = await UniversalSourceSettings.CreateAsync("input.mp4");
var sourceBlock = new UniversalSourceBlock(sourceSettings);

// Puits MKV — le constructeur accepte des paramètres d'encodeur, pas des blocs d'encodeur.
var mkvSinkSettings = new MKVSinkSettings("processed.mkv");
var mkvOutput = new MKVOutputBlock(
    mkvSinkSettings,
    videoSettings: new OpenH264EncoderSettings(),
    audioSettings: new VOAACEncoderSettings());

// Câbler les pads de la source directement à la sortie MKV — elle gère l'encodage en interne.
pipeline.Connect(sourceBlock.VideoOutput, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(sourceBlock.AudioOutput, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

## Avantages de l'accélération matérielle

L'encodage accéléré matériellement offre des avantages significatifs aux développeurs construisant des applications en temps réel ou de traitement par lots :

1. **Charge CPU réduite** : Décharge l'encodage sur du matériel dédié
2. **Traitement plus rapide** : Amélioration des performances jusqu'à 5 à 10 fois
3. **Efficacité énergétique** : Consommation d'énergie plus faible, importante pour les apps mobiles
4. **Qualité supérieure** : Certains encodeurs matériels offrent une meilleure qualité par débit binaire

## Bonnes pratiques pour les développeurs

Lors de l'implémentation de la sortie MKV dans vos applications, prenez en compte ces recommandations :

1. **Vérifiez toujours la disponibilité matérielle** avant d'utiliser des encodeurs accélérés par GPU
2. **Sélectionnez des débits binaires appropriés** en fonction du type de contenu et de la résolution
3. **Utilisez des encodeurs spécifiques à la plateforme** lorsque c'est possible pour des performances optimales
4. **Testez sur les plateformes cibles** pour garantir la compatibilité
5. **Tenez compte des compromis qualité/taille** en fonction des besoins de votre application

## Conclusion

Le format MKV fournit aux développeurs un conteneur flexible et robuste pour le contenu vidéo dans les applications .NET. Avec les SDK VisioForge, vous pouvez tirer parti de l'accélération matérielle, des options d'encodage avancées et du traitement personnalisé pour créer des applications vidéo haute performance.

En comprenant les encodeurs et options de configuration disponibles, vous pouvez optimiser votre implémentation pour des plateformes matérielles spécifiques tout en maintenant une compatibilité multiplateforme lorsque c'est nécessaire.
