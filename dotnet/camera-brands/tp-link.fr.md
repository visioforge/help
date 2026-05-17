---
title: Guide URL RTSP TP-Link et Tapo pour C# .NET — caméras IP
description: Modèles d'URL RTSP pour caméras TP-Link et Tapo série C en C# .NET. Intégrez les modèles TL-SC, NC et Tapo avec le VisioForge Video Capture SDK.
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
  - MP4
  - H.264
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP TP-Link en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**TP-Link** est un fabricant mondial d'équipements réseau dont le siège est à Shenzhen, en Chine. Bien que principalement connue pour ses routeurs et son matériel réseau, TP-Link produit des caméras IP sous les marques **TP-Link** (série TL-SC, maintenant discontinuée) et **Tapo** (marque domotique avec la série Tapo C, actuellement active). La gamme Tapo est devenue l'une des marques de caméras grand public les plus vendues au monde grâce à des prix agressifs et à une configuration basée sur une application.

**Faits clés :**

- **Gammes de produits :** Série TL-SC (héritée, discontinuée), série NC (caméras cloud, discontinuées), série Tapo C (caméras domotiques actuelles)
- **Prise en charge des protocoles :** RTSP, HTTP/MJPEG, ONVIF (modèles Tapo avec mise à jour du firmware), protocole cloud propriétaire
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Varient selon la génération (voir ci-dessous)
- **Prise en charge ONVIF :** Série Tapo C (nécessite une activation dans l'application Tapo) ; la série TL-SC n'a pas d'ONVIF
- **Codecs vidéo :** H.264 (tous les modèles), H.265 (Tapo C320WS et plus récents)

### Identifiants par gamme de produits

| Gamme de produits | Nom d'utilisateur par défaut | Mot de passe par défaut | Notes |
|-------------|-----------------|-----------------|-------|
| Série TL-SC | admin | admin | Héritée, fixe |
| Série NC | admin | admin | Gérée dans le cloud |
| Série Tapo C | (défini dans l'application) | (défini dans l'application) | Il faut créer des identifiants RTSP dans l'application Tapo |

!!! info "Identifiants des caméras Tapo"
    Les caméras Tapo nécessitent que vous créiez un **compte caméra** séparé dans l'application Tapo (Advanced Settings > Camera Account) avant que l'accès RTSP ne fonctionne. Ce nom d'utilisateur/mot de passe est différent de votre compte cloud TP-Link.

## Modèles d'URL RTSP

### Série Tapo C (modèles actuels)

La gamme de caméras Tapo utilise un format d'URL RTSP simple :

| Modèle | URL RTSP | Flux | Audio |
|-------|----------|--------|-------|
| Tapo C100 (intérieur) | `rtsp://IP:554/stream1` | Principal (1080p) | Oui |
| Tapo C100 (intérieur) | `rtsp://IP:554/stream2` | Sous (360p) | Oui |
| Tapo C110 (intérieur 3MP) | `rtsp://IP:554/stream1` | Principal (2304x1296) | Oui |
| Tapo C110 (intérieur 3MP) | `rtsp://IP:554/stream2` | Sous | Oui |
| Tapo C200 (pan/tilt) | `rtsp://IP:554/stream1` | Principal (1080p) | Oui |
| Tapo C200 (pan/tilt) | `rtsp://IP:554/stream2` | Sous (360p) | Oui |
| Tapo C210 (pan/tilt 3MP) | `rtsp://IP:554/stream1` | Principal (2304x1296) | Oui |
| Tapo C310 (extérieur) | `rtsp://IP:554/stream1` | Principal (2048x1296) | Oui |
| Tapo C320WS (extérieur 2K) | `rtsp://IP:554/stream1` | Principal (2560x1440) | Oui |
| Tapo C500 (PTZ extérieur) | `rtsp://IP:554/stream1` | Principal (1080p) | Oui |
| Tapo C520WS (PTZ extérieur 2K) | `rtsp://IP:554/stream1` | Principal (2560x1440) | Oui |

### Série TL-SC (modèles hérités)

La série TL-SC discontinuée utilisait différents formats d'URL selon le modèle :

| Modèle | URL RTSP | Codec | Audio |
|-------|----------|-------|-------|
| TL-SC3130 | `rtsp://IP:554/video.mp4` | MPEG-4 | Oui |
| TL-SC3130G | `rtsp://IP:554/video.mp4` | MPEG-4 | Oui |
| TL-SC3171 | `rtsp://IP:554/video.mp4` | MPEG-4 | Oui |
| TL-SC3171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Oui |
| TL-SC3230 | `rtsp://IP:554/video.h264` | H.264 | Oui |
| TL-SC3230N | `rtsp://IP:554/video.h264` | H.264 | Oui |
| TL-SC3430 | `rtsp://IP:554/video.h264` | H.264 | Oui |
| TL-SC3430N | `rtsp://IP:554/video.h264` | H.264 | Oui |
| TL-SC4171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Oui |

### Formats d'URL alternatifs TL-SC

| Modèle d'URL | Codec | Notes |
|-------------|-------|-------|
| `rtsp://IP:554/video.mp4` | MPEG-4 | Principal pour les modèles SC3xxx |
| `rtsp://IP:554/video.h264` | H.264 | Principal pour les modèles SC plus récents |
| `rtsp://IP:554/video.mjpg` | MJPEG | Qualité inférieure, compatibilité plus large |
| `rtsp://IP:554/video.pro2` | MPEG-4 | Profil alternatif |
| `rtsp://IP:554/live.sdp` | H.264 | Flux basé sur SDP |
| `rtsp://IP:554/cam1/h264` | H.264 | Format basé sur le canal |
| `rtsp://IP:554/media.amp` | Auto | Firmware compatible Axis |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra TP-Link avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// TP-Link Tapo C200, flux principal
var uri = new Uri("rtsp://192.168.1.100:554/stream1");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/stream2` à la place.

## URL de capture instantanée et MJPEG

### Série Tapo C

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture | `http://IP/snapshot.jpg` | Peut nécessiter une authentification |

### Série TL-SC

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/jpg/image.jpg` | Capture basique |
| Capture redimensionnée | `http://IP/jpg/image.jpg?size=3` | Taille prédéfinie |
| Capture CGI | `http://IP/cgi-bin/jpg/image` | Basée sur CGI |
| Flux MJPEG | `http://IP/video.mjpg` | MJPEG continu |
| MJPEG (qualité) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | Contrôle qualité/FPS |
| Video CGI | `http://IP/video.cgi?resolution=VGA` | Spécifique à la résolution |
| Net Video CGI | `http://IP/cgi-bin/net_video.cgi?channel=1` | Basé sur le canal |
| Compatible Axis | `http://IP/axis-cgi/mjpg/video.cgi` | API Axis émulée |

## Dépannage

### Caméra Tapo : « Connection refused » ou « Unauthorized »

Le problème le plus courant avec les caméras Tapo est l'absence de configuration des identifiants RTSP :

1. Ouvrez l'**application Tapo** sur votre téléphone
2. Accédez aux paramètres de votre caméra
3. Naviguez vers **Advanced Settings > Camera Account**
4. Créez un nom d'utilisateur et un mot de passe
5. Utilisez ces identifiants (pas votre compte TP-Link) dans les URL RTSP

### Caméra Tapo : ONVIF ne fonctionne pas

ONVIF est désactivé par défaut sur les caméras Tapo. Pour l'activer :

1. Ouvrez l'application Tapo
2. Accédez aux paramètres de la caméra > Advanced Settings
3. Activez le commutateur **ONVIF**
4. La caméra redémarrera

### Modèles TL-SC : mauvaise URL de codec

Les caméras TL-SC sont spécifiques au codec dans leurs URL :

- **Séries SC3130/3171 :** Utilisez `/video.mp4` (MPEG-4)
- **Séries SC3230/3430 :** Utilisez `/video.h264` (H.264)
- L'utilisation du mauvais codec dans le chemin d'URL n'entraînera aucun flux

### Stream2 sur les caméras Tapo affiche une résolution faible

C'est par conception. `stream2` est le sous-flux destiné à une bande passante plus faible. Utilisez `stream1` pour la résolution complète. Vous pouvez ajuster la résolution du sous-flux dans l'application Tapo sous les paramètres de la caméra.

### Modèles TL-SC : videostream.asf ne fonctionne pas

Le format d'URL `videostream.asf` nécessite des identifiants intégrés dans l'URL :
`http://IP/videostream.asf?user=admin&pwd=admin&resolution=64&rate=0`

Les valeurs du paramètre `resolution` : 32 = 320x240, 64 = 640x480.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Tapo ?**

L'URL est `rtsp://username:password@CAMERA_IP:554/stream1` pour le flux principal et `stream2` pour le sous-flux. Vous devez d'abord créer des identifiants RTSP dans l'application Tapo sous Advanced Settings > Camera Account.

**Puis-je utiliser les caméras Tapo sans le service cloud Tapo ?**

Oui. Une fois que vous avez configuré les identifiants RTSP via l'application Tapo, vous pouvez accéder au flux RTSP de la caméra directement sur votre réseau local sans aucune dépendance au cloud. L'application Tapo n'est nécessaire que pour la configuration initiale et la configuration des identifiants.

**Quelle est la différence entre les caméras TL-SC et Tapo ?**

La série TL-SC était l'ancienne gamme de caméras IP TP-Link (discontinuée) avec une gestion traditionnelle basée sur le web. Tapo est la marque actuelle de caméras domotiques avec une configuration basée sur une application. Les deux prennent en charge RTSP mais utilisent des modèles d'URL et des méthodes d'authentification différents.

**Les caméras Tapo prennent-elles en charge H.265 ?**

Certains modèles comme le Tapo C320WS et le C520WS prennent en charge l'encodage H.265. La plupart des caméras Tapo utilisent H.264. Vérifiez les spécifications de votre modèle spécifique pour la prise en charge H.265.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Reolink](reolink.md) — Alternative grand public avec RTSP
- [Guide de connexion Mercusys](mercusys.md) — Sous-marque TP-Link, même firmware
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
