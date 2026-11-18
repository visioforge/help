---
title: Audio bidireccional con cámara IP en C# WPF — ONVIF
description: "Hable por el altavoz de una cámara IP desde C# WPF: vista previa RTSP con el sonido de la cámara y el micrófono del PC en G.711 por el backchannel ONVIF."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WPF
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - Audio
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - AudioRendererSettings
  - DeviceEnumerator
  - VisioForgeX

---

# Audio bidireccional con una cámara IP en C# WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Este tutorial construye un pequeño intercomunicador en una aplicación C# WPF con el motor multiplataforma `VideoCaptureCoreX`:

- la URL RTSP de la cámara, su usuario y su contraseña se escriben en la ventana;
- un `VideoView` muestra la cámara en directo, y el propio micrófono de la cámara suena en los altavoces del PC elegidos en la ventana;
- el micrófono del PC elegido en la ventana suena en el altavoz de la cámara, enviado por el backchannel de audio RTSP de ONVIF;
- Start y Stop abren y cierran las dos direcciones a la vez.

Sirve para videoporteros, estaciones de intercomunicación y cámaras con altavoz o salida de audio. Cómo funciona el backchannel, la versión con Media Blocks y el volumen del altavoz por ONVIF se describen en la página de [audio bidireccional](../video-sources/ip-cameras/two-way-audio.md).

!!! note "Versión del SDK"
    `Audio_BackchannelSource` está en las versiones del SDK posteriores a 2026.11.16.

## Repositorio del código fuente

El proyecto completo está en GitHub:

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-two-way-audio-wpf)

## Paquetes necesarios

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — el SDK; el `VideoView` de WPF viene en él
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — el runtime de GStreamer sobre el que funciona el motor X, con la fuente RTSP, los decodificadores y el codificador G.711 que usa el backchannel
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — el decodificador AAC, para las cámaras que envían el sonido en AAC

Son los paquetes x64, por eso el proyecto indica `<PlatformTarget>x64</PlatformTarget>`; una aplicación de 32 bits usa los paquetes `.x86`. La lista completa está en [Implementación en Windows](../../deployment-x/Windows.md).

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Ejemplo de implementación en C#

### La ventana (MainWindow.xaml)

Los ajustes están a la izquierda y la cámara a la derecha. `PasswordBox` muestra puntos. Start está desactivado hasta que el SDK se ha cargado, y Stop hasta que la cámara ha arrancado.

```xml
<Window x:Class="IpCameraTwoWayAudioWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="IP Camera Two-Way Audio" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera URL" />
            <TextBox x:Name="edURL" />
            <Label Content="Login" Margin="0,10,0,0" />
            <TextBox x:Name="edLogin" />
            <Label Content="Password" Margin="0,10,0,0" />
            <PasswordBox x:Name="edPassword" />
            <Label Content="Microphone (to the camera speaker)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="Speakers (the camera's sound)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbSpeakers" />

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

### El código (MainWindow.xaml.cs)

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraTwoWayAudioWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private AudioCaptureDeviceInfo[] microphones;

    private AudioOutputDeviceInfo[] speakers;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Muestra los errores del SDK en la barra de título
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.DisplayName);

        // Preselecciona la salida predeterminada del sistema
        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = Math.Max(0, Array.FindIndex(speakers, device => device.IsDefault));

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Title = "Enter the camera URL";
            return;
        }

        if (cbMicrophone.SelectedIndex < 0)
        {
            Title = "No microphone";
            return;
        }

        Title = "Connecting...";
        btStart.IsEnabled = false;

        var started = false;
        try
        {
            // El vídeo de la cámara y su propio micrófono, reproducidos en los altavoces elegidos
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Password, audioEnabled: true);
            videoCapture1.Audio_OutputDevice = cbSpeakers.SelectedIndex < 0 ? null : new AudioRendererSettings(speakers[cbSpeakers.SelectedIndex]);
            videoCapture1.Audio_Play = true;

            // El micrófono del PC, enviado al altavoz de la cámara por el backchannel de audio ONVIF
            videoCapture1.Audio_BackchannelSource = microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC();

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Title = ex.Message;
        }

        // Un error del SDK publicado durante el inicio se queda en el título
        if (Title == "Connecting...") Title = started ? "Talking to the camera" : "The camera did not start";
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        Title = "IP Camera Two-Way Audio";
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

        e.Cancel = true; // todavía cargando o hablando: primero detener
        if (!btStop.IsEnabled) return; // ya cargando, iniciando o deteniendo
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // no Close(): WPF lanza una excepción dentro de Closing
    }
}
```

## Características clave de la implementación

1. **Inicialización del SDK** - `VisioForgeX.InitSDKAsync` se ejecuta una vez, al cargarse la ventana. La primera ejecución en un PC tarda más, por eso Start se activa solo cuando termina. `VisioForgeX.DestroySDK` lo libera al cerrar la ventana
2. **La cámara** - `RTSPSourceSettings.CreateAsync` recibe la URL, el usuario y la contraseña como argumentos separados, de modo que la URL no los lleva. Con `audioEnabled: true` se usa el propio sonido de la cámara cuando su flujo lo tiene, y `Audio_Play` lo reproduce en los altavoces indicados en `Audio_OutputDevice`, ya que `Audio_Source` se deja vacío. Sin `Audio_OutputDevice` el sonido no se reproduce. El [directorio de marcas de cámaras IP](../../camera-brands/index.md) recoge las rutas RTSP de muchas cámaras
3. **El backchannel** - `Audio_BackchannelSource` acepta las mismas fuentes de audio que `Audio_Source`; `CreateSourceSettingsVC` convierte el micrófono elegido en una de ellas. Al iniciar, el SDK pide a la cámara su backchannel de audio ONVIF y le envía el micrófono en G.711 mu-law (PCMU) a 8000 Hz, el códec que ONVIF exige a toda cámara con salida de audio; a una cámara cuyo backchannel solo ofrece G.711 A-law (PCMA) le envía A-law. El sonido sale del PC tal como se captura, sin añadir el búfer de recepción RTSP
4. **El inicio** - una cámara que no responde tarda en fallar, por eso el título dice "Connecting..." y Start queda desactivado hasta que `StartAsync` termina. Un mensaje de `OnError` publicado durante el inicio se queda en el título
5. **Detener y cerrar** - `StopAsync` cierra las dos direcciones. Cerrar la ventana mientras funciona cancela el cierre, espera a `StopAsync` y vuelve a cerrar; mientras el SDK se carga, o hay un inicio o una detención en curso, el cierre se cancela: vuelva a cerrar la ventana cuando termine

## Solución de problemas

- **La cámara rechaza la conexión** - la ruta no corresponde a la marca de la cámara, o el usuario o la contraseña son incorrectos. Varias contraseñas erróneas seguidas hacen que muchas cámaras bloqueen la cuenta durante un tiempo
- **No llega ninguna imagen, con la URL y la contraseña correctas** - pedir el backchannel añade la cabecera ONVIF `Require: www.onvif.org/ver20/backchannel` a la petición DESCRIBE y al SETUP del backchannel, y una cámara sin backchannel puede rechazar cualquiera de ellas (responde 551 Option Not Supported): entonces un error sustituye al título, aunque el inicio ya haya indicado éxito. Deje `Audio_BackchannelSource` vacío y vuelva a iniciar: si llega la imagen, la cámara no tiene backchannel
- **Llega la imagen, pero el altavoz de la cámara está en silencio** - la cámara no ofrece backchannel, o no ofrece ni G.711 mu-law ni A-law a 8000 Hz: el SDK indica en el título de la ventana los códecs que ofrece la cámara. Compruebe que el audio bidireccional está activado en la interfaz web de la cámara, que ninguna aplicación del fabricante ni ningún NVR ocupa su canal de conversación, y suba el volumen del altavoz (consulte [Volumen del altavoz por ONVIF](../video-sources/ip-cameras/two-way-audio.md#volumen-del-altavoz-por-onvif))
- **No se oye a sí mismo por el micrófono de la cámara** - es lo esperado. Las cámaras con altavoz lo cancelan en su micrófono, así que compruebe el altavoz de oído, junto a la cámara
- **La persona junto a la cámara oye un eco** - el PC reproduce el sonido de la cámara en sus altavoces mientras su micrófono envía a la cámara, así que el micrófono del PC vuelve a captar el sonido de la cámara y lo devuelve. Use unos auriculares con micrófono en el PC, o ponga `Audio_Play` a `false`

## Guías relacionadas

- [Audio bidireccional con cámaras IP](../video-sources/ip-cameras/two-way-audio.md) — cómo funciona el backchannel, Media Blocks, volumen del altavoz por ONVIF
- [Grabar una cámara IP en MP4](ip-camera-capture-mp4.md) — la misma conexión con la cámara, grabada en un archivo
- [Configuración de la fuente RTSP](../video-sources/ip-cameras/rtsp.md) — transporte UDP/TCP, baja latencia
- [Integración de cámaras IP ONVIF](../video-sources/ip-cameras/onvif.md) — detección y elección de perfil

---
Visite nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para obtener más ejemplos de código. ¿Necesita la URL RTSP de su cámara? Consulte nuestro [directorio de marcas de cámaras IP](../../camera-brands/index.md).
