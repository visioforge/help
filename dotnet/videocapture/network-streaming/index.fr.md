---
title: Streaming réseau en C# .NET — RTSP, RTMP, NDI, SRT
description: Diffusez de la vidéo en direct via RTSP, RTMP, NDI, HLS et SRT depuis VisioForge Video Capture. Protocoles, encodeurs et exemples C#.
sidebar_label: Streaming réseau
order: 5
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming

---

# Streaming réseau dans les applications .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction au streaming réseau

Le streaming réseau est devenu un composant fondamental de l'infrastructure moderne de communication numérique, permettant la transmission en temps réel de données audio et vidéo sur des environnements réseau variés. Avec l'expansion continue des capacités de bande passante et l'évolution des attentes des utilisateurs en matière de diffusion de contenu, les développeurs ont besoin d'outils robustes pour implémenter des solutions de streaming qui équilibrent performance, qualité et fiabilité.

Le VisioForge Video Capture SDK pour .NET offre une prise en charge complète de plusieurs protocoles de streaming, offrant aux développeurs un kit d'outils polyvalent pour intégrer des capacités de streaming sophistiquées dans leurs applications. Que vous construisiez des plateformes de diffusion en direct, des outils de visioconférence, des systèmes de surveillance ou des réseaux de diffusion de contenu, ce SDK fournit la base pour implémenter des solutions de streaming professionnelles.

## Démarrage rapide — streaming RTMP avec VideoCaptureCoreX

Le chemin le plus court de bout en bout : choisissez un périphérique de capture, ajoutez un bloc de sortie RTMP au moteur, démarrez. Remplacez `RTMPOutput` par `YouTubeOutput`, `FacebookLiveOutput`, `RTSPServerOutput` ou `HLSOutput` pour cibler d'autres destinations — le câblage autour reste identique.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// 1. Initialiser le moteur multiplateforme une fois par processus.
VisioForgeX.InitSDK();

// 2. Créer le moteur, lié à un IVideoView (ou null pour sans interface).
var videoCapture = new VideoCaptureCoreX(videoView as IVideoView);

// 3. Sélectionner le premier périphérique de capture énuméré + micro.
var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync()).First();
var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync()).First();

videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevice);
videoCapture.Audio_Source = new AudioCaptureDeviceSourceSettings(audioDevice);
videoCapture.Audio_Record = true;

// 4. Ajouter une sortie RTMP. La chaîne est l'URL d'ingestion complète, incluant la clé de flux.
var rtmpOutput = new RTMPOutput("rtmp://live.example.com/app/<stream-key>");
videoCapture.Outputs_Add(rtmpOutput, true);

// 5. Démarrer. OnError se déclenche en cas d'échec réseau / encodeur.
await videoCapture.StartAsync();
```

Pour la configuration par protocole (réglage de l'encodeur, tailles de tampon, authentification, débits adaptatifs, etc.), consultez les pages dédiées sous [Protocoles disponibles](#available-protocols) ci-dessous.

## Protocoles principaux de streaming réseau

### RTSP (Real-Time Streaming Protocol)

RTSP est un protocole de niveau application conçu pour contrôler la diffusion de données possédant des propriétés temps réel. Il sert de « télécommande réseau » pour les serveurs multimédias, établissant et contrôlant les sessions multimédias entre points d'extrémité.

#### Fonctionnalités clés de RTSP :

- **Contrôle de session** : permet aux clients d'établir et de gérer des sessions multimédias avec les serveurs
- **Indépendance du transport** : fonctionne avec divers protocoles de transport notamment UDP, TCP et UDP multicast
- **Prise en charge des commandes** : implémente des commandes comme PLAY, PAUSE et RECORD pour un contrôle granulaire de la session
- **Évolutivité** : prend en charge la diffusion unicast et multicast pour une utilisation efficace de la bande passante

### RTMP (Real-Time Messaging Protocol)

Développé à l'origine par Macromedia pour diffuser du contenu Flash, RTMP a évolué pour devenir l'un des protocoles les plus largement utilisés pour la diffusion en direct. Il maintient des connexions persistantes entre le client et le serveur, facilitant une transmission à faible latence cruciale pour les applications interactives.

#### Fonctionnalités clés de RTMP :

- **Faible latence** : offre généralement une latence inférieure à la seconde, le rendant adapté au streaming interactif
- **Distribution fiable** : utilise TCP comme couche de transport pour une distribution fiable des paquets
- **Protection du contenu** : prend en charge le chiffrement pour une diffusion sécurisée du contenu
- **Large prise en charge** : compatible avec de nombreux CDN et plateformes de streaming

### NDI (Network Device Interface)

NDI représente une avancée significative dans les flux de travail de production vidéo professionnels, permettant une transmission vidéo haute qualité et faible latence sur des réseaux IP standards. Développé par NewTek, NDI a été largement adopté dans les environnements de diffusion et de production.

#### Fonctionnalités clés de NDI :

- **Flux de travail basé sur IP** : tire parti de l'infrastructure réseau existante sans matériel spécialisé
- **Communication bidirectionnelle** : prend en charge les métadonnées et les données de contrôle aux côtés de l'audio/vidéo
- **Mécanisme de découverte** : découverte automatique des périphériques et des sources sur les réseaux locaux
- **Encodage haute qualité** : maintient la qualité visuelle tout en optimisant pour les conditions du réseau

### HLS (HTTP Live Streaming)

Développé par Apple, HLS est devenu l'un des protocoles de streaming les plus largement pris en charge. Il segmente le contenu en petits téléchargements de fichiers HTTP, permettant un streaming à débit adaptatif qui ajuste la qualité en fonction de la bande passante disponible du spectateur.

#### Fonctionnalités clés de HLS :

- **Débit adaptatif** : ajuste dynamiquement la qualité du flux selon les conditions du réseau
- **Large compatibilité** : pris en charge sur la plupart des navigateurs et appareils modernes
- **Distribution HTTP** : utilise des serveurs web standard et des CDN pour une distribution efficace du contenu
- **Protection du contenu** : prend en charge le chiffrement et l'intégration DRM

### SRT (Secure Reliable Transport)

SRT est un protocole open source optimisé pour offrir une vidéo haute qualité et faible latence sur des réseaux imprévisibles. Il combine la fiabilité de TCP avec la vitesse d'UDP tout en ajoutant des fonctionnalités de sécurité.

#### Fonctionnalités clés de SRT :

- **Récupération de perte de paquets** : implémente des mécanismes de retransmission dynamique
- **Chiffrement** : chiffrement AES intégré pour une transmission sécurisée
- **Surveillance de la santé du réseau** : évalue en continu la qualité de la connexion
- **Contrôle de la latence** : paramètres de latence configurables pour équilibrer fiabilité et délai

## Protocoles supplémentaires pris en charge

### HTTP MJPEG (Motion JPEG)

MJPEG sur HTTP transmet une séquence d'images JPEG, fournissant une solution de streaming simple mais efficace. Bien que moins efficace en bande passante que les codecs modernes, sa simplicité et sa compatibilité le rendent adapté à certaines applications.

### UDP (User Datagram Protocol)

Le streaming UDP privilégie la vitesse à la fiabilité, le rendant idéal pour les applications temps réel où une perte occasionnelle de paquets est préférable à une latence accrue. Le SDK fournit des paramètres de tampon configurables pour aider à optimiser le streaming UDP selon les conditions réseau spécifiques.

### WMV (Windows Media Video)

Le SDK prend en charge le streaming au format WMV de Microsoft, qui reste pertinent pour certaines applications centrées sur Windows et l'intégration de systèmes hérités.

### Streaming spécifique à la plateforme

Le SDK s'intègre également avec des plateformes de streaming populaires, notamment :

- **YouTube Live** : streaming direct vers les chaînes YouTube
- **Facebook Live** : diffusion Facebook Live intégrée
- **AWS S3** : distribution multimédia basée sur le cloud via Amazon Web Services

## Considérations d'implémentation

Lors de l'implémentation du streaming réseau avec le SDK, les développeurs doivent considérer :

1. **Besoins en bande passante** : estimez et testez la bande passante requise pour votre qualité et fréquence d'images cibles
2. **Résilience réseau** : implémentez une gestion d'erreurs et une logique de reconnexion appropriées
3. **Qualité vs latence** : équilibrez la qualité visuelle face aux exigences de latence
4. **Compatibilité multiplateforme** : sélectionnez les protocoles selon vos plateformes cibles
5. **Besoins de sécurité** : implémentez le chiffrement et l'authentification où nécessaire

## Conclusion

Le VisioForge Video Capture SDK pour .NET offre une prise en charge complète des protocoles de streaming contemporains, permettant aux développeurs d'implémenter des solutions de streaming sophistiquées sans gérer directement la complexité des implémentations de protocoles. Des diffusions en direct à faible latence à la livraison sécurisée de contenu, les capacités de streaming du SDK répondent à des cas d'usage variés dans toutes les industries.

En tirant parti de ces capacités, les développeurs peuvent se concentrer sur la création d'expériences convaincantes tout en s'appuyant sur le SDK pour gérer les défis techniques d'un streaming réseau fiable. Que vous développiez des applications pour le divertissement, l'éducation, la sécurité ou la production vidéo professionnelle, le SDK fournit une base solide pour vos besoins de streaming.

## Protocoles disponibles { #available-protocols }

* [Adobe Flash Server](../../general/network-streaming/adobe-flash.md)
* [AWS S3](../../general/network-streaming/aws-s3.md)
* [Facebook](../../general/network-streaming/facebook.md)
* [HLS](../../general/network-streaming/hls-streaming.md)
* [HTTP MJPEG](../../general/network-streaming/http-mjpeg.md)
* [NDI](../../general/network-streaming/ndi.md)
* [RTMP](../../general/network-streaming/rtmp.md)
* [RTSP](../../general/network-streaming/rtsp.md)
* [SRT](../../general/network-streaming/srt.md)
* [UDP](../../general/network-streaming/udp.md)
* [WMV](../../general/network-streaming/wmv.md)
* [YouTube](../../general/network-streaming/youtube.md)

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
