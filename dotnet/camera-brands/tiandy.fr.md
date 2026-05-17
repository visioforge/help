---
title: Caméra IP Tiandy en C# .NET — guide RTSP et ONVIF complet
description: Connectez les caméras IP Tiandy (TC-C, TC-NC, TC-A, NVR TC-R) à C# / .NET via RTSP et ONVIF. URL de flux, identifiants, configurations H.265. Exemple de code.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Tiandy en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Tiandy Technologies** (Tiandy Technologies Co., Ltd.) est un fabricant chinois de vidéosurveillance dont le siège est à Tianjin, en Chine. Fondée en 1994, Tiandy est l'un des plus grands fabricants chinois d'équipements de sécurité et s'étend rapidement sur les marchés internationaux en Asie, au Moyen-Orient, en Afrique et en Amérique latine. Tiandy est spécialisée dans les caméras IP propulsées par l'IA, les NVR et les solutions intégrées de gestion vidéo.

**Faits clés :**

- **Gammes de produits :** TC-C (caméras IP actuelles), TC-NC (IP héritées), TC-A (analyses IA), TC-R (NVR), TC-NR (enregistreurs réseau)
- **Prise en charge des protocoles :** RTSP, ONVIF Profile S/T, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 1111 (anciens modèles) ou admin / admin123 (varie selon la région)
- **Prise en charge ONVIF :** Oui (modèles actuels)
- **Codecs vidéo :** H.264, H.265 (SuperH.265), MJPEG
- **Fonctionnalités IA :** Smart H.265+, détection de visages, protection de périmètre, comptage de personnes

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Tiandy utilisent une structure d'URL RTSP basée sur le canal et le flux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de caméra (1 pour les caméras autonomes) |
| `subtype` | 0 | Flux principal (résolution la plus élevée) |
| `subtype` | 1 | Sous-flux (résolution plus faible) |

!!! info "Format d'URL compatible Dahua"
    De nombreuses caméras Tiandy utilisent le même format d'URL RTSP `cam/realmonitor` que les caméras Dahua. Si vous êtes familier avec l'intégration Dahua, les mêmes modèles d'URL peuvent fonctionner avec Tiandy. Consultez notre [guide de connexion Dahua](dahua.md) pour plus de détails.

### Formats d'URL alternatifs

| Modèle d'URL | Description |
|-------------|-------------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Compatible Dahua (de nombreux modèles) |
| `rtsp://IP:554/live/ch0` | Flux principal (format hérité) |
| `rtsp://IP:554/live/ch1` | Sous-flux (format hérité) |
| `rtsp://IP:554/media/video1` | Compatible Uniview (certains modèles) |
| `rtsp://IP:554/Streaming/Channels/101` | Compatible Hikvision (certains modèles OEM) |
| `rtsp://IP:554/h264` | Chemin de flux H.264 simple |

### Modèles de caméras

| Série de modèles | Résolution | URL du flux principal | Audio |
|-------------|-----------|----------------|-------|
| TC-C32JN (bullet 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Non |
| TC-C34JN (bullet 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Non |
| TC-C35JN (bullet 5MP) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Non |
| TC-C38JN (bullet 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| TC-C32DN (dôme 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Non |
| TC-C34DN (dôme 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| TC-C32EP (turret 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| TC-C34EP (turret 4MP) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| TC-A32E2T (IA 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| TC-C32WP (WiFi 2MP) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |

### URL des canaux NVR

Pour les NVR Tiandy (TC-R3100, TC-R3200, série TC-NR) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Caméra 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Caméra N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Tiandy avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Tiandy TC-C34JN (bullet 4MP), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `subtype=1` à la place de `subtype=0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Nécessite une authentification digest |
| Flux MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continu |

## Dépannage

### Plusieurs formats d'URL

Les caméras Tiandy peuvent utiliser différents formats d'URL RTSP selon la version du firmware et le modèle. Si un format ne fonctionne pas, essayez les alternatives dans cet ordre :

1. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (compatible Dahua, le plus courant)
2. `rtsp://IP:554/live/ch0` (format Tiandy hérité)
3. `rtsp://IP:554/h264` (chemin simple)

### Les identifiants par défaut varient

Les mots de passe par défaut Tiandy diffèrent selon le modèle et la région. Les valeurs par défaut courantes incluent :

- `admin` / `1111`
- `admin` / `admin123`
- `admin` / `123456`

Si aucun ne fonctionne, la caméra peut nécessiter une activation initiale via l'interface web ou l'utilitaire EasyLive de Tiandy.

### Codec SuperH.265

Le SuperH.265 de Tiandy est une optimisation propriétaire qui produit des flux H.265/HEVC standards. Aucun décodeur spécial n'est requis. Le SDK VisioForge gère nativement les flux H.265.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Tiandy ?**

La plupart des caméras Tiandy utilisent `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` pour le flux principal, qui est le même format que les caméras Dahua. Certains modèles plus anciens utilisent plutôt `rtsp://IP:554/live/ch0`.

**Les caméras Tiandy sont-elles des OEM Dahua ?**

Non. Tiandy est un fabricant indépendant avec son propre matériel et firmware. Cependant, certains firmwares Tiandy utilisent le même format d'URL RTSP que Dahua (`cam/realmonitor`), ce qui est courant chez plusieurs fabricants chinois de vidéosurveillance.

**Les caméras Tiandy prennent-elles en charge ONVIF ?**

Oui. Les modèles Tiandy actuels prennent en charge ONVIF Profile S et Profile T. ONVIF doit être activé dans l'interface web de la caméra sous les paramètres réseau. Certains modèles nécessitent la création d'un compte utilisateur ONVIF séparé.

**Quelle série de caméras Tiandy choisir ?**

**TC-C** est la série grand public actuelle. Le numéro après « TC-C3 » indique la résolution : **2** = 2MP, **4** = 4MP, **5** = 5MP, **8** = 4K. Les lettres suffixes indiquent le facteur de forme : **JN** = bullet, **DN** = dôme, **EP** = turret/eyeball, **WP** = WiFi.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Format d'URL similaire
- [Guide de connexion Uniview](uniview.md) — Une autre marque chinoise majeure de vidéosurveillance
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
