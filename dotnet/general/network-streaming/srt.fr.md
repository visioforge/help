---
title: Diffusion SRT en C# .NET — Envoi et réception vidéo sur IP
description: Diffusez et recevez de la vidéo via SRT en C# .NET — modes caller/listener, chiffrement AES et multiplexage MPEG-TS. Exemples SDK inclus.
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
  - IP Camera
  - NDI
  - SRT
  - TS
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - SRTSinkSettings
  - SRTMPEGTSSinkBlock
  - MediaBlocksPipeline
  - SRTSourceSettings
  - SRTSourceBlock

---

# Guide d'implémentation de la diffusion SRT

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Qu'est-ce que SRT ?

SRT (Secure Reliable Transport) est un protocole de streaming conçu pour la livraison vidéo à faible latence et de haute qualité sur des réseaux peu fiables. Il offre une récupération d'erreur intégrée, un chiffrement AES et la traversée des pare-feu, ce qui le rend idéal pour :

- La diffusion en direct sur Internet
- Les flux de contribution entre installations de production
- Le backhaul de caméras distantes via des liens cellulaires ou satellitaires
- Le transport vidéo sécurisé point à point
- L'ingestion et la distribution vidéo dans le cloud

Les SDK .NET de VisioForge prennent en charge l'envoi et la réception de flux SRT sur Windows, macOS, Mac Catalyst, Linux, iOS et Android. Les flux SRT utilisent le multiplexage MPEG-TS pour transporter ensemble la vidéo et l'audio.

Vous pouvez vérifier la disponibilité de SRT à l'exécution :

```csharp
bool srtAvailable = SRTSinkBlock.IsAvailable(); // pour l'envoi
bool srtSourceAvailable = SRTSourceBlock.IsAvailable(); // pour la réception
```

Sous Windows, macOS et Mac Catalyst, le plugin SRT est fourni dans le paquet de runtime `Adds`, et non dans le paquet principal : `VisioForge.CrossPlatform.Core.Windows.Adds.x64` (ou ses variantes x86 et arm64), `VisioForge.CrossPlatform.Core.macOS.Adds` ou `VisioForge.CrossPlatform.Core.macCatalyst.Adds`. Ajoutez-le à côté du paquet de runtime principal, sinon `IsAvailable()` renvoie `false`. Les paquets de runtime iOS et Android incluent SRT. Sous Linux, le SDK utilise le GStreamer du système ; sous Ubuntu et Debian, SRT fait partie de `gstreamer1.0-plugins-bad`.

## Modes de connexion SRT

[VideoCaptureCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

SRT prend en charge trois modes de connexion via l'énumération `SRTConnectionMode` :

| Mode | Description | Cas d'usage |
| --- | --- | --- |
| **Caller** | Se connecte à un listener distant | Client se connectant à un serveur |
| **Listener** | Attend les connexions entrantes sur un port | Serveur acceptant des connexions |
| **Rendezvous** | Les deux côtés se connectent simultanément | Point à point, traversée de pare-feu |

### Mode Listener (serveur)

Le listener attend les connexions SRT entrantes sur un port spécifié :

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener
};
```

### Mode Caller (client)

Le caller se connecte à un listener SRT distant :

```csharp
// CreateAsync lit les pistes du flux avant de rendre la main, l'émetteur doit donc être en marche. Uri est System.Uri, pas une chaîne.
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
sourceSettings.Mode = SRTConnectionMode.Caller;
```

`CreateAsync` se connecte avec la seule URI ; ce dont cette première connexion a besoin va donc dans la requête de l'URI : un récepteur en mode listener utilise `srt://0.0.0.0:8888?mode=listener`, et un serveur qui exige un stream ID reçoit `?streamid=...`.

### Mode Rendezvous

Les deux extrémités se connectent simultanément — utile lorsque les deux côtés sont derrière des pare-feu :

```csharp
var settings = new SRTSinkSettings
{
    Uri = "srt://remote-host:8888",
    Mode = SRTConnectionMode.Rendezvous,
    LocalPort = 8888
};
```

## Sortie SRT de base

### Video Capture SDK

```csharp
// Initialiser la sortie SRT avec l'URL de destination
var srtOutput = new SRTOutput("srt://streaming-server:1234");

// Ajouter la sortie SRT configurée à votre moteur de capture
videoCapture.Outputs_Add(srtOutput, true);  // videoCapture est une instance VideoCaptureCoreX
```

Dans Video Capture SDK .NET, `SRTOutput` nécessite l'édition Premium, et un flux SRT utilisé comme `Video_Source` nécessite l'édition Professional.

### Media Blocks SDK

Le `SRTMPEGTSSinkBlock` multiplexe la vidéo et l'audio dans un conteneur MPEG-TS et envoie via SRT :

```csharp
// Créer un puits SRT MPEG-TS en mode listener
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings { Uri = "srt://:8888" });

// Connecter la sortie de l'encodeur vidéo au puits SRT
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connecter la sortie de l'encodeur audio au puits SRT
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Diffusion d'une caméra en SRT

[MediaBlocksPipeline](#){ .md-button }

Cet exemple complet capture depuis une webcam et un microphone, encode en H.264/AAC et diffuse via SRT :

### Architecture du pipeline

```text
SystemVideoSourceBlock → H264EncoderBlock → SRTMPEGTSSinkBlock (entrée vidéo)
SystemAudioSourceBlock → AACEncoderBlock  → SRTMPEGTSSinkBlock (entrée audio)
```

### Exemple de code

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

// Initialiser le SDK une fois au démarrage
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Énumérer les périphériques
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Source vidéo (première caméra)
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Source audio (premier microphone)
var audioSource = new SystemAudioSourceBlock(
    new AudioCaptureDeviceSourceSettings(audioDevices[0]));

// Encodeur vidéo — H.264 avec repli matériel
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());

// Encodeur audio — AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings());

// Sortie SRT en mode listener sur le port 8888
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Latency = TimeSpan.FromMilliseconds(125)
});

// Construire le pipeline : caméra → encodeur → SRT
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

Les récepteurs peuvent se connecter avec `ffplay srt://your-ip:8888` ou tout lecteur compatible SRT.

## Réception d'un flux SRT

[MediaBlocksPipeline](#){ .md-button }

Utilisez `SRTSourceBlock` pour recevoir et lire un flux SRT avec décodage automatique :

```csharp
var pipeline = new MediaBlocksPipeline();

// Se connecter à un émetteur SRT (mode caller par défaut)
var sourceSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://192.168.1.100:8888"));
var srtSource = new SRTSourceBlock(sourceSettings);

// Moteur de rendu vidéo
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(srtSource.VideoOutput, videoRenderer.Input);

// Moteur de rendu audio
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(srtSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

Pour un enregistrement direct sans décodage (par exemple, sauvegarder le flux MPEG-TS brut), utilisez `SRTRAWSourceBlock` à la place.

## Chiffrement

SRT prend en charge le chiffrement AES avec des clés de 128, 192 ou 256 bits. L'émetteur et le récepteur doivent utiliser la même phrase secrète et la même longueur de clé.

### Émetteur (chiffré)

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Passphrase = "my-secret-passphrase",  // de 10 à 79 octets en UTF-8
    PbKeyLen = SRTKeyLength.Length32       // AES 256 bits
};
```

### Récepteur (chiffré)

`SRTSourceSettings.CreateAsync` ouvre le flux avec la seule URI pour lire ses pistes avant de rendre la main ; si vous l'utilisez pour un flux chiffré, placez donc la phrase secrète dans l'URI, sous forme de paramètres de requête `passphrase` et `pbkeylen`. Une phrase secrète occupe de 10 à 79 octets en UTF-8, soit de 10 à 79 caractères pour des lettres latines et des chiffres ; dans une URI, utilisez des lettres, des chiffres, `-`, `_`, `.` et `~` :

```csharp
// CreateAsync lit le flux avec la seule URI, la phrase secrète et la longueur de clé vont donc dans la requête
var sourceSettings = await SRTSourceSettings.CreateAsync(
    new Uri("srt://192.168.1.100:8888?passphrase=my-secret-passphrase&pbkeylen=32"));
```

Avec Media Blocks SDK .NET, vous pouvez vous passer de `CreateAsync` et définir la phrase secrète comme propriété. Elle ne figure alors pas dans l'URI et peut contenir n'importe quel caractère. `SRTSourceBlock` ne connaît pas les pistes à l'avance et crée donc une sortie vidéo et une sortie audio :

```csharp
var sourceSettings = new SRTSourceSettings
{
    Uri = new Uri("srt://192.168.1.100:8888"),
    Passphrase = "my secret passphrase!",
    PbKeyLen = SRTKeyLength.Length32
};
var srtSource = new SRTSourceBlock(sourceSettings);
```

Pour un flux vidéo seul, passez `renderAudio: false`.

Longueurs de clé disponibles : `SRTKeyLength.NoKey` (désactivé), `Length16` (128 bits), `Length24` (192 bits), `Length32` (256 bits).

## Configuration de la latence

La propriété `Latency` contrôle la taille du tampon récepteur SRT (par défaut : 125 ms). Des valeurs plus faibles réduisent le délai mais augmentent la sensibilité à la gigue du réseau :

```csharp
// Faible latence pour réseau local
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(50)
};

// Latence plus élevée pour réseaux peu fiables (diffusion Internet)
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(500)
};
```

| Réseau | Latence recommandée | Notes |
| --- | --- | --- |
| LAN local | 20–80 ms | Gigue minimale |
| Internet fiable | 125 ms (par défaut) | Bon équilibre |
| Peu fiable/longue distance | 250–1000 ms | Évite les pertes |

## Options d'encodage vidéo

### Encodeurs logiciels

- **OpenH264** — Encodeur H.264 multiplateforme, par défaut sur Linux et sur Windows lorsqu'aucun encodeur GPU n'est disponible

### Encodeurs accélérés matériellement

- **NVIDIA NVENC** (H.264/HEVC) — Encodage accéléré par GPU sur cartes NVIDIA
- **Intel Quick Sync** (H.264/HEVC) — Accélération sur GPU Intel intégré
- **AMD AMF** (H.264/HEVC) — Accélération sur GPU AMD
- **Microsoft Media Foundation HEVC** — Encodeur matériel Windows uniquement

### Sélection d'encodeur avec repli

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    srtOutput.Video = new NVENCH264EncoderSettings();
}
else
{
    srtOutput.Video = new OpenH264EncoderSettings();
}
```

## Encodage audio

Les flux SRT utilisent typiquement de l'audio AAC. Le SDK fournit plusieurs encodeurs :

- **VO-AAC** — Multiplateforme, performance cohérente
- **AVENC AAC** — Basé sur FFmpeg avec options étendues
- **MF AAC** — Windows uniquement, Microsoft Media Foundation

Par défaut, le SDK utilise AVENC AAC sur toutes les plateformes.

## Résolution des problèmes

### Impossible d'établir la connexion SRT

**Symptôme :** la connexion expire ou est refusée.

**Solutions :**

- Vérifiez le format de l'URL SRT : `srt://host:port` pour caller, `srt://:port` pour listener
- Assurez-vous que le port est ouvert dans les pare-feu des deux côtés
- Vérifiez que les deux côtés utilisent des modes de connexion correspondants (un caller, un listener)
- Vérifiez que les phrases secrètes correspondent si le chiffrement est activé

### Utilisation CPU élevée ou pertes d'images

**Symptôme :** les performances se dégradent durant la diffusion.

**Solutions :**

- Passez aux encodeurs accélérés matériellement (NVENC, QSV, AMF)
- Réduisez la résolution ou le débit binaire
- Augmentez la valeur `Latency` pour donner plus de marge au tampon

### Échec d'initialisation de l'encodeur

**Symptôme :** exception au démarrage du pipeline.

**Solutions :**

- Utilisez `IsAvailable()` pour vérifier la prise en charge de l'encodeur avant de le créer
- Vérifiez que les pilotes GPU sont à jour pour les encodeurs matériels
- Rabattez-vous sur OpenH264 comme encodeur logiciel universel

### L'arrêt se bloque après la déconnexion de l'autre côté

**Symptôme :** `StopAsync` ne rend pas la main après que le pair distant est devenu injoignable.

**Solutions :**

- Sous Windows, macOS, Mac Catalyst, iOS et Android, utilisez les paquets VisioForge (sous Windows, macOS et Mac Catalyst, le paquet `Adds`) : ils fournissent un plugin SRT corrigé qui relit sa propriété `auto-reconnect` avant chaque tentative de reconnexion. Le SDK désactive cette propriété à l'arrêt, de sorte que l'arrêt rend la main même si le pair a disparu
- Sous Linux, le SDK utilise le plugin SRT de la distribution, qui n'inclut pas ce correctif. Si l'arrêt s'y bloque, définissez `AutoReconnect = false` dans les paramètres : la perte du pair termine alors le flux par une erreur au lieu d'une boucle de reconnexion

## Questions fréquentes

### Quelle est la différence entre les modes caller et listener SRT ?

Le **listener** se lie à un port et attend les connexions entrantes — il agit comme serveur. Le **caller** initie la connexion vers l'adresse et le port d'un listener — il agit comme client. Pour la traversée de pare-feu lorsque les deux côtés sont derrière du NAT, utilisez le mode **rendezvous** où les deux extrémités se connectent simultanément.

### Comment chiffrer un flux SRT ?

Côté émetteur, définissez `Passphrase` (de 10 à 79 octets en UTF-8) et `PbKeyLen` sur `SRTSinkSettings`. Côté récepteur, passez les mêmes valeurs dans l'URI donnée à `SRTSourceSettings.CreateAsync` (`srt://host:8888?passphrase=...&pbkeylen=32`), car `CreateAsync` ouvre le flux avec la seule URI ; dans une URI, utilisez des lettres, des chiffres, `-`, `_`, `.` et `~`. Avec Media Blocks SDK .NET, vous pouvez à la place définir `Passphrase` et `PbKeyLen` sur un nouveau `SRTSourceSettings` et vous passer de `CreateAsync`. Les deux côtés doivent utiliser des valeurs identiques. Longueurs de clé disponibles : 128 bits (`Length16`), 192 bits (`Length24`) et 256 bits (`Length32`). Voir la section [Chiffrement](#chiffrement) pour des exemples de code.

### Comment recevoir et lire un flux SRT en C# ?

Créez `SRTSourceSettings` avec l'URL de l'émetteur, puis passez-le à `SRTSourceBlock`. Connectez `VideoOutput` à un `VideoRendererBlock` et `AudioOutput` à un `AudioRendererBlock`. Le bloc source gère automatiquement le démultiplexage MPEG-TS et le décodage. Voir la section [Réception d'un flux SRT](#reception-dun-flux-srt) pour l'exemple complet.

### Quels codecs vidéo SRT prend-il en charge ?

SRT en lui-même est agnostique vis-à-vis du codec — il transporte toute donnée sur le réseau. Avec `SRTMPEGTSSinkBlock`, le flux est multiplexé en MPEG-TS, qui prend en charge les codecs vidéo H.264, HEVC (H.265), MPEG-2 et AV1. H.264 est le choix le plus largement compatible pour la diffusion SRT.

### Comment réduire la latence de diffusion SRT ?

Abaissez la propriété `Latency` sur les paramètres de l'émetteur et du récepteur (par défaut 125 ms). Pour les réseaux locaux, des valeurs aussi basses que 20–50 ms fonctionnent bien. Pour la diffusion Internet, gardez au moins 125 ms pour gérer la gigue. Assurez-vous également que votre encodeur est configuré en mode faible latence et que vous utilisez l'accélération matérielle pour minimiser le délai d'encodage.

## Voir aussi

- [Intégration de la diffusion NDI](ndi.md) — diffusion vidéo NDI sur IP
- [Visionneuse de flux RTSP et lecteur de caméra IP](../../mediablocks/Guides/rtsp-player-csharp.md) — guide de diffusion caméra RTSP
- [Format de sortie MPEG-TS](../output-formats/mpegts.md) — configuration du conteneur MPEG-TS
- [Guide de déploiement](../../deployment-x/index.md) — paquets d'exécution spécifiques aux plateformes
- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — démo source SRT
- [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) — page produit et téléchargements
