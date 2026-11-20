---
title: Record the Screen to AVI in C# WinForms (MJPEG, PCM, MP3)
description: Record the screen and the speaker sound to AVI in a C# WinForms app with VideoCaptureCore - pick the monitor, frame rate and codec, check the file with ffprobe.
sidebar_label: Screen Capture to AVI
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - MJPEG
  - MP3
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - AVIOutput
  - MP3Output
  - VideoCaptureCoreX
  - MJPEGEncoderSettings
  - VideoView
video:
  id: V9iZiSx7slI
  upload_date: 2026-11-13T14:48:04Z
  duration: PT12M1S

---

# C# Screen Capture to AVI Implementation Guide

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records a whole monitor and the sound from the speakers to an AVI file in a C# WinForms app with the VideoCaptureCore engine. The monitor, the frame rate and the mouse cursor are picked on the form, the sound comes from the VisioForge What You Hear source, the video codec is picked from the ones installed on the PC (MJPEG by default), and the audio is uncompressed PCM or MP3.

This part uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine. On macOS, Linux, Android and iOS, VideoCaptureCoreX records AVI as well: see [VideoCaptureCoreX](#modern-api-video-capture-sdk-x) below.

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/V9iZiSx7slI?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-avi-winforms)

## When to Use AVI for Screen Recording

- **Every frame is a whole picture** - with MJPEG, you can cut and edit on any frame, and read each frame on its own for analysis, without decoding the frames before it
- **No audio loss** - a plain `AVIOutput` writes uncompressed PCM
- **DirectShow tools** - AVI with MJPEG opens in DirectShow-based editors and players on Windows

**Trade-off:** size. In the video, a 4K screen at 10 frames per second with MJPEG takes about 30 MB a second: 679 MB for 22 seconds, about 110 GB an hour. For the web, phones and sharing, record [MP4 with H.264](screen-capture-mp4.md) instead.

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the video capture redist. Its DirectShow filters include the screen source and the What You Hear sound source; AVI needs nothing more:

- For 64-bit applications: [x64 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- For 32-bit applications: [x86 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)

Set `PlatformTarget` in the project file to match the redist (`x64` or `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the monitor, frame rate, sound device and video codec, "Mouse cursor" and "Record sound" check boxes (both checked), an "MP3 audio" check box, and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, `Form1.cs` builds unchanged in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()` with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureAvi;

public partial class Form1 : Form
{
    private VideoCaptureCore videoCapture1 = null!;

    public Form1()
    {
        InitializeComponent();
    }

    // The engine names a monitor by the N of its device name, \\.\DISPLAYN
    private static int MonitorNumber(Screen screen) =>
        int.Parse(screen.DeviceName.Replace(@"\\.\DISPLAY", string.Empty));

    private void Form1_Load(object sender, EventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        foreach (var screen in Screen.AllScreens)
            cbMonitor.Items.Add(MonitorNumber(screen));

        cbMonitor.SelectedItem = MonitorNumber(Screen.PrimaryScreen!);

        cbFrameRate.Items.AddRange(new object[] { 5, 10, 15, 25, 30 });
        cbFrameRate.SelectedItem = 10;

        // "VisioForge What You Hear Source" records what plays on the speakers
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbMP3.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbMP3.Enabled = cbRecordSound.Checked; // MP3 needs sound

        // The video codecs installed on this PC
        foreach (var codec in videoCapture1.Video_Codecs())
            cbVideoCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "MJPEG Compressor";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        Text = "Screen to AVI";

        // The whole monitor at the frame rate from the list
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication: faster than GDI
            DisplayIndex = (int)cbMonitor.SelectedItem!,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate((int)cbFrameRate.SelectedItem!)
        };

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        videoCapture1.Audio_RecordAudio = recordSound;
        // No device when the sound is off: a What You Hear device left here still opens the speakers
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // never play the captured sound back into the speakers

        // Audio: MP3, or uncompressed PCM
        var avi = recordSound && cbMP3.Checked ? new AVIOutput(new MP3Output()) : new AVIOutput();
        avi.Video_Codec = cbVideoCodec.Text;

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.avi");
        videoCapture1.Output_Format = avi;
        videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

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

1. **Monitor** - `DisplayIndex` takes the number at the end of the monitor's Windows device name, `\\.\DISPLAY1` → `1`; `MonitorNumber` reads it from `Screen.DeviceName`, and the primary monitor is selected
2. **Screen Source** - `FullScreen` records the whole monitor at its full resolution, and `AllowDesktopDuplicationEngine` captures through Windows Desktop Duplication instead of GDI. `GrabMouseCursor` is off by default and `FrameRate` is 10 by default
3. **Speaker Sound** - `VisioForge What You Hear Source`, from the video capture redist, records what plays on the speakers. With Record sound off, the app sets `Audio_RecordAudio` to `false` and clears `Audio_CaptureDevice` too, and `Audio_PlayAudio` stays `false` so the recorded sound isn't played back into the speakers
4. **Video Codec** - `Video_Codecs()` returns the DirectShow video compressors installed on the PC; `AVIOutput.Video_Codec` takes one of those names and defaults to `MJPEG Compressor`, which stores every frame as a JPEG picture
5. **Audio** - A plain `AVIOutput` writes uncompressed PCM, 16-bit stereo at 44.1 kHz, about ten megabytes a minute; `new AVIOutput(new MP3Output())` writes MP3 at 192 kbps, about one and a half megabytes a minute
6. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the form finishes the AVI file first

## Check the File with ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.avi -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=mjpeg|width=3840|height=2160
# stream|codec_name=pcm_s16le|sample_rate=44100|channels=2
# format|format_name=avi|duration=22.400000|size=679261184
```

Twenty-two seconds of a 4K screen at 10 frames per second is 679 megabytes, roughly 240 megabits per second, which is why AVI is for editing and analysis, not for sharing.

## Video Capture SDK X (VideoCaptureCoreX) { #modern-api-video-capture-sdk-x }

The cross-platform API uses `VideoCaptureCoreX`. For the complete console application pattern with screen capture setup, audio configuration, and recording lifecycle, see the [Screen Capture to MP4](screen-capture-mp4.md#modern-api-video-capture-sdk-x) guide. To output AVI instead of MP4, replace the output configuration:

```csharp
// AVI with default codecs (H.264 video + MP3 audio)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### AVI Codec Options

Choose the video encoder based on your workflow:

```csharp
// MJPEG — frame-independent, larger files, ideal for editing
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 in AVI container — smaller files, less editing-friendly
var h264AviOutput = new AVIOutput(outputPath);
h264AviOutput.Video = new OpenH264EncoderSettings();
```

Region capture, multi-monitor recording, audio, cursor highlighting, and GPU encoding options are covered in the [MP4 guide](screen-capture-mp4.md) — all source configuration features work identically with AVI output.

## Frequently Asked Questions

### Why is my AVI screen recording file so large?

MJPEG compresses each frame on its own, trading file size for editing convenience: a 4K screen at 10 frames per second takes about 30 MB a second. To reduce the size, lower the frame rate, capture a smaller region instead of the full screen (`FullScreen = false` with `Left`, `Top`, `Right` and `Bottom`), or switch to [MP4 output](screen-capture-mp4.md), whose H.264 compresses across frames.

### How do I record the sound of the computer, not the microphone?

Select `VisioForge What You Hear Source` as the `AudioCaptureSource`: it records what plays on the speakers, and it ships in the video capture redist. Keep `Audio_PlayAudio` at `false`, so the recorded sound isn't played back into the speakers.

### When should I use AVI instead of MP4 for screen recording?

Choose AVI when you need frame-independent access for video editing (MJPEG allows scrubbing to any frame without decoding previous frames), when you need lossless PCM audio, or when working with DirectShow-based tools. For all other cases, MP4 with H.264 offers better compression, smaller files, and broader cross-platform support.

## Video Transcript

### What we build

Let's record the screen and the sound from the speakers to an AVI file in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms app that records a whole monitor to an AVI file. The monitor, the frame rate and the mouse cursor are picked on the form. It also records the sound that plays on the speakers, as uncompressed PCM or as MP3. And the video codec is any one installed on the PC, MJPEG by default. This is the finished app, recording the screen it runs on.

### AVI or MP4

When is AVI the right choice over MP4? With MJPEG, every frame of the recording is a whole picture, so you can cut and edit on any frame, and read each frame on its own for analysis. PCM audio has no loss. The price is size: here about twenty-eight megabytes every second, for a 4K screen at ten frames per second. For the web, phones and sharing, MP4 with H.264 is the better choice. That's the screen capture to MP4 video.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. No camera and no microphone this time. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project ScreenCaptureAvi. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the SDK itself, and the video capture redist. It carries the DirectShow filters, the screen source and the What You Hear sound source among them. AVI needs nothing more. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. The redist is the x64 one, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redist instead.

### Design the WinForms form

From the Toolbox, draw a VideoView, where the app previews what it records. Add a label and a combo box for the monitor, and give every control a clear name in the Properties window. Do the same for the frame rate, the sound device and the video codec. Add three check boxes: the mouse cursor and Record sound, both checked, and MP3 audio. Then the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### Fill the lists

The engine names a monitor by the number at the end of its Windows device name: one for DISPLAY1, two for DISPLAY2. A small helper reads that number. The Load handler lists every monitor, and selects the primary one. The frame rate list holds a few fixed values. Ten frames per second is selected, the SDK's own default for the screen. Audio_CaptureDevices lists the sound devices. VisioForge What You Hear Source records whatever plays on the speakers, so it's selected when it's there. With no sound device at all, both sound boxes are disabled. And MP3 only makes sense with sound, so it follows Record sound. Video_Codecs lists the video codecs installed on this PC. We select MJPEG Compressor, or the first codec when MJPEG isn't there. Video_Codecs returns the DirectShow video compressors on this PC. This one has two: MJPEG Compressor and DV Video Encoder. MJPEG stores every frame as a JPEG picture. DV writes standard-definition DV only. A codec you install yourself shows up in the same list.

### Capture the screen

The Start handler first checks that a video codec is selected. If not, it says so in the title bar. Then it describes the screen source. FullScreen records the whole monitor. AllowDesktopDuplicationEngine captures through Windows Desktop Duplication instead of GDI. DisplayIndex is the monitor number from the list, GrabMouseCursor draws the cursor into the video, and FrameRate is the rate from the list. A quick recap. FullScreen records the whole monitor at its full resolution. DisplayIndex is the number from the monitor's device name. The cursor is off by default, and the frame rate is ten. This video records a 4K screen at ten frames per second.

### Record the speaker sound

With Record sound checked, the app records the selected device. With it unchecked, the device is cleared too: a What You Hear device left in place still opens the speakers. Audio_PlayAudio stays off, so the recorded sound isn't played back into the speakers it comes from.

### AVIOutput: PCM or MP3 audio

Now the output. A plain AVIOutput writes uncompressed PCM audio. Pass an MP3Output to its constructor, and the audio is MP3 instead. Set Video_Codec to the name from the list, write output.avi into the Videos folder, and switch the engine to screen capture mode. A plain AVIOutput writes PCM: 16-bit stereo at 44.1 kilohertz, with no loss, about ten megabytes a minute. With an MP3Output it writes MP3 at 192 kilobits per second, about one and a half megabytes a minute. Next to MJPEG video, both are small.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finishes writing the AVI file.

### Close the window

Closing the window while it records would leave the AVI unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The lists fill in: the monitor, the frame rate, the sound device and the codec. The sound list has What You Hear and the microphones. What You Hear stays selected. A sound plays on the speakers, and Start begins the recording. The preview shows the screen with the cursor. Stop finishes output.avi, and then we close the window.

### Check the file with ffprobe

The file lands in Videos: about six hundred and fifty megabytes for twenty-three seconds. ffprobe shows what's inside. The container is AVI. The video is MJPEG at 3840x2160. The audio is PCM, 16-bit, 44.1 kilohertz stereo: the sound from the speakers. That's about 240 megabits per second, which is why AVI is for editing and analysis, not for sharing.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. A silent file, because Record sound was off, or a microphone was selected instead of What You Hear. x86 and x64 packages mixed in one project. Closing the window mid-recording: await StopAsync first. And AVI where MP4 belongs: an hour of 4K MJPEG runs to about a hundred gigabytes.

### Next steps

Next, the screen capture to MP4 video saves the screen to small H.264 files. The webcam to AVI video records a camera the same way as here. And the docs show how to record a region or a single window instead of the whole screen. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Screen Capture to MP4](screen-capture-mp4.md) — recommended format with full feature coverage (region, multi-monitor, audio, GPU encoding, cross-platform)
- [Webcam to AVI](video-capture-camera-avi.md) — the same AVI output with a camera and a microphone
- [Screen Capture to WMV](screen-capture-wmv.md) — Windows Media format alternative
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture settings
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
