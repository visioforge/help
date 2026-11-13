---
title: Screen Capture to MP4 in C# — Tutorial, GPU, Multi-Monitor
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
  - WPF
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
  - VideoCaptureCoreX
  - MP4Output
  - ScreenCaptureSourceSettings
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCore
  - AudioCaptureSource
  - LoopbackAudioCaptureDeviceSourceSettings
  - VideoView
video:
  id: 17eaRlDqM_I
  upload_date: 2026-11-11T19:35:01Z
  duration: PT8M51S

---

# Screen capture to MP4 file

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

This tutorial records the screen and the system sound to an MP4 file from a WPF application. The `VideoCaptureCoreX` engine captures the primary monitor through Direct3D 11, draws a live preview in a `VideoView`, and encodes H.264 on the GPU when the PC has a hardware encoder, with AAC audio. The sound that plays on the speakers is recorded through WASAPI loopback.

The classic `VideoCaptureCore` engine records the screen as well; its version of the app is [further down this page](#second-option-videocapturecore).

## YouTube tutorial

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/17eaRlDqM_I?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/screen-capture-mp4-wpf)

## Required NuGet packages

- [VisioForge.DotNet.VideoCapture](https://www.nuget.org/packages/VisioForge.DotNet.VideoCapture/) — the SDK; the WPF `VideoView` ships in it
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64/) — the GStreamer runtime the X engine runs on
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64/) — the AAC audio encoder

These are the x64 packages, so the project sets `<PlatformTarget>x64</PlatformTarget>`; a 32-bit app takes the `.x86` packages instead. See [Windows deployment](../../deployment-x/Windows.md) for the full list.

## The window in XAML

The `vf` prefix maps the `VisioForge.Core.UI.WPF` namespace of the `VisioForge.Core` assembly. The left column holds the speakers list, the sound check box and the buttons, the right column the `VideoView` where the live preview is drawn. Start stays disabled until the SDK has loaded.

```xml
<Window x:Class="ScreenCaptureMp4Wpf.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:vf="clr-namespace:VisioForge.Core.UI.WPF;assembly=VisioForge.Core"
        Title="Screen to MP4" Height="560" Width="1000"
        Loaded="Window_Loaded" Closing="Window_Closing">
    <Grid Margin="10">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="260" />
            <ColumnDefinition Width="*" />
        </Grid.ColumnDefinitions>

        <StackPanel Grid.Column="0" Margin="0,0,10,0">
            <Label Content="Speakers" />
            <ComboBox x:Name="cbSpeakers" />
            <CheckBox x:Name="cbRecordSound" Content="Record system sound" IsChecked="True"
                      Margin="0,10,0,0" />

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

## The code behind the window

```csharp
using System.ComponentModel;
using System.IO;
using System.Windows;
using VisioForge.Core;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

namespace ScreenCaptureMp4Wpf;

public partial class MainWindow : Window
{
    private VideoCaptureCoreX videoCapture1;

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

        // The sound of these outputs is recorded through WASAPI loopback
        speakers = await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2);
        foreach (var device in speakers)
            cbSpeakers.Items.Add(device.Name);

        if (cbSpeakers.Items.Count > 0) cbSpeakers.SelectedIndex = 0;

        btStart.IsEnabled = true;
    }

    private async void btStart_Click(object sender, RoutedEventArgs e)
    {
        // The whole primary monitor, with the mouse cursor
        videoCapture1.Video_Source = new ScreenCaptureD3D11SourceSettings
        {
            FrameRate = new VideoFrameRate(30)
        };

        var recordSound = cbRecordSound.IsChecked == true && cbSpeakers.SelectedIndex >= 0;
        videoCapture1.Audio_Source = recordSound
            ? new LoopbackAudioCaptureDeviceSourceSettings(speakers[cbSpeakers.SelectedIndex])
            : null;
        videoCapture1.Audio_Record = recordSound;
        videoCapture1.Audio_Play = false; // never play the captured sound back into the speakers

        // H.264 on the GPU encoder when there is one, OpenH264 otherwise; AAC audio
        videoCapture1.Outputs_Clear();
        videoCapture1.Outputs_Add(new MP4Output(Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), $"screen_{DateTime.Now:yyyyMMdd_HHmmss}.mp4")));

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

        e.Cancel = true; // still loading or recording: finish the MP4 first
        if (!btStop.IsEnabled) return; // already loading, starting or stopping
        btStop.IsEnabled = false;
        await videoCapture1.StopAsync();
        btStart.IsEnabled = true;
        await Dispatcher.InvokeAsync(Close); // not Close(): WPF throws inside Closing
    }
}
```

## How it works

1. **SDK initialization** — `VisioForgeX.InitSDKAsync` runs once, when the window loads. The first run on a PC takes longer, so Start is enabled only after it returns. `VisioForgeX.DestroySDK` releases it when the window closes.
2. **Screen source** — `ScreenCaptureD3D11SourceSettings` captures the whole primary monitor with the mouse cursor by default. It uses DXGI Desktop Duplication, which starts at once even on a still screen, and falls back to Windows Graphics Capture where Desktop Duplication is not supported. The default frame rate is 10 fps, so the code sets 30.
3. **System sound** — `AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` lists the speakers and headphones, and `LoopbackAudioCaptureDeviceSourceSettings` records what plays on the one picked. `Audio_Play` is `false`, so the recorded sound is not played back.
4. **MP4 output** — `MP4Output` with no encoder settings picks NVIDIA NVENC, then AMD AMF, then Intel Quick Sync, and OpenH264 on the CPU when none of them is present. Audio is AAC. An output stays in the engine after a recording, so `Outputs_Clear` runs before each new one.
5. **Start, stop and close** — `StartAsync` returns whether the capture started, and `StopAsync` finalizes the MP4 file. Closing the window during a recording cancels the close, awaits `StopAsync`, and closes again.

## Notes

- A WPF project leaves `System.IO` out of its implicit usings, so the file adds it for `Path`.
- The preview shows the screen it records, so the app window appears inside itself, in the preview and in the file. On a PC with two monitors, move the window to the one that is not recorded; with one monitor, minimize it after Start and restore it to stop.
- With **Record system sound** cleared, or with no speakers in the list, the MP4 holds video only.
- `MP4Output` picks a GPU encoder because its GStreamer element exists, which does not prove it can encode right now. `H264EncoderBlock.CanEncodeAsync` encodes a few test frames to find out; to stay on the CPU, pass `new OpenH264EncoderSettings()` to the `MP4Output` constructor.

## Second option: VideoCaptureCore

The classic Windows engine, `VideoCaptureCore`, records the screen to MP4 in a WinForms app, with or without the microphone.

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-mp4)

### Required redists

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)
- MP4 redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.MP4.x64/)

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

### How it works — VideoCaptureCore

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

#### Audio Configuration

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

#### Region Capture

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

#### Multi-Monitor Recording

Select which display to record using the `DisplayIndex` property. The primary monitor is index `0`, the secondary is `1`, and so on:

```csharp
videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings()
{
    FullScreen = true,
    DisplayIndex = 1  // Record the secondary monitor
};
```

#### Recording Quality Settings

Customize the MP4 output by configuring the H.264 encoder. The default bitrate is 3500 kbps, which produces approximately 25 MB per minute at 1080p:

```csharp
var mp4Output = new MP4Output();
mp4Output.Video.Bitrate = 5000;                          // Higher quality (kbps)
mp4Output.Video.Profile = H264Profile.ProfileMain;       // Better compression than Baseline
mp4Output.Video.RateControl = H264RateControl.VBR;       // Variable bitrate
videoCapture1.Output_Format = mp4Output;
```

#### Mouse Cursor Options

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

## Console app — VideoCaptureCoreX { #modern-api-video-capture-sdk-x }

The cross-platform `VideoCaptureCoreX` engine also runs without a window, with Direct3D 11 screen capture (Desktop Duplication). This console application records the full screen to MP4 with optional system audio.

It takes the same packages as the WPF app: see [Required NuGet packages](#required-nuget-packages).

The same packages are used for the 30-day trial and licensed applications. See the [VisioForge licensing page](https://www.visioforge.com/licensing) for trial terms and `.vflicense` activation.

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
            // Configure Direct3D 11 screen capture (Desktop Duplication by default)
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

On Windows, the SDK uses DXGI Desktop Duplication by default: it starts at once, even on a still screen, and falls back to WGC where Desktop Duplication is not supported. Window capture always uses WGC. You can select WGC for a screen too; it also starts at once on a still screen:

```csharp
var screenSource = new ScreenCaptureD3D11SourceSettings
{
    API = D3D11ScreenCaptureAPI.WGC  // Windows Graphics Capture instead of Desktop Duplication
};
```

For Android and iOS, and a .NET MAUI recorder that runs on Windows, Mac Catalyst, Android and iOS, see [Cross-platform screen recording in C# and .NET](https://www.visioforge.com/solutions/screen-recording-csharp).

## Frequently Asked Questions

### What license do I need for a C# screen capture application?

Video Capture SDK .NET requires a license for development and distribution. A Developer license removes the evaluation watermark and unlocks all features during development. A Release license is required when distributing your application to end users. The SDK is available in Premium edition which includes all capture modes, MP4/AVI/WMV output, and both DirectShow and GStreamer engines. You can evaluate the SDK without a license — screen capture works fully but includes a watermark overlay. Visit the [product page](https://www.visioforge.com/video-capture-sdk-net) for pricing and license options.

### How do I control recording quality and file size for MP4 screen capture?

Configure the `MP4Output` video bitrate to balance quality and file size. The default is 3500 kbps, which works well for most screen recordings. Lower the bitrate to 1500–2000 kbps for smaller files, or increase to 5000–8000 kbps for high-quality captures. Use `H264Profile.ProfileMain` or `ProfileHigh` instead of `ProfileBaseline` for better compression at the same quality. Frame rate also affects file size — 15 FPS is sufficient for presentations, while 30 FPS is better for software demos. A 1080p recording at 3500 kbps produces roughly 25 MB per minute.

### Can I capture system audio (desktop sound) instead of microphone input?

Yes. With `VideoCaptureCoreX`, pass a speaker from `DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)` to `LoopbackAudioCaptureDeviceSourceSettings`, as the WPF app above does: it records whatever plays on that device, with no special driver. With `VideoCaptureCore`, call `Audio_CaptureDevices()` to enumerate all available audio devices, then select a loopback or system audio device (such as "Stereo Mix" or "What U Hear") by name. Set `Audio_RecordAudio = true` and `Audio_PlayAudio = false`. The available loopback device names depend on your audio hardware and drivers. You can record both microphone and system audio simultaneously by configuring additional audio sources.

### How do I record a specific monitor in a multi-monitor setup?

With `VideoCaptureCoreX`, set `MonitorIndex` on `ScreenCaptureD3D11SourceSettings` (`0` is the first monitor, `-1` the primary) and set `Rectangle` to that monitor's size. With `VideoCaptureCore`, set the `DisplayIndex` property on `ScreenCaptureSourceSettings` — use `0` for the primary monitor, `1` for the secondary, and so on. Combine with `FullScreen = true` to capture the entire selected display. You can also set `FullScreen = false` with `Left/Top/Right/Bottom` coordinates to capture a specific region on the chosen monitor.

### What frame rate should I use for screen recording?

The default frame rate is 10 FPS. Use 15 FPS for presentations, slides, and mostly static content. Use 25–30 FPS for software tutorials and UI demonstrations where smooth mouse movement matters. Use 60 FPS for game recording or high-motion content. Higher frame rates increase file size and CPU usage proportionally. Set the frame rate via `ScreenCaptureD3D11SourceSettings.FrameRate` with `VideoCaptureCoreX`, or `ScreenCaptureSourceSettings.FrameRate` with `VideoCaptureCore`.

## Video Transcript

### What we build

Let's record the screen and the system sound to an MP4 file in a C# WPF app with the VisioForge Video Capture SDK. Here's what we build. The app records the whole primary monitor, with the mouse cursor, and the sound that plays on the speakers. It writes MP4, with H.264 encoded on the GPU when the PC has a hardware encoder, and AAC audio. A VideoView shows a live preview, and the window is laid out in XAML. The code behind it is about ninety lines of C#. This is the finished app, recording. The preview shows the screen, and the screen contains the preview, so it repeats itself like two facing mirrors.

### Prerequisites

You need Windows 10 or 11, and Visual Studio 2022 or 2026 with the .NET desktop development workload. The project targets .NET 8 or later. The written guide also shows screen recording on the classic VideoCaptureCore engine.

### Create a WPF project

In Visual Studio, create a new project. Search for WPF, and pick WPF Application for C#. Name the project ScreenCaptureMp4Wpf. Choose .NET 10 and click Create. Visual Studio opens MainWindow.xaml, with the designer above and the XAML below.

### Add the NuGet packages

Now add three NuGet packages. The Video Capture SDK itself. CrossPlatform Core, the GStreamer runtime the X engine runs on. And CrossPlatform Libav, which holds the AAC encoder. Or use the NuGet Package Manager: right-click the project, Manage NuGet Packages, and install VisioForge.DotNet.VideoCapture from the Browse tab. Then install the Core and Libav packages for Windows x64. The Installed tab lists all three. These are the x64 packages, so set PlatformTarget to x64 in the project file. A 32-bit app takes the x86 packages instead.

### Build the window in XAML

The window is written in XAML. First the window itself, with a two-column grid and a VideoView, and the designer draws it right away. Map the VisioForge.Core.UI.WPF namespace to a prefix, here vf. The VideoView goes into the right-hand column: that is where the live preview is drawn. The window also names its Loaded and Closing handlers. Next, the speakers list and a checkbox for the sound. The left-hand column is a StackPanel with a label, a combo box for the speakers, and a checkbox that turns the sound on or off. It starts checked. Last, the Start and Stop buttons. Each button names a Click handler. Both start disabled: Start until the SDK has loaded, and Stop until a recording runs. Every handler the XAML names goes into MainWindow.xaml.cs, and the next chapters write them.

### Initialize the SDK

MainWindow.xaml.cs starts with these usings. A WPF project leaves System.IO out of its implicit usings, so add it for Path. When the window loads, await VisioForgeX.InitSDKAsync once. Then create the VideoCaptureCoreX engine and pass it the VideoView, where the preview is drawn. Engine errors arrive in the OnError event; here they go to the title bar.

### List the speakers

AudioOutputsAsync with WASAPI2 lists the speakers and headphones. Their names fill the list, and the first one is selected. Start is enabled only now, once the SDK has loaded. WASAPI loopback records whatever plays on an output device. It needs no Stereo Mix and no virtual cable. Pick the device the sound actually plays on, because loopback records only that one.

### Capture the screen

In the Start handler, set the video source to ScreenCaptureD3D11SourceSettings. Out of the box it captures the whole primary monitor with the mouse cursor. The default frame rate is ten, so set thirty. The source uses Windows Graphics Capture on Windows 10 version 1803 and later, and DXGI Desktop Duplication on older systems. MonitorIndex picks another monitor, and CaptureCursor set to false leaves the cursor out.

### Record the system sound

When the checkbox is checked and a device is selected, the audio source is LoopbackAudioCaptureDeviceSourceSettings for that device, and Audio_Record is on. Otherwise there's no audio source, and the MP4 holds video only. Audio_Play is false, so the recorded sound is not played back into the speakers.

### Write the MP4

Add an MP4Output that writes to the Videos folder, with the date and time in the file name, so a second recording never overwrites the first. An output stays in the engine after a recording, so Outputs_Clear runs before each new one. With no encoder settings, MP4Output picks NVIDIA NVENC, then AMD AMF, then Intel Quick Sync, and OpenH264 on the CPU when the PC has none of them. The audio is AAC. H264EncoderBlock.CanEncodeAsync encodes a few test frames when you need to know that a GPU encoder really works.

### Start and stop

StartAsync returns whether the capture started, and the buttons follow it. StopAsync finalizes the MP4 file. If the window closes during a recording, cancel the close, await StopAsync, and close again. On that second pass the engine is disposed and the SDK released, so the file is always complete.

### Run it

Run the app from Visual Studio. Once the SDK has loaded, the speakers list fills and Start is enabled. Click Start. The preview shows the screen with the app inside it, again and again. The alarm sound that plays meanwhile goes into the file through loopback. Stop finalizes the MP4. Then close the window. The file lands in Videos: 4K at thirty frames per second, with the system sound.

### Common mistakes

A few common mistakes. Leaving out the Libav package, which holds the AAC encoder. Mixing x86 and x64 packages, or running an x86 app with the x64 ones. Forgetting that every window on the recorded monitor ends up in the video, the app's own window included: on a PC with two monitors, move it to the one that is not recorded. And loopback on a device the sound does not play on. Engine errors arrive in the OnError event.

### Next steps

There are also guides for recording a region or a single window, adding a microphone, webcam recording and network streaming. The full source is on GitHub, and the docs are at visioforge.com/help. Thanks for watching.

## See Also

- [Screen Capture to AVI](screen-capture-avi.md) — record screen to AVI format with uncompressed or MJPEG video
- [Screen Capture to WMV](screen-capture-wmv.md) — record screen to Windows Media format
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording application in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture region, multi-monitor, cursor, and frame rate settings
- [Save Webcam Video](../guides/save-webcam-video.md) — webcam capture to MP4 with audio configuration
- [Code Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — additional screen capture code snippets on GitHub
- [Cross-platform screen recording in C#](https://www.visioforge.com/solutions/screen-recording-csharp) — capture API per platform, a .NET MAUI recorder and the trial download
- [Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
