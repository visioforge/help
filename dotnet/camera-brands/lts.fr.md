---
title: Caméras IP LTS — URL RTSP et guide de connexion C# .NET
description: Caméras LTS (LT Security) en C# .NET avec URL RTSP et exemples de code pour les séries CMIP, CMHR et les NVR. Firmware Hikvision.
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

# Comment se connecter à une caméra IP LTS en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**LTS (LT Security Inc.)** est une société américaine de sécurité basée à City of Industry, en Californie. Les caméras LTS sont fabriquées par **Hikvision** et utilisent le firmware, les protocoles et les interfaces web Hikvision. LTS remarque le matériel Hikvision avec un support technique basé aux États-Unis et des prix compétitifs, ce qui en fait un choix populaire sur le marché des installations professionnelles.

Puisque les caméras LTS exécutent le firmware Hikvision, elles utilisent le même format d'URL RTSP, la même implémentation ONVIF et les mêmes endpoints API que les caméras Hikvision. Tout code d'intégration écrit pour Hikvision fonctionne avec LTS et vice versa.

**Faits clés :**

- **Gammes de produits :** CMIP (caméras IP), CMHR (HD-TVI analogiques), LTD (DVR), LTN (NVR)
- **Protocoles pris en charge :** RTSP, ONVIF Profile S/G/T, HTTP/ISAPI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / 123456 (certains modèles : admin / admin)
- **Prise en charge ONVIF :** Oui (tous les modèles actuels)
- **Codecs vidéo :** H.264, H.265 (CMIP4xxx et plus récents)
- **Base OEM :** Hikvision (format d'URL RTSP identique)

!!! info "LTS = OEM Hikvision"
    Les caméras LTS utilisent le firmware Hikvision et exactement le même format d'URL RTSP que les caméras Hikvision. Tout code écrit pour les caméras Hikvision fonctionne avec LTS. Consultez notre [guide de connexion Hikvision](hikvision.md) pour des détails supplémentaires.

## Modèles d'URL RTSP

### Format d'URL standard (Hikvision)

La plupart des caméras LTS actuelles utilisent le modèle d'URL Hikvision standard `Streaming/Channels` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| Paramètre | Valeur | Description |
|-----------|--------|-------------|
| `CHANNEL_ID` | 101 | Canal 1, flux principal |
| `CHANNEL_ID` | 102 | Canal 1, sous-flux |
| `CHANNEL_ID` | 201 | Canal 2, flux principal (NVR) |
| `CHANNEL_ID` | 202 | Canal 2, sous-flux (NVR) |

!!! note "Double barre oblique dans l'URL"
    Certaines caméras LTS/Hikvision utilisent `//Streaming/Channels/1` (avec une double barre oblique avant `Streaming`). Les variantes simple et double barre oblique fonctionnent généralement, mais essayez la version à double barre oblique si l'URL à simple barre oblique échoue.

### URL RTSP par modèle

| Modèle | URL RTSP | Résolution | Notes |
|--------|----------|------------|-------|
| CMIP3122 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Format Hikvision standard |
| CMIP3132-28 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Format Hikvision standard |
| CMIP3432 | `rtsp://IP:554/Streaming/Channels/101` | 4MP | Format Hikvision standard |
| CMIP3243 | `rtsp://IP:554/live.h264` | 3MP | Flux H.264 alternatif |
| CMIP3412-28 | `rtsp://IP:554/live.h264` | 4MP | Flux H.264 alternatif |
| CMIP8232 | `rtsp://IP:554/live.sdp` | 8MP/4K | Flux SDP en direct |
| CMIP8232 (alt) | `rtsp://IP:554/HighResolutionVideo` | 8MP/4K | Flux haute résolution |
| CMIP8232 (sub) | `rtsp://IP:554/h264/ch1/sub/` | 8MP/4K | Sous-flux H.264 |
| Série CMIP (basse rés.) | `rtsp://IP:554/LowResolutionVideo` | Variable | Sous-flux basse résolution |

### Formats d'URL alternatifs

Certains anciens modèles LTS ou versions de firmware spécifiques prennent en charge ces URL alternatives :

| Modèle d'URL | Notes |
|--------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Hikvision standard (recommandé) |
| `rtsp://IP:554//Streaming/Channels/1` | Variante double barre oblique |
| `rtsp://IP:554/live.h264` | Flux H.264 en direct (anciens CMIP3xxx) |
| `rtsp://IP:554/live.sdp` | Flux SDP en direct (CMIP8xxx) |
| `rtsp://IP:554/HighResolutionVideo` | Flux haute résolution nommé |
| `rtsp://IP:554/LowResolutionVideo` | Flux basse résolution nommé |
| `rtsp://IP:554/h264/ch1/sub/` | Sous-flux H.264 par canal |
| `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | MPEG-4 avec authentification basée sur l'URL |

### URL des canaux NVR (série LTN)

Pour les NVR LTS (LTN8704, LTN8708, LTN8716, etc.) :

| Canal | Flux principal | Sous-flux |
|-------|----------------|-----------|
| Caméra 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Caméra 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Caméra N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Récapitulatif des séries de modèles

| Série de modèles | URL RTSP principale | URL alternatives |
|------------------|---------------------|------------------|
| CMIP3xxx (3MP) | `/Streaming/Channels/101` | `/live.h264` (certains modèles) |
| CMIP4xxx (4MP) | `/Streaming/Channels/101` | `/live.h264` (certains modèles) |
| CMIP8xxx (8MP/4K) | `/Streaming/Channels/101` | `/live.sdp`, `/HighResolutionVideo`, `/h264/ch1/sub/` |
| NVR LTN | `/Streaming/Channels/N01` | Basé sur le canal |
| DVR LTD | `/Streaming/Channels/N01` | Basé sur le canal |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra LTS avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// LTS CMIP3432, flux principal (format Hikvision)
var uri = new Uri("rtsp://192.168.1.80:554/Streaming/Channels/101");
var username = "admin";
var password = "123456";
```

Pour accéder au sous-flux, utilisez l'ID de canal `102` au lieu de `101`.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot JPEG | `http://IP/snapshot.jpg` | Snapshot standard |
| Snapshot 3GP | `http://IP/snapshot_3gp.jpg` | Format 3GP (optimisé pour mobile) |
| Snapshot de flux | `http://IP/stream.jpg` | Snapshot basé sur le flux |
| Snapshot canal DVR | `http://IP/stillimg[CHANNEL].jpg` | Remplacez `[CHANNEL]` par le numéro de canal (DVR LTD) |
| Snapshot ISAPI | `http://IP/ISAPI/Streaming/channels/101/picture` | ISAPI Hikvision (nécessite une auth) |

## Dépannage

### Identifier le bon format d'URL

Les caméras LTS couvrent plusieurs générations avec différents formats d'URL. Pour déterminer quelle URL utilise votre caméra :

1. Essayez d'abord le format Hikvision standard : `rtsp://IP:554/Streaming/Channels/101`
2. Si cela échoue, essayez la variante à double barre oblique : `rtsp://IP:554//Streaming/Channels/1`
3. Pour les anciens modèles CMIP3xxx, essayez `rtsp://IP:554/live.h264`
4. Pour les modèles CMIP8xxx (4K), essayez `rtsp://IP:554/live.sdp`

### Identifiants par défaut et activation

- Anciennes caméras LTS : le mot de passe par défaut est `123456` ou `admin`
- Nouvelles caméras LTS (avec firmware Hikvision 5.3+) : nécessitent une activation par mot de passe lors de la première utilisation, similaire à Hikvision
- Si vous ne pouvez pas vous connecter avec les identifiants par défaut, la caméra peut devoir être activée via l'outil LTS Discovery Tool ou Hikvision SADP Tool

### Utilisation des outils Hikvision avec les caméras LTS

Puisque les caméras LTS exécutent le firmware Hikvision, vous pouvez utiliser les utilitaires Hikvision pour la découverte réseau et la configuration :

- **Hikvision SADP Tool** — détecte les caméras LTS sur le réseau local et peut les activer/réinitialiser
- **LTS Discovery Tool** — version étiquetée LTS de SADP avec une fonctionnalité identique
- **iVMS-4200** — le logiciel VMS gratuit de Hikvision fonctionne avec les caméras LTS

### Erreur « 401 Unauthorized »

1. Vérifiez que vos identifiants sont corrects (par défaut : admin / 123456)
2. Sur les firmwares plus récents, assurez-vous que la caméra a été activée et que vous utilisez le mot de passe défini lors de l'activation
3. Vérifiez si la caméra a une politique de verrouillage — trop de tentatives de connexion échouées peuvent bloquer temporairement l'accès
4. Certains modèles nécessitent une authentification digest plutôt qu'une authentification basique pour RTSP

### Problème d'URL à double barre oblique

L'URL `//Streaming/Channels/1` avec une double barre oblique au début est un modèle Hikvision connu. Certains clients HTTP ou bibliothèques RTSP peuvent normaliser cela en une seule barre oblique. Si votre connexion échoue :

- Assurez-vous que votre chaîne d'URL préserve la double barre oblique
- Essayez les deux variantes `//Streaming/Channels/1` et `/Streaming/Channels/101`

## FAQ

**Les caméras LTS sont-elles les mêmes que Hikvision ?**

Les caméras LTS sont fabriquées par Hikvision et exécutent le firmware Hikvision. Le format d'URL RTSP (`/Streaming/Channels/101`), l'implémentation ONVIF et l'interface ISAPI sont identiques. Les principales différences sont la marque, les prix et le support technique basé aux États-Unis de LTS. Tout code écrit pour les caméras Hikvision fonctionne avec les caméras LTS.

**Quelle est l'URL RTSP par défaut pour les caméras LTS ?**

Pour la plupart des caméras LTS actuelles, utilisez `rtsp://admin:123456@CAMERA_IP:554/Streaming/Channels/101` pour le flux principal. Utilisez l'ID de canal `102` pour le sous-flux. Les modèles plus anciens peuvent utiliser `/live.h264` ou `/live.sdp` à la place.

**Les caméras LTS prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras IP LTS actuelles (série CMIP) prennent en charge ONVIF Profile S et Profile T. ONVIF peut être utilisé pour la découverte automatique et la configuration aux côtés des URL RTSP directes.

**Quelle est la différence entre les séries CMIP et CMHR ?**

Les caméras CMIP sont des caméras IP (réseau) qui prennent en charge le streaming RTSP. Les caméras CMHR sont des caméras analogiques HD-TVI qui se connectent directement aux DVR via un câble coaxial et n'ont pas de capacité RTSP réseau. Seules les caméras de la série CMIP peuvent être connectées via des URL RTSP dans un logiciel.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Hikvision](hikvision.md) — Même format d'URL (base OEM)
- [Guide de connexion Annke](annke.md) — Autre OEM Hikvision
- [Guide d'intégration de caméra RTSP](../videocapture/video-sources/ip-cameras/rtsp.md) — Configuration des flux RTSP LTS
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
