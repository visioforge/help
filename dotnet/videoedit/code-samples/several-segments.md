---
title: Extract and Combine Video Segments from a File in C#
description: Extract and combine multiple segments from a single video file using VisioForge Video Edit SDK .NET. Highlight reels and clip assembly in C#.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - C#
  - NuGet
primary_api_classes:
  - VideoSource
  - FileSegment
  - AudioSource

---

# Adding Multiple Segments from a Single Video File in C#

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction

When developing video editing applications, you often need to extract specific portions of a video file and combine them into a new composition. This technique is essential for creating highlight reels, removing unwanted sections, or assembling a compilation of key moments from a larger video.

This guide demonstrates how to programmatically extract and combine multiple segments from the same video file using C#. You'll learn the step-by-step process with working code examples that you can implement in your own applications.

## Why Extract Multiple Segments?

Extracting specific segments from videos serves many practical purposes:

- Creating highlight reels from longer recordings
- Removing unwanted sections (ads, errors, irrelevant content)
- Assembling a compilation of key moments
- Creating trailers or previews from full-length content
- Generating shorter clips for social media from longer videos

## Implementation Overview

The implementation involves three key steps:

1. Defining the time segments you want to extract
2. Creating a video source that includes these specified segments
3. Adding the segmented file to your editing timeline

Let's break down each step with detailed code examples and explanations.

## Detailed Implementation

### Step 1: Define Your Segments

First, you need to specify each segment's start and stop times. Each segment is defined by a starting point and duration, measured in milliseconds.

```cs
// Define multiple segments from a single video file
FileSegment[] segments = new[] { 
    new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)),  // First 5 seconds
    new FileSegment(TimeSpan.FromMilliseconds(3000), TimeSpan.FromMilliseconds(10000))  // From 3s to 13s mark
};
```

In this example, we've defined two segments:

- The first segment starts at the beginning of the video (0ms) and lasts for 5 seconds
- The second segment starts at the 3-second mark and continues for 10 seconds

Note that segments can overlap, as shown in this example where the second segment starts before the first one ends. This can be useful for creating smooth transitions or when you want certain portions to appear multiple times.

### Step 2: Create a Video Source with Segments

Next, create a VideoSource object that incorporates your defined segments:

```cs
// Create a video source that includes the specified segments
VideoSource videoFile = new VideoSource(
    videoFileName,   // Path to your video file
    segments,        // Array of segments defined above
    VideoEditStretchMode.Letterbox,  // How to handle aspect ratio differences
    0,               // streamNumber — which video stream to read from a multi-stream file
    1.0);            // rate — playback rate (1.0 = normal speed; 2.0 = 2×; 0.5 = half speed)
```

The VideoSource constructor takes several parameters:

- `videoFileName`: The path to your source video file
- `segments`: The array of FileSegment objects you defined in Step 1
- `VideoEditStretchMode`: How to handle aspect ratio differences (Letterbox, Stretch, Crop)
- `streamNumber`: Zero-based index of the video stream to use from a multi-stream file (not rotation)
- `rate`: Playback rate multiplier — 1.0 = normal, 0.5 = slow-mo, 2.0 = fast-forward

### Step 3: Add to Timeline

Finally, add the segmented video source to your editing timeline:

```cs
// Add the segmented file to the timeline.
// Signature: Input_AddVideoFile(VideoSource fileSource, TimeSpan? timelineInsertTime = null,
//   int targetVideoStream = 0, int customWidth = 0, int customHeight = 0).
// Pass only the source to append at the current end of the timeline.
VideoEdit1.Input_AddVideoFile(videoFile);

// Or splice the source at a specific timeline position:
// VideoEdit1.Input_AddVideoFile(videoFile, TimeSpan.FromSeconds(5));
```

The `Input_AddVideoFile` method takes the `VideoSource` plus an optional timeline-insert position (`TimeSpan?`, not an `int` track number). Additional optional parameters choose which video stream to consume from a multi-stream source and override custom width/height.

## Working with Audio Segments

The same approach works for audio files. Simply use AudioSource instead of VideoSource:

```cs
// Define your audio segments. FileSegment(startTime, stopTime) — stop must be greater than start.
FileSegment[] audioSegments = new[] {
    new FileSegment(TimeSpan.FromMilliseconds(0),     TimeSpan.FromMilliseconds(8000)),   // 0 → 8s
    new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(27000))   // 15s → 27s
};

// Create audio source with segments.
// Signature: AudioSource(string filename, FileSegment[] segments, string fileToSync = null,
//   int streamNumber = 0, double rate = 1.0).
// Position 3 is fileToSync (a *string*), not a speed factor — use a named arg for rate.
AudioSource audioFile = new AudioSource(
    audioFileName,
    audioSegments,
    rate: 1.0);

// Append to the timeline.
VideoEdit1.Input_AddAudioFile(audioFile);
```

## Advanced Usage Scenarios

### Variable Speed Segments

You can create interesting effects by varying the speed factor for different segments:

```cs
// Create segments with different speeds. FileSegment(start, stop) — stop must be > start.
VideoSource slowMotionSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(8000)) },  // 5s → 8s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    0.5);   // rate — half speed (slow motion)

VideoSource fastForwardSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(15000)) }, // 10s → 15s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    2.0);   // rate — double speed

// Add segments to the timeline. Position arg is TimeSpan? (insert point), not an int track.
VideoEdit1.Input_AddVideoFile(slowMotionSegment);
VideoEdit1.Input_AddVideoFile(fastForwardSegment, TimeSpan.FromMilliseconds(3000));
```

### Combining Multiple Files with Segments

You can combine segments from different files by creating multiple VideoSource objects:

```cs
// Create segments from different files. FileSegment(start, stop) — stop must be > start.
VideoSource file1Segments = new VideoSource(
    videoFileName1,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)) },  // 0 → 5s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    1.0);   // rate

VideoSource file2Segments = new VideoSource(
    videoFileName2,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(6000)) }, // 2s → 6s
    VideoEditStretchMode.Letterbox,
    0,      // streamNumber
    1.0);   // rate

// Add to timeline in sequence. Position arg is TimeSpan?, not an int.
VideoEdit1.Input_AddVideoFile(file1Segments);
VideoEdit1.Input_AddVideoFile(file2Segments, TimeSpan.FromMilliseconds(5000));
```

## Required Dependencies

To use this functionality, you'll need to install the appropriate redistributable packages:

- Video Edit SDK redistributables:
  - [x86 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [x64 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

For information about installing or deploying these redistributables to your users' PCs, see the [deployment guide](../deployment.md).

## Conclusion

Extracting and combining multiple segments from a video file is a powerful technique for creating dynamic video content in your applications. By following the steps outlined in this guide, you can implement this functionality in your C# applications with minimal effort.

This approach gives you fine-grained control over which portions of a video are included in your final output, allowing for creative editing possibilities without requiring complex manual video editing tools.

---
Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for more code samples and examples.