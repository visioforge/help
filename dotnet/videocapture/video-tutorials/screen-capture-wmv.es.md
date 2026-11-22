---
title: Grabar la pantalla en WMV con C# WinForms (WMV 9, WMA, CBR)
description: "Grabe la pantalla y el sonido de los altavoces en WMV con C# WinForms y VideoCaptureCore: códecs Windows Media a bitrate constante, con ffprobe."
sidebar_label: Captura de Pantalla a WMV
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - WMVOutput
  - VideoCaptureCoreX
  - VideoView
video:
  id: gpaVniBKXBw
  upload_date: 2026-11-16T06:07:30Z
  duration: PT13M21S

---

# Guía de implementación de captura de pantalla a WMV en C\#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba un monitor entero y el sonido de los altavoces en un archivo WMV desde una aplicación C# WinForms con el motor VideoCaptureCore. El monitor, la velocidad de fotogramas y el cursor del ratón se eligen en el formulario, y el sonido viene de la fuente VisioForge What You Hear. La salida es un `WMVOutput` con configuración propia: Windows Media Video 9 a 10 Mbps constantes y audio WMA, con los códecs y el formato de audio elegidos de las listas que ofrece Windows.

Esta parte usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK. VideoCaptureCoreX también escribe WMV: consulte [VideoCaptureCoreX](#api-moderna-video-capture-sdk-x) más abajo.

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/gpaVniBKXBw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-wmv-winforms)

## Cuándo usar WMV para grabar la pantalla

WMV (Windows Media Video) usa los códecs Windows Media de Microsoft y el contenedor ASF. Sigue siendo útil en escenarios concretos centrados en Windows:

- **Reproducción nativa en Windows** - los archivos WMV se reproducen en Windows sin instalar nada, y los codificadores de Windows Media vienen con Windows: sin GPU y sin paquete de códecs
- **Un tamaño de archivo conocido** - con una velocidad de bits constante, el tamaño del archivo tiene un límite que conoce antes de grabar
- **Archivos más pequeños que AVI** - en el video, una pantalla 4K a 15 fotogramas por segundo ocupa unos 33 MB por minuto; en el [tutorial de AVI](screen-capture-avi.md), una pantalla 4K a solo 10 fotogramas por segundo con MJPEG ocupa unos 30 MB por segundo
- **Entornos empresariales heredados** - muchos entornos corporativos estandarizan los formatos Windows Media para la distribución interna de video

**Contrapartida:** WMV es un formato centrado en Windows, con soporte limitado en macOS, Linux y los teléfonos. Para la compatibilidad multiplataforma, [MP4 es el formato recomendado](screen-capture-mp4.md).

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y el redist de captura de video. Sus filtros DirectShow incluyen la fuente de pantalla y la fuente de sonido What You Hear; los codificadores WMV vienen con Windows.

Establezca `PlatformTarget` en el archivo del proyecto según el redist (`x64` o `x86`). En las ediciones N de Windows, instale el Media Feature Pack para tener los codificadores de Windows Media.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

Para una aplicación de 32 bits (x86), use en su lugar el paquete x86: `VisioForge.DotNet.Core.Redist.VideoCapture.x86`.

`VisioForge.DotNet.VideoCapture`, el paquete del bloque Try it al principio de esta página, también sirve: instala el mismo `VisioForge.DotNet.Core`. Los paquetes redist son necesarios en ambos casos.

## Ejemplo de implementación con código C\#

El formulario tiene un `VideoView`, listas desplegables para el monitor, la velocidad de fotogramas, el dispositivo de sonido, el códec de video, el códec de audio y el formato de audio, las casillas "Mouse cursor" y "Record sound" (ambas marcadas) y los botones Start y Stop; el archivo del diseñador está en el código fuente de arriba. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, `Form1.cs` se compila sin cambios en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()` por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureWmv;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

    public Form1()
    {
        InitializeComponent();
    }

    // El motor identifica un monitor por la N de su nombre de dispositivo, \\.\DISPLAYN
    private static int MonitorNumber(Screen screen) =>
        int.Parse(screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty));

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Mostrar los errores del SDK en la barra de título
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var screen in Screen.AllScreens)
            cbMonitor.Items.Add(MonitorNumber(screen));

        cbMonitor.SelectedItem = MonitorNumber(Screen.PrimaryScreen!);

        cbFrameRate.Items.AddRange(new object[] { 5, 10, 15, 25, 30 });
        cbFrameRate.SelectedItem = 15;

        // "VisioForge What You Hear Source" graba lo que suena en los altavoces
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbRecordSound.Checked; // los ajustes de audio necesitan sonido

        // Los códecs de Windows Media que codifican con una velocidad de bits constante
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
        if (cbAudioCodec.SelectedIndex < 0 && cbAudioCodec.Items.Count > 0) cbAudioCodec.SelectedIndex = 0;
    }

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // Las frecuencias de muestreo, canales y velocidades de bits que ofrece este códec de audio
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 44100 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        if (recordSound && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Screen to WMV";

        // El monitor entero a la velocidad de fotogramas de la lista
        var frameRate = (int)cbFrameRate.SelectedItem!;
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication: más rápido que GDI
            DisplayIndex = (int)cbMonitor.SelectedItem!,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate(frameRate)
        };

        videoCapture1.Audio_RecordAudio = recordSound;
        // Ningún dispositivo sin sonido: un dispositivo What You Hear que se quede aquí sigue abriendo los altavoces
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // no reproducir nunca el sonido capturado en los altavoces

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 10000, // kbps
            Custom_Video_SizeSameAsInput = true, // la resolución del monitor
            Custom_Video_FrameRate = frameRate,
            Custom_Video_KeyFrameInterval = 2, // segundos

            Custom_Audio_StreamPresent = recordSound,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
        videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

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

1. **Monitor** - `DisplayIndex` toma el número del final del nombre de dispositivo de Windows del monitor, `\\.\DISPLAY1` → `1`; `MonitorNumber` lo lee de `Screen.DeviceName`, y se selecciona el monitor principal
2. **Fuente de pantalla** - `FullScreen` graba el monitor entero a su resolución completa, y `AllowDesktopDuplicationEngine` captura mediante Windows Desktop Duplication en lugar de GDI. `GrabMouseCursor` está desactivado por defecto y `FrameRate` es 10 por defecto; la aplicación graba a 15
3. **Sonido de los altavoces** - `VisioForge What You Hear Source`, del redist de captura de video, graba lo que suena en los altavoces. Con Record sound desmarcado, la aplicación establece `Audio_RecordAudio` en `false` y también borra `Audio_CaptureDevice`, y `Audio_PlayAudio` sigue en `false` para que el sonido grabado no se reproduzca de nuevo en los altavoces
4. **Códecs de Windows Media** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` y `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` listan los codificadores de Windows Media que trabajan con una velocidad de bits constante, y `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` lista los formatos de un códec de audio. Elija los nombres de estas listas en lugar de escribirlos. Windows Media Video 9 graba una pantalla 4K
5. **Configuración propia** - `new WMVOutput()` sin más usa un perfil integrado; `Mode = WMVMode.CustomSettings` toma en su lugar los valores `Custom_Video_*` y `Custom_Audio_*`. `Custom_Video_Bitrate` está en kbps, así que `10000` son 10 Mbps; `Custom_Video_SizeSameAsInput` mantiene la resolución del monitor, y `Custom_Video_KeyFrameInterval` está en segundos. El flujo de audio solo se escribe cuando `Custom_Audio_StreamPresent` es `true`
6. **Operación asíncrona** - `StartAsync` y `StopAsync` mantienen la interfaz receptiva, y al cerrar el formulario primero se termina el archivo WMV

## Comprobar el archivo con ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=3840|height=2160
# stream|codec_name=wmav2|sample_rate=44100|channels=2
# format|format_name=asf|duration=16.731000|size=9157531
```

El contenedor es ASF, el formato detrás de WMV. `wmv3` es el nombre que ffprobe da a Windows Media Video 9, y `wmav2` es el flujo WMA. Diecisiete segundos de una pantalla 4K ocupan 9,2 megabytes (8,7 MB según el Explorador de Windows), es decir, 4,4 Mbps: 10000 kbps es el techo, y un escritorio que apenas se mueve necesita menos.

## Video Capture SDK X (VideoCaptureCoreX) { #api-moderna-video-capture-sdk-x }

La API multiplataforma usa `VideoCaptureCoreX`. Para el patrón completo de aplicación de consola con configuración de captura de pantalla, audio y ciclo de vida de grabación, consulte la guía de [Captura de Pantalla a MP4](screen-capture-mp4.md#api-moderna-video-capture-sdk-x). Para generar WMV en lugar de MP4, reemplace la configuración de salida:

```csharp
using VisioForge.Core.Types.X.Output;

// Salida WMV (codecs Windows Media Video + WMA audio)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

Captura de región, grabación multi-monitor, audio, resaltado de cursor y codificación GPU se cubren en la [guía de MP4](screen-capture-mp4.md) — todas las funciones de configuración de fuente funcionan de forma idéntica con la salida WMV.

## Preguntas frecuentes

### ¿Cuándo debo usar WMV en lugar de MP4 para grabar la pantalla?

Elija WMV cuando su público usa Windows y necesita reproducción nativa sin instalar códecs adicionales, cuando el tamaño del archivo debe quedar por debajo de un límite conocido, o cuando trabaja en entornos empresariales que estandarizan los formatos Windows Media. Para la distribución multiplataforma o la publicación web, MP4 con H.264 es la mejor opción: más dispositivos compatibles y mejor compresión.

### ¿Por qué mi archivo WMV no tiene sonido?

Compruebe que "Record sound" está activado y que está seleccionado `VisioForge What You Hear Source`, no un micrófono: graba lo que suena en los altavoces y viene en el redist de captura de video. Con el sonido activado, `Custom_Audio_StreamPresent` debe ser `true` y `Custom_Audio_Format` debe ser un nombre de `WMV_CustomProfile_AudioFormats` para el códec de audio seleccionado.

### ¿Cómo establezco la velocidad de bits del WMV?

Establezca `Mode = WMVMode.CustomSettings`, `Custom_Video_Mode = WMVStreamMode.CBR` y `Custom_Video_Bitrate` en kilobits por segundo: `10000` son 10 Mbps. La velocidad de bits del audio forma parte del nombre del formato de audio, como `CBR 128 kbps, 44100 Hz, Stereo, 16 bit`.

## Transcripción del video

### Qué vamos a construir

Vamos a grabar la pantalla y el sonido de los altavoces en un archivo WMV en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms que graba un monitor entero en un archivo WMV. El monitor, la velocidad de fotogramas y el cursor del ratón se eligen en el formulario. También graba el sonido que suena en los altavoces, como audio WMA. El video es Windows Media Video 9, con una velocidad de bits constante que fijamos nosotros: diez megabits por segundo. Y los códecs y el formato de audio salen de las listas que ofrece Windows. Esta es la aplicación terminada, grabando la pantalla en la que se ejecuta.

### WMV, AVI o MP4

¿Cuándo conviene WMV? Un archivo WMV se reproduce en Windows sin instalar nada. Los codificadores de Windows Media vienen con Windows, así que no necesita GPU ni paquete de códecs. Con una velocidad de bits constante, el tamaño del archivo tiene un límite conocido antes de grabar. Y es pequeño: aquí unos treinta megabytes por minuto, para una pantalla 4K a quince fotogramas por segundo. La misma pantalla en AVI con MJPEG ocupaba unos veintiocho megabytes cada segundo. Para la web, los teléfonos y los Mac, MP4 con H.264 es la mejor opción. Ese es el video de captura de pantalla a MP4.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Esta vez no hace falta cámara ni micrófono. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto ScreenCaptureWmv. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio SDK y el redist de captura de video. Contiene los filtros DirectShow, entre ellos la fuente de pantalla y la fuente de sonido What You Hear. Los codificadores WMV vienen con Windows, así que no hace falta nada más. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. El redist es el de x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa el redist x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView, donde la aplicación muestra la vista previa de lo que graba. Añada una etiqueta y un combo box para el monitor, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para la velocidad de fotogramas, el dispositivo de sonido, el códec de video, el códec de audio y el formato de audio. Añada dos casillas, ambas marcadas: el cursor del ratón y Record sound. Luego, los botones Start y Stop. Stop queda desactivado hasta que empieza una grabación. Haga doble clic en el formulario, en la lista de códecs de audio y en los dos botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título.

### Llenar las listas

El motor identifica un monitor por el número al final de su nombre de dispositivo de Windows: uno para DISPLAY1, dos para DISPLAY2. Un pequeño método auxiliar lee ese número. El manejador Load lista todos los monitores y selecciona el principal. La lista de velocidades de fotogramas contiene unos pocos valores fijos. Se selecciona quince fotogramas por segundo. Audio_CaptureDevices lista los dispositivos de sonido. VisioForge What You Hear Source graba todo lo que suena en los altavoces, así que se selecciona cuando está disponible. Si no hay ningún dispositivo de sonido, Record sound y las dos listas de audio quedan desactivadas. Y el códec y el formato de audio solo importan con sonido, así que siguen a Record sound.

### Códecs de Windows Media

WMV_CustomProfile_VideoCodecs y WMV_CustomProfile_AudioCodecs listan los codificadores de Windows Media que pueden trabajar con una velocidad de bits constante. Llenamos las dos listas y seleccionamos Windows Media Video 9 y Windows Media Audio 9.2, o el primer códec si alguno no está. Cuando cambia el códec de audio, WMV_CustomProfile_AudioFormats lista lo que ofrece ese códec. Elegimos 128 kilobits por segundo, 44,1 kilohercios estéreo: la frecuencia que entrega What You Hear aquí, así que no se remuestrea nada. En este PC la lista de video tiene siete entradas: Windows Media Video 9, su Advanced Profile, los códecs Screen e Image, y los antiguos V7 y V8. Windows Media Video 9 graba esta pantalla 4K, así que es nuestra opción por defecto. Para audio a tasa de bits constante están WMA 9.2 y WMA 10 Professional. Cada formato de audio indica su tasa de bits, frecuencia de muestreo, canales y bits. Elija siempre estos nombres de las listas; nunca los escriba a mano.

### Capturar la pantalla

El manejador Start comprueba primero que haya un códec de video seleccionado. Si no, lo indica en la barra de título. El sonido solo se graba si Record sound está marcado y hay un dispositivo seleccionado. Entonces también hace falta un formato de audio. Después describe la fuente de pantalla. FullScreen graba el monitor entero. AllowDesktopDuplicationEngine captura mediante Windows Desktop Duplication en lugar de GDI. DisplayIndex es el número de monitor de la lista, GrabMouseCursor dibuja el cursor en el video, y FrameRate es la velocidad de la lista. Un breve resumen. FullScreen graba el monitor entero a su resolución completa. DisplayIndex es el número del nombre de dispositivo del monitor. El cursor está desactivado por defecto, y la velocidad de fotogramas es diez. Este video graba una pantalla 4K a quince fotogramas por segundo.

### Grabar el sonido de los altavoces

Con el sonido activado, la aplicación graba el dispositivo seleccionado. Con el sonido desactivado, el dispositivo también se borra: un dispositivo What You Hear que se quede ahí sigue abriendo los altavoces. Audio_PlayAudio sigue desactivado, así que el sonido grabado no se reproduce de nuevo en los altavoces de los que viene.

### WMVOutput: configuración propia

Ahora, la salida. Un WMVOutput en modo CustomSettings usa nuestros propios ajustes en lugar de un perfil integrado. El flujo de video usa el códec de la lista a 10000 kilobits por segundo constantes. SizeSameAsInput mantiene la resolución del monitor, la velocidad de fotogramas es la que elegimos y hay un fotograma clave cada dos segundos. El flujo de audio toma el códec y el formato de las listas, y solo está presente cuando el sonido está activado. Luego escriba output.wmv en la carpeta Videos y ponga el motor en modo de captura de pantalla. Un WMVOutput sin más usa un perfil integrado. CustomSettings nos da nuestro propio códec, tasa de bits y tamaño. Custom_Video_Bitrate está en kilobits por segundo, así que 10000 son diez megabits. Es el techo: un escritorio que apenas se mueve necesita menos, y esta grabación quedó en unos 4,4 megabits por segundo. Y un fotograma clave cada dos segundos da al reproductor un punto de búsqueda cada dos segundos.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve en cuanto la captura está en marcha, y StopAsync termina de escribir el archivo WMV.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el WMV sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el manejador ha terminado. Si no hay nada grabándose, el manejador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas se llenan: el monitor, la velocidad de fotogramas, el dispositivo de sonido, los dos códecs y el formato de audio. La lista de formatos de audio tiene cada velocidad de bits y frecuencia de muestreo que ofrece el códec. El nuestro sigue seleccionado. Suena un sonido en los altavoces, y Start inicia la grabación. La vista previa muestra la pantalla con el cursor. Stop termina output.wmv y luego cerramos la ventana.

### Comprobar el archivo con ffprobe

El archivo queda en Videos: menos de nueve megabytes para diecisiete segundos de una pantalla 4K. ffprobe muestra lo que hay dentro. El contenedor es ASF, el formato que hay detrás de WMV. El video es Windows Media Video 9, que ffprobe llama wmv3, a 3840x2160. El audio es WMA, 44,1 kilohercios estéreo: el sonido de los altavoces.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Un archivo sin sonido, porque Record sound estaba desmarcado, o se eligió un micrófono en lugar de What You Hear. Nombres de códec y formato escritos a mano: tómelos de las listas. Custom_Video_Bitrate escrito en bits por segundo: va en kilobits. Y cerrar la ventana a mitad de grabación: espere primero a StopAsync con await.

### Próximos pasos

A continuación, el video de captura de pantalla a MP4 guarda la pantalla en archivos H.264 que se reproducen en todas partes. El video de captura de pantalla a AVI guarda cada fotograma como una imagen completa, para editar. Y el video de webcam a WMV graba una cámara con los mismos ajustes de Windows Media que aquí. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Captura de Pantalla a MP4](screen-capture-mp4.md) — formato recomendado con cobertura completa de funciones (región, multi-monitor, audio, codificación GPU, multiplataforma)
- [Captura de Pantalla a AVI](screen-capture-avi.md) — formato AVI con MJPEG para edición independiente de fotogramas
- [Webcam a WMV](video-capture-webcam-wmv.md) — los mismos ajustes de Windows Media con una cámara y un micrófono
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para configuración de captura
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
