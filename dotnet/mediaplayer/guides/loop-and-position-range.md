---
title: Loop & Position Range - Media Player SDK .NET
description: Implement loop playback and segment control in .NET. Learn DirectShow and GStreamer features for video loops and position range selection.
keywords: video loop, media player loop, position range playback, segment playback, DirectShow loop, GStreamer loop, video player SDK, continuous playback, kiosk video loop, seamless loop
sidebar_label: Loop Mode & Position Range
order: 2

---

# Loop Mode and Position Range Playback

This guide explains how to use loop mode and custom start-stop position (position range) features in the Media Player SDK for both MediaPlayerCore (DirectShow engine) and MediaPlayerCoreX (GStreamer engine).

## Overview

Both Media Player engines support:

- **Loop Mode**: Automatically restart playback when media reaches the end
- **Position Range**: Play only a specific segment of media between start and stop positions
- **Combined Mode**: Loop a specific segment of media continuously

These features are useful for:

- Creating video loops for kiosks or displays
- Preview playback of specific segments
- Testing specific portions of media files
- Creating seamless background video loops
- Educational applications showing repeated content

## MediaPlayerCore (DirectShow Engine)

MediaPlayerCore is the Windows-only DirectShow-based media player engine.

### Loop Mode Properties

#### `Loop` Property

Enables or disables automatic loop playback.

```csharp
// Enable loop mode
mediaPlayer.Loop = true;

// Disable loop mode
mediaPlayer.Loop = false;
```

**Default value**: `false`

**Behavior**:
- When enabled, playback automatically restarts from the beginning when the end is reached
- The `OnLoop` event fires each time playback restarts
- For playlists, loops the entire playlist, not individual files

#### `Loop_DoNotSeekToBeginning` Property

Controls whether to seek to the beginning when loop restarts.

```csharp
// Restart without seeking to beginning (seamless loop)
mediaPlayer.Loop_DoNotSeekToBeginning = true;

// Seek to beginning before restarting (default)
mediaPlayer.Loop_DoNotSeekToBeginning = false;
```

**Default value**: `false`

**Behavior**:
- Only affects behavior when `Loop` is `true`
- When `true`, restarts from current position without seeking
- Improves performance and avoids visual glitches during loop transitions
- Useful for seamless looping of content

### Position Range Properties

#### `Selection_Active` Property

Enables or disables playback range selection.

```csharp
// Enable position range playback
mediaPlayer.Selection_Active = true;

// Disable position range playback (play entire file)
mediaPlayer.Selection_Active = false;
```

**Default value**: `false`

**Behavior**:
- When active, playback is constrained between `Selection_Start` and `Selection_Stop`
- The player automatically stops or loops when reaching `Selection_Stop`
- Useful for playing specific segments or creating clip previews

#### `Selection_Start` Property

Sets the start position for range-based playback.

```csharp
// Start playback at 30 seconds
mediaPlayer.Selection_Start = TimeSpan.FromSeconds(30);

// Start playback at 2 minutes 15 seconds
mediaPlayer.Selection_Start = new TimeSpan(0, 2, 15);
```

**Type**: `TimeSpan`

**Requirements**:
- Only used when `Selection_Active` is `true`
- Must be less than `Selection_Stop`
- Must be within media duration
- The player automatically seeks to this position when starting

#### `Selection_Stop` Property

Sets the end position for range-based playback.

```csharp
// Stop playback at 1 minute
mediaPlayer.Selection_Stop = TimeSpan.FromSeconds(60);

// Play to the end of file
mediaPlayer.Selection_Stop = TimeSpan.Zero;
```

**Type**: `TimeSpan`

**Requirements**:
- Only used when `Selection_Active` is `true`
- Must be greater than `Selection_Start`
- Use `TimeSpan.Zero` to play to the end of the media file
- When playback reaches this position, it stops (or loops if `Loop` is enabled)

### MediaPlayerCore Events

#### `OnLoop` Event

Fires each time playback restarts in loop mode.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("Playback looped!");
    // Update loop counter, perform actions at loop point, etc.
};
```

**When it fires**:
- Only when `Loop` property is `true`
- Each time playback cycles from end to beginning
- After seeking to the beginning (if applicable)

**Use cases**:
- Track loop iterations
- Update loop counters in UI
- Perform actions at each loop point
- Log playback statistics

### Code Examples for MediaPlayerCore

#### Example 1: Basic Loop Mode

```csharp
using VisioForge.Core.MediaPlayer;
using VisioForge.Core.Types;

// Create media player instance
var player = new MediaPlayerCore();

// Enable loop mode
player.Loop = true;

// Subscribe to loop event
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Loop iteration at {DateTime.Now}");
};

// Set source and play
player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\sample.mp4");
await player.PlayAsync();
```

#### Example 2: Seamless Loop Without Seeking

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Enable seamless loop (no seeking to beginning)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\background.mp4");
await player.PlayAsync();
```

#### Example 3: Play Specific Segment

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Enable position range
player.Selection_Active = true;

// Play from 1 minute to 2 minutes
player.Selection_Start = TimeSpan.FromMinutes(1);
player.Selection_Stop = TimeSpan.FromMinutes(2);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\long-video.mp4");
await player.PlayAsync();
```

#### Example 4: Loop Specific Segment

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

// Enable both loop and position range
player.Loop = true;
player.Selection_Active = true;

// Loop segment from 30 to 45 seconds
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(45);

// Track loop count
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Segment loop #{loopCount}");
};

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();
```

#### Example 5: Dynamic Position Range Update

```csharp
using VisioForge.Core.MediaPlayer;

var player = new MediaPlayerCore();

player.Selection_Active = true;
player.Selection_Start = TimeSpan.FromSeconds(10);
player.Selection_Stop = TimeSpan.FromSeconds(20);

player.Playlist_Clear();
player.Playlist_Add(@"C:\Videos\video.mp4");
await player.PlayAsync();

// Later, during playback, update the range
await Task.Delay(5000);

// Change to different segment
player.Selection_Start = TimeSpan.FromSeconds(30);
player.Selection_Stop = TimeSpan.FromSeconds(40);

// Seek to new start position
player.Position_Set_Time(player.Selection_Start);
```

## MediaPlayerCoreX (GStreamer Engine)

MediaPlayerCoreX is the cross-platform GStreamer-based media player engine, supporting Windows, Linux, macOS, Android, and iOS.

### Loop Mode Property

#### `Loop` Property

Enables or disables automatic loop playback.

```csharp
// Enable loop mode
mediaPlayer.Loop = true;

// Disable loop mode
mediaPlayer.Loop = false;
```

**Default value**: `false`

**Behavior**:
- When enabled, playback automatically restarts when end-of-stream (EOS) is reached
- The underlying `MediaBlocksPipeline` handles the loop logic
- The `OnLoop` event fires each time playback restarts
- Seamlessly restarts playback without seeking overhead

### Position Range Properties

#### `Segment_Start` Property

Sets the start position for segment playback.

```csharp
// Start playback at 45 seconds
mediaPlayer.Segment_Start = TimeSpan.FromSeconds(45);

// Start playback at 3 minutes
mediaPlayer.Segment_Start = TimeSpan.FromMinutes(3);
```

**Type**: `TimeSpan`

**Default value**: `TimeSpan.Zero`

**Behavior**:
- Defines where playback should begin
- The player automatically seeks to this position when starting
- Used in combination with `Segment_Stop` to define playback range
- Applied through the underlying `MediaBlocksPipeline.StartPosition` property

#### `Segment_Stop` Property

Sets the end position for segment playback.

```csharp
// Stop playback at 2 minutes
mediaPlayer.Segment_Stop = TimeSpan.FromMinutes(2);

// Play to the end (no stop position)
mediaPlayer.Segment_Stop = TimeSpan.Zero;
```

**Type**: `TimeSpan`

**Default value**: `TimeSpan.Zero`

**Behavior**:
- Defines where playback should end
- When playback reaches this position, end-of-stream (EOS) is triggered
- If `Loop` is enabled, playback restarts from `Segment_Start`
- Use `TimeSpan.Zero` for no stop position (play to end)
- Applied through the underlying `MediaBlocksPipeline.StopPosition` property

### MediaPlayerCoreX Events

#### `OnLoop` Event

Fires each time playback restarts in loop mode.

```csharp
mediaPlayer.OnLoop += (sender, e) =>
{
    Console.WriteLine("Playback looped!");
};
```

**When it fires**:
- Only when `Loop` property is `true`
- When end-of-stream (EOS) is reached
- Before playback restarts

### Code Examples for MediaPlayerCoreX

#### Example 1: Basic Loop Mode

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

// Create media player instance
var player = new MediaPlayerCoreX();

// Enable loop mode
player.Loop = true;

// Subscribe to loop event
player.OnLoop += (sender, e) =>
{
    Console.WriteLine($"Loop iteration at {DateTime.Now}");
};

// Set source and play
var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\sample.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Example 2: Play Specific Segment

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Set segment range: play from 30s to 90s
player.Segment_Start = TimeSpan.FromSeconds(30);
player.Segment_Stop = TimeSpan.FromSeconds(90);

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\long-video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Example 3: Loop Specific Segment

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Enable loop and set segment
player.Loop = true;
player.Segment_Start = TimeSpan.FromMinutes(1);
player.Segment_Stop = TimeSpan.FromMinutes(2);

// Track loop count
int loopCount = 0;
player.OnLoop += (sender, e) =>
{
    loopCount++;
    Console.WriteLine($"Segment loop #{loopCount}");
};

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Example 4: Cross-Platform Looping Video

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Enable seamless loop
player.Loop = true;

// For cross-platform video file path
string videoPath;
#if ANDROID
    videoPath = "/storage/emulated/0/Movies/background.mp4";
#elif IOS
    videoPath = Path.Combine(NSBundle.MainBundle.BundlePath, "background.mp4");
#else
    videoPath = Path.Combine(Environment.GetFolderPath(
        Environment.SpecialFolder.MyVideos), "background.mp4");
#endif

var source = new UniversalSourceSettings()
{
    URI = new Uri(videoPath)
};

await player.OpenAsync(source);
await player.PlayAsync();
```

#### Example 5: Segment Preview with UI Update

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Preview mode: show 10 second clips
TimeSpan clipDuration = TimeSpan.FromSeconds(10);

async Task PreviewSegment(TimeSpan startTime)
{
    player.Segment_Start = startTime;
    player.Segment_Stop = startTime + clipDuration;
    
    // Seek to start position
    await player.Position_SetAsync(startTime);
    
    Console.WriteLine($"Previewing segment: {startTime} to {startTime + clipDuration}");
}

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\movie.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();

// Preview first segment
await PreviewSegment(TimeSpan.FromSeconds(0));

// Preview segment starting at 30 seconds
await Task.Delay(11000);
await PreviewSegment(TimeSpan.FromSeconds(30));
```

#### Example 6: Loop with Pause-on-Stop

```csharp
using VisioForge.Core.MediaPlayerX;
using VisioForge.Core.Types.X.Sources;

var player = new MediaPlayerCoreX();

// Loop mode with pause instead of stop at end
player.Loop = true;
player.PauseOnStop = true; // Pause instead of stop at EOS

player.OnLoop += (sender, e) =>
{
    Console.WriteLine("Reached end, pausing briefly before loop...");
    
    // Wait before resuming (if needed)
    Task.Delay(1000).ContinueWith(_ => 
    {
        // Playback will restart automatically due to Loop = true
    });
};

var source = new UniversalSourceSettings()
{
    URI = new Uri(@"C:\Videos\video.mp4")
};

await player.OpenAsync(source);
await player.PlayAsync();
```

## Best Practices

### Performance Considerations

1. **Seamless Looping (MediaPlayerCore)**:
   - Use `Loop_DoNotSeekToBeginning = true` for seamless loops without visual glitches
   - Test with your specific media format for best results

2. **Short Segments**:
   - For very short segments (< 1 second), ensure media is properly indexed
   - Some formats may not support frame-accurate seeking

3. **Cross-Platform (MediaPlayerCoreX)**:
   - Test on all target platforms as GStreamer behavior may vary slightly
   - Use appropriate video codecs that support seeking (H.264, H.265)

### Common Use Cases

#### Kiosk Video Loop
```csharp
// MediaPlayerCore (Windows)
player.Loop = true;
player.Loop_DoNotSeekToBeginning = true;

// MediaPlayerCoreX (Cross-platform)
player.Loop = true;
```

#### Preview Window
```csharp
// MediaPlayerCore
player.Selection_Active = true;
player.Selection_Start = previewStart;
player.Selection_Stop = previewEnd;

// MediaPlayerCoreX
player.Segment_Start = previewStart;
player.Segment_Stop = previewEnd;
```

#### Continuous Segment Loop
```csharp
// MediaPlayerCore
player.Loop = true;
player.Selection_Active = true;
player.Selection_Start = segmentStart;
player.Selection_Stop = segmentEnd;

// MediaPlayerCoreX
player.Loop = true;
player.Segment_Start = segmentStart;
player.Segment_Stop = segmentEnd;
```

### Troubleshooting

#### Loop Not Working

**MediaPlayerCore**:
- Ensure `Loop` property is set before calling `PlayAsync()`
- Check that `OnStop` event is not calling `Stop()` method
- Verify media file is not corrupted

**MediaPlayerCoreX**:
- Ensure `Loop` property is set before calling `PlayAsync()`
- Check GStreamer is properly initialized
- Verify the pipeline is not being disposed prematurely

#### Position Range Not Accurate

**MediaPlayerCore**:
- Ensure `Selection_Active` is set to `true`
- Verify `Selection_Start` < `Selection_Stop`
- Some media formats may not support frame-accurate seeking

**MediaPlayerCoreX**:
- Verify `Segment_Start` and `Segment_Stop` are valid
- Use seekable media formats (MP4, MKV with proper indexing)
- Check that the media file supports seeking (not all streaming sources do)

#### Segment Playback Starts at Wrong Position

**Both Engines**:
- Ensure positions are set before calling `PlayAsync()`
- Wait for media to be fully loaded before setting positions
- Use keyframe-based seeking for better accuracy

## Property Comparison Table

| Feature | MediaPlayerCore | MediaPlayerCoreX |
|---------|-----------------|------------------|
| **Loop Mode** | `Loop` (bool) | `Loop` (bool) |
| **Seamless Loop** | `Loop_DoNotSeekToBeginning` (bool) | Built-in (no extra property) |
| **Range Active** | `Selection_Active` (bool) | Implicit (when Segment properties set) |
| **Start Position** | `Selection_Start` (TimeSpan) | `Segment_Start` (TimeSpan) |
| **Stop Position** | `Selection_Stop` (TimeSpan) | `Segment_Stop` (TimeSpan) |
| **Loop Event** | `OnLoop` | `OnLoop` |
| **Platform** | Windows only | Cross-platform |
| **Engine** | DirectShow | GStreamer |

## Related Documentation

- [MediaPlayerCore API Reference](https://api.visioforge.org/dotnet/api/VisioForge.Core.MediaPlayerX.MediaPlayerCoreX.html)
- [MediaPlayerCoreX API Reference](https://api.visioforge.org/dotnet/api/VisioForge.Core.MediaPlayerX.MediaPlayerCoreX.html)

## See Also

- [Media Player SDK Overview](../index.md)
- [Code Samples](../code-samples/index.md)
- [Additional Guides](index.md)
