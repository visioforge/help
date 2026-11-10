---
title: Grabar webcam a MP4 en C# WPF con H.264 y audio AAC
description: Una ventana WPF en XAML con vista previa de la webcam que graba MP4 con H.264 en la GPU y AAC, con Video Capture SDK .NET. Código C# y vídeo.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WPF
  - Capture
  - Webcam
  - MP4
  - H.264
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4HWOutput
  - VideoView
video:
  id: NNqmz2vJyQc
  upload_date: 2026-11-10T16:30:06Z
  duration: PT9M

---

# Captura de vídeo de webcam a MP4 en una aplicación WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la captura de webcam en WPF

Este tutorial graba una webcam y un micrófono en un archivo MP4 desde una aplicación WPF. La ventana se define en XAML, la vista previa en directo se dibuja en un `VideoView` y el motor `VideoCaptureCore` codifica H.264 en la GPU cuando el PC tiene un codificador por hardware, con audio AAC.

Es la misma aplicación que la del [tutorial de webcam a MP4 en WinForms](video-capture-webcam-mp4.md), construida al estilo de WPF:

- La ventana, sus listas y sus botones se escriben en XAML
- Los controladores de los cuadros combinados leen `SelectedItem`, porque dentro de `SelectionChanged` la propiedad `Text` todavía contiene el elemento anterior
- Si se cierra la ventana durante una grabación, el archivo MP4 se finaliza antes de que la ventana desaparezca

## Tutorial de implementación en vídeo

El siguiente vídeo construye la aplicación paso a paso:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/NNqmz2vJyQc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo está disponible en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-wpf)

## Componentes redistribuibles necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y los paquetes redistribuibles siguientes. El `VideoView` de WPF viene en el paquete principal, así que no hace falta ningún paquete de interfaz aparte.

### Componentes de captura de vídeo

- Para aplicaciones de 32 bits: [Redist de captura de vídeo x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Para aplicaciones de 64 bits: [Redist de captura de vídeo x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Componentes de codificación MP4

- Para aplicaciones de 32 bits: [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Para aplicaciones de 64 bits: [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## La ventana en XAML

El prefijo `vf` asigna el espacio de nombres `VisioForge.Core.UI.WPF` del ensamblado `VisioForge.Core`. La columna izquierda contiene las listas y los botones; la derecha, el `VideoView` donde se dibuja la vista previa en directo. Cada controlador que se nombra aquí se escribe más abajo, en `MainWindow.xaml.cs`.

```xml
<Window x:Class="WebcamMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" SelectionChanged="cbCamera_SelectionChanged" />
            <Label Content="Format" />
            <ComboBox x:Name="cbFormat" SelectionChanged="cbFormat_SelectionChanged" />
            <Label Content="Frame rate" />
            <ComboBox x:Name="cbFrameRate" />
            <Label Content="Microphone" />
            <ComboBox x:Name="cbMicrophone" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

## Ejemplo de implementación con código C#

El código de la ventana. El proyecto usa .NET 10 con usings implícitos; un proyecto WPF deja `System.IO` fuera de ellos, por lo que se añade para `Path`.

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCore videoCapture1;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Mostrar los errores del SDK en la barra de título
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
    }

    private void cbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFrameRate.Items.Clear();
        if (cbFormat.SelectedItem is null) return; // la lista se acaba de vaciar

        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        var format = device.VideoFormats.First(f => f.Name == (string)cbFormat.SelectedItem);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        Title = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource((string)cbCamera.SelectedItem);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = (string)cbFormat.SelectedItem;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // sin micrófono: grabar solo vídeo
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // grabar el micrófono sin reproducirlo

        var mp4 = new MP4HWOutput();

        // NVIDIA, luego Intel, luego AMD y, por último, el codificador de la CPU
        var gpu = VideoCaptureCore.HWEncodersAvailable();
        mp4.Video.Codec =
            gpu.NVENC_H264 ? MFVideoEncoder.NVENC_H264 :
            gpu.QSV_H264 ? MFVideoEncoder.QSV_H264 :
            gpu.AMD_H264 ? MFVideoEncoder.AMD_H264 :
            MFVideoEncoder.MS_H264;

        mp4.Video.AvgBitrate = 6000; // kbps
        mp4.Video.MaxBitrate = 9000;
        mp4.Video.RateControl = MFCommonRateControlMode.PeakConstrainedVBR;
        mp4.Video.Profile = MFH264Profile.Main;
        mp4.Audio.Bitrate = 192; // AAC, kbps

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = mp4;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "text")
        {
            Text = "Hello World!", Left = 50, Top = 50, FontColor = System.Drawing.Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

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
        if (btStart.IsEnabled) { videoCapture1.Dispose(); return; }

        e.Cancel = true; // todavía se graba: primero se finaliza el MP4
        if (!btStop.IsEnabled) return; // ya se está iniciando o deteniendo
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // no Close(): WPF lanza una excepción dentro de Closing
    }
}
```

## Características clave de la implementación

1. **Ventana en XAML** - Un `Grid` de dos columnas: las listas y los botones a la izquierda, el `VideoView` con la vista previa en directo a la derecha
2. **Selección de dispositivo y formato** - La cámara, su formato, su velocidad de fotogramas y el micrófono se eligen de listas que se rellenan desde los propios dispositivos; los controladores leen `SelectedItem`
3. **Codificación por hardware con alternativa en la CPU** - `MP4HWOutput` codifica H.264 en una GPU NVIDIA, Intel o AMD; `HWEncodersAvailable` indica qué codificadores tiene el PC, y `MS_H264` funciona en la CPU cuando no hay ninguno
4. **Ajustes del codificador** - Una media de 6000 kbps, picos de hasta 9000 kbps, el perfil Main y audio AAC a 192 kbps
5. **Operación asíncrona** - `StartAsync` y `StopAsync` mantienen la interfaz receptiva, y al cerrar la ventana durante una grabación se espera a `StopAsync` antes de cerrarla

## Notas sobre WPF

- `FontColor` de `VideoEffectTextLogo` es un `System.Drawing.Color`, no un `Color` de WPF, por eso se escribe completo
- El controlador de `OnError` cambia el título de la ventana mediante `Dispatcher.InvokeAsync`, así que se ejecuta en el hilo de la interfaz, sea cual sea el hilo que genere el evento
- `Format_UseBest` está activado por defecto y elige la resolución más alta hasta 1080p, con preferencia por MJPG a la misma resolución; desactívelo para grabar el formato elegido en la lista
- Si no hay micrófono en la lista, `Audio_RecordAudio` se pone en `false` y el MP4 contiene solo vídeo
- Cuando el motor ya se ha detenido (tras un error), `StopAsync` termina al instante y el código posterior al `await` sigue ejecutándose dentro de `Closing`, donde WPF no permite `Close()`; por eso la ventana se vuelve a cerrar mediante `Dispatcher.InvokeAsync`

## Transcripción del vídeo

### Qué vamos a construir

Vamos a grabar una webcam en un archivo MP4 en una aplicación C# WPF con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una ventana WPF con vista previa de la cámara en directo, diseñada en XAML. Graba en MP4, con H.264 codificado en la GPU cuando el PC tiene una, y audio AAC. La cámara, su formato y el micrófono se eligen en listas, y el código de la ventana ocupa unas cien líneas de C#. Esta es la aplicación terminada, grabando desde una Logitech C920.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto usa .NET 8 o posterior. La misma aplicación en WinForms tiene su propio video.

### Crear un proyecto WPF

En Visual Studio, abra File, New, Project. Busque WPF y elija WPF Application para C#. Llame al proyecto WebcamMp4Wpf. Elija .NET 10 y haga clic en Create. Visual Studio abre MainWindow.xaml, con el diseñador arriba y el XAML abajo.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet: el propio SDK, el redist de captura de video y el redist de MP4. Estos son los paquetes x64. Una aplicación x86 usa en su lugar los redist x86. También puede instalar los mismos tres paquetes desde el administrador de paquetes NuGet. El VideoView de WPF viene en el paquete principal, así que no hace falta nada más.

### Construir la ventana en XAML

En WPF, la ventana se escribe en XAML. Primero la ventana en sí, con una cuadrícula de dos columnas y un VideoView, y el diseñador de arriba la dibuja al momento. El VideoView también está en el Toolbox, bajo VisioForge. Asocie el espacio de nombres VisioForge.Core.UI.WPF a un prefijo, aquí vf. Después, un VideoView va en la columna derecha de una cuadrícula: ahí se dibuja la vista previa en directo. La ventana también nombra sus controladores Loaded y Closing. Después, las etiquetas y los cuadros combinados van en la columna izquierda. La columna izquierda es un StackPanel con una etiqueta y un cuadro combinado para la cámara, el formato, la velocidad de fotogramas y el micrófono. Las listas de cámara y de formato nombran un controlador SelectionChanged. Por último, los botones Start y Stop. Cada botón nombra un controlador Click. Stop permanece desactivado hasta que empieza una grabación. Cada controlador nombrado en el XAML va en MainWindow.xaml.cs, y los próximos capítulos los escriben.

### Crear el motor de captura

En el controlador Loaded de la ventana, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo.

### Enumerar las cámaras

Video_CaptureDevices devuelve todas las cámaras del equipo. Añadimos sus nombres a la lista y seleccionamos la primera. Cuando cambia la cámara, su controlador llena la lista con los formatos de esa cámara. Aquí lea SelectedItem, no Text: dentro de SelectionChanged, el Text de un cuadro combinado de WPF todavía contiene el elemento anterior. Cuando cambia el formato, su controlador llena de la misma forma la lista de velocidades de fotogramas. Al vaciar la lista de formatos se dispara SelectionChanged sin nada seleccionado, así que el controlador sale antes. Una cámara informa de su nombre, de formatos como 1920x1080 MJPG o 1280x720 YUY2, y de las velocidades de fotogramas de cada formato. MJPG se comprime en la cámara, así que las resoluciones altas caben por USB. YUY2 no está comprimido, así que USB 2 solo admite resoluciones más bajas.

### Elegir el formato de la cámara

Seleccione la cámara por su nombre. Después desactive Format_UseBest y pase el formato y la velocidad de fotogramas elegidos en las listas. Format_UseBest está activado por defecto. Elige la mayor resolución hasta 1080p que ofrece la cámara y, a igual resolución, prefiere MJPG. Para un formato fijo, póngalo en false e indique usted mismo el formato y la velocidad de fotogramas.

### Seleccionar el micrófono

El micrófono funciona igual. Audio_CaptureDevices los enumera, y usamos el elegido en la lista. Audio_PlayAudio en false graba el micrófono sin reproducirlo.

### MP4HWOutput: cualquier GPU, alternativa en la CPU

Para la salida, cree un MP4HWOutput, escriba output.mp4 en la carpeta Videos y cambie el motor al modo de captura de video, para que grabe y no solo muestre la vista previa. Un proyecto WPF deja System.IO fuera de sus usings implícitos, así que añádalo para Path. MP4HWOutput codifica en la GPU: NVENC en NVIDIA, Quick Sync en Intel y AMF en AMD, en H.264 o HEVC. Cuando no hay codificador por hardware, se encarga el codificador de Windows en la CPU. El código y el archivo son los mismos en cualquier PC. HWEncodersAvailable indica qué codificadores tiene este PC. Use primero NVIDIA, luego Intel, luego AMD, y recurra al codificador de la CPU.

### Ajustar el codificador H.264

Después ajuste el codificador: seis mil kilobits por segundo de media, picos de hasta nueve mil, el perfil Main y audio AAC a ciento noventa y dos. Como guía: de 2500 a 4000 kilobits para 720p, de 5000 a 8000 para 1080p y de 15000 a 25000 para 4K. Ambas tasas de bits se indican en kilobits por segundo.

### Añadir texto superpuesto

Los efectos de video añaden un texto superpuesto: un Hello World fijo y un reloj que muestra la hora actual en cada fotograma. El color de la fuente es un color de System.Drawing, no de WPF, así que escríbalo completo.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve cuando la captura está en marcha, y StopAsync finaliza el archivo MP4. Si la ventana se cierra durante una grabación, cancele el cierre, espere a StopAsync con await y vuelva a cerrar. El archivo queda completo, y el motor se libera en la segunda pasada. Ejecute la aplicación desde Visual Studio. La lista de formatos viene directamente de la cámara. Start inicia la grabación, con el reloj superpuesto en marcha. Stop finaliza output.mp4. El archivo queda en Videos: 1920x1080, video H.264 y audio AAC.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Una cámara que ya está abierta en otra aplicación. Paquetes x86 y x64 mezclados en un mismo proyecto. Y liberar el motor en plena captura: espere primero a StopAsync. Los errores del motor llegan en el evento OnError.

### Próximos pasos

También hay guías sobre cámaras IP, grabación de pantalla, capturas de imágenes y streaming por red. El código completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4 en WinForms](video-capture-webcam-mp4.md) — la misma aplicación como proyecto de Windows Forms
- [Grabación Pre-Evento](../guides/pre-event-recording.md) — grabación con buffer circular con disparadores de detección de movimiento para webcams y cámaras IP
- [Vista Previa de Video de Cámara](camera-video-preview.md) — mostrar video de cámara en vivo con captura de fotogramas

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación.
