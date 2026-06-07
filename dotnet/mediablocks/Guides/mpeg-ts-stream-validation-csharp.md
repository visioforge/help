---
title: Analyze and Validate MPEG-TS Transport Streams in C#
description: Step-by-step C# guide to analyze a file or live UDP/SRT MPEG-TS stream, read ETSI TR 101 290 results, and alert on errors with the Media Blocks SDK .NET.
sidebar_label: MPEG-TS Stream Validation
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - UDP
  - SRT
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerSettings
  - TSAnalyzerReport
  - UDPRAWMPEGTSSourceBlock

---

# Analyze and Validate MPEG-TS Streams in C# .NET

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This guide shows how to analyze and validate an MPEG-TS transport stream from C# using the [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) `TSAnalyzerBlock`. You will:

1. Analyze a local `.ts` / `.m2ts` file and read the final report.
2. Monitor a **live** `udp://` or `srt://` feed with periodic updates.
3. Read **ETSI TR 101 290** results and raise an alert when the stream is non-compliant.
4. Analyze a feed **while you record it** using passthrough mode.

For the full API reference (every setting and report field), see the [MPEG-TS Stream Analyzer Block](../Special/TSAnalyzerBlock.md) page.

## Prerequisites

Install the Media Blocks SDK NuGet package and the platform runtime package:

- `VisioForge.DotNet.MediaBlocks`
- A platform runtime, e.g. `VisioForge.CrossPlatform.Core.Windows.x64` on Windows.

Initialize the SDK once at startup:

```csharp
using VisioForge.Core;

await VisioForgeX.InitSDKAsync();
```

The namespaces used throughout this guide:

```csharp
using System;
using System.Linq;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.X;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Special;
```

## 1. Analyze a file

For a file, run the pipeline to end-of-stream and read the final report. Connect the analyzer in `Input` mode (terminal):

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new BasicFileSourceBlock("stream.ts");
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.Input);

// Signal completion when the pipeline stops on its own (end-of-stream).
var completed = new TaskCompletionSource<bool>();
pipeline.OnStop += (s, e) => completed.TrySetResult(true);

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
await completed.Task;             // wait for end-of-stream
await pipeline.StopAsync();

TSAnalyzerReport report = analyzer.GetReport();

Console.WriteLine($"Packet size: {report.PacketSize}  |  total: {report.TotalBitrateKbps:F0} kbps");
Console.WriteLine($"Programs: {report.Programs.Count}  |  PAT: {(report.HasPAT ? "yes" : "no")}");

foreach (var program in report.Programs)
{
    var name = string.IsNullOrEmpty(program.ServiceName) ? "(no name)" : program.ServiceName;
    Console.WriteLine($"Program {program.ProgramNumber} \"{name}\" (PMT {program.PmtPid}, PCR {program.PcrPid})");

    foreach (var es in program.Streams)
    {
        var lang = string.IsNullOrEmpty(es.Language) ? "" : $" [{es.Language}]";
        Console.WriteLine($"    PID {es.Pid}  {es.Kind}  {es.Codec}{lang}");
    }
}
```

## 2. Monitor a live stream

For a live `udp://` or `srt://` feed, subscribe to `OnAnalysisUpdated` and read each periodic snapshot. Set `StatisticsInterval` to control the cadence.

```csharp
var pipeline = new MediaBlocksPipeline();

// Live UDP multicast (raw MPEG-TS, no demux)
var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

var analyzer = new TSAnalyzerBlock(
    TSAnalyzerMode.Input,
    new TSAnalyzerSettings { StatisticsInterval = TimeSpan.FromSeconds(1) });

analyzer.OnAnalysisUpdated += (sender, e) =>
{
    if (e.IsFinal)
    {
        return;     // end-of-stream report; periodic snapshots have IsFinal == false
    }

    TSAnalyzerReport report = e.Report;
    Console.WriteLine(
        $"{report.TotalBitrateKbps:F0} kbps total, " +
        $"{report.EffectiveBitrateKbps:F0} kbps effective, " +
        $"programs: {report.Programs.Count}");
};

pipeline.Connect(source.Output, analyzer.Input);

await pipeline.StartAsync();
// ... keep running while you monitor ...
```

To feed SRT instead of UDP, swap the source:

```csharp
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri("srt://server:9000"), ignoreMediaInfoReader: true);
var source = new SRTRAWSourceBlock(srtSettings);
```

> **Threading.** Periodic `OnAnalysisUpdated` callbacks run on a background timer thread. In a UI app, marshal to the UI thread before updating controls — e.g. `Dispatcher.BeginInvoke(() => UpdateUI(e.Report));` in WPF.

## 3. Read TR 101 290 and alert on errors

The TR 101 290 report groups errors by priority. A common pattern is to alert whenever a Priority 1 (critical) error appears, and to log the individual failing checks:

```csharp
analyzer.OnAnalysisUpdated += (sender, e) =>
{
    var tr = e.Report.Tr101290;
    if (tr == null || tr.AllOk)
    {
        return;     // TR 101 290 disabled, or the stream is fully compliant
    }

    if (tr.P1Count > 0)
    {
        RaiseAlert($"Critical TR 101 290 (P1) errors: {tr.P1Count}");
    }

    foreach (var check in tr.Checks.Where(c => c.Count > 0))
    {
        Console.WriteLine($"P{check.Priority} {check.Name}: {check.Count}");
    }
};
```

`tr.P1Count` / `P2Count` / `P3Count` give you the per-priority totals; `tr.Checks` lets you drill into a specific failing measurement (for example `PCR_repetition_error` or `Continuity_count_error`). Use `tr.AllOk` as a single-line compliance gate.

## 4. Analyze while recording (passthrough)

To validate a feed and record it in the same pipeline, create the analyzer in `InputOutput` mode and connect its `Output` to a sink. The transport stream is forwarded unchanged, so the recording is a bit-exact copy:

```csharp
var pipeline = new MediaBlocksPipeline();

var source = new UDPRAWMPEGTSSourceBlock(new UDPRAWMPEGTSSourceSettings("udp://239.0.0.1:1234"));

// Passthrough analyzer: forwards the stream while analyzing it
var analyzer = new TSAnalyzerBlock(TSAnalyzerMode.InputOutput);
analyzer.OnAnalysisUpdated += (s, e) => { /* monitor while recording */ };

var sink = new MPEGTSSinkBlock(new MPEGTSSinkSettings("recording.ts"));

pipeline.Connect(source.Output, analyzer.Input);
pipeline.Connect(analyzer.Output, sink.Input);

await pipeline.StartAsync();
```

## Troubleshooting

- **`HasPAT` is false / no programs.** The input is probably not a valid MPEG-TS byte stream, or the packet size was misdetected. Force it via `TSAnalyzerSettings.PacketSize` (`Size188` or `Size192`), or check the source URI.
- **Many `PCR_repetition_error` entries on a valid file.** This is expected when the muxer emits PCR slower than the 40 ms TR 101 290 limit (common for file-oriented muxing). The stream still plays; it is simply not 40 ms-PCR compliant.
- **`GetReport()` returns null.** The block has not been built yet — call it after `StartAsync()`.
- **No multicast packets received.** Verify `AutoJoinMulticast` is `true` (the default) and set `MulticastInterface` if the host has multiple network interfaces.
- **High CPU on a heavy stream.** Disable stages you do not need via `TSAnalyzerSettings` — `ParseEPG` is off by default; you can also turn off `ParseCodecDetails` or `TrackPtsDts`.

## Frequently asked questions

### How do I analyze a live UDP multicast MPEG-TS stream in C#?

Create a `UDPRAWMPEGTSSourceBlock` with a `udp://group:port` URI, connect its `Output` to a `TSAnalyzerBlock` in `Input` mode, and subscribe to `OnAnalysisUpdated` for periodic snapshots. See [section 2](#2-monitor-a-live-stream).

### Can I check ETSI TR 101 290 compliance from .NET?

Yes. With `ValidateTR101290` enabled (the default), `report.Tr101290` exposes the Priority 1/2/3 check list with per-check counts and pass/fail status, plus `P1Count`/`P2Count`/`P3Count` totals and an `AllOk` flag. See [section 3](#3-read-tr-101-290-and-alert-on-errors).

### Can I record a stream and analyze it at the same time?

Yes. Use `TSAnalyzerMode.InputOutput` and connect the analyzer's `Output` to a sink such as `MPEGTSSinkBlock`. The stream is forwarded unchanged. See [section 4](#4-analyze-while-recording-passthrough).

## See also

- [MPEG-TS Stream Analyzer Block](../Special/TSAnalyzerBlock.md) — the complete block reference.
- [MPEG-TS Analysis in C#: VisioForge vs ffprobe](mpeg-ts-analysis-vs-ffprobe.md) — positioning and feature comparison.
