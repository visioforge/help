---
title: Adding Image Overlays to Video Streams
description: Learn how to overlay images, animated GIFs, and transparent PNGs on video streams in .NET. Step-by-step guide with code examples for implementing image overlays using different formats and transparency effects.
sidebar_label: Image Overlay

---

# Image overlay

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="MediaPlayerCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

## Introduction

This example demonstrates how to overlay an image on a video stream.

JPG, PNG, BMP, and GIF images are supported.

## Sample code

Most simple image overlay with image added from a file with custom position:

```csharp
 var effect = new VideoEffectImageLogo(true, "imageoverlay");
 effect.Filename = @"logo.png";
 effect.Left = 100;
 effect.Top = 100;

 VideoCapture1.Video_Effects_Add(effect);
```

### Transparent image overlay

SDK fully supports transparency in PNG images. If you want to set a custom transparency level, you can use the `TransparencyLevel` property with a range (0..255).

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"logo.jpg";

effect.TransparencyLevel = 50;

VideoCapture1.Video_Effects_Add(effect);
```

### Animated GIF overlay

You can overlay an animated GIF image on a video stream. The SDK will play the GIF animation in the overlay.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.Filename = @"animated.gif";

effect.Animated = true;
effect.AnimationEnabled = true;

VideoCapture1.Video_Effects_Add(effect);
```

### Image overlay from `System.Drawing.Bitmap`

You can overlay an image from a `System.Drawing.Bitmap` object.

```csharp
var effect = new VideoEffectImageLogo(true, "imageoverlay");
effect.MemoryBitmap = new Bitmap("logo.jpg");
VideoCapture1.Video_Effects_Add(effect);
```

### Image overlay from RGB/RGBA byte array

You can overlay an image from RGB/RGBA data.

```csharp
// add image logo
var effect = new VideoEffectImageLogo(true, "imageoverlay");

// load image from JPG file
var bitmap = new Bitmap("logo.jpg");

// lock bitmap data and save to byte data (IntPtr)
var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
var pixels = Marshal.AllocCoTaskMem(bitmapData.Stride * bitmapData.Height);
NativeAPI.CopyMemory(pixels, bitmapData.Scan0, bitmapData.Stride * bitmapData.Height);
bitmap.UnlockBits(bitmapData);

// set data to effect
effect.Bitmap = pixels;

// set bitmap properties
effect.BitmapWidth = bitmap.Width;
effect.BitmapHeight = bitmap.Height;
effect.BitmapDepth = 3; // RGB24

// free bitmap
bitmap.Dispose();

// add effect
VideoCapture1.Video_Effects_Add(effect);
```

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
