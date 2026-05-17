---
title: Serveur RTSP en C# .NET — guide développeur VisioForge
description: Créez des serveurs de streaming RTSP pour la diffusion audio/vidéo à faible latence avec prise en charge H.264/H.265 grâce au VisioForge Media Blocks SDK.
sidebar_label: Serveur RTSP
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
  - RTSPServerBlock
  - RTSPServerSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - SystemVideoSourceBlock

---

# Bloc serveur RTSP - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le `RTSPServerBlock` fonctionne sous **Windows, macOS et Linux** via GStreamer (nécessite le plugin `gst-rtsp-server`). Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour la configuration Ubuntu / NVIDIA Jetson / Raspberry Pi.

Le bloc serveur RTSP crée un point de terminaison serveur RTSP (Real-Time Streaming Protocol) pour diffuser du contenu audio/vidéo sur les réseaux. Les clients peuvent se connecter pour recevoir des flux multimédias en direct ou enregistrés avec une faible latence.

## Vue d'ensemble

Le RTSPServerBlock fournit une implémentation complète de serveur de streaming RTSP avec les capacités suivantes :

- **Prise en charge de plusieurs clients** : gère les connexions simultanées depuis plusieurs clients RTSP
- **Conformité aux standards** : compatible avec VLC, FFmpeg, GStreamer et les visualiseurs de caméras IP
- **Codecs vidéo** : H.264, H.265 et autres formats compressés
- **Codecs audio** : AAC, MP3, Opus et autres formats
- **Protocoles de transport** : RTP/RTCP pour une livraison média fiable
- **Authentification** : prise en charge facultative de l'authentification RTSP
- **Configurable** : liaison de port personnalisée et points de montage
- **Faible latence** : optimisé pour les applications de streaming en temps réel

## Informations sur le bloc

Nom : RTSPServerBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Entrée vidéo | vidéo compressée (H.264, H.265) | 1 |
| Entrée audio | audio compressé (AAC, MP3, etc.) | 1 |

## Pipeline d'exemple

```mermaid
graph LR;
    SystemVideoSourceBlock-->H264EncoderBlock;
    SystemAudioSourceBlock-->AACEncoderBlock;
    H264EncoderBlock-->RTSPServerBlock;
    AACEncoderBlock-->RTSPServerBlock;
```

## Paramètres

Le RTSPServerBlock est configuré via `RTSPServerSettings` :

### Propriétés de RTSPServerSettings

- `Port` (`int`) : port TCP du serveur RTSP. Par défaut `8554`.
- `Point` (`string`) : chemin URL sous lequel le flux est servi (par exemple `/live`). Par défaut `/live`.
- `Username` (`string`) : nom d'utilisateur facultatif pour l'authentification RTSP de base.
- `Password` (`string`) : mot de passe facultatif pour l'authentification RTSP de base.
- `Address` (`string`) : adresse à laquelle le serveur se lie. Par défaut `127.0.0.1` ; définir à `0.0.0.0` pour écouter sur toutes les interfaces.
- `Name` (`string`) : nom d'affichage du serveur annoncé aux clients. Par défaut `"VisioForge RTSP Server"`.
- `Description` (`string`) : description lisible par l'humain. Par défaut `"VisioForge RTSP Server"`.
- `Latency` (`TimeSpan`) : latence de mise en tampon appliquée par le serveur. Par défaut `250 ms`.
- `URL` (`string`, lecture seule) : URL `rtsp://{Address}:{Port}{Point}` calculée — utilisez-la après la construction des paramètres pour journaliser l'URL côté client.
- `VideoEncoder` (`IVideoEncoder`) : paramètres de l'encodeur vidéo. Passer `null` pour désactiver le flux vidéo.
- `AudioEncoder` (`IAudioEncoder`) : paramètres de l'encodeur audio. Passer `null` pour désactiver le flux audio.

`RTSPServerSettings` **n'a pas de constructeur sans paramètres**. Utilisez l'un de :

- `new RTSPServerSettings(IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)`
- `new RTSPServerSettings(Uri uri, IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)` — analyse hôte/port/point depuis l'URI.

## Exemple de code

### Serveur RTSP de base

```csharp
var pipeline = new MediaBlocksPipeline();

// Créer la source vidéo
var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
var videoFormat = videoDevice.VideoFormats[0];
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
{
    Format = videoFormat.ToFormat()
};
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Créer la source audio
var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
var audioFormat = audioDevice.Formats[0];
var audioSettings = audioDevice.CreateSourceSettings(audioFormat.ToFormat());
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Créer l'encodeur vidéo (classe de paramètres concrète — utilisez NVENC/QSV/AMF lorsque disponibles, OpenH264 comme solution de repli portable)
var h264Settings = new OpenH264EncoderSettings
{
    Bitrate = 2000 // kbps
};
var h264Encoder = new H264EncoderBlock(h264Settings);
pipeline.Connect(videoSource.Output, h264Encoder.Input);

// Créer l'encodeur audio (AAC multiplateforme)
var aacSettings = new VOAACEncoderSettings
{
    Bitrate = 128 // kbps
};
var aacEncoder = new AACEncoderBlock(aacSettings);
pipeline.Connect(audioSource.Output, aacEncoder.Input);

// Créer le serveur RTSP (le ctor de Settings prend videoEncoder + audioEncoder).
// Comme nous pré-encodons ci-dessus, passez null pour les deux — le RTSPServerBlock se contente
// de transmettre les flux encodés. Passez des encodeurs concrets uniquement si vous souhaitez que le serveur encode en interne.
var rtspSettings = new RTSPServerSettings(videoEncoder: null, audioEncoder: null)
{
    Port = 8554,
    Point = "/live",
    Address = "0.0.0.0"   // écouter sur toutes les interfaces ; par défaut 127.0.0.1
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(h264Encoder.Output, rtspServer.VideoInput);
pipeline.Connect(aacEncoder.Output, rtspServer.AudioInput);

// Démarrer la diffusion
await pipeline.StartAsync();

// rtspSettings.URL renvoie l'URL côté client :
Console.WriteLine($"RTSP server started at {rtspSettings.URL}");
Console.WriteLine($"Connect with: vlc {rtspSettings.URL}");
```

### Laisser le serveur encoder en interne

Si vous préférez transmettre les images caméra brutes directement au serveur RTSP et laisser le serveur effectuer son propre encodage H.264, construisez `RTSPServerSettings` avec des paramètres d'encodeur concrets et omettez les `H264EncoderBlock` / `AACEncoderBlock` autonomes :

```csharp
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: null)
{
    Port = 8554,
    Point = "/live",
    Address = "0.0.0.0"
};

var rtspServer = new RTSPServerBlock(rtspSettings);

// Connecter des sources brutes (non encodées) — le serveur encodera.
pipeline.Connect(videoSource.Output, rtspServer.VideoInput);

await pipeline.StartAsync();
Console.WriteLine(rtspSettings.URL);
```

### Serveur RTSP avec authentification

```csharp
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/secure",
    Username = "admin",
    Password = "password123"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Configurer les encodeurs et connecter comme ci-dessus
// ...

await pipeline.StartAsync();

// Les clients doivent s'authentifier : rtsp://admin:password123@localhost:8554/secure
Console.WriteLine($"Secure RTSP server started at {rtspSettings.URL}");
```

### Diffusion de fichier via RTSP

```csharp
var pipeline = new MediaBlocksPipeline();

// Utiliser un fichier comme source
var fileSettings = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
var fileSource = new UniversalSourceBlock(fileSettings);

// Créer le serveur RTSP — laissez-le encoder les deux flux en interne
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/vod"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Connecter la vidéo et l'audio bruts du fichier
pipeline.Connect(fileSource.VideoOutput, rtspServer.VideoInput);
pipeline.Connect(fileSource.AudioOutput, rtspServer.AudioInput);

await pipeline.StartAsync();

// Diffuser le contenu du fichier via RTSP
Console.WriteLine($"Video-on-Demand RTSP server started at {rtspSettings.URL}");
```

## Connexion client

Les clients peuvent se connecter au serveur RTSP via le format d'URL :

```
rtsp://hostname:port/mountpoint
```

Exemples :
- `rtsp://localhost:8554/live`
- `rtsp://192.168.1.100:8554/stream1`
- `rtsp://username:password@server.com:8554/secure`

### Lecteur VLC

```bash
vlc rtsp://localhost:8554/live
```

### FFmpeg

```bash
ffplay rtsp://localhost:8554/live
ffmpeg -i rtsp://localhost:8554/live -c copy output.mp4
```

### GStreamer

```bash
gst-launch-1.0 rtspsrc location=rtsp://localhost:8554/live ! decodebin ! autovideosink
```

## Cas d'usage

- **Diffusion en direct** : diffusez les flux de caméras en direct sur les réseaux
- **Systèmes de sécurité** : diffusez les caméras de vidéosurveillance vers les stations de supervision
- **Distribution vidéo** : distribuez le contenu multimédia à plusieurs clients
- **Surveillance à distance** : permettez la visualisation à distance des procédés industriels
- **Broadcasting** : créez des solutions de streaming à faible latence
- **Visioconférence** : diffusez les flux des participants vers les spectateurs
- **Caméras IoT** : fournissez un point de terminaison RTSP pour les caméras intelligentes

## Considérations de performance

- **Encodage** : la vidéo/audio doit être encodée avant la diffusion
- **Bande passante** : surveillez la bande passante réseau pour plusieurs clients
- **Latence** : optimisez les paramètres de l'encodeur pour les exigences de faible latence
- **Clients** : chaque client consomme des ressources serveur et de la bande passante
- **Port** : assurez-vous que le pare-feu autorise le trafic sur le port configuré
- **Multicast** : utilisez-le pour une diffusion un-vers-plusieurs efficace sur les réseaux locaux

## Remarques

- Les flux vidéo et audio doivent être compressés/encodés avant d'être transmis au serveur RTSP
- Le serveur prend en charge H.264/H.265 pour la vidéo et AAC/MP3/Opus pour l'audio
- Le port 8554 est le port RTSP standard mais peut être modifié
- Le point de montage définit le chemin URL d'accès au flux
- Le serveur continue de diffuser tant que le pipeline est actif
- Plusieurs instances de RTSPServerBlock peuvent fonctionner simultanément sur des ports différents
- Utilisez l'authentification pour des environnements de streaming sécurisés

## Plateformes

Windows, macOS, Linux.

Remarque : nécessite GStreamer avec la prise en charge serveur RTSP (plugin gst-rtsp-server).

## Applications d'exemple

- [Serveur RTSP webcam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Webcam%20Server)

## Blocs associés

- [H264EncoderBlock](../VideoEncoders/index.md#h264-encoder) - Encodage vidéo H.264
- [H265EncoderBlock](../VideoEncoders/index.md#hevch265-encoder) - Encodage vidéo H.265
- [AACEncoderBlock](../AudioEncoders/index.md#aac-encoder) - Encodage audio AAC
- [SystemVideoSourceBlock](../Sources/index.md#system-video-source) - Capture caméra
- [SystemAudioSourceBlock](../Sources/index.md#system-audio-source) - Capture audio

## Guides associés

- [Approfondissement du protocole RTSP](../../general/network-streaming/rtsp.md) — fonctionnement interne de RTSP
- [Configuration de source caméra RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — consommez des flux RTSP dans vos applications
- [Lecteur RTSP Media Blocks](../Guides/rtsp-player-csharp.md) — un pipeline client pour les flux RTSP (à associer à ce serveur)
- [Enregistrer un flux RTSP sans réencodage](../Guides/rtsp-save-original-stream.md) — archivez les flux d'une source RTSP sur disque
- [Reconnexion RTSP et solution de repli](../../general/network-sources/reconnection-and-fallback.md) — gérez les coupures de source amont avec des événements de reconnexion et `FallbackSwitch` lorsque vous alimentez ce serveur depuis une caméra RTSP
