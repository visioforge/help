---
title: URL RTSP des caméras IP ABUS — intégration en C# .NET SDK
description: Modèles d'URL RTSP des caméras ABUS TVIP, CASA et Digi-Lan pour C# .NET. Diffusez et enregistrez avec le VisioForge Video Capture SDK multiplateforme.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP ABUS en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**ABUS** (August Bremicker Soehne KG) est une entreprise allemande de sécurité dont le siège est à Wetter, en Allemagne. Fondée en 1924, ABUS est l'un des plus grands fabricants européens de produits de sécurité, réputé pour ses serrures, ses systèmes d'alarme et la vidéosurveillance. La série **TVIP** de caméras IP est largement déployée en Europe, en particulier en Allemagne, en Autriche et dans les pays du Benelux.

**Faits clés :**

- **Gammes de produits :** TVIP (caméras IP), CASA (grand public), TV (IP analogique historique), Digi-Lan (IP plus ancien)
- **Numérotation des modèles TVIP :** TVIP1xxxx (grand public), TVIP2xxxx (2MP), TVIP4xxxx (4MP), TVIP5xxxx (5MP), TVIP6xxxx/7xxxx (spécial)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (certains modèles : root / pass)
- **Prise en charge ONVIF :** Oui (TVIP2xxxx et plus récents)
- **Codecs vidéo :** H.264 (TVIP2xxxx et plus récents), MJPEG (modèles plus anciens)

!!! info "Numérotation des modèles ABUS TVIP"
    Le numéro de modèle TVIP indique le niveau de résolution : **1xxxx** = basique/grand public, **2xxxx** = 2MP (1080p), **4xxxx** = 4MP, **5xxxx** = 5MP, **6xxxx/7xxxx** = modèles spéciaux. Cela aide à identifier quels formats d'URL et codecs prend en charge une caméra.

## Modèles d'URL RTSP

### Formats d'URL principaux

Les caméras ABUS prennent en charge plusieurs formats d'URL RTSP selon la génération du modèle :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

| Modèle d'URL | Description |
|-------------|-------------|
| `rtsp://IP:554/video.mp4` | Flux MP4 (recommandé pour les modèles H.264) |
| `rtsp://IP:554/live.sdp` | Flux SDP en direct (modèles grand public et anciens) |
| `rtsp://IP:554/video.h264` | Flux H.264 direct |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | Format VideoInput (modèles 4MP) |

### Série TVIP1xxxx (grand public)

| Modèle | URL du flux principal | Notes |
|-------|----------------|-------|
| TVIP10000 | `rtsp://IP:554/live.sdp` | MJPEG uniquement |
| TVIP10500 | `rtsp://IP:554/live.sdp` | MJPEG uniquement |
| TVIP10550 | `rtsp://IP:554/live.sdp` | MJPEG uniquement |
| TVIP11000 | `rtsp://IP:554/live.sdp` | MJPEG/H.264 |

!!! warning "Modèles MJPEG uniquement"
    Certains modèles TVIP1xxxx plus anciens (TVIP10000, TVIP10500, TVIP10550) ne prennent en charge que l'encodage MJPEG, sans H.264. Utilisez les URL de flux MJPEG HTTP listées dans la section Capture instantanée et MJPEG ci-dessous pour ces modèles.

### Série TVIP2xxxx (2MP)

| Modèle | URL du flux principal | URL alternative |
|-------|----------------|-----------------|
| TVIP20000 | `rtsp://IP:554/video.mp4` | - |
| TVIP20550 | `rtsp://IP:554/video.mp4` | - |
| TVIP21550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/live.sdp` |
| TVIP22500 | `rtsp://IP:554/video.h264` | - |

### Série TVIP4xxxx (4MP)

| Modèle | URL du flux principal | URL alternative |
|-------|----------------|-----------------|
| TVIP41500 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |
| TVIP41550 | `rtsp://IP:554/video.mp4` | `rtsp://IP:554/VideoInput/1/h264/1` |

### Série TVIP5xxxx (5MP)

| Modèle | URL du flux principal |
|-------|----------------|
| TVIP51550 | `rtsp://IP:554/video.mp4` |

### Série TVIP6xxxx / TVIP7xxxx (spéciale)

| Modèle | URL du flux principal |
|-------|----------------|
| TVIP61500 | `rtsp://IP:554/video.mp4` |
| TVIP71550 | `rtsp://IP:554/video.mp4` |

### Série CASA (grand public)

| Modèle | URL du flux principal |
|-------|----------------|
| CASA20550 | `rtsp://IP:554/live.sdp` |

### Séries historiques TV / Digi-Lan

| Modèle | URL du flux principal |
|-------|----------------|
| Digi-Lan TV7220 | `rtsp://IP:554/live.sdp` |
| TV7240-LAN | `rtsp://IP:554/live.sdp` |
| TV32500 | `rtsp://IP:554/video.mp4` |

!!! tip "Quel format d'URL essayer en premier"
    Pour les caméras ABUS, essayez d'abord `video.mp4` pour le streaming H.264, puis `live.sdp` comme solution de repli. Pour les modèles TVIP1xxxx plus anciens, `live.sdp` est généralement la seule option RTSP. Le format `VideoInput` est spécifique aux modèles TVIP4xxxx.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra ABUS avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// ABUS TVIP41550, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

Pour les modèles utilisant le format `VideoInput`, utilisez :

```csharp
// ABUS TVIP41500, format VideoInput
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
```

## URL de capture instantanée et MJPEG

### Captures JPEG

| Type | Modèle d'URL | Modèles pris en charge |
|------|-------------|------------------|
| Capture standard | `http://IP/jpg/image.jpg` | TVIP10500, TVIP10550, TVIP11000, TVIP20000, TVIP21550, TVIP51550 |
| Capture haute résolution | `http://IP/jpg/image.jpg?size=3` | TVIP10001, TVIP21050, TVIP71550 |
| Visualiseur CGI | `http://IP/cgi-bin/viewer/video.jpg?channel=CH&resolution=WxH` | CASA20550, TVIP41550, TVIP51550 |
| Capture CGI simple | `http://IP/cgi-bin/video.jpg` | Digi-Lan, modèles TV |
| CGI alternatif | `http://IP/cgi-bin/jpg/image` | TVIP20050 |
| Image de profil | `http://IP/cgi-bin/view/image?pro_CHANNEL` | TVIP20000, TVIP21500 |
| JPEG pull | `http://IP/jpeg/pull` | TVIP62000 |

### Flux MJPEG

| Type | Modèle d'URL | Modèles pris en charge |
|------|-------------|------------------|
| MJPEG standard | `http://IP/video.mjpg` | TVIP10000, TVIP11000, TVIP21500, TVIP21550, TVIP51550, TVIP71501 |
| MJPEG avec paramètres | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | TVIP31550, TVIP21501, TVIP51550, TVIP71550 |

!!! note "Paramètres MJPEG"
    Le paramètre `q` contrôle la qualité JPEG (1-100), `fps` définit la fréquence d'images, et `id` est un identifiant de session. Ajustez ces valeurs selon vos besoins de bande passante et de qualité.

## Dépannage

### Plusieurs formats d'URL fonctionnent sur la même caméra

De nombreuses caméras ABUS répondent à plusieurs formats d'URL RTSP et HTTP différents. Cela est intentionnel. Pour de meilleurs résultats :

1. Essayez d'abord `rtsp://IP:554/video.mp4` pour le streaming H.264
2. Repliez-vous sur `rtsp://IP:554/live.sdp` si `video.mp4` ne fonctionne pas
3. Utilisez `http://IP/video.mjpg` pour le streaming MJPEG en dernier recours

### Les modèles TVIP1xxxx plus anciens n'ont pas H.264

Certaines caméras TVIP1xxxx de première génération (TVIP10000, TVIP10500, TVIP10550) ne prennent en charge que l'encodage MJPEG. Ces caméras ne répondront pas aux URL RTSP `video.mp4` ou `video.h264`. Utilisez à la place le flux MJPEG HTTP (`http://IP/video.mjpg`) ou l'URL RTSP `live.sdp`.

### Les identifiants par défaut varient selon le modèle

La plupart des caméras ABUS utilisent `admin` / `admin` comme identifiants par défaut. Cependant, certains modèles utilisent par défaut `root` / `pass`. Si l'authentification échoue avec un jeu d'identifiants, essayez l'autre. Consultez la documentation de la caméra pour connaître les identifiants par défaut spécifiques.

### Décodage du numéro de modèle TVIP

Si vous ne savez pas quel format d'URL utiliser, le numéro de modèle TVIP fournit des indications :

- **TVIP1xxxx :** Commencez par `live.sdp`, peut être MJPEG uniquement
- **TVIP2xxxx :** Commencez par `video.mp4`, la plupart prennent en charge H.264
- **TVIP4xxxx :** Commencez par `video.mp4`, essayez aussi `VideoInput/1/h264/1`
- **TVIP5xxxx+ :** Commencez par `video.mp4`

### Paramètre de résolution de capture

Pour l'URL `jpg/image.jpg?size=N`, le paramètre `size` contrôle la résolution :

- `size=1` = Résolution la plus basse
- `size=2` = Résolution moyenne
- `size=3` = Résolution la plus haute

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras ABUS ?**

Pour la plupart des caméras ABUS actuelles (TVIP2xxxx et plus récentes), l'URL par défaut est `rtsp://admin:admin@CAMERA_IP:554/video.mp4`. Pour les modèles grand public plus anciens (TVIP1xxxx), essayez plutôt `rtsp://admin:admin@CAMERA_IP:554/live.sdp`.

**Les caméras ABUS prennent-elles en charge ONVIF ?**

Oui. Les caméras ABUS à partir de la génération TVIP2xxxx prennent en charge ONVIF, qui fournit une découverte et un streaming de caméras standardisés. Les modèles TVIP1xxxx plus anciens peuvent ne pas prendre en charge ONVIF.

**Puis-je utiliser le streaming MJPEG avec les caméras ABUS ?**

Oui. La plupart des caméras ABUS prennent en charge le streaming MJPEG via `http://CAMERA_IP/video.mjpg`. Cela est particulièrement utile pour les modèles TVIP1xxxx plus anciens qui ne prennent pas en charge l'encodage H.264. MJPEG utilise plus de bande passante que H.264 mais est compatible avec un plus large éventail de logiciels.

**Que signifient les numéros de modèle ABUS TVIP ?**

Le nombre à cinq chiffres après « TVIP » indique le niveau de résolution de la caméra : 1xxxx = basique/grand public, 2xxxx = 2MP (1080p), 4xxxx = 4MP, 5xxxx = 5MP, et 6xxxx/7xxxx = modèles spéciaux. Les nombres plus élevés indiquent généralement un matériel plus récent avec une prise en charge plus large des protocoles et des codecs.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion INSTAR](instar.md) — Caméras allemandes grand public / maison connectée
- [Intégration de caméras IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuration de périphérique ONVIF ABUS
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
