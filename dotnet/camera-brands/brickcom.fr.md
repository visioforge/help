---
title: Guide de configuration URL RTSP BrickCom pour C# .NET
description: Modèles d'URL RTSP BrickCom séries CB, MB, OB, VD et WCB pour C# .NET. Diffusez et enregistrez avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP BrickCom en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**BrickCom** (Brickcom Corporation) est un fabricant taïwanais professionnel de caméras IP dont le siège est à Taipei, à Taïwan. Fondée en 2004, BrickCom cible les marchés de la sécurité professionnelle et de la vidéosurveillance industrielle avec une large gamme de formats incluant bullet, dôme, cube et caméras spécialisées. La marque est connue pour son modèle d'URL RTSP basé sur les canaux, simple et cohérent dans toutes ses gammes de produits.

**Faits clés :**

- **Gammes de produits :** CB (cube), MB (mini bullet), OB (bullet extérieur), VD (dôme anti-vandalisme), FD (dôme fixe), MD (multi-directionnelle), WCB/WOB (sans fil)
- **Modèle d'URL basé sur les canaux :** `/channel1` pour le flux principal, `/channel2` pour le sous-flux
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (la plupart des modèles)
- **Codecs vidéo :** H.264, MJPEG
- **URL RTSP principale :** `rtsp://IP:554/channel1`

!!! tip "Numérotation des canaux"
    BrickCom utilise des URL simples basées sur les canaux. Utilisez `/channel1` pour le flux principal (haute qualité) et `/channel2` pour le flux secondaire (bande passante inférieure).

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras BrickCom utilisent un modèle d'URL RTSP simple basé sur les canaux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/channel1
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel1` | Flux principal | Flux principal (résolution la plus élevée) |
| `channel2` | Sous-flux | Flux secondaire (résolution inférieure, moins de bande passante) |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| CB-100 (cube) | Cube | `rtsp://IP:554/channel1` | Caméra cube intérieure |
| MB-300Ap (mini bullet) | Mini Bullet | `rtsp://IP:554/channel1` | Format bullet compact |
| OB-100Ap (bullet extérieur) | Bullet extérieur | `rtsp://IP:554/channel1` | Bullet étanche |
| OB-300Af (bullet extérieur) | Bullet extérieur | `rtsp://IP:554/channel1` | Bullet à mise au point automatique |
| VD-130Ae (dôme anti-vandalisme) | Dôme anti-vandalisme | `rtsp://IP:554/channel1` | Dôme certifié IK10 |
| VD-301AF (dôme anti-vandalisme) | Dôme anti-vandalisme | `rtsp://IP:554/channel1` | Dôme anti-vandalisme à mise au point automatique |
| VD-500Af (dôme anti-vandalisme) | Dôme anti-vandalisme | `rtsp://IP:554/channel1` | Dôme anti-vandalisme 5MP |
| WCB-100Ap (cube sans fil) | Cube sans fil | `rtsp://IP:554/channel1` | Caméra cube Wi-Fi |
| WCB-300AP (cube sans fil) | Cube sans fil | `rtsp://IP:554/channel1` | Cube Wi-Fi, 3MP |
| WOB-100Ae (bullet sans fil) | Bullet sans fil | `rtsp://IP:554/channel1` | Bullet extérieur Wi-Fi |
| MD-500AP-360-A1 (multi-dôme) | Multi-directionnelle | `rtsp://IP:554/channel1` | Multi-capteurs 360 degrés |

### Formats d'URL alternatifs

Certains modèles BrickCom et versions de firmware prennent en charge ces URL RTSP supplémentaires :

| Modèle d'URL | Modèles pris en charge | Notes |
|-------------|-----------------|-------|
| `rtsp://IP:554/channel1` | La plupart des modèles | Standard (recommandé) |
| `rtsp://IP:554/h264` | Divers | Flux H.264 direct |
| `rtsp://IP//ONVIF/channel2` | VD-500Af, WCB-100Ap | Sous-flux ONVIF |
| `rtsp://IP/stream/bidirect/channel1` | Modèles sélectionnés | Flux bidirectionnel avec audio |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra BrickCom avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// BrickCom OB-300Af, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/channel1");
var username = "admin";
var password = "admin";
```

Pour accéder au sous-flux, utilisez `/channel2` au lieu de `/channel1`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/snapshot.jpg` | Aucune authentification requise |
| Capture JPEG (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Authentification par URL |
| Capture de canal | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=1` | Canal spécifique avec auth |
| Capture CGI | `http://IP/cgi-bin/media.cgi?action=getSnapshot` | Capture via CGI |
| Flux HTTP de canal | `http://IP/channel2` | Sous-flux HTTP |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras BrickCom sont livrées avec les identifiants par défaut **admin / admin**. Si vous avez modifié le mot de passe via l'interface web :

1. Accédez à la caméra à `http://CAMERA_IP` dans un navigateur
2. Naviguez vers **Configuration > User Management**
3. Vérifiez vos identifiants
4. Utilisez ces identifiants dans votre URL RTSP

### L'URL de canal ne se connecte pas

Si `rtsp://IP:554/channel1` ne fonctionne pas, essayez l'URL H.264 alternative :

- `rtsp://IP:554/h264` — flux H.264 direct sans spécification de canal
- Certaines versions anciennes du firmware peuvent nécessiter le format ONVIF : `rtsp://IP//ONVIF/channel2`

### Problèmes de découverte ONVIF

Les caméras BrickCom prennent en charge ONVIF sur la plupart des modèles. Si la découverte ONVIF échoue :

1. Accédez à l'interface web à `http://CAMERA_IP`
2. Naviguez vers **Configuration > Network > ONVIF**
3. Assurez-vous qu'ONVIF est activé
4. Vérifiez le port ONVIF (par défaut : 80 ou 8080)

### Les modèles sans fil (WCB/WOB) perdent la connexion

Les caméras BrickCom sans fil (séries WCB et WOB) peuvent subir des déconnexions RTSP intermittentes sur les réseaux Wi-Fi congestionnés. Utilisez le sous-flux (`/channel2`) pour des besoins de bande passante plus faibles, ou connectez-vous via Ethernet pour une fiabilité maximale.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras BrickCom ?**

L'URL est `rtsp://admin:admin@CAMERA_IP:554/channel1` pour le flux principal. Utilisez `/channel2` pour le sous-flux avec une résolution et une bande passante inférieures.

**Les caméras BrickCom prennent-elles en charge ONVIF ?**

Oui. La plupart des modèles BrickCom actuels prennent en charge ONVIF. Certains modèles exposent également un chemin RTSP spécifique à ONVIF à `rtsp://IP//ONVIF/channel2`.

**Quelle est la différence entre channel1 et channel2 ?**

`/channel1` fournit le flux principal haute résolution, et `/channel2` fournit un flux secondaire de résolution inférieure adapté aux miniatures, à la visualisation mobile ou aux scénarios à bande passante limitée.

**Puis-je accéder à plusieurs flux simultanément ?**

Oui. Les caméras BrickCom prennent en charge des connexions simultanées à la fois sur `/channel1` et `/channel2`. Le nombre maximal de connexions simultanées dépend du modèle spécifique.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion AVTech](avtech.md) — Caméras industrielles taïwanaises
- [Guide de connexion LILIN](lilin.md) — Caméras professionnelles taïwanaises
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
