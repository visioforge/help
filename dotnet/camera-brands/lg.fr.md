---
title: Caméras IP LG — URL RTSP et streaming en C# .NET VisioForge
description: Modèles d'URL RTSP des caméras LG SmartIP, LW et LV pour C# .NET. Diffusez les modèles sans fil et filaires avec l'intégration du VisioForge Video Capture SDK.
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
  - Webcam
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP LG en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**LG Electronics** est une société d'électronique multinationale sud-coréenne dont le siège social est à Séoul, en Corée du Sud. LG a produit des caméras IP sous la marque **SmartIP** et la **série LW/LV** pour le marché professionnel de la sécurité. LG a depuis largement quitté l'activité des caméras IP et a vendu sa division sécurité. Un nombre limité de caméras LG reste déployé dans des installations commerciales et d'entreprise.

**Faits clés :**

- **Gammes de produits :** LW Series (dôme/bullet sans fil), LV Series (filaire), SmartIP (entreprise)
- **Protocoles pris en charge :** RTSP, HTTP/CGI, ONVIF (série SmartIP), PSIA (modèles sélectionnés)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Oui (série SmartIP), limitée ou absente sur les séries LW/LV
- **Codecs vidéo :** H.264 (LW130W, LW332, série SmartIP), MJPEG (modèles plus anciens)

!!! warning "Gamme de produits abandonnée"
    LG a quitté le marché des caméras IP et vendu sa division sécurité. Aucune nouvelle mise à jour de firmware ni support officiel ne sont disponibles. De nombreuses entrées de bases de données étiquetées « LG » sont en réalité des smartphones LG utilisés comme caméras IP via des applications tierces — seuls les véritables modèles de caméras LG (LW, LV, SmartIP, 7210R) sont couverts ici.

## Modèles d'URL RTSP

### Formats d'URL standard

Les caméras LG utilisent plusieurs modèles d'URL RTSP différents selon la série du modèle :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video1+audio1
```

| Modèle d'URL | Description |
|--------------|-------------|
| `video1+audio1` | Vidéo H.264 avec audio (série LW, 7210R) |
| `/` (racine) | Flux racine (série LV) |
| `//Master-0` | Flux Master (alternative LW130W) |
| `camera.stm` | Flux caméra (LW332) |
| `live1.sdp` | Flux SDP en direct (alternative LW332) |
| URL canal PSIA | Streaming entreprise PSIA (SmartIP) |

### Modèles de caméras — Flux RTSP

| Modèle | Type | URL du flux principal | Notes |
|--------|------|-----------------------|-------|
| LW130W | Dôme sans fil | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| LW130W | Dôme sans fil | `rtsp://IP//Master-0` | Flux Master alternatif |
| LW332 | Bullet sans fil | `rtsp://IP:554/camera.stm` | Flux caméra |
| LW332 | Bullet sans fil | `rtsp://IP:554/live1.sdp` | Flux SDP alternatif |
| LVW700 | Dôme filaire | `rtsp://IP:554/` | Flux racine |
| LVW701 | Dôme filaire | `rtsp://IP:554/` | Flux racine |
| 7210R | Caméra IP | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| SmartIP | Entreprise | `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | Flux PSIA H.264 |

### Modèles par série

#### Série LW (caméras sans fil)

| Modèle | URL de streaming | Protocole |
|--------|------------------|-----------|
| LW130W | `video1+audio1` ou `//Master-0` | RTSP + HTTP |
| LW332 | `camera.stm` ou `live1.sdp` | RTSP + HTTP |

#### Série LV (caméras filaires)

| Modèle | URL de streaming | Protocole |
|--------|------------------|-----------|
| LVW700 | Flux racine (`rtsp://IP:554/`) | RTSP |
| LVW701 | Flux racine (`rtsp://IP:554/`) | RTSP |

#### Série SmartIP (caméras d'entreprise)

| Modèle | URL de streaming | Protocole |
|--------|------------------|-----------|
| Modèles SmartIP | URL canal PSIA | RTSP + PSIA + ONVIF |

#### Modèles autonomes

| Modèle | URL de streaming | Protocole |
|--------|------------------|-----------|
| 7210R | `video1+audio1` | RTSP |

### Formats d'URL alternatifs

| Modèle d'URL | Modèles | Notes |
|--------------|---------|-------|
| `rtsp://IP:554/video1+audio1` | LW130W, 7210R | Standard (recommandé pour ces modèles) |
| `rtsp://IP//Master-0` | LW130W | Alternatif ; notez la double barre oblique, sans port |
| `rtsp://IP:554/camera.stm` | LW332 | Standard pour LW332 |
| `rtsp://IP:554/live1.sdp` | LW332 | Format SDP alternatif |
| `rtsp://IP:554/` | LVW700, LVW701 | Flux racine (inhabituel mais valide) |
| `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | SmartIP | Streaming entreprise PSIA |

!!! tip "URL de flux racine"
    Les LVW700 et LVW701 utilisent une URL RTSP racine (`rtsp://IP:554/`) sans composant de chemin. Cela est inhabituel mais valide. Assurez-vous que votre client RTSP ne supprime pas la barre oblique finale et n'ajoute pas de chemin par défaut.

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra LG avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// LG LW130W, flux H.264 + audio
var uri = new Uri("rtsp://192.168.1.90:554/video1+audio1");
var username = "admin";
var password = "admin";
```

Pour les caméras LW332, utilisez `camera.stm` ou `live1.sdp` comme chemin de flux à la place.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Notes |
|------|--------------|-------|
| Snapshot CGI | `http://IP/snapshot.cgi` | Snapshot CGI standard |
| Snapshot JPEG | `http://IP/snapshot.jpg` | JPEG direct (LW130W) |
| Flux vidéo | `http://IP/videofeed` | Flux vidéo en direct |
| Flux MJPEG | `http://IP/video?submenu=mjpg` | Flux MJPEG continu |
| Vidéo de profil | `http://IP/video?profile=CHANNEL` | Sélection vidéo basée sur le profil |

## Dépannage

### Confusion entre caméras LG et smartphones LG

De nombreuses bases de données de caméras RTSP contiennent des entrées étiquetées « LG » qui sont en réalité des **smartphones LG** (P350, P509, P970, Nexus 4, Optimus V, LS670) exécutant des applications de caméra IP tierces comme « IP Webcam ». Ce ne sont pas de véritables caméras IP LG. Recherchez les numéros de modèle commençant par **LW**, **LV**, **SmartIP** ou **7210R** pour identifier les véritables caméras de sécurité LG.

### L'URL du flux racine ne se connecte pas

Les caméras LVW700 et LVW701 utilisent une URL racine nue (`rtsp://IP:554/`) sans chemin de flux. Certaines bibliothèques client RTSP peuvent ne pas gérer cela correctement. Si vous rencontrez des problèmes de connexion :

1. Assurez-vous que la barre oblique finale est incluse
2. Essayez de spécifier l'URL comme `rtsp://admin:admin@192.168.1.90:554/`
3. Vérifiez que la caméra répond sur le port 554 à l'aide d'un scanner réseau

### Plusieurs formats d'URL par modèle

Certaines caméras LG (notamment la LW130W et la LW332) prennent en charge plusieurs formats d'URL RTSP. Si un format échoue, essayez l'autre :

- **LW130W :** essayez `video1+audio1` en premier, puis `//Master-0`
- **LW332 :** essayez `camera.stm` en premier, puis `live1.sdp`

### Streaming PSIA sur les modèles SmartIP

Les caméras d'entreprise SmartIP prennent en charge le streaming PSIA (Physical Security Interoperability Alliance). Le format d'URL PSIA est :

```
rtsp://admin:admin@192.168.1.90:554/PSIA/Streaming/channels/2?videoCodecType=H.264
```

Modifiez le numéro de canal pour sélectionner différents flux. PSIA nécessite une authentification via l'URL ou HTTP digest.

### Aucune mise à jour de firmware disponible

LG a quitté le marché des caméras de sécurité. Aucun nouveau firmware, correctif ou canal de support officiel n'est disponible. Si vous rencontrez des bogues ou des vulnérabilités de sécurité, envisagez de remplacer la caméra par un modèle actuellement pris en charge.

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras IP LG ?**

Cela dépend de la série du modèle. Pour les caméras LW130W et 7210R, utilisez `rtsp://admin:admin@CAMERA_IP:554/video1+audio1`. Pour la LW332, utilisez `rtsp://admin:admin@CAMERA_IP:554/camera.stm`. Pour les LVW700/LVW701, utilisez `rtsp://admin:admin@CAMERA_IP:554/`. Chaque série de modèle a un modèle d'URL différent.

**Les caméras IP LG sont-elles toujours prises en charge ?**

Non. LG a vendu sa division sécurité et a quitté le marché des caméras IP. Aucune mise à jour de firmware, aucun nouveau modèle ni aucun support technique officiel ne sont disponibles. Les caméras existantes continuent à fonctionner mais ne recevront pas de correctifs de sécurité ni de mises à jour de fonctionnalités.

**Les caméras LG prennent-elles en charge ONVIF ?**

Seule la série d'entreprise SmartIP prend en charge ONVIF. Les caméras grand public des séries LW et LV ont un support ONVIF limité ou inexistant. Les caméras SmartIP prennent également en charge PSIA comme protocole d'interopérabilité alternatif.

**Pourquoi vois-je des modèles de téléphones LG dans les bases de données de caméras IP ?**

De nombreuses bases de données d'URL RTSP répertorient des modèles de smartphones LG (Nexus 4, Optimus V, P509, etc.) comme « caméras LG ». Ce sont en réalité des téléphones exécutant des applications tierces comme « IP Webcam » qui transforment le téléphone en caméra de sécurité de fortune. Ce ne sont pas de véritables produits de caméras IP LG et ils utilisent des modèles d'URL complètement différents déterminés par l'application.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Samsung](samsung.md) — caméras d'entreprise coréennes
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
