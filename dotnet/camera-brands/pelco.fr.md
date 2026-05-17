---
title: Caméras IP Pelco — URL RTSP et intégration en C# .NET
description: Intégration RTSP des caméras Pelco Sarix et Spectra PTZ pour C# .NET. Modèles d'URL pour IX, IMP, IME avec prise en charge ONVIF et code SDK VisioForge.
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

# Comment se connecter à une caméra IP Pelco en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Pelco** (maintenant partie de **Motorola Solutions**) est un fabricant de premier plan d'équipements de vidéosurveillance professionnels, dont le siège social est à Fresno, en Californie. Pelco est particulièrement fort sur les marchés de l'entreprise, du gouvernement et des infrastructures critiques. La marque est reconnue pour sa gamme de caméras fixes **Sarix** et sa gamme de caméras PTZ **Spectra**. Motorola Solutions a acquis Pelco en 2020.

**Faits clés :**

- **Gammes de produits :** Sarix (caméras fixes Professional/Enhanced/Value), Spectra (PTZ professionnel), IX (box fixe), IMP/IME (mini dôme), série D (dôme PTZ)
- **Protocoles pris en charge :** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, protocole série Pelco D/P
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin (doit être changé à la première connexion pour les modèles actuels)
- **Prise en charge ONVIF :** Oui (tous les modèles Sarix et Spectra actuels)
- **Codecs vidéo :** H.264, H.265 (Sarix Professional), MJPEG

!!! info "Double barre oblique dans les URL RTSP"
    Les caméras Pelco utilisent systématiquement une **double barre oblique** avant le chemin du flux : `rtsp://IP:554//stream1`. Cela est intentionnel et requis pour la plupart des modèles Pelco.

## Modèles d'URL RTSP

### Modèles actuels (Sarix Professional/Enhanced/Value)

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Flux principal | `rtsp://IP:554//stream1` | Résolution complète (notez la double barre oblique) |
| Sous-flux | `rtsp://IP:554//stream2` | Résolution inférieure |
| Flux basse résolution | `rtsp://IP:554/LowResolutionVideo` | Qualité la plus basse |
| Flux canal | `rtsp://IP:554/stream1` | Simple barre oblique (certains modèles) |
| Canal numéroté | `rtsp://IP:554/1/stream1` | Spécifique au canal |

### URL spécifiques aux modèles

| Série de modèles | URL RTSP | Type | Notes |
|------------------|----------|------|-------|
| Sarix Pro (IMP/IME) | `rtsp://IP:554//stream1` | Dôme fixe | Génération actuelle |
| Sarix Enhanced (IX) | `rtsp://IP:554//stream1` | Box fixe | Milieu de gamme |
| Sarix Value | `rtsp://IP:554//stream1` | Fixe | Entrée de gamme |
| IX10 | `rtsp://IP:554//stream1` | Box fixe | Professionnel |
| IX30C / IX30DN | `rtsp://IP:554//stream1` | Box fixe | Jour/nuit |
| IXDN30 | `rtsp://IP:554//stream1` | Box fixe | Jour/nuit |
| IXE10LW | `rtsp://IP:554//stream1` | Dôme fixe | Sans fil |
| IXE20DN | `rtsp://IP:554//stream1` | Dôme fixe | Jour/nuit |
| IXP31 | `rtsp://IP:554//stream1` | Dôme fixe | Professionnel |
| IMP519 | `rtsp://IP:554//stream1` | Mini dôme | 5MP |
| IMP1110-1 / IMP1110-1E | `rtsp://IP:554//stream1` | Mini dôme | Sarix Pro |
| IM10C10 | `rtsp://IP:554//stream1` | Multi-capteur | Sarix IMM |
| IM10DN10-1E | `rtsp://IP:554//stream1` | Multi-capteur | Jour/nuit |
| D5230-ADFRZ28 | `rtsp://IP:554//stream1` | Dôme PTZ | Spectra |
| Spectra IV | `rtsp://IP:554//stream1` | Dôme PTZ | PTZ hérité |
| Spectra Professional | `rtsp://IP:554//stream1` | Dôme PTZ | PTZ actuel |

### Multi-canal / multi-capteur

Pour les périphériques Pelco multi-canal :

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Canal 1, principal | `rtsp://IP:554/1/stream1` | Premier capteur/canal |
| Canal 2, principal | `rtsp://IP:554/2/stream1` | Second capteur/canal |
| Flux canal (alt) | `rtsp://IP:554/stream1` | Canal unique (certains modèles) |

### Modèles hérités

| Modèle | URL | Notes |
|--------|-----|-------|
| IP110 / IP-110 | `http://IP/api/jpegControl.php?frameRate=10` | Flux JPEG |
| Spectra IV (HTTP) | `http://IP/jpeg` | Snapshot JPEG |
| Spectra IV (pull) | `http://IP/jpeg/pull` | JPEG continu |
| Spectra IV (API) | `http://IP/api/jpegControl.php?frameRate=10` | JPEG à fréquence d'images |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Pelco avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Pelco Sarix, flux principal
var uri = new Uri("rtsp://192.168.1.85:554//stream1");
var username = "admin";
var password = "YourPassword";
```

Pour accéder au sous-flux, utilisez `//stream2` à la place. Pour les caméras multi-capteur, utilisez `/1/stream1` pour la sélection de canal.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG | `http://IP/jpeg` | La plupart des modèles actuels |
| JPEG (canal) | `http://IP/jpeg?id=1` | Spécifique au canal |
| JPEG (API) | `http://IP/api/jpegControl.php?frameRate=10` | Modèles hérités |
| JPEG (tmpfs) | `http://IP/tmpfs/auto.jpg` | Auto-capture |
| Fichier image | `http://IP/img.jpg` | Snapshot simple |

## Dépannage

### La double barre oblique est requise

La plupart des caméras Pelco nécessitent une **double barre oblique** avant le chemin du flux :

- Correct : `rtsp://IP:554//stream1`
- Peut ne pas fonctionner : `rtsp://IP:554/stream1`

Si une URL à simple barre oblique échoue, essayez toujours d'abord la variante à double barre oblique.

### Numérotation des canaux pour multi-capteur

Les caméras Pelco multi-capteur (série IM10, Sarix IMM) utilisent des chemins de canal numérotés :

- `rtsp://IP:554/1/stream1` — premier capteur
- `rtsp://IP:554/2/stream1` — second capteur

Les caméras à capteur unique doivent utiliser `//stream1` sans numéro de canal.

### Protocole Pelco D/P contre RTSP

Pelco est également connu pour les protocoles de communication série **Pelco D** et **Pelco P** utilisés pour contrôler les caméras PTZ. Ce sont des protocoles série pour le contrôle PTZ, pas pour le streaming vidéo. Le streaming vidéo utilise toujours RTSP ou HTTP, quel que soit le protocole de contrôle PTZ utilisé.

### Caméras PTZ Spectra

Les caméras Pelco Spectra PTZ utilisent le même format d'URL RTSP (`//stream1`) que les caméras fixes. Le contrôle PTZ est géré séparément via les commandes ONVIF PTZ ou le protocole série Pelco D/P, pas via l'URL RTSP.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Pelco ?**

Pour la plupart des caméras Pelco, utilisez `rtsp://admin:password@CAMERA_IP:554//stream1` (notez la double barre oblique). Pour le sous-flux, utilisez `//stream2`. Les modèles multi-capteur utilisent `/1/stream1` pour un accès spécifique au canal.

**Pelco est-elle toujours une société indépendante ?**

Non. Pelco a été acquise par Motorola Solutions en 2020. Les caméras Pelco actuelles sont fabriquées et prises en charge par Motorola Solutions. La marque Pelco et les gammes de produits (Sarix, Spectra) continuent sous le portefeuille de sécurité vidéo de Motorola Solutions.

**Les caméras Pelco prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras Pelco Sarix et Spectra actuelles prennent en charge ONVIF Profile S, G et T. ONVIF est la méthode de découverte et de configuration recommandée pour les nouvelles intégrations Pelco.

**Quelle est la différence entre Pelco D et RTSP ?**

Pelco D (et Pelco P) sont des protocoles série pour le contrôle des caméras PTZ (commandes pan, tilt, zoom). RTSP est le protocole de streaming vidéo. Vous utilisez RTSP pour la vidéo et Pelco D/ONVIF pour le contrôle PTZ — ils servent à des fins différentes et ne sont pas interchangeables.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Avigilon](avigilon.md) — Également Motorola Solutions, caméras d'entreprise
- [Guide de connexion Honeywell](honeywell.md) — Caméras de vidéosurveillance d'entreprise
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
