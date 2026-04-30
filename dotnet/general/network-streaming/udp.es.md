---
title: Streaming UDP con Contenedor MPEG-TS y H.264 en C# .NET
description: Transmita video H.264/HEVC por UDP en C# / .NET: multicast, punto a punto, baja latencia. Ejemplos de envío y recepción con ajuste de bitrate.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - UDP
  - MP4
  - TS
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - FFMPEGEXEOutput
  - BasicVideoSettings
  - MediaBlockPadMediaType
  - MediaBlocksPipeline
  - UDPMPEGTSSinkBlock
  - UDPSinkSettings
  - MultiUDPMPEGTSSinkBlock

---

# Transmisión UDP con SDK de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! tip "Agentes de IA: usa el servidor MCP de VisioForge"

    ¿Lo construyes con **Claude Code**, **Cursor** u otro agente de IA?
    Conecta al servidor MCP público de VisioForge
    ([documentación](../mcp-server-usage.md))
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introducción a la Transmisión UDP

El Protocolo de Datagramas de Usuario (UDP) es un protocolo de transporte ligero y sin conexión que proporciona una interfaz simple entre aplicaciones de red y la red IP subyacente. A diferencia de TCP, UDP ofrece una sobrecarga mínima y no garantiza la entrega de paquetes, lo que lo hace ideal para aplicaciones en tiempo real donde la velocidad es crucial y la pérdida ocasional de paquetes es aceptable.

Los SDK de VisioForge ofrecen un soporte robusto para la transmisión UDP, permitiendo a los desarrolladores implementar soluciones de transmisión de alto rendimiento y baja latencia para diversas aplicaciones, incluyendo transmisiones en vivo, vigilancia de video y sistemas de comunicación en tiempo real.

## Características Clave y Capacidades

La suite de SDK de VisioForge proporciona funcionalidad completa de transmisión UDP con las siguientes características clave:

### Soporte de Códecs de Video y Audio

- **Códecs de Video**: Soporte completo para H.264 (AVC) y H.265 (HEVC), ofreciendo excelente eficiencia de compresión mientras se mantiene una alta calidad de video.
- **Códec de Audio**: Soporte de Codificación de Audio Avanzada (AAC), proporcionando calidad de audio superior a bitrates más bajos en comparación con códecs de audio más antiguos.

### Flujo de Transporte MPEG (MPEG-TS)

El SDK utiliza MPEG-TS como formato de contenedor para la transmisión UDP. MPEG-TS ofrece varias ventajas:

- Diseñado específicamente para transmisión sobre redes potencialmente no confiables
- Capacidades integradas de corrección de errores
- Soporte para multiplexar múltiples flujos de audio y video
- Características de baja latencia ideales para transmisión en vivo

### Integración FFMPEG

Los SDK de VisioForge aprovechan el poder de FFMPEG para la transmisión UDP, asegurando:

- Codificación y transmisión de alto rendimiento
- Amplia compatibilidad con diversas redes y clientes receptores
- Manejo confiable de paquetes y gestión de flujos

### Soporte Unicast y Multicast

- **Unicast**: Transmisión punto a punto desde un solo remitente a un solo receptor
- **Multicast**: Distribución eficiente del mismo contenido a múltiples destinatarios simultáneamente sin duplicar el ancho de banda en la fuente

## Detalles de Implementación Técnica

La transmisión UDP en los SDK de VisioForge involucra varios componentes técnicos clave:

1. **Codificación de Video**: El video fuente se comprime usando codificadores H.264 o HEVC con parámetros configurables para bitrate, resolución y frame rate.

2. **Codificación de Audio**: Los flujos de audio se procesan a través de codificadores AAC con ajustes de calidad configurables.

3. **Multiplexación**: Los flujos de video y audio se combinan en un solo contenedor MPEG-TS.

4. **Paquetización**: El flujo MPEG-TS se divide en paquetes UDP de tamaño apropiado para la transmisión de red.

5. **Transmisión**: Los paquetes se envían sobre la red a direcciones unicast o multicast especificadas.

La implementación prioriza la baja latencia mientras mantiene suficiente calidad para aplicaciones profesionales. Mecanismos avanzados de buffering ayudan a gestionar el jitter de red y asegurar una reproducción suave en el extremo receptor.

## Implementación de Salida UDP Solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

### Paso 1: Habilitar Transmisión de Red

El primer paso es habilitar la funcionalidad de transmisión de red en su aplicación. Esto se hace configurando la propiedad `Network_Streaming_Enabled` a true:

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

### Paso 2: Configurar Transmisión de Audio (Opcional)

Si su aplicación requiere transmisión de audio junto con video, habilítelo con:

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

### Paso 3: Establecer el Formato de Transmisión

Especifique UDP como el formato de transmisión configurando la propiedad `Network_Streaming_Format` a `UDP_FFMPEG_EXE`:

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.UDP_FFMPEG_EXE;
```

### Paso 4: Configurar la URL del Flujo UDP

Establezca la URL de destino para su flujo UDP. Para un flujo unicast básico a localhost:

```cs
VideoCapture1.Network_Streaming_URL = "udp://127.0.0.1:10000?pkt_size=1316";
```

El parámetro `pkt_size` define el tamaño del paquete UDP. El valor 1316 está optimizado para la mayoría de entornos de red, permitiendo transmisión eficiente mientras se minimiza la fragmentación.

### Paso 5: Configuración Multicast (Opcional)

Para transmisión multicast a múltiples receptores, use una dirección multicast (típicamente en el rango 224.0.0.0 a 239.255.255.255):

```cs
VideoCapture1.Network_Streaming_URL = "udp://239.101.101.1:1234?ttl=1&pkt_size=1316";
```

Los parámetros adicionales incluyen:

- **ttl**: Valor de tiempo de vida que determina cuántos saltos de red pueden atravesar los paquetes
- **pkt_size**: Tamaño de paquete como se explicó arriba

### Paso 6: Configurar Configuraciones de Salida

Finalmente, configure los parámetros de salida de transmisión usando la clase `FFMPEGEXEOutput`:

```cs
var ffmpegOutput = new FFMPEGEXEOutput();

ffmpegOutput.FillDefaults(DefaultsProfile.MP4_H264_AAC, true);
ffmpegOutput.OutputMuxer = OutputMuxer.MPEGTS;

VideoCapture1.Network_Streaming_Output = ffmpegOutput;
```

Este código:

1. Crea una nueva configuración de salida FFMPEG
2. Aplica configuraciones predeterminadas para video H.264 y audio AAC
3. Especifica MPEG-TS como formato de contenedor
4. Asigna esta configuración a la salida de transmisión

## Opciones de Configuración Avanzada

### Gestión de Bitrate

Para un rendimiento óptimo de transmisión, ajuste los bitrates de video y audio
según la capacidad de su red. `FFMPEGEXEOutput` expone las perillas del
codificador vía `.Video` y `.Audio` (no `VideoSettings`/`AudioSettings`), y los
`BasicVideoSettings` / `BasicAudioSettings` subyacentes almacenan el bitrate en
**kbps**:

```cs
ffmpegOutput.Video.Bitrate = 2500; // 2.5 Mbps para video (kbps)
ffmpegOutput.Audio.Bitrate = 128;  // 128 kbps para audio
```

### Resolución y Frame Rate

Las resoluciones más bajas reducen el ancho de banda. Configure el tamaño
objetivo dentro de `VideoCapture1.Video_Resize` (el engine clásico lo expone
como un objeto `IVideoResizeSettings`, no como propiedades planas en el core)
y active la etapa de resize con `Video_ResizeOrCrop_Enabled`:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width  = 1280,   // resolución 720p
    Height = 720,
    Mode   = VideoResizeMode.Letterbox,
};

// El frame rate se configura en el formato del dispositivo de captura, no en
// el core — elija un formato de 30 fps vía Video_CaptureDevice_Format / _FrameRate.
```

### Configuración de Tamaño de Buffer

El balance latencia/estabilidad para streaming basado en FFMPEG se controla en
el objeto de salida, no en el core. Valores en milisegundos:

```cs
ffmpegOutput.VideoBufferSize = 5000; // buffer de 5 s para transmisión más estable
```

## Mejores Prácticas para Transmisión UDP

### Consideraciones de Red

1. **Evaluación de Ancho de Banda**: Asegúrese de tener suficiente ancho de banda para su calidad objetivo. Como guía:
   - Calidad SD (480p): 1-2 Mbps
   - Calidad HD (720p): 2.5-4 Mbps
   - Full HD (1080p): 4-8 Mbps

2. **Estabilidad de Red**: UDP no garantiza la entrega de paquetes. En redes inestables, considere:
   - Reducir resolución o bitrate
   - Implementar recuperación de errores a nivel de aplicación
   - Usar corrección de errores hacia adelante cuando esté disponible

3. **Configuración de Firewall**: Asegúrese de que los puertos UDP estén abiertos en los firewalls de remitente y receptor.

### Optimización de Rendimiento

1. **Aceleración por hardware / Keyframes / Preset**: `FFMPEGEXEOutput` no expone
   propiedades de primera clase para HW accel, intervalo de keyframes o presets
   de x264 — en su lugar, inyéctelos como flags CLI de FFMPEG vía
   `Custom_AdditionalVideoArgs`. FFMPEG los aplica a la invocación del codificador.

```cs
// Codificador NVENC + intervalo de keyframes de 2 s (60 frames @ 30 fps)
// + preset ultrafast (menor latencia).
ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_nvenc -g 60 -preset p1";

// Intel QuickSync en su lugar:
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v h264_qsv -g 60";

// x264 por software con trade-off calidad/velocidad:
// ffmpegOutput.Custom_AdditionalVideoArgs = "-c:v libx264 -g 60 -preset ultrafast";
```

2. **Transporte por pipe** (evita un archivo temporal entre el SDK y FFMPEG)
   generalmente reduce la latencia:

```cs
ffmpegOutput.UsePipe = true;
```

## Solución de Problemas Comunes

1. **Flujo No Recibido**: Verifique conectividad de red, disponibilidad de puerto y configuraciones de firewall.

2. **Alta Latencia**: Verifique congestión de red, reduzca bitrate o ajuste tamaños de buffer.

3. **Mala Calidad**: Aumente bitrate, ajuste configuraciones de codificación o verifique pérdida de paquetes de red.

4. **Problemas de Sincronización Audio/Video**: Asegúrese de sincronización adecuada de timestamps en su aplicación.

## Conclusión

La transmisión UDP con SDK de VisioForge proporciona una solución poderosa para transmisión de video y audio en tiempo real con latencia mínima. Al aprovechar códecs de video H.264/HEVC, audio AAC y empaquetado MPEG-TS, los desarrolladores pueden crear aplicaciones de transmisión robustas adecuadas para una amplia gama de casos de uso.

La flexibilidad del SDK permite el ajuste fino de todos los parámetros de transmisión, permitiendo optimización para condiciones de red específicas y requisitos de calidad. Ya sea implementando un flujo punto a punto simple o un sistema de distribución multicast complejo, las capacidades de transmisión UDP de VisioForge proporcionan las herramientas necesarias para el éxito.

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más muestras de código y demostraciones funcionales de implementaciones de transmisión UDP.