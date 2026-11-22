---
title: Webcam Preview and Frame Capture to JPEG in C# WinForms
description: Show a live webcam preview in a C# WinForms app with VideoCaptureCore and save the current frame to a JPEG file with Frame_SaveAsync.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - Webcam
  - Snapshot
  - JPEG
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoCaptureSource
  - VideoView
video:
  id: Qp9m9xJ_QCA
  upload_date: 2026-11-15T13:01:09Z
  duration: PT10M16S

---

# Webcam Preview and Frame Capture in C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial shows a live webcam preview in a C# WinForms app with the VideoCaptureCore engine and saves the current frame to a JPEG file:

- the camera, its format and its frame rate are picked from lists;
- `VideoPreview` mode shows the camera without writing a file, so no encoder and no output format are needed;
- a Save Frame button writes the current frame with `Frame_SaveAsync`, at the camera's own resolution, to a new timestamped file in the Pictures folder;
- the app opens the saved file in the default image viewer.

Photo booths and ID photos, scanning a document or a label, inspection stations that keep a picture of each part, and any app that needs a still to process or to put into a report work this way.

This tutorial uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine.

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/Qp9m9xJ_QCA?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/camera-video-preview-winforms)

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the video capture redist with its DirectShow filters. A preview writes no file, so it needs no encoder package.

Set `PlatformTarget` in the project file to match the redist (`x64` or `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

For a 32-bit (x86) application, use the x86 package instead: `VisioForge.DotNet.Core.Redist.VideoCapture.x86`.

`VisioForge.DotNet.VideoCapture`, the package in the Try it block at the top of this page, works as well: it installs the same `VisioForge.DotNet.Core`. The redist packages are needed either way.

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the camera, format and frame rate, and Start, Stop and Save Frame buttons; Stop and Save Frame start disabled. The designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()`, which .NET Framework does not generate, with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`, and set `<dpiAware>true</dpiAware>` in an `app.manifest` to keep the system DPI awareness that .NET 10 sets.

```csharp
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamPreview;

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

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
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
        Text = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam preview";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        // Video only: audio playback is on by default
        videoCapture1.Audio_PlayAudio = false;
        videoCapture1.Mode = VideoCaptureMode.VideoPreview;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btSaveFrame.Enabled = started;
        btStart.Enabled = !started;
    }

    private async void btSaveFrame_Click(object sender, EventArgs e)
    {
        // A new JPEG in Pictures on every click, at the camera's own resolution
        var file = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
            $"frame-{DateTime.Now:yyyyMMdd-HHmmss-fff}.jpg");

        if (!await videoCapture1.Frame_SaveAsync(file, ImageFormat.Jpeg, 85)) return;

        // Open it in the default viewer; with no app for .jpg, the path in the title is all there is
        Text = file;
        try { Process.Start(new ProcessStartInfo(file) { UseShellExecute = true }); }
        catch (Win32Exception) { }
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        btSaveFrame.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Starting or stopping: wait for it before closing
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true;
            return;
        }

        // Dispose stops a running preview: there is no file to finish
        videoCapture1.Dispose();
    }
}
```

## Key Implementation Features

1. **Preview mode** - `Mode = VideoCaptureMode.VideoPreview` puts the camera on screen and writes no file, so there is no `Output_Filename` and no `Output_Format` to set. The frames still reach the app, which is what `Frame_SaveAsync` uses. Switch the mode to `VideoCapture`, add an output file and a format, and the same app records
2. **Camera format** - with `Format_UseBest` left on, the engine picks the highest resolution up to 1080p by itself; the sample turns it off and passes the format and the frame rate picked in the lists
3. **Audio** - `Audio_PlayAudio` is `true` by default and plays the audio source selected in `Audio_CaptureDevice` through the speakers, or, for a capture card whose video filter has an audio pin, the video source itself (`IsAudioSource`; a USB webcam microphone is a separate audio device). The sample shows video only, so it turns it off; left on with neither, the engine raises `OnError` ("The audio source is not configured"), sets `Audio_PlayAudio` to `false` and runs without sound
4. **Saving a frame** - `Frame_SaveAsync(filename, format, quality = 85, customWidth = 0, customHeight = 0)` writes the current frame in any `ImageFormat`, like JPEG, PNG, BMP or TIFF. The quality applies to JPEG only. Set both a width and a height to resize the frame; without them, the file has the camera's resolution (1920x1080 here), not the size of the preview on screen
5. **When saving fails** - `Frame_SaveAsync` returns `false` and raises `OnError` when the preview is not running or no frame has arrived yet. The sample enables Save Frame only after `StartAsync` returns `true`
6. **New file per click** - the file name carries the date and time down to the millisecond, so a frame never replaces the previous one
7. **Opening the file** - `Process.Start` with `UseShellExecute = true` opens the JPEG in the default image viewer. On a PC with no app for `.jpg` it throws a `Win32Exception`, and the path in the title bar is what is left
8. **Closing** - `Dispose()` stops a running preview, and a preview has no file to finish, so the close handler only waits while the preview is starting or stopping

## Frames in Memory

To work on a frame without a file:

- `Frame_GetCurrent()` returns the current frame as a `System.Drawing.Bitmap`;
- the `OnVideoFrameBitmap` event hands you every frame as it arrives.

## Video Transcript

### What we build

Let's show a live webcam preview in a C# WinForms app, and save frames from it, with the VisioForge Video Capture SDK. Here's what we build. A WinForms app with a live webcam preview. You pick the camera, its format and its frame rate from lists. A Save Frame button writes the current frame to a JPEG file, and the app opens that file in the default image viewer, so you can see what was saved. This is the finished app, previewing a Logitech C920 at 1920 by 1080. Nothing is recorded. The preview only shows the picture, and a frame is saved when you ask for one. Where does this help? Photo booths and ID photos. Scanning a document or a label with a webcam. Inspection stations that keep a picture of each part. And any app that needs a still to process, or to put into a report.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And a webcam. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamPreview. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the SDK itself, and the video capture redist with its DirectShow filters. A preview writes no file, so it needs no encoder package. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. The redist is x64, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redist instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format and the frame rate. Add the Start, Stop and Save Frame buttons. Stop and Save Frame stay disabled until the preview runs. Double-click the form, the camera and format lists, and the three buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. System.Diagnostics opens the saved file, System.ComponentModel has the exception it can throw, and System.Drawing.Imaging names the image format. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### List the camera formats

Video_CaptureDevices returns every camera. We list their names and select the first. When the camera changes, its handler fills the format list with the formats of that camera, and picks 1920x1080 MJPG when it's there. When the format changes, its handler fills the frame rate list the same way, and picks thirty frames per second.

### Start the preview

The Start handler first checks that a frame rate is selected. Then it selects the camera by name, turns Format_UseBest off, and passes the format and the frame rate from the lists. Left on, Format_UseBest picks the highest resolution up to 1080p by itself. Audio_PlayAudio is on by default: it plays the selected audio source through the speakers. This app shows video only, so it goes off. Left on with no audio source selected, the SDK reports an error and runs without sound. Mode VideoPreview shows the camera without writing a file. StartAsync returns once the preview is running, and then Stop and Save Frame are enabled. VideoPreview puts the camera on screen and writes no file, so there's no encoder and no output format to set. The frames still reach the app, which is what Save Frame uses. Switch the mode to VideoCapture, add an output file and a format, and the same app records.

### Save a frame

Save Frame builds a file name in the Pictures folder, with the date and time down to the millisecond, so every click makes a new file. Frame_SaveAsync writes the current frame there as a JPEG, at quality 85, and returns false when it can't. Then the path goes to the title bar, and Process.Start with UseShellExecute opens the file in the default image viewer. On a PC with no app for JPEG files, Process.Start throws a Win32Exception, and the path in the title is what's left. Frame_SaveAsync takes the file name and an image format, like JPEG, PNG, BMP or TIFF. The quality applies to JPEG only, and it's 85 by default. Set both a width and a height, and the frame is resized to them. Without them, the file has the camera's resolution, not the size of the preview on screen. It returns false, and raises OnError, when the preview isn't running or no frame has arrived yet.

### Stop and close

Stop disables Stop and Save Frame, and StopAsync stops the camera. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While the preview is starting or stopping, both buttons are disabled, and the handler waits by cancelling the close. Otherwise it disposes the engine. Dispose stops a running preview, and there's no file to finish.

### Run it

Run the app from Visual Studio. The camera and format lists fill in from the devices. Start begins the preview. The C920 takes a few seconds to send its first frame, and then the picture appears. Click Save Frame. The JPEG opens in Photos. Its size is 1920 by 1080, the camera's format, although the preview on the form is much smaller. Close Photos, stop the preview, and close the window.

### Common mistakes

A few common mistakes. Audio_PlayAudio left on with no audio source selected, so OnError reports a missing audio source. Saving before Start, or before the camera sends its first frame, so Frame_SaveAsync returns false. One file name for every click, so each frame replaces the last. And a quality value with PNG or BMP, where it's ignored.

### Next steps

Next, the webcam to MP4 video records what you preview, and the text overlay video draws a caption, a clock and frame numbers over it. To work on a frame in memory instead of a file, Frame_GetCurrent returns it as a Bitmap. And the OnVideoFrameBitmap event hands you every frame as it arrives. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4](video-capture-webcam-mp4.md) — record the same camera to an MP4 file
- [Webcam text overlay](webcam-capture-text-overlay.md) — draw a caption, a clock and frame numbers over the video
- [Pre-Event Recording](../guides/pre-event-recording.md) — buffer camera video and save clips on motion detection or a manual trigger

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
