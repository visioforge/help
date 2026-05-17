---
title: Caméra IP ONVIF en C# .NET — découverte et contrôle PTZ
description: Découverte automatique de caméras ONVIF via WS-Discovery, contrôle des préréglages PTZ et enregistrement avec VisioForge Video Capture SDK. Exemples C#.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Decoding
  - Webcam
  - IP Camera
  - NDI Source
  - RTSP
  - ONVIF
  - NDI
  - H.264
  - C#
primary_api_classes:
  - RTSPSourceSettings
  - ONVIFClientX
  - VideoCaptureCoreX
  - MediaBlocksPipeline
  - RTSPSourceBlock

---

# Intégration de caméras IP ONVIF — Guide complet

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Prise en charge multiplateforme"
    La découverte ONVIF et le contrôle PTZ fonctionnent à la fois sur `VideoCaptureCore` (Windows uniquement, DirectShow classique) et `VideoCaptureCoreX` / Media Blocks SDK (multiplateforme : **Windows, macOS, Linux, Android et iOS** via GStreamer). Consultez la [matrice de prise en charge des plateformes](../../../platform-matrix.md) et le [guide de déploiement Linux](../../../deployment-x/Ubuntu.md).

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Table des matières

- [Intégration de caméras IP ONVIF — Guide complet](#integration-de-cameras-ip-onvif-guide-complet)
  - [Table des matières](#table-des-matieres)
  - [Qu'est-ce qu'ONVIF ?](#quest-ce-quonvif)
  - [Avantages de l'intégration ONVIF](#avantages-de-lintegration-onvif)
  - [Découverte et énumération des caméras](#decouverte-et-enumeration-des-cameras)
    - [Découverte des caméras ONVIF sur votre réseau](#decouverte-des-cameras-onvif-sur-votre-reseau)
    - [Interrogation des capacités de la caméra](#interrogation-des-capacites-de-la-camera)
  - [Connexion aux caméras ONVIF](#connexion-aux-cameras-onvif)
    - [Connexion de base](#connexion-de-base)
  - [Travailler avec les profils multimédias](#travailler-avec-les-profils-multimedias)
  - [Aperçu vidéo](#apercu-video)
    - [Configuration de l'aperçu de base](#configuration-de-lapercu-de-base)
    - [Aperçu à faible latence](#apercu-a-faible-latence)
  - [Contrôle PTZ (Pan-Tilt-Zoom)](#controle-ptz-pan-tilt-zoom)
    - [Opérations PTZ de base](#operations-ptz-de-base)
    - [Préréglages PTZ](#prereglages-ptz)
    - [Positionnement absolu](#positionnement-absolu)
  - [Actions et capacités de la caméra](#actions-et-capacites-de-la-camera)
    - [Interroger les capacités de la caméra](#interroger-les-capacites-de-la-camera)
    - [Redémarrer la caméra](#redemarrer-la-camera)
    - [Obtenir la date et l'heure système](#obtenir-la-date-et-lheure-systeme)
  - [Transformer une caméra locale en source ONVIF](#transformer-une-camera-locale-en-source-onvif)
  - [Bonnes pratiques](#bonnes-pratiques)
    - [Gestion des connexions](#gestion-des-connexions)
    - [Optimisation des performances](#optimisation-des-performances)
    - [Considérations réseau](#considerations-reseau)
    - [Sécurité](#securite)
    - [Gestion des erreurs](#gestion-des-erreurs)
  - [Dépannage](#depannage)
    - [Problèmes courants](#problemes-courants)
      - ["Impossible de se connecter à la caméra ONVIF"](#impossible-de-se-connecter-a-la-camera-onvif)
      - ["Aucune caméra découverte"](#aucune-camera-decouverte)
      - ["Le flux ne se lit pas"](#le-flux-ne-se-lit-pas)
      - ["Utilisation CPU élevée pendant l'enregistrement"](#utilisation-cpu-elevee-pendant-lenregistrement)
      - ["Les commandes PTZ ne fonctionnent pas"](#les-commandes-ptz-ne-fonctionnent-pas)
    - [Outils de diagnostic](#outils-de-diagnostic)
      - [Activer la journalisation de débogage](#activer-la-journalisation-de-debogage)
      - [Tester la connexion RTSP](#tester-la-connexion-rtsp)
    - [Obtenir de l'aide](#obtenir-de-laide)
    - [Démos associées](#demos-associees)

## Qu'est-ce qu'ONVIF ?

ONVIF (Open Network Video Interface Forum) est un protocole standard de l'industrie qui permet une interopérabilité fluide entre les produits vidéo réseau de différents fabricants. Ce protocole définit une interface commune pour les périphériques de sécurité basés sur IP, notamment les caméras, les NVR (Network Video Recorders) et les systèmes de contrôle d'accès.

**Avantages clés :**
- **Indépendance vis-à-vis du fabricant** : travaillez avec des caméras de différents fabricants via une API unifiée
- **Communication standardisée** : méthodes cohérentes pour la découverte de périphériques, le streaming et le contrôle
- **Pérenne** : les nouveaux périphériques compatibles ONVIF fonctionnent avec les applications existantes
- **Riche jeu de fonctionnalités** : accès aux profils, aux flux multimédias, aux événements, au PTZ et plus

## Avantages de l'intégration ONVIF

- **Neutralité fabricant** : créez des applications qui fonctionnent avec des caméras de plusieurs fabricants
- **Développement pérenne** : à mesure que de nouvelles caméras compatibles ONVIF entrent sur le marché, votre application les prendra en charge
- **Communication standardisée** : méthodes cohérentes pour la découverte de périphériques, le streaming vidéo et les contrôles PTZ
- **Temps de développement réduit** : pas besoin d'implémenter des API propriétaires pour chaque marque de caméra
- **Fonctionnalités avancées** : accès aux profils, aux flux multimédias, aux événements et à la gestion des périphériques

## Découverte et énumération des caméras

### Découverte des caméras ONVIF sur votre réseau

La première étape du travail avec des caméras ONVIF consiste à les découvrir sur votre réseau local à l'aide du protocole WS-Discovery.

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFDiscovery.Models;

private Discovery _onvifDiscovery = new Discovery();
private CancellationTokenSource _cts;

// Découvrir les caméras pendant 5 secondes
_cts = new CancellationTokenSource();

try
{
    await _onvifDiscovery.Discover(5, (device) =>
    {
        if (device.XAdresses?.Any() == true)
        {
            var address = device.XAdresses.FirstOrDefault();
            if (!string.IsNullOrEmpty(address))
            {
                Console.WriteLine($"Found camera at: {address}");
                // Ajouter à la liste de l'UI, etc.
            }
        }
    }, _cts.Token);
}
catch (OperationCanceledException)
{
    // Découverte annulée
}
```

**Fonctionnalités clés :**
- **Protocole WS-Discovery** : découvre automatiquement les caméras compatibles ONVIF sur le réseau local
- **Contrôle du délai** : spécifiez la durée de découverte en secondes
- **Rappel asynchrone** : recevez les périphériques découverts en temps réel à mesure qu'ils répondent
- **Prise en charge de l'annulation** : annulez la découverte à l'aide d'un CancellationToken

### Interrogation des capacités de la caméra

Une fois découverte, vous pouvez vous connecter à une caméra et interroger ses capacités :

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
var result = await onvifClient.ConnectAsync(cameraUrl, username, password);

if (result)
{
    // Obtenir les informations sur le périphérique
    var deviceInfo = onvifClient.DeviceInformation;
    Console.WriteLine($"Camera: {deviceInfo?.Model}, S/N: {deviceInfo?.SerialNumber}");
    
    // Obtenir les profils disponibles
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles != null)
    {
        foreach (var profile in profiles)
        {
            var mediaUri = await onvifClient.GetStreamUriAsync(profile);
            if (mediaUri != null)
            {
                Console.WriteLine($"Profile: {profile.Name}, URI: {mediaUri.Uri}");
            }
        }
    }
}
```

## Connexion aux caméras ONVIF

### Connexion de base

```cs
using VisioForge.Core.ONVIFX;

// Se connecter à la caméra ONVIF
var onvifClient = new ONVIFClientX();
var connected = await onvifClient.ConnectAsync(
    "http://192.168.1.100:80/onvif/device_service", 
    "admin", 
    "password");

if (connected)
{
    Console.WriteLine("Successfully connected to camera");
}
else
{
    Console.WriteLine("Connection failed");
}
```

## Travailler avec les profils multimédias

Les caméras ONVIF fournissent généralement plusieurs profils multimédias avec différentes résolutions, codecs et fréquences d'images.

```cs
// Obtenir tous les profils disponibles
var profiles = await onvifClient.GetProfilesAsync();

if (profiles != null && profiles.Length > 0)
{
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name}");
        Console.WriteLine($"Token: {profile.token}");
        
        // Obtenir l'URI du flux pour ce profil
        var mediaUri = await onvifClient.GetStreamUriAsync(profile);
        if (mediaUri != null)
        {
            Console.WriteLine($"  Stream URI: {mediaUri.Uri}");
        }
    }
    
    // Utiliser le premier profil
    var selectedProfile = profiles[0];
    var streamUri = await onvifClient.GetStreamUriAsync(selectedProfile);
}
```

## Aperçu vidéo

### Configuration de l'aperçu de base

=== "Media Blocks SDK"

    
    ```cs
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoRendering;
    using VisioForge.Core.MediaBlocks.AudioRendering;
    using VisioForge.Core.Types.X.Sources;
    
    // Créer le pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Obtenir l'URL RTSP depuis le profil ONVIF
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Créer la source RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // activer l'audio
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    
    // Créer le moteur de rendu vidéo
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Connecter les blocs
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    
    // Optionnel : ajouter le rendu audio
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);
    
    // Démarrer l'aperçu
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.VideoCaptureX;
    
    // Créer le moteur de capture vidéo
    var videoCapture = new VideoCaptureCoreX(videoView);
    
    // Obtenir l'URI du flux depuis ONVIF
    var streamUri = await onvifClient.GetStreamUriAsync(profile);
    
    // Créer les paramètres de source RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true); // activer l'audio
    
    // Définir la source vidéo
    videoCapture.Video_Source = rtspSettings;
    
    // Configurer l'audio
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    // Démarrer l'aperçu
    await videoCapture.StartAsync();
    ```
    


### Aperçu à faible latence

Pour les applications de vidéosurveillance et de monitoring en temps réel, activez le mode à faible latence :

=== "Media Blocks SDK"

    
    ```cs
    // Créer la source RTSP avec faible latence
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Activer le mode faible latence (60-120 ms de latence totale)
    rtspSettings.LowLatencyMode = true;
    
    var rtspSource = new RTSPSourceBlock(rtspSettings);
    var videoRenderer = new VideoRendererBlock(pipeline, videoView);
    
    // Désactiver la synchronisation pour une latence encore plus faible
    videoRenderer.IsSync = false;
    
    pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
    await pipeline.StartAsync();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // Créer la source RTSP
    var rtspSettings = await RTSPSourceSettings.CreateAsync(
        new Uri(streamUri.Uri), 
        username, 
        password, 
        true);
    
    // Activer le mode faible latence
    rtspSettings.LowLatencyMode = true;
    
    videoCapture.Video_Source = rtspSettings;
    videoCapture.Audio_Record = true;
    videoCapture.Audio_Play = true;
    
    await videoCapture.StartAsync();
    ```
    


## Contrôle PTZ (Pan-Tilt-Zoom)

### Opérations PTZ de base

```cs
using VisioForge.Core.ONVIFX;
using VisioForge.Core.ONVIFX.PTZ;

// Se connecter à la caméra
var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Obtenir le token de profil
var profiles = await onvifClient.GetProfilesAsync();
var profileToken = profiles[0].token;

// La surcharge scalaire légère de ContinuousMoveAsync prend les vitesses
// pan/tilt/zoom directement (plage -1.0..1.0 ; 0 = aucun mouvement sur cet axe).

// Pan à droite
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0.5f, tilt: 0f, zoom: 0f);

// Pan à gauche
await onvifClient.ContinuousMoveAsync(profileToken, pan: -0.5f, tilt: 0f, zoom: 0f);

// Tilt vers le haut
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0.5f, zoom: 0f);

// Tilt vers le bas
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: -0.5f, zoom: 0f);

// Zoom avant
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0f, zoom: 0.5f);

// Zoom arrière
await onvifClient.ContinuousMoveAsync(profileToken, pan: 0f, tilt: 0f, zoom: -0.5f);

// Arrêter le mouvement sur les deux axes
await onvifClient.StopMoveAsync(profileToken, panTilt: true, zoom: true);
```

### Préréglages PTZ

```cs
// Obtenir les préréglages disponibles
var presets = await onvifClient.GetPresetsAsync(profileToken);

foreach (var preset in presets)
{
    Console.WriteLine($"Preset: {preset.Name}, Token: {preset.token}");
}

// Aller au préréglage (position initiale)
if (presets != null && presets.Length > 0)
{
    await onvifClient.GoToPresetAsync(
        profileToken,
        presets[0].token,
        panSpeed:  1.0f,
        tiltSpeed: 1.0f,
        zoomSpeed: 1.0f);
}

// Définir la position actuelle comme préréglage
await onvifClient.SetPresetAsync(profileToken, "MyPreset");
```

### Positionnement absolu

```cs
// Se déplacer vers une position pan + tilt + zoom absolue avec des vitesses par axe
await onvifClient.AbsoluteMoveAsync(
    profileToken,
    pan:       0.5f,
    tilt:      0.3f,
    zoom:      0.7f,
    panSpeed:  1.0f,
    tiltSpeed: 1.0f,
    zoomSpeed: 1.0f);
```

## Actions et capacités de la caméra

### Interroger les capacités de la caméra

```cs
using VisioForge.Core.ONVIFX;

var onvifClient = new ONVIFClientX();
await onvifClient.ConnectAsync(cameraUrl, username, password);

// Obtenir les informations sur le périphérique
var deviceInfo = onvifClient.DeviceInformation;
Console.WriteLine($"Manufacturer: {deviceInfo?.Manufacturer}");
Console.WriteLine($"Model: {deviceInfo?.Model}");
Console.WriteLine($"Firmware: {deviceInfo?.FirmwareVersion}");
Console.WriteLine($"Serial Number: {deviceInfo?.SerialNumber}");
Console.WriteLine($"Hardware ID: {deviceInfo?.HardwareId}");

// Obtenir les capacités des services
var capabilities = await onvifClient.GetCapabilitiesAsync();

// Vérifier la prise en charge PTZ
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ supported");
}

// Vérifier l'analytique
if (capabilities?.Analytics != null)
{
    Console.WriteLine("Analytics supported");
}

// Vérifier les événements
if (capabilities?.Events != null)
{
    Console.WriteLine("Events supported");
}
```

### Redémarrer la caméra

```cs
await onvifClient.SystemRebootAsync();
```

### Obtenir la date et l'heure système

```cs
var dateTime = await onvifClient.GetSystemDateAndTimeAsync();
Console.WriteLine($"Camera time: {dateTime}");
```

## Transformer une caméra locale en source ONVIF

Convertissez votre webcam USB locale en flux IP que des clients ONVIF peuvent consommer. La démo console `RTSP Webcam Server` incluse avec Media Blocks SDK expose une caméra DirectShow en tant que point de terminaison RTSP que les enregistreurs ONVIF et logiciels VMS peuvent ingérer.

=== "Media Blocks SDK"

    
    ```cs
    using System;
    using System.Threading;
    using VisioForge.Core;
    using VisioForge.Core.MediaBlocks;
    using VisioForge.Core.MediaBlocks.Sinks;
    using VisioForge.Core.MediaBlocks.Sources;
    using VisioForge.Core.MediaBlocks.VideoEncoders;
    using VisioForge.Core.Types.X.Output;
    using VisioForge.Core.Types.X.Sources;
    using VisioForge.Core.Types.X.VideoCapture;
    
    Console.WriteLine("Initializing VisioForge SDK.");
    VisioForgeX.InitSDK();
    
    var cameras = DeviceEnumerator.Shared.VideoSources();
    Console.WriteLine("Select the web camera");
    for (int i = 0; i < cameras.Length; i++)
    {
        Console.WriteLine($"{i + 1}: {cameras[i].DisplayName}");
    }
    
    Console.Write("Enter the number of the camera: ");
    VideoCaptureDeviceInfo cameraInfo = null;
    if (int.TryParse(Console.ReadLine(), out int cameraIndex) && cameraIndex > 0 && cameraIndex <= cameras.Length)
    {
        cameraInfo = cameras[cameraIndex - 1];
        Console.WriteLine($"Selected camera: {cameraInfo.DisplayName}");
    }
    else
    {
        Console.WriteLine("Invalid selection. Exiting.");
    
        VisioForgeX.DestroySDK();
        return;
    }
    
    var pipeline = new MediaBlocksPipeline();
    
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(cameraInfo);
    videoSourceSettings.Format = cameraInfo.GetHDVideoFormatAndFrameRate(out var frameRate).ToFormat();
    videoSourceSettings.Format.FrameRate = frameRate;
    
    var cameraSource = new SystemVideoSourceBlock(videoSourceSettings);
    
    var rtspServerSettings = new RTSPServerSettings(H264EncoderBlock.GetDefaultSettings(), null)
    {
        Port = 8554,
    };
    
    var rtspBlock = new RTSPServerBlock(rtspServerSettings);
    
    Console.WriteLine("RTSP Server URL: " + rtspBlock.Settings.URL);
    
    pipeline.Connect(cameraSource.Output, rtspBlock.Input);
    
    Console.WriteLine("Starting the pipeline...");
    
    new Thread(() =>
    {
        pipeline.Start();
    }).Start();
    
    Console.WriteLine("Pipeline started. Press any key to stop the server and exit.");
    Console.ReadKey();
    
    Console.WriteLine("Stopping the pipeline...");
    
    pipeline.Stop();
    
    Console.WriteLine("Pipeline stopped.");
    
    pipeline.Dispose();
    
    VisioForgeX.DestroySDK();
    ```
    

=== "Video Capture SDK"

    
    ```cs
    // La fonctionnalité serveur ONVIF/RTSP est fournie par Media Blocks SDK.
    ```
    


## Bonnes pratiques

### Gestion des connexions

1. **Libérez toujours correctement les clients ONVIF :**
   ```cs
   using (var onvifClient = new ONVIFClientX())
   {
       await onvifClient.ConnectAsync(url, username, password);
       // ... utiliser le client ...
   } // Automatiquement libéré
   ```

2. **Gérez gracieusement les échecs de connexion :**
   ```cs
   var maxRetries = 3;
   var retryCount = 0;
   
   while (retryCount < maxRetries)
   {
       try
       {
           var connected = await onvifClient.ConnectAsync(url, user, pass);
           if (connected)
               break;
       }
       catch (Exception ex)
       {
           Console.WriteLine($"Connection attempt {retryCount + 1} failed: {ex.Message}");
       }
       
       retryCount++;
       await Task.Delay(2000); // Attendre avant de réessayer
   }
   ```

3. **Utilisez des jetons d'annulation pour la découverte :**
   ```cs
   var cts = new CancellationTokenSource();
   cts.CancelAfter(TimeSpan.FromSeconds(10));
   
   await _onvifDiscovery.Discover(10, callback, cts.Token);
   ```

### Optimisation des performances

1. **Utilisez RTSPRAWSourceBlock pour l'enregistrement sans réencodage** — réduit considérablement l'utilisation CPU
2. **N'activez le mode faible latence que lorsque c'est nécessaire** — échange stabilité contre vitesse
3. **Limitez les flux simultanés** en fonction des capacités matérielles
4. **Utilisez les décodeurs matériels** lorsqu'ils sont disponibles :
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```

### Considérations réseau

1. **Utilisez TCP pour des connexions fiables** (`AllowedProtocols` est un enum-drapeau `RTSPSourceProtocol` — définissez-le pour forcer un transport spécifique) :
   ```cs
   rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;
   ```

2. **Ajustez la latence du tampon anti-gigue** (il n'y a pas de propriété `Timeout` autonome ; `Latency` contrôle le tampon anti-gigue RTSP — des valeurs plus élevées échangent la latence contre la stabilité sur les réseaux avec pertes) :
   ```cs
   rtspSettings.Latency = TimeSpan.FromMilliseconds(1000);
   ```

3. **Surveillez les déconnexions :**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       if (e.Message.Contains("disconnect"))
       {
           // Tenter la reconnexion
       }
   };
   ```

### Sécurité

1. **Ne codez jamais les identifiants en dur** — utilisez des fichiers de configuration ou un stockage sécurisé
2. **Utilisez HTTPS pour le streaming web** lorsque c'est possible
3. **Implémentez l'authentification** pour les points de terminaison de streaming
4. **Validez les entrées utilisateur** lors de la construction des URL
5. **Gardez les identifiants en mémoire le moins longtemps possible**

### Gestion des erreurs

1. **Journalisez toutes les erreurs à des fins de diagnostic :**
   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Logger.Error($"Pipeline error: {e.Message}");
   };
   ```

2. **Activez le mode débogage pendant le développement :**
   ```cs
   pipeline.Debug_Mode = true;
   pipeline.Debug_Dir = @"C:\Logs\VisioForge";
   ```

3. **Gérez les interruptions de flux :**
   ```cs
   // Implémenter la logique de reconnexion automatique
   var reconnectAttempts = 0;
   const int maxReconnects = 5;
   
   pipeline.OnError += async (sender, e) =>
   {
       if (reconnectAttempts < maxReconnects)
       {
           reconnectAttempts++;
           await Task.Delay(5000);
           await pipeline.StopAsync();
           await pipeline.StartAsync();
       }
   };
   ```

## Dépannage

### Problèmes courants

#### "Impossible de se connecter à la caméra ONVIF"

**Causes possibles :**
- Format d'URL incorrect (devrait être : `http://IP:PORT/onvif/device_service`)
- Identifiants incorrects
- Pare-feu réseau bloquant la connexion
- Service ONVIF de la caméra désactivé

**Solutions :**
```cs
// Essayer différents formats d'URL
var urls = new[]
{
    "http://192.168.1.100:80/onvif/device_service",
    "http://192.168.1.100:8080/onvif/device_service",
    "http://192.168.1.100/onvif/device_service"
};

foreach (var url in urls)
{
    if (await onvifClient.ConnectAsync(url, user, pass))
    {
        Console.WriteLine($"Connected using: {url}");
        break;
    }
}
```

#### "Aucune caméra découverte"

**Causes possibles :**
- Caméras sur un sous-réseau différent
- Multicast bloqué par le réseau
- Pare-feu bloquant WS-Discovery

**Solutions :**
1. Vérifiez la configuration réseau
2. Essayez une connexion directe avec une IP connue
3. Vérifiez que la prise en charge ONVIF de la caméra est activée
4. Augmentez le délai de découverte

#### "Le flux ne se lit pas"

**Causes possibles :**
- Codec non pris en charge
- Bande passante réseau insuffisante
- URL de flux incorrecte

**Solutions :**
```cs
// Lire et mettre en cache les informations du flux avant de démarrer le pipeline.
// ReadInfoAsync sonde la caméra ; GetInfo() retourne ensuite le MediaFileInfo mis en cache.
var info = await rtspSettings.ReadInfoAsync();
if (info == null)
{
    Console.WriteLine("Cannot get stream info - check URL");
    return;
}

Console.WriteLine($"Video codec: {info.VideoStreams[0].Codec}");
Console.WriteLine($"Resolution: {info.VideoStreams[0].Width}x{info.VideoStreams[0].Height}");
Console.WriteLine($"Bitrate: {info.VideoStreams[0].Bitrate}");
```

#### "Utilisation CPU élevée pendant l'enregistrement"

**Causes possibles :**
- Réencodage inutile
- Trop de flux simultanés
- Décodage logiciel au lieu de matériel

**Solutions :**
1. Utilisez `RTSPRAWSourceBlock` pour l'enregistrement sans réencodage
2. Activez le décodeur matériel :
   ```cs
   rtspSettings.UseGPUDecoder = true;
   ```
3. Limitez les flux simultanés
4. Réduisez la résolution/le débit à la caméra

#### "Les commandes PTZ ne fonctionnent pas"

**Causes possibles :**
- La caméra ne prend pas en charge le PTZ
- Mauvais profil sélectionné
- Service PTZ désactivé

**Solutions :**
```cs
// Vérifier les capacités PTZ
var capabilities = await onvifClient.GetCapabilitiesAsync();
if (capabilities?.PTZ != null)
{
    Console.WriteLine("PTZ is supported");
    
    // Obtenir la configuration PTZ
    var profiles = await onvifClient.GetProfilesAsync();
    foreach (var profile in profiles)
    {
        Console.WriteLine($"Profile: {profile.Name}, Token: {profile.token}");
    }
}
else
{
    Console.WriteLine("PTZ not supported by this camera");
}
```

### Outils de diagnostic

#### Activer la journalisation de débogage

=== "Media Blocks SDK"

    
    ```cs
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    

=== "Video Capture SDK"

    
    ```cs
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), 
        "VisioForge", 
        "Logs");
    ```
    


#### Tester la connexion RTSP

```cs
// Tester si l'URL RTSP est valide
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), 
    username, 
    password, 
    true);

var info = await rtspSettings.ReadInfoAsync();
if (info != null)
{
    Console.WriteLine("✓ RTSP connection successful");
    Console.WriteLine($"  Video streams: {info.VideoStreams.Count}");
    Console.WriteLine($"  Audio streams: {info.AudioStreams.Count}");

    foreach (var stream in info.VideoStreams)
    {
        Console.WriteLine($"  Video: {stream.Codec} {stream.Width}x{stream.Height}");
    }
}
else
{
    Console.WriteLine("✗ RTSP connection failed");
}
```

### Obtenir de l'aide

- **Exemples de code** : [Dépôt GitHub Samples](https://github.com/visioforge/.Net-SDK-s-samples)
- **Documentation** : [Documentation VisioForge](https://www.visioforge.com/help/)
- **Forum de support** : [Support VisioForge](https://support.visioforge.com)

### Démos associées

- [Démo RTSP MultiView](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) — enregistrement de plusieurs caméras sans réencodage
- [Démo RTSP Preview](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) — aperçu et enregistrement avec post-traitement
- [Démo IP Capture](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) — intégration complète de caméras IP avec contrôle PTZ
- [Répertoire des marques de caméras IP](../../../camera-brands/index.md) — URL RTSP et guides de connexion pour plus de 60 fabricants de caméras

### Documentation associée

- [Configuration des sources de caméras RTSP](rtsp.md) — référence `IPCameraSourceSettings` et `RTSPSourceSettings` avec réglage UDP/TCP
- [Plongée approfondie dans le protocole RTSP](../../../general/network-streaming/rtsp.md) — fonctionnement interne du protocole et architecture de streaming
- [Intégration de sources NDI](ndi.md) — alternative vidéo-sur-IP professionnelle à ONVIF/RTSP
- [Tutoriel d'aperçu en direct d'une caméra IP](../../video-tutorials/ip-camera-preview.md) — pas-à-pas vidéo avec un exemple C# minimal
- [Tutoriel d'enregistrement RTSP vers MP4](../../video-tutorials/ip-camera-capture-mp4.md) — capturer dans un fichier les caméras découvertes par ONVIF
- [Lecteur RTSP Media Blocks](../../../mediablocks/Guides/rtsp-player-csharp.md) — lecture RTSP basée sur pipeline
- [Grille RTSP multi-caméras (mur NVR)](../../../mediablocks/Guides/multi-camera-rtsp-grid.md) — mur d'aperçu en direct 4×4 pour WPF et MAUI, avec des caméras découvertes via ONVIF
- [Reconnexion RTSP et basculement de repli](../../../general/network-sources/reconnection-and-fallback.md) — gérer les coupures de caméras avec des événements de déconnexion et un `FallbackSwitch` automatique
