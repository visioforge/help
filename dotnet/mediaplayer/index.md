---
title: Video Player API for C# .NET — Play, Stream, Embed
description: Embed video and audio playback into your app with VisioForge Media Player SDK .NET. Supports MP4, MKV, RTSP, HLS across Windows, macOS, Linux, and mobile.
sidebar_label: Media Player SDK .NET
order: 13
tags:
  - Media Player SDK
  - .NET
  - DirectShow
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - Streaming
primary_api_classes:
  - UniversalSourceSettings
  - MediaPlayerCoreX
  - VideoView

---

# Media Player SDK for C# .NET — Video Player, Audio Player, and Streaming API

[Media Player SDK .NET](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The Media Player SDK for .NET is a C# video player API that lets you play video and audio files, network streams (RTSP, HLS, MPEG-DASH), and special content like 360-degree videos in your .NET applications. It replaces low-level DirectShow playback code and Windows Media Player SDK integration with a modern async API — open a file, start playback, and control seeking and volume in a few lines of C#.

The SDK uses GStreamer-based decoding with hardware acceleration and runs on Windows, macOS, Linux, Android, and iOS. Whether you need to build a desktop video player, embed media playback in a kiosk app, or stream RTSP feeds from IP cameras, the API covers it.

## Quick Start

### 1. Install NuGet Packages

```bash
dotnet add package VisioForge.DotNet.MediaPlayer
```

Add platform-specific native dependencies:

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

For the full list of packages and UI framework support (WinForms, WPF, MAUI, Avalonia), see the [Installation Guide](../install/index.md).

### 2. Initialize the SDK

Call `InitSDKAsync()` once at application startup before using any playback functionality:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

### 3. Play Video in C# (Minimal Example)

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Create player instance linked to a VideoView control
var player = new MediaPlayerCoreX(videoView);

// Open a video file
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("C:\\Videos\\sample.mp4"));
await player.OpenAsync(source);

// Start playback
await player.PlayAsync();

// ... when done:
await player.StopAsync();
await player.DisposeAsync();
```

### 4. Cleanup at Shutdown

```csharp
VisioForgeX.DestroySDK();
```

## Core Workflow

Every media player application follows the same pattern:

1. **Initialize SDK** — `VisioForgeX.InitSDKAsync()` (once per app lifetime)
2. **Create player** — `new MediaPlayerCoreX(videoView)` for video, or without a view for audio-only
3. **Open media** — `await player.OpenAsync(source)` with a file path or stream URL
4. **Play** — `await player.PlayAsync()`
5. **Control playback** — seek with `Position_SetAsync()`, pause with `PauseAsync()`, adjust volume
6. **Stop** — `await player.StopAsync()`
7. **Dispose** — `await player.DisposeAsync()` releases all resources
8. **Destroy SDK** — `VisioForgeX.DestroySDK()` at application shutdown

## Common C# Video Player Scenarios

### Play Video File in C# (MP4, AVI, MKV)

Open and play any supported video file with automatic codec detection:

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("movie.mp4"));
await player.OpenAsync(source);
await player.PlayAsync();
```

See the full tutorial: [Build a Video Player in C#](guides/video-player-csharp.md)

### Play RTSP Stream in C#

Play live video from IP cameras and RTSP sources. Supports RTSP, HTTP, HLS, and MPEG-DASH:

```csharp
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("rtsp://admin:password@192.168.1.100:554/stream"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Audio-Only Playback

Play audio files (MP3, AAC, FLAC, WAV) without a video view:

```csharp
var player = new MediaPlayerCoreX(); // no VideoView needed
var source = await UniversalSourceSettings.CreateAsync(
    new Uri("music.mp3"));
await player.OpenAsync(source);
await player.PlayAsync();
```

### Extract Frame from Video

Grab a still image from the current playback position:

```csharp
// Save the current frame directly to disk — the simplest path.
bool saved = await player.Snapshot_SaveAsync("screenshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, quality: 85);

// Or grab the raw VideoFrameX for in-memory processing (caller owns the pixel buffer).
var frame = await player.Snapshot_GetAsync();
if (frame != null)
{
    // frame.Data (IntPtr), frame.DataSize, frame.Width, frame.Height, frame.Stride, frame.Format.
    // Free the unmanaged buffer when done:
    frame.Free();
}
```

See: [Get a Specific Frame from a Video File](code-samples/get-frame-from-video-file.md)

### Seeking, Volume, and Speed Control

```csharp
// Seek to a specific position
await player.Position_SetAsync(TimeSpan.FromSeconds(30));

// Get current position and duration
var position = await player.Position_GetAsync();
var duration = await player.DurationAsync();

// Adjust volume (0.0 to 1.0)
player.Audio_OutputDevice_Volume = 0.8;

// Change playback speed (0.25x to 4.0x)
await player.Rate_SetAsync(1.5);
```

### Loop Playback and Segment Play

Play a specific segment of a file or loop continuously:

```csharp
// Enable loop mode
player.Loop = true;

// Play a specific time range
player.Segment_Start = TimeSpan.FromSeconds(10);
player.Segment_Stop = TimeSpan.FromSeconds(60);
```

See: [Loop Mode and Position Range Playback](guides/loop-and-position-range.md)

## Supported Formats

| Category | Formats |
| -------- | ------- |
| Video Containers | MP4, AVI, MKV, WMV, WebM, MOV, TS, MTS, FLV |
| Audio Formats | MP3, AAC, WAV, WMA, FLAC, OGG, Vorbis |
| Video Codecs | H.264, H.265/HEVC, VP8, VP9, AV1, MPEG-2 |
| Streaming Protocols | RTSP, HTTP, HLS, MPEG-DASH |

## Platform Support

| Platform | UI Frameworks | Notes |
| -------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, WinUI, MAUI, Avalonia, Console | Full feature set including DirectShow engine |
| macOS | MAUI, Avalonia, Console | AVFoundation-based rendering |
| Linux x64 | Avalonia, Console | GStreamer-based decoding |
| Android | MAUI | Via MAUI integration |
| iOS | MAUI | Via MAUI integration |

For cross-platform implementations, see the [Avalonia Player Guide](guides/avalonia-player.md) (desktop, includes Linux) or the [.NET MAUI Player Guide](guides/maui-player.md) (mobile + desktop).

## Developer Documentation

### Guides

* [Build a Video Player in C# (WinForms / WPF)](guides/video-player-csharp.md)
* [Build a Video Player in VB.NET](guides/video-player-vb-net.md)
* [Avalonia Player Implementation](guides/avalonia-player.md)
* [.NET MAUI Player](guides/maui-player.md)
* [Android Player Implementation](guides/android-player.md)
* [Loop Mode and Position Range](guides/loop-and-position-range.md)
* [All Guides](guides/index.md)

### Code Examples

* [Get a Frame from a Video File](code-samples/get-frame-from-video-file.md)
* [Play a Fragment of a File](code-samples/play-fragment-file.md)
* [Show the First Frame](code-samples/show-first-frame.md)
* [Memory Playback](code-samples/memory-playback.md)
* [Playlist API](code-samples/playlist-api.md)
* [Reverse Video Playback](code-samples/reverse-video-playback.md)
* [All Code Examples](code-samples/index.md)

### Deployment

* [Deployment Guide](deployment.md)

## Developer Resources

* [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/)
* [API Reference](https://api.visioforge.org/dotnet/api/index.html)
* [Changelog](../changelog.md)
* [End User License Agreement](../../eula.md)
* [Licensing Information](../../../licensing.md)
