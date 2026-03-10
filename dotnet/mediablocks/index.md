---
title: Modular Multimedia Processing Pipeline API for C# .NET
description: Build custom multimedia pipelines in C# with VisioForge Media Blocks SDK. Connect source, processing, and output blocks for transcoding, capture, and streaming.
sidebar_label: Media Blocks SDK .NET
order: 14

---

# Media Blocks SDK for C# .NET — Modular Multimedia Pipeline API

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Media Blocks SDK for .NET is a pipeline-based multimedia framework that lets you build custom video and audio processing workflows in C#. Instead of using a fixed API with predefined behavior, you create a pipeline by connecting typed blocks — sources, encoders, effects, renderers, and sinks — to construct exactly the processing chain your application needs.

The SDK runs on Windows, macOS, Linux, Android, and iOS. It powers use cases that the higher-level Video Capture SDK and Media Player SDK cannot cover: multi-source compositing, custom transcoding pipelines, simultaneous encoding to multiple formats, real-time video effects chains, and integration with hardware like Blackmagic Decklink or industrial cameras.

## When to Use Media Blocks

Media Blocks SDK is the right choice when you need full control over the multimedia pipeline. Use it instead of (or alongside) the other VisioForge SDKs when:

| Scenario | Recommended SDK |
| -------- | --------------- |
| Simple webcam recording to MP4 | [Video Capture SDK](../videocapture/index.md) |
| Play a video file with standard controls | [Media Player SDK](../mediaplayer/index.md) |
| Custom pipeline: source → effects → encode → multiple outputs | **Media Blocks SDK** |
| Live video compositing from multiple sources | **Media Blocks SDK** |
| Transcoding / format conversion with custom processing | **Media Blocks SDK** |
| RTSP recording with post-processing (overlay, resize, re-encode) | **Media Blocks SDK** |
| Cross-platform Avalonia or MAUI media app | **Media Blocks SDK** |
| Integration with Decklink, GenICam, or NVIDIA hardware | **Media Blocks SDK** |

## Common Use Cases

### Video File Transcoding and Format Conversion

Convert video files between formats (e.g., AVI to MP4, MKV to WebM) with full control over codecs, resolution, bitrate, and processing. Chain resize, deinterlace, or color correction blocks between the source and encoder.

### Custom Camera Capture with Processing Pipeline

Build camera capture workflows that go beyond simple recording. Insert real-time effects, text overlays, or computer vision blocks between the camera source and the file sink. Output to multiple destinations simultaneously — preview window, file, and network stream.

See: [Camera Viewer Application Tutorial](GettingStarted/camera.md)

### Live Video Compositing and Mixing

Combine multiple video sources into a single output with the [Live Video Compositor](LiveVideoCompositor/index.md). Position, scale, and layer video feeds for multi-camera production, picture-in-picture, or surveillance grid layouts.

### RTSP Stream Recording with Post-Processing

Capture RTSP streams from IP cameras and apply processing before saving — resize to lower resolution, add timestamp overlays, re-encode with different quality settings, or split into segments.

See: [RTSP Stream Save Guide](Guides/rtsp-save-original-stream.md) | [ONVIF Capture with Post-Processing](Guides/onvif-capture-with-postprocessing.md)

### Text and Image Overlay / Watermarking

Add text, images, or SVG overlays to live video or recorded files using the [Overlay Manager Block](VideoProcessing/OverlayManagerBlock.md). Useful for watermarking, timestamp insertion, branding, and on-screen display.

### Barcode and QR Code Reading from Video

Process live camera feeds or video files to detect and decode barcodes and QR codes in real-time.

See: [Barcode & QR Code Reader Guide](Guides/barcode-qr-reader-guide.md)

### Pre-Event Recording

Implement circular buffer recording that captures video continuously and writes event clips (including footage from before the trigger) to disk.

See: [Pre-Event Recording Guide](Guides/pre-event-recording.md)

## Platform Support

| Platform | UI Frameworks | Notes |
| -------- | ------------- | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | Full feature set including DirectShow bridges |
| macOS | MAUI, Avalonia, Console | AVFoundation camera access |
| Linux x64 | Avalonia, Console | V4L2 camera, GStreamer-based processing |
| Android | MAUI | Via MAUI integration |
| iOS | MAUI | Via MAUI integration |

## Core SDK Components

### Sources

[Source Blocks](Sources/index.md) ingest media from cameras, files, network streams, virtual generators, and capture hardware.

### Video Processing

- [Video Encoders](VideoEncoders/index.md) — H.264, H.265/HEVC, VP8, VP9, AV1 with GPU acceleration (NVENC, AMF, Quick Sync)
- [Video Decoders](VideoDecoders/index.md) — Hardware and software decoding for all major codecs
- [Video Processing](VideoProcessing/index.md) — Resize, crop, rotate, color correction, deinterlace, effects
- [Video Rendering](VideoRendering/index.md) — Display video in WinForms, WPF, MAUI, and Avalonia controls
- [Live Video Compositor](LiveVideoCompositor/index.md) — Multi-source mixing and compositing

### Audio Processing

- [Audio Encoders](AudioEncoders/index.md) — AAC, MP3, Vorbis, Opus, FLAC encoding
- [Audio Processing](AudioProcessing/index.md) — Filters, effects, sample rate conversion, channel mixing
- [Audio Rendering](AudioRendering/index.md) — Playback to system audio devices
- [Audio Visualizers](AudioVisualizers/index.md) — Waveform and spectrum visualization blocks

### Output and Connectivity

- [Sinks](Sinks/index.md) — Write to MP4, WebM, AVI, MKV, TS files and network streams
- [Output Blocks](Outputs/index.md) — High-level output configurations
- [Bridges](Bridge/index.md) — Connect pipeline segments and synchronize blocks
- [Demuxers](Demuxers/index.md) and [Parsers](Parsers/index.md) — Stream demultiplexing and parsing

### Hardware and Platform-Specific

- [NVIDIA](Nvidia/index.md) — NVENC/NVDEC hardware acceleration blocks
- [Blackmagic Decklink](Decklink/index.md) — Professional capture and playback hardware
- [OpenCV](OpenCV/index.md) — Computer vision integration
- [OpenGL](OpenGL/index.md) — GPU-based video processing
- [AWS](AWS/index.md) — Cloud integration blocks
- [RTSP Server](RTSPServer/index.md) — Serve video as an RTSP stream

## Getting Started

Ready to build your first pipeline? The Developer Quick Start Guide covers installation, core concepts, pipeline architecture, and step-by-step code examples:

[Developer Quick Start Guide](GettingStarted/index.md)

Additional getting started tutorials:

- [Complete Pipeline Implementation](GettingStarted/pipeline.md)
- [Media Player Development](GettingStarted/player.md)
- [Camera Viewer Application](GettingStarted/camera.md)
- [Device Enumeration](GettingStarted/device-enum.md)

## Guides

- [Save RTSP Stream to File](Guides/rtsp-save-original-stream.md)
- [RTSP Stream Viewer and IP Camera Player](Guides/rtsp-player-csharp.md)
- [ONVIF Capture with Post-Processing](Guides/onvif-capture-with-postprocessing.md)
- [Barcode & QR Code Reader](Guides/barcode-qr-reader-guide.md)
- [Pre-Event Recording](Guides/pre-event-recording.md)
- [Audio Metadata Tags](Guides/audio-metadata-tags.md)
- [Custom Video Effects and OpenGL Shaders](Guides/custom-video-effects-csharp.md)

## Developer Resources

- [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)
- [Deployment Guide](../deployment-x/index.md)
- [API Reference](https://api.visioforge.org/dotnet/api/index.html)
- [Changelog](../changelog.md)
- [End User License Agreement](../../eula.md)
- [Licensing Information](../../../licensing.md)
