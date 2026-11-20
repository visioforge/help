---
title: "Webcam: vista previa y captura de fotogramas en C# WinForms"
description: Muestre la vista previa en directo de una webcam en C# WinForms con VideoCaptureCore y guarde el fotograma actual en un archivo JPEG con Frame_SaveAsync.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - Snapshot
  - JPEG
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoView
video:
  id: Qp9m9xJ_QCA
  upload_date: 2026-11-15T13:01:09Z
  duration: PT10M16S

---

# Vista previa de webcam y captura de fotogramas en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial muestra la vista previa en directo de una webcam en una aplicación C# WinForms con el motor VideoCaptureCore y guarda el fotograma actual en un archivo JPEG:

- la cámara, su formato y su velocidad de fotogramas se eligen en listas;
- el modo `VideoPreview` muestra la cámara sin escribir ningún archivo, así que no hace falta codificador ni formato de salida;
- un botón Save Frame escribe el fotograma actual con `Frame_SaveAsync`, a la resolución propia de la cámara, en un archivo nuevo con fecha y hora en la carpeta Imágenes;
- la aplicación abre el archivo guardado en el visor de imágenes predeterminado.

Así funcionan los fotomatones y las fotos de identificación, el escaneo de un documento o una etiqueta, los puestos de inspección que guardan una foto de cada pieza, y cualquier aplicación que necesite una imagen fija para procesarla o ponerla en un informe.

Este tutorial usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK.

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Qp9m9xJ_QCA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/camera-video-preview-winforms)

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) y el redist de captura de video con sus filtros DirectShow. Una vista previa no escribe ningún archivo, así que no necesita un paquete de codificador:

- Para aplicaciones de 64 bits: [Redist de captura de video x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Para aplicaciones de 32 bits: [Redist de captura de video x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Establezca `PlatformTarget` en el archivo del proyecto según el redist (`x64` o `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Ejemplo de implementación con código C#

El formulario tiene un `VideoView`, cuadros combinados para la cámara, el formato y la velocidad de fotogramas, y los botones Start, Stop y Save Frame; Stop y Save Frame empiezan deshabilitados. El archivo del diseñador está en el código fuente anterior. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()`, que .NET Framework no genera, por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`, y ponga `<dpiAware>true</dpiAware>` en un `app.manifest` para conservar la compatibilidad con el DPI del sistema que activa .NET 10.

```csharp
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamPreview;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

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

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
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
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam preview";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        // Solo video: la reproducción de audio está activada por defecto
        videoCapture1.Audio_PlayAudio = false;
        videoCapture1.Mode = VideoCaptureMode.VideoPreview;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btSaveFrame.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btSaveFrame_Click(object sender, EventArgs e)
    {
        // Un JPEG nuevo en Imágenes en cada clic, a la resolución propia de la cámara
        var file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            $"frame-{DateTime.Now:yyyyMMdd-HHmmss-fff}.jpg");

        if (!await videoCapture1.Frame_SaveAsync(file, ImageFormat.Jpeg, 85)) return;

        // Abrirlo en el visor predeterminado; sin ninguna aplicación para .jpg, solo queda la ruta en el título
        Text = file;
        try { Process.Start(new ProcessStartInfo(file) { UseShellExecute = true }); }
        catch (Win32Exception) { }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btSaveFrame.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Iniciando o deteniendo: esperar antes de cerrar
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true;
            return;
        }

        // Dispose detiene una vista previa en marcha: no hay ningún archivo que terminar
        videoCapture1.Dispose();
    }
}
```

## Características clave de la implementación

1. **Modo de vista previa** - `Mode = VideoCaptureMode.VideoPreview` pone la cámara en pantalla y no escribe ningún archivo, así que no hay `Output_Filename` ni `Output_Format` que configurar. Los fotogramas siguen llegando a la aplicación, y eso es lo que usa `Frame_SaveAsync`. Cambie el modo a `VideoCapture`, añada un archivo de salida y un formato, y la misma aplicación graba
2. **Formato de la cámara** - con `Format_UseBest` activado, el motor elige por sí mismo la resolución más alta hasta 1080p; el ejemplo lo desactiva y pasa el formato y la velocidad de fotogramas elegidos en las listas
3. **Audio** - `Audio_PlayAudio` es `true` por defecto y reproduce por los altavoces la fuente de audio seleccionada en `Audio_CaptureDevice`, o, en una capturadora cuyo filtro de video tiene un pin de audio, la propia fuente de video (`IsAudioSource`; el micrófono de una webcam USB es un dispositivo de audio aparte). El ejemplo solo muestra video, así que lo desactiva; si se deja activado sin ninguna de las dos, el motor lanza `OnError` ("The audio source is not configured"), pone `Audio_PlayAudio` en `false` y funciona sin sonido
4. **Guardar un fotograma** - `Frame_SaveAsync(filename, format, quality = 85, customWidth = 0, customHeight = 0)` escribe el fotograma actual en cualquier `ImageFormat`, como JPEG, PNG, BMP o TIFF. La calidad solo se aplica a JPEG. Indique a la vez un ancho y un alto para redimensionar el fotograma; sin ellos, el archivo tiene la resolución de la cámara (aquí 1920x1080), no el tamaño de la vista previa en pantalla
5. **Cuando no se puede guardar** - `Frame_SaveAsync` devuelve `false` y lanza `OnError` cuando la vista previa no está en marcha o todavía no ha llegado ningún fotograma. El ejemplo activa Save Frame solo después de que `StartAsync` devuelve `true`
6. **Un archivo nuevo en cada clic** - el nombre del archivo lleva la fecha y la hora hasta el milisegundo, así que un fotograma nunca sustituye al anterior
7. **Abrir el archivo** - `Process.Start` con `UseShellExecute = true` abre el JPEG en el visor de imágenes predeterminado. En un PC sin ninguna aplicación para `.jpg` lanza una `Win32Exception`, y lo que queda es la ruta en la barra de título
8. **Cerrar** - `Dispose()` detiene una vista previa en marcha, y una vista previa no tiene ningún archivo que terminar, así que el manejador de cierre solo espera mientras la vista previa se inicia o se detiene

## Fotogramas en memoria

Para trabajar con un fotograma sin archivo:

- `Frame_GetCurrent()` devuelve el fotograma actual como un `System.Drawing.Bitmap`;
- el evento `OnVideoFrameBitmap` le entrega cada fotograma en cuanto llega.

## Transcripción del video

### Qué vamos a construir

Vamos a mostrar la vista previa en directo de una webcam en una aplicación C# WinForms, y a guardar fotogramas de ella, con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms con vista previa en directo de la webcam. La cámara, su formato y su velocidad de fotogramas se eligen en listas. Un botón Save Frame escribe el fotograma actual en un archivo JPEG, y la aplicación abre ese archivo en el visor de imágenes predeterminado, para que vea lo que se guardó. Esta es la aplicación terminada, con la vista previa de una Logitech C920 a 1920 por 1080. No se graba nada. La vista previa solo muestra la imagen, y se guarda un fotograma cuando usted lo pide. ¿Dónde resulta útil? Fotomatones y fotos de identificación. Escanear un documento o una etiqueta con una webcam. Puestos de inspección que guardan una foto de cada pieza. Y cualquier aplicación que necesite una imagen fija para procesarla o para ponerla en un informe.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una webcam. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación solo funciona en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto WebcamPreview. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada dos paquetes NuGet: el propio SDK y el redist de captura de video con sus filtros DirectShow. Una vista previa no escribe ningún archivo, así que no necesita un paquete de codificador. En Visual Studio, eso es Manage NuGet Packages. Busque VisioForge.DotNet.Core e instálelo. Después instale el redist de captura de video para x64. El redist es x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa el redist x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la vista previa en directo. Añada una etiqueta y un cuadro combinado para la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo para el formato de la cámara y la velocidad de fotogramas. Añada los botones Start, Stop y Save Frame. Stop y Save Frame quedan desactivados hasta que la vista previa está en marcha. Haga doble clic en el formulario, en las listas de cámara y de formato, y en los tres botones. Visual Studio añade un manejador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos usings. System.Diagnostics abre el archivo guardado, System.ComponentModel contiene la excepción que puede lanzar, y System.Drawing.Imaging da nombre al formato de imagen. En el manejador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la vista previa en directo. Los errores del motor llegan en el evento OnError. Aquí van a la barra de título.

### Listar los formatos de la cámara

Video_CaptureDevices devuelve todas las cámaras. Listamos sus nombres y seleccionamos la primera. Cuando cambia la cámara, su manejador llena la lista de formatos con los formatos de esa cámara, y elige 1920x1080 MJPG cuando existe. Cuando cambia el formato, su manejador llena de la misma manera la lista de velocidades de fotogramas, y elige treinta fotogramas por segundo.

### Iniciar la vista previa

El manejador de Start comprueba primero que haya una velocidad de fotogramas seleccionada. Después selecciona la cámara por su nombre, desactiva Format_UseBest y pasa el formato y la velocidad de fotogramas de las listas. Si se deja activado, Format_UseBest elige por sí mismo la resolución más alta hasta 1080p. Audio_PlayAudio está activado por defecto: reproduce la fuente de audio seleccionada por los altavoces. Esta aplicación solo muestra video, así que se desactiva. Si se deja activado sin ninguna fuente de audio seleccionada, el SDK informa de un error y funciona sin sonido. El modo VideoPreview muestra la cámara sin escribir ningún archivo. StartAsync termina cuando la vista previa está en marcha, y entonces se activan Stop y Save Frame. VideoPreview pone la cámara en pantalla y no escribe ningún archivo, así que no hay codificador ni formato de salida que configurar. Los fotogramas siguen llegando a la aplicación, y eso es lo que usa Save Frame. Cambie el modo a VideoCapture, añada un archivo de salida y un formato, y la misma aplicación graba.

### Guardar un fotograma

Save Frame crea un nombre de archivo en la carpeta Pictures, con la fecha y la hora hasta el milisegundo, así que cada clic crea un archivo nuevo. Frame_SaveAsync escribe ahí el fotograma actual como JPEG, con calidad 85, y devuelve false cuando no puede. Después la ruta va a la barra de título, y Process.Start con UseShellExecute abre el archivo en el visor de imágenes predeterminado. En un PC sin ninguna aplicación para archivos JPEG, Process.Start lanza una Win32Exception, y lo que queda es la ruta en el título. Frame_SaveAsync recibe el nombre del archivo y un formato de imagen, como JPEG, PNG, BMP o TIFF. La calidad solo se aplica a JPEG, y es 85 por defecto. Indique a la vez un ancho y un alto, y el fotograma se redimensiona a ese tamaño. Sin ellos, el archivo tiene la resolución de la cámara, no el tamaño de la vista previa en pantalla. Devuelve false, y lanza OnError, cuando la vista previa no está en marcha o todavía no ha llegado ningún fotograma.

### Detener y cerrar

Stop desactiva Stop y Save Frame, y StopAsync detiene la cámara. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este manejador. Mientras la vista previa se inicia o se detiene, los dos botones están desactivados, y el manejador espera cancelando el cierre. Si no, libera el motor. Dispose detiene una vista previa en marcha, y no hay ningún archivo que terminar.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Las listas de cámara y de formato se llenan a partir de los dispositivos. Start inicia la vista previa. La C920 tarda unos segundos en enviar su primer fotograma, y luego aparece la imagen. Haga clic en Save Frame. El JPEG se abre en Photos. Su tamaño es 1920 por 1080, el formato de la cámara, aunque la vista previa del formulario es mucho más pequeña. Cierre Photos, detenga la vista previa y cierre la ventana.

### Errores frecuentes

Algunos errores frecuentes. Audio_PlayAudio activado sin ninguna fuente de audio seleccionada, de modo que OnError informa de que falta una fuente de audio. Guardar antes de Start, o antes de que la cámara envíe su primer fotograma, de modo que Frame_SaveAsync devuelve false. Un mismo nombre de archivo para cada clic, de modo que cada fotograma sustituye al anterior. Y un valor de calidad con PNG o BMP, donde se ignora.

### Próximos pasos

A continuación, el video de webcam a MP4 graba lo que usted ve en la vista previa, y el video de texto sobre el video dibuja encima un rótulo, un reloj y números de fotograma. Para trabajar con un fotograma en memoria en lugar de un archivo, Frame_GetCurrent lo devuelve como un Bitmap. Y el evento OnVideoFrameBitmap le entrega cada fotograma en cuanto llega. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Webcam a MP4](video-capture-webcam-mp4.md) — grabar la misma cámara en un archivo MP4
- [Texto sobre webcam](webcam-capture-text-overlay.md) — dibujar un rótulo, un reloj y números de fotograma sobre el video
- [Grabación previa al evento](../guides/pre-event-recording.md) — guardar video de la cámara en un búfer y grabar clips al detectar movimiento o con un disparador manual

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código y de implementación.
