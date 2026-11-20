---
title: Record IP Camera RTSP to MP4 in C# .NET — Code Sample
description: Record an RTSP IP camera to MP4 in a C# WinForms app with VideoCaptureCoreX - URL, login and password, live preview, the camera's audio, H.264 on the GPU.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Streaming
  - IP Camera
  - RTSP
  - ONVIF
  - MP4
  - H.264
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - MP4Output
  - VisioForgeX
video:
  id: 6CTrBCzHySE
  upload_date: 2026-11-17T08:57:31Z
  duration: PT9M52S

---

# Record an IP Camera RTSP Stream to MP4 in C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial records an IP camera to an MP4 file in a C# WinForms app with the cross-platform `VideoCaptureCoreX` engine:

- the camera's RTSP URL, its login and its password are typed into the form, and the password field shows dots;
- `RTSPSourceSettings.CreateAsync` connects to the camera and reads what the stream carries, so a camera without a microphone is recorded video-only;
- a `VideoView` shows a live preview while the file is written;
- `MP4Output` encodes H.264 on the GPU when the PC has a hardware encoder, OpenH264 on the CPU otherwise, with AAC audio;
- every recording gets its own file name, with the date and time, in the Videos folder.

The classic `VideoCaptureCore` engine records IP cameras as well: the [IP camera preview tutorial](ip-camera-preview.md) builds its app, and its `IPCapture` mode writes the file.

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/6CTrBCzHySE?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### The short version

The same recording as a console app in a few lines of C#, without the preview and the sound. The login and password are written in its code: replace `admin` and `password` with your camera's.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/CUrQcoK86nQ?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

Its code: [ip-camera-mp4-short-console on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-mp4-short-console).

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-capture-mp4-winforms)

## Required Packages

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — the SDK; the WinForms `VideoView` ships in it
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — the GStreamer runtime the X engine runs on, with the RTSP source and the H.264 encoders
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — the AAC audio encoder

These are the x64 packages, so the project sets `<PlatformTarget>x64</PlatformTarget>`; a 32-bit app takes the `.x86` packages instead. See [Windows deployment](../../deployment-x/Windows.md) for the full list.

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Implementation Example with C# Code

The form has a `VideoView` named `VideoView1`, the text boxes `edURL`, `edLogin` and `edPassword` (with `UseSystemPasswordChar`), a **Record audio** check box `cbRecordAudio`, and the buttons `btStart` and `btStop`, both disabled in the designer. The form's `Load` and `FormClosing` events are wired to the handlers below.

```csharp
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraCaptureMp4;

public partial class Form1 : Form
{
    private VideoCaptureCoreX videoCapture1 = null!;

    private string outputFile = string.Empty;

    public Form1()
    {
        InitializeComponent();
    }

    private async void Form1_Load(object sender, EventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => BeginInvoke(() => Text = args.Message);

        btStart.Enabled = true;
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        Text = "Connecting...";
        btStart.Enabled = false;
        cbRecordAudio.Enabled = false;

        var started = false;
        try
        {
            // Reads the stream info first: audio is recorded only when the info shows an audio track
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Text, cbRecordAudio.Checked);

            videoCapture1.Audio_Record = cbRecordAudio.Checked;
            videoCapture1.Audio_Play = false; // record the camera's sound, don't play it

            // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
            outputFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"ipcam_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            videoCapture1.Outputs_Clear();
            videoCapture1.Outputs_Add(new MP4Output(outputFile));

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Text = ex.Message;
        }

        // An SDK error posted during the start stays in the title
        if (Text == "Connecting...") Text = started ? "Recording..." : "The recording did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbRecordAudio.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync(); // finishes the MP4 file
        Text = "Saved: " + outputFile;
        btStart.Enabled = true;
        cbRecordAudio.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (!btStart.Enabled && !btStop.Enabled)
        {
            e.Cancel = true; // loading, starting or stopping: wait for it
            return;
        }

        if (btStop.Enabled)
        {
            e.Cancel = true; // recording: finish the MP4 first
            btStop.Enabled = false;
            await videoCapture1.StopAsync();
            btStart.Enabled = true;
            BeginInvoke(Close);
            return;
        }

        videoCapture1.Dispose();
        VisioForgeX.DestroySDK();
    }
}
```

## Key Implementation Features

1. **SDK initialization** - `VisioForgeX.InitSDKAsync` runs once, when the form loads. The first run on a PC takes longer, so Start is enabled only after it returns. `VisioForgeX.DestroySDK` releases it when the form closes
2. **The camera's address** - `rtsp://`, the camera's IP address and port (usually 554), then a path that differs per brand. The login and the password are separate arguments of `CreateAsync`, so the URL does not carry them. An `http://` address is taken as the camera's ONVIF service, and the RTSP URL of its best profile is read from it. The [IP camera brands directory](../../camera-brands/index.md) lists the paths of many cameras
3. **Audio** - with **Record audio** checked, `CreateAsync` asks for the camera's audio and `Audio_Record` puts it in the file. The audio is recorded only when the stream info shows an audio track, so a camera without a microphone gives a video-only file. `Audio_Play` is `true` by default; the app sets it to `false`, so the sound is recorded but not played
4. **MP4 output** - `MP4Output` with no encoder settings picks NVIDIA NVENC, then AMD AMF, then Intel Quick Sync, and OpenH264 on the CPU when none of them is present. Audio is AAC. An output stays in the engine after a recording, so `Outputs_Clear` runs before each new one
5. **Starting** - a camera that does not answer takes a while to fail, so the title says "Connecting..." and Start is disabled until `StartAsync` returns. An `OnError` message posted during the start stays in the title
6. **Stopping and closing** - `StopAsync` finalizes the MP4 file. Closing the form during a recording cancels the close, awaits `StopAsync`, and closes again; while the SDK is loading, or a start or a stop is under way, the close waits

## Troubleshooting

- **The camera refuses the stream** - the path does not match the camera's brand, or the login or password is wrong. The error is shown in the window title
- **The file has no sound** - the camera sends no audio track, or audio is turned off in the camera's own settings
- **Choosing the encoder** - to stay on the CPU, pass `new OpenH264EncoderSettings()` (namespace `VisioForge.Core.Types.X.VideoEncoders`) to the `MP4Output` constructor; `H264EncoderBlock.CanEncodeAsync` (namespace `VisioForge.Core.MediaBlocks.VideoEncoders`) tests whether a GPU encoder can encode right now
- **Several cameras** - use one `VideoCaptureCoreX` and one `VideoView` for each

## Video Transcript

### What we build

Let's record an IP camera to an MP4 file in a C# WinForms app, with the VisioForge Video Capture SDK. Here's what we build. A WinForms app that records an IP camera to an MP4 file. You type the camera's RTSP address, its login and its password. The picture shows live while the app records. The video is H.264, and the camera's sound is recorded as AAC. This is the finished app, recording. The stream here comes from a test RTSP server on the local network, which plays the open movie Big Buck Bunny, so it needs no login. A real camera works the same way, with its login and password. Where does this help? A simple video recorder for your own cameras. Saving an event to a file as evidence. Recording a production line or a lab test. And clips you review or share later.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. And an IP camera that streams RTSP, with its address. The engine here is VideoCaptureCoreX, the SDK's cross-platform engine, built on GStreamer. The same code runs on macOS and Linux with their own UI.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project IpCameraCaptureMp4. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add three NuGet packages. The Video Capture SDK itself. The cross-platform core package, with the GStreamer runtime for Windows x64. And the Libav package, which brings the AAC audio encoder. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.VideoCapture and install it. Then install the cross-platform core package for Windows x64. And the Libav package for Windows x64. The runtime packages are x64, so set PlatformTarget to x64 in the project file.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live picture. Add a label and a text box for the camera URL, and give every control a clear name in the Properties window. Do the same for the login and the password. On the password box, set UseSystemPasswordChar to True, so it shows dots. Add a Record audio check box, checked by default, and the Start and Stop buttons. Both buttons start disabled. Start is enabled once the SDK has loaded, and Stop once the recording runs. Double-click the form and the two buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. The X namespaces hold the cross-platform engine, its sources and its outputs. In the form's Load handler, InitSDKAsync loads the SDK and its GStreamer runtime, once per app. Then create the VideoCaptureCoreX engine and pass it the VideoView. Engine errors arrive in the OnError event, and here they go to the title bar. Finally, Start is enabled. A field keeps the output file name, to show it after the stop.

### Connect to the camera

The Start handler first checks that the URL is a valid address. Connecting can take a few seconds, so the title says Connecting, and Start and the check box are disabled. RTSPSourceSettings.CreateAsync takes the URL, the login, the password, and whether to use audio. The URL has no login or password in it, so they never show up in the address. CreateAsync reads the stream's info first. When the camera sends no sound, audio is left out, and the recording is video only. Audio_Record follows the check box. Audio_Play is on by default, and this app records the sound without playing it, so it goes off. The address starts with rtsp, then the camera's IP address and the port, usually 554. Then comes a path, and every brand has its own. Many cameras send a main stream at full resolution and a smaller sub stream. The camera's manual or its web page gives the exact path.

### Record to MP4

The file goes to the Videos folder, with the date and time in its name. Outputs_Clear removes the output of the last run, and Outputs_Add adds an MP4Output. With no encoders passed, MP4Output picks H.264 on the GPU encoder when there is one, NVIDIA, AMD or Intel, and OpenH264 otherwise. The sound is AAC. StartAsync connects, and returns once the recording runs. An exception, like a camera that refuses the connection, goes to the title. An error from the SDK during the start stays in the title too. Otherwise the title says Recording. Stop is enabled when the start worked, and Start when it didn't.

### Stop and close

Stop disables itself, and StopAsync stops the stream and finishes the MP4 file. The title then shows where the file was saved, and Start and the check box are enabled again. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While the app is loading, starting or stopping, both buttons are disabled, and the handler cancels the close: close the window again once that is done. While it records, the close is cancelled too. StopAsync finishes the file, and then the form closes itself. Only then are the engine disposed and the SDK released. A file that is not finished cannot be played.

### Run it

Run the app from Visual Studio. Start is enabled once the SDK has loaded. Type the stream's RTSP address. This test stream needs no login. Click Start. The stream connects in a few seconds, the picture appears, and the title says Recording. Click Stop. The file is finished, and the title shows where it was saved, in the Videos folder. And close the window. This file has H.264 video at 1280 by 720, and AAC sound in stereo.

### Common mistakes

A few common mistakes. Leaving out the Libav package, so there is no AAC encoder for the sound. Creating the engine before InitSDKAsync has loaded the SDK. Putting the login and password into the URL, where they show up wherever the address does. And closing the app without StopAsync, which leaves an MP4 file that cannot be played.

### Next steps

Next, pass your own encoder settings to MP4Output, for a bitrate or a codec of your choice. Split a long recording into files by time. For several cameras, use one VideoCaptureCoreX and one VideoView for each. And ONVIF finds the cameras on the network for you. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## Related Guides

- [IP camera live preview tutorial](ip-camera-preview.md) — the same form on the classic `VideoCaptureCore` engine
- [RTSP camera source configuration](../video-sources/ip-cameras/rtsp.md) — UDP/TCP transport, low-latency tuning, engine selection
- [ONVIF IP camera integration](../video-sources/ip-cameras/onvif.md) — auto-discovery and profile selection for IP cameras
- [Save original RTSP stream without re-encoding](../../mediablocks/Guides/rtsp-save-original-stream.md) — Media Blocks alternative preserving source bitrate

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples. Need the RTSP URL for your camera? Check our [IP camera brands directory](../../camera-brands/index.md).
