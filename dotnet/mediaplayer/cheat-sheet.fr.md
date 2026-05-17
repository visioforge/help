---
title: VisioForge Media Player SDK en C# .NET — Aide-mémoire
description: Référence d'une page pour Media Player SDK — paquets NuGet, API MediaPlayerCoreX, exemple canonique, plateformes prises en charge et pièges.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
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
  - Playback
  - Streaming
  - RTSP
  - HLS
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - VideoView
  - UniversalSourceSettings
  - AudioRendererSettings
  - ErrorsEventArgs
---

# VisioForge Media Player SDK en C# .NET — Aide-mémoire

Utilisez VisioForge Media Player SDK .Net lorsque vous devez lire des fichiers locaux (MP4, MKV, MOV, WebM), des flux réseau (RTSP, HLS, MPEG-DASH) et de l'audio (MP3, AAC, FLAC) dans une application C# sur Windows, macOS, Linux, Android ou iOS. `MediaPlayerCoreX` est le moteur multiplateforme principal ; `VideoView` est le contrôle UI qui lie le moteur à l'arbre visuel.

!!! tip "Agents de codage IA : utilisez le serveur MCP VisioForge"

    Vous développez ceci avec **Claude Code**, **Cursor** ou un autre agent de codage IA ?
    Connectez-vous au [serveur MCP public VisioForge](../general/mcp-server-usage.md)
    à l'adresse `https://mcp.visioforge.com/mcp` pour des recherches API structurées,
    des exemples de code exécutables et des guides de déploiement — plus précis qu'un
    grep sur `llms.txt`. Aucune authentification requise.

    Claude Code : `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prise en charge des plateformes

- **Windows** (x64 et x86) — WinForms, WPF, WinUI, MAUI, Avalonia, Console
- **macOS** (natif et MacCatalyst) — MAUI, Avalonia, Console
- **Linux** (x64) — Avalonia, Console ; nécessite GStreamer 1.22+ du système
- **Android** — via MAUI
- **iOS** — via MAUI

Toutes les cibles multiplateformes utilisent un pipeline de décodage basé sur GStreamer avec accélération matérielle lorsqu'elle est disponible. Pour la matrice complète codec × plateforme, consultez [platform-matrix.md](../platform-matrix.md).

## Paquets NuGet

Paquet principal du SDK (toujours requis) :

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
```

Runtime natif Windows x64 :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

Runtime natif Windows x86 :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86" Version="2025.11.0" />
```

macOS (natif) et MacCatalyst (MAUI macOS) :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
```

Linux x64 (plus GStreamer 1.22+ du système) :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
```

Android et iOS :

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.11.0" />
```

Paquets optionnels du framework UI — ajoutez celui qui correspond à votre cible :

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.2.19" />
```

Guide d'installation complet par IDE : [install/index.md](../install/index.md).

## Classes API principales

| Classe | Rôle | Voir aussi |
|---|---|---|
| `MediaPlayerCoreX` | Moteur de lecture principal (multiplateforme, basé sur GStreamer) | [Démarrage rapide](./index.md) |
| `VideoView` | Contrôle UI qui lie le lecteur à l'arbre visuel | [Guide Avalonia](./guides/avalonia-player.md) |
| `UniversalSourceSettings` | Construit un descripteur de source à partir d'un URI (fichier, URL, flux) | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `AudioRendererSettings` | Configure le périphérique de sortie audio | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `ErrorsEventArgs` | Charge utile de l'événement d'erreur exposée via `OnError` | [video-player-csharp.md](./guides/video-player-csharp.md) |

## Exemple minimal canonique

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

public partial class MainForm : Form
{
    private MediaPlayerCoreX _player;

    private async void MainForm_Load(object sender, EventArgs e)
    {
        // 1. Initialiser le SDK (une fois par processus)
        await VisioForgeX.InitSDKAsync();

        // 2. Créer le lecteur lié à un contrôle VideoView
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;

        // 3. (Optionnel) choisir un périphérique de sortie audio
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        if (devices.Length > 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(devices[0]);

        // 4. Construire une source à partir d'un chemin de fichier ou d'une URL de flux
        var source = await UniversalSourceSettings.CreateAsync(
            new Uri("C:\\Videos\\sample.mp4"));

        // 5. Ouvrir et lire
        await _player.OpenAsync(source);
        await _player.PlayAsync();

        // Contrôle en cours de lecture :
        //   await _player.PauseAsync();
        //   await _player.ResumeAsync();
        //   await _player.StopAsync();
    }

    private void Player_OnError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Player error: {e.Message}");
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        // 6. Libérer le lecteur et démanteler le SDK
        if (_player != null) await _player.DisposeAsync();
        VisioForgeX.DestroySDK();
        base.OnFormClosing(e);
    }
}
```

## Flux de travail typique

1. Initialisez le SDK avec `VisioForgeX.InitSDKAsync()`.
2. Instanciez `MediaPlayerCoreX` lié à un `VideoView` (omettez la vue pour de l'audio seul).
3. Configurez optionnellement la sortie audio avec `AudioRendererSettings`.
4. Créez une source via `UniversalSourceSettings.CreateAsync(uri)`.
5. `OpenAsync(source)` → `PlayAsync()` ; contrôlez avec `PauseAsync`, `ResumeAsync`, `Position_SetAsync`, `Rate_SetAsync`, `StopAsync`.
6. Libérez le lecteur et appelez `VisioForgeX.DestroySDK()` à la sortie de l'application.

## Pièges fréquents

- **L'init/destroy du SDK doit encadrer toute utilisation.** Oublier `VisioForgeX.DestroySDK()` à l'arrêt fuit des handles natifs et peut laisser des threads GStreamer en cours. Toujours l'associer à `InitSDKAsync()`.
- **AnyCPU sur Windows nécessite les deux redistribuables.** Déployez À LA FOIS `VisioForge.CrossPlatform.Core.Windows.x86` ET `.x64` (plus les paquets Libav correspondants), sinon l'application échouera à l'exécution sur l'architecture manquante.
- **Énumérez les périphériques audio de façon asynchrone avant l'assignation.** `Audio_OutputDevicesAsync(...)` doit se terminer avant que vous ne construisiez `AudioRendererSettings` et définissiez `Audio_OutputDevice` ; définir celui-ci contre une liste de périphériques non initialisée lève une exception.
- **Linux nécessite GStreamer 1.22+ du système.** Le redistribuable NuGet Linux suppose un GStreamer installé sur le système (paquets `gstreamer1.0-*`) — il ne le remplace PAS, et les paquets Windows Libav ne s'appliquent pas à Linux.
- **`MediaPlayerCoreX` ≠ `MediaPlayerCore`.** `MediaPlayerCoreX` est le moteur multiplateforme basé sur GStreamer utilisé partout dans cette page. `MediaPlayerCore` (sans `X`) est le moteur DirectShow de premier rang réservé à Windows avec des signatures de méthode différentes — les deux sont entièrement pris en charge ; ne mélangez pas les API entre eux.

## Voir aussi

- **Frameworks UI**
    - WinForms / WPF bureau → [video-player-csharp.md](./guides/video-player-csharp.md)
    - Avalonia multiplateforme (Windows/macOS/Linux) → [avalonia-player.md](./guides/avalonia-player.md)
    - .NET MAUI (mobile + bureau) → [maui-player.md](./guides/maui-player.md)
    - Android uniquement → [android-player.md](./guides/android-player.md)
- **Streaming réseau**
    - RTSP → [rtsp.md](../general/network-streaming/rtsp.md)
    - HLS → [hls-streaming.md](../general/network-streaming/hls-streaming.md)
- **Déploiement**
    - Windows → [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS → [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Linux (Ubuntu) → [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android → [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS → [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Prise en main + guide complet** → [index.md](./index.md), [video-player-csharp.md](./guides/video-player-csharp.md)

## FAQ

### Le SDK peut-il lire des flux RTSP ?

Oui. Passez un URI `rtsp://` à `UniversalSourceSettings.CreateAsync(...)` et appelez `OpenAsync` / `PlayAsync` — le même chemin de code qu'un fichier local. Les URI HTTP, HLS et MPEG-DASH fonctionnent à l'identique.

### Quelles plateformes nécessitent l'installation de GStreamer au niveau système ?

Linux. Sur Linux x64, le paquet NuGet `VisioForge.CrossPlatform.Core.Linux.x64` s'appuie sur un runtime GStreamer 1.22+ installé sur le système. Windows, macOS, Android et iOS regroupent tout via leurs redistribuables NuGet — pas d'installation système requise.

### Quelle est la différence entre `MediaPlayerCoreX` et `MediaPlayerCore` ?

`MediaPlayerCoreX` est le moteur multiplateforme utilisé tout au long de cet aide-mémoire — il fonctionne sur Windows, macOS, Linux, Android et iOS via GStreamer et utilise des signatures de méthode asynchrones. `MediaPlayerCore` (sans `X`) est le moteur DirectShow réservé à Windows avec une API synchrone et événementielle ; il reste un moteur de premier rang entièrement pris en charge (choisissez-le lorsque vous ciblez uniquement Windows et avez besoin d'un comportement spécifique à DirectShow). Pour les nouveaux projets multiplateformes, préférez `MediaPlayerCoreX`.

### Comment lire de l'audio sans afficher de fenêtre vidéo ?

Instanciez `MediaPlayerCoreX` sans `VideoView` (`new MediaPlayerCoreX()`) et procédez avec le même flux `UniversalSourceSettings.CreateAsync` → `OpenAsync` → `PlayAsync`. Les sources audio seules (MP3, AAC, FLAC, WAV) seront lues via l'`Audio_OutputDevice` configuré.
