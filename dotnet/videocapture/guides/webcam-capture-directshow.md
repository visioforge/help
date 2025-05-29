---
title: C# Webcam Capture - DirectShow .NET SDK Guide
description: Learn to build C# webcam capture apps with the Video Capture SDK .NET using DirectShow. This guide covers device enumeration, configuration, recording, and effects.
sidebar_label: Webcam Capture (DirectShow)
---

# C# Webcam Capture Using DirectShow .NET SDK

## Introduction

This guide demonstrates how to create a webcam capture application in C# using the Video Capture SDK .Net with Microsoft DirectShow technology. DirectShow is a powerful Windows media framework that enables applications to control and process audio and video data from various sources, including webcams and USB cameras.

The full source code of the sample application is available in the [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK/WPF/CSharp/Simple%20Video%20Capture).

## Prerequisites

- Visual Studio 2019 or later
- Windows 10 or 11 operating system
- Webcam or other video capture device (USB camera)
- Basic understanding of C# programming

## Installation

Check the main [installation guide](../../install/index.md) for detailed steps on how to install the DirectShow .NET SDK in your application.

## Key Components

### Core Classes

- `VideoCaptureCore` - Main class for capturing video from a source device
- `VideoCaptureSource` - Represents a video source device (webcam, camera)
- `AudioCaptureSource` - Represents an audio capture device (microphone)

## Implementation Guide

### 1. Initialize the Capture Engine

The first step in any DirectShow application is to create the core capture object:

```csharp
// Create a new instance with a video view
var captureCore = await VideoCaptureCore.CreateAsync(videoView as IVideoView);

// Register error handler for robust application development
captureCore.OnError += (sender, args) => {
    Console.WriteLine($"Error: {args.Message}");
};
```

### 2. Device Enumeration and Selection

#### Enumerate Available Devices

To access webcams and microphones, you need to enumerate available devices:

```csharp
// Get video sources
foreach (var item in captureCore.Video_CaptureDevices())
{
    Console.WriteLine($"Video device: {item.Name}");
    
    // List available formats for this device
    foreach (var format in item.VideoFormats)
    {
        Console.WriteLine($"  Format: {format}");
    }
}

// Get audio sources
foreach (var item in captureCore.Audio_CaptureDevices())
{
    Console.WriteLine($"Audio device: {item.Name}");
    
    foreach (var format in item.Formats)
    {
        Console.WriteLine($"  Format: {format}");
    }
}
```

#### Configure Video and Audio Capture Sources

This sample code shows how to filter and select specific video and audio formats required by your application:

```csharp
// Find and configure video source with specific format
var videoDevices = captureCore.Video_CaptureDevices();
VideoCaptureSource selectedVideoDevice = null;
string targetVideoFormat = null;

// Find a device with 1280x720 MJPG support
foreach (var item in videoDevices)
{
    Console.WriteLine($"Checking video device: {item.Name}");
    
    foreach (var format in item.VideoFormats)
    {
        Console.WriteLine($"  Format: {format}");
        
        // Look for 1280x720 MJPG format
        if (format.Contains("1280x720") && format.Contains("MJPG"))
        {
            selectedVideoDevice = item;
            targetVideoFormat = format;
            Console.WriteLine($"  SELECTED: {format}");
            break;
        }
    }
    
    if (selectedVideoDevice != null)
        break;
}

// Find and configure audio source with specific format
var audioDevices = captureCore.Audio_CaptureDevices();
AudioCaptureSource selectedAudioDevice = null;
string targetAudioFormat = null;

// Find a device with 44100 Hz, 16 bit, 2 channels support
foreach (var item in audioDevices)
{
    Console.WriteLine($"Checking audio device: {item.Name}");
    
    foreach (var format in item.Formats)
    {
        Console.WriteLine($"  Format: {format}");
        
        // Look for 44100 Hz, 16 bit, 2 channels format
        if (format.Contains("44100 Hz") && format.Contains("16 Bit") && format.Contains("2 Channel"))
        {
            selectedAudioDevice = item;
            targetAudioFormat = format;
            Console.WriteLine($"  SELECTED: {format}");
            break;
        }
    }
    
    if (selectedAudioDevice != null)
        break;
}

// Set up video capture with the selected device and format
if (selectedVideoDevice != null)
{
    captureCore.Video_CaptureDevice = new VideoCaptureSource(selectedVideoDevice.Name);
    
    if (targetVideoFormat != null)
    {
        captureCore.Video_CaptureDevice.Format = targetVideoFormat;
        captureCore.Video_CaptureDevice.Format_UseBest = false; // Using specific format
    }
    else
    {
        captureCore.Video_CaptureDevice.Format_UseBest = true; // Use best available format
    }
    
    // Configure video frame rate if needed
    captureCore.Video_CaptureDevice.FrameRate = new VideoFrameRate(30.0);
}
else
{
    Console.WriteLine("No video device with 1280x720 MJPG found.");
    
    // Fallback to first available device with best format
    if (videoDevices.Count() > 0)
    {
        captureCore.Video_CaptureDevice = new VideoCaptureSource(videoDevices[0].Name);
        captureCore.Video_CaptureDevice.Format_UseBest = true;
    }
}

// Select audio capture device. Specify the device name and format
if (selectedAudioDevice != null)
{
    captureCore.Audio_CaptureDevice = new AudioCaptureSource(selectedAudioDevice.Name);
    
    if (targetAudioFormat != null)
    {
        captureCore.Audio_CaptureDevice.Format = targetAudioFormat;
        captureCore.Audio_CaptureDevice.Format_UseBest = false; // Using specific format
    }
    else
    {
        captureCore.Audio_CaptureDevice.Format_UseBest = true; // Set true to detect the best format automatically
    }
}
else
{
    Console.WriteLine("No audio device with 44100 Hz, 16 bit, 2 channels found.");
    
    // Fallback to the first available device
    if (audioDevices.Count() > 0)
    {
        captureCore.Audio_CaptureDevice = new AudioCaptureSource(audioDevices[0].Name);
        captureCore.Audio_CaptureDevice.Format_UseBest = true;
    }
}

// Open device settings dialogs if needed
// captureCore.Video_CaptureDevice_SettingsDialog_Show(IntPtr.Zero, captureCore.Video_CaptureDevice.Name);
// captureCore.Audio_CaptureDevice_SettingsDialog_Show(IntPtr.Zero, captureCore.Audio_CaptureDevice.Name);
```

### 3. Configuring Audio Settings

Control audio recording and playback with these settings:

```csharp
// Enable audio recording
captureCore.Audio_RecordAudio = true;

// Enable audio playback during capture
captureCore.Audio_PlayAudio = true;

// Configure audio output device
captureCore.Audio_OutputDevice = "Default DirectSound Device";

// Set audio volume (0-100)
captureCore.Audio_OutputDevice_Volume_Set(75);

// Set audio balance (-100 to 100)
captureCore.Audio_OutputDevice_Balance_Set(0);
```

### 4. Output Configuration

#### MP4 Video Output

To save the captured video to an MP4 file, use the following code:

```csharp
// Set capture mode
captureCore.Mode = VideoCaptureMode.VideoCapture;
captureCore.Output_Filename = "output.mp4";

// Configure MP4 output (CPU/QuickSync)
var mp4Output = new MP4Output
{
    MP4Mode = MP4Mode.CPU_QSV,
    AudioFormat = MP4AudioEncoder.AAC
};

// Configure AAC audio settings
mp4Output.Audio_AAC = new M4AOutput
{
    Bitrate = 128,
    Object = AACObject.Low,
    Output = AACOutput.RAW,
    Version = AACVersion.MPEG4
};

// Or use MP3 for audio (via LAME encoder)
var mp3Output = new MP3Output();
mp4Output.Audio_LAME = mp3Output;
mp4Output.AudioFormat = MP4AudioEncoder.MP3_LAME;

// Configure H.264 video settings
mp4Output.Video = new MP4OutputH264Settings
{
    // Basic settings
    Bitrate = 3500,
    MaxBitrate = 6000,
    MinBitrate = 1500,
    BitrateAuto = false,
    
    // Profile and level
    Profile = H264Profile.ProfileMain,
    Level = H264Level.Level41,
    
    // Frame structure
    IDR_Period = 15,         // I-frame interval in frames
    P_Period = 3,            // Distance between I- or P-key frames (if 1, no B-frames)
    
    // Encoding settings
    RateControl = H264RateControl.VBR,
    MBEncoding = H264MBEncoding.CAVLC,  // or CABAC for better compression
    TargetUsage = H264TargetUsage.Balanced,
    Preset = H264Peset.Balanced,
    
    // Other options
    Deblocking = true,
    GOP = true,
    PictureType = H264PictureType.Auto
};

// Assign output format
captureCore.Output_Format = mp4Output;
```

#### Hardware-Accelerated Encoding

For hardware acceleration using NVIDIA GPU:

```csharp
// Use hardware acceleration with NVENC
var mp4HWOutput = new MP4HWOutput
{
    // H264 settings
    Video = new MFVideoEncoderSettings
    {
        Codec = MFVideoEncoder.NVENC_H264,  // or MS_H264, QSV_H264, AMD_H264
        AvgBitrate = 3500,
        MaxBitrate = 6000,
        Profile = MFH264Profile.Main,
        Level = MFH264Level.Level41,
        RateControl = MFCommonRateControlMode.CBR,
        CABAC = true,
        QualityVsSpeed = 85,
        MaxKeyFrameSpacing = 125
    },
    
    // AAC settings
    Audio = new M4AOutput
    {
        Bitrate = 128,
        Object = AACObject.Low
    }
};

captureCore.Output_Format = mp4HWOutput;
```

#### Custom Output Formats

For more advanced output configurations:

- Check the [Custom output formats](../../general/output-formats/custom.md) guide for more information.

### 5. Video Processing and Effects

DirectShow allows for video processing through filters. Here's how to add various effects:

```csharp
// Enable video effects
captureCore.Video_Effects_Enabled = true;
captureCore.Video_Effects_MergeImageLogos = true;
captureCore.Video_Effects_MergeTextLogos = true;

// Add grayscale effect
var grayscale = new VideoEffectGrayscale(true);
captureCore.Video_Effects_Add(grayscale);

// Add contrast effect (0-200)
var contrast = new VideoEffectContrast(true, 50);
captureCore.Video_Effects_Add(contrast);

// Add brightness effect (0-200)
var lightness = new VideoEffectLightness(true, 50);
captureCore.Video_Effects_Add(lightness);

// Add saturation effect (0-255)
var saturation = new VideoEffectSaturation(120);
captureCore.Video_Effects_Add(saturation);

// Add darkness effect (0-200)
var darkness = new VideoEffectDarkness(true, 20);
captureCore.Video_Effects_Add(darkness);

// Add invert colors effect
var invert = new VideoEffectInvert(true);
captureCore.Video_Effects_Add(invert);

// Add flip effects
var flipHorizontal = new VideoEffectFlipHorizontal(true);
captureCore.Video_Effects_Add(flipHorizontal);

var flipVertical = new VideoEffectFlipVertical(true);
captureCore.Video_Effects_Add(flipVertical);
```

#### Adding Overlays

```csharp
// Add image logo
var imageLogo = new VideoEffectImageLogo(true, "MyLogo");
imageLogo.Filename = "logo.png"; // Path to your logo image
// Configure logo properties here
captureCore.Video_Effects_Add(imageLogo);

// Add text logo
var textLogo = new VideoEffectTextLogo(true, "MyTextLogo");
textLogo.Text = "My Text Logo";
// Configure text properties here
captureCore.Video_Effects_Add(textLogo);

// Add scrolling text
var scrollingText = new VideoEffectScrollingTextLogo(true);
// Configure text properties here
captureCore.Video_Effects_Add(scrollingText);
```

#### Custom DirectShow Filters

DirectShow's power comes from its filter-based architecture. Here's how to add filter for video processing:

```csharp
// List all available DirectShow filters
var filters = captureCore.DirectShow_Filters();

// Use a specific filter
var filter = filters.FirstOrDefault(f => f.Name == "MyFilter");

captureCore.Video_Filters_Add(new CustomProcessingFilter(filter));
```

### 6. Capture Control

Control the media flow with these simple methods:

```csharp
// Set debugging options if needed
captureCore.Debug_Mode = true;
captureCore.Debug_Dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "VisioForge");

// Start capture
await captureCore.StartAsync();

// Pause capture
await captureCore.PauseAsync();

// Resume capture
await captureCore.ResumeAsync();

// Stop capture
await captureCore.StopAsync();

// Dispose when done
captureCore.Dispose();
```

### 7. Taking Snapshots

Capture still images from your video stream:

```csharp
// Take a snapshot and save as JPEG with quality 85
await captureCore.Frame_SaveAsync("snapshot.jpg", ImageFormat.Jpeg, 85);

// Other supported formats
await captureCore.Frame_SaveAsync("snapshot.png", ImageFormat.Png, 0);
await captureCore.Frame_SaveAsync("snapshot.bmp", ImageFormat.Bmp, 0);
await captureCore.Frame_SaveAsync("snapshot.gif", ImageFormat.Gif, 0);
await captureCore.Frame_SaveAsync("snapshot.tiff", ImageFormat.Tiff, 0);
```

## Advanced Topics

For more advanced DirectShow .NET SDK functionality, explore:

- Stream publishing to network
- Multi-source recording
- Custom filter graphs
- Event handling and notifications
- Timeline editing

## Troubleshooting

Common issues when working with DirectShow in C#:

- Device busy or inaccessible: Ensure no other application is using the webcam
- Format compatibility: Not all formats work with all filters and output configurations
- Memory leaks: Always dispose of DirectShow objects properly
- Driver issues: Make sure webcam/camera drivers are up to date

## Conclusion

This tutorial demonstrates the core functionality needed to create a C# video capture application using the DirectShow .NET SDK. With this framework, you can build applications that capture, process, and save video and audio from various sources like webcams and USB cameras. The DirectShow architecture provides access to a wide range of media processing capabilities through its filter-based design, making it a powerful choice for Windows media applications.
