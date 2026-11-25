---
title: Audio bidireccional con cámaras IP en C# .NET (ONVIF)
description: Envíe audio del micrófono al altavoz de una cámara IP por el backchannel de audio RTSP de ONVIF en C# para intercomunicadores, timbres y megafonía.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - VideoCaptureCoreX
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - IP Camera
  - RTSP
  - ONVIF
  - Audio
  - C#
primary_api_classes:
  - RTSPSourceSettings
  - RTSPSourceBlock
  - VideoCaptureCoreX
  - ONVIFClientX

---

# Audio bidireccional con cámaras IP (backchannel de audio ONVIF)

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

Muchas cámaras IP, videoporteros e intercomunicadores tienen un altavoz integrado o una salida de audio. El SDK puede enviar audio a ese altavoz por el **backchannel de audio RTSP de ONVIF** mientras recibe el vídeo y el audio de la cámara en la misma sesión RTSP. Usos típicos: hablar con un visitante en la puerta, un aviso de megafonía a través de una cámara o un intercomunicador bidireccional.

## Cómo funciona

El backchannel de audio ONVIF es una pista de audio `sendonly` en la descripción RTSP (SDP) de la cámara. Cuando `RTSPSourceSettings.AudioBackchannel` es `true`, el SDK solicita el backchannel (`Require: www.onvif.org/ver20/backchannel`), localiza esa pista y le envía su audio como RTP por el transporte que ya usa la sesión (TCP o UDP).

```mermaid
flowchart LR
    Mic[Micrófono u otra fuente de audio] --> BC["RTSPSourceBlock.BackchannelInput"]
    BC -->|RTP G.711| Cam[Altavoz de la cámara IP]
    Cam -->|H.264/H.265 + audio| Src[Salidas de RTSPSourceBlock]
```

- El audio se envía como **G.711 mu-law (PCMU), 8000 Hz, mono**, el códec de backchannel que ONVIF exige a toda cámara con salida de audio. Una cámara cuyo backchannel ofrece PCMA/8000 pero no PCMU/8000 recibe **G.711 A-law (PCMA)**. Cualquier formato de entrada se convierte.
- La cámara debe ofrecer un backchannel. Las cámaras ONVIF Profile S y Profile T con salida de audio lo ofrecen. Una cámara que no admite la cabecera `Require` de ONVIF puede rechazar la conexión, así que active `AudioBackchannel` solo en cámaras con altavoz.
- El backchannel necesita GStreamer 1.22 o posterior. Los paquetes de runtime del SDK para Windows, macOS, Android e iOS lo incluyen; en Linux, donde se usa el GStreamer del sistema, eso significa Ubuntu 24.04 u otra distribución con 1.22 o posterior. Con un runtime más antiguo, el SDK registra un error que lo indica y no envía audio; el vídeo y el audio de la cámara siguen reproduciéndose.
- El audio se envía tal como se captura: la cámara lo recibe sin que se le sume el búfer de recepción (`RTSPSourceSettings.Latency`).
- Si el backchannel de la cámara no incluye ni PCMU/8000 ni PCMA/8000, el SDK registra un error con los códecs que ofrece la cámara, y el vídeo y el audio de la cámara siguen reproduciéndose.

## Video Capture SDK .NET (VideoCaptureCoreX)

Asigne a `Audio_BackchannelSource` el audio que desea enviar. Abre el backchannel de la fuente RTSP; usted no tiene que activar `AudioBackchannel`, y su `RTSPSourceSettings` no se modifica. Admite los mismos tipos de fuente que `Audio_Source`, creados como una fuente aparte, así que no asigne el mismo dispositivo de captura a ambas.

```cs
var core = new VideoCaptureCoreX(VideoView1 as IVideoView);

// La cámara.
var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/cam/realmonitor?channel=1&subtype=0"),
    "admin", "password", audioEnabled: true);
core.Video_Source = rtsp;

// El sonido propio de la cámara, reproducido en los altavoces predeterminados del PC.
var outputs = await DeviceEnumerator.Shared.AudioOutputsAsync();
var speakers = outputs.FirstOrDefault(device => device.IsDefault) ?? outputs.FirstOrDefault();
if (speakers != null) core.Audio_OutputDevice = new AudioRendererSettings(speakers);

// El micrófono que habla por el altavoz de la cámara.
var mic = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
core.Audio_BackchannelSource = mic.CreateSourceSettingsVC();

await core.StartAsync();
```

`Audio_Source` es independiente: cuando es `null`, el micrófono propio de la cámara se sigue reproduciendo en `Audio_OutputDevice` y grabando en las salidas, de modo que usted oye al visitante mientras él le oye a usted.

## Media Blocks SDK .NET (RTSPSourceBlock)

Con `AudioBackchannel` activado, `RTSPSourceBlock` tiene una entrada de audio, `BackchannelInput`. Conecte a ella cualquier bloque de audio antes del inicio.

```cs
var pipeline = new MediaBlocksPipeline();

var rtsp = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.22:554/cam/realmonitor?channel=1&subtype=0"),
    "admin", "password", audioEnabled: true);
rtsp.AudioBackchannel = true;

var camera = new RTSPSourceBlock(rtsp);

// Vídeo y audio de la cámara hacia este PC.
pipeline.Connect(camera.VideoOutput, new VideoRendererBlock(pipeline, VideoView1).Input);
// AudioOutput es null si la fuente se creó con audioEnabled: false o la información de la cámara no muestra audio; en modo de compatibilidad con audio activado existe aunque la cámara no envíe audio.
if (camera.AudioOutput != null)
{
    pipeline.Connect(camera.AudioOutput, new AudioRendererBlock().Input);
}

// Micrófono hacia el altavoz de la cámara.
var mic = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
var micSource = new SystemAudioSourceBlock(mic.CreateSourceSettings());
pipeline.Connect(micSource.Output, camera.BackchannelInput);

await pipeline.StartAsync();
```

La entrada del backchannel acepta cualquier bloque de audio: un archivo mediante `UniversalSourceBlock` para un aviso grabado, o `VirtualAudioSourceBlock` para un tono de prueba. El audio se envía en tiempo real.

Si `BackchannelInput` se deja sin conectar, no se solicita ningún backchannel y el bloque se comporta como sin `AudioBackchannel`. El backchannel lo admite `RTSPSourceBlock`; `FallbackSwitchSourceBlock` (y `FallbackSwitch` en `VideoCaptureCoreX`) no lo abre.

## Volumen del altavoz por ONVIF

`ONVIFClientX` lee y establece la configuración de la salida de audio de la cámara, incluido el volumen del altavoz (`OutputLevel`).

```cs
var onvif = new ONVIFClientX();
await onvif.ConnectAsync("http://192.168.1.22/onvif/device_service", "admin", "password");

var outputs = await onvif.GetAudioOutputsAsync();            // vacío: la cámara no tiene altavoz
var configs = await onvif.GetAudioOutputConfigurationsAsync();
if (configs?.Length > 0)
{
    var config = configs[0];
    var options = await onvif.GetAudioOutputConfigurationOptionsAsync(config.token);

    config.OutputLevel = options.OutputLevelRange.Max;       // el más alto
    await onvif.SetAudioOutputConfigurationAsync(config);
}
```

`GetAudioOutputsAsync` también sirve como comprobación rápida antes de activar el backchannel: una cámara que no informa de ninguna salida de audio no tiene altavoz al que enviar.

## Solución de problemas

- **La conexión falla tras activar `AudioBackchannel`**: la cámara rechaza la solicitud de backchannel ONVIF. Compruebe que tiene una salida de audio (`GetAudioOutputsAsync`) y que el audio bidireccional está activado en su interfaz web.
- **No se oye nada en el altavoz de la cámara**: revise en el registro de errores los códecs que ofrece la cámara, suba `OutputLevel` y compruebe que ningún otro cliente (una aplicación del fabricante o un NVR) ocupa el canal de conversación de la cámara.
- **El micrófono de la cámara no capta su propio altavoz**: es lo esperado. Las cámaras con altavoz lo cancelan en su micrófono (cancelación de eco, o un `SendPrimacy` semidúplex como `www.onvif.org/ver20/HalfDuplex/Auto` que silencia el micrófono mientras suena el altavoz), así que usted no se oye a sí mismo. Compruebe el altavoz de oído.

## Documentación relacionada

- [Tutorial de audio bidireccional con cámara IP](../../video-tutorials/ip-camera-two-way-audio.md) — una aplicación WPF completa: vista previa de la cámara con su sonido y el micrófono del PC al altavoz de la cámara
- [Flujos de cámaras RTSP](rtsp.md) — conexión, transporte y ajustes de baja latencia
- [Integración de cámaras IP ONVIF](onvif.md) — descubrimiento, perfiles y control PTZ
- [Directorio de marcas de cámaras IP](../../../camera-brands/index.md) — URL RTSP de cada marca
