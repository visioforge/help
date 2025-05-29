---
title: Build Video Editing Apps with .NET SDK
description: Master video editing in .NET applications with our advanced SDK. Learn to create professional video editors with customizable timelines, multiple formats support, transitions, effects, and real-time previews. Follow our step-by-step guide for developers building robust media applications.
sidebar_label: Getting Started
sidebar_position: 0

---

# Building Professional Video Editing Applications with .NET SDK

## Introduction to Video Editing with .NET

The Video Edit SDK offers powerful functionality for .NET developers who want to create professional video editing applications. This SDK supports a wide range of platforms and UI frameworks, enabling you to build feature-rich video editors that handle multiple formats, apply effects, manage transitions, and deliver high-quality output.

## Setting Up Your Development Environment

### Creating Your Initial Project

The SDK is designed to work seamlessly with various development environments. You can utilize either Visual Studio or JetBrains Rider to create your project, selecting your preferred platform and UI framework according to your requirements.

For a smooth setup process, please refer to our detailed [installation guide](../install/index.md) which provides instructions for adding the necessary NuGet packages and setting up native dependencies correctly.

## Implementing Core Video Editing Functionality

### Initializing the Video Editing Engine

The SDK provides a robust core editing object that serves as the foundation of your video editing application. Follow these steps to create and initialize this essential component:

+++ VideoEditCore

```cs
private VideoEditCore core;

core = new VideoEditCore(VideoView1 as IVideoView);
```

+++ VideoEditCoreX

```cs
private VideoEditCoreX core;

core = new VideoEditCoreX(VideoView1 as IVideoView);
```

+++

You'll need to specify a Video View object as a parameter to enable video preview functionality during editing operations.

### Implementing Robust Event Handling

#### Error Event Management

For proper error management in your application, implement the `OnError` event handler:

```cs
core.OnError += Core_OnError;

private void Core_OnError(object sender, ErrorsEventArgs e)
{
    Debug.WriteLine("Error: " + e.Message);
}
```

#### Progress Tracking System

To keep your users informed about ongoing processes, implement the progress event handler:

```cs
core.OnProgress += Core_OnProgress;

private void Core_OnProgress(object sender, ProgressEventArgs e)
{
    Debug.WriteLine("Progress: " + e.Progress);
}
```

#### Operation Completion Notification

To detect when editing operations have completed, implement the stop event handler:

```cs
core.OnStop += Core_OnStop;

private void Core_OnStop(object sender, EventArgs e)
{
    Debug.WriteLine("Editing completed");
}
```

### Configuring Timeline Parameters

Before adding media sources to your project, you must establish the basic timeline parameters such as frame rate and resolution, which vary depending on which engine you're using:

+++ VideoEditCore

```cs
core.Video_FrameRate = new VideoFrameRate(30);
```

+++ VideoEditCoreX

```cs
core.Output_VideoSize = new VisioForge.Core.Types.Size(1920, 1080);
core.Output_VideoFrameRate = new VideoFrameRate(30);
```

+++

## Working with Media Sources

### Managing Video and Audio on Your Timeline

The SDK enables you to add various media sources to your timeline using straightforward API methods. For each source, you can precisely control parameters such as start time, end time, and position on the timeline. You have the flexibility to add video and audio sources either independently or together.

### Integrating Video Files

The SDK supports an extensive range of video formats including MP4, AVI, MOV, WMV, and many others. Here's how to add video content to your project:

First, create a video source object and set the source file path. You can specify start and end times in the constructor or use null parameters to include the entire file.

For files with multiple video streams, you can select which stream to use by specifying the stream number.

+++ VideoEditCore

```cs
var videoFile = new VisioForge.Core.Types.VideoEdit.VideoSource(
    filename,
    null,
    null, 
    VideoEditStretchMode.Letterbox, 
    0, 
    1.0);
```

API:

```cs
public VideoSource(
    string filename,
    TimeSpan? startTime = null,
    TimeSpan? stopTime = null,
    VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
    int streamNumber = 0,
    double rate = 1.0)
```

+++ VideoEditCoreX

```cs
var videoFile = new VisioForge.Core.Types.X.VideoEdit.VideoFileSource(
    filename,
    null,
    null, 
    0, 
    1.0);
```

API:

```cs
public VideoFileSource(
    string filename,
    TimeSpan? startTime = null,
    TimeSpan? stopTime = null,
    int streamNumber = 0,
    double rate = 1.0)
```

+++

You can control playback speed by adjusting the rate parameter. For example, setting the rate to 2.0 will play the file at twice the normal speed.

An alternative constructor allows you to add multiple file segments:

+++ VideoEditCore

```cs
public VideoSource(
    string filename,
    FileSegment[] segments,
    VideoEditStretchMode stretchMode = VideoEditStretchMode.Letterbox,
    int streamNumber = 0,
    double rate = 1.0)
```

+++ VideoEditCoreX

```cs
public VideoFileSource(
    string filename,
    FileSegment[] segments,
    int streamNumber = 0,
    double rate = 1.0)
```

+++

To add the source to your timeline, use the following methods:

+++ VideoEditCore

```cs
await core.Input_AddVideoFileAsync(
    videoFile, 
    null, 
    0);
```

The third parameter specifies the destination video stream number. Use 0 to add the source to the first video stream.

API:

```cs
public Task<bool> Input_AddVideoFileAsync(
    VideoSource fileSource,
    TimeSpan? timelineInsertTime = null,
    int targetVideoStream = 0,
    int customWidth = 0,
    int customHeight = 0)
```

+++ VideoEditCoreX

```cs
core.Input_AddVideoFile(
    videoFile,
    null);
```

API:

```cs
public bool Input_AddVideoFile(
    VideoFileSource source, 
    TimeSpan? insertTime = null)
```

+++

The second parameter determines the timeline position. Using null automatically adds the source at the end of the existing timeline.

### Incorporating Audio Files

The SDK supports numerous audio formats including AAC, MP3, WMA, OPUS, Vorbis and more. The process for adding audio is similar to adding video:

+++ VideoEditCore

```cs
var audioFile = new VisioForge.Core.Types.VideoEdit.AudioSource(
    filename,
    startTime: null,
    stopTime: null,
    fileToSync: string.Empty,
    streamNumber: 0,
    rate: 1.0);                      
```

The `fileToSync` parameter enables audio-video synchronization. When working with separate video and audio files, you can specify the video filename in this parameter to ensure the audio synchronizes correctly with the video.

```cs
await core.Input_AddAudioFileAsync(
    audioFile,
    insertTime: null, 
    0);
```

+++ VideoEditCoreX

```cs
var audioFile = new AudioFileSource(
    filename,
    startTime: null,
    stopTime: null);

core.Input_AddAudioFile(
    audioFile,
    insertTime: null);
```

+++

### Combining Video and Audio Sources

For efficiency, you can add combined video and audio sources with a single method call:

+++ VideoEditCoreX

```cs
core.Input_AddAudioVideoFile(
    filename,
    startTime: null,
    stopTime: null,
    insertTime: null);
```

+++

### Working with Static Images

The SDK supports adding still images to your timeline, including JPG, PNG, BMP, and GIF formats. When adding an image, you'll need to specify how long it should appear on the timeline:

+++ VideoEditCore

```cs
await core.Input_AddImageFileAsync(
    filename,
    duration: TimeSpan.FromMilliseconds(2000),
    timelineInsertTime: null,
    stretchMode: VideoEditStretchMode.Letterbox);
```

+++ VideoEditCoreX

```cs
core.Input_AddImageFile(
    filename,
    duration: TimeSpan.FromMilliseconds(2000),
    insertTime: null);
```

+++

## Configuring Output Settings

### Setting Output Format and Encoding Options

The SDK offers flexible output options with support for numerous video and audio formats, including MP4, AVI, WMV, MKV, WebM, AAC, MP3, and many others.

Use the `Output_Format` property to configure your desired output format:

+++ VideoEditCore

```cs
var mp4Output = new MP4HWOutput();
core.Output_Format = mp4Output;
```

+++ VideoEditCoreX

```cs
var mp4Output = new MP4Output("output.mp4");
core.Output_Format = mp4Output;
```

+++

For a comprehensive list of supported output formats and detailed code examples, please refer to our [Output Formats](../general/output-formats/index.md) documentation section.

## Enhancing Your Videos

### Applying Professional Video Effects

The SDK provides a rich collection of video effects that you can apply to enhance your video content. For detailed information on implementing effects, see our [Video Effects](../general/video-effects/index.md) guide.

The `VideoEditCoreX` engine includes dedicated API methods for adding text overlays. For implementation details, refer to our [Text Overlays](./code-samples/add-text-overlay.md) guide.

### Adding Smooth Transitions

To create professional-looking transitions between video clips, check our detailed [transition usage code sample](./code-samples/transition-video.md).

## Processing Your Video Project

### Starting the Editing Process

Once you've configured all your sources, effects, and output settings, you can initiate the editing process:

+++ VideoEditCore

```cs
await core.StartAsync();
```

+++ VideoEditCoreX

```cs
core.Start();
```

+++

During the editing process, your application will receive progress updates through the event handlers you've implemented, and you'll be notified when the operation completes via the stop event.

## Conclusion

By following this guide, you've learned the fundamental techniques for creating a powerful video editing application using the Video Edit SDK for .NET. This foundation will enable you to build sophisticated video editing tools that can compete with professional video editing software while being tailored to your specific requirements.
