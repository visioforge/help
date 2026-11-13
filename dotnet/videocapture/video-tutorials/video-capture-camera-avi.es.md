---
title: Grabar webcam a AVI en C# WinForms (MJPEG, PCM, MP3)
description: "Grabe una webcam en AVI en una app C# WinForms con VideoCaptureCore: elija el códec de video, audio PCM o MP3, y compruebe el archivo con ffprobe."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - AVI
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoView
video:
  id: f5EXTFQETKA
  upload_date: 2026-11-12T15:20:03Z
  duration: PT10M56S

---

# Grabación de webcam a archivo AVI en aplicaciones .NET C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba una webcam en un archivo AVI desde una aplicación C# WinForms con el motor VideoCaptureCore. El códec de video se elige entre los instalados en el PC (MJPEG por defecto), y el audio es PCM sin comprimir o MP3.

**Cuándo conviene AVI en lugar de MP4:** con MJPEG, cada fotograma es una imagen completa, así que puede cortar y editar en cualquier fotograma y leer cada fotograma por separado para su análisis, y el audio PCM no tiene pérdidas. El precio es el tamaño: unos veinte megabytes por segundo a 1080p. Para la web, los teléfonos y para compartir, grabe [MP4 con H.264](video-capture-webcam-mp4.md) en su lugar.

Este tutorial usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK. En macOS, Linux, Android e iOS, VideoCaptureCoreX también graba AVI: consulte [Salida AVI](../../general/output-formats/avi.md) y [Guardar vídeo de webcam en C#](../guides/save-webcam-video.md).

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/f5EXTFQETKA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-avi-winforms)

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y el redist de captura de video con sus filtros DirectShow. AVI no necesita nada más:

- Para aplicaciones de 64 bits: [Redist de captura de video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Para aplicaciones de 32 bits: [Redist de captura de video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Establezca `PlatformTarget` en el archivo del proyecto según el redist (`x64` o `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Ejemplo de implementación con código C#

El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato, la velocidad de fotogramas, el micrófono y el códec de video, una casilla «MP3 audio» y los botones Start y Stop; el archivo del diseñador está en el código fuente anterior. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamAvi;

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
        cbMP3.Enabled = cbMicrophone.Items.Count > 0;

        // Los códecs de video instalados en este PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
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

        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Webcam to AVI";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // sin micrófono: grabar solo video
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // grabar el micrófono sin reproducirlo

        // Audio: MP3, o PCM sin comprimir
        var avi = cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

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
        // Terminar el AVI antes de que se cierre la ventana
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // ya se está iniciando o deteniendo

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // volver a cerrar cuando este controlador haya terminado
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Características clave de la implementación

1. **Selección de dispositivo y formato** - La cámara, su formato, su velocidad de fotogramas y el micrófono se eligen en listas que se llenan desde los propios dispositivos; sin micrófono, `Audio_RecordAudio` se establece en `false` y el AVI contiene solo video
2. **Formato de la cámara** - `Format_UseBest` está activado por defecto: elige la resolución más alta hasta 1080p y luego prefiere MJPG y la velocidad de fotogramas más rápida. Para grabar en 4K, o en cualquier formato que elija, desactívelo y establezca `Format` y `FrameRate`
3. **Códec de video** - `Video_Codecs()` devuelve los compresores de video DirectShow instalados en el PC; `AVIOutput.Video_Codec` acepta uno de esos nombres y por defecto es `MJPEG Compressor`, que guarda cada fotograma como una imagen JPEG. Un códec que instale usted mismo aparece en la misma lista
4. **Audio** - Un `AVIOutput` simple escribe PCM sin comprimir, 16 bits estéreo a 44,1 kHz, unos diez megabytes por minuto; `new AVIOutput(new MP3Output())` escribe MP3 a 192 kbps, alrededor de un megabyte y medio por minuto
5. **Operación asíncrona** - `StartAsync` y `StopAsync` mantienen la interfaz receptiva, y al cerrar el formulario primero se termina el archivo AVI

## Comprobar el archivo con ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=1920|height=1080
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=14.883073|size=316108288
```

Quince segundos de MJPEG a 1080p ocupan unos trescientos megabytes, aproximadamente 170 megabits por segundo; por eso AVI sirve para la edición y el análisis, no para compartir.

## Transcripción del video

### Qué vamos a construir

Vamos a capturar una webcam en un archivo AVI en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una ventana WinForms con una vista previa en directo de la cámara. Graba un archivo AVI con cualquier códec de video instalado en el PC, MJPEG por defecto. El audio es PCM sin comprimir, o MP3 si marca una casilla. La cámara, su formato, la velocidad de fotogramas y el micrófono se eligen de listas. Esta es la aplicación terminada, grabando desde una Logitech C920.

### AVI o MP4

¿Cuándo conviene AVI en lugar de MP4? Con MJPEG, cada fotograma es una imagen completa, así que puede cortar y editar en cualquier fotograma, y leer cada fotograma por separado para analizarlo. El audio PCM no tiene pérdidas. El precio es el tamaño: aquí, unos veinte megabytes por segundo a 1080p. Para la web, los teléfonos y compartir, MP4 con H.264 es la mejor opción. Ese es el video de webcam a MP4.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una webcam con micrófono. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamAvi. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio SDK y el redist de captura de video con sus filtros DirectShow. AVI no necesita nada más. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. El redist es el de x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa el redist x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un combo box para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara, la velocidad de fotogramas, el micrófono y el códec de video. Añada una casilla para el audio MP3, y los botones Start y Stop. Stop queda desactivado hasta que empieza una grabación. Haga doble clic en el formulario, en las listas de cámara y formato, y en los dos botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título.

### Listar los dispositivos

Video_CaptureDevices devuelve todas las cámaras, y Audio_CaptureDevices todos los micrófonos. Listamos sus nombres y seleccionamos el primero de cada lista. Sin micrófono, la casilla MP3 queda desactivada. Cuando cambia la cámara, su manejador llena la lista de formatos con los formatos de esa cámara, y elige 1920x1080 MJPG si existe. Cuando cambia el formato, su manejador llena la lista de velocidades de fotogramas del mismo modo, y elige treinta fotogramas por segundo.

### Elegir el formato de la cámara

El manejador de Start comprueba primero que haya una velocidad de fotogramas y un códec de video seleccionados. Si no, lo indica en la barra de título. Después selecciona la cámara por su nombre, desactiva Format_UseBest y pasa el formato y la velocidad de fotogramas elegidos en las listas. Format_UseBest está activado por defecto. Elige la resolución más alta hasta 1080p, y después prefiere MJPG y la velocidad de fotogramas más rápida. Para grabar en 4K, o en cualquier formato que elija, desactívelo y fije Format y FrameRate, como hacemos aquí.

### Seleccionar el micrófono

Sin ningún micrófono en la lista, la aplicación captura solo video. Si no, graba el seleccionado. Audio_PlayAudio está desactivado, así que el micrófono no suena por los altavoces mientras graba.

### Elegir el códec de video

De vuelta en el manejador Load, Video_Codecs lista los códecs de video instalados en este PC. Llenamos la lista de códecs y seleccionamos MJPEG Compressor. Video_Codecs devuelve los compresores de video DirectShow de este PC. Este tiene dos: MJPEG Compressor y DV Video Encoder. MJPEG guarda cada fotograma como una imagen JPEG. DV solo escribe DV de definición estándar. Un códec que instale usted mismo aparece en la misma lista.

### AVIOutput: audio PCM o MP3

Ahora, la salida. Un AVIOutput simple escribe audio PCM sin comprimir. Pase un MP3Output a su constructor, y el audio será MP3. Ponga en Video_Codec el nombre de la lista, escriba output.avi en la carpeta Videos y cambie el motor al modo de captura de video, para que grabe y no solo muestre la vista previa. Un AVIOutput simple escribe PCM: estéreo de 16 bits a 44,1 kilohercios, sin pérdidas, unos diez megabytes por minuto. Con un MP3Output escribe MP3 a 192 kilobits por segundo, alrededor de un megabyte y medio por minuto. Al lado del video MJPEG, ambos son pequeños.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve en cuanto la captura está en marcha, y StopAsync termina de escribir el archivo AVI.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el AVI sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el manejador ha terminado. Si no hay nada grabándose, el manejador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas de cámara, formato y micrófono se llenan con los dispositivos. La lista de códecs muestra los dos códecs de este PC. MJPEG sigue seleccionado. Start inicia la grabación. La C920 tarda unos segundos en enviar su primer fotograma. Stop termina output.avi, y después cerramos la ventana.

### Comprobar el archivo con ffprobe

El archivo queda en Videos: unos trescientos megabytes por quince segundos. ffprobe muestra lo que hay dentro. El contenedor es AVI. El video es MJPEG a 1920x1080. El audio es PCM, de 16 bits, estéreo a 44,1 kilohercios. Son unos 170 megabits por segundo, y por eso AVI sirve para editar y analizar, no para compartir.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Una cámara que ya está abierta en otra aplicación. Paquetes x86 y x64 mezclados en un mismo proyecto. Cerrar la ventana a mitad de una grabación: espere a StopAsync primero. Y AVI donde corresponde MP4: una hora de MJPEG a 1080p ocupa decenas de gigabytes.

### Próximos pasos

A continuación, el video de webcam a MP4 guarda la misma cámara en archivos H.264 pequeños. También hay guías para cámaras IP, grabación de pantalla e instantáneas. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4](video-capture-webcam-mp4.md) — la misma cámara guardada en archivos H.264 pequeños
- [Captura de pantalla a AVI](screen-capture-avi.md) — grabar la pantalla en un archivo AVI
- [Vista previa de video de cámara](camera-video-preview.md) — mostrar video de cámara en vivo con captura de fotogramas

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
