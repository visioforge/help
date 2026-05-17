---
title: Media Player SDK para Delphi — Guía del desarrollador
description: Reproducción de vídeo y audio en Delphi con TVFMediaPlayer — control, búsqueda, volumen, captura de fotogramas y pantalla completa.
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

# Media Player SDK para Delphi

[Media Player SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Esta página describe cómo utilizar Media Player SDK con Delphi para crear aplicaciones de reproducción multimedia basadas en el componente VCL `TVFMediaPlayer`.

## Componente principal

### TVFMediaPlayer

`TVFMediaPlayer` es un descendiente de `TCustomPanel` que expone la API completa de reproducción. Colóquelo sobre un formulario en tiempo de diseño, o créelo por programa y asócielo a un panel anfitrión.

```pascal
var
  MediaPlayer1: TVFMediaPlayer;
begin
  MediaPlayer1 := TVFMediaPlayer.Create(Self);
  MediaPlayer1.Parent := Panel1;
  MediaPlayer1.Align := alClient;
end;
```

## Reproducción básica

### Reproducir un archivo

Asigne a `FilenameOrURL` la ruta de entrada (o URL de red), elija un motor de origen mediante `Source_Mode` y, a continuación, llame a `Play`.

```pascal
procedure TForm1.PlayFile(const Filename: WideString);
begin
  MediaPlayer1.FilenameOrURL := Filename;
  MediaPlayer1.Source_Mode := SM_File_FFMPEG; // SM_File_DS, SM_File_VLC, SM_File_LAV
  MediaPlayer1.Audio_Play := True;
  MediaPlayer1.Play;
end;
```

### Controles de reproducción

`Play` devuelve `True` si tiene éxito. `Pause`, `Resume` y `Stop` son procedimientos.

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

El estado actual se expone mediante la propiedad de solo lectura `Status` (`ST_PLAY`, `ST_PAUSE`, `ST_FREE`).

```pascal
if MediaPlayer1.Status = ST_PLAY then
  StatusBar1.SimpleText := 'Playing';
```

## Búsqueda de posición

La posición se expresa en milisegundos. `Position_Get_Time` devuelve la posición actual, `Position_Set_Time` se desplaza a una posición absoluta e `Info_Video_DurationMSec(0)` devuelve la duración total del primer flujo de vídeo.

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

La búsqueda con precisión de fotograma también está disponible mediante `Position_Get_Frame` y `Position_Set_Frame`.

## Control de audio

`TVFMediaPlayer` admite hasta ocho flujos de salida de audio independientes. El volumen y el balance se direccionan por índice de flujo con base cero.

```pascal
procedure TForm1.SetVolume(StreamIndex, Volume: Integer);
begin
  // Rango de volumen: 0..100 (el SDK lo escala internamente)
  MediaPlayer1.Audio_Volume_Set(StreamIndex, Volume);
end;

function TForm1.GetVolume(StreamIndex: Integer): Integer;
begin
  Result := MediaPlayer1.Audio_Volume_Get(StreamIndex);
end;

procedure TForm1.SetBalance(StreamIndex, Balance: Integer);
begin
  // Rango de balance: -100 (izquierda) a +100 (derecha)
  MediaPlayer1.Audio_Balance_Set(StreamIndex, Balance);
end;
```

Para silenciar el audio, ponga el volumen a cero y restaure el valor anterior al reactivarlo, o alterne `Audio_Play` antes de iniciar la reproducción.

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

## Velocidad de reproducción

`SetSpeed` acepta un multiplicador entre 0,01 y 100,0.

```pascal
procedure TForm1.SetPlaybackSpeed(Rate: Double);
begin
  // Rate: 0.5 (mitad de velocidad) a 2.0 (doble velocidad)
  MediaPlayer1.SetSpeed(Rate);
end;
```

## Reproducción desde URL de red

La misma propiedad `FilenameOrURL` acepta URL de red. Elija el motor que mejor admita el protocolo — `SM_File_VLC` y `SM_File_FFMPEG` cubren el conjunto más amplio de fuentes de streaming; `SM_MMS_WMV_DS` está dedicado a flujos MMS/WMV.

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

## Captura de fotogramas

`Frame_Save` guarda el fotograma actual en disco en el formato de imagen elegido. `Frame_GetCurrent` rellena un `TBitmap` que usted proporciona.

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

Opcionalmente puede redimensionar el fotograma guardado configurando `Frame_Save_Resize`, `Frame_Save_Resize_Width` y `Frame_Save_Resize_Height` antes de la llamada.

## Pistas de audio y subtítulos

Los auxiliares `Info_Audio_*` e `Info_Text_*` enumeran los flujos detectados dentro del archivo de origen. Las pistas de audio se activan o desactivan individualmente con `Audio_SetStream`.

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
  // Activar la pista elegida y desactivar las demás
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

`Info_Text_Language(i)` e `Info_Text_Codec(i)` también están disponibles para obtener metadatos de subtítulos más completos.

## Modo de pantalla completa

Alterne el modo de pantalla completa mediante la propiedad `Screen_VR_FullScreen`. Funciona con el renderizador de vídeo configurado.

```pascal
procedure TForm1.ToggleFullscreen;
begin
  MediaPlayer1.Screen_VR_FullScreen := not MediaPlayer1.Screen_VR_FullScreen;
end;
```

## Eventos

`TVFMediaPlayer` expone tres eventos principales de reproducción. `OnStart` y `OnStop` no reciben parámetros; `OnError` recibe el texto del error.

```pascal
procedure TForm1.MediaPlayer1Start;
begin
  StatusBar1.SimpleText := 'Reproduciendo';
end;

procedure TForm1.MediaPlayer1Stop;
begin
  StatusBar1.SimpleText := 'Detenido';
end;

procedure TForm1.MediaPlayer1Error(ErrorText: WideString);
begin
  ShowMessage('Error: ' + ErrorText);
end;
```

Para seguir las actualizaciones de posición, consulte `Position_Get_Time` desde un `TTimer` mientras `Status = ST_PLAY`:

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

## Formatos compatibles

| Tipo | Formatos |
|------|----------|
| Vídeo | MP4, AVI, MKV, MOV, WMV, FLV, WebM |
| Audio | MP3, AAC, FLAC, WAV, OGG, WMA |
| Streaming | RTSP, RTMP, HTTP, HLS, DASH |

La cobertura de formatos depende del `Source_Mode` seleccionado — los motores FFmpeg y VLC cubren la gama más amplia desde el principio; DirectShow (`SM_File_DS`) depende de los códecs instalados en el sistema.

## Recursos e información adicional

Para profundizar en las capacidades y el uso de la biblioteca `TVFMediaPlayer`, consulte los siguientes recursos oficiales:

* **Página del producto:** [VisioForge Media Player SDK](https://www.visioforge.com/all-in-one-media-framework)
* **Referencia de la API:** [Referencia de la API Delphi Media Player](https://api.visioforge.org/delphi/media_player_sdk/index.html)
* **Registro de cambios:** [Actualizaciones y correcciones recientes](changelog.md)
* **Guía de instalación:** [Configuración de la biblioteca](install/index.md)
* **Implementación:** [Distribución de su aplicación](deployment.md)
* **Acuerdo de licencia:** [Acuerdo de licencia de usuario final](../../eula.md)

## Tutoriales y ejemplos de código

Ejemplos prácticos que muestran cómo implementar funciones específicas:

* [¿Cómo reproducir un archivo de vídeo con múltiples flujos de vídeo?](file-multiple-video-streams.md)
* *(Se añadirán más tutoriales aquí a medida que estén disponibles)*
