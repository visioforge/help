---
title: Guide URL RTSP Zmodo — caméras IP et DVR en C# .NET SDK
description: Modèles d'URL RTSP pour les caméras Zmodo ZH Wi-Fi, ZP PoE et DVR/NVR en C# .NET. Streaming et enregistrement des caméras Zmodo avec le SDK VisioForge.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Zmodo en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Zmodo Technology** est une marque grand public de caméras de sécurité dont le siège est à Shenzhen, en Chine. Zmodo est connue pour ses caméras IP Wi-Fi et filaires abordables, ses systèmes DVR/NVR et ses produits de sécurité domotique. La marque cible le marché grand public économique et est largement disponible chez les revendeurs en ligne.

**Faits clés :**

- **Gammes de produits :** ZH-IXx (caméras Wi-Fi), ZP-IBH/IBI (caméras PoE), CM-I (caméras IP héritées), ZMD-ISV (systèmes DVR), Greet (sonnette intelligente)
- **Prise en charge des protocoles :** RTSP, HTTP/MJPEG (hérité), Zmodo Zink (propriétaire), ONVIF (limité, certains modèles ZP)
- **Port RTSP par défaut :** 10554 (caméras Wi-Fi), 554 (modèles standard/DVR)
- **Identifiants par défaut :** admin / admin ou admin / (mot de passe vide)
- **Prise en charge ONVIF :** Limitée (certains modèles PoE série ZP plus récents uniquement)
- **Codecs vidéo :** H.264, MPEG-4 (DVR hérités)

!!! warning "Caméras Zmodo Zink"
    Les caméras Zmodo qui utilisent le protocole propriétaire **Zink** ne prennent **pas** du tout en charge RTSP. Ces caméras ne peuvent être accessibles que via l'application Zmodo. Vérifiez les spécifications de votre caméra avant de tenter des connexions RTSP.

## Modèles d'URL RTSP

Les caméras Zmodo utilisent différents ports RTSP et formats d'URL selon la gamme de produits.

### Caméras Wi-Fi (série ZH) — Port 10554

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:10554//tcp/av0_0
```

!!! warning "Port non standard 10554"
    Les caméras Wi-Fi Zmodo (série ZH) utilisent le **port 10554**, pas le port 554 standard. C'est le problème de connexion le plus courant avec les caméras Zmodo.

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux principal | `rtsp://IP:10554//tcp/av0_0` | Résolution complète |
| Sous-flux | `rtsp://IP:10554//tcp/av0_1` | Résolution plus faible |

### URL spécifiques aux modèles (Wi-Fi / PoE)

| Modèle | URL RTSP | Type |
|-------|----------|------|
| ZH-IXA15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi intérieur |
| ZH-IXB15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi intérieur |
| ZH-IXC15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi intérieur |
| ZH-IXD15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi intérieur |
| ZH-IBH13-W | `rtsp://IP:10554//tcp/av0_0` | Bullet Wi-Fi |
| ZP-IBH13-P | `rtsp://IP:10554//tcp/av0_0` | Bullet PoE |
| ZP-IBI13-W | `rtsp://IP:10554//tcp/av0_0` | PoE intérieur |

### Caméras H.264 standard — Port 554

Certaines caméras Zmodo utilisent le port RTSP standard :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| H.264 direct | `rtsp://IP:554/h264` | Port standard |
| Flux par canal | `rtsp://IP:554/VideoInput/1/h264/1` | Basé sur le canal |
| Numéro de canal | `rtsp://IP:554/[CHANNEL]` | Canal direct |

### Série CM-I héritée

| Modèle | URL RTSP | URL alternative | Notes |
|-------|----------|---------|-------|
| CM-I11123BK | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | Solution de repli HTTP |
| CM-I12316GY | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | Solution de repli HTTP |

### Systèmes DVR/NVR

| Modèle | URL RTSP | Notes |
|-------|----------|-------|
| ZMD-ISV-BFS23NM | `rtsp://IP:554/VideoInput/1/h264/1` | Canal 1 |
| DVR (MPEG-4) | `rtsp://IP:554/mpeg4` | Format hérité |
| DVR (auth dans l'URL) | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | Authentification dans le chemin |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Zmodo avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Wi-Fi Zmodo série ZH, flux principal — notez le port 10554 !
var uri = new Uri("rtsp://192.168.1.60:10554//tcp/av0_0");
var username = "admin";
var password = "admin";
```

Pour accéder au sous-flux, utilisez `//tcp/av0_1` à la place de `//tcp/av0_0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | Modèles standard |
| Capture (caméra) | `http://IP/snapshot.cgi?camera=1` | Sélection de caméra |
| Capture DVR | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Systèmes DVR |
| Flux ASF | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=64&rate=0` | CM-I hérité |
| Flux MJPEG | `http://IP/videostream.cgi?rate=11` | Modèles hérités |

## Dépannage

### Vous devez utiliser le port 10554 pour les caméras Wi-Fi

Le problème de connexion Zmodo le plus courant est l'utilisation du port 554 alors que la caméra nécessite le **port 10554**. Toutes les caméras Wi-Fi série ZH et de nombreuses caméras PoE série ZP utilisent le port 10554. Si votre connexion expire sur le port 554, basculez sur 10554.

### Transport TCP dans le chemin d'URL

Le chemin `//tcp/av0_0` spécifie explicitement le transport TCP. Ceci est intégré au format d'URL Zmodo et n'est pas optionnel. Ne supprimez pas le préfixe `//tcp/` du chemin.

### L'application Zmodo est requise pour la configuration initiale

Certaines caméras Zmodo nécessitent l'application mobile Zmodo pour la configuration et l'activation Wi-Fi initiales. L'accès RTSP peut ne pas être disponible tant que la caméra n'a pas été configurée via l'application au moins une fois. Effectuez la configuration initiale avant de tenter les connexions RTSP.

### Les caméras au protocole Zink ne prennent pas en charge RTSP

Les caméras Zmodo qui utilisent le protocole propriétaire **Zink** sont conçues exclusivement pour l'écosystème Zmodo et ne prennent pas en charge RTSP, ONVIF ou tout autre protocole de streaming tiers. Vérifiez les spécifications de la caméra ou l'emballage pour la mention « Zink ». Si votre caméra utilise Zink, elle ne peut pas être accessible via RTSP.

### Les caméras CM-I héritées utilisent le streaming HTTP

Les caméras de la série CM-I plus anciennes peuvent avoir une prise en charge RTSP limitée ou peu fiable. Si RTSP échoue sur un modèle CM-I, utilisez les URL de streaming HTTP ASF ou MJPEG : `http://IP/videostream.asf?user=USER&pwd=PASS`.

### Format d'authentification DVR

Certains DVR Zmodo intègrent les identifiants dans le chemin RTSP plutôt que d'utiliser l'authentification RTSP standard : `rtsp://IP:554/0/USERNAME:PASSWORD/main`. Si l'authentification standard échoue, essayez ce format d'URL.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Wi-Fi Zmodo ?**

Pour les caméras Wi-Fi série ZH, l'URL est `rtsp://admin:admin@CAMERA_IP:10554//tcp/av0_0`. Notez le port non standard 10554 et le préfixe `//tcp/` dans le chemin.

**Pourquoi ma caméra Zmodo utilise-t-elle le port 10554 au lieu de 554 ?**

Zmodo a choisi le port 10554 pour sa gamme de caméras Wi-Fi. C'est un port fixe dans le firmware de la caméra. Certaines caméras Zmodo standard (non Wi-Fi) et les systèmes DVR utilisent le port 554 standard.

**Toutes les caméras Zmodo prennent-elles en charge RTSP ?**

Non. Les caméras Zmodo qui utilisent le protocole Zink propriétaire ne prennent pas en charge RTSP. Ces caméras sont limitées à l'application et au service cloud Zmodo. La plupart des caméras des séries ZH, ZP et CM-I prennent en charge RTSP.

**Zmodo prend-il en charge ONVIF ?**

La prise en charge ONVIF sur les caméras Zmodo est limitée. Certains modèles PoE de la série ZP plus récents incluent la prise en charge ONVIF, mais la plupart des modèles Wi-Fi grand public (série ZH) ne le font pas. Vérifiez les spécifications de votre modèle spécifique pour la compatibilité ONVIF.

**Quelle est la différence entre av0_0 et av0_1 ?**

Dans l'URL RTSP Zmodo, `av0_0` est le flux principal (de la plus haute qualité) et `av0_1` est le sous-flux (résolution plus faible). Utilisez `av0_1` lorsque vous avez besoin d'une consommation de bande passante plus faible pour la visualisation à distance.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Foscam](foscam.md) — Caméras IP grand public économiques
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
