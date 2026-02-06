---
title: Windows Media Audio Encoder Integration Guide
description: Implement WMA audio encoding in .NET with cross-platform and Windows-specific approaches, bitrate controls, and codec configuration.
---

# Windows Media Audio encoder

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Windows Media Audio (WMA) is a popular audio codec developed by Microsoft for efficient audio compression. This documentation covers the WMA encoder implementations available in the VisioForge .Net SDKs.

## Overview

The VisioForge SDK provides two distinct approaches for WMA encoding: the platform-specific [WMAOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMAOutput.html) for Windows environments and the cross-platform [WMAEncoderSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.AudioEncoders.WMAEncoderSettings.html). Let's explore both implementations in detail to understand their capabilities and use cases.

## Cross-platform WMA output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

The `WMAEncoderSettings` provides a cross-platform solution for WMA encoding. This implementation is built on SDK and offers consistent behavior across different operating systems.

### Key Features

The encoder supports the following audio configurations:

- Sample rates: 44.1 kHz and 48 kHz
- Bitrates: 128, 192, 256, and 320 Kbps
- Channel configurations: Mono (1) and Stereo (2)

### Rate Control

The WMA encoder implements constant bitrate (CBR) encoding, allowing you to specify a fixed bitrate from the supported values. This ensures consistent audio quality and predictable file sizes throughout the encoded content.

### Usage Example

Add the WMA output to the Video Capture SDK core instance:

```csharp
// Create a Video Capture SDK core instance
var core = new VideoCaptureCoreX();

// Create a WMA output
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Add the WMA output
core.Outputs_Add(wmaOutput, true);
```

Set the output format for the Video Edit SDK core instance:

```csharp
// Create a Video Edit SDK core instance
var core = new VideoEditCoreX();

// Create a WMA output
var wmaOutput = new WMAOutput("output.wma");
wmaOutput.Audio.SampleRate = 48000;
wmaOutput.Audio.Channels = 2;
wmaOutput.Audio.Bitrate = 320;

// Add the WMA output
core.Output_Format = wmaOutput;
```

Create a Media Blocks WMA output instance:

```csharp
// Create a WMA encoder settings instance
var wmaSettings = new WMAEncoderSettings();

// Create a WMA output instance
var wmaOutput = new WMAEncoderBlock(wmaSettings);

// Create a ASF output instance
var asfOutput = new ASFSinkBlock(new ASFSinkSettings("output.wma"));

// Connect the WMA encoder to the ASF output
pipeline.Connect(wmaOutput.Output, asfOutput.Input); // pipeline is MediaBlocksPipeline
```

Check if MP3 encoding is available.

```
if (!MP3EncoderSettings.IsAvailable())
{
   // Handle error
}
```

## Windows-only WMA output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The `WMAOutput` class provides a comprehensive Windows-specific implementation with advanced features and configuration options. This implementation leverages the Windows Media Format SDK for optimal performance on Windows systems.

### Key Features

The Windows-specific implementation offers:

- Multiple profile support (internal, external, and custom)
- Language and localization settings
- Quality-based encoding
- Advanced bitrate control with peak bitrate settings
- Buffer size configuration

### Rate Control

The Windows implementation supports three stream modes through the WMVStreamMode enumeration:

- CBR (Constant Bitrate)
- VBR (Variable Bitrate)
- Quality-based VBR

### Usage Example

Here's how to set up the Windows-specific WMA encoder:

Use an internal profile for simple configuration

```csharp
var wmaOutput = new WMAOutput
{
    // Use an internal profile for simple configuration
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Audio 9 High (192K)"
};

core.Output_Format = wmaOutput; // Core is VideoCaptureCore or VideoEditCore
```

Or configure custom settings

```csharp
var wmaOutput = new WMAOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Quality = 98,        // High quality setting
    Custom_Audio_PeakBitrate = 320,   // Maximum bitrate in Kbps
    Custom_Audio_PeakBufferSize = 3   // Buffer size for streaming
};

core.Output_Format = wmaOutput; // Core is VideoCaptureCore or VideoEditCore
```

### Profile Management

The Windows implementation supports three profile modes:

1. Internal Profiles:
   - Pre-configured profiles for common use cases
   - Access through `Internal_Profile_Name`

2. External Profiles:
   - Load profiles from external files
   - Configure using `External_Profile_FileName` or `External_Profile_Text`

3. Custom Profiles:
   - Fine-grained control over encoding parameters
   - Configure through Custom_* properties

## Best Practices

When implementing WMA encoding in your application:

1. For Windows applications requiring advanced features:
   - Use WMAOutput for access to Windows-specific optimizations
   - Consider saving configurations to JSON for reuse
   - Implement proper error handling for profile loading

2. For cross-platform applications:
   - Stick to WMAEncoderSettings for consistent behavior
   - Verify supported rates before setting configuration
   - Use the highest supported sample rate and bitrate for best quality

This documentation provides a foundation for implementing WMA encoding in your applications. The choice between cross-platform and Windows-specific implementations should be based on your application's requirements for platform support, encoding features, and quality control.
