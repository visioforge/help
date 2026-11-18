---
title: Transmisión de Video en Red con RTSP, RTMP, NDI en .NET
description: Implemente transmisión en red en .NET con protocolos RTSP, RTMP, NDI, HLS y SRT utilizando consejos de implementación y mejores prácticas.
sidebar_label: Transmisión en Red
order: 5
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
video:
  id: _mkmAxqQesw
  upload_date: 2026-11-17T11:35:00Z
  duration: PT10M8S

---

# Transmisión en Red en Aplicaciones .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introducción a la Transmisión en Red

La transmisión en red se ha convertido en un componente fundamental de la infraestructura de comunicación digital moderna, permitiendo la transmisión en tiempo real de datos de audio y video a través de diversos entornos de red. A medida que las capacidades de ancho de banda continúan expandiéndose y las expectativas de los usuarios para la entrega de contenido evolucionan, los desarrolladores necesitan herramientas robustas para implementar soluciones de transmisión que equilibren el rendimiento, la calidad y la confiabilidad.

El VisioForge Video Capture SDK para .NET proporciona un soporte integral para múltiples protocolos de transmisión, ofreciendo a los desarrolladores un conjunto de herramientas versátil para integrar capacidades de transmisión sofisticadas en sus aplicaciones. Ya sea que esté construyendo plataformas de transmisión en vivo, herramientas de videoconferencia, sistemas de vigilancia o redes de entrega de contenido, este SDK proporciona la base para implementar soluciones de transmisión de grado profesional.

## Tutorial en vídeo — transmitir una webcam por RTMP en C# WPF

Este tutorial crea una aplicación WPF sobre el motor `VideoCaptureCoreX` que muestra la vista previa en directo de una webcam y, al mismo tiempo, transmite la cámara y el micrófono por RTMP:

- el vídeo es H.264, codificado en la GPU cuando el PC tiene un codificador por hardware (NVIDIA NVENC, AMD AMF, Intel Quick Sync) y con OpenH264 en la CPU en caso contrario; el audio es AAC;
- la transmisión va a cualquier servidor RTMP: MediaMTX, YouTube Live o el punto de ingesta de una CDN;
- en el vídeo va a un servidor MediaMTX de la red local, y un navegador la reproduce desde ese servidor.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/_mkmAxqQesw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### Código fuente

El proyecto completo está en GitHub: [network-streaming-rtmp-wpf](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/network-streaming-rtmp-wpf).

### Paquetes necesarios

El SDK, CrossPlatform Core (el runtime de GStreamer sobre el que funciona el motor X, con el sink RTMP y los codificadores H.264) y CrossPlatform Libav (el codificador AAC):

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

Son los paquetes x64, así que establezca `PlatformTarget` en `x64` en el archivo de proyecto. Una aplicación de 32 bits usa los paquetes x86.

### La ventana en XAML

Una cuadrícula de dos columnas: a la izquierda las listas de cámaras y micrófonos, la URL RTMP y los botones Start y Stop; a la derecha el `VideoView` que dibuja la vista previa. Los dos botones empiezan deshabilitados: Start hasta que el SDK se ha cargado, Stop hasta que hay una transmisión en curso.

```xml
<Window x:Class="WebcamRtmpWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to RTMP" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" />
            <Label Content="Microphone" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="RTMP URL" Margin="0,10,0,0" />
            <TextBox x:Name="edURL" Text="rtmp://localhost:1935/webcam" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" IsEnabled="False"
                        Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

### El código

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace WebcamRtmpWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private VideoCaptureDeviceInfo[] cameras;

    private AudioCaptureDeviceInfo[] microphones;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        foreach (var device in cameras)
            cbCamera.Items.Add(device.DisplayName);

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (cbCamera.SelectedIndex < 0)
        {
            Title = "No camera";
            return;
        }

        if (string.IsNullOrWhiteSpace(edURL.Text))
        {
            Title = "Enter the RTMP URL";
            return;
        }

        Title = "Webcam to RTMP";

        // 1080p at 25 fps or more when the camera has it, otherwise 720p, otherwise another format
        videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[cbCamera.SelectedIndex]);

        var hasMic = cbMicrophone.SelectedIndex >= 0;
        videoCapture1.Audio_Source = hasMic ? microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC() : null;
        videoCapture1.Audio_Record = hasMic; // no microphone: stream video only
        videoCapture1.Audio_Play = false; // stream the microphone without playing it back

        // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new RTMPOutput(edURL.Text.Trim()));

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
        if (btStart.IsEnabled)
        {
            videoCapture1.Dispose();
            VisioForgeX.DestroySDK();
            return;
        }

        e.Cancel = true; // still loading or streaming: stop the stream first
        if (!btStop.IsEnabled) return; // already loading, starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

- `VideoCaptureDeviceSourceSettings` toma 1080p a 25 fotogramas por segundo o más cuando la cámara lo admite; si no, 720p; si no, otro formato.
- Una URL vacía se detiene en el controlador Start: el sink RTMP usaría su propio servidor predeterminado en lugar de fallar.
- Una salida permanece en el motor después de una transmisión, así que `Outputs_Clear` se ejecuta antes de cada una nueva.
- Las propiedades `Video` y `Audio` de `RTMPOutput` admiten otros ajustes de codificador, por ejemplo una tasa de bits fija.
- Los errores del motor llegan en el evento `OnError`; la aplicación los muestra en la barra de título, y ahí aparece un servidor al que no se puede llegar.

### La URL RTMP

- **MediaMTX**: `rtmp://<servidor>:1935/<nombre>`. La ruta después del puerto es el nombre de la transmisión, y la misma ruta se reproduce por WebRTC (`http://<servidor>:8889/<nombre>`, el reproductor del vídeo), HLS y RTSP.
- **YouTube Live**: la dirección de ingesta seguida de su clave de transmisión.
- **rtmps://** se conecta por TLS.

Otros destinos usan otras salidas, y el resto del código no cambia: `SRTOutput` recibe una URL como `RTMPOutput`, `YouTubeOutput` y `FacebookLiveOutput` reciben la clave de transmisión, y `RTSPServerOutput` y `HLSOutput` reciben un objeto de ajustes (`RTSPServerSettings`, `HLSSinkSettings`).

## Inicio rápido — streaming RTMP con VideoCaptureCoreX

La misma transmisión sin ventana: sin un `VideoView` no hay vista previa, así que el motor se crea sin él. Las llamadas al motor son las mismas en todas las plataformas donde funciona el motor X; los paquetes cambian según la plataforma, y en Android e iOS la aplicación pide permiso para la cámara y el micrófono antes de enumerar los dispositivos.

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Inicialice el SDK una vez por proceso.
await VisioForgeX.InitSDKAsync();

// Sin VideoView: sin vista previa. Los errores llegan en OnError.
var videoCapture = new VideoCaptureCoreX();
videoCapture.OnError += (_, e) => Console.WriteLine(e.Message);

// La primera cámara y el primer micrófono; solo vídeo cuando no hay micrófono.
var camera = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault();
var microphone = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault();
if (camera == null) return;

videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(camera);
videoCapture.Audio_Source = microphone?.CreateSourceSettingsVC();
videoCapture.Audio_Record = microphone != null;
videoCapture.Audio_Play = false; // no reproducir el micrófono

// La URL de ingesta completa, con la clave de transmisión.
videoCapture.Outputs_Add(new RTMPOutput("rtmp://live.example.com/app/<stream-key>"));
await videoCapture.StartAsync();

// ... y cuando deba terminar la transmisión:
await videoCapture.StopAsync();
videoCapture.Dispose();
VisioForgeX.DestroySDK();
```

Para la configuración de cada protocolo (ajustes del codificador, búferes, autenticación, tasas de bits adaptativas, etc.), consulte las páginas dedicadas en [Protocolos Disponibles](#protocolos-disponibles) más abajo.

## Protocolos Principales de Transmisión en Red

### RTSP (Protocolo de Transmisión en Tiempo Real)

RTSP es un protocolo de nivel de aplicación diseñado para controlar la entrega de datos con propiedades de tiempo real. Sirve como un "control remoto de red" para servidores multimedia, estableciendo y controlando sesiones de medios entre puntos finales.

#### Características Clave de RTSP:

- **Control de Sesión**: Permite a los clientes establecer y gestionar sesiones de medios con servidores
- **Independencia de Transporte**: Funciona con varios protocolos de transporte, incluidos UDP, TCP y UDP multidifusión
- **Soporte de Comandos**: Implementa comandos como PLAY, PAUSE y RECORD para un control granular de la sesión
- **Escalabilidad**: Soporta entrega unicast y multicast para una utilización eficiente del ancho de banda

### RTMP (Protocolo de Mensajería en Tiempo Real)

Originalmente desarrollado por Macromedia para transmitir contenido Flash, RTMP ha evolucionado hasta convertirse en uno de los protocolos más utilizados para la transmisión en vivo. Mantiene conexiones persistentes entre el cliente y el servidor, facilitando la transmisión de baja latencia crucial para aplicaciones interactivas.

#### Características Clave de RTMP:

- **Baja Latencia**: Típicamente ofrece latencia de sub-segundo, haciéndolo adecuado para transmisión interactiva
- **Entrega Confiable**: Utiliza TCP como su capa de transporte para una entrega confiable de paquetes
- **Protección de Contenido**: Soporta cifrado para una entrega segura de contenido
- **Soporte Generalizado**: Compatible con numerosas CDN y plataformas de transmisión

### NDI (Interfaz de Dispositivo de Red)

NDI representa un avance significativo en los flujos de trabajo de producción de video profesional, permitiendo la transmisión de video de alta calidad y baja latencia a través de redes IP estándar. Desarrollado por NewTek, NDI ha ganado una adopción generalizada en entornos de transmisión y producción.

#### Características Clave de NDI:

- **Flujo de Trabajo Basado en IP**: Aprovecha la infraestructura de red existente sin hardware especializado
- **Comunicación Bidireccional**: Soporta metadatos y datos de control junto con audio/video
- **Mecanismo de Descubrimiento**: Descubrimiento automático de dispositivos y fuentes en redes locales
- **Codificación de Alta Calidad**: Mantiene la calidad visual mientras optimiza para las condiciones de la red

### HLS (Transmisión en Vivo HTTP)

Desarrollado por Apple, HLS se ha convertido en uno de los protocolos de transmisión más ampliamente soportados. Segmenta el contenido en pequeñas descargas de archivos basadas en HTTP, permitiendo la transmisión de tasa de bits adaptativa que ajusta la calidad según el ancho de banda disponible del espectador.

#### Características Clave de HLS:

- **Tasa de Bits Adaptativa**: Ajusta dinámicamente la calidad del flujo según las condiciones de la red
- **Amplia Compatibilidad**: Soportado en la mayoría de los navegadores y dispositivos modernos
- **Entrega HTTP**: Utiliza servidores web estándar y CDN para una entrega eficiente de contenido
- **Protección de Contenido**: Soporta cifrado e integración DRM

### SRT (Transporte Seguro y Confiable)

SRT es un protocolo de código abierto optimizado para entregar video de alta calidad y baja latencia a través de redes impredecibles. Combina la confiabilidad de TCP con la velocidad de UDP mientras agrega características de seguridad.

#### Características Clave de SRT:

- **Recuperación de Pérdida de Paquetes**: Implementa mecanismos de retransmisión dinámica
- **Cifrado**: Cifrado AES incorporado para una transmisión segura
- **Monitoreo de Salud de la Red**: Evalúa continuamente la calidad de la conexión
- **Control de Latencia**: Configuraciones de latencia configurables para equilibrar la confiabilidad y el retraso

## Protocolos Adicionales Soportados

### HTTP MJPEG (Motion JPEG)

MJPEG sobre HTTP transmite una secuencia de imágenes JPEG, proporcionando una solución de transmisión simple pero efectiva. Aunque no es tan eficiente en ancho de banda como los códecs modernos, su simplicidad y compatibilidad lo hacen adecuado para ciertas aplicaciones.

### UDP (Protocolo de Datagramas de Usuario)

La transmisión UDP prioriza la velocidad sobre la confiabilidad, haciéndola ideal para aplicaciones en tiempo real donde la pérdida ocasional de paquetes es preferible a una mayor latencia. El SDK proporciona configuraciones de búfer configurables para ayudar a optimizar la transmisión UDP para condiciones de red específicas.

### WMV (Windows Media Video)

El SDK soporta la transmisión en el formato WMV de Microsoft, que sigue siendo relevante para ciertas aplicaciones centradas en Windows e integración de sistemas heredados.

### Transmisión Específica de Plataforma

El SDK también se integra con plataformas de transmisión populares, incluyendo:

- **YouTube Live**: Transmisión directa a canales de YouTube
- **Facebook Live**: Transmisión integrada de Facebook Live
- **AWS S3**: Distribución de medios basada en la nube a través de Amazon Web Services

## Consideraciones de Implementación

Al implementar la transmisión en red con el SDK, los desarrolladores deben considerar:

1. **Requisitos de Ancho de Banda**: Estime y pruebe el ancho de banda requerido para su calidad y velocidad de cuadros objetivo
2. **Resiliencia de la Red**: Implemente un manejo de errores y lógica de reconexión apropiados
3. **Calidad vs. Latencia**: Equilibre la calidad visual frente a los requisitos de latencia
4. **Compatibilidad Multiplataforma**: Seleccione protocolos basados en sus plataformas objetivo
5. **Necesidades de Seguridad**: Implemente cifrado y autenticación donde sea necesario

## Conclusión

El VisioForge Video Capture SDK para .NET proporciona un soporte integral para protocolos de transmisión contemporáneos, empoderando a los desarrolladores para implementar soluciones de transmisión sofisticadas sin gestionar la complejidad de las implementaciones de protocolos directamente. Desde transmisiones en vivo de baja latencia hasta entrega segura de contenido, las capacidades de transmisión del SDK abordan diversos casos de uso en todas las industrias.

Al aprovechar estas capacidades, los desarrolladores pueden centrarse en construir experiencias atractivas mientras confían en el SDK para manejar los desafíos técnicos de la transmisión en red confiable. Ya sea que esté desarrollando aplicaciones para entretenimiento, educación, seguridad o producción de video profesional, el SDK proporciona una base sólida para sus necesidades de transmisión.

## Protocolos Disponibles

* [Adobe Flash Server](../../general/network-streaming/adobe-flash.md)
* [AWS S3](../../general/network-streaming/aws-s3.md)
* [Facebook](../../general/network-streaming/facebook.md)
* [HLS](../../general/network-streaming/hls-streaming.md)
* [HTTP MJPEG](../../general/network-streaming/http-mjpeg.md)
* [NDI](../../general/network-streaming/ndi.md)
* [RTMP](../../general/network-streaming/rtmp.md)
* [RTSP](../../general/network-streaming/rtsp.md)
* [SRT](../../general/network-streaming/srt.md)
* [UDP](../../general/network-streaming/udp.md)
* [WMV](../../general/network-streaming/wmv.md)
* [YouTube](../../general/network-streaming/youtube.md)


## Transcripción del vídeo

### Qué vamos a construir

Vamos a mostrar la vista previa de una webcam y transmitirla a la red por RTMP, en una aplicación C# WPF con el VisioForge Video Capture SDK. Esto es lo que vamos a construir. Una aplicación WPF que muestra la vista previa en directo de la webcam y, al mismo tiempo, transmite la cámara y el micrófono por RTMP. El vídeo es H.264, codificado en la GPU cuando el PC tiene un codificador por hardware, y el audio es AAC. La transmisión va a cualquier servidor RTMP: MediaMTX, YouTube Live o una CDN. Esta es la aplicación terminada, a la izquierda, transmitiendo a un servidor MediaMTX de la red local. A la derecha, un navegador reproduce la misma transmisión desde ese servidor. ¿Por qué RTMP? Es el protocolo de ingesta de YouTube Live, Twitch y la mayoría de las CDN. Un servidor multimedia lo recibe y lo vuelve a servir como HLS, WebRTC o RTSP, así que los espectadores no necesitan nada especial. Y transporta H.264 y AAC, que cualquier reproductor puede decodificar. SRT, RTSP, HLS y WHIP son salidas del mismo motor y se añaden de la misma manera.

### Requisitos previos

Necesita Windows 10 u 11 y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto apunta a .NET 8 o posterior. Y una webcam, y un servidor RTMP al que transmitir. Aquí es MediaMTX, un servidor multimedia gratuito y de código abierto: un único ejecutable que recibe RTMP en el puerto 1935.

### Crear un proyecto WPF

En Visual Studio, cree un proyecto nuevo. Busque WPF y elija WPF Application para C#. Llame al proyecto WebcamRtmpWpf. Elija .NET 10 y haga clic en Create.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. El propio Video Capture SDK. CrossPlatform Core, el runtime de GStreamer sobre el que funciona el motor X, con el sink RTMP y los codificadores H.264. Y CrossPlatform Libav, que contiene el codificador AAC. O use el administrador de paquetes NuGet: clic derecho en el proyecto, Manage NuGet Packages, e instale VisioForge.DotNet.VideoCapture desde la pestaña Browse. Después instale los paquetes Core y Libav para Windows x64. La pestaña Installed muestra los tres. Son los paquetes x64, así que ponga PlatformTarget en x64 en el archivo de proyecto. Una aplicación de 32 bits usa los paquetes x86.

### Construir la ventana en XAML

La ventana se escribe en XAML. Primero la propia ventana, con una cuadrícula de dos columnas y un VideoView, y el diseñador la dibuja al instante. Asocie el espacio de nombres VisioForge.Core.UI.WPF a un prefijo, aquí vf. El VideoView va en la columna derecha: ahí se dibuja la vista previa en directo. La ventana también nombra sus controladores Loaded y Closing. Después, las listas de cámaras y micrófonos, y un cuadro de texto para la dirección RTMP. La columna izquierda es un StackPanel: un cuadro combinado para las cámaras, otro para los micrófonos y un cuadro de texto para la URL RTMP. Su valor predeterminado apunta a un servidor MediaMTX en este equipo. Por último, los botones Start y Stop. Cada botón nombra un controlador Click. Los dos empiezan deshabilitados: Start hasta que el SDK se ha cargado, y Stop hasta que hay una transmisión en curso. Cada controlador que nombra el XAML va en MainWindow.xaml.cs, y los próximos capítulos los escriben.

### Inicializar el SDK

MainWindow.xaml.cs empieza con estos using: el núcleo del SDK, los tipos de salida y de fuente de X, y el motor VideoCaptureCoreX. Cuando se carga la ventana, espere VisioForgeX.InitSDKAsync una vez. Después cree el motor VideoCaptureCoreX y pásele el VideoView, donde se dibuja la vista previa. Los errores del motor llegan en el evento OnError; aquí van a la barra de título, y ahí aparece también un servidor al que no se puede llegar.

### Enumerar los dispositivos

DeviceEnumerator.Shared.VideoSourcesAsync enumera las cámaras. Guarde el array: el controlador Start toma de él el dispositivo seleccionado, y el cuadro combinado muestra el nombre visible de cada uno. AudioSourcesAsync enumera los micrófonos de la misma manera. Se seleccionan la primera cámara y el primer micrófono, y Start se habilita solo ahora, una vez cargado el SDK.

### Elegir la cámara y el micrófono

El controlador Start comprueba primero que hay una cámara y una dirección RTMP. Una dirección vacía no fallaría: el sink usaría su propio servidor predeterminado, así que se detiene aquí. La fuente de vídeo es VideoCaptureDeviceSourceSettings para la cámara seleccionada. De forma predeterminada toma 1080p a 25 fotogramas por segundo o más cuando la cámara lo admite, y 720p en caso contrario. El micrófono se convierte en la fuente de audio con CreateSourceSettingsVC, y Audio_Record lo incluye en la transmisión. Audio_Play es false, así que la aplicación no reproduce el micrófono por los altavoces.

### Transmitir por RTMP

La transmisión en sí es una línea: un RTMPOutput con la URL del cuadro de texto, añadido al motor. La vista previa sigue en el VideoView mientras sale la transmisión. Una salida permanece en el motor después de una transmisión, así que Outputs_Clear se ejecuta antes de cada una nueva. Sin ajustes de codificador, RTMPOutput toma el codificador H.264 de la GPU: NVIDIA NVENC, AMD AMF o Intel Quick Sync, y OpenH264 en la CPU cuando el PC no tiene ninguno. El audio es AAC. Sus propiedades Video y Audio admiten otros ajustes de codificador, por ejemplo una tasa de bits fija. La URL indica el servidor y la transmisión. En MediaMTX, la ruta después del puerto es el nombre de la transmisión, y la misma ruta se reproduce por HLS, WebRTC y RTSP. Para YouTube Live es la dirección de ingesta seguida de su clave de transmisión. Y rtmps, con una s, se conecta por TLS.

### Iniciar y detener

StartAsync devuelve si la captura se ha iniciado, y los botones lo siguen. StopAsync termina la transmisión y la vista previa. Si la ventana se cierra mientras transmite, cancele el cierre, espere StopAsync y vuelva a cerrar. En esa segunda pasada se liberan el motor y el SDK.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Una vez cargado el SDK, se llenan las listas de cámaras y micrófonos y se habilita Start. Escriba la dirección del servidor MediaMTX de la red local, con webcam como nombre de la transmisión. Haga clic en Start. La vista previa se pone en marcha, y la misma imagen sale hacia el servidor. Ahora, el otro lado. MediaMTX sirve cada transmisión en su propio reproductor web, en el puerto 8889 con la misma ruta. El navegador de la derecha reproduce la transmisión desde el servidor, un instante por detrás de la vista previa. Cierre el reproductor, haga clic en Stop y cierre la ventana.

### Errores frecuentes

Algunos errores frecuentes. Omitir el paquete Libav, que contiene el codificador AAC. Mezclar paquetes x86 y x64, o ejecutar una aplicación x86 con los paquetes x64. Un firewall que bloquea el puerto 1935 hacia el servidor. Y una dirección de YouTube sin la clave de transmisión al final. Pase lo que pase, el motor lo notifica en el evento OnError.

### Próximos pasos

También hay guías para las salidas SRT, RTSP, HLS y WebRTC, para grabar un MP4 mientras transmite, para transmitir una cámara IP o la pantalla, y para YouTube Live y Facebook Live. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el vídeo.

---

Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código.
