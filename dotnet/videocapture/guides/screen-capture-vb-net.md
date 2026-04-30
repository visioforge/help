---
title: Screen Capture in VB.NET - Record Desktop Video to MP4
description: Learn how to record your desktop screen in VB.NET. Complete guide for full screen and region capture to MP4 with system audio using Video Capture SDK .Net.
tags:
  - Video Capture SDK
  - .NET
  - VideoCaptureCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Webcam
  - Screen Capture
  - MP4
  - WebM
  - AVI
  - C#
  - VB.NET
  - NuGet
primary_api_classes:
  - ScreenCaptureD3D11SourceSettings
  - VideoCaptureCoreX
  - MP4Output
  - VideoView
  - LoopbackAudioCaptureDeviceSourceSettings

---

# Screen Capture in VB.NET: Complete Guide to Record Desktop Video

Recording your desktop screen in VB.NET (Visual Basic .NET) applications is essential for building screen recorders, tutorial tools, and surveillance software. Whether you need full screen capture, region recording, or system audio capture, this guide provides step-by-step Visual Basic code examples using [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net).

## Why Use Video Capture SDK .Net for VB.NET Screen Recording

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) provides a complete screen recording solution for VB.NET developers:

- Full desktop capture and custom region screen recording
- Multi-monitor support with display enumeration
- Mouse cursor capture with optional click highlighting
- System audio recording (microphone and loopback/system sound)
- MP4 output with H.264/H.265 encoding and GPU acceleration
- Real-time screen preview during recording
- Async/await pattern for non-blocking capture
- WinForms and WPF support

## Required NuGet Packages

Add the following packages to your VB.NET project:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Complete VB.NET Screen Recording Example

The following screen capture example demonstrates a WinForms application that records the desktop with audio and saves it to an MP4 file.

### SDK Initialization and Display Enumeration

Initialize the SDK and enumerate available displays and audio devices when the form loads:

```vb.net
Imports System.IO
Imports VisioForge.Core
Imports VisioForge.Core.VideoCaptureX
Imports VisioForge.Core.Types.X.Sources
Imports VisioForge.Core.Types.X.Output
Imports VisioForge.Core.Types.X.AudioRenderers

Public Class Form1
    Private VideoCapture1 As VideoCaptureCoreX

    Private Async Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize the SDK
        Await VisioForgeX.InitSDKAsync()

        ' Create the video capture engine with VideoView for preview
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Enumerate available displays
        For i As Integer = 0 To Screen.AllScreens.Length - 1
            Dim scr = Screen.AllScreens(i)
            cbDisplayIndex.Items.Add(
                $"{i}: {scr.DeviceName} ({scr.Bounds.Width}x{scr.Bounds.Height})")
        Next
        If cbDisplayIndex.Items.Count > 0 Then cbDisplayIndex.SelectedIndex = 0

        ' Enumerate audio input devices (microphones)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Enumerate audio loopback devices (system sound)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
            AudioOutputDeviceAPI.WASAPI2)
        For Each audioOutput In audioOutputs
            cbAudioLoopbackDevice.Items.Add(audioOutput.Name)
            If cbAudioLoopbackDevice.Items.Count = 1 Then cbAudioLoopbackDevice.SelectedIndex = 0
        Next

        ' Set default output file path
        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
            "screen_capture.mp4")
    End Sub
```

### Full Screen Capture to MP4

Use the [ScreenCaptureD3D11SourceSettings](../video-sources/screen.md) class to capture the entire desktop at a specified frame rate and save to MP4:

```vb.net
    Private Async Sub btStartFullScreen_Click(
            sender As Object, e As EventArgs) Handles btStartFullScreen.Click
        Try
            ' Configure full screen capture source
            Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
                .MonitorIndex = cbDisplayIndex.SelectedIndex,
                .FrameRate = New VideoFrameRate(15, 1),
                .CaptureCursor = True
            }

            VideoCapture1.Video_Source = screenSource
            VideoCapture1.Video_Play = True

            ' Configure MP4 output
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Start capture
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Region Capture (Custom Area)

Capture a specific rectangular area of the screen instead of the full desktop:

```vb.net
    Private Async Sub btStartRegion_Click(
            sender As Object, e As EventArgs) Handles btStartRegion.Click
        Try
            ' Configure region capture with custom rectangle
            Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
                .Rectangle = New Rect(
                    CInt(edLeft.Text),
                    CInt(edTop.Text),
                    CInt(edRight.Text),
                    CInt(edBottom.Text)),
                .FrameRate = New VideoFrameRate(15, 1),
                .CaptureCursor = True
            }

            VideoCapture1.Video_Source = screenSource
            VideoCapture1.Video_Play = True

            ' Configure MP4 output
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Start capture
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Recording with System Audio (Microphone)

Add microphone audio recording to your screen capture:

```vb.net
    ' Configure microphone audio source
    Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
    Dim audioDevice = audioSources.FirstOrDefault(
        Function(d) d.DisplayName = cbAudioInputDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
    End If

    VideoCapture1.Audio_Record = True
```

### Recording with Loopback Audio (System Sound)

Capture system audio output (what you hear from speakers) instead of microphone input:

```vb.net
    ' Configure loopback audio source (system sound)
    Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync(
        AudioOutputDeviceAPI.WASAPI2)
    Dim audioDevice = audioOutputs.FirstOrDefault(
        Function(d) d.Name = cbAudioLoopbackDevice.Text)

    If audioDevice IsNot Nothing Then
        VideoCapture1.Audio_Source = New LoopbackAudioCaptureDeviceSourceSettings(audioDevice)
    End If

    VideoCapture1.Audio_Record = True
```

### Stopping Recording

Stop the screen recording and finalize the output file:

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Cleanup on Form Close

Properly dispose of resources when the application exits:

```vb.net
    Private Async Sub Form1_FormClosing(
            sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing

        If VideoCapture1 IsNot Nothing Then
            Await VideoCapture1.DisposeAsync()
            VideoCapture1 = Nothing
        End If

        VisioForgeX.DestroySDK()
    End Sub

    Private Sub VideoCapture1_OnError(sender As Object, e As ErrorsEventArgs)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() mmLog.Text &= e.Message & Environment.NewLine)
        Else
            mmLog.Text &= e.Message & Environment.NewLine
        End If
    End Sub
End Class
```

## Preview-Only Mode (No Recording)

To preview the desktop capture feed without saving to a file, skip adding the output:

```vb.net
' Configure screen source
Dim screenSource As New ScreenCaptureD3D11SourceSettings() With {
    .MonitorIndex = 0,
    .FrameRate = New VideoFrameRate(15, 1),
    .CaptureCursor = True
}

VideoCapture1.Video_Source = screenSource
VideoCapture1.Video_Play = True

' No output added - preview only
Await VideoCapture1.StartAsync()
```

## Output Format Options

While this guide focuses on [MP4 recording](../../general/output-formats/mp4.md), you can save your screen capture in other formats:

### WebM Output

[WebM](../../general/output-formats/webm.md) is ideal for web-based screen recordings, offering royalty-free VP8/VP9 encoding with smaller file sizes:

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### AVI Output

AVI provides maximum compatibility with legacy video editors and Windows-based workflows:

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

## Sample Applications

Complete VB.NET sample applications are available:

- [Screen Capture X VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Screen%20Capture%20X%20VB) — full-featured screen capture application with region and full screen modes

## Frequently Asked Questions

### How do I record both microphone and system audio simultaneously in VB.NET screen capture?

`VideoCaptureCoreX` exposes a single `Audio_Source`, so to mix a microphone with system-loopback audio you build a Media Blocks pipeline: one `SystemAudioSourceBlock` for the microphone, one loopback source, and an `AudioMixerBlock` that combines them before the encoder/sink. The [Media Blocks audio processing reference](../../mediablocks/AudioProcessing/index.md) covers `AudioMixerBlock` setup. Alternatively, on Windows switch to `VideoCaptureCore` (non-X) where `Additional_Audio_CaptureDevice_MixChannels` + the internal additional-devices list handle mixing directly.

### What frame rate should I use for VB.NET screen recording?

Use 10–15 FPS for general desktop recording (documents, presentations, browsing) and 25–30 FPS for motion-intensive content like video playback or animations. Higher frame rates increase CPU usage and file size. The example code in this guide uses `New VideoFrameRate(15, 1)` — change the first parameter to your desired FPS. For GPU-accelerated encoding, pair higher frame rates with `MP4Output` which uses hardware H.264 encoding when available.

### Can I record the screen without showing a preview in VB.NET?

Yes. Create `VideoCaptureCoreX` without a `VideoView` parameter: `VideoCapture1 = New VideoCaptureCoreX()`. Set `Video_Play = False` to skip rendering, configure the screen source and MP4 output as usual, then call `Await VideoCapture1.StartAsync()`. This reduces CPU usage because no frames are rendered to the UI. Console applications and Windows Services can use this approach for headless screen recording.

### How do I handle high-DPI and scaled displays in VB.NET screen recording?

`ScreenCaptureD3D11SourceSettings` captures the native screen resolution regardless of Windows display scaling. A 4K monitor at 150% scaling is still recorded at 3840×2160 pixels. If your VB.NET WinForms application displays incorrect coordinates in region capture, add `<dpiAware>true</dpiAware>` to your `app.manifest` file or call `SetProcessDPIAware()` at startup so that `Screen.AllScreens` reports physical pixel dimensions instead of scaled values.

## See Also

- [Screen Capture to MP4 Tutorial](../video-tutorials/screen-capture-mp4.md) — step-by-step C# screen recording walkthrough
- [Screen Source Configuration](../video-sources/screen.md) — DirectX 11/12 and WGC capture API reference
- [Record Webcam Video in VB.NET](record-webcam-vb-net.md) — capture from webcam instead of screen in Visual Basic
- [Save Webcam Video in C#](save-webcam-video.md) — same webcam capture functionality with C# code examples
- [Build a Video Player in VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — VB.NET media player with playback controls
- [Pre-Event Recording](pre-event-recording.md) — circular buffer recording that captures footage before the trigger event
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
