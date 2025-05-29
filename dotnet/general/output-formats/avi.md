---
title: AVI File Output Guide for .NET SDK Development
description: Learn how to implement AVI file output in .NET applications with step-by-step examples. Covers video and audio encoding options, hardware acceleration, cross-platform support, and best practices for developers working with multimedia container formats.
sidebar_label: AVI

---

# AVI File Output in VisioForge .NET SDKs

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

AVI (Audio Video Interleave) is a Microsoft-developed multimedia container format that stores both audio and video data in a single file with synchronized playback. It supports both compressed and uncompressed data, offering flexibility while sometimes resulting in larger file sizes.

## Technical Overview of AVI Format

AVI files use a RIFF (Resource Interchange File Format) structure to organize data. This format divides content into chunks, with each chunk containing either audio or video frames. Key technical aspects include:

- Container format supporting multiple audio and video codecs
- Interleaved audio and video data for synchronized playback
- Maximum file size of 4GB in standard AVI (extended to 16EB in OpenDML AVI)
- Support for multiple audio tracks and subtitles
- Widely supported across platforms and media players

Despite newer container formats like MP4 and MKV offering more features, AVI remains valuable for certain workflows due to its simplicity and compatibility with legacy systems.

## Cross-Platform AVI Implementation

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

The [AVIOutput](https://api.visioforge.org/dotnet/api/VisioForge.Core.Types.Output.AVIOutput.html) class provides a robust way to configure and generate AVI files with various encoding options.

### Setting Up AVI Output

Create an `AVIOutput` instance by specifying a target filename:

```csharp
var aviOutput = new AVIOutput("output_video.avi");
```

This constructor automatically initializes default encoders:

- Video: OpenH264 encoder
- Audio: MP3 encoder

### Video Encoder Options

Configure video encoding through the `Video` property with several available encoders:

#### Standard Encoder

```csharp
// Open-source H.264 encoder for general use
aviOutput.Video = new OpenH264EncoderSettings();
```

#### Hardware-Accelerated Encoders

```csharp
// NVIDIA GPU acceleration
aviOutput.Video = new NVENCH264EncoderSettings();  // H.264
aviOutput.Video = new NVENCHEVCEncoderSettings(); // HEVC

// Intel Quick Sync acceleration
aviOutput.Video = new QSVH264EncoderSettings();   // H.264
aviOutput.Video = new QSVHEVCEncoderSettings();   // HEVC

// AMD GPU acceleration
aviOutput.Video = new AMFH264EncoderSettings();   // H.264
aviOutput.Video = new AMFHEVCEncoderSettings();   // HEVC
```

#### Special Purpose Encoder

```csharp
// Motion JPEG for high-quality frame-by-frame encoding
aviOutput.Video = new MJPEGEncoderSettings();
```

### Audio Encoder Options

The `Audio` property lets you configure audio encoding settings:

```csharp
// Standard MP3 encoding
aviOutput.Audio = new MP3EncoderSettings();

// AAC encoding options
aviOutput.Audio = new VOAACEncoderSettings();
aviOutput.Audio = new AVENCAACEncoderSettings();
aviOutput.Audio = new MFAACEncoderSettings(); // Windows only
```

### Integration with SDK Components

#### Video Capture SDK

```csharp
var core = new VideoCaptureCoreX();
core.Outputs_Add(aviOutput, true);
```

#### Video Edit SDK

```csharp
var core = new VideoEditCoreX();
core.Output_Format = aviOutput;
```

#### Media Blocks SDK

```csharp
var aac = new VOAACEncoderSettings();
var h264 = new OpenH264EncoderSettings();
var aviSinkSettings = new AVISinkSettings("output.avi");
var aviOutput = new AVIOutputBlock(aviSinkSettings, h264, aac);
```

### File Management

You can get or change the output filename after initialization:

```csharp
// Get current filename
string currentFile = aviOutput.GetFilename();

// Set new filename
aviOutput.SetFilename("new_output.avi");
```

### Complete Example

Here's a full example showing how to configure AVI output with hardware acceleration:

```csharp
// Create AVI output with specified filename
var aviOutput = new AVIOutput("high_quality_output.avi");

// Configure hardware-accelerated NVIDIA H.264 encoding
aviOutput.Video = new NVENCH264EncoderSettings();

// Configure AAC audio encoding
aviOutput.Audio = new VOAACEncoderSettings();
```

## Windows-Specific AVI Implementation

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

The Windows-only components provide additional options for AVI output configuration.

### Basic Setup

Create the AVIOutput object:

```csharp
var aviOutput = new AVIOutput();
```

### Configuration Methods

#### Method 1: Using Settings Dialog

```csharp
var aviSettingsDialog = new AVISettingsDialog(
  VideoCapture1.Video_Codecs.ToArray(),
  VideoCapture1.Audio_Codecs.ToArray());

aviSettingsDialog.ShowDialog(this);
aviSettingsDialog.SaveSettings(ref aviOutput);
```

#### Method 2: Programmatic Configuration

First, get available codecs:

```csharp
// Populate codec lists
foreach (string codec in VideoCapture1.Video_Codecs)
{
  cbVideoCodecs.Items.Add(codec);
}

foreach (string codec in VideoCapture1.Audio_Codecs)
{
  cbAudioCodecs.Items.Add(codec);
}
```

Then set video and audio settings:

```csharp
// Configure video
aviOutput.Video_Codec = cbVideoCodecs.Text;

// Configure audio
aviOutput.ACM.Name = cbAudioCodecs.Text;
aviOutput.ACM.Channels = 2;
aviOutput.ACM.BPS = 16;
aviOutput.ACM.SampleRate = 44100;
aviOutput.ACM.UseCompression = true;
```

### Implementation

Apply settings and start capture:

```csharp
// Set output format
VideoCapture1.Output_Format = aviOutput;

// Set capture mode
VideoCapture1.Mode = VideoCaptureMode.VideoCapture;

// Set output file path
VideoCapture1.Output_Filename = "output.avi";

// Start capture
await VideoCapture1.StartAsync();
```

## Best Practices for AVI Output

### Encoder Selection Guidelines

1. **General-Purpose Applications**
   - OpenH264 provides good compatibility and quality
   - Suitable for most standard development scenarios

2. **Performance-Critical Applications**
   - Use hardware-accelerated encoders (NVENC, QSV, AMF) when available
   - Offers significant performance advantages with minimal quality loss

3. **Quality-Focused Applications**
   - HEVC encoders provide better compression at similar quality
   - MJPEG for scenarios requiring frame-by-frame accuracy

### Audio Encoding Recommendations

- MP3: Good compatibility with reasonable quality
- AAC: Better quality-to-size ratio, preferred for newer applications
- Choose based on your target platform and quality requirements

### Platform Considerations

- Some encoders are platform-specific:
  - MF HEVC and MF AAC encoders are Windows-only
  - Hardware-accelerated encoders require appropriate GPU support

- Check encoder availability with `GetVideoEncoders()` and `GetAudioEncoders()` when developing cross-platform applications

### Error Handling Tips

- Always verify encoder availability before use
- Implement fallback encoders for platform-specific scenarios
- Check file write permissions before setting output paths

## Troubleshooting Common Issues

### Codec Not Found

If you encounter "Codec not found" errors:

```csharp
// Check if codec is available before using
if (!VideoCapture1.Video_Codecs.Contains("H264"))
{
    // Fall back to another codec or show error
    MessageBox.Show("H264 codec not available. Please install required codecs.");
    return;
}
```

### File Write Permission Issues

Handle permission-related errors:

```csharp
try
{
    // Test write permissions
    using (var fs = File.Create(outputPath, 1, FileOptions.DeleteOnClose)) { }
    
    // If successful, proceed with AVI output
    aviOutput.SetFilename(outputPath);
}
catch (UnauthorizedAccessException)
{
    // Handle permission error
    MessageBox.Show("Cannot write to the specified location. Please select another folder.");
}
```

### Memory Issues with Large Files

For handling large file recording:

```csharp
// Split recording into multiple files when size limit is reached
void SetupLargeFileRecording()
{
    var aviOutput = new AVIOutput("recording_part1.avi");
    
    // Set file size limit (3.5GB to stay under 4GB AVI limit)
    aviOutput.MaxFileSize = 3.5 * 1024 * 1024 * 1024;
    
    // Enable auto-split functionality
    aviOutput.AutoSplit = true;
    aviOutput.SplitNamingPattern = "recording_part{0}.avi";
    
    // Apply to Video Capture
    var core = new VideoCaptureCoreX();
    core.Outputs_Add(aviOutput, true);
}
```

## Required Dependencies

### Video Capture SDK .Net

- [x86 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/)
- [x64 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

### Video Edit SDK .Net

- [x86 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
- [x64 Redist](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Additional Resources

- [VisioForge API Documentation](https://api.visioforge.org/dotnet/)
- [Sample Projects Repository](https://github.com/visioforge/.Net-SDK-s-samples)
- [Support and Community Forums](https://support.visioforge.com/)
