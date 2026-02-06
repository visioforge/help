---
title: Streaming en Vivo de YouTube para Apps .NET
description: Transmita a YouTube Live con RTMP en aplicaciones .NET usando codificadores de video optimizados, configuración de audio y soporte multiplataforma.
---

# Streaming en Vivo de YouTube con los SDK de VisioForge

## Introducción a la Integración de Streaming YouTube

La funcionalidad de salida RTMP de YouTube en los SDK de VisioForge permite a los desarrolladores crear aplicaciones .NET robustas que transmiten contenido de video de alta calidad directamente a YouTube. Esta implementación aprovecha varios codificadores de video y audio para optimizar el rendimiento del streaming en diferentes configuraciones de hardware y plataformas.

## Plataformas SDK soportadas

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Todas las principales plataformas SDK de VisioForge proporcionan capacidades multiplataforma para streaming de YouTube, asegurando funcionalidad consistente en Windows, macOS y otros sistemas operativos.

## Entendiendo la clase YouTubeOutput

La clase `YouTubeOutput` sirve como la interfaz principal para la configuración de streaming de YouTube, ofreciendo extensas opciones de personalización incluyendo:

- **Selección y configuración de codificador de video**: Elija entre múltiples codificadores acelerados por hardware y basados en software
- **Selección y configuración de codificador de audio**: Configure codificadores de audio AAC con parámetros personalizados
- **Procesamiento personalizado de video y audio**: Aplique filtros y transformaciones antes del streaming
- **Configuración de sink específica de YouTube**: Ajuste parámetros de streaming específicos a los requisitos de YouTube

## Comenzando: Proceso de configuración básica

### Configuración de clave de stream

La base de cualquier implementación de streaming de YouTube comienza con su clave de stream de YouTube. Este token de autenticación conecta su aplicación a su canal de YouTube:

```csharp
// Inicializar salida YouTube con su clave de stream
var youtubeOutput = new YouTubeOutput("su-clave-de-stream-youtube");
```

## Opciones de configuración del codificador de video

### Soporte completo de codificadores de video

El SDK proporciona soporte para múltiples codificadores de video, cada uno optimizado para diferentes entornos de hardware y requisitos de rendimiento:

| Tipo de codificador | Plataforma/Hardware | Características de rendimiento |
|---------------------|---------------------|-------------------------------|
| OpenH264 | Multiplataforma (software) | Intensivo en CPU, ampliamente compatible |
| NVENC H264 | GPUs NVIDIA | Acelerado por hardware, uso reducido de CPU |
| QSV H264 | CPUs Intel con Quick Sync | Acelerado por hardware, eficiente |
| AMF H264 | GPUs AMD | Acelerado por hardware para hardware AMD |
| HEVC/H265 | Varios (donde sea soportado) | Mayor eficiencia de compresión |

### Selección dinámica de codificador

El sistema selecciona inteligentemente codificadores predeterminados basándose en la plataforma:

```csharp
// Ejemplo: Usar codificador NVIDIA NVENC si está disponible
if (NVENCH264EncoderSettings.IsAvailable())
{
    youtubeOutput.Video = new NVENCH264EncoderSettings();
}
```

### Configuración de parámetros de codificación de video

Cada codificador soporta personalización de varios parámetros:

```csharp
var videoSettings = new OpenH264EncoderSettings
{
    Bitrate = 4500000,  // 4.5 Mbps
    KeyframeInterval = 60,  // Keyframe cada 2 segundos a 30fps
};
youtubeOutput.Video = videoSettings;
```

## Configuración del codificador de audio

### Codificadores de audio AAC soportados

El SDK soporta múltiples codificadores de audio AAC:

- **VO-AAC**: Predeterminado para plataformas no Windows, proporcionando codificación de audio consistente
- **AVENC AAC**: Opción alternativa multiplataforma
- **MF AAC**: Codificador específico de Windows usando Media Foundation

```csharp
// Ejemplo: Configurar ajustes del codificador de audio
var audioSettings = new VOAACEncoderSettings
{
    Bitrate = 128000,  // 128 kbps
    SampleRate = 48000  // 48 kHz (recomendado por YouTube)
};
youtubeOutput.Audio = audioSettings;
```

## Mejores prácticas para streaming óptimo

### Selección de codificador consciente del hardware

- Siempre verifique la disponibilidad del codificador antes de implementar opciones aceleradas por hardware
- Implemente mecanismos de respaldo a OpenH264 cuando hardware especializado no esté disponible

### Configuración de stream optimizada para YouTube

- Siga las tasas de bits recomendadas por YouTube para su resolución objetivo
- Implemente el intervalo estándar de keyframe de 2 segundos (60 cuadros a 30fps)
- Configure tasa de muestreo de audio de 48 kHz para cumplir con las especificaciones de audio de YouTube

## Ejemplos de implementación completa

### Integración VideoCaptureCoreX/VideoEditCoreX

```csharp
try
{
    var youtubeOutput = new YouTubeOutput("su-clave-de-stream");
    
    // Configurar codificación de video
    if (NVENCH264EncoderSettings.IsAvailable())
    {
        youtubeOutput.Video = new NVENCH264EncoderSettings
        {
            Bitrate = 4500000,
            KeyframeInterval = 60
        };
    }
    
    // Configurar codificación de audio
    youtubeOutput.Audio = new MFAACEncoderSettings
    {
        Bitrate = 128000,
        SampleRate = 48000
    };
    
    // Agregar la salida a la instancia de captura de video
    core.Outputs_Add(youtubeOutput, true); // core es una instancia de VideoCaptureCoreX

    // O establecer la salida para la instancia de edición de video
    videoEdit.Output_Format = youtubeOutput; // videoEdit es una instancia de VideoEditCoreX
}
catch (Exception ex)
{
    // Manejar errores de inicialización
    Console.WriteLine($"Error al inicializar salida YouTube: {ex.Message}");
}
```

### Implementación con Media Blocks SDK

```csharp
// Crear el bloque sink de YouTube (usando RTMP)
var youtubeSinkBlock = YouTubeSinkBlock(new YouTubeSinkSettings("clave de streaming"));

// Conectar el codificador de video al bloque sink
pipeline.Connect(h264Encoder.Output, youtubeSinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar el codificador de audio al bloque sink
pipeline.Connect(aacEncoder.Output, youtubeSinkBlock.CreateNewInput(MediaBlockPadMediaType.Audio));
```

## Solución de problemas comunes

### Problemas de inicialización del codificador

- Verifique la disponibilidad del codificador de hardware a través de diagnósticos del sistema
- Asegure que el sistema cumple todos los requisitos para el codificador elegido
- Confirme la instalación apropiada de controladores específicos de hardware para aceleración GPU

### Fallos de conexión del stream

- Valide el formato y estado de expiración de la clave de stream
- Pruebe la conectividad de red a los servidores de streaming de YouTube
- Verifique el estado del servicio de YouTube a través de canales oficiales

## Recursos adicionales

- [Documentación oficial de YouTube Live Streaming](https://support.google.com/youtube/topic/9257891)
- [Requisitos técnicos de stream de YouTube](https://support.google.com/youtube/answer/2853702)
