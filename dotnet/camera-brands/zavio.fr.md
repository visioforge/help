---
title: Guide d'URL RTSP Zavio pour l'intégration en C# .NET SDK
description: Modèles d'URL RTSP pour les caméras Zavio bullet, dôme et PTZ en C# .NET. Intégration compatible ONVIF avec le SDK VisioForge pour tous les modèles Zavio.
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

# Comment se connecter à une caméra IP Zavio en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Zavio** (Zavio Inc.) est un fabricant taïwanais de caméras IP dont le siège est à Hsinchu, à Taïwan. Zavio est connue pour ses caméras réseau de qualité professionnelle avec des modèles d'URL distinctifs qui incluent à la fois des chemins de flux directs et des chemins basés sur des profils. La société cible les marchés de la sécurité PME et professionnels avec une gamme de modèles de caméras bullet, dôme, fixes, mini et PTZ.

**Faits clés :**

- **Gammes de produits :** B (bullet), D (dôme), F (fixe/box), M (mini), P (PTZ/pan-tilt), V (anti-vandalisme)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI, MJPEG
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (la plupart des modèles)
- **Codecs vidéo :** H.264, MPEG-4, MJPEG
- **Doubles modèles d'URL :** Certains modèles utilisent `/video.mp4`, d'autres utilisent `/video.proN` (basé sur les profils)

!!! tip "URL basées sur les profils"
    Les caméras Zavio prennent en charge des URL basées sur les profils. Utilisez `/video.pro1` pour le profil principal et `/video.pro2` pour le profil secondaire. Les profils disponibles dépendent de la configuration de votre caméra.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Zavio prennent en charge deux modèles d'URL RTSP principaux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video.mp4
```

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//video.pro1
```

| Chemin d'URL | Description |
|----------|-------------|
| `/video.mp4` | Flux principal MP4 (format le plus courant) |
| `//video.pro1` | Profil 1 / flux principal (préfixe à double barre) |
| `//video.pro2` | Profil 2 / sous-flux (préfixe à double barre) |
| `//video.h264` | Flux H.264 direct (certains modèles) |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| B5110 (bullet) | Bullet | `rtsp://IP//video.pro1` | Basé sur les profils, prend également en charge `//video.h264` |
| B5210 (bullet) | Bullet | `rtsp://IP//video.pro1` | Basé sur les profils |
| B7110 (bullet) | Bullet | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| B7210 (bullet) | Bullet | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D3100 (dôme) | Dôme | `rtsp://IP//video.pro1` | Basé sur les profils |
| D3200 (dôme) | Dôme | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D4210 (dôme) | Dôme | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D50E (dôme) | Dôme | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D510E (dôme) | Dôme | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D520E (dôme) | Dôme | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| D7111 (dôme) | Dôme | `rtsp://IP:554//video.pro2` | Sous-flux profil 2 |
| D7210 (dôme) | Dôme | `rtsp://IP:554//video.pro2` | Sous-flux profil 2 |
| F1100 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F1105 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F1150 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F210A (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3100 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3102 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3110 (fixe) | Fixe | `rtsp://IP:554//video.pro2` | Sous-flux profil 2 |
| F3115 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F312A (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3201 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3206 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3210 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F3215 (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F511E (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F520IE (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F521E (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| F731E (fixe) | Fixe | `rtsp://IP:554/video.mp4` | Flux principal MP4 |
| M510W (mini) | Mini | `rtsp://IP:554/video.mp4` | Mini caméra sans fil |
| M511E (mini) | Mini | `rtsp://IP:554/video.mp4` | Mini caméra |
| P5110 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5115 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |
| P5210 (PTZ) | PTZ | `rtsp://IP:554/video.mp4` | Pan-tilt-zoom |

### Formats d'URL alternatifs

Certains modèles Zavio prennent en charge ces URL alternatives :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/video.mp4` | Flux MP4 (recommandé pour la plupart des modèles) |
| `rtsp://IP//video.pro1` | Profil 1, flux principal |
| `rtsp://IP:554//video.pro2` | Profil 2, sous-flux |
| `rtsp://IP//video.h264` | Flux H.264 direct (B5110 et similaires) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Zavio avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Zavio B7110, flux principal MP4
var uri = new Uri("rtsp://192.168.1.90:554/video.mp4");
var username = "admin";
var password = "admin";
```

Pour les caméras basées sur les profils, utilisez `//video.pro1` pour le flux principal ou `//video.pro2` pour le sous-flux.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture par profil | `http://IP/cgi-bin/view/image?pro_CHANNEL` | Capture par numéro de profil |
| Capture JPEG | `http://IP/jpg/image.jpg` | Capture JPEG standard |
| JPEG redimensionné | `http://IP/jpg/image.jpg?size=3` | JPEG avec paramètre de taille |
| JPEG CGI | `http://IP/cgi-bin/jpg/image` | Capture JPEG basée sur CGI |
| Flux MJPEG | `http://IP/video.mjpg` | Flux MJPEG continu |
| MJPEG (qualité/FPS) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | MJPEG avec contrôle qualité et FPS |
| Flux HTTP par profil | `http://IP/stream?uri=video.proN` | Flux HTTP basé sur le profil |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras Zavio sont livrées avec les identifiants par défaut `admin` / `admin`. Si la caméra a été configurée avec des identifiants différents :

1. Accédez à la caméra à l'adresse `http://CAMERA_IP` dans un navigateur
2. Connectez-vous et vérifiez les paramètres **Network > RTSP**
3. Vérifiez que l'authentification RTSP est activée et que vos identifiants sont corrects

### Choisir entre /video.mp4 et /video.proN

Les caméras Zavio ont deux familles d'URL. Le choix correct dépend de votre modèle :

- **La plupart des modèles** (B7110, F210A, F312A, F520IE, F521E, F731E, etc.) : utilisez `/video.mp4`
- **Modèles plus anciens ou basés sur les profils** (B5110, B5210, D3100) : utilisez `//video.pro1`
- Si un format échoue, essayez l'autre

### Double barre dans les URL de profil

Les URL Zavio basées sur les profils nécessitent une double barre (`//`) avant `video.proN`. Ceci est intentionnel :

- Correct : `rtsp://IP//video.pro1`
- Incorrect : `rtsp://IP/video.pro1`

Si vous omettez la double barre sur un modèle basé sur les profils, la connexion peut échouer.

### Pas de vidéo avec le codec MPEG-4

Certains anciens modèles Zavio utilisent par défaut l'encodage MPEG-4. Si vous rencontrez des problèmes de codec :

- Connectez-vous à l'interface web de la caméra
- Changez le codec vidéo en **H.264** dans la configuration du flux
- Utilisez l'URL `/video.mp4` ou `//video.pro1` après avoir modifié le paramètre

### Connexion refusée sur le port 554

Vérifiez que RTSP est activé sur la caméra :

- Interface web : vérifiez les paramètres **Network > RTSP**
- Confirmez que le port 554 n'est pas bloqué par un pare-feu
- Le port RTSP par défaut est 554

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Zavio ?**

L'URL la plus courante est `rtsp://admin:admin@CAMERA_IP:554/video.mp4` pour le flux principal MP4. Pour les modèles basés sur les profils, utilisez plutôt `rtsp://admin:admin@CAMERA_IP//video.pro1`.

**Les caméras Zavio prennent-elles en charge ONVIF ?**

Oui. La plupart des modèles Zavio prennent en charge ONVIF, qui fournit une méthode standardisée pour la découverte de caméra et le streaming sans nécessiter de modèles d'URL spécifiques à la marque.

**Quelle est la différence entre /video.mp4 et /video.pro1 ?**

`/video.mp4` est un chemin de flux direct utilisé par la plupart des nouveaux modèles Zavio. `//video.pro1` et `//video.pro2` sont des chemins basés sur les profils qui référencent les profils de flux configurés dans l'interface web de la caméra. Le profil 1 est généralement le flux principal (haute résolution), et le profil 2 est généralement le sous-flux (résolution plus faible).

**Quels sont les identifiants de connexion par défaut pour les caméras Zavio ?**

Le nom d'utilisateur par défaut est `admin` et le mot de passe par défaut est `admin`. Il est fortement recommandé de modifier ces identifiants après la configuration initiale.

**Puis-je contrôler la qualité et la fréquence d'images MJPEG ?**

Oui. Les caméras Zavio prennent en charge des paramètres MJPEG dans l'URL. Utilisez `http://IP/video.mjpg?q=30&fps=33&id=0.5` pour spécifier la qualité (`q`), les images par seconde (`fps`) et l'identifiant de flux (`id`).

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Edimax](edimax.md) — Caméras PME taïwanaises
- [Capture ONVIF avec post-traitement](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de capture ONVIF Zavio
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
