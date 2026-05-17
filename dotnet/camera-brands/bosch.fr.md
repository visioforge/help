---
title: Modèles d'URL RTSP Bosch — caméras IP en C# .NET VisioForge
description: URL RTSP Bosch Dinion, Flexidome, Autodome et encodeurs VIP pour C# .NET. SDK VisioForge pour la vidéosurveillance entreprise.
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
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Bosch en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Bosch Security and Safety Systems** (une division de Robert Bosch GmbH) est un fabricant allemand d'équipements de vidéosurveillance professionnels et entreprise. Dont le siège est à Grasbrunn près de Munich, Bosch produit des caméras IP, des encodeurs, des solutions d'enregistrement et des analyses vidéo principalement pour les marchés des infrastructures critiques, du transport et de la sécurité entreprise.

**Faits clés :**

- **Gammes de produits :** Dinion (bullet/boîtier), Flexidome (dôme), Autodome (PTZ), MIC (renforcées), NBN/NDN/NTC (réseau historique), NWC (compactes), VideoJet/VIP (encodeurs)
- **Prise en charge des protocoles :** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Bosch VMS (BVMS), enregistrement iSCSI direct
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Variables selon le modèle et la version du firmware ; beaucoup nécessitent une configuration via Bosch Configuration Manager
- **Prise en charge ONVIF :** Oui (toutes les caméras IP actuelles)
- **Codecs vidéo :** H.264, H.265, MJPEG
- **Caractéristique unique :** Mode tunnel RTSP pour traverser les pare-feu

## Modèles d'URL RTSP

Les caméras Bosch utilisent plusieurs modèles d'URL selon la génération du modèle. Les plus courants sont les chemins `/rtsp_tunnel` et `/video`.

### Modèles actuels (firmware Bosch série CPP)

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux vidéo 1 | `rtsp://IP:554/video?inst=1` | Flux principal |
| Flux vidéo 2 | `rtsp://IP:554/video?inst=2` | Sous-flux |
| Tunnel RTSP | `rtsp://IP:554//rtsp_tunnel` | Compatible pare-feu (notez la double barre) |
| H.264 direct | `rtsp://IP:554/h264` | Flux H.264 direct |

!!! info "Mode tunnel RTSP"
    L'URL `//rtsp_tunnel` (avec double barre) est le mode de tunnel RTSP propriétaire de Bosch qui fonctionne mieux à travers les pare-feu et le NAT. Il encapsule les données RTP dans la connexion TCP RTSP. Utilisez l'URL `/video` standard pour la plupart des intégrations.

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Codec | Notes |
|-------------|----------|-------|-------|
| Dinion IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Actuel |
| Flexidome IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Actuel |
| Autodome IP 4000/5000/7000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | PTZ actuel |
| MIC IP fusion/starlight/ultra | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Renforcée |
| NDC-225-PI | `rtsp://IP:554//rtsp_tunnel` | H.264 | Historique |
| NDC-255-P | `rtsp://IP:554//rtsp_tunnel` | H.264 | Historique |
| NDC-265-P | `rtsp://IP:554/h264` | H.264 | Historique |
| NDN-832v | `rtsp://IP:554//rtsp_tunnel` | H.264 | Dôme historique |
| NTC-255-PI | `rtsp://IP:554/video` | H.264 | Thermique historique |
| NTC-265-PI | `rtsp://IP:554/h264` | H.264 | Thermique historique |
| NTI-50022-V3 | `rtsp://IP:554/h264` | H.264 | Bullet IP |
| NWC-0455-20P | `rtsp://IP:554/h264` | H.264 | Compacte |

### URL des encodeurs

Les encodeurs vidéo Bosch (séries VideoJet, VIP) permettent de connecter des caméras analogiques aux réseaux IP :

| Encodeur | URL RTSP | Notes |
|---------|----------|-------|
| VideoJet 10 | `rtsp://IP:554/video?inst=1` | Canal unique |
| VIP X1 | `rtsp://IP:554//rtsp_tunnel` | Canal unique |
| VIP X1600 | `rtsp://IP:554/video?inst=1` | Multi-canal |
| VIP X2 | `rtsp://IP:554/video?inst=1` | Double canal |

### URL RTSP des DVR

| Modèle DVR | URL RTSP | Notes |
|-----------|----------|-------|
| DVR 440/480/600 | `rtsp://IP:554/rtsp_tunnel` | Barre unique |
| DVR 440/480/600 | `rtsp://IP:554/video` | Alternative |
| DVR (canal) | `rtsp://IP:554/cgi-bin/rtspStream/CHANNEL` | Spécifique au canal |
| DVR (SDP) | `rtsp://IP:554/user=USER&password=PASS&channel=1&stream=0.sdp?` | Basé sur SDP |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Bosch avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Bosch Dinion/Flexidome, flux principal
var uri = new Uri("rtsp://192.168.1.60:554/video?inst=1");
var username = "service";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `?inst=2` au lieu de `?inst=1`. Pour les modèles Bosch historiques, utilisez l'URL de tunnel RTSP `//rtsp_tunnel` (notez la double barre).

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/snap.jpg` | Capture basique |
| Capture (dimensionnée) | `http://IP/snap.jpg?JpegSize=XL` | Tailles XL, M disponibles |
| Capture (canal) | `http://IP/snap.jpg?JpegCam=CHANNEL` | Encodeurs multi-canaux |
| Capture (auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | Authentification par URL |
| Flux MJPEG | `http://IP/img/mjpeg.jpg` | MJPEG continu |
| Image | `http://IP/img.jpg` | Image unique |
| Image (alt) | `http://IP/image.jpg` | Chemin alternatif |

## Dépannage

### Double barre dans l'URL rtsp_tunnel

L'URL `//rtsp_tunnel` (avec double barre avant `rtsp_tunnel`) est intentionnelle pour les caméras Bosch historiques. Ce n'est pas une faute de frappe :

- Correct : `rtsp://IP:554//rtsp_tunnel`
- Incorrect : `rtsp://IP:554/rtsp_tunnel` (peut fonctionner sur certains modèles, mais pas tous)

### Bosch Configuration Manager requis

De nombreuses caméras Bosch nécessitent une configuration initiale via l'application de bureau **Bosch Configuration Manager** avant que l'accès RTSP ne fonctionne. La caméra peut ne pas répondre aux connexions RTSP tant que la configuration initiale n'est pas terminée.

### Le nom d'utilisateur par défaut varie

- **Modèles actuels :** utilisateur `service` avec mot de passe défini lors de la configuration
- **Modèles historiques :** Peuvent utiliser `admin`, `user` ou `service` selon le firmware
- Vérifiez Bosch Configuration Manager ou l'interface web de la caméra pour les paramètres utilisateur

### Paramètre inst

Le paramètre `?inst=1` sélectionne l'instance de flux vidéo :
- `inst=1` = Premier flux vidéo (principal)
- `inst=2` = Deuxième flux vidéo (secondaire)
- Tous les modèles ne prennent pas en charge plusieurs instances

### Sélection de canal d'encodeur

Pour les encodeurs multi-canaux (VIP X1600, série VideoJet X), utilisez le paramètre `inst` pour sélectionner le canal :
- `rtsp://IP:554/video?inst=1` = Canal 1
- `rtsp://IP:554/video?inst=2` = Canal 2

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Bosch ?**

Pour les caméras Bosch actuelles, l'URL est `rtsp://service:password@CAMERA_IP:554/video?inst=1`. Pour les modèles historiques, essayez `rtsp://CAMERA_IP:554//rtsp_tunnel` ou `rtsp://CAMERA_IP:554/h264`.

**Qu'est-ce que le mode tunnel RTSP Bosch ?**

Le tunnel RTSP (`//rtsp_tunnel`) est le mode propriétaire de Bosch qui encapsule les données RTP dans la connexion TCP RTSP, facilitant la traversée des pare-feu. C'est le mode de streaming par défaut sur de nombreuses caméras Bosch historiques.

**Les caméras Bosch prennent-elles en charge H.265 ?**

Les caméras IP Bosch actuelles (plateforme CPP13/CPP14, dont les séries Dinion/Flexidome 7000/8000) prennent en charge H.265. Les caméras historiques prennent en charge H.264 et MPEG-4. Consultez la fiche technique de votre modèle spécifique pour connaître la prise en charge des codecs.

**Puis-je utiliser des encodeurs Bosch pour connecter des caméras analogiques ?**

Oui. Les encodeurs Bosch VideoJet et VIP convertissent les signaux de caméras analogiques en flux IP accessibles via RTSP. Utilisez le même format d'URL (`/video?inst=1` ou `//rtsp_tunnel`) que pour les caméras IP.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Axis](axis.md) — Pair en vidéosurveillance entreprise
- [Guide de connexion Honeywell](honeywell.md) — Segment entreprise / commercial
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
