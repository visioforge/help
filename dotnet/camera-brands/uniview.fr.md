---
title: Connexion à une caméra IP Uniview (UNV) en C# .NET
description: Modèles d'URL RTSP pour les caméras Uniview IPC-B, IPC-T, IPC-D, IPC-E et NVR en C# .NET. Intégration compatible ONVIF avec exemples de code SDK VisioForge.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Uniview (UNV) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Uniview** (Zhejiang Uniview Technologies Co., Ltd.), également connue sous le nom de **UNV**, est le troisième plus grand fabricant mondial de vidéosurveillance en parts de marché, derrière Hikvision et Dahua. Fondée en 2005 et dont le siège est à Hangzhou, en Chine, Uniview a été pionnière de la vidéosurveillance IP en Chine et propose une gamme complète de caméras IP, NVR, logiciels VMS et produits de contrôle d'accès pour les marchés d'entreprise et gouvernementaux.

**Faits clés :**

- **Gammes de produits :** IPC-B (bullet), IPC-T (turret), IPC-D (dôme), IPC-E (eyeball), IPC-P (PTZ), NVR30x/50x (NVR)
- **Prise en charge des protocoles :** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SDK (EZStation)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 123456 (doit être modifié à la première connexion avec le firmware plus récent)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265 (U-Code Smart Codec), MJPEG
- **Position sur le marché :** #3 mondial en vidéosurveillance

!!! info "Image de marque Uniview vs UNV"
    Uniview commercialise sous les noms de marque **Uniview** et **UNV** selon la région. Les modèles d'URL RTSP et le firmware sont identiques quelle que soit la marque. Certains partenaires OEM rebadgent le matériel Uniview sous leurs propres marques.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Uniview utilisent une structure d'URL basée sur le profil de média :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/media/video[STREAM]
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `video1` | Flux principal | Résolution la plus élevée (4K/5MP/4MP/2MP) |
| `video2` | Sous-flux | Résolution plus faible, bande passante réduite |
| `video3` | Troisième flux | Optimisé mobile (si pris en charge) |

### Formats d'URL alternatifs

Les caméras Uniview prennent en charge plusieurs modèles d'URL RTSP :

| Modèle d'URL | Description |
|-------------|-------------|
| `rtsp://IP:554/media/video1` | Flux principal (recommandé) |
| `rtsp://IP:554/media/video2` | Sous-flux |
| `rtsp://IP:554/media/video3` | Troisième flux |
| `rtsp://IP:554/unicast/c1/s0/live` | Flux principal unicast (alternative) |
| `rtsp://IP:554/unicast/c1/s1/live` | Sous-flux unicast (alternative) |
| `rtsp://IP:554/live/ch00_0` | Format hérité (firmware plus ancien) |
| `rtsp://IP:554/live/ch00_1` | Sous-flux hérité |

### Modèles de caméras IP

| Série de modèles | Résolution | URL du flux principal | Audio |
|-------------|-----------|----------------|-------|
| IPC-B112-PF28 (bullet 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | Non |
| IPC-B314-APKZ (bullet 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Oui |
| IPC-B315-APKZ (bullet 5MP) | 2880x1620 | `rtsp://IP:554/media/video1` | Oui |
| IPC-T112-PF28 (turret 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | Non |
| IPC-T314-APKZ (turret 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Oui |
| IPC-D312-APKZ (dôme 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Oui |
| IPC-D314-APKZ (dôme 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Oui |
| IPC-E312-APKZ (eyeball 4MP) | 2688x1520 | `rtsp://IP:554/media/video1` | Oui |
| IPC-P1E2-I (PTZ 2MP) | 1920x1080 | `rtsp://IP:554/media/video1` | Oui |
| IPC-B182-PF28 (bullet 4K) | 3840x2160 | `rtsp://IP:554/media/video1` | Oui |

### URL des canaux NVR

Pour les NVR Uniview (NVR301, NVR302, NVR304, NVR501, NVR516) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/media/video1` | `rtsp://IP:554/media/video2` |
| Caméra 2 | `rtsp://IP:554/media/video3` | `rtsp://IP:554/media/video4` |
| Caméra 3 | `rtsp://IP:554/media/video5` | `rtsp://IP:554/media/video6` |
| Caméra N | `rtsp://IP:554/media/video[2N-1]` | `rtsp://IP:554/media/video[2N]` |

!!! tip "Numérotation des canaux NVR"
    Sur les NVR Uniview, le numéro de flux vidéo encode à la fois le canal et le type de flux. Chaque canal utilise deux numéros consécutifs : impair pour le flux principal, pair pour le sous-flux. Caméra 1 = video1/video2, Caméra 2 = video3/video4, et ainsi de suite.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Uniview avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Uniview IPC-B314-APKZ, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/media/video2` à la place de `/media/video1`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification digest |
| Capture ONVIF | `http://IP/onvif-http/snapshot?channel=1` | Capture ONVIF HTTP |

## Dépannage

### Le mot de passe par défaut doit être modifié

Les caméras Uniview avec firmware actuel exigent que le mot de passe par défaut (`123456`) soit modifié lors de la configuration initiale. Si vous n'avez pas encore configuré la caméra :

1. Accédez à la caméra à l'adresse `http://CAMERA_IP` dans un navigateur
2. Suivez l'assistant d'activation
3. Définissez un mot de passe fort
4. Utilisez ces identifiants dans votre URL RTSP

### Format d'URL « unicast » vs « media »

Si `/media/video1` ne fonctionne pas sur votre caméra, essayez le format unicast : `rtsp://IP:554/unicast/c1/s0/live`. Les anciennes versions du firmware Uniview peuvent ne prendre en charge que le chemin unicast. Les firmwares plus récents prennent en charge les deux formats.

### Le flux H.265 ne se lit pas

Le codec intelligent U-Code de Uniview produit des flux H.265/HEVC standards. Si la lecture H.265 échoue :

1. Installez le redistribuable du décodeur HEVC
2. Ou basculez la caméra vers l'encodage H.264 dans l'interface web : **Setup > Video > Video**
3. Utilisez `rtspSettings.UseGPUDecoder = true` pour un décodage H.265 accéléré matériellement

### Problèmes de découverte ONVIF

ONVIF est activé par défaut sur les caméras Uniview mais peut nécessiter un mot de passe ONVIF séparé. Vérifiez **Setup > Network > ONVIF** dans l'interface web et assurez-vous que le compte utilisateur ONVIF est configuré.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Uniview ?**

L'URL standard est `rtsp://admin:password@CAMERA_IP:554/media/video1` pour le flux principal. Utilisez `/media/video2` pour le sous-flux. Certains modèles plus anciens utilisent plutôt `rtsp://IP:554/unicast/c1/s0/live`.

**Uniview est-il identique à UNV ?**

Oui. Uniview et UNV sont la même entreprise (Zhejiang Uniview Technologies). L'image de marque varie selon la région. Toutes les caméras utilisent un firmware, des formats d'URL RTSP et des interfaces web identiques, qu'elles portent l'étiquette Uniview ou UNV.

**Les caméras Uniview prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Uniview actuelles prennent en charge ONVIF Profile S et Profile T. ONVIF permet la découverte automatique de la caméra et l'accès standardisé au flux sans utiliser d'URL RTSP spécifiques à la marque.

**Comment accéder à plusieurs canaux sur un NVR Uniview ?**

Les NVR Uniview utilisent des numéros de flux vidéo séquentiels : Caméra 1 = video1 (principal) / video2 (sous), Caméra 2 = video3 (principal) / video4 (sous), et ainsi de suite. La formule est : flux principal = video[2N-1], sous-flux = video[2N] où N est le numéro de canal de la caméra.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Hikvision](hikvision.md) — Leader mondial du marché, format d'URL différent
- [Guide de connexion Dahua](dahua.md) — Une autre marque chinoise majeure de vidéosurveillance
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
