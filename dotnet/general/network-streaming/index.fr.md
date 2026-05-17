---
title: Diffusion réseau en .NET — RTMP, RTSP, HLS, NDI, SRT
description: Implémentez les protocoles RTMP, RTSP, HLS, NDI et autres en .NET avec accélération matérielle pour la diffusion en direct et les plateformes multimédias.
sidebar_label: Diffusion réseau
order: 16
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

# Guide complet de la diffusion réseau

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction à la diffusion réseau

La diffusion réseau permet la transmission en temps réel de contenu audio et vidéo sur Internet ou sur des réseaux locaux. Les SDK complets de VisioForge fournissent des outils puissants pour implémenter divers protocoles de diffusion dans vos applications .NET, vous permettant de créer des solutions de diffusion de qualité professionnelle avec un effort de développement minimal.

Ce guide couvre toutes les options de diffusion disponibles dans les SDK VisioForge, y compris les détails d'implémentation, les bonnes pratiques et des exemples de code pour vous aider à choisir la technologie de diffusion la plus appropriée à vos besoins spécifiques.

## Vue d'ensemble des protocoles de diffusion

Les SDK VisioForge prennent en charge un large éventail de protocoles de diffusion, chacun ayant des avantages uniques selon le cas d'usage :

### Protocoles temps réel

- **[RTMP (Real-Time Messaging Protocol)](rtmp.md)** : protocole standard de l'industrie pour la diffusion en direct à faible latence, largement utilisé pour la diffusion en direct vers les CDN et les plateformes de streaming
- **[RTSP (Real-Time Streaming Protocol)](rtsp.md)** : idéal pour l'intégration de caméras IP et les applications de vidéosurveillance, offrant un contrôle précis sur les sessions multimédias
- **[SRT (Secure Reliable Transport)](srt.md)** : protocole avancé conçu pour la livraison vidéo de haute qualité et à faible latence sur des réseaux imprévisibles
- **[NDI (Network Device Interface)](ndi.md)** : protocole de qualité professionnelle pour la transmission vidéo de haute qualité et à faible latence sur les réseaux locaux

### Diffusion basée sur HTTP

- **[HLS (HTTP Live Streaming)](hls-streaming.md)** : protocole développé par Apple qui découpe les flux en segments téléchargeables, offrant une excellente compatibilité avec les navigateurs et les appareils mobiles
- **[Diffusion HTTP MJPEG](http-mjpeg.md)** : implémentation simple pour diffuser du Motion JPEG sur des connexions HTTP
- **[IIS Smooth Streaming](iis-smooth-streaming.md)** : technologie de streaming adaptatif de Microsoft pour la diffusion de médias via des serveurs IIS

### Solutions spécifiques à certaines plateformes

- **[Windows Media Streaming (WMV)](wmv.md)** : format de streaming natif de Microsoft, idéal pour les déploiements centrés sur Windows
- **[Adobe Flash Media Server](adobe-flash.md)** : solution de streaming héritée pour les applications basées sur Flash

### Intégration cloud et réseaux sociaux

- **[AWS S3](aws-s3.md)** : diffusion directe vers le stockage Amazon Web Services S3
- **[YouTube Live](youtube.md)** : intégration simplifiée avec la plateforme de diffusion en direct YouTube
- **[Facebook Live](facebook.md)** : diffusion directe vers le service de streaming Facebook

## Composants clés de la diffusion réseau

### Encodeurs vidéo

Les SDK VisioForge proposent plusieurs options d'encodage pour équilibrer qualité, performance et compatibilité :

#### Encodeurs logiciels
- **OpenH264** : encodeur H.264 logiciel multiplateforme
- **AVENC H264** : encodeur logiciel basé sur FFmpeg

#### Encodeurs accélérés matériellement
- **NVENC H264/HEVC** : encodage accéléré par GPU NVIDIA
- **QSV H264/HEVC** : accélération Intel Quick Sync Video
- **AMF H264/HEVC** : encodage accéléré par GPU AMD
- **Apple Media H264** : accélération matérielle spécifique à macOS

## Bonnes pratiques pour la diffusion réseau

### Optimisation des performances

1. **Accélération matérielle** : exploitez l'encodage par GPU lorsque disponible pour réduire l'utilisation du CPU
2. **Résolution et fréquence d'images** : adaptez la sortie au type de contenu (60 fps pour les jeux, 30 fps pour le contenu général)
3. **Allocation du débit binaire** : allouez 80 à 90 % de la bande passante à la vidéo et 10 à 20 % à l'audio

### Fiabilité du réseau

1. **Test de la connexion** : vérifiez la vitesse d'envoi avant la diffusion
2. **Gestion des erreurs** : implémentez une logique de reconnexion pour les flux interrompus
3. **Surveillance** : suivez les métriques de diffusion en temps réel pour détecter les problèmes

### Assurance qualité

1. **Vérifications avant diffusion** : validez les paramètres de l'encodeur et les paramètres de sortie
2. **Surveillance de la qualité** : vérifiez régulièrement la qualité du flux pendant la diffusion
3. **Conformité aux plateformes** : respectez les exigences spécifiques à chaque plateforme (YouTube, Facebook, etc.)

## Résolution des problèmes courants

1. **Surcharge d'encodage** : si vous observez des pertes d'images, réduisez la résolution ou le débit binaire
2. **Échecs de connexion** : vérifiez la stabilité du réseau et les adresses des serveurs
3. **Synchronisation audio/vidéo** : assurez une synchronisation correcte des horodatages entre les flux
4. **Rejet par la plateforme** : confirmez la conformité avec les exigences spécifiques à la plateforme
5. **Échecs de l'accélération matérielle** : vérifiez l'installation et la compatibilité des pilotes

## Conclusion

La diffusion réseau avec les SDK VisioForge fournit une solution complète pour implémenter une diffusion multimédia de qualité professionnelle dans vos applications .NET. En comprenant les protocoles disponibles et en suivant les bonnes pratiques, vous pouvez créer des expériences de diffusion de haute qualité pour vos utilisateurs sur de multiples plateformes.

Pour les détails d'implémentation propres à chaque protocole, consultez les guides dédiés référencés tout au long de ce document.
