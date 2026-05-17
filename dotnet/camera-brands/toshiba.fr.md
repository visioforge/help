---
title: URL RTSP Toshiba et streaming en C# .NET — caméras IP
description: Modèles d'URL RTSP pour les caméras Toshiba IK-WB, IK-WD, IK-WR et IK-WP en C# .NET. Streaming et enregistrement avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP Toshiba en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Toshiba** (Toshiba Corporation) est un conglomérat multinational japonais dont le siège est à Tokyo, au Japon. La division sécurité de Toshiba produisait la **série IK-W** de caméras IP, couvrant les formats box, dôme, bullet et PTZ. Toshiba s'est depuis retiré du marché des caméras de sécurité autonomes et a vendu son activité de vidéosurveillance. Malgré l'arrêt de la production, de nombreuses caméras de la série IK-W restent déployées dans des installations commerciales et industrielles à travers le monde.

**Faits clés :**

- **Gammes de produits :** IK-WB (caméras box), IK-WD (caméras dôme), IK-WR (caméras bullet/rugged), IK-WP (caméras PTZ)
- **Prise en charge des protocoles :** RTSP, HTTP/CGI, ONVIF (limité, modèles plus récents uniquement)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 1234
- **Prise en charge ONVIF :** Limitée (modèles IK-W14/16/30/70/80 plus récents uniquement)
- **Codecs vidéo :** H.264 (séries IK-W14/16/30/70/80), MJPEG (anciens modèles)

!!! warning "Gamme de produits discontinuée"
    Toshiba a quitté le marché des caméras IP et a vendu son activité de vidéosurveillance. Aucune nouvelle mise à jour de firmware ni support officiel ne sont disponibles. De nombreux premiers modèles IK-WB (01A, 02A, 11A) ne prennent en charge que la capture HTTP et ne fournissent pas de streaming RTSP.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras Toshiba série IK-W utilisent le modèle d'URL `live.sdp` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live.sdp
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| `live.sdp` | Flux principal | Flux H.264 principal (résolution la plus élevée) |
| `live2.sdp` | Sous-flux | Flux secondaire (résolution plus faible) |
| `live3.sdp` | Troisième flux | Troisième flux (optimisé mobile, certains modèles) |

### Modèles de caméras - Flux RTSP

| Modèle | Type | URL du flux principal | URL du sous-flux | Notes |
|-------|------|----------------|----------------|-------|
| IK-WB16A | Box | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WB80A | Box | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD01A | Dôme | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD12A | Dôme | `rtsp://IP:554//live.sdp` | -- | Chemin double barre |
| IK-WD14A | Dôme | `rtsp://IP:554/live.sdp` | `rtsp://IP:554/live2.sdp` | Prend également en charge `live3.sdp` |
| IK-WR04A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR12A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR14A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WP41A | PTZ | `rtsp://IP:554/live.sdp` | -- | H.264 |

### Modèles par série

#### Série IK-WB (caméras box)

| Modèle | Streaming | Protocole |
|-------|-----------|----------|
| IK-WB01A | Capture HTTP uniquement | HTTP |
| IK-WB02A | Capture HTTP uniquement | HTTP |
| IK-WB11A | Capture HTTP uniquement | HTTP |
| IK-WB15A | Capture HTTP + CGI | HTTP |
| IK-WB16A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB16A-W | RTSP `live.sdp`, `live3.sdp` | RTSP + HTTP |
| IK-WB21A | CGI HTTP uniquement | HTTP |
| IK-WB30A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB70A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |
| IK-WB80A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |

#### Série IK-WD (caméras dôme)

| Modèle | Streaming | Protocole |
|-------|-----------|----------|
| IK-WD01A | RTSP `live.sdp` | RTSP |
| IK-WD12A | RTSP `//live.sdp` | RTSP (double barre) |
| IK-WD14A | RTSP `live.sdp`, `live2.sdp`, `live3.sdp` | RTSP (multi-flux) |

#### Série IK-WR (caméras bullet/rugged)

| Modèle | Streaming | Protocole |
|-------|-----------|----------|
| IK-WR01A | Capture HTTP uniquement | HTTP |
| IK-WR02A | Capture HTTP uniquement | HTTP |
| IK-WR04A | RTSP `live.sdp` | RTSP |
| IK-WR12A | RTSP `live.sdp` | RTSP + MJPEG |
| IK-WR14A | RTSP `live.sdp` | RTSP + HTTP |

#### Série IK-WP (caméras PTZ)

| Modèle | Streaming | Protocole |
|-------|-----------|----------|
| IK-WP41A | RTSP `live.sdp` | RTSP |

### Formats d'URL alternatifs

Certains modèles Toshiba utilisent une double barre dans le chemin RTSP :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/live.sdp` | Standard (recommandé) |
| `rtsp://IP:554//live.sdp` | Variante double barre (IK-WD12A, certaines unités IK-WD14A) |
| `rtsp://IP:554/live2.sdp` | Sous-flux (IK-WD14A) |
| `rtsp://IP:554/live3.sdp` | Troisième flux (IK-WB16A-W, IK-WD14A) |

!!! tip "Chemin à double barre"
    Si `rtsp://IP:554/live.sdp` ne fonctionne pas sur votre caméra Toshiba, essayez la variante à double barre `rtsp://IP:554//live.sdp`. Certains modèles IK-WD nécessitent ce format.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Toshiba avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Toshiba IK-WD14A, flux principal
var uri = new Uri("rtsp://192.168.1.90:554/live.sdp");
var username = "admin";
var password = "1234";
```

Pour accéder au sous-flux sur les modèles compatibles, utilisez `live2.sdp` à la place de `live.sdp`.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Modèles compatibles | Notes |
|------|-------------|-------------------|-------|
| Capture JPEG | `http://IP/__live.jpg?&&&` | IK-WB01A, WB11A, WB15A, WB16A-W, WB21A | Notez le préfixe avec tiret bas |
| Capture CGI | `http://IP/GetData.cgi` | IK-WB01A, WB11A, WB15A, WB21A, WR01A | Capture basique |
| Capture configurable | `http://IP/GetData.cgi?CH=CHANNEL&Codec=jpeg&Size=WIDTHxHEIGHT` | Série IK-WB | Définir résolution et canal |
| Capture par résolution | `http://IP/cgi-bin/viewer/video.jpg?resolution=WIDTHxHEIGHT` | IK-WB15A, WB16A, WB30A, WB70A, WR12A, WR14A | Spécifier la résolution de sortie |
| Capture simple | `http://IP/Jpeg/CamImg.jpg` | IK-WB02A, WR01A | Capture JPEG basique |
| Flux MJPEG | `http://IP/video.mjpg` | IK-WB70A, WB80A, WR12A | Flux MJPEG continu |

!!! note "Modèles HTTP uniquement"
    Les premiers modèles Toshiba (IK-WB01A, WB02A, WB11A, WR01A, WR02A) ne prennent pas en charge RTSP. Pour ces caméras, utilisez les URL de capture HTTP ou les flux MJPEG. Vous pouvez les capturer via les modes source HTTP ou MJPEG du SDK VisioForge.

## Dépannage

### La caméra est HTTP uniquement (pas de RTSP)

De nombreux premiers modèles IK-WB (01A, 02A, 11A) et IK-WR (01A, 02A) ne prennent pas du tout en charge le streaming RTSP. Ces caméras ne fournissent que des captures HTTP et des points de terminaison CGI. Si votre caméra ne répond pas sur le port 554, vérifiez s'il s'agit d'un modèle HTTP uniquement dans les tableaux ci-dessus.

### Préfixe avec tiret bas dans l'URL de capture

L'URL de capture `__live.jpg` utilise un **double préfixe avec tiret bas**, ce qui est inhabituel. Assurez-vous d'inclure les deux tirets bas :

```
http://192.168.1.90/__live.jpg?&&&
```

Les caractères `&&&` finaux sont également requis sur certaines versions de firmware.

### Double barre dans le chemin RTSP

Certaines caméras de la série IK-WD (WD12A, certaines unités WD14A) nécessitent une double barre dans le chemin RTSP :

```
rtsp://admin:1234@192.168.1.90:554//live.sdp
```

Si l'URL standard à simple barre ne se connecte pas, essayez cette variante.

### Aucune mise à jour de firmware disponible

Toshiba a quitté le marché des caméras de sécurité. Aucun nouveau firmware, correctif ou canal de support officiel n'est disponible. Si vous rencontrez des bugs ou des vulnérabilités de sécurité, envisagez de remplacer la caméra par un modèle actuellement pris en charge.

### Les identifiants par défaut ne fonctionnent pas

Les identifiants d'usine par défaut sont **admin / 1234**. Si ceux-ci ne fonctionnent pas, le mot de passe a peut-être été modifié par un administrateur précédent. Une réinitialisation matérielle d'usine (généralement un bouton de réinitialisation accessible par une épingle) restaurera les valeurs par défaut sur la plupart des modèles.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras IP Toshiba ?**

L'URL RTSP principale est `rtsp://admin:1234@CAMERA_IP:554/live.sdp` pour les modèles prenant en charge le streaming RTSP. Utilisez `live2.sdp` pour le sous-flux sur les modèles comme l'IK-WD14A. Notez que les anciens modèles IK-WB01A/02A/11A et IK-WR01A/02A ne prennent pas du tout en charge RTSP.

**Les caméras IP Toshiba sont-elles toujours prises en charge ?**

Non. Toshiba a vendu son activité de vidéosurveillance et quitté le marché des caméras IP. Aucune mise à jour de firmware, aucun nouveau modèle ni support technique officiel ne sont disponibles. Les caméras existantes continuent de fonctionner mais ne recevront pas de correctifs de sécurité ni de mises à jour de fonctionnalités.

**Les caméras Toshiba prennent-elles en charge ONVIF ?**

Seuls les modèles plus récents de la gamme IK-W14/16/30/70/80 ont une prise en charge ONVIF limitée. Les anciens modèles (IK-WB01A à WB11A, IK-WR01A/02A) ne prennent pas en charge ONVIF. Pour la découverte et la configuration ONVIF, utilisez uniquement les modèles compatibles.

**Pourquoi ma caméra Toshiba ne fournit-elle que des captures, pas des flux vidéo ?**

Les premiers modèles Toshiba IK-W ont été conçus comme des caméras de capture réseau et n'incluent pas de serveur RTSP. Ces modèles (IK-WB01A, WB02A, WB11A, WR01A, WR02A) ne prennent en charge que les captures JPEG basées sur HTTP et les points de terminaison CGI. Pour obtenir de la vidéo en continu, vous avez besoin d'un modèle plus récent de la série IK-W14/16/30/70/80.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Sony](sony.md) — Caméras d'entreprise japonaises
- [Guide de connexion JVC](jvc.md) — Marque japonaise de vidéosurveillance héritée
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
