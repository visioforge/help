---
title: Advanced Text Overlays for .NET Video Processing
description: Learn how to implement custom text overlays in video streams with complete control over font, size, color, position, rotation, and animation effects. Perfect for adding timestamps, captions, and dynamic text to your .NET video applications.
sidebar_label: Text Overlay

---

# Implementing Text Overlays in Video Streams

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="MediaPlayerCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

## Introduction

Text overlays provide a powerful way to enhance video streams with dynamic information, branding, captions, or timestamps. This guide explores how to implement fully customizable text overlays with precise control over appearance, positioning, and animations.

## Classic Engine Implementation

Our classic engines (VideoCaptureCore, MediaPlayerCore, VideoEditCore) offer a straightforward API for adding text to video streams.

### Basic Text Overlay Implementation

The following example demonstrates a simple text overlay with custom positioning:

```csharp
var effect = new VideoEffectTextLogo(true, "textoverlay");

// set position
effect.Left = 20;
effect.Top = 20;

// set Font (System.Drawing.Font)
effect.Font = new Font("Arial", 40);

// set text
effect.Text = "Hello, world!";

// set text color
effect.FontColor = Color.Yellow;

MediaPlayer1.Video_Effects_Add(effect);
```

### Dynamic Information Display Options

#### Timestamp and Date Display

You can automatically display current date, time, or video timestamp information using specialized modes:

```csharp
// set mode and mask
effect.Mode = TextLogoMode.DateTime;
effect.DateTimeMask = "yyyy-MM-dd. hh:mm:ss";
```

The SDK supports custom formatting masks for timestamps and dates, allowing precise control over the displayed information format. Frame number display requires no additional configuration.

### Animation and Transition Effects

#### Implementing Fade Effects

Create smooth text appearances and disappearances with customizable fade effects:

```csharp
// add the fade-in
effect.FadeIn = true; 
effect.FadeInDuration = TimeSpan.FromMilliseconds(5000);

// add the fade-out
effect.FadeOut = true;
effect.FadeOutDuration = TimeSpan.FromMilliseconds(5000);
```

### Text Rotation Options

Rotate your text overlay to match your design requirements:

```csharp
// set rotation mode
effect.RotationMode = TextRotationMode.Rm90;
```

### Text Flip Transformations

Apply mirror effects to your text for creative presentations:

```csharp
// set flip mode
effect.FlipMode = TextFlipMode.XAndY;
```

## X-Engine Implementation

Our newer X-engines (VideoCaptureCoreX, MediaPlayerCoreX, VideoEditCoreX) provide an enhanced API with additional features.

### Basic X-Engine Text Overlay

```csharp
// text overlay
var textOverlay = new TextOverlayVideoEffect() { Text = "Hello World!" };
 
// set position
textOverlay.XPad = 20;
textOverlay.YPad = 20;

textOverlay.HorizontalAlignment = TextOverlayHAlign.Left;
textOverlay.VerticalAlignment = TextOverlayVAlign.Top;

// set Font (System.Drawing.Font)
textOverlay.Font = new FontSettings("Arial", "Bold", 24);

// set text
textOverlay.Text = "Hello, world!";

// set text color
textOverlay.Color = SKColors.Yellow;

// add the effect
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

### Advanced Dynamic Content Display

#### Video Timestamp Integration

Display the current position within the video:

```csharp
// text overlay
var textOverlay = new TextOverlayVideoEffect();
  
// set text
textOverlay.Text = "Timestamp: ";

// set Timestamp mode
textOverlay.Mode = TextOverlayMode.Timestamp;

// add the effect
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

#### System Time Integration

Show the current system time alongside your video content:

```csharp
// text overlay
var textOverlay = new TextOverlayVideoEffect();
 
// set text
textOverlay.Text = "Time: ";

// set System Time mode
textOverlay.Mode = TextOverlayMode.SystemTime;

// add the effect
await videoCapture1.Video_Effects_AddOrUpdateAsync(textOverlay);
```

## Best Practices for Text Overlays

- Consider readability against different backgrounds
- Use appropriate font sizes for the target display resolution
- Implement fade effects for less intrusive overlays
- Test performance impact with complex text effects

---

For more code examples and implementation details, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
