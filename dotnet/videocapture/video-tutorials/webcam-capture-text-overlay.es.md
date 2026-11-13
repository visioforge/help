---
title: Texto sobre webcam en C# WinForms (fecha, fotograma)
description: Dibuje un texto que cambia durante la grabación, la fecha y hora, el número de fotograma y el tiempo de grabación sobre un MP4 de webcam en C# WinForms.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - MP4
  - Text Overlay
  - Video Effects
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEffectTextLogo
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4Output
  - VideoView
video:
  id: wgRV_RT-txg
  upload_date: 2026-11-12T21:13:37Z
  duration: PT13M6S

---

# Captura de webcam con superposición de texto en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba una webcam en MP4 desde una aplicación C# WinForms con el motor VideoCaptureCore y dibuja cuatro textos sobre los fotogramas del video con `VideoEffectTextLogo`:

- un rótulo abajo en el centro, que puede cambiar durante la grabación;
- la fecha y la hora en la esquina superior derecha;
- el número de fotograma;
- el tiempo transcurrido desde el inicio de la grabación.

El texto forma parte de cada fotograma, así que aparece en la vista previa en vivo y en el archivo. La fecha y la hora en cada fotograma son lo que necesitan las grabaciones de seguridad y de evidencia, el número de fotograma permite encontrar y comparar más tarde un fotograma exacto, y el tiempo de grabación alinea el video con los registros y los datos de sensores.

Este tutorial usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK. Para fundidos, rotación y los motores X multiplataforma, consulte [Superposiciones de texto](../../general/video-effects/text-overlay.md).

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/wgRV_RT-txg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-text-overlay-winforms)

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/), el redist de captura de video con sus filtros DirectShow y el redist MP4, que escribe el archivo MP4:

- Para aplicaciones de 64 bits: [Redist de captura de video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/) y [Redist MP4 x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- Para aplicaciones de 32 bits: [Redist de captura de video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) y [Redist MP4 x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)

Establezca `PlatformTarget` en el archivo del proyecto según los redists (`x64` o `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.MP4.x64
```

## Ejemplo de implementación con código C#

El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato, la velocidad de fotogramas y el micrófono, un cuadro de texto para el rótulo con un botón Apply que empieza deshabilitado, y los botones Start y Stop; el archivo del diseñador está en el código fuente anterior. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamTextOverlay;

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

    private void AddOverlays()
    {
        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();

        // Un rótulo abajo, que Apply cambia durante la grabación
        using var font = new Font("Segoe UI", 40, FontStyle.Bold); // el efecto guarda su propia copia
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "caption")
        {
            Text = tbCaption.Text,
            Font = font,
            FontColor = Color.Yellow,
            HorizontalAlignment = TextLogoHorizontalAlignment.Center,
            VerticalAlignment = TextLogoVerticalAlignment.Bottom
        });

        // Fecha y hora en la esquina superior derecha
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime,
            DateTimeMask = "yyyy-MM-dd HH:mm:ss",
            Text = "",
            HorizontalAlignment = TextLogoHorizontalAlignment.Right,
            VerticalAlignment = TextLogoVerticalAlignment.Top
        });

        // El número de fotograma en la esquina superior izquierda
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "frame")
        {
            Mode = TextLogoMode.FrameNumber,
            Text = "Frame ",
            Left = 20,
            Top = 20
        });

        // Tiempo desde el inicio de la grabación, bajo el número de fotograma
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "time")
        {
            Mode = TextLogoMode.Timestamp,
            TimeStampMask = @"hh\:mm\:ss\.fff",
            Text = "Rec ",
            Left = 20,
            Top = 140 // el tamaño del texto sigue la escala de pantalla de Windows
        });
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam with text overlay";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // sin micrófono: grabar solo video
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // grabar el micrófono sin reproducirlo

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = new MP4Output(); // H.264 y AAC
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        AddOverlays();

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btApply.Enabled = started;
        btStart.Enabled = !started;
    }

    private void btApply_Click(object sender, EventArgs e)
    {
        // Cambiar el rótulo durante la grabación: texto nuevo y luego Update para redibujarlo
        if (videoCapture1.Video_Effects_Get("caption") is VideoEffectTextLogo caption)
        {
            caption.Text = tbCaption.Text;
            caption.Update();
        }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btApply.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Terminar el MP4 antes de que se cierre la ventana
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // ya se está iniciando o deteniendo

            btStop.Enabled = false;
            btApply.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // cerrar de nuevo cuando este controlador haya terminado
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Características clave de la implementación

1. **Efectos activados** - `Video_Effects_Enabled` es `false` por defecto, y entonces no se dibuja ninguna superposición. `Video_Effects_Clear` elimina las superposiciones de la grabación anterior, para que Start pueda ejecutarse de nuevo
2. **Posición** - `HorizontalAlignment` y `VerticalAlignment` colocan una superposición en un borde o en el centro, sea cual sea el tamaño del fotograma; sin ellos, `Left` y `Top` la colocan en píxeles del video. Sin nada configurado, una superposición es Arial 36 blanca en la esquina superior izquierda
3. **Modos** - `Mode` elige lo que muestra una superposición: `Text` dibuja la propiedad `Text`, `DateTime` el reloj del PC con el formato de `DateTimeMask`, `Timestamp` el tiempo desde el inicio de la grabación con el formato de `TimeStampMask` (un formato de `TimeSpan`, así que `:` y `.` necesitan una barra invertida), y `FrameNumber` el número del fotograma. En los tres últimos, `Text` se dibuja delante del valor, y su valor por defecto es `Hello!!!`, así que establézcalo en una cadena vacía o en su propio prefijo
4. **Cambiar una superposición durante la grabación** - `Video_Effects_Get` devuelve la superposición con ese nombre, o `null`. Cambie sus propiedades y llame a `Update()`: el motor la vuelve a dibujar desde el siguiente fotograma, y la grabación conserva el cambio desde ese fotograma. Sin `Update()`, el texto anterior se queda
5. **Fuentes** - el setter de `Font` guarda su propia copia de la fuente, así que la que usted pasa puede liberarse enseguida
6. **Tamaño del texto y escala de pantalla** - un tamaño de fuente en puntos se dibuja con los DPI de la pantalla, así que crece con la escala de pantalla de Windows y no se reduce con el fotograma: al 200 por ciento, una línea de Arial 36 mide unos 110 píxeles de alto sea cual sea el tamaño del fotograma. Este ejemplo deja espacio para ello con `Top = 140`, que basta hasta el 200 por ciento. Para obtener el mismo tamaño en todos los PC, cree la fuente en píxeles: `new Font("Arial", 36, GraphicsUnit.Pixel)`
7. **Coste** - una superposición cuyo texto cambia (el reloj, el número de fotograma, el tiempo de grabación o un rótulo después de `Update()`) se vuelve a dibujar en cada fotograma; mantenga pocas superposiciones de este tipo

## Comprobar el archivo con ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.mp4 -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=h264|width=1920|height=1080
# stream|codec_name=aac|sample_rate=48000|channels=2
# format|format_name=mov,mp4,m4a,3gp,3g2,mj2|duration=30.698667|size=4843264
```

## Transcripción del video

### Qué vamos a construir

Vamos a dibujar texto sobre una grabación de webcam en una aplicación C# WinForms con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms que graba una webcam en MP4 con vista previa en directo. Sobre el video dibuja un rótulo que puede cambiar mientras graba, la fecha y la hora, el número de fotograma y el tiempo transcurrido desde que empezó la grabación. Esta es la aplicación terminada, grabando desde una Logitech C920. El texto se dibuja dentro de los fotogramas del video, así que está en la vista previa y en el archivo. ¿Dónde resulta útil? La fecha y la hora en cada fotograma es lo que necesitan las grabaciones de seguridad y las que sirven como prueba. Un número de fotograma permite encontrar y comparar más tarde un fotograma exacto. El tiempo de grabación alinea el video con los registros y los datos de sensores. Y un rótulo indica lo que se ve en cámara, como el operador, la prueba o el puesto, y puede cambiarlo cuando cambian las cosas.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una webcam, con un micrófono si quiere sonido. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación solo funciona en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamTextOverlay. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet: el propio SDK, el redist de captura de video con sus filtros DirectShow y el redist de MP4, que escribe el archivo MP4. En Visual Studio, eso es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. Y el redist de MP4 para x64. Los dos redists son x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa en su lugar los redists x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un cuadro combinado para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara, la velocidad de fotogramas y el micrófono. Después una etiqueta y un cuadro de texto para el rótulo, y un botón Apply. Apply empieza deshabilitado, porque solo funciona mientras se graba. Añada los botones Start y Stop. Stop sigue deshabilitado hasta que empieza una grabación. Haga doble clic en el formulario, en las listas de cámara y de formato, y en los tres botones. Visual Studio añade un controlador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. Los tipos de las superposiciones de texto están en VideoEffects. En el controlador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo. Los errores del motor llegan en el evento OnError. Aquí van a la barra de título.

### Listar los dispositivos

Video_CaptureDevices devuelve todas las cámaras, y Audio_CaptureDevices todos los micrófonos. Listamos sus nombres y seleccionamos el primero de cada lista. Cuando cambia la cámara, su controlador llena la lista de formatos con los formatos de esa cámara, y elige 1920x1080 MJPG si está disponible. Cuando cambia el formato, su controlador llena de la misma manera la lista de velocidades de fotogramas, y elige treinta fotogramas por segundo.

### Cámara, micrófono y MP4

El controlador de Start comprueba primero que haya una velocidad de fotogramas seleccionada. Después selecciona la cámara por su nombre, desactiva Format_UseBest y pasa el formato y la velocidad de fotogramas de las listas. Si se deja activado, Format_UseBest elige por sí mismo la resolución más alta hasta 1080p. Sin micrófono, la aplicación solo captura video. Si no, graba el seleccionado, sin reproducirlo por los altavoces. La salida es output.mp4 en la carpeta Videos. Un MP4Output sin más escribe video H.264 y audio AAC, codificados en la CPU, o con Intel Quick Sync si el PC lo tiene.

### Dibujar texto sobre el video

Las superposiciones tienen su propio método. Video_Effects_Enabled activa los efectos de video: están desactivados por defecto, y entonces no se dibuja nada. Video_Effects_Clear quita las superposiciones de la grabación anterior, para que Start pueda ejecutarse de nuevo. La primera superposición es el rótulo: un VideoEffectTextLogo llamado caption, con el texto del cuadro de texto. Es Segoe UI, de 40 puntos, en negrita y amarillo. Las dos alineaciones lo colocan abajo en el centro, sea cual sea el tamaño del fotograma. El efecto guarda su propia copia de la fuente, así que la nuestra se libera enseguida. Cada superposición de texto es un VideoEffectTextLogo. Su nombre permite encontrarla de nuevo más tarde. Text, Font y FontColor definen lo que dibuja. Left y Top la colocan en píxeles del video, o las propiedades de alineación la ponen en un borde o en el centro. Sin ellas, es Arial blanca de 36 puntos, en la esquina superior izquierda.

### Modos de texto: reloj, fotograma, tiempo

Ahora, la fecha y la hora. El modo DateTime dibuja el reloj del PC, con el formato de DateTimeMask y las cadenas de formato habituales de .NET. Text se dibuja delante del valor, así que lo dejamos como cadena vacía. Su valor por defecto es Hello. La alineación Right y Top pone el reloj en la esquina superior derecha. Mode elige lo que muestra una superposición. Text dibuja la propiedad Text. DateTime dibuja el reloj del PC. Timestamp dibuja el tiempo desde que empezó la grabación, con el formato de TimeStampMask. FrameNumber dibuja el número del fotograma. En los tres últimos, Text va delante del valor. El número de fotograma va en la esquina superior izquierda, con el prefijo Frame. Esta vez Left y Top lo colocan, en píxeles. Y debajo, el tiempo de grabación, con el prefijo Rec, en horas, minutos, segundos y milisegundos. En un formato de TimeSpan, los dos puntos y el punto necesitan una barra invertida. Deje espacio entre las filas: el tamaño del texto sigue la escala de pantalla de Windows, así que al 200 % es el doble de alto.

### Iniciar y detener

De vuelta en el controlador de Start, AddOverlays se ejecuta antes de StartAsync. StartAsync vuelve cuando la captura está en marcha, y entonces se habilitan Stop y Apply. Stop deshabilita Stop y Apply, y StopAsync termina de escribir el archivo MP4.

### Cambiar el rótulo durante la grabación

Apply cambia el rótulo durante la grabación. Video_Effects_Get encuentra la superposición por su nombre. Asignamos el texto nuevo y llamamos a Update, que indica al motor que vuelva a dibujar la superposición en el siguiente fotograma. Sin Update, se queda el texto antiguo. Video_Effects_Get devuelve la superposición que añadió, o null si ninguna superposición tiene ese nombre. Puede cambiar cualquiera de sus propiedades, como el texto, el color o la posición. Después llame a Update, y se vuelve a dibujar desde el siguiente fotograma. La grabación conserva el cambio desde ese fotograma.

### Cerrar la ventana

Cerrar la ventana mientras graba dejaría el MP4 sin terminar. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este controlador. Mientras se graba, cancela el cierre. Después detiene la grabación y vuelve a cerrar la ventana cuando el controlador ha terminado. Si no se está grabando, el controlador libera el motor.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas de cámara, formato y micrófono se llenan con los dispositivos. Start inicia la grabación. La C920 tarda unos segundos en enviar su primer fotograma. Luego aparecen sobre el video el rótulo, el reloj, el número de fotograma y el tiempo de grabación. Escriba un rótulo nuevo y haga clic en Apply. El texto nuevo aparece en el siguiente fotograma. Stop termina output.mp4, y luego cerramos la ventana.

### Comprobar el archivo

Este es un fotograma de output.mp4, a los pocos segundos. El rótulo, el reloj, el número de fotograma y el tiempo de grabación forman parte de la imagen. Y un fotograma después de Apply. A partir de ese fotograma, el archivo tiene el rótulo nuevo. ffprobe muestra lo que hay dentro: video H.264 a 1920x1080 y audio AAC, en un archivo MP4.

### Errores frecuentes

Algunos errores frecuentes. Video_Effects_Enabled se queda en false, y no se dibuja nada. Un reloj que empieza por Hello, porque no se vació Text. Un Text nuevo sin Update, y se queda el texto antiguo. Video_Effects_Get con otro nombre, que devuelve null. Y muchas superposiciones cuyo texto cambia: cada una se vuelve a dibujar en cada fotograma, así que use pocas.

### Próximos pasos

Después, el video de webcam a MP4 codifica en la GPU, y el video de webcam a AVI crea archivos para edición. Para imágenes sobre el video está VideoEffectImageLogo, y para un texto en movimiento, VideoEffectScrollingTextLogo. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4](video-capture-webcam-mp4.md) — la misma cámara guardada en MP4, con codificación por GPU
- [Webcam a AVI](video-capture-camera-avi.md) — archivos MJPEG y PCM para edición y análisis
- [Superposiciones de texto](../../general/video-effects/text-overlay.md) — fundidos, rotación y volteos, y superposiciones de texto en los motores X multiplataforma

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y ejemplos de implementación.
