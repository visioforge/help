---
title: "Vista previa en directo de cámara IP RTSP en C# WinForms"
description: "Muestre en directo una cámara IP por RTSP en C# WinForms con VideoCaptureCore, sin credenciales en la URL y con la imagen pixelada."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IPCameraSourceSettings
  - IPSourceEngine
  - VideoCaptureMode
  - VideoEffectMosaic
  - IVideoView
video:
  id: dxVWXHwgFQg
  upload_date: 2026-11-16T20:53:27Z
  duration: PT9M47S

---

# Vista previa de cámara IP por RTSP en C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial muestra en directo una cámara IP por RTSP en una aplicación C# WinForms con el motor VideoCaptureCore:

- la URL RTSP de la cámara, su usuario y su contraseña se escriben en el formulario, y el campo de la contraseña muestra puntos;
- el usuario y la contraseña van en sus propias propiedades de `IPCameraSourceSettings`, nunca en la URL;
- el motor LAV (`IPSourceEngine.Auto_LAV`) lee el flujo;
- el modo `IPPreview` muestra la cámara sin escribir ningún archivo;
- una casilla Pixelate aplica un `VideoEffectMosaic` a toda la imagen.

Así funcionan un monitor de seguridad para sus propias cámaras, la comprobación de lo que ve una cámara mientras se instala, la ocultación de caras o pantallas antes de mostrar el video, y los quioscos o pantallas de recepción. La cámara del video es una Dahua de la red local, pixelada a propósito con el efecto de la propia aplicación.

Este tutorial usa el motor VideoCaptureCore, exclusivo de Windows: el motor DirectShow del SDK.

## Video tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/dxVWXHwgFQg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Repositorio de código fuente

El proyecto completo, con el archivo del diseñador, está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/ip-camera-preview-winforms)

## Paquetes necesarios

Instale el paquete [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/), el redist de captura de video con sus filtros DirectShow y el redist de LAV, el decodificador que lee el flujo RTSP. Una vista previa no escribe ningún archivo, así que no necesita un paquete de codificador.

Establezca `PlatformTarget` en el archivo del proyecto según los redists (`x64` o `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.LAV.x64
```

Para una aplicación de 32 bits (x86), use en su lugar los paquetes x86: `VisioForge.DotNet.Core.Redist.VideoCapture.x86` y `VisioForge.DotNet.Core.Redist.LAV.x86`.

`VisioForge.DotNet.VideoCapture`, el paquete del bloque Try it al principio de esta página, también sirve: instala el mismo `VisioForge.DotNet.Core`. Los paquetes redist son necesarios en ambos casos.

## Ejemplo de implementación con código C\#

El formulario tiene un `VideoView`, cuadros de texto para la URL, el usuario y la contraseña (con `UseSystemPasswordChar` activado), una casilla Pixelate marcada por defecto, y los botones Start y Stop; Stop empieza deshabilitado. El archivo del diseñador está en el código fuente anterior. El proyecto usa .NET 10 con usings implícitos. En .NET Framework 4.7.2, el mismo código se compila en un proyecto de estilo SDK con `<LangVersion>10</LangVersion>` y `<ImplicitUsings>enable</ImplicitUsings>`; en `Program.cs`, sustituya `ApplicationConfiguration.Initialize()`, que .NET Framework no genera, por `Application.EnableVisualStyles()` y `Application.SetCompatibleTextRenderingDefault(false)`, y ponga `<dpiAware>true</dpiAware>` en un `app.manifest` para conservar la compatibilidad con el DPI del sistema que activa .NET 10.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace IpCameraPreview;

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
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        // La URL queda sin credenciales: el usuario y la contraseña van aparte
        videoCapture1.IP_Camera_Source = new IPCameraSourceSettings
        {
            URL = url,
            Login = edLogin.Text,
            Password = edPassword.Text,
            Type = IPSourceEngine.Auto_LAV
        };

        // Solo video: la reproducción de audio está activada por defecto
        videoCapture1.Audio_PlayAudio = false;

        // Los efectos se configuran antes del inicio
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Enabled = cbPixelate.Checked;
        if (cbPixelate.Checked)
            videoCapture1.Video_Effects_Add(new VideoEffectMosaic(true, 40));

        videoCapture1.Mode = VideoCaptureMode.IPPreview;

        // Una cámara que no responde tarda hasta ~20 s en fallar
        Text = "Connecting...";
        btStart.Enabled = false;
        cbPixelate.Enabled = false;
        var started = await videoCapture1.StartAsync();

        // Un error del SDK publicado durante el inicio se queda en el título
        if (Text == "Connecting...") Text = started ? "IP camera preview" : "The preview did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbPixelate.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
        cbPixelate.Enabled = true;
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

1. **Credenciales fuera de la URL** - `Login` y `Password` son propiedades de `IPCameraSourceSettings`, así que la URL nunca las lleva y no aparecen allí donde se muestra o se registra la dirección. El cuadro de texto de la contraseña activa `UseSystemPasswordChar`, así que muestra puntos
2. **El motor** - `IPSourceEngine.Auto_LAV` lee flujos RTSP y HTTP con LAV Filters y puede decodificar en la GPU. Sin el redist de LAV, el inicio falla con "Unable to create LAV Splitter Source filter"
3. **La dirección de la cámara** - `rtsp://`, la dirección IP y el puerto de la cámara (normalmente 554), y luego una ruta que cambia según la marca. Muchas cámaras envían un flujo principal a resolución completa y un subflujo más pequeño; el manual de la cámara o su página web indican la ruta exacta, y el [directorio de marcas de cámaras IP](../../camera-brands/index.md) recoge muchas de ellas
4. **Modo de vista previa** - `Mode = VideoCaptureMode.IPPreview` muestra la cámara y no escribe ningún archivo. Añada el redist de MP4 ([x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/), [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)), cambie a `IPCapture`, indique un archivo de salida y un formato, y la misma aplicación graba; [Grabar un flujo RTSP en MP4](ip-camera-capture-mp4.md) muestra la misma grabación con el motor multiplataforma `VideoCaptureCoreX`
5. **Audio** - `Audio_PlayAudio` es `true` por defecto; el ejemplo solo muestra video, así que lo desactiva
6. **Pixelar** - `VideoEffectMosaic(enabled, value)` recibe el tamaño del bloque en píxeles, de 1 a 255: de 2 a 5 es un efecto ligero, y desde 16 no queda nada legible. Cubre todo el fotograma, tanto en la vista previa como en una grabación. Los efectos se configuran antes de `StartAsync`, así que la casilla queda bloqueada mientras la vista previa está en marcha, y `Video_Effects_Clear` quita el efecto de la ejecución anterior
7. **El inicio** - una cámara que no responde tarda hasta unos 20 segundos en fallar, así que el título dice "Connecting..." y Start queda deshabilitado hasta que `StartAsync` vuelve. Un mensaje de `OnError` publicado durante el inicio se queda en el título
8. **Cerrar** - `Dispose()` detiene una vista previa en marcha, y una vista previa no tiene ningún archivo que terminar, así que el manejador de cierre solo espera mientras la vista previa se inicia o se detiene

## Solución de problemas

- **La cámara se conecta pero la imagen sigue en negro** - el motor LAV recibe el video RTSP por UDP, y un firewall, un NAT o una VPN que bloquea UDP lo detiene; una máquina virtual con red NAT hace lo mismo. Permita el UDP de la cámara, o use `IPSourceEngine.RTSP_LowLatency`, que lee RTSP por TCP de forma predeterminada (`RTSP_LowLatency_UseUDP` es `false`)
- **La cámara rechaza el flujo** - la ruta no corresponde a la marca de la cámara, o el usuario o la contraseña son incorrectos
- **Varias cámaras** - use un `VideoCaptureCore` y un `VideoView` para cada una
- **Encontrar cámaras** - `IP_Camera_ONVIF_ListSourcesAsync` descubre las cámaras ONVIF de la red

## Transcripción del video

### Qué vamos a construir

Vamos a mostrar en directo una cámara IP por RTSP en una aplicación C# WinForms, con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WinForms que muestra en directo una cámara IP. Usted escribe la dirección RTSP de la cámara, su usuario y su contraseña, y el campo de la contraseña muestra puntos. Una casilla Pixelate aplica un efecto de mosaico a la imagen. Esta es la aplicación terminada, con la vista previa de una cámara Dahua de la red local. La imagen está pixelada a propósito, con el efecto de la propia aplicación, para que esta cámara real no muestre nada privado. No se graba nada. La vista previa solo muestra la imagen. ¿Dónde resulta útil? Un monitor de seguridad para sus propias cámaras. Comprobar lo que ve una cámara mientras la instala. Ocultar caras o pantallas antes de mostrar el video a nadie. Y quioscos o pantallas de recepción.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto puede usar .NET 8 o posterior, o .NET Framework 4.7.2. Y una cámara IP que emita por RTSP, con su dirección, usuario y contraseña. VideoCaptureCore es el motor DirectShow del SDK, así que esta aplicación funciona solo en Windows.

### Crear un proyecto WinForms

En Visual Studio, cree un proyecto nuevo. Busque Windows Forms y elija Windows Forms App para C#. Llame al proyecto IpCameraPreview. Elija .NET 10 y haga clic en Create. Visual Studio abre el formulario vacío en el diseñador.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. El propio SDK. El redist de captura de video con sus filtros DirectShow. Y el redist de LAV, el decodificador que lee el flujo RTSP. Una vista previa no escribe ningún archivo, así que no necesita ningún paquete de codificador. En Visual Studio, eso es Manage NuGet Packages. Busque VisioForge.DotNet.Core en Browse e instálelo. Después instale el redist de captura de video para x64. Y el redist de LAV para x64. Sin él, el inicio falla, porque no hay nada que lea el flujo. Los redists son x64, así que ponga PlatformTarget en x64 en el archivo del proyecto. Una aplicación x86 usa en su lugar los redists x86.

### Diseñar el formulario WinForms

Desde el Toolbox, dibuje un VideoView para la imagen en directo. Añada una etiqueta y un cuadro de texto para la URL de la cámara, y dé a cada control un nombre claro en la ventana Properties. Haga lo mismo con el usuario y la contraseña. En el cuadro de la contraseña, ponga UseSystemPasswordChar en True, para que muestre puntos. Añada una casilla Pixelate, marcada por defecto, y los botones Start y Stop. Stop queda desactivado hasta que la vista previa esté en marcha. Haga doble clic en el formulario y en los dos botones. Visual Studio añade un controlador vacío para cada uno.

### Crear el motor de captura

Al principio de Form1.cs, añada estos using. VideoEffects contiene el efecto de mosaico. En el controlador Load del formulario, cree el motor VideoCaptureCore y pásele el VideoView, donde se dibuja la imagen. Los errores del motor llegan en el evento OnError. Aquí van a la barra de título.

### Conectar con la cámara

El controlador de Start comprueba primero que la URL sea una dirección válida. Después IP_Camera_Source recibe un objeto IPCameraSourceSettings. La URL no lleva ni usuario ni contraseña. Van en sus propias propiedades, así que nunca aparecen en la dirección. Type Auto_LAV elige el motor LAV, que lee flujos RTSP y HTTP y puede decodificar en la GPU. La dirección empieza por rtsp, luego la dirección IP de la cámara y el puerto, normalmente 554. Después viene una ruta, y cada marca tiene la suya. Muchas cámaras envían un flujo principal a resolución completa y un subflujo más pequeño. El manual de la cámara o su página web indican la ruta exacta.

### Pixelar la imagen

Audio_PlayAudio está activado por defecto, y esta aplicación muestra solo video, así que se desactiva. Los efectos se configuran antes del inicio. Video_Effects_Clear quita los efectos de la ejecución anterior, y Video_Effects_Enabled sigue a la casilla. Si está marcada, Video_Effects_Add añade un VideoEffectMosaic con bloques de 40 píxeles. VideoEffectMosaic recibe un tamaño de bloque en píxeles, de 1 a 255. De dos a cinco es un efecto ligero, y desde 16 no se puede leer nada. Cubre todo el fotograma, tanto en la vista previa como en una grabación. El efecto se configura antes de StartAsync, así que la casilla queda bloqueada mientras la vista previa está en marcha.

### Iniciar la vista previa

El modo IPPreview muestra la cámara sin escribir ningún archivo. Una cámara que no responde puede tardar unos 20 segundos en fallar, así que el título dice Connecting, y Start y la casilla se desactivan. StartAsync vuelve cuando la vista previa ya está en marcha. Un error del SDK se queda en el título, y Stop se activa si el inicio funcionó.

### Detener y cerrar

Stop se desactiva a sí mismo, y StopAsync detiene el flujo. Después Start y la casilla vuelven a activarse. En la ventana Properties, en la pestaña Events, haga doble clic en FormClosing para añadir este controlador. Mientras la vista previa se inicia o se detiene, los dos botones están desactivados, y el controlador espera cancelando el cierre. Si no, libera el motor. Dispose detiene una vista previa en marcha, y no hay ningún archivo que terminar.

### Ejecutarla

Ejecute la aplicación desde Visual Studio y escriba la dirección RTSP de la cámara. Después el usuario y la contraseña, que se muestra como puntos. Haga clic en Start. La cámara responde en unos tres segundos, y aparece la imagen en directo, pixelada. Detenga la vista previa y cierre la ventana.

### Errores frecuentes

Algunos errores frecuentes. Olvidar el redist de LAV, y el inicio falla. Poner el usuario y la contraseña en la URL, donde aparecen allí donde aparezca la dirección. Una ruta equivocada para la marca de la cámara, y la cámara rechaza el flujo. Y una red que bloquea UDP, como un firewall, un NAT o una VPN. La cámara se conecta, pero la imagen sigue en negro. El motor RTSP_LowLatency lee RTSP por TCP de forma predeterminada.

### Próximos pasos

A continuación, añada el redist de MP4, cambie el modo a IPCapture, indique un archivo de salida, y la misma aplicación graba la cámara en MP4. También se puede dibujar texto sobre el video. Para varias cámaras, use un VideoCaptureCore y un VideoView para cada una. Y ONVIF encuentra por usted las cámaras de la red. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Configuración de la fuente de cámara RTSP](../video-sources/ip-cameras/rtsp.md) — transporte UDP/TCP, ajuste de baja latencia, elección del motor
- [Integración de cámaras IP ONVIF](../video-sources/ip-cameras/onvif.md) — descubrimiento automático, selección de perfil, control PTZ
- [Grabar un flujo RTSP en MP4](ip-camera-capture-mp4.md) — capturar en un archivo el flujo de la vista previa

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para explorar más ejemplos de código. ¿Necesita la URL RTSP de su cámara? Consulte nuestro [directorio de marcas de cámaras IP](../../camera-brands/index.md).
