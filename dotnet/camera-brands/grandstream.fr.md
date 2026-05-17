---
title: Comment se connecter à une caméra IP Grandstream en C# .NET
description: Intégrez les caméras Grandstream GXV et GSC dans des applis C# .NET via RTSP. Modèles d'URL pour GXV3500, GXV3610, GSC3610 et exemples SDK VisioForge.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Grandstream en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Grandstream Networks** est une société américaine dont le siège est à Boston, dans le Massachusetts, États-Unis, connue pour ses téléphones VoIP et ses produits de vidéosurveillance IP. Grandstream propose des caméras IP et des encodeurs vidéo sous les gammes **GXV** (historique) et **GSC** (génération actuelle), ciblant les marchés PME et professionnels. Leurs caméras sont souvent déployées aux côtés des systèmes VoIP et UCM (Unified Communications Manager) de Grandstream.

**Faits clés :**

- **Gammes de produits :** GXV (caméras IP et encodeurs vidéo, historique), GSC (caméras intelligentes génération actuelle)
- **Prise en charge des protocoles :** RTSP, ONVIF (GXV36xx et nouvelles séries GSC), HTTP/CGI, SIP (appels vidéo)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (GXV36xx et modèles GSC plus récents)
- **Codecs vidéo :** H.264, H.265 (modèles GSC actuels), MPEG-4 (modèles GXV historiques)

## Modèles d'URL RTSP

### Caméras série GSC actuelle

Les caméras Grandstream GSC de génération actuelle utilisent un format d'URL basé sur canal :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux principal | `rtsp://IP:554/live/ch00_0` | Flux principal, canal 0 |
| Flux secondaire | `rtsp://IP:554/live/ch00_1` | Sous-flux |

### Série GXV (historique)

Les anciennes caméras GXV prennent en charge plusieurs formats d'URL selon le modèle :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux principal | `rtsp://IP:554//0` | Flux principal (canal 0) |
| Flux secondaire | `rtsp://IP:554//4` | Sous-flux (canal 4) |
| SDP H.264 | `rtsp://IP:554/ipcam_h264.sdp` | Accès basé sur fichier SDP |
| H.264 en direct | `rtsp://IP:554/live/h264` | Flux nommé |
| Basé sur canal | `rtsp://IP:554/[CHANNEL]` | Numéro de canal direct |
| Flux avec auth | `rtsp://IP:554//0/888888:888888/main` | Avec identifiants intégrés |
| MPEG-4 (historique) | `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | Flux MPEG-4 historique |

!!! info "Numérotation de canaux inhabituelle"
    Grandstream utilise un schéma de numérotation de canaux non standard. Pour les caméras à canal unique, le canal **0** est le flux principal et le canal **4** est le flux secondaire. Cela diffère de la plupart des autres marques qui utilisent une numérotation séquentielle.

### URL spécifiques aux modèles

| Modèle | Flux principal | Flux secondaire | Type |
|-------|---------------|-----------------|------|
| GXV3500 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Encodeur vidéo |
| GXV3504 (canal 1) | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Encodeur 4 canaux |
| GXV3504 (canal 2) | `rtsp://IP:554/1` | `rtsp://IP:554/5` | Encodeur 4 canaux |
| GXV3504 (canal 3) | `rtsp://IP:554/2` | `rtsp://IP:554/6` | Encodeur 4 canaux |
| GXV3504 (canal 4) | `rtsp://IP:554/3` | `rtsp://IP:554/7` | Encodeur 4 canaux |
| GXV3601 / GXV3611 | `rtsp://IP:554//4` | -- | Caméra dôme |
| GXV3601 (alt) | `rtsp://IP:554/ipcam_h264.sdp` | -- | Basé sur SDP |
| GXV3610 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Dôme HD |
| GXV3651 / GXV3661 / GXV3662 | `rtsp://IP:554/0` | `rtsp://IP:554/4` | Caméras FHD |
| GXV3672 | `rtsp://IP:554//0` | `rtsp://IP:554/live/ch00_0` | Extérieur HD/FHD |
| GSC3610 / GSC3615 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Dôme actuel |
| GSC3620 | `rtsp://IP:554/live/ch00_0` | `rtsp://IP:554/live/ch00_1` | Extérieur actuel |

### Carte de canaux de l'encodeur multi-canaux (GXV3504)

Le GXV3504 est un encodeur vidéo à 4 canaux avec la numérotation suivante :

| Entrée | Canal principal | Canal secondaire |
|-------|----------------|-------------------|
| Entrée 1 | 0 | 4 |
| Entrée 2 | 1 | 5 |
| Entrée 3 | 2 | 6 |
| Entrée 4 | 3 | 7 |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Grandstream avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Grandstream série GSC, flux principal
var uri = new Uri("rtsp://192.168.1.60:554/live/ch00_0");
var username = "admin";
var password = "YourPassword";
```

Pour les modèles GXV historiques, utilisez `rtsp://IP:554//0` pour le flux principal ou `rtsp://IP:554//4` pour le flux secondaire.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/snapshot/view0.jpg` | Capture du canal 0 |
| Capture JPEG (ch 1) | `http://IP/snapshot/view1.jpg` | Canal 1 (multi-canal) |
| Flux HTTP | `http://IP/goform/stream?cmd=get&channel=0` | Flux HTTP basé sur canal |

## Dépannage

### Confusion sur la numérotation des canaux

La numérotation des canaux Grandstream est non conventionnelle :

- **Caméras à canal unique :** Canal 0 = flux principal, Canal 4 = flux secondaire
- **GXV3504 (encodeur 4 canaux) :** Les canaux 0-3 sont les flux principaux pour les entrées 1-4 ; les canaux 4-7 sont les flux secondaires pour les entrées 1-4

Si vous obtenez un flux vide ou une erreur, vérifiez bien que vous utilisez le bon numéro de canal pour la qualité de flux souhaitée.

### Identifiants d'usine `888888`

Certains anciens modèles GXV Grandstream utilisent `888888` comme mot de passe par défaut (ou intégré dans l'URL RTSP sous la forme `888888:888888`). Si `admin` / `admin` ne fonctionne pas, essayez `888888` comme mot de passe.

### RTSP non activé

Sur certains anciens modèles GXV, le streaming RTSP doit être explicitement activé dans l'interface web de la caméra. Naviguez vers la page des paramètres de streaming ou multimédia et confirmez que RTSP est activé et défini sur le port 554.

### Plusieurs formats d'URL par modèle

De nombreuses caméras GXV prennent en charge plusieurs formats d'URL RTSP simultanément. Si l'un des formats ne fonctionne pas, essayez les alternatives :

1. `rtsp://IP:554//0` (numéro de canal avec double barre)
2. `rtsp://IP:554/live/ch00_0` (canal nommé)
3. `rtsp://IP:554/ipcam_h264.sdp` (fichier SDP)
4. `rtsp://IP:554/live/h264` (flux nommé)

### Compatibilité des codecs

Les caméras série GSC actuelles prennent en charge H.265 et H.264. Les modèles GXV historiques peuvent utiliser MPEG-4 par défaut. Si vous rencontrez des problèmes de décodage avec un modèle historique, vérifiez l'interface web de la caméra et basculez le codec sur H.264 si disponible.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Grandstream ?**

Pour les caméras série GSC actuelles, utilisez `rtsp://admin:password@CAMERA_IP:554/live/ch00_0`. Pour les anciennes caméras série GXV, essayez `rtsp://admin:password@CAMERA_IP:554//0` pour le flux principal.

**Les caméras Grandstream prennent-elles en charge ONVIF ?**

Oui, la série GXV36xx et les caméras série GSC actuelles prennent en charge ONVIF. Les anciens modèles GXV35xx et les encodeurs vidéo ne prennent généralement pas en charge ONVIF.

**Quelle est la différence entre le canal 0 et le canal 4 ?**

Sur les caméras Grandstream à canal unique, le canal 0 est le flux principal (haute qualité) et le canal 4 est le flux secondaire (qualité inférieure). C'est une convention spécifique à Grandstream qui diffère de la plupart des autres marques de caméras.

**Puis-je utiliser les caméras Grandstream avec un système UCM ?**

Oui. Les caméras Grandstream s'intègrent nativement aux systèmes Grandstream UCM (Unified Communications Manager). Cependant, l'accès RTSP fonctionne indépendamment de l'UCM et peut être utilisé avec n'importe quel logiciel tiers, y compris les SDK VisioForge.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Milesight](milesight.md) — Segment caméras PME / professionnel
- [Intégration de caméras IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuration de périphérique ONVIF Grandstream
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
