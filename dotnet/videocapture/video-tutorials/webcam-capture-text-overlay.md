---
title: Add Text Overlay to Webcam Video in C# .NET
description: Capture webcam video and add custom text overlays in C# .NET with step-by-step instructions, complete code samples, and techniques.
---

# Adding Text Overlays to Webcam Video Capture in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Tutorial Walkthrough

The following video demonstrates the process of capturing video from a webcam and adding text overlays using C# and .NET:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/D_JPo9A9HMA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Access

Access the complete source code for this implementation on our GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/video-capture-text-overlay)

## Required Dependencies

To implement this functionality in your application, you'll need to install the following NuGet packages:

- **Video Capture Dependencies:**
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
  
- **MP4 Processing Dependencies:**
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Implementation Overview

This tutorial demonstrates how to:

- Access and capture video from a connected webcam
- Process the video stream in real-time
- Add customizable text overlays with color and positioning
- Save the captured video with text overlay to an MP4 file

The implementation uses Windows Forms for the user interface, but the core capture and text overlay functionality works with any .NET UI framework.

## Complete C# Implementation

The following code sample demonstrates a complete implementation of webcam capture with text overlay functionality:

```csharp
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;

namespace video_capture_text_overlay
{
    public partial class Form1 : Form
    {
        // The main video capture component that handles all capture functionality
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts the video capture with text overlay
        /// </summary>
        private async void btStart_Click(object sender, EventArgs e)
        {
            // Configure the video capture device - uses the first available camera
            videoCapture1.Video_CaptureDevice = new VideoCaptureSource(videoCapture1.Video_CaptureDevices()[0].Name);
            
            // Configure the audio capture device - uses the first available microphone
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(videoCapture1.Audio_CaptureDevices()[0].Name);
            
            // Set the capture mode to standard video capture
            videoCapture1.Mode = VideoCaptureMode.VideoCapture;
            
            // Set the output filename to save in the user's Videos folder
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
            
            // Configure MP4 as the output format - supports other formats as well
            videoCapture1.Output_Format = new MP4Output();

            // Add text overlay to the video
            // Step 1: Enable video effects
            videoCapture1.Video_Effects_Enabled = true;
            
            // Step 2: Clear any existing effects
            videoCapture1.Video_Effects_Clear();
            
            // Step 3: Create a new text overlay effect
            var textOverlay = new VideoEffectTextLogo(true) { 
                Text = "Hello World!",  // The text to display
                Top = 50,               // Y position (from top)
                Left = 50,              // X position (from left)
                FontColor = Color.Red   // Text color
            };
            
            // Step 4: Add the text overlay to the effects collection
            videoCapture1.Video_Effects_Add(textOverlay);

            // Start the capture process asynchronously
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Stops the video capture
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the capture process asynchronously
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Initialize the video capture component when the form loads
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the VideoCaptureCore with the video view control
            // VideoView1 should be a control on your form that implements IVideoView
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
            
            // Additional initialization options:
            // videoCapture1.Audio_PlayAudio = true;  // Play audio during capture
            // videoCapture1.Audio_RecordAudio = true;  // Record audio with video
        }
    }
}
```

## Key Implementation Details

### Device Selection and Configuration

The code automatically selects the first available camera and microphone devices for video and audio capture. In a production environment, you might want to provide users with options to select specific devices.

### Text Overlay Properties

The text overlay implementation supports various customization options:

- **Text Content**: Any string can be displayed on the video
- **Position**: Specify the exact coordinates for text placement
- **Color**: Choose any color for the text display
- **Size & Style**: Customize font, size, and style (commented options)

### Asynchronous Operation

The implementation uses C# async/await pattern for non-blocking video operations, ensuring your application remains responsive during capture.

### File Output

The captured video with text overlay is saved as an MP4 file in the user's Videos folder. The output format and location can be customized based on your application requirements.

## Additional Resources

For more examples and code samples related to video processing and manipulation, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
