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
  - UniversalSourceBlock
video:
  id: YP5rS11zoNc
  upload_date: 2026-11-22T09:36:39Z
  duration: PT46S

---

# Bloc serveur RTSP - VisioForge Media Blocks SDK .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    Le `RTSPServerBlock` fonctionne sous **Windows, macOS et Linux** via GStreamer (nécessite le plugin `gst-rtsp-server`). Consultez la [matrice de prise en charge des plateformes](../../platform-matrix.md) pour les détails sur les codecs et l'accélération matérielle, ainsi que le [guide de déploiement Linux](../../deployment-x/Ubuntu.md) pour la configuration Ubuntu / NVIDIA Jetson / Raspberry Pi.

Le bloc serveur RTSP crée un point de terminaison serveur RTSP (Real-Time Streaming Protocol) pour diffuser du contenu audio/vidéo sur les réseaux. Les clients peuvent se connecter pour recevoir des flux multimédias en direct ou enregistrés avec une faible latence.

## Tutoriel vidéo : diffuser un fichier MP4 via un serveur RTSP en quelques lignes de C\#

Une application console qui sert un fichier vidéo en flux RTSP : `UniversalSourceBlock` lit le fichier, et `RTSPServerBlock` l'encode en H.264, et en AAC si le fichier a de l'audio, puis le sert à l'adresse `rtsp://127.0.0.1:8554/live`.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YP5rS11zoNc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Son code : [rtsp-server-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/rtsp-server-short-console) sur GitHub, et [Diffusion de fichier via RTSP](#diffusion-de-fichier-via-rtsp) plus bas.

## Vue d'ensemble

Le RTSPServerBlock fournit une implémentation complète de serveur de streaming RTSP avec les capacités suivantes :

- **Prise en charge de plusieurs clients** : gère les connexions simultanées depuis plusieurs clients RTSP, qui partagent un seul flux encodé
- **Conformité aux standards** : compatible avec VLC, FFmpeg, GStreamer et les visualiseurs de caméras IP
- **Encodage intégré** : le bloc reçoit la vidéo et l'audio non compressés et les encode lui-même
- **Codecs vidéo** : H.264, H.265 (HEVC) et VP9
- **Codecs audio** : AAC, MP3, Opus et Vorbis
- **Protocoles de transport** : RTP/RTCP pour une livraison média fiable
- **Authentification** : prise en charge facultative de l'authentification RTSP
- **Configurable** : port, point de montage et adresse d'écoute personnalisés
- **Faible latence** : optimisé pour les applications de streaming en temps réel

## Informations sur le bloc

Nom : RTSPServerBlock.

| Direction du pin | Type de média | Nombre de pins |
| --- | :---: | :---: |
| Entrée vidéo | vidéo non compressée, encodée avec `VideoEncoder` | 1 |
| Entrée audio | audio non compressé, encodé avec `AudioEncoder` | 1 |

## Pipeline d'exemple

```mermaid
graph LR;
    SystemVideoSourceBlock-->RTSPServerBlock;
    SystemAudioSourceBlock-->RTSPServerBlock;
```

## Paramètres

Le RTSPServerBlock est configuré via `RTSPServerSettings` :

### Propriétés de RTSPServerSettings

- `Port` (`int`) : port TCP du serveur RTSP. Par défaut `8554`.
- `Point` (`string`) : chemin URL sous lequel le flux est servi (par exemple `/live`). Par défaut `/live`.
- `Username` (`string`) : nom d'utilisateur facultatif pour l'authentification RTSP de base.
- `Password` (`string`) : mot de passe facultatif pour l'authentification RTSP de base.
- `Address` (`string`) : nom d'hôte ou adresse IP par lesquels les clients joignent le serveur. Elle ne définit que l'hôte de `URL` ; elle ne choisit pas les interfaces sur lesquelles le serveur écoute. Par défaut `127.0.0.1`.
- `ListenAddress` (`string`) : adresse IP locale sur laquelle le serveur écoute. Par défaut `0.0.0.0`, toutes les interfaces IPv4 ; `127.0.0.1` n'accepte que les clients de la même machine. Si vous indiquez ici une adresse précise, définissez aussi `Address` pour que `URL` la reprenne.
- `Name` (`string`) : nom d'affichage du serveur annoncé aux clients. Par défaut `"VisioForge RTSP Server"`.
- `Description` (`string`) : description lisible par l'humain. Par défaut `"VisioForge RTSP Server"`.
- `Latency` (`TimeSpan`) : latence de mise en tampon appliquée par le serveur. Par défaut `250 ms`.
- `URL` (`string`, lecture seule) : URL `rtsp://{Address}:{Port}{Point}` calculée — utilisez-la après la construction des paramètres pour journaliser l'URL côté client.
- `VideoEncoder` (`IVideoEncoder`) : paramètres de l'encodeur vidéo : H.264, H.265 ou VP9. Passer `null` pour désactiver le flux vidéo.
- `AudioEncoder` (`IAudioEncoder`) : paramètres de l'encodeur audio : AAC, MP3, Opus ou Vorbis. Passer `null` pour désactiver le flux audio.

`RTSPServerSettings` **n'a pas de constructeur sans paramètres**. Utilisez l'un de :

- `new RTSPServerSettings(IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)`
- `new RTSPServerSettings(Uri uri, IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)` — analyse hôte/port/point depuis l'URI ; il ne définit pas `ListenAddress` : pour un hôte IPv6, définissez aussi `ListenAddress` sur une adresse IPv6.

## Exemple de code

### Serveur RTSP de base

Connectez la caméra et le microphone directement au serveur : il les encode avec les encodeurs indiqués dans ses paramètres.

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

// Créer le serveur RTSP : il encode la vidéo en H.264 et l'audio en AAC
var rtspSettings = new RTSPServerSettings(
    H264EncoderBlock.GetDefaultSettings(),
    AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/live"
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(videoSource.Output, rtspServer.VideoInput);
pipeline.Connect(audioSource.Output, rtspServer.AudioInput);

// Démarrer la diffusion
await pipeline.StartAsync();

// rtspSettings.URL renvoie l'URL côté client :
Console.WriteLine($"RTSP server started at {rtspSettings.URL}");
Console.WriteLine($"Connect with: vlc {rtspSettings.URL}");
```

Par défaut, le serveur écoute sur toutes les interfaces réseau : les autres machines le joignent à l'adresse IP de cette machine. Pour le limiter à cette machine, définissez `ListenAddress = "127.0.0.1"`.

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

// Connecter les sources comme ci-dessus
// ...

await pipeline.StartAsync();

// Les clients doivent s'authentifier : rtsp://admin:password123@localhost:8554/secure
Console.WriteLine($"Secure RTSP server started at {rtspSettings.URL}");
```

### Diffusion de fichier via RTSP

Une application console complète qui sert un fichier MP4 (ou tout autre fichier vidéo que lit `UniversalSourceBlock`) à l'adresse `rtsp://127.0.0.1:8554/live` jusqu'à l'appui sur Entrée. Passez le chemin du fichier en premier argument. L'encodeur AAC n'est défini que si le fichier a une piste audio : un fichier vidéo seule donne un flux vidéo seule.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();
await using var p = new MediaBlocksPipeline();
p.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var file = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(
        args[0]));
var aac = file.AudioOutput == null ? null
    : AACEncoderBlock.GetDefaultSettings();
var rtsp = new RTSPServerBlock(
    new RTSPServerSettings(
        H264EncoderBlock.GetDefaultSettings(),
        aac));
p.Connect(file.VideoOutput, rtsp.VideoInput);
if (aac != null)
    p.Connect(file.AudioOutput, rtsp.AudioInput);
if (!await p.StartAsync()) return;
Console.WriteLine(rtsp.Settings.URL);
Console.ReadLine();
await p.StopAsync();
```

Le fichier est lu une fois, en temps réel, dès le démarrage du pipeline : un client qui se connecte plus tard le reçoit à partir de ce point. Quand le fichier se termine, les clients connectés reçoivent des images noires jusqu'à l'arrêt de l'application.

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

- **Encodage** : le serveur encode une seule fois pour tous les clients ; un encodeur matériel (NVENC, QSV, AMF) libère le CPU
- **Bande passante** : surveillez la bande passante réseau pour plusieurs clients
- **Latence** : optimisez les paramètres de l'encodeur pour les exigences de faible latence
- **Clients** : chaque client consomme des ressources serveur et de la bande passante
- **Port** : assurez-vous que le pare-feu autorise le trafic sur le port configuré

## Remarques

- Connectez la vidéo et l'audio non compressés ; le serveur les encode avec `VideoEncoder` et `AudioEncoder`
- Le serveur prend en charge H.264, H.265 et VP9 pour la vidéo et AAC, MP3, Opus et Vorbis pour l'audio
- Le port 8554 est l'alternative habituelle au port RTSP standard 554, qui exige root sous Linux ; tout port libre convient
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

## Transcription de la vidéo

Servez un fichier vidéo en flux RTSP, en quelques lignes de C#. Chargez le SDK et créez un pipeline. La source est le fichier vidéo. `RTSPServerBlock` l'encode en H.264, et en AAC si le fichier a du son, et le sert sur le port 8554. Lancez-le avec un fichier. Il affiche l'URL, et n'importe quel lecteur RTSP peut l'ouvrir. Le film, reçu en RTSP. Cette page ajoute une caméra et un mot de passe : voir [Serveur RTSP de base](#serveur-rtsp-de-base) et [Serveur RTSP avec authentification](#serveur-rtsp-avec-authentification).

Le fichier vidéo est Big Buck Bunny, (c) copyright 2008, Blender Foundation, www.bigbuckbunny.org, CC BY 3.0.
