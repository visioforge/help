---
title: Hoja de referencia de VisioForge Media Player SDK en C# .NET
description: Referencia de una página para Media Player SDK .Net — paquetes NuGet, API MediaPlayerCoreX, ejemplo canónico en C#, soporte de plataformas y fallos comunes.
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

# Hoja de referencia de VisioForge Media Player SDK en C# .NET

Use VisioForge Media Player SDK .Net cuando necesite reproducir archivos locales (MP4, MKV, MOV, WebM), streams de red (RTSP, HLS, MPEG-DASH) y audio (MP3, AAC, FLAC) dentro de una aplicación C# en Windows, macOS, Linux, Android o iOS. `MediaPlayerCoreX` es el motor multiplataforma principal; `VideoView` es el control de UI que vincula el motor al árbol visual.

!!! tip "Agentes de IA para programación: usen el servidor MCP de VisioForge"

    ¿Está construyendo esto con **Claude Code**, **Cursor** u otro agente de IA para programación?
    Conéctese al [servidor MCP público de VisioForge](../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para búsquedas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que hacer grep
    sobre `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Soporte de plataformas

- **Windows** (x64 y x86) — WinForms, WPF, WinUI, MAUI, Avalonia, Consola
- **macOS** (nativo y MacCatalyst) — MAUI, Avalonia, Consola
- **Linux** (x64) — Avalonia, Consola; requiere GStreamer 1.22+ del sistema
- **Android** — mediante MAUI
- **iOS** — mediante MAUI

Todos los objetivos multiplataforma usan una pipeline de decodificación respaldada por GStreamer con aceleración por hardware cuando está disponible. Para la matriz completa de códecs × plataforma, consulte [platform-matrix.md](../platform-matrix.md).

## Paquetes NuGet

Paquete principal del SDK (siempre obligatorio):

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
```

Runtime nativo para Windows x64:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

Runtime nativo para Windows x86:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86" Version="2025.11.0" />
```

macOS (nativo) y MacCatalyst (MAUI macOS):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
```

Linux x64 (más GStreamer 1.22+ del sistema):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
```

Android e iOS:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.11.0" />
```

Paquetes opcionales de frameworks de UI — agregue el que corresponda a su objetivo:

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.2.19" />
```

Guía completa de instalación por IDE: [install/index.md](../install/index.md).

## Clases principales de la API

| Clase | Función | Véase también |
|---|---|---|
| `MediaPlayerCoreX` | Motor principal de reproducción (multiplataforma, respaldado por GStreamer) | [Inicio rápido](./index.md) |
| `VideoView` | Control de UI que vincula el reproductor al árbol visual | [Guía de Avalonia](./guides/avalonia-player.md) |
| `UniversalSourceSettings` | Construye un descriptor de fuente a partir de un URI (archivo, URL, stream) | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `AudioRendererSettings` | Configura el dispositivo de salida de audio | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `ErrorsEventArgs` | Payload del evento de error expuesto vía `OnError` | [video-player-csharp.md](./guides/video-player-csharp.md) |

## Ejemplo canónico mínimo

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
        // 1. Initialize the SDK (once per process)
        await VisioForgeX.InitSDKAsync();

        // 2. Create the player bound to a VideoView control
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;

        // 3. (Optional) pick an audio output device
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        if (devices.Length > 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(devices[0]);

        // 4. Build a source from a file path or stream URL
        var source = await UniversalSourceSettings.CreateAsync(
            "C:\\Videos\\sample.mp4");

        // 5. Open and play
        await _player.OpenAsync(source);
        await _player.PlayAsync();

        // Mid-playback control:
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
        // 6. Dispose the player and tear down the SDK
        if (_player != null) await _player.DisposeAsync();
        VisioForgeX.DestroySDK();
        base.OnFormClosing(e);
    }
}
```

## Flujo de trabajo típico

1. Inicialice el SDK con `VisioForgeX.InitSDKAsync()`.
2. Instancie `MediaPlayerCoreX` vinculado a un `VideoView` (omita la vista para solo audio).
3. Opcionalmente configure la salida de audio con `AudioRendererSettings`.
4. Cree una fuente mediante `UniversalSourceSettings.CreateAsync(uri)`.
5. `OpenAsync(source)` → `PlayAsync()`; controle con `PauseAsync`, `ResumeAsync`, `Position_SetAsync`, `Rate_SetAsync`, `StopAsync`.
6. Libere el reproductor y llame a `VisioForgeX.DestroySDK()` al cerrar la aplicación.

## Fallos comunes

- **El ciclo init/destroy del SDK debe envolver todo uso.** Olvidar `VisioForgeX.DestroySDK()` al cerrar filtra handles nativos y puede dejar hilos de GStreamer en ejecución. Empareje siempre con `InitSDKAsync()`.
- **AnyCPU en Windows necesita ambos redistribuibles.** Distribuya TANTO `VisioForge.CrossPlatform.Core.Windows.x86` COMO `.x64` (más los paquetes Libav correspondientes) o la aplicación fallará en tiempo de ejecución en la arquitectura que falte.
- **Enumere los dispositivos de audio de forma asíncrona antes de asignar.** `Audio_OutputDevicesAsync(...)` debe completarse antes de construir `AudioRendererSettings` y establecer `Audio_OutputDevice`; asignarlo contra una lista de dispositivos no inicializada lanza una excepción.
- **Linux requiere GStreamer 1.22+ del sistema.** El redistribuible NuGet de Linux asume una instalación de GStreamer en el sistema (paquetes `gstreamer1.0-*`) — NO es un sustituto, y los paquetes Libav de Windows no son aplicables a Linux.
- **`MediaPlayerCoreX` ≠ `MediaPlayerCore`.** `MediaPlayerCoreX` es el motor multiplataforma respaldado por GStreamer que se utiliza en toda esta página. `MediaPlayerCore` (sin `X`) es el motor DirectShow de primera clase exclusivo de Windows con firmas de método diferentes — ambos están totalmente soportados; no mezcle APIs entre ellos.

## Véase también

- **Frameworks de UI**
    - Escritorio WinForms / WPF → [video-player-csharp.md](./guides/video-player-csharp.md)
    - Avalonia multiplataforma (Windows/macOS/Linux) → [avalonia-player.md](./guides/avalonia-player.md)
    - .NET MAUI (móvil + escritorio) → [maui-player.md](./guides/maui-player.md)
    - Solo Android → [android-player.md](./guides/android-player.md)
- **Streaming de red**
    - RTSP → [rtsp.md](../general/network-streaming/rtsp.md)
    - HLS → [hls-streaming.md](../general/network-streaming/hls-streaming.md)
- **Despliegue**
    - Windows → [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS → [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Linux (Ubuntu) → [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android → [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS → [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Primeros pasos + guía completa** → [index.md](./index.md), [video-player-csharp.md](./guides/video-player-csharp.md)

## Preguntas frecuentes

### ¿Puede el SDK reproducir streams RTSP?

Sí. Pase un URI `rtsp://` a `UniversalSourceSettings.CreateAsync(...)` y llame a `OpenAsync` / `PlayAsync` — la misma ruta de código que un archivo local. Los URIs HTTP, HLS y MPEG-DASH funcionan de forma idéntica.

### ¿Qué plataformas requieren que GStreamer esté instalado a nivel de sistema?

Linux. En Linux x64, el paquete NuGet `VisioForge.CrossPlatform.Core.Linux.x64` depende de un runtime de GStreamer 1.22+ instalado en el sistema. Windows, macOS, Android e iOS incluyen todo a través de sus redistribuibles NuGet — no se necesita instalación a nivel de sistema.

### ¿Cuál es la diferencia entre `MediaPlayerCoreX` y `MediaPlayerCore`?

`MediaPlayerCoreX` es el motor multiplataforma utilizado a lo largo de esta hoja de referencia — se ejecuta en Windows, macOS, Linux, Android e iOS mediante GStreamer y usa firmas de método asíncronas. `MediaPlayerCore` (sin `X`) es el motor DirectShow exclusivo de Windows con una API síncrona basada en eventos; sigue siendo un motor de primera clase y totalmente soportado (elígelo cuando tu objetivo sea solo Windows y necesites comportamiento específico de DirectShow). Para nuevos proyectos multiplataforma, prefiere `MediaPlayerCoreX`.

### ¿Cómo reproduzco audio sin mostrar una ventana de video?

Instancie `MediaPlayerCoreX` sin un `VideoView` (`new MediaPlayerCoreX()`) y continúe con el mismo flujo `UniversalSourceSettings.CreateAsync` → `OpenAsync` → `PlayAsync`. Las fuentes solo de audio (MP3, AAC, FLAC, WAV) se reproducirán a través del `Audio_OutputDevice` configurado.
