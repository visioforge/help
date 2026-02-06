---
title: Adding Image Overlays to Video in .NET SDK
description: Implement image overlays in videos with .NET SDK using step-by-step guide with code examples for positioning, transparency, and timing.
---

# Adding Image Overlays to Videos in .NET

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introduction to Image Overlays

Our .NET SDK provides powerful functionality for adding image overlays to your video projects. With this feature, developers can seamlessly integrate logos, watermarks, graphics, and other visual elements into video content. The SDK offers extensive customization options including precise positioning, transparency adjustment, and timing control.

## Supported Image File Formats

The SDK is compatible with all standard image formats used in professional video production:

- BMP (Bitmap)
- GIF (Graphics Interchange Format)
- JPEG/JPG (Joint Photographic Experts Group)
- PNG (Portable Network Graphics)
- TIFF (Tagged Image File Format)

## Implementation Guide

Below you'll find detailed code examples demonstrating how to implement image overlays in your video processing applications using our SDK.

### Using the VideoEditCoreX Engine

The following code sample demonstrates how to add an image overlay with custom positioning, transparency, and timing using the VideoEditCoreX engine:

```cs
// add an image overlay to a video source effects from PNG file
var imageOverlay = new ImageOverlayVideoEffect("logo.png");

// set position
imageOverlay.X = 50;
imageOverlay.Y = 50;

// set alpha
imageOverlay.Alpha = 0.5;

// set start time and stop time
imageOverlay.StartTime = TimeSpan.FromSeconds(0);
imageOverlay.StopTime = TimeSpan.FromSeconds(5);

// add video source to timeline
VideoEdit1.Video_Effects.Add(imageOverlay);
```

### Using the VideoEditCore Engine

For developers working with the VideoEditCore engine, here's how to achieve the same functionality:

```cs
var effect = new VideoEffectImageLogo(true, name);

   // set position
   effect.Left = 50;
   effect.Top = 50;

   // set alpha (0..255)
   effect.TransparencyLevel = 127;

   // set start time and stop time
   effect.StartTime = TimeSpan.FromSeconds(5);
   effect.StopTime = TimeSpan.FromSeconds(15);

VideoEdit1.Video_Effects_Add(effect);
```

## Advanced Configuration Options

When implementing image overlays, consider these additional configuration options:

- **Positioning**: Adjust X/Y or Left/Top values to place your overlay precisely
- **Transparency**: Configure Alpha or TransparencyLevel to control overlay opacity
- **Timing**: Set StartTime and StopTime to determine when the overlay appears and disappears
- **Size**: You can resize overlays to fit your specific requirements

## Additional Resources

For more code examples and implementation guidance, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
