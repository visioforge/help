---
title: URL RTSP Rhombus — intégration API cloud en C# .NET SDK
description: Options d'intégration des caméras Rhombus en C# .NET. Architecture cloud, accès API et approches alternatives pour les caméras Rhombus Systems.
meta:
  - name: robots
    content: "noindex, follow"
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

# Comment se connecter à une caméra Rhombus en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Rhombus Systems** (Rhombus, Inc.) est une société américaine de sécurité vidéo gérée dans le cloud dont le siège est à Sacramento, en Californie. Fondée en 2016, Rhombus fournit des caméras, capteurs et contrôle d'accès pour les entreprises avec une plateforme de gestion cloud-first. Similaire à Verkada, les caméras Rhombus sont gérées via une console cloud centralisée.

**Faits clés :**

- **Gammes de produits :** série R (dôme), série R Pro (avancée), série R Mini (compacte)
- **Architecture :** Gérée dans le cloud avec traitement IA embarqué
- **Prise en charge RTSP :** Limitée — disponible sur certains modèles via configuration LAN
- **Prise en charge ONVIF :** Non
- **Codecs vidéo :** H.264, H.265
- **Accès API :** API Rhombus (REST, nécessite un abonnement)
- **Stockage embarqué :** Oui (carte SD locale pour le stockage en périphérie)

!!! info "Streaming local limité"
    Certains modèles de caméras Rhombus prennent en charge RTSP pour le streaming sur le LAN local, mais cette fonctionnalité doit être activée via la console Rhombus et n'est pas disponible sur tous les modèles ou niveaux d'abonnement. Vérifiez les paramètres de votre compte Rhombus pour la disponibilité de RTSP.

## Accès RTSP (lorsque disponible)

### Activer RTSP

Sur les caméras Rhombus prises en charge :

1. Connectez-vous à la **console Rhombus** (console.rhombus.com)
2. Accédez aux paramètres de votre caméra
3. Recherchez les paramètres **Local Streaming** ou **RTSP**
4. Activez RTSP et notez l'URL fournie

### Format d'URL RTSP

Lorsque RTSP est disponible :

```
rtsp://[IP]:554/live
```

Le format exact de l'URL et la méthode d'authentification dépendent du modèle de caméra et de la version du firmware. La console Rhombus fournira l'URL spécifique lorsque RTSP est activé.

## Connexion avec le SDK VisioForge

Si RTSP est disponible sur votre caméra Rhombus, utilisez l'URL avec l'une des trois approches du SDK présentées dans le [guide de démarrage rapide](index.md#quick-start-code) :

```csharp
// Caméra Rhombus avec RTSP activé
var uri = new Uri("rtsp://192.168.1.90:554/live");
var username = "admin";
var password = "YourPassword"; // depuis la console Rhombus
```

## Intégration via l'API Rhombus

Pour les caméras sans accès RTSP, Rhombus fournit une API REST qui offre :

- Liste et statut des caméras
- Export et téléchargement de clips vidéo
- Récupération de captures/miniatures
- Données d'événements et d'analyses
- Notifications webhook

L'API ne fournit pas de flux RTSP en temps réel. Elle est conçue pour la récupération de clips, l'accès aux métadonnées et les flux d'automatisation.

## Dépannage

### RTSP indisponible sur ma caméra

Toutes les caméras Rhombus ou tous les niveaux d'abonnement ne prennent pas en charge RTSP. Contactez le support Rhombus pour vérifier la disponibilité de RTSP pour votre modèle de caméra spécifique et votre forfait.

### Le flux RTSP se déconnecte

Les caméras Rhombus privilégient la connectivité cloud. Si le flux RTSP local est instable :

1. Assurez-vous que la caméra dispose d'une bande passante réseau suffisante pour les flux cloud et locaux
2. Utilisez le sous-flux pour des exigences de bande passante plus faibles
3. Vérifiez la console Rhombus pour les mises à jour de firmware

## FAQ

**Les caméras Rhombus prennent-elles en charge RTSP ?**

Certaines caméras Rhombus prennent en charge RTSP pour le streaming sur le LAN local, mais cela doit être activé via la console Rhombus et peut ne pas être disponible sur tous les modèles ou niveaux d'abonnement. Contactez le support Rhombus pour les spécificités.

**Puis-je utiliser le SDK VisioForge avec les caméras Rhombus ?**

Si votre caméra Rhombus a RTSP activé, oui. Utilisez l'URL RTSP de la console Rhombus avec la source RTSP du SDK VisioForge. Pour les caméras sans RTSP, vous devrez utiliser séparément l'API REST Rhombus pour l'accès aux clips et aux captures.

**Comment Rhombus se compare-t-il à Verkada ?**

Les deux sont des plateformes gérées dans le cloud. Rhombus propose RTSP sur certains modèles, tandis que Verkada ne prend pas du tout en charge RTSP. Les deux fournissent des API REST pour l'accès aux clips/captures. Consultez notre [guide Verkada](verkada.md) pour comparaison.

**Quelles sont les bonnes alternatives à Rhombus avec une prise en charge RTSP complète ?**

Pour des caméras d'entreprise avec prise en charge RTSP et ONVIF native, considérez [Axis](axis.md), [Bosch](bosch.md), [Hanwha Vision](hanwha.md) ou [Avigilon](avigilon.md).

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Verkada](verkada.md) — Une autre plateforme gérée dans le cloud
- [Guide de connexion Axis](axis.md) — Alternative d'entreprise avec RTSP complet
- [Guide de connexion Hanwha Vision](hanwha.md) — Alternative d'entreprise avec RTSP
- [Installation du SDK et exemples](index.md#get-started)
