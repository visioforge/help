---
title: Play Video in Unity with the Media Blocks SDK .NET
description: Play local video files in Unity 6 with the VisioForge Media Blocks SDK .NET — the SimplePlayer sample, MediaBlocksPipeline, and BufferSinkBlock into a RawImage.
sidebar_label: Play a media file
order: 51
tags:
  - Media Blocks SDK
  - .NET
  - Unity
  - Windows
  - Playback
  - C#
primary_api_classes:
  - MediaBlocksPipeline
  - UniversalSourceBlock
  - BufferSinkBlock
  - AudioRendererBlock
---

# Play a media file in Unity

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The **`SimplePlayer`** scene plays a local video file with the **Media Blocks SDK .NET** and
renders it into a Unity `RawImage`. This article assumes you have imported the Unity package and
applied the two required project settings — see [Using VisioForge in Unity](index.md) first.

## Run the sample

1. In the **Project** window open `Assets/Scenes/SimplePlayer.unity` (double-click it).
2. In the **Hierarchy** select the **RawImage** GameObject. The `MediaBlocksPlayer` component is
   attached to it.
3. In the **Inspector**, set **File Path** to an absolute path to a local media file.
4. Press **▶ Play** — the video appears in the Game view and audio plays through the system
   default device.

![SimplePlayer scene Hierarchy: Canvas with RawImage, EventSystem, Main Camera](unity-simpleplayer-hierarchy.webp)

![MediaBlocksPlayer component in the Inspector with the File Path field](unity-simpleplayer-inspector.webp)

!!! tip "The RawImage is blank until you press Play"
    The video texture is created at runtime, so the `RawImage` shows nothing in edit mode.

## Inspector fields

| Field | Default | Description |
|---|---|---|
| **File Path** | `C:\Samples\!video.avi` | Absolute path to the media file to play. |
| **Auto Play On Start** | `true` | Start playback automatically in `Start()`. |
| **Render Audio** | `true` | Render audio through the system default device. |
| **Use Test Pattern** | `false` | Play a synthetic test pattern instead of the file (diagnostic baseline). |
| **Aspect Mode** | `Letterbox` | How the video is fitted into the `RawImage`: `Stretch`, `Letterbox`, or `Crop`. |

## The pipeline

`MediaBlocksPlayer` builds this pipeline:

```mermaid
graph LR;
    src[UniversalSourceBlock] -->|video| sink["BufferSinkBlock (RGBA)"];
    sink --> view["VisioForgeVideoView (Texture2D)"];
    src -->|audio, optional| audio[AudioRendererBlock];
```

The core of `PlayAsync`:

```csharp
_pipeline = new MediaBlocksPipeline();

_videoSink = new BufferSinkBlock(VideoFormatX.RGBA);
_videoSink.OnVideoFrameBuffer += _videoView.OnFrameBuffer;

// ignoreMediaInfoReader:true skips the media pre-probe (it can fail under the Unity
// runtime); the codec is negotiated when the pipeline starts.
var settings = await UniversalSourceSettings.CreateAsync(
    filePath, renderVideo: true, renderAudio: _renderAudio, ignoreMediaInfoReader: true);

_source = new UniversalSourceBlock(settings);
_pipeline.Connect(_source.VideoOutput, _videoSink.Input);

if (_renderAudio && _source.AudioOutput != null)
{
    _audioRenderer = new AudioRendererBlock();
    _pipeline.Connect(_source.AudioOutput, _audioRenderer.Input);
}

await _pipeline.StartAsync();
```

`UniversalSourceBlock` auto-detects the container and codec. The audio branch is connected only
when the file has an audio stream (`_source.AudioOutput != null`).

## Use it in your own scene

You do not have to use the sample scene:

1. Add a **Canvas → Raw Image** (*GameObject → UI → Raw Image*).
2. Select the **Raw Image** and **Add Component →** `MediaBlocksPlayer`.
3. Set **File Path** and press **▶ Play**.

The aspect handling (`Stretch` / `Letterbox` / `Crop`), the `RawImage` layout, and the vertical
flip are handled for you by the bundled `VisioForgeVideoView` — you do not write any texture code.
To switch the same GameObject to RTSP playback, swap `MediaBlocksPlayer` for `RTSPViewerPlayer`
(see [View an RTSP camera](rtsp-viewer.md)).

## Frequently Asked Questions

### Which video and audio formats can it play?

The package bundles FFmpeg/libav, so common formats decode out of the box — MP4, MKV, AVI, MOV with
H.264/H.265, MPEG-4, plus MP3/AAC audio, among others. `UniversalSourceBlock` auto-detects the
format.

### Can I change the file at runtime?

Yes. Set the `FilePath` property (or call `PlayAsync(path)`) and the player rebuilds the pipeline
for the new file.

### How do I control how the video fits the RawImage?

Use the **Aspect Mode** field: `Stretch` (fill, may distort), `Letterbox` (fit with bars), or
`Crop` (fill and crop the overflow).

### Does audio play too?

Yes, when **Render Audio** is enabled and the file has an audio track — audio plays through the
system default device. The audio branch is skipped automatically for video-only files.

## See Also

- [Using VisioForge in Unity](index.md) — package overview, setup, and how rendering works
- [View an RTSP camera in Unity](rtsp-viewer.md) — the live RTSP / IP camera sample
- [Media Blocks SDK .NET overview](../../mediablocks/index.md) — the full block catalog
- [Media Blocks RTSP player in C#](../../mediablocks/Guides/rtsp-player-csharp.md) — a non-Unity playback example
