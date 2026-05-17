---
title: Connexion RTSP des caméras IP FLIR en C# .NET — thermique
description: Modèles d'URL RTSP des caméras thermiques FLIR Quasar, Saros et Elara pour C# .NET. Intégration Teledyne FLIR avec le VisioForge Video Capture SDK.
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
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP FLIR en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**FLIR Systems** (désormais **Teledyne FLIR** après l'acquisition de 2021 par Teledyne Technologies) est un fabricant de premier plan de caméras d'imagerie thermique et de caméras de sécurité à lumière visible. Dont le siège est à Wilsonville, en Oregon, États-Unis, FLIR dessert les marchés entreprise, infrastructures critiques et gouvernementaux. FLIR est surtout connu pour l'imagerie thermique mais produit également une gamme complète de caméras IP à lumière visible pour la vidéosurveillance professionnelle. FLIR a précédemment acquis **Lorex** et **DVTEL** (désormais FLIR Latitude VMS).

**Faits clés :**

- **Gammes de produits :** Quasar (multi-capteurs/mini-dôme haut de gamme), Saros (détection de périmètre), Elara (double capteur thermique+visible), CM (mini dôme compact), CF (fixe compact), PT/PTZ (pan-tilt-zoom), FC (thermique uniquement), FLIR FX (grand public, abandonné)
- **Prise en charge des protocoles :** RTSP, ONVIF (séries Quasar, Saros, Elara), HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (la plupart des modèles), admin / fliradmin (certains modèles Quasar)
- **Prise en charge ONVIF :** Oui (séries Quasar, Saros, Elara)
- **Codecs vidéo :** H.264, H.265 (série Quasar), MJPEG
- **Spécialisation thermique :** Les caméras thermiques FLIR fournissent des données radiométriques en plus de la vidéo en lumière visible, avec des flux RTSP distincts pour chaque capteur

!!! warning "Les caméras thermiques ont des flux distincts"
    Les caméras FLIR à double capteur (Elara, série PT) fournissent des flux RTSP distincts pour les canaux visible et thermique. Généralement `ch0` est le canal visible et `ch1` est le canal thermique.

## Modèles d'URL RTSP

### Modèles actuels (Quasar, Saros, Elara)

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Canal visible | `rtsp://IP:554/ch0` | Flux visible principal |
| Canal thermique | `rtsp://IP:554/ch1` | Flux thermique (modèles à double capteur) |
| Visible (alt) | `rtsp://IP:554/vis` | Visible sur série PT |
| Thermique grand champ | `rtsp://IP:554/wfov` | Série PT, champ de vision large |
| Flux avec auth | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | Avec identifiants intégrés |

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Type | Notes |
|-------------|----------|------|-------|
| Quasar CM-3308 | `rtsp://IP:554/ch0` | Mini dôme | Multi-capteurs compact |
| Quasar CM-6208 | `rtsp://IP:554/ch0` | Mini dôme | Multi-capteurs compact |
| Série D (dôme fixe) | `rtsp://IP:554/ch0` | Dôme fixe | Flux visible |
| Série F (fixe) | `rtsp://IP:554/ch0` | Fixe | Flux visible |
| Série PT (PTZ-35x140) | `rtsp://IP:554/vis` | PTZ | Canal visible |
| Série PT (PTZ-35x140) | `rtsp://IP:554/wfov` | PTZ | Thermique grand champ |
| Elara (visible) | `rtsp://IP:554/ch0` | Thermique+visible | Canal visible |
| Elara (thermique) | `rtsp://IP:554/ch1` | Thermique+visible | Canal thermique |
| Série FC (thermique) | `rtsp://IP:554/ch0` | Thermique uniquement | Flux thermique |

### Caméras thermiques à double capteur

Les caméras FLIR à double capteur fournissent à la fois vidéo visible et thermique sur des canaux distincts :

| Flux | URL RTSP | Notes |
|--------|----------|-------|
| Elara visible | `rtsp://IP:554/ch0` | Capteur lumière visible |
| Elara thermique | `rtsp://IP:554/ch1` | Capteur thermique |
| PT visible | `rtsp://IP:554/vis` | Capteur lumière visible |
| PT thermique grand champ | `rtsp://IP:554/wfov` | Capteur thermique grand champ |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra FLIR avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// FLIR Quasar mini-dôme, flux visible
var uri = new Uri("rtsp://192.168.1.70:554/ch0");
var username = "admin";
var password = "admin";
```

Pour les caméras à double capteur, utilisez `ch1` pour accéder au flux thermique. Pour les caméras de la série PT, utilisez `/vis` pour le canal visible ou `/wfov` pour le canal thermique grand champ.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/jpg/image.jpg` | Certains modèles |
| Capture (alt) | `http://IP/snapshot.jpg` | Chemin alternatif |

## Dépannage

### Canaux thermique vs visible

Les caméras FLIR à double capteur (Elara, série PT) exposent des flux RTSP distincts pour chaque capteur :

- `ch0` = canal lumière visible (la plupart des modèles)
- `ch1` = canal thermique (la plupart des modèles)
- `/vis` = canal visible (série PT)
- `/wfov` = thermique grand champ (série PT)

Si vous vous connectez au mauvais canal, vous pouvez recevoir une imagerie thermique alors que vous attendiez du visible ou vice versa. Vérifiez la documentation de votre caméra pour les attributions de canaux.

### Les identifiants par défaut diffèrent selon le modèle

- **La plupart des caméras FLIR :** admin / admin
- **Certains modèles Quasar :** admin / fliradmin
- **Firmware Teledyne FLIR actuel :** Le mot de passe peut nécessiter d'être défini lors de la configuration initiale

Modifiez toujours les identifiants par défaut avant de déployer des caméras sur un réseau de production.

### Renommage Teledyne FLIR

Teledyne Technologies a racheté FLIR Systems en 2021. Les versions actuelles du firmware peuvent afficher la marque Teledyne FLIR, et les caméras plus récentes peuvent être livrées avec des interfaces web et des outils de configuration mis à jour. Les modèles d'URL RTSP restent cohérents avec les caméras FLIR historiques.

### Caméras grand public FLIR FX

La gamme abandonnée de caméras grand public FLIR FX utilisait un accès uniquement cloud et ne prend pas en charge le streaming RTSP. Ces caméras ne peuvent pas être connectées via des URL RTSP directes.

### Caméras FLIR Lorex

FLIR a racheté Lorex, mais les caméras Lorex utilisent leurs propres modèles d'URL RTSP (basés sur le firmware Dahua). N'utilisez pas les modèles d'URL FLIR pour les caméras Lorex. Consultez la page [Lorex](lorex.md) pour les URL spécifiques à Lorex.

### Disponibilité d'ONVIF

ONVIF est pris en charge sur les caméras de génération actuelle (Quasar, Saros, Elara). Les caméras FLIR plus anciennes et les modèles grand public (FLIR FX) ne prennent pas en charge ONVIF. Pour les modèles prenant en charge ONVIF, utilisez la découverte ONVIF comme alternative à la configuration manuelle d'URL RTSP.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras FLIR ?**

Pour la plupart des caméras FLIR, utilisez `rtsp://admin:admin@CAMERA_IP:554/ch0` pour le flux visible. Pour les caméras thermiques à double capteur, utilisez `ch1` pour le flux thermique. Pour les caméras de la série PT, utilisez `/vis` (visible) ou `/wfov` (thermique grand champ).

**FLIR prend-il en charge H.265 ?**

Les caméras de la série Quasar prennent en charge l'encodage H.265. Les autres gammes de caméras FLIR utilisent principalement H.264 et MJPEG. Consultez la fiche technique de votre modèle spécifique pour la prise en charge des codecs.

**Comment accéder au flux thermique sur une caméra FLIR à double capteur ?**

Les caméras à double capteur fournissent des flux RTSP distincts pour les canaux visible et thermique. Sur les modèles Elara, `ch0` est visible et `ch1` est thermique. Sur les modèles de la série PT, `/vis` est visible et `/wfov` est le flux thermique grand champ. Connectez-vous à l'URL appropriée pour le capteur souhaité.

**FLIR et Teledyne FLIR sont-elles la même société ?**

Oui. Teledyne Technologies a racheté FLIR Systems en 2021. La société opère désormais sous le nom de Teledyne FLIR. Les caméras FLIR existantes continuent de fonctionner avec les mêmes modèles d'URL RTSP. Les produits plus récents peuvent porter la marque Teledyne FLIR.

**Puis-je utiliser les modèles d'URL FLIR pour les caméras Lorex ?**

Non. Bien que FLIR ait racheté Lorex, les caméras Lorex utilisent un firmware basé sur Dahua avec des modèles d'URL RTSP différents. Consultez le guide de connexion des caméras [Lorex](lorex.md) pour les URL correctes.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Basler](basler.md) — Caméras industrielles / vision industrielle
- [Guide de connexion Mobotix](mobotix.md) — Caméras industrielles allemandes
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
