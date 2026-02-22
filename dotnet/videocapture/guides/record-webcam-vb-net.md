---
title: Record Webcam Video in VB.NET - Capture & Save Guide
description: Learn to record webcam video in VB.NET with device enumeration, MP4 output, live preview, and screenshot capture using async/await.
---

# Record Webcam Video in VB.NET: Complete Guide

Recording webcam video in VB.NET applications is a common requirement for video conferencing, surveillance, and media projects. This guide provides step-by-step instructions to capture, preview, and save webcam video to MP4 files using [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) in VB.NET (Visual Basic .NET).

## Why Use Video Capture SDK .Net for VB.NET Webcam Recording

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) provides first-class VB.NET support with:

- Full async/await support for non-blocking webcam capture
- Device enumeration for video sources, audio sources, and audio outputs
- MP4 recording with H.264/H.265 encoding and GPU acceleration
- Real-time webcam preview during recording
- Screenshot capture from live webcam feed
- WinForms and WPF support

## Required NuGet Packages

Add the following packages to your VB.NET project:

```xml
<PackageReference Include="VisioForge.DotNet.VideoCapture" Version="2026.2.19" />
<PackageReference Include="VisioForge.CrossPlatform.Core.Windows.x64" Version="2025.11.0" />
<PackageReference Include="VisioForge.CrossPlatform.Libav.Windows.x64" Version="2025.11.0" />
```

## Complete VB.NET Webcam Recording Example

The following example demonstrates a full WinForms application that enumerates webcam devices, previews the webcam feed, and records video to MP4.

### SDK Initialization and Device Enumeration

First, initialize the SDK and enumerate available devices when the form loads:

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

        ' Create the video capture engine
        VideoCapture1 = New VideoCaptureCoreX(VideoView1)
        AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError

        ' Enumerate video sources (webcams)
        Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
        For Each source In videoSources
            cbVideoInputDevice.Items.Add(source.DisplayName)
            If cbVideoInputDevice.Items.Count = 1 Then cbVideoInputDevice.SelectedIndex = 0
        Next

        ' Enumerate audio sources (microphones)
        Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
        For Each source In audioSources
            cbAudioInputDevice.Items.Add(source.DisplayName)
            If cbAudioInputDevice.Items.Count = 1 Then cbAudioInputDevice.SelectedIndex = 0
        Next

        ' Enumerate audio outputs (speakers)
        Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
        For Each audioOutput In audioOutputs
            cbAudioOutputDevice.Items.Add(audioOutput.DisplayName)
            If cbAudioOutputDevice.Items.Count = 1 Then cbAudioOutputDevice.SelectedIndex = 0
        Next

        edOutput.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "capture.mp4")
    End Sub
```

### Listing Available Video Formats and Frame Rates

When the user selects a webcam device, populate the available video formats:

```vb.net
    Private Async Sub cbVideoInputDevice_SelectedIndexChanged(
            sender As Object, e As EventArgs) Handles cbVideoInputDevice.SelectedIndexChanged

        If cbVideoInputDevice.SelectedIndex = -1 Then Return

        cbVideoInputFormat.Items.Clear()
        cbVideoInputFrameRate.Items.Clear()

        Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
        Dim device = videoSources.FirstOrDefault(
            Function(d) d.DisplayName = cbVideoInputDevice.Text)

        If device IsNot Nothing Then
            For Each videoFormat In device.VideoFormats
                cbVideoInputFormat.Items.Add(videoFormat.Name)
            Next
            If cbVideoInputFormat.Items.Count > 0 Then cbVideoInputFormat.SelectedIndex = 0
        End If
    End Sub
```

### Starting Webcam Recording to MP4

Configure the video and audio sources, set the MP4 output, and start the recording:

```vb.net
    Private Async Sub btStart_Click(sender As Object, e As EventArgs) Handles btStart.Click
        Try
            ' Configure video source
            Dim videoSources = Await DeviceEnumerator.Shared.VideoSourcesAsync()
            Dim videoDevice = videoSources.FirstOrDefault(
                Function(d) d.DisplayName = cbVideoInputDevice.Text)

            If videoDevice IsNot Nothing Then
                Dim videoSourceSettings As New VideoCaptureDeviceSourceSettings(videoDevice)
                VideoCapture1.Video_Source = videoSourceSettings
            End If

            ' Configure audio source
            Dim audioSources = Await DeviceEnumerator.Shared.AudioSourcesAsync()
            Dim audioDevice = audioSources.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioInputDevice.Text)

            If audioDevice IsNot Nothing Then
                VideoCapture1.Audio_Source = audioDevice.CreateSourceSettingsVC(Nothing)
            End If

            ' Configure audio output for live monitoring
            Dim audioOutputs = Await DeviceEnumerator.Shared.AudioOutputsAsync()
            Dim audioOutput = audioOutputs.FirstOrDefault(
                Function(d) d.DisplayName = cbAudioOutputDevice.Text)

            If audioOutput IsNot Nothing Then
                VideoCapture1.Audio_OutputDevice = New AudioRendererSettings(audioOutput)
            End If

            VideoCapture1.Audio_Play = True
            VideoCapture1.Audio_Record = True
            VideoCapture1.Video_Play = True

            ' Configure MP4 output
            VideoCapture1.Outputs_Clear()
            VideoCapture1.Outputs_Add(New MP4Output(edOutput.Text), True)

            ' Start recording
            Await VideoCapture1.StartAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Stopping Webcam Recording

Stop the recording and release resources:

```vb.net
    Private Async Sub btStop_Click(sender As Object, e As EventArgs) Handles btStop.Click
        Try
            Await VideoCapture1.StopAsync()
        Catch ex As Exception
            MessageBox.Show(Me, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
```

### Saving Screenshots from Webcam

Capture still images from the live webcam feed:

```vb.net
    Private Async Sub btSaveScreenshot_Click(sender As Object, e As EventArgs) Handles btSaveScreenshot.Click
        Dim saveDialog As New SaveFileDialog With {
            .Filter = "JPEG|*.jpg|PNG|*.png|BMP|*.bmp",
            .FileName = "snapshot.jpg"
        }

        If saveDialog.ShowDialog() = DialogResult.OK Then
            Dim format As SKEncodedImageFormat = SKEncodedImageFormat.Jpeg
            If saveDialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase) Then
                format = SKEncodedImageFormat.Png
            End If

            Await VideoCapture1.Snapshot_SaveAsync(saveDialog.FileName, format)
        End If
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

To preview the webcam feed without recording, simply skip adding the MP4 output:

```vb.net
' No output added - preview only
VideoCapture1.Video_Play = True
VideoCapture1.Audio_Play = True

Await VideoCapture1.StartAsync()
```

## Output Format Options

While this guide focuses on MP4 recording, you can save webcam video in other formats:

### WebM Output

```vb.net
VideoCapture1.Outputs_Add(New WebMOutput(edOutput.Text), True)
```

### AVI Output

```vb.net
VideoCapture1.Outputs_Add(New AVIOutput(edOutput.Text), True)
```

For detailed format configuration, see the [MP4 format documentation](../../general/output-formats/mp4.md) and [WebM format documentation](../../general/output-formats/webm.md).

## Sample Applications

Complete VB.NET sample applications are available:

- [Simple Video Capture VB.NET (WinForms)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/WinForms/VB/Simple%20Video%20Capture%20X%20VB) — full-featured webcam capture application

## Frequently Asked Questions

### How do I convert C# webcam capture examples to VB.NET?

The Video Capture SDK .Net API is identical in C# and VB.NET — only the language syntax differs. Key conversions: replace `+=` event handlers with `AddHandler`/`AddressOf`, replace C# lambdas `(x) =>` with `Function(x)` or `Sub(x)`, and use `Async Sub` instead of `async void`. All classes, methods, and properties shown in C# documentation work directly in VB.NET with these syntax adjustments.

### Does VB.NET support async webcam recording with Await?

Yes. VB.NET fully supports `Async`/`Await` for all SDK operations including `StartAsync()`, `StopAsync()`, and `Snapshot_SaveAsync()`. Mark event handlers as `Async Sub` and use `Await` before each SDK call to keep the UI thread responsive during webcam recording. The VB.NET async pattern is functionally identical to C# `async`/`await`.

### Which VB.NET project types support webcam video capture?

The SDK supports VB.NET WinForms and WPF applications on .NET 8+ and .NET Framework 4.7.2+. WinForms is the most common choice for VB.NET webcam projects because it provides the `VideoView` control via the visual designer. For WPF, add the `VideoView` control in XAML. Console applications can record without preview by omitting the `VideoView` parameter.

### How do I handle webcam errors and device disconnection in VB.NET?

Subscribe to the `OnError` event using `AddHandler VideoCapture1.OnError, AddressOf VideoCapture1_OnError`. Inside the handler, use `Me.InvokeRequired` and `Me.Invoke()` to safely update the UI from a background thread. Wrap `StartAsync()` and `StopAsync()` calls in `Try`/`Catch` blocks to handle device disconnection or permission errors gracefully.

## See Also

- [Save Webcam Video in C#](save-webcam-video.md) — same functionality with C# code examples
- [Screen Capture in VB.NET](screen-capture-vb-net.md) — record desktop screen instead of webcam in Visual Basic
- [Build a Video Player in VB.NET](../../mediaplayer/guides/video-player-vb-net.md) — VB.NET media player with playback controls
- [Webcam to MP4 Tutorial](../video-tutorials/video-capture-webcam-mp4.md) — step-by-step C# webcam recording walkthrough
- [Web Camera Photo Capture](make-photo-using-webcam.md) — capture still images from webcam instead of video
- [Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net) — product page and downloads
