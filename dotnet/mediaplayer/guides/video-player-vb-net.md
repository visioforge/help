---
title: VB.NET Video Player with Playback Controls and Seeking
description: Build a video player in VB.NET with playback controls, seeking, and volume adjustment. Complete code examples using VisioForge Media Player SDK .NET.
tags:
  - Media Player SDK
  - .NET
  - DirectShow
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
  - Webcam
  - MPEG-DASH
  - MP4
  - MKV
  - WebM
  - AVI
  - MOV
  - WMV
  - H.265
  - AAC
  - MP3
  - FLAC
  - WAV
  - WMA
  - VB.NET
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - MediaPlayerCore

---

# Build a Video Player in VB.NET

This guide walks you through building a full-featured video player application in VB.NET (Visual Basic .NET) using [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net). The player supports MP4, AVI, MKV, WMV, WebM, and many other formats with playback controls, timeline seeking, and volume adjustment.

## Why Use Media Player SDK .Net for VB.NET

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) provides robust VB.NET video playback with:

- Support for all major video formats (MP4, AVI, MKV, WMV, WebM, MOV, TS)
- Audio format support (MP3, AAC, WAV, WMA, FLAC)
- Hardware-accelerated video decoding
- Timeline seeking and position tracking
- Volume and playback speed control
- WinForms and WPF UI integration

## Required NuGet Packages

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Complete VB.NET Video Player Example

### SDK Initialization and Engine Setup

Initialize the SDK and create the player engine when the form loads:

```vb.net
Imports VisioForge.Core
Imports VisioForge.Core.MediaPlayerX
Imports VisioForge.Core.Types.Events
Imports VisioForge.Core.Types.X.AudioRenderers
Imports VisioForge.Core.Types.X.Sources

Public Class Form1
    Private WithEvents _timer As System.Timers.Timer
    Private _timerFlag As Boolean
    Private _player As MediaPlayerCoreX

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize the SDK
        Await VisioForgeX.InitSDKAsync()

        ' Set up a timer for timeline updates
        _timer = New System.Timers.Timer(500)
        AddHandler _timer.Elapsed, AddressOf _timer_Elapsed

        ' Create the player engine with the VideoView control
        CreateEngine()

        ' Enumerate audio output devices
        For Each device In Await _player.Audio_OutputDevicesAsync(AudioOutputDeviceAPI.DirectSound)
            cbAudioOutput.Items.Add(device.Name)
        Next

        If cbAudioOutput.Items.Count > 0 Then
            cbAudioOutput.SelectedIndex = 0
        End If
    End Sub

    Private Sub CreateEngine()
        _player = New MediaPlayerCoreX(VideoView1)
        AddHandler _player.OnError, AddressOf Player_OnError
        AddHandler _player.OnStop, AddressOf Player_OnStop
    End Sub
```

### Opening and Playing a Video File

Select a file and start playback:

```vb.net
    Private Sub btSelectFile_Click(sender As Object, e As EventArgs) Handles btSelectFile.Click
        Dim ofd As New OpenFileDialog()
        ofd.Filter = "Video Files|*.mp4;*.ts;*.mts;*.mov;*.avi;*.mkv;*.wmv;*.webm" &
                     "|Audio Files|*.mp3;*.aac;*.wav;*.wma|All Files|*.*"
        If ofd.ShowDialog() = DialogResult.OK Then
            edFilename.Text = ofd.FileName
        End If
    End Sub

    Private Async Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        edLog.Clear()

        ' Destroy previous engine instance if exists
        Await DestroyEngineAsync()
        CreateEngine()

        ' Set audio output device
        Dim audioOutputDevice = (Await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound)).First(
            Function(x) x.Name = cbAudioOutput.Text)
        _player.Audio_OutputDevice = New AudioRendererSettings(audioOutputDevice)

        ' Open and play the file
        Dim source = Await UniversalSourceSettings.CreateAsync(New Uri(edFilename.Text))
        Await _player.OpenAsync(source)
        Await _player.PlayAsync()

        _timer.Start()
    End Sub
```

### Playback Controls (Pause, Resume, Stop)

Implement standard media player controls:

```vb.net
    Private Async Sub btPause_Click(sender As Object, e As EventArgs) Handles btPause.Click
        Await _player.PauseAsync()
    End Sub

    Private Async Sub btResume_Click(sender As Object, e As EventArgs) Handles btResume.Click
        Await _player.ResumeAsync()
    End Sub

    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        _timer.Stop()

        If _player IsNot Nothing Then
            Await _player.StopAsync()
            Await DestroyEngineAsync()
            _player = Nothing
        End If

        tbTimeline.Value = 0
    End Sub
```

### Timeline Seeking and Position Tracking

Update the timeline slider as the video plays and allow seeking:

```vb.net
    Private Async Sub _timer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs)
        _timerFlag = True

        If _player Is Nothing Then Return

        Dim position = Await _player.Position_GetAsync()
        Dim duration = Await _player.DurationAsync()

        Invoke(Sub()
                   tbTimeline.Maximum = CInt(duration.TotalSeconds)
                   lbTime.Text = position.ToString("hh\:mm\:ss") & " | " &
                                 duration.ToString("hh\:mm\:ss")

                   If tbTimeline.Maximum >= position.TotalSeconds Then
                       tbTimeline.Value = CInt(position.TotalSeconds)
                   End If
               End Sub)

        _timerFlag = False
    End Sub

    Private Async Sub tbTimeline_Scroll(sender As Object, e As EventArgs) Handles tbTimeline.Scroll
        If Not _timerFlag AndAlso _player IsNot Nothing Then
            Await _player.Position_SetAsync(TimeSpan.FromSeconds(tbTimeline.Value))
        End If
    End Sub
```

### Volume and Playback Speed Control

```vb.net
    Private Sub tbVolume1_Scroll(sender As Object, e As EventArgs) Handles tbVolume1.Scroll
        If _player IsNot Nothing Then
            _player.Audio_OutputDevice_Volume = tbVolume1.Value / 100.0
        End If
    End Sub

    Private Async Sub tbSpeed_Scroll(sender As Object, e As EventArgs) Handles tbSpeed.Scroll
        Await _player.Rate_SetAsync(tbSpeed.Value / 10.0)
    End Sub
```

### Error Handling and Cleanup

```vb.net
    Private Sub Player_OnError(sender As Object, e As ErrorsEventArgs)
        Invoke(Sub()
                   edLog.Text = edLog.Text + e.Message + Environment.NewLine
               End Sub)
    End Sub

    Private Sub Player_OnStop(sender As Object, e As StopEventArgs)
        Invoke(Sub()
                   tbTimeline.Value = 0
               End Sub)
    End Sub

    Private Async Function DestroyEngineAsync() As Task
        If _player IsNot Nothing Then
            RemoveHandler _player.OnError, AddressOf Player_OnError
            RemoveHandler _player.OnStop, AddressOf Player_OnStop
            Await _player.DisposeAsync()
            _player = Nothing
        End If
    End Function

    Private Async Sub Form1_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        _timer.Stop()
        Await DestroyEngineAsync()
        VisioForgeX.DestroySDK()
    End Sub
End Class
```

## Supported Video and Audio Formats

The VB.NET video player supports all major formats:

| Category | Formats |
|----------|---------|
| Video containers | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS |
| Video codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Audio containers | MP3, AAC, WAV, WMA, FLAC, OGG |
| Streaming | RTSP, HTTP, HLS |

## Sample Applications

Complete VB.NET video player demos:

- [Simple Player Demo VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Simple%20Player%20Demo%20X%20VB) — full-featured video player with all playback controls

## Frequently Asked Questions

### What license do I need for a VB.NET video player application?

Media Player SDK .Net requires a license for development and distribution. A Developer license removes the evaluation watermark and unlocks all features during development. A Release license is required when distributing your application to end users. The SDK is available in Premium edition which includes all supported formats, hardware acceleration, and both DirectShow and GStreamer engines. You can evaluate the SDK without a license — playback works fully but includes a watermark overlay. Visit the [product page](https://www.visioforge.com/media-player-sdk-net) for pricing and license options.

### Which video and audio formats does the VB.NET player support?

The player supports all formats listed in the table above, including MP4, AVI, MKV, WMV, WebM, MOV, and TS containers with H.264, H.265/HEVC, VP8, VP9, AV1, and MPEG-2 video codecs. Audio formats include MP3, AAC, WAV, WMA, FLAC, and OGG. Network streaming protocols such as RTSP, HTTP, HLS, and MPEG-DASH are also supported. Format support is identical to the C# version because both languages use the same GStreamer-based playback engine.

### Should I use DirectShow or GStreamer engine for my VB.NET video player?

The `MediaPlayerCoreX` API shown in this guide uses the GStreamer engine and is the recommended choice for new cross-platform projects (Windows, macOS, Linux, iOS, Android), with the broadest format coverage and hardware-accelerated decoding. The DirectShow engine (`MediaPlayerCore`) is Windows-only but is a fully supported, first-class engine — pick it when your application is Windows-only and you want the lower per-frame overhead and the mature DirectShow filter ecosystem. Both engines work identically from VB.NET code; for new cross-platform work start with `MediaPlayerCoreX`, and for Windows-only deployments either engine is a good fit.

### How do I deploy a VB.NET video player application?

Include the NuGet redist packages listed in the NuGet reference section above — these bundle the native GStreamer libraries your application needs. Use `dotnet publish` with a self-contained deployment to avoid requiring a separate runtime install on the target machine. The `VisioForge.CrossPlatform.Core.Windows.x64` package contains the GStreamer runtime, so no separate GStreamer installation is needed. For WPF projects, the same deployment approach works — just reference the WPF-specific VideoView control instead of the WinForms one.

## See Also

- [Build a Video Player in C#](video-player-csharp.md) — WinForms and WPF player with DirectShow and GStreamer engines
- [.NET MAUI Video Player](maui-player.md) — iOS, Android, macOS, and Windows from one C# codebase
- [Avalonia Player Guide](avalonia-player.md) — complete MVVM implementation with file dialogs and platform setup
- [Loop Mode & Position Range](loop-and-position-range.md) — loop playback and segment selection for both engines
- [Record Webcam Video in VB.NET](../../videocapture/guides/record-webcam-vb-net.md) — webcam capture application in Visual Basic .NET
- [Code Samples](../code-samples/index.md) — frame extraction, playlists, and reverse playback examples
- [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net) — product page and downloads
