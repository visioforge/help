---
title: IP Camera Streaming to MP4 Files in .NET C#
description: Implement IP camera video streaming and recording to MP4 files in .NET using C# with RTSP connection, encoding options, and code examples.
---

# Capturing IP Camera Streams to MP4 Files in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to IP Camera Recording

IP cameras provide powerful surveillance and monitoring capabilities through network connections. This guide demonstrates how to leverage these devices in .NET applications to capture and save video streams to MP4 files. Using modern C# coding practices, you'll learn how to establish connections to IP cameras, configure video streams, and save high-quality recordings for various applications including security systems, monitoring solutions, and video archiving tools.

## Video Implementation Tutorial

The following video walks through the complete implementation process:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/qX3AiGyWbO8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-capture-mp4)

## Step-by-Step Implementation Guide

This guide demonstrates how to establish connections to IP cameras, stream their video content, and encode it directly to MP4 files using the VideoCaptureCoreX component. The implementation supports multiple camera protocols including RTSP, HTTP, and ONVIF.

### Code Implementation

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core;

namespace ip_camera_capture_mp4
{
    public partial class Form1 : Form
    {
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // RTSP camera source
            var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
            videoCapture1.Video_Source = rtsp;

            // audio output device
            var audioOutputDevice = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.DirectSound))[0];
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioOutputDevice);

            // configure MP4 output
            var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            // start
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();

            await videoCapture1.DisposeAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            await VisioForgeX.InitSDKAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Implementation Details and Best Practices

### SDK Initialization

Before working with IP cameras, proper SDK initialization is crucial to ensure all components are loaded and ready:

```csharp
private async void Form1_Load(object sender, EventArgs e)
{
    await VisioForgeX.InitSDKAsync();
}
```

Always initialize the SDK at application startup to ensure all required components are properly loaded before attempting camera connections.

### Camera Connection Configuration

The example uses RTSP (Real-Time Streaming Protocol), one of the most common protocols for IP camera streaming:

```csharp
// RTSP camera source
var rtsp = await RTSPSourceSettings.CreateAsync(new Uri(edURL.Text), edLogin.Text, edPassword.Text, cbAudioStream.Checked);
videoCapture1.Video_Source = rtsp;
```

When connecting to IP cameras, consider these important parameters:

- **Camera URL** - The complete RTSP URL to your camera (e.g., `rtsp://192.168.1.100:554/stream1`)
- **Authentication** - Many cameras require username and password credentials
- **Audio Stream** - Toggle whether to include audio from the camera
- **Connection Timeout** - For production applications, implement appropriate timeout handling

### MP4 Output Configuration

MP4 is an ideal container format for video recordings due to its excellent compatibility and compression:

```csharp
// configure MP4 output
var mp4Output = new MP4Output(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4"));
videoCapture1.Outputs_Add(mp4Output);
```

When configuring MP4 output, consider these options:

- **File Naming** - Implement dynamic file naming based on date/time for organized recordings
- **Storage Location** - Choose appropriate storage paths with sufficient disk space
- **Video Quality** - Configure bitrate, framerate, and resolution settings based on your requirements
- **Metadata** - Add relevant metadata to recordings for easier classification and searching

### Resource Management

Proper resource management is critical when working with video streams to prevent memory leaks and ensure stable performance:

```csharp
private async void btStop_Click(object sender, EventArgs e)
{
    await videoCapture1.StopAsync();
    await videoCapture1.DisposeAsync();
}

private void Form1_FormClosing(object sender, FormClosingEventArgs e)
{
    VisioForgeX.DestroySDK();
}
```

Always implement proper resource cleanup, especially:

- Stop active streams before application closing
- Dispose of capture resources when no longer needed
- Release SDK resources when your application exits

### Advanced Implementation Considerations

For production-grade applications, consider these additional features:

1. **Error Handling** - Implement comprehensive error handling for network disconnections, authentication failures, and storage issues
2. **Monitoring** - Add status monitoring to track stream health and recording status
3. **Reconnection Logic** - Implement automatic reconnection for network interruptions
4. **Multi-Camera Support** - Extend the implementation to handle multiple camera streams simultaneously
5. **Recording Scheduling** - Add time-based recording functions for surveillance applications

## Required Dependencies

For this implementation to work correctly, the following packages must be included in your project:

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- LAV redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Troubleshooting Common Issues

When implementing IP camera capture, be prepared to address these common challenges:

1. **Connection Failures** - Verify network connectivity, camera credentials, and firewall settings
2. **Stream Performance** - Balance quality settings with available bandwidth and processing power
3. **Output File Errors** - Ensure adequate disk space and appropriate write permissions
4. **Resource Leaks** - Monitor memory usage during long recording sessions
5. **Camera Compatibility** - Different camera models may require specific configuration adjustments

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.