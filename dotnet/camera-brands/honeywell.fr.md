---
title: URL RTSP des caméras IP Honeywell en C# .NET — intégration
description: Connectez les caméras Honeywell Performance Series et equIP en C# .NET avec modèles d'URL RTSP et exemples pour HD, HDZ, HBD, HBW et modèles PSIA.
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
  - H.264
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Honeywell en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Honeywell Commercial Security** (partie de Honeywell Building Technologies) est un fabricant majeur d'équipements de vidéosurveillance entreprise. Les caméras Honeywell sont largement déployées dans les bâtiments commerciaux, les infrastructures critiques, les installations gouvernementales et les systèmes de transport dans le monde entier. Honeywell a acquis plusieurs marques de caméras au fil des ans, dont **Samsung Techwin** (brièvement), et commercialise des caméras sous les gammes **Performance Series**, **30 Series** et **60 Series**.

**Faits clés :**

- **Gammes de produits :** Performance Series (equIP, série H), 30 Series (HC30W, HC35W), 60 Series (HC60W), HDZ/HD (equIP historique), HBD/HBW (bullet/dôme), IPCAM (grand public)
- **Prise en charge des protocoles :** RTSP, ONVIF (Profile S/G/T), PSIA, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 1234 (Performance Series) ; admin / admin (modèles historiques) ; variable selon le modèle et le firmware
- **Prise en charge ONVIF :** Oui (toutes les caméras Performance Series et 30/60 Series actuelles)
- **Codecs vidéo :** H.264, H.265 (modèles actuels), MPEG-4 (historique)

## Modèles d'URL RTSP

### Modèles actuels (Performance Series, 30/60 Series)

| Flux | URL RTSP | Codec | Notes |
|--------|----------|-------|-------|
| Flux principal (H.264) | `rtsp://IP:554/h264` | H.264 | Flux principal |
| Flux principal (H.265) | `rtsp://IP:554/h265` | H.265 | Modèles actuels uniquement |
| Flux de canal (H.264) | `rtsp://IP:554/cam1/h264` | H.264 | Spécifique au canal |
| Flux de canal (MPEG-4) | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Solution de repli historique |
| Flux PSIA | `rtsp://IP:554/PSIA/Streaming/channels/1` | H.264 | Compatible PSIA |

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Résolution | Notes |
|-------------|----------|------------|-------|
| HC30W/HC35W (30 Series) | `rtsp://IP:554/h264` | Jusqu'à 5MP | Wi-Fi actuel |
| HC60W (60 Series) | `rtsp://IP:554/h264` | Jusqu'à 4K | Filaire actuel |
| HD45IP | `rtsp://IP:554/h264` | 1080p | Dôme equIP |
| HD54IP | `rtsp://IP:554/h264` | 1080p | Boîtier equIP |
| HD55IPX | `rtsp://IP:554/h264` | 1080p+ | Boîtier equIP |
| HDZ20HDEX/HDZ20HDX | `rtsp://IP:554/h264` | 1080p | PTZ equIP |
| HD4MDIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Multi-canal |
| HDM3DIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Mini dôme |
| Série HBD/HBW | `rtsp://IP:554/h264` | Jusqu'à 4MP | Bullet/dôme |

### Streaming PSIA

Les caméras Honeywell qui prennent en charge **PSIA (Physical Security Interoperability Alliance)** utilisent un format d'URL différent :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1` | Premier canal |
| Canal 2 | `rtsp://IP:554/PSIA/Streaming/channels/2` | Deuxième canal |

### Modèles historiques (HTTP uniquement)

Les anciennes caméras grand public Honeywell (série IPCAM) utilisent HTTP :

| Modèle | URL | Notes |
|-------|-----|-------|
| IPCAM / IPCAM-PT | `http://IP/img/snapshot.cgi?size=3` | Capture JPEG |
| IPCAM-PT | `http://IP/img/video.mjpeg` | Flux MJPEG |
| IPCAM-PT | `http://IP/img/video.asf` | Flux ASF (audio) |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.mjpeg` | Flux MJPEG |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.asf` | Flux ASF (audio) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Honeywell avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Honeywell Performance Series, flux principal
var uri = new Uri("rtsp://192.168.1.75:554/h264");
var username = "admin";
var password = "YourPassword";
```

Pour les flux de canal PSIA, utilisez plutôt `/PSIA/Streaming/channels/1`. Pour les modèles multi-canaux, utilisez le format `/cam1/h264`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/img/snapshot.cgi?size=3` | La plupart des modèles |
| Flux MJPEG | `http://IP/img/video.mjpeg` | MJPEG continu |
| Flux ASF | `http://IP/img/video.asf` | ASF avec audio |
| Capture HREP | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=1` | Capture canal NVR |

## Dépannage

### Format d'URL RTSP

Les caméras Honeywell utilisent un format d'URL RTSP simple par rapport à d'autres marques :

- Principal : `rtsp://IP:554/h264` (pas de chemins complexes)
- Multi-canal : `rtsp://IP:554/cam1/h264` (numéro de canal dans le chemin)
- PSIA : `rtsp://IP:554/PSIA/Streaming/channels/1` (standard PSIA)

Si `/h264` ne fonctionne pas, essayez `/cam1/h264` ou l'URL PSIA.

### Les identifiants par défaut varient

Honeywell a utilisé différents identifiants par défaut selon les gammes de produits :

- **Performance Series :** admin / 1234 (doit être modifié lors de la première connexion)
- **30/60 Series :** Défini lors de la configuration initiale (pas de valeur par défaut)
- **equIP historique :** admin / admin
- **Série IPCAM :** admin / (vide) ou admin / admin

### PSIA vs ONVIF

Les caméras Honeywell prennent en charge à la fois les protocoles PSIA et ONVIF :

- **ONVIF** est recommandé pour les nouvelles intégrations (compatibilité plus large)
- **PSIA** est le standard d'interopérabilité historique de Honeywell, toujours pris en charge sur la plupart des modèles
- Les deux fournissent les mêmes flux vidéo via différents mécanismes de découverte et configuration

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Honeywell ?**

Pour la plupart des caméras Honeywell actuelles, utilisez `rtsp://admin:password@CAMERA_IP:554/h264`. Pour les modèles multi-canaux, utilisez `rtsp://admin:password@CAMERA_IP:554/cam1/h264`. Les caméras compatibles PSIA répondent également à `/PSIA/Streaming/channels/1`.

**Les caméras Honeywell prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Honeywell Performance Series, 30 Series et 60 Series actuelles prennent en charge ONVIF Profile S (streaming), Profile G (enregistrement) et Profile T (streaming avancé). Les modèles equIP historiques peuvent ne prendre en charge que le profil ONVIF S.

**Qu'est-ce que PSIA sur les caméras Honeywell ?**

PSIA (Physical Security Interoperability Alliance) est une alternative à ONVIF pour l'interopérabilité entre appareils. Honeywell a historiquement pris en charge PSIA en parallèle d'ONVIF. Les flux PSIA utilisent le format d'URL `rtsp://IP:554/PSIA/Streaming/channels/1`.

**Les modèles Honeywell IPCAM sont-ils toujours pris en charge ?**

La série IPCAM grand public est abandonnée. Ces caméras ne prennent en charge que MJPEG/JPEG HTTP et n'ont pas de RTSP. Pour les modèles IPCAM, utilisez l'URL de capture HTTP `http://IP/img/snapshot.cgi?size=3` ou le flux MJPEG `http://IP/img/video.mjpeg`.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Bosch](bosch.md) — Pair sur le segment entreprise / commercial
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
