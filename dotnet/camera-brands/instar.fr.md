---
title: URL RTSP des caméras IP INSTAR et guide de connexion C# .NET
description: Modèles d'URL RTSP des caméras INSTAR IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx HD pour C# .NET. Diffusez et enregistrez avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP INSTAR en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**INSTAR** (INSTAR Deutschland GmbH) est un fabricant allemand de caméras IP dont le siège est à Hanau, en Allemagne. INSTAR se spécialise dans les caméras IP intérieures et extérieures abordables pour les marchés grand public et petites entreprises, avec une forte présence en Europe, en particulier en Allemagne. Les caméras INSTAR sont connues pour leurs options de stockage local, leur intégration MQTT à la maison connectée (Home Assistant, ioBroker, Node-RED) et leur configuration simple.

**Faits clés :**

- **Gammes de produits :** IN-2xxx/3xxx/4xxx (VGA historique), IN-5xxx (720p), IN-6xxx (HD 720p), IN-7xxx (Full HD 1080p), IN-8xxx (1080p+ actuel), IN-9xxx (4K/WQHD actuel)
- **Prise en charge des protocoles :** RTSP, HTTP, ONVIF (IN-6xxx et plus récents), MQTT
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / instar (variable selon le modèle)
- **Prise en charge ONVIF :** Oui (séries IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx)
- **Codecs vidéo :** H.265 (IN-9xxx), H.264 (IN-6xxx/7xxx/8xxx), MPEG-4 (IN-5xxx), MJPEG (IN-2xxx/3xxx/4xxx historiques)

## Modèles d'URL RTSP

Les caméras INSTAR utilisent un format d'URL distinctif avec une **double barre oblique** avant le numéro de flux.

### Format d'URL (modèles HD)

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//11
```

!!! warning "Double barre oblique"
    Les caméras HD INSTAR utilisent une **double barre oblique** (`//`) avant le numéro de flux. L'utilisation d'une barre unique entraînera l'échec de la connexion.

### Modèles HD (IN-6xxx / IN-7xxx / IN-8xxx / IN-9xxx)

| Flux | URL RTSP | Résolution | Notes |
|--------|----------|------------|-------|
| Flux principal | `rtsp://IP:554//11` | Résolution complète | H.264 / H.265 |
| Sous-flux | `rtsp://IP:554//12` | Résolution inférieure | Économe en bande passante |
| Troisième flux | `rtsp://IP:554//13` | Optimisé mobile | Résolution la plus basse |

### URL spécifiques aux modèles

| Modèle | URL RTSP | Résolution | Type |
|-------|----------|------------|------|
| IN-6012 HD | `rtsp://IP:554//11` | 720p | Pan/tilt intérieur |
| IN-6014 HD | `rtsp://IP:554//11` | 720p | Intérieur |
| IN-7011 HD | `rtsp://IP:554//11` | 1080p | Pan/tilt intérieur |
| IN-8015 Full HD | `rtsp://IP:554//11` | 1080p | Intérieur/Extérieur |
| IN-9008 Full HD | `rtsp://IP:554//11` | 1080p+ | Extérieur PoE |
| IN-9020 Full HD | `rtsp://IP:554//11` | WQHD/4K | Extérieur PoE |

### Modèles 720p plus anciens (IN-5xxx — MPEG-4)

Les caméras IN-5xxx utilisent un chemin RTSP différent avec l'encodage MPEG-4 :

| Modèle | URL RTSP | Résolution | Notes |
|-------|----------|------------|-------|
| IN-5905 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Extérieur |
| IN-5907 HD | `rtsp://IP:554/MediaInput/mpeg4` | 720p | Extérieur |

### Modèles historiques (IN-2xxx / IN-3xxx / IN-4xxx — HTTP uniquement)

Les anciennes caméras INSTAR à résolution VGA ne prennent pas en charge RTSP. Elles utilisent uniquement le streaming basé sur HTTP :

| Série de modèles | URL HTTP | Type | Notes |
|-------------|----------|------|-------|
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | Flux ASF | Résolution VGA |
| IN-2xxx/3xxx/4xxx | `http://IP/videostream.cgi?rate=11` | MJPEG | Pas d'audio |
| IN-2xxx/3xxx/4xxx | `http://IP//iphone/11?USER:PASS&` | Flux mobile | Compatible iPhone |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra INSTAR avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// INSTAR IN-8015 Full HD, flux principal -- notez la double barre oblique !
var uri = new Uri("rtsp://192.168.1.50:554//11");
var username = "admin";
var password = "instar";
```

Pour accéder au sous-flux, utilisez `//12` au lieu de `//11`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture (HD) | `http://IP/tmpfs/auto.jpg` | IN-6xxx/7xxx/8xxx/9xxx |
| Capture (HD, auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | Avec identifiants |
| Capture (historique) | `http://IP/snapshot.cgi` | IN-2xxx/3xxx/4xxx |
| Capture (historique, auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Historique avec identifiants |
| Flux ASF (historique) | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=32&rate=0` | ASF VGA |
| Flux MJPEG (historique) | `http://IP/videostream.cgi?rate=11` | MJPEG historique |

## Dépannage

### Double barre oblique requise

Le problème de connexion INSTAR le plus courant est d'oublier la **double barre oblique** avant le numéro de flux. L'URL correcte est `rtsp://IP:554//11` (deux barres), et non `rtsp://IP:554/11` (une barre).

### Les caméras historiques n'ont pas de prise en charge RTSP

Les caméras IN-2xxx, IN-3xxx et IN-4xxx sont HTTP uniquement. Elles ne prennent pas du tout en charge RTSP. Utilisez les URL de streaming HTTP ASF ou MJPEG pour ces modèles.

### IN-5xxx utilise un chemin RTSP différent

Les caméras IN-5xxx utilisent `rtsp://IP:554/MediaInput/mpeg4` au lieu du chemin `//11` utilisé par les modèles HD plus récents. Si l'URL `//11` échoue sur une caméra INSTAR 720p, vérifiez si votre modèle appartient à la série IN-5xxx.

### MQTT et intégration à la maison connectée

Les caméras INSTAR prennent en charge MQTT pour l'intégration à Home Assistant, ioBroker et Node-RED. MQTT est utilisé pour le contrôle de la caméra et les notifications d'événements, pas pour le streaming vidéo. Pour l'intégration vidéo avec les plateformes de maison connectée, utilisez l'URL RTSP.

### Disponibilité PoE

Les modèles extérieurs IN-8xxx et IN-9xxx prennent en charge Power over Ethernet (PoE), permettant un seul câble pour l'alimentation et les données. Les modèles intérieurs nécessitent généralement un adaptateur d'alimentation séparé.

### Les identifiants varient selon le modèle

Bien que les identifiants par défaut courants soient admin / instar, certains modèles peuvent utiliser des valeurs par défaut différentes. Vérifiez la documentation ou l'étiquette de la caméra pour les identifiants d'usine. Les caméras INSTAR nécessitent généralement de modifier le mot de passe par défaut lors de la configuration initiale.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras INSTAR ?**

Pour les modèles HD actuels (IN-6xxx, IN-7xxx, IN-8xxx, IN-9xxx), l'URL est `rtsp://admin:instar@CAMERA_IP:554//11`. Notez la double barre oblique avant `11`. Pour les modèles IN-5xxx, utilisez `rtsp://admin:instar@CAMERA_IP:554/MediaInput/mpeg4`.

**Toutes les caméras INSTAR prennent-elles en charge RTSP ?**

Non. Les modèles historiques (IN-2xxx, IN-3xxx, IN-4xxx) sont des caméras à résolution VGA qui ne prennent en charge que le streaming basé sur HTTP au format ASF ou MJPEG. Toutes les caméras IN-5xxx et plus récentes prennent en charge RTSP.

**Quelle est la différence entre les flux //11, //12 et //13 ?**

Le flux `//11` est le flux principal (qualité la plus élevée), `//12` est un sous-flux à résolution inférieure adapté à la visualisation à distance sur bande passante limitée, et `//13` est un troisième flux optimisé pour mobile avec la résolution la plus basse.

**Les caméras INSTAR prennent-elles en charge ONVIF ?**

Oui. ONVIF est pris en charge sur les caméras des séries IN-6xxx, IN-7xxx, IN-8xxx et IN-9xxx. Les modèles historiques ne prennent pas en charge ONVIF. Vous pouvez utiliser les fonctionnalités ONVIF du SDK VisioForge pour la découverte de caméras et le contrôle PTZ sur les modèles pris en charge.

**Puis-je intégrer les caméras INSTAR avec Home Assistant ?**

Oui. Les caméras INSTAR prennent en charge MQTT, ce qui facilite leur intégration à Home Assistant, ioBroker et Node-RED pour l'automatisation et les actions déclenchées par événements. Pour le streaming vidéo dans Home Assistant, utilisez l'URL RTSP dans une intégration de caméra générique.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion ABUS](abus.md) — Caméras allemandes grand public / maison connectée
- [Enregistrer le flux RTSP original](../mediablocks/Guides/rtsp-save-original-stream.md) — Enregistrer les flux INSTAR sans réencoder
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
