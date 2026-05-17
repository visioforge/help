---
title: Connecter une caméra IP Arlo en C# .NET — RTSP et streaming
description: Limitations RTSP des caméras Arlo en C# .NET. Pas de prise en charge RTSP native. Options de contournement et alternatives recommandées pour développeurs.
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

# Comment se connecter à une caméra Arlo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Arlo Technologies** est une entreprise américaine de sécurité de maison connectée dont le siège est à Carlsbad, en Californie. À l'origine marque Netgear, Arlo est devenu indépendant en 2018. Arlo est l'une des marques de caméras de sécurité sans fil les plus vendues en Amérique du Nord et en Europe, connue pour ses caméras extérieures alimentées par batterie, ses sonnettes et ses caméras avec projecteur.

**Faits clés :**

- **Gammes de produits :** Pro (haut de gamme), Ultra (4K), Essential (entrée de gamme), Go (cellulaire), Floodlight, Doorbell
- **Architecture :** Cloud d'abord avec stockage local optionnel (Arlo SmartHub/Station de base)
- **Prise en charge RTSP :** Non (retirée du SmartHub en 2021)
- **Prise en charge ONVIF :** Non
- **Codecs vidéo :** H.264, H.265 (modèles sélectionnés)
- **Dépendance cloud :** Élevée — toutes les fonctionnalités nécessitent un abonnement Arlo Secure
- **Alimentation :** Batterie, solaire ou filaire selon le modèle

!!! warning "Pas de prise en charge RTSP"
    Les caméras Arlo ne prennent **pas** en charge RTSP. Arlo proposait auparavant l'accès RTSP via le SmartHub (VMB4540/VMB5000) pour des modèles de caméras sélectionnés, mais cette fonctionnalité a été **retirée** dans une mise à jour du firmware en 2021. Il n'existe actuellement aucun moyen d'accéder aux flux des caméras Arlo via RTSP.

## Historique RTSP sur Arlo

Arlo a connu une brève période de prise en charge RTSP :

| Période | Statut | Détails |
|--------|--------|---------|
| Avant 2019 | Pas de RTSP | Accès cloud uniquement |
| 2019-2021 | RTSP disponible (bêta) | Via SmartHub pour Ultra/Pro 3/Pro 4 uniquement |
| 2021-aujourd'hui | RTSP retiré | Mise à jour firmware retirant la fonctionnalité RTSP |

La fonctionnalité RTSP était disponible sur l'**Arlo SmartHub (VMB5000)** pour :
- Arlo Ultra (VMC5040)
- Arlo Pro 3 (VMC4040P)
- Arlo Pro 4 (VMC4050P)

Elle n'a jamais été disponible pour les modèles Arlo Essential, Go ou Doorbell.

## Pourquoi pas d'intégration directe

L'architecture d'Arlo empêche l'intégration directe au SDK :

1. **Streaming obligatoirement cloud :** Toute la vidéo transite par les serveurs cloud d'Arlo
2. **Pas d'accès réseau local :** Les caméras communiquent avec le SmartHub/station de base via des protocoles propriétaires
3. **Pas de ports ouverts :** Ni les caméras ni les stations de base n'exposent de points de terminaison vidéo RTSP ou HTTP
4. **Dépendance à l'abonnement :** L'accès vidéo nécessite un plan Arlo Secure actif

## Contournements possibles

### Option 1 : API Arlo (non officielle)

Des bibliothèques développées par la communauté existent qui s'interfacent avec l'API cloud d'Arlo pour :

- Récupérer des images de capture instantanée
- Télécharger les clips enregistrés
- Déclencher des actions de la caméra

Ces bibliothèques ne sont pas officielles et peuvent cesser de fonctionner avec les mises à jour des services Arlo. Elles ne fournissent pas de flux RTSP en temps réel.

### Option 2 : Sortie HDMI du SmartHub

L'Arlo SmartHub (VMB5000) dispose d'une sortie HDMI qui affiche une grille en direct des caméras. Vous pouvez capturer celle-ci avec une carte de capture HDMI :

```csharp
// Capturer la sortie HDMI du SmartHub Arlo via une carte de capture USB
var pipeline = new MediaBlocksPipeline();

var captureDevice = new SystemVideoSourceBlock(captureDeviceSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(captureDevice.Output, videoRenderer.Input);
await pipeline.StartAsync();
```

Cela fournit une vue composite de toutes les caméras, et non des flux individuels.

## Alternatives recommandées

Pour les développeurs ayant besoin d'une intégration directe avec des caméras RTSP, ces caméras grand public offrent une prise en charge RTSP native :

| Alternative | Type | RTSP | Option batterie | Guide |
|------------|------|------|---------------|-------|
| Reolink Argus 3 | Batterie extérieure | Oui | Oui | [Guide de connexion](reolink.md) |
| Amcrest | Filaire extérieure | Oui | Non | [Guide de connexion](amcrest.md) |
| EZVIZ | Intérieur/extérieur | Oui (activation requise) | Limitée | [Guide de connexion](ezviz.md) |
| TP-Link Tapo | Intérieur/extérieur | Oui | Non | [Guide de connexion](tp-link.md) |
| Eufy Security | Filaire/batterie | Oui (certains modèles) | Oui | [Guide de connexion](eufy.md) |

## FAQ

**Les caméras Arlo prennent-elles en charge RTSP ?**

Non. Les caméras Arlo ne prennent pas actuellement en charge RTSP. Une brève bêta RTSP était disponible sur le SmartHub (2019-2021) pour des modèles sélectionnés, mais elle a été retirée dans une mise à jour du firmware. Il n'existe actuellement aucun moyen d'accéder aux flux Arlo via RTSP.

**Puis-je utiliser le SDK VisioForge avec des caméras Arlo ?**

Pas directement. Les caméras Arlo n'ont pas de points de terminaison RTSP, ONVIF ou de streaming local. La seule option d'intégration est de capturer la sortie HDMI du SmartHub à l'aide d'une carte de capture. Pour une intégration directe au SDK, utilisez des caméras avec prise en charge RTSP native.

**Arlo ramènera-t-il RTSP ?**

Il n'y a pas eu d'annonce officielle d'Arlo concernant la restauration de la prise en charge RTSP. Le modèle d'affaires d'Arlo est basé sur l'abonnement, et le streaming local entre en conflit avec cette approche.

**Quelles caméras à batterie prennent en charge RTSP ?**

Pour les caméras alimentées par batterie avec prise en charge RTSP, envisagez [Reolink](reolink.md) (série Argus) ou [Eufy Security](eufy.md) (modèles sélectionnés). La plupart des caméras à batterie d'autres marques sont également cloud uniquement.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Reolink](reolink.md) — Alternative grand public avec RTSP
- [Guide de connexion Eufy Security](eufy.md) — Grand public avec RTSP partiel
- [Guide de connexion Wyze](wyze.md) — Une autre marque cloud d'abord avec RTSP limité
- [Installation du SDK et exemples](index.md#get-started)
