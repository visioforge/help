---
title: Video Editing Code Examples for C# and .NET Developers
description: Ready-to-use C# code examples for VisioForge Video Edit SDK .NET. Overlays, transitions, trimming, merging, and audio control with detailed tutorials.
sidebar_label: Code Examples
tags:
  - Video Edit SDK
  - .NET

---

# .NET Video Editing Code Examples & Tutorials

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }

This page collects ready-to-use C# recipes for the most common editing scenarios using the Video Edit SDK .Net. Every snippet is verified against the SDK source and demos under [`Video Edit SDK`](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Edit%20SDK). The recipes below use the Windows-only `VideoEditCore` engine. Cross-platform code based on `VideoEditCoreX` follows a similar shape with engine-specific source and effect types.

## Available Recipes

### Visual Effects & Overlays

- [**Adding Image Overlays to Video**](add-image-overlay.md) — Learn how to superimpose images on your video content
- [**Text Overlay Implementation**](add-text-overlay.md) — Techniques for adding and formatting text overlays on videos
- [**Picture-In-Picture Effects**](picture-in-picture.md) — Create professional PiP effects in your video applications

### Audio Manipulation

- [**Audio Volume Envelope Effects**](audio-envelope.md) — Control audio volume changes over time
- [**Multiple Audio Streams in AVI Files**](multiple-audio-streams-avi.md) — Working with multiple audio tracks
- [**Custom Volume Control for Audio Tracks**](volume-for-track.md) — Precise audio level management techniques

### Video Composition

- [**Creating Videos from Multiple Sources**](output-file-from-multiple-sources.md) — Combine various input files into a single output
- [**Working with Video Segments**](several-segments.md) — Extract and use multiple segments from the same source file
- [**Transition Effects Between Video Fragments**](transition-video.md) — Implementing smooth transitions between clips
- [**Generating Videos from Image Sequences**](video-images-console.md) — Console application example for image-to-video conversion
- [**Multi-Audio Stream Video Integration**](adding-video-file-with-multiple-audio-streams.md) — Working with complex audio-video combinations

## iOS

- [iOS Video Editor](ios-video-editor.md) — Building video editing apps for iPhone and iPad

## Recipe — Join Multiple Source Files into a Single MP4

`VideoSource` accepts a filename plus optional start/stop times. To trim a section from a source, pass the start and stop offsets; to use the whole file, pass `null`. Each call to `Input_AddVideoFile` appends to the timeline.

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task JoinClipsAsync(string output)
{
    var videoEdit = new VideoEditCore();

    // Output container.
    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // First clip — first 10 seconds.
    var clip1 = new VideoSource(
        @"intro.mp4",
        TimeSpan.Zero,
        TimeSpan.FromSeconds(10),
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip1);

    // Second clip — the whole file appended to the timeline.
    var clip2 = new VideoSource(
        @"content.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip2);

    // Third clip — the whole file appended.
    var clip3 = new VideoSource(
        @"outro.mp4",
        null,
        null,
        VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(clip3);

    // Run the engine.
    await videoEdit.StartAsync();
}
```

## Recipe — Add an Image Logo Overlay

`VideoEffectImageLogo` is the legacy image-overlay effect. Create it with a unique effect name (the second constructor argument), assign the image file via `Filename`, and add it to the engine. Position is controlled by `Left`/`Top` (in pixels) when no automatic alignment is used.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;
using VisioForge.Core.Types.VideoEffects;

public async Task AddLogoAsync(string source, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Image logo overlay (PNG with alpha is recommended for transparent logos).
    var logo = new VideoEffectImageLogo(enabled: true, name: "logo1")
    {
        Filename = "logo.png",
        Left = 10,
        Top = 10
    };
    videoEdit.Video_Effects_Add(logo);

    await videoEdit.StartAsync();
}
```

## Recipe — Replace the Source Audio With a Music Track

Add the video file as a video-only source (via `targetStreamIndex`-aware overloads of `Input_AddVideoFile` if needed), then add a separate `AudioSource` for the music track. The `AudioSource` constructor accepts a filename plus optional start/stop offsets.

```csharp
using VisioForge.Core.Types.VideoEdit;
using VisioForge.Core.VideoEdit;
using VisioForge.Core.Types.Output;

public async Task ReplaceAudioAsync(string source, string music, string output)
{
    var videoEdit = new VideoEditCore();

    videoEdit.Output_Filename = output;
    videoEdit.Output_Format = new MP4Output();

    // Source video (original audio is ignored by the engine when a separate
    // AudioSource is added; see Input_AddAudioFile overloads to control mixing).
    var video = new VideoSource(source, null, null, VideoEditStretchMode.Letterbox);
    await videoEdit.Input_AddVideoFileAsync(video);

    // Background music — full file.
    var music_src = new AudioSource(music);
    await videoEdit.Input_AddAudioFileAsync(music_src);

    await videoEdit.StartAsync();
}
```

## Additional Resources

Find more extensive code samples and resources on our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples), where we regularly update our collection with new examples and implementation techniques for .NET developers.
