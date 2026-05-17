---
title: URL RTSP Vivotek — guide de connexion en C# .NET VisioForge
description: Modèles d'URL RTSP pour les caméras Vivotek FD, IP, SD et fisheye FE en C# .NET. Streaming et enregistrement avec le VisioForge Video Capture SDK et ONVIF.
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
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Vivotek en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Vivotek Inc.** est un fabricant taïwanais de solutions de vidéosurveillance réseau dont le siège est à New Taipei City. Fondée en 2000, Vivotek est l'une des principales marques mondiales de caméras IP, largement déployée dans la vidéosurveillance d'entreprise, commerciale, des transports et urbaine. Vivotek est reconnue pour sa large gamme de facteurs de forme, notamment les caméras fisheye, panoramiques, dôme rapide et spécialisées.

**Faits clés :**

- **Gammes de produits :** FD (dôme fixe), IP (box/bullet), IB (bullet), SD (dôme rapide), FE (fisheye), MD (dôme mobile), CC (compacte), VS (serveurs vidéo/encodeurs)
- **Prise en charge des protocoles :** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, MJPEG
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** root / (vide ou défini lors de la configuration) ; modèles hérités : root / root
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265, MJPEG

## Modèles d'URL RTSP

### Modèles actuels

Toutes les caméras Vivotek actuelles utilisent le modèle d'URL `live.sdp` pour le streaming RTSP :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Stream 1 (principal) | `rtsp://IP:554/live.sdp` | Flux principal, H.264/H.265 |
| Stream 2 (sous) | `rtsp://IP:554/live2.sdp` | Sous-flux |
| Stream 3 | `rtsp://IP:554/live3.sdp` | Troisième flux (si pris en charge) |
| Stream 4 | `rtsp://IP:554/live4.sdp` | Quatrième flux (certains modèles) |

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Facteur de forme |
|-------------|----------|-------------|
| FD81xx (dôme fixe) | `rtsp://IP:554/live.sdp` | Dôme fixe |
| FD83xx (dôme fixe) | `rtsp://IP:554/live.sdp` | Dôme fixe |
| FD8134/FD8136 | `rtsp://IP:554/live.sdp` | Mini dôme |
| FD8161/FD8162/FD8166 | `rtsp://IP:554/live.sdp` | Dôme fixe |
| FD8335H | `rtsp://IP:554/live.sdp` | Dôme fixe |
| FD8361/FD8362E/FD8372 | `rtsp://IP:554/live.sdp` | Dôme fixe |
| FE8171V/FE8172V/FE8174 | `rtsp://IP:554/live.sdp` | Fisheye |
| IP7130/IP7131/IP7132 | `rtsp://IP:554/live.sdp` | Caméra box |
| IP7160/IP7161 | `rtsp://IP:554/live.sdp` | Caméra box |
| IP7330/IP7361 | `rtsp://IP:554/live.sdp` | Bullet |
| IP8130/IP8133/IP8152 | `rtsp://IP:554/live.sdp` | Caméra box |
| IP8331/IP8332/IP8335H | `rtsp://IP:554/live.sdp` | Caméra box |
| IP8362/IP8364 | `rtsp://IP:554/live.sdp` | Caméra box |
| SD8362E | `rtsp://IP:554/live.sdp` | Dôme rapide |
| CC8130 | `rtsp://IP:554/live.sdp` | Compacte |
| MD7560/MD8562 | `rtsp://IP:554/live.sdp` | Dôme mobile |

### Modèles hérités

Les anciens modèles Vivotek (séries IP3xxx, IP6xxx, PT3xxx, PZ6xxx) utilisaient le streaming HTTP uniquement :

| Série de modèles | URL | Notes |
|-------------|-----|-------|
| IP3121/IP3122/IP3133/IP3135 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG uniquement |
| IP6127 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG uniquement |
| PT3112/PT3122 | `http://IP/cgi-bin/video.jpg?size=2` | Pan/tilt, JPEG |
| PZ6114/PZ6122 | `http://IP/cgi-bin/video.jpg?size=2` | Pan/zoom, JPEG |

### URL des serveurs vidéo

Les serveurs vidéo Vivotek encodent les flux de caméras analogiques pour le streaming IP :

| Modèle | URL RTSP | Notes |
|-------|----------|-------|
| VS2403 | `rtsp://IP:554/live.sdp` | Serveur vidéo, multicanal |
| VS3100P | `http://IP/cgi-bin/video.jpg?size=2` | Encodeur hérité |
| VS7100 | `rtsp://IP:554/live.sdp` | Serveur vidéo |
| VS8102 | `rtsp://IP:554/live.sdp` | Serveur vidéo |
| VS8401 | `rtsp://IP:554/live.sdp` | Serveur 4 canaux |
| VS8801 | `rtsp://IP:554/live.sdp` | Serveur 8 canaux |

### URL NVR

| Modèle | URL RTSP | Notes |
|-------|----------|-------|
| NVR NR8x01 | `rtsp://IP:554/live.sdp` | Via NVR |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Vivotek avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Vivotek, flux principal
var uri = new Uri("rtsp://192.168.1.50:554/live.sdp");
var username = "root";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/live2.sdp` à la place.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/viewer/video.jpg?resolution=640x480` | Modèles actuels |
| Capture JPEG (canal) | `http://IP/cgi-bin/viewer/video.jpg?channel=1&resolution=640x480` | Multicanal |
| Flux MJPEG | `http://IP/video.mjpg` | MJPEG continu |
| Flux MJPEG (alt) | `http://IP/video2.mjpg` | Deuxième flux |
| MJPEG (paramètres) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | Avec paramètres qualité/fps |
| Capture héritée | `http://IP/cgi-bin/video.jpg` | Anciens modèles |
| Capture héritée (taille) | `http://IP/cgi-bin/video.jpg?size=2` | Anciens modèles, VGA |
| Capture CGI | `http://IP/snapshot.cgi` | Certains modèles |

## Dépannage

### Modèle d'URL cohérent

Contrairement à de nombreuses marques, Vivotek utilise le même modèle d'URL RTSP `live.sdp` sur pratiquement tous ses modèles compatibles RTSP. Si `rtsp://IP:554/live.sdp` ne fonctionne pas, essayez :

- `rtsp://IP:554/live2.sdp` (sous-flux)
- `rtsp://IP:554/live3.sdp` (troisième flux)

### Identifiants par défaut

- **Modèles actuels :** `root` avec mot de passe défini lors de la configuration initiale
- **Modèles hérités :** `root` / (mot de passe vide) ou `root` / `root`
- Certains modèles nécessitent une configuration via l'interface web avant que RTSP ne soit accessible

### Ports non standard sur certains modèles

Certaines caméras Vivotek peuvent utiliser des ports RTSP non standard (par exemple, 1025, 1032) si configurés. Vérifiez l'interface web de la caméra sous Network > RTSP si le port 554 ne répond pas.

### Caméras héritées HTTP uniquement

Les très anciennes caméras Vivotek (séries IP31xx, IP61xx, PT31xx, PZ61xx) ne prennent en charge que les flux HTTP JPEG et MJPEG, pas RTSP. Ces caméras ne peuvent pas utiliser la source RTSP — utilisez plutôt l'intégration de capture HTTP ou MJPEG.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Vivotek ?**

L'URL standard est `rtsp://root:password@CAMERA_IP:554/live.sdp` pour le flux principal. Utilisez `live2.sdp` pour le sous-flux et `live3.sdp` pour le troisième flux. Ce modèle fonctionne sur pratiquement tous les modèles Vivotek compatibles RTSP.

**Les caméras Vivotek prennent-elles en charge H.265 ?**

Oui. Les caméras Vivotek actuelles prennent en charge H.265 (HEVC). Utilisez la même URL `live.sdp` — le codec est configuré dans l'interface web de la caméra, pas dans l'URL.

**Quelle est la différence entre live.sdp et live2.sdp ?**

`live.sdp` est le flux principal (de la plus haute qualité), `live2.sdp` est généralement un sous-flux de résolution plus faible pour une visualisation avec bande passante limitée, et `live3.sdp` est un troisième flux souvent utilisé pour la visualisation mobile.

**Les serveurs vidéo Vivotek prennent-ils en charge RTSP ?**

Oui. Les serveurs vidéo Vivotek actuels (VS2403, VS7100, VS8102, VS8401, VS8801) prennent en charge RTSP en utilisant le même modèle d'URL `live.sdp` que les caméras. Les serveurs hérités (VS3100P) ne prennent en charge que JPEG HTTP.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion GeoVision](geovision.md) — Caméras d'entreprise taïwanaises
- [Guide de connexion ACTi](acti.md) — Caméras professionnelles taïwanaises
- [Capture de caméra IP vers MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Enregistrer les flux Vivotek dans un fichier
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
