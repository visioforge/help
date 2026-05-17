---
title: Visionneuse RTSP et lecteur de caméra IP en C# .NET
description: Visionneuse RTSP et lecteur de caméra IP en C# avec VisioForge Media Blocks — aperçu, découverte ONVIF, enregistrement passthrough.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
  - Recording
  - Encoding
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - UDP
  - MP4
  - TS
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - RTSPSourceSettings
  - RTSPSourceBlock
  - MediaBlocksPipeline
  - VideoRendererBlock
  - RTSPRAWSourceBlock

---

# Visionneuse RTSP et lecteur de caméra IP en C#

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous construisez avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP VisioForge](../../general/mcp-server-usage.md) public à
    `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis
    que de grepper `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction

Ce guide vous accompagne dans la construction d'une visionneuse de flux RTSP et d'une application de lecture de caméra IP en C# avec le VisioForge Media Blocks SDK. Vous apprendrez à vous connecter aux caméras IP via RTSP, afficher la vidéo en direct avec audio, découvrir les caméras via ONVIF et enregistrer les flux dans un fichier — avec ou sans réencodage. Le Media Blocks SDK fonctionne sous Windows, macOS et Linux, le même code fonctionne donc sur toutes les plateformes.

Les cas d'usage courants incluent les tableaux de bord de vidéosurveillance, les applications NVR (Network Video Recorder), les outils de gestion de caméras et tout projet devant afficher ou enregistrer programmatiquement des flux de caméras IP.

!!!info Exemples de démo
    Pour des exemples complets et fonctionnels, consultez les [démos RTSP sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) : RTSP Preview Demo, RTSP RAW Capture Demo et RTSP MultiView Demo.
!!!

## Prérequis

Ajoutez le paquet NuGet du Media Blocks SDK à votre projet :

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Vous avez également besoin des paquets de runtime spécifiques à la plateforme. Pour Windows :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Pour les autres plateformes (macOS, Linux, Android, iOS), consultez le [guide de déploiement](../../deployment-x/index.md).

## Aperçu RTSP en direct

Le schéma central connecte une source RTSP à des moteurs de rendu vidéo et audio via un `MediaBlocksPipeline`. Initialisez le SDK une fois au démarrage de l'application, puis créez les pipelines selon les besoins.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// Initialiser le SDK une fois au démarrage
await VisioForgeX.InitSDKAsync();

// Créer le pipeline
var pipeline = new MediaBlocksPipeline();

// Se connecter à la caméra RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/Streaming/Channels/101"),
    "admin",          // identifiant
    "password",       // mot de passe
    true);            // audio activé

var rtspSource = new RTSPSourceBlock(rtspSettings);

// Créer le moteur de rendu vidéo (VideoView1 est un contrôle IVideoView sur votre formulaire)
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

// Créer le moteur de rendu audio (facultatif)
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

// Démarrer la lecture
await pipeline.StartAsync();
```

Pour arrêter la lecture et libérer les ressources :

```csharp
await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Authentification RTSP

La plupart des caméras IP exigent des identifiants pour accéder au flux vidéo. Vous pouvez fournir l'authentification de deux manières.

**Identifiants en paramètres** — passez le nom d'utilisateur et le mot de passe à `RTSPSourceSettings.CreateAsync()` :

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin",      // nom d'utilisateur
    "mypassword", // mot de passe
    true);        // audio activé
```

**Identifiants dans l'URL** — intégrez-les directement dans l'URI RTSP :

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://admin:mypassword@192.168.1.21:554/stream"),
    null, null, true);
```

Pour les caméras ONVIF, authentifiez-vous via `ONVIFClientX` pour récupérer automatiquement l'URI du flux — consultez la section [Découverte de caméra ONVIF](#decouverte-de-camera-onvif) ci-dessous. La plupart des caméras utilisent l'authentification digest par défaut. Si vous rencontrez des échecs de connexion, vérifiez que le mode d'authentification de votre caméra correspond (digest vs basic) et que le port RTSP (typiquement 554) est accessible.

## Mode faible latence

Pour la surveillance en temps réel ou le contrôle PTZ, activez le mode faible latence pour réduire le délai du flux du défaut ~250 ms à 60-120 ms :

```csharp
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

rtspSettings.LowLatencyMode = true;

var rtspSource = new RTSPSourceBlock(rtspSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
videoRenderer.IsSync = false; // Désactiver la synchro A/V pour la latence la plus faible

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
```

Pour le réglage détaillé du tampon et la configuration transport UDP vs TCP, consultez la référence [Configuration du protocole RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md).

## Découverte de caméra ONVIF

Découvrez automatiquement les caméras IP sur le réseau local via le protocole de découverte ONVIF, puis récupérez les URI des flux et les informations des caméras.

```csharp
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Découvrir les caméras sur le réseau (délai de 5 secondes)
var discovery = new Discovery();
var cameras = await discovery.Discover(5);

foreach (var camera in cameras)
{
    Console.WriteLine($"Found camera: {camera.XAdresses?.FirstOrDefault()}");
}

// Se connecter à une caméra spécifique via ONVIF
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(
    "http://192.168.1.21/onvif/device_service",
    "admin",
    "password");

if (connected)
{
    // Obtenir les informations de la caméra
    var info = onvifClient.DeviceInformation;
    Console.WriteLine($"Camera: {info?.Model}, Serial: {info?.SerialNumber}");

    // Obtenir les profils disponibles et l'URI du flux
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles?.Length > 0)
    {
        var mediaUri = await onvifClient.GetStreamUriAsync(profiles[0]);
        Console.WriteLine($"Stream URI: {mediaUri.Uri}");
    }
}
```

Pour le contrôle PTZ, plusieurs profils et fonctionnalités ONVIF avancées, consultez le guide [Intégration ONVIF de caméra IP](../../videocapture/video-sources/ip-cameras/onvif.md).

## Options d'enregistrement

Il existe deux approches d'enregistrement des flux RTSP, chacune adaptée à des cas d'usage différents :

| | Passthrough (sans réencodage) | Réencodage |
| --- | --- | --- |
| Consommation CPU | Minimale | Élevée |
| Qualité vidéo | Originale (sans perte) | Recompressée |
| Taille de fichier | Identique à la source | Débit configurable |
| Traitement vidéo | Pas de superpositions, redimensionnement ou effets | Capacité d'édition complète |
| Idéal pour | Archivage vidéosurveillance, NVR | Streaming, post-traitement |

### Enregistrement avec passthrough (sans réencodage)

L'enregistrement passthrough sauvegarde le flux compressé d'origine directement dans un fichier sans décoder ni réencoder la vidéo. Cette approche utilise `RTSPRAWSourceBlock` au lieu de `RTSPSourceBlock` — les données vidéo passent directement de la caméra au puits de fichier avec zéro surcoût CPU pour le traitement vidéo.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;

// Créer un pipeline séparé pour l'enregistrement
var recordPipeline = new MediaBlocksPipeline();

// Créer la source RTSP RAW (reçoit le flux compressé directement)
var rawSettings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

var rawSource = new RTSPRAWSourceBlock(rawSettings);

// Créer le puits fichier MP4
var muxer = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));

// Connecter la vidéo — pas de décodage, pas de réencodage
var videoPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Video);
recordPipeline.Connect(rawSource.VideoOutput, videoPad);

// Connecter l'audio — réencoder en AAC pour la compatibilité MP4
var audioPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Audio);

var decodeBin = new DecodeBinBlock(false, true, false);
var aacEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

recordPipeline.Connect(rawSource.AudioOutput, decodeBin.Input);
recordPipeline.Connect(decodeBin.AudioOutput, aacEncoder.Input);
recordPipeline.Connect(aacEncoder.Output, audioPad);

await recordPipeline.StartAsync();
```

**Choix du format de conteneur :** utilisez MP4 pour une large compatibilité de lecture. Utilisez MPEG-TS (`MPEGTSSinkBlock`) pour un enregistrement résistant aux plantages — si le processus se termine de manière inattendue, toutes les données écrites jusqu'à ce point sont préservées. MPEG-TS est préférable pour la vidéosurveillance 24/7.

Pour une implémentation complète avec gestion des erreurs, gestion d'état et nettoyage des ressources, consultez le guide [Enregistrer un flux RTSP sans réencodage](rtsp-save-original-stream.md).

### Enregistrement avec réencodage

Lorsque vous devez redimensionner la vidéo, ajouter des superpositions ou filigranes, changer le codec ou réduire le débit, utilisez `RTSPSourceBlock` (qui décode le flux) suivi de blocs d'encodage :

```csharp
// RTSPSourceBlock décode le flux, permettant le traitement
var rtspSource = new RTSPSourceBlock(rtspSettings);
var mp4Sink = new MP4SinkBlock("output.mp4");

// Vidéo : décodée → (traitement facultatif) → encodage H.264 → multiplexage
// Audio : décodé → encodage AAC → multiplexage

pipeline.Connect(rtspSource.VideoOutput, /* blocs d'encodeur/traitement */);
```

Pour des exemples de code détaillés d'enregistrement avec traitement vidéo (redimensionnement, effets, détection de visages), consultez le guide [Capture ONVIF avec post-traitement](onvif-capture-with-postprocessing.md). Pour un exemple de réencodage plus simple, consultez le tutoriel [Capture caméra IP vers MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md).

## Vue multi-caméras

Pour afficher plusieurs caméras IP simultanément, créez des instances `MediaBlocksPipeline` indépendantes — une par caméra. Chaque pipeline s'exécute dans son propre thread et peut être démarré et arrêté indépendamment.

```csharp
// Créer des pipelines séparés pour chaque caméra
var pipeline1 = new MediaBlocksPipeline();
var pipeline2 = new MediaBlocksPipeline();

// Caméra 1
var source1 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"), "admin", "pass1", true));
var renderer1 = new VideoRendererBlock(pipeline1, VideoView1);
pipeline1.Connect(source1.VideoOutput, renderer1.Input);

// Caméra 2
var source2 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/stream"), "admin", "pass2", true));
var renderer2 = new VideoRendererBlock(pipeline2, VideoView2);
pipeline2.Connect(source2.VideoOutput, renderer2.Input);

// Démarrer les deux
await pipeline1.StartAsync();
await pipeline2.StartAsync();
```

La [démo RTSP MultiView](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) sur GitHub présente une disposition en grille avec des contrôles d'enregistrement par caméra et enregistrement passthrough.

## Foire aux questions

### Comment m'authentifier auprès d'une caméra IP RTSP en C# ?

Intégrez les identifiants directement dans l'URL RTSP (`rtsp://user:pass@ip:554/path`) ou passez-les comme paramètres distincts à `RTSPSourceSettings.CreateAsync()`. Pour les caméras ONVIF, connectez-vous d'abord via `ONVIFClientX` avec nom d'utilisateur et mot de passe, puis récupérez l'URI du flux authentifié via `GetStreamUriAsync()`. La plupart des caméras prennent en charge l'authentification digest par défaut.

### Dois-je utiliser passthrough ou réencodage lors de l'enregistrement de flux RTSP ?

Utilisez le passthrough pour l'archivage vidéosurveillance et les applications NVR — il ne nécessite aucun CPU pour le traitement vidéo et préserve la qualité d'origine de la caméra. Utilisez le réencodage lorsque vous devez redimensionner, ajouter des superpositions, changer de codec ou de débit, ou diffuser dans un format différent. La plupart des applications d'enregistrement professionnelles utilisent le passthrough pour minimiser la charge serveur.

### Comment réduire la latence du flux RTSP en dessous de 100 ms ?

Activez `LowLatencyMode = true` sur `RTSPSourceSettings` et désactivez la synchro du moteur de rendu vidéo avec `IsSync = false`. Utilisez le transport UDP lorsque votre réseau le permet. Latence attendue : 60-120 ms contre 250 ms par défaut. Consultez le [guide du protocole RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) pour les options avancées de réglage du tampon.

### Puis-je visualiser et enregistrer depuis plusieurs caméras IP simultanément ?

Oui. Créez des instances `MediaBlocksPipeline` séparées pour chaque caméra — chaque pipeline fonctionne indépendamment avec sa propre connexion RTSP, son décodeur et son moteur de rendu. Vous pouvez ajouter l'enregistrement par caméra en créant des pipelines d'enregistrement supplémentaires utilisant `RTSPRAWSourceBlock` pour la capture passthrough. La démo RTSP MultiView sur GitHub présente une implémentation complète avec disposition en grille et contrôles d'enregistrement individuels.

### Quel format de conteneur utiliser pour l'enregistrement passthrough — MP4 ou MPEG-TS ?

Utilisez MP4 pour la compatibilité de lecture standard sur tous les appareils et lecteurs. Utilisez MPEG-TS pour un enregistrement résistant aux plantages — si l'application ou le système plante pendant l'enregistrement, toutes les données écrites jusqu'au point de défaillance sont préservées. Pour la vidéosurveillance 24/7 ou les enregistrements critiques, MPEG-TS est le choix recommandé.

## Voir aussi

- [Enregistrer un flux RTSP sans réencodage](rtsp-save-original-stream.md) — enregistrement passthrough détaillé avec implémentation complète de la classe RTSPRecorder
- [Intégration ONVIF de caméra IP](../../videocapture/video-sources/ip-cameras/onvif.md) — découverte ONVIF, profils, contrôle PTZ
- [Configuration du protocole RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — UDP vs TCP, paramètres de tampon, réglage faible latence
- [Capture caméra IP vers MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) — tutoriel d'enregistrement avec réencodage
- [Capture ONVIF avec post-traitement](onvif-capture-with-postprocessing.md) — redimensionnement, effets, flou des visages pendant l'enregistrement
- [Grille RTSP multi-caméras (mur NVR)](multi-camera-rtsp-grid.md) — passer d'un lecteur unique à un mur d'aperçu en direct 4×4 sur WPF ou MAUI
- [Reconnexion RTSP et solution de repli](../../general/network-sources/reconnection-and-fallback.md) — gérez les coupures de caméras avec des événements de reconnexion et le `FallbackSwitch` déclaratif
- [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — démos d'aperçu RTSP, de capture et multi-vue
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — page produit et téléchargements
