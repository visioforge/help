---
title: WMV Network Streaming Implementation in .NET Applications
description: Implement Windows Media Video streaming in .NET with compression algorithms, adaptive bitrates, and bandwidth optimization for network delivery.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Streaming
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - WMVOutput
  - WMVMode
  - NetworkStreamingFormat

---

# Windows Media Video (WMV) Network Streaming Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Introduction to WMV Streaming Technology

Windows Media Video (WMV) represents a versatile and powerful streaming technology developed by Microsoft. As an integral component of the Windows Media framework, WMV has established itself as a reliable solution for efficiently delivering video content across networks. This format utilizes sophisticated compression algorithms that substantially reduce file sizes while maintaining acceptable visual quality, making it particularly well-suited for streaming applications where bandwidth optimization is critical.

The WMV format supports an extensive range of video resolutions and bitrates, allowing developers to tailor their streaming implementations to accommodate varying network conditions and end-user requirements. This adaptability makes WMV an excellent choice for applications that need to serve diverse client environments with different connectivity constraints.

## Technical Overview of WMV Format

### Key Features and Capabilities

WMV implements the Advanced Systems Format (ASF) container, which provides several technical advantages for streaming applications:

- **Efficient compression**: Employs codec technology that balances quality with file size
- **Scalable bitrate adjustment**: Adapts to available bandwidth conditions
- **Error resilience**: Built-in mechanisms for packet loss recovery
- **Content protection**: Supports Digital Rights Management (DRM) when required
- **Metadata support**: Allows embedding of descriptive information about the stream

### Technical Specifications

| Feature | Specification |
|---------|---------------|
| Codec | VC-1 (primarily) |
| Container | ASF (Advanced Systems Format) |
| Supported resolutions | Up to 4K UHD (depending on profile) |
| Bitrate range | 10 Kbps to 20+ Mbps |
| Audio support | WMA (Windows Media Audio) |
| Streaming protocols | HTTP, RTSP, MMS |

## Windows-Only WMV Streaming Implementation

[VideoCaptureCore](#){ .md-button }

The VisioForge SDK provides a robust framework for implementing WMV streaming in Windows environments. This implementation allows applications to broadcast video over networks while simultaneously capturing to a file if desired.

### Implementation Prerequisites

Before implementing WMV streaming in your application, ensure the following requirements are met:

1. Your development environment includes the VisioForge Video Capture SDK
2. Required redistributables are installed (details provided in the Deployment section)
3. Your application targets Windows operating systems
4. Network ports are properly configured and accessible

### Step-by-Step Implementation Guide

#### 1. Initialize the Video Capture Component

Begin by setting up the core video capture component in your application:

```cs
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

// Initialize VideoCaptureCore with the VideoView that hosts the preview
var VideoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

// Configure basic capture settings (device selection, mode, etc.)
// ...
```

#### 2. Enable Network Streaming

To activate network streaming functionality, you need to enable it explicitly and set the format to WMV:

```cs
// Enable network streaming
VideoCapture1.Network_Streaming_Enabled = true;

// Set the streaming format to WMV
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.WMV;
```

#### 3. Configure WMV Output Settings

Create and configure a WMV output object with appropriate settings:

```cs
// Create WMV output. Default ctor picks the "Windows Media Video 9 for Local Network
// (768 kbps)" internal profile with Mode = WMVMode.InternalProfile.
var wmvOutput = new WMVOutput();

// Option A: pick a different built-in profile
wmvOutput.Mode = WMVMode.InternalProfile;
wmvOutput.Internal_Profile_Name = "Windows Media Video 9 for Broadband (NTSC, 1400 Kbps)";

// Option B: drive the encoder from custom settings instead of a profile.
// Note the flat "Custom_*" naming — WMVOutput has no nested Profile object.
// Bitrate is bits/sec; KeyFrameInterval is seconds between keyframes;
// Quality is a 0..100 byte.
wmvOutput.Mode = WMVMode.CustomSettings;
wmvOutput.Custom_Video_StreamPresent = true;
wmvOutput.Custom_Video_Bitrate = 2_000_000;      // 2 Mbps
wmvOutput.Custom_Video_KeyFrameInterval = 3;     // seconds
wmvOutput.Custom_Video_Quality = 85;             // 0..100
wmvOutput.Custom_Video_SizeSameAsInput = true;
wmvOutput.Custom_Audio_StreamPresent = true;

// Cap the number of simultaneous clients (lives on WMVOutput, not on VideoCaptureCore)
wmvOutput.Network_Streaming_WMV_Maximum_Clients = 25;

// Wire the WMV output into the network-streaming pipeline
VideoCapture1.Network_Streaming_Output = wmvOutput;

// The port that the Windows Media server binds to
VideoCapture1.Network_Streaming_Network_Port = 12345;
```

#### 4. Start the Streaming Process

Once everything is configured, you can start the streaming process:

```cs
// Start the streaming process
try
{
    await VideoCapture1.StartAsync();

    // The streaming URL is now available for clients
    string streamingUrl = VideoCapture1.Network_Streaming_URL;

    // Display or log the streaming URL for client connections
    Console.WriteLine($"Streaming available at: {streamingUrl}");
}
catch (Exception ex)
{
    // Handle any exceptions during streaming initialization
    Console.WriteLine($"Streaming error: {ex.Message}");
}
```

### Advanced Configuration Options

#### External .prx Profile Files

For fine control beyond the built-in profiles, point WMVOutput at a profile file
authored with Windows Media Profile Editor:

```cs
wmvOutput.Mode = WMVMode.ExternalProfile;
wmvOutput.External_Profile_FileName = @"C:\profiles\my-stream.prx";

// Or paste the profile XML inline:
// wmvOutput.Mode = WMVMode.ExternalProfileFromText;
// wmvOutput.External_Profile_Text = "<profile ...>...</profile>";

VideoCapture1.Network_Streaming_Output = wmvOutput;
```

## Client-Side Connection Implementation

Clients can connect to the WMV stream using Windows Media Player or any application that supports the Windows Media streaming protocol. The connection URL follows this format:

```
http://[server_ip]:[port]/
```

For example:
```
http://192.168.1.100:12345/
```

### Sample Client Connection Code

For programmatic connections to the WMV stream in client applications:

```cs
// Client-side WMV stream connection using Windows Media Player control
using System.Windows.Forms;

public partial class StreamViewerForm : Form
{
    public StreamViewerForm(string streamUrl)
    {
        InitializeComponent();
        
        // Assuming you have a Windows Media Player control named 'wmPlayer' on your form
        wmPlayer.URL = streamUrl;
        wmPlayer.Ctlcontrols.play();
    }
}
```

## Performance Optimization

When implementing WMV network streaming, consider these optimization strategies:

1. **Adjust bitrate based on network conditions**: Lower bitrates for constrained networks
2. **Balance keyframe intervals**: Frequent keyframes improve seek performance but increase bandwidth
3. **Monitor CPU usage**: WMV encoding can be CPU-intensive; adjust quality settings accordingly
4. **Implement network quality detection**: Adapt streaming parameters dynamically
5. **Consider buffer settings**: Larger buffers improve stability but increase latency

## Troubleshooting Common Issues

| Issue | Possible Solution |
|-------|-------------------|
| Connection failures | Verify network port is open in firewall settings |
| Poor video quality | Increase bitrate or adjust compression settings |
| High CPU usage | Reduce resolution or frame rate |
| Client buffering | Adjust buffer window settings or reduce bitrate |
| Authentication errors | Verify credentials on both server and client |

## Deployment Requirements

### Required Redistributables

To successfully deploy applications using WMV streaming functionality, include the following redistributable packages:

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Installation Commands

Using NuGet Package Manager:

```
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

Or for 32-bit systems:

```
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
```

## Conclusion

WMV network streaming provides a reliable way to broadcast video content across networks in Windows environments. The VisioForge SDK simplifies implementation with its comprehensive API while giving developers fine-grained control over streaming parameters. By following the guidelines in this document, you can create robust streaming applications that deliver high-quality video content to multiple clients simultaneously.

For more advanced implementations and additional code samples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
