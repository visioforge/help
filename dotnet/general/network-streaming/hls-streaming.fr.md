---
title: Diffusion HLS en direct en C# .NET — m3u8 et segments
description: Générez des playlists .m3u8 et des segments .ts avec serveur HTTP intégré. Intégration HLS.js, AVPlayer iOS et ExoPlayer Android. Exemples du SDK VisioForge.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - HLS
  - TS
  - H.264
  - AAC
  - C#
primary_api_classes:
  - HLSOutput
  - HLSSinkSettings
  - MediaBlockPadMediaType
  - HLSPlaylistType
  - H264EncoderBlock

---

# Guide complet d'implémentation de la diffusion réseau HLS en .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Agents de code IA : utilisez le serveur MCP VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de code IA ?
    Connectez-vous au [serveur MCP VisioForge](../mcp-server-usage.md) public
    à `https://mcp.visioforge.com/mcp` pour des recherches API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis que de
    parcourir `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Qu'est-ce que HTTP Live Streaming (HLS) ?

HTTP Live Streaming (HLS) est un protocole de communication de streaming à débit adaptatif conçu et développé par Apple Inc. Présenté pour la première fois en 2009, il est depuis devenu l'un des protocoles de streaming les plus largement adoptés sur diverses plateformes et appareils. HLS fonctionne en découpant le flux global en une séquence de petits téléchargements de fichiers basés sur HTTP, chacun contenant un court segment du contenu global.

### Principales caractéristiques de la diffusion HLS

- **Diffusion à débit adaptatif** : HLS ajuste automatiquement la qualité vidéo en fonction des conditions réseau du spectateur, garantissant une qualité de lecture optimale sans mise en mémoire tampon.
- **Compatibilité multiplateforme** : fonctionne sur iOS, macOS, Android, Windows et la plupart des navigateurs web modernes.
- **Livraison basée sur HTTP** : exploite l'infrastructure standard des serveurs web, permettant au contenu de traverser les pare-feu et serveurs proxy.
- **Chiffrement et authentification du média** : prend en charge la protection du contenu via le chiffrement et diverses méthodes d'authentification.
- **Contenu en direct et à la demande** : utilisable à la fois pour la diffusion en direct et les médias pré-enregistrés.

### Structure technique HLS

La livraison de contenu HLS repose sur trois composants clés :

1. **Fichier de manifeste (.m3u8)** : fichier de playlist contenant les métadonnées sur les différents flux disponibles
2. **Fichiers de segment (.ts)** : le contenu multimédia proprement dit, divisé en petits morceaux (typiquement 2 à 10 secondes chacun)
3. **Serveur HTTP** : responsable de la livraison du manifeste et des fichiers de segment

HLS étant entièrement basé sur HTTP, vous aurez besoin soit d'un serveur HTTP dédié, soit du serveur interne léger fourni par nos SDK.

## Implémentation de la diffusion HLS avec le Media Blocks SDK

Le Media Blocks SDK propose une approche complète de la diffusion HLS grâce à son architecture de pipeline, offrant aux développeurs un contrôle granulaire sur chaque aspect du processus de diffusion.

### Création d'un flux HLS de base

L'exemple suivant montre comment configurer un flux HLS avec le Media Blocks SDK :

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Initialiser une fois par processus le moteur multiplateforme.
VisioForgeX.InitSDK();

const string URL = "http://localhost:8088/";

// Pipeline (attacher le gestionnaire d'erreurs avant d'ajouter les blocs).
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");

// Source vidéo : premier périphérique de capture énuméré.
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Source audio : premier périphérique de capture audio énuméré.
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Encodeurs H.264 et AAC.
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
var aacEncoder = new AACEncoderBlock();

// Puits HLS.
var settings = new HLSSinkSettings
{
    Location             = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles             = 10,
    PlaylistLength       = 5,
    PlaylistLocation     = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot         = URL.TrimEnd('/'),
    SendKeyframeRequests = true,
    TargetDuration       = TimeSpan.FromSeconds(5),  // TimeSpan, pas int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port    = 8088
};

var hlsSink = new HLSSinkBlock(settings);

// Câblage du graphe : source → encodeur → puits HLS (pads séparés pour vidéo/audio).
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(h264Encoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(aacEncoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Démarrer. Ouvrir http://localhost:8088/playlist.m3u8 dans un lecteur ou un navigateur.
await pipeline.StartAsync();
```

### Options de configuration avancées

Le Media Blocks SDK propose plusieurs options de configuration avancées pour la diffusion HLS :

- **Multiples variantes de débit binaire** : créez différents niveaux de qualité pour le streaming adaptatif
- **Durée de segment personnalisée** : optimisez selon les types de contenu et environnements de visionnage
- **Options côté serveur** : configurez les en-têtes de cache et autres comportements du serveur
- **Fonctionnalités de sécurité** : implémentez l'authentification par jeton ou le chiffrement

Vous pouvez utiliser ce SDK pour diffuser à la fois la capture vidéo en direct et des fichiers multimédias existants en HLS. L'architecture de pipeline flexible permet une personnalisation poussée du workflow de traitement multimédia.

## Diffusion HLS avec Video Capture SDK .NET

Video Capture SDK .NET propose une approche simplifiée de la diffusion HLS spécialement conçue pour les sources vidéo en direct comme les webcams, les cartes de capture et autres périphériques d'entrée.

### Implémentation avec VideoCaptureCoreX

Le moteur VideoCaptureCoreX propose une approche moderne et orientée objet de la capture vidéo et de la diffusion :

```csharp
// Créer les paramètres du puits HLS
var settings = new HLSSinkSettings
{
    Location = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles = 10,
    PlaylistLength = 5,
    PlaylistLocation = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot = edStreamingKey.Text,
    SendKeyframeRequests = true,
    TargetDuration = TimeSpan.FromSeconds(5), // TimeSpan, pas int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port = new Uri(edStreamingKey.Text).Port
};

// Créer la sortie HLS
var hlsOutput = new HLSOutput(settings);

// Créer les encodeurs vidéo et audio avec les paramètres par défaut
hlsOutput.Video = new OpenH264EncoderSettings();
hlsOutput.Audio = new VOAACEncoderSettings();

// Ajouter la sortie HLS à l'objet de capture vidéo
videoCapture.Outputs_Add(hlsOutput, true);
```

### Implémentation avec VideoCaptureCore

Pour les développeurs travaillant avec le moteur classique VideoCaptureCore, l'implémentation est légèrement différente mais tout aussi simple :

```csharp
VideoCapture1.Network_Streaming_Enabled = true;
VideoCapture1.Network_Streaming_Audio_Enabled = true;
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Durée des segments en secondes
        NumSegments = 5,                        // Nombre de segments dans la playlist
        OutputFolder = "c:\\hls\\",             // Dossier de sortie
        PlaylistType = HLSPlaylistType.Live,    // Type de playlist
        Custom_HTTP_Server_Enabled = true,      // Utiliser le serveur HTTP interne
        Custom_HTTP_Server_Port = 8088          // Port du serveur HTTP interne
    }
};

VideoCapture1.Network_Streaming_Output = hls;
```

### Considérations de performance

Lors de la diffusion avec Video Capture SDK, tenez compte de ces techniques d'optimisation des performances :

- Maintenez la durée des segments entre 2 et 10 secondes (10 secondes est optimal pour la plupart des cas d'usage)
- Ajustez le nombre de segments en fonction des habitudes de visionnage attendues
- Utilisez l'accélération matérielle lorsqu'elle est disponible pour l'encodage
- Configurez des débits binaires appropriés en fonction des vitesses de connexion de votre public cible

## Conversion de fichiers multimédias en HLS avec Video Edit SDK .NET

Le Video Edit SDK .NET permet aux développeurs de convertir des fichiers multimédias existants au format HLS pour le streaming, idéal pour les applications de vidéo à la demande.

### Implémentation avec VideoEditCore

```csharp
VideoEdit1.Network_Streaming_Enabled = true;
VideoEdit1.Network_Streaming_Audio_Enabled = true;
VideoEdit1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Durée des segments en secondes
        NumSegments = 5,                        // Nombre de segments dans la playlist
        OutputFolder = "c:\\hls\\",             // Dossier de sortie
        PlaylistType = HLSPlaylistType.Live,    // Type de playlist
        Custom_HTTP_Server_Enabled = true,      // Utiliser le serveur HTTP interne
        Custom_HTTP_Server_Port = 8088          // Port du serveur HTTP interne
    }
};

VideoEdit1.Network_Streaming_Output = hls;
```

### Considérations sur les formats de fichier

Lors de la conversion de fichiers en HLS, prenez en compte ces facteurs :

- Tous les formats d'entrée ne sont pas également efficaces pour la conversion
- Les fichiers MP4, MOV et MKV donnent généralement les meilleurs résultats
- Les formats fortement compressés peuvent demander davantage de puissance de traitement
- Envisagez de pré-transcoder les fichiers très volumineux vers un format intermédiaire

## Lecture et intégration

### Intégration du lecteur HTML5

Toutes les applications mettant en œuvre la diffusion HLS devraient inclure un fichier HTML avec un lecteur vidéo. Les lecteurs HTML5 modernes comme HLS.js, Video.js ou JW Player offrent une excellente prise en charge des flux HLS.

Voici un exemple de base utilisant HLS.js :

```html
<!DOCTYPE html>
<html>
<head>
    <title>HLS Player</title>
    <script src="https://cdn.jsdelivr.net/npm/hls.js@latest"></script>
</head>
<body>
    <video id="video" controls></video>
    <script>
      var video = document.getElementById('video');
      var videoSrc = 'http://localhost:8088/playlist.m3u8';
      
      if (Hls.isSupported()) {
        var hls = new Hls();
        hls.loadSource(videoSrc);
        hls.attachMedia(video);
      } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = videoSrc;
      }
    </script>
</body>
</html>
```

Pour un exemple complet de lecteur, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples/blob/master/Media%20Blocks%20SDK/Console/HLS%20Streamer/index.htm).

### Intégration aux applications mobiles

Nos SDK prennent également en charge l'intégration aux applications mobiles via :

- Lecture iOS native via AVPlayer
- Lecture Android via ExoPlayer
- Options multiplateformes comme Xamarin ou MAUI

## Résolution des problèmes courants

### Configuration CORS

Lors de la diffusion de contenu HLS vers des navigateurs web, vous pouvez rencontrer des problèmes CORS (Cross-Origin Resource Sharing). Assurez-vous que votre serveur est configuré pour envoyer les en-têtes CORS appropriés :

```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, HEAD, OPTIONS
Access-Control-Allow-Headers: Range
Access-Control-Expose-Headers: Accept-Ranges, Content-Encoding, Content-Length, Content-Range
```

### Optimisation de la latence

HLS introduit intrinsèquement une certaine latence. Pour la minimiser :

- Utilisez des durées de segment plus courtes (2 à 4 secondes) pour une latence plus faible
- Envisagez d'activer Low-Latency HLS (LL-HLS) s'il est pris en charge
- Optimisez votre infrastructure réseau pour minimiser les délais

## Conclusion

La diffusion HLS fournit une solution robuste et multiplateforme pour livrer du contenu vidéo en direct comme à la demande à un large éventail d'appareils. Avec les SDK .NET de VisioForge, l'implémentation de HLS dans vos applications devient simple, vous permettant de vous concentrer sur la création de contenu attractif plutôt que sur les détails techniques.

Pour davantage d'exemples de code et d'implémentations avancées, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

---
## Ressources supplémentaires
- [Spécification HLS](https://developer.apple.com/streaming/)
