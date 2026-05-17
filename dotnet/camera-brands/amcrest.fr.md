---
title: Format d'URL RTSP Amcrest et configuration C# .NET
description: Modèles d'URL RTSP Amcrest IP2M, IP4M, IP5M, IP8M et NVR pour C# .NET. Diffusez et enregistrez avec le SDK VisioForge avec auto-découverte ONVIF.
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

# Comment se connecter à une caméra IP Amcrest en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Amcrest** (Amcrest Technologies LLC) est une marque américaine de caméras de sécurité grand public basée à Houston, au Texas. Les caméras Amcrest sont fabriquées par **Dahua Technology** et utilisent le firmware et les protocoles Dahua. Cela signifie que les caméras Amcrest partagent des modèles d'URL RTSP, des interfaces web et des points de terminaison API identiques aux caméras Dahua. Amcrest est devenu l'une des marques de caméras IP les plus vendues sur Amazon en Amérique du Nord.

**Faits clés :**

- **Gammes de produits :** IP2M (1080p), IP4M (4MP), IP5M (5MP), IP8M (4K/8MP), ASH (maison connectée), NV (NVR)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI, Amcrest Cloud, RTMP
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (doit être modifié à la première connexion avec un firmware plus récent)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264 (tous les modèles), H.265 (IP4M et plus récents)
- **Base OEM :** Dahua (format d'URL RTSP identique)

!!! info "Amcrest = Dahua"
    Les caméras Amcrest utilisent le firmware Dahua et exactement le même format d'URL RTSP que les caméras Dahua. Si vous êtes familier avec l'intégration Dahua, Amcrest fonctionne de manière identique. Consultez notre [guide de connexion Dahua](dahua.md) pour plus de détails.

## Modèles d'URL RTSP

### Format d'URL standard

Amcrest utilise le modèle d'URL Dahua `cam/realmonitor` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de la caméra (1 pour les caméras autonomes) |
| `subtype` | 0 | Flux principal (résolution la plus élevée) |
| `subtype` | 1 | Sous-flux (résolution inférieure, moins de bande passante) |
| `subtype` | 2 | Troisième flux (si pris en charge, optimisé mobile) |

### Modèles de caméras

| Modèle | Résolution | URL du flux principal | Audio |
|-------|-----------|----------------|-------|
| IP2M-841 (bullet 1080p) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| IP2M-844 (dôme 1080p) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| IP4M-1051 (bullet 4MP) | 2688x1520 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| IP5M-T1179E (turret 5MP) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| IP8M-2493E (bullet 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| IP8M-T2599E (turret 4K) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| ASH-41 (pan/tilt) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |
| ASH-42 (intérieur) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Oui |

### URL des canaux NVR

Pour les NVR Amcrest (NV4108E, NV4216E, NV5216E, etc.) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Caméra 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Caméra N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Formats d'URL alternatifs

Certains anciens modèles Amcrest ou versions de firmware prennent en charge ces URL alternatives :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Standard (recommandé) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Forcer l'unicast |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&proto=Onvif` | Compatible ONVIF |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Amcrest avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Amcrest IP4M-1051, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `subtype=1` au lieu de `subtype=0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Nécessite une authentification basique |
| Capture JPEG (historique) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Authentification par URL |
| Flux MJPEG | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | MJPEG continu |
| Image actuelle | `http://IP/onvif-http/snapshot?channel=1` | Capture HTTP ONVIF |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras Amcrest avec un firmware plus récent exigent que le mot de passe soit modifié par rapport à la valeur par défaut lors de la première connexion. Si vous n'avez pas encore configuré la caméra via l'interface web ou l'application Amcrest :

1. Accédez à la caméra à l'adresse `http://CAMERA_IP` dans un navigateur
2. Complétez l'assistant de configuration initial
3. Définissez un mot de passe fort
4. Utilisez ces identifiants dans votre URL RTSP

### Port 554 vs port personnalisé

Certaines versions du firmware Amcrest permettent de modifier le port RTSP. Vérifiez le paramètre de port à :

- Interface web : **Setup > Network > Port > RTSP Port**
- La valeur par défaut est 554

### Confusion entre les types de flux

- `subtype=0` = Flux principal (résolution complète, bande passante plus élevée)
- `subtype=1` = Sous-flux (résolution réduite, bande passante plus faible)
- `subtype=2` = Troisième flux (si disponible, généralement pour mobile)

### Caméras Amcrest SmartHome (ASH)

Les caméras de la série ASH (comme ASH-41, ASH-42) utilisent le même format d'URL RTSP, mais certains modèles nécessitent d'abord d'activer RTSP dans l'application Amcrest Smart Home.

## FAQ

**Les caméras Amcrest et Dahua sont-elles identiques ?**

Les caméras Amcrest sont fabriquées par Dahua et utilisent le firmware Dahua. Le format d'URL RTSP (`cam/realmonitor?channel=1&subtype=0`) est identique. Tout code écrit pour les caméras Dahua fonctionne avec Amcrest et vice versa. Les principales différences sont la marque, la garantie et le support nord-américain.

**Quelle est l'URL RTSP par défaut pour les caméras Amcrest ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` pour le flux principal. Remplacez `channel=1` par le canal approprié pour les configurations NVR et `subtype=0` par `subtype=1` pour le sous-flux.

**Les caméras Amcrest prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Amcrest actuelles prennent en charge ONVIF Profile S et Profile T. ONVIF est activé par défaut sur la plupart des modèles.

**Puis-je utiliser les caméras Amcrest sans le cloud Amcrest ?**

Oui. RTSP, ONVIF et l'interface web fonctionnent tous localement sans dépendance au cloud. Le service cloud Amcrest est optionnel et nécessaire uniquement pour la visualisation à distance via les applications Amcrest.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Même format d'URL (base OEM)
- [Guide de connexion Lorex](lorex.md) — Utilise aussi le format d'URL Dahua
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
