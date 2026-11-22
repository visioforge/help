---
title: Configuración de Salida FFMPEG.exe en .NET Video SDKs
description: Configure salida FFMPEG.exe en .NET para captura y edición de video con aceleración por hardware, códecs personalizados y opciones de codificación profesional.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Encoding
  - Editing
  - MP4
  - WebM
  - AVI
  - MOV
  - TS
  - H.264
  - H.265
  - VP9
  - Opus
  - Webcam
  - WinForms
  - ProRes
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEditCore
  - FFMPEGEXEOutput
  - H264MFSettings
  - H264NVENCSettings
  - HEVCMFSettings
  - BasicVideoSettings
  - BasicAudioSettings
  - VideoCaptureSource
video:
  id: bNn7p2H6i3M
  upload_date: 2026-11-17T06:59:13Z
  duration: PT13M6S

---

# Integración de FFMPEG.exe con los SDK .NET de VisioForge

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

!!! note "Edición"
    La salida FFmpeg.exe requiere la edición Premium tanto en `VideoCaptureCore` como en `VideoEditCore`. Con una licencia Standard o Professional, el trabajo se ejecuta en modo de prueba, con la marca de agua de prueba, solo mientras duran los 30 días de prueba; después, `Start` devuelve `false`.

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/bNn7p2H6i3M?controls=1" title="Grabar una webcam en MP4 y WebM con FFmpeg en C# - tutorial de YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)

El video crea el proyecto con .NET 10, que requiere Visual Studio 2026; en Visual Studio 2022, elija .NET 8 o .NET 9.

## Introducción a salida FFMPEG en .NET

Esta guía proporciona instrucciones detalladas para implementar salida FFMPEG.exe en aplicaciones Windows usando los SDK .NET de VisioForge. La integración funciona tanto con [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) como con [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net), utilizando los motores `VideoCaptureCore` y `VideoEditCore`.

FFMPEG funciona como un poderoso framework multimedia que permite a los desarrolladores generar salida a una amplia variedad de formatos de video y audio. Su flexibilidad proviene del extenso soporte de códecs y control granular sobre parámetros de codificación tanto para flujos de video como de audio.

## ¿Por qué usar FFMPEG con los SDK de VisioForge?

Integrar FFMPEG en sus aplicaciones potenciadas por VisioForge proporciona varias ventajas técnicas:

- **Versatilidad de formatos**: Soporte para virtualmente todos los formatos de contenedor modernos
- **Flexibilidad de códecs**: Acceso a códecs tanto de código abierto como propietarios
- **Optimización de rendimiento**: Opciones para aceleración CPU y GPU
- **Profundidad de personalización**: Control detallado sobre parámetros de codificación

## Características y capacidades clave

### Formatos de salida soportados

FFMPEG soporta numerosos formatos de contenedor, incluyendo pero no limitado a:

- MP4 (MPEG-4 Part 14)
- WebM (VP8/VP9 con Vorbis/Opus)
- MKV (Matroska)
- AVI (Audio Video Interleave)
- MOV (QuickTime)
- WMV (Windows Media Video)
- FLV (Flash Video)
- TS (MPEG Transport Stream)

### Opciones de aceleración de hardware

La codificación de video moderna se beneficia de tecnologías de aceleración de hardware que mejoran significativamente la velocidad y eficiencia de codificación:

- **Intel QuickSync**: Aprovecha gráficos integrados Intel para codificación H.264 y HEVC
- **NVIDIA NVENC**: Utiliza GPUs NVIDIA para codificación acelerada (requiere tarjeta gráfica NVIDIA compatible)
- **AMD AMF/VCE**: Emplea procesadores gráficos AMD para aceleración de codificación

### Soporte de códecs de video

La integración ofrece acceso a múltiples códecs de video con parámetros personalizables:

- **H.264/AVC**: Estándar de la industria con excelente relación calidad-tamaño
- **H.265/HEVC**: Códec de mayor eficiencia para contenido 4K+
- **VP9**: Códec de video abierto de Google usado en WebM
- **MPEG-2**: Códec heredado para compatibilidad con DVD y transmisión
- **ProRes**: Códec profesional para flujos de trabajo de edición

## Proceso de implementación

### 1. Configuración de su entorno de desarrollo

Antes de implementar salida FFMPEG, asegúrese de que su entorno de desarrollo esté correctamente configurado:

1. Cree un nuevo proyecto .NET o abra uno existente
2. Instale los paquetes NuGet apropiados del SDK de VisioForge
3. Agregue los paquetes de dependencia FFMPEG (detallados en la sección Dependencias)
4. Importe los namespaces necesarios en su código:

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.Types.Output;     // FFMPEGEXEOutput
using VisioForge.Core.Types.FFMPEGEXE;  // H264MFSettings, H264NVENCSettings, H264QSVSettings, HEVCQSVSettings, OutputMuxer, BasicAudioSettings, ...
```

### 2. Inicialización de salida FFMPEG

Comience creando una instancia de `FFMPEGEXEOutput` para manejar su configuración de salida:

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
```

Este objeto servirá como el contenedor para todos sus ajustes y preferencias de codificación.

### 3. Configuración del formato de contenedor de salida

Establezca su formato de contenedor de salida deseado usando la propiedad `OutputMuxer`:

```csharp
ffmpegOutput.OutputMuxer = OutputMuxer.MP4;
```

Otras opciones comunes de contenedor incluyen:

- `OutputMuxer.Matroska` - Para contenedor Matroska (.mkv)
- `OutputMuxer.WebM` - Para formato WebM
- `OutputMuxer.AVI` - Para formato AVI
- `OutputMuxer.MOV` - Para contenedor QuickTime

### 4. Configuración del codificador de video

FFMPEG proporciona múltiples opciones de codificador de video. Seleccione y configure el codificador apropiado según sus requisitos y hardware disponible:

#### Codificación H.264 estándar basada en CPU

```csharp
var videoEncoder = new H264MFSettings
{
    Bitrate     = 5000000, // 5 Mbps
    RateControl = H264MFRateControl.CBR,
};
ffmpegOutput.Video = videoEncoder;
```

#### Codificación NVIDIA acelerada por hardware

```csharp
var nvidiaEncoder = new H264NVENCSettings
{
    Bitrate = 8000000,        // 8 Mbps
};
ffmpegOutput.Video = nvidiaEncoder;
```

#### Codificación Intel QuickSync acelerada por hardware

```csharp
var intelEncoder = new H264QSVSettings
{
    Bitrate = 6000000
};
ffmpegOutput.Video = intelEncoder;
```

#### Codificación HEVC/H.265 para mayor eficiencia

```csharp
var hevcEncoder = new HEVCQSVSettings
{
    Bitrate = 3000000,  
};
ffmpegOutput.Video = hevcEncoder;
```

### 5. Configuración del codificador de audio

Configure sus ajustes de codificación de audio según requisitos de calidad y compatibilidad de plataforma objetivo:

```csharp
var audioEncoder = new BasicAudioSettings
{
    Bitrate = 192000,    // 192 kbps
    Channels = 2,        // Estéreo
    SampleRate = 48000,  // 48 kHz - estándar profesional
    Encoder = AudioEncoder.AAC,
    Mode = AudioMode.CBR
};

ffmpegOutput.Audio = audioEncoder;
```

### 6. Configuración final y ejecución

Aplique todos los ajustes e inicie el proceso de codificación:

```csharp
// Aplicar configuración de formato
core.Output_Format = ffmpegOutput;

// Establecer modo de operación
core.Mode = VideoCaptureMode.VideoCapture;  // Para Video Capture SDK
// core.Mode = VideoEditMode.Convert;       // Para Video Edit SDK

// Establecer ruta de salida
core.Output_Filename = "output.mp4";

// Comenzar procesamiento
await core.StartAsync();
```

## Dependencias requeridas

Instale los siguientes paquetes NuGet según su arquitectura objetivo para asegurar funcionalidad apropiada:

### Dependencias de Video Capture SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

Para objetivos x86:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

### Dependencias de Video Edit SDK

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

Para objetivos x86:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

## Solución de problemas y optimización

### Problemas comunes y soluciones

- **Errores de códec no encontrado**: un codificador que el ejecutable de FFmpeg no incluye hace fallar `Start` / `StartAsync`, y `OnError` indica el codificador y el ejecutable. El `vfffmpeg64.exe` incluido no tiene libx264 ni libx265: use `H264MFSettings`, `OpenH264Settings` o un codificador por hardware, o asigne a `ExecutableFilename` una compilación de FFmpeg que los tenga. `ffmpeg -encoders` muestra lo que incluye una compilación
- **Fallos de aceleración de hardware**: un codificador QSV, NVENC o AMF en un PC sin esa GPU arranca y después FFmpeg termina; `OnError` lo notifica con la última línea de error de FFmpeg. Verifique compatibilidad de GPU y versiones de controladores
- **Problemas de rendimiento**: Ajuste conteo de hilos y preset de codificación según recursos de CPU disponibles
- **Problemas de calidad de salida**: Ajuste tasa de bits, perfil y parámetros de codificación

### Consejos de optimización de rendimiento

- Use aceleración de hardware cuando esté disponible
- Elija presets apropiados según sus requisitos de calidad/velocidad
- Establezca tasas de bits razonables según el tipo de contenido y resolución
- Considere codificación de dos pasadas para escenarios no en tiempo real que requieren máxima calidad

## Código del tutorial: webcam a MP4 y WebM

El `Form1.cs` completo del video. El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato, la velocidad de fotogramas, el micrófono y la salida, y los botones Start y Stop; Stop empieza deshabilitado. El archivo del diseñador está en el código fuente en GitHub. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()`, que .NET Framework no genera, por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`, y ponga `<dpiAware>true</dpiAware>` en un `app.manifest` para conservar la compatibilidad con el DPI del sistema que activa .NET 10.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.FFMPEGEXE;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamFfmpeg;

public partial class Form1 : Form
{
    private const string Mp4H264 = "MP4 - H.264 + AAC";
    private const string Mp4Hevc = "MP4 - HEVC + AAC";
    private const string WebmVp9 = "WebM - VP9 + Opus";

    private VideoCaptureCore videoCapture1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        cbOutput.Items.Add(Mp4H264);
        cbOutput.Items.Add(WebmVp9);

        // HEVC needs a hardware encoder: an Intel, NVIDIA or AMD GPU
        var hw = VideoCaptureCore.HWEncodersAvailable();
        if (hw.QSV_H265 || hw.NVENC_H265 || hw.AMD_H265)
            cbOutput.Items.Add(Mp4Hevc);

        cbOutput.SelectedIndex = 0;
    }

    private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        var format = device.VideoFormats.First(f => f.Name == cbFormat.Text);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbFrameRate.SelectedItem is null)
        {
            Text = "No camera, format or frame rate";
            return;
        }

        Text = "Webcam to MP4 and WebM with FFmpeg";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        var ffmpeg = new FFMPEGEXEOutput();
        string extension;
        switch (cbOutput.Text)
        {
            case Mp4Hevc:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new HEVCMFSettings
                {
                    ForceHWEncoding = true, // the GPU's encoder
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 6_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;

            case WebmVp9:
                ffmpeg.OutputMuxer = OutputMuxer.WebM;
                ffmpeg.Video = new BasicVideoSettings
                {
                    Encoder = VideoEncoder.VP9,
                    BasicMode = VideoMode.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = new BasicAudioSettings
                {
                    Encoder = AudioEncoder.OPUS,
                    Mode = AudioMode.CBR,
                    Bitrate = 128_000,
                    SampleRate = 0, // 0: keep the microphone's own
                    Channels = 0,
                };
                extension = ".webm";
                break;

            default:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new H264MFSettings
                {
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;
        }

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output" + extension);
        videoCapture1.Output_Format = ffmpeg;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private static BasicAudioSettings Aac() => new()
    {
        Encoder = AudioEncoder.AAC,
        Mode = AudioMode.CBR,
        Bitrate = 192_000,
        SampleRate = 0, // 0: keep the microphone's own
        Channels = 0,
    };

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finish the file before the window goes away
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // already starting or stopping

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // close again once this handler has returned
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Recursos adicionales

- **[Ejemplo del tutorial de salida FFmpeg (Video Capture SDK, WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)** — la aplicación del video: una webcam y su micrófono a MP4 con H.264 y AAC o a WebM con VP9 y Opus, y a MP4 con HEVC cuando el PC tiene un codificador HEVC por hardware de Intel, NVIDIA o AMD.

Para más ejemplos de código e implementación, visite nuestro [repositorio de GitHub](https://github.com/visioforge/.Net-SDK-s-samples).

Para aprender más sobre parámetros y capacidades de FFMPEG, consulte la [documentación oficial de FFMPEG](https://ffmpeg.org/documentation.html).

## Transcripción del video

### Qué vamos a construir

Vamos a grabar una webcam en archivos MP4 y WebM a través de FFmpeg, en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una ventana WinForms con una vista previa en directo de la cámara. Graba archivos MP4 con video H.264 y audio AAC, y archivos WebM con video VP9 y audio Opus. En un PC con un codificador de GPU para HEVC, también graba HEVC en MP4. Los tres salen de una sola clase de salida: FFMPEGEXEOutput. Esta es la aplicación terminada, grabando desde una Logitech C920.

### Por qué FFmpeg

¿Por qué FFmpeg? El SDK captura la cámara y el micrófono, y entrega sus fotogramas a FFmpeg. FFmpeg los codifica y escribe el archivo. Así, una sola clase de salida cubre muchos contenedores y códecs. FFmpeg viene en un paquete NuGet, así que no hay que instalar nada más. FFMPEGEXEOutput requiere la edición Premium del SDK, y la versión de prueba lo ejecuta durante treinta días.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una webcam con micrófono. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamFfmpeg. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet: el propio SDK, el redist de captura de video con sus filtros DirectShow y el redist de FFmpeg. Este último coloca FFmpeg junto a su aplicación. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. Y el redist de FFmpeg para x64. Los dos redist son los de x64, así que establezca PlatformTarget en x64 en el archivo de proyecto. Una aplicación x86 usa en su lugar los redists x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un combo box para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara, la velocidad de fotogramas, el micrófono y la salida. Añada los botones Start y Stop. Stop queda desactivado hasta que empieza una grabación. Haga doble clic en el formulario, en las listas de cámara y formato, y en los dos botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. La configuración de FFmpeg está en VisioForge.Core.Types.FFMPEGEXE. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título.

### Listar los dispositivos

Video_CaptureDevices devuelve todas las cámaras, y Audio_CaptureDevices todos los micrófonos. Listamos sus nombres y seleccionamos el primero de cada lista. Cuando cambia la cámara, su manejador llena la lista de formatos con los formatos de esa cámara, y elige 1920x1080 MJPG si existe. Cuando cambia el formato, su manejador llena la lista de velocidades de fotogramas del mismo modo, y elige treinta fotogramas por segundo.

### Listar las salidas

De vuelta en el manejador Load, la lista de salidas recibe sus entradas. H.264 y WebM funcionan en cualquier PC. HEVC necesita un codificador por hardware, así que HWEncodersAvailable nos dice si una GPU Intel, NVIDIA o AMD tiene uno, y solo entonces HEVC aparece en la lista. Esto es lo que usa el redist de FFmpeg para codificar. H.264 usa el codificador de Media Foundation que viene con Windows. VP9 y Opus son códecs abiertos para WebM, codificados en la CPU. AAC es el audio para MP4. HEVC pasa por el codificador de Media Foundation de la GPU, por eso el código establece ForceHWEncoding. En un PC sin él, HEVC queda fuera de la lista.

### Elegir el formato de la cámara

El controlador de Start comprueba primero que haya una velocidad de fotogramas seleccionada. Si no, lo indica en la barra de título. Después selecciona la cámara por su nombre, desactiva Format_UseBest y pasa el formato y la velocidad de fotogramas elegidos en las listas. Format_UseBest está activado por defecto. Elige la resolución más alta hasta 1080p, y después prefiere MJPG y la velocidad de fotogramas más rápida. Para grabar en 4K, o en cualquier formato que elija, desactívelo y fije Format y FrameRate, como hacemos aquí.

### Seleccionar el micrófono

Sin micrófono en la lista, la aplicación graba solo video. Si no, graba el seleccionado. Audio_PlayAudio está desactivado, así que el micrófono no suena por los altavoces mientras graba.

### FFMPEGEXEOutput

Ahora, la salida. Un FFMPEGEXEOutput recibe un muxer, que es el contenedor, la configuración de video y la de audio. El caso por defecto es MP4. H264MFSettings selecciona el codificador H.264, a ocho megabits por segundo constantes, y el audio es AAC. El caso HEVC también es MP4. HEVCMFSettings con ForceHWEncoding usa el codificador de la GPU. HEVC necesita menos bits para la misma imagen, así que aquí son seis megabits por segundo. Para WebM, el muxer es WebM. BasicVideoSettings elige el codificador VP9 a ocho megabits por segundo constantes, y BasicAudioSettings elige Opus a 128 kilobits por segundo. Los dos casos MP4 comparten esta configuración AAC: 192 kilobits por segundo a tasa de bits constante. Algunos detalles. Aquí Bitrate está en bits por segundo, así que ocho millones significa ocho megabits. SampleRate y Channels a cero conservan el formato propio del micrófono. Ocho megabits de video son unos sesenta megabytes por minuto. Y la extensión del archivo debe coincidir con el muxer: MP4 o WebM.

### Iniciar y detener

Después del switch, el archivo va a la carpeta Videos, con la extensión que eligió el caso. El motor pasa al modo de captura de video, así que graba y no solo muestra la vista previa. StartAsync regresa en cuanto la captura está en marcha. StopAsync detiene la captura, y FFmpeg termina de escribir el archivo.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el archivo sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el manejador ha terminado. Si no hay nada grabándose, el manejador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas de cámara, formato y micrófono se llenan con los dispositivos. Con MP4 y H.264 seleccionados, Start inicia la grabación. La C920 tarda unos segundos en enviar su primer fotograma. Stop termina output.mp4. Luego elija WebM y grabe de nuevo. Stop termina output.webm, y después cerramos la ventana.

### Comprobar los archivos con ffprobe

Los dos archivos quedan en la carpeta Videos. ffprobe muestra lo que hay dentro. El MP4 tiene video H.264 a 1920x1080 y audio AAC a 48 kilohercios estéreo. El WebM tiene video VP9 a 1920x1080 y audio Opus a 48 kilohercios estéreo.

### Errores frecuentes

Algunos errores frecuentes. Falta el paquete redist de FFmpeg, así que el SDK informa de que no se encontró vfffmpeg64.exe. HEVC en un PC sin codificador de GPU para él: compruebe antes HWEncodersAvailable. Una tasa de bits escrita en kilobits: aquí son bits por segundo. Una licencia Standard o Professional: FFMPEGEXEOutput requiere la edición Premium. Y cerrar la ventana a mitad de grabación: espere primero a StopAsync con await.

### Próximos pasos

A continuación, el video de webcam a MP4 graba H.264 con los codificadores propios del SDK, sin FFmpeg, y el video de webcam a WMV usa los codificadores de Windows Media. También hay guías para cámaras IP y grabación de pantalla. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.
