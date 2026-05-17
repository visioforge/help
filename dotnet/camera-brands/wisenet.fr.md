---
title: URL RTSP Wisenet — Connexion caméra IP avec SDK C# .NET
description: Modèles d'URL RTSP pour les séries Wisenet X, P, Q et L en C# .NET. Intégration des caméras et NVR Hanwha Vision avec ONVIF et le SDK VisioForge.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Wisenet en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Wisenet** est le nom de marque produit utilisé par **Hanwha Vision** (anciennement Hanwha Techwin / Samsung Techwin) pour toutes les caméras IP, NVR et systèmes de gestion vidéo. Wisenet n'est pas une société distincte mais plutôt le nom de famille de produits utilisé dans l'ensemble de la gamme de vidéosurveillance Hanwha Vision.

**Faits clés :**

- **Fabricant :** Hanwha Vision (Corée du Sud)
- **Niveaux de produits :** X (premium), P (IA), Q (grand public), Q mini (compact), L (économique), T (thermique)
- **Prise en charge des protocoles :** RTSP, ONVIF Profile S/G/T, SUNAPI (propriétaire)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de l'activation)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265, WiseStream III, MJPEG

!!! info "Wisenet = Produits Hanwha Vision"
    Wisenet est la **marque produit**, Hanwha Vision est l'**entreprise**. Toutes les caméras Wisenet utilisent les mêmes modèles d'URL RTSP. Pour des instructions de connexion détaillées, y compris l'accès aux canaux NVR et le dépannage, consultez notre [guide de connexion Hanwha Vision](hanwha.md). Pour les caméras de marque Samsung héritées, consultez le [guide Samsung/Hanwha](samsung.md).

## Modèles d'URL RTSP

### Format d'URL standard

Toutes les caméras Wisenet partagent la même structure d'URL basée sur les profils :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

### Par niveau de produit

| Niveau Wisenet | Exemples de modèles | URL du flux principal | Caractéristique clé |
|-------------|---------------|----------------|-------------|
| **Série X** (premium) | XNO-6080R, XND-8080RV, XNP-6120H | `rtsp://IP:554/profile2/media.smp` | Meilleur WDR, faible luminosité |
| **Série P** (IA) | PNO-A9081R, PND-A9081RV | `rtsp://IP:554/profile2/media.smp` | Analyses par apprentissage profond |
| **Série Q** (grand public) | QNO-8080R, QND-8080R, QNE-8021R | `rtsp://IP:554/profile2/media.smp` | Fonctionnalités/prix équilibrés |
| **Q mini** (compact) | QND-8021 | `rtsp://IP:554/profile2/media.smp` | Facteur de forme discret |
| **Série L** (économique) | LNO-6032R, LND-6032R | `rtsp://IP:554/profile2/media.smp` | Entrée de gamme |
| **Série T** (thermique) | TNO-4030T, TNO-4050T | `rtsp://IP:554/profile2/media.smp` | Thermique + visible |

### Modèles multi-capteurs et multidirectionnels

| Type de modèle | URL du flux | Notes |
|-----------|-----------|-------|
| Capteur unique | `rtsp://IP:554/profile2/media.smp` | Standard |
| Multi-capteurs canal 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | Premier capteur |
| Multi-capteurs canal 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | Deuxième capteur |
| Panoramique assemblé | `rtsp://IP:554/profile2/media.smp/trackID=channel5` | Vue combinée |

### Accès NVR / WAVE VMS

Pour les NVR Wisenet (séries XRN, QRN, LRN) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Caméra 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Caméra N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Wisenet avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Wisenet QNO-8080R (série Q 5MP), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/profile3/media.smp` à la place de `/profile2/media.smp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Nécessite une authentification digest |
| Flux MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | MJPEG continu |

## Dépannage

### Quel numéro de profil correspond au flux principal ?

Les caméras Wisenet utilisent généralement `profile2` comme flux principal (de la plus haute qualité). C'est différent de la plupart des autres marques. Si vous obtenez des résultats inattendus, vérifiez la configuration du profil dans l'interface web de la caméra (**Setup > Video/Audio > Video Profile**).

### Économies de bande passante WiseStream III

WiseStream III ajuste dynamiquement l'encodage par région dans l'image. La sortie est du H.265 ou H.264 standard — aucun décodeur spécial n'est nécessaire. Les paramètres WiseStream peuvent être configurés dans l'interface web de la caméra.

### Activation de la caméra

Les nouvelles caméras Wisenet nécessitent une activation (définition d'un mot de passe) avant utilisation. Utilisez l'utilitaire Wisenet Installation Wizard, un navigateur web ou l'application mobile Wisenet pour la configuration initiale.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Wisenet ?**

Toutes les caméras Wisenet utilisent `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` pour le flux principal. Utilisez `profile3` pour le sous-flux.

**Quelle est la différence entre Wisenet X, P, Q et L ?**

**X** = entreprise premium. **P** = analyses propulsées par l'IA. **Q** = grand public professionnel. **L** = économique/entrée de gamme. **T** = thermique. Tous les niveaux utilisent le même format d'URL RTSP.

**Wisenet est-il identique aux caméras de sécurité Samsung ?**

Wisenet est la marque produit actuelle de Hanwha Vision, qui a acquis la division sécurité de Samsung en 2015. Les caméras Samsung Techwin héritées peuvent utiliser des formats d'URL différents. Consultez notre [guide Samsung/Hanwha](samsung.md) pour les anciens modèles.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Hanwha Vision](hanwha.md) — Intégration Hanwha Vision détaillée
- [Guide hérité Samsung/Hanwha](samsung.md) — Anciens modèles Samsung Techwin
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
