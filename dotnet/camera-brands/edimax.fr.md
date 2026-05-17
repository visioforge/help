---
title: URL RTSP des caméras IP Edimax et guide de connexion C# .NET
description: Connectez les caméras IP Edimax avec C# .NET via des modèles d'URL RTSP, conseils d'authentification et exemples pour les séries IC, IR, PT et VS.
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
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Edimax en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Edimax** (Edimax Technology Co.) est un fabricant taïwanais d'équipements réseau dont le siège est à Taipei, à Taïwan. Fondée en 1986, Edimax est connue principalement pour ses produits réseau tels que routeurs, switches et adaptateurs Wi-Fi, mais fabrique également une gamme de caméras IP grand public et pour PME-PMI sous la série IC. Au fil des années, les caméras Edimax ont évolué à travers plusieurs générations de formats d'URL.

**Faits clés :**

- **Gammes de produits :** IC (caméra IP), IR (infrarouge), PT (pan/tilt), VS (serveur vidéo)
- **Plusieurs générations de format d'URL :** les anciens modèles utilisent `/ipcam.sdp`, les plus récents utilisent `/stream1`
- **Port RTSP par défaut :** 554 (certains modèles utilisent 8000)
- **Identifiants par défaut :** admin / 1234
- **Prise en charge ONVIF :** Oui (modèles plus récents)
- **Codecs vidéo :** H.264, MJPEG
- **URL RTSP principale :** `rtsp://IP:554/ipcam_h264.sdp`

!!! note "Générations de format d'URL"
    Les caméras Edimax ont évolué à travers plusieurs générations de formats d'URL. Les anciennes séries IC-1500/IC-3000 utilisent `/ipcam.sdp` ou `/ipcam_h264.sdp`, tandis que les nouvelles séries IR/PT utilisent `/stream1`. Essayez les deux formats si l'un ne fonctionne pas.

!!! warning "Doubles styles d'authentification"
    Les caméras Edimax utilisent deux styles différents de paramètres d'authentification dans les URL HTTP : `account=USER&password=PASS` (firmware plus ancien) et `user=USER&pwd=PASS` (firmware plus récent). Vérifiez quel format prend en charge votre caméra.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Edimax utilisent principalement une URL RTSP basée sur SDP :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/ipcam_h264.sdp
```

| Modèle d'URL | Description |
|-------------|-------------|
| `/ipcam.sdp` | Flux basé sur SDP (ancienne série IC) |
| `/ipcam_h264.sdp` | Flux SDP H.264 (recommandé pour la plupart des modèles) |
| `/stream1` | Flux principal (nouvelles séries IR/PT) |
| `/stream2` | Flux secondaire (nouvelles séries IR/PT) |
| `/live1.sdp` | Flux SDP en direct (série PT) |

### Modèles de caméras

| Modèle | Type | URL du flux principal | Notes |
|-------|------|----------------|-------|
| IC-1500WG (VGA sans fil) | VGA | `rtsp://IP:554/ipcam.sdp` | Ancien format SDP |
| IC-3010 (HD) | HD | `rtsp://IP:554/ipcam.sdp` | Ancien format SDP |
| IC-3015WN (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, format SDP |
| IC-3030WN (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam.sdp` | Wi-Fi, format SDP |
| IC-3030IWN (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam.sdp` | Wi-Fi intérieur |
| IC-3100W (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam_h264.sdp` | Format SDP H.264 |
| IC-3110W (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam_h264.sdp` | Format SDP H.264 |
| IC-3116W (HD sans fil) | HD sans fil | `rtsp://IP:554/ipcam_h264.sdp` | Format SDP H.264 |
| IC-7000 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | Pan/tilt/zoom |
| IC-7010PTN (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam.sdp` | PTZ réseau |
| IC-7100 (HD PTZ) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ H.264 |
| IC-7110P (HD PTZ PoE) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ PoE |
| IC-7110W (HD PTZ sans fil) | HD PTZ | `rtsp://IP:554/ipcam_h264.sdp` | PTZ Wi-Fi |
| IC-9000 (extérieur) | Extérieur | `rtsp://IP:554/CHANNEL/USERNAME:PASSWORD/main` | Identifiants dans l'URL |
| IR-112E (infrarouge) | Infrarouge | `rtsp://IP:554//stream2` | Nouveau format de flux |
| IR-113E (infrarouge) | Infrarouge | `rtsp://IP:554//stream1` | Nouveau format de flux |
| PT-112E (pan/tilt) | Pan/Tilt | `rtsp://IP:554/live1.sdp` | Format Live SDP |
| PT-31E (pan/tilt) | Pan/Tilt | `rtsp://IP:8000//stream1` | Port 8000 |
| VS100 (serveur vidéo) | Serveur vidéo | `rtsp://IP:554//stream1` | Encodeur/serveur |

### Formats d'URL alternatifs

Certains modèles Edimax et versions de firmware prennent en charge ces URL RTSP supplémentaires :

| Modèle d'URL | Modèles pris en charge | Notes |
|-------------|-----------------|-------|
| `rtsp://IP:554/ipcam.sdp` | IC-1500, IC-3010, IC-3015WN, IC-3030WN, IC-7000, IC-7010PTN | Ancien format SDP |
| `rtsp://IP:554/ipcam_h264.sdp` | IC-3100W, IC-3110W, IC-3116W, IC-7100, IC-7110P, IC-7110W | SDP H.264 (recommandé) |
| `rtsp://IP:554//stream1` | IR-113E, VS100 | Nouveau format de flux (notez la double barre) |
| `rtsp://IP:554//stream2` | IR-112E | Sous-flux (double barre) |
| `rtsp://IP:554/stream1` | Modèles plus récents sélectionnés | Flux sans double barre |
| `rtsp://IP:554/live1.sdp` | PT-112E | Live SDP pour pan/tilt |
| `rtsp://IP:8000//stream1` | PT-31E | Port non standard 8000 |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Edimax avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Edimax IC-3116W, flux principal H.264
var uri = new Uri("rtsp://192.168.1.90:554/ipcam_h264.sdp");
var username = "admin";
var password = "1234";
```

Pour les nouveaux modèles des séries IR/PT, utilisez `/stream1` ou `//stream1` au lieu de `/ipcam_h264.sdp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/snapshot.jpg` | Capture basique, peut nécessiter une authentification |
| Capture (auth account) | `http://IP/snapshot.jpg?account=USER&password=PASS` | Style d'authentification ancien firmware |
| Capture (auth user) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Style d'authentification firmware plus récent |
| Image JPEG | `http://IP/jpg/image.jpg` | JPEG direct |
| JPEG canal | `http://IP/jpg/1/image.jpg` | JPEG spécifique au canal |
| Flux MJPEG | `http://IP/mjpg/video.mjpg` | Flux MJPEG continu |
| MJPEG canal | `http://IP/mjpg/1/video.mjpg` | MJPEG spécifique au canal |
| Capture CGI | `http://IP/snapshot.cgi` | Capture via CGI |
| CGI MJPEG | `http://IP/cgi/mjpg/mjpeg.cgi` | Flux MJPEG CGI |
| Stream CGI | `http://IP/cgi-bin/Stream?Video` | Flux vidéo alternatif |

## Dépannage

### Erreur « 401 Unauthorized »

Les caméras Edimax sont livrées avec les identifiants par défaut **admin / 1234**. Si l'authentification échoue :

1. Accédez à la caméra à `http://CAMERA_IP` dans un navigateur
2. Connectez-vous avec les identifiants par défaut ou ceux que vous avez configurés
3. Naviguez vers **Configuration > Security** pour vérifier ou réinitialiser les identifiants
4. Utilisez ces identifiants dans votre URL RTSP

### Le format d'URL ne fonctionne pas

Edimax a utilisé plusieurs formats d'URL RTSP différents selon les générations de modèles. Si votre URL ne se connecte pas :

1. Essayez d'abord `/ipcam_h264.sdp` (fonctionne avec la plupart des modèles de génération intermédiaire)
2. Essayez `/ipcam.sdp` pour les anciennes séries IC-1500 et IC-3000
3. Essayez `//stream1` (avec double barre) pour les nouvelles séries IR et PT
4. Essayez `/stream1` (barre unique) si la double barre échoue
5. Vérifiez si votre modèle utilise le port 8000 au lieu de 554 (par exemple PT-31E)

### URL à double barre

Certains modèles Edimax plus récents utilisent une double barre dans le chemin RTSP (par exemple, `rtsp://IP:554//stream1`). C'est intentionnel et ce n'est pas une faute de frappe. Si `/stream1` ne fonctionne pas, essayez `//stream1`.

### Port 554 vs port 8000

La plupart des caméras Edimax utilisent le port RTSP standard 554, mais certains modèles (comme PT-31E) utilisent le port 8000. Vérifiez l'interface web de votre caméra sous **Configuration > Network > RTSP** pour le paramètre de port correct.

### Style d'authentification des captures

Si les URL de capture renvoient des erreurs 401 même avec les bons identifiants, essayez de basculer entre les deux styles de paramètres d'authentification :

- Firmware plus ancien : `?account=USER&password=PASS`
- Firmware plus récent : `?user=USER&pwd=PASS`

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Edimax ?**

Pour la plupart des caméras Edimax de la série IC, l'URL est `rtsp://admin:1234@CAMERA_IP:554/ipcam_h264.sdp`. Pour les nouvelles séries IR et PT, utilisez `rtsp://admin:1234@CAMERA_IP:554//stream1`. Le format exact dépend du modèle et de la version du firmware.

**Les caméras Edimax prennent-elles en charge ONVIF ?**

Les modèles Edimax plus récents prennent en charge ONVIF. Les anciens modèles des séries IC-1500 et IC-3000 peuvent ne pas avoir de prise en charge ONVIF. Vérifiez les spécifications de votre caméra ou l'interface web pour les paramètres ONVIF.

**Pourquoi ma caméra Edimax utilise-t-elle une double barre dans l'URL ?**

Certains modèles Edimax plus récents (séries IR et PT) utilisent un format de chemin à double barre comme `//stream1`. C'est le format correct pour ces modèles et ce n'est pas une faute de frappe. Selon la version du firmware, `//stream1` et `/stream1` peuvent fonctionner tous les deux.

**Quelle est la différence entre ipcam.sdp et ipcam_h264.sdp ?**

`/ipcam.sdp` est le flux SDP générique utilisé par les modèles plus anciens et peut fournir soit MJPEG soit H.264 selon la configuration de la caméra. `/ipcam_h264.sdp` demande explicitement le flux encodé en H.264 et est recommandé pour une meilleure compression et qualité.

**Puis-je utiliser les deux styles d'authentification de captures ?**

Non. Chaque version du firmware de la caméra ne prend en charge qu'un seul style d'authentification pour les URL HTTP. Le firmware plus ancien utilise `account=USER&password=PASS` tandis que le firmware plus récent utilise `user=USER&pwd=PASS`. Essayez les deux pour déterminer celui que votre caméra attend.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Zavio](zavio.md) — Caméras PME taïwanaises
- [Guide de streaming vidéo RTSP](../general/network-streaming/rtsp.md) — Streaming réseau RTSP Edimax
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
