---
title: Modèles d'URL RTSP Annke — caméra IP avec exemples C# .NET
description: Modèles d'URL RTSP Annke C500, C800, CZ400, NC400 et NVR pour C# .NET. Intégrez avec le VisioForge Video Capture SDK pour streaming et enregistrement.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Annke en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Annke** (Annke Innovation Co., Ltd.) est une marque grand public et prosumer de caméras de sécurité basée à Hong Kong, vendant principalement via Amazon et en direct au consommateur. Les caméras Annke sont fabriquées avec du matériel OEM **Hikvision**, et la plupart des modèles utilisent un firmware et des modèles d'URL RTSP compatibles Hikvision. Annke propose des prix compétitifs sur les caméras PoE, les NVR et les kits de vidéosurveillance complets.

**Faits clés :**

- **Gammes de produits :** Série C (caméras IP), série CZ (PTZ), série NC (NVR), série I (turret/dôme)
- **Prise en charge des protocoles :** RTSP, ONVIF Profile S, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de la configuration initiale ; certains modèles : admin / admin)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265 (4MP et au-dessus)
- **Base OEM :** Hikvision (la plupart des modèles utilisent un firmware compatible Hikvision)

!!! info "Annke utilise le firmware Hikvision"
    La plupart des caméras Annke utilisent le firmware OEM Hikvision. Le format d'URL RTSP (`/Streaming/Channels/`) est identique à celui d'Hikvision. Consultez notre [guide de connexion Hikvision](hikvision.md) pour plus de détails et le dépannage.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Annke utilisent le modèle d'URL Hikvision `Streaming/Channels` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| ID de canal | Flux | Description |
|-----------|--------|-------------|
| 101 | Flux principal | Résolution complète |
| 102 | Sous-flux | Résolution inférieure |
| 103 | Troisième flux | Optimisé mobile (si pris en charge) |

### Modèles de caméras

| Modèle | Résolution | URL du flux principal | Audio |
|-------|-----------|----------------|-------|
| C500 (bullet 5MP) | 2592x1944 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| C800 (bullet 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| C1200 (bullet 12MP) | 4000x3000 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| CZ400 (PTZ 4MP) | 2560x1440 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| I91BN (turret 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| I91BM (dôme 4K) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Oui |
| NC400 (NVR 4 canaux) | N/A | Voir section NVR | N/A |
| N48PAW (NVR 8 canaux PoE) | N/A | Voir section NVR | N/A |

### URL des canaux NVR

Pour les NVR Annke (NC400, N48PAW, N46PCK, etc.) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Caméra 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Caméra N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Formats d'URL alternatifs

Certains modèles Annke (en particulier les variantes non Hikvision OEM) utilisent des modèles d'URL différents :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Style Hikvision (la plupart des modèles) |
| `rtsp://IP:554/h264/ch1/main/av_stream` | Firmware Hikvision plus ancien |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Style Dahua (certains anciens modèles) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Annke avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Annke C800 (bullet 4K), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez plutôt `/Streaming/Channels/102`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/ISAPI/Streaming/channels/101/picture` | Nécessite une authentification digest |
| Flux MJPEG | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Sous-flux en MJPEG |
| Capture historique | `http://IP/Streaming/channels/1/picture` | Firmware plus ancien |

## Dépannage

### La caméra nécessite une activation

Les caméras Annke avec un firmware plus récent nécessitent une activation initiale (configuration du mot de passe) avant que l'accès RTSP ne fonctionne. Utilisez l'interface web de la caméra à l'adresse `http://CAMERA_IP` ou l'outil de découverte compatible SADP d'Annke.

### Le format d'URL Hikvision ne fonctionne pas

Certains modèles Annke utilisent un firmware OEM différent. Si `/Streaming/Channels/101` ne fonctionne pas, essayez :

1. `/h264/ch1/main/av_stream` (firmware Hikvision plus ancien)
2. `/cam/realmonitor?channel=1&subtype=0` (style Dahua)
3. Utilisez la découverte ONVIF pour récupérer automatiquement l'URL de flux correcte

### Problèmes de flux H.265

Les caméras Annke 4K (C800, I91BN) utilisent par défaut l'encodage H.265. Si la lecture échoue, basculez la caméra en H.264 dans l'interface web ou installez le redistribuable du décodeur HEVC.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Annke ?**

La plupart des caméras Annke utilisent `rtsp://admin:password@CAMERA_IP:554/Streaming/Channels/101` pour le flux principal. Utilisez le canal `102` pour le sous-flux. C'est le même format que les caméras Hikvision.

**Les caméras Annke sont-elles des OEM Hikvision ?**

La plupart des caméras Annke utilisent du matériel et du firmware OEM Hikvision. Le format d'URL RTSP, l'interface web et l'API sont généralement identiques à Hikvision. Certains modèles Annke peuvent utiliser des bases OEM différentes.

**Les caméras Annke prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Annke actuelles prennent en charge ONVIF Profile S, fournissant une découverte standardisée et un accès aux flux.

**Puis-je mélanger des caméras Annke avec des NVR Hikvision ?**

Oui. Comme les caméras Annke utilisent des protocoles compatibles Hikvision, elles fonctionnent nativement avec les NVR Hikvision et vice versa. Vous pouvez également intégrer des caméras Annke dans n'importe quel NVR ou VMS compatible ONVIF.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Hikvision](hikvision.md) — Même format d'URL (base OEM)
- [Guide de connexion LTS](lts.md) — Un autre OEM Hikvision
- [Guide de connexion Dahua](dahua.md) — Écosystème OEM alternatif
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
