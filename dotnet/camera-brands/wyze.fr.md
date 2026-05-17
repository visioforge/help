---
title: Connexion à une caméra Wyze en C# .NET — RTSP et streaming
description: Connectez-vous aux caméras Wyze en C# .NET avec firmware RTSP ou pont RTSP Docker. Limitations RTSP, contournements et approches alternatives expliqués.
meta:
  - name: robots
    content: "noindex, follow"
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

# Comment se connecter à une caméra Wyze en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Wyze Labs** est une société américaine d'électronique grand public basée à Kirkland, dans l'État de Washington. Connue pour ses caméras domotiques extrêmement abordables, Wyze est devenue l'une des marques de caméras les plus vendues en Amérique du Nord. Cependant, les caméras Wyze sont des **appareils cloud-first** avec une prise en charge RTSP très limitée, ce qui rend l'intégration directe difficile.

**Faits clés :**

- **Gammes de produits :** Cam v3/v4 (intérieur/extérieur), Cam Pan v3 (PTZ), Cam OG (compacte), Doorbell, Floodlight
- **Prise en charge des protocoles :** Wyze Cloud (principal), RTSP (limité — nécessite un firmware spécial sur certains modèles)
- **Port RTSP par défaut :** 8554 (lors de l'utilisation du firmware RTSP)
- **Prise en charge ONVIF :** Non
- **Codecs vidéo :** H.264
- **Dépendance au cloud :** Élevée — la plupart des fonctionnalités nécessitent l'application Wyze et le cloud

!!! warning "Prise en charge RTSP très limitée"
    Les caméras Wyze ne prennent **pas** nativement en charge RTSP. Un firmware RTSP officiel n'a été publié que pour la **Wyze Cam v2** et la **Wyze Cam Pan** d'origine, et ces builds de firmware ne sont plus maintenus activement. Pour les modèles plus récents (v3, v4, OG, Pan v3), RTSP nécessite des solutions tierces comme un firmware personnalisé.

## Prise en charge RTSP par modèle

| Modèle | RTSP natif | Firmware RTSP | RTSP tiers | Notes |
|-------|------------|--------------|-----------------|-------|
| Wyze Cam v2 | Non | Oui (bêta) | Oui | Firmware RTSP officiel disponible |
| Wyze Cam Pan v1 | Non | Oui (bêta) | Oui | Firmware RTSP officiel disponible |
| Wyze Cam v3 | Non | Non | Oui (docker-wyze-bridge) | Contournement communautaire |
| Wyze Cam v4 | Non | Non | Oui (docker-wyze-bridge) | Contournement communautaire |
| Wyze Cam Pan v3 | Non | Non | Oui (docker-wyze-bridge) | Contournement communautaire |
| Wyze Cam OG | Non | Non | Oui (docker-wyze-bridge) | Contournement communautaire |
| Wyze Doorbell v2 | Non | Non | Limité | Peut fonctionner avec le pont |
| Wyze Cam Floodlight v2 | Non | Non | Oui (docker-wyze-bridge) | Contournement communautaire |

## Option 1 : Firmware RTSP officiel (Cam v2 / Pan v1 uniquement)

Wyze a publié un firmware RTSP bêta pour la Cam v2 et la Cam Pan d'origine. Une fois flashé :

### Format d'URL RTSP

```
rtsp://[IP]:8554/live
```

!!! note "Port non standard"
    Le firmware RTSP de Wyze utilise le port **8554**, pas le port 554 standard.

### Étapes de configuration

1. Téléchargez le firmware RTSP depuis le support Wyze (recherchez « Wyze RTSP firmware »)
2. Flashez le firmware sur la caméra via une carte microSD
3. Dans l'application Wyze, accédez aux paramètres de la caméra et activez RTSP
4. Notez l'URL RTSP affichée dans l'application (généralement `rtsp://CAMERA_IP:8554/live`)

### Connexion avec le SDK VisioForge

```csharp
// Wyze Cam v2 avec firmware RTSP
var uri = new Uri("rtsp://192.168.1.90:8554/live");
var username = ""; // pas d'authentification sur le firmware RTSP Wyze
var password = "";
```

Utilisez l'URL RTSP de votre caméra Wyze avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code).

## Option 2 : Pont Docker Wyze (tous les modèles)

Pour les Wyze Cam v3, v4, Pan v3, OG et autres modèles plus récents, le projet communautaire **docker-wyze-bridge** crée un proxy RTSP qui convertit les flux cloud Wyze en RTSP local :

### Comment ça fonctionne

1. Docker Wyze Bridge s'authentifie avec votre compte Wyze
2. Il se connecte à la caméra via l'API cloud Wyze
3. Il rediffuse la vidéo sous forme de flux RTSP local
4. Votre application se connecte au pont, pas directement à la caméra

### Format d'URL RTSP (via le pont)

```
rtsp://[BRIDGE_IP]:8554/[CAMERA_NAME]
```

Où `CAMERA_NAME` est le nom que vous avez attribué dans l'application Wyze (espaces remplacés par des tirets, en minuscules).

### Connexion via le pont avec le SDK VisioForge

```csharp
// Wyze Cam v3 via docker-wyze-bridge
var uri = new Uri("rtsp://192.168.1.50:8554/front-door");
var username = ""; // le pont gère l'authentification
var password = "";
```

!!! warning "Limitations du pont"
    Docker Wyze Bridge introduit une latence supplémentaire (généralement 3 à 10 secondes) car la vidéo passe par le cloud Wyze avant d'atteindre votre flux RTSP local. Il nécessite également les identifiants de votre compte Wyze et une connexion Internet active.

## Dépannage

### Pas d'option RTSP dans l'application Wyze

Le commutateur RTSP n'apparaît que sur la Wyze Cam v2 et la Pan v1 lorsqu'elles sont flashées avec le firmware RTSP. Il n'est pas disponible sur les modèles plus récents. Pour les v3/v4/OG/Pan v3, utilisez l'approche Docker Wyze Bridge.

### Le firmware RTSP ne se connecte pas

Après avoir flashé le firmware RTSP sur la Cam v2 :

1. Attendez 2 à 3 minutes que la caméra démarre complètement
2. Vérifiez que la caméra est sur le même réseau que votre application
3. Essayez d'abord `rtsp://CAMERA_IP:8554/live` dans VLC pour confirmer que le flux fonctionne
4. Le flux n'a pas d'authentification — laissez le nom d'utilisateur/mot de passe vides

### Latence élevée avec Docker Wyze Bridge

Le pont achemine la vidéo via les serveurs cloud de Wyze, ajoutant de la latence. Pour les exigences de faible latence, les caméras Wyze peuvent ne pas convenir. Envisagez des caméras avec une prise en charge RTSP native comme [Reolink](reolink.md), [Amcrest](amcrest.md) ou [TP-Link Tapo](tp-link.md).

### Qualité du flux

Les caméras Wyze produisent généralement des flux H.264 1080p. Le firmware RTSP ne prend pas en charge la modification de la résolution ou du codec. Ce que la caméra capture est ce que le flux RTSP fournit.

## FAQ

**Les caméras Wyze prennent-elles en charge RTSP nativement ?**

Non. Les caméras Wyze sont des appareils cloud-first. La Wyze Cam v2 et la Cam Pan d'origine disposent d'un firmware RTSP bêta officiel, mais il n'est plus maintenu activement. Les modèles plus récents (v3, v4, OG, Pan v3) n'ont pas de firmware RTSP et nécessitent des ponts tiers.

**Puis-je utiliser les caméras Wyze sans le cloud ?**

Très limité. Même avec le firmware RTSP, la configuration initiale nécessite l'application Wyze et un compte cloud. Le firmware RTSP pour Cam v2/Pan v1 désactive certaines fonctionnalités cloud. Pour les modèles plus récents, Docker Wyze Bridge passe toujours par le cloud.

**Quelles caméras dois-je utiliser à la place de Wyze pour RTSP ?**

Pour des caméras abordables avec prise en charge RTSP native, envisagez [Reolink](reolink.md) (grand public), [Amcrest](amcrest.md) (grand public/PME), [TP-Link Tapo](tp-link.md) (grand public) ou [EZVIZ](ezviz.md) (domotique). Tous fournissent un accès RTSP direct sans contournements.

**Les caméras Wyze prennent-elles en charge ONVIF ?**

Non. Les caméras Wyze ne prennent pas en charge ONVIF dans aucune version de firmware.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Reolink](reolink.md) — Alternative abordable avec RTSP natif
- [Guide de connexion Amcrest](amcrest.md) — Caméras grand public avec RTSP complet
- [Guide de connexion TP-Link](tp-link.md) — Caméras économiques avec prise en charge RTSP
- [Installation du SDK et exemples](index.md#get-started)
