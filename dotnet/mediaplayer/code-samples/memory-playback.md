---
title: Play Video from Memory Stream or Byte Array in C# .NET
description: Play a video from a .NET Stream or a byte array in a C# WPF app with VisioForge Media Player SDK .NET — MemoryStreamSource, ManagedIStream and Memory_DS.
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

# Play a Video from Memory in C# WPF

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

This tutorial builds a WPF player that plays a video from a .NET `Stream` instead of a file name, with [Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net) and its `MediaPlayerCore` engine. The app reads the file in one of two ways: as a `FileStream`, or as a byte array read whole into memory and wrapped in a `MemoryStream`. The same code plays any readable, seekable stream: a download copied into memory, a database blob, or a file decrypted in memory.

`MediaPlayerCore` is the SDK's DirectShow engine, so the app runs on Windows only.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/zSu_50b7IVQ?controls=1" title="Play a video from memory in C# WPF - YouTube tutorial" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf)

The film in the video is [Big Buck Bunny](https://peach.blender.org/) (c) Blender Foundation, under [CC BY 3.0](https://creativecommons.org/licenses/by/3.0/).

The video creates the project on .NET 10, which needs Visual Studio 2026; in Visual Studio 2022, choose .NET 8 or .NET 9.

## Required NuGet packages

- [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) — the SDK with `MediaPlayerCore` and the WPF `VideoView`
- [VisioForge.DotNet.Core.Redist.MediaPlayer.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MediaPlayer.x64/) — the SDK's DirectShow filters, among them the source that reads the stream
- [VisioForge.DotNet.Core.Redist.LAV.x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/) — the LAV splitter and decoders, which find the video and audio tracks in the stream and decode them

```xml
<PackageReference Include="VisioForge.DotNet.Core" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.MediaPlayer.x64" Version="[2026.11.12]" />
<PackageReference Include="VisioForge.DotNet.Core.Redist.LAV.x64" Version="[2026.11.12]" />
```

These are the x64 packages, so the project sets `<PlatformTarget>x64</PlatformTarget>`; a 32-bit app takes the `.x86` packages instead. See the [deployment guide](../deployment.md) for the full list.

## The window in XAML

The `vf` prefix maps the `VisioForge.Core.UI.WPF` namespace of the `VisioForge.Core` assembly. The `VideoView` fills the left column, with the timeline and the playback buttons under it; the right column holds the file name, two radio buttons that choose how the file is read, and the error log. Play is enabled from the start: `MediaPlayerCore` needs no SDK initialization.

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

## The code behind the window

`MainWindow.xaml.cs`. The project has `ImplicitUsings` enabled, so `System`, `System.Threading.Tasks` and the other common namespaces are not listed.

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

    private Stream _source; // the player reads it while it plays: closed only after the stop

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(500) };

    private bool _timerMovesSlider; // true while the timer, not the user, moves the timeline

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        _player = new MediaPlayerCore(VideoView1);
        _player.OnError += (_, args) => Dispatcher.InvokeAsync(() => edLog.AppendText(args.Message + Environment.NewLine));

        // Raised by StopAsync and at the end of the file; acted on only for the end of the file
        // (Stop is enabled), not for a late stop of the previous file (State is no longer Free)
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
                ? new MemoryStream(await File.ReadAllBytesAsync(edFilename.Text)) // the whole file in memory
                : new FileStream(edFilename.Text, FileMode.Open, FileAccess.Read);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            edLog.AppendText(ex.Message + Environment.NewLine);
            ShowPlaying(false);
            return;
        }

        // A seekable .NET stream, wrapped as the COM IStream the player reads; the LAV splitter
        // finds the video and audio in it
        _player.Source_MemoryStream = new MemoryStreamSource(new ManagedIStream(_source), _source.Length);
        _player.Source_Mode = MediaPlayerSourceMode.Memory_DS;

        if (!await _player.PlayAsync() || _player.State() == PlaybackState.Free)
        {
            Stopped(); // did not play (the reason is in the log), or has already ended
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
        if (!btStop.IsEnabled) return; // stopped meanwhile
        btResume.IsEnabled = paused;
        btPause.IsEnabled = !paused;
    }

    private async void btResume_Click(object sender, RoutedEventArgs e)
    {
        btResume.IsEnabled = false;
        var resumed = await _player.ResumeAsync();
        if (!btStop.IsEnabled) return; // stopped meanwhile
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

        _source?.Dispose(); // the player has let go of it
        _source = null;

        lbTime.Text = "00:00:00 / 00:00:00";
        _timerMovesSlider = true;
        slTimeline.Value = 0;
        _timerMovesSlider = false;
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        var position = await _player.Position_Get_TimeAsync();
        if (!_timer.IsEnabled || slTimeline.IsMouseCaptureWithin) return; // stopped, or the user drags

        var duration = TimeSpan.FromSeconds(slTimeline.Maximum); // read once, after PlayAsync
        lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

        _timerMovesSlider = true;
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
        if (btStop.IsEnabled) await _player.Position_Set_TimeAsync(TimeSpan.FromSeconds(slTimeline.Value));
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        _timer.Stop();
        _player.Dispose(); // stops playback; Stopped() then closes the stream, or the app's exit does
    }
}
```

## How it works

1. **The source** — with *Byte array* checked, `File.ReadAllBytesAsync` reads the whole file into memory and a `MemoryStream` wraps the array; otherwise a `FileStream` opened with `FileAccess.Read` reads the file as the player asks for it. A missing or locked file goes to the log, and the window stays stopped.
2. **Memory playback** — `ManagedIStream` (namespace `VisioForge.Core`) wraps the .NET stream as the COM `IStream` the DirectShow engine reads. `MemoryStreamSource` takes it together with the stream's length, and `Source_Mode = MediaPlayerSourceMode.Memory_DS` makes the player read from it. With the LAV package installed, the LAV splitter finds the video and audio tracks in the stream, so the two-argument constructor is enough.
3. **Play** — `PlayAsync` returns `false` when the stream cannot be played, and the reason arrives through `OnError`. `State()` returning `PlaybackState.Free` means it did not play, or a very short file has already ended. Otherwise `Duration_TimeAsync` is read once, for the timeline's scale.
4. **The stream's lifetime** — the player reads the stream for as long as it plays and seeks in it, so `Stopped()` is the one place that disposes it: after `StopAsync`, or when `OnStop` reports the end of the file. A `using` block around `PlayAsync` would close the stream while the video plays.
5. **Timeline** — a `DispatcherTimer` reads `Position_Get_TimeAsync` twice a second on the UI thread. The `_timerMovesSlider` flag tells the timer's own updates apart from the user's, so the timer never seeks. A click on the track seeks at once; a drag seeks once, in `Thumb.DragCompleted`. `Position_Set_TimeAsync` seeks inside the stream.
6. **Stop and close** — `OnStop` fires after `StopAsync` and at the end of the file, on another thread, so the handler goes back to the UI thread with `Dispatcher.InvokeAsync`, as `OnError` does. It acts only while Stop is enabled and `State()` is `PlaybackState.Free`. Closing the window calls `Dispose`, which stops playback and releases the player.

## Notes

- The stream must be readable and seekable, and its `Length` must be known: the player seeks in it to find the tracks and to follow the timeline. A network or decompression stream that cannot seek is copied into a `MemoryStream` first.
- A byte array holds the whole file in memory. For a large file, a `FileStream` reads only what the player needs.
- Without the LAV package the player cannot tell what the stream holds: use the constructor `MemoryStreamSource(IStream stream, bool? videoPresent, bool? audioPresent, long size)` and set both flags to what the stream really contains.
- `ManagedIStream` reads through any `Stream`, so a custom stream class — a decrypting stream, for example — plays the same way, as long as it is readable and seekable.

## Sample Applications

- [Memory playback tutorial WPF (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/_VideoTutorials/memory-playback-wpf) — the code of this page and the video
- [Memory Stream Demo WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Memory%20Stream)
- [Encrypted Memory Playback Demo WinForms (C#)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WinForms/CSharp/Encrypted%20Memory%20Playback%20Demo)

## Frequently Asked Questions

### Can I play a video from a byte array in C#?

Yes. Wrap the array in a `MemoryStream`, wrap that in `ManagedIStream`, and pass it to `MemoryStreamSource` with the array's length. Set `Source_Mode` to `MediaPlayerSourceMode.Memory_DS` and call `PlayAsync`. Keep the `MemoryStream` open until playback has stopped.

### Why does playback stop or fail right after it starts?

The stream is usually disposed too early, for example by a `using` block around `PlayAsync`, or the stream cannot seek. Keep the stream in a field and dispose it after `StopAsync` or at the end of the file.

### Can I play a stream downloaded from the network?

Copy it into a `MemoryStream` first. A network stream cannot seek and has no length, and the player needs both.

### Which formats play from memory?

What the LAV splitter and decoders read from a file: MP4, MKV, AVI, MOV, WMV, WebM, MPEG-TS and others. The stream holds the same bytes as the file.

## Video Transcript

### What we build

Let's play a video from memory in a C# WPF app, with the VisioForge Media Player SDK. Here's what we build. A WPF player that reads a .NET stream instead of a file name. Either a FileStream, or the whole file read into a byte array and wrapped in a MemoryStream. It has Play, Pause, Resume and Stop, and a timeline to seek. The same code plays a stream you got from anywhere else: a download, a database, or a file you decrypted in memory. This is the finished player. The film is Big Buck Bunny, by the Blender Foundation, under a Creative Commons Attribution license.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project is a WPF app on .NET 10. And a video file to play. The player here is MediaPlayerCore, the SDK's DirectShow engine, so this app runs on Windows.

### Create a WPF project

In Visual Studio, create a new project. Search for WPF, and pick WPF Application for C#. Name the project MemoryPlaybackWpf. Choose .NET 10 and click Create.

### Add the NuGet packages

Now add three NuGet packages. VisioForge.DotNet.Core holds MediaPlayerCore. Redist MediaPlayer holds the SDK's DirectShow filters, among them the source that reads the stream. And Redist LAV holds the LAV splitter and decoders, which find the video and audio in the stream and decode them. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.Core from the Browse tab. Then install the MediaPlayer and LAV redists for x64. The Installed tab lists all three. These are the x64 packages, so set PlatformTarget to x64 in the project file. A 32-bit app takes the x86 packages instead.

### Build the window in XAML

The window is written in XAML. First the window itself, with a grid and a VideoView, and the designer draws it right away. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. The grid has two columns, the video and a 260-pixel settings column, and three rows: the video, the timeline and the buttons. The VideoView fills the first cell. The window also names its Loaded and Closing handlers. Under the video goes the timeline. A slider for the position, with the time on its right. IsMoveToPointEnabled makes a click on the track jump straight to that point. The slider names two handlers: ValueChanged, and DragCompleted of its thumb. Then the playback buttons. Play, Pause, Resume and Stop, each with a Click handler. Play is enabled from the start, because this engine needs no SDK initialization. The other three start disabled, until a file plays. Last, the settings column on the right. It holds the file name with a browse button, and two radio buttons that choose how the app reads the file: as a FileStream, or as a byte array in a MemoryStream. Stream is checked at first. Then a read-only text box for the error log. Every handler the XAML names goes into MainWindow.xaml.cs, and the next chapters write them.

### Create the player

MainWindow.xaml.cs starts with these usings: System.IO for the streams, WPF namespaces for the slider thumb and the timer, Microsoft.Win32 for the file dialog, and the VisioForge namespaces. The window keeps the player, the stream it plays, a timer for the timeline, and a flag. When the window loads, create MediaPlayerCore and pass it the VideoView, where the video is drawn. Errors arrive in the OnError event on another thread, so the handler goes back to the UI thread and adds them to the log. OnStop fires after StopAsync, and when the file reaches its end. The handler acts only while Stop is enabled and State is Free, which is the end of the file. Then Window_Loaded wires up the timer's Tick handler. The browse button shows an OpenFileDialog filtered to video files, and puts the chosen path into the file name box.

### Play from memory

The Play handler first disables the buttons and clears the log. Then it opens the source. With Byte array checked, ReadAllBytesAsync reads the whole file into memory, and a MemoryStream wraps the array. Otherwise a FileStream reads the file as the player needs it. A file that is missing or locked goes to the log, and the window stays stopped. This is the part that plays from memory. ManagedIStream wraps the .NET stream as the COM IStream the DirectShow engine reads. MemoryStreamSource takes it with the stream's length, and Source_Mode set to Memory_DS makes the player read from it. The stream must be readable and seekable. The LAV splitter finds the video and audio tracks in it, so you do not tell the player what the stream holds. PlayAsync returns false when the stream cannot be played, and the reason arrives in OnError. State Free means it did not play, or a very short file has already ended, and the window goes back to stopped. Otherwise read the duration once, for the timeline's scale, switch the buttons to playing, and start the timer.

### Pause, resume and stop

Pause disables itself, awaits PauseAsync, and enables Resume when the player has paused. If playback stopped in the meantime, it leaves the buttons alone. Resume is the same, with ResumeAsync. Stop stops the timer, disables the buttons while it works, awaits StopAsync, and resets the window. Two small helpers set the buttons. ShowBusy disables all four. ShowPlaying enables either Play, or Pause and Stop. Stopped puts the window back, and it is the one place that closes the stream. The player reads the stream for as long as it plays, so dispose it only once the player has stopped, never right after PlayAsync. Then the time label and the timeline go back to zero.

### The timeline

The timer ticks twice a second on the UI thread. Each tick reads Position_Get_TimeAsync. While the user holds the slider, the tick leaves it alone. Otherwise it updates the time label and moves the slider, with the flag set, so that this move does not seek. A click on the track seeks at once, in ValueChanged. A drag seeks once, when the thumb is released, in DragCompleted. Both call Position_Set_TimeAsync while a file plays. Seeking works on a memory stream too, because the stream is seekable.

### Close the window

When the window closes, Dispose stops playback and releases the player. The stream is closed by Stopped, or with the app.

### Run it

Run the app from Visual Studio. Click the browse button and pick the video file. Stream is checked, so Play reads the file through a FileStream. The time label shows the position and the length. A click on the timeline seeks inside the stream. Pause holds the picture, Resume carries on from the same point, and Stop resets the window. Now check Byte array and click Play. The app reads the whole file into memory first, and the player plays it from the MemoryStream, with no file access while it plays. Stop, and close the window.

### Common mistakes

A few common mistakes. Disposing the stream while the player still reads it, for example with a using block around PlayAsync: close it only after the stop. A stream that cannot seek, such as a network stream: the player needs to seek and to know the length, so copy it into a MemoryStream first. Leaving out the LAV package: without it the player cannot tell what the stream holds. A byte array for a huge file, which takes that much memory, so prefer a FileStream there. And mixing the x86 and x64 packages.

### Next steps

There are also guides for a full video player with volume and speed, for playing only a fragment of a file, for showing the first frame of a video, and for deploying the redists with your app. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Build a Video Player in C#](../guides/video-player-csharp.md) — a full player with volume, speed and the audio output
- [Play a Fragment of a File](play-fragment-file.md) — play only a range of a media file
- [Show the First Frame of a Video](show-first-frame.md) — display a video's first frame as a preview
- [Deployment Guide](../deployment.md) — the NuGet packages and installers the app needs
- [Media Player SDK .NET Product Page](https://www.visioforge.com/media-player-sdk-net)
