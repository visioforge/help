---
title: IP Camera Two-Way Audio in C# WPF — ONVIF Backchannel
description: Talk through an IP camera speaker from C# WPF - RTSP preview with the camera's sound, PC microphone sent as G.711 over the ONVIF audio backchannel.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WPF
  - Capture
  - IP Camera
  - RTSP
  - ONVIF
  - Audio
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - RTSPSourceSettings
  - AudioRendererSettings
  - DeviceEnumerator
  - VisioForgeX

---

# IP Camera Two-Way Audio in C# WPF

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

This tutorial builds a small intercom in a C# WPF app with the cross-platform `VideoCaptureCoreX` engine:

- the camera's RTSP URL, its login and its password are typed into the window;
- a `VideoView` shows the camera live, and the camera's own microphone plays on the PC speakers picked in the window;
- the PC microphone picked in the window plays on the camera speaker, sent over the ONVIF RTSP audio backchannel;
- Start and Stop open and close both directions at once.

It suits video doorbells, intercom stations and cameras with a speaker or an audio output. How the backchannel works, the Media Blocks version and the speaker volume over ONVIF are on the [two-way audio](../video-sources/ip-cameras/two-way-audio.md) page.

!!! note "SDK version"
    `Audio_BackchannelSource` is in the SDK releases after 2026.11.16.

## Source Code Repository

The complete project is on GitHub:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/ip-camera-two-way-audio-wpf)

## Required Packages

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — the SDK; the WPF `VideoView` ships in it
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — the GStreamer runtime the X engine runs on, with the RTSP source, the decoders and the G.711 encoder the backchannel uses
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — the AAC decoder, for cameras that send AAC sound

These are the x64 packages, so the project sets `<PlatformTarget>x64</PlatformTarget>`; a 32-bit app takes the `.x86` packages instead. See [Windows deployment](../../deployment-x/Windows.md) for the full list.

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

## Implementation Example with C# Code

### The window (MainWindow.xaml)

The settings are on the left, the camera on the right. `PasswordBox` shows dots. Start is disabled until the SDK has loaded, and Stop until the camera has started.

```xml
<Window x:Class="IpCameraTwoWayAudioWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="IP Camera Two-Way Audio" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera URL" />
            <TextBox x:Name="edURL" />
            <Label Content="Login" Margin="0,10,0,0" />
            <TextBox x:Name="edLogin" />
            <Label Content="Password" Margin="0,10,0,0" />
            <PasswordBox x:Name="edPassword" />
            <Label Content="Microphone (to the camera speaker)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="Speakers (the camera's sound)" Margin="0,10,0,0" />
            <ComboBox x:Name="cbSpeakers" />

            <StackPanel Orientation="Horizontal" Margin="0,20,0,0">
                <Button x:Name="btStart" Content="Start" Width="100" IsEnabled="False"
                        Click="btStart_Click" />
                <Button x:Name="btStop" Content="Stop" Width="100" Margin="10,0,0,0"
                        IsEnabled="False" Click="btStop_Click" />
            </StackPanel>
        </StackPanel>

        <vf:VideoView x:Name="VideoView1" Grid.Column="1" Background="Black" />
    </Grid>
</Window>
```

### The code (MainWindow.xaml.cs)

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.AudioRenderers;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace IpCameraTwoWayAudioWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private AudioCaptureDeviceInfo[] microphones;

    private AudioOutputDeviceInfo[] speakers;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        await VisioForgeX.InitSDKAsync();

        videoCapture1 = new VideoCaptureCoreX(VideoView1);

        // Show SDK errors in the title bar
        videoCapture1.OnError += (_, args) => Dispatcher.InvokeAsync(() => Title = args.Message);

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.DisplayName);

        // Preselect the system's default output
        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = Math.Max(0, Array.FindIndex(speakers, device => device.IsDefault));

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (!Uri.TryCreate(edURL.Text, UriKind.Absolute, out var url))
        {
            Title = "Enter the camera URL";
            return;
        }

        if (cbMicrophone.SelectedIndex < 0)
        {
            Title = "No microphone";
            return;
        }

        Title = "Connecting...";
        btStart.IsEnabled = false;

        var started = false;
        try
        {
            // The camera's video and its own microphone, played on the chosen speakers
            videoCapture1.Video_Source = await RTSPSourceSettings.CreateAsync(url, edLogin.Text, edPassword.Password, audioEnabled: true);
            videoCapture1.Audio_OutputDevice = cbSpeakers.SelectedIndex < 0 ? null : new AudioRendererSettings(speakers[cbSpeakers.SelectedIndex]);
            videoCapture1.Audio_Play = true;

            // The PC microphone, sent to the camera speaker over the ONVIF audio backchannel
            videoCapture1.Audio_BackchannelSource = microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC();

            started = await videoCapture1.StartAsync();
        }
        catch (Exception ex)
        {
            Title = ex.Message;
        }

        // An SDK error posted during the start stays in the title
        if (Title == "Connecting...") Title = started ? "Talking to the camera" : "The camera did not start";
        btStop.IsEnabled = started;
        btStart.IsEnabled = !started;
    }

    private async void btStop_Click(object sender, RoutedEventArgs e)
    {
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        Title = "IP Camera Two-Way Audio";
        btStart.IsEnabled = true;
    }

    private async void Window_Closing(object sender, CancelEventArgs e)
    {
        if (btStart.IsEnabled)
        {
            videoCapture1.Dispose();
            VisioForgeX.DestroySDK();
            return;
        }

        e.Cancel = true; // still loading or talking: stop first
        if (!btStop.IsEnabled) return; // already loading, starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

## Key Implementation Features

1. **SDK initialization** - `VisioForgeX.InitSDKAsync` runs once, when the window loads. The first run on a PC takes longer, so Start is enabled only after it returns. `VisioForgeX.DestroySDK` releases it when the window closes
2. **The camera** - `RTSPSourceSettings.CreateAsync` takes the URL, the login and the password as separate arguments, so the URL does not carry them. With `audioEnabled: true` the camera's own sound is used when its stream has one, and `Audio_Play` plays it on the speakers set in `Audio_OutputDevice`, since `Audio_Source` is left empty. Without `Audio_OutputDevice` the sound is not played. The [IP camera brands directory](../../camera-brands/index.md) lists the RTSP paths of many cameras
3. **The backchannel** - `Audio_BackchannelSource` takes the same audio sources as `Audio_Source`; `CreateSourceSettingsVC` turns the chosen microphone into one. At the start the SDK asks the camera for its ONVIF audio backchannel and sends the microphone to it as G.711 mu-law (PCMU), 8000 Hz, the codec ONVIF requires from every camera with an audio output; a camera whose backchannel offers only G.711 A-law (PCMA) gets A-law. The sound leaves the PC as it is captured, without the RTSP receive buffer added on top
4. **Starting** - a camera that does not answer takes a while to fail, so the title says "Connecting..." and Start is disabled until `StartAsync` returns. An `OnError` message posted during the start stays in the title
5. **Stopping and closing** - `StopAsync` closes both directions. Closing the window while it runs cancels the close, awaits `StopAsync`, and closes again; while the SDK is loading, or a start or a stop is under way, the close is cancelled: close the window again once it is done

## Troubleshooting

- **The camera refuses the connection** - the path does not match the camera's brand, or the login or password is wrong. Several wrong passwords in a row make many cameras lock the account for a while
- **No picture at all, with the right URL and password** - asking for the backchannel adds the ONVIF `Require: www.onvif.org/ver20/backchannel` header to the DESCRIBE request and to the backchannel's SETUP, and a camera without a backchannel may refuse either (it answers 551 Option Not Supported): an error then replaces the title, even when the start has already reported success. Leave `Audio_BackchannelSource` empty and start again: if the picture comes, the camera has no backchannel
- **The picture comes, but the camera speaker is silent** - the camera offers no backchannel, or neither G.711 mu-law nor A-law at 8000 Hz: the SDK names the codecs the camera offers in the window title. Check that two-way audio is on in the camera's web interface, that no vendor app or NVR holds its talk channel, and raise the speaker volume (see [Speaker Volume over ONVIF](../video-sources/ip-cameras/two-way-audio.md#speaker-volume-over-onvif))
- **You do not hear yourself through the camera's microphone** - this is expected. Cameras with a speaker cancel it in their microphone, so check the speaker by ear, next to the camera
- **The person at the camera hears an echo** - the PC plays the camera's sound on its speakers while its microphone sends to the camera, so the PC microphone picks the camera's sound up again and sends it back. Use a headset on the PC, or set `Audio_Play` to `false`

## Related Guides

- [Two-way audio with IP cameras](../video-sources/ip-cameras/two-way-audio.md) — how the backchannel works, Media Blocks, speaker volume over ONVIF
- [Record an IP camera to MP4](ip-camera-capture-mp4.md) — the same camera connection, recorded to a file
- [RTSP camera source configuration](../video-sources/ip-cameras/rtsp.md) — UDP/TCP transport, low-latency tuning
- [ONVIF IP camera integration](../video-sources/ip-cameras/onvif.md) — discovery and profile selection

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples. Need the RTSP URL for your camera? Check our [IP camera brands directory](../../camera-brands/index.md).
