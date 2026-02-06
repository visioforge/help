---
title: Video Capture SDK para Delphi
description: Captura video desde cámaras, webcams y dispositivos usando Video Capture SDK con Delphi/Object Pascal. Incluye ejemplos de código completos.
---

# Video Capture SDK para Delphi

[Video Capture SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Esta sección cubre el uso de Video Capture SDK con Delphi para aplicaciones de captura de video.

## Componente Principal

### TVideoCapture

El componente `TVideoCapture` proporciona capacidades completas de captura de video.

```pascal
var
  VideoCapture1: TVideoCapture;
begin
  VideoCapture1 := TVideoCapture.Create(Self);
  VideoCapture1.Parent := Panel1;
  VideoCapture1.Align := alClient;
end;
```

## Enumeración de Dispositivos

```pascal
procedure TForm1.EnumerateDevices;
var
  i: Integer;
begin
  // Dispositivos de video
  ComboBoxVideo.Clear;
  for i := 0 to VideoCapture1.Video_CaptureDevices_Count - 1 do
  begin
    ComboBoxVideo.Items.Add(VideoCapture1.Video_CaptureDevice_Name(i));
  end;
  
  // Dispositivos de audio
  ComboBoxAudio.Clear;
  for i := 0 to VideoCapture1.Audio_CaptureDevices_Count - 1 do
  begin
    ComboBoxAudio.Items.Add(VideoCapture1.Audio_CaptureDevice_Name(i));
  end;
end;
```

## Vista Previa

```pascal
procedure TForm1.StartPreview;
begin
  // Configurar dispositivos
  VideoCapture1.Video_CaptureDevice := ComboBoxVideo.Text;
  VideoCapture1.Audio_CaptureDevice := ComboBoxAudio.Text;
  
  // Modo vista previa
  VideoCapture1.Mode := TVideoCaptureMode.VideoPreview;
  
  // Iniciar
  VideoCapture1.Start;
end;

procedure TForm1.StopPreview;
begin
  VideoCapture1.Stop;
end;
```

## Captura a Archivo

```pascal
procedure TForm1.StartCapture;
begin
  // Configurar dispositivos
  VideoCapture1.Video_CaptureDevice := ComboBoxVideo.Text;
  VideoCapture1.Audio_CaptureDevice := ComboBoxAudio.Text;
  
  // Configurar salida
  VideoCapture1.Output_Filename := 'C:\Videos\captura.mp4';
  VideoCapture1.Output_Format := TVideoOutputFormat.MP4;
  
  // Modo captura
  VideoCapture1.Mode := TVideoCaptureMode.VideoCapture;
  
  // Iniciar
  VideoCapture1.Start;
end;
```

## Captura de Fotos

```pascal
procedure TForm1.TakeSnapshot;
begin
  VideoCapture1.Snapshot_Save('C:\Fotos\foto.jpg');
end;

procedure TForm1.TakeSnapshotToMemory;
var
  Bitmap: TBitmap;
begin
  Bitmap := TBitmap.Create;
  try
    VideoCapture1.Snapshot_Get(Bitmap);
    Image1.Picture.Assign(Bitmap);
  finally
    Bitmap.Free;
  end;
end;
```

## Cámaras IP (RTSP)

```pascal
procedure TForm1.ConnectIPCamera;
begin
  VideoCapture1.IPCamera_URL := 'rtsp://usuario:contraseña@192.168.1.100:554/stream';
  VideoCapture1.Mode := TVideoCaptureMode.IPCamera;
  VideoCapture1.Start;
end;
```

## Captura de Pantalla

```pascal
procedure TForm1.StartScreenCapture;
begin
  VideoCapture1.Screen_Capture_Enabled := True;
  VideoCapture1.Screen_Capture_FrameRate := 30;
  VideoCapture1.Screen_Capture_CursorVisible := True;
  
  VideoCapture1.Output_Filename := 'C:\Videos\pantalla.mp4';
  VideoCapture1.Output_Format := TVideoOutputFormat.MP4;
  
  VideoCapture1.Mode := TVideoCaptureMode.ScreenCapture;
  VideoCapture1.Start;
end;
```

## Efectos de Video

```pascal
procedure TForm1.ApplyVideoEffects;
begin
  // Brillo (-100 a 100)
  VideoCapture1.Video_Effect_Brightness := 10;
  
  // Contraste (-100 a 100)
  VideoCapture1.Video_Effect_Contrast := 20;
  
  // Saturación (-100 a 100)
  VideoCapture1.Video_Effect_Saturation := 10;
  
  // Voltear horizontalmente
  VideoCapture1.Video_Effect_FlipHorizontal := True;
end;
```

## Superposición de Texto

```pascal
procedure TForm1.AddTextOverlay;
begin
  VideoCapture1.TextOverlay_Enabled := True;
  VideoCapture1.TextOverlay_Text := 'Mi Video';
  VideoCapture1.TextOverlay_X := 10;
  VideoCapture1.TextOverlay_Y := 10;
  VideoCapture1.TextOverlay_FontName := 'Arial';
  VideoCapture1.TextOverlay_FontSize := 24;
  VideoCapture1.TextOverlay_FontColor := clWhite;
end;
```

## Eventos

```pascal
procedure TForm1.VideoCapture1Start(Sender: TObject);
begin
  Button1.Caption := 'Detener';
end;

procedure TForm1.VideoCapture1Stop(Sender: TObject);
begin
  Button1.Caption := 'Iniciar';
end;

procedure TForm1.VideoCapture1Error(Sender: TObject; ErrorCode: Integer; ErrorText: string);
begin
  ShowMessage('Error de captura: ' + ErrorText);
end;
```
