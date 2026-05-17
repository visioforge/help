---
title: Diffusion RTMP en direct vers YouTube et Facebook en C# .NET
description: Passez en direct avec un encodage H.264 accéléré matériellement. Repli automatique des encodeurs (NVENC, QSV, AMF, OpenH264). Exemples C# multiplateformes.
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
  - Encoding
  - Editing
  - RTMP
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - RTMPOutput
  - NVENCH264EncoderSettings
  - RTMPSinkSettings
  - VideoCaptureCoreX
  - VideoEditCoreX

---

# Diffusion RTMP avec les SDK VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la diffusion RTMP

RTMP (Real-Time Messaging Protocol) est un protocole de communication robuste conçu pour la transmission haute performance de données audio, vidéo et autres entre un serveur et un client. Les SDK VisioForge offrent une prise en charge complète de la diffusion RTMP, permettant aux développeurs de créer de puissantes applications de streaming avec un effort minimal.

Ce guide couvre les détails d'implémentation de la diffusion RTMP dans les différents produits VisioForge, y compris les solutions multiplateformes et les intégrations spécifiques à Windows.

## Implémentation RTMP multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La classe `RTMPOutput` constitue le point central de configuration pour la diffusion RTMP dans les scénarios multiplateformes. Elle implémente plusieurs interfaces, notamment `IVideoEditXBaseOutput` et `IVideoCaptureXBaseOutput`, ce qui la rend polyvalente pour les workflows d'édition vidéo comme de capture.

### Configuration de la sortie RTMP

Pour commencer à implémenter la diffusion RTMP, vous devez créer et configurer une instance `RTMPOutput` :

```csharp
// Initialiser avec l'URL de diffusion
var rtmpOutput = new RTMPOutput("rtmp://your-streaming-server/stream-key");

// Autre option : définir l'URL après l'initialisation
var rtmpOutput = new RTMPOutput();
rtmpOutput.Sink.Location = "rtmp://your-streaming-server/stream-key";
```

### Intégration avec les SDK VisioForge

#### Intégration au Video Capture SDK

```csharp
// Ajouter la sortie RTMP au moteur Video Capture SDK
core.Outputs_Add(rtmpOutput, true); // core est une instance de VideoCaptureCoreX
```

#### Intégration au Video Edit SDK

```csharp
// Définir RTMP comme format de sortie pour Video Edit SDK
core.Output_Format = rtmpOutput; // core est une instance de VideoEditCoreX
```

#### Intégration au Media Blocks SDK

```csharp
// Créer un bloc puits RTMP
var rtmpSink = new RTMPSinkBlock(new RTMPSinkSettings() 
{ 
    Location = "rtmp://streaming-server/stream" 
});

// Connecter les encodeurs vidéo et audio au puits RTMP
pipeline.Connect(h264Encoder.Output, rtmpSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(aacEncoder.Output, rtmpSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Configuration de l'encodeur vidéo

### Encodeurs vidéo pris en charge

VisioForge offre une prise en charge étendue de divers encodeurs vidéo, permettant d'optimiser la diffusion selon le matériel disponible :

- **OpenH264** : encodeur logiciel par défaut pour la plupart des plateformes
- **NVENC H264** : encodage accéléré matériellement pour les GPU NVIDIA
- **QSV H264** : accélération Intel Quick Sync Video
- **AMF H264** : accélération basée sur les GPU AMD
- **HEVC/H265** : diverses implémentations incluant MF HEVC, NVENC HEVC, QSV HEVC et AMF H265

### Implémentation de l'encodage accéléré matériellement

Pour des performances optimales, il est recommandé d'utiliser l'accélération matérielle lorsqu'elle est disponible :

```csharp
// Vérifier la disponibilité de l'encodeur NVIDIA et l'utiliser si présent
if (NVENCH264EncoderSettings.IsAvailable())
{
    rtmpOutput.Video = new NVENCH264EncoderSettings();
}
// Se rabattre sur OpenH264 si l'accélération matérielle n'est pas disponible
else
{
    rtmpOutput.Video = new OpenH264EncoderSettings();
}
```

## Configuration de l'encodeur audio

### Encodeurs audio pris en charge

Le SDK prend en charge plusieurs implémentations de l'encodeur AAC :

- **VO-AAC** : par défaut pour les plateformes non Windows
- **AVENC AAC** : implémentation multiplateforme
- **MF AAC** : par défaut pour les plateformes Windows

```csharp
// Configurer l'encodeur MF AAC sur les plateformes Windows
rtmpOutput.Audio = new MFAACEncoderSettings();

// Pour macOS ou d'autres plateformes
rtmpOutput.Audio = new VOAACEncoderSettings();
```

## Considérations spécifiques à chaque plateforme

### Implémentation Windows

Sur les plateformes Windows, la configuration par défaut utilise :
- OpenH264 pour l'encodage vidéo
- MF AAC pour l'encodage audio

Par ailleurs, Windows prend en charge l'encodage HEVC Microsoft Media Foundation pour une diffusion à haute efficacité.

### Implémentation macOS

Pour les applications macOS, le système utilise :
- AppleMediaH264EncoderSettings pour l'encodage vidéo
- VO-AAC pour l'encodage audio

### Détection automatique de la plateforme

Le SDK gère automatiquement les différences entre plateformes par compilation conditionnelle :

```csharp
#if __MACOS__
    Video = new AppleMediaH264EncoderSettings();
#else
    Video = new OpenH264EncoderSettings();
#endif
```

## Bonnes pratiques pour la diffusion RTMP

### 1. Stratégie de sélection de l'encodeur

Vérifiez toujours la disponibilité de l'encodeur avant de tenter d'utiliser l'accélération matérielle :

```csharp
// Vérifier la disponibilité d'Intel Quick Sync
if (QSVH264EncoderSettings.IsAvailable())
{
    rtmpOutput.Video = new QSVH264EncoderSettings();
}
// Vérifier l'accélération NVIDIA
else if (NVENCH264EncoderSettings.IsAvailable())
{
    rtmpOutput.Video = new NVENCH264EncoderSettings();
}
// Se rabattre sur l'encodage logiciel
else
{
    rtmpOutput.Video = new OpenH264EncoderSettings();
}
```

### 2. Gestion des erreurs

Implémentez une gestion d'erreurs robuste pour gérer élégamment les échecs de diffusion :

```csharp
try
{
    var rtmpOutput = new RTMPOutput(streamUrl);
    // Configurer et démarrer la diffusion
}
catch (Exception ex)
{
    logger.LogError($"RTMP streaming initialization failed: {ex.Message}");
    // Implémenter une stratégie appropriée de récupération d'erreur
}
```

### 3. Gestion des ressources

Assurez la libération correcte des ressources une fois la diffusion terminée :

```csharp
// RTMPOutput lui-même est un simple objet de paramètres et n'implémente pas IDisposable.
// Arrêtez et libérez le pipeline/core qui le possède à la place.
await core.StopAsync();
await core.DisposeAsync();  // VideoCaptureCoreX / MediaBlocksPipeline disposent tous deux via DisposeAsync
rtmpOutput = null;
```

## Configuration RTMP avancée

### Sélection dynamique de l'encodeur

Pour les applications devant s'adapter à différents environnements, vous pouvez énumérer les encodeurs disponibles :

```csharp
var rtmpOutput = new RTMPOutput();
var availableVideoEncoders = rtmpOutput.GetVideoEncoders();
var availableAudioEncoders = rtmpOutput.GetAudioEncoders();

// Présenter les options aux utilisateurs ou sélectionner selon les capacités du système
```

### Configuration personnalisée du puits

Affinez les paramètres de diffusion avec la classe RTMPSinkSettings :

```csharp
rtmpOutput.Sink = new RTMPSinkSettings
{
    Location = "rtmp://streaming-server/stream"
};
```

## Implémentation RTMP spécifique à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Pour les applications Windows uniquement, VisioForge propose une implémentation alternative basée sur FFmpeg :

```csharp
// Activer la diffusion réseau
VideoCapture1.Network_Streaming_Enabled = true;

// Définir le format de diffusion sur RTMP via FFmpeg
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.RTMP_FFMPEG_EXE;

// Créer et configurer la sortie FFmpeg
var ffmpegOutput = new FFMPEGEXEOutput();
ffmpegOutput.FillDefaults(DefaultsProfile.MP4_H264_AAC, true);
ffmpegOutput.OutputMuxer = OutputMuxer.FLV;

// Assigner la sortie au composant de capture
VideoCapture1.Network_Streaming_Output = ffmpegOutput;

// Activer la diffusion audio (requise pour de nombreux services)
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

## Diffusion vers les plateformes populaires

### YouTube Live

```csharp
// Format : rtmp://a.rtmp.youtube.com/live2/ + [clé de flux YouTube]
VideoCapture1.Network_Streaming_URL = "rtmp://a.rtmp.youtube.com/live2/xxxx-xxxx-xxxx-xxxx";
```

### Facebook Live

```csharp
// Format : rtmps://live-api-s.facebook.com:443/rtmp/ + [clé de flux Facebook]
VideoCapture1.Network_Streaming_URL = "rtmps://live-api-s.facebook.com:443/rtmp/xxxx-xxxx-xxxx-xxxx";
```

### Serveurs RTMP personnalisés

```csharp
// Se connecter à n'importe quel serveur RTMP
VideoCapture1.Network_Streaming_URL = "rtmp://your-streaming-server:1935/live/stream";
```

## Optimisation des performances

Pour obtenir des performances de diffusion optimales :

1. **Utilisez l'accélération matérielle** lorsque disponible afin de réduire la charge CPU
2. **Surveillez l'utilisation des ressources** durant la diffusion pour identifier les goulots d'étranglement
3. **Ajustez la résolution et le débit binaire** en fonction de la bande passante disponible
4. **Implémentez un débit binaire adaptatif** pour faire face aux conditions réseau variables
5. **Pensez à la taille du GOP** et aux intervalles d'images-clés pour la qualité du flux

## Résolution des problèmes courants

- **Échecs de connexion** : vérifiez le format de l'URL du serveur et la connectivité réseau
- **Erreurs d'encodeur** : confirmez la disponibilité de l'encodeur matériel et des pilotes
- **Problèmes de performance** : surveillez l'utilisation CPU/GPU et ajustez les paramètres d'encodage
- **Synchronisation audio/vidéo** : vérifiez les paramètres de synchronisation des horodatages

## Conclusion

L'implémentation RTMP de VisioForge offre aux développeurs un cadre puissant et flexible pour créer des applications de diffusion robustes. En tirant parti des composants SDK appropriés et en suivant les bonnes pratiques décrites dans ce guide, vous pouvez créer des solutions de diffusion haute performance fonctionnant sur plusieurs plateformes et intégrables aux services de streaming populaires.

## Ressources associées

- [Diffusion vers Adobe Flash Media Server](adobe-flash.md)
- [Intégration de la diffusion YouTube](youtube.md)
- [Implémentation Facebook Live](facebook.md)
