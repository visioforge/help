---
title: C# Screen Recording to WMV - .NET Implementation
description: Implement professional screen recording in .NET apps with C# using step-by-step guide, working code samples, and configuration options.
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

## Essential C# Implementation Code

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

## Advanced Configuration Options

### Capturing Specific Screen Regions

If you need to record only a portion of the screen rather than the entire display:

```csharp
// Define a specific rectangular region to capture (x, y, width, height)
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
    FullScreen = false,
    Rectangle = new Rectangle(0, 0, 800, 600) 
};
```

## Common Implementation Scenarios

### Creating a Lightweight Recording Application

For scenarios where system resources are limited:

1. Lower the capture frame rate
2. Record to a more efficient codec
3. Capture smaller screen regions
4. Use hardware acceleration when available

### Implementing Background Recording

For applications that need to record in the background:

1. Initialize the capture component in a separate thread
2. Implement minimal UI for control
3. Consider adding system tray functionality
4. Implement proper resource management

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.