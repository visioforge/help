---
title: Modèles d'URL RTSP Mobotix — caméras IP en C# .NET SDK
description: Connectez les caméras MOBOTIX en C# .NET avec les modèles d'URL RTSP pour les séries Mx classiques et MOVE. Inclut MxPEG, MJPEG et options de flux H.264.
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
  - USB3 Vision / GigE
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Mobotix en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**MOBOTIX** (MOBOTIX AG) est un fabricant allemand de caméras IP dont le siège social est à Langmeil, en Allemagne, fondé en 1999. MOBOTIX a été un pionnier du concept de systèmes vidéo IP décentralisés où le traitement intelligent, l'enregistrement et les analyses se produisent directement à l'intérieur de la caméra plutôt que sur un serveur central. La société a été acquise par **Konica Minolta** en 2016. Les caméras MOBOTIX sont reconnues pour leur construction robuste, leur longue durée de vie opérationnelle et leur aptitude aux environnements industriels, extérieurs et d'infrastructures critiques.

**Faits clés :**

- **Gammes de produits :** Série M (extérieur), série D (dôme), série S (hémisphérique), série Q (panoramique), série T (interphone vidéo), MOVE (gamme ONVIF plus récente)
- **Protocoles pris en charge :** RTSP, HTTP/CGI, MxPEG (propriétaire) ; ONVIF (série MOVE uniquement)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / meinsm
- **Prise en charge ONVIF :** Série MOVE uniquement (les caméras Mx classiques ne prennent pas en charge ONVIF)
- **Codecs vidéo :** MxPEG (propriétaire), MJPEG, H.264 (modèles plus récents)
- **Architecture :** Décentralisée, enregistrement et traitement dans la caméra

!!! warning "Séries classique et MOVE"
    Les caméras Mobotix classiques (séries M/D/S/Q) utilisent principalement le codec propriétaire MxPEG et ne prennent pas en charge ONVIF. Pour ONVIF et RTSP H.264/H.265 standard, utilisez la nouvelle série MOBOTIX MOVE.

!!! note "À propos de MxPEG"
    MxPEG est un codec vidéo propriétaire développé par MOBOTIX pour une utilisation efficace de la bande passante avec leur architecture décentralisée. Si votre application ne peut pas décoder MxPEG nativement, utilisez le flux MJPEG de repli via HTTP (`/cgi-bin/faststream.jpg`) ou configurez la caméra pour générer du MJPEG ou H.264 standard où cela est pris en charge. Le SDK VisioForge peut se connecter aux caméras MOBOTIX en utilisant le flux MJPEG HTTP ou le flux RTSP H.264 sur les modèles pris en charge.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras MOBOTIX utilisent des URL RTSP basées sur des chemins de marque :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/mobotix.h264
```

| Flux | Modèle d'URL | Description |
|------|--------------|-------------|
| Flux principal H.264 | `rtsp://IP:554/mobotix.h264` | Flux H.264 principal (modèles plus récents) |
| Flux MJPEG | `rtsp://IP:554/mobotix.mjpeg` | MJPEG sur RTSP |

### Séries de caméras et URL

| Série | Type | URL recommandée | Codec |
|-------|------|-----------------|-------|
| MOVE Bullet | Bullet IP | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Dome | Dôme IP | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Vandal Dome | Antivandalisme IP | `rtsp://IP:554/mobotix.h264` | H.264 |
| Série M (M73, M16) | Extérieur | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Série D (D16, D26) | Dôme | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Série S (S16, S26) | Hémisphérique | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Série Q (Q26) | Panoramique 360 | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Série T (T26) | Interphone vidéo | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |

### URL ONVIF de la série MOVE

Les caméras MOBOTIX MOVE prennent en charge l'ONVIF standard et fournissent des flux RTSP conventionnels :

| Flux | Modèle d'URL | Notes |
|------|--------------|-------|
| Flux principal | `rtsp://IP:554/mobotix.h264` | Flux H.264 principal |
| Sous-flux | `rtsp://IP:554/mobotix.mjpeg` | Flux MJPEG secondaire |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra MOBOTIX avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// MOBOTIX MOVE ou caméra Mx classique, flux H.264
var uri = new Uri("rtsp://192.168.1.90:554/mobotix.h264");
var username = "admin";
var password = "meinsm";
```

Pour les caméras Mx classiques qui ne prennent en charge que MxPEG, utilisez l'URL du flux MJPEG HTTP à la place (voir ci-dessous).

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| MJPEG pleine résolution | `http://IP/cgi-bin/faststream.jpg?stream=full` | MJPEG continu à pleine résolution |
| Flux MxPEG | `http://IP/cgi-bin/faststream.jpg?stream=MxPEG&needlength&fps=6` | MxPEG propriétaire à 6 fps |
| MJPEG à fps contrôlé | `http://IP/control/faststream.jpg?stream=full&fps=10` | MJPEG plafonné à 10 fps |
| Snapshot actuel | `http://IP/record/current.jpg` | Snapshot JPEG unique |

## Dépannage

### Caméra Mx classique ne se connectant pas via RTSP

Les caméras MOBOTIX classiques (séries M, D, S, Q, T) utilisent principalement le codec propriétaire MxPEG. Si le flux RTSP échoue :

1. Essayez l'URL RTSP MJPEG : `rtsp://IP:554/mobotix.mjpeg`
2. Si RTSP n'est pas disponible, utilisez le flux MJPEG HTTP : `http://IP/cgi-bin/faststream.jpg?stream=full`
3. Vérifiez que RTSP est activé dans l'interface web de la caméra sous **Admin Menu > Network Setup > RTSP Server**

### Erreur « 401 Unauthorized »

Les caméras MOBOTIX utilisent les identifiants par défaut `admin` / `meinsm`. Si l'authentification échoue :

1. Accédez à l'interface web de la caméra sur `http://CAMERA_IP`
2. Connectez-vous avec les identifiants par défaut ou configurés
3. Vérifiez que le compte utilisateur a les autorisations d'accès au streaming
4. Utilisez les identifiants corrects dans votre URL RTSP

### Le flux MxPEG ne se décode pas

MxPEG est un codec propriétaire que les lecteurs multimédias et bibliothèques standard peuvent ne pas prendre en charge. Solutions de contournement :

- Utilisez le flux MJPEG de repli via `http://IP/cgi-bin/faststream.jpg?stream=full`
- Configurez la caméra pour générer du H.264 si le modèle et le firmware le prennent en charge
- Pour les caméras de la série MOVE, H.264 RTSP est pris en charge nativement

### La découverte ONVIF ne trouve pas la caméra

Seules les caméras de la série MOBOTIX MOVE prennent en charge ONVIF. Les caméras Mx classiques (séries M, D, S, Q, T) n'implémentent pas le protocole ONVIF. Pour les caméras classiques, connectez-vous directement en utilisant les URL RTSP ou HTTP listées ci-dessus.

### Faible fréquence d'images sur les flux MJPEG

Les caméras MOBOTIX classiques peuvent par défaut utiliser de faibles fréquences d'images pour économiser la bande passante. Pour ajuster :

1. Ouvrez l'interface web de la caméra
2. Naviguez vers **Admin Menu > Image Control > Frame Rate**
3. Augmentez la fréquence d'images maximale
4. Pour les flux HTTP, spécifiez les fps souhaités dans l'URL : `http://IP/control/faststream.jpg?stream=full&fps=15`

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras MOBOTIX ?**

Pour les caméras MOBOTIX MOVE plus récentes, l'URL par défaut est `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.h264`. Pour les caméras Mx classiques, utilisez `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.mjpeg` ou le flux MJPEG HTTP sur `http://CAMERA_IP/cgi-bin/faststream.jpg?stream=full`.

**Qu'est-ce que MxPEG et en ai-je besoin ?**

MxPEG est un codec vidéo propriétaire développé par MOBOTIX pour un streaming économe en bande passante dans leur architecture de caméra décentralisée. Vous n'avez pas besoin de la prise en charge de MxPEG pour utiliser les caméras MOBOTIX avec le SDK VisioForge. À la place, utilisez le flux MJPEG HTTP standard ou le flux RTSP H.264 (sur les modèles pris en charge) comme décrit sur cette page.

**Les caméras MOBOTIX prennent-elles en charge ONVIF ?**

Seule la série MOBOTIX MOVE prend en charge ONVIF. Les caméras MOBOTIX classiques (séries M, D, S, Q, T) utilisent une interface web propriétaire et ne prennent pas en charge la découverte ou les profils ONVIF.

**Quelle est la différence entre les caméras MOBOTIX classiques et MOVE ?**

Les caméras MOBOTIX classiques (séries M, D, S, Q, T) utilisent une architecture décentralisée avec enregistrement dans la caméra et le codec propriétaire MxPEG. Les caméras de la série MOVE sont la nouvelle gamme de produits MOBOTIX qui suit les protocoles standard de l'industrie incluant ONVIF, H.264 et H.265, ce qui les rend plus faciles à intégrer avec les solutions VMS et SDK tierces.

**Puis-je me connecter aux caméras MOBOTIX sans ONVIF ?**

Oui. Toutes les caméras MOBOTIX prennent en charge les connexions RTSP ou HTTP directes en utilisant les URL listées sur cette page. ONVIF n'est pas requis pour le streaming vidéo basique.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Basler](basler.md) — Caméras industrielles / vision par machine
- [Guide de connexion FLIR](flir.md) — Imagerie industrielle et thermique
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
