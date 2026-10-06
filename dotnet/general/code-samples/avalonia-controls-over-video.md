---
title: Overlay Avalonia Controls on Video in C# .NET Applications
description: Place Avalonia buttons, panels and scrolling content on top of VideoView. Software rendering mode, native airspace limitations, and WSLg.
sidebar_label: Avalonia Controls over Video
tags:
  - Media Blocks SDK
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Avalonia
  - VideoRendererBlock
  - VideoView
  - Windows
  - Linux
  - macOS
  - Playback
  - Capture
  - C#
primary_api_classes:
  - VideoView
  - VideoRendererBlock

---

# Overlaying Avalonia Controls on Video

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Placing Avalonia elements — buttons, overlays, status banners — on top of the video preview only works when the video is drawn *inside* the Avalonia visual tree. The Avalonia `VideoView` draws there in one of two modes. The default mode hands the frames to a native child window hosted through `NativeControlHost` (an HWND-hosted child on Windows, an X11 window via XEmbed on Linux and macOS), and a native child window is composed after — and on top of — everything Avalonia has drawn: overlays under it are invisible, and it is not clipped by `ScrollViewer` content. This is the same airspace limitation the WPF control has, and Avalonia documents it as a [limitation of native embedding](https://docs.avaloniaui.net/docs/app-development/native-interop).

| Render mode | How to enable | Avalonia controls on top | Notes |
|---|---|---|---|
| Native window embedding | default (`SetNativeRendering(true)`) | No | Fastest path. The video is clipped to the host control's own bounds and always draws above Avalonia content. |
| Software (`WriteableBitmap`) | `VideoView1.SetNativeRendering(false)` | Yes | Frames are blitted into a `WriteableBitmap` in the visual tree, at the cost of a CPU copy per frame. |

There is no GPU-composited equivalent of the WPF `D3D11Composable` mode in the Avalonia control; software mode is the mode that composes.

## Software rendering mode

Call `SetNativeRendering(false)` on the view **before** the pipeline or engine starts:

```cs
VideoView1.SetNativeRendering(false);
```

In Media Blocks SDK .NET, wire the view the usual way — `VideoRendererBlock` reads the mode from the view at build time and delivers frames through a buffer sink instead of embedding a window renderer:

```cs
_pipeline = new MediaBlocksPipeline();

_fileSource = new UniversalSourceBlock(
    await UniversalSourceSettings.CreateAsync(filename, renderVideo: true, renderAudio: false));

_videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
_pipeline.Connect(_fileSource.VideoOutput, _videoRenderer.Input);

await _pipeline.StartAsync();
```

In software mode the video is an ordinary Avalonia element: panels, buttons and popups draw on top of it, and it clips correctly inside a `ScrollViewer` or any clipped container. The frame is scaled uniformly and centered inside the view, like the native mode.

X engines (`VideoCaptureCoreX`, `MediaPlayerCoreX`, `VideoEditCoreX`) honor the same call — set it before attaching the engine / starting playback.

## Layout

Put the view and the overlay into the same `Grid` cell. Later children are drawn on top:

```xml
<Window xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.Avalonia;assembly=VisioForge.Core.UI.Avalonia"
        x:Class="MyPlayer.MainWindow">
    <Panel>
        <vf:VideoView x:Name="VideoView1" />

        <Border Background="#88000000" CornerRadius="8"
                VerticalAlignment="Top" HorizontalAlignment="Left"
                Margin="8" Padding="10,6">
            <TextBlock Text="LIVE" Foreground="White" FontWeight="SemiBold" />
        </Border>
    </Panel>
</Window>
```

## Running under WSL (WSLg)

Inside WSLg the native child-window embedding crashes the application when the first video frame arrives: Mesa cannot initialize its GPU path (`ZINK: failed to choose pdev`, `failed to create dri2 screen`), and the XEmbed window creation that follows fails with it. This is a WSLg windowing limitation, not an SDK defect — the same application renders correctly on a native Ubuntu installation.

Software mode does not create a native child window at all, so it renders normally under WSLg. If you develop inside WSL, call `SetNativeRendering(false)`, or gate it on a WSL detection at startup. See [Ubuntu Deployment](../../deployment-x/Ubuntu.md) for the GStreamer packages WSL needs.

## Troubleshooting

**The overlay is invisible, the video plays.** The view is in native embedding mode. Call `SetNativeRendering(false)` before starting the pipeline — the mode is read when the pipeline builds, so restart playback for the change to take effect.

**A separate video window opens instead of rendering inside the layout.** A `VideoView` that is not part of a rooted window gives the video sink no parent window, and the sink opens its own top-level window. Add the view to the window's visual tree (the control adds its native host on start).

**The application exits as soon as the first frame arrives, with Mesa/Zink errors in the console.** Native embedding under WSLg — see the section above.

## Related pages

- [Overlay WPF Controls on Video](wpf-controls-over-video.md) — the same airspace topic for WPF, including the GPU-composited `D3D11Composable` mode.
- [Ubuntu Deployment](../../deployment-x/Ubuntu.md) — GStreamer packages for Linux and WSL.
