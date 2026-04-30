---
title: Video Capture SDK para C# .NET - Webcam, Pantalla e IP
description: SDK de captura de video para C# y .NET — graba webcam, pantalla, cámara IP (RTSP/ONVIF) a MP4. API multiplataforma con codificación GPU. Reemplaza DirectShow.
sidebar_label: Video Capture SDK .NET
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

# Video Capture SDK para C# .NET — API de Captura de Webcam, Pantalla y Cámara IP

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El Video Capture SDK para .NET es una API de captura de video en C# que permite grabar desde webcams, cámaras IP (RTSP/ONVIF) y pantallas en aplicaciones .NET. Reemplaza el código de captura de video DirectShow de bajo nivel con una API async moderna — enumera dispositivos, configura fuentes e inicia la grabación en pocas líneas de C#.

El SDK gestiona la enumeración de dispositivos, negociación de formatos, codificación acelerada por GPU y salida de archivos en Windows, macOS y Linux. Ya sea que necesites guardar un stream RTSP en archivo, capturar fotos desde una webcam o construir una herramienta de captura de pantalla, la API lo cubre.

## Inicio Rápido

### 1. Instalar Paquetes NuGet

```bash
dotnet add package VisioForge.DotNet.VideoCapture
```

Agrega dependencias nativas específicas de plataforma:

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

Para la lista completa de paquetes y soporte de frameworks de UI (WinForms, WPF, MAUI, Avalonia), consulta la [Guía de Instalación](../install/index.md).

### 2. Inicializar el SDK

Llama a `InitSDKAsync()` una vez al iniciar la aplicación antes de usar cualquier funcionalidad de captura:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Captura de Webcam a MP4 en C# (Ejemplo Mínimo)

```csharp
using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;

// Crear instancia de captura vinculada a un control VideoView
var capture = new VideoCaptureCoreX(videoView);

// Enumerar dispositivos disponibles
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Configurar fuentes de video y audio
capture.Video_Source = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;

// Agregar salida MP4 (H.264 + AAC, aceleración GPU cuando esté disponible)
capture.Outputs_Add(new MP4Output("recording.mp4"));

// Iniciar captura
await capture.StartAsync();

// ... cuando termine:
await capture.StopAsync();
await capture.DisposeAsync();
```

### 4. Limpieza al Cerrar

```csharp
VisioForgeX.DestroySDK();
```

## Flujo de Trabajo Principal

Toda aplicación de captura sigue el mismo patrón:

1. **Inicializar SDK** — `VisioForgeX.InitSDKAsync()` (una vez por vida de la aplicación)
2. **Enumerar dispositivos** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`
3. **Crear objeto de captura** — `new VideoCaptureCoreX(videoView)` para vista previa, o sin vista para grabación sin interfaz
4. **Configurar fuente** — Establecer `Video_Source` a una webcam, cámara IP, pantalla u otra fuente
5. **Configurar salida** — Agregar una o más salidas: `MP4Output`, `WebMOutput`, `AVIOutput`, etc.
6. **Iniciar** — `await capture.StartAsync()`
7. **Detener** — `await capture.StopAsync()` finaliza el archivo de salida
8. **Liberar** — `await capture.DisposeAsync()` libera todos los recursos

## Escenarios Comunes de Captura de Video en C#

### Grabar Video de Webcam en C#

Graba desde una webcam USB o cámara integrada a MP4 con H.264/AAC:

```csharp
capture.Video_Source = new VideoCaptureDeviceSourceSettings(device);
capture.Outputs_Add(new MP4Output("webcam.mp4"));
```

Ver tutorial completo: [Guardar Video de Webcam en C#](guides/save-webcam-video.md)

### Guardar Stream RTSP en Archivo con C#

Conéctate a cámaras IP y guarda streams RTSP en archivos MP4. Soporta protocolos RTSP, ONVIF y HTTP:

```csharp
// RTSPSourceSettings tiene ctor privado — constrúyelo vía la factoría async
// para que la fuente pueda descubrir códecs antes de iniciar el pipeline.
capture.Video_Source = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.100:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);
capture.Outputs_Add(new MP4Output("ipcam.mp4"));
```

Ver: [Cámara IP a MP4](video-tutorials/ip-camera-capture-mp4.md) | [Guía RTSP](video-sources/ip-cameras/rtsp.md) | [Guía ONVIF](video-sources/ip-cameras/onvif.md)

### Captura de Pantalla a MP4 en C#

Graba el escritorio o una región específica de pantalla a MP4:

```csharp
// La captura de pantalla con X-engine en Windows usa ScreenCaptureD3D11SourceSettings
// (otras plataformas: ScreenCaptureGDISourceSettings, ScreenCaptureMacOSSourceSettings,
// ScreenCaptureXDisplaySourceSettings — todas implementan IVideoCaptureBaseVideoSourceSettings).
capture.Video_Source = new ScreenCaptureD3D11SourceSettings
{
    FrameRate     = new VideoFrameRate(30),
    CaptureCursor = true,
};
capture.Outputs_Add(new MP4Output("screen.mp4"));
```

Ver: [Captura de Pantalla a MP4](video-tutorials/screen-capture-mp4.md)

### Grabación Solo de Audio

Graba desde un micrófono sin video:

```csharp
capture.Video_Source = null; // desactivar video para captura solo de audio
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;
capture.Outputs_Add(new MP4Output("audio.mp4"));
```

### Capturar Foto desde Webcam

Captura una imagen fija del feed de video en vivo de la webcam:

```csharp
capture.Snapshot_Grabber_Enabled = true;
await capture.StartAsync();

// Después, guardar un fotograma:
await capture.Snapshot_SaveAsync("photo.jpg", SKEncodedImageFormat.Jpeg, 85);
```

Ver: [Captura de Fotos con Webcam](guides/make-photo-using-webcam.md)

## Dispositivos de Entrada Soportados

### Cámaras y Hardware de Captura

* Webcams USB y dispositivos de captura (hasta 4K)
* Videocámaras DV y HDV MPEG-2
* Tarjetas de captura PCI profesionales
* Sintonizadores de televisión (con y sin codificador MPEG)

### Cámaras de Red e IP

* Cámaras IP habilitadas para RTSP
* Cámaras HTTP JPEG/MJPEG
* Fuentes de streaming RTMP
* Cámaras compatibles con ONVIF
* Fuentes SRT y NDI

Consulta nuestro [directorio de marcas de cámaras IP](../camera-brands/index.md) para URLs RTSP específicas por marca y guías de conexión cubriendo más de 60 fabricantes.

### Equipos Profesionales e Industriales

* Dispositivos de captura Blackmagic Decklink
* Microsoft Kinect y Kinect 2
* Cámaras industriales GenICam / GigE Vision / USB3 Vision

### Fuentes de Audio

* Dispositivos de captura de audio del sistema y tarjetas de sonido
* Dispositivos de audio ASIO profesionales
* Captura de audio loopback (sonido del sistema)

## Formatos de Salida y Codificadores

El SDK soporta múltiples contenedores y códecs de salida. La codificación acelerada por GPU (NVIDIA NVENC, AMD AMF, Intel Quick Sync) se usa automáticamente cuando está disponible.

| Formato | Códecs de Video | Códecs de Audio | Caso de Uso |
| ------- | --------------- | --------------- | ----------- |
| MP4 | H.264, H.265 (HEVC) | AAC | Propósito general, mayor compatibilidad |
| WebM | VP8, VP9, AV1 | Vorbis, Opus | Entrega web, libre de regalías |
| AVI | MJPEG, códecs personalizados | PCM, MP3 | Compatibilidad legacy |
| MKV | H.264, H.265, VP9 | AAC, Vorbis, FLAC | Contenedor flexible, múltiples pistas |
| GIF | GIF animado | — | Clips cortos, vistas previas |

Para configuración detallada de códecs, consulta [Formatos de Salida](../general/output-formats/mp4.md) y [Codificadores de Video](../general/video-encoders/h264.md).

## Soporte de Plataformas

| Plataforma | Frameworks de UI | Notas |
| ---------- | ---------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Consola | Conjunto completo de características incluyendo fuentes DirectShow |
| macOS | MAUI, Avalonia, Consola | Acceso a cámara AVFoundation |
| Linux x64 | Avalonia, Consola | Acceso a cámara V4L2 |
| Android | MAUI | Vía integración de cámara MAUI |
| iOS | MAUI | Vía integración de cámara MAUI |

## Migración desde Captura de Video DirectShow

Si actualmente usas DirectShow (DirectShow.NET) para captura de video en C#, el Video Capture SDK proporciona un reemplazo moderno. En lugar de construir manualmente grafos de filtros con `IGraphBuilder`, conectar pines y gestionar objetos COM, usas clases de configuración tipadas y métodos async.

| Concepto DirectShow | Equivalente en Video Capture SDK |
| ------------------- | -------------------------------- |
| Grafo de filtros `IGraphBuilder` | Gestionado automáticamente por `VideoCaptureCoreX` |
| `ICaptureGraphBuilder2` | No necesario — fuente y salida configuradas vía propiedades |
| Enumeración de dispositivos vía `DsDevice` | `DeviceEnumerator.Shared.VideoSourcesAsync()` |
| `ISampleGrabber` para acceso a fotogramas | `Snapshot_SaveAsync()` o evento `OnVideoFrameBuffer` |
| Conexión manual de pines | Automática — o usa [Media Blocks SDK](../mediablocks/GettingStarted/index.md) para pipelines explícitos |
| Limpieza COM / `Marshal.ReleaseComObject` | Patrón estándar `IAsyncDisposable` |

Para una guía de migración detallada con ejemplos de código lado a lado, consulta [Migrar desde DirectShow.NET](https://www.visioforge.com/compare/migrate-from-directshow-net).

## Documentación para Desarrolladores

### Funcionalidad Principal

* [Configurando Fuentes de Video](video-sources/index.md)
* [Configurando Fuentes de Audio](audio-sources/index.md)
* [Procesamiento de Video y Efectos](video-processing/index.md)
* [Renderizado de Audio](audio-rendering/index.md)

### Características Avanzadas

* [Implementación de Captura de Video](video-capture/index.md)
* [Implementación de Captura de Audio](audio-capture/index.md)
* [Detección de Movimiento](motion-detection/index.md)
* [Streaming de Red](network-streaming/index.md)
* [Grabación Pre-Evento](guides/pre-event-recording.md)
* [Sincronización de Múltiples Capturas](guides/start-in-sync.md)

### Tutoriales (Paso a Paso con Código)

* [Webcam a MP4](video-tutorials/video-capture-webcam-mp4.md) | [a AVI](video-tutorials/video-capture-camera-avi.md) | [a WMV](video-tutorials/video-capture-webcam-wmv.md)
* [Captura de Pantalla a MP4](video-tutorials/screen-capture-mp4.md) | [a AVI](video-tutorials/screen-capture-avi.md) | [a WMV](video-tutorials/screen-capture-wmv.md)
* [Vista Previa Cámara IP](video-tutorials/ip-camera-preview.md) | [Cámara IP a MP4](video-tutorials/ip-camera-capture-mp4.md)
* [Overlay de Texto en Webcam](video-tutorials/webcam-capture-text-overlay.md)
* [Vista Previa de Video de Cámara](video-tutorials/camera-video-preview.md)
* [Todos los Tutoriales de Video](video-tutorials/index.md)

### Guías

* [Guardar Video de Webcam en C#](guides/save-webcam-video.md)
* [Grabar Webcam en VB.NET](guides/record-webcam-vb-net.md)
* [Captura de Pantalla en VB.NET](guides/screen-capture-vb-net.md)
* [Captura de Fotos con Webcam](guides/make-photo-using-webcam.md)
* [Todas las Guías](guides/index.md)

### Integración

* [Integración con Software de Terceros](3rd-party-software/index.md)
* [Visión por Computadora (Detección Facial)](computer-vision/index.md)
* [Grabación de Cámara MAUI](maui/camera-recording-maui.md)

## Recursos para Desarrolladores

* [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Guías de Despliegue](deployment.md)
* [Referencia de API](https://api.visioforge.org/dotnet/api/index.html)
* [Registro de Cambios](../changelog.md)
* [Contrato de Licencia de Usuario Final](../../eula.md)
* [Información de Licenciamiento](../../../licensing.md)
