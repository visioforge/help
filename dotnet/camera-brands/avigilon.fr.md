---
title: Modèles d'URL RTSP Avigilon et configuration C# .NET
description: Modèles d'URL RTSP Avigilon H5A, H5M, H5 Pro, H5SL et NVR Unity pour C# .NET. Intégration de caméras entreprise avec exemples de code SDK VisioForge.
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

# Comment se connecter à une caméra IP Avigilon en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Avigilon** (Avigilon Corporation) est un fabricant de caméras de sécurité pour entreprise initialement basé à Vancouver, au Canada. Fondée en 2004, Avigilon a été rachetée par **Motorola Solutions** en 2018 pour environ 1 milliard de dollars. La société est connue pour ses caméras haute résolution (jusqu'à 61MP), ses analyses vidéo basées sur l'IA, dont la Unusual Motion Detection (UMD) et l'Appearance Search, et sa technologie propriétaire HDSM (High Definition Stream Management). Les caméras Avigilon sont largement déployées dans les environnements entreprise, gouvernementaux et d'infrastructures critiques.

**Faits clés :**

- **Gammes de produits :** H5A (bullet/dôme), H5M (mini dôme), H5 Pro (multi-capteurs), H5SL (gamme économique), Unity (NVR)
- **Anciennes gammes :** HD Pro, HD Multisensor, HD Micro Dome, HD PTZ
- **Prise en charge des protocoles :** RTSP, ONVIF (Profile S, Profile T), HTTP
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (doit être modifié à la configuration initiale)
- **Prise en charge ONVIF :** Oui (Profile S, Profile T)
- **Codecs vidéo :** H.264, H.265, HDSM SmartCodec (basé sur H.265)
- **Connu pour :** Analyses IA (Unusual Motion Detection, Appearance Search), HDSM SmartCodec

!!! info "Avigilon fait désormais partie de Motorola Solutions"
    Avigilon fait désormais partie de Motorola Solutions. La gamme de caméras Avigilon continue sous la division Video Security & Access Control de Motorola Solutions. Voir aussi notre [guide Pelco](pelco.md) pour une autre marque de caméras Motorola Solutions.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Avigilon utilisent le modèle d'URL `defaultPrimary` / `defaultSecondary` avec un paramètre de type de flux unicast :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/defaultPrimary?streamType=u
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `defaultPrimary` | Flux principal | Flux principal (résolution la plus élevée) |
| `defaultSecondary` | Flux secondaire | Sous-flux (résolution inférieure, moins de bande passante) |
| `streamType` | `u` | Livraison de flux en unicast |

### Modèles de caméras

| Série de modèles | Type | URL du flux principal | Notes |
|-------------|------|----------------|-------|
| H5A Bullet | Bullet fixe | `rtsp://IP:554/defaultPrimary?streamType=u` | Compatible IA, jusqu'à 8MP |
| H5A Dome | Dôme fixe | `rtsp://IP:554/defaultPrimary?streamType=u` | Compatible IA, jusqu'à 8MP |
| H5M Mini Dome | Mini dôme | `rtsp://IP:554/defaultPrimary?streamType=u` | Format compact |
| H5 Pro Multi-capteurs | Multi-capteurs | `rtsp://IP:554/defaultPrimary0?streamType=u` | Voir la note multi-capteurs ci-dessous |
| H5SL Bullet | Bullet économique | `rtsp://IP:554/defaultPrimary?streamType=u` | Gamme à prix réduit |
| H5SL Dome | Dôme économique | `rtsp://IP:554/defaultPrimary?streamType=u` | Gamme à prix réduit |
| HD Pro | Haute résolution historique | `rtsp://IP:554/defaultPrimary?streamType=u` | Jusqu'à 61MP |

### URL des caméras multi-capteurs

Pour Avigilon H5 Pro et autres caméras multi-capteurs, chaque tête de capteur possède son propre index de flux :

| Capteur | Flux principal | Sous-flux |
|--------|-------------|------------|
| Capteur 1 | `rtsp://IP:554/defaultPrimary0?streamType=u` | `rtsp://IP:554/defaultSecondary0?streamType=u` |
| Capteur 2 | `rtsp://IP:554/defaultPrimary1?streamType=u` | `rtsp://IP:554/defaultSecondary1?streamType=u` |
| Capteur 3 | `rtsp://IP:554/defaultPrimary2?streamType=u` | `rtsp://IP:554/defaultSecondary2?streamType=u` |
| Capteur 4 | `rtsp://IP:554/defaultPrimary3?streamType=u` | `rtsp://IP:554/defaultSecondary3?streamType=u` |

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/defaultPrimary?streamType=u` | Principal standard (recommandé) |
| `rtsp://IP:554/defaultSecondary?streamType=u` | Secondaire / sous-flux |
| `rtsp://IP:554/defaultPrimary0?streamType=u` | Variante du flux principal (utilisé aussi pour le capteur 1 multi-capteurs) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Avigilon avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Avigilon H5A dôme, flux principal (unicast)
var uri = new Uri("rtsp://192.168.1.100:554/defaultPrimary?streamType=u");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `defaultSecondary` à la place de `defaultPrimary`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/snapshot.jpg` | Nécessite une authentification basique |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras Avigilon nécessitent que le mot de passe par défaut soit modifié lors de la configuration initiale. Si vous n'avez pas encore configuré la caméra :

1. Accédez à la caméra à l'adresse `http://CAMERA_IP` dans un navigateur
2. Complétez l'assistant de configuration initial et définissez un mot de passe fort
3. Utilisez ces identifiants dans votre URL RTSP

### Flux HDSM SmartCodec

Le HDSM SmartCodec d'Avigilon est basé sur H.265. Assurez-vous que votre décodeur prend en charge H.265 lors de la connexion à des caméras configurées pour utiliser HDSM SmartCodec. Si vous rencontrez des problèmes de décodage, essayez de basculer la caméra sur l'encodage H.264 standard dans l'interface web de la caméra.

### Paramètre de type de flux

Le paramètre `streamType=u` demande une livraison unicast. Si vous omettez ce paramètre, la caméra peut basculer par défaut sur le multicast, ce qui peut causer des problèmes sur les réseaux non configurés pour le routage multicast.

### Les caméras multi-capteurs n'affichent qu'une seule vue

Pour les modèles multi-capteurs (H5 Pro), chaque capteur est accessible comme un flux séparé. Utilisez `defaultPrimary0`, `defaultPrimary1`, etc. pour accéder aux têtes de capteurs individuelles. Voir le tableau d'URL multi-capteurs ci-dessus.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Avigilon ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/defaultPrimary?streamType=u` pour le flux principal. Utilisez `defaultSecondary` au lieu de `defaultPrimary` pour le sous-flux à résolution inférieure.

**Les caméras Avigilon prennent-elles en charge ONVIF ?**

Oui. Les caméras Avigilon prennent en charge ONVIF Profile S et Profile T. ONVIF peut être activé via l'interface web de la caméra ou le logiciel Avigilon Control Center (ACC).

**Qu'est-ce que HDSM SmartCodec ?**

HDSM (High Definition Stream Management) SmartCodec est la technologie de compression propriétaire d'Avigilon basée sur H.265. Elle réduit les besoins en bande passante et en stockage en encodant intelligemment différentes régions de l'image à différents niveaux de qualité tout en conservant les détails dans les zones d'intérêt. Les flux encodés avec HDSM SmartCodec sont compatibles avec les décodeurs H.265 standard.

**Puis-je utiliser les caméras Avigilon sans Avigilon Control Center ?**

Oui. Bien qu'Avigilon Control Center (ACC) soit le VMS recommandé, les caméras exposent des flux RTSP standard et prennent en charge ONVIF, permettant l'intégration avec n'importe quelle application compatible RTSP, y compris les SDK VisioForge.

**Comment accéder aux capteurs individuels sur les caméras multi-capteurs ?**

Chaque tête de capteur sur une caméra multi-capteurs (comme l'H5 Pro) possède sa propre URL de flux. Utilisez `defaultPrimary0` pour le capteur 1, `defaultPrimary1` pour le capteur 2, et ainsi de suite. Chaque capteur peut également avoir un flux secondaire accessible via `defaultSecondary0`, `defaultSecondary1`, etc.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Pelco](pelco.md) — Également Motorola Solutions, caméras entreprise
- [Capture ONVIF avec post-traitement](../mediablocks/Guides/onvif-capture-with-postprocessing.md) — Pipeline de capture ONVIF Avigilon
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
