---
title: Modèles d'URL RTSP Sanyo — caméras IP et C# .NET VisioForge
description: Connectez les caméras IP Sanyo VCC, VDC et VCC-HD en C# .NET avec modèles d'URL RTSP, points de terminaison de capture et exemples de code SDK VisioForge.
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

# Comment se connecter à une caméra IP Sanyo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Sanyo** (Sanyo Electric Co., Ltd.) était un fabricant japonais d'électronique dont le siège était à Osaka, au Japon. La division caméras de sécurité de Sanyo produisait les gammes de caméras VCC et VDC, très appréciées dans les installations de vidéosurveillance professionnelles. Entre 2009 et 2011, Panasonic a acquis Sanyo Electric, et la technologie de caméra a été intégrée à la gamme de produits i-PRO de Panasonic. Bien que les caméras Sanyo ne soient plus fabriquées, de nombreuses unités restent déployées dans des installations héritées à travers le monde.

**Faits clés :**

- **Gammes de produits :** VCC (caméras box), VDC (caméras dôme), VCC-HD (série HD)
- **Statut :** Discontinué (acquis par Panasonic 2009-2011)
- **Prise en charge des protocoles :** RTSP, HTTP/CGI, ONVIF limité (firmware plus récent)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Limitée (firmware plus ancien uniquement)
- **Codecs vidéo :** H.264, MJPEG
- **Successeur :** Panasonic i-PRO

!!! warning "Les caméras Sanyo sont discontinuées"
    Les caméras de sécurité Sanyo sont discontinuées. Sanyo Electric a été acquise par Panasonic, et la technologie de caméra a été intégrée à la gamme de produits i-PRO de Panasonic. Consultez notre [guide de connexion Panasonic/i-PRO](panasonic.md) pour les produits actuels.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Sanyo utilisent le chemin RTSP `VideoInput` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/VideoInput/1/h264/1
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `VideoInput` | 1, 2, 3... | Canal de caméra (1 pour les caméras autonomes) |
| `h264` | h264 | Codec vidéo H.264 |
| `1` final | 1 | Index de flux |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| VCC-HD2300P | Caméra box HD | `rtsp://IP:554/VideoInput/1/h264/1` | Flux principal H.264 |
| Série VCC-HD | Caméras HD | `rtsp://IP:554/VideoInput/1/h264/1` | Flux principal H.264 |
| VCC-9574N | Caméra box | `rtsp://IP:554/VideoInput/1/h264/1` | Flux principal H.264 |
| VCC-P9574N | Caméra PTZ | `rtsp://IP:554/VideoInput/1/h264/1` | Flux principal H.264 |
| Série VDC | Caméras dôme | `rtsp://IP:554/VideoInput/1/h264/1` | Flux principal H.264 |

### URL des canaux DVR

Pour les systèmes DVR Sanyo à canaux multiples :

| Canal | URL du flux principal |
|---------|----------------|
| Caméra 1 | `rtsp://IP:554/VideoInput/1/h264/1` |
| Caméra 2 | `rtsp://IP:554/VideoInput/2/h264/1` |
| Caméra N | `rtsp://IP:554/VideoInput/N/h264/1` |

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/VideoInput/1/h264/1` | Flux H.264 standard (recommandé) |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | Accès DVR multicanal |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Sanyo avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Sanyo VCC-HD2300P, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
var username = "admin";
var password = "YourPassword";
```

Pour l'accès DVR multicanal, remplacez `VideoInput/1` par le numéro de canal approprié.

## URL de capture instantanée et MJPEG

!!! note "Point de terminaison liveimg.cgi de Sanyo"
    Les caméras Sanyo utilisent un point de terminaison distinctif `/liveimg.cgi` pour les captures HTTP et les flux MJPEG. Le paramètre `serverpush=1` active le streaming MJPEG continu.

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture en direct | `http://IP/liveimg.cgi` | Image JPEG unique |
| Flux MJPEG | `http://IP/liveimg.cgi?serverpush=1` | MJPEG continu en server-push |
| MJPEG avec canal | `http://IP/liveimg.cgi?serverpush=1&jpeg=1&stream=CHANNEL` | Flux MJPEG spécifique au canal |
| Capture de canal (DVR) | `http://IP/liveimg.cgi?ch=CHANNEL` | Capture spécifique au canal pour DVR |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras Sanyo utilisent l'authentification basique par défaut. Assurez-vous de fournir les bons identifiants :

1. Accédez à la caméra à l'adresse `http://CAMERA_IP` dans un navigateur
2. Connectez-vous avec vos identifiants (par défaut : admin / admin)
3. Vérifiez que le service RTSP est activé dans les paramètres réseau
4. Utilisez ces identifiants dans votre URL RTSP

### Flux H.264 indisponible

Les anciens modèles Sanyo peuvent ne prendre en charge que MJPEG. Si l'URL H.264 ne fonctionne pas, essayez d'utiliser plutôt le flux HTTP MJPEG :

```
http://CAMERA_IP/liveimg.cgi?serverpush=1
```

### Firmware et compatibilité

Comme les caméras Sanyo sont discontinuées, les mises à jour de firmware ne sont plus disponibles. Si vous rencontrez des problèmes de compatibilité :

- Assurez-vous que le firmware de la caméra est la dernière version disponible
- Essayez d'utiliser la découverte ONVIF si la connexion par URL directe échoue
- Envisagez de migrer vers les caméras Panasonic i-PRO, qui héritent de la technologie Sanyo

### Le server-push MJPEG ne fonctionne pas

Le paramètre `serverpush=1` nécessite que le serveur HTTP de la caméra prenne en charge l'encodage de transfert par morceaux (chunked transfer). Certaines versions de firmware plus anciennes peuvent ne pas le prendre en charge de manière fiable. Essayez le point de terminaison de capture unique (`/liveimg.cgi` sans paramètres) et interrogez à la fréquence d'images souhaitée à la place.

## FAQ

**Les caméras Sanyo sont-elles toujours prises en charge ?**

Les caméras de sécurité Sanyo sont discontinuées. Sanyo Electric a été entièrement acquise par Panasonic, et la technologie de caméras de surveillance a été fusionnée avec la gamme de produits i-PRO de Panasonic. Aucune nouvelle mise à jour de firmware ni support ne sont disponibles pour les caméras de marque Sanyo.

**Quelle est l'URL RTSP par défaut pour les caméras Sanyo ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/VideoInput/1/h264/1` pour le flux H.264 principal. Pour les configurations DVR, remplacez `VideoInput/1` par le numéro de canal approprié (par exemple, `VideoInput/2` pour le canal 2).

**Les caméras Sanyo prennent-elles en charge ONVIF ?**

Seules certaines caméras Sanyo avec des versions de firmware plus récentes disposent d'une prise en charge ONVIF limitée. La plupart des modèles plus anciens ne prennent pas en charge ONVIF et nécessitent une configuration d'URL RTSP directe.

**Que dois-je utiliser à la place des caméras Sanyo ?**

La gamme de produits i-PRO de Panasonic est le successeur direct de la division caméras de sécurité de Sanyo. Les caméras i-PRO utilisent des chemins RTSP VideoInput similaires et offrent des fonctionnalités modernes comme H.265, des analyses avancées et la prise en charge ONVIF complète. Consultez notre [guide de connexion Panasonic/i-PRO](panasonic.md).

**Comment obtenir des captures d'une caméra Sanyo ?**

Utilisez le point de terminaison HTTP `/liveimg.cgi` : `http://CAMERA_IP/liveimg.cgi` renvoie une image JPEG unique. Ajoutez `?serverpush=1` pour un flux MJPEG continu, ou `?ch=CHANNEL` pour un canal DVR spécifique.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Panasonic/i-PRO](panasonic.md) — Gamme de produits successeur
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
