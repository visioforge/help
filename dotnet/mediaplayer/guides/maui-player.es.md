---
title: Reproductor de Video .NET MAUI en C# — iOS, Android, Windows
description: Cree un reproductor de video .NET MAUI en C# que funciona en iOS, Android, macOS y Windows desde una sola base de código con el control VideoView.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Android
  - iOS
  - MAUI
  - Playback
  - Streaming
  - MPEG-DASH
  - C#
  - NuGet
  - Entitlements
primary_api_classes:
  - VideoView
  - MediaPlayerCoreX
  - UniversalSourceSettings
  - IVideoView
  - ValueChangedEventArgs

---

# Crear un Reproductor de Video .NET MAUI en C#

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

.NET MAUI permite distribuir una sola base de código C# a **iOS, Android, macOS y Windows**. Esta guía conecta el control `VideoView` de VisioForge con `MediaPlayerCoreX` para reproducir archivos locales y streams de red — un arranque enfocado que refleja el [demo SimplePlayer MAUI](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer).

> **¿Eligiendo framework?**
> Avalonia (escritorio, incluye Linux) → [Guía del reproductor Avalonia](avalonia-player.md).
> WinForms / WPF (escritorio Windows) → [Crear un reproductor de video en C#](video-player-csharp.md).
> Solo Android (sin MAUI) → [Guía del reproductor Android](android-player.md).

!!! tip "Agentes de IA: usa el servidor MCP de VisioForge"

    ¿Lo construyes con **Claude Code**, **Cursor** u otro agente de IA?
    Conecta al servidor MCP público de VisioForge
    ([documentación](../../general/mcp-server-usage.md))
    en `https://mcp.visioforge.com/mcp` para consultas estructuradas de la API,
    ejemplos de código ejecutables y guías de despliegue — más preciso que
    buscar en `llms.txt`. Sin autenticación requerida.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Requisitos Previos

- .NET 8 SDK (o superior) con el workload MAUI: `dotnet workload install maui`
- Toolchains de plataforma para los targets que distribuya (Xcode para iOS/macCatalyst, Android SDK para Android)
- Consulte la [guía de instalación de MAUI](../../install/maui.md) para los detalles completos de configuración de plataforma

## Paquetes NuGet

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />

<!-- Redists de plataforma — incluya solo los targets para los que compila -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0"
                  Condition="$(TargetFramework.Contains('windows'))" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0"
                  Condition="$(TargetFramework.Contains('windows'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1"
                  Condition="$(TargetFramework.Contains('maccatalyst'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="15.10.33"
                  Condition="$(TargetFramework.Contains('android'))" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.0.16"
                  Condition="$(TargetFramework.Contains('ios'))" />
```

## MauiProgram.cs

Registre los manejadores de VisioForge para que MAUI pueda resolver el control `VideoView`:

```csharp
using VisioForge.Core.UI.MAUI;

public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();
    builder
        .UseMauiApp<App>()
        .UseSkiaSharp()
        .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers());

    return builder.Build();
}
```

## Diseño XAML

Coloque un `VideoView` en su página junto con un slider de búsqueda y botones de reproducción:

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:my="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
             x:Class="MauiPlayerDemo.MainPage">

    <Grid RowDefinitions="*,Auto" RowSpacing="0">
        <my:VideoView Grid.Row="0" x:Name="videoView"
                      HorizontalOptions="FillAndExpand"
                      VerticalOptions="FillAndExpand"
                      Background="Black" />

        <VerticalStackLayout Grid.Row="1" Spacing="4" Padding="12,8">
            <Slider x:Name="slSeeking" ValueChanged="slSeeking_ValueChanged" />

            <Grid ColumnDefinitions="*,*,*" ColumnSpacing="8">
                <Button Grid.Column="0" x:Name="btOpen"
                        Text="ABRIR" Clicked="btOpen_Clicked" />
                <Button Grid.Column="1" x:Name="btPlayPause"
                        Text="REPRODUCIR" Clicked="btPlayPause_Clicked" />
                <Button Grid.Column="2" x:Name="btStop"
                        Text="DETENER" Clicked="btStop_Clicked" />
            </Grid>

            <Slider x:Name="slVolume" Minimum="0" Maximum="100" Value="50"
                    ValueChanged="slVolume_ValueChanged" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## Código del Reproductor

`VideoView.GetVideoView()` devuelve un puente `IVideoView` que `MediaPlayerCoreX` consume de forma idéntica en cada target de MAUI (WinUI, macCatalyst, AndroidX, UIKit):

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.UI.MAUI;

public partial class MainPage : ContentPage
{
    private MediaPlayerCoreX _player;
    private System.Timers.Timer _positionTimer = new(500);
    private string _filename;
    private volatile bool _isTimerUpdate;

    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;
        _positionTimer.Elapsed += PositionTimer_Elapsed;
    }

    private async void MainPage_Loaded(object sender, EventArgs e)
    {
        IVideoView vv = videoView.GetVideoView();
        _player = new MediaPlayerCoreX(vv);

        _player.OnError += (_, args) => Debug.WriteLine(args.Message);
        _player.OnStop  += Player_OnStop;

        // iOS no enumera dispositivos de salida de audio — omita el selector allí.
#if !__IOS__ || __MACCATALYST__
        var outputs = await _player.Audio_OutputDevicesAsync();
        if (outputs.Length > 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(outputs[0]);
#endif

        Window.Destroying += async (_, _) =>
        {
            if (_player != null)
            {
                await _player.StopAsync();
                await _player.DisposeAsync();
                _player = null;
            }
            VisioForgeX.DestroySDK();
        };
    }
}
```

## Abrir un Archivo con `FilePicker`

```csharp
private async void btOpen_Clicked(object sender, EventArgs e)
{
    var result = await FilePicker.Default.PickAsync();
    if (result == null) return;

    _filename = result.FullPath;

    var source = await UniversalSourceSettings.CreateAsync(new Uri(_filename));
    await _player.OpenAsync(source);
    await _player.PlayAsync();

    _positionTimer.Start();
    btPlayPause.Text = "PAUSA";
}
```

Puede pasar cualquier URI soportada (HTTP, HLS, RTSP) a `UniversalSourceSettings.CreateAsync` — la misma ruta de código reproduce streams de red en móvil.

## Reproducir / Pausar / Detener

`MediaPlayerCoreX.State` expone el `PlaybackState` actual, así un solo botón cubre todo el ciclo de vida:

```csharp
private async void btPlayPause_Clicked(object sender, EventArgs e)
{
    if (_player == null || string.IsNullOrEmpty(_filename)) return;

    switch (_player.State)
    {
        case PlaybackState.Play:
            await _player.PauseAsync();
            btPlayPause.Text = "REPRODUCIR";
            break;
        case PlaybackState.Pause:
            await _player.ResumeAsync();
            btPlayPause.Text = "PAUSA";
            break;
        case PlaybackState.Free:
            var source = await UniversalSourceSettings.CreateAsync(new Uri(_filename));
            await _player.OpenAsync(source);
            await _player.PlayAsync();
            _positionTimer.Start();
            btPlayPause.Text = "PAUSA";
            break;
    }
}

private async void btStop_Clicked(object sender, EventArgs e)
{
    _positionTimer.Stop();
    if (_player != null) await _player.StopAsync();
    btPlayPause.Text = "REPRODUCIR";
}
```

## Búsqueda y Volumen

El slider de búsqueda y el de volumen pasan por la bandera del timer para que las actualizaciones de posición no compitan con los arrastres del usuario:

```csharp
private async void PositionTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
{
    if (_player == null) return;

    var position = await _player.Position_GetAsync();
    var duration = await _player.DurationAsync();

    await MainThread.InvokeOnMainThreadAsync(() =>
    {
        _isTimerUpdate = true;
        slSeeking.Maximum = duration.TotalMilliseconds;
        slSeeking.Value   = Math.Min(position.TotalMilliseconds, slSeeking.Maximum);
        _isTimerUpdate = false;
    });
}

private async void slSeeking_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (!_isTimerUpdate && _player != null)
        await _player.Position_SetAsync(TimeSpan.FromMilliseconds(e.NewValue));
}

private void slVolume_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (_player != null)
        _player.Audio_OutputDevice_Volume = e.NewValue / 100.0;
}

private void Player_OnStop(object sender, StopEventArgs e)
    => MainThread.BeginInvokeOnMainThread(() =>
    {
        btPlayPause.Text = "REPRODUCIR";
        slSeeking.Value  = 0;
    });
```

## Notas de Plataforma

- **iOS** — agregue las excepciones de [App Transport Security](../../install/maui.md) para streams HTTP (no HTTPS) y las descripciones de uso de micrófono / biblioteca de fotos que necesite. `_player.Audio_OutputDevicesAsync()` no está disponible en iOS — el renderizador selecciona la salida activa automáticamente.
- **Android** — declare el permiso `INTERNET` para streams de red; para archivos locales en API 33+ necesita `READ_MEDIA_VIDEO` o una URI otorgada por el usuario vía `FilePicker`.
- **macCatalyst** — use `VisioForge.CrossPlatform.Core.macCatalyst`; la decodificación por hardware se enruta por `VideoToolbox`.
- **Windows (WinUI)** — use los redists `Windows.x64` + `Libav.Windows.x64`; la decodificación por hardware va por `d3d11va` / `nvdec` / `qsv` según la GPU.

El cableado específico por plataforma (entitlements, manifiestos, provisioning) está en la [guía de instalación de MAUI](../../install/maui.md).

## Formatos Compatibles

| Categoría | Formatos |
|-----------|----------|
| Contenedores | MP4, MOV, MKV, WebM, AVI, TS, FLV |
| Códecs de video | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Códecs de audio | AAC, MP3, Opus, Vorbis, FLAC, AC-3 |
| Streaming | HTTP, HLS, RTSP, MPEG-DASH |

## Aplicaciones de Ejemplo

- [MAUI SimplePlayer (Media Player SDK X)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer) — el código fuente completo detrás de este tutorial
- [MAUI SimplePlayer (Media Blocks SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimplePlayer) — la misma app sobre la API de pipeline, si necesita procesamiento personalizado

## Ver También

- [Reproductor Multiplataforma Avalonia](avalonia-player.md) — multiplataforma orientado a escritorio (incluye Linux)
- [Crear un reproductor de video en C#](video-player-csharp.md) — WinForms/WPF en Windows
- [Guía del reproductor Android](android-player.md) — configuración y despliegue específicos de Android
- [Reproductor con Media Blocks Pipeline](../../mediablocks/GettingStarted/player.md) — alternativa basada en bloques con renderizadores de video/audio explícitos
- [Guía de instalación de MAUI](../../install/maui.md) — entitlements, workloads y empaquetado de redists
