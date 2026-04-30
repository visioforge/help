---
title: Media Blocks SDK Sources — Webcam, File, RTSP, Screen in C#
description: Connect any input to a Media Blocks pipeline: webcams, video files, RTSP, RTMP, SRT, NDI, Decklink, screen, virtual. C# / .NET device enumeration API.
sidebar_label: Sources
tags:
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
primary_api_classes:
  - VideoRendererBlock
  - MediaBlocksPipeline
  - AudioRendererBlock
  - UniversalSourceBlock
  - DeviceEnumerator

---

# Source Blocks - VisioForge Media Blocks SDK .Net

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

Source blocks provide data to the pipeline and are typically the first blocks in any media processing chain. VisioForge Media Blocks SDK .Net provides a comprehensive collection of source blocks for various inputs including hardware devices, files, networks, and virtual sources.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../../general/mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Hardware Source Blocks

### System Video Source

SystemVideoSourceBlock is used to access webcams and other video capture devices.

#### Block info

Name: SystemVideoSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | uncompressed video | 1 |

#### Enumerate available devices

Use the `DeviceEnumerator.Shared.VideoSourcesAsync()` method to get a list of available devices and their specifications: available resolutions, frame rates, and video formats.
This method returns a list of `VideoCaptureDeviceInfo` objects. Each `VideoCaptureDeviceInfo` object provides detailed information about a capture device.

#### The sample pipeline

```mermaid
graph LR;
    SystemVideoSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create video source
VideoCaptureDeviceSourceSettings videoSourceSettings = null;

// select the first device
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    // select the first format (maybe not the best, but it is just a sample)
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        // select the first frame rate
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// create video source block using the selected device and format
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);

// create video renderer block
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connect blocks
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// start pipeline
await pipeline.StartAsync();
```

#### Sample applications

- [Simple Video Capture Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Remarks

You can specify an API to use during the device enumeration (refer to the `VideoCaptureDeviceAPI` enum description under `SystemVideoSourceBlock` for typical values). Android and iOS platforms have only one API, while Windows and Linux have multiple APIs.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### System Audio Source

SystemAudioSourceBlock is used to access mics and other audio capture devices.

#### Block info

Name: SystemAudioSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | uncompressed audio | 1 |

#### Enumerate available devices

Use the `DeviceEnumerator.Shared.AudioSourcesAsync()` method call to get a list of available devices and their specifications.

During device enumeration, you can get the list of available devices and their specifications. You can select the device and its format to create the source settings.

#### The sample pipeline

```mermaid
graph LR;
    SystemAudioSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create audio source block
IAudioCaptureDeviceSourceSettings audioSourceSettings = null;

// select first device
var device = (await DeviceEnumerator.Shared.AudioSourcesAsync())[0];
if (device != null)
{
    // select first format
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = device.CreateSourceSettings(formatItem.ToFormat());
    }    
}

// create audio source block using selected device and format
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// create audio renderer block  
var audioRenderer = new AudioRendererBlock();

// connect blocks
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// start pipeline
await pipeline.StartAsync();
```

#### Capture audio from speakers (loopback)

Currently, loopback audio capture is supported only on Windows. Use the `LoopbackAudioCaptureDeviceSourceSettings` class to create the source settings for loopback audio capture.

WASAPI2 is used as the default API for loopback audio capture. You can specify the API to use during device enumeration.

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create audio source block
var deviceItem = (await DeviceEnumerator.Shared.AudioOutputsAsync(AudioOutputDeviceAPI.WASAPI2))[0];
if (deviceItem == null)
{
    return;
}

var audioSourceSettings = new LoopbackAudioCaptureDeviceSourceSettings(deviceItem);
var audioSource = new SystemAudioSourceBlock(audioSourceSettings);

// create audio renderer block  
var audioRenderer = new AudioRendererBlock();

// connect blocks
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// start pipeline
await pipeline.StartAsync();
```

#### Sample applications

- [Audio Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Audio%20Capture%20Demo)
- [Simple Capture Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Capture%20Demo)

#### Remarks

You can specify an API to use during the device enumeration. Android and iOS platforms have only one API, while Windows and Linux have multiple APIs.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Basler Source Block

The Basler source block supports Basler USB3 Vision and GigE cameras.
The Pylon SDK or Runtime should be installed to use the camera source.

#### Block info

Name: BaslerSourceBlock.

| Pin direction   |      Media type      | Pins count  |
|-----------------|:--------------------:|:-----------:|
| Output video    |     Uncompressed     |      1      |

#### The sample pipeline

```mermaid
graph LR;
    BaslerSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// get Basler source info by enumerating sources
var sources = await DeviceEnumerator.Shared.BaslerSourcesAsync();
var sourceInfo = sources[0];

// create Basler source    
var source = new BaslerSourceBlock(new BaslerSourceSettings(sourceInfo));

// create video renderer for VideoView
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connect
pipeline.Connect(source.Output, videoRenderer.Input);

// start
await pipeline.StartAsync();
```

#### Sample applications

- [Basler Source Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Basler%20Source%20Demo)

#### Platforms

Windows, Linux.

### Spinnaker/FLIR Source Block

The Spinnaker/FLIR source supports connection to FLIR cameras using Spinnaker SDK.

To use the `SpinnakerSourceBlock`, you first need to enumerate available Spinnaker cameras and then configure the source using `SpinnakerSourceSettings`.

#### Enumerate Devices & `SpinnakerCameraInfo`

Use `DeviceEnumerator.Shared.SpinnakerSourcesAsync()` to get a list of `SpinnakerCameraInfo` objects. Each `SpinnakerCameraInfo` provides details about a detected camera:

- `Name` (string): Unique identifier or name of the camera. Often a serial number or model-serial combination.
- `NetworkInterfaceName` (string): Name of the network interface if it's a GigE camera.
- `Vendor` (string): Camera vendor name.
- `Model` (string): Camera model name.
- `SerialNumber` (string): Camera's serial number.
- `FirmwareVersion` (string): Camera's firmware version.
- `SensorSize` (`Size`): Reports the sensor dimensions (Width, Height). You might need to call a method on `SpinnakerCameraInfo` like `ReadInfo()` (if available, or implied by enumeration) to populate this.
- `WidthMax` (int): Maximum sensor width.
- `HeightMax` (int): Maximum sensor height.

You select a `SpinnakerCameraInfo` object from the list to initialize `SpinnakerSourceSettings`.

#### Settings

The `SpinnakerSourceBlock` is configured using `SpinnakerSourceSettings`. Key properties:

- `Name` (string): The name of the camera (from `SpinnakerCameraInfo.Name`) to use.
- `Region` (`Rect`): Defines the Region of Interest (ROI) to capture from the camera sensor. Set X, Y, Width, Height.
- `FrameRate` (`VideoFrameRate`): The desired frame rate.
- `PixelFormat` (`SpinnakerPixelFormat` enum): The desired pixel format (e.g., `RGB`, `Mono8`, `BayerRG8`). Default `RGB`.
- `OffsetX` (int): X offset for the ROI on the sensor (default 0). Often implicitly part of `Region.X`.
- `OffsetY` (int): Y offset for the ROI on the sensor (default 0). Often implicitly part of `Region.Y`.
- `ExposureMinimum` (int): Minimum exposure time for auto-exposure algorithm (microseconds, e.g., 10-29999999). Default 0 (auto/camera default).
- `ExposureMaximum` (int): Maximum exposure time for auto-exposure algorithm (microseconds). Default 0 (auto/camera default).
- `ShutterType` (`SpinnakerSourceShutterType` enum): Type of shutter (e.g., `Rolling`, `Global`). Default `Rolling`.

Constructor:
`SpinnakerSourceSettings(string deviceName, Rect region, VideoFrameRate frameRate, SpinnakerPixelFormat pixelFormat = SpinnakerPixelFormat.RGB)`

#### Block info

Name: SpinnakerSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | various | one or more |

#### The sample pipeline

`SpinnakerSourceBlock:Output` &#8594;  `VideoRendererBlock`

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var sources = await DeviceEnumerator.Shared.SpinnakerSourcesAsync();
var sourceSettings = new SpinnakerSourceSettings(sources[0].Name, new VisioForge.Core.Types.Rect(0, 0, 1280, 720), new VideoFrameRate(10)); 

var source = new SpinnakerSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Requirements

- Spinnaker SDK installed.

#### Platforms

Windows

### Allied Vision Source Block

The Allied Vision Source Block enables integration with Allied Vision cameras using the Vimba SDK. It allows capturing video streams from these industrial cameras.

#### Block info

Name: AlliedVisionSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### The sample pipeline

```mermaid
graph LR;
    AlliedVisionSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerate Allied Vision cameras
var alliedVisionCameras = await DeviceEnumerator.Shared.AlliedVisionSourcesAsync();
if (alliedVisionCameras.Count == 0)
{
    Console.WriteLine("No Allied Vision cameras found.");
    return;
}

var cameraInfo = alliedVisionCameras[0]; // Select the first camera

// Create Allied Vision source settings
// Width, height, x, y are optional and depend on whether you want to set a specific ROI
// If null, it might use default/full sensor resolution. Camera.ReadInfo() should be called.
cameraInfo.ReadInfo(); // Ensure camera info like Width/Height is read

var alliedVisionSettings = new AlliedVisionSourceSettings(
    cameraInfo,
    width: cameraInfo.Width, // Or a specific ROI width
    height: cameraInfo.Height // Or a specific ROI height
);

// Optionally configure other settings
alliedVisionSettings.ExposureAuto = VmbSrcExposureAutoModes.Continuous;
alliedVisionSettings.Gain = 10; // Example gain value

var alliedVisionSource = new AlliedVisionSourceBlock(alliedVisionSettings);

// Create video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control

// Connect blocks
pipeline.Connect(alliedVisionSource.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Requirements

- Allied Vision Vimba SDK must be installed.

#### Sample applications

- Refer to samples demonstrating industrial camera integration if available.

#### Platforms

Windows, macOS, Linux.

### Blackmagic Decklink Source Block

For information about Decklink sources, see [Decklink](../Decklink/index.md).

## File Source Blocks

### Universal Source Block

A universal source that decodes video and audio files/network streams and provides uncompressed data to the connected blocks.

Block supports MP4, WebM, AVI, TS, MKV, MP3, AAC, M4A, and many other formats. If FFMPEG redist is available, all decoders available in FFMPEG will also be supported.

#### Settings

The `UniversalSourceBlock` is configured through `UniversalSourceSettings`. It's recommended to create settings using the static factory method `await UniversalSourceSettings.CreateAsync(...)`.

Key properties and parameters for `UniversalSourceSettings`:

- **URI/Filename**:
  - `UniversalSourceSettings.CreateAsync(string filename, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)`: Creates settings from a local file path.
  - `UniversalSourceSettings.CreateAsync(System.Uri uri, bool renderVideo = true, bool renderAudio = true, bool renderSubtitle = false)`: Creates settings from a `System.Uri` (can be a file URI or network URI like HTTP, RTSP - though dedicated blocks are often preferred for network streams). For iOS, an `Foundation.NSUrl` is used.
  - The `renderVideo`, `renderAudio`, `renderSubtitle` booleans control which streams are processed. The `CreateAsync` method may update these based on actual stream availability in the media file/stream if `ignoreMediaInfoReader` is `false` (default).
- `StartPosition` (`TimeSpan?`): Sets the starting position for playback.
- `StopPosition` (`TimeSpan?`): Sets the stopping position for playback.
- `VideoCustomFrameRate` (`VideoFrameRate?`): If set, video frames will be dropped or duplicated to match this custom frame rate.
- `UseAdvancedEngine` (bool): If `true` (default, except Android where it's `false`), uses an advanced engine with stream selection support.
- `DisableHWDecoders` (bool): If `true` (default `false`, except Android where it's `true`), hardware-accelerated decoders will be disabled, forcing software decoding.
- `MPEGTSProgramNumber` (int): For MPEG-TS streams, specifies the program number to select (default -1, meaning automatic selection or first program).
- `ReadInfoAsync()`: Asynchronously reads media file information (`MediaFileInfo`). This is called internally by `CreateAsync` unless `ignoreMediaInfoReader` is true.
- `GetInfo()`: Gets the cached `MediaFileInfo`.

The `UniversalSourceBlock` itself is then instantiated with these settings: `new UniversalSourceBlock(settings)`.
The `Filename` property on `UniversalSourceBlock` instance (as seen in older examples) is a shortcut that internally creates basic `UniversalSourceSettings`. Using `UniversalSourceSettings.CreateAsync` provides more control.

#### Block info

Name: UniversalSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | depends from decoder | one or more |
| Output video | depends from decoder | one or more |
| Output subtitle | depends from decoder | one or more |

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock-->VideoRendererBlock;
    UniversalSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri("test.mp4"));
var fileSource = new UniversalSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(fileSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(fileSource.AudioOutput, audioRenderer.Input);            

await pipeline.StartAsync();
```

#### Sample applications

- [Simple Player Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Simple%20Player%20Demo%20WPF)

#### Platforms

Windows, macOS, Linux, iOS, Android.


### Subtitle Source Block

The Subtitle Source Block loads subtitles from a file and outputs them as a subtitle stream, which can then be overlaid on video or rendered separately.

#### Block info

Name: `SubtitleSourceBlock`.

| Pin direction   | Media type         | Pins count  |
|-----------------|:--------------------:|:-----------:|
| Output subtitle | Subtitle data      | 1           |

#### Settings

The `SubtitleSourceBlock` is configured using `SubtitleSourceSettings`. Key properties include:

- `Filename` (string): The path to the subtitle file (e.g., .srt, .ass).

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlock --> SubtitleOverlayBlock;
    SubtitleSourceBlock --> SubtitleOverlayBlock;
    SubtitleOverlayBlock --> VideoRendererBlock;
    UniversalSourceBlock --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create subtitle source settings
var subtitleSettings = new SubtitleSourceSettings("path/to/your/subtitles.srt");
var subtitleSource = new SubtitleSourceBlock(subtitleSettings);

// Example: Overlaying subtitles on a video from UniversalSourceBlock
var fileSource = await UniversalSourceSettings.CreateAsync("path/to/your/video.mp4");
var universalSource = new UniversalSourceBlock(fileSource);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

// This is a conceptual overlay. Actual implementation might need a specific subtitle overlay block.
// For simplicity, let's assume a downstream block can consume a subtitle stream,
// or you connect it to a block that renders subtitles on the video.
// Example with a hypothetical SubtitleOverlayBlock:
// var subtitleOverlay = new SubtitleOverlayBlock(); // Assuming such a block exists
// pipeline.Connect(universalSource.VideoOutput, subtitleOverlay.VideoInput);
// pipeline.Connect(subtitleSource.Output, subtitleOverlay.SubtitleInput);
// pipeline.Connect(subtitleOverlay.Output, videoRenderer.Input);
// pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);

// For a simple player without explicit overlay shown here:
pipeline.Connect(universalSource.VideoOutput, videoRenderer.Input);
pipeline.Connect(universalSource.AudioOutput, audioRenderer.Input);
// How subtitles from subtitleSource.Output are used would depend on the rest of the pipeline design.
// This block primarily provides the subtitle stream.

Console.WriteLine("Subtitle source created. Connect its output to a compatible block like a subtitle overlay or renderer.");

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android (Depends on subtitle parsing capabilities).

### Stream Source Block

The Stream Source Block allows reading media data from a `System.IO.Stream`. This is useful for playing media from memory, embedded resources, or custom stream providers without needing a temporary file. The format of the data within the stream must be parsable by the underlying media framework (GStreamer).

#### Block info

Name: `StreamSourceBlock`.
(Pin information is dynamic, similar to `UniversalSourceBlock`, based on stream content. Typically, it would have an output that connects to a demuxer/decoder like `DecodeBinBlock`, or provide decoded audio/video pins if it includes demuxing/decoding capabilities.)

| Pin direction | Media type         | Pins count |
|---------------|:------------------:|:----------:|
| Output data   | Varies (raw stream)| 1          |
| Output video  | Depends on stream  | 0 or 1     |
| Output audio  | Depends on stream  | 0 or 1+    |

#### Settings

The `StreamSourceBlock` is typically instantiated directly with a `System.IO.Stream`. The `StreamSourceSettings` class serves as a wrapper to provide this stream.

- `Stream` (`System.IO.Stream`): The input stream containing the media data. The stream must be readable and, if seeking is required by the pipeline, seekable.

#### The sample pipeline

If `StreamSourceBlock` outputs raw data that needs decoding:

```mermaid
graph LR;
    StreamSourceBlock -- Stream Data --> DecodeBinBlock;
    DecodeBinBlock -- Video Output --> VideoRendererBlock;
    DecodeBinBlock -- Audio Output --> AudioRendererBlock;
```

If `StreamSourceBlock` handles decoding internally (less common for a generic stream source):

```mermaid
graph LR;
    StreamSourceBlock -- Video Output --> VideoRendererBlock;
    StreamSourceBlock -- Audio Output --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Example: Load a video file into a MemoryStream
byte[] fileBytes = File.ReadAllBytes("path/to/your/video.mp4");
var memoryStream = new MemoryStream(fileBytes);

// StreamSourceSettings is a container for the stream.
var streamSettings = new StreamSourceSettings(memoryStream);
// The CreateBlock method of StreamSourceSettings would typically return new StreamSourceBlock(streamSettings.Stream)
var streamSource = streamSettings.CreateBlock() as StreamSourceBlock; 
// Or, more directly: var streamSource = new StreamSourceBlock(memoryStream);

// Create video and audio renderers
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1
var audioRenderer = new AudioRendererBlock();

// Connect outputs. Commonly, a StreamSourceBlock provides raw data to a DecodeBinBlock.
var decodeBin = new DecodeBinBlock();
pipeline.Connect(streamSource.Output, decodeBin.Input); // Assuming a single 'Output' pin on StreamSourceBlock
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();

// Important: Ensure the stream remains open and valid for the duration of playback.
// Dispose of the stream when the pipeline is stopped or disposed.
// Consider this in relation to pipeline.DisposeAsync() or similar cleanup.
// memoryStream.Dispose(); // Typically after pipeline.StopAsync() and pipeline.DisposeAsync()
```

#### Remarks

The `StreamSourceBlock` itself will attempt to read from the provided stream. The success of playback depends on the format of the data in the stream and the availability of appropriate demuxers and decoders in the subsequent parts of the pipeline (often managed via `DecodeBinBlock`).

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Stream Source Block With Decoder

The `StreamSourceBlockWithDecoder` combines stream reading and automatic decoding into a single block. It accepts a `System.IO.Stream` or a file path and automatically demuxes and decodes the content — no additional `DecodeBinBlock` is required.

#### Block info

Name: StreamSourceBlockWithDecoder.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 0 or 1 |
| Output audio | Uncompressed audio | 0 or 1 |

#### Constructors

```csharp
// From a .NET stream
StreamSourceBlockWithDecoder(Stream stream)

// From a file path
StreamSourceBlockWithDecoder(string filename)
```

The block internally connects a `StreamSourceBlock` to a `DecodeBinBlock`. The `VideoOutput` and `AudioOutput` pads are `null` if the source does not contain the respective media type.

#### The sample pipeline

```mermaid
graph LR;
    StreamSourceBlockWithDecoder -- Video --> VideoRendererBlock;
    StreamSourceBlockWithDecoder -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// From a file path
var source = new StreamSourceBlockWithDecoder("C:/media/video.mp4");

// Or from a stream:
// var stream = File.OpenRead("C:/media/video.mp4");
// var source = new StreamSourceBlockWithDecoder(stream);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Image Sequence Source Block

The `ImageSequenceSourceBlock` generates a video stream from a sequence of still images stored in a folder. Configurable frame rate and live/seekable stream mode are supported.

#### Block info

Name: ImageSequenceSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |

#### Settings

The block takes an `ImageSequenceSourceSettings` instance:

Constructor: `ImageSequenceSourceSettings(string folderPath, string filePattern = null)`

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `FolderPath` | `string` | required | Folder containing image files |
| `FilePattern` | `string` | auto-detected | Filename pattern (e.g., `image_%05d.jpg`); auto-detected if `null` |
| `StartIndex` | `int` | auto-detected | Starting index of the image sequence; auto-detected from the first file name |
| `FrameRate` | `VideoFrameRate` | `FPS_25` | Output frame rate |
| `IsLive` | `bool` | `false` | Treat the source as live (non-seekable) |
| `NumBuffers` | `int` | `-1` | Maximum frames to output (−1 = unlimited) |

Supported image formats: `.jpg`, `.jpeg`, `.png`, `.bmp`, `.gif`, `.tiff`, `.tif`.

#### The sample pipeline

```mermaid
graph LR;
    ImageSequenceSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageSequenceSourceSettings("C:/images/sequence/")
{
    FrameRate = VideoFrameRate.FPS_25
};

var imageSource = new ImageSequenceSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

`ImageSequenceSourceBlock.IsAvailable(settings)` returns `true` if the GStreamer image sequence element is available.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Animated GIF Source Block

The `AnimatedGIFSourceBlock` generates a video stream from an animated GIF file. It supports configurable frame rate, looping, and live/seekable stream mode.

#### Block info

Name: AnimatedGIFSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |

#### Settings

The block takes an `ImageVideoSourceSettings` instance:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Filename` | `string` | required | Path to the animated GIF file |
| `IsLive` | `bool` | `false` | Treat the source as live (non-seekable) |
| `NumBuffers` | `int` | `-1` | Maximum frames to output (−1 = unlimited) |
| `AnimationInfiniteLoop` | `bool` | `true` | Loop the animation indefinitely |
| `FrameRate` | `VideoFrameRate` | `FPS_10` | Output frame rate |

#### The sample pipeline

```mermaid
graph LR;
    AnimatedGIFSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("C:/media/animation.gif")
{
    AnimationInfiniteLoop = true,
    FrameRate = VideoFrameRate.FPS_25
};

var gifSource = new AnimatedGIFSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(gifSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

`AnimatedGIFSourceBlock.IsAvailable()` returns `true` if the GStreamer animated GIF element is available.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### CDG Source Block

The CDG Source Block is designed to play CD+G (Compact Disc + Graphics) files, commonly used for karaoke. It decodes both the audio track and the low-resolution graphics stream.

#### Block info

Name: CDGSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output audio  | Uncompressed audio | 1          |
| Output video  | Uncompressed video | 1          |

#### The sample pipeline

```mermaid
graph LR;
    CDGSourceBlock -- Audio --> AudioRendererBlock;
    CDGSourceBlock -- Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create CDG source settings
var cdgSettings = new CDGSourceSettings(
    "path/to/your/file.cdg",  // Path to the CDG graphics file
    "path/to/your/file.mp3"   // Path to the corresponding audio file (MP3, WAV, etc.)
);
// If audioFilename is null or empty, audio will be ignored.

var cdgSource = new CDGSourceBlock(cdgSettings);

// Create video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control
pipeline.Connect(cdgSource.VideoOutput, videoRenderer.Input);

// Create audio renderer (if audio is to be played)
if (!string.IsNullOrEmpty(cdgSettings.AudioFilename) && cdgSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(cdgSource.AudioOutput, audioRenderer.Input);
}

// Start pipeline
await pipeline.StartAsync();
```

#### Remarks

Requires both a `.cdg` file for graphics and a separate audio file (e.g., MP3, WAV) for the music.

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Network Source Blocks

### VNC Source Block

The VNC Source Block allows capturing video from a VNC (Virtual Network Computing) or RFB (Remote Framebuffer) server. This is useful for streaming the desktop of a remote machine.

#### Block info

Name: `VNCSourceBlock`.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### Settings

The `VNCSourceBlock` is configured using `VNCSourceSettings`. Key properties include:

- `Host` (string): The hostname or IP address of the VNC server.
- `Port` (int): The port number of the VNC server.
- `Password` (string): The password for VNC server authentication, if required.
- `Uri` (string): Alternatively, a full RFB URI (e.g., "rfb://host:port").
- `Width` (int): Desired output width. The block may connect to a VNC server that provides specific dimensions.
- `Height` (int): Desired output height.
- `Shared` (bool): Whether to share the desktop with other clients (default `true`).
- `ViewOnly` (bool): If `true`, no input (mouse/keyboard) is sent to the VNC server (default `false`).
- `Incremental` (bool): Whether to use incremental updates (default `true`).
- `UseCopyrect` (bool): Whether to use copyrect encoding (default `false`).
- `RFBVersion` (string): RFB protocol version (default "3.3").
- `OffsetX` (int): X offset for screen scraping.
- `OffsetY` (int): Y offset for screen scraping.

#### The sample pipeline

```mermaid
graph LR;
    VNCSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configure VNC source settings
var vncSettings = new VNCSourceSettings
{
    Host = "your-vnc-server-ip", // or use Uri
    Port = 5900, // Standard VNC port
    Password = "your-password", // if any
    // Width = 1920, // Optional: desired width
    // Height = 1080, // Optional: desired height
};

var vncSource = new VNCSourceBlock(vncSettings);

// Create video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control

// Connect blocks
pipeline.Connect(vncSource.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux (Depends on underlying GStreamer VNC plugin availability).

### RTSP Source Block

The RTSP source supports connection to IP cameras and other devices supporting the RTSP protocol.

Supported video codecs: H264, HEVC, MJPEG.
Supported audio codecs: AAC, MP3, PCM, G726, G711, and some others if FFMPEG redist is installed.

#### Block info

Name: RTSPSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | depends from decoder | one or more |
| Output video | depends from decoder | one or more |
| Output subtitle | depends from decoder | one or more |

#### Settings

The `RTSPSourceBlock` is configured using `RTSPSourceSettings`. Key properties include:

- `Uri`: The RTSP URL of the stream.
- `Login`: Username for RTSP authentication, if required.
- `Password`: Password for RTSP authentication, if required.
- `AudioEnabled`: A boolean indicating whether to attempt to process the audio stream.
- `Latency`: Specifies the buffering duration for the incoming stream (default is 1000ms).
- `AllowedProtocols`: Defines the transport protocols to be used for receiving the stream. It's a flags enum `RTSPSourceProtocol` with values:
  - `UDP`: Stream data over UDP.
  - `UDP_Multicast`: Stream data over UDP multicast.
  - `TCP` (Recommended): Stream data over TCP.
  - `HTTP`: Stream data tunneled over HTTP.
  - `EnableTLS`: Encrypt TCP and HTTP with TLS (use `rtsps://` or `httpsps://` in URI).
- `DoRTCP`: Enables RTCP (RTP Control Protocol) for stream statistics and control (default is usually true).
- `RTPBlockSize`: Specifies the size of RTP blocks.
- `UDPBufferSize`: Buffer size for UDP transport.
- `CustomVideoDecoder`: Allows specifying a custom GStreamer video decoder element name if the default is not suitable.
- `UseGPUDecoder`: If set to `true`, the SDK will attempt to use a hardware-accelerated GPU decoder if available.
- `CompatibilityMode`: If `true`, the SDK will not try to read camera information before attempting to play, which can be useful for problematic streams.
- `EnableRAWVideoAudioEvents`: If `true`, enables events for raw (undecoded) video and audio sample data.

It's recommended to initialize `RTSPSourceSettings` using the static factory method `RTSPSourceSettings.CreateAsync(Uri uri, string login, string password, bool audioEnabled, bool readInfo = true)`. This method can also handle ONVIF discovery if the URI points to an ONVIF device service. Setting `readInfo` to `false` enables `CompatibilityMode`.

#### The sample pipeline

```mermaid
graph LR;
    RTSPSourceBlock-->VideoRendererBlock;
    RTSPSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// It's recommended to use CreateAsync to initialize settings
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    new Uri("rtsp://login:pwd@192.168.1.64:554/Streaming/Channels/101?transportmode=unicast&profile=Profile_1"),
    "login", 
    "pwd",
    audioEnabled: true);

// Optionally, configure more settings
// rtspSettings.Latency = TimeSpan.FromMilliseconds(500);
// rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP; // Prefer TCP

var rtspSource = new RTSPSourceBlock(rtspSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtspSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtspSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Sample applications

- [RTSP Preview Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20Preview%20Demo)
- [RTSP MultiViewSync Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20MultiViewSync%20Demo)

#### Platforms

Windows, macOS, Linux, iOS, Android.

### RTSP RAW Source Block

The `RTSPRAWSourceBlock` connects to an RTSP stream and delivers the **encoded** (undecoded) video and audio data directly. Unlike `RTSPSourceBlock`, no decoding pipeline is created, resulting in lower CPU overhead. Use this block when you need to record, re-stream, or process the raw compressed bitstream.

#### Block info

Name: RTSPRAWSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Encoded video (e.g. H.264) | 1 |
| Output audio | Encoded audio (e.g. AAC) | 1 |

#### Settings

Use the factory method `RTSPRAWSourceSettings.CreateAsync(uri, login, password, audioEnabled)` to create settings. Key properties:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Uri` | `Uri` | — | RTSP stream URL |
| `AllowedProtocols` | `RTSPSourceProtocol` | All | Transport protocols (TCP, UDP, UDP_Multicast, HTTP) |
| `Latency` | `int` | — | Buffering in milliseconds |
| `Login` | `string` | — | Authentication username |
| `Password` | `string` | — | Authentication password |
| `AudioEnabled` | `bool` | `true` | Enable audio output pad |
| `WaitForKeyframe` | `bool` | `true` | Wait for IDR frame before forwarding video |
| `SyncAudioWithKeyframe` | `bool` | `true` | Synchronize audio delivery to first keyframe |
| `DoRTCP` | `bool` | — | Enable RTCP control protocol |
| `UDPBufferSize` | `int` | — | UDP receive buffer size |

#### The sample pipeline

```mermaid
graph LR;
    RTSPRAWSourceBlock-->MP4SinkBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://192.168.1.64:554/stream"),
    login: "admin",
    password: "password",
    audioEnabled: true);

var rtspRawSource = new RTSPRAWSourceBlock(settings);

// VideoOutput carries encoded video (e.g. H.264)
// AudioOutput carries encoded audio (e.g. AAC)
var mp4Sink = new MP4SinkBlock(new MP4SinkSettings("output.mp4"));
pipeline.Connect(rtspRawSource.VideoOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Video));
pipeline.Connect(rtspRawSource.AudioOutput, mp4Sink.CreateNewInput(MediaBlockPadMediaType.Audio));

await pipeline.StartAsync();
```

#### Availability

`RTSPRAWSourceBlock.IsAvailable()` returns `true` if the required GStreamer RTSP elements are available.

#### Platforms

Windows, macOS, Linux.

### RTMP Source Block

The RTMP source block connects to RTMP streams published by media servers (Nginx-RTMP, Wowza, OBS, FFmpeg, etc.). It outputs compressed video and audio pads for use with decoder or recording blocks.

Supported video codecs: H.264, H.265, VP8.
Supported audio codecs: AAC, MP3.

#### Block info

Name: RTMPSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Compressed video | 1 |
| Output audio | Compressed audio | 1 |

#### Settings

`RTMPSourceBlock` is configured using `RTMPSourceSettings`. Use the static factory method to initialize settings with stream information read from the server:

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);
```

Set `readInfo: false` (which enables `CompatibilityMode`) for streams where pre-read fails:

```csharp
var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true,
    readInfo: false);
```

Key properties:

- `Uri`: RTMP stream URI.
- `AudioEnabled`: Whether to create an audio output pad (default `true`).
- `CompatibilityMode`: Skip stream-info reading; useful for problematic streams.
- `UseGPUDecoder`: Attempt hardware-accelerated decoding.
- `ExtraConnectArgs`: Additional RTMP connection query parameters.

#### The sample pipeline

```mermaid
graph LR;
    RTMPSourceBlock-->VideoRendererBlock;
    RTMPSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var rtmpSettings = await RTMPSourceSettings.CreateAsync(
    new Uri("rtmp://example.com/live/stream"),
    audioEnabled: true);

var rtmpSource = new RTMPSourceBlock(rtmpSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(rtmpSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(rtmpSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux.

### HTTP Source Block

The HTTP source block allows data to be retrieved using HTTP/HTTPS protocols.
It can be used to read data from MJPEG IP cameras, MP4 network files, or other sources.

#### Block info

Name: HTTPSourceBlock.

| Pin direction |  Media type  | Pins count  |
|---------------|:------------:|:-----------:|
| Output        |     Data     |      1      |

#### The sample pipeline

The sample pipeline reads data from an MJPEG camera and displays it using VideoView.

```mermaid
graph LR;
    HTTPSourceBlock-->JPEGDecoderBlock;
    JPEGDecoderBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new HTTPSourceSettings(new Uri("http://mjpegcamera:8080"))
{
    UserID = "username",
    UserPassword = "password"
};

var source = new HTTPSourceBlock(settings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var jpegDecoder = new JPEGDecoderBlock();

pipeline.Connect(source.Output, jpegDecoder.Input);
pipeline.Connect(jpegDecoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux.


### HTTP MJPEG Source Block

The HTTP MJPEG Source Block is specifically designed to connect to and decode MJPEG (Motion JPEG) video streams over HTTP/HTTPS. This is common for many IP cameras.

#### Block info

Name: HTTPMJPEGSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### The sample pipeline

```mermaid
graph LR;
    HTTPMJPEGSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create settings for the HTTP MJPEG source
var mjpegSettings = await HTTPMJPEGSourceSettings.CreateAsync(
    new Uri("http://your-mjpeg-camera-url/stream"), // Replace with your camera's MJPEG stream URL
    "username", // Optional: username for camera authentication
    "password"  // Optional: password for camera authentication
);

if (mjpegSettings == null)
{
    Console.WriteLine("Failed to initialize HTTP MJPEG settings.");
    return;
}

mjpegSettings.CustomVideoFrameRate = new VideoFrameRate(25); // Optional: Set if camera doesn't report frame rate
mjpegSettings.Latency = TimeSpan.FromMilliseconds(200); // Optional: Adjust latency

var httpMjpegSource = new HTTPMJPEGSourceBlock(mjpegSettings);

// Create video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control

// Connect blocks
pipeline.Connect(httpMjpegSource.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Sample applications

- Similar to HTTP MJPEG Source Demo mentioned under the generic HTTP Source Block.

#### Platforms

Windows, macOS, Linux.

### NDI Source Block

The NDI source block supports connection to NDI software sources and devices supporting the NDI protocol.

#### Block info

Name: NDISourceBlock.

| Pin direction   |      Media type      | Pins count  |
|-----------------|:--------------------:|:-----------:|
| Output audio    |     Uncompressed     |      1      |
| Output video    |     Uncompressed     |      1      |

#### The sample pipeline

```mermaid
graph LR;
    NDISourceBlock-->VideoRendererBlock;
    NDISourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// get NDI source info by enumerating sources
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

// CreateAsync takes (ContextX context, NDISourceInfo info) and must be awaited.
var ndiSettings = await NDISourceSettings.CreateAsync(pipeline.GetContext(), ndiSourceInfo);

var ndiSource = new NDISourceBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);      

await pipeline.StartAsync();
```

#### Sample applications

- [NDI Source Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/NDI%20Source%20Demo)

#### Platforms

Windows, macOS, Linux.

### NDI Source X Block

The `NDISourceXBlock` is an extended NDI source that captures video and audio from NDI network sources. It uses the same `NDISourceSettings` as `NDISourceBlock` but uses an alternative GStreamer-based NDI element (`ndisrcx`).

#### Block info

Name: NDISourceXBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |
| Output audio | Uncompressed audio | 1 |

#### Settings

The block accepts `NDISourceSettings`. Use the static async factory to create it:

```csharp
var ndiSettings = await NDISourceSettings.CreateAsync(context, ndiSourceInfo);
```

Key properties of `NDISourceSettings`:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `SourceName` | `string` | `""` | Name of the NDI source |
| `URL` | `string` | `""` | Source URL |
| `ReceiverName` | `string` | `"VF NDI Receiver"` | Receiver application name |
| `Bandwidth` | `int` | `100` | −10 metadata only, 10 audio only, 100 full quality |
| `ColorFormat` | `NDIRecvColorFormat` | `UyvyBgra` | Pixel format for received video |
| `Timeout` | `TimeSpan` | 5 s | Timeout for detecting disconnection |
| `ConnectTimeout` | `TimeSpan` | 10 s | Timeout for initial connection |
| `TimestampMode` | `NDITimestampMode` | `Auto` | Timestamp synchronization mode |

#### The sample pipeline

```mermaid
graph LR;
    NDISourceXBlock -- Video --> VideoRendererBlock;
    NDISourceXBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSourceInfo = ndiSources[0];

// CreateAsync takes (ContextX context, NDISourceInfo info).
var ndiSettings = await NDISourceSettings.CreateAsync(pipeline.GetContext(), ndiSourceInfo);

var ndiSource = new NDISourceXBlock(ndiSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(ndiSource.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(ndiSource.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows.

### GenICam Source Block

The GenICam source supports connection to GigE, and the USB3 Vision camera supports the GenICam protocol.

#### Block info

Name: GenICamSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | various | one or more |

#### The sample pipeline

```mermaid
graph LR;
    GenICamSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var sourceSettings = new GenICamSourceSettings(cbCamera.Text, new VisioForge.Core.Types.Rect(0, 0, 512, 512), 15, GenICamPixelFormat.Mono8); 
var source = new GenICamSourceBlock(sourceSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Sample applications

- [GenICam Source Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/GenICam%20Source%20Demo)

Read more about the [GenICam Source](../../videocapture/video-sources/usb3v-gige-genicam/index.md).

#### Platforms

Windows, macOS, Linux

### SRT Source Block (with decoding)

The `Secure Reliable Transport (SRT)` is an open-source video streaming protocol designed for secure and low-latency delivery over unpredictable networks, like the public internet. Developed by Haivision, SRT optimizes streaming performance by dynamically adapting to varying bandwidths and minimizing the effects of packet loss. It incorporates AES encryption for secure content transmission. Primarily used in broadcasting and online streaming, SRT is crucial for delivering high-quality video feeds in real-time applications, enhancing viewer experiences even in challenging network conditions. It supports point-to-point and multicast streaming, making it versatile for diverse setups.

The SRT source block provides decoded video and audio streams from an SRT source.

#### Block info

Name: SRTSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed | 0+ |
| Output audio | Uncompressed | 0+ |

#### Settings

The `SRTSourceBlock` is configured using `SRTSourceSettings`. This class provides comprehensive options for SRT connections:

- `Uri` (string): The SRT URI (e.g., "srt://127.0.0.1:8888" or "srt://example.com:9000?mode=listener"). Default is "srt://127.0.0.1:8888".
- `Mode` (`SRTConnectionMode` enum): Specifies the SRT connection mode. Default is `Caller`. See `SRTConnectionMode` enum details below.
- `Passphrase` (string): The password for encrypted transmission.
- `PbKeyLen` (`SRTKeyLength` enum): The crypto key length for AES encryption. Default is `NoKey`. See `SRTKeyLength` enum details below.
- `Latency` (`TimeSpan`): The maximum accepted transmission latency (receiver side for caller/listener, or for both in rendezvous). Default is 125 milliseconds.
- `StreamId` (string): The stream ID for SRT access control.
- `LocalAddress` (string): The local address to bind to when in `Listener` or `Rendezvous` mode. Default `null` (any).
- `LocalPort` (uint): The local port to bind to when in `Listener` or `Rendezvous` mode. Default 7001.
- `Authentication` (bool): Whether to authenticate the connection. Default `true`.
- `AutoReconnect` (bool): Whether the source should attempt to reconnect if the connection fails. Default `true`.
- `KeepListening` (bool): If `false` (default), the element will signal end-of-stream when the remote client disconnects (in listener mode). If `true`, it keeps waiting for reconnection.
- `PollTimeout` (`TimeSpan`): Polling timeout used when an SRT poll is started. Default 1000 milliseconds.
- `WaitForConnection` (bool): If `true` (default), blocks the stream until a client connects (in listener mode).

The `SRTSourceSettings` can be initialized using `await SRTSourceSettings.CreateAsync(string uri, bool ignoreMediaInfoReader = false)`. Setting `ignoreMediaInfoReader` to `true` can be useful if media info reading fails for a live stream.

##### `SRTConnectionMode` Enum

Defines the operational mode for an SRT connection:

- `None` (0): No connection mode specified (should not typically be used directly).
- `Caller` (1): The source initiates the connection to a listener.
- `Listener` (2): The source waits for an incoming connection from a caller.
- `Rendezvous` (3): Both ends initiate connection to each other simultaneously, useful for traversing firewalls.

##### `SRTKeyLength` Enum

Defines the key length for SRT's AES encryption:

- `NoKey` (0) / `Length0` (0): No encryption is used.
- `Length16` (16): 16-byte (128-bit) AES encryption key.
- `Length24` (24): 24-byte (192-bit) AES encryption key.
- `Length32` (32): 32-byte (256-bit) AES encryption key.

#### The sample pipeline

```mermaid
graph LR;
    SRTSourceBlock-->VideoRendererBlock;
    SRTSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// SRTSourceSettings has only a private constructor; use the async factory.
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri(edURL.Text));
var source = new SRTSourceBlock(srtSettings);
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.VideoOutput, videoRenderer.Input);
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Sample applications

- [SRT Source Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/SRT%20Source%20Demo)

#### Platforms

Windows, macOS, Linux, iOS, Android.

### SRT RAW Source Block

`The Secure Reliable Transport (SRT)` is a streaming protocol that optimizes video data delivery over unpredictable networks, like the Internet. It is open-source and designed to handle high-performance video and audio streaming. SRT provides security through end-to-end encryption, reliability by recovering lost packets, and low latency, which is suitable for live broadcasts. It adapts to varying network conditions by dynamically managing bandwidth, ensuring high-quality streams even under suboptimal conditions. Widely used in broadcasting and streaming applications, SRT supports interoperability and is ideal for remote production and content distribution.

The SRT source supports connection to SRT sources and provides a data stream. You can connect this block to `DecodeBinBlock` to decode the stream.

#### Block info

Name: SRTRAWSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output data | Any | one |

#### Settings

The `SRTRAWSourceBlock` is configured using `SRTSourceSettings`. Refer to the detailed description of `SRTSourceSettings` and its related enums (`SRTConnectionMode`, `SRTKeyLength`) under the `SRT Source Block (with decoding)` section for all available properties and their explanations.

#### The sample pipeline

```mermaid
graph LR;
    SRTRAWSourceBlock-->DecodeBinBlock;
    DecodeBinBlock-->VideoRendererBlock;
    DecodeBinBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// SRTSourceSettings has only a private constructor; use the async factory.
var srtSettings = await SRTSourceSettings.CreateAsync(new Uri(edURL.Text));
var source = new SRTRAWSourceBlock(srtSettings);
var decodeBin = new DecodeBinBlock();
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
var audioRenderer = new AudioRendererBlock();

pipeline.Connect(source.Output, decodeBin.Input);
pipeline.Connect(decodeBin.VideoOutput, videoRenderer.Input);
pipeline.Connect(decodeBin.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Other Source Blocks

### Screen Source Block

Screen source supports recording video from the screen. You can select the display (if more than one), the part of the screen to be recorded, and optional mouse cursor recording.

#### Settings

The `ScreenSourceBlock` uses platform-specific settings classes. The choice of settings class determines the underlying screen capture technology. The `ScreenCaptureSourceType` enum indicates the available technologies:

##### Windows

- `ScreenCaptureDX9SourceSettings` - Use `DirectX 9` for screen recording. (`ScreenCaptureSourceType.DX9`)
- `ScreenCaptureD3D11SourceSettings` - Use `Direct3D 11` Desktop Duplication for screen recording. Allows specific window capture. (`ScreenCaptureSourceType.D3D11DesktopDuplication`)
- `ScreenCaptureGDISourceSettings` - Use `GDI` for screen recording. (`ScreenCaptureSourceType.GDI`)

##### macOS

`ScreenCaptureMacOSSourceSettings` - Use `AVFoundation` for screen recording. (`ScreenCaptureSourceType.AVFoundation`)

##### Linux

`ScreenCaptureXDisplaySourceSettings` - Use `X11` (XDisplay) for screen recording. (`ScreenCaptureSourceType.XDisplay`)

##### iOS

`IOSScreenSourceSettings` - Use `AVFoundation` for current window/app recording. (`ScreenCaptureSourceType.IOSScreen`)

#### Block info

Name: ScreenSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    ScreenSourceBlock-->H264EncoderBlock;
    H264EncoderBlock-->MP4SinkBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// create source settings
var screenSourceSettings = new ScreenCaptureDX9SourceSettings() { FrameRate = 15 };

// create source block
var screenSourceBlock = new ScreenSourceBlock(screenSourceSettings);

// create video encoder block and connect it to the source block
var h264EncoderBlock = new H264EncoderBlock(new OpenH264EncoderSettings());
pipeline.Connect(screenSourceBlock.Output, h264EncoderBlock.Input);

// create MP4 sink block and connect it to the encoder block
var mp4SinkBlock = new MP4SinkBlock(new MP4SinkSettings(@"output.mp4"));
pipeline.Connect(h264EncoderBlock.Output, mp4SinkBlock.CreateNewInput(MediaBlockPadMediaType.Video));

// run pipeline
await pipeline.StartAsync();
```

#### [Windows] Window capture

You can capture a specific window by using the `ScreenCaptureD3D11SourceSettings` class.

```csharp
// create Direct3D11 source
var source = new ScreenCaptureD3D11SourceSettings();

// set frame rate
source.FrameRate = new VideoFrameRate(30);

// get handle of the window
var wih = new System.Windows.Interop.WindowInteropHelper(this);
source.WindowHandle = wih.Handle;

// create source block using the D3D11 settings built above
var screenSourceBlock = new ScreenSourceBlock(source);

// other code is the same as above
```

#### Sample applications

- [Screen Capture Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Screen%20Capture)
- [Screen Capture Demo (MAUI)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/MAUI/ScreenCaptureMB)
- [Screen Capture Demo (iOS)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/iOS/ScreenCapture)

#### Platforms

Windows, macOS, Linux, iOS.

### Fallback Switch Source Block

The `FallbackSwitchSourceBlock` wraps a primary video source and automatically switches to fallback content when the main source fails or becomes unavailable — ideal for reliable live streaming pipelines.

#### Block info

Name: FallbackSwitchSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |
| Output audio | Uncompressed audio | 1 |

#### Supported main source types

The first constructor argument is `IVideoSourceSettings`. Supported implementations:

- `RTSPSourceSettings` — RTSP stream
- `SRTSourceSettings` — SRT stream
- `NDISourceSettings` — NDI source
- `RTMPSourceSettings` — RTMP stream
- `HTTPSourceSettings` — HTTP stream

#### Settings

**`FallbackSwitchSettings`**

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Enabled` | `bool` | `false` | Enable fallback switching |
| `Fallback` | `FallbackSwitchSettingsBase` | `null` | Fallback content configuration |
| `EnableVideo` | `bool` | `true` | Apply fallback to video |
| `EnableAudio` | `bool` | `true` | Apply fallback to audio |
| `MinLatencyMs` | `int` | `0` | Minimum latency before switching |
| `FallbackVideoCaps` | `string` | `null` | Video caps override (e.g., `"video/x-raw,width=1920,height=1080"`) |
| `FallbackAudioCaps` | `string` | `null` | Audio caps override |

**`FallbackSwitchSettingsBase`** properties (shared by all fallback types):

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `TimeoutMs` | `int` | `5000` | Milliseconds before switching to fallback |
| `RestartTimeoutMs` | `int` | `5000` | Milliseconds before retrying the main source |
| `RetryTimeoutMs` | `int` | `60000` | Milliseconds before stopping retries |
| `ImmediateFallback` | `bool` | `false` | Switch to fallback immediately on error |
| `RestartOnEos` | `bool` | `false` | Restart main source on end-of-stream |
| `ManualUnblock` | `bool` | `false` | Require manual `Unblock()` call to resume main source |

**Fallback content types** (set `Fallback` to one of):

- `StaticTextFallbackSettings` — text overlay with configurable font, color, and position
- `StaticImageFallbackSettings` — displays a static image (file path, scale, aspect ratio options)
- `MediaBlockFallbackSettings` — plays a fallback URI or custom GStreamer pipeline

**Diagnostic methods:**

- `GetStatus()` — returns current fallback/main source state as a string
- `GetStatistics()` — returns a dictionary of retry count and buffering statistics
- `Unblock()` — manually unblocks when `ManualUnblock` is enabled

#### The sample pipeline

```mermaid
graph LR;
    FallbackSwitchSourceBlock -- Video --> VideoRendererBlock;
    FallbackSwitchSourceBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// RTSPSourceSettings has a private constructor — build it via the CreateAsync factory.
var mainSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://camera.example.com/stream"),
    login: null,
    password: null,
    audioEnabled: true);

var fallbackSettings = new FallbackSwitchSettings
{
    Enabled = true,
    Fallback = new StaticTextFallbackSettings
    {
        Text = "Stream unavailable",
        BackgroundColor = SKColors.Black,
        TextColor = SKColors.White
    }
};

var source = new FallbackSwitchSourceBlock(mainSettings, fallbackSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Virtual Video Source Block

VirtualVideoSourceBlock is used to produce test video data in a wide variety of video formats. The type of test data is controlled by the settings.

#### Settings

The `VirtualVideoSourceBlock` is configured using `VirtualVideoSourceSettings`. Key properties:

- `Pattern` (`VirtualVideoSourcePattern` enum): Specifies the type of test pattern to generate. See `VirtualVideoSourcePattern` enum below for available patterns. Default is `SMPTE`.
- `Width` (int): Width of the output video (default 1280).
- `Height` (int): Height of the output video (default 720).
- `FrameRate` (`VideoFrameRate`): Frame rate of the output video (default 30fps).
- `Format` (`VideoFormatX` enum): Pixel format of the video (default `RGB`).
- `ForegroundColor` (`SKColor`): For patterns that use a foreground color (e.g., `SolidColor`), this property defines it (default `SKColors.White`).

Constructors:

- `VirtualVideoSourceSettings()`: Default constructor.
- `VirtualVideoSourceSettings(int width, int height, VideoFrameRate frameRate)`: Initializes with specified dimensions and frame rate.

##### `VirtualVideoSourcePattern` Enum

Defines the test pattern generated by `VirtualVideoSourceBlock`:

- `SMPTE` (0): SMPTE 100% color bars.
- `Snow` (1): Random (television snow).
- `Black` (2): 100% Black.
- `White` (3): 100% White.
- `Red` (4), `Green` (5), `Blue` (6): Solid colors.
- `Checkers1` (7) to `Checkers8` (10): Checkerboard patterns with 1, 2, 4, or 8 pixel squares.
- `Circular` (11): Circular pattern.
- `Blink` (12): Blinking pattern.
- `SMPTE75` (13): SMPTE 75% color bars.
- `ZonePlate` (14): Zone plate.
- `Gamut` (15): Gamut checkers.
- `ChromaZonePlate` (16): Chroma zone plate.
- `SolidColor` (17): A solid color, defined by `ForegroundColor`.
- `Ball` (18): Moving ball.
- `SMPTE100` (19): Alias for SMPTE 100% color bars.
- `Bar` (20): Bar pattern.
- `Pinwheel` (21): Pinwheel pattern.
- `Spokes` (22): Spokes pattern.
- `Gradient` (23): Gradient pattern.
- `Colors` (24): Various colors pattern.
- `SMPTERP219` (25): SMPTE test pattern, RP 219 conformant.

#### Block info

Name: VirtualVideoSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | uncompressed video | 1 |

#### The sample pipeline

```mermaid
graph LR;
    VirtualVideoSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Virtual Audio Source Block

VirtualAudioSourceBlock is used to produce test audio data in a wide variety of audio formats. The type of test data is controlled by the settings.

#### Settings

The `VirtualAudioSourceBlock` is configured using `VirtualAudioSourceSettings`. Key properties:

- `Wave` (`VirtualAudioSourceSettingsWave` enum): Specifies the type of audio waveform to generate. See `VirtualAudioSourceSettingsWave` enum below. Default `Sine`.
- `Format` (`AudioFormatX` enum): Audio sample format (default `S16LE`).
- `SampleRate` (int): Sample rate in Hz (default 48000).
- `Channels` (int): Number of audio channels (default 2).
- `Volume` (double): Volume of the test signal (0.0 to 1.0, default 0.8).
- `Frequency` (double): Frequency of the test signal in Hz (e.g., for Sine wave, default 440).
- `IsLive` (bool): Indicates if the source is live (default `true`).
- `ApplyTickRamp` (bool): Apply ramp to tick samples (default `false`).
- `CanActivatePull` (bool): Can activate in pull mode (default `false`).
- `CanActivatePush` (bool): Can activate in push mode (default `true`).
- `MarkerTickPeriod` (uint): Make every Nth tick a marker tick (for `Ticks` wave, 0 = no marker, default 0).
- `MarkerTickVolume` (double): Volume of marker ticks (default 1.0).
- `SamplesPerBuffer` (int): Number of samples in each outgoing buffer (default 1024).
- `SinePeriodsPerTick` (uint): Number of sine wave periods in one tick (for `Ticks` wave, default 10).
- `TickInterval` (`TimeSpan`): Distance between start of current and start of next tick (default 1 second).
- `TimestampOffset` (`TimeSpan`): An offset added to timestamps (default `TimeSpan.Zero`).

Constructor:

- `VirtualAudioSourceSettings(VirtualAudioSourceSettingsWave wave = VirtualAudioSourceSettingsWave.Ticks, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

##### `VirtualAudioSourceSettingsWave` Enum

Defines the waveform for `VirtualAudioSourceBlock`:

- `Sine` (0): Sine wave.
- `Square` (1): Square wave.
- `Saw` (2): Sawtooth wave.
- `Triangle` (3): Triangle wave.
- `Silence` (4): Silence.
- `WhiteNoise` (5): White uniform noise.
- `PinkNoise` (6): Pink noise.
- `SineTable` (7): Sine table.
- `Ticks` (8): Periodic Ticks.
- `GaussianNoise` (9): White Gaussian noise.
- `RedNoise` (10): Red (Brownian) noise.
- `BlueNoise` (11): Blue noise.
- `VioletNoise` (12): Violet noise.

#### Block info

Name: VirtualAudioSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | uncompressed audio | 1 |

#### The sample pipeline

```mermaid
graph LR;
    VirtualAudioSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var audioSourceBlock = new VirtualAudioSourceBlock(new VirtualAudioSourceSettings());
var videoSourceBlock = new VirtualVideoSourceBlock(new VirtualVideoSourceSettings());
                      
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoSourceBlock.Output, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSourceBlock.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Demuxer Source Block

The Demuxer Source Block is used to demultiplex local media files into their constituent elementary streams (video, audio, subtitles). It allows for selective rendering of these streams.

#### Block info

Name: DemuxerSourceBlock.

| Pin direction   | Media type         | Pins count  |
|-----------------|:--------------------:|:-----------:|
| Output video    | Depends on file    | 0 or 1      |
| Output audio    | Depends on file    | 0 or 1+     |
| Output subtitle | Depends on file    | 0 or 1+     |

#### The sample pipeline

```mermaid
graph LR;
    DemuxerSourceBlock -- Video Stream --> VideoRendererBlock;
    DemuxerSourceBlock -- Audio Stream --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create settings, ensure to await CreateAsync
var demuxerSettings = await DemuxerSourceSettings.CreateAsync(
    "path/to/your/video.mp4", 
    renderVideo: true, 
    renderAudio: true, 
    renderSubtitle: false);

if (demuxerSettings == null)
{
    Console.WriteLine("Failed to initialize demuxer settings. Ensure the file exists and is readable.");
    return;
}

var demuxerSource = new DemuxerSourceBlock(demuxerSettings);

// Setup video rendering if video is available and rendered
if (demuxerSettings.RenderVideo && demuxerSource.VideoOutput != null)
{
    var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control
    pipeline.Connect(demuxerSource.VideoOutput, videoRenderer.Input);
}

// Setup audio rendering if audio is available and rendered
if (demuxerSettings.RenderAudio && demuxerSource.AudioOutput != null)
{
    var audioRenderer = new AudioRendererBlock();
    pipeline.Connect(demuxerSource.AudioOutput, audioRenderer.Input);
}

// Start pipeline
await pipeline.StartAsync();
```

#### Sample applications

- No specific sample application link, but can be used in player-like scenarios.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Image Video Source Block

The Image Video Source Block generates a video stream from a static image file (e.g., JPG, PNG). It repeatedly outputs the image as video frames according to the specified frame rate.

#### Block info

Name: ImageVideoSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### The sample pipeline

```mermaid
graph LR;
    ImageVideoSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Create image video source settings
var imageSourceSettings = new ImageVideoSourceSettings("path/to/your/image.jpg"); // Replace with your image path
imageSourceSettings.FrameRate = new VideoFrameRate(10); // Output 10 frames per second
imageSourceSettings.IsLive = true; // Treat as a live source (optional)
// imageSourceSettings.NumBuffers = 100; // Optional: output only 100 frames then stop

var imageSource = new ImageVideoSourceBlock(imageSourceSettings);

// Create video renderer
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1); // Assuming VideoView1 is your display control

// Connect blocks
pipeline.Connect(imageSource.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();
```

#### Remarks

This block uses SkiaSharp for image decoding, so ensure necessary dependencies are met if not using a standard VisioForge package that includes it.

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Text Overlay Source Block

The `TextOverlaySourceBlock` generates a video stream displaying configurable text on a solid background. It uses GStreamer's `videotestsrc` and `textoverlay` elements internally. This block is useful as a placeholder or fallback source — for example, showing "No Signal" when a primary camera is unavailable.

#### Block info

Name: TextOverlaySourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |

#### Settings

The block is configured via `StaticTextFallbackSettings`:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Text` | `string` | `"Source Unavailable"` | Text to display |
| `FontFamily` | `string` | `"Arial"` | Font family name |
| `FontSize` | `float` | `24` | Font size in points |
| `FontStyle` | `FontStyle` | `Normal` | Font style (Normal, Bold, Italic, etc.) |
| `TextColor` | `SKColor` | White | Text color |
| `BackgroundColor` | `SKColor` | Black | Background fill color |
| `Position` | `SKPoint` | `(0.5, 0.5)` | Text anchor position as proportion (0–1) |
| `CenterAlign` | `bool` | `true` | Center-align the text |

#### The sample pipeline

```mermaid
graph LR;
    TextOverlaySourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new StaticTextFallbackSettings
{
    Text = "No Signal",
    FontSize = 36,
    TextColor = SKColors.White,
    BackgroundColor = SKColors.Black
};

var textSource = new TextOverlaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(textSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Video Mixer Source Block

The `VideoMixerSourceBlock` composites multiple video sources into a single output stream. Each input is placed at a configurable rectangle on the output canvas. During playback the block supports live repositioning, opacity fading, and chroma key removal per input.

The mixer backend is controlled by `VideoMixerType`: CPU (default), OpenGL, or Direct3D 11.

#### Block info

Name: VideoMixerSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |

#### Settings (`VideoMixerSourceSettings`)

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Output canvas width in pixels |
| `Height` | `int` | `1080` | Output canvas height in pixels |
| `FrameRate` | `VideoFrameRate` | `FPS_30` | Output frame rate |
| `MixerType` | `VideoMixerType` | `CPU` | Rendering backend (CPU, OpenGL, D3D11) |

Use `settings.Add(source, rect)` or `settings.Add(source, left, top, width, height)` to register each input source and its bounding rectangle on the canvas.

#### Runtime control

The block implements `IVideoMixerControl` for live updates:

- `Input_Move(id, rect, duration, startAlpha, endAlpha)` — animate position and opacity
- `Input_Update(stream)` — update stream properties
- `Input_UpdateChromaKeySettings(id, settings)` — change chroma key
- `StartFadeIn(id, duration)` / `StartFadeOut(id, duration)` — fade controls
- `Input_List()` — enumerate all input streams

#### The sample pipeline

```mermaid
graph LR;
    Source1-->VideoMixerSourceBlock;
    Source2-->VideoMixerSourceBlock;
    VideoMixerSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new VideoMixerSourceSettings(1920, 1080, VideoFrameRate.FPS_30);

// Add a file source on the left half
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(new Uri("source1.mp4")));
settings.Add(fileSource, new Rect(0, 0, 960, 1080));

// Add a webcam on the right half
var videoSource = new SystemVideoSourceBlock(videoSourceSettings);
settings.Add(videoSource, new Rect(960, 0, 960, 1080));

var mixer = new VideoMixerSourceBlock(pipeline, settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixer.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

`VideoMixerSourceBlock.IsAvailable()` returns `true` if the required GStreamer compositor elements are available.

#### Platforms

Windows, macOS, Linux, iOS, Android.

## Push Source Blocks

Push Source blocks allow you to feed media data (video, audio, JPEG images, or generic data) directly into the Media Blocks pipeline from your application code. This is useful when your media originates from a custom source, such as a proprietary capture device, a network stream not supported by built-in blocks, or procedurally generated content.

The behavior of push sources is generally controlled by common settings available through the `IPushSourceSettings` interface, implemented by specific push source settings classes:

- `IsLive` (bool): Indicates if the source is live. Defaults vary by type (e.g., `true` for audio/video).
- `DoTimestamp` (bool): If `true`, the block will attempt to generate timestamps for the pushed data.
- `StreamType` (`PushSourceStreamType` enum: `Stream` or `SeekableStream`): Defines the stream characteristics.
- `PushFormat` (`PushSourceFormat` enum: `Bytes`, `Time`, `Default`, `Automatic`): Controls how data is pushed (e.g., based on byte count or time).
- `BlockPushData` (bool): If `true`, the push operation will block until the data is consumed by the pipeline.

The specific type of push source is determined by the `PushSourceType` enum: `Video`, `Audio`, `Data`, `JPEG`.

### Push Video Source Block

Allows pushing raw video frames into the pipeline.

#### Block info

Name: PushVideoSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### Settings

Configured using `PushVideoSourceSettings`:

- `Width` (int): Width of the video frames.
- `Height` (int): Height of the video frames.
- `FrameRate` (`VideoFrameRate`): Frame rate of the video.
- `Format` (`VideoFormatX` enum): Pixel format of the video frames (e.g., `RGB`, `NV12`, `I420`).
- Inherits common push settings like `IsLive` (defaults to `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushVideoSourceSettings(int width, int height, VideoFrameRate frameRate, VideoFormatX format = VideoFormatX.RGB)`

#### The sample pipeline

```mermaid
graph LR;
    PushVideoSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
    PushVideoSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configure push video source
var videoPushSettings = new PushVideoSourceSettings(
    width: 640, 
    height: 480, 
    frameRate: new VideoFrameRate(30), 
    format: VideoFormatX.RGB);
// videoPushSettings.IsLive = true; // Default

var videoPushSource = new PushVideoSourceBlock(videoPushSettings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(videoPushSource.Output, videoRenderer.Input);

await pipeline.StartAsync();

// Push frames from your application:
// var frame = new VideoFrameX(...);
// videoPushSource.PushFrame(frame);
// videoPushSource.ClearQueue(); // Clear buffered frames if needed
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Push Audio Source Block

Allows pushing raw audio samples into the pipeline.

#### Block info

Name: PushAudioSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output audio  | Uncompressed audio | 1          |

#### Settings

Configured using `PushAudioSourceSettings`:

- `SampleRate` (int): Sample rate of the audio (e.g., 44100, 48000).
- `Channels` (int): Number of audio channels (e.g., 1 for mono, 2 for stereo).
- `Format` (`AudioFormatX` enum): Format of the audio samples (e.g., `S16LE` for 16-bit signed little-endian PCM).
- Inherits common push settings like `IsLive` (defaults to `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushAudioSourceSettings(bool isLive = true, int sampleRate = 48000, int channels = 2, AudioFormatX format = AudioFormatX.S16LE)`

#### The sample pipeline

```mermaid
graph LR;
    PushAudioSourceBlock-->AudioEncoderBlock-->MP4SinkBlock;
    PushAudioSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configure push audio source
var audioPushSettings = new PushAudioSourceSettings(
    isLive: true, 
    sampleRate: 44100, 
    channels: 2, 
    format: AudioFormatX.S16LE);

var audioPushSource = new PushAudioSourceBlock(audioPushSettings);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioPushSource.Output, audioRenderer.Input);

await pipeline.StartAsync();

// Push audio data from your application:
// var frame = new AudioFrame(...);
// audioPushSource.PushData(frame);
// Or raw bytes: audioPushSource.PushData(audioData, audioData.Length);
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Push Data Source Block

Allows pushing generic byte data into the pipeline. The interpretation of this data depends on the `Caps` (capabilities) specified.

#### Block info

Name: `PushSourceBlock` (configured for data).

| Pin direction | Media type | Pins count |
|---------------|:----------:|:----------:|
| Output data   | Custom     | 1          |

#### Settings

Configured using `PushDataSourceSettings`:

- `Caps` (`Gst.Caps`): GStreamer capabilities string describing the data format (e.g., "video/x-h264, stream-format=byte-stream"). This is crucial for downstream blocks to understand the data.
- `PadMediaType` (`MediaBlockPadMediaType` enum): Specifies the type of the output pad (e.g., `Video`, `Audio`, `Data`, `Auto`).
- Inherits common push settings like `IsLive`, `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

#### The sample pipeline

```mermaid
graph LR;
    PushDataSourceBlock-->ParserOrDecoder-->Renderer;
```

Example: Pushing H.264 Annex B byte stream

```mermaid
graph LR;
    PushDataSourceBlock-->H264ParseBlock-->H264DecoderBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configure push data source for H.264 byte stream
var dataPushSettings = new PushDataSourceSettings();
dataPushSettings.Caps = new Gst.Caps("video/x-h264, stream-format=(string)byte-stream");
dataPushSettings.PadMediaType = MediaBlockPadMediaType.Video;
// dataPushSettings.IsLive = true; // Set if live

var dataPushSource = new PushSourceBlock(dataPushSettings);

// Example: Decode and render H.264 stream
var h264Parser = new H264ParseBlock();
var h264Decoder = new H264DecoderBlock(); // H.264 decoder (auto-selects a concrete IH264DecoderSettings)
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

pipeline.Connect(dataPushSource.Output, h264Parser.Input);
pipeline.Connect(h264Parser.Output, h264Decoder.Input);
pipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();

// In a separate thread or task, push H.264 NALUs:
// byte[] naluData = ... ; // Your H.264 NALU data
// dataPushSource.PushFrame(naluData);
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### Push JPEG Source Block

Allows pushing individual JPEG images, which are then output as a video stream.

#### Block info

Name: `PushSourceBlock` (configured for JPEG).

| Pin direction | Media type         | Pins count |
|---------------|:--------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### Settings

Configured using `PushJPEGSourceSettings`:

- `Width` (int): Width of the decoded JPEG images.
- `Height` (int): Height of the decoded JPEG images.
- `FrameRate` (`VideoFrameRate`): The frame rate at which the JPEG images will be presented as a video stream.
- Inherits common push settings like `IsLive` (defaults to `true`), `DoTimestamp`, `StreamType`, `PushFormat`, `BlockPushData`.

Constructor: `PushJPEGSourceSettings(int width, int height, VideoFrameRate frameRate)`

#### The sample pipeline

```mermaid
graph LR;
    PushJPEGSourceBlock-->VideoRendererBlock;
    PushJPEGSourceBlock-->VideoEncoderBlock-->MP4SinkBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Configure push JPEG source
var jpegPushSettings = new PushJPEGSourceSettings(
    width: 1280, 
    height: 720, 
    frameRate: new VideoFrameRate(10)); // Present JPEGs as a 10 FPS video

var jpegPushSource = new PushSourceBlock(jpegPushSettings);

// Example: Render the video stream from JPEGs
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(jpegPushSource.Output, videoRenderer.Input);

// Start pipeline
await pipeline.StartAsync();

// In a separate thread or task, push JPEG image data:
// byte[] jpegImageData = File.ReadAllBytes("image.jpg");
// jpegPushSource.PushFrame(jpegImageData); 
// Call PushFrame for each new JPEG image.
```

#### Platforms

Windows, macOS, Linux, iOS, Android.

### H264 Push Source Block

The H264 Push Source Block allows pushing raw H.264 encoded data directly into a Media Blocks pipeline for decoding and rendering. It wraps a GStreamer `appsrc` element configured for H.264 byte-stream input.

This block is designed for scenarios where you receive raw H.264 encoded data from an external source (such as an RTSP callback, a custom network transport, or a file reader) and need to decode and display it. It automatically handles:

- **AVC-to-byte-stream conversion**: RTSP and many other sources deliver H.264 in AVC format (4-byte length-prefixed NAL units). The block automatically converts this to Annex B byte-stream format (start codes) expected by GStreamer's H.264 parser.
- **PTS rebasing**: Presentation timestamps from external sources (e.g., RTSP cameras) are on the source's time base, which may be far from zero. The block rebases all timestamps relative to the first frame so the pipeline clock starts near zero, preventing display delays.

#### Block info

Name: H264PushSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | H.264 encoded (byte-stream) | 1 |

#### Properties

- `DoTimestamp` (bool): When `true`, the appsrc auto-generates timestamps using the pipeline clock (lowest latency, but breaks A/V sync). When `false` (default), the PTS provided to `PushData()` is used after rebasing. Must be set before pipeline start.

#### Methods

- `PushData(byte[] data, int length, TimeSpan timestamp)`: Push H.264 encoded data with a presentation timestamp. Handles AVC-to-byte-stream conversion and PTS rebasing automatically. Returns `FlowReturn` status.
- `PushData(byte[] data, int length)`: Push H.264 data without a timestamp (uses `TimeSpan.Zero`).
- `SendEOS()`: Signal end of stream to the pipeline.

#### The sample pipeline

The typical pipeline connects H264PushSourceBlock through a parser and decoder to a video renderer:

```mermaid
graph LR;
    H264PushSourceBlock-->H264ParseBlock;
    H264ParseBlock-->H264DecoderBlock;
    H264DecoderBlock-->VideoRendererBlock;
```

#### Two-pipeline architecture with RTSP callback

A common use case is the two-pipeline pattern: one pipeline captures raw H.264 from an RTSP camera, and a second pipeline decodes and displays it. This gives you access to the raw encoded bytes between capture and decode.

```mermaid
graph LR;
    subgraph "Pipeline 1 - Grabber"
        RTSPRAWSourceBlock-->BufferSinkBlock;
    end
    subgraph "Pipeline 2 - Player"
        H264PushSourceBlock-->H264ParseBlock;
        H264ParseBlock-->H264DecoderBlock;
        H264DecoderBlock-->VideoRendererBlock;
    end
    BufferSinkBlock-.PushData callback.->H264PushSourceBlock;
```

#### Sample code

```csharp
// === Pipeline 2: Player (decode and render) ===
var playerPipeline = new MediaBlocksPipeline();

// Create H264 push source - this is where raw H.264 data will be injected
var h264Source = new H264PushSourceBlock();

// Create parser, decoder, and renderer
var h264Parser = new H264ParseBlock();
var h264Decoder = new H264DecoderBlock(new FFMPEGH264DecoderSettings());
var videoRenderer = new VideoRendererBlock(playerPipeline, VideoView1);

// Connect: push source -> parser -> decoder -> renderer
playerPipeline.Connect(h264Source.Output, h264Parser.Input);
playerPipeline.Connect(h264Parser.Output, h264Decoder.Input);
playerPipeline.Connect(h264Decoder.Output, videoRenderer.Input);

// Preload the player pipeline (creates elements but stays in PAUSED state)
await playerPipeline.StartAsync(onlyPreload: true);

// === Pipeline 1: Grabber (RTSP capture) ===
var grabberPipeline = new MediaBlocksPipeline();

var rtspSettings = await RTSPRAWSourceSettings.CreateAsync(
    new Uri("rtsp://admin:password@192.168.1.64:554/stream"),
    "admin", "password");
rtspSettings.Latency = 50;

var rtspSource = new RTSPRAWSourceBlock(rtspSettings);
var bufferSink = new BufferSinkBlock();

// Wire up the callback to push raw H.264 data into the player pipeline
bufferSink.OnDataFrameBuffer += (sender, args) =>
{
    // Copy raw H.264 data from unmanaged memory
    var data = new byte[args.Size];
    System.Runtime.InteropServices.Marshal.Copy(args.Data, data, 0, args.Size);

    // Push into the player pipeline (AVC-to-byte-stream conversion is automatic)
    h264Source.PushData(data, args.Size, args.Timestamp);
};

grabberPipeline.Connect(rtspSource.Output, bufferSink.Input);

// Start grabber (begins RTSP connection and H.264 frame delivery)
await grabberPipeline.StartAsync();

// Wait for initial data to arrive, then resume the player
await Task.Delay(1000);
await playerPipeline.ResumeAsync();
```

#### Pipeline startup sequence

The startup order is critical for live streaming with two pipelines:

1. **Create and connect** the player pipeline blocks.
2. **Preload** the player pipeline (`StartAsync(onlyPreload: true)`) -- transitions to PAUSED, creates GStreamer elements and links pads.
3. **Create and start** the grabber pipeline -- RTSP connection is established, H.264 frames arrive via the callback.
4. **Push data** into the paused player pipeline -- `h264parse` accumulates SPS/PPS parameter sets.
5. **Resume** the player pipeline (`ResumeAsync()`) -- transitions to PLAYING, decoder starts producing frames.

#### AVC vs byte-stream format

RTSP sources typically deliver H.264 in AVC format where each NAL unit is prefixed with a 4-byte big-endian length:

```text
AVC:         [00 00 00 17][NAL data...][00 00 1C CC][NAL data...]
              length=23                 length=7372
```

GStreamer's `h264parse` expects Annex B byte-stream format with start codes:

```text
Byte-stream: [00 00 00 01][NAL data...][00 00 00 01][NAL data...]
              start code               start code
```

The `PushData()` method converts AVC to byte-stream automatically and in-place (both use 4-byte prefixes, so no buffer reallocation is needed). Data already in byte-stream format is detected and passed through unchanged.

#### Sample applications

- [RTSP RAW Camera H264 Callback Demo (WPF)](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/RTSP%20RAW%20Camera%20H264%20Callback%20Demo)

#### Platforms

Windows, macOS, Linux.

## Apple Platform Source Blocks

### iOS Video Source Block

iOSVideoSourceBlock provides video capture from the device camera on iOS platforms. It is available only on iOS (not macOS Catalyst).

#### Block info

Name: IOSVideoSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:------------------:|:----------:|
| Output video  | Uncompressed video | 1          |

#### Enumerate available devices

Use `DeviceEnumerator.Shared.VideoSourcesAsync()` to get a list of available video devices on iOS. Each device is represented by a `VideoCaptureDeviceInfo` object.

#### The sample pipeline

```mermaid
graph LR;
    IOSVideoSourceBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// select the first available video device
var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
VideoCaptureDeviceSourceSettings videoSourceSettings = null;
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        videoSourceSettings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };
        videoSourceSettings.Format.FrameRate = formatItem.FrameRateList[0];
    }
}

// create iOS video source block
var videoSource = new IOSVideoSourceBlock(videoSourceSettings);

// create video renderer block
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);

// connect blocks
pipeline.Connect(videoSource.Output, videoRenderer.Input);

// start pipeline
await pipeline.StartAsync();
```

#### Platforms

iOS (not available on macOS Catalyst)

---

### macOS Audio Source Block

OSXAudioSourceBlock provides audio capture from input devices on macOS platforms.

#### Block info

Name: OSXAudioSourceBlock.

| Pin direction | Media type         | Pins count |
|---------------|:------------------:|:----------:|
| Output audio  | Uncompressed audio | 1          |

#### Enumerate available devices

Use `DeviceEnumerator.Shared.AudioSourcesAsync()` to get a list of available audio devices on macOS. Each device is represented by an `AudioCaptureDeviceInfo` object.

#### The sample pipeline

```mermaid
graph LR;
    OSXAudioSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
// create pipeline
var pipeline = new MediaBlocksPipeline();

// select the first available audio device
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices.Length > 0 ? devices[0] : null;
OSXAudioSourceSettings audioSourceSettings = null;
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        audioSourceSettings = new OSXAudioSourceSettings(device.DeviceID, formatItem);
    }
}

// create macOS audio source block
var audioSource = new OSXAudioSourceBlock(audioSourceSettings);

// create audio renderer block
var audioRenderer = new AudioRendererBlock();

// connect blocks
pipeline.Connect(audioSource.Output, audioRenderer.Input);

// start pipeline
await pipeline.StartAsync();
```

#### Platforms

macOS (not available on iOS)

---

### Blu-Ray Source Block

The `BluRaySourceBlock` reads video, audio, and optional subtitle streams from a Blu-Ray disc or Blu-Ray disc image folder using GStreamer's `bluray` element.

#### Block info

Name: BluRaySourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 0 or 1 |
| Output audio | Uncompressed audio | 0 or 1 |
| Output subtitle | Subtitle | 0 or 1 |

#### Settings

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Location` | `string` | required | Path to Blu-Ray device or disc image folder |
| `Title` | `int` | `-1` | Title to play (−1 = main/longest title) |
| `RenderVideo` | `bool` | `true` | Enable video output pad |
| `RenderAudio` | `bool` | `true` | Enable audio output pad |
| `RenderSubtitle` | `bool` | `false` | Enable subtitle output pad |
| `AudioStream` | `int` | `-1` | Audio stream index (−1 = default) |
| `SubtitleStream` | `int` | `-1` | Subtitle stream index (−1 = default) |

#### The sample pipeline

```mermaid
graph LR;
    BluRaySourceBlock -- Video --> VideoRendererBlock;
    BluRaySourceBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new BluRaySourceSettings("/dev/sr0")
{
    Title = -1,
    RenderVideo = true,
    RenderAudio = true
};

var source = new BluRaySourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = BluRaySourceBlock.IsAvailable();
```

#### Platforms

Windows, macOS, Linux.

---

### DVB Source Block

The `DVBSourceBlock` captures a live digital television stream from a DVB adapter (DVB-T, DVB-C, DVB-S, DVB-S2, or ISDB-T). The block tunes to the specified channel and outputs a raw MPEG-TS stream.

#### Block info

Name: DVBSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | MPEG-TS stream | 1 |

#### Settings

`DVBSourceSettings` contains extensive tuning parameters. Core properties:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `AdapterID` | `int` | `0` | DVB adapter device index |
| `FrontendID` | `int` | `0` | Frontend device index |
| `DeliverySystem` | `DVBDeliverySystem` | Undefined | DVB-T / DVB-C / DVB-S / DVB-S2 / ISDB-T |
| `Frequency` | `uint` | `0` | Center frequency in Hz (or kHz for satellite) |
| `Pids` | `string` | `"8192"` | PID filter list (8192 = full transport stream) |
| `Modulation` | `DVBModulation` | QAM_16 | Modulation scheme |
| `SymbolRate` | `uint` | `0` | Symbol rate in kBd (satellite) |
| `BandwidthHZ` | `uint` | `0` | Channel bandwidth in Hz (DVB-T) |

Additional properties cover guard interval, transmission mode, hierarchy, code rates, LNB configuration, DiSEqC, ISDB-T layer parameters, and tuning timeouts.

Events fired during tuning: `TuningStart`, `TuningDone`, `TuningFail`.

#### The sample pipeline

```mermaid
graph LR;
    DVBSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Video --> VideoRendererBlock;
    DecodeBinBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new DVBSourceSettings
{
    AdapterID = 0,
    DeliverySystem = DVBDeliverySystem.DVBT,
    Frequency = 506000000,
    BandwidthHZ = 8000000
};

var dvb = new DVBSourceBlock(settings);
dvb.TuningDone += (s, e) => Console.WriteLine("Tuning complete");

// Decode the MPEG-TS stream
var decoder = new DecodeBinBlock();
pipeline.Connect(dvb.VideoOutput, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = DVBSourceBlock.IsAvailable();
```

#### Platforms

Windows, Linux.

---

### PulseAudio Source Block

The `PulseAudioSourceBlock` captures audio from a PulseAudio device on Linux. It implements `IAudioVolumeMute` for real-time volume and mute control.

#### Block info

Name: PulseAudioSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output audio | Uncompressed audio | 1 |

#### Settings

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Device` | `AudioCaptureDeviceInfo` | — | Capture device (from device enumerator) |
| `Format` | `AudioCaptureDeviceFormat` | — | Audio format (sample rate, channels, bit depth) |
| `DeviceName` | `string` | `null` | PulseAudio device name (null = system default) |
| `ClientName` | `string` | `"VisioForge"` | Name shown in audio control applications |
| `ProvideClock` | `bool` | `true` | Provide clock for the pipeline |

#### The sample pipeline

```mermaid
graph LR;
    PulseAudioSourceBlock-->AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Enumerate PulseAudio devices
var devices = await DeviceEnumerator.Shared.AudioSourcesAsync();
var device = devices[0];
var format = device.Formats[0];

var settings = new PulseAudioSourceSettings(device, format);
var audioSource = new PulseAudioSourceBlock(settings);

// Optional: control volume at runtime
audioSource.SetVolume(0.8);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(audioSource.Output, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = PulseAudioSourceBlock.IsAvailable();
```

#### Platforms

Linux.

---

### Raspberry Pi Source Block

The `RaspberryPiSourceBlock` captures video from the Raspberry Pi Camera Module using the `rpicamsrc` GStreamer element. It outputs H.264-encoded video and supports camera controls including rotation, flip, and image adjustments.

#### Block info

Name: RaspberryPiSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | H.264 video | 1 |

#### Settings

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `CameraNumber` | `int` | `0` | Camera index for multi-camera setups |
| `Width` | `uint` | `1920` | Capture width in pixels |
| `Height` | `uint` | `1080` | Capture height in pixels |
| `FrameRate` | `VideoFrameRate` | FPS_30 | Capture frame rate |
| `Bitrate` | `uint` | `17000000` | H.264 bitrate in bps |
| `Rotation` | `int` | `0` | Rotation in degrees: 0, 90, 180, 270 |
| `HorizontalFlip` | `bool` | `false` | Flip image horizontally |
| `VerticalFlip` | `bool` | `false` | Flip image vertically |
| `Brightness` | `int` | `50` | Brightness: 0–100 |
| `Contrast` | `int` | `0` | Contrast: −100 to 100 |
| `Saturation` | `int` | `0` | Saturation: −100 to 100 |
| `ISO` | `uint` | `0` | ISO sensitivity (0 = auto) |
| `ExposureMode` | `string` | `"auto"` | Exposure mode (auto, night, sports, etc.) |
| `AWBMode` | `string` | `"auto"` | Auto white balance mode |

#### The sample pipeline

```mermaid
graph LR;
    RaspberryPiSourceBlock-->H264DecoderBlock;
    H264DecoderBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new RaspberryPiSourceSettings
{
    Width = 1920,
    Height = 1080,
    FrameRate = VideoFrameRate.FPS_30,
    Rotation = 0,
    Bitrate = 17000000
};

var camera = new RaspberryPiSourceBlock(settings);

// Decode H.264 output before rendering
var decoder = new H264DecoderBlock();
pipeline.Connect(camera.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = RaspberryPiSourceBlock.IsAvailable();
```

#### Platforms

Linux (Raspberry Pi).

---

### GStreamer PlayBin Source Block

The `PlayBinSourceBlock` plays back media using GStreamer's built-in `playbin` element. It accepts the same `UniversalSourceSettings` as `UniversalSourceBlock` but delegates the entire decoding pipeline to GStreamer's high-level playback engine.

#### Block info

Name: PlayBinSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 0 or 1 |
| Output audio | Uncompressed audio | 0 or 1 |
| Output subtitle | Subtitle | 0 or 1 |

#### Settings

Uses `UniversalSourceSettings`. Key properties:

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | required | Media source URI |
| `RenderVideo` | `bool` | `true` | Enable video output pad |
| `RenderAudio` | `bool` | `true` | Enable audio output pad |
| `RenderSubtitle` | `bool` | `false` | Enable subtitle output pad |
| `StartPosition` | `TimeSpan?` | `null` | Playback start position |
| `StopPosition` | `TimeSpan?` | `null` | Playback stop position |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Override output frame rate |

#### The sample pipeline

```mermaid
graph LR;
    PlayBinSourceBlock -- Video --> VideoRendererBlock;
    PlayBinSourceBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new PlayBinSourceBlock(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, Android, iOS.

---

### Basic File Source Block

The `BasicFileSourceBlock` reads raw binary data from a file and exposes it as a single output pad. An optional type-find stage can detect the stream format automatically so it can be connected to a demuxer or decoder.

#### Block info

Name: BasicFileSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output | Auto (raw data) | 1 |

#### Parameters

| Parameter | Type | Default | Description |
| --- | --- | :---: | --- |
| `filename` | `string` | — | Path to the source file |
| `addTypeFind` | `bool` | `false` | Add a type-find element for automatic format detection |

#### The sample pipeline

```mermaid
graph LR;
    BasicFileSourceBlock-->DecodeBinBlock;
    DecodeBinBlock -- Video --> VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

// Read file with automatic type detection
var fileSource = new BasicFileSourceBlock("video.ts", addTypeFind: true);

var decoder = new DecodeBinBlock();
pipeline.Connect(fileSource.Output, decoder.Input);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(decoder.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = BasicFileSourceBlock.IsAvailable();
```

#### Platforms

Windows, macOS, Linux, Android, iOS.

---

### Custom Mixer Source Block

The `CustomMixerSourceBlock` is a push-based source that accepts externally provided video and audio frames via managed queues, exposing them as live output pads. It is designed for custom mixing and compositing pipelines where frames originate from application code.

#### Block info

Name: CustomMixerSourceBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |
| Output audio | Uncompressed audio | 1 |

#### Settings

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Width` | `int` | `1920` | Video width in pixels |
| `Height` | `int` | `1080` | Video height in pixels |
| `VideoFormat` | `VideoFormatX` | BGRA | Video pixel format |
| `FrameRate` | `VideoFrameRate` | 30 fps | Output frame rate |
| `AudioFormat` | `AudioFormatX` | S16LE | Audio sample format |
| `AudioSampleRate` | `int` | `48000` | Audio sample rate in Hz |
| `AudioChannels` | `int` | `2` | Number of audio channels |
| `IsLive` | `bool` | `true` | Mark source as live |
| `DoTimestamp` | `bool` | `true` | Automatically timestamp pushed buffers |
| `EnableThreadedPush` | `bool` | `true` | Push frames from a dedicated thread |
| `ManagedQueueMaxBuffers` | `int` | `4` | Maximum queued frames before dropping |

#### Events

- `QueueDropped` — fired when the managed queue drops a buffer due to backpressure.

#### The sample pipeline

```mermaid
graph LR;
    CustomMixerSourceBlock -- Video --> VideoRendererBlock;
    CustomMixerSourceBlock -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new CustomMixerSourceSettings
{
    Width = 1280,
    Height = 720,
    FrameRate = VideoFrameRate.FPS_30,
    AudioSampleRate = 48000,
    AudioChannels = 2
};

var mixerSource = new CustomMixerSourceBlock(settings);
mixerSource.QueueDropped += (s, e) => Console.WriteLine("Buffer dropped");

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(mixerSource.VideoOutput, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = CustomMixerSourceBlock.IsAvailable();
```

#### Platforms

Windows, macOS, Linux, Android, iOS.

---

### Image Video Source Cairo Block

The `ImageVideoSourceCairoBlock` renders a static or animated image (including animated GIFs) as a continuous video stream using the Cairo 2D graphics library. Unlike the standard image source, the output frame size is specified explicitly via a `Size` constructor parameter.

#### Block info

Name: ImageVideoSourceCairoBlock.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 1 |

#### Constructor parameters

| Parameter | Type | Description |
| --- | --- | --- |
| `pipeline` | `MediaBlocksPipeline` | Owning pipeline |
| `settings` | `ImageVideoSourceSettings` | Image source settings |
| `videoSize` | `Size` | Output frame dimensions in pixels |

#### Settings (ImageVideoSourceSettings)

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `Filename` | `string` | required | Path to the image file |
| `FrameRate` | `VideoFrameRate` | 10 fps | Output frame rate |
| `IsLive` | `bool` | `false` | Mark source as live |
| `NumBuffers` | `int` | `-1` | Frames to output (−1 = unlimited) |
| `AnimationInfiniteLoop` | `bool` | `true` | Loop animated images infinitely |

#### The sample pipeline

```mermaid
graph LR;
    ImageVideoSourceCairoBlock-->VideoRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new ImageVideoSourceSettings("image.png")
{
    FrameRate = VideoFrameRate.FPS_30,
    IsLive = true
};

var imageSource = new ImageVideoSourceCairoBlock(pipeline, settings, new Size(1920, 1080));

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(imageSource.Output, videoRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux.

---

### Universal Source Block Mini

The `UniversalSourceBlockMini` is a lightweight variant of `UniversalSourceBlock` using the same `UniversalSourceSettings`. It uses a simplified internal pipeline suited for low-resource environments or scenarios where the full decoding engine overhead is undesirable.

#### Block info

Name: UniversalSourceBlockMini.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 0 or 1 |
| Output audio | Uncompressed audio | 0 or 1 |
| Output subtitle | Subtitle | 0 or 1 |

#### Settings

Uses `UniversalSourceSettings` — the same class as `UniversalSourceBlock`.

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | required | Media source URI |
| `RenderVideo` | `bool` | `true` | Enable video output |
| `RenderAudio` | `bool` | `true` | Enable audio output |
| `RenderSubtitle` | `bool` | `false` | Enable subtitle output |
| `StartPosition` | `TimeSpan?` | `null` | Playback start position |
| `StopPosition` | `TimeSpan?` | `null` | Playback stop position |

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlockMini -- Video --> VideoRendererBlock;
    UniversalSourceBlockMini -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = await UniversalSourceSettings.CreateAsync(new Uri("file:///video.mp4"));
var source = new UniversalSourceBlockMini(settings);

var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Availability

```csharp
bool available = UniversalSourceBlockMini.IsAvailable();
```

#### Platforms

Windows, macOS, Linux, Android, iOS.

---

### Universal Source Block V2

The `UniversalSourceBlockV2` is an advanced multi-stream media source that exposes separate output pad lists for video, audio, subtitle, and metadata streams. It is suited for complex media files with multiple tracks and scenarios requiring per-stream decoder control.

#### Block info

Name: UniversalSourceBlockV2.

| Pin direction | Media type | Pins count |
| --- | :---: | :---: |
| Output video | Uncompressed video | 0…N |
| Output audio | Uncompressed audio | 0…N |
| Output subtitle | Subtitle | 0…N |
| Output metadata | Metadata | 0…N |

#### Settings (UniversalSourceSettingsV2)

| Property | Type | Default | Description |
| --- | --- | :---: | --- |
| `URI` | `Uri` | required | Media source URI |
| `RenderVideo` | `bool` | `true` | Enable video output pads |
| `RenderAudio` | `bool` | `true` | Enable audio output pads |
| `RenderSubtitle` | `bool` | `false` | Enable subtitle output pads |
| `StartPosition` | `TimeSpan?` | `null` | Playback start position |
| `StopPosition` | `TimeSpan?` | `null` | Playback stop position |
| `VideoCustomFrameRate` | `VideoFrameRate?` | `null` | Override output frame rate |

#### The sample pipeline

```mermaid
graph LR;
    UniversalSourceBlockV2 -- Video --> VideoRendererBlock;
    UniversalSourceBlockV2 -- Audio --> AudioRendererBlock;
```

#### Sample code

```csharp
var pipeline = new MediaBlocksPipeline();

var settings = new UniversalSourceSettingsV2(new Uri("file:///multi_track.mkv"))
{
    RenderVideo = true,
    RenderAudio = true
};

var source = new UniversalSourceBlockV2(settings);

// Access first video and audio streams
var videoRenderer = new VideoRendererBlock(pipeline, VideoView1);
pipeline.Connect(source.VideoOutput, videoRenderer.Input);

var audioRenderer = new AudioRendererBlock();
pipeline.Connect(source.AudioOutput, audioRenderer.Input);

await pipeline.StartAsync();
```

#### Platforms

Windows, macOS, Linux, Android, iOS.
