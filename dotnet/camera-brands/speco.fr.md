---
title: Connexion à une caméra IP Speco Technologies en C# .NET
description: Intégration RTSP Speco Technologies série O, VIP et DVR pour C# .NET. Modèles d'URL, sélection de canal et code SDK VisioForge pour caméras IP.
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

# Comment se connecter à une caméra IP Speco Technologies en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Speco Technologies** est une société américaine de vidéosurveillance professionnelle basée à Amityville, dans l'État de New York. Fondée en 1969, Speco fabrique des caméras IP, des caméras analogiques, des DVR, des NVR et des équipements de contrôle d'accès pour le marché des intégrateurs de sécurité professionnels. Les produits Speco sont vendus par l'intermédiaire de distributeurs agréés et d'intégrateurs de sécurité plutôt que par des canaux de vente directs aux consommateurs, ce qui en fait un choix courant dans les installations commerciales.

**Faits clés :**

- **Gammes de produits :** Série O (caméras IP : O2B, O2D, OINT), série VIP (caméras IP), série ZIP, série SIP, série LS, gammes DVR (TH/TL, RS, PCPRO)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (toutes les caméras IP actuelles)
- **Codecs vidéo :** H.264 (tous les modèles actuels), MPEG-4 (anciens modèles)

!!! info "Plusieurs gammes de produits"
    Speco Technologies dispose de nombreuses gammes de produits distinctes, chacune avec des formats d'URL RTSP différents. Identifiez votre série de modèle exacte (O, VIP, LS, ZIP, SIP ou type de DVR) avant de configurer l'URL du flux. Le flux racine `rtsp://IP:554/` fonctionne sur de nombreux appareils Speco comme test rapide.

## Modèles d'URL RTSP

### Caméras IP série O

La série O est la gamme actuelle de caméras IP de Speco, incluant les modèles bullet (O2B), dôme (O2D) et intensificateur (OINT) :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/
```

| Modèle | Résolution | URL du flux principal | Notes |
|-------|-----------|----------------|-------|
| O2B2 (bullet) | 1080p | `rtsp://IP:554/` | Flux racine |
| O2D4 (dôme) | 1080p | `rtsp://IP:554/` | Flux racine |
| OINT56B1G (intensificateur) | 1080p | `rtsp://IP:554/mpeg4` | Flux MPEG-4 |
| OINT56B1G (intensificateur) | 1080p | `rtsp://IP:554/` | Flux racine (H.264) |

### Caméras IP série VIP

La série VIP utilise un format de chemin de flux numéroté :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/1/stream1
```

| Modèle | Résolution | URL du flux principal | Notes |
|-------|-----------|----------------|-------|
| VIP2B1M (bullet) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (principal) |
| VIP2C1N (cube) | 1080p | `rtsp://IP:554/1/stream1` | Stream 1 (principal) |

### Série LS

La série LS utilise un chemin H.264 basé sur les canaux et prend également en charge un format avec identifiants dans l'URL :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/cam1/h264` | Flux H.264 canal 1 |
| `rtsp://IP:554/cam2/h264` | Flux H.264 canal 2 |
| `rtsp://IP:554/cam[N]/h264` | Flux H.264 canal N |
| `rtsp://IP:554//user=admin_password=tlJwpbo6_channel=1_stream=0.sdp` | Format identifiants dans l'URL |

!!! warning "Format d'identifiants série LS"
    La série LS prend en charge un format inhabituel d'identifiants dans l'URL où le nom d'utilisateur et le mot de passe sont intégrés directement dans le chemin. Le mot de passe dans ce format est spécifique à l'appareil et peut différer du mot de passe de l'interface web. Vérifiez la page des paramètres RTSP de l'appareil pour la valeur correcte.

### Série ZIP

La série ZIP utilise un format de streaming basé sur les profils :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//stream0/Channel=0;Profile=0
```

| Modèle | URL du flux principal | Notes |
|-------|----------------|-------|
| ZIP2B (bullet) | `rtsp://IP:554//stream0/Channel=0;Profile=0` | Profil 0 (principal) |

### Modèles DVR

Les DVR Speco utilisent divers formats d'URL selon la gamme :

| Série DVR | Modèle d'URL | Notes |
|------------|-------------|-------|
| DVR4WM | `rtsp://IP:554/` | Flux racine |
| Série RS | `rtsp://IP:554/Live/Channel=1` | Format canal Live |
| Série RS | `rtsp://IP:554/Live/Channel=2` | Canal 2 |
| DVR génériques | `rtsp://IP:554/` | Flux racine (solution de repli) |

### URL des canaux DVR (série RS)

Pour les DVR Speco série RS :

| Canal | URL du flux principal |
|---------|----------------|
| Caméra 1 | `rtsp://IP:554/Live/Channel=1` |
| Caméra 2 | `rtsp://IP:554/Live/Channel=2` |
| Caméra N | `rtsp://IP:554/Live/Channel=N` |

### Récapitulatif de tous les formats d'URL

| Modèle d'URL | Gamme de produits | Notes |
|-------------|-------------|-------|
| `rtsp://IP:554/` | Série O, DVR, général | Flux racine (fonctionne sur de nombreux appareils) |
| `rtsp://IP:554/mpeg4` | Série O (plus ancienne) | Flux MPEG-4 |
| `rtsp://IP:554/1/stream1` | Série VIP | Format de flux numéroté |
| `rtsp://IP:554/cam[N]/h264` | Série LS | H.264 basé sur le canal |
| `rtsp://IP:554//stream0/Channel=0;Profile=0` | Série ZIP | Format basé sur les profils |
| `rtsp://IP:554/Live/Channel=N` | DVR RS | Format canal Live |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Speco avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra dôme Speco O2D4, flux racine
var uri = new Uri("rtsp://192.168.1.64:554/");
var username = "admin";
var password = "admin";
```

Pour les caméras série VIP, utilisez plutôt le chemin `/1/stream1` :

```csharp
// Caméra bullet Speco VIP2B1M, flux principal
var uri = new Uri("rtsp://192.168.1.64:554/1/stream1");
var username = "admin";
var password = "admin";
```

## URL de capture instantanée et MJPEG

### Captures de caméra IP

| Type | Modèle d'URL | Modèles | Notes |
|------|-------------|--------|-------|
| Image fixe | `http://IP/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Capture JPEG basique |
| Image fixe (port 554) | `http://IP:554/stillimg.jpg` | O2B2, O2D4, OINT56B1G | Port alternatif |
| Capture par encodeur | `http://IP/cgi-bin/encoder?USER=user&PWD=pass&SNAPSHOT` | IP-SD10X, série SIP | Basée sur CGI avec identifiants |
| Flux système | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=user&PWD=pass` | Diverses caméras IP | Format commande système |

### Captures DVR

| Type | Modèle d'URL | Série DVR | Notes |
|------|-------------|------------|-------|
| Image complète | `http://IP/images1full` | DVR divers | Remplacez `1` par le numéro de canal |
| Image SIF | `http://IP/images1sif` | DVR divers | Résolution plus faible, remplacez `1` par le canal |
| Get Image | `http://IP/getimage?camera=1&fmt=full` | DVR divers | Remplacez `1` par le numéro de canal |
| Capture mobile | `http://IP/mobile/channel1.jpg` | DVR PCPRO | Optimisé mobile, remplacez `1` par le canal |
| Flux TH/TL | `http://IP/ivop.get?action=live&THREAD_ID=` | DVR TH/TL | Flux en direct via HTTP |

## Dépannage

### Formats d'URL incohérents entre gammes de produits

Speco Technologies a de nombreuses gammes de produits différentes, chacune avec son propre format d'URL RTSP. Si un modèle d'URL ne fonctionne pas, essayez d'abord le flux racine `rtsp://IP:554/` comme test de référence. Essayez ensuite le format spécifique à votre gamme de produits comme indiqué dans les tableaux ci-dessus.

### Limitations du flux racine

Le flux racine (`rtsp://IP:554/`) fonctionne sur de nombreux appareils Speco pour le flux principal mais n'est pas fiable pour accéder aux sous-flux ou aux canaux spécifiques sur des appareils multicanaux. Utilisez le format d'URL spécifique à la gamme de produits pour un contrôle complet de la sélection du flux.

### Format d'identifiants dans l'URL de la série LS

La série LS utilise un format d'URL inhabituel où les identifiants sont intégrés dans le chemin (`/user=admin_password=VALUE_channel=1_stream=0.sdp`). Le mot de passe dans ce format peut être une valeur générée par l'appareil qui diffère du mot de passe de l'interface web. Vérifiez la page **RTSP Settings** dans l'interface web de l'appareil pour la chaîne d'identifiants correcte.

### Découverte réseau

Speco fournit un outil DDNS et un utilitaire de découverte d'appareils pour trouver les caméras sur le réseau. Téléchargez l'outil Speco DDNS depuis le site web de Speco Technologies pour localiser les appareils qui ne répondent pas aux adresses IP attendues.

### Identifiants par défaut

Les appareils Speco sont généralement livrés avec les identifiants par défaut `admin` / `admin`. Si ceux-ci ne fonctionnent pas, le mot de passe a peut-être été modifié lors de l'installation par l'intégrateur de sécurité.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Speco ?**

Pour la plupart des caméras IP Speco, essayez d'abord le flux racine : `rtsp://admin:admin@CAMERA_IP:554/`. Pour les caméras série VIP, utilisez `rtsp://IP:554/1/stream1`, et pour les caméras série LS, utilisez `rtsp://IP:554/cam1/h264`. L'URL correcte dépend de votre gamme de produits spécifique.

**Les caméras Speco prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras IP Speco actuelles prennent en charge ONVIF. La découverte et le streaming ONVIF sont le moyen le plus fiable de se connecter aux caméras Speco si vous n'êtes pas sûr du format d'URL RTSP exact pour votre modèle.

**Pourquoi y a-t-il autant de formats d'URL différents pour les caméras Speco ?**

Speco Technologies fabrique des équipements de vidéosurveillance depuis 1969 et a acquis ou développé plusieurs gammes de produits au fil des décennies. Chaque gamme de produits (série O, VIP, LS, ZIP, SIP, gammes DVR) peut utiliser un firmware et une architecture de streaming différents, ce qui entraîne des formats d'URL différents. Identifiez toujours votre série de modèle exacte avant de configurer la connexion.

**Comment trouver ma caméra Speco sur le réseau ?**

Utilisez l'outil DDNS Speco ou l'utilitaire de découverte d'appareils, disponibles sur le site web de Speco Technologies. Vous pouvez également utiliser la découverte ONVIF via le SDK VisioForge ou un outil de scan réseau pour localiser l'adresse IP de la caméra sur votre réseau local.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion EverFocus](everfocus.md) — Caméras de vidéosurveillance professionnelles
- [Enregistrer le flux RTSP original](../mediablocks/Guides/rtsp-save-original-stream.md) — Enregistrer les flux Speco sans réencodage
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
