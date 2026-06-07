---
title: Diffusion vidéo UDP avec conteneur MPEG-TS en C# .NET
description: Diffusez la vidéo H.264/HEVC sur UDP en C# .NET — multicast, point-à-point, basse latence. Exemples envoi/réception avec réglage du débit.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - UDP
  - MP4
  - TS
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - FFMPEGEXEOutput
  - BasicVideoSettings
  - MediaBlockPadMediaType
  - MediaBlocksPipeline
  - UDPMPEGTSSinkBlock
  - UDPSinkSettings
  - MultiUDPMPEGTSSinkBlock

---

# Diffusion UDP avec les SDK VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Agents de code IA : utilisez le serveur MCP VisioForge"

    Vous développez avec **Claude Code**, **Cursor** ou un autre agent de code IA ?
    Connectez-vous au [serveur MCP VisioForge](../mcp-server-usage.md) public
    à `https://mcp.visioforge.com/mcp` pour des recherches API structurées, des
    exemples de code exécutables et des guides de déploiement — plus précis que de
    parcourir `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction à la diffusion UDP

Le protocole User Datagram Protocol (UDP) est un protocole de transport léger et sans connexion qui fournit une interface simple entre les applications réseau et le réseau IP sous-jacent. Contrairement à TCP, UDP offre une surcharge minimale et ne garantit pas la livraison des paquets, ce qui le rend idéal pour les applications en temps réel où la vitesse est cruciale et où des pertes de paquets occasionnelles sont acceptables.

Les SDK VisioForge offrent une prise en charge robuste de la diffusion UDP, permettant aux développeurs de mettre en œuvre des solutions de diffusion haute performance à faible latence pour diverses applications, notamment les diffusions en direct, la vidéosurveillance et les systèmes de communication en temps réel.

## Principales caractéristiques et capacités

La suite SDK VisioForge fournit une fonctionnalité complète de diffusion UDP avec les caractéristiques clés suivantes :

### Prise en charge des codecs vidéo et audio

- **Codecs vidéo** : prise en charge complète de H.264 (AVC) et H.265 (HEVC), offrant une excellente efficacité de compression tout en conservant une qualité vidéo élevée.
- **Codec audio** : prise en charge d'Advanced Audio Coding (AAC), offrant une qualité audio supérieure à des débits binaires plus faibles que les anciens codecs audio.

### MPEG Transport Stream (MPEG-TS)

Le SDK utilise MPEG-TS comme format conteneur pour la diffusion UDP. MPEG-TS offre plusieurs avantages :

- Conçu spécifiquement pour la transmission sur des réseaux potentiellement peu fiables
- Capacités de correction d'erreur intégrées
- Prise en charge du multiplexage de plusieurs flux audio et vidéo
- Caractéristiques de faible latence idéales pour la diffusion en direct

### Intégration FFMPEG

Les SDK VisioForge tirent parti de la puissance de FFMPEG pour la diffusion UDP, garantissant :

- Encodage et diffusion haute performance
- Large compatibilité avec divers réseaux et clients récepteurs
- Gestion fiable des paquets et du flux

### Prise en charge unicast et multicast

- **Unicast** : transmission point-à-point d'un seul émetteur vers un seul récepteur
- **Multicast** : distribution efficace du même contenu à plusieurs destinataires simultanément sans dupliquer la bande passante à la source

## Détails techniques d'implémentation

La diffusion UDP dans les SDK VisioForge implique plusieurs composants techniques clés :

1. **Encodage vidéo** : la vidéo source est compressée à l'aide d'encodeurs H.264 ou HEVC avec des paramètres configurables pour le débit binaire, la résolution et la fréquence d'images.

2. **Encodage audio** : les flux audio sont traités via des encodeurs AAC avec des réglages de qualité ajustables.

3. **Multiplexage** : les flux vidéo et audio sont combinés dans un seul conteneur MPEG-TS.

4. **Paquetisation** : le flux MPEG-TS est divisé en paquets UDP de taille appropriée pour la transmission réseau.

5. **Transmission** : les paquets sont envoyés sur le réseau vers des adresses unicast ou multicast spécifiées.

L'implémentation privilégie la faible latence tout en maintenant une qualité suffisante pour les applications professionnelles. Des mécanismes de mise en tampon avancés aident à gérer la gigue du réseau et garantissent une lecture fluide côté récepteur.

## Implémentation de la sortie UDP spécifique à Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

### Étape 1 : activer la diffusion réseau

La première étape consiste à activer la fonctionnalité de diffusion réseau dans votre application. Pour ce faire, définissez la propriété `Network_Streaming_Enabled` sur true :

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

### Étape 2 : configurer la diffusion audio (facultatif)

Si votre application nécessite la diffusion audio en plus de la vidéo, activez-la avec :

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

### Étape 3 : définir le format de diffusion

Spécifiez UDP comme format de diffusion en définissant la propriété `Network_Streaming_Format` sur `UDP_FFMPEG_EXE` :

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.UDP_FFMPEG_EXE;
```

### Étape 4 : configurer l'URL du flux UDP

Définissez l'URL de destination pour votre flux UDP. Pour un flux unicast de base vers localhost :

```cs
VideoCapture1.Network_Streaming_URL = "udp://127.0.0.1:10000?pkt_size=1316";
```

Le paramètre `pkt_size` définit la taille des paquets UDP. La valeur 1316 est optimisée pour la plupart des environnements réseau, permettant une transmission efficace tout en minimisant la fragmentation.

### Étape 5 : configuration multicast (facultatif)

Pour la diffusion multicast vers plusieurs récepteurs, utilisez une adresse multicast (typiquement dans la plage 224.0.0.0 à 239.255.255.255) :

```cs
VideoCapture1.Network_Streaming_URL = "udp://239.101.101.1:1234?ttl=1&pkt_size=1316";
```

Les paramètres supplémentaires incluent :
- **ttl** : valeur Time-to-live qui détermine combien de sauts réseau les paquets peuvent traverser
- **pkt_size** : taille de paquet comme expliqué ci-dessus

### Étape 6 : configurer les paramètres de sortie

Enfin, configurez les paramètres de sortie de diffusion à l'aide de la classe `FFMPEGEXEOutput` :

```cs
var ffmpegOutput = new FFMPEGEXEOutput();

ffmpegOutput.FillDefaults(DefaultsProfile.MP4_H264_AAC, true);
ffmpegOutput.OutputMuxer = OutputMuxer.MPEGTS;

VideoCapture1.Network_Streaming_Output = ffmpegOutput;
```

Ce code :
1. Crée une nouvelle configuration de sortie FFMPEG
2. Applique les paramètres par défaut pour vidéo H.264 et audio AAC
3. Spécifie MPEG-TS comme format conteneur
4. Assigne cette configuration à la sortie de diffusion

## Sortie UDP multiplateforme avec Media Blocks

[MediaBlocksPipeline](#){ .md-button }

Le Media Blocks SDK prend en charge la diffusion UDP multiplateforme grâce à des blocs basés sur GStreamer. Ces blocs fonctionnent sur Windows, macOS, Linux, iOS et Android.

### Diffusion MPEG-TS vers une seule destination

Utilisez `UDPMPEGTSSinkBlock` pour multiplexer audio et vidéo en MPEG-TS et envoyer via UDP vers une seule destination :

```csharp
var pipeline = new MediaBlocksPipeline();

var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync("input.mp4"));

var videoEncoder = new H264EncoderBlock(new OpenH264EncoderSettings());
var audioEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings() { Bitrate = 192 });

pipeline.Connect(fileSource.VideoOutput, videoEncoder.Input);
pipeline.Connect(fileSource.AudioOutput, audioEncoder.Input);

var udpSettings = new UDPSinkSettings
{
    Host = "192.168.1.100",
    Port = 5004,
    TTL = 64
};

var udpSink = new UDPMPEGTSSinkBlock(udpSettings);
pipeline.Connect(videoEncoder.Output, udpSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, udpSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

### Diffusion MPEG-TS vers plusieurs destinations

Utilisez `MultiUDPMPEGTSSinkBlock` pour envoyer le même flux MPEG-TS à plusieurs récepteurs simultanément :

```csharp
var multiUdpSettings = new MultiUDPSinkSettings();
multiUdpSettings.AddClient("192.168.1.100", 5004);
multiUdpSettings.AddClient("192.168.1.101", 5004);

var multiUdpSink = new MultiUDPMPEGTSSinkBlock(multiUdpSettings);
pipeline.Connect(videoEncoder.Output, multiUdpSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, multiUdpSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

### Diffusion multicast

Pour la livraison multicast, définissez `Host` sur une adresse multicast (224.0.0.0 – 239.255.255.255) :

```csharp
var udpSettings = new UDPSinkSettings
{
    Host = "239.101.101.1",
    Port = 5004,
    MulticastTTL = 4,
    AutoMulticast = true
};
```

### Réception de flux UDP

Vous pouvez vérifier le flux à l'aide des outils en ligne de commande GStreamer :

```bash
gst-launch-1.0 udpsrc port=5004 ! tsdemux ! decodebin ! autovideosink
```

Ou recevoir avec VLC :

```
vlc udp://@:5004
```

## Options de configuration avancées

### Gestion du débit binaire

Pour des performances de diffusion optimales, ajustez les débits binaires vidéo et audio à la capacité de votre
réseau. `FFMPEGEXEOutput` expose les réglages d'encodeur via `.Video` et `.Audio`
(et non `VideoSettings`/`AudioSettings`), et les sous-jacents
`BasicVideoSettings` / `BasicAudioSettings` stockent le débit binaire en **kbps** :

```cs
ffmpegOutput.Video.Bitrate = 2500; // 2,5 Mbps pour la vidéo (kbps)
ffmpegOutput.Audio.Bitrate = 128;  // 128 kbps pour l'audio
```

### Résolution et fréquence d'images

Des résolutions plus faibles réduisent la bande passante. Définissez la taille cible dans
`VideoCapture1.Video_Resize` (le moteur classique l'expose comme objet
`IVideoResizeSettings`, et non comme propriétés à plat sur le core), et activez
l'étape de redimensionnement avec `Video_ResizeOrCrop_Enabled` :

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width  = 1280,   // résolution 720p
    Height = 720,
    Mode   = VideoResizeMode.Letterbox,
};

// La fréquence d'images se configure sur le format du périphérique de capture, pas sur le core — choisissez
// un format de périphérique à 30 fps via Video_CaptureDevice_Format / _FrameRate.
```

### Configuration de la taille du tampon

Le compromis latence/stabilité pour la diffusion basée sur FFMPEG se contrôle sur l'objet
de sortie, pas sur le core. En millisecondes :

```cs
ffmpegOutput.VideoBufferSize = 5000; // tampon de 5 s pour une diffusion plus fluide
```

## Bonnes pratiques pour la diffusion UDP

### Considérations réseau

1. **Évaluation de la bande passante** : assurez une bande passante suffisante pour la qualité cible. À titre indicatif :
   - Qualité SD (480p) : 1-2 Mbps
   - Qualité HD (720p) : 2,5-4 Mbps
   - Full HD (1080p) : 4-8 Mbps

2. **Stabilité du réseau** : UDP ne garantit pas la livraison des paquets. Sur les réseaux instables, envisagez :
   - De réduire la résolution ou le débit binaire
   - D'implémenter une récupération d'erreur au niveau applicatif
   - D'utiliser la correction d'erreur en amont quand elle est disponible

3. **Configuration du pare-feu** : assurez-vous que les ports UDP sont ouverts sur les pare-feu de l'émetteur et du récepteur.

### Optimisation des performances

1. **Accélération matérielle / images-clés / preset** : `FFMPEGEXEOutput` n'expose pas
   de propriétés de premier ordre pour l'accélération matérielle, l'intervalle d'image-clé ou les presets x264 — injectez-les
   plutôt comme drapeaux CLI FFMPEG via `Custom_AdditionalVideoArgs`. FFMPEG les applique alors
   à l'invocation de l'encodeur vidéo.

```cs
// Encodeur matériel NVENC + intervalle d'image-clé de 2 secondes (60 images @ 30 fps)
// + preset ultrafast (latence la plus faible).
ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_nvenc -g 60 -preset p1";

// Intel QuickSync à la place :
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_qsv -g 60";

// x264 logiciel avec un compromis qualité/vitesse :
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v libx264 -g 60 -preset ultrafast";
```

2. **Transport basé sur pipe** (évite un fichier temporaire entre SDK et FFMPEG)
   réduit généralement la latence :

```cs
ffmpegOutput.UsePipe = true;
```

## Résolution des problèmes courants

1. **Flux non reçu** : vérifiez la connectivité réseau, la disponibilité du port et les paramètres du pare-feu.

2. **Latence élevée** : vérifiez la congestion du réseau, réduisez le débit binaire ou ajustez la taille du tampon.

3. **Qualité médiocre** : augmentez le débit binaire, ajustez les paramètres d'encodage ou vérifiez la perte de paquets réseau.

4. **Problèmes de synchronisation audio/vidéo** : assurez la synchronisation correcte des horodatages dans votre application.

## Conclusion

La diffusion UDP avec les SDK VisioForge fournit une solution puissante pour la transmission vidéo et audio en temps réel avec une latence minimale. En tirant parti des codecs vidéo H.264/HEVC, de l'audio AAC et du conditionnement MPEG-TS, les développeurs peuvent créer des applications de diffusion robustes adaptées à un large éventail de cas d'usage.

La flexibilité du SDK permet le réglage fin de tous les paramètres de diffusion, permettant l'optimisation pour des conditions réseau et des exigences de qualité spécifiques. Qu'il s'agisse d'implémenter un simple flux point-à-point ou un système complexe de distribution multicast, les capacités de diffusion UDP de VisioForge fournissent les outils nécessaires au succès.

---
Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code et de démonstrations fonctionnelles d'implémentations de diffusion UDP.
