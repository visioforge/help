---
title: Configuration de sortie FFMPEG.exe dans les SDK vidéo .NET
description: Configurez la sortie FFMPEG.exe en .NET pour capture et édition vidéo avec accélération matérielle, codecs personnalisés et options pro.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Encoding
  - Editing
  - MP4
  - WebM
  - AVI
  - MOV
  - TS
  - H.264
  - H.265
  - ProRes
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEditCore
  - FFMPEGEXEOutput
  - H264MFSettings
  - H264NVENCSettings

---

# Intégration de FFMPEG.exe avec les SDK VisioForge .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

## Introduction à la sortie FFMPEG en .NET

Ce guide fournit des instructions détaillées pour implémenter une sortie FFMPEG.exe dans des applications Windows à l'aide des SDK .NET de VisioForge. L'intégration fonctionne aussi bien avec [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) qu'avec [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net), en utilisant les moteurs `VideoCaptureCore` et `VideoEditCore`.

FFMPEG fonctionne comme un puissant framework multimédia qui permet aux développeurs de produire une sortie dans une grande variété de formats vidéo et audio. Sa flexibilité provient d'une prise en charge étendue des codecs et d'un contrôle granulaire sur les paramètres d'encodage pour les flux vidéo comme audio.

## Pourquoi utiliser FFMPEG avec les SDK VisioForge ?

L'intégration de FFMPEG dans vos applications propulsées par VisioForge procure plusieurs avantages techniques :

- **Polyvalence des formats** : prise en charge de pratiquement tous les formats de conteneur modernes
- **Flexibilité des codecs** : accès aux codecs open source comme propriétaires
- **Optimisation des performances** : options pour l'accélération CPU et GPU
- **Profondeur de personnalisation** : contrôle fin des paramètres d'encodage
- **Compatibilité multiplateforme** : sortie cohérente sur différents systèmes

## Fonctionnalités et capacités clés

### Formats de sortie pris en charge

FFMPEG prend en charge de nombreux formats de conteneur, parmi lesquels :

- MP4 (MPEG-4 Part 14)
- WebM (VP8/VP9 avec Vorbis/Opus)
- MKV (Matroska)
- AVI (Audio Video Interleave)
- MOV (QuickTime)
- WMV (Windows Media Video)
- FLV (Flash Video)
- TS (MPEG Transport Stream)

### Options d'accélération matérielle

L'encodage vidéo moderne profite des technologies d'accélération matérielle qui améliorent significativement la vitesse et l'efficacité d'encodage :

- **Intel QuickSync** : exploite les graphiques intégrés Intel pour l'encodage H.264 et HEVC
- **NVIDIA NVENC** : utilise les GPU NVIDIA pour un encodage accéléré (nécessite une carte graphique NVIDIA compatible)
- **AMD AMF/VCE** : utilise les processeurs graphiques AMD pour l'accélération de l'encodage

### Prise en charge des codecs vidéo

L'intégration offre l'accès à plusieurs codecs vidéo avec des paramètres personnalisables :

- **H.264/AVC** : standard industriel avec un excellent rapport qualité/taille
- **H.265/HEVC** : codec à plus haute efficacité pour le contenu 4K et au-delà
- **VP9** : codec vidéo ouvert de Google utilisé dans WebM
- **AV1** : codec ouvert de nouvelle génération (lorsqu'il est pris en charge)
- **MPEG-2** : codec historique pour la compatibilité DVD et diffusion
- **ProRes** : codec professionnel pour les workflows d'édition

## Processus d'implémentation

### 1. Configuration de votre environnement de développement

Avant d'implémenter la sortie FFMPEG, assurez-vous que votre environnement de développement est correctement configuré :

1. Créez un nouveau projet .NET ou ouvrez-en un existant
2. Installez les paquets NuGet du SDK VisioForge appropriés
3. Ajoutez les paquets de dépendance FFMPEG (détaillés dans la section Dépendances)
4. Importez les espaces de noms nécessaires dans votre code :

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.Types.Output;     // FFMPEGEXEOutput
using VisioForge.Core.Types.FFMPEGEXE;  // H264MFSettings, H264NVENCSettings, H264QSVSettings, HEVCQSVSettings, OutputMuxer, BasicAudioSettings, ...
```

### 2. Initialisation de la sortie FFMPEG

Commencez par créer une instance de `FFMPEGEXEOutput` pour gérer votre configuration de sortie :

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
```

Cet objet servira de conteneur pour tous vos paramètres et préférences d'encodage.

### 3. Configuration du format de conteneur de sortie

Définissez le format de conteneur de sortie souhaité à l'aide de la propriété `OutputMuxer` :

```csharp
ffmpegOutput.OutputMuxer = OutputMuxer.MP4;
```

Les autres options de conteneur courantes incluent :

- `OutputMuxer.Matroska` — pour le conteneur Matroska (.mkv)
- `OutputMuxer.WebM` — pour le format WebM
- `OutputMuxer.AVI` — pour le format AVI
- `OutputMuxer.MOV` — pour le conteneur QuickTime

### 4. Configuration de l'encodeur vidéo

FFMPEG propose plusieurs options d'encodeur vidéo. Sélectionnez et configurez l'encodeur approprié en fonction de vos besoins et du matériel disponible :

#### Encodage H.264 standard basé sur le CPU

```csharp
var videoEncoder = new H264MFSettings
{
    Bitrate     = 5000, // Kbit/s — 5000 = 5 Mbps
    RateControl = H264MFRateControl.CBR,
};
ffmpegOutput.Video = videoEncoder;
```

#### Encodage NVIDIA accéléré matériellement

```csharp
var nvidiaEncoder = new H264NVENCSettings
{
    Bitrate = 8000000,        // 8 Mbps
};
ffmpegOutput.Video = nvidiaEncoder;
```

#### Encodage Intel QuickSync accéléré matériellement

```csharp
var intelEncoder = new H264QSVSettings
{
    Bitrate = 6000000
};
ffmpegOutput.Video = intelEncoder;
```

#### Encodage HEVC/H.265 pour une efficacité supérieure

```csharp
var hevcEncoder = new HEVCQSVSettings
{
    Bitrate = 3000000,  
};
ffmpegOutput.Video = hevcEncoder;
```

### 5. Configuration de l'encodeur audio

Configurez vos paramètres d'encodage audio en fonction des exigences de qualité et de la compatibilité avec la plateforme cible :

```csharp
var audioEncoder = new BasicAudioSettings
{
    Bitrate = 192000,    // 192 kbps
    Channels = 2,        // Stéréo
    SampleRate = 48000,  // 48 kHz — standard professionnel
    Encoder = AudioEncoder.AAC,
    Mode = AudioMode.CBR
};

ffmpegOutput.Audio = audioEncoder;
```

### 6. Configuration finale et exécution

Appliquez tous les paramètres et démarrez le processus d'encodage :

```csharp
// Appliquer les paramètres de format
core.Output_Format = ffmpegOutput;

// Définir le mode de fonctionnement
core.Mode = VideoCaptureMode.VideoCapture;  // Pour Video Capture SDK
// core.Mode = VideoEditMode.Convert;       // Pour Video Edit SDK

// Définir le chemin de sortie
core.Output_Filename = "output.mp4";

// Lancer le traitement
await core.StartAsync();
```

## Dépendances requises

Installez les paquets NuGet suivants en fonction de votre architecture cible pour assurer un fonctionnement correct :

### Dépendances Video Capture SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x64
```

Pour les cibles x86 :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x86
```

### Dépendances Video Edit SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x64
```

Pour les cibles x86 :

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEGEXE.x86
```

## Dépannage et optimisation

### Problèmes courants et solutions

- **Erreurs « codec introuvable »** : assurez-vous d'avoir installé le paquet FFMPEG correct avec la prise en charge des codecs nécessaire
- **Échecs d'accélération matérielle** : vérifiez la compatibilité du GPU et la version des pilotes
- **Problèmes de performance** : ajustez le nombre de threads et le preset d'encodage selon les ressources CPU disponibles
- **Problèmes de qualité de sortie** : affinez le débit, le profil et les paramètres d'encodage

### Conseils d'optimisation des performances

- Utilisez l'accélération matérielle lorsqu'elle est disponible
- Choisissez les presets appropriés en fonction de vos exigences de qualité/vitesse
- Définissez des débits raisonnables selon le type de contenu et la résolution
- Envisagez l'encodage en deux passes pour les scénarios non temps réel exigeant la plus haute qualité

## Ressources supplémentaires

Pour davantage d'exemples de code et d'implémentation, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Pour en savoir plus sur les paramètres et les capacités de FFMPEG, consultez la [documentation officielle de FFMPEG](https://ffmpeg.org/documentation.html).
