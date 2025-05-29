---
title: Video Capture to MPEG-TS Files in C# and .NET
description: Learn how to capture video and audio to MPEG-TS files in C# applications. Step-by-step guide with code samples covering hardware acceleration, format selection, and cross-platform considerations for .NET developers.
sidebar_label: Video Capture to MPEG-TS
---

# Video Capture to MPEG-TS in C# and .NET: Complete Guide

## Introduction

This technical guide demonstrates how to capture C# TS video from cameras and microphones using two powerful VisioForge multimedia solutions: Video Capture SDK .NET with VideoCaptureCoreX engine and Media Blocks SDK .NET with MediaBlocksPipeline engine. Both SDKs provide robust capabilities for capturing, recording, and editing TS (MPEG Transport Stream) files in .NET applications. We'll explore detailed code samples for implementing video/audio capture to TS in C# with optimized performance and quality.

## Installation and deployment

Please refer to the [installation guide](../../install/index.md) for detailed instructions on how to install the VisioForge .NET SDKs on your system.

## Video Capture SDK .NET (VideoCaptureCoreX) - Capture MPEG-TS in C#

VideoCaptureCoreX provides a streamlined approach to capture TS video and audio in C#. Its component-based architecture handles the complex media pipeline, allowing developers to focus on configuration rather than lower-level implementation details when working with TS files in .NET.

### Core Components

1. **VideoCaptureCoreX**: Main engine for managing video capture, rendering, and TS output.
2. **VideoView**: UI component for real-time video rendering during capture.
3. **DeviceEnumerator**: Class for discovering video/audio devices.
4. **VideoCaptureDeviceSourceSettings**: Configuration for camera input when capturing MPEG-TS.
5. **AudioRendererSettings**: Configuration for audio playback with AAC support.
6. **MPEGTSOutput**: Configuration specifically for MPEG-TS file output.

### Implementation Example

Here's a complete C# implementation to capture and record MPEG-TS files:

```csharp
// Class instance for video capture engine
VideoCaptureCoreX videoCapture;

private async Task StartCaptureAsync()
{
    // Initialize the VisioForge SDK
    await VisioForgeX.InitSDKAsync();

    // Create VideoCaptureCoreX instance and associate with UI VideoView control
    videoCapture = new VideoCaptureCoreX(videoView: VideoView1);

    // Get list of available video capture devices
    var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();

    // Initialize video source settings
    VideoCaptureDeviceSourceSettings videoSourceSettings = null;

    // Get first available video capture device
    var videoDevice = videoSources[0];

    // Try to get HD resolution and frame rate capabilities from device
    var videoFormat = videoDevice.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    if (videoFormat != null)
    {
        // Configure video source with HD format
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(videoDevice)
        {
            Format = videoFormat.ToFormat()
        };

        // Set capture frame rate
        videoSourceSettings.Format.FrameRate = frameRate;
    }

    // Configure video capture device with settings
    videoCapture.Video_Source = videoSourceSettings;

    // Configure audio capture (microphone)

    // Initialize audio source settings
    IVideoCaptureBaseAudioSourceSettings audioSourceSettings = null;

    // Get available audio capture devices using DirectSound API
    var audioApi = AudioCaptureDeviceAPI.DirectSound;
    var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(audioApi);

    // Get first available audio capture device
    var audioDevice = audioDevices[0];
    if (audioDevice != null)
    {
        // Get default audio format supported by device
        var audioFormat = audioDevice.GetDefaultFormat();
        if (audioFormat != null)
        {
            // Configure audio source with default format
            audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);
        }
    }

    // Configure audio capture device with settings
    videoCapture.Audio_Source = audioSourceSettings;

    // Configure audio playback device
    // Get first available audio output device
    var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync())[0];

    // Configure audio renderer for playback through selected device
    videoCapture.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

    // Enable audio monitoring and recording
    videoCapture.Audio_Play = true;    // Enable real-time audio monitoring
    videoCapture.Audio_Record = true;   // Enable audio recording to output file

    // Configure MPEG Transport Stream output
    var mpegtsOutput = new MPEGTSOutput("output.ts");

    // Configure video encoder with hardware acceleration if available
    if (NVENCH264EncoderSettings.IsAvailable())
    {
        // Use NVIDIA hardware encoder
        mpegtsOutput.Video = new NVENCH264EncoderSettings();
    }
    else if (QSVH264EncoderSettings.IsAvailable())
    {
        // Use Intel Quick Sync hardware encoder
        mpegtsOutput.Video = new QSVH264EncoderSettings();
    }
    else if (AMFH264EncoderSettings.IsAvailable())
    {
        // Use AMD hardware encoder
        mpegtsOutput.Video = new AMFH264EncoderSettings();
    }
    else
    {
        // Fallback to software encoder
        mpegtsOutput.Video = new OpenH264EncoderSettings();
    }

    // Configure audio encoder for MPEG-TS output
    // mpegtsOutput.Audio = ...

    // Add MPEG-TS output to capture pipeline
    // autostart: true means output starts automatically with capture
    videoCapture.Outputs_Add(mpegtsOutput, autostart: true);

    // Start the capture process
    await videoCapture.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Stop all capture and encoding
    await videoCapture.StopAsync();

    // Clean up resources
    await videoCapture.DisposeAsync();
}
```

### VideoCaptureCoreX Advanced Features for MPEG-TS Recording

1. **Hardware Acceleration**: Support for NVIDIA (NVENC), Intel (QSV), and AMD (AMF) hardware encoding.
2. **Format Selection**: The SDK provides access to the native camera formats and frame rates.
3. **Audio Configuration**: Provides volume control and format selection.
4. **Multiple Outputs**: Ability to add multiple output formats simultaneously.

## Media Blocks SDK .NET (MediaBlocksPipeline) - Capture TS in C#

The MediaBlocksPipeline engine in Media Blocks SDK .Net takes a different architectural approach, focusing on a modular block-based system where each component (block) has specific responsibilities in the media processing pipeline.

### Core Blocks

1. **MediaBlocksPipeline**: The main container and controller for the media blocks pipeline.
2. **SystemVideoSourceBlock**: Captures video from webcams.
3. **SystemAudioSourceBlock**: Captures audio from microphones.
4. **VideoRendererBlock**: Renders the video to a VideoView control.
5. **AudioRendererBlock**: Handles audio playback.
6. **TeeBlock**: Splits media streams for simultaneous processing (e.g., display and encoding).
7. **H264EncoderBlock**: Encodes video using H.264.
8. **AACEncoderBlock**: Encodes audio using AAC.
9. **MPEGTSSinkBlock**: Saves encoded streams to an MPEG-TS file.

### Implementation Example

Here's how to implement advanced capture of TS files in C#:

```csharp
// Pipeline instance
MediaBlocksPipeline pipeline;

private async Task StartCaptureAsync()
{
    // Initialize the SDK
    await VisioForgeX.InitSDKAsync();

    // Create new pipeline instance
    pipeline = new MediaBlocksPipeline();

    // Get first available video device and configure HD format
    var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var formatItem = device.GetHDVideoFormatAndFrameRate(out VideoFrameRate frameRate);
    var videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
    {
        Format = formatItem.ToFormat()
    };
    videoSourceSettings.Format.FrameRate = frameRate;

    // Create video source block with configured settings
    var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

    // Get first available audio device and configure default format
    var audioDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
    var audioFormat = audioDevice.GetDefaultFormat();
    var audioSourceSettings = audioDevice.CreateSourceSettings(audioFormat);
    var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

    // Create video renderer block and connect to UI VideoView control
    var videoRenderer = new VideoRendererBlock(pipeline, videoView: VideoView1) { IsSync = false };

    // Create audio renderer block for playback
    var audioRenderer = new AudioRendererBlock() { IsSync = false };

    // Note: IsSync is false to maximize encoding performance

    // Create video and audio tees  
    var videoTee = new TeeBlock(2, MediaBlockPadMediaType.Video);
    var audioTee = new TeeBlock(2, MediaBlockPadMediaType.Audio);

    // Create MPEG-TS muxer
    var muxer = new MPEGTSSinkBlock(new MPEGTSSinkSettings("output.ts"));

    // Create video and audio encoders with hardware acceleration if available
    var videoEncoder = new H264EncoderBlock();
    var audioEncoder = new AACEncoderBlock();

    // Connect video processing blocks:
    // Source -> Tee -> Renderer (preview) and Encoder -> Muxer
    pipeline.Connect(videoSource.Output, videoTee.Input);
    pipeline.Connect(videoTee.Outputs[0], videoRenderer.Input);
    pipeline.Connect(videoTee.Outputs[1], videoEncoder.Input);
    pipeline.Connect(videoEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Video));

    // Connect audio processing blocks:
    // Source -> Tee -> Renderer (playback) and Encoder -> Muxer
    pipeline.Connect(audioSource.Output, audioTee.Input);
    pipeline.Connect(audioTee.Outputs[0], audioRenderer.Input);
    pipeline.Connect(audioTee.Outputs[1], audioEncoder.Input);
    pipeline.Connect(audioEncoder.Output, (muxer as IMediaBlockDynamicInputs).CreateNewInput(MediaBlockPadMediaType.Audio));

    // Start the pipeline processing
    await pipeline.StartAsync();
}

private async Task StopCaptureAsync()
{
    // Stop all pipeline processing
    await pipeline.StopAsync();

    // Clean up resources
    await pipeline.DisposeAsync();
    pipeline = null;
}
```

### MediaBlocksPipeline Advanced Features

1. **Fine-Grained Control**: Direct control over each processing step in the pipeline.
2. **Dynamic Pipeline Construction**: Ability to create complex processing pipelines by connecting blocks.
3. **Multiple Processing Paths**: Using TeeBlock to split streams for different processing paths.
4. **Custom Blocks**: Ability to create and integrate custom processing blocks.
5. **Granular Error Handling**: Error events at each block level.

## TS Output Configuration with AAC Audio

Both SDKs provide robust support for MPEG-TS output, which is particularly useful for broadcasting and streaming applications due to its error resilience and low latency characteristics.

Read more about video and audio encoders available for TS capture in .NET:

- [H264 encoders](../video-encoders/h264.md)
- [HEVC encoders](../video-encoders/hevc.md)
- [AAC encoders](../audio-encoders/aac.md)
- [MP3 encoders](../audio-encoders/mp3.md)
- [MPEG-TS output](../output-formats/mpegts.md)

## Cross-Platform Considerations

Both SDKs offer cross-platform capabilities, but with different approaches:

1. **VideoCaptureCoreX**: Provides a unified API across platforms with platform-specific implementations.
2. **MediaBlocksPipeline**: Uses a consistent block-based architecture across platforms, with blocks handling platform differences internally.

## Sample applications

- [VideoCaptureCoreX Sample Application](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WPF/CSharp/Simple%20Video%20Capture)
- [MediaBlocksPipeline Sample Application](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

## Conclusion: Choosing the Right SDK for C# MPEG-TS Capture

VisioForge offers two powerful solutions for recording MPEG-TS files in C# and .NET:

- **VideoCaptureCoreX** provides a streamlined API for quick implementation of MPEG-TS capture in C#, ideal for projects where ease of use is essential.

- **MediaBlocksPipeline** offers maximum flexibility for complex MPEG-TS recording and editing scenarios in .NET through its modular block architecture.

Both SDKs excel at capturing video from cameras and audio from microphones, with comprehensive support for MPEG-TS output, making them valuable tools for developing a wide range of multimedia applications.

Choose VideoCaptureCoreX for rapid implementation of standard TS capture scenarios, or MediaBlocksPipeline for advanced editing and custom processing workflows with TS files in your .NET applications.
