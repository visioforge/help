---
title: Grabar Webcam a MP4 en C# — Video Capture SDK .NET
description: Enumeración automática de dispositivos, codificación H.264/AAC, grabación async con vista previa WinForms. Seleccione webcam y micrófono, configure resolución.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
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
  id: fAt1Bp6qYPc
  upload_date: 2026-11-03T13:29:29Z
  duration: PT7M40S

---

# Implementación de Captura de Video de Webcam a MP4 en .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Captura de Webcam en Aplicaciones .NET

Crear aplicaciones que graben video de webcams a archivos MP4 es un requisito común para muchos proyectos de software. Este tutorial proporciona un enfoque orientado a desarrolladores para implementar esta funcionalidad usando técnicas modernas de .NET y el Video Capture SDK.

Al implementar capacidades de grabación de webcam, los desarrolladores necesitan considerar:

- Seleccionar los dispositivos de entrada de video y audio apropiados
- Configurar ajustes de compresión de video y audio
- Gestionar el ciclo de vida de captura (inicialización, inicio, detención)
- Manejar la creación y gestión del archivo de salida

Este tutorial usa el motor VideoCaptureCore, solo para Windows. [Video Capture SDK .NET](https://www.visioforge.com/es/video-capture-sdk-net) también graba webcams en MP4 en macOS, Linux, Android e iOS con VideoCaptureCoreX; consulte [Guardar vídeo de webcam en C#](../guides/save-webcam-video.md) y las guías de implementación para [Android](../../deployment-x/Android.md) e [iOS](../../deployment-x/iOS.md).

## Tutorial de Implementación en Video

El siguiente video demuestra el proceso completo de configurar una aplicación de captura de webcam:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fAt1Bp6qYPc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de Código Fuente

Para desarrolladores que prefieren examinar la implementación completa, todo el código fuente está disponible en nuestro repositorio de GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-winforms)

## Componentes Redistribuibles Requeridos

Antes de implementar la solución, instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y los paquetes redistribuibles necesarios:

### Componentes de Captura de Video

- Para aplicaciones de 32 bits: [Redist de Captura de Video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- Para aplicaciones de 64 bits: [Redist de Captura de Video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Componentes de Codificación MP4

- Para aplicaciones de 32 bits: [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- Para aplicaciones de 64 bits: [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Ejemplo de Implementación con Código C#

La siguiente implementación en C# demuestra cómo crear una aplicación de Windows Forms que captura video de una webcam y lo guarda a un archivo MP4. El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato, la velocidad de fotogramas y el micrófono, y los botones Start y Stop; el archivo del diseñador está en el código fuente indicado arriba. El proyecto usa .NET 10 con usings implícitos.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1;

    public Form1()
    {
        InitializeComponent();
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Mostrar los errores del SDK en la barra de título
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
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
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(cbMicrophone.Text);
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
            Text = "Hello World!", Left = 50, Top = 50, FontColor = Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finalizar el MP4 antes de que se cierre la ventana
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // ya se está iniciando o deteniendo

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            Close();
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Características Clave de Implementación

Esta implementación proporciona varias capacidades importantes para desarrolladores:

1. **Selección de Dispositivo y Formato** - La cámara, su formato, su velocidad de fotogramas y el micrófono se eligen de listas que se rellenan desde los propios dispositivos
2. **Codificación por Hardware con Alternativa en la CPU** - `MP4HWOutput` codifica H.264 en una GPU NVIDIA, Intel o AMD; `HWEncodersAvailable` indica qué codificadores tiene el PC, y `MS_H264` funciona en la CPU cuando no hay ninguno
3. **Ajustes del Codificador** - Una media de 6000 kbps, picos de hasta 9000 kbps, el perfil Main y audio AAC a 192 kbps
4. **Texto Superpuesto** - Un texto fijo y un reloj que muestra la hora actual, añadidos como efectos de video
5. **Operación Asíncrona** - `StartAsync` y `StopAsync` mantienen la UI receptiva, y al cerrar el formulario primero se finaliza el archivo MP4

## Opciones de Configuración Avanzada

Los desarrolladores pueden extender la solución con características adicionales:

- HEVC en lugar de H.264: `NVENC_H265`, `QSV_H265` o `AMD_H265`
- Otros efectos de video y transformaciones al vuelo
- Soporte de múltiples pistas de audio

## Transcripción del video

### Qué vamos a construir

Vamos a grabar una webcam en un archivo MP4 en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una ventana WinForms con vista previa de la cámara en directo. Graba en MP4, con H.264 codificado en la GPU cuando el PC tiene una, y audio AAC. La cámara, su formato y el micrófono se eligen de listas, y toda la aplicación tiene unas cien líneas de C#. Esta es la aplicación terminada, grabando desde una Logitech BRIO.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Este video trata sobre WinForms. La versión para WPF viene a continuación.

### Crear un proyecto WinForms

En Visual Studio, abra File, New, Project. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamMp4. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet: el propio SDK, el redist de captura de video y el redist de MP4. Estos son los paquetes x64. Una aplicación x86 usa en su lugar los redist x86. También puede instalar los mismos tres paquetes desde el administrador de paquetes NuGet.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un cuadro combinado para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara, la velocidad de fotogramas y el micrófono. Añada los botones Start y Stop. Stop permanece desactivado hasta que empieza una grabación. Haga doble clic en el formulario, en los cuadros combinados y en los dos botones. Visual Studio añade un controlador vacío para cada uno.

### Crear el motor de captura

En el controlador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo.

### Enumerar las cámaras

Video_CaptureDevices devuelve todas las cámaras del equipo. Añadimos sus nombres a la lista y seleccionamos la primera. Cuando cambia la cámara, su controlador llena la lista con los formatos de esa cámara. Cuando cambia el formato, su controlador llena de la misma forma la lista de velocidades de fotogramas. Una cámara informa de su nombre, de formatos como 1920x1080 MJPG o 1280x720 YUY2, y de las velocidades de fotogramas de cada formato. MJPG se comprime en la cámara, así que las resoluciones altas caben por USB. YUY2 no está comprimido, así que USB 2 solo admite resoluciones más bajas.

### Elegir el formato de la cámara

Seleccione la cámara por su nombre. Después desactive Format_UseBest y pase el formato y la velocidad de fotogramas elegidos en las listas. Format_UseBest está activado por defecto. Elige la resolución más alta que ofrece la cámara, hasta 1080p, y prefiere MJPG. Un formato vacío también recurre a él.

### Seleccionar el micrófono

El micrófono funciona igual. Audio_CaptureDevices los enumera, y usamos el elegido en la lista.

### MP4HWOutput: cualquier GPU, alternativa en la CPU

Para la salida, cree un MP4HWOutput, escriba output.mp4 en la carpeta Videos y cambie el motor al modo de captura de video, para que grabe y no solo muestre la vista previa. MP4HWOutput codifica en la GPU: NVENC en NVIDIA, Quick Sync en Intel y AMF en AMD, en H.264 o HEVC. Cuando no hay codificador por hardware, se encarga el codificador de Windows en la CPU. El código y el archivo son los mismos en cualquier PC. HWEncodersAvailable indica qué codificadores tiene este PC. Use primero NVIDIA, luego Intel, luego AMD, y recurra al codificador de la CPU.

### Ajustar el codificador H.264

Después ajuste el codificador: seis mil kilobits por segundo de media, picos de hasta nueve mil, el perfil Main y audio AAC a ciento noventa y dos. Como guía: de 2500 a 4000 kilobits para 720p, de 5000 a 8000 para 1080p y de 15000 a 25000 para 4K. Ambas tasas de bits se indican en kilobits por segundo.

### Añadir texto superpuesto

Los efectos de video añaden un texto superpuesto: un Hello World fijo y un reloj que muestra la hora actual en cada fotograma.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve cuando la captura está en marcha, y StopAsync finaliza el archivo MP4. Ejecute la aplicación desde Visual Studio. La lista de formatos viene directamente de la cámara. Start inicia la grabación, con el reloj superpuesto en marcha. Stop finaliza output.mp4. El archivo queda en Videos: 1920x1080, video H.264 y audio AAC.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Una cámara que ya está abierta en otra aplicación. Paquetes x86 y x64 mezclados en un mismo proyecto. Y liberar el motor en plena captura: espere primero a StopAsync. Los errores del motor llegan en el evento OnError.

### Próximos pasos

A continuación, creamos la misma aplicación en WPF. También hay guías sobre cámaras IP, grabación de pantalla, capturas de imágenes y streaming por red. El código completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4 en WPF](video-capture-webcam-mp4-wpf.md) — la misma aplicación como proyecto WPF, con la ventana en XAML
- [Grabación Pre-Evento](../guides/pre-event-recording.md) — grabación con buffer circular con disparadores de detección de movimiento para webcams y cámaras IP
- [Vista Previa de Video de Cámara](camera-video-preview.md) — mostrar video de cámara en vivo con captura de fotogramas

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación.