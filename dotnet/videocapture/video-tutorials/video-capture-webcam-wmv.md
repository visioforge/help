---
title: Webcam to WMV Capture in .NET | Video SDK Tutorial
description: Implement webcam video capture to WMV format in .NET with step-by-step instructions, C# code examples, and integration best practices.
---

# Webcam Video Capture to WMV Format in .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Implementation Tutorial

This tutorial demonstrates how to create a Windows Forms application that captures video from a webcam and saves it in WMV format using the Video Capture SDK .NET. The example below provides a complete walkthrough with fully commented source code.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Bqss-zdalXg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Access

Access the complete project with all source files and additional examples on our GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-wmv)

## Required Dependencies  

Before implementing the code, ensure you have installed the necessary redistributable packages via NuGet:

- **Video Capture Redistributables:**
  - [x86 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 Package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Implementation Steps

The following sections outline the key steps to implement webcam capture functionality in your .NET application.

### Setup Project Structure

First, create a Windows Forms application and add the SDK references through NuGet. Then implement the form with necessary controls including a video preview area and start/stop buttons.

### C# Implementation Code

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_wmv
{
    public partial class Form1 : Form
    {
        // Main VideoCapture component instance
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Set up the default video capture device (webcam)
            // Using the first available device in the system
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Set up the default audio capture device (microphone)
            // Using the first available audio device in the system
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Set the output file path to the user's Videos folder
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");

            // Configure output format as WMV with default settings
            // WMV is a good choice for Windows compatibility
            videoCapture1.Output_Format = new WMVOutput();
            
            // Set the capture mode to VideoCapture (captures both video and audio)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Start the capture process asynchronously
            // This allows the UI to remain responsive during capture
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the capture process asynchronously
            // This ensures proper cleanup of resources
            await videoCapture1.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the VideoCaptureCore when the form loads
            // Connect it to the VideoView control on the form for preview
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

### Key Implementation Details

1. **Component Initialization**: The `VideoCaptureCore` is initialized when the form loads, connecting to the video preview control.

2. **Device Selection**: The code automatically selects the first available webcam and microphone devices on the system.

3. **Output Configuration**: WMV format is selected for its compatibility with Windows systems and wide platform support.

4. **Resource Management**: The `StopAsync()` method ensures proper cleanup of system resources when recording ends.

## Advanced Customization Options

The SDK provides additional options not shown in this basic example:

- Camera resolution and framerate settings
- Video quality and compression parameters
- Custom watermarks and overlays
- Multi-device capture capabilities

## Troubleshooting Tips

- Ensure your webcam is properly connected and recognized by Windows
- Verify the correct redistributables are installed for your target platform (x86/x64)
- Check Windows permissions for camera access in newer operating systems

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and advanced implementation examples.