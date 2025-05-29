---
title: Creating Videos from Images in C# Console Apps
description: Learn how to programmatically generate video files from image sequences in C# console applications with step-by-step guidance, code examples, performance tips, and troubleshooting advice for .NET developers.
sidebar_label: Video from Images in a Console Application

---

# Creating Videos from Images in C# Console Applications

[!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge variant="dark" size="xl" text="VideoEditCore"]

## Introduction

Converting a sequence of images into a video file is a common requirement for many software applications. This guide demonstrates how to create a video from images using a C# console application with the Video Edit SDK .Net. The same approach works for WinForms and WPF applications with minimal modifications.

## Prerequisites

Before you begin, ensure you have:

- .NET development environment set up
- Video Edit SDK .Net installed
- Basic knowledge of C# programming
- A folder containing image files (JPG, PNG, etc.)

## Key Concepts

When creating videos from images, understanding these fundamental concepts will help you achieve better results:

- **Frame rate**: Determines how smoothly your video plays (typically 25-30 frames per second)
- **Image duration**: How long each image appears in the video
- **Transition effects**: Optional effects between images
- **Output format**: The video container and codec specifications
- **Resolution**: The dimensions of the output video

## Step-by-Step Implementation

### Setting Up the Project

First, create a new console application project and add the necessary references:

```cs
using System;
using System.IO;
using VisioForge.Types;
using VisioForge.Types.Output;
using VisioForge.VideoEdit;
using VisioForge.Controls;
using VisioForge.Controls.VideoEdit;
```

### Core Implementation

```cs
namespace ve_console
{
    class Program
    {
        // Folder contains images
        private const string AssetDir = "c:\\samples\\pics\\";

        static void Main(string[] args)
        {
            if (!Directory.Exists(AssetDir))
            {
                Console.WriteLine(@"Folder with images does not exists: " + AssetDir);
                return;
            }

            var images = Directory.GetFiles(AssetDir, "*.jpg");
            if (images.Length == 0)
            {
                Console.WriteLine(@"Folder with images is empty or do not have files with .jpg extension: " + AssetDir);
                return;
            }

            if (File.Exists(AssetDir + "output.avi"))
            {
                File.Delete(AssetDir + "output.avi");
            }

            var ve = new VideoEditCore();

            int insertTime = 0;

            foreach (string img in images)
            {
                ve.Input_AddImageFile(img, TimeSpan.FromMilliseconds(2000), TimeSpan.FromMilliseconds(insertTime), VideoEditStretchMode.Letterbox, 0, 640, 480);
                insertTime += 2000;
            }

            ve.Video_Effects_Clear();
            ve.Mode = VideoEditMode.Convert;

            ve.Video_Resize = true;
            ve.Video_Resize_Width = 640;
            ve.Video_Resize_Height = 480;

            ve.Video_FrameRate = 25;
            ve.Video_Renderer = new VideoRendererSettings
            {
                VideoRenderer = VideoRendererMode.None,
                StretchMode = VideoRendererStretchMode.Letterbox
            };

            var aviOutput = new AVIOutput
            {
                Video_Codec = "MJPEG Compressor"
            };

            ve.Output_Format = aviOutput;
            ve.Output_Filename = AssetDir + "output.avi";

            ve.Video_Effects_Enabled = true;
            ve.Video_Effects_Clear();

            ve.OnError += VideoEdit1_OnError;
            ve.OnProgress += VideoEdit1_OnProgress;

            ve.ConsoleUsage = true;

            ve.Start();

            Console.WriteLine(@"Video saved to: " + ve.Output_Filename);
        }

        private static void VideoEdit1_OnProgress(object sender, ProgressEventArgs progressEventArgs)
        {
            Console.WriteLine(progressEventArgs.Progress);
        }

        private static void VideoEdit1_OnError(object sender, ErrorsEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
```

## Detailed Component Breakdown

### Image Input Configuration

The code above uses `Input_AddImageFile()` to add each image to the timeline with specific parameters:

- **File path**: Path to the image file
- **Duration**: How long the image appears (2000ms in this example)
- **Start time**: When the image appears in the timeline
- **Stretch mode**: How the image fits the video frame (Letterbox preserves aspect ratio)
- **Rotation**: Image rotation in degrees
- **Width/Height**: Dimensions for the image in the video

### Video Output Settings

The output video settings are configured with these key properties:

- **Video_Resize**: Enable/disable resizing
- **Video_Resize_Width/Height**: Output video dimensions
- **Video_FrameRate**: Frames per second (25 FPS is standard for PAL)
- **Video_Renderer**: Rendering settings including mode and stretching
- **Output_Format**: Container format and codec settings
- **Output_Filename**: Where to save the resulting video file

### Progress and Error Handling

The implementation includes event handlers for monitoring progress and catching errors:

```cs
ve.OnError += VideoEdit1_OnError;
ve.OnProgress += VideoEdit1_OnProgress;
```

These handlers provide feedback during video creation, which is essential for longer operations.

## Advanced Customization Options

### Transition Effects

To add transitions between images, you can use the `Video_Transition_Add` method:

```cs
// Example of adding a fade transition between images

// Get the ID for the "FadeIn" transition effect
int transitionId = ve.Video_Transition_GetIDFromName("FadeIn");

// Add the transition - parameters are start time, end time, and transition ID
ve.Video_Transition_Add(
    TimeSpan.FromMilliseconds(1900),  // Start time of transition
    TimeSpan.FromMilliseconds(2100),  // End time of transition
    transitionId                      // Transition ID
);

// For more advanced transition options with border and other properties:
// ve.Video_Transition_Add(
//     TimeSpan.FromMilliseconds(1900),  // Start time
//     TimeSpan.FromMilliseconds(2100),  // End time
//     transitionId,                     // Transition ID
//     Color.Blue,                       // Border color
//     5,                                // Border softness
//     2,                                // Border width
//     0,                                // Offset X
//     0,                                // Offset Y
//     0,                                // Replicate X
//     0,                                // Replicate Y
//     1,                                // Scale X
//     1                                 // Scale Y
// );
```

## Performance Optimization Tips

- **Pre-resize images**: For better performance, resize images before processing
- **Batch processing**: Process images in smaller batches for large collections
- **Memory management**: Dispose of large objects when no longer needed
- **Output codec**: Choose codecs based on quality vs. processing speed requirements
- **Hardware acceleration**: Enable hardware acceleration when available

## Troubleshooting Common Issues

### Missing Codec Errors

If you encounter codec-related errors, ensure you have installed the required redistributables:

- Video Edit SDK redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

### Image Format Compatibility

Not all image formats are supported equally. For best results:

- Use common formats like JPG, PNG, or BMP
- Ensure consistent dimensions across images
- Test with a small subset before processing large collections

## Conclusion

Creating videos from images in a C# console application is straightforward with the right approach. This guide covered the essential implementation details, configuration options, and best practices to help you successfully integrate this functionality into your applications.

Remember to adjust the parameters to match your specific requirements, particularly the image duration, frame rate, and output format settings.

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for more code samples and implementations.
