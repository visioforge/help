---
title: Video Editing - Timeline, Transitions, Overlays in C# .NET
description: Build video editing apps with VisioForge Video Edit SDK .NET. Timeline editing, transitions, overlays, format conversion, and GPU-accelerated encoding.
sidebar_label: Video Edit SDK .NET
order: 12

---

# Video Edit SDK for C# .NET — Timeline Video Editing API

[Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Video Edit SDK for .NET is a C# video editing library that lets you build timeline-based video editing applications. Add video and audio files to a timeline, trim segments, apply transitions and effects, overlay text and images, and render the result to MP4, AVI, MKV, WebM, or other formats — all from your .NET code.

The SDK provides two engines: **VideoEditCore** (Windows-only, DirectShow-based) and **VideoEditCoreX** (cross-platform, runs on Windows, macOS, Linux, Android, and iOS). Both engines share the same timeline model — add sources, configure output, and start editing.

## Quick Start

### 1. Install NuGet Package

```bash
dotnet add package VisioForge.DotNet.VideoEditX
```

For platform-specific dependencies and UI framework setup, see the [Installation Guide](../install/index.md).

### 2. Minimal Video Editing Example

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

// Initialize SDK
VisioForgeX.InitSDK();

// Create editor with video preview
var editor = new VideoEditCoreX(videoView);

// Set output resolution and frame rate
editor.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
editor.Output_VideoFrameRate = new VideoFrameRate(30);

// Add video files to timeline
editor.Input_AddAudioVideoFile("intro.mp4", null, null, null);
editor.Input_AddAudioVideoFile("main.mp4", null, null, null);

// Set output format
editor.Output_Format = new MP4Output("output.mp4");

// Start editing
editor.Start();

// ... when done:
editor.Stop();
editor.Dispose();
VisioForgeX.DestroySDK();
```

For the complete implementation guide with event handling, audio sources, image sources, and advanced timeline configuration, see the [Getting Started Guide](getting-started.md).

## Common Use Cases

### Combine Multiple Video Files

Merge multiple video clips into a single output file. Add files to the timeline in sequence, set the output format, and render. Supports mixing different source formats — combine MP4, AVI, and MOV files into one output.

See: [Creating Videos from Multiple Sources](code-samples/output-file-from-multiple-sources.md)

### Trim and Cut Video Segments

Extract specific time ranges from video files by setting start and stop times on each source. Combine multiple segments from the same file or different files into a final edit.

See: [Working with Video Segments](code-samples/several-segments.md)

### Add Text and Image Overlays

Insert text titles, captions, logos, and watermarks over video content. Position, scale, and time overlays on the timeline.

See: [Text Overlay Implementation](code-samples/add-text-overlay.md) | [Adding Image Overlays](code-samples/add-image-overlay.md)

### Apply Transitions Between Clips

Add cross-dissolve, wipe, slide, fade, and 100+ SMPTE standard transitions between video segments. Control transition duration, border style, and direction.

See: [Transition Effects Between Video Fragments](code-samples/transition-video.md) | [Transitions Reference](transitions.md)

### Create Slideshow from Images

Build video slideshows from JPG, PNG, BMP, and GIF images with configurable display duration per image, transitions between slides, and background music.

See: [Generating Videos from Image Sequences](code-samples/video-images-console.md)

### Add Background Music and Audio Mixing

Mix multiple audio tracks with video content. Control volume per track, apply audio envelope effects for fade-in/fade-out, and synchronize audio with video.

See: [Audio Volume Envelope Effects](code-samples/audio-envelope.md) | [Custom Volume Control](code-samples/volume-for-track.md)

### Picture-in-Picture Composition

Layer multiple video sources with position and size control for picture-in-picture layouts, reaction videos, or multi-camera compositions.

See: [Picture-In-Picture Effects](code-samples/picture-in-picture.md)

## Supported Formats

| Category | Formats |
| -------- | ------- |
| Video Containers | MP4, AVI, MOV, WMV, MKV, WebM, TS, FLV |
| Video Codecs | H.264, H.265/HEVC, VP9, AV1, MPEG-2 |
| Audio Formats | AAC, MP3, WMA, OPUS, Vorbis, FLAC, WAV |
| Image Formats | JPG, PNG, BMP, GIF |

## Platform Support

| Platform | UI Frameworks | Engine | Notes |
| -------- | ------------- | ------ | ----- |
| Windows x64 | WinForms, WPF, MAUI, Avalonia, Console | VideoEditCore, VideoEditCoreX | Full feature set including DirectShow bridges |
| macOS | MAUI, Avalonia, Console | VideoEditCoreX | Intel and Apple Silicon |
| Linux x64 | Avalonia, Console | VideoEditCoreX | Ubuntu, Debian, CentOS |
| Android | MAUI | VideoEditCoreX | Via MAUI integration |
| iOS | MAUI | VideoEditCoreX | Via MAUI integration |

## Developer Documentation

### Guides

* [Getting Started Guide](getting-started.md) — Full implementation walkthrough with both engines
* [Code Examples](code-samples/index.md) — Ready-to-use examples for overlays, transitions, audio, and composition
* [Deployment Guide](deployment.md) — NuGet packages, installers, and manual installation
* [Transitions Reference](transitions.md) — 100+ SMPTE wipe transition codes and properties

### iOS

* [iOS Video Editor](code-samples/ios-video-editor.md) — Building video editing apps for iPhone and iPad

## Developer Resources

* [Code Samples on GitHub](https://github.com/visioforge/.Net-SDK-s-samples)
* [API Reference](https://api.visioforge.org/dotnet/api/index.html)
* [Changelog](../changelog.md)
* [End User License Agreement](../../eula.md)
* [Licensing Information](../../../licensing.md)
