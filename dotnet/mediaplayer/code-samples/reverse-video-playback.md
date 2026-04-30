---
title: Reverse Video Playback in .NET with C# Code Examples
description: Implement reverse video playback with frame-by-frame navigation using VisioForge Media Player SDK .NET on Windows and cross-platform targets.
tags:
  - Media Player SDK
  - .NET
  - MediaPlayerCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Playback
  - MP4
  - C#
primary_api_classes:
  - MediaPlayerCoreX
  - MediaPlayerCore
  - PlaybackState
  - UniversalSourceSettings

---

# Implementing Reverse Video Playback in .NET Applications

Playing video in reverse is a powerful feature for media applications, allowing users to review content, create unique visual effects, or enhance the user experience with non-linear playback options. This guide provides complete implementations for reverse playback in .NET applications, focusing on both cross-platform and Windows-specific solutions.

## Understanding Reverse Playback Mechanisms

Reverse video playback can be achieved through several techniques, each with distinct advantages depending on your application's requirements:

1. **Rate-based reverse playback** - Setting a negative playback rate to reverse the video stream
2. **Frame-by-frame backward navigation** - Manually stepping backward through cached video frames
3. **Buffer-based approaches** - Creating a frame buffer to enable smooth reverse navigation

Let's explore how to implement each approach using the Media Player SDK for .NET.

## Cross-Platform Reverse Playback with MediaPlayerCoreX

The MediaPlayerCoreX engine provides cross-platform support for reverse video playback with a straightforward implementation. This approach works across Windows, macOS, and other supported platforms.

### Basic Implementation

The simplest method for reverse playback involves setting a negative rate value:

```cs
// Create new instance of MediaPlayerCoreX
MediaPlayerCoreX MediaPlayer1 = new MediaPlayerCoreX(VideoView1);

// Set the source file
var fileSource = await UniversalSourceSettings.CreateAsync(new Uri("video.mp4"));
await MediaPlayer1.OpenAsync(fileSource);

// Start normal playback first
await MediaPlayer1.PlayAsync();

// Change to reverse playback with normal speed
MediaPlayer1.Rate_Set(-1.0);
```

### Controlling Reverse Playback Speed

You can control the reverse playback speed by adjusting the negative rate value:

```cs
// Double-speed reverse playback
MediaPlayer1.Rate_Set(-2.0);

// Half-speed reverse playback (slow motion in reverse)
MediaPlayer1.Rate_Set(-0.5);

// Quarter-speed reverse playback (very slow motion in reverse)
MediaPlayer1.Rate_Set(-0.25);
```

### Tracking Position During Reverse Playback

`MediaPlayerCoreX` does not raise a position-changed event; poll the position on a timer instead:

```cs
// Poll the player position every 100 ms and update the UI
var positionTimer = new System.Threading.Timer(_ =>
{
    TimeSpan currentPosition = MediaPlayer1.Position_Get();
    UpdatePositionUI(currentPosition);

    // Detect reaching the start while playing backwards
    if (MediaPlayer1.Rate_Get() < 0 && currentPosition <= TimeSpan.FromMilliseconds(100))
    {
        // Switch to forward playback (or pause)
        MediaPlayer1.Rate_Set(1.0);
    }
}, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
```

## Windows-Specific Frame-by-Frame Reverse Navigation

The classic `MediaPlayerCore` engine (Windows-only) provides enhanced frame-by-frame control with its frame caching system, allowing precise backward navigation even with codecs that don't natively support it. Declare a separate classic-engine instance — the `ReversePlayback_*` API lives on `MediaPlayerCore` and is **not** available on `MediaPlayerCoreX`.

### Setting Up Frame Caching

Before starting playback, configure the reverse playback cache:

```cs
// Classic Windows engine — different type from MediaPlayerCoreX above
MediaPlayerCore classicPlayer = new MediaPlayerCore(VideoView1);

// Configure reverse playback before starting
classicPlayer.ReversePlayback_CacheSize = 100; // Cache 100 frames
classicPlayer.ReversePlayback_Enabled = true;  // Enable the feature

// Start playback
await classicPlayer.PlayAsync();
```

### Navigating Frame by Frame

With the cache configured, you can navigate to previous frames:

```cs
// Navigate to the previous frame
classicPlayer.ReversePlayback_PreviousFrame();

// Navigate backward multiple frames
for(int i = 0; i < 5; i++)
{
    classicPlayer.ReversePlayback_PreviousFrame();
    // Optional: add delay between frames for controlled playback
    await Task.Delay(40); // ~25fps equivalent timing
}
```

### Advanced Frame Cache Configuration

For applications with specific memory or performance requirements, you can fine-tune the cache:

```cs
// For high-resolution videos, you might need fewer frames to manage memory
classicPlayer.ReversePlayback_CacheSize = 50; // Reduce cache size

// For applications that need extensive backward navigation
classicPlayer.ReversePlayback_CacheSize = 250; // Increase cache size
```

## Implementing UI Controls for Reverse Playback

A complete reverse playback implementation typically includes dedicated UI controls:

```cs
// Button click handler for reverse playback
private async void ReversePlaybackButton_Click(object sender, EventArgs e)
{
    if(MediaPlayer1.State == PlaybackState.Play)
    {
        // Toggle between forward and reverse
        if(MediaPlayer1.Rate_Get() > 0)
        {
            MediaPlayer1.Rate_Set(-1.0);
            UpdateUIForReverseMode(true);
        }
        else
        {
            MediaPlayer1.Rate_Set(1.0);
            UpdateUIForReverseMode(false);
        }
    }
    else
    {
        // Start playback in reverse
        await MediaPlayer1.PlayAsync();
        MediaPlayer1.Rate_Set(-1.0);
        UpdateUIForReverseMode(true);
    }
}

// Button click handler for frame-by-frame backward navigation
// (assumes the Windows-only classic engine `classicPlayer` declared above)
private async void PreviousFrameButton_Click(object sender, EventArgs e)
{
    // Ensure we're paused first — classic MediaPlayerCore exposes State() as a method (X is a property).
    if (classicPlayer.State() == PlaybackState.Play)
    {
        await classicPlayer.PauseAsync();
    }
    
    // Navigate to previous frame
    classicPlayer.ReversePlayback_PreviousFrame();
    UpdateFrameCountDisplay();
}
```

## Performance Considerations

Reverse playback can be resource-intensive, especially with high-resolution videos. Consider these optimization techniques:

1. **Limit cache size** for devices with memory constraints
2. **Use hardware acceleration** when available
3. **Monitor performance** during reverse playback with debugging tools
4. **Provide fallback options** for devices that struggle with full-speed reverse playback

```cs
// Example of performance monitoring during reverse playback.
// Rate tracking lives on MediaPlayerCoreX (`MediaPlayer1`);
// cache adjustment targets the classic MediaPlayerCore (`classicPlayer`).
private void MonitorPerformance()
{
    Timer performanceTimer = new Timer(1000);
    performanceTimer.Elapsed += (s, e) => 
    {
        if(MediaPlayer1.Rate_Get() < 0)
        {
            // Log or display current memory usage, frame rate, etc.
            LogPerformanceMetrics();
        }

        // Adjust classic-engine frame cache if memory pressure is high
        if(IsMemoryUsageHigh())
        {
            classicPlayer.ReversePlayback_CacheSize = 
                Math.Max(10, classicPlayer.ReversePlayback_CacheSize / 2);
        }
    };
    performanceTimer.Start();
}
```

## Required Dependencies

To ensure proper functionality of reverse playback features, include these dependencies:

- Base redistributable package
- SDK redistributable package

These packages contain the necessary codecs and media processing components to enable smooth reverse playback across different video formats.

## Additional Resources and Advanced Techniques

For complex media applications requiring advanced reverse playback features, consider exploring:

- Frame extraction and manual rendering for custom effects
- Keyframe analysis for optimized navigation
- Buffering strategies for smoother reverse playback

## Conclusion

Implementing reverse video playback adds significant value to media applications, providing users with enhanced control over content navigation. By following the implementation patterns in this guide, developers can create robust, performant reverse playback experiences in .NET applications.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page for more complete code samples and implementation examples.