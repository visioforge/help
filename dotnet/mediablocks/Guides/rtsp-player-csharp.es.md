---
title: Visor de Streams RTSP y Reproductor de Cámaras IP en C# .NET
description: Cree un visor RTSP y reproductor de cámaras IP en C# con VisioForge Media Blocks SDK: vista previa en vivo, ONVIF y grabación passthrough.
---

# Visor de Streams RTSP y Reproductor de Cámaras IP en C#

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

Esta guía le muestra cómo construir una aplicación de visor de streams RTSP y reproductor de cámaras IP en C# usando el SDK VisioForge Media Blocks. Aprenderá a conectarse a cámaras IP vía RTSP, mostrar video en vivo con audio, descubrir cámaras usando ONVIF y grabar streams a archivo — con o sin recodificación. El SDK Media Blocks funciona en Windows, macOS y Linux, por lo que el mismo código funciona en todas las plataformas.

Los casos de uso comunes incluyen paneles de vigilancia, aplicaciones NVR (grabador de video en red), herramientas de gestión de cámaras y cualquier proyecto que necesite mostrar o grabar feeds de cámaras IP programáticamente.

!!!info Ejemplos de Demo
    Para ejemplos funcionales completos, vea los [demos RTSP en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK): RTSP Preview Demo, RTSP RAW Capture Demo y RTSP MultiView Demo.
!!!

## Requisitos Previos

Agregue el paquete NuGet del SDK Media Blocks a su proyecto:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

También necesita los paquetes de runtime específicos de la plataforma. Para Windows:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

Para otras plataformas (macOS, Linux, Android, iOS), consulte la [Guía de Despliegue](../../deployment-x/index.md).

## Vista Previa RTSP en Vivo

El patrón principal conecta una fuente RTSP a renderizadores de video y audio a través de un `MediaBlocksPipeline`. Inicialice el SDK una vez al inicio de la aplicación, luego cree pipelines según sea necesario.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

// Inicializar SDK una vez al inicio
await VisioForgeX.InitSDKAsync();

// Crear el pipeline
var pipeline = new MediaBlocksPipeline();

// Conectar a la cámara RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/Streaming/Channels/101"),
    "admin",          // usuario
    "password",       // contraseña
    true);            // audio habilitado

var rtspSource = new RTSPSourceBlock(rtspSettings);

// Crear renderizador de video (VideoView1 es un control IVideoView en su formulario)
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

// Crear renderizador de audio (opcional)
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);

// Iniciar reproducción
await pipeline.StartAsync();
```

Para detener la reproducción y liberar recursos:

```csharp
await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Autenticación RTSP

La mayoría de las cámaras IP requieren credenciales para acceder al stream de video. Puede proporcionar autenticación de dos maneras.

**Credenciales como parámetros** — pase nombre de usuario y contraseña a `RTSPSourceSettings.CreateAsync()`:

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin",      // nombre de usuario
    "mypassword", // contraseña
    true);        // audio habilitado
```

**Credenciales en la URL** — incorpórelas directamente en la URI RTSP:

```csharp
var settings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://admin:mypassword@192.168.1.21:554/stream"),
    null, null, true);
```

Para cámaras ONVIF, autentíquese a través de `ONVIFClientX` para recuperar la URI del stream automáticamente — vea la sección [Descubrimiento de Cámaras ONVIF](#descubrimiento-de-camaras-onvif) a continuación. La mayoría de las cámaras usan autenticación digest por defecto. Si encuentra fallas de conexión, verifique que el modo de autenticación de su cámara coincida (digest vs basic) y que el puerto RTSP (típicamente 554) sea accesible.

## Modo de Baja Latencia

Para monitoreo en tiempo real o control PTZ, habilite el modo de baja latencia para reducir el retraso del stream de los ~250ms por defecto a 60–120ms:

```csharp
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

rtspSettings.LowLatencyMode = true;

var rtspSource = new RTSPSourceBlock(rtspSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
videoRenderer.IsSync = false; // Deshabilitar sincronización A/V para menor latencia

pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);
```

Para configuración detallada de buffers y transporte UDP vs TCP, consulte la referencia de [Configuración del Protocolo RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md).

## Descubrimiento de Cámaras ONVIF

Descubra automáticamente cámaras IP en la red local usando el protocolo de descubrimiento ONVIF, luego obtenga las URIs de stream e información de la cámara.

```csharp
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Descubrir cámaras en la red (timeout de 5 segundos)
var discovery = new Discovery();
var cameras = await discovery.Discover(5);

foreach (var camera in cameras)
{
    Console.WriteLine($"Cámara encontrada: {camera.XAdresses?.FirstOrDefault()}");
}

// Conectar a una cámara específica vía ONVIF
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(
    "http://192.168.1.21/onvif/device_service",
    "admin",
    "password");

if (connected)
{
    // Obtener información de la cámara
    var info = onvifClient.DeviceInformation;
    Console.WriteLine($"Cámara: {info?.Model}, Serie: {info?.SerialNumber}");

    // Obtener perfiles disponibles y URI del stream
    var profiles = await onvifClient.GetProfilesAsync();
    if (profiles?.Length > 0)
    {
        var mediaUri = await onvifClient.GetStreamUriAsync(profiles[0]);
        Console.WriteLine($"URI del Stream: {mediaUri.Uri}");
    }
}
```

Para control PTZ, múltiples perfiles y funciones ONVIF avanzadas, consulte la guía de [Integración de Cámaras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md).

## Opciones de Grabación

Hay dos enfoques para grabar streams RTSP, cada uno adecuado para diferentes casos de uso:

| | Passthrough (Sin Recodificación) | Recodificación |
| --- | --- | --- |
| Uso de CPU | Mínimo | Alto |
| Calidad de video | Original (sin pérdida) | Recomprimida |
| Tamaño de archivo | Igual que la fuente | Bitrate configurable |
| Procesamiento de video | Sin superposiciones, redimensión o efectos | Capacidad de edición completa |
| Ideal para | Archivo de vigilancia, NVR | Streaming, post-procesamiento |

### Grabación con Passthrough (Sin Recodificación)

La grabación passthrough guarda el stream comprimido original directamente a un archivo sin decodificar ni recodificar el video. Este enfoque usa `RTSPRAWSourceBlock` en lugar de `RTSPSourceBlock` — los datos de video pasan directamente de la cámara al archivo de salida sin carga de CPU para el procesamiento de video.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;

// Crear un pipeline separado para grabación
var recordPipeline = new MediaBlocksPipeline();

// Crear fuente RTSP RAW (recibe stream comprimido directamente)
var rawSettings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"),
    "admin", "password", true);

var rawSource = new RTSPRAWSourceBlock(rawSettings);

// Crear sink de archivo MP4
var muxer = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));

// Conectar video — sin decodificación, sin recodificación
var videoPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Video);
recordPipeline.Connect(rawSource.VideoOutput, videoPad);

// Conectar audio — recodificar a AAC para compatibilidad MP4
var audioPad = (muxer as IMediaBlockDynamicInputs)
    .CreateNewInput(MediaBlockPadMediaType.Audio);

var decodeBin = new DecodeBinBlock(false, true, false);
var aacEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

recordPipeline.Connect(rawSource.AudioOutput, decodeBin.Input);
recordPipeline.Connect(decodeBin.AudioOutput, aacEncoder.Input);
recordPipeline.Connect(aacEncoder.Output, audioPad);

await recordPipeline.StartAsync();
```

**Elección del formato contenedor:** Use MP4 para amplia compatibilidad de reproducción. Use MPEG-TS (`MPEGTSSinkBlock`) para grabación segura ante fallos — si el proceso termina inesperadamente, todos los datos escritos hasta ese punto se preservan. MPEG-TS es preferido para grabación de vigilancia 24/7.

Para una implementación completa con manejo de errores, gestión de estado y limpieza de recursos, consulte la guía [Guardar Stream RTSP Sin Recodificación](rtsp-save-original-stream.md).

### Grabación con Recodificación

Cuando necesite redimensionar el video, agregar superposiciones o marcas de agua, cambiar el codec o reducir el bitrate, use `RTSPSourceBlock` (que decodifica el stream) seguido de bloques de codificación:

```csharp
// RTSPSourceBlock decodifica el stream, permitiendo procesamiento
var rtspSource = new RTSPSourceBlock(rtspSettings);
var mp4Sink = new MP4SinkBlock("output.mp4");

// Video: decodificado → (procesamiento opcional) → codificar a H.264 → mux
// Audio: decodificado → codificar a AAC → mux

pipeline.Connect(rtspSource.VideoOutput, /* bloques de codificación/procesamiento */);
```

Para ejemplos de código detallados de grabación con procesamiento de video (redimensionar, efectos, detección facial), consulte la guía [Captura ONVIF con Post-procesamiento](onvif-capture-with-postprocessing.md). Para un ejemplo más simple de recodificación, vea el tutorial [Captura de Cámara IP a MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md).

## Vista Multi-Cámara

Para mostrar múltiples cámaras IP simultáneamente, cree instancias independientes de `MediaBlocksPipeline` — una por cámara. Cada pipeline se ejecuta en su propio hilo y puede iniciarse y detenerse independientemente.

```csharp
// Crear pipelines separados para cada cámara
var pipeline1 = new MediaBlocksPipeline();
var pipeline2 = new MediaBlocksPipeline();

// Cámara 1
var source1 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.21:554/stream"), "admin", "pass1", true));
var renderer1 = new VideoRendererBlock(pipeline1, VideoView1);
pipeline1.Connect(source1.VideoOutput, renderer1.Input);

// Cámara 2
var source2 = new RTSPSourceBlock(await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/stream"), "admin", "pass2", true));
var renderer2 = new VideoRendererBlock(pipeline2, VideoView2);
pipeline2.Connect(source2.VideoOutput, renderer2.Input);

// Iniciar ambos
await pipeline1.StartAsync();
await pipeline2.StartAsync();
```

El [RTSP MultiView Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) en GitHub muestra un diseño de cuadrícula con controles de grabación por cámara y grabación passthrough.

## Preguntas Frecuentes

### ¿Cómo me autentico con una cámara IP RTSP en C#?

Incorpore las credenciales directamente en la URL RTSP (`rtsp://usuario:contraseña@ip:554/ruta`) o páselas como parámetros separados a `RTSPSourceSettings.CreateAsync()`. Para cámaras ONVIF, conéctese vía `ONVIFClientX` primero con nombre de usuario y contraseña, luego obtenga la URI del stream autenticada usando `GetStreamUriAsync()`. La mayoría de las cámaras soportan autenticación digest por defecto.

### ¿Debo usar passthrough o recodificación al grabar streams RTSP?

Use passthrough para archivo de vigilancia y aplicaciones NVR — requiere cero CPU para procesamiento de video y preserva la calidad original de la cámara. Use recodificación cuando necesite redimensionar, agregar superposiciones, cambiar el codec o bitrate, o transmitir a un formato diferente. La mayoría de las aplicaciones profesionales de grabación usan passthrough para minimizar la carga del servidor.

### ¿Cómo reduzco la latencia del stream RTSP por debajo de 100ms?

Habilite `LowLatencyMode = true` en `RTSPSourceSettings` y deshabilite la sincronización del renderizador de video con `IsSync = false`. Use transporte UDP cuando su red lo soporte. Latencia esperada: 60–120ms vs los 250ms por defecto. Consulte la [guía del protocolo RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) para opciones avanzadas de ajuste de buffers.

### ¿Puedo ver y grabar de múltiples cámaras IP simultáneamente?

Sí. Cree instancias separadas de `MediaBlocksPipeline` para cada cámara — cada pipeline se ejecuta independientemente con su propia conexión RTSP, decodificador y renderizador. Puede agregar grabación por cámara creando pipelines de grabación adicionales usando `RTSPRAWSourceBlock` para captura passthrough. El RTSP MultiView Demo en GitHub muestra una implementación completa con diseño de cuadrícula y controles de grabación individuales.

### ¿Qué formato contenedor debo usar para grabación passthrough — MP4 o MPEG-TS?

Use MP4 para compatibilidad estándar de reproducción en dispositivos y reproductores. Use MPEG-TS para grabación segura ante fallos — si la aplicación o el sistema falla durante la grabación, todos los datos escritos hasta el punto de falla se preservan. Para grabación de vigilancia 24/7 o grabación de misión crítica, MPEG-TS es la opción recomendada.

## Ver También

- [Guardar Stream RTSP Sin Recodificación](rtsp-save-original-stream.md) — grabación passthrough detallada con implementación completa de la clase RTSPRecorder
- [Integración de Cámaras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — descubrimiento ONVIF, perfiles, control PTZ
- [Configuración del Protocolo RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — UDP vs TCP, configuración de buffers, ajuste de baja latencia
- [Captura de Cámara IP a MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) — tutorial de grabación con recodificación
- [Captura ONVIF con Post-procesamiento](onvif-capture-with-postprocessing.md) — redimensionar, efectos, desenfoque facial durante la grabación
- [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — demos de vista previa, captura y multi-vista RTSP
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — página del producto y descargas
