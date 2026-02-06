---
title: Record, Capture & Edit MP3 Audio in C#
description: Record, capture, and edit MP3 audio in C# with VisioForge .NET SDK using LAME encoder for high-quality audio processing.
---

# Mastering MP3 Audio: Record, Capture & Edit in C# and .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The VisioForge SDK empowers developers to seamlessly record, capture, and edit MP3 audio within C# applications. This guide explores how to leverage our robust .NET SDK for high-quality MP3 audio processing. Whether you need to capture media streams, record MP3 files, or edit audio waveforms, our C# media toolkit provides comprehensive tools using the LAME library. MP3, a widely adopted lossy audio compression format, is ideal for audio streaming and efficient storage.

You can utilize the MP3 encoder to integrate audio capture and recording functionalities into various container formats such as MP4, AVI, and MKV, enhancing your audio capture projects. Our SDK works seamlessly with Visual Studio for a smooth development experience.

SDK contains MP3 audio encoder that can be used to encode audio streams to MP3 format using the LAME library. MP3 is a lossy audio compression format that is widely used in audio streaming and storage.

You can use MP3 encode to encode audio in MP4, AVI, MKV, and other containers.

## Cross-platform MP3 Audio Capture and Recording

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

The [MP3EncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.MP3EncoderSettings.html) class provides developers with a streamlined approach to configure MP3 encoding for C# audio capture projects. This cross-platform solution supports various rate controls and quality settings, making it ideal for record .NET MP3 applications across different operating systems.

### Supported Formats and Specifications for C# MP3 Recording

- Input Format: S16LE (Signed 16-bit Little Endian)
- Sample Rates: 8000, 11025, 12000, 16000, 22050, 24000, 32000, 44100, 48000 Hz
- Channels: Mono (1) or Stereo (2)

### Rate Control Modes

The encoder supports three rate control modes:

1. **CBR (Constant Bit Rate)**
   - Fixed bitrate throughout the entire encoding process
   - Supported bitrates: 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 Kbit/s
   - Best for streaming MP3 and when consistent file size is important

2. **ABR (Average Bit Rate)**
   - Maintains an average bitrate while allowing some variation
   - More efficient than CBR while still maintaining predictable file sizes
   - Useful for streaming services that need approximate file size estimates

3. **Quality-based VBR**
   - Variable Bit Rate based on sound complexity
   - Quality setting ranges from 0 (best) to 10
   - Most efficient for storage and best quality-to-size ratio  

### C# MP3 Encoding Examples

Create basic MP3 encoder settings with CBR.

```csharp
// Create basic MP3 encoder settings using Constant Bit Rate mode
var mp3Settings = new MP3EncoderSettings
{
    // Set to Constant Bit Rate - provides consistent file size and streaming reliability
    RateControl = MP3EncoderRateControl.CBR,
    // 192 kbps offers good quality for most music content while keeping file size reasonable
    Bitrate = 192,
    // Standard quality offers a good balance between encoding speed and output quality
    EncodingEngineQuality = MP3EncodingQuality.Standard,
    // Keep stereo channels (false) - set to true if you want to convert to mono
    ForceMono = false
};
```

Quality-based VBR configuration for high-quality .NET MP3 editing.

```csharp
// Configure MP3 encoder with Variable Bit Rate for optimal quality-to-size ratio
var vbrSettings = new MP3EncoderSettings
{
    // Quality-based VBR adjusts bitrate dynamically based on audio complexity
    RateControl = MP3EncoderRateControl.Quality,
    // Quality scale: 0 (best) to 10 (worst) - 2.0 provides excellent quality with reasonable file size
    Quality = 2.0f,
    // High quality encoding uses more CPU but produces better results
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

Add the MP3 output to capture C# MP3 audio with the Video Capture SDK:

The [MP3Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) class implements multiple interfaces:

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- IOutputAudioProcessor

```csharp
// Create a Video Capture SDK core instance for recording
var core = new VideoCaptureCoreX();

// Initialize MP3 output with target filename
var mp3Output = new MP3Output("output.mp3");

// Configure audio encoding settings
mp3Output.Audio.RateControl = MP3EncoderRateControl.CBR;  // Use Constant Bit Rate for reliable streaming
mp3Output.Audio.Bitrate = 128;  // 128 kbps is suitable for general audio recording

// Add the MP3 output to the capture pipeline
core.Outputs_Add(mp3Output, true);
```

Set the output format for the Video Edit SDK core instance:

```csharp
// Initialize Video Edit SDK for processing existing media
var core = new VideoEditCoreX();

// Create MP3 output with target filename
var mp3Output = new MP3Output("output.mp3");

// Configure Variable Bit Rate encoding for better quality-to-size ratio
mp3Output.Audio.RateControl = MP3EncoderRateControl.Quality;
mp3Output.Audio.Quality = 5.0f;  // Middle quality setting (0-10 scale) - good balance of quality and size

// Set as the primary output format for the editor
core.Output_Format = mp3Output;
```

### Initialization

To create a new MP3Output instance, you need to provide the output filename:

```csharp
// Initialize MP3 output with destination filename
var mp3Output = new MP3Output("output.mp3");
```

### Audio Settings

The `Audio` property provides access to MP3 encoder settings:

```csharp
// Create default MP3 encoder settings object
mp3Output.Audio = new MP3EncoderSettings();
// Additional configuration can be applied to mp3Output.Audio properties
```

### Custom Audio Processing

You can set a custom audio processor using the `CustomAudioProcessor` property to handle waveform manipulations:

```csharp
// Attach a custom audio processor for advanced audio manipulation
mp3Output.CustomAudioProcessor = new MediaBlock();
// The MediaBlock can be configured for effects, filtering, or other audio processing
```

### Filename Operations

There are multiple ways to work with the output filename:

```csharp
// Retrieve the current output filename
string currentFile = mp3Output.GetFilename();

// Change the output destination
mp3Output.SetFilename("newoutput.mp3");

// Alternative way to set the filename via property
mp3Output.Filename = "another.mp3";
```

### Audio Encoders

The MP3Output class supports MP3 encoding exclusively. You can verify the available encoders:

```csharp
// Get information about available audio encoders
var audioEncoders = mp3Output.GetAudioEncoders();
// Returns a list of tuples containing encoder names and their setting types
// For MP3Output, this will contain a single entry for MP3
```

### MP3OutputBlock class

The [MP3OutputBlock](../../mediablocks/AudioEncoders/index.md) class provides a more flexible way to configure MP3 encoding.

Create a Media Blocks MP3 output instance:

```csharp
// Create MP3 encoder settings with desired configuration
var mp3Settings = new MP3EncoderSettings();

// Initialize MP3 output block with destination file and settings
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
```

Check if MP3 encoding is available.

```cs
// Check if MP3 encoding is available on the current system
if (!MP3EncoderSettings.IsAvailable())
{
   // Handle case where MP3 encoding is not available
   // This might occur if LAME or other required libraries are missing
}
```

### Encoding Quality Levels

The encoder supports three quality presets that affect the encoding speed and CPU usage:

- `Fast`: Quickest encoding, lower CPU usage
- `Standard`: Balanced speed and quality (default)
- `High`: Best quality, higher CPU usage

### Common Scenarios

#### High-Quality Music Capture in C#

```csharp
// Configure settings for high-quality music recording
var highQualitySettings = new MP3EncoderSettings
{
    // Use quality-based Variable Bit Rate for optimal audio fidelity
    RateControl = MP3EncoderRateControl.Quality,
    // Highest quality setting (0.0f) for maximum audio fidelity
    Quality = 0.0f,
    // Use high-quality encoding algorithm (more CPU intensive but better results)
    EncodingEngineQuality = MP3EncodingQuality.High
};
```

#### Streaming Audio

```csharp
// Configure settings optimized for audio streaming applications
var streamingSettings = new MP3EncoderSettings
{
    // Use Constant Bit Rate for predictable streaming performance
    RateControl = MP3EncoderRateControl.CBR,
    // 128 kbps provides good quality for most content while being bandwidth-friendly
    Bitrate = 128,
    // Fast encoding reduces CPU usage, important for real-time streaming
    EncodingEngineQuality = MP3EncodingQuality.Fast
};
```

## Windows-only MP3 output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The [MP3 file output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP3Output.html) class provides advanced configuration options for MP3 encoding in C# audio video capture and editing scenarios.

### Key Features

- Flexible channel mode selection
- VBR and CBR encoding support for optimal .NET MP3 recording
- Advanced encoding parameters for professional audio applications
- Quality control settings for perfect C# MP3 editing results

### Basic Configuration

#### CBR_Bitrate

Controls the Constant Bit Rate (CBR) setting for MP3 encoding.

- For MPEG-1 (32, 44.1, 48 kHz): Valid values are 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320 kbps
- For MPEG-2 (16, 22.05, 24 kHz): Valid values are 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160 kbps
- Default values: 128 kbps (MPEG-1) or 64 kbps (MPEG-2)

#### SampleRate

Specifies the audio sampling frequency in Hz. Common values are:

- 44100 Hz (CD quality, default)
- 48000 Hz (professional audio)
- 32000 Hz (broadcast)
- 22050 Hz (lower quality)
- 16000 Hz (voice)

#### ChannelsMode

Determines how audio channels are encoded. Options include:

1. StandardStereo: Independent channel encoding with dynamic bit allocation
2. JointStereo: Exploits correlation between channels using mid/side encoding
3. DualStereo: Independent encoding with fixed 50/50 bit allocation (ideal for dual language)
4. Mono: Single channel output (downmixes stereo input)

### Variable Bit Rate (VBR) Settings

#### VBR_Mode

Enables Variable Bit Rate encoding when set to true (default). VBR allows the encoder to adjust bitrate based on audio complexity.

#### VBR_MinBitrate

Sets the minimum allowed bitrate for VBR encoding (default: 96 kbps).

#### VBR_MaxBitrate

Sets the maximum allowed bitrate for VBR encoding (default: 192 kbps).

#### VBR_Quality

Controls VBR encoding quality (0-9):

- Lower values (0-4): Higher quality, slower encoding
- Middle values (5-6): Balanced quality and speed
- Higher values (7-9): Lower quality, faster encoding

### Quality and Performance

#### EncodingQuality

Determines the algorithmic quality of encoding (0-9):

- 0-1: Best quality, slowest encoding
- 2: Recommended for high quality
- 5: Default, good balance of speed and quality
- 7: Fast encoding with acceptable quality
- 9: Fastest encoding, lowest quality

### Special Features

#### ForceMono

When enabled, automatically downmixes multi-channel audio to mono.

#### VoiceEncodingMode

Experimental mode optimized for voice content.

#### KeepAllFrequencies

Disables automatic frequency filtering, preserving all frequencies at the cost of efficiency.

#### DisableShortBlocks

Forces use of long blocks only, which may improve quality at very low bitrates but can cause pre-echo artifacts.

### MP3 Frame Flags

#### Copyright

Sets the copyright bit in MP3 frames.

#### Original

Marks the stream as original content.

#### CRCProtected

Enables CRC error detection at the cost of 16 bits per frame.

#### EnableXingVBRTag

Adds VBR information headers for better player compatibility.

#### StrictISOCompliance

Enforces strict ISO MP3 standard compliance.

### Example MP3 Recording and Editing Configurations

Basic settings for C# MP3 capture applications.

```csharp
// Configure basic MP3 output with standard settings
var mp3Output = new MP3Output
{
    // 192 kbps provides good quality for most music content
    CBR_Bitrate = 192,
    // CD-quality sample rate
    SampleRate = 44100,
    // Joint stereo mode provides better compression for most stereo content
    ChannelsMode = MP3ChannelsMode.JointStereo,
};

// Set as the output format for capture or editing
core.Output_Format = mp3Output; // Core is VideoCaptureCore or VideoEditCore
```

VBR configuration.

```csharp
// Configure MP3 output with Variable Bit Rate for better quality/size balance
var mp3Output = new MP3Output
{    
    // Enable Variable Bit Rate encoding
    VBR_Mode = true,
    // Set minimum bitrate floor to ensure acceptable quality
    VBR_MinBitrate = 96,
    // Limit maximum bitrate to control file size
    VBR_MaxBitrate = 192,
    // Quality level 6 provides a good balance between quality and file size
    VBR_Quality = 6,
};

// Set as the output format for capture or editing
core.Output_Format = mp3Output; // Core is VideoCaptureCore or VideoEditCore
```

#### Basic Stereo MP3 Encoding

```csharp
// Configure standard stereo MP3 encoding with fixed bitrate
var mp3Output = new MP3Output
{
    // 192 kbps provides good quality for most music while keeping file size reasonable
    CBR_Bitrate = 192,
    // Standard stereo mode encodes left and right channels independently
    ChannelsMode = MP3ChannelsMode.StandardStereo,
    // CD-quality sample rate
    SampleRate = 44100,
    // Disable Variable Bit Rate to ensure consistent file size and playback
    VBR_Mode = false
};
```

#### Voice-Optimized Encoding

```csharp
// Configure MP3 settings optimized for voice recordings
var voiceMP3 = new MP3Output
{
    // Enable voice-optimized encoding algorithms
    VoiceEncodingMode = true,
    // Use mono for voice to reduce file size (most voice doesn't benefit from stereo)
    ChannelsMode = MP3ChannelsMode.Mono,
    // Lower sample rate is sufficient for voice content
    SampleRate = 22050,
    // Enable Variable Bit Rate for better quality/size ratio
    VBR_Mode = true,
    // Better quality setting for voice clarity while keeping file size reasonable
    VBR_Quality = 4
};
```

#### High-Quality Music Encoding

```csharp
// Configure high-quality MP3 settings for music archiving
var highQualityMP3 = new MP3Output
{
    // Enable Variable Bit Rate for optimal quality-to-size ratio
    VBR_Mode = true,
    // Set minimum bitrate to ensure good quality even in simple passages
    VBR_MinBitrate = 128,
    // Allow high bitrate for complex passages to preserve audio detail
    VBR_MaxBitrate = 320,
    // Use high quality setting (2) for excellent audio fidelity
    VBR_Quality = 2,
    // Set encoder algorithm to high quality mode
    EncodingQuality = 2,
    // Joint stereo provides better compression for most music content
    ChannelsMode = MP3ChannelsMode.JointStereo,
    // Professional audio sample rate captures full audible spectrum
    SampleRate = 48000,
    // Add VBR header for better player compatibility and seeking
    EnableXingVBRTag = true
};
```

### Advanced Settings

- **CRC Protection**: Adds error detection capability at the cost of 16 bits per frame
- **Short Blocks**: Can be disabled to potentially increase quality at very low bitrates
- **Frequency Range**: Option to keep all frequencies (disables automatic lowpass filtering)
- **Voice Mode**: Experimental mode optimized for voice content

### Best Practices

1. **Choosing Rate Control for Different Applications**
   - Use CBR for streaming and real-time C# MP3 capturing
   - Use Quality-based VBR for archival and highest quality .NET MP3 recording
   - Use ABR when you need a balance between consistent size and quality

2. **Quality Settings for Different Use Cases**
   - For archival: Use VBR with quality 0-2
   - For general C# audio video capture: VBR with quality 3-5 or CBR 192-256kbps
   - For voice recording in .NET: Consider using voice encoding mode with lower bitrates

3. **Channel Mode Selection**
   - Use Joint Stereo for most music content
   - Use Standard Stereo for critical listening and complex stereo mixes
   - Use Mono for voice recordings or when bandwidth is critical

4. **Performance Optimization**
   - Use Fast encoding quality for real-time applications
   - Use Standard quality for general purpose encoding
   - Use High quality only for archival purposes where encoding time is not critical

### Notes on Default Values

The class constructor sets these default values:

- CBR_Bitrate = 192 kbps
- VBR_MinBitrate = 96 kbps
- VBR_MaxBitrate = 192 kbps
- VBR_Quality = 6
- EncodingQuality = 6
- SampleRate = 44100 Hz
- ChannelsMode = MP3ChannelsMode.StandardStereo
- VBR_Mode = true
