---
title: Embed Multiple Audio Language Tracks into AVI in C# .NET
description: Embed multiple audio tracks into AVI files with VisioForge Video Edit SDK .NET. Multi-language and commentary support with C# code examples.
---

# Adding Multiple Audio Streams to AVI Files in .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

Multiple audio streams allow you to include different language tracks, commentary, or music options within a single video file. This functionality is essential for creating multilingual content or providing alternative audio experiences for viewers.

## Implementation Details

When creating multiple audio streams in an AVI file, you need to add each audio source to the timeline using specific targeting parameters. This approach ensures each audio stream is properly indexed and accessible during playback.

## Code Example

The following C# sample demonstrates how to add two different audio streams to an AVI file:

```cs
var videoSource = new VideoSource("video1.avi");
var audioSource1 = new AudioSource("video1.avi");
var audioSource2 = new AudioSource("audio2.mp3"); 

VideoEdit1.Input_Clear_List();
VideoEdit1.Input_AddVideoFile(videoSource);
VideoEdit1.Input_AddAudioFile(audioSource1, targetStreamIndex: 0);
VideoEdit1.Input_AddAudioFile(audioSource2, targetStreamIndex: 1);
```

## Key Parameters Explained

- `targetStreamIndex`: Defines which audio stream index the source will be assigned to
- First audio stream uses index 0, second uses index 1, and so on
- You can add as many audio streams as needed using incremental index values

## Required Dependencies

To implement this functionality, you'll need:

- Video Edit SDK redistributables:
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Deployment Information

For details on installing or deploying the required dependencies to end-user systems, refer to our [deployment guide](../deployment.md).

---
Find additional code examples and implementation details on our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).