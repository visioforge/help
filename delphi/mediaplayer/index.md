---
title: Media Player SDK for Delphi - TVFMediaPlayer Guide
description: Embed video and audio playback in Delphi apps with TVFMediaPlayer, including playback controls, seeking, volume, frame capture, audio tracks, and fullscreen.
sidebar_label: TVFMediaPlayer
tags:
  - All-in-One Media Framework
  - Delphi
  - ActiveX
  - DirectShow
  - Windows
  - VCL
  - Playback
  - Streaming
primary_api_classes:
  - TVFMediaPlayer

---

# Media Player SDK for Delphi

[Media Player SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

This page covers using Media Player SDK with Delphi to build multimedia playback applications around the `TVFMediaPlayer` VCL component.

## Main Component

### TVFMediaPlayer

`TVFMediaPlayer` is a `TCustomPanel` descendant that exposes the full playback API. Drop it on a form at design time, or create it programmatically and attach it to a host panel.

```pascal
var
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);
  MediaPlayer1.Parent := Panel1;
  MediaPlayer1.Align := alClient;
end;
```

## Basic Playback

### Play a File

Set `FilenameOrURL` to the input path (or network URL), pick a source engine via `Source_Mode`, then call `Play`.

```pascal
procedure TForm1.PlayFile(const Filename: WideString);
begin
  MediaPlayer1.FilenameOrURL := Filename;
  MediaPlayer1.Source_Mode := SM_File_FFMPEG; // SM_File_DS, SM_File_VLC, SM_File_LAV
  MediaPlayer1.Audio_Play := True;
  MediaPlayer1.Play;
end;
```

### Playback Controls

`Play` returns `True` on success. `Pause`, `Resume`, and `Stop` are procedures.

```pascal
procedure TForm1.btnPlayClick(Sender: TObject);
begin
  MediaPlayer1.Play;
end;

procedure TForm1.btnPauseClick(Sender: TObject);
begin
  MediaPlayer1.Pause;
end;

procedure TForm1.btnResumeClick(Sender: TObject);
begin
  MediaPlayer1.Resume;
end;

procedure TForm1.btnStopClick(Sender: TObject);
begin
  MediaPlayer1.Stop;
end;
```

The current state is exposed by the read-only `Status` property (`ST_PLAY`, `ST_PAUSE`, `ST_FREE`).

```pascal
if MediaPlayer1.Status = ST_PLAY then
  StatusBar1.SimpleText := 'Playing';
```

## Seeking

Position is expressed in milliseconds. `Position_Get_Time` returns the current position, `Position_Set_Time` seeks to an absolute position, and `Info_Video_DurationMSec(0)` returns the total duration of the first video stream.

```pascal
procedure TForm1.SeekTo(PositionMs: Integer);
begin
  MediaPlayer1.Position_Set_Time(PositionMs);
end;

function TForm1.GetDurationMs: Integer;
begin
  Result := MediaPlayer1.Info_Video_DurationMSec(0);
end;

function TForm1.GetPositionMs: Integer;
begin
  Result := MediaPlayer1.Position_Get_Time;
end;
```

Frame-accurate seeking is also available via `Position_Get_Frame` and `Position_Set_Frame`.

## Audio Control

`TVFMediaPlayer` supports up to eight independent audio output streams. Volume and balance are addressed by zero-based stream index.

```pascal
procedure TForm1.SetVolume(StreamIndex, Volume: Integer);
begin
  // Volume range: 0..100 (SDK scales internally)
  MediaPlayer1.Audio_Volume_Set(StreamIndex, Volume);
end;

function TForm1.GetVolume(StreamIndex: Integer): Integer;
begin
  Result := MediaPlayer1.Audio_Volume_Get(StreamIndex);
end;

procedure TForm1.SetBalance(StreamIndex, Balance: Integer);
begin
  // Balance range: -100 (left) to +100 (right)
  MediaPlayer1.Audio_Balance_Set(StreamIndex, Balance);
end;
```

To mute audio, set the volume to zero and restore the previous value on unmute, or toggle `Audio_Play` before starting playback.

```pascal
procedure TForm1.Mute;
begin
  FSavedVolume := MediaPlayer1.Audio_Volume_Get(0);
  MediaPlayer1.Audio_Volume_Set(0, 0);
end;

procedure TForm1.Unmute;
begin
  MediaPlayer1.Audio_Volume_Set(0, FSavedVolume);
end;
```

## Playback Speed

`SetSpeed` accepts a multiplier between 0.01 and 100.0.

```pascal
procedure TForm1.SetPlaybackSpeed(Rate: Double);
begin
  // Rate: 0.5 (half speed) to 2.0 (double speed)
  MediaPlayer1.SetSpeed(Rate);
end;
```

## Network URL Playback

The same `FilenameOrURL` property accepts network URLs. Pick the engine that best supports the protocol — `SM_File_VLC` and `SM_File_FFMPEG` cover the broadest set of streaming sources; `SM_MMS_WMV_DS` is dedicated to MMS/WMV streams.

```pascal
procedure TForm1.PlayURL(const URL: WideString);
begin
  MediaPlayer1.FilenameOrURL := URL;
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;

procedure TForm1.PlayRTSP;
begin
  MediaPlayer1.FilenameOrURL := 'rtsp://192.168.1.100:554/stream';
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;

procedure TForm1.PlayHLS;
begin
  MediaPlayer1.FilenameOrURL := 'https://server.example.com/playlist.m3u8';
  MediaPlayer1.Source_Mode := SM_File_FFMPEG;
  MediaPlayer1.Play;
end;
```

## Frame Capture

`Frame_Save` writes the current frame to disk in the chosen image format. `Frame_GetCurrent` fills a `TBitmap` you supply.

```pascal
procedure TForm1.CaptureFrame;
begin
  // Frame_Save(Filename, Format, Quality)
  MediaPlayer1.Frame_Save('C:\Snapshots\frame.jpg', IM_JPEG, 85);
end;

procedure TForm1.CaptureFrameToBitmap;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    MediaPlayer1.Frame_GetCurrent(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

Optionally resize the saved frame by configuring `Frame_Save_Resize`, `Frame_Save_Resize_Width`, and `Frame_Save_Resize_Height` before the call.

## Audio and Subtitle Tracks

The `Info_Audio_*` and `Info_Text_*` helpers enumerate streams discovered inside the source file. Audio streams are enabled or disabled individually with `Audio_SetStream`.

```pascal
procedure TForm1.PopulateAudioTracks;
var
  i: Integer;
begin
  cbAudioTracks.Items.Clear;
  for i := 0 to MediaPlayer1.Info_Audio_Streams_Count - 1 do
    cbAudioTracks.Items.Add(MediaPlayer1.Info_Audio_Codec(i));
end;

procedure TForm1.SelectAudioTrack(Index: Integer);
var
  i: Integer;
begin
  // Enable the chosen track and disable the others
  for i := 0 to MediaPlayer1.Info_Audio_Streams_Count - 1 do
    MediaPlayer1.Audio_SetStream(i, i = Index);
end;

procedure TForm1.PopulateSubtitles;
var
  i: Integer;
begin
  cbSubtitles.Items.Clear;
  for i := 0 to MediaPlayer1.Info_Text_Streams_Count - 1 do
    cbSubtitles.Items.Add(MediaPlayer1.Info_Text_Name(i));
end;
```

`Info_Text_Language(i)` and `Info_Text_Codec(i)` are also available for richer subtitle metadata.

## Fullscreen Mode

Toggle fullscreen via the `Screen_VR_FullScreen` property. This works with the configured video renderer.

```pascal
procedure TForm1.ToggleFullscreen;
begin
  MediaPlayer1.Screen_VR_FullScreen := not MediaPlayer1.Screen_VR_FullScreen;
end;
```

## Events

`TVFMediaPlayer` exposes three core playback events. `OnStart` and `OnStop` take no parameters; `OnError` receives the error text.

```pascal
procedure TForm1.MediaPlayer1Start;
begin
  StatusBar1.SimpleText := 'Playing';
end;

procedure TForm1.MediaPlayer1Stop;
begin
  StatusBar1.SimpleText := 'Stopped';
end;

procedure TForm1.MediaPlayer1Error(ErrorText: WideString);
begin
  ShowMessage('Error: ' + ErrorText);
end;
```

To track position updates, poll `Position_Get_Time` from a `TTimer` while `Status = ST_PLAY`:

```pascal
procedure TForm1.PositionTimerTick(Sender: TObject);
var
  Position, Duration: Integer;
begin
  if MediaPlayer1.Status <> ST_PLAY then
    Exit;

  Position := MediaPlayer1.Position_Get_Time;
  Duration := MediaPlayer1.Info_Video_DurationMSec(0);
  if Duration > 0 then
  begin
    TrackBar1.Max := Duration;
    TrackBar1.Position := Position;
  end;
  LabelPosition.Caption := Format('%d:%.2d / %d:%.2d',
    [Position div 60000, (Position div 1000) mod 60,
     Duration div 60000, (Duration div 1000) mod 60]);
end;
```

## Supported Formats

| Type | Formats |
|------|---------|
| Video | MP4, AVI, MKV, MOV, WMV, FLV, WebM |
| Audio | MP3, AAC, FLAC, WAV, OGG, WMA |
| Streaming | RTSP, RTMP, HTTP, HLS, DASH |

Format coverage depends on the selected `Source_Mode` — the FFmpeg and VLC engines cover the broadest range out of the box; DirectShow (`SM_File_DS`) relies on the codecs installed on the system.

## Resources and Further Information

To explore the capabilities and usage of the `TVFMediaPlayer` library in more depth, see the following official resources:

* **Product page:** [VisioForge Media Player SDK](https://www.visioforge.com/all-in-one-media-framework)
* **API reference:** [Delphi Media Player API Reference](https://api.visioforge.org/delphi/media_player_sdk/index.html)
* **Changelog:** [Recent updates and fixes](changelog.md)
* **Installation guide:** [Setting up the library](install/index.md)
* **Deployment:** [Distributing your application](deployment.md)
* **License agreement:** [End User License Agreement](../../eula.md)

## Tutorials and Code Samples

Practical examples demonstrating how to implement specific features:

* [How to play a video file with multiple video streams?](file-multiple-video-streams.md)
* *(More tutorials will be added here as they become available)*
