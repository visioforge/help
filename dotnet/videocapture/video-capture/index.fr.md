---
title: Video Capture SDK .Net pour l'enregistrement avancé
description: Video Capture SDK .NET puissant — formats étendus, intégration matérielle et déploiement flexible pour les applications d'enregistrement.
sidebar_label: Capture vidéo
order: 11
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - Streaming

---

# Video Capture SDK pour les développeurs .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Notre Video Capture SDK pour .NET fournit aux développeurs une solution puissante et polyvalente pour implémenter des capacités d'enregistrement vidéo de qualité professionnelle dans leurs applications. Conçu spécifiquement pour les environnements .NET, ce SDK offre une intégration fluide avec votre base de code existante tout en assurant des performances et une fiabilité exceptionnelles.

## Démarrage rapide — Enregistrer une webcam vers MP4

Chemin minimal de bout en bout avec `VideoCaptureCoreX` (multiplateforme). Remplacez `MP4Output` par `MKVOutput` / `WebMOutput` / `MOVOutput` pour changer de conteneur, ou par `RTSPServerOutput` / `RTMPOutput` / `HLSOutput` pour diffuser au lieu d'enregistrer — le câblage environnant reste identique.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// 1. Initialiser le moteur multiplateforme une seule fois par processus.
VisioForgeX.InitSDK();

// 2. Créer le moteur, lié à un IVideoView pour l'aperçu (ou null pour un mode sans interface).
var videoCapture = new VideoCaptureCoreX(videoView as IVideoView);

// 3. Sélectionner la première caméra et le premier microphone énumérés.
var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync()).First();
var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync()).First();
videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevice);
videoCapture.Audio_Source = new AudioCaptureDeviceSourceSettings(audioDevice);
videoCapture.Audio_Record = true;

// 4. Ajouter une sortie MP4. Par défaut : H.264 + AAC.
videoCapture.Outputs_Add(new MP4Output("output.mp4"), true);

// 5. Démarrer. OnError se déclenche en cas d'erreurs du pipeline.
await videoCapture.StartAsync();
```

## Capacités clés

- **Capture multisource** — Enregistrez depuis des webcams, des cartes de capture et d'autres périphériques vidéo
- **Traitement en temps réel** — Appliquez des filtres et des effets pendant la capture
- **Paramètres de qualité personnalisables** — Contrôlez le débit binaire, la fréquence d'images et la résolution
- **Architecture événementielle** — Réagissez aux événements de capture dans votre application
- **Prise en charge multiplateforme** — Fonctionne sur les environnements Windows de bureau

## Prise en charge étendue des formats

Notre SDK prend en charge une large gamme de formats de sortie afin de répondre à des exigences de projet variées, garantissant que vos applications peuvent livrer la vidéo exactement dans le format dont vos utilisateurs ont besoin :

### Formats vidéo standards

- [MP4 (H.264/AAC)](../../general/output-formats/mp4.md) — Format standard de l'industrie offrant une excellente compatibilité sur l'ensemble des appareils et plateformes, idéal pour la distribution et les applications de streaming
- [WebM](../../general/output-formats/webm.md) — Format open source optimisé pour les applications web grâce à une compression efficace et à un large support des navigateurs
- [AVI](../../general/output-formats/avi.md) — Format conteneur classique offrant une compatibilité étendue et une surcharge de traitement minimale, prenant en charge pratiquement tous les codecs compatibles DirectShow
- [WMV](../../general/output-formats/wmv.md) — Format vidéo de Microsoft offrant de bons taux de compression et une bonne intégration avec les environnements Windows

### Formats professionnels et de diffusion

- [MKV (Matroska)](../../general/output-formats/mkv.md) — Format conteneur flexible prenant en charge plusieurs pistes audio, vidéo et de sous-titres, idéal pour l'archivage et le stockage en haute qualité
- [MOV (QuickTime)](../../general/output-formats/mov.md) — Format conteneur d'Apple largement utilisé dans les flux de travail professionnels d'édition et de production vidéo
- [MPEG-TS (Transport Stream)](../../general/output-formats/mpegts.md) — Format optimisé pour les applications de diffusion et de streaming avec une correction d'erreurs robuste
- [MXF (Material Exchange Format)](../../general/output-formats/mxf.md) — Format de production vidéo professionnel utilisé dans les environnements de diffusion et de post-production

### Options de sortie spécialisées

- [GIF (Graphics Interchange Format)](../../general/output-formats/gif.md) — Parfait pour créer de courtes animations en boucle avec une large compatibilité web
- [DV (Digital Video)](dv.md) — Format de qualité professionnelle couramment utilisé avec les caméscopes numériques, préservant une vidéo de haute qualité pour les flux d'édition
- [Intégration FFMPEG](../../general/output-formats/ffmpeg-exe.md) — Options d'encodage avancées tirant parti de la puissante bibliothèque FFMPEG pour des exigences d'encodage spécialisées
- [Solutions de sortie personnalisées](../../general/output-formats/custom.md) — Créez vos propres spécifications de format et pipelines de traitement pour des besoins applicatifs uniques

### Capture optimisée matériellement

- [Intégration caméscope MPEG-2](mpeg2-camcorder.md) — Capture directe depuis des caméscopes numériques avec un encodage optimisé matériellement pour une efficacité maximale
- [Tuner TV MPEG-2 avec encodage matériel](mpeg2-tvtuner.md) — Spécialement optimisé pour les périphériques de capture télévisuelle, en tirant parti de l'accélération matérielle lorsqu'elle est disponible

## Encodeurs vidéo avancés

Notre SDK intègre plusieurs encodeurs vidéo avancés afin d'offrir une efficacité de compression et des performances optimales pour différents scénarios de capture :

### Encodeurs modernes à haute efficacité

- [H.264 (AVC)](../../general/video-encoders/h264.md) — Encodeur standard de l'industrie offrant une excellente compatibilité et de multiples options d'accélération matérielle de NVIDIA, AMD et Intel
- [HEVC (H.265)](../../general/video-encoders/hevc.md) — Encodeur de nouvelle génération offrant une compression environ 50 % meilleure que H.264, avec prise en charge du 4K et du contenu HDR
- [AV1](../../general/video-encoders/av1.md) — Standard ouvert libre de droits offrant une efficacité de compression supérieure, idéal pour les applications de streaming web

### Encodeurs spécialisés et hérités

- [MJPEG](../../general/video-encoders/mjpeg.md) — Encodeur Motion JPEG offrant une compression image par image à faible latence avec des implémentations matérielles et logicielles
- [VP8/VP9](../../general/video-encoders/vp8-vp9.md) — Codecs open source de Google offrant des rapports qualité/débit compétitifs pour les conteneurs WebM

### Prise en charge de l'accélération matérielle

Nos encodeurs prennent en charge l'accélération matérielle des principaux fournisseurs :

- **NVIDIA NVENC** — Encodage accéléré par GPU avec une utilisation CPU minimale
- **AMD AMF** — Advanced Media Framework pour un encodage efficace sur GPU AMD
- **Intel QuickSync** — Traitement vidéo accéléré matériellement par Intel
- **Solutions de repli logicielles** — Implémentations logicielles complètes lorsque l'accélération matérielle n'est pas disponible

Pour les spécifications détaillées des encodeurs, les options de configuration et les comparaisons de performances, consultez notre [guide des encodeurs vidéo](../../general/video-encoders/index.md).

## Techniques d'implémentation avancées

- [Aperçu et capture simultanés](separate-capture.md) — Implémentez l'aperçu et l'enregistrement simultanés pour améliorer l'expérience utilisateur
- **Optimisation de la mémoire** — Bonnes pratiques pour la gestion de la mémoire lors de longues sessions d'enregistrement
- **Gestion des threads** — Recommandations pour un threading approprié afin de garantir des applications réactives

## Ressources pour les développeurs

Pour des exemples d'implémentation supplémentaires, une documentation détaillée et des exemples de code, consultez notre [dépôt GitHub](https://github.com/visioforge/.Net-SDK-s-samples).
