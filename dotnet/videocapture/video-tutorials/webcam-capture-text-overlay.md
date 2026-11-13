---
title: Webcam Text Overlay in C# WinForms (Date, Frame Number)
description: Draw a caption you can change while recording, the date and time, the frame number and the recording time over a webcam MP4 in C# WinForms.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - MP4
  - Text Overlay
  - Video Effects
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEffectTextLogo
  - VideoCaptureSource
  - AudioCaptureSource
  - MP4Output
  - VideoView
video:
  id: wgRV_RT-txg
  upload_date: 2026-11-12T21:13:37Z
  duration: PT13M6S

---

# Webcam Capture with a Text Overlay in C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records a webcam to MP4 in a C# WinForms app with the VideoCaptureCore engine and draws four text overlays into the video frames with `VideoEffectTextLogo`:

- a caption at the bottom center, which you can change while recording;
- the date and time in the top-right corner;
- the frame number;
- the time since the recording started.

The text is part of every frame, so it is in the live preview and in the file. A date and time on each frame is what security and evidence recordings need, a frame number lets you find and compare an exact frame later, and the recording time lines the video up with logs and sensor data.

This tutorial uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine. For fades, rotation and the cross-platform X engines, see [Text overlays](../../general/video-effects/text-overlay.md).

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/wgRV_RT-txg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-text-overlay-winforms)

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package, the video capture redist with its DirectShow filters, and the MP4 redist, which writes the MP4 file:

- For 64-bit applications: [x64 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/) and [x64 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)
- For 32-bit applications: [x86 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) and [x86 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)

Set `PlatformTarget` in the project file to match the redists (`x64` or `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.MP4.x64
```

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the camera, format, frame rate and microphone, a caption text box with an Apply button that starts disabled, and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamTextOverlay;

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

    private void AddOverlays()
    {
        videoCapture1.Video_Effects_Enabled = true;
        videoCapture1.Video_Effects_Clear();

        // A caption at the bottom, changed with Apply while recording
        using var font = new Font("Segoe UI", 40, FontStyle.Bold); // the effect keeps its own copy
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "caption")
        {
            Text = tbCaption.Text,
            Font = font,
            FontColor = Color.Yellow,
            HorizontalAlignment = TextLogoHorizontalAlignment.Center,
            VerticalAlignment = TextLogoVerticalAlignment.Bottom
        });

        // Date and time in the top-right corner
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime,
            DateTimeMask = "yyyy-MM-dd HH:mm:ss",
            Text = "",
            HorizontalAlignment = TextLogoHorizontalAlignment.Right,
            VerticalAlignment = TextLogoVerticalAlignment.Top
        });

        // The frame number in the top-left corner
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "frame")
        {
            Mode = TextLogoMode.FrameNumber,
            Text = "Frame ",
            Left = 20,
            Top = 20
        });

        // Time since the recording started, under the frame number
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "time")
        {
            Mode = TextLogoMode.Timestamp,
            TimeStampMask = @"hh\:mm\:ss\.fff",
            Text = "Rec ",
            Left = 20,
            Top = 140 // the text size follows the Windows display scale
        });
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam with text overlay";
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

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.mp4");
        videoCapture1.Output_Format = new MP4Output(); // H.264 and AAC
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        AddOverlays();

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btApply.Enabled = started;
        btStart.Enabled = !started;
    }

    private void btApply_Click(object sender, EventArgs e)
    {
        // Change the caption while recording: new text, then Update to redraw it
        if (videoCapture1.Video_Effects_Get("caption") is VideoEffectTextLogo caption)
        {
            caption.Text = tbCaption.Text;
            caption.Update();
        }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btApply.Enabled = false;
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
            btApply.Enabled = false;
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

1. **Effects on** - `Video_Effects_Enabled` is `false` by default, and then no overlay is drawn. `Video_Effects_Clear` removes the overlays of the previous recording, so Start can run again
2. **Placement** - `HorizontalAlignment` and `VerticalAlignment` put an overlay at an edge or in the center whatever the frame size; without them, `Left` and `Top` place it in video pixels. With nothing set, an overlay is white Arial 36 in the top-left corner
3. **Modes** - `Mode` picks what an overlay shows: `Text` draws the `Text` property, `DateTime` the PC clock formatted by `DateTimeMask`, `Timestamp` the time since the recording started formatted by `TimeStampMask` (a `TimeSpan` format, so `:` and `.` need a backslash), and `FrameNumber` the number of the frame. In the last three, `Text` is drawn in front of the value, and its default is `Hello!!!`, so set it to an empty string or to your own prefix
4. **Changing an overlay while recording** - `Video_Effects_Get` returns the overlay with that name, or `null`. Change its properties and call `Update()`: the engine redraws it from the next frame, and the recording keeps the change from that frame on. Without `Update()`, the old text stays
5. **Fonts** - the `Font` setter keeps its own copy of the font, so the one you pass can be disposed right away
6. **Text size and the display scale** - a font size in points is drawn at the screen's DPI, so it grows with the Windows display scale and does not shrink with the frame: at 200 percent, a line of Arial 36 is about 110 pixels tall whatever the frame size. This sample leaves room for that with `Top = 140`, which fits up to 200 percent. For the same size on every PC, create the font in pixels: `new Font("Arial", 36, GraphicsUnit.Pixel)`
7. **Cost** - an overlay whose text changes (the clock, the frame number, the recording time, or a caption after `Update()`) is redrawn on every frame; keep such overlays few

## Check the File with ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.mp4 -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=h264|width=1920|height=1080
# stream|codec_name=aac|sample_rate=48000|channels=2
# format|format_name=mov,mp4,m4a,3gp,3g2,mj2|duration=30.698667|size=4843264
```

## Video Transcript

### What we build

Let's draw text over a webcam recording in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms app that records a webcam to MP4 with a live preview. Over the video it draws a caption that you can change while it records, the date and time, the frame number, and the time since the recording started. This is the finished app, recording from a Logitech C920. The text is drawn into the video frames, so it's in the preview and in the file. Where does this help? A date and time on every frame is what security and evidence recordings need. A frame number lets you find and compare an exact frame later. The recording time lines the video up with logs and sensor data. And a caption labels what's on camera, like the operator, the test or the station, and you can change it as things change.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And a webcam, with a microphone if you want sound. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamTextOverlay. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add three NuGet packages: the SDK itself, the video capture redist with its DirectShow filters, and the MP4 redist, which writes the MP4 file. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. And the MP4 redist for x64. Both redists are x64, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redists instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format, the frame rate and the microphone. Then a label and a text box for the caption, and an Apply button. Apply starts disabled, because it only works while recording. Add the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the camera and format lists, and the three buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. The text overlay types live in VideoEffects. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### List the devices

Video_CaptureDevices returns every camera, and Audio_CaptureDevices every microphone. We list their names and select the first of each. When the camera changes, its handler fills the format list with the formats of that camera, and picks 1920x1080 MJPG when it's there. When the format changes, its handler fills the frame rate list the same way, and picks thirty frames per second.

### Camera, microphone and MP4

The Start handler first checks that a frame rate is selected. Then it selects the camera by name, turns Format_UseBest off, and passes the format and the frame rate from the lists. Left on, Format_UseBest picks the highest resolution up to 1080p by itself. With no microphone, the app captures video only. Otherwise it records the selected one, without playing it through the speakers. The output is output.mp4 in the Videos folder. A plain MP4Output writes H.264 video and AAC audio, encoded on the CPU, or with Intel Quick Sync where the PC has it.

### Draw text over the video

The overlays get a method of their own. Video_Effects_Enabled turns video effects on: it's off by default, and then nothing is drawn. Video_Effects_Clear removes the overlays of the previous recording, so Start can run again. The first overlay is the caption: a VideoEffectTextLogo named caption, with the text from the text box. It's Segoe UI, 40 points, bold and yellow. The two alignments put it at the bottom center, whatever the frame size. The effect keeps its own copy of the font, so ours is disposed right away. Every text overlay is a VideoEffectTextLogo. Its name lets you find it again later. Text, Font and FontColor set what it draws. Left and Top place it in video pixels, or the alignment properties put it at an edge or in the center. Without them it's white Arial at 36 points, in the top-left corner.

### Text modes: clock, frame, time

Next, the date and time. Mode DateTime draws the PC clock, formatted by DateTimeMask with the usual .NET format strings. Text is drawn in front of the value, so we set it to an empty string. Its default is Hello. Right and Top alignment put the clock in the top-right corner. Mode picks what an overlay shows. Text draws the Text property. DateTime draws the PC clock. Timestamp draws the time since the recording started, formatted by TimeStampMask. FrameNumber draws the number of the frame. In the last three, Text goes in front of the value. The frame number goes in the top-left corner, with the prefix Frame. This time Left and Top place it, in pixels. And under it, the recording time, with the prefix Rec, in hours, minutes, seconds and milliseconds. In a TimeSpan format, the colons and the dot need a backslash. Leave room between the rows: the text size follows the Windows display scale, so at 200 percent it's twice as tall.

### Start and stop

Back in the Start handler, AddOverlays runs before StartAsync. StartAsync returns once the capture is running, and then Stop and Apply are enabled. Stop disables Stop and Apply, and StopAsync finishes writing the MP4 file.

### Change the caption while recording

Apply changes the caption while recording. Video_Effects_Get finds the overlay by its name. We set the new text and call Update, which tells the engine to draw the overlay again on the next frame. Without Update, the old text stays. Video_Effects_Get returns the overlay you added, or null when no overlay has that name. You can change any of its properties, like the text, the color or the position. Then call Update, and it's redrawn from the next frame. The recording keeps the change from that frame on.

### Close the window

Closing the window while it records would leave the MP4 unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The camera, format and microphone lists fill in from the devices. Start begins the recording. The C920 takes a few seconds to send its first frame. Then the caption, the clock, the frame number and the recording time appear over the video. Type a new caption and click Apply. The new text shows on the next frame. Stop finishes output.mp4, and then we close the window.

### Check the file

Here's a frame from output.mp4, a few seconds in. The caption, the clock, the frame number and the recording time are part of the picture. And a frame after Apply. From that frame on, the file has the new caption. ffprobe shows what's inside: H.264 video at 1920x1080, and AAC audio, in an MP4 file.

### Common mistakes

A few common mistakes. Video_Effects_Enabled left false, so nothing is drawn. A clock that starts with Hello, because Text wasn't cleared. A new Text without Update, so the old text stays. Video_Effects_Get with a different name, which returns null. And many overlays whose text changes: each one is redrawn on every frame, so keep them few.

### Next steps

Next, the webcam to MP4 video encodes on the GPU, and the webcam to AVI video makes files for editing. For pictures over the video there's VideoEffectImageLogo, and for a moving ticker, VideoEffectScrollingTextLogo. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4](video-capture-webcam-mp4.md) — the same camera saved to MP4, with GPU encoding
- [Webcam to AVI](video-capture-camera-avi.md) — MJPEG and PCM files for editing and analysis
- [Text overlays](../../general/video-effects/text-overlay.md) — fades, rotation and flips, and text overlays in the cross-platform X engines

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
