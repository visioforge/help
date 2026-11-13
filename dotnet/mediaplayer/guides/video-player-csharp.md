---
title: C# Video Player for WinForms & WPF — Seek & Volume Controls
description: Build a C# video player for Windows WinForms or WPF desktop apps with VisioForge Media Player SDK .NET — seeking, volume, speed, and subtitle support.
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

# Build a C# Video Player for WinForms & WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

This tutorial builds a **Windows desktop** video player in C# with [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) and its `MediaPlayerCoreX` engine: a WPF window that plays a file with a timeline, pause and resume, volume, playback speed, a choice of the audio output and an error log. The same player as a WinForms form is [further down this page](#second-option-winforms).

!!! info "Choose your approach"
    This page is for **Windows desktop apps** (WinForms or WPF) using Media Player SDK .NET.

    - **Cross-platform desktop (includes Linux)** → [Avalonia Player Guide](avalonia-player.md)
    - **Mobile + desktop from one codebase (iOS, Android, macOS, Windows)** → [.NET MAUI Player Guide](maui-player.md)
    - **Android only** → [Android Player Guide](android-player.md)
    - **Visual Basic .NET** → [VB.NET Video Player Guide](video-player-vb-net.md)
    - **Pipeline-based (block graph with explicit renderers)** → [Media Blocks SDK Player](../../mediablocks/GettingStarted/player.md)

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/YJbUV370TKo?controls=1" title="Build a video player in C# WPF - YouTube tutorial" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/_VideoTutorials/video-player-csharp-wpf)

The film in the video is [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

The video creates the project on .NET 10, which needs Visual Studio 2026; in Visual Studio 2022, choose .NET 8 or .NET 9.

## Required NuGet packages

- [VisioForge.DotNet.MediaPlayer](https://www.nuget.org/packages/VisioForge.DotNet.MediaPlayer/) — the SDK; the WPF and WinForms `VideoView` controls ship in it
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — the GStreamer runtime `MediaPlayerCoreX` runs on
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — the FFmpeg (libav) codecs that many formats decode through

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="[2026.11.12]"/>
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="[2026.11.12]"/>
```

These are the x64 packages, so the project sets `<PlatformTarget>x64</PlatformTarget>`; a 32-bit app takes the `.x86` packages instead. See [Windows deployment](../../deployment-x/Windows.md) for the full list.

## The window in XAML

The `vf` prefix maps the `VisioForge.Core.UI.WPF` namespace of the `VisioForge.Core` assembly. The `VideoView` fills the left column, with the timeline and the playback buttons under it; the right column holds the file name, the audio output, the volume and speed sliders and the error log. Play stays disabled until the SDK has loaded.

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

## The code behind the window

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

    private bool _timerMovesSlider; // true while the timer, not the user, moves the timeline

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Raised by StopAsync and at the end of the file; acted on only for the end of the file
        // (Stop is enabled), not for a late stop of the previous file (State is no longer Free)
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
            await _player.Rate_SetAsync(slSpeed.Value); // a newly opened file starts at 1.0
            await _player.PlayAsync();
        }

        // Free: the file did not open or play (the reason is in the log), or it has already ended
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
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // stopped, or the user drags

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
        slTimeline.Maximum = duration.TotalSeconds;
        slTimeline.Value = position.TotalSeconds;
        _timerMovesSlider = false;
    }

    // A click on the track seeks at once, a drag seeks once when it ends
    private async void slTimeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (!_timerMovesSlider && !slTimeline.IsMouseCaptureWithin) await SeekAsync();
    }

    private async void slTimeline_DragCompleted(object sender, DragCompletedEventArgs e)
    {
        if (!e.Canceled && e.HorizontalChange != 0) await SeekAsync(); // not a press without a move
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
        if (btStop.IsEnabled) await _player.Rate_SetAsync(slSpeed.Value); // Play applies it otherwise
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        if (!btPlay.IsEnabled && !btStop.IsEnabled)
        {
            e.Cancel = true; // still loading the SDK, opening a file or stopping
            return;
        }

        _timer.Stop();
        _player.Dispose(); // stops playback first
        VisioForgeX.DestroySDK();
    }
}
```

## How it works

1. **SDK initialization** — `VisioForgeX.InitSDKAsync` runs once, when the window loads. The first run on a PC takes longer, so Play is enabled only after it returns. One `MediaPlayerCoreX` serves every file the app plays; `Dispose` and `VisioForgeX.DestroySDK` release them when the window closes.
2. **Audio output** — `Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)` lists the speakers and headphones, and `AudioRendererSettings` sends the sound to the one picked. `Audio_OutputDevice_Volume` takes 0.0 to 1.0 and can change while the file plays.
3. **Open and play** — `OpenAsync` takes a file path or a URL and returns `false` when the source cannot be opened; the reason arrives through `OnError`. `PlayAsync` starts playback. The speed is set after `OpenAsync`, because a newly opened file always starts at 1.0. Then `State` tells whether the file plays: `PlaybackState.Free` means it did not open or play, or a very short file has already ended, and the window goes back to stopped.
4. **Timeline** — a `DispatcherTimer` reads `Position_GetAsync` and `DurationAsync` twice a second on the UI thread. The `_timerMovesSlider` flag tells the timer's own updates apart from the user's, so the timer never seeks. A click on the track seeks at once; a drag seeks once, in `Thumb.DragCompleted`, and the timer leaves the slider alone while the mouse holds it. Seeking works while paused as well.
5. **Speed** — `Rate_SetAsync` sets the playback rate: 1.0 is normal speed, 2.0 double, 0.5 half. The slider here goes from 0.5 to 2 in steps of 0.25.
6. **Stop, the end of the file and closing** — `OnStop` fires after `StopAsync` and when the file ends; it arrives on another thread, so the handler goes back to the UI thread with `Dispatcher.InvokeAsync`, as `OnError` does. Play and Stop disable the playback buttons while they work, and the Stop button resets the window itself after `StopAsync`, so the handler acts only while Stop is enabled and `State` is `PlaybackState.Free`: the end of the file, not a late `OnStop` of the previous file that arrives after the next one has started. Closing the window while the SDK loads, a file opens or playback stops is cancelled; otherwise `Dispose` stops playback and releases the player.

## Notes

- Volume set before Play is kept and applied when playback starts.
- With no audio device in the list, the file plays without sound.
- `OpenAsync` also takes an `rtsp://`, `http://` or HLS URL; a live stream has no duration, so the time label shows the position, the timeline stays at the start, and seeking depends on the source.

## Second option: WinForms

The same player in a WinForms form. A `TrackBar` raises `Scroll` only when the user moves it, so the timer can set the timeline without the flag the WPF slider needs, and a `System.Windows.Forms.Timer` ticks on the UI thread. The trackbar holds the mouse while a button is pressed on it, so a click or a drag seeks once, in `MouseUp`. The form holds a `VideoView` from `VisioForge.Core.UI.WinForms` named `VideoView1`, `TrackBar`s named `tbTimeline`, `tbVolume` (0–100) and `tbSpeed` (5–20, read as 0.5–2.0), a `ComboBox` `cbAudioOutput`, the buttons `btOpenFile`, `btStart`, `btPause`, `btResume` and `btStop` (all but `btOpenFile` disabled at design time), the label `lbTime`, and the text boxes `edFilename` and `edLog` (multiline, read-only). Wire `Scroll` on the three trackbars and `MouseUp` on `tbTimeline`, `Click` on the buttons, and `Load` and `FormClosing` on the form.

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

    private bool _scrolled; // true when the user moved the timeline with the mouse button down

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += (_, args) => BeginInvoke(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Raised by StopAsync and at the end of the file; acted on only for the end of the file
        // (Stop is enabled), not for a late stop of the previous file (State is no longer Free)
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
            await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // a newly opened file starts at 1.0
            await _player.PlayAsync();
        }

        // Free: the file did not open or play (the reason is in the log), or it has already ended
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
        if (!_timer.Enabled || tbTimeline.Capture) return; // stopped, or the user drags

        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        // Setting Value does not raise Scroll, so this does not seek
        tbTimeline.Maximum = (int)duration.TotalSeconds;
        tbTimeline.Value = Math.Min((int)position.TotalSeconds, tbTimeline.Maximum);
    }

    // A key seeks at once, a click or a drag seeks once when the mouse button is released
    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (tbTimeline.Capture) _scrolled = true;
        else await SeekAsync();
    }

    private async void tbTimeline_MouseUp(object sender, MouseEventArgs e)
    {
        if (!_scrolled) return; // a press without a move does not seek

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
        if (btStop.Enabled) await _player.Rate_SetAsync(tbSpeed.Value / 10.0); // Start applies it otherwise
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // still loading the SDK, opening a file or stopping
            return;
        }

        _timer.Stop();
        _player.Dispose(); // stops playback first
        VisioForgeX.DestroySDK();
    }
}
```

## Alternative: Media Blocks SDK Pipeline

If you need the extra flexibility of a block graph (custom processing, multiple sinks, overlays), the [Media Blocks SDK Player Guide](../../mediablocks/GettingStarted/player.md) builds an equivalent pipeline with `UniversalSourceBlock` + `VideoRendererBlock` + `AudioRendererBlock`. The code above uses Media Player SDK because it is purpose-built for this scenario — one `MediaPlayerCoreX` instance replaces the block wiring.

## Supported Formats

| Category | Formats |
|----------|---------|
| Video | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Audio | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Streaming | RTSP, HTTP, HLS, MPEG-DASH |

## Sample Applications

- [Simple Player Demo WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo)
- [Media Player Code Snippet (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/media-player)
- [WinForms Main Demo (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Main%20Demo)

## Frequently Asked Questions

### What video formats does the .NET video player support?

The SDK supports all major video formats including MP4, AVI, MKV, WMV, WebM, MOV, TS, and FLV. Codec support includes H.264, H.265/HEVC, VP8, VP9, AV1, and MPEG-2 through the bundled GStreamer-based engine. Audio formats such as MP3, AAC, WAV, FLAC, and OGG are also supported for playback.

### Can I play RTSP streams or network video in my C# application?

Yes. `OpenAsync` takes an RTSP, HTTP, HLS or MPEG-DASH URL as well as a file path, so the same code plays a stream. For RTSP sources that require authentication, include the credentials directly in the URL (e.g., `rtsp://user:password@host:554/stream`).

### How do I control playback speed in the video player?

Call `Rate_SetAsync(double rate)` on the player after `OpenAsync`. A rate of 1.0 is normal speed, 2.0 is double speed, and 0.5 is half speed. The rate can be changed during playback without stopping the video. After `StopAsync` or the end of the file the next playback starts at 1.0, so set the rate again after the next `OpenAsync`, as the code above does.

### Does the SDK support subtitle rendering?

Yes. `Subtitles_Enabled` is on by default, so `OpenAsync` draws the embedded subtitle track of an MKV or MP4 file. To load an external SRT, ASS or WebVTT file, set `Subtitles_ExternalFile` before `OpenAsync`; `Subtitles_Settings` sets how subtitles look, and `Subtitle_Streams` lists the subtitle tracks the file holds.

### Can I build a cross-platform video player with this SDK?

Yes. The SDK runs on Windows, macOS, Linux, Android, and iOS. For cross-platform desktop applications, use the [Avalonia UI framework](avalonia-player.md) with the same `MediaPlayerCoreX` API. The core playback engine is identical across platforms — only the video rendering surface and NuGet packages differ.

## Video Transcript

### What we build

Let's build a video player in a C# WPF app with the VisioForge Media Player SDK. Here's what we build. A WPF window that plays a video file: MP4, MKV, AVI, MOV, WebM and many more formats. It has Play, Pause, Resume and Stop, a timeline you can click or drag to seek, sliders for the volume and the playback speed, a list of the audio outputs, and a log for errors. The code behind it is about a hundred and eighty lines of C#. This is the finished player. The film is Big Buck Bunny, by the Blender Foundation, under a Creative Commons Attribution license.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. And a video file to play. The written guide also has the same player as a WinForms form.

### Create a WPF project

In Visual Studio, create a new project. Search for WPF, and pick WPF Application for C#. Name the project VideoPlayerWpf. Choose .NET 10 and click Create. Visual Studio opens MainWindow.xaml, with the designer above and the XAML below.

### Add the NuGet packages

Now add three NuGet packages. The Media Player SDK itself. CrossPlatform Core, the GStreamer runtime the X engine runs on. And CrossPlatform Libav, the FFmpeg codecs that many formats decode through. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.MediaPlayer from the Browse tab. Then install the Core and Libav packages for Windows x64. The Installed tab lists all three. These are the x64 packages, so set PlatformTarget to x64 in the project file. A 32-bit app takes the x86 packages instead.

### Build the window in XAML

The window is written in XAML. First the window itself, with a grid and a VideoView, and the designer draws it right away. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. The grid has two columns, the video and a 260-pixel settings column, and three rows: the video, the timeline and the buttons. The VideoView fills the first cell. The window also names its Loaded and Closing handlers. Under the video goes the timeline. A slider for the position, with the time on its right. IsMoveToPointEnabled makes a click on the track jump straight to that point. The slider names two handlers: ValueChanged, and DragCompleted of its thumb. Then the playback buttons. Play, Pause, Resume and Stop, each with a Click handler. They all start disabled: Play until the SDK has loaded, and the other three until a file plays. Last, the settings column on the right. It holds the file name with a browse button, and a combo box for the audio output. Then a volume slider from 0 to 100, a speed slider from 0.5 to 2 in steps of 0.25, and a read-only text box for the error log. Every handler the XAML names goes into MainWindow.xaml.cs, and the next chapters write them.

### Initialize the SDK

MainWindow.xaml.cs starts with these usings: WPF namespaces for the slider thumb and the timer, Microsoft.Win32 for the file dialog, and the VisioForge namespaces. When the window loads, await VisioForgeX.InitSDKAsync once. Then create the MediaPlayerCoreX engine and pass it the VideoView, where the video is drawn. Errors arrive in the OnError event on another thread, so the handler goes back to the UI thread and adds them to the log. Audio_OutputDevicesAsync with DirectSound lists the speakers and headphones. Their names fill the combo box, and the first one is selected. Play is enabled only now, once the SDK has loaded. The browse button shows an OpenFileDialog filtered to video files, and puts the chosen path into the file name box.

### Open and play

The Play handler first disables the buttons and clears the log. AudioRendererSettings sends the sound to the selected output. Audio_OutputDevice_Volume takes 0 to 1, so the slider's value is divided by a hundred. OpenAsync takes the file path, or a URL, and returns false when the source cannot be opened. The reason arrives in OnError. Set the speed after OpenAsync, because a newly opened file always starts at normal speed. Then PlayAsync starts playback. Then check State. PlaybackState.Free means the file did not open or play, or a very short file has already ended, and the window goes back to stopped. Otherwise the buttons switch to playing, and the timer for the timeline starts.

### Pause, resume and stop

Pause and Resume are one call each: PauseAsync and ResumeAsync. Stop stops the timer, disables the buttons while it works, awaits StopAsync, and resets the window. Two small helpers set the buttons. ShowBusy disables all four, and ShowPlaying enables either Play or the other three. Stopped puts the window back: the buttons, the time label, and the timeline at zero. The flag around that last line tells the timeline's handler that this move is not the user's. OnStop fires after StopAsync, and when the file reaches its end. The handler acts only while Stop is enabled and State is Free, which is the end of the file. A late OnStop of the previous file is ignored, because the next one is already playing.

### The timeline

The timeline follows a DispatcherTimer that ticks twice a second on the UI thread. Its Tick handler is wired up in Window_Loaded. Each tick reads Position_GetAsync and DurationAsync. While the user holds the slider, the tick leaves it alone. Otherwise it updates the time label and moves the slider, with the flag set, so that this move does not seek. A click on the track seeks at once, in ValueChanged. A drag seeks once, when the thumb is released, in DragCompleted. A press on the thumb without a move does not seek. SeekAsync calls Position_SetAsync while a file is open. It works while paused, too.

### Volume and speed

The volume follows its slider at once, through Audio_OutputDevice_Volume. Rate_SetAsync changes the speed while a file plays: 1 is normal speed, 2 is double, and 0.5 is half. When nothing plays, Play applies the slider's value.

### Close the window

When the window closes, Dispose stops playback and releases the player, and DestroySDK releases the SDK. A close while the SDK loads, a file opens or playback stops is cancelled, so the player is never released in the middle of a call.

### Run it

Run the app from Visual Studio. Once the SDK has loaded, the audio outputs fill the list and Play is enabled. Click the browse button and pick the video file. Click Play. The time label shows the position and the length of the file. A click on the timeline jumps straight to that point. Or drag the thumb: playback jumps once, when you let go. The volume follows its slider, and at one and a half times the speed the film runs faster. Pause holds the picture, and Resume carries on from the same point. Stop resets the window. Then close it.

### Common mistakes

A few common mistakes. Leaving out the Libav package, so many formats do not decode. Mixing x86 and x64 packages, or running an x86 app with the x64 ones. Setting the speed before OpenAsync: the new file starts at normal speed again. Letting the timer's own moves of the slider seek, which makes playback stutter. And errors arrive in the OnError event, on another thread.

### Next steps

There are also guides for looping a file or playing only a range of it, for the same player in VB.NET, and for cross-platform players with Avalonia, .NET MAUI and Android. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Build a Video Player in VB.NET](video-player-vb-net.md) — same tutorial using VB.NET syntax
- [Avalonia Cross-Platform Player](avalonia-player.md) — build a video player for Windows, macOS, and Linux with Avalonia UI
- [.NET MAUI Player Guide](maui-player.md) — one C# codebase for iOS, Android, macOS, and Windows
- [Android Player Guide](android-player.md) — Android-specific player configuration and deployment
- [Loop Mode and Position Range](loop-and-position-range.md) — configure looping, A-B repeat, and segment playback
- [Media Blocks SDK Player](../../mediablocks/GettingStarted/player.md) — pipeline-based alternative with explicit renderers
- [Media Player SDK .NET Product Page](https://www.visioforge.com/media-player-sdk-net)
