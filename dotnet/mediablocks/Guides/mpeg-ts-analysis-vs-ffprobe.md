---
title: "MPEG-TS Analysis in C# .NET: VisioForge vs ffprobe"
description: In-process .NET MPEG-TS analysis with live ETSI TR 101 290 monitoring — how the Media Blocks SDK TS Analyzer compares to ffprobe and broadcast analyzers.
sidebar_label: TS Analysis vs ffprobe
tags:
  - Media Blocks SDK
  - .NET
  - MPEG-TS
  - Analysis
  - TR 101 290
  - ffprobe
  - Streaming
  - C#
primary_api_classes:
  - TSAnalyzerBlock
  - TSAnalyzerReport
  - TSAnalyzerSettings

---

# MPEG-TS Analysis in C#: VisioForge vs ffprobe

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Why this matters

ffprobe is a superb command-line tool for a quick look at a media file. But the moment you need transport-stream analysis **inside a .NET application** — a live monitoring dashboard, an ingest validator, an automated QC gate — the CLI model starts working against you: you have to spawn a separate process, capture its text or JSON output, parse it, and you still only get a one-shot snapshot with no compliance framework.

The [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net) `TSAnalyzerBlock` is built for exactly that job. It is an **in-process, live, broadcast-grade MPEG-TS monitor** that reports strongly-typed results continuously, with full ETSI **TR 101 290** Priority 1/2/3 compliance — something ffprobe simply does not provide.

## What sets the TS Analyzer apart

### 1. In-process, strongly-typed — no CLI, no text parsing

You connect a source block to the analyzer and read a `TSAnalyzerReport` object. No `Process.Start`, no stdout capture, no brittle string/JSON parsing, no version-dependent output format to track. Every metric is a typed property you can bind, log, or assert on directly.

### 2. Live and event-driven

ffprobe is one-shot: it runs, prints, exits. The `TSAnalyzerBlock` raises `OnAnalysisUpdated` on a configurable cadence (every second by default) for the entire life of the stream — perfect for real-time dashboards, drift detection, and alerting. A final report is delivered at end-of-stream.

### 3. Broadcast-grade TR 101 290 — built in

The analyzer evaluates the full TR 101 290 Priority 1/2/3 check list (sync loss, PAT/PMT/PID errors, continuity-count errors, transport errors, CRC errors, PCR/PTS errors, SDT/EIT/TDT/NIT errors) and returns each check with its priority, error count, and pass/fail status. ffprobe has no equivalent compliance framework — you would have to build one yourself on top of its output.

### 4. Passthrough mode — analyze while you record or relay

In `InputOutput` mode the original transport stream is forwarded unchanged to the next block, so you can validate a feed **at the same time** as you record or restream it — zero re-mux, one pipeline. ffprobe sits outside your media path entirely.

### 5. One API, any source, every platform

File, UDP (unicast and multicast), and SRT all feed the same analyzer through the same API, on Windows, macOS, Linux, iOS, and Android.

## Feature comparison

The matrix below reflects the **current** capabilities of the `TSAnalyzerBlock`. ✅ = supported, ⚠️ = partial/indirect, ❌ = not available.

| Capability | VisioForge TS Analyzer | ffprobe |
| --- | :---: | :---: |
| Packet size 188/192 auto-detect | ✅ | ✅ |
| PAT/PMT, programs, PCR PID | ✅ | ✅ |
| stream_type → codec | ✅ | ✅ |
| Per-PID bitrate + packet count | ✅ | ⚠️ |
| Per-PID instantaneous + peak bitrate | ✅ | ❌ |
| Continuity-counter errors | ✅ | ⚠️ |
| PCR interval min/avg/max + discontinuities | ✅ | ❌ |
| PCR jitter (max) + repetition >40 ms | ✅ | ❌ |
| PCR accuracy / drift (ppm) | ⚠️ | ❌ |
| Transport-error indicator | ✅ | ⚠️ |
| Null % / stuffing / effective bitrate | ✅ | ❌ |
| Scrambling (TSC + free_CA_mode) | ✅ | ⚠️ |
| PTS/DTS presence + A/V sync offset | ✅ | ✅ |
| SDT → service name / provider / type | ✅ | ⚠️ |
| Audio language (ISO 639, PMT) | ✅ | ✅ |
| NIT network name; TDT/TOT stream UTC | ✅ | ⚠️ |
| EIT → EPG events | ✅ (opt-in) | ⚠️ |
| Unreferenced PID detection | ✅ | ❌ |
| Codec resolution / profile / level / chroma / aspect | ✅ (H.264/HEVC/MPEG-2) | ✅ |
| CRC-32 PSI validation | ✅ | ⚠️ |
| **Structured TR 101 290 P1/P2/P3** | ✅ | ❌ |
| **In-process .NET API (no CLI / parsing)** | ✅ | ❌ |
| **Live continuous monitoring (events)** | ✅ | ❌ |
| **Passthrough (analyze while recording/relaying)** | ✅ | ❌ |
| Cross-platform (Windows/macOS/Linux/mobile) | ✅ | ✅ |

A few honest notes so the table stays trustworthy. Codec coverage is per-attribute, not uniform: **resolution** is parsed for H.264, HEVC, and MPEG-2; **profile, level, and chroma** for H.264/HEVC only; **aspect ratio and frame rate** from the MPEG-1/2 sequence header (not yet from the H.264/HEVC VUI). The PCR clock **drift in ppm** field is reserved and not yet computed (jitter and repetition are). ffprobe also remains excellent at exhaustive per-codec metadata and supports a far wider range of container/codec formats beyond MPEG-TS.

## When to use which

- **Reach for ffprobe** when you want a quick terminal check of an arbitrary media file, or you are scripting a one-off in a shell and the format may not be MPEG-TS.
- **Reach for the `TSAnalyzerBlock`** when transport-stream analysis needs to live **inside your .NET app** — continuous monitoring, TR 101 290 compliance gating, live dashboards/alerts, or analyzing a feed while you simultaneously record or relay it.

They are not mutually exclusive: many teams use ffprobe at the desk and ship VisioForge in the product.

## Next steps

- [MPEG-TS Stream Analyzer Block](../Special/TSAnalyzerBlock.md) — the full block reference: modes, settings, and the complete report model.
- [Analyze and validate an MPEG-TS stream in C#](mpeg-ts-stream-validation-csharp.md) — a step-by-step task guide.
