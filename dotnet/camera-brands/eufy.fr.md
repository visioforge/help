---
title: Connexion RTSP des caméras Eufy Security en C# .NET
description: Intégration RTSP des caméras Eufy en C# .NET. Modèles eufyCam, SoloCam et Indoor Cam compatibles RTSP/ONVIF, exemples SDK VisioForge.
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

# Comment se connecter à une caméra Eufy Security en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Eufy Security** est une marque de sécurité de maison connectée détenue par **Anker Innovations**, dont le siège est à Changsha, en Chine. Eufy est réputée pour son stockage local (sans abonnement cloud obligatoire), sa détection basée sur l'IA et une large gamme de caméras intérieures, extérieures, à batterie et sonnettes. La prise en charge RTSP et ONVIF varie considérablement selon le modèle et la version du firmware.

**Faits clés :**

- **Gammes de produits :** eufyCam (batterie), SoloCam (autonome), Indoor Cam, Floodlight Cam, Video Doorbell, HomeBase
- **Prise en charge des protocoles :** RTSP (modèles sélectionnés, doit être activé), ONVIF (firmware plus récent), application Eufy Security
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Pas de valeurs par défaut standard — définis lors de l'activation RTSP
- **Prise en charge ONVIF :** Ajoutée dans les mises à jour récentes du firmware pour de nombreux modèles
- **Codecs vidéo :** H.264, H.265 (modèles sélectionnés)
- **Stockage local :** Oui — HomeBase ou microSD de la caméra (pas de cloud requis pour l'enregistrement)

!!! info "La prise en charge RTSP/ONVIF varie selon le modèle"
    Eufy a progressivement ajouté la prise en charge RTSP et ONVIF dans sa gamme de produits via des mises à jour de firmware. Tous les modèles ne prennent pas en charge ces fonctionnalités. Vérifiez les paramètres de l'application Eufy Security pour votre caméra spécifique pour voir si RTSP est disponible.

## Prise en charge RTSP par modèle

| Modèle | RTSP | ONVIF | Notes |
|-------|------|-------|-------|
| eufyCam 2 / 2 Pro | Oui (via HomeBase) | Oui | Nécessite HomeBase 2 |
| eufyCam 2C / 2C Pro | Oui (via HomeBase) | Oui | Nécessite HomeBase 2 |
| eufyCam 3 / 3C | Oui (via HomeBase 3) | Oui | Nécessite HomeBase 3 |
| eufyCam S330 | Oui (via HomeBase 3) | Oui | Modèle 4K |
| SoloCam S340 | Oui | Oui | Double objectif, RTSP autonome |
| SoloCam C210 | Oui | Oui | Autonome avec RTSP |
| Indoor Cam 2K | Oui | Oui | Wi-Fi, dépend du firmware |
| Indoor Cam Pan & Tilt | Oui | Oui | Wi-Fi, dépend du firmware |
| Floodlight Cam 2 Pro | Oui | Oui | Filaire |
| Video Doorbell 2K | Limité | Non | Via HomeBase uniquement |
| Video Doorbell Dual | Limité | Non | Via HomeBase uniquement |

## Activation de RTSP

### Pour les caméras connectées à HomeBase (série eufyCam)

1. Ouvrez l'application **Eufy Security**
2. Allez dans **HomeBase Settings > Storage > NAS** ou **RTSP**
3. Activez le streaming RTSP
4. Définissez un nom d'utilisateur et un mot de passe RTSP
5. Notez l'URL RTSP affichée pour chaque caméra

### Pour les caméras autonomes (SoloCam, Indoor Cam, Floodlight)

1. Ouvrez l'application **Eufy Security**
2. Sélectionnez votre caméra → **Settings** (icône d'engrenage)
3. Recherchez **RTSP** ou **Advanced > RTSP Stream**
4. Activez RTSP et définissez les identifiants
5. Notez l'URL RTSP fournie

## Modèles d'URL RTSP

### Format d'URL standard

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live0
```

| Flux | Modèle d'URL | Description |
|--------|-------------|-------------|
| Flux principal | `rtsp://IP:554/live0` | Résolution complète |
| Sous-flux | `rtsp://IP:554/live1` | Résolution inférieure |

### URL RTSP HomeBase

Lorsqu'elle est connectée via une HomeBase, l'URL RTSP pointe vers l'IP de la HomeBase :

```
rtsp://[USERNAME]:[PASSWORD]@[HOMEBASE_IP]:554/live0
```

Pour plusieurs caméras sur une seule HomeBase, chaque caméra obtient un chemin de flux unique affiché dans l'application.

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/live0` | Flux principal (courant) |
| `rtsp://IP:554/live1` | Sous-flux |
| `rtsp://IP:554/stream1` | Format alternatif (certains modèles) |
| `rtsp://IP:554/h264_stream` | H.264 explicite (certains firmwares) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Eufy avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Eufy SoloCam S340, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/live0");
var username = "rtsp_user"; // défini dans l'application Eufy Security
var password = "rtsp_pass";
```

Pour accéder au sous-flux, utilisez `/live1` au lieu de `/live0`.

## Dépannage

### L'option RTSP n'est pas visible dans l'application

La prise en charge RTSP nécessite des versions spécifiques de firmware. Mettez à jour le firmware de votre caméra et de votre HomeBase via l'application Eufy Security. Si RTSP n'apparaît toujours pas, votre modèle peut ne pas encore le prendre en charge.

### HomeBase vs RTSP autonome

- **Caméras HomeBase** (série eufyCam) : Les flux RTSP proviennent de l'**IP de la HomeBase**, pas de l'IP de la caméra. La HomeBase agit comme un proxy.
- **Caméras autonomes** (SoloCam, Indoor Cam) : Les flux RTSP proviennent directement de l'**IP de la caméra**.

### Coupures de flux sur les caméras à batterie

Les modèles eufyCam alimentés par batterie peuvent arrêter le streaming RTSP lorsqu'ils sont en mode veille. La caméra doit être en enregistrement actif ou en mode « always streaming » pour un accès RTSP continu. Cela a un impact significatif sur l'autonomie de la batterie.

### Découverte ONVIF

Le firmware Eufy plus récent prend en charge la découverte ONVIF. Utilisez ONVIF pour trouver automatiquement les caméras sur votre réseau au lieu de configurer manuellement les URL RTSP.

### Incohérences de firmware

Eufy a déployé la prise en charge RTSP/ONVIF progressivement. Différentes caméras de votre installation peuvent avoir des capacités différentes selon leur version de firmware. Mettez toujours à jour tous les appareils avec le firmware le plus récent.

## FAQ

**Les caméras Eufy prennent-elles en charge RTSP ?**

De nombreuses caméras Eufy prennent désormais en charge RTSP, mais cela doit être activé dans l'application Eufy Security et varie selon le modèle. Les caméras connectées à HomeBase diffusent RTSP via la HomeBase, tandis que les caméras autonomes diffusent directement. Vérifiez les capacités de votre modèle spécifique dans les paramètres de l'application.

**Les caméras Eufy nécessitent-elles un abonnement cloud pour RTSP ?**

Non. Le streaming RTSP fonctionne localement sans aucun abonnement cloud. Les caméras Eufy stockent les enregistrements sur la HomeBase ou la carte microSD de la caméra. L'abonnement cloud (Eufy Security Plan) est optionnel et fournit un stockage cloud supplémentaire et d'autres fonctionnalités.

**Puis-je utiliser les caméras Eufy sans l'application Eufy ?**

La configuration initiale nécessite l'application Eufy Security. Après la configuration et l'activation de RTSP, vous pouvez accéder au flux RTSP sans l'application. Cependant, les mises à jour de firmware et les modifications de configuration nécessitent toujours l'application.

**Quelle est la différence entre le RTSP HomeBase et le RTSP autonome ?**

Le RTSP HomeBase diffuse toutes les caméras connectées via l'adresse IP de la HomeBase. La HomeBase agit comme une passerelle. Les caméras autonomes (SoloCam, Indoor Cam, Floodlight) diffusent directement depuis leur propre IP. Le RTSP HomeBase peut avoir une latence légèrement plus élevée.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Arlo](arlo.md) — Alternative grand public (pas de RTSP)
- [Guide de connexion Reolink](reolink.md) — Grand public avec RTSP natif
- [Guide de connexion EZVIZ](ezviz.md) — Caméras de maison connectée avec RTSP
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
