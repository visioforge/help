---
title: Grabar la pantalla en AVI con C# WinForms (MJPEG, PCM, MP3)
description: "Grabe la pantalla y el sonido de los altavoces en AVI con C# WinForms y VideoCaptureCore: monitor, fotogramas, códec y comprobación con ffprobe."
sidebar_label: Captura de Pantalla a AVI
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoCaptureCoreX
  - MJPEGEncoderSettings
  - VideoView
video:
  id: V9iZiSx7slI
  upload_date: 2026-11-13T14:48:04Z
  duration: PT12M1S

---

# Guía de implementación de captura de pantalla a AVI en C\#

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba un monitor entero y el sonido de los altavoces en un archivo AVI desde una aplicación C# WinForms con el motor VideoCaptureCore. El monitor, la velocidad de fotogramas y el cursor del ratón se eligen en el formulario, el sonido viene de la fuente VisioForge What You Hear, el códec de video se elige entre los instalados en el PC (MJPEG por defecto) y el audio es PCM sin comprimir o MP3.

Esta parte usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK. En macOS, Linux, Android e iOS, VideoCaptureCoreX también graba AVI: consulte [VideoCaptureCoreX](#api-moderna-video-capture-sdk-x) más abajo.

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/V9iZiSx7slI?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-avi-winforms)

## Cuándo usar AVI para grabar la pantalla

- **Cada fotograma es una imagen completa** - con MJPEG, puede cortar y editar en cualquier fotograma, y leer cada fotograma por separado para su análisis, sin decodificar los fotogramas anteriores
- **Audio sin pérdidas** - un `AVIOutput` simple escribe PCM sin comprimir
- **Herramientas DirectShow** - AVI con MJPEG se abre en editores y reproductores de Windows basados en DirectShow

**Contrapartida:** el tamaño. En el video, una pantalla 4K a 10 fotogramas por segundo con MJPEG ocupa unos 30 MB por segundo: 679 MB en 22 segundos, unos 110 GB por hora. Para la web, los teléfonos y para compartir, grabe [MP4 con H.264](screen-capture-mp4.md) en su lugar.

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y el redist de captura de video. Sus filtros DirectShow incluyen la fuente de pantalla y la fuente de sonido What You Hear; AVI no necesita nada más.

Establezca `PlatformTarget` en el archivo del proyecto según el redist (`x64` o `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

Para una aplicación de 32 bits (x86), use en su lugar el paquete x86: `VisioForge.DotNet.Core.Redist.VideoCapture.x86`.

`VisioForge.DotNet.VideoCapture`, el paquete del bloque Try it al principio de esta página, también sirve: instala el mismo `VisioForge.DotNet.Core`. Los paquetes redist son necesarios en ambos casos.

## Ejemplo de implementación con código C\#

El formulario tiene un `VideoView`, listas desplegables para el monitor, la velocidad de fotogramas, el dispositivo de sonido y el códec de video, las casillas "Mouse cursor" y "Record sound" (ambas marcadas), una casilla "MP3 audio" y los botones Start y Stop; el archivo del diseñador está en el código fuente de arriba. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, `Form1.cs` se compila sin cambios en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()` por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureAvi;

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
        cbFrameRate.SelectedItem = 10;

        // "VisioForge What You Hear Source" graba lo que suena en los altavoces
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbMP3.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbMP3.Enabled = cbRecordSound.Checked; // MP3 necesita sonido

        // Los códecs de video instalados en este PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Screen to AVI";

        // El monitor entero a la velocidad de fotogramas de la lista
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication: más rápido que GDI
            DisplayIndex = (int)cbMonitor.SelectedItem!,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate((int)cbFrameRate.SelectedItem!)
        };

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        videoCapture1.Audio_RecordAudio = recordSound;
        // Ningún dispositivo sin sonido: un dispositivo What You Hear que se quede aquí sigue abriendo los altavoces
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // no reproducir nunca el sonido capturado en los altavoces

        // Audio: MP3, o PCM sin comprimir
        var avi = recordSound && cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
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

1. **Monitor** - `DisplayIndex` toma el número del final del nombre de dispositivo de Windows del monitor, `\\.\DISPLAY1` → `1`; `MonitorNumber` lo lee de `Screen.DeviceName`, y se selecciona el monitor principal
2. **Fuente de pantalla** - `FullScreen` graba el monitor entero a su resolución completa, y `AllowDesktopDuplicationEngine` captura mediante Windows Desktop Duplication en lugar de GDI. `GrabMouseCursor` está desactivado por defecto y `FrameRate` es 10 por defecto
3. **Sonido de los altavoces** - `VisioForge What You Hear Source`, del redist de captura de video, graba lo que suena en los altavoces. Con Record sound desmarcado, la aplicación establece `Audio_RecordAudio` en `false` y también borra `Audio_CaptureDevice`, y `Audio_PlayAudio` sigue en `false` para que el sonido grabado no se reproduzca de nuevo en los altavoces
4. **Códec de video** - `Video_Codecs()` devuelve los compresores de video DirectShow instalados en el PC; `AVIOutput.Video_Codec` toma uno de esos nombres y por defecto es `MJPEG Compressor`, que guarda cada fotograma como una imagen JPEG
5. **Audio** - Un `AVIOutput` simple escribe PCM sin comprimir, estéreo de 16 bits a 44,1 kHz, unos diez megabytes por minuto; `new AVIOutput(new MP3Output())` escribe MP3 a 192 kbps, alrededor de un megabyte y medio por minuto
6. **Operación asíncrona** - `StartAsync` y `StopAsync` mantienen la interfaz receptiva, y al cerrar el formulario primero se termina el archivo AVI

## Comprobar el archivo con ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=3840|height=2160
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=22.400000|size=679261184
```

Veintidós segundos de una pantalla 4K a 10 fotogramas por segundo ocupan 679 megabytes, aproximadamente 240 megabits por segundo; por eso AVI sirve para la edición y el análisis, no para compartir.

## Video Capture SDK X (VideoCaptureCoreX) { #api-moderna-video-capture-sdk-x }

La API multiplataforma usa `VideoCaptureCoreX`. Para el patrón completo de aplicación de consola con configuración de captura de pantalla, audio y ciclo de vida de grabación, consulte la guía de [Captura de Pantalla a MP4](screen-capture-mp4.md#api-moderna-video-capture-sdk-x). Para generar AVI en lugar de MP4, reemplace la configuración de salida:

```csharp
// AVI con codecs predeterminados (video H.264 + audio MP3)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### Opciones de Codec AVI

Elija el codificador de video según su flujo de trabajo:

```csharp
// MJPEG — independiente por fotograma, archivos más grandes, ideal para edición
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 en contenedor AVI — archivos más pequeños, menos amigable para edición
var h264AviOutput = new AVIOutput(outputPath);
h264AviOutput.Video = new OpenH264EncoderSettings();
```

Captura de región, grabación multi-monitor, audio, resaltado de cursor y codificación GPU se cubren en la [guía de MP4](screen-capture-mp4.md) — todas las funciones de configuración de fuente funcionan de forma idéntica con la salida AVI.

## Preguntas frecuentes

### ¿Por qué mi archivo de grabación de pantalla AVI es tan grande?

MJPEG comprime cada fotograma por separado, a cambio de facilitar la edición: una pantalla 4K a 10 fotogramas por segundo ocupa unos 30 MB por segundo. Para reducir el tamaño, baje la velocidad de fotogramas, capture una región más pequeña en lugar de la pantalla completa (`FullScreen = false` con `Left`, `Top`, `Right` y `Bottom`), o cambie a la [salida MP4](screen-capture-mp4.md), cuyo H.264 comprime entre fotogramas.

### ¿Cómo grabo el sonido del ordenador y no el del micrófono?

Seleccione `VisioForge What You Hear Source` como `AudioCaptureSource`: graba lo que suena en los altavoces y viene en el redist de captura de video. Deje `Audio_PlayAudio` en `false`, para que el sonido grabado no se reproduzca de nuevo en los altavoces.

### ¿Cuándo debo usar AVI en lugar de MP4 para grabar la pantalla?

Elija AVI cuando necesite acceso independiente a cada fotograma para la edición de video (MJPEG permite saltar a cualquier fotograma sin decodificar los anteriores), cuando necesite audio PCM sin pérdidas o cuando trabaje con herramientas basadas en DirectShow. Para todos los demás casos, MP4 con H.264 ofrece mejor compresión, archivos más pequeños y un soporte multiplataforma más amplio.

## Transcripción del video

### Qué vamos a construir

Vamos a grabar la pantalla y el sonido de los altavoces en un archivo AVI en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms que graba un monitor entero en un archivo AVI. El monitor, la velocidad de fotogramas y el cursor del ratón se eligen en el formulario. También graba el sonido que suena en los altavoces, como PCM sin comprimir o como MP3. Y el códec de video es cualquiera de los instalados en el PC, MJPEG por defecto. Esta es la aplicación terminada, grabando la pantalla en la que se ejecuta.

### AVI o MP4

¿Cuándo conviene AVI en lugar de MP4? Con MJPEG, cada fotograma de la grabación es una imagen completa, así que puede cortar y editar en cualquier fotograma, y leer cada fotograma por separado para analizarlo. El audio PCM no tiene pérdidas. El precio es el tamaño: aquí, unos veintiocho megabytes por segundo, para una pantalla 4K a diez fotogramas por segundo. Para la web, los teléfonos y compartir, MP4 con H.264 es la mejor opción. Ese es el video de captura de pantalla a MP4.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Esta vez no hace falta cámara ni micrófono. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto ScreenCaptureAvi. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio SDK y el redist de captura de video. Contiene los filtros DirectShow, entre ellos la fuente de pantalla y la fuente de sonido What You Hear. AVI no necesita nada más. En Visual Studio, es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. El redist es el de x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa el redist x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView, donde la aplicación muestra la vista previa de lo que graba. Añada una etiqueta y un combo box para el monitor, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para la velocidad de fotogramas, el dispositivo de sonido y el códec de video. Añada tres casillas: el cursor del ratón y Record sound, ambas marcadas, y MP3 audio. Luego, los botones Start y Stop. Stop queda desactivado hasta que empieza una grabación. Haga doble clic en el formulario y en los dos botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa. Los errores del motor llegan en el evento OnError. Aquí se muestran en la barra de título.

### Llenar las listas

El motor identifica un monitor por el número al final de su nombre de dispositivo de Windows: uno para DISPLAY1, dos para DISPLAY2. Un pequeño método auxiliar lee ese número. El manejador Load lista todos los monitores y selecciona el principal. La lista de velocidades de fotogramas contiene unos pocos valores fijos. Se seleccionan diez fotogramas por segundo, el valor predeterminado del propio SDK para la pantalla. Audio_CaptureDevices lista los dispositivos de sonido. VisioForge What You Hear Source graba todo lo que suena en los altavoces, así que se selecciona cuando está disponible. Sin ningún dispositivo de sonido, las dos casillas de sonido se desactivan. Y MP3 solo tiene sentido con sonido, así que sigue a Record sound. Video_Codecs lista los códecs de video instalados en este PC. Seleccionamos MJPEG Compressor, o el primer códec si MJPEG no está. Video_Codecs devuelve los compresores de video DirectShow de este PC. Este tiene dos: MJPEG Compressor y DV Video Encoder. MJPEG guarda cada fotograma como una imagen JPEG. DV solo escribe DV de definición estándar. Un códec que instale usted mismo aparece en la misma lista.

### Capturar la pantalla

El manejador Start comprueba primero que haya un códec de video seleccionado. Si no, lo indica en la barra de título. Después describe la fuente de pantalla. FullScreen graba el monitor entero. AllowDesktopDuplicationEngine captura mediante Windows Desktop Duplication en lugar de GDI. DisplayIndex es el número de monitor de la lista, GrabMouseCursor dibuja el cursor en el video, y FrameRate es la velocidad de la lista. Un breve resumen. FullScreen graba el monitor entero a su resolución completa. DisplayIndex es el número del nombre de dispositivo del monitor. El cursor está desactivado por defecto, y la velocidad de fotogramas es diez. Este video graba una pantalla 4K a diez fotogramas por segundo.

### Grabar el sonido de los altavoces

Con Record sound marcado, la aplicación graba el dispositivo seleccionado. Si está desmarcado, también se borra el dispositivo: un dispositivo What You Hear que se quede asignado sigue abriendo los altavoces. Audio_PlayAudio sigue desactivado, así que el sonido grabado no se reproduce de nuevo en los altavoces de los que viene.

### AVIOutput: audio PCM o MP3

Ahora, la salida. Un AVIOutput simple escribe audio PCM sin comprimir. Pase un MP3Output a su constructor, y el audio será MP3. Ponga en Video_Codec el nombre de la lista, escriba output.avi en la carpeta Videos y cambie el motor al modo de captura de pantalla. Un AVIOutput simple escribe PCM: estéreo de 16 bits a 44,1 kilohercios, sin pérdidas, unos diez megabytes por minuto. Con un MP3Output escribe MP3 a 192 kilobits por segundo, alrededor de un megabyte y medio por minuto. Al lado del video MJPEG, ambos son pequeños.

### Iniciar y detener

Start y Stop son asíncronos. StartAsync vuelve en cuanto la captura está en marcha, y StopAsync termina de escribir el archivo AVI.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el AVI sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el manejador ha terminado. Si no hay nada grabándose, el manejador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas se llenan: el monitor, la velocidad de fotogramas, el dispositivo de sonido y el códec. La lista de sonido tiene What You Hear y los micrófonos. What You Hear sigue seleccionado. Suena un sonido en los altavoces, y Start inicia la grabación. La vista previa muestra la pantalla con el cursor. Stop termina output.avi, y después cerramos la ventana.

### Comprobar el archivo con ffprobe

El archivo queda en Videos: unos seiscientos cincuenta megabytes por veintitrés segundos. ffprobe muestra lo que hay dentro. El contenedor es AVI. El video es MJPEG a 3840x2160. El audio es PCM, de 16 bits, estéreo a 44,1 kilohercios: el sonido de los altavoces. Son unos 240 megabits por segundo, y por eso AVI sirve para editar y analizar, no para compartir.

### Errores frecuentes

Algunos errores frecuentes. Falta un paquete redist, así que el motor no arranca. Un archivo sin sonido, porque Record sound estaba desmarcado, o se eligió un micrófono en lugar de What You Hear. Paquetes x86 y x64 mezclados en un mismo proyecto. Cerrar la ventana a mitad de una grabación: espere a StopAsync primero. Y AVI donde corresponde MP4: una hora de MJPEG en 4K ocupa unos cien gigabytes.

### Próximos pasos

A continuación, el video de captura de pantalla a MP4 guarda la pantalla en archivos H.264 pequeños. El video de webcam a AVI graba una cámara del mismo modo que aquí. Y la documentación muestra cómo grabar una región o una sola ventana en lugar de la pantalla entera. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Captura de Pantalla a MP4](screen-capture-mp4.md) — formato recomendado con cobertura completa de funciones (región, multi-monitor, audio, codificación GPU, multiplataforma)
- [Webcam a AVI](video-capture-camera-avi.md) — la misma salida AVI con una cámara y un micrófono
- [Captura de Pantalla a WMV](screen-capture-wmv.md) — alternativa en formato Windows Media
- [Captura de Pantalla en VB.NET](../guides/screen-capture-vb-net.md) — grabación de pantalla en Visual Basic .NET
- [Configuración de Fuente de Pantalla](../video-sources/screen.md) — referencia completa para configuración de captura
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — página del producto y descargas

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
