---
title: Format d'URL RTSP Axis — caméras IP et intégration C# .NET
description: Connectez les caméras Axis Communications en C# .NET avec modèles d'URL RTSP, API VAPIX et exemples de code pour les séries M, P, Q et F.
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
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Axis en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Axis Communications** est un fabricant suédois largement considéré comme le pionnier des caméras réseau, ayant créé la première caméra IP au monde en 1996. Dont le siège est à Lund, en Suède et désormais filiale de Canon, Axis produit des caméras IP haut de gamme, des encodeurs et des produits audio réseau principalement pour le marché de la vidéosurveillance professionnel et entreprise.

**Faits clés :**

- **Gammes de produits :** Série M (compactes/mini), série P (fixes), série Q (professionnelles), série F (modulaires), série V (anti-vandalisme), caméras PTZ
- **Prise en charge des protocoles :** ONVIF Profile S/G/T, RTSP, VAPIX (API HTTP propriétaire Axis), HTTP/MJPEG
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** root / (défini lors de la configuration initiale ; firmware plus ancien : root / pass)
- **Prise en charge ONVIF :** Complète — Axis était un membre fondateur d'ONVIF
- **Codecs vidéo :** H.264, H.265 (modèles plus récents), MJPEG
- **Caractéristiques uniques :** API HTTP VAPIX pour un contrôle complet de la caméra, ACAP (Axis Camera Application Platform)

## Modèles d'URL RTSP

Les caméras Axis utilisent le chemin RTSP `axis-media/media.amp` avec des paramètres optionnels pour le contrôle de la résolution et du codec.

### Format d'URL

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/axis-media/media.amp
```

### URL RTSP principales

| Série de modèles | URL RTSP | Codec | Audio |
|-------------|----------|-------|-------|
| Tous les modèles modernes | `rtsp://IP:554/axis-media/media.amp` | H.264 (par défaut) | Possible |
| Tous les modèles modernes | `rtsp://IP:554/axis-media/media.amp?videocodec=h264` | H.264 (explicite) | Possible |
| Tous les modèles modernes | `rtsp://IP:554/axis-media/media.amp?videocodec=h265` | H.265 | Possible |
| Profil ONVIF | `rtsp://IP:554/onvif-media/media.amp` | H.264 | Oui |
| Modèles historiques | `rtsp://IP:554/mpeg4/media.amp` | MPEG-4 | Possible |

### Sélection de profil de flux

Les caméras Axis prennent en charge des profils de flux nommés qui peuvent être sélectionnés via un paramètre d'URL :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Quality` | Profil haute qualité |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Balanced` | Profil équilibré |
| `rtsp://IP:554/axis-media/media.amp?streamprofile=Bandwidth` | Profil bande passante faible |
| `rtsp://IP:554/axis-media/media.amp?resolution=1920x1080` | Résolution explicite |
| `rtsp://IP:554/axis-media/media.amp?resolution=640x480` | Résolution inférieure |
| `rtsp://IP:554/axis-media/media.amp?fps=15` | Limite de fréquence d'images |

### Modèles multi-canaux (encodeurs, multi-capteurs)

Pour les appareils multi-canaux comme les encodeurs vidéo (M7001, P7214) et les caméras multi-capteurs :

| Appareil | URL RTSP | Canal |
|--------|----------|---------|
| Canal 1 | `rtsp://IP:554/axis-media/media.amp?camera=1` | 1 |
| Canal 2 | `rtsp://IP:554/axis-media/media.amp?camera=2` | 2 |
| Canal 3 | `rtsp://IP:554/axis-media/media.amp?camera=3` | 3 |
| Canal 4 | `rtsp://IP:554/axis-media/media.amp?camera=4` | 4 |

### Formats d'URL historiques

Les anciennes caméras Axis (série 200, séries 1000 anciennes) peuvent nécessiter ces formats :

| Modèle d'URL | Modèles | Notes |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4/media.amp` | 200, 205, 206, 207 | Flux MPEG-4 |
| `http://IP/axis-cgi/mjpg/video.cgi` | Tous les modèles | MJPEG sur HTTP |
| `http://IP/mjpg/video.mjpg` | Série 200 | Flux MJPEG direct |
| `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | Multi-canal | Canal spécifique |
| `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | Tous les modèles | Résolution spécifique |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Axis avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Axis, flux principal H.264
var uri = new Uri("rtsp://192.168.1.50:554/axis-media/media.amp");
var username = "root";
var password = "YourPassword";
```

Pour accéder au sous-flux, ajoutez le paramètre `?resolution=640x480`.

### Découverte ONVIF

Axis était membre fondateur d'ONVIF et possède une conformité ONVIF de référence dans l'industrie. Consultez le [guide d'intégration ONVIF](../mediablocks/Sources/index.md) pour des exemples de code de découverte.

## URL de capture instantanée et MJPEG (API VAPIX)

Les caméras Axis fournissent l'API HTTP VAPIX, plus riche en fonctionnalités que la plupart des autres marques :

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/axis-cgi/jpg/image.cgi` | Image actuelle |
| Capture (dimensionnée) | `http://IP/axis-cgi/jpg/image.cgi?resolution=1920x1080` | Résolution spécifique |
| Capture (avec superposition) | `http://IP/axis-cgi/jpg/image.cgi?date=1&clock=1` | Superposition date/heure |
| Capture (sélection caméra) | `http://IP/axis-cgi/jpg/image.cgi?camera=1` | Appareil multi-canal |
| Capture simple | `http://IP/jpg/image.jpg` | Capture JPEG de base |
| Capture dimensionnée | `http://IP/jpg/image.jpg?size=3` | Taille prédéfinie (1-5) |
| Flux MJPEG | `http://IP/axis-cgi/mjpg/video.cgi` | MJPEG continu |
| MJPEG (résolution) | `http://IP/axis-cgi/mjpg/video.cgi?resolution=640x480` | MJPEG dimensionné |
| MJPEG (direct) | `http://IP/mjpg/video.mjpg` | MJPEG direct (historique) |

## Dépannage

### Audio « Possible » vs « Oui »

Axis marque la prise en charge audio comme « Possible » sur de nombreux flux RTSP parce que la disponibilité audio dépend du fait que le modèle de caméra dispose d'un microphone intégré ou d'une entrée audio externe. L'URL RTSP est la même que l'audio soit présent ou non — le SDK détectera et utilisera automatiquement l'audio s'il est disponible.

### Erreurs « 401 Unauthorized »

- Les caméras Axis utilisent par défaut l'authentification digest pour RTSP
- Assurez-vous d'utiliser les bons identifiants (le nom d'utilisateur par défaut est `root`, et non `admin`)
- Sur les firmware plus récents, le mot de passe doit respecter des exigences de complexité (minimum 8 caractères)

### Flux MPEG-4 non disponible sur les modèles plus récents

Les caméras Axis modernes (firmware 5.x+) ont abandonné la prise en charge MPEG-4. Utilisez `/axis-media/media.amp` (H.264) au lieu de `/mpeg4/media.amp`.

### La résolution ne correspond pas à la sortie attendue

Les caméras Axis négocient la résolution dynamiquement. Pour forcer une résolution spécifique, ajoutez le paramètre `resolution` :
`rtsp://IP:554/axis-media/media.amp?resolution=1920x1080`

### Connexions à des encodeurs multi-canaux

Lors de la connexion à un encodeur Axis (M7001, P7214, etc.), vous devez spécifier le paramètre caméra/canal. Sans lui, vous obtenez le canal 1 par défaut.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Axis ?**

L'URL standard est `rtsp://root:password@CAMERA_IP:554/axis-media/media.amp`. Cela fonctionne pour pratiquement toutes les caméras Axis modernes (séries M, P, Q, F, V). Le nom d'utilisateur par défaut est `root` (et non `admin` comme la plupart des autres marques).

**Comment basculer entre H.264 et H.265 sur les caméras Axis ?**

Ajoutez le paramètre `videocodec` à l'URL RTSP : `rtsp://IP:554/axis-media/media.amp?videocodec=h265` pour H.265, ou `videocodec=h264` pour H.264. Notez que H.265 n'est disponible que sur les modèles Axis plus récents avec des chipsets Artpec-7 ou plus récents.

**Puis-je contrôler la qualité du flux via l'URL RTSP ?**

Oui. Axis prend en charge plusieurs paramètres d'URL : `resolution` (par exemple, `1920x1080`), `fps` (fréquence d'images), `compression` (0-100), et `streamprofile` (profils nommés configurés dans la caméra). Exemple : `rtsp://IP:554/axis-media/media.amp?resolution=1280x720&fps=15`.

**Pourquoi Axis utilise-t-il « root » comme nom d'utilisateur par défaut au lieu d'« admin » ?**

Les caméras Axis fonctionnent sous Linux embarqué, et suivant les conventions Unix, l'utilisateur administratif s'appelle `root`. Cela diffère de la plupart des autres marques de caméras qui utilisent `admin`.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Bosch](bosch.md) — Pair en vidéosurveillance entreprise
- [Guide de connexion Hanwha Vision](hanwha.md) — Pair en vidéosurveillance entreprise
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
