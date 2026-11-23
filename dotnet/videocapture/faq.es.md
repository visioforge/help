---
title: Preguntas frecuentes de Video Capture SDK .NET — RTSP
description: Respuestas sobre Video Capture SDK .NET — usuario y reconexión RTSP, grabar sin detener la vista previa, capturas, dispositivos y superposiciones.
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

# Preguntas frecuentes de Video Capture SDK .NET

Respuestas a las preguntas que los desarrolladores hacen con más frecuencia sobre Video Capture SDK .NET. Se cubren los dos motores: `VideoCaptureCore` (Windows, DirectShow) y `VideoCaptureCoreX` (multiplataforma). Los errores de despliegue como `Unable to create filter` están reunidos en [Solución de problemas](../general/troubleshooting.md).

## Cámaras IP

### ¿Cómo me conecto a una cámara IP que necesita usuario y contraseña?

Pase las credenciales en la configuración de la fuente. Con `VideoCaptureCore`:

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

Con `VideoCaptureCoreX` o Media Blocks SDK .NET, `RTSPSourceSettings.CreateAsync(uri, login, password, audioEnabled)` las recibe como argumentos. La mayoría de las cámaras también aceptan la forma `rtsp://login:password@host:port/path`, pero las propiedades separadas evitan problemas con caracteres especiales en la contraseña.

La URL debe ser la dirección del propio flujo de video, no la de la página web de la cámara. La ruta es distinta para cada fabricante: consulte el manual de la cámara, las [guías por marca de cámara](../camera-brands/index.md), o descubra el flujo con [ONVIF](video-sources/ip-cameras/onvif.md). Puede usarse cualquier cámara compatible con RTSP, ONVIF o HTTP MJPEG, y no importa si está conectada por Wi-Fi o por cable. Una prueba rápida: si VLC abre la URL, el SDK también puede abrirla. Con `VideoCaptureCore`, una URL que no se abre se notifica con un mensaje que empieza por `Unable to set URL to network FFMPEG source filter`, `Unable to set URL to LAV Splitter Source filter` o `Unable to set URL to network VLC source filter`, según el motor.

### ¿Cómo me reconecto cuando la cámara se desconecta o todavía no está en línea?

`VideoCaptureCore` genera `OnNetworkSourceDisconnect` cuando se pierde un flujo ya establecido, y `OnError` cuando falla la primera conexión. En ambos casos detenga el motor y vuelva a iniciarlo, normalmente tras una pausa. Para evitar un intento de conexión completo, compruebe antes la cámara:

```csharp
var uri = new Uri("rtsp://192.168.1.10/stream1");
bool online = VideoCapture1.IP_Camera_CheckAvailable(
    uri, 554, System.Net.Sockets.ProtocolType.Tcp, TimeSpan.FromSeconds(5));
```

Pase el puerto real: cuando la URL no tiene ninguno, `Uri.Port` devuelve el predeterminado del esquema, o -1 para `rtsp://`, que .NET no conoce. El puerto RTSP predeterminado es 554. El tiempo de espera cubre a la vez la resolución del nombre del host y la conexión TCP. Solo se admite TCP: una comprobación UDP devuelve `false`. La comprobación informa de una conexión rechazada mediante `OnError`, así que ejecútela en una tarea en segundo plano y no dentro del controlador de `OnError`. El conjunto completo de opciones de reconexión, incluido el conmutador de respaldo automático de los motores X, está en [Reconexión y respaldo RTSP](../general/network-sources/reconnection-and-fallback.md).

### El motor RTSP de baja latencia se conecta pero no muestra nada

`IPSourceEngine.RTSP_LowLatency` necesita que la cámara informe de su velocidad de fotogramas. Cuando una cámara no lo hace, establézcala usted mismo:

```csharp
VideoCapture1.IP_Camera_Source.Type = IPSourceEngine.RTSP_LowLatency;
VideoCapture1.IP_Camera_Source.ForcedFramerate = VideoFrameRate.FPS_25;
```

Pruebe también UDP (`RTSP_LowLatency_UseUDP = true`) y compruebe que el redistribuible MP4 está desplegado: la fuente de baja latencia forma parte de él.

### ¿Cómo muestro "Conectando…" hasta que llega el primer fotograma?

Las superposiciones y los efectos se dibujan sobre los fotogramas de video, así que no pueden aparecer antes del primero. Use en su lugar la superposición de estado del `VideoView`; se dibuja mientras el motor está iniciado pero la fuente aún no está conectada:

```csharp
VideoView1.StatusOverlay = new TextStatusOverlay
{
    TextConnecting = "Connecting to the camera..."
};
```

`TextStatusOverlay` también tiene propiedades `Font`, `Brush` y de alineación. `ImageStatusOverlay` y `GIFStatusOverlay` muestran una imagen o una animación en lugar de texto.

### ¿Cómo grabo muchas cámaras IP sin un uso alto de CPU?

La CPU se dedica a decodificar, recodificar y dibujar cada flujo:

- Grabe el flujo original sin recodificar cuando el formato lo permita; consulte [Guardar el flujo RTSP original](../mediablocks/Guides/rtsp-save-original-stream.md).
- Cuando tenga que recodificar, use un codificador de GPU (NVENC, Intel Quick Sync, AMD AMF); consulte [Codificadores H.264](../general/video-encoders/h264.md).
- Desactive el renderizado de las cámaras que nadie mira: `VideoCapture1.Video_Renderer.VideoRenderer = VideoRendererMode.None;`.

## Grabación

### ¿Cómo inicio y detengo la grabación sin detener la vista previa?

Use la captura separada: establezca `SeparateCapture_Enabled = true`, elija un modo de captura (`VideoCaptureMode.VideoCapture`, `IPCapture`, `ScreenCapture`, etc., no un modo de vista previa) y configure `Output_Format`. Con `SeparateCapture_AutostartCapture` desactivado (el valor predeterminado), `StartAsync` ejecuta entonces solo la vista previa, salvo con las salidas DirectCapture del dispositivo, que graban desde el inicio; llame a `SeparateCapture_StartAsync(filename)` y `SeparateCapture_StopAsync()` tantas veces como necesite. La vista previa sigue en marcha y cada grabación es un archivo distinto. Los detalles y el equivalente para el motor X están en [Captura separada](video-capture/separate-capture.md).

Los efectos de video, como una superposición de texto, se aplican antes de que se divida el flujo, así que se graban. Para una superposición visible solo en la vista previa, use la API de visualización en pantalla (OSD).

### ¿Puedo reproducir un archivo mientras todavía se está grabando?

No un archivo MP4: su índice se escribe cuando termina la grabación, por lo que el archivo no puede abrirse hasta entonces. Grabe en MPEG-TS en su lugar (`MPEGTSOutput`, con los mismos códecs H.264 y AAC). Un archivo MPEG-TS puede leerse mientras crece. Otra opción es grabar por segmentos y reproducir los ya terminados.

### ¿Cómo grabo el sonido de varios dispositivos de audio en un solo archivo?

Seleccione el dispositivo principal como siempre y añada los demás antes de iniciar:

```csharp
VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("Microphone (USB Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_Add("Line In (Realtek Audio)");
VideoCapture1.Additional_Audio_CaptureDevice_MixChannels = true;
VideoCapture1.Audio_RecordAudio = true;
```

Con `Additional_Audio_CaptureDevice_MixChannels = true`, el dispositivo principal y el primer dispositivo adicional se mezclan en una sola pista de audio; sin ella, cada dispositivo se escribe como un flujo de audio propio, que no todos los reproductores muestran. Con `VideoCaptureCoreX`, mezcle las fuentes en una canalización de Media Blocks con un `AudioMixerBlock`; consulte [Procesamiento de audio en Media Blocks](../mediablocks/AudioProcessing/index.md). Un micrófono no es exclusivo: el mismo dispositivo puede alimentar varias grabaciones o aplicaciones a la vez.

## Dispositivos

### ¿Cómo veo una cámara que se conectó después de iniciar la aplicación?

Las listas de dispositivos se leen cuando se crea el motor. Llame a `RefreshLists` para volver a leerlas, por ejemplo desde los eventos `OnDeviceAdded` y `OnDeviceRemoved`:

```csharp
VideoCapture1.OnDeviceAdded += (s, e) =>
{
    VideoCapture1.RefreshLists(true, false, false);
    var cameras = VideoCapture1.Video_CaptureDevices();
    var microphones = VideoCapture1.Audio_CaptureDevices();
};
```

### "Unable to run filter graph", error 80004005

El grafo DirectShow de `VideoCaptureCore` no pudo iniciarse. Una causa habitual es el renderizador de video: en una máquina virtual, por Escritorio remoto o con un controlador de pantalla antiguo, seleccione otro (`VideoRendererMode.EVR`, `VMR9` o `VideoRenderer`) en `Video_Renderer.VideoRenderer`. Compruebe que el dispositivo funciona en la Main Demo y que otra aplicación no lo está ocupando. Si sigue fallando, [envíe los registros](../general/sendlogs.md).

## Capturas de imagen

### ¿Cómo tomo una captura del video en directo?

Con `VideoCaptureCore`, llame a `Frame_GetCurrent()` para obtener un `Bitmap`, o a `Frame_SaveAsync(filename, ImageFormat.Jpeg)` para escribir un archivo. Para procesar cada fotograma, use el evento `OnVideoFrameBitmap` u `OnVideoFrameBuffer`.

Con `VideoCaptureCoreX`, active el capturador de instantáneas **antes** de `StartAsync`; está desactivado por defecto para ahorrar recursos:

```csharp
core.Snapshot_Grabber_Enabled = true;
await core.StartAsync();

// más tarde
await core.Snapshot_SaveAsync("snapshot.jpg", SkiaSharp.SKEncodedImageFormat.Jpeg, 90);
```

## Superposiciones y captura de pantalla

### ¿Cómo muestro una fecha y hora que no es el reloj del sistema?

Genere el texto usted mismo. Con los motores X, `OverlayManagerText.TextProvider` se llama en cada fotograma con la marca de tiempo del fotograma, así que puede devolver cualquier hora que calcule; consulte [Texto que cambia en cada fotograma](../general/video-effects/text-overlay.md#texto-que-cambia-en-cada-fotograma). Con `VideoCaptureCore`, conserve un `VideoEffectTextLogo`, cambie su `Text` desde un temporizador y llame a `Update()`:

```csharp
textLogo.Text = myClock.ToString("yyyy-MM-dd HH:mm:ss");
textLogo.Update();
```

Cambie solo las propiedades que realmente cambian; reconstruir todo el efecto en cada tic añade retraso.

### La captura de pantalla graba solo una parte de la pantalla en un monitor 4K o escalado

Ocurre cuando el escalado de pantalla de Windows supera el 100 % y la aplicación no es consciente del DPI: Windows le entrega entonces coordenadas escaladas, mientras que la captura trabaja en píxeles físicos. Haga que la aplicación sea consciente del DPI por monitor (un ajuste de `app.manifest` en WPF, `Application.SetHighDpiMode(HighDpiMode.PerMonitorV2)` en WinForms). Como alternativa, establezca `FullScreen = false` y pase el rectángulo exacto de la pantalla en píxeles físicos con `Left`, `Top`, `Right` y `Bottom` de `ScreenCaptureSourceSettings`.

## Salida hacia otras aplicaciones

### ¿Cómo envío el video capturado a una cámara virtual?

Establezca `VideoCapture1.Virtual_Camera_Output_Enabled = true` antes de `StartAsync`. Las demás aplicaciones (Zoom, Teams, OBS, navegadores) ven entonces el video, con todos los efectos y superposiciones, como la cámara virtual de VisioForge. La cámara virtual en sí procede de [Virtual Camera SDK](../../directshow/virtual-camera-sdk/index.md), que debe estar instalado o desplegado con sus filtros registrados. Un ejemplo funcional es [virtual-camera-streaming](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/virtual-camera-streaming).

### ¿Cómo transmito a un servidor RTMP que requiere autenticación?

Con `VideoCaptureCore`, use el formato de transmisión `RTMP_FFMPEG_EXE` y rellene la salida FFmpeg con el perfil FLV, que RTMP requiere:

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
ffmpegOutput.FillDefaults(DefaultsProfile.FLV_H264_AAC, true);
```

Las credenciales forman parte de la URL que le da su servidor de transmisión: normalmente una clave de flujo en la ruta, o `rtmp://login:password@host/app/stream` en los servidores con autenticación por usuario. Más opciones, incluidos los motores X, en [Transmisión RTMP](../general/network-streaming/rtmp.md).
