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

### How do I play a Blu-ray disc?

Set `Source_Mode` to `MediaPlayerSourceMode.BluRay` and open the disc's `BDMV\index.bdmv`. The player plays the largest `.m2ts` file in the disc's `BDMV\STREAM` folder, which is the main movie on most discs. It uses the same decoders as the FFMPEG mode, so the app needs the `VisioForge.DotNet.Core.Redist.FFMPEG.x64` package (`.x86` for 32-bit apps).

```csharp
player.Source_Mode = MediaPlayerSourceMode.BluRay;
player.Playlist_Add(@"E:\BDMV\index.bdmv");
await player.PlayAsync();
```

Not supported:

- ISO images. Mount the image in Windows and open `BDMV\index.bdmv` on the new drive.
- Playlist files (`.mpls`). Open `BDMV\index.bdmv` instead.
- Titles split over several clips. Only the largest clip plays.
- Disc menus.
- Encrypted (AACS) discs, which includes most commercial movie discs.

An ISO image, a playlist file, an encrypted disc, a disc without a `BDMV\STREAM` folder or a disc that cannot be read does not play: `PlayAsync` returns `false` and `OnError` names the reason.

## Hardware decoding

### Which hardware decoder should the FFMPEG source engine use?

With `Source_Mode` set to `MediaPlayerSourceMode.FFMPEG`, set `Source_FFMPEG_HWDecoder` before `PlayAsync`. For IP cameras in Video Capture SDK .NET, the same choice is `FFMPEG_HWDecoder` in `IPCameraSourceSettings` with the `Auto_FFMPEG` engine. `DXVA2Native` is not available there.

```csharp
player.Source_Mode = MediaPlayerSourceMode.FFMPEG;
player.Source_FFMPEG_HWDecoder = FFMPEGHWDecoder.Direct3D11;
```

| Value | Use it when |
|---|---|
| `Auto` (default) | You want the behaviour of earlier versions: software decoding. |
| `Software` | The video must be decoded on the CPU, for example to work around a driver problem. |
| `NvidiaCUDA` | The computer has an NVIDIA GPU. |
| `IntelQuickSync` | The computer has Intel graphics. |
| `Direct3D11` | Any GPU. A good first choice when you do not know the GPU vendor. |
| `DXVA2CopyBack` | Any GPU, including older drivers without Direct3D 11 video decoding. |
| `DXVA2Native` | The lowest CPU use, with EVR or madVR and without video effects or the sample grabber. |

`NvidiaCUDA`, `IntelQuickSync`, `Direct3D11` and `DXVA2CopyBack` copy the decoded frames back to system memory, so every renderer, video effect and the sample grabber work with them.

Every value except `Auto` is strict. When the computer has no such GPU, the GPU cannot decode the video (codec, profile, bit depth or frame size), or `DXVA2Native` has no renderer that accepts its surfaces, playback does not start and `OnError` names the decoder and the reason. The SDK never falls back to software decoding silently: handle `OnError` and choose another decoder, or `Auto`.
