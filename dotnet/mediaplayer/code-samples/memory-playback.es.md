---
title: Reproducir video desde un stream o array de bytes en C#
description: Reproduzca un video desde un Stream .NET o un array de bytes en C# WPF con Media Player SDK .NET — MemoryStreamSource, ManagedIStream y Memory_DS.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCore
  - Windows
  - DirectShow
  - Playback
  - WPF
  - C#
  - NuGet
primary_api_classes:
  - MemoryStreamSource
  - MediaPlayerSourceMode
  - ManagedIStream
  - MediaPlayerCore
video:
  id: zSu_50b7IVQ
  upload_date: 2026-11-12T09:06:24Z
  duration: PT11M32S

---

# Reproducir un video desde memoria en C# WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

Este tutorial crea un reproductor WPF que reproduce un video desde un `Stream` .NET en lugar de un nombre de archivo, con [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) y su motor `MediaPlayerCore`. La aplicación lee el archivo de una de dos formas: como un `FileStream`, o como un array de bytes leído entero en memoria y envuelto en un `MemoryStream`. El mismo código reproduce cualquier stream legible y con búsqueda: una descarga copiada en memoria, un blob de una base de datos o un archivo descifrado en memoria.

`MediaPlayerCore` es el motor DirectShow del SDK, por lo que la aplicación funciona solo en Windows.

!!! tip "Agentes de IA: use el servidor MCP de VisioForge"

    ¿Lo construye con **Claude Code**, **Cursor** u otro agente de IA?
    Conéctese al [servidor MCP público de VisioForge](../../general/mcp-server-usage.md)
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Tutorial de YouTube

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/zSu_50b7IVQ?controls=1" title="Reproducir un video desde memoria en C# WPF - tutorial de YouTube" frameborder="0" allowfullscreen></iframe>
</div>

[Código fuente en GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf)

La película del video es [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, con licencia [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

El video crea el proyecto con .NET 10, que requiere Visual Studio 2026; en Visual Studio 2022, elija .NET 8 o .NET 9.

## Paquetes NuGet requeridos

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) — el SDK con `MediaPlayerCore` y el `VideoView` de WPF
- [VisioForge.DotNet.Core.Redist.MediaPlayer.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/) — los filtros DirectShow del SDK, entre ellos la fuente que lee el stream
- [VisioForge.DotNet.Core.Redist.LAV.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/) — el divisor y los decodificadores LAV, que encuentran las pistas de video y audio en el stream y las decodifican

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.MediaPlayer.x64" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="[2026.11.12]" />
```

Son los paquetes x64, por lo que el proyecto establece `<PlatformTarget>x64</PlatformTarget>`; una aplicación de 32 bits usa los paquetes `.x86`. Consulte la [guía de despliegue](../deployment.md) para ver la lista completa.

## La ventana en XAML

El prefijo `vf` asigna el espacio de nombres `VisioForge.Core.UI.WPF` del ensamblado `VisioForge.Core`. El `VideoView` ocupa la columna izquierda, con la línea de tiempo y los botones de reproducción debajo; la columna derecha contiene el nombre del archivo, dos botones de opción que eligen cómo se lee el archivo y el registro de errores. Play está habilitado desde el principio: `MediaPlayerCore` no necesita inicializar el SDK.

```xml
<Window x:Class="MemoryPlaybackWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Memory Playback" Height="600" Width="1000"
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
            <Button x:Name="btPlay" Content="Play" Width="90" Click="btPlay_Click" />
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

            <Label Content="Read the file as" Margin="0,10,0,0" />
            <RadioButton x:Name="rbStream" Content="Stream (FileStream)" IsChecked="True" />
            <RadioButton x:Name="rbByteArray" Content="Byte array (MemoryStream)" Margin="0,5,0,0" />

            <Label Content="Log" Margin="0,10,0,0" />
            <TextBox x:Name="edLog" Height="150" IsReadOnly="True" TextWrapping="Wrap"
                     VerticalScrollBarVisibility="Auto" />
        </StackPanel>
    </Grid>
</Window>
```

## El código de la ventana

`MainWindow.xaml.cs`. El proyecto tiene `ImplicitUsings` habilitado, por lo que `System`, `System.Threading.Tasks` y los demás espacios de nombres comunes no aparecen en la lista.

```cs
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Microsoft.Win32;
using VisioForge.Core;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;
using VisioForge.Core.Types.MediaPlayer;

namespace MemoryPlaybackWpf;

public partial class MainWindow : Window
{
    private MediaPlayerCore _player;

    private Stream _source; // el reproductor lo lee mientras reproduce: se cierra solo después de detenerse

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };

    private bool _timerMovesSlider; // true mientras el temporizador, no el usuario, mueve la línea de tiempo

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _player = new MediaPlayerCore(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Lo generan StopAsync y el final del archivo; solo se atiende el final del archivo
        // (Stop habilitado), no una detención tardía del archivo anterior (State ya no es Free)
        _player.OnStop += (_, _) => Dispatcher.InvokeAsync(() =>
        {
            if (btStop.IsEnabled && _player.State() == PlaybackState.Free) Stopped();
        });

        _timer.Tick += Timer_Tick;
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

        try
        {
            _source = rbByteArray.IsChecked == true
                ? new MemoryStream(await File.ReadAllBytesAsync(edFilename.Text)) // el archivo completo en memoria
                : new FileStream(edFilename.Text, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            edLog.AppendText(ex.Message + Environment.NewLine);
            ShowPlaying(false);
            return;
        }

        // Un stream .NET con búsqueda, envuelto como el IStream COM que lee el reproductor; el
        // divisor LAV encuentra en él el video y el audio
        _player.Source_MemoryStream = new MemoryStreamSource(new ManagedIStream(_source), _source.Length);
        _player.Source_Mode = MediaPlayerSourceMode.Memory_DS;

        if (!await _player.PlayAsync() || _player.State() == PlaybackState.Free)
        {
            Stopped(); // no se reprodujo (el motivo está en el registro) o ya terminó
            return;
        }

        slTimeline.Maximum = (await _player.Duration_TimeAsync()).TotalSeconds;
        ShowPlaying(true);
        _timer.Start();
    }

    private async void btPause_Click(object sender, RoutedEventArgs e)
    {
        btPause.IsEnabled = false;
        var paused = await _player.PauseAsync();
        if (!btStop.IsEnabled) return; // detenido mientras tanto
        btResume.IsEnabled = paused;
        btPause.IsEnabled = !paused;
    }

    private async void btResume_Click(object sender, RoutedEventArgs e)
    {
        btResume.IsEnabled = false;
        var resumed = await _player.ResumeAsync();
        if (!btStop.IsEnabled) return; // detenido mientras tanto
        btPause.IsEnabled = resumed;
        btResume.IsEnabled = !resumed;
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
        btPause.IsEnabled = btStop.IsEnabled = playing;
        btResume.IsEnabled = false;
    }

    private void Stopped()
    {
        _timer.Stop();
        ShowPlaying(false);

        _source?.Dispose(); // el reproductor ya lo soltó
        _source = null;

        lbTime.Text = "00:00:00 / 00:00:00";
        _timerMovesSlider = true;
        slTimeline.Value = 0;
        _timerMovesSlider = false;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_Get_TimeAsync();
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // detenido, o el usuario arrastra

        var duration = TimeSpan.FromSeconds(slTimeline.Maximum); // leída una vez, después de PlayAsync
        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
        slTimeline.Value = position.TotalSeconds;
        _timerMovesSlider = false;
    }

    // Un clic en la pista busca al instante; un arrastre busca una vez, al terminar
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
        if (btStop.IsEnabled) await _player.Position_Set_TimeAsync(TimeSpan.FromSeconds(slTimeline.Value));
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _timer.Stop();
        _player.Dispose(); // detiene la reproducción; luego Stopped() cierra el stream, o lo hace la salida de la aplicación
    }
}
```

## Cómo funciona

1. **La fuente** — con *Byte array* marcado, `File.ReadAllBytesAsync` lee el archivo completo en memoria y un `MemoryStream` envuelve el array; si no, un `FileStream` abierto con `FileAccess.Read` lee el archivo a medida que el reproductor lo pide. Un archivo inexistente o bloqueado va al registro y la ventana sigue detenida.
2. **Reproducción desde memoria** — `ManagedIStream` (espacio de nombres `VisioForge.Core`) envuelve el stream .NET como el `IStream` COM que lee el motor DirectShow. `MemoryStreamSource` lo recibe junto con la longitud del stream, y `Source_Mode = MediaPlayerSourceMode.Memory_DS` hace que el reproductor lea de él. Con el paquete LAV instalado, el divisor LAV encuentra las pistas de video y audio en el stream, así que basta con el constructor de dos argumentos.
3. **Reproducir** — `PlayAsync` devuelve `false` cuando el stream no se puede reproducir, y el motivo llega por `OnError`. Si `State()` devuelve `PlaybackState.Free`, no se reprodujo o un archivo muy corto ya terminó. En caso contrario, `Duration_TimeAsync` se lee una vez, para la escala de la línea de tiempo.
4. **La vida del stream** — el reproductor lee el stream y busca en él mientras reproduce, por eso `Stopped()` es el único lugar que lo libera: después de `StopAsync`, o cuando `OnStop` informa del final del archivo. Un bloque `using` alrededor de `PlayAsync` cerraría el stream mientras se reproduce el video.
5. **Línea de tiempo** — un `DispatcherTimer` lee `Position_Get_TimeAsync` dos veces por segundo en el hilo de la interfaz. La marca `_timerMovesSlider` distingue los movimientos del temporizador de los del usuario, así el temporizador nunca busca. Un clic en la pista busca al instante; un arrastre busca una vez, en `Thumb.DragCompleted`. `Position_Set_TimeAsync` busca dentro del stream.
6. **Detener y cerrar** — `OnStop` se dispara después de `StopAsync` y al final del archivo, en otro hilo, por lo que el controlador vuelve al hilo de la interfaz con `Dispatcher.InvokeAsync`, igual que `OnError`. Solo actúa mientras Stop está habilitado y `State()` es `PlaybackState.Free`. Al cerrar la ventana se llama a `Dispose`, que detiene la reproducción y libera el reproductor.

## Notas

- El stream debe ser legible y admitir búsqueda, y su `Length` debe conocerse: el reproductor busca en él para encontrar las pistas y seguir la línea de tiempo. Un stream de red o de descompresión que no admite búsqueda se copia antes en un `MemoryStream`.
- Un array de bytes mantiene todo el archivo en memoria. Para un archivo grande, un `FileStream` lee solo lo que el reproductor necesita.
- Sin el paquete LAV el reproductor no puede saber qué contiene el stream: use el constructor `MemoryStreamSource(IStream stream, bool? videoPresent, bool? audioPresent, long size)` y establezca ambas marcas según lo que el stream contiene realmente.
- `ManagedIStream` lee a través de cualquier `Stream`, así que una clase de stream propia — por ejemplo, un stream que descifra — se reproduce igual, siempre que sea legible y admita búsqueda.

## Aplicaciones de ejemplo

- [Tutorial de reproducción desde memoria WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf) — el código de esta página y del video
- [Demo Memory Stream WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Memory%20Stream)
- [Demo Encrypted Memory Playback WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Encrypted%20Memory%20Playback%20Demo)

## Preguntas frecuentes

### ¿Puedo reproducir un video desde un array de bytes en C#?

Sí. Envuelva el array en un `MemoryStream`, envuelva este en `ManagedIStream` y páselo a `MemoryStreamSource` con la longitud del array. Establezca `Source_Mode` en `MediaPlayerSourceMode.Memory_DS` y llame a `PlayAsync`. Mantenga el `MemoryStream` abierto hasta que la reproducción se haya detenido.

### ¿Por qué la reproducción se detiene o falla justo después de empezar?

Normalmente el stream se libera demasiado pronto, por ejemplo con un bloque `using` alrededor de `PlayAsync`, o el stream no admite búsqueda. Guarde el stream en un campo y libérelo después de `StopAsync` o al final del archivo.

### ¿Puedo reproducir un stream descargado de la red?

Cópielo antes en un `MemoryStream`. Un stream de red no admite búsqueda ni tiene longitud, y el reproductor necesita ambas cosas.

### ¿Qué formatos se reproducen desde memoria?

Los que el divisor y los decodificadores LAV leen de un archivo: MP4, MKV, AVI, MOV, WMV, WebM, MPEG-TS y otros. El stream contiene los mismos bytes que el archivo.

## Transcripción del video

### Qué vamos a construir

Vamos a reproducir un video desde la memoria en una aplicación C# WPF, con el VisioForge Media Player SDK. Esto es lo que vamos a construir. Un reproductor WPF que lee un stream de .NET en lugar de un nombre de archivo. Ya sea un FileStream, o el archivo completo leído en un array de bytes y envuelto en un MemoryStream. Tiene Play, Pause, Resume y Stop, y una línea de tiempo para desplazarse. El mismo código reproduce un stream que haya obtenido de cualquier otro sitio: una descarga, una base de datos o un archivo que haya descifrado en memoria. Este es el reproductor terminado. La película es Big Buck Bunny, de la Blender Foundation, con licencia Creative Commons Attribution.

### Requisitos previos

Necesita Windows 10 u 11, y Visual Studio 2022 o 2026 con la carga de trabajo de desarrollo de escritorio de .NET. El proyecto es una aplicación WPF en .NET 10. Y un archivo de video para reproducir. El reproductor aquí es MediaPlayerCore, el motor DirectShow del SDK, así que esta aplicación funciona en Windows.

### Crear un proyecto WPF

En Visual Studio, cree un proyecto nuevo. Busque WPF y elija WPF Application para C#. Llame al proyecto MemoryPlaybackWpf. Elija .NET 10 y haga clic en Create.

### Añadir los paquetes NuGet

Ahora añada tres paquetes NuGet. VisioForge.DotNet.Core contiene MediaPlayerCore. Redist MediaPlayer contiene los filtros DirectShow del SDK, entre ellos la fuente que lee el stream. Y Redist LAV contiene el divisor y los decodificadores LAV, que encuentran el video y el audio en el stream y los decodifican. O use el administrador de paquetes NuGet: haga clic derecho en el proyecto, Manage NuGet Packages, e instale VisioForge.DotNet.Core desde la pestaña Browse. Después instale los redists MediaPlayer y LAV para x64. La pestaña Installed muestra los tres. Estos son los paquetes x64, así que establezca PlatformTarget en x64 en el archivo del proyecto. Una aplicación de 32 bits usa en su lugar los paquetes x86.

### Construir la ventana en XAML

La ventana se escribe en XAML. Primero la propia ventana, con un grid y un VideoView, y el diseñador la dibuja de inmediato. Asigne el espacio de nombres VisioForge.Core.UI.WPF a un prefijo, aquí vf. El grid tiene dos columnas, el video y una columna de ajustes de 260 píxeles, y tres filas: el video, la línea de tiempo y los botones. El VideoView ocupa la primera celda. La ventana también nombra sus controladores Loaded y Closing. Debajo del video va la línea de tiempo. Un control deslizante para la posición, con el tiempo a su derecha. IsMoveToPointEnabled hace que un clic en la pista salte directamente a ese punto. El control deslizante nombra dos controladores: ValueChanged, y DragCompleted de su thumb. Luego los botones de reproducción. Play, Pause, Resume y Stop, cada uno con un controlador Click. Play está habilitado desde el principio, porque este motor no necesita inicializar el SDK. Los otros tres empiezan deshabilitados, hasta que se reproduce un archivo. Por último, la columna de ajustes a la derecha. Contiene el nombre del archivo con un botón para examinar, y dos botones de opción que eligen cómo lee la aplicación el archivo: como un FileStream, o como un array de bytes en un MemoryStream. Stream está marcado al principio. Luego un cuadro de texto de solo lectura para el registro de errores. Cada controlador que nombra el XAML va en MainWindow.xaml.cs, y los siguientes capítulos los escriben.

### Crear el reproductor

MainWindow.xaml.cs empieza con estos using: System.IO para los streams, espacios de nombres de WPF para el thumb del control deslizante y el temporizador, Microsoft.Win32 para el diálogo de archivos, y los espacios de nombres de VisioForge. La ventana guarda el reproductor, el stream que reproduce, un temporizador para la línea de tiempo y un indicador. Cuando se carga la ventana, cree MediaPlayerCore y pásele el VideoView, donde se dibuja el video. Los errores llegan en el evento OnError en otro hilo, así que el controlador vuelve al hilo de la interfaz y los añade al registro. OnStop se dispara después de StopAsync, y cuando el archivo llega a su fin. El controlador solo actúa mientras Stop está habilitado y State es Free, que es el fin del archivo. Después Window_Loaded conecta el controlador Tick del temporizador. El botón para examinar muestra un OpenFileDialog filtrado a archivos de video, y pone la ruta elegida en el cuadro del nombre de archivo.

### Reproducir desde la memoria

El controlador de Play primero deshabilita los botones y borra el registro. Luego abre la fuente. Con Byte array marcado, ReadAllBytesAsync lee el archivo completo en memoria, y un MemoryStream envuelve el array. Si no, un FileStream lee el archivo a medida que el reproductor lo necesita. Un archivo que falta o está bloqueado va al registro, y la ventana se queda detenida. Esta es la parte que reproduce desde la memoria. ManagedIStream envuelve el stream de .NET como el IStream de COM que lee el motor DirectShow. MemoryStreamSource lo recibe con la longitud del stream, y Source_Mode establecido en Memory_DS hace que el reproductor lea de él. El stream debe admitir lectura y desplazamiento. El divisor LAV encuentra en él las pistas de video y audio, así que no tiene que decirle al reproductor qué contiene el stream. PlayAsync devuelve false cuando el stream no se puede reproducir, y el motivo llega en OnError. State Free significa que no se reprodujo, o que un archivo muy corto ya ha terminado, y la ventana vuelve a quedar detenida. Si no, lea la duración una vez, para la escala de la línea de tiempo, cambie los botones a reproducción e inicie el temporizador.

### Pausar, reanudar y detener

Pause se deshabilita a sí mismo, espera a PauseAsync y habilita Resume cuando el reproductor se ha pausado. Si la reproducción se detuvo mientras tanto, deja los botones como están. Resume es igual, con ResumeAsync. Stop detiene el temporizador, deshabilita los botones mientras trabaja, espera a StopAsync y restablece la ventana. Dos pequeños métodos auxiliares configuran los botones. ShowBusy deshabilita los cuatro. ShowPlaying habilita o bien Play, o bien Pause y Stop. Stopped restablece la ventana, y es el único lugar que cierra el stream. El reproductor lee el stream mientras reproduce, así que libérelo solo cuando el reproductor se haya detenido, nunca justo después de PlayAsync. Luego la etiqueta de tiempo y la línea de tiempo vuelven a cero.

### La línea de tiempo

El temporizador se dispara dos veces por segundo en el hilo de la interfaz. Cada tick lee Position_Get_TimeAsync. Mientras el usuario sujeta el control deslizante, el tick no lo toca. Si no, actualiza la etiqueta de tiempo y mueve el control deslizante, con el indicador activado, para que este movimiento no provoque un desplazamiento. Un clic en la pista se desplaza de inmediato, en ValueChanged. Un arrastre se desplaza una sola vez, al soltar el thumb, en DragCompleted. Ambos llaman a Position_Set_TimeAsync mientras se reproduce un archivo. El desplazamiento también funciona con un stream en memoria, porque el stream lo admite.

### Cerrar la ventana

Cuando se cierra la ventana, Dispose detiene la reproducción y libera el reproductor. El stream lo cierra Stopped, o se cierra con la aplicación.

### Ejecutarla

Ejecute la aplicación desde Visual Studio. Haga clic en el botón para examinar y elija el archivo de video. Stream está marcado, así que Play lee el archivo mediante un FileStream. La etiqueta de tiempo muestra la posición y la duración. Un clic en la línea de tiempo se desplaza dentro del stream. Pause congela la imagen, Resume continúa desde el mismo punto, y Stop restablece la ventana. Ahora marque Byte array y haga clic en Play. La aplicación primero lee el archivo completo en memoria, y el reproductor lo reproduce desde el MemoryStream, sin acceder al archivo mientras reproduce. Detenga la reproducción y cierre la ventana.

### Errores frecuentes

Algunos errores frecuentes. Liberar el stream mientras el reproductor aún lo lee, por ejemplo con un bloque using alrededor de PlayAsync: ciérrelo solo después de detener. Un stream que no admite desplazamiento, como un stream de red: el reproductor necesita desplazarse y conocer la longitud, así que cópielo primero en un MemoryStream. Omitir el paquete LAV: sin él, el reproductor no sabe qué contiene el stream. Un array de bytes para un archivo enorme, que ocupa esa misma memoria, así que ahí es mejor un FileStream. Y mezclar los paquetes x86 y x64.

### Próximos pasos

También hay guías sobre un reproductor de video completo con volumen y velocidad, sobre cómo reproducir solo un fragmento de un archivo, cómo mostrar el primer fotograma de un video y cómo distribuir los redists con su aplicación. El código fuente completo está en GitHub, y la documentación está en visioforge.com/help. Gracias por ver el video.

## Ver también

- [Crear un reproductor de video en C#](../guides/video-player-csharp.md) — un reproductor completo con volumen, velocidad y salida de audio
- [Reproducir un fragmento de un archivo](play-fragment-file.md) — reproducir solo un rango de un archivo multimedia
- [Mostrar el primer fotograma de un video](show-first-frame.md) — mostrar el primer fotograma de un video como vista previa
- [Guía de despliegue](../deployment.md) — los paquetes NuGet e instaladores que necesita la aplicación
- [Página del producto Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net)
