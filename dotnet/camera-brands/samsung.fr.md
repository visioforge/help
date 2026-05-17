---
title: Connexion à une caméra IP Samsung (Hanwha) en C# .NET
description: Modèles d'URL RTSP pour les caméras Samsung Wisenet SNO, SND, XNO, XND et PNO en C# .NET. Intégration Hanwha Vision avec des exemples de code SDK VisioForge.
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
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Samsung (Hanwha) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Hanwha Vision** (anciennement Samsung Techwin, puis Hanwha Techwin) est un fabricant sud-coréen d'équipements de vidéosurveillance professionnels et d'entreprise. La marque de caméras de sécurité Samsung a été renommée **Wisenet** après l'acquisition de Samsung Techwin par Hanwha Group en 2015. Les caméras Hanwha Vision sont largement déployées dans les installations d'entreprise, gouvernementales et d'infrastructure critique dans le monde entier.

**Faits clés :**

- **Gammes de produits :** XNO/XND/XNV (série X, fleuron actuel), PNO/PND/PNV (série P, grand public), QNO/QND/QNV (série Q, économique), SNO/SND/SNV/SNB (série S, héritage Samsung)
- **Convention de nommage :** Première lettre = série, N = réseau, O = extérieur, D = dôme, V = anti-vandalisme, B = box, P = PTZ
- **Prise en charge des protocoles :** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Wisenet WAVE VMS
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de la configuration initiale) ; modèles Samsung hérités : admin / 4321
- **Prise en charge ONVIF :** Oui (tous les modèles actuels et la plupart des modèles hérités)
- **Codecs vidéo :** H.264, H.265, MJPEG

## Modèles d'URL RTSP

### Modèles actuels (séries Wisenet X/P/Q)

Les caméras Hanwha Vision actuelles utilisent un format d'URL basé sur un profil :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Profil 1 (principal) | `rtsp://IP:554/profile2/media.smp` | Flux principal, H.264/H.265 |
| Profil 2 (sous) | `rtsp://IP:554/profile3/media.smp` | Sous-flux |
| Profil ONVIF 1 | `rtsp://IP:554//onvif/profile1/media.smp` | Compatible ONVIF (notez la double barre) |
| Profil ONVIF 2 | `rtsp://IP:554//onvif/profile2/media.smp` | Sous-flux ONVIF |

!!! warning "Double barre dans les URL ONVIF"
    Les URL ONVIF Samsung/Hanwha utilisent une double barre (`//onvif/`). Ceci est intentionnel et obligatoire. Utiliser une seule barre échouera.

### Série S Samsung héritée

| Série de modèles | URL RTSP | Codec |
|-------------|----------|-------|
| SNB-xxxx (box) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SND-xxxx (dôme) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNO-xxxx (extérieur) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNV-xxxx (anti-vandalisme) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNP-xxxx (PTZ) | `rtsp://IP:554/profile2/media.smp` | H.264 |

### Anciens modèles Samsung (pré-Wisenet)

Les anciennes caméras Samsung utilisaient différents formats d'URL :

| Modèle d'URL | Modèles | Codec |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4unicast` | SNB-2000, SNC-1300, SNP-3301/3370 | MPEG-4 |
| `rtsp://IP:554/h264unicast` | SNP-3301/H, SNP-3370/TH | H.264 |
| `rtsp://IP:554/mjpegunicast` | SNP-3301/H, SNP-3370/TH | MJPEG |
| `rtsp://IP:554/H264/media.smp` | SNB-3000, SND-3080, SNV-3080/3081 | H.264 |
| `rtsp://IP:554/MPEG4/media.smp` | SNV-3080/3081 | MPEG-4 |
| `rtsp://IP:554/MJPEG/media.smp` | SNB-3000, SNV-3081, SNV-6084R | MJPEG |
| `rtsp://IP:554/MediaInput/h264` | Divers Samsung | H.264 |

### URL DVR/NVR

| Appareil | URL RTSP | Notes |
|--------|----------|-------|
| DVR SRD-165 | `rtsp://IP:558/` | Port non standard 558 |
| DVR SME | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |
| DVR SMT | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Samsung (Hanwha Wisenet) avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Hanwha Wisenet série X, flux principal
var uri = new Uri("rtsp://192.168.1.70:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `profile3/media.smp` à la place de `profile2/media.smp`. Pour les modèles Samsung série S hérités, utilisez le mot de passe par défaut `4321` et le chemin d'URL `/mpeg4unicast` ou `/H264/media.smp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg` | Modèles actuels |
| Flux MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpg` | Modèles actuels |
| Capture héritée | `http://IP/video?submenu=jpg` | Modèles pré-Wisenet |
| MJPEG hérité | `http://IP/video?submenu=mjpg` | Modèles pré-Wisenet |
| Capture CGI | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=CHANNEL` | Modèles DVR |
| Capture (taille) | `http://IP/snap.jpg?JpegSize=XL` | Certains firmwares OEM Bosch |

## Dépannage

### Différences de mot de passe par défaut

- **Modèles Hanwha Vision actuels :** Le mot de passe doit être défini lors de la configuration initiale via un navigateur web
- **Série S Samsung héritée :** Le mot de passe par défaut est `4321`
- **Très anciens modèles Samsung :** Certains utilisaient `admin` / `admin`

### profile2 vs profile1 dans l'URL

Les caméras Samsung/Hanwha utilisent `profile2/media.smp` pour le flux principal (pas `profile1`). C'est une source courante de confusion :

- `profile2/media.smp` = Flux principal (généralement H.264 en résolution complète)
- `profile3/media.smp` = Sous-flux
- Les numéros de profil peuvent différer selon la configuration de la caméra

### Problème de double barre ONVIF

Le format d'URL ONVIF nécessite une double barre avant `onvif` :
- Correct : `rtsp://IP:554//onvif/profile1/media.smp`
- Incorrect : `rtsp://IP:554/onvif/profile1/media.smp`

### Confusion sur le nom de la marque

Samsung Techwin a été acquise par Hanwha en 2015. La marque s'est appelée :
- **Samsung Techwin** (avant 2015)
- **Hanwha Techwin** (2015-2022)
- **Hanwha Vision** (2022-présent)
- **Wisenet** (nom de marque produit, utilisé en permanence)

Toutes utilisent les mêmes modèles d'URL RTSP au sein de leur génération respective.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Samsung/Hanwha ?**

Pour les modèles Wisenet actuels, l'URL est `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp`. Pour les modèles Samsung hérités, essayez `rtsp://admin:4321@CAMERA_IP:554/mpeg4unicast` ou `rtsp://CAMERA_IP:554/H264/media.smp`.

**Samsung est-il identique à Hanwha Vision ?**

Oui. La division caméras de sécurité de Samsung a été acquise par Hanwha Group en 2015. La marque produit est **Wisenet**. Les caméras Samsung héritées (séries SNB, SND, SNO) et les caméras Hanwha Vision actuelles (séries XNO, XND, PNO) utilisent des modèles RTSP similaires.

**Les caméras Samsung/Hanwha prennent-elles en charge H.265 ?**

Oui. Les caméras actuelles des séries X et P prennent en charge H.265 (HEVC). Les caméras héritées de la série S prennent généralement en charge uniquement H.264 et MPEG-4.

**Quel VMS fonctionne avec les caméras Hanwha ?**

Le VMS propre à Hanwha est **Wisenet WAVE**. Cependant, toutes les caméras Hanwha prennent en charge RTSP et ONVIF standards, ce qui les rend compatibles avec n'importe quel logiciel tiers, y compris les applications SDK VisioForge.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Hanwha Vision](hanwha.md) — Nom de marque actuel, mêmes URL
- [Guide de connexion Wisenet](wisenet.md) — Famille de produits Hanwha Vision
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
