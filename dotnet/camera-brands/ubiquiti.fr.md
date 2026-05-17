---
title: Connexion à une caméra IP Ubiquiti (UniFi) en C# .NET
description: Modèles d'URL RTSP pour les caméras Ubiquiti UniFi Protect G3, G4, G5 et série AI en C# .NET. Activez RTSP dans UniFi et intégrez avec le SDK VisioForge.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - VideoCaptureCoreX
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

# Comment se connecter à une caméra IP Ubiquiti (UniFi) en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Ubiquiti Inc.** est une société technologique américaine basée à New York, connue pour ses équipements réseau sous la marque **UniFi**. La gamme de caméras d'Ubiquiti fait partie de l'écosystème **UniFi Protect**, qui comprend des caméras, des NVR (Network Video Recorders), des sonnettes et des capteurs. Les caméras UniFi Protect sont gérées via une console centrale (Dream Machine, Cloud Key ou NVR) et sont populaires dans les environnements semi-professionnels et PME.

**Faits clés :**

- **Gammes de produits :** UniFi Protect G3 (1080p), G4 (2K/4MP), G5 (2K/4MP mise à jour), série AI (avec IA embarquée), UVC (AirCam hérité)
- **Prise en charge des protocoles :** RTSP (doit être activé par caméra), ONVIF (limité), protocole UniFi Protect propriétaire
- **Port RTSP par défaut :** 7447 (UniFi Protect) ou 554 (AirCam hérité)
- **Identifiants par défaut :** Définis lors de la configuration UniFi Protect (RTSP utilise des identifiants séparés par caméra)
- **Prise en charge ONVIF :** Non pris en charge nativement ; RTSP est la méthode d'intégration tierce
- **Codecs vidéo :** H.264 (tous les modèles)

!!! warning "RTSP doit être activé"
    Les caméras UniFi Protect n'ont **pas** RTSP activé par défaut. Vous devez activer RTSP pour chaque caméra individuellement via l'interface web ou l'application UniFi Protect. Sans cela, la caméra ne répondra pas aux connexions RTSP.

### Activer RTSP sur les caméras UniFi Protect

1. Ouvrez l'interface web **UniFi Protect** (via votre Dream Machine, Cloud Key ou NVR)
2. Allez dans **Devices** et sélectionnez la caméra
3. Ouvrez l'onglet **Settings**
4. Faites défiler jusqu'à la section **Advanced**
5. Activez le commutateur **RTSP**
6. Notez l'URL RTSP affichée (inclut un jeton unique)

## Modèles d'URL RTSP

### Caméras UniFi Protect (actuelles)

Les caméras UniFi Protect exposent RTSP sur le **port 7447** avec sélection de qualité de flux :

| Flux | URL RTSP | Résolution | Notes |
|--------|----------|------------|-------|
| Haute qualité | `rtsp://IP:7447/STREAM_TOKEN` | Complète (jusqu'à 2688x1512) | Flux principal |
| Qualité moyenne | `rtsp://IP:7447/STREAM_TOKEN` | Réduite | Flux moyen |
| Faible qualité | `rtsp://IP:7447/STREAM_TOKEN` | Faible (640x360) | Optimisé pour la bande passante |

!!! info "Jetons de flux"
    UniFi Protect génère des URL RTSP uniques par caméra lorsque vous activez RTSP. L'URL contient un jeton unique. Vous pouvez trouver l'URL exacte dans l'interface UniFi Protect sous les paramètres Advanced de chaque caméra.

Le format d'URL RTSP est généralement :

```
rtsp://CAMERA_IP:7447/UNIQUE_TOKEN_STRING
```

Où le jeton est généré automatiquement et affiché dans l'interface utilisateur UniFi Protect.

### Modèles de caméras UniFi Protect

| Modèle | Résolution | Flux | Facteur de forme |
|-------|-----------|---------|-------------|
| G3 Instant | 1920x1080 | High/Low | Mini intérieur |
| G3 Flex | 1920x1080 | High/Medium/Low | Flex intérieur/extérieur |
| G3 Bullet | 1920x1080 | High/Medium/Low | Bullet extérieur |
| G3 Dome | 1920x1080 | High/Medium/Low | Dôme extérieur |
| G4 Instant | 2688x1512 | High/Medium/Low | Mini intérieur |
| G4 Bullet | 2688x1512 | High/Medium/Low | Bullet extérieur |
| G4 Dome | 2688x1512 | High/Medium/Low | Dôme extérieur |
| G4 Pro | 3840x2160 | High/Medium/Low | Pro extérieur |
| G4 PTZ | 3840x2160 | High/Medium/Low | PTZ |
| G5 Bullet | 2688x1512 | High/Medium/Low | Bullet extérieur |
| G5 Dome | 2688x1512 | High/Medium/Low | Dôme extérieur |
| G5 Turret Ultra | 3840x2160 | High/Medium/Low | Turret extérieur |
| AI 360 | 3840x2160 | High/Medium/Low | Fisheye |
| AI Bullet | 3840x2160 | High/Medium/Low | Bullet extérieur |
| AI Pro | 3840x2160 | High/Medium/Low | Pro extérieur |

### URL AirCam/AirVision héritées

Les anciennes caméras Ubiquiti (série AirCam, avant UniFi Protect) utilisaient le port 554 standard :

| Modèle | URL RTSP | Notes |
|-------|----------|-------|
| AirCam | `rtsp://IP:554/live/ch00_0` | Flux principal |
| AirCam Dome | `rtsp://IP:554/live/ch00_0` | Variante dôme |
| AirCam Mini | `rtsp://IP:554/live/ch00_0` | Variante mini |
| AirCam (canal) | `rtsp://IP:554/ch0N_0` | N = numéro de canal |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra UniFi Protect avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra UniFi Protect, authentification basée sur jeton (pas de nom d'utilisateur/mot de passe requis)
var uri = new Uri("rtsp://192.168.1.40:7447/YOUR_STREAM_TOKEN");
```

Les caméras UniFi Protect utilisent une authentification basée sur jeton — le jeton de flux unique est fourni dans l'interface utilisateur UniFi Protect lorsque vous activez RTSP. Aucun nom d'utilisateur ni mot de passe séparé n'est requis. Pour différents flux de qualité (high/medium/low), sélectionnez le flux correspondant dans l'interface Protect pour obtenir son jeton.

Pour les modèles AirCam hérités, utilisez le port 554 avec les identifiants `ubnt`/`ubnt` et le chemin `/live/ch00_0`.

## URL de capture instantanée

### AirCam hérité

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture | `http://IP/snapshot.cgi` | Capture basique |
| Capture (auth) | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | Avec identifiants |
| Capture (alt) | `http://IP:554/snapshot.cgi?user=USER&pwd=PASS&count=0` | Via port RTSP |

### UniFi Protect

Les caméras UniFi Protect n'exposent pas directement de points de terminaison de capture HTTP. Les captures sont accessibles via l'API UniFi Protect ou en capturant des images depuis le flux RTSP dans votre application.

## Dépannage

### « Connection refused » sur le port 554

Les caméras UniFi Protect utilisent le **port 7447** pour RTSP, pas le port 554 standard. Le port 554 ne s'applique qu'aux modèles AirCam hérités. Assurez-vous d'utiliser le bon port :

- **Caméras UniFi Protect :** Port 7447
- **AirCam hérité :** Port 554

### RTSP non activé

RTSP est désactivé par défaut sur les caméras UniFi Protect. Vous devez l'activer dans l'interface UniFi Protect :

1. UniFi Protect > Devices > Select Camera > Settings > Advanced > Enable RTSP

### Le jeton de flux a changé

Le jeton de flux RTSP peut changer si vous :
- Désactivez et réactivez RTSP sur la caméra
- Réinitialisez la caméra
- Mettez à jour le firmware

Vérifiez toujours l'URL RTSP actuelle dans l'interface UniFi Protect si votre connexion cesse de fonctionner.

### Latence élevée

Les caméras UniFi Protect peuvent présenter une latence de 2-5 secondes par défaut. Pour réduire la latence :

- Définissez `LowLatencyMode = true` sur le `RTSPSourceSettings` passé à VideoCaptureCoreX
- Sélectionnez le flux de faible qualité (résolution plus faible = moins de mise en tampon)
- Utilisez le transport TCP pour une livraison plus fiable

### Pas de prise en charge ONVIF

Les caméras UniFi Protect ne prennent pas en charge ONVIF. Utilisez RTSP pour l'intégration tierce. Si vous avez besoin de la découverte ONVIF, elle ne fonctionnera pas avec ces caméras.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras UniFi Protect ?**

Le format d'URL RTSP est `rtsp://CAMERA_IP:7447/UNIQUE_TOKEN`. RTSP doit être activé par caméra dans l'interface UniFi Protect, qui affichera l'URL unique. Il n'y a pas d'URL par défaut universelle — chaque caméra reçoit un jeton de flux unique.

**Puis-je utiliser les caméras UniFi sans UniFi Protect ?**

Les caméras UniFi actuelles nécessitent un contrôleur UniFi Protect (Dream Machine, Cloud Key ou NVR) pour la configuration et la gestion initiales. Une fois RTSP activé, vous pouvez diffuser vers un logiciel tiers. Les modèles AirCam hérités fonctionnent de manière autonome.

**Les caméras UniFi prennent-elles en charge H.265 ?**

Avec le firmware actuel, les caméras UniFi Protect diffusent en H.264 via RTSP. La prise en charge H.265 peut être disponible pour l'enregistrement interne mais n'est généralement pas exposée via RTSP.

**Quels sont les identifiants par défaut d'AirCam ?**

Les caméras AirCam héritées utilisent `ubnt` / `ubnt` comme identifiants par défaut. Les caméras UniFi Protect actuelles utilisent l'authentification RTSP basée sur un jeton.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Reolink](reolink.md) — Alternative semi-professionnelle avec RTSP
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
