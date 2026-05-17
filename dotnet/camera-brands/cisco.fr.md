---
title: Configuration URL RTSP Cisco et intégration C# .NET
description: Intégration RTSP des caméras Cisco CIVS et Meraki pour C# .NET. Modèles d'URL pour modèles entreprise PVC, VC avec exemples de code SDK VisioForge et ONVIF.
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

# Comment se connecter à une caméra IP Cisco en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Cisco Systems** est la plus grande entreprise de réseautage au monde, dont le siège est à San Jose, en Californie, États-Unis. Cisco a produit des caméras IP sous les marques **Cisco** et **Linksys**. Les principales gammes de caméras étaient **Cisco Video Surveillance (CIVS/VC/PVC)** pour les déploiements entreprise et l'ancienne série de marque **Linksys (WVC)** pour les marchés grand public et petites entreprises. Cisco a vendu son activité de vidéosurveillance à Verkada et a arrêté la plupart des produits de caméras, mais de nombreuses unités restent déployées sur le terrain. La gamme **Meraki MV** (gérée dans le cloud) est le seul produit de caméra Cisco actuel.

**Faits clés :**

- **Gammes de produits :** CIVS (vidéosurveillance entreprise), PVC (petites entreprises), VC (caméra vidéo), WVC (caméra vidéo sans fil, héritage Linksys), WCS (serveur de caméra sans fil), Meraki MV (gérée dans le cloud, actuelle)
- **Prise en charge des protocoles :** RTSP, HTTP/CGI, MJPEG, ASF (modèles WVC) ; Meraki MV est cloud uniquement
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (variable selon le modèle)
- **Prise en charge ONVIF :** Limitée (uniquement certains modèles CIVS plus récents ; la série WVC ne prend pas en charge ONVIF)
- **Codecs vidéo :** H.264, MPEG-4 (série WVC), MJPEG

!!! warning "Caméras Meraki MV"
    Les caméras Cisco Meraki MV utilisent un accès uniquement cloud et ne prennent **pas** en charge le streaming RTSP direct. Elles ne peuvent pas être connectées via des URL RTSP locales. Les informations de cette page s'appliquent aux gammes de produits de caméras Cisco et Linksys historiques.

## Modèles d'URL RTSP

### Caméras entreprise (séries CIVS / VC / PVC)

La plupart des caméras IP Cisco entreprise utilisent le chemin `/img/media.sav` :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux principal | `rtsp://IP:554/img/media.sav` | La plupart des caméras CIVS et WVC |
| Live SDP | `rtsp://IP:554/live.sdp` | Séries VC240, PVC300, VC220 |
| Vidéo SAV | `rtsp://IP:554/img/video.sav` | PVC2300 |
| Code d'accès | `rtsp://IP:554/[ACCESS_CODE]` | Code configuré dans l'interface web de la caméra |
| Flux racine | `rtsp://IP:554/` | Solution de repli pour PVC2300 et autres |

### URL spécifiques aux modèles

| Modèle | URL RTSP | Type |
|-------|----------|------|
| CIVS 2500 / 2521 / 2531 | `rtsp://IP:554/img/media.sav` | Dôme/bullet entreprise |
| PVC300 | `rtsp://IP:554/live.sdp` | PTZ petites entreprises |
| PVC2300 | `rtsp://IP:554/img/video.sav` | Boîtier petites entreprises |
| PVC2300 (alt) | `rtsp://IP:554/` | Solution de repli |
| VC220 | `rtsp://IP:554/live.sdp` | Caméra dôme |
| VC240 | `rtsp://IP:554/live.sdp` | Caméra dôme |
| WVC80N | `rtsp://IP:554/img/media.sav` | Linksys sans fil |
| WVC210 | `rtsp://IP:554/img/media.sav` | Linksys sans fil |
| WVC54GCA | `rtsp://IP:554/img/media.sav` | Linksys sans fil |
| WCS-1130 | `rtsp://IP:554/play1.sdp` | Serveur de caméra sans fil |

!!! info "Extension de fichier inhabituelle"
    Les caméras Cisco et Linksys utilisent le chemin `/img/media.sav`, qui a une extension de fichier `.sav` inhabituelle. Ce n'est pas un format multimédia standard — c'est un point de terminaison RTSP spécifique à Cisco. Ne le confondez pas avec une URL de téléchargement de fichier.

### Série WVC (héritage Linksys)

La série WVC (Wireless Video Camera) de Linksys était une gamme populaire de caméras grand public avant que Cisco ne la rebrande et finalement l'arrête :

| Modèle | URL RTSP | Résolution | Notes |
|-------|----------|------------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | 640x480 | Wi-Fi, MPEG-4 |
| WVC80N | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-N |
| WVC210 | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-G PTZ |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Cisco avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Cisco entreprise (CIVS/WVC), flux principal
var uri = new Uri("rtsp://192.168.1.50:554/img/media.sav");
var username = "admin";
var password = "YourPassword";
```

Pour les caméras VC240 ou PVC300, utilisez `/live.sdp` au lieu de `/img/media.sav`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/img/snapshot.cgi?size=3` | La plupart des caméras Cisco (size : 1=QQVGA, 2=QVGA, 3=VGA) |
| Capture JPEG (VGA) | `http://IP/img/snapshot.cgi?img=vga` | Résolution nommée |
| Flux MJPEG | `http://IP/img/video.mjpeg` | Flux MJPEG continu |
| MJPEG (alt) | `http://IP/img/mjpeg.jpg` | Point de terminaison MJPEG alternatif |
| Flux ASF | `http://IP/img/video.asf` | Flux ASF de la série WVC |
| Capture PVC300 | `http://IP/cgi-bin/viewer/snapshot.jpg?resolution=640x480` | Paramètre de résolution requis |

## Dépannage

### `/img/media.sav` ne répond pas

L'extension `.sav` est un point de terminaison RTSP spécifique à Cisco. Si l'URL ne fonctionne pas :

1. Vérifiez l'adresse IP de la caméra et que le port 554 est ouvert
2. Confirmez que RTSP est activé dans l'interface web de la caméra
3. Certains modèles nécessitent qu'un code d'accès soit configuré avant que l'accès RTSP ne fonctionne — vérifiez les paramètres de streaming de la caméra
4. Essayez l'URL de repli `rtsp://IP:554/` si le chemin spécifique ne répond pas

### La série WVC ne renvoie que MPEG-4

Les caméras Linksys WVC (WVC54GCA, WVC80N, WVC210) prennent en charge MPEG-4 mais pas H.264. Le SDK VisioForge gère automatiquement les flux MPEG-4. Si vous voyez des artefacts, assurez-vous de ne pas forcer le décodage H.264.

### Authentification par code d'accès

Certaines caméras Cisco utilisent un code d'accès au lieu du traditionnel nom d'utilisateur / mot de passe pour RTSP. Le code d'accès est configuré dans l'interface web de la caméra sous les paramètres de streaming et est ajouté à l'URL :

```
rtsp://IP:554/[YOUR_ACCESS_CODE]
```

### Flux HTTP pour les caméras historiques

Les caméras Cisco/Linksys plus anciennes peuvent mieux fonctionner avec des flux ASF ou MJPEG basés sur HTTP qu'avec RTSP. Utilisez l'URL ASF (`http://IP/img/video.asf`) comme solution de repli si RTSP n'est pas fiable.

### Les caméras Meraki MV ne sont pas accessibles

Les caméras Meraki MV sont uniquement gérées dans le cloud et ne prennent pas en charge l'accès RTSP local. Il n'existe aucune URL RTSP locale disponible pour ces caméras. La vidéo ne peut être consultée que via l'interface cloud du Meraki Dashboard.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Cisco ?**

Pour la plupart des caméras IP Cisco et Linksys, utilisez `rtsp://admin:password@CAMERA_IP:554/img/media.sav`. Pour les caméras des séries VC/PVC, essayez plutôt `rtsp://admin:password@CAMERA_IP:554/live.sdp`.

**Les caméras Cisco prennent-elles en charge ONVIF ?**

Seules certaines caméras entreprise de la série CIVS plus récentes disposent d'une prise en charge ONVIF limitée. Les caméras grand public Linksys WVC et les caméras Meraki MV ne prennent pas en charge ONVIF.

**Les caméras Cisco sont-elles toujours fabriquées ?**

Cisco a vendu son activité de vidéosurveillance (gamme CIVS) à Verkada et a arrêté les caméras Linksys WVC. Le seul produit de caméra Cisco actuel est la série Meraki MV, gérée dans le cloud et qui ne prend pas en charge RTSP. Cependant, de nombreuses caméras Cisco historiques restent déployées et opérationnelles.

**Quels codecs utilisent les caméras Cisco ?**

Les caméras entreprise CIVS plus récentes prennent en charge H.264. La série Linksys WVC utilise principalement MPEG-4 et MJPEG. Le codec dépend du modèle et de la version du firmware.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Linksys](linksys.md) — Mêmes modèles d'URL, filiale Cisco
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
