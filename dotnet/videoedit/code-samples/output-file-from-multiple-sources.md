---
title: Create New Video Files from Multiple Sources in .NET
description: Combine multiple video and audio files into a single output without reencoding using C# with step-by-step guide for merging streams.
---

# Creating New Files from Multiple Sources Without Reencoding

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

When developing multimedia applications, you may need to combine content from different files. This guide demonstrates how to merge streams from multiple video and audio sources into a single output file without quality loss from reencoding.

## Benefits of Working with Multiple Sources

- Preserve original quality of all source files
- Combine audio tracks from different sources
- Add background music to video files
- Create multilingual content with different audio tracks
- Save processing time by avoiding unnecessary reencoding

## Step-by-Step Implementation

### 1. Initialize the Streams Collection

First, create a list to hold all the stream references:

```cs
var streams = new List();
```

### 2. Add Video Stream

Add a video stream from your first source file. The ID "v" designates this as the video component:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video.avi",
                ID = "v"
});
```

### 3. Add Primary Audio Stream

Incorporate an audio stream from an MP3 file. The ID "a" identifies this as an audio component:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!sophie.mp3",
                ID = "a"
});
```

### 4. Add Additional Audio Streams

You can add more audio streams from other video files. Again, use the ID "a" to specify this as an audio component:

```cs
streams.Add(new FFMPEGStream
{
                Filename = "c:\\samples\\!video2.avi",
                ID = "a"
});
```

### 5. Process and Generate Output

Finally, combine all streams into a single output file. Setting the second parameter to "true" ensures the output duration matches the shortest stream, preventing playback issues:

```cs
VideoEdit1.FastEdit_MuxStreams(streams, true, outputFile);
```

## Important Technical Considerations

When combining streams from multiple sources, keep in mind:

- Source formats must be compatible with the output container format
- Audio codec compatibility should be verified beforehand
- Stream synchronization may require additional configuration in complex scenarios
- Some players may have issues if stream durations vary significantly

## Required Dependencies

To implement this functionality, you'll need to reference:

- SDK redist
- FFMPEG redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.FFMPEG.x64/)

For more information on deploying these dependencies to end users, see [our deployment guide](../deployment.md).

## Further Resources

Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for additional code samples and implementation examples.
