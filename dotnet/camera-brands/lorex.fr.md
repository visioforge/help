---
title: Caméras IP Lorex — URL RTSP et intégration en C# .NET
description: URL RTSP des Lorex LNB, LNE, LNZ, DVR et NVR en C# .NET. Diffusion et enregistrement avec le VisioForge Video Capture SDK.
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
  - C#

---

# Comment se connecter à une caméra IP Lorex en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Lorex Technology** (une filiale de Dahua Technology via FLIR/Lorex) est une marque majeure de caméras de sécurité grand public et prosommateur en Amérique du Nord. Les caméras Lorex sont principalement fabriquées par **Dahua Technology** et vendues sous la marque Lorex via des canaux de distribution incluant Amazon, Costco et Best Buy. Lorex est l'une des marques de caméras de sécurité les plus vendues aux États-Unis et au Canada.

**Faits clés :**

- **Gammes de produits :** LNB (bullet IP), LNE (dôme/turret IP), LNZ (PTZ IP), LNC (Wi-Fi grand public), IPSC (hérité), série L (héritée)
- **Protocoles pris en charge :** RTSP, ONVIF, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de la configuration du NVR/caméra) ; certains modèles plus anciens : admin / admin
- **Prise en charge ONVIF :** Oui (la plupart des modèles actuels)
- **Codecs vidéo :** H.264, H.265 (modèles plus récents)
- **Base OEM :** Dahua Technology (certains modèles utilisent le firmware Hikvision)

!!! info "Lorex utilise plusieurs sources OEM"
    La plupart des caméras IP Lorex sont fabriquées par Dahua et utilisent le format d'URL RTSP de Dahua. Cependant, certains modèles Lorex (notamment LNB2153 et MCNB2153) utilisent un firmware basé sur Hikvision avec des URL `/Streaming/Channels/`. Vérifiez les deux formats d'URL si l'un ne fonctionne pas.

## Modèles d'URL RTSP

### Modèles basés sur Dahua (la plupart des caméras IP Lorex)

La plupart des caméras IP Lorex utilisent le format d'URL Dahua :

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Flux principal | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Résolution complète |
| Sous-flux | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Résolution inférieure |

### Modèles basés sur Hikvision

Certains modèles Lorex utilisent le firmware Hikvision :

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Flux principal | `rtsp://IP:554//Streaming/Channels/1` | Résolution complète (notez la double barre oblique) |
| Sous-flux | `rtsp://IP:554//Streaming/Channels/2` | Résolution inférieure |
| H.264 direct | `rtsp://IP:554/ch0_0.h264` | Flux H.264 direct |

### URL spécifiques aux modèles

| Modèle | URL RTSP | Base OEM | Notes |
|--------|----------|----------|-------|
| LNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | Bullet 1080p |
| LNB2184 | `rtsp://IP:554/video.mp4` | Dahua | Bullet 4MP |
| LNE1001 | `rtsp://IP:554/` | Dahua | Dôme 1080p |
| LNE3003 | `rtsp://IP:554/video.mp4` | Dahua | Dôme 2K |
| LNZ4001 | `rtsp://IP:554/video.mp4` | Dahua | PTZ |
| MCNB2153 | `rtsp://IP:554//Streaming/Channels/1` | Hikvision | Bullet 1080p |

### Formats d'URL alternatifs

Certaines caméras Lorex répondent également à ces URL :

| Modèle d'URL | Notes |
|--------------|-------|
| `rtsp://IP:554/` | Chemin racine (certains modèles) |
| `rtsp://IP:554/video.mp4` | Flux vidéo |
| `rtsp://IP:554/ch0_0.h264` | H.264 direct |

### Modèles hérités

| Modèle | URL | Notes |
|--------|-----|-------|
| Série IPSC | `rtsp://IP:554/` | Caméras IP héritées |
| L23WD | `rtsp://IP:554/` | Sans fil hérité |
| IP1240 | `http://IP/GetData.cgi` | HTTP uniquement |
| LNC104/116/204 | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Caméras Wi-Fi, HTTP uniquement |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Lorex avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code).

### Modèles basés sur Dahua (la plupart des caméras Lorex)

```csharp
// Caméra Lorex (basée sur Dahua), flux principal
var uri = new Uri("rtsp://192.168.1.65:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

### Modèles Lorex basés sur Hikvision

```csharp
// Lorex LNB2153 (basé sur Hikvision), flux principal
var uri = new Uri("rtsp://192.168.1.65:554//Streaming/Channels/1");
var username = "admin";
var password = "YourPassword";
```

Consultez le [guide d'identification OEM](#determine-your-oem-base) dans le Dépannage pour déterminer quel format d'URL utilise votre caméra Lorex.

## URL des snapshots

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG | `http://IP/jpg/image.jpg` | La plupart des caméras IP Lorex |
| Snapshot (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Caméras Wi-Fi grand public |
| Snapshot (compte) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Authentification alternative |
| GetData | `http://IP/GetData.cgi` | Modèles hérités |
| Flux MJPEG | `http://IP/video.mjpg` | MJPEG continu |

## Dépannage

### Déterminer votre base OEM { #determine-your-oem-base }

Les caméras Lorex utilisent le firmware de différents fabricants. Pour déterminer quel format d'URL utiliser :

1. Essayez d'abord le **format Dahua** : `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0`
2. Si cela échoue, essayez le **format Hikvision** : `rtsp://IP:554//Streaming/Channels/1`
3. Consultez l'interface web de la caméra — les caméras basées sur Dahua ont une UI web bleue/blanche, tandis que celles basées sur Hikvision ont une UI gris foncé/noire

### NVR contre accès direct à la caméra

- Lors de la connexion via un NVR Lorex, utilisez `channel=N` dans le format d'URL Dahua pour sélectionner la caméra
- Lors de la connexion directe à une caméra IP, utilisez toujours `channel=1`

### Caméras Wi-Fi grand public Lorex (série LNC)

La série LNC (LNC104, LNC116, LNC204) sont des caméras Wi-Fi grand public qui ne prennent généralement pas en charge RTSP. Elles fournissent uniquement des URL de snapshot HTTP et sont principalement conçues pour être utilisées avec l'application Lorex.

### Port 9000

Certaines très anciennes caméras Lorex utilisaient le port 9000 pour le streaming au lieu du 554. Si le port 554 standard ne fonctionne pas sur un modèle plus ancien, essayez : `rtsp://IP:9000/`

## FAQ

**Les caméras Lorex sont-elles les mêmes que Dahua ?**

La plupart des caméras IP Lorex sont fabriquées par Dahua et utilisent un firmware identique. Le format d'URL RTSP (`cam/realmonitor?channel=1&subtype=0`) est le même. Cependant, certains modèles Lorex utilisent le firmware Hikvision. Consultez notre [guide de connexion Dahua](dahua.md) pour des détails supplémentaires.

**Quelle est l'URL RTSP par défaut pour les caméras Lorex ?**

Essayez d'abord `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` (basée sur Dahua). Si cela échoue, essayez `rtsp://admin:password@CAMERA_IP:554//Streaming/Channels/1` (basée sur Hikvision).

**Puis-je utiliser les caméras Lorex sans le NVR Lorex ?**

Oui. Les caméras IP Lorex avec prise en charge RTSP peuvent être connectées directement en utilisant leurs adresses IP individuelles. Vous n'avez pas besoin du NVR Lorex pour l'intégration avec un logiciel tiers.

**Les caméras Lorex prennent-elles en charge ONVIF ?**

La plupart des caméras IP Lorex actuelles prennent en charge ONVIF. Les caméras Wi-Fi grand public (série LNC) généralement ne le prennent pas en charge.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Même format d'URL pour la plupart des modèles
- [Guide de connexion Amcrest](amcrest.md) — Autre OEM Dahua
- [Guide de connexion Swann](swann.md) — Concurrent du segment grand public/prosommateur
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
