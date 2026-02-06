---
title: MP4 Video Output Integration for .NET
description: Implement MP4 output in .NET with H.264/HEVC hardware encoding, audio configuration, and optimized performance for video processing apps.
---

# MP4 file output

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

MP4 (MPEG-4 Part 14), introduced in 2001, is a digital multimedia container format most commonly used to store video and audio. It also supports subtitles and images. MP4 is known for its high compression and compatibility across various devices and platforms, making it a popular choice for streaming and sharing.

Capturing videos from a webcam and saving them to a file is a common requirement in many applications. One way to achieve this is by using a software development kit (SDK) like VisioForge Video Capture SDK .Net, which provides an easy-to-use API for capturing and processing videos in C#.

To capture video in MP4 format using Video Capture SDK, you need to configure video output format using one of the classes for MP4 output. You can use several available software and hardware video encoders, including Intel QuickSync, Nvidia NVENC, and AMD/ATI APU.

## Cross-platform MP4 output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

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

### File Splitting

The [MP4SplitSinkSettings](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.X.Sinks.MP4SplitSinkSettings.html) class provides automatic file splitting capabilities, allowing you to split MP4 output into multiple files based on size, duration, or timecode. This class can be used with both `MP4OutputBlock` (which includes encoding) and `MP4SinkBlock` (muxing only). This is particularly useful for:

- Long recording sessions that need to be broken into manageable file sizes
- Creating time-based segments for easier archival or distribution
- Managing disk space by limiting the number of files kept on storage
- Implementing rolling buffer recording where only recent files are retained

**Split by File Size**

Split the output when a file reaches a specific size in bytes:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Create split sink settings with filename pattern
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Split when file reaches 100 MB (104857600 bytes)
splitSettings.SplitFileSize = 104857600;

// Disable duration-based splitting (default is 1 minute)
splitSettings.SplitDuration = TimeSpan.Zero;

// Create output block
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Split by Duration**

Split the output when a file reaches a specific duration:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Create split sink settings with filename pattern
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Split every 5 minutes
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Create output block
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Limit Maximum Files**

Control the maximum number of files to keep on disk. Once the limit is reached, the oldest files are automatically deleted:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Create split sink settings
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Split every 10 minutes
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Keep only the last 6 files (1 hour of recordings)
splitSettings.SplitMaxFiles = 6;

// Create output block
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Split by Timecode**

Split the output based on timecode difference between first and last frame:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Create split sink settings
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Split when timecode difference reaches 1 hour
// Format: "HH:MM:SS:FF" where FF is frames
splitSettings.SplitMaxSizeTimecode = "01:00:00:00";

// Disable other splitting methods
splitSettings.SplitFileSize = 0;
splitSettings.SplitDuration = TimeSpan.Zero;

// Create output block
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Combined Settings**

You can combine splitting criteria. The file will split when any of the enabled criteria is met:

```csharp
var h264 = new OpenH264EncoderSettings();
var aac = new VOAACEncoderSettings();

// Create split sink settings
var splitSettings = new MP4SplitSinkSettings("recording_%05d.mp4");

// Split at 200 MB OR 10 minutes, whichever comes first
splitSettings.SplitFileSize = 209715200; // 200 MB
splitSettings.SplitDuration = TimeSpan.FromMinutes(10);

// Keep only last 12 files
splitSettings.SplitMaxFiles = 12;

// Start numbering from 1 instead of 0
splitSettings.StartIndex = 1;

// Create output block
var mp4Output = new MP4OutputBlock(splitSettings, h264, aac);
```

**Using with MP4SinkBlock**

For scenarios where you already have encoded streams and only need muxing (container creation), use `MP4SinkBlock` with `MP4SplitSinkSettings`:

```csharp
// Create split sink settings
var splitSettings = new MP4SplitSinkSettings("output_%05d.mp4");
splitSettings.SplitDuration = TimeSpan.FromMinutes(5);

// Create MP4 sink block (muxing only, no encoding)
var mp4Sink = new MP4SinkBlock(splitSettings);

// Connect your pre-encoded streams to the sink block
// (connection logic depends on your pipeline structure)
```

**Important Notes:**

- The filename parameter must include a format specifier (like `%05d`) for the file index
- Set `SplitFileSize` to 0 to disable size-based splitting (default is 0)
- The default `SplitDuration` is 1 minute; set to `TimeSpan.Zero` to disable duration-based splitting
- Set `SplitMaxFiles` to 0 to keep all files (default is 0, no deletion)
- When combining criteria, splitting occurs when ANY criterion is met
- The `StartIndex` property controls the initial file number (default is 0)

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

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

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