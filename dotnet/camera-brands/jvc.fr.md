---
title: Caméras IP JVC — URL RTSP et intégration C# .NET VisioForge
description: URL RTSP des caméras réseau JVC VN-H, VN-T, VN-C et VN-X en C# .NET. Diffusion et enregistrement avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP JVC en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**JVC** (JVCKENWOOD Corporation) est une société japonaise d'électronique dont le siège social est à Yokohama, au Japon. La division Systèmes Professionnels de JVC produisait les caméras IP de la série VN destinées aux applications de vidéosurveillance. JVC a quitté le marché des caméras IP autonomes vers 2015, mais de nombreuses caméras de la série VN restent en service actif dans des installations d'entreprise et gouvernementales. Ces caméras sont reconnues pour leur prise en charge robuste du protocole PSIA et leur fiabilité.

**Faits clés :**

- **Gammes de produits :** VN-H Series (VN-H37, VN-H137, VN-H237, VN-H657), VN-T Series (VN-T216U), VN-X Series (VN-X35U, VN-X235U), VN-C Series (VN-C20U)
- **Protocoles pris en charge :** RTSP, ONVIF (séries VN-H/VN-T), PSIA, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / jvc (varie selon le modèle)
- **Prise en charge ONVIF :** Oui (séries VN-H et VN-T)
- **Codecs vidéo :** H.264 (séries VN-H/VN-T), MPEG-4 (anciens modèles VN-C)

!!! warning "Gamme de produits abandonnée"
    JVC a quitté le marché des caméras IP vers 2015. Bien que les caméras de la série VN restent largement déployées, les mises à jour de firmware ne sont plus disponibles. Envisagez une segmentation réseau et des règles de pare-feu pour protéger ces caméras, car elles ne recevront pas de correctifs de sécurité.

## Modèles d'URL RTSP

### Formats d'URL standard

Les caméras JVC prennent en charge plusieurs modèles d'URL RTSP selon la série du modèle :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/PSIA/Streaming/channels/0?videoCodecType=H.264
```

| Modèle d'URL | Protocole | Description |
|--------------|-----------|-------------|
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | Flux H.264 principal (la plupart des modèles VN-H) |
| `rtsp://IP:554/PSIA/Streaming/channels/CHANNEL` | PSIA | Flux PSIA par numéro de canal |
| `rtsp://IP:554/video.h264` | H.264 | Flux H.264 direct (série VN générale) |
| `rtsp://IP:554/1/stream1` | H.264 | URL de flux alternative (VN-T216U) |
| `rtsp://IP:554//livestream` | H.264 | URL de flux en direct (VN-H57) |

!!! note "Numérotation des canaux PSIA"
    Les caméras JVC utilisent une numérotation des canaux à base zéro pour les URL PSIA. Le canal 0 est le premier (et généralement unique) canal vidéo. Cela diffère de la plupart des autres marques qui commencent la numérotation à 1.

### URL des canaux PSIA

| Canal | URL | Description |
|-------|-----|-------------|
| Canal 0 (principal) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | Premier canal vidéo (flux principal) |
| Canal 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` | Second canal vidéo (sous-flux, si disponible) |

### Modèles de caméras

| Série de modèle | Résolution | URL du flux principal | Codec |
|-----------------|------------|-----------------------|-------|
| VN-H37 (dôme HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H137 (bullet HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H237 (dôme HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H657 (PTZ HD) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-T216U (box HD) | 1920x1080 | `rtsp://IP:554/1/stream1` | H.264 |
| VN-X35U (caméra réseau) | 1280x960 | `rtsp://IP:554/video.h264` | H.264 |
| VN-X235U (caméra réseau) | 1920x1080 | `rtsp://IP:554/video.h264` | H.264 |
| VN-C20U (réseau hérité) | 640x480 | N/A (snapshot HTTP uniquement) | MJPEG |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra JVC avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// JVC VN-H Series, flux H.264 principal PSIA
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
var username = "admin";
var password = "jvc";
```

Pour les caméras de la série VN-T utilisant le format d'URL alternatif :

```csharp
// JVC VN-T216U, URL de flux alternative
var uri = new Uri("rtsp://192.168.1.90:554/1/stream1");
```

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG | `http://IP/cgi-bin/video.jpg` | Snapshot standard (la plupart des modèles) |
| Snapshot Java Applet | `http://IP/java.jpg` | Snapshot basé sur Java (hérité) |
| Snapshot API | `http://IP/api/video?encode=jpeg` | Capture JPEG via API (série VN-X) |
| Flux MJPEG | `http://IP/api/video?encode=jpeg&framerate=15&boundary=on` | MJPEG continu via API |

### URL de snapshot spécifiques au modèle

| Série de modèle | URL de snapshot | Notes |
|-----------------|-----------------|-------|
| VN-H Series | `http://IP/cgi-bin/video.jpg` | Snapshot basé sur CGI |
| VN-T Series | `http://IP/cgi-bin/video.jpg` | Snapshot basé sur CGI |
| VN-C Series (VN-C20U) | `http://IP/cgi-bin/video.jpg` | Snapshot basé sur CGI |
| VN-X Series (VN-X35U/X235U) | `http://IP/api/video?encode=jpeg` | Snapshot basé sur API |

## Dépannage

### La numérotation des canaux PSIA commence à 0

Contrairement à la plupart des marques de caméras où la numérotation des canaux commence à 1, JVC utilise une numérotation PSIA **à base zéro**. Si vous portez du code depuis une autre marque :

- Canal 0 = Premier canal vidéo (équivalent au canal 1 sur d'autres marques)
- Canal 1 = Second canal vidéo (sous-flux ou capteur secondaire)

### Les identifiants par défaut ne fonctionnent pas

Les caméras JVC sont livrées avec différents identifiants par défaut selon le modèle et la version du firmware :

1. Essayez `admin` / `jvc` (le plus courant)
2. Essayez `admin` / `admin`
3. Accédez à l'interface web sur `http://CAMERA_IP` pour réinitialiser ou vérifier les identifiants
4. Certains modèles sont livrés sans mot de passe par défaut — accédez d'abord à l'interface web pour en définir un

### Mises à jour de firmware indisponibles

JVC ayant abandonné sa gamme de caméras IP vers 2015, les mises à jour de firmware ne sont plus disponibles. Pour atténuer les risques de sécurité :

- Placez les caméras sur un VLAN ou un segment de réseau isolé
- Utilisez des règles de pare-feu pour restreindre l'accès aux ports de la caméra
- Désactivez UPnP et toute fonctionnalité de connectivité cloud
- Envisagez de remplacer les caméras en fin de vie par des modèles actuellement pris en charge

### Accès HTTP uniquement pour la série VN-C

Les anciennes caméras de la série VN-C (comme la VN-C20U) ne prennent pas en charge le streaming RTSP et ne fournissent qu'un accès MJPEG basé sur HTTP. Utilisez les URL de snapshot HTTP ou de flux MJPEG pour ces modèles au lieu de RTSP.

### Plusieurs formats d'URL sur la série VN-T

La VN-T216U prend en charge plusieurs formats d'URL RTSP. Si l'un ne fonctionne pas, essayez les alternatives :

1. `rtsp://IP:554/1/stream1` (préféré)
2. `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` (PSIA)
3. `rtsp://IP:554/video.h264` (H.264 direct)

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras JVC ?**

Pour la plupart des caméras de la série VN-H, l'URL RTSP principale est `rtsp://admin:jvc@CAMERA_IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. La série VN-T utilise `rtsp://IP:554/1/stream1` comme alternative. Les modèles de la série VN-X utilisent `rtsp://IP:554/video.h264`.

**Les caméras IP JVC sont-elles toujours prises en charge ?**

JVC a quitté le marché des caméras IP autonomes vers 2015. Les caméras restent fonctionnelles mais ne reçoivent plus de mises à jour de firmware ni de support officiel. De nombreuses caméras de la série VN sont toujours activement déployées dans des systèmes de vidéosurveillance à travers le monde.

**Les caméras JVC prennent-elles en charge ONVIF ?**

Les caméras des séries VN-H et VN-T prennent en charge ONVIF Profile S. Les modèles plus anciens VN-C et certains VN-X ne prennent pas en charge ONVIF et reposent sur des interfaces PSIA ou CGI propriétaires.

**Pourquoi la numérotation des canaux PSIA commence-t-elle à 0 ?**

JVC implémente une numérotation des canaux PSIA à base zéro, ce qui signifie que le premier canal vidéo est le canal 0 plutôt que le canal 1. Cela est spécifique à l'implémentation PSIA de JVC et diffère de la plupart des autres fabricants de caméras. Lors de la migration depuis une autre marque, ajustez vos numéros de canal en conséquence.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Sony](sony.md) — caméras d'entreprise japonaises
- [Guide de connexion Canon](canon.md) — caméras professionnelles japonaises
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
