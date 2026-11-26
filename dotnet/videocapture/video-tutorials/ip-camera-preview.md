---
title: "RTSP IP Camera Live Preview in C# WinForms: .NET Tutorial"
description: Show a live RTSP IP camera in a C# WinForms app with VideoCaptureCore, keep the login and password out of the URL, and pixelate the picture.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WinForms
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - IPCameraSourceSettings
  - IPSourceEngine
  - VideoCaptureMode
  - VideoEffectMosaic
  - IVideoView
video:
  id: dxVWXHwgFQg
  upload_date: 2026-11-16T20:53:27Z
  duration: PT9M47S

---

# IP Camera Preview over RTSP in C# WinForms

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial shows a live IP camera over RTSP in a C# WinForms app with the VideoCaptureCore engine:

- the camera's RTSP URL, its login and its password are typed into the form, and the password field shows dots;
- the login and the password go into their own `IPCameraSourceSettings` properties, never into the URL;
- the LAV engine (`IPSourceEngine.Auto_LAV`) reads the stream;
- `IPPreview` mode shows the camera without writing a file;
- a Pixelate check box puts a `VideoEffectMosaic` over the whole picture.

A security monitor for your own cameras, checking a camera's view while it is installed, hiding faces or screens before the video is shown, and kiosks or lobby screens work this way. The camera in the video is a Dahua on the local network, pixelated on purpose by the app's own effect.

This tutorial uses the Windows-only VideoCaptureCore engine, the SDK's DirectShow engine.

## Video Walkthrough

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/dxVWXHwgFQg?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### The short version

The same preview in a few lines of C#: the engine on the `VideoView`, the camera's address with its login and password, and `IPPreview` mode. Its code is an excerpt of this tutorial's `Form1.cs`, without the Pixelate effect, the Stop button and the close handler; the full code is in [Implementation Example with C# Code](#implementation-example-with-c-code) below.

```csharp
videoCapture1 = new VideoCaptureCore(
    VideoView1 as IVideoView);

videoCapture1.IP_Camera_Source =
    new IPCameraSourceSettings
    {
        URL = url,
        Login = edLogin.Text,
        Password = edPassword.Text,
        Type = IPSourceEngine.Auto_LAV
    };

videoCapture1.Mode =
    VideoCaptureMode.IPPreview;

await videoCapture1.StartAsync();
```

## Source Code Repository

The complete project, with the designer file, is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/ip-camera-preview-winforms)

## Required Packages

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package, the video capture redist with its DirectShow filters, and the LAV redist, the decoder that reads the RTSP stream. A preview writes no file, so it needs no encoder package.

Set `PlatformTarget` in the project file to match the redists (`x64` or `x86`).

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoCapture.x64
dotnet add package VisioForge.DotNet.Core.Redist.LAV.x64
```

For a 32-bit (x86) application, use the x86 packages instead: `VisioForge.DotNet.Core.Redist.VideoCapture.x86` and `VisioForge.DotNet.Core.Redist.LAV.x86`.

`VisioForge.DotNet.VideoCapture`, the package in the Try it block at the top of this page, works as well: it installs the same `VisioForge.DotNet.Core`. The redist packages are needed either way.

## Implementation Example with C# Code

The form has a `VideoView`, text boxes for the URL, the login and the password (`UseSystemPasswordChar` on), a Pixelate check box checked by default, and Start and Stop buttons; Stop starts disabled. The designer file is in the source code above. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()`, which .NET Framework does not generate, with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`, and set `<dpiAware>true</dpiAware>` in an `app.manifest` to keep the system DPI awareness that .NET 10 sets.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace IpCameraPreview;

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
    }

    private async void btStart_Click(object sender, EventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Text = "Enter the camera URL";
            return;
        }

        // The URL stays free of credentials: login and password go separately
        videoCapture1.IP_Camera_Source = new IPCameraSourceSettings
        {
            URL = url,
            Login = edLogin.Text,
            Password = edPassword.Text,
            Type = IPSourceEngine.Auto_LAV
        };

        // Video only: audio playback is on by default
        videoCapture1.Audio_PlayAudio = false;

        // Effects are set before the start
        videoCapture1.Video_Effects_Clear();
        videoCapture1.Video_Effects_Enabled = cbPixelate.Checked;
        if (cbPixelate.Checked)
            videoCapture1.Video_Effects_Add(new VideoEffectMosaic(true, 40));

        videoCapture1.Mode = VideoCaptureMode.IPPreview;

        // A camera that does not answer takes up to ~20 s to fail
        Text = "Connecting...";
        btStart.Enabled = false;
        cbPixelate.Enabled = false;
        var started = await videoCapture1.StartAsync();

        // An SDK error posted during the start stays in the title
        if (Text == "Connecting...") Text = started ? "IP camera preview" : "The preview did not start";
        btStop.Enabled = started;
        btStart.Enabled = !started;
        cbPixelate.Enabled = !started;
    }

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
        cbPixelate.Enabled = true;
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

1. **Credentials out of the URL** - `Login` and `Password` are properties of `IPCameraSourceSettings`, so the URL never carries them and they never show up wherever the address is shown or logged. The password text box sets `UseSystemPasswordChar`, so it shows dots
2. **The engine** - `IPSourceEngine.Auto_LAV` reads RTSP and HTTP streams with LAV Filters and can decode on the GPU. Without the LAV redist the start fails with "Unable to create LAV Splitter Source filter"
3. **The camera's address** - `rtsp://`, the camera's IP address and port (usually 554), then a path that differs per brand. Many cameras send a full-resolution main stream and a smaller sub stream; the camera's manual or web page gives the exact path, and the [IP camera brands directory](../../camera-brands/index.md) lists many of them
4. **Preview mode** - `Mode = VideoCaptureMode.IPPreview` shows the camera and writes no file. Add the MP4 redist ([x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/), [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)), switch to `IPCapture`, set an output file and a format, and the same app records; [Record RTSP stream to MP4](ip-camera-capture-mp4.md) shows the same recording on the cross-platform `VideoCaptureCoreX`
5. **Audio** - `Audio_PlayAudio` is `true` by default; the sample shows video only, so it turns it off
6. **Pixelate** - `VideoEffectMosaic(enabled, value)` takes the block size in pixels, from 1 to 255: 2 to 5 is a light effect, 16 and up leaves nothing readable. It covers the whole frame, in the preview and in a recording alike. Effects are set before `StartAsync`, so the check box is locked while the preview runs, and `Video_Effects_Clear` removes the previous run's effect
7. **Starting** - a camera that does not answer takes up to about 20 seconds to fail, so the title says "Connecting..." and Start is disabled until `StartAsync` returns. An `OnError` message posted during the start stays in the title
8. **Closing** - `Dispose()` stops a running preview, and a preview has no file to finish, so the close handler only waits while the preview is starting or stopping

## Troubleshooting

- **The camera connects but the picture stays black** - the LAV engine receives RTSP video over UDP, and a firewall, a NAT or a VPN that blocks UDP stops it; a virtual machine on NAT networking does this too. Allow UDP from the camera, or use `IPSourceEngine.RTSP_LowLatency`, which reads RTSP over TCP by default (`RTSP_LowLatency_UseUDP` is `false`)
- **The camera refuses the stream** - the path does not match the camera's brand, or the login or password is wrong
- **Several cameras** - use one `VideoCaptureCore` and one `VideoView` for each
- **Finding cameras** - `IP_Camera_ONVIF_ListSourcesAsync` discovers ONVIF cameras on the network

## Video Transcript

### What we build

Let's show a live IP camera over RTSP in a C# WinForms app, with the VisioForge Video Capture SDK. Here's what we build. A WinForms app that shows a live IP camera. You type the camera's RTSP address, its login and its password, and the password field shows dots. A Pixelate check box puts a mosaic effect over the picture. This is the finished app, previewing a Dahua camera on the local network. The picture is pixelated on purpose, by the app's own effect, so this real camera shows nothing private. Nothing is recorded. The preview only shows the picture. Where does this help? A security monitor for your own cameras. Checking what a camera sees while you install it. Hiding faces or screens before the video is shown to anyone. And kiosks or lobby screens.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And an IP camera that streams RTSP, with its address, login and password. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project IpCameraPreview. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add three NuGet packages. The SDK itself. The video capture redist with its DirectShow filters. And the LAV redist, the decoder that reads the RTSP stream. A preview writes no file, so it needs no encoder package. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. And the LAV redist for x64. Without it, the start fails, because there is nothing to read the stream. The redists are x64, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redists instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live picture. Add a label and a text box for the camera URL, and give every control a clear name in the Properties window. Do the same for the login and the password. On the password box, set UseSystemPasswordChar to True, so it shows dots. Add a Pixelate check box, checked by default, and the Start and Stop buttons. Stop stays disabled until the preview runs. Double-click the form and the two buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. VideoEffects holds the mosaic effect. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the picture is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### Connect to the camera

The Start handler first checks that the URL is a valid address. Then IP_Camera_Source gets an IPCameraSourceSettings object. The URL has no login or password in it. They go into their own properties, so they never show up in the address. Type Auto_LAV picks the LAV engine, which reads RTSP and HTTP streams and can decode on the GPU. The address starts with rtsp, then the camera's IP address and the port, usually 554. Then comes a path, and every brand has its own. Many cameras send a main stream at full resolution and a smaller sub stream. The camera's manual or its web page gives the exact path.

### Pixelate the picture

Audio_PlayAudio is on by default, and this app shows video only, so it goes off. Effects are set before the start. Video_Effects_Clear removes the effects of the last run, and Video_Effects_Enabled follows the check box. When it's checked, Video_Effects_Add adds a VideoEffectMosaic with blocks of 40 pixels. VideoEffectMosaic takes a block size in pixels, from 1 to 255. Two to five is a light effect, and from 16 up nothing is readable. It covers the whole frame, in the preview and in a recording alike. The effect is set before StartAsync, so the check box is locked while the preview runs.

### Start the preview

Mode IPPreview shows the camera without writing a file. A camera that doesn't answer can take about 20 seconds to fail, so the title says Connecting, and Start and the check box are disabled. StartAsync returns once the preview is running. An error from the SDK stays in the title, and Stop is enabled when the start worked.

### Stop and close

Stop disables itself, and StopAsync stops the stream. Then Start and the check box are enabled again. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While the preview is starting or stopping, both buttons are disabled, and the handler waits by cancelling the close. Otherwise it disposes the engine. Dispose stops a running preview, and there's no file to finish.

### Run it

Run the app from Visual Studio, and type the camera's RTSP address. Then the login, and the password, which shows as dots. Click Start. The camera answers in about three seconds, and the live picture appears, pixelated. Stop the preview, and close the window.

### Common mistakes

A few common mistakes. Leaving out the LAV redist, so the start fails. Putting the login and password into the URL, where they show up wherever the address does. A wrong path for the camera's brand, so the camera refuses the stream. And a network that blocks UDP, like a firewall, a NAT or a VPN. The camera connects, but the picture stays black. The RTSP_LowLatency engine reads RTSP over TCP by default.

### Next steps

Next, add the MP4 redist, switch the mode to IPCapture, set an output file, and the same app records the camera to MP4. Text can be drawn over the video, too. For several cameras, use one VideoCaptureCore and one VideoView for each. And ONVIF finds the cameras on the network for you. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [RTSP camera source configuration](../video-sources/ip-cameras/rtsp.md) — UDP/TCP transport, low-latency tuning, engine selection
- [ONVIF IP camera integration](../video-sources/ip-cameras/onvif.md) — auto-discovery, profile selection, PTZ control
- [Record RTSP stream to MP4](ip-camera-capture-mp4.md) — capture the previewed stream to a file

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to explore more code samples. Need the RTSP URL for your camera? Check our [IP camera brands directory](../../camera-brands/index.md).
