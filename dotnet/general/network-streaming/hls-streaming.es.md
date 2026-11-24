---
title: HLS Streaming en Vivo en C# .NET — Playlist m3u8 y Segmentos
description: Genere listas .m3u8 y segmentos .ts con servidor HTTP integrado. Integración con HLS.js, iOS AVPlayer y Android ExoPlayer. Ejemplos con VisioForge SDK.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCoreX
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - HLS
  - TS
  - H.264
  - AAC
  - C#
primary_api_classes:
  - HLSOutput
  - HLSSinkSettings
  - MediaBlockPadMediaType
  - HLSPlaylistType
  - H264EncoderBlock

video:
  id: Cw3UCgzu2wI
  upload_date: 2026-11-24T20:18:00Z
  duration: PT1M

---

# Guía Completa para Implementación de Transmisión de Red HLS en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Agentes de IA: usa el servidor MCP de VisioForge"

    ¿Lo construyes con **Claude Code**, **Cursor** u otro agente de IA?
    Conecta al servidor MCP público de VisioForge
    ([documentación](../mcp-server-usage.md))
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## ¿Qué es HTTP Live Streaming (HLS)?

HTTP Live Streaming (HLS) es un protocolo de comunicaciones de transmisión de bitrate adaptativo diseñado y desarrollado por Apple Inc. Introducido por primera vez en 2009, se ha convertido en uno de los protocolos de transmisión más ampliamente adoptados en varias plataformas y dispositivos. HLS funciona dividiendo el flujo general en una secuencia de pequeñas descargas de archivos basadas en HTTP, cada una conteniendo un segmento corto del contenido general.

### Características Clave de Transmisión HLS

- **Transmisión de Bitrate Adaptativo**: HLS ajusta automáticamente la calidad de video basada en las condiciones de red del espectador, asegurando calidad de reproducción óptima sin buffering.
- **Compatibilidad Multiplataforma**: Funciona en iOS, macOS, Android, Windows y la mayoría de navegadores web modernos.
- **Entrega Basada en HTTP**: Aprovecha infraestructura de servidor web estándar, permitiendo que el contenido pase a través de firewalls y servidores proxy.
- **Cifrado de Medios y Autenticación**: Soporta protección de contenido a través de cifrado y varios métodos de autenticación.
- **Contenido en Vivo y Bajo Demanda**: Puede usarse tanto para transmisión en vivo como para medios pre-grabados.

### Estructura Técnica HLS

La entrega de contenido HLS se basa en tres componentes clave:

1. **Archivo de Manifiesto (.m3u8)**: Un archivo de lista de reproducción que contiene metadatos sobre los varios flujos disponibles
2. **Archivos de Segmento (.ts)**: El contenido de medios real, dividido en pequeños chunks (típicamente 2-10 segundos cada uno)
3. **Servidor HTTP**: Responsable de entregar tanto archivos de manifiesto como de segmento

Dado que HLS es completamente basado en HTTP, necesitará un servidor HTTP dedicado o puede usar el servidor interno ligero proporcionado por nuestros SDK.

## Implementando Transmisión HLS con Media Blocks SDK

El Media Blocks SDK ofrece un enfoque completo para transmisión HLS a través de su arquitectura de pipeline, dando a los desarrolladores control granular sobre cada aspecto del proceso de transmisión.

### La versión corta

El vídeo de un minuto ejecuta esta aplicación de consola y reproduce el flujo en un navegador.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Cw3UCgzu2wI?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Una aplicación de consola completa: transmite en directo por HLS la primera cámara web hasta que se pulsa Enter. `SystemVideoSourceBlock` captura la cámara y `H264EncoderBlock` codifica el vídeo en H.264. `HLSSinkBlock` divide el flujo en segmentos MPEG-TS de unos cinco segundos (`TargetDuration`, cortados en fotogramas clave) y los escribe, con la lista de reproducción `playlist.m3u8`, en la subcarpeta `hls` de la carpeta de la aplicación; conserva en disco los diez últimos segmentos y en la lista los cinco últimos. Antes, la aplicación borra los segmentos y la lista que dejó allí una ejecución anterior, para que un reproductor nunca tome el flujo antiguo. Con `Custom_HTTP_Server_Enabled`, el servidor HTTP integrado del sink sirve esa subcarpeta, y solo ella, en el puerto 8088: abra `http://localhost:8088/` en un navegador. El flujo solo lleva vídeo; el ejemplo siguiente añade un micrófono con `AACEncoderBlock`.

```csharp
using System;
using System.IO;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();
await using var p = new MediaBlocksPipeline();
p.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var cams = await DeviceEnumerator.Shared
    .VideoSourcesAsync();
var cam = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(
        cams[0]));
var h264 = new H264EncoderBlock();
var dir = Path.Combine(
    AppContext.BaseDirectory, "hls");
Directory.CreateDirectory(dir);
var old = Directory.GetFiles(dir, "*.ts");
foreach (var f in old) File.Delete(f);
var m3u8 = Path.Combine(dir, "playlist.m3u8");
File.Delete(m3u8);
var hls = new HLSSinkBlock(new HLSSinkSettings
{
    Location = Path.Combine(
        dir, "segment_%05d.ts"),
    PlaylistLocation = m3u8,
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port = 8088
});
p.Connect(cam.Output, h264.Input);
p.Connect(h264.Output, hls.CreateNewInput(
    MediaBlockPadMediaType.Video));
if (!await p.StartAsync()) return;
Console.WriteLine("http://localhost:8088/");
Console.ReadLine();
await p.StopAsync();
```

La subcarpeta `hls` contiene también `index.htm`, copiado allí al compilar. El servidor lo envía como página predeterminada de la carpeta, y reproduce la lista con [hls.js](https://github.com/video-dev/hls.js), o con la reproducción HLS propia del navegador cuando hls.js no puede ejecutarse (iPhone antiguos, o si el script de la CDN no se carga):

```html
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="utf-8">
  <title>Live HLS from a webcam</title>
  <script src="https://cdn.jsdelivr.net/npm/hls.js@1.7.4/dist/hls.min.js"></script>
</head>
<body style="margin:0;background:#000">
  <video id="video" controls autoplay muted playsinline style="display:block;width:100%;height:100vh"></video>
  <script>
    // El servidor HTTP integrado de la aplicación sirve esta página, la lista y los segmentos desde la carpeta hls.
    const video = document.getElementById("video");
    if (window.Hls && Hls.isSupported()) {
      const hls = new Hls();
      hls.loadSource("playlist.m3u8");
      hls.attachMedia(video);
    } else {
      // Sin hls.js (sin MSE, o la CDN no cargó): la reproducción HLS propia del navegador, si la tiene.
      video.src = "playlist.m3u8";
    }
  </script>
</body>
</html>
```

El primer segmento se escribe unos cinco segundos después del inicio, así que una página abierta antes aún no encuentra la lista: recárguela. `ffprobe` sobre un segmento muestra `mpegts` con vídeo H.264 en la resolución y la frecuencia de fotogramas que negoció la cámara. Sin derechos de administrador, el servidor solo responde en este equipo (`localhost`). Para acceder desde otros dispositivos, ejecute dos comandos una vez como administrador: reserve el puerto con `netsh http add urlacl url=http://*:8088/ user=<DOMINIO>\<usuario> listen=yes` y ábralo en el Firewall de Windows con `netsh advfirewall firewall add rule name="HLS 8088" dir=in action=allow protocol=TCP localport=8088`.

El proyecto completo es [hls-webcam-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/hls-webcam-short-console), una aplicación de consola para Windows x64 que hace referencia a `VisioForge.DotNet.MediaBlocks` y al paquete nativo del SDK para esa plataforma, `VisioForge.CrossPlatform.Core.Windows.x64`.

### Creando un Flujo HLS Básico

El siguiente ejemplo demuestra cómo configurar un flujo HLS usando Media Blocks SDK:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.VideoEncoders;

// Inicializa el motor multiplataforma una vez por proceso.
VisioForgeX.InitSDK();

const string URL = "http://localhost:8088/";

// Pipeline (adjunta el manejador de errores antes de añadir bloques).
var pipeline = new MediaBlocksPipeline();
pipeline.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");

// Fuente de video: primer dispositivo de captura enumerado.
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Fuente de audio: primer dispositivo de captura de audio enumerado.
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Codificadores H.264 y AAC.
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
var aacEncoder = new AACEncoderBlock();

// Sumidero HLS.
var settings = new HLSSinkSettings
{
    Location             = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles             = 10,
    PlaylistLength       = 5,
    PlaylistLocation     = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot         = URL.TrimEnd('/'),
    SendKeyframeRequests = true,
    TargetDuration       = TimeSpan.FromSeconds(5),  // TimeSpan, no int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port    = 8088
};

var hlsSink = new HLSSinkBlock(settings);

// Conectar el grafo: fuente → codificador → sumidero HLS (pads separados para video/audio).
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(h264Encoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(aacEncoder.Output, hlsSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar. Abre http://localhost:8088/playlist.m3u8 en un reproductor o navegador.
await pipeline.StartAsync();
```

### Opciones de Configuración Avanzada

El Media Blocks SDK ofrece varias opciones de configuración avanzada para transmisión HLS:

- **Variantes de Bitrate Múltiple**: Crear diferentes niveles de calidad para transmisión adaptativa
- **Duración de Segmento Personalizado**: Optimizar para diferentes tipos de contenido y entornos de visualización
- **Opciones del Lado del Servidor**: Configurar encabezados de control de caché y otros comportamientos del servidor
- **Características de Seguridad**: Implementar autenticación basada en token o cifrado

Puede usar este SDK para transmitir tanto captura de video en vivo como archivos de medios existentes a HLS. La arquitectura de pipeline flexible permite una amplia personalización del flujo de trabajo de procesamiento de medios.

## Transmisión HLS con Video Capture SDK .NET

El Video Capture SDK .NET proporciona un enfoque simplificado para transmisión HLS específicamente diseñado para fuentes de video en vivo como webcams, tarjetas de captura y otros dispositivos de entrada.

### Implementación VideoCaptureCoreX

El motor VideoCaptureCoreX ofrece un enfoque moderno, orientado a objetos para captura de video y transmisión:

```csharp
// Crear configuraciones de sumidero HLS
var settings = new HLSSinkSettings
{
    Location = Path.Combine(AppContext.BaseDirectory, "segment_%05d.ts"),
    MaxFiles = 10,
    PlaylistLength = 5,
    PlaylistLocation = Path.Combine(AppContext.BaseDirectory, "playlist.m3u8"),
    PlaylistRoot = edStreamingKey.Text,
    SendKeyframeRequests = true,
    TargetDuration = TimeSpan.FromSeconds(5), // TimeSpan, no int
    Custom_HTTP_Server_Enabled = true,
    Custom_HTTP_Server_Port = new Uri(edStreamingKey.Text).Port
};

// Crear salida HLS
var hlsOutput = new HLSOutput(settings);

// Crear codificadores de video y audio con configuraciones predeterminadas
hlsOutput.Video = new OpenH264EncoderSettings();
hlsOutput.Audio = new AVENCAACEncoderSettings();

// Agregar salida HLS al objeto de captura de video
videoCapture.Outputs_Add(hlsOutput, true);
```

### Implementación VideoCaptureCore

Para aquellos trabajando con el motor tradicional VideoCaptureCore, la implementación es ligeramente diferente pero igualmente directa:

```csharp
VideoCapture1.Network_Streaming_Enabled = true;
VideoCapture1.Network_Streaming_Audio_Enabled = true;
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Duración de segmento en segundos
        NumSegments = 5,                        // Número de segmentos en lista de reproducción
        OutputFolder = "c:\\hls\\",             // Carpeta de salida
        PlaylistType = HLSPlaylistType.Live,    // Tipo de lista de reproducción
        Custom_HTTP_Server_Enabled = true,      // Usar servidor HTTP interno
        Custom_HTTP_Server_Port = 8088          // Puerto para servidor HTTP interno
    }
};

VideoCapture1.Network_Streaming_Output = hls;
```

### Consideraciones de Rendimiento

Cuando transmita con Video Capture SDK, considere estas técnicas de optimización de rendimiento:

- Mantenga duraciones de segmento entre 2-10 segundos (10 segundos es óptimo para la mayoría de casos de uso)
- Ajuste el número de segmentos basado en patrones de visualización esperados
- Use aceleración por hardware cuando esté disponible para codificación
- Configure bitrates apropiados basado en velocidades de conexión de su audiencia objetivo

## Convirtiendo Archivos de Medios a HLS con Video Edit SDK .NET

El Video Edit SDK .NET permite a los desarrolladores convertir archivos de medios existentes al formato HLS para transmisión, ideal para aplicaciones de video bajo demanda.

### Implementación VideoEditCore

```csharp
VideoEdit1.Network_Streaming_Enabled = true;
VideoEdit1.Network_Streaming_Audio_Enabled = true;
VideoEdit1.Network_Streaming_Format = NetworkStreamingFormat.HLS;

var hls = new HLSOutput
{
    HLS =
    {
        SegmentDuration = 10,                   // Duración de segmento en segundos
        NumSegments = 5,                        // Número de segmentos en lista de reproducción
        OutputFolder = "c:\\hls\\",             // Carpeta de salida
        PlaylistType = HLSPlaylistType.Live,    // Tipo de lista de reproducción
        Custom_HTTP_Server_Enabled = true,      // Usar servidor HTTP interno
        Custom_HTTP_Server_Port = 8088          // Puerto para servidor HTTP interno
    }
};

VideoEdit1.Network_Streaming_Output = hls;
```

### Consideraciones de Formato de Archivo

Cuando convierta archivos a HLS, considere estos factores:

- No todos los formatos de entrada son igualmente eficientes para conversión
- Los archivos MP4, MOV y MKV típicamente proporcionan los mejores resultados
- Los formatos altamente comprimidos pueden requerir más poder de procesamiento
- Considere pre-transcodificar archivos muy grandes a un formato intermedio

## Reproducción e Integración

### Integración de Reproductor HTML5

Todas las aplicaciones implementando transmisión HLS deberían incluir un archivo HTML con un reproductor de video. Los reproductores HTML5 modernos como HLS.js, Video.js o JW Player proporcionan excelente soporte para flujos HLS.

Aquí hay un ejemplo básico usando HLS.js:

```html
<!DOCTYPE html>
<html>
<head>
    <title>Reproductor HLS</title>
    <script src="https://cdn.jsdelivr.net/npm/hls.js@latest"></script>
</head>
<body>
    <video id="video" controls></video>
    <script>
      var video = document.getElementById('video');
      var videoSrc = 'http://localhost:8088/playlist.m3u8';
      
      if (Hls.isSupported()) {
        var hls = new Hls();
        hls.loadSource(videoSrc);
        hls.attachMedia(video);
      } else if (video.canPlayType('application/vnd.apple.mpegurl')) {
        video.src = videoSrc;
      }
    </script>
</body>
</html>
```

Para un reproductor de ejemplo completo, consulte nuestro [repositorio GitHub](https://github.com/visioforge/.Net-SDK-s-samples/blob/master/Media%20Blocks%20SDK/Console/HLS%20Streamer/index.htm).

### Integración de Aplicación Móvil

Nuestros SDK también soportan integración con aplicaciones móviles a través de:

- Reproducción nativa iOS usando AVPlayer
- Reproducción Android vía ExoPlayer
- Opciones multiplataforma como .NET MAUI, Avalonia o Uno Platform

## Solución de Problemas Comunes

### Configuración CORS

Cuando sirva contenido HLS a navegadores web, puede encontrar problemas de Cross-Origin Resource Sharing (CORS). Asegúrese de que su servidor esté configurado para enviar los encabezados CORS apropiados:

```
Access-Control-Allow-Origin: *
Access-Control-Allow-Methods: GET, HEAD, OPTIONS
Access-Control-Allow-Headers: Range
Access-Control-Expose-Headers: Accept-Ranges, Content-Encoding, Content-Length, Content-Range
```

### Optimización de Latencia

HLS introduce inherentemente algo de latencia. Para minimizar esto:

- Use duraciones de segmento más cortas (2-4 segundos) para menor latencia
- Considere habilitar Low-Latency HLS (LL-HLS) si está soportado
- Optimice su infraestructura de red para retrasos mínimos

## Conclusión

La transmisión HLS proporciona una solución robusta y multiplataforma para entregar contenido de video tanto en vivo como bajo demanda a una amplia gama de dispositivos. Con los SDK de .NET de VisioForge, implementar HLS en sus aplicaciones se vuelve directo, permitiéndole enfocarse en crear contenido convincente en lugar de luchar con detalles técnicos.

Para más muestras de código e implementaciones avanzadas, visite nuestro [repositorio GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

---
## Recursos Adicionales
- [Especificación HLS](https://developer.apple.com/streaming/)