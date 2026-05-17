---
title: Sortie vidéo MPEG-TS avec découpe de fichier en C# .NET
description: Découpe de fichier par durée, taille ou timecode. Tampon glissant avec limite max. Encodage GPU H.264/HEVC et mode Blu-ray M2TS. Exemples SDK VisioForge.
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
  - Streaming
  - Recording
  - Encoding
  - Editing
  - MP4
  - TS
  - H.264
  - H.265
  - AAC
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - MPEGTSOutput
  - NVENCH264EncoderSettings
  - MPEGTSSplitSinkSettings
  - MFBaseOutput
  - QSVH264EncoderSettings

---

# Sortie MPEG-TS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le module de sortie MPEG-TS (Transport Stream) du SDK VisioForge offre les fonctionnalités nécessaires pour créer des fichiers MPEG Transport Stream avec diverses options d'encodage vidéo et audio. Ce guide explique comment configurer et utiliser efficacement la classe `MPEGTSOutput`.

## Sortie MPEG-TS multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Pour créer une nouvelle sortie MPEG-TS, utilisez le constructeur suivant :

```csharp
// Initialiser avec audio AAC (recommandé)
var output = new MPEGTSOutput("output.ts", useAAC: true);
```

Vous pouvez aussi utiliser un audio MP3 au lieu de l'AAC :

```csharp
// Initialiser avec audio MP3 au lieu de l'AAC
var output = new MPEGTSOutput("output.ts", useAAC: false);
```

### Options d'encodage vidéo

[MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) prend en charge plusieurs encodeurs vidéo via la propriété `Video`. Les encodeurs disponibles incluent :

**[Encodeurs H.264](../video-encoders/h264.md)**

- OpenH264 (logiciel)
- NVENC H.264 (accélération GPU NVIDIA)
- QSV H.264 (Intel Quick Sync)
- AMF H.264 (accélération GPU AMD)

**[Encodeurs H.265/HEVC](../video-encoders/hevc.md)**

- MF HEVC (Windows Media Foundation, Windows uniquement)
- NVENC HEVC (accélération GPU NVIDIA)
- QSV HEVC (Intel Quick Sync)
- AMF H.265 (accélération GPU AMD)

Exemple de définition d'un encodeur vidéo spécifique :

```csharp
// Vérifier si l'encodeur NVIDIA est disponible
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else
{
    // Repli sur OpenH264
    output.Video = new OpenH264EncoderSettings();
}
```

### Options d'encodage audio

Les encodeurs audio suivants sont pris en charge via la propriété `Audio` :

**[Encodeurs AAC](../audio-encoders/aac.md)**

- VO-AAC (multiplateforme)
- AVENC AAC
- MF AAC (Windows uniquement)
  
**[Encodeur MP3](../audio-encoders/mp3.md)** :

- MP3EncoderSettings

Exemple de définition d'un encodeur audio :

```csharp
// Pour les plateformes Windows
output.Audio = new MFAACEncoderSettings();
```

```csharp
// Pour la compatibilité multiplateforme
output.Audio = new VOAACEncoderSettings();
```

```csharp
// Utiliser le MP3 au lieu de l'AAC
output.Audio = new MP3EncoderSettings();
```

### Gestion des fichiers

Vous pouvez obtenir ou définir le nom du fichier de sortie après l'initialisation :

```csharp
// Obtenir le nom de fichier actuel
string currentFile = output.GetFilename();

// Modifier le nom du fichier de sortie
output.SetFilename("new_output.ts");
```

### Découpe de fichier

La classe `MPEGTSSplitSinkSettings` permet la découpe automatique de la sortie MPEG-TS en plusieurs fichiers en fonction de la taille, de la durée ou du timecode. C'est utile pour :

- Créer des fichiers segmentés pour le streaming HLS
- Gérer le stockage en limitant les tailles de fichiers
- Enregistrer des vidéos en time-lapse
- Implémenter un enregistrement en tampon glissant

#### Options de configuration

```csharp
using VisioForge.Core.Types.X.Sinks;

// Créer les paramètres de découpe avec un modèle de nom de fichier
// %05d sera remplacé par le numéro de segment (00000, 00001, etc.)
var splitSettings = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    // Découper par durée (par ex. toutes les 5 minutes)
    SplitDuration = TimeSpan.FromMinutes(5),
    
    // Découper par taille de fichier (par ex. 100 Mo, 0 = désactivé)
    SplitFileSize = 100 * 1024 * 1024,
    
    // Nombre maximal de fichiers à conserver (les anciens sont supprimés, 0 = illimité)
    SplitMaxFiles = 10,
    
    // Découper par différence de timecode (optionnel)
    SplitMaxSizeTimecode = "01:00:00:00", // 1 heure
    
    // Index de départ pour la numérotation des segments
    StartIndex = 0,
    
    // Mode M2TS (format Blu-ray avec paquets de 192 octets)
    M2TSMode = false
};

// Appliquer à la sortie
output.Sink = splitSettings;
```

#### Déclencheurs de découpe

Les fichiers seront découpés lorsque l'une de ces conditions est remplie :

1. **Par durée** : `SplitDuration` — nouveau fichier créé après le temps spécifié
2. **Par taille** : `SplitFileSize` — nouveau fichier créé lorsque la limite de taille est atteinte
3. **Par timecode** : `SplitMaxSizeTimecode` — nouveau fichier lorsque la différence de timecode est atteinte

#### Modèle de nom de fichier

Le modèle de nom de fichier utilise un formatage de style printf pour les numéros de segment :

```csharp
// Exemples de modèles de noms de fichier
"recording_%02d.ts"   // recording_00.ts, recording_01.ts, ...
"stream_%05d.ts"      // stream_00000.ts, stream_00001.ts, ...
"output_%d.ts"        // output_0.ts, output_1.ts, ...
```

#### Enregistrement en tampon glissant

Pour implémenter un tampon glissant qui ne conserve que les N derniers segments :

```csharp
var settings = new MPEGTSSplitSinkSettings("buffer_%03d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(1),  // Segments d'1 minute
    SplitMaxFiles = 60  // Conserver les 60 dernières minutes
};
```

#### Exemple d'utilisation

```csharp
// Exemple complet avec paramètres de découpe
var output = new MPEGTSOutput("video_%05d.ts", useAAC: true);

// Configurer les paramètres de découpe
output.Sink = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(5),
    SplitMaxFiles = 12,  // Conserver la dernière heure (12 x 5 minutes)
    M2TSMode = false
};

// Configurer les encodeurs
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}

// À utiliser avec VideoCaptureCoreX ou MediaBlocksPipeline
// Le modèle de nom de fichier sera utilisé automatiquement
```

### Fonctionnalités avancées

#### Traitement personnalisé

MPEGTSOutput prend en charge le traitement personnalisé vidéo et audio via les MediaBlocks :

```csharp
// Ajouter un traitement vidéo personnalisé
output.CustomVideoProcessor = new YourCustomVideoProcessor();

// Ajouter un traitement audio personnalisé
output.CustomAudioProcessor = new YourCustomAudioProcessor();
```

#### Paramètres du puits

La sortie utilise `MPEGTSSinkSettings` (ou la classe dérivée `MPEGTSSplitSinkSettings` pour une sortie segmentée) pour la configuration :

```csharp
// Accéder aux paramètres du puits
output.Sink.Filename = "modified_output.ts";
```

### Considérations sur les plateformes

- Certains encodeurs (MF AAC, MF HEVC) ne sont disponibles que sur les plateformes Windows
- Les applications multiplateformes doivent utiliser des encodeurs agnostiques de plateforme comme VO-AAC pour l'audio

### Bonnes pratiques

1. **Accélération matérielle** : lorsqu'elle est disponible, préférez les encodeurs accélérés matériellement (NVENC, QSV, AMF) aux encodeurs logiciels pour de meilleures performances.

2. **Choix du codec audio** : utilisez l'AAC pour une meilleure compatibilité et qualité, sauf si vous avez des exigences spécifiques pour le MP3.

3. **Gestion des erreurs** : vérifiez toujours la disponibilité de l'encodeur avant d'utiliser les options accélérées matériellement :

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    // Utiliser l'encodeur NVIDIA
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    // Repli sur Intel Quick Sync
}
else
{
    // Repli sur l'encodage logiciel
}
```

**Compatibilité multiplateforme** : pour les applications multiplateformes, assurez-vous d'utiliser des encodeurs disponibles sur toutes les plateformes cibles ou implémentez des solutions de repli appropriées.

### Exemple d'implémentation

Voici un exemple complet montrant comment créer et configurer une sortie MPEG-TS :

```csharp
var output = new MPEGTSOutput("output.ts", useAAC: true);

// Configurer l'encodeur vidéo
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    output.Video = new QSVH264EncoderSettings();
}
else
{
    output.Video = new OpenH264EncoderSettings();
}

// Configurer l'encodeur audio en fonction de la plateforme
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new VOAACEncoderSettings();
#endif

// Optionnel : ajouter un traitement personnalisé
output.CustomVideoProcessor = new YourCustomVideoProcessor();
output.CustomAudioProcessor = new YourCustomAudioProcessor();
```

## Sortie MPEG-TS uniquement Windows

[VideoCaptureCore](#){ .md-button }

La classe `MPEGTSOutput` fournit des paramètres de configuration pour la sortie MPEG Transport Stream (MPEG-TS) dans le framework de traitement vidéo VisioForge. Cette classe hérite de `MFBaseOutput` et implémente l'interface `IVideoCaptureBaseOutput`, donc le moteur Windows classique l'expose uniquement via `VideoCaptureCore` — le `VideoEditCore` classique ne dispose pas de chemin de sortie MPEG-TS (utilisez `VideoEditCoreX` pour une sortie MPEG-TS multiplateforme à la place).

### Hiérarchie de classes

```text
MFBaseOutput
    └── MPEGTSOutput
```

### Paramètres vidéo hérités

La classe [MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) hérite des capacités d'encodage vidéo de MFBaseOutput, qui incluent :

**Configuration d'encodage vidéo** : via la propriété `Video`, prenant en charge :

- Plusieurs options de codec (H.264/H.265) avec prise en charge de l'accélération matérielle
- Contrôle de débit (CBR/VBR)
- Paramètres de qualité
- Configuration du type d'image et de la structure GOP
- Options d'entrelacement
- Contrôles de résolution et de rapport hauteur/largeur

### Paramètres audio hérités

La configuration audio est gérée via la propriété héritée `Audio` de type M4AOutput, qui inclut :

Encodage audio AAC configurable :

- Version (par défaut : MPEG-4)
- Type d'objet (par défaut : AAC-LC)
- Débit (par défaut : 128 kbps)
- Format de sortie (par défaut : RAW)

### Utilisation

#### Implémentation de base

```csharp
// Créer l'instance VideoCaptureCore
var core = new VideoCaptureCore();

// Définir le nom du fichier de sortie
core.Output_Filename = "output.ts";

// Créer la sortie MPEG-TS
var mpegtsOutput = new MPEGTSOutput();

// Configurer les paramètres vidéo
mpegtsOutput.Video.Codec = MFVideoEncoder.MS_H264;
mpegtsOutput.Video.AvgBitrate = 2000; // 2 Mbps
mpegtsOutput.Video.RateControl = MFCommonRateControlMode.CBR;

// Configurer les paramètres audio
mpegtsOutput.Audio.Bitrate = 128; // 128 kbps
mpegtsOutput.Audio.Version = AACVersion.MPEG4;

core.Output_Format = mpegtsOutput;
```

#### Prise en charge de la sérialisation

La classe fournit une prise en charge intégrée de la sérialisation JSON pour enregistrer et charger les configurations :

```csharp
// Enregistrer la configuration
string jsonConfig = mpegtsOutput.Save();

// Charger la configuration
MPEGTSOutput loadedConfig = MPEGTSOutput.Load(jsonConfig);
```

### Configuration par défaut

La classe `MPEGTSOutput` s'initialise avec ces paramètres par défaut :

#### Valeurs vidéo par défaut (héritées de MFBaseOutput)

- Débit moyen : 2000 kbps
- Codec : Microsoft H.264
- Profil : Main
- Niveau : 4.2
- Contrôle de débit : CBR
- Qualité vs vitesse : 85
- Nombre maximal d'images de référence : 2
- MaxKeyFrameSpacing (taille GOP) : 125 images
- Nombre d'images B : 0
- Mode faible latence : désactivé
- CABAC : désactivé
- Mode d'entrelacement : progressif

#### Valeurs audio par défaut

- Débit : 128 kbps
- Version AAC : MPEG-4
- Type d'objet AAC : Low Complexity (LC)
- Format de sortie : RAW

### Bonnes pratiques

1. **Configuration du débit** :
   - Pour les applications de streaming, assurez-vous que les débits combinés vidéo et audio respectent votre bande passante cible
   - Envisagez l'utilisation de VBR pour les scénarios de stockage et de CBR pour le streaming

2. **Accélération matérielle** :
   - Lorsqu'ils sont disponibles, utilisez les encodeurs accélérés matériellement (QSV, NVENC, AMD) pour de meilleures performances
   - Repli sur MS_H264/MS_H265 lorsque l'accélération matérielle n'est pas disponible

3. **Optimisation de la qualité** :
   - Pour une qualité supérieure au prix de la performance, augmentez la valeur de `QualityVsSpeed`
   - Activez CABAC pour une meilleure efficacité de compression dans les scénarios non à faible latence
   - Ajustez `MaxKeyFrameSpacing` selon votre cas d'usage spécifique (valeurs plus faibles pour un meilleur seek, valeurs plus élevées pour une meilleure compression)

### Notes techniques

1. **Caractéristiques de MPEG-TS** :
   - Adapté aux applications de streaming et de diffusion
   - Procure une résilience aux erreurs grâce à sa structure basée sur des paquets
   - Prend en charge plusieurs programmes et flux élémentaires

2. **Considérations de performance** :
   - Le mode faible latence sacrifie la qualité pour réduire le délai d'encodage
   - Les images B améliorent la compression mais augmentent la latence
   - L'accélération matérielle peut réduire significativement l'utilisation CPU

### Redists requis  

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code.
