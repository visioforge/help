---
title: URL RTSP des caméras IP CP Plus en C# .NET — intégration SDK
description: Modèles d'URL RTSP CP Plus séries UNC, NC, RNP, Guard+ et Cosmic pour C# .NET. Intégrez avec le SDK VisioForge pour streaming et enregistrement de caméras IP.
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

# Comment se connecter à une caméra IP CP Plus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**CP Plus** (Aditya Infotech Ltd.) est la marque numéro 1 des caméras de sécurité en Inde et l'un des plus grands fabricants de vidéosurveillance au monde, dont le siège est à Delhi, en Inde. Les caméras CP Plus sont principalement des produits **OEM Dahua**, ce qui signifie que la plupart des modèles utilisent le firmware Dahua et partagent des modèles d'URL RTSP identiques. Certains modèles utilisent des chipsets alternatifs avec différents formats d'URL. CP Plus domine les marchés indien, du Moyen-Orient et d'Asie du Sud-Est avec une gamme complète de caméras IP, NVR et systèmes analogiques.

**Faits clés :**

- **Gammes de produits :** UNC (caméras IP), RNP (NVR), VAC (analogique), Guard+ (sans fil), série E (entrée de gamme), Cosmic (économique)
- **Base OEM :** Dahua (la plupart des modèles utilisent le firmware et les modèles d'URL Dahua)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (la plupart des modèles)
- **Codecs vidéo :** H.264, H.265 (modèles plus récents)
- **Marché dominant :** Inde (n°1), Moyen-Orient, Asie du Sud-Est

!!! info "CP Plus = OEM Dahua"
    Les caméras CP Plus sont principalement des produits OEM Dahua et utilisent les mêmes modèles d'URL RTSP. Consultez notre [guide de connexion Dahua](dahua.md) pour plus de détails. Certains modèles CP Plus utilisent plutôt le format d'URL `/VideoInput/`.

## Modèles d'URL RTSP

### Format d'URL standard (style Dahua)

La plupart des caméras CP Plus utilisent le modèle d'URL Dahua `cam/realmonitor` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=1
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Canal de la caméra (1 pour caméras autonomes) |
| `subtype` | 0 | Flux principal (résolution la plus élevée) |
| `subtype` | 1 | Sous-flux (résolution inférieure, moins de bande passante) |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| CP-UNC-DP10L2C | Dôme IP | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL style Dahua |
| CP-UNC-TY20FL2C | Turret IP | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL style Dahua |
| CP-NC9W-K | Caméra réseau | `rtsp://IP:554/VideoInput/1/mpeg4/1` | Format VideoInput |
| Série B | Basique | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | URL style Dahua |

### URL des canaux NVR

Pour les NVR CP Plus (CP-RNP-36D, CN-RNP-36D, etc.) :

| Canal | Flux principal | Sous-flux |
|---------|-------------|------------|
| Caméra 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Caméra 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Caméra N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Formats d'URL alternatifs

Certains modèles CP Plus utilisent différents modèles d'URL selon le firmware et le chipset :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Style Dahua standard (la plupart des modèles) |
| `rtsp://IP:554//cam/realmonitor` | Style Dahua alternatif (double barre) |
| `rtsp://IP:554/VideoInput/1/mpeg4/1` | Format VideoInput (CP-NC9W-K, CP-UNC-DP10L2C sur certains firmwares) |
| `rtsp://IP:554//cam/realmonitor?channel=1&subtype=00&authbasic=AUTH` | Avec authentification encodée en base64 |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra CP Plus avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// CP Plus CP-UNC-DP10L2C, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `subtype=1` au lieu de `subtype=0`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi?1` | Nécessite une authentification basique (CP-UNC-TY20FL2C) |
| Capture JPEG (historique) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | Authentification par URL pour les anciens modèles |
| Image JPEG | `http://IP/cgi-bin/jpg/image.cgi` | Point de terminaison JPEG alternatif |
| Flux MJPEG | `http://IP/api/mjpegvideo.cgi?InputNumber=1&StreamNumber=CHANNEL` | Flux MJPEG continu |
| Flux HTTP direct | `http://IP:8008/` | Certains modèles NVR (CN-RNP-36D, CP-RNP-36D) |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras CP Plus sont livrées avec les identifiants par défaut (`admin` / `admin`). Si vous avez modifié le mot de passe via l'interface web ou l'application mobile, assurez-vous que votre URL RTSP utilise les identifiants mis à jour.

1. Accédez à la caméra à `http://CAMERA_IP` dans un navigateur
2. Connectez-vous avec vos identifiants
3. Vérifiez que RTSP est activé sous **Setup > Network > Port**
4. Utilisez ces identifiants dans votre URL RTSP

### L'URL style Dahua ne fonctionne pas

Certains modèles CP Plus (en particulier la série CP-NC) utilisent le format d'URL `/VideoInput/` au lieu du modèle Dahua `cam/realmonitor`. Essayez :

```
rtsp://admin:password@IP:554/VideoInput/1/mpeg4/1
```

### Port 554 vs port personnalisé

Vérifiez le paramètre du port RTSP à :

- Interface web : **Setup > Network > Port > RTSP Port**
- La valeur par défaut est 554

### Confusion entre les types de flux

- `subtype=0` = Flux principal (résolution complète, bande passante plus élevée)
- `subtype=1` = Sous-flux (résolution réduite, bande passante plus faible)

### Flux HTTP direct sur le port 8008

Certains modèles NVR CP Plus (CP-RNP-36D, CN-RNP-36D) exposent un flux HTTP direct sur le port 8008. Essayez d'accéder à `http://CAMERA_IP:8008/` si le RTSP standard n'est pas disponible.

## FAQ

**Les caméras CP Plus sont-elles identiques à Dahua ?**

La plupart des caméras CP Plus sont fabriquées par Dahua et utilisent le firmware Dahua. Le format d'URL RTSP (`cam/realmonitor?channel=1&subtype=0`) est identique pour la majorité des modèles. Cependant, certains modèles CP Plus utilisent différents chipsets avec le format d'URL `/VideoInput/`. Tout code écrit pour les caméras Dahua fonctionne généralement avec CP Plus et vice versa.

**Quelle est l'URL RTSP par défaut pour les caméras CP Plus ?**

L'URL est `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=1` pour le sous-flux. Remplacez `subtype=1` par `subtype=0` pour le flux principal. Pour les configurations NVR, remplacez `channel=1` par le numéro de canal approprié.

**Les caméras CP Plus prennent-elles en charge ONVIF ?**

Oui. La plupart des caméras IP CP Plus actuelles prennent en charge ONVIF, qui fournit un moyen standardisé de découvrir et de se connecter aux caméras quel que soit le format d'URL spécifique.

**Et si ma caméra CP Plus utilise un format d'URL différent ?**

Certains modèles CP Plus (en particulier la série CP-NC) utilisent `rtsp://IP:554/VideoInput/1/mpeg4/1` au lieu de l'URL style Dahua. Si l'URL standard ne fonctionne pas, essayez le format VideoInput. Vous pouvez également utiliser la découverte ONVIF pour détecter automatiquement la bonne URL.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Dahua](dahua.md) — Même format d'URL pour la plupart des modèles
- [Guide de connexion Amcrest](amcrest.md) — Un autre OEM Dahua
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
