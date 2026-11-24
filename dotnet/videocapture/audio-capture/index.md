---
title: Capture System Audio and Record Microphone in C# .NET
description: Record microphone audio and capture system sound (speaker/loopback) in C# with VisioForge SDK. Complete code examples for audio-only recording to MP3, M4A, WAV.
sidebar_label: Audio Capture
order: 10
tags:
  - Video Capture SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - DeviceEnumerator
  - VideoCaptureCoreX
  - LoopbackAudioCaptureDeviceSourceSettings
  - SystemAudioSourceBlock
  - MediaBlocksPipeline

video:
  id: dXehQhz7xa0
  upload_date: 2026-11-24T15:25:52Z
  duration: PT59S
---

# Audio Capture and System Sound Recording in C# .NET

[Video Capture SDK .NET](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

The VisioForge Video Capture SDK provides audio capture capabilities for .NET developers, covering microphone recording, system audio (loopback/speaker) capture, and combined audio recording. Whether you're building a podcast recorder, screen recording tool with audio, or a voice capture application, the SDK handles device enumeration, format negotiation, and encoding.

This guide provides complete, runnable code examples for the most common audio capture scenarios using both the **Video Capture SDK X** and **Media Blocks SDK** APIs.

## Supported Audio Sources

- **Physical microphones** — Desktop, USB, and Bluetooth microphones
- **Line-in ports** — External mixers or instruments
- **System audio (loopback)** — Record what's playing through your speakers or headphones
- **Virtual audio devices** — Capture audio from other applications
- **Network streams** — Audio from RTSP, HTTP, and other streaming sources

For detailed source configuration, see [Audio Sources](../audio-sources/index.md).

## Audio Format Support

### Lossy Formats

- [MP3](../../general/audio-encoders/mp3.md) — Industry standard, adjustable bitrates from 8 kbps to 320 kbps
- [M4A (AAC)](../../general/audio-encoders/aac.md) — Excellent quality-to-size ratio
- [Windows Media Audio](../../general/audio-encoders/wma.md) — Good compression with Windows integration
- [Ogg Vorbis](../../general/audio-encoders/vorbis.md) — Open-source, excellent quality at lower bitrates
- [Speex](../../general/audio-encoders/speex.md) — Optimized for speech

### Lossless Formats

- [WAV](../../general/audio-encoders/wav.md) — Uncompressed, perfect quality
- [FLAC](../../general/audio-encoders/flac.md) — Lossless compression without quality loss

## Record Microphone Audio to MP3

This console application records audio from the default microphone and saves it as an MP3 file.

### Required NuGet Packages

```bash
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.VideoCapture
```

Add the [redist package](../../deployment-x/index.md) for your platform (e.g., `VisioForge.DotNet.Core.Redist.Base.x64`).

The same packages are used for the 30-day trial and licensed applications. See the [VisioForge licensing page](https://www.visioforge.com/licensing) for trial terms and `.vflicense` activation.

### Complete Example

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core;
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
            // Enumerate audio capture devices
            var audioDevices = await DeviceEnumerator.Shared.AudioSourcesAsync(
                AudioCaptureDeviceAPI.DirectSound);

            if (audioDevices.Length == 0)
            {
                Console.WriteLine("No audio capture device found.");
                return;
            }

            // Display available devices
            Console.WriteLine("Available audio devices:");
            for (int i = 0; i < audioDevices.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioDevices[i].DisplayName}");
            }

            // Select first device (default microphone)
            var selectedDevice = audioDevices[0];
            var audioFormat = selectedDevice.GetDefaultFormat();
            var audioSource = selectedDevice.CreateSourceSettingsVC(audioFormat);

            // Configure audio-only capture
            videoCapture.Audio_Source = audioSource;
            videoCapture.Video_Source = null;
            videoCapture.Video_Play = false;
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;

            // Configure MP3 output
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"mic_recording_{DateTime.Now:yyyyMMdd_HHmmss}.mp3");

            var mp3Output = new MP3Output(outputPath);
            videoCapture.Outputs_Add(mp3Output, autostart: true);

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

### The Short Version

The one-minute video runs this console app and plays back the MP3 file it recorded.

<div class="video-wrapper">
  <iframe src="https://www.youtube.com/embed/dXehQhz7xa0?controls=1" frameborder="0" allowfullscreen></iframe>
</div>

A complete console app: it records the default microphone to `mic.mp3` until Enter is pressed. `DeviceEnumerator.Shared.AudioSourcesAsync()` lists the audio inputs of every audio API on the machine; on Windows, `IsDefault` marks the WASAPI2 entry `Default Audio Capture Device`, which follows the microphone selected in the Windows sound settings. To record another input, pass a part of its name that no other device has, exactly as the Windows sound settings show it (case and accents count), as the first argument. Each output device is listed as a loopback input under its own name, so passing an output's name, such as `Speakers`, records what that output plays. A device can be listed under more than one audio API, and the first entry whose name contains the text is used, whatever its API. With no default input, or no name that matches, `First` throws. `CreateSourceSettingsVC()` opens the device in its default format. `Audio_Play = false` keeps the sound off the speakers, `Audio_Record = true` sends it to the output, and `MP3Output` encodes it as MP3, 192 kbps CBR by default. `StopAsync` finishes the file: closing the window or pressing Ctrl+C ends the app without it, and the file is left unfinished.

<!-- vf-compile -->
```csharp
using System;
using System.Linq;
using VisioForge.Core;
using VisioForge.Core.Types.X.Output;
using VisioForge.Core.Types.X.Sources;
using VisioForge.Core.VideoCaptureX;

await VisioForgeX.InitSDKAsync();
await using var rec = new VideoCaptureCoreX();
rec.OnError += (_, e) =>
    Console.WriteLine(e.Message);
var mics = await DeviceEnumerator.Shared
    .AudioSourcesAsync();
var mic = args.Length > 0
    ? mics.First(d => d.Name.Contains(args[0]))
    : mics.First(d => d.IsDefault);
rec.Audio_Source =
    mic.CreateSourceSettingsVC();
rec.Audio_Play = false;
rec.Audio_Record = true;
rec.Outputs_Add(new MP3Output("mic.mp3"));
if (!await rec.StartAsync()) return;
Console.ReadLine();
await rec.StopAsync();
```

In the video the app runs as `dotnet run -- Speakers`: the input is the virtual machine's speakers loopback, recording a synthetic voice clip played on that machine; no live microphone was used. `ffprobe` shows MP3 at 192 kb/s, encoded from the device's 48000 Hz stereo input. The full project is [audio-capture-short-console](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_VideoTutorials/audio-capture-short-console), a Windows x64 console app that references `VisioForge.DotNet.VideoCapture` and the SDK's native package for that platform, `VisioForge.CrossPlatform.Core.Windows.x64`.

## Capture System Audio (Speaker / Loopback)

System audio capture (also called loopback or speaker capture) records any sound playing through your computer's output device. This is commonly used for screen recording with audio, capturing conference calls, or recording streaming audio.

On Windows, loopback capture uses the **WASAPI2** API to access output devices.

### Complete Example — Video Capture SDK X

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
        // Initialize SDK
        await VisioForgeX.InitSDKAsync();

        var videoCapture = new VideoCaptureCoreX();

        try
        {
            // Enumerate WASAPI2 audio output devices (speakers/headphones)
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No WASAPI2 audio output device found.");
                return;
            }

            // Display available loopback sources
            Console.WriteLine("Available loopback devices:");
            for (int i = 0; i < audioOutputs.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {audioOutputs[i].Name}");
            }

            // Select the first device
            var outputDevice = audioOutputs[0];

            // Create loopback source settings
            var audioSource = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            videoCapture.Audio_Source = audioSource;

            // Configure for audio-only capture
            videoCapture.Audio_Play = false;
            videoCapture.Audio_Record = true;
            videoCapture.Video_Play = false;

            // Configure M4A (AAC) output
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var m4aOutput = new M4AOutput(outputPath);
            videoCapture.Outputs_Add(m4aOutput, autostart: true);

            // Start capturing system audio
            await videoCapture.StartAsync();
            Console.WriteLine($"Capturing system audio to: {outputPath}");
            Console.WriteLine("Play some audio on your computer, then press ENTER to stop...");
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

### Complete Example — Media Blocks SDK

The Media Blocks SDK uses a pipeline approach where you connect source, processing, and output blocks. This example captures system audio using `SystemAudioSourceBlock` and saves it to an M4A file.

```csharp
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core;
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.Sources;

class Program
{
    static async Task Main(string[] args)
    {
        // Initialize SDK
        await VisioForgeX.InitSDKAsync();

        MediaBlocksPipeline pipeline = null;

        try
        {
            // Get the first WASAPI2 output device for loopback capture
            var audioOutputs = await DeviceEnumerator.Shared.AudioOutputsAsync(
                AudioOutputDeviceAPI.WASAPI2);

            if (audioOutputs.Length == 0)
            {
                Console.WriteLine("No WASAPI2 audio output device found.");
                return;
            }

            var outputDevice = audioOutputs[0];
            Console.WriteLine($"Using loopback device: {outputDevice.Name}");

            // Create pipeline
            pipeline = new MediaBlocksPipeline();

            // Create loopback audio source
            var sourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(outputDevice);
            var audioSource = new SystemAudioSourceBlock(sourceSettings);

            // Create M4A output
            string outputPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),
                $"system_audio_{DateTime.Now:yyyyMMdd_HHmmss}.m4a");

            var output = new M4AOutputBlock(outputPath);

            // Connect source to output
            pipeline.Connect(audioSource, output);

            // Start pipeline
            await pipeline.StartAsync();
            Console.WriteLine($"Capturing system audio to: {outputPath}");
            Console.WriteLine("Press ENTER to stop...");
            Console.ReadLine();

            // Stop pipeline
            await pipeline.StopAsync();
            Console.WriteLine("Recording saved.");
        }
        finally
        {
            if (pipeline != null)
            {
                await pipeline.DisposeAsync();
            }

            VisioForgeX.DestroySDK();
        }
    }
}
```

## Key Features

### Device Control

- Enumerate all available audio input and output devices
- Select specific input devices programmatically
- Set input volume levels and mute status
- Monitor audio levels in real-time with [VU meters](../../general/code-samples/vu-meters.md)
- Auto-select default system devices

### Advanced Processing

- Real-time audio visualization with spectrum and waveform analysis
- Noise reduction and echo cancellation
- Gain control and normalization
- Voice activity detection (VAD)
- Stereo/mono channel management
- Sample rate conversion

### Recording Controls

- Start, pause, resume, and stop recording
- Buffer management for low-latency operation
- Timed recordings with automatic stop
- File splitting for large recordings
- Auto file naming with timestamps

## Cross-Platform Notes

| Platform | Microphone | System Audio (Loopback) |
| -------- | ---------- | ----------------------- |
| Windows  | DirectSound, WASAPI2 | WASAPI2 loopback |
| macOS    | CoreAudio | Not available via SDK |
| Linux    | PulseAudio, ALSA | PulseAudio monitor |

System audio loopback capture is primarily a Windows feature using the WASAPI2 API. On Linux, PulseAudio monitor devices may provide similar functionality.

## Best Practices

1. **Check device availability** before starting capture — devices can be disconnected at any time
2. **Monitor audio levels** during recording to detect silence or clipping
3. **Choose the right format** — MP3/M4A for compressed output, WAV for maximum quality, FLAC for lossless compression
4. **Use WASAPI2** for loopback capture on Windows — it provides the lowest latency and most reliable system audio capture
5. **Handle errors gracefully** — implement error handling for device disconnection events
6. **Test on target hardware** — audio device behavior varies across systems

## Sample Applications

Complete working examples are available on GitHub:

- [Speaker Capture — Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/_CodeSnippets/speaker-capture)
- [Speaker Capture — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X/_CodeSnippets/speaker-capture)
- [Audio Capture Demo — Video Capture SDK X](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Capture%20SDK%20X)

## Related Documentation

- [Audio Sources — Device Configuration](../audio-sources/index.md)
- [Audio Rendering — Playback](../audio-rendering/index.md)
- [WMA Recording and Editing](../../general/guides/wma-recording-editing.md)
- [Screen Capture with Audio](../video-tutorials/screen-capture-mp4.md)

## Video Transcript

Record a microphone to an MP3 file, in a few lines of C#. Load the SDK and create the capture engine. List the audio inputs, and take the one named on the command line, or the default microphone. Audio only, not played back, recorded to an MP3 file. Start, wait for Enter, and stop: `StopAsync` finishes the file. Here it records the virtual machine's speakers, which play a synthetic voice clip, not a live microphone. `ffprobe` reads MP3 at 48 kHz, stereo, 192 kb/s. The guide adds system sound, other formats and devices.
