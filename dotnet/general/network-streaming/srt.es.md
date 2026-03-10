---
title: Streaming SRT en C# .NET - Enviar y Recibir Video por IP
description: Transmita y reciba video mediante el protocolo SRT en C# .NET con modos caller/listener, cifrado AES y multiplexación MPEG-TS. Incluye ejemplos de código SDK.
---

# Guía de Implementación de Transmisión SRT

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## ¿Qué es SRT?

SRT (Secure Reliable Transport) es un protocolo de transmisión diseñado para entrega de video de baja latencia y alta calidad a través de redes no confiables. Proporciona recuperación de errores incorporada, cifrado AES y traversal de firewall — haciéndolo ideal para:

- Transmisión en vivo por internet
- Feeds de contribución entre instalaciones de producción
- Backhaul de cámara remota sobre enlaces celulares o satelitales
- Transporte de video seguro punto a punto
- Ingesta y distribución de video basada en la nube

Los SDK de VisioForge .NET soportan tanto el envío como la recepción de flujos SRT en Windows, macOS y Linux. Los flujos SRT usan multiplexación MPEG-TS para transportar video y audio juntos.

Puede verificar la disponibilidad de SRT en tiempo de ejecución:

```csharp
bool srtAvailable = SRTSinkBlock.IsAvailable(); // para enviar
bool srtSourceAvailable = SRTSourceBlock.IsAvailable(); // para recibir
```

## Modos de Conexión SRT

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

SRT soporta tres modos de conexión a través del enum `SRTConnectionMode`:

| Modo | Descripción | Caso de Uso |
| --- | --- | --- |
| **Caller** | Se conecta a un listener remoto | Cliente conectándose a un servidor |
| **Listener** | Espera conexiones entrantes en un puerto | Servidor aceptando conexiones |
| **Rendezvous** | Ambos lados se conectan simultáneamente | Peer-to-peer, traversal de firewall |

### Modo Listener (Servidor)

El listener espera conexiones SRT entrantes en un puerto especificado:

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener
};
```

### Modo Caller (Cliente)

El caller se conecta a un listener SRT remoto:

```csharp
var sourceSettings = new SRTSourceSettings
{
    Uri = "srt://192.168.1.100:8888",
    Mode = SRTConnectionMode.Caller
};
```

### Modo Rendezvous

Ambos endpoints se conectan simultáneamente — útil cuando ambos lados están detrás de firewalls:

```csharp
var settings = new SRTSinkSettings
{
    Uri = "srt://remote-host:8888",
    Mode = SRTConnectionMode.Rendezvous,
    LocalPort = 8888
};
```

## Salida SRT Básica

### Video Capture SDK

```csharp
// Inicializar salida SRT con URL de destino
var srtOutput = new SRTOutput("srt://streaming-server:1234");

// Agregar la salida SRT configurada a su motor de captura
videoCapture.Outputs_Add(srtOutput, true);  // videoCapture es una instancia de VideoCaptureCoreX
```

### Media Blocks SDK

El `SRTMPEGTSSinkBlock` multiplexa video y audio en un contenedor MPEG-TS y envía sobre SRT:

```csharp
// Crear un sumidero SRT MPEG-TS en modo listener
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings { Uri = "srt://:8888" });

// Conectar salida del codificador de video al sumidero SRT
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar salida del codificador de audio al sumidero SRT
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Transmisión de Cámara a SRT

[MediaBlocksPipeline](#){ .md-button }

Este ejemplo completo captura desde una webcam y micrófono, codifica a H.264/AAC y transmite por SRT:

### Arquitectura del Pipeline

```text
SystemVideoSourceBlock → H264EncoderBlock → SRTMPEGTSSinkBlock (entrada de video)
SystemAudioSourceBlock → AACEncoderBlock  → SRTMPEGTSSinkBlock (entrada de audio)
```

### Ejemplo de Código

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoEncoders;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

// Inicializar SDK una vez al inicio
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Enumerar dispositivos
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Fuente de video (primera cámara)
var videoSource = new SystemVideoSourceBlock(
    new VideoCaptureDeviceSourceSettings(videoDevices[0]));

// Fuente de audio (primer micrófono)
var audioSource = new SystemAudioSourceBlock(
    new AudioCaptureDeviceSourceSettings(audioDevices[0]));

// Codificador de video — H.264 con fallback de software
var h264Encoder = new H264EncoderBlock(new OpenH264EncoderSettings());

// Codificador de audio — AAC
var aacEncoder = new AACEncoderBlock(new VOAACEncoderSettings());

// Salida SRT en modo listener en puerto 8888
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Latency = TimeSpan.FromMilliseconds(125)
});

// Construir pipeline: cámara → codificador → SRT
pipeline.Connect(videoSource.Output, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

pipeline.Connect(audioSource.Output, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

Los receptores pueden conectarse usando `ffplay srt://tu-ip:8888` o cualquier reproductor compatible con SRT.

## Recepción de un Flujo SRT

[MediaBlocksPipeline](#){ .md-button }

Use `SRTSourceBlock` para recibir y reproducir un flujo SRT con decodificación automática:

```csharp
var pipeline = new MediaBlocksPipeline();

// Conectar a un emisor SRT (modo caller por defecto)
var sourceSettings = await SRTSourceSettings.CreateAsync("srt://192.168.1.100:8888");
var srtSource = new SRTSourceBlock(sourceSettings);

// Renderizador de video
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(srtSource.VideoOutput, videoRenderer.Input);

// Renderizador de audio
var audioRenderer = new AudioRendererBlock();
pipeline.Connect(srtSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

Para grabación passthrough sin decodificación (por ejemplo, guardar el flujo MPEG-TS sin procesar), use `SRTRAWSourceBlock` en su lugar.

## Cifrado

SRT soporta cifrado AES con claves de 128, 192 o 256 bits. Tanto el emisor como el receptor deben usar la misma frase de contraseña y longitud de clave.

### Emisor (Cifrado)

```csharp
var sinkSettings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Mode = SRTConnectionMode.Listener,
    Passphrase = "my-secret-passphrase",  // mínimo 10 caracteres
    PbKeyLen = SRTKeyLength.Length32       // AES de 256 bits
};
```

### Receptor (Cifrado)

```csharp
var sourceSettings = new SRTSourceSettings
{
    Uri = "srt://192.168.1.100:8888",
    Mode = SRTConnectionMode.Caller,
    Passphrase = "my-secret-passphrase",
    PbKeyLen = SRTKeyLength.Length32
};
```

Longitudes de clave disponibles: `SRTKeyLength.NoKey` (deshabilitado), `Length16` (128 bits), `Length24` (192 bits), `Length32` (256 bits).

## Configuración de Latencia

La propiedad `Latency` controla el tamaño del buffer del receptor SRT (por defecto: 125ms). Valores más bajos reducen el retraso pero aumentan la sensibilidad al jitter de red:

```csharp
// Baja latencia para red local
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(50)
};

// Mayor latencia para redes no confiables (streaming por internet)
var settings = new SRTSinkSettings
{
    Uri = "srt://:8888",
    Latency = TimeSpan.FromMilliseconds(500)
};
```

| Red | Latencia Recomendada | Notas |
| --- | --- | --- |
| LAN local | 20–80ms | Jitter mínimo |
| Internet confiable | 125ms (por defecto) | Buen equilibrio |
| No confiable/larga distancia | 250–1000ms | Previene pérdidas |

## Opciones de Codificación de Video

### Codificadores de Software

- **OpenH264** — Codificador H.264 multiplataforma por defecto

### Codificadores Acelerados por Hardware

- **NVIDIA NVENC** (H.264/HEVC) — Codificación acelerada por GPU en tarjetas NVIDIA
- **Intel Quick Sync** (H.264/HEVC) — Aceleración de GPU integrada Intel
- **AMD AMF** (H.264/HEVC) — Aceleración de GPU AMD
- **Microsoft Media Foundation HEVC** — Codificador por hardware solo Windows

### Selección de Codificador con Fallback

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    srtOutput.Video = new NVENCH264EncoderSettings();
}
else
{
    srtOutput.Video = new OpenH264EncoderSettings();
}
```

## Codificación de Audio

Los flujos SRT típicamente usan audio AAC. El SDK proporciona múltiples codificadores:

- **VO-AAC** — Multiplataforma, rendimiento consistente
- **AVENC AAC** — Basado en FFmpeg con opciones extensas
- **MF AAC** — Solo Windows, Microsoft Media Foundation

El SDK auto-selecciona el mejor codificador disponible por plataforma (MF AAC en Windows, VO AAC en otras).

## Solución de Problemas

### No se Puede Establecer Conexión SRT

**Síntoma:** La conexión expira o es rechazada.

**Soluciones:**

- Verifique el formato de URL SRT: `srt://host:puerto` para caller, `srt://:puerto` para listener
- Asegúrese de que el puerto esté abierto en firewalls en ambos lados
- Confirme que ambos lados usen modos de conexión compatibles (un caller, un listener)
- Verifique que las frases de contraseña coincidan si el cifrado está habilitado

### Alto Uso de CPU o Frames Descartados

**Síntoma:** El rendimiento se degrada durante la transmisión.

**Soluciones:**

- Cambie a codificadores acelerados por hardware (NVENC, QSV, AMF)
- Reduzca resolución o bitrate
- Aumente el valor de `Latency` para dar más espacio de buffer

### El Codificador Falla al Inicializar

**Síntoma:** Excepción al iniciar el pipeline.

**Soluciones:**

- Use `IsAvailable()` para verificar soporte del codificador antes de crearlo
- Verifique que los controladores de GPU estén actualizados para codificadores de hardware
- Retroceda a OpenH264 como codificador de software universal

## Preguntas Frecuentes

### ¿Cuál es la diferencia entre los modos caller y listener de SRT?

El **listener** se vincula a un puerto y espera conexiones entrantes — actúa como servidor. El **caller** inicia la conexión a la dirección y puerto de un listener — actúa como cliente. Para traversal de firewall donde ambos lados están detrás de NAT, use el modo **rendezvous** donde ambos endpoints se conectan simultáneamente.

### ¿Cómo cifro un flujo SRT?

Establezca la propiedad `Passphrase` (mínimo 10 caracteres) y `PbKeyLen` tanto en `SRTSinkSettings` como en `SRTSourceSettings`. Tanto el emisor como el receptor deben usar valores idénticos. Las longitudes de clave disponibles son 128 bits (`Length16`), 192 bits (`Length24`) y 256 bits (`Length32`). Consulte la sección [Cifrado](#cifrado) para ejemplos de código.

### ¿Cómo recibo y reproduzco un flujo SRT en C#?

Cree `SRTSourceSettings` con la URL del emisor, luego páselo a `SRTSourceBlock`. Conecte `VideoOutput` a un `VideoRendererBlock` y `AudioOutput` a un `AudioRendererBlock`. El bloque fuente maneja el demuxing MPEG-TS y la decodificación automáticamente. Consulte la sección [Recepción de un Flujo SRT](#recepcion-de-un-flujo-srt) para el ejemplo completo.

### ¿Qué codecs de video soporta SRT?

SRT en sí es agnóstico de codec — transporta cualquier dato por la red. Cuando usa `SRTMPEGTSSinkBlock`, el flujo se multiplexa como MPEG-TS, que soporta codecs de video H.264, HEVC (H.265), MPEG-2 y AV1. H.264 es la opción más ampliamente compatible para transmisión SRT.

### ¿Cómo reduzco la latencia de transmisión SRT?

Baje la propiedad `Latency` tanto en la configuración del emisor como del receptor (el valor por defecto es 125ms). Para redes locales, valores tan bajos como 20–50ms funcionan bien. Para streaming por internet, mantenga al menos 125ms para manejar el jitter. También asegúrese de que su codificador esté configurado para modo de baja latencia y que esté usando aceleración por hardware para minimizar el retraso de codificación.

## Ver También

- [Integración de Transmisión NDI](ndi.md) — transmisión de video sobre IP con NDI
- [Visor de Streams RTSP y Reproductor de Cámaras IP](../../mediablocks/Guides/rtsp-player-csharp.md) — guía de streaming de cámaras RTSP
- [Formato de Salida MPEG-TS](../output-formats/mpegts.md) — configuración del contenedor MPEG-TS
- [Guía de Despliegue](../../deployment-x/index.md) — paquetes de runtime específicos de plataforma
- [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — demo de fuente SRT
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — página del producto y descargas
