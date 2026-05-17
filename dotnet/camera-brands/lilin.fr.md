---
title: Caméras IP LILIN — URL RTSP et intégration en C# .NET
description: Modèles d'URL RTSP des caméras LILIN séries LR, Z, D, S et P pour C# .NET. Inclut endpoints snapshot et code d'intégration streaming du SDK VisioForge.
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

# Comment se connecter à une caméra IP LILIN en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**LILIN** (Merit LILIN Co., Ltd.) est un fabricant taïwanais de caméras de sécurité professionnelles dont le siège social est à New Taipei City, Taïwan. Fondée en 1980, LILIN est l'un des plus anciens fabricants de caméras IP au monde. La société est reconnue pour ses caméras de vidéosurveillance de qualité professionnelle avec des modèles d'URL RTSP distinctifs qui encodent la résolution directement dans le chemin de l'URL.

**Faits clés :**

- **Gammes de produits :** Z Series (bullet), S Series (speed dome), D Series (dôme), LR Series (IR), P Series (panoramique)
- **Protocoles pris en charge :** RTSP, ONVIF, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / pass
- **Prise en charge ONVIF :** Oui (la plupart des modèles actuels)
- **Codecs vidéo :** H.264, MJPEG
- **Modèle d'URL unique :** Résolution encodée dans le chemin RTSP (par exemple, `rtsph264720p`, `rtsph2641080p`)

!!! info "Chemins RTSP basés sur la résolution"
    LILIN utilise un modèle d'URL unique où la résolution est encodée directement dans le chemin RTSP (par exemple, `rtsph264720p` pour 720p, `rtsph2641080p` pour 1080p). Veillez à utiliser le suffixe de résolution correct pour votre modèle de caméra.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras LILIN utilisent un format de chemin RTSP basé sur la résolution :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/rtsph2641080p
```

| Paramètre | Valeur | Description |
|-----------|--------|-------------|
| `rtsph264720p` | Flux 720p | H.264 à la résolution 1280x720 |
| `rtsph2641080p` | Flux 1080p | H.264 à la résolution 1920x1080 |
| `rtsph2641024p` | Flux 1024p | H.264 à la résolution 1280x1024 (note : double barre oblique sur certains modèles) |

### Modèles de caméras

| Modèle | Résolution | URL du flux principal | Notes |
|--------|------------|-----------------------|-------|
| LR7022E4 (bullet IR) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Série LR, 1080p |
| LR7722X (bullet IR) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Série LR, 1080p |
| IPR712M4.3 (PTZ) | 1280x1024 | `rtsp://IP:554//rtsph2641024p` | Série IPR, chemin à double barre oblique |
| Série Z (bullet) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Caméras bullet extérieures |
| Série D (dôme) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Dôme intérieur/extérieur |
| Série S (speed dome) | 1920x1080 | `rtsp://IP:554/rtsph2641080p` | Speed dome PTZ |

### Formats d'URL alternatifs

Certains modèles ou versions de firmware LILIN prennent en charge ces URL alternatives :

| Modèle d'URL | Notes |
|--------------|-------|
| `rtsp://IP:554/rtsph2641080p` | 1080p standard (recommandé) |
| `rtsp://IP:554/rtsph264720p` | Flux 720p |
| `rtsp://IP:554//rtsph2641024p` | Flux 1024p (double barre oblique, certains modèles PTZ) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra LILIN avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// LILIN LR7022E4, flux principal 1080p
var uri = new Uri("rtsp://192.168.1.90:554/rtsph2641080p");
var username = "admin";
var password = "pass";
```

Pour un accès 720p, utilisez `rtsph264720p` à la place de `rtsph2641080p`.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG (VGA) | `http://IP/getimage?camera=CHANNEL&fmt=vga` | Snapshot en résolution VGA |
| Snapshot par canal | `http://IP/getimage[CHANNEL]` | Remplacez CHANNEL par le numéro de caméra |
| Snapshot rapide | `http://IP/snap` | URL de snapshot simple |
| Snapshot CGI | `http://IP/cgi-bin/net_jpeg.cgi?ch=CHANNEL` | Snapshot basé sur CGI |
| Snapshot avec auth | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Authentification basée sur l'URL |
| Image directe | `http://IP/image/CHANNEL.jpg` | Image JPEG directe par canal |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras LILIN sont livrées avec les identifiants par défaut `admin` / `pass`. Si vous avez changé le mot de passe via l'interface web, assurez-vous de mettre à jour les identifiants dans votre URL RTSP.

1. Accédez à la caméra sur `http://CAMERA_IP` dans un navigateur
2. Connectez-vous avec vos identifiants
3. Vérifiez les paramètres RTSP dans la section de configuration réseau

### Double barre oblique dans le chemin RTSP

Certains modèles LILIN, notamment la série IPR PTZ, nécessitent une double barre oblique (`//`) avant le chemin de résolution. Si une URL à simple barre oblique échoue :

- Essayez `rtsp://IP:554//rtsph2641024p` au lieu de `rtsp://IP:554/rtsph2641024p`
- Cela est couramment observé avec les modèles à résolution 1024p

### Choix du suffixe de résolution correct

Les caméras LILIN n'utilisent pas `subtype=0/1` comme beaucoup d'autres marques. À la place, la résolution du flux est sélectionnée en modifiant le chemin de l'URL :

- `rtsph264720p` pour 720p (1280x720)
- `rtsph2641080p` pour 1080p (1920x1080)
- `rtsph2641024p` pour 1024p (1280x1024)

Si vous spécifiez une résolution que votre caméra ne prend pas en charge, la connexion échouera.

### Connexion refusée sur le port 554

Vérifiez que RTSP est activé sur la caméra :

- Interface web : consultez les paramètres **Network > RTSP**
- Confirmez que le port 554 n'est pas bloqué par un pare-feu
- Le port RTSP par défaut est 554

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras LILIN ?**

L'URL la plus courante est `rtsp://admin:pass@CAMERA_IP:554/rtsph2641080p` pour le flux principal 1080p. Remplacez le suffixe de résolution (`rtsph2641080p`) par la valeur appropriée à la résolution de votre caméra.

**Les caméras LILIN prennent-elles en charge ONVIF ?**

Oui. La plupart des modèles LILIN actuels prennent en charge ONVIF, ce qui fournit une méthode alternative pour découvrir et se connecter à la caméra sans avoir besoin de modèles d'URL spécifiques à la marque.

**Pourquoi LILIN utilise-t-il un format d'URL RTSP différent ?**

LILIN encode la résolution directement dans le chemin RTSP plutôt que d'utiliser des paramètres canal/sous-type comme Dahua ou Hikvision. Il s'agit d'un choix de conception propriétaire. Le format est simple une fois que vous savez quel suffixe de résolution votre modèle de caméra prend en charge.

**Quels sont les identifiants de connexion par défaut des caméras LILIN ?**

Le nom d'utilisateur par défaut est `admin` et le mot de passe par défaut est `pass`. Il est recommandé de changer ces identifiants après la configuration initiale pour des raisons de sécurité.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion AVTech](avtech.md) — caméras industrielles taïwanaises
- [Guide de connexion BrickCom](brickcom.md) — caméras industrielles taïwanaises
- [Guide d'intégration de caméra RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — configuration des flux RTSP LILIN
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
