---
title: Guía de Integración del Codificador de Audio FLAC
description: Implemente compresión de audio sin pérdida FLAC en .NET con configuraciones de calidad, parámetros de compresión y procesamiento de audio de alta calidad.
---

# Codificador y salida FLAC

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El codificador FLAC (Free Lossless Audio Codec) proporciona compresión de audio sin pérdida de alta calidad mientras preserva la calidad de audio original.

## Salida FLAC multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Características

El codificador FLAC soporta una amplia gama de configuraciones de audio:

- Tasas de muestreo desde 1 Hz hasta 655,350 Hz
- Hasta 8 canales de audio (mono a surround 7.1)
- Compresión sin pérdida con configuraciones de calidad ajustables
- Soporte de salida para streaming
- Tamaños de bloque configurables y parámetros de compresión

### Configuración de calidad

El codificador proporciona un parámetro de calidad que va de 0 a 9:

- 0: Compresión más rápida (menor uso de CPU)
- 1-7: Configuraciones de compresión equilibradas
- 8: Mayor compresión (mayor uso de CPU)
- 9: Compresión extrema (extremadamente intensivo en CPU)

La configuración de calidad predeterminada es 5, que ofrece un buen balance entre ratio de compresión y velocidad de procesamiento.

### Configuración básica

La clase multiplataforma [FLACEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.FLACEncoderSettings.html) ofrece opciones de configuración avanzadas:

```csharp
// Crear configuración del codificador FLAC con calidad predeterminada
var flacSettings = new FLACEncoderSettings
{
    // Nivel de compresión predeterminado
    Quality = 5,        

    // Tamaño de bloque de audio en muestras
    BlockSize = 4608,              

    // Habilitar soporte de streaming
    StreamableSubset = true,    

    // Habilitar procesamiento estéreo
    MidSideStereo = true          
};
```

### Configuración avanzada de compresión

```csharp
// Crear configuración del codificador FLAC con configuración avanzada
var advancedSettings = new FLACEncoderSettings
{
    // Configuración de predicción lineal
    // Orden LPC máximo para predicción
    MaxLPCOrder = 8,               
    // Precisión automática para coeficientes
    QlpCoeffPrecision = 0,        
    
    // Configuración de codificación residual
    MinResidualPartitionOrder = 3,
    MaxResidualPartitionOrder = 3,
    
    // Configuración de optimización de búsqueda
    // Deshabilitar búsqueda costosa de coeficientes
    ExhaustiveModelSearch = false, 
    // Deshabilitar búsqueda de precisión
    QlpCoeffPrecSearch = false,    
    // Deshabilitar búsqueda de código de escape
    EscapeCoding = false          
};
```

### Código de ejemplo

Agregar la salida FLAC a la instancia del núcleo Video Capture SDK:

```csharp
// Crear una instancia del núcleo Video Capture SDK
var core = new VideoCaptureCoreX();

// Crear una instancia de salida FLAC
var flacOutput = new FLACOutput("output.flac");

// Establecer la calidad del codificador FLAC
flacOutput.Audio.Quality = 5;

// Agregar la salida FLAC
core.Outputs_Add(flacOutput, true);
```

Establecer el formato de salida para la instancia del núcleo Video Edit SDK:

```csharp
// Crear una instancia del núcleo Video Edit SDK
 var core = new VideoEditCoreX();

// Crear una instancia de salida FLAC
 var flacOutput = new FLACOutput("output.flac");

 // Establecer la calidad 
 flacOutput.Audio.Quality = 5;

 // Establecer el formato de salida
 core.Output_Format = flacOutput;
```

Crear una instancia de salida FLAC de Media Blocks:

```csharp
// Crear una instancia de configuración del codificador FLAC
var flacSettings = new FLACEncoderSettings();

// Crear una instancia de salida FLAC
var flacOutput = new FLACOutputBlock("output.flac", flacSettings);
```

### Clase FLACOutput

La clase `FLACOutput` proporciona funcionalidad para configurar la salida FLAC (Free Lossless Audio Codec) en los SDK de VisioForge.

```csharp
// Crear una nueva instancia de salida FLAC
var flacOutput = new FLACOutput("output.flac");

// Configurar ajustes del codificador FLAC
flacOutput.Audio.CompressionLevel = 5; // Ejemplo de configuración
```

#### Filename

- Establecer el nombre de archivo de salida durante la inicialización o usando la propiedad
- También se puede acceder/modificar usando los métodos `GetFilename()` y `SetFilename()`

```csharp
// Establecer durante la inicialización
var flacOutput = new FLACOutput("audio_output.flac");
```

```csharp
// O usando la propiedad
flacOutput.Filename = "new_output.flac";
```

#### Configuración de audio

La propiedad `Audio` proporciona acceso a configuraciones de codificación específicas de FLAC a través de la clase `FLACEncoderSettings`:

```csharp
flacOutput.Audio = new FLACEncoderSettings();
// Configure parámetros de codificación FLAC específicos aquí
```

#### Procesamiento de audio personalizado

Puede establecer un procesador de audio personalizado usando la propiedad `CustomAudioProcessor`:

```csharp
flacOutput.CustomAudioProcessor = new CustomMediaBlock();
```

#### Notas de implementación

- La clase implementa múltiples interfaces:
  - `IVideoEditXBaseOutput`
  - `IVideoCaptureXBaseOutput`
  - `IOutputAudioProcessor`
  
- Solo se soporta codificación de audio FLAC (sin capacidades de codificación de video)
- La configuración predeterminada del codificador FLAC se crea automáticamente durante la inicialización

El SDK Media Blocks contiene un [bloque de codificador FLAC](../../mediablocks/AudioEncoders/index.md) dedicado.

### Consideraciones de rendimiento

Al configurar el codificador FLAC, considere estos factores de rendimiento:

1. Configuraciones de calidad más altas (7-9) aumentarán significativamente el uso de CPU
2. La opción `ExhaustiveModelSearch` puede impactar grandemente la velocidad de codificación
3. Tamaños de bloque más grandes pueden mejorar la compresión pero aumentan el uso de memoria
4. `StreamableSubset` debe permanecer habilitado a menos que tenga requisitos específicos

### Compatibilidad

El codificador soporta las siguientes configuraciones:

- Canales de audio: 1 a 8 canales
- Tasas de muestreo: 1 Hz a 655,350 Hz
- Tasa de bits: Variable (compresión sin pérdida)

### Manejo de errores

Siempre verifique la disponibilidad del codificador antes de usar:

```csharp
if (!FLACEncoderSettings.IsAvailable())
{
    // Manejar escenario de codificador no disponible
    Console.WriteLine("El codificador FLAC no está disponible en este sistema");
    return;
}
```

### Mejores prácticas

1. Comience con la configuración de calidad predeterminada (5) y ajuste según sus necesidades
2. Habilite `MidSideStereo` para contenido estéreo para mejorar la compresión
3. Use `SeekPoints` para archivos de audio más largos para habilitar búsqueda rápida
4. Mantenga `StreamableSubset` habilitado a menos que tenga requisitos específicos
5. Evite usar `ExhaustiveModelSearch` a menos que el ratio de compresión sea crítico

## Salida FLAC solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La clase [FLACOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.FLACOutput.html) proporciona configuraciones solo para Windows para el codificador FLAC. Esta clase implementa tanto las interfaces `IVideoEditBaseOutput` como `IVideoCaptureBaseOutput`, haciéndola adecuada tanto para escenarios de edición como de captura de video.

### Propiedades

#### Nivel de compresión

- **Propiedad**: `Level`
- **Tipo**: `int`
- **Rango**: 0-8
- **Predeterminado**: 5
- **Descripción**: Controla el nivel de compresión, donde 0 proporciona la compresión más rápida y 8 proporciona la mayor compresión.

#### Tamaño de bloque

- **Propiedad**: `BlockSize`
- **Tipo**: `int`
- **Predeterminado**: 4608
- **Valores válidos**: Para flujos subset, debe ser uno de:
  - 192, 256, 512, 576, 1024, 1152, 2048, 2304, 4096, 4608
  - 8192, 16384 (solo si la tasa de muestreo > 48kHz)
- **Descripción**: Especifica el tamaño del bloque en muestras. El codificador usa el mismo tamaño de bloque para todo el flujo.

#### Orden LPC

- **Propiedad**: `LPCOrder`
- **Tipo**: `int`
- **Predeterminado**: 8
- **Restricciones**:
  - Debe ser ≤ 32
  - Para flujos subset a ≤ 48kHz, debe ser ≤ 12
- **Descripción**: Especifica el orden máximo de codificación predictiva lineal. Establecer a 0 deshabilita la predicción lineal genérica y usa solo predictores fijos, lo cual es más rápido pero típicamente resulta en archivos 5-10% más grandes.

#### Opciones de codificación Mid-Side

##### Codificación Mid-Side

- **Propiedad**: `MidSideCoding`
- **Tipo**: `bool`
- **Predeterminado**: `false`
- **Descripción**: Habilita la codificación mid-side para flujos estéreo. Esto típicamente aumenta la compresión en algunos puntos porcentuales al codificar tanto el par estéreo como las versiones mid-side de cada bloque y seleccionar la trama resultante más pequeña.

##### Codificación Mid-Side adaptativa

- **Propiedad**: `AdaptiveMidSideCoding`
- **Tipo**: `bool`
- **Predeterminado**: `false`
- **Descripción**: Habilita la codificación mid-side adaptativa para flujos estéreo. Esto proporciona codificación más rápida que la codificación mid-side completa pero con compresión ligeramente menor al cambiar adaptativamente entre codificación independiente y mid-side.

#### Parámetros Rice

##### Rice mínimo

- **Propiedad**: `RiceMin`
- **Tipo**: `int`
- **Predeterminado**: 3
- **Descripción**: Establece el orden de partición residual mínimo. Funciona junto con RiceMax para controlar cómo se particiona la señal residual.

##### Rice máximo

- **Propiedad**: `RiceMax`
- **Tipo**: `int`
- **Predeterminado**: 3
- **Descripción**: Establece el orden de partición residual máximo. El residual se particiona en 2^min a 2^max piezas, cada una con su propio parámetro Rice. Las configuraciones óptimas típicamente dependen del tamaño del bloque, con mejores resultados cuando blocksize/(2^n)=128.

#### Opciones avanzadas

##### Búsqueda exhaustiva de modelo

- **Propiedad**: `ExhaustiveModelSearch`
- **Tipo**: `bool`
- **Predeterminado**: `false`
- **Descripción**: Habilita la búsqueda exhaustiva de modelo para codificación óptima. Cuando está habilitado, el codificador genera subtramas para cada orden y usa la más pequeña, potencialmente mejorando la compresión en ~0.5% a costa de tiempo de codificación significativamente aumentado.

### Métodos

#### Constructor

```csharp
public FLACOutput()
```

Inicializa una nueva instancia con valores predeterminados:

- Level = 5
- RiceMin = 3
- RiceMax = 3
- LPCOrder = 8
- BlockSize = 4608

### Serialización

#### Save()

```csharp
public string Save()
```

Serializa la configuración a una cadena JSON.

#### Load(string json)

```csharp
public static FLACOutput Load(string json)
```

Crea una nueva instancia FLACOutput desde una cadena JSON.

### Ejemplo de uso

```csharp
var flacSettings = new FLACOutput
{
    Level = 8,                   // Compresión máxima
    BlockSize = 4608,            // Tamaño de bloque predeterminado
    MidSideCoding = true,        // Habilitar codificación mid-side para mejor compresión
    ExhaustiveModelSearch = true // Habilitar búsqueda exhaustiva para mejor compresión
};

core.Output_Format = flacSettings; // Core es VideoCaptureCore o VideoEditCore
```

### Mejores prácticas

#### Selección de nivel de compresión

- Use Level 0-3 para codificación más rápida con compresión moderada
- Use Level 4-6 para compresión/velocidad equilibrada
- Use Level 7-8 para máxima compresión sin importar la velocidad

#### Consideraciones de tamaño de bloque

- Tamaños de bloque más grandes generalmente proporcionan mejor compresión
- Manténgase en valores estándar (4608, 4096, etc.) para máxima compatibilidad
- Considere restricciones de memoria al seleccionar tamaño de bloque

#### Codificación Mid-Side

- Habilite para contenido estéreo cuando la compresión es prioridad
- Use modo adaptativo cuando la velocidad de codificación es importante
- Deshabilite para contenido mono ya que no tiene efecto

#### Parámetros Rice

- Los valores predeterminados (3,3) son adecuados para la mayoría de casos de uso
- Aumente para potencialmente mejor compresión a costa de velocidad de codificación
- Valores más allá de 6 raramente proporcionan beneficios significativos
