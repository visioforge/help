---
title: Grabar webcam a WMV en C# WinForms (WMV 9, WMA, CBR)
description: "Grabe una webcam en WMV en una app C# WinForms con VideoCaptureCore: Windows Media Video 9 a tasa de bits constante, audio WMA y comprobación con ffprobe."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - WMV
  - WMA
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - WMVOutput
  - WMVStreamMode
  - VideoView
video:
  id: EoOfXkhOtP8
  upload_date: 2026-11-14T07:13:25Z
  duration: PT12M20S

---

# Grabación de webcam a archivo WMV en aplicaciones .NET C#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba una webcam en un archivo WMV desde una aplicación C# WinForms con el motor VideoCaptureCore. El video es Windows Media Video 9 a la resolución propia de la cámara y a 8 Mbps constantes; el audio es WMA, en un formato elegido de la lista que devuelve el SDK.

**Cuándo conviene WMV en lugar de MP4:** un archivo WMV se reproduce en Windows sin instalar nada, y los codificadores de Windows Media vienen con Windows, así que no necesita GPU ni paquete de códecs. Con una tasa de bits constante, el tamaño del archivo se conoce antes de grabar, y algunos sistemas ya esperan archivos WMV o ASF. Para la web, los teléfonos y los Mac, grabe [MP4 con H.264](video-capture-webcam-mp4.md) en su lugar.

Este tutorial usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK. VideoCaptureCoreX, el motor multiplataforma, tiene su propio codificador WMV: consulte [Salida WMV multiplataforma](../../general/output-formats/wmv.md#salida-wmv-multiplataforma) y [Guardar video de webcam en C#](../guides/save-webcam-video.md).

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EoOfXkhOtP8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-wmv-winforms)

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y el redist de captura de video con sus filtros DirectShow. Los codificadores de Windows Media vienen con Windows, así que no se necesita nada más:

- Para aplicaciones de 64 bits: [Redist de captura de video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Para aplicaciones de 32 bits: [Redist de captura de video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Establezca `PlatformTarget` en el archivo del proyecto según el redist (`x64` o `x86`). En las ediciones N de Windows, los componentes de Windows Media vienen con el Media Feature Pack: instálelo primero.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Ejemplo de implementación con código C#

El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato, la velocidad de fotogramas, el micrófono, el códec de video, el códec de audio y el formato de audio, y los botones Start y Stop; el archivo del diseñador está en el código fuente anterior. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamWmv;

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

        // Los códecs de Windows Media que pueden codificar a una tasa de bits constante
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
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

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Las frecuencias de muestreo, los canales y las tasas de bits que ofrece este códec de audio
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 48000 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
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

        var mic = (string)cbMicrophone.SelectedItem;
        if (mic is not null && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Webcam to WMV";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        var frameRate = (VideoFrameRate)cbFrameRate.SelectedItem;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = frameRate;

        videoCapture1.Audio_RecordAudio = mic is not null; // sin micrófono: grabar solo video
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // grabar el micrófono sin reproducirlo

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 8000, // kbps
            Custom_Video_SizeSameAsInput = true, // la resolución de la cámara
            Custom_Video_FrameRate = frameRate.Value,
            Custom_Video_KeyFrameInterval = 2, // segundos

            Custom_Audio_StreamPresent = mic is not null,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
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
        // Terminar el WMV antes de que se cierre la ventana
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

1. **Selección de dispositivo y formato** - La cámara, su formato, su velocidad de fotogramas y el micrófono se eligen en listas que se llenan desde los propios dispositivos; sin micrófono, `Audio_RecordAudio` y `Custom_Audio_StreamPresent` son `false` y el WMV contiene solo video
2. **Formato de la cámara** - `Format_UseBest` está activado por defecto: elige la resolución más alta hasta 1080p y luego prefiere MJPG y la velocidad de fotogramas más rápida. Para grabar en 4K, o en cualquier formato que elija, desactívelo y establezca `Format` y `FrameRate`
3. **Códecs de Windows Media** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` y `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` devuelven los codificadores de Windows Media que pueden trabajar a tasa de bits constante, y `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` los formatos de un códec de audio, cada uno nombrado por su tasa de bits, frecuencia de muestreo, canales y bits. Windows Media Video 9 es el códec para video de cámara; los códecs Screen e Image están pensados para grabaciones de pantalla e imágenes fijas, Advanced Profile es el perfil VC-1 pensado para video entrelazado y de difusión, y V7 y V8 son versiones antiguas para reproductores antiguos. Tome cada nombre de códec y de formato de estas listas, nunca lo escriba a mano
4. **Configuración propia** - Un `new WMVOutput()` simple usa el perfil integrado "Windows Media Video 9 High (2.5 Mbps)" desde el SDK 2026.11.16; hasta la 2026.11.13 era "Windows Media Video 9 for Local Network (768 kbps)", el perfil que nombra el video. `WMVMode.CustomSettings` acepta su propio códec, tasa de bits y tamaño: `Custom_Video_Bitrate` está en kilobits por segundo, así que `8000` son 8 Mbps, `Custom_Video_SizeSameAsInput` mantiene la resolución de la cámara y `Custom_Video_KeyFrameInterval` está en segundos. Con audio a 128 kbps, son unos sesenta megabytes por minuto
5. **Operación asíncrona** - `StartAsync` y `StopAsync` mantienen la interfaz receptiva, y al cerrar el formulario primero se termina el archivo WMV

## Comprobar el archivo con ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=1920|height=1080
# stream|codec_name=wmav2|sample_rate=48000|channels=2
# format|format_name=asf|duration=16.496000|size=16941531
```

Dieciséis segundos y medio ocupan 16 941 531 bytes, unos 16 MB según el Explorador de archivos: 8 Mbps de video más 128 kbps de audio. ffprobe llama `wmv3` a WMV 9 y `wmav2` a WMA 9.2, y el contenedor es ASF, el formato que hay detrás de WMV.

## Transcripción del video

### Qué vamos a construir

Vamos a grabar una webcam en un archivo WMV en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una ventana WinForms con una vista previa en directo de la cámara. Graba Windows Media Video 9 a la resolución propia de la cámara, con una tasa de bits constante que fijamos nosotros: ocho megabits por segundo. El audio es WMA, en un formato elegido de una lista. La cámara, su formato, la velocidad de fotogramas y el micrófono también se eligen de listas. Esta es la aplicación terminada, grabando desde una Logitech C920.

### WMV o MP4

¿Cuándo conviene WMV? Un archivo WMV se reproduce en Windows sin instalar nada. Los codificadores de Windows Media vienen con Windows, así que no necesita GPU ni paquete de códecs. Con una tasa de bits constante, el tamaño del archivo se conoce antes de grabar. Y algunos sistemas ya esperan archivos WMV o ASF. Para la web, los teléfonos y los Mac, MP4 con H.264 es la mejor opción. Ese es el video de webcam a MP4.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una webcam con micrófono. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamWmv. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio SDK y el redist de captura de video con sus filtros DirectShow. Los codificadores WMV vienen con Windows, así que no hace falta nada más. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. El redist es el de x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa el redist x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un combo box para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara, la velocidad de fotogramas y el micrófono, y luego para el códec de video, el códec de audio y el formato de audio. Añada los botones Start y Stop. Stop queda desactivado hasta que empieza una grabación. Haga doble clic en el formulario, en las listas de cámara, formato y códec de audio, y en los dos botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título.

### Listar los dispositivos

Video_CaptureDevices devuelve todas las cámaras, y Audio_CaptureDevices todos los micrófonos. Listamos sus nombres y seleccionamos el primero de cada lista. Cuando cambia la cámara, su manejador llena la lista de formatos con los formatos de esa cámara, y elige 1920x1080 MJPG si existe. Cuando cambia el formato, su manejador llena la lista de velocidades de fotogramas del mismo modo, y elige treinta fotogramas por segundo.

### Códecs de Windows Media

De vuelta en el manejador Load, WMV_CustomProfile_VideoCodecs y WMV_CustomProfile_AudioCodecs listan los codificadores de Windows Media que pueden trabajar a tasa de bits constante. Llenamos las dos listas y seleccionamos Windows Media Video 9 y Windows Media Audio 9.2. Cuando cambia el códec de audio, WMV_CustomProfile_AudioFormats lista lo que ofrece ese códec. Elegimos 128 kilobits por segundo, 48 kilohercios estéreo. Windows Media Video 9 es el códec para el video de cámara. Los códecs Screen e Image están hechos para grabaciones de pantalla e imágenes fijas, y V7 y V8 son versiones antiguas, para reproductores antiguos. Para audio a tasa de bits constante están WMA 9.2 y WMA 10 Professional. Cada formato de audio indica su tasa de bits, frecuencia de muestreo, canales y bits. Elija siempre estos nombres de las listas; nunca los escriba a mano.

### Elegir el formato de la cámara

El manejador de Start comprueba primero que haya una velocidad de fotogramas y un códec de video seleccionados. Si no, lo indica en la barra de título. Si hay un micrófono en la lista, también necesita un formato de audio. Después selecciona la cámara por su nombre, desactiva Format_UseBest y pasa el formato y la velocidad de fotogramas elegidos en las listas. Format_UseBest está activado por defecto. Elige la resolución más alta hasta 1080p, y después prefiere MJPG y la velocidad de fotogramas más rápida. Para grabar en 4K, o en cualquier formato que elija, desactívelo y fije Format y FrameRate, como hacemos aquí.

### Seleccionar el micrófono

Sin ningún micrófono en la lista, la aplicación captura solo video. Si no, graba el seleccionado. Audio_PlayAudio está desactivado, así que el micrófono no suena por los altavoces mientras graba.

### WMVOutput: configuración propia

Ahora, la salida. Un WMVOutput en modo CustomSettings toma nuestra propia configuración. La secuencia de video usa el códec de la lista a 8000 kilobits por segundo constantes. SizeSameAsInput mantiene la resolución de la cámara, la velocidad de fotogramas es la que elegimos, y hay un fotograma clave cada dos segundos. La secuencia de audio toma el códec y el formato de las listas, y solo existe cuando tenemos micrófono. Luego escriba output.wmv en la carpeta Videos y cambie el motor al modo de captura de video, para que grabe y no solo muestre la vista previa. Un WMVOutput sin más usa un perfil integrado de 768 kilobits por segundo, pensado para una red local. (Desde el SDK 2026.11.16, el perfil predeterminado es "Windows Media Video 9 High (2.5 Mbps)".) CustomSettings nos da nuestro propio códec, tasa de bits y tamaño. Custom_Video_Bitrate está en kilobits por segundo, así que 8000 son ocho megabits. Con 128 kilobits de audio, son unos sesenta megabytes por minuto. Y un fotograma clave cada dos segundos da al reproductor un punto de búsqueda cada dos segundos.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve en cuanto la captura está en marcha, y StopAsync termina de escribir el archivo WMV.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el WMV sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el manejador ha terminado. Si no hay nada grabándose, el manejador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas de cámara, formato y micrófono se llenan con los dispositivos. La lista de códecs de video muestra los codificadores de Windows Media. Windows Media Video 9 queda seleccionado. Start inicia la grabación. La C920 tarda unos segundos en enviar su primer fotograma. Stop termina output.wmv y luego cerramos la ventana.

### Comprobar el archivo con ffprobe

El archivo queda en Videos: unos dieciséis megabytes por dieciséis segundos. ffprobe muestra lo que hay dentro. El contenedor es ASF, el formato que hay detrás de WMV. El video es WMV 9, que ffprobe llama wmv3, a 1920x1080. El audio es WMA, 48 kilohercios estéreo.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Nombres de códec y formato escritos a mano: tómelos de las listas. Custom_Video_Bitrate escrito en bits por segundo: va en kilobits. En las ediciones N de Windows, los componentes de Windows Media vienen con el Media Feature Pack, así que instálelo primero. Y cerrar la ventana a mitad de grabación: espere primero a StopAsync con await.

### Próximos pasos

A continuación, el video de webcam a MP4 guarda la misma cámara en archivos H.264 que se reproducen en todas partes. También hay guías para cámaras IP, grabación de pantalla e instantáneas. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4](video-capture-webcam-mp4.md) — la misma cámara guardada en archivos H.264 que se reproducen en todas partes
- [Webcam a AVI](video-capture-camera-avi.md) — cada fotograma es una imagen completa, para edición y análisis
- [Salida WMV](../../general/output-formats/wmv.md) — todos los modos de WMVOutput: perfiles internos, propios y externos

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
