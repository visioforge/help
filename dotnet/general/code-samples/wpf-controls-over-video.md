---
title: Overlay WPF Controls on Video in C# .NET Applications
description: Place WPF buttons, text and panels on top of VideoView. D3D11 composable renderer, WriteableBitmap mode, airspace issues, and the WPF_WinUI_Callback migration.
sidebar_label: WPF Controls over Video
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - Media Blocks SDK
  - .NET
  - WPF
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - MediaPlayerCore
  - VideoCaptureCore
  - Windows
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererMode
  - IVideoSurfaceProvider
  - MediaPlayerCoreX
  - VideoCaptureCore

---

# Overlaying WPF Controls on Video

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Placing WPF elements — buttons, overlays, status banners, transport controls — on top of the video preview only works when the video is drawn *inside* the WPF visual tree. If the frames go to a native child window (HWND), WPF cannot paint anything over them: that is the classic WPF **airspace** limitation, and no amount of `Panel.ZIndex` fixes it.

The WPF `VisioForge.Core.UI.WPF.VideoView` control supports three ways to draw a frame. Two of them compose with WPF, one does not:

| Render mode | How to enable | WPF controls on top | Notes |
|---|---|---|---|
| `D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` | Yes | Frames stay on the GPU, presented to WPF as a `D3DImage`. Recommended. |
| Software (`WriteableBitmap`) | X engines: `VideoView1.SetNativeRendering(false)`; classic engines: `VideoRendererMode.FrameCallback` on the engine | Yes | CPU upload per frame. The classic `WPF_WinUI_Callback` mode. |
| Native HWND | default (`VideoView1.SetNativeRendering(true)`) | No | Fastest path, but airspace blocks every overlay. |

!!! tip "Coming from `VideoRendererMode.WPF_WinUI_Callback`?"
    In the classic engines the mode was set on the engine: `VideoCapture1.Video_Renderer.VideoRenderer`. The cross-platform X engines (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) have no `Video_Renderer` object — the render mode moved onto the `VideoView` control itself, so a single view works with any engine. See [Migrating from WPF_WinUI_Callback](#migrating-from-wpf_winui_callback) below.

## D3D11 composable renderer (recommended)

`D3D11Composable` uploads each frame into a shared Direct3D 11 texture, opens it on a hidden Direct3D 9Ex device and exposes it to WPF as a `D3DImage`. The video becomes an ordinary element of the visual tree: render transforms, opacity, z-order and rounded clipping all apply to it, and there is no HWND to fight with.

**Requirements:** Windows Vista or later (Direct3D 9Ex) and a Direct3D 10/11-class GPU.

### Enabling it with the X engines

Call `UseD3D11ComposableRenderer()` before you start playback or capture — the call is idempotent, so it is safe to repeat it after an engine restart:

```cs
// Before creating / starting the engine.
VideoView1.UseD3D11ComposableRenderer();

_player = new MediaPlayerCoreX(VideoView1);

var source = await UniversalSourceSettingsV2.CreateAsync(new Uri(filename));
await _player.OpenAsync(source);
await _player.PlayAsync();
```

The same call works for `VideoCaptureCoreX` and `VideoEditCoreX`, and for a `VideoRendererBlock` built on the same view in Media Blocks SDK .NET.

### Enabling it with the classic engines

`VideoCaptureCore`, `MediaPlayerCore` and `VideoEditCore` use exactly the same call on the view:

```cs
VideoView1.UseD3D11ComposableRenderer();
```

The engine is switched to the frame-callback channel behind the scenes and its frames are uploaded into the shared texture. Prefer this call over selecting `VideoRendererMode.D3D11Composable` on the engine's `Video_Renderer` settings: the view starts in native rendering mode, and while it stays there it overrides the engine's choice with a native HWND. `UseD3D11ComposableRenderer()` takes care of that for you.

### XAML layout

Put the view and the overlay into the same `Grid` cell. Later children are drawn on top:

```xml
<Window
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
    x:Class="MyPlayer.MainWindow">
    <Grid>
        <WPF:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>

        <StackPanel Orientation="Horizontal" VerticalAlignment="Bottom"
                    HorizontalAlignment="Center" Margin="0,0,0,16">
            <Button Content="Play" Width="80" Click="btPlay_Click" />
            <Button Content="Pause" Width="80" Margin="8,0,0,0" Click="btPause_Click" />
        </StackPanel>
    </Grid>
</Window>
```

Because the video is a normal WPF element in this mode, the view itself can be transformed as well:

```cs
VideoView1.RenderTransformOrigin = new Point(0.5, 0.5);
VideoView1.RenderTransform = new RotateTransform(10);
VideoView1.Opacity = 0.7;
```

### Custom GPU processing on top of the frame

In `D3D11Composable` mode the view exposes the shared texture, so you can run your own shaders on each frame without a round trip through CPU memory:

```cs
var provider = VideoView1.GetSurfaceProvider();
if (provider != null)
{
    provider.FrameReady += (sender, e) =>
    {
        // e.SharedHandle — DXGI shared handle, open it on your own ID3D11Device
        // via OpenSharedResource; e.Width / e.Height — texture size.
    };
}
```

`FrameReady` is raised synchronously on the thread that pushed the frame — a DirectShow callback or a GStreamer streaming thread, not the WPF dispatcher, and not necessarily the same thread on every frame. Marshal to the dispatcher before touching any WPF control, and keep thread-affine resources out of the handler.

`GetSurfaceProvider()` returns `null` in every other render mode — a quick way to confirm the composable panel is really active.

## Software rendering mode

If you cannot use Direct3D — a virtual machine without GPU acceleration, a remote desktop session, or a very old target — switch to the software path. Frames are drawn into a `WriteableBitmap` hosted by a WPF `Image` inside the view, which composes with WPF just as well, at the cost of a per-frame CPU upload.

With the X engines, turn native rendering off on the view before the engine starts:

```cs
VideoView1.SetNativeRendering(false);
```

On the classic engines the mode belongs to the engine. Set it before `PlayAsync()` / `StartAsync()` and the view follows it out of native mode on its own:

```cs
MediaPlayer1.Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback;
```

On builds released before 2026.8.18 the view stayed in native mode and replaced that setting with a native HWND renderer, so add `VideoView1.SetNativeRendering(false)` before the line above if you target one of them. The extra call remains harmless on current builds.

The XAML above is unchanged in both cases. Software rendering and the composable panel are mutually exclusive: once the panel is installed, `SetNativeRendering(true)` is ignored and logged as a warning.

## Why native HWND rendering blocks overlays

The native path hands the frames to a Win32 child window hosted inside the WPF layout. It is the fastest option and the view's default, but the child window is composed by the desktop window manager *after* WPF has drawn its own content, so any WPF element positioned above the video is simply invisible. The same limitation applies to the classic `VideoRendererMode.WPF_NativeHWND` mode.

If you need both maximum throughput and an overlay, prefer `D3D11Composable` — it keeps the frame GPU-resident and still composes with WPF.

## Migrating from WPF_WinUI_Callback

Nothing was removed from the classic engines: `VideoRendererMode.WPF_WinUI_Callback` still exists, and `VideoRendererMode.FrameCallback` is an alias for the same value with a more self-descriptive name. It is set on the engine, as shown under [Software rendering mode](#software-rendering-mode).

What changed is where the setting lives when you move to an X engine:

| Classic engine | X engine equivalent |
|---|---|
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_WinUI_Callback` | `VideoView1.SetNativeRendering(false)` |
| `Video_Renderer.VideoRenderer = VideoRendererMode.D3D11Composable` | `VideoView1.UseD3D11ComposableRenderer()` (both families) |
| `Video_Renderer.VideoRenderer = VideoRendererMode.WPF_NativeHWND` | default behaviour, or `VideoView1.SetNativeRendering(true)` |

For per-frame access to pixel data — computer vision, AI inference, custom drawing — see [Custom Video Effects Using Frame Events](custom-video-effects.md) and [Image drawing via OnVideoFrameBuffer](image-onvideoframebuffer.md).

## Troubleshooting

**A separate video window opens instead of rendering inside the layout.** The view is running the HWND path while your code expects the composable panel. Call `UseD3D11ComposableRenderer()` *before* attaching the engine, and check `GetSurfaceProvider()` for a non-`null` result after playback starts.

**The overlay is invisible, the video plays.** The view is in native rendering mode. Call `UseD3D11ComposableRenderer()`, or — for the software path — `SetNativeRendering(false)` on an X engine, or `Video_Renderer.VideoRenderer = VideoRendererMode.FrameCallback` on a classic one. All of them must run before playback starts, so restart the preview for the change to take effect.

**Preview stays black in a remote desktop / VM session.** Direct3D acceleration may be unavailable there — fall back to the software path described above.

## Demos

- **[Simple Player Demo D3D11 (X engine)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/WPF/Simple%20Player%20Demo%20D3D11)** — `MediaPlayerCoreX` with a WPF banner over the video, plus view rotation and opacity.
- **[Simple Player Demo D3D11 (classic engine)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK/WPF/CSharp/Simple%20Player%20Demo%20D3D11)** — the same renderer driven by the DirectShow-based `MediaPlayerCore`.

## Related pages

- [Video Renderer Selection (WinForms)](select-video-renderer-winforms.md) — all renderer modes of the classic engines.
- [WPF Multi-screen Video Output](multiple-screens-wpf.md) — several independent preview surfaces in one application.
- [Custom Image Video View](video-view-set-custom-image.md) — replacing the preview with a static image.
