---
title: Codificadores AV1 en SDKs .NET
description: Configure codificadores AV1 en los SDK de Video Capture, Video Edit y Media Blocks multiplataforma para compresión de video eficiente de próxima generación.
---

# Codificadores AV1

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

VisioForge soporta múltiples implementaciones de codificadores AV1, cada una con sus propias características y capacidades únicas. Este documento cubre los codificadores disponibles y sus opciones de configuración.

Actualmente, los codificadores AV1 están soportados en los motores multiplataforma: `VideoCaptureCoreX`, `VideoEditCoreX`, y `Media Blocks SDK`.

## Codificadores disponibles

1. [Codificador AMD AMF AV1 (AMF)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AMFAV1EncoderSettings.html)
2. [Codificador NVIDIA NVENC AV1 (NVENC)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.NVENCAV1EncoderSettings.html)
3. [Codificador Intel QuickSync AV1 (QSV)](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.QSVAV1EncoderSettings.html)
4. [Codificador AOM AV1](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.AOMAV1EncoderSettings.html)
5. [Codificador RAV1E](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.VideoEncoders.RAV1EEncoderSettings.html)

Puede usar el codificador AV1 con [salida WebM](../output-formats/webm.md) o para streaming de red.

## Codificador AMD AMF AV1

El codificador AMD AMF AV1 proporciona codificación acelerada por hardware usando tarjetas gráficas AMD.

### Características

- Múltiples preajustes de calidad
- Modos de control de tasa de bits variable
- Control de tamaño GOP
- Control de QP (Parámetro de Cuantización)
- Soporte de Smart Access Video

### Modos de control de tasa

- `Default`: Depende del uso
- `CQP`: QP constante
- `LCVBR`: VBR restringido por latencia
- `VBR`: VBR restringido por pico
- `CBR`: Tasa de bits constante

### Ejemplo de uso

```csharp
var encoderSettings = new AMFAV1EncoderSettings
{
    Bitrate = 3000,                              // 3 Mbps
    GOPSize = 30,                                // Tamaño GOP de 30 cuadros
    Preset = AMFAV1EncoderPreset.Quality,        // Preajuste de calidad
    RateControl = AMFAV1RateControlMode.VBR,     // Modo de tasa de bits variable
    Usage = AMFAV1EncoderUsage.Transcoding,      // Uso de transcodificación
    MaxBitrate = 5000,                           // Tasa de bits máxima de 5 Mbps
    QpI = 26,                                    // QP de cuadro I
    QpP = 26,                                    // QP de cuadro P
    RefFrames = 1,                               // Número de cuadros de referencia
    SmartAccessVideo = false                     // Smart Access Video deshabilitado
};
```

## Codificador NVIDIA NVENC AV1

El codificador NVENC AV1 de NVIDIA proporciona codificación acelerada por hardware usando GPUs NVIDIA.

### Características

- Múltiples preajustes de codificación
- Soporte de cuadros B adaptativos
- AQ (Cuantización Adaptativa) temporal
- Control de buffer VBV (Video Buffering Verifier)
- Soporte de AQ espacial

### Modos de control de tasa

- `Default`: Modo predeterminado
- `ConstQP`: Parámetro de cuantización constante
- `CBR`: Tasa de bits constante
- `VBR`: Tasa de bits variable
- `CBR_LD_HQ`: CBR de baja latencia, alta calidad
- `CBR_HQ`: CBR, alta calidad (más lento)
- `VBR_HQ`: VBR, alta calidad (más lento)

### Ejemplo de uso

```csharp
var encoderSettings = new NVENCAV1EncoderSettings
{
    Bitrate = 3000,                          // 3 Mbps
    Preset = NVENCPreset.HighQuality,        // Preajuste de alta calidad
    RateControl = NVENCRateControl.VBR,      // Modo de tasa de bits variable
    GOPSize = 75,                            // Tamaño GOP de 75 cuadros
    MaxBitrate = 5000,                       // Tasa de bits máxima de 5 Mbps
    BFrames = 2,                             // 2 cuadros B entre I y P
    RCLookahead = 8,                         // 8 cuadros de lookahead
    TemporalAQ = true,                       // Habilitar AQ temporal
    Tune = NVENCTune.HighQuality,            // Ajuste de alta calidad
    VBVBufferSize = 6000                     // Buffer VBV de 6000k
};
```

## Codificador Intel QuickSync AV1

El codificador QuickSync AV1 de Intel proporciona codificación acelerada por hardware usando GPUs Intel.

### Características

- Soporte de modo de baja latencia
- Uso objetivo configurable
- Control de cuadros de referencia
- Configuración flexible de tamaño GOP

### Modos de control de tasa

- `CBR`: Tasa de bits constante
- `VBR`: Tasa de bits variable
- `CQP`: Cuantizador constante

### Ejemplo de uso

```csharp
var encoderSettings = new QSVAV1EncoderSettings
{
    Bitrate = 2000,                              // 2 Mbps
    LowLatency = false,                          // Modo de latencia estándar
    TargetUsage = 4,                             // Balance calidad/velocidad
    GOPSize = 30,                                // Tamaño GOP de 30 cuadros
    MaxBitrate = 4000,                           // Tasa de bits máxima de 4 Mbps
    QPI = 26,                                    // QP de cuadro I
    QPP = 28,                                    // QP de cuadro P
    RateControl = QSVAV1EncRateControl.VBR,      // Modo de tasa de bits variable
    RefFrames = 1                                // Número de cuadros de referencia
};
```

## Codificador AOM AV1

El codificador AOM AV1 de la Alliance for Open Media es una implementación de referencia basada en software.

### Características

- Configuraciones de control de buffer
- Optimización de uso de CPU
- Soporte de descarte de cuadros
- Capacidades de multi-threading
- Soporte de super-resolución

### Modos de control de tasa

- `VBR`: Modo de tasa de bits variable
- `CBR`: Modo de tasa de bits constante
- `CQ`: Modo de calidad restringida
- `Q`: Modo de calidad constante

### Ejemplo de uso

```csharp
var encoderSettings = new AOMAV1EncoderSettings
{
    BufferInitialSize = TimeSpan.FromMilliseconds(4000),
    BufferOptimalSize = TimeSpan.FromMilliseconds(5000),
    BufferSize = TimeSpan.FromMilliseconds(6000),
    CPUUsed = 4,                                   // Nivel de uso de CPU
    DropFrame = 0,                                 // Deshabilitar descarte de cuadros
    RateControl = AOMAV1EncoderEndUsageMode.VBR,   // Modo de tasa de bits variable
    TargetBitrate = 256,                           // 256 Kbps
    Threads = 0,                                   // Conteo automático de hilos
    UseRowMT = true,                               // Habilitar threading basado en filas
    SuperResMode = AOMAV1SuperResolutionMode.None  // Sin super-resolución
};
```

## Codificador RAV1E

RAV1E es un codificador AV1 rápido y seguro escrito en Rust.

### Características

- Control de preajuste de velocidad
- Configuración de cuantizador
- Control de intervalo de keyframes
- Modo de baja latencia
- Ajuste psicovisual

### Ejemplo de uso

```csharp
var encoderSettings = new RAV1EEncoderSettings
{
    Bitrate = 3000,                               // 3 Mbps
    LowLatency = false,                           // Modo de latencia estándar
    MaxKeyFrameInterval = 240,                    // Intervalo máximo de keyframes
    MinKeyFrameInterval = 12,                     // Intervalo mínimo de keyframes
    MinQuantizer = 0,                             // Valor mínimo de cuantizador
    Quantizer = 100,                              // Valor base de cuantizador
    SpeedPreset = 6,                              // Preajuste de velocidad (0-10)
    Tune = RAV1EEncoderTune.Psychovisual          // Ajuste psicovisual
};
```

## Notas generales de uso

1. Todos los codificadores implementan la interfaz `IAV1EncoderSettings`, proporcionando una forma consistente de crear bloques de codificador.
2. Cada codificador tiene su propio conjunto específico de optimizaciones y compensaciones.
3. Los codificadores de hardware (AMF, NVENC, QSV) generalmente proporcionan mejor rendimiento pero pueden tener requisitos de hardware específicos.
4. Los codificadores de software (AOM, RAV1E) ofrecen más flexibilidad pero pueden requerir más recursos de CPU.

## Recomendaciones

- Para GPUs AMD: Use el codificador AMF
- Para GPUs NVIDIA: Use el codificador NVENC
- Para GPUs Intel: Use el codificador QSV
- Para máxima calidad: Use el codificador AOM
- Para codificación eficiente en CPU: Use el codificador RAV1E

## Mejores prácticas

1. Siempre verifique la disponibilidad del codificador antes de usarlo
2. Establezca tasas de bits apropiadas basándose en su resolución y tasa de cuadros objetivo
3. Use tamaños GOP apropiados basándose en su tipo de contenido
4. Considere la compensación entre calidad y velocidad de codificación
5. Pruebe diferentes modos de control de tasa para encontrar el mejor ajuste para su caso de uso
