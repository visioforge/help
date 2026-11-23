---
title: Video Capture SDK .NET FAQ — IP Cameras, Recording, Devices
description: Answers to common Video Capture SDK .NET questions — RTSP login and reconnect, recording without stopping preview, snapshots, devices and overlays.
sidebar_label: FAQ
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCoreX
  - Windows
  - Capture
  - IP Camera
  - RTSP
  - Screen Capture
  - Webcam
  - Troubleshooting
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureCoreX
  - IPCameraSourceSettings
  - RTSPSourceSettings
  - TextStatusOverlay

---

# Video Capture SDK .NET FAQ

Answers to the questions developers ask most often about Video Capture SDK .NET. Both engines are covered: `VideoCaptureCore` (Windows, DirectShow) and `VideoCaptureCoreX` (cross-platform). Deployment errors such as `Unable to create filter` are collected in [Troubleshooting](../general/troubleshooting.md).

## IP cameras

### How do I connect to an IP camera that needs a login and password?

Pass the credentials in the source settings. With `VideoCaptureCore`:

```csharp
VideoCapture1.IP_Camera_Source = new IPCameraSourceSettings
{
    URL = new Uri("rtsp://192.168.1.10:554/stream1"),
    Login = "admin",
    Password = "password",
    Type = IPSourceEngine.Auto_LAV
};

VideoCapture1.Mode = VideoCaptureMode.IPPreview;
await VideoCapture1.StartAsync();
```

With `VideoCaptureCoreX` or Media Blocks SDK .NET, `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)` takes them as arguments. Most cameras also accept the `rtsp://login:password@host:port/path` form, but separate properties avoid problems with special characters in the password.

The URL must be the address of the video stream itself, not the address of the camera's web page. The path is different for every vendor: check the camera manual, the [camera brand guides](../camera-brands/index.md), or discover the stream with [ONVIF](video-sources/ip-cameras/onvif.md). Any camera that supports RTSP, ONVIF or HTTP MJPEG can be used, and it makes no difference whether it is connected by Wi-Fi or cable. A quick test: if VLC opens the URL, the SDK can open it too. With `VideoCaptureCore`, a URL that does not open is reported by a message that starts with `Unable to set URL to network FFMPEG source filter`, `Unable to set URL to LAV Splitter Source filter` or `Unable to set URL to network VLC source filter`, depending on the engine.

### How do I reconnect when the camera goes offline or is not online yet?

`VideoCaptureCore` raises `OnNetworkSourceDisconnect` when an established stream is lost, and `OnError` when the first connection fails. In both cases stop the engine and start it again, typically after a delay. To avoid a full connection attempt, test the camera first:

```csharp
var uri = new Uri("rtsp://192.168.1.10/stream1");
bool online = VideoCapture1.IP_Camera_CheckAvailable(
    uri, 554, System.Net.Sockets.ProtocolType.Tcp, TimeSpan.FromSeconds(5));
```

Pass the real port: when the URL has none, `Uri.Port` returns the scheme default, or -1 for `rtsp://`, which .NET does not know. The default RTSP port is 554. The timeout covers the host name lookup and the TCP connection together. Only TCP is supported: a UDP check returns `false`. The check reports a refused connection through `OnError`, so run it on a background task and not from inside the `OnError` handler. The full set of reconnection options, including the automatic fallback switch of the X engines, is in [RTSP reconnection and fallback](../general/network-sources/reconnection-and-fallback.md).

### The low-latency RTSP engine connects but shows nothing

`IPSourceEngine.RTSP_LowLatency` needs the camera to report its frame rate. When a camera does not, set it yourself:

```csharp
VideoCapture1.IP_Camera_Source.Type = IPSourceEngine.RTSP_LowLatency;
VideoCapture1.IP_Camera_Source.ForcedFramerate = VideoFrameRate.FPS_25;
```

Also try UDP (`RTSP_LowLatency_UseUDP = true`), and check that the MP4 redistributable is deployed — the low-latency source is part of it.

### How do I show "Connecting…" until the first frame arrives?

Overlays and effects are drawn on video frames, so they cannot appear before the first frame. Use the status overlay of the `VideoView` instead; it is drawn while the engine is started but the source is not connected yet:

```csharp
VideoView1.StatusOverlay = new TextStatusOverlay
{
    TextConnecting = "Connecting to the camera..."
};
```

`TextStatusOverlay` also has `Font`, `Brush` and alignment properties. `ImageStatusOverlay` and `GIFStatusOverlay` show a picture or an animation instead of text.

### How do I record many IP cameras without high CPU usage?

The CPU goes into decoding, re-encoding and drawing every stream:

- Record the original stream without re-encoding where the format allows it — see [Save the original RTSP stream](../mediablocks/Guides/rtsp-save-original-stream.md).
- When you have to re-encode, use a GPU encoder (NVENC, Intel Quick Sync, AMD AMF) — see [H.264 encoders](../general/video-encoders/h264.md).
- Disable rendering for cameras nobody watches: `VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;`.

## Recording

### How do I start and stop recording without stopping the preview?

Use separate capture: set `SeparateCapture_Enabled = true`, choose a capture mode (`VideoCaptureMode.VideoCapture`, `IPCapture`, `ScreenCapture` and so on, not a preview mode) and set `Output_Format`. With `SeparateCapture_AutostartCapture` off (the default), `StartAsync` then runs only the preview, except for device DirectCapture outputs, which record from the start; call `SeparateCapture_StartAsync(filename)` and `SeparateCapture_StopAsync()` as often as you need. The preview keeps running and every recording is a separate file. Details and the X-engine equivalent are in [Separate capture](video-capture/separate-capture.md).

Video effects such as a text overlay are applied before the stream is split, so they are recorded. For an overlay that is only visible in the preview, use the on-screen display (OSD) API.

### Can I play a file while it is still being recorded?

Not an MP4 file: its index is written when the recording stops, so the file cannot be opened until then. Record to MPEG-TS instead (`MPEGTSOutput`, the same H.264 and AAC codecs). An MPEG-TS file can be read while it grows. Another option is to record in segments and play the finished ones.

### How do I record sound from several audio devices into one file?

Select the main device as usual and add the others before you start:

```csharp
VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("Microphone (USB Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_Add("Line In (Realtek Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_MixChannels = true;
VideoCapture1.Audio_RecordAudio = true;
```

With `Additional_Audio_CaptureDevice_MixChannels = true` the main and the first additional device are mixed into one audio track; without it, each device is written as its own audio stream, which not every player shows. With `VideoCaptureCoreX`, mix the sources in a Media Blocks pipeline with an `AudioMixerBlock` — see [Media Blocks audio processing](../mediablocks/AudioProcessing/index.md). A microphone is not exclusive: the same device can feed several recordings or applications at once.

## Devices

### How do I see a camera that was plugged in after the application started?

The device lists are read when the engine is created. Call `RefreshLists` to read them again, for example from the `OnDeviceAdded` and `OnDeviceRemoved` events:

```csharp
VideoCapture1.OnDeviceAdded += (s, e) =>
{
    VideoCapture1.RefreshLists(true, false, false);
    var cameras = VideoCapture1.Video_CaptureDevices();
    var microphones = VideoCapture1.Audio_CaptureDevices();
};
```

### "Unable to run filter graph", error 80004005

The DirectShow graph of `VideoCaptureCore` could not start. A common cause is the video renderer: in a virtual machine, over Remote Desktop or with an old display driver, select another one (`VideoRendererMode.EVR`, `VMR9` or `VideoRenderer`) in `Video_Renderer.VideoRenderer`. Check that the device works in the Main Demo, and that another application is not holding it. If it still fails, [send the logs](../general/sendlogs.md).

## Snapshots

### How do I take a snapshot of the live video?

With `VideoCaptureCore`, call `Frame_GetCurrent()` to get a `Bitmap`, or `Frame_SaveAsync(filename, ImageFormat.Jpeg)` to write a file. To process every frame, use the `OnVideoFrameBitmap` or `OnVideoFrameBuffer` event.

With `VideoCaptureCoreX`, enable the snapshot grabber **before** `StartAsync` — it is off by default to save resources:

```csharp
core.Snapshot_Grabber_Enabled = true;
await core.StartAsync();

// later
await core.Snapshot_SaveAsync("snapshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
```

## Overlays and screen capture

### How do I show a date and time that is not the system clock?

Generate the text yourself. With the X engines, `OverlayManagerText.TextProvider` is called for every frame with the frame timestamp, so you can return any time you calculate — see [Text that changes on every frame](../general/video-effects/text-overlay.md#text-that-changes-on-every-frame). With `VideoCaptureCore`, keep a `VideoEffectTextLogo`, change its `Text` from a timer and call `Update()`:

```csharp
textLogo.Text = myClock.ToString("yyyy-MM-dd HH:mm:ss");
textLogo.Update();
```

Change only the properties that really change; rebuilding the whole effect on every tick adds delay.

### Screen capture records only part of the screen on a 4K or scaled monitor

This happens when Windows display scaling is above 100% and the application is not DPI aware: Windows then gives the application scaled coordinates, while the capture works in physical pixels. Make the application per-monitor DPI aware (an `app.manifest` setting for WPF, `Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` for WinForms). Alternatively set `FullScreen = false` and pass the exact rectangle of the screen in physical pixels with `Left`, `Top`, `Right` and `Bottom` of `ScreenCaptureSourceSettings`.

## Output to other applications

### How do I send the captured video to a virtual camera?

Set `VideoCapture1.Virtual_Camera_Output_Enabled = true` before `StartAsync`. Other applications (Zoom, Teams, OBS, browsers) then see the video, with all effects and overlays, as the VisioForge virtual camera. The virtual camera itself comes from [Virtual Camera SDK](../../directshow/virtual-camera-sdk/index.md), which has to be installed or deployed with its registered filters. A working sample is [virtual-camera-streaming](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/virtual-camera-streaming).

### How do I stream to an RTMP server that needs authentication?

With `VideoCaptureCore`, use the `RTMP_FFMPEG_EXE` streaming format and fill the FFmpeg output with the FLV profile, which RTMP requires:

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
ffmpegOutput.FillDefaults(DefaultsProfile.FLV_H264_AAC, true);
```

Credentials are part of the URL your streaming server gives you — usually a stream key in the path, or `rtmp://login:password@host/app/stream` for servers with login authentication. More options, including the X engines, are in [RTMP streaming](../general/network-streaming/rtmp.md).
