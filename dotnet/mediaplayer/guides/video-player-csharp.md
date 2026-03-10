---
title: C# Video Player: Build with Seeking and Volume Controls
description: Build a video player in C# with VisioForge Media Player SDK .NET. Step-by-step guide covering playback controls, seeking, and volume adjustment.
---

# Build a Video Player in C#

This guide shows you how to build a full-featured video player in C# using [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net). You will implement file playback, timeline seeking, pause/resume, and volume control. Two approaches are covered: the high-level `MediaPlayerCoreX` API and the pipeline-based `MediaBlocksPipeline` API.

## MediaPlayerCoreX Approach (Recommended)

`MediaPlayerCoreX` is the simplest way to build a video player in C# with full playback control.

### Required NuGet Packages

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

### Complete C# Video Player Implementation

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

public partial class Form1 : Form
{
    private MediaPlayerCoreX _player;
    private System.Timers.Timer _timer;
    private bool _timerFlag;

    private async void Form1_Load(object sender, EventArgs e)
    {
        // Initialize the SDK
        await VisioForgeX.InitSDKAsync();

        _timer = new System.Timers.Timer(500);
        _timer.Elapsed += Timer_Elapsed;

        // Create the player engine with the VideoView control
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;
        _player.OnStop += Player_OnStop;

        // Populate audio output devices
        foreach (var device in await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound))
        {
            cbAudioOutput.Items.Add(device.Name);
        }

        if (cbAudioOutput.Items.Count > 0)
            cbAudioOutput.SelectedIndex = 0;
    }
```

### Opening and Playing a Video File

```csharp
    private void btOpenFile_Click(object sender, EventArgs e)
    {
        var ofd = new OpenFileDialog
        {
            Filter = "Video Files|*.mp4;*.avi;*.mkv;*.wmv;*.webm;*.mov;*.ts" +
                     "|Audio Files|*.mp3;*.aac;*.wav;*.wma|All Files|*.*"
        };

        if (ofd.ShowDialog() == DialogResult.OK)
            edFilename.Text = ofd.FileName;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        // Set audio output device
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        var audioDevice = devices.First(x => x.Name == cbAudioOutput.Text);
        _player.Audio_OutputDevice = new AudioRendererSettings(audioDevice);

        // Open the media file
        var source = await UniversalSourceSettings.CreateAsync(
            new Uri(edFilename.Text));
        await _player.OpenAsync(source);

        // Start playback
        await _player.PlayAsync();

        _timer.Start();
    }
```

### Pause, Resume, and Stop

```csharp
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

        if (_player != null)
        {
            await _player.StopAsync();
        }

        tbTimeline.Value = 0;
    }
```

### Timeline Seeking

```csharp
    private async void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
    {
        _timerFlag = true;

        if (_player == null) return;

        var position = await _player.Position_GetAsync();
        var duration = await _player.DurationAsync();

        Invoke(() =>
        {
            tbTimeline.Maximum = (int)duration.TotalSeconds;
            lbTime.Text = $"{position:hh\\:mm\\:ss} / {duration:hh\\:mm\\:ss}";

            if (tbTimeline.Maximum >= position.TotalSeconds)
                tbTimeline.Value = (int)position.TotalSeconds;
        });

        _timerFlag = false;
    }

    private async void tbTimeline_Scroll(object sender, EventArgs e)
    {
        if (!_timerFlag && _player != null)
        {
            await _player.Position_SetAsync(
                TimeSpan.FromSeconds(tbTimeline.Value));
        }
    }
```

### Volume and Speed Control

```csharp
    private void tbVolume_Scroll(object sender, EventArgs e)
    {
        if (_player != null)
            _player.Audio_OutputDevice_Volume = tbVolume.Value / 100.0;
    }

    private async void tbSpeed_Scroll(object sender, EventArgs e)
    {
        await _player.Rate_SetAsync(tbSpeed.Value / 10.0);
    }
```

### Error Handling and Cleanup

```csharp
    private void Player_OnError(object sender, ErrorsEventArgs e)
    {
        Invoke(() => edLog.Text += e.Message + Environment.NewLine);
    }

    private void Player_OnStop(object sender, StopEventArgs e)
    {
        Invoke(() => tbTimeline.Value = 0);
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        _timer.Stop();

        if (_player != null)
        {
            _player.OnError -= Player_OnError;
            _player.OnStop -= Player_OnStop;
            await _player.DisposeAsync();
        }

        VisioForgeX.DestroySDK();
    }
}
```

## MediaBlocksPipeline Approach

For maximum flexibility, use the `MediaBlocksPipeline` to build a player with custom processing blocks.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.AudioRendering;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

private MediaBlocksPipeline _pipeline;

private async void StartPlayback(string filePath)
{
    _pipeline = new MediaBlocksPipeline();

    // Create file source and detect available streams
    var fileSourceSettings = await UniversalSourceSettings.CreateAsync(filePath);
    var hasVideo = fileSourceSettings.GetInfo().VideoStreams.Count > 0;
    var hasAudio = fileSourceSettings.GetInfo().AudioStreams.Count > 0;

    var fileSource = new UniversalSourceBlock(fileSourceSettings);

    // Connect video renderer if video is available
    if (hasVideo)
    {
        var videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
        _pipeline.Connect(fileSource, videoRenderer);
    }

    // Connect audio renderer if audio is available
    if (hasAudio)
    {
        var audioOutputDevice = (await DeviceEnumerator.Shared
            .AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound))[0];
        var audioOutput = new AudioRendererBlock(
            new AudioRendererSettings(audioOutputDevice));
        _pipeline.Connect(fileSource, audioOutput);
    }

    // Start playback
    await _pipeline.StartAsync();
}
```

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

Yes. The `UniversalSourceSettings.CreateAsync` method accepts URIs for RTSP, HTTP, HLS, and MPEG-DASH streams. Pass the stream URL as a `Uri` object just as you would a local file path. For RTSP sources that require authentication, include the credentials directly in the URI (e.g., `rtsp://user:password@host:554/stream`).

### How do I control playback speed in the video player?

Call `Rate_SetAsync(double rate)` on the player instance. A rate of 1.0 is normal speed, 2.0 is double speed, and 0.5 is half speed. The supported range depends on the media format, but most files support rates between 0.25x and 4.0x. The rate can be changed during playback without stopping the video.

### Does the SDK support subtitle rendering?

Yes. The SDK can render embedded subtitles from MKV and MP4 containers as well as external SRT and ASS subtitle files. Subtitle tracks are detected automatically when a file is opened, and you can select which track to display through the player API.

### Can I build a cross-platform video player with this SDK?

Yes. The SDK runs on Windows, macOS, Linux, Android, and iOS. For cross-platform desktop applications, use the [Avalonia UI framework](avalonia-player.md) with the same `MediaPlayerCoreX` API. The core playback engine is identical across platforms — only the video rendering surface and NuGet packages differ.

## See Also

- [Build a Video Player in VB.NET](video-player-vb-net.md) — same tutorial using VB.NET syntax
- [Avalonia Cross-Platform Player](avalonia-player.md) — build a video player for Windows, macOS, and Linux with Avalonia UI
- [Play Video in .NET (Cross-Platform Guide)](play-video-dotnet.md) — overview of all supported UI frameworks and platforms
- [Loop Mode and Position Range](loop-and-position-range.md) — configure looping, A-B repeat, and segment playback
- [MediaBlocks Pipeline Getting Started](../../mediablocks/GettingStarted/index.md) — learn the pipeline-based approach for advanced media processing
- [Media Player SDK .Net Product Page](https://www.visioforge.com/media-player-sdk-net)
