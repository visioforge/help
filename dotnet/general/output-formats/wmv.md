---
title: WMV File Output and Encoding Guide
description: Learn how to implement Windows Media Video (WMV) encoding in .NET applications. Covers audio/video configuration, streaming options, profile management, and cross-platform solutions with code examples.
sidebar_label: Windows Media Video

---

# Windows Media Video encoders

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

This documentation covers the Windows Media Video (WMV) encoding capabilities available in VisioForge, including both Windows-specific and cross-platform solutions.

## Windows-only output

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

The [WMVOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.Output.WMVOutput.html) class provides comprehensive Windows Media encoding capabilities for both audio and video on Windows platforms.

### Audio Encoding Features

The `WMVOutput` class offers several audio-specific configuration options:

- Custom audio codec selection
- Audio format customization
- Multiple stream modes
- Bitrate control
- Quality settings
- Language support
- Buffer size management

### Rate Control Modes

WMV encoding supports four rate control modes through the `WMVStreamMode` enum:

1. CBR (Constant Bitrate)
2. VBRQuality (Variable Bitrate based on quality)
3. VBRBitrate (Variable Bitrate with target bitrate)
4. VBRPeakBitrate (Variable Bitrate with peak bitrate constraint)

### Configuration Modes

The encoder can be configured in several ways using the `WMVMode` enum:

- ExternalProfile: Load settings from a profile file
- ExternalProfileFromText: Load settings from a text string
- InternalProfile: Use built-in profiles
- CustomSettings: Manual configuration
- V8SystemProfile: Use Windows Media 8 system profiles

### Sample Code

Create new WMV custom output configuration:

```csharp
var wmvOutput = new WMVOutput
{
    // Basic configuration
    Mode = WMVMode.CustomSettings,
    
    // Audio settings
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_PeakBitrate = 192000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Optional language setting
    Custom_Audio_LanguageID = "en-US"
};
```

Using an internal profile:

```csharp
var profileWmvOutput = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Local Network (768 kbps)"
};
```

Network streaming configuration:

```csharp
var streamingWmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Network_Streaming_WMV_Maximum_Clients = 20,
    Custom_Audio_Mode = WMVStreamMode.CBR
};
```

### Custom Profile Configuration

Custom profiles give you the most flexibility by allowing you to configure every aspect of the encoding process. Here are several examples for different scenarios:

High-quality video streaming configuration:

```csharp
var highQualityConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings
    Custom_Video_StreamPresent = true,
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 80,
    Custom_Video_Buffer_UseDefault = false,
    Custom_Video_Buffer_Size = 4000,
    
    // Audio settings
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 98,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    Custom_Audio_PeakBitrate = 320000,
    Custom_Audio_PeakBufferSize = 3,
    
    // Profile metadata
    Custom_Profile_Name = "High Quality Streaming",
    Custom_Profile_Description = "1080p streaming profile with high quality audio",
    Custom_Profile_Language = "en-US"
};
```

Low bandwidth configuration for mobile streaming:

```csharp
var mobileLowBandwidthConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings optimized for mobile
    Custom_Video_StreamPresent = true,
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 800000, // 800 kbps
    Custom_Video_Width = 854,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 24.0,
    Custom_Video_KeyFrameInterval = 5,
    Custom_Video_Smoothness = 60,
    
    // Audio settings for low bandwidth
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 64000, // 64 kbps
    Custom_Audio_Format = "44kHz 16bit Mono",
    
    Custom_Profile_Name = "Mobile Low Bandwidth",
    Custom_Profile_Description = "480p optimized for mobile devices"
};
```

Audio-focused configuration for music content:

```csharp
var audioFocusedConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // High quality audio settings
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 99,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_PeakBitrate = 512000,
    Custom_Audio_PeakBufferSize = 4,
    
    // Minimal video settings
    Custom_Video_StreamPresent = true,
    Custom_Video_Mode = WMVStreamMode.VBRBitrate,
    Custom_Video_Bitrate = 500000,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 25.0,
    
    Custom_Profile_Name = "Audio Focus",
    Custom_Profile_Description = "High quality audio configuration for music content"
};
```

### Internal Profile Usage

Internal profiles provide pre-configured settings optimized for common scenarios. Here are examples of using different internal profiles:

Standard broadcast quality profile:

```csharp
var broadcastProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Advanced Profile",
    Custom_Video_TVSystem = WMVTVSystem.NTSC  // Optional TV system override
};
```

Web streaming profile:

```csharp
var webStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Broadband (2 Mbps)",
    Network_Streaming_WMV_Maximum_Clients = 100  // Optional streaming override
};
```

Low latency profile for live streaming:

```csharp
var liveStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Screen (Low Rate)",
    Network_Streaming_WMV_Maximum_Clients = 50
};
```

### External Profile Configuration

External profiles allow you to load encoding settings from files or text. This is useful for sharing configurations across different projects or storing multiple configurations:

Loading profile from a file:

```csharp
var fileBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfile,
    External_Profile_FileName = @"C:\Profiles\HighQualityStreaming.prx"
};
```

Loading profile from text configuration:

```csharp
var textBasedProfile = new WMVOutput
{
    Mode = WMVMode.ExternalProfileFromText,
    External_Profile_Text = @"
        <profile version=""589824"" 
                 storageformat=""1"" 
                 name=""Custom Streaming Profile"" 
                 description=""High quality streaming profile"">
            <streamconfig majortype=""{73647561-0000-0010-8000-00AA00389B71}"" 
                         streamnumber=""1"" 
                         streamname=""Audio Stream"" 
                         inputname=""Audio409"" 
                         bitrate=""128000"" 
                         bufferwindow=""5000"" 
                         reliabletransport=""0"" 
                         decodercomplexity="""" 
                         rfc1766langid=""en-us""/>
            <!-- Additional profile configuration -->
        </profile>"
};
```

Saving and loading profiles programmatically:

```csharp
async Task SaveAndLoadProfile(WMVOutput profile, string filename)
{
    // Save profile configuration to JSON
    string jsonConfig = profile.Save();
    await File.WriteAllTextAsync(filename, jsonConfig);
    
    // Load profile configuration from JSON
    string loadedJson = await File.ReadAllTextAsync(filename);
    WMVOutput loadedProfile = WMVOutput.Load(loadedJson);
}
```

Example usage of profile saving/loading:

```csharp
var profile = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configure settings ...
};

await SaveAndLoadProfile(profile, "encoding_profile.json");
```

### Working with Legacy Windows Media 8 Profiles

For compatibility with older systems, you can use Windows Media 8 system profiles:

Using Windows Media 8 profile:

```csharp
var wmv8Profile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Dial-up Access (28.8 Kbps)",
};
```

Customizing streaming settings for Windows Media 8 profiles:

```csharp
var wmv8StreamingProfile = new WMVOutput
{
    Mode = WMVMode.V8SystemProfile,
    V8ProfileName = "Windows Media Video 8 for Local Area Network (384 Kbps)",
    Network_Streaming_WMV_Maximum_Clients = 25,
    Custom_Video_TVSystem = WMVTVSystem.PAL  // Optional TV system override
};
```

### Apply settings to your core object

```csharp
var core = new VideoCaptureCore(); // or VideoEditCore
core.Output_Format = wmvOutput;
core.Output_Filename = "output.wmv";
```

## Cross-platform WMV output

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

The `WMVEncoderSettings` class provides a cross-platform solution for WMV encoding using GStreamer technology.

### Features

- Platform-independent implementation
- Integration with GStreamer backend
- Simple configuration interface
- Availability checking

### Sample Code

Add the WebM output to the Video Capture SDK core instance:

```csharp
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);
```

Set the output format for the Video Edit SDK core instance:

```csharp
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoEditCoreX();
core.Output_Format = wmvOutput;
```

Create a Media Blocks WebM output instance:

```csharp
var wma = new WMAEncoderSettings();
var wmv = new WMVEncoderSettings();
var sinkSettings = new ASFSinkSettings("output.wmv");
var webmOutput = new WMVOutputBlock(sinkSettings, wmv, wma);
```

### Choosing Between Encoders

Consider the following factors when choosing between Windows-specific `WMVOutput` and cross-platform `WMVEncoderSettings`:

#### Windows-Specific WMVOutput

- Pros:
  - Full access to Windows Media format features
  - Advanced rate control options
  - Network streaming support
  - Profile-based configuration
- Cons:
  - Windows-only compatibility
  - Requires Windows Media components

#### Cross-Platform WMV

- Pros:
  - Platform independence
  - Simpler implementation
- Cons:
  - More limited feature set
  - Basic configuration options only

## Best Practices

1. Always check encoder availability before use, especially with cross-platform implementations
2. Use appropriate rate control modes based on your quality and bandwidth requirements
3. Consider using internal profiles for common scenarios when using WMVOutput
4. Implement proper error handling for codec availability checks
5. Test encoding performance across different platforms when using cross-platform solutions

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
