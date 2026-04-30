---
title: VisioForge Media Player SDK in C# .NET — Cheat Sheet
description: One-page Media Player SDK reference with NuGet packages, MediaPlayerCoreX APIs, a canonical example, platform support, and pitfalls.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Avalonia
  - MAUI
  - WPF
  - WinForms
  - GStreamer
  - Playback
  - Streaming
  - RTSP
  - HLS
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - MediaPlayerCoreX
  - VideoView
  - UniversalSourceSettings
  - AudioRendererSettings
  - ErrorsEventArgs
---

# VisioForge Media Player SDK in C# .NET — Cheat Sheet

Use VisioForge Media Player SDK .Net when you need to play local files (MP4, MKV, MOV, WebM), network streams (RTSP, HLS, MPEG-DASH), and audio (MP3, AAC, FLAC) inside a C# app on Windows, macOS, Linux, Android, or iOS. `MediaPlayerCoreX` is the primary cross-platform engine; `VideoView` is the UI control that binds the engine to the visual tree.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Platform support

- **Windows** (x64 and x86) — WinForms, WPF, WinUI, MAUI, Avalonia, Console
- **macOS** (native and MacCatalyst) — MAUI, Avalonia, Console
- **Linux** (x64) — Avalonia, Console; requires system GStreamer 1.22+
- **Android** — via MAUI
- **iOS** — via MAUI

All cross-platform targets use a GStreamer-backed decoding pipeline with hardware acceleration where available. For the full codec × platform matrix, see [platform-matrix.md](../platform-matrix.md).

## NuGet packages

Main SDK package (always required):

```xml
<PackageReference Include="VisioForge.DotNet.MediaPlayer" Version="2026.2.19" />
```

Windows x64 native runtime:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

Windows x86 native runtime:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x86" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x86" Version="2025.11.0" />
```

macOS (native) and MacCatalyst (MAUI macOS):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />
<PackageReference Include="VisioForge.CrossPlatform.Core.macCatalyst" Version="2025.9.1" />
```

Linux x64 (plus system GStreamer 1.22+):

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
```

Android and iOS:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Android" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Core.iOS" Version="2025.11.0" />
```

Optional UI framework packages — add whichever you target:

```xml
<PackageReference Include="VisioForge.DotNet.Core.UI.MAUI" Version="2026.2.19" />
<PackageReference Include="VisioForge.DotNet.Core.UI.Avalonia" Version="2026.2.19" />
```

Full per-IDE install walkthrough: [install/index.md](../install/index.md).

## Primary API classes

| Class | Role | See also |
|---|---|---|
| `MediaPlayerCoreX` | Main playback engine (cross-platform, GStreamer-backed) | [Quick Start](./index.md) |
| `VideoView` | UI control that binds the player to the visual tree | [Avalonia guide](./guides/avalonia-player.md) |
| `UniversalSourceSettings` | Builds a source descriptor from a URI (file, URL, stream) | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `AudioRendererSettings` | Configures audio output device | [video-player-csharp.md](./guides/video-player-csharp.md) |
| `ErrorsEventArgs` | Error event payload surfaced via `OnError` | [video-player-csharp.md](./guides/video-player-csharp.md) |

## Canonical minimum example

```csharp
using System;
using VisioForge.Core;
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Sources;

public partial class MainForm : Form
{
    private MediaPlayerCoreX _player;

    private async void MainForm_Load(object sender, EventArgs e)
    {
        // 1. Initialize the SDK (once per process)
        await VisioForgeX.InitSDKAsync();

        // 2. Create the player bound to a VideoView control
        _player = new MediaPlayerCoreX(VideoView1);
        _player.OnError += Player_OnError;

        // 3. (Optional) pick an audio output device
        var devices = await _player.Audio_OutputDevicesAsync(
            AudioOutputDeviceAPI.DirectSound);
        if (devices.Length > 0)
            _player.Audio_OutputDevice = new AudioRendererSettings(devices[0]);

        // 4. Build a source from a file path or stream URL
        var source = await UniversalSourceSettings.CreateAsync(
            new Uri("C:\\Videos\\sample.mp4"));

        // 5. Open and play
        await _player.OpenAsync(source);
        await _player.PlayAsync();

        // Mid-playback control:
        //   await _player.PauseAsync();
        //   await _player.ResumeAsync();
        //   await _player.StopAsync();
    }

    private void Player_OnError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Player error: {e.Message}");
    }

    protected override async void OnFormClosing(FormClosingEventArgs e)
    {
        // 6. Dispose the player and tear down the SDK
        if (_player != null) await _player.DisposeAsync();
        VisioForgeX.DestroySDK();
        base.OnFormClosing(e);
    }
}
```

## Typical workflow

1. Initialize the SDK with `VisioForgeX.InitSDKAsync()`.
2. Instantiate `MediaPlayerCoreX` bound to a `VideoView` (omit the view for audio-only).
3. Optionally configure audio output with `AudioRendererSettings`.
4. Create a source via `UniversalSourceSettings.CreateAsync(uri)`.
5. `OpenAsync(source)` → `PlayAsync()`; control with `PauseAsync`, `ResumeAsync`, `Position_SetAsync`, `Rate_SetAsync`, `StopAsync`.
6. Dispose the player and call `VisioForgeX.DestroySDK()` on app exit.

## Common pitfalls

- **SDK init/destroy must bracket all usage.** Forgetting `VisioForgeX.DestroySDK()` at shutdown leaks native handles and can leave GStreamer threads running. Always pair it with `InitSDKAsync()`.
- **AnyCPU on Windows needs both redistributables.** Deploy BOTH `VisioForge.CrossPlatform.Core.Windows.x86` AND `.x64` (plus the matching Libav packages) or the app will fail at runtime on whichever architecture is missing.
- **Enumerate audio devices asynchronously before assigning.** `Audio_OutputDevicesAsync(...)` must complete before you construct `AudioRendererSettings` and set `Audio_OutputDevice`; setting it against an uninitialized device list throws.
- **Linux requires system GStreamer 1.22+.** The Linux NuGet redistributable assumes system-installed GStreamer (`gstreamer1.0-*` packages) — it is NOT a substitute, and the Windows Libav packages do not apply to Linux.
- **`MediaPlayerCoreX` ≠ `MediaPlayerCore`.** `MediaPlayerCoreX` is the cross-platform GStreamer-backed engine used throughout this page. `MediaPlayerCore` (no `X`) is the first-class Windows-only DirectShow engine with different method signatures — both are fully supported; do not mix APIs between them.

## See also

- **UI frameworks**
    - WinForms / WPF desktop → [video-player-csharp.md](./guides/video-player-csharp.md)
    - Avalonia cross-platform (Windows/macOS/Linux) → [avalonia-player.md](./guides/avalonia-player.md)
    - .NET MAUI (mobile + desktop) → [maui-player.md](./guides/maui-player.md)
    - Android only → [android-player.md](./guides/android-player.md)
- **Network streaming**
    - RTSP → [rtsp.md](../general/network-streaming/rtsp.md)
    - HLS → [hls-streaming.md](../general/network-streaming/hls-streaming.md)
- **Deployment**
    - Windows → [../deployment-x/Windows.md](../deployment-x/Windows.md)
    - macOS → [../deployment-x/macOS.md](../deployment-x/macOS.md)
    - Linux (Ubuntu) → [../deployment-x/Ubuntu.md](../deployment-x/Ubuntu.md)
    - Android → [../deployment-x/Android.md](../deployment-x/Android.md)
    - iOS → [../deployment-x/iOS.md](../deployment-x/iOS.md)
- **Getting started + full guide** → [index.md](./index.md), [video-player-csharp.md](./guides/video-player-csharp.md)

## FAQ

### Can the SDK play RTSP streams?

Yes. Pass an `rtsp://` URI to `UniversalSourceSettings.CreateAsync(...)` and call `OpenAsync` / `PlayAsync` — the same code path as a local file. HTTP, HLS, and MPEG-DASH URIs work identically.

### Which platforms require GStreamer to be installed at the system level?

Linux. On Linux x64, the `VisioForge.CrossPlatform.Core.Linux.x64` NuGet package relies on a system-installed GStreamer 1.22+ runtime. Windows, macOS, Android, and iOS bundle everything through their NuGet redistributables — no system install needed.

### What is the difference between `MediaPlayerCoreX` and `MediaPlayerCore`?

`MediaPlayerCoreX` is the cross-platform engine used throughout this cheat sheet — it runs on Windows, macOS, Linux, Android, and iOS via GStreamer and uses async method signatures. `MediaPlayerCore` (no `X`) is the Windows-only DirectShow engine with a synchronous, event-based API; it remains a first-class, fully supported engine (choose it when you target Windows-only and need DirectShow-specific behaviour). For new cross-platform projects, prefer `MediaPlayerCoreX`.

### How do I play audio without showing a video window?

Instantiate `MediaPlayerCoreX` without a `VideoView` (`new MediaPlayerCoreX()`) and proceed with the same `UniversalSourceSettings.CreateAsync` → `OpenAsync` → `PlayAsync` flow. Audio-only sources (MP3, AAC, FLAC, WAV) will play through the configured `Audio_OutputDevice`.
