---
title: Grid RTSP Multi-Cámara en C# .NET para Muros NVR 4x4
description: Construye un muro de cámaras RTSP estilo NVR 4×4 en C# / .NET con VisioForge Media Blocks SDK. Ejemplos WPF y MAUI, inicio sincronizado, baja latencia.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Android
  - iOS
  - WPF
  - MAUI
  - GStreamer
  - Streaming
  - Decoding
  - Mixing
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoView
  - RTSPPlayEngine
  - VideoRendererBlock
  - RTSPSourceSettings
  - IVideoView

---

# Grid RTSP Multi-Cámara — Muro NVR 4×4 en C# / .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Soporte multiplataforma"
    El Media Blocks SDK funciona en **Windows, macOS, Linux, Android e iOS** vía GStreamer — los ejemplos WPF y MAUI a continuación cubren toda la historia multiplataforma. Consulta la [matriz de soporte de plataformas](../../platform-matrix.md) para códecs y detalles de aceleración por hardware, y la [guía de despliegue en Linux](../../deployment-x/Ubuntu.md) para configuración en Ubuntu / NVIDIA Jetson / Raspberry Pi.

Esta guía muestra cómo construir un grid de vista previa en vivo 4×4 (16 cámaras RTSP simultáneas) usando VisioForge Media Blocks SDK — el clásico layout NVR / muro de video / dashboard de vigilancia. Obtendrás una clase helper `RTSPPlayEngine` reutilizable más ejemplos completos de XAML + código para WPF y MAUI, incluyendo el patrón de inicio sincronizado que mantiene los 16 streams alineados cuadro a cuadro en pantalla.

## Arquitectura — Un Pipeline por Cámara

Media Blocks SDK soporta dos formas de mostrar múltiples videos a la vez, y elegir la correcta importa:

- **Un pipeline por cámara, un `VideoRendererBlock` por celda** (esta guía). Cada cámara obtiene su propio `MediaBlocksPipeline` + `RTSPSourceBlock` + `VideoRendererBlock`, y cada `VideoRendererBlock` dibuja en su propio `VideoView` en la UI. Esto es lo que un muro NVR necesita — 16 streams independientes, cada uno redimensionable, cada uno reiniciable por separado.
- **Un pipeline con `VideoMixerBlock`** componiendo todas las fuentes en un único frame de salida. Útil cuando quieres un video único fusionado (transmitir todo el muro como un feed RTMP único, grabarlo como un MP4). No es lo que quieres para un grid de vista previa interactivo — pierdes el control independiente.

Esta guía usa el primer patrón. La topología para 16 cámaras:

```text
┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #0   │ ──► │ VideoRendererBlock #0 │ ──► videoView[0,0]
└───────────────────────┘     └──────────────────────┘

┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #1   │ ──► │ VideoRendererBlock #1 │ ──► videoView[0,1]
└───────────────────────┘     └──────────────────────┘

                           ... ×16 pipelines independientes ...
```

## Paquetes NuGet Requeridos

**WPF (Windows x64)**:

- [VisioForge.DotNet.Core.UI.WPF](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.WPF/) — control `VideoView` de WPF
- [VisioForge.DotNet.Core.Redist.MediaBlocks.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaBlocks.x64/) — runtime GStreamer para Windows x64

**MAUI (Windows / Android / iOS / macCatalyst)**:

- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI/) — control `VideoView` de MAUI
- Por plataforma: [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/), [VisioForge.CrossPlatform.Core.Android](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Android/), [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst/)

## La Clase Helper Reutilizable `RTSPPlayEngine`

Tanto el ejemplo WPF como el MAUI usan la misma clase wrapper. Toma un `RTSPSourceSettings` ya configurado más un `IVideoView`, construye un grafo de reproducción de pipeline único, y expone métodos async de ciclo de vida más un evento `OnError`.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Sources;

public class RTSPPlayEngine : IAsyncDisposable
{
    private MediaBlocksPipeline _pipeline;
    private VideoRendererBlock _videoRenderer;
    private AudioRendererBlock _audioRenderer;
    private RTSPSourceBlock _source;
    private bool _disposed;

    public event EventHandler<ErrorsEventArgs> OnError;

    public RTSPPlayEngine(RTSPSourceSettings rtspSettings, IVideoView videoView)
    {
        _pipeline = new MediaBlocksPipeline();
        _pipeline.OnError += (s, e) => OnError?.Invoke(this, e);

        _source = new RTSPSourceBlock(rtspSettings);
        _videoRenderer = new VideoRendererBlock(_pipeline, videoView) { IsSync = false };
        _pipeline.Connect(_source.VideoOutput, _videoRenderer.Input);

        if (rtspSettings.AudioEnabled)
        {
            _audioRenderer = new AudioRendererBlock() { IsSync = false };
            _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
        }
    }

    // Iniciar el pipeline en estado pausado (usado para inicio sincronizado)
    public Task<bool> PreloadAsync() => _pipeline.StartAsync(true);

    // Iniciar reproducción inmediatamente (uso no sincronizado)
    public Task<bool> StartAsync() => _pipeline.StartAsync();

    // Reanudar un pipeline que fue precargado
    public Task ResumeAsync() => _pipeline.ResumeAsync();

    public Task<bool> StopAsync() => _pipeline.StopAsync(true);

    public bool IsPaused() => _pipeline.State == PlaybackState.Pause;
    public bool IsStarted() => _pipeline.State == PlaybackState.Play;

    public async ValueTask DisposeAsync()
    {
        if (_disposed) return;
        _disposed = true;

        if (_pipeline != null)
        {
            await _pipeline.DisposeAsync();
            _pipeline = null;
        }

        _videoRenderer?.Dispose();
        _audioRenderer?.Dispose();
        _source?.Dispose();
    }
}
```

Los dos puntos de diseño clave:

- **`IsSync = false`** en ambos renderizadores. Para vigilancia en vivo quieres comportamiento de descarte de frames tardíos, no el lipsync basado en reloj por defecto (que bloquearía toda la celda si un solo paquete llega tarde).
- **`PreloadAsync` vs `StartAsync`**. `PreloadAsync` llama a `pipeline.StartAsync(true)` que precarga el pipeline al estado Pausado. Combinado con un `ResumeAsync` posterior, esto te permite arrancar las 16 cámaras, esperar hasta que cada una esté en su primer frame, y luego lanzarlas todas a Play a la vez — sin desfase en el muro.

## Ejemplo WPF — Grid 4×4

### XAML

```xml
<Window x:Class="MultiCameraWall.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:wpf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="RTSP 4×4 Wall"
        Width="1600" Height="900"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <DockPanel>
        <StackPanel DockPanel.Dock="Top" Orientation="Horizontal" Margin="10">
            <Button x:Name="btStart" Content="Start all" Width="80" Click="btStart_Click" />
            <Button x:Name="btStop"  Content="Stop all"  Width="80" Click="btStop_Click" Margin="10,0,0,0" />
        </StackPanel>

        <UniformGrid Rows="4" Columns="4">
            <wpf:VideoView x:Name="videoView00" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView01" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView02" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView03" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView10" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView11" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView12" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView13" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView20" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView21" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView22" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView23" Background="Black" Margin="1" />

            <wpf:VideoView x:Name="videoView30" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView31" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView32" Background="Black" Margin="1" />
            <wpf:VideoView x:Name="videoView33" Background="Black" Margin="1" />
        </UniformGrid>
    </DockPanel>
</Window>
```

`UniformGrid` es la primitiva WPF más limpia para un grid regular — sin definiciones de filas/columnas, distribuye los hijos en un layout 4×4 automáticamente.

### Código

```csharp
using System;
using System.Threading.Tasks;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Sources;

public partial class MainWindow : Window
{
    private const int GridSize = 4;
    private readonly RTSPPlayEngine[] _engines = new RTSPPlayEngine[GridSize * GridSize];
    private readonly IVideoView[] _views;

    // 16 URLs RTSP — reemplaza con las tuyas. Las URLs de servicio ONVIF también funcionan;
    // RTSPSourceSettings.CreateAsync las resolverá a RTSP internamente.
    private static readonly string[] Urls =
    {
        "rtsp://192.168.1.101:554/stream1", "rtsp://192.168.1.102:554/stream1",
        "rtsp://192.168.1.103:554/stream1", "rtsp://192.168.1.104:554/stream1",
        "rtsp://192.168.1.105:554/stream1", "rtsp://192.168.1.106:554/stream1",
        "rtsp://192.168.1.107:554/stream1", "rtsp://192.168.1.108:554/stream1",
        "rtsp://192.168.1.109:554/stream1", "rtsp://192.168.1.110:554/stream1",
        "rtsp://192.168.1.111:554/stream1", "rtsp://192.168.1.112:554/stream1",
        "rtsp://192.168.1.113:554/stream1", "rtsp://192.168.1.114:554/stream1",
        "rtsp://192.168.1.115:554/stream1", "rtsp://192.168.1.116:554/stream1",
    };

    public MainWindow()
    {
        InitializeComponent();
        _views = new IVideoView[]
        {
            videoView00, videoView01, videoView02, videoView03,
            videoView10, videoView11, videoView12, videoView13,
            videoView20, videoView21, videoView22, videoView23,
            videoView30, videoView31, videoView32, videoView33,
        };
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        await DestroyAllAsync();

        // 1. Construir fuentes y engines en paralelo.
        var createTasks = new Task[GridSize * GridSize];
        for (int i = 0; i < _engines.Length; i++)
        {
            int idx = i;
            createTasks[i] = Task.Run(async () =>
            {
                var settings = await RTSPSourceSettings.CreateAsync(
                    new Uri(Urls[idx]), login: "admin", password: "admin123",
                    audioEnabled: false);

                settings.LowLatencyMode = true;          // minimizar buffer de jitter
                settings.UseGPUDecoder  = true;          // descargar H.264 a GPU

                var engine = new RTSPPlayEngine(settings, _views[idx]);
                engine.OnError += (s, err) =>
                    Dispatcher.Invoke(() =>
                        System.Diagnostics.Debug.WriteLine($"cam[{idx}]: {err.Message}"));
                _engines[idx] = engine;
            });
        }
        await Task.WhenAll(createTasks);

        // 2. Precargar cada pipeline al estado Pausado.
        await Task.WhenAll(Array.ConvertAll(_engines, en => en.PreloadAsync()));

        // 3. Esperar hasta que todos los pipelines reporten Pausado.
        for (int tries = 0; tries < 100; tries++)   // 100 × 50 ms = 5 s max
        {
            bool allPaused = true;
            foreach (var en in _engines)
                if (!en.IsPaused()) { allPaused = false; break; }
            if (allPaused) break;
            await Task.Delay(50);
        }

        // 4. Reanudar todos simultáneamente — inicio alineado por frame en 16 celdas.
        foreach (var en in _engines)
            await en.ResumeAsync().ConfigureAwait(false);
    }

    private async void btStop_Click(object sender, RoutedEventArgs e) => await DestroyAllAsync();

    private async Task DestroyAllAsync()
    {
        for (int i = 0; i < _engines.Length; i++)
        {
            if (_engines[i] != null)
            {
                await _engines[i].DisposeAsync();
                _engines[i] = null;
            }
        }
    }

    private async void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        await DestroyAllAsync();
        VisioForgeX.DestroySDK();
    }
}
```

## Ejemplo MAUI — Grid 4×4

La versión MAUI usa la misma clase `RTSPPlayEngine` sin cambios — las únicas diferencias son el namespace XAML, el puente `videoView.GetVideoView()` para obtener un `IVideoView` desde un `VideoView` de MAUI, y el registro de handlers en `MauiProgram.cs`.

### MauiProgram.cs

```csharp
using SkiaSharp.Views.Maui.Controls.Hosting;
using VisioForge.Core.UI.MAUI;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .ConfigureMauiHandlers(handlers => handlers.AddVisioForgeHandlers())
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        return builder.Build();
    }
}
```

### MainPage.xaml

```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vf="clr-namespace:VisioForge.Core.UI.MAUI;assembly=VisioForge.Core.UI.MAUI"
             x:Class="MultiCameraWall.MainPage">
    <Grid RowDefinitions="Auto,*">
        <HorizontalStackLayout Grid.Row="0" Padding="10" Spacing="10">
            <Button x:Name="btStart" Text="Start all" Clicked="OnStartClicked" />
            <Button x:Name="btStop"  Text="Stop all"  Clicked="OnStopClicked"  />
        </HorizontalStackLayout>

        <Grid Grid.Row="1"
              RowDefinitions="*,*,*,*"
              ColumnDefinitions="*,*,*,*"
              RowSpacing="1" ColumnSpacing="1" BackgroundColor="Black">

            <vf:VideoView x:Name="cam00" Grid.Row="0" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam01" Grid.Row="0" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam02" Grid.Row="0" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam03" Grid.Row="0" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam10" Grid.Row="1" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam11" Grid.Row="1" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam12" Grid.Row="1" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam13" Grid.Row="1" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam20" Grid.Row="2" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam21" Grid.Row="2" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam22" Grid.Row="2" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam23" Grid.Row="2" Grid.Column="3" BackgroundColor="Black" />

            <vf:VideoView x:Name="cam30" Grid.Row="3" Grid.Column="0" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam31" Grid.Row="3" Grid.Column="1" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam32" Grid.Row="3" Grid.Column="2" BackgroundColor="Black" />
            <vf:VideoView x:Name="cam33" Grid.Row="3" Grid.Column="3" BackgroundColor="Black" />
        </Grid>
    </Grid>
</ContentPage>
```

### MainPage.xaml.cs

```csharp
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

public partial class MainPage : ContentPage
{
    private const int GridSize = 4;
    private readonly RTSPPlayEngine[] _engines = new RTSPPlayEngine[GridSize * GridSize];
    private IVideoView[] _views;

    private static readonly string[] Urls =
    {
        "rtsp://192.168.1.101:554/stream1", /* ...15 más... */
    };

    public MainPage()
    {
        InitializeComponent();
        _views = new IVideoView[]
        {
            cam00.GetVideoView(), cam01.GetVideoView(), cam02.GetVideoView(), cam03.GetVideoView(),
            cam10.GetVideoView(), cam11.GetVideoView(), cam12.GetVideoView(), cam13.GetVideoView(),
            cam20.GetVideoView(), cam21.GetVideoView(), cam22.GetVideoView(), cam23.GetVideoView(),
            cam30.GetVideoView(), cam31.GetVideoView(), cam32.GetVideoView(), cam33.GetVideoView(),
        };

        VisioForgeX.InitSDK();
    }

    private async void OnStartClicked(object sender, EventArgs e)
    {
        await DestroyAllAsync();

        var createTasks = new Task[GridSize * GridSize];
        for (int i = 0; i < _engines.Length; i++)
        {
            int idx = i;
            createTasks[i] = Task.Run(async () =>
            {
                var settings = await RTSPSourceSettings.CreateAsync(
                    new Uri(Urls[idx]), "admin", "admin123", audioEnabled: false);

                settings.LowLatencyMode = true;
                settings.UseGPUDecoder  = true;

                _engines[idx] = new RTSPPlayEngine(settings, _views[idx]);
            });
        }
        await Task.WhenAll(createTasks);

        await Task.WhenAll(Array.ConvertAll(_engines, en => en.PreloadAsync()));

        for (int tries = 0; tries < 100; tries++)
        {
            bool allPaused = true;
            foreach (var en in _engines)
                if (!en.IsPaused()) { allPaused = false; break; }
            if (allPaused) break;
            await Task.Delay(50);
        }

        foreach (var en in _engines)
            await en.ResumeAsync().ConfigureAwait(false);
    }

    private async void OnStopClicked(object sender, EventArgs e) => await DestroyAllAsync();

    private async Task DestroyAllAsync()
    {
        for (int i = 0; i < _engines.Length; i++)
        {
            if (_engines[i] != null)
            {
                await _engines[i].DisposeAsync();
                _engines[i] = null;
            }
        }
    }
}
```

### Manifiesto Android

Añade en `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

### iOS / MacCatalyst

Añade al `.csproj` para que las dependencias de GStreamer carguen correctamente bajo el intérprete Mono:

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-ios')) Or $(TargetFramework.Contains('-maccatalyst'))">
    <UseInterpreter>true</UseInterpreter>
</PropertyGroup>
```

## Inicio Sincronizado — Por Qué Preload + Resume

Si simplemente llamas a `StartAsync()` en 16 pipelines en un bucle, cada uno empezará a reproducir en el instante en que llegue su primer keyframe — y eso es un tiempo de reloj de pared diferente por cámara, dependiendo de la latencia del handshake RTSP y la cadencia de keyframes. El ojo detecta el desfase inmediatamente en una vista de muro.

El patrón Preload + Resume lo arregla:

1. `PreloadAsync()` en cada pipeline → cada pipeline entra en **Pausado** en el primer frame.
2. Polling hasta que `IsPaused()` sea true en las 16.
3. `ResumeAsync()` en cada pipeline en rápida sucesión → todas las celdas se descongelan dentro del mismo frame.

No necesitas este patrón si las cámaras muestran escenas no relacionadas (16 salas diferentes). Úsalo cuando la continuidad visual entre celdas importa (misma sala desde múltiples ángulos, reproducción sincronizada en tiempo, etc).

Para omitir la sincronización: reemplaza el bloque preload/resume por un único bucle que llame a `await engine.StartAsync()` en cada engine, secuencialmente o vía `Task.WhenAll`.

## Ajuste de Rendimiento

Para un muro de 16 cámaras responsivo, ajusta cada `RTSPSourceSettings`:

- **`LowLatencyMode = true`** — establece internamente `buffer-mode=None` + `drop-on-latency=true`. Reduce el buffer de jitter de ~1 s a ~200 ms.
- **`UseGPUDecoder = true`** — decodificación H.264 / H.265 por hardware. Sin esto, un muro de 16 streams 1080p saturará la CPU en la mayoría de laptops.
- **`AudioEnabled = false`** — en las 16. Nadie quiere 16 streams de audio superpuestos.
- **`VideoRendererBlock.IsSync = false`** — descartar frames tardíos. El wrapper anterior ya lo establece.
- **Resolución**: pide a cada cámara su **sub-stream** (típicamente 720p o 480p) vía perfil ONVIF, no el feed principal 4K. 16 × 4K es un problema de ancho de banda antes de ser un problema de renderizado.

En un escritorio gama media (CPU 8 cores + GPU integrada), un muro 4×4 720p H.264 se mantiene en aproximadamente 15–25% de CPU y ≤200 MB de RAM con estos ajustes.

## Manejo de Errores

La clase `RTSPPlayEngine` reenvía los errores del pipeline a través de su evento `OnError`. En una vista de muro la respuesta correcta es **registrar y mantener las otras 15 funcionando** — nunca derribar todo el grid por un fallo de una sola cámara.

```csharp
engine.OnError += (s, err) =>
{
    // Log por cámara. Marshal al hilo UI si actualizas la UI aquí.
    Dispatcher.Invoke(() => LogLine($"cam[{idx}] error: {err.Message}"));
};
```

Para un NVR de producción añadirías: timestamps, filtrado por severidad, y un overlay "cámara desconectada" en el `VideoView` afectado (ver siguiente sección).

## Reconexión — Fallback Switch

`RTSPSourceSettings` expone una propiedad `FallbackSwitch`: cuando el stream RTSP falla, el pipeline cambia automáticamente a una imagen estática, tarjeta de texto o archivo multimedia alternativo sin derribarse. Eso significa que la celda sigue mostrando *algo* (como una placa "cámara offline") en lugar de congelarse en el último buen frame.

```csharp
settings.FallbackSwitch = new FallbackSwitchSettings
{
    // Ajustes de imagen/texto — ver la documentación de FallbackSwitch para opciones.
};
```

Para la API completa de `FallbackSwitch` (tipos texto / imagen / media alternativo, timeouts ajustables, `ManualUnblock`, telemetría a nivel de pipeline vía `OnNetworkSourceDisconnect`) consulta la [guía dedicada de reconexión RTSP y fallback switch](../../general/network-sources/reconnection-and-fallback.md). Para un muro multi-cámara, habilitarlo en cada engine es una mejora de una línea hacia resiliencia de producción.

## Documentación Relacionada

- [Configuración de fuente de cámara RTSP](../../videocapture/video-sources/ip-cameras/rtsp.md) — referencia de `RTSPSourceSettings`, transporte UDP/TCP, ajuste de buffer
- [Integración de cámara IP ONVIF](../../videocapture/video-sources/ip-cameras/onvif.md) — WS-Discovery y selección de perfiles para encontrar las URLs de sub-stream para tu muro
- [Reproductor RTSP mono-cámara](rtsp-player-csharp.md) — la versión de stream único de esta guía
- [Guardar stream RTSP original](rtsp-save-original-stream.md) — grabar cualquiera / todos los 16 feeds a disco sin re-codificar
- [Inmersión profunda en el protocolo RTSP](../../general/network-streaming/rtsp.md) — cómo funciona RTSP por dentro
- [Bloque de salida de servidor RTSP](../RTSPServer/index.md) — servir tu propio muro compuesto como una única salida RTSP

## Proyectos de Ejemplo en GitHub

- [RTSP MultiViewSync Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo) — demo de sincronización de 3 cámaras en la que se basa esta guía
- [RTSP MultiView Demo (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) — equivalente 3×3 en WinForms (sin sincronización)

---
Visita nuestra página de [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) para más ejemplos de código Media Blocks SDK. ¿Necesitas la URL RTSP para tu cámara? Consulta nuestro [directorio de marcas de cámaras IP](../../camera-brands/index.md) cubriendo más de 60 fabricantes.
