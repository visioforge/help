---
title: Record Webcam to MP4 in C# WPF with H.264 and AAC Audio
description: Build a WPF window in XAML that previews a webcam and records it to MP4 with GPU H.264 and AAC, using VisioForge Video Capture SDK. C# code and video.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCore
  - Windows
  - WPF
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
  id: NNqmz2vJyQc
  upload_date: 2026-11-10T16:30:06Z
  duration: PT9M

---

# Webcam to MP4 Video Capture in a WPF Application

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Webcam Capture in WPF

This tutorial records a webcam and a microphone to an MP4 file from a WPF application. The window is laid out in XAML, the live preview is drawn in a `VideoView`, and the `VideoCaptureCore` engine encodes H.264 on the GPU when the PC has a hardware encoder, with AAC audio.

It is the same app as the [WinForms webcam to MP4 tutorial](video-capture-webcam-mp4.md), built the WPF way:

- The window, its lists and its buttons are written in XAML
- The combo box handlers read `SelectedItem`, because `Text` still holds the previous item inside `SelectionChanged`
- Closing the window during a recording finishes the MP4 file before the window goes away

## Video Implementation Tutorial

The following video builds the application step by step:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/NNqmz2vJyQc?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

The complete project is available in our GitHub repository:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-mp4-wpf)

## Required Redistributable Components

Install the [VisioForge.DotNet.Core](https://www.nuget.org/packages/VisioForge.DotNet.Core/) package and the redistributable packages below. The WPF `VideoView` ships in the core package, so no separate UI package is needed.

### Video Capture Components

- For 32-bit applications: [x86 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- For 64-bit applications: [x64 Video Capture Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### MP4 Encoding Components

- For 32-bit applications: [x86 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/)
- For 64-bit applications: [x64 MP4 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## The Window in XAML

The `vf` prefix maps the `VisioForge.Core.UI.WPF` namespace of the `VisioForge.Core` assembly. The left column holds the lists and the buttons, the right column the `VideoView` where the live preview is drawn. Every handler named here is written in `MainWindow.xaml.cs` below.

```xml
<Window x:Class="WebcamMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" SelectionChanged="cbCamera_SelectionChanged" />
            <Label Content="Format" />
            <ComboBox x:Name="cbFormat" SelectionChanged="cbFormat_SelectionChanged" />
            <Label Content="Frame rate" />
            <ComboBox x:Name="cbFrameRate" />
            <Label Content="Microphone" />
            <ComboBox x:Name="cbMicrophone" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

## Implementation Example with C# Code

The code behind the window. The project targets .NET 10 with implicit usings; a WPF project leaves `System.IO` out of them, so it is added for `Path`.

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEffects;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCore videoCapture1;

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        foreach (var device in videoCapture1.Video_CaptureDevices())
            cbCamera.Items.Add(device.Name);

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;
    }

    private void cbCamera_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFormat.Items.Clear();
        cbFrameRate.Items.Clear();
        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        foreach (var format in device.VideoFormats)
            cbFormat.Items.Add(format.Name);

        var index = cbFormat.Items.IndexOf("1920x1080 MJPG, 24 bit");
        if (cbFormat.Items.Count > 0) cbFormat.SelectedIndex = index >= 0 ? index : 0;
    }

    private void cbFormat_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        cbFrameRate.Items.Clear();
        if (cbFormat.SelectedItem is null) return; // the list was just cleared

        var device = videoCapture1.Video_CaptureDevices().First(d => d.Name == (string)cbCamera.SelectedItem);
        var format = device.VideoFormats.First(f => f.Name == (string)cbFormat.SelectedItem);
        foreach (var frameRate in format.FrameRates)
            cbFrameRate.Items.Add(frameRate);

        var rate = cbFrameRate.Items.IndexOf(VideoFrameRate.FPS_30);
        if (cbFrameRate.Items.Count > 0) cbFrameRate.SelectedIndex = rate >= 0 ? rate : 0;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        Title = cbFrameRate.SelectedItem is null ? "No camera, format or frame rate" : "Webcam to MP4";
        if (cbFrameRate.SelectedItem is null) return;

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource((string)cbCamera.SelectedItem);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = (string)cbFormat.SelectedItem;
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
            Text = "Hello World!", Left = 50, Top = 50, FontColor = System.Drawing.Color.Red
        });
        videoCapture1.Video_Effects_Add(new VideoEffectTextLogo(true, "clock")
        {
            Mode = TextLogoMode.DateTime, DateTimeMask = "HH:mm:ss", Text = "",
            Left = 50, Top = 110
        });

        btStart.IsEnabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
    }

    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        if (btStart.IsEnabled) { videoCapture1.Dispose(); return; }

        e.Cancel = true; // still recording: finish the MP4 first
        if (!btStop.IsEnabled) return; // already starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

## Key Implementation Features

1. **Window in XAML** - A two-column `Grid`: the lists and buttons on the left, the `VideoView` with the live preview on the right
2. **Device and Format Selection** - The camera, its format, its frame rate and the microphone are picked from lists filled from the devices themselves; the handlers read `SelectedItem`
3. **Hardware Encoding with a CPU Fallback** - `MP4HWOutput` encodes H.264 on an NVIDIA, Intel or AMD GPU; `HWEncodersAvailable` tells which encoders the PC has, and `MS_H264` runs on the CPU when none is present
4. **Encoder Settings** - An average of 6000 kbps, peaks up to 9000 kbps, the Main profile and AAC audio at 192 kbps
5. **Asynchronous Operation** - `StartAsync` and `StopAsync` keep the UI responsive, and closing the window during a recording awaits `StopAsync` before it closes

## WPF Notes

- `FontColor` of `VideoEffectTextLogo` is a `System.Drawing.Color`, not a WPF `Color`, so it is written in full
- The `OnError` handler sets the window title through `Dispatcher.InvokeAsync`, so it runs on the UI thread whichever thread raises the event
- `Format_UseBest` is on by default and picks the highest resolution up to 1080p, preferring MJPG at the same resolution; turn it off to record the format picked in the list
- With no microphone in the list, `Audio_RecordAudio` is set to `false` and the MP4 holds video only
- When the engine has already stopped (after an error), `StopAsync` completes at once and the code after the `await` still runs inside `Closing`, where WPF does not allow `Close()`; the window closes again through `Dispatcher.InvokeAsync` instead

## Video Transcript

### What we build

Let's record a webcam to an MP4 file in a C# WPF app with the VisioForge Video Capture SDK. Here's what we build. A WPF window with a live camera preview, laid out in XAML. It records MP4, with H.264 encoded on the GPU when the PC has one, and AAC audio. The camera, its format and the microphone are picked from lists, and the code behind the window is about a hundred lines of C#. This is the finished app, recording from a Logitech C920.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. The same app in WinForms has its own video.

### Create a WPF project

In Visual Studio, open File, New, Project. Search for WPF, and pick WPF Application for C#. Name the project WebcamMp4Wpf. Choose .NET 10 and click Create. Visual Studio opens MainWindow.xaml, with the designer above and the XAML below.

### Add the NuGet packages

Now add three NuGet packages: the SDK itself, the video capture redist and the MP4 redist. These are the x64 packages. An x86 app takes the x86 redists instead. You can also install the same three packages from the NuGet Package Manager. The WPF VideoView ships in the core package, so nothing else is needed.

### Build the window in XAML

In WPF, the window is written in XAML. First the window itself, with a two-column grid and a VideoView, and the designer above draws it right away. The VideoView is also in the Toolbox, under VisioForge. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. Then a VideoView goes into the right-hand column of a grid: that is where the live preview is drawn. The window also names its Loaded and Closing handlers. Next, the labels and combo boxes go into the left column. The left-hand column is a StackPanel with a label and a combo box for the camera, the format, the frame rate and the microphone. The camera and format lists name a SelectionChanged handler. Last, the Start and Stop buttons. Each button names a Click handler. Stop stays disabled until a recording starts. Every handler named in the XAML goes into MainWindow.xaml.cs, and the next chapters write them.

### Create the capture engine

In the window's Loaded handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn.

### Enumerate cameras

Video_CaptureDevices returns every camera on the machine. We list their names and select the first one. When the camera changes, its handler fills the format list with the formats of that camera. Read SelectedItem here, not Text: inside SelectionChanged, the Text of a WPF combo box still holds the previous item. When the format changes, its handler fills the frame rate list the same way. Clearing the format list raises SelectionChanged with nothing selected, so the handler returns early. A camera reports its name, formats like 1920x1080 MJPG or 1280x720 YUY2, and the frame rates of each format. MJPG is compressed in the camera, so high resolutions fit through USB. YUY2 is uncompressed, so USB 2 carries only lower resolutions.

### Choose the camera format

Select the camera by name. Then turn Format_UseBest off, and pass the format and the frame rate picked from the lists. Format_UseBest is on by default. It picks the highest resolution up to 1080p that the camera offers, and prefers MJPG at the same resolution. For a fixed format, set it to false and pass the format and the frame rate yourself.

### Select the microphone

The microphone works the same way. Audio_CaptureDevices lists them, and we use the one picked in the list. Audio_PlayAudio set to false records the microphone without playing it back.

### MP4HWOutput: any GPU, CPU fallback

For the output, create an MP4HWOutput, write output.mp4 into the Videos folder, and switch the engine to video capture mode, so it records and not just previews. A WPF project leaves System.IO out of its implicit usings, so add it for Path. MP4HWOutput encodes on the GPU: NVENC on NVIDIA, Quick Sync on Intel, and AMF on AMD, in H.264 or HEVC. When there's no hardware encoder, the Windows encoder on the CPU takes over. The code and the file are the same on every PC. HWEncodersAvailable tells you which encoders this PC has. Take NVIDIA first, then Intel, then AMD, and fall back to the CPU encoder.

### Tune the H.264 encoder

Then tune the encoder: six thousand kilobits per second on average, peaks up to nine thousand, the Main profile, and AAC audio at a hundred and ninety-two. As a guide: 2500 to 4000 kilobits for 720p, 5000 to 8000 for 1080p, and 15000 to 25000 for 4K. Both bitrates are set in kilobits per second.

### Add a text overlay

Video effects add a text overlay: a fixed Hello World, and a clock that shows the current time on every frame. The font color is a System.Drawing color, not a WPF one, so write it in full.

### Start and stop

Start and Stop are async. StartAsync returns once the capture is running, and StopAsync finalizes the MP4 file. If the window closes during a recording, cancel the close, await StopAsync, and close again. The file is complete, and the engine is disposed on the second pass. Run the app from Visual Studio. The format list comes straight from the camera. Start begins the recording, with the clock overlay ticking. Stop finalizes output.mp4. The file lands in Videos: 1920x1080, H.264 video and AAC audio.

### Common mistakes

A few common mistakes. A missing redist package, so the engine won't start. A camera that's already open in another app. x86 and x64 packages mixed in one project. And disposing the engine mid-capture: await StopAsync first. Engine errors arrive in the OnError event.

### Next steps

There are also guides for IP cameras, screen recording, snapshots and network streaming. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See also

- [Webcam to MP4 in WinForms](video-capture-webcam-mp4.md) — the same app as a Windows Forms project
- [Pre-Event Recording](../guides/pre-event-recording.md) — circular buffer recording with motion detection triggers for webcams and IP cameras
- [Camera Video Preview](camera-video-preview.md) — display live camera video with frame capture

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples and implementation examples.
