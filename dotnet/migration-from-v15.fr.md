---
title: Guide de migration — SDK .NET VisioForge v15 vers v2026
description: Guide pas à pas pour migrer du SDK .NET VisioForge v15 vers v2026, couvrant les parcours DirectShow et moteurs X multiplateformes.
sidebar_label: Migration depuis v15
order: 25
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Streaming
  - Editing
primary_api_classes:
  - MP4Output
  - DeviceEnumerator
  - VideoCaptureCore
  - VideoEditCore
  - MediaPlayerCore

---

# Guide de migration : v15 vers v2026

Ce guide vous aide à mettre à niveau votre application depuis le SDK .NET VisioForge v15.x vers v2026.x. Le SDK v2026 contient **deux types de moteurs**, et vous pouvez choisir le parcours de migration qui correspond le mieux à vos besoins :

- **Parcours A : rester sur DirectShow** — Modifications de code minimales, même architecture Windows uniquement. Idéal pour les applications de production qui ont besoin d'une mise à niveau rapide et à faible risque.
- **Parcours B : migrer vers les moteurs X** — Modifications d'API significatives, mais vous gagnez la prise en charge multiplateforme, des codecs modernes et de nouvelles fonctionnalités. Idéal pour les applications planifiant une mise à jour majeure.

> **Important :** Les classes DirectShow héritées (`VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`) sont **toujours entièrement prises en charge et activement mises à jour** dans v2026. Vous n'avez pas besoin de migrer immédiatement vers les moteurs X. Nous recommandons d'abord le Parcours A, puis de migrer vers les moteurs X lorsque votre calendrier le permettra.

## Modifications des frameworks cibles

Le tableau suivant présente les frameworks cibles pris en charge dans v2026 :

| Framework | Moteurs DirectShow | Moteurs X |
|-----------|-------------------|-----------|
| .NET Framework 4.6.1+ | Pris en charge | Pris en charge |
| .NET Core 3.1 | Pris en charge | Pris en charge |
| .NET 5.0 / 6.0 / 7.0 | Pris en charge | Pris en charge |
| .NET 8.0 | Pris en charge | Pris en charge |
| .NET 9.0 | Pris en charge | Pris en charge |
| .NET 10.0 | Pris en charge | Pris en charge |

> **Note :** Les classes DirectShow nécessitent un TFM Windows (par exemple `net10.0-windows`) ou .NET Framework. Pour les moteurs X, des TFM spécifiques à la plateforme contrôlent la cible : `net10.0-windows` pour Windows, `net10.0-macos` pour macOS, `net10.0-ios` pour iOS, `net10.0-android` pour Android. Le simple `net10.0` (sans suffixe de plateforme) cible uniquement Linux.

---

## Parcours A : DirectShow v15 → DirectShow v2026 (impact minimal)

Ce parcours conserve votre code DirectShow existant avec des modifications minimales.

### Modifications des paquets NuGet

| Paquet v15 | Paquet v2026 |
|-------------|---------------|
| `VisioForge.DotNet.Core` | `VisioForge.DotNet.Core` (même nom, nouvelle version) |

Des paquets spécifiques au produit sont également disponibles :

- `VisioForge.DotNet.VideoCapture`
- `VisioForge.DotNet.VideoEdit`
- `VisioForge.DotNet.MediaPlayer`

### Modifications des espaces de noms

**Aucune modification d'espace de noms requise.** Tous les espaces de noms DirectShow restent identiques :

| Espace de noms | Statut |
|-----------|--------|
| `VisioForge.Core.VideoCapture` | Inchangé |
| `VisioForge.Core.VideoEdit` | Inchangé |
| `VisioForge.Core.MediaPlayer` | Inchangé |
| `VisioForge.Core.Types` | Inchangé |
| `VisioForge.Core.Types.VideoCapture` | Inchangé |
| `VisioForge.Core.Types.Output` | Inchangé |
| `VisioForge.Core.Types.VideoEffects` | Inchangé |
| `VisioForge.Core.Types.AudioEffects` | Inchangé |
| `VisioForge.Core.Types.Events` | Inchangé |
| `VisioForge.Core.Types.MediaPlayer` | Inchangé |
| `VisioForge.Core.Types.VideoEdit` | Inchangé |

### Modifications d'API

L'API DirectShow est largement inchangée entre v15 et v2026 :

- **Aucune initialisation du SDK requise** (identique à v15)
- Mêmes noms de classes : `VideoCaptureCore`, `VideoEditCore`, `MediaPlayerCore`
- Mêmes schémas de création : `new VideoCaptureCore(videoView)`, `new MediaPlayerCore(videoView)`, `new VideoEditCore(videoView)`
- Même énumération des périphériques : `VideoCapture1.Video_CaptureDevices()`, `.Audio_CaptureDevices()`
- Même configuration de sortie : `VideoCapture1.Output_Format = mp4Output;`
- Même définition de mode : `VideoCapture1.Mode = VideoCaptureMode.VideoCapture;`
- Mêmes noms d'événements : `OnError`, `OnStop`, etc.

### Dépannage

| Problème | Solution |
|---------|----------|
| L'espace de noms `VisioForge.Core.Types` est introuvable | Vérifiez que le paquet NuGet approprié est installé (`VisioForge.DotNet.Core` ou un paquet spécifique au produit) |
| Types ou classes manquants | Pour .NET 8+, assurez-vous que votre TFM inclut `-windows` (par exemple `net8.0-windows`, pas `net8.0`). Pour .NET Framework, utilisez `net461` ou `net472`. |
| `VideoView` WPF introuvable | Le projet doit cibler un TFM Windows (par exemple `net8.0-windows`) avec `<UseWPF>true</UseWPF>` |

---

## Parcours B : migrer vers les moteurs X (multiplateformes)

Ce parcours migre des classes DirectShow vers les nouvelles classes de moteurs X multiplateformes. Il s'agit d'un changement plus important, mais il apporte des avantages majeurs.

### Pourquoi migrer vers les moteurs X ?

- **Multiplateforme** : Windows, macOS, Linux, iOS, Android
- **Codecs modernes** : AV1, HEVC avec encodage matériel (NVIDIA NVENC, AMD AMF, Intel QSV)
- **Sorties multiples simultanées** : enregistrez en MP4 et diffusez en RTMP en même temps
- **Nouveaux protocoles de streaming** : SRT, RIST, WebRTC WHIP, HLS, DASH
- **API async-first** : prise en charge complète d'async/await partout
- **Pipeline MediaBlocks** : construisez des pipelines de traitement multimédia personnalisés avec des blocs composables

### Paquets NuGet

Vous avez besoin de paquets supplémentaires pour les moteurs X :

```xml
<!-- SDK principal -->
<PackageReference Include="VisioForge.DotNet.Core" Version="2026.*" />

<!-- Runtime de plateforme (requis pour les moteurs X) -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2026.*" />

<!-- Paquet d'interface utilisateur (choisissez-en un selon votre framework UI) -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WPF" Version="2026.*" />
<!-- OU -->
<PackageReference Include="VisioForge.DotNet.Core.UI.WinForms" Version="2026.*" />
<!-- OU -->
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<!-- OU -->
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

### Initialisation du SDK (requise pour les moteurs X)

Les moteurs X nécessitent une initialisation explicite au démarrage de l'application et un nettoyage à la sortie. **Ceci n'est pas requis pour les classes DirectShow héritées.**

```csharp
// Au démarrage de l'application (par exemple Window_Loaded, OnStartup)
await VisioForgeX.InitSDKAsync();

// À la fermeture de l'application (par exemple Window_Closing, OnExit)
VisioForgeX.DestroySDK();
```

> **Avertissement :** Si le SDK n'est pas correctement désinitialisé, l'application peut se figer à la sortie.

### Migration des espaces de noms

| Espace de noms v15 (DirectShow) | Espace de noms v2026 (moteur X) |
|---|---|
| `VisioForge.Core.VideoCapture` | `VisioForge.Core.VideoCaptureX` |
| `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| `VisioForge.Core.MediaPlayer` | `VisioForge.Core.MediaPlayerX` |
| `VisioForge.Core.Types.VideoCapture` | `VisioForge.Core.Types.X.Sources` + `VisioForge.Core.Types.X.VideoCapture` |
| `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| `VisioForge.Core.Types.AudioEffects` | `VisioForge.Core.Types.X.AudioEffects` |
| `VisioForge.Core.Types.Events` | `VisioForge.Core.Types.Events` (inchangé) |
| `VisioForge.Core.Types.MediaPlayer` | `VisioForge.Core.Types.X.Sources` |
| `VisioForge.Core.Types.VideoEdit` | `VisioForge.Core.Types.X.VideoEdit` |
| — (nouveau) | `VisioForge.Core.Types.X.AudioRenderers` |
| — (nouveau) | `VisioForge.Core.Types.X.VideoEncoders` |
| — (nouveau) | `VisioForge.Core.Types.X.AudioEncoders` |
| — (nouveau) | `VisioForge.Core.Types.X.Sinks` |
| — (nouveau) | `VisioForge.Core.MediaBlocks` |

### Migration des classes

| v15 (DirectShow) | v2026 (moteur X) | Notes |
|---|---|---|
| `VideoCaptureCore` | `VideoCaptureCoreX` | Constructeur direct au lieu d'une factory |
| `VideoEditCore` | `VideoEditCoreX` | |
| `MediaPlayerCore` | `MediaPlayerCoreX` | |
| `VideoCaptureSource` | `VideoCaptureDeviceSourceSettings` | Fortement typé |
| `AudioCaptureSource` | via `device.CreateSourceSettingsVC()` | Création basée sur le périphérique |
| `MP4Output` | `MP4Output` | Même nom, espace de noms différent |
| `MP4HWOutput` | `MP4Output` avec encodeur matériel | Voir la section encodeur |
| `AVIOutput` | `AVIOutput` | Même nom, espace de noms différent |
| `WMVOutput` | `WMVOutput` | Même nom, espace de noms différent |
| `MOVOutput` | `MOVOutput` | Même nom, espace de noms différent |
| `MPEGTSOutput` | `MPEGTSOutput` | Même nom, espace de noms différent |
| `WebMOutput` | `WebMOutput` | Même nom, espace de noms différent |
| `VideoCaptureMode` | Non nécessaire | L'aperçu est par défaut ; ajoutez des sorties pour l'enregistrement |

---

### Capture vidéo : migration côte à côte

#### Création du moteur

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoCapture;

VideoCaptureCore VideoCapture1;
VideoCapture1 = await VideoCaptureCore.CreateAsync(VideoView1 as IVideoView);

// v2026 (moteur X)
using VisioForge.Core.VideoCaptureX;

VideoCaptureCoreX VideoCapture1;
await VisioForgeX.InitSDKAsync();  // Une seule fois au démarrage de l'application
VideoCapture1 = new VideoCaptureCoreX(VideoView1 as IVideoView);
```

#### Énumération des périphériques

```csharp
// v15 (DirectShow) — synchrone, méthode d'instance
foreach (var device in VideoCapture1.Video_CaptureDevices())
{
    cbVideoInputDevice.Items.Add(device.Name);
}

foreach (var device in VideoCapture1.Audio_CaptureDevices())
{
    cbAudioInputDevice.Items.Add(device.Name);
}

foreach (string device in VideoCapture1.Audio_OutputDevices())
{
    cbAudioOutputDevice.Items.Add(device);
}
```

```csharp
// v2026 (moteur X) — asynchrone, singleton partagé
using VisioForge.Core.MediaBlocks;

// Option 1 : énumération ponctuelle
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
foreach (var device in videoDevices)
{
    cbVideoInputDevice.Items.Add(device.DisplayName);  // Note : DisplayName, pas Name
}

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
foreach (var device in audioDevices)
{
    cbAudioInputDevice.Items.Add(device.DisplayName);
}

var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
foreach (var device in audioOutputs)
{
    cbAudioOutputDevice.Items.Add(device.DisplayName);
}

// Option 2 : surveillance du branchement à chaud (nouvelle fonctionnalité)
DeviceEnumerator.Shared.OnVideoSourceAdded += (sender, device) =>
{
    cbVideoInputDevice.Items.Add(device.DisplayName);
};
await DeviceEnumerator.Shared.StartVideoSourceMonitorAsync();
```

#### Configuration de la source vidéo

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbVideoInputDevice.Text);
VideoCapture1.Video_CaptureDevice.Format = cbVideoInputFormat.Text;
VideoCapture1.Video_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Video_CaptureDevice.FrameRate = new VideoFrameRate(30.0);
```

```csharp
// v2026 (moteur X)
using VisioForge.Core.Types.X.Sources;

var devices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var deviceItem = devices.FirstOrDefault(d => d.DisplayName == cbVideoInputDevice.Text);

var videoSourceSettings = new VideoCaptureDeviceSourceSettings(deviceItem);

// Définir le format (typé, plutôt que basé sur une chaîne)
var formatItem = deviceItem.VideoFormats
    .FirstOrDefault(f => f.Name == cbVideoInputFormat.Text);
if (formatItem != null)
{
    videoSourceSettings.Format = formatItem.ToFormat();
    videoSourceSettings.Format.FrameRate = new VideoFrameRate(30.0);
}

VideoCapture1.Video_Source = videoSourceSettings;
```

#### Configuration de la source audio

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(cbAudioInputDevice.Text);
VideoCapture1.Audio_CaptureDevice.Format = cbAudioInputFormat.Text;
VideoCapture1.Audio_CaptureDevice.Format_UseBest = cbUseBestFormat.IsChecked == true;
VideoCapture1.Audio_OutputDevice = cbAudioOutputDevice.Text;
```

```csharp
// v2026 (moteur X)
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioRenderers;

var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var deviceItem = audioDevices.FirstOrDefault(d => d.DisplayName == cbAudioInputDevice.Text);

var formatItem = deviceItem.Formats
    .FirstOrDefault(f => f.Name == cbAudioInputFormat.Text);
IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(formatItem?.ToFormat());
VideoCapture1.Audio_Source = audioSource;

// Périphérique de sortie audio
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var outputDevice = audioOutputs.FirstOrDefault(d => d.DisplayName == cbAudioOutputDevice.Text);
VideoCapture1.Audio_OutputDevice = new AudioRendererSettings(outputDevice);
```

#### Sortie et enregistrement

```csharp
// v15 (DirectShow) — sortie unique
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

VideoCapture1.Mode = VideoCaptureMode.VideoCapture;
VideoCapture1.Output_Filename = "output.mp4";

var mp4Output = new MP4Output();
// configurer mp4Output...
VideoCapture1.Output_Format = mp4Output;

await VideoCapture1.StartAsync();
```

```csharp
// v2026 (moteur X) — sorties multiples simultanées
using VisioForge.Core.Types.X.Output;

// Aucune propriété Mode nécessaire — l'aperçu est par défaut
// Ajoutez la ou les sorties pour l'enregistrement
VideoCapture1.Outputs_Add(new MP4Output("output.mp4"), false);

// Vous pouvez ajouter plusieurs sorties simultanément :
// VideoCapture1.Outputs_Add(new WebMOutput("output.webm"), false);

await VideoCapture1.StartAsync();
```

#### Aperçu seul (sans enregistrement)

```csharp
// v15 (DirectShow)
VideoCapture1.Mode = VideoCaptureMode.VideoPreview;
await VideoCapture1.StartAsync();

// v2026 (moteur X) — n'ajoutez simplement aucune sortie
await VideoCapture1.StartAsync();
```

#### Encodage accéléré matériellement

```csharp
// v15 (DirectShow)
using VisioForge.Core.Types.Output;

var mp4Output = new MP4HWOutput();
VideoCapture1.Output_Format = mp4Output;
```

```csharp
// v2026 (moteur X)
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEncoders;

var mp4Output = new MP4Output("output.mp4");
mp4Output.Video = new NVENCH264EncoderSettings();  // NVIDIA
// ou : mp4Output.Video = new QSVH264EncoderSettings();  // Intel
// ou : mp4Output.Video = new AMFH264EncoderSettings();  // AMD
VideoCapture1.Outputs_Add(mp4Output, false);
```

#### Contrôle du volume

```csharp
// v15 (DirectShow)
VideoCapture1.Audio_OutputDevice_Volume_Set(70);
VideoCapture1.Audio_OutputDevice_Balance_Set(0);

// v2026 (moteur X)
VideoCapture1.Audio_OutputDevice_Volume = 0.7;  // 0.0 à 1.0 (double)
```

#### Nettoyage

```csharp
// v15 (DirectShow)
VideoCapture1.Dispose();

// v2026 (moteur X)
await VideoCapture1.DisposeAsync();
VisioForgeX.DestroySDK();  // À la fermeture de l'application
```

---

### Lecteur multimédia : migration côte à côte

#### Création du moteur et lecture

```csharp
// v15 (DirectShow)
using VisioForge.Core.MediaPlayer;

MediaPlayerCore _player;
_player = new MediaPlayerCore(VideoView1 as IVideoView);
_player.Audio_OutputDevice = cbAudioOutput.Text;
_player.Playlist_Clear();
_player.Playlist_Add("video.mp4");
await _player.PlayAsync();
```

```csharp
// v2026 (moteur X)
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();  // Une seule fois au démarrage de l'application

MediaPlayerCoreX _player;
_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri("video.mp4"));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

#### Différences clés

| Fonctionnalité | v15 (DirectShow) | v2026 (moteur X) |
|---------|-------------------|-------------------|
| Configuration de la source | `Playlist_Add("file.mp4")` | `OpenAsync(UniversalSourceSettingsV2)` |
| Position | `_player.Position_Get_Time()` (méthode, ms) | `await _player.Position_GetAsync()` |
| Durée | `_player.Duration_Time()` (méthode, ms) | `await _player.DurationAsync()` |
| Version | `_player.SDK_Version()` (instance) | `MediaPlayerCoreX.SDK_Version` (statique) |
| Boucle | `_player.Loop` | `_player.Loop` (même nom) |

---

### Édition vidéo : migration côte à côte

#### Création du moteur

```csharp
// v15 (DirectShow)
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

VideoEditCore VideoEdit1;
VideoEdit1 = new VideoEditCore(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Filename = "output.mp4";
VideoEdit1.Output_Format = new MP4Output();
await VideoEdit1.StartAsync();
```

```csharp
// v2026 (moteur X)
using VisioForge.Core.VideoEditX;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;

await VisioForgeX.InitSDKAsync();  // Une seule fois au démarrage de l'application

VideoEditCoreX VideoEdit1;
VideoEdit1 = new VideoEditCoreX(VideoView1 as IVideoView);
VideoEdit1.Input_AddVideoFile("input.mp4");
VideoEdit1.Output_Format = new MP4Output("output.mp4");
await VideoEdit1.StartAsync();
```

#### Différences clés

| Fonctionnalité | v15 (DirectShow) | v2026 (moteur X) |
|---------|-------------------|-------------------|
| Espace de noms | `VisioForge.Core.VideoEdit` | `VisioForge.Core.VideoEditX` |
| Types de sortie | `VisioForge.Core.Types.Output` | `VisioForge.Core.Types.X.Output` |
| Nom du fichier de sortie | Propriété `Output_Filename` séparée | Inclus dans le constructeur de sortie : `new MP4Output("path")` |
| Effets | `VisioForge.Core.Types.VideoEffects` | `VisioForge.Core.Types.X.VideoEffects` |
| Taille vidéo | `VisioForge.Core.Types.Size` | `VisioForge.Core.Types.Size` (inchangé) |

---

### Événements

La plupart des événements conservent les mêmes noms dans les deux moteurs :

| Événement | v15 | v2026 moteur X | Notes |
|-------|-----|----------------|-------|
| `OnError` | Oui | Oui | `ErrorsEventArgs` (identique) |
| `OnStop` | Oui | Oui | |
| `OnStart` | Oui | Oui | |
| `OnAudioVUMeter` | — | Oui | Nouveau dans le moteur X |
| `OnOutputStarted` | — | Oui | Nouveau — événements par sortie |
| `OnOutputStopped` | — | Oui | Nouveau — événements par sortie |
| `OnBarcodeDetected` | — | Oui | Nouveau |
| `OnFaceDetected` | — | Oui | Nouveau |
| `OnMotionDetection` | Oui | Oui | |
| `OnVideoFrameBuffer` | Oui | Oui | |

---

## FAQ

### « L'espace de noms VisioForge.Core.Types n'est plus disponible »

Cela dépend du moteur que vous utilisez :

- **Classes DirectShow** : `VisioForge.Core.Types` existe toujours. Assurez-vous que le bon paquet NuGet est installé. Pour .NET 8+, assurez-vous que votre TFM inclut `-windows`.
- **Classes moteur X** : utilisez les sous-espaces de noms `VisioForge.Core.Types.X.*` (par exemple `VisioForge.Core.Types.X.Output`, `VisioForge.Core.Types.X.Sources`).

### « Puis-je continuer à utiliser les classes DirectShow (VideoCaptureCore, etc.) ? »

Oui. Les classes DirectShow sont entièrement prises en charge et activement mises à jour dans v2026. Vous pouvez mettre à niveau le paquet NuGet et conserver votre code existant avec des modifications minimales.

### « MP4HWOutput est introuvable »

Dans les moteurs X, `MP4HWOutput` est remplacé par `MP4Output` avec un encodeur matériel spécifique :

```csharp
var mp4 = new MP4Output("output.mp4");
mp4.Video = new NVENCH264EncoderSettings();  // ou QSVH264, AMFH264, etc.
```

### « VideoCaptureMode est introuvable »

Les moteurs X n'utilisent pas de propriété `Mode`. L'aperçu est le comportement par défaut. Pour enregistrer, ajoutez des sorties avec `Outputs_Add()`.

### « La méthode Audio_CaptureDevices() est introuvable »

Dans les moteurs X, l'énumération des périphériques utilise un singleton asynchrone partagé :

```csharp
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
```

### « Quelle est la stratégie de migration recommandée pour les applications de production ? »

1. **D'abord** : mettez à niveau vers v2026 en conservant les classes DirectShow (Parcours A) — faible risque, modifications de code minimales
2. **Testez** : vérifiez que votre application fonctionne correctement avec la nouvelle version du paquet
3. **Ensuite** : lorsque vous êtes prêt, migrez vers les moteurs X (Parcours B) — module par module si nécessaire
4. **Les deux moteurs peuvent coexister** dans la même application durant la période de transition

---

Visitez notre page [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) pour obtenir davantage d'exemples de code.
