---
title: Media Blocks Pipeline Core for Media Processing
description: Discover how to efficiently utilize the Media Blocks Pipeline to create powerful media applications for video playback, recording, and streaming. Learn essential pipeline operations including creation, block connections, error handling, and proper resource management.
sidebar_label: Pipeline Core Usage
order: 0

---

# Media Blocks Pipeline: Core Functionality

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## Overview of Pipeline and Block Structure

The Media Blocks SDK is built around the `MediaBlocksPipeline` class, which manages a collection of modular processing blocks. Each block implements the `IMediaBlock` interface or one of its specialized variants. Blocks are connected via input and output pads, allowing for flexible media processing chains.

### Main Block Interfaces

- **IMediaBlock**: Base interface for all blocks. Exposes properties for name, type, input/output pads, and methods for YAML conversion and pipeline context retrieval.
- **IMediaBlockDynamicInputs**: For blocks (like muxers) that can create new inputs dynamically. Methods: `CreateNewInput(mediaType)` and `GetInput(mediaType)`.
- **IMediaBlockInternals**: Internal methods for pipeline integration (e.g., `SetContext`, `Build`, `CleanUp`, `GetElement`, `GetCore`).
- **IMediaBlockInternals2**: For post-connection logic (`PostConnect()`).
- **IMediaBlockRenderer**: For renderer blocks, exposes `IsSync` property.
- **IMediaBlockSettings**: For settings/configuration objects that can create a block (`CreateBlock()`).
- **IMediaBlockSink**: For sink blocks, exposes filename/URL getter/setter.
- **IMediaBlockSource**: For source blocks (currently only commented-out pad accessors).

### Pads and Media Types

- **MediaBlockPad**: Represents a connection point (input/output) on a block. Has direction (`In`/`Out`), media type (`Video`, `Audio`, `Subtitle`, `Metadata`, `Auto`), and connection logic.
- **Pad connection**: Use `pipeline.Connect(outputPad, inputPad)` or `pipeline.Connect(block1.Output, block2.Input)`. For dynamic inputs, use `CreateNewInput()` on the sink block.

## Setting Up Your Pipeline Environment

### Creating a New Pipeline Instance

The first step in working with Media Blocks is instantiating a pipeline object:

```csharp
using VisioForge.Core.MediaBlocks;

// Create a standard pipeline instance
var pipeline = new MediaBlocksPipeline();

// Optionally, you can assign a name to your pipeline for easier identification
pipeline.Name = "MainVideoPlayer";
```

### Implementing Robust Error Handling

Media applications must handle various error scenarios that may occur during operation. Implementing proper error handling ensures your application remains stable:

```csharp
// Subscribe to error events to capture and handle exceptions
pipeline.OnError += (sender, args) =>
{
    // Log the error message
    Debug.WriteLine($"Pipeline error occurred: {args.Message}");
    
    // Implement appropriate error recovery based on the message
    if (args.Message.Contains("Access denied"))
    {
        // Handle permission issues
    }
    else if (args.Message.Contains("File not found"))
    {
        // Handle missing file errors
    }
};
```

## Managing Media Timing and Navigation

### Retrieving Duration and Position Information

Accurate timing control is essential for media applications:

```csharp
// Get the total duration of the media (returns TimeSpan.Zero for live streams)
var duration = await pipeline.DurationAsync();
Console.WriteLine($"Media duration: {duration.TotalSeconds} seconds");

// Get the current playback position
var position = await pipeline.Position_GetAsync();
Console.WriteLine($"Current position: {position.TotalSeconds} seconds");
```

### Implementing Seeking Functionality

Enable your users to navigate through media content with seeking operations:

```csharp
// Basic seeking to a specific time position
await pipeline.Position_SetAsync(TimeSpan.FromSeconds(10));

// Seeking with keyframe alignment for more efficient navigation
await pipeline.Position_SetAsync(TimeSpan.FromMinutes(2), seekToKeyframe: true);

// Advanced seeking with start and stop positions for partial playback
await pipeline.Position_SetRangeAsync(
    TimeSpan.FromSeconds(30),  // Start position
    TimeSpan.FromSeconds(60)   // Stop position
);
```

## Controlling Pipeline Execution Flow

### Starting Media Playback

Control the playback of media with these essential methods:

```csharp
// Start playback immediately
await pipeline.StartAsync();

// Preload media without starting playback (useful for reducing startup delay)
await pipeline.StartAsync(onlyPreload: true);
await pipeline.ResumeAsync(); // Start the preloaded pipeline when ready
```

### Managing Playback States

Monitor and control the pipeline's current execution state:

```csharp
// Check the current state of the pipeline
var state = pipeline.State;
if (state == PlaybackState.Play)
{
    Console.WriteLine("Pipeline is currently playing");
}

// Subscribe to important state change events
pipeline.OnStart += (sender, args) =>
{
    Console.WriteLine("Pipeline playback has started");
    UpdateUIForPlaybackState();
};

pipeline.OnStop += (sender, args) =>
{
    Console.WriteLine("Pipeline playback has stopped");
    Console.WriteLine($"Stopped at position: {args.Position.TotalSeconds} seconds");
    ResetPlaybackControls();
};

pipeline.OnPause += (sender, args) =>
{
    Console.WriteLine("Pipeline playback is paused");
    UpdatePauseButtonState();
};

pipeline.OnResume += (sender, args) =>
{
    Console.WriteLine("Pipeline playback has resumed");
    UpdatePlayButtonState();
};
```

### Pausing and Resuming Operations

Implement pause and resume functionality for better user experience:

```csharp
// Pause the current playback
await pipeline.PauseAsync();

// Resume playback from paused state
await pipeline.ResumeAsync();
```

### Stopping Pipeline Execution

Properly terminate pipeline operations:

```csharp
// Standard stop operation
await pipeline.StopAsync();

// Force stop in time-sensitive scenarios (may affect output file integrity)
await pipeline.StopAsync(force: true);
```

## Building Media Processing Chains

### Connecting Media Processing Blocks

The true power of the Media Blocks SDK comes from connecting specialized blocks to create processing chains:

```csharp
// Basic connection between two blocks
pipeline.Connect(block1.Output, block2.Input);

// Connect blocks with specific media types
pipeline.Connect(videoSource.GetOutputPadByType(MediaBlockPadMediaType.Video), 
                 videoEncoder.GetInputPadByType(MediaBlockPadMediaType.Video));
```

Different blocks may have multiple specialized inputs and outputs:

- Standard I/O: `Input` and `Output` properties
- Media-specific I/O: `VideoOutput`, `AudioOutput`, `VideoInput`, `AudioInput`
- Arrays of I/O: `Inputs[]` and `Outputs[]` for complex blocks

### Working with Dynamic Input Blocks

Some advanced sink blocks dynamically create inputs on demand:

```csharp
// Create a specialized MP4 muxer for recording
var mp4Muxer = new MP4SinkBlock();
mp4Muxer.FilePath = "output_recording.mp4";

// Request a new video input from the muxer
var videoInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Video);

// Connect a video source to the newly created input
pipeline.Connect(videoSource.Output, videoInput);

// Similarly for audio
var audioInput = mp4Muxer.CreateNewInput(MediaBlockPadMediaType.Audio);
pipeline.Connect(audioSource.Output, audioInput);
```

This flexibility enables complex media processing scenarios with multiple input streams.

## Proper Resource Management

### Disposing Pipeline Resources

Media applications can consume significant system resources. Always properly dispose of pipeline objects:

```csharp
// Synchronous disposal pattern
try
{
    // Use pipeline
}
finally
{
    pipeline.Dispose();
}
```

For modern applications, use the asynchronous pattern to prevent UI freezing:

```csharp
// Asynchronous disposal (preferred for UI applications)
try
{
    // Use pipeline
}
finally
{
    await pipeline.DisposeAsync();
}
```

### Using 'using' Statements for Automatic Cleanup

Leverage C# language features for automatic resource management:

```csharp
// Automatic disposal with 'using' statement
using (var pipeline = new MediaBlocksPipeline())
{
    // Configure and use pipeline
    await pipeline.StartAsync();
    // Pipeline will be automatically disposed when exiting this block
}

// C# 8.0+ using declaration
using var pipeline = new MediaBlocksPipeline();
// Pipeline will be disposed when the containing method exits
```

## Advanced Pipeline Features

### Playback Rate Control

Adjust playback speed for slow-motion or fast-forward effects:

```csharp
// Get current playback rate
double currentRate = await pipeline.Rate_GetAsync();

// Set playback rate (1.0 is normal speed)
await pipeline.Rate_SetAsync(0.5);  // Slow motion (half speed)
await pipeline.Rate_SetAsync(2.0);  // Double speed
```

### Loop Playback Configuration

Implement continuous playback functionality:

```csharp
// Enable looping for continuous playback
pipeline.Loop = true;

// Listen for loop events
pipeline.OnLoop += (sender, args) =>
{
    Console.WriteLine("Media has looped back to start");
    UpdateLoopCounter();
};
```

### Debug Mode for Development

Enable debugging features during development:

```csharp
// Enable debug mode for more detailed logging
pipeline.Debug_Mode = true;
pipeline.Debug_Dir = Path.Combine(Environment.GetFolderPath(
    Environment.SpecialFolder.MyDocuments), "PipelineDebugLogs");
```

## Block Types Reference

The SDK provides a wide range of block types for sources, processing, and sinks. See the `MediaBlockType` enum in the source code for a full list of available block types.

## Notes

- The pipeline supports both synchronous and asynchronous methods for starting, stopping, and disposing. Prefer asynchronous methods in UI or long-running applications.
- Events are available for error handling, state changes, and stream information.
- Use the correct interface for each block type to access specialized features (e.g., dynamic inputs, rendering, settings).
