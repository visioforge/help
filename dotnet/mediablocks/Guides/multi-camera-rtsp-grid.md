---
title: Multi-Camera RTSP Grid in C# .NET for 4x4 NVR Walls
description: Build a 4×4 NVR-style RTSP camera wall in C# / .NET with VisioForge Media Blocks SDK. WPF and MAUI examples, synchronized start, low-latency tuning.
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

# Multi-Camera RTSP Grid — 4×4 NVR Wall in C# / .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    The Media Blocks SDK runs on **Windows, macOS, Linux, Android, and iOS** via GStreamer — the WPF and MAUI examples below cover the full cross-platform story. See the [platform support matrix](../../platform-matrix.md) for codec and hardware-acceleration details, and the [Linux deployment guide](../../deployment-x/Ubuntu.md) for Ubuntu / NVIDIA Jetson / Raspberry Pi setup.

This guide shows how to build a 4×4 live preview grid (16 RTSP cameras at once) using the VisioForge Media Blocks SDK — the classic NVR / video wall / surveillance dashboard layout. You'll get a reusable `RTSPPlayEngine` helper class plus full XAML + code-behind examples for WPF and MAUI, including the synchronized-start pattern that keeps all 16 streams frame-aligned on screen.

## Architecture — One Pipeline per Camera

The Media Blocks SDK supports two ways to show multiple videos at once, and picking the right one matters:

- **One pipeline per camera, one `VideoRendererBlock` per cell** (this guide). Each camera gets its own `MediaBlocksPipeline` + `RTSPSourceBlock` + `VideoRendererBlock`, and each `VideoRendererBlock` draws into its own `VideoView` on the UI. This is what an NVR wall needs — 16 independent streams, each resizable, each restartable on its own.
- **One pipeline with `VideoMixerBlock`** compositing all sources into a single output frame. Useful when you want a single merged video (stream the whole wall out as one RTMP feed, record it as one MP4). Not what you want for an interactive preview grid — you lose independent control.

This guide uses the first pattern. The topology for 16 cameras:

```text
┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #0   │ ──► │ VideoRendererBlock #0 │ ──► videoView[0,0]
└───────────────────────┘     └──────────────────────┘

┌───────────────────────┐     ┌──────────────────────┐
│  RTSPSourceBlock #1   │ ──► │ VideoRendererBlock #1 │ ──► videoView[0,1]
└───────────────────────┘     └──────────────────────┘

                           ... ×16 independent pipelines ...
```

## Required NuGet Packages

**WPF (Windows x64)**:

- [VisioForge.DotNet.Core.UI.WPF](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.WPF/) — WPF `VideoView` control
- [VisioForge.DotNet.Core.Redist.MediaBlocks.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaBlocks.x64/) — GStreamer runtime for Windows x64

**MAUI (Windows / Android / iOS / macCatalyst)**:

- [VisioForge.DotNet.Core.UI.MAUI](https://www.nuget.org/packages/VisioForge.DotNet.Core.UI.MAUI/) — MAUI `VideoView` control
- Per-platform: [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/), [VisioForge.CrossPlatform.Core.Android](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Android/), [VisioForge.CrossPlatform.Core.iOS](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.iOS/), [VisioForge.CrossPlatform.Core.macCatalyst](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.macCatalyst/)

## The Reusable `RTSPPlayEngine` Helper

Both the WPF and MAUI examples use the same wrapper class. It takes an already-configured `RTSPSourceSettings` plus an `IVideoView`, builds a single-pipeline playback graph, and exposes async lifecycle methods plus an `OnError` event.

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

    // Start the pipeline in paused state (used for synchronized start)
    public Task<bool> PreloadAsync() => _pipeline.StartAsync(true);

    // Start playing immediately (non-synchronized use)
    public Task<bool> StartAsync() => _pipeline.StartAsync();

    // Resume a pipeline that was preloaded
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

The two key design points:

- **`IsSync = false`** on both renderers. For live surveillance you want drop-late-frame behaviour, not the default clock-based lipsync (which would stall the whole cell if a single packet is late).
- **`PreloadAsync` vs `StartAsync`**. `PreloadAsync` calls `pipeline.StartAsync(true)` which preloads the pipeline into Paused state. Combined with a later `ResumeAsync` this lets you fire up all 16 cameras, wait until each is sitting at its first frame, then kick them all to Play at once — no stagger on the wall.

## WPF Example — 4×4 Grid

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

`UniformGrid` is the cleanest WPF primitive for a regular grid — no row/column definitions, it spreads children into a 4×4 layout automatically.

### Code-behind

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

    // 16 RTSP URLs — replace with your own. ONVIF service URLs work too;
    // RTSPSourceSettings.CreateAsync will resolve them to RTSP internally.
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

        // 1. Build sources and engines in parallel.
        var createTasks = new Task[GridSize * GridSize];
        for (int i = 0; i < _engines.Length; i++)
        {
            int idx = i;
            createTasks[i] = Task.Run(async () =>
            {
                var settings = await RTSPSourceSettings.CreateAsync(
                    new Uri(Urls[idx]), login: "admin", password: "admin123",
                    audioEnabled: false);

                settings.LowLatencyMode = true;          // minimise jitter buffer
                settings.UseGPUDecoder  = true;          // offload H.264 to GPU

                var engine = new RTSPPlayEngine(settings, _views[idx]);
                engine.OnError += (s, err) =>
                    Dispatcher.Invoke(() =>
                        System.Diagnostics.Debug.WriteLine($"cam[{idx}]: {err.Message}"));
                _engines[idx] = engine;
            });
        }
        await Task.WhenAll(createTasks);

        // 2. Preload every pipeline into the Paused state.
        await Task.WhenAll(Array.ConvertAll(_engines, en => en.PreloadAsync()));

        // 3. Wait until all pipelines report Paused.
        for (int tries = 0; tries < 100; tries++)   // 100 × 50 ms = 5 s max
        {
            bool allPaused = true;
            foreach (var en in _engines)
                if (!en.IsPaused()) { allPaused = false; break; }
            if (allPaused) break;
            await Task.Delay(50);
        }

        // 4. Resume all simultaneously — frame-aligned start across 16 cells.
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

## MAUI Example — 4×4 Grid

The MAUI version uses the same `RTSPPlayEngine` class unchanged — the only differences are the XAML namespace, the `videoView.GetVideoView()` bridge to get an `IVideoView` from a MAUI `VideoView`, and the `MauiProgram.cs` handler registration.

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
        "rtsp://192.168.1.101:554/stream1", /* ...15 more... */
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

### Android manifest

Add to `Platforms/Android/AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.INTERNET" />
<uses-permission android:name="android.permission.ACCESS_NETWORK_STATE" />
```

### iOS / MacCatalyst

Add to the `.csproj` so the GStreamer dependencies load correctly under the Mono interpreter:

```xml
<PropertyGroup Condition="$(TargetFramework.Contains('-ios')) Or $(TargetFramework.Contains('-maccatalyst'))">
    <UseInterpreter>true</UseInterpreter>
</PropertyGroup>
```

## Synchronized Start — Why Preload + Resume

If you simply call `StartAsync()` on 16 pipelines in a loop, each one will begin playing the instant its first keyframe arrives — and that's a different wall-clock time per camera, depending on RTSP handshake latency and keyframe cadence. The eye catches the stagger immediately on a wall view.

The Preload + Resume pattern fixes this:

1. `PreloadAsync()` on every pipeline → every pipeline enters **Paused** at the first frame.
2. Poll until `IsPaused()` is true on all 16.
3. `ResumeAsync()` on every pipeline in quick succession → all cells unfreeze within the same frame.

You don't need this pattern if the cameras are showing unrelated scenes (16 different rooms). Use it when the visual continuity between cells matters (same room from multiple angles, time-synced replay, etc).

To skip sync: replace the preload/resume block with a single loop that calls `await engine.StartAsync()` on each engine, serially or via `Task.WhenAll`.

## Performance Tuning

For a responsive 16-camera wall, tune each `RTSPSourceSettings`:

- **`LowLatencyMode = true`** — sets `buffer-mode=None` + `drop-on-latency=true` internally. Cuts jitter buffer from ~1 s to ~200 ms.
- **`UseGPUDecoder = true`** — hardware H.264 / H.265 decode. Without it a 16-stream 1080p wall will saturate CPU on most laptops.
- **`AudioEnabled = false`** — on all 16. No one wants 16 overlapping audio streams.
- **`VideoRendererBlock.IsSync = false`** — drop-late-frames. The wrapper above already sets this.
- **Resolution**: ask each camera for its **sub-stream** (typically 720p or 480p) via ONVIF profile, not the main 4K feed. 16 × 4K is a bandwidth problem before it's a rendering problem.

On a mid-range desktop (8-core CPU + integrated GPU), a 4×4 720p H.264 wall sits at roughly 15–25% CPU and ≤200 MB RAM with these settings.

## Error Handling

The `RTSPPlayEngine` forwards pipeline errors through its `OnError` event. In a wall view the right response is **log and keep the other 15 running** — never tear down the whole grid on a single camera fault.

```csharp
engine.OnError += (s, err) =>
{
    // Per-camera log. Marshal to UI thread if you update UI here.
    Dispatcher.Invoke(() => LogLine($"cam[{idx}] error: {err.Message}"));
};
```

For a production NVR you'd add: timestamps, severity filtering, and a "camera disconnected" overlay on the affected `VideoView` (see next section).

## Reconnection — Fallback Switch

`RTSPSourceSettings` exposes a `FallbackSwitch` property: when the RTSP stream fails, the pipeline automatically switches to a static image, text card, or fallback media file without tearing itself down. That means the cell keeps showing *something* (like a "camera offline" slate) instead of freezing on the last good frame.

```csharp
settings.FallbackSwitch = new FallbackSwitchSettings
{
    // Image/text settings — see the FallbackSwitch docs for options.
};
```

For the full `FallbackSwitch` API (text / image / alternate-media types, tunable timeouts, `ManualUnblock`, pipeline-level telemetry via `OnNetworkSourceDisconnect`) see the dedicated [RTSP reconnection and fallback switch guide](../../general/network-sources/reconnection-and-fallback.md). For a multi-camera wall, enabling it on every engine is a one-line upgrade to production resilience.

## Related Documentation

- [RTSP camera source configuration](../../videocapture/video-sources/ip-cameras/rtsp.md) — `RTSPSourceSettings` reference, UDP/TCP transport, buffer tuning
- [ONVIF IP camera integration](../../videocapture/video-sources/ip-cameras/onvif.md) — WS-Discovery and profile selection to find the sub-stream URLs for your wall
- [Single-camera RTSP player](rtsp-player-csharp.md) — the single-stream version of this guide
- [Save original RTSP stream](rtsp-save-original-stream.md) — record any / all of the 16 feeds to disk without re-encoding
- [RTSP protocol deep-dive](../../general/network-streaming/rtsp.md) — how RTSP works under the hood
- [RTSP server output block](../RTSPServer/index.md) — serve your own composite wall as a single RTSP output

## GitHub Sample Projects

- [RTSP MultiViewSync Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo) — 3-camera sync demo this guide is based on
- [RTSP MultiView Demo (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo) — 3×3 WinForms equivalent (no sync)

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for more Media Blocks SDK code samples. Need the RTSP URL for your camera? Browse our [IP camera brands directory](../../camera-brands/index.md) covering 60+ manufacturers.
