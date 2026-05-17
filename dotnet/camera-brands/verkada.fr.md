---
title: Options d'intégration RTSP et C# .NET pour caméras Verkada
description: Options d'intégration des caméras Verkada en C# .NET. Comprenez l'architecture gérée dans le cloud, les limitations RTSP et les approches alternatives.
meta:
  - name: robots
    content: "noindex, follow"
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Webcam
  - IP Camera
  - RTSP
  - ONVIF
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - SystemVideoSourceBlock
  - VideoRendererBlock

---

# Comment se connecter à une caméra Verkada en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Verkada** est une société américaine de caméras de sécurité gérées dans le cloud dont le siège est à San Mateo, en Californie. Fondée en 2016, Verkada propose des caméras d'entreprise avec une architecture entièrement gérée dans le cloud. Contrairement aux caméras IP traditionnelles, les caméras Verkada sont gérées exclusivement via la plateforme cloud de Verkada — il n'y a pas de flux RTSP locaux, de prise en charge ONVIF ni d'accès réseau direct aux caméras.

**Faits clés :**

- **Gammes de produits :** CD (mini dôme), CB (bullet), CE (dôme extérieur), CF (fisheye), CM (multi-capteurs), CP (PTZ)
- **Architecture :** Gérée dans le cloud — tout le traitement vidéo et l'accès passent par la plateforme Verkada Command
- **Prise en charge RTSP :** Non
- **Prise en charge ONVIF :** Non
- **Accès réseau local :** Aucun accès direct — les caméras communiquent uniquement avec le cloud Verkada
- **Codecs vidéo :** H.264, H.265 (gérés par la plateforme cloud)
- **Accès API :** API Verkada (basée sur le cloud, nécessite un abonnement entreprise)

!!! warning "Pas de RTSP ni de streaming local"
    Les caméras Verkada ne prennent **pas** en charge RTSP, ONVIF ou tout autre protocole de streaming local standard. Ce sont des appareils gérés dans le cloud, accessibles uniquement via la plateforme Command ou l'API de Verkada. L'intégration directe avec la source RTSP du SDK VisioForge n'est pas possible avec les caméras Verkada.

## Pourquoi Verkada n'a pas de RTSP

L'architecture de Verkada est fondamentalement différente de celle des caméras IP traditionnelles :

1. **Conception cloud-first :** La vidéo est traitée sur la caméra et diffusée vers le cloud Verkada
2. **Pas de ports réseau locaux :** Les caméras n'exposent pas le port 554 ni aucun point de terminaison RTSP
3. **Accès géré :** Tout l'accès vidéo passe par Verkada Command (web/mobile)
4. **Sécurité zero-trust :** Pas de connexions directes caméra-client sur le LAN

Cette architecture offre un déploiement simplifié et une gestion centralisée mais élimine l'intégration directe via SDK.

## Options d'intégration

### Option 1 : API Verkada (basée sur le cloud)

Verkada propose une API REST pour les clients entreprise qui fournit :

- Liste et statut des caméras
- Export/téléchargement de clips vidéo
- Récupération de miniatures/captures
- Données d'événements et d'alertes

L'API **ne fournit pas** de flux RTSP en direct ni de flux vidéo en temps réel. Elle est conçue pour la récupération de clips et l'accès aux métadonnées.

### Option 2 : Sortie HDMI (certains modèles)

Certains modèles Verkada incluent un port de sortie HDMI pour l'affichage local. Vous pouvez capturer cette sortie à l'aide d'une carte de capture HDMI :

```csharp
// Capturer la sortie HDMI d'une caméra Verkada via une carte de capture USB
var pipeline = new MediaBlocksPipeline();

// Utiliser la source vidéo système (la carte de capture HDMI apparaît comme une webcam)
var captureDevice = new SystemVideoSourceBlock(captureDeviceSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(captureDevice.Output, videoRenderer.Input);
await pipeline.StartAsync();
```

Cette approche fournit une vidéo locale en temps réel mais nécessite une connectivité HDMI physique et une carte de capture.

### Option 3 : Caméras alternatives avec RTSP

Si vous avez besoin d'une intégration RTSP directe avec des caméras d'entreprise, envisagez ces alternatives :

| Alternative | Segment de marché | RTSP | ONVIF | Guide |
|------------|---------------|------|-------|-------|
| Axis | Entreprise | Oui | Oui | [Guide de connexion](axis.md) |
| Bosch | Entreprise | Oui | Oui | [Guide de connexion](bosch.md) |
| Hanwha Vision | Entreprise | Oui | Oui | [Guide de connexion](hanwha.md) |
| Avigilon | Entreprise | Oui | Oui | [Guide de connexion](avigilon.md) |
| Hikvision | Entreprise | Oui | Oui | [Guide de connexion](hikvision.md) |

## FAQ

**Puis-je me connecter aux caméras Verkada avec RTSP ?**

Non. Les caméras Verkada ne prennent pas en charge RTSP, ONVIF ou tout autre protocole de streaming local. Ce sont des appareils gérés dans le cloud, accessibles uniquement via la plateforme Command ou l'API de Verkada.

**Verkada dispose-t-il d'une API pour l'accès vidéo ?**

Oui, mais l'API Verkada fournit l'export de clips et la récupération de captures, pas le streaming vidéo en direct. La vidéo en temps réel n'est disponible que via l'interface web Verkada Command ou l'application mobile. L'accès à l'API entreprise nécessite un abonnement Verkada.

**Puis-je utiliser le SDK VisioForge avec les caméras Verkada ?**

Pas directement via RTSP. La seule option d'intégration locale consiste à capturer la sortie HDMI de certains modèles Verkada à l'aide d'une carte de capture avec la source vidéo système du SDK VisioForge. Pour une intégration basée sur le cloud, vous devrez utiliser séparément l'API de Verkada.

**Quelles caméras d'entreprise prennent en charge RTSP ?**

Pour les caméras d'entreprise avec prise en charge RTSP et ONVIF complète, consultez nos guides pour [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md), [Avigilon](avigilon.md) et [Hikvision](hikvision.md).

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Axis](axis.md) — Alternative d'entreprise avec RTSP
- [Guide de connexion Bosch](bosch.md) — Alternative d'entreprise avec RTSP
- [Guide de connexion Rhombus](rhombus.md) — Une autre plateforme gérée dans le cloud
- [Installation du SDK et exemples](index.md#get-started)
