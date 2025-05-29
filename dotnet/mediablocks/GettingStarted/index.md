---
title: Media Blocks SDK .Net - Developer Quick Start Guide
description: Learn to integrate Media Blocks SDK .Net into your applications with our detailed tutorial. From installation to implementation, discover how to create powerful multimedia pipelines, process video streams, and build robust media applications.
sidebar_label: Getting Started
order: 20
---

# Media Blocks SDK .Net - Developer Quick Start Guide

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## Introduction

This guide provides a comprehensive walkthrough for integrating the Media Blocks SDK .Net into your applications. The SDK is built around a modular pipeline architecture, enabling you to create, connect, and manage multimedia processing blocks for video, audio, and more. Whether you're building video processing tools, streaming solutions, or multimedia applications, this guide will help you get started quickly and correctly.

## SDK Installation Process

The SDK is distributed as a NuGet package for easy integration into your .Net projects. Install it using:

```bash
dotnet add package VisioForge.DotNet.MediaBlocks
```

For platform-specific requirements and additional installation details, refer to the [detailed installation guide](../../install/index.md).

## Core Concepts and Architecture

### MediaBlocksPipeline

- The central class for managing the flow of media data between processing blocks.
- Handles block addition, connection, state management, and event handling.
- Implements `IMediaBlocksPipeline` and exposes events such as `OnError`, `OnStart`, `OnPause`, `OnResume`, `OnStop`, and `OnLoop`.

### MediaBlock and Interfaces

- Each processing unit is a `MediaBlock` (or a derived class), implementing the `IMediaBlock` interface.
- Key interfaces:
  - `IMediaBlock`: Base interface for all blocks. Defines properties for `Name`, `Type`, `Input`, `Inputs`, `Output`, `Outputs`, and methods for pipeline context and YAML export.
  - `IMediaBlockDynamicInputs`: For blocks that support dynamic input creation (e.g., mixers).
  - `IMediaBlockInternals`/`IMediaBlockInternals2`: For internal pipeline management, building, and post-connection logic.
  - `IMediaBlockRenderer`: For blocks that render media (e.g., video/audio renderers), with a property to control stream synchronization.
  - `IMediaBlockSink`/`IMediaBlockSource`: For blocks that act as sinks (outputs) or sources (inputs).
  - `IMediaBlockSettings`: For settings objects that can create blocks.

### Pads and Media Types

- Blocks are connected via `MediaBlockPad` objects, which have a direction (`In`/`Out`) and a media type (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`).
- Pads can be connected/disconnected, and their state can be queried.

### Block Types

- The SDK provides a wide range of built-in block types (see `MediaBlockType` enum in the source code) for sources, sinks, renderers, effects, and more.

## Creating and Managing a Pipeline

### 1. Initialize the SDK (if required)

```csharp
using VisioForge.Core;

// Initialize the SDK at application startup
VisioForgeX.InitSDK();
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

### 6. SDK Cleanup (if required)

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

## Example: Minimal Pipeline Setup

```csharp
using VisioForge.Core.MediaBlocks;

var pipeline = new MediaBlocksPipeline();
var source = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
var renderer = new VideoRendererBlock(pipeline, videoViewControl);

pipeline.AddBlock(source);
pipeline.AddBlock(renderer);
pipeline.Connect(source.Output, renderer.Input);
await pipeline.StartAsync();
// ...
await pipeline.StopAsync();
pipeline.Dispose();
```

## Reference: Key Interfaces

- `IMediaBlock`: Base interface for all blocks.
- `IMediaBlockDynamicInputs`: For blocks with dynamic input support.
- `IMediaBlockInternals`, `IMediaBlockInternals2`: For internal pipeline logic.
- `IMediaBlockRenderer`: For renderer blocks.
- `IMediaBlockSink`, `IMediaBlockSource`: For sink/source blocks.
- `IMediaBlockSettings`: For block settings objects.
- `IMediaBlocksPipeline`: Main pipeline interface.
- `MediaBlockPad`, `MediaBlockPadDirection`, `MediaBlockPadMediaType`: For pad management.

## Further Reading and Samples

- [Complete Pipeline Implementation](pipeline.md)
- [Media Player Development Guide](player.md)
- [Camera Viewer Application Tutorial](camera.md)
- [GitHub repository with code samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK)

For a full list of block types and advanced usage, consult the SDK API reference and source code.
