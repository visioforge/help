---
title: Reproductor de video multiplataforma con Avalonia y MAUI
description: Cree reproductores de video multiplataforma con Avalonia y .NET MAUI usando Media Blocks SDK para Windows, macOS, Linux, Android e iOS.
---

# Reproductor de Video Multiplataforma: Guía de Avalonia y MAUI

Cree reproductores de video que funcionen en Windows, macOS, Linux, Android e iOS utilizando [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net). Tanto Avalonia como .NET MAUI utilizan la API `MediaBlocksPipeline`, que proporciona una arquitectura flexible basada en bloques para la reproducción multimedia multiplataforma.

> **¿Busca WinForms o WPF?** Consulte [Crear un Reproductor de Video en C#](video-player-csharp.md) o [Crear un Reproductor de Video en VB.NET](video-player-vb-net.md) para implementaciones exclusivas de Windows que utilizan `MediaPlayerCoreX`.

## Elegir el Enfoque Correcto

| Framework | Plataforma | SDK | API |
|-----------|------------|-----|-----|
| WinForms | Windows | [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) | `MediaPlayerCoreX` |
| WPF | Windows | [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) | `MediaPlayerCoreX` |
| Avalonia | Windows, macOS, Linux | [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) | `MediaBlocksPipeline` |
| MAUI | Windows, macOS, Android, iOS | [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) | `MediaBlocksPipeline` |

## Reproductor de Video con Avalonia

Avalonia permite la reproducción de video en Windows, macOS y Linux desde una única base de código utilizando `MediaBlocksPipeline`.

### Paquetes NuGet

Paquete principal del SDK:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.2.19" />
```

Paquetes redist específicos de plataforma (agregue los correspondientes a sus plataformas objetivo):

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

### Diseño AXAML

Agregue el control `VideoView` a su vista de Avalonia:

```xml
<UserControl xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia">
    <Grid RowDefinitions="*,Auto">
        <avalonia:VideoView x:Name="videoView1" Background="#0C0C0C" />

        <StackPanel Grid.Row="1" Orientation="Vertical">
            <Slider Name="slSeeking" Maximum="{Binding SeekingMaximum}"
                    Value="{Binding SeekingValue}" />
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                <Button Command="{Binding OpenFileCommand}" Content="ABRIR ARCHIVO" />
                <Button Command="{Binding PlayPauseCommand}" Content="{Binding PlayPauseText}" />
                <Button Command="{Binding StopCommand}" Content="DETENER" />
            </StackPanel>
        </StackPanel>
    </Grid>
</UserControl>
```

### Configuración del Pipeline y Reproducción

Cree el pipeline, conecte la fuente a los renderizadores de video y audio, e inicie la reproducción:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

private MediaBlocksPipeline _pipeline;

private async Task CreateEngineAsync(string filePath, IVideoView videoView)
{
    if (_pipeline != null)
    {
        await _pipeline.StopAsync();
        await _pipeline.DisposeAsync();
    }

    _pipeline = new MediaBlocksPipeline();

    // Crear fuente de archivo
    var sourceSettings = await UniversalSourceSettings.CreateAsync(
        new Uri(filePath));
    var source = new UniversalSourceBlock(sourceSettings);

    // Conectar renderizador de video
    var videoRenderer = new VideoRendererBlock(_pipeline, videoView);
    _pipeline.Connect(source.VideoOutput, videoRenderer.Input);

    // Conectar renderizador de audio
    var audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(source.AudioOutput, audioRenderer.Input);

    await _pipeline.StartAsync();
}
```

### Patrón MVVM con ReactiveUI

El enfoque recomendado para Avalonia utiliza MVVM con ReactiveUI. El ViewModel gestiona el ciclo de vida del pipeline, el estado de reproducción y los enlaces de la interfaz de usuario:

```csharp
using ReactiveUI;

public class MainViewModel : ReactiveObject
{
    private MediaBlocksPipeline _pipeline;
    private System.Timers.Timer _tmPosition = new(1000);

    public IVideoView VideoViewIntf { get; set; }

    private string? _playPauseText = "REPRODUCIR";
    public string? PlayPauseText
    {
        get => _playPauseText;
        set => this.RaiseAndSetIfChanged(ref _playPauseText, value);
    }

    public ReactiveCommand<Unit, Unit> PlayPauseCommand { get; }
    public ReactiveCommand<Unit, Unit> StopCommand { get; }

    public MainViewModel()
    {
        PlayPauseCommand = ReactiveCommand.CreateFromTask(PlayPauseAsync);
        StopCommand = ReactiveCommand.CreateFromTask(StopAsync);

        _tmPosition.Elapsed += OnPositionTimerElapsed;

        VisioForgeX.InitSDK();
    }

    private async Task PlayPauseAsync()
    {
        if (_pipeline == null || _pipeline.State == PlaybackState.Free)
        {
            await CreateEngineAsync();
            await _pipeline.StartAsync();
            _tmPosition.Start();
            PlayPauseText = "PAUSA";
        }
        else if (_pipeline.State == PlaybackState.Play)
        {
            await _pipeline.PauseAsync();
            PlayPauseText = "REPRODUCIR";
        }
        else if (_pipeline.State == PlaybackState.Pause)
        {
            await _pipeline.ResumeAsync();
            PlayPauseText = "PAUSA";
        }
    }
}
```

Para una implementación completa del reproductor Avalonia que incluye diálogos de archivos, búsqueda, control de volumen, configuración específica de plataforma para Android/iOS y la estructura completa del proyecto, consulte la [Guía del Reproductor Avalonia](avalonia-player.md).

## Reproductor de Video con MAUI

.NET MAUI permite la reproducción de video en Windows, macOS, Android e iOS utilizando `MediaBlocksPipeline`.

### Paquetes NuGet Requeridos

Paquete principal del SDK:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.19" />
```

Paquetes redist específicos de plataforma:

```xml
<!-- Windows x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />

<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />

<!-- Android -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33" />

<!-- iOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16" />
```

### Configuración de la Aplicación MAUI

Registre los controladores de VisioForge en `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder.UseMauiApp<App>()
        .UseSkiaSharp()
        .AddVisioForgeHandlers();

    return builder.Build();
}
```

### Diseño XAML

Agregue el control `VideoView` a su página MAUI:

```xml
<ContentPage xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI">
    <Grid RowDefinitions="*,Auto">
        <my:VideoView Grid.Row="0" x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand"
                      Background="Black" />

        <StackLayout Grid.Row="1" Orientation="Vertical">
            <Slider x:Name="slSeeking"
                    ValueChanged="slSeeking_ValueChanged" />
            <StackLayout Orientation="Horizontal" HorizontalOptions="Center">
                <Button x:Name="btOpen" Text="ABRIR ARCHIVO"
                        Clicked="btOpen_Clicked" />
                <Button x:Name="btPlayPause" Text="REPRODUCIR"
                        Clicked="btPlayPause_Clicked" />
                <Button x:Name="btStop" Text="DETENER"
                        Clicked="btStop_Clicked" />
            </StackLayout>
        </StackLayout>
    </Grid>
</ContentPage>
```

### Configuración del Pipeline

Construya el pipeline con los bloques de fuente, renderizador de video y renderizador de audio:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.Sources;

private MediaBlocksPipeline _pipeline;
private UniversalSourceBlock _source;
private AudioRendererBlock _audioRenderer;

private async Task CreateEngineAsync()
{
    if (_pipeline != null)
    {
        await _pipeline.StopAsync();
        await _pipeline.DisposeAsync();
    }

    _pipeline = new MediaBlocksPipeline();

    // Crear fuente desde archivo
    _source = new UniversalSourceBlock(
        await UniversalSourceSettings.CreateAsync(new Uri(_filename)));

    // Conectar renderizador de video
    var videoRenderer = new VideoRendererBlock(
        _pipeline, videoView.GetVideoView());
    _pipeline.Connect(_source.VideoOutput, videoRenderer.Input);

    // Conectar renderizador de audio
    _audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
}
```

### Controles de Reproducción

Gestione reproducir/pausar, detener y selección de archivos:

```csharp
private async void btOpen_Clicked(object sender, EventArgs e)
{
    var result = await FilePicker.Default.PickAsync();
    if (result != null)
    {
        _filename = result.FullPath;
    }
}

private async void btPlayPause_Clicked(object sender, EventArgs e)
{
    if (_pipeline == null || _pipeline.State == PlaybackState.Free)
    {
        await CreateEngineAsync();
        await _pipeline.StartAsync();
        btPlayPause.Text = "PAUSA";
    }
    else if (_pipeline.State == PlaybackState.Play)
    {
        await _pipeline.PauseAsync();
        btPlayPause.Text = "REPRODUCIR";
    }
    else if (_pipeline.State == PlaybackState.Pause)
    {
        await _pipeline.ResumeAsync();
        btPlayPause.Text = "PAUSA";
    }
}

private async void btStop_Clicked(object sender, EventArgs e)
{
    if (_pipeline != null)
    {
        await _pipeline.StopAsync();
    }
}
```

### Búsqueda y Control de Volumen

```csharp
// Búsqueda (controlador de cambio de valor del slider)
private async void slSeeking_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (!_isTimerUpdate && _pipeline != null)
    {
        await _pipeline.Position_SetAsync(
            TimeSpan.FromMilliseconds(e.NewValue));
    }
}

// Control de volumen (escala de 0-100 a 0.0-1.0)
private void slVolume_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (_audioRenderer != null)
    {
        _audioRenderer.Volume = e.NewValue / 100.0;
    }
}
```

### Limpieza

Libere los recursos del pipeline cuando se cierre la ventana:

```csharp
private async void Window_Destroying(object sender, EventArgs e)
{
    if (_pipeline != null)
    {
        await _pipeline.StopAsync();
        _pipeline.Dispose();
        _pipeline = null;
    }

    VisioForgeX.DestroySDK();
}
```

## Referencia de Paquetes NuGet Multiplataforma

| Plataforma | Paquete | Versión |
|------------|---------|---------|
| Todos | `VisioForge.DotNet.MediaBlocks` | 2026.2.19 |
| Avalonia UI | `VisioForge.DotNet.Core.UI.Avalonia` | 2026.2.19 |
| Windows x64 | `VisioForge.CrossPlatform.Core.Windows.x64` | 2025.11.0 |
| Windows x64 | `VisioForge.CrossPlatform.Libav.Windows.x64` | 2025.11.0 |
| macOS | `VisioForge.CrossPlatform.Core.macOS` | 2025.9.1 |
| macOS (MAUI) | `VisioForge.CrossPlatform.Core.macCatalyst` | 2025.9.1 |
| Linux x64 | `VisioForge.CrossPlatform.Core.Linux.x64` | 2025.11.0 |
| Linux x64 | `VisioForge.CrossPlatform.Libav.Linux.x64` | 2025.11.0 |
| Android | `VisioForge.CrossPlatform.Core.Android` | 15.10.33 |
| iOS | `VisioForge.CrossPlatform.Core.iOS` | 2025.0.16 |

## Formatos Compatibles

- **Video:** MP4, AVI, MKV, WMV, WebM, MOV, TS, FLV
- **Audio:** MP3, AAC, WAV, WMA, FLAC, OGG
- **Códecs:** H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2
- **Streaming:** RTSP, HTTP, HLS, MPEG-DASH

## Aplicaciones de Ejemplo

| Plataforma | Demo |
|------------|------|
| Avalonia MVVM | [SimplePlayerMVVM](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayerMVVM) |
| Avalonia Simple | [SimplePlayer](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayer) |
| MAUI | [SimplePlayer](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimplePlayer) |

## Recursos Relacionados

- [Crear un Reproductor de Video en C#](video-player-csharp.md) (WinForms/WPF)
- [Crear un Reproductor de Video en VB.NET](video-player-vb-net.md) (WinForms)
- [Guía del Reproductor Avalonia](avalonia-player.md) (implementación MVVM completa)
- [Guía del Reproductor Android](android-player.md)
- [Página del Producto Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net)
