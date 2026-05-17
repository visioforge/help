---
title: URL RTSP des caméras IP D-Link DCS et exemples C# .NET
description: Modèles d'URL RTSP des caméras D-Link DCS pour intégration C# .NET. Couvre DCS-930, DCS-2130, DCS-5222 avec code SDK VisioForge et authentification.
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

# Comment se connecter à une caméra IP D-Link en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**D-Link Corporation** est un fabricant taïwanais d'équipements réseau dont le siège est à Taipei. D-Link produit des caméras IP sous la gamme **DCS (D-Link Cloud Security)**, ciblant les marchés grand public et petites entreprises. Les caméras D-Link sont largement disponibles dans les circuits de vente au détail et sont populaires pour la sécurité résidentielle et les déploiements en petits bureaux.

**Faits clés :**

- **Gammes de produits :** DCS-930/932/933/934 (Wi-Fi grand public), DCS-2130/2132/2230/2310/2330/2332 (prosumer), DCS-5020/5222/5615 (PTZ), DCS-6010/6113/6818 (entreprise), DCS-7010/7110/7410 (extérieur professionnel)
- **Prise en charge des protocoles :** RTSP, ONVIF (certains modèles), HTTP/CGI, MJPEG, cloud D-Link mydlink
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (mot de passe vide) ; certains modèles : admin / admin
- **Prise en charge ONVIF :** Modèles sélectionnés uniquement (généralement DCS-2xxx et supérieurs)
- **Codecs vidéo :** H.264, MJPEG, MPEG-4 (historique)

## Modèles d'URL RTSP

### Modèles actuels et récents

Les caméras D-Link utilisent le format d'URL `live.sdp` ou `play.sdp` :

| Flux | URL RTSP | Qualité | Notes |
|--------|----------|---------|-------|
| Flux principal (H.264) | `rtsp://IP:554/live1.sdp` | Élevée | Flux principal H.264 |
| Sous-flux (H.264) | `rtsp://IP:554/live2.sdp` | Moyenne | Deuxième flux |
| Troisième flux | `rtsp://IP:554/live3.sdp` | Basse | Troisième flux (certains modèles) |
| Flux principal (alt) | `rtsp://IP:554/play1.sdp` | Élevée | URL alternative |
| Sous-flux (alt) | `rtsp://IP:554/play2.sdp` | Moyenne | URL alternative |

### URL spécifiques aux modèles

| Modèle | URL RTSP | Résolution | Type |
|-------|----------|------------|------|
| DCS-930L | `rtsp://IP:554/play1.sdp` | 640x480 | Wi-Fi grand public |
| DCS-932L | `rtsp://IP:554/play1.sdp` | 640x480 | Wi-Fi grand public IR |
| DCS-933L | `rtsp://IP:554/play1.sdp` | 640x480 | Wi-Fi grand public |
| DCS-934L | `rtsp://IP:554/play1.sdp` | 1280x720 | HD grand public |
| DCS-942L | `rtsp://IP:554/play1.sdp` | 640x480 | IR grand public |
| DCS-2100+ | `rtsp://IP:554/live.sdp` | 640x480 | Historique |
| DCS-2121 | `rtsp://IP:554/play1.sdp` | 640x480 | Prosumer |
| DCS-2130 | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumer HD |
| DCS-2132L | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumer HD |
| DCS-2230 | `rtsp://IP:554//live1.sdp` | 1920x1080 | Prosumer FHD |
| DCS-2310L | `rtsp://IP:554/live1.sdp` | 1280x720 | Extérieur HD |
| DCS-2332L | `rtsp://IP:554//live1.sdp` | 1280x720 | Extérieur HD |
| DCS-5020L | `rtsp://IP:554/play1.sdp` | 640x480 | PTZ grand public |
| DCS-5222L | `rtsp://IP:554//live1.sdp` | 1280x720 | PTZ HD |
| DCS-6010L | `rtsp://IP:554/live1.sdp` | 1600x1200 | Panoramique |
| DCS-6113 | `rtsp://IP:554/live1.sdp` | 1920x1080 | Caméra boîtier |
| DCS-6818 | `rtsp://IP:554/live3.sdp` | 1920x1080 | Entreprise |
| DCS-7010L | `rtsp://IP:554/live1.sdp` | 1280x720 | Extérieur PoE |
| DCS-7110 | `rtsp://IP:554/live1.sdp` | 1280x800 | Extérieur HD |
| DCS-7410 | `rtsp://IP:554/live1.sdp` | 1280x720 | Extérieur entreprise |

!!! info "Double barre dans certaines URL"
    Certains modèles D-Link utilisent une double barre avant le chemin : `rtsp://IP:554//live1.sdp`. C'est courant sur les modèles DCS-2130, DCS-2132L, DCS-2230, DCS-2332L et DCS-5222L. Essayez les deux formats (simple et double barre) si l'un ne fonctionne pas.

### Modèles historiques (HTTP uniquement)

Les très anciennes caméras D-Link DCS ne prennent en charge que HTTP :

| Modèle | URL | Notes |
|-------|-----|-------|
| DCS-900 | `http://IP/cgi-bin/video.jpg` | JPEG uniquement |
| DCS-910 | `http://IP/video.cgi` | MJPEG |
| DCS-920 | `http://IP/video.cgi` | MJPEG |
| DCS-2100 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG uniquement |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra D-Link avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra D-Link DCS, flux principal
var uri = new Uri("rtsp://192.168.1.45:554/live1.sdp");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez plutôt `/live2.sdp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/image/jpeg.cgi` | La plupart des modèles DCS actuels |
| Flux MJPEG | `http://IP/video/mjpg.cgi` | MJPEG continu |
| MJPEG (alt) | `http://IP/video.cgi` | Modèles plus anciens |
| MJPEG (auth) | `http://IP/mjpeg.cgi?user=USER&password=PASS&channel=1` | Avec authentification |
| Capture DMS | `http://IP/dms.jpg` | DCS-2130/2132/2230/2310/2332 |
| Flux DMS | `http://IP/dms?nowprofileid=2` | Basé sur profil |
| Flux ipcam | `http://IP/ipcam/stream.cgi?nowprofileid=2` | Certains modèles |
| JPEG historique | `http://IP/cgi-bin/video.jpg` | Très anciens modèles DCS |

## Dépannage

### Format d'URL live vs play

Les caméras D-Link utilisent deux conventions de nommage d'URL :

- **Modèles actuels (DCS-2xxx+) :** `live1.sdp`, `live2.sdp`, `live3.sdp`
- **Modèles grand public (DCS-930/932/933/942) :** `play1.sdp`, `play2.sdp`, `play3.sdp`

Si l'un des formats ne fonctionne pas, essayez l'autre.

### Le mot de passe par défaut est vide

De nombreuses caméras D-Link sont livrées avec `admin` comme nom d'utilisateur et un **mot de passe vide**. Vous devrez peut-être définir un mot de passe via l'interface web ou l'assistant de configuration D-Link avant que RTSP fonctionne correctement.

### Caméras cloud mydlink

Certaines caméras D-Link plus récentes sont conçues principalement pour l'écosystème cloud mydlink et peuvent avoir une prise en charge RTSP limitée ou nulle. Vérifiez les spécifications de la caméra pour la prise en charge « RTSP » ou « intégration tierce ».

### Configuration du port

Les caméras D-Link utilisent le port 554 par défaut pour RTSP. L'interface HTTP est généralement sur le port 80. Les deux peuvent être modifiés dans l'interface web de la caméra sous Network Settings.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras D-Link ?**

Pour la plupart des caméras D-Link DCS, essayez `rtsp://admin:password@CAMERA_IP:554/live1.sdp` ou `rtsp://admin:password@CAMERA_IP:554/play1.sdp`. Le format `live` est utilisé par les modèles plus récents, tandis que `play` est utilisé par les modèles grand public.

**Les caméras D-Link prennent-elles en charge ONVIF ?**

Certains modèles prennent en charge ONVIF (généralement DCS-2xxx et modèles haut de gamme). Les caméras grand public comme la DCS-930L et la DCS-932L ne prennent généralement pas en charge ONVIF.

**Quelle est la différence entre live1.sdp et play1.sdp ?**

Les deux remplissent le même rôle (flux vidéo principal) mais sont utilisés par différentes générations de caméras D-Link. `live1.sdp` est plus courant sur les modèles prosumer/professionnels plus récents, tandis que `play1.sdp` est utilisé sur les anciens modèles grand public.

**Puis-je me connecter aux caméras D-Link sans l'application mydlink ?**

Oui. Les caméras D-Link avec prise en charge RTSP sont accessibles directement via leur adresse IP sans le service cloud mydlink. Le cloud mydlink est optionnel pour l'accès à distance.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Foscam](foscam.md) — Pair en caméras IP grand public
- [Guide de connexion TP-Link](tp-link.md) — Caméras grand public avec RTSP
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
