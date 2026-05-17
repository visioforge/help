---
title: Video Capture SDK pour C# .NET — Webcam, écran, caméra IP
description: Enregistrez webcam, écran et caméra IP (RTSP/ONVIF) en MP4 avec VisioForge Video Capture. API asynchrone, encodage GPU pour WinForms, WPF, MAUI.
sidebar_label: Video Capture SDK .NET
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
  - MP4Output
  - DeviceEnumerator
  - VideoCaptureCoreX
  - VideoCaptureDeviceSourceSettings
  - VideoView

---

# Video Capture SDK pour C# .NET — API de capture webcam, écran et caméra IP

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Le Video Capture SDK pour .NET est une API de capture vidéo C# qui vous permet d'enregistrer depuis des webcams, des caméras IP (RTSP/ONVIF) et des écrans dans vos applications .NET. Il remplace le code de capture vidéo DirectShow bas niveau par une API asynchrone moderne — énumérez les périphériques, configurez les sources et démarrez l'enregistrement en quelques lignes de C#.

Le SDK prend en charge l'énumération des périphériques, la négociation des formats, l'encodage accéléré par GPU et la sortie fichier sur Windows, macOS et Linux. Que vous ayez besoin d'enregistrer un flux RTSP dans un fichier, de capturer des photos depuis une webcam ou de créer un outil de capture d'écran, l'API couvre tout cela.

## Démarrage rapide

### 1. Installer les paquets NuGet

```bash
dotnet add package VisioForge.DotNet.VideoCapture
```

Ajoutez les dépendances natives spécifiques à la plateforme :

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

Pour la liste complète des paquets et la prise en charge des frameworks UI (WinForms, WPF, MAUI, Avalonia), consultez le [Guide d'installation](../install/index.md).

### 2. Initialiser le SDK

Appelez `InitSDKAsync()` une seule fois au démarrage de l'application avant d'utiliser la moindre fonctionnalité de capture :

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Capture C# de webcam vers MP4 (exemple minimal)

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// Créer l'instance de capture liée à un contrôle VideoView
var capture = new VideoCaptureCoreX(videoView);

// Énumérer les périphériques disponibles
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Configurer les sources vidéo et audio
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// Ajouter la sortie MP4 (H.264 + AAC, accélérée par GPU lorsque disponible)
capture.Outputs_Add(new MP4Output("recording.mp4"));

// Démarrer la capture
await capture.StartAsync();

// ... une fois terminé :
await capture.StopAsync();
await capture.DisposeAsync();
```

### 4. Nettoyage à l'arrêt

```csharp
VisioForgeX.DestroySDK();
```

## Flux de travail principal

Toute application de capture suit le même modèle :

1. **Initialiser le SDK** — `VisioForgeX.InitSDKAsync()` (une fois par cycle de vie d'application)
2. **Énumérer les périphériques** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`
3. **Créer l'objet de capture** — `new VideoCaptureCoreX(videoView)` pour l'aperçu, ou sans vue pour un enregistrement sans interface
4. **Configurer la source** — Définir `Video_Source` sur une webcam, une caméra IP, un écran ou une autre source
5. **Configurer la sortie** — Ajouter une ou plusieurs sorties : `MP4Output`, `WebMOutput`, `AVIOutput`, etc.
6. **Démarrer** — `await capture.StartAsync()`
7. **Arrêter** — `await capture.StopAsync()` finalise le fichier de sortie
8. **Libérer** — `await capture.DisposeAsync()` libère toutes les ressources

## Scénarios courants de capture vidéo en C#

### Enregistrer une vidéo webcam en C#

Enregistrez depuis une webcam USB ou une caméra intégrée en MP4 avec H.264/AAC :

```csharp
capture.Video_Source = new VideoCaptureDeviceSourceSettings(device);
capture.Outputs_Add(new MP4Output("webcam.mp4"));
```

Consultez le tutoriel complet : [Enregistrer une vidéo webcam en C#](guides/save-webcam-video.md)

### Enregistrer un flux RTSP dans un fichier en C#

Connectez-vous à des caméras IP et enregistrez des flux RTSP dans des fichiers MP4. Prend en charge les protocoles RTSP, ONVIF et HTTP :

```csharp
// RTSPSourceSettings a un constructeur privé — construisez via la fabrique asynchrone
// pour que la source puisse sonder les codecs avant le démarrage du pipeline.
capture.Video_Source = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.100:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);
capture.Outputs_Add(new MP4Output("ipcam.mp4"));
```

Consultez : [Caméra IP vers MP4](video-tutorials/ip-camera-capture-mp4.md) | [Guide RTSP](video-sources/ip-cameras/rtsp.md) | [Guide ONVIF](video-sources/ip-cameras/onvif.md)

### Capture d'écran C# vers MP4

Enregistrez le bureau ou une région d'écran spécifique en MP4 :

```csharp
// La capture d'écran avec le moteur X sous Windows utilise ScreenCaptureD3D11SourceSettings
// (autres plateformes : ScreenCaptureGDISourceSettings, ScreenCaptureMacOSSourceSettings,
// ScreenCaptureXDisplaySourceSettings — toutes implémentent IVideoCaptureBaseVideoSourceSettings).
capture.Video_Source = new ScreenCaptureD3D11SourceSettings
{
    FrameRate     = new VideoFrameRate(30),
    CaptureCursor = true,
};
capture.Outputs_Add(new MP4Output("screen.mp4"));
```

Consultez : [Capture d'écran vers MP4](video-tutorials/screen-capture-mp4.md)

### Enregistrement audio uniquement

Enregistrez depuis un microphone sans vidéo :

```csharp
capture.Video_Source = null; // désactiver la vidéo pour une capture audio uniquement
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;
capture.Outputs_Add(new MP4Output("audio.mp4"));
```

### Capturer une photo depuis une webcam

Récupérez une image fixe depuis le flux vidéo en direct de la webcam :

```csharp
capture.Snapshot_Grabber_Enabled = true;
await capture.StartAsync();

// Plus tard, enregistrez une image :
await capture.Snapshot_SaveAsync("photo.jpg", SKEncodedImageFormat.Jpeg, 85);
```

Consultez : [Capture photo webcam](guides/make-photo-using-webcam.md)

## Périphériques d'entrée pris en charge

### Caméras et matériel de capture

* Webcams USB et périphériques de capture (jusqu'à 4K)
* Caméscopes DV et HDV MPEG-2
* Cartes de capture PCI professionnelles
* Tuners TV (avec ou sans encodeur MPEG)

### Caméras réseau et IP

* Caméras IP compatibles RTSP
* Caméras HTTP JPEG/MJPEG
* Sources de streaming RTMP
* Caméras compatibles ONVIF
* Sources SRT et NDI

Consultez notre [répertoire des marques de caméras IP](../camera-brands/index.md) pour les URL RTSP spécifiques à chaque marque et les guides de connexion couvrant plus de 60 fabricants.

### Équipements professionnels et industriels

* Périphériques de capture Blackmagic Decklink
* Microsoft Kinect et Kinect 2
* Caméras industrielles GenICam / GigE Vision / USB3 Vision

### Sources audio

* Périphériques de capture audio système et cartes son
* Périphériques audio ASIO professionnels
* Capture audio en loopback (son système)

## Formats de sortie et encodeurs

Le SDK prend en charge plusieurs conteneurs et codecs de sortie. L'encodage accéléré par GPU (NVIDIA NVENC, AMD AMF, Intel Quick Sync) est utilisé automatiquement lorsqu'il est disponible.

| Format | Codecs vidéo | Codecs audio | Cas d'usage |
| ------ | ------------ | ------------ | -------- |
| MP4 | H.264, H.265 (HEVC) | AAC | Usage général, compatibilité maximale |
| WebM | VP8, VP9, AV1 | Vorbis, Opus | Diffusion web, libre de droits |
| AVI | MJPEG, codecs personnalisés | PCM, MP3 | Compatibilité héritée |
| MKV | H.264, H.265, VP9 | AAC, Vorbis, FLAC | Conteneur flexible, pistes multiples |
| GIF | GIF animé | — | Clips courts, aperçus |

Pour la configuration détaillée des codecs, consultez [Formats de sortie](../general/output-formats/mp4.md) et [Encodeurs vidéo](../general/video-encoders/h264.md).

## Prise en charge des plateformes

| Plateforme | Frameworks UI | Notes |
| -------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | Jeu de fonctionnalités complet incluant les sources DirectShow |
| macOS | MAUI, Avalonia, Console | Accès caméra AVFoundation |
| Linux x64 | Avalonia, Console | Accès caméra V4L2 |
| Android | MAUI | Via l'intégration caméra MAUI |
| iOS | MAUI | Via l'intégration caméra MAUI |

## Migration depuis la capture vidéo DirectShow

Si vous utilisez actuellement DirectShow (DirectShow.NET) pour la capture vidéo en C#, le Video Capture SDK fournit un remplacement moderne. Au lieu de construire manuellement des graphes de filtres avec `IGraphBuilder`, de connecter des pins et de gérer des objets COM, vous utilisez des classes de paramètres typées et des méthodes asynchrones.

| Concept DirectShow | Équivalent Video Capture SDK |
| ------------------ | ---------------------------- |
| Graphe de filtres `IGraphBuilder` | Géré automatiquement par `VideoCaptureCoreX` |
| `ICaptureGraphBuilder2` | Non requis — source et sortie configurées via des propriétés |
| Énumération de périphériques via `DsDevice` | `DeviceEnumerator.Shared.VideoSourcesAsync()` |
| `ISampleGrabber` pour l'accès aux images | `Snapshot_SaveAsync()` ou événement `OnVideoFrameBuffer` |
| Connexion manuelle des pins | Automatique — ou utilisez [Media Blocks SDK](../mediablocks/GettingStarted/index.md) pour des pipelines explicites |
| Nettoyage COM / `Marshal.ReleaseComObject` | Modèle `IAsyncDisposable` standard |

Pour un guide de migration détaillé avec des exemples de code côte à côte, consultez [Migrer depuis DirectShow.NET](https://www.visioforge.com/compare/migrate-from-directshow-net).

## Documentation pour développeurs

### Fonctionnalités principales

* [Configuration des sources vidéo](video-sources/index.md)
* [Configuration des sources audio](audio-sources/index.md)
* [Traitement vidéo et effets](video-processing/index.md)
* [Rendu audio](audio-rendering/index.md)

### Fonctionnalités avancées

* [Implémentation de la capture vidéo](video-capture/index.md)
* [Implémentation de la capture audio](audio-capture/index.md)
* [Détection de mouvement](motion-detection/index.md)
* [Streaming réseau](network-streaming/index.md)
* [Enregistrement pré-événement](guides/pre-event-recording.md)
* [Synchronisation de plusieurs captures](guides/start-in-sync.md)

### Tutoriels (pas à pas avec code)

* [Webcam vers MP4](video-tutorials/video-capture-webcam-mp4.md) | [vers AVI](video-tutorials/video-capture-camera-avi.md) | [vers WMV](video-tutorials/video-capture-webcam-wmv.md)
* [Capture d'écran vers MP4](video-tutorials/screen-capture-mp4.md) | [vers AVI](video-tutorials/screen-capture-avi.md) | [vers WMV](video-tutorials/screen-capture-wmv.md)
* [Aperçu caméra IP](video-tutorials/ip-camera-preview.md) | [Caméra IP vers MP4](video-tutorials/ip-camera-capture-mp4.md)
* [Superposition de texte sur webcam](video-tutorials/webcam-capture-text-overlay.md)
* [Aperçu vidéo caméra](video-tutorials/camera-video-preview.md)
* [Tous les tutoriels vidéo](video-tutorials/index.md)

### Guides

* [Enregistrer une vidéo webcam en C#](guides/save-webcam-video.md)
* [Enregistrement webcam en VB.NET](guides/record-webcam-vb-net.md)
* [Capture d'écran en VB.NET](guides/screen-capture-vb-net.md)
* [Capture photo webcam](guides/make-photo-using-webcam.md)
* [Tous les guides](guides/index.md)

### Intégration

* [Intégration de logiciels tiers](3rd-party-software/index.md)
* [Vision par ordinateur (détection de visages)](computer-vision/index.md)
* [Enregistrement caméra MAUI](maui/camera-recording-maui.md)

## Ressources pour développeurs

* [Exemples de code sur GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Directives de déploiement](deployment.md)
* [Référence API](https://api.visioforge.org/dotnet/api/index.html)
* [Journal des modifications](../changelog.md)
* [Contrat de licence utilisateur final](../../eula.md)
* [Informations de licence](../../../licensing.md)
