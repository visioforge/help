---
title: Webcam to AVI Recording in .NET C# Applications
description: Learn how to implement webcam to AVI file recording in your .NET applications with this step-by-step tutorial. Includes complete C# code example with async operations, device selection, and proper implementation patterns for Windows Forms applications.
sidebar_label: Webcam to AVI Recording

---

# Webcam to AVI File Recording in .NET C# Applications

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net)

## Overview

This tutorial demonstrates how to capture video from a webcam and save it directly to an AVI file format in your .NET applications. The implementation uses asynchronous programming techniques for smooth UI responsiveness and provides a clean architecture approach suitable for professional software development.

## Video Walkthrough

Watch our detailed video tutorial covering the implementation process:

[!embed](https://www.youtube.com/embed/8yFKz1QOJbk?controls=1)

## Source Code Repository

Access the complete source code from our official GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-webcam-avi)

## Required Dependencies  

Before implementing this solution, ensure you have installed the necessary dependencies:

- Video capture redistributables:
  - [x86 Version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 Version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Implementation Details

### Prerequisites

Before starting, make sure you have:

- Visual Studio with .NET development tools
- A webcam connected to your development machine
- Basic understanding of Windows Forms applications

### Code Implementation

The following example demonstrates a complete implementation for webcam capture to AVI file:

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

// Import VisioForge SDK namespaces
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_capture_webcam_avi
{
    public partial class Form1 : Form
    {
        // Declare the VideoCaptureCore object that will handle all capture operations
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            // Standard Windows Forms initialization
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the VideoCaptureCore object and bind it to the VideoView control
            // VideoView1 is a control that displays the video preview
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Set the video capture device - using the first available camera
            // videoCapture1.Video_CaptureDevices() returns a list of available video devices
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Set the audio capture device - using the first available microphone
            // videoCapture1.Audio_CaptureDevices() returns a list of available audio devices
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Set the output filename to "output.avi" in the user's Videos folder
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");

            // Configure output format as AVI
            // By default, this uses MJPEG codec for video and PCM for audio
            videoCapture1.Output_Format = new AVIOutput();
            
            // Set the mode to VideoCapture (other modes include ScreenCapture, AudioCapture, etc.)
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;

            // Start the capture process asynchronously 
            // Using async/await pattern for non-blocking UI
            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the capture process asynchronously
            await videoCapture1.StopAsync();
        }
    }
}
```

### Key Implementation Points

#### Device Selection

The example automatically selects the first available video and audio devices. In a production application, you might want to present users with a list of available devices for selection.

#### File Output Configuration

The example saves the output to an AVI file in the user's Videos folder. You can customize the path and filename according to your application requirements.

#### Asynchronous Operation

The implementation uses async/await pattern to prevent UI freezing during capture operations, which is critical for maintaining a responsive application experience.

## Advanced Customization Options

The SDK provides numerous customization options including:

- Video resolution and framerate settings
- Audio quality configuration
- Codec selection for different output requirements
- Video effects and transformations

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and explore additional implementation scenarios.
