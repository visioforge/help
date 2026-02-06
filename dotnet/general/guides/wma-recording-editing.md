---
title: Record and Edit WMA Files in C# and .NET
description: Record WMA audio from microphone and edit WMA files in .NET with VideoCaptureCoreX and VideoEditCoreX classes for Windows Media Audio capture and editing.
---

# Record and Edit WMA Files in C# and .NET: A Comprehensive Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to WMA Recording and Editing in .NET

This article provides a comprehensive guide for developers working with Windows Media Audio (WMA) files in .NET applications. We'll explore how to record .NET WMA audio from microphones and other capture devices using the VideoCaptureCoreX class, and how to edit dotnet WMA files using the VideoEditCoreX class from the VisioForge .NET SDKs.

Windows Media Audio is a popular audio format developed by Microsoft that offers excellent compression while maintaining good audio quality. The WMA format is widely used in Windows applications and supports various bitrates and sample rates, making it suitable for both voice records and high-quality music.

The VisioForge library provides powerful classes for capturing audio data from system devices and processing audio video content. Whether you need to create a simple voice recorder console application or a complex WinForms audio editor, these SDKs deliver the functionality you need. This guide will show you how to capture dotnet WMA audio and record csharp WMA files with ease.

## Prerequisites and Installation

Before you begin recording or editing WMA files in your dotnet application, ensure you have the following:

- Visual Studio 2019 or later
- .NET 6.0 or later (or .NET Framework 4.7.2+)
- VisioForge Video Capture SDK .NET or Video Edit SDK .NET

### Installing the NuGet Packages

Install the required packages using the NuGet Package Manager:

```bash
# For WMA recording with VideoCaptureCoreX
Install-Package VisioForge.DotNet.VideoCapture

# For WMA editing with VideoEditCoreX
Install-Package VisioForge.DotNet.VideoEdit
```

For detailed installation instructions, please refer to the [installation guide](../../install/index.md).

## Recording WMA Files from Microphone Using VideoCaptureCoreX

The VideoCaptureCoreX class provides a straightforward approach to capture csharp WMA audio from microphones and other audio input devices. This section demonstrates how to record csharp WMA audio files with proper device enumeration and encoder configuration. Learn how to capture csharp WMA content for various application scenarios.

### Core Components for WMA Recording

1. **VideoCaptureCoreX**: Main engine class for managing audio capture and WMA output in .NET.
2. **DeviceEnumerator**: Class for discovering available audio capture devices on the system.
3. **AudioCaptureDeviceSourceSettings**: Configuration settings for microphone or audio input device.
4. **WMAOutput**: Output format configuration specifically for Windows Media Audio file creation.
5. **WMAEncoderSettings**: Settings class for WMA encoder parameters like bitrate and sample rate.

### Basic WMA Recording Implementation

Here's a complete csharp implementation to capture and record WMA files from a microphone:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public class WmaRecorder
{
    private VideoCaptureCoreX _videoCapture;

    // Call this method once during application startup or form load
    public async Task InitializeAsync()
    {
        // Initialize the VisioForge SDK
        await VisioForgeX.InitSDKAsync();
    }

    public async Task StartRecordingAsync(string outputPath)
    {
        // Create VideoCaptureCoreX instance for audio capture
        _videoCapture = new VideoCaptureCoreX();

        // Configure audio capture device (microphone)
        await ConfigureAudioSourceAsync();

        // Disable video capture - we only need audio
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;

        // Configure audio settings
        _videoCapture.Audio_Play = false;    // Disable audio monitoring during recording
        _videoCapture.Audio_Record = true;   // Enable audio recording to file

        // Configure WMA output format
        var wmaOutput = new WMAOutput(outputPath);
        
        // Configure WMA encoder settings
        wmaOutput.Audio.Bitrate = 192;       // 192 Kbps bitrate
        wmaOutput.Audio.SampleRate = 48000;  // 48 kHz sample rate
        wmaOutput.Audio.Channels = 2;        // Stereo recording

        // Add WMA output to capture pipeline
        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Start the audio capture process
        await _videoCapture.StartAsync();

        Console.WriteLine("Recording started. Press any key to stop...");
    }

    private async Task ConfigureAudioSourceAsync()
    {
        // Get available audio capture devices using DirectSound API
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        if (audioDevices.Length == 0)
        {
            throw new Exception("No audio capture device found.");
        }

        // Get first available audio capture device (usually the default microphone)
        var audioDevice = audioDevices[0];

        // Get supported format from the device
        var audioFormat = audioDevice.GetDefaultFormat();

        // Create audio source settings with the selected device and format
        var audioSourceSettings = audioDevice.CreateSourceSettingsVC(audioFormat);

        // Configure audio capture device
        _videoCapture.Audio_Source = audioSourceSettings;
    }

    public async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            // Stop all capture and encoding
            await _videoCapture.StopAsync();

            // Clean up resources
            await _videoCapture.DisposeAsync();
            _videoCapture = null;

            Console.WriteLine("Recording stopped and file saved.");
        }
    }
}
```

### Console Application Example for WMA Recording

Here's a complete console application that records WMA audio from a microphone:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("WMA Audio Recorder - Console Application");
        Console.WriteLine("========================================");

        // Initialize SDK
        await VisioForgeX.InitSDKAsync();

        // Create capture instance
        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Enumerate and display available audio capture devices
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            Console.WriteLine("\nAvailable audio capture devices:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No audio capture device found. Exiting.");
                return;
            }

            // Select first device for recording
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configure video capture for audio-only recording
            videoCapture.Audio_Source = audioSourceSettings;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configure WMA output file
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"recording_{DateTime.Now:yyyyMMdd_HHmmss}.wma");

            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoCapture.Outputs_Add(wmaOutput, autostart: true);

            // Start recording
            Console.WriteLine($"\nRecording to: {outputPath}");
            Console.WriteLine("Press ENTER to stop recording...\n");

            await videoCapture.StartAsync();

            // Wait for user input to stop
            Console.ReadLine();

            // Stop recording
            await videoCapture.StopAsync();
            Console.WriteLine($"\nRecording saved to: {outputPath}");
        }
        finally
        {
            // Cleanup
            await videoCapture.DisposeAsync();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### WinForms Application for WMA Recording

For a Windows Forms application with visual controls, here's an implementation example:

```csharp
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

public partial class WmaRecorderForm : Form
{
    private VideoCaptureCoreX _videoCapture;
    private bool _isRecording = false;

    public WmaRecorderForm()
    {
        InitializeComponent();
    }

    private async void Form_Load(object sender, EventArgs e)
    {
        // Initialize SDK
        await VisioForgeX.InitSDKAsync();

        // Populate audio device dropdown
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);

        foreach (var device in audioDevices)
        {
            cmbAudioDevices.Items.Add(device.DisplayName);
        }

        if (cmbAudioDevices.Items.Count > 0)
        {
            cmbAudioDevices.SelectedIndex = 0;
        }

        // Set default output path
        txtOutputPath.Text = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
            "recording.wma");
    }

    private async void btnStartStop_Click(object sender, EventArgs e)
    {
        if (!_isRecording)
        {
            await StartRecordingAsync();
        }
        else
        {
            await StopRecordingAsync();
        }
    }

    private async Task StartRecordingAsync()
    {
        _videoCapture = new VideoCaptureCoreX();

        // Get selected audio device
        var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
            AudioCaptureDeviceAPI.DirectSound);
        var selectedDevice = audioDevices.FirstOrDefault(
            d => d.DisplayName == cmbAudioDevices.SelectedItem.ToString());

        if (selectedDevice == null)
        {
            MessageBox.Show("Please select an audio device.");
            return;
        }

        // Configure audio source
        var audioFormat = selectedDevice.GetDefaultFormat();
        var audioSourceSettings = selectedDevice.CreateSourceSettingsVC(audioFormat);
        _videoCapture.Audio_Source = audioSourceSettings;

        // Configure for audio-only capture
        _videoCapture.Video_Source = null;
        _videoCapture.Video_Play = false;
        _videoCapture.Audio_Play = false;
        _videoCapture.Audio_Record = true;

        // Configure WMA output
        var wmaOutput = new WMAOutput(txtOutputPath.Text);
        wmaOutput.Audio.Bitrate = (int)numBitrate.Value;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = rbStereo.Checked ? 2 : 1;

        _videoCapture.Outputs_Add(wmaOutput, autostart: true);

        // Start recording
        await _videoCapture.StartAsync();

        _isRecording = true;
        btnStartStop.Text = "Stop Recording";
        lblStatus.Text = "Recording...";
    }

    private async Task StopRecordingAsync()
    {
        if (_videoCapture != null)
        {
            await _videoCapture.StopAsync();
            await _videoCapture.DisposeAsync();
            _videoCapture = null;
        }

        _isRecording = false;
        btnStartStop.Text = "Start Recording";
        lblStatus.Text = "Recording saved.";
    }

    private void Form_FormClosing(object sender, FormClosingEventArgs e)
    {
        if (_isRecording)
        {
            StopRecordingAsync().Wait();
        }

        VisioForgeX.DestroySDK();
    }
}
```

### Advanced Audio Capture Settings

The VideoCaptureCoreX class supports various audio capture configurations for different recording scenarios:

```csharp
// Configure high-quality WMA recording
var wmaOutput = new WMAOutput("high_quality.wma");
wmaOutput.Audio.Bitrate = 320;       // Maximum quality bitrate
wmaOutput.Audio.SampleRate = 48000;  // Professional sample rate
wmaOutput.Audio.Channels = 2;        // Stereo

// Configure voice recording (smaller file size)
var voiceOutput = new WMAOutput("voice_memo.wma");
voiceOutput.Audio.Bitrate = 128;     // Good for voice
voiceOutput.Audio.SampleRate = 44100; // Standard sample rate
voiceOutput.Audio.Channels = 1;       // Mono is sufficient for voice

// Check if WMA encoder is available on the system
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is not available on this system.");
    // Consider falling back to MP3 or other format
}
```

## Editing WMA Files Using VideoEditCoreX

The VideoEditCoreX class provides powerful capabilities for editing WMA files and converting audio video content to Windows Media format. This section demonstrates how to edit dotnet WMA files with trimming, merging, and format conversion.

### Core Components for WMA Editing

1. **VideoEditCoreX**: Main engine class for managing audio and video editing operations.
2. **AudioFileSource**: Class for adding audio file sources to the editing timeline.
3. **WMAOutput**: Output format configuration for Windows Media Audio export.
4. **Audio_Effects**: Collection for applying audio effects during editing.

### Basic WMA File Editing

Here's how to edit WMA files using the VideoEditCoreX class:

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaEditor
{
    private VideoEditCoreX _videoEdit;

    // Call this method once during application startup or form load
    public async Task InitializeAsync()
    {
        // Initialize the VisioForge SDK
        await VisioForgeX.InitSDKAsync();
    }

    public async Task EditWmaFileAsync(
        string inputPath, 
        string outputPath,
        TimeSpan? startTime = null,
        TimeSpan? endTime = null)
    {
        // Create VideoEditCoreX instance
        _videoEdit = new VideoEditCoreX();

        // Set up event handlers
        _videoEdit.OnProgress += VideoEdit_OnProgress;
        _videoEdit.OnStop += VideoEdit_OnStop;
        _videoEdit.OnError += VideoEdit_OnError;

        // Add input WMA file with optional trimming
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,  // Start time for trimming (null for beginning)
            endTime);   // End time for trimming (null for end)

        _videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configure WMA output format
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        _videoEdit.Output_Format = wmaOutput;

        // Start the editing process
        _videoEdit.Start();

        Console.WriteLine("Editing in progress...");
    }

    private void VideoEdit_OnProgress(object sender, ProgressEventArgs e)
    {
        Console.WriteLine($"Progress: {e.Progress}%");
    }

    private void VideoEdit_OnStop(object sender, StopEventArgs e)
    {
        if (e.Successful)
        {
            Console.WriteLine("Editing completed successfully!");
        }
        else
        {
            Console.WriteLine("Editing stopped with errors.");
        }

        // Cleanup
        _videoEdit?.Dispose();
        _videoEdit = null;
    }

    private void VideoEdit_OnError(object sender, ErrorsEventArgs e)
    {
        Console.WriteLine($"Error: {e.Message}");
    }
}
```

### Merging Multiple WMA Files

The VideoEditCoreX class allows you to merge multiple audio files into a single WMA output:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.Types.Events;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.VideoEdit;
using VisioForge.Core.VideoEditX;

public class WmaMerger
{
    // Call this method once during application startup or form load
    public async Task InitializeAsync()
    {
        // Initialize SDK
        await VisioForgeX.InitSDKAsync();
    }

    public async Task MergeWmaFilesAsync(
        List<string> inputFiles, 
        string outputPath)
    {
        var videoEdit = new VideoEditCoreX();

        try
        {
            // Set up progress reporting
            videoEdit.OnProgress += (s, e) => 
                Console.WriteLine($"Merging progress: {e.Progress}%");

            videoEdit.OnError += (s, e) => 
                Console.WriteLine($"Error: {e.Message}");

            // Add each input file sequentially
            foreach (var inputFile in inputFiles)
            {
                var audioFile = new AudioFileSource(inputFile);
                
                // Adding with null insertTime appends to end of timeline
                videoEdit.Input_AddAudioFile(audioFile, insertTime: null);
                
                Console.WriteLine($"Added: {inputFile}");
            }

            // Configure output format
            var wmaOutput = new WMAOutput(outputPath);
            wmaOutput.Audio.Bitrate = 192;
            wmaOutput.Audio.SampleRate = 48000;
            wmaOutput.Audio.Channels = 2;

            videoEdit.Output_Format = wmaOutput;

            // Create completion signal
            var completionSource = new TaskCompletionSource<bool>();
            videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

            // Start merging
            videoEdit.Start();

            // Wait for completion
            bool success = await completionSource.Task;

            if (success)
            {
                Console.WriteLine($"Files merged successfully to: {outputPath}");
            }
            else
            {
                Console.WriteLine("Merge operation failed.");
            }
        }
        finally
        {
            videoEdit.Dispose();
            VisioForgeX.DestroySDK();
        }
    }
}
```

### Trimming WMA Files

Extract a specific portion of a WMA file:

```csharp
// Note: Call VisioForgeX.InitSDKAsync() once during application startup or form load
public async Task TrimWmaFileAsync(
    string inputPath,
    string outputPath,
    TimeSpan startTime,
    TimeSpan endTime)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Add input file with specific start and end times
        var audioFile = new AudioFileSource(
            inputPath,
            startTime,   // e.g., TimeSpan.FromSeconds(10)
            endTime);    // e.g., TimeSpan.FromSeconds(60)

        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configure WMA output
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        // Create completion signal
        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        // Start trimming
        videoEdit.Start();

        // Wait for completion
        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? "Trim completed successfully!" 
            : "Trim operation failed.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Converting Video Files to WMA Audio

Extract audio from video files and save as WMA:

```csharp
// Note: Call VisioForgeX.InitSDKAsync() once during application startup or form load
public async Task ExtractAudioToWmaAsync(
    string videoInputPath,
    string wmaOutputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Add video file - audio will be extracted automatically
        var audioFile = new AudioFileSource(videoInputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Configure WMA output for audio extraction
        var wmaOutput = new WMAOutput(wmaOutputPath);
        wmaOutput.Audio.Bitrate = 256;       // Higher quality for music
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnProgress += (s, e) => 
            Console.WriteLine($"Extraction progress: {e.Progress}%");
        videoEdit.OnStop += (s, e) => 
            completionSource.SetResult(e.Successful);

        videoEdit.Start();

        bool success = await completionSource.Task;

        Console.WriteLine(success 
            ? $"Audio extracted to: {wmaOutputPath}" 
            : "Audio extraction failed.");
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

### Applying Audio Effects During WMA Editing

The VideoEditCoreX class supports various audio effects that can be applied during the editing process:

```csharp
using VisioForge.Core.Types.X.AudioEffects;

// Note: Call VisioForgeX.InitSDKAsync() once during application startup or form load
public async Task EditWmaWithEffectsAsync(
    string inputPath,
    string outputPath)
{
    var videoEdit = new VideoEditCoreX();

    try
    {
        // Add input file
        var audioFile = new AudioFileSource(inputPath);
        videoEdit.Input_AddAudioFile(audioFile, insertTime: null);

        // Apply audio effects

        // Volume amplification effect
        var amplifyEffect = new AmplifyAudioEffect(1.5); // 150% volume
        videoEdit.Audio_Effects.Add(amplifyEffect);

        // 10-band equalizer effect
        var eqLevels = new double[] 
        { 
            3.0,   // 60 Hz
            2.0,   // 170 Hz
            1.0,   // 310 Hz
            0.0,   // 600 Hz
            0.0,   // 1 kHz
            0.0,   // 3 kHz
            1.0,   // 6 kHz
            2.0,   // 12 kHz
            2.0,   // 14 kHz
            3.0    // 16 kHz
        };
        var equalizerEffect = new Equalizer10AudioEffect(eqLevels);
        videoEdit.Audio_Effects.Add(equalizerEffect);

        // Configure WMA output
        var wmaOutput = new WMAOutput(outputPath);
        wmaOutput.Audio.Bitrate = 192;
        wmaOutput.Audio.SampleRate = 48000;
        wmaOutput.Audio.Channels = 2;

        videoEdit.Output_Format = wmaOutput;

        var completionSource = new TaskCompletionSource<bool>();
        videoEdit.OnStop += (s, e) => completionSource.SetResult(e.Successful);

        videoEdit.Start();

        await completionSource.Task;
    }
    finally
    {
        videoEdit.Dispose();
    }
}
```

## WMA Encoder Configuration

The WMAEncoderSettings class provides various configuration options for the Windows Media Audio encoder:

### Available Settings

```csharp
var wmaSettings = new WMAEncoderSettings
{
    // Bitrate in Kbps - supported values: 128, 192, 256, 320
    Bitrate = 192,
    
    // Sample rate in Hz - supported values: 44100, 48000
    SampleRate = 48000,
    
    // Number of channels - supported values: 1 (mono), 2 (stereo)
    Channels = 2
};

// Query supported configurations
int[] supportedBitrates = wmaSettings.GetSupportedBitrates();
// Returns: [128, 192, 256, 320]

int[] supportedSampleRates = wmaSettings.GetSupportedSampleRates();
// Returns: [44100, 48000]

int[] supportedChannels = wmaSettings.GetSupportedChannelCounts();
// Returns: [1, 2]
```

### Quality Presets

Here are recommended presets for different use cases:

```csharp
// High-quality music recording
var musicPreset = new WMAEncoderSettings
{
    Bitrate = 320,
    SampleRate = 48000,
    Channels = 2
};

// Voice recording / podcasts
var voicePreset = new WMAEncoderSettings
{
    Bitrate = 128,
    SampleRate = 44100,
    Channels = 1
};

// Balanced quality and file size
var balancedPreset = new WMAEncoderSettings
{
    Bitrate = 192,
    SampleRate = 48000,
    Channels = 2
};
```

## Working with Audio Packets and Buffers

For advanced scenarios, you may need to work directly with audio data packets. The SDK provides mechanisms for accessing raw audio data during capturing and processing.

### Processing Audio Packets

```csharp
// During capture, you can monitor audio levels and packets
_videoCapture.OnAudioVUMeter += (sender, args) =>
{
    // Get audio levels for VU meter display
    double leftChannel = args.Left;
    double rightChannel = args.Right;
    
    // Update UI with audio levels
    UpdateVUMeter(leftChannel, rightChannel);
};
```

## Error Handling and Best Practices

### Proper Resource Management

Always ensure proper cleanup of SDK resources:

```csharp
public class WmaProcessor : IDisposable
{
    private VideoCaptureCoreX _videoCapture;
    private bool _disposed = false;

    public async Task InitializeAsync()
    {
        await VisioForgeX.InitSDKAsync();
        _videoCapture = new VideoCaptureCoreX();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _videoCapture?.Dispose();
                VisioForgeX.DestroySDK();
            }
            _disposed = true;
        }
    }
}
```

### Error Handling

Implement comprehensive error handling for production applications:

```csharp
try
{
    await _videoCapture.StartAsync();
}
catch (Exception ex)
{
    // Log the error
    Console.WriteLine($"Failed to start recording: {ex.Message}");
    
    // Clean up resources
    await _videoCapture.DisposeAsync();
    
    // Notify user or retry
    throw;
}
```

### Checking Encoder Availability

Before creating WMA files, verify the encoder is available:

```csharp
if (!WMAEncoderSettings.IsAvailable())
{
    Console.WriteLine("WMA encoder is not available.");
    Console.WriteLine("Falling back to MP3 format...");
    
    // Use MP3 as alternative
    var mp3Output = new MP3Output("output.mp3");
    // ... continue with MP3 encoding
}
```

## Cross-Platform Considerations

The WMA format and Windows Media components are primarily designed for Windows systems. When developing cross-platform applications:

- **Windows**: Full WMA support with all encoding options
- **Linux/macOS**: WMA encoding may require additional GStreamer plugins
- **Mobile (Android/iOS)**: Consider using more universal formats like AAC or MP3

For cross-platform audio recording, consider these alternatives:

```csharp
// Check platform and select appropriate format
string outputFormat = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
    ? "output.wma"
    : "output.m4a";  // AAC container for non-Windows

if (outputFormat.EndsWith(".wma"))
{
    var wmaOutput = new WMAOutput(outputFormat);
    _videoCapture.Outputs_Add(wmaOutput, true);
}
else
{
    var m4aOutput = new M4AOutput(outputFormat);
    _videoCapture.Outputs_Add(m4aOutput, true);
}
```

## Sample Applications and Resources

Explore complete sample applications demonstrating WMA recording and editing:

- [Video Capture SDK X Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)
- [Video Edit SDK X Samples](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK%20X)

### Additional Documentation

- [Windows Media Audio Encoder Guide](../audio-encoders/wma.md)
- [Windows Media Video Output Guide](../output-formats/wmv.md)
- [Audio Sample Grabber](../audio-effects/audio-sample-grabber.md)
- [API Documentation](https://api.visioforge.org/dotnet/api/index.html)

## Conclusion

This comprehensive guide has demonstrated how to record and edit WMA files using the VisioForge .NET SDKs. You've learned how to record .NET WMA audio, capture dotnet WMA content, and create professional audio applications. The VideoCaptureCoreX class provides powerful capabilities for capturing audio from microphones and other devices, while the VideoEditCoreX class offers flexible options for editing and converting audio content to Windows Media format.

Key takeaways:

- **Record WMA files**: Use VideoCaptureCoreX with WMAOutput for capturing audio from system devices and reserve optimal quality settings for your output
- **Edit WMA files**: Use VideoEditCoreX for trimming, merging, and applying effects to audio records
- **Configuration**: The WMAEncoderSettings class allows fine-tuning of bitrate, sample rate, and channels
- **Cross-platform**: Consider platform-specific requirements when working with Windows Media formats
- **Windows application support**: Create WinForms, WPF, and console applications with ease

Both classes integrate seamlessly with WinForms, WPF, and console applications, making it easy to create powerful audio recording and editing solutions in your .NET applications. The data processing capabilities and library features allow you to build professional-grade audio editor applications.
