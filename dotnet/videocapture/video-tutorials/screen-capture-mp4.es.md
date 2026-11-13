---
title: Captura de pantalla a MP4 en C# — GPU y multimonitor
description: Captura de pantalla completa, de región y multimonitor. Codificación GPU (NVENC/QSV/AMF), audio loopback, resaltado de cursor. Ejemplos API legacy y moderna.
sidebar_label: Captura de pantalla a MP4
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - WPF
  - GStreamer
  - Capture
  - Encoding
  - Webcam
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - MP4Output
  - ScreenCaptureSourceSettings
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCore
  - AudioCaptureSource
  - LoopbackAudioCaptureDeviceSourceSettings
  - VideoView
video:
  id: 17eaRlDqM_I
  upload_date: 2026-11-11T19:35:01Z
  duration: PT8M51S

---

# Captura de pantalla a archivo MP4

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

Este tutorial graba la pantalla y el sonido del sistema en un archivo MP4 desde una aplicación WPF. El motor `VideoCaptureCoreX` captura el monitor principal mediante Direct3D 11, dibuja una vista previa en directo en un `VideoView` y codifica H.264 en la GPU cuando el equipo tiene un codificador por hardware, con audio AAC. El sonido que se reproduce en los altavoces se graba mediante WASAPI loopback.

El motor clásico `VideoCaptureCore` también graba la pantalla; su versión de la aplicación está [más abajo en esta página](#segunda-opcion-videocapturecore).

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/17eaRlDqM_I?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/screen-capture-mp4-wpf)

## Paquetes NuGet requeridos

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — el SDK; el `VideoView` de WPF viene incluido
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — el runtime de GStreamer sobre el que funciona el motor X
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — el codificador de audio AAC

Son los paquetes x64, así que el proyecto fija `<PlatformTarget>x64</PlatformTarget>`; una aplicación de 32 bits usa los paquetes `.x86`. Consulte [Despliegue en Windows](../../deployment-x/Windows.md) para la lista completa.

## La ventana en XAML

El prefijo `vf` asigna el espacio de nombres `VisioForge.Core.UI.WPF` del ensamblado `VisioForge.Core`. La columna izquierda contiene la lista de altavoces, la casilla de sonido y los botones; la columna derecha, el `VideoView` donde se dibuja la vista previa en directo. Start permanece deshabilitado hasta que el SDK termina de cargarse.

```xml
<Window x:Class="ScreenCaptureMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Screen to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Speakers" />
            <ComboBox x:Name="cbSpeakers" />
            <CheckBox x:Name="cbRecordSound" Content="Record system sound" IsChecked="True"
                      Margin="0,10,0,0" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" IsEnabled="False"
                        Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

## El código de la ventana

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace ScreenCaptureMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private AudioOutputDeviceInfo[] speakers;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Muestra los errores del SDK en la barra de título
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        // El sonido de estas salidas se graba mediante WASAPI loopback
        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.Name);

        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        // Todo el monitor principal, con el cursor del ratón
        videoCapture1.Video_Source = new ScreenCaptureD3D11SourceSettings
        {
            FrameRate = new VideoFrameRate(30)
        };

        var recordSound = cbRecordSound.IsChecked == true && cbSpeakers.SelectedIndex >= 0;
        videoCapture1.Audio_Source = recordSound
            ? new LoopbackAudioCaptureDeviceSourceSettings(speakers[cbSpeakers.SelectedIndex])
            : null;
        videoCapture1.Audio_Record = recordSound;
        videoCapture1.Audio_Play = false; // nunca reproduce el sonido capturado en los altavoces

        // H.264 en el codificador de la GPU si lo hay, OpenH264 si no; audio AAC
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new MP4Output(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4")));

        btStart.IsEnabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
    }

    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        if (btStart.IsEnabled)
        {
            videoCapture1.Dispose();
            VisioForgeX.DestroySDK();
            return;
        }

        e.Cancel = true; // aún cargando o grabando: primero se finaliza el MP4
        if (!btStop.IsEnabled) return; // ya está cargando, iniciando o deteniéndose
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // no Close(): WPF lanza una excepción dentro de Closing
    }
}
```

## Cómo funciona

1. **Inicialización del SDK** — `VisioForgeX.InitSDKAsync` se ejecuta una vez, al cargarse la ventana. La primera ejecución en un equipo tarda más, por eso Start se habilita solo cuando termina. `VisioForgeX.DestroySDK` lo libera al cerrar la ventana.
2. **Fuente de pantalla** — `ScreenCaptureD3D11SourceSettings` captura por defecto todo el monitor principal con el cursor del ratón. Usa DXGI Desktop Duplication, que arranca de inmediato incluso con la pantalla quieta, y recurre a Windows Graphics Capture donde Desktop Duplication no está soportado. La velocidad por defecto es de 10 fps, por eso el código fija 30.
3. **Sonido del sistema** — `AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` enumera los altavoces y auriculares, y `LoopbackAudioCaptureDeviceSourceSettings` graba lo que suena en el elegido. `Audio_Play` es `false`, así que el sonido grabado no se reproduce.
4. **Salida MP4** — `MP4Output` sin ajustes de codificador elige NVIDIA NVENC, después AMD AMF, después Intel Quick Sync, y OpenH264 en la CPU cuando no hay ninguno. El audio es AAC. Una salida permanece en el motor después de una grabación, por eso `Outputs_Clear` se ejecuta antes de cada nueva.
5. **Iniciar, detener y cerrar** — `StartAsync` devuelve si la captura se inició, y `StopAsync` finaliza el archivo MP4. Cerrar la ventana durante una grabación cancela el cierre, espera a `StopAsync` y vuelve a cerrar.

## Notas

- Un proyecto WPF deja `System.IO` fuera de sus usings implícitos, por eso el archivo lo añade para `Path`.
- La vista previa muestra la pantalla que se graba, así que la ventana de la aplicación aparece dentro de sí misma, en la vista previa y en el archivo. En un equipo con dos monitores, mueva la ventana al que no se graba; con un solo monitor, minimícela después de Start y restáurela para detener.
- Con **Record system sound** desmarcada, o sin altavoces en la lista, el MP4 contiene solo vídeo.
- `MP4Output` elige un codificador GPU porque existe su elemento de GStreamer, lo que no demuestra que pueda codificar en ese momento. `H264EncoderBlock.CanEncodeAsync` codifica unos fotogramas de prueba para comprobarlo; para quedarse en la CPU, pase `new OpenH264EncoderSettings()` al constructor de `MP4Output`.

## Segunda opción: VideoCaptureCore

El motor clásico de Windows, `VideoCaptureCore`, graba la pantalla en MP4 en una aplicación WinForms, con o sin micrófono.

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

### Redistribuibles requeridos

- Redistribuibles de captura de video [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Redistribuibles MP4 [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

### Ejemplo de código

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
            // VideoView1 debe ser un control en su formulario que implemente la interfaz IVideoView
            // Esto permite vista previa en vivo de la captura cuando se desee
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Cómo funciona — VideoCaptureCore

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

#### Configuración de audio

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

#### Captura de región

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

#### Grabación multi-monitor

Seleccione qué pantalla grabar usando la propiedad `DisplayIndex`. El monitor principal es el índice `0`, el secundario es `1`, y así sucesivamente:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Grabar el monitor secundario
};
```

#### Configuración de calidad de grabación

Personalice la salida MP4 configurando el codificador H.264. El bitrate predeterminado es 3500 kbps, que produce aproximadamente 25 MB por minuto a 1080p:

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Mayor calidad (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Mejor compresión que Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Bitrate variable
videoCapture1.Output_Format = mp4Output;
```

#### Opciones del cursor del mouse

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

## Aplicación de consola — VideoCaptureCoreX { #api-moderna-video-capture-sdk-x }

El motor multiplataforma `VideoCaptureCoreX` también funciona sin ventana, con captura de pantalla Direct3D 11 (Desktop Duplication). Esta aplicación de consola graba la pantalla completa a MP4 con audio del sistema opcional.

Usa los mismos paquetes que la aplicación WPF: consulte [Paquetes NuGet requeridos](#paquetes-nuget-requeridos).

Los mismos paquetes se usan para la prueba de 30 días y las aplicaciones con licencia. Consulte la [página de licencias de VisioForge](https://www.visioforge.com/licensing) para conocer las condiciones de la prueba y la activación mediante `.vflicense`.

### Ejemplo completo

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
            // Configurar captura de pantalla Direct3D 11 (Desktop Duplication por defecto)
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

### Agregar audio del sistema (Loopback)

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

## Codificación acelerada por GPU

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

Los codificadores HEVC (H.265) también están disponibles para mejor compresión a la misma calidad: `NVENCHEVCEncoderSettings`, `QSVHEVCEncoderSettings`, `AMFHEVCEncoderSettings`.

!!! note "Nota de motor: `MP4Output` y el motor X"

    La clase `MP4Output` mostrada en esta sección refiere al `MP4Output`
    del **motor X** (`VisioForge.Core.Types.X.Output.MP4Output`), que
    acepta tipos `*EncoderSettings` multiplataforma
    (`NVENCH264EncoderSettings`, `OpenH264EncoderSettings`, etc.).
    Cuando la codificación por hardware no está disponible en el motor X
    y usa el ctor sin parámetros `new MP4Output(filename)`, el SDK
    selecciona automáticamente un codificador de software disponible. El
    motor clásico DirectShow solo-Windows tiene su propio
    `VisioForge.Core.Types.Output.MP4Output` con una forma de settings
    diferente — no los mezcle.

## Captura de pantalla con Media Blocks SDK

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

## Captura de pantalla multiplataforma

El SDK soporta captura de pantalla en Windows, macOS y Linux con configuraciones de fuente específicas por plataforma:

| Plataforma | Método de Captura | Clase de Configuración | Requisitos |
|------------|-------------------|------------------------|------------|
| Windows | Direct3D 11 / WGC | `ScreenCaptureD3D11SourceSettings` | Windows 8+ (WGC: Windows 10 v1803+) |
| macOS | AVFoundation | `ScreenCaptureMacOSSourceSettings` | macOS 10.15+ (permiso de grabación de pantalla) |
| Linux | X11 / XDisplay | `ScreenCaptureXDisplaySourceSettings` | Servidor X11 |

En Windows, el SDK usa DXGI Desktop Duplication por defecto: arranca de inmediato, incluso con la pantalla quieta, y recurre a WGC donde Desktop Duplication no está soportado. La captura de ventana siempre usa WGC. También puede elegir WGC para una pantalla; también arranca de inmediato con la pantalla quieta:

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.WGC  // Windows Graphics Capture en lugar de Desktop Duplication
};
```

Para Android e iOS, y un grabador en .NET MAUI que funciona en Windows, Mac Catalyst, Android e iOS, consulte [Grabación de pantalla multiplataforma en C# y .NET](https://www.visioforge.com/es/solutions/screen-recording-csharp).

## Preguntas frecuentes

### ¿Qué licencia necesito para una aplicación de captura de pantalla en C#?

El Video Capture SDK .NET requiere una licencia para desarrollo y distribución. Una licencia de Desarrollador elimina la marca de agua de evaluación y desbloquea todas las funciones durante el desarrollo. Una licencia de Release es necesaria al distribuir su aplicación a los usuarios finales. El SDK está disponible en edición Premium que incluye todos los modos de captura, salida MP4/AVI/WMV y los motores DirectShow y GStreamer. Puede evaluar el SDK sin licencia — la captura de pantalla funciona completamente pero incluye una marca de agua superpuesta. Visite la [página del producto](https://www.visioforge.com/video-capture-sdk-net) para conocer precios y opciones de licencia.

### ¿Cómo controlo la calidad de grabación y el tamaño de archivo para la captura de pantalla MP4?

Configure el bitrate de video de `MP4Output` para equilibrar calidad y tamaño de archivo. El valor predeterminado es 3500 kbps, que funciona bien para la mayoría de las grabaciones de pantalla. Reduzca el bitrate a 1500–2000 kbps para archivos más pequeños, o aumente a 5000–8000 kbps para capturas de alta calidad. Use `H264Profile.ProfileMain` o `ProfileHigh` en lugar de `ProfileBaseline` para mejor compresión a la misma calidad. La tasa de fotogramas también afecta el tamaño del archivo — 15 FPS es suficiente para presentaciones, mientras que 30 FPS es mejor para demos de software. Una grabación de 1080p a 3500 kbps produce aproximadamente 25 MB por minuto.

### ¿Puedo capturar el audio del sistema (sonido del escritorio) en lugar de la entrada del micrófono?

Sí. Con `VideoCaptureCoreX`, pase un altavoz de `DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` a `LoopbackAudioCaptureDeviceSourceSettings`, como hace la aplicación WPF de arriba: graba lo que suena en ese dispositivo, sin ningún controlador especial. Con `VideoCaptureCore`, llame a `Audio_CaptureDevices()` para enumerar todos los dispositivos de audio disponibles, luego seleccione un dispositivo de loopback o audio del sistema (como "Stereo Mix" o "What U Hear") por nombre. Establezca `Audio_RecordAudio = true` y `Audio_PlayAudio = false`. Los nombres de dispositivos de loopback disponibles dependen de su hardware de audio y controladores. Puede grabar tanto el micrófono como el audio del sistema simultáneamente configurando fuentes de audio adicionales.

### ¿Cómo grabo un monitor específico en una configuración multi-monitor?

Con `VideoCaptureCoreX`, establezca `MonitorIndex` en `ScreenCaptureD3D11SourceSettings` (`0` es el primer monitor, `-1` el principal) y ajuste `Rectangle` al tamaño de ese monitor. Con `VideoCaptureCore`, establezca la propiedad `DisplayIndex` en `ScreenCaptureSourceSettings` — use `0` para el monitor principal, `1` para el secundario, y así sucesivamente. Combine con `FullScreen = true` para capturar toda la pantalla seleccionada. También puede establecer `FullScreen = false` con coordenadas `Left/Top/Right/Bottom` para capturar una región específica en el monitor elegido.

### ¿Qué tasa de fotogramas debo usar para la grabación de pantalla?

La tasa de fotogramas predeterminada es 10 FPS. Use 15 FPS para presentaciones, diapositivas y contenido mayormente estático. Use 25–30 FPS para tutoriales de software y demostraciones de UI donde el movimiento suave del mouse importa. Use 60 FPS para grabación de juegos o contenido con mucho movimiento. Tasas de fotogramas más altas aumentan el tamaño del archivo y el uso de CPU proporcionalmente. Establezca la tasa de fotogramas a través de `ScreenCaptureD3D11SourceSettings.FrameRate` con `VideoCaptureCoreX`, o de `ScreenCaptureSourceSettings.FrameRate` con `VideoCaptureCore`.

## Transcripción del vídeo

### Qué vamos a construir

Vamos a grabar la pantalla y el sonido del sistema en un archivo MP4 en una aplicación C# WPF con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. La aplicación graba todo el monitor principal, con el cursor del ratón, y el sonido que se reproduce en los altavoces. Escribe MP4, con H.264 codificado en la GPU cuando el PC tiene un codificador por hardware, y audio AAC. Un VideoView muestra una vista previa en directo, y la ventana se diseña en XAML. El código de la ventana ocupa unas noventa líneas de C#. Esta es la aplicación terminada, grabando. La vista previa muestra la pantalla, y la pantalla contiene la vista previa, así que se repite como dos espejos enfrentados.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto usa .NET 8 o posterior. La guía escrita también muestra la grabación de pantalla con el motor clásico VideoCaptureCore.

### Crear un proyecto WPF

En Visual Studio, cree un proyecto nuevo. Busque WPF y elija WPF Application para C#. Llame al proyecto ScreenCaptureMp4Wpf. Elija .NET 10 y haga clic en Create. Visual Studio abre MainWindow.xaml, con el diseñador arriba y el XAML abajo.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. El propio Video Capture SDK. CrossPlatform Core, el runtime de GStreamer sobre el que funciona el motor X. Y CrossPlatform Libav, que contiene el codificador AAC. O use el NuGet Package Manager: clic derecho en el proyecto, Manage NuGet Packages, e instale VisioForge.DotNet.VideoCapture desde la pestaña Browse. Después instale los paquetes Core y Libav para Windows x64. La pestaña Installed muestra los tres. Son los paquetes x64, así que fije PlatformTarget en x64 en el archivo de proyecto. Una aplicación de 32 bits usa los paquetes x86.

### Construir la ventana en XAML

La ventana se escribe en XAML. Primero la ventana en sí, con una cuadrícula de dos columnas y un VideoView, y el diseñador la dibuja al instante. Asigne el espacio de nombres VisioForge.Core.UI.WPF a un prefijo, aquí vf. El VideoView va en la columna derecha: ahí se dibuja la vista previa en directo. La ventana también nombra sus manejadores Loaded y Closing. Después, la lista de altavoces y una casilla para el sonido. La columna izquierda es un StackPanel con una etiqueta, un cuadro combinado para los altavoces y una casilla que activa o desactiva el sonido. Empieza marcada. Por último, los botones Start y Stop. Cada botón nombra un manejador Click. Los dos empiezan deshabilitados: Start hasta que el SDK se carga, y Stop hasta que hay una grabación en curso. Cada manejador que nombra el XAML va en MainWindow.xaml.cs, y los próximos capítulos los escriben.

### Inicializar el SDK

MainWindow.xaml.cs empieza con estos usings. Un proyecto WPF deja System.IO fuera de sus usings implícitos, así que añádalo para Path. Cuando la ventana se carga, espere a VisioForgeX.InitSDKAsync una sola vez. Después cree el motor VideoCaptureCoreX y pásele el VideoView, donde se dibuja la vista previa. Los errores del motor llegan en el evento OnError; aquí van a la barra de título.

### Enumerar los altavoces

AudioOutputsAsync con WASAPI2 enumera los altavoces y auriculares. Sus nombres llenan la lista, y se selecciona el primero. Solo ahora se habilita Start, una vez cargado el SDK. WASAPI loopback graba lo que suena en un dispositivo de salida. No necesita Stereo Mix ni un cable virtual. Elija el dispositivo en el que realmente suena el audio, porque loopback graba solo ese.

### Capturar la pantalla

En el manejador de Start, asigne a la fuente de vídeo ScreenCaptureD3D11SourceSettings. Por defecto captura todo el monitor principal con el cursor del ratón. La velocidad por defecto es de diez fotogramas por segundo, así que fije treinta. La fuente usa Windows Graphics Capture en Windows 10 versión 1803 y posteriores, y DXGI Desktop Duplication en sistemas más antiguos. MonitorIndex elige otro monitor, y CaptureCursor en false deja fuera el cursor.

### Grabar el sonido del sistema

Cuando la casilla está marcada y hay un dispositivo seleccionado, la fuente de audio es LoopbackAudioCaptureDeviceSourceSettings para ese dispositivo, y Audio_Record está activado. Si no, no hay fuente de audio, y el MP4 contiene solo vídeo. Audio_Play es false, así que el sonido grabado no se reproduce en los altavoces.

### Escribir el MP4

Añada un MP4Output que escriba en la carpeta Vídeos, con la fecha y la hora en el nombre del archivo, para que una segunda grabación nunca sobrescriba la primera. Una salida permanece en el motor después de una grabación, por eso Outputs_Clear se ejecuta antes de cada nueva. Sin ajustes de codificador, MP4Output elige NVIDIA NVENC, después AMD AMF, después Intel Quick Sync, y OpenH264 en la CPU cuando el PC no tiene ninguno. El audio es AAC. H264EncoderBlock.CanEncodeAsync codifica unos fotogramas de prueba cuando necesita saber si un codificador GPU funciona de verdad.

### Iniciar y detener

StartAsync devuelve si la captura se inició, y los botones lo siguen. StopAsync finaliza el archivo MP4. Si la ventana se cierra durante una grabación, cancele el cierre, espere a StopAsync y vuelva a cerrar. En esa segunda pasada se libera el motor y el SDK, así que el archivo siempre queda completo.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Cuando el SDK se ha cargado, la lista de altavoces se llena y Start se habilita. Haga clic en Start. La vista previa muestra la pantalla con la aplicación dentro, una y otra vez. El sonido de alarma que suena mientras tanto entra en el archivo mediante loopback. Stop finaliza el MP4. Después cierre la ventana. El archivo queda en Vídeos: 4K a treinta fotogramas por segundo, con el sonido del sistema.

### Errores frecuentes

Algunos errores frecuentes. Omitir el paquete Libav, que contiene el codificador AAC. Mezclar paquetes x86 y x64, o ejecutar una aplicación x86 con los x64. Olvidar que toda ventana del monitor grabado acaba en el vídeo, incluida la de la propia aplicación: en un PC con dos monitores, muévala al que no se graba. Y loopback en un dispositivo en el que no suena el audio. Los errores del motor llegan en el evento OnError.

### Próximos pasos

También hay guías para grabar una región o una sola ventana, añadir un micrófono, grabar una webcam y transmitir por red. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el vídeo.

## Ver también

- [Captura de Pantalla a AVI](screen-capture-avi.md) — grabar pantalla a formato AVI con video sin comprimir o MJPEG
- [Captura de Pantalla a WMV](screen-capture-wmv.md) — grabar pantalla a formato Windows Media
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — aplicación de grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para región de captura, multi-monitor, cursor y configuración de tasa de fotogramas
- [Guardar Video de Webcam](../guides/save-webcam-video.md) — captura de webcam a MP4 con configuración de audio
- [Ejemplos de Código](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — ejemplos adicionales de captura de pantalla en GitHub
- [Grabación de pantalla multiplataforma en C#](https://www.visioforge.com/es/solutions/screen-recording-csharp) — API de captura por plataforma, un grabador en .NET MAUI y la descarga de la versión de prueba
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas
