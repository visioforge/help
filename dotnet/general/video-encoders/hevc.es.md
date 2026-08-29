---
title: Codificación HEVC: AMD, NVIDIA, Intel y Apple VideoToolbox
description: Codifique HEVC (H.265) con GPU AMD, NVIDIA, Intel y Apple VideoToolbox, incluido 10 bits y alfa, en sus aplicaciones .NET.
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
  - Streaming
  - Encoding
  - Editing
  - Conversion
  - Webcam
  - H.265
  - C#
primary_api_classes:
  - AMFHEVCEncoderSettings
  - NVENCHEVCEncoderSettings
  - QSVHEVCEncoderSettings
  - AppleMediaHEVCEncoderSettings
  - IHEVCEncoderSettings
  - SVTHEVCEncoderSettings

---

# Codificación HEVC por Hardware en Aplicaciones .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Esta guía explora las opciones de codificación HEVC (H.265) acelerada por hardware disponibles en los SDK .NET de VisioForge. Cubriremos los detalles de implementación para codificadores de GPU AMD, NVIDIA e Intel, ayudándole a elegir la solución correcta para sus necesidades de procesamiento de video.

Para formatos de salida específicos de Windows, consulte nuestra [documentación de salida MP4](../output-formats/mp4.md).

## Descripción general de codificadores HEVC por hardware

Las GPUs modernas ofrecen potentes capacidades de codificación por hardware que superan significativamente las soluciones basadas en software. Los SDK de VisioForge soportan cuatro codificadores HEVC de hardware:

- **AMD AMF** - Para GPUs AMD Radeon
- **NVIDIA NVENC** - Para GPUs NVIDIA GeForce y profesionales
- **Intel QuickSync** - Para CPUs Intel con gráficos integrados
- **Apple VideoToolbox** - Para macOS, Mac Catalyst e iOS

Cada codificador proporciona características únicas y opciones de optimización. Exploremos sus capacidades y detalles de implementación.

## Codificador AMD AMF HEVC

El Advanced Media Framework (AMF) de AMD ofrece codificación HEVC acelerada por hardware en GPUs Radeon compatibles. Equilibra velocidad de codificación, calidad y eficiencia para varios escenarios.

### Características principales y configuración

- **Métodos de control de tasa**:
  - `CQP` (QP constante) para configuraciones de calidad fija
  - `LCVBR` (VBR restringido por latencia) para streaming
  - `VBR` (Tasa de bits variable) para codificación sin conexión
  - `CBR` (Tasa de bits constante) para uso de ancho de banda confiable

- **Perfiles de uso**:
  - Transcodificación (mayor calidad)
  - Ultra baja latencia (para aplicaciones en tiempo real)
  - Baja latencia (para streaming interactivo)
  - Cámara web (optimizado para fuentes de webcam)

- **Preajustes de calidad**: Balance entre velocidad de codificación y calidad de salida

### Ejemplo de implementación

```csharp
var encoder = new AMFHEVCEncoderSettings
{
    Bitrate = 3000, // Tasa de bits objetivo de 3 Mbps
    MaxBitrate = 5000, // Tasa de bits pico de 5 Mbps
    RateControl = AMFHEVCEncoderRateControl.CBR,
    
    // Optimización de calidad
    Preset = AMFHEVCEncoderPreset.Quality,
    Usage = AMFHEVCEncoderUsage.Transcoding,
    
    // Configuración de GOP y cuadros
    GOPSize = 30, // Intervalo de keyframes
    QP_I = 22, // Parámetro de cuantización de cuadros I
    QP_P = 22, // Parámetro de cuantización de cuadros P
    
    RefFrames = 1 // Conteo de cuadros de referencia
};
```

## Codificador NVIDIA NVENC HEVC

La tecnología NVENC de NVIDIA proporciona hardware de codificación dedicado en GPUs GeForce y profesionales, ofreciendo excelente rendimiento y calidad a través de varias tasas de bits.

### Capacidades principales

- **Soporte de múltiples perfiles**:
  - Main (8-bit)
  - Main10 (HDR de 10 bits)
  - Main444 (alta precisión de color)
  - Opciones de profundidad de bits extendida (12 bits)

- **Características avanzadas de codificación**:
  - Soporte de cuadros B con colocación adaptativa
  - Cuantización adaptativa temporal
  - Predicción ponderada
  - Control de tasa con look-ahead

- **Preajustes de rendimiento**: Desde codificación enfocada en calidad hasta ultra rápida

### Ejemplo de implementación

```csharp
var encoder = new NVENCHEVCEncoderSettings
{
    // Configuración de tasa de bits
    Bitrate = 3000, // Objetivo de 3 Mbps
    MaxBitrate = 5000, // Máximo de 5 Mbps
    
    // Configuración de perfil
    Profile = NVENCHEVCProfile.Main,
    Level = NVENCHEVCLevel.Level5_1,
    
    // Opciones de mejora de calidad
    BFrames = 2, // Número de cuadros B
    BAdaptive = true, // Colocación adaptativa de cuadros B
    TemporalAQ = true, // Cuantización adaptativa temporal
    WeightedPrediction = true, // Mejora calidad para fundidos
    RCLookahead = 20, // Cuadros a analizar para control de tasa
    
    // Configuración de buffer
    VBVBufferSize = 0 // Usar tamaño de buffer predeterminado
};
```

## Codificador Intel QuickSync HEVC

Intel QuickSync aprovecha la GPU integrada presente en procesadores Intel modernos para codificación por hardware eficiente, haciéndolo accesible sin una tarjeta gráfica dedicada.

### Características principales

- **Opciones versátiles de control de tasa**:
  - `CBR` (Tasa de bits constante)
  - `VBR` (Tasa de bits variable)
  - `CQP` (Cuantizador constante)
  - `ICQ` (Calidad constante inteligente)
  - `VCM` (Modo de videoconferencia)
  - `QVBR` (VBR definido por calidad)

- **Configuraciones de optimización**:
  - Parámetro de uso objetivo (balance calidad vs velocidad)
  - Modo de baja latencia para streaming
  - Controles de conformidad HDR
  - Opciones de inserción de subtítulos

- **Soporte de perfiles**:
  - Main (8-bit)
  - Main10 (HDR de 10 bits)

### Ejemplo de implementación

```csharp
var encoder = new QSVHEVCEncoderSettings
{
    // Configuración de tasa de bits
    Bitrate = 3000, // Objetivo de 3 Mbps
    MaxBitrate = 5000, // Pico de 5 Mbps
    RateControl = QSVHEVCEncRateControl.VBR,
    
    // Ajuste de calidad
    TargetUsage = 4, // 1=Mejor calidad, 7=Codificación más rápida
    
    // Estructura del flujo
    GOPSize = 30, // Intervalo de keyframes
    RefFrames = 2, // Cuadros de referencia
    
    // Configuración de características
    Profile = QSVHEVCEncProfile.Main,
    LowLatency = false, // Habilitar para streaming
    
    // Opciones avanzadas
    CCInsertMode = QSVHEVCEncSEIInsertMode.Insert,
    DisableHRDConformance = false
};
```

## Codificador HEVC de Apple VideoToolbox

VideoToolbox es el codificador por hardware en macOS, Mac Catalyst e iOS. Está disponible en
Apple Silicon y en Macs Intel con gráficos compatibles con QuickSync.

### Características principales

- **Control de tasa**:
  - `ABR` (tasa de bits media, el valor por defecto)
  - `CBR` (tasa de bits constante)
  - Límites de tasa de datos: un techo de bitrate promediado sobre una ventana deslizante, en modo `ABR`

- **Perfiles soportados**:
  - Main (8 bits)
  - Main10 (10 bits)

- **Canal alfa**: `PreserveAlpha` cambia al codificador con soporte de alfa

### Ejemplo de implementación

```csharp
// Tasa de bits constante, para un endpoint de ancho de banda fijo.
var cbr = new AppleMediaHEVCEncoderSettings
{
    Bitrate = 3000,                              // 3 Mbps
    RateControl = AppleMediaRateControl.CBR,
    Profile = AppleMediaHEVCProfile.Main,
    Realtime = true,
    Quality = 0.5
};

// Tasa media con un techo duro: objetivo de 3 Mbps, nunca más de 4,5 Mbps
// promediados sobre cualquier segundo.
var cappedAbr = new AppleMediaHEVCEncoderSettings
{
    Bitrate = 3000,
    RateControl = AppleMediaRateControl.ABR,     // el valor por defecto
    DataRateLimitBitrate = 4500,
    DataRateLimitDuration = 1.0,
    Profile = AppleMediaHEVCProfile.Main
};
```

### CBR y los límites de tasa de datos son alternativas

No configures ambos. Con `CBR` seleccionado el codificador ignora por completo los límites de
tasa de datos y lo indica en su registro (`Ignoring data-rate-limits property, CBR mode is
enabled`): la tasa constante ya es un techo. Los límites se aplican en modo `ABR`, donde
convierten «apunta a esta tasa» en «apunta a esta tasa y nunca superes aquella otra en ninguna
ventana de esta duración».

### CBR no es constante en todas partes

La tasa de bits verdaderamente constante es una capacidad de VideoToolbox, no del SDK: requiere
macOS 13+ o iOS 16+ sobre Apple Silicon. En sistemas anteriores y en Macs Intel, VideoToolbox
emula CBR mediante límites de tasa de datos propios, derivados de `Bitrate`, y lo indica en su
propio registro (`CBR is unsupported on your system, emulating with custom data rate limits`).
La salida codificada resulta entonces casi constante en lugar de constante, lo que importa si se
dimensiona un presupuesto de transporte fijo a partir de ella.

### Notas de plataforma

- **10 bits** (`AppleMediaHEVCProfile.Main10`), el codificador con **alfa** y el control de tasa
  requieren el runtime de GStreamer 1.28.6, que se incluye en los paquetes de macOS y Mac Catalyst.
  El paquete de iOS todavía incluye 1.24.9, que no tiene ninguno de ellos: allí el control de tasa
  se descarta con una advertencia en lugar de aplicarse en silencio, y los 10 bits no negocian.
- `ForceHWUsage` fija el codificador al elemento exclusivo de hardware, fallando en lugar de caer
  a una implementación por software. No está disponible en iOS.
- En la ruta del pipeline de MediaBlocks el SDK siempre inserta `h265parse` después de este
  codificador: VideoToolbox emite solo `hvc1`, mientras que el multiplexor MPEG-TS detrás de la
  salida SRT/UDP/RIST acepta solo byte-stream. La ruta RTSP no necesita parser: `rtph265pay` acepta
  `hvc1` directamente.

## Preajustes de calidad para configuración simplificada

Los codificadores de AMD, NVIDIA e Intel soportan preajustes de calidad estandarizados a través de la enumeración `VideoQuality`, proporcionando un enfoque de configuración simplificado:

- **Low**: Objetivo de 1 Mbps, máximo de 2 Mbps (para streaming básico)
- **Normal**: Objetivo de 3 Mbps, máximo de 5 Mbps (para contenido estándar)
- **High**: Objetivo de 6 Mbps, máximo de 10 Mbps (para contenido detallado)
- **Very High**: Objetivo de 15 Mbps, máximo de 25 Mbps (para calidad premium)


`AppleMediaHEVCEncoderSettings` no tiene constructor con `VideoQuality`: configura `Bitrate` y `Quality` directamente, como en el ejemplo de VideoToolbox de arriba.
### Uso de preajustes de calidad

```csharp
// Para AMD AMF
var amfEncoder = new AMFHEVCEncoderSettings(VideoQuality.High);

// Para NVIDIA NVENC
var nvencEncoder = new NVENCHEVCEncoderSettings(VideoQuality.High);

// Para Intel QuickSync
var qsvEncoder = new QSVHEVCEncoderSettings(VideoQuality.High);
```

## Detección de hardware y estrategia de respaldo

Una implementación robusta debe verificar la disponibilidad del codificador e implementar respaldos apropiados:

```csharp
// Crear el codificador más apropiado para el sistema actual
IHEVCEncoderSettings GetOptimalHEVCEncoder()
{
    if (AMFHEVCEncoderSettings.IsAvailable())
    {
        return new AMFHEVCEncoderSettings(VideoQuality.High);
    }
    else if (NVENCHEVCEncoderSettings.IsAvailable())
    {
        return new NVENCHEVCEncoderSettings(VideoQuality.High);
    }
    else if (QSVHEVCEncoderSettings.IsAvailable())
    {
        return new QSVHEVCEncoderSettings(VideoQuality.High);
    }
    else
    {
#if __MACOS__ || __MACCATALYST__ || __IOS__
        // Apple VideoToolbox.
        return new AppleMediaHEVCEncoderSettings { Bitrate = 6000 };
#elif NET_LINUX
        // Usar el codificador software SVT-HEVC en Linux (gated solo a Linux — ver SVTHEVCEncoderSettings).
        return new SVTHEVCEncoderSettings();
#else
        // El motor X NO incluye una clase IHEVCEncoderSettings tipada de respaldo por software para Windows.
        // El llamador debe decidir: mantener H.264, instalar un driver
        // de GPU con QSV/NVENC/AMF, o usar el pipeline FFMPEG-EXE del motor clásico.
        throw new NotSupportedException("No hay codificador HEVC disponible en esta plataforma. Instala un driver de GPU o cambia a H.264.");
#endif
    }
}
```

## Mejores prácticas para codificación HEVC

### 1. Selección de codificador

- **GPUs AMD**: Mejor para aplicaciones donde sabe que los usuarios tienen hardware AMD
- **GPUs NVIDIA**: Proporciona calidad consistente a través de generaciones, ideal para aplicaciones profesionales
- **Intel QuickSync**: Gran opción universal cuando una GPU dedicada no está garantizada
- **Apple VideoToolbox**: El único codificador HEVC por hardware en macOS, Mac Catalyst e iOS

### 2. Selección de control de tasa

- **Streaming**: Use CBR para utilización de ancho de banda consistente
- **Contenido VoD**: VBR proporciona mejor calidad al mismo tamaño de archivo
- **Archivo**: CQP asegura calidad consistente independientemente de la complejidad del contenido

### 3. Optimización de rendimiento

- Reduzca el conteo de cuadros de referencia para codificación más rápida
- Ajuste el tamaño GOP basándose en el tipo de contenido (menor para alto movimiento, mayor para escenas estáticas)
- Considere deshabilitar cuadros B para aplicaciones de ultra baja latencia

### 4. Mejora de calidad

- Habilite características de cuantización adaptativa para contenido con complejidad variable
- Use predicción ponderada para contenido con fundidos o transiciones graduales
- Implemente look-ahead cuando la calidad de codificación es más importante que la latencia

## Solución de problemas comunes

1. **No disponibilidad del codificador**: Asegúrese de que los controladores de GPU estén actualizados
2. **Calidad inferior a la esperada**: Verifique que los preajustes de calidad coincidan con su tipo de contenido
3. **Problemas de rendimiento**: Monitoree la utilización de GPU y ajuste la configuración en consecuencia
4. **Problemas de compatibilidad**: Verifique que los dispositivos objetivo soporten el perfil HEVC seleccionado

## Conclusión

La codificación HEVC acelerada por hardware ofrece ventajas significativas de rendimiento para aplicaciones .NET que trabajan con procesamiento de video. Al aprovechar AMD AMF, NVIDIA NVENC o Intel QuickSync a través de los SDK de VisioForge, puede lograr un balance óptimo entre calidad, velocidad y eficiencia.

Elija el codificador y configuración correctos basándose en sus requisitos específicos, audiencia objetivo y tipo de contenido para ofrecer la mejor experiencia posible en sus aplicaciones.

Comience detectando los codificadores de hardware disponibles, implementando configuraciones de calidad apropiadas y probando a través de varios tipos de contenido para asegurar resultados óptimos.
