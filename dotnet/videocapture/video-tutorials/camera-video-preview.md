---
title: Webcam Frame Capture & Preview in C#
description: Learn how to implement real-time webcam video preview with frame capture functionality in C# .NET applications. Complete tutorial with working code examples for WinForms, WPF, and console applications. Perfect for building custom camera solutions.
sidebar_label: Webcam Frame Capture & Preview

---

# Implementing Webcam Video Preview with Frame Capture in C#

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net)

## Overview

This tutorial demonstrates how to create a professional webcam video preview application with the ability to capture individual frames as images. This functionality is essential for developing applications that require image analysis, snapshot capabilities, or custom camera interfaces.

## Step-by-Step Video Tutorial

Watch our detailed video walkthrough that covers all aspects of webcam integration:

[!embed](https://www.youtube.com/embed/kxC6JrJddek?controls=1)

## Getting Started

Before diving into the code, you'll need to set up your development environment with the necessary dependencies. The complete source code is available on GitHub for reference:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-preview-webcam-frame-capture)

## Implementation Guide

### Required Dependencies

First, ensure you have the following NuGet packages installed:

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Source Code Example

The following C# implementation demonstrates how to:

1. Initialize a video capture component
2. Connect to a webcam device
3. Display real-time video preview
4. Capture and save individual frames as JPEG images

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// VisioForge SDK references for video capture functionality
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace video_preview_webcam_frame_capture
{
    public partial class Form1 : Form
    {
        // Main video capture component to handle camera input
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        // Form initialization - creates a new VideoCaptureCore instance connected to our video view
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the video capture component with the VideoView control
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        // Start button click handler - configures and starts video capture
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configure the default video capture device (first available camera)
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configure the default audio capture device (first available microphone)
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Set mode to VideoPreview (display only, no recording)
            videoCapture1.Mode = VideoCaptureMode.VideoPreview;

            // Start the video preview asynchronously
            await videoCapture1.StartAsync();
        }

        // Stop button click handler - stops the video capture
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the video preview asynchronously
            await videoCapture1.StopAsync();
        }

        // Save Frame button click handler - captures and saves current frame as JPEG
        private async void btSaveFrame_Click(object sender, EventArgs e)
        {
            // Save the current frame as a JPEG image in the user's Pictures folder
            // The quality parameter (85) specifies the JPEG compression level (0-100)
            await videoCapture1.Frame_SaveAsync(
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "frame.jpg"),
                ImageFormat.Jpeg,
                85);
        }
    }
}
```

## Code Breakdown

### Initialization

The application begins by creating a new `VideoCaptureCore` instance during form load, which serves as the main interface for interacting with webcam devices.

### Device Selection

When the user clicks the Start button, the application automatically selects the first available video and audio capture devices. In a production environment, you might want to provide a dropdown selection for users with multiple cameras.

### Preview Mode

The application is configured in `VideoPreview` mode, which enables real-time display without recording the stream to disk. This is ideal for applications that only need to show camera output.

### Frame Capture

The frame capture functionality demonstrates how to save the current video frame as a JPEG image with a specified quality level. The image is saved to the user's Pictures folder by default.

## Advanced Applications

This code can be extended to support various real-world applications:

- Document scanning
- Photo booth applications
- Video conferencing
- Computer vision and image processing
- Security and surveillance systems

## Additional Resources

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to explore more code samples and advanced implementation techniques.
