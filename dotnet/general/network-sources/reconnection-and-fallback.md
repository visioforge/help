---
title: RTSP Reconnect & Fallback Switch in C# .NET — All SDKs
description: Handle IP camera disconnects in C# / .NET with reconnect events, DisconnectEventInterval, and FallbackSwitch (image/text/media) across all VisioForge SDKs.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - .NET
  - DirectShow
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Playback
  - Streaming
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - C#
primary_api_classes:
  - FallbackSwitchSettings
  - RTSPSourceSettings
  - MediaPlayerCore
  - VideoCaptureCoreX
  - MediaPlayerCoreX

---

# RTSP Reconnection and Fallback Switch in C# / .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    Reconnection events work on every SDK. The declarative `FallbackSwitch` is GStreamer-backed and therefore runs on **Windows, macOS, Linux, Android, and iOS** in the modern `X` engines and Media Blocks — it's not available on classic DirectShow-only engines. See the [platform support matrix](../../platform-matrix.md) and the [Linux deployment guide](../../deployment-x/Ubuntu.md).

IP cameras drop connections — network blips, power cycles, router reboots, keyframe starvation. VisioForge SDKs give you two ways to handle it:

- **Reactive** — your code subscribes to a disconnect event, then stops and restarts the source.
- **Declarative** — you configure a `FallbackSwitch` on the source, the pipeline swaps in a placeholder image / text card / alternate stream automatically, and your code never sees the hiccup.

Pick based on which SDK you're using and the UX you want. This guide covers both approaches across every SDK in the family.

## Which approach is available where

| SDK | Reactive (detect + restart) | Declarative (FallbackSwitch) |
|---|:---:|:---:|
| VideoCaptureCore (classic, Windows) | ✅ `OnNetworkSourceDisconnect` + `DisconnectEventInterval` | ⛔ |
| MediaPlayerCore (classic, Windows) | ✅ `OnNetworkSourceStop` (FFMPEG) / `OnError` | ⛔ |
| VideoCaptureCoreX (cross-platform) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ via `RTSPSourceSettings.FallbackSwitch` |
| MediaPlayerCoreX (cross-platform) | ✅ pipeline `OnNetworkSourceDisconnect` / `OnError` | ✅ |
| Media Blocks SDK | ✅ pipeline event | ✅ `FallbackSwitchSourceBlock` |

Rule of thumb: on the classic Windows engines you have reactive only. On any modern (cross-platform) engine, prefer declarative for the UX, add reactive on top for telemetry.

## Reactive — VideoCaptureCore (classic)

`IPCameraSourceSettings.DisconnectEventInterval` defines how long the SDK waits without incoming frames before firing the disconnect event. Default is 10 seconds; drop it to 3–5 seconds for surveillance.

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.VideoCapture;

videoCapture1.IP_Camera_Source = new IPCameraSourceSettings
{
    URL = new Uri("rtsp://192.168.1.100:554/stream1"),
    Login = "admin",
    Password = "admin123",
    Type = IPSourceEngine.Auto_LAV,
    DisconnectEventInterval = TimeSpan.FromSeconds(5)
};

videoCapture1.OnNetworkSourceDisconnect += async (s, e) =>
{
    // Exponential backoff: 1s, 2s, 4s, 8s... capped at 30s.
    int attempt = 0;
    int delayMs = 1000;
    while (attempt < 10)
    {
        await videoCapture1.StopAsync();
        await Task.Delay(delayMs);
        if (await videoCapture1.StartAsync()) break;
        delayMs = Math.Min(delayMs * 2, 30_000);
        attempt++;
    }
};

videoCapture1.Mode = VideoCaptureMode.IPPreview;
await videoCapture1.StartAsync();
```

`OnNetworkSourceDisconnect` fires from a timer thread — marshal to the UI thread (`Invoke` / `Dispatcher.Invoke`) before touching controls.

## Reactive — MediaPlayerCore (classic)

Classic `MediaPlayerCore` exposes `OnNetworkSourceStop` on the FFMPEG engine; for other engines and any generic failure, subscribe `OnError`.

```csharp
using VisioForge.Core.MediaPlayer;

mediaPlayer1.OnNetworkSourceStop += async (s, e) =>
{
    await mediaPlayer1.StopAsync();
    await Task.Delay(2000);
    await mediaPlayer1.StartAsync();
};

mediaPlayer1.OnError += (s, e) =>
{
    // Log but don't teardown — let the retry loop above handle real drops.
    Debug.WriteLine($"Player error: {e.Message}");
};
```

## Reactive — Media Blocks pipelines

`MediaBlocksPipeline` exposes `OnNetworkSourceDisconnect` with rich `NetworkSourceDisconnectEventArgs` — useful when multiple RTSP sources share one app and you need to know *which* one dropped.

```csharp
using VisioForge.Core.Types.Events;

pipeline.OnNetworkSourceDisconnect += (s, e) =>
{
    // e.SourceElementName — the GStreamer element that faulted
    // e.ErrorMessage       — short description
    // e.DebugInfo          — GStreamer debug output (may be null)
    // e.Uri                — the failing RTSP URL (may be null)
    // e.Timestamp          — when the SDK detected the failure
    Log($"[{e.Timestamp:HH:mm:ss}] {e.SourceElementName} dropped: {e.ErrorMessage}");
};

pipeline.OnError += (s, e) =>
{
    Log($"Pipeline error: {e.Message}");
};
```

## Reactive — X engines (VideoCaptureCoreX / MediaPlayerCoreX)

`VideoCaptureCoreX` and `MediaPlayerCoreX` do **not** publicly expose their internal `MediaBlocksPipeline`. To react to disconnects on these engines:

1. Subscribe to `OnError` on the engine — fires for all pipeline-level errors including lost RTSP sources.
2. Use the declarative `FallbackSwitch` (below) to keep the UI alive during transient drops.
3. If you need the granular `NetworkSourceDisconnectEventArgs` per-source, build the capture pipeline directly with `MediaBlocksPipeline` + `RTSPSourceBlock` and use the Media Blocks pattern above.

```csharp
videoCaptureCoreX.OnError += (s, e) =>
{
    // e.Message carries the engine-level error; for RTSP drops the text includes
    // the source element name. Pair this handler with a retry loop or FallbackSwitch.
    Log($"VideoCaptureCoreX error: {e.Message}");
};
```

## Declarative — FallbackSwitch overview

`FallbackSwitch` wraps a live source with automatic failover. When the main source stops producing frames for longer than `TimeoutMs`, the pipeline switches to a pre-configured fallback (text, image, or alternate media) and keeps your `VideoView` rendering. When the main source recovers, the pipeline switches back.

### The `FallbackSwitchSettings` container

```csharp
public class FallbackSwitchSettings
{
    public bool Enabled { get; set; } = false;
    public FallbackSwitchSettingsBase Fallback { get; set; }
    public bool EnableVideo { get; set; } = true;
    public bool EnableAudio { get; set; } = true;
    public int MinLatencyMs { get; set; } = 0;
    public string FallbackVideoCaps { get; set; }   // e.g. "video/x-raw,width=1920,height=1080"
    public string FallbackAudioCaps { get; set; }   // e.g. "audio/x-raw,rate=48000,channels=2"
}
```

### Base tunables (shared by all fallback types)

```csharp
public abstract class FallbackSwitchSettingsBase
{
    public int  TimeoutMs        { get; set; } = 5000;   // silence before fallback kicks in
    public int  RestartTimeoutMs { get; set; } = 5000;   // wait between main-source restart attempts
    public int  RetryTimeoutMs   { get; set; } = 60000;  // give-up window after repeated failures
    public bool ImmediateFallback{ get; set; } = false;  // show fallback from the first frame gap
    public bool RestartOnEos     { get; set; } = false;  // treat EOS as a failure (for loops)
    public bool ManualUnblock    { get; set; } = false;  // require app to call Unblock() on recovery
}
```

### Three fallback types

1. **`StaticTextFallbackSettings`** — render a "Camera offline" text card.
2. **`StaticImageFallbackSettings`** — show a brand logo, last snapshot, or "reconnecting" slate.
3. **`MediaBlockFallbackSettings`** — play an alternate live stream or a looping file.

## Fallback — static text

```csharp
using SkiaSharp;
using VisioForge.Core.Types.X.Sources;

var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text            = "Camera offline — reconnecting...",
        FontFamily      = "Arial",
        FontSize        = 32f,
        TextColor       = SKColors.White,
        BackgroundColor = SKColors.Black,
        Position        = new SKPoint(0.5f, 0.5f),   // centre
        TimeoutMs       = 3000,
        RestartTimeoutMs= 5000,
    },
};
```

## Fallback — static image

```csharp
var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticImageFallbackSettings
    {
        ImagePath           = @"C:\assets\camera-offline.png",
        ScaleToFit          = true,
        MaintainAspectRatio = true,
        BackgroundColor     = SKColors.Black,
        TimeoutMs           = 3000,
    },
};
```

## Fallback — alternate media source

```csharp
var fallback = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new MediaBlockFallbackSettings
    {
        FallbackUri      = "file:///C:/assets/standby-loop.mp4",
        IsLive           = false,
        RestartOnEos     = true,      // restart the loop when the file ends
        TimeoutMs        = 3000,
    },
};
```

`FallbackUri` accepts any URI GStreamer can read — `file://`, another `rtsp://`, HLS, HTTP. `CustomPipeline` lets you inject a bespoke GStreamer launch line for advanced scenarios (e.g. `videotestsrc pattern=snow`).

## Using FallbackSwitch — high-level (`RTSPSourceSettings.FallbackSwitch`)

The simplest path: attach the settings object directly to `RTSPSourceSettings` and pass it to `VideoCaptureCoreX` / `MediaPlayerCoreX` as usual. No extra pipeline plumbing.

```csharp
var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.100:554/stream1"),
    "admin", "admin123", audioEnabled: false);

rtsp.LowLatencyMode = true;
rtsp.FallbackSwitch = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticImageFallbackSettings
    {
        ImagePath = "offline.png",
        TimeoutMs = 3000,
    },
};

_videoCapture.Video_Source = rtsp;
await _videoCapture.StartAsync();
```

## Using FallbackSwitch — low-level (`FallbackSwitchSourceBlock`)

In the Media Blocks SDK you wire the fallback directly at the pipeline level via `FallbackSwitchSourceBlock`. This works with *any* source — RTSP, HTTP MJPEG, file, device — not just RTSP.

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;

var fallbackSwitch = new FallbackSwitchSourceBlock(
    mainSourceSettings: rtspSettings,                          // IVideoSourceSettings
    fallbackSettings:   new FallbackSwitchSettings
    {
        Enabled = true,
        Fallback = new StaticTextFallbackSettings { Text = "OFFLINE" },
    });

// Telemetry — works alongside the declarative UI
pipeline.OnNetworkSourceDisconnect += (s, e) =>
    metrics.RecordDisconnect(e.SourceElementName, e.Uri, e.Timestamp);

// FallbackSwitchSourceBlock exposes separate VideoOutput / AudioOutput pads — no single Output.
pipeline.Connect(fallbackSwitch.VideoOutput, renderer.Input);
await pipeline.StartAsync();

// Inspect runtime state:
string status = fallbackSwitch.GetStatus();
var stats = fallbackSwitch.GetStatistics();

// If ManualUnblock is set, call Unblock() when you want playback to resume the main source.
// fallbackSwitch.Unblock();
```

## Common patterns

**Exponential backoff** — in the reactive loop, double the delay on each failed reconnect, cap at 30 s. Stops you from hammering a dead camera's ARP cache.

**Declarative UX + reactive telemetry** — let `FallbackSwitch` keep the screen alive, and use `pipeline.OnNetworkSourceDisconnect` to feed your monitoring dashboard / Slack alert / NVR log. Neither approach precludes the other.

**Multi-camera wall** — never tear down the whole grid on one fault. See the [multi-camera RTSP grid guide](../../mediablocks/Guides/multi-camera-rtsp-grid.md) for the one-pipeline-per-camera pattern; attach a `FallbackSwitch` to each engine independently.

**Cross-platform note** — `FallbackSwitch` depends on the GStreamer `fallbackswitch` element, which ships with the X redist. Classic Windows-only `VideoCaptureCore` / `MediaPlayerCore` don't have it — use the reactive approach there.

## Troubleshooting

**Fallback never activates** — `Enabled = true`? `TimeoutMs` lower than the gap you want to catch? First few frames need to succeed before the timeout starts counting — a camera that fails on first handshake is a pipeline-start error, not a timeout.

**Fallback activates but never leaves** — the main source isn't recovering. Check `RestartTimeoutMs` (too high delays the next retry) and `RetryTimeoutMs` (after this window the block stops retrying). With `ManualUnblock = true` you must call `fallbackSwitch.Unblock()` yourself.

**Image fallback shows black** — codec caps mismatch between the main pipeline and the fallback decoder. Set `FallbackVideoCaps` explicitly, e.g. `"video/x-raw,width=1920,height=1080,format=RGB"`, matching the renderer's expected format.

**Text fallback — wrong font / position** — `FontFamily` must exist on the target platform (Arial is on Windows and macOS; prefer `DejaVuSans` on Linux). `Position` is 0.0–1.0 fractions of the video frame.

**`OnNetworkSourceDisconnect` fires repeatedly** — the source block is retrying and failing in quick succession. Raise `RestartTimeoutMs` to 10–15 s on a known-flaky network, or wrap logging with a debounce.

**UI thread exceptions** — `OnNetworkSourceDisconnect` fires from the GStreamer bus thread. Use `Dispatcher.Invoke` (WPF) / `Control.Invoke` (WinForms) / `MainThread.BeginInvokeOnMainThread` (MAUI) before touching controls.

## Related Documentation

- [RTSP protocol deep-dive](../network-streaming/rtsp.md) — how RTSP works, transport options, and streaming architecture
- [RTSP camera source configuration](../../videocapture/video-sources/ip-cameras/rtsp.md) — `IPCameraSourceSettings` / `RTSPSourceSettings` reference
- [ONVIF IP camera integration](../../videocapture/video-sources/ip-cameras/onvif.md) — discovery + PTZ; pair ONVIF re-discovery with FallbackSwitch for hard-to-reach cameras
- [Media Blocks RTSP player](../../mediablocks/Guides/rtsp-player-csharp.md) — single-camera pipeline
- [Multi-camera RTSP grid (NVR wall)](../../mediablocks/Guides/multi-camera-rtsp-grid.md) — 4×4 wall with per-cell fallback
- [Save RTSP stream without re-encoding](../../mediablocks/Guides/rtsp-save-original-stream.md) — archive alongside live preview
- [RTSP server output](../../mediablocks/RTSPServer/index.md) — server-side resilience for your own stream
- [Platform support matrix](../../platform-matrix.md) — codec and hardware-acceleration details across platforms

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for code samples including RTSP preview, record, and MultiView demos.
