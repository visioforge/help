---
title: Media Player SDK para Delphi
description: Guía completa para usar Media Player SDK con Delphi/Object Pascal para reproducción multimedia avanzada con soporte de múltiples formatos y streaming.
---

# Media Player SDK para Delphi

[Media Player SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Esta sección cubre el uso de Media Player SDK con Delphi para aplicaciones de reproducción multimedia.

## Componente Principal

### TMediaPlayer

El componente `TMediaPlayer` proporciona capacidades completas de reproducción multimedia.

```pascal
var
  MediaPlayer1: TMediaPlayer;
begin
  MediaPlayer1 := TMediaPlayer.Create(Self);
  MediaPlayer1.Parent := Panel1;
  MediaPlayer1.Align := alClient;
end;
```

## Reproducción Básica

### Reproducir Archivo

```pascal
procedure TForm1.PlayFile(const Filename: string);
begin
  MediaPlayer1.Filename := Filename;
  MediaPlayer1.Play;
end;
```

### Controles de Reproducción

```pascal
procedure TForm1.Play;
begin
  MediaPlayer1.Play;
end;

procedure TForm1.Pause;
begin
  MediaPlayer1.Pause;
end;

procedure TForm1.Stop;
begin
  MediaPlayer1.Stop;
end;

procedure TForm1.Resume;
begin
  MediaPlayer1.Resume;
end;
```

## Navegación

```pascal
procedure TForm1.SeekTo(PositionMs: Int64);
begin
  MediaPlayer1.Position := PositionMs;
end;

function TForm1.GetDuration: Int64;
begin
  Result := MediaPlayer1.Duration;
end;

function TForm1.GetPosition: Int64;
begin
  Result := MediaPlayer1.Position;
end;
```

## Control de Audio

```pascal
procedure TForm1.SetVolume(Value: Integer);
begin
  // Volume: 0 a 100
  MediaPlayer1.Volume := Value;
end;

procedure TForm1.Mute;
begin
  MediaPlayer1.Mute := True;
end;

procedure TForm1.Unmute;
begin
  MediaPlayer1.Mute := False;
end;

procedure TForm1.SetBalance(Value: Integer);
begin
  // Balance: -100 (izquierda) a 100 (derecha)
  MediaPlayer1.Balance := Value;
end;
```

## Velocidad de Reproducción

```pascal
procedure TForm1.SetPlaybackSpeed(Rate: Double);
begin
  // Rate: 0.5 (mitad) a 2.0 (doble)
  MediaPlayer1.PlaybackRate := Rate;
end;
```

## Reproducción de URLs

```pascal
procedure TForm1.PlayURL(const URL: string);
begin
  MediaPlayer1.Filename := URL;
  MediaPlayer1.Play;
end;

procedure TForm1.PlayRTSP;
begin
  MediaPlayer1.Filename := 'rtsp://192.168.1.100:554/stream';
  MediaPlayer1.Play;
end;

procedure TForm1.PlayHLS;
begin
  MediaPlayer1.Filename := 'https://servidor.com/playlist.m3u8';
  MediaPlayer1.Play;
end;
```

## Captura de Fotogramas

```pascal
procedure TForm1.CaptureFrame;
begin
  MediaPlayer1.Snapshot_Save('C:\Capturas\frame.jpg');
end;

procedure TForm1.CaptureFrameToMemory;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    MediaPlayer1.Snapshot_Get(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

## Pistas de Audio y Subtítulos

```pascal
procedure TForm1.GetAudioTracks;
var
  i: Integer;
begin
  ComboBoxAudioTracks.Clear;
  for i := 0 to MediaPlayer1.Audio_Streams_Count - 1 do
  begin
    ComboBoxAudioTracks.Items.Add(MediaPlayer1.Audio_Stream_Name(i));
  end;
end;

procedure TForm1.SelectAudioTrack(Index: Integer);
begin
  MediaPlayer1.Audio_Stream_Index := Index;
end;

procedure TForm1.GetSubtitles;
var
  i: Integer;
begin
  ComboBoxSubtitles.Clear;
  for i := 0 to MediaPlayer1.Subtitles_Streams_Count - 1 do
  begin
    ComboBoxSubtitles.Items.Add(MediaPlayer1.Subtitles_Stream_Name(i));
  end;
end;

procedure TForm1.SelectSubtitle(Index: Integer);
begin
  MediaPlayer1.Subtitles_Stream_Index := Index;
end;
```

## Modo Pantalla Completa

```pascal
procedure TForm1.ToggleFullscreen;
begin
  MediaPlayer1.Fullscreen := not MediaPlayer1.Fullscreen;
end;
```

## Eventos

```pascal
procedure TForm1.MediaPlayer1Play(Sender: TObject);
begin
  StatusBar1.SimpleText := 'Reproduciendo';
end;

procedure TForm1.MediaPlayer1Pause(Sender: TObject);
begin
  StatusBar1.SimpleText := 'Pausado';
end;

procedure TForm1.MediaPlayer1Stop(Sender: TObject);
begin
  StatusBar1.SimpleText := 'Detenido';
end;

procedure TForm1.MediaPlayer1End(Sender: TObject);
begin
  StatusBar1.SimpleText := 'Fin del archivo';
end;

procedure TForm1.MediaPlayer1Error(Sender: TObject; ErrorCode: Integer; ErrorText: string);
begin
  ShowMessage('Error: ' + ErrorText);
end;

procedure TForm1.MediaPlayer1PositionChange(Sender: TObject; Position, Duration: Int64);
begin
  TrackBar1.Max := Duration;
  TrackBar1.Position := Position;
  LabelPosition.Caption := Format('%d:%02d / %d:%02d', 
    [Position div 60000, (Position div 1000) mod 60,
     Duration div 60000, (Duration div 1000) mod 60]);
end;
```

## Formatos Soportados

| Tipo | Formatos |
|------|----------|
| Video | MP4, AVI, MKV, MOV, WMV, FLV, WebM |
| Audio | MP3, AAC, FLAC, WAV, OGG, WMA |
| Streaming | RTSP, RTMP, HTTP, HLS, DASH |
