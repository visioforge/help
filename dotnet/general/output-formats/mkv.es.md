---
title: Formato de Contenedor MKV en Aplicaciones .NET
description: Salida MKV en .NET con codificación acelerada por hardware, múltiples pistas de audio y soporte flexible de contenedor Matroska.
---

# Salida MKV en los SDK .NET de VisioForge

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

## Introducción al formato MKV

MKV (Matroska Video) es un formato de contenedor flexible de estándar abierto que puede contener un número ilimitado de pistas de video, audio y subtítulos en un solo archivo. Los SDK de VisioForge proporcionan soporte robusto para salida MKV con varias opciones de codificación para satisfacer diversos requisitos de desarrollo.

Este formato es particularmente valioso para desarrolladores que trabajan en aplicaciones que requieren:

- Múltiples pistas de audio o idiomas
- Video de alta calidad con múltiples opciones de códec
- Compatibilidad multiplataforma
- Soporte para metadatos y capítulos

## Comenzando con salida MKV

La clase `MKVOutput` sirve como la interfaz principal para generar archivos MKV en los SDK de VisioForge. Puede inicializarla con configuración predeterminada o especificar codificadores personalizados para coincidir con las necesidades de su aplicación.

### Implementación básica

```csharp
// Crear salida MKV con codificadores predeterminados
var mkvOutput = new MKVOutput("output.mkv");

// O especificar codificadores personalizados durante la inicialización
var videoEncoder = new NVENCH264EncoderSettings();
var audioEncoder = new MFAACEncoderSettings();
var mkvOutput = new MKVOutput("output.mkv", videoEncoder, audioEncoder);
```

## Opciones de codificación de video

El formato MKV soporta múltiples códecs de video, dando a los desarrolladores flexibilidad para equilibrar calidad, rendimiento y compatibilidad. Los SDK de VisioForge ofrecen tanto codificadores de software como acelerados por hardware.

### Opciones de codificador H.264

H.264 sigue siendo uno de los códecs de video más ampliamente soportados, proporcionando excelente compresión y calidad. Para opciones de configuración detalladas, consulte la [documentación del codificador H.264](../video-encoders/h264.md).

- **OpenH264**: Codificador basado en software, usado por defecto cuando la aceleración de hardware no está disponible
- **NVENC H.264**: Codificación acelerada por GPU NVIDIA para rendimiento superior
- **QSV H.264**: Tecnología Intel Quick Sync Video para aceleración de hardware
- **AMF H.264**: Opción de codificación acelerada por GPU AMD

### Opciones de codificador HEVC (H.265)

Para aplicaciones que requieren mayor eficiencia de compresión o contenido 4K, consulte la [documentación del codificador HEVC](../video-encoders/hevc.md) para configuración detallada:

- **MF HEVC**: Implementación Windows Media Foundation (solo Windows)
- **NVENC HEVC**: Aceleración GPU NVIDIA para H.265
- **QSV HEVC**: Implementación Intel Quick Sync para H.265
- **AMF HEVC**: Aceleración GPU AMD para codificación H.265

### Configuración de un codificador de video

```csharp
mkvOutput.Video = new NVENCH264EncoderSettings();
```

## Opciones de codificación de audio

La calidad de audio es igualmente importante para la mayoría de aplicaciones. Los SDK de VisioForge proporcionan varias opciones de codificador de audio para salida MKV:

### Códecs de audio soportados

- **Codificadores AAC** - Consulte la [documentación del codificador AAC](../audio-encoders/aac.md):
  - **VO AAC**: Opción predeterminada para plataformas no Windows
  - **AVENC AAC**: Implementación AAC de FFMPEG
  - **MF AAC**: Implementación Windows Media Foundation (predeterminado en Windows)
  
- **Formatos de audio alternativos**:
  - **[MP3](../audio-encoders/mp3.md)**: Formato común con amplia compatibilidad
  - **[Vorbis](../audio-encoders/vorbis.md)**: Códec de audio de código abierto
  - **[OPUS](../audio-encoders/opus.md)**: Códec moderno con excelente relación calidad-tamaño

### Configuración de codificación de audio

```csharp
// Selección de codificador de audio específico de plataforma
#if NET_WINDOWS
    var aacSettings = new MFAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 48000
    };
    mkvOutput.Audio = aacSettings;
#else
    var aacSettings = new VOAACEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 44100
    };
    mkvOutput.Audio = aacSettings;
#endif

// O usar OPUS para mejor calidad a tasas de bits más bajas
var opusSettings = new OPUSEncoderSettings
{
    Bitrate = 128,
    Channels = 2
};
mkvOutput.Audio = opusSettings;
```

## Configuración avanzada de MKV

### Procesamiento personalizado de video y audio

Para aplicaciones que requieren procesamiento especial, puede integrar procesadores MediaBlock personalizados:

```csharp
// Agregar un procesador de video para efectos o transformaciones
var textOverlayBlock = new TextOverlayBlock(new TextOverlaySettings("¡Hola mundo!"));
mkvOutput.CustomVideoProcessor = textOverlayBlock;

// Agregar procesamiento de audio
var volumeBlock = new VolumeBlock() { Level = 1.2 }; // Aumentar volumen 20%
mkvOutput.CustomAudioProcessor = volumeBlock;
```

### Gestión de configuración de Sink

Controle las propiedades del archivo de salida a través de la configuración de sink:

```csharp
// Cambiar nombre de archivo de salida
mkvOutput.Sink.Filename = "salida_procesada.mkv";

// Obtener nombre de archivo actual
string archivoActual = mkvOutput.GetFilename();

// Actualizar nombre de archivo con marca de tiempo
string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
mkvOutput.SetFilename($"grabacion_{timestamp}.mkv");
```

## Integración con componentes del SDK de VisioForge

### Con Video Capture SDK

```csharp
// Inicializar núcleo de captura
var captureCore = new VideoCaptureCoreX();

// Configurar fuente de video y audio
// ...

// Agregar salida MKV al pipeline de grabación
var mkvOutput = new MKVOutput("captura.mkv");
captureCore.Outputs_Add(mkvOutput, true);

// Iniciar grabación
await captureCore.StartAsync();
```

### Con Video Edit SDK

```csharp
// Inicializar núcleo de edición
var editCore = new VideoEditCoreX();

// Agregar fuentes de entrada
// ...

// Configurar salida MKV con aceleración de hardware
var h265Encoder = new NVENCHEVCEncoderSettings
{
    Bitrate = 10000
};
var mkvOutput = new MKVOutput("editado.mkv", h265Encoder);
editCore.Output_Format = mkvOutput;

// Procesar el archivo
await editCore.StartAsync();
```

### Con Media Blocks SDK

```csharp
// Crear un pipeline
var pipeline = new MediaBlocksPipeline();

// Agregar bloque fuente
var sourceBlock = // algún bloque

## Implementación de interfaz

// Configurar salida MKV
var aacEncoder = new VOAACEncoderSettings();
var h264Encoder = new OpenH264EncoderSettings();
var mkvSinkSettings = new MKVSinkSettings("procesado.mkv");
var mkvOutput = new MKVOutputBlock(mkvSinkSettings, h264Encoder, aacEncoder);

// Conectar bloques y ejecutar el pipeline
pipeline.Connect(sourceBlock.VideoOutput, h264Encoder.Input);
pipeline.Connect(h264Encoder.Output, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(sourceBlock.AudioOutput, aacEncoder.Input);
pipeline.Connect(aacEncoder.Output, mkvOutput.CreateNewInput(MediaBlockPadMediaType.Audio));
pipeline.Connect(mkvOutput.Output, pipeline.Sink);

// Iniciar el pipeline
await pipeline.StartAsync();
```

## Beneficios de la aceleración de hardware

La codificación acelerada por hardware ofrece ventajas significativas para desarrolladores que construyen aplicaciones de procesamiento por lotes o en tiempo real:

1. **Carga de CPU reducida**: Descarga la codificación a hardware dedicado
2. **Procesamiento más rápido**: Mejora de rendimiento de hasta 5-10x
3. **Eficiencia energética**: Menor consumo de energía, importante para aplicaciones móviles
4. **Mayor calidad**: Algunos codificadores de hardware proporcionan mejor calidad por tasa de bits

## Mejores prácticas para desarrolladores

Al implementar salida MKV en sus aplicaciones, considere estas recomendaciones:

1. **Siempre verifique la disponibilidad de hardware** antes de usar codificadores acelerados por GPU
2. **Seleccione tasas de bits apropiadas** basándose en el tipo de contenido y resolución
3. **Use codificadores específicos de plataforma** donde sea posible para rendimiento óptimo
4. **Pruebe en plataformas objetivo** para asegurar compatibilidad
5. **Considere compensaciones calidad-tamaño** basándose en las necesidades de su aplicación

## Conclusión

El formato MKV proporciona a los desarrolladores un contenedor flexible y robusto para contenido de video en aplicaciones .NET. Con los SDK de VisioForge, puede aprovechar la aceleración de hardware, opciones avanzadas de codificación y procesamiento personalizado para crear aplicaciones de video de alto rendimiento.

Al entender los codificadores disponibles y las opciones de configuración, puede optimizar su implementación para plataformas de hardware específicas mientras mantiene compatibilidad multiplataforma donde sea necesario.
