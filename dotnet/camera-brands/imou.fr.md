---
title: Caméra IP Imou en C# .NET — RTSP, ONVIF et streaming SDK
description: Connectez les caméras Imou (Cruiser, Ranger, Bullet, Cell) aux applis C# / .NET via RTSP/ONVIF. Identifiants par défaut, URL de flux, configs H.264/H.265. Code.
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

# Comment se connecter à une caméra IP Imou en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Imou** (prononcer « i-mou ») est une marque grand public de caméras de sécurité et de maison connectée détenue par **Dahua Technology**. Lancée en 2019, Imou cible le marché grand public et petites entreprises avec des caméras Wi-Fi, des caméras à batterie, des sonnettes et des kits de sécurité résidentielle. Les caméras Imou utilisent le firmware Dahua et les modèles d'URL RTSP de Dahua.

**Faits clés :**

- **Gammes de produits :** Cruiser (PT extérieur), Ranger (PT intérieur), Bullet (fixe extérieure), Cell (batterie), Versa (polyvalente), Rex (intérieure)
- **Prise en charge des protocoles :** RTSP (doit être activé sur certains modèles), ONVIF (modèles sélectionnés), cloud Imou Life
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (ou admin / imou + suffixe du numéro de série)
- **Prise en charge ONVIF :** Oui (la plupart des modèles filaires)
- **Codecs vidéo :** H.264, H.265 (modèles sélectionnés)
- **Société mère :** Dahua Technology

!!! info "Imou = marque grand public Dahua"
    Les caméras Imou utilisent le firmware Dahua et le même format d'URL RTSP `cam/realmonitor` que les caméras Dahua. Consultez notre [guide de connexion Dahua](dahua.md) pour plus de détails.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Imou utilisent le modèle d'URL Dahua `cam/realmonitor` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel` | 1 | Canal de la caméra (toujours 1 pour les caméras autonomes) |
| `subtype` | 0 | Flux principal (résolution la plus élevée) |
| `subtype` | 1 | Sous-flux (résolution inférieure, moins de bande passante) |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Audio |
|-------|------|----------------|-------|
| Cruiser SE+ 4MP | PTZ extérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Cruiser 2E 4MP | PTZ extérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Ranger 2 (IPC-A22EP) | PTZ intérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Ranger SE 4MP | PTZ intérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Rex 3D (IPC-GS7EP) | PTZ intérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Bullet 2E (IPC-F22FP) | Fixe extérieure | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Bullet 2S (IPC-F26FP) | Fixe extérieure | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Versa 4MP | Intérieur/extérieur | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| Cell 2 | Batterie extérieure | Limité — voir note | Oui |
| Cell Go | Mini batterie | Pas de RTSP | Non |

!!! warning "Modèles batterie"
    Les caméras Imou alimentées par batterie (série Cell) ont une prise en charge RTSP limitée ou nulle. La Cell 2 peut prendre en charge RTSP lorsqu'elle est connectée à la station de base Imou, mais la Cell Go et d'autres mini caméras à batterie sont des appareils cloud uniquement.

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Standard (recommandé) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Forcer l'unicast |
| `rtsp://IP:554/live` | Chemin simple (certains modèles) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Imou avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Imou Cruiser SE+ 4MP, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `subtype=1` au lieu de `subtype=0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Nécessite une authentification basique |
| Flux MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continu |

## Dépannage

### RTSP non accessible

Certaines caméras Imou nécessitent l'activation de RTSP via l'application Imou Life :

1. Ouvrez l'application **Imou Life** → sélectionnez votre caméra
2. Allez dans **Settings > Advanced Settings > RTSP**
3. Activez RTSP et notez le mot de passe (peut différer du mot de passe de l'application)

### Identifiants par défaut

Les valeurs par défaut du mot de passe Imou varient selon le modèle et le firmware :

- `admin` / `admin` (courant sur les modèles plus anciens)
- `admin` / code spécifique (vérifiez l'étiquette de la caméra)
- Mot de passe personnalisé défini lors de la configuration de l'application Imou Life

Si la connexion RTSP échoue, vérifiez le mot de passe RTSP dans les paramètres de l'application Imou Life.

### Adresse IP des caméras Wi-Fi

Les caméras Wi-Fi Imou obtiennent leur IP de votre routeur via DHCP. Trouvez l'IP locale de la caméra dans :

1. L'application Imou Life → Device Info
2. La liste des clients DHCP de votre routeur
3. La découverte ONVIF (si prise en charge)

### Interface web Dahua

Certaines caméras Imou exposent l'interface web Dahua à `http://CAMERA_IP`. Cela fournit des options de configuration supplémentaires au-delà de l'application Imou Life, notamment les paramètres RTSP, l'encodage vidéo et la configuration réseau.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Imou ?**

L'URL standard est `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` pour le flux principal. C'est le même format que les caméras Dahua. RTSP peut nécessiter d'être activé d'abord dans l'application Imou Life.

**Imou est-il identique à Dahua ?**

Imou est une marque grand public détenue par Dahua Technology. Les caméras Imou utilisent le firmware Dahua et le même format d'URL RTSP (`cam/realmonitor`). Les principales différences sont la marque, les fonctionnalités orientées grand public et l'intégration au service cloud.

**Puis-je utiliser les caméras Imou sans le cloud ?**

Partiellement. Vous pouvez accéder aux flux RTSP localement sans le cloud Imou pour la visualisation en direct et l'enregistrement. Cependant, la configuration initiale de la caméra nécessite l'application Imou Life. Les fonctionnalités dépendant du cloud comme les alertes intelligentes, le stockage cloud et l'accès à distance nécessitent un abonnement Imou.

**Les caméras Imou prennent-elles en charge ONVIF ?**

La plupart des caméras Imou filaires et connectées en Wi-Fi prennent en charge ONVIF. Les modèles alimentés par batterie ne le font généralement pas. Vérifiez les spécifications de votre caméra dans l'application Imou Life.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Société mère, format d'URL identique
- [Guide de connexion Amcrest](amcrest.md) — Une autre marque OEM Dahua
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
