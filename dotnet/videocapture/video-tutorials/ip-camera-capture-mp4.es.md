---
title: Grabar una cámara IP RTSP en MP4 en C# .NET — Ejemplo
description: "Grabe una cámara IP RTSP en MP4 en C# WinForms con VideoCaptureCoreX: URL, usuario y contraseña, vista previa en directo, audio de la cámara, H.264 en GPU."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - MP4Output
  - VisioForgeX
video:
  id: 6CTrBCzHySE
  upload_date: 2026-11-17T08:57:31Z
  duration: PT9M52S

---

# Grabar un flujo RTSP de una cámara IP en MP4 con C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial graba una cámara IP en un archivo MP4 desde una aplicación C# WinForms con el motor multiplataforma `VideoCaptureCoreX`:

- la URL RTSP de la cámara, su usuario y su contraseña se escriben en el formulario, y el campo de la contraseña muestra puntos;
- `RTSPSourceSettings.CreateAsync` se conecta a la cámara y lee lo que lleva el flujo, de modo que una cámara sin micrófono se graba solo con video;
- un `VideoView` muestra la vista previa en directo mientras se escribe el archivo;
- `MP4Output` codifica H.264 en la GPU cuando el equipo tiene un codificador por hardware, y OpenH264 en la CPU si no lo tiene, con audio AAC;
- cada grabación recibe su propio nombre de archivo, con la fecha y la hora, en la carpeta Vídeos.

El motor clásico `VideoCaptureCore` también graba cámaras IP: el [tutorial de vista previa de cámara IP](ip-camera-preview.md) construye su aplicación, y su modo `IPCapture` escribe el archivo.

## Tutorial en video

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/6CTrBCzHySE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### La versión corta

La misma grabación como aplicación de consola en unas pocas líneas de C#, sin la vista previa ni el sonido. El usuario y la contraseña están escritos en su código: sustituya `admin` y `password` por los de su cámara.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/CUrQcoK86nQ?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Su código: [ip-camera-mp4-short-console en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-mp4-short-console).

## Repositorio del código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-capture-mp4-winforms)

## Paquetes necesarios

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — el SDK; el `VideoView` de WinForms viene en él
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — el runtime de GStreamer sobre el que funciona el motor X, con la fuente RTSP y los codificadores H.264
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — el codificador de audio AAC

Son los paquetes x64, por eso el proyecto indica `<PlatformTarget>x64</PlatformTarget>`; una aplicación de 32 bits usa los paquetes `.x86`. La lista completa está en [Implementación en Windows](../../deployment-x/Windows.md).

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Ejemplo de implementación en C\#

El formulario tiene un `VideoView` llamado `VideoView1`, los cuadros de texto `edURL`, `edLogin` y `edPassword` (con `UseSystemPasswordChar`), una casilla **Record audio** `cbRecordAudio` y los botones `btStart` y `btStop`, ambos desactivados en el diseñador. Los eventos `Load` y `FormClosing` del formulario están conectados a los controladores siguientes.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraCaptureMp4;

public partial class Form1 : Form
{
    private VideoCaptureCoreX videoCapture1 = null!;

    private string outputFile = string.Empty;

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        btStart.Enabled = true;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        Text = "Connecting...";
        btStart.Enabled = false;
        cbRecordAudio.Enabled = false;

        var started = false;
        try
        {
            // Reads the stream info first: audio is recorded only when the info shows an audio track
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Text, cbRecordAudio.Checked);

            videoCapture1.Audio_Record = cbRecordAudio.Checked;
            videoCapture1.Audio_Play = false; // record the camera's sound, don't play it

            // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"ipcam_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            videoCapture1.Outputs_Clear();
            videoCapture1.Outputs_Add(new MP4Output(outputFile));

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Text = ex.Message;
        }

        // An SDK error posted during the start stays in the title
        if (Text == "Connecting...") Text = started ? "Recording..." : "The recording did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbRecordAudio.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync(); // finishes the MP4 file
        Text = "Saved: " + outputFile;
        btStart.Enabled = true;
        cbRecordAudio.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // loading, starting or stopping: wait for it
            return;
        }

        if (btStop.Enabled)
        {
            e.Cancel = true; // recording: finish the MP4 first
            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close);
            return;
        }

        videoCapture1.Dispose();
        VisioForgeX.DestroySDK();
    }
}
```

## Características clave de la implementación

1. **Inicialización del SDK** - `VisioForgeX.InitSDKAsync` se ejecuta una vez, al cargar el formulario. La primera ejecución en un equipo tarda más, por eso Start se activa solo cuando termina. `VisioForgeX.DestroySDK` lo libera al cerrar el formulario
2. **La dirección de la cámara** - `rtsp://`, la dirección IP y el puerto de la cámara (normalmente 554) y luego una ruta que depende de la marca. El usuario y la contraseña son argumentos aparte de `CreateAsync`, así que la URL no los lleva. Una dirección `http://` se toma como el servicio ONVIF de la cámara, y de él se lee la URL RTSP de su mejor perfil. El [directorio de marcas de cámaras IP](../../camera-brands/index.md) recoge las rutas de muchas cámaras
3. **Audio** - con **Record audio** marcada, `CreateAsync` pide el audio de la cámara y `Audio_Record` lo incluye en el archivo. El audio se graba solo cuando la información del flujo muestra una pista de audio, así que una cámara sin micrófono da un archivo solo con video. `Audio_Play` es `true` por defecto; la aplicación lo pone en `false`, de modo que el sonido se graba pero no se reproduce
4. **Salida MP4** - `MP4Output` sin ajustes de codificador elige NVIDIA NVENC, luego AMD AMF, luego Intel Quick Sync, y OpenH264 en la CPU cuando no hay ninguno de ellos. El audio es AAC. Una salida queda en el motor tras una grabación, por eso `Outputs_Clear` se ejecuta antes de cada nueva
5. **Inicio** - una cámara que no responde tarda en fallar, por eso el título dice "Connecting..." y Start queda desactivado hasta que `StartAsync` devuelve. Un mensaje de `OnError` publicado durante el inicio se queda en el título
6. **Detener y cerrar** - `StopAsync` finaliza el archivo MP4. Cerrar el formulario durante una grabación cancela el cierre, espera a `StopAsync` y vuelve a cerrar; mientras el SDK se carga, o mientras un inicio o una parada están en curso, el cierre espera

## Solución de problemas

- **La cámara rechaza el flujo** - la ruta no corresponde a la marca de la cámara, o el usuario o la contraseña son incorrectos. El error se muestra en el título de la ventana
- **El archivo no tiene sonido** - la cámara no envía pista de audio, o el audio está desactivado en la configuración de la propia cámara
- **Elegir el codificador** - para quedarse en la CPU, pase `new OpenH264EncoderSettings()` (espacio de nombres `VisioForge.Core.Types.X.VideoEncoders`) al constructor de `MP4Output`; `H264EncoderBlock.CanEncodeAsync` (espacio de nombres `VisioForge.Core.MediaBlocks.VideoEncoders`) comprueba si un codificador de GPU puede codificar en este momento
- **Varias cámaras** - use un `VideoCaptureCoreX` y un `VideoView` para cada una

## Transcripción del video

### Qué vamos a construir

Vamos a grabar una cámara IP en un archivo MP4 en una aplicación C# WinForms, con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms que graba una cámara IP en un archivo MP4. Usted escribe la dirección RTSP de la cámara, su usuario y su contraseña. La imagen se ve en directo mientras la aplicación graba. El video es H.264, y el sonido de la cámara se graba en AAC. Esta es la aplicación terminada, grabando. Aquí la transmisión viene de un servidor RTSP de prueba en la red local, que reproduce la película libre Big Buck Bunny, así que no necesita usuario. Una cámara real funciona igual, con su usuario y su contraseña. ¿Dónde resulta útil? Un grabador de video sencillo para sus propias cámaras. Guardar un suceso en un archivo como prueba. Grabar una línea de producción o una prueba de laboratorio. Y clips que se revisan o se comparten más tarde.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto usa .NET 8 o posterior. Y una cámara IP que transmita por RTSP, con su dirección. El motor aquí es VideoCaptureCoreX, el motor multiplataforma del SDK, basado en GStreamer. El mismo código funciona en macOS y Linux, con su propia interfaz.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto IpCameraCaptureMp4. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. El propio Video Capture SDK. El paquete core multiplataforma, con el runtime de GStreamer para Windows x64. Y el paquete Libav, que aporta el codificador de audio AAC. En Visual Studio, eso es Manage NuGet Packages. Busque VisioForge.DotNet.VideoCapture e instálelo. Después instale el paquete core multiplataforma para Windows x64. Y el paquete Libav para Windows x64. Los paquetes del runtime son x64, así que ponga PlatformTarget en x64 en el archivo del proyecto.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la imagen en directo. Añada una etiqueta y un cuadro de texto para la URL de la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo con el usuario y la contraseña. En el cuadro de la contraseña, ponga UseSystemPasswordChar en True, para que muestre puntos. Añada una casilla Record audio, marcada por defecto, y los botones Start y Stop. Los dos botones empiezan desactivados. Start se activa cuando el SDK se ha cargado, y Stop cuando la grabación está en marcha. Haga doble clic en el formulario y en los dos botones. Visual Studio añade un controlador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. Los espacios de nombres X contienen el motor multiplataforma, sus fuentes y sus salidas. En el controlador Load del formulario, InitSDKAsync carga el SDK y su runtime de GStreamer, una vez por aplicación. Después cree el motor VideoCaptureCoreX y pásele el VideoView. Los errores del motor llegan en el evento OnError, y aquí van a la barra de título. Por último, se activa Start. Un campo guarda el nombre del archivo de salida, para mostrarlo después de detener.

### Conectar con la cámara

El controlador de Start comprueba primero que la URL sea una dirección válida. Conectar puede tardar unos segundos, así que el título dice Connecting, y Start y la casilla se desactivan. RTSPSourceSettings.CreateAsync recibe la URL, el usuario, la contraseña y si se usa el audio. La URL no lleva usuario ni contraseña, así que nunca aparecen en la dirección. CreateAsync lee primero la información de la transmisión. Cuando la cámara no envía sonido, el audio se omite y la grabación es solo de video. Audio_Record sigue a la casilla. Audio_Play está activado por defecto, y esta aplicación graba el sonido sin reproducirlo, así que se desactiva. La dirección empieza por rtsp, luego la dirección IP de la cámara y el puerto, normalmente 554. Después viene una ruta, y cada marca tiene la suya. Muchas cámaras envían un flujo principal a resolución completa y un subflujo más pequeño. El manual de la cámara o su página web indican la ruta exacta.

### Grabar en MP4

El archivo va a la carpeta Vídeos, con la fecha y la hora en el nombre. Outputs_Clear quita la salida de la ejecución anterior, y Outputs_Add añade un MP4Output. Sin codificadores indicados, MP4Output elige H.264 en el codificador de la GPU cuando lo hay, NVIDIA, AMD o Intel, y OpenH264 en caso contrario. El sonido es AAC. StartAsync conecta y vuelve cuando la grabación está en marcha. Una excepción, como una cámara que rechaza la conexión, va al título. Un error del SDK durante el inicio también se queda en el título. Si no, el título dice Recording. Stop se activa si el inicio funcionó, y Start si no.

### Detener y cerrar

Stop se desactiva a sí mismo, y StopAsync detiene la transmisión y termina el archivo MP4. Después el título muestra dónde se guardó el archivo, y Start y la casilla vuelven a activarse. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este controlador. Mientras la aplicación carga, inicia o se detiene, los dos botones están desactivados, y el controlador cancela el cierre: cierre la ventana de nuevo cuando termine. Mientras graba, el cierre también se cancela. StopAsync termina el archivo, y después el formulario se cierra solo. Solo entonces se liberan el motor y el SDK. Un archivo sin terminar no se puede reproducir.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Start se activa cuando el SDK se ha cargado. Escriba la dirección RTSP de la transmisión. Esta transmisión de prueba no necesita usuario. Haga clic en Start. La transmisión conecta en unos segundos, aparece la imagen y el título dice Recording. Haga clic en Stop. El archivo queda terminado, y el título muestra dónde se guardó, en la carpeta Vídeos. Y cierre la ventana. Este archivo tiene video H.264 de 1280 por 720 y sonido AAC en estéreo.

### Errores frecuentes

Algunos errores frecuentes. Omitir el paquete Libav, de modo que no hay codificador AAC para el sonido. Crear el motor antes de que InitSDKAsync haya cargado el SDK. Poner el usuario y la contraseña en la URL, donde aparecen allí donde aparezca la dirección. Y cerrar la aplicación sin StopAsync, lo que deja un archivo MP4 que no se puede reproducir.

### Próximos pasos

A continuación, pase sus propios ajustes de codificador a MP4Output, para la tasa de bits o el códec que prefiera. Divida una grabación larga en archivos por tiempo. Para varias cámaras, use un VideoCaptureCoreX y un VideoView para cada una. Y ONVIF encuentra por usted las cámaras de la red. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Guías relacionadas

- [Tutorial de vista previa de cámara IP](ip-camera-preview.md) — el mismo formulario con el motor clásico `VideoCaptureCore`
- [Configuración de la fuente RTSP](../video-sources/ip-cameras/rtsp.md) — transporte UDP/TCP, baja latencia, elección del motor
- [Integración de cámaras IP ONVIF](../video-sources/ip-cameras/onvif.md) — detección automática y elección de perfil
- [Guardar el flujo RTSP original sin recodificar](../../mediablocks/Guides/rtsp-save-original-stream.md) — alternativa con Media Blocks que conserva el bitrate de origen

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código. ¿Necesita la URL RTSP de su cámara? Consulte nuestro [directorio de marcas de cámaras IP](../../camera-brands/index.md).
