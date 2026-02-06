---
title: Captura ONVIF a MP4 con Postprocesamiento
description: Capturar video desde cámaras IP ONVIF y aplicar efectos de procesamiento de video como redimensionamiento, ajuste de brillo y filtros antes de guardar en MP4.
---

# Capturar MP4 desde Cámara ONVIF con Postprocesamiento

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!!info Muestras de Demo
Para ejemplos completos de trabajo, vea:
- [Demo de Vista Previa RTSP](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo) - Muestra vista previa de cámara ONVIF con postprocesamiento
- [Demo de Captura IP (Video Capture SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/IP%20Capture) - Alternativa usando Video Capture SDK

Para documentación completa de ONVIF, vea la [Guía de Integración de Cámaras IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Tabla de Contenidos

- [Capturar MP4 desde Cámara ONVIF con Postprocesamiento](#capturar-mp4-desde-camara-onvif-con-postprocesamiento)
  - [Tabla de Contenidos](#tabla-de-contenidos)
  - [Resumen](#resumen)
  - [Cuándo Usar Postprocesamiento](#cuando-usar-postprocesamiento)
  - [Prerrequisitos](#prerrequisitos)
  - [Configuración Básica: Descubrimiento y Conexión ONVIF](#configuracion-basica-descubrimiento-y-conexion-onvif)
  - [Ejemplo 1: Redimensionar Video](#ejemplo-1-redimensionar-video)
  - [Ejemplo 2: Aplicar Efectos de Video](#ejemplo-2-aplicar-efectos-de-video)
  - [Ejemplo 3: Desenfoque de Rostro en Tiempo Real](#ejemplo-3-desenfoque-de-rostro-en-tiempo-real)
  - [Ejemplo 4: Marca de Agua y Superposición de Logo](#ejemplo-4-marca-de-agua-y-superposicion-de-logo)
  - [Consideraciones de Rendimiento](#consideraciones-de-rendimiento)
  - [Mejores Prácticas](#mejores-practicas)
  - [Solución de Problemas](#solucion-de-problemas)

## Resumen

Esta guía demuestra cómo capturar video desde cámaras IP ONVIF mientras se aplican varios efectos de postprocesamiento antes de codificar a MP4. A diferencia de la grabación pass-through que preserva el flujo original, el postprocesamiento requiere decodificar el video, aplicar transformaciones y recodificar.

Este enfoque es útil cuando necesita:
- Redimensionar o recortar video
- Aplicar correcciones de brillo, contraste o color
- Agregar marcas de agua o logos
- Desenfocar rostros para privacidad
- Aplicar efectos artísticos o filtros
- Combinar múltiples pasos de procesamiento

## Cuándo Usar Postprocesamiento

**Use postprocesamiento cuando:**
- Necesita redimensionar video (ej. de 4K a 1080p)
- Quiere aplicar efectos de video (brillo, contraste, etc.)
- Necesita agregar superposiciones o marcas de agua
- Los requisitos de privacidad exigen desenfoque de rostros
- Está combinando múltiples flujos de cámara
- Necesita aplicar algoritmos de IA/CV

**Use pass-through (sin postprocesamiento) cuando:**
- Quiere preservar la calidad de video original
- Necesita minimizar el uso de CPU
- El espacio de almacenamiento no es una preocupación
- La duración de grabación es larga (horas/días)

Para grabación pass-through, vea [Guardar Flujo RTSP sin Recodificación](./rtsp-save-original-stream.md).

## Prerrequisitos

1. **VisioForge Media Blocks SDK .NET** instalado
2. **Cámara ONVIF** accesible en su red
3. **Credenciales válidas de cámara** (usuario y contraseña)
4. **Comprensión básica de**:
   - C# async/await
   - Conceptos básicos del protocolo ONVIF
   - Parámetros de codificación de video

## Configuración Básica: Descubrimiento y Conexión ONVIF

Primero, descubra y conéctese a su cámara ONVIF:

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

// Conectar a cámara
var onvifClient = new ONVIFClientX();
bool connected = await onvifClient.ConnectAsync(cameraUrl, "admin", "password");

if (!connected)
{
    Console.WriteLine("Error al conectar a la cámara");
    return;
}

// Obtener URL de flujo RTSP
var profiles = await onvifClient.GetProfilesAsync();
var streamUri = await onvifClient.GetStreamUriAsync(profiles[0]);
string rtspUrl = streamUri.Uri;

Console.WriteLine($"URL RTSP: {rtspUrl}");
```

## Ejemplo 1: Redimensionar Video

Redimensionar video desde cámara ONVIF antes de guardar en MP4:

```cs
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoProcessing;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP desde cámara ONVIF
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";
rtspSource.Transport = RTSPTransport.TCP;

// Bloque de redimensionamiento de video - reducir a 1280x720
var videoResize = new VideoResizeBlock(1280, 720);
videoResize.Mode = VideoResizeMode.Stretch; // O Fit, Fill, Crop

// Codificador H.264
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 2000000; // 2 Mbps
h264Encoder.Framerate = 25;
h264Encoder.Profile = H264Profile.High;
h264Encoder.Level = H264Level.Level41;

// Codificador de audio AAC
var aacEncoder = new AACEncoderBlock();
aacEncoder.Bitrate = 128000; // 128 kbps

// Sumidero MP4
var mp4Sink = new MP4SinkBlock("output_resized.mp4");

// Conectar pipeline de video
rtspSource.VideoOutput.Connect(videoResize.Input);
videoResize.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Conectar pipeline de audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Agregar bloques al pipeline
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(videoResize);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

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

Aplicar ajustes de brillo, contraste, tono y saturación:

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// Bloque de balance de video - ajustar brillo, contraste, saturación, tono
var videoBalance = new VideoBalanceBlock();
videoBalance.Brightness = 0.2; // Rango: -1.0 a 1.0 (0.2 = 20% más brillante)
videoBalance.Contrast = 1.15;   // Rango: 0.0 a 2.0 (1.15 = 15% más contraste)
videoBalance.Saturation = 1.3;  // Rango: 0.0 a 2.0 (1.3 = 30% más saturación)
videoBalance.Hue = 0.0;         // Rango: -1.0 a 1.0 (0 = sin cambio de tono)

// Bloque de efectos de color - aplicar efectos de color preestablecidos
var colorEffects = new ColorEffectsBlock();
colorEffects.Preset = ColorEffectsPreset.Sepia; // Probar: None, Heat, Sepia, XRay, etc.

// Codificador H.264
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000; // 3 Mbps para mayor calidad
h264Encoder.Framerate = 25;

// AAC audio
var aacEncoder = new AACEncoderBlock();
aacEncoder.Bitrate = 128000;

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_enhanced.mp4");

// Conectar pipeline de video con efectos
rtspSource.VideoOutput.Connect(videoBalance.Input);
videoBalance.Output.Connect(colorEffects.Input);
colorEffects.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Conectar audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Agregar todos los bloques
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(videoBalance);
await pipeline.AddAsync(colorEffects);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

// Iniciar
await pipeline.StartAsync();

Console.WriteLine("Grabando con mejoras...");
await Task.Delay(TimeSpan.FromMinutes(5)); // Grabar por 5 minutos

await pipeline.StopAsync();
await pipeline.DisposeAsync();
```

## Ejemplo 3: Desenfoque de Rostro en Tiempo Real

Aplicar detección y desenfoque de rostros para protección de privacidad:

```cs
using VisioForge.Core.MediaBlocks.OpenCV;
using VisioForge.Core.Types.X.OpenCV;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// Bloque CVFaceBlur - detección automática de rostros y desenfoque
var faceBlurSettings = new CVFaceBlurSettings();
faceBlurSettings.ScaleFactor = 1.25;        // Factor de escala de detección
faceBlurSettings.MinNeighbors = 3;          // Vecinos mínimos para detección
faceBlurSettings.MinSize = new Size(30, 30); // Tamaño mínimo de rostro
faceBlurSettings.MainCascadeFile = "haarcascade_frontalface_default.xml";

var faceBlur = new CVFaceBlurBlock(faceBlurSettings);

// Codificador H.264
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000;
h264Encoder.Framerate = 25;

// AAC audio
var aacEncoder = new AACEncoderBlock();

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_face_blur.mp4");

// Conectar pipeline de video
rtspSource.VideoOutput.Connect(faceBlur.Input);
faceBlur.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Agregar bloques
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(faceBlur);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

// Iniciar
await pipeline.StartAsync();
```

## Ejemplo 4: Marca de Agua y Superposición de Logo

Agregar una marca de agua o logo al video:

```cs
using VisioForge.Core.MediaBlocks.VideoProcessing;

// Crear pipeline
var pipeline = new MediaBlocksPipeline();

// Fuente RTSP
var rtspSource = new RTSPSourceBlock(new Uri(rtspUrl));
rtspSource.Username = "admin";
rtspSource.Password = "password";

// Cargar logo/marca de agua
var logoOverlay = new ImageOverlayBlock();
logoOverlay.ImagePath = "watermark.png";
logoOverlay.X = 10;            // 10 píxeles desde la izquierda
logoOverlay.Y = 10;            // 10 píxeles desde arriba
logoOverlay.Opacity = 0.7f;     // 70% opaco

// Superposición de texto para timestamp
var textOverlay = new TextOverlayBlock();
textOverlay.Text = "Cámara 1 - {timestamp}";
textOverlay.FontSize = 24;
textOverlay.FontColor = Color.White;
textOverlay.X = 10;
textOverlay.Y = -50;            // 50 píxeles desde abajo
textOverlay.UpdateInterval = TimeSpan.FromSeconds(1); // Actualizar cada segundo

// Codificador H.264
var h264Encoder = new H264EncoderBlock();
h264Encoder.Bitrate = 3000000;

// AAC audio
var aacEncoder = new AACEncoderBlock();

// Salida MP4
var mp4Sink = new MP4SinkBlock("output_watermarked.mp4");

// Conectar pipeline de video
rtspSource.VideoOutput.Connect(logoOverlay.Input);
logoOverlay.Output.Connect(textOverlay.Input);
textOverlay.Output.Connect(h264Encoder.Input);
h264Encoder.Output.Connect(mp4Sink.VideoInput);

// Audio
rtspSource.AudioOutput.Connect(aacEncoder.Input);
aacEncoder.Output.Connect(mp4Sink.AudioInput);

// Agregar bloques
await pipeline.AddAsync(rtspSource);
await pipeline.AddAsync(logoOverlay);
await pipeline.AddAsync(textOverlay);
await pipeline.AddAsync(h264Encoder);
await pipeline.AddAsync(aacEncoder);
await pipeline.AddAsync(mp4Sink);

// Iniciar
await pipeline.StartAsync();
```

## Consideraciones de Rendimiento

1. **Uso de CPU**: El procesamiento de video es intensivo en CPU. Cada efecto agrega sobrecarga:
   - Redimensionamiento simple: ~10-20% CPU por flujo
   - Corrección de color: ~5-15% CPU
   - Detección de rostros: ~30-50% CPU (depende de resolución)
   - Múltiples efectos: Uso de CPU aditivo

2. **Aceleración GPU**: Use codificadores acelerados por hardware cuando estén disponibles:
   ```cs
   // Codificación GPU NVIDIA
   var nvencEncoder = new NVENCEncoderBlock();
   nvencEncoder.Bitrate = 4000000;
   nvencEncoder.Preset = NVENCPreset.P4; // Equilibrar calidad/rendimiento
   ```

3. **Uso de Memoria**: Las resoluciones más altas requieren más memoria. Monitoree el uso:
   ```cs
   pipeline.MemoryWarning += (sender, e) =>
   {
       Console.WriteLine($"Advertencia de memoria: {e.UsageMB} MB usados");
   };
   ```

4. **Equilibrio de Configuración de Codificación**:
   - **Calidad**: Bitrate más alto, preset más lento = mejor calidad, más CPU
   - **Rendimiento**: Bitrate más bajo, preset más rápido = calidad más baja, menos CPU
   - **Tamaño de Archivo**: El bitrate afecta directamente el tamaño del archivo

## Mejores Prácticas

1. **Probar Rendimiento Primero**:
   - Comenzar con pipeline simple
   - Agregar efectos uno a la vez
   - Monitorear uso de CPU/memoria
   - Ajustar configuraciones basadas en hardware

2. **Elegir Bitrates Apropiados**:
   - 720p: 1-2 Mbps
   - 1080p: 2-4 Mbps
   - 4K: 8-15 Mbps

3. **Manejar Errores con Elegancia**:
   ```cs
   pipeline.Error += (sender, e) =>
   {
       Console.WriteLine($"Error de pipeline: {e.Message}");
       // Intentar recuperación o limpieza
   };
   ```

4. **Disponer Recursos**:
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

5. **Monitorear Estado de Pipeline**:
   ```cs
   pipeline.StateChanged += (sender, e) =>
   {
       Console.WriteLine($"Estado de pipeline: {e.State}");
   };
   ```

## Solución de Problemas

**Uso Alto de CPU:**
- Reducir resolución de video
- Bajar preset del codificador (codificación más rápida)
- Remover efectos innecesarios
- Usar aceleración GPU si está disponible

**Frames Perdidos:**
- Verificar si CPU está saturado
- Reducir frame rate
- Bajar bitrate
- Simplificar pipeline de procesamiento

**Calidad de Video Deficiente:**
- Aumentar bitrate
- Usar preset de codificador más lento
- Verificar calidad de video fuente
- Verificar ancho de banda de red para RTSP

**Fugas de Memoria:**
- Asegurar disposición apropiada de bloques
- Verificar referencias circulares
- Monitorear grabaciones de larga duración

**Efectos No Aplicados:**
- Verificar conexiones de bloques
- Verificar que parámetros de efecto sean válidos
- Asegurar que bloques estén agregados al pipeline
- Revisar orden del pipeline (cadena de efectos)

---
Para grabación más simple sin postprocesamiento, vea [Guardar Flujo RTSP sin Recodificación](./rtsp-save-original-stream.md).
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para ejemplos completos de trabajo.