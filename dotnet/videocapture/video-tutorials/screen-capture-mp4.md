---
title: Screen Capture to MP4 with .NET | C# Video Recording
description: Implement screen recording to MP4 with C# in .NET using complete code examples for audio and silent captures with H264/AAC encoding.
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

## Code Example

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

## How It Works

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

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.