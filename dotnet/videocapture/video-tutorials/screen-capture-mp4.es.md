---
title: Captura de Pantalla a MP4 en C# — Video Capture SDK .NET
description: Grabación pantalla completa, región y multi-monitor. Codificación GPU (NVENC/QSV/AMF), audio loopback, resaltado de cursor. Ejemplos API legacy y moderna.
sidebar_label: Captura de Pantalla a MP4
---

# Captura de pantalla a archivo MP4

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fPJEoOz6lIM?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

## Redistribuibles requeridos

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## API Moderna — Video Capture SDK X

La API moderna multiplataforma usa `VideoCaptureCoreX` con captura de pantalla Direct3D 11 y Windows Graphics Capture (WGC). Esta aplicación de consola graba la pantalla completa a MP4 con audio del sistema opcional.

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
using VisioForge.Core.Types;
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
            // Configurar captura de pantalla Direct3D 11 con WGC
            var screenSource = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true,
                MonitorIndex = 0  // Monitor principal (-1 también selecciona el principal)
            };

            videoCapture.Video_Source = screenSource;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = false;

            // Configurar salida MP4 (H.264 + AAC, codificadores auto-seleccionados)
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");

            var mp4Output = new MP4Output(outputPath);
            videoCapture.Outputs_Add(mp4Output, autostart: true);

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

### Agregar Audio del Sistema (Loopback)

Para incluir el audio del escritorio en la grabación, agregue captura de loopback WASAPI2:

```csharp
// Enumerar dispositivos de salida WASAPI2 para captura de loopback
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
    AudioOutputDeviceAPI.WASAPI2);

if (audioOutputs.Length > 0)
{
    var loopbackSource = new LoopbackAudioCaptureDeviceSourceSettings(audioOutputs[0]);
    videoCapture.Audio_Source = loopbackSource;
    videoCapture.Audio_Record = true;
}
```

Para audio de micrófono en su lugar, use `DeviceEnumerator.Shared.AudioSourcesAsync()` — consulte la [guía de Captura de Audio](../audio-capture/index.md) para ejemplos completos.

## Codificación Acelerada por GPU

La codificación acelerada por hardware descarga la compresión H.264/HEVC a su GPU, reduciendo significativamente el uso de CPU durante grabaciones de alta resolución o alta tasa de fotogramas.

```csharp
// NVIDIA NVENC H.264
var mp4Output = new MP4Output(
    outputPath,
    new NVENCH264EncoderSettings());

// Intel Quick Sync Video H.264
var mp4Output = new MP4Output(
    outputPath,
    new QSVH264EncoderSettings());

// AMD AMF H.264
var mp4Output = new MP4Output(
    outputPath,
    new AMFH264EncoderSettings());
```

Los codificadores HEVC (H.265) también están disponibles para mejor compresión a la misma calidad: `NVENCHEVCEncoderSettings`, `QSVHEVCEncoderSettings`, `AMFHEVCEncoderSettings`. Si la codificación por hardware no está disponible, el SDK recurre automáticamente al codificador de software `OpenH264EncoderSettings` cuando usa el constructor predeterminado `new MP4Output(filename)`.

## Captura de Pantalla con Media Blocks SDK

El Media Blocks SDK usa un enfoque de pipeline donde conecta bloques de fuente, procesamiento y salida. Esto da control total sobre el flujo de datos y permite dividir el flujo de video a múltiples salidas simultáneamente.

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        await VisioForgeX.InitSDKAsync();

        var pipeline = new MediaBlocksPipeline();

        try
        {
            // Fuente de captura de pantalla
            var screenSettings = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true
            };
            var screenSource = new ScreenSourceBlock(screenSettings);

            // Salida MP4
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_mb_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            var mp4Sink = new MP4OutputBlock(outputPath);

            // Conectar fuente a salida
            pipeline.Connect(screenSource.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

            // Iniciar pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Grabando en: {outputPath}");
            Console.WriteLine("Presione ENTER para detener...");
            Console.ReadLine();

            await pipeline.StopAsync();
            Console.WriteLine("Grabación guardada.");
        }
        finally
        {
            await pipeline.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Captura de Pantalla Multiplataforma

El SDK soporta captura de pantalla en Windows, macOS y Linux con configuraciones de fuente específicas por plataforma:

| Plataforma | Método de Captura | Clase de Configuración | Requisitos |
|------------|-------------------|------------------------|------------|
| Windows | Direct3D 11 / WGC | `ScreenCaptureD3D11SourceSettings` | Windows 8+ (WGC: Windows 10 v1803+) |
| macOS | AVFoundation | `ScreenCaptureMacOSSourceSettings` | macOS 10.15+ (permiso de grabación de pantalla) |
| Linux | X11 / XDisplay | `ScreenCaptureXDisplaySourceSettings` | Servidor X11 |

En Windows, el SDK auto-selecciona WGC cuando está disponible, recurriendo a DXGI Desktop Duplication en sistemas más antiguos. Puede forzar una API específica:

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.DXGI  // Forzar Desktop Duplication en lugar de WGC
};
```

## API Legada — Video Capture SDK

### Ejemplo de Código

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_mp4
{
    public partial class Form1 : Form
    {
        // Componente principal de VideoCapture que maneja todas las operaciones de grabación
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Inicia la grabación de pantalla con audio del dispositivo predeterminado
        /// </summary>
        private async void btStartWithAudio_Click(object sender, EventArgs e)
        {
            // Configurar captura de pantalla para grabar toda la pantalla
            // ScreenCaptureSourceSettings permite control fino de la región de captura y parámetros
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true  // Captura toda la pantalla en lugar de una región específica
            };

            // Configurar captura de audio seleccionando el primer dispositivo de entrada de audio disponible
            // Audio_CaptureDevices() retorna todos los micrófonos y entradas de audio conectados
            // Seleccionamos el primer dispositivo (índice 0) en la colección
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(
                videoCapture1.Audio_CaptureDevices()[0].Name);

            // Deshabilitar monitoreo/reproducción de audio durante la grabación para prevenir retroalimentación
            // Esto significa que no escucharemos el audio capturado mientras grabamos
            videoCapture1.Audio_PlayAudio = false;

            // Habilitar grabación de audio para incluir sonido en el archivo de salida
            videoCapture1.Audio_RecordAudio = true;

            // Establecer la ubicación del archivo de salida en la carpeta Videos del usuario
            // Environment.GetFolderPath asegura que la ruta funcione en diferentes sistemas Windows
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Usar formato contenedor MP4 con codecs de video H.264 y audio AAC (formato estándar)
            // MP4Output puede configurarse más con parámetros de codificación personalizados si es necesario
            videoCapture1.Output_Format = new MP4Output();
            
            // Establecer el modo de captura a grabación de pantalla
            // Otros modos incluyen captura de cámara, procesamiento de archivo de video, etc.
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Comenzar el proceso de captura de forma asíncrona
            // Usando patrón async/await para prevenir congelamiento de UI durante la operación
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Inicia la grabación de pantalla sin audio (solo video)
        /// </summary>
        private async void btStartWithoutAudio_Click(object sender, EventArgs e)
        {
            // Configurar captura de pantalla para grabación a pantalla completa
            // Misma ScreenCaptureSourceSettings que en la grabación con audio
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true 
            };

            // Deshabilitar tanto reproducción como grabación de audio en una sola línea
            // Esto crea un archivo MP4 solo de video sin pista de audio
            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;

            // Establecer ruta de archivo de salida al directorio Videos del usuario con extensión MP4
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Configurar salida como MP4 (codec de video H.264)
            videoCapture1.Output_Format = new MP4Output();
            
            // Establecer modo a captura de pantalla
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Iniciar grabación de pantalla de forma asíncrona
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Detiene el proceso de grabación actual de forma segura
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Detener la grabación de forma asíncrona
            // Esto finaliza correctamente el archivo MP4 y libera recursos
            // Usar async asegura que la UI permanezca receptiva durante la finalización del archivo
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Inicializa el componente VideoCapture cuando el formulario se carga
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Inicializar el componente de captura de video y conectarlo a un control de vista previa de video
            // VideoView1 debe ser un control en tu formulario que implemente la interfaz IVideoView
            // Esto permite vista previa en vivo de la captura cuando se desee
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Cómo Funciona

Esta aplicación de Windows Forms demuestra la funcionalidad de captura de pantalla con y sin audio usando VisioForge Video Capture SDK:

1. **Configuración**: El objeto `VideoCaptureCore` se inicializa en el evento de carga del formulario, conectándolo a un componente de vista de video.

2. **Captura con Audio**:
   - Establece captura de pantalla a modo de pantalla completa
   - Selecciona el primer dispositivo de audio disponible para grabación
   - Deshabilita reproducción de audio pero habilita grabación de audio
   - Establece el archivo de salida a formato MP4 en la carpeta Videos del usuario
   - Usa método asíncrono para iniciar captura

3. **Captura sin Audio**:
   - Similar a lo anterior pero deshabilita tanto reproducción como grabación de audio
   - Usa el mismo formato de salida MP4 y modo de captura

4. **Deteniendo la Captura**:
   - Proporciona un método simple de detención que detiene la grabación de forma asíncrona

La aplicación demuestra cómo configurar diferentes escenarios de captura con código mínimo usando la interfaz fluida del SDK y patrones async.

### Configuración de Audio

El ejemplo de código anterior captura audio del micrófono seleccionando el primer dispositivo disponible. Puede seleccionar un dispositivo de audio específico por nombre, incluyendo dispositivos de audio del sistema (loopback) para capturar el sonido del escritorio:

```csharp
// Seleccionar un dispositivo de loopback (ej. "Stereo Mix") para captura de audio del sistema
var devices = videoCapture1.Audio_CaptureDevices();
var loopbackDevice = devices.FirstOrDefault(d => d.Name.Contains("Stereo Mix"));

if (loopbackDevice != null)
{
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(loopbackDevice.Name);
}
else
{
    // Alternativa: usar el primer dispositivo de audio disponible
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(devices[0].Name);
}

// Habilitar grabación, deshabilitar reproducción para prevenir retroalimentación
videoCapture1.Audio_RecordAudio = true;
videoCapture1.Audio_PlayAudio = false;
```

Para crear una grabación silenciosa sin pista de audio, deshabilite ambas propiedades de audio:

```csharp
videoCapture1.Audio_PlayAudio = false;
videoCapture1.Audio_RecordAudio = false;
```

### Captura de Región

En lugar de grabar la pantalla completa, capture un área rectangular específica estableciendo `FullScreen = false` y proporcionando coordenadas en píxeles:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = false,
    Left = 100,
    Top = 100,
    Right = 1380,
    Bottom = 820
};
```

Las coordenadas definen el rectángulo de captura en píxeles de pantalla. Esto es útil para grabar una ventana de aplicación específica o una porción del escritorio.

### Grabación Multi-Monitor

Seleccione qué pantalla grabar usando la propiedad `DisplayIndex`. El monitor principal es el índice `0`, el secundario es `1`, y así sucesivamente:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Grabar el monitor secundario
};
```

### Configuración de Calidad de Grabación

Personalice la salida MP4 configurando el codificador H.264. El bitrate predeterminado es 3500 kbps, que produce aproximadamente 25 MB por minuto a 1080p:

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Mayor calidad (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Mejor compresión que Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Bitrate variable
videoCapture1.Output_Format = mp4Output;
```

Para codificación acelerada por GPU, consulte la sección [Codificación Acelerada por GPU](#codificacion-acelerada-por-gpu) anterior.

### Opciones del Cursor del Mouse

Incluya el cursor del mouse en la grabación y opcionalmente agregue un efecto de resaltado para screencasts estilo tutorial:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    GrabMouseCursor = true,
    MouseHighlight = true,
    MouseHighlightColor = System.Drawing.Color.Yellow,
    MouseHighlightRadius = 40,
    MouseHighlightOpacity = 0.4
};
```

El resaltado dibuja un círculo translúcido alrededor del cursor, facilitando que los espectadores sigan los movimientos del mouse.

## Preguntas Frecuentes

### ¿Qué licencia necesito para una aplicación de captura de pantalla en C#?

Video Capture SDK .Net requiere una licencia para desarrollo y distribución. Una licencia de Desarrollador elimina la marca de agua de evaluación y desbloquea todas las funciones durante el desarrollo. Una licencia de Release es necesaria al distribuir su aplicación a los usuarios finales. El SDK está disponible en edición Premium que incluye todos los modos de captura, salida MP4/AVI/WMV y los motores DirectShow y GStreamer. Puede evaluar el SDK sin licencia — la captura de pantalla funciona completamente pero incluye una marca de agua superpuesta. Visite la [página del producto](https://www.visioforge.com/video-capture-sdk-net) para conocer precios y opciones de licencia.

### ¿Cómo controlo la calidad de grabación y el tamaño de archivo para la captura de pantalla MP4?

Configure el bitrate de video de `MP4Output` para equilibrar calidad y tamaño de archivo. El valor predeterminado es 3500 kbps, que funciona bien para la mayoría de las grabaciones de pantalla. Reduzca el bitrate a 1500–2000 kbps para archivos más pequeños, o aumente a 5000–8000 kbps para capturas de alta calidad. Use `H264Profile.ProfileMain` o `ProfileHigh` en lugar de `ProfileBaseline` para mejor compresión a la misma calidad. La tasa de fotogramas también afecta el tamaño del archivo — 15 FPS es suficiente para presentaciones, mientras que 30 FPS es mejor para demos de software. Una grabación de 1080p a 3500 kbps produce aproximadamente 25 MB por minuto.

### ¿Puedo capturar el audio del sistema (sonido del escritorio) en lugar de la entrada del micrófono?

Sí. Llame a `Audio_CaptureDevices()` para enumerar todos los dispositivos de audio disponibles, luego seleccione un dispositivo de loopback o audio del sistema (como "Stereo Mix" o "What U Hear") por nombre. Establezca `Audio_RecordAudio = true` y `Audio_PlayAudio = false`. Los nombres de dispositivos de loopback disponibles dependen de su hardware de audio y controladores. Puede grabar tanto el micrófono como el audio del sistema simultáneamente configurando fuentes de audio adicionales.

### ¿Cómo grabo un monitor específico en una configuración multi-monitor?

Establezca la propiedad `DisplayIndex` en `ScreenCaptureSourceSettings` — use `0` para el monitor principal, `1` para el secundario, y así sucesivamente. Combine con `FullScreen = true` para capturar toda la pantalla seleccionada. También puede establecer `FullScreen = false` con coordenadas `Left/Top/Right/Bottom` para capturar una región específica en el monitor elegido.

### ¿Qué tasa de fotogramas debo usar para la grabación de pantalla?

La tasa de fotogramas predeterminada es 10 FPS. Use 15 FPS para presentaciones, diapositivas y contenido mayormente estático. Use 25–30 FPS para tutoriales de software y demostraciones de UI donde el movimiento suave del mouse importa. Use 60 FPS para grabación de juegos o contenido con mucho movimiento. Tasas de fotogramas más altas aumentan el tamaño del archivo y el uso de CPU proporcionalmente. Establezca la tasa de fotogramas a través de `ScreenCaptureSourceSettings.FrameRate`.

## Ver También

- [Captura de Pantalla a AVI](screen-capture-avi.md) — grabar pantalla a formato AVI con video sin comprimir o MJPEG
- [Captura de Pantalla a WMV](screen-capture-wmv.md) — grabar pantalla a formato Windows Media
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — aplicación de grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para región de captura, multi-monitor, cursor y configuración de tasa de fotogramas
- [Guardar Video de Webcam](../guides/save-webcam-video.md) — captura de webcam a MP4 con configuración de audio
- [Ejemplos de Código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — ejemplos adicionales de captura de pantalla en GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
