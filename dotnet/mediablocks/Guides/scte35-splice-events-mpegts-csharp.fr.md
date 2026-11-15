---
title: Marqueurs SCTE-35 dans les flux MPEG-TS et HLS en C# .NET
description: Lisez les marqueurs publicitaires SCTE-35 splice_insert et time_signal de MPEG-TS et HLS, gardez-les au remux et insérez-les dans une sortie TS en C#.
sidebar_label: Événements de raccord SCTE-35
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

# Marqueurs SCTE-35 dans MPEG-TS et HLS en C# .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Présentation

Les sections de raccord SCTE-35 marquent les points où une chaîne de diffusion ou FAST quitte le flux du réseau
pour une coupure publicitaire, puis y revient. Elles circulent dans le flux de transport sur leur propre PID (type
de flux 0x86). Media Blocks SDK .NET vous permet de :

1. **Les lire** depuis un fichier ou un flux MPEG-TS (`MPEGTSDemuxBlock`), ou depuis une source MPEG-TS ou HLS
   décodée par `UniversalSourceBlock`, sous forme d'événement `OnSpliceEvent`.
2. **Les conserver** lors d'un remux : un TS lu avec SCTE-35 activé et écrit par `MPEGTSSinkBlock` avec un PID
   SCTE-35 porte les mêmes commandes de raccord dans sa sortie.
3. **Insérer** vos propres commandes splice_insert dans une sortie MPEG-TS.

SCTE-35 est désactivé par défaut : sans `EnableSCTE35`, le démultiplexeur ignore le PID SCTE-35.

## Lire les événements de raccord depuis MPEG-TS

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
// Connectez demux.VideoOutput et demux.AudioOutput à des moteurs de rendu, des encodeurs ou des sinks.

await pipeline.StartAsync();
```

`SpliceEventArgs` contient :

| Propriété | Signification |
|-----------|---------------|
| `Section` | La splice_info_section complète, de table_id (0xFC) à CRC_32. Utilisez-la pour les descripteurs (segmentation, avail) et pour les commandes autres que splice_insert et time_signal. |
| `CommandType` | `Insert` (splice_insert), `TimeSignal`, `Null`, `Schedule`, `BandwidthReservation` ou `Private`. |
| `EventId`, `Cancel`, `OutOfNetwork`, `Immediate` | Les champs de splice_insert. `OutOfNetwork = true` ouvre une coupure, `false` la ferme. |
| `PtsTime` | L'instant du raccord dans l'horloge PTS à 90 kHz du flux, pts_adjustment appliqué. |
| `RunningTime` | Le même point en temps d'exécution du pipeline, la base de temps des tampons qui sortent du démultiplexeur. Là où les horodatages du flux sautent, le démultiplexeur comble l'écart, et `RunningTime` suit sa sortie, pas le PTS d'origine. `null` lorsque le démultiplexeur ne peut pas le convertir : la section arrive avant que l'horloge du flux soit connue, ou le point se situe avant le début de la sortie. |
| `BreakDuration`, `AutoReturn` | Le break_duration d'un splice_insert. |
| `Encrypted`, `PtsAdjustment` | Lorsque `Encrypted` est défini, seuls `Section` et `PtsAdjustment` sont renseignés. |

L'événement est déclenché sur un thread de streaming lorsque le démultiplexeur atteint la section, généralement
quelques secondes avant le point de raccord lui-même. Revenez vite et confiez le travail à votre propre thread ou au
répartiteur de l'interface.

## Lire les événements de raccord depuis HLS

`UniversalSourceBlock` déclenche le même événement pour une source MPEG-TS et pour un flux HLS de segments MPEG-TS.
Les sections parviennent à l'application après le passage par les décodeurs :

```csharp
var settings = await UniversalSourceSettings.CreateAsync(new Uri("https://example.com/live/index.m3u8"));
settings.EnableSCTE35 = true;

var source = new UniversalSourceBlock(settings);
source.OnSpliceEvent += (sender, e) => Console.WriteLine($"{e.CommandType} {e.EventId} at {e.RunningTime}");
```

Ce sont les sections SCTE-35 à l'intérieur des segments TS qui sont lues. Les balises de la liste de lecture
(`#EXT-X-CUE-OUT`, `#EXT-X-DATERANGE`) ne le sont pas.

## Conserver les événements de raccord lors d'un remux

Activez `EnableSCTE35` sur le démultiplexeur et `SCTE35PID` sur le sink. Chaque section voyage avec les flux, et le
multiplexeur l'écrit sur le nouveau PID avec une temporisation ajustée à la sortie :

```csharp
var demux = new MPEGTSDemuxBlock(renderVideo: true, renderAudio: true);
demux.Settings.EnableSCTE35 = true;
pipeline.Connect(source.Output, demux.Input);

var videoParse = new H264ParseBlock();
pipeline.Connect(demux.VideoOutput, videoParse.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts") { SCTE35PID = 500 });
pipeline.Connect(videoParse.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));
```

## Insérer des événements de raccord dans une sortie MPEG-TS

Avec `SCTE35PID` défini, `MPEGTSSinkBlock` écrit un flux SCTE-35 avec un battement splice_null toutes les 5
minutes, et `InsertSpliceOut` / `InsertSpliceIn` ajoutent des commandes splice_insert pendant l'exécution du
pipeline :

```csharp
var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("channel.ts") { SCTE35PID = 500 });
pipeline.Connect(h264Encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// Une coupure de 30 secondes qui commence à 10 s de temps d'exécution, annoncée à l'avance.
sink.InsertSpliceOut(eventId: 1, spliceTime: TimeSpan.FromSeconds(10), duration: TimeSpan.FromSeconds(30));
// Attendez jusqu'à environ 5 s avant la fin de la coupure, puis annoncez le retour.
await Task.Delay(TimeSpan.FromSeconds(35));
sink.InsertSpliceIn(eventId: 1, spliceTime: TimeSpan.FromSeconds(40));
```

- Les temps sont en temps d'exécution du pipeline. Passez `null` pour un raccord immédiat.
- Annoncez un raccord quelques secondes avant qu'il ait lieu (SCTE 67 recommande au moins 4 secondes). Le
  multiplexeur convertit le temps en PTS de la sortie et demande à l'encodeur une image clé au point de raccord.
- Envoyez chaque raccord quelques secondes avant son propre point, pas toute la coupure d'un coup. Le
  multiplexeur ne garde qu'une section en attente jusqu'à son paquet suivant : un second appel avant ce
  moment remplace le premier, et les deux renvoient `true`.
- Utilisez un PID qu'aucun autre flux de la sortie n'utilise.

## Plateformes

La lecture et l'insertion fonctionnent sous Windows, macOS, Linux et iOS. Android n'est pas pris en charge : sa
version de GStreamer n'exporte pas la bibliothèque de sections MPEG-TS, donc `OnSpliceEvent` n'y est jamais
déclenché (le pipeline signale une erreur à l'arrivée de la première section) et les méthodes d'insertion renvoient `false` avec une erreur dans le journal.

SCTE-104 (la forme SDI/VANC de ces marqueurs) n'est pas couvert par cette API.

## Voir aussi

- [Blocs démultiplexeurs](../Demuxers/index.md)
- [Blocs sink](../Sinks/index.md)
- [Métadonnées KLV dans MPEG-TS](klv-metadata-mpegts-csharp.md)
