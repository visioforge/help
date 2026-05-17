---
title: Connecter une caméra IP Arecont Vision en C# .NET — RTSP
description: Modèles d'URL RTSP Arecont Vision pour C# .NET. Intégrez les caméras AV Series, MegaDome et SurroundVideo panoramiques avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP Arecont Vision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Arecont Vision** (aujourd'hui partie de Costar Group) est une entreprise américaine de caméras IP fondée en 2003 et basée à Glendale, en Californie. Arecont Vision a été un pionnier des caméras IP mégapixel et est connue pour ses modèles haute résolution (jusqu'à 20MP) et ses caméras panoramiques multi-capteurs. La société a été rachetée par **Costar Group** en 2019, qui continue à prendre en charge et à fabriquer les produits Arecont Vision.

**Faits clés :**

- **Gammes de produits :** AV Series (fixes mégapixel), MegaDome, MegaBall, SurroundVideo (panoramique multi-capteurs), MicroDome
- **Prise en charge des protocoles :** RTSP, ONVIF (modèles plus récents), PSIA, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Variables selon le modèle (beaucoup sont livrés sans mot de passe par défaut)
- **Prise en charge ONVIF :** Oui (modèles plus récents), prise en charge PSIA sur la plupart des modèles
- **Codecs vidéo :** H.264, MJPEG (modèles plus anciens MJPEG uniquement)

!!! info "Arecont Vision et Costar Group"
    Arecont Vision a été racheté par Costar Group en 2019. Les caméras Arecont existantes continuent à utiliser les mêmes formats d'URL RTSP et PSIA. Les modèles plus récents sous marque Costar peuvent utiliser un firmware mis à jour, mais maintiennent des modèles d'URL rétrocompatibles.

## Modèles d'URL RTSP

### Formats d'URL standard

Les caméras Arecont Vision prennent en charge plusieurs modèles d'URL RTSP selon la génération du modèle et le protocole configuré :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp
```

| Modèle d'URL | Protocole | Description |
|-------------|----------|-------------|
| `rtsp://IP:554/h264.sdp` | H.264 | Flux H.264 standard (la plupart des modèles actuels) |
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | Flux H.264 basé sur PSIA |
| `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Flux MPEG-4 historique (anciens modèles) |

### Flux H.264 avec paramètres de ROI

Les caméras Arecont prennent en charge des paramètres optionnels de région d'intérêt (ROI) pour personnaliser la sortie du flux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264.sdp?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0
```

| Paramètre | Valeurs | Description |
|-----------|--------|-------------|
| `res` | `full`, `half` | Résolution du flux (pleine ou demi de la résolution du capteur) |
| `x0`, `y0` | 0 - max | Coin supérieur gauche de la région d'intérêt |
| `x1`, `y1` | 0 - max | Coin inférieur droit de la région d'intérêt |
| `quality` | 1 - 21 | Facteur de qualité JPEG/H.264 (plus bas = qualité plus élevée) |
| `doublescan` | 0, 1 | Activer le mode double-scan pour une qualité d'image améliorée |

!!! tip "Les paramètres ROI sont optionnels"
    Les paramètres ROI (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) sont optionnels et peuvent être complètement omis pour un streaming en plein cadre. Utilisez `rtsp://IP:554/h264.sdp` sans paramètres pour la connexion la plus simple.

### Modèles de caméras

| Série de modèles | Résolution | URL du flux principal | Codec |
|-------------|-----------|----------------|-------|
| AV Series (mégapixel générique) | Variable | `rtsp://IP:554/h264.sdp` | H.264 |
| AV2100 (2MP) | 1600x1200 | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 |
| AV5115/AV5125 | 2592x1944 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV8185DN (8MP multi-capteurs) | 6400x1200 | `rtsp://IP:554/h264.sdp` | H.264 |
| AV10005/AV10115 (10MP) | 3648x2752 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| AV20185 (20MP multi-capteurs) | 10240x1536 | `rtsp://IP:554/h264.sdp` | H.264 |
| Série MegaDome | Variable | `rtsp://IP:554/h264.sdp` | H.264 |
| Série MegaBall | Variable | `rtsp://IP:554/h264.sdp` | H.264 |
| Série MicroDome | Variable | `rtsp://IP:554/h264.sdp` | H.264 |

### URL de streaming PSIA

Les modèles qui prennent en charge le protocole PSIA peuvent utiliser le format d'URL suivant :

| Canal | URL |
|---------|-----|
| Canal 0 (par défaut) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` |
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Arecont Vision avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Arecont Vision AV Series, flux principal H.264
var uri = new Uri("rtsp://192.168.1.90:554/h264.sdp");
var username = "admin";
var password = "YourPassword";
```

Pour le streaming basé sur PSIA, utilisez plutôt l'URL PSIA :

```csharp
// Arecont Vision via le protocole PSIA
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
```

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/img.jpg` | La plupart des modèles, nécessite une authentification basique |
| Capture JPEG (alt) | `http://IP/Jpeg/CamImg.jpg` | URL de capture alternative |
| Capture configurable | `http://IP/image?res=half&x0=0&y0=0&x1=1600&y1=1200&quality=15&doublescan=0` | Capture avec paramètres ROI |
| Flux MJPEG | `http://IP/mjpeg?res=full&x0=0&y0=0&x1=100%&y1=100%&quality=12&doublescan=0` | Flux MJPEG continu |

### URL de capture multi-capteurs (SurroundVideo)

Pour les caméras SurroundVideo et autres caméras multi-capteurs, chaque capteur a sa propre URL de capture :

| Capteur | Modèle d'URL | Notes |
|--------|-------------|-------|
| Canal 1 | `http://IP/image1?res=half&x1=0&y1=0` | Premier capteur |
| Canal 2 | `http://IP/image2?res=half&x1=0&y1=0` | Deuxième capteur |
| Canal 3 | `http://IP/image3?res=half&x1=0&y1=0` | Troisième capteur |
| Canal 4 | `http://IP/image4?res=half&x1=0&y1=0` | Quatrième capteur |

## Dépannage

### Les paramètres ROI causent des problèmes

Les caméras Arecont ont des paramètres de région d'intérêt uniques (`res`, `x0`, `y0`, `x1`, `y1`, `quality`, `doublescan`) intégrés dans leurs URL. Si vous rencontrez des problèmes de connexion :

1. Supprimez tous les paramètres ROI et utilisez l'URL minimale : `rtsp://IP:554/h264.sdp`
2. Vérifiez que la résolution de la caméra prend en charge les coordonnées ROI demandées
3. Utilisez `res=full` pour un streaming pleine résolution ou `res=half` pour une bande passante réduite

### MJPEG uniquement sur les anciens modèles

Certains anciens modèles Arecont (AV1300, AV2100, AV3100) ne prennent en charge que l'encodage MJPEG et n'ont pas de capacité H.264. Pour ces caméras :

- Utilisez l'URL RTSP MPEG-4 : `rtsp://IP:554/cam1/mpeg4`
- Ou utilisez le flux MJPEG HTTP : `http://IP/mjpeg`

### PSIA vs RTSP direct

Les caméras Arecont prennent en charge à la fois des URL RTSP directes et basées sur PSIA. Si l'un des formats ne fonctionne pas :

- Essayez le format alternatif (basculez entre `h264.sdp` et `PSIA/Streaming/channels/0`)
- Vérifiez que la version du firmware de la caméra prend en charge le protocole choisi
- Vérifiez que PSIA est activé dans l'interface web de la caméra

### Expiration de la connexion

Les caméras Arecont peuvent prendre plus de temps que d'autres marques à établir une session RTSP, en particulier pour les modèles haute résolution (10MP+) :

- Augmentez votre délai d'expiration de connexion à au moins 10 secondes
- Pour les modèles multi-capteurs, connectez-vous à des canaux individuels plutôt qu'au flux composite pour une latence plus faible

## FAQ

**Quel format d'URL RTSP utilise Arecont Vision ?**

L'URL RTSP principale est `rtsp://IP:554/h264.sdp` pour le streaming H.264. Le streaming basé sur PSIA utilise `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. Les anciens modèles peuvent utiliser `rtsp://IP:554/cam1/mpeg4` pour les flux MPEG-4.

**Les caméras Arecont Vision sont-elles toujours prises en charge après l'acquisition par Costar ?**

Oui. Costar Group a racheté Arecont Vision en 2019 et continue à fabriquer et à prendre en charge les produits Arecont Vision. Les caméras existantes restent entièrement fonctionnelles, et des mises à jour de firmware sont disponibles via les canaux de support Costar.

**Comment me connecter à une caméra SurroundVideo multi-capteurs ?**

Les caméras SurroundVideo exposent les canaux de capteurs individuels via des URL numérotées. Pour les captures, utilisez `http://IP/image1`, `http://IP/image2`, etc. Pour RTSP, utilisez l'URL H.264 standard avec le composite panoramique complet, ou des URL PSIA basées sur les canaux pour les capteurs individuels.

**Les caméras Arecont Vision prennent-elles en charge ONVIF ?**

Les modèles Arecont Vision plus récents prennent en charge ONVIF Profile S. Les modèles plus anciens reposent plutôt sur le protocole PSIA. Vérifiez les spécifications de votre caméra ou l'interface web pour confirmer la disponibilité d'ONVIF.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion GeoVision](geovision.md) — Caméras de vidéosurveillance professionnelle
- [Capture ONVIF avec post-traitement](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de capture ONVIF Arecont
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
