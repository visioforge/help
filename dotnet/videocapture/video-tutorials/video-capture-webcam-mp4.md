---
title: Record Webcam to MP4 with H.264 Encoding in C# .NET
description: Enumerate webcams, encode H.264/AAC, and record to MP4 using VisioForge Video Capture SDK. Async capture with WinForms preview and C# code examples.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - MP4
  - H.264
  - AAC
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4HWOutput
  - VideoView
video:
  id: fAt1Bp6qYPc
  upload_date: 2026-11-03T13:29:29Z
  duration: PT7M40S

---

# Webcam to MP4 Video Capture Implementation in .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Webcam Capture in .NET Applications

Creating applications that record video from webcams to MP4 files is a common requirement for many software projects. This tutorial provides a developer-focused approach to implementing this functionality using modern .NET techniques and the Video Capture SDK.

When implementing webcam recording capabilities, developers need to consider:

- Selecting the appropriate video and audio input devices
- Configuring video and audio compression settings
- Managing the capture lifecycle (initialization, start, stop)
- Handling output file creation and management

This tutorial uses the Windows-only VideoCaptureCore engine. [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) also records webcams to MP4 on macOS, Linux, Android and iOS with VideoCaptureCoreX — see [Save Webcam Video in C#](../guides/save-webcam-video.md) and the [Android](../../deployment-x/Android.md) and [iOS](../../deployment-x/iOS.md) deployment guides.

## Video Implementation Tutorial

The following video demonstrates the complete process of setting up a webcam capture application:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fAt1Bp6qYPc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

For developers who prefer to examine the complete implementation, all source code is available in our GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-winforms)

## Required Redistributable Components

Before implementing the solution, install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the necessary redistributable packages:

### Video Capture Components

- For 32-bit applications: [x86 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- For 64-bit applications: [x64 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### MP4 Encoding Components

- For 32-bit applications: [x86 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- For 64-bit applications: [x64 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Implementation Example with C# Code

The following C# implementation demonstrates how to create a Windows Forms application that captures video from a webcam and saves it to an MP4 file. The form has a `VideoView`, combo boxes for the camera, format, frame rate and microphone, and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4;

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

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
    }

    private void cbCamera_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == cbCamera.Text);
        var format = device.VideoFormats.First(f => f.Name == cbFormat.Text);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        var mp4 = new MP4HWOutput();

        // NVIDIA, then Intel, then AMD, then the CPU encoder
        var gpu = VideoCaptureCore.HWEncodersAvailable();
        mp4.Video.Codec =
            gpu.NVENC_H264 ? MFVideoEncoder.NVENC_H264 :
            gpu.QSV_H264 ? MFVideoEncoder.QSV_H264 :
            gpu.AMD_H264 ? MFVideoEncoder.AMD_H264 :
            MFVideoEncoder.MS_H264;

        mp4.Video.AvgBitrate = 6000; // kbps
        mp4.Video.MaxBitrate = 9000;
        mp4.Video.RateControl = MFCommonRateControlMode.PeakConstrainedVBR;
        mp4.Video.Profile = MFH264Profile.Main;
        mp4.Audio.Bitrate = 192; // AAC, kbps

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = mp4;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "text")
        {
            Text = "Hello World!", Left = 50, Top = 50, FontColor = Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finish the MP4 before the window goes away
        if (!btStart.Enabled)
        {
            e.Cancel = true;
            if (!btStop.Enabled) return; // already starting or stopping

            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close); // close again once this handler has returned
            return;
        }

        videoCapture1.Dispose();
    }
}
```

## Key Implementation Features

This implementation provides several important capabilities for developers:

1. **Device and Format Selection** - The camera, its format, its frame rate and the microphone are picked from lists filled from the devices themselves; with no microphone, `Audio_RecordAudio` is set to `false` and the MP4 holds video only
2. **Hardware Encoding with a CPU Fallback** - `MP4HWOutput` encodes H.264 on an NVIDIA, Intel or AMD GPU; `HWEncodersAvailable` tells which encoders the PC has, and `MS_H264` runs on the CPU when none is present
3. **Encoder Settings** - An average of 6000 kbps, peaks up to 9000 kbps, the Main profile and AAC audio at 192 kbps
4. **Text Overlay** - A fixed text and a clock that shows the current time, added as video effects
5. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the form finishes the MP4 file first

## Advanced Configuration Options

Developers can extend the solution with additional features:

- HEVC instead of H.264: `NVENC_H265`, `QSV_H265` or `AMD_H265`
- Other on-the-fly video effects and transformations
- Multiple audio track support

## Video Transcript

### What we build

Let's record a webcam to an MP4 file in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms window with a live camera preview. It records MP4, with H.264 encoded on the GPU when the PC has one, and AAC audio. The camera, its format and the microphone are picked from lists, and the whole app is about a hundred lines of C#. This is the finished app, recording from a Logitech BRIO.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. This video is about WinForms. The WPF version comes next.

### Create a WinForms project

In Visual Studio, open File, New, Project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamMp4. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add three NuGet packages: the SDK itself, the video capture redist and the MP4 redist. These are the x64 packages. An x86 app takes the x86 redists instead. You can also install the same three packages from the NuGet Package Manager.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format, the frame rate and the microphone. Add the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the combo boxes and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn.

### Enumerate cameras

Video_CaptureDevices returns every camera on the machine. We list their names and select the first one. When the camera changes, its handler fills the format list with the formats of that camera. When the format changes, its handler fills the frame rate list the same way. A camera reports its name, formats like 1920x1080 MJPG or 1280x720 YUY2, and the frame rates of each format. MJPG is compressed in the camera, so high resolutions fit through USB. YUY2 is uncompressed, so USB 2 carries only lower resolutions.

### Choose the camera format

Select the camera by name. Then turn Format_UseBest off, and pass the format and the frame rate picked from the lists. Format_UseBest is on by default. It picks the highest resolution the camera offers, up to 1080p, and prefers MJPG. An empty format falls back to it as well.

### Select the microphone

The microphone works the same way. Audio_CaptureDevices lists them, and we use the one picked in the list.

### MP4HWOutput: any GPU, CPU fallback

For the output, create an MP4HWOutput, write output.mp4 into the Videos folder, and switch the engine to video capture mode, so it records and not just previews. MP4HWOutput encodes on the GPU: NVENC on NVIDIA, Quick Sync on Intel, and AMF on AMD, in H.264 or HEVC. When there's no hardware encoder, the Windows encoder on the CPU takes over. The code and the file are the same on every PC. HWEncodersAvailable tells you which encoders this PC has. Take NVIDIA first, then Intel, then AMD, and fall back to the CPU encoder.

### Tune the H.264 encoder

Then tune the encoder: six thousand kilobits per second on average, peaks up to nine thousand, the Main profile, and AAC audio at a hundred and ninety-two. As a guide: 2500 to 4000 kilobits for 720p, 5000 to 8000 for 1080p, and 15000 to 25000 for 4K. Both bitrates are set in kilobits per second.

### Add a text overlay

Video effects add a text overlay: a fixed Hello World, and a clock that shows the current time on every frame.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finalizes the MP4 file. Run the app from Visual Studio. The format list comes straight from the camera. Start begins the recording, with the clock overlay ticking. Stop finalizes output.mp4. The file lands in Videos: 1920x1080, H.264 video and AAC audio.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. A camera that's already open in another app. x86 and x64 packages mixed in one project. And disposing the engine mid-capture: await StopAsync first. Engine errors arrive in the OnError event.

### Next steps

Next, we build the same app in WPF. There are also guides for IP cameras, screen recording, snapshots and network streaming. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4 in WPF](video-capture-webcam-mp4-wpf.md) — the same app as a WPF project, with the window in XAML
- [Pre-Event Recording](../guides/pre-event-recording.md) — circular buffer recording with motion detection triggers for webcams and IP cameras
- [Camera Video Preview](camera-video-preview.md) — display live camera video with frame capture

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.