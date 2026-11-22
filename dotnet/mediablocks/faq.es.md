---
title: Preguntas frecuentes de Media Blocks SDK .NET — RTSP
description: Respuestas sobre Media Blocks SDK .NET — enviar fotogramas H.264, obtener mapas de bits sin vista, detección de movimiento, latencia y servidores RTSP.
sidebar_label: FAQ
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - RTSP
  - H.264
  - Troubleshooting
primary_api_classes:
  - MediaBlocksPipeline
  - VideoSampleGrabberBlock
  - H264PushSourceBlock
  - CVMotionCellsBlock
  - RTSPSourceSettings
  - RTSPServerBlock
  - VirtualCameraSinkBlock
  - LiveVideoCompositor

---

# Preguntas frecuentes de Media Blocks SDK .NET

Respuestas a las preguntas que los desarrolladores hacen con más frecuencia sobre las canalizaciones de Media Blocks SDK .NET. Los errores de despliegue, como la ausencia de `libgstreamer-1.0` en Windows, están reunidos en [Solución de problemas](../general/troubleshooting.md).

## Entrada y salida de fotogramas

### ¿Cómo reproduzco fotogramas H.264 que mi aplicación recibe por un socket?

Envíelos a la canalización con `H264PushSourceBlock` y conéctelo a un decodificador y a un renderizador. El bloque acepta datos Annex B (flujo de bytes) y AVC (con prefijo de longitud), y reajusta las marcas de tiempo que usted pasa a `PushData()`. Empiece a enviar desde un fotograma clave. El bloque, sus propiedades y una canalización completa se describen en [Bloque Fuente Push H264](Sources/index.md#bloque-fuente-push-h264). Para otros formatos, `PushSourceBlock` con `PushDataSourceSettings` acepta cualquier dato que usted describa con caps de GStreamer.

Implemente `OnError` en la canalización mientras la construye: ahí se notifica un pad que no puede conectarse o un decodificador que rechaza los datos.

### ¿Cómo obtengo fotogramas de varias cámaras como mapas de bits sin mostrarlos?

Sustituya el `VideoRendererBlock` por un `VideoSampleGrabberBlock`; no se necesita ningún `VideoView`:

```csharp
var grabber = new VideoSampleGrabberBlock(VideoFormatX.BGRA, addNullRenderer: true);
grabber.OnVideoFrameBuffer += Grabber_OnVideoFrameBuffer;

pipeline.Connect(rtspSource.VideoOutput, grabber.Input);
```

`addNullRenderer: true` cierra la rama para que la canalización funcione sin pantalla. Use una canalización, o una rama, por cámara. Elija el formato de píxel que espera su tipo de mapa de bits: SkiaSharp y `Bgra32` de WPF usan BGRA, y los mapas de bits de 32 bits de `System.Drawing` también son BGRA en memoria. Procese o copie el fotograma dentro del controlador y libere cada mapa de bits que cree a partir de él: una memoria que crece con cada cámara añadida suele significar que los fotogramas se conservan o se encolan más rápido de lo que se procesan. Consulte [Capturador de muestra de video](VideoProcessing/index.md#capturador-de-muestra-de-video).

## Análisis de video

### ¿Cómo detecto movimiento en una canalización de Media Blocks?

Use `CVMotionCellsBlock`. Necesita el redistribuible OpenCV de su plataforma, por ejemplo el paquete NuGet `VisioForge.CrossPlatform.OpenCV.Windows.x64`, y su salida debe estar conectada: a un renderizador, o a un `NullRendererBlock` cuando no muestra el video:

```csharp
var motion = new CVMotionCellsBlock(new CVMotionCellsSettings());
motion.MotionDetected += Motion_MotionDetected;

pipeline.Connect(source.VideoOutput, motion.Input);
pipeline.Connect(motion.Output, videoRenderer.Input);
```

`CVMotionCellsBlock.IsAvailable()` indica si se encontró el plugin de OpenCV. La configuración y los eventos están en [Bloque CV Motion Cells](OpenCV/index.md#bloque-cv-motion-cells).

## RTSP

### ¿Qué latencia debo establecer para una fuente RTSP?

`RTSPSourceSettings.Latency` es el búfer de jitter de la fuente RTSP y su valor predeterminado es 500 ms. Un valor menor que un fotograma (20 ms para una cámara de 25 fps, cuyos fotogramas están separados 40 ms) no puede funcionar. Para baja latencia, empiece con 100–200 ms y baje solo mientras la imagen siga fluida; `LowLatencyMode = true` desactiva el almacenamiento en búfer adicional y fija la latencia en 150 ms, salvo que usted ya la haya cambiado.

Conecte la rama de audio solo cuando la cámara envíe audio de verdad: `RTSPSourceSettings.CreateAsync` lee la información del flujo por defecto, y `IsAudioAvailable()` le indica entonces si debe añadir el renderizador o el codificador de audio. Una rama de audio que espera un flujo que nunca llega es una causa habitual de canalizaciones que no arrancan. Para un visor completo, consulte [Visor de streams RTSP](Guides/rtsp-player-csharp.md).

### ¿Puedo servir un archivo de video por RTSP, en bucle?

Sí. Construya una canalización con una fuente de archivo, codificadores y un `RTSPServerBlock`; los clientes se conectan entonces a la URL del servidor. Establezca la propiedad `Loop` de la canalización para que el archivo se reinicie al terminar; funciona con fuentes en las que se puede buscar, como los archivos. Consulte [Bloque Servidor RTSP](RTSPServer/index.md).

## Salida hacia otras aplicaciones

### ¿Cómo envío la salida de la canalización, o el Live Video Compositor, a una cámara virtual?

Conecte el video (y, opcionalmente, el audio) a un `VirtualCameraSinkBlock`. Zoom, Teams, OBS y los navegadores lo ven entonces como una webcam. Los filtros Virtual Camera y Virtual Audio Card deben estar registrados antes en el equipo; el bloque puede comprobarlos y registrarlos. Los detalles están en [Sink de cámara virtual](Sinks/index.md#sink-de-camara-virtual), y la [Virtual Camera Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Virtual%20Camera%20Demo) muestra el flujo completo.

Con el Live Video Compositor, envuelva el bloque en una salida y añádala al compositor:

```csharp
var vcBlock = new VirtualCameraSinkBlock(new VirtualCameraSinkSettings());
var output = new LVCVideoAudioOutput("Virtual Camera", compositor, vcBlock, autostart: true);
await compositor.Output_AddAsync(output);
```

La [Live Video Compositor Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo) la incluye como una de sus salidas. Sin una clave de licencia de Virtual Camera SDK, la cámara virtual muestra una franja TRIAL.
