---
title: .Net Live Video Compositor
description: Master real-time video compositing in .Net. Add/remove multiple live video/audio sources and outputs on the fly. Build dynamic streaming & recording apps.
sidebar_label: Live Video Compositor
---

# Live Video Compositor

[!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

Live Video Compositor is a part of the [VisioForge Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net) that allows you to add and remove sources and outputs in real time to a pipeline.

This allows you to create applications that simultaneously handle multiple video and audio sources.

For example, the LVC allows you to start streaming to YouTube at just the right moment while simultaneously recording video to disk.
Using the LVC, you can create an application similar to OBS Studio.

Each source and output has its unique identifier that can be used to add and remove sources and outputs in real time.  

Each source and output has its own independent pipeline that can be started and stopped.

## Features

- Supports multiple video and audio sources
- Supports multiple video and audio outputs
- Setting the position and size of video sources
- Setting the transparency of video sources
- Setting the volume of audio sources

## LiveVideoCompositor class

The `LiveVideoCompositor` is the main class that allows the addition and removal of live sources and outputs to the pipeline. When creating it, it is necessary to specify the resolution and frame rate to use. All sources with a different frame rate will be automatically converted to the frame rate specified when creating the LVC.

`LiveVideoCompositorSettings` allows you to set the video and audio parameters. Key properties include:

- `MixerType`: Specifies the video mixer type (e.g., `LVCMixerType.OpenGL`, `LVCMixerType.D3D11` (Windows only), or `LVCMixerType.CPU`).
- `AudioEnabled`: A boolean indicating whether the audio stream is enabled.
- `VideoWidth`, `VideoHeight`, `VideoFrameRate`: Define the output video resolution and frame rate.
- `AudioFormat`, `AudioSampleRate`, `AudioChannels`: Define the output audio parameters.
- `VideoView`: An optional `IVideoView` for rendering video output directly.
- `AudioOutput`: An optional `AudioRendererBlock` for rendering audio output directly.

It is also necessary to set the maximum number of sources and outputs when designing your application, though this is not a direct parameter of `LiveVideoCompositorSettings`.

### Sample code

1. Create a new instance of the `LiveVideoCompositor` class.

```csharp
var settings = new LiveVideoCompositorSettings(1920, 1080, VideoFrameRate.FPS_25);
// Optionally, configure other settings like MixerType, AudioEnabled, etc.
// settings.MixerType = LVCMixerType.OpenGL;
// settings.AudioEnabled = true;
var compositor = new LiveVideoCompositor(settings);
```

2. Add video and audio sources and outputs (see below)
3. Start the pipeline.

```csharp
await compositor.StartAsync();
```

## LVC Video Input

The `LVCVideoInput` class is used to add video sources to the LVC pipeline. The class allows you to set the video parameters and the rectangle of the video source.

You can use any block that has a video output pad. For example, you can use `VirtualVideoSourceBlock` to create a virtual video source or `SystemVideoSourceBlock` to capture video from the webcam.

Key properties for `LVCVideoInput` include:

- `Rectangle`: Defines the position and size of the video source within the compositor's output.
- `ZOrder`: Determines the stacking order of overlapping video sources.
- `ResizePolicy`: Specifies how the video source should be resized if its aspect ratio differs from the target rectangle (`LVCResizePolicy.Stretch`, `LVCResizePolicy.Letterbox`, `LVCResizePolicy.LetterboxToFill`).
- `VideoView`: An optional `IVideoView` to preview this specific input source.

### Usage

When creating an `LVCVideoInput` object, you must specify the `MediaBlock` to be used as the video data source, along with `VideoFrameInfoX` describing the video, a `Rect` for its placement, and whether it should `autostart`.

### Sample code

#### Virtual video source

The sample code below shows how to create an `LVCVideoInput` object with a `VirtualVideoSourceBlock` as the video source.

```csharp
var rect = new Rect(0, 0, 640, 480);

var name = "Video source [Virtual]";
var settings = new VirtualVideoSourceSettings();
var info = new VideoFrameInfoX(settings.Width, settings.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new VirtualVideoSourceBlock(settings), info, rect, true);
// Optionally, set ZOrder or ResizePolicy
// src.ZOrder = 1;
// src.ResizePolicy = LVCResizePolicy.Letterbox;
if (await _compositor.Input_AddAsync(src))
{
    // added successfully
}
else
{
    src.Dispose();
}
```

#### Screen source

For Desktop platforms, we can capture the screen. The sample code below shows how to create an `LVCVideoInput` object with a `ScreenSourceBlock` as the video source.

```csharp
var settings = new ScreenCaptureDX9SourceSettings();
settings.CaptureCursor = true;
settings.Monitor = 0;
settings.FrameRate = new VideoFrameRate(30);
settings.Rectangle = new Rectangle(0, 0, 1920, 1080);

var rect = new Rect(0, 0, 640, 480);
var name = $"Screen source";
var info = new VideoFrameInfoX(settings.Rectangle.Width, settings.Rectangle.Height, settings.FrameRate);
var src = new LVCVideoInput(name, _compositor, new ScreenSourceBlock(settings), info, rect, true);
// Optionally, set ZOrder or ResizePolicy
// src.ZOrder = 0;
// src.ResizePolicy = LVCResizePolicy.Stretch;
if (await _compositor.Input_AddAsync(src))
{
    // added successfully
}
else
{ 
    src.Dispose(); 
}
```

#### System video source (webcam)

The sample code below shows how to create an `LVCVideoInput` object with a `SystemVideoSourceBlock` as the video source.

We use the `DeviceEnumerator` class to get the video source devices. The first video device will be used as the video source. The first video format of the device will be used as the video format.

```csharp
VideoCaptureDeviceSourceSettings settings = null;

var device = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
if (device != null)
{
    var formatItem = device.VideoFormats[0];
    if (formatItem != null)
    {
        settings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = formatItem.ToFormat()
        };

        settings.Format.FrameRate = dlg.FrameRate;
    }
}

if (settings == null)
{
    MessageBox.Show(this, "Unable to configure video capture device.");
    return;
}

var name = $"Camera source [{device.Name}]";
var rect = new Rect(0, 0, 1280, 720);
var videoInfo = new VideoFrameInfoX(settings.Format.Width, settings.Format.Height, settings.Format.FrameRate);
var src = new LVCVideoInput(name, _compositor, new SystemVideoSourceBlock(settings), videoInfo, rect, true);
// Optionally, set ZOrder or ResizePolicy
// src.ZOrder = 2;
// src.ResizePolicy = LVCResizePolicy.LetterboxToFill;

if (await _compositor.Input_AddAsync(src))
{
    // added successfully
}
else
{
    src.Dispose();
}
```

## LVC Audio Input

The `LVCAudioInput` class is used to add audio sources to the LVC pipeline. The class allows you to set the audio parameters and the volume of the audio source.

You can use any block that has an audio output pad. For example, you can use the `VirtualAudioSourceBlock` to create a virtual audio source or `SystemAudioSourceBlock` to capture audio from the microphone.

### Usage

When creating an `LVCAudioInput` object, you must specify the `MediaBlock` to be used as the audio data source, along with `AudioInfoX` (which requires format, channels, and sample rate) and whether it should `autostart`.

### Sample code

#### Virtual audio source

The sample code below shows how to create an `LVCAudioInput` object with a `VirtualAudioSourceBlock` as the audio source.

```csharp
var name = "Audio source [Virtual]";
var settings = new VirtualAudioSourceSettings();
var info = new AudioInfoX(settings.Format, settings.SampleRate, settings.Channels);
var src = new LVCAudioInput(name, _compositor, new VirtualAudioSourceBlock(settings), info, true);            
if (await _compositor.Input_AddAsync(src))
{
    // added successfully
}
else
{
    src.Dispose();
}
```

#### System audio source (DirectSound in Windows)

The sample code below shows how to create an `LVCAudioInput` object with a `SystemAudioSourceBlock` as the audio source.

We use the `DeviceEnumerator` class to get the audio devices. The first audio device is used as the audio source. The first audio format of the device is used as the audio format.

```csharp
DSAudioCaptureDeviceSourceSettings settings = null;
AudioCaptureDeviceFormat deviceFormat = null;

var device = (await DeviceEnumerator.Shared.AudioSourcesAsync(AudioCaptureDeviceAPI.DirectSound))[0]];
if (device != null)
{
    var formatItem = device.Formats[0];
    if (formatItem != null)
    {
        deviceFormat = formatItem.ToFormat();
        settings = new DSAudioCaptureDeviceSourceSettings(device, deviceFormat);
    }
}    

if (settings == null)
{
    MessageBox.Show(this, "Unable to configure audio capture device.");
    return;
}

var name = $"Audio source [{device.Name}]";
var info = new AudioInfoX(deviceFormat.Format, deviceFormat.SampleRate, deviceFormat.Channels);
var src = new LVCAudioInput(name, _compositor, new SystemAudioSourceBlock(settings), info, true);
if (await _compositor.Input_AddAsync(src))
{
    // added successfully
}
else
{
    src.Dispose();
}
```

## LVC Video Output

The `LVCVideoOutput` class is used to add video outputs to the LVC pipeline. You can start and stop the output pipeline independently from the main pipeline.

### Usage

When creating an `LVCVideoOutput` object, you must specify the `MediaBlock` to be used as the video data output, its `name`, a reference to the `LiveVideoCompositor`, and whether it should `autostart` with the main pipeline. An optional processing `MediaBlock` can also be provided. Usually, this element is used to save the video as a file or stream it (without audio).

For video+audio outputs, use the `LVCVideoAudioOutput` class.

You can use the SuperMediaBlock to make a custom block pipeline for video output. For example, you can add a video encoder, a muxer, and a file writer to save the video to a file.

## LVC Audio Output

The `LVCAudioOutput` class is used to add audio outputs to the LVC pipeline. You can start and stop the output pipeline independently from the main pipeline.

### Usage

When creating an `LVCAudioOutput` object, you must specify the `MediaBlock` to be used as the audio data output, its `name`, a reference to the `LiveVideoCompositor`, and whether it should `autostart`.

### Sample code

#### Add an audio renderer

Add an audio renderer to the LVC pipeline. You need to create an `AudioRendererBlock` object and then create an `LVCAudioOutput` object. Finally, add the output to the compositor.

The first device is used as an audio output.

```csharp
var audioRenderer = new AudioRendererBlock((await DeviceEnumerator.Shared.AudioOutputsAsync())[0]); 
var audioRendererOutput = new LVCAudioOutput("Audio renderer", _compositor, audioRenderer, true);
await _compositor.Output_AddAsync(audioRendererOutput, true);
```

#### Add an MP3 output

Add an MP3 output to the LVC pipeline. You need to create an `MP3OutputBlock` object and then create an `LVCAudioOutput` object. Finally, add the output to the compositor.

```csharp
var mp3Output = new MP3OutputBlock(outputFile, new MP3EncoderSettings());
var output = new LVCAudioOutput(outputFile, _compositor, mp3Output, false);

if (await _compositor.Output_AddAsync(output))
{
    // added successfully
}
else
{
    output.Dispose();
}
```

## LVC Video/Audio Output

The `LVCVideoAudioOutput` class is used to add video+audio outputs to the LVC pipeline. You can start and stop the output pipeline independently from the main pipeline.

### Usage

When creating an `LVCVideoAudioOutput` object, you must specify the `MediaBlock` to be used as the video+audio data output, its `name`, a reference to the `LiveVideoCompositor`, and whether it should `autostart`. Optional processing `MediaBlock`s for video and audio can also be provided.

### Sample code

#### Add an MP4 output

```csharp
var mp4Output = new MP4OutputBlock(new MP4SinkSettings("output.mp4"), new OpenH264EncoderSettings(), new MFAACEncoderSettings());

var output = new LVCVideoAudioOutput(outputFile, _compositor, mp4Output, false); 

if (await _compositor.Output_AddAsync(output))
{
    // added successfully
}
else
{
    output.Dispose();
}
```

#### Add a WebM output

```csharp
var webmOutput = new WebMOutputBlock(new WebMSinkSettings("output.webm"), new VP8EncoderSettings(), new VorbisEncoderSettings());
var output = new LVCVideoAudioOutput(outputFile, _compositor, webmOutput, false);

if (await _compositor.Output_AddAsync(output))
{
   // added successfully
}
else
{
    output.Dispose();
}
```

## LVC Video View Output

The `LVCVideoViewOutput` class is used to add video view to the LVC pipeline. You can use it to display the video on the screen.

### Usage

When creating an `LVCVideoViewOutput` object, you must specify the `IVideoView` control to be used, its `name`, a reference to the `LiveVideoCompositor`, and whether it should `autostart`. An optional processing `MediaBlock` can also be provided.

### Sample code

```csharp
var name = "[VideoView] Preview";
var videoRendererOutput = new LVCVideoViewOutput(name, _compositor, VideoView1, true);
await _compositor.Output_AddAsync(videoRendererOutput);
```

VideoView1 is a `VideoView` object that is used to display the video. Each platform / UI framework has its own `VideoView` implementation.

You can add several `LVCVideoViewOutput` objects to the LVC pipeline to display the video on different displays.

---

[Sample application on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Live%20Video%20Compositor%20Demo)
