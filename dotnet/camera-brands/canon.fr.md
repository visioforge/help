---
title: URL RTSP des caméras IP Canon et guide de connexion C# .NET
description: Modèles d'URL RTSP des caméras Canon VB-H, VB-M, VB-S, VB-R pour C# .NET. Intégrez avec le SDK VisioForge pour streaming et enregistrement WPF, WinForms, MAUI.
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

# Comment se connecter à une caméra IP Canon en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Canon Inc.** est une multinationale japonaise dont le siège est à Tokyo. La division caméras IP de Canon produit la **série VB** de caméras réseau ciblant les marchés de la vidéosurveillance professionnel et entreprise. Les caméras Canon sont réputées pour leur qualité optique, tirant parti de l'expertise de Canon dans la fabrication d'objectifs. Canon a réduit sa gamme de caméras IP ces dernières années, en se concentrant sur les modèles haut de gamme.

**Faits clés :**

- **Gammes de produits :** Série VB-H (boîtier/PTZ, actuelle), série VB-M (PTZ/compactes), série VB-S (compactes), série VB-R (PTZ), série VB-C (PTZ historiques)
- **Prise en charge des protocoles :** RTSP, ONVIF (séries VB-H et VB-M), HTTP/CGI avec chemin propriétaire `-wvhttp-01-`
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** root / (spécifique à la caméra) ou admin / admin (variable selon le modèle)
- **Prise en charge ONVIF :** Oui (séries VB-H et VB-M)
- **Codecs vidéo :** H.264, H.265 (séries VB-H47, VB-H761), MJPEG

## Modèles d'URL RTSP

Les caméras Canon utilisent un streaming basé sur des profils avec des identifiants de canal et de profil dans le chemin de l'URL.

### Modèles actuels (séries VB-H / VB-M / VB-S / VB-R)

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Basé sur canal | `rtsp://IP:554/cam1/h264` | Canal 1, H.264 |
| Flux de profil | `rtsp://IP:554//stream/profile1=r` | Profil 1, mode lecture (notez la double barre) |
| Profil (court) | `rtsp://IP:554/profile1=r` | Profil 1, variante plus courte |
| Profil unicast | `rtsp://IP/profile1=u` | Mode unicast, pas de port |

!!! info "Streaming basé sur des profils"
    Les caméras Canon utilisent des identifiants de profil avec des modes d'accès : `profile1=r` pour **read** (multicast possible) et `profile1=u` pour **unicast** (connexion directe). Utilisez `=r` pour un accès général et `=u` lors d'une connexion directe sans multicast.

### URL spécifiques aux modèles

| Modèle | URL RTSP | Type | Notes |
|-------|----------|------|-------|
| VB-H41 | `rtsp://IP:554//stream/profile1=r` | Boîtier fixe | Profil avec double barre |
| VB-H43 / VB-H45 | `rtsp://IP:554/cam1/h264` | Boîtier fixe | Basé sur canal |
| VB-H47 | `rtsp://IP:554/cam1/h264` | Boîtier fixe | Compatible H.265 |
| VB-H610D / VB-H610VE | `rtsp://IP:554/cam1/h264` | Dôme fixe | Actuel |
| VB-H730F | `rtsp://IP:554/cam1/h264` | Dôme fixe | Fisheye |
| VB-H751LE | `rtsp://IP:554/cam1/h264` | Bullet fixe | Extérieur |
| VB-H761LVE | `rtsp://IP:554/cam1/h264` | Bullet fixe | Compatible H.265 |
| VB-M40 | `rtsp://IP/profile1=u` | PTZ compacte | Unicast, pas de port spécifié |
| VB-M42 / VB-M44 | `rtsp://IP:554/cam1/h264` | PTZ compacte | Basé sur canal |
| VB-M600D | `rtsp://IP/profile1=r` | Dôme compacte | Mode lecture |
| VB-M620D / VB-M640V | `rtsp://IP:554/cam1/h264` | Dôme compacte | Actuel |
| VB-M741LE | `rtsp://IP:554/cam1/h264` | PTZ compacte | Extérieur |
| VB-S30D / VB-S31D | `rtsp://IP:554/cam1/h264` | Compacte | Intérieur |
| VB-S800D / VB-S900F | `rtsp://IP:554/cam1/h264` | Compacte | Intérieur |
| VB-R11 / VB-R11VE | `rtsp://IP:554/cam1/h264` | Dôme PTZ | Actuel |
| VB-R12VE | `rtsp://IP:554/cam1/h264` | Dôme PTZ | Extérieur |

### Modèles historiques (série VB-C — HTTP uniquement)

Les caméras VB-C historiques ne prennent pas en charge RTSP. Elles utilisent les URL HTTP propriétaires Canon `-wvhttp-01-` :

| Modèle | URL HTTP | Type | Notes |
|-------|----------|------|-------|
| VB-C300 | `http://IP/-wvhttp-01-/GetLiveImage` | Dôme PTZ | HTTP uniquement |
| VB-C10 | `http://IP/-wvhttp-01-/GetLiveImage` | Compacte | HTTP uniquement |
| VB-C50i | `http://IP/-wvhttp-01-/GetLiveImage` | Dôme PTZ | HTTP uniquement |
| VB-610 | `http://IP/-wvhttp-01-/video.cgi` | Fixe | HTTP uniquement |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Canon avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Canon série VB-H, canal 1
var uri = new Uri("rtsp://192.168.1.70:554/cam1/h264");
var username = "root";
var password = "YourPassword";
```

Pour l'accès basé sur profil sur les anciens modèles VB, utilisez `rtsp://IP:554/profile1=r` ou `rtsp://IP/profile1=u` selon le modèle.

## URL de capture instantanée et MJPEG

Canon utilise un préfixe de chemin distinctif `-wvhttp-01-` pour tous les accès image et vidéo basés sur HTTP :

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Image en direct | `http://IP/-wvhttp-01-/GetLiveImage` | Capture actuelle |
| Flux MJPEG | `http://IP/-wvhttp-01-/video.cgi` | MJPEG continu |
| Capture (dimensionnée) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT` | Résolution personnalisée |
| Capture (continue) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT&frame_count=0` | Capture continue |

!!! info "Préfixe `-wvhttp-01-` de Canon"
    Le préfixe de chemin `-wvhttp-01-` est unique aux caméras réseau Canon. Toutes les URL d'image et vidéo basées sur HTTP utilisent ce préfixe. Ce chemin distinctif peut aider à identifier les caméras Canon sur un réseau.

## Dépannage

### RTSP doit être activé dans l'interface web

Les caméras Canon peuvent ne pas avoir RTSP activé par défaut. Accédez à l'interface web de la caméra et naviguez vers les paramètres de streaming pour activer RTSP. Sans cela, la caméra ne répondra qu'aux requêtes HTTP.

### La série VB-C historique ne prend en charge que HTTP

La série VB-C (VB-C300, VB-C10, VB-C50i) et VB-610 ne prennent pas du tout en charge RTSP. Utilisez les URL HTTP `-wvhttp-01-` de Canon pour l'accès vidéo depuis ces modèles :

- `http://IP/-wvhttp-01-/GetLiveImage` pour les captures
- `http://IP/-wvhttp-01-/video.cgi` pour le streaming MJPEG

### Modes lecture vs unicast pour les profils

Les URL de profil Canon utilisent deux modes d'accès :

- `profile1=r` — **Mode lecture** : Permet la distribution multicast, adapté à plusieurs spectateurs
- `profile1=u` — **Mode unicast** : Connexion directe, un spectateur par flux

Si le multicast n'est pas configuré sur votre réseau, utilisez `profile1=u` pour une connexion unicast directe.

### Double barre dans certaines URL

Certains modèles Canon (notamment VB-H41) nécessitent une **double barre oblique** avant le chemin du flux :

- VB-H41 : `rtsp://IP:554//stream/profile1=r` (double barre)
- La plupart des autres : `rtsp://IP:554/cam1/h264` (barre unique)

### Les identifiants par défaut varient

Les caméras Canon n'ont pas d'identifiants par défaut universels :

- **Modèles actuels :** Souvent `root` avec un mot de passe défini lors de la configuration initiale
- **Modèles plus anciens :** Peuvent utiliser `admin` / `admin` ou `root` / `camera`
- Vérifiez l'étiquette de la caméra ou le guide de configuration pour les valeurs par défaut spécifiques au modèle

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Canon ?**

Pour les caméras Canon actuelles des séries VB-H et VB-M, utilisez `rtsp://root:password@CAMERA_IP:554/cam1/h264`. Pour les modèles plus anciens, essayez `rtsp://CAMERA_IP:554/profile1=r` ou `rtsp://CAMERA_IP/profile1=u`.

**Les caméras Canon prennent-elles en charge H.265 ?**

Certains modèles Canon prennent en charge H.265, dont les séries VB-H47 et VB-H761. La plupart des autres caméras série VB utilisent H.264. Les modèles VB-C historiques ne prennent en charge que MJPEG sur HTTP.

**Qu'est-ce que le chemin `-wvhttp-01-` dans les URL Canon ?**

Le préfixe `-wvhttp-01-` est le chemin HTTP propriétaire de Canon utilisé pour tous les accès image et vidéo sur le web de leurs caméras réseau. Il est utilisé pour les captures (`GetOneShot`, `GetLiveImage`), le streaming MJPEG (`video.cgi`) et le contrôle de la caméra. Ce chemin est unique aux caméras Canon.

**Puis-je me connecter aux caméras Canon VB-C historiques ?**

Les caméras VB-C historiques (VB-C300, VB-C10, VB-C50i) sont HTTP uniquement et ne prennent pas en charge RTSP. Vous pouvez accéder à leur vidéo à l'aide de l'URL HTTP `http://CAMERA_IP/-wvhttp-01-/GetLiveImage` pour les captures ou `http://CAMERA_IP/-wvhttp-01-/video.cgi` pour le streaming MJPEG.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Sony](sony.md) — Caméras entreprise japonaises
- [Guide de connexion Axis](axis.md) — Leader de la vidéosurveillance entreprise
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
