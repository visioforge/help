---
title: Speex Audio Encoder Integration for .NET
description: Implement Speex speech compression in .NET with optimized voice encoding settings, quality controls, and cross-platform audio capture.
---

# Speex Audio Encoder for .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Speex

Speex is a patent-free audio codec specifically designed for speech encoding in .NET applications. Whether you need to capture, edit, or record audio in C#, Speex provides excellent compression while maintaining voice quality across various bitrates. VisioForge integrates this powerful encoder into its .NET SDKs, offering developers flexible configuration options for speech-based applications. The codec is particularly well-suited for C# developers looking to implement high-quality audio capture and recording features in their applications.

## Core Functionality

The Speex encoder in VisioForge SDKs supports:

- Multiple frequency bands for different quality levels
- Variable and fixed bitrate encoding
- Voice activity detection and silence compression
- Adjustable complexity and quality settings
- Cross-platform compatibility across Windows, macOS, and Linux
- Seamless integration with dotnet applications

## Cross-platform Implementation

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Encoder Modes

Speex offers four operation modes optimized for different frequency ranges:

| Mode | Value | Optimal Sample Rate |
|------|-------|-------------------|
| Auto | 0 | Automatic selection based on input |
| Ultra Wide Band | 1 | 32 kHz |
| Wide Band | 2 | 16 kHz |
| Narrow Band | 3 | 8 kHz |

The encoder automatically adjusts internal parameters based on the selected mode. For most speech applications, Wide Band (mode 2) offers an excellent balance between quality and bandwidth usage.

## Technical Specifications

### Supported Sample Rates

Speex works with three standard sampling frequencies:

- 8,000 Hz - Best for telephone-quality audio (Narrow Band)
- 16,000 Hz - Recommended for most voice applications (Wide Band)
- 32,000 Hz - Highest quality speech encoding (Ultra Wide Band)

### Channel Configuration

The encoder handles both:

- Mono (1 channel) - Ideal for speech recordings
- Stereo (2 channels) - For multi-speaker or immersive audio

## Rate Control Methods

### Quality-Based Encoding

For consistent perceptual quality, use the `Quality` parameter:

```csharp
var settings = new SpeexEncoderSettings {
    Quality = 8.0f, // Range from 0 (lowest) to 10 (highest)
    VBR = false     // Fixed quality mode
};
```

Higher quality values produce better audio at the expense of increased file size. Most speech applications work well with quality values between 5-8.

### Variable Bit Rate (VBR)

VBR dynamically adjusts the bitrate based on speech complexity:

```csharp
var settings = new SpeexEncoderSettings {
    VBR = true,
    Quality = 8.0f  // Target quality level
};
```

This approach typically saves bandwidth while maintaining consistent perceived quality, making it ideal for streaming applications.

### Average Bit Rate (ABR)

ABR maintains a target bitrate over time while allowing quality fluctuations:

```csharp
var settings = new SpeexEncoderSettings {
    ABR = 15.0f,   // Target bitrate in kbps
    VBR = true     // Required for ABR mode
};
```

This option works well when you need predictable file sizes or bandwidth usage.

### Fixed Bitrate Encoding

For consistent data rates throughout the encoding process:

```csharp
var settings = new SpeexEncoderSettings {
    Bitrate = 24.6f,  // Fixed rate in kbps
    VBR = false
};
```

Supported bitrates range from 2.15 kbps to 24.6 kbps:

- 2.15 kbps - Ultra-compressed speech (limited quality)
- 3.95 kbps - Low bandwidth voice
- 5.95 kbps - Basic speech clarity
- 8.00 kbps - Standard voice quality
- 11.0 kbps - Good speech reproduction
- 15.0 kbps - Near-transparent speech
- 18.2 kbps - High-quality voice
- 24.6 kbps - Maximum quality speech

## Voice Optimization Features

### Voice Activity Detection (VAD)

VAD identifies the presence of speech in audio signals:

```csharp
var settings = new SpeexEncoderSettings {
    VAD = true,    // Enable voice detection
    DTX = true     // Recommended with VAD
};
```

This feature improves bandwidth efficiency by focusing encoding resources on actual speech segments.

### Discontinuous Transmission (DTX)

DTX reduces data transmission during silence periods:

```csharp
var settings = new SpeexEncoderSettings {
    DTX = true     // Enable silence compression
};
```

For VoIP and real-time communications, enabling DTX can significantly reduce bandwidth requirements.

### Encoding Complexity

Control CPU usage versus encoding quality:

```csharp
var settings = new SpeexEncoderSettings {
    Complexity = 3  // Range: 1 (fastest) to 10 (highest quality)
};
```

Lower values prioritize speed and reduce CPU load, while higher values improve audio quality at the cost of performance.

## Implementation Examples

### Checking Encoder Availability

Always verify encoder availability before implementing Speex in your C# application:

```csharp
if (!SpeexEncoderSettings.IsAvailable())
{
    throw new InvalidOperationException("Speex encoder not available on this system.");
}
```

### Basic Configuration for Audio Capture

Here's how to set up basic Speex encoding for audio capture in dotnet:

```csharp
var encoderSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    Quality = 7.0f
};
```

### Optimized for Voice Recording

For voice recording applications in .NET, use these optimized settings:

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 4
};
```

### Highest Quality Audio Capture

For maximum quality audio capture in dotnet:

```csharp
var highQualitySettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.UltraWideBand,
    SampleRate = 32000,
    Channels = 2,
    Bitrate = 24.6f,
    Complexity = 8
};
```

## SDK Integration

### Video Capture SDK Integration

Learn how to capture audio using Speex in your C# application:

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Create a Video Capture SDK core instance
var core = new VideoCaptureCoreX();

// Set the audio input device, filter by API
var api = AudioCaptureDeviceAPI.DirectSound;
var audioInputDevice = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault(x => x.API == api);
if (audioInputDevice == null)
{
    MessageBox.Show("No audio input device found.");
    return;
}

var audioInput = new AudioCaptureDeviceSourceSettings(api, audioInputDevice, audioInputDevice.GetDefaultFormat());

core.Audio_Source = audioInput;

// Configure Speex settings
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput("output.spx", speexSettings);

// Add the Speex output
core.Outputs_Add(speexOutput, true);

// Set the audio record mode
core.Audio_Record = true;
core.Audio_Play = false;

// Start the capture
await core.StartAsync();

// Stop after 10 seconds
await Task.Delay(10000);

// Stop the capture
await core.StopAsync();
```

### Video Edit SDK Integration

Edit and process audio files using Speex in dotnet:

```csharp
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Create a Video Edit SDK core instance
var core = new VideoEditCoreX();

// Add the audio source file
var audioFile = new AudioFileSource(@"c:\samples\!audio.mp3");
VideoEdit1.Input_AddAudioFile(audioFile, null);

// Configure Speex settings
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    Quality = 7.0f
};

var speexOutput = new SpeexOutput(@"output.spx", speexSettings);

// Add the Speex output
core.Output_Format = speexOutput;

// Catch OnStop event
core.OnStop += (s, e) =>
{
    // Handle the stop event here
    MessageBox.Show("Editing complete.");
};

core.OnProgress += (s, e) =>
{
    // Handle progress updates here
    Debug.WriteLine($"Progress: {e.Progress}%");
};

core.OnError += (s, e) =>
{
    // Handle errors here
    Debug.WriteLine($"Error: {e.Message}");
};

// Start the editing
core.Start();
```

### Media Blocks SDK Integration

Process audio streams using Speex in your .NET application:

```csharp
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.AudioEncoders;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;

using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;

// Create a new pipeline
var pipeline = new MediaBlocksPipeline();

// Add universal source to read audio file
var sourceSettings = await UniversalSourceSettings.CreateAsync(@"c:\samples\!audio.mp3", renderVideo: false, renderAudio: true);
var source = new UniversalSourceBlock(sourceSettings);

// Add Speex output
var speexSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    DTX = true,
    VAD = true
};

var speexOutput = new OGGSpeexOutputBlock("output.spx", speexSettings);

// Connect
pipeline.Connect(source.AudioOutput, speexOutput.Input);

// Add OnStop event handler
pipeline.OnStop += (sender, e) =>
{
    // Do something when the pipeline stops
    MessageBox.Show("Conversion complete");
};

// Start
await pipeline.StartAsync();
```

## Performance Optimization

When implementing Speex encoding, consider these optimization strategies:

1. **Match sample rate to content** - Use Narrow Band (8 kHz) for telephone audio, Wide Band (16 kHz) for most voice applications, and Ultra Wide Band (32 kHz) only when maximum quality is required

2. **Enable VBR with VAD/DTX** for speech content - This combination provides optimal bandwidth efficiency for typical voice recordings

3. **Adjust complexity based on platform** - Mobile applications may benefit from lower complexity values (2-4), while desktop applications can use higher values (5-8)

4. **Use ABR for streaming** - Average Bit Rate provides predictable bandwidth usage while maintaining quality flexibility

5. **Test different quality settings** - Often a quality setting of 5-7 provides excellent results without excessive file size

## Use Cases

Speex encoding excels in these developer scenarios:

- VoIP applications and internet telephony
- Voice chat features in games and collaboration tools
- Podcast creation and distribution
- Speech recognition preprocessing
- Voice note applications
- Audio archiving of speech content

## Installation and Setup

To get started with Speex in your dotnet application, check the main installation guide [here](../../install/index.md).

## Common Use Cases

### Audio Capture and Recording

For streaming applications, use these optimized settings:

```csharp
var streamingSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.WideBand,
    SampleRate = 16000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 6.0f,
    Complexity = 3
};
```

### Voice Over IP Applications

For VoIP applications, prioritize low latency and bandwidth efficiency:

```csharp
var voipSettings = new SpeexEncoderSettings
{
    Mode = SpeexEncoderMode.NarrowBand,
    SampleRate = 8000,
    Channels = 1,
    VBR = true,
    VAD = true,
    DTX = true,
    Quality = 5.0f,
    Complexity = 2
};
```

## Licensing and Community

Speex is released under the BSD license, making it free for both commercial and non-commercial use. The codec is actively maintained by the open-source community, with regular updates and improvements.

## Frequently Asked Questions

### What is the best bitrate for voice recording?

For most voice applications, a bitrate between 8-15 kbps provides excellent quality while maintaining reasonable file sizes. Use VBR mode for optimal results.

### How does Speex compare to other codecs?

Speex offers superior speech quality compared to many other codecs at similar bitrates, especially for voice content. It's particularly effective for low-bitrate applications.

### Can I use Speex for music encoding?

While Speex can encode music, it's specifically optimized for speech. For music content, consider using other codecs like AAC or MP3.

## Conclusion

The VisioForge implementation of Speex provides .NET developers with a powerful tool for capturing, editing, and recording audio in C# applications. Whether you're building a new voice capture application or enhancing an existing one, Speex delivers exceptional results with minimal resource usage. The codec's flexibility and performance make it an excellent choice for any .NET developer working with audio processing.
