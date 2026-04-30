---
title: Screen Recording in C# .NET — MP4, GPU, Multi-Monitor
description: Record full screen, region, or multi-monitor to MP4 using VisioForge Video Capture SDK. GPU encoding (NVENC/QSV/AMF), audio loopback, and C# examples.
sidebar_label: Screen Capture to MP4
tags:
  - Video Capture SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - GStreamer
  - Capture
  - Encoding
  - Webcam
  - Screen Capture
  - MP4
  - AVI
  - H.264
  - H.265
  - C#
  - NuGet
primary_api_classes:
  - MP4Output
  - ScreenCaptureSourceSettings
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCore
  - AudioCaptureSource

---

# Screen capture to MP4 file

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/fPJEoOz6lIM?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

## Required redists  

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

## Legacy API — Video Capture SDK

### Code Example

```csharp
using System;
using System.IO;
using System.Windows.Forms;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_mp4
{
    public partial class Form1 : Form
    {
        // Main VideoCapture component that handles all recording operations
        private VideoCaptureCore videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Starts screen recording with audio from the default device
        /// </summary>
        private async void btStartWithAudio_Click(object sender, EventArgs e)
        {
            // Configure screen capture to record the entire screen
            // ScreenCaptureSourceSettings allows fine control of capture region and parameters
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true  // Capture the entire screen rather than a specific region
            };

            // Configure audio capture by selecting the first available audio input device
            // Audio_CaptureDevices() returns all connected microphones and audio inputs
            // We select the first device (index 0) in the collection
            videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(
                videoCapture1.Audio_CaptureDevices()[0].Name);

            // Disable audio monitoring/playback during recording to prevent feedback
            // This means we won't hear the captured audio while recording
            videoCapture1.Audio_PlayAudio = false;

            // Enable audio recording to include sound in the output file
            videoCapture1.Audio_RecordAudio = true;

            // Set the output file location to the user's Videos folder
            // Environment.GetFolderPath ensures the path works across different Windows systems
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Use MP4 container format with H.264 video and AAC audio codecs (standard format)
            // MP4Output can be further configured with custom encoding parameters if needed
            videoCapture1.Output_Format = new MP4Output();
            
            // Set the capture mode to screen recording
            // Other modes include camera capture, video file processing, etc.
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Begin the capture process asynchronously
            // Using async/await pattern to prevent UI freezing during operation
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Starts screen recording without audio (video only)
        /// </summary>
        private async void btStartWithoutAudio_Click(object sender, EventArgs e)
        {
            // Configure screen capture for full screen recording
            // Same ScreenCaptureSourceSettings as in audio recording
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings() { 
                FullScreen = true 
            };

            // Disable both audio playback and recording in a single line
            // This creates a video-only MP4 file with no audio track
            videoCapture1.Audio_PlayAudio = videoCapture1.Audio_RecordAudio = false;

            // Set output file path to user's Videos directory with MP4 extension
            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4");
            
            // Configure output as MP4 (H.264 video codec)
            videoCapture1.Output_Format = new MP4Output();
            
            // Set mode to screen capture
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            // Start screen recording asynchronously
            await videoCapture1.StartAsync();
        }

        /// <summary>
        /// Stops the current recording process safely
        /// </summary>
        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop the recording asynchronously
            // This properly finalizes the MP4 file and releases resources
            // Using async ensures UI remains responsive during file finalization
            await videoCapture1.StopAsync();
        }

        /// <summary>
        /// Initializes the VideoCapture component when the form loads
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            // Initialize the video capture component and connect it to a video preview control
            // VideoView1 should be a control on your form that implements IVideoView interface
            // This allows for live preview of the capture when desired
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }
    }
}
```

## Modern API — Video Capture SDK X

The modern cross-platform API uses `VideoCaptureCoreX` with Direct3D 11 screen capture and Windows Graphics Capture (WGC). This console application records the full screen to MP4 with optional system audio.

### Required NuGet Packages

```bash
dotnet add package VisioForge.DotNet.Core.TRIAL
dotnet add package VisioForge.DotNet.VideoCapture.TRIAL
```

Add the [redist package](../../deployment-x/index.md) for your platform (e.g., `VisioForge.DotNet.Redist.Base.Windows.x64`).

### Complete Example

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Configure Direct3D 11 screen capture with WGC
            var screenSource = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true,
                MonitorIndex = 0  // Primary monitor (-1 also selects primary)
            };

            videoCapture.Video_Source = screenSource;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = false;

            // Configure MP4 output (H.264 + AAC, auto-selected encoders)
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");

            var mp4Output = new MP4Output(outputPath);
            videoCapture.Outputs_Add(mp4Output, autostart: true);

            // Start recording
            await videoCapture.StartAsync();
            Console.WriteLine($"Recording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            // Stop and save
            await videoCapture.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Adding System Audio (Loopback)

To include desktop audio in the recording, add WASAPI2 loopback capture:

```csharp
// Enumerate WASAPI2 output devices for loopback capture
var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
    AudioOutputDeviceAPI.WASAPI2);

if (audioOutputs.Length > 0)
{
    var loopbackSource = new LoopbackAudioCaptureDeviceSourceSettings(audioOutputs[0]);
    videoCapture.Audio_Source = loopbackSource;
    videoCapture.Audio_Record = true;
}
```

For microphone audio instead, use `DeviceEnumerator.Shared.AudioSourcesAsync()` — see the [Audio Capture guide](../audio-capture/index.md) for complete examples.

## GPU-Accelerated Encoding

Hardware-accelerated encoding offloads H.264/HEVC compression to your GPU, significantly reducing CPU usage during high-resolution or high-FPS recording.

```csharp
// NVIDIA NVENC H.264
var mp4Output = new MP4Output(
    outputPath,
    new NVENCH264EncoderSettings());

// Intel Quick Sync Video H.264
var mp4Output = new MP4Output(
    outputPath,
    new QSVH264EncoderSettings());

// AMD AMF H.264
var mp4Output = new MP4Output(
    outputPath,
    new AMFH264EncoderSettings());
```

HEVC (H.265) encoders are also available for better compression at the same quality: `NVENCHEVCEncoderSettings`, `QSVHEVCEncoderSettings`, `AMFHEVCEncoderSettings`.

!!! note "Engine note: `MP4Output` and the X-engine"

    The `MP4Output` class shown in this section refers to the **X-engine**
    `VisioForge.Core.Types.X.Output.MP4Output`, which accepts cross-platform
    `*EncoderSettings` types (`NVENCH264EncoderSettings`,
    `OpenH264EncoderSettings`, etc.). When hardware encoding is unavailable
    on the X engine and you use the parameterless ctor
    `new MP4Output(filename)`, the SDK selects an available software encoder
    automatically. The classic Windows-only DirectShow engine has its own
    `VisioForge.Core.Types.Output.MP4Output` with a different settings shape
    — don't mix them.

## Screen Capture with Media Blocks SDK

The Media Blocks SDK uses a pipeline approach where you connect source, processing, and output blocks. This gives full control over the data flow and allows splitting the video stream to multiple outputs simultaneously.

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Special;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        await VisioForgeX.InitSDKAsync();

        var pipeline = new MediaBlocksPipeline();

        try
        {
            // Screen capture source
            var screenSettings = new ScreenCaptureD3D11SourceSettings
            {
                FrameRate = new VideoFrameRate(25, 1),
                CaptureCursor = true
            };
            var screenSource = new ScreenSourceBlock(screenSettings);

            // MP4 output
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                $"screen_mb_{DateTime.Now:yyyyMMdd_HHmmss}.mp4");
            var mp4Sink = new MP4OutputBlock(outputPath);

            // Connect source to output
            pipeline.Connect(screenSource.Output, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));

            // Start pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Recording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            await pipeline.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            await pipeline.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Cross-Platform Screen Capture

The SDK supports screen capture on Windows, macOS, and Linux with platform-specific source settings:

| Platform | Capture Method | Settings Class | Requirements |
|----------|---------------|----------------|--------------|
| Windows | Direct3D 11 / WGC | `ScreenCaptureD3D11SourceSettings` | Windows 8+ (WGC: Windows 10 v1803+) |
| macOS | AVFoundation | `ScreenCaptureMacOSSourceSettings` | macOS 10.15+ (screen recording permission) |
| Linux | X11 / XDisplay | `ScreenCaptureXDisplaySourceSettings` | X11 server |

On Windows, the SDK auto-selects WGC when available, falling back to DXGI Desktop Duplication on older systems. You can force a specific API:

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.DXGI  // Force Desktop Duplication instead of WGC
};
```

## How It Works — Legacy API

This Windows Forms application demonstrates screen capture functionality with and without audio using VisioForge Video Capture SDK:

1. **Setup**: The `VideoCaptureCore` object is initialized in the form load event, connecting it to a video view component.

2. **Capturing with Audio**:
   - Sets screen capture to full screen mode
   - Selects the first available audio device for recording
   - Disables audio playback but enables audio recording
   - Sets the output file to MP4 format in the user's Videos folder
   - Uses asynchronous method to start capture

3. **Capturing without Audio**:
   - Similar to above but disables both audio playback and recording
   - Uses the same MP4 output format and capture mode

4. **Stopping Capture**:
   - Provides a simple stop method that asynchronously stops the recording

The application demonstrates how to configure different capture scenarios with minimal code using the SDK's fluent interface and async patterns.

### Audio Configuration

The code example above captures microphone audio by selecting the first available device. You can select a specific audio device by name, including system audio (loopback) devices for capturing desktop sound:

```csharp
// Select a loopback device (e.g., "Stereo Mix") for system audio capture
var devices = videoCapture1.Audio_CaptureDevices();
var loopbackDevice = devices.FirstOrDefault(d => d.Name.Contains("Stereo Mix"));

if (loopbackDevice != null)
{
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(loopbackDevice.Name);
}
else
{
    // Fallback: use the first available audio device
    videoCapture1.Audio_CaptureDevice = new AudioCaptureSource(devices[0].Name);
}

// Enable recording, disable playback to prevent feedback
videoCapture1.Audio_RecordAudio = true;
videoCapture1.Audio_PlayAudio = false;
```

To create a silent recording with no audio track, disable both audio properties:

```csharp
videoCapture1.Audio_PlayAudio = false;
videoCapture1.Audio_RecordAudio = false;
```

### Region Capture

Instead of recording the full screen, capture a specific rectangular area by setting `FullScreen = false` and providing pixel coordinates:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = false,
    Left = 100,
    Top = 100,
    Right = 1380,
    Bottom = 820
};
```

The coordinates define the capture rectangle in screen pixels. This is useful for recording a specific application window or a portion of the desktop.

### Multi-Monitor Recording

Select which display to record using the `DisplayIndex` property. The primary monitor is index `0`, the secondary is `1`, and so on:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Record the secondary monitor
};
```

### Recording Quality Settings

Customize the MP4 output by configuring the H.264 encoder. The default bitrate is 3500 kbps, which produces approximately 25 MB per minute at 1080p:

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Higher quality (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Better compression than Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Variable bitrate
videoCapture1.Output_Format = mp4Output;
```

For GPU-accelerated encoding, see the [GPU-Accelerated Encoding](#gpu-accelerated-encoding) section above.

### Mouse Cursor Options

Include the mouse cursor in the recording and optionally add a highlight effect for tutorial-style screencasts:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    GrabMouseCursor = true,
    MouseHighlight = true,
    MouseHighlightColor = System.Drawing.Color.Yellow,
    MouseHighlightRadius = 40,
    MouseHighlightOpacity = 0.4
};
```

The highlight draws a translucent circle around the cursor, making it easier for viewers to follow mouse movements.

## Frequently Asked Questions

### What license do I need for a C# screen capture application?

Video Capture SDK .Net requires a license for development and distribution. A Developer license removes the evaluation watermark and unlocks all features during development. A Release license is required when distributing your application to end users. The SDK is available in Premium edition which includes all capture modes, MP4/AVI/WMV output, and both DirectShow and GStreamer engines. You can evaluate the SDK without a license — screen capture works fully but includes a watermark overlay. Visit the [product page](https://www.visioforge.com/video-capture-sdk-net) for pricing and license options.

### How do I control recording quality and file size for MP4 screen capture?

Configure the `MP4Output` video bitrate to balance quality and file size. The default is 3500 kbps, which works well for most screen recordings. Lower the bitrate to 1500–2000 kbps for smaller files, or increase to 5000–8000 kbps for high-quality captures. Use `H264Profile.ProfileMain` or `ProfileHigh` instead of `ProfileBaseline` for better compression at the same quality. Frame rate also affects file size — 15 FPS is sufficient for presentations, while 30 FPS is better for software demos. A 1080p recording at 3500 kbps produces roughly 25 MB per minute.

### Can I capture system audio (desktop sound) instead of microphone input?

Yes. Call `Audio_CaptureDevices()` to enumerate all available audio devices, then select a loopback or system audio device (such as "Stereo Mix" or "What U Hear") by name. Set `Audio_RecordAudio = true` and `Audio_PlayAudio = false`. The available loopback device names depend on your audio hardware and drivers. You can record both microphone and system audio simultaneously by configuring additional audio sources.

### How do I record a specific monitor in a multi-monitor setup?

Set the `DisplayIndex` property on `ScreenCaptureSourceSettings` — use `0` for the primary monitor, `1` for the secondary, and so on. Combine with `FullScreen = true` to capture the entire selected display. You can also set `FullScreen = false` with `Left/Top/Right/Bottom` coordinates to capture a specific region on the chosen monitor.

### What frame rate should I use for screen recording?

The default frame rate is 10 FPS. Use 15 FPS for presentations, slides, and mostly static content. Use 25–30 FPS for software tutorials and UI demonstrations where smooth mouse movement matters. Use 60 FPS for game recording or high-motion content. Higher frame rates increase file size and CPU usage proportionally. Set the frame rate via `ScreenCaptureSourceSettings.FrameRate`.

## See Also

- [Screen Capture to AVI](screen-capture-avi.md) — record screen to AVI format with uncompressed or MJPEG video
- [Screen Capture to WMV](screen-capture-wmv.md) — record screen to Windows Media format
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording application in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture region, multi-monitor, cursor, and frame rate settings
- [Save Webcam Video](../guides/save-webcam-video.md) — webcam capture to MP4 with audio configuration
- [Code Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — additional screen capture code snippets on GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
