---
title: Video Player Code Examples and Tutorials in C# .NET
description: Implement playback, frame extraction, playlists, and streaming with VisioForge Media Player SDK .NET. WinForms, WPF, and Console samples included.
sidebar_label: Code Examples
tags:
  - Media Player SDK
  - .NET
  - Windows
  - Playback
  - Streaming

---

# .NET Media Player SDK Implementation Examples

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

This page collects ready-to-use C# recipes for the most common playback scenarios using the Media Player SDK .Net. Each snippet is verified against the SDK source and demos under `_SOURCE/_DEMOS/Media Player SDK/`. The Windows-only `MediaPlayerCore` engine is used in the examples below; cross-platform code based on `MediaPlayerCoreX` follows the same overall shape with engine-specific source settings.

## Available Recipes

### Basic Playback

- Open a local file and start playback
- Pause, resume, and stop a running player
- Adjust the output volume

### Streaming

- Play a network stream (HTTP / RTSP) by URL
- Switch source engine to the FFmpeg backend for live streams

### Effects and Processing

- Capture the current frame as a `Bitmap`
- Seek to a specific position and read playback time

## Recipe — Simple Player

The following class wraps `MediaPlayerCore` with start/stop calls. The constructor receives an `IVideoView` instance (`VideoView` for WPF, `VideoViewWinForms` for WinForms, etc.) that is provided by your UI layer.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Events;

public class SimplePlayer
{
    private readonly MediaPlayerCore _player;

    public SimplePlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);

        // Subscribe to events.
        _player.OnStop += OnPlaybackStopped;
        _player.OnError += OnPlaybackError;
    }

    public async Task PlayFileAsync(string filePath)
    {
        _player.Playlist_Clear();
        _player.Playlist_Add(filePath);
        await _player.PlayAsync();
    }

    public Task StopAsync() => _player.StopAsync();

    private void OnPlaybackStopped(object sender, StopEventArgs e)
    {
        Console.WriteLine("Playback finished.");
    }

    private void OnPlaybackError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

## Recipe — Player With Pause and Volume Controls

`MediaPlayerCore` exposes `PauseAsync`/`ResumeAsync` for in-flight playback control and a per-output-device volume API (`Audio_OutputDevice_Volume_Set`) that accepts an integer in the 0–100 range.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;

public class ControllablePlayer
{
    private readonly MediaPlayerCore _player;
    private bool _isPaused;

    public ControllablePlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);
    }

    public async Task PlayPauseAsync()
    {
        if (_isPaused)
        {
            await _player.ResumeAsync();
            _isPaused = false;
        }
        else
        {
            await _player.PauseAsync();
            _isPaused = true;
        }
    }

    public Task SeekAsync(TimeSpan position) =>
        _player.Position_Set_TimeAsync(position);

    public void SetVolume(int volume)
    {
        // volume: 0 (mute) … 100 (full) for the first audio output device.
        _player.Audio_OutputDevice_Volume_Set(0, volume);
    }

    public Task<TimeSpan> GetDurationAsync() => _player.Duration_TimeAsync();

    public Task<TimeSpan> GetPositionAsync() => _player.Position_Get_TimeAsync();
}
```

## Recipe — Capture the Current Frame

`Frame_GetCurrent()` returns the most recently rendered frame as a `System.Drawing.Bitmap`. Save it via the `Bitmap.Save` API or use `Frame_Save` for built-in disk output with `ImageFormat`.

```csharp
using System.Drawing.Imaging;
using System.Threading.Tasks;

public Task<bool> SaveCurrentFrameAsync(MediaPlayerCore player, string outputPath)
{
    // Built-in helper: encodes the current frame to PNG (or any ImageFormat).
    return player.Frame_SaveAsync(outputPath, ImageFormat.Png);
}
```

## Recipe — Play a Network Stream

To consume RTSP, RTMP, HTTP, UDP, or TCP URLs through the FFmpeg back-end, switch `Source_Mode` to `MediaPlayerSourceMode.FFMPEG` and add the URL to the playlist like any other source.

```csharp
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.MediaPlayer;

public async Task PlayRtspAsync(MediaPlayerCore player, string url)
{
    player.Source_Mode = MediaPlayerSourceMode.FFMPEG;

    player.Playlist_Clear();
    player.Playlist_Add(url);

    await player.PlayAsync();
}
```

## Recipe — Playlist Navigation

The built-in playlist API tracks the current index for you. `Playlist_PlayNext` advances to the next item; `OnStop` fires on natural end-of-stream, where you can chain the next track.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types.Events;

public class PlaylistPlayer
{
    private readonly MediaPlayerCore _player;

    public PlaylistPlayer(IVideoView videoView)
    {
        _player = new MediaPlayerCore(videoView);
        _player.OnStop += OnTrackFinished;
    }

    public void AddToPlaylist(string filePath) => _player.Playlist_Add(filePath);

    public Task PlayAsync() => _player.PlayAsync();

    public Task<bool> NextAsync() => _player.Playlist_PlayNextAsync();

    private async void OnTrackFinished(object sender, StopEventArgs e)
    {
        // Auto-advance to the next playlist entry on natural EOS.
        if (e.Successful)
        {
            await _player.Playlist_PlayNextAsync();
        }
    }
}
```

## Featured Implementation Examples

### Video Processing Examples

- [How to get a specific frame from a video file?](get-frame-from-video-file.md)
- [How to play a fragment of the source file?](play-fragment-file.md)
- [How to show the first frame?](show-first-frame.md)

### Advanced Playback Examples

- [Memory playback implementation](memory-playback.md)
- [Playlist API integration](playlist-api.md)
- [Previous frame and reverse video playback](reverse-video-playback.md)

---

## Additional Resources

For a more extensive collection of code examples and implementation scenarios, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
