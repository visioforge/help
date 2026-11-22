---
title: FFMPEG.exe Output Format Configuration in .NET Video SDKs
description: Configure FFMPEG.exe output in .NET for video capture and editing with hardware acceleration, custom codecs, and professional encoding options.
tags:
  - Video Capture SDK
  - Video Edit SDK
  - .NET
  - VideoCaptureCore
  - VideoEditCore
  - Windows
  - Capture
  - Encoding
  - Editing
  - MP4
  - WebM
  - AVI
  - MOV
  - TS
  - H.264
  - H.265
  - VP9
  - Opus
  - Webcam
  - WinForms
  - ProRes
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCore
  - VideoEditCore
  - FFMPEGEXEOutput
  - H264MFSettings
  - H264NVENCSettings
  - HEVCMFSettings
  - BasicVideoSettings
  - BasicAudioSettings
  - VideoCaptureSource
video:
  id: bNn7p2H6i3M
  upload_date: 2026-11-17T06:59:13Z
  duration: PT13M6S

---

# FFMPEG.exe Integration with VisioForge .NET SDKs

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

!!! note "Edition"
    FFmpeg.exe output requires the Premium edition in both `VideoCaptureCore` and `VideoEditCore`. With a Standard or Professional license the job runs in trial mode, with the trial watermark, only while the 30-day trial lasts; after that `Start` returns `false`.

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/bNn7p2H6i3M?controls=1" title="Record a webcam to MP4 and WebM with FFmpeg in C# - YouTube tutorial" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)

The video creates the project with .NET 10, which requires Visual Studio 2026; in Visual Studio 2022, choose .NET 8 or .NET 9.

## Introduction to FFMPEG Output in .NET

This guide provides detailed instructions for implementing FFMPEG.exe output in Windows applications using VisioForge's .NET SDKs. The integration works with both [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) and [Video Edit SDK .NET](https://www.visioforge.com/video-edit-sdk-net), utilizing the `VideoCaptureCore` and `VideoEditCore` engines.

FFMPEG functions as a powerful multimedia framework that enables developers to output to a wide variety of video and audio formats. Its flexibility stems from extensive codec support and granular control over encoding parameters for both video and audio streams.

## Why Use FFMPEG with VisioForge SDKs?

Integrating FFMPEG into your VisioForge-powered applications provides several technical advantages:

- **Format versatility**: Support for virtually all modern container formats
- **Codec flexibility**: Access to both open-source and proprietary codecs
- **Performance optimization**: Options for CPU and GPU acceleration
- **Customization depth**: Fine-grained control over encoding parameters

## Key Features and Capabilities

### Supported Output Formats

FFMPEG supports numerous container formats, including but not limited to:

- MP4 (MPEG-4 Part 14)
- WebM (VP8/VP9 with Vorbis/Opus)
- MKV (Matroska)
- AVI (Audio Video Interleave)
- MOV (QuickTime)
- WMV (Windows Media Video)
- FLV (Flash Video)
- TS (MPEG Transport Stream)

### Hardware Acceleration Options

Modern video encoding benefits from hardware acceleration technologies that significantly improve encoding speed and efficiency:

- **Intel QuickSync**: Leverages Intel integrated graphics for H.264 and HEVC encoding
- **NVIDIA NVENC**: Utilizes NVIDIA GPUs for accelerated encoding (requires compatible NVIDIA graphics card)
- **AMD AMF/VCE**: Employs AMD graphics processors for encoding acceleration

### Video Codec Support

The integration offers access to multiple video codecs with customizable parameters:

- **H.264/AVC**: Industry standard with excellent quality-to-size ratio
- **H.265/HEVC**: Higher efficiency codec for 4K+ content
- **VP9**: Google's open video codec used in WebM
- **MPEG-2**: Legacy codec for DVD and broadcast compatibility
- **ProRes**: Professional codec for editing workflows

## Implementation Process

### 1. Setting Up Your Development Environment

Before implementing FFMPEG output, ensure your development environment is properly configured:

1. Create a new or open an existing .NET project
2. Install the appropriate VisioForge SDK NuGet packages
3. Add FFMPEG dependency packages (detailed in the Dependencies section)
4. Import the necessary namespaces in your code:

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.Types.Output;     // FFMPEGEXEOutput
using VisioForge.Core.Types.FFMPEGEXE;  // H264MFSettings, H264NVENCSettings, H264QSVSettings, HEVCQSVSettings, OutputMuxer, BasicAudioSettings, ...
```

### 2. Initializing FFMPEG Output

Start by creating an instance of `FFMPEGEXEOutput` to handle your output configuration:

```csharp
var ffmpegOutput = new FFMPEGEXEOutput();
```

This object will serve as the container for all your encoding settings and preferences.

### 3. Configuring Output Container Format

Set your desired output container format using the `OutputMuxer` property:

```csharp
ffmpegOutput.OutputMuxer = OutputMuxer.MP4;
```

Other common container options include:

- `OutputMuxer.Matroska` - For Matroska container (.mkv)
- `OutputMuxer.WebM` - For WebM format
- `OutputMuxer.AVI` - For AVI format
- `OutputMuxer.MOV` - For QuickTime container

### 4. Video Encoder Configuration

FFMPEG provides multiple video encoder options. Select and configure the appropriate encoder based on your requirements and available hardware:

#### Standard CPU-Based H.264 Encoding

```csharp
var videoEncoder = new H264MFSettings
{
    Bitrate     = 5000000, // 5 Mbps
    RateControl = H264MFRateControl.CBR,
};
ffmpegOutput.Video = videoEncoder;
```

#### Hardware-Accelerated NVIDIA Encoding

```csharp
var nvidiaEncoder = new H264NVENCSettings
{
    Bitrate = 8000000,        // 8 Mbps
};
ffmpegOutput.Video = nvidiaEncoder;
```

#### Hardware-Accelerated Intel QuickSync Encoding

```csharp
var intelEncoder = new H264QSVSettings
{
    Bitrate = 6000000
};
ffmpegOutput.Video = intelEncoder;
```

#### HEVC/H.265 Encoding for Higher Efficiency

```csharp
var hevcEncoder = new HEVCQSVSettings
{
    Bitrate = 3000000,  
};
ffmpegOutput.Video = hevcEncoder;
```

### 5. Audio Encoder Configuration

Configure your audio encoding settings based on quality requirements and target platform compatibility:

```csharp
var audioEncoder = new BasicAudioSettings
{
    Bitrate = 192000,    // 192 kbps
    Channels = 2,        // Stereo
    SampleRate = 48000,  // 48 kHz - professional standard
    Encoder = AudioEncoder.AAC,
    Mode = AudioMode.CBR
};

ffmpegOutput.Audio = audioEncoder;
```

### 6. Final Configuration and Execution

Apply all settings and start the encoding process:

```csharp
// Apply format settings
core.Output_Format = ffmpegOutput;

// Set operation mode
core.Mode = VideoCaptureMode.VideoCapture;  // For Video Capture SDK
// core.Mode = VideoEditMode.Convert;       // For Video Edit SDK

// Set output path
core.Output_Filename = "output.mp4";

// Begin processing
await core.StartAsync();
```

## Required Dependencies

Install the following NuGet packages based on your target architecture to ensure proper functionality:

### Video Capture SDK Dependencies

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

For x86 targets:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoCapture.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

### Video Edit SDK Dependencies

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x64
```

For x86 targets:

```cmd
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x86
Install-Package VisioForge.DotNet.Core.Redist.FFMPEG.x86
```

## Troubleshooting and Optimization

### Common Issues and Solutions

- **Codec not found errors**: an encoder the FFmpeg executable is built without fails `Start` / `StartAsync`, and `OnError` names the encoder and the executable. The bundled `vfffmpeg64.exe` has no libx264 or libx265: use `H264MFSettings`, `OpenH264Settings` or a hardware encoder, or set `ExecutableFilename` to an FFmpeg build that has them. `ffmpeg -encoders` lists what a build has
- **Hardware acceleration failures**: a QSV, NVENC or AMF encoder on a PC without that GPU starts, and then FFmpeg exits; `OnError` reports it with FFmpeg's last error line. Verify GPU compatibility and driver versions
- **Performance issues**: Adjust thread count and encoding preset based on available CPU resources
- **Output quality problems**: Fine-tune bitrate, profile, and encoding parameters

### Performance Optimization Tips

- Use hardware acceleration when available
- Choose appropriate presets based on your quality/speed requirements
- Set reasonable bitrates based on content type and resolution
- Consider two-pass encoding for non-realtime scenarios requiring highest quality

## Tutorial code: webcam to MP4 and WebM

The complete `Form1.cs` from the video. The form has a `VideoView`, combo boxes for the camera, format, frame rate, microphone and output, and Start and Stop buttons; Stop starts disabled. The designer file is in the source code on GitHub. The project targets .NET 10 with implicit usings. On .NET Framework 4.7.2, the same code builds in an SDK-style project with `<LangVersion>10</LangVersion>` and `<ImplicitUsings>enable</ImplicitUsings>`; in `Program.cs`, replace `ApplicationConfiguration.Initialize()`, which .NET Framework does not generate, with `Application.EnableVisualStyles()` and `Application.SetCompatibleTextRenderingDefault(false)`, and set `<dpiAware>true</dpiAware>` in an `app.manifest` to keep the system DPI awareness that .NET 10 sets.

```csharp
using VisioForge.Core.Types;
using VisioForge.Core.Types.FFMPEGEXE;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.UI;
using VisioForge.Core.VideoCapture;

namespace WebcamFfmpeg;

public partial class Form1 : Form
{
    private const string Mp4H264 = "MP4 - H.264 + AAC";
    private const string Mp4Hevc = "MP4 - HEVC + AAC";
    private const string WebmVp9 = "WebM - VP9 + Opus";

    private VideoCaptureCore videoCapture1;

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

        foreach (var device in videoCapture1.Audio_CaptureDevices())
            cbMicrophone.Items.Add(device.Name);

        if (cbCamera.Items.Count > 0) cbCamera.SelectedIndex = 0;
        if (cbMicrophone.Items.Count > 0) cbMicrophone.SelectedIndex = 0;

        cbOutput.Items.Add(Mp4H264);
        cbOutput.Items.Add(WebmVp9);

        // HEVC needs a hardware encoder: an Intel, NVIDIA or AMD GPU
        var hw = VideoCaptureCore.HWEncodersAvailable();
        if (hw.QSV_H265 || hw.NVENC_H265 || hw.AMD_H265)
            cbOutput.Items.Add(Mp4Hevc);

        cbOutput.SelectedIndex = 0;
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
        if (cbFrameRate.SelectedItem is null)
        {
            Text = "No camera, format or frame rate";
            return;
        }

        Text = "Webcam to MP4 and WebM with FFmpeg";

        videoCapture1.Video_CaptureDevice = new VideoCaptureSource(cbCamera.Text);
        var camera = videoCapture1.Video_CaptureDevice;
        camera.Format_UseBest = false;
        camera.Format = cbFormat.Text;
        camera.FrameRate = (VideoFrameRate)cbFrameRate.SelectedItem;

        var mic = (string)cbMicrophone.SelectedItem;
        videoCapture1.Audio_RecordAudio = mic is not null; // no microphone: record video only
        if (mic is not null) videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(mic);
        videoCapture1.Audio_PlayAudio = false; // record the microphone without playing it back

        var ffmpeg = new FFMPEGEXEOutput();
        string extension;
        switch (cbOutput.Text)
        {
            case Mp4Hevc:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new HEVCMFSettings
                {
                    ForceHWEncoding = true, // the GPU's encoder
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 6_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;

            case WebmVp9:
                ffmpeg.OutputMuxer = OutputMuxer.WebM;
                ffmpeg.Video = new BasicVideoSettings
                {
                    Encoder = VideoEncoder.VP9,
                    BasicMode = VideoMode.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = new BasicAudioSettings
                {
                    Encoder = AudioEncoder.OPUS,
                    Mode = AudioMode.CBR,
                    Bitrate = 128_000,
                    SampleRate = 0, // 0: keep the microphone's own
                    Channels = 0,
                };
                extension = ".webm";
                break;

            default:
                ffmpeg.OutputMuxer = OutputMuxer.MP4;
                ffmpeg.Video = new H264MFSettings
                {
                    RateControl = H264MFRateControl.CBR,
                    Bitrate = 8_000_000,
                };
                ffmpeg.Audio = Aac();
                extension = ".mp4";
                break;
        }

        videoCapture1.Output_Filename = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "output" + extension);
        videoCapture1.Output_Format = ffmpeg;
        videoCapture1.Mode = VideoCaptureMode.VideoCapture;

        btStart.Enabled = false;
        var started = await videoCapture1.StartAsync();
        btStop.Enabled = started;
        btStart.Enabled = !started;
    }

    private static BasicAudioSettings Aac() => new()
    {
        Encoder = AudioEncoder.AAC,
        Mode = AudioMode.CBR,
        Bitrate = 192_000,
        SampleRate = 0, // 0: keep the microphone's own
        Channels = 0,
    };

    private async void btStop_Click(object sender, EventArgs e)
    {
        btStop.Enabled = false;
        await videoCapture1.StopAsync();
        btStart.Enabled = true;
    }

    private async void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
        // Finish the file before the window goes away
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

## Additional Resources

- **[FFmpeg output tutorial sample (Video Capture SDK, WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_VideoTutorials/webcam-ffmpeg-winforms)** — the app from the video: a webcam and its microphone to MP4 with H.264 and AAC or to WebM with VP9 and Opus, and to MP4 with HEVC when the PC has an Intel, NVIDIA or AMD hardware HEVC encoder.

For more code samples and implementation examples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).

To learn more about FFMPEG parameters and capabilities, refer to the [official FFMPEG documentation](https://ffmpeg.org/documentation.html).

## Video transcript

### What we build

Let's record a webcam to MP4 and WebM files through FFmpeg, in a C# WinForms app with the VisioForge Video Capture SDK. Here's what we build. A WinForms window with a live camera preview. It records MP4 files with H.264 video and AAC audio, and WebM files with VP9 video and Opus audio. On a PC with a GPU encoder for HEVC, it records HEVC in MP4 too. All three come from one output class: FFMPEGEXEOutput. This is the finished app, recording from a Logitech C920.

### Why FFmpeg

Why FFmpeg? The SDK captures the camera and the microphone, and hands their frames to FFmpeg. FFmpeg encodes them and writes the file. So one output class covers many containers and codecs. FFmpeg comes in a NuGet package, so there's nothing else to install. FFMPEGEXEOutput needs the Premium edition of the SDK, and the trial runs it for thirty days.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project can target .NET 8 or later, or .NET Framework 4.7.2. And a webcam with a microphone. VideoCaptureCore is the SDK's DirectShow engine, so this app runs on Windows only.

### Create a WinForms project

In Visual Studio, create a new project. Search for Windows Forms, and pick Windows Forms App for C#. Name the project WebcamFfmpeg. Choose .NET 10 and click Create. Visual Studio opens the empty form in the designer.

### Add the NuGet packages

Now add three NuGet packages: the SDK itself, the video capture redist with its DirectShow filters, and the FFmpeg redist. That last one puts FFmpeg next to your app. In Visual Studio, that's Manage NuGet Packages. Browse for VisioForge.DotNet.Core and install it. Then install the video capture redist for x64. And the FFmpeg redist for x64. Both redists are the x64 ones, so set PlatformTarget to x64 in the project file. An x86 app takes the x86 redists instead.

### Design the WinForms form

From the Toolbox, draw a VideoView for the live preview. Add a label and a combo box for the camera, and give every control a clear name in the Properties window. Do the same for the camera format, the frame rate, the microphone and the output. Add the Start and Stop buttons. Stop stays disabled until a recording starts. Double-click the form, the camera and format lists, and both buttons. Visual Studio adds an empty handler for each one.

### Create the capture engine

At the top of Form1.cs, add these usings. The FFmpeg settings live in VisioForge.Core.Types.FFMPEGEXE. In the form's Load handler, create the VideoCaptureCore engine and pass it the VideoView, where the live preview is drawn. Engine errors arrive in the OnError event. Here they go to the title bar.

### List the devices

Video_CaptureDevices returns every camera, and Audio_CaptureDevices every microphone. We list their names and select the first of each. When the camera changes, its handler fills the format list with the formats of that camera, and picks 1920x1080 MJPG when it's there. When the format changes, its handler fills the frame rate list the same way, and picks thirty frames per second.

### List the outputs

Back in the Load handler, the output list gets its entries. H.264 and WebM work on any PC. HEVC needs a hardware encoder, so HWEncodersAvailable tells us whether an Intel, NVIDIA or AMD GPU has one, and only then is HEVC in the list. Here's what the FFmpeg redist encodes with. H.264 uses the Media Foundation encoder that comes with Windows. VP9 and Opus are open codecs for WebM, encoded on the CPU. AAC is the audio for MP4. HEVC goes through the GPU's Media Foundation encoder, which is why the code sets ForceHWEncoding. On a PC without one, HEVC stays out of the list.

### Choose the camera format

The Start handler first checks that a frame rate is selected. If not, it says so in the title bar. Then it selects the camera by name, turns Format_UseBest off, and passes the format and the frame rate picked from the lists. Format_UseBest is on by default. It picks the highest resolution up to 1080p, then prefers MJPG and the fastest frame rate. To record 4K, or any format you choose, turn it off and set Format and FrameRate, as we do here.

### Select the microphone

With no microphone in the list, the app records video only. Otherwise it records the selected one. Audio_PlayAudio is off, so the microphone isn't played through the speakers while you record.

### FFMPEGEXEOutput

Now the output. One FFMPEGEXEOutput takes a muxer, which is the container, video settings and audio settings. The default case is MP4. H264MFSettings selects the H.264 encoder, at a constant eight megabits per second, and the audio is AAC. The HEVC case is MP4 as well. HEVCMFSettings with ForceHWEncoding uses the GPU's encoder. HEVC needs fewer bits for the same picture, so six megabits per second here. For WebM, the muxer is WebM. BasicVideoSettings picks the VP9 encoder at a constant eight megabits per second, and BasicAudioSettings picks Opus at 128 kilobits per second. Both MP4 cases share this AAC setting: 192 kilobits per second at a constant bitrate. A few details. Bitrate is in bits per second here, so eight million means eight megabits. SampleRate and Channels set to zero keep the microphone's own format. Eight megabits of video is about sixty megabytes a minute. And the file extension should match the muxer: MP4 or WebM.

### Start and stop

After the switch, the file goes into the Videos folder, with the extension the case picked. The engine switches to video capture mode, so it records and not just previews. StartAsync returns once the capture is running. StopAsync stops the capture, and FFmpeg finishes writing the file.

### Close the window

Closing the window while it records would leave the file unfinished. In the Properties window, on the Events tab, double-click FormClosing to add this handler. While recording, it cancels the close. Then it stops the recording, and closes the window again once the handler has returned. With nothing recording, the handler disposes the engine.

### Run it

Run the app from Visual Studio. The camera, format and microphone lists fill in from the devices. With MP4 and H.264 selected, Start begins the recording. The C920 takes a few seconds to send its first frame. Stop finishes output.mp4. Then pick WebM and record again. Stop finishes output.webm, and then we close the window.

### Check the files with ffprobe

Both files land in the Videos folder. ffprobe shows what's inside. The MP4 has H.264 video at 1920x1080 and AAC audio at 48 kilohertz stereo. The WebM has VP9 video at 1920x1080 and Opus audio at 48 kilohertz stereo.

### Common mistakes

A few common mistakes. The FFmpeg redist package is missing, so the SDK reports that vfffmpeg64.exe was not found. HEVC on a PC without a GPU encoder for it: check HWEncodersAvailable first. A bitrate written in kilobits: here it's bits per second. A Standard or Professional licence: FFMPEGEXEOutput needs the Premium edition. And closing the window mid-recording: await StopAsync first.

### Next steps

Next, the webcam to MP4 video records H.264 with the SDK's own encoders, without FFmpeg, and the webcam to WMV video uses the Windows Media encoders. There are also guides for IP cameras and screen recording. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.
