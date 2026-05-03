---
title: Streaming NDI de Video y Audio por Red IP en C# .NET
description: Transmita video y audio a NDI desde cámaras y archivos en C# .NET. Guía con ejemplos de salida SDK, remuestreo de audio y solución de problemas.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - GStreamer
  - Capture
  - Streaming
  - Editing
  - Webcam
  - NDI
  - MP4
  - C#
primary_api_classes:
  - NDISinkBlock
  - AudioResamplerBlock
  - NDIOutput
  - MediaBlockPadMediaType
  - MediaBlocksPipeline

---

# Integración de Transmisión Network Device Interface (NDI)

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## ¿Qué es NDI?

Network Device Interface (NDI) es un estándar de la industria para producción de video en vivo sobre redes IP. Permite transmisión de video y audio de alta calidad y baja latencia sobre Ethernet convencional, reemplazando costoso cableado SDI con flujos de trabajo basados en software. Los casos de uso comunes incluyen:

- Transmisión y streaming en vivo
- Videoconferencia profesional
- Configuraciones de producción multi-cámara
- Flujos de trabajo de producción remota
- Aplicaciones de servidor de playout

El SDK de VisioForge proporciona soporte de salida NDI para flujos de trabajo de escritorio y servidor, permitiéndole transmitir cámaras, dispositivos de captura o archivos multimedia a receptores NDI en su red.

## Requisitos de Instalación

Para usar transmisión NDI, instale uno de los siguientes paquetes oficiales de NDI:

1. **[NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/)** - Recomendado para desarrolladores
2. **[NDI Tools](https://ndi.video/tools/)** - Adecuado para pruebas

Estos proporcionan los componentes de runtime que habilitan la comunicación NDI. Puede verificar la disponibilidad de NDI en código:

```csharp
bool ndiAvailable = NDISinkBlock.IsAvailable();
```

## Salida NDI Multiplataforma

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Clase NDIOutput

La clase `NDIOutput` proporciona salida NDI para los motores VideoCaptureCoreX y VideoEditCoreX:

```csharp
public class NDIOutput : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

#### Configuración

| Propiedad | Tipo | Descripción |
| --- | --- | --- |
| `Sink` | `NDISinkSettings` | Configuración de salida NDI (nombre del flujo, compresión, configuraciones de red) |
| `CustomVideoProcessor` | `MediaBlock` | Procesamiento de video personalizado opcional antes de la transmisión NDI |
| `CustomAudioProcessor` | `MediaBlock` | Procesamiento de audio personalizado opcional antes de la transmisión NDI |

#### Constructores

```csharp
// Crear con nombre de flujo
var output = new NDIOutput("My Stream");

// Crear con configuraciones pre-configuradas
var output = new NDIOutput(new NDISinkSettings("My Stream"));
```

## Ejemplos de Implementación

### Media Blocks SDK

```cs
// Crear un bloque de salida NDI con un nombre de flujo descriptivo
var ndiSink = new NDISinkBlock("VisioForge Production Stream");

// Conectar fuente de video a la salida NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Conectar fuente de audio a la salida NDI
pipeline.Connect(audioSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

### Video Capture SDK

```cs
// Inicializar salida NDI con un nombre de flujo amigable para red
var ndiOutput = new NDIOutput("VisioForge_Studio_Output");

// Agregar la salida NDI configurada al pipeline de captura de video
core.Outputs_Add(ndiOutput); // core representa la instancia VideoCaptureCoreX
```

## Transmisión de Cámara a NDI

[MediaBlocksPipeline](#){ .md-button }

El caso de uso más común es transmitir una webcam local y micrófono a NDI. Este ejemplo usa el SDK Media Blocks para capturar desde dispositivos del sistema y enviar a NDI con el remuestreo de audio adecuado.

### Arquitectura del Pipeline

```text
SystemVideoSourceBlock → NDISinkBlock (entrada de video)
SystemAudioSourceBlock → AudioResamplerBlock (48kHz, F32LE, estéreo) → NDISinkBlock (entrada de audio)
```

### Ejemplo de Código

```cs
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.AudioProcessing;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.AudioEncoders;

// Inicializar SDK una vez al inicio
await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Enumerar dispositivos disponibles
var videoDevices = await DeviceEnumerator.Shared.VideoSourcesAsync();
var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync();

// Configurar fuente de video (primera cámara disponible)
var videoSettings = new VideoCaptureDeviceSourceSettings(videoDevices[0]);
var videoSource = new SystemVideoSourceBlock(videoSettings);

// Configurar fuente de audio (primer micrófono disponible)
var audioSettings = new AudioCaptureDeviceSourceSettings(audioDevices[0]);
var audioSource = new SystemAudioSourceBlock(audioSettings);

// Crear salida NDI
var ndiSink = new NDISinkBlock("My Camera Stream");

// Conectar video directamente a NDI
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Remuestrear audio a 48kHz F32LE estéreo (requerido por NDI)
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(audioSource.Output, audioResampler.Input);
pipeline.Connect(audioResampler.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

## Implementación NDI Específica de Windows

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Para implementaciones específicas de Windows, el SDK proporciona opciones de configuración adicionales a través de los componentes VideoCaptureCore o VideoEditCore.

### Guía de Implementación Paso a Paso

#### 1. Habilitar Transmisión de Red

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

#### 2. Configurar Transmisión de Audio

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

#### 3. Seleccionar Protocolo NDI

```csharp
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.NDI;
```

#### 4. Crear y Configurar Salida NDI

```cs
var streamName = "VisioForge NDI Streamer";
var ndiOutput = new NDIOutput(streamName);
```

#### 5. Asignar la Salida

```cs
VideoCapture1.Network_Streaming_Output = ndiOutput;
```

#### 6. Generar la URL NDI (Opcional)

```cs
string ndiUrl = $"ndi://{System.Net.Dns.GetHostName()}/{streamName}";
Debug.WriteLine(ndiUrl);
```

## Reproducción de Archivos a Salida NDI

El SDK Media Blocks puede transmitir archivos multimedia locales (MP4, MKV, AVI, etc.) directamente a NDI sin ninguna renderización local — ideal para aplicaciones de servidor de playout.

### Pipeline Recomendado

```text
UniversalSourceBlock (archivo)
  VideoOutput → NDISinkBlock (entrada de video)
  AudioOutput → AudioResamplerBlock (48kHz, F32LE, estéreo) → NDISinkBlock (entrada de audio)
```

### Ejemplo de Código

```cs
var pipeline = new MediaBlocksPipeline();

// Fuente de archivo con detección automática de formato
var fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(new Uri("file:///path/to/video.mp4")));

// Salida NDI
var ndiSink = new NDISinkBlock("My NDI Stream");

// Conectar video directamente a NDI
pipeline.Connect(fileSource.VideoOutput,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Remuestrear audio a 48kHz F32LE estéreo para compatibilidad NDI
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
pipeline.Connect(fileSource.AudioOutput, audioResampler.Input);
pipeline.Connect(audioResampler.Output,
    ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));

// Opcional: habilitar reproducción en bucle
pipeline.Loop = true;

await pipeline.StartAsync();
```

## Requisitos de Formato de Audio

NDI requiere audio **48kHz, punto flotante de 32 bits (F32LE), entrelazado**. Cuando transmita desde fuentes que puedan contener audio a otras frecuencias de muestreo (por ejemplo, AAC a 44.1kHz en archivos MP4, o frecuencias variables de micrófono), siempre incluya un `AudioResamplerBlock` para convertir a 48kHz. Sin remuestreo, el audio puede tartamudear, tener fallos o perder sincronización con el video.

```cs
var audioResampler = new AudioResamplerBlock(
    new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2));
```

## Recepción de Fuentes NDI

Esta guía se centra en enviar video y audio a NDI. Para descubrir, conectar, previsualizar o capturar fuentes NDI, incluidas aplicaciones Android y MAUI de reproducción NDI, consulte la [Referencia de Fuentes de Video NDI](../../videocapture/video-sources/ip-cameras/ndi.md).

## Solución de Problemas

### Tartamudeo o Fallos de Audio en Salida NDI

**Síntoma:** El audio no es fluido al transmitir desde archivos — tartamudea, tiene fallos o problemas de sincronización labial.

**Causa:** El archivo fuente contiene audio a una frecuencia de muestreo diferente de 48kHz (por ejemplo, AAC a 44.1kHz en archivos MP4). NDI espera audio a 48kHz.

**Solución:** Inserte un `AudioResamplerBlock` configurado para 48kHz F32LE estéreo entre la fuente de archivo y el sumidero NDI, como se muestra en el ejemplo de reproducción de archivos anterior.

## Preguntas Frecuentes

### ¿Cómo transmito video desde una cámara a NDI en C#?

Use el SDK Media Blocks para crear un pipeline con `SystemVideoSourceBlock` para la cámara, `SystemAudioSourceBlock` para el micrófono y `NDISinkBlock` como salida. Conecte el audio a través de un `AudioResamplerBlock` configurado a 48kHz F32LE estéreo, que NDI requiere. Consulte la sección [Transmisión de Cámara a NDI](#transmision-de-camara-a-ndi) para el código completo.

### ¿Qué formato de audio requiere NDI?

NDI requiere audio de 48kHz, punto flotante de 32 bits (F32LE), estéreo entrelazado. Siempre incluya un `AudioResamplerBlock(new AudioResamplerSettings(AudioFormatX.F32LE, 48000, 2))` en su pipeline entre la fuente de audio y el sumidero NDI. Sin el remuestreo adecuado, puede experimentar tartamudeo de audio, fallos o problemas de sincronización A/V.

### ¿Puedo transmitir un archivo de video a NDI para playout?

Sí. Use `UniversalSourceBlock` para leer el archivo, conecte el video directamente a `NDISinkBlock` y dirija el audio a través de `AudioResamplerBlock` para la conversión a 48kHz. Habilite `pipeline.Loop = true` para playout continuo. Este patrón es ideal para servidores de playout de transmisión con cero sobrecarga de renderización local.

### ¿Cuáles son los requisitos del sistema para transmisión NDI en .NET?

Necesita el [NDI SDK](https://ndi.video/for-developers/ndi-sdk/download/) o [NDI Tools](https://ndi.video/tools/) instalado para soporte NDI en tiempo de ejecución. El SDK de VisioForge soporta Windows, macOS y Linux. Verifique la disponibilidad de NDI en tiempo de ejecución con `NDISinkBlock.IsAvailable()`. Los requisitos de ancho de banda de red dependen de la resolución y la tasa de fotogramas — un flujo NDI HD típico usa aproximadamente 100-150 Mbps.

### ¿Cómo verifico si NDI está disponible en el sistema?

Llame a `NDISinkBlock.IsAvailable()` antes de crear componentes del pipeline NDI. Este método estático verifica si las bibliotecas de runtime NDI están instaladas y accesibles. Si devuelve `false`, solicite al usuario que instale el paquete NDI SDK o NDI Tools.

## Ver También

- [Referencia de Fuentes de Video NDI](../../videocapture/video-sources/ip-cameras/ndi.md) — recibir y capturar fuentes NDI en .NET
- [Visor de Streams RTSP y Reproductor de Cámaras IP](../../mediablocks/Guides/rtsp-player-csharp.md) — guía similar de streaming IP para cámaras RTSP
- [Guía de Despliegue](../../deployment-x/index.md) — paquetes de runtime específicos de plataforma para Windows, macOS, Linux
- [Ejemplos de Código en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK) — demos de transmisor y fuente NDI
- [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) — página del producto y descargas
