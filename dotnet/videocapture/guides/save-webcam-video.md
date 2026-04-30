---
title: Save Webcam Video in C# .NET: Record and Capture Guide
description: Save webcam video to MP4 and WebM in C# with VisioForge Video Capture SDK. GPU-accelerated encoding, device enumeration, and cross-platform support.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - WinForms
  - Capture
  - Encoding
  - Webcam
  - IP Camera
  - Screen Capture
  - MP4
  - WebM
  - H.264
  - H.265
  - VP8
  - VP9
  - AV1
  - C#
  - NuGet
primary_api_classes:
  - VideoCaptureCoreX
  - DeviceEnumerator
  - VideoCaptureDeviceSourceSettings
  - MP4Output
  - WebMOutput

---

# Save Webcam Video Using .Net: A Complete Guide to Record and Capture Webcam

Capturing and saving video from webcams is a common requirement in many modern applications, from video conferencing tools to surveillance systems. Whether you need to record webcam footage, display the webcam feed, or capture images, implementing reliable webcam functionality in .NET (DotNet) can be challenging. This tutorial provides a simple step-by-step guide to save webcam video using C# (C Sharp) with minimal code.

## Key Features of Video Capture SDK .Net

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) is a powerful library designed specifically for .NET developers who need to implement webcam capture functionality in their applications. Whether you want to record webcam video, save webcam frames as images, or display the webcam feed in your application, this SDK has you covered. Some of its standout features include:

- Seamless integration with .NET applications
- Support for multiple capture devices (USB webcams, IP cameras, capture cards)
- High-performance video and audio recording and processing
- Simple code to get and display webcam feeds
- Create and save video files in various formats
- GPU-accelerated encoding for optimal performance
- Cross-platform compatibility

## Output Formats: MP4 and WebM in Detail

### MP4 Format

MP4 is one of the most widely supported video container formats, making it an excellent choice for applications where compatibility is a priority. For detailed configuration options, see the [MP4 format documentation](../../general/output-formats/mp4.md).

Supported codecs for MP4:

- **H.264 (AVC):** The industry standard for video compression, offering excellent quality and wide compatibility. See the [H.264 encoder documentation](../../general/video-encoders/h264.md).
- **H.265 (HEVC):** Next-generation codec providing up to 50% better compression than H.264 while maintaining the same quality. See the [HEVC encoder documentation](../../general/video-encoders/hevc.md).
- **AAC:** Advanced Audio Coding, the industry standard for lossy digital audio compression.

### WebM Format

WebM is an open, royalty-free media file format designed for the web. For detailed configuration options, see the [WebM format documentation](../../general/output-formats/webm.md).

Supported codecs for WebM:

- **VP8:** Open-source video codec developed by Google, primarily used with WebM format.
- **VP9:** Successor to VP8, offering significantly improved compression efficiency.
- **AV1:** Newest open-source video codec with superior compression and quality, particularly at lower bitrates.
- **Vorbis:** Free and open-source audio compression format offering good quality at lower bitrates.

Each codec can be fine-tuned with various parameters to achieve the optimal balance between quality and file size for your specific application requirements.

## GPU Acceleration for Efficient Encoding

One of the standout features of Video Capture SDK .Net is its robust support for GPU-accelerated video encoding, which offers several significant advantages:

### Benefits of GPU Acceleration

- **Dramatically reduced CPU usage:** Free up CPU resources for other application tasks
- **Faster encoding speeds:** Up to 10x faster than CPU-only encoding
- **Real-time high-resolution encoding:** Enable 4K video capture with minimal system impact
- **Lower power consumption:** Particularly important for mobile and laptop applications
- **Higher quality at the same bitrate:** Some GPU encoders offer better quality-per-bit than CPU encoding

### Supported GPU Technologies

Video Capture SDK .Net leverages multiple GPU acceleration technologies:

- **NVIDIA NVENC:** Hardware-accelerated encoding on NVIDIA GPUs
- **AMD AMF/VCE:** Acceleration on AMD graphics cards
- **Intel Quick Sync Video:** Hardware encoding on Intel integrated graphics

The SDK automatically detects available hardware and selects the optimal encoding path based on your system's capabilities, with fallback to software encoding when necessary.

## Implementation Example (C# Code to Capture Video from Webcam)

Let's walk through a simple tutorial on how to record webcam video using C#. Implementing webcam capture with Video Capture SDK .Net is straightforward. Here's a complete example showing how to get your webcam feed, display it in your application, and save it to an MP4 file with H.264 encoding:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

using VisioForge.Core;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.AudioRenderers;

namespace video_capture_webcam_mp4
{
    public partial class Form1 : Form
    {
        // Core Video Capture object
        private VideoCaptureCoreX videoCapture1;

        public Form1()
        {
            InitializeComponent();
        }

        private async void btStart_Click(object sender, EventArgs e)
        {
            // Create VideoCaptureCoreX instance and set VideoView for video rendering
            videoCapture1 = new VideoCaptureCoreX(VideoView1);

            // Enumerate video and audio sources
            var videoSources = await DeviceEnumerator.Shared.VideoSourcesAsync();
            var audioSources = await DeviceEnumerator.Shared.AudioSourcesAsync();

            // Set default video source
            videoCapture1.Video_Source = new VideoCaptureDeviceSourceSettings(videoSources[0]);

            // Set default audio source
            videoCapture1.Audio_Source = audioSources[0].CreateSourceSettingsVC();

            // Set default audio sink
            var audioRenderers = await DeviceEnumerator.Shared.AudioOutputsAsync();
            videoCapture1.Audio_OutputDevice = new AudioRendererSettings(audioRenderers[0]);
            videoCapture1.Audio_Play = true;

            // Configure MP4 output. Default video and audio encoders will be used. 
            // GPU encoders will be used if available.
            var mp4Output = new MP4Output(Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
                "output.mp4"));
            videoCapture1.Outputs_Add(mp4Output);

            videoCapture1.Audio_Record = true;

            // Start capture
            await videoCapture1.StartAsync();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // We have to initialize the engine on start
            Text += " [FIRST TIME LOAD, BUILDING THE REGISTRY...]";
            this.Enabled = false;
            await VisioForgeX.InitSDKAsync();
            this.Enabled = true;
            Text = Text.Replace("[FIRST TIME LOAD, BUILDING THE REGISTRY...]", "");
        }

        private async void btStop_Click(object sender, EventArgs e)
        {
            // Stop capture
            await videoCapture1.StopAsync();

            // Release resources
            await videoCapture1.DisposeAsync();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Release SDK
            VisioForgeX.DestroySDK();
        }
    }
}
```

## Additional Code Examples

### WebM Output with VP9

For WebM output with VP9 encoding, simply modify the encoder settings:

```csharp
using VisioForge.Core.Types.X.Output;

// Configure WebM output with VP9 codec
var webmOutput = new WebMOutput(Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), 
    "output.webm"));
videoCapture1.Outputs_Add(webmOutput);
```

### Enable Snapshot Grabber

Here's a simple example of how to just save a single image from the webcam. Enable video sample grabber:

```csharp
// Enable snapshot grabber before starting capture
videoCapture1.Snapshot_Grabber_Enabled = true;
await videoCapture1.StartAsync();
```

### Save Snapshot

Get and save a single image from the webcam:

```csharp
using SkiaSharp;

// Save current frame as JPEG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.jpg", 
    SKEncodedImageFormat.Jpeg, 
    85);

// Or save as PNG
await videoCapture1.Snapshot_SaveAsync(
    "snapshot.png", 
    SKEncodedImageFormat.Png);
```

## Native Dependencies

Video Capture SDK .Net relies on native libraries to access webcam devices and perform video and audio processing. These native dependencies are bundled with the SDK and are automatically deployed with your application, ensuring seamless integration and compatibility across different systems.

### Package Reference

Main SDK package:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
```

### Platform-Specific Redist Packages

Windows x64:

```xml
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

For other platforms:

```xml
<!-- macOS -->
<PackageReference Include="VisioForge.CrossPlatform.Core.macOS" Version="2025.9.1" />

<!-- Linux x64 -->
<PackageReference Include="VisioForge.CrossPlatform.Core.Linux.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Linux.x64" Version="2025.11.0" />
```

## Cross-Platform Compatibility

Video Capture SDK .Net is designed with cross-platform compatibility in mind, making it an ideal choice for developers working on applications that need to run on multiple operating systems.

### MAUI Compatibility

For developers working with .NET MAUI (Multi-platform App UI), Video Capture SDK .Net offers:

- Full compatibility with MAUI applications
- Consistent API across all supported platforms
- Platform-specific optimizations while maintaining a unified codebase
- Sample MAUI projects demonstrating implementation across platforms

This cross-platform capability allows developers to write code once and deploy across Windows, macOS, and mobile platforms through MAUI, significantly reducing development time and maintenance overhead.

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) provides a comprehensive solution for adding webcam video capture capabilities to your DotNet applications. Whether you need to record webcam footage, save webcam images, or simply display the webcam feed in your application, this library makes the process simple with just a few lines of C# code.

With support for industry-standard formats like MP4 and WebM, modern codecs including H.264/H.265 and VP8/VP9/AV1, and powerful GPU acceleration, it offers the performance and flexibility needed for even the most demanding video capture applications. The ability to create and save video files efficiently makes this library perfect for any application that needs to record webcam content.

The SDK's cross-platform compatibility, extending to macOS and MAUI applications, ensures that your webcam capture solution works consistently across different operating systems. Whether you're building a video conferencing tool, a surveillance application, or any other software requiring webcam functionality, Video Capture SDK .Net offers the tools you need to implement these features quickly.

Getting started is as simple as following the step-by-step tutorial and code examples provided above. For more advanced use cases and detailed documentation on how to record webcam video using .NET, visit our website or refer to the SDK documentation.

## Frequently Asked Questions

### Which format should I use to save webcam video — MP4 or WebM?

MP4 with H.264 encoding is the best choice for most applications because it offers broad device and player compatibility, efficient compression, and hardware-accelerated encoding on most GPUs. Choose WebM with VP9 or AV1 if you need a royalty-free format for web delivery or browser-based playback. Both formats support high-quality audio recording alongside video.

### How do I set the resolution and frame rate for webcam recording?

Use the `VideoCaptureDeviceSourceSettings` class to select a specific resolution and frame rate from your camera's supported formats. After creating the source settings object, call `GetFormats()` to enumerate available modes, then assign your preferred format before starting capture. The SDK automatically negotiates the closest match if the exact format is unavailable.

### Can I use GPU acceleration to record webcam video in C#?

Yes. Video Capture SDK .Net automatically detects available GPU hardware — NVIDIA NVENC, AMD AMF/VCE, and Intel Quick Sync Video — and selects hardware-accelerated encoding when you create an `MP4Output` or `WebMOutput`. No additional configuration is required. If no compatible GPU is found, the SDK falls back to optimized software encoding transparently.

### How do I record webcam video with audio?

Set the `Audio_Source` property to a microphone or other audio input device, then enable recording with `Audio_Record = true`. To monitor audio during capture, set `Audio_Play = true` and assign an `Audio_OutputDevice`. When saving to MP4, audio is encoded as AAC by default; WebM uses Vorbis.

### Does webcam recording work on macOS and Linux?

Yes. The SDK is fully cross-platform. Add the appropriate platform-specific NuGet redist packages (macOS, Linux x64) alongside the main `VisioForge.DotNet.VideoCapture` package. The same C# code runs on Windows, macOS, and Linux, and the SDK also supports .NET MAUI for mobile and cross-platform desktop deployment.

## VB.NET Support

Looking for VB.NET webcam recording? See our dedicated guide: [Record Webcam Video in VB.NET](record-webcam-vb-net.md).

## See Also

- [Record Webcam Video in VB.NET](record-webcam-vb-net.md) — same functionality with VB.NET code examples
- [Screen Capture to MP4](../video-tutorials/screen-capture-mp4.md) — record desktop screen instead of webcam
- [Webcam to MP4 Tutorial](../video-tutorials/video-capture-webcam-mp4.md) — step-by-step MP4 recording walkthrough
- [IP Camera Capture to MP4](../video-tutorials/ip-camera-capture-mp4.md) — record from network cameras instead of local webcams
- [Barcode & QR Code Scanner](../../mediablocks/Guides/barcode-qr-reader-guide.md) — combine webcam capture with barcode detection
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
