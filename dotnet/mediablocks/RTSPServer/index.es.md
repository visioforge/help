---
title: Bloque Servidor RTSP para .Net
description: Cree servidores de streaming RTSP para entrega de audio y video de baja latencia a través de redes con soporte de codecs H.264/H.265 en Media Blocks SDK.
---

# Bloque Servidor RTSP - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El bloque Servidor RTSP crea un endpoint de servidor RTSP (Protocolo de Transmisión en Tiempo Real) para transmitir contenido de audio/video a través de redes. Los clientes pueden conectarse para recibir streams de medios en vivo o grabados con baja latencia.

## Visión General

El RTSPServerBlock proporciona una implementación completa de servidor de streaming RTSP con las siguientes capacidades:

- **Soporte Multi-Cliente**: Maneja conexiones simultáneas de múltiples clientes RTSP
- **Cumplimiento de Estándares**: Compatible con VLC, FFmpeg, GStreamer y visores de cámaras IP
- **Codecs de Video**: H.264, H.265 y otros formatos comprimidos
- **Codecs de Audio**: AAC, MP3, Opus y otros formatos
- **Protocolos de Transporte**: RTP/RTCP para entrega confiable de medios
- **Autenticación**: Soporte opcional de autenticación RTSP
- **Configurable**: Vinculación de puerto personalizada y puntos de montaje
- **Baja Latencia**: Optimizado para aplicaciones de streaming en tiempo real

## Información del Bloque

Nombre: RTSPServerBlock.

| Dirección del pin | Tipo de medio | Cantidad de pines |
| --- | :---: | :---: |
| Entrada de video | video comprimido (H.264, H.265) | 1 |
| Entrada de audio | audio comprimido (AAC, MP3, etc.) | 1 |

## Pipeline de Ejemplo

```mermaid
graph LR;
    SystemVideoSourceBlock-->H264EncoderBlock;
    SystemAudioSourceBlock-->AACEncoderBlock;
    H264EncoderBlock-->RTSPServerBlock;
    AACEncoderBlock-->RTSPServerBlock;
```

## Configuraciones

El RTSPServerBlock se configura usando `RTSPServerSettings`:

### Propiedades de RTSPServerSettings

- `Port` (`int`): El puerto TCP para el servidor RTSP (predeterminado: 8554)
- `MountPoint` (`string`): La ruta URL para el stream (ej., "/stream1")
- `Username` (`string`): Nombre de usuario opcional para autenticación
- `Password` (`string`): Contraseña opcional para autenticación
- `Multicast` (`bool`): Habilitar streaming multicast
- `MulticastAddress` (`string`): Dirección del grupo multicast
- `TTL` (`int`): Tiempo de vida para paquetes multicast

## Código de Ejemplo

### Servidor RTSP Básico

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

// Crear codificador de video
var h264Settings = new H264EncoderSettings
{
    Bitrate = 2000,
    Profile = H264Profile.Main
};
var h264Encoder = new H264EncoderBlock(h264Settings);
pipeline.Connect(videoSource.Output, h264Encoder.Input);

// Crear codificador de audio
var aacSettings = new AACEncoderSettings
{
    Bitrate = 128
};
var aacEncoder = new AACEncoderBlock(aacSettings);
pipeline.Connect(audioSource.Output, aacEncoder.Input);

// Crear servidor RTSP
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/live"
};
var rtspServer = new RTSPServerBlock(rtspSettings);
pipeline.Connect(h264Encoder.Output, rtspServer.VideoInput);
pipeline.Connect(aacEncoder.Output, rtspServer.AudioInput);

// Iniciar streaming
await pipeline.StartAsync();

// El servidor ahora es accesible en rtsp://localhost:8554/live
Console.WriteLine("Servidor RTSP iniciado en rtsp://localhost:8554/live");
Console.WriteLine("Conectar con: vlc rtsp://localhost:8554/live");
```

### Servidor RTSP con Autenticación

```csharp
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/secure",
    Username = "admin",
    Password = "password123"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Configurar codificadores y conectar como arriba
// ...

await pipeline.StartAsync();

// Los clientes deben autenticarse: rtsp://admin:password123@localhost:8554/secure
Console.WriteLine("Servidor RTSP seguro iniciado en rtsp://localhost:8554/secure");
```

### Streaming de Archivo sobre RTSP

```csharp
var pipeline = new MediaBlocksPipeline();

// Usar archivo como fuente
var fileSettings = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
var fileSource = new UniversalSourceBlock(fileSettings);

// Crear servidor RTSP
var rtspSettings = new RTSPServerSettings
{
    Port = 8554,
    MountPoint = "/vod"
};
var rtspServer = new RTSPServerBlock(rtspSettings);

// Conectar video y audio desde archivo
pipeline.Connect(fileSource.VideoOutput, rtspServer.VideoInput);
pipeline.Connect(fileSource.AudioOutput, rtspServer.AudioInput);

await pipeline.StartAsync();

// Transmitir contenido del archivo vía RTSP
Console.WriteLine("Servidor RTSP Video-on-Demand iniciado en rtsp://localhost:8554/vod");
```

## Conexión de Cliente

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

## Casos de Uso

- **Streaming en Vivo**: Transmitir feeds de cámara en vivo a través de redes
- **Sistemas de Seguridad**: Transmitir cámaras de vigilancia a estaciones de monitoreo
- **Distribución de Video**: Distribuir contenido de medios a múltiples clientes
- **Monitoreo Remoto**: Habilitar visualización remota de procesos industriales
- **Transmisión**: Crear soluciones de streaming de baja latencia
- **Videoconferencia**: Transmitir feeds de participantes a espectadores
- **Cámaras IoT**: Proporcionar endpoint RTSP para dispositivos de cámara inteligente

## Consideraciones de Rendimiento

- **Codificación**: Video/audio debe ser codificado antes de transmitir
- **Ancho de Banda**: Monitorear ancho de banda de red para múltiples clientes
- **Latencia**: Optimizar configuraciones del codificador para requisitos de baja latencia
- **Clientes**: Cada cliente consume recursos del servidor y ancho de banda
- **Puerto**: Asegurar que el firewall permita tráfico en el puerto configurado
- **Multicast**: Usar para streaming eficiente uno-a-muchos en redes locales

## Observaciones

- Los streams de video y audio deben ser comprimidos/codificados antes de alimentar al servidor RTSP
- El servidor soporta H.264/H.265 para video y AAC/MP3/Opus para audio
- El puerto 8554 es el puerto estándar RTSP pero puede cambiarse
- El punto de montaje define la ruta URL para acceder al stream
- El servidor continúa transmitiendo mientras el pipeline esté ejecutándose
- Múltiples instancias de RTSPServerBlock pueden ejecutarse en diferentes puertos simultáneamente
- Use autenticación para entornos de streaming seguros

## Plataformas

Windows, macOS, Linux.

Nota: Requiere GStreamer con soporte de servidor RTSP (plugin gst-rtsp-server).

## Aplicaciones de Ejemplo

- [Servidor RTSP Webcam](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Webcam%20Server)

## Bloques Relacionados

- [H264EncoderBlock](../VideoEncoders/index.md#codificador-h264) - Codificación de video H.264
- [H265EncoderBlock](../VideoEncoders/index.md#codificador-hevch265) - Codificación de video H.265
- [AACEncoderBlock](../AudioEncoders/index.md#codificador-aac) - Codificación de audio AAC
- [SystemVideoSourceBlock](../Sources/index.md#fuente-de-video-del-sistema) - Captura de cámara
- [SystemAudioSourceBlock](../Sources/index.md#fuente-de-audio-del-sistema) - Captura de audio
