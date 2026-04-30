---
title: .NET MAUI Video Player in C# — iOS, Android, Windows
description: Build a .NET MAUI video player in C# that runs on iOS, Android, macOS, and Windows from one codebase using VisioForge Media Player SDK's VideoView control.
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

# Build a .NET MAUI Video Player in C#

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

.NET MAUI lets you ship one C# codebase to **iOS, Android, macOS, and Windows**. This guide wires up the VisioForge `VideoView` control with `MediaPlayerCoreX` to play local files and network streams — a focused starter that mirrors the [Simple Player MAUI demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer).

> **Choosing a framework?**
> Avalonia (desktop-first, includes Linux) → [Avalonia Player Guide](avalonia-player.md).
> WinForms / WPF (Windows desktop) → [Build a Video Player in C#](video-player-csharp.md).
> Android-only (no MAUI) → [Android Player Guide](android-player.md).

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Prerequisites

- .NET 8 SDK (or newer) with MAUI workload: `dotnet workload install maui`
- Platform toolchains for the targets you ship to (Xcode for iOS/macCatalyst, Android SDK for Android)
- See the [MAUI installation guide](../../install/maui.md) for full platform setup details

## NuGet Packages

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />

<!-- Platform redists — include only the targets you build for -->
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

Register the VisioForge handlers so MAUI can resolve the `VideoView` control:

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

## XAML Layout

Drop a `VideoView` into your page alongside a seek slider and playback buttons:

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
                        Text="OPEN" Clicked="btOpen_Clicked" />
                <Button Grid.Column="1" x:Name="btPlayPause"
                        Text="PLAY" Clicked="btPlayPause_Clicked" />
                <Button Grid.Column="2" x:Name="btStop"
                        Text="STOP" Clicked="btStop_Clicked" />
            </Grid>

            <Slider x:Name="slVolume" Minimum="0" Maximum="100" Value="50"
                    ValueChanged="slVolume_ValueChanged" />
        </VerticalStackLayout>
    </Grid>
</ContentPage>
```

## Code-Behind: Player Setup

`VideoView.GetVideoView()` returns an `IVideoView` bridge that `MediaPlayerCoreX` consumes identically on every MAUI target (WinUI, macCatalyst, AndroidX, UIKit):

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

        // iOS does not enumerate audio output devices — skip the picker there.
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

## Open a File with `FilePicker`

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
    btPlayPause.Text = "PAUSE";
}
```

You can pass any supported URI (HTTP, HLS, RTSP) to `UniversalSourceSettings.CreateAsync` — the same code path plays network streams on mobile.

## Play / Pause / Stop

`MediaPlayerCoreX.State` exposes the current `PlaybackState` so one button can cover the full lifecycle:

```csharp
private async void btPlayPause_Clicked(object sender, EventArgs e)
{
    if (_player == null || string.IsNullOrEmpty(_filename)) return;

    switch (_player.State)
    {
        case PlaybackState.Play:
            await _player.PauseAsync();
            btPlayPause.Text = "PLAY";
            break;
        case PlaybackState.Pause:
            await _player.ResumeAsync();
            btPlayPause.Text = "PAUSE";
            break;
        case PlaybackState.Free:
            var source = await UniversalSourceSettings.CreateAsync(new Uri(_filename));
            await _player.OpenAsync(source);
            await _player.PlayAsync();
            _positionTimer.Start();
            btPlayPause.Text = "PAUSE";
            break;
    }
}

private async void btStop_Clicked(object sender, EventArgs e)
{
    _positionTimer.Stop();
    if (_player != null) await _player.StopAsync();
    btPlayPause.Text = "PLAY";
}
```

## Seeking and Volume

The seek slider and volume slider both route through the timer flag so timer-driven position updates do not fight user drags:

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
        btPlayPause.Text = "PLAY";
        slSeeking.Value  = 0;
    });
```

## Platform Notes

- **iOS** — add the [App Transport Security](../../install/maui.md) exceptions for HTTP (non-HTTPS) streams and the microphone/photo-library usage descriptions you need. `_player.Audio_OutputDevicesAsync()` is not available on iOS — the renderer picks the active output automatically.
- **Android** — declare `INTERNET` permission for network streams; for local-file playback on API 33+ you need `READ_MEDIA_VIDEO` or a user-granted `FilePicker` URI.
- **macCatalyst** — use `VisioForge.CrossPlatform.Core.macCatalyst`; hardware decoding runs through `VideoToolbox`.
- **Windows (WinUI)** — use the `Windows.x64` + `Libav.Windows.x64` redists; hardware decoding goes through `d3d11va` / `nvdec` / `qsv` depending on the GPU.

Full platform-specific wiring (entitlements, manifests, provisioning) lives in the [MAUI installation guide](../../install/maui.md).

## Supported Formats

| Category | Formats |
|----------|---------|
| Containers | MP4, MOV, MKV, WebM, AVI, TS, FLV |
| Video codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Audio codecs | AAC, MP3, Opus, Vorbis, FLAC, AC-3 |
| Streaming | HTTP, HLS, RTSP, MPEG-DASH |

## Sample Applications

- [MAUI SimplePlayer (Media Player SDK X)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/MAUI/SimplePlayer) — the full working source behind this tutorial
- [MAUI SimplePlayer (Media Blocks SDK)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/SimplePlayer) — the same app built on the pipeline API, if you need custom processing

## See Also

- [Avalonia Cross-Platform Player](avalonia-player.md) — desktop-first cross-platform (includes Linux)
- [Build a Video Player in C#](video-player-csharp.md) — WinForms/WPF on Windows
- [Android Player Guide](android-player.md) — Android-specific setup and deployment
- [Media Blocks Pipeline Player](../../mediablocks/GettingStarted/player.md) — block-based alternative with explicit video/audio renderers
- [MAUI Installation Guide](../../install/maui.md) — platform entitlements, workloads, and redist packaging
