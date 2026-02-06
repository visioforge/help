---
title: Guía de Implementación del Codificador de Audio AAC
description: Implemente codificación de audio AAC en .NET con múltiples tipos de codificadores, configuraciones de tasa de bits y soporte de salida M4A multiplataforma.
---

# Codificador AAC y salida M4A

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El SDK de VisioForge proporciona varias implementaciones de codificadores AAC, cada una con características únicas y casos de uso.

## ¿Qué es la salida M4A?

M4A es un formato de archivo utilizado para almacenar datos de audio codificados con el códec Advanced Audio Coding (AAC). Los SDK de VisioForge .Net proporcionan soporte robusto para crear archivos de audio M4A de alta calidad a través de su clase dedicada M4AOutput. Este formato es ampliamente utilizado para la distribución de audio digital debido a su excelente eficiencia de compresión y calidad de sonido.

## Salida M4A (AAC) multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Los SDK con capacidad multiplataforma (VideoCaptureCoreX, VideoEditCoreX, MediaBlocksPipeline) permiten utilizar varias implementaciones de codificadores AAC a través de `M4AOutput`. Esta guía se centra en tres enfoques principales utilizando objetos de configuración dedicados:

1. [Codificador AVENC AAC](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.AVENCAACEncoderSettings.html) - Un codificador multiplataforma con muchas características.
2. [Codificador VO-AAC](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.VOAACEncoderSettings.html) - Un codificador multiplataforma simplificado.
3. Codificador AAC de Media Foundation - Un codificador de sistema específico de Windows, accesible en plataformas Windows a través de `MFAACEncoderSettings`.

### Codificador AVENC AAC

El codificador AVENC AAC ofrece las opciones de configuración más completas para la codificación de audio. Proporciona configuraciones avanzadas para codificación estéreo, predicción y modelado de ruido.

#### Características principales

- Múltiples estrategias de codificador
- Codificación estéreo configurable
- Técnicas avanzadas de ruido y predicción

#### Estrategias de codificador

El codificador AVENC AAC soporta tres estrategias de codificador:

- `ANMR`: Método avanzado de modelado y reducción de ruido
- `TwoLoop`: Método de búsqueda de dos bucles para optimización
- `Fast`: Algoritmo de búsqueda rápida predeterminado (recomendado para la mayoría de casos de uso)

#### Configuración de ejemplo

```csharp
var aacSettings = new AVENCAACEncoderSettings
{
    Coder = AVENCAACEncoderCoder.Fast,
    Bitrate = 192,
    IntensityStereo = true,
    ForceMS = true,
    TNS = true
};
```

#### Parámetros soportados

- **Tasas de bits**: 0, 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Tasas de muestreo**: 7350 a 96000 Hz
- **Canales**: 1 a 6 canales

### Codificador VO-AAC

El codificador VO-AAC es un codificador más simplificado con opciones de configuración más simples.

#### Características principales

- Configuración simplificada
- Controles directos de tasa de bits y tasa de muestreo
- Limitado a audio estéreo

#### Configuración de ejemplo

```csharp
var aacSettings = new VOAACEncoderSettings
{
    Bitrate = 128
};
```

#### Parámetros soportados

- **Tasas de bits**: 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Tasas de muestreo**: 8000 a 96000 Hz
- **Canales**: 1-2 canales

### Codificador AAC de Media Foundation (solo Windows)

Este codificador es específico para plataformas Windows y ofrece una solución de codificación limitada pero optimizada para rendimiento.

#### Características principales

- Implementación específica de Windows
- Opciones de tasa de bits predefinidas
- Soporte limitado de tasa de muestreo

#### Parámetros soportados

- **Tasas de bits**: 0 (Automático), 96, 128, 160, 192, 576, 768, 960, 1152 kbps
- **Tasas de muestreo**: 44100, 48000 Hz
- **Canales**: 1, 2, 6 canales

### Disponibilidad y selección de codificador

Cada codificador proporciona un método estático `IsAvailable()` para verificar si el codificador puede usarse en el entorno actual. Esto es útil para verificaciones de compatibilidad en tiempo de ejecución.

```csharp
if (AVENCAACEncoderSettings.IsAvailable())
{
    // Usar codificador AVENC AAC
}
else if (VOAACEncoderSettings.IsAvailable())
{
    // Alternativa al codificador VO-AAC
}
```

### Comenzando con M4AOutput

La implementación multiplataforma usa la clase [M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.M4AOutput.html) como base para la creación de archivos M4A. Para comenzar a usar esta característica, inicialice la clase con el nombre de archivo de salida deseado:

```csharp
var output = new M4AOutput("output.m4a");
```

### Cambio entre codificadores

La selección predeterminada del codificador depende de la plataforma:

- Entornos Windows: MF AAC
- Otras plataformas: VO-AAC

Puede anular esta selección predeterminada estableciendo explícitamente la propiedad `Audio`:

```csharp
// Para codificador VO-AAC
output.Audio = new VOAACEncoderSettings();

// Para codificador AVENC AAC
output.Audio = new AVENCAACEncoderSettings();

// Para codificador MF AAC (solo Windows)
#if NET_WINDOWS
output.Audio = new MFAACEncoderSettings();
#endif
```

### Configuración de ajustes del sink MP4

Dado que los archivos M4A se basan en el formato de contenedor MP4, puede ajustar varios parámetros de salida a través de la propiedad `Sink`:

```csharp
// Cambiar el nombre de archivo de salida
output.Sink.Filename = "new_output.m4a";
```

### Procesamiento de audio avanzado

Para flujos de trabajo que requieren procesamiento de audio especializado, la clase M4AOutput soporta procesadores de audio personalizados:

```csharp
// Implemente su lógica de procesamiento de audio personalizada
output.CustomAudioProcessor = new MyCustomAudioProcessor(); 
```

### Métodos clave para gestión de archivos

La clase M4AOutput proporciona varios métodos para manejar archivos y recuperar información del codificador:

```csharp
// Obtener nombre de archivo de salida actual
string currentFile = output.GetFilename();

// Actualizar el nombre de archivo de salida
output.SetFilename("updated_file.m4a");

// Recuperar codificadores de audio disponibles
var audioEncoders = output.GetAudioEncoders();
```

### Uso de salida M4A en diferentes SDK

Cada SDK de VisioForge tiene un enfoque ligeramente diferente para implementar la salida M4A:

#### Con Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

#### Con Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

#### Con Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var sinkSettings = new MP4SinkSettings("output.m4a");
var m4aOutput = new M4AOutputBlock(sinkSettings, aac);
```

### Consideraciones de control de tasa

1. **Codificador AVENC AAC**:
   - Control de tasa más flexible
   - Soporta tasa de bits constante (CBR)
   - Múltiples estrategias de codificación afectan la calidad y el rendimiento

2. **Codificador VO-AAC**:
   - Control simple de tasa de bits constante
   - Recomendado para necesidades de codificación simples
   - Configuración avanzada limitada

3. **Codificador Media Foundation**:
   - Limitado a tasas de bits predefinidas
   - Bueno para codificación rápida basada en Windows
   - Opción de tasa de bits automática disponible

### Recomendaciones

- Para codificación de audio avanzada con máximo control, use el codificador AVENC AAC
- Para codificación simple multiplataforma, use el codificador VO-AAC
- Para codificación optimizada específica de Windows, use el codificador Media Foundation

### Consideraciones de rendimiento y calidad

- **Tasa de bits vs. Calidad vs. Tamaño de archivo**: Tasas de bits más altas generalmente resultan en mejor calidad de audio pero también en archivos más grandes. Experimente con diferentes tasas de bits para encontrar el balance óptimo para su contenido específico y necesidades de distribución.
- **Coincidencia de tasa de muestreo**: Siempre intente elegir tasas de muestreo que coincidan con su audio fuente. Esto evita el remuestreo innecesario, que puede potencialmente degradar la calidad de audio.
- **Características del codificador**:
  - `Codificador AVENC AAC`: Ofrece las opciones de configuración más extensas, permitiendo control detallado sobre calidad y rendimiento. Ideal para casos de uso avanzados.
  - `Codificador VO-AAC`: Proporciona un buen balance de simplicidad, compatibilidad multiplataforma y calidad. Una opción sólida para muchos escenarios comunes.
  - `Codificador AAC de Media Foundation`: Aprovecha las capacidades de procesamiento de audio integradas de Windows. Puede ser eficiente en Windows pero ofrece menos flexibilidad de configuración que AVENC.
- **Configuración de canales (Mono vs. Estéreo)**:
  - Para contenido solo de voz, usar codificación mono (1 canal) puede reducir significativamente el tamaño del archivo sin pérdida notable de calidad para el habla. Verifique si los ajustes de codificador elegidos (ej., `AVENCAACEncoderSettings.Channels`) permiten configuración explícita de canales.
  - Para música y entornos de audio ricos, el estéreo (2 canales) generalmente es preferido.
- **Rangos de tasa de bits específicos por contenido**: Mientras que más alto es a menudo mejor, la "mejor" tasa de bits depende del contenido de audio:
  - *Habla/Voz:* 64-96 kbps puede ser adecuado.
  - *Música general:* 128-192 kbps es un objetivo común para buena calidad.
  - *Audio de alta fidelidad:* 256-320 kbps o más pueden usarse cuando la calidad prístina es crítica.
    Estas son guías; siempre pruebe con su audio específico.
- **Audiencia y plataforma objetivo**: Considere quién escuchará y en qué dispositivos. Por ejemplo, si el audio es principalmente para streaming web a dispositivos móviles, tasas de bits extremadamente altas podrían llevar a problemas de buffering o consumo de datos innecesario. Adapte su elección de codificador y configuración en consecuencia.

### Código de ejemplo

- Consulte la guía de [salida MP4](../output-formats/mp4.md) para código de ejemplo.
- Consulte el [bloque de codificador AAC](../../mediablocks/AudioEncoders/index.md) para código de ejemplo.

## Salida AAC solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

[M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.M4AOutput.html) es la clase principal para configurar ajustes de salida M4A (AAC). Implementa tanto las interfaces `IVideoEditBaseOutput` como `IVideoCaptureBaseOutput`.

### Propiedades

| Propiedad | Tipo | Descripción | Valor predeterminado |
|-----------|------|-------------|----------------------|
| Version | AACVersion | Especifica la versión AAC (MPEG-2 o MPEG-4) | MPEG4 |
| Object | AACObject | Define el tipo de objeto AAC | Low |
| Output | AACOutput | Establece el modo de salida AAC | RAW |
| Bitrate | int | Especifica la tasa de bits AAC en kbps | 128 |

### Métodos

#### `GetInternalTypeVC()`

- Retorna: `VideoCaptureOutputFormat.M4A`
- Propósito: Obtiene el formato de salida interno para captura de video

#### `GetInternalTypeVE()`

- Retorna: `VideoEditOutputFormat.M4A`
- Propósito: Obtiene el formato de salida interno para edición de video

#### `Save()`

- Retorna: Representación de cadena JSON del objeto M4AOutput
- Propósito: Serializa la configuración actual a JSON

#### `Load(string json)`

- Parámetros: Cadena JSON conteniendo configuración M4AOutput
- Retorna: Nueva instancia M4AOutput
- Propósito: Crea una nueva instancia M4AOutput desde configuración JSON

### Enumeraciones de soporte

#### AACVersion

Define la versión de AAC a usar:

| Valor | Descripción |
|-------|-------------|
| MPEG4 | AAC MPEG-4 (predeterminado) |
| MPEG2 | AAC MPEG-2 |

#### AACObject

Especifica el tipo de objeto de flujo del codificador AAC:

| Valor | Descripción |
|-------|-------------|
| Undefined | No debe usarse |
| Main | Perfil principal |
| Low | Perfil de baja complejidad (predeterminado) |
| SSR | Perfil de tasa de muestreo escalable |
| LTP | Perfil de predicción a largo plazo |

#### AACOutput

Determina el tipo de salida del flujo del codificador AAC:

| Valor | Descripción |
|-------|-------------|
| RAW | Flujo AAC crudo (predeterminado) |
| ADTS | Formato de flujo de transporte de datos de audio |

### Ejemplo de uso

```csharp
// Crear nueva configuración de salida M4A
var core = new VideoCaptureCore();
core.Mode = VideoCaptureMode.VideoCapture;
core.Output_Filename = "output.m4a";

var output = new M4AOutput
{
    Bitrate = 192,
    Version = AACVersion.MPEG4,
    Object = AACObject.Low,
    Output = AACOutput.ADTS
};

core.Output_Format = output; // core es una instancia de VideoCaptureCore o VideoEditCore
```

### Selección de la tasa de bits correcta

La tasa de bits óptima depende de su tipo de contenido y requisitos de calidad:

- **64-96 kbps**: Adecuado para grabaciones de voz y contenido hablado
- **128-192 kbps**: Recomendado para música general y contenido de audio
- **256-320 kbps**: Ideal para música de alta fidelidad donde la calidad es primordial

### Elección del perfil apropiado

- Use `AACObject.Low` para la mayoría de aplicaciones ya que proporciona un excelente balance entre calidad y eficiencia de codificación
- Reserve `AACObject.Main` para casos de uso especializados que requieren máxima calidad
- Evite `AACObject.Undefined` ya que no es una opción de codificación válida

### Selección del formato de contenedor

- `AACOutput.ADTS` proporciona mejor compatibilidad con varios reproductores y dispositivos
- `AACOutput.RAW` es preferible cuando el flujo AAC será incrustado dentro de otro formato de contenedor
