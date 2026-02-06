---
title: Adding Multiple Segments from a Single Video File
description: Extract and combine multiple segments from the same video or audio file in C# with step-by-step guide and code examples for .NET.
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
    0,               // Rotation angle (0 = no rotation)
    1.0);            // Speed factor (1.0 = normal speed)
```

The VideoSource constructor takes several parameters:

- `videoFileName`: The path to your source video file
- `segments`: The array of FileSegment objects you defined in Step 1
- `VideoEditStretchMode`: How to handle aspect ratio differences (Letterbox, Stretch, Crop)
- Rotation angle (in degrees): Use 0 for no rotation, or 90, 180, 270 for rotated video
- Speed factor: Use 1.0 for normal speed, values below 1.0 for slow motion, above 1.0 for fast motion

### Step 3: Add to Timeline

Finally, add the segmented video source to your editing timeline:

```cs
// Add the segmented file to the timeline (track 0)
VideoEdit1.Input_AddVideoFile(videoFile, 0);
```

The `Input_AddVideoFile` method takes two parameters:

- `videoFile`: The VideoSource object you created
- `0`: The track number to place the video on (0 is typically the main video track)

## Working with Audio Segments

The same approach works for audio files. Simply use AudioSource instead of VideoSource:

```cs
// Define your audio segments
FileSegment[] audioSegments = new[] { 
    new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(8000)),
    new FileSegment(TimeSpan.FromMilliseconds(15000), TimeSpan.FromMilliseconds(12000))
};

// Create audio source with segments
AudioSource audioFile = new AudioSource(
    audioFileName,
    audioSegments,
    1.0);  // Speed factor

// Add to timeline (audio track 0)
VideoEdit1.Input_AddAudioFile(audioFile, 0);
```

## Advanced Usage Scenarios

### Variable Speed Segments

You can create interesting effects by varying the speed factor for different segments:

```cs
// Create segments with different speeds
VideoSource slowMotionSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(5000), TimeSpan.FromMilliseconds(3000)) },
    VideoEditStretchMode.Letterbox,
    0,
    0.5);  // Half speed (slow motion)

VideoSource fastForwardSegment = new VideoSource(
    videoFileName,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(10000), TimeSpan.FromMilliseconds(5000)) },
    VideoEditStretchMode.Letterbox,
    0,
    2.0);  // Double speed

// Add segments to different positions on the timeline
VideoEdit1.Input_AddVideoFile(slowMotionSegment, 0);
VideoEdit1.Input_AddVideoFile(fastForwardSegment, 0, TimeSpan.FromMilliseconds(3000));
```

### Combining Multiple Files with Segments

You can combine segments from different files by creating multiple VideoSource objects:

```cs
// Create segments from different files
VideoSource file1Segments = new VideoSource(
    videoFileName1,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(5000)) },
    VideoEditStretchMode.Letterbox,
    0,
    1.0);

VideoSource file2Segments = new VideoSource(
    videoFileName2,
    new[] { new FileSegment(TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(4000)) },
    VideoEditStretchMode.Letterbox,
    0,
    1.0);

// Add to timeline in sequence
VideoEdit1.Input_AddVideoFile(file1Segments, 0);
VideoEdit1.Input_AddVideoFile(file2Segments, 0, TimeSpan.FromMilliseconds(5000));
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