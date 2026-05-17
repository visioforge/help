---
title: Caméras USB3 Vision et GigE en C# .NET — GenICam industriel
description: Capturez depuis des caméras USB3 Vision, GigE Vision et GenICam en C# / .NET via GenTL. Support Basler, FLIR, IDS, Allied Vision. Vision industrielle.
sidebar_label: Périphériques USB3 Vision, GigE et GenICam
order: 15
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - GenICamSourceSettings
  - H264EncoderBlock
  - GenICamSourceBlock

---

# Intégration de caméras USB3 Vision, GigE et GenICam

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button }

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../../../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Vue d'ensemble

Les caméras industrielles utilisant les standards USB3 Vision, GigE Vision et GenICam offrent une qualité d'image et des performances supérieures pour les applications de vision industrielle. Notre SDK permet une intégration fluide avec ces types de caméras professionnelles via différentes options de connectivité.

## Protocole GigE Vision

GigE Vision est un standard d'interface pour caméras industrielles basé sur la technologie Gigabit Ethernet. Il offre plusieurs avantages pour les applications de vision industrielle :

- **Transfert de données à haute vitesse** : prend en charge jusqu'à 1 Gbps sur les réseaux GigE standards et 10+ Gbps sur les réseaux 10GigE modernes
- **Longueur de câble étendue** : peut fonctionner sur des distances allant jusqu'à 100 mètres avec un câblage Ethernet standard
- **Architecture réseau** : plusieurs caméras peuvent partager la même infrastructure réseau
- **Power over Ethernet (PoE)** : les caméras peuvent recevoir l'alimentation via le même câble Ethernet (avec des commutateurs compatibles PoE)
- **Découverte des périphériques** : détection automatique des caméras GigE Vision sur le réseau
- **Capacités multidiffusion** : permet le streaming vers plusieurs clients simultanément

GigE Vision combine l'interface de programmation GenICam avec la couche de transport GigE, fournissant des structures de commandes cohérentes entre les caméras de différents fabricants.

## Protocole USB3 Vision

USB3 Vision est un standard d'interface pour caméras qui tire parti de l'interface USB 3.0 à haute vitesse pour les applications d'imagerie industrielle :

- **Bande passante élevée** : jusqu'à 5 Gbps de débit théorique, permettant haute résolution et fréquences d'images élevées
- **Plug-and-play** : connectivité simple sans cartes d'interface spécialisées
- **Connexion à chaud** : les périphériques peuvent être connectés ou déconnectés sans redémarrage du système
- **Longueur de câble** : prend généralement en charge des distances allant jusqu'à 5 mètres (extensible avec des câbles actifs)
- **Alimentation** : jusqu'à 4,5 W fournis directement par la connexion USB
- **Architecture de pilote standard** : utilise les pilotes USB standards des systèmes d'exploitation

USB3 Vision fonctionne en complément du standard GenICam pour fournir un contrôle cohérent des caméras entre différents fabricants.

## Prise en charge du protocole GenTL (Generic Transport Layer)

VisioForge fournit une prise en charge complète du standard **GenICam GenTL (Generic Transport Layer)**, qui est un composant clé des systèmes de vision industrielle. GenTL définit une interface standardisée pour l'accès aux caméras via différents protocoles de transport tout en conservant une compatibilité indépendante du fournisseur.

### Qu'est-ce que GenTL ?

GenTL (Generic Transport Layer) est une spécification d'interface standardisée qui fournit :

- **Accès agnostique au transport** : API unifiée pour les caméras indépendamment de la couche de transport physique (GigE, USB3, CoaXPress, Camera Link, etc.)
- **Neutralité du fournisseur** : interface cohérente entre les différents fabricants de caméras
- **Architecture modulaire** : sépare les implémentations spécifiques au transport de la logique applicative
- **Modèle producteur/consommateur** : les producteurs GenTL gèrent les spécificités de transport, tandis que les consommateurs GenTL (applications) utilisent des interfaces standardisées

### Implémentation GenTL VisioForge

Notre SDK inclut une prise en charge complète de GenTL via :

#### 1. **Détection automatique du protocole**

Le système détecte automatiquement lorsqu'une caméra est connectée via GenTL et définit le protocole en conséquence.

#### 2. **Configuration de l'environnement GenTL**

Prise en charge des variables d'environnement GenTL standards :

- **GENICAM_GENTL64_PATH** : chemin vers les bibliothèques de producteurs GenTL (64 bits)
- Découverte automatique des producteurs GenTL installés

#### 3. **Gestion complète des erreurs**

Prise en charge complète des codes d'erreur spécifiques à GenTL, notamment :

- Erreurs d'initialisation système
- Problèmes de communication de la couche transport
- Accès aux périphériques et gestion des ressources
- Erreurs de tampon et de streaming
- Délais d'expiration et conditions d'abandon

#### 4. **Fonctionnalités avancées**

- **Énumération des périphériques** : découverte des périphériques compatibles GenTL sur toutes les couches de transport disponibles
- **Gestion de flux** : streaming haute performance avec gestion des tampons GenTL
- **Accès aux fonctionnalités** : accès complet à l'arborescence des fonctionnalités GenICam via l'interface GenTL
- **Prise en charge multitransport** : accès simultané aux caméras sur différentes couches de transport

### Compatibilité des producteurs GenTL

L'implémentation GenTL de VisioForge est compatible avec les producteurs des principaux fabricants :

- **Camera Link** : interfaces de frame grabber à haute vitesse
- **CoaXPress** : connexions longue distance et haute bande passante
- **10 GigE** : connexions Ethernet ultra-haut débit
- **Couches de transport personnalisées** : implémentations de transport spécifiques au fournisseur
- **Systèmes multi-interfaces** : environnements de transport mixtes

### Avantages de l'intégration

L'utilisation de GenTL avec VisioForge offre plusieurs avantages :

1. **Architecture pérenne** : prise en charge de nouvelles couches de transport sans modification de l'application
2. **Développement simplifié** : API unique pour tous les types de transport pris en charge
3. **Performances améliorées** : implémentations optimisées spécifiques au transport
4. **Prise en charge élargie des caméras** : accès à des caméras non disponibles via les interfaces natives
5. **Fonctionnalités professionnelles** : capacités avancées de déclenchement, synchronisation et contrôle

### Exigences de configuration

Pour utiliser des caméras GenTL avec VisioForge :

1. Installez le producteur GenTL approprié de votre fabricant de caméra
2. Définissez la variable d'environnement `GENICAM_GENTL64_PATH` pour qu'elle pointe vers la bibliothèque du producteur
3. Assurez-vous que les caméras sont correctement connectées et reconnues par le producteur GenTL
4. Utilisez les méthodes d'énumération GenICam standards de VisioForge pour découvrir les périphériques GenTL

Le système gère automatiquement l'initialisation GenTL, la découverte des périphériques et la gestion de la couche de transport.

## Prise en charge des pilotes DirectShow

La plupart des fabricants de caméras industrielles incluent des pilotes compatibles DirectShow avec leurs kits de développement. Ces pilotes créent un pont entre l'interface native de la caméra et le framework DirectShow, permettant à notre SDK d'accéder et de contrôler ces périphériques spécialisés.

Avantages clés :

- Voie d'intégration simplifiée
- Accès complet aux flux de la caméra
- Compatibilité avec les flux de travail DirectShow existants

## Prise en charge GenICam multiplateforme

Pour les développeurs travaillant dans des environnements multiplateformes, le moteur multiplateforme de notre SDK prend en charge les caméras implémentant l'interface standard unifiée GenICam. Cela fournit un accès cohérent aux fonctionnalités des caméras entre différents systèmes d'exploitation.

## Prérequis

### macOS

Installez le paquet `Aravis` à l'aide de Homebrew :

```bash
brew install aravis
```

### Linux

Installez le paquet `Aravis` à l'aide du gestionnaire de paquets :

```bash
sudo apt-get install libaravis-0.8-dev
```

### Windows

Installez le paquet `VisioForge.CrossPlatform.GenICam.Windows.x64` dans votre projet via NuGet.

#### Installation du pilote USB sur Windows

Par défaut sous Windows, les caméras USB3 Vision peuvent ne pas disposer du pilote USB3 approprié installé, ce qui peut les empêcher d'apparaître dans les listes d'énumération des périphériques. Il s'agit d'un problème courant avec les caméras USB industrielles qui nécessitent une prise en charge de pilote spécifique.

#### Solutions d'installation de pilote

##### Option 1 : installation d'un pilote USB générique avec Zadig

Pour les caméras sans pilotes spécifiques au fabricant, vous pouvez installer des pilotes USB génériques à l'aide de [Zadig](https://zadig.akeo.ie/), une application Windows qui simplifie l'installation des pilotes USB :

1. **Téléchargez et exécutez Zadig** depuis [https://zadig.akeo.ie/](https://zadig.akeo.ie/)
2. **Connectez votre caméra USB3 Vision** à l'ordinateur
3. **Sélectionnez le périphérique caméra** dans la liste des périphériques de Zadig
4. **Choisissez le pilote approprié** :
   - **WinUSB** : recommandé pour la plupart des applications GenICam
   - **libusb-win32** : pour les applications héritées basées sur libusb
   - **libusbK** : pilote USB alternatif haute performance
5. **Installez le pilote** en cliquant sur « Install Driver » ou « Replace Driver »

Après l'installation, la caméra devrait apparaître dans l'énumération de périphériques de VisioForge et être accessible via l'interface GenICam.

##### Option 2 : SDK du fabricant avec pont GenTL

Si vous disposez d'un SDK de caméra du fournisseur du périphérique, la caméra peut être connectée à l'aide de l'approche **pont GenTL** :

1. **Installez le SDK du fabricant** (par exemple, Basler pylon, FLIR Spinnaker)
2. **Configurez l'environnement GenTL** en définissant la variable d'environnement `GENICAM_GENTL64_PATH`
3. **Utilisez le producteur GenTL** fourni par le SDK du fabricant
4. **Accédez à la caméra** via la prise en charge GenTL de VisioForge

Cette approche fournit l'accès aux fonctionnalités et optimisations spécifiques au fournisseur tout en conservant la compatibilité avec l'interface GenICam unifiée de VisioForge.

## SDK compatibles des principaux fabricants

Les SDK de fabricants suivants sont connus pour bien fonctionner avec notre intégration :

- [SDK Basler pylon](https://www.baslerweb.com/en/software/pylon/sdk/) — Boîte à outils complète pour les caméras Basler
- [SDK FLIR/Teledyne Spinnaker](https://www.teledynevisionsolutions.com/) — Solution d'imagerie avancée pour les caméras FLIR et Teledyne

## Exemples de code

Les exemples suivants démontrent l'implémentation pratique de caméras GenICam, USB3 Vision et GigE avec le Video Capture SDK de VisioForge et l'intégration GenICam.

### Découverte de base et informations sur les caméras

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

// Découvrir les caméras GenICam disponibles
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

Console.WriteLine($"Found {devices.Length} GenICam devices");

foreach (var device in devices)
{
    Console.WriteLine($"Camera: {device.CameraName}");
    Console.WriteLine($"Device ID: {device.DeviceId}");
    Console.WriteLine($"Address: {device.Address}");
    Console.WriteLine();
}

// Obtenir des informations détaillées sur une caméra spécifique
if (devices.Length > 0)
{
    var cameraDeviceId = devices[0].DeviceId;
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    
    if (camera != null && GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        camera.ReadInfo();
        
        Console.WriteLine($"Connected to: {camera.VendorName} {camera.ModelName}");
        Console.WriteLine($"Serial Number: {camera.SerialNumber}");
        Console.WriteLine($"Protocol: {camera.Protocol}");
        Console.WriteLine($"Sensor Size: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
        Console.WriteLine($"Available Pixel Formats: {string.Join(", ", camera.AvailablePixelFormats)}");
        
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
```

### Aperçu en direct avec VideoCaptureCoreX

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

// Créer une instance VideoCaptureCoreX (en supposant que vous disposez d'un contrôle de vue vidéo)
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Découvrir les caméras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0]; // Utiliser la première caméra
    Console.WriteLine($"Using camera: {selectedDevice.CameraName}");

    // Configurer la caméra avant de démarrer la capture
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Configurer les paramètres de la caméra
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(10000); // Exposition de 10 ms
        }
        
        if (camera.GainAvailable)
        {
            camera.SetGain(0.0); // Gain minimum
        }
        
        // Obtenir la résolution et la fréquence d'images de la caméra
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Créer la source GenICam
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Démarrer l'aperçu
        await videoCapture.StartAsync();
        
        Console.WriteLine("Live preview started. Press any key to stop...");
        Console.ReadKey();
        
        await videoCapture.StopAsync();
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Enregistrement vers un fichier MP4

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

// Créer une instance VideoCaptureCoreX
var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);

try
{
    // Configurer le mode débogage
    videoCapture.Debug_Mode = true;
    videoCapture.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Découvrir et sélectionner la caméra
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0];
    Console.WriteLine($"Recording from camera: {selectedDevice.CameraName}");

    // Configurer les paramètres de la caméra
    var camera = GenICamCameraManager.GetCamera(selectedDevice.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(selectedDevice.DeviceId))
    {
        camera.ReadInfo();
        
        // Définir les paramètres de la caméra
        if (camera.ExposureTimeAvailable)
        {
            camera.SetExposureTime(5000); // Exposition de 5 ms
        }
        
        if (camera.FrameRateAvailable)
        {
            var targetFps = Math.Min(30.0, camera.FrameRateBounds.Max);
            camera.SetFrameRate(new VideoFrameRate(targetFps));
        }

        // Obtenir la résolution et la fréquence d'images de la caméra
        var sensorSize = camera.GetSensorSize();
        var frameRate = camera.GetFrameRate();
        
        // Créer la source GenICam
        var sourceSettings = new GenICamSourceSettings(
            selectedDevice.DeviceId,
            new VisioForge.Core.Types.Rect(0, 0, sensorSize.Width, sensorSize.Height),
            frameRate,
            GenICamPixelFormat.Default
        );
        
        videoCapture.Video_Source = sourceSettings;
        
        // Configurer la sortie MP4
        string outputFilename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4Output(outputFilename, H264EncoderBlock.GetDefaultSettings(), null);
        videoCapture.Outputs_Add(mp4Output);
        
        // Démarrer l'enregistrement
        await videoCapture.StartAsync();
        
        Console.WriteLine($"Recording started to: {outputFilename}");
        Console.WriteLine("Press any key to stop recording...");
        Console.ReadKey();
        
        // Arrêter l'enregistrement
        await videoCapture.StopAsync();
        Console.WriteLine($"Recording saved to: {outputFilename}");
        
        GenICamCameraManager.CloseCamera(selectedDevice.DeviceId);
    }
}
finally
{
    await videoCapture.DisposeAsync();
}
```

### Configuration avancée de la caméra

```csharp
using VisioForge.Core.GenICam;
using VisioForge.Core.Types;
using System;
using System.Linq;
using System.Threading;

// Découvrir et se connecter à la caméra
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
if (devices.Length == 0) return;

var camera = GenICamCameraManager.GetCamera(devices[0].DeviceId);

if (camera != null && GenICamCameraManager.OpenCamera(devices[0].DeviceId))
{
    camera.ReadInfo();
    
    // Afficher les capacités de la caméra
    Console.WriteLine($"Camera: {camera}");
    Console.WriteLine($"Available pixel formats: {string.Join(", ", camera.AvailablePixelFormats)}");
    
    // Configurer le format de pixel
    if (camera.AvailablePixelFormats.Contains("Mono8"))
    {
        camera.SetPixelFormat("Mono8");
        Console.WriteLine("Set pixel format to Mono8");
    }
    
    // Configurer l'exposition avec le mode auto
    if (camera.IsExposureAutoAvailable)
    {
        // Essayer d'abord l'exposition automatique
        camera.SetExposureAuto(GenICamAuto.Once);
        Thread.Sleep(1000); // Attendre la fin de l'exposition automatique
        
        // Puis passer en manuel et lire la valeur auto-calculée
        camera.SetExposureAuto(GenICamAuto.Off);
        var autoExposure = camera.GetExposureTime();
        Console.WriteLine($"Auto exposure calculated: {autoExposure:F2} μs");
        
        // Affiner manuellement si nécessaire
        camera.SetExposureTime(autoExposure * 1.2); // Exposition 20 % plus longue
    }
    
    // Configurer le gain
    if (camera.IsGainAutoAvailable)
    {
        camera.SetGainAuto(GenICamAuto.Continuous);
        Console.WriteLine("Enabled continuous auto gain");
    }
    
    // Configurer le binning pour des fréquences d'images plus élevées
    if (camera.BinningAvailable)
    {
        camera.SetBinning(2, 2); // Binning 2x2
        Console.WriteLine("Set 2x2 binning for higher sensitivity and frame rate");
    }
    
    // Configurer le déclenchement logiciel
    if (camera.SoftwareTriggerSupported)
    {
        camera.SetStringFeature("TriggerMode", "On");
        camera.SetStringFeature("TriggerSource", "Software");
        camera.SetAcquisitionMode(GenICamAcquisitionMode.Continuous);
        
        Console.WriteLine("Configured for software triggering");
        
        // Remarque : avec VideoCaptureCoreX, le déclenchement logiciel serait
        // intégré au pipeline de capture plutôt qu'appelé directement
    }
    
    // Lire et afficher les fonctionnalités avancées
    camera.ReadAvailableFeatures();
    Console.WriteLine($"Camera has {camera.AvailableStringFeatures.Length + camera.AvailableIntegerFeatures.Length + camera.AvailableFloatFeatures.Length + camera.AvailableBooleanFeatures.Length} features");
    Console.WriteLine($"Advanced features available: {camera.HasAdvancedFeatures}");
    
    GenICamCameraManager.CloseCamera(devices[0].DeviceId);
}
```

### Utilisation de GenICamSourceBlock avec un pipeline Media Blocks

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.IO;
using System.Threading.Tasks;

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

// Créer le pipeline Media Blocks
var pipeline = new MediaBlocksPipeline();

try
{
    // Configurer le mode débogage
    pipeline.Debug_Mode = true;
    pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

    // Découvrir les caméras
    var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
    if (devices.Length == 0)
    {
        Console.WriteLine("No GenICam cameras found!");
        return;
    }

    var selectedDevice = devices[0];
    string cameraDeviceId = selectedDevice.DeviceId;

    // Configurer la caméra
    if (GenICamCameraManager.OpenCamera(cameraDeviceId))
    {
        var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
        camera?.ReadInfo();

        // Créer le bloc source GenICam
        var sourceSettings = new GenICamSourceSettings(cameraDeviceId);
        var sourceBlock = new GenICamSourceBlock(sourceSettings);

        // Créer le moteur de rendu vidéo pour l'aperçu
        var videoRenderer = new VideoRendererBlock(pipeline, yourVideoViewControl) { IsSync = false };

        // Créer un bloc tee pour séparer le flux
        var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);

        // Créer le bloc de sortie MP4
        string outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "genicam_capture.mp4");
        var mp4Output = new MP4OutputBlock(new MP4SinkSettings(outputFile), H264EncoderBlock.GetDefaultSettings(), aacSettings: null);

        // Connecter le pipeline
        pipeline.Connect(sourceBlock.Output, videoTee.Input);
        pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
        
        var videoInput = mp4Output.CreateNewInput(MediaBlockPadMediaType.Video);
        pipeline.Connect(videoTee.Outputs[1], videoInput);

        // Démarrer le pipeline
        await pipeline.StartAsync();

        Console.WriteLine($"Recording to: {outputFile}");
        Console.WriteLine("Press any key to stop...");
        Console.ReadKey();

        // Arrêter le pipeline
        await pipeline.StopAsync();
        Console.WriteLine($"Recording saved to: {outputFile}");

        // Nettoyage
        mp4Output.Dispose();
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Gestion des erreurs et récupération

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

string cameraDeviceId = null;
VideoCaptureCoreX videoCapture = null;

try
{
    // Découvrir les caméras avec logique de retry
    int maxDiscoveryRetries = 3;
    var devices = Array.Empty<GenICamCamera>();
    
    for (int attempt = 1; attempt <= maxDiscoveryRetries; attempt++)
    {
        try
        {
            GenICamCameraManager.UpdateDeviceList();
            devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();
            
            if (devices.Length > 0)
            {
                Console.WriteLine($"Found {devices.Length} cameras on attempt {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Discovery attempt {attempt} failed: {ex.Message}");
            if (attempt < maxDiscoveryRetries)
            {
                Thread.Sleep(2000); // Attendre avant de réessayer
            }
        }
    }
    
    if (devices.Length == 0)
    {
        Console.WriteLine("No cameras found after all attempts");
        return;
    }
    
    cameraDeviceId = devices[0].DeviceId;
    
    // Connexion à la caméra avec logique de retry
    int maxRetries = 3;
    bool connected = false;
    
    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            connected = GenICamCameraManager.OpenCamera(cameraDeviceId);
            if (connected)
            {
                Console.WriteLine($"Connected to camera on attempt {attempt}");
                break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Connection attempt {attempt} failed: {ex.Message}");
            if (attempt < maxRetries)
            {
                Thread.Sleep(2000); // Attendre avant de réessayer
            }
        }
    }
    
    if (!connected)
    {
        Console.WriteLine("Failed to connect after all attempts");
        return;
    }
    
    // Configurer la caméra
    var camera = GenICamCameraManager.GetCamera(cameraDeviceId);
    camera?.ReadInfo();
    
    // Créer VideoCaptureCoreX avec gestion d'erreurs
    videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
    
    // Configurer le gestionnaire d'événements d'erreur
    videoCapture.OnError += (sender, e) =>
    {
        Console.WriteLine($"Capture error: {e.Message}");
    };
    
    // Configurer la source
    var sourceSettings = new GenICamSourceSettings(
        cameraDeviceId,
        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
        camera.GetFrameRate(),
        GenICamPixelFormat.Default
    );
    
    videoCapture.Video_Source = sourceSettings;
    
    // Démarrer la capture avec surveillance
    await videoCapture.StartAsync();
    
    Console.WriteLine("Capture started. Monitoring for errors...");
    
    // Surveiller pendant 30 secondes
    var startTime = DateTime.Now;
    while ((DateTime.Now - startTime).TotalSeconds < 30)
    {
        Thread.Sleep(1000);
        
        // Vérifier l'état de la capture
        if (videoCapture.State != VisioForge.Core.Types.PlaybackState.Play)
        {
            Console.WriteLine("Capture stopped unexpectedly. Attempting restart...");
            
            try
            {
                await videoCapture.StopAsync();
                await Task.Delay(1000);
                await videoCapture.StartAsync();
                Console.WriteLine("Capture restarted successfully");
            }
            catch (Exception restartEx)
            {
                Console.WriteLine($"Failed to restart capture: {restartEx.Message}");
                break;
            }
        }
    }
    
    await videoCapture.StopAsync();
    Console.WriteLine("Capture monitoring completed");
}
catch (Exception ex)
{
    Console.WriteLine($"Unexpected error: {ex.Message}");
}
finally
{
    // Nettoyer
    if (videoCapture != null)
    {
        await videoCapture.DisposeAsync();
    }
    
    if (!string.IsNullOrEmpty(cameraDeviceId))
    {
        GenICamCameraManager.CloseCamera(cameraDeviceId);
    }
    
    Console.WriteLine("Resources cleaned up");
}
```

### Travailler avec des caméras GenTL

```csharp
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.GenICam;
using VisioForge.Core;
using System;
using System.Threading.Tasks;

// Pour les caméras GenTL, assurez-vous que la variable d'environnement est définie
// GENICAM_GENTL64_PATH doit pointer vers la bibliothèque du producteur GenTL

// Exemple : définir au démarrage de l'application ou dans l'environnement
Environment.SetEnvironmentVariable("GENICAM_GENTL64_PATH", @"C:\Program Files\Basler\pylon 7\Runtime\x64");

// Initialiser le SDK
await VisioForgeX.InitSDKAsync();

// Découvrir les caméras GenTL (elles apparaîtront aux côtés des autres périphériques GenICam)
GenICamCameraManager.UpdateDeviceList();
var devices = await DeviceEnumerator.Shared.GenICamSourcesAsync();

foreach (var device in devices)
{
    // Vérifier les informations de la caméra
    var camera = GenICamCameraManager.GetCamera(device.DeviceId);
    if (camera != null && GenICamCameraManager.OpenCamera(device.DeviceId))
    {
        camera.ReadInfo();
        
        // Vérifier s'il s'agit d'un périphérique GenTL
        if (camera.Protocol == "GenTL")
        {
            Console.WriteLine($"Found GenTL camera: {camera}");
            
            try
            {
                // Configurer les fonctionnalités spécifiques à GenTL pour des performances maximales
                if (camera.IsFeatureAvailable("StreamBufferCountMode"))
                {
                    camera.SetStringFeature("StreamBufferCountMode", "Manual");
                }
                
                if (camera.IsFeatureAvailable("StreamBufferCountManual"))
                {
                    camera.SetIntegerFeature("StreamBufferCountManual", 20); // Plus de tampons
                }
                
                // Définir les paramètres d'acquisition
                if (camera.ExposureTimeAvailable)
                {
                    camera.SetExposureTime(1000); // Exposition de 1 ms
                }
                
                // Utiliser avec VideoCaptureCoreX
                var videoCapture = new VideoCaptureCoreX(videoView: yourVideoViewControl);
                
                try
                {
                    var sourceSettings = new GenICamSourceSettings(
                        device.DeviceId,
                        new VisioForge.Core.Types.Rect(0, 0, camera.SensorSize.Width, camera.SensorSize.Height),
                        camera.GetFrameRate(),
                        GenICamPixelFormat.Default
                    );
                    
                    videoCapture.Video_Source = sourceSettings;
                    
                    // Démarrer l'aperçu
                    await videoCapture.StartAsync();
                    Console.WriteLine($"GenTL camera preview started: {camera.SensorSize.Width}x{camera.SensorSize.Height}");
                    
                    // Laisser tourner quelques secondes
                    await Task.Delay(3000);
                    
                    await videoCapture.StopAsync();
                    Console.WriteLine("GenTL camera preview stopped");
                }
                finally
                {
                    await videoCapture.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error using GenTL camera: {ex.Message}");
            }
        }
        
        GenICamCameraManager.CloseCamera(device.DeviceId);
    }
}
```

## Exemples de projets

Pour des exemples d'intégration complets et des projets, explorez ces implémentations spécifiques à GenICam :

- **[Démo de capture GenICam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/GenICam%20Capture)** — Application WPF complète démontrant l'intégration de caméra GenICam avec VideoCaptureCoreX
- **[Démo Media Blocks GenICam Source](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)** — Implémentation avancée d'un pipeline Media Blocks utilisant des sources GenICam

Pour des exemples d'intégration et des projets supplémentaires, visitez notre [dépôt d'exemples GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour explorer plus d'exemples de code sur différentes plateformes et cas d'usage.
