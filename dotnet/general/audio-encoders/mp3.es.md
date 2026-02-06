---
title: Grabar, Capturar y Editar Audio MP3 en C#
description: Grabe, capture y edite audio MP3 en C# con el SDK .NET de VisioForge usando el codificador LAME para procesamiento de audio de alta calidad.
---

# Dominando el Audio MP3: Grabar, Capturar y Editar en C# y .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

El SDK de VisioForge permite a los desarrolladores grabar, capturar y editar audio MP3 de forma fluida dentro de aplicaciones C#. Esta guía explora cómo aprovechar nuestro robusto SDK .NET para el procesamiento de audio MP3 de alta calidad. Ya sea que necesite capturar flujos multimedia, grabar archivos MP3 o editar formas de onda de audio, nuestro kit de herramientas multimedia C# proporciona herramientas completas usando la biblioteca LAME. MP3, un formato de compresión de audio con pérdida ampliamente adoptado, es ideal para streaming de audio y almacenamiento eficiente.

Puede utilizar el codificador MP3 para integrar funcionalidades de captura y grabación de audio en varios formatos de contenedor como MP4, AVI y MKV, mejorando sus proyectos de captura de audio. Nuestro SDK funciona perfectamente con Visual Studio para una experiencia de desarrollo fluida.

El SDK contiene un codificador de audio MP3 que se puede usar para codificar flujos de audio al formato MP3 usando la biblioteca LAME. MP3 es un formato de compresión de audio con pérdida que se usa ampliamente en streaming y almacenamiento de audio.

Puede usar la codificación MP3 para codificar audio en contenedores MP4, AVI, MKV y otros.

## Captura y grabación de audio MP3 multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

La clase [MP3EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.MP3EncoderSettings.html) proporciona a los desarrolladores un enfoque simplificado para configurar la codificación MP3 para proyectos de captura de audio en C#. Esta solución multiplataforma soporta varios controles de tasa y configuraciones de calidad, siendo ideal para aplicaciones de grabación MP3 en .NET en diferentes sistemas operativos.

### Formatos y especificaciones soportados para grabación MP3 en C#

- Formato de entrada: S16LE (Signed 16-bit Little Endian)
- Tasas de muestreo: 8000, 11025, 12000, 16000, 22050, 24000, 32000, 44100, 48000 Hz
- Canales: Mono (1) o Estéreo (2)

### Modos de control de tasa

El codificador soporta tres modos de control de tasa:

1. **CBR (Tasa de bits constante)**
   - Tasa de bits fija durante todo el proceso de codificación
   - Tasas de bits soportadas: 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 Kbit/s
   - Mejor para streaming MP3 y cuando el tamaño de archivo consistente es importante

2. **ABR (Tasa de bits promedio)**
   - Mantiene una tasa de bits promedio mientras permite cierta variación
   - Más eficiente que CBR mientras mantiene tamaños de archivo predecibles
   - Útil para servicios de streaming que necesitan estimaciones aproximadas de tamaño de archivo

3. **VBR basado en calidad**
   - Tasa de bits variable basada en la complejidad del sonido
   - Configuración de calidad que va de 0 (mejor) a 10
   - Más eficiente para almacenamiento y mejor relación calidad-tamaño

### Ejemplos de codificación MP3 en C#

Crear configuración básica del codificador MP3 con CBR.

```csharp
// Crear configuración básica del codificador MP3 usando modo de tasa de bits constante
var mp3Settings = new MP3EncoderSettings
{
    // Establecer a tasa de bits constante - proporciona tamaño de archivo consistente y fiabilidad de streaming
    RateControl = MP3EncoderRateControl.CBR,
    // 192 kbps ofrece buena calidad para la mayoría del contenido musical manteniendo el tamaño de archivo razonable
    Bitrate = 192,
    // Calidad estándar ofrece un buen balance entre velocidad de codificación y calidad de salida
    EncodingEngineQuality = MP3EncodingQuality.Standard,
    // Mantener canales estéreo (false) - establecer a true si desea convertir a mono
    ForceMono = false
};
```

Configuración VBR basada en calidad para edición MP3 en .NET de alta calidad.

```csharp
// Configurar codificador MP3 con tasa de bits variable para relación óptima calidad-tamaño
var vbrSettings = new MP3EncoderSettings
{
    // VBR basado en calidad ajusta la tasa de bits dinámicamente según la complejidad del audio
    RateControl = MP3EncoderRateControl.Quality,
    // Escala de calidad: 0 (mejor) a 10 (peor) - 2.0 proporciona excelente calidad con tamaño de archivo razonable
    Quality = 2.0f,
    // Codificación de alta calidad usa más CPU pero produce mejores resultados
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

Agregar la salida MP3 para capturar audio MP3 en C# con el Video Capture SDK:

La clase [MP3Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) implementa múltiples interfaces:

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- IOutputAudioProcessor

```csharp
// Crear una instancia del núcleo Video Capture SDK para grabación
var core = new VideoCaptureCoreX();

// Inicializar salida MP3 con nombre de archivo destino
var mp3Output = new MP3Output("output.mp3");

// Configurar ajustes de codificación de audio
mp3Output.Audio.RateControl = MP3EncoderRateControl.CBR;  // Usar tasa de bits constante para streaming fiable
mp3Output.Audio.Bitrate = 128;  // 128 kbps es adecuado para grabación de audio general

// Agregar la salida MP3 al pipeline de captura
core.Outputs_Add(mp3Output, true);
```

Establecer el formato de salida para la instancia del núcleo Video Edit SDK:

```csharp
// Inicializar Video Edit SDK para procesar medios existentes
var core = new VideoEditCoreX();

// Crear salida MP3 con nombre de archivo destino
var mp3Output = new MP3Output("output.mp3");

// Configurar codificación de tasa de bits variable para mejor relación calidad-tamaño
mp3Output.Audio.RateControl = MP3EncoderRateControl.Quality;
mp3Output.Audio.Quality = 5.0f;  // Configuración de calidad media (escala 0-10) - buen balance de calidad y tamaño

// Establecer como formato de salida principal para el editor
core.Output_Format = mp3Output;
```

### Inicialización

Para crear una nueva instancia MP3Output, necesita proporcionar el nombre de archivo de salida:

```csharp
// Inicializar salida MP3 con nombre de archivo destino
var mp3Output = new MP3Output("output.mp3");
```

### Configuración de audio

La propiedad `Audio` proporciona acceso a los ajustes del codificador MP3:

```csharp
// Crear objeto de configuración predeterminado del codificador MP3
mp3Output.Audio = new MP3EncoderSettings();
// Se puede aplicar configuración adicional a las propiedades de mp3Output.Audio
```

### Procesamiento de audio personalizado

Puede establecer un procesador de audio personalizado usando la propiedad `CustomAudioProcessor` para manejar manipulaciones de forma de onda:

```csharp
// Adjuntar un procesador de audio personalizado para manipulación de audio avanzada
mp3Output.CustomAudioProcessor = new MediaBlock();
// El MediaBlock puede configurarse para efectos, filtrado u otro procesamiento de audio
```

### Operaciones de nombre de archivo

Hay múltiples formas de trabajar con el nombre de archivo de salida:

```csharp
// Recuperar el nombre de archivo de salida actual
string currentFile = mp3Output.GetFilename();

// Cambiar el destino de salida
mp3Output.SetFilename("newoutput.mp3");

// Forma alternativa de establecer el nombre de archivo mediante propiedad
mp3Output.Filename = "another.mp3";
```

### Codificadores de audio

La clase MP3Output soporta codificación MP3 exclusivamente. Puede verificar los codificadores disponibles:

```csharp
// Obtener información sobre codificadores de audio disponibles
var audioEncoders = mp3Output.GetAudioEncoders();
// Devuelve una lista de tuplas conteniendo nombres de codificadores y sus tipos de configuración
// Para MP3Output, esto contendrá una sola entrada para MP3
```

### Clase MP3OutputBlock

La clase [MP3OutputBlock](../../mediablocks/AudioEncoders/index.md) proporciona una forma más flexible de configurar la codificación MP3.

Crear una instancia de salida MP3 de Media Blocks:

```csharp
// Crear configuración del codificador MP3 con configuración deseada
var mp3Settings = new MP3EncoderSettings();

// Inicializar bloque de salida MP3 con archivo destino y configuración
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
```

Verificar si la codificación MP3 está disponible.

```cs
// Verificar si la codificación MP3 está disponible en el sistema actual
if (!MP3EncoderSettings.IsAvailable())
{
   // Manejar caso donde la codificación MP3 no está disponible
   // Esto podría ocurrir si LAME u otras bibliotecas requeridas faltan
}
```

### Niveles de calidad de codificación

El codificador soporta tres preajustes de calidad que afectan la velocidad de codificación y el uso de CPU:

- `Fast`: Codificación más rápida, menor uso de CPU
- `Standard`: Velocidad y calidad equilibradas (predeterminado)
- `High`: Mejor calidad, mayor uso de CPU

### Escenarios comunes

#### Captura de música de alta calidad en C#

```csharp
// Configurar ajustes para grabación de música de alta calidad
var highQualitySettings = new MP3EncoderSettings
{
    // Usar tasa de bits variable basada en calidad para fidelidad de audio óptima
    RateControl = MP3EncoderRateControl.Quality,
    // Configuración de máxima calidad (0.0f) para máxima fidelidad de audio
    Quality = 0.0f,
    // Usar algoritmo de codificación de alta calidad (más intensivo en CPU pero mejores resultados)
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

#### Audio en streaming

```csharp
// Configurar ajustes optimizados para aplicaciones de streaming de audio
var streamingSettings = new MP3EncoderSettings
{
    // Usar tasa de bits constante para rendimiento de streaming predecible
    RateControl = MP3EncoderRateControl.CBR,
    // 128 kbps proporciona buena calidad para la mayoría del contenido siendo amigable con el ancho de banda
    Bitrate = 128,
    // Codificación rápida reduce el uso de CPU, importante para streaming en tiempo real
    EncodingEngineQuality = MP3EncodingQuality.Fast
};
```

## Salida MP3 solo Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

La clase [salida de archivo MP3](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) proporciona opciones de configuración avanzadas para codificación MP3 en escenarios de captura y edición de audio video en C#.

### Características principales

- Selección flexible de modo de canal
- Soporte de codificación VBR y CBR para grabación MP3 óptima en .NET
- Parámetros de codificación avanzados para aplicaciones de audio profesionales
- Configuraciones de control de calidad para resultados perfectos de edición MP3 en C#

### Configuración básica

#### CBR_Bitrate

Controla la configuración de tasa de bits constante (CBR) para codificación MP3.

- Para MPEG-1 (32, 44.1, 48 kHz): Valores válidos son 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 kbps
- Para MPEG-2 (16, 22.05, 24 kHz): Valores válidos son 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 kbps
- Valores predeterminados: 128 kbps (MPEG-1) o 64 kbps (MPEG-2)

#### SampleRate

Especifica la frecuencia de muestreo de audio en Hz. Valores comunes son:

- 44100 Hz (calidad CD, predeterminado)
- 48000 Hz (audio profesional)
- 32000 Hz (transmisión)
- 22050 Hz (calidad inferior)
- 16000 Hz (voz)

#### ChannelsMode

Determina cómo se codifican los canales de audio. Las opciones incluyen:

1. StandardStereo: Codificación de canales independientes con asignación dinámica de bits
2. JointStereo: Explota la correlación entre canales usando codificación mid/side
3. DualStereo: Codificación independiente con asignación fija 50/50 de bits (ideal para idioma dual)
4. Mono: Salida de canal único (mezcla entrada estéreo)

### Configuración de tasa de bits variable (VBR)

#### VBR_Mode

Habilita la codificación de tasa de bits variable cuando se establece a true (predeterminado). VBR permite al codificador ajustar la tasa de bits basándose en la complejidad del audio.

#### VBR_MinBitrate

Establece la tasa de bits mínima permitida para codificación VBR (predeterminado: 96 kbps).

#### VBR_MaxBitrate

Establece la tasa de bits máxima permitida para codificación VBR (predeterminado: 192 kbps).

#### VBR_Quality

Controla la calidad de codificación VBR (0-9):

- Valores más bajos (0-4): Mayor calidad, codificación más lenta
- Valores medios (5-6): Calidad y velocidad equilibradas
- Valores más altos (7-9): Menor calidad, codificación más rápida

### Calidad y rendimiento

#### EncodingQuality

Determina la calidad algorítmica de la codificación (0-9):

- 0-1: Mejor calidad, codificación más lenta
- 2: Recomendado para alta calidad
- 5: Predeterminado, buen balance de velocidad y calidad
- 7: Codificación rápida con calidad aceptable
- 9: Codificación más rápida, menor calidad

### Características especiales

#### ForceMono

Cuando está habilitado, mezcla automáticamente audio multicanal a mono.

#### VoiceEncodingMode

Modo experimental optimizado para contenido de voz.

#### KeepAllFrequencies

Deshabilita el filtrado automático de frecuencias, preservando todas las frecuencias a costa de eficiencia.

#### DisableShortBlocks

Fuerza el uso de solo bloques largos, lo que puede mejorar la calidad a tasas de bits muy bajas pero puede causar artefactos de pre-eco.

### Banderas de trama MP3

#### Copyright

Establece el bit de copyright en tramas MP3.

#### Original

Marca el flujo como contenido original.

#### CRCProtected

Habilita la detección de errores CRC a costa de 16 bits por trama.

#### EnableXingVBRTag

Agrega encabezados de información VBR para mejor compatibilidad con reproductores.

#### StrictISOCompliance

Impone cumplimiento estricto del estándar ISO MP3.

### Ejemplos de configuraciones de grabación y edición MP3

Configuración básica para aplicaciones de captura MP3 en C#.

```csharp
// Configurar salida MP3 básica con configuración estándar
var mp3Output = new MP3Output
{
    // 192 kbps proporciona buena calidad para la mayoría del contenido musical
    CBR_Bitrate = 192,
    // Tasa de muestreo de calidad CD
    SampleRate = 44100,
    // Modo estéreo conjunto proporciona mejor compresión para la mayoría del contenido estéreo
    ChannelsMode = MP3ChannelsMode.JointStereo,
};

// Establecer como formato de salida para captura o edición
core.Output_Format = mp3Output; // Core es VideoCaptureCore o VideoEditCore
```

Configuración VBR.

```csharp
// Configurar salida MP3 con tasa de bits variable para mejor balance calidad/tamaño
var mp3Output = new MP3Output
{    
    // Habilitar codificación de tasa de bits variable
    VBR_Mode = true,
    // Establecer piso de tasa de bits mínima para asegurar calidad aceptable
    VBR_MinBitrate = 96,
    // Limitar tasa de bits máxima para controlar tamaño de archivo
    VBR_MaxBitrate = 192,
    // Nivel de calidad 6 proporciona buen balance entre calidad y tamaño de archivo
    VBR_Quality = 6,
};

// Establecer como formato de salida para captura o edición
core.Output_Format = mp3Output; // Core es VideoCaptureCore o VideoEditCore
```

#### Codificación MP3 estéreo básica

```csharp
// Configurar codificación MP3 estéreo estándar con tasa de bits fija
var mp3Output = new MP3Output
{
    // 192 kbps proporciona buena calidad para la mayoría de la música manteniendo tamaño de archivo razonable
    CBR_Bitrate = 192,
    // Modo estéreo estándar codifica canales izquierdo y derecho independientemente
    ChannelsMode = MP3ChannelsMode.StandardStereo,
    // Tasa de muestreo de calidad CD
    SampleRate = 44100,
    // Deshabilitar tasa de bits variable para asegurar tamaño de archivo y reproducción consistentes
    VBR_Mode = false
};
```

#### Codificación optimizada para voz

```csharp
// Configurar ajustes MP3 optimizados para grabaciones de voz
var voiceMP3 = new MP3Output
{
    // Habilitar algoritmos de codificación optimizados para voz
    VoiceEncodingMode = true,
    // Usar mono para voz para reducir tamaño de archivo (la mayoría de voz no se beneficia del estéreo)
    ChannelsMode = MP3ChannelsMode.Mono,
    // Tasa de muestreo más baja es suficiente para contenido de voz
    SampleRate = 22050,
    // Habilitar tasa de bits variable para mejor relación calidad/tamaño
    VBR_Mode = true,
    // Mejor configuración de calidad para claridad de voz manteniendo tamaño de archivo razonable
    VBR_Quality = 4
};
```

#### Codificación de música de alta calidad

```csharp
// Configurar ajustes MP3 de alta calidad para archivo de música
var highQualityMP3 = new MP3Output
{
    // Habilitar tasa de bits variable para relación óptima calidad-tamaño
    VBR_Mode = true,
    // Establecer tasa de bits mínima para asegurar buena calidad incluso en pasajes simples
    VBR_MinBitrate = 128,
    // Permitir alta tasa de bits para pasajes complejos para preservar detalle de audio
    VBR_MaxBitrate = 320,
    // Usar configuración de alta calidad (2) para excelente fidelidad de audio
    VBR_Quality = 2,
    // Establecer algoritmo del codificador a modo de alta calidad
    EncodingQuality = 2,
    // Estéreo conjunto proporciona mejor compresión para la mayoría del contenido musical
    ChannelsMode = MP3ChannelsMode.JointStereo,
    // Tasa de muestreo de audio profesional captura espectro audible completo
    SampleRate = 48000,
    // Agregar encabezado VBR para mejor compatibilidad con reproductores y búsqueda
    EnableXingVBRTag = true
};
```

### Configuración avanzada

- **Protección CRC**: Agrega capacidad de detección de errores a costa de 16 bits por trama
- **Bloques cortos**: Pueden deshabilitarse para potencialmente aumentar calidad a tasas de bits muy bajas
- **Rango de frecuencia**: Opción para mantener todas las frecuencias (deshabilita filtrado pasa-bajo automático)
- **Modo voz**: Modo experimental optimizado para contenido de voz

### Mejores prácticas

1. **Elección de control de tasa para diferentes aplicaciones**
   - Use CBR para streaming y captura MP3 en tiempo real en C#
   - Use VBR basado en calidad para archivo y grabación MP3 de máxima calidad en .NET
   - Use ABR cuando necesite un balance entre tamaño consistente y calidad

2. **Configuración de calidad para diferentes casos de uso**
   - Para archivo: Use VBR con calidad 0-2
   - Para captura general de audio video en C#: VBR con calidad 3-5 o CBR 192-256kbps
   - Para grabación de voz en .NET: Considere usar modo de codificación de voz con tasas de bits más bajas

3. **Selección de modo de canal**
   - Use estéreo conjunto para la mayoría del contenido musical
   - Use estéreo estándar para escucha crítica y mezclas estéreo complejas
   - Use mono para grabaciones de voz o cuando el ancho de banda es crítico

4. **Optimización de rendimiento**
   - Use calidad de codificación rápida para aplicaciones en tiempo real
   - Use calidad estándar para codificación de propósito general
   - Use alta calidad solo para propósitos de archivo donde el tiempo de codificación no es crítico

### Notas sobre valores predeterminados

El constructor de la clase establece estos valores predeterminados:

- CBR_Bitrate = 192 kbps
- VBR_MinBitrate = 96 kbps
- VBR_MaxBitrate = 192 kbps
- VBR_Quality = 6
- EncodingQuality = 6
- SampleRate = 44100 Hz
- ChannelsMode = MP3ChannelsMode.StandardStereo
- VBR_Mode = true
