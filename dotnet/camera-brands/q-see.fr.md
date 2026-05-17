---
title: Connexion à une caméra IP et DVR Q-See en C# .NET — RTSP
description: Modèles d'URL RTSP des caméras et DVR Q-See séries QC, QCN et QS pour C# .NET. Identifiants par défaut, configuration des canaux et code SDK VisioForge.
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

# Comment se connecter à une caméra IP et DVR Q-See en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Q-See** était une marque américaine de vidéosurveillance grand public basée à Anaheim, en Californie. Les DVR et caméras IP Q-See étaient des systèmes de vidéosurveillance économiques populaires vendus chez les principaux revendeurs américains, dont Costco et Amazon. L'entreprise a essentiellement cessé ses activités en 2020, mais un grand nombre de systèmes Q-See restent déployés dans des foyers et entreprises. Les produits Q-See utilisaient un mélange de **caméras OEM Dahua** et de composants d'autres fabricants chinois, ce qui signifie que la plupart des appareils Q-See suivent les conventions d'URL RTSP de Dahua.

**Faits clés :**

- **Gammes de produits :** Série QC (DVR), série QCN (caméras IP), série QS (kits DVR)
- **Prise en charge des protocoles :** RTSP, HTTP/CGI, ONVIF (certains modèles de caméras IP)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin ou admin / 123456
- **Prise en charge ONVIF :** Certains modèles de caméras IP (série QCN)
- **Codecs vidéo :** H.264 (la plupart des modèles), MPEG-4 (DVR plus anciens)
- **Base OEM :** Mélange de Dahua et d'autres fabricants

!!! warning "Q-See a cessé ses activités"
    Q-See a cessé ses activités vers 2020. Aucune mise à jour de firmware, aucun support technique et aucun service cloud ne sont disponibles. Si vous intégrez du matériel Q-See, traitez-le comme un équipement compatible Dahua et essayez d'abord les modèles d'URL Dahua. Consultez notre [guide de connexion Dahua](dahua.md) pour des détails complémentaires.

## Modèles d'URL RTSP

### Format d'URL standard (basé sur Dahua)

La plupart des appareils Q-See utilisent le modèle d'URL `cam/realmonitor` de Dahua :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de caméra (1 pour les caméras autonomes, 1-N pour les canaux DVR) |
| `subtype` | 0 | Flux principal (résolution la plus élevée) |
| `subtype` | 1 | Sous-flux (résolution plus faible, moins de bande passante) |

### Modèles DVR (série QC, série QS)

| Modèle / Série | URL du flux principal | Notes |
|----------------|----------------|-------|
| QC-804 (DVR 4 canaux) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Format Dahua, modifiez `channel` pour chaque entrée |
| QS408 / QS411 (kits DVR) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Format Dahua |
| Divers DVR | `rtsp://IP:554/` | Flux racine (solution de repli) |
| Divers DVR | `rtsp://IP:554/live.sdp` | Flux Live SDP |
| Divers DVR | `rtsp://IP:554/ch0_unicast_firststream` | Premier flux unicast |

### Modèles de caméras IP (série QCN)

| Modèle | Résolution | URL | Notes |
|-------|-----------|-----|-------|
| QCN7001B | 1080p | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Format Dahua (recommandé) |
| QCN7001B | 1080p | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | Format PSIA |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/h264/1` | VideoInput H.264 |
| QCN7001B | 1080p | `rtsp://IP:554/VideoInput/1/mpeg4/1` | VideoInput MPEG-4 |
| QCN7005B | 1080p | `rtsp://IP:554/` | Flux racine |

### Formats d'URL alternatifs

Certains appareils Q-See prennent en charge des modèles d'URL supplémentaires :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Format Dahua standard (recommandé) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sous-flux (bande passante réduite) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=00&authbasic=BASE64` | Avec identifiants encodés en base64 |
| `rtsp://IP:554/` | Flux racine (solution de repli pour de nombreux modèles) |
| `rtsp://IP:554/live.sdp` | Format Live SDP |
| `rtsp://IP:554/ch0_unicast_firststream` | Premier flux unicast |

!!! note "Authentification base64"
    Le paramètre `authbasic=` utilisé dans certaines URL Q-See prend des identifiants encodés en base64 au format `username:password`. Par exemple, `admin:admin` est encodé en `YWRtaW46YWRtaW4=`.

### URL des canaux DVR

Pour les DVR multicanaux Q-See (QC-804, QS408, QS411, etc.) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Caméra 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Caméra N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Q-See avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Q-See QC-804 DVR, flux principal du canal 1
var uri = new Uri("rtsp://192.168.1.100:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "admin";
```

Pour accéder au sous-flux, utilisez `subtype=1` à la place de `subtype=0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture CGI | `http://IP/cgi-bin/snapshot.cgi?chn=1&u=USER&p=PASS` | Capture basée sur le canal avec identifiants |
| Capture par identifiants | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Capture basée sur les paramètres de connexion |
| Image fixe | `http://IP/stillimg1.jpg` | Remplacez `1` par le numéro de canal |
| Image de flux | `http://IP/images/stream_1.jpg` | Remplacez `1` par le numéro de canal |
| Flux rapide (série QS) | `http://IP/control/faststream.jpg?stream=MxPEG&needlength&fps=6` | Flux rapide continu |

## Dépannage

### Aucune mise à jour de firmware ni support

Q-See a cessé ses activités vers 2020. Il n'y a aucune mise à jour de firmware, aucun support technique et aucune pièce de rechange disponible. Si votre appareil Q-See présente une vulnérabilité de sécurité ou un bug, il ne peut pas être corrigé. Envisagez de passer à une marque de caméra actuellement prise en charge.

### Essayez d'abord les modèles d'URL Dahua

La plupart des DVR Q-See et de nombreuses caméras IP utilisent le firmware Dahua en interne. Si les URL spécifiques à Q-See listées ci-dessus ne fonctionnent pas, essayez le format Dahua standard `cam/realmonitor`. Consultez notre [guide de connexion Dahua](dahua.md) pour l'ensemble complet des modèles d'URL Dahua.

### Paramètre d'authentification base64

Certains appareils Q-See utilisent un paramètre `authbasic=` dans l'URL RTSP au lieu d'intégrer les identifiants dans l'URI. Encodez `username:password` en base64 :

- `admin:admin` = `YWRtaW46YWRtaW4=`
- `admin:123456` = `YWRtaW46MTIzNDU2`

### Redirection de port pour l'accès distant

Les DVR Q-See nécessitent généralement une redirection de port manuelle pour l'accès RTSP distant. Redirigez le port **554** (RTSP) et éventuellement le port **80** ou **8080** (HTTP) de votre routeur vers l'adresse IP locale du DVR.

### Identifiants par défaut

Les appareils Q-See sont généralement livrés avec l'une de ces paires d'identifiants :

- `admin` / `admin`
- `admin` / `123456`

Si aucun ne fonctionne, le mot de passe a peut-être été modifié par le propriétaire précédent ou l'installateur.

## FAQ

**Quel format d'URL RTSP les caméras Q-See utilisent-elles ?**

La plupart des appareils Q-See utilisent le format Dahua `cam/realmonitor` : `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0`. C'est parce que les caméras et DVR Q-See étaient principalement des versions OEM du matériel Dahua. Utilisez `channel=1` pour les caméras autonomes ou le numéro de canal approprié pour les entrées DVR.

**Les caméras Q-See sont-elles toujours prises en charge ?**

Non. Q-See a cessé ses activités vers 2020. Aucune mise à jour de firmware, aucun service cloud ni support technique ne sont disponibles. Le matériel fonctionne toujours, mais il n'y aura pas de futurs correctifs ou améliorations. De nombreux utilisateurs ont migré vers d'autres marques comme Amcrest ou Reolink qui utilisent des protocoles similaires basés sur Dahua.

**Puis-je utiliser les caméras Q-See avec ONVIF ?**

Certaines caméras IP Q-See (série QCN) prennent en charge ONVIF, mais la plupart des DVR Q-See ne le font pas. Si la découverte ONVIF échoue, utilisez plutôt les modèles d'URL RTSP directs listés ci-dessus.

**Quel est le mot de passe par défaut des caméras Q-See ?**

Les identifiants par défaut sont généralement `admin` / `admin` ou `admin` / `123456`. Comme Q-See n'est plus disponible, il n'existe pas d'outil officiel de réinitialisation de mot de passe. Une réinitialisation d'usine (généralement un bouton accessible par une épingle sur l'appareil) restaurera les identifiants par défaut sur la plupart des modèles.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Même format d'URL pour la plupart des appareils Q-See
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
