---
title: URL RTSP Tenda — guide de connexion en C# .NET VisioForge
description: Modèles d'URL RTSP pour les caméras Tenda CP, CT, IT et pan/tilt en C# .NET. Streaming et enregistrement avec le VisioForge Video Capture SDK.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - C#

---

# Comment se connecter à une caméra IP Tenda en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Tenda Technology** est un fabricant chinois d'équipements réseau dont le siège est à Shenzhen, en Chine. Fondée en 1999, Tenda est principalement connue pour ses routeurs et son matériel réseau, mais elle s'est étendue au marché des caméras de sécurité avec une gamme croissante de caméras IP abordables ciblant les segments grand public et des petites entreprises. Les caméras Tenda gagnent du terrain sur les marchés émergents d'Asie, d'Amérique du Sud et d'Afrique.

**Faits clés :**

- **Gammes de produits :** CP (pan/tilt), CT (bullet/turret extérieur), IT (intérieur)
- **Prise en charge des protocoles :** RTSP, ONVIF (certains modèles), HTTP
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (varie selon le modèle)
- **Prise en charge ONVIF :** Oui (modèles plus récents)
- **Codecs vidéo :** H.264, H.265 (certains modèles)
- **Application compagnon :** Tenda Security (pour la configuration et la visualisation à distance)

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Tenda utilisent une URL RTSP basée sur un numéro de flux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Flux | Modèle d'URL | Description |
|--------|-------------|-------------|
| Flux principal | `rtsp://IP:554/stream1` | Résolution complète |
| Sous-flux | `rtsp://IP:554/stream2` | Résolution plus faible |

### Modèles de caméras

| Modèle | Type | Résolution | URL du flux principal | Audio |
|-------|------|-----------|----------------|-------|
| CP3 (pan/tilt intérieur) | PTZ intérieur | 1920x1080 | `rtsp://IP:554/stream1` | Oui |
| CP6 (pan/tilt 2K) | PTZ intérieur | 2304x1296 | `rtsp://IP:554/stream1` | Oui |
| CP7 (pan/tilt 4MP) | PTZ intérieur | 2560x1440 | `rtsp://IP:554/stream1` | Oui |
| CT3 (bullet extérieur) | Extérieur | 1920x1080 | `rtsp://IP:554/stream1` | Oui |
| CT6 (extérieur 2K) | Extérieur | 2304x1296 | `rtsp://IP:554/stream1` | Oui |
| CT7 (extérieur 4MP) | Extérieur | 2560x1440 | `rtsp://IP:554/stream1` | Oui |
| IT6 (intérieur) | Intérieur | 1920x1080 | `rtsp://IP:554/stream1` | Oui |
| IT7 (intérieur 2K) | Intérieur | 2304x1296 | `rtsp://IP:554/stream1` | Oui |

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/stream1` | Flux principal (recommandé) |
| `rtsp://IP:554/stream2` | Sous-flux |
| `rtsp://IP:554/live/ch0` | Format alternatif (certains modèles) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Compatible Dahua (certains firmwares OEM) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Tenda avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Tenda CP7 (pan/tilt 4MP), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/stream2` à la place de `/stream1`.

## URL de capture instantanée

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification basique |

## Dépannage

### La caméra nécessite d'abord une configuration via l'application

Les caméras Tenda doivent initialement être configurées via l'application Tenda Security. La caméra a besoin d'identifiants Wi-Fi et de la configuration d'un compte avant que RTSP ne soit accessible. Après la configuration, vous pouvez vous connecter directement via RTSP sur le réseau local.

### Plusieurs formats d'URL

Certaines caméras Tenda utilisent différentes bases de firmware. Si `/stream1` ne fonctionne pas, essayez :

1. `rtsp://IP:554/live/ch0` (format alternatif)
2. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (compatible Dahua)
3. Utilisez la découverte ONVIF pour récupérer automatiquement l'URL correcte

### Trouver l'adresse IP de la caméra

Les caméras WiFi Tenda obtiennent leur IP via DHCP. Trouvez-la dans :

1. L'application Tenda Security (Device Info)
2. La liste des clients DHCP de votre routeur
3. La découverte ONVIF (si prise en charge)

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Tenda ?**

La plupart des caméras Tenda utilisent `rtsp://admin:password@CAMERA_IP:554/stream1` pour le flux principal et `/stream2` pour le sous-flux. Certains modèles utilisent des chemins d'URL alternatifs.

**Les caméras Tenda prennent-elles en charge ONVIF ?**

Les modèles de caméras Tenda plus récents prennent en charge ONVIF pour la découverte et le streaming standardisés. Les modèles plus anciens ou économiques peuvent ne pas le faire. Vérifiez les spécifications de votre caméra dans l'application Tenda Security.

**Les caméras Tenda conviennent-elles à l'intégration de développement ?**

Les caméras Tenda offrent des prix compétitifs et une prise en charge RTSP standard, ce qui les rend adaptées au développement et au prototypage. Pour les déploiements en production nécessitant une compatibilité RTSP/ONVIF garantie, envisagez des marques de vidéosurveillance établies comme [Hikvision](hikvision.md), [Dahua](dahua.md) ou [Reolink](reolink.md).

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion TP-Link](tp-link.md) — Segment grand public similaire
- [Guide de connexion Mercusys](mercusys.md) — Alternative de caméra économique
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
