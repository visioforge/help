---
title: Intégration MXF professionnelle pour les applications .NET
description: Générez des fichiers MXF de diffusion en .NET avec accélération matérielle, optimisation de codecs et workflows professionnels pour la production broadcast.
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
  - Encoding
  - Editing
  - MXF
  - H.264
  - H.265
  - AAC
  - MP3
  - C#
primary_api_classes:
  - QSVH264EncoderSettings
  - MXFOutput
  - NVENCH264EncoderSettings
  - AMFH264EncoderSettings
  - MFAACEncoderSettings

---

# Sortie MXF dans les SDK VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Material Exchange Format (MXF) est un format de conteneur normalisé conçu pour les applications vidéo professionnelles. Il est largement adopté dans les environnements de diffusion, les workflows de post-production et les systèmes d'archivage. Les SDK VisioForge offrent de robustes capacités de sortie MXF multiplateforme qui permettent aux développeurs d'intégrer ce format professionnel dans leurs applications.

## Comprendre le format MXF

MXF sert d'enveloppe pouvant contenir divers types de données vidéo et audio ainsi que des métadonnées. Le format a été conçu pour résoudre les problèmes d'interopérabilité dans les workflows vidéo professionnels :

- **Standard industriel** : adopté par les grands diffuseurs du monde entier
- **Métadonnées professionnelles** : prend en charge des métadonnées techniques et descriptives étendues
- **Conteneur polyvalent** : compatible avec de nombreux codecs audio et vidéo
- **Multiplateforme** : pris en charge sur Windows, macOS et Linux

## Prise en main de la sortie MXF

Deux chemins de code couvrent 99 % des cas d'utilisation :

- **`MXFOutput`** (classe dans `VisioForge.Core.Types.X.Output`) est un objet de paramètres consommé par `VideoCaptureCoreX.Outputs_Add(...)` ou défini comme `VideoEditCoreX.Output_Format`.
- **`MXFSinkBlock`** + **`MXFSinkSettings`** constituent le chemin Media Blocks lorsque vous pilotez le pipeline manuellement.

### Implémentation de base

Voici le code fondamental pour créer une sortie MXF :

```csharp
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);
```

Cela crée une sortie MXF valide avec les paramètres d'encodage par défaut. Pour les applications professionnelles, vous voudrez généralement personnaliser les paramètres d'encodage.

## Options d'encodage vidéo pour MXF

La qualité et la compatibilité de votre sortie MXF dépendent largement du choix de votre encodeur vidéo. Les SDK VisioForge prennent en charge plusieurs options d'encodeurs pour équilibrer performance, qualité et compatibilité. Pour des options de configuration détaillées, consultez la [documentation de l'encodeur H.264](../video-encoders/h264.md) et la [documentation de l'encodeur HEVC](../video-encoders/hevc.md).

> Les propriétés `Bitrate` des encodeurs vidéo dans l'espace de noms X sont en **Kbps** (donc 8000 = 8 Mbps). Ne passez pas une valeur en bits par seconde bruts.

### Encodeurs accélérés matériellement

Pour des performances optimales dans les applications temps réel, les encodeurs accélérés matériellement sont recommandés :

#### Encodeurs NVIDIA NVENC

```csharp
// Vérifier d'abord la disponibilité
if (NVENCH264EncoderSettings.IsAvailable())
{
    var nvencSettings = new NVENCH264EncoderSettings
    {
        Bitrate = 8000, // 8 Mbps (Kbps)
    };

    mxfOutput.Video = nvencSettings;
}
```

#### Encodeurs Intel Quick Sync Video (QSV)

```csharp
if (QSVH264EncoderSettings.IsAvailable())
{
    var qsvSettings = new QSVH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = qsvSettings;
}
```

#### Encodeurs AMD Advanced Media Framework (AMF)

```csharp
if (AMFH264EncoderSettings.IsAvailable())
{
    var amfSettings = new AMFH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = amfSettings;
}
```

### Encodeurs logiciels

Lorsque l'accélération matérielle n'est pas disponible, les encodeurs logiciels constituent des alternatives fiables :

#### Encodeur OpenH264

```csharp
var openH264Settings = new OpenH264EncoderSettings
{
    Bitrate = 8000,
};

mxfOutput.Video = openH264Settings;
```

### High-Efficiency Video Coding (HEVC/H.265)

Pour les applications nécessitant une efficacité de compression supérieure :

```csharp
// Encodeur HEVC NVIDIA
if (NVENCHEVCEncoderSettings.IsAvailable())
{
    var nvencHevcSettings = new NVENCHEVCEncoderSettings
    {
        Bitrate = 5000, // Débit plus faible possible avec HEVC
    };

    mxfOutput.Video = nvencHevcSettings;
}
```

## Encodage audio pour les fichiers MXF

Bien que la vidéo attire souvent le plus d'attention, un encodage audio approprié est crucial pour les sorties MXF professionnelles. Les SDK VisioForge offrent plusieurs options d'encodeurs audio. Pour des options de configuration détaillées, consultez la [documentation de l'encodeur AAC](../audio-encoders/aac.md) et la [documentation de l'encodeur MP3](../audio-encoders/mp3.md).

> Le `Bitrate` des encodeurs audio dans l'espace de noms X est également en **Kbps** (donc 192 = 192 kbps). `MFAACEncoderSettings` et `VOAACEncoderSettings` exposent bien une propriété `SampleRate` (par défaut 48000) ; seul `MP3EncoderSettings` n'a pas de setter de fréquence d'échantillonnage et suit le format audio de la source en amont. La disposition des canaux sur les trois suit l'audio en amont à moins d'être reformée en amont (par ex. via `AudioResamplerBlock`).

### Encodeurs AAC

L'AAC est le codec préféré pour la plupart des applications professionnelles :

```csharp
// Media Foundation AAC (Windows uniquement)
#if NET_WINDOWS
    var mfAacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192, // kbps
    };

    mxfOutput.Audio = mfAacSettings;
#else
    // Alternative AAC multiplateforme
    var voAacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
    };

    mxfOutput.Audio = voAacSettings;
#endif
```

### Encodeur MP3

Pour une compatibilité maximale :

```csharp
var mp3Settings = new MP3EncoderSettings
{
    Bitrate = 320,         // Kbps — doit être l'une des valeurs : 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320
    ForceMono = false      // Par défaut ; définir à true pour fusionner en mono
};

mxfOutput.Audio = mp3Settings;
```

## Configuration MXF avancée

### Pipelines de traitement personnalisés

L'une des puissantes fonctionnalités des SDK VisioForge est la possibilité d'ajouter du traitement personnalisé à votre chaîne de sortie MXF :

```csharp
// Ajouter un traitement vidéo personnalisé
mxfOutput.CustomVideoProcessor = yourVideoProcessingBlock;

// Ajouter un traitement audio personnalisé
mxfOutput.CustomAudioProcessor = yourAudioProcessingBlock;
```

### Configuration du puits

Affinez votre sortie MXF avec les paramètres du puits :

```csharp
// Accéder aux paramètres du puits (MXFSinkSettings)
mxfOutput.Sink.Filename = "new_output.mxf";
```

## Considérations multiplateformes

Construire des applications qui fonctionnent sur différentes plateformes nécessite une planification soignée :

```csharp
// Sélection d'encodeur spécifique à la plateforme
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

#if NET_WINDOWS
    if (QSVH264EncoderSettings.IsAvailable())
    {
        mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
        mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
    }
#elif NET_MACOS
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new MP3EncoderSettings { Bitrate = 320 };
#endif
```

## Gestion des erreurs et validation

Les implémentations MXF robustes nécessitent une gestion appropriée des erreurs :

```csharp
try
{
    // Créer la sortie MXF
    var mxfOutput = new MXFOutput(
        filename: Path.Combine(outputDirectory, "output.mxf"),
        videoStreamType: MXFVideoStreamType.H264,
        audioStreamType: MXFAudioStreamType.MPEG
    );

    // Valider la disponibilité de l'encodeur
    if (!OpenH264EncoderSettings.IsAvailable())
    {
        throw new ApplicationException("Aucun encodeur H.264 compatible trouvé");
    }

    // Valider le répertoire de sortie
    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(mxfOutput.Sink.Filename));
    if (!directoryInfo.Exists)
    {
        Directory.CreateDirectory(directoryInfo.FullName);
    }

    // Attacher MXFOutput comme sortie VideoCaptureCoreX
    videoCapture.Outputs_Add(mxfOutput, autostart: true);
    await videoCapture.StartAsync();
}
catch (Exception ex)
{
    logger.LogError($"Erreur de sortie MXF : {ex.Message}");
    // Implémenter une stratégie de repli
}
```

## Optimisation des performances

Pour une performance optimale de la sortie MXF :

1. **Privilégier l'accélération matérielle** : recherchez toujours et utilisez en premier les encodeurs matériels
2. **Gestion des tampons** : ajustez la taille des tampons en fonction des capacités du système
3. **Traitement parallèle** : utilisez le multithreading lorsque c'est approprié
4. **Sélection de preset** : choisissez les presets d'encodeur en fonction des exigences qualité/vitesse

## Exemple d'implémentation complet — VideoCaptureCoreX

Voici un exemple complet démontrant l'implémentation MXF avec options de repli :

```csharp
// Créer la sortie MXF avec des types de flux spécifiques
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

// Configurer l'encodeur vidéo avec une chaîne de repli priorisée (débit en Kbps)
if (NVENCH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new NVENCH264EncoderSettings { Bitrate = 8000 };
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
}
else if (AMFH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new AMFH264EncoderSettings { Bitrate = 8000 };
}
else
{
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
}

// Configurer l'audio optimisé pour la plateforme (Kbps)
#if NET_WINDOWS
    mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#endif

// Attacher à VideoCaptureCoreX (ou à VideoEditCoreX : videoEdit.Output_Format = mxfOutput;)
videoCapture.Outputs_Add(mxfOutput, autostart: true);

await videoCapture.StartAsync();
```

## Exemple d'implémentation complet — MediaBlocksPipeline

Lorsque vous pilotez le pipeline manuellement, utilisez `MXFSinkBlock` + `MXFSinkSettings` à la place de `MXFOutput` :

```csharp
var pipeline = new MediaBlocksPipeline();

var mxfSettings = new MXFSinkSettings("output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG);

var mxfSink = new MXFSinkBlock(mxfSettings);

// videoEncoder / audioEncoder sont des instances existantes de H264EncoderBlock / AACEncoderBlock
pipeline.Connect(videoEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

En suivant ce guide, vous pouvez implémenter une sortie MXF de qualité professionnelle dans vos applications à l'aide des SDK .NET VisioForge, en assurant la compatibilité avec les workflows de diffusion et les systèmes de post-production.
