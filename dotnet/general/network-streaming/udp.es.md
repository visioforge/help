---
title: Transmisión de Video y Audio UDP en .NET
description: Transmita video con el protocolo UDP para transmisiones de baja latencia, vigilancia y transmisión multicast con sobrecarga mínima en aplicaciones .NET.
---

# Transmisión UDP con SDK de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

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

Para un rendimiento óptimo de transmisión, considere ajustar los bitrates de video y audio basados en la capacidad de su red:

```cs
ffmpegOutput.VideoSettings.Bitrate = 2500000; // 2.5 Mbps for video
ffmpegOutput.AudioSettings.Bitrate = 128000;  // 128 kbps for audio
```

### Resolución y Frame Rate

Resoluciones y frame rates más bajos reducen los requisitos de ancho de banda:

```cs
VideoCapture1.Video_Resize_Enabled = true;
VideoCapture1.Video_Resize_Width = 1280;    // 720p resolution
VideoCapture1.Video_Resize_Height = 720;
VideoCapture1.Video_FrameRate = 30;         // 30 fps
```

### Configuración de Tamaño de Buffer

Ajustar los tamaños de buffer puede ayudar a gestionar el equilibrio entre latencia y estabilidad:

```cs
VideoCapture1.Network_Streaming_BufferSize = 8192; // in KB
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

1. **Aceleración por Hardware**: Cuando esté disponible, habilite la aceleración por hardware para codificación:

```cs
ffmpegOutput.VideoSettings.HWAcceleration = HWAcceleration.Auto;
```

2. **Intervalos de Keyframe**: Para menor latencia, reduzca los intervalos de keyframe (I-frame):

```cs
ffmpegOutput.VideoSettings.KeyframeInterval = 60; // One keyframe every 2 seconds at 30 fps
```

3. **Selección de Preset**: Elija presets de codificación basados en su capacidad de CPU y requisitos de latencia:

```cs
ffmpegOutput.VideoSettings.EncoderPreset = H264EncoderPreset.Ultrafast; // Lowest latency, higher bitrate
// or
ffmpegOutput.VideoSettings.EncoderPreset = H264EncoderPreset.Medium; // Balance between quality and CPU load
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