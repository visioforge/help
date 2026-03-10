---
title: Screen Capture to AVI with MJPEG Encoding in C# .NET
description: Record screen to AVI format in C# with MJPEG or uncompressed video. When to choose AVI over MP4, codec options, and complete code examples using VisioForge SDK.
sidebar_label: Screen Capture to AVI
---

# C# Screen Capture to AVI Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Video Tutorial Walkthrough

Watch our detailed tutorial that demonstrates the implementation process:

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/AUT8oVPinUs?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

## Source Code Repository

Access the complete source code for this tutorial:

[Source code on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets/screen-capture-avi)

## When to Use AVI for Screen Recording

AVI (Audio Video Interleave) is a legacy container format that remains useful in specific scenarios:

- **DirectShow-based editing workflows** — AVI files integrate seamlessly with DirectShow filters and older video editing tools
- **MJPEG codec** — Each frame is independently compressed, making AVI ideal for frame-by-frame video editing where you need random access to any frame without decoding preceding frames
- **Maximum compatibility** — AVI is supported by virtually every video player and editor on Windows, including legacy applications
- **Simple codec structure** — Unlike MP4's complex atom-based format, AVI has a straightforward structure that's easier to recover from incomplete recordings

**Trade-off:** AVI files with MJPEG are significantly larger than MP4 files with H.264. A 1080p recording at 25 FPS produces roughly 150–200 MB per minute with MJPEG, compared to ~25 MB per minute with H.264 MP4.

For most screen recording use cases, [MP4 is the recommended format](screen-capture-mp4.md). Use AVI when you specifically need MJPEG frame independence, DirectShow compatibility, or uncompressed capture.

## Modern API — Video Capture SDK X

The modern cross-platform API uses `VideoCaptureCoreX`. For the complete console application pattern with screen capture setup, audio configuration, and recording lifecycle, see the [Screen Capture to MP4](screen-capture-mp4.md#modern-api-video-capture-sdk-x) guide. To output AVI instead of MP4, replace the output configuration:

```csharp
// AVI with default codecs (OpenH264 video + MP3 audio)
var aviOutput = new AVIOutput(outputPath);
videoCapture.Outputs_Add(aviOutput, autostart: true);
```

### AVI Codec Options

Choose the video encoder based on your workflow:

```csharp
// MJPEG — frame-independent, larger files, ideal for editing
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new MJPEGEncoderSettings();

// H.264 in AVI container — smaller files, less editing-friendly
var aviOutput = new AVIOutput(outputPath);
aviOutput.Video = new OpenH264EncoderSettings();
```

Region capture, multi-monitor recording, audio, cursor highlighting, and GPU encoding options are covered in the [MP4 guide](screen-capture-mp4.md) — all source configuration features work identically with AVI output.

## Legacy API — Video Capture SDK

This WPF example demonstrates screen capture to AVI using the legacy `VideoCaptureCore` API. Add a `VideoView` control named `VideoView1` to your XAML window.

### Code Example

```csharp
using System;
using System.IO;
using System.Windows;
using VisioForge.Core.VideoCapture;
using VisioForge.Core.Types;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoCapture;

namespace screen_capture_avi
{
    public partial class MainWindow : Window
    {
        private VideoCaptureCore videoCapture1;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            videoCapture1 = new VideoCaptureCore(VideoView1 as IVideoView);
        }

        private async void btStart_Click(object sender, RoutedEventArgs e)
        {
            // Capture the entire screen
            videoCapture1.Screen_Capture_Source = new ScreenCaptureSourceSettings
            {
                FullScreen = true
            };

            // Video only — no audio
            videoCapture1.Audio_RecordAudio = false;
            videoCapture1.Audio_PlayAudio = false;

            videoCapture1.Output_Filename = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "output.avi");

            // AVI output with MJPEG video and PCM audio
            videoCapture1.Output_Format = new AVIOutput();
            videoCapture1.Mode = VideoCaptureMode.ScreenCapture;

            await videoCapture1.StartAsync();
        }

        private async void btStop_Click(object sender, RoutedEventArgs e)
        {
            await videoCapture1.StopAsync();
        }
    }
}
```

### Required Dependencies

Install the following NuGet packages:

- Video capture redistributable components:
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Frequently Asked Questions

### Why is my AVI screen recording file so large?

MJPEG compresses each frame independently, trading file size for editing convenience (see the size comparison above). To reduce file size: lower the frame rate (10–15 FPS is sufficient for presentations), capture a smaller region instead of full screen, or switch to [MP4 output](screen-capture-mp4.md) which uses inter-frame H.264 compression and produces files about 6–8x smaller.

### When should I use AVI instead of MP4 for screen recording?

Choose AVI when you need frame-independent access for video editing (MJPEG allows scrubbing to any frame without decoding previous frames), when working with legacy DirectShow-based pipelines, or when you need maximum compatibility with older Windows video tools. For all other cases, MP4 with H.264 offers better compression, smaller files, and broader cross-platform support.

## See Also

- [Screen Capture to MP4](screen-capture-mp4.md) — recommended format with full feature coverage (region, multi-monitor, audio, GPU encoding, cross-platform)
- [Screen Capture to WMV](screen-capture-wmv.md) — Windows Media format alternative
- [Screen Capture in VB.NET](../guides/screen-capture-vb-net.md) — screen recording in Visual Basic .NET
- [Screen Source Configuration](../video-sources/screen.md) — full reference for capture settings
- [Code Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/_CodeSnippets) — additional screen capture code snippets on GitHub
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
