---
title: Hoja de referencia de VisioForge Media Blocks SDK en C# .NET
description: Referencia rápida de Media Blocks SDK con paquetes NuGet, API de MediaBlocksPipeline, ejemplo canónico, plataformas y errores comunes.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
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
  - Capture
  - Recording
  - Streaming
  - Encoding
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - VideoRendererBlock
  - SystemVideoSourceBlock
  - H264EncoderBlock
  - MP4SinkBlock
  - DeviceEnumerator
---

# Hoja de referencia de VisioForge Media Blocks SDK en C# .NET

Media Blocks SDK es el SDK .NET más flexible de VisioForge — permite construir pipelines de medios arbitrarios componiendo bloques (fuentes, codificadores, renderizadores, sinks). Elija Media Blocks cuando necesite pipelines personalizados que los SDK de más alto nivel (Media Player / Video Capture / Video Edit) no puedan expresar.

!!! tip "Agentes de IA para programación: usen el servidor MCP de VisioForge"

    ¿Está construyendo esto con **Claude Code**, **Cursor** u otro agente de IA para programación?
    Conéctese al [servidor MCP público de VisioForge](../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para obtener búsquedas de API estructuradas,
    ejemplos de código ejecutables y guías de despliegue — más preciso que hacer grep
    sobre `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Soporte de plataformas

- Se ejecuta en Windows (x64 / x86), macOS, Linux x64, Android e iOS.
- Frameworks de UI: WinForms, WPF, MAUI, Avalonia, Uno, además de consola.
- El procesamiento multiplataforma se apoya en un backend GStreamer empaquetado en macOS/Android/iOS y en la instalación del sistema de GStreamer 1.22+ en Linux.
- Para la matriz completa de códecs × plataformas, consulte [platform-matrix.md](../platform-matrix.md).

## Paquetes NuGet

Paquete principal del SDK (todas las plataformas):

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Runtime de Windows x64 (elija x86 para aplicaciones de 32 bits):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<!-- Opcional: decodificadores/codificadores adicionales vía Libav -->
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Windows x86:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86.UPX" Version="2025.4.9" />
```

macOS (nativo y MAUI / macCatalyst):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.4.9" />
```

Linux x64 (también requiere GStreamer 1.22+ instalado en el sistema):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.4.9" />
```

Android e iOS:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.4.9" />
```

Paquetes de integración de UI (opcionales, por framework):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2025.4.9" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2025.4.9" />
```

Guía completa de instalación: [install/index.md](../install/index.md).

## Clases principales de la API

| Clase | Función | Véase también |
| --- | --- | --- |
| `MediaBlocksPipeline` | Objeto raíz del pipeline. Conecta bloques mediante `Connect(output, input)`. Expone `StartAsync` / `StopAsync` / `DisposeAsync` y los eventos `OnError`, `OnStart`, `OnStop`. | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `UniversalSourceBlock` | Abre un archivo, URL o stream como entrada. Analiza los flujos automáticamente y expone los pads `VideoOutput` / `AudioOutput`. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `VideoRendererBlock` | Enlaza la salida de video del pipeline a un control `IVideoView` (WinForms / WPF / MAUI / Avalonia). Admite capturas de pantalla. | [GettingStarted/player.md](./GettingStarted/player.md) |
| `SystemVideoSourceBlock` | Entrada de webcam / USB / cámara integrada. Se configura mediante `VideoCaptureDeviceSourceSettings`. | [GettingStarted/camera.md](./GettingStarted/camera.md) |
| `H264EncoderBlock` | Codificador H.264 con backends por software y hardware (NVENC / AMF / Quick Sync). | [GettingStarted/pipeline.md](./GettingStarted/pipeline.md) |
| `MP4SinkBlock` | Escribe video + audio codificados en un archivo `.mp4`. Añada entradas mediante `IMediaBlockDynamicInputs.CreateNewInput`. | [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md) |
| `DeviceEnumerator` | Enumera cámaras, micrófonos y salidas de audio de forma asíncrona mediante `DeviceEnumerator.Shared.VideoSourcesAsync()`, etc. | [GettingStarted/device-enum.md](./GettingStarted/device-enum.md) |

## Ejemplo mínimo canónico

El pipeline útil más simple — cargar un archivo, renderizar video + audio y limpiar los recursos. Tómelo y adáptelo desde [GettingStarted/player.md](./GettingStarted/player.md).

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// 1. Inicialice el SDK una vez al arranque de la aplicación
await VisioForgeX.InitSDKAsync();

// 2. Cree el pipeline y suscríbase a los errores
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine(e.Message);

// 3. Fuente: abrir un archivo de medios (URL o ruta local vía URI)
var sourceSettings = await UniversalSourceSettings.CreateAsync(
    new Uri("file:///C:/Videos/sample.mp4"));
var fileSource = new UniversalSourceBlock(sourceSettings);

// 4. Renderizador de video — VideoView1 es un IVideoView en su formulario/página
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// 5. Renderizador de audio — seleccione la primera salida de audio del sistema
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var audioRenderer = new AudioRendererBlock(audioOutputs[0]);

// 6. Conecte los pads de salida → pads de entrada
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);

// 7. Reproducir
await pipeline.StartAsync();

// ... más tarde, al detener por el usuario / salir de la aplicación:
await pipeline.StopAsync();
await pipeline.DisposeAsync();
VisioForgeX.DestroySDK(); // solo síncrono — no existe variante async
```

Sustituya `UniversalSourceBlock` por `SystemVideoSourceBlock` (cámara) o `RTSPSourceBlock` (cámara IP) sin cambiar el resto del cableado.

## Flujo de trabajo típico

1. Inicializar el SDK — `await VisioForgeX.InitSDKAsync()`.
2. Crear `MediaBlocksPipeline` y suscribirse a `OnError`.
3. Instanciar bloques de fuente (`UniversalSourceBlock`, `SystemVideoSourceBlock`, `RTSPSourceBlock`, …).
4. Instanciar bloques de procesamiento — codificadores, mezcladores, overlays, efectos (opcional).
5. Instanciar bloques sink / renderizador (`VideoRendererBlock`, `AudioRendererBlock`, `MP4SinkBlock`, …).
6. Cablear los bloques con `pipeline.Connect(output, input)` — cada ruta de datos debe conectarse explícitamente.
7. `StartAsync` → `StopAsync` → `DisposeAsync`, y después `DestroySDK` al salir de la aplicación.

## Errores comunes

- **No hay flujo de datos implícito.** Los bloques deben cablearse explícitamente con `pipeline.Connect(output, input)`; añadir un bloque al pipeline no basta para que los medios circulen a través de él.
- **Orden de overlays / entradas dinámicas.** Los overlays en `OverlayManagerBlock` y los pads dinámicos en los sinks (`MP4SinkBlock` mediante `IMediaBlockDynamicInputs.CreateNewInput`) deben añadirse antes de arrancar el pipeline — añadirlos después de `StartAsync` falla silenciosamente o lanza una excepción.
- **Enumeración solo con await.** `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioOutputsAsync()` deben usarse con await — no existe una variante síncrona. Usar `.Result` desde el hilo de UI puede provocar un deadlock.
- **Linux necesita GStreamer del sistema.** `VisioForge.CrossPlatform.Core.Linux.x64` espera que GStreamer 1.22+ ya esté instalado (`apt install gstreamer1.0-*`); el NuGet de Libav es complementario, no un sustituto. Consulte [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md).
- **Higiene del ciclo de vida.** Siempre haga `await pipeline.StopAsync()` y `await pipeline.DisposeAsync()` antes de crear otro pipeline en el mismo motor. Omitir el dispose provoca fugas de handles nativos y puede causar cuelgues al cerrar.

## Véase también

- **Primeros pasos**
    - Reproductor de archivos de video — [GettingStarted/player.md](./GettingStarted/player.md)
    - Visor de cámara — [GettingStarted/camera.md](./GettingStarted/camera.md)
    - Tutorial completo de pipeline — [GettingStarted/pipeline.md](./GettingStarted/pipeline.md)
    - Enumeración de dispositivos — [GettingStarted/device-enum.md](./GettingStarted/device-enum.md)
- **Pipelines específicos**
    - Reproductor RTSP / cámara IP — [Guides/rtsp-player-csharp.md](./Guides/rtsp-player-csharp.md)
    - Guardar flujo RTSP (passthrough) — [Guides/rtsp-save-original-stream.md](./Guides/rtsp-save-original-stream.md)
    - Cuadrícula multicámara — [Guides/multi-camera-rtsp-grid.md](./Guides/multi-camera-rtsp-grid.md)
- **Despliegue**
    - Windows — [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS — [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Ubuntu — [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android — [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS — [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Instalación y matriz de plataformas** — [../install/index.md](../install/index.md), [../platform-matrix.md](../platform-matrix.md)

## Preguntas frecuentes

### ¿En qué se diferencia Media Blocks de Media Player SDK?

Media Player SDK es un reproductor de alto nivel con una sola clase (`MediaPlayerCoreX`) y controles de reproducción integrados — óptimo cuando solo necesita reproducir un archivo o stream. Media Blocks expone el pipeline subyacente para que pueda insertar codificadores, efectos, sinks multi-salida o procesamiento personalizado. Si se encuentra luchando contra Media Player SDK para añadir un paso que no expone, cambie a Media Blocks.

### ¿Puedo ejecutarlo en Linux sin GStreamer?

No. El NuGet del runtime de Linux x64 es un puente fino al stack de GStreamer 1.22+ del sistema. Instale `gstreamer1.0-plugins-base`, `-good`, `-bad`, `-ugly` y `-libav` desde su distribución. Consulte [deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md) para la lista completa de paquetes. macOS / Android / iOS sí incluyen un GStreamer empaquetado a través del NuGet de plataforma.

### ¿Cómo añado un efecto de video personalizado?

Use `GLShaderBlock` para un fragment shader GLSL personalizado (acelerado por GPU) o la familia predefinida `GL*Block` (por ejemplo `GLBlurBlock`, `GLColorBalanceBlock`) para efectos comunes. Para procesamiento basado en CV, use la familia `CV*Block` (por ejemplo `CVFaceDetectBlock`, `CVEdgeDetectBlock`, `CVDewarpBlock`). Todos se encajan entre un decodificador y un renderizador como cualquier otro bloque. Receta completa: [Guides/custom-video-effects-csharp.md](./Guides/custom-video-effects-csharp.md).
