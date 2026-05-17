---
title: URL RTSP des caméras IP AVTech et guide de connexion C# .NET
description: Guide d'intégration des caméras IP AVTech pour C# .NET avec modèles d'URL RTSP, URL des canaux DVR/NVR et exemples pour AVM, AVN, AVC et AVI.
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
  - AVI
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP AVTech en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**AVTech** (AVTech Corporation) est un fabricant taïwanais d'équipements de vidéosurveillance basé à Taipei, à Taïwan, fondé en 1996. AVTech est l'un des plus grands fabricants de DVR/NVR au monde, avec une forte présence sur les marchés Asie-Pacifique, Moyen-Orient et Amérique latine. La société produit une large gamme de caméras IP, DVR, NVR, ainsi que la plateforme de visualisation mobile EagleEyes. AVTech est connu pour proposer des solutions de vidéosurveillance économiques avec une large compatibilité de modèles.

**Faits clés :**

- **Gammes de produits :** AVM (caméras IP), AVN (caméras réseau), AVC (DVR), AVI (caméras spécialisées), EagleEyes (application mobile)
- **Prise en charge des protocoles :** RTSP, ONVIF (modèles plus récents), HTTP/CGI, MJPEG
- **Port RTSP par défaut :** 554 (certains modèles utilisent le port 88)
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (modèles plus récents)
- **Codecs vidéo :** H.264, MPEG-4, MJPEG
- **Accès invité :** De nombreux modèles autorisent les captures JPEG sans authentification via un CGI invité

!!! note "Certains modèles AVTech utilisent le port 88"
    Certains modèles AVTech plus récents utilisent le port 88 au lieu de 554 pour RTSP. Si le port 554 ne fonctionne pas, essayez le port 88 avec le modèle d'URL `rtsp://IP:88//live/h264_ulaw/VGA`.

!!! warning "Sécurité de l'accès invité"
    De nombreuses caméras AVTech exposent un point de terminaison CGI invité (`/cgi-bin/guest/Video.cgi`) qui permet un accès aux captures sans authentification. Assurez-vous que les paramètres d'accès invité de votre caméra sont configurés de manière sécurisée.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras AVTech utilisent le modèle d'URL basé sur le chemin `/live/` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live/h264
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `/live/h264` | Flux H.264 | Flux vidéo H.264 principal |
| `/live/mpeg4` | Flux MPEG-4 | Flux vidéo MPEG-4 historique |
| `/live/h264/ch[N]` | Canal N | Flux spécifique au canal pour DVR/NVR |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| AVM217 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM328 | Dôme IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM357 | Dôme IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM457 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM459 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM552 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM561 | Dôme IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVM571 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN211 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN252 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN257 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN304 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN314 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN362 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN801 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN812 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVN813 | Caméra réseau | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVI201 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |
| AVI203 | Caméra IP | `rtsp://IP:554/live/h264` | Flux principal H.264 |

### URL des canaux DVR/NVR

Pour les DVR et NVR AVTech (série AVC et autres) :

| Canal | Flux principal (H.264) | Flux principal (MPEG-4) |
|---------|---------------------|----------------------|
| Canal 1 | `rtsp://IP:554/live/h264/ch1` | `rtsp://IP:554/live/mpeg4/ch1` |
| Canal 2 | `rtsp://IP:554/live/h264/ch2` | `rtsp://IP:554/live/mpeg4/ch2` |
| Canal 3 | `rtsp://IP:554/live/h264/ch3` | `rtsp://IP:554/live/mpeg4/ch3` |
| Canal N | `rtsp://IP:554/live/h264/chN` | `rtsp://IP:554/live/mpeg4/chN` |

### Formats d'URL alternatifs

Certains modèles AVTech, en particulier les plus récents, utilisent le port 88 et différents formats de chemin :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/live/h264` | H.264 standard (recommandé) |
| `rtsp://IP:554/live/mpeg4` | Flux MPEG-4 |
| `rtsp://IP//live/h264` | Sans port explicite (certains modèles) |
| `rtsp://IP:88//live/h264_ulaw/VGA` | Port 88, avec audio, résolution VGA |
| `rtsp://IP:88//live/video_audio/profile1` | Port 88 avec sélection de profil |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra AVTech avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// AVTech AVM552, flux principal H.264
var uri = new Uri("rtsp://192.168.1.80:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au flux MPEG-4, utilisez `/live/mpeg4` au lieu de `/live/h264`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG (invité) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG` | Aucune authentification requise (si accès invité activé) |
| Capture JPEG (canal) | `http://IP/cgi-bin/guest/Video.cgi?media=JPEG&channel=CHANNEL` | Capture spécifique au canal |
| Flux MJPEG en direct | `http://IP/live/mjpeg` | Flux MJPEG continu |

## Dépannage

### Erreur « 401 Unauthorized »

Si vous recevez une erreur d'authentification :

1. Vérifiez vos identifiants — la valeur par défaut est admin / admin
2. Accédez à la caméra à `http://CAMERA_IP` dans un navigateur pour confirmer que la connexion fonctionne
3. Assurez-vous que RTSP est activé dans les paramètres réseau de la caméra
4. Essayez d'inclure les identifiants dans l'URL : `rtsp://admin:password@IP:554/live/h264`

### Port 554 vs port 88

Certains modèles AVTech plus récents utilisent le port 88 au lieu du port RTSP standard 554. Si vous ne pouvez pas vous connecter sur le port 554 :

1. Essayez le port 88 : `rtsp://IP:88//live/h264_ulaw/VGA`
2. Notez la double barre (`//`) dans certains modèles d'URL du port 88
3. Vérifiez l'interface web de la caméra sous les paramètres réseau pour le port RTSP configuré

### MPEG-4 vs H.264

Les modèles AVTech plus anciens peuvent ne prendre en charge que MPEG-4. Si l'URL du flux H.264 ne fonctionne pas :

- Essayez plutôt `rtsp://IP:554/live/mpeg4`
- Vérifiez les paramètres d'encodage de la caméra dans l'interface web
- Les modèles plus récents prennent en charge H.264 ; les modèles plus anciens peuvent être MPEG-4 uniquement

### Double barre dans l'URL

Certains modèles d'URL AVTech incluent une double barre (`//`) après l'IP ou le port. Cela est intentionnel et requis par certaines versions du firmware. Si une URL à barre unique ne fonctionne pas, essayez la variante à double barre.

### Application mobile EagleEyes

L'application EagleEyes est la plateforme de visualisation mobile d'AVTech. L'accès RTSP fonctionne indépendamment d'EagleEyes et ne nécessite pas que l'application soit configurée.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras AVTech ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/live/h264` pour le flux H.264 principal. Pour les DVR/NVR, ajoutez le numéro de canal : `rtsp://IP:554/live/h264/ch1` pour le canal 1.

**Les caméras AVTech prennent-elles en charge ONVIF ?**

Les modèles AVTech plus récents prennent en charge ONVIF. Les modèles plus anciens peuvent ne pas avoir de prise en charge ONVIF et reposent sur des protocoles propriétaires et RTSP pour l'intégration.

**Quelle est la différence entre les séries AVM et AVN ?**

La série AVM regroupe des caméras IP conçues pour une connexion réseau directe, tandis que la série AVN regroupe des caméras réseau pouvant inclure des fonctionnalités supplémentaires comme le Wi-Fi intégré ou l'audio. Les deux séries utilisent le même format d'URL RTSP.

**Puis-je accéder aux captures AVTech sans authentification ?**

De nombreuses caméras AVTech disposent d'un point de terminaison CGI invité (`/cgi-bin/guest/Video.cgi?media=JPEG`) qui permet un accès aux captures JPEG sans authentification. C'est un problème de sécurité si votre caméra est accessible sur le réseau. Vérifiez les paramètres d'accès invité de votre caméra et désactivez-le s'il n'est pas nécessaire.

**Pourquoi certaines URL AVTech utilisent-elles le port 88 ?**

Certaines versions plus récentes du firmware AVTech utilisent par défaut le port 88 pour RTSP au lieu du port standard 554. Si vous ne pouvez pas vous connecter sur le port 554, essayez le port 88. Le paramètre de port peut généralement être vérifié et modifié dans l'interface web de la caméra sous la configuration réseau.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion LILIN](lilin.md) — Caméras industrielles taïwanaises
- [Guide de connexion BrickCom](brickcom.md) — Caméras industrielles taïwanaises
- [Intégration de caméras IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Découverte de périphériques ONVIF AVTech
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
