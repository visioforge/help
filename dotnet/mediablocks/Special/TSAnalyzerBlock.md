---
title: MPEG-TS Stream Analyzer Block for C# .NET Applications
description: Analyze MPEG-TS programs, per-PID bitrate, PCR timing and ETSI TR 101 290 compliance in C# with Media Blocks SDK .NET TSAnalyzerBlock — live and in-process.
sidebar_label: TS Analyzer
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - MPEG-TS
  - Streaming
  - Analysis
  - TR 101 290
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# MPEG-TS Stream Analyzer Block

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

The `TSAnalyzerBlock` turns the [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) into a **broadcast-grade MPEG-TS monitor that runs inside your own .NET application** — no external tools, no spawning a CLI, no parsing text output. Point it at a file, a live UDP multicast feed, or an SRT stream and it continuously reports the program line-up, per-PID bitrate, PCR timing, scrambling, service information, and full **ETSI TR 101 290** Priority 1/2/3 compliance — as strongly-typed objects you can bind to a dashboard, log, or alert on.

It analyzes the raw transport stream packet-by-packet (PAT/PMT/PSI parsing, continuity counters, PCR clock recovery, PES timing, codec headers) and surfaces everything through a single `TSAnalyzerReport`. The block runs either as a terminal sink (`Input` mode) or as an inline passthrough (`InputOutput` mode) so you can analyze a stream **while you are still recording or relaying it**.

In short, the block measures:

- **Structure** — PAT/PMT/PSI, programs/services, PMT/PCR PIDs, stream types and codecs.
- **Bitrate** — per-PID and total bitrate (running average, instantaneous, and peak), plus null-padding and effective payload bitrate.
- **Timing** — PCR interval min/avg/max, discontinuities, jitter, repetition errors; PES PTS/DTS presence and audio/video sync offset.
- **Integrity** — continuity-counter errors, transport-error indicator, CRC-32 of PSI tables, and the full TR 101 290 P1/P2/P3 check list.
- **Service info** — SDT service name/provider/type, NIT network name, TDT/TOT stream UTC time, ISO 639 audio language, scrambling state, and (optionally) EIT/EPG events.
- **Codec details** — resolution, profile, level, chroma format, and aspect ratio for H.264, HEVC, and MPEG-2 video.

## How it works

In `Input` mode the block is the terminal of the pipeline — it consumes the transport stream and produces the report:

```mermaid
graph LR;
    Source["Source (file / UDP / SRT)"]-->TSAnalyzerBlock;
```

In `InputOutput` mode the untouched stream is forwarded to the output pad, so you can analyze and record/relay at the same time:

```mermaid
graph LR;
    Source["Source (file / UDP / SRT)"]-->TSAnalyzerBlock;
    TSAnalyzerBlock-->MPEGTSSinkBlock;
```

### Block info

Name: TSAnalyzerBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Input | MPEG-TS byte stream | 1 |
| Output | MPEG-TS byte stream (passthrough) | 1 in `InputOutput` mode, 0 in `Input` mode |

## Modes

The constructor takes a `TSAnalyzerMode`:

| Mode | Description |
| --- | --- |
| `TSAnalyzerMode.Input` | Terminal analyzer. The block accepts the stream and exposes no output pad — use it when you only need the report. |
| `TSAnalyzerMode.InputOutput` | Passthrough analyzer. The stream is forwarded unchanged to the output pad while it is analyzed, so you can record or relay it in the same pipeline. |

```csharp
TSAnalyzerBlock(TSAnalyzerMode mode = TSAnalyzerMode.Input, TSAnalyzerSettings settings = null)
```

## Quick start

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;

await VisioForgeX.InitSDKAsync();

var pipeline = new MediaBlocksPipeline();

// Live UDP multicast MPEG-TS feed (raw bytes, no demux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Terminal analyzer with a 1-second snapshot cadence
var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    TSAnalyzerReport report = e.Report;        // e.IsFinal == true on end-of-stream
    Console.WriteLine($"{report.TotalBitrateKbps:F0} kbps, programs: {report.Programs.Count}");

    if (report.Tr101290 != null && report.Tr101290.P1Count > 0)
    {
        Console.WriteLine($"TR 101 290 Priority 1 errors: {report.Tr101290.P1Count}");
    }
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... run until end-of-stream or a user stop ...
await pipeline.StopAsync();

// You can also pull a snapshot on demand at any time:
TSAnalyzerReport final = analyzer.GetReport();
```

## Feeding the analyzer (file / UDP / SRT)

The analyzer accepts a raw MPEG-TS byte stream from any source block that produces one. The three common feeds:

```csharp
// 1. Local file (.ts / .m2ts)
var fileSource = new BasicFileSourceBlock("stream.ts");
pipeline.Connect(fileSource.Output, analyzer.Input);

// 2. Live UDP (unicast or multicast) — raw MPEG-TS, no demux
var udpSource = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));
pipeline.Connect(udpSource.Output, analyzer.Input);

// 3. Live SRT
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var srtSource = new SRTRAWSourceBlock(srtSettings);
pipeline.Connect(srtSource.Output, analyzer.Input);
```

### UDPRAWMPEGTSSourceBlock

`UDPRAWMPEGTSSourceBlock` receives a live UDP unicast or multicast transport stream and exposes the **untouched** MPEG-TS byte stream on its `Output` pad — without demuxing — which is exactly what the analyzer needs. It is configured through `UDPRAWMPEGTSSourceSettings`:

| Property | Type | Default | Description |
| --- | :---: | :---: | --- |
| `Uri` | `Uri` | — | Source URI, e.g. `udp://239.0.0.1:1234` (multicast) or `udp://0.0.0.0:1234` (all interfaces). |
| `AutoJoinMulticast` | `bool` | `true` | Automatically join/leave the multicast group for multicast addresses. |
| `MulticastInterface` | `string` | `null` | Network interface(s) to join the multicast group on (e.g. `"eth0"` or `"eth0,eth1"`). |
| `BufferSize` | `int` | `524288` | Kernel receive buffer size in bytes; `0` uses the OS default. |
| `PacketSize` | `int` | `188` | Advertised TS packet size: `188` (standard) or `192` (M2TS-style with a 4-byte timecode prefix). |

```csharp
var settings = new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234")
{
    AutoJoinMulticast = true,
    PacketSize = 188
};

var source = new UDPRAWMPEGTSSourceBlock(settings);
```

> **Related block.** If you want the stream **demuxed** into separate encoded video/audio pads (for recording or remuxing without re-encoding) rather than the raw byte stream, use `UDPRAWSourceBlock` instead. See the [UDP MPEG-TS Recording guide](../Guides/udp-mpegts-record-without-reencoding.md).

## Settings

`TSAnalyzerSettings` controls the snapshot cadence and which analysis stages are enabled. Heavier stages can be turned off when you do not need them.

| Property | Type | Default | Description |
| --- | :---: | :---: | --- |
| `StatisticsInterval` | `TimeSpan` | `1 second` | How often a snapshot is produced and `OnAnalysisUpdated` is raised. |
| `TrackPCR` | `bool` | `true` | Track PCR timing (interval statistics and discontinuities). |
| `ValidateContinuity` | `bool` | `true` | Validate continuity-counter errors per PID. |
| `CalculateBitrate` | `bool` | `true` | Calculate per-PID and total bitrate. |
| `PacketSize` | `TSPacketSizeMode` | `Auto` | TS packet size handling — `Auto`, `Size188`, or `Size192`. |
| `ParseServiceInfo` | `bool` | `true` | Parse DVB service information (SDT, NIT, TDT/TOT) for service and network names. |
| `ParseEPG` | `bool` | `false` | Parse EIT for EPG events. Opt-in — it is the heaviest stage. |
| `TrackScrambling` | `bool` | `true` | Track scrambling via the `transport_scrambling_control` bits. |
| `ValidateTR101290` | `bool` | `true` | Evaluate the ETSI TR 101 290 Priority 1/2/3 checks. |
| `TrackPtsDts` | `bool` | `true` | Track PES PTS/DTS presence and audio/video sync. |
| `ParseCodecDetails` | `bool` | `true` | Parse codec details (resolution/profile/level) from SPS/sequence headers. |

`TSPacketSizeMode` values: `Auto` (detect 188 or 192 from the sync-byte spacing), `Size188` (standard MPEG-TS), `Size192` (M2TS / Blu-ray with a 4-byte timestamp prefix).

## The report model

Every snapshot is a `TSAnalyzerReport`. Top-level fields summarize the whole stream; nested collections describe programs, PIDs, and timing.

| Property | Type | Description |
| --- | :---: | --- |
| `PacketSize` | `int` | Detected TS packet size (188 or 192). |
| `TotalBitrateKbps` | `double` | Total stream bitrate (running average since analysis start). |
| `TotalPackets` | `long` | Total TS packets analyzed. |
| `HasPAT` | `bool` | Whether a PAT (PID 0) was seen. |
| `TransportErrors` | `long` | Packets with the transport-error indicator set. |
| `Programs` | `List<TSProgramInfo>` | Programs/services discovered from PAT + PMT. |
| `Pids` | `List<TSPidInfo>` | Per-PID statistics. |
| `Pcr` | `List<TSPcrStats>` | PCR timing statistics (one entry per PCR PID). |
| `NullPacketCount` | `long` | Null-padding (PID 0x1FFF) packet count. |
| `NullBitrateKbps` | `double` | Bitrate consumed by null padding. |
| `EffectiveBitrateKbps` | `double` | Effective payload bitrate (total minus null padding). |
| `SyncByteErrors` | `long` | Sync-byte errors (0x47 not at the expected position). |
| `SyncLossEvents` | `long` | Sync-loss events (parser re-sync occurrences). |
| `NetworkName` | `string` | Network name from the NIT network descriptor, or `null`. |
| `StreamTimeUtc` | `DateTime?` | UTC stream time from TDT/TOT, or `null`. |
| `AvSyncMs` | `double?` | Audio/video sync offset in ms (video PTS minus audio PTS), or `null`. |
| `Tr101290` | `TSTR101290Report` | The TR 101 290 measurement report, or `null` if validation is disabled. |
| `Events` | `List<TSEpgEvent>` | EPG events from EIT (when `ParseEPG` is enabled). |

### Programs and elementary streams

`TSProgramInfo` (per program/service):

| Property | Type | Description |
| --- | :---: | --- |
| `ProgramNumber` | `int` | Program number (service ID). |
| `PmtPid` | `int` | PID of the PMT for this program. |
| `PcrPid` | `int` | PID carrying the PCR for this program. |
| `Streams` | `List<TSElementaryStreamInfo>` | Elementary streams in this program. |
| `ServiceName` | `string` | Service name from the SDT, or `null`. |
| `ServiceProvider` | `string` | Service provider from the SDT, or `null`. |
| `ServiceType` | `int` | DVB `service_type` byte (e.g. `0x01` = digital television). |
| `IsScrambled` | `bool` | Whether the service is scrambled (SDT `free_CA_mode`). |

`TSElementaryStreamInfo` (per elementary stream):

| Property | Type | Description |
| --- | :---: | --- |
| `Pid` | `int` | Elementary stream PID. |
| `StreamType` | `byte` | MPEG-TS `stream_type` byte. |
| `Codec` | `string` | Human-readable codec/content name. |
| `Kind` | `TSPidKind` | Classified media kind (Video, Audio, Private, …). |
| `Language` | `string` | ISO 639 language code from the PMT descriptor, or `null`. |
| `CodecDetails` | `TSCodecDetails` | Parsed codec details, or `null`. |

`TSCodecDetails`: `Width`, `Height`, `FrameRateNum`/`FrameRateDen`, `Profile`, `Level`, `ChromaFormat`, `AspectRatio`. Resolution is parsed for H.264, HEVC, and MPEG-2. Profile, level, and chroma format are parsed for H.264 and HEVC only. Frame rate and aspect ratio are read from the MPEG-1/2 sequence header (not yet from the H.264/HEVC VUI).

### Per-PID statistics

`TSPidInfo`:

| Property | Type | Description |
| --- | :---: | --- |
| `Pid` | `int` | Packet identifier. |
| `Kind` | `TSPidKind` | Classified role of the PID. |
| `StreamType` | `byte` | `stream_type` byte (for elementary streams). |
| `Codec` | `string` | Human-readable codec/content name. |
| `BitrateKbps` | `double` | Average bitrate (running average). |
| `InstantBitrateKbps` | `double` | Most recent per-interval bitrate. |
| `PeakBitrateKbps` | `double` | Peak per-interval bitrate observed. |
| `PacketCount` | `long` | Total packets on this PID. |
| `ContinuityErrors` | `long` | Continuity-counter errors detected. |
| `IsPcrPid` | `bool` | Whether this PID carries the PCR. |
| `IsScrambled` | `bool` | Whether scrambled packets were observed. |
| `ScrambledPacketCount` | `long` | Packets with non-zero `transport_scrambling_control`. |
| `IsUnreferenced` | `bool` | PID not referenced by PAT/PMT/PSI and not null padding. |
| `HasPts` / `HasDts` | `bool` | Whether a PTS / DTS was seen in the PES payload. |

### PCR timing

`TSPcrStats`: `Pid`, `SampleCount`, `MinIntervalMs`, `AvgIntervalMs`, `MaxIntervalMs`, `Discontinuities`, `MaxJitterMs` (largest deviation from the running mean interval), and `RepetitionErrors` (intervals exceeding 40 ms per TR 101 290).

## ETSI TR 101 290

TR 101 290 is the DVB measurement standard for transport-stream monitoring. It groups errors into three priorities — **P1** (must be present to decode), **P2** (recommended for continuous monitoring), and **P3** (optional service/SI information). When `ValidateTR101290` is enabled, the analyzer fills `report.Tr101290`:

| Property | Type | Description |
| --- | :---: | --- |
| `Checks` | `List<TSTR101290Check>` | Individual checks across all priority groups. |
| `P1Count` / `P2Count` / `P3Count` | `long` | Total error count per priority group. |
| `TotalCount` | `long` | Sum across all groups. |
| `AllOk` | `bool` | `true` when every check passed (`TotalCount == 0`). |

Each `TSTR101290Check` has `Name` (e.g. `PAT_error`, `CRC_error`), `Priority` (1/2/3), `Count`, and `Ok`. The checks evaluated include:

- **Priority 1:** TS_sync_loss, Sync_byte_error, PAT_error, Continuity_count_error, PMT_error, PID_error.
- **Priority 2:** Transport_error, CRC_error, PCR_error, PCR_repetition_error (>40 ms), PCR_discontinuity_indicator_error, PCR_accuracy_error, PTS_error (>700 ms), CAT_error.
- **Priority 3:** NIT_error, SI_repetition_error, Unreferenced_PID, SDT_error, EIT_error, TDT_error.

```csharp
var tr = report.Tr101290;
if (tr != null && !tr.AllOk)
{
    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
}
```

> **Why this matters.** A stream can play in a media player and still be non-compliant — a PCR that repeats slower than 40 ms, a CC jump, a missing PMT. TR 101 290 is how broadcast operators quantify that. The `TSAnalyzerBlock` gives you the same structured P1/P2/P3 verdicts as a dedicated hardware analyzer, directly in managed code.

## Live updates vs on-demand snapshots

There are two ways to read the report:

- **`OnAnalysisUpdated`** — raised every `StatisticsInterval` with a fresh snapshot, and once more at end-of-stream with `IsFinal == true`. Periodic updates run on a background timer thread; the final report runs on the GStreamer thread. Marshal to the UI thread before touching UI elements.
- **`GetReport()`** — returns the current snapshot on demand (or `null` if the block has not been built). It does not perturb the periodic instant/peak bitrate accounting.

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    // e.IsFinal distinguishes the periodic updates from the end-of-stream report.
    Dispatcher.BeginInvoke(() => UpdateUI(e.Report));   // WPF: marshal to UI thread
};
```

## Sample applications

- **WPF — TS Analyzer Demo:** a full UI with a per-PID grid, a TR 101 290 panel, and a summary line (service name, null/effective bitrate, network name, stream UTC, A/V sync). [Browse the source](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/TS%20Analyzer%20Demo).
- **Console — TS Analyzer CLI:** analyzes a file or a live `udp://` / `srt://` feed and prints the full report. [Browse the source](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/TS%20Analyzer%20CLI).

## Availability

Call `TSAnalyzerBlock.IsAvailable()` to verify the analyzer is available in the current environment before creating an instance.

## Platforms

Windows, macOS, Linux, iOS, Android.

## See also

- [MPEG-TS Analysis in C#: VisioForge vs ffprobe](../Guides/mpeg-ts-analysis-vs-ffprobe.md) — how the block compares to ffprobe and professional broadcast analyzers.
- [Analyze and validate an MPEG-TS stream in C#](../Guides/mpeg-ts-stream-validation-csharp.md) — a step-by-step task guide.
- [UDP MPEG-TS Recording without re-encoding](../Guides/udp-mpegts-record-without-reencoding.md).
