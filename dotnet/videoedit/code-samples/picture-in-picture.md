---
title: Creating Dynamic Picture-in-Picture Videos in .NET
description: Implement Picture-in-Picture, side-by-side videos, and custom video layouts in C# with complete code samples for overlay positioning.
---

# Creating Picture-in-Picture and Split-Screen Videos in .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Introduction to Video Composition Techniques

Professional video applications often require combining multiple video sources into a single output. This capability is essential for creating engaging content such as tutorials with presenter overlays, reaction videos, interview panels, or sports broadcasts with replay windows. The Video Edit SDK for .NET makes these advanced techniques straightforward to implement in your C# applications.

This guide covers three primary video composition approaches:

1. **Picture-in-Picture (PIP)**: Placing a smaller video overlay on top of a main video
2. **Horizontal Split**: Positioning two videos side-by-side horizontally
3. **Vertical Split**: Arranging two videos one above the other

Each technique has specific use cases and can be customized to create exactly the visual presentation your application requires.

## Creating Picture-in-Picture Video Overlays

Picture-in-Picture is the most common video composition technique, where a smaller video appears in a corner or custom position over a larger background video. This is perfect for creating commentary videos, tutorials, or any content where you want to show two perspectives simultaneously without dividing the screen evenly.

### Step 1: Define Your Video Files

First, specify the file paths for the videos you want to combine:

```cs
string[] files = {  "!video.avi", "!video2.wmv" };
```

You can use various video formats including MP4, AVI, MOV, WMV and many others supported by the SDK.

### Step 2: Create Segments for the Main Video

Define which portion of the main video to use by setting start and stop times:

```cs
FileSegment[] segments1 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };
```

This example uses the first 10 seconds of the main video. You can adjust these values to use any segment of your source file.

### Step 3: Initialize the Main Video Source

Create a VideoSource object for your main video with your preferred settings:

```cs
var videoFile = new VideoSource(
                                files[0],
                                segments1,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

The parameters include:

- File path
- Time segments to include
- Stretch mode (how to handle aspect ratio differences)
- Rotation angle
- Volume multiplier

### Step 4: Configure the PIP Video Source

Similarly, set up the video that will appear as the overlay:

```cs
FileSegment[] segments2 = new[] { new FileSegment(TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10000)) };

var videoFile2 = new VideoSource(
                                files[1],
                                segments2,
                                VideoEditStretchMode.Letterbox,
                                0,
                                1.0);
```

### Step 5: Define Video Placement Rectangles

Specify the size and position for both videos:

```cs
// Rectangle for the main background video (full frame)
var rect1 = new Rectangle(0, 0, 1280, 720);

// Rectangle for the smaller PIP video overlay
var rect2 = new Rectangle(100, 100, 320, 240);
```

You can adjust the second rectangle's position and size to place the PIP video wherever you want on the screen. Common positions include top-right or bottom-left corners.

### Step 6: Combine the Videos with PIP

Finally, add both video sources to your project using the PIP mode:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile,          // Main video
    videoFile2,         // PIP video
    TimeSpan.FromMilliseconds(0),         // Start time
    TimeSpan.FromMilliseconds(10000),     // Duration
    VideoEditPIPMode.Custom,              // PIP mode
    true,                                // Show both videos
    1280, 720,                           // Output resolution
    0,                                   // Transition type
    rect2,                               // PIP video rectangle
    rect1                                // Main video rectangle
);
```

The resulting video will show your main video filling the entire frame with the second video appearing in the specified rectangle position.

## Creating Side-by-Side Video Layouts

Side-by-side layouts divide the screen evenly between two video sources. This approach works well for comparison videos, reaction content, or interview presentations where both videos deserve equal screen space.

### Horizontal Split Screen

A horizontal split places videos side-by-side horizontally. This works well for comparing before/after effects or showing two people in conversation:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Horizontal, 
    false
);
```

### Vertical Split Screen

A vertical split stacks videos one above the other. This can be useful for showing cause and effect relationships or creating top/bottom panel layouts:

```cs
VideoEdit1.Input_AddVideoFile_PIP(
    videoFile, 
    videoFile2, 
    TimeSpan.FromMilliseconds(0), 
    TimeSpan.FromMilliseconds(10000), 
    VideoEditPIPMode.Vertical, 
    false
);
```

## Advanced Customization Options

The SDK offers numerous options to further customize your video compositions:

- **Custom Positioning**: Define exact screen coordinates for precise video placement
- **Transitions**: Add fade or other transition effects between video segments
- **Audio Control**: Adjust volume levels for each video source independently
- **Border Effects**: Add borders, shadows, or frames around PIP video elements
- **Animation**: Create moving PIP elements that change position over time
- **Multiple Overlays**: Add more than two videos to create complex compositions

These capabilities allow you to create professional-level video productions directly from your .NET applications.

## Implementation Requirements

To successfully implement these video composition techniques in your application, you'll need to include the appropriate redistributable packages:

- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

For information on deploying these dependencies to your users' systems, see our [deployment guide](../deployment.md).

## Performance Considerations

When working with multiple video sources, especially at high resolutions, be mindful of system resource usage. Video composition operations can be processor-intensive. Consider the following best practices:

- Pre-render complex compositions for playback in production environments
- Optimize video resolution and bitrate for your target platform
- Test performance on hardware similar to your target deployment environment

## Conclusion

Picture-in-Picture and split-screen video compositions add professional capabilities to media applications. The Video Edit SDK for .NET makes implementing these features straightforward with its intuitive API. Whether you're developing a video editing application, a streaming platform, or integrating video processing into a larger system, these techniques provide powerful ways to combine and present multiple video sources.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.