---
title: Record Webcam to WMV in C# WinForms (WMV 9, WMA, CBR)
description: "Record a webcam to WMV in a C# WinForms app with VideoCaptureCore - Windows Media Video 9 at a constant bitrate, WMA audio, and an ffprobe check."
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - WMV
  - WMA
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - AudioCaptureSource
  - WMVOutput
  - WMVStreamMode
  - VideoView
video:
  id: EoOfXkhOtP8
  upload_date: 2026-11-14T07:13:25Z
  duration: PT12M20S

---

# Webcam to WMV File Recording in .NET C# Applications

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records a webcam to a WMV file in a C# WinForms app with the VideoCaptureCore engine. The video is Windows Media Video 9 at the camera's own resolution and a constant 8 Mbps; the audio is WMA, in a format picked from the list the SDK returns.

**When WMV is the right choice over MP4:** a WMV file plays on Windows out of the box, and the Windows Media encoders come with Windows, so you need no GPU and no codec pack. With a constant bitrate, the file size is known before you record, and some systems already expect WMV or ASF files. For the web, phones and Macs, record [MP4 with H.264](video-capture-webcam-mp4.md) instead.

This tutorial uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine. VideoCaptureCoreX, the cross-platform engine, has its own WMV encoder: see [Cross-platform WMV output](../../general/output-formats/wmv.md#cross-platform-wmv-output) and [Save Webcam Video in C#](../guides/save-webcam-video.md).

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/EoOfXkhOtP8?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-wmv-winforms)

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the video capture redist with its DirectShow filters. The Windows Media encoders come with Windows, so nothing more is needed.

Set `PlatformTarget` in the project file to match the redist (`x64` or `x86`). On the N editions of Windows, the Windows Media components come with the Media Feature Pack: install it first.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

For a 32-bit (x86) application, use the x86 package instead: `VisioForge.DotNet.Core.Redist.VideoCapture.x86`.

`VisioForge.DotNet.VideoCapture`, the package in the Try it block at the top of this page, works as well: it installs the same `VisioForge.DotNet.Core`. The redist packages are needed either way.

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the camera, format, frame rate, microphone, video codec, audio codec and audio format, and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamWmv;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

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

        // The Windows Media codecs that can encode at a constant bitrate
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
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

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // The sample rates, channels and bitrates this audio codec offers
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 48000 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
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

        var mic = cbMicrophone.SelectedItem as string;
        if (mic is not null && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Webcam to WMV";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        var frameRate = (VideoFrameRate)cbFrameRate.SelectedItem;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = frameRate;

        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 8000, // kbps
            Custom_Video_SizeSameAsInput = true, // the camera's resolution
            Custom_Video_FrameRate = frameRate.Value,
            Custom_Video_KeyFrameInterval = 2, // seconds

            Custom_Audio_StreamPresent = mic is not null,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
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
        // Finish the WMV before the window goes away
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

1. **Device and Format Selection** - The camera, its format, its frame rate and the microphone are picked from lists filled from the devices themselves; with no microphone, `Audio_RecordAudio` and `Custom_Audio_StreamPresent` are `false` and the WMV holds video only
2. **Camera Format** - `Format_UseBest` is on by default: it picks the highest resolution up to 1080p, then prefers MJPG and the fastest frame rate. To record 4K, or any format you choose, turn it off and set `Format` and `FrameRate`
3. **Windows Media Codecs** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` and `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` return the Windows Media encoders that can work at a constant bitrate, and `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` the formats of one audio codec, each named by its bitrate, sample rate, channels and bits. Windows Media Video 9 is the codec for camera video; the Screen and Image codecs are made for screen recordings and still pictures, Advanced Profile is the VC-1 profile made for interlaced and broadcast video, and V7 and V8 are older versions for old players. Take every codec and format name from these lists, never type it by hand
4. **Custom Settings** - A plain `new WMVOutput()` uses the built-in "Windows Media Video 9 High (2.5 Mbps)" profile from SDK 2026.11.16 on; up to 2026.11.13 it was "Windows Media Video 9 for Local Network (768 kbps)", the profile the video names. `WMVMode.CustomSettings` takes your own codec, bitrate and size: `Custom_Video_Bitrate` is in kilobits per second, so `8000` is 8 Mbps, `Custom_Video_SizeSameAsInput` keeps the camera's resolution, and `Custom_Video_KeyFrameInterval` is in seconds. With 128 kbps audio, that is about sixty megabytes a minute
5. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the form finishes the WMV file first

## Check the File with ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=1920|height=1080
# stream|codec_name=wmav2|sample_rate=48000|channels=2
# format|format_name=asf|duration=16.496000|size=16941531
```

Sixteen and a half seconds come to 16,941,531 bytes, about 16 MB as File Explorer shows it: 8 Mbps of video plus 128 kbps of audio. ffprobe calls WMV 9 `wmv3` and WMA 9.2 `wmav2`, and the container is ASF, the format behind WMV.

## Video Transcript

### What we build

Let's record a webcam to a WMV file in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms window with a live camera preview. It records Windows Media Video 9 at the camera's own resolution, with a constant bitrate we set ourselves: eight megabits per second. The audio is WMA, in a format picked from a list. The camera, its format, the frame rate and the microphone are picked from lists too. This is the finished app, recording from a Logitech C920.

### WMV or MP4

When is WMV the right choice? A WMV file plays on Windows out of the box. The Windows Media encoders come with Windows, so you need no GPU and no codec pack. With a constant bitrate, the file size is known before you record. And some systems already expect WMV or ASF files. For the web, phones and Macs, MP4 with H.264 is the better choice. That's the webcam to MP4 video.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And a webcam with a microphone. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamWmv. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the SDK itself, and the video capture redist with its DirectShow filters. The WMV encoders come with Windows, so that's all. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. The redist is the x64 one, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redist instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format, the frame rate and the microphone, then for the video codec, the audio codec and the audio format. Add the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the camera, format and audio codec lists, and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### List the devices

Video_CaptureDevices returns every camera, and Audio_CaptureDevices every microphone. We list their names and select the first of each. When the camera changes, its handler fills the format list with the formats of that camera, and picks 1920x1080 MJPG when it's there. When the format changes, its handler fills the frame rate list the same way, and picks thirty frames per second.

### Windows Media codecs

Back in the Load handler, WMV_CustomProfile_VideoCodecs and WMV_CustomProfile_AudioCodecs list the Windows Media encoders that can work at a constant bitrate. We fill both lists, and select Windows Media Video 9 and Windows Media Audio 9.2. When the audio codec changes, WMV_CustomProfile_AudioFormats lists what that codec offers. We pick 128 kilobits per second, 48 kilohertz stereo. Windows Media Video 9 is the codec for camera video. The Screen and Image codecs are made for screen recordings and still pictures, and V7 and V8 are older versions, for old players. For audio at a constant bitrate there's WMA 9.2 and WMA 10 Professional. Each audio format names its bitrate, sample rate, channels and bits. Always pick these names from the lists, never type them by hand.

### Choose the camera format

The Start handler first checks that a frame rate and a video codec are selected. If not, it says so in the title bar. With a microphone in the list, it needs an audio format as well. Then it selects the camera by name, turns Format_UseBest off, and passes the format and the frame rate picked from the lists. Format_UseBest is on by default. It picks the highest resolution up to 1080p, then prefers MJPG and the fastest frame rate. To record 4K, or any format you choose, turn it off and set Format and FrameRate, as we do here.

### Select the microphone

With no microphone in the list, the app captures video only. Otherwise it records the selected one. Audio_PlayAudio is off, so the microphone isn't played through the speakers while you record.

### WMVOutput: custom settings

Now the output. A WMVOutput in CustomSettings mode takes our own settings. The video stream uses the codec from the list at a constant 8000 kilobits per second. SizeSameAsInput keeps the camera's resolution, the frame rate is the one we picked, and there's a keyframe every two seconds. The audio stream takes the codec and the format from the lists, and it's there only when we have a microphone. Then write output.wmv into the Videos folder, and switch the engine to video capture mode, so it records and not just previews. A plain WMVOutput uses a built-in profile at 768 kilobits per second, made for a local network. (From SDK 2026.11.16 on, the default is "Windows Media Video 9 High (2.5 Mbps)".) CustomSettings gives us our own codec, bitrate and size. Custom_Video_Bitrate is in kilobits per second, so 8000 means eight megabits. With 128 kilobits of audio, that's about sixty megabytes a minute. And a keyframe every two seconds gives the player a seek point every two seconds.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finishes writing the WMV file.

### Close the window

Closing the window while it records would leave the WMV unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The camera, format and microphone lists fill in from the devices. The video codec list shows the Windows Media encoders. Windows Media Video 9 stays selected. Start begins the recording. The C920 takes a few seconds to send its first frame. Stop finishes output.wmv, and then we close the window.

### Check the file with ffprobe

The file lands in Videos: about sixteen megabytes for sixteen seconds. ffprobe shows what's inside. The container is ASF, the format behind WMV. The video is WMV 9, which ffprobe calls wmv3, at 1920x1080. The audio is WMA, 48 kilohertz stereo.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. Codec and format names typed by hand: take them from the lists. Custom_Video_Bitrate written in bits per second: it's in kilobits. On the N editions of Windows, the Windows Media components come with the Media Feature Pack, so install it first. And closing the window mid-recording: await StopAsync first.

### Next steps

Next, the webcam to MP4 video saves the same camera to H.264 files that play everywhere. There are also guides for IP cameras, screen recording and snapshots. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4](video-capture-webcam-mp4.md) — the same camera saved to H.264 files that play everywhere
- [Webcam to AVI](video-capture-camera-avi.md) — every frame a whole picture, for editing and analysis
- [WMV output](../../general/output-formats/wmv.md) — every WMVOutput mode: internal, custom and external profiles

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
