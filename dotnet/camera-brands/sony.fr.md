---
title: Modèles d'URL RTSP Sony SNC et configuration C# .NET
description: Modèles d'URL RTSP pour les caméras Sony SNC CH, DH, EB, CX et IPELA en C# .NET. Streaming et enregistrement avec le VisioForge Video Capture SDK.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Sony en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Sony** (Sony Corporation, Security Systems Division) était un fabricant majeur de caméras IP de vidéosurveillance professionnelles sous la marque **IPELA** puis la gamme de produits **SNC** (Sony Network Camera). Sony s'est retiré du marché des caméras de sécurité en 2020, en vendant son activité de sécurité à **Bosch**. Cependant, une large base installée de caméras Sony reste en service dans le monde, en particulier dans les installations d'entreprise et gouvernementales.

**Faits clés :**

- **Gammes de produits :** SNC-CH (box, H.264), SNC-DH (dôme, H.264), SNC-EB/ER (série E), SNC-CX (compacte), SNC-VB/VM/WR/XM (génération actuelle avant retrait), SNC-DF/RX/RZ/CS (IPELA hérité), SNT (encodeurs vidéo)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI, Sony propriétaire (DEPA)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (doit être modifié lors de la configuration)
- **Prise en charge ONVIF :** Oui (SNC-CH/DH et modèles plus récents)
- **Codecs vidéo :** H.264, H.265 (modèles tardifs), MPEG-4 (hérité), MJPEG
- **Statut :** Sony a quitté le marché des caméras de sécurité en 2020

!!! warning "Fin de vie"
    Sony s'est retiré du marché des caméras de sécurité en 2020. Bien que les caméras existantes continuent de fonctionner, aucune nouvelle mise à jour de firmware ni nouveau modèle ne sont publiés. Les URL RTSP documentées ici restent valides pour les installations existantes.

## Modèles d'URL RTSP

### Modèles de génération actuelle (SNC-CH/DH/EB/ER/CX/VB/VM/WR/XM)

| Flux | URL RTSP | Codec | Notes |
|--------|----------|-------|-------|
| Video 1 (principal) | `rtsp://IP:554/media/video1` | H.264 | Flux principal |
| Video 2 (sous) | `rtsp://IP:554/media/video2` | H.264 | Sous-flux |
| Profil ONVIF | `rtsp://IP//profile` | H.264 | Basé sur ONVIF (notez la double barre) |
| Direct | `rtsp://IP//media/video1` | H.264 | Alternative (double barre) |

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Résolution | Notes |
|-------------|----------|------------|-------|
| SNC-CH110 | `rtsp://IP/media/video1` | 1280x1024 | Caméra box |
| SNC-CH120/CH140 | `rtsp://IP/media/video1` | 1280x1024 / 1920x1080 | Caméra box |
| SNC-CH160/CH180 | `rtsp://IP/media/video1` | 1920x1080 | Caméra box |
| SNC-CH210/CH260/CH280 | `rtsp://IP/media/video1` | 1920x1080 / 2MP | Caméra box |
| SNC-DH110/DH120/DH140 | `rtsp://IP/media/video1` | Jusqu'à 1080p | Dôme fixe |
| SNC-DH160/DH180 | `rtsp://IP/media/video1` | 1920x1080 | Dôme fixe |
| SNC-DH210/DH260 | `rtsp://IP/media/video1` | 1920x1080 | Dôme fixe |
| SNC-EB600B | `rtsp://IP/media/video1` | 1080p | Série E |
| SNC-CX600W | `rtsp://IP:554//media/video1` | 1080p | Compact |
| SNC-VB630/WR630/XM632 | `rtsp://IP//profile` | 1080p+ | Dernière génération |
| SNC-DM110 | `rtsp://IP:554//media/video1` | 720p | Mini dôme |

### Modèles IPELA hérités (SNC-RX/RZ/DF/CS/EP)

Les anciennes caméras Sony IPELA ne prennent généralement pas en charge RTSP et utilisent le streaming HTTP :

| Série de modèles | URL | Notes |
|-------------|-----|-------|
| SNC-RX530/RX550 | `http://IP/jpeg/vga.jpg` | Capture JPEG |
| SNC-RZ25/RZ30/RZ50 | `http://IP/oneshotimage.jpg` | Capture JPEG |
| SNC-DF40/DF50/DF70/DF80 | `http://IP/image` | Capture JPEG |
| SNC-CS11/CS3P/CS50P | `http://IP/oneshotimage.jpg` | Capture JPEG |
| SNC-EP520/EP580 | `http://IP/jpeg/vga.jpg` | Capture JPEG |
| SNC-M1/M3 | `http://IP/image` | Très ancien MPEG-4 |

### URL d'encodeur vidéo

| Encodeur | URL | Notes |
|---------|-----|-------|
| SNT-EX101/EX104 | `http://IP/oneshotimage.jpg` | Capture par canal |
| SNT-EX104 (canal) | `http://IP/CH1/oneshotimage.jpg` | Spécifique au canal |
| SNT-V704 | `http://IP/CH1/oneshotimage.jpg` | Encodeur 4 canaux |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Sony avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Sony SNC, flux principal
var uri = new Uri("rtsp://192.168.1.55:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/media/video2` à la place.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/oneshotimage.jpg` | La plupart des modèles SNC |
| JPEG (VGA) | `http://IP/jpeg/vga.jpg` | Résolution VGA |
| JPEG (QVGA) | `http://IP/jpeg/qvga.jpg` | Résolution QVGA |
| Flux MJPEG | `http://IP/img/mjpeg.cgi` | MJPEG continu |
| MJPEG (alt) | `http://IP/mjpeg` | Chemin MJPEG alternatif |
| H.264 sur HTTP | `http://IP/h264` | Flux H.264 via HTTP |
| Image | `http://IP/image` | Capture générique |
| Capture de canal | `http://IP/oneshotimage1` | Spécifique au canal |

## Dépannage

### Double barre dans les URL

Certains modèles Sony utilisent une double barre avant le chemin dans les URL RTSP :

- `rtsp://IP//media/video1` (double barre)
- `rtsp://IP:554/media/video1` (simple barre avec port)

Les deux formats fonctionnent généralement, mais essayez la variante à double barre si l'URL standard échoue.

### ONVIF vs RTSP direct

Les caméras Sony prennent en charge à la fois les connexions RTSP directes et basées sur ONVIF :

- RTSP direct : `rtsp://IP:554/media/video1` (recommandé)
- ONVIF : `rtsp://IP//profile` (URL découverte via ONVIF)

### Caméras héritées sans RTSP

Les anciennes caméras Sony IPELA (séries SNC-RX, SNC-RZ, SNC-DF, SNC-CS, SNC-M) ne prennent souvent pas en charge RTSP et n'offrent que JPEG/MJPEG sur HTTP. Pour ces caméras, utilisez l'intégration de capture HTTP.

### Sony s'est retiré du marché

Sony a vendu son activité de caméras de sécurité en 2020. Les caméras existantes continuent de fonctionner mais ne reçoivent aucune nouvelle mise à jour de firmware. Prévoyez une migration éventuelle lors du déploiement de nouvelles intégrations.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Sony SNC ?**

Pour les caméras Sony SNC actuelles, utilisez `rtsp://admin:password@CAMERA_IP:554/media/video1` pour le flux principal et `media/video2` pour le sous-flux.

**Sony fabrique-t-il encore des caméras IP ?**

Non. Sony s'est retiré du marché des caméras de sécurité en 2020. Les caméras Sony SNC existantes restent en service et leurs flux RTSP continuent de fonctionner, mais aucun nouveau modèle ni mise à jour de firmware n'est publié.

**Les caméras Sony prennent-elles en charge ONVIF ?**

Oui. Les séries Sony SNC-CH, SNC-DH et plus récentes prennent en charge ONVIF Profile S. Utilisez `rtsp://IP//profile` pour les connexions basées sur ONVIF.

**Qu'en est-il des caméras Sony IPELA ?**

IPELA était l'ancienne marque de caméra de Sony. De nombreux modèles IPELA (séries SNC-RX, SNC-RZ, SNC-DF) ne prennent en charge que JPEG/MJPEG sur HTTP, pas RTSP. Les modèles IPELA plus tardifs (séries SNC-CH/DH) prennent en charge RTSP via `media/video1`.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Canon](canon.md) — Caméras d'entreprise japonaises
- [Guide de connexion Axis](axis.md) — Homologue de vidéosurveillance d'entreprise
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
