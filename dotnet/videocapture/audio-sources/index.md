---
title: Implementing Audio Capture in .NET Applications
description: Learn how to integrate multiple audio sources in your .NET applications including capture devices, system audio loopback, IP cameras, and Decklink devices with complete code examples and implementation steps.
sidebar_label: Audio Sources
order: 15

---

# Working with Audio Sources in .NET

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoCaptureCore"]

## Available Audio Sources

When building media applications, you'll need to capture audio from various sources. This guide covers how to implement audio capture from multiple input types using our SDK:

* Audio capture devices (microphones, line-in)
* System audio (speakers/headphones via loopback)
* Network streams (IP cameras)
* Professional Decklink devices

Each source type requires different initialization methods and has unique capabilities. Let's explore how to work with each one.

## Implementing Audio Capture Devices

Audio capture devices include microphones, webcams with built-in mics, and other input hardware connected to your system. Working with these devices involves three key steps:

1. Enumerating available devices
2. Selecting appropriate audio formats
3. Configuring the selected device as your audio source

### Enumerating Available Audio Devices

First, you need to detect all audio input devices connected to the system:

+++ VideoCaptureCoreX

```csharp
var audioSources = await core.Audio_SourcesAsync();
foreach (var source in audioSources)
{
    // add to some combobox
    cbAudioInputDevice.Items.Add(source.DisplayName);
}
```

+++ VideoCaptureCore

```csharp
foreach (var device in core.Audio_CaptureDevices())
{
    // add to some combobox
    cbAudioInputDevice.Items.Add(device.Name);
}
```

+++

This code retrieves all audio input devices and can display them in a dropdown for user selection. The async approach in VideoCaptureCoreX provides better performance for systems with many connected devices.

### Discovering Supported Audio Formats

Once you've identified available devices, you'll need to determine which audio formats each device supports:

+++ VideoCaptureCoreX

```csharp
// find the device by name
var deviceItem = (await VideoCapture1.Audio_SourcesAsync()).FirstOrDefault(device => device.DisplayName == "Some device name");
if (deviceItem == null)
{
    return;
}

// enumerate formats
foreach (var format in deviceItem.Formats)
{
    cbAudioInputFormat.Items.Add(format.Name);
}
```

+++ VideoCaptureCore

```csharp
// find the device by name
var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");

// enumerate formats
foreach (var format in deviceItem.Formats)
{
    cbAudioInputFormat.Items.Add(format);
}
```

+++

Different audio devices support various formats with different bit depths, sample rates, and channel configurations. Enumerating these options allows you to select the most appropriate format for your application's needs.

### Setting Up the Audio Capture Device

After selecting a device and format, configure it as your audio source:

+++ VideoCaptureCoreX

```csharp
// find the device by name
var deviceItem = (await VideoCapture1.Audio_CaptureDevices()).FirstOrDefault(device => device.DisplayName == "Device name");
if (deviceItem == null)
{
    return;
}

// set the first format
AudioCaptureDeviceFormat format = formats[0].ToFormat();    

// create audio source settings
IVideoCaptureBaseAudioSourceSettings audioSource = deviceItem.CreateSourceSettingsVC(format);    

// set audio source
VideoCapture1.Audio_Source = audioSource;                   
```

+++ VideoCaptureCore

```csharp
// find the device by name
var deviceItem = VideoCapture1.Audio_CaptureDevices().FirstOrDefault(device => device.Name == "Some device name");
VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource(deviceItem.Name);
VideoCapture1.Audio_CaptureDevice.Format = deviceItem.Formats[0].ToString(); // set the first format
```

+++

This code configures your application to capture audio from the selected device using the specified format. The VideoCaptureCoreX API provides more granular control over format selection and device configuration.

## Capturing System Audio via Loopback

Audio loopback allows you to record any sound playing through your system's speakers or headphones. This is particularly useful for:

* Screen recording with audio
* Capturing application sounds
* Recording audio from web conferences or streaming services

Here's how to implement it:

+++ VideoCaptureCoreX

First, enumerate available loopback devices:

```csharp
// Enumerate audio loopback devices
var audioSinks = await DeviceEnumerator.Shared.AudioOutputsAsync();
foreach (var sink in audioSinks)
{   
    // Filter by WASAPI2 API
    if (sink.API == AudioOutputDeviceAPI.WASAPI2)
    {
        // Add to some combobox
        cbAudioLoopbackDevice.Items.Add(sink.Name);
    }
}
```

Next, create source settings for your selected output device:

```csharp
// audio input
var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2)).FirstOrDefault(device => device.Name == "Output device name");
if (deviceItem == null)
{
    return;
}

IVideoCaptureBaseAudioSourceSettings audioSource = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);

VideoCapture1.Audio_Source = audioSource;
```

The WASAPI2 API provides the most reliable loopback functionality on Windows systems, with lower latency and better performance compared to other options.

+++ VideoCaptureCore

In VideoCaptureCore, loopback functionality is simplified with a dedicated virtual device:

```cs
VideoCapture1.Audio_CaptureDevice = new AudioCaptureSource("VisioForge What You Hear Source");
VideoCapture1.Audio_CaptureDevice.Format_UseBest = true;
```

This approach automatically selects the best available format for the loopback source, making implementation straightforward.

+++

## Working with Network Audio Sources

For IP cameras and other network streams, audio capture is typically handled as part of the overall stream connection. The exact implementation depends on the network protocol being used (RTSP, HLS, etc.) and the specific device capabilities.

When connecting to network sources, you'll generally:

1. Establish a connection to the IP address and port
2. Authenticate if required
3. Configure audio parameters as supported by the device

Audio from network sources may come in various formats including AAC, MP3, or raw PCM data depending on the device. Our SDK handles the necessary format conversion and synchronization with video streams automatically.

## Implementing Decklink Audio Capture

Decklink devices provide professional-grade audio capture capabilities with features like:

* High sample rates (up to 192kHz)
* Multiple channel configurations
* Synchronized audio/video capture
* Embedded audio in SDI signals

When working with Decklink hardware, audio settings are typically configured as part of the overall device setup. The SDK provides specialized classes and methods for working with these professional devices.

## Best Practices for Audio Capture

To ensure high-quality audio capture in your applications:

1. **Sample rate selection**: Choose appropriate sample rates based on your target output. For most applications, 44.1kHz or 48kHz is sufficient.

2. **Buffer management**: Configure appropriate buffer sizes to balance between latency and stability. Smaller buffers reduce latency but may cause audio dropouts.

3. **Format handling**: Support multiple formats to accommodate various devices. Always have fallback options when specific formats aren't available.

4. **Level monitoring**: Implement audio level monitoring to detect silence or clipping, allowing your application to respond appropriately.

5. **Error handling**: Implement robust error handling for device disconnections or format negotiation failures.

## Conclusion

Implementing audio capture capabilities in your .NET application involves selecting the appropriate source, configuring formats, and managing the audio stream. Whether you're capturing from microphones, system audio, or network sources, our SDK provides the tools needed to build sophisticated audio applications.

By following the code examples and implementation patterns outlined in this guide, you'll be able to integrate powerful audio capture functionality into your projects efficiently.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
