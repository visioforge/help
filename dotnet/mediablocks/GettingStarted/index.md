---
title: Media Pipeline Quick Start - Install and Setup in C# .NET
description: Get started with VisioForge Media Blocks SDK — installation, pipeline architecture, block connections, and multimedia processing tutorials.
sidebar_label: Getting Started
order: 20
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
primary_api_classes:
  - MediaBlocksPipeline
  - IMediaBlock
  - MediaBlockPad
  - IMediaBlocksPipeline
  - MediaBlock
  - SystemVideoSourceBlock
  - VideoRendererBlock
  - DeviceEnumerator

---

# Media Blocks SDK .NET - Developer Quick Start Guide

[Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

This guide provides a comprehensive walkthrough for integrating the Media Blocks SDK .NET into your applications. The SDK is built around a modular pipeline architecture, enabling you to create, connect, and manage multimedia processing blocks for video, audio, and more. Whether you're building video processing tools, streaming solutions, or multimedia applications, this guide will help you get started quickly and correctly.

## SDK Installation Process

The SDK is distributed as a NuGet package for easy integration into your .NET projects. Install it using:

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

This package holds the managed SDK only. The native engine — GStreamer and its plugin set — comes in a separate redistributable package that you add next to it. An application with the managed package alone builds and then stops at start-up: `VisioForgeX.InitSDKAsync` throws `DllNotFoundException` before any window opens.

For a Windows x64 application:

```bash
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

The first carries the engine; every application needs it. The second adds the FFmpeg (libav) decoders and encoders and is recommended for every application.

Use the architecture of the running process: `.x86` for a 32-bit build and `.arm64` for a Windows on Arm build (no NDI there) — a redist of another architecture fails to load into the process. An AnyCPU application usually runs as x64 on a 64-bit Windows, and as ARM64 on Windows on Arm.

A redist package must be the newest release at or before your `VisioForge.DotNet.MediaBlocks` release — never newer, and never far behind. Pin it with `--version` when needed: `dotnet add package VisioForge.CrossPlatform.Core.Windows.x64 --version <version>`.

The other platforms take their own redistributable package:

- **macOS, Mac Catalyst** - `VisioForge.CrossPlatform.Core.macOS`; Mac Catalyst also needs a build target that copies the native libraries into the app bundle ([macOS deployment](../../deployment-x/macOS.md))
- **iOS** - `VisioForge.CrossPlatform.Core.iOS` ([iOS deployment](../../deployment-x/iOS.md))
- **Android** - `VisioForge.CrossPlatform.Core.Android`, and the Java bindings project ([Android deployment](../../deployment-x/Android.md))
- **Linux** - `VisioForge.CrossPlatform.Core.Linux.x64` or `.arm64`, and GStreamer 1.20+ from the distribution ([Ubuntu deployment](../../deployment-x/Ubuntu.md))

For the required packages, refer to the [detailed installation guide](../../install/index.md); the optional ones are listed in the [deployment guide](../../deployment-x/index.md).

## Your First Pipeline

The smallest useful pipeline has two blocks: a source and a renderer. This one shows the first camera found on the machine:

```csharp
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.VideoRendering;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

// Call from the UI thread once your window exists.
async Task<MediaBlocksPipeline> StartCameraAsync(IVideoView videoView)
{
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];

    var pipeline = new MediaBlocksPipeline();
    var source = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(device));
    var renderer = new VideoRendererBlock(pipeline, videoView);

    pipeline.AddBlock(source);
    pipeline.AddBlock(renderer);
    pipeline.Connect(source.Output, renderer.Input);

    await pipeline.StartAsync();

    return pipeline;
}
```

- The SDK must be initialized first with `await VisioForgeX.InitSDKAsync();` — see step 1 below.
- No camera attached? Drop the `device` line and use the test pattern: `var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());` in place of `SystemVideoSourceBlock`. The connection and start lines are the same.
- Store the returned pipeline in a field and stop it when the window closes: `await pipeline.StopAsync(); pipeline.Dispose();`.
- Device and format selection, snapshots and platform specifics: [Camera Viewer Application Tutorial](camera.md).

## Core Concepts and Architecture

### MediaBlocksPipeline

- The central class for managing the flow of media data between processing blocks.
- Handles block addition, connection, state management, and event handling.
- Implements `IMediaBlocksPipeline` and exposes events such as `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop`, and `OnLoop`.

### MediaBlock and Interfaces

- Each processing unit is a `MediaBlock` (or a derived class), implementing the `IMediaBlock` interface.
- Key interfaces:
  - `IMediaBlock`: Base interface for all blocks. Defines properties for `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs`, and a method for the block's pipeline context.
  - `IMediaBlockDynamicInputs`: For blocks that support dynamic input creation (e.g., mixers).
  - `IMediaBlockRenderer`: For blocks that render media (e.g., video/audio renderers), with a property to control stream synchronization.
  - `IMediaBlockSettings`: For settings objects that can create blocks.

### Pads and Media Types

- Blocks are connected via `MediaBlockPad` objects, which have a direction (`In`/`Out`) and a media type (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Pads can be connected/disconnected, and their state can be queried.

### Block Types

- The SDK provides a wide range of built-in block types (listed in the `MediaBlockType` enum) for sources, sinks, renderers, effects, and more.
- The block catalogue: [Sources](../Sources/index.md), [Video Rendering](../VideoRendering/index.md), [Sinks](../Sinks/index.md), [Video Processing](../VideoProcessing/index.md).

## Creating and Managing a Pipeline

### 1. Initialize the SDK

```csharp
using VisioForge.Core;

// Initialize the SDK at application startup (await returns once native libraries are loaded)
await VisioForgeX.InitSDKAsync();
```

### 2. Create a Pipeline and Blocks

```csharp
using VisioForge.Core.MediaBlocks;

// Create a new pipeline instance
var pipeline = new MediaBlocksPipeline();

// Example: Create a virtual video source and a video renderer
var virtualSource = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // VideoView1 is your UI control

// Add blocks to the pipeline
pipeline.AddBlock(virtualSource);
pipeline.AddBlock(videoRenderer);
```

`VideoView1` is the `VisioForge.Core.UI.WPF.VideoView` control of a WPF application. Declare it in XAML with `xmlns:WPF="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"` and `<WPF:VideoView x:Name="VideoView1" />`. Windows Forms uses `VisioForge.Core.UI.WinForms.VideoView` from the same assembly; MAUI, Avalonia, WinUI and Uno ship their own `VideoView` in their own UI packages.

### 3. Connect Blocks

```csharp
// Connect the output of the source to the input of the renderer
pipeline.Connect(virtualSource.Output, videoRenderer.Input);
```

- You can also use `pipeline.Connect(sourceBlock, targetBlock)` to connect default pads, or connect multiple pads for complex graphs.
- For blocks supporting dynamic inputs, use the `IMediaBlockDynamicInputs` interface.

### 4. Start and Stop the Pipeline

```csharp
// Start the pipeline asynchronously
await pipeline.StartAsync();

// ... later, stop processing
await pipeline.StopAsync();
```

### 5. Resource Cleanup

```csharp
// Dispose of the pipeline when done
pipeline.Dispose();
```

### 6. SDK Cleanup

```csharp
// Release all SDK resources at application shutdown
VisioForgeX.DestroySDK();
```

## Error Handling and Events

- Subscribe to pipeline events for robust error and state management:

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine($"Pipeline error: {args.Message}");
    // Implement your error handling logic here
};

pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("Pipeline started");
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("Pipeline stopped");
};
```

## Advanced Features

- **Dynamic Block Addition/Removal:** You can add or remove blocks at runtime as needed.
- **Pad Management:** Use `MediaBlockPad` methods to query and manage pad connections.
- **Hardware/Software Decoder Selection:** Use helper methods in `MediaBlocksPipeline` for hardware acceleration.
- **Segment Playback:** Set `StartPosition` and `StopPosition` properties for partial playback.
- **Debugging:** Export pipeline graphs for debugging using provided methods.

## Further Reading and Samples

- [Complete Pipeline Implementation](pipeline.md)
- [Media Player Development Guide](player.md)
- [Camera Viewer Application Tutorial](camera.md)
- [GitHub repository with code samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

For every block property and default, see the [API reference](https://api.visioforge.org/dotnet/api/index.html).
