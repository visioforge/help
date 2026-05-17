---
title: Delphi Video Capture SDK - TVFVideoCapture Full Guide
description: Build Delphi video and audio capture apps with TVFVideoCapture, including device enumeration, preview, MP4/AVI recording, screen capture, and IP cameras.
sidebar_label: TVFVideoCapture
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - Windows
  - VCL
  - Capture
  - Streaming
primary_api_classes:
  - TVFVideoCapture

---

# Video Capture SDK for Delphi

[Video Capture SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

This section covers using the Video Capture SDK with Delphi to build video and audio capture applications. The `TVFVideoCapture` component (declared in `VideoCaptureMain.pas`) is a `TCustomPanel` descendant, so it both renders the preview surface and exposes the full capture API as published properties, methods, and events.

## Main Component

### TVFVideoCapture

`TVFVideoCapture` provides complete video and audio capture functionality. You can drop it on a form from the IDE palette or create it at runtime. As it descends from `TCustomPanel`, set `Parent` and `Align` like any other VCL container.

```pascal
var
  VideoCapture1: TVFVideoCapture;
begin
  VideoCapture1 := TVFVideoCapture.Create(Self);
  VideoCapture1.Parent := Panel1;
  VideoCapture1.Align := alClient;
end;
```

## Device Enumeration

Populate combo boxes with the video and audio capture devices discovered on the host. The SDK exposes `_GetCount` / `_GetItem(Index)` pairs for every device list.

```pascal
procedure TForm1.EnumerateDevices;
var
  i: Integer;
begin
  // Video capture devices
  cbVideoDevices.Clear;
  for i := 0 to VideoCapture1.Video_CaptureDevices_GetCount - 1 do
    cbVideoDevices.Items.Add(VideoCapture1.Video_CaptureDevices_GetItem(i));

  // Audio capture devices
  cbAudioDevices.Clear;
  for i := 0 to VideoCapture1.Audio_CaptureDevices_GetCount - 1 do
    cbAudioDevices.Items.Add(VideoCapture1.Audio_CaptureDevices_GetItem(i));
end;
```

## Preview

Switch the component to `Mode_Video_Preview` to render live video to the panel without writing anything to disk. Audio monitoring is controlled by `Audio_PlayAudio`.

```pascal
procedure TForm1.StartPreview;
begin
  // Select devices by display name
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;

  // Preview mode (no file output)
  VideoCapture1.Mode := Mode_Video_Preview;
  VideoCapture1.Audio_PlayAudio := True;

  VideoCapture1.Start;
end;

procedure TForm1.StopPreview;
begin
  VideoCapture1.Stop;
end;
```

## Capturing to a File

Switch to `Mode_Video_Capture` and assign `Output_Filename` and `Output_Format`. The `TVFOutputFormat` enum lists every container the component can write (`Format_MP4`, `Format_AVI`, `Format_WMV`, `Format_DV`, `Format_WebM`, `Format_FFMPEG`, `Format_FFMPEGX`, and so on).

```pascal
procedure TForm1.StartCapture;
begin
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;
  VideoCapture1.Audio_RecordAudio := True;

  // Output file and container
  VideoCapture1.Output_Filename := 'C:\Videos\capture.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  // Switch to capture mode
  VideoCapture1.Mode := Mode_Video_Capture;

  VideoCapture1.Start;
end;
```

You can change the output file on the fly without stopping the graph by calling `OutputFilename_ChangeOnTheFly`.

## Snapshots

Save the current preview frame to disk in any of the supported `TVFImageFormat` values (`IM_BMP`, `IM_JPEG`, `IM_PNG`, `IM_GIF`, `IM_TIFF`). The third parameter is the JPEG quality (ignored for other formats).

```pascal
procedure TForm1.TakeSnapshot;
begin
  VideoCapture1.Frame_Save('C:\Photos\snapshot.jpg', IM_JPEG, 85);
end;
```

To pull the frame straight into a `TBitmap` (for example to display in a `TImage`), use `Frame_GetCurrent`:

```pascal
procedure TForm1.TakeSnapshotToMemory;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    VideoCapture1.Frame_GetCurrent(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

## IP Cameras (RTSP / HTTP)

Switch the mode to `Mode_IP_Preview` or `Mode_IP_Capture` and set `IP_Camera_URL`. Use `IP_Camera_Type` to pick the engine and protocol — for example `IP_RTSP_TCP` for RTSP over TCP via the FFmpeg engine, or `IP_Auto_VLC` to fall back to the bundled VLC source.

```pascal
procedure TForm1.ConnectIPCamera;
begin
  VideoCapture1.IP_Camera_URL := 'rtsp://user:password@192.168.1.100:554/stream';
  VideoCapture1.IP_Camera_Type := IP_RTSP_TCP;
  VideoCapture1.Mode := Mode_IP_Preview;
  VideoCapture1.Start;
end;
```

## Screen Capture

The component records the desktop, an individual display, a region, or a window. Set the capture rectangle (or enable full-screen), choose a frame rate, then switch to `Mode_Screen_Capture`.

```pascal
procedure TForm1.StartScreenCapture;
begin
  // Full-screen capture at 30 fps with the mouse cursor included
  VideoCapture1.Screen_Capture_FullScreen := True;
  VideoCapture1.Screen_Capture_FrameRate := 30.0;
  VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := True;

  VideoCapture1.Output_Filename := 'C:\Videos\screen.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  VideoCapture1.Mode := Mode_Screen_Capture;
  VideoCapture1.Start;
end;
```

For region capture, disable `Screen_Capture_FullScreen` and supply `Screen_Capture_Left`, `Screen_Capture_Top`, `Screen_Capture_Right`, and `Screen_Capture_Bottom` (all in pixels). See [Screen Capture Implementation](screen-capture.md) for the full set of options.

## Video Effects

Effects (brightness, contrast, saturation, flip, blur, grayscale, sepia, and many more) are managed with `Video_Effect_Ex`. Enable the effects pipeline first with `Video_Effects_Enabled := True`, then add each effect by `TVFEffectType`. The `Amount` argument is the strength of the effect — for `ef_contrast` it shifts contrast, for `ef_flip_right` it acts as an on/off flag, and so on.

```pascal
procedure TForm1.ApplyVideoEffects;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=1, applied for the full graph (StartTime=0, StopTime=0), enabled,
  // contrast effect with amount = 20
  VideoCapture1.Video_Effect_Ex(1, 0, 0, True, ef_contrast, 20.0, '');

  // ID=2, horizontal flip
  VideoCapture1.Video_Effect_Ex(2, 0, 0, True, ef_flip_right, 0.0, '');
end;
```

Remove individual effects with `Video_Effects_Remove(ID)` or clear them all with `Video_Effects_Clear`.

## Text Overlay

Use `Video_Effects_Text_Logo` (legacy GDI overlay) or `Video_Effects_Text_Logo_Plus` (modern GDI+ overlay with gradients, rotation, and outline effects) to burn text into the video stream. The example below uses `Video_Effects_Text_Logo_Plus`.

```pascal
procedure TForm1.AddTextOverlay;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=10, full duration (0..0), enabled, "My Video" at (10, 10),
  // Arial 24, not bold/italic/underline/strike-out, white color
  VideoCapture1.Video_Effects_Text_Logo_Plus(
    10, 0, 0, True, 'My Video', 10, 10,
    'Arial', 24, False, False, False, False, clWhite);
end;
```

## Events

`TVFVideoCapture` raises three core lifecycle events: `OnStart`, `OnStop`, and `OnError`. None of them carry a `Sender` parameter — `OnError` receives the error message as a `WideString`.

```pascal
procedure TForm1.VideoCapture1Start;
begin
  Button1.Caption := 'Stop';
end;

procedure TForm1.VideoCapture1Stop;
begin
  Button1.Caption := 'Start';
end;

procedure TForm1.VideoCapture1Error(ErrorText: WideString);
begin
  ShowMessage('Capture error: ' + ErrorText);
end;
```

Additional events cover live frame access (`OnVideoFrame`, `OnAudioFrame`), motion detection (`OnMotion`), mouse and keyboard activity on the preview surface, VU-meter values, DV transport events, and TV-tuner channel scanning.

## Development Resources

For detailed implementation guidance, explore these essential resources:

- [Complete Changelog and Version History](changelog.md)
- [Installation and Configuration Guide](install/index.md)
- [Deployment Best Practices](deployment.md)
- [Licensing Information and EULA](../../eula.md)
- [Comprehensive API Documentation](https://api.visioforge.org/delphi/video_capture_sdk/index.html)

## Implementation Tutorials

### Audio Recording and Processing

Master audio capture with these step-by-step guides:

- [MP3 Audio Capture Implementation](audio-capture-mp3.md) - Learn how to capture audio streams and encode them directly to MP3 format with configurable bitrates and quality settings.
- [WAV Audio Recording with Compression Options](audio-capture-wav.md) - Implement high-quality WAV audio recording with optional compression codecs and format configurations.
- [Configuring Audio Output Devices](audio-output.md) - Guide to selecting and configuring audio output devices for monitoring and playback in your applications.

### Video Capture and Device Control

Learn essential video handling techniques:

- [AVI Video Capture Implementation](video-capture-avi.md) - Develop applications that capture video streams to AVI format with customizable codecs and container settings.
- [DV Camcorder Control and Integration](dv-camcorder.md) - Connect and control DV camcorders through FireWire/IEEE-1394 with transport controls and metadata handling.
- [Device Selection for Video and Audio Sources](video-audio-sources.md) - Techniques for enumerating, selecting, and managing multiple capture devices in your applications.
- [Hardware Video Adjustment Parameters](hardware-adjustments.md) - Access and modify device-level parameters including brightness, contrast, saturation, and white balance.
- [Video Input Configuration via Crossbar](video-input-crossbar.md) - Learn to configure video input routing through crossbar interfaces for multi-input capture devices.
- [Video Renderer Selection and Configuration](video-renderer.md) - Choose and configure the optimal video rendering engine for your capture application.

### Advanced Media Techniques

Explore sophisticated implementation scenarios:

- [Custom Output Format Configuration](custom-output.md) - Create specialized output formats with custom compression settings and container configurations.
- [FM Radio and TV Tuner Integration](fm-radio-tv-tuning.md) - Implement FM radio reception and TV channel tuning in applications with supported hardware.
- [Network Streaming with WMV Format](network-streaming-wmv.md) - Stream captured video over networks using Windows Media Video format with bandwidth optimization.
- [Resolution Management with Resize and Crop](resize-crop.md) - Process video frames with dynamic resizing and cropping to achieve custom output dimensions.
- [Screen Capture Implementation](screen-capture.md) - Capture on-screen content with configurable frame rates and region selection capabilities.
- [DV File Capture with Compression Options](video-capture-dv.md) - Save video directly to DV format or with recompression for optimized storage requirements.
- [MPEG-2 Capture with TV Tuner Integration](mpeg2-capture.md) - Utilize hardware MPEG-2 encoders in TV tuners for efficient high-quality broadcast capture.
- [Windows Media Video Capture with External Profiles](video-capture-wmv.md) - Implement Windows Media Video encoding with external profile configurations for optimized quality and size.
