---
title: Resize and Crop Operations in Video Capture SDK .Net
description: Learn how to implement professional video resizing and cropping in your .NET applications with optimized code examples for webcams, screen captures, IP cameras, and other video sources. Improve video quality with multiple resize algorithms.
sidebar_label: Video resize and crop
---

# Video Resize and Crop Operations for .NET Developers

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net)

## Introduction to Video Processing

When working with video streams in .NET applications, controlling the dimensions and focus area of your video is essential for creating professional applications. This guide explains how to implement resize and crop operations on video streams from webcams, screen captures, IP cameras, and other sources.

## Video Resizing Implementation

Resizing allows you to standardize video dimensions across different video sources, which is particularly useful when working with multiple camera inputs or when targeting specific output formats.

### Step 1: Enable Resize Functionality

First, enable the resize or crop feature in your application:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Step 2: Configure Resize Parameters

Set your desired width and height, and determine whether to maintain aspect ratio with letterboxing:

```cs
VideoCapture1.Video_Resize = new VideoResizeSettings
{
    Width = 640,
    Height = 480,
    LetterBox = true
};
```

### Step 3: Select Appropriate Resize Algorithm

Choose the algorithm that best fits your performance and quality requirements:

```cs
switch (cbResizeMode.SelectedIndex)
{
  case 0: VideoCapture1.Video_Resize.Mode = VideoResizeMode.NearestNeighbor; 
          break;
  case 1: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Bilinear; 
          break;
  case 2: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Bicubic; 
          break;
  case 3: VideoCapture1.Video_Resize.Mode = VideoResizeMode.Lancroz; 
          break;
}
```

## Video Cropping Implementation

Cropping allows you to focus on specific regions of interest in your video feed, removing unwanted areas from the frame.

### Step 1: Enable Crop Functionality

Similar to resizing, first enable the crop functionality:

```cs
VideoCapture1.Video_ResizeOrCrop_Enabled = true;
```

### Step 2: Define Crop Region

Specify the crop region by setting the margins to remove from each edge of the video frame:

```cs
VideoCapture1.Video_Crop = new VideoCropSettings(40, 0, 40, 0);
```

## Performance Considerations

When implementing resize and crop operations in production applications, consider the following:

- Resizing operations require CPU resources, especially at higher resolutions
- More complex algorithms (Bicubic, Lanczos) provide better quality but require more processing power
- For real-time applications, balance quality and performance based on your target hardware

## Required Dependencies

Ensure your project includes the necessary redistributable packages:

- Video capture redist [x86](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x86/) [x64](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoCapture.x64/)

## Additional Resources

For more advanced implementations and code samples, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) containing numerous examples for .NET developers.
