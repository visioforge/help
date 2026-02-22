---
title: Crear un Reproductor de Video en C# - Guía Completa de .NET
description: Cree un reproductor de video en C# con controles de reproducción, búsqueda en la línea de tiempo y volumen usando Media Player SDK .Net.
---

# Crear un Reproductor de Video en C#

Esta guia le muestra como crear un reproductor de video con todas las funciones en C# utilizando [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net). Implementara la reproduccion de archivos, la busqueda en la linea de tiempo, pausa/reanudacion y control de volumen. Se cubren dos enfoques: la API de alto nivel `MediaPlayerCoreX` y la API basada en pipeline `MediaBlocksPipeline`.

## Enfoque MediaPlayerCoreX (Recomendado)

`MediaPlayerCoreX` es la forma mas sencilla de crear un reproductor de video en C# con control total de reproduccion.

### Paquetes NuGet Requeridos

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

### Implementacion Completa del Reproductor de Video en C#

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

public partial class Form1 : Form
{
    private MediaPlayerCoreX _player;
    private System.Timers.Timer _timer;
    private bool _timerFlag;

    private async void Form1_Load(object sender, EventArgs e)
    {
        // Inicializar el SDK
        await VisioForgeX.InitSDKAsync();

        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += Timer_Elapsed;

        // Crear el motor del reproductor con el control VideoView
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;
        _player.OnStop += Player_OnStop;

        // Rellenar los dispositivos de salida de audio
        foreach (var device in await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound))
        {
            cbAudioOutput.Items.Add(device.Name);
        }

        if (cbAudioOutput.Items.Count > 0)
            cbAudioOutput.SelectedIndex = 0;
    }
```

### Abrir y Reproducir un Archivo de Video

```csharp
    private void btOpenFile_Click(object sender, EventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.avi;*.mkv;*.wmv;*.webm;*.mov;*.ts" +
                     "|Audio Files|*.mp3;*.aac;*.wav;*.wma|All Files|*.*"
        };

        if (ofd.ShowDialog() == DialogResult.OK)
            edFilename.Text = ofd.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // Establecer el dispositivo de salida de audio
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        var audioDevice = devices.First(x => x.Name == cbAudioOutput.Text);
        _player.Audio_OutputDevice = new AudioRendererSettings(audioDevice);

        // Abrir el archivo multimedia
        var source = await UniversalSourceSettings.CreateAsync(
            new Uri(edFilename.Text));
        await _player.OpenAsync(source);

        // Iniciar la reproduccion
        await _player.PlayAsync();

        _timer.Start();
    }
```

### Pausa, Reanudacion y Detencion

```csharp
    private async void btPause_Click(object sender, EventArgs e)
    {
        await _player.PauseAsync();
    }

    private async void btResume_Click(object sender, EventArgs e)
    {
        await _player.ResumeAsync();
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        _timer.Stop();

        if (_player != null)
        {
            await _player.StopAsync();
        }

        tbTimeline.Value = 0;
    }
```

### Busqueda en la Linea de Tiempo

```csharp
    private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        _timerFlag = true;

        if (_player == null) return;

        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();

        Invoke(() =>
        {
            tbTimeline.Maximum = (int)duration.TotalSeconds;
            lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

            if (tbTimeline.Maximum >= position.TotalSeconds)
                tbTimeline.Value = (int)position.TotalSeconds;
        });

        _timerFlag = false;
    }

    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (!_timerFlag && _player != null)
        {
            await _player.Position_SetAsync(
                TimeSpan.FromSeconds(tbTimeline.Value));
        }
    }
```

### Control de Volumen y Velocidad

```csharp
    private void tbVolume_Scroll(object sender, EventArgs e)
    {
        if (_player != null)
            _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
    }

    private async void tbSpeed_Scroll(object sender, EventArgs e)
    {
        await _player.Rate_SetAsync(tbSpeed.Value / 10.0);
    }
```

### Manejo de Errores y Limpieza

```csharp
    private void Player_OnError(object sender, ErrorsEventArgs e)
    {
        Invoke(() => edLog.Text += e.Message + Environment.NewLine);
    }

    private void Player_OnStop(object sender, StopEventArgs e)
    {
        Invoke(() => tbTimeline.Value = 0);
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _timer.Stop();

        if (_player != null)
        {
            _player.OnError -= Player_OnError;
            _player.OnStop -= Player_OnStop;
            await _player.DisposeAsync();
        }

        VisioForgeX.DestroySDK();
    }
}
```

## Enfoque MediaBlocksPipeline

Para mayor flexibilidad, utilice `MediaBlocksPipeline` para construir un reproductor con bloques de procesamiento personalizados.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

private MediaBlocksPipeline _pipeline;

private async void StartPlayback(string filePath)
{
    _pipeline = new MediaBlocksPipeline();

    // Crear la fuente del archivo y detectar los flujos disponibles
    var fileSourceSettings = await UniversalSourceSettings.CreateAsync(filePath);
    var hasVideo = fileSourceSettings.GetInfo().VideoStreams.Count > 0;
    var hasAudio = fileSourceSettings.GetInfo().AudioStreams.Count > 0;

    var fileSource = new UniversalSourceBlock(fileSourceSettings);

    // Conectar el renderizador de video si hay video disponible
    if (hasVideo)
    {
        var videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
        _pipeline.Connect(fileSource, videoRenderer);
    }

    // Conectar el renderizador de audio si hay audio disponible
    if (hasAudio)
    {
        var audioOutputDevice = (await DeviceEnumerator.Shared
            .AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound))[0];
        var audioOutput = new AudioRendererBlock(
            new AudioRendererSettings(audioOutputDevice));
        _pipeline.Connect(fileSource, audioOutput);
    }

    // Iniciar la reproduccion
    await _pipeline.StartAsync();
}
```

## Formatos Soportados

| Categoria | Formatos |
|-----------|----------|
| Video | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Aplicaciones de Ejemplo

- [Demo de Reproductor Simple WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo)
- [Fragmento de Codigo de Media Player (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/media-player)
- [Demo Principal WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Main%20Demo)

## Preguntas Frecuentes

### Que formatos de video soporta el reproductor de video .NET?

El SDK soporta todos los formatos de video principales, incluyendo MP4, AVI, MKV, WMV, WebM, MOV, TS y FLV. El soporte de codecs incluye H.264, H.265/HEVC, VP8, VP9, AV1 y MPEG-2 a traves del motor basado en GStreamer incluido. Los formatos de audio como MP3, AAC, WAV, FLAC y OGG tambien son compatibles para la reproduccion.

### Puedo reproducir streams RTSP o video de red en mi aplicacion C#?

Si. El metodo `UniversalSourceSettings.CreateAsync` acepta URIs para streams RTSP, HTTP, HLS y MPEG-DASH. Pase la URL del stream como un objeto `Uri` de la misma forma que una ruta de archivo local. Para fuentes RTSP que requieren autenticacion, incluya las credenciales directamente en la URI (por ejemplo, `rtsp://usuario:contraseña@host:554/stream`).

### Como controlo la velocidad de reproduccion en el reproductor de video?

Llame a `Rate_SetAsync(double rate)` en la instancia del reproductor. Una velocidad de 1.0 es normal, 2.0 es el doble y 0.5 es la mitad. El rango soportado depende del formato del medio, pero la mayoria de los archivos admiten velocidades entre 0.25x y 4.0x. La velocidad puede cambiarse durante la reproduccion sin detener el video.

### El SDK soporta renderizado de subtitulos?

Si. El SDK puede renderizar subtitulos incrustados en contenedores MKV y MP4, asi como archivos de subtitulos externos SRT y ASS. Las pistas de subtitulos se detectan automaticamente cuando se abre un archivo, y puede seleccionar cual pista mostrar a traves de la API del reproductor.

### Puedo construir un reproductor de video multiplataforma con este SDK?

Si. El SDK funciona en Windows, macOS, Linux, Android e iOS. Para aplicaciones de escritorio multiplataforma, utilice el [framework Avalonia UI](avalonia-player.md) con la misma API `MediaPlayerCoreX`. El motor de reproduccion principal es identico en todas las plataformas — solo difieren la superficie de renderizado de video y los paquetes NuGet.

## Ver Tambien

- [Crear un Reproductor de Video en VB.NET](video-player-vb-net.md) — el mismo tutorial usando sintaxis VB.NET
- [Reproductor Multiplataforma con Avalonia](avalonia-player.md) — construya un reproductor de video para Windows, macOS y Linux con Avalonia UI
- [Reproducir Video en .NET (Guia Multiplataforma)](play-video-dotnet.md) — resumen de todos los frameworks de UI y plataformas compatibles
- [Modo Bucle y Rango de Posicion](loop-and-position-range.md) — configure el bucle, repeticion A-B y reproduccion de segmentos
- [Primeros Pasos con MediaBlocks Pipeline](../../mediablocks/GettingStarted/index.md) — aprenda el enfoque basado en pipeline para procesamiento multimedia avanzado
- [Pagina del Producto Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net)
