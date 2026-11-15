---
title: Marcadores SCTE-35 en flujos MPEG-TS y HLS con C# .NET
description: Lea marcadores publicitarios SCTE-35 splice_insert y time_signal de MPEG-TS y HLS, consérvelos en un remux e insértelos en una salida TS con C#.
sidebar_label: Eventos de empalme SCTE-35
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - HLS
  - SCTE-35
  - Ad insertion
  - C#
primary_api_classes:
  - MPEGTSDemuxBlock
  - UniversalSourceBlock
  - MPEGTSSinkBlock
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
3. **Insertar** sus propios comandos splice_insert en una salida MPEG-TS.

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
- Use un PID que ningún otro flujo de la salida utilice.

## Plataformas

La lectura y la inserción funcionan en Windows, macOS, Linux e iOS. Android no es compatible: su compilación de
GStreamer no exporta la biblioteca de secciones MPEG-TS, por lo que `OnSpliceEvent` nunca se genera allí (el pipeline informa de un error al llegar la primera sección) y los
métodos de inserción devuelven `false` con un error en el registro.

SCTE-104 (la forma SDI/VANC de estos marcadores) no está cubierto por esta API.

## Consulte también

- [Bloques demultiplexores](../Demuxers/index.md)
- [Bloques sink](../Sinks/index.md)
- [Metadatos KLV en MPEG-TS](klv-metadata-mpegts-csharp.md)
