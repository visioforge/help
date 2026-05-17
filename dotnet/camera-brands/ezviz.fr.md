---
title: URL RTSP des caméras IP EZVIZ et guide de connexion C# .NET
description: Connectez les caméras EZVIZ en C# .NET avec modèles d'URL RTSP pour C1C, C3W, C6N, BC1C et autres. Activez RTSP sur les caméras cloud d'abord.
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
  - C#

---

# Comment se connecter à une caméra IP EZVIZ en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**EZVIZ** est une marque grand public de caméras de sécurité et de maison connectée détenue par **Hikvision**. Lancée à l'origine comme la division grand public d'Hikvision, EZVIZ est devenue une marque indépendante axée sur les caméras résidentielles, sonnettes, serrures intelligentes et appareils IoT. Les caméras EZVIZ sont conçues principalement pour une utilisation cloud via l'application EZVIZ, mais de nombreux modèles prennent en charge le streaming RTSP local lorsqu'il est activé.

**Faits clés :**

- **Gammes de produits :** Série C (intérieur/extérieur), série BC (batterie), série DB (sonnettes), série H (pan/tilt)
- **Prise en charge des protocoles :** RTSP (doit être activé), ONVIF (modèles limités), EZVIZ Cloud (par défaut)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / code de vérification (imprimé sur l'étiquette de la caméra)
- **Prise en charge ONVIF :** Limitée (certains modèles plus récents uniquement)
- **Codecs vidéo :** H.264, H.265 (modèles sélectionnés)
- **Société mère :** Hikvision

!!! warning "RTSP doit être activé manuellement"
    Les caméras EZVIZ sont des appareils cloud d'abord. **RTSP est désactivé par défaut** sur la plupart des modèles. Vous devez activer RTSP via l'application mobile EZVIZ ou le portail web avant de vous connecter avec le SDK VisioForge. Certains modèles économiques et à batterie ne prennent pas du tout en charge RTSP.

## Activation de RTSP sur les caméras EZVIZ

Avant de vous connecter, activez l'accès RTSP :

1. Ouvrez l'**application EZVIZ** sur votre téléphone
2. Sélectionnez votre caméra → **Settings** (icône d'engrenage)
3. Naviguez vers **Local Network** ou **LAN access**
4. Activez **RTSP** ou **Third-party access**
5. Notez le code de vérification (généralement imprimé sur l'étiquette de la caméra ou affiché dans l'application)

Vous pouvez également utiliser le portail web EZVIZ à `https://www.ezvizlife.com` pour gérer les paramètres de la caméra.

## Modèles d'URL RTSP

### Format d'URL standard

Les caméras EZVIZ utilisent des modèles d'URL RTSP dérivés d'Hikvision :

```
rtsp://admin:[VERIFICATION_CODE]@[IP]:554/h264/ch1/main/av_stream
```

| Flux | Modèle d'URL | Description |
|--------|-------------|-------------|
| Flux principal | `rtsp://IP:554/h264/ch1/main/av_stream` | Résolution complète |
| Sous-flux | `rtsp://IP:554/h264/ch1/sub/av_stream` | Résolution inférieure |

!!! info "Code de vérification comme mot de passe"
    Les caméras EZVIZ utilisent le **code de vérification** (imprimé sur l'étiquette de la caméra) comme mot de passe RTSP. Le nom d'utilisateur est toujours `admin`. Cela diffère du mot de passe du compte cloud EZVIZ.

### Formats d'URL alternatifs

Certains modèles EZVIZ prennent en charge des modèles d'URL supplémentaires :

| Modèle d'URL | Notes |
|-------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Standard (recommandé) |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Sous-flux |
| `rtsp://IP:554/Streaming/Channels/101` | Style Hikvision (certains modèles) |
| `rtsp://IP:554/Streaming/Channels/102` | Sous-flux style Hikvision |
| `rtsp://IP:554/live` | Chemin simple (modèles plus anciens) |

### Modèles de caméras

| Modèle | Type | Prise en charge RTSP | URL du flux principal |
|-------|------|-------------|----------------|
| C6N (intérieur pan/tilt) | Intérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C6W (intérieur PT 4MP) | Intérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C1C (intérieur 1080p) | Intérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H6c (pan/tilt) | Intérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H8c (extérieur PT) | Extérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3W (bullet extérieur) | Extérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3WN (bullet extérieur) | Extérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3X (double objectif extérieur) | Extérieur | Oui | `rtsp://IP:554/h264/ch1/main/av_stream` |
| BC1C (caméra batterie) | Batterie | Non | N/A — cloud uniquement |
| DB1C (sonnette) | Sonnette | Non | N/A — cloud uniquement |

!!! warning "Modèles batterie et sonnette"
    Les caméras EZVIZ alimentées par batterie (série BC) et les sonnettes vidéo (série DB) ne prennent généralement **pas** en charge RTSP. Ces appareils ne diffusent que via le cloud EZVIZ. Seules les caméras alimentées en courant alternatif avec connexion réseau filaire ou Wi-Fi stable prennent en charge RTSP.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra EZVIZ avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// EZVIZ C6N, flux principal (code de vérification depuis l'étiquette)
var uri = new Uri("rtsp://192.168.1.90:554/h264/ch1/main/av_stream");
var username = "admin";
var password = "ABCDEF"; // code de vérification depuis l'étiquette de la caméra
```

Pour accéder au sous-flux, utilisez `/h264/ch1/sub/av_stream` à la place.

## URL de capture

| Type | Modèle d'URL | Notes |
|------|-------------|-------|
| Capture JPEG | `http://IP/cgi-bin/snapshot.cgi` | Nécessite une authentification basique avec le code de vérification |

## Dépannage

### « Connection refused » ou aucune réponse

RTSP est désactivé par défaut sur les caméras EZVIZ. Vous devez d'abord l'activer via l'application EZVIZ. Vérifiez **Settings > Local Network > Third-party access**.

### Mauvais mot de passe

Les caméras EZVIZ utilisent le **code de vérification** (6 lettres majuscules imprimées sur l'étiquette de la caméra) comme mot de passe RTSP, **pas** votre mot de passe de compte cloud EZVIZ. Le nom d'utilisateur est toujours `admin`.

### Caméra pas sur le réseau local

Les caméras EZVIZ se connectent au cloud via Wi-Fi. Pour utiliser RTSP, la caméra et votre application doivent être sur le même réseau local. L'IP locale de la caméra peut être trouvée dans l'application EZVIZ sous **Device Info** ou dans la liste des clients DHCP de votre routeur.

### Option RTSP non disponible dans l'application

Certains modèles EZVIZ et versions de firmware n'exposent pas les paramètres RTSP. Dans ce cas :

1. Mettez à jour le firmware de la caméra via l'application EZVIZ
2. Si RTSP n'apparaît toujours pas, le modèle peut ne pas prendre en charge le streaming local
3. Les modèles à batterie et les sonnettes ne prennent généralement pas en charge RTSP

### Le format d'URL Hikvision fonctionne sur certains modèles

Comme les caméras EZVIZ utilisent un firmware Hikvision, certains modèles acceptent également le format d'URL Hikvision (`/Streaming/Channels/101`). Essayez ceci si l'URL EZVIZ standard ne fonctionne pas.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras EZVIZ ?**

L'URL standard est `rtsp://admin:VERIFICATION_CODE@CAMERA_IP:554/h264/ch1/main/av_stream`. Le VERIFICATION_CODE est le code à 6 caractères imprimé sur l'étiquette de votre caméra. RTSP doit d'abord être activé dans l'application EZVIZ.

**EZVIZ est-il lié à Hikvision ?**

Oui. EZVIZ est une marque détenue par Hikvision, axée sur le marché grand public de la maison connectée. Les caméras EZVIZ utilisent un firmware dérivé d'Hikvision, c'est pourquoi des modèles d'URL RTSP similaires fonctionnent. Cependant, les caméras EZVIZ sont conçues principalement pour une utilisation basée sur le cloud.

**Puis-je utiliser les caméras EZVIZ sans le cloud ?**

Partiellement. Vous pouvez accéder aux flux RTSP localement sans le cloud EZVIZ pour la visualisation en direct et l'enregistrement. Cependant, la configuration initiale de la caméra, les mises à jour de firmware et l'activation de RTSP nécessitent l'application EZVIZ (qui utilise le cloud). Des fonctionnalités comme les alertes de mouvement et le stockage de clips nécessitent un abonnement au cloud EZVIZ.

**Les caméras EZVIZ prennent-elles en charge ONVIF ?**

Certains modèles EZVIZ plus récents prennent en charge ONVIF, mais ce n'est pas disponible sur toutes les caméras. Vérifiez les spécifications de votre caméra ou les paramètres de l'application EZVIZ pour la prise en charge ONVIF. Pour la plupart des caméras EZVIZ, la connexion RTSP directe est plus fiable qu'ONVIF.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Hikvision](hikvision.md) — Société mère, format d'URL similaire
- [Guide de connexion Imou](imou.md) — Marque grand public Dahua, marché similaire
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation du SDK et exemples](index.md#get-started)
