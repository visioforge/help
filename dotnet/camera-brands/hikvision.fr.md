---
title: URL RTSP Hikvision en C# .NET — Guide caméra IP et NVR
description: Format d'URL RTSP Hikvision pour DS-2CD, DS-2DE et modèles NVR en C# .NET. Découverte ONVIF, flux multi-canaux et intégration SDK VisioForge.
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
primary_api_classes:
  - RTSPSourceProtocol

---

# Comment se connecter à une caméra IP Hikvision en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Hikvision** (Hangzhou Hikvision Digital Technology Co., Ltd.) est le plus grand fabricant mondial d'équipements de vidéosurveillance en parts de marché. Fondée en 2001 et dont le siège est à Hangzhou, en Chine, Hikvision produit des caméras IP, DVR, NVR et logiciels de gestion vidéo utilisés sur les marchés entreprise, gouvernemental et grand public.

**Faits clés :**

- **Gammes de produits :** DS-2CD (caméras fixes), DS-2DE (caméras PTZ), DS-76/77/96 (NVR), DS-7200/7300/7600 (DVR)
- **Prise en charge des protocoles :** ONVIF Profile S/G/T, RTSP, HTTP, ISAPI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / (défini lors de la configuration initiale ; firmware plus ancien : admin / 12345)
- **Prise en charge ONVIF :** Complète — recommandée pour la découverte et la configuration automatiques
- **Codecs vidéo :** H.264, H.265 (Smart Codec), MJPEG

## Modèles d'URL RTSP

Les caméras Hikvision utilisent une structure d'URL basée sur les canaux. Les numéros de canal encodent à la fois le canal de la caméra et le type de flux.

### Format d'URL

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/Streaming/Channels/[CHANNEL_ID]
```

**Encodage de l'ID de canal :**

- ID de canal = (numéro_caméra * 100) + numéro_flux
- Flux 1 = flux principal, Flux 2 = sous-flux, Flux 3 = troisième flux
- Exemple : Caméra 1, flux principal = **101** ; Caméra 1, sous-flux = **102**

### Caméras IP (canal unique)

| Série de modèles | URL RTSP | Flux | Audio |
|-------------|----------|--------|-------|
| DS-2CD2xx2 (fixe 2MP) | `rtsp://IP:554/Streaming/Channels/101` | Principal (1080p) | Oui |
| DS-2CD2xx2 (fixe 2MP) | `rtsp://IP:554/Streaming/Channels/102` | Sous-flux (CIF/D1) | Oui |
| DS-2CD2x32 (fixe 3MP) | `rtsp://IP:554/Streaming/Channels/101` | Principal (2048x1536) | Oui |
| DS-2CD2x32 (fixe 3MP) | `rtsp://IP:554/Streaming/Channels/102` | Sous-flux | Oui |
| DS-2CD21xx-I (série économique) | `rtsp://IP:554/Streaming/Channels/1` | Principal | Oui |
| DS-2CD21xx-I (série économique) | `rtsp://IP:554/Streaming/Channels/2` | Sous-flux | Oui |
| Série DS-2DE (PTZ) | `rtsp://IP:554/Streaming/Channels/101` | Principal | Oui |
| DS-2CD6362F (fisheye) | `rtsp://IP:554/Streaming/Channels/101` | Principal (3072x2048) | Oui |

### Canaux NVR / DVR

Pour les appareils NVR et DVR, modifiez le numéro de caméra dans l'ID de canal :

| Appareil | Canal | URL RTSP | Flux |
|--------|---------|----------|--------|
| NVR Caméra 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Principal |
| NVR Caméra 1 | 1 | `rtsp://IP:554/Streaming/Channels/102` | Sous-flux |
| NVR Caméra 2 | 2 | `rtsp://IP:554/Streaming/Channels/201` | Principal |
| NVR Caméra 2 | 2 | `rtsp://IP:554/Streaming/Channels/202` | Sous-flux |
| NVR Caméra 8 | 8 | `rtsp://IP:554/Streaming/Channels/801` | Principal |
| DVR Canal 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Principal |

### Formats d'URL alternatifs

Certains modèles Hikvision plus anciens et variantes OEM utilisent différents modèles d'URL :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Versions de firmware plus anciennes |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Firmware plus ancien, sous-flux |
| `rtsp://IP:554/PSIA/Streaming/channels/101` | Protocole PSIA (historique) |
| `rtsp://IP:554/video.h264` | Certains modèles OEM |
| `rtsp://IP:554/live.sdp` | Certains modèles plus anciens |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | OEM compatible Dahua |
| `rtsp://IP:554/mpeg4` | Flux MPEG4 (historique) |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Hikvision avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Hikvision DS-2CD2032-I, flux principal
var uri = new Uri("rtsp://192.168.1.64:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez plutôt `/Streaming/Channels/102`.

### Découverte ONVIF

Les caméras Hikvision ont une excellente prise en charge ONVIF. Utilisez ONVIF pour découvrir automatiquement les caméras sur votre réseau et récupérer leurs URI de flux sans construire manuellement les URL RTSP. Consultez le [guide d'intégration ONVIF](../mediablocks/Sources/index.md) pour des exemples de code de découverte.

## URL de capture instantanée et MJPEG

Les caméras Hikvision fournissent également des points de terminaison HTTP pour les captures et les flux MJPEG :

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/ISAPI/Streaming/channels/101/picture` | Nécessite une authentification |
| Flux MJPEG | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Sous-flux en MJPEG |
| Capture historique | `http://IP/Streaming/channels/1/picture` | Firmware plus ancien |
| Capture CGI | `http://IP/cgi-bin/snapshot.cgi` | Authentification basique |

## Dépannage

### « Double barre » dans le chemin d'URL

Les URL RTSP Hikvision utilisent un chemin commençant par `/Streaming/Channels/`. Certains outils ou codes génèrent `//Streaming/Channels/` (double barre). Les deux fonctionnent avec les caméras Hikvision, mais utilisez une barre unique pour rester correct.

### Connexion refusée sur le port 554

- Vérifiez que RTSP est activé dans l'interface web de la caméra : **Configuration > Network > Advanced Settings > RTSP**
- Vérifiez que le port RTSP n'a pas été modifié par rapport à la valeur par défaut (554)
- Assurez-vous qu'aucun pare-feu ne bloque le port entre votre application et la caméra

### Échecs d'authentification

- Les caméras Hikvision nécessitent une **authentification digest** par défaut. Le SDK VisioForge la gère automatiquement.
- Sur les firmwares plus récents, les identifiants par défaut `admin/12345` sont désactivés. Vous devez définir un mot de passe fort lors de la configuration initiale via l'outil Hikvision SADP ou l'interface web.
- Si vous vous connectez à un NVR, utilisez les identifiants du NVR, et non ceux des caméras individuelles.

### Le flux H.265 ne se lit pas

- Assurez-vous d'avoir le redistribuable du décodeur HEVC installé
- Vous pouvez aussi configurer la caméra pour utiliser l'encodage H.264 dans ses paramètres vidéo
- Utilisez `rtspSettings.UseGPUDecoder = true` pour un décodage H.265 accéléré par le matériel

### Latence élevée

- Utilisez le transport TCP : `rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP`
- Réduisez la latence du tampon : `rtspSettings.Latency = TimeSpan.FromMilliseconds(200)`
- Passez au sous-flux (canal 102) pour des besoins de bande passante plus faibles
- Désactivez l'audio si non nécessaire : `audioEnabled: false`

Combiné sur une instance unique de `RTSPSourceSettings` (construite via la fabrique async) :

```csharp
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:554/Streaming/Channels/102"),  // sous-flux
    login: "admin",
    password: "password",
    audioEnabled: false);

rtspSettings.UseGPUDecoder    = true;                        // H.265 accéléré par le matériel
rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;      // transport TCP
rtspSettings.Latency          = TimeSpan.FromMilliseconds(200);
```

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Hikvision ?**

L'URL RTSP standard pour les caméras Hikvision est `rtsp://admin:password@CAMERA_IP:554/Streaming/Channels/101` pour le flux principal. Remplacez `admin` et `password` par les identifiants de votre caméra, et `CAMERA_IP` par l'adresse IP de la caméra. Utilisez le canal `102` pour le sous-flux.

**Comment trouver l'adresse IP de ma caméra Hikvision ?**

Utilisez l'outil Hikvision SADP (Search Active Devices Protocol), un utilitaire gratuit qui découvre tous les appareils Hikvision sur votre réseau local. Vous pouvez aussi vérifier la liste des clients DHCP de votre routeur ou utiliser la découverte d'appareils ONVIF avec le SDK VisioForge.

**Puis-je me connecter à un NVR Hikvision et visualiser les canaux de caméras individuels ?**

Oui. Utilisez le même format d'URL RTSP mais modifiez le numéro de canal. La caméra 1 est le canal 101 (principal) ou 102 (sous-flux), la caméra 2 est le canal 201/202, et ainsi de suite. La formule est : ID de canal = (numéro_caméra x 100) + numéro_flux.

**Le SDK VisioForge prend-il en charge H.265+ (Smart Codec) de Hikvision ?**

Oui. Le SDK prend en charge le décodage H.265/HEVC standard. Le H.265+ de Hikvision est une optimisation de compression propriétaire qui produit des flux H.265 standard, donc il fonctionne avec n'importe quel décodeur compatible H.265.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion LTS](lts.md) — OEM Hikvision, utilise le même format d'URL
- [Guide de connexion EZVIZ](ezviz.md) — Marque grand public Hikvision
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
