---
title: Integración del Codificador de Audio Speex para .NET
description: Implemente compresión de voz Speex en .NET con configuraciones de codificación de voz optimizadas, controles de calidad y captura de audio multiplataforma.
---

# Codificador de Audio Speex para .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a Speex

Speex es un códec de audio libre de patentes diseñado específicamente para codificación de voz en aplicaciones .NET. Ya sea que necesite capturar, editar o grabar audio en C#, Speex proporciona excelente compresión mientras mantiene la calidad de voz a través de varias tasas de bits. VisioForge integra este potente codificador en sus SDK .NET, ofreciendo a los desarrolladores opciones de configuración flexibles para aplicaciones basadas en voz. El códec es particularmente adecuado para desarrolladores C# que buscan implementar características de captura y grabación de audio de alta calidad en sus aplicaciones.

## Funcionalidad principal

El codificador Speex en los SDK de VisioForge soporta:

- Múltiples bandas de frecuencia para diferentes niveles de calidad
- Codificación de tasa de bits variable y fija
- Detección de actividad de voz y compresión de silencio
- Configuraciones ajustables de complejidad y calidad
- Compatibilidad multiplataforma en Windows, macOS y Linux
- Integración perfecta con aplicaciones dotnet

## Implementación multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Modos del codificador

Speex ofrece cuatro modos de operación optimizados para diferentes rangos de frecuencia:

| Modo | Valor | Tasa de muestreo óptima |
|------|-------|-------------------------|
| Auto | 0 | Selección automática basada en entrada |
| Banda ultra ancha | 1 | 32 kHz |
| Banda ancha | 2 | 16 kHz |
| Banda estrecha | 3 | 8 kHz |

El codificador ajusta automáticamente los parámetros internos basándose en el modo seleccionado. Para la mayoría de aplicaciones de voz, Banda Ancha (modo 2) ofrece un excelente balance entre calidad y uso de ancho de banda.

## Especificaciones técnicas

### Tasas de muestreo soportadas

Speex funciona con tres frecuencias de muestreo estándar:

- 8,000 Hz - Mejor para audio de calidad telefónica (Banda Estrecha)
- 16,000 Hz - Recomendado para la mayoría de aplicaciones de voz (Banda Ancha)
- 32,000 Hz - Codificación de voz de máxima calidad (Banda Ultra Ancha)

### Configuración de canales

El codificador maneja tanto:

- Mono (1 canal) - Ideal para grabaciones de voz
- Estéreo (2 canales) - Para audio inmersivo o de múltiples hablantes

## Métodos de control de tasa

### Codificación basada en calidad

Para calidad perceptual consistente, use el parámetro `Quality`:

```csharp
var settings = new SpeexEncoderSettings {
    Quality = 8.0f, // Rango de 0 (más bajo) a 10 (más alto)
    VBR = false     // Modo de calidad fija
};
```

Valores de calidad más altos producen mejor audio a expensas de mayor tamaño de archivo. La mayoría de aplicaciones de voz funcionan bien con valores de calidad entre 5-8.

### Tasa de bits variable (VBR)

VBR ajusta dinámicamente la tasa de bits basándose en la complejidad de la voz:

```csharp
var settings = new SpeexEncoderSettings {
    VBR = true,
    Quality = 8.0f  // Nivel de calidad objetivo
};
```

Este enfoque típicamente ahorra ancho de banda mientras mantiene calidad percibida consistente, haciéndolo ideal para aplicaciones de streaming.

### Tasa de bits promedio (ABR)

ABR mantiene una tasa de bits objetivo a lo largo del tiempo mientras permite fluctuaciones de calidad:

```csharp
var settings = new SpeexEncoderSettings {
    ABR = 15.0f,   // Tasa de bits objetivo en kbps
    VBR = true     // Requerido para modo ABR
};
```

Esta opción funciona bien cuando necesita tamaños de archivo predecibles o uso de ancho de banda.

### Codificación de tasa de bits fija

Para tasas de datos consistentes a lo largo del proceso de codificación:

```csharp
var settings = new SpeexEncoderSettings {
    Bitrate = 24.6f,  // Tasa fija en kbps
    VBR = false
};
```

Las tasas de bits soportadas van desde 2.15 kbps hasta 24.6 kbps:

- 2.15 kbps - Voz ultra comprimida (calidad limitada)
- 3.95 kbps - Voz de bajo ancho de banda
- 5.95 kbps - Claridad de voz básica
- 8.00 kbps - Calidad de voz estándar
- 11.0 kbps - Buena reproducción de voz
- 15.0 kbps - Voz casi transparente
- 18.2 kbps - Voz de alta calidad
- 24.6 kbps - Voz de máxima calidad

## Características de optimización de voz

### Detección de actividad de voz (VAD)

VAD identifica la presencia de voz en señales de audio:

```csharp
var settings = new SpeexEncoderSettings {
    VAD = true,    // Habilitar detección de voz
    DTX = true     // Recomendado con VAD
};
```

Esta característica mejora la eficiencia del ancho de banda al enfocar los recursos de codificación en segmentos de voz real.

### Transmisión discontinua (DTX)

DTX reduce la transmisión de datos durante períodos de silencio:

```csharp
var settings = new SpeexEncoderSettings {
    DTX = true     // Habilitar compresión de silencio
};
```

Para VoIP y comunicaciones en tiempo real, habilitar DTX puede reducir significativamente los requisitos de ancho de banda.

### Complejidad de codificación

Controle el uso de CPU versus calidad de codificación:

```csharp
var settings = new SpeexEncoderSettings {
    Complexity = 3  // Rango: 1 (más rápido) a 10 (mayor calidad)
};
```

Valores más bajos priorizan velocidad y reducen carga de CPU, mientras que valores más altos mejoran la calidad de audio a costa de rendimiento.

## Ejemplos de implementación

### Verificación de disponibilidad del codificador

Siempre verifique la disponibilidad del codificador antes de implementar Speex en su aplicación C#:

```csharp
if (!SpeexEncoderSettings.IsAvailable())
{
    throw new InvalidOperationException("El codificador Speex no está disponible en este sistema.");
}
```

### Configuración básica para captura de audio

Aquí está cómo configurar la codificación Speex básica para captura de audio en dotnet:

```csharp
var encoderSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    Quality = 7.0f
};
```

### Optimizado para grabación de voz

Para aplicaciones de grabación de voz en .NET, use estas configuraciones optimizadas:

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 4
};
```

### Captura de audio de máxima calidad

Para captura de audio de máxima calidad en dotnet:

```csharp
var highQualitySettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.UltraWideBand,
    SampleRate = 32000,
    Channels = 2,
    Bitrate = 24.6f,
    Complexity = 8
};
```

## Integración de SDK

### Integración con Video Capture SDK

Aprenda cómo capturar audio usando Speex en su aplicación C#:

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Crear una instancia del núcleo Video Capture SDK
var core = new VideoCaptureCoreX();

// Establecer el dispositivo de entrada de audio, filtrar por API
var api = AudioCaptureDeviceAPI.DirectSound;
var audioInputDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x => x.API == api);
if (audioInputDevice == null)
{
    MessageBox.Show("No se encontró dispositivo de entrada de audio.");
    return;
}

var audioInput = new AudioCaptureDeviceSourceSettings(api, audioInputDevice, audioInputDevice.GetDefaultFormat());

core.Audio_Source = audioInput;

// Configurar ajustes Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput("output.spx", speexSettings);

// Agregar la salida Speex
core.Outputs_Add(speexOutput, true);

// Establecer el modo de grabación de audio
core.Audio_Record = true;
core.Audio_Play = false;

// Iniciar la captura
await core.StartAsync();

// Detener después de 10 segundos
await Task.Delay(10000);

// Detener la captura
await core.StopAsync();
```

### Integración con Video Edit SDK

Edite y procese archivos de audio usando Speex en dotnet:

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Crear una instancia del núcleo Video Edit SDK
var core = new VideoEditCoreX();

// Agregar el archivo de audio fuente
var audioFile = new AudioFileSource(@"c:\samples\!audio.mp3");
VideoEdit1.Input_AddAudioFile(audioFile, null);

// Configurar ajustes Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput(@"output.spx", speexSettings);

// Agregar la salida Speex
core.Output_Format = speexOutput;

// Capturar evento OnStop
core.OnStop += (s, e) =>
{
    // Manejar el evento de parada aquí
    MessageBox.Show("Edición completa.");
};

core.OnProgress += (s, e) =>
{
    // Manejar actualizaciones de progreso aquí
    Debug.WriteLine($"Progreso: {e.Progress}%");
};

core.OnError += (s, e) =>
{
    // Manejar errores aquí
    Debug.WriteLine($"Error: {e.Message}");
};

// Iniciar la edición
core.Start();
```

### Integración con Media Blocks SDK

Procese flujos de audio usando Speex en su aplicación .NET:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;

using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Crear un nuevo pipeline
var pipeline = new MediaBlocksPipeline();

// Agregar fuente universal para leer archivo de audio
var sourceSettings = await UniversalSourceSettings.CreateAsync(@"c:\samples\!audio.mp3", renderVideo: false, renderAudio: true);
var source = new UniversalSourceBlock(sourceSettings);

// Agregar salida Speex
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    DTX = true,
    VAD = true
};

var speexOutput = new OGGSpeexOutputBlock("output.spx", speexSettings);

// Conectar
pipeline.Connect(source.AudioOutput, speexOutput.Input);

// Agregar manejador de evento OnStop
pipeline.OnStop += (sender, e) =>
{
    // Hacer algo cuando el pipeline se detiene
    MessageBox.Show("Conversión completa");
};

// Iniciar
await pipeline.StartAsync();
```

## Optimización de rendimiento

Al implementar codificación Speex, considere estas estrategias de optimización:

1. **Coincidir tasa de muestreo con el contenido** - Use Banda Estrecha (8 kHz) para audio telefónico, Banda Ancha (16 kHz) para la mayoría de aplicaciones de voz, y Banda Ultra Ancha (32 kHz) solo cuando se requiere máxima calidad

2. **Habilite VBR con VAD/DTX** para contenido de voz - Esta combinación proporciona eficiencia de ancho de banda óptima para grabaciones de voz típicas

3. **Ajuste la complejidad según la plataforma** - Las aplicaciones móviles pueden beneficiarse de valores de complejidad más bajos (2-4), mientras que las aplicaciones de escritorio pueden usar valores más altos (5-8)

4. **Use ABR para streaming** - La Tasa de Bits Promedio proporciona uso de ancho de banda predecible mientras mantiene flexibilidad de calidad

5. **Pruebe diferentes configuraciones de calidad** - A menudo una configuración de calidad de 5-7 proporciona excelentes resultados sin tamaño de archivo excesivo

## Casos de uso

La codificación Speex sobresale en estos escenarios de desarrollo:

- Aplicaciones VoIP y telefonía por internet
- Características de chat de voz en juegos y herramientas de colaboración
- Creación y distribución de podcasts
- Preprocesamiento de reconocimiento de voz
- Aplicaciones de notas de voz
- Archivo de audio de contenido hablado

## Instalación y configuración

Para comenzar con Speex en su aplicación dotnet, consulte la guía de instalación principal [aquí](../../install/index.md).

## Casos de uso comunes

### Captura y grabación de audio

Para aplicaciones de streaming, use estas configuraciones optimizadas:

```csharp
var streamingSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 3
};
```

### Aplicaciones Voice Over IP

Para aplicaciones VoIP, priorice baja latencia y eficiencia de ancho de banda:

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 5.0f,
    Complexity = 2
};
```

## Licencias y comunidad

Speex se publica bajo la licencia BSD, haciéndolo gratuito tanto para uso comercial como no comercial. El códec es mantenido activamente por la comunidad de código abierto, con actualizaciones y mejoras regulares.

## Preguntas frecuentes

### ¿Cuál es la mejor tasa de bits para grabación de voz?

Para la mayoría de aplicaciones de voz, una tasa de bits entre 8-15 kbps proporciona excelente calidad mientras mantiene tamaños de archivo razonables. Use modo VBR para resultados óptimos.

### ¿Cómo se compara Speex con otros códecs?

Speex ofrece calidad de voz superior comparado con muchos otros códecs a tasas de bits similares, especialmente para contenido de voz. Es particularmente efectivo para aplicaciones de baja tasa de bits.

### ¿Puedo usar Speex para codificación de música?

Aunque Speex puede codificar música, está específicamente optimizado para voz. Para contenido musical, considere usar otros códecs como AAC o MP3.

## Conclusión

La implementación de Speex por VisioForge proporciona a los desarrolladores .NET una herramienta poderosa para capturar, editar y grabar audio en aplicaciones C#. Ya sea que esté construyendo una nueva aplicación de captura de voz o mejorando una existente, Speex ofrece resultados excepcionales con uso mínimo de recursos. La flexibilidad y rendimiento del códec lo convierten en una excelente opción para cualquier desarrollador .NET que trabaje con procesamiento de audio.
