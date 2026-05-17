---
title: Connecter une caméra IP Hanwha Vision en C# .NET — RTSP
description: Modèles d'URL RTSP Hanwha Vision X, Q, P, L et NVR Wisenet pour C# .NET. Intégration compatible ONVIF avec exemples de code SDK VisioForge.
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

# Comment se connecter à une caméra IP Hanwha Vision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Hanwha Vision** (anciennement Hanwha Techwin, anciennement Samsung Techwin) est un fabricant sud-coréen de vidéosurveillance et une filiale du groupe Hanwha. Hanwha a acquis la division sécurité de Samsung en 2015 et a été rebaptisé Hanwha Vision en 2023. Toutes les caméras sont vendues sous la marque produit **Wisenet**. Hanwha Vision est l'un des cinq plus grands fabricants mondiaux de vidéosurveillance avec une forte présence sur les marchés entreprise et gouvernementaux.

**Faits clés :**

- **Gammes de produits :** Wisenet X (haut de gamme), Wisenet P (IA/4K), Wisenet Q (grand public), Wisenet L (économique), Wisenet T (thermique)
- **Prise en charge des protocoles :** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SUNAPI (propriétaire)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de la configuration initiale ; modèles plus anciens : admin / 4321)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265 (WiseStream II), MJPEG
- **Marque produit :** Wisenet (voir aussi notre [guide Samsung/Hanwha](samsung.md) pour les URL Samsung Techwin historiques)

!!! info "Hanwha Vision vs Samsung vs Wisenet"
    **Hanwha Vision** est le nom de la société (depuis 2023). **Wisenet** est la marque produit pour toutes les caméras et NVR. **Samsung Techwin** était le précédent nom de la société (avant 2015). Notre [guide Samsung/Hanwha](samsung.md) couvre les modèles historiques de marque Samsung. Cette page couvre les produits Hanwha Vision / Wisenet actuels.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Hanwha Vision utilisent une structure d'URL RTSP basée sur des profils :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `profile1` | Profil 1 | Généralement configuré comme flux principal |
| `profile2` | Profil 2 | Généralement configuré comme sous-flux |
| `profile3` | Profil 3 | Troisième flux (si configuré) |

### Modèles de caméras

| Série de modèles | Résolution | URL du flux principal | Audio |
|-------------|-----------|----------------|-------|
| XNO-6080R (bullet X 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Oui |
| XNO-8080R (bullet X 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Oui |
| XNO-9080R (bullet X 4K) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Oui |
| XND-6080 (dôme X 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Oui |
| XND-8080RV (dôme X 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Oui |
| XNP-6120H (PTZ X 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Oui |
| PNO-A9081R (bullet IA P 4K) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Oui |
| QNO-8080R (bullet Q 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Oui |
| QND-8080R (dôme Q 5MP) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Oui |
| LNO-6032R (bullet L 2MP) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Non |

!!! tip "Numérotation des profils"
    Sur la plupart des caméras Hanwha Vision, `profile2` est le flux principal et `profile1` est réservé à un usage interne. Si `profile2` ne fonctionne pas, essayez `profile1` ou `profile3`. Vous pouvez vérifier les attributions de profils dans l'interface web de la caméra sous **Video Profile**.

### URL des canaux NVR

Pour les NVR Wisenet (séries XRN, QRN, LRN) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Caméra 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Caméra N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/profile2/media.smp` | Standard (recommandé) |
| `rtsp://IP:554/profile1/media.smp` | Premier profil |
| `rtsp://IP:554/onvif-media/media.amp` | Service média ONVIF |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Certaines variantes OEM |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Hanwha Vision avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Hanwha Vision XNO-8080R (Wisenet X 5MP), flux principal
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `/profile3/media.smp` au lieu de `/profile2/media.smp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Capture pleine résolution |
| Capture JPEG (simple) | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification digest |
| Flux MJPEG | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | MJPEG continu |

## Dépannage

### Confusion profile2 vs profile1

Les caméras Hanwha Vision attribuent généralement `profile2` comme flux principal (qualité la plus élevée), ce qui diffère de la plupart des autres marques qui utilisent le profil/canal 1. Si vous n'obtenez aucune vidéo ou une faible résolution depuis `profile2`, vérifiez la configuration du profil dans l'interface web de la caméra sous **Video Profile**.

### Activation du mot de passe requise

Les caméras Hanwha Vision actuelles sont livrées sans mot de passe par défaut. Vous devez activer la caméra et définir un mot de passe via :

1. Assistant d'installation Wisenet (outil IP Installer)
2. Navigateur web à `http://CAMERA_IP`
3. Application mobile Wisenet

Les anciens modèles Samsung Techwin utilisaient `admin` / `4321` comme valeurs par défaut.

### Codec WiseStream II

WiseStream II est la technologie d'encodage dynamique de Hanwha qui ajuste la compression par région dans l'image. Elle produit des flux H.265 ou H.264 standard compatibles avec n'importe quel décodeur. Aucun codec spécial n'est requis.

### SUNAPI vs ONVIF

Les caméras Hanwha Vision prennent en charge à la fois leur SUNAPI propriétaire et l'ONVIF standard. Pour l'intégration au SDK VisioForge, utilisez soit les URL RTSP ci-dessus, soit la découverte ONVIF. SUNAPI est principalement utilisé par le VMS de Hanwha (SSM/Wisenet WAVE).

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Hanwha Vision (Wisenet) ?**

L'URL standard est `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` pour le flux principal. Utilisez `profile3` pour le sous-flux. Les numéros de profil peuvent être personnalisés dans l'interface web de la caméra.

**Les caméras Hanwha Vision et Samsung sont-elles identiques ?**

Hanwha Vision a acquis la division caméras de sécurité de Samsung en 2015 (alors nommée Samsung Techwin, puis Hanwha Techwin, désormais Hanwha Vision). Les caméras actuelles sont vendues sous la marque **Wisenet**. Les caméras historiques de marque Samsung peuvent utiliser des modèles d'URL différents — consultez notre [guide Samsung/Hanwha](samsung.md).

**Quelle est la différence entre les séries Wisenet X, P, Q et L ?**

**X** = entreprise haut de gamme (meilleure faible luminosité, WDR). **P** = avec IA (analyses deep learning). **Q** = entreprise grand public (bon équilibre fonctionnalités/prix). **L** = entrée de gamme (fonctionnalités basiques, prix compétitif). **T** = imagerie thermique.

**Les caméras Hanwha Vision prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Hanwha Vision actuelles prennent en charge ONVIF Profile S, G et T. ONVIF fournit une découverte, un streaming et un contrôle PTZ standardisés.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide historique Samsung/Hanwha](samsung.md) — Anciens modèles Samsung Techwin
- [Guide produit Wisenet](wisenet.md) — Détails de la famille de produits Wisenet
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
