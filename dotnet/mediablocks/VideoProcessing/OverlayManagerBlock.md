---
title: OverlayManagerBlock usage guide
description: Use OverlayManagerBlock to add overlays to video content with features to manage layers, shadows, rotation, and opacity in real-time.
---

# Overlay Manager Block usage guide

## Overview

The `OverlayManagerBlock` is a powerful MediaBlocks component that provides dynamic multi-layer video overlay composition and management. It allows you to add various overlay elements (images, text, shapes, animations) on top of video content with real-time updates, layer management, and advanced features like shadows, rotation, and opacity control.

## Key Features

- **Multiple Overlay Types**: Text, scrolling text, images, GIFs, SVG, shapes (rectangles, circles, triangles, stars, lines), live video (NDI, Decklink)
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

### Live Video Overlays

The Overlay Manager supports live video sources as overlays, allowing you to composite real-time video from Decklink capture cards or NDI network sources.

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

**Common Methods for Live Video Overlays:**

- `Initialize(bool autoStart)` - Initialize the video capture pipeline
- `Play()` - Start or resume video capture
- `Pause()` - Pause video capture
- `Stop()` - Stop video capture
- `Dispose()` - Clean up resources

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

## Performance Considerations

1. **Z-Index Ordering**: Elements are sorted by Z-index before rendering. Use appropriate values to minimize sorting overhead.

2. **Image Formats**: Use RGBA8888 format images when possible to avoid color conversion.

3. **Shadow Effects**: Shadows with blur are computationally expensive. Use sparingly for real-time applications.

4. **Updates**: Use `Video_Overlay_Update()` for existing elements rather than remove/add operations.

5. **Resource Management**: Dispose image and GIF overlays when no longer needed to free memory.

## Platform Notes

- **Windows**: Supports System.Drawing.Bitmap in addition to SkiaSharp
- **iOS**: Font defaults to "System-ui"
- **Android**: Font defaults to "System-ui"
- **Linux/macOS**: Enumerates available fonts at runtime

## Thread Safety

The overlay manager uses internal locking for thread-safe operations. You can safely add, remove, or update overlays from any thread.

## Troubleshooting

1. **Overlay not visible**: Check `Enabled` property, `StartTime`/`EndTime`, and `ZIndex` ordering.

2. **Text appears blurry**: Ensure font size is appropriate for video resolution.

3. **Memory usage**: Dispose unused image/GIF overlays and use appropriate image sizes.
