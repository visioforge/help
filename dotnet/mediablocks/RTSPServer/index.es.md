---
title: Servidor RTSP - streaming H.264 de baja latencia en C# .NET
description: Cree servidores de streaming RTSP para entrega de audio y video de baja latencia con soporte H.264/H.265 usando VisioForge Media Blocks SDK.
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - RTSPServerBlock
  - RTSPServerSettings
  - H264EncoderBlock
  - AACEncoderBlock
  - SystemVideoSourceBlock
  - UniversalSourceBlock
video:
  id: YP5rS11zoNc
  upload_date: 2026-11-22T09:36:39Z
  duration: PT46S

sidebar_label: RTSP Server
---

# Bloque Servidor RTSP - VisioForge Media Blocks SDK .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Soporte multiplataforma"
    El `RTSPServerBlock` funciona en **Windows, macOS y Linux** vía GStreamer (requiere el plugin `gst-rtsp-server`). Consulte la [matriz de soporte de plataformas](../../platform-matrix.md) para códecs y detalles de aceleración por hardware, y la [guía de despliegue en Linux](../../deployment-x/Ubuntu.md) para configuración en Ubuntu / NVIDIA Jetson / Raspberry Pi.

El bloque Servidor RTSP crea un endpoint de servidor RTSP (Protocolo de Transmisión en Tiempo Real) para transmitir contenido de audio/video a través de redes. Los clientes pueden conectarse para recibir streams de medios en vivo o grabados con baja latencia.

## Video tutorial: servir un archivo MP4 como servidor RTSP con unas pocas líneas de C\#

Una aplicación de consola que sirve un archivo de vídeo como stream RTSP: `UniversalSourceBlock` lee el archivo y `RTSPServerBlock` lo codifica en H.264, y en AAC si el archivo tiene audio, y lo sirve en `rtsp://127.0.0.1:8554/live`.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YP5rS11zoNc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Su código: [rtsp-server-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_VideoTutorials/rtsp-server-short-console) en GitHub, y [Streaming de Archivo sobre RTSP](#streaming-de-archivo-sobre-rtsp) más abajo.

## Visión general

El RTSPServerBlock proporciona una implementación completa de servidor de streaming RTSP con las siguientes capacidades:

- **Soporte Multi-Cliente**: Maneja conexiones simultáneas de múltiples clientes RTSP, que comparten un único stream codificado
- **Cumplimiento de Estándares**: Compatible con VLC, FFmpeg, GStreamer y visores de cámaras IP
- **Codificación Integrada**: El bloque recibe video y audio sin comprimir y los codifica él mismo
- **Codecs de Video**: H.264, H.265 (HEVC) y VP9
- **Codecs de Audio**: AAC, MP3, Opus y Vorbis
- **Protocolos de Transporte**: RTP/RTCP para entrega confiable de medios
- **Autenticación**: Soporte opcional de autenticación RTSP
- **Configurable**: Puerto, punto de montaje y dirección de escucha personalizados
- **Baja Latencia**: Optimizado para aplicaciones de streaming en tiempo real

## Información del bloque

Nombre: RTSPServerBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | video sin comprimir, codificado con `VideoEncoder` | 1 |
| Entrada de audio | audio sin comprimir, codificado con `AudioEncoder` | 1 |

## Pipeline de ejemplo

```mermaid
graph LR;
    SystemVideoSourceBlock-->RTSPServerBlock;
    SystemAudioSourceBlock-->RTSPServerBlock;
```

## Configuraciones

El RTSPServerBlock se configura usando `RTSPServerSettings`:

### Propiedades de RTSPServerSettings

- `Port` (`int`): El puerto TCP del servidor RTSP. Predeterminado `8554`.
- `Point` (`string`): La ruta URL bajo la que se sirve el stream (por ejemplo `/live`). Predeterminado `/live`.
- `Username` (`string`): Nombre de usuario opcional para autenticación RTSP básica.
- `Password` (`string`): Contraseña opcional para autenticación RTSP básica.
- `Address` (`string`): El nombre de host o la dirección IP con que los clientes llegan al servidor. Solo define el host de `URL`; no elige las interfaces en las que escucha el servidor. Predeterminado `127.0.0.1`.
- `ListenAddress` (`string`): La dirección IP local en la que escucha el servidor. Predeterminado `0.0.0.0`, todas las interfaces IPv4; `127.0.0.1` acepta solo clientes del mismo equipo. Si indica aquí una dirección concreta, establezca también `Address` para que `URL` la muestre.
- `Name` (`string`): Nombre del servidor anunciado a los clientes. Predeterminado `"VisioForge RTSP Server"`.
- `Description` (`string`): Descripción legible. Predeterminado `"VisioForge RTSP Server"`.
- `Latency` (`TimeSpan`): Latencia de buffering aplicada por el servidor. Predeterminado `250 ms`.
- `URL` (`string`, solo lectura): URL computada `rtsp://{Address}:{Port}{Point}` — úsela tras construir los settings para registrar la URL visible al cliente.
- `VideoEncoder` (`IVideoEncoder`): Configuración del codificador de video: H.264, H.265 o VP9. Pase `null` para deshabilitar el stream de video.
- `AudioEncoder` (`IAudioEncoder`): Configuración del codificador de audio: AAC, MP3, Opus o Vorbis. Pase `null` para deshabilitar el stream de audio.

`RTSPServerSettings` **no tiene constructor sin parámetros**. Use uno de:

- `new RTSPServerSettings(IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)`
- `new RTSPServerSettings(Uri uri, IVideoEncoder videoEncoder, IAudioEncoder audioEncoder)` — parsea host / puerto / point desde el URI; no establece `ListenAddress`, así que para un host IPv6 establezca también `ListenAddress` en una dirección IPv6.

## Código de ejemplo

### Servidor RTSP Básico

Conecte la cámara y el micrófono directamente al servidor: este los codifica con los codificadores indicados en su configuración.

```csharp
var pipeline = new MediaBlocksPipeline();

// Crear fuente de video
var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
var videoFormat = videoDevice.VideoFormats[0];
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
{
    Format = videoFormat.ToFormat()
};
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Crear fuente de audio
var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
var audioFormat = audioDevice.Formats[0];
var audioSettings = audioDevice.CreateSourceSettings(audioFormat.ToFormat());
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Crear servidor RTSP: codifica el video en H.264 y el audio en AAC
var rtspSettings = new RTSPServerSettings(
    H264EncoderBlock.GetDefaultSettings(),
    AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/live"
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(videoSource.Output, rtspServer.VideoInput);
pipeline.Connect(audioSource.Output, rtspServer.AudioInput);

// Iniciar streaming
await pipeline.StartAsync();

// rtspSettings.URL devuelve la URL visible al cliente:
Console.WriteLine($"Servidor RTSP iniciado en {rtspSettings.URL}");
Console.WriteLine($"Conectar con: vlc {rtspSettings.URL}");
```

Por defecto el servidor escucha en todas las interfaces de red, así que otros equipos lo alcanzan en la dirección IP de este equipo. Para limitarlo a este equipo, establezca `ListenAddress = "127.0.0.1"`.

### Servidor RTSP con Autenticación

```csharp
var rtspSettings = new RTSPServerSettings(
    videoEncoder: H264EncoderBlock.GetDefaultSettings(),
    audioEncoder: AACEncoderBlock.GetDefaultSettings())
{
    Port = 8554,
    Point = "/secure",
    Username = "admin",
    Password = "password123"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Conectar las fuentes como arriba
// ...

await pipeline.StartAsync();

// Los clientes deben autenticarse: rtsp://admin:password123@localhost:8554/secure
Console.WriteLine($"Servidor RTSP seguro iniciado en {rtspSettings.URL}");
```

### Streaming de Archivo sobre RTSP

Una aplicación de consola completa que sirve un archivo MP4 (o cualquier otro archivo de vídeo que reproduzca `UniversalSourceBlock`) en `rtsp://127.0.0.1:8554/live` hasta que se pulsa Enter. Pase la ruta del archivo como primer argumento. El codificador AAC se configura solo si el archivo tiene pista de audio, de modo que un archivo solo de video da un stream solo de video.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

await VisioForgeX.InitSDKAsync();
await using var p = new MediaBlocksPipeline();
p.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var file = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(
        args[0]));
var aac = file.AudioOutput == null ? null
    : AACEncoderBlock.GetDefaultSettings();
var rtsp = new RTSPServerBlock(
    new RTSPServerSettings(
        H264EncoderBlock.GetDefaultSettings(),
        aac));
p.Connect(file.VideoOutput, rtsp.VideoInput);
if (aac != null)
    p.Connect(file.AudioOutput, rtsp.AudioInput);
if (!await p.StartAsync()) return;
Console.WriteLine(rtsp.Settings.URL);
Console.ReadLine();
await p.StopAsync();
```

El archivo se reproduce una vez, en tiempo real, desde que se inicia el pipeline: un cliente que se conecta más tarde lo recibe a partir de ese punto. Cuando el archivo termina, los clientes conectados reciben fotogramas negros hasta que la aplicación se detiene.

## Conexión de cliente

Los clientes pueden conectarse al servidor RTSP usando el formato de URL:

```
rtsp://hostname:puerto/puntomontaje
```

Ejemplos:
- `rtsp://localhost:8554/live`
- `rtsp://192.168.1.100:8554/stream1`
- `rtsp://usuario:contraseña@servidor.com:8554/secure`

### VLC Player

```bash
vlc rtsp://localhost:8554/live
```

### FFmpeg

```bash
ffplay rtsp://localhost:8554/live
ffmpeg -i rtsp://localhost:8554/live -c copy output.mp4
```

### GStreamer

```bash
gst-launch-1.0 rtspsrc location=rtsp://localhost:8554/live ! decodebin ! autovideosink
```

## Casos de uso

- **Streaming en Vivo**: Transmitir feeds de cámara en vivo a través de redes
- **Sistemas de Seguridad**: Transmitir cámaras de vigilancia a estaciones de monitoreo
- **Distribución de Video**: Distribuir contenido de medios a múltiples clientes
- **Monitoreo Remoto**: Habilitar visualización remota de procesos industriales
- **Transmisión**: Crear soluciones de streaming de baja latencia
- **Videoconferencia**: Transmitir feeds de participantes a espectadores
- **Cámaras IoT**: Proporcionar endpoint RTSP para dispositivos de cámara inteligente

## Consideraciones de rendimiento

- **Codificación**: El servidor codifica una sola vez para todos los clientes; un codificador por hardware (NVENC, QSV, AMF) libera la CPU
- **Ancho de Banda**: Monitorear ancho de banda de red para múltiples clientes
- **Latencia**: Optimizar configuraciones del codificador para requisitos de baja latencia
- **Clientes**: Cada cliente consume recursos del servidor y ancho de banda
- **Puerto**: Asegurar que el firewall permita tráfico en el puerto configurado

## Observaciones

- Conecte video y audio sin comprimir; el servidor los codifica con `VideoEncoder` y `AudioEncoder`
- El servidor soporta H.264, H.265 y VP9 para video y AAC, MP3, Opus y Vorbis para audio
- El puerto 8554 es la alternativa habitual al puerto RTSP estándar 554, que en Linux requiere root; sirve cualquier puerto libre
- El punto de montaje define la ruta URL para acceder al stream
- El servidor continúa transmitiendo mientras el pipeline esté ejecutándose
- Múltiples instancias de RTSPServerBlock pueden ejecutarse en diferentes puertos simultáneamente
- Use autenticación para entornos de streaming seguros

## Plataformas

Windows, macOS, Linux.

Nota: Requiere GStreamer con soporte de servidor RTSP (plugin gst-rtsp-server).

## Aplicaciones de ejemplo

- [Servidor RTSP Webcam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Webcam%20Server)

## Bloques Relacionados

- [H264EncoderBlock](../VideoEncoders/index.md#codificador-h264) - Codificación de video H.264
- [H265EncoderBlock](../VideoEncoders/index.md#codificador-hevch265) - Codificación de video H.265
- [AACEncoderBlock](../AudioEncoders/index.md#codificador-aac) - Codificación de audio AAC
- [SystemVideoSourceBlock](../Sources/index.md#fuente-de-video-del-sistema) - Captura de cámara
- [SystemAudioSourceBlock](../Sources/index.md#fuente-de-audio-del-sistema) - Captura de audio

## Guías relacionadas

- [Inmersión profunda en el protocolo RTSP](../../general/network-streaming/rtsp.md) — cómo funciona RTSP por dentro
- [Configuración de fuente de cámara RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — consumir streams RTSP en sus aplicaciones
- [Reproductor RTSP de Media Blocks](../Guides/rtsp-player-csharp.md) — pipeline cliente para streams RTSP (combinar con este servidor)
- [Guardar stream RTSP sin re-codificación](../Guides/rtsp-save-original-stream.md) — archivar streams desde una fuente RTSP a disco
- [Reconexión RTSP y fallback switch](../../general/network-sources/reconnection-and-fallback.md) — maneje caídas de fuente upstream con eventos de reconexión y `FallbackSwitch` cuando alimente este servidor desde una cámara RTSP

## Transcripción del video

Sirva un archivo de vídeo como stream RTSP, en unas pocas líneas de C#. Cargue el SDK y cree un pipeline. La fuente es el archivo de vídeo. `RTSPServerBlock` lo codifica en H.264, y en AAC si el archivo tiene sonido, y lo sirve en el puerto 8554. Ejecútelo con un archivo. Muestra la URL, y cualquier reproductor RTSP puede abrirla. La película, recibida por RTSP. Esta página añade una cámara y una contraseña: consulte [Servidor RTSP Básico](#servidor-rtsp-basico) y [Servidor RTSP con Autenticación](#servidor-rtsp-con-autenticacion).

El archivo de vídeo es Big Buck Bunny, (c) copyright 2008, Blender Foundation, www.bigbuckbunny.org, CC BY 3.0.
