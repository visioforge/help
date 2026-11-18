---
title: Network Streaming in C# .NET — RTSP, RTMP, NDI, SRT
description: Stream live video over RTSP, RTMP, NDI, HLS, and SRT from VisioForge Video Capture SDK. Protocol setup, encoder config, and C# code examples.
sidebar_label: Network Streaming
order: 5
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Streaming
video:
  id: _mkmAxqQesw
  upload_date: 2026-11-17T11:35:00Z
  duration: PT10M8S

---

# Network Streaming in .NET Applications

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .NET](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to Network Streaming

Network streaming has become a fundamental component of modern digital communication infrastructure, enabling real-time transmission of audio and video data across diverse network environments. As bandwidth capabilities continue to expand and user expectations for content delivery evolve, developers need robust tools to implement streaming solutions that balance performance, quality, and reliability.

The VisioForge Video Capture SDK for .NET provides comprehensive support for multiple streaming protocols, offering developers a versatile toolkit for integrating sophisticated streaming capabilities into their applications. Whether you're building live broadcasting platforms, video conferencing tools, surveillance systems, or content delivery networks, this SDK provides the foundation for implementing professional-grade streaming solutions.

## Video Tutorial — Stream a Webcam over RTMP in C# WPF

This tutorial builds a WPF app on the `VideoCaptureCoreX` engine that shows a live webcam preview and, at the same time, streams the camera and the microphone over RTMP:

- the video is H.264, encoded on the GPU when the PC has a hardware encoder (NVIDIA NVENC, AMD AMF, Intel Quick Sync) and with OpenH264 on the CPU otherwise; the audio is AAC;
- the stream goes to any RTMP server: MediaMTX, YouTube Live, or a CDN ingest;
- in the video it goes to a MediaMTX server on the local network, and a browser plays it back from that server.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/_mkmAxqQesw?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

### Source Code

The complete project is on GitHub: [network-streaming-rtmp-wpf](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/network-streaming-rtmp-wpf).

### Required Packages

The SDK, CrossPlatform Core (the GStreamer runtime the X engine runs on, with the RTMP sink and the H.264 encoders) and CrossPlatform Libav (the AAC encoder):

```bash
dotnet add package VisioForge.DotNet.VideoCapture
dotnet add package VisioForge.CrossPlatform.Core.Windows.x64
dotnet add package VisioForge.CrossPlatform.Libav.Windows.x64
```

These are the x64 packages, so set `PlatformTarget` to `x64` in the project file. A 32-bit app takes the x86 packages instead.

### The Window in XAML

A two-column grid: the camera and microphone lists, the RTMP URL and the Start and Stop buttons on the left, and the `VideoView` that draws the preview on the right. Both buttons start disabled: Start until the SDK has loaded, Stop until a stream runs.

```xml
<Window x:Class="WebcamRtmpWpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Webcam to RTMP" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Camera" />
            <ComboBox x:Name="cbCamera" />
            <Label Content="Microphone" Margin="0,10,0,0" />
            <ComboBox x:Name="cbMicrophone" />
            <Label Content="RTMP URL" Margin="0,10,0,0" />
            <TextBox x:Name="edURL" Text="rtmp://localhost:1935/webcam" />

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

### The Code

```csharp
using System.ComponentModel;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace WebcamRtmpWpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

    private VideoCaptureDeviceInfo[] cameras;

    private AudioCaptureDeviceInfo[] microphones;

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

        cameras = await DeviceEnumerator.Shared.VideoSourcesAsync();
        foreach (var device in cameras)
            cbCamera.Items.Add(device.DisplayName);

        microphones = await DeviceEnumerator.Shared.AudioSourcesAsync();
        foreach (var device in microphones)
            cbMicrophone.Items.Add(device.DisplayName);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        if (cbCamera.SelectedIndex < 0)
        {
            Title = "No camera";
            return;
        }

        if (string.IsNullOrWhiteSpace(edURL.Text))
        {
            Title = "Enter the RTMP URL";
            return;
        }

        Title = "Webcam to RTMP";

        // 1080p at 25 fps or more when the camera has it, otherwise 720p, otherwise another format
        videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(cameras[cbCamera.SelectedIndex]);

        var hasMic = cbMicrophone.SelectedIndex >= 0;
        videoCapture1.Audio_Source = hasMic ? microphones[cbMicrophone.SelectedIndex].CreateSourceSettingsVC() : null;
        videoCapture1.Audio_Record = hasMic; // no microphone: stream video only
        videoCapture1.Audio_Play = false; // stream the microphone without playing it back

        // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new RTMPOutput(edURL.Text.Trim()));

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
        if (btStart.IsEnabled)
        {
            videoCapture1.Dispose();
            VisioForgeX.DestroySDK();
            return;
        }

        e.Cancel = true; // still loading or streaming: stop the stream first
        if (!btStop.IsEnabled) return; // already loading, starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

- `VideoCaptureDeviceSourceSettings` takes 1080p at 25 frames per second or more when the camera has it, otherwise 720p, otherwise another format.
- An empty URL is stopped in the Start handler: the RTMP sink would fall back to its own default server instead of failing.
- An output stays in the engine after a stream, so `Outputs_Clear` runs before each new one.
- The `Video` and `Audio` properties of `RTMPOutput` take other encoder settings, for a fixed bitrate for example.
- Engine errors arrive in the `OnError` event; the app shows them in the title bar, and that is where a server that cannot be reached shows up.

### The RTMP URL

- **MediaMTX**: `rtmp://<server>:1935/<name>`. The path after the port is the stream's name, and the same path plays back over WebRTC (`http://<server>:8889/<name>`, the player shown in the video), HLS and RTSP.
- **YouTube Live**: the ingest address followed by your stream key.
- **rtmps://** connects over TLS.

Other destinations take other outputs, and the rest of the code stays the same: `SRTOutput` takes a URL like `RTMPOutput`, `YouTubeOutput` and `FacebookLiveOutput` take the stream key, and `RTSPServerOutput` and `HLSOutput` take a settings object (`RTSPServerSettings`, `HLSSinkSettings`).

## Quick Start — RTMP streaming with VideoCaptureCoreX

The same stream without a window: with no `VideoView` there is no preview, so the engine is created without one. The engine calls are the same on every platform the X engine runs on; the packages differ per platform, and on Android and iOS the app asks for camera and microphone permission before it lists the devices.

```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

// Initialize the SDK once per process.
await VisioForgeX.InitSDKAsync();

// No VideoView: no preview. Errors arrive in OnError.
var videoCapture = new VideoCaptureCoreX();
videoCapture.OnError += (_, e) => Console.WriteLine(e.Message);

// The first camera and microphone; stream video only when there is no microphone.
var camera = (await DeviceEnumerator.Shared.VideoSourcesAsync()).FirstOrDefault();
var microphone = (await DeviceEnumerator.Shared.AudioSourcesAsync()).FirstOrDefault();
if (camera == null) return;

videoCapture.Video_Source = new VideoCaptureDeviceSourceSettings(camera);
videoCapture.Audio_Source = microphone?.CreateSourceSettingsVC();
videoCapture.Audio_Record = microphone != null;
videoCapture.Audio_Play = false; // do not play the microphone back

// The full ingest URL, including the stream key.
videoCapture.Outputs_Add(new RTMPOutput("rtmp://live.example.com/app/<stream-key>"));
await videoCapture.StartAsync();

// ... and when the stream should end:
await videoCapture.StopAsync();
videoCapture.Dispose();
VisioForgeX.DestroySDK();
```

For per-protocol configuration (encoder tuning, buffer sizes, authentication, adaptive bitrates, etc.) see the dedicated pages under [Available Protocols](#available-protocols) below.

## Core Network Streaming Protocols

### RTSP (Real-Time Streaming Protocol)

RTSP is an application-level protocol designed for controlling the delivery of data with real-time properties. It serves as a "network remote control" for multimedia servers, establishing and controlling media sessions between endpoints.

#### Key RTSP Features:

- **Session Control**: Enables clients to establish and manage media sessions with servers
- **Transport Independence**: Works with various transport protocols including UDP, TCP, and multicast UDP
- **Command Support**: Implements commands like PLAY, PAUSE, and RECORD for granular session control
- **Scalability**: Supports unicast and multicast delivery for efficient bandwidth utilization

### RTMP (Real-Time Messaging Protocol)

Originally developed by Macromedia for streaming Flash content, RTMP has evolved into one of the most widely used protocols for live broadcasting. It maintains persistent connections between the client and server, facilitating low-latency transmission crucial for interactive applications.

#### Key RTMP Features:

- **Low Latency**: Typically delivers sub-second latency, making it suitable for interactive streaming
- **Reliable Delivery**: Uses TCP as its transport layer for reliable packet delivery
- **Content Protection**: Supports encryption for secure content delivery
- **Widespread Support**: Compatible with numerous CDNs and streaming platforms

### NDI (Network Device Interface)

NDI represents a significant advancement in professional video production workflows, enabling high-quality, low-latency video transmission over standard IP networks. Developed by NewTek, NDI has gained widespread adoption in broadcast and production environments.

#### Key NDI Features:

- **IP-Based Workflow**: Leverages existing network infrastructure without specialized hardware
- **Bidirectional Communication**: Supports metadata and control data alongside audio/video
- **Discovery Mechanism**: Automatic device and source discovery on local networks
- **High-Quality Encoding**: Maintains visual quality while optimizing for network conditions

### HLS (HTTP Live Streaming)

Developed by Apple, HLS has become one of the most widely supported streaming protocols. It segments content into small HTTP-based file downloads, allowing adaptive bitrate streaming that adjusts quality based on the viewer's available bandwidth.

#### Key HLS Features:

- **Adaptive Bitrate**: Dynamically adjusts stream quality based on network conditions
- **Wide Compatibility**: Supported across most modern browsers and devices
- **HTTP Delivery**: Uses standard web servers and CDNs for efficient content delivery
- **Content Protection**: Supports encryption and DRM integration

### SRT (Secure Reliable Transport)

SRT is an open-source protocol optimized for delivering high-quality, low-latency video over unpredictable networks. It combines the reliability of TCP with the speed of UDP while adding security features.

#### Key SRT Features:

- **Packet Loss Recovery**: Implements dynamic retransmission mechanisms
- **Encryption**: Built-in AES encryption for secure transmission
- **Network Health Monitoring**: Continually assesses connection quality
- **Latency Control**: Configurable latency settings to balance reliability and delay

## Additional Supported Protocols

### HTTP MJPEG (Motion JPEG)

MJPEG over HTTP transmits a sequence of JPEG images, providing a simple yet effective streaming solution. While not as bandwidth-efficient as modern codecs, its simplicity and compatibility make it suitable for certain applications.

### UDP (User Datagram Protocol)

UDP streaming prioritizes speed over reliability, making it ideal for real-time applications where occasional packet loss is preferable to increased latency. The SDK provides configurable buffer settings to help optimize UDP streaming for specific network conditions.

### WMV (Windows Media Video)

The SDK supports streaming in Microsoft's WMV format, which remains relevant for certain Windows-centric applications and legacy systems integration.

### Platform-Specific Streaming

The SDK also integrates with popular streaming platforms including:

- **YouTube Live**: Direct streaming to YouTube channels
- **Facebook Live**: Integrated Facebook Live broadcasting
- **AWS S3**: Cloud-based media distribution via Amazon Web Services

## Implementation Considerations

When implementing network streaming with the SDK, developers should consider:

1. **Bandwidth Requirements**: Estimate and test the required bandwidth for your target quality and framerate
2. **Network Resilience**: Implement appropriate error handling and reconnection logic
3. **Quality vs. Latency**: Balance visual quality against latency requirements
4. **Cross-Platform Compatibility**: Select protocols based on your target platforms
5. **Security Needs**: Implement encryption and authentication where required

## Conclusion

The VisioForge Video Capture SDK for .NET provides comprehensive support for contemporary streaming protocols, empowering developers to implement sophisticated streaming solutions without managing the complexity of protocol implementations directly. From low-latency live broadcasts to secure content delivery, the SDK's streaming capabilities address diverse use cases across industries.

By leveraging these capabilities, developers can focus on building compelling experiences while relying on the SDK to handle the technical challenges of reliable network streaming. Whether you're developing applications for entertainment, education, security, or professional video production, the SDK provides a solid foundation for your streaming needs.

## Available Protocols

* [Adobe Flash Server](../../general/network-streaming/adobe-flash.md)
* [AWS S3](../../general/network-streaming/aws-s3.md)
* [Facebook](../../general/network-streaming/facebook.md)
* [HLS](../../general/network-streaming/hls-streaming.md)
* [HTTP MJPEG](../../general/network-streaming/http-mjpeg.md)
* [NDI](../../general/network-streaming/ndi.md)
* [RTMP](../../general/network-streaming/rtmp.md)
* [RTSP](../../general/network-streaming/rtsp.md)
* [SRT](../../general/network-streaming/srt.md)
* [UDP](../../general/network-streaming/udp.md)
* [WMV](../../general/network-streaming/wmv.md)
* [YouTube](../../general/network-streaming/youtube.md)


## Video Transcript

### What we build

Let's preview a webcam and stream it to the network over RTMP, in a C# WPF app with the VisioForge Video Capture SDK. Here's what we build. A WPF app that shows a live webcam preview and, at the same time, streams the camera and the microphone over RTMP. The video is H.264, encoded on the GPU when the PC has a hardware encoder, and the audio is AAC. The stream goes to any RTMP server: MediaMTX, YouTube Live, or a CDN. This is the finished app on the left, streaming to a MediaMTX server on the local network. On the right, a browser plays the same stream back from that server. Why RTMP? It is the ingest protocol of YouTube Live, Twitch and most CDNs. A media server takes it in and serves it again as HLS, WebRTC or RTSP, so viewers need nothing special. And it carries H.264 and AAC, which every player can decode. SRT, RTSP, HLS and WHIP are outputs of the same engine, added the same way.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. And a webcam, and an RTMP server to stream to. Here it is MediaMTX, a free, open-source media server: one executable that takes RTMP on port 1935.

### Create a WPF project

In Visual Studio, create a new project. Search for WPF, and pick WPF Application for C#. Name the project WebcamRtmpWpf. Choose .NET 10 and click Create.

### Add the NuGet packages

Now add three NuGet packages. The Video Capture SDK itself. CrossPlatform Core, the GStreamer runtime the X engine runs on, with the RTMP sink and the H.264 encoders. And CrossPlatform Libav, which holds the AAC encoder. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.VideoCapture from the Browse tab. Then install the Core and Libav packages for Windows x64. The Installed tab lists all three. These are the x64 packages, so set PlatformTarget to x64 in the project file. A 32-bit app takes the x86 packages instead.

### Build the window in XAML

The window is written in XAML. First the window itself, with a two-column grid and a VideoView, and the designer draws it right away. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. The VideoView goes into the right-hand column: that is where the live preview is drawn. The window also names its Loaded and Closing handlers. Next, the camera and microphone lists, and a text box for the RTMP address. The left-hand column is a StackPanel: a combo box for the cameras, one for the microphones, and a text box for the RTMP URL. Its default points at a MediaMTX server on this computer. Last, the Start and Stop buttons. Each button names a Click handler. Both start disabled: Start until the SDK has loaded, and Stop until a stream runs. Every handler the XAML names goes into MainWindow.xaml.cs, and the next chapters write them.

### Initialize the SDK

MainWindow.xaml.cs starts with these usings: the SDK core, the X output and source types, and the VideoCaptureCoreX engine. When the window loads, await VisioForgeX.InitSDKAsync once. Then create the VideoCaptureCoreX engine and pass it the VideoView, where the preview is drawn. Engine errors arrive in the OnError event; here they go to the title bar, and that is also where a server that cannot be reached shows up.

### List the devices

DeviceEnumerator.Shared.VideoSourcesAsync lists the cameras. Keep the array: the Start handler takes the selected device from it, and the combo box shows each display name. AudioSourcesAsync lists the microphones the same way. The first camera and the first microphone are selected, and Start is enabled only now, once the SDK has loaded.

### Pick the camera and the microphone

The Start handler checks first that there is a camera and an RTMP address. An empty address would not fail: the sink would fall back to its own default server, so it is stopped here. The video source is VideoCaptureDeviceSourceSettings for the selected camera. Out of the box it takes 1080p at 25 frames per second or more when the camera has it, and 720p otherwise. The microphone becomes the audio source through CreateSourceSettingsVC, and Audio_Record puts it into the stream. Audio_Play is false, so the app does not play the microphone back into the speakers.

### Stream over RTMP

The stream itself is one line: an RTMPOutput with the URL from the text box, added to the engine. The preview keeps running in the VideoView while the stream goes out. An output stays in the engine after a stream, so Outputs_Clear runs before each new one. With no encoder settings, RTMPOutput takes the GPU's H.264 encoder: NVIDIA NVENC, AMD AMF or Intel Quick Sync, and OpenH264 on the CPU when the PC has none of them. The audio is AAC. Its Video and Audio properties take other encoder settings, for a fixed bitrate for example. The URL names the server and the stream. On MediaMTX the path after the port is the stream's name, and the same path plays back over HLS, WebRTC and RTSP. For YouTube Live it is the ingest address followed by your stream key. And rtmps with an s connects over TLS.

### Start and stop

StartAsync returns whether the capture started, and the buttons follow it. StopAsync ends the stream and the preview. If the window closes while it streams, cancel the close, await StopAsync, and close again. On that second pass the engine is disposed and the SDK released.

### Run it

Run the app from Visual Studio. Once the SDK has loaded, the camera and microphone lists fill and Start is enabled. Type the address of the MediaMTX server on the local network, with webcam as the stream's name. Click Start. The preview runs, and the same picture goes out to the server. Now the other side. MediaMTX serves every stream in its own web player, on port 8889 with the same path. The browser on the right plays the stream back from the server, a moment behind the preview. Close the player, click Stop, and close the window.

### Common mistakes

A few common mistakes. Leaving out the Libav package, which holds the AAC encoder. Mixing x86 and x64 packages, or running an x86 app with the x64 ones. A firewall that blocks port 1935 to the server. And a YouTube address without the stream key at the end. Whatever goes wrong, the engine reports it in the OnError event.

### Next steps

There are also guides for the SRT, RTSP, HLS and WebRTC outputs, for recording an MP4 while you stream, for streaming an IP camera or the screen, and for YouTube Live and Facebook Live. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
