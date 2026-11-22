---
title: Record the Screen to WMV in C# WinForms (WMV 9, WMA, CBR)
description: Record the screen and the speaker sound to WMV in C# WinForms with VideoCaptureCore - Windows Media codecs at a constant bitrate, checked with ffprobe.
sidebar_label: Screen Capture to WMV
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - VideoCaptureCore
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Screen Capture
  - MP4
  - AVI
  - WMV
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - ScreenCaptureSourceSettings
  - AudioCaptureSource
  - WMVOutput
  - VideoCaptureCoreX
  - VideoView
video:
  id: gpaVniBKXBw
  upload_date: 2026-11-16T06:07:30Z
  duration: PT13M21S

---

# C# Screen Capture to WMV Implementation Guide

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records a whole monitor and the sound from the speakers to a WMV file in a C# WinForms app with the VideoCaptureCore engine. The monitor, the frame rate and the mouse cursor are picked on the form, and the sound comes from the VisioForge What You Hear source. The output is a `WMVOutput` with custom settings: Windows Media Video 9 at a constant 10 Mbps and WMA audio, with the codecs and the audio format picked from the lists Windows reports.

This part uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine. VideoCaptureCoreX writes WMV as well: see [VideoCaptureCoreX](#modern-api-video-capture-sdk-x) below.

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/gpaVniBKXBw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/screen-capture-wmv-winforms)

## When to Use WMV for Screen Recording

WMV (Windows Media Video) uses Microsoft's Windows Media codecs and the ASF container. It remains useful in specific Windows-centric scenarios:

- **Native Windows playback** - WMV files play on Windows out of the box, and the Windows Media encoders come with Windows: no GPU and no codec pack
- **A known file size** - at a constant bitrate, the size of the file has a ceiling you know before you record
- **Smaller files than AVI** - in the video, a 4K screen at 15 frames per second takes about 33 MB a minute; in the [AVI tutorial](screen-capture-avi.md), a 4K screen at only 10 frames per second with MJPEG takes about 30 MB a second
- **Legacy enterprise environments** - many corporate environments standardize on Windows Media formats for internal video distribution

**Trade-off:** WMV is a Windows-centric format with limited support on macOS, Linux and phones. For cross-platform compatibility, [MP4 is the recommended format](screen-capture-mp4.md).

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the video capture redist. Its DirectShow filters include the screen source and the What You Hear sound source; the WMV encoders come with Windows.

Set `PlatformTarget` in the project file to match the redist (`x64` or `x86`). On the N editions of Windows, install the Media Feature Pack for the Windows Media encoders.

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
```

For a 32-bit (x86) application, use the x86 package instead: `VisioForge.DotNet.Core.Redist.VideoCapture.x86`.

`VisioForge.DotNet.VideoCapture`, the package in the Try it block at the top of this page, works as well: it installs the same `VisioForge.DotNet.Core`. The redist packages are needed either way.

## Implementation Example with C# Code

The form has a `VideoView`, combo boxes for the monitor, frame rate, sound device, video codec, audio codec and audio format, "Mouse cursor" and "Record sound" check boxes (both checked), and Start and Stop buttons; the designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, `Form1.cs` builds unchanged in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()` with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace ScreenCaptureWmv;

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
        cbFrameRate.SelectedItem = 15;

        // "VisioForge What You Hear Source" records what plays on the speakers
        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbSound.Items.Add(device.Name);

        cbSound.SelectedItem = "VisioForge What You Hear Source";
        if (cbSound.SelectedIndex < 0 && cbSound.Items.Count > 0) cbSound.SelectedIndex = 0;
        cbRecordSound.Enabled = cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbSound.Items.Count > 0;
        cbRecordSound.CheckedChanged += (_, _) => cbAudioCodec.Enabled = cbAudioFormat.Enabled = cbRecordSound.Checked; // audio settings need sound

        // The Windows Media codecs that can encode at a constant bitrate
        foreach (var codec in videoCapture1.WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR))
            cbVideoCodec.Items.Add(codec);

        foreach (var codec in videoCapture1.WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR))
            cbAudioCodec.Items.Add(codec);

        cbVideoCodec.SelectedItem = "Windows Media Video 9";
        if (cbVideoCodec.SelectedIndex < 0 && cbVideoCodec.Items.Count > 0) cbVideoCodec.SelectedIndex = 0;
        cbAudioCodec.SelectedItem = "Windows Media Audio 9.2";
        if (cbAudioCodec.SelectedIndex < 0 && cbAudioCodec.Items.Count > 0) cbAudioCodec.SelectedIndex = 0;
    }

    private void cbAudioCodec_SelectedIndexChanged(object sender, EventArgs e)
    {
        // The sample rates, channels and bitrates this audio codec offers
        cbAudioFormat.Items.Clear();
        foreach (var format in videoCapture1.WMV_CustomProfile_AudioFormats(cbAudioCodec.Text, WMVStreamMode.CBR))
            cbAudioFormat.Items.Add(format);

        var index = cbAudioFormat.Items.IndexOf("CBR 128 kbps, 44100 Hz, Stereo, 16 bit");
        if (cbAudioFormat.Items.Count > 0) cbAudioFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (cbVideoCodec.SelectedItem is null)
        {
            Text = "No video codec selected";
            return;
        }

        var recordSound = cbRecordSound.Checked && cbSound.SelectedItem is not null;
        if (recordSound && cbAudioFormat.SelectedItem is null)
        {
            Text = "No audio format selected";
            return;
        }

        Text = "Screen to WMV";

        // The whole monitor at the frame rate from the list
        var frameRate = (int)cbFrameRate.SelectedItem!;
        videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
        {
            FullScreen = true,
            AllowDesktopDuplicationEngine = true, // DXGI Desktop Duplication: faster than GDI
            DisplayIndex = (int)cbMonitor.SelectedItem!,
            GrabMouseCursor = cbCursor.Checked,
            FrameRate = new VideoFrameRate(frameRate)
        };

        videoCapture1.Audio_RecordAudio = recordSound;
        // No device when the sound is off: a What You Hear device left here still opens the speakers
        videoCapture1.Audio_CaptureDevice = recordSound ? new AudioCaptureSource(cbSound.Text) : null;
        videoCapture1.Audio_PlayAudio = false; // never play the captured sound back into the speakers

        var wmv = new WMVOutput
        {
            Mode = WMVMode.CustomSettings,

            Custom_Video_StreamPresent = true,
            Custom_Video_Codec = cbVideoCodec.Text,
            Custom_Video_Mode = WMVStreamMode.CBR,
            Custom_Video_Bitrate = 10000, // kbps
            Custom_Video_SizeSameAsInput = true, // the monitor's resolution
            Custom_Video_FrameRate = frameRate,
            Custom_Video_KeyFrameInterval = 2, // seconds

            Custom_Audio_StreamPresent = recordSound,
            Custom_Audio_Codec = cbAudioCodec.Text,
            Custom_Audio_Format = cbAudioFormat.Text,
            Custom_Audio_Mode = WMVStreamMode.CBR,
        };

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output.wmv");
        videoCapture1.Output_Format = wmv;
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

1. **Monitor** - `DisplayIndex` takes the number at the end of the monitor's Windows device name, `\\.\DISPLAY1` → `1`; `MonitorNumber` reads it from `Screen.DeviceName`, and the primary monitor is selected
2. **Screen Source** - `FullScreen` records the whole monitor at its full resolution, and `AllowDesktopDuplicationEngine` captures through Windows Desktop Duplication instead of GDI. `GrabMouseCursor` is off by default and `FrameRate` is 10 by default; the app records at 15
3. **Speaker Sound** - `VisioForge What You Hear Source`, from the video capture redist, records what plays on the speakers. With Record sound off, the app sets `Audio_RecordAudio` to `false` and clears `Audio_CaptureDevice` too, and `Audio_PlayAudio` stays `false` so the recorded sound isn't played back into the speakers
4. **Windows Media Codecs** - `WMV_CustomProfile_VideoCodecs(WMVStreamMode.CBR)` and `WMV_CustomProfile_AudioCodecs(WMVStreamMode.CBR)` list the Windows Media encoders that work at a constant bitrate, and `WMV_CustomProfile_AudioFormats(codec, WMVStreamMode.CBR)` lists the formats of one audio codec. Pick the names from these lists rather than typing them. Windows Media Video 9 records a 4K screen
5. **Custom Settings** - `new WMVOutput()` alone uses a built-in profile; `Mode = WMVMode.CustomSettings` takes the `Custom_Video_*` and `Custom_Audio_*` values instead. `Custom_Video_Bitrate` is in kbps, so `10000` is 10 Mbps; `Custom_Video_SizeSameAsInput` keeps the monitor's resolution, and `Custom_Video_KeyFrameInterval` is in seconds. The audio stream is written only when `Custom_Audio_StreamPresent` is `true`
6. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the form finishes the WMV file first

## Check the File with ffprobe

```powershell
cd ([Environment]::GetFolderPath('MyVideos'))
ffprobe -v error -of compact output.wmv -show_entries format=format_name,duration,size:stream=codec_name,width,height,sample_rate,channels

# stream|codec_name=wmv3|width=3840|height=2160
# stream|codec_name=wmav2|sample_rate=44100|channels=2
# format|format_name=asf|duration=16.731000|size=9157531
```

The container is ASF, the format behind WMV. `wmv3` is ffprobe's name for Windows Media Video 9, and `wmav2` is the WMA stream. Seventeen seconds of a 4K screen is 9.2 megabytes (8.7 MB as Windows Explorer counts them), or 4.4 Mbps: 10000 kbps is the ceiling, and a desktop that barely moves takes less.

## Video Capture SDK X (VideoCaptureCoreX) { #modern-api-video-capture-sdk-x }

The cross-platform API uses `VideoCaptureCoreX`. For the complete console application pattern with screen capture setup, audio configuration, and recording lifecycle, see the [Screen Capture to MP4](screen-capture-mp4.md#modern-api-video-capture-sdk-x) guide. To output WMV instead of MP4, replace the output configuration:

```csharp
using VisioForge.Core.Types.X.Output;

// WMV output (Windows Media Video + WMA audio codecs)
var wmvOutput = new WMVOutput(outputPath);
videoCapture.Outputs_Add(wmvOutput, autostart: true);
```

Region capture, multi-monitor recording, audio, cursor highlighting, and GPU encoding options are covered in the [MP4 guide](screen-capture-mp4.md) — all source configuration features work identically with WMV output.

## Frequently Asked Questions

### When should I use WMV instead of MP4 for screen recording?

Choose WMV when your target audience uses Windows and you need native playback without additional codec installation, when the file size has to stay under a known ceiling, or when working within enterprise environments that standardize on Windows Media formats. For cross-platform distribution or web publishing, MP4 with H.264 is the better choice: broader device support and better compression.

### Why does my WMV file have no sound?

Check that "Record sound" is on and that `VisioForge What You Hear Source` is selected, not a microphone: it records what plays on the speakers, and it ships in the video capture redist. With sound on, `Custom_Audio_StreamPresent` must be `true` and `Custom_Audio_Format` must be a name from `WMV_CustomProfile_AudioFormats` for the selected audio codec.

### How do I set the WMV bitrate?

Set `Mode = WMVMode.CustomSettings`, `Custom_Video_Mode = WMVStreamMode.CBR` and `Custom_Video_Bitrate` in kilobits per second: `10000` is 10 Mbps. The audio bitrate is part of the audio format name, such as `CBR 128 kbps, 44100 Hz, Stereo, 16 bit`.

## Video Transcript

### What we build

Let's record the screen and the sound from the speakers to a WMV file in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms app that records a whole monitor to a WMV file. The monitor, the frame rate and the mouse cursor are picked on the form. It also records the sound that plays on the speakers, as WMA audio. The video is Windows Media Video 9, at a constant bitrate we set ourselves: ten megabits per second. And the codecs and the audio format come from the lists Windows reports. This is the finished app, recording the screen it runs on.

### WMV, AVI or MP4

When is WMV the right choice? A WMV file plays on Windows out of the box. The Windows Media encoders come with Windows, so you need no GPU and no codec pack. With a constant bitrate, the size of the file has a known ceiling before you record. And it's small: here about thirty megabytes a minute, for a 4K screen at fifteen frames per second. The same screen in AVI with MJPEG took about twenty-eight megabytes every second. For the web, phones and Macs, MP4 with H.264 is the better choice. That's the screen capture to MP4 video.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. No camera and no microphone this time. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project ScreenCaptureWmv. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add two NuGet packages: the SDK itself, and the video capture redist. It carries the DirectShow filters, the screen source and the What You Hear sound source among them. The WMV encoders come with Windows, so that's all. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. The redist is the x64 one, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redist instead.

### Design the WinForms form

From the Toolbox, draw a VideoView, where the app previews what it records. Add a label and a combo box for the monitor, and give every control a clear name in the Properties window. Do the same for the frame rate, the sound device, the video codec, the audio codec and the audio format. Add two check boxes, both checked: the mouse cursor, and Record sound. Then the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the audio codec list and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### Fill the lists

The engine names a monitor by the number at the end of its Windows device name: one for DISPLAY1, two for DISPLAY2. A small helper reads that number. The Load handler lists every monitor, and selects the primary one. The frame rate list holds a few fixed values. Fifteen frames per second is selected. Audio_CaptureDevices lists the sound devices. VisioForge What You Hear Source records whatever plays on the speakers, so it's selected when it's there. With no sound device at all, Record sound and both audio lists are disabled. And the audio codec and format only matter with sound, so they follow Record sound.

### Windows Media codecs

WMV_CustomProfile_VideoCodecs and WMV_CustomProfile_AudioCodecs list the Windows Media encoders that can work at a constant bitrate. We fill both lists, and select Windows Media Video 9 and Windows Media Audio 9.2, or the first codec when one isn't there. When the audio codec changes, WMV_CustomProfile_AudioFormats lists what that codec offers. We pick 128 kilobits per second, 44.1 kilohertz stereo: the rate What You Hear delivers here, so nothing is resampled. On this PC the video list has seven entries: Windows Media Video 9, its Advanced Profile, the Screen and Image codecs, and the older V7 and V8. Windows Media Video 9 records this 4K screen, so it's our default. For audio at a constant bitrate there's WMA 9.2 and WMA 10 Professional. Each audio format names its bitrate, sample rate, channels and bits. Always pick these names from the lists, never type them by hand.

### Capture the screen

The Start handler first checks that a video codec is selected. If not, it says so in the title bar. Sound is recorded only when Record sound is checked and a device is selected. Then an audio format is needed as well. Then it describes the screen source. FullScreen records the whole monitor. AllowDesktopDuplicationEngine captures through Windows Desktop Duplication instead of GDI. DisplayIndex is the monitor number from the list, GrabMouseCursor draws the cursor into the video, and FrameRate is the rate from the list. A quick recap. FullScreen records the whole monitor at its full resolution. DisplayIndex is the number from the monitor's device name. The cursor is off by default, and the frame rate is ten. This video records a 4K screen at fifteen frames per second.

### Record the speaker sound

With sound on, the app records the selected device. With it off, the device is cleared too: a What You Hear device left in place still opens the speakers. Audio_PlayAudio stays off, so the recorded sound isn't played back into the speakers it comes from.

### WMVOutput: custom settings

Now the output. A WMVOutput in CustomSettings mode takes our own settings instead of a built-in profile. The video stream uses the codec from the list at a constant 10000 kilobits per second. SizeSameAsInput keeps the monitor's resolution, the frame rate is the one we picked, and there's a keyframe every two seconds. The audio stream takes the codec and the format from the lists, and it's there only when sound is on. Then write output.wmv into the Videos folder, and switch the engine to screen capture mode. A plain WMVOutput uses a built-in profile. CustomSettings gives us our own codec, bitrate and size. Custom_Video_Bitrate is in kilobits per second, so 10000 means ten megabits. That's the ceiling: a desktop that barely moves needs less, and this recording came to about four point four megabits per second. And a keyframe every two seconds gives the player a seek point every two seconds.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finishes writing the WMV file.

### Close the window

Closing the window while it records would leave the WMV unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The lists fill in: the monitor, the frame rate, the sound device, both codecs and the audio format. The audio format list has every bitrate and sample rate the codec offers. Ours stays selected. A sound plays on the speakers, and Start begins the recording. The preview shows the screen with the cursor. Stop finishes output.wmv, and then we close the window.

### Check the file with ffprobe

The file lands in Videos: under nine megabytes for seventeen seconds of a 4K screen. ffprobe shows what's inside. The container is ASF, the format behind WMV. The video is Windows Media Video 9, which ffprobe calls wmv3, at 3840x2160. The audio is WMA, 44.1 kilohertz stereo: the sound from the speakers.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. A silent file, because Record sound was off, or a microphone was selected instead of What You Hear. Codec and format names typed by hand: take them from the lists. Custom_Video_Bitrate written in bits per second: it's in kilobits. And closing the window mid-recording: await StopAsync first.

### Next steps

Next, the screen capture to MP4 video saves the screen to H.264 files that play everywhere. The screen capture to AVI video keeps every frame as a whole picture, for editing. And the webcam to WMV video records a camera with the same Windows Media settings as here. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Screen Capture to MP4](screen-capture-mp4.md) — recommended format with full feature coverage (region, multi-monitor, audio, GPU encoding, cross-platform)
- [Screen Capture to AVI](screen-capture-avi.md) — AVI format with MJPEG for frame-independent editing
- [Webcam to WMV](video-capture-webcam-wmv.md) — the same Windows Media settings with a camera and a microphone
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture settings
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
