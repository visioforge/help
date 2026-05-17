---
title: Caméras IP Linksys — URL RTSP et configuration C# .NET
description: Connectez les caméras Linksys WVC, PVC et LCAB en C# .NET avec les modèles d'URL RTSP, flux ASF/MJPEG et exemples de code pour les modèles WVC abandonnés.
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
  - MP4
  - H.264
  - MJPEG
  - C#

---

# Comment se connecter à une caméra IP Linksys en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Linksys** est une société de réseautique américaine basée à Irvine, en Californie. Acquise à l'origine par Cisco Systems en 2003, la marque a été vendue à Belkin (une filiale de Foxconn) en 2013. Durant les années de propriété par Cisco, Linksys a produit la populaire série **WVC (Wireless Video Camera)** de caméras IP pour le marché grand public et prosommateur. La gamme de caméras a été abandonnée vers 2014, mais de nombreuses unités restent déployées et opérationnelles.

Puisque Linksys était une marque Cisco, ses caméras partagent des modèles d'URL et un firmware identiques à ceux des produits de caméras grand public Cisco. L'extension `.sav` dans les URL RTSP est un format d'endpoint propriétaire Cisco/Linksys.

**Faits clés :**

- **Gammes de produits :** WVC Series (WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC2300), PVC Series (PVC2300), LCAB Series
- **Protocoles pris en charge :** RTSP, HTTP/ASF, MJPEG, MMS (Windows Media)
- **Port RTSP par défaut :** 554
- **Identifiants par défaut :** admin / admin
- **Prise en charge ONVIF :** Limitée (série LCAB uniquement)
- **Codecs vidéo :** MPEG-4/ASF (série WVC), H.264 (modèles plus récents), MJPEG

!!! warning "Gamme de produits abandonnée"
    Les caméras IP Linksys ont été abandonnées vers 2014. Aucune nouvelle mise à jour de firmware ni support officiel n'est disponible. Les informations de cette page sont fournies pour les déploiements existants. De nombreux modèles WVC nécessitent Internet Explorer avec ActiveX pour leur interface web.

!!! info "Linksys = caméras grand public Cisco"
    Les caméras Linksys utilisent les mêmes modèles d'URL que les caméras grand public Cisco car Linksys était une marque Cisco. Consultez notre [guide de connexion Cisco](cisco.md) pour des détails supplémentaires et les modèles de caméras Cisco d'entreprise.

## Modèles d'URL RTSP

### Format d'URL standard

La plupart des caméras Linksys utilisent le chemin RTSP Cisco `/img/media.sav` :

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/img/media.sav
```

!!! note "Extension `.sav` inhabituelle"
    L'extension `.sav` est un endpoint RTSP propriétaire Cisco/Linksys — ce n'est pas un format de fichier multimédia standard. Ne la confondez pas avec une URL de téléchargement de fichier.

### URL RTSP par modèle

| Modèle | URL RTSP | Codec | Notes |
|--------|----------|-------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G, 640x480 |
| WVC80N | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-N, 640x480 |
| WVC80N (alt) | `rtsp://IP:554/img/video.sav` | MPEG-4 | Endpoint vidéo alternatif |
| WVC210 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G PTZ |
| WVC200 | `rtsp://IP:554/img/media.sav` | MPEG-4 | Wireless-G |
| PVC2300 | `rtsp://IP:554/video.mp4` | MPEG-4/H.264 | Caméra box pour petites entreprises |
| LCAB03VLNOD | `rtsp://IP:554//ONVIF/channel2` | H.264 | Caméra extérieure compatible ONVIF |

### Récapitulatif par famille de modèles

| Famille de modèles | Flux RTSP | ASF HTTP | MJPEG | Snapshot CGI |
|--------------------|-----------|----------|-------|--------------|
| WVC54G / WVC54GC / WVC54GCA | `/img/media.sav` | Oui | Oui | Oui |
| WVC80N | `/img/media.sav`, `/img/video.sav` | Oui | Oui | -- |
| WVC200 / WVC210 | `/img/media.sav` | Oui | Oui | Oui |
| WVC2300 | `/img/media.sav` | Oui | -- | -- |
| PVC2300 | `/video.mp4` | Oui | -- | -- |
| Série LCAB | `//ONVIF/channel2` | -- | -- | -- |

## Connexion avec le SDK VisioForge

Utilisez l'URL RTSP de votre caméra Linksys avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Linksys WVC80N, flux RTSP principal
var uri = new Uri("rtsp://192.168.1.60:554/img/media.sav");
var username = "admin";
var password = "admin";
```

Pour les caméras PVC2300, utilisez `/video.mp4` au lieu de `/img/media.sav`.

## URL des snapshots et MJPEG

| Type | Modèle d'URL | Modèles compatibles |
|------|--------------|---------------------|
| Flux vidéo ASF | `http://IP/img/video.asf` | WVC54G, WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210, WVC11b |
| Flux MJPEG | `http://IP/img/video.mjpeg` | WVC54GC, WVC54GCA, WVC80N, WVC200, WVC210 |
| Image MJPEG unique | `http://IP/img/mjpeg.jpg` | La plupart des modèles WVC |
| MJPEG CGI | `http://IP/img/mjpeg.cgi` | La plupart des modèles WVC |
| MJPEG (majuscules) | `http://IP/MJPEG.CGI` | Certains modèles WVC |
| Snapshot haute résolution | `http://IP/img/snapshot.cgi?size=3` | WVC54GCA, WVC200, WVC210 |
| Snapshot moyen | `http://IP/img/snapshot.cgi?size=2` | WVC54GCA, WVC200, WVC210 |
| Snapshot VGA | `http://IP/img/snapshot.cgi?img=vga` | WVC54GCA, WVC200, WVC210 |
| ASF alternatif | `http://IP/videostream.asf` | WVC54GC, WVC80N |
| Flux MMS | `mms://IP/img/video.asf` | Windows Media hérité (tous les modèles WVC) |

!!! tip "Flux HTTP comme solution de repli RTSP"
    De nombreuses caméras Linksys WVC fonctionnent plus de manière plus fiable avec les flux ASF ou MJPEG basés sur HTTP qu'avec RTSP. Si l'URL RTSP ne répond pas, essayez le flux ASF sur `http://IP/img/video.asf` comme solution de repli.

## Dépannage

### Le flux RTSP ne se connecte pas

Les caméras Linksys WVC ont une prise en charge RTSP limitée. De nombreux modèles diffusent principalement la vidéo via HTTP en utilisant ASF (Advanced Streaming Format) plutôt qu'un véritable RTSP :

1. Vérifiez l'adresse IP de la caméra et que le port 554 est ouvert
2. Confirmez que RTSP est activé dans l'interface web de la caméra
3. Essayez le flux ASF HTTP (`http://IP/img/video.asf`) comme alternative
4. Certains modèles nécessitent que l'interface web soit accédée d'abord (via Internet Explorer avec ActiveX) avant que RTSP ne devienne disponible

### Le flux ASF nécessite un traitement spécifique

Les flux ASF (Advanced Streaming Format) des caméras WVC utilisent un conteneur propriétaire Microsoft. Le SDK VisioForge gère les flux ASF automatiquement. Si vous rencontrez des problèmes :

- Assurez-vous de vous connecter via HTTP, pas RTSP, pour les URL ASF
- Les flux ASF peuvent nécessiter des composants Windows Media ou des filtres LAV sur certains systèmes

### Flux du protocole MMS

Les URL du protocole `mms://` sont spécifiques à Windows Media et ne fonctionnent qu'avec Windows Media Player ou des décodeurs compatibles. Pour les applications modernes, utilisez l'URL ASF HTTP (`http://IP/img/video.asf`) au lieu de l'équivalent MMS.

### L'interface web nécessite Internet Explorer

De nombreux modèles WVC nécessitent Internet Explorer avec des contrôles ActiveX pour leur interface de configuration web. Utilisez Internet Explorer ou un navigateur compatible ActiveX pour accéder aux paramètres de la caméra. Les flux RTSP et HTTP eux-mêmes fonctionnent avec n'importe quel client.

### La caméra n'est pas détectable sur le réseau

Les caméras Linksys ne prennent pas en charge les protocoles de découverte modernes (sauf la série LCAB avec ONVIF). Pour trouver la caméra :

1. Vérifiez la table de baux DHCP de votre routeur pour l'adresse IP de la caméra
2. Utilisez l'utilitaire Linksys Camera Utility (si encore disponible) pour la découverte
3. Essayez l'adresse IP par défaut attribuée par la caméra (consultez le manuel du modèle)
4. Utilisez un scanner réseau tel qu'Advanced IP Scanner

## FAQ

**Quelle est l'URL RTSP par défaut pour les caméras Linksys ?**

Pour la plupart des caméras Linksys WVC, utilisez `rtsp://admin:admin@CAMERA_IP:554/img/media.sav`. Pour la PVC2300, utilisez `rtsp://admin:admin@CAMERA_IP:554/video.mp4` à la place. Si RTSP ne fonctionne pas, essayez le flux ASF HTTP sur `http://CAMERA_IP/img/video.asf`.

**Les caméras Linksys sont-elles toujours disponibles à l'achat ?**

Non. Linksys a abandonné toute sa gamme de produits de caméras IP vers 2014, peu après que la marque a été vendue par Cisco à Belkin/Foxconn. Aucune mise à jour de firmware ni support officiel n'est disponible. Cependant, de nombreuses caméras WVC et PVC restent en service et fonctionnelles.

**Les caméras Linksys prennent-elles en charge ONVIF ?**

Seules les caméras de la série LCAB prennent en charge ONVIF. Les séries WVC et PVC ne prennent pas en charge ONVIF. Pour les caméras WVC, utilisez les modèles d'URL RTSP ou HTTP directs listés ci-dessus.

**Les URL des caméras Linksys et Cisco sont-elles les mêmes ?**

Oui. Les caméras Linksys ont été produites pendant la propriété de Cisco de la marque et partagent le même firmware et les mêmes modèles d'URL que les caméras grand public Cisco. Le chemin RTSP `/img/media.sav` et le chemin HTTP `/img/video.asf` sont identiques sur les deux marques. Consultez notre [guide de connexion Cisco](cisco.md) pour plus de détails.

## Ressources associées

- [Toutes les marques de caméras — répertoire des URL RTSP](index.md)
- [Guide de connexion Cisco](cisco.md) — Mêmes modèles d'URL, société mère
- [Tutoriel d'aperçu de caméra IP](../videocapture/video-tutorials/ip-camera-preview.md)
- [Installation et exemples du SDK](index.md#get-started)
