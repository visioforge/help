---
title: Screen Capture to AVI - C# Tutorial for Developers
description: Implement screen capture with mouse cursor to AVI video files in C# with step-by-step guide and complete source code examples for recording.
---

# C# Screen Capture to AVI Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Tutorial Walkthrough

Watch our detailed tutorial that demonstrates the implementation process:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/AUT8oVPinUs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

Access the complete source code for this tutorial:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-avi)

## Implementation Code Sample

Below is the complete C# code implementation for capturing your screen to an AVI file:

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

namespace screen_capture_avi
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { FullScreen = true };
            videoCapture1.Audio_RecordAudio = videoCapture1.Audio_PlayAudio = false;
            videoCapture1.Output_Filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");

            // Default AVI output with MJPEG for video and PCM for audio
            videoCapture1.Output_Format = new AVIOutput(); 

            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

## Code Explanation

The implementation showcases:

- Capturing the entire screen with a simple configuration
- Saving the output to the user's Videos folder
- Using MJPEG compression for the AVI format
- Asynchronous start and stop methods for better application responsiveness

## Required Dependencies

To use this code in your project, install the following NuGet packages:

- Video capture redistributable components:
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Additional Resources

For more examples and advanced implementation techniques:

- Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for additional code samples
- Explore customization options for screen capture regions, video quality, and formats

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
