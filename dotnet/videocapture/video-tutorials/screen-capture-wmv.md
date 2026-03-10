---
title: Screen Capture to WMV with Windows Media Codec in C# .NET
description: Record screen to WMV format with Windows Media codecs using VisioForge Video Capture SDK. Codec configuration and complete C# code examples included.
sidebar_label: Screen Capture to WMV
---

# Implementing Screen Recording to WMV in C# .NET Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Tutorial Walkthrough

Watch our detailed video walkthrough that demonstrates each step of the implementation process:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/8JYSDw2JeAo?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

Access the complete source code for this tutorial on our GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-wmv)

## Required Dependencies

Before you begin, ensure you have installed the necessary redistributable packages:

- Video capture redistributables:
  - [x86 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## When to Use WMV for Screen Recording

WMV (Windows Media Video) uses Microsoft's Windows Media codecs and the ASF container. It remains useful in specific Windows-centric scenarios:

- **Native Windows integration** — WMV files play in Windows Media Player without additional codecs and integrate with Windows Movie Maker and other Microsoft tools
- **ASF streaming** — The ASF container supports live streaming over Windows Media Services, useful for intranet broadcasting
- **Smaller files than AVI** — WMV compression is more efficient than MJPEG, though less efficient than H.264 MP4
- **Legacy enterprise environments** — Many corporate environments standardize on Windows Media formats for internal video distribution

**Trade-off:** WMV is a Windows-only format with limited support on macOS and Linux. For cross-platform compatibility, [MP4 is the recommended format](screen-capture-mp4.md).

## Modern API — Video Capture SDK X

The modern cross-platform API uses `VideoCaptureCoreX`. For the complete console application pattern with screen capture setup, audio configuration, and recording lifecycle, see the [Screen Capture to MP4](screen-capture-mp4.md#modern-api-video-capture-sdk-x) guide. To output WMV instead of MP4, replace the output configuration:

```csharp
// WMV output (Windows Media Video + WMA audio codecs)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

Region capture, multi-monitor recording, audio, cursor highlighting, and GPU encoding options are covered in the [MP4 guide](screen-capture-mp4.md) — all source configuration features work identically with WMV output.

## Legacy API — Video Capture SDK

### Code Example

The following code snippet demonstrates how to create a basic screen recording application that captures your screen to a WMV file:

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
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_wmv
{
    public partial class Form1 : Form
    {
        // Declare the main VideoCaptureCore object that will handle the screen recording
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Initialize form components (buttons, panels, etc.)
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the VideoCaptureCore instance and associate it with the VideoView control
            // VideoView1 is a UI control where the screen capture preview will be displayed
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configure screen capture to record the full screen
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { FullScreen = true };
            
            // Disable audio recording and playback
            videoCapture1.Audio_RecordAudio = videoCapture1.Audio_PlayAudio = false;
            
            // Set the output file path to the user's Videos folder
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
            
            // Set the output format to WMV with default settings
            videoCapture1.Output_Format = new WMVOutput();
            
            // Set the capture mode to screen capture
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;
            
            // Start the screen capture process asynchronously
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the screen capture process asynchronously
            await videoCapture1.StopAsync();
        }
    }
}
```

## Frequently Asked Questions

### When should I use WMV instead of MP4 for screen recording?

Choose WMV when your target audience uses Windows exclusively and you need native playback without additional codec installation, when distributing video through Windows Media Services or SharePoint, or when working within enterprise environments that standardize on Windows Media formats. For cross-platform distribution or web publishing, MP4 with H.264 is the better choice — it offers smaller files, broader device support, and better compression.

## See Also

- [Screen Capture to MP4](screen-capture-mp4.md) — recommended format with full feature coverage (region, multi-monitor, audio, GPU encoding, cross-platform)
- [Screen Capture to AVI](screen-capture-avi.md) — AVI format with MJPEG for frame-independent editing
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture settings
- [Code Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — additional screen capture code snippets on GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
