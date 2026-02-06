---
title: FLAC Audio Encoder Integration Guide
description: Implement FLAC lossless audio compression in .NET with quality settings, compression parameters, and high-quality audio processing.
---

# FLAC encoder and output

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The FLAC (Free Lossless Audio Codec) encoder provides high-quality lossless audio compression while preserving the original audio quality.

## Cross-platform FLAC output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

### Features

The FLAC encoder supports a wide range of audio configurations:

- Sample rates from 1 Hz to 655,350 Hz
- Up to 8 audio channels (mono to 7.1 surround)
- Lossless compression with adjustable quality settings
- Streamable output support
- Configurable block sizes and compression parameters

### Quality Settings

The encoder provides a quality parameter ranging from 0 to 9:

- 0: Fastest compression (lowest CPU usage)
- 1-7: Balanced compression settings
- 8: Highest compression (higher CPU usage)
- 9: Insane compression (extremely CPU intensive)

The default quality setting is 5, which offers a good balance between compression ratio and processing speed.

### Basic Settings

The cross-platform [FLACEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.FLACEncoderSettings.html) class offers advanced configuration options:

```csharp
// Create FLAC encoder settings with default quality
var flacSettings = new FLACEncoderSettings
{
    // Default compression level
    Quality = 5,        

    // Audio block size in samples
    BlockSize = 4608,              

    // Enable streaming support
    StreamableSubset = true,    

    // Enable stereo processing
    MidSideStereo = true          
};
```

### Advanced Compression Settings

```csharp
// Create FLAC encoder settings with advanced configuration
var advancedSettings = new FLACEncoderSettings
{
    // Linear Prediction settings
    // Maximum LPC order for prediction
    MaxLPCOrder = 8,               
    // Auto precision for coefficients
    QlpCoeffPrecision = 0,        
    
    // Residual coding settings
    MinResidualPartitionOrder = 3,
    MaxResidualPartitionOrder = 3,
    
    // Search optimization settings
    // Disable expensive coefficient search
    ExhaustiveModelSearch = false, 
    // Disable precision search
    QlpCoeffPrecSearch = false,    
    // Disable escape code search
    EscapeCoding = false          
};
```

### Sample Code

Add the FLAC output to the Video Capture SDK core instance:

```csharp
// Create a Video Capture SDK core instance
var core = new VideoCaptureCoreX();

// Create a FLAC output instance
var flacOutput = new FLACOutput("output.flac");

// Set the quality of the FLAC encoder
flacOutput.Audio.Quality = 5;

// Add the FLAC output
core.Outputs_Add(flacOutput, true);
```

Set the output format for the Video Edit SDK core instance:

```csharp
// Create a Video Edit SDK core instance
 var core = new VideoEditCoreX();

// Create a FLAC output instance
 var flacOutput = new FLACOutput("output.flac");

 // Set the quality 
 flacOutput.Audio.Quality = 5;

 // Set the output format
 core.Output_Format = flacOutput;
```

Create a Media Blocks FLAC output instance:

```csharp
// Create a FLAC encoder settings instance
var flacSettings = new FLACEncoderSettings();

// Create a FLAC output instance
var flacOutput = new FLACOutputBlock("output.flac", flacSettings);
```

### FLACOutput class

The `FLACOutput` class provides functionality for configuring FLAC (Free Lossless Audio Codec) output in the VisioForge SDKs.

```csharp
// Create a new FLAC output instance
var flacOutput = new FLACOutput("output.flac");

// Configure FLAC encoder settings
flacOutput.Audio.CompressionLevel = 5; // Example setting
```

#### Filename

- Set the output filename during initialization or using the property
- Can also be accessed/modified using `GetFilename()` and `SetFilename()` methods

```csharp
// Set during initialization
var flacOutput = new FLACOutput("audio_output.flac");
```

```csharp
// Or using the property
flacOutput.Filename = "new_output.flac";
```

#### Audio Settings

The `Audio` property provides access to FLAC-specific encoding settings through the `FLACEncoderSettings` class:

```csharp
flacOutput.Audio = new FLACEncoderSettings();
// Configure specific FLAC encoding parameters here
```

#### Custom Audio Processing

You can set a custom audio processor using the `CustomAudioProcessor` property:

```csharp
flacOutput.CustomAudioProcessor = new CustomMediaBlock();
```

#### Implementation Notes

- The class implements multiple interfaces:
  - `IVideoEditXBaseOutput`
  - `IVideoCaptureXBaseOutput`
  - `IOutputAudioProcessor`
  
- Only FLAC audio encoding is supported (no video encoding capabilities)
- Default FLAC encoder settings are automatically created during initialization

Media Blocks SDK contains a dedicated [FLAC encoder block](../../mediablocks/AudioEncoders/index.md).

### Performance Considerations

When configuring the FLAC encoder, consider these performance factors:

1. Higher quality settings (7-9) will significantly increase CPU usage
2. The `ExhaustiveModelSearch` option can greatly impact encoding speed
3. Larger block sizes may improve compression but increase memory usage
4. `StreamableSubset` should remain enabled unless you have specific requirements

### Compatibility

The encoder supports the following configurations:

- Audio channels: 1 to 8 channels
- Sample rates: 1 Hz to 655,350 Hz
- Bitrate: Variable (lossless compression)

### Error Handling

Always check for encoder availability before use:

```csharp
if (!FLACEncoderSettings.IsAvailable())
{
    // Handle unavailable encoder scenario
    Console.WriteLine("FLAC encoder is not available on this system");
    return;
}
```

### Best Practices

1. Start with the default quality setting (5) and adjust based on your needs
2. Enable `MidSideStereo` for stereo content to improve compression
3. Use `SeekPoints` for longer audio files to enable quick seeking
4. Keep `StreamableSubset` enabled unless you have specific requirements
5. Avoid using `ExhaustiveModelSearch` unless compression ratio is critical

## Windows-only FLAC output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The [FLACOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.FLACOutput.html) class provides Windows-only settings for the FLAC encoder. This class implements both `IVideoEditBaseOutput` and `IVideoCaptureBaseOutput` interfaces, making it suitable for both video editing and capture scenarios.

### Properties

#### Compression Level

- **Property**: `Level`
- **Type**: `int`
- **Range**: 0-8
- **Default**: 5
- **Description**: Controls the compression level, where 0 provides fastest compression and 8 provides highest compression.

#### Block Size

- **Property**: `BlockSize`
- **Type**: `int`
- **Default**: 4608
- **Valid Values**: For subset streams, must be one of:
  - 192, 256, 512, 576, 1024, 1152, 2048, 2304, 4096, 4608
  - 8192, 16384 (only if sample rate > 48kHz)
- **Description**: Specifies the block size in samples. The encoder uses the same block size for the entire stream.

#### LPC Order

- **Property**: `LPCOrder`
- **Type**: `int`
- **Default**: 8
- **Constraints**:
  - Must be ≤ 32
  - For subset streams at ≤ 48kHz, must be ≤ 12
- **Description**: Specifies the maximum Linear Predictive Coding order. Setting to 0 disables generic linear prediction and uses only fixed predictors, which is faster but typically results in 5-10% larger files.

#### Mid-Side Coding Options

##### Mid-Side Coding

- **Property**: `MidSideCoding`
- **Type**: `bool`
- **Default**: `false`
- **Description**: Enables mid-side coding for stereo streams. This typically increases compression by a few percent by encoding both stereo pair and mid-side versions of each block and selecting the smallest resulting frame.

##### Adaptive Mid-Side Coding

- **Property**: `AdaptiveMidSideCoding`
- **Type**: `bool`
- **Default**: `false`
- **Description**: Enables adaptive mid-side coding for stereo streams. This provides faster encoding than full mid-side coding but with slightly less compression by adaptively switching between independent and mid-side coding.

#### Rice Parameters

##### Rice Minimum

- **Property**: `RiceMin`
- **Type**: `int`
- **Default**: 3
- **Description**: Sets the minimum residual partition order. Works in conjunction with RiceMax to control how the residual signal is partitioned.

##### Rice Maximum

- **Property**: `RiceMax`
- **Type**: `int`
- **Default**: 3
- **Description**: Sets the maximum residual partition order. The residual is partitioned into 2^min to 2^max pieces, each with its own Rice parameter. Optimal settings typically depend on block size, with best results when blocksize/(2^n)=128.

#### Advanced Options

##### Exhaustive Model Search

- **Property**: `ExhaustiveModelSearch`
- **Type**: `bool`
- **Default**: `false`
- **Description**: Enables exhaustive model search for optimal encoding. When enabled, the encoder generates subframes for every order and uses the smallest, potentially improving compression by ~0.5% at the cost of significantly increased encoding time.

### Methods

#### Constructor

```csharp
public FLACOutput()
```

Initializes a new instance with default values:

- Level = 5
- RiceMin = 3
- RiceMax = 3
- LPCOrder = 8
- BlockSize = 4608

### Serialization

#### Save()

```csharp
public string Save()
```

Serializes the settings to a JSON string.

#### Load(string json)

```csharp
public static FLACOutput Load(string json)
```

Creates a new FLACOutput instance from a JSON string.

### Usage Example

```csharp
var flacSettings = new FLACOutput
{
    Level = 8,                   // Maximum compression
    BlockSize = 4608,            // Default block size
    MidSideCoding = true,        // Enable mid-side coding for better compression
    ExhaustiveModelSearch = true // Enable exhaustive search for best compression
};

core.Output_Format = flacSettings; // Core is VideoCaptureCore or VideoEditCore
```

### Best Practices

#### Compression Level Selection

- Use Level 0-3 for faster encoding with moderate compression
- Use Level 4-6 for balanced compression/speed
- Use Level 7-8 for maximum compression regardless of speed

#### Block Size Considerations

- Larger block sizes generally provide better compression
- Stick to standard values (4608, 4096, etc.) for maximum compatibility
- Consider memory constraints when selecting block size

#### Mid-Side Coding

- Enable for stereo content when compression is priority
- Use adaptive mode when encoding speed is important
- Disable for mono content as it has no effect

#### Rice Parameters

- Default values (3,3) are suitable for most use cases
- Increase for potentially better compression at the cost of encoding speed
- Values beyond 6 rarely provide significant benefits
