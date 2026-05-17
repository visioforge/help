---
title: "Sources vidéo : webcam, caméra IP, écran en C# .NET"
description: Configurez webcam, caméra IP, capture d'écran, Decklink et caméras industrielles avec le SDK VisioForge. Énumération des périphériques et exemples C#.
sidebar_label: Sources vidéo
order: 16
tags:
  - Video Capture SDK
  - .NET
  - Streaming

---

# Sources vidéo pour les développeurs .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction aux sources d'entrée vidéo

Le Video Capture SDK pour .NET offre une prise en charge robuste de pratiquement toutes les sources d'entrée vidéo standards disponibles dans les environnements de développement modernes. Cette flexibilité permet aux développeurs de créer des applications capables de capturer, traiter et manipuler du contenu vidéo provenant d'une grande variété de périphériques matériels et de flux réseau.

Que vous développiez un logiciel d'édition vidéo professionnel, créiez des solutions de surveillance personnalisées ou construisiez des applications d'imagerie médicale, la compréhension des options de sources vidéo disponibles est cruciale pour implémenter la bonne solution selon vos besoins spécifiques.

## Démarrage rapide — Choisir et assigner une source vidéo

Chaque source se branche sur `VideoCaptureCoreX.Video_Source` via une classe de paramètres. Le modèle est le même pour tous les types de sources — seule la classe de paramètres change :

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

VisioForgeX.InitSDK();
var videoCapture = new VideoCaptureCoreX(videoView as IVideoView);

// Choisir UNE des sources suivantes :

// --- Webcam / caméra USB / caméra virtuelle
var camera = (await DeviceEnumerator.Shared.VideoSourcesAsync()).First();
videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(camera);

// --- Capture d'écran (Direct3D 11 sur Windows)
videoCapture.Video_Source = new ScreenCaptureD3D11SourceSettings
{
    FrameRate = new VideoFrameRate(30),
    CaptureCursor = true
};

// --- Blackmagic Decklink
var dl = (await DeviceEnumerator.Shared.DecklinkVideoSourcesAsync()).First();
videoCapture.Video_Source = new DecklinkVideoSourceSettings(dl);

// --- Caméra IP / RTSP (utiliser la fabrique asynchrone — le ctor est privé)
videoCapture.Video_Source = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:554/stream"),
    login: "admin", password: "password", audioEnabled: true);

// --- NDI (multiplateforme, nécessite le runtime NDI)
var ndi = (await DeviceEnumerator.Shared.NDISourcesAsync()).First();
videoCapture.Video_Source = await NDISourceSettings.CreateAsync(videoCapture.GetContext(), ndi);

// Puis ajouter une sortie et démarrer comme d'habitude.
videoCapture.Outputs_Add(new MP4Output("output.mp4"), true);
await videoCapture.StartAsync();
```

## Webcams USB et intégrées

### Compatibilité des périphériques

Le SDK prend en charge tous les périphériques de capture vidéo standards conformes aux interfaces de pilote courantes, notamment :

- Webcams USB (périphériques connectés en USB 2.0, 3.0 et USB-C)
- Caméras intégrées d'ordinateurs portables et de tablettes
- Adaptateurs et dongles USB de capture vidéo externes
- Périphériques logiciels de caméras virtuelles

### Fonctionnalités d'implémentation

Lorsqu'ils travaillent avec des caméras USB et intégrées, les développeurs peuvent :

- Accéder et énumérer tous les périphériques connectés
- Sélectionner parmi les formats vidéo et résolutions disponibles
- Contrôler des paramètres spécifiques à la caméra (mise au point, exposition, balance des blancs)
- Appliquer des filtres de traitement vidéo en temps réel
- Capturer des images brutes pour une analyse d'image personnalisée
- Configurer la luminosité, le contraste et les paramètres de couleur de manière programmatique

## Matériel professionnel Blackmagic Decklink

### Modèles et fonctionnalités pris en charge

Le SDK fournit une intégration native avec le matériel professionnel de capture vidéo Decklink de Blackmagic Design :

- Prise en charge complète de toutes les gammes de produits Decklink :
  - Série Decklink Mini (capture compacte et économique)
  - Modèles Decklink Studio (fonctionnalités de diffusion de milieu de gamme)
  - Série Decklink 4K et 8K (production haute résolution)
  - Variantes Decklink Duo et Quad (capture multi-entrées)

### Capacités techniques

- Prise en charge des connexions d'entrée SDI (Serial Digital Interface) et HDMI
- Compatible avec les résolutions standards de diffusion :
  - SD (PAL/NTSC)
  - HD (720p, 1080i, 1080p)
  - UHD/4K (2160p)
  - 8K lorsque le matériel le prend en charge
- Accès à tous les canaux audio embarqués (jusqu'à 16 canaux)
- Interprétation et synchronisation de timecode
- Contrôle du tampon d'image pour des performances de capture constantes
- Accès aux métadonnées vidéo et aux données auxiliaires

## Sources vidéo réseau

### Prise en charge des caméras IP et des flux

Le SDK permet aux applications de se connecter directement à des sources vidéo en réseau :

- Flux RTSP (Real-Time Streaming Protocol) avec différentes options de transport :
  - Transport UDP (faible latence, potentiellement moins fiable)
  - Transport TCP (fiable, latence potentiellement plus élevée)
  - Tunnellisation HTTP pour la traversée de pare-feu
- Sources RTMP (Real-Time Messaging Protocol) :
  - Prise en charge des flux en direct
  - Compatibilité avec les serveurs RTMP
  - Compatibilité Flash Media Server
- Streaming basé HTTP :
  - Flux MJPEG
  - Sources de téléchargement progressif HTTP
- Formats de streaming standards de l'industrie :
  - HLS (HTTP Live Streaming)
  - DASH (Dynamic Adaptive Streaming over HTTP)
  - SRT (Secure Reliable Transport)
- Intégration WebRTC pour la communication vidéo basée sur le navigateur

### Détails d'implémentation

- Prise en charge de l'authentification pour les flux sécurisés (Basic, Digest, NTLM)
- Paramètres de tampon configurables pour équilibrer latence et fluidité
- Logique de reconnexion automatique pour les conditions réseau instables
- Techniques de traversée NAT pour les environnements réseau complexes
- Statistiques de trafic pour la surveillance de la consommation de bande passante
- Adaptation multi-débit pour les conditions réseau variables

## Capture d'écran et de fenêtre

### Options de capture du bureau

Pour les scénarios nécessitant l'enregistrement d'écran ou la capture d'application :

- Capture de contenu plein écran avec prise en charge de :
  - Configurations à un seul moniteur
  - Configurations multi-moniteurs avec affichages cibles sélectionnables
  - Diverses options de mise à l'échelle pour optimiser les performances
- Capacités de capture de fenêtre spécifique :
  - Capture par handle de fenêtre
  - Ciblage spécifique à l'application
  - Capture de fenêtres sans bordure
- Sélection d'une région d'intérêt :
  - Sélection rectangulaire personnalisée
  - Suivi dynamique de région
  - Positionnement basé sur les coordonnées

### Implémentation technique

- Capture accélérée matériellement lorsque disponible
- Options d'inclusion/exclusion du curseur
- Contrôle de la fréquence d'images pour équilibrer qualité et charge système
- Options de visualisation des clics de souris pour les enregistrements de tutoriels
- Compatibilité avec le contenu DirectX/OpenGL
- Gestion des fenêtres en couches pour les compositions de bureau complexes

## Périphériques hérités et spécialisés

### Intégration de caméscope DV

Le SDK maintient la prise en charge des caméras au format Digital Video (DV) :

- Prise en charge de la connectivité FireWire/IEEE 1394
- Compatibilité avec les formats standards DV, DVCAM et HDV
- Capture image par image avec préservation du timecode
- Fonctionnalités de contrôle des périphériques (lorsque le matériel le permet) :
  - Contrôles lecture/pause/arrêt
  - Fonctions d'avance rapide et de retour
  - Démarrage de l'enregistrement

### Caméras industrielles et scientifiques

Pour les scénarios de développement spécialisés, le SDK prend en charge les caméras de vision industrielle via plusieurs normes :

- Périphériques conformes à USB3 Vision offrant :
  - Acquisition d'images à haute vitesse
  - Découverte et énumération des fonctionnalités des périphériques
  - Gestion des événements pour la capture déclenchée
- Matériel compatible GigE Vision avec :
  - Protocoles de découverte réseau
  - Diffusion d'images à large bande passante
  - Accès à la configuration des périphériques
- Prise en charge de l'interface standard GenICam :
  - Conventions de nommage standardisées des paramètres
  - Cohérence d'accès aux fonctionnalités entre fabricants
  - Configuration des périphériques basée sur descripteurs XML
- Contrôle des paramètres de caméra spécialisés :
  - Ajustements d'exposition et de gain
  - Options de déclenchement (logiciel/matériel)
  - Définition d'une région d'intérêt (ROI)
  - Divers formats de pixel et profondeurs en bits

## Techniques d'optimisation des performances

Lorsque vous travaillez avec des sources vidéo, les considérations de performance sont critiques. Le SDK fournit plusieurs voies d'optimisation :

- Options d'accélération matérielle :
  - Traitement accéléré par DirectX
  - Encodage/décodage basé GPU
  - Utilisation d'instructions SIMD
- Stratégies de gestion de la mémoire :
  - Mise en commun de tampons pour réduire la surcharge d'allocation
  - Accès direct à la mémoire lorsqu'il est pris en charge
  - Tampons d'image préalloués
- Traitement multithread :
  - Traitement parallèle des images vidéo
  - Utilisation de pools de threads pour les chaînes de filtres
  - Traitement en arrière-plan pour les scénarios non temps réel

## Conclusion

La prise en charge étendue des sources vidéo dans le Video Capture SDK .NET permet aux développeurs de créer des applications vidéo polyvalentes avec un minimum de contraintes sur le matériel d'entrée. En comprenant les capacités et les limites de chaque type de source, vous pouvez concevoir des solutions de traitement vidéo plus efficaces et performantes.

Pour les références API détaillées et les exemples d'implémentation pour des types de sources vidéo spécifiques, consultez la documentation des classes et les guides de méthodes dans les ressources de référence du SDK.

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir plus d'exemples de code.
