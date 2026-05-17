---
title: Capture vidéo vers MPEG-TS avec encodage matériel en C# .NET
description: Capturez vidéo et audio vers MPEG-TS en C# avec accélération matérielle, sélection de format et prise en charge multiplateforme.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Webcam
  - HLS
  - TS
  - H.264
  - AAC
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - MediaBlocksPipeline
  - DeviceEnumerator
  - MediaBlockPadMediaType
  - VideoView

---

# Capture vidéo vers MPEG-TS en C# et .NET : guide complet

## Introduction

Ce guide technique montre comment capturer de la vidéo TS en C# depuis des caméras et des microphones en utilisant deux puissantes solutions multimédias VisioForge : Video Capture SDK .NET avec le moteur VideoCaptureCoreX et Media Blocks SDK .NET avec le moteur MediaBlocksPipeline. Les deux SDK offrent des capacités robustes pour capturer, enregistrer et éditer des fichiers TS (MPEG Transport Stream) dans des applications .NET. Nous explorerons des exemples de code détaillés pour implémenter la capture vidéo/audio vers TS en C# avec des performances et une qualité optimisées.

## Installation et déploiement

Veuillez consulter le [guide d'installation](../../install/index.md) pour des instructions détaillées sur l'installation des SDK VisioForge .NET sur votre système.

## Video Capture SDK .NET (VideoCaptureCoreX) — Capturer MPEG-TS en C#

VideoCaptureCoreX fournit une approche simplifiée pour capturer de la vidéo et de l'audio TS en C#. Son architecture orientée composants gère le pipeline multimédia complexe, ce qui permet aux développeurs de se concentrer sur la configuration plutôt que sur les détails d'implémentation de bas niveau lorsqu'ils travaillent avec des fichiers TS en .NET.

### Composants principaux

1. **VideoCaptureCoreX** : moteur principal pour gérer la capture vidéo, le rendu et la sortie TS.
2. **VideoView** : composant d'interface utilisateur pour le rendu vidéo en temps réel pendant la capture.
3. **DeviceEnumerator** : classe pour découvrir les périphériques vidéo/audio.
4. **VideoCaptureDeviceSourceSettings** : configuration pour l'entrée caméra lors de la capture MPEG-TS.
5. **AudioRendererSettings** : configuration pour la lecture audio avec prise en charge AAC.
6. **MPEGTSOutput** : configuration spécifiquement pour la sortie de fichiers MPEG-TS.

### Exemple d'implémentation

Voici une implémentation C# complète pour capturer et enregistrer des fichiers MPEG-TS :

```csharp
// Instance de la classe pour le moteur de capture vidéo
VideoCaptureCoreX videoCapture;

private async Task StartCaptureAsync()
{
    // Initialiser le SDK VisioForge
    await VisioForgeX.InitSDKAsync();

    // Créer l'instance VideoCaptureCoreX et l'associer au contrôle UI VideoView
    videoCapture = new VideoCaptureCoreX(videoView: VideoView1);

    // Obtenir la liste des périphériques de capture vidéo disponibles
    var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

    // Initialiser les paramètres de la source vidéo
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;

    // Obtenir le premier périphérique de capture vidéo disponible
    var videoDevice = videoSources[0];

    // Tenter d'obtenir les capacités HD de résolution et de fréquence d'images depuis le périphérique
    var videoFormat = videoDevice.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    if (videoFormat != null)
    {
        // Configurer la source vidéo avec le format HD
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
        {
            Format = videoFormat.ToFormat()
        };

        // Définir la fréquence d'images de capture
        videoSourceSettings.Format.FrameRate = frameRate;
    }

    // Configurer le périphérique de capture vidéo avec les paramètres
    videoCapture.Video_Source = videoSourceSettings;

    // Configurer la capture audio (microphone)

    // Initialiser les paramètres de la source audio
    IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;

    // Obtenir les périphériques de capture audio disponibles via l'API DirectSound
    var audioApi = AudioCaptureDeviceAPI.DirectSound;
    var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(audioApi);

    // Obtenir le premier périphérique de capture audio disponible
    var audioDevice = audioDevices[0];
    if (audioDevice != null)
    {
        // Obtenir le format audio par défaut pris en charge par le périphérique
        var audioFormat = audioDevice.GetDefaultFormat();
        if (audioFormat != null)
        {
            // Configurer la source audio avec le format par défaut
            audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);
        }
    }

    // Configurer le périphérique de capture audio avec les paramètres
    videoCapture.Audio_Source = audioSourceSettings;

    // Configurer le périphérique de lecture audio
    // Obtenir le premier périphérique de sortie audio disponible
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())[0];

    // Configurer le moteur de rendu audio pour la lecture via le périphérique sélectionné
    videoCapture.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

    // Activer la surveillance et l'enregistrement audio
    videoCapture.Audio_Play = true;    // Activer la surveillance audio en temps réel
    videoCapture.Audio_Record = true;   // Activer l'enregistrement audio vers le fichier de sortie

    // Configurer la sortie MPEG Transport Stream
    var mpegtsOutput = new MPEGTSOutput("output.ts");

    // Configurer l'encodeur vidéo avec accélération matérielle si disponible
    if (NVENCH264EncoderSettings.IsAvailable())
    {
        // Utiliser l'encodeur matériel NVIDIA
        mpegtsOutput.Video = new NVENCH264EncoderSettings();
    }
    else if (QSVH264EncoderSettings.IsAvailable())
    {
        // Utiliser l'encodeur matériel Intel Quick Sync
        mpegtsOutput.Video = new QSVH264EncoderSettings();
    }
    else if (AMFH264EncoderSettings.IsAvailable())
    {
        // Utiliser l'encodeur matériel AMD
        mpegtsOutput.Video = new AMFH264EncoderSettings();
    }
    else
    {
        // Repli vers l'encodeur logiciel
        mpegtsOutput.Video = new OpenH264EncoderSettings();
    }

    // Configurer l'encodeur audio pour la sortie MPEG-TS
    // mpegtsOutput.Audio = ...

    // Ajouter la sortie MPEG-TS au pipeline de capture
    // autostart: true signifie que la sortie démarre automatiquement avec la capture
    videoCapture.Outputs_Add(mpegtsOutput, autostart: true);

    // Démarrer le processus de capture
    await videoCapture.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Arrêter toute la capture et l'encodage
    await videoCapture.StopAsync();

    // Nettoyer les ressources
    await videoCapture.DisposeAsync();
}
```

### Fonctionnalités avancées de VideoCaptureCoreX pour l'enregistrement MPEG-TS

1. **Accélération matérielle** : prise en charge de l'encodage matériel NVIDIA (NVENC), Intel (QSV) et AMD (AMF).
2. **Sélection de format** : le SDK donne accès aux formats natifs et aux fréquences d'images des caméras.
3. **Configuration audio** : fournit le contrôle du volume et la sélection du format.
4. **Sorties multiples** : capacité à ajouter plusieurs formats de sortie simultanément.

## Media Blocks SDK .NET (MediaBlocksPipeline) — Capturer TS en C#

Le moteur MediaBlocksPipeline du Media Blocks SDK .Net adopte une approche architecturale différente, axée sur un système modulaire basé sur des blocs où chaque composant (bloc) a des responsabilités spécifiques dans le pipeline de traitement multimédia.

### Blocs principaux

1. **MediaBlocksPipeline** : le conteneur et contrôleur principal du pipeline de blocs multimédias.
2. **SystemVideoSourceBlock** : capture la vidéo depuis les webcams.
3. **SystemAudioSourceBlock** : capture l'audio depuis les microphones.
4. **VideoRendererBlock** : effectue le rendu de la vidéo vers un contrôle VideoView.
5. **AudioRendererBlock** : gère la lecture audio.
6. **TeeBlock** : divise les flux multimédias pour un traitement simultané (par exemple, affichage et encodage).
7. **H264EncoderBlock** : encode la vidéo en H.264.
8. **AACEncoderBlock** : encode l'audio en AAC.
9. **MPEGTSSinkBlock** : enregistre les flux encodés dans un fichier MPEG-TS.

### Exemple d'implémentation

Voici comment implémenter une capture avancée de fichiers TS en C# :

```csharp
// Instance du pipeline
MediaBlocksPipeline pipeline;

private async Task StartCaptureAsync()
{
    // Initialiser le SDK
    await VisioForgeX.InitSDKAsync();

    // Créer une nouvelle instance du pipeline
    pipeline = new MediaBlocksPipeline();

    // Obtenir le premier périphérique vidéo disponible et configurer le format HD
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var formatItem = device.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = formatItem.ToFormat()
    };
    videoSourceSettings.Format.FrameRate = frameRate;

    // Créer le bloc source vidéo avec les paramètres configurés
    var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

    // Obtenir le premier périphérique audio disponible et configurer le format par défaut
    var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
    var audioFormat = audioDevice.GetDefaultFormat();
    var audioSourceSettings = audioDevice.CreateSourceSettings(audioFormat);
    var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

    // Créer le bloc de rendu vidéo et le connecter au contrôle UI VideoView
    var videoRenderer = new VideoRendererBlock(pipeline, videoView: VideoView1) { IsSync = false };

    // Créer le bloc de rendu audio pour la lecture
    var audioRenderer = new AudioRendererBlock() { IsSync = false };

    // Note : IsSync est false pour maximiser les performances d'encodage

    // Créer les tees vidéo et audio
    var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);
    var audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio);

    // Créer le multiplexeur MPEG-TS
    var muxer = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));

    // Créer les encodeurs vidéo et audio avec accélération matérielle si disponible
    var videoEncoder = new H264EncoderBlock();
    var audioEncoder = new AACEncoderBlock();

    // Connecter les blocs de traitement vidéo :
    // Source -> Tee -> Renderer (aperçu) et Encoder -> Muxer
    pipeline.Connect(videoSource.Output, videoTee.Input);
    pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
    pipeline.Connect(videoTee.Outputs[1], videoEncoder.Input);
    pipeline.Connect(videoEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video));

    // Connecter les blocs de traitement audio :
    // Source -> Tee -> Renderer (lecture) et Encoder -> Muxer
    pipeline.Connect(audioSource.Output, audioTee.Input);
    pipeline.Connect(audioTee.Outputs[0], audioRenderer.Input);
    pipeline.Connect(audioTee.Outputs[1], audioEncoder.Input);
    pipeline.Connect(audioEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio));

    // Démarrer le traitement du pipeline
    await pipeline.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Arrêter tout le traitement du pipeline
    await pipeline.StopAsync();

    // Nettoyer les ressources
    await pipeline.DisposeAsync();
    pipeline = null;
}
```

### Fonctionnalités avancées de MediaBlocksPipeline

1. **Contrôle granulaire** : contrôle direct sur chaque étape de traitement dans le pipeline.
2. **Construction dynamique du pipeline** : capacité à créer des pipelines de traitement complexes en connectant des blocs.
3. **Chemins de traitement multiples** : utilisation de TeeBlock pour diviser les flux vers différents chemins de traitement.
4. **Blocs personnalisés** : capacité à créer et à intégrer des blocs de traitement personnalisés.
5. **Gestion d'erreurs granulaire** : événements d'erreur au niveau de chaque bloc.

## Configuration de la sortie TS avec audio AAC

Les deux SDK offrent une prise en charge robuste de la sortie MPEG-TS, particulièrement utile pour les applications de radiodiffusion et de streaming en raison de sa résilience aux erreurs et de sa faible latence.

### Découpe de fichiers pour l'enregistrement MPEG-TS

Les deux SDK prennent en charge la découpe automatique des fichiers pour la sortie MPEG-TS, permettant l'enregistrement segmenté basé sur la durée, la taille du fichier ou le timecode. Ceci est essentiel pour :

- **Streaming HLS** : créer des fichiers segmentés pour HTTP Live Streaming
- **Gestion du stockage** : limiter la taille des fichiers et implémenter des tampons circulaires
- **Fonctionnalité DVR** : enregistrer des flux continus avec rotation automatique des fichiers
- **Enregistrement time-lapse** : diviser les enregistrements à intervalles réguliers

#### Utilisation de la découpe de fichiers avec VideoCaptureCoreX

```csharp
using VisioForge.Core.Types.X.Sinks;

// Créer les paramètres de découpe pour des segments de 5 minutes
var mpegtsOutput = new MPEGTSOutput("recording_%05d.ts", useAAC: true);
mpegtsOutput.Sink = new MPEGTSSplitSinkSettings("recording_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(5),  // Nouveau fichier toutes les 5 minutes
    SplitMaxFiles = 12,  // Ne conserver que les 12 derniers fichiers (1 heure au total)
    M2TSMode = false
};

// Ajouter au pipeline de capture
videoCapture.Outputs_Add(mpegtsOutput, autostart: true);
```

#### Utilisation de la découpe de fichiers avec MediaBlocksPipeline

```csharp
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.MediaBlocks.Sinks;

// Créer le multiplexeur MPEG-TS avec les paramètres de découpe
var splitSettings = new MPEGTSSplitSinkSettings("output_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(10),  // Découper toutes les 10 minutes
    SplitFileSize = 100 * 1024 * 1024,  // Ou lorsque le fichier atteint 100 Mo
    SplitMaxFiles = 6,  // Conserver les 6 derniers fichiers
    StartIndex = 0
};

var muxer = new MPEGTSSinkBlock(splitSettings);

// Connecter les encodeurs vidéo et audio au multiplexeur
pipeline.Connect(videoEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio));
```

#### Options de configuration de la découpe

La classe `MPEGTSSplitSinkSettings` propose plusieurs options :

- **SplitDuration** : durée maximale par fichier (TimeSpan)
- **SplitFileSize** : taille maximale de fichier en octets (ulong)
- **SplitMaxSizeTimecode** : découpe basée sur la différence de timecode (string, format : « HH:MM:SS:FF »)
- **SplitMaxFiles** : nombre maximal de fichiers à conserver ; les fichiers plus anciens sont supprimés automatiquement (uint, 0 = illimité)
- **StartIndex** : index de départ pour la numérotation des segments (int)
- **M2TSMode** : utiliser le format Blu-ray M2TS avec des paquets de 192 octets (bool)

Les fichiers seront divisés lorsque **l'un** des critères configurés (durée, taille ou timecode) est atteint en premier.

#### Modèle de nom de fichier

Le modèle de nom de fichier utilise un formatage de style printf pour les numéros de segment :
- `"video_%05d.ts"` → `video_00000.ts`, `video_00001.ts`, …
- `"stream_%02d.ts"` → `stream_00.ts`, `stream_01.ts`, …

Pour en savoir plus sur les encodeurs vidéo et audio disponibles pour la capture TS en .NET :

- [Encodeurs H264](../video-encoders/h264.md)
- [Encodeurs HEVC](../video-encoders/hevc.md)
- [Encodeurs AAC](../audio-encoders/aac.md)
- [Encodeurs MP3](../audio-encoders/mp3.md)
- [Sortie MPEG-TS](../output-formats/mpegts.md)

## Considérations multiplateformes

Les deux SDK offrent des capacités multiplateformes, mais avec des approches différentes :

1. **VideoCaptureCoreX** : fournit une API unifiée sur toutes les plateformes avec des implémentations propres à chaque plateforme.
2. **MediaBlocksPipeline** : utilise une architecture cohérente basée sur des blocs sur toutes les plateformes, les blocs gérant les différences entre plateformes en interne.

## Exemples d'applications

- [Application d'exemple VideoCaptureCoreX](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Simple%20Video%20Capture)
- [Application d'exemple MediaBlocksPipeline](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

## Conclusion : choisir le bon SDK pour la capture MPEG-TS en C#

VisioForge propose deux solutions puissantes pour enregistrer des fichiers MPEG-TS en C# et .NET :

- **VideoCaptureCoreX** fournit une API simplifiée pour une implémentation rapide de la capture MPEG-TS en C#, idéale pour les projets où la facilité d'utilisation est essentielle.

- **MediaBlocksPipeline** offre une flexibilité maximale pour les scénarios complexes d'enregistrement et d'édition MPEG-TS en .NET grâce à son architecture modulaire de blocs.

Les deux SDK excellent dans la capture vidéo depuis les caméras et audio depuis les microphones, avec une prise en charge complète de la sortie MPEG-TS, ce qui en fait des outils précieux pour le développement d'un large éventail d'applications multimédias.

Choisissez VideoCaptureCoreX pour une implémentation rapide de scénarios standards de capture TS, ou MediaBlocksPipeline pour des flux d'édition avancée et de traitement personnalisé avec des fichiers TS dans vos applications .NET.
