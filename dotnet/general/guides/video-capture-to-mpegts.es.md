---
title: Captura de Video a MPEG-TS en C# y .NET
description: Captura video y audio a archivos MPEG-TS en C# con aceleración por hardware, selección de formato y soporte multiplataforma para aplicaciones .NET.
---

# Captura de Video a MPEG-TS en C# y .NET: Guía Completa

## Introducción

Esta guía técnica demuestra cómo capturar video TS en C# desde cámaras y micrófonos usando dos potentes soluciones multimedia de VisioForge: Video Capture SDK .NET con motor VideoCaptureCoreX y Media Blocks SDK .NET con motor MediaBlocksPipeline. Ambos SDKs proporcionan capacidades robustas para capturar, grabar y editar archivos TS (MPEG Transport Stream) en aplicaciones .NET. Exploraremos muestras de código detalladas para implementar captura de video/audio a TS en C# con rendimiento y calidad optimizados.

## Instalación y despliegue

Por favor consulta la [guía de instalación](../../install/index.md) para instrucciones detalladas sobre cómo instalar los SDKs .NET de VisioForge en tu sistema.

## Video Capture SDK .NET (VideoCaptureCoreX) - Capturar MPEG-TS en C#

VideoCaptureCoreX proporciona un enfoque simplificado para capturar video y audio TS en C#. Su arquitectura basada en componentes maneja el complejo pipeline de medios, permitiendo a los desarrolladores enfocarse en la configuración en lugar de detalles de implementación de bajo nivel al trabajar con archivos TS en .NET.

### Componentes Principales

1. **VideoCaptureCoreX**: Motor principal para gestionar captura de video, renderizado y salida TS.
2. **VideoView**: Componente de UI para renderizado de video en tiempo real durante la captura.
3. **DeviceEnumerator**: Clase para descubrir dispositivos de video/audio.
4. **VideoCaptureDeviceSourceSettings**: Configuración para entrada de cámara al capturar MPEG-TS.
5. **AudioRendererSettings**: Configuración para reproducción de audio con soporte AAC.
6. **MPEGTSOutput**: Configuración específicamente para salida de archivo MPEG-TS.

### Ejemplo de Implementación

Aquí hay una implementación completa en C# para capturar y grabar archivos MPEG-TS:

```csharp
// Instancia de clase para motor de captura de video
VideoCaptureCoreX videoCapture;

private async Task StartCaptureAsync()
{
    // Inicializar el SDK de VisioForge
    await VisioForgeX.InitSDKAsync();

    // Crear instancia de VideoCaptureCoreX y asociar con control UI VideoView
    videoCapture = new VideoCaptureCoreX(videoView: VideoView1);

    // Obtener lista de dispositivos de captura de video disponibles
    var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

    // Inicializar configuración de fuente de video
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;

    // Obtener primer dispositivo de captura de video disponible
    var videoDevice = videoSources[0];

    // Intentar obtener resolución HD y capacidades de tasa de fotogramas del dispositivo
    var videoFormat = videoDevice.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    if (videoFormat != null)
    {
        // Configurar fuente de video con formato HD
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
        {
            Format = videoFormat.ToFormat()
        };

        // Establecer tasa de fotogramas de captura
        videoSourceSettings.Format.FrameRate = frameRate;
    }

    // Configurar dispositivo de captura de video con ajustes
    videoCapture.Video_Source = videoSourceSettings;

    // Configurar captura de audio (micrófono)

    // Inicializar configuración de fuente de audio
    IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;

    // Obtener dispositivos de captura de audio disponibles usando API DirectSound
    var audioApi = AudioCaptureDeviceAPI.DirectSound;
    var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(audioApi);

    // Obtener primer dispositivo de captura de audio disponible
    var audioDevice = audioDevices[0];
    if (audioDevice != null)
    {
        // Obtener formato de audio predeterminado soportado por el dispositivo
        var audioFormat = audioDevice.GetDefaultFormat();
        if (audioFormat != null)
        {
            // Configurar fuente de audio con formato predeterminado
            audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);
        }
    }

    // Configurar dispositivo de captura de audio con ajustes
    videoCapture.Audio_Source = audioSourceSettings;

    // Configurar dispositivo de reproducción de audio
    // Obtener primer dispositivo de salida de audio disponible
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())[0];

    // Configurar renderizador de audio para reproducción a través del dispositivo seleccionado
    videoCapture.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

    // Habilitar monitoreo y grabación de audio
    videoCapture.Audio_Play = true;    // Habilitar monitoreo de audio en tiempo real
    videoCapture.Audio_Record = true;   // Habilitar grabación de audio a archivo de salida

    // Configurar salida MPEG Transport Stream
    var mpegtsOutput = new MPEGTSOutput("output.ts");

    // Configurar codificador de video con aceleración por hardware si está disponible
    if (NVENCH264EncoderSettings.IsAvailable())
    {
        // Usar codificador de hardware NVIDIA
        mpegtsOutput.Video = new NVENCH264EncoderSettings();
    }
    else if (QSVH264EncoderSettings.IsAvailable())
    {
        // Usar codificador de hardware Intel Quick Sync
        mpegtsOutput.Video = new QSVH264EncoderSettings();
    }
    else if (AMFH264EncoderSettings.IsAvailable())
    {
        // Usar codificador de hardware AMD
        mpegtsOutput.Video = new AMFH264EncoderSettings();
    }
    else
    {
        // Recurrir a codificador de software
        mpegtsOutput.Video = new OpenH264EncoderSettings();
    }

    // Configurar codificador de audio para salida MPEG-TS
    // mpegtsOutput.Audio = ...

    // Añadir salida MPEG-TS al pipeline de captura
    // autostart: true significa que la salida inicia automáticamente con la captura
    videoCapture.Outputs_Add(mpegtsOutput, autostart: true);

    // Iniciar el proceso de captura
    await videoCapture.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Detener toda captura y codificación
    await videoCapture.StopAsync();

    // Limpiar recursos
    await videoCapture.DisposeAsync();
}
```

### Características Avanzadas de VideoCaptureCoreX para Grabación MPEG-TS

1. **Aceleración por Hardware**: Soporte para codificación por hardware NVIDIA (NVENC), Intel (QSV) y AMD (AMF).
2. **Selección de Formato**: El SDK proporciona acceso a los formatos nativos de cámara y tasas de fotogramas.
3. **Configuración de Audio**: Proporciona control de volumen y selección de formato.
4. **Múltiples Salidas**: Capacidad de añadir múltiples formatos de salida simultáneamente.

## Media Blocks SDK .NET (MediaBlocksPipeline) - Capturar TS en C#

El motor MediaBlocksPipeline en Media Blocks SDK .Net toma un enfoque arquitectónico diferente, enfocándose en un sistema modular basado en bloques donde cada componente (bloque) tiene responsabilidades específicas en el pipeline de procesamiento de medios.

### Bloques Principales

1. **MediaBlocksPipeline**: El contenedor principal y controlador para el pipeline de bloques de medios.
2. **SystemVideoSourceBlock**: Captura video desde webcams.
3. **SystemAudioSourceBlock**: Captura audio desde micrófonos.
4. **VideoRendererBlock**: Renderiza el video a un control VideoView.
5. **AudioRendererBlock**: Maneja la reproducción de audio.
6. **TeeBlock**: Divide flujos de medios para procesamiento simultáneo (ej. visualización y codificación).
7. **H264EncoderBlock**: Codifica video usando H.264.
8. **AACEncoderBlock**: Codifica audio usando AAC.
9. **MPEGTSSinkBlock**: Guarda flujos codificados a un archivo MPEG-TS.

### Ejemplo de Implementación

Aquí está cómo implementar captura avanzada de archivos TS en C#:

```csharp
// Instancia del pipeline
MediaBlocksPipeline pipeline;

private async Task StartCaptureAsync()
{
    // Inicializar el SDK
    await VisioForgeX.InitSDKAsync();

    // Crear nueva instancia del pipeline
    pipeline = new MediaBlocksPipeline();

    // Obtener primer dispositivo de video disponible y configurar formato HD
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var formatItem = device.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = formatItem.ToFormat()
    };
    videoSourceSettings.Format.FrameRate = frameRate;

    // Crear bloque de fuente de video con ajustes configurados
    var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

    // Obtener primer dispositivo de audio disponible y configurar formato predeterminado
    var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
    var audioFormat = audioDevice.GetDefaultFormat();
    var audioSourceSettings = audioDevice.CreateSourceSettings(audioFormat);
    var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

    // Crear bloque de renderizado de video y conectar al control UI VideoView
    var videoRenderer = new VideoRendererBlock(pipeline, videoView: VideoView1) { IsSync = false };

    // Crear bloque de renderizado de audio para reproducción
    var audioRenderer = new AudioRendererBlock() { IsSync = false };

    // Nota: IsSync es false para maximizar el rendimiento de codificación

    // Crear tees de video y audio  
    var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);
    var audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio);

    // Crear muxer MPEG-TS
    var muxer = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));

    // Crear codificadores de video y audio con aceleración por hardware si está disponible
    var videoEncoder = new H264EncoderBlock();
    var audioEncoder = new AACEncoderBlock();

    // Conectar bloques de procesamiento de video:
    // Fuente -> Tee -> Renderizador (vista previa) y Codificador -> Muxer
    pipeline.Connect(videoSource.Output, videoTee.Input);
    pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
    pipeline.Connect(videoTee.Outputs[1], videoEncoder.Input);
    pipeline.Connect(videoEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video));

    // Conectar bloques de procesamiento de audio:
    // Fuente -> Tee -> Renderizador (reproducción) y Codificador -> Muxer
    pipeline.Connect(audioSource.Output, audioTee.Input);
    pipeline.Connect(audioTee.Outputs[0], audioRenderer.Input);
    pipeline.Connect(audioTee.Outputs[1], audioEncoder.Input);
    pipeline.Connect(audioEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio));

    // Iniciar el procesamiento del pipeline
    await pipeline.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Detener todo procesamiento del pipeline
    await pipeline.StopAsync();

    // Limpiar recursos
    await pipeline.DisposeAsync();
    pipeline = null;
}
```

### Características Avanzadas de MediaBlocksPipeline

1. **Control Detallado**: Control directo sobre cada paso de procesamiento en el pipeline.
2. **Construcción Dinámica de Pipeline**: Capacidad de crear pipelines de procesamiento complejos conectando bloques.
3. **Múltiples Rutas de Procesamiento**: Usar TeeBlock para dividir flujos para diferentes rutas de procesamiento.
4. **Bloques Personalizados**: Capacidad de crear e integrar bloques de procesamiento personalizados.
5. **Manejo de Errores Granular**: Eventos de error a nivel de cada bloque.

## Configuración de Salida TS con Audio AAC

Ambos SDKs proporcionan soporte robusto para salida MPEG-TS, que es particularmente útil para aplicaciones de transmisión y streaming debido a sus características de resistencia a errores y baja latencia.

### División de Archivos para Grabación MPEG-TS

Ambos SDKs soportan división automática de archivos para salida MPEG-TS, habilitando grabación segmentada basada en duración, tamaño de archivo o código de tiempo. Esto es esencial para:

- **Streaming HLS**: Crear archivos segmentados para HTTP Live Streaming
- **Gestión de Almacenamiento**: Limitar tamaños de archivo e implementar búferes circulares
- **Funcionalidad DVR**: Grabar flujos continuos con rotación automática de archivos
- **Grabación Time-Lapse**: Dividir grabaciones a intervalos regulares

#### Usando División de Archivos con VideoCaptureCoreX

```csharp
using VisioForge.Core.Types.X.Sinks;

// Crear configuración de división para segmentos de 5 minutos
var mpegtsOutput = new MPEGTSOutput("grabacion_%05d.ts", useAAC: true);
mpegtsOutput.Sink = new MPEGTSSplitSinkSettings("grabacion_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(5),  // Nuevo archivo cada 5 minutos
    SplitMaxFiles = 12,  // Mantener solo últimos 12 archivos (1 hora total)
    M2TSMode = false
};

// Añadir al pipeline de captura
videoCapture.Outputs_Add(mpegtsOutput, autostart: true);
```

#### Usando División de Archivos con MediaBlocksPipeline

```csharp
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.MediaBlocks.Sinks;

// Crear muxer MPEG-TS con configuración de división
var splitSettings = new MPEGTSSplitSinkSettings("salida_%05d.ts")
{
    SplitDuration = TimeSpan.FromMinutes(10),  // Dividir cada 10 minutos
    SplitFileSize = 100 * 1024 * 1024,  // O cuando archivo alcanza 100 MB
    SplitMaxFiles = 6,  // Mantener últimos 6 archivos
    StartIndex = 0
};

var muxer = new MPEGTSSinkBlock(splitSettings);

// Conectar codificadores de video y audio al muxer
pipeline.Connect(videoEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(audioEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio));
```

#### Opciones de Configuración de División

La clase `MPEGTSSplitSinkSettings` proporciona varias opciones:

- **SplitDuration**: Duración máxima por archivo (TimeSpan)
- **SplitFileSize**: Tamaño máximo de archivo en bytes (ulong)
- **SplitMaxSizeTimecode**: Dividir basado en diferencia de código de tiempo (string, formato: "HH:MM:SS:FF")
- **SplitMaxFiles**: Número máximo de archivos a mantener; archivos antiguos se eliminan automáticamente (uint, 0 = ilimitado)
- **StartIndex**: Índice inicial para numeración de segmentos (int)
- **M2TSMode**: Usar formato Blu-ray M2TS con paquetes de 192 bytes (bool)

Los archivos se dividirán cuando **cualquiera** de los criterios configurados (duración, tamaño o código de tiempo) se cumpla primero.

#### Patrón de Nombre de Archivo

El patrón de nombre de archivo usa formato estilo printf para números de segmento:
- `"video_%05d.ts"` → `video_00000.ts`, `video_00001.ts`, ...
- `"stream_%02d.ts"` → `stream_00.ts`, `stream_01.ts`, ...

Lee más sobre codificadores de video y audio disponibles para captura TS en .NET:

- [Codificadores H264](../video-encoders/h264.md)
- [Codificadores HEVC](../video-encoders/hevc.md)
- [Codificadores AAC](../audio-encoders/aac.md)
- [Codificadores MP3](../audio-encoders/mp3.md)
- [Salida MPEG-TS](../output-formats/mpegts.md)

## Consideraciones Multiplataforma

Ambos SDKs ofrecen capacidades multiplataforma, pero con diferentes enfoques:

1. **VideoCaptureCoreX**: Proporciona una API unificada a través de plataformas con implementaciones específicas de plataforma.
2. **MediaBlocksPipeline**: Usa una arquitectura consistente basada en bloques a través de plataformas, con bloques manejando diferencias de plataforma internamente.

## Aplicaciones de Ejemplo

- [Aplicación de Ejemplo VideoCaptureCoreX](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Simple%20Video%20Capture)
- [Aplicación de Ejemplo MediaBlocksPipeline](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

## Conclusión: Elegir el SDK Correcto para Captura MPEG-TS en C#

VisioForge ofrece dos soluciones potentes para grabar archivos MPEG-TS en C# y .NET:

- **VideoCaptureCoreX** proporciona una API simplificada para implementación rápida de captura MPEG-TS en C#, ideal para proyectos donde la facilidad de uso es esencial.

- **MediaBlocksPipeline** ofrece máxima flexibilidad para escenarios complejos de grabación y edición MPEG-TS en .NET a través de su arquitectura modular de bloques.

Ambos SDKs sobresalen en capturar video desde cámaras y audio desde micrófonos, con soporte completo para salida MPEG-TS, haciéndolos herramientas valiosas para desarrollar una amplia gama de aplicaciones multimedia.

Elige VideoCaptureCoreX para implementación rápida de escenarios de captura TS estándar, o MediaBlocksPipeline para flujos de trabajo avanzados de edición y procesamiento personalizado con archivos TS en tus aplicaciones .NET.
