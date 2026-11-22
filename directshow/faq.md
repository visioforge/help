---
title: DirectShow Filters FAQ — MP4 Muxers, MF Mux and HEVC
description: Answers to common questions about VisioForge DirectShow filters — MP4 muxer crashes in DES graphs and the H.265 encoders MF Mux can use.
sidebar_label: FAQ
tags:
  - DirectShow
  - C++
  - .NET
  - Windows
  - Encoding
  - MP4
  - H.264
  - HEVC
  - Troubleshooting

---

# DirectShow Filters FAQ

Answers to questions developers ask when they use the VisioForge DirectShow filters directly in their own graphs. For the .NET SDKs built on top of these filters, see [Troubleshooting](../dotnet/general/troubleshooting.md).

## Encoding and muxing

### The MP4 muxer crashes, or the graph fails with "Attempted to divide by zero"

Both the MP4 v10 muxer and the MF Mux filter take the frame size and the frame rate from the media type of the video connection. A media type without them — typical for graphs built by DirectShow Editing Services (DES) or by custom source filters — can make the encoder fail to start, throw a divide-by-zero error in `IMediaControl.Run()`, or fail intermittently inside the muxer.

Set them explicitly on the video media type that you connect to the encoder or muxer, or on the DES group:

- `biWidth` and `biHeight` in the `BITMAPINFOHEADER`;
- `AvgTimePerFrame` in the `VIDEOINFOHEADER` (in 100-ns units, for example `400000` for 25 fps).

A complete C# graph with the MP4 v10 muxer is in the [MP4 v10 Output Demo](https://github.com/visioforge/directshow-samples/tree/main/Encoding%20Filters%20Pack/dotnet/MP4%20v10%20Output%20Demo). If the problem continues with correct media types, compare with the demo and send a small graph that reproduces it to support.

### Which H.265 encoders can MF Mux use?

MF Mux has no HEVC encoder of its own. It encodes H.265 with the Media Foundation encoders installed on the PC:

- **GPU encoders** — Intel Quick Sync, NVIDIA NVENC and AMD AMF support HEVC on most GPUs released in the last several years.
- **The Microsoft H.265 encoder** — installed with the *HEVC Video Extensions* package from the Microsoft Store, which is not available on Windows Server.

When neither can encode on a PC, the encoder fails to start for HEVC, and the debug output shows a message such as `Unable to init NVENC H265 encoder.` (or its Intel QSV and AMD form), or `MS HEVC encoder not found.` for the Microsoft encoder; encode H.264 there instead. In MF Mux the Microsoft H.264 and H.265 encoders accept video up to 1920 pixels wide and 1080 pixels high; use a GPU encoder (NVENC, Quick Sync, AMF) for larger video. H.264 is available on every Windows edition with the media components, in hardware or in software; Windows N and KN editions need the Media Feature Pack first, and Windows Server needs the Media Foundation feature. H.265 support in MF Mux requires a current version of the filter: old builds (version 11 from 2019, for example) do not have it. The current filters are part of the [Encoding Filters Pack](filters-enc/index.md).
