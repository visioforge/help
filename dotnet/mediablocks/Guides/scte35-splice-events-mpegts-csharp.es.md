---
title: Marcadores SCTE-35 en flujos MPEG-TS y HLS con C# .NET
description: Lea marcadores SCTE-35 de MPEG-TS y HLS, consérvelos en un remux e insértelos en archivos TS y flujos UDP, SRT o RIST con C#.
sidebar_label: Eventos de empalme SCTE-35
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - HLS
  - SCTE-35
  - SRT
  - UDP
  - RIST
  - Ad insertion
  - C#
primary_api_classes:
  - MPEGTSDemuxBlock
  - UniversalSourceBlock
  - MPEGTSSinkBlock
  - UDPMPEGTSSinkBlock
  - SRTMPEGTSSinkBlock
  - RISTMPEGTSSinkBlock
  - VideoCaptureCoreX
  - SpliceEventArgs
---

# Marcadores SCTE-35 en MPEG-TS y HLS con C# .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Descripción general

Las secciones de empalme SCTE-35 marcan los puntos en los que un canal de difusión o FAST abandona la señal de la
red para una pausa publicitaria y vuelve a ella. Viajan en el flujo de transporte en un PID propio (tipo de flujo
0x86). Media Blocks SDK .NET le permite:

1. **Leerlas** de un archivo o flujo MPEG-TS (`MPEGTSDemuxBlock`), o de una fuente MPEG-TS o HLS decodificada por
   `UniversalSourceBlock`, como un evento `OnSpliceEvent`.
2. **Conservarlas** en un remux: un TS leído con SCTE-35 activado y escrito por `MPEGTSSinkBlock` con un PID SCTE-35
   lleva los mismos comandos de empalme en su salida.
3. **Insertar** sus propios comandos splice_insert en un archivo MPEG-TS, o en un flujo MPEG-TS enviado por
   UDP, SRT o RIST, desde un pipeline de Media Blocks o desde `VideoCaptureCoreX`.

SCTE-35 está desactivado por defecto: sin `EnableSCTE35`, el demultiplexor descarta el PID SCTE-35.

## Leer eventos de empalme de MPEG-TS

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new BasicFileSourceBlock("channel.ts");

var demux = new MPEGTSDemuxBlock(renderVideo: true, renderAudio: true);
demux.Settings.EnableSCTE35 = true;
demux.OnSpliceEvent += (sender, e) =>
{
    if (e.CommandType == SpliceCommandType.Insert)
    {
        var kind = e.OutOfNetwork ? "break starts" : "break ends";
        Console.WriteLine($"Event {e.EventId}: {kind} at {e.RunningTime}, duration {e.BreakDuration}");
    }
};

pipeline.Connect(source.Output, demux.Input);
// Conecte demux.VideoOutput y demux.AudioOutput a renderizadores, codificadores o sinks.

await pipeline.StartAsync();
```

`SpliceEventArgs` contiene:

| Propiedad | Significado |
|-----------|-------------|
| `Section` | La splice_info_section completa, desde table_id (0xFC) hasta CRC_32. Úsela para los descriptores (segmentation, avail) y para los comandos distintos de splice_insert y time_signal. |
| `CommandType` | `Insert` (splice_insert), `TimeSignal`, `Null`, `Schedule`, `BandwidthReservation` o `Private`. |
| `EventId`, `Cancel`, `OutOfNetwork`, `Immediate` | Los campos de splice_insert. `OutOfNetwork = true` inicia una pausa, `false` la termina. |
| `PtsTime` | El instante del empalme en el reloj PTS de 90 kHz del flujo, con pts_adjustment aplicado. |
| `RunningTime` | El mismo punto como tiempo de ejecución del pipeline, la base de tiempo de los búferes que salen del demultiplexor. Donde las marcas de tiempo del flujo saltan, el demultiplexor cierra el hueco y `RunningTime` sigue su salida, no el PTS original. `null` cuando el demultiplexor no puede convertirlo: la sección llega antes de conocer el reloj del flujo, o el punto queda antes del inicio de la salida. |
| `BreakDuration`, `AutoReturn` | El break_duration de un splice_insert. |
| `Encrypted`, `PtsAdjustment` | Cuando `Encrypted` está activado, solo se rellenan `Section` y `PtsAdjustment`. |

El evento se genera en un hilo de streaming cuando el demultiplexor alcanza la sección, normalmente unos segundos
antes del propio punto de empalme. Retorne rápido y pase el trabajo a su propio hilo o al despachador de la interfaz.

## Leer eventos de empalme de HLS

`UniversalSourceBlock` genera el mismo evento para una fuente MPEG-TS y para un flujo HLS de segmentos MPEG-TS. Las
secciones llegan a la aplicación después de pasar por los decodificadores:

```csharp
var settings = await UniversalSourceSettings.CreateAsync(new Uri("https://example.com/live/index.m3u8"));
settings.EnableSCTE35 = true;

var source = new UniversalSourceBlock(settings);
source.OnSpliceEvent += (sender, e) => Console.WriteLine($"{e.CommandType} {e.EventId} at {e.RunningTime}");
```

Se leen las secciones SCTE-35 dentro de los segmentos TS. Las etiquetas de la lista de reproducción
(`#EXT-X-CUE-OUT`, `#EXT-X-DATERANGE`) no.

## Conservar los eventos de empalme en un remux

Active `EnableSCTE35` en el demultiplexor y `SCTE35PID` en el sink. Cada sección viaja con los flujos, y el
multiplexor la escribe en el nuevo PID con su temporización ajustada a la salida:

```csharp
var demux = new MPEGTSDemuxBlock(renderVideo: true, renderAudio: true);
demux.Settings.EnableSCTE35 = true;
pipeline.Connect(source.Output, demux.Input);

var videoParse = new H264ParseBlock();
pipeline.Connect(demux.VideoOutput, videoParse.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts") { SCTE35PID = 500 });
pipeline.Connect(videoParse.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));
```

## Insertar eventos de empalme en una salida MPEG-TS

Con `SCTE35PID` establecido, `MPEGTSSinkBlock` escribe un flujo SCTE-35 con un latido splice_null cada 5 minutos, e
`InsertSpliceOut` / `InsertSpliceIn` añaden comandos splice_insert mientras el pipeline se ejecuta:

```csharp
var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("channel.ts") { SCTE35PID = 500 });
pipeline.Connect(h264Encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// Una pausa de 30 segundos que empieza a los 10 s de tiempo de ejecución, anunciada con antelación.
sink.InsertSpliceOut(eventId: 1, spliceTime: TimeSpan.FromSeconds(10), duration: TimeSpan.FromSeconds(30));
// Espere hasta unos 5 s antes de que termine la pausa y anuncie el regreso.
await Task.Delay(TimeSpan.FromSeconds(35));
sink.InsertSpliceIn(eventId: 1, spliceTime: TimeSpan.FromSeconds(40));
```

- Los tiempos son tiempo de ejecución del pipeline. Pase `null` para un empalme inmediato.
- Anuncie un empalme unos segundos antes de que ocurra (SCTE 67 recomienda al menos 4 segundos). El multiplexor
  convierte el tiempo al PTS de la salida y pide al codificador un fotograma clave en el punto de empalme.
- Envíe cada empalme unos segundos antes de su propio punto, no toda la pausa de una vez. El multiplexor
  guarda una sola sección pendiente hasta su siguiente paquete: una segunda llamada antes de eso reemplaza
  la primera, y ambas devuelven `true`.
- Use un PID de 0x50 a 0x1FFE que ningún otro flujo de la salida utilice (500 en los ejemplos). Los PID
  inferiores los ocupan las tablas y los propios flujos del multiplexor; un PID fuera de ese rango se notifica
  mediante `OnError`. Un choque con otro flujo dentro del rango no se detecta.

## Insertar eventos de empalme en un flujo UDP, SRT o RIST

Los bloques sink MPEG-TS de red aceptan la misma propiedad `SCTE35PID` y los mismos métodos `InsertSpliceOut` /
`InsertSpliceIn` que `MPEGTSSinkBlock`. Úselos cuando el canal sale en directo, hacia una cabecera, un
empaquetador en la nube o un servidor de inserción publicitaria que lee SCTE-35 del flujo de transporte, en lugar
de a un archivo.

| Bloque sink | Clase de configuración con `SCTE35PID` |
|-------------|----------------------------------------|
| `UDPMPEGTSSinkBlock` | `UDPSinkSettings` |
| `MultiUDPMPEGTSSinkBlock` | `MultiUDPSinkSettings` |
| `SRTMPEGTSSinkBlock` | `SRTSinkSettings` |
| `RISTMPEGTSSinkBlock` | `RISTSinkSettings` |

```csharp
var pipeline = new MediaBlocksPipeline();

var video = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var encoder = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(video.Output, encoder.Input);

var srt = new SRTMPEGTSSinkBlock(new SRTSinkSettings
{
    Uri = "srt://ingest.example.com:9000",
    Mode = SRTConnectionMode.Caller,
    SCTE35PID = 500,
});
pipeline.Connect(encoder.Output, srt.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// Una pausa de 30 segundos a los 60 s de tiempo de ejecución, anunciada 5 segundos antes.
await Task.Delay(TimeSpan.FromSeconds(55));
srt.InsertSpliceOut(eventId: 7, spliceTime: TimeSpan.FromSeconds(60), duration: TimeSpan.FromSeconds(30));

// El regreso, anunciado cerca del final de la pausa.
await Task.Delay(TimeSpan.FromSeconds(30));
srt.InsertSpliceIn(eventId: 7, spliceTime: TimeSpan.FromSeconds(90));
```

Las reglas de la sección anterior se aplican sin cambios: puntos de empalme en tiempo de ejecución, unos segundos
de antelación y una sola sección pendiente en el multiplexor. Establezca `SCTE35PID` antes de iniciar el pipeline:
un PID establecido después nunca llega al multiplexor y los métodos de inserción devuelven `false`. Las secciones
de empalme leídas por un `MPEGTSDemuxBlock` anterior con `EnableSCTE35` también pasan por estos sinks, igual que
por `MPEGTSSinkBlock`.

Una entrada de archivo no necesita ajuste de ritmo. Cuando la entrada no es en vivo, los sinks MPEG-TS de UDP,
UDP multidestino y SRT envían cada paquete en su marca de tiempo, de modo que un archivo sale en tiempo real y el
sink no descarta nada; una conexión SRT detenida retiene el archivo hasta que se recupera. Con una fuente en vivo
envían cada paquete en cuanto llega y descartan los más antiguos si la red se detiene, para que el resto del
pipeline - una vista previa, una grabación - siga funcionando. La entrada cuenta como en vivo si
alguna de sus fuentes lo es. Una fuente push está en vivo cuando `IsLive` está activado en su configuración:
`PushVideoSourceSettings`, `PushAudioSourceSettings` y `PushJPEGSourceSettings` lo tienen en `true` por defecto,
`PushDataSourceSettings` en `false`.

`UDPSinkBlock`, `MultiUDPSinkBlock` y `SRTSinkBlock` envían sin cambios un flujo ya multiplexado e ignoran
`SCTE35PID`.

## Insertar eventos de empalme desde Video Capture SDK .NET

`VideoCaptureCoreX` construye sus salidas `SRTOutput`, `RISTOutput` y `MPEGTSOutput` sobre los mismos sinks.
Establezca el PID en la configuración `Sink` de la salida e inserte por el índice de la salida:

```csharp
var capture = new VideoCaptureCoreX(videoView);
capture.Video_Source = videoSourceSettings;

var srt = new SRTOutput("srt://ingest.example.com:9000");
srt.Video = new OpenH264EncoderSettings();
srt.Sink.SCTE35PID = 500;
capture.Outputs_Add(srt);

await capture.StartAsync();

// Índice 0: la primera salida añadida. Los tiempos son el tiempo de ejecución de la salida, el reloj que informa Outputs_GetDuration.
capture.Outputs_InsertSpliceOut(0, eventId: 7, spliceTime: TimeSpan.FromSeconds(60), duration: TimeSpan.FromSeconds(30));
// ... cerca del final de la pausa:
capture.Outputs_InsertSpliceIn(0, eventId: 7, spliceTime: TimeSpan.FromSeconds(90));
```

Ambos métodos devuelven `false` y generan `OnError` cuando la salida no se ha iniciado o se detuvo, se inició
sin PID o no es una salida MPEG-TS: una de estas tres, o un `CustomBlocksOutputSettings` cuyo `Sink` es
cualquier bloque sink MPEG-TS, incluidos `UDPMPEGTSSinkBlock` y `MultiUDPMPEGTSSinkBlock`.

## Plataformas

La lectura y la inserción, en archivos y en flujos UDP, SRT y RIST, funcionan en Windows, macOS, Linux e iOS,
allí donde el propio bloque sink está disponible (`IsAvailable()`). Android no es compatible: su compilación de
GStreamer no exporta la biblioteca de secciones MPEG-TS, por lo que `OnSpliceEvent` nunca se genera allí (el pipeline informa de un error al llegar la primera sección) y los
métodos de inserción devuelven `false` con un error en el registro.

SCTE-104 (la forma SDI/VANC de estos marcadores) no está cubierto por esta API.

## Consulte también

- [Bloques demultiplexores](../Demuxers/index.md)
- [Bloques sink](../Sinks/index.md)
- [Metadatos KLV en MPEG-TS](klv-metadata-mpegts-csharp.md)
