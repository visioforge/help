---
title: URL RTSP des caméras IP Basler en C# .NET — GenICam et Pylon
description: Connectez les caméras IP Basler BIP2 en C# .NET avec modèles d'URL RTSP et exemples. Notes sur les protocoles vision industrielle vs caméras IP de sécurité.
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
  - USB3 Vision / GigE
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Basler en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Basler** (Basler AG) est un fabricant allemand de caméras dont le siège est à Ahrensburg, en Allemagne, fondé en 1988. Basler est un leader mondial des caméras de vision industrielle et produit également des caméras IP de sécurité sous la gamme **BIP2**. Bien que les caméras de vision industrielle Basler utilisent des protocoles industriels spécialisés, la série BIP2 fournit une connectivité RTSP et ONVIF standard pour les applications de sécurité et de vidéosurveillance.

**Faits clés :**

- **Gammes de produits :** ace (vision industrielle), dart (compactes), boost (haute vitesse), BIP2 (sécurité IP)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI (série BIP2) ; GigE Vision, USB3 Vision (vision industrielle)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (série BIP2)
- **Codecs vidéo :** H.264, MPEG-4, MJPEG
- **Usage principal :** Vision industrielle, automatisation d'usine, contrôle qualité, vidéosurveillance IP

!!! info "Les caméras de vision industrielle utilisent des protocoles différents"
    Les caméras de vision industrielle Basler ace, dart et boost utilisent les protocoles GigE Vision ou USB3 Vision, et non RTSP. Elles nécessitent le SDK Pylon de Basler ou un framework compatible GenICam. Les URL RTSP de cette page s'appliquent à la gamme de caméras IP de sécurité BIP2 de Basler.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras IP Basler BIP2 utilisent des URL RTSP simples basées sur le chemin :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/h264
```

| Flux | Modèle d'URL | Description |
|--------|-------------|-------------|
| Flux principal H.264 | `rtsp://IP:554/h264` | Flux principal, meilleure qualité |
| Flux MPEG-4 | `rtsp://IP:554/mpeg4` | Flux historique encodé MPEG-4 |
| JPEG sur RTSP | `rtsp://IP:554/jpeg` | Images JPEG sur RTSP |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Codec |
|-------|------|----------------|-------|
| BIP2-1280c (720p) | Bullet IP | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c (1,3MP) | Bullet IP | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c (1080p) | Bullet IP | `rtsp://IP:554/h264` | H.264 |
| BIP2-1300c-dn | IP jour/nuit | `rtsp://IP:554/h264` | H.264 |
| BIP2-1920c-dn | IP jour/nuit | `rtsp://IP:554/h264` | H.264 |

### Formats d'URL alternatifs

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/h264` | Flux H.264 (recommandé) |
| `rtsp://IP:554/mpeg4` | Flux MPEG-4 (historique) |
| `rtsp://IP:554/jpeg` | JPEG sur RTSP |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Basler avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Basler BIP2, flux principal H.264
var uri = new Uri("rtsp://192.168.1.90:554/h264");
var username = "admin";
var password = "admin";
```

Pour les flux MPEG-4, remplacez `/h264` par `/mpeg4` dans l'URL.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Flux MJPEG | `http://IP/cgi-bin/mjpeg` | Flux MJPEG continu |
| Capture JPEG | `http://IP/cgi-bin/jpeg?stream=0` | Capture depuis le canal 0 |
| Capture JPEG (canal) | `http://IP/cgi-bin/jpeg?stream=CHANNEL` | Capture depuis un canal spécifique |

## Dépannage

### Caméra de vision industrielle ne se connectant pas en RTSP

Les caméras ace, dart et boost de Basler ne prennent pas en charge RTSP. Ces caméras utilisent les protocoles GigE Vision (Ethernet) ou USB3 Vision (USB) et nécessitent le SDK Pylon de Basler ou une bibliothèque compatible GenICam. Seule la série de caméras IP BIP2 prend en charge le streaming RTSP.

### Erreur « 401 Unauthorized »

Les caméras Basler BIP2 sont livrées avec les identifiants par défaut `admin` / `admin`. Si les identifiants ont été modifiés :

1. Accédez à l'interface web de la caméra à `http://CAMERA_IP`
2. Connectez-vous et vérifiez ou réinitialisez les identifiants
3. Utilisez les identifiants mis à jour dans votre URL RTSP

### Pas de sortie vidéo sur le flux MPEG-4

Certaines versions plus récentes du firmware Basler BIP2 peuvent utiliser H.264 par défaut uniquement. Si le flux MPEG-4 ne renvoie aucune donnée :

1. Ouvrez l'interface web de la caméra
2. Naviguez vers les paramètres de flux vidéo
3. Assurez-vous que l'encodage MPEG-4 est activé
4. Sinon, utilisez plutôt le chemin de flux `/h264`

### La découverte ONVIF ne trouve pas la caméra

ONVIF n'est pris en charge que sur les caméras de la série BIP2. Assurez-vous que :

- Le firmware de la caméra est à jour
- ONVIF est activé dans les paramètres réseau de la caméra
- La caméra et le client de découverte sont sur le même sous-réseau

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Basler ?**

Pour les caméras IP Basler BIP2, l'URL par défaut est `rtsp://admin:admin@CAMERA_IP:554/h264` pour le flux principal H.264. Remplacez les identifiants s'ils ont été modifiés par rapport aux valeurs par défaut.

**Puis-je utiliser le SDK VisioForge avec les caméras de vision industrielle Basler ?**

La connexion basée sur RTSP décrite dans cette page s'applique uniquement aux caméras IP de sécurité Basler BIP2. Les caméras de vision industrielle de Basler (ace, dart, boost) utilisent les protocoles GigE Vision ou USB3 Vision et nécessitent le SDK Pylon de Basler ou un framework compatible GenICam pour une intégration directe.

**Les caméras Basler prennent-elles en charge ONVIF ?**

Oui, mais seule la série de caméras IP de sécurité BIP2 prend en charge ONVIF. Les caméras de vision industrielle de Basler utilisent plutôt des protocoles industriels (GigE Vision, USB3 Vision).

**Quels codecs prennent en charge les caméras IP Basler ?**

Les caméras Basler BIP2 prennent en charge H.264, MPEG-4 et MJPEG. H.264 est recommandé pour le meilleur équilibre entre qualité et efficacité de bande passante.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Mobotix](mobotix.md) — Caméras industrielles allemandes
- [Guide de connexion FLIR](flir.md) — Imagerie industrielle et thermique
- [Création d'applications de caméra avec Media Blocks](../mediablocks/GettingStarted/camera.md)
- [Installation du SDK et exemples](index.md#get-started)
