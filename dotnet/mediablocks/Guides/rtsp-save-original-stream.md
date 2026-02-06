---
title: Save Original RTSP Stream (No Video Re-encoding)
description: Save RTSP stream to file (MP4) from IP camera without re-encoding video using .NET with VisioForge Media Blocks for programmatic control.
---

# How to Save RTSP Stream to File: Record IP Camera Video without Re-encoding

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!!info Demo Sample
For a complete working example of recording RTSP streams without re-encoding, see the [RTSP MultiView Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WinForms/CSharp/RTSP%20MultiView%20Demo).

For ONVIF camera-specific documentation, see the [ONVIF IP Camera Integration Guide](../../videocapture/video-sources/ip-cameras/onvif.md).
!!!

## Table of Contents

- [How to Save RTSP Stream to File: Record IP Camera Video without Re-encoding](#how-to-save-rtsp-stream-to-file-record-ip-camera-video-without-re-encoding)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Core Features](#core-features)
  - [Core Concept](#core-concept)
  - [Prerequisites](#prerequisites)
  - [Code Sample: RTSPRecorder Class](#code-sample-rtsprecorder-class)
  - [Explanation of the Code](#explanation-of-the-code)
  - [How to Use the `RTSPRecorder`](#how-to-use-the-rtsprecorder)
  - [Key Considerations](#key-considerations)
  - [Full GitHub Sample](#full-github-sample)
  - [Best Practices](#best-practices)
  - [Troubleshooting](#troubleshooting)

## Overview

This guide demonstrates how to save an RTSP stream to an MP4 file by capturing the original video stream from an RTSP IP camera without re-encoding the video. This approach is highly beneficial for preserving the original video quality from cameras and minimizing CPU usage when you need to record footage. The audio stream can be passed through or, optionally, re-encoded for better compatibility, allowing you to save the complete streaming data. Tools like FFmpeg and VLC offer command-line or UI-based methods to record an RTSP stream; however, this guide focuses on a programmatic approach using the VisioForge Media Blocks SDK for .NET developers who need to create applications that connect to and record video from RTSP cameras.

## Core Features

- **Direct Stream Recording**: Save RTSP camera feeds without quality loss
- **CPU-Efficient Processing**: No video re-encoding required
- **Flexible Audio Handling**: Pass-through or re-encode audio as needed
- **Professional Integration**: Programmatic control for enterprise applications
- **High Performance**: Optimized for continuous recording

We will be using the VisioForge Media Blocks SDK, a powerful .NET library for building custom media processing applications, to effectively save RTSP to file.

## Core Concept

The main idea is to take the raw video stream from the RTSP source and directly send it to a file sink (e.g., MP4 muxer) without any decoding or encoding steps for the video. This is a common requirement for recording RTSP streams with maximum fidelity.

- **Video Stream**: Passed through directly from the RTSP source to the MP4 sink. This ensures the original video data is saved, crucial for applications that need to record high-quality footage from cameras.
- **Audio Stream**: Can either be passed through directly (if the original audio codec is compatible with the MP4 container) or re-encoded (e.g., to AAC) to ensure compatibility and potentially reduce file size when you save the RTSP stream.

## Prerequisites

You'll need the VisioForge Media Blocks SDK. You can add it to your .NET project via NuGet:

```xml
<PackageReference Include="VisioForge.DotNet.MediaBlocks" Version="2025.5.2" />
```

Depending on your target platform (Windows, macOS, Linux, including ARM-based systems like Jetson Nano for embedded camera applications), you will also need the corresponding native runtime packages. For example, on Windows to record video:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.4.9" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64.UPX" Version="2025.4.9" />
```

For detailed information about deployment requirements, and platform-specific dependencies, please refer to our [Deployment Guide](../../deployment-x/index.md). It's important to check these details to ensure your video stream capture application works correctly.

Refer to the `RTSP Capture Original.csproj` file in the sample project for a complete list of dependencies for different platforms.

## Code Sample: RTSPRecorder Class

The following C# code defines an `RTSPRecorder` class that encapsulates the logic for capturing and saving the RTSP stream.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sinks;
using VisioForge.Core.Types.X.Sources;

namespace RTSPCaptureOriginalStream
{
    /// <summary>
    /// RTSPRecorder class encapsulates the RTSP recording functionality to save RTSP stream to file.
    /// It uses the MediaBlocks SDK to create a pipeline that connects an 
    /// RTSP source (like an IP camera) to an MP4 sink (file).
    /// </summary>
    public class RTSPRecorder : IAsyncDisposable
    {
        /// <summary>
        /// The MediaBlocks pipeline that manages the flow of media data.
        /// </summary>
        public MediaBlocksPipeline Pipeline { get; private set; }

        // Private fields for the MediaBlock components
        private MediaBlock _muxer;               // MP4 container muxer (sink)
        private RTSPRAWSourceBlock _rtspRawSource; // RTSP stream source (provides raw streams)
        private DecodeBinBlock _decodeBin;       // Optional: Audio decoder (if re-encoding audio)
        private AACEncoderBlock _audioEncoder;   // Optional: AAC audio encoder (if re-encoding audio)
        private bool disposedValue;              // Flag to prevent multiple disposals

        /// <summary>
        /// Event fired when an error occurs in the pipeline.
        /// </summary>
        public event EventHandler<ErrorsEventArgs> OnError;

        /// <summary>
        /// Event fired when a status message is available.
        /// </summary>
        public event EventHandler<string> OnStatusMessage;

        /// <summary>
        /// Output filename for the MP4 recording.
        /// </summary>
        public string Filename { get; set; } = "output.mp4";

        /// <summary>
        /// Whether to re-encode audio to AAC format (recommended for compatibility).
        /// If false, audio is passed through.
        /// </summary>
        public bool ReencodeAudio { get; set; } = true;

        /// <summary>
        /// Starts the recording session by creating and configuring the MediaBlocks pipeline.
        /// </summary>
        /// <param name="rtspSettings">RTSP source configuration settings.</param>
        /// <returns>True if the pipeline started successfully, false otherwise.</returns>
        public async Task<bool> StartAsync(RTSPRAWSourceSettings rtspSettings)
        {
            // Create a new MediaBlocks pipeline
            Pipeline = new MediaBlocksPipeline();
            Pipeline.OnError += (sender, e) => OnError?.Invoke(this, e); // Bubble up errors

            OnStatusMessage?.Invoke(this, "Creating pipeline to record RTSP stream...");

            // 1. Create the RTSP source block.
            // RTSPRAWSourceBlock provides raw, un-decoded elementary streams (video and audio) from your IP camera or other RTSP cameras.
            _rtspRawSource = new RTSPRAWSourceBlock(rtspSettings);
            
            // 2. Create the MP4 sink (muxer) block.
            // This block will write the media streams into an MP4 file.
            _muxer = new MP4SinkBlock(new MP4SinkSettings(Filename));

            // 3. Connect Video Stream (Passthrough)
            // Create a dynamic input pad on the muxer for the video stream.
            // We connect the raw video output from the RTSP source directly to the MP4 sink.
            // This ensures the video is not re-encoded when you record the camera feed.
            var inputVideoPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video);
            Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);
            OnStatusMessage?.Invoke(this, "Video stream connected (passthrough for original quality video).");

            // 4. Connect Audio Stream (Optional Re-encoding)
            // This section handles how the audio from the RTSP stream is processed and saved to the file.
            if (rtspSettings.AudioEnabled)
            {
                // Create a dynamic input pad on the muxer for the audio stream.
                var inputAudioPad = (_muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio);

                if (ReencodeAudio)
                {
                    // If audio re-encoding is enabled (e.g., to AAC for compatibility):
                    OnStatusMessage?.Invoke(this, "Setting up audio re-encoding to AAC for the recording...");
                    
                    // Create a decoder block that only handles audio.
                    // We need to decode the original audio before re-encoding it to save the MP4 stream with compatible audio.
                    _decodeBin = new DecodeBinBlock(videoDisabled: false, audioDisabled: true, subtitlesDisabled: false) 
                    {
                         // We can disable the internal audio converter if we're sure about the format 
                         // or if the encoder handles conversion. For AAC, it's generally fine.
                         DisableAudioConverter = true 
                    };

                    // Create an AAC encoder with default settings.
                    _audioEncoder = new AACEncoderBlock(new AVENCAACEncoderSettings());

                    // Connect the audio processing pipeline:
                    // RTSP audio output -> Decoder -> AAC Encoder -> MP4 Sink audio input
                    Pipeline.Connect(_rtspRawSource.AudioOutput, _decodeBin.Input);
                    Pipeline.Connect(_decodeBin.AudioOutput, _audioEncoder.Input);
                    Pipeline.Connect(_audioEncoder.Output, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Audio stream connected (re-encoding to AAC for MP4 file).");
                }
                else
                {
                    // If audio re-encoding is disabled, connect RTSP audio directly to the muxer.
                    // Note: This may cause issues if the original audio format is not 
                    // compatible with the MP4 container (e.g., G.711 PCMU/PCMA) when trying to save the RTSP stream.
                    // Common compatible formats include AAC. Check your camera's audio format.
                    Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);
                    OnStatusMessage?.Invoke(this, "Audio stream connected (passthrough). Warning: Compatibility depends on original camera audio format for the file.");
                }
            }

            // 5. Start the pipeline to record video
            OnStatusMessage?.Invoke(this, "Starting recording pipeline to save RTSP stream to file...");
            bool success = await Pipeline.StartAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Recording pipeline started successfully.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Failed to start recording pipeline.");
            }
            return success;
        }

        /// <summary>
        /// Stops the recording by stopping the MediaBlocks pipeline.
        /// </summary>
        /// <returns>True if the pipeline stopped successfully, false otherwise.</returns>
        public async Task<bool> StopAsync()
        {
            if (Pipeline == null)
                return false;

            OnStatusMessage?.Invoke(this, "Stopping recording pipeline...");
            bool success = await Pipeline.StopAsync();
            if (success)
            {
                OnStatusMessage?.Invoke(this, "Recording pipeline stopped successfully.");
            }
            else
            {
                OnStatusMessage?.Invoke(this, "Failed to stop recording pipeline.");
            }
            
            // Detach the error handler to prevent issues if StopAsync is called multiple times
            // or before DisposeAsync
            if (Pipeline != null)
            {
                 Pipeline.OnError -= OnError;
            }

            return success;
        }

        /// <summary>
        /// Asynchronously disposes of the RTSPRecorder and all its resources.
        /// Implements the IAsyncDisposable pattern for proper resource cleanup.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            if (!disposedValue)
            {
                if (Pipeline != null)
                {
                    Pipeline.OnError -= (sender, e) => OnError?.Invoke(this, e); // Ensure detachment
                    await Pipeline.DisposeAsync();
                    Pipeline = null;
                }

                // Dispose of all MediaBlock components
                // Using 'as IDisposable' for safe casting and disposal.
                (_muxer as IDisposable)?.Dispose();
                _muxer = null;

                _rtspRawSource?.Dispose();
                _rtspRawSource = null;

                _decodeBin?.Dispose();
                _decodeBin = null;

                _audioEncoder?.Dispose();
                _audioEncoder = null;

                disposedValue = true;
            }
        }
    }
}
```

## Explanation of the Code

1. **`RTSPRecorder` Class**: This class is central to helping a user save RTSP stream to file.
    - Implements `IAsyncDisposable` for proper resource management.
    - `Pipeline`: The `MediaBlocksPipeline` object that orchestrates the media flow.
    - `_rtspRawSource`: An `RTSPRAWSourceBlock` is used. The "RAW" is key here, as it provides the elementary streams (video and audio) from camera without attempting to decode them initially.
    - `_muxer`: An `MP4SinkBlock` is used to write the incoming video and audio streams into an MP4 file.
    - `_decodeBin` and `_audioEncoder`: These are optional blocks used only if `ReencodeAudio` is true. `_decodeBin` decodes the original audio from the IP camera, and `_audioEncoder` (e.g., `AACEncoderBlock`) re-encodes it to a more compatible format like AAC.
    - `Filename`: Specifies the output MP4 file path where the video will be saved.
    - `ReencodeAudio`: A boolean property to control audio processing. If `true`, audio is re-encoded to AAC. If `false`, audio is passed through directly. Check your camera audio format for compatibility if set to false.

2. **`StartAsync(RTSPRAWSourceSettings rtspSettings)` Method**: This method initiates the process to **record RTSP stream**.
    - Initializes `MediaBlocksPipeline`.
    - **RTSP Source**: Creates `_rtspRawSource` with `RTSPRAWSourceSettings`. These settings include the URL (the path to your camera's stream), credentials for user access, and audio capture settings.
    - **MP4 Sink**: Creates `_muxer` (MP4 sink) with the target filename.
    - **Video Path (Passthrough)**:
        - A new dynamic input pad for video is created on the `_muxer`.
        - `Pipeline.Connect(_rtspRawSource.VideoOutput, inputVideoPad);` This line directly connects the raw video output from the RTSP source to the MP4 muxer's*video input. No re-encoding occurs for the video stream.
    - **Audio Path (Conditional)**: Determines how audio from the **camera** is handled when you **save to file**.
        - If `rtspSettings.AudioEnabled` is true:
            - A new dynamic input pad for audio is created on the `_muxer`.
            - If `ReencodeAudio` is `true` (recommended for wider file compatibility):
                - `_decodeBin` is created to decode the incoming audio from the camera. It's configured to only process audio (`audioDisabled: false`).
                - `_audioEncoder` (e.g., `AACEncoderBlock`) is created.
                - The pipeline is connected: `_rtspRawSource.AudioOutput` -> `_decodeBin.Input` -> `_decodeBin.AudioOutput` -> `_audioEncoder.Input` -> `_audioEncoder.Output` -> `inputAudioPad` (muxer's audio input).
            - If `ReencodeAudio` is `false`:
                - `Pipeline.Connect(_rtspRawSource.AudioOutput, inputAudioPad);` The raw audio output from the camera source is connected directly to the MP4 muxer. *Caution*: This relies on the original audio codec from the camera being compatible with the MP4 container (e.g., AAC). Formats like G.711 (PCMU/PCMA) are common in RTSP cameras but are not standard in MP4 and might lead to playback issues or require specialized players if you save this way. Check your camera's documentation.
    - Starts the pipeline using `Pipeline.StartAsync()` to begin the streaming video record process.

3. **`StopAsync()` Method**: Stops the `Pipeline`.

4. **`DisposeAsync()` Method**:
    - Cleans up all resources, including the pipeline and individual media blocks.

## How to Use the `RTSPRecorder`

Here's a basic example of how you might use the `RTSPRecorder` class:

```csharp
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VisioForge.Core; // For VisioForgeX.DestroySDK()
using VisioForge.Core.Types.X.Sources; // For RTSPRAWSourceSettings
using RTSPCaptureOriginalStream; // Namespace of your RTSPRecorder class

class Demo
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("RTSP Camera to MP4 Capture (Original Video Stream)");
        Console.WriteLine("-------------------------------------------------");

        string rtspUrl = "rtsp://your_camera_ip:554/stream_path"; // Replace with your RTSP URL
        string username = "admin"; // Replace with your username, or empty if none
        string password = "password"; // Replace with your password, or empty if none
        string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "rtsp_original_capture.mp4");

        Directory.CreateDirectory(Path.GetDirectoryName(outputFilePath));

        Console.WriteLine($"Capturing from: {rtspUrl}");
        Console.WriteLine($"Saving to: {outputFilePath}");
        Console.WriteLine("Press any key to stop recording...");

        var cts = new CancellationTokenSource();
        RTSPRecorder recorder = null;

        try
        {
            recorder = new RTSPRecorder
            {
                Filename = outputFilePath,
                ReencodeAudio = true // Set to false to pass through audio (check compatibility)
            };

            recorder.OnError += (s, e) => Console.WriteLine($"ERROR: {e.Message}");
            recorder.OnStatusMessage += (s, msg) => Console.WriteLine($"STATUS: {msg}");

            // Configure RTSP source settings
            var rtspSettings = new RTSPRAWSourceSettings(new Uri(rtspUrl), audioEnabled: true)
            {
                Login = username,
                Password = password,
                // Adjust other settings as needed, e.g., transport protocol
                // RTSPTransport = VisioForge.Core.Types.RTSPTransport.TCP, 
            };

            if (await recorder.StartAsync(rtspSettings))
            {
                Console.ReadKey(true); // Wait for a key press to stop
            }
            else
            {
                Console.WriteLine("Failed to start recording. Check status messages and RTSP URL/credentials.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An unexpected error occurred: {ex.Message}");
        }
        finally
        {
            if (recorder != null)
            {
                Console.WriteLine("Stopping recording...");
                await recorder.StopAsync();
                await recorder.DisposeAsync();
                Console.WriteLine("Recording stopped and resources disposed.");
            }

            // Important: Clean up VisioForge SDK resources on application exit
            VisioForgeX.DestroySDK(); 
        }

        Console.WriteLine("Press any key to exit.");
        Console.ReadKey(true);
    }
}
```

## Key Considerations

- **Audio Compatibility (Passthrough)**: If you choose `ReencodeAudio = false`, ensure the camera's audio codec (e.g., AAC, MP3) is compatible with the MP4 container. Common RTSP audio codecs like G.711 (PCMU/PCMA) are generally not directly supported in MP4 files and will likely result in silent audio or playback errors. Re-encoding to AAC is generally safer for wider compatibility.
- **Network Conditions**: RTSP streaming is sensitive to network stability, so ensure a reliable network connection to the camera.
- **Error Handling**: Robust applications should implement thorough error handling by subscribing to the `OnError` event of the `RTSPRecorder` (or directly from the `MediaBlocksPipeline`).
- **Resource Management**: Always `DisposeAsync` the `RTSPRecorder` instance (and thus the `MediaBlocksPipeline`) when done to free up resources. `VisioForgeX.DestroySDK()` should be called once when your application exits.

## Full GitHub Sample

For a complete, runnable console application demonstrating these concepts, including user input for RTSP details and dynamic duration display, please refer to the official VisioForge samples repository:

- **[RTSP Capture Original Stream Sample on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Console/RTSP%20Capture%20Original)**

This sample provides a more comprehensive example and showcases additional features.

## Best Practices

- Always implement proper error handling
- Monitor network stability for reliable streaming
- Use appropriate audio encoding settings
- Manage system resources effectively
- Implement proper cleanup procedures

## Troubleshooting

Common issues and their solutions when saving RTSP streams:

- Network connectivity problems
- Audio codec compatibility
- Resource management
- Stream initialization errors
- Recording storage considerations

---
This guide provides a foundational understanding of how to save an RTSP stream's original video while flexibly handling the audio stream using the VisioForge Media Blocks SDK. By leveraging the `RTSPRAWSourceBlock` and direct pipeline connections, you can achieve efficient, high-quality recordings.