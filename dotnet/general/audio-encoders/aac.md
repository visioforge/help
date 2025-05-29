---
title: AAC Audio Encoder Implementation Guide
description: Learn how to implement AAC audio encoding in .NET applications with multiple encoder types, bitrate configurations, and cross-platform support. Includes code examples and best practices for developers.
sidebar_label: AAC (M4A)

---

# AAC encoder and M4A output

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

The VisioForge SDK provides several AAC encoder implementations, each with unique characteristics and use cases.

## What is M4A Output?

M4A is a file format used for storing audio data encoded with the Advanced Audio Coding (AAC) codec. VisioForge .Net SDKs provide robust support for creating high-quality M4A audio files through their dedicated M4AOutput class. This format is widely used for digital audio distribution due to its excellent compression efficiency and sound quality.

## Cross-platform M4A (AAC) output

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

The cross-platform capable SDKs (VideoCaptureCoreX, VideoEditCoreX, MediaBlocksPipeline) allow you to utilize several AAC encoder implementations via `M4AOutput`. This guide focuses on three main approaches using dedicated settings objects:

1. [AVENC AAC Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.AVENCAACEncoderSettings.html) - A feature-rich, cross-platform encoder.
2. [VO-AAC Encoder](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.VOAACEncoderSettings.html) - A streamlined, cross-platform encoder.
3. Media Foundation AAC Encoder - A Windows-specific system encoder, accessible on Windows platforms via `MFAACEncoderSettings`.

### AVENC AAC Encoder

The AVENC AAC Encoder offers the most comprehensive configuration options for audio encoding. It provides advanced settings for stereo coding, prediction, and noise shaping.

#### Key Features

- Multiple coder strategies
- Configurable stereo coding
- Advanced noise and prediction techniques

#### Coder Strategies

The AVENC AAC Encoder supports three coder strategies:

- `ANMR`: Advanced noise modeling and reduction method
- `TwoLoop`: Two-loop searching method for optimization
- `Fast`: Default fast search algorithm (recommended for most use cases)

#### Sample Configuration

```csharp
var aacSettings = new AVENCAACEncoderSettings
{
    Coder = AVENCAACEncoderCoder.Fast,
    Bitrate = 192,
    IntensityStereo = true,
    ForceMS = true,
    TNS = true
};
```

#### Supported Parameters

- **Bitrates**: 0, 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Sample Rates**: 7350 to 96000 Hz
- **Channels**: 1 to 6 channels

### VO-AAC Encoder

The VO-AAC Encoder is a more streamlined encoder with simpler configuration options.

#### Key Features

- Simplified configuration
- Straightforward bitrate and sample rate controls
- Limited to stereo audio

#### Sample Configuration

```csharp
var aacSettings = new VOAACEncoderSettings
{
    Bitrate = 128
};
```

#### Supported Parameters

- **Bitrates**: 32, 64, 96, 128, 160, 192, 224, 256, 320 kbps
- **Sample Rates**: 8000 to 96000 Hz
- **Channels**: 1-2 channels

### Media Foundation AAC Encoder (Windows Only)

This encoder is specific to Windows platforms and offers a limited but performance-optimized encoding solution.

#### Key Features

- Windows-specific implementation
- Predefined bitrate options
- Limited sample rate support

#### Supported Parameters

- **Bitrates**: 0 (Auto), 96, 128, 160, 192, 576, 768, 960, 1152 kbps
- **Sample Rates**: 44100, 48000 Hz
- **Channels**: 1, 2, 6 channels

### Encoder Availability and Selection

Each encoder provides a static `IsAvailable()` method to check if the encoder can be used in the current environment. This is useful for runtime compatibility checks.

```csharp
if (AVENCAACEncoderSettings.IsAvailable())
{
    // Use AVENC AAC Encoder
}
else if (VOAACEncoderSettings.IsAvailable())
{
    // Fallback to VO-AAC Encoder
}
```

### Getting Started with M4AOutput

The cross-platform implementation uses the [M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.Output.M4AOutput.html) class as the foundation for M4A file creation. To begin using this feature, initialize the class with your desired output filename:

```csharp
var output = new M4AOutput("output.m4a");
```

### Switching Between Encoders

The default encoder selection is platform-dependent:

- Windows environments: MF AAC
- Other platforms: VO-AAC

You can override this default selection by explicitly setting the `Audio` property:

```csharp
// For VO-AAC encoder
output.Audio = new VOAACEncoderSettings();

// For AVENC AAC encoder
output.Audio = new AVENCAACEncoderSettings();

// For MF AAC encoder (Windows only)
#if NET_WINDOWS
output.Audio = new MFAACEncoderSettings();
#endif
```

### Configuring MP4 Sink Settings

Since M4A files are based on the MP4 container format, you can adjust various output parameters through the `Sink` property:

```csharp
// Change the output filename
output.Sink.Filename = "new_output.m4a";
```

### Advanced Audio Processing

For workflows requiring specialized audio processing, the M4AOutput class supports custom audio processors:

```csharp
// Implement your custom audio processing logic
output.CustomAudioProcessor = new MyCustomAudioProcessor(); 
```

### Key Methods for File Management

The M4AOutput class provides several methods for handling files and retrieving encoder information:

```csharp
// Get current output filename
string currentFile = output.GetFilename();

// Update the output filename
output.SetFilename("updated_file.m4a");

// Retrieve available audio encoders
var audioEncoders = output.GetAudioEncoders();
```

### Using M4A Output in Different SDKs

Each VisioForge SDK has a slightly different approach to implementing M4A output:

#### With Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

#### With Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

#### With Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var sinkSettings = new MP4SinkSettings("output.m4a");
var m4aOutput = new M4AOutputBlock(sinkSettings, aac);
```

### Rate Control Considerations

1. **AVENC AAC Encoder**:
   - Most flexible rate control
   - Supports constant bitrate (CBR)
   - Multiple encoding strategies affect quality and performance

2. **VO-AAC Encoder**:
   - Simple constant bitrate control
   - Recommend for straightforward encoding needs
   - Limited advanced configuration

3. **Media Foundation Encoder**:
   - Limited to predefined bitrates
   - Good for quick Windows-based encoding
   - Auto bitrate option available

### Recommendations

- For advanced audio encoding with maximum control, use AVENC AAC Encoder
- For simple, cross-platform encoding, use VO-AAC Encoder
- For Windows-specific, optimized encoding, use Media Foundation Encoder

### Performance and Quality Considerations

- **Bitrate vs. Quality vs. File Size**: Higher bitrates generally result in better audio quality but also lead to larger file sizes. Experiment with different bitrates to find the optimal balance for your specific content and distribution needs.
- **Sample Rate Matching**: Always try to choose sample rates that match your source audio. This avoids unnecessary resampling, which can potentially degrade audio quality.
- **Encoder Characteristics**:
  - `AVENC AAC Encoder`: Offers the most extensive configuration options, allowing for fine-grained control over quality and performance. Ideal for advanced use cases.
  - `VO-AAC Encoder`: Provides a good balance of simplicity, cross-platform compatibility, and quality. A solid choice for many common scenarios.
  - `Media Foundation AAC Encoder`: Leverages built-in Windows audio processing capabilities. It can be efficient on Windows but offers less configuration flexibility than AVENC.
- **Channel Configuration (Mono vs. Stereo)**:
  - For voice-only content, using mono encoding (1 channel) can significantly reduce file size without a noticeable loss in quality for speech. Check if your chosen encoder settings (e.g., `AVENCAACEncoderSettings.Channels`) allow explicit channel configuration.
  - For music and rich audio environments, stereo (2 channels) is generally preferred.
- **Content-Specific Bitrate Ranges**: While higher is often better, the "best" bitrate depends on the audio content:
  - *Speech/Voice:* 64-96 kbps can be adequate.
  - *General Music:* 128-192 kbps is a common target for good quality.
  - *High-Fidelity Audio:* 256-320 kbps or higher might be used when pristine quality is critical.
    These are guidelines; always test with your specific audio.
- **Target Audience and Platform**: Consider who will be listening and on what devices. For example, if the audio is primarily for web streaming to mobile devices, extremely high bitrates might lead to buffering issues or unnecessary data consumption. Tailor your encoder choice and settings accordingly.

### Sample Code

- Check the [MP4 output](../output-formats/mp4.md) guide for sample code.
- Check the [AAC encoder block](../../mediablocks/AudioEncoders/index.md) for sample code.

## Windows-only AAC output

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

[M4AOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.Output.M4AOutput.html) is the primary class for configuring M4A (AAC) output settings. It implements both `IVideoEditBaseOutput` and `IVideoCaptureBaseOutput` interfaces.

### Properties

| Property | Type | Description | Default Value |
|----------|------|-------------|---------------|
| Version | AACVersion | Specifies the AAC version (MPEG-2 or MPEG-4) | MPEG4 |
| Object | AACObject | Defines the AAC object type | Low |
| Output | AACOutput | Sets the AAC output mode | RAW |
| Bitrate | int | Specifies the AAC bitrate in kbps | 128 |

### Methods

#### `GetInternalTypeVC()`

- Returns: `VideoCaptureOutputFormat.M4A`
- Purpose: Gets the internal output format for video capture

#### `GetInternalTypeVE()`

- Returns: `VideoEditOutputFormat.M4A`
- Purpose: Gets the internal output format for video editing

#### `Save()`

- Returns: JSON string representation of the M4AOutput object
- Purpose: Serializes the current configuration to JSON

#### `Load(string json)`

- Parameters: JSON string containing M4AOutput configuration
- Returns: New M4AOutput instance
- Purpose: Creates a new M4AOutput instance from JSON configuration

### Supporting Enums

#### AACVersion

Defines the version of AAC to be used:

| Value | Description |
|-------|-------------|
| MPEG4 | MPEG-4 AAC (default) |
| MPEG2 | MPEG-2 AAC |

#### AACObject

Specifies the AAC encoder stream object type:

| Value | Description |
|-------|-------------|
| Undefined | Not to be used |
| Main | Main profile |
| Low | Low Complexity profile (default) |
| SSR | Scalable Sample Rate profile |
| LTP | Long Term Prediction profile |

#### AACOutput

Determines the AAC encoder stream output type:

| Value | Description |
|-------|-------------|
| RAW | Raw AAC stream (default) |
| ADTS | Audio Data Transport Stream format |

### Usage Example

```csharp
// Create new M4A output configuration
var core = new VideoCaptureCore();
core.Mode = VideoCaptureMode.VideoCapture;
core.Output_Filename = "output.m4a";

var output = new VisioForge.Core.Types.Output.M4AOutput
{
    Bitrate = 192,
    Version = AACVersion.MPEG4,
    Object = AACObject.Low,
    Output = AACOutput.ADTS
};

core.Output_Format = output; // core is an instance of VideoCaptureCore or VideoEditCore
```

### Selecting the Right Bitrate

The optimal bitrate depends on your content type and quality requirements:

- **64-96 kbps**: Suitable for voice recordings and speech content
- **128-192 kbps**: Recommended for general music and audio content
- **256-320 kbps**: Ideal for high-fidelity music where quality is paramount

### Choosing the Appropriate Profile

- Use `AACObject.Low` for most applications as it provides an excellent balance between quality and encoding efficiency
- Reserve `AACObject.Main` for specialized use cases requiring maximum quality
- Avoid `AACObject.Undefined` as it isn't a valid encoding option

### Container Format Selection

- `AACOutput.ADTS` provides better compatibility with various players and devices
- `AACOutput.RAW` is preferable when the AAC stream will be embedded within another container format
