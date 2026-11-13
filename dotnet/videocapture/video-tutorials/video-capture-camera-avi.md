---
title: Record Webcam to AVI in C# WinForms (MJPEG, PCM, MP3)
description: Record a webcam to AVI in a C# WinForms app with VideoCaptureCore - pick the video codec, PCM or MP3 audio, and check the file with ffprobe.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - AVI
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoView
video:
  id: f5EXTFQETKA
  upload_date: 2026-11-12T15:20:03Z
  duration: PT10M56S

---

# Webcam to AVI File Recording in .NET C# Applications

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records a webcam to an AVI file in a C# WinForms app with the VideoCaptureCore engine. The video codec is picked from the ones installed on the PC (MJPEG by default), and the audio is uncompressed PCM or MP3.

**When AVI is the right choice over MP4:** with MJPEG, every frame is a whole picture, so you can cut and edit on any frame and read each frame on its own for analysis, and PCM audio has no loss. The price is size: about twenty megabytes every second at 1080p. For the web, phones and sharing, record [MP4 with H.264](video-capture-webcam-mp4.md) instead.

This tutorial uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine. On macOS, Linux, Android and iOS, VideoCaptureCoreX records AVI as well: see [AVI output](../../general/output-formats/avi.md) and [Save Webcam Video in C#](../guides/save-webcam-video.md).

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/f5EXTFQETKA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-avi-winforms)

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the video capture redist with its DirectShow filters. AVI needs nothing more:

- For 64-bit applications: [x64 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- For 32-bit applications: [x86 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Set `PlatformTarget` in the project file to match the redist (`x64` or `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the camera, format, frame rate, microphone and video codec, an "MP3 audio" check box, and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamAvi;

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
        cbMP3.Enabled = cbMicrophone.Items.Count > 0;

        // The video codecs installed on this PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
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
        if (cbFrameRate.SelectedItem is null)
        {
            Text = "No camera, format or frame rate";
            return;
        }

        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Webcam to AVI";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        // Audio: MP3, or uncompressed PCM
        var avi = cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

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
        // Finish the AVI before the window goes away
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

1. **Device and Format Selection** - The camera, its format, its frame rate and the microphone are picked from lists filled from the devices themselves; with no microphone, `Audio_RecordAudio` is set to `false` and the AVI holds video only
2. **Camera Format** - `Format_UseBest` is on by default: it picks the highest resolution up to 1080p, then prefers MJPG and the fastest frame rate. To record 4K, or any format you choose, turn it off and set `Format` and `FrameRate`
3. **Video Codec** - `Video_Codecs()` returns the DirectShow video compressors installed on the PC; `AVIOutput.Video_Codec` takes one of those names and defaults to `MJPEG Compressor`, which stores every frame as a JPEG picture. A codec you install yourself shows up in the same list
4. **Audio** - A plain `AVIOutput` writes uncompressed PCM, 16-bit stereo at 44.1 kHz, about ten megabytes a minute; `new AVIOutput(new MP3Output())` writes MP3 at 192 kbps, about one and a half megabytes a minute
5. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the form finishes the AVI file first

## Check the File with ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=1920|height=1080
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=14.883073|size=316108288
```

Fifteen seconds of 1080p MJPEG is about three hundred megabytes, roughly 170 megabits per second, which is why AVI is for editing and analysis, not for sharing.

## Video Transcript

### What we build

Let's capture a webcam to an AVI file in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms window with a live camera preview. It records an AVI file with any video codec installed on the PC, MJPEG by default. The audio is uncompressed PCM, or MP3 when you tick a box. The camera, its format, the frame rate and the microphone are picked from lists. This is the finished app, recording from a Logitech C920.

### AVI or MP4

When is AVI the right choice over MP4? With MJPEG, every frame is a whole picture, so you can cut and edit on any frame, and read each frame on its own for analysis. PCM audio has no loss. The price is size: here about twenty megabytes every second at 1080p. For the web, phones and sharing, MP4 with H.264 is the better choice. That's the webcam to MP4 video.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And a webcam with a microphone. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamAvi. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the SDK itself, and the video capture redist with its DirectShow filters. AVI needs nothing more. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. The redist is the x64 one, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redist instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format, the frame rate, the microphone and the video codec. Add a check box for MP3 audio, and the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the camera and format lists, and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### List the devices

Video_CaptureDevices returns every camera, and Audio_CaptureDevices every microphone. We list their names and select the first of each. Without a microphone, the MP3 box is disabled. When the camera changes, its handler fills the format list with the formats of that camera, and picks 1920x1080 MJPG when it's there. When the format changes, its handler fills the frame rate list the same way, and picks thirty frames per second.

### Choose the camera format

The Start handler first checks that a frame rate and a video codec are selected. If not, it says so in the title bar. Then it selects the camera by name, turns Format_UseBest off, and passes the format and the frame rate picked from the lists. Format_UseBest is on by default. It picks the highest resolution up to 1080p, then prefers MJPG and the fastest frame rate. To record 4K, or any format you choose, turn it off and set Format and FrameRate, as we do here.

### Select the microphone

With no microphone in the list, the app captures video only. Otherwise it records the selected one. Audio_PlayAudio is off, so the microphone isn't played through the speakers while you record.

### Choose the video codec

Back in the Load handler, Video_Codecs lists the video codecs installed on this PC. We fill the codec list and select MJPEG Compressor. Video_Codecs returns the DirectShow video compressors on this PC. This one has two: MJPEG Compressor and DV Video Encoder. MJPEG stores every frame as a JPEG picture. DV writes standard-definition DV only. A codec you install yourself shows up in the same list.

### AVIOutput: PCM or MP3 audio

Now the output. A plain AVIOutput writes uncompressed PCM audio. Pass an MP3Output to its constructor, and the audio is MP3 instead. Set Video_Codec to the name from the list, write output.avi into the Videos folder, and switch the engine to video capture mode, so it records and not just previews. A plain AVIOutput writes PCM: 16-bit stereo at 44.1 kilohertz, with no loss, about ten megabytes a minute. With an MP3Output it writes MP3 at 192 kilobits per second, about one and a half megabytes a minute. Next to MJPEG video, both are small.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finishes writing the AVI file.

### Close the window

Closing the window while it records would leave the AVI unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The camera, format and microphone lists fill in from the devices. The codec list shows the two codecs on this PC. MJPEG stays selected. Start begins the recording. The C920 takes a few seconds to send its first frame. Stop finishes output.avi, and then we close the window.

### Check the file with ffprobe

The file lands in Videos: about three hundred megabytes for fifteen seconds. ffprobe shows what's inside. The container is AVI. The video is MJPEG at 1920x1080. The audio is PCM, 16-bit, 44.1 kilohertz stereo. That's about 170 megabits per second, which is why AVI is for editing and analysis, not for sharing.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. A camera that's already open in another app. x86 and x64 packages mixed in one project. Closing the window mid-recording: await StopAsync first. And AVI where MP4 belongs: an hour of 1080p MJPEG runs to tens of gigabytes.

### Next steps

Next, the webcam to MP4 video saves the same camera to small H.264 files. There are also guides for IP cameras, screen recording and snapshots. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4](video-capture-webcam-mp4.md) — the same camera saved to small H.264 files
- [Screen Capture to AVI](screen-capture-avi.md) — record the screen to an AVI file
- [Camera Video Preview](camera-video-preview.md) — display live camera video with frame capture

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
