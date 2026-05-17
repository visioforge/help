---
title: URL RTSP des caméras IP EverFocus en C# .NET — intégration
description: Modèles d'URL RTSP EverFocus séries EAN, EHN, EMN, EPN et EQN pour C# .NET. Diffusez et enregistrez avec le VisioForge Video Capture SDK.
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

# Comment se connecter à une caméra IP EverFocus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**EverFocus Electronics** est une entreprise taïwanaise de vidéosurveillance professionnelle dont le siège est à Nouveau Taipei, à Taïwan, avec des opérations américaines basées à Duarte, en Californie. Fondée en 1995, EverFocus fabrique des caméras IP, DVR et solutions de vidéosurveillance mobile conçues pour les intégrateurs de sécurité professionnels. La société est bien connue sur le marché de la vidéosurveillance commerciale et industrielle.

**Faits clés :**

- **Gammes de produits :** EAN (bullet), EHN (dôme), EMN (mini dôme), EPN (PTZ), EZN (compactes), EQN (turret), ECOR/EPARA (DVR)
- **Prise en charge des protocoles :** RTSP, ONVIF, HTTP/CGI
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (toutes les caméras IP actuelles)
- **Codecs vidéo :** H.264 (tous les modèles actuels)

!!! info "Format d'URL RTSP EverFocus"
    Les caméras EverFocus utilisent un chemin unique `rtspStreamOvf` dans leurs URL RTSP. Ce format est spécifique à EverFocus et ne doit pas être confondu avec les modèles d'URL d'autres fabricants. Notez la double barre (`//`) requise avant `cgi-bin`.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras EverFocus utilisent le chemin CGI `rtspStreamOvf` pour le streaming RTSP :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//cgi-bin/rtspStreamOvf/0
```

| Paramètre | Valeur | Description |
|-----------|-------|-------------|
| Index de flux | `/0` | Flux principal (résolution la plus élevée) |
| Index de flux | `/1` | Sous-flux (résolution inférieure, moins de bande passante) |

!!! warning "Double barre requise"
    Le chemin de l'URL doit inclure une double barre avant `cgi-bin` (c'est-à-dire `//cgi-bin/rtspStreamOvf/...`). Omettre la barre de tête provoquera l'échec de la connexion.

### Format d'URL principal (rtspStreamOvf)

La plupart des caméras IP EverFocus utilisent le format `rtspStreamOvf` :

| Modèle | Série | URL du flux principal | URL du sous-flux |
|-------|--------|----------------|----------------|
| EAN3220 | EAN (bullet) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EHN3260 | EHN (dôme) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN2220 | EMN (mini dôme) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN1360 | EMN (mini dôme) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EMN3260 | EMN (mini dôme) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EPN4220 | EPN (PTZ) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |
| EZN3160 | EZN (compacte) | `rtsp://IP:554//cgi-bin/rtspStreamOvf/0` | `rtsp://IP:554//cgi-bin/rtspStreamOvf/1` |

### Format d'URL alternatif (streaming/channels)

Certains modèles EverFocus plus récents prennent également en charge le format `streaming/channels` :

| Modèle | Série | URL du flux principal |
|-------|--------|----------------|
| EPN4220 | EPN (PTZ) | `rtsp://IP:554/streaming/channels/0` |
| EZN3240 | EZN (compacte) | `rtsp://IP:554/streaming/channels/0` |
| EHN3260 | EHN (dôme) | `rtsp://IP:554/streaming/channels/0` |

!!! tip "Quel format utiliser"
    Essayez d'abord le format `rtspStreamOvf`, car il est pris en charge sur toutes les gammes de produits de caméras IP EverFocus. Le format `streaming/channels` est une alternative disponible sur certains modèles plus récents.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra EverFocus avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// EverFocus EHN3260, flux principal
var uri = new Uri("rtsp://192.168.1.90:554//cgi-bin/rtspStreamOvf/0");
var username = "admin";
var password = "admin";
```

Pour accéder au sous-flux, utilisez `/1` au lieu de `/0` à la fin de l'URL.

## URL de capture instantanée et MJPEG

| Type | Modèle d'URL | Modèles pris en charge |
|------|-------------|------------------|
| Capture (avec auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=CHANNEL` | EQN2101 |
| Capture (simple) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | Caméras IP générales |
| Capture mobile | `http://IP/m/camera[CHANNEL].jpg` | DVR ECOR/EPARA |

!!! note "Captures DVR"
    Pour les modèles DVR ECOR et EPARA, remplacez `[CHANNEL]` par le numéro de canal de la caméra (par exemple, `camera1.jpg` pour le canal 1).

## Dépannage

### Connexion refusée ou expirée

Les caméras EverFocus utilisent le chemin CGI `rtspStreamOvf` qui est unique à cette marque. Assurez-vous de ne pas utiliser accidentellement un format d'URL d'un autre fabricant :

1. Vérifiez que l'URL inclut la double barre : `//cgi-bin/rtspStreamOvf/0`
2. Confirmez que le port RTSP est 554 (ou vérifiez les paramètres réseau de la caméra pour un port personnalisé)
3. Assurez-vous que la caméra est accessible sur le réseau en pingant son adresse IP

### Les index de flux commencent à 0

Contrairement à certaines autres marques de caméras où les canaux commencent à 1, les index de flux EverFocus commencent à **0** :

- `/0` = Flux principal (résolution complète)
- `/1` = Sous-flux (résolution réduite)

Utiliser `/1` en s'attendant au flux principal renverra plutôt le sous-flux.

### Le format d'URL alternatif ne fonctionne pas

Le format d'URL `streaming/channels/0` n'est disponible que sur certains modèles plus récents (EPN4220, EZN3240, EHN3260). Si ce format ne fonctionne pas, repliez-vous sur le format standard `//cgi-bin/rtspStreamOvf/0`.

### Problèmes d'authentification

Les caméras EverFocus utilisent par défaut `admin` / `admin`. Si vous avez modifié le mot de passe via l'interface web et l'avez oublié, un bouton de réinitialisation matériel sur la caméra restaurera les valeurs par défaut d'usine.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras EverFocus ?**

L'URL par défaut est `rtsp://admin:admin@CAMERA_IP:554//cgi-bin/rtspStreamOvf/0` pour le flux principal. Utilisez `/1` à la fin pour le sous-flux. Notez la double barre avant `cgi-bin` qui est requise.

**Pourquoi l'URL RTSP EverFocus semble-t-elle différente de celle d'autres caméras ?**

EverFocus utilise un chemin CGI propriétaire `rtspStreamOvf` qui est unique à leur firmware de caméra. C'est différent des formats plus courants utilisés par Hikvision, Dahua ou des chemins ONVIF génériques. La double barre (`//cgi-bin/...`) est intentionnelle et requise.

**Les caméras EverFocus prennent-elles en charge ONVIF ?**

Oui. Toutes les caméras IP EverFocus actuelles prennent en charge ONVIF, qui fournit un moyen standardisé de découvrir et de se connecter à la caméra. Vous pouvez utiliser ONVIF comme alternative au format d'URL RTSP propriétaire.

**Puis-je me connecter aux DVR EverFocus (ECOR/EPARA) via RTSP ?**

Les DVR EverFocus exposent principalement des URL de capture basées sur HTTP pour les canaux individuels (`http://IP/m/camera[CHANNEL].jpg`). Pour le streaming RTSP depuis les canaux DVR, consultez la documentation de votre modèle DVR spécifique ou utilisez la découverte ONVIF.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Speco](speco.md) — Caméras de vidéosurveillance professionnelle
- [Guide de streaming vidéo RTSP](../general/network-streaming/rtsp.md) — Configuration streaming RTSP EverFocus
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
