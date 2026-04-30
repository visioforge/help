---
title: Integración Profesional de MXF para Aplicaciones .NET
description: Genere archivos MXF de broadcast en .NET con aceleración por hardware, optimización de códecs y flujos de trabajo profesionales para producción de transmisión.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Encoding
  - Editing
  - MXF
  - H.264
  - H.265
  - AAC
  - MP3
  - C#
primary_api_classes:
  - QSVH264EncoderSettings
  - MXFOutput
  - NVENCH264EncoderSettings
  - AMFH264EncoderSettings
  - MFAACEncoderSettings

---

# Salida MXF en los SDK .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Material Exchange Format (MXF) es un formato contenedor estándar de la industria diseñado para aplicaciones profesionales de video. Es ampliamente adoptado en entornos de broadcast, flujos de trabajo de post-producción y sistemas de archivo. Los SDK de VisioForge proporcionan capacidades robustas y multiplataforma de salida MXF que permiten a los desarrolladores integrar este formato profesional en sus aplicaciones.

## Entendiendo el formato MXF

MXF sirve como un envoltorio que puede contener varios tipos de datos de video y audio junto con metadatos. El formato fue diseñado para abordar problemas de interoperabilidad en flujos de trabajo profesionales de video:

- **Estándar de la industria**: Adoptado por las principales emisoras del mundo
- **Metadatos profesionales**: Admite amplios metadatos técnicos y descriptivos
- **Contenedor versátil**: Compatible con numerosos códecs de audio y video
- **Multiplataforma**: Compatible con Windows, macOS y Linux

## Comenzando con la salida MXF

Dos rutas de código cubren el 99% de los casos:

- **`MXFOutput`** (clase en `VisioForge.Core.Types.X.Output`) es un objeto de configuración consumido por `VideoCaptureCoreX.Outputs_Add(...)` o establecido como `VideoEditCoreX.Output_Format`.
- **`MXFSinkBlock`** + **`MXFSinkSettings`** es la ruta de Media Blocks cuando manejas el pipeline a mano.

### Implementación básica

Aquí está el código básico para crear una salida MXF:

```csharp
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);
```

Esto crea una salida MXF válida con configuraciones de codificación predeterminadas. Para aplicaciones profesionales, normalmente querrás personalizar los parámetros de codificación.

## Opciones de codificación de video para MXF

La calidad y compatibilidad de tu salida MXF depende en gran medida de tu elección del codificador de video. Los SDK de VisioForge admiten múltiples opciones de codificador para equilibrar rendimiento, calidad y compatibilidad. Para opciones detalladas de configuración, consulta la [documentación del codificador H.264](../video-encoders/h264.md) y la [documentación del codificador HEVC](../video-encoders/hevc.md).

> Las propiedades `Bitrate` de los codificadores de video del espacio X están en **Kbps** (así 8000 = 8 Mbps). No pases bits por segundo directos.

### Codificadores acelerados por hardware

Para un rendimiento óptimo en aplicaciones en tiempo real, se recomiendan los codificadores acelerados por hardware:

#### Codificadores NVIDIA NVENC

```csharp
// Verifica la disponibilidad primero
if (NVENCH264EncoderSettings.IsAvailable())
{
    var nvencSettings = new NVENCH264EncoderSettings
    {
        Bitrate = 8000, // 8 Mbps (Kbps)
    };

    mxfOutput.Video = nvencSettings;
}
```

#### Codificadores Intel Quick Sync Video (QSV)

```csharp
if (QSVH264EncoderSettings.IsAvailable())
{
    var qsvSettings = new QSVH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = qsvSettings;
}
```

#### Codificadores AMD Advanced Media Framework (AMF)

```csharp
if (AMFH264EncoderSettings.IsAvailable())
{
    var amfSettings = new AMFH264EncoderSettings
    {
        Bitrate = 8000,
    };

    mxfOutput.Video = amfSettings;
}
```

### Codificadores por software

Cuando la aceleración por hardware no está disponible, los codificadores por software proporcionan alternativas confiables:

#### Codificador OpenH264

```csharp
var openH264Settings = new OpenH264EncoderSettings
{
    Bitrate = 8000,
};

mxfOutput.Video = openH264Settings;
```

### Codificación de Video de Alta Eficiencia (HEVC/H.265)

Para aplicaciones que requieren mayor eficiencia de compresión:

```csharp
// Codificador HEVC NVIDIA
if (NVENCHEVCEncoderSettings.IsAvailable())
{
    var nvencHevcSettings = new NVENCHEVCEncoderSettings
    {
        Bitrate = 5000, // Bitrate más bajo posible con HEVC
    };

    mxfOutput.Video = nvencHevcSettings;
}
```

## Codificación de audio para archivos MXF

Mientras que el video a menudo recibe la mayor atención, la codificación adecuada de audio es crucial para salidas MXF profesionales. Los SDK de VisioForge ofrecen múltiples opciones de codificador de audio. Para opciones detalladas de configuración, consulta la [documentación del codificador AAC](../audio-encoders/aac.md) y la [documentación del codificador MP3](../audio-encoders/mp3.md).

> El `Bitrate` de codificadores de audio del espacio X también está en **Kbps** (así 192 = 192 kbps). `MFAACEncoderSettings` y `VOAACEncoderSettings` sí exponen una propiedad `SampleRate` (por defecto 48000); solo `MP3EncoderSettings` carece de setter de tasa de muestreo y sigue el formato de audio de la fuente upstream. La disposición de canales en los tres sigue el audio upstream salvo que se reconfigure antes (p. ej., con `AudioResamplerBlock`).

### Codificadores AAC

AAC es el códec preferido para la mayoría de aplicaciones profesionales:

```csharp
// Media Foundation AAC (solo Windows)
#if NET_WINDOWS
    var mfAacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192, // kbps
    };

    mxfOutput.Audio = mfAacSettings;
#else
    // Alternativa AAC multiplataforma
    var voAacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
    };

    mxfOutput.Audio = voAacSettings;
#endif
```

### Codificador MP3

Para máxima compatibilidad:

```csharp
var mp3Settings = new MP3EncoderSettings
{
    Bitrate = 320,         // Kbps — debe ser uno de 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320
    ForceMono = false      // Por defecto; establece true para mezclar a mono
};

mxfOutput.Audio = mp3Settings;
```

## Configuración avanzada de MXF

### Pipelines de procesamiento personalizados

Una de las características poderosas de los SDK de VisioForge es la capacidad de agregar procesamiento personalizado a tu cadena de salida MXF:

```csharp
// Agregar procesamiento de video personalizado
mxfOutput.CustomVideoProcessor = yourVideoProcessingBlock;

// Agregar procesamiento de audio personalizado
mxfOutput.CustomAudioProcessor = yourAudioProcessingBlock;
```

### Configuración del sink

Afina tu salida MXF con configuraciones de sink:

```csharp
// Acceder a las configuraciones del sink (MXFSinkSettings)
mxfOutput.Sink.Filename = "new_output.mxf";
```

## Consideraciones multiplataforma

Construir aplicaciones que funcionen en diferentes plataformas requiere planificación cuidadosa:

```csharp
// Selección de codificador específico de plataforma
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

#if NET_WINDOWS
    if (QSVH264EncoderSettings.IsAvailable())
    {
        mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
        mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
    }
#elif NET_MACOS
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
    mxfOutput.Audio = new MP3EncoderSettings { Bitrate = 320 };
#endif
```

## Manejo de errores y validación

Las implementaciones robustas de MXF requieren manejo adecuado de errores:

```csharp
try
{
    // Crear salida MXF
    var mxfOutput = new MXFOutput(
        filename: Path.Combine(outputDirectory, "output.mxf"),
        videoStreamType: MXFVideoStreamType.H264,
        audioStreamType: MXFAudioStreamType.MPEG
    );

    // Validar disponibilidad del codificador
    if (!OpenH264EncoderSettings.IsAvailable())
    {
        throw new ApplicationException("No se encontró un codificador H.264 compatible");
    }

    // Validar el directorio de salida
    var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(mxfOutput.Sink.Filename));
    if (!directoryInfo.Exists)
    {
        Directory.CreateDirectory(directoryInfo.FullName);
    }

    // Adjuntar MXFOutput como salida de VideoCaptureCoreX
    videoCapture.Outputs_Add(mxfOutput, autostart: true);
    await videoCapture.StartAsync();
}
catch (Exception ex)
{
    logger.LogError($"Error de salida MXF: {ex.Message}");
    // Implementar estrategia de fallback
}
```

## Optimización de rendimiento

Para un rendimiento óptimo de salida MXF:

1. **Prioriza la aceleración por hardware**: Siempre verifica y usa codificadores de hardware primero
2. **Gestión de búfer**: Ajusta tamaños de búfer basado en las capacidades del sistema
3. **Procesamiento paralelo**: Utiliza multi-threading donde sea apropiado
4. **Selección de preset**: Elige presets de codificador basados en requisitos de calidad vs. velocidad

## Ejemplo completo de implementación — VideoCaptureCoreX

Aquí tienes un ejemplo completo que demuestra la implementación MXF con opciones de fallback:

```csharp
// Crear salida MXF con tipos de stream específicos
var mxfOutput = new MXFOutput(
    filename: "output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG
);

// Configurar codificador de video con cadena de fallback priorizada (bitrate en Kbps)
if (NVENCH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new NVENCH264EncoderSettings { Bitrate = 8000 };
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new QSVH264EncoderSettings { Bitrate = 8000 };
}
else if (AMFH264EncoderSettings.IsAvailable())
{
    mxfOutput.Video = new AMFH264EncoderSettings { Bitrate = 8000 };
}
else
{
    mxfOutput.Video = new OpenH264EncoderSettings { Bitrate = 8000 };
}

// Configurar audio optimizado por plataforma (Kbps)
#if NET_WINDOWS
    mxfOutput.Audio = new MFAACEncoderSettings { Bitrate = 192 };
#else
    mxfOutput.Audio = new VOAACEncoderSettings { Bitrate = 192 };
#endif

// Adjuntar a VideoCaptureCoreX (o VideoEditCoreX: videoEdit.Output_Format = mxfOutput;)
videoCapture.Outputs_Add(mxfOutput, autostart: true);

await videoCapture.StartAsync();
```

## Ejemplo completo de implementación — MediaBlocksPipeline

Cuando manejes el pipeline a mano, usa `MXFSinkBlock` + `MXFSinkSettings` en lugar de `MXFOutput`:

```csharp
var pipeline = new MediaBlocksPipeline();

var mxfSettings = new MXFSinkSettings("output.mxf",
    videoStreamType: MXFVideoStreamType.H264,
    audioStreamType: MXFAudioStreamType.MPEG);

var mxfSink = new MXFSinkBlock(mxfSettings);

// videoEncoder / audioEncoder son instancias existentes de H264EncoderBlock / AACEncoderBlock
pipeline.Connect(videoEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, mxfSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

Siguiendo esta guía, puedes implementar salida MXF de calidad profesional en tus aplicaciones usando los SDK .NET de VisioForge, garantizando compatibilidad con flujos de trabajo de broadcast y sistemas de post-producción.
