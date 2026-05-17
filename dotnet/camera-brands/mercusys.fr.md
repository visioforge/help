---
title: Caméras IP Mercusys — URL RTSP et intégration C# .NET
description: URL RTSP des caméras Mercusys MC et MB en C# .NET. Diffusion et enregistrement avec le code d'intégration du SDK VisioForge.
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
  - C#

---

# Comment se connecter à une caméra IP Mercusys en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Mercusys** est une marque de réseautique et de maison connectée appartenant à **TP-Link**. Mercusys cible le segment soucieux du budget avec des routeurs, des systèmes mesh et des caméras de sécurité abordables. Les caméras Mercusys partagent des similitudes de conception et de firmware avec les caméras TP-Link Tapo, offrant une prise en charge RTSP standard à des prix plus bas.

**Faits clés :**

- **Gammes de produits :** MC (caméras intérieures), MB (caméras extérieures)
- **Protocoles pris en charge :** RTSP, ONVIF (modèles sélectionnés), HTTP
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Définis lors de la configuration via l'application (pas de valeurs d'usine)
- **Prise en charge ONVIF :** Oui (modèles plus récents, doit être activé)
- **Codecs vidéo :** H.264
- **Société mère :** TP-Link
- **Application compagnon :** application Mercusys Security

!!! info "Mercusys et TP-Link Tapo"
    Les caméras Mercusys partagent l'architecture du firmware avec les caméras TP-Link Tapo. Le format d'URL RTSP (`/stream1`, `/stream2`) est similaire. Si vous connaissez l'intégration Tapo, la même approche fonctionne avec Mercusys. Consultez notre [guide de connexion TP-Link](tp-link.md) pour des détails supplémentaires.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Mercusys utilisent une URL RTSP basée sur le numéro de flux :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Flux | Modèle d'URL | Description |
|------|--------------|-------------|
| Flux principal | `rtsp://IP:554/stream1` | Résolution complète |
| Sous-flux | `rtsp://IP:554/stream2` | Résolution inférieure, moins de bande passante |

### Modèles de caméras

| Modèle | Type | Résolution | URL du flux principal | Audio |
|--------|------|------------|-----------------------|-------|
| MC50 (PT intérieur) | Pan/tilt intérieur | 1920x1080 | `rtsp://IP:554/stream1` | Oui |
| MC60 (PT intérieur 2K) | Pan/tilt intérieur | 2304x1296 | `rtsp://IP:554/stream1` | Oui |
| MC70 (PT intérieur 4MP) | Pan/tilt intérieur | 2560x1440 | `rtsp://IP:554/stream1` | Oui |
| MB50 (bullet extérieur) | Extérieur | 1920x1080 | `rtsp://IP:554/stream1` | Oui |
| MB60 (extérieur 2K) | Extérieur | 2304x1296 | `rtsp://IP:554/stream1` | Oui |
| MB70 (extérieur 4MP) | Extérieur | 2560x1440 | `rtsp://IP:554/stream1` | Oui |

### Activation de RTSP / ONVIF

RTSP et ONVIF peuvent devoir être activés dans les paramètres de la caméra :

1. Ouvrez l'application **Mercusys Security**
2. Sélectionnez votre caméra → **Settings**
3. Naviguez vers **Advanced Settings**
4. Activez **RTSP** et/ou **ONVIF**
5. Définissez un nom d'utilisateur et un mot de passe pour l'accès RTSP

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Mercusys avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Mercusys MC70 (pan/tilt intérieur 4MP), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "rtsp_user"; // défini dans l'application Mercusys Security
var password = "rtsp_pass";
```

Pour accéder au sous-flux, utilisez `/stream2` au lieu de `/stream1`.

## URL des snapshots

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification basique |

## Dépannage

### RTSP non accessible

Les caméras Mercusys nécessitent une configuration initiale via l'application Mercusys Security. RTSP peut également devoir être explicitement activé dans les paramètres avancés. Après l'activation de RTSP, les identifiants définis dans l'application doivent être utilisés pour l'authentification RTSP.

### Découverte de l'IP de la caméra

Trouvez l'adresse IP de votre caméra Mercusys dans :

1. L'application Mercusys Security → Device Info
2. La liste des clients DHCP de votre routeur
3. La découverte ONVIF (si activée)

### Similaire à TP-Link Tapo

Si le dépannage standard Mercusys ne résout pas votre problème, consultez notre [guide TP-Link Tapo](tp-link.md) pour des étapes de dépannage supplémentaires, car le firmware est similaire.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Mercusys ?**

Les caméras Mercusys utilisent `rtsp://username:password@CAMERA_IP:554/stream1` pour le flux principal et `/stream2` pour le sous-flux. Le nom d'utilisateur et le mot de passe sont définis lors de l'activation de RTSP dans l'application Mercusys Security.

**Mercusys est-elle la même chose que TP-Link ?**

Mercusys est une marque appartenant à TP-Link qui cible le segment budget. Les caméras Mercusys partagent l'architecture du firmware avec les caméras TP-Link Tapo et utilisent des formats d'URL RTSP similaires.

**Les caméras Mercusys prennent-elles en charge ONVIF ?**

Les modèles de caméras Mercusys plus récents prennent en charge ONVIF, mais il doit être activé via l'application Mercusys Security. Les modèles plus anciens peuvent ne pas inclure la prise en charge ONVIF.

**Comment les caméras Mercusys se comparent-elles à TP-Link Tapo ?**

Les caméras Mercusys sont positionnées comme une alternative plus abordable à Tapo. Elles partagent un firmware similaire et des modèles d'URL RTSP. Les caméras Tapo ont généralement plus d'options de modèles et un support communautaire plus large.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion TP-Link](tp-link.md) — Société mère, firmware similaire
- [Guide de connexion Tenda](tenda.md) — Alternative caméra budget
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
