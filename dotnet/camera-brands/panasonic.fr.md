---
title: Caméra IP Panasonic (i-PRO) en C# .NET — RTSP et ONVIF
description: URL RTSP pour caméras Panasonic i-PRO et héritées WV/BL/BB en C# .NET. Intégration ONVIF avec le SDK VisioForge pour toutes générations.
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

# Comment se connecter à une caméra IP Panasonic (i-PRO) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Panasonic i-PRO** (anciennement Panasonic Security Systems, opérant désormais sous le nom de i-PRO Co., Ltd.) est un fabricant japonais d'équipements de vidéosurveillance professionnels. Faisant à l'origine partie de Panasonic Corporation, la division sécurité a été scindée en **i-PRO** en 2019. Les caméras Panasonic/i-PRO sont largement déployées dans les environnements d'entreprise, gouvernementaux, de transport et de vente au détail à travers le monde.

**Faits clés :**

- **Gammes de produits :** WV-S (série S actuelle), WV-X (série X IA), WV-SF/SC/SP/SW (génération intermédiaire), WV-NP/NS/NW (professionnel hérité), BL (grand public/PME), BB/KX-HCM (grand public hérité)
- **Protocoles pris en charge :** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, MJPEG, Panasonic propriétaire
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 12345 (les modèles actuels exigent le changement de mot de passe à la première connexion) ; les modèles BB/BL hérités n'avaient souvent pas de mot de passe par défaut
- **Prise en charge ONVIF :** Oui (tous les modèles actuels WV-S/WV-X)
- **Codecs vidéo :** H.264, H.265 (modèles actuels), MPEG-4 (hérité), MJPEG

## Modèles d'URL RTSP

### Modèles actuels (séries WV-S/WV-X, i-PRO)

Les caméras Panasonic i-PRO actuelles utilisent le format d'URL `MediaInput` :

| Flux | URL RTSP | Codec | Notes |
|------|----------|-------|-------|
| Flux H.264 | `rtsp://IP:554/MediaInput/h264` | H.264 | Flux RTSP principal |
| Flux H.265 | `rtsp://IP:554/MediaInput/h265` | H.265 | Modèles actuels uniquement |
| Flux MPEG-4 | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | Solution de repli héritée |
| Flux ONVIF | `rtsp://IP//ONVIF/MediaInput` | H.264 | Compatible ONVIF (notez la double barre oblique) |

!!! warning "Double barre oblique dans les URL ONVIF"
    Les URL ONVIF Panasonic utilisent une double barre oblique avant `ONVIF` : `rtsp://IP//ONVIF/MediaInput`. Cela est intentionnel et requis pour les connexions basées sur ONVIF.

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Génération |
|------------------|----------|------------|
| WV-S1131/S1132 | `rtsp://IP:554/MediaInput/h264` | Actuelle (i-PRO) |
| WV-S2131L/S2231L | `rtsp://IP:554/MediaInput/h264` | Actuelle (i-PRO) |
| WV-X1551L/X2251L | `rtsp://IP:554/MediaInput/h264` | Série IA actuelle |
| WV-SF132/SF135/SF138 | `rtsp://IP:554/MediaInput/h264` | Génération intermédiaire |
| WV-SF332/SF335/SF346 | `rtsp://IP:554/MediaInput/h264` | Génération intermédiaire |
| WV-SC384/SC385/SC386 | `rtsp://IP:554/MediaInput/h264` | Génération intermédiaire |
| WV-SP105/SP306/SP508 | `rtsp://IP:554/MediaInput/h264` | Génération intermédiaire |
| WV-SW115/SW155/SW175 | `rtsp://IP:554/MediaInput/h264` | Extérieur génération intermédiaire |
| WV-SW316/SW352/SW355 | `rtsp://IP:554/MediaInput/h264` | Extérieur génération intermédiaire |
| WV-SW395/SW396/SW458 | `rtsp://IP:554/MediaInput/h264` | Extérieur génération intermédiaire |
| WV-SW558/SW559/SW598 | `rtsp://IP:554/MediaInput/h264` | Extérieur génération intermédiaire |
| WV-ST162/ST165 | `rtsp://IP:554/MediaInput/h264` | PTZ génération intermédiaire |
| WV-NP240/NP244/NP304 | `rtsp://IP:554/MediaInput/mpeg4` | Professionnel hérité |
| WV-NP502/NP1000/NP1004 | `rtsp://IP:554/MediaInput/mpeg4` | Professionnel hérité |
| WV-NS202/NS324/NS954 | `rtsp://IP:554/MediaInput/mpeg4` | PTZ hérité |
| WV-NW484/NW502/NW960/NW964 | `rtsp://IP:554/MediaInput/mpeg4` | Extérieur hérité |

### Modèles grand public hérités (séries BB/BL/KX)

Les caméras grand public Panasonic plus anciennes utilisaient des modèles d'URL différents :

| Série de modèles | URL RTSP | Codec | Notes |
|------------------|----------|-------|-------|
| BL-C210/C230 | `rtsp://IP:554/MediaInput/h264` | H.264 | Modèles grand public tardifs |
| BL-C210/C230 | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | Solution de repli MPEG-4 |
| BL-VP101/VP104 | `rtsp://IP:554/MediaInput/h264` | H.264 | Compact |
| BB-HCM531A/735 | `rtsp://IP/nphMpeg4/g726-640x48` | MPEG-4 | Format très ancien |
| BB/BL/KX (HTTP uniquement) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | MJPEG | Flux MJPEG HTTP |

!!! info "Caméras BB/BL héritées"
    De nombreuses caméras Panasonic plus anciennes des séries BB et BL ne prennent pas du tout en charge RTSP. Elles ne fournissent que des flux MJPEG et JPEG snapshot basés sur HTTP. Les caméras i-PRO actuelles prennent pleinement en charge RTSP.

### URL encodeur/DVR

| Périphérique | URL RTSP | Notes |
|--------------|----------|-------|
| Encodeur WJ-GXE500 | `http://IP/cgi-bin/camera` | MJPEG via HTTP |
| DVR WJ-HD220 | `http://IP/cgi-bin/jpeg` | Snapshot depuis DVR |
| NVR WJ-ND400 | `http://IP/cgi-bin/jpeg` | Snapshot depuis NVR |
| NVR WJ-NV200 | `http://IP/cgi-bin/checkimage.cgi?UID=USER&CAM=CHANNEL` | Snapshot de canal |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Panasonic avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Panasonic i-PRO, flux H.264
var uri = new Uri("rtsp://192.168.1.80:554/MediaInput/h264");
var username = "admin";
var password = "YourPassword";
```

Pour H.265, utilisez `/MediaInput/h265` à la place.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Flux MJPEG (actuel) | `http://IP/cgi-bin/mjpeg?stream=1` | Modèles WV-S/WV-X actuels |
| Snapshot JPEG | `http://IP/cgi-bin/camera` | Modèles actuels |
| Snapshot (taille) | `http://IP/SnapshotJPEG?Resolution=640x480` | Modèles génération intermédiaire |
| Snapshot (qualité) | `http://IP/SnapShotJPEG?Resolution=320x240&Quality=Motion` | Modèles hérités |
| Flux MJPEG (hérité) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | Modèles BB/BL/KX |
| Server push | `http://IP/cgi-bin/nphContinuousServerPush` | Push JPEG continu |

## Dépannage

### Confusion sur le nom de marque

La marque de caméras de sécurité Panasonic a évolué :

- **Panasonic** (avant 2019) : marque Panasonic complète
- **i-PRO** (2019 à aujourd'hui) : scindée de Panasonic en tant qu'i-PRO Co., Ltd.
- Les produits actuels portent la marque **i-PRO** mais de nombreux utilisateurs recherchent encore « caméra Panasonic »

Toutes utilisent des modèles d'URL RTSP compatibles au sein de leur génération.

### MediaInput/h264 contre ONVIF/MediaInput

- Utilisez `rtsp://IP:554/MediaInput/h264` pour les connexions RTSP directes (recommandé)
- Utilisez `rtsp://IP//ONVIF/MediaInput` pour les connexions compatibles ONVIF (notez la double barre oblique)
- Les deux fournissent le même flux vidéo mais utilisent différents mécanismes d'authentification

### Caméras héritées sans RTSP

De nombreuses caméras Panasonic plus anciennes des séries BB et BL (notamment BL-C1, BL-C10, BL-C30, BL-C101, BL-C111, BL-C131 et antérieures) ne prennent pas en charge RTSP. Ces caméras ne fournissent que :

- MJPEG HTTP : `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard`
- Snapshot HTTP : `http://IP/SnapshotJPEG?Resolution=320x240`

### MPEG-4 contre H.264

Les caméras héritées des séries WV-NP/NS/NW peuvent ne prendre en charge que MPEG-4 sur RTSP. Essayez `MediaInput/mpeg4` si `MediaInput/h264` échoue sur les anciens modèles.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Panasonic/i-PRO ?**

Pour les caméras i-PRO actuelles, utilisez `rtsp://admin:password@CAMERA_IP:554/MediaInput/h264`. Pour les connexions ONVIF, utilisez `rtsp://CAMERA_IP//ONVIF/MediaInput`. Les modèles hérités peuvent nécessiter `MediaInput/mpeg4`.

**Panasonic est-il la même chose qu'i-PRO ?**

Oui. La division des caméras de sécurité de Panasonic a été scindée en i-PRO Co., Ltd. en 2019. Les caméras actuelles portent la marque i-PRO, mais utilisent les mêmes modèles d'URL RTSP que les caméras Panasonic série WV de génération tardive.

**Les caméras Panasonic prennent-elles en charge H.265 ?**

Les caméras i-PRO actuelles (séries WV-S et WV-X) prennent en charge H.265. Utilisez `rtsp://IP:554/MediaInput/h265` pour le flux H.265. Les modèles de génération intermédiaire et plus anciens ne prennent en charge que H.264 et MPEG-4.

**Puis-je me connecter aux caméras Panasonic BB/BL héritées ?**

De nombreuses anciennes caméras des séries BB et BL ne prennent pas en charge RTSP et ne fournissent que des flux MJPEG HTTP. Utilisez l'URL MJPEG HTTP `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` avec une source HTTP au lieu de RTSP.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Sanyo](sanyo.md) — Acquise par Panasonic, gamme de produits prédécesseur
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
