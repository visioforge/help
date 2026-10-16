---
title: URL RTSP Verkada et connexion des caméras en C# .NET
description: Format d'URL RTSP Verkada rtsp://IP:8554/standard pour C# .NET. Activez RTSP dans Verkada Command et connectez-vous depuis Media Blocks SDK .NET.
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
  - C#
primary_api_classes:
  - RTSPSourceSettings

---

# Comment se connecter à une caméra Verkada en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation de la marque

**Verkada** est une société américaine de caméras de sécurité gérées dans le cloud dont le siège est à San Mateo, en Californie. Fondée en 2016, Verkada propose des caméras d'entreprise gérées via la plateforme Verkada Command. En plus de cette gestion dans le cloud, une caméra peut diffuser des flux RTSP directement sur le réseau local dès que RTSP est activé pour elle — les caméras CR de la série distante font exception.

**Faits clés :**

- **Gammes de produits :** CD (dôme), CB (bullet), CF (fisheye), CM (mini dôme), CH et CY (multi-capteurs), CP (PTZ), CR (distante) ; le suffixe `-E` désigne une version extérieure
- **Architecture :** Gérée dans le cloud, avec streaming RTSP local
- **Prise en charge RTSP :** Oui — activée caméra par caméra dans Command
- **Prise en charge ONVIF :** Non
- **Port RTSP :** 8554
- **Chemins de flux :** `/standard` et `/high`
- **Accès réseau local :** RTSP uniquement, sur des adresses privées RFC 1918
- **Codecs vidéo :** H.264, H.265 — choisis caméra par caméra dans Command
- **Accès API :** API Verkada (basée sur le cloud ; un administrateur de l'organisation émet les clés dans Command)

## Activer RTSP

RTSP reste désactivé jusqu'à ce qu'un administrateur du site l'active pour une caméra donnée : **All Products → Cameras → sélectionnez la caméra → Settings → Device → Real Time Streaming Protocol (RTSP)**. Définissez un nom d'utilisateur et un mot de passe, puis cliquez sur **Enable**. Le mot de passe n'est affiché qu'une fois : conservez-le. Command affiche ensuite l'URL RTSP de la caméra avec un espace réservé `[PASSWORD]` : c'est à vous de le remplacer par le mot de passe que vous venez de définir, car le mot de passe ne peut plus être relu dans Command. Command peut aussi exporter un CSV de toutes les caméras de l'organisation dont RTSP est activé, avec leur URL.

Aucune licence RTSP distincte n'est requise : la règle habituelle d'une licence par caméra s'applique.

## Format de l'URL RTSP

```
rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[STREAM]
```

| Flux | Chemin | Remarques |
|--------|------|-------|
| Standard | `/standard` | Débit plus faible |
| Haute qualité | `/high` | Qualité complète |

La résolution et le débit suivent les flux natifs de la caméra et ne peuvent pas être modifiés pour RTSP.

Les caméras multi-capteurs (séries CH et CY) placent l'id de la caméra avant le segment de flux, `rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/[CAMERA-ID]/high`, où l'id est celui que Command affiche pour ce capteur.

## Connexion avec le SDK VisioForge

Placez l'adresse et le chemin du flux dans l'URI, et passez les identifiants de Command comme `login` et `password` :

```csharp
// Caméra Verkada avec RTSP activé, flux haute qualité
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:8554/high"),
    login: "rtsp-user",
    password: "YourRtspPassword",
    audioEnabled: true);
```

Une URI contenant le mot de passe (`rtsp://rtsp-user:YourRtspPassword@192.168.1.100:8554/high`) fonctionne également, car `rtspsrc` s'authentifie avec les identifiants présents dans l'URL. Il reste préférable de les passer séparément : l'URL que Command affiche contient un espace réservé `[PASSWORD]` à renseigner, et une fois renseignée elle circule dans les journaux, les fichiers de configuration et les captures d'écran, où le mot de passe n'a rien à faire.

## Limites

- **Réseau local uniquement.** Le flux provient directement de la caméra sur des adresses privées et n'est pas relayé par Verkada Cloud : un client situé sur un autre réseau a besoin d'un VPN vers ce réseau.
- **Non chiffré.** Verkada ne propose pas RTSPS. Leur propre recommandation est de garder le réseau des caméras privé et de limiter les clients autorisés à y accéder.
- **Le codec appartient à la caméra.** Le réglage **Settings → Device → Video Codec** de la caméra décide si le flux RTSP est en H.264 ou H.265, et H.265 n'est pas activé automatiquement pour RTSP. Il n'existe pas de repli vers H.264 : un client incapable de décoder H.265 ne voit rien tant que la caméra n'est pas configurée en H.264.
- **L'audio est désactivé tant qu'il n'est pas activé.** L'audio de la caméra est désactivé par défaut ; un administrateur du site l'active caméra par caméra dans **Settings → Device → Camera Audio**, ce qui redémarre cette caméra. Tant qu'il est désactivé, le flux RTSP ne comporte aucune piste audio et `CreateAsync` renvoie des réglages avec `AudioEnabled` désactivé.
- **Certains modèles n'ont pas d'audio en RTSP pour autant.** Les CD22, CD22-E, CM22, CF81-E, CF83-E, toute la série CB (Bullet), CH52-E, CP52-E et CP63-E : passez `audioEnabled: false` pour ceux-là. Parmi les modèles qui diffusent effectivement en RTSP, les autres comportent une piste audio.
- **Pas de RTSP sur les CR63-E et CR63-TE.**

## API Verkada

L'API REST de Verkada couvre la liste et l'état des caméras, l'export et le téléchargement de clips, les miniatures, ainsi que les données d'événements et d'alertes, et elle diffuse aussi la vidéo en direct — avec une clé à laquelle un accès streaming a été accordé. Un administrateur de l'organisation émet les clés d'API dans **Command → Admin → API & Integrations**, où leurs permissions sont également définies. Utilisez l'API lorsque la vidéo doit venir de l'extérieur du réseau de la caméra ; le flux RTSP est la voie locale à faible latence.

## Caméras d'entreprise avec RTSP et ONVIF

Verkada diffuse désormais en RTSP, mais ne propose toujours pas ONVIF : la découverte automatique et la sélection de profils restent indisponibles. Lorsque cela compte, ces marques offrent les deux :

| Alternative | Segment de marché | RTSP | ONVIF | Guide |
|------------|---------------|------|-------|-------|
| Axis | Entreprise | Oui | Oui | [Guide de connexion](axis.md) |
| Bosch | Entreprise | Oui | Oui | [Guide de connexion](bosch.md) |
| Hanwha Vision | Entreprise | Oui | Oui | [Guide de connexion](hanwha.md) |
| Avigilon | Entreprise | Oui | Oui | [Guide de connexion](avigilon.md) |
| Hikvision | Entreprise | Oui | Oui | [Guide de connexion](hikvision.md) |

## FAQ

**Quelle est l'URL RTSP d'une caméra Verkada ?**

`rtsp://[USERNAME]:[PASSWORD]@[CAMERA-IP]:8554/high`, où `CAMERA-IP` est l'adresse privée de la caméra et `[USERNAME]` et `[PASSWORD]` sont les identifiants définis pour cette caméra dans Command. Utilisez `/standard` à la place de `/high` pour le flux à débit plus faible. Les caméras multi-capteurs insèrent l'id de la caméra avant le segment de flux.

**Les caméras Verkada prennent-elles en charge RTSP ?**

Oui. Verkada a annoncé le RTSP à faible latence pour ses caméras, diffusé sur le réseau local. Il est désactivé par défaut et un administrateur du site doit l'activer caméra par caméra dans Command.

**Les caméras Verkada prennent-elles en charge ONVIF ?**

Non. Il n'existe pas de point de terminaison ONVIF : la découverte ONVIF et la sélection de profils ne s'appliquent pas. Utilisez l'URL RTSP fournie par Command.

**Puis-je accéder à un flux RTSP Verkada depuis l'extérieur du réseau local ?**

Pas directement. Verkada ne diffuse RTSP que sur des adresses privées et ne le relaie pas via le cloud : un client sur un autre réseau a besoin d'un VPN.

**Le flux RTSP Verkada inclut-il l'audio ?**

Pas avant qu'un administrateur du site n'active **Camera Audio** pour cette caméra dans Command (**Settings → Device → Camera Audio**, ce qui redémarre la caméra) : jusque-là, aucune piste audio n'est présente. Et pas sur les modèles dont le flux RTSP ne comporte pas d'audio : CD22, CD22-E, CM22, CF81-E, CF83-E, la série CB, CH52-E, CP52-E et CP63-E. Parmi les modèles qui diffusent en RTSP, les autres comportent une piste audio : `audioEnabled: true` s'applique.

## Ressources connexes

- [Toutes les marques de caméras — Annuaire des URL RTSP](index.md)
- [Guide de connexion Rhombus](rhombus.md) — Une autre plateforme gérée dans le cloud avec RTSP
- [Guide de connexion Axis](axis.md) — Alternative d'entreprise avec RTSP et ONVIF
- [Installation du SDK et exemples](index.md#get-started)
