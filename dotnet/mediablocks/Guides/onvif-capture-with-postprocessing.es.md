---
title: Captura de Cámara IP ONVIF a MP4 con Efectos en C#
description: Capture video de cámaras IP ONVIF y aplique redimensionamiento, brillo y filtros antes de guardar a MP4 con VisioForge Media Blocks SDK para .NET.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Recording
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - AAC
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - RTSPSourceBlock
  - RTSPSourceSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - MP4SinkBlock

---

# Captura MP4 de Cámara ONVIF con Postprocesamiento

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Soporte multiplataforma"
    El Media Blocks SDK funciona en **Windows, macOS, Linux, Android e iOS** a través de GStreamer. Consulte la [matriz de soporte de plataformas](../../platform-matrix.md) para detalles de códecs y aceleración por hardware, y la [guía de despliegue para Linux](../../deployment-x/Ubuntu.md) para Ubuntu / NVIDIA Jetson / Raspberry Pi.

!!!info Ejemplos de demostración
Para ejemplos de trabajo completos, consulte:
- [RTSP Preview Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) — muestra vista previa de cámara ONVIF con postprocesamiento
- [IP Capture Demo (Video Capture SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) — alternativa usando Video Capture SDK

Para documentación completa de ONVIF, consulte la [Guía de Integración de Cámaras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Tabla de Contenidos

- [Captura MP4 de Cámara ONVIF con Postprocesamiento](#captura-mp4-de-camara-onvif-con-postprocesamiento)
  - [Tabla de Contenidos](#tabla-de-contenidos)
  - [Resumen](#resumen)
  - [Cuándo Usar Postprocesamiento](#cuando-usar-postprocesamiento)
  - [Requisitos Previos](#requisitos-previos)
  - [Configuración Básica: Descubrimiento y Conexión ONVIF](#configuracion-basica-descubrimiento-y-conexion-onvif)
  - [Ejemplo 1: Redimensionar Video](#ejemplo-1-redimensionar-video)
  - [Ejemplo 2: Aplicar Efectos de Video](#ejemplo-2-aplicar-efectos-de-video)
  - [Ejemplo 3: Desenfoque de Rostros en Tiempo Real](#ejemplo-3-desenfoque-de-rostros-en-tiempo-real)
  - [Ejemplo 4: Marca de Agua y Superposición de Logo](#ejemplo-4-marca-de-agua-y-superposicion-de-logo)
  - [Consideraciones de Rendimiento](#consideraciones-de-rendimiento)
  - [Mejores Prácticas](#mejores-practicas)
  - [Solución de Problemas](#solucion-de-problemas)

## Resumen

Esta guía demuestra cómo capturar video de cámaras IP ONVIF aplicando varios efectos de postprocesamiento antes de codificar a MP4. A diferencia de la grabación pass-through que preserva el stream original, el postprocesamiento requiere decodificar el video, aplicar transformaciones y volver a codificar.

Este enfoque es útil cuando necesita:
- Redimensionar o recortar video
- Aplicar correcciones de brillo, contraste o color
- Agregar marcas de agua o logos
- Desenfocar rostros por privacidad
- Aplicar efectos artísticos o filtros
- Combinar múltiples pasos de procesamiento

## Cuándo Usar Postprocesamiento

**Use postprocesamiento cuando:**
- Necesita redimensionar video (ej. de 4K a 1080p)
- Quiere aplicar efectos de video (brillo, contraste, etc.)
- Necesita agregar superposiciones o marcas de agua
- Los requisitos de privacidad demandan desenfoque de rostros
- Está combinando múltiples feeds de cámaras
- Necesita aplicar algoritmos IA/CV

**Use pass-through (sin postprocesamiento) cuando:**
- Quiere preservar la calidad original del video
- Necesita minimizar el uso de CPU
- El espacio de almacenamiento no es una preocupación
- La duración de grabación es larga (horas/días)

Para grabación pass-through, consulte [Guardar Stream RTSP sin Re-codificar](./rtsp-save-original-stream.md).

## Requisitos Previos

1. **VisioForge Media Blocks SDK .NET** instalado
2. **Cámara ONVIF** accesible en su red
3. **Credenciales válidas de cámara** (usuario y contraseña)
4. **Conocimiento básico de**:
   - C# async/await
   - Protocolo ONVIF
   - Parámetros de codificación de video

## Configuración Básica: Descubrimiento y Conexión ONVIF

Primero, descubra y conecte con su cámara ONVIF:

```cs
using VisioForge.Core.ONVIFDiscovery;
using VisioForge.Core.ONVIFX;

// Descubrir cámaras ONVIF
var discovery = new Discovery();
var cts = new CancellationTokenSource();
string cameraUrl = null;

await discovery.Discover(5, (device) =>
{
    if (device.XAdresses?.Any() == true)
    {
        cameraUrl = device.XAdresses.FirstOrDefault();
        Console.WriteLine($"Cámara encontrada: {cameraUrl}");
    }
}, cts.Token);

if (string.IsNullOrEmpty(cameraUrl))
{
    Console.WriteLine("No se encontraron cámaras ONVIF");
    return;
}

// Conectar a la cámara
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(cameraUrl, "admin", "password");

if (!connected)
{
    Console.WriteLine("Error al conectar con la cámara");
    return;
}

// Obtener URL del stream RTSP
var profiles = await onvifClient.GetProfilesAsync();
var streamUri = await onvifClient.GetStreamUriAsync(profiles[0]);
string rtspUrl = streamUri.Uri;

Console.WriteLine($"URL RTSP: {rtspUrl}");
```

## Ejemplo 1: Redimensionar Video

Redimensione el video de una cámara ONVIF antes de guardarlo a MP4. La API de
Media Blocks recibe objetos de configuración — `RTSPSourceBlock` se construye
desde un `RTSPSourceSettings`, el codificador desde su objeto de settings, el
sink MP4 entrega pads de entrada mediante `CreateNewInput(...)`, y los enlaces
se declaran sobre el pipeline.

```cs
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP de la cámara ONVIF. Credenciales y latencia viven en el objeto
// settings; use la factoría async para que el settings descubra la info de códecs.
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Bloque de redimensionamiento — reducir a 1280x720. El ctor (ancho, alto) es un
// atajo para `new VideoResizeBlock(new ResizeVideoEffect(w, h))`.
var videoResize = new VideoResizeBlock(1280, 720);

// Codificador H.264. Elige una clase concreta de settings — Bitrate está en Kbit/s (2000 = 2 Mbps).
// OpenH264EncoderSettings funciona en todas las plataformas; cámbialo por NVENC / QSV / AMF / MFH264 para aceleración GPU.
var h264Settings = new OpenH264EncoderSettings { Bitrate = 2000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// Codificador de audio AAC (Bitrate en Kbit/s — 128 = 128 kbps).
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Sink MP4 — el ctor por nombre de archivo es la ruta más corta a un escritor MP4 válido.
var mp4Sink = new MP4SinkBlock("output_resized.mp4");

// Agregar cada bloque al pipeline antes de cablear los enlaces
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoResize);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Cablear el camino de video (RTSP video → resize → H.264 → pad de video del MP4)
pipeline.Connect(rtspSource.VideoOutput, videoResize.Input);
pipeline.Connect(videoResize.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Cablear el camino de audio (RTSP audio → AAC → pad de audio del MP4)
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar grabación
await pipeline.StartAsync();

Console.WriteLine("Grabando con redimensionamiento... Presione Enter para detener.");
Console.ReadLine();

// Detener y limpiar
await pipeline.StopAsync();
await pipeline.DisposeAsync();

Console.WriteLine("Grabación completa: output_resized.mp4");
```

## Ejemplo 2: Aplicar Efectos de Video

Aplique ajustes de brillo, contraste, tono y saturación. Los bloques de
procesamiento reciben un objeto settings en el ctor; los settings llevan las
perillas.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Bloque de balance de video — las perillas viven en el objeto settings.
// Brightness: -1.0..1.0 (0.2 = ligeramente más brillante)
// Contrast:    0.0..2.0 (1.15 = +15% contraste)
// Saturation:  0.0..2.0 (1.3  = +30% saturación)
// Hue:        -1.0..1.0 (0.0  = sin cambio)
var balanceSettings = new VideoBalanceVideoEffect
{
    Brightness = 0.2,
    Contrast   = 1.15,
    Saturation = 1.3,
    Hue        = 0.0,
};
var videoBalance = new VideoBalanceBlock(balanceSettings);

// El bloque de efectos de color recibe un preset directamente en el ctor
var colorEffects = new ColorEffectsBlock(ColorEffectsPreset.Sepia);

// Codificador H.264 (3 Mbps)
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_enhanced.mp4");

// Agregar todo al pipeline, luego cablear
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(videoBalance);
pipeline.AddBlock(colorEffects);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Cadena de video: RTSP → balance → color-effects → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, videoBalance.Input);
pipeline.Connect(videoBalance.Output, colorEffects.Input);
pipeline.Connect(colorEffects.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Cadena de audio
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar
await pipeline.StartAsync();

Console.WriteLine("Grabando con mejoras...");
await Task.Delay(TimeSpan.FromMinutes(5)); // Grabar por 5 minutos

await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Ejemplo 3: Desenfoque de Rostros en Tiempo Real

Aplique detección y desenfoque de rostros para proteger la privacidad:

```cs
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// CVFaceBlurBlock — detección y desenfoque automático de rostros vía OpenCV
var faceBlurSettings = new CVFaceBlurSettings
{
    ScaleFactor      = 1.25,
    MinNeighbors     = 3,
    MinSize          = new Size(30, 30),
    MainCascadeFile  = "haarcascade_frontalface_default.xml",
};
var faceBlur = new CVFaceBlurBlock(faceBlurSettings);

// Codificador H.264
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_face_blur.mp4");

// Agregar + cablear
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(faceBlur);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

pipeline.Connect(rtspSource.VideoOutput, faceBlur.Input);
pipeline.Connect(faceBlur.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar
await pipeline.StartAsync();
```

## Ejemplo 4: Marca de Agua y Superposición de Logo

Añada una marca de agua y una superposición de texto. `ImageOverlayBlock` recibe
un nombre de archivo o un `ImageOverlaySettings`; la posición/transparencia
viven en el settings. `TextOverlayBlock` siempre recibe un `TextOverlaySettings`.

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.Types.X.VideoEffects;
using SkiaSharp;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri(rtspUrl), "admin", "password", audioEnabled: true);
var rtspSource = new RTSPSourceBlock(rtspSettings);

// Logo / marca de agua: el ctor por nombre de archivo carga la imagen. Las
// perillas de posición viven en el settings; la transparencia es Alpha (0..1).
var logoOverlay = new ImageOverlayBlock(new ImageOverlaySettings("watermark.png")
{
    X     = 10,   // 10 px desde la izquierda
    Y     = 10,   // 10 px desde arriba
    Alpha = 0.7,  // 0..1
});

// Texto estático. TextOverlaySettings lleva Text, posición, Color (SKColor) y
// perillas de fuente — consulte la referencia de OverlayManagerText para todas.
var textOverlay = new TextOverlayBlock(new TextOverlaySettings("Camera 1")
{
    Color = SKColors.White,
});

// Codificador H.264
var h264Settings = new OpenH264EncoderSettings { Bitrate = 3000 };
var h264Encoder = new H264EncoderBlock(h264Settings);

// AAC audio
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings { Bitrate = 128 });

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_watermarked.mp4");

// Agregar + cablear
pipeline.AddBlock(rtspSource);
pipeline.AddBlock(logoOverlay);
pipeline.AddBlock(textOverlay);
pipeline.AddBlock(h264Encoder);
pipeline.AddBlock(aacEncoder);
pipeline.AddBlock(mp4Sink);

// Cadena de video: RTSP → logo → texto → H.264 → MP4
pipeline.Connect(rtspSource.VideoOutput, logoOverlay.Input);
pipeline.Connect(logoOverlay.Output, textOverlay.Input);
pipeline.Connect(textOverlay.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Cadena de audio
pipeline.Connect(rtspSource.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar
await pipeline.StartAsync();
```

## Consideraciones de Rendimiento

1. **Uso de CPU**: El procesamiento de video consume CPU. Cada efecto añade sobrecarga:
   - Redimensionamiento simple: ~10-20% CPU por stream
   - Corrección de color: ~5-15% CPU
   - Detección de rostros: ~30-50% CPU (depende de la resolución)
   - Múltiples efectos: uso aditivo de CPU

2. **Aceleración por GPU**: Use codificadores acelerados por hardware cuando estén
   disponibles. `H264EncoderBlock.GetDefaultSettings()` ya prefiere NVENC / QSV /
   AMF cuando la plataforma los soporta, pero puede forzar un backend específico:

   ```cs
   // Codificador H.264 NVIDIA NVENC (Bitrate en Kbit/s — 4000 = 4 Mbps)
   var nvencSettings = new NVENCH264EncoderSettings { Bitrate = 4000 };
   var h264Encoder   = new H264EncoderBlock(nvencSettings);
   ```

3. **Manejo de errores**: Suscríbase a `OnError` para conocer los fallos del pipeline:

   ```cs
   pipeline.OnError += (sender, e) =>
   {
       Console.WriteLine($"Error del pipeline: {e.Message}");
   };
   ```

4. **Balance de ajustes de codificación**:
   - **Calidad**: mayor bitrate, preset más lento = mejor calidad, más CPU
   - **Rendimiento**: menor bitrate, preset más rápido = menor calidad, menos CPU
   - **Tamaño del archivo**: el bitrate afecta directamente al tamaño

## Mejores Prácticas

1. **Pruebe el rendimiento primero**:
   - Comience con un pipeline simple
   - Añada efectos uno a la vez
   - Monitoree uso de CPU/memoria
   - Ajuste según el hardware

2. **Elija bitrates apropiados** (todos los valores en Kbit/s):
   - 720p: 1000-2000
   - 1080p: 2000-4000
   - 4K: 8000-15000

3. **Libere recursos**:

   ```cs
   try
   {
       await pipeline.StartAsync();
       // ... grabación ...
   }
   finally
   {
       await pipeline.StopAsync();
       await pipeline.DisposeAsync();
       onvifClient?.Dispose();
   }
   ```

4. **Observe los eventos de ciclo de vida del pipeline**:

   ```cs
   pipeline.OnStart  += (s, e) => Console.WriteLine("Pipeline iniciado");
   pipeline.OnStop   += (s, e) => Console.WriteLine("Pipeline detenido");
   pipeline.OnPause  += (s, e) => Console.WriteLine("Pipeline pausado");
   pipeline.OnResume += (s, e) => Console.WriteLine("Pipeline reanudado");
   ```

## Solución de Problemas

**Alto uso de CPU:**
- Reducir la resolución de video
- Preset de codificación más rápido
- Eliminar efectos innecesarios
- Usar aceleración por GPU si está disponible

**Cuadros perdidos:**
- Verificar si la CPU está saturada
- Reducir la tasa de cuadros
- Reducir el bitrate
- Simplificar el pipeline de procesamiento

**Mala calidad de video:**
- Aumentar bitrate
- Usar preset de codificación más lento
- Verificar la calidad del video fuente
- Verificar el ancho de banda de red para RTSP

**Fugas de memoria:**
- Asegurar la liberación adecuada de bloques
- Verificar referencias circulares
- Monitorear grabaciones largas

**Los efectos no se aplican:**
- Verificar las conexiones de bloques con `pipeline.Connect(outPad, inPad)`
- Comprobar que los parámetros del efecto son válidos
- Asegurar que cada bloque se registra vía `pipeline.AddBlock(...)` antes de `StartAsync`
- Revisar el orden del pipeline (cadena de efectos)

---
Para grabación más simple sin postprocesamiento, consulte [Guardar Stream RTSP sin Re-codificar](./rtsp-save-original-stream.md).
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos completos.
