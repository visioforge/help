---
title: VisioForge Video Capture SDK en C# .NET — Aide-mémoire
description: Référence d'une page pour Video Capture SDK .Net — paquets NuGet, API VideoCaptureCoreX, exemple d'enregistrement MP4, plateformes et pièges.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - Capture
  - Recording
  - Streaming
  - Encoding
  - Webcam
  - IP Camera
  - Screen Capture
  - RTSP
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - VideoCaptureDeviceSourceSettings
  - DeviceEnumerator
  - VideoView
  - MP4Output
---

# VisioForge Video Capture SDK .Net — Aide-mémoire

Video Capture SDK .Net capture depuis des webcams, des caméras IP (RTSP/ONVIF), des écrans, des Decklink et des appareils GenICam/GigE Vision sur les 5 plateformes .NET (Windows, macOS, Linux, Android, iOS). `VideoCaptureCoreX` est le moteur multiplateforme ; les sources se configurent via des classes de paramètres typées comme `VideoCaptureDeviceSourceSettings`, et les périphériques sont énumérés de façon asynchrone avec `DeviceEnumerator`.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prise en charge des plateformes

- **Windows x64 / x86** — WinForms, WPF, MAUI, Avalonia, Console. Sources DirectShow complètes + encodage matériel NVENC / AMF / Intel Quick Sync.
- **macOS / macCatalyst** — MAUI, Avalonia, Console. Sources caméra AVFoundation + encodage matériel VideoToolbox.
- **Linux x64** — Avalonia, Console. Sources caméra V4L2 + encodage matériel NVENC / VA-API.
- **Android** — MAUI. Sources caméra natives + encodage matériel MediaCodec.
- **iOS** — MAUI. Sources caméra AVFoundation + encodage matériel VideoToolbox.

Pour la matrice complète codec × source × plateforme, consultez [platform-matrix.md](../platform-matrix.md).

## Paquets NuGet

Paquet principal du SDK (obligatoire, toutes plateformes) :

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.*" />
```

Runtimes natifs spécifiques à chaque plateforme — ajoutez ceux que vous ciblez :

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- Windows x86 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86" Version="2025.11.0" />

<!-- macOS / macCatalyst -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.2.15" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />

<!-- Android / iOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.24" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
```

Intégration de l'interface utilisateur (choisissez celle qui correspond à votre pile UI) :

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

Les contrôles `VideoView` pour WinForms et WPF sont inclus dans le paquet principal. Pour la liste complète des paquets (y compris les redistribuables du moteur `VideoCaptureCore` réservé à Windows), consultez le [Guide d'installation](../install/index.md).

## Classes API principales

| Classe | Rôle | Voir aussi |
|---|---|---|
| `VideoCaptureCoreX` | Moteur de capture multiplateforme — possède le pipeline, la source, les sorties, l'aperçu | [Démarrage rapide](./index.md) |
| `VideoCaptureDeviceSourceSettings` | Configure une source de webcam / USB / caméra DirectShow | [Énumérer et sélectionner un périphérique](./video-sources/video-capture-devices/enumerate-and-select.md) |
| `DeviceEnumerator` | Énumération asynchrone des sources vidéo, sources audio et sorties audio via `DeviceEnumerator.Shared` | [Vue d'ensemble des sources vidéo](./video-sources/index.md) |
| `VideoView` | Contrôle d'aperçu WinForms / WPF / MAUI / Avalonia lié au moteur de capture | [Installation — contrôles vidéo](../install/index.md) |
| `MP4Output` | Multiplexe les flux capturés en `.mp4` avec H.264 / H.265 + AAC (accéléré GPU lorsque disponible) | [Tutoriel webcam → MP4](./video-tutorials/video-capture-webcam-mp4.md) |

Classes de paramètres de source supplémentaires : `RTSPSourceSettings` (caméras IP — construire via `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)`, et non `new`), `ScreenCaptureD3D11SourceSettings` / `ScreenCaptureGDISourceSettings` / `ScreenCaptureDX9SourceSettings` (écran Windows), `ScreenCaptureMacOSSourceSettings` (macOS), `ScreenCaptureXDisplaySourceSettings` (Linux), `DecklinkVideoSourceSettings`, `GenICamSourceSettings`, `NDISourceSettings`.

## Exemple minimal canonique

Enregistrer la première webcam + le microphone par défaut vers `recording.mp4` :

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// 1. Initialiser le SDK une seule fois au démarrage de l'application.
await VisioForgeX.InitSDKAsync();

// 2. Énumérer les périphériques connectés (asynchrone — ne jamais bloquer le thread UI).
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

if (videoDevices.Length == 0)
    throw new InvalidOperationException("No video capture devices found.");

// 3. Créer le moteur de capture, lié à un VideoView pour l'aperçu en direct.
//    Passer null à la place de videoView pour un enregistrement sans interface.
var capture = new VideoCaptureCoreX(videoView);

// 4. Configurer les sources vidéo + audio.
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// 5. Ajouter la sortie MP4 AVANT d'appeler StartAsync.
capture.Outputs_Add(new MP4Output("recording.mp4"));

// 6. Démarrer la capture — l'aperçu apparaît immédiatement dans videoView.
await capture.StartAsync();

// ... plus tard, lorsque l'utilisateur clique sur Stop :
await capture.StopAsync();     // Finalise et ferme le fichier MP4.
await capture.DisposeAsync();  // Libère le pipeline.

// 7. À l'arrêt de l'application :
VisioForgeX.DestroySDK();
```

Remplacez `VideoCaptureDeviceSourceSettings(...)` par `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` pour les caméras IP, ou par `new ScreenCaptureD3D11SourceSettings()` pour l'enregistrement du bureau (Windows) — le reste du pipeline reste identique.

## Flux de travail typique

1. **Initialisation** — `await VisioForgeX.InitSDKAsync()` (une fois par application).
2. **Énumération** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`.
3. **Configuration de la source** — choisir une classe de paramètres (`VideoCaptureDeviceSourceSettings`, `RTSPSourceSettings`, `ScreenCaptureD3D11SourceSettings` / `ScreenCaptureMacOSSourceSettings` / `ScreenCaptureXDisplaySourceSettings`, `DecklinkVideoSourceSettings`, ...) et l'assigner à `capture.Video_Source`.
4. **Création du moteur** — `new VideoCaptureCoreX(videoView)` pour l'aperçu, ou `new VideoCaptureCoreX(null)` sans interface.
5. **Ajout des sorties** — `capture.Outputs_Add(new MP4Output(path))`, ou `MKVOutput` / `WebMOutput` / `AVIOutput` / `RTMPOutput` / `HLSOutput`.
6. **Exécution** — `StartAsync()` → `StopAsync()` → `DisposeAsync()`, puis `VisioForgeX.DestroySDK()` à l'arrêt.

## Pièges fréquents

- **L'énumération des périphériques est asynchrone.** Utilisez toujours `await DeviceEnumerator.Shared.VideoSourcesAsync()` — appeler `.Result` ou `.Wait()` sur le thread UI peut provoquer un interblocage. L'audio utilise ses propres appels `AudioSourcesAsync()` / `AudioOutputsAsync()` ; l'énumérateur vidéo ne les couvre pas.
- **Les sorties doivent être ajoutées avant `StartAsync`.** `Outputs_Add(...)` pendant une capture active n'est pas pris en charge — arrêtez la capture, ajoutez la sortie, puis redémarrez.
- **La caméra virtuelle nécessite un enregistrement supplémentaire.** Le paquet redistribuable `VirtualCamera` doit être enregistré (consultez le [guide de déploiement](./deployment.md)) avant que la caméra virtuelle ne devienne visible pour des applications externes comme Zoom ou Teams.
- **Les applications AnyCPU nécessitent à la fois les redistribuables x86 et x64.** Sous Windows, distribuez les deux architectures ou imposez-en une via `<PlatformTarget>` — un déploiement partiel échoue silencieusement à l'initialisation de la source.
- **Les permissions mobiles doivent être accordées avant l'énumération.** Sur Android et iOS, les permissions de caméra et de microphone doivent être demandées et accordées avant l'exécution de `DeviceEnumerator` — sinon la liste retournée est vide. Consultez le [guide d'enregistrement caméra MAUI](./maui/camera-recording-maui.md) pour la configuration des permissions par plateforme.

## Voir aussi

- **Sources vidéo**
    - Webcam / caméra USB → [Énumérer et sélectionner un périphérique](./video-sources/video-capture-devices/enumerate-and-select.md)
    - Caméra IP → [RTSP](./video-sources/ip-cameras/rtsp.md), [ONVIF](./video-sources/ip-cameras/onvif.md), [NDI](./video-sources/ip-cameras/ndi.md)
    - Capture d'écran → [screen.md](./video-sources/screen.md)
    - Decklink / GenICam → [decklink.md](./video-sources/decklink.md), [USB3 Vision / GigE / GenICam](./video-sources/usb3v-gige-genicam/index.md)
- **Sorties et streaming**
    - Enregistrement MP4 → [Webcam → MP4](./video-tutorials/video-capture-webcam-mp4.md), [Caméra IP → MP4](./video-tutorials/ip-camera-capture-mp4.md)
    - Streaming réseau (RTMP / RTSP / HLS / SRT / NDI) → [network-streaming](./network-streaming/index.md)
- **Plateforme et déploiement**
    - Windows / macOS / Ubuntu / Android / iOS → [../deployment-x/](../deployment-x/index.md)
    - Installation et frameworks cibles → [../install/index.md](../install/index.md)
    - Matrice complète codec × source → [../platform-matrix.md](../platform-matrix.md)
- **MAUI**
    - Enregistrement caméra multiplateforme → [Enregistrement caméra MAUI](./maui/camera-recording-maui.md)

## FAQ

### Comment lister les caméras connectées ?

`var cams = await DeviceEnumerator.Shared.VideoSourcesAsync();` — retourne une liste de `VideoCaptureDeviceInfo` contenant des noms conviviaux, les formats pris en charge et les fréquences d'images.

### Ce SDK prend-il en charge les caméras IP ?

Oui. Construisez les paramètres via la fabrique asynchrone — `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` — et passez-les à `VideoCaptureCoreX.Video_Source`. Le constructeur étant privé, `new RTSPSourceSettings(...)` ne compilera pas. Les caméras ONVIF et HTTP-JPEG sont également prises en charge. Consultez le [répertoire des marques de caméras IP](../camera-brands/index.md) pour les URL spécifiques aux fabricants.

### Vers quels formats de fichier puis-je enregistrer ?

MP4 (H.264 / H.265 + AAC), MKV, WebM (VP8 / VP9 / AV1 + Opus / Vorbis), AVI, GIF, ainsi que le streaming en direct RTMP / RTSP / HLS / SRT / NDI. Consultez les tableaux de formats de sortie dans [la vue d'ensemble du SDK](./index.md).
