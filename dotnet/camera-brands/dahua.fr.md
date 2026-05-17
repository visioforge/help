---
title: Modèles d'URL RTSP Dahua — caméras IP et guide C# .NET
description: Intégrez les caméras Dahua IPC-HDW, IPC-HFW, NVR et DVR dans des applications C# .NET. Format d'URL RTSP, auto-découverte ONVIF et code SDK VisioForge.
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
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Dahua en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Dahua Technology** (Zhejiang Dahua Technology Co., Ltd.) est le deuxième plus grand fabricant mondial de vidéosurveillance. Fondée en 2001 et dont le siège est à Hangzhou, en Chine, Dahua produit des caméras IP, NVR, DVR, systèmes de contrôle d'accès et interphones vidéo. Les caméras Dahua sont également largement vendues sous des marques OEM dont Amcrest, Lorex et autres.

**Faits clés :**

- **Gammes de produits :** IPC-HDW (dôme), IPC-HFW (bullet), IPC-HDBW (dôme anti-vandalisme), SD (PTZ), NVR4xxx/5xxx (NVR), XVR (DVR)
- **Prise en charge des protocoles :** ONVIF Profile S/G/T, RTSP, HTTP, Dahua propriétaire (DHIP)
- **Port RTSP par défaut :** 554 (certains modèles utilisent 1554)
- **Identifiants par défaut :** admin / admin (firmware plus ancien) ; admin / (défini lors de la configuration sur les firmwares plus récents)
- **Prise en charge ONVIF :** Complète
- **Codecs vidéo :** H.264, H.265, H.265+, MJPEG

## Modèles d'URL RTSP

Les caméras Dahua utilisent une structure d'URL `cam/realmonitor` avec des paramètres de canal et de sous-type.

### Format d'URL

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/cam/realmonitor?channel=[CH]&subtype=[ST]
```

**Paramètres :**

- `channel` = numéro de canal de la caméra (1 pour les caméras à canal unique, 1-N pour NVR/DVR)
- `subtype` = type de flux : 0 = flux principal, 1 = sous-flux, 2 = troisième flux

### Caméras IP (canal unique)

| Série de modèles | URL RTSP | Flux | Audio |
|-------------|----------|--------|-------|
| IPC-HDW (dôme) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Oui |
| IPC-HDW (dôme) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sous-flux | Oui |
| IPC-HFW (bullet) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Oui |
| IPC-HDBW (dôme anti-vandalisme) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Oui |
| SD (PTZ) | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal | Oui |
| DH-IPC-HF2100P | `rtsp://IP:1554/cam/realmonitor?channel=1&subtype=0` | Principal | Oui |

### Format d'URL simplifié

De nombreuses caméras Dahua acceptent également un format d'URL plus court :

| Modèle d'URL | Flux | Notes |
|-------------|--------|-------|
| `rtsp://IP:554/cam/realmonitor` | Principal (ch1) | Par défaut canal 1, flux principal |
| `rtsp://IP:554/` | Principal | URL minimale, certains modèles uniquement |
| `rtsp://IP:554/live` | Principal | Format historique |

### Canaux NVR / DVR

| Appareil | Canal | URL RTSP | Flux |
|--------|---------|----------|--------|
| NVR Caméra 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Principal |
| NVR Caméra 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Sous-flux |
| NVR Caméra 2 | 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | Principal |
| NVR Caméra 4 | 4 | `rtsp://IP:554/cam/realmonitor?channel=4&subtype=0` | Principal |
| DVR Canal 1 | 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=01` | Sous-flux |

### Amcrest / Lorex (OEM Dahua)

Les caméras Amcrest et Lorex utilisent le même format d'URL RTSP Dahua :

| Marque | URL RTSP | Notes |
|-------|----------|-------|
| Amcrest | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Identique à Dahua |
| Lorex | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Identique à Dahua |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Dahua avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Dahua série IPC-HDW, flux principal
var uri = new Uri("rtsp://192.168.1.108:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez plutôt `subtype=1`.

### Découverte ONVIF

Les caméras Dahua offrent une prise en charge ONVIF solide. Consultez le [guide d'intégration ONVIF](../mediablocks/Sources/index.md) pour des exemples de code de découverte.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Nécessite une authentification basique |
| Capture JPEG (historique) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Authentification par URL |
| Flux MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1` | MJPEG continu |
| MJPEG compatible Axis | `http://IP/axis-cgi/mjpg/video.cgi?camera=1` | API Axis émulée |
| Capture CGI | `http://IP/cgi-bin/video.jpg` | Capture simple |
| Image CGI | `http://IP/cgi-bin/jpg/image.cgi` | Capture alternative |

## Dépannage

### Port 554 vs 1554

Certains modèles Dahua (en particulier la série DH-IPC-HF) utilisent le port **1554** au lieu du standard 554. Si la connexion échoue sur le port 554, essayez 1554.

### Méthodes d'authentification

- Dahua prend en charge à la fois l'authentification RTSP **basic** et **digest**
- Le firmware plus récent utilise par défaut l'authentification digest
- Le SDK VisioForge gère automatiquement les deux méthodes
- Si vous utilisez les URL de capture HTTP, certaines nécessitent des identifiants intégrés dans l'URL (paramètres `loginuse`/`loginpas`) tandis que le firmware plus récent utilise l'authentification HTTP basic/digest standard

### Coupures de connexion

- Les caméras Dahua peuvent être sensibles à la congestion réseau. Utilisez le transport TCP pour la fiabilité.
- Réduisez la résolution du flux principal ou passez au sous-flux (`subtype=1`) pour diminuer la bande passante
- Vérifiez le paramètre **Max User Connections** de la caméra (Configuration > Network > Connection) — la valeur par défaut est généralement 10

### Caméras Amcrest/Lorex ne se connectant pas

Si vous disposez d'une caméra Amcrest ou Lorex (OEM Dahua), utilisez exactement les mêmes modèles d'URL RTSP listés ci-dessus. Les ports et chemins par défaut sont identiques à Dahua. La seule différence peut concerner les identifiants par défaut :

- **Amcrest par défaut :** admin / admin
- **Lorex par défaut :** admin / (défini lors de la configuration)

### Format de flux supplémentaire DVR

Lors de la connexion à des canaux DVR, notez que `subtype=00` et `subtype=0` sont équivalents pour le flux principal. Certains firmwares plus anciens nécessitent le format à deux chiffres (`01` au lieu de `1`).

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Dahua ?**

L'URL standard est `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` pour le flux principal. Utilisez `subtype=1` pour le sous-flux (résolution inférieure, moins de bande passante).

**Les caméras Amcrest utilisent-elles les mêmes URL RTSP que Dahua ?**

Oui. Les caméras Amcrest sont fabriquées par Dahua et utilisent des modèles d'URL RTSP, une authentification et des configurations de port identiques. Toute URL RTSP qui fonctionne pour une caméra Dahua fonctionnera pour le modèle Amcrest correspondant.

**Comment accéder à plusieurs caméras sur un NVR Dahua ?**

Modifiez le paramètre `channel` dans l'URL RTSP. Le canal 1 est la première caméra, le canal 2 la seconde, et ainsi de suite. Par exemple, `rtsp://IP:554/cam/realmonitor?channel=3&subtype=0` se connecte à la troisième caméra sur le flux principal du NVR.

**Pourquoi ma caméra Dahua utilise-t-elle le port 1554 au lieu de 554 ?**

Certains modèles Dahua plus anciens, en particulier la série DH-IPC-HF, utilisent par défaut le port RTSP 1554. Vous pouvez modifier cela dans l'interface web de la caméra sous Configuration > Network > Port. Les modèles plus récents utilisent par défaut le port 554.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Amcrest](amcrest.md) — OEM Dahua, format d'URL identique
- [Guide de connexion Lorex](lorex.md) — Utilise le format d'URL Dahua pour de nombreux modèles
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
