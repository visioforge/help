---
title: Media Player SDK .NET FAQ — Streams, Seeking, Speed, WPF
description: Answers to common Media Player SDK .NET questions — network URLs, frames after seeking, speed and frame stepping, synchronized start, WPF and karaoke.
sidebar_label: FAQ
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - Windows
  - Playback
  - RTSP
  - WPF
  - Troubleshooting
primary_api_classes:
  - MediaPlayerCore
  - MediaPlayerCoreX
  - CDGSourceSettings
  - MediaInfoReader
  - MediaInfoReaderX

---

# Media Player SDK .NET FAQ

Answers to the questions developers ask most often about Media Player SDK .NET, for both engines: `MediaPlayerCore` (Windows, DirectShow) and `MediaPlayerCoreX` (cross-platform). Deployment errors are collected in [Troubleshooting](../general/troubleshooting.md).

## Network sources

### "Unable to load source file or stream" — a network stream does not open

`MediaPlayerCore` reports this message, or `Unable to set URL to network VLC source filter` with the `HTTP_RTSP_VLC` source mode, when the source engine could not open the URL. Check, in this order:

1. The URL is the address of the stream itself (for example `rtsp://host:554/path` or `http://host/stream.m3u8`), not of a web page or just the IP address of the camera.
2. The same URL opens in VLC on the same computer. If it does not, the problem is the URL, the network or a firewall, not the player.
3. Credentials are included: `rtsp://login:password@host:port/path`.
4. The port is reachable from the computer that runs the application, not only from the development machine.

For IP cameras that need more than a URL, such as ONVIF discovery or a low-latency connection, see [Video Capture SDK FAQ](../videocapture/faq.md#ip-cameras).

## Seeking, speed and frames

### Frame_GetCurrent returns the previous frame right after a seek

`Position_Set_Time` returns when the seek is requested, before the new frame has been decoded and rendered, so an immediate `Frame_GetCurrent()` still sees the old one. Take the frame from the `OnVideoFrameBitmap` event after the seek instead: set a flag when you seek, and keep a copy of `e.Frame` (`new Bitmap(e.Frame)`) the first time the handler runs after that; the event's own bitmap belongs to the SDK. To read frames from a file without playing it, see [Get a frame from a video file](code-samples/get-frame-from-video-file.md).

### How do I play faster or slower, step by frame or play backwards?

With `MediaPlayerCore`:

- `SetSpeedAsync(2.0)` plays at double speed; values below 1.0 slow it down.
- `NextFrame()` and `PreviousFrame()` pause playback and step one frame.
- Continuous backward playback uses the reverse playback mode — see [Reverse video playback](code-samples/reverse-video-playback.md).

With `MediaPlayerCoreX`, use `Rate_SetAsync(rate)` and `NextFrame()`.

Stepping backwards is expensive with formats that use key frames (H.264, HEVC and most others): the decoder has to go back to the previous key frame and decode forward again. Seeking with `Position_Set_Time` to an earlier position is the general way to go back.

### How do I start playback at an exact moment, or several players at once?

Preload the file and start it later. `PlayAsync(true)` builds and prepares the playback graph without starting it; `PlayDelayed()` then starts playback with almost no delay:

```csharp
await player1.PlayAsync(true);
await player2.PlayAsync(true);

// at the moment you need
player1.PlayDelayed();
player2.PlayDelayed();
```

This is the way to synchronize video with an external clock or audio track on Windows. Media Player SDK .NET supports WinForms, WPF, WinUI 3, MAUI, Avalonia and Uno applications; UWP apps are not supported.

## WPF

### Can I put WPF controls over the video, or capture it with a VisualBrush?

Yes. The WPF `VideoView` is a WPF element, not a hosted WinForms window, so it takes part in WPF layout, transforms and z-order when it renders in a composable mode. Use the D3D11 composable renderer for GPU rendering, or the software mode. Only the native window renderers draw over everything — see [WPF controls over video](../general/code-samples/wpf-controls-over-video.md).

## Files and formats

### How do I play karaoke CDG files (MP3+G or ZIP) with pitch shifting?

`MediaPlayerCoreX` opens them with `CDGSourceSettings`, for a CDG and audio file pair or a ZIP archive that contains both:

```csharp
var settings = new CDGSourceSettings("song.zip");
settings.EnablePitchShifting = true;
settings.PitchSemitones = -2;
await player.OpenAsync(settings);
await player.PlayAsync();
```

`Rate_SetAsync` changes the tempo of karaoke files too. The [Karaoke Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WinForms/Karaoke%20Demo) shows a complete karaoke player.
