---
title: Capturar Audio del Sistema y Grabar Micrófono en C# .NET
description: Grabe audio de micrófono y capture sonido del sistema (loopback) en C# con VisioForge SDK. Ejemplos de grabación de audio a MP3, M4A, WAV.
sidebar_label: Captura de Audio
order: 10
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureCoreX
  - LoopbackAudioCaptureDeviceSourceSettings
  - SystemAudioSourceBlock
  - MediaBlocksPipeline

---

# Captura de Audio y Grabación de Sonido del Sistema en C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción

El VisioForge Video Capture SDK proporciona capacidades de captura de audio para desarrolladores .NET, cubriendo grabación de micrófono, captura de audio del sistema (loopback/altavoz) y grabación de audio combinada. Ya sea que esté construyendo un grabador de podcast, una herramienta de grabación de pantalla con audio o una aplicación de captura de voz, el SDK maneja la enumeración de dispositivos, negociación de formato y codificación.

Esta guía proporciona ejemplos de código completos y ejecutables para los escenarios más comunes de captura de audio usando las APIs de **Video Capture SDK X** y **Media Blocks SDK**.

## Fuentes de Audio Soportadas

- **Micrófonos físicos** — Micrófonos de escritorio, USB y Bluetooth
- **Puertos de entrada de línea** — Mezcladores externos o instrumentos
- **Audio del sistema (loopback)** — Grabe lo que se reproduce a través de sus altavoces o auriculares
- **Dispositivos de audio virtuales** — Capture audio de otras aplicaciones
- **Flujos de red** — Audio de RTSP, HTTP y otras fuentes de streaming

Para configuración detallada de fuentes, consulte [Fuentes de Audio](../audio-sources/index.md).

## Soporte de Formatos de Audio

### Formatos con Pérdida

- [MP3](../../general/audio-encoders/mp3.md) — Estándar de la industria, tasas de bits ajustables de 8 kbps a 320 kbps
- [M4A (AAC)](../../general/audio-encoders/aac.md) — Excelente relación calidad-tamaño
- [Windows Media Audio](../../general/audio-encoders/wma.md) — Buena compresión con integración Windows
- [Ogg Vorbis](../../general/audio-encoders/vorbis.md) — Código abierto, excelente calidad a tasas de bits bajas
- [Speex](../../general/audio-encoders/speex.md) — Optimizado para voz

### Formatos sin Pérdida

- [WAV](../../general/audio-encoders/wav.md) — Sin compresión, calidad perfecta
- [FLAC](../../general/audio-encoders/flac.md) — Compresión sin pérdida de calidad

## Grabar Audio de Micrófono a MP3

Esta aplicación de consola graba audio del micrófono predeterminado y lo guarda como archivo MP3.

### Paquetes NuGet Requeridos

```bash
dotnet add package VisioForge.DotNet.Core.TRIAL
dotnet add package VisioForge.DotNet.VideoCapture.TRIAL
```

Agregue el [paquete de redistribución](../../deployment-x/index.md) para su plataforma (por ejemplo, `VisioForge.DotNet.Redist.Base.Windows.x64`).

### Ejemplo Completo

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Enumerar dispositivos de captura de audio
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No se encontró dispositivo de captura de audio.");
                return;
            }

            // Mostrar dispositivos disponibles
            Console.WriteLine("Dispositivos de audio disponibles:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            // Seleccionar primer dispositivo (micrófono predeterminado)
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSource = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configurar captura solo de audio
            videoCapture.Audio_Source = audioSource;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configurar salida MP3
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"mic_recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");

            var mp3Output = new MP3Output(outputPath);
            videoCapture.Outputs_Add(mp3Output, autostart: true);

            // Iniciar grabación
            await videoCapture.StartAsync();
            Console.WriteLine($"Grabando en: {outputPath}");
            Console.WriteLine("Presione ENTER para detener...");
            Console.ReadLine();

            // Detener y guardar
            await videoCapture.StopAsync();
            Console.WriteLine("Grabación guardada.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Capturar Audio del Sistema (Altavoz / Loopback)

La captura de audio del sistema (también llamada loopback o captura de altavoz) graba cualquier sonido que se reproduce a través del dispositivo de salida de su computadora. Esto se usa comúnmente para grabación de pantalla con audio, captura de llamadas de conferencia o grabación de audio en streaming.

En Windows, la captura de loopback usa la API **WASAPI2** para acceder a los dispositivos de salida.

### Ejemplo Completo — Video Capture SDK X

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Enumerar dispositivos de salida de audio WASAPI2 (altavoces/auriculares)
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No se encontró dispositivo de salida de audio WASAPI2.");
                return;
            }

            // Mostrar fuentes de loopback disponibles
            Console.WriteLine("Dispositivos de loopback disponibles:");
            for (int i = 0; i < audioOutputs.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioOutputs[i].Name}");
            }

            // Seleccionar el primer dispositivo
            var outputDevice = audioOutputs[0];

            // Crear configuración de fuente loopback
            var audioSource = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            videoCapture.Audio_Source = audioSource;

            // Configurar para captura solo de audio
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;
            videoCapture.Video_Play = false;

            // Configurar salida M4A (AAC)
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var m4aOutput = new M4AOutput(outputPath);
            videoCapture.Outputs_Add(m4aOutput, autostart: true);

            // Iniciar captura de audio del sistema
            await videoCapture.StartAsync();
            Console.WriteLine($"Capturando audio del sistema en: {outputPath}");
            Console.WriteLine("Reproduzca algo de audio en su computadora, luego presione ENTER para detener...");
            Console.ReadLine();

            // Detener y guardar
            await videoCapture.StopAsync();
            Console.WriteLine("Grabación guardada.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Ejemplo Completo — Media Blocks SDK

El Media Blocks SDK usa un enfoque de pipeline donde conecta bloques de fuente, procesamiento y salida. Este ejemplo captura audio del sistema usando `SystemAudioSourceBlock` y lo guarda en un archivo M4A.

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        // Inicializar SDK
        await VisioForgeX.InitSDKAsync();

        MediaBlocksPipeline pipeline = null;

        try
        {
            // Obtener el primer dispositivo de salida WASAPI2 para captura de loopback
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No se encontró dispositivo de salida de audio WASAPI2.");
                return;
            }

            var outputDevice = audioOutputs[0];
            Console.WriteLine($"Usando dispositivo de loopback: {outputDevice.Name}");

            // Crear pipeline
            pipeline = new MediaBlocksPipeline();

            // Crear fuente de audio loopback
            var sourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            var audioSource = new SystemAudioSourceBlock(sourceSettings);

            // Crear salida M4A
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var output = new M4AOutputBlock(outputPath);

            // Conectar fuente a salida
            pipeline.Connect(audioSource, output);

            // Iniciar pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Capturando audio del sistema en: {outputPath}");
            Console.WriteLine("Presione ENTER para detener...");
            Console.ReadLine();

            // Detener pipeline
            await pipeline.StopAsync();
            Console.WriteLine("Grabación guardada.");
        }
        finally
        {
            if (pipeline != null)
            {
                await pipeline.DisposeAsync();
            }

            VisioForgeX.DestroySDK();
        }
    }
}
```

## Características Clave

### Control de Dispositivos

- Enumerar todos los dispositivos de entrada y salida de audio disponibles
- Seleccionar dispositivos de entrada específicos programáticamente
- Establecer niveles de volumen de entrada y estado de silencio
- Monitorear niveles de audio en tiempo real con [medidores VU](../../general/code-samples/vu-meters.md)
- Selección automática de dispositivos predeterminados del sistema

### Procesamiento Avanzado

- Visualización de audio en tiempo real con análisis de espectro y forma de onda
- Reducción de ruido y cancelación de eco
- Control de ganancia y normalización
- Detección de actividad de voz (VAD)
- Gestión de canales estéreo/mono
- Conversión de frecuencia de muestreo

### Controles de Grabación

- Iniciar, pausar, reanudar y detener grabación
- Gestión de búfer para operación de baja latencia
- Grabaciones temporizadas con parada automática
- División de archivos para grabaciones grandes
- Nombramiento automático de archivos con marcas de tiempo

## Notas Multiplataforma

| Plataforma | Micrófono | Audio del Sistema (Loopback) |
| ---------- | --------- | ---------------------------- |
| Windows | DirectSound, WASAPI2 | Loopback WASAPI2 |
| macOS | CoreAudio | No disponible vía SDK |
| Linux | PulseAudio, ALSA | Monitor PulseAudio |

La captura de loopback de audio del sistema es principalmente una característica de Windows usando la API WASAPI2. En Linux, los dispositivos de monitor PulseAudio pueden proporcionar funcionalidad similar.

## Mejores Prácticas

1. **Verifique la disponibilidad del dispositivo** antes de iniciar la captura — los dispositivos pueden desconectarse en cualquier momento
2. **Monitoree los niveles de audio** durante la grabación para detectar silencio o saturación
3. **Elija el formato correcto** — MP3/M4A para salida comprimida, WAV para máxima calidad, FLAC para compresión sin pérdida
4. **Use WASAPI2** para captura de loopback en Windows — proporciona la menor latencia y la captura de audio del sistema más confiable
5. **Maneje errores con elegancia** — implemente manejo de errores para eventos de desconexión de dispositivos
6. **Pruebe en el hardware objetivo** — el comportamiento de los dispositivos de audio varía entre sistemas

## Aplicaciones de Ejemplo

Ejemplos de trabajo completos están disponibles en GitHub:

- [Captura de Altavoz — Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/speaker-capture)
- [Captura de Altavoz — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_CodeSnippets/speaker-capture)
- [Demo de Captura de Audio — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)

## Documentación Relacionada

- [Fuentes de Audio — Configuración de Dispositivos](../audio-sources/index.md)
- [Renderizado de Audio — Reproducción](../audio-rendering/index.md)
- [Grabación y Edición WMA](../../general/guides/wma-recording-editing.md)
- [Captura de Pantalla con Audio](../video-tutorials/screen-capture-mp4.md)
