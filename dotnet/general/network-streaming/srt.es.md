---
title: Transmisión de Protocolo SRT en .NET
description: Integre el protocolo SRT para transmisión de video segura y de baja latencia con recuperación de errores y cifrado en aplicaciones .NET para entrega confiable.
---

# Guía de Implementación de Transmisión SRT para SDK de VisioForge .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## ¿Qué es SRT y Por Qué Debería Usarlo?

SRT (Secure Reliable Transport) es un protocolo de transmisión de alto rendimiento diseñado para entregar video de alta calidad y baja latencia a través de redes impredecibles. A diferencia de los protocolos de transmisión tradicionales, SRT sobresale en condiciones de red desafiantes al incorporar mecanismos únicos de recuperación de errores y características de cifrado.

Los SDK de VisioForge .NET proporcionan soporte completo para transmisión SRT a través de una API de configuración intuitiva, permitiendo a los desarrolladores implementar entrega de video segura y confiable en sus aplicaciones con esfuerzo mínimo.

## Comenzando con SRT en VisioForge

### Plataformas SDK Soportadas

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Configuración Básica SRT

Implementar transmisión SRT en su aplicación comienza con especificar la URL de destino de transmisión. La URL SRT sigue un formato estándar que incluye información de protocolo, host y puerto.

#### Implementación de Video Capture SDK

```csharp
// Inicializar salida SRT con URL de destino
var srtOutput = new SRTOutput("srt://streaming-server:1234");

// Agregar la salida SRT configurada a su motor de captura
videoCapture.Outputs_Add(srtOutput, true);  // videoCapture es una instancia de VideoCaptureCoreX
```

#### Implementación de Media Blocks SDK

```csharp
// Crear un bloque de sumidero SRT con configuraciones apropiadas
var srtSink = new SRTMPEGTSSinkBlock(new SRTSinkSettings() { Uri = "srt://:8888" });

// Configurar codificadores para compatibilidad SRT
h264Encoder.Settings.ParseStream = false; // Deshabilitar análisis para codificador H264

// Conectar su codificador de video al sumidero SRT
pipeline.Connect(h264Encoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar su codificador de audio al sumidero SRT
pipeline.Connect(aacEncoder.Output, srtSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Opciones de Codificación de Video para Transmisión SRT

Los SDK de VisioForge ofrecen opciones de codificación flexibles para equilibrar calidad, rendimiento y utilización de hardware. Puede elegir entre codificadores basados en software o opciones aceleradas por hardware según sus requisitos específicos.

### Codificadores de Video Basados en Software

- **OpenH264**: El codificador multiplataforma predeterminado que proporciona excelente compatibilidad en diferentes entornos

### Codificadores de Video Acelerados por Hardware

- **NVIDIA NVENC (H.264/HEVC)**: Aprovecha la aceleración GPU de NVIDIA para codificación de alto rendimiento
- **Intel Quick Sync Video (H.264/HEVC)**: Utiliza el hardware dedicado de procesamiento de medios de Intel
- **AMD AMF (H.264/H.265)**: Habilita aceleración por hardware en procesadores gráficos AMD
- **Microsoft Media Foundation HEVC**: Codificador acelerado por hardware específico de Windows

#### Ejemplo: Configurando Aceleración por Hardware NVIDIA

```csharp
// Configurar salida SRT para usar aceleración por hardware NVIDIA
srtOutput.Video = new NVENCH264EncoderSettings();
```

## Codificación de Audio para Flujos SRT

La calidad de audio es crítica para muchas aplicaciones de transmisión. Los SDK de VisioForge proporcionan múltiples opciones de codificación de audio:

- **VO-AAC**: Codificador AAC multiplataforma con rendimiento consistente
- **AVENC AAC**: Codificador AAC basado en FFmpeg con opciones de configuración extensas
- **MF AAC**: Codificador AAC de Microsoft Media Foundation (solo Windows)

El SDK selecciona automáticamente el codificador de audio predeterminado más apropiado basado en la plataforma:
- Los sistemas Windows predeterminan a MF AAC
- Otras plataformas predeterminan a VO AAC

## Optimizaciones Específicas de Plataforma

### Características Específicas de Windows

Cuando se ejecuta en sistemas Windows, el SDK puede aprovechar los marcos de Microsoft Media Foundation:

- El codificador MF AAC proporciona codificación de audio eficiente
- El codificador MF HEVC entrega compresión de video de alta calidad y eficiente

### Optimizaciones macOS

En plataformas macOS, el SDK selecciona automáticamente:

- Codificador Apple Media H264 para codificación de video optimizada
- Codificador VO AAC para codificación de audio confiable

## Opciones de Configuración SRT Avanzadas

### Pipeline de Procesamiento de Medios Personalizado

Para aplicaciones con requisitos especializados, el SDK soporta procesamiento personalizado para flujos de video y audio:

```csharp
// Agregar procesamiento de video personalizado antes de codificar
srtOutput.CustomVideoProcessor = new SomeMediaBlock();

// Agregar procesamiento de audio personalizado antes de codificar
srtOutput.CustomAudioProcessor = new SomeMediaBlock();
```

Estos procesadores le permiten implementar filtros, transformaciones o análisis antes de codificar y transmitir.

### Configuración de Sumidero SRT

Ajuste finamente su conexión SRT usando la clase SRTSinkSettings:

```csharp
// Actualizar la URI de destino SRT
srtOutput.Sink.Uri = "srt://new-server:5678";
```

## Mejores Prácticas para Transmisión SRT

### Optimizando Selección de Codificador

1. **Prioridad de Aceleración por Hardware**: Siempre elija codificadores acelerados por hardware cuando estén disponibles. Los beneficios de rendimiento son significativos, particularmente para transmisión de alta resolución.

2. **Mecanismos de Fallback Inteligentes**: Implemente verificaciones de disponibilidad de codificador para retroceder automáticamente a codificación por software si la aceleración por hardware no está disponible:

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

### Optimización de Rendimiento

1. **Configuración de Bitrate**: Ajuste cuidadosamente los bitrates del codificador basados en su tipo de contenido y condiciones de red objetivo. Bitrates más altos aumentan la calidad pero requieren más ancho de banda.

2. **Monitoreo de Recursos**: Monitoree el uso de CPU y GPU durante la transmisión para identificar cuellos de botella potenciales. Si el uso de CPU es consistentemente alto, considere cambiar a aceleración por hardware.

3. **Gestión de Latencia**: Configure tamaños de buffer apropiados basados en sus requisitos de latencia. Buffers más pequeños reducen la latencia pero pueden aumentar la susceptibilidad a fluctuaciones de red.

## Solución de Problemas de Implementaciones SRT

### Problemas Comunes y Soluciones

#### Fallos de Inicialización de Codificador

- **Problema**: El codificador seleccionado falla al inicializar o lanza excepciones
- **Solución**: Verifique que el codificador sea soportado en su plataforma y que los controladores requeridos estén instalados y actualizados

#### Problemas de Conexión de Transmisión

- **Problema**: Incapaz de establecer conexión SRT
- **Solución**: Confirme que el formato de URL SRT sea correcto y que los puertos especificados estén abiertos en todos los firewalls y equipos de red

#### Cuellos de Botella de Rendimiento

- **Problema**: Alto uso de CPU o frames descartados durante la transmisión
- **Solución**: Considere cambiar a codificadores acelerados por hardware o reducir resolución/bitrate

## Ejemplos de Integración

### Configuración Completa de Transmisión SRT

```csharp
// Crear y configurar salida SRT
var srtOutput = new SRTOutput("srt://streaming-server:1234");

// Configurar codificación de video - intentar aceleración por hardware con fallback
if (NVENCH264EncoderSettings.IsAvailable())
{
    var nvencSettings = new NVENCH264EncoderSettings();
    nvencSettings.Bitrate = 4000000; // 4 Mbps
    srtOutput.Video = nvencSettings;
}
else
{
    var softwareSettings = new OpenH264EncoderSettings();
    softwareSettings.Bitrate = 2000000; // 2 Mbps para codificación por software
    srtOutput.Video = softwareSettings;
}

// Agregar al motor de captura
videoCapture.Outputs_Add(srtOutput, true);

// Iniciar transmisión
videoCapture.Start();
```

## Conclusión

La transmisión SRT en SDK de VisioForge .NET proporciona una solución poderosa para entrega de video de alta calidad y baja latencia a través de condiciones de red desafiantes. Al aprovechar las opciones de codificador flexibles y capacidades de configuración, los desarrolladores pueden implementar soluciones de transmisión robustas para una amplia gama de aplicaciones.

Ya sea que esté construyendo una plataforma de transmisión en vivo, una solución de videoconferencia o un sistema de entrega de contenido, la combinación de SRT de seguridad, confiabilidad y rendimiento lo hace una excelente opción para aplicaciones de video modernas.

Para más información sobre codificadores específicos u opciones de configuración avanzadas, consulte la documentación completa del SDK de VisioForge.