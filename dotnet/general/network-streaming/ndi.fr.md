---
title: Diffusion vidéo et audio NDI sur réseau IP en C# .NET
description: Diffusez vidéo et audio en NDI depuis caméras, fichiers et périphériques en C# .NET. Configuration, exemples SDK, rééchantillonnage audio et dépannage.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - GStreamer
  - Capture
  - Streaming
  - Editing
  - Webcam
  - NDI
  - MP4
  - C#
primary_api_classes:
  - NDISinkBlock
  - AudioResamplerBlock
  - NDIOutput
  - MediaBlockPadMediaType
  - MediaBlocksPipeline

---

# Intégration de la diffusion Network Device Interface (NDI)

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Qu'est-ce que NDI ?

Network Device Interface (NDI) est un standard industriel pour la production vidéo en direct sur réseaux IP. Il permet le streaming vidéo et audio de haute qualité à faible latence sur Ethernet standard, remplaçant le câblage SDI coûteux par des workflows logiciels. Les cas d'usage courants comprennent :

- Diffusion en direct et streaming
- Visioconférence professionnelle
- Configurations de production multi-caméras
- Workflows de production à distance
- Applications de serveur de diffusion

Le SDK VisioForge prend en charge la sortie NDI pour les workflows de bureau/serveur, vous permettant de diffuser des caméras, des périphériques de capture ou des fichiers multimédias vers des récepteurs NDI sur votre réseau.

## Exigences d'installation

Pour utiliser la diffusion NDI, installez l'un des paquets NDI officiels suivants :

1. **[NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/)** - Recommandé pour les développeurs
2. **[NDI Tools](https://ndi.video/tools/)** - Adapté aux tests

Ils fournissent les composants d'exécution qui permettent la communication NDI. Vous pouvez vérifier la disponibilité de NDI dans le code :

```csharp
bool ndiAvailable = NDISinkBlock.IsAvailable();
```

## Sortie NDI multiplateforme

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Classe NDIOutput

La classe `NDIOutput` fournit la sortie NDI pour les moteurs VideoCaptureCoreX et VideoEditCoreX :

```csharp
public class NDIOutput : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

#### Configuration

| Propriété | Type | Description |
|----------|------|-------------|
| `Sink` | `NDISinkSettings` | Configuration de la sortie NDI (nom du flux, compression, paramètres réseau) |
| `CustomVideoProcessor` | `MediaBlock` | Traitement vidéo personnalisé facultatif avant transmission NDI |
| `CustomAudioProcessor` | `MediaBlock` | Traitement audio personnalisé facultatif avant transmission NDI |

#### Constructeurs

```csharp
// Créer avec un nom de flux
var output = new NDIOutput("My Stream");

// Créer avec des paramètres préconfigurés
var output = new NDIOutput(new NDISinkSettings("My Stream"));
```

## Exemples d'implémentation

### Media Blocks SDK

```cs
// Créer un bloc de sortie NDI avec un nom de flux descriptif
var ndiSink = new NDISinkBlock("VisioForge Production Stream");

// Connecter la source vidéo à la sortie NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connecter la source audio à la sortie NDI
pipeline.Connect(audioSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

### Video Capture SDK

```cs
// Initialiser la sortie NDI avec un nom de flux adapté au réseau
var ndiOutput = new NDIOutput("VisioForge_Studio_Output");

// Ajouter la sortie NDI configurée au pipeline de capture vidéo
core.Outputs_Add(ndiOutput); // core représente l'instance VideoCaptureCoreX
```

## Diffusion d'une caméra vers NDI

[MediaBlocksPipeline](#){ .md-button }

Le cas d'usage le plus courant est la diffusion d'une webcam et d'un microphone locaux vers NDI. Cet exemple utilise le Media Blocks SDK pour capturer depuis les périphériques système et envoyer vers NDI avec un rééchantillonnage audio correct.

### Architecture du pipeline

```text
SystemVideoSourceBlock → NDISinkBlock (entrée vidéo)
SystemAudioSourceBlock → AudioResamplerBlock (48kHz, F32LE, stéréo) → NDISinkBlock (entrée audio)
```

### Exemple de code

```cs
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.AudioProcessing;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioEncoders;

// Initialiser le SDK une fois au démarrage
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Énumérer les périphériques disponibles
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Configurer la source vidéo (première caméra disponible)
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Configurer la source audio (premier microphone disponible)
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Créer la sortie NDI
var ndiSink = new NDISinkBlock("My Camera Stream");

// Connecter la vidéo directement à NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Rééchantillonner l'audio en 48 kHz F32LE stéréo (requis par NDI)
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(audioSource.Output, audioResampler.Input);
pipeline.Connect(audioResampler.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

## Implémentation NDI spécifique à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Pour les implémentations spécifiques à Windows, le SDK fournit des options de configuration supplémentaires via les composants VideoCaptureCore ou VideoEditCore.

### Guide d'implémentation pas à pas

#### 1. Activer la diffusion réseau

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

#### 2. Configurer la diffusion audio

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

#### 3. Sélectionner le protocole NDI

```csharp
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.NDI;
```

#### 4. Créer et configurer la sortie NDI

```cs
var streamName = "VisioForge NDI Streamer";
var ndiOutput = new NDIOutput(streamName);
```

#### 5. Assigner la sortie

```cs
VideoCapture1.Network_Streaming_Output = ndiOutput;
```

#### 6. Générer l'URL NDI (facultatif)

```cs
string ndiUrl = $"ndi://{System.Net.Dns.GetHostName()}/{streamName}";
Debug.WriteLine(ndiUrl);
```

## Lecture de fichier vers sortie NDI

Le Media Blocks SDK peut diffuser des fichiers multimédias locaux (MP4, MKV, AVI, etc.) directement vers NDI sans rendu local, idéal pour les applications de serveur de diffusion.

### Pipeline recommandé

```text
UniversalSourceBlock (fichier)
  VideoOutput → NDISinkBlock (entrée vidéo)
  AudioOutput → AudioResamplerBlock (48kHz, F32LE, stéréo) → NDISinkBlock (entrée audio)
```

### Exemple de code

```cs
var pipeline = new MediaBlocksPipeline();

// Source fichier avec détection automatique du format
var fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(new Uri("file:///path/to/video.mp4")));

// Sortie NDI
var ndiSink = new NDISinkBlock("My NDI Stream");

// Connecter la vidéo directement à NDI
pipeline.Connect(fileSource.VideoOutput,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Rééchantillonner l'audio en 48 kHz F32LE stéréo pour compatibilité NDI
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(fileSource.AudioOutput, audioResampler.Input);
pipeline.Connect(audioResampler.Output,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Facultatif : activer la lecture en boucle
pipeline.Loop = true;

await pipeline.StartAsync();
```

## Exigences de format audio

NDI exige de l'audio **48 kHz, virgule flottante 32 bits (F32LE), entrelacé**. Lors d'une diffusion depuis des sources pouvant contenir de l'audio à d'autres fréquences d'échantillonnage (par exemple, AAC 44,1 kHz dans des fichiers MP4 ou taux variables des microphones), incluez toujours un `AudioResamplerBlock` pour convertir en 48 kHz. Sans rééchantillonnage, l'audio peut saccader, présenter des artefacts ou perdre la synchronisation avec la vidéo.

```cs
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
```

## Réception de sources NDI

Ce guide se concentre sur l'envoi de vidéo et d'audio vers NDI. Pour découvrir, vous connecter, prévisualiser ou capturer des sources NDI, y compris les applications de lecteur NDI Android et MAUI, consultez la [référence de la source vidéo NDI](../../videocapture/video-sources/ip-cameras/ndi.md).

## Résolution des problèmes

### Saccades ou artefacts audio en sortie NDI

**Symptôme :** l'audio n'est pas fluide lors de la diffusion depuis une source fichier — saccades, artefacts ou problèmes de synchronisation labiale.

**Cause :** le fichier source contient de l'audio à une fréquence d'échantillonnage autre que 48 kHz (par exemple, AAC 44,1 kHz dans des fichiers MP4). NDI attend de l'audio à 48 kHz.

**Solution :** insérez un `AudioResamplerBlock` configuré pour 48 kHz F32LE stéréo entre la source fichier et le puits NDI, comme indiqué dans l'exemple de lecture de fichier ci-dessus.

## Questions fréquentes

### Comment diffuser de la vidéo depuis une caméra vers NDI en C# ?

Utilisez le Media Blocks SDK pour créer un pipeline avec `SystemVideoSourceBlock` pour la caméra, `SystemAudioSourceBlock` pour le microphone et `NDISinkBlock` comme sortie. Connectez l'audio via un `AudioResamplerBlock` réglé sur 48 kHz F32LE stéréo, requis par NDI. Voir la section [Diffusion d'une caméra vers NDI](#diffusion-dune-camera-vers-ndi) pour le code complet.

### Quel format audio NDI exige-t-il ?

NDI exige de l'audio stéréo entrelacé 48 kHz, virgule flottante 32 bits (F32LE). Incluez toujours un `AudioResamplerBlock(new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2))` dans votre pipeline entre la source audio et le puits NDI. Sans rééchantillonnage correct, vous pouvez rencontrer des saccades audio, des artefacts ou des problèmes de synchronisation A/V.

### Puis-je diffuser un fichier vidéo vers NDI pour la diffusion ?

Oui. Utilisez `UniversalSourceBlock` pour lire le fichier, connectez la vidéo directement à `NDISinkBlock` et acheminez l'audio via `AudioResamplerBlock` pour la conversion en 48 kHz. Activez `pipeline.Loop = true` pour une diffusion continue. Ce schéma est idéal pour les serveurs de diffusion broadcast sans surcharge de rendu local.

### Quelles sont les exigences système pour la diffusion NDI en .NET ?

Vous avez besoin du [NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/) ou de [NDI Tools](https://ndi.video/tools/) installé pour la prise en charge NDI à l'exécution. Le SDK VisioForge prend en charge Windows, macOS et Linux. Vérifiez la disponibilité de NDI à l'exécution avec `NDISinkBlock.IsAvailable()`. Les exigences de bande passante réseau dépendent de la résolution et de la fréquence d'images — un flux NDI HD typique utilise environ 100 à 150 Mbit/s.

### Comment vérifier si NDI est disponible sur le système ?

Appelez `NDISinkBlock.IsAvailable()` avant de créer les composants du pipeline NDI. Cette méthode statique vérifie si les bibliothèques d'exécution NDI sont installées et accessibles. Si elle retourne `false`, invitez l'utilisateur à installer le paquet NDI SDK ou NDI Tools.

## Voir aussi

- [Référence de la source vidéo NDI](../../videocapture/video-sources/ip-cameras/ndi.md) — réception et capture de sources NDI en .NET
- [Visionneuse de flux RTSP et lecteur de caméra IP](../../mediablocks/Guides/rtsp-player-csharp.md) — guide de diffusion IP similaire pour les caméras RTSP
- [Guide de déploiement](../../deployment-x/index.md) — paquets d'exécution spécifiques aux plateformes pour Windows, macOS, Linux
- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — démos diffuseur et source NDI
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — page produit et téléchargements
