---
title: Video Capture SDK para C# .NET - Webcam, pantalla e IP
description: Video Capture SDK para C# y .NET — graba webcam, pantalla, cámara IP (RTSP/ONVIF) a MP4. API multiplataforma con codificación GPU. Reemplaza DirectShow.
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

order: 15
---

# Video Capture SDK para C# .NET — API de captura de webcam, pantalla y cámara IP

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El Video Capture SDK para .NET es una API de captura de video en C# que permite grabar desde webcams, cámaras IP (RTSP/ONVIF) y pantallas en aplicaciones .NET. Reemplaza el código de captura de video DirectShow de bajo nivel con una API async moderna — enumera dispositivos, configura fuentes e inicia la grabación en pocas líneas de C#.

El SDK gestiona la enumeración de dispositivos, negociación de formatos, codificación acelerada por GPU y salida de archivos en Windows, macOS y Linux. Ya sea que necesite guardar un stream RTSP en archivo, capturar fotos desde una webcam o construir una herramienta de captura de pantalla, la API lo cubre.

## Inicio rápido

### 1. Instalar paquetes NuGet

```bash
dotnet add package VisioForge.DotNet.VideoCapture
```

Agregue dependencias nativas específicas de plataforma:

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.16]"/>
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.16]"/>

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="[2026.11.22]"/>

<!-- Linux x64 (requiere GStreamer 1.20+ del sistema) -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="[2026.10.26]"/>
```

En Linux ARM64, `VisioForge.CrossPlatform.Core.Linux.arm64` sustituye al paquete `.Linux.x64`; consulte la [implementación en Ubuntu](../deployment-x/Ubuntu.md) para un fragmento de proyecto que elige uno según el identificador de runtime.

Para la lista completa de paquetes y soporte de frameworks de UI (WinForms, WPF, MAUI, Avalonia), consulte la [Guía de Instalación](../install/index.md).

### 2. Inicializar el SDK

Llame a `InitSDKAsync()` una vez al iniciar la aplicación antes de usar cualquier funcionalidad de captura. En una aplicación WPF o WinForms, espérelo con `await` en un controlador `async` del evento `Loaded` de la ventana (WPF) o del evento `Load` del formulario (WinForms), antes de crear el objeto de captura:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Agregar un VideoView

La vista previa se muestra en un control `VideoView` llamado `videoView`. En WPF y WinForms el control está en el ensamblado `VisioForge.Core`, que `VisioForge.DotNet.VideoCapture` ya incluye, por lo que no necesita ningún paquete de UI adicional.

WPF: en `MainWindow.xaml`, declare el espacio de nombres `vf` en el elemento `<Window>` y coloque el control:

```xml
<Window ...
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core">
    <Grid>
        <vf:VideoView x:Name="videoView" Background="Black" />
    </Grid>
</Window>
```

WinForms: cree el control en el código del formulario:

```csharp
// Form1.cs
private readonly VisioForge.Core.UI.WinForms.VideoView videoView =
    new VisioForge.Core.UI.WinForms.VideoView { Dock = DockStyle.Fill };

public Form1()
{
    InitializeComponent();
    Controls.Add(videoView); // agregue esta línea al constructor existente
}
```

Para MAUI, Avalonia, WinUI y Uno, consulte la [Guía de Instalación](../install/index.md).

### 4. Captura de webcam a MP4 en C# (ejemplo mínimo)

Las directivas `using` van al principio del archivo; el resto se ejecuta desde un controlador de eventos `async` de la ventana o el formulario, como el evento `Loaded` de la ventana (WPF), el evento `Load` del formulario (WinForms) o el controlador `Click` de un botón. Si las líneas de detención se ejecutan desde otro controlador (un botón Detener), guarde `capture` en un campo en lugar de una variable local.

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
capture.Audio_Play = false; // solo grabar, sin escucha por los altavoces

// Agregar salida MP4 (H.264 + AAC, aceleración GPU cuando esté disponible)
capture.Outputs_Add(new MP4Output("recording.mp4"));

// Con una licencia comprada, llame a SetLicenseCertificateAsync aquí (paso 5)

// Iniciar captura
await capture.StartAsync();

// ... cuando termine:
await capture.StopAsync();
await capture.DisposeAsync();
```

### 5. Aplicar su licencia

Sin licencia, el SDK funciona como una versión de prueba de 30 días. Con la compra recibe un archivo de certificado `.vflicense`, no una clave. Cárguelo en cada instancia de captura antes de `StartAsync()`; licenciar una instancia no licencia otra. El código siguiente lo lee de la carpeta de la aplicación: agregue `your.vflicense` al proyecto y establezca su propiedad **Copiar en el directorio de salida** en Visual Studio:

```csharp
var licensePath = System.IO.Path.Combine(System.AppContext.BaseDirectory, "your.vflicense");
await capture.SetLicenseCertificateAsync(System.IO.File.ReadAllBytes(licensePath));
```

Consulte [Licencias](https://www.visioforge.com/licensing) para más detalles.

### 6. Limpieza al cerrar

Llame a `VisioForgeX.DestroySDK()` una vez al salir de la aplicación, después de detener y liberar todas las instancias de captura, por ejemplo en el evento `Closed` de la ventana (WPF) o en el evento `FormClosed` del formulario (WinForms):

```csharp
VisioForgeX.DestroySDK();
```

## Flujo de trabajo principal

Toda aplicación de captura sigue el mismo patrón:

1. **Inicializar SDK** — `VisioForgeX.InitSDKAsync()` (una vez por vida de la aplicación)
2. **Enumerar dispositivos** — `DeviceEnumerator.Shared.VideoSourcesAsync()` / `AudioSourcesAsync()`
3. **Crear objeto de captura** — `new VideoCaptureCoreX(videoView)` para vista previa, o sin vista para grabación sin interfaz
4. **Configurar fuente** — Establecer `Video_Source` a una webcam, cámara IP, pantalla u otra fuente
5. **Configurar salida** — Agregar una o más salidas: `MP4Output`, `WebMOutput`, `AVIOutput`, etc.
6. **Iniciar** — `await capture.StartAsync()`
7. **Detener** — `await capture.StopAsync()` finaliza el archivo de salida
8. **Liberar** — `await capture.DisposeAsync()` libera todos los recursos

## Escenarios comunes de captura de video en C\#

### Grabar video de webcam en C\#

Grabe desde una webcam USB o cámara integrada a MP4 con H.264/AAC:

```csharp
capture.Video_Source = new VideoCaptureDeviceSourceSettings(device);
capture.Outputs_Add(new MP4Output("webcam.mp4"));
```

Ver tutorial completo: [Guardar Video de Webcam en C#](guides/save-webcam-video.md)

### Guardar stream RTSP en archivo con C\#

Conéctese a cámaras IP y guarde streams RTSP en archivos MP4. Soporta protocolos RTSP, ONVIF y HTTP:

```csharp
// RTSPSourceSettings tiene ctor privado — constrúyalo vía la factoría async
// para que la fuente pueda descubrir códecs antes de iniciar el pipeline.
capture.Video_Source = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.100:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);
capture.Outputs_Add(new MP4Output("ipcam.mp4"));
```

Ver: [Cámara IP a MP4](video-tutorials/ip-camera-capture-mp4.md) | [Guía RTSP](video-sources/ip-cameras/rtsp.md) | [Guía ONVIF](video-sources/ip-cameras/onvif.md)

### Captura de pantalla a MP4 en C\#

Grabe el escritorio o una región específica de pantalla a MP4:

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

### Grabación solo de audio

Grabe desde un micrófono sin video:

```csharp
capture.Video_Source = null; // desactivar video para captura solo de audio
capture.Audio_Source = audioDevices[0].CreateSourceSettingsVC();
capture.Audio_Record = true;
capture.Audio_Play = false; // solo grabar, sin escucha por los altavoces
capture.Outputs_Add(new MP4Output("audio.mp4"));
```

### Capturar foto desde webcam

Capture una imagen fija del feed de video en vivo de la webcam:

```csharp
capture.Snapshot_Grabber_Enabled = true;
await capture.StartAsync();

// Después, guardar un fotograma:
await capture.Snapshot_SaveAsync("photo.jpg", SKEncodedImageFormat.Jpeg, 85);
```

Ver: [Captura de Fotos con Webcam](guides/make-photo-using-webcam.md)

## Dispositivos de entrada soportados

### Cámaras y hardware de captura

* Webcams USB y dispositivos de captura (hasta 4K)
* Videocámaras DV y HDV MPEG-2 (Windows, motor clásico `VideoCaptureCore`)
* Tarjetas de captura PCI profesionales
* Sintonizadores de televisión con y sin codificador MPEG (Windows, motor clásico `VideoCaptureCore`, [guía](video-sources/tv-tuner.md))

### Cámaras de red e IP

* Cámaras IP habilitadas para RTSP
* Cámaras HTTP JPEG/MJPEG
* Fuentes de streaming RTMP
* Cámaras compatibles con ONVIF
* Fuentes SRT y NDI

Consulte nuestro [directorio de marcas de cámaras IP](../camera-brands/index.md) para URLs RTSP específicas por marca y guías de conexión cubriendo más de 60 fabricantes.

### Equipos profesionales e industriales

* Dispositivos de captura Blackmagic Decklink
* Cámaras industriales GenICam / GigE Vision / USB3 Vision

### Fuentes de audio

* Dispositivos de captura de audio del sistema y tarjetas de sonido
* Dispositivos de audio ASIO profesionales
* Captura de audio loopback (sonido del sistema)

## Formatos de salida y codificadores

El SDK soporta múltiples contenedores y códecs de salida. La codificación acelerada por GPU (NVIDIA NVENC, AMD AMF, Intel Quick Sync) se usa automáticamente cuando está disponible.

| Formato | Códecs de Video | Códecs de Audio | Caso de Uso |
| ------- | --------------- | --------------- | ----------- |
| MP4 | H.264, H.265 (HEVC) | AAC | Propósito general, mayor compatibilidad |
| WebM | VP8, VP9, AV1 | Vorbis, Opus | Entrega web, libre de regalías |
| AVI | MJPEG, códecs personalizados | PCM, MP3 | Compatibilidad legacy |
| MKV | H.264, H.265, VP9 | AAC, Vorbis, FLAC | Contenedor flexible, múltiples pistas |
| GIF | GIF animado | — | Clips cortos, vistas previas |

Para configuración detallada de códecs, consulte [Formatos de Salida](../general/output-formats/mp4.md) y [Codificadores de Video](../general/video-encoders/h264.md).

## Soporte de plataformas

| Plataforma | Frameworks de UI | Notas |
| ---------- | ---------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Consola | Conjunto completo de características incluyendo fuentes DirectShow |
| macOS | MAUI, Avalonia, Consola | Acceso a cámara AVFoundation |
| Linux x64, ARM64 | Avalonia, Consola | Acceso a cámara V4L2 |
| Android | MAUI | Vía integración de cámara MAUI |
| iOS | MAUI | Vía integración de cámara MAUI |

## Migración desde captura de video DirectShow

Si actualmente usa DirectShow (DirectShow.NET) para captura de video en C#, el Video Capture SDK proporciona un reemplazo moderno. En lugar de construir manualmente grafos de filtros con `IGraphBuilder`, conectar pines y gestionar objetos COM, usa clases de configuración tipadas y métodos async.

| Concepto DirectShow | Equivalente en Video Capture SDK |
| ------------------- | -------------------------------- |
| Grafo de filtros `IGraphBuilder` | Gestionado automáticamente por `VideoCaptureCoreX` |
| `ICaptureGraphBuilder2` | No necesario — fuente y salida configuradas vía propiedades |
| Enumeración de dispositivos vía `DsDevice` | `DeviceEnumerator.Shared.VideoSourcesAsync()` |
| `ISampleGrabber` para acceso a fotogramas | `Snapshot_SaveAsync()` o evento `OnVideoFrameBuffer` |
| Conexión manual de pines | Automática — o use [Media Blocks SDK](../mediablocks/GettingStarted/index.md) para pipelines explícitos |
| Limpieza COM / `Marshal.ReleaseComObject` | Patrón estándar `IAsyncDisposable` |

Para una guía de migración detallada con ejemplos de código lado a lado, consulte [Migrar desde DirectShow.NET](https://www.visioforge.com/compare/migrate-from-directshow-net).

## Documentación para desarrolladores

### Funcionalidad principal

* [Configurando Fuentes de Video](video-sources/index.md)
* [Configurando Fuentes de Audio](audio-sources/index.md)
* [Procesamiento de Video y Efectos](video-processing/index.md)
* [Renderizado de Audio](audio-rendering/index.md)

### Características avanzadas

* [Implementación de Captura de Video](video-capture/index.md)
* [Implementación de Captura de Audio](audio-capture/index.md)
* [Detección de Movimiento](motion-detection/index.md)
* [Streaming de Red](network-streaming/index.md)
* [Grabación Pre-Evento](guides/pre-event-recording.md)
* [Sincronización de Múltiples Capturas](guides/start-in-sync.md)

### Tutoriales (paso a paso con código)

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

## Recursos para desarrolladores

* [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [Guías de Despliegue](deployment.md)
* [Referencia de API](https://api.visioforge.org/dotnet/api/index.html)
* [Registro de Cambios](../changelog.md)
* [Contrato de Licencia de Usuario Final](../../eula.md)
* [Información de Licenciamiento](https://www.visioforge.com/es/licensing)
