---
title: Integración Blackmagic Decklink con Media Blocks SDK
description: Integre dispositivos Blackmagic Decklink para captura y renderizado SDI y HDMI profesional con soporte multi-dispositivo en aplicaciones .NET.

---

# Integración Blackmagic Decklink con Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Integración Decklink

El VisioForge Media Blocks SDK para .NET proporciona soporte robusto para dispositivos Blackmagic Decklink, permitiendo a los desarrolladores implementar funcionalidad de audio y video de grado profesional en sus aplicaciones. Esta integración permite operaciones de captura y renderizado sin problemas usando el hardware de alta calidad de Decklink.

Nuestro SDK incluye bloques especializados diseñados específicamente para dispositivos Decklink, dándole acceso completo a sus capacidades incluyendo SDI, HDMI y otras entradas/salidas. Estos bloques están optimizados para rendimiento y ofrecen una API directa para implementar flujos de trabajo de medios complejos.

### Capacidades Principales

- **Captura y Renderizado de Audio**: Capture y emita audio a través de dispositivos Decklink
- **Captura y Renderizado de Video**: Capture y emita video en varios formatos y resoluciones
- **Soporte Multi-Dispositivo**: Trabaje con múltiples dispositivos Decklink simultáneamente
- **Opciones de E/S Profesional**: Utilice SDI, HDMI y otras interfaces profesionales
- **Procesamiento de Alta Calidad**: Mantenga la calidad profesional de video/audio a través del pipeline
- **Bloques Combinados de Audio/Video**: Manejo simplificado de flujos de audio y video sincronizados con bloques source y sink dedicados.

## Requisitos del Sistema

Antes de usar los bloques Decklink, asegúrese de que su sistema cumpla estos requisitos:

- **Hardware**: Dispositivo Blackmagic Decklink compatible
- **Software**: SDK o drivers de Blackmagic Decklink instalados

## Tipos de Bloques Decklink

El SDK proporciona varios tipos de bloques para trabajar con dispositivos Decklink:

1. **Bloque Audio Sink**: Para salida de audio a dispositivos Decklink.
2. **Bloque Audio Source**: Para captura de audio desde dispositivos Decklink.
3. **Bloque Video Sink**: Para salida de video a dispositivos Decklink.
4. **Bloque Video Source**: Para captura de video desde dispositivos Decklink.
5. **Bloque Video + Audio Sink**: Para salida sincronizada de video y audio a dispositivos Decklink usando un solo bloque.
6. **Bloque Video + Audio Source**: Para captura sincronizada de video y audio desde dispositivos Decklink usando un solo bloque.

Cada tipo de bloque está diseñado para manejar operaciones de medios específicas mientras mantiene sincronización y calidad.

## Trabajando con el Bloque Decklink Audio Sink

El bloque Decklink Audio Sink permite la salida de audio a dispositivos Blackmagic Decklink. Este bloque maneja las complejidades de temporización de audio e interfaz con el dispositivo.

### Enumeración de Dispositivos

Antes de crear un bloque audio sink, necesitará enumerar los dispositivos disponibles:

```csharp
var devices = await DecklinkAudioSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Dispositivo encontrado: {item.Name}, Número de Dispositivo: {item.DeviceNumber}");
}
```

Este código recupera todos los dispositivos Decklink disponibles que soportan funcionalidad de salida de audio.

### Creación y Configuración del Bloque

Una vez que haya identificado el dispositivo objetivo, puede crear y configurar el bloque audio sink:

```csharp
// Obtener el primer dispositivo disponible
var deviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Crear configuraciones para el dispositivo seleccionado
DecklinkAudioSinkSettings audioSinkSettings = null;
if (deviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(deviceInfo);
    // Ejemplo: audioSinkSettings.DeviceNumber = deviceInfo.DeviceNumber; (ya establecido por el constructor)
    // Configuración adicional:
    // audioSinkSettings.BufferTime = TimeSpan.FromMilliseconds(100);
    // audioSinkSettings.IsSync = true;
}

// Crear el bloque con las configuraciones configuradas
var decklinkAudioSink = new DecklinkAudioSinkBlock(audioSinkSettings);
```

### Configuraciones Principales de Audio Sink

La clase `DecklinkAudioSinkSettings` incluye propiedades como:

- `DeviceNumber`: La instancia del dispositivo de salida a usar.
- `BufferTime`: Latencia mínima reportada por el sink (predeterminado: 50ms).
- `AlignmentThreshold`: Umbral de alineación de marca de tiempo (predeterminado: 40ms).
- `DiscontWait`: Tiempo de espera antes de crear una discontinuidad (predeterminado: 1s).
- `IsSync`: Habilita sincronización (predeterminado: true).

### Conectando al Pipeline

El bloque audio sink incluye un pad `Input` que acepta datos de audio de otros bloques en su pipeline:

```csharp
// Ejemplo: Conectar un codificador/fuente de audio al sink de audio Decklink
audioEncoder.Output.Connect(decklinkAudioSink.Input);
```

## Trabajando con el Bloque Decklink Audio Source

El bloque Decklink Audio Source permite capturar audio desde dispositivos Blackmagic Decklink. Soporta varios formatos y configuraciones de audio.

### Enumeración de Dispositivos

Enumere los dispositivos de fuente de audio disponibles:

```csharp
var devices = await DecklinkAudioSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Fuente de audio disponible: {item.Name}, Número de Dispositivo: {item.DeviceNumber}");
}
```

### Creación y Configuración del Bloque

Cree y configure el bloque audio source:

```csharp
// Obtener el primer dispositivo disponible
var deviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Crear configuraciones para el dispositivo seleccionado
DecklinkAudioSourceSettings audioSourceSettings = null;
if (deviceInfo != null)
{
    // crear objeto de configuraciones
    audioSourceSettings = new DecklinkAudioSourceSettings(deviceInfo);
    // Configuración adicional:
    // audioSourceSettings.Channels = DecklinkAudioChannels.Ch2;
    // audioSourceSettings.Connection = DecklinkAudioConnection.Embedded;
    // audioSourceSettings.Format = DecklinkAudioFormat.S16LE; // SampleRate es fijo a 48000
}

// Crear el bloque con las configuraciones configuradas
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);
```

### Configuraciones Principales de Audio Source

La clase `DecklinkAudioSourceSettings` incluye propiedades como:

- `DeviceNumber`: La instancia del dispositivo de entrada a usar.
- `Channels`: Canales de audio a capturar (ej., `DecklinkAudioChannels.Ch2`, `Ch8`, `Ch16`). Predeterminado `Ch2`.
- `Format`: Formato de muestra de audio (ej., `DecklinkAudioFormat.S16LE`). Predeterminado `S16LE`. La tasa de muestreo es fija a 48000 Hz.
- `Connection`: Tipo de conexión de audio (ej., `DecklinkAudioConnection.Embedded`, `AES`, `Analog`). Predeterminado `Auto`.
- `BufferSize`: Tamaño de buffer interno en frames (predeterminado: 5).
- `DisableAudioConversion`: Establezca en `true` para deshabilitar la conversión de audio interna. Predeterminado `false`.

### Conectando al Pipeline

El bloque audio source proporciona un pad `Output` que puede conectarse a otros bloques:

```csharp
// Ejemplo: Conectar la fuente de audio a un codificador o procesador de audio
audioSource.Output.Connect(audioProcessor.Input);
```

## Trabajando con el Bloque Decklink Video Sink

El bloque Decklink Video Sink permite la salida de video a dispositivos Blackmagic Decklink, soportando varios formatos de video y resoluciones.

### Enumeración de Dispositivos

Encuentre los dispositivos video sink disponibles:

```csharp
var devices = await DecklinkVideoSinkBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Dispositivo de salida de video disponible: {item.Name}, Número de Dispositivo: {item.DeviceNumber}");
}
```

### Creación y Configuración del Bloque

Cree y configure el bloque video sink:

```csharp
// Obtener el primer dispositivo disponible
var deviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();

// Crear configuraciones para el dispositivo seleccionado
// Nota: Mode es requerido y debe especificarse en el constructor
DecklinkVideoSinkSettings videoSinkSettings = null;
if (deviceInfo != null)
{
    // Mode es requerido - especifica la resolución de video y tasa de frames de salida
    videoSinkSettings = new DecklinkVideoSinkSettings(deviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_10bit,
        // Opcional: Configuración adicional
        // KeyerMode = DecklinkKeyerMode.Internal,
        // KeyerLevel = 128,
        // Profile = DecklinkProfileID.Default,
        // TimecodeFormat = DecklinkTimecodeFormat.RP188Any
    };
}

// Crear el bloque con las configuraciones configuradas
var decklinkVideoSink = new DecklinkVideoSinkBlock(videoSinkSettings);
```

### Configuraciones Principales de Video Sink

La clase `DecklinkVideoSinkSettings` incluye propiedades como:

- `DeviceNumber`: La instancia del dispositivo de salida a usar (solo lectura, se establece via constructor).
- `Mode`: Especifica la resolución de video y tasa de frames (ej., `DecklinkMode.HD1080i60`, `HD720p60`). **Requerido** - debe especificarse en el constructor.
- `VideoFormat`: Define el formato de píxel usando enum `DecklinkVideoFormat` (ej., `DecklinkVideoFormat.YUV_8bit`, `YUV_10bit`). Predeterminado `YUV_8bit`.
- `KeyerMode`: Controla opciones de keying/composición usando `DecklinkKeyerMode` (si es soportado por el dispositivo). Predeterminado `Off`.
- `KeyerLevel`: Establece el nivel de keyer (0-255). Predeterminado `255`.
- `Profile`: Especifica el perfil Decklink a usar con `DecklinkProfileID`.
- `TimecodeFormat`: Especifica el formato de timecode para reproducción usando `DecklinkTimecodeFormat`. Predeterminado `RP188Any`.
- `CustomVideoSize`: Efecto de redimensionado opcional a aplicar antes de la salida.
- `CustomFrameRate`: Conversión de tasa de frames opcional antes de la salida.
- `IsSync`: Habilita sincronización (predeterminado: true).

**Importante**: El parámetro `Mode` es requerido y determina la tasa de frames y resolución de salida. Si no se especifica correctamente, el hardware Decklink puede emitir a una tasa de frames inesperada.

## Trabajando con el Bloque Decklink Video Source

El bloque Decklink Video Source permite capturar video desde dispositivos Blackmagic Decklink, soportando varios formatos de entrada y resoluciones.

### Enumeración de Dispositivos

Enumere los dispositivos de captura de video:

```csharp
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
foreach (var item in devices)
{
    Console.WriteLine($"Dispositivo de captura de video disponible: {item.Name}, Número de Dispositivo: {item.DeviceNumber}");
}
```

### Creación y Configuración del Bloque

Cree y configure el bloque video source:

```csharp
// Obtener el primer dispositivo disponible
var deviceInfo = (await DecklinkVideoSourceBlock.GetDevicesAsync()).FirstOrDefault();

// Crear configuraciones para el dispositivo seleccionado
DecklinkVideoSourceSettings videoSourceSettings = null;
if (deviceInfo != null)
{
    videoSourceSettings = new DecklinkVideoSourceSettings(deviceInfo);
    
    // Configurar formato de entrada de video y modo
    videoSourceSettings.Mode = DecklinkMode.HD1080i60;
    videoSourceSettings.Connection = DecklinkConnection.SDI; 
    // videoSourceSettings.VideoFormat = DecklinkVideoFormat.Auto; // Frecuentemente usado con Mode=Auto
}

// Crear el bloque con configuraciones configuradas
var videoSourceBlock = new DecklinkVideoSourceBlock(videoSourceSettings);
```

### Configuraciones Principales de Video Source

La clase `DecklinkVideoSourceSettings` incluye propiedades como:

- `DeviceNumber`: La instancia del dispositivo de entrada a usar.
- `Mode`: Especifica la resolución de entrada esperada y tasa de frames (ej., `DecklinkMode.HD1080i60`). Predeterminado `Unknown`.
- `Connection`: Define qué entrada física usar, usando enum `DecklinkConnection` (ej., `DecklinkConnection.HDMI`, `DecklinkConnection.SDI`). Predeterminado `Auto`.
- `VideoFormat`: Especifica el tipo de formato de video para entrada, usando enum `DecklinkVideoFormat`. Predeterminado `Auto` (especialmente cuando `Mode` es `Auto`).
- `Profile`: Especifica el perfil Decklink usando `DecklinkProfileID`. Predeterminado `Default`.
- `DropNoSignalFrames`: Si `true`, descarta frames marcados como sin señal de entrada. Predeterminado `false`.
- `OutputAFDBar`: Si `true`, extrae y emite datos AFD/Bar como Meta. Predeterminado `false`.
- `OutputCC`: Si `true`, extrae y emite Closed Captions como Meta. Predeterminado `false`.
- `TimecodeFormat`: Especifica el formato de timecode usando `DecklinkTimecodeFormat`. Predeterminado `RP188Any`.
- `DisableVideoConversion`: Establezca en `true` para deshabilitar la conversión de video interna. Predeterminado `false`.

## Trabajando con el Bloque Decklink Video + Audio Source

El `DecklinkVideoAudioSourceBlock` simplifica la captura de flujos de video y audio sincronizados desde un solo dispositivo Decklink.

### Enumeración de Dispositivos y Configuración

La selección de dispositivos se gestiona a través de `DecklinkVideoSourceSettings` y `DecklinkAudioSourceSettings`. Típicamente enumeraría dispositivos de video usando `DecklinkVideoSourceBlock.GetDevicesAsync()` y dispositivos de audio usando `DecklinkAudioSourceBlock.GetDevicesAsync()`, luego configuraría los objetos de configuraciones respectivos para el dispositivo elegido. El `DecklinkVideoAudioSourceBlock` también proporciona `GetDevicesAsync()` que enumera fuentes de video.

```csharp
// Enumerar dispositivos de video (para la parte de video de la fuente combinada)
var videoDeviceInfo = (await DecklinkVideoAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(); // o DecklinkVideoSourceBlock.GetDevicesAsync()
var audioDeviceInfo = (await DecklinkAudioSourceBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoDeviceInfo.DeviceNumber); // Ejemplo: coincidir por número de dispositivo

DecklinkVideoSourceSettings videoSettings = null;
if (videoDeviceInfo != null)
{
    videoSettings = new DecklinkVideoSourceSettings(videoDeviceInfo);
    videoSettings.Mode = DecklinkMode.HD1080i60;
    videoSettings.Connection = DecklinkConnection.SDI;
}

DecklinkAudioSourceSettings audioSettings = null;
if (audioDeviceInfo != null)
{
    audioSettings = new DecklinkAudioSourceSettings(audioDeviceInfo);
    audioSettings.Channels = DecklinkAudioChannels.Ch2;
}

// Crear el bloque con configuraciones configuradas
if (videoSettings != null && audioSettings != null)
{
    var decklinkVideoAudioSource = new DecklinkVideoAudioSourceBlock(videoSettings, audioSettings);

    // Conectar salidas
    // decklinkVideoAudioSource.VideoOutput.Connect(videoProcessor.Input);
    // decklinkVideoAudioSource.AudioOutput.Connect(audioProcessor.Input);
}
```

### Creación y Configuración del Bloque

Usted instancia `DecklinkVideoAudioSourceBlock` proporcionando objetos `DecklinkVideoSourceSettings` y `DecklinkAudioSourceSettings` preconfigurados.

```csharp
// Asumiendo que videoSourceSettings y audioSourceSettings están configurados como arriba
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);
```

### Conectando al Pipeline

El bloque proporciona pads `VideoOutput` y `AudioOutput` separados:

```csharp
// Ejemplo: Conectar a procesadores/codificadores de video y audio
videoAudioSource.VideoOutput.Connect(videoEncoder.Input);
videoAudioSource.AudioOutput.Connect(audioEncoder.Input);
```

## Trabajando con el Bloque Decklink Video + Audio Sink

El `DecklinkVideoAudioSinkBlock` simplifica el envío de flujos de video y audio sincronizados a un solo dispositivo Decklink.

### Enumeración de Dispositivos y Configuración

Similar a la fuente combinada, la selección de dispositivos se gestiona vía `DecklinkVideoSinkSettings` y `DecklinkAudioSinkSettings`. Enumere dispositivos usando `DecklinkVideoSinkBlock.GetDevicesAsync()` y `DecklinkAudioSinkBlock.GetDevicesAsync()`.

```csharp
var videoSinkDeviceInfo = (await DecklinkVideoSinkBlock.GetDevicesAsync()).FirstOrDefault();
var audioSinkDeviceInfo = (await DecklinkAudioSinkBlock.GetDevicesAsync()).FirstOrDefault(d => d.DeviceNumber == videoSinkDeviceInfo.DeviceNumber); // Ejemplo de coincidencia

DecklinkVideoSinkSettings videoSinkSettings = null;
if (videoSinkDeviceInfo != null)
{
    // El modo es requerido en el constructor para garantizar una configuración correcta de la velocidad de fotogramas
    videoSinkSettings = new DecklinkVideoSinkSettings(videoSinkDeviceInfo, DecklinkMode.HD1080i60)
    {
        VideoFormat = DecklinkVideoFormat.YUV_8bit
    };
}

DecklinkAudioSinkSettings audioSinkSettings = null;
if (audioSinkDeviceInfo != null)
{
    audioSinkSettings = new DecklinkAudioSinkSettings(audioSinkDeviceInfo);
}

// Crear el bloque
if (videoSinkSettings != null && audioSinkSettings != null)
{
    var decklinkVideoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
    
    // Conectar entradas
    // videoEncoder.Output.Connect(decklinkVideoAudioSink.VideoInput);
    // audioEncoder.Output.Connect(decklinkVideoAudioSink.AudioInput);
}
```

### Creación y Configuración del Bloque

Instancie `DecklinkVideoAudioSinkBlock` con `DecklinkVideoSinkSettings` y `DecklinkAudioSinkSettings` configurados.

```csharp
// Asumiendo que videoSinkSettings y audioSinkSettings están configurados
var videoAudioSink = new DecklinkVideoAudioSinkBlock(videoSinkSettings, audioSinkSettings);
```

### Conectando al Pipeline

El bloque proporciona pads `VideoInput` y `AudioInput` separados:

```csharp
// Ejemplo: Conectar desde codificadores de video y audio
videoEncoder.Output.Connect(videoAudioSink.VideoInput);
audioEncoder.Output.Connect(videoAudioSink.AudioInput);
```

## Ejemplos de Uso Avanzado

### Captura Sincronizada de Audio/Video

**Usando bloques source separados:**

```csharp
// Asuma que videoSourceSettings y audioSourceSettings están configurados para el mismo dispositivo/temporización
var videoSource = new DecklinkVideoSourceBlock(videoSourceSettings);
var audioSource = new DecklinkAudioSourceBlock(audioSourceSettings);

// Crear un codificador MP4
var mp4Settings = new MP4SinkSettings("output.mp4");
var sink = new MP4SinkBlock(mp4Settings);

// Crear codificador de video
var videoEncoder = new H264EncoderBlock();

// Crear codificador de audio
var audioEncoder = new AACEncoderBlock();

// Conectar fuentes de video y audio
pipeline.Connect(videoSource.Output, videoEncoder.Input);
pipeline.Connect(audioSource.Output, audioEncoder.Input);

// Conectar codificador de video al sink
pipeline.Connect(videoEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar codificador de audio al sink
pipeline.Connect(audioEncoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Iniciar el pipeline
await pipeline.StartAsync();
```

**Usando `DecklinkVideoAudioSourceBlock` para captura sincronizada simplificada:**
Si usa `DecklinkVideoAudioSourceBlock` (como está configurado en su sección dedicada), la configuración de fuente se convierte en:

```csharp
// Asumiendo que videoSourceSettings y audioSourceSettings están configurados para el mismo dispositivo
var videoAudioSource = new DecklinkVideoAudioSourceBlock(videoSourceSettings, audioSourceSettings);

// ... (configuración de codificadores y sink como arriba) ...

// Conectar video y audio desde la fuente combinada
pipeline.Connect(videoAudioSource.VideoOutput, videoEncoder.Input);
pipeline.Connect(videoAudioSource.AudioOutput, audioEncoder.Input);

// ... (conectar codificadores al sink e iniciar pipeline como arriba) ...
```

Esto asegura que audio y video se obtienen del dispositivo Decklink de manera sincronizada por el SDK.

## Consejos de Solución de Problemas

- **No se Encuentran Dispositivos**: Asegúrese de que los drivers/SDK de Blackmagic estén instalados y actualizados. Verifique si el dispositivo es reconocido por Blackmagic Desktop Video Setup.
- **Desajuste de Formato**: Verifique que el dispositivo soporte su modo de video/audio, formato y tipo de conexión seleccionados. Para fuentes con `Mode = DecklinkMode.Unknown` (auto-detección), asegúrese de que haya una señal estable presente.
- **Problemas de Rendimiento**: Verifique recursos del sistema (CPU, RAM, E/S de disco). Considere reducir resolución/tasa de frames si los problemas persisten.
- **Detección de Señal**: Para dispositivos de entrada, verifique las conexiones de cable y asegúrese de que el dispositivo fuente esté emitiendo una señal válida.
- **Errores "Unable to build ...Block"**: Verifique que todas las configuraciones sean válidas para el dispositivo y modo seleccionados. Asegúrese de usar el `DeviceNumber` correcto si hay múltiples tarjetas Decklink presentes.

## Aplicaciones de Ejemplo

Para ejemplos de trabajo completos, consulte estas aplicaciones de ejemplo:

- [Demo Decklink](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Decklink%20Demo)

## Conclusión

Los bloques Blackmagic Decklink en el VisioForge Media Blocks SDK proporcionan una forma potente y flexible de integrar hardware de video y audio profesional en sus aplicaciones .NET. Al aprovechar los bloques source y sink específicos, incluyendo los bloques combinados de audio/video, puede implementar eficientemente flujos de trabajo complejos de captura y reproducción. Siempre consulte las clases de configuraciones específicas para opciones de configuración detalladas.
