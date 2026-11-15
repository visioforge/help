---
title: SCTE-35 Ad Markers in MPEG-TS and HLS with C# .NET
description: Read SCTE-35 ad markers from MPEG-TS and HLS, keep them through a remux, and insert them into TS files and UDP, SRT or RIST streams in C#.
sidebar_label: SCTE-35 Splice Events
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

# SCTE-35 Ad Markers in MPEG-TS and HLS with C# .NET

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

SCTE-35 splice sections mark the points where a broadcast or FAST channel leaves the network feed for an ad
break and comes back. They travel in the transport stream on a PID of their own (stream type 0x86). The
Media Blocks SDK .NET lets you:

1. **Read** them from an MPEG-TS file or stream (`MPEGTSDemuxBlock`), or from an MPEG-TS or HLS source
   decoded by `UniversalSourceBlock`, as an `OnSpliceEvent` event.
2. **Keep** them through a remux: a TS read with SCTE-35 enabled and written by `MPEGTSSinkBlock` with a
   SCTE-35 PID carries the same splice commands in its output.
3. **Insert** your own splice_insert commands into an MPEG-TS file, or into an MPEG-TS stream sent over
   UDP, SRT or RIST - from a Media Blocks pipeline or from `VideoCaptureCoreX`.

SCTE-35 is off by default: without `EnableSCTE35` the demuxer drops the SCTE-35 PID.

## Read splice events from MPEG-TS

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
// Connect demux.VideoOutput and demux.AudioOutput to renderers, encoders or sinks.

await pipeline.StartAsync();
```

`SpliceEventArgs` carries:

| Property | Meaning |
|----------|---------|
| `Section` | The whole splice_info_section, from table_id (0xFC) to CRC_32. Use it for descriptors (segmentation, avail) and for commands other than splice_insert and time_signal. |
| `CommandType` | `Insert` (splice_insert), `TimeSignal`, `Null`, `Schedule`, `BandwidthReservation` or `Private`. |
| `EventId`, `Cancel`, `OutOfNetwork`, `Immediate` | The splice_insert fields. `OutOfNetwork = true` starts a break, `false` ends it. |
| `PtsTime` | The splice time in the stream's 90 kHz PTS clock, pts_adjustment applied. |
| `RunningTime` | The same point as pipeline running time - the time base of the buffers coming out of the demuxer. Where the stream's timestamps jump, the demuxer closes the gap, and `RunningTime` follows its output, not the raw PTS. `null` when the demuxer cannot map it: the section arrives before the stream's clock is known, or the point lies before the start of the output. |
| `BreakDuration`, `AutoReturn` | The break_duration of a splice_insert. |
| `Encrypted`, `PtsAdjustment` | When `Encrypted` is set, only `Section` and `PtsAdjustment` are filled in. |

The event is raised on a streaming thread as the demuxer reaches the section, usually a few seconds before
the splice point itself. Return quickly; hand the work to your own thread or the UI dispatcher.

## Read splice events from HLS

`UniversalSourceBlock` raises the same event for an MPEG-TS source and for an HLS stream of MPEG-TS
segments. The sections reach the application after passing the decoders:

```csharp
var settings = await UniversalSourceSettings.CreateAsync(new Uri("https://example.com/live/index.m3u8"));
settings.EnableSCTE35 = true;

var source = new UniversalSourceBlock(settings);
source.OnSpliceEvent += (sender, e) => Console.WriteLine($"{e.CommandType} {e.EventId} at {e.RunningTime}");
```

The SCTE-35 sections inside the TS segments are what is read. Playlist cue tags (`#EXT-X-CUE-OUT`,
`#EXT-X-DATERANGE`) are not.

## Keep splice events through a remux

Set `EnableSCTE35` on the demuxer and `SCTE35PID` on the sink. Each section travels with the streams, and
the muxer writes it on the new PID with its timing adjusted to the output:

```csharp
var demux = new MPEGTSDemuxBlock(renderVideo: true, renderAudio: true);
demux.Settings.EnableSCTE35 = true;
pipeline.Connect(source.Output, demux.Input);

var videoParse = new H264ParseBlock();
pipeline.Connect(demux.VideoOutput, videoParse.Input);

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts") { SCTE35PID = 500 });
pipeline.Connect(videoParse.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));
```

## Insert splice events into MPEG-TS output

With `SCTE35PID` set, `MPEGTSSinkBlock` writes a SCTE-35 stream with a splice_null heartbeat every 5
minutes, and `InsertSpliceOut` / `InsertSpliceIn` add splice_insert commands while the pipeline runs:

```csharp
var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("channel.ts") { SCTE35PID = 500 });
pipeline.Connect(h264Encoder.Output, sink.CreateNewInput(MediaBlockPadMediaType.Video));

await pipeline.StartAsync();

// A 30-second break starting at 10 s of pipeline running time, announced ahead of time.
sink.InsertSpliceOut(eventId: 1, spliceTime: TimeSpan.FromSeconds(10), duration: TimeSpan.FromSeconds(30));
// Wait until about 5 s before the break ends, then announce the return.
await Task.Delay(TimeSpan.FromSeconds(35));
sink.InsertSpliceIn(eventId: 1, spliceTime: TimeSpan.FromSeconds(40));
```

- Times are pipeline running time. Pass `null` for an immediate splice.
- Announce a splice a few seconds before it happens (SCTE 67 recommends at least 4 seconds). The muxer
  converts the time to the output's PTS and asks the encoder for a key frame at the splice point.
- Send each splice a few seconds before its own point, not the whole break at once. The muxer keeps one
  pending section until its next packet: a second call before then replaces the first, and both return
  `true`.
- Use a PID from 0x50 to 0x1FFE that no other stream in the output uses (500 in the samples). Lower PIDs
  are taken by the tables and by the muxer's own streams; a PID outside that range is reported through
  `OnError`. A clash with another stream inside the range is not detected.

## Insert splice events into a UDP, SRT or RIST stream

The network MPEG-TS sinks take the same `SCTE35PID` setting and the same `InsertSpliceOut` /
`InsertSpliceIn` methods as `MPEGTSSinkBlock`. Use them when the channel goes out live - to a headend,
a cloud packager or an ad-insertion server that reads SCTE-35 from the transport stream - rather than
into a file.

| Sink block | Settings class carrying `SCTE35PID` |
|------------|-------------------------------------|
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

// A 30-second break at 60 s of running time, announced 5 seconds ahead.
await Task.Delay(TimeSpan.FromSeconds(55));
srt.InsertSpliceOut(eventId: 7, spliceTime: TimeSpan.FromSeconds(60), duration: TimeSpan.FromSeconds(30));

// The return, announced near the end of the break.
await Task.Delay(TimeSpan.FromSeconds(30));
srt.InsertSpliceIn(eventId: 7, spliceTime: TimeSpan.FromSeconds(90));
```

The rules of the previous section apply unchanged: running-time splice points, a few seconds of notice,
and one pending section in the muxer. Set `SCTE35PID` before the pipeline starts - a PID set later never
reaches the muxer, and the insert methods return `false`. Splices read by an upstream `MPEGTSDemuxBlock`
with `EnableSCTE35` pass through these sinks too, as they do through `MPEGTSSinkBlock`.

A file input needs no pacing. When the input is not live, the UDP, multi-destination UDP and SRT MPEG-TS
sinks send each packet at its timestamp, so a file goes out in real time and the sink drops nothing; a stalled
SRT connection holds the file back until it recovers. With a live source they send each packet as it arrives,
and drop the oldest ones if the network stalls, so the rest of the pipeline - a preview, a recording - keeps
running. The input counts as live when any of its sources is live. A push source is live when `IsLive` is set on
its settings: `PushVideoSourceSettings`, `PushAudioSourceSettings` and `PushJPEGSourceSettings` default to `true`,
`PushDataSourceSettings` to `false`.

`UDPSinkBlock`, `MultiUDPSinkBlock` and `SRTSinkBlock` send an already-muxed stream unchanged and ignore
`SCTE35PID`.

## Insert splice events from Video Capture SDK .NET

`VideoCaptureCoreX` builds its `SRTOutput`, `RISTOutput` and `MPEGTSOutput` on the same sinks. Set the
PID on the output's `Sink` settings, and insert by the output's index:

```csharp
var capture = new VideoCaptureCoreX(videoView);
capture.Video_Source = videoSourceSettings;

var srt = new SRTOutput("srt://ingest.example.com:9000");
srt.Video = new OpenH264EncoderSettings();
srt.Sink.SCTE35PID = 500;
capture.Outputs_Add(srt);

await capture.StartAsync();

// Index 0: the first output added. Times are the output's running time - the clock Outputs_GetDuration reports.
capture.Outputs_InsertSpliceOut(0, eventId: 7, spliceTime: TimeSpan.FromSeconds(60), duration: TimeSpan.FromSeconds(30));
// ... near the end of the break:
capture.Outputs_InsertSpliceIn(0, eventId: 7, spliceTime: TimeSpan.FromSeconds(90));
```

Both methods return `false` and raise `OnError` when the output has not started or was stopped, was started
without a PID, or is not an MPEG-TS output - one of these three, or a `CustomBlocksOutputSettings` whose `Sink`
is any MPEG-TS sink block, `UDPMPEGTSSinkBlock` and `MultiUDPMPEGTSSinkBlock` included.

## Platforms

Reading and inserting - into files and into UDP, SRT and RIST streams - work on Windows, macOS, Linux and
iOS, wherever the sink block itself is available (`IsAvailable()`). Android is not supported: its GStreamer
build does not export the MPEG-TS section library, so `OnSpliceEvent` never fires there (the pipeline reports one error when the first section arrives) and the insert
methods return `false` with an error in the log.

SCTE-104 (the SDI/VANC form of these markers) is not covered by this API.

## See also

- [Demuxer blocks](../Demuxers/index.md)
- [Sink blocks](../Sinks/index.md)
- [KLV metadata in MPEG-TS](klv-metadata-mpegts-csharp.md)
