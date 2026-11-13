---
title: Reproductor de Video C# WinForms/WPF — Búsqueda y Volumen
description: Cree un reproductor de video en C# para aplicaciones de escritorio WinForms o WPF con Media Player SDK .NET — búsqueda, volumen, velocidad y subtítulos.
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
  - AudioRendererSettings
  - ErrorsEventArgs
video:
  id: YJbUV370TKo
  upload_date: 2026-11-12T05:22:05Z
  duration: PT12M7S

---

# Crear un Reproductor de Video en C# para WinForms y WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Este tutorial crea un reproductor de video **para escritorio Windows** en C# con [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) y su motor `MediaPlayerCoreX`: una ventana WPF que reproduce un archivo con línea de tiempo, pausa y reanudación, volumen, velocidad de reproducción, elección de la salida de audio y un registro de errores. El mismo reproductor como formulario WinForms está [más abajo en esta página](#segunda-opcion-winforms).

!!! info "Elija su enfoque"
    Esta página es para **aplicaciones de escritorio Windows** (WinForms o WPF) con Media Player SDK .NET.

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

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YJbUV370TKo?controls=1" title="Crear un reproductor de video en C# WPF - tutorial de YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/_VideoTutorials/video-player-csharp-wpf)

La película del video es [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, con licencia [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

El video crea el proyecto con .NET 10, que requiere Visual Studio 2026; en Visual Studio 2022, elija .NET 8 o .NET 9.

## Paquetes NuGet requeridos

- [VisioForge.DotNet.MediaPlayer](https://www.nuget.org/packages/VisioForge.DotNet.MediaPlayer/) — el SDK; los controles `VideoView` de WPF y WinForms vienen incluidos
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — el runtime de GStreamer sobre el que funciona `MediaPlayerCoreX`
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — los códecs FFmpeg (libav) con los que se decodifican muchos formatos

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.12]"/>
```

Son los paquetes x64, por lo que el proyecto define `<PlatformTarget>x64</PlatformTarget>`; una aplicación de 32 bits usa los paquetes `.x86`. Consulte [Despliegue en Windows](../../deployment-x/Windows.md) para la lista completa.

## La ventana en XAML

El prefijo `vf` asigna el espacio de nombres `VisioForge.Core.UI.WPF` del ensamblado `VisioForge.Core`. El `VideoView` ocupa la columna izquierda, con la línea de tiempo y los botones de reproducción debajo; la columna derecha contiene el nombre del archivo, la salida de audio, los controles deslizantes de volumen y velocidad y el registro de errores. Play permanece deshabilitado hasta que el SDK se ha cargado.

```xml
<Window x:Class="VideoPlayerWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Video Player" Height="600" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="*" />
            <ColumnDefinition Width="260" />
        </Grid.ColumnDefinitions>
        <Grid.RowDefinitions>
            <RowDefinition Height="*" />
            <RowDefinition Height="Auto" />
            <RowDefinition Height="Auto" />
        </Grid.RowDefinitions>

        <vf:VideoView x:Name="VideoView1" Background="Black" />

        <DockPanel Grid.Row="1" Margin="0,10,0,0">
            <TextBlock x:Name="lbTime" DockPanel.Dock="Right" Text="00:00:00 / 00:00:00"
                       Margin="10,0,0,0" VerticalAlignment="Center" />
            <Slider x:Name="slTimeline" IsMoveToPointEnabled="True"
                    ValueChanged="slTimeline_ValueChanged"
                    Thumb.DragCompleted="slTimeline_DragCompleted" />
        </DockPanel>

        <StackPanel Grid.Row="2" Orientation="Horizontal" Margin="0,10,0,0">
            <Button x:Name="btPlay" Content="Play" Width="90" IsEnabled="False" Click="btPlay_Click" />
            <Button x:Name="btPause" Content="Pause" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btPause_Click" />
            <Button x:Name="btResume" Content="Resume" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btResume_Click" />
            <Button x:Name="btStop" Content="Stop" Width="90" Margin="10,0,0,0" IsEnabled="False"
                    Click="btStop_Click" />
        </StackPanel>

        <StackPanel Grid.Column="1" Grid.RowSpan="3" Margin="10,0,0,0">
            <Label Content="File" />
            <DockPanel>
                <Button x:Name="btOpen" Content="..." DockPanel.Dock="Right" Width="30" Margin="5,0,0,0"
                        Click="btOpen_Click" />
                <TextBox x:Name="edFilename" />
            </DockPanel>

            <Label Content="Audio output" Margin="0,10,0,0" />
            <ComboBox x:Name="cbSpeakers" />

            <Label Content="Volume" Margin="0,10,0,0" />
            <Slider x:Name="slVolume" Maximum="100" Value="80" AutoToolTipPlacement="TopLeft"
                    ValueChanged="slVolume_ValueChanged" />

            <Label Content="Speed" Margin="0,10,0,0" />
            <Slider x:Name="slSpeed" Minimum="0.5" Maximum="2" Value="1" TickFrequency="0.25"
                    IsSnapToTickEnabled="True" TickPlacement="BottomRight"
                    AutoToolTipPlacement="TopLeft" AutoToolTipPrecision="2"
                    ValueChanged="slSpeed_ValueChanged" />

            <Label Content="Log" Margin="0,10,0,0" />
            <TextBox x:Name="edLog" Height="150" IsReadOnly="True" TextWrapping="Wrap"
                     VerticalScrollBarVisibility="Auto" />
        </StackPanel>
    </Grid>
</Window>
```

## El código de la ventana

```csharp
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;

namespace VideoPlayerWpf;

public partial class MainWindow : Window
{
    private MediaPlayerCoreX _player;

    private AudioOutputDeviceInfo[] _speakers;

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };

    private bool _timerMovesSlider; // true mientras el temporizador, no el usuario, mueve la línea de tiempo

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Lo generan StopAsync y el fin del archivo; solo se atiende el fin del archivo
        // (Stop está habilitado), no una parada tardía del archivo anterior (State ya no es Free)
        _player.OnStop += (_, _) => Dispatcher.InvokeAsync(() =>
        {
            if (btStop.IsEnabled && _player.State == PlaybackState.Free) Stopped();
        });

        _speakers = await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound);
        foreach (var device in _speakers)
            cbSpeakers.Items.Add(device.Name);

        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = 0;

        _timer.Tick += Timer_Tick;
        btPlay.IsEnabled = true;
    }

    private void btOpen_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            Filter = "Video files|*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.ts|All files|*.*"
        };

        if (dialog.ShowDialog() == true) edFilename.Text = dialog.FileName;
    }

    private async void btPlay_Click(object sender, RoutedEventArgs e)
    {
        ShowBusy();
        edLog.Clear();

        if (cbSpeakers.SelectedIndex >= 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(_speakers[cbSpeakers.SelectedIndex]);
        _player.Audio_OutputDevice_Volume = slVolume.Value / 100;

        if (await _player.OpenAsync(edFilename.Text))
        {
            await _player.Rate_SetAsync(slSpeed.Value); // un archivo recién abierto empieza a 1.0
            await _player.PlayAsync();
        }

        // Free: el archivo no se abrió o no se reprodujo (el motivo está en el registro), o ya ha terminado
        if (_player.State == PlaybackState.Free)
        {
            Stopped();
            return;
        }

        ShowPlaying(true);
        _timer.Start();
    }

    private async void btPause_Click(object sender, RoutedEventArgs e)
    {
        await _player.PauseAsync();
    }

    private async void btResume_Click(object sender, RoutedEventArgs e)
    {
        await _player.ResumeAsync();
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        _timer.Stop();
        ShowBusy();
        await _player.StopAsync();
        Stopped();
    }

    private void ShowBusy()
    {
        btPlay.IsEnabled = btPause.IsEnabled = btResume.IsEnabled = btStop.IsEnabled = false;
    }

    private void ShowPlaying(bool playing)
    {
        btPlay.IsEnabled = !playing;
        btPause.IsEnabled = btResume.IsEnabled = btStop.IsEnabled = playing;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        lbTime.Text = "00:00:00 / 00:00:00";
        _timerMovesSlider = true;
        slTimeline.Value = 0;
        _timerMovesSlider = false;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // detenido, o el usuario arrastra

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
        slTimeline.Maximum = duration.TotalSeconds;
        slTimeline.Value = position.TotalSeconds;
        _timerMovesSlider = false;
    }

    // Un clic en la pista busca de inmediato; un arrastre busca una vez al terminar
    private async void slTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_timerMovesSlider && !slTimeline.IsMouseCaptureWithin) await SeekAsync();
    }

    private async void slTimeline_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!e.Canceled && e.HorizontalChange != 0) await SeekAsync(); // no una pulsación sin movimiento
    }

    private async Task SeekAsync()
    {
        if (btStop.IsEnabled) await _player.Position_SetAsync(TimeSpan.FromSeconds(slTimeline.Value));
    }

    private void slVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_player != null) _player.Audio_OutputDevice_Volume = slVolume.Value / 100;
    }

    private async void slSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (btStop.IsEnabled) await _player.Rate_SetAsync(slSpeed.Value); // si no, Play la aplica
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!btPlay.IsEnabled && !btStop.IsEnabled)
        {
            e.Cancel = true; // todavía cargando el SDK, abriendo un archivo o deteniendo
            return;
        }

        _timer.Stop();
        _player.Dispose(); // primero detiene la reproducción
        VisioForgeX.DestroySDK();
    }
}
```

## Cómo funciona

1. **Inicialización del SDK** — `VisioForgeX.InitSDKAsync` se ejecuta una vez, al cargarse la ventana. La primera ejecución en un equipo tarda más, por eso Play solo se habilita cuando la llamada termina. Un solo `MediaPlayerCoreX` sirve para todos los archivos que reproduce la aplicación; `Dispose` y `VisioForgeX.DestroySDK` los liberan al cerrar la ventana.
2. **Salida de audio** — `Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)` enumera los altavoces y auriculares, y `AudioRendererSettings` envía el sonido al elegido. `Audio_OutputDevice_Volume` acepta de 0.0 a 1.0 y puede cambiar mientras se reproduce el archivo.
3. **Abrir y reproducir** — `OpenAsync` acepta una ruta de archivo o una URL y devuelve `false` cuando la fuente no se puede abrir; el motivo llega por `OnError`. `PlayAsync` inicia la reproducción. La velocidad se establece después de `OpenAsync`, porque un archivo recién abierto siempre empieza a 1.0. Después, `State` indica si el archivo se reproduce: `PlaybackState.Free` significa que no se abrió o no se reprodujo, o que un archivo muy corto ya ha terminado, y la ventana vuelve al estado detenido.
4. **Línea de tiempo** — un `DispatcherTimer` lee `Position_GetAsync` y `DurationAsync` dos veces por segundo en el hilo de la interfaz. El indicador `_timerMovesSlider` distingue las actualizaciones del propio temporizador de las del usuario, de modo que el temporizador nunca busca. Un clic en la pista busca de inmediato; un arrastre busca una vez, en `Thumb.DragCompleted`, y el temporizador no toca el control deslizante mientras el ratón lo sujeta. La búsqueda también funciona en pausa.
5. **Velocidad** — `Rate_SetAsync` establece la velocidad de reproducción: 1.0 es la velocidad normal, 2.0 el doble, 0.5 la mitad. Aquí el control deslizante va de 0.5 a 2 en pasos de 0.25.
6. **Detener, fin del archivo y cierre** — `OnStop` se dispara después de `StopAsync` y al terminar el archivo; llega en otro hilo, por eso el controlador vuelve al hilo de la interfaz con `Dispatcher.InvokeAsync`, igual que `OnError`. Play y Stop deshabilitan los botones de reproducción mientras trabajan, y el botón Stop restablece la ventana por sí mismo después de `StopAsync`, así que el controlador solo actúa mientras Stop está habilitado y `State` es `PlaybackState.Free`: el fin del archivo, no un `OnStop` tardío del archivo anterior que llega cuando el siguiente ya ha empezado. Cerrar la ventana mientras se carga el SDK, se abre un archivo o se detiene la reproducción se cancela; en otro caso, `Dispose` detiene la reproducción y libera el reproductor.

## Notas

- El volumen fijado antes de Play se conserva y se aplica al iniciar la reproducción.
- Sin ningún dispositivo de audio en la lista, el archivo se reproduce sin sonido.
- `OpenAsync` también acepta una URL `rtsp://`, `http://` o HLS; un flujo en directo no tiene duración, así que la etiqueta de tiempo muestra la posición, la línea de tiempo se queda al principio y la búsqueda depende de la fuente.

## Segunda opción: WinForms

El mismo reproductor en un formulario WinForms. Un `TrackBar` genera `Scroll` solo cuando el usuario lo mueve, así que el temporizador puede actualizar la línea de tiempo sin el indicador que necesita el control deslizante de WPF, y un `System.Windows.Forms.Timer` funciona en el hilo de la interfaz. El `TrackBar` retiene el ratón mientras se mantiene pulsado un botón sobre él, así que un clic o un arrastre busca una vez, en `MouseUp`. El formulario contiene un `VideoView` de `VisioForge.Core.UI.WinForms` llamado `VideoView1`, los `TrackBar` `tbTimeline`, `tbVolume` (0–100) y `tbSpeed` (5–20, leído como 0.5–2.0), un `ComboBox` `cbAudioOutput`, los botones `btOpenFile`, `btStart`, `btPause`, `btResume` y `btStop` (todos salvo `btOpenFile` deshabilitados en tiempo de diseño), la etiqueta `lbTime` y los cuadros de texto `edFilename` y `edLog` (multilínea, de solo lectura). Conecte `Scroll` en los tres `TrackBar` y `MouseUp` en `tbTimeline`, `Click` en los botones, y `Load` y `FormClosing` en el formulario.

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;

public partial class Form1 : Form
{
    private MediaPlayerCoreX _player;

    private AudioOutputDeviceInfo[] _speakers;

    private readonly System.Windows.Forms.Timer _timer = new() { Interval = 500 };

    private bool _scrolled; // true cuando el usuario movió la línea de tiempo con el botón del ratón pulsado

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => BeginInvoke(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Lo generan StopAsync y el fin del archivo; solo se atiende el fin del archivo
        // (Stop está habilitado), no una parada tardía del archivo anterior (State ya no es Free)
        _player.OnStop += (_, _) => BeginInvoke(() =>
        {
            if (btStop.Enabled && _player.State == PlaybackState.Free) Stopped();
        });

        _speakers = await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound);
        foreach (var device in _speakers)
            cbAudioOutput.Items.Add(device.Name);

        if (cbAudioOutput.Items.Count > 0) cbAudioOutput.SelectedIndex = 0;

        _timer.Tick += Timer_Tick;
        btStart.Enabled = true;
    }

    private void btOpenFile_Click(object sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Filter = "Video files|*.mp4;*.mkv;*.avi;*.mov;*.wmv;*.webm;*.ts|All files|*.*"
        };

        if (dialog.ShowDialog() == DialogResult.OK) edFilename.Text = dialog.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        ShowBusy();
        edLog.Clear();

        if (cbAudioOutput.SelectedIndex >= 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(_speakers[cbAudioOutput.SelectedIndex]);
        _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;

        if (await _player.OpenAsync(edFilename.Text))
        {
            await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // un archivo recién abierto empieza a 1.0
            await _player.PlayAsync();
        }

        // Free: el archivo no se abrió o no se reprodujo (el motivo está en el registro), o ya ha terminado
        if (_player.State == PlaybackState.Free)
        {
            Stopped();
            return;
        }

        ShowPlaying(true);
        _timer.Start();
    }

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
        ShowBusy();
        await _player.StopAsync();
        Stopped();
    }

    private void ShowBusy()
    {
        btStart.Enabled = btPause.Enabled = btResume.Enabled = btStop.Enabled = false;
    }

    private void ShowPlaying(bool playing)
    {
        btStart.Enabled = !playing;
        btPause.Enabled = btResume.Enabled = btStop.Enabled = playing;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        lbTime.Text = "00:00:00 / 00:00:00";
        tbTimeline.Value = 0;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();
        if (!_timer.Enabled || tbTimeline.Capture) return; // detenido, o el usuario arrastra

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        // Asignar Value no genera Scroll, así que esto no busca
        tbTimeline.Maximum = (int)duration.TotalSeconds;
        tbTimeline.Value = Math.Min((int)position.TotalSeconds, tbTimeline.Maximum);
    }

    // Una tecla busca de inmediato; un clic o un arrastre busca una vez al soltar el botón del ratón
    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (tbTimeline.Capture) _scrolled = true;
        else await SeekAsync();
    }

    private async void tbTimeline_MouseUp(object sender, MouseEventArgs e)
    {
        if (!_scrolled) return; // una pulsación sin movimiento no busca

        _scrolled = false;
        await SeekAsync();
    }

    private async Task SeekAsync()
    {
        if (btStop.Enabled) await _player.Position_SetAsync(TimeSpan.FromSeconds(tbTimeline.Value));
    }

    private void tbVolume_Scroll(object sender, EventArgs e)
    {
        if (_player != null) _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
    }

    private async void tbSpeed_Scroll(object sender, EventArgs e)
    {
        if (btStop.Enabled) await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // si no, Start la aplica
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // todavía cargando el SDK, abriendo un archivo o deteniendo
            return;
        }

        _timer.Stop();
        _player.Dispose(); // primero detiene la reproducción
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

Sí. `OpenAsync` acepta una URL RTSP, HTTP, HLS o MPEG-DASH además de una ruta de archivo, así que el mismo código reproduce un flujo. Para fuentes RTSP que requieren autenticación, incluya las credenciales directamente en la URL (p. ej., `rtsp://user:password@host:554/stream`).

### ¿Cómo controlo la velocidad de reproducción en el reproductor de video?

Llame a `Rate_SetAsync(double rate)` en el reproductor después de `OpenAsync`. Una velocidad de 1.0 es normal, 2.0 es el doble y 0.5 es la mitad. La velocidad puede cambiarse durante la reproducción sin detener el video. Después de `StopAsync` o del fin del archivo, la siguiente reproducción empieza a 1.0, así que establezca la velocidad de nuevo tras el siguiente `OpenAsync`, como hace el código anterior.

### ¿El SDK soporta renderizado de subtítulos?

Sí. `Subtitles_Enabled` está activado por defecto, así que `OpenAsync` dibuja la pista de subtítulos incrustada de un archivo MKV o MP4. Para cargar un archivo externo SRT, ASS o WebVTT, establezca `Subtitles_ExternalFile` antes de `OpenAsync`; `Subtitles_Settings` define el aspecto de los subtítulos y `Subtitle_Streams` enumera las pistas de subtítulos del archivo.

### ¿Puedo construir un reproductor de video multiplataforma con este SDK?

Sí. El SDK funciona en Windows, macOS, Linux, Android e iOS. Para aplicaciones de escritorio multiplataforma, utilice el [framework Avalonia UI](avalonia-player.md) con la misma API `MediaPlayerCoreX`. El motor de reproducción principal es idéntico en todas las plataformas — solo difieren la superficie de renderizado de video y los paquetes NuGet.

## Transcripción del video

### Qué vamos a construir

Vamos a crear un reproductor de video en una aplicación C# WPF con el VisioForge Media Player SDK. Esto es lo que vamos a construir. Una ventana WPF que reproduce un archivo de video: MP4, MKV, AVI, MOV, WebM y muchos formatos más. Tiene Play, Pause, Resume y Stop, una línea de tiempo en la que puede hacer clic o arrastrar para desplazarse, controles deslizantes para el volumen y la velocidad de reproducción, una lista de las salidas de audio y un registro de errores. El código de la ventana ocupa unas ciento ochenta líneas de C#. Este es el reproductor terminado. La película es Big Buck Bunny, de la Blender Foundation, con licencia Creative Commons Attribution.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto usa .NET 8 o posterior. Y un archivo de video para reproducir. La guía escrita también incluye el mismo reproductor como formulario WinForms.

### Crear un proyecto WPF

En Visual Studio, cree un proyecto nuevo. Busque WPF y elija WPF Application para C#. Llame al proyecto VideoPlayerWpf. Elija .NET 10 y haga clic en Create. Visual Studio abre MainWindow.xaml, con el diseñador arriba y el XAML debajo.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. El propio Media Player SDK. CrossPlatform Core, el runtime de GStreamer sobre el que funciona el motor X. Y CrossPlatform Libav, los códecs de FFmpeg con los que se decodifican muchos formatos. O use el NuGet Package Manager: haga clic derecho en el proyecto, Manage NuGet Packages, e instale VisioForge.DotNet.MediaPlayer desde la pestaña Browse. Después instale los paquetes Core y Libav para Windows x64. La pestaña Installed muestra los tres. Son los paquetes x64, así que establezca PlatformTarget en x64 en el archivo del proyecto. Una aplicación de 32 bits usa en su lugar los paquetes x86.

### Construir la ventana en XAML

La ventana se escribe en XAML. Primero la ventana en sí, con una cuadrícula y un VideoView, y el diseñador la dibuja al momento. Asigne el espacio de nombres VisioForge.Core.UI.WPF a un prefijo, aquí vf. La cuadrícula tiene dos columnas, el video y una columna de ajustes de 260 píxeles, y tres filas: el video, la línea de tiempo y los botones. El VideoView ocupa la primera celda. La ventana también nombra sus controladores Loaded y Closing. Debajo del video va la línea de tiempo. Un control deslizante para la posición, con el tiempo a su derecha. IsMoveToPointEnabled hace que un clic en la pista salte directamente a ese punto. El control deslizante nombra dos controladores: ValueChanged, y DragCompleted de su control de arrastre. Después, los botones de reproducción. Play, Pause, Resume y Stop, cada uno con un controlador Click. Todos empiezan deshabilitados: Play hasta que el SDK se ha cargado, y los otros tres hasta que se reproduce un archivo. Por último, la columna de ajustes de la derecha. Contiene el nombre del archivo con un botón para examinar, y un cuadro combinado para la salida de audio. Después, un control deslizante de volumen de 0 a 100, uno de velocidad de 0.5 a 2 en pasos de 0.25, y un cuadro de texto de solo lectura para el registro de errores. Cada controlador que nombra el XAML va en MainWindow.xaml.cs, y los capítulos siguientes los escriben.

### Inicializar el SDK

MainWindow.xaml.cs empieza con estos using: los espacios de nombres de WPF para el control de arrastre y el temporizador, Microsoft.Win32 para el diálogo de archivos, y los espacios de nombres de VisioForge. Cuando la ventana se carga, espere VisioForgeX.InitSDKAsync una vez. Después cree el motor MediaPlayerCoreX y pásele el VideoView, donde se dibuja el video. Los errores llegan en el evento OnError en otro hilo, así que el controlador vuelve al hilo de la interfaz y los añade al registro. Audio_OutputDevicesAsync con DirectSound enumera los altavoces y los auriculares. Sus nombres llenan el cuadro combinado, y se selecciona el primero. Play se habilita solo ahora, una vez cargado el SDK. El botón para examinar muestra un OpenFileDialog filtrado a archivos de video, y pone la ruta elegida en el cuadro del nombre de archivo.

### Abrir y reproducir

El controlador de Play primero deshabilita los botones y limpia el registro. AudioRendererSettings envía el sonido a la salida seleccionada. Audio_OutputDevice_Volume admite de 0 a 1, así que el valor del control deslizante se divide entre cien. OpenAsync recibe la ruta del archivo, o una URL, y devuelve false cuando no se puede abrir el origen. El motivo llega en OnError. Establezca la velocidad después de OpenAsync, porque un archivo recién abierto siempre empieza a velocidad normal. Después PlayAsync inicia la reproducción. Luego compruebe State. PlaybackState.Free significa que el archivo no se abrió o no se reprodujo, o que un archivo muy corto ya ha terminado, y la ventana vuelve al estado detenido. Si no, los botones pasan a reproducción, y se inicia el temporizador de la línea de tiempo.

### Pausar, reanudar y detener

Pause y Resume son una llamada cada uno: PauseAsync y ResumeAsync. Stop detiene el temporizador, deshabilita los botones mientras trabaja, espera StopAsync y restablece la ventana. Dos pequeños métodos auxiliares configuran los botones. ShowBusy deshabilita los cuatro, y ShowPlaying habilita o bien Play o bien los otros tres. Stopped devuelve la ventana a su estado inicial: los botones, la etiqueta de tiempo y la línea de tiempo a cero. El indicador alrededor de esa última línea le dice al controlador de la línea de tiempo que este movimiento no es del usuario. OnStop se dispara después de StopAsync, y cuando el archivo llega al final. El controlador actúa solo mientras Stop está habilitado y State es Free, es decir, al final del archivo. Un OnStop tardío del archivo anterior se ignora, porque el siguiente ya se está reproduciendo.

### La línea de tiempo

La línea de tiempo sigue un DispatcherTimer que se activa dos veces por segundo en el hilo de la interfaz. Su controlador Tick se conecta en Window_Loaded. Cada activación lee Position_GetAsync y DurationAsync. Mientras el usuario sujeta el control deslizante, la activación no lo toca. Si no, actualiza la etiqueta de tiempo y mueve el control deslizante, con el indicador activado, para que este movimiento no desplace la reproducción. Un clic en la pista se desplaza al instante, en ValueChanged. Un arrastre se desplaza una vez, al soltar el control, en DragCompleted. Pulsar el control sin moverlo no desplaza la reproducción. SeekAsync llama a Position_SetAsync mientras hay un archivo abierto. También funciona en pausa.

### Volumen y velocidad

El volumen sigue a su control deslizante al instante, mediante Audio_OutputDevice_Volume. Rate_SetAsync cambia la velocidad durante la reproducción: 1 es la velocidad normal, 2 es el doble, y 0.5 es la mitad. Cuando no se reproduce nada, Play aplica el valor del control deslizante.

### Cerrar la ventana

Cuando la ventana se cierra, Dispose detiene la reproducción y libera el reproductor, y DestroySDK libera el SDK. Un cierre mientras el SDK se carga, un archivo se abre o la reproducción se detiene se cancela, para que el reproductor nunca se libere en mitad de una llamada.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Una vez cargado el SDK, las salidas de audio llenan la lista y Play se habilita. Haga clic en el botón para examinar y elija el archivo de video. Haga clic en Play. La etiqueta de tiempo muestra la posición y la duración del archivo. Un clic en la línea de tiempo salta directamente a ese punto. O arrastre el control: la reproducción salta una vez, al soltarlo. El volumen sigue a su control deslizante, y a una vez y media la velocidad la película va más rápido. Pause congela la imagen, y Resume continúa desde el mismo punto. Stop restablece la ventana. Después ciérrela.

### Errores frecuentes

Algunos errores frecuentes. Omitir el paquete Libav, de modo que muchos formatos no se decodifican. Mezclar paquetes x86 y x64, o ejecutar una aplicación x86 con los de x64. Establecer la velocidad antes de OpenAsync: el archivo nuevo vuelve a empezar a velocidad normal. Dejar que los propios movimientos del control deslizante por el temporizador desplacen la reproducción, lo que la hace entrecortada. Y los errores llegan en el evento OnError, en otro hilo.

### Próximos pasos

También hay guías para reproducir un archivo en bucle o solo un fragmento, para el mismo reproductor en VB.NET, y para reproductores multiplataforma con Avalonia, .NET MAUI y Android. El código fuente completo está en GitHub, y la documentación en visioforge.com/help. Gracias por ver el video.

## Ver También

- [Crear un Reproductor de Video en VB.NET](video-player-vb-net.md) — el mismo tutorial usando sintaxis VB.NET
- [Reproductor Multiplataforma con Avalonia](avalonia-player.md) — construya un reproductor de video para Windows, macOS y Linux con Avalonia UI
- [Guía del reproductor .NET MAUI](maui-player.md) — una base de código C# para iOS, Android, macOS y Windows
- [Guía del reproductor Android](android-player.md) — configuración y despliegue específicos de Android
- [Modo Bucle y Rango de Posición](loop-and-position-range.md) — configure el bucle, repetición A-B y reproducción de segmentos
- [Reproductor Media Blocks SDK](../../mediablocks/GettingStarted/player.md) — alternativa basada en pipeline con renderizadores explícitos
- [Página del Producto Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net)
