---
title: Video Overlay Manager - Text, Image, PiP Layers in C# .NET
description: Add text, images, shapes, and PiP overlays to live video using VisioForge Media Blocks SDK OverlayManagerBlock with real-time layer management.
---

# Overlay Manager Block usage guide

## Overview

The `OverlayManagerBlock` is a powerful MediaBlocks component that provides dynamic multi-layer video overlay composition and management. It allows you to add various overlay elements (images, text, shapes, animations) on top of video content with real-time updates, layer management, and advanced features like shadows, rotation, and opacity control.

## Key Features

- **Multiple Overlay Types**: Text, scrolling text, images, image sequences, GIFs, SVG, shapes (rectangles, circles, triangles, stars, lines), video files/URLs, live video (NDI, Decklink), WebView2 web content (Windows), WPF controls (Windows)
- **Video File Overlays**: Play video files or stream URLs as overlays with full playback control and optional audio output
- **WPF Control Overlays**: Render live WPF elements with animations and data binding as video overlays (Windows only)
- **Overlay Groups**: Synchronize multiple overlays for coordinated start/stop with preloading support
- **Squeezeback Effects**: Scale video to a custom rectangle with overlay image on top (broadcast-style)
- **Video Transformations**: Zoom and pan effects that transform the entire video frame
- **Animation Support**: Animate video position/scale with easing functions
- **Fade Effects**: Fade in/out for video and overlay elements
- **Layer Management**: Z-index ordering for proper overlay stacking
- **Advanced Effects**: Shadows, rotation, opacity, custom positioning
- **Real-time Updates**: Dynamic overlay modification during playback
- **Time-based Display**: Show/hide overlays at specific timestamps
- **Custom Drawing**: Callback support for custom Cairo drawing operations
- **Live Video Sources**: Support for NDI network sources and Decklink capture cards
- **Cross-platform**: Works on Windows, Linux, macOS, iOS, and Android

## Class Reference

### OverlayManagerBlock

**Namespace**: `VisioForge.Core.MediaBlocks.VideoProcessing`

```csharp
public class OverlayManagerBlock : MediaBlock, IMediaBlockInternals
```

#### Properties

| Property | Type | Description |
|----------|------|-------------|
| `Type` | `MediaBlockType` | Returns `MediaBlockType.OverlayManager` |
| `Input` | `MediaBlockPad` | Video input pad |
| `Output` | `MediaBlockPad` | Video output pad with overlays |

#### Methods

##### Static Methods

```csharp
public static bool IsAvailable()
```

Checks if the overlay manager is available in the current environment (requires Cairo overlay support).

##### Instance Methods

```csharp
public void Video_Overlay_Add(IOverlayManagerElement overlay)
```

Adds a new overlay element to the video composition.

```csharp
public void Video_Overlay_Remove(IOverlayManagerElement overlay)
```

Removes a specific overlay element.

```csharp
public void Video_Overlay_RemoveAt(int index)
```

Removes an overlay at the specified index.

```csharp
public void Video_Overlay_Clear()
```

Removes all overlay elements.

```csharp
public void Video_Overlay_Update(IOverlayManagerElement overlay)
```

Updates an existing overlay (removes and re-adds with new properties).

## Overlay Element Types

### Common Properties (IOverlayManagerElement)

All overlay elements implement the `IOverlayManagerElement` interface with these common properties:

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Name` | `string` | - | Optional name for identification |
| `Enabled` | `bool` | `true` | Enable/disable the overlay |
| `StartTime` | `TimeSpan` | `Zero` | When to start showing (optional) |
| `EndTime` | `TimeSpan` | `Zero` | When to stop showing (optional) |
| `Opacity` | `double` | `1.0` | Transparency (0.0-1.0) |
| `Rotation` | `double` | `0.0` | Rotation angle in degrees (0-360) |
| `ZIndex` | `int` | `0` | Layer order (higher = on top) |
| `Shadow` | `OverlayManagerShadowSettings` | - | Shadow configuration |

### OverlayManagerText

Displays text with optional background and formatting.

```csharp
public class OverlayManagerText : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "Hello!!!" | Text to display |
| `X` | `int` | `100` | X position |
| `Y` | `int` | `100` | Y position |
| `Font` | `FontSettings` | System default | Font configuration |
| `Color` | `SKColor` | `Red` | Text color |
| `Background` | `IOverlayManagerBackground` | `null` | Optional background |
| `CustomWidth` | `int` | `0` | Custom bounding width (0 = auto) |
| `CustomHeight` | `int` | `0` | Custom bounding height (0 = auto) |

**Example:**

```csharp
var text = new OverlayManagerText("Hello World!", 100, 100);
text.Color = SKColors.White;
text.Font.Size = 48;
text.Font.Name = "Arial";
text.Shadow = new OverlayManagerShadowSettings(true, depth: 5, direction: 45);
overlayManager.Video_Overlay_Add(text);
```

### OverlayManagerImage

Displays static images with stretch modes.

```csharp
public class OverlayManagerImage : IOverlayManagerElement, IDisposable
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Display width (0 = original) |
| `Height` | `int` | - | Display height (0 = original) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `None` | Image scaling mode |

**Stretch Modes:**

- `None` - Original size
- `Stretch` - Fill target area (may distort)
- `Letterbox` - Fit within area (maintain aspect ratio)
- `CropToFill` - Fill area by cropping (maintain aspect ratio)

**Constructors:**

```csharp
// From file
new OverlayManagerImage(string filename, int x, int y, double alpha = 1.0)

// From SkiaSharp bitmap
new OverlayManagerImage(SKBitmap image, int x, int y, double alpha = 1.0)

// From System.Drawing.Bitmap (Windows only)
new OverlayManagerImage(System.Drawing.Bitmap image, int x, int y, double alpha = 1.0)
```

**Example:**

```csharp
var image = new OverlayManagerImage("logo.png", 10, 10);
image.StretchMode = OverlayManagerImageStretchMode.Letterbox;
image.Width = 200;
image.Height = 100;
overlayManager.Video_Overlay_Add(image);
```

### OverlayManagerGIF

Displays animated GIF images.

```csharp
public class OverlayManagerGIF : IOverlayManagerElement, IDisposable
```

| Property | Type | Description |
|----------|------|-------------|
| `Position` | `SKPoint` | Position of the GIF |
| `AnimationLength` | `TimeSpan` | Total animation duration |

**Example:**

```csharp
var gif = new OverlayManagerGIF("animation.gif", new SKPoint(150, 150));
overlayManager.Video_Overlay_Add(gif);
```

### OverlayManagerImageSequence

Displays a sequence of images, each shown for a specified duration, with support for looping, animation, fade effects, and all standard overlay properties.

```csharp
public class OverlayManagerImageSequence : IOverlayManagerElement, IDisposable
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | `0` | Display width (0 = original) |
| `Height` | `int` | `0` | Display height (0 = original) |
| `Loop` | `bool` | `true` | Restart sequence after last frame |
| `StretchMode` | `OverlayManagerImageStretchMode` | `None` | Image scaling mode |
| `AnimationLength` | `TimeSpan` | - | Total duration of all frames (read-only) |
| `FrameCount` | `int` | - | Number of loaded frames (read-only) |

**Animation Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `AnimationEnabled` | `bool` | `false` | Enable position/size animation |
| `TargetX` | `int` | `0` | Animation target X |
| `TargetY` | `int` | `0` | Animation target Y |
| `TargetWidth` | `int` | `0` | Animation target width (0 = keep current) |
| `TargetHeight` | `int` | `0` | Animation target height (0 = keep current) |
| `AnimationStartTime` | `TimeSpan` | `Zero` | Animation start time |
| `AnimationEndTime` | `TimeSpan` | `Zero` | Animation end time |
| `Easing` | `OverlayManagerPanEasing` | `Linear` | Position animation easing |

**Fade Properties:**

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FadeEnabled` | `bool` | `false` | Enable fade animation |
| `FadeType` | `OverlayManagerFadeType` | `FadeIn` | Fade direction |
| `FadeStartTime` | `TimeSpan` | `Zero` | Fade start time |
| `FadeEndTime` | `TimeSpan` | `Zero` | Fade end time |
| `FadeEasing` | `OverlayManagerPanEasing` | `Linear` | Fade easing function |

**Constructors:**

```csharp
// Basic: position only
new OverlayManagerImageSequence(IEnumerable<ImageSequenceItem> items, int x, int y)

// Full: position, size, and stretch mode
new OverlayManagerImageSequence(
    IEnumerable<ImageSequenceItem> items,
    int x, int y,
    int width, int height,
    OverlayManagerImageStretchMode stretchMode = None)
```

**Methods:**

```csharp
// Add a frame dynamically during playback
void AddFrame(string filename, TimeSpan duration)

// Start position/size animation
void StartAnimation(int targetX, int targetY, int targetWidth, int targetHeight,
    TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)

// Get interpolated position/size at the given time
(int X, int Y, int Width, int Height) GetCurrentRect(TimeSpan currentTime)

// Fade effects
void StartFadeIn(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
void StartFadeOut(TimeSpan startTime, TimeSpan duration, OverlayManagerPanEasing easing = Linear)
double GetCurrentOpacity(TimeSpan currentTime)
```

**Example - Basic Image Sequence:**

```csharp
// Define images with per-frame durations
var items = new List<ImageSequenceItem>
{
    new ImageSequenceItem("slide1.png", TimeSpan.FromSeconds(3)),
    new ImageSequenceItem("slide2.png", TimeSpan.FromSeconds(2)),
    new ImageSequenceItem("slide3.png", TimeSpan.FromSeconds(4))
};

// Create sequence at position (100, 100), scaled to 320x240
var sequence = new OverlayManagerImageSequence(items, 100, 100, 320, 240)
{
    Loop = true,
    Opacity = 0.9,
    ZIndex = 5,
    Name = "SlideShow"
};

overlayManager.Video_Overlay_Add(sequence);
```

**Example - With Animation and Fade:**

```csharp
// Animate the sequence from top-left to center over 3 seconds
sequence.StartAnimation(
    targetX: 400, targetY: 200,
    targetWidth: 640, targetHeight: 480,
    startTime: TimeSpan.FromSeconds(2),
    duration: TimeSpan.FromSeconds(3),
    easing: OverlayManagerPanEasing.EaseInOut);

// Fade in over the first 1.5 seconds
sequence.StartFadeIn(
    startTime: TimeSpan.Zero,
    duration: TimeSpan.FromSeconds(1.5),
    easing: OverlayManagerPanEasing.EaseOut);
```

**Example - Add Frames Dynamically:**

```csharp
// Add a new frame to a running sequence
sequence.AddFrame("slide4.png", TimeSpan.FromSeconds(2.5));
```

**Convenience Methods on OverlayManagerBlock:**

```csharp
// Add image sequence overlay
var element = overlayManager.Video_Overlay_AddImageSequence(
    items, x: 100, y: 100, width: 320, height: 240,
    loop: true, name: "SlideShow");

// Update position
overlayManager.Video_Overlay_UpdateImageSequencePosition(
    "SlideShow", x: 200, y: 150, width: 400, height: 300);

// Animate position/size
overlayManager.Video_Overlay_AnimateImageSequence(
    "SlideShow", targetX: 500, targetY: 300, targetWidth: 640, targetHeight: 480,
    startTime: currentPosition, duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut);

// Fade effects
overlayManager.Video_Overlay_ImageSequenceFadeIn(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
overlayManager.Video_Overlay_ImageSequenceFadeOut(
    "SlideShow", startTime: currentPosition, duration: TimeSpan.FromSeconds(1));
```

#### ImageSequenceItem

Represents a single image frame in an image sequence.

```csharp
public class ImageSequenceItem
```

| Property   | Type       | Description                     |
|------------|------------|---------------------------------|
| `Filename` | `string`   | Full path to the image file     |
| `Duration` | `TimeSpan` | How long this image is displayed |

```csharp
// Constructors
new ImageSequenceItem()
new ImageSequenceItem(string filename, TimeSpan duration)
```

### OverlayManagerDateTime

Displays current date/time with custom formatting.

```csharp
public class OverlayManagerDateTime : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "[DATETIME]" | Text template |
| `Format` | `string` | "MM/dd/yyyy HH:mm:ss" | DateTime format |
| `X` | `int` | `100` | X position |
| `Y` | `int` | `100` | Y position |
| `Font` | `FontSettings` | System default | Font configuration |
| `Color` | `SKColor` | `Red` | Text color |

**Example:**

```csharp
var dateTime = new OverlayManagerDateTime();
dateTime.Format = "yyyy-MM-dd HH:mm:ss";
dateTime.X = 10;
dateTime.Y = 30;
overlayManager.Video_Overlay_Add(dateTime);
```

### OverlayManagerScrollingText

Displays scrolling text that moves across the video in a specified direction.

```csharp
public class OverlayManagerScrollingText : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Text` | `string` | "Scrolling Text" | Text to display |
| `X` | `int` | `0` | X position of the scrolling area |
| `Y` | `int` | `100` | Y position of the scrolling area |
| `Width` | `int` | `0` | Width of scrolling area (0 = uses DefaultWidth) |
| `Height` | `int` | `0` | Height of scrolling area (0 = auto based on font) |
| `DefaultWidth` | `int` | `1920` | Default width when Width is 0 (set to video width) |
| `DefaultHeight` | `int` | `1080` | Default height when Height is 0 for vertical scroll |
| `Speed` | `int` | `5` | Scrolling speed in pixels per frame |
| `Direction` | `ScrollDirection` | `RightToLeft` | Scroll direction |
| `Font` | `FontSettings` | System default | Font configuration |
| `Color` | `SKColor` | `White` | Text color |
| `BackgroundTransparent` | `bool` | `true` | Whether background is transparent |
| `BackgroundColor` | `SKColor` | `Black` | Background color (when not transparent) |
| `Infinite` | `bool` | `true` | Loop scrolling infinitely |
| `TextRestarted` | `EventHandler` | `null` | Called when text loops back to start |

**ScrollDirection Enum:**

- `LeftToRight` - Text scrolls from left to right
- `RightToLeft` - Text scrolls from right to left
- `BottomToTop` - Text scrolls from bottom to top
- `TopToBottom` - Text scrolls from top to bottom

**Example:**

```csharp
// Create a news ticker style scrolling text
var scrollingText = new OverlayManagerScrollingText(
    "Breaking News: VisioForge Media Framework now supports scrolling text overlays!",
    x: 0,
    y: 50,
    speed: 3,
    direction: ScrollDirection.RightToLeft);

scrollingText.Font.Size = 24;
scrollingText.Color = SKColors.Yellow;
scrollingText.BackgroundTransparent = false;
scrollingText.BackgroundColor = SKColors.DarkBlue;

// Set the default width to match your video resolution
// This is used when Width is not explicitly set
scrollingText.DefaultWidth = 1920; // Full HD width

// Or set Width directly for a specific scrolling area width
// scrollingText.Width = 1920;

// Add event handler for when text loops
scrollingText.TextRestarted += (sender, e) => {
    Console.WriteLine("Scrolling text restarted");
};

overlayManager.Video_Overlay_Add(scrollingText);

// To reset scrolling position
scrollingText.Reset();

// To update after changing text or font
scrollingText.Text = "Updated news text...";
scrollingText.Update();
```

### Video Overlays

The Overlay Manager supports video overlays from multiple sources, including video files/URLs, Decklink capture cards, and NDI network sources. Video overlays play within the overlay composition with full playback control.

#### OverlayManagerVideo

Plays video files or stream URLs as overlays on the video composition. Each video overlay runs its own internal playback pipeline with optional audio output.

```csharp
public class OverlayManagerVideo : IOverlayManagerElement, IDisposable
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Source` | `string` | - | Video source file path or URL |
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Overlay width |
| `Height` | `int` | - | Overlay height |
| `Loop` | `bool` | `true` | Whether the video loops |
| `PlaybackRate` | `double` | `1.0` | Playback speed (1.0 = normal) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | How to fit video |
| `VideoView` | `IVideoView` | `null` | Optional external video preview window |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Renderer settings for VideoView |
| `AudioOutput` | `AudioOutputDeviceInfo` | `null` | Audio output device (null = discard audio) |
| `AudioOutput_Volume` | `double` | `1.0` | Audio volume (0.0-1.0+, above 1.0 amplifies) |
| `AudioOutput_Mute` | `bool` | `false` | Mute audio output |

**Methods:**

- `Initialize(bool autoStart)` - Initialize the video pipeline. If `autoStart` is true, begins playing immediately; if false, preloads to PAUSED state.
- `Play()` - Start or resume video playback
- `Pause()` - Pause video playback
- `Stop()` - Stop video playback
- `Seek(TimeSpan position)` - Seek to a specific position in the video
- `UpdateSource(string source)` - Change the video source dynamically
- `Dispose()` - Clean up resources

**Example - Video File Overlay:**

```csharp
// Create a video file overlay
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 100,
    y: 100,
    width: 640,
    height: 360)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Optionally enable audio output
var audioOutputs = await AudioRendererBlock.GetDevicesAsync(AudioOutputDeviceAPI.DirectSound);
videoOverlay.AudioOutput = audioOutputs[0];
videoOverlay.AudioOutput_Volume = 0.5;

// Initialize and add to overlay manager
if (videoOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(videoOverlay);
}

// Control playback at runtime
videoOverlay.Pause();
videoOverlay.Seek(TimeSpan.FromSeconds(10));
videoOverlay.Play();

// Change source dynamically
videoOverlay.UpdateSource("outro.mp4");

// Clean up when done
videoOverlay.Stop();
videoOverlay.Dispose();
```

**Example - Picture-in-Picture:**

```csharp
// Create a small PiP video in the corner
var pipVideo = new OverlayManagerVideo(
    source: "camera_feed.mp4",
    x: 20,
    y: 20,
    width: 240,
    height: 135)
{
    Loop = true,
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 100,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.DarkGray,
        Opacity = 0.7,
        BlurRadius = 8,
        Depth = 3,
        Direction = 45
    }
};

if (pipVideo.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(pipVideo);
}
```

**Example - Stream URL Overlay:**

```csharp
// Overlay a network stream
var streamOverlay = new OverlayManagerVideo(
    source: "rtsp://192.168.1.21:554/Streaming/Channels/101",
    x: 400,
    y: 50,
    width: 320,
    height: 240)
{
    Loop = false,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 10
};

if (streamOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(streamOverlay);
}
```

#### OverlayManagerDecklinkVideo

Captures and displays video from Blackmagic Decklink capture cards.

```csharp
public class OverlayManagerDecklinkVideo : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `DecklinkSettings` | `DecklinkVideoSourceSettings` | - | Decklink device configuration |
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Overlay width |
| `Height` | `int` | - | Overlay height |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | How to fit video |
| `VideoView` | `IVideoView` | `null` | Optional video preview |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Renderer settings |

**Example:**

```csharp
// Get Decklink devices
var devices = await DecklinkVideoSourceBlock.GetDevicesAsync();
var decklinkSettings = new DecklinkVideoSourceSettings(devices[0]);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

// Create Decklink overlay
var decklinkOverlay = new OverlayManagerDecklinkVideo(
    decklinkSettings,
    x: 10,
    y: 10,
    width: 640,
    height: 360);

// Initialize and add to overlay manager
if (decklinkOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(decklinkOverlay);
}

// Clean up when done
decklinkOverlay.Stop();
decklinkOverlay.Dispose();
```

#### OverlayManagerNDIVideo

Captures and displays video from NDI (Network Device Interface) sources.

```csharp
public class OverlayManagerNDIVideo : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `NDISettings` | `NDISourceSettings` | - | NDI source configuration |
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Overlay width |
| `Height` | `int` | - | Overlay height |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | How to fit video |
| `VideoView` | `IVideoView` | `null` | Optional video preview |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Renderer settings |

**Example:**

```csharp
// Discover NDI sources on the network
var ndiSources = await DeviceEnumerator.Shared.NDISourcesAsync();
var ndiSettings = await NDISourceSettings.CreateAsync(
    null,
    ndiSources[0].Name,
    ndiSources[0].URL);

// Create NDI overlay
var ndiOverlay = new OverlayManagerNDIVideo(
    ndiSettings,
    x: 10,
    y: 10,
    width: 640,
    height: 360);

// Initialize and add to overlay manager
if (ndiOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(ndiOverlay);
}

// Clean up when done
ndiOverlay.Stop();
ndiOverlay.Dispose();
```

**Common Methods for Video Overlays:**

- `Initialize(bool autoStart)` - Initialize the video pipeline
- `Play()` - Start or resume video playback/capture
- `Pause()` - Pause video playback/capture
- `Stop()` - Stop video playback/capture
- `Dispose()` - Clean up resources

**Additional Methods (OverlayManagerVideo only):**

- `Seek(TimeSpan position)` - Seek to a specific position
- `UpdateSource(string source)` - Change the video source dynamically

### OverlayManagerWPFControl (Windows Only)

Renders a WPF `FrameworkElement` as a video overlay. This enables using any WPF visual tree — including controls with Storyboard animations, data binding, and complex layouts — as an overlay. The element is periodically snapshot at a configurable refresh rate.

> **Note**: This overlay type is only available on Windows (`NET_WINDOWS` build target).

```csharp
public class OverlayManagerWPFControl : IOverlayManagerElement, IDisposable
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ElementFactory` | `Func<FrameworkElement>` | - | Factory that creates the WPF element on the internal STA thread |
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Overlay width (must be > 0) |
| `Height` | `int` | - | Overlay height (must be > 0) |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Stretch` | How to fit the rendered control |
| `RefreshRate` | `int` | `15` | Snapshots per second (1-60) |
| `Dpi` | `double` | `96` | DPI for rendering |

**Methods:**

- `Initialize()` - Initialize the WPF control overlay and start periodic rendering
- `InvokeOnUIThread(Action action)` - Execute an action on the WPF STA thread for safe runtime updates
- `InvokeOnUIThread<T>(Func<T> func)` - Execute a function on the WPF STA thread and return the result
- `Dispose()` - Clean up resources

**Convenience Method on OverlayManagerBlock:**

```csharp
public OverlayManagerWPFControl Video_Overlay_AddWPFControl(
    Func<FrameworkElement> elementFactory,
    int x, int y, int width, int height,
    int refreshRate = 15,
    string name = null)
```

Creates, initializes, and adds a WPF control overlay in one call. Returns the overlay instance, or `null` if initialization failed.

**Example - Using Convenience Method:**

```csharp
// Add a color-cycling text overlay using the convenience method
var wpfOverlay = overlayManager.Video_Overlay_AddWPFControl(
    elementFactory: () =>
    {
        var border = new Border
        {
            Width = 350,
            Height = 60,
            Background = new SolidColorBrush(Color.FromArgb(160, 0, 0, 0)),
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15, 5, 15, 5)
        };

        var text = new TextBlock
        {
            Text = "VisioForge Media Framework",
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center
        };

        var brush = new SolidColorBrush(Colors.Red);
        text.Foreground = brush;
        border.Child = text;

        // Animate text color
        var colorAnim = new ColorAnimationUsingKeyFrames
        {
            Duration = TimeSpan.FromSeconds(5),
            RepeatBehavior = RepeatBehavior.Forever
        };
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(0.0)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Gold, KeyTime.FromPercent(0.25)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Cyan, KeyTime.FromPercent(0.5)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Magenta, KeyTime.FromPercent(0.75)));
        colorAnim.KeyFrames.Add(new LinearColorKeyFrame(Colors.Red, KeyTime.FromPercent(1.0)));
        brush.BeginAnimation(SolidColorBrush.ColorProperty, colorAnim);

        return border;
    },
    x: 50, y: 300, width: 350, height: 60,
    refreshRate: 30, name: "ColorText");

if (wpfOverlay == null)
{
    // Initialization failed
}
```

**Example - Manual Creation with Animated Clock:**

```csharp
// Create a WPF analog clock overlay manually
var clockOverlay = new OverlayManagerWPFControl(
    elementFactory: () =>
    {
        var canvas = new Canvas { Width = 200, Height = 200 };

        // Clock face
        var face = new Ellipse
        {
            Width = 180, Height = 180,
            Stroke = Brushes.White, StrokeThickness = 3
        };
        Canvas.SetLeft(face, 10);
        Canvas.SetTop(face, 10);
        canvas.Children.Add(face);

        // Second hand with rotation animation
        var secondHand = new Line
        {
            X1 = 100, Y1 = 100, X2 = 100, Y2 = 25,
            Stroke = Brushes.Red, StrokeThickness = 1
        };
        var secondRotate = new RotateTransform(DateTime.Now.Second * 6, 100, 100);
        secondHand.RenderTransform = secondRotate;
        canvas.Children.Add(secondHand);

        var anim = new DoubleAnimation
        {
            From = DateTime.Now.Second * 6,
            To = DateTime.Now.Second * 6 + 360,
            Duration = TimeSpan.FromSeconds(60),
            RepeatBehavior = RepeatBehavior.Forever
        };
        secondRotate.BeginAnimation(RotateTransform.AngleProperty, anim);

        return canvas;
    },
    x: 20, y: 20, width: 200, height: 200, refreshRate: 30);

if (clockOverlay.Initialize())
{
    overlayManager.Video_Overlay_Add(clockOverlay);
}

// Update WPF control at runtime (thread-safe)
clockOverlay.InvokeOnUIThread(() =>
{
    // Safe to modify WPF elements here
});

// Clean up
clockOverlay.Dispose();
```

### OverlayManagerWebView2Video (Windows Only)

Renders live web content (HTML, CSS, JavaScript) as a video overlay using Microsoft WebView2. This enables displaying web pages, dynamic HTML dashboards, animated web content, news tickers, or any browser-rendered content as an overlay on your video. The web page is rendered offscreen and captured as video frames at the browser's native refresh rate.

> **Note**: This overlay type is only available on Windows (`NET_WINDOWS` build target). Requires the Microsoft WebView2 Runtime and the GStreamer WebView2 plugin (`webview2src`).

```csharp
public class OverlayManagerWebView2Video : IOverlayManagerElement, IDisposable
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Location` | `string` | `"about:blank"` | URL to display in the overlay |
| `JavaScript` | `string` | `null` | JavaScript code to execute after each navigation completes |
| `Adapter` | `int` | `-1` | DXGI adapter index for GPU selection (-1 = any available device) |
| `UserDataFolder` | `string` | `null` | Absolute path to WebView2 user data folder for cache and profile data |
| `X` | `int` | - | X position |
| `Y` | `int` | - | Y position |
| `Width` | `int` | - | Overlay width |
| `Height` | `int` | - | Overlay height |
| `StretchMode` | `OverlayManagerImageStretchMode` | `Letterbox` | How to fit the rendered content |
| `VideoView` | `IVideoView` | `null` | Optional external video preview window |
| `VideoRendererSettings` | `VideoRendererSettingsX` | `null` | Renderer settings for VideoView |

**Methods:**

- `Initialize(bool autoStart = true)` - Initialize the WebView2 rendering pipeline. If `autoStart` is true, begins rendering immediately; if false, preloads to PAUSED state. Returns `true` if successful.
- `Play()` - Start or resume web page rendering
- `Pause()` - Pause web page rendering
- `Stop()` - Stop web page rendering
- `UpdateLocation(string location)` - Change the displayed URL dynamically
- `Dispose()` - Clean up resources

**Example - Basic Web Page Overlay:**

```csharp
// Display a web page as a video overlay
var webOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/dashboard",
    x: 50,
    y: 50,
    width: 640,
    height: 480)
{
    Opacity = 0.9,
    StretchMode = OverlayManagerImageStretchMode.Letterbox,
    ZIndex = 5
};

// Initialize and add to overlay manager
if (webOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(webOverlay);
}
else
{
    webOverlay.Dispose();
}
```

**Example - Web Overlay with JavaScript Injection:**

```csharp
// Overlay a web page and inject JavaScript to customize it
var tickerOverlay = new OverlayManagerWebView2Video(
    location: "https://example.com/ticker",
    x: 0,
    y: 680,
    width: 1920,
    height: 40)
{
    // JavaScript runs after each navigation completes
    JavaScript = "document.body.style.background = 'transparent';",
    Opacity = 0.8,
    ZIndex = 10,
    Shadow = new OverlayManagerShadowSettings
    {
        Enabled = true,
        Color = SKColors.Black,
        Opacity = 0.5,
        BlurRadius = 5,
        Depth = 5,
        Direction = 45
    }
};

if (tickerOverlay.Initialize(autoStart: true))
{
    overlayManager.Video_Overlay_Add(tickerOverlay);
}
```

**Example - Dynamic URL Update at Runtime:**

```csharp
// Change the displayed page at runtime
webOverlay.UpdateLocation("https://example.com/new-page");

// Control rendering
webOverlay.Pause();
webOverlay.Play();

// Clean up when done
webOverlay.Stop();
webOverlay.Dispose();
```

### Shape Overlays

#### OverlayManagerLine

```csharp
public class OverlayManagerLine : IOverlayManagerElement
```

| Property | Type | Description |
|----------|------|-------------|
| `Start` | `SKPoint` | Line start point |
| `End` | `SKPoint` | Line end point |
| `Color` | `SKColor` | Line color |

#### OverlayManagerRectangle

```csharp
public class OverlayManagerRectangle : IOverlayManagerElement
```

| Property | Type | Description |
|----------|------|-------------|
| `Rectangle` | `SKRect` | Rectangle bounds |
| `Color` | `SKColor` | Fill/stroke color |
| `Fill` | `bool` | Fill or stroke only |

#### OverlayManagerCircle

```csharp
public class OverlayManagerCircle : IOverlayManagerElement
```

| Property | Type | Description |
|----------|------|-------------|
| `Center` | `SKPoint` | Circle center |
| `Radius` | `double` | Circle radius |
| `Color` | `SKColor` | Fill/stroke color |
| `Fill` | `bool` | Fill or stroke only |

#### OverlayManagerTriangle

```csharp
public class OverlayManagerTriangle : IOverlayManagerElement
```

| Property | Type | Description |
|----------|------|-------------|
| `Point1` | `SKPoint` | First vertex |
| `Point2` | `SKPoint` | Second vertex |
| `Point3` | `SKPoint` | Third vertex |
| `Color` | `SKColor` | Fill/stroke color |
| `Fill` | `bool` | Fill or stroke only |

#### OverlayManagerStar

```csharp
public class OverlayManagerStar : IOverlayManagerElement
```

| Property | Type | Description |
|----------|------|-------------|
| `Center` | `SKPoint` | Star center |
| `OuterRadius` | `double` | Outer points radius |
| `InnerRadius` | `double` | Inner points radius |
| `StrokeColor` | `SKColor` | Stroke color |
| `FillColor` | `SKColor` | Fill color |

### OverlayManagerSVG

Displays SVG vector graphics.

```csharp
public class OverlayManagerSVG : IOverlayManagerElement, IDisposable
```

| Property | Type | Description |
|----------|------|-------------|
| `X` | `int` | X position |
| `Y` | `int` | Y position |
| `Width` | `int` | Display width |
| `Height` | `int` | Display height |

### OverlayManagerCallback

Custom drawing using Cairo graphics.

```csharp
public class OverlayManagerCallback : IOverlayManagerElement
```

**Event:**

```csharp
public event EventHandler<OverlayManagerCallbackEventArgs> OnDraw;
```

**Example:**

```csharp
var callback = new OverlayManagerCallback();
callback.OnDraw += (sender, e) => {
    var ctx = e.Context;
    ctx.SetSourceRGB(1, 0, 0);
    ctx.Arc(200, 200, 50, 0, 2 * Math.PI);
    ctx.Fill();
};
overlayManager.Video_Overlay_Add(callback);
```

### OverlayManagerGroup

Groups multiple overlays for synchronized lifecycle management. This is especially useful when you need to preload multiple video overlays and start them at exactly the same time.

```csharp
public class OverlayManagerGroup : IOverlayManagerElement, IDisposable
```

| Property   | Type                            | Default    | Description                         |
|------------|----------------------------------|------------|-------------------------------------|
| `Overlays` | `List<IOverlayManagerElement>`   | Empty list | Overlays in this group (read-only)  |

> **Note**: The standard `IOverlayManagerElement` properties (`Opacity`, `Rotation`, `ZIndex`, `Shadow`) are present on the group but not applied at the group level — individual overlay properties within the group are used for rendering.

**Methods:**

- `Add(IOverlayManagerElement overlay)` - Add an overlay to the group. Throws `InvalidOperationException` if already initialized.
- `Remove(IOverlayManagerElement overlay)` - Remove an overlay from the group. Throws `InvalidOperationException` if already initialized.
- `Initialize()` - Preload all `OverlayManagerVideo` overlays in the group to PAUSED state. Returns `true` if all succeeded. Other overlay types (Decklink, NDI) must be initialized manually.
- `Play()` - Start all `OverlayManagerVideo` overlays synchronously. Must call `Initialize()` first. Other video overlay types must call `Play()` individually.
- `Pause()` - Pause all `OverlayManagerVideo` overlays in the group.
- `Stop()` - Stop all `OverlayManagerVideo` overlays in the group.
- `GetRenderableOverlays()` - Returns all enabled overlays in the group (used internally).
- `Dispose()` - Stop and dispose all overlays in the group.

> **Important**: You cannot add or remove overlays after `Initialize()` has been called.

**Why add non-video overlays to a group?** While `Initialize()`, `Play()`, `Pause()`, and `Stop()` only control `OverlayManagerVideo` instances, adding other overlay types (Decklink, NDI, text, images) to a group provides organizational grouping for rendering and centralized disposal — `Dispose()` cleans up **all** overlays in the group regardless of type.

**Example - Synchronized Video + Decklink Group:**

```csharp
// Create a group for overlays that must start simultaneously
var group = new OverlayManagerGroup("SyncGroup");

// Add a video file overlay
var videoOverlay = new OverlayManagerVideo(
    source: "intro.mp4",
    x: 10, y: 10, width: 640, height: 360)
{
    Loop = true,
    ZIndex = 10
};
group.Add(videoOverlay);

// Add a Decklink capture overlay
var decklinkSettings = new DecklinkVideoSourceSettings(deviceNumber);
decklinkSettings.Mode = DecklinkMode.HD1080p2997;

var decklinkOverlay = new OverlayManagerDecklinkVideo(
    settings: decklinkSettings,
    x: 660, y: 10, width: 640, height: 360)
{
    ZIndex = 11
};

// Initialize Decklink manually (group handles OverlayManagerVideo only)
decklinkOverlay.Initialize(autoStart: false);
group.Add(decklinkOverlay);

// You can also mix in non-video overlays
var label = new OverlayManagerText("Camera 1", 10, 380);
label.Font.Size = 14;
label.Color = SKColors.White;
group.Add(label);

// Add group to overlay manager
overlayManager.Video_Overlay_Add(group);

// Start all OverlayManagerVideo overlays in the group
group.Play();
// Decklink must be started separately (group.Play() only handles OverlayManagerVideo)
decklinkOverlay.Play();

// Later, pause/stop all at once
group.Pause();
group.Stop();

// Clean up
group.Dispose();
```

## Video Transformation Effects

The OverlayManagerBlock supports advanced video transformation effects that modify the entire video frame, not just overlays on top.

### OverlayManagerZoom

Applies a zoom effect to the video frame, scaling from a center point.

```csharp
public class OverlayManagerZoom : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ZoomFactor` | `double` | `1.0` | Zoom level (1.0 = no zoom, 2.0 = 2x zoom) |
| `CenterX` | `double` | `0.5` | Horizontal center (0.0-1.0, relative to frame) |
| `CenterY` | `double` | `0.5` | Vertical center (0.0-1.0, relative to frame) |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Quality/speed tradeoff |

**Example:**

```csharp
// Create a 1.5x zoom centered on the frame
var zoom = new OverlayManagerZoom
{
    ZoomFactor = 1.5,
    CenterX = 0.5,
    CenterY = 0.5,
    Name = "VideoZoom"
};
zoom.ZIndex = -1000; // Process before other overlays
overlayManager.Video_Overlay_Add(zoom);

// Animate zoom over time
zoom.ZoomFactor = 2.0; // Update dynamically
```

### OverlayManagerPan

Applies a pan (translate) effect to the video frame.

```csharp
public class OverlayManagerPan : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `OffsetX` | `double` | `0.0` | Horizontal offset in pixels |
| `OffsetY` | `double` | `0.0` | Vertical offset in pixels |
| `InterpolationMode` | `OverlayManagerInterpolationMode` | `Bilinear` | Quality/speed tradeoff |

**Example:**

```csharp
// Pan video 100 pixels right and 50 pixels down
var pan = new OverlayManagerPan
{
    OffsetX = 100,
    OffsetY = 50,
    Name = "VideoPan"
};
pan.ZIndex = -1000; // Process before other overlays
overlayManager.Video_Overlay_Add(pan);
```

### OverlayManagerFade

Applies a fade effect to the entire video frame.

```csharp
public class OverlayManagerFade : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `FadeMode` | `OverlayManagerFadeMode` | `None` | Fade type (None, FadeIn, FadeOut) |
| `StartTime` | `TimeSpan` | `Zero` | When fade effect starts |
| `Duration` | `TimeSpan` | `1 second` | How long the fade takes |
| `MinOpacity` | `double` | `0.0` | Minimum opacity (fully faded) |
| `MaxOpacity` | `double` | `1.0` | Maximum opacity (fully visible) |

**Example:**

```csharp
// Fade in video over 2 seconds starting at playback position
var fade = new OverlayManagerFade
{
    FadeMode = OverlayManagerFadeMode.FadeIn,
    StartTime = TimeSpan.Zero,
    Duration = TimeSpan.FromSeconds(2),
    Name = "VideoFade"
};
overlayManager.Video_Overlay_Add(fade);
```

### OverlayManagerSqueezeback

Creates a broadcast-style "squeezeback" effect where the video is scaled to a custom rectangle and an overlay image (typically PNG with alpha transparency) is drawn on top. This is commonly used for lower-thirds, frames, and broadcast graphics.

```csharp
public class OverlayManagerSqueezeback : IOverlayManagerElement
```

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackgroundImageFilename` | `string` | - | Overlay image path (PNG with alpha recommended) |
| `VideoRect` | `Rect` | - | Where video is scaled and positioned |
| `BackgroundRect` | `Rect` | `null` | Overlay image position (null = full frame) |
| `VideoOnTop` | `bool` | `false` | Layer order (false = video below, image on top) |
| `VideoOpacity` | `double` | `1.0` | Video layer opacity |
| `BackgroundOpacity` | `double` | `1.0` | Overlay image opacity |

**Animation Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `VideoAnimationEnabled` | `bool` | Enable video position animation |
| `VideoAnimationStartRect` | `Rect` | Animation start position |
| `VideoAnimationTargetRect` | `Rect` | Animation end position |
| `VideoAnimationStartTimeMs` | `double` | Animation start time (ms) |
| `VideoAnimationDurationMs` | `double` | Animation duration (ms) |
| `VideoAnimationEasing` | `OverlayManagerPanEasing` | Easing function |

**Fade Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `VideoFadeMode` | `OverlayManagerFadeMode` | Video fade type |
| `VideoFadeStartTimeMs` | `double` | Video fade start time |
| `VideoFadeDurationMs` | `double` | Video fade duration |
| `BackgroundFadeMode` | `OverlayManagerFadeMode` | Background fade type |
| `BackgroundFadeStartTimeMs` | `double` | Background fade start time |
| `BackgroundFadeDurationMs` | `double` | Background fade duration |

**Example - Basic Squeezeback:**

```csharp
// Create squeezeback with video in lower-right corner
var squeezeback = new OverlayManagerSqueezeback
{
    BackgroundImageFilename = "frame.png", // PNG with transparent center
    VideoRect = new Rect(960, 540, 1920, 1080), // Lower-right quadrant
    Name = "Squeezeback"
};
squeezeback.ZIndex = -2000; // Process first
overlayManager.Video_Overlay_Add(squeezeback);
```

**Example - Animated Squeezeback:**

```csharp
// Get current playback position
var position = await pipeline.Position_GetAsync();

// Animate video from full-screen to corner over 2 seconds
var squeezeback = overlayManager.Video_Overlay_GetByName("Squeezeback") as OverlayManagerSqueezeback;
squeezeback.AnimateVideo(
    startRect: new Rect(0, 0, 1920, 1080),      // Full screen
    targetRect: new Rect(1280, 720, 1920, 1080), // Lower-right corner
    startTime: position,
    duration: TimeSpan.FromSeconds(2),
    easing: OverlayManagerPanEasing.EaseInOut
);
```

**Example - Fade Effects:**

```csharp
var position = await pipeline.Position_GetAsync();

// Fade out video over 1.5 seconds
squeezeback.StartVideoFadeOut(position, TimeSpan.FromSeconds(1.5));

// Later, fade it back in
squeezeback.StartVideoFadeIn(position, TimeSpan.FromSeconds(1.5));

// Fade background image independently
squeezeback.StartBackgroundFadeOut(position, TimeSpan.FromSeconds(1));
```

**Convenience Methods on OverlayManagerBlock:**

```csharp
// Add squeezeback
var element = overlayManager.Video_Overlay_AddSqueezeback(
    backgroundImageFilename: "frame.png",
    videoRect: new Rect(960, 540, 1920, 1080),
    backgroundRect: null,  // Full frame
    name: "Squeezeback"
);

// Update video position
overlayManager.Video_Overlay_Squeezeback_UpdateVideoPosition("Squeezeback", newRect);

// Animate video
overlayManager.Video_Overlay_Squeezeback_AnimateVideo(
    "Squeezeback", startRect, targetRect, startTime, duration, easing);

// Fade controls
overlayManager.Video_Overlay_Squeezeback_VideoFadeIn("Squeezeback", startTime, duration);
overlayManager.Video_Overlay_Squeezeback_VideoFadeOut("Squeezeback", startTime, duration);

// Layer ordering
overlayManager.Video_Overlay_Squeezeback_SetVideoOnTop("Squeezeback");
overlayManager.Video_Overlay_Squeezeback_SetBackgroundOnTop("Squeezeback");
```

### Easing Functions

Animation easing options available via `OverlayManagerPanEasing`:

| Value | Description |
|-------|-------------|
| `Linear` | Constant speed |
| `EaseIn` | Slow start, fast end |
| `EaseOut` | Fast start, slow end |
| `EaseInOut` | Slow start and end |
| `EaseInCubic` | Cubic slow start |
| `EaseOutCubic` | Cubic slow end |
| `EaseInOutCubic` | Cubic slow start and end |

### Interpolation Modes

Quality/performance tradeoff for scaling via `OverlayManagerInterpolationMode`:

| Value | Description |
|-------|-------------|
| `Nearest` | Fastest, pixelated edges |
| `Bilinear` | Good quality/speed balance |
| `Gaussian` | Higher quality, slower |

## Shadow Settings

Configure drop shadows for overlay elements:

```csharp
public class OverlayManagerShadowSettings
```

| Property | Type | Range | Default | Description |
|----------|------|-------|---------|-------------|
| `Enabled` | `bool` | - | `false` | Enable shadows |
| `Depth` | `double` | 0-30 | `5.0` | Shadow offset distance |
| `Direction` | `double` | 0-360° | `45.0` | Shadow direction |
| `Opacity` | `double` | 0-1 | `0.5` | Shadow transparency |
| `BlurRadius` | `double` | 0-10 | `2.0` | Shadow blur amount |
| `Color` | `SKColor` | - | `Black` | Shadow color |

**Direction Reference:**

- 0° = Right
- 90° = Down
- 180° = Left
- 270° = Up

## Text Backgrounds

Text overlays can have various background shapes:

### OverlayManagerBackgroundRectangle

```csharp
var text = new OverlayManagerText("Info", 100, 100);
text.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.Black.WithAlpha(128),
    Fill = true,
    Margin = new Thickness(5, 3, 5, 3)
};
```

### OverlayManagerBackgroundSquare

Similar to rectangle but maintains square aspect ratio.

### OverlayManagerBackgroundImage

Uses an image as text background with stretch modes.

### OverlayManagerBackgroundTriangle/Star

Custom shaped backgrounds for text.

## Font Settings

Configure text appearance:

```csharp
public class FontSettings
```

| Property | Type | Description |
|----------|------|-------------|
| `Name` | `string` | Font family name |
| `Size` | `int` | Font size in points |
| `Style` | `FontStyle` | Normal, Italic, Oblique |
| `Weight` | `FontWeight` | Normal, Bold, Light, etc. |

## Complete Example

```csharp
// Create pipeline and blocks
var pipeline = new MediaBlocksPipeline();
var fileSource = new UniversalSourceBlock(await UniversalSourceSettings.CreateAsync(videoUri));
var overlayManager = new OverlayManagerBlock();
var videoRenderer = new VideoRendererBlock(pipeline, videoView);

// Connect pipeline
pipeline.Connect(fileSource.VideoOutput, overlayManager.Input);
pipeline.Connect(overlayManager.Output, videoRenderer.Input);

// Add logo watermark
var logo = new OverlayManagerImage("logo.png", 10, 10);
logo.Opacity = 0.5;
logo.ZIndex = 10; // On top
overlayManager.Video_Overlay_Add(logo);

// Add timestamp
var timestamp = new OverlayManagerDateTime();
timestamp.X = 10;
timestamp.Y = pipeline.Height - 30;
timestamp.Font.Size = 16;
timestamp.Color = SKColors.White;
timestamp.Shadow = new OverlayManagerShadowSettings(true);
overlayManager.Video_Overlay_Add(timestamp);

// Add animated title (appears after 5 seconds)
var title = new OverlayManagerText("Welcome!", 100, 100);
title.Font.Size = 72;
title.Color = SKColors.Yellow;
title.StartTime = TimeSpan.FromSeconds(5);
title.EndTime = TimeSpan.FromSeconds(10);
title.Rotation = -10; // Slight tilt
title.Background = new OverlayManagerBackgroundRectangle {
    Color = SKColors.DarkBlue,
    Fill = true
};
overlayManager.Video_Overlay_Add(title);

// Start playback
await pipeline.StartAsync();

// Update overlays dynamically
title.Text = "Updated Text!";
overlayManager.Video_Overlay_Update(title);
```

## Sample Application

For a complete working example demonstrating all overlay types, refer to:

- [Overlay Manager Demo](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/WPF/CSharp/Overlay%20Manager%20Demo)

## Performance Considerations

1. **Z-Index Ordering**: Elements are sorted by Z-index before rendering. Use appropriate values to minimize sorting overhead.

2. **Image Formats**: Use RGBA8888 format images when possible to avoid color conversion.

3. **Shadow Effects**: Shadows with blur are computationally expensive. Use sparingly for real-time applications.

4. **Updates**: Use `Video_Overlay_Update()` for existing elements rather than remove/add operations.

5. **Resource Management**: Dispose image, GIF, and image sequence overlays when no longer needed to free memory.

6. **Video Overlays**: Each `OverlayManagerVideo` overlay runs its own internal GStreamer pipeline. Limit the number of simultaneous video overlays to avoid excessive CPU and memory usage.

7. **WPF Control Overlays**: Higher `RefreshRate` values increase CPU usage. Use the minimum refresh rate needed for smooth visual updates — 15 fps is sufficient for most static or slowly-changing content.

8. **WebView2 Overlays**: Each `OverlayManagerWebView2Video` overlay runs its own internal rendering pipeline with an offscreen browser. Limit the number of simultaneous WebView2 overlays to avoid excessive CPU, GPU, and memory usage.

9. **Overlay Groups**: Use `OverlayManagerGroup` to preload video overlays. This avoids staggered start times when multiple video overlays need to begin simultaneously.

## Platform Notes

- **Windows**: Supports System.Drawing.Bitmap in addition to SkiaSharp
- **Windows (WPF)**: Supports `OverlayManagerWPFControl` for rendering WPF visual elements as overlays. Requires `NET_WINDOWS` build target.
- **Windows (WebView2)**: Supports `OverlayManagerWebView2Video` for rendering live web content (HTML/CSS/JS) as overlays. Requires Microsoft WebView2 Runtime and the GStreamer WebView2 plugin (`webview2src`).
- **iOS**: Font defaults to "System-ui"
- **Android**: Font defaults to "System-ui"
- **Linux/macOS**: Enumerates available fonts at runtime

## Thread Safety

The overlay manager uses internal locking for thread-safe operations. You can safely add, remove, or update overlays from any thread.

## Troubleshooting

1. **Overlay not visible**: Check `Enabled` property, `StartTime`/`EndTime`, and `ZIndex` ordering.

2. **Text appears blurry**: Ensure font size is appropriate for video resolution.

3. **Memory usage**: Dispose unused image/GIF/image sequence overlays and use appropriate image sizes.

4. **Video overlay shows no frames**: Ensure `Initialize()` returns `true` before adding to the overlay manager. Check that the source file path is valid and accessible, and that GStreamer has the required codecs.

5. **WPF overlay not updating**: Verify `RefreshRate` is appropriate for your content. Use `InvokeOnUIThread()` for all WPF element modifications to avoid cross-thread exceptions.

6. **WebView2 overlay not rendering**: Ensure the Microsoft WebView2 Runtime is installed on the target machine. Check that `Initialize()` returns `true` before adding to the overlay manager. The GStreamer WebView2 plugin (`webview2src`) must be available.

7. **Group overlays not starting together**: Ensure all overlays are added to the group before calling `Initialize()`. Overlays cannot be added after initialization.

## Frequently Asked Questions

### How do I overlay a video file on top of another video in C#?

Use `OverlayManagerVideo` to play a video file or stream URL as an overlay. Create an instance with the source path, position, and dimensions, then call `Initialize()` and add it to the `OverlayManagerBlock`. You get full playback control with `Play()`, `Pause()`, `Stop()`, and `Seek()` methods, plus optional audio output. See the [OverlayManagerVideo](#overlaymanagervideo) section for examples.

### Can I use WPF controls as live video overlays?

Yes. `OverlayManagerWPFControl` renders any WPF `FrameworkElement` as a video overlay, including controls with Storyboard animations, data binding, and complex visual trees. The element is periodically snapshot at a configurable refresh rate (1-60 fps). This is Windows-only and requires the `NET_WINDOWS` build target. Use the convenience method `Video_Overlay_AddWPFControl()` for the simplest setup. See the [OverlayManagerWPFControl](#overlaymanagerwpfcontrol-windows-only) section.

### How do I synchronize multiple video overlays to start at the same time?

Use `OverlayManagerGroup` to group overlays that need coordinated lifecycle. Add all overlays to the group before calling `Initialize()`, which preloads video overlays to PAUSED state. Then call `Play()` to start them all simultaneously. This is especially useful for multi-camera compositions. See the [OverlayManagerGroup](#overlaymanagergroup) section.

### Can I play audio from a video file overlay?

Yes. Set the `AudioOutput` property on `OverlayManagerVideo` to an audio output device before calling `Initialize()`. Control volume with `AudioOutput_Volume` (0.0-1.0+) and mute with `AudioOutput_Mute`. If `AudioOutput` is null (the default), audio from the video file is discarded.

### What overlay types does OverlayManagerBlock support?

The OverlayManagerBlock supports: text (`OverlayManagerText`), date/time (`OverlayManagerDateTime`), scrolling text (`OverlayManagerScrollingText`), images (`OverlayManagerImage`), animated GIFs (`OverlayManagerGIF`), image sequences (`OverlayManagerImageSequence`), SVG graphics (`OverlayManagerSVG`), shapes (rectangle, circle, triangle, star, line), video files/URLs (`OverlayManagerVideo`), Decklink capture cards (`OverlayManagerDecklinkVideo`), NDI network sources (`OverlayManagerNDIVideo`), WebView2 web content (`OverlayManagerWebView2Video`, Windows only), WPF controls (`OverlayManagerWPFControl`, Windows only), overlay groups (`OverlayManagerGroup`), custom Cairo drawing (`OverlayManagerCallback`), and video transformation effects (zoom, pan, fade, squeezeback).

### Can I render live web content as a video overlay?

Yes. `OverlayManagerWebView2Video` renders any web page (HTML, CSS, JavaScript) as a video overlay using Microsoft WebView2. You can display dashboards, animated web content, tickers, or any browser-rendered content. It supports JavaScript injection after navigation for customizing the displayed page. This is Windows-only and requires the Microsoft WebView2 Runtime. See the [OverlayManagerWebView2Video](#overlaymanagerwebview2video-windows-only) section.
