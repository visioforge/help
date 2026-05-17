---
title: URL RTSP des caméras IP Foscam en C# .NET — intégration SDK
description: Connectez les caméras Foscam en C# .NET avec modèles d'URL RTSP et HTTP, accès API CGI et exemples pour modèles FI, C1, C2, R2 et R4.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Foscam en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Foscam** (Shenzhen Foscam Intelligent Technology Co., Ltd.) est un fabricant chinois spécialisé dans les caméras IP grand public et petites entreprises. Fondée en 2007 et dont le siège est à Shenzhen, en Chine, Foscam a gagné en popularité avec ses caméras Wi-Fi abordables et a été l'une des premières marques à apporter des caméras IP à bas coût sur le marché grand public.

**Faits clés :**

- **Gammes de produits :** Série FI (pan/tilt historique), C1/C2 (intérieur HD), R2/R4 (pan/tilt intérieur), SD (extérieur), série G (batterie), série VZ (sonnette)
- **Prise en charge des protocoles :** RTSP, HTTP/CGI, ONVIF (modèles plus récents), P2P
- **Port RTSP par défaut :** 88 (pas 554 — c'est unique à Foscam)
- **Port HTTP par défaut :** 88
- **Identifiants par défaut :** admin / (mot de passe vide sur les modèles plus anciens) ; admin / (défini lors de la configuration sur les modèles plus récents)
- **Prise en charge ONVIF :** Partielle (modèles HD plus récents uniquement, par exemple C1, C2, R2, R4)
- **Codecs vidéo :** H.264 (modèles HD), MJPEG (modèles historiques)

## Modèles d'URL RTSP

Les caméras Foscam utilisent un port non standard (88) et des noms de chemins de flux simples.

### Format d'URL

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:88/videoMain
```

!!! warning "Port non standard"
    Les caméras Foscam utilisent généralement le **port 88** à la fois pour RTSP et HTTP, et non le port standard 554. C'est le problème de connexion le plus courant.

### Modèles HD (H.264)

| Série de modèles | URL RTSP | Flux | Audio |
|-------------|----------|--------|-------|
| C1 / C1 Lite (intérieur) | `rtsp://IP:88/videoMain` | Principal (720p) | Oui |
| C1 / C1 Lite (intérieur) | `rtsp://IP:88/videoSub` | Sous-flux (VGA) | Oui |
| C2 (intérieur 1080p) | `rtsp://IP:88/videoMain` | Principal (1080p) | Oui |
| C2 (intérieur 1080p) | `rtsp://IP:88/videoSub` | Sous-flux (VGA) | Oui |
| R2 (pan/tilt 1080p) | `rtsp://IP:88/videoMain` | Principal (1080p) | Oui |
| R4 (pan/tilt 1440p) | `rtsp://IP:88/videoMain` | Principal (2560x1440) | Oui |
| FI9821W V2 (pan/tilt) | `rtsp://IP:88/videoMain` | Principal (720p) | Oui |
| FI9826W (pan/tilt/zoom) | `rtsp://IP:88/videoMain` | Principal (960p) | Oui |
| FI9828P (PTZ extérieur) | `rtsp://IP:88/videoMain` | Principal (960p) | Oui |
| FI9900P (bullet extérieur) | `rtsp://IP:88/videoMain` | Principal (1080p) | Oui |
| SD2 (pan/tilt extérieur) | `rtsp://IP:88/videoMain` | Principal (1080p) | Oui |

### Modèles historiques (MJPEG uniquement)

Les anciens modèles Foscam (FI8904W, FI8910W, FI8918W, FI8919W) ne prennent pas en charge RTSP. Ils utilisent uniquement le streaming HTTP :

| Modèle | URL HTTP | Type | Audio |
|-------|----------|------|-------|
| FI8904W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flux ASF | Oui |
| FI8910W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flux ASF | Oui |
| FI8918W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flux ASF | Oui |
| FI8919W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Flux ASF | Oui |
| FI8904W | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | MJPEG | Non |

### Ports RTSP alternatifs

Certains modèles Foscam peuvent être configurés pour des ports alternatifs :

| Modèle d'URL | Port | Notes |
|-------------|------|-------|
| `rtsp://IP:88/videoMain` | 88 | Par défaut pour la plupart des modèles |
| `rtsp://IP:554/videoMain` | 554 | Si reconfiguré dans les paramètres |
| `rtsp://IP:554/cam1/mpeg4` | 554 | Certaines variantes OEM |
| `rtsp://IP:554/live1.sdp` | 554 | Firmware compatible DCS |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Foscam avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Foscam R2, flux principal -- notez le port 88, et non 554 !
var uri = new Uri("rtsp://192.168.1.30:88/videoMain");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez plutôt `/videoSub`.

## URL de capture instantanée et MJPEG

Foscam fournit une API CGI pour les captures et le contrôle :

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture CGI (HD) | `http://IP:88/cgi-bin/CGIProxy.fcgi?cmd=snapPicture2&usr=USER&pwd=PASS` | Modèles HD |
| Capture historique | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS` | Modèles historiques |
| Capture (count) | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS&count=0` | Image unique |
| Flux MJPEG (historique) | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | MJPEG VGA |
| Flux ASF (historique) | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | Conteneur ASF |
| CGI Video | `http://IP:88/video.cgi?resolution=VGA` | Vidéo directe |

## Dépannage

### Mauvais port — utilisez 88, pas 554

Le problème de connexion Foscam le plus courant est l'utilisation du port 554. Les caméras Foscam utilisent par défaut le **port 88** pour tous les services (RTSP, HTTP et CGI). Si votre connexion expire, vérifiez d'abord le numéro de port.

### Modèles historiques vs HD

Foscam propose deux générations de produits fondamentalement différentes :

- **Historique (FI89xx) :** MJPEG uniquement, streaming HTTP via `videostream.asf` ou `videostream.cgi`, pas de RTSP
- **HD (C1, C2, R2, R4, FI99xx) :** H.264, RTSP via `videoMain`/`videoSub`, prise en charge ONVIF

Si `rtsp://IP:88/videoMain` ne fonctionne pas, votre caméra est probablement un modèle historique — utilisez plutôt les URL de streaming HTTP.

### Mot de passe vide/blanc

Les anciennes caméras Foscam sont livrées avec un mot de passe vide (nom d'utilisateur : `admin`, mot de passe : chaîne vide). Le firmware plus récent nécessite de définir un mot de passe lors de la configuration initiale. Si l'authentification échoue avec un mot de passe, essayez un mot de passe vide pour les modèles historiques.

### Instabilité de la connexion Wi-Fi

Les caméras Foscam Wi-Fi peuvent subir des coupures de flux. Recommandations :

- Utilisez le mode de transport TCP pour la fiabilité
- Positionnez la caméra plus près du routeur Wi-Fi
- Utilisez le Wi-Fi 2,4 GHz (meilleure portée) au lieu de 5 GHz
- Réduisez la résolution du flux vers le sous-flux : `rtsp://IP:88/videoSub`

### ONVIF non disponible

ONVIF n'est pris en charge que sur les modèles HD plus récents (C1, C2, R2, R4, FI99xx). Les caméras FI89xx historiques ne prennent pas en charge ONVIF. Pour les modèles historiques, utilisez plutôt des URL HTTP/RTSP directes.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Foscam ?**

Pour les modèles HD, l'URL est `rtsp://admin:password@CAMERA_IP:88/videoMain`. Notez le port non standard 88 (et non 554). Pour les modèles historiques (série FI89xx), utilisez HTTP : `http://CAMERA_IP:88/videostream.asf?user=admin&pwd=password`.

**Pourquoi Foscam utilise-t-il le port 88 au lieu du 554 standard ?**

Foscam a choisi le port 88 comme valeur par défaut pour tous les services de caméra afin d'éviter les conflits avec d'autres appareils réseau. Vous pouvez modifier cela dans l'interface web de la caméra sous Settings > Network > Port, mais la valeur par défaut est 88.

**Puis-je changer le port RTSP Foscam à 554 ?**

Oui. Accédez à l'interface web de la caméra à `http://CAMERA_IP:88`, allez dans Settings > Network > Port, et changez le port RTSP à 554. Après enregistrement et redémarrage, vous pouvez utiliser le port standard 554 dans vos URL RTSP.

**Foscam prend-il en charge le contrôle pan/tilt/zoom via le SDK ?**

Les modèles PTZ Foscam (R2, R4, FI9821, FI9826) prennent en charge pan/tilt via leur API CGI et ONVIF (modèles HD). Vous pouvez envoyer des commandes PTZ via ONVIF en utilisant les fonctionnalités de contrôle PTZ du SDK VisioForge.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion TP-Link](tp-link.md) — Caméras grand public avec RTSP
- [Guide de connexion D-Link](dlink.md) — Pair sur le segment grand public
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
