---
title: Hoja de referencia de Video Capture SDK en C# .NET
description: Referencia de una página para Video Capture SDK .Net — paquetes NuGet, API VideoCaptureCoreX, ejemplo canónico de grabación MP4, plataformas y errores comunes.
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

# VisioForge Video Capture SDK .Net — Hoja de referencia

Video Capture SDK .Net captura desde webcams, cámaras IP (RTSP/ONVIF), pantallas, Decklink y dispositivos GenICam/GigE Vision en las 5 plataformas .NET (Windows, macOS, Linux, Android, iOS). `VideoCaptureCoreX` es el motor multiplataforma; las fuentes se configuran mediante clases de ajustes tipadas como `VideoCaptureDeviceSourceSettings`, y los dispositivos se enumeran de forma asíncrona con `DeviceEnumerator`.

!!! tip "Agentes de IA para programación: usen el servidor MCP de VisioForge"

    ¿Estás construyendo esto con **Claude Code**, **Cursor** u otro agente de IA para programación?
    Conéctate al [servidor MCP público de VisioForge](../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para obtener búsquedas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que rastrear
    `llms.txt`. No requiere autenticación.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Soporte de plataformas

- **Windows x64 / x86** — WinForms, WPF, MAUI, Avalonia, Consola. Fuentes DirectShow completas + codificación por hardware NVENC / AMF / Intel Quick Sync.
- **macOS / macCatalyst** — MAUI, Avalonia, Consola. Fuentes de cámara AVFoundation + codificación por hardware VideoToolbox.
- **Linux x64** — Avalonia, Consola. Fuentes de cámara V4L2 + codificación por hardware NVENC / VA-API.
- **Android** — MAUI. Fuentes de cámara nativas + codificación por hardware MediaCodec.
- **iOS** — MAUI. Fuentes de cámara AVFoundation + codificación por hardware VideoToolbox.

Para la matriz completa de códec × fuente × plataforma consulta [platform-matrix.md](../platform-matrix.md).

## Paquetes NuGet

Paquete principal del SDK (obligatorio, todas las plataformas):

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.*" />
```

Runtimes nativos específicos por plataforma — añade los que correspondan a tu objetivo:

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

Integración de UI (elige la que corresponda a tu stack de UI):

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.*" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.*" />
```

Los controles `VideoView` para WinForms y WPF se incluyen dentro del paquete principal. Para la lista completa de paquetes (incluidos los redistribuibles del motor `VideoCaptureCore` exclusivo de Windows) consulta la [Guía de instalación](../install/index.md).

## Clases principales de la API

| Clase | Rol | Ver también |
|---|---|---|
| `VideoCaptureCoreX` | Motor de captura multiplataforma — gestiona la canalización, la fuente, las salidas y la vista previa | [Inicio rápido](./index.md) |
| `VideoCaptureDeviceSourceSettings` | Configura una fuente de cámara webcam / USB / DirectShow | [Enumerar y seleccionar dispositivo](./video-sources/video-capture-devices/enumerate-and-select.md) |
| `DeviceEnumerator` | Enumeración asíncrona de fuentes de video, fuentes de audio y salidas de audio mediante `DeviceEnumerator.Shared` | [Resumen de fuentes de video](./video-sources/index.md) |
| `VideoView` | Control de vista previa para WinForms / WPF / MAUI / Avalonia vinculado al motor de captura | [Instalación — controles de video](../install/index.md) |
| `MP4Output` | Multiplexa los streams capturados a `.mp4` con H.264 / H.265 + AAC (acelerado por GPU cuando está disponible) | [Tutorial Webcam → MP4](./video-tutorials/video-capture-webcam-mp4.md) |

Clases de ajustes de fuente adicionales: `RTSPSourceSettings` (cámaras IP — construir con `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)`, no con `new`), `ScreenCaptureD3D11SourceSettings` / `ScreenCaptureGDISourceSettings` / `ScreenCaptureDX9SourceSettings` (captura de pantalla Windows), `ScreenCaptureMacOSSourceSettings` (macOS), `ScreenCaptureXDisplaySourceSettings` (Linux), `DecklinkVideoSourceSettings`, `GenICamSourceSettings`, `NDISourceSettings`.

## Ejemplo canónico mínimo

Graba la primera webcam + micrófono predeterminado a `recording.mp4`:

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// 1. Init SDK once at application startup.
await VisioForgeX.InitSDKAsync();

// 2. Enumerate connected devices (async — never block the UI thread).
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

if (videoDevices.Length == 0)
    throw new InvalidOperationException("No video capture devices found.");

// 3. Create the capture engine, bound to a VideoView for live preview.
//    Pass null instead of videoView for headless recording.
var capture = new VideoCaptureCoreX(videoView);

// 4. Configure video + audio sources.
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// 5. Add the MP4 output BEFORE calling StartAsync.
capture.Outputs_Add(new MP4Output("recording.mp4"));

// 6. Start capture — preview renders into videoView immediately.
await capture.StartAsync();

// ... later, when the user clicks Stop:
await capture.StopAsync();     // Finalizes and closes the MP4 file.
await capture.DisposeAsync();  // Releases the pipeline.

// 7. On application shutdown:
VisioForgeX.DestroySDK();
```

Sustituye `VideoCaptureDeviceSourceSettings(...)` por `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` para cámaras IP, o por `new ScreenCaptureD3D11SourceSettings()` para grabación de escritorio (Windows) — el resto de la canalización es idéntico.

## Flujo de trabajo típico

1. **Inicializar** — `await VisioForgeX.InitSDKAsync()` (una vez por aplicación).
2. **Enumerar** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`.
3. **Configurar fuente** — elige una clase de ajustes (`VideoCaptureDeviceSourceSettings`, `RTSPSourceSettings`, `ScreenCaptureD3D11SourceSettings`, `DecklinkVideoSourceSettings`, ...) y asígnala a `capture.Video_Source`.
4. **Crear motor** — `new VideoCaptureCoreX(videoView)` para vista previa, o `new VideoCaptureCoreX(null)` para modo sin UI.
5. **Añadir salidas** — `capture.Outputs_Add(new MP4Output(path))`, o `MKVOutput` / `WebMOutput` / `AVIOutput` / `RTMPOutput` / `HLSOutput`.
6. **Ejecutar** — `StartAsync()` → `StopAsync()` → `DisposeAsync()`, y luego `VisioForgeX.DestroySDK()` al cerrar la aplicación.

## Errores comunes

- **La enumeración de dispositivos es asíncrona.** Usa siempre `await DeviceEnumerator.Shared.VideoSourcesAsync()` — llamar a `.Result` o `.Wait()` en el hilo de la UI puede provocar un interbloqueo. El audio utiliza sus propias llamadas `AudioSourcesAsync()` / `AudioOutputsAsync()`; el enumerador de video no las cubre.
- **Las salidas deben añadirse antes de `StartAsync`.** No se admite `Outputs_Add(...)` durante una captura activa — detén la captura, añade la salida y vuelve a iniciarla.
- **Virtual Camera requiere un registro adicional.** El paquete redistribuible `VirtualCamera` debe registrarse (consulta la [guía de despliegue](./deployment.md)) antes de que la cámara virtual sea visible para aplicaciones externas como Zoom o Teams.
- **Las aplicaciones AnyCPU necesitan los redistribuibles x86 y x64.** En Windows, distribuye ambas arquitecturas o fuerza una específica mediante `<PlatformTarget>` — un despliegue parcial falla en silencio al inicializar la fuente.
- **Los permisos en móviles deben concederse antes de la enumeración.** En Android e iOS, los permisos de cámara y micrófono deben solicitarse y concederse antes de ejecutar `DeviceEnumerator` — de lo contrario la lista devuelta estará vacía. Consulta la [guía de grabación con cámara en MAUI](./maui/camera-recording-maui.md) para la configuración de permisos por plataforma.

## Ver también

- **Fuentes de video**
    - Webcam / cámara USB → [Enumerar y seleccionar un dispositivo](./video-sources/video-capture-devices/enumerate-and-select.md)
    - Cámara IP → [RTSP](./video-sources/ip-cameras/rtsp.md), [ONVIF](./video-sources/ip-cameras/onvif.md), [NDI](./video-sources/ip-cameras/ndi.md)
    - Captura de pantalla → [screen.md](./video-sources/screen.md)
    - Decklink / GenICam → [decklink.md](./video-sources/decklink.md), [USB3 Vision / GigE / GenICam](./video-sources/usb3v-gige-genicam/index.md)
- **Salidas y streaming**
    - Grabación MP4 → [Webcam → MP4](./video-tutorials/video-capture-webcam-mp4.md), [Cámara IP → MP4](./video-tutorials/ip-camera-capture-mp4.md)
    - Streaming de red (RTMP / RTSP / HLS / SRT / NDI) → [network-streaming](./network-streaming/index.md)
- **Plataforma y despliegue**
    - Windows / macOS / Ubuntu / Android / iOS → [../deployment-x/](../deployment-x/index.md)
    - Instalación y frameworks objetivo → [../install/index.md](../install/index.md)
    - Matriz completa de códec × fuente → [../platform-matrix.md](../platform-matrix.md)
- **MAUI**
    - Grabación con cámara multiplataforma → [Grabación con cámara en MAUI](./maui/camera-recording-maui.md)

## Preguntas frecuentes

### ¿Cómo listo las cámaras conectadas?

`var cams = await DeviceEnumerator.Shared.VideoSourcesAsync();` — devuelve una lista de `VideoCaptureDeviceInfo` con nombres legibles, formatos admitidos y tasas de fotogramas.

### ¿Este SDK admite cámaras IP?

Sí. Construye los ajustes mediante la factoría async — `await RTSPSourceSettings.CreateAsync(new Uri("rtsp://..."), login, password, audioEnabled)` — y asígnalos a `VideoCaptureCoreX.Video_Source`. El constructor es privado, por lo que `new RTSPSourceSettings(...)` no compila. También se admiten cámaras ONVIF y HTTP-JPEG. Consulta el [directorio de marcas de cámaras IP](../camera-brands/index.md) para URLs específicas de cada fabricante.

### ¿A qué formatos de archivo puedo grabar?

MP4 (H.264 / H.265 + AAC), MKV, WebM (VP8 / VP9 / AV1 + Opus / Vorbis), AVI, GIF, además de streaming en directo RTMP / RTSP / HLS / SRT / NDI. Consulta las tablas de formatos de salida en [la descripción general del SDK](./index.md).
