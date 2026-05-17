---
title: URL RTSP des caméras IP GeoVision en C# .NET — intégration
description: Modèles d'URL RTSP GeoVision GV-BL, GV-FD, GV-VD, GV-FE et GV-DVR pour C# .NET. Intégrez avec le SDK VisioForge pour applications multi-canaux.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP GeoVision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**GeoVision** (GeoVision Inc.) est un fabricant taïwanais de caméras IP, d'enregistreurs vidéo réseau et de logiciels de gestion vidéo, dont le siège est à Taipei, à Taïwan. GeoVision est une marque bien établie sur le marché de la vidéosurveillance entreprise et professionnelle, connue pour ses caméras IP série GV et la plateforme VMS GeoVision.

**Faits clés :**

- **Gammes de produits :** GV-BL (bullet), GV-FD (dôme fixe), GV-VD (dôme anti-vandalisme), GV-FE (fisheye), GV-CB (cube), GV-CA (caméra), GV-DVR (enregistreur vidéo numérique), GV-NVR
- **Prise en charge des protocoles :** RTSP, ONVIF, PSIA, HTTP/CGI
- **Port RTSP par défaut :** 8554 (caméras IP), 554 (DVR/Serveur)
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (modèles actuels)
- **Codecs vidéo :** H.264, H.265 (modèles actuels), MPEG-4 (historique)

!!! warning "Port RTSP non standard"
    Les caméras IP GeoVision utilisent par défaut le **port 8554**, et non le port standard 554. Veillez à spécifier le bon port lors de la construction de votre URL RTSP. Le logiciel GeoVision DVR/Serveur utilise le port standard 554.

## Modèles d'URL RTSP

### Format standard caméra IP

Les caméras IP GeoVision utilisent un modèle d'URL SDP basé sur canal sur le port 8554 :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:8554//CH001.sdp
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| Port | 8554 | Par défaut pour les caméras IP GeoVision |
| `CH001` | CH001, CH002... | Numéro de canal (3 chiffres avec zéros) |
| `.sdp` | Requis | Suffixe descripteur de session SDP |

!!! note "Double barre"
    Certains modèles GeoVision nécessitent une double barre (`//`) avant l'identifiant de canal. Si une barre unique ne fonctionne pas, essayez `//CH001.sdp`.

### Flux des caméras IP

| Flux | URL | Notes |
|--------|-----|-------|
| Flux principal | `rtsp://IP:8554//CH001.sdp` | Résolution complète, port 8554 |
| Sous-flux | `rtsp://IP:8554//CH002.sdp` | Résolution inférieure |

### DVR / GeoVision Server

Le logiciel GeoVision DVR et GV-Server utilise le port 554 :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/CH001.sdp
```

| Canal | URL du flux principal | Notes |
|---------|----------------|-------|
| Canal 1 | `rtsp://IP:554/CH001.sdp` | Port 554 sur DVR/Serveur |
| Canal 2 | `rtsp://IP:554/CH002.sdp` | Port 554 sur DVR/Serveur |
| Canal N | `rtsp://IP:554/CH00N.sdp` | Compléter à 3 chiffres avec zéros |

### Streaming PSIA

GeoVision prend également en charge les URL RTSP compatibles PSIA :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/PSIA/Streaming/channels/1?videoCodecType=MPEG4
```

### Tableau récapitulatif des URL

| Type d'appareil | URL du flux principal | Port par défaut | Notes |
|-------------|----------------|--------------|-------|
| GV-BL (bullet) | `rtsp://IP:8554//CH001.sdp` | 8554 | Caméra IP standard |
| GV-FD (dôme fixe) | `rtsp://IP:8554//CH001.sdp` | 8554 | Caméra IP standard |
| GV-VD (dôme anti-vandalisme) | `rtsp://IP:8554//CH001.sdp` | 8554 | Caméra IP standard |
| GV-FE (fisheye) | `rtsp://IP:8554//CH001.sdp` | 8554 | Caméra IP standard |
| GV-CB (cube) | `rtsp://IP:8554//CH001.sdp` | 8554 | Caméra IP standard |
| GV-DVR | `rtsp://IP:554/CH001.sdp` | 554 | Logiciel DVR |
| GV-NVR | `rtsp://IP:554/CH001.sdp` | 554 | Logiciel NVR |
| Flux PSIA | `rtsp://IP:554/PSIA/Streaming/channels/1` | 554 | Compatible PSIA |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra GeoVision avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// GeoVision GV-BL2702, flux principal (notez le port 8554)
var uri = new Uri("rtsp://192.168.1.90:8554//CH001.sdp");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `CH002.sdp` au lieu de `CH001.sdp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification basique |
| Capture JPEG (alt) | `http://IP/GetImage.cgi` | Certains modèles |
| Capture JPEG (alt) | `http://IP/cgi-bin/getimage` | Certains modèles |
| Capture JPEG (visualiseur) | `http://IP/cgi-bin/viewer/video.jpg` | Interface visualiseur web |
| Image statique (alt) | `http://IP/cgi-bin/jpg/image.cgi` | Certains modèles |
| Capture historique | `http://IP/cam1.jpg` | Firmware 6.0-8.x, canal 1 |
| Capture historique (canal N) | `http://IP/camN.jpg` | Firmware 6.0-8.x, canal N |

## Dépannage

### Mauvais port — 554 vs 8554

Le problème de connexion le plus courant avec les caméras GeoVision est l'utilisation du mauvais port :

- **Caméras IP** (GV-BL, GV-FD, GV-VD, GV-FE, GV-CB) : Utilisez le **port 8554**
- **Logiciel DVR / GV-Server** : Utilisez le **port 554**

Si vous obtenez une expiration de connexion, vérifiez que vous utilisez le bon port pour votre type d'appareil.

### Double barre dans le chemin d'URL

Certains modèles de caméras IP GeoVision nécessitent une double barre avant l'identifiant de canal (`//CH001.sdp`). Si une barre unique (`/CH001.sdp`) renvoie une erreur, ajoutez la barre supplémentaire.

### Format de numérotation des canaux

GeoVision utilise des numéros de canal à trois chiffres avec zéros : `CH001`, `CH002`, `CH003`, etc. Utiliser `CH1` au lieu de `CH001` ne fonctionnera pas.

### Différences entre versions du firmware

Les anciennes versions du firmware GeoVision (6.x-8.x) peuvent utiliser différents formats d'URL de capture. Si l'URL de capture basée sur CGI ne fonctionne pas, essayez le format historique (`http://IP/cam1.jpg`).

## FAQ

**Quel port utilise GeoVision pour RTSP ?**

Les caméras IP GeoVision utilisent par défaut le **port 8554**, qui diffère du port standard de l'industrie 554. Le logiciel GeoVision DVR et GV-Server utilise le port standard 554.

**Quelle est l'URL RTSP par défaut pour les caméras IP GeoVision ?**

L'URL est `rtsp://admin:password@CAMERA_IP:8554//CH001.sdp` pour le flux principal. Utilisez `CH002.sdp` pour le sous-flux. Notez la double barre avant `CH001` et le port 8554.

**Les caméras GeoVision prennent-elles en charge ONVIF ?**

Oui. Tous les modèles actuels de caméras IP GeoVision prennent en charge ONVIF Profile S. La découverte ONVIF peut être utilisée comme alternative à la configuration manuelle d'URL RTSP.

**Puis-je me connecter à un DVR et à une caméra IP GeoVision en même temps ?**

Oui. Connectez-vous au DVR sur le port 554 et aux caméras IP individuelles sur le port 8554. Chaque appareil a sa propre adresse IP et son propre point de terminaison RTSP.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Vivotek](vivotek.md) — Caméras entreprise taïwanaises
- [Guide de connexion ACTi](acti.md) — Caméras professionnelles taïwanaises
- [Guide d'intégration de caméras RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — Configuration de flux RTSP GeoVision
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
