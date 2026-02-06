---
title: WMV File Output and Encoding Guide
description: Implement Windows Media Video encoding in .NET with audio/video configuration, streaming options, and cross-platform profile management.
---

# Windows Media Video encoders

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

This documentation covers the Windows Media Video (WMV) encoding capabilities available in VisioForge, including both Windows-specific and cross-platform solutions.

## Windows-only output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The [WMVOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.WMVOutput.html) class provides comprehensive Windows Media encoding capabilities for both audio and video on Windows platforms.

### Quick Start Guide

#### Simple Video Capture with Default Settings

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var captureCore = new VideoCaptureCore();

// Use default WMV settings (Internal Profile mode)
captureCore.Output_Format = new WMVOutput();
captureCore.Output_Filename = "output.wmv";

await captureCore.StartAsync();
```

#### Simple Video Editing with Default Settings

```csharp
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

var editCore = new VideoEditCore();

// Use default WMV settings
editCore.Output_Format = new WMVOutput();
editCore.Output_Filename = "edited_output.wmv";

// Add input files and configure editing...

await editCore.StartAsync();
```

#### Custom Settings Example

```csharp
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;

var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video configuration
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    
    // Audio configuration
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 90,
    Custom_Audio_Format = "48kHz 16bit Stereo"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = wmvOutput;
captureCore.Output_Filename = "custom_output.wmv";

await captureCore.StartAsync();
```

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

#### Understanding WMV Custom Settings Properties

Before diving into examples, it's important to understand the key properties available in the `WMVOutput` class for custom configuration:

**Video Properties:**
- `Custom_Video_StreamPresent` (bool): Enables video stream in the output
- `Custom_Video_Codec` (string): Specifies the video codec (e.g., "Windows Media Video 9")
- `Custom_Video_Mode` (WMVStreamMode): Rate control mode (CBR, VBRQuality, VBRBitrate, VBRPeakBitrate)
- `Custom_Video_Bitrate` (int): Target bitrate in bits per second
- `Custom_Video_Quality` (byte): Quality level (0-100) for VBR quality mode
- `Custom_Video_Width` (int): Output video width in pixels
- `Custom_Video_Height` (int): Output video height in pixels
- `Custom_Video_SizeSameAsInput` (bool): Use input video dimensions
- `Custom_Video_FrameRate` (double): Output frame rate
- `Custom_Video_KeyFrameInterval` (byte): Number of frames between keyframes
- `Custom_Video_Smoothness` (byte): Smoothness level (0-100)
- `Custom_Video_Peak_BitRate` (int): Peak bitrate for VBR peak mode
- `Custom_Video_Peak_BufferSizeSeconds` (byte): Peak buffer window in seconds
- `Custom_Video_Buffer_Size` (int): Buffer size in milliseconds
- `Custom_Video_Buffer_UseDefault` (bool): Use default buffer settings
- `Custom_Video_TVSystem` (WMVTVSystem): TV system standard (NTSC, PAL)

**Audio Properties:**
- `Custom_Audio_StreamPresent` (bool): Enables audio stream in the output
- `Custom_Audio_Codec` (string): Specifies the audio codec (e.g., "Windows Media Audio 9.2")
- `Custom_Audio_Format` (string): Format specification (e.g., "48kHz 16bit Stereo")
- `Custom_Audio_Mode` (WMVStreamMode): Rate control mode
- `Custom_Audio_Quality` (byte): Quality level (0-100) for VBR quality mode
- `Custom_Audio_PeakBitrate` (int): Peak bitrate in bits per second
- `Custom_Audio_PeakBufferSize` (byte): Peak buffer window in seconds
- `Custom_Audio_LanguageID` (string): Language identifier (e.g., "en-US")

**Profile Metadata:**
- `Custom_Profile_Name` (string): Profile name for identification
- `Custom_Profile_Description` (string): Detailed description of profile purpose
- `Custom_Profile_Language` (string): Profile language identifier

#### High-Quality Video Streaming Configuration

Perfect for professional streaming applications requiring excellent visual quality:

```csharp
var highQualityConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings - High quality 1080p
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 80,
    Custom_Video_Buffer_UseDefault = false,
    Custom_Video_Buffer_Size = 4000,
    
    // Audio settings - High quality stereo
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
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

// Apply to VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = highQualityConfig;
captureCore.Output_Filename = "output_hq.wmv";

// Or apply to VideoEditCore
var editCore = new VideoEditCore();
editCore.Output_Format = highQualityConfig;
editCore.Output_Filename = "output_hq.wmv";
```

#### Low Bandwidth Configuration for Mobile Streaming

Optimized for mobile devices with limited bandwidth:

```csharp
var mobileLowBandwidthConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings optimized for mobile
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 800000, // 800 kbps
    Custom_Video_Width = 854,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 24.0,
    Custom_Video_KeyFrameInterval = 5,
    Custom_Video_Smoothness = 60,
    Custom_Video_Buffer_UseDefault = true,
    
    // Audio settings for low bandwidth
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 64000, // 64 kbps
    Custom_Audio_Format = "44kHz 16bit Mono",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Mobile Low Bandwidth",
    Custom_Profile_Description = "480p optimized for mobile devices"
};

// Apply to VideoCaptureCore
var captureCore = new VideoCaptureCore();
captureCore.Output_Format = mobileLowBandwidthConfig;
captureCore.Output_Filename = "output_mobile.wmv";
```

#### Audio-Focused Configuration for Music Content

High-quality audio with minimal video processing:

```csharp
var audioFocusedConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // High quality audio settings
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 99,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_PeakBitrate = 512000,
    Custom_Audio_PeakBufferSize = 4,
    Custom_Audio_LanguageID = "en-US",
    
    // Minimal video settings
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.VBRBitrate,
    Custom_Video_Bitrate = 500000,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 25.0,
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Buffer_UseDefault = true,
    
    Custom_Profile_Name = "Audio Focus",
    Custom_Profile_Description = "High quality audio configuration for music content"
};

// Apply to VideoEditCore for processing audio files with video
var editCore = new VideoEditCore();
editCore.Output_Format = audioFocusedConfig;
editCore.Output_Filename = "output_audio_focus.wmv";
```

#### Constant Bitrate (CBR) for Streaming

CBR mode is ideal for network streaming where consistent bandwidth is required:

```csharp
var cbrStreamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings with CBR
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 2000000, // 2 Mbps constant
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 3000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Audio settings with CBR
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000, // 128 kbps constant
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Network streaming configuration
    Network_Streaming_WMV_Maximum_Clients = 50,
    
    Custom_Profile_Name = "CBR Streaming",
    Custom_Profile_Description = "Constant bitrate for reliable network streaming"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = cbrStreamingConfig;
captureCore.Output_Filename = "output_cbr_stream.wmv";
```

#### Variable Bitrate with Peak Control

VBR with peak bitrate constraint provides quality optimization while limiting maximum bandwidth:

```csharp
var vbrPeakConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings with peak bitrate control
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Video_Bitrate = 3000000, // 3 Mbps average
    Custom_Video_Peak_BitRate = 5000000, // 5 Mbps peak
    Custom_Video_Peak_BufferSizeSeconds = 3,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 4,
    Custom_Video_Smoothness = 75,
    
    // Audio settings with peak control
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRPeakBitrate,
    Custom_Audio_PeakBitrate = 256000,
    Custom_Audio_PeakBufferSize = 2,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    Custom_Profile_Name = "VBR Peak Control",
    Custom_Profile_Description = "Variable bitrate with peak constraints for quality and bandwidth balance"
};

var editCore = new VideoEditCore();
editCore.Output_Format = vbrPeakConfig;
editCore.Output_Filename = "output_vbr_peak.wmv";
```

#### Screen Recording Optimized Configuration

Optimized for screen capture with efficient encoding of static content:

```csharp
var screenRecordingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings optimized for screen content
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Screen",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 90,
    Custom_Video_SizeSameAsInput = true, // Use screen resolution
    Custom_Video_FrameRate = 15.0, // Lower frame rate for screen recording
    Custom_Video_KeyFrameInterval = 10,
    Custom_Video_Smoothness = 50,
    Custom_Video_Buffer_UseDefault = true,
    
    // Audio settings for voice narration
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 85,
    Custom_Audio_Format = "44kHz 16bit Mono", // Mono for voice
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Screen Recording",
    Custom_Profile_Description = "Optimized for screen capture with efficient compression"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = screenRecordingConfig;
captureCore.Output_Filename = "screen_recording.wmv";
```

#### Archival Quality Configuration

Maximum quality for archival purposes:

```csharp
var archivalConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings for maximum quality
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 100,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 1, // Every frame is a keyframe
    Custom_Video_Smoothness = 100,
    Custom_Video_Buffer_Size = 8000,
    Custom_Video_Buffer_UseDefault = false,
    
    // Audio settings for maximum quality
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2 Lossless",
    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
    Custom_Audio_Quality = 100,
    Custom_Audio_Format = "96kHz 24bit Stereo",
    Custom_Audio_LanguageID = "en-US",
    
    Custom_Profile_Name = "Archival Quality",
    Custom_Profile_Description = "Maximum quality for long-term storage",
    Custom_Profile_Language = "en-US"
};

var editCore = new VideoEditCore();
editCore.Output_Format = archivalConfig;
editCore.Output_Filename = "archival_quality.wmv";
```

### Internal Profile Usage

Internal profiles provide pre-configured settings optimized for common scenarios. Here are examples of using different internal profiles:

#### Available Codecs and Formats

Before configuring custom settings, it's useful to understand the available codecs and formats:

**Video Codecs:**
- "Windows Media Video 9" - Standard WMV9 codec
- "Windows Media Video 9 Advanced Profile" - Advanced features support
- "Windows Media Video 9 Screen" - Optimized for screen content
- "Windows Media Video 8" - Legacy WMV8 codec

**Audio Codecs:**
- "Windows Media Audio 9.2" - Standard WMA codec
- "Windows Media Audio 9.2 Professional" - High-quality audio
- "Windows Media Audio 9.2 Lossless" - Lossless compression
- "Windows Media Audio Voice 9" - Optimized for speech

**Audio Formats:**
Common format strings for `Custom_Audio_Format` property:
- "48kHz 16bit Stereo" - CD quality stereo
- "44kHz 16bit Stereo" - Standard quality stereo
- "44kHz 16bit Mono" - Standard quality mono
- "96kHz 24bit Stereo" - High-resolution audio
- "22kHz 16bit Mono" - Voice recording quality

#### Enumerating Available Codecs

You can enumerate available codecs programmatically:

```csharp
// For VideoCaptureCore
var captureCore = new VideoCaptureCore();

// Get available video codecs
string[] videoCodecs = captureCore.WMV_VideoCodecs_Available();
foreach (var codec in videoCodecs)
{
    Console.WriteLine($"Video Codec: {codec}");
}

// Get available audio codecs
string[] audioCodecs = captureCore.WMV_AudioCodecs_Available();
foreach (var codec in audioCodecs)
{
    Console.WriteLine($"Audio Codec: {codec}");
}

// Get available audio formats for a specific codec
string selectedCodec = audioCodecs[0];
string[] audioFormats = captureCore.WMV_AudioFormats_Available(selectedCodec);
foreach (var format in audioFormats)
{
    Console.WriteLine($"Audio Format: {format}");
}
```

Standard broadcast quality profile:

```csharp
var broadcastProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Advanced Profile",
    Custom_Video_TVSystem = WMVTVSystem.NTSC  // Optional TV system override
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = broadcastProfile;
captureCore.Output_Filename = "broadcast_output.wmv";
```

Web streaming profile:

```csharp
var webStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 for Broadband (2 Mbps)",
    Network_Streaming_WMV_Maximum_Clients = 100  // Optional streaming override
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = webStreamingProfile;
captureCore.Output_Filename = "web_stream.wmv";
```

Low latency profile for live streaming:

```csharp
var liveStreamingProfile = new WMVOutput
{
    Mode = WMVMode.InternalProfile,
    Internal_Profile_Name = "Windows Media Video 9 Screen (Low Rate)",
    Network_Streaming_WMV_Maximum_Clients = 50
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = liveStreamingProfile;
captureCore.Output_Filename = "live_stream.wmv";
```

#### Enumerating Available Internal Profiles

You can get a list of all available internal profiles:

```csharp
var captureCore = new VideoCaptureCore();
string[] profiles = captureCore.WMV_InternalProfiles_Available();

foreach (var profile in profiles)
{
    Console.WriteLine($"Available Profile: {profile}");
}

// Common internal profiles include:
// - "Windows Media Video 9 for Local Network (768 kbps)"
// - "Windows Media Video 9 for Broadband (2 Mbps)"
// - "Windows Media Video 9 Advanced Profile"
// - "Windows Media Video 9 Screen (Low Rate)"
// - "Windows Media Video 9 Screen (Medium Rate)"
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

### Complete Working Example

Here's a complete example showing video capture with custom WMV settings, including proper initialization and error handling:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace WMVCaptureExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize VisioForge SDK
            VisioForge.Core.VisioForge.InitSDK();
            
            // Create VideoCaptureCore instance
            var captureCore = new VideoCaptureCore();
            
            try
            {
                // Configure video source (first available camera)
                var videoDevices = captureCore.Video_CaptureDevices();
                if (videoDevices.Length > 0)
                {
                    captureCore.Video_CaptureDevice = new VideoCaptureSource(videoDevices[0].Name);
                }
                
                // Configure audio source (first available microphone)
                var audioDevices = captureCore.Audio_CaptureDevices();
                if (audioDevices.Length > 0)
                {
                    captureCore.Audio_CaptureDevice = new AudioCaptureSource(audioDevices[0].Name);
                }
                
                // Configure WMV output with custom settings
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // Video settings
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9",
                    Custom_Video_Mode = WMVStreamMode.VBRQuality,
                    Custom_Video_Quality = 85,
                    Custom_Video_Width = 1280,
                    Custom_Video_Height = 720,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 5,
                    
                    // Audio settings
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 90,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "Standard Capture",
                    Custom_Profile_Description = "Standard quality capture profile"
                };
                
                // Apply output settings
                captureCore.Output_Format = wmvOutput;
                captureCore.Output_Filename = "capture_output.wmv";
                captureCore.Mode = VideoCaptureMode.VideoCapture;
                
                // Start capture
                Console.WriteLine("Starting video capture...");
                await captureCore.StartAsync();
                
                Console.WriteLine("Recording... Press any key to stop.");
                Console.ReadKey();
                
                // Stop capture
                Console.WriteLine("Stopping video capture...");
                await captureCore.StopAsync();
                
                Console.WriteLine($"Video saved to: capture_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Clean up
                captureCore?.Dispose();
                VisioForge.Core.VisioForge.DestroySDK();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

### Complete Video Editing Example

Here's a complete example for video editing with custom WMV settings:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEdit;

namespace WMVEditExample
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize VisioForge SDK
            VisioForge.Core.VisioForge.InitSDK();
            
            var editCore = new VideoEditCore();
            
            try
            {
                // Add input video files
                editCore.Input_AddVideoFile("input_video1.mp4", false);
                editCore.Input_AddVideoFile("input_video2.mp4", false);
                
                // Configure WMV output
                var wmvOutput = new WMVOutput
                {
                    Mode = WMVMode.CustomSettings,
                    
                    // High-quality video settings
                    Custom_Video_StreamPresent = true,
                    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
                    Custom_Video_Mode = WMVStreamMode.VBRPeakBitrate,
                    Custom_Video_Bitrate = 4000000, // 4 Mbps average
                    Custom_Video_Peak_BitRate = 6000000, // 6 Mbps peak
                    Custom_Video_Peak_BufferSizeSeconds = 3,
                    Custom_Video_Width = 1920,
                    Custom_Video_Height = 1080,
                    Custom_Video_FrameRate = 30.0,
                    Custom_Video_KeyFrameInterval = 4,
                    Custom_Video_Smoothness = 80,
                    
                    // High-quality audio settings
                    Custom_Audio_StreamPresent = true,
                    Custom_Audio_Codec = "Windows Media Audio 9.2 Professional",
                    Custom_Audio_Mode = WMVStreamMode.VBRQuality,
                    Custom_Audio_Quality = 95,
                    Custom_Audio_Format = "48kHz 16bit Stereo",
                    
                    Custom_Profile_Name = "High Quality Edit",
                    Custom_Profile_Description = "High quality output for edited videos"
                };
                
                // Apply output settings
                editCore.Output_Format = wmvOutput;
                editCore.Output_Filename = "edited_output.wmv";
                
                // Configure edit mode
                editCore.Mode = VideoEditMode.Convert;
                
                // Start editing
                Console.WriteLine("Starting video editing...");
                await editCore.StartAsync();
                
                // Monitor progress
                while (editCore.State == VideoEditCoreState.Working)
                {
                    var progress = editCore.Progress();
                    Console.WriteLine($"Progress: {progress}%");
                    await Task.Delay(500);
                }
                
                Console.WriteLine($"Video editing complete! Output saved to: edited_output.wmv");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
            finally
            {
                // Clean up
                editCore?.Dispose();
                VisioForge.Core.VisioForge.DestroySDK();
            }
            
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
```

## Cross-platform WMV output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

The `WMVEncoderSettings` class provides a cross-platform solution for WMV encoding using GStreamer technology.

### Features

- Platform-independent implementation
- Integration with GStreamer backend
- Simple configuration interface
- Availability checking

### Sample Code

#### VideoCaptureCoreX Configuration

Add the WMV output to the Video Capture SDK core instance:

```csharp
// Basic configuration with default settings
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// With custom encoder settings
var wmvOutput2 = new WMVOutput("output_custom.wmv");
wmvOutput2.Video = new WMVEncoderSettings();
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 192,  // Bitrate in Kbps
    SampleRate = 48000,  // Sample rate in Hz
    Channels = 2  // Stereo
};

var core2 = new VideoCaptureCoreX();
core2.Outputs_Add(wmvOutput2, true);
```

#### VideoEditCoreX Configuration

Set the output format for the Video Edit SDK core instance:

```csharp
// Basic configuration
var wmvOutput = new WMVOutput("output.wmv");
var core = new VideoEditCoreX();
core.Output_Format = wmvOutput;

// With custom audio settings
var wmvOutput2 = new WMVOutput("output_high_quality.wmv");
wmvOutput2.Audio = new WMAEncoderSettings
{
    Bitrate = 256,  // High-quality audio
    SampleRate = 48000,
    Channels = 2
};

var core2 = new VideoEditCoreX();
core2.Output_Format = wmvOutput2;
```

#### Media Blocks Pipeline Configuration

Create a Media Blocks WMV output instance:

```csharp
// Configure audio encoder
var wma = new WMAEncoderSettings
{
    Bitrate = 128,  // Bitrate in Kbps
    SampleRate = 48000,  // 48 kHz
    Channels = 2  // Stereo
};

// Configure video encoder  
var wmv = new WMVEncoderSettings();

// Configure ASF sink (container)
var sinkSettings = new ASFSinkSettings("output.wmv");

// Create output block
var wmvOutput = new WMVOutputBlock(sinkSettings, wmv, wma);

// Add to pipeline
var pipeline = new MediaBlocksPipeline();
// ... configure sources and connect to wmvOutput
```

#### Checking Encoder Availability

Always check if encoders are available before use:

```csharp
// Check WMV encoder availability
if (WMVEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMV encoder is available");
    var wmvOutput = new WMVOutput("output.wmv");
    // ... use encoder
}
else
{
    Console.WriteLine("WMV encoder is not available on this system");
    // Fall back to alternative encoder
}

// Check WMA encoder availability
if (WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is available");
}
```

#### Advanced Cross-Platform Configuration

```csharp
// Create a high-quality cross-platform WMV output
var wmvOutput = new WMVOutput("output_hq.wmv");

// Configure high-quality audio
wmvOutput.Audio = new WMAEncoderSettings
{
    Bitrate = 320,  // Maximum quality
    SampleRate = 48000,
    Channels = 2
};

// Video encoder settings (uses default WMV1 encoder)
wmvOutput.Video = new WMVEncoderSettings();

// Check if custom video processor is needed
// wmvOutput.CustomVideoProcessor = myCustomProcessor;

// Apply to core
var core = new VideoCaptureCoreX();
core.Outputs_Add(wmvOutput, true);

// Start capture
await core.StartAsync();
```

#### Available Audio Settings

The `WMAEncoderSettings` class provides the following configuration options:

```csharp
var audioSettings = new WMAEncoderSettings
{
    // Bitrate in Kbps - supported values: 128, 192, 256, 320
    Bitrate = 192,
    
    // Sample rate in Hz - supported values: 44100, 48000
    SampleRate = 48000,
    
    // Number of channels - supported values: 1 (mono), 2 (stereo)
    Channels = 2
};

// Get supported bitrates
int[] supportedBitrates = audioSettings.GetSupportedBitrates();
// Returns: [128, 192, 256, 320]

// Get supported sample rates
int[] supportedSampleRates = audioSettings.GetSupportedSampleRates();
// Returns: [44100, 48000]

// Get supported channel counts
int[] supportedChannels = audioSettings.GetSupportedChannelCounts();
// Returns: [1, 2]
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

### MSDN References

For detailed information about Windows Media technologies, refer to these official Microsoft resources:

- [Windows Media Format SDK](https://learn.microsoft.com/es-es/windows/win32/wmformat/windows-media-format-11-sdk) - Complete Windows Media Format documentation
- [Working with Profiles](https://learn.microsoft.com/en-us/windows/win32/wmformat/working-with-profiles) - Profile management and configuration
- [Windows Media Codecs](https://learn.microsoft.com/en-us/windows/win32/medfound/windows-media-codecs) - Audio and video codec information
- [ASF File Structure](https://learn.microsoft.com/en-us/windows/win32/medfound/asf-file-structure) - Advanced Systems Format container details
- [Configuring Video Streams](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-video-streams) - Video encoding parameters
- [Configuring Audio Streams](https://learn.microsoft.com/en-us/windows/win32/wmformat/configuring-audio-streams) - Audio encoding parameters

### Choosing Rate Control Modes

Select the appropriate rate control mode based on your use case:

1. **CBR (Constant Bitrate)**
   - Use for: Network streaming, broadcasting
   - Advantages: Predictable bandwidth, consistent quality
   - Disadvantages: Less efficient compression, may not adapt to content complexity
   - Example: Live streaming to ensure smooth playback

2. **VBRQuality (Variable Bitrate - Quality)**
   - Use for: File-based output, archival, high-quality video
   - Advantages: Best quality-to-size ratio, adapts to content complexity
   - Disadvantages: Unpredictable file size and bitrate
   - Example: Recording tutorials or presentations for later playback

3. **VBRBitrate (Variable Bitrate - Target Bitrate)**
   - Use for: When you need quality optimization with size constraints
   - Advantages: Balances quality and target file size
   - Disadvantages: Quality may vary between scenes
   - Example: Creating videos for upload with size limits

4. **VBRPeakBitrate (Variable Bitrate - Peak Constrained)**
   - Use for: Streaming with bandwidth constraints
   - Advantages: Quality optimization with bandwidth ceiling
   - Disadvantages: More complex configuration
   - Example: Adaptive streaming scenarios

### Performance Optimization

1. **Buffer Configuration**
   - Set `Custom_Video_Buffer_UseDefault = false` for fine-tuned control
   - Increase `Custom_Video_Buffer_Size` for smoother streaming (default: 3000ms)
   - Balance buffer size with latency requirements

2. **KeyFrame Interval**
   - Lower values (1-3): Better seek performance, larger file size
   - Higher values (5-10): Smaller file size, less seek precision
   - Recommended: 3-5 for streaming, 10+ for screen recording

3. **Smoothness Settings**
   - 0-50: Prioritize compression efficiency
   - 50-75: Balanced quality and efficiency
   - 75-100: Prioritize visual quality

### Resolution and Frame Rate Guidelines

```csharp
// 4K/UHD Configuration
var uhd4KConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9 Advanced Profile",
    Custom_Video_Mode = WMVStreamMode.VBRQuality,
    Custom_Video_Quality = 95,
    Custom_Video_Width = 3840,
    Custom_Video_Height = 2160,
    Custom_Video_FrameRate = 30.0,
    // ... other settings
};

// Full HD Configuration
var fullHDConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1920,
    Custom_Video_Height = 1080,
    Custom_Video_FrameRate = 30.0,
    // ... other settings
};

// HD Ready Configuration
var hdReadyConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    // ... other settings
};

// SD Configuration
var sdConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    Custom_Video_StreamPresent = true,
    Custom_Video_Width = 720,
    Custom_Video_Height = 480,
    Custom_Video_FrameRate = 29.97,
    Custom_Video_TVSystem = WMVTVSystem.NTSC,
    // ... other settings
};
```

### Error Handling and Validation

Always validate your configuration before starting capture or editing:

```csharp
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuration
};

try
{
    var captureCore = new VideoCaptureCore();
    captureCore.Output_Format = wmvOutput;
    captureCore.Output_Filename = "output.wmv";
    
    // Validate configuration
    if (captureCore.Output_Filename == null || captureCore.Output_Filename.Length == 0)
    {
        throw new InvalidOperationException("Output filename is required");
    }
    
    // Check if custom codecs are available
    if (wmvOutput.Mode == WMVMode.CustomSettings)
    {
        var videoCodecs = captureCore.WMV_VideoCodecs_Available();
        if (!videoCodecs.Contains(wmvOutput.Custom_Video_Codec))
        {
            Console.WriteLine($"Warning: Video codec '{wmvOutput.Custom_Video_Codec}' may not be available");
        }
    }
    
    await captureCore.StartAsync();
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    // Handle error appropriately
}
```

### Network Streaming Configuration

For network streaming scenarios, configure both encoder and streaming settings:

```csharp
var streamingConfig = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    
    // Video settings optimized for streaming
    Custom_Video_StreamPresent = true,
    Custom_Video_Codec = "Windows Media Video 9",
    Custom_Video_Mode = WMVStreamMode.CBR,
    Custom_Video_Bitrate = 1500000, // 1.5 Mbps
    Custom_Video_Width = 1280,
    Custom_Video_Height = 720,
    Custom_Video_FrameRate = 30.0,
    Custom_Video_KeyFrameInterval = 3,
    Custom_Video_Buffer_Size = 2000, // Lower buffer for reduced latency
    Custom_Video_Buffer_UseDefault = false,
    
    // Audio settings
    Custom_Audio_StreamPresent = true,
    Custom_Audio_Codec = "Windows Media Audio 9.2",
    Custom_Audio_Mode = WMVStreamMode.CBR,
    Custom_Audio_PeakBitrate = 128000,
    Custom_Audio_Format = "48kHz 16bit Stereo",
    
    // Network streaming settings
    Network_Streaming_WMV_Maximum_Clients = 100,
    
    Custom_Profile_Name = "Network Streaming",
    Custom_Profile_Description = "Optimized for network streaming with low latency"
};

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = streamingConfig;
captureCore.Output_Filename = "http://localhost:8080/stream"; // Or file path
```

### Testing and Validation

1. **Always test your configuration** with sample content before production use
2. **Monitor encoding performance** to ensure real-time encoding capability
3. **Check file compatibility** with your target playback devices
4. **Validate audio sync** especially with custom frame rates
5. **Test network streaming** under various bandwidth conditions

### Profile Management

Save and reuse configurations for consistency:

```csharp
// Save configuration to JSON
var wmvOutput = new WMVOutput
{
    Mode = WMVMode.CustomSettings,
    // ... configuration
};

string jsonConfig = wmvOutput.Save();
File.WriteAllText("profile_high_quality.json", jsonConfig);

// Load configuration from JSON
string loadedJson = File.ReadAllText("profile_high_quality.json");
var loadedProfile = WMVOutput.Load(loadedJson);

var captureCore = new VideoCaptureCore();
captureCore.Output_Format = loadedProfile;
```

### Common Issues and Solutions

1. **Large File Sizes**
   - Use VBRBitrate mode instead of VBRQuality
   - Reduce video quality or resolution
   - Increase KeyFrameInterval
   - Consider using screen codec for screen recordings

2. **Poor Quality**
   - Increase video quality setting
   - Use higher bitrate
   - Switch to VBRQuality mode
   - Ensure sufficient buffer size

3. **Streaming Issues**
   - Use CBR mode for consistent bandwidth
   - Reduce buffer size for lower latency
   - Test with appropriate client count
   - Monitor network bandwidth

4. **Codec Not Available**
   - Ensure Windows Media components are installed
   - Check codec enumeration programmatically
   - Fall back to default internal profiles
   - Consider cross-platform alternatives (WMVEncoderSettings)

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.