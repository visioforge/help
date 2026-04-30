---
title: Reproductor de Video C# WinForms/WPF — Búsqueda y Volumen
description: Cree un reproductor de video en C# para aplicaciones de escritorio WinForms o WPF con Media Player SDK .Net — búsqueda, volumen, velocidad y subtítulos.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Playback
  - Streaming
  - Editing
  - RTSP
  - MPEG-DASH
  - MP4
  - MKV
  - WebM
  - AVI
  - MOV
  - WMV
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - VideoView
  - UniversalSourceSettings
  - AudioRendererSettings
  - ErrorsEventArgs

---

# Crear un Reproductor de Video en C# para WinForms y WPF

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Esta guía le muestra cómo crear un reproductor de video **para escritorio Windows** (WinForms o WPF) en C# utilizando [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) con el motor de alto nivel `MediaPlayerCoreX`. Conectará reproducción de archivos, búsqueda en la línea de tiempo, pausa/reanudación, volumen y velocidad de reproducción contra un control `VideoView` de WinForms o WPF.

!!! info "Elija su enfoque"
    Esta página es para **aplicaciones de escritorio Windows** (WinForms o WPF) con Media Player SDK .Net.

    - **Escritorio multiplataforma (incluye Linux)** → [Guía del reproductor Avalonia](avalonia-player.md)
    - **Móvil + escritorio desde una sola base de código (iOS, Android, macOS, Windows)** → [Guía del reproductor .NET MAUI](maui-player.md)
    - **Solo Android** → [Guía del reproductor Android](android-player.md)
    - **Visual Basic .NET** → [Guía del reproductor de video VB.NET](video-player-vb-net.md)
    - **Basado en pipeline (grafo de bloques con renderizadores explícitos)** → [Reproductor Media Blocks SDK](../../mediablocks/GettingStarted/player.md)

!!! tip "Agentes de IA: usa el servidor MCP de VisioForge"

    ¿Lo construyes con **Claude Code**, **Cursor** u otro agente de IA?
    Conecta al servidor MCP público de VisioForge
    ([documentación](../../general/mcp-server-usage.md))
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Enfoque MediaPlayerCoreX

`MediaPlayerCoreX` es la forma más sencilla de crear un reproductor de video en C# con control total de reproducción.

### Paquetes NuGet Requeridos

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

### Implementación Completa del Reproductor de Video en C#

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

        // Iniciar la reproducción
        await _player.PlayAsync();

        _timer.Start();
    }
```

### Pausa, Reanudación y Detención

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

### Búsqueda en la Línea de Tiempo

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

## Alternativa: Pipeline de Media Blocks SDK

Si necesita la flexibilidad adicional de un grafo de bloques (procesamiento personalizado, múltiples sinks, overlays), la [Guía del Reproductor de Media Blocks SDK](../../mediablocks/GettingStarted/player.md) construye un pipeline equivalente con `UniversalSourceBlock` + `VideoRendererBlock` + `AudioRendererBlock`. El código anterior usa Media Player SDK porque está diseñado específicamente para este escenario — una instancia de `MediaPlayerCoreX` reemplaza el cableado de bloques.

## Formatos Soportados

| Categoría | Formatos |
|-----------|----------|
| Video | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Aplicaciones de Ejemplo

- [Demo de Reproductor Simple WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo)
- [Fragmento de Código de Media Player (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/media-player)
- [Demo Principal WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Main%20Demo)

## Preguntas Frecuentes

### ¿Qué formatos de video soporta el reproductor de video .NET?

El SDK soporta todos los formatos de video principales, incluyendo MP4, AVI, MKV, WMV, WebM, MOV, TS y FLV. El soporte de codecs incluye H.264, H.265/HEVC, VP8, VP9, AV1 y MPEG-2 a través del motor basado en GStreamer incluido. Los formatos de audio como MP3, AAC, WAV, FLAC y OGG también son compatibles para la reproducción.

### ¿Puedo reproducir streams RTSP o video de red en mi aplicación C#?

Sí. El método `UniversalSourceSettings.CreateAsync` acepta URIs para streams RTSP, HTTP, HLS y MPEG-DASH. Pase la URL del stream como un objeto `Uri` de la misma forma que una ruta de archivo local. Para fuentes RTSP que requieren autenticación, incluya las credenciales directamente en la URI (por ejemplo, `rtsp://usuario:contraseña@host:554/stream`).

### ¿Cómo controlo la velocidad de reproducción en el reproductor de video?

Llame a `Rate_SetAsync(double rate)` en la instancia del reproductor. Una velocidad de 1.0 es normal, 2.0 es el doble y 0.5 es la mitad. El rango soportado depende del formato del medio, pero la mayoría de los archivos admiten velocidades entre 0.25x y 4.0x. La velocidad puede cambiarse durante la reproducción sin detener el video.

### ¿El SDK soporta renderizado de subtítulos?

Sí. El SDK puede renderizar subtítulos incrustados en contenedores MKV y MP4, así como archivos de subtítulos externos SRT y ASS. Las pistas de subtítulos se detectan automáticamente cuando se abre un archivo, y puede seleccionar cuál pista mostrar a través de la API del reproductor.

### ¿Puedo construir un reproductor de video multiplataforma con este SDK?

Sí. El SDK funciona en Windows, macOS, Linux, Android e iOS. Para aplicaciones de escritorio multiplataforma, utilice el [framework Avalonia UI](avalonia-player.md) con la misma API `MediaPlayerCoreX`. El motor de reproducción principal es idéntico en todas las plataformas — solo difieren la superficie de renderizado de video y los paquetes NuGet.

## Ver También

- [Crear un Reproductor de Video en VB.NET](video-player-vb-net.md) — el mismo tutorial usando sintaxis VB.NET
- [Reproductor Multiplataforma con Avalonia](avalonia-player.md) — construya un reproductor de video para Windows, macOS y Linux con Avalonia UI
- [Guía del reproductor .NET MAUI](maui-player.md) — una base de código C# para iOS, Android, macOS y Windows
- [Guía del reproductor Android](android-player.md) — configuración y despliegue específicos de Android
- [Modo Bucle y Rango de Posición](loop-and-position-range.md) — configure el bucle, repetición A-B y reproducción de segmentos
- [Reproductor Media Blocks SDK](../../mediablocks/GettingStarted/player.md) — alternativa basada en pipeline con renderizadores explícitos
- [Página del Producto Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net)
