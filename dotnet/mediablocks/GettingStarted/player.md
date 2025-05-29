---
title: Media Blocks SDK .Net Player Implementation Guide
description: Learn how to build a robust video player application with Media Blocks SDK .Net. This step-by-step tutorial covers essential components including source blocks, video rendering, audio output configuration, pipeline creation, and advanced playback controls for .NET developers.
sidebar_label: Player Sample
---

# Building a Feature-Rich Video Player with Media Blocks SDK

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

This detailed tutorial walks you through the process of creating a professional-grade video player application using Media Blocks SDK .Net. By following these instructions, you'll understand how to implement key functionalities including media loading, playback control, and audio-video rendering.

## Essential Components for Your Player Application

To construct a fully functional video player, your application pipeline requires these critical building blocks:

- [Universal source](../Sources/index.md) - This versatile component handles media input from various sources, allowing your player to read and process video files from local storage or network streams.
- [Video renderer](../VideoRendering/index.md) - The visual component responsible for displaying video frames on screen with proper timing and formatting.
- [Audio renderer](../AudioRendering/index.md) - Manages sound output, ensuring synchronized audio playback alongside your video content.

## Setting Up the Media Pipeline

### Creating the Foundation

The first step in developing your player involves establishing the media pipeline—the core framework that manages data flow between components.

```csharp
using VisioForge.Core.MediaBlocks;

var pipeline = new MediaBlocksPipeline();
```

### Implementing Error Handling

Robust error management is essential for a reliable player application. Subscribe to the pipeline's error events to capture and respond to exceptions.

```csharp
pipeline.OnError += (sender, args) =>
{
    Console.WriteLine(args.Message);
    // Additional error handling logic can be implemented here
};
```

### Setting Up Event Listeners

For complete control over your player's lifecycle, implement event handlers for critical state changes:

```csharp
pipeline.OnStart += (sender, args) => 
{
    // Execute code when pipeline starts
    Console.WriteLine("Playback started");
};

pipeline.OnStop += (sender, args) => 
{
    // Execute code when pipeline stops
    Console.WriteLine("Playback stopped");
};
```

## Configuring Media Blocks

### Initializing the Source Block

The Universal Source Block serves as the entry point for media content. Configure it with the path to your media file:

```csharp
var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri(filePath));
var fileSource = new UniversalSourceBlock(sourceSettings);
```

During initialization, the SDK automatically analyzes the file to extract crucial metadata about video and audio streams, enabling proper configuration of downstream components.

### Setting Up Video Display

To render video content on screen, create and configure a Video Renderer Block:

```csharp
var videoRenderer = new VideoRendererBlock(_pipeline, VideoView1);
```

The renderer requires two parameters: a reference to your pipeline and the UI control where video frames will be displayed.

### Configuring Audio Output

For audio playback, you'll need to select and initialize an appropriate audio output device:

```csharp
var audioRenderers = await DeviceEnumerator.Shared.AudioOutputsAsync();
var audioRenderer = new AudioRendererBlock(audioRenderers[0]);
```

This code retrieves available audio output devices and configures the first available option for playback.

## Establishing Component Connections

Once all blocks are configured, you must establish connections between them to create a cohesive media flow:

```csharp
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);
```

These connections define the path data takes through your application:

- Video data flows from the source to the video renderer
- Audio data flows from the source to the audio renderer

For files containing only video or audio, you can selectively connect only the relevant outputs.

### Validating Media Content

Before playback, you can inspect available streams using the Universal Source Settings:

```csharp
var mediaInfo = await sourceSettings.ReadInfoAsync();
bool hasVideo = mediaInfo.VideoStreams.Count > 0;
bool hasAudio = mediaInfo.AudioStreams.Count > 0;
```

## Controlling Media Playback

### Starting Playback

To begin media playback, call the pipeline's asynchronous start method:

```csharp
await pipeline.StartAsync();
```

Once executed, your application will begin rendering video frames and playing audio through the configured outputs.

### Managing Playback State

To halt playback, invoke the pipeline's stop method:

```csharp
await pipeline.StopAsync();
```

This gracefully terminates all media processing and releases associated resources.

## Advanced Implementation

For a complete implementation example with additional features like seeking, volume control, and full-screen support, refer to our comprehensive source code on [GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Player%20Demo%20WPF).

The repository contains working demonstrations for various platforms including WPF, Windows Forms, and cross-platform .NET applications.
