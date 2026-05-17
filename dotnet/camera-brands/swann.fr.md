---
title: URL RTSP Swann — guide de connexion en C# .NET VisioForge
description: Modèles d'URL RTSP pour les caméras Swann NHD, SWNHD, DVR/NVR et ADS en C# .NET. Streaming et enregistrement avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP Swann en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Swann** (Swann Communications) est une marque australienne de sécurité grand public dont le siège est à Melbourne, en Australie, désormais détenue par **Infinova**. Swann est l'une des marques de sécurité grand public et semi-professionnelles les plus connues, populaire pour ses systèmes de caméras groupés DVR/NVR vendus chez les principaux revendeurs. Swann propose une gamme de caméras IP autonomes, de systèmes de caméras analogiques sur coaxial (BNC) et d'enregistreurs vidéo réseau.

**Faits clés :**

- **Gammes de produits :** NHD (caméras réseau HD actuelles), SWNHD (caméras IP HD), SWPRO (analogique sur coaxial), systèmes DVR/NVR, ADS (caméras IP héritées)
- **Prise en charge des protocoles :** RTSP, ONVIF (modèles NHD actuels), HTTP/MJPEG (hérité)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin ou admin / (vide) sur les modèles plus anciens
- **Prise en charge ONVIF :** Oui (caméras série NHD actuelles)
- **Codecs vidéo :** H.264, H.265 (modèles actuels), MPEG-4 (DVR hérités)
- **Base OEM :** De nombreux NVR Swann récents sont des OEM Hikvision et utilisent les modèles d'URL RTSP Hikvision

!!! info "NVR Swann et Hikvision"
    De nombreux NVR Swann actuels sont fabriqués par Hikvision et utilisent le firmware Hikvision. Si l'URL RTSP Swann standard ne fonctionne pas sur votre NVR, essayez le format Hikvision (`/Streaming/Channels/`). Consultez notre [guide de connexion Hikvision](hikvision.md) pour plus de détails.

## Modèles d'URL RTSP

### Caméras IP actuelles série NHD

Les caméras IP autonomes Swann série NHD (SWNHD-820CAM, SWNHD-830CAM, NHD-866, etc.) utilisent l'URL suivante :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live/h264
```

### Systèmes NVR (basés sur Hikvision)

La plupart des NVR Swann actuels utilisent des chemins RTSP de type Hikvision :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//Streaming/Channels/[CHANNEL_ID]
```

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554//Streaming/Channels/1` | `rtsp://IP:554//Streaming/Channels/102` |
| Caméra 2 | `rtsp://IP:554//Streaming/Channels/2` | `rtsp://IP:554//Streaming/Channels/202` |
| Caméra N | `rtsp://IP:554//Streaming/Channels/N` | `rtsp://IP:554//Streaming/Channels/N02` |

!!! note "Numérotation des canaux"
    Pour les NVR basés sur Hikvision, l'ID de canal du flux principal correspond au numéro de caméra (1, 2, 3...). Le sous-flux utilise le format `N02` où N est le numéro de caméra (102, 202, 302...).

### Modèles DVR hérités

Les anciens systèmes DVR Swann (DVR4-PRO-NET, etc.) et les caméras autonomes utilisent MPEG-4 :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/mpeg4
```

### Tableau récapitulatif des URL

| Modèle / Série | URL du flux principal | Notes |
|----------------|----------------|-------|
| Caméras série NHD (SWNHD-820/830) | `rtsp://IP:554/live/h264` | Caméras IP autonomes |
| IP-3G ConnectCam | `rtsp://IP:554/mpeg4` | Autonome hérité |
| Max-IP-Cam | `rtsp://IP:554/mpeg4` | Autonome hérité |
| NVR actuel (canal 1) | `rtsp://IP:554//Streaming/Channels/1` | OEM Hikvision |
| NVR actuel (canal 1, sous) | `rtsp://IP:554//Streaming/Channels/102` | OEM Hikvision |
| DVR4-PRO-NET | `rtsp://IP:554/mpeg4` | DVR hérité |
| Caméras IP Swann génériques | `rtsp://IP:554/live/h264` | Essayez celui-ci en premier |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Swann avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Swann série NHD, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

Pour l'accès au sous-flux NVR, utilisez `/Streaming/Channels/102` à la place de `/Streaming/Channels/1`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Flux HTTP (ADS-440 hérité) | `http://IP/videostream.asf?user=USER&pwd=PASS` | Format ASF, pas de RTSP |
| Flux MJPEG (hérité) | `http://IP/videostream.cgi?user=USER&pwd=PASS` | Modèles plus anciens |
| Capture ONVIF | `http://IP/onvif-http/snapshot` | Série NHD avec ONVIF |

!!! warning "Caméras héritées HTTP uniquement"
    La série ADS-440 et certains autres anciens modèles Swann ne prennent en charge que le streaming HTTP (ASF ou MJPEG) et ne prennent pas du tout en charge RTSP. Utilisez directement l'URL HTTP pour ces caméras.

## Dépannage

### Identifier le type de firmware de votre NVR

De nombreux NVR Swann sont OEM Hikvision. Pour déterminer le format d'URL à utiliser :

1. Accédez à l'interface web du NVR à l'adresse `http://NVR_IP`
2. Vérifiez la page de connexion — les NVR basés sur Hikvision affichent souvent une interface de type Hikvision
3. Essayez d'abord l'URL Hikvision (`/Streaming/Channels/1`), puis revenez aux URL Swann (`/live/h264` ou `/mpeg4`)

### « Connection refused » sur les caméras héritées

Les anciennes caméras Swann (série ADS-440, premiers modèles DVR) peuvent ne pas prendre en charge RTSP du tout. Ces caméras n'utilisent que le streaming HTTP. Essayez l'URL HTTP ASF ou MJPEG au lieu de RTSP.

### Les identifiants par défaut ne fonctionnent pas

- Les modèles actuels sont généralement livrés avec admin / admin mais nécessitent un changement de mot de passe à la première configuration
- Certains modèles plus anciens utilisent admin avec un mot de passe vide
- Effectuez toujours la configuration initiale via l'interface web Swann ou l'application SwannView avant de tenter l'accès RTSP

### SwannView vs accès RTSP local

SwannView (le service cloud de Swann) est distinct de l'accès RTSP local. Vous n'avez pas besoin d'un compte SwannView pour utiliser le streaming RTSP sur votre réseau local. RTSP fonctionne uniquement via la connexion réseau locale.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Swann ?**

Pour les caméras série NHD actuelles, utilisez `rtsp://admin:password@CAMERA_IP:554/live/h264`. Pour les NVR Swann (basés sur Hikvision), utilisez `rtsp://admin:password@NVR_IP:554//Streaming/Channels/1` pour le flux principal du canal 1.

**Les NVR Swann sont-ils compatibles avec les URL RTSP Hikvision ?**

Oui. De nombreux NVR Swann actuels sont fabriqués par Hikvision et utilisent un firmware identique. Le format d'URL RTSP Hikvision (`/Streaming/Channels/`) fonctionne sur ces systèmes. Si l'URL Swann standard échoue, essayez le format Hikvision.

**Toutes les caméras Swann prennent-elles en charge RTSP ?**

Non. Certains modèles Swann hérités (en particulier la série ADS-440) ne prennent en charge que le streaming HTTP au format ASF ou MJPEG. Toutes les caméras et NVR actuels de la série NHD prennent en charge RTSP.

**Les caméras Swann prennent-elles en charge ONVIF ?**

Oui, les caméras série NHD actuelles prennent en charge ONVIF. Les modèles hérités (séries SWPRO, ADS) ne prennent généralement pas en charge ONVIF.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Lorex](lorex.md) — Homologue du segment grand public/semi-professionnel
- [Guide de connexion Hikvision](hikvision.md) — NVR Swann avec firmware Hikvision
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
