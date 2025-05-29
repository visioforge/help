---
title: MP4 Video Output Integration for .NET
description: Learn how to implement MP4 video output in .NET applications using hardware-accelerated encoders. Guide covers H.264/HEVC encoding, audio configuration, and best practices for optimal video processing performance.
sidebar_label: MP4
---

# MP4 file output

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

MP4 (MPEG-4 Part 14), introduced in 2001, is a digital multimedia container format most commonly used to store video and audio. It also supports subtitles and images. MP4 is known for its high compression and compatibility across various devices and platforms, making it a popular choice for streaming and sharing.

Capturing videos from a webcam and saving them to a file is a common requirement in many applications. One way to achieve this is by using a software development kit (SDK) like VisioForge Video Capture SDK .Net, which provides an easy-to-use API for capturing and processing videos in C#.

To capture video in MP4 format using Video Capture SDK, you need to configure video output format using one of the classes for MP4 output. You can use several available software and hardware video encoders, including Intel QuickSync, Nvidia NVENC, and AMD/ATI APU.

## Cross-platform MP4 output

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

The [MP4Output](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Output.MP4Output.html?q=MP4Output) class provides a flexible and powerful way to configure MP4 video output settings for video capture and editing operations. This guide will walk you through how to use the MP4Output class effectively, covering its key features and common usage patterns.

MP4Output implements several important interfaces:

- IVideoEditXBaseOutput
- IVideoCaptureXBaseOutput
- Media Block creation

This makes it suitable for both video editing and capture scenarios while providing extensive control over video and audio processing.

### Basic Usage

The simplest way to create an MP4Output instance is using the constructor with a filename:

```csharp
var output = new MP4Output("output.mp4");
```

This creates an MP4Output with default video and audio encoder settings. On Windows, it will use OpenH264 for video encoding and Media Foundation AAC for audio encoding by default.

### Video Encoder Configuration

The MP4Output class supports multiple video encoders through its `Video` property. Here are the supported video encoders:

**[H.264 Encoders](../video-encoders/h264.md)**

- OpenH264EncoderSettings (Default, CPU)
- AMFH264EncoderSettings (AMD)
- NVENCH264EncoderSettings (NVIDIA)
- QSVH264EncoderSettings (Intel Quick Sync)

**[HEVC (H.265) Encoders](../video-encoders/hevc.md)**

- MFHEVCEncoderSettings (Windows only)
- AMFH265EncoderSettings (AMD)
- NVENCHEVCEncoderSettings (NVIDIA)
- QSVHEVCEncoderSettings (Intel Quick Sync)

You can check the availability of specific encoders using the `IsAvailable` method:

```csharp
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

Example of configuring a specific video encoder:

```csharp
var output = new MP4Output("output.mp4");
output.Video = new NVENCH264EncoderSettings(); // Use NVIDIA encoder
```

### Audio Encoder Configuration

The `Audio` property allows you to specify the audio encoder. Supported audio encoders include:

- [VOAACEncoderSettings](../audio-encoders/aac.md)
- [AVENCAACEncoderSettings](../audio-encoders/aac.md)
- [MFAACEncoderSettings](../audio-encoders/aac.md) (Windows only)
- [MP3EncoderSettings](../audio-encoders/mp3.md)

Example of setting a custom audio encoder:

```csharp
var output = new MP4Output("output.mp4");
output.Audio = new MP3EncoderSettings();
```

The MP4Output class automatically selects appropriate default encoders based on the platform.

### Sample code

Add the MP4 output to the Video Capture SDK core instance:

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(output, true);
```

Set the output format for the Video Edit SDK core instance:

```csharp
var core = new VideoEditCoreX();
core.Output_Format = output;
```

Create a Media Blocks MP4 output instance:

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var mp4SinkSettings = new MP4SinkSettings("output.mp4");
var mp4Output = new MP4OutputBlock(mp4SinkSettings, h264, aac);
```

### Best Practices

**Hardware Acceleration**: When possible, use hardware-accelerated encoders (NVENC, AMF, QSV) for better performance:

```csharp
var output = new MP4Output("output.mp4");
if (NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new NVENCH264EncoderSettings();
}
```

**Encoder Selection**: Use the provided methods to enumerate available encoders:

```csharp
var output = new MP4Output("output.mp4");
var availableVideoEncoders = output.GetVideoEncoders();
var availableAudioEncoders = output.GetAudioEncoders();
```

### Common Issues and Solutions

1. **File Access**: The MP4Output constructor attempts to verify write access by creating and immediately deleting a test file. Ensure the application has proper permissions to the output directory.

2. **Encoder Availability**: Hardware encoders might not be available on all systems. Always provide a fallback:

```csharp
var output = new MP4Output("output.mp4");
if (!NVENCH264EncoderSettings.IsAvailable())
{
    output.Video = new OpenH264EncoderSettings(); // Fallback to software encoder
}
```

3. **Platform Compatibility**: Some encoders are platform-specific. Use conditional compilation or runtime checks when targeting multiple platforms:

```csharp
#if NET_WINDOWS
    output.Audio = new MFAACEncoderSettings();
#else
    output.Audio = new MP3EncoderSettings();
#endif
```

## Windows-only MP4 output

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

`The same sample code can be used for Video Edit SDK .Net. Use the VideoEditCore class instead of VideoCaptureCore.`

### CPU encoder or Intel QuickSync GPU encoder  

Create an `MP4Output` object for MP4 output.

```cs
var mp4Output = new MP4Output();
```

Set MP4 mode to `CPU_QSV`.

```cs
mp4Output.MP4Mode = MP4Mode.CPU_QSV;
```

Set video settings.

```cs
mp4Output.Video.Profile = H264Profile.ProfileMain; // H264 profile
mp4Output.Video.Level = H264Level.Level4; // H264 level
mp4Output.Video.Bitrate = 2000; // bitrate

// optional parameters
mp4Output.Video.MBEncoding = H264MBEncoding.CABAC; //CABAC / CAVLC
mp4Output.Video.BitrateAuto = false; // true to use auto bitrate
mp4Output.Video.RateControl = H264RateControl.VBR; // rate control - CBR or VBR
```

Set AAC audio settings.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW or ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // type of AAC
```

### Nvidia NVENC encoder  

Create the `MP4Output` object for MP4 output.

```cs
var mp4Output = new MP4Output();
```

Set MP4 mode to `NVENC`.

```cs
mp4Output.MP4Mode = MP4Mode.NVENC;
```

Set the video settings.

```cs
mp4Output.Video_NVENC.Profile = NVENCVideoEncoderProfile.H264_Main; // H264 profile
mp4Output.Video_NVENC.Level = NVENCEncoderLevel.H264_4; // H264 level
mp4Output.Video_NVENC.Bitrate = 2000; // bitrate

// optional parameters
mp4Output.Video_NVENC.RateControl = NVENCRateControlMode.VBR; // rate control - CBR or VBR
```

Set the audio settings.

```cs
mp4Output.Audio_AAC.Bitrate = 192;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio_AAC.Output = AACOutput.RAW; // RAW or ADTS
mp4Output.Audio_AAC.Object = AACObject.Low; // type of AAC
```

### CPU/GPU encoders  

Using MP4 HW output, you can use hardware-accelerated encoders by Intel (QuickSync), Nvidia (NVENC), and AMD/ATI.

Create `MP4HWOutput` object for MP4 HW output.

```cs
var mp4Output = new MP4HWOutput();
```

Get available encoders.

```cs
var availableEncoders = VideoCaptureCore.HWEncodersAvailable();
// or
var availableEncoders = VideoEditCore.HWEncodersAvailable();
```

Depending on available encoders, select video codec.

```cs
mp4Output.Video.Codec = MFVideoEncoder.MS_H264; // Microsoft H264
mp4Output.Video.Profile = MFH264Profile.Main; // H264 profile
mp4Output.Video.Level = MFH264Level.Level4; // H264 level
mp4Output.Video.AvgBitrate = 2000; // bitrate

// optional parameters
mp4Output.Video.CABAC = true; // CABAC / CAVLC
mp4Output.Video.RateControl = MFCommonRateControlMode.CBR; // rate control

// many other parameters are available
```

Set audio settings.

```cs
mp4Output.Audio.Bitrate = 192;
mp4Output.Audio.Version = AACVersion.MPEG4; // MPEG-4 / MPEG-2
mp4Output.Audio.Output = AACOutput.RAW; // RAW or ADTS
mp4Output.Audio.Object = AACObject.Low; // type of AAC
```

Now, we can apply MP4 output settings to the core class (VideoCaptureCore or VideoEditCore) and start video capture or editing.

### Apply video capture settings

Set MP4 format settings for output.

```cs
core.Output_Format = mp4Output;
```

Set a video capture mode (or video convert mode if you use Video Edit SDK).

```cs
core.Mode = VideoCaptureMode.VideoCapture;
```

Set a file name (ensure you have to write access rights).

```cs
core.Output_Filename = "output.mp4";
```

Start video capture (convert) to a file.

```cs
await VideoCapture1.StartAsync();
```

Finally, when we're done capturing the video, we need to stop the video capture and release the resources. We can do this by calling the `StopAsync` method of the `VideoCaptureCore` class.

### Required redists  

- Video Capture SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
