---
title: Guía de Salida de Archivos MPEG-TS para .NET
description: Implemente salida MPEG Transport Stream en .NET con codificación de video y audio, aceleración de hardware y soporte de streaming multiplataforma.
---

# Salida MPEG-TS

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El módulo de salida MPEG-TS (Transport Stream) en el SDK de VisioForge proporciona funcionalidad para crear archivos de flujo de transporte MPEG con varias opciones de codificación de video y audio. Esta guía explica cómo configurar y usar la clase `MPEGTSOutput` efectivamente.

## Salida MPEG-TS multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Para crear una nueva salida MPEG-TS, use el siguiente constructor:

```csharp
// Inicializar con audio AAC (recomendado)
var output = new MPEGTSOutput("output.ts", useAAC: true);
```

También puede usar audio MP3 en lugar de AAC:

```csharp
// Inicializar con audio MP3 en lugar de AAC
var output = new MPEGTSOutput("output.ts", useAAC: false);
```

### Opciones de codificación de video

La clase [MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) soporta múltiples codificadores de video a través de la propiedad `Video`. Los codificadores disponibles incluyen:

**[Codificadores H.264](../video-encoders/h264.md)**

- OpenH264 (Basado en software)
- NVENC H.264 (Aceleración GPU NVIDIA)
- QSV H.264 (Intel Quick Sync)
- AMF H.264 (Aceleración GPU AMD)

**[Codificadores H.265/HEVC](../video-encoders/hevc.md)**

- MF HEVC (Windows Media Foundation, solo Windows)
- NVENC HEVC (Aceleración GPU NVIDIA)
- QSV HEVC (Intel Quick Sync)
- AMF H.265 (Aceleración GPU AMD)

Ejemplo de configuración de un codificador de video específico:

```csharp
// Verificar si el codificador NVIDIA está disponible
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else
{
    // Recurrir a OpenH264
    output.Video = new OpenH264EncoderSettings();
}
```

### Opciones de codificación de audio

Los siguientes codificadores de audio son soportados a través de la propiedad `Audio`:

**[Codificadores AAC](../audio-encoders/aac.md)**

- VO-AAC (Multiplataforma)
- AVENC AAC
- MF AAC (Solo Windows)
  
**[Codificador MP3](../audio-encoders/mp3.md)**:

- MP3EncoderSettings

Ejemplo de configuración de un codificador de audio:

```csharp
// Para plataformas Windows
output.Audio = new MFAACEncoderSettings();
```

```csharp
// Para compatibilidad multiplataforma
output.Audio = new VOAACEncoderSettings();
```

```csharp
// Usando MP3 en lugar de AAC
output.Audio = new MP3EncoderSettings();
```

### Gestión de archivos

Puede obtener o establecer el nombre de archivo de salida después de la inicialización:

```csharp
// Obtener nombre de archivo actual
string archivoActual = output.GetFilename();

// Cambiar nombre de archivo de salida
output.SetFilename("nueva_salida.ts");
```

### División de archivos

La clase `MPEGTSSplitSinkSettings` permite la división automática de la salida MPEG-TS en múltiples archivos basándose en tamaño, duración o código de tiempo. Esto es útil para:

- Crear archivos segmentados para streaming HLS
- Gestionar almacenamiento limitando tamaños de archivo
- Grabar videos de time-lapse
- Implementar grabación de búfer circular

#### Opciones de configuración

```csharp
using VisioForge.Core.Types.X.Sinks;

// Crear configuración de división con patrón de nombre de archivo
// %05d será reemplazado con número de segmento (00000, 00001, etc.)
var splitSettings = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    // Dividir por duración (ej., cada 5 minutos)
    SplitDuration = TimeSpan.FromMinutes(5),
    
    // Dividir por tamaño de archivo (ej., 100 MB, 0 = deshabilitado)
    SplitFileSize = 100 * 1024 * 1024,
    
    // Número máximo de archivos a mantener (archivos antiguos eliminados, 0 = ilimitado)
    SplitMaxFiles = 10,
    
    // Dividir por diferencia de código de tiempo (opcional)
    SplitMaxSizeTimecode = "01:00:00:00", // 1 hora
    
    // Índice inicial para numeración de segmentos
    StartIndex = 0,
    
    // Modo M2TS (formato Blu-ray con paquetes de 192 bytes)
    M2TSMode = false
};

// Aplicar a salida
output.Sink = splitSettings;
```

#### Disparadores de división

Los archivos se dividirán cuando se cumpla cualquiera de estas condiciones:

1. **Basado en duración**: `SplitDuration` - Se crea nuevo archivo después del tiempo especificado
2. **Basado en tamaño**: `SplitFileSize` - Se crea nuevo archivo cuando se alcanza el límite de tamaño
3. **Basado en código de tiempo**: `SplitMaxSizeTimecode` - Nuevo archivo cuando se alcanza la diferencia de código de tiempo

#### Patrón de nombre de archivo

El patrón de nombre de archivo usa formato estilo printf para números de segmento:

```csharp
// Ejemplos de patrones de nombre de archivo
"grabacion_%02d.ts"   // grabacion_00.ts, grabacion_01.ts, ...
"stream_%05d.ts"      // stream_00000.ts, stream_00001.ts, ...
"salida_%d.ts"        // salida_0.ts, salida_1.ts, ...
```

#### Grabación de búfer circular

Para implementar un búfer circular que mantenga solo los últimos N segmentos:

```csharp
var settings = new MPEGTSSplitSinkSettings("buffer_%03d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(1),  // Segmentos de 1 minuto
    SplitMaxFiles = 60  // Mantener últimos 60 minutos
};
```

#### Ejemplo de uso

```csharp
// Ejemplo completo con configuración de división
var output = new MPEGTSOutput("video_%05d.ts", useAAC: true);

// Configurar ajustes de división
output.Sink = new MPEGTSSplitSinkSettings("video_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(5),
    SplitMaxFiles = 12,  // Mantener última hora (12 x 5 minutos)
    M2TSMode = false
};

// Configurar codificadores
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}

// Usar con VideoCaptureCoreX o MediaBlocksPipeline
// El patrón de nombre de archivo se usará automáticamente
```

### Características avanzadas

#### Procesamiento personalizado

MPEGTSOutput soporta procesamiento personalizado de video y audio a través de MediaBlocks:

```csharp
// Agregar procesamiento de video personalizado
output.CustomVideoProcessor = new SuProcesadorDeVideoPersonalizado();

// Agregar procesamiento de audio personalizado
output.CustomAudioProcessor = new SuProcesadorDeAudioPersonalizado();
```

#### Configuración de Sink

La salida usa MP4SinkSettings para configuración:

```csharp
// Acceder a configuración de sink
output.Sink.Filename = "salida_modificada.ts";
```

### Consideraciones de plataforma

- Algunos codificadores (MF AAC, MF HEVC) solo están disponibles en plataformas Windows
- Las aplicaciones multiplataforma deben usar codificadores agnósticos de plataforma como VO-AAC para audio

### Mejores prácticas

1. **Aceleración de hardware**: Cuando esté disponible, prefiera codificadores acelerados por hardware (NVENC, QSV, AMF) sobre codificadores de software para mejor rendimiento.

2. **Selección de códec de audio**: Use AAC para mejor compatibilidad y calidad a menos que tenga requisitos específicos para MP3.

3. **Manejo de errores**: Siempre verifique la disponibilidad del codificador antes de usar opciones aceleradas por hardware:

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    // Usar codificador NVIDIA
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    // Recurrir a Intel Quick Sync
}
else
{
    // Recurrir a codificación de software
}
```

**Compatibilidad multiplataforma**: Para aplicaciones multiplataforma, asegúrese de usar codificadores disponibles en todas las plataformas objetivo o implemente respaldos apropiados.

### Ejemplo de implementación

Aquí hay un ejemplo completo que muestra cómo crear y configurar una salida MPEG-TS:

```csharp
var output = new MPEGTSOutput("output.ts", useAAC: true);

// Configurar codificador de video
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
else if (QSVH264EncoderSettings.IsAvailable())
{
    output.Video = new QSVH264EncoderSettings();
}
else
{
    output.Video = new OpenH264EncoderSettings();
}

// Configurar codificador de audio basado en plataforma
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new VOAACEncoderSettings();
#endif

// Opcional: Agregar procesamiento personalizado
output.CustomVideoProcessor = new SuProcesadorDeVideoPersonalizado();
output.CustomAudioProcessor = new SuProcesadorDeAudioPersonalizado();
```

## Salida MPEG-TS solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La clase `MPEGTSOutput` proporciona configuración para salida MPEG Transport Stream (MPEG-TS) en el framework de procesamiento de video VisioForge. Esta clase hereda de `MFBaseOutput` e implementa la interfaz `IVideoCaptureBaseOutput`, permitiendo su uso específicamente para escenarios de captura de video con formato MPEG-TS.

### Jerarquía de clases

```text
MFBaseOutput
    └── MPEGTSOutput
```

### Configuración de video heredada

La clase [MPEGTSOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MPEGTSOutput.html) hereda capacidades de codificación de video de MFBaseOutput, que incluye:

**Configuración de codificación de video**: A través de la propiedad `Video`, soportando:

- Múltiples opciones de códec (H.264/H.265) con soporte de aceleración de hardware
- Control de tasa de bits (CBR/VBR)
- Configuración de calidad
- Configuración de tipo de cuadro y estructura GOP
- Opciones de entrelazado
- Controles de resolución y relación de aspecto

### Configuración de audio heredada

La configuración de audio se maneja a través de la propiedad `Audio` heredada de tipo M4AOutput, que incluye:

Codificación de audio AAC con configurables:

- Versión (predeterminado: MPEG-4)
- Tipo de objeto (predeterminado: AAC-LC)
- Tasa de bits (predeterminado: 128 kbps)
- Formato de salida (predeterminado: RAW)

### Uso

#### Implementación básica

```csharp
// Crear instancia de VideoCaptureCore
var core = new VideoCaptureCore();

// Establecer nombre de archivo de salida
core.Output_Filename = "output.ts";

// Crear salida MPEG-TS
var mpegtsOutput = new MPEGTSOutput();

// Configurar ajustes de video
mpegtsOutput.Video.Codec = MFVideoEncoder.MS_H264;
mpegtsOutput.Video.AvgBitrate = 2000; // 2 Mbps
mpegtsOutput.Video.RateControl = MFCommonRateControlMode.CBR;

// Configurar ajustes de audio
mpegtsOutput.Audio.Bitrate = 128; // 128 kbps
mpegtsOutput.Audio.Version = AACVersion.MPEG4;

core.Output_Format = mpegtsOutput;
```

#### Soporte de serialización

La clase proporciona soporte de serialización JSON integrado para guardar y cargar configuraciones:

```csharp
// Guardar configuración
string jsonConfig = mpegtsOutput.Save();

// Cargar configuración
MPEGTSOutput loadedConfig = MPEGTSOutput.Load(jsonConfig);
```

### Configuración predeterminada

La clase `MPEGTSOutput` se inicializa con estos ajustes predeterminados:

#### Predeterminados de video (heredados de MFBaseOutput)

- Tasa de bits promedio: 2000 kbps
- Códec: Microsoft H.264
- Perfil: Main
- Nivel: 4.2
- Control de tasa: CBR
- Calidad vs Velocidad: 85
- Cuadros de referencia máximos: 2
- Tamaño GOP: 50 cuadros
- Conteo de B-Pictures: 0
- Modo de baja latencia: Deshabilitado
- CABAC: Deshabilitado
- Modo de entrelazado: Progresivo

#### Predeterminados de audio

- Tasa de bits: 128 kbps
- Versión AAC: MPEG-4
- Tipo de objeto AAC: Complejidad baja (LC)
- Formato de salida: RAW

### Mejores prácticas

1. **Configuración de tasa de bits**:
   - Para aplicaciones de streaming, asegúrese de que las tasas de bits combinadas de video y audio estén dentro de su ancho de banda objetivo
   - Considere usar VBR para escenarios de almacenamiento y CBR para streaming

2. **Aceleración de hardware**:
   - Cuando esté disponible, use codificadores acelerados por hardware (QSV, NVENC, AMD) para mejor rendimiento
   - Recurra a MS_H264/MS_H265 cuando la aceleración de hardware no esté disponible

3. **Optimización de calidad**:
   - Para mayor calidad a costa del rendimiento, aumente el valor de `QualityVsSpeed`
   - Habilite CABAC para mejor eficiencia de compresión en escenarios sin baja latencia
   - Ajuste `MaxKeyFrameSpacing` según su caso de uso específico (valores más bajos para mejor búsqueda, valores más altos para mejor compresión)

### Notas técnicas

1. **Características de MPEG-TS**:
   - Adecuado para aplicaciones de streaming y transmisión
   - Proporciona resiliencia a errores a través de estructura basada en paquetes
   - Soporta múltiples programas y flujos elementales

2. **Consideraciones de rendimiento**:
   - El modo de baja latencia intercambia calidad por retardo de codificación reducido
   - Los B-frames mejoran la compresión pero aumentan la latencia
   - La aceleración de hardware puede reducir significativamente el uso de CPU

### Redists requeridos

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.