---
title: Sortie vidéo MP4 en C# .NET — multiplexage H.264, HEVC, AAC
description: Auto-sélection d'encodeur avec repli NVENC/QSV/AMF. Découpe par taille, durée ou timecode. Tampon glissant et multiplexage de flux. SDK VisioForge.
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
  - Webcam
  - MP4
  - H.264
  - H.265
  - MPEG-2
  - AAC
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - MP4Output
  - MP4SplitSinkSettings
  - NVENCH264EncoderSettings
  - VOAACEncoderSettings
  - MP4OutputBlock

---

# Sortie de fichier MP4

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Le MP4 (MPEG-4 Part 14), introduit en 2001, est un format de conteneur multimédia numérique utilisé le plus souvent pour stocker de la vidéo et de l'audio. Il prend également en charge les sous-titres et les images. MP4 est réputé pour sa forte compression et sa compatibilité avec divers appareils et plateformes, ce qui en fait un choix populaire pour le streaming et le partage.

Capturer des vidéos depuis une webcam et les enregistrer dans un fichier est une exigence courante dans de nombreuses applications. Une façon d'y parvenir consiste à utiliser un kit de développement logiciel (SDK) tel que VisioForge Video Capture SDK .Net, qui fournit une API facile à utiliser pour capturer et traiter de la vidéo en C#.

Pour capturer une vidéo au format MP4 avec Video Capture SDK, vous devez configurer le format de sortie vidéo via l'une des classes dédiées à la sortie MP4. Vous pouvez utiliser plusieurs encodeurs vidéo logiciels et matériels, notamment Intel QuickSync, Nvidia NVENC et AMD/ATI APU.

## Sortie MP4 multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La classe [MP4Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP4Output.html?q=MP4Output) offre une manière flexible et puissante de configurer les paramètres de sortie vidéo MP4 pour les opérations de capture et d'édition vidéo. Ce guide vous accompagnera dans l'utilisation efficace de la classe MP4Output, en couvrant ses fonctionnalités clés et ses modèles d'utilisation courants.

MP4Output implémente plusieurs interfaces importantes :

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- Création de Media Block

Cela la rend adaptée aussi bien aux scénarios d'édition que de capture vidéo, tout en offrant un contrôle étendu sur le traitement vidéo et audio.

### Utilisation de base

La manière la plus simple de créer une instance MP4Output consiste à utiliser le constructeur avec un nom de fichier :

```csharp
var output = new MP4Output("output.mp4");
```

Cela crée un MP4Output avec les paramètres par défaut d'encodeur vidéo et audio. Sur Windows, il utilise OpenH264 pour l'encodage vidéo et Media Foundation AAC pour l'encodage audio par défaut.

### Configuration de l'encodeur vidéo

La classe MP4Output prend en charge plusieurs encodeurs vidéo via sa propriété `Video`. Voici les encodeurs vidéo pris en charge :

**[Encodeurs H.264](../video-encoders/h264.md)**

- OpenH264EncoderSettings (Par défaut, CPU)
- AMFH264EncoderSettings (AMD)
- NVENCH264EncoderSettings (NVIDIA)
- QSVH264EncoderSettings (Intel Quick Sync)

**[Encodeurs HEVC (H.265)](../video-encoders/hevc.md)**

- MFHEVCEncoderSettings (Windows uniquement)
- AMFHEVCEncoderSettings (AMD)
- NVENCHEVCEncoderSettings (NVIDIA)
- QSVHEVCEncoderSettings (Intel Quick Sync)

Vous pouvez vérifier la disponibilité d'encodeurs spécifiques à l'aide de la méthode `IsAvailable` :

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

Exemple de configuration d'un encodeur vidéo spécifique :

```csharp
var output = new MP4Output("output.mp4");
output.Video = new NVENCH264EncoderSettings(); // Utiliser l'encodeur NVIDIA
```

### Configuration de l'encodeur audio

La propriété `Audio` permet de spécifier l'encodeur audio. Les encodeurs audio pris en charge incluent :

- [VOAACEncoderSettings](../audio-encoders/aac.md)
- [AVENCAACEncoderSettings](../audio-encoders/aac.md)
- [MFAACEncoderSettings](../audio-encoders/aac.md) (Windows uniquement)
- [MP3EncoderSettings](../audio-encoders/mp3.md)

Exemple de définition d'un encodeur audio personnalisé :

```csharp
var output = new MP4Output("output.mp4");
output.Audio = new MP3EncoderSettings();
```

La classe MP4Output sélectionne automatiquement des encodeurs par défaut adaptés en fonction de la plateforme.

### Exemple de code

Ajoutez la sortie MP4 à l'instance principale du Video Capture SDK :

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

Définissez le format de sortie pour l'instance principale du Video Edit SDK :

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

Créez une instance de sortie MP4 Media Blocks :

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var mp4SinkSettings = new MP4SinkSettings("output.mp4");
var mp4Output = new MP4OutputBlock(mp4SinkSettings, h264, aac);
```

### Découpe de fichier

La classe [MP4SplitSinkSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Sinks.MP4SplitSinkSettings.html) offre des capacités de découpe automatique de fichier, vous permettant de diviser la sortie MP4 en plusieurs fichiers selon la taille, la durée ou le timecode. Cette classe peut être utilisée à la fois avec `MP4OutputBlock` (qui inclut l'encodage) et `MP4SinkBlock` (multiplexage uniquement). Cela est particulièrement utile pour :

- Les longues sessions d'enregistrement qui doivent être découpées en tailles de fichier gérables
- La création de segments temporels pour un archivage ou une distribution plus simples
- La gestion de l'espace disque en limitant le nombre de fichiers conservés sur le stockage
- L'implémentation d'un enregistrement à tampon glissant où seuls les fichiers récents sont conservés

**Découpe par taille de fichier**

Découpez la sortie lorsqu'un fichier atteint une taille spécifique en octets :

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Créer les paramètres de découpe avec un motif de nom de fichier
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Découper lorsque le fichier atteint 100 Mo (104857600 octets)
splitSettings.SplitFileSize = 104857600;

// Désactiver la découpe basée sur la durée (par défaut 1 minute)
splitSettings.SplitDuration = TimeSpan.Zero;

// Créer le bloc de sortie
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Découpe par durée**

Découpez la sortie lorsqu'un fichier atteint une durée spécifique :

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Créer les paramètres de découpe avec un motif de nom de fichier
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Découper toutes les 5 minutes
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Créer le bloc de sortie
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Limiter le nombre maximal de fichiers**

Contrôlez le nombre maximal de fichiers à conserver sur le disque. Une fois la limite atteinte, les fichiers les plus anciens sont automatiquement supprimés :

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Créer les paramètres de découpe
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Découper toutes les 10 minutes
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Conserver uniquement les 6 derniers fichiers (1 heure d'enregistrements)
splitSettings.SplitMaxFiles = 6;

// Créer le bloc de sortie
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Découpe par timecode**

Découpez la sortie en fonction de la différence de timecode entre la première et la dernière image :

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Créer les paramètres de découpe
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Découper lorsque la différence de timecode atteint 1 heure
// Format : "HH:MM:SS:FF" où FF correspond aux images
splitSettings.SplitMaxSizeTimecode = "01:00:00:00";

// Désactiver les autres méthodes de découpe
splitSettings.SplitFileSize = 0;
splitSettings.SplitDuration = TimeSpan.Zero;

// Créer le bloc de sortie
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Combinaison de paramètres**

Vous pouvez combiner les critères de découpe. Le fichier est divisé dès que l'un des critères activés est satisfait :

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Créer les paramètres de découpe
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Découper à 200 Mo OU 10 minutes, selon ce qui arrive en premier
splitSettings.SplitFileSize = 209715200; // 200 Mo
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Conserver uniquement les 12 derniers fichiers
splitSettings.SplitMaxFiles = 12;

// Commencer la numérotation à 1 au lieu de 0
splitSettings.StartIndex = 1;

// Créer le bloc de sortie
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Utilisation avec MP4SinkBlock**

Pour les scénarios où vous disposez déjà de flux encodés et n'avez besoin que du multiplexage (création de conteneur), utilisez `MP4SinkBlock` avec `MP4SplitSinkSettings` :

```csharp
// Créer les paramètres de découpe
var splitSettings = new MP4SplitSinkSettings("output_%05d.mp4");
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Créer le bloc puits MP4 (multiplexage uniquement, pas d'encodage)
var mp4Sink = new MP4SinkBlock(splitSettings);

// Connectez vos flux pré-encodés au bloc puits
// (la logique de connexion dépend de la structure de votre pipeline)
```

**Notes importantes :**

- Le paramètre nom de fichier doit inclure un spécificateur de format (comme `%05d`) pour l'index de fichier
- Définissez `SplitFileSize` à 0 pour désactiver la découpe basée sur la taille (par défaut 0)
- La `SplitDuration` par défaut est de 1 minute ; définissez-la à `TimeSpan.Zero` pour désactiver la découpe basée sur la durée
- Définissez `SplitMaxFiles` à 0 pour conserver tous les fichiers (par défaut 0, pas de suppression)
- Lors de la combinaison de critères, la découpe se produit dès qu'UN critère est satisfait
- La propriété `StartIndex` contrôle le numéro de fichier initial (par défaut 0)

### Bonnes pratiques

**Accélération matérielle** : Lorsque c'est possible, utilisez des encodeurs accélérés matériellement (NVENC, AMF, QSV) pour de meilleures performances :

```csharp
var output = new MP4Output("output.mp4");
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

**Sélection d'encodeur** : Utilisez les méthodes fournies pour énumérer les encodeurs disponibles :

```csharp
var output = new MP4Output("output.mp4");
var availableVideoEncoders = output.GetVideoEncoders();
var availableAudioEncoders = output.GetAudioEncoders();
```

### Problèmes courants et solutions

1. **Accès aux fichiers** : Le constructeur MP4Output tente de vérifier l'accès en écriture en créant puis en supprimant immédiatement un fichier de test. Assurez-vous que l'application dispose des autorisations adéquates sur le répertoire de sortie.

2. **Disponibilité des encodeurs** : Les encodeurs matériels peuvent ne pas être disponibles sur tous les systèmes. Prévoyez toujours une solution de repli :

```csharp
var output = new MP4Output("output.mp4");
if (!NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new OpenH264EncoderSettings(); // Repli sur l'encodeur logiciel
}
```

3. **Compatibilité des plateformes** : Certains encodeurs sont spécifiques à une plateforme. Utilisez la compilation conditionnelle ou des vérifications à l'exécution lorsque vous ciblez plusieurs plateformes :

```csharp
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new MP3EncoderSettings();
#endif
```

## Sortie MP4 réservée à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

`Le même exemple de code peut être utilisé pour Video Edit SDK .Net. Utilisez la classe VideoEditCore à la place de VideoCaptureCore.`

### Encodeur CPU ou encodeur GPU Intel QuickSync

Créez un objet `MP4Output` pour la sortie MP4.

```cs
var mp4Output = new MP4Output();
```

Définissez le mode MP4 sur `CPU_QSV`.

```cs
mp4Output.MP4Mode = MP4Mode.CPU_QSV;
```

Définissez les paramètres vidéo.

```cs
mp4Output.Video.Profile = H264Profile.ProfileMain; // Profil H264
mp4Output.Video.Level = H264Level.Level4; // Niveau H264
mp4Output.Video.Bitrate = 2000; // débit binaire

// paramètres facultatifs
mp4Output.Video.MBEncoding = H264MBEncoding.CABAC; //CABAC / CAVLC
mp4Output.Video.BitrateAuto = false; // true pour utiliser le débit automatique
mp4Output.Video.RateControl = H264RateControl.VBR; // contrôle de débit - CBR ou VBR
```

Définissez les paramètres audio AAC.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW ou ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // type d'AAC
```

### Encodeur Nvidia NVENC

Créez l'objet `MP4Output` pour la sortie MP4.

```cs
var mp4Output = new MP4Output();
```

Définissez le mode MP4 sur `NVENC`.

```cs
mp4Output.MP4Mode = MP4Mode.NVENC;
```

Définissez les paramètres vidéo.

```cs
mp4Output.Video_NVENC.Profile = NVENCVideoEncoderProfile.H264_Main; // Profil H264
mp4Output.Video_NVENC.Level = NVENCEncoderLevel.H264_4; // Niveau H264
mp4Output.Video_NVENC.Bitrate = 2000; // débit binaire

// paramètres facultatifs
mp4Output.Video_NVENC.RateControl = NVENCRateControlMode.VBR; // contrôle de débit - CBR ou VBR
```

Définissez les paramètres audio.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW ou ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // type d'AAC
```

### Encodeurs CPU/GPU

En utilisant la sortie MP4 HW, vous pouvez utiliser des encodeurs accélérés matériellement par Intel (QuickSync), Nvidia (NVENC) et AMD/ATI.

Créez l'objet `MP4HWOutput` pour la sortie MP4 HW.

```cs
var mp4Output = new MP4HWOutput();
```

Obtenez les encodeurs disponibles.

```cs
var availableEncoders = VideoCaptureCore.HWEncodersAvailable();
// ou
var availableEncoders = VideoEditCore.HWEncodersAvailable();
```

En fonction des encodeurs disponibles, sélectionnez le codec vidéo.

```cs
mp4Output.Video.Codec = MFVideoEncoder.MS_H264; // Microsoft H264
mp4Output.Video.Profile = MFH264Profile.Main; // Profil H264
mp4Output.Video.Level = MFH264Level.Level4; // Niveau H264
mp4Output.Video.AvgBitrate = 2000; // débit binaire

// paramètres facultatifs
mp4Output.Video.CABAC = true; // CABAC / CAVLC
mp4Output.Video.RateControl = MFCommonRateControlMode.CBR; // contrôle de débit

// de nombreux autres paramètres sont disponibles
```

Définissez les paramètres audio.

```cs
mp4Output.Audio.Bitrate = 192;
mp4Output.Audio.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio.Output = AACOutput.RAW; // RAW ou ADTS
mp4Output.Audio.Object = AACObject.Low; // type d'AAC
```

Vous pouvez désormais appliquer les paramètres de sortie MP4 à la classe principale (VideoCaptureCore ou VideoEditCore) et démarrer la capture ou l'édition vidéo.

### Appliquer les paramètres de capture vidéo

Définissez les paramètres de format MP4 pour la sortie.

```cs
core.Output_Format = mp4Output;
```

Définissez un mode de capture vidéo (ou un mode de conversion vidéo si vous utilisez Video Edit SDK).

```cs
core.Mode = VideoCaptureMode.VideoCapture;
```

Définissez un nom de fichier (assurez-vous de disposer des droits d'écriture).

```cs
core.Output_Filename = "output.mp4";
```

Démarrez la capture vidéo (conversion) vers un fichier.

```cs
await VideoCapture1.StartAsync();
```

Enfin, lorsque nous avons terminé la capture vidéo, arrêtez le pipeline et libérez les ressources. `StopAsync()` vide le multiplexeur et finalise le fichier de sortie — ne sautez pas cette étape, sans quoi le MP4 ne sera pas lisible :

```cs
await VideoCapture1.StopAsync();
VideoCapture1.Dispose();
```

### Redistribuables requis

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---
Consultez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
