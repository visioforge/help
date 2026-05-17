---
title: URL RTSP des caméras IP ACTi et guide de connexion C# .NET
description: Modèles d'URL RTSP des séries ACTi A, B, D, E et des caméras historiques ACM/KCM/TCM pour C# .NET. Intégrez avec le VisioForge Video Capture SDK.
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
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP ACTi en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**ACTi Corporation** est un fabricant taïwanais de caméras de vidéosurveillance IP et de solutions de gestion vidéo. Basée à Taipei, à Taïwan, ACTi cible les marchés professionnels et entreprise avec une large gamme de caméras fixes, dôme, bullet et PTZ. ACTi est connu pour ses caméras actuelles des séries A/B/D/E et pour ses gammes historiques ACM, KCM et TCM.

**Faits clés :**

- **Gammes de produits :** Série A (boîtier), série B (bullet/zoom), série D (dôme), série E (dôme hémisphérique), KCM (dôme historique), ACM (boîtier/dôme historique), TCM (boîtier historique)
- **Prise en charge des protocoles :** RTSP, ONVIF (séries actuelles A/B/D/E), HTTP/CGI
- **Port RTSP par défaut :** 7070 (la plupart des modèles), 554 (certains modèles historiques)
- **Identifiants par défaut :** Admin / 123456 (modèles actuels), admin / admin (historiques)
- **Prise en charge ONVIF :** Oui (séries actuelles A/B/D/E)
- **Codecs vidéo :** H.264, H.265 (série E), MJPEG

!!! warning "Port non standard"
    Les caméras ACTi utilisent par défaut le **port 7070** pour RTSP, et non le port standard 554. C'est le problème de connexion le plus fréquent lors de l'intégration de caméras ACTi.

## Modèles d'URL RTSP

### Modèles actuels (séries A/B/D/E)

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Flux principal | `rtsp://IP:7070//stream1` | Flux principal (notez la double barre) |
| Flux racine | `rtsp://IP:7070/` | Solution de repli |
| H.264 direct | `rtsp://IP:7070/h264` | Sélection explicite du codec |
| Flux ONVIF | `rtsp://IP:7070//onvif-stream1` | Variante ONVIF |

!!! info "Double barre avant stream1"
    Les caméras ACTi utilisent une **double barre oblique** avant `stream1` dans leurs URL RTSP : `rtsp://IP:7070//stream1`. Cela est intentionnel et requis pour la plupart des modèles actuels.

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Type | Notes |
|-------------|----------|------|-------|
| D11, D21, D31, D32 | `rtsp://IP:7070//stream1` | Dôme | Actuel |
| D42, D51, D52, D55, D72 | `rtsp://IP:7070//stream1` | Dôme | Actuel |
| E12, E32, E33, E43, E46 | `rtsp://IP:7070//stream1` | Hémisphérique | Actuel, compatible H.265 |
| E51, E52, E63, E65, E73 | `rtsp://IP:7070//stream1` | Hémisphérique | Actuel, compatible H.265 |
| E82, E84, E96 | `rtsp://IP:7070//stream1` | Hémisphérique | Actuel, compatible H.265 |
| B53, B87, B95 | `rtsp://IP:7070//stream1` | Bullet/Zoom | Actuel |
| Série A (boîtier) | `rtsp://IP:7070//stream1` | Boîtier | Actuel |

### Modèles historiques

Les caméras ACTi historiques peuvent utiliser le port 554 ou 7070 selon le modèle et la version du firmware :

| Série de modèles | URL RTSP | Type | Notes |
|-------------|----------|------|-------|
| ACM-1011 | `rtsp://IP:554/` ou `rtsp://IP:7070/` | Boîtier | Historique |
| ACM-3401 | `rtsp://IP:554/` ou `rtsp://IP:7070/` | Dôme | Historique |
| ACM-5601 | `rtsp://IP:554/` ou `rtsp://IP:7070/` | Boîtier | Historique |
| ACM-7411 | `rtsp://IP:554/` ou `rtsp://IP:7070/` | Dôme | Historique |
| KCM-3311 | `rtsp://IP:7070/` | Dôme | Historique |
| KCM-5611 | `rtsp://IP:7070/` | Dôme | Historique |
| KCM-7211 | `rtsp://IP:7070/` | Dôme | Historique |
| TCM-1231 | `rtsp://IP:7070/` | Boîtier | Historique |
| TCM-3511 | `rtsp://IP:7070/` | Boîtier | Historique |
| TCM-5111 | `rtsp://IP:7070/` | Boîtier | Historique |
| TCM-5311 | `rtsp://IP:7070/` | Boîtier | Historique |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra ACTi avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra ACTi série D/E, flux principal -- notez le port 7070, et non 554 !
var uri = new Uri("rtsp://192.168.1.50:7070//stream1");
var username = "Admin";
var password = "123456";
```

Pour les modèles ACM historiques qui utilisent le port 554, modifiez le port en conséquence. Pour un flux racine plus simple, utilisez `rtsp://IP:7070/` comme URL.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture CGI | `http://IP/cgi-bin/encoder?USER=USERNAME&PWD=PASSWORD&SNAPSHOT` | Capture authentifiée |
| Streaming HTTP | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=USERNAME&PWD=PASSWORD` | Flux continu |
| Image JPEG | `http://IP/jpg/image.jpg` | JPEG direct |
| JPEG (alt) | `http://IP/now.jpg` | Chemin de capture alternatif |

## Dépannage

### Port 7070, et non 554

Le problème de connexion ACTi le plus courant est l'utilisation du port standard 554. Les caméras ACTi utilisent par défaut le **port 7070** pour RTSP. Si votre connexion expire ou est refusée, vérifiez que vous utilisez le bon port.

- Correct : `rtsp://IP:7070//stream1`
- Probablement incorrect : `rtsp://IP:554//stream1` (sauf si vous utilisez un modèle ACM historique)

### Double barre avant stream1

Les caméras ACTi de génération actuelle utilisent une **double barre oblique** avant `stream1` :

- Correct : `rtsp://IP:7070//stream1`
- Peut ne pas fonctionner : `rtsp://IP:7070/stream1`

### Les identifiants par défaut varient selon la génération

- **Modèles actuels (séries A/B/D/E) :** Nom d'utilisateur `Admin` (A majuscule), mot de passe `123456`
- **Modèles historiques (ACM/KCM/TCM) :** Nom d'utilisateur `admin` (minuscule), mot de passe `admin`

Modifiez toujours les identifiants par défaut avant de déployer des caméras sur un réseau de production.

### Modèles ACM historiques et port 554

Certaines caméras de la série ACM plus anciennes (ACM-1011, ACM-3401, ACM-5601, ACM-7411) peuvent utiliser le port 554 au lieu de 7070. Si le port 7070 échoue sur un modèle historique, essayez le port 554 avec l'URL racine `rtsp://IP:554/`.

### Disponibilité d'ONVIF

ONVIF n'est pris en charge que sur les caméras de génération actuelle (séries A, B, D et E). Les caméras historiques ACM, KCM et TCM ne prennent pas en charge ONVIF. Pour les modèles historiques, utilisez des URL RTSP ou HTTP directes.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras ACTi ?**

Pour les caméras ACTi actuelles (séries A/B/D/E), utilisez `rtsp://Admin:123456@CAMERA_IP:7070//stream1`. Notez le port non standard 7070 et la double barre avant `stream1`. Pour les modèles historiques, essayez `rtsp://admin:admin@CAMERA_IP:7070/` ou `rtsp://admin:admin@CAMERA_IP:554/`.

**Pourquoi ACTi utilise-t-il le port 7070 au lieu du 554 ?**

ACTi a choisi le port 7070 comme port RTSP par défaut. Cela peut être modifié dans l'interface web de la caméra, mais la valeur d'usine est 7070 pour la plupart des modèles. Certaines caméras de la série ACM historique utilisent par défaut le port 554.

**ACTi prend-il en charge H.265 ?**

Les caméras actuelles de la série E (modèles dôme hémisphérique) prennent en charge l'encodage H.265. Les autres séries actuelles (A, B, D) utilisent principalement H.264. Les modèles historiques (ACM, KCM, TCM) ne prennent en charge que H.264 et MJPEG.

**Quelle est la différence entre les séries de produits ACTi ?**

ACTi classe ses caméras par lettre : **A** = caméras boîtier, **B** = caméras bullet et zoom, **D** = caméras dôme, **E** = caméras dôme hémisphérique. Les gammes de produits historiques incluent ACM (boîtier/dôme), KCM (dôme) et TCM (boîtier).

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Vivotek](vivotek.md) — Caméras entreprise taïwanaises
- [Guide de connexion GeoVision](geovision.md) — Caméras professionnelles taïwanaises
- [Intégration de caméras IP ONVIF](../videocapture/video-sources/ip-cameras/onvif.md) — Configuration de périphérique ONVIF ACTi
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
