---
title: Guide de connexion RTSP Reolink en C# .NET — caméras IP
description: Modèles d'URL RTSP pour les caméras Reolink RLC, E1, Argus, CX et Duo en C# .NET. Streaming et enregistrement avec le SDK VisioForge et la découverte ONVIF.
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
  - C#

---

# Comment se connecter à une caméra IP Reolink en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Reolink** (Reolink Digital Technology Co., Ltd.) est un fabricant de caméras IP grand public et semi-professionnelles dont le siège est à Hong Kong. Fondée en 2009, Reolink a connu une croissance rapide grâce aux ventes directes aux consommateurs sur Amazon et sur son propre site web, en proposant des caméras à prix compétitifs avec un accès RTSP simple. Reolink se distingue par une documentation claire des URL RTSP et une intégration facile avec les logiciels tiers.

**Faits clés :**

- **Gammes de produits :** série RLC (PoE filaire), série RLN (NVR), série E1 (Wi-Fi pan/tilt), série Argus (batterie/solaire), série CX (vision nocturne ColorX), série Duo (double objectif), TrackMix (suivi automatique)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/HTTPS, protocole Reolink propriétaire
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (mot de passe vide ou défini lors de la configuration)
- **Prise en charge ONVIF :** Oui (la plupart des modèles actuels, peut nécessiter une activation dans les paramètres de la caméra)
- **Codecs vidéo :** H.264 (tous les modèles), H.265 (la plupart des modèles actuels)

## Modèles d'URL RTSP

Reolink utilise un modèle d'URL cohérent sur la plupart des modèles. La principale différence concerne les caméras et les NVR (qui utilisent des numéros de canal).

### URL RTSP des caméras

| Flux | URL RTSP | Résolution | Notes |
|--------|----------|------------|-------|
| Principal (clear) | `rtsp://IP:554/h264Preview_01_main` | Complète (jusqu'à 4K/8MP) | Flux principal H.264 |
| Sous-flux (fluent) | `rtsp://IP:554/h264Preview_01_sub` | Réduite (640x360) | Bande passante plus faible |

!!! info "Flux H.265"
    Pour les caméras avec H.265 activé, l'URL reste la même (`h264Preview_01_main`). Le `h264` dans le chemin de l'URL n'est pas spécifique au codec — il fonctionne pour les flux H.264 comme H.265.

### URL des canaux NVR

Pour les NVR Reolink (RLN8-410, RLN16-410, RLN36, etc.), ajoutez le numéro de canal :

| Canal | URL du flux principal | URL du sous-flux |
|---------|----------------|----------------|
| Canal 1 | `rtsp://IP:554/h264Preview_01_main` | `rtsp://IP:554/h264Preview_01_sub` |
| Canal 2 | `rtsp://IP:554/h264Preview_02_main` | `rtsp://IP:554/h264Preview_02_sub` |
| Canal 3 | `rtsp://IP:554/h264Preview_03_main` | `rtsp://IP:554/h264Preview_03_sub` |
| Canal N | `rtsp://IP:554/h264Preview_0N_main` | `rtsp://IP:554/h264Preview_0N_sub` |

### Modèles et résolutions

| Modèle | Résolution | Codec | Wi-Fi | RTSP |
|-------|-----------|-------|-------|------|
| RLC-410 | 2560x1440 (4MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-510A | 2560x1920 (5MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-520A | 2560x1920 (5MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-810A | 3840x2160 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-811A | 3840x2160 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-820A | 3840x2160 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| RLC-1212A | 4512x2512 (12MP) | H.264/H.265 | Non (PoE) | Oui |
| E1 Zoom | 2560x1920 (5MP) | H.264/H.265 | Oui | Oui |
| E1 Pro | 2560x1440 (4MP) | H.264 | Oui | Oui |
| E1 Outdoor | 2560x1920 (5MP) | H.264/H.265 | Oui | Oui |
| CX410 | 2560x1440 (4MP) | H.264/H.265 | Non (PoE) | Oui |
| CX810 | 3840x2160 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| TrackMix PoE | 3840x2160 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| Duo 2 PoE | 4608x1728 (8MP) | H.264/H.265 | Non (PoE) | Oui |
| Argus 3 Pro | 2560x1440 (4MP) | H.264 | Oui (batterie) | Oui |

!!! warning "Caméras Argus sur batterie"
    Les caméras de la série Argus alimentées par batterie prennent en charge RTSP mais déchargent rapidement la batterie en cas de streaming continu. Utilisez RTSP uniquement pour les tests ou l'enregistrement déclenché par événement, pas pour la surveillance 24/7.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Reolink avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Reolink RLC-810A, flux principal
var uri = new Uri("rtsp://192.168.1.88:554/h264Preview_01_main");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `h264Preview_01_sub` à la place de `h264Preview_01_main`. Pour les canaux NVR, modifiez le numéro de canal (par exemple, `h264Preview_03_main` pour le canal 3).

## URL de capture instantanée

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/api.cgi?cmd=Snap&channel=0&rs=abc123&user=USER&password=PASS` | Capture basée sur l'API |

## Dépannage

### RTSP ne fonctionne pas — « Connection refused »

RTSP peut nécessiter une activation sur certains modèles Reolink :

1. Ouvrez l'**application Reolink** ou l'interface web
2. Allez dans **Settings > Network > Advanced > Port Settings**
3. Assurez-vous que le **port RTSP** est activé et défini sur 554
4. Certaines versions de firmware plus anciennes ont RTSP désactivé par défaut

### Le flux H.265 ne se décode pas

Si votre caméra Reolink est configurée pour H.265 et que le flux ne se décode pas :

- Le SDK prend en charge nativement H.265, mais assurez-vous d'utiliser une version récente du SDK
- Essayez de basculer la caméra sur H.264 dans **Settings > Display > Encode** comme solution de contournement
- Le chemin d'URL RTSP (`h264Preview`) reste le même quel que soit le codec réel

### Le sous-flux affiche une faible qualité

Le sous-flux (`h264Preview_01_sub`) a intentionnellement une résolution plus faible (généralement 640x360) pour réduire la bande passante. Utilisez `h264Preview_01_main` pour la résolution complète. Vous pouvez ajuster la qualité du sous-flux dans l'application Reolink sous les paramètres Display.

### Numérotation des canaux NVR

Les canaux NVR Reolink sont indexés à partir de 1 avec un format à deux chiffres rempli de zéros : `01`, `02`, `03`... `16`. Le canal 0 n'existe pas.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Reolink ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/h264Preview_01_main` pour le flux principal et `h264Preview_01_sub` pour le sous-flux. Le mot de passe est celui que vous avez défini lors de la configuration de la caméra.

**L'URL RTSP change-t-elle lors de l'utilisation de H.265 ?**

Non. Le chemin d'URL `h264Preview_01_main` est utilisé pour les flux H.264 comme H.265. Le `h264` dans le chemin est une convention de nommage héritée, pas un sélecteur de codec.

**Puis-je accéder aux caméras Reolink à distance via RTSP ?**

RTSP est conçu pour l'accès au réseau local. Pour l'accès distant, vous devrez configurer la redirection de port sur votre routeur (redirigez le port 554 vers l'IP de la caméra) ou utiliser un VPN. L'accès cloud/P2P de Reolink utilise un protocole propriétaire, pas RTSP.

**Les caméras Reolink Duo ont-elles des flux RTSP séparés pour chaque objectif ?**

Oui. Les caméras Reolink Duo exposent l'objectif grand angle sur le canal standard et peuvent fournir des flux supplémentaires. Consultez la documentation de votre modèle spécifique pour l'accès au flux à double objectif.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Amcrest](amcrest.md) — Alternative grand public avec RTSP
- [Guide de connexion TP-Link](tp-link.md) — Caméras économiques avec RTSP natif
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
