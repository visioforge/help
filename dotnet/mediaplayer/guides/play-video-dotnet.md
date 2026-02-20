---
title: Cross-Platform Video Player - Avalonia & MAUI Guide
description: Build cross-platform video players with Avalonia and .NET MAUI. Play video on Windows, macOS, Linux, Android, and iOS from a single C# codebase.
---

# Cross-Platform Video Player: Avalonia & MAUI Guide

Build video players that run on Windows, macOS, Linux, Android, and iOS using [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net). Both Avalonia and .NET MAUI use the `MediaBlocksPipeline` API, which provides a flexible block-based architecture for cross-platform media playback.

> **Looking for WinForms or WPF?** See [Build a Video Player in C#](video-player-csharp.md) or [Build a Video Player in VB.NET](video-player-vb-net.md) for Windows-only implementations using `MediaPlayerCoreX`.

## Choosing the Right Approach

| Framework | Platform | SDK | API |
|-----------|----------|-----|-----|
| WinForms | Windows | [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) | `MediaPlayerCoreX` |
| WPF | Windows | [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) | `MediaPlayerCoreX` |
| Avalonia | Windows, macOS, Linux | [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) | `MediaBlocksPipeline` |
| MAUI | Windows, macOS, Android, iOS | [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) | `MediaBlocksPipeline` |

## Avalonia Video Player

Avalonia enables video playback on Windows, macOS, and Linux from a single codebase using the `MediaBlocksPipeline`.

### NuGet Packages

Core SDK package:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.2.19" />
```

Platform-specific redist packages (add the ones for your target platforms):

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

### AXAML Layout

Add the `VideoView` control to your Avalonia view:

```xml
<UserControl xmlns:avalonia="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia">
    <Grid RowDefinitions="*,Auto">
        <avalonia:VideoView x:Name="videoView1" Background="#0C0C0C" />

        <StackPanel Grid.Row="1" Orientation="Vertical">
            <Slider Name="slSeeking" Maximum="{Binding SeekingMaximum}"
                    Value="{Binding SeekingValue}" />
            <StackPanel Orientation="Horizontal" HorizontalAlignment="Center">
                <Button Command="{Binding OpenFileCommand}" Content="OPEN FILE" />
                <Button Command="{Binding PlayPauseCommand}" Content="{Binding PlayPauseText}" />
                <Button Command="{Binding StopCommand}" Content="STOP" />
            </StackPanel>
        </StackPanel>
    </Grid>
</UserControl>
```

### Pipeline Setup and Playback

Create the pipeline, connect source to video and audio renderers, and start playback:

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

    // Create file source
    var sourceSettings = await UniversalSourceSettings.CreateAsync(
        new Uri(filePath));
    var source = new UniversalSourceBlock(sourceSettings);

    // Connect video renderer
    var videoRenderer = new VideoRendererBlock(_pipeline, videoView);
    _pipeline.Connect(source.VideoOutput, videoRenderer.Input);

    // Connect audio renderer
    var audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(source.AudioOutput, audioRenderer.Input);

    await _pipeline.StartAsync();
}
```

### MVVM Pattern with ReactiveUI

The recommended approach for Avalonia uses MVVM with ReactiveUI. The ViewModel manages the pipeline lifecycle, playback state, and UI bindings:

```csharp
using ReactiveUI;

public class MainViewModel : ReactiveObject
{
    private MediaBlocksPipeline _pipeline;
    private System.Timers.Timer _tmPosition = new(1000);

    public IVideoView VideoViewIntf { get; set; }

    private string? _playPauseText = "PLAY";
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
            PlayPauseText = "PAUSE";
        }
        else if (_pipeline.State == PlaybackState.Play)
        {
            await _pipeline.PauseAsync();
            PlayPauseText = "PLAY";
        }
        else if (_pipeline.State == PlaybackState.Pause)
        {
            await _pipeline.ResumeAsync();
            PlayPauseText = "PAUSE";
        }
    }
}
```

For a complete Avalonia player implementation including file dialogs, seeking, volume control, platform-specific setup for Android/iOS, and full project structure, see the [Avalonia Player Guide](avalonia-player.md).

## MAUI Video Player

.NET MAUI enables video playback on Windows, macOS, Android, and iOS using `MediaBlocksPipeline`.

### Required NuGet Packages

Core SDK package:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2026.2.19" />
```

Platform-specific redist packages:

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

### MAUI App Configuration

Register VisioForge handlers in `MauiProgram.cs`:

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

### XAML Layout

Add the `VideoView` control to your MAUI page:

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
                <Button x:Name="btOpen" Text="OPEN FILE"
                        Clicked="btOpen_Clicked" />
                <Button x:Name="btPlayPause" Text="PLAY"
                        Clicked="btPlayPause_Clicked" />
                <Button x:Name="btStop" Text="STOP"
                        Clicked="btStop_Clicked" />
            </StackLayout>
        </StackLayout>
    </Grid>
</ContentPage>
```

### Pipeline Setup

Build the pipeline with source, video renderer, and audio renderer blocks:

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

    // Create source from file
    _source = new UniversalSourceBlock(
        await UniversalSourceSettings.CreateAsync(new Uri(_filename)));

    // Connect video renderer
    var videoRenderer = new VideoRendererBlock(
        _pipeline, videoView.GetVideoView());
    _pipeline.Connect(_source.VideoOutput, videoRenderer.Input);

    // Connect audio renderer
    _audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
}
```

### Playback Controls

Handle play/pause, stop, and file selection:

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
        btPlayPause.Text = "PAUSE";
    }
    else if (_pipeline.State == PlaybackState.Play)
    {
        await _pipeline.PauseAsync();
        btPlayPause.Text = "PLAY";
    }
    else if (_pipeline.State == PlaybackState.Pause)
    {
        await _pipeline.ResumeAsync();
        btPlayPause.Text = "PAUSE";
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

### Seeking and Volume Control

```csharp
// Seeking (slider value changed handler)
private async void slSeeking_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (!_isTimerUpdate && _pipeline != null)
    {
        await _pipeline.Position_SetAsync(
            TimeSpan.FromMilliseconds(e.NewValue));
    }
}

// Volume control (0-100 scale to 0.0-1.0)
private void slVolume_ValueChanged(object sender, ValueChangedEventArgs e)
{
    if (_audioRenderer != null)
    {
        _audioRenderer.Volume = e.NewValue / 100.0;
    }
}
```

### Cleanup

Dispose pipeline resources when the window closes:

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

## Cross-Platform NuGet Package Reference

| Platform | Package | Version |
|----------|---------|---------|
| All | `VisioForge.DotNet.MediaBlocks` | 2026.2.19 |
| Avalonia UI | `VisioForge.DotNet.Core.UI.Avalonia` | 2026.2.19 |
| Windows x64 | `VisioForge.CrossPlatform.Core.Windows.x64` | 2025.11.0 |
| Windows x64 | `VisioForge.CrossPlatform.Libav.Windows.x64` | 2025.11.0 |
| macOS | `VisioForge.CrossPlatform.Core.macOS` | 2025.9.1 |
| macOS (MAUI) | `VisioForge.CrossPlatform.Core.macCatalyst` | 2025.9.1 |
| Linux x64 | `VisioForge.CrossPlatform.Core.Linux.x64` | 2025.11.0 |
| Linux x64 | `VisioForge.CrossPlatform.Libav.Linux.x64` | 2025.11.0 |
| Android | `VisioForge.CrossPlatform.Core.Android` | 15.10.33 |
| iOS | `VisioForge.CrossPlatform.Core.iOS` | 2025.0.16 |

## Supported Formats

- **Video:** MP4, AVI, MKV, WMV, WebM, MOV, TS, FLV
- **Audio:** MP3, AAC, WAV, WMA, FLAC, OGG
- **Codecs:** H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2
- **Streaming:** RTSP, HTTP, HLS, MPEG-DASH

## Sample Applications

| Platform | Demo |
|----------|------|
| Avalonia MVVM | [SimplePlayerMVVM](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayerMVVM) |
| Avalonia Simple | [SimplePlayer](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Avalonia/SimplePlayer) |
| MAUI | [SimplePlayer](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimplePlayer) |

## Related Resources

- [Build a Video Player in C#](video-player-csharp.md) (WinForms/WPF)
- [Build a Video Player in VB.NET](video-player-vb-net.md) (WinForms)
- [Avalonia Player Guide](avalonia-player.md) (full MVVM implementation)
- [Android Player Guide](android-player.md)
- [Media Blocks SDK .Net Product Page](https://www.visioforge.com/media-blocks-sdk-net)
