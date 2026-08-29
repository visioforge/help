---
title: Add Text Overlay to Video in C# with Custom Fonts and Fade
description: Create dynamic text overlays with font, color, position, rotation, and animation control for timestamps, captions, and branding in .NET video.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - MediaPlayerCoreX
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Playback
  - Editing
  - Effects
  - C#
primary_api_classes:
  - TextOverlayVideoEffect
  - OverlayManagerText
  - FontSettings
  - VideoEffectTextLogo

---

# Implementing Text Overlays in Video Streams

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

[VideoCaptureCore](#){ .md-button } [MediaPlayerCore](#){ .md-button } [VideoEditCore](#){ .md-button }

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

// set Font - using object initializer
textOverlay.Font = new FontSettings
{
    Name = "Arial",
    Size = 24,
    Weight = FontWeight.Bold
};

// Alternative: using constructor with font face string
// textOverlay.Font = new FontSettings("Arial", "Bold", 24);

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

### Text That Changes on Every Frame

`TextOverlayVideoEffect` is built for text that rarely changes, and it has no time window. For a live
readout - a sensor value, the frame number, the clock - use `OverlayManagerText` through
`Video_Overlay_Add` and give it a `TextProvider`. It is asked for the text once per frame:

```csharp
// The overlay manager is only inserted into the pipeline when this is true,
// and it must be set before Start/StartAsync.
videoCapture1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 40, y: 40);
text.Color = SKColors.Yellow;
text.Font.Size = 28;

// Called once per frame on the streaming thread. The argument is the frame
// timestamp, counted from the start of the pipeline.
text.TextProvider = ts => "Camera 1\nOperator: demo\nREC\n"
    + $"Sensor {_sensorValue:F1}   {DateTime.Now:HH:mm:ss}   {ts:hh\\:mm\\:ss}";

videoCapture1.Video_Overlay_Add(text);
```

Static and dynamic lines live in one string, so a caption block with a single live line still costs
one callback per frame. Returning the same string as last time is cheap: the text layout is only
re-measured when the string actually differs.

The callback runs on the streaming thread, under the same lock `Video_Overlay_Add` and
`Video_Overlay_Remove` take. Keep it short and do not block in it - calling `Dispatcher.Invoke` from
inside it to read a UI value can deadlock, not merely drop a frame. Read a field the UI thread has
already written instead. A callback that throws is logged once and then not called again, and the
element falls back to its `Text`; assigning `TextProvider` again re-enables it.

`OverlayManagerText` also honours `StartTime` and `EndTime`. Either bound works on its own - a zero
`StartTime` means "from the beginning" and a zero `EndTime` means "no end" - and both are compared
against the frame timestamp, not the wall clock.

See the [OverlayManagerBlock page](../../mediablocks/VideoProcessing/OverlayManagerBlock.md) for the
full list of overlay elements.

`X` and `Y` are the top-left corner of the text, in pixels. `(0, 0)` is the top-left of the frame
and the first line of a multiline string is fully visible there.

### Preview-only overlays in MediaPlayerCoreX

The same `Video_Overlay_*` API is available on `MediaPlayerCoreX`. Set `Video_Overlay_Enabled`
before `OpenAsync` / `PlayAsync`. The overlay is inserted on the renderer branch only, after the
sample grabber and after any custom video outputs, so the file on disk, snapshots and exports are
not modified. Elements added with `Video_Overlay_Add` survive `Stop` and opening another file:

```csharp
mediaPlayer1.Video_Overlay_Enabled = true;

var text = new OverlayManagerText(string.Empty, x: 0, y: 0);
text.TextProvider = ts => $"T {ts:hh\\:mm\\:ss}";
mediaPlayer1.Video_Overlay_Add(text);
```

## Best Practices for Text Overlays

- Consider readability against different backgrounds
- Use appropriate font sizes for the target display resolution
- Implement fade effects for less intrusive overlays
- Test performance impact with complex text effects

---
For more code examples and implementation details, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).