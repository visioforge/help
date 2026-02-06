---
title: Adding Professional Text Overlays to Videos in .NET
description: Implement dynamic text overlays with complete control over font, color, position, timing, and animations with step-by-step code examples.
---

# Implementing Text Overlays in Video Projects

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCoreX](#){ .md-button }

## Introduction to Text Overlays

Text overlays are essential components in professional video editing. They allow you to add titles, subtitles, watermarks, captions, and other important text elements to your videos. With the Video Edit SDK for .NET, you can create sophisticated text overlays with precise control over appearance, positioning, and timing.

## Key Features and Capabilities

The SDK provides extensive customization options for text overlays, including:

- Custom font selection from system-installed fonts
- Complete control over font size, weight, and style
- Flexible color options for both text and background
- Precise positioning with multiple alignment options
- Timing control to set when text appears and disappears
- Transparency and opacity settings

## Implementation Example

The following code demonstrates how to create and configure a text overlay in your .NET application:

```cs
// Initialize the VideoEditCoreX object (assumed to be already created)
// var videoEdit = new VideoEditCoreX();

// Create a new text overlay object with your desired text
var textOverlay = new VisioForge.Core.Types.X.VideoEdit.TextOverlay("Hello world!");

// Set when the text should appear and for how long
// This example: text appears 2 seconds into the video and stays for 5 seconds
textOverlay.Start = TimeSpan.FromMilliseconds(2000);
textOverlay.Duration = TimeSpan.FromMilliseconds(5000);

// Define the position of the text on the video frame
// X and Y coordinates are measured in pixels from the top-left corner
textOverlay.X = 50;
textOverlay.Y = 50;

// Configure font properties for the text
textOverlay.FontFamily = "Arial";  // Set the font family
textOverlay.FontSize = 40;         // Set the font size in points
textOverlay.FontWidth = SkiaSharp.SKFontStyleWidth.Normal;   // Normal width characters
textOverlay.FontSlant = SkiaSharp.SKFontStyleSlant.Italic;   // Apply italic style
textOverlay.FontWeight = SkiaSharp.SKFontStyleWeight.Bold;   // Apply bold weight

// Set the text color to red
textOverlay.Color = SkiaSharp.SKColors.Red;

// Set a transparent background behind the text
// You could use any color with alpha channel for semi-transparency
textOverlay.BackgroundColor = SkiaSharp.SKColors.Transparent;

// Add the configured text overlay to your video project
videoEdit.Video_TextOverlays.Add(textOverlay);
```

## Positioning Options

The SDK uses the X and Y coordinates for absolute positioning. X and Y represent pixel coordinates from the top-left corner of the video frame:

```cs
// Position text at specific coordinates (in pixels)
textOverlay.X = 50;   // Horizontal position from left edge
textOverlay.Y = 50;   // Vertical position from top edge

// You can also position text in other areas:
// Bottom-right corner (assuming 1920x1080 video):
// textOverlay.X = 1820;  // 1920 - 100 for some margin
// textOverlay.Y = 980;   // 1080 - 100 for some margin

// Centered (assuming 1920x1080 video and measuring text size):
// textOverlay.X = 960;   // Half of video width
// textOverlay.Y = 540;   // Half of video height
```

## Working with Fonts

The SDK leverages the SkiaSharp library for powerful text rendering capabilities. This provides access to all system fonts and advanced typography features.

### Retrieving Available Font Families

You can dynamically retrieve the list of fonts available on the current system:

```cs
// Get all available fonts on the current system
// This is useful for creating font selection dropdowns in your UI
var availableFonts = videoEdit.Fonts;

// You can now iterate through the fonts or bind them to a UI control
foreach (var font in availableFonts)
{
    // Use the font information as needed
    Console.WriteLine(font);
}
```

## Advanced Styling Techniques

For more sophisticated text effects, consider combining text overlays with other SDK features:

- Apply animation effects to make text move across the screen
- Use multiple text overlays with different timing for sequential display
- Combine with shape overlays to create text boxes with custom backgrounds
- Integrate with video transitions for dynamic text entry and exit effects

## Development Resources

For more code examples and advanced implementation techniques, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) containing comprehensive .NET SDK samples.
