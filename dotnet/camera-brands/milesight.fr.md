---
title: Caméras IP Milesight — URL RTSP et intégration C# .NET
description: Modèles d'URL RTSP des séries Milesight MS-C, MS-A, MS-V pour C# .NET. Intégration conforme ONVIF avec exemples de code du VisioForge Video Capture SDK.
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
  - H.265
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Milesight en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Milesight Technology** est un fabricant chinois de caméras IP et de périphériques IoT, dont le siège social est à Xiamen, en Chine. Milesight cible les marchés professionnels et PME avec une gamme en croissance rapide de caméras dotées d'IA à des prix compétitifs. La marque est reconnue pour sa forte conformité ONVIF, ses analyses IA intégrées (détection de visages, reconnaissance LPR, comptage de personnes) et son intégration simple avec les plateformes VMS tierces.

**Faits clés :**

- **Gammes de produits :** MS-C (mini dôme/bullet), MS-A (PTZ/speed dome), MS-V (dôme antivandalisme), MS-F (fisheye), MS-B (box), MS-N (NVR)
- **Protocoles pris en charge :** RTSP, ONVIF (Profile S/G/T sur tous les modèles actuels), HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / ms1234 (doit être changé à la première connexion)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels, Profile S/G/T)
- **Codecs vidéo :** H.264, H.265, MJPEG

!!! info "Double barre oblique dans les URL RTSP"
    Les caméras Milesight utilisent une **double barre oblique** avant `main` et `sub` dans leurs URL RTSP : `rtsp://IP:554//main`. Cela est intentionnel et requis pour tous les modèles Milesight actuels.

## Modèles d'URL RTSP

### Modèles actuels (toutes les séries)

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Flux principal | `rtsp://IP:554//main` | Résolution complète (notez la double barre oblique) |
| Sous-flux | `rtsp://IP:554//sub` | Résolution inférieure |
| Flux racine | `rtsp://IP:554/` | Solution de repli |

### URL spécifiques aux modèles

Tous les modèles de caméras Milesight actuels utilisent le même modèle d'URL RTSP :

| Série de modèles | URL RTSP | Type | Notes |
|------------------|----------|------|-------|
| MS-C2672-P | `rtsp://IP:554//main` | Mini dôme | 2MP |
| MS-C3366-FP | `rtsp://IP:554//main` | Bullet | 3MP, IA |
| MS-C3366-FPH | `rtsp://IP:554//main` | Bullet | 3MP, IA, chauffage |
| MS-C2363 | `rtsp://IP:554//main` | Mini dôme | 2MP |
| MS-2681 | `rtsp://IP:554//main` | Dôme | 8MP |
| MS-3672 | `rtsp://IP:554//main` | Bullet | 3MP |
| Série MS-A (PTZ) | `rtsp://IP:554//main` | PTZ | Speed dome |
| Série MS-V (antivandalisme) | `rtsp://IP:554//main` | Dôme antivandalisme | Classé IK10 |
| Série MS-F (fisheye) | `rtsp://IP:554//main` | Fisheye | 360 degrés |
| Série MS-B (box) | `rtsp://IP:554//main` | Box | Professionnel |

### Flux par canal NVR

Pour les NVR Milesight de la série MS-N, utilisez le paramètre `channel` pour sélectionner les flux des caméras individuelles :

| Flux | URL RTSP | Notes |
|------|----------|-------|
| Canal 1, principal | `rtsp://IP:554//main?channel=1` | Canal 1 du NVR |
| Canal 2, principal | `rtsp://IP:554//main?channel=2` | Canal 2 du NVR |
| Canal 1, sous-flux | `rtsp://IP:554//sub?channel=1` | Canal 1 du NVR, basse résolution |
| Canal 2, sous-flux | `rtsp://IP:554//sub?channel=2` | Canal 2 du NVR, basse résolution |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Milesight avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Milesight, flux principal — notez la double barre oblique avant "main"
var uri = new Uri("rtsp://192.168.1.90:554//main");
var username = "admin";
var password = "ms1234";
```

Pour accéder au sous-flux, utilisez `//sub` au lieu de `//main`. Pour la sélection de canal NVR, ajoutez `?channel=N` à l'URL.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot CGI | `http://IP/cgi-bin/snapshot.cgi` | Authentification basique requise |

## Dépannage

### La double barre oblique est requise

Les caméras Milesight nécessitent une **double barre oblique** avant le chemin du flux :

- Correct : `rtsp://IP:554//main`
- Peut ne pas fonctionner : `rtsp://IP:554/main`

Si votre connexion échoue, vérifiez que vous utilisez le format d'URL à double barre oblique. Ce modèle est similaire à celui des caméras Pelco et ACTi.

### Le mot de passe par défaut doit être changé

Le mot de passe d'usine par défaut est `ms1234`, mais les caméras Milesight exigent que ce mot de passe soit changé lors de la première connexion via l'interface web ou Milesight CMS. Si le mot de passe par défaut ne fonctionne pas, la caméra a probablement été configurée avec un nouveau mot de passe.

### Les fonctionnalités IA sont indépendantes de RTSP

Les fonctionnalités IA de Milesight (détection de visages, reconnaissance de plaques d'immatriculation, comptage de personnes, détection d'intrusion) fonctionnent sur le processeur intégré de la caméra et n'affectent pas le streaming RTSP. Les métadonnées et événements IA sont délivrés via les événements ONVIF ou l'API Milesight, pas via le flux RTSP lui-même.

### Milesight CMS est optionnel

Milesight CMS (Central Management Software) est le VMS propriétaire de Milesight. Il n'est pas requis pour le streaming RTSP. Les caméras Milesight fonctionnent avec tout VMS compatible ONVIF ou toute application prenant en charge les connexions RTSP standard.

### Numérotation des canaux NVR

Lors de la connexion via un NVR Milesight MS-N, les numéros de canal commencent à 1 et correspondent à l'entrée caméra physique ou à l'ordre des caméras réseau configuré dans le NVR. Utilisez `?channel=1` pour la première caméra, `?channel=2` pour la deuxième, et ainsi de suite.

### Découverte ONVIF

Toutes les caméras Milesight actuelles prennent en charge ONVIF Profile S, G et T. Si vous préférez la découverte automatique à la configuration manuelle des URL RTSP, utilisez la découverte de périphériques ONVIF pour trouver les caméras sur votre réseau et récupérer automatiquement leurs URL de streaming.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Milesight ?**

Pour toutes les caméras Milesight actuelles, utilisez `rtsp://admin:ms1234@CAMERA_IP:554//main` (notez la double barre oblique). Pour le sous-flux, utilisez `//sub`. Pour l'accès NVR, ajoutez `?channel=N` pour sélectionner le canal de caméra souhaité.

**Tous les modèles Milesight utilisent-ils la même URL RTSP ?**

Oui. Tous les modèles de caméras Milesight actuels (séries MS-C, MS-A, MS-V, MS-F, MS-B) utilisent le même modèle d'URL `//main` et `//sub`. Cela fait de Milesight l'une des marques les plus cohérentes pour l'intégration RTSP.

**Milesight prend-il en charge H.265 ?**

Oui. Toutes les caméras Milesight actuelles prennent en charge l'encodage H.264, H.265 et MJPEG. H.265 peut être activé via l'interface web de la caméra ou Milesight CMS pour réduire la bande passante et les besoins de stockage.

**Pourquoi la double barre oblique est-elle importante dans les URL Milesight ?**

La double barre oblique (`//main` au lieu de `/main`) fait partie de la spécification d'URL RTSP de Milesight. L'omission de la barre oblique supplémentaire peut provoquer des échecs de connexion. Cette convention est partagée avec quelques autres marques de caméras (Pelco, ACTi) mais n'est pas universelle dans l'industrie.

**Puis-je accéder aux analyses IA Milesight via RTSP ?**

Non. RTSP délivre uniquement le flux vidéo. Les résultats des analyses IA (événements de détection de visages, données de plaques d'immatriculation, statistiques de comptage de personnes) sont accessibles via les événements ONVIF, l'API HTTP Milesight ou Milesight CMS. Le flux vidéo lui-même ne contient pas de métadonnées IA intégrées.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Grandstream](grandstream.md) — Segment caméras PME / professionnel
- [Capture de caméra IP vers MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Enregistrer les flux Milesight dans un fichier
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
