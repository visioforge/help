---
title: Video Capture SDK para Delphi — guía TVFVideoCapture
description: Captura de vídeo y audio en Delphi con TVFVideoCapture — enumeración de dispositivos, preview, grabación MP4/AVI y cámaras IP.
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

# Video Capture SDK para Delphi

[Video Capture SDK Delphi](https://www.visioforge.com/all-in-one-media-framework){ .md-button .md-button--primary target="_blank" }

Esta sección describe cómo usar el Video Capture SDK con Delphi para crear aplicaciones de captura de vídeo y audio. El componente `TVFVideoCapture` (declarado en `VideoCaptureMain.pas`) es un descendiente de `TCustomPanel`, por lo que actúa tanto como superficie de previsualización como expone toda la API de captura mediante propiedades publicadas, métodos y eventos.

## Componente principal

### TVFVideoCapture

`TVFVideoCapture` proporciona funcionalidad completa de captura de vídeo y audio. Puede arrastrarlo a un formulario desde la paleta del IDE o crearlo en tiempo de ejecución. Como desciende de `TCustomPanel`, establezca `Parent` y `Align` igual que en cualquier otro contenedor VCL.

```pascal
var
  VideoCapture1: TVFVideoCapture;
begin
  VideoCapture1 := TVFVideoCapture.Create(Self);
  VideoCapture1.Parent := Panel1;
  VideoCapture1.Align := alClient;
end;
```

## Enumeración de dispositivos

Rellene combo-boxes con los dispositivos de captura de vídeo y audio detectados en el host. El SDK expone pares `_GetCount` / `_GetItem(Index)` para cada lista de dispositivos.

```pascal
procedure TForm1.EnumerateDevices;
var
  i: Integer;
begin
  // Dispositivos de captura de vídeo
  cbVideoDevices.Clear;
  for i := 0 to VideoCapture1.Video_CaptureDevices_GetCount - 1 do
    cbVideoDevices.Items.Add(VideoCapture1.Video_CaptureDevices_GetItem(i));

  // Dispositivos de captura de audio
  cbAudioDevices.Clear;
  for i := 0 to VideoCapture1.Audio_CaptureDevices_GetCount - 1 do
    cbAudioDevices.Items.Add(VideoCapture1.Audio_CaptureDevices_GetItem(i));
end;
```

## Previsualización

Cambie el componente a `Mode_Video_Preview` para mostrar el vídeo en directo en el panel sin escribir nada en disco. La monitorización de audio se controla mediante `Audio_PlayAudio`.

```pascal
procedure TForm1.StartPreview;
begin
  // Selección de dispositivos por su nombre de visualización
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;

  // Modo previsualización (sin salida a fichero)
  VideoCapture1.Mode := Mode_Video_Preview;
  VideoCapture1.Audio_PlayAudio := True;

  VideoCapture1.Start;
end;

procedure TForm1.StopPreview;
begin
  VideoCapture1.Stop;
end;
```

## Captura a un fichero

Cambie a `Mode_Video_Capture` y asigne `Output_Filename` y `Output_Format`. La enumeración `TVFOutputFormat` lista todos los contenedores que el componente puede escribir (`Format_MP4`, `Format_AVI`, `Format_WMV`, `Format_DV`, `Format_WebM`, `Format_FFMPEG`, `Format_FFMPEGX`, etc.).

```pascal
procedure TForm1.StartCapture;
begin
  VideoCapture1.Video_CaptureDevice := cbVideoDevices.Text;
  VideoCapture1.Audio_CaptureDevice := cbAudioDevices.Text;
  VideoCapture1.Audio_RecordAudio := True;

  // Fichero de salida y contenedor
  VideoCapture1.Output_Filename := 'C:\Videos\capture.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  // Cambia al modo captura
  VideoCapture1.Mode := Mode_Video_Capture;

  VideoCapture1.Start;
end;
```

Puede cambiar el fichero de salida al vuelo, sin detener el grafo, llamando a `OutputFilename_ChangeOnTheFly`.

## Capturas de fotograma

Guarde el fotograma actual de la previsualización en disco en cualquiera de los valores `TVFImageFormat` admitidos (`IM_BMP`, `IM_JPEG`, `IM_PNG`, `IM_GIF`, `IM_TIFF`). El tercer parámetro es la calidad JPEG (se ignora para los demás formatos).

```pascal
procedure TForm1.TakeSnapshot;
begin
  VideoCapture1.Frame_Save('C:\Photos\snapshot.jpg', IM_JPEG, 85);
end;
```

Para obtener el fotograma directamente en un `TBitmap` (por ejemplo, para mostrarlo en un `TImage`), utilice `Frame_GetCurrent`:

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

## Cámaras IP (RTSP / HTTP)

Cambie el modo a `Mode_IP_Preview` o `Mode_IP_Capture` y establezca `IP_Camera_URL`. Use `IP_Camera_Type` para elegir el motor y el protocolo — por ejemplo `IP_RTSP_TCP` para RTSP sobre TCP mediante el motor FFmpeg, o `IP_Auto_VLC` para recurrir a la fuente VLC incluida.

```pascal
procedure TForm1.ConnectIPCamera;
begin
  VideoCapture1.IP_Camera_URL := 'rtsp://user:password@192.168.1.100:554/stream';
  VideoCapture1.IP_Camera_Type := IP_RTSP_TCP;
  VideoCapture1.Mode := Mode_IP_Preview;
  VideoCapture1.Start;
end;
```

## Captura de pantalla

El componente graba el escritorio, una pantalla individual, una región o una ventana. Defina el rectángulo de captura (o active la pantalla completa), elija una frecuencia de fotogramas y, a continuación, cambie a `Mode_Screen_Capture`.

```pascal
procedure TForm1.StartScreenCapture;
begin
  // Captura a pantalla completa a 30 fps con el cursor del ratón incluido
  VideoCapture1.Screen_Capture_FullScreen := True;
  VideoCapture1.Screen_Capture_FrameRate := 30.0;
  VideoCapture1.Screen_Capture_Grab_Mouse_Cursor := True;

  VideoCapture1.Output_Filename := 'C:\Videos\screen.mp4';
  VideoCapture1.Output_Format := Format_MP4;

  VideoCapture1.Mode := Mode_Screen_Capture;
  VideoCapture1.Start;
end;
```

Para capturar una región, desactive `Screen_Capture_FullScreen` y proporcione `Screen_Capture_Left`, `Screen_Capture_Top`, `Screen_Capture_Right` y `Screen_Capture_Bottom` (todos en píxeles). Consulte [Implementación de captura de pantalla](screen-capture.md) para ver el conjunto completo de opciones.

## Efectos de vídeo

Los efectos (brillo, contraste, saturación, volteo, desenfoque, escala de grises, sepia y muchos más) se gestionan con `Video_Effect_Ex`. Active primero la canalización de efectos con `Video_Effects_Enabled := True` y luego añada cada efecto mediante su valor `TVFEffectType`. El argumento `Amount` es la intensidad del efecto — para `ef_contrast` desplaza el contraste, para `ef_flip_right` funciona como un indicador de activación, etc.

```pascal
procedure TForm1.ApplyVideoEffects;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=1, aplicado a todo el grafo (StartTime=0, StopTime=0), activado,
  // efecto de contraste con intensidad = 20
  VideoCapture1.Video_Effect_Ex(1, 0, 0, True, ef_contrast, 20.0, '');

  // ID=2, volteo horizontal
  VideoCapture1.Video_Effect_Ex(2, 0, 0, True, ef_flip_right, 0.0, '');
end;
```

Elimine efectos individuales con `Video_Effects_Remove(ID)` o bórrelos todos con `Video_Effects_Clear`.

## Superposición de texto

Utilice `Video_Effects_Text_Logo` (superposición GDI clásica) o `Video_Effects_Text_Logo_Plus` (superposición GDI+ moderna con degradados, rotación y efectos de contorno) para incrustar texto en el flujo de vídeo. El ejemplo siguiente emplea `Video_Effects_Text_Logo_Plus`.

```pascal
procedure TForm1.AddTextOverlay;
begin
  VideoCapture1.Video_Effects_Enabled := True;

  // ID=10, duración completa (0..0), activado, «My Video» en (10, 10),
  // Arial 24, sin negrita/cursiva/subrayado/tachado, color blanco
  VideoCapture1.Video_Effects_Text_Logo_Plus(
    10, 0, 0, True, 'My Video', 10, 10,
    'Arial', 24, False, False, False, False, clWhite);
end;
```

## Eventos

`TVFVideoCapture` dispara tres eventos principales de ciclo de vida: `OnStart`, `OnStop` y `OnError`. Ninguno lleva un parámetro `Sender` — `OnError` recibe el mensaje de error como `WideString`.

```pascal
procedure TForm1.VideoCapture1Start;
begin
  Button1.Caption := 'Detener';
end;

procedure TForm1.VideoCapture1Stop;
begin
  Button1.Caption := 'Iniciar';
end;

procedure TForm1.VideoCapture1Error(ErrorText: WideString);
begin
  ShowMessage('Error de captura: ' + ErrorText);
end;
```

Otros eventos cubren el acceso en directo a tramas (`OnVideoFrame`, `OnAudioFrame`), la detección de movimiento (`OnMotion`), la actividad del ratón y del teclado sobre la superficie de previsualización, los valores de los vúmetros, los eventos de transporte DV y la búsqueda de canales del sintonizador de TV.

## Recursos de desarrollo

Para una guía de implementación detallada, consulte estos recursos esenciales:

- [Registro de cambios completo e historial de versiones](changelog.md)
- [Guía de instalación y configuración](install/index.md)
- [Mejores prácticas de despliegue](deployment.md)
- [Información de licencia y EULA](../../eula.md)
- [Documentación API completa](https://api.visioforge.org/delphi/video_capture_sdk/index.html)

## Tutoriales de implementación

### Grabación y procesamiento de audio

Domine la captura de audio con estas guías paso a paso:

- [Implementación de captura de audio MP3](audio-capture-mp3.md) — Aprenda a capturar flujos de audio y a codificarlos directamente al formato MP3 con tasas de bits y ajustes de calidad configurables.
- [Grabación de audio WAV con opciones de compresión](audio-capture-wav.md) — Implemente una grabación de audio WAV de alta calidad con códecs de compresión opcionales y configuraciones de formato.
- [Configuración de los dispositivos de salida de audio](audio-output.md) — Guía para seleccionar y configurar los dispositivos de salida de audio para la monitorización y reproducción en sus aplicaciones.

### Captura de vídeo y control de dispositivos

Aprenda las técnicas esenciales de manejo de vídeo:

- [Implementación de captura de vídeo AVI](video-capture-avi.md) — Desarrolle aplicaciones que capturen flujos de vídeo al formato AVI con códecs y ajustes de contenedor personalizables.
- [Control e integración de videocámaras DV](dv-camcorder.md) — Conecte y controle videocámaras DV mediante FireWire/IEEE-1394 con controles de transporte y gestión de metadatos.
- [Selección de dispositivos para fuentes de vídeo y audio](video-audio-sources.md) — Técnicas para enumerar, seleccionar y gestionar varios dispositivos de captura en sus aplicaciones.
- [Parámetros de ajuste de vídeo por hardware](hardware-adjustments.md) — Acceda y modifique los parámetros a nivel de dispositivo, como brillo, contraste, saturación y balance de blancos.
- [Configuración de la entrada de vídeo mediante Crossbar](video-input-crossbar.md) — Aprenda a configurar el enrutamiento de entrada de vídeo a través de interfaces crossbar para dispositivos de captura multi-entrada.
- [Selección y configuración del renderizador de vídeo](video-renderer.md) — Elija y configure el motor de renderizado de vídeo óptimo para su aplicación de captura.

### Técnicas multimedia avanzadas

Explore escenarios de implementación sofisticados:

- [Configuración de formato de salida personalizado](custom-output.md) — Cree formatos de salida especializados con ajustes de compresión y configuraciones de contenedor a medida.
- [Integración de radio FM y sintonizador de TV](fm-radio-tv-tuning.md) — Implemente la recepción de radio FM y la sintonización de canales de TV en aplicaciones con el hardware compatible.
- [Streaming en red con formato WMV](network-streaming-wmv.md) — Transmita vídeo capturado por la red usando el formato Windows Media Video con optimización del ancho de banda.
- [Gestión de resolución con redimensionado y recorte](resize-crop.md) — Procese tramas de vídeo con redimensionado y recorte dinámicos para conseguir dimensiones de salida personalizadas.
- [Implementación de captura de pantalla](screen-capture.md) — Capture el contenido de la pantalla con frecuencias de fotogramas y capacidades de selección de región configurables.
- [Captura de fichero DV con opciones de compresión](video-capture-dv.md) — Guarde el vídeo directamente en formato DV o con recompresión para optimizar las necesidades de almacenamiento.
- [Captura MPEG-2 con integración del sintonizador de TV](mpeg2-capture.md) — Aproveche los codificadores MPEG-2 por hardware de los sintonizadores de TV para una captura broadcast eficiente y de alta calidad.
- [Captura Windows Media Video con perfiles externos](video-capture-wmv.md) — Implemente la codificación Windows Media Video con configuraciones de perfil externas para una calidad y un tamaño optimizados.
