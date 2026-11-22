---
title: Media Blocks SDK .NET FAQ — Frames, Push Sources, RTSP
description: Answers to common Media Blocks SDK .NET questions — pushing H.264 frames, getting bitmaps without a view, motion detection, RTSP latency and servers.
sidebar_label: FAQ
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - RTSP
  - H.264
  - Troubleshooting
primary_api_classes:
  - MediaBlocksPipeline
  - VideoSampleGrabberBlock
  - H264PushSourceBlock
  - CVMotionCellsBlock
  - RTSPSourceSettings
  - RTSPServerBlock
  - VirtualCameraSinkBlock
  - LiveVideoCompositor

---

# Media Blocks SDK .NET FAQ

Answers to the questions developers ask most often about Media Blocks SDK .NET pipelines. Deployment errors such as a missing `libgstreamer-1.0` on Windows are collected in [Troubleshooting](../general/troubleshooting.md).

## Getting frames in and out

### How do I play H.264 frames that my application receives over a socket?

Push them into the pipeline with `H264PushSourceBlock` and connect it to a decoder and a renderer. The block accepts both Annex B byte-stream and AVC (length-prefixed) data, and rebases the timestamps you pass to `PushData()`. Start pushing from a key frame. The block, its properties and a complete pipeline are described in [H264 Push Source Block](Sources/index.md#h264-push-source-block). For other formats, `PushSourceBlock` with `PushDataSourceSettings` takes any data that you describe with GStreamer caps.

Implement `OnError` on the pipeline while you build it: a pad that cannot be connected or a decoder that rejects the data is reported there.

### How do I get frames from several cameras as bitmaps without showing them?

Replace the `VideoRendererBlock` with a `VideoSampleGrabberBlock`; no `VideoView` is needed:

```csharp
var grabber = new VideoSampleGrabberBlock(VideoFormatX.BGRA, addNullRenderer: true);
grabber.OnVideoFrameBuffer += Grabber_OnVideoFrameBuffer;

pipeline.Connect(rtspSource.VideoOutput, grabber.Input);
```

`addNullRenderer: true` terminates the branch so the pipeline runs without a display. Use one pipeline, or one branch, per camera. Choose the pixel format that your bitmap type expects: SkiaSharp and WPF `Bgra32` use BGRA, `System.Drawing` 32-bit bitmaps are BGRA in memory too. Process or copy the frame inside the handler, and dispose every bitmap you create from it — memory that grows with each added camera usually means frames are kept or queued faster than they are processed. See [Video sample grabber](VideoProcessing/index.md#video-sample-grabber).

## Video analysis

### How do I detect motion in a Media Blocks pipeline?

Use `CVMotionCellsBlock`. It needs the OpenCV redistributable of your platform, for example the `VisioForge.CrossPlatform.OpenCV.Windows.x64` NuGet package, and its output must be connected — to a renderer, or to a `NullRendererBlock` when you do not display the video:

```csharp
var motion = new CVMotionCellsBlock(new CVMotionCellsSettings());
motion.MotionDetected += Motion_MotionDetected;

pipeline.Connect(source.VideoOutput, motion.Input);
pipeline.Connect(motion.Output, videoRenderer.Input);
```

`CVMotionCellsBlock.IsAvailable()` tells you whether the OpenCV plugin was found. Settings and events are in [CV Motion Cells Block](OpenCV/index.md#cv-motion-cells-block).

## RTSP

### Which latency should I set for an RTSP source?

`RTSPSourceSettings.Latency` is the jitter buffer of the RTSP source and defaults to 500 ms. A value shorter than one frame — 20 ms for a 25 fps camera, whose frames are 40 ms apart — cannot work. For low latency, start from 100–200 ms and go down only while the picture stays smooth; `LowLatencyMode = true` disables extra buffering and sets the latency to 150 ms unless you have already changed it.

Connect the audio branch only when the camera really sends audio: `RTSPSourceSettings.CreateAsync` reads the stream information by default, and `IsAudioAvailable()` then tells you whether to add the audio renderer or encoder. An audio branch waiting for a stream that never comes is a common cause of pipelines that do not start. For a complete viewer, see [RTSP Stream Viewer](Guides/rtsp-player-csharp.md).

### Can I serve a video file over RTSP, in a loop?

Yes. Build a pipeline with a file source, encoders and an `RTSPServerBlock`; clients then connect to the URL of the server. Set the pipeline's `Loop` property to restart the file when it ends; it works with seekable sources such as files. See [RTSP Server Block](RTSPServer/index.md).

## Output to other applications

### How do I send the pipeline output, or the Live Video Compositor, to a virtual camera?

Connect the video (and optionally the audio) to a `VirtualCameraSinkBlock`. Zoom, Teams, OBS and browsers then see it as a webcam. The Virtual Camera and Virtual Audio Card filters must be registered on the machine first; the block can check and register them. Details are in [Virtual Camera Sink](Sinks/index.md#virtual-camera-sink), and the [Virtual Camera Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Virtual%20Camera%20Demo) shows the whole flow.

With the Live Video Compositor, wrap the block in an output and add it to the compositor:

```csharp
var vcBlock = new VirtualCameraSinkBlock(new VirtualCameraSinkSettings());
var output = new LVCVideoAudioOutput("Virtual Camera", compositor, vcBlock, autostart: true);
await compositor.Output_AddAsync(output);
```

The [Live Video Compositor Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo) has this as one of its outputs. Without a Virtual Camera SDK license key, the virtual camera shows a TRIAL strip.
