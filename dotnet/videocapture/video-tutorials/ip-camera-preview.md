---
title: IP Camera Preview in .NET - Setup Guide
description: Implement real-time IP camera preview in .NET apps with step-by-step tutorial and complete C# code examples for WinForms, WPF, Console.
---

# IP Camera Preview Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Walkthrough

This tutorial demonstrates how to set up IP camera preview functionality in your .NET applications:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/9n44ChQJT7s?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/ip-camera-preview)

## Required Redistributables  

Before you begin, ensure you have the following packages installed:

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- LAV redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.LAV.x64/)

## Implementation Example

Below is a complete WinForms example showing how to integrate IP camera preview functionality:

```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace ip_camera_preview
{
    public partial class Form1 : Form
    {
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Several engines are available. We'll use LAV as the most compatible. For low latency RTSP playback, use the RTSP Low Latency engine.
            videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
            {
                URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
                Type = IPSourceEngine.Auto_LAV
            };

            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Mode = VideoCaptureMode.IPPreview;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

## Key Implementation Details

### Setting the IP Camera Source

The code demonstrates configuring the IP camera source with the appropriate URL and engine type:

```csharp
videoCapture1.IP_Camera_Source = new IPCameraSourceSettings()
{
    URL = new Uri("http://192.168.233.129:8000/camera/mjpeg"),
    Type = IPSourceEngine.Auto_LAV
};
```

### Handling Audio Settings

For simple preview applications, you may want to disable audio playback and recording:

```csharp
videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;
```

### Setting the Capture Mode

The correct mode for IP camera preview is:

```csharp
videoCapture1.Mode = VideoCaptureMode.IPPreview;
```

## Advanced Options

For production applications, consider implementing:

- Error handling and connection retry logic
- UI feedback during connection attempts
- Camera authentication handling
- Frame rate and resolution control

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to explore more code samples.