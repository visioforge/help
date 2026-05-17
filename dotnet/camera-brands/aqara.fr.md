---
title: Connecter une caméra Aqara en C# .NET — Guide RTSP par jeton
description: Connectez les caméras Aqara en C# / .NET via RTSP. URL par jeton pour G2H, G3, E1 et G4 Doorbell. Configuration de l'application Aqara Home et exemples.
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
  - C#
primary_api_classes:
  - RTSPSourceSettings

---

# Comment se connecter à une caméra Aqara en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Aqara** est une marque chinoise de maison connectée (de Lumi United Technology) qui produit des appareils de maison connectée et des caméras basés sur Zigbee/Thread. Les caméras Aqara sont uniques sur le marché parce qu'elles font office de hubs de maison connectée (passerelle Zigbee) tout en fonctionnant comme caméras de sécurité. Aqara s'intègre principalement à Apple HomeKit et prend en charge RTSP pour le streaming local.

**Faits clés :**

- **Gammes de produits :** Camera Hub G2H/G3 (hub + caméra), E1 (caméra uniquement), G4 (sonnette)
- **Prise en charge des protocoles :** RTSP, Apple HomeKit Secure Video, Zigbee 3.0 (fonction hub)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** Aucun — l'URL RTSP inclut un jeton
- **Prise en charge ONVIF :** Non
- **Codecs vidéo :** H.264
- **Caractéristique unique :** Les caméras servent de hubs de maison connectée Zigbee

!!! info "RTSP doit être activé dans l'application Aqara Home"
    Les caméras Aqara prennent en charge RTSP, mais cela doit être activé via l'application **Aqara Home**. L'application génère une URL RTSP unique avec un jeton d'authentification. L'accès RTSP fonctionne indépendamment de HomeKit Secure Video.

## Activation de RTSP sur les caméras Aqara

1. Ouvrez l'application **Aqara Home** sur votre téléphone
2. Sélectionnez votre caméra
3. Allez dans **Camera Settings** (icône d'engrenage)
4. Trouvez **RTSP** ou **Network streaming**
5. Activez RTSP
6. L'application affichera l'URL RTSP complète avec le jeton d'authentification
7. Copiez cette URL pour l'utiliser dans votre application

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Aqara utilisent une URL RTSP basée sur un jeton :

```
rtsp://[IP]:554/live/ch00_1?token=[AUTH_TOKEN]
```

Le jeton d'authentification est généré par l'application Aqara Home et est unique à chaque caméra.

### Modèles de caméras

| Modèle | Type | Prise en charge RTSP | Résolution | Fonction hub |
|-------|------|-------------|-----------|-------------|
| Camera Hub G2H Pro | Hub + caméra | Oui | 1920x1080 | Zigbee 3.0 |
| Camera Hub G3 | Hub + caméra | Oui | 2304x1296 (2K) | Zigbee 3.0 |
| Camera E1 | Caméra uniquement | Oui | 1920x1080 | Non |
| G4 Video Doorbell | Sonnette | Limitée | 1600x1200 | Non |

### Variantes d'URL

| Modèle d'URL | Description |
|-------------|-------------|
| `rtsp://IP:554/live/ch00_1?token=TOKEN` | Flux principal (recommandé) |
| `rtsp://IP:554/live/ch00_0?token=TOKEN` | Sous-flux (résolution inférieure) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Aqara avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Aqara Camera Hub G3, flux principal (jeton issu de l'application Aqara Home)
var uri = new Uri("rtsp://192.168.1.90:554/live/ch00_1?token=YOUR_TOKEN_HERE");
var username = ""; // l'authentification est dans le jeton
var password = "";
```

!!! tip "Authentification par jeton"
    Les caméras Aqara n'utilisent pas l'authentification par nom d'utilisateur / mot de passe pour RTSP. À la place, le jeton d'authentification est intégré dans l'URL. Laissez les champs nom d'utilisateur et mot de passe vides dans `RTSPSourceSettings.CreateAsync()` et incluez le jeton dans l'URI.

## Dépannage

### L'URL RTSP ne fonctionne pas

1. Vérifiez que RTSP est activé dans l'application Aqara Home
2. Vérifiez que le jeton dans l'URL correspond à ce qu'affiche l'application
3. Assurez-vous que la caméra et votre application sont sur le même réseau
4. Essayez de régénérer l'URL RTSP dans l'application (Settings > RTSP > régénérer)

### Le jeton expire ou change

Le jeton RTSP peut changer après :

- Mises à jour du firmware de la caméra
- Réappairage de l'application Aqara Home
- Désactivation et réactivation de RTSP

Si votre flux cesse de fonctionner, vérifiez l'application Aqara Home pour une URL mise à jour.

### Pas de prise en charge ONVIF

Les caméras Aqara ne prennent pas en charge ONVIF. Vous ne pouvez pas utiliser la découverte ONVIF pour trouver des caméras Aqara. L'URL RTSP doit être obtenue depuis l'application Aqara Home.

### Limité à H.264

Les caméras Aqara n'encodent qu'en H.264. Il n'y a pas d'option H.265. Cela garantit une large compatibilité mais utilise plus de bande passante que H.265 à qualité équivalente.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Aqara ?**

Les caméras Aqara utilisent `rtsp://CAMERA_IP:554/live/ch00_1?token=AUTH_TOKEN`, où le jeton est généré par l'application Aqara Home. Il n'y a pas d'identifiants par défaut — l'authentification se fait via le jeton dans l'URL.

**Puis-je utiliser les caméras Aqara avec HomeKit et RTSP simultanément ?**

Oui. HomeKit Secure Video et RTSP peuvent fonctionner en même temps. L'activation de RTSP ne désactive pas la fonctionnalité HomeKit. Cependant, exécuter les deux flux peut légèrement réduire les performances de la caméra.

**Les caméras Aqara fonctionnent-elles comme hubs Zigbee pendant le streaming RTSP ?**

Oui. Les modèles Camera Hub G2H et G3 servent simultanément de passerelles Zigbee 3.0 et de caméras. Activer RTSP n'affecte pas la fonctionnalité de hub.

**Les caméras Aqara prennent-elles en charge ONVIF ?**

Non. Les caméras Aqara ne prennent en charge que RTSP (avec authentification par jeton) et HomeKit Secure Video. La découverte ONVIF n'est pas disponible.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion EZVIZ](ezviz.md) — Une autre marque de caméras de maison connectée
- [Guide de connexion TP-Link](tp-link.md) — Caméras grand public avec RTSP
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
