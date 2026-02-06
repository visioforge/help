---
title: Play Video & Audio File Segments in C# .NET Apps
description: Play precise time-based segments of video and audio files with Media Player SDK for Windows and cross-platform .NET applications.
---

# Playing Media File Fragments: Implementation Guide for .NET Developers

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

When developing media applications, one frequently requested feature is the ability to play specific segments of a video or audio file. This functionality is crucial for creating video editors, highlight reels, educational platforms, or any application requiring precise media segment playback.

## Understanding Fragment Playback in .NET Applications

Fragment playback allows you to define specific time segments of a media file for playback, effectively creating clips without modifying the source file. This technique is particularly useful when you need to:

- Create preview segments from longer media files
- Focus on specific sections of instructional videos
- Create looping segments for demonstrations or presentations
- Build clip-based media players for sports highlights or video compilations
- Implement training applications that focus on specific video segments

The Media Player SDK .NET provides two primary engines for implementing fragment playback, each with its own approach and platform compatibility considerations.

## Windows-Only Implementation: MediaPlayerCore Engine

The MediaPlayerCore engine provides a straightforward implementation for Windows applications. This solution works across WPF, WinForms, and console applications but is limited to Windows operating systems.

### Setting Up Fragment Playback

To implement fragment playback with the MediaPlayerCore engine, you'll need to follow three key steps:

1. Activate the selection mode on your MediaPlayer instance
2. Define the starting position of your fragment (in milliseconds)
3. Define the ending position of your fragment (in milliseconds)

### Implementation Example

The following C# code demonstrates how to configure fragment playback to play only the segment between 2000ms and 5000ms of your source file:

```csharp
// Step 1: Enable fragment selection mode
MediaPlayer1.Selection_Active = true;

// Step 2: Set the starting position to 2000 milliseconds (2 seconds)
MediaPlayer1.Selection_Start = TimeSpan.FromMilliseconds(2000);

// Step 3: Set the ending position to 5000 milliseconds (5 seconds)
MediaPlayer1.Selection_Stop = TimeSpan.FromMilliseconds(5000);

// When you call Play() or PlayAsync(), only the specified fragment will play
```

When your application calls the Play or PlayAsync method after setting these properties, the player will automatically jump to the selection start position and stop playback when it reaches the selection end position.

### Required Redistributables for Windows Implementation

For the MediaPlayerCore engine implementation to function correctly, you must include:

- Base redistributable package
- SDK redistributable package

These packages contain the necessary components for the Windows-based playback functionality. For detailed information on deploying these redistributables to end-user machines, refer to the [deployment documentation](../deployment.md).

## Cross-Platform Implementation: MediaPlayerCoreX Engine

For developers requiring fragment playback functionality across multiple platforms, the MediaPlayerCoreX engine provides a more versatile solution. This implementation works across Windows, macOS, iOS, Android, and Linux environments.

### Setting Up Cross-Platform Fragment Playback

The cross-platform implementation follows a similar conceptual approach but uses different property names. The key steps include:

1. Creating a MediaPlayerCoreX instance
2. Loading your media source
3. Defining the segment start and stop positions
4. Initiating playback

### Cross-Platform Implementation Example

The following example demonstrates how to implement fragment playback in a cross-platform .NET application:

```csharp
// Step 1: Create a new instance of MediaPlayerCoreX with your video view
MediaPlayerCoreX MediaPlayer1 = new MediaPlayerCoreX(VideoView1);

// Step 2: Set the source media file
var fileSource = await UniversalSourceSettings.CreateAsync(new Uri("video.mkv"));
await MediaPlayer1.OpenAsync(fileSource);

// Step 3: Define the segment start time (2 seconds from beginning)
MediaPlayer1.Segment_Start = TimeSpan.FromMilliseconds(2000);

// Step 4: Define the segment end time (5 seconds from beginning)
MediaPlayer1.Segment_Stop = TimeSpan.FromMilliseconds(5000);

// Step 5: Start playback of the defined segment
await MediaPlayer1.PlayAsync();
```

This implementation uses the Segment_Start and Segment_Stop properties instead of the Selection properties used in the Windows-only implementation. Also note the asynchronous approach used in the cross-platform example, which improves UI responsiveness.

## Advanced Fragment Playback Techniques

### Dynamic Fragment Adjustment

In more complex applications, you might need to adjust fragment boundaries dynamically. Both engines support changing the segment boundaries during runtime:

```csharp
// For Windows-only implementation
private void UpdateFragmentBoundaries(int startMs, int endMs)
{
    MediaPlayer1.Selection_Start = TimeSpan.FromMilliseconds(startMs);
    MediaPlayer1.Selection_Stop = TimeSpan.FromMilliseconds(endMs);
    
    // If playback is in progress, restart it to apply the new boundaries
    if (MediaPlayer1.State == PlaybackState.Playing)
    {
        MediaPlayer1.Position_Set(MediaPlayer1.Selection_Start);
    }
}

// For cross-platform implementation
private async Task UpdateFragmentBoundariesAsync(int startMs, int endMs)
{
    MediaPlayer1.Segment_Start = TimeSpan.FromMilliseconds(startMs);
    MediaPlayer1.Segment_Stop = TimeSpan.FromMilliseconds(endMs);
    
    // If playback is in progress, restart from the new start position
    if (await MediaPlayer1.StateAsync() == PlaybackState.Playing)
    {
        await MediaPlayer1.Position_SetAsync(MediaPlayer1.Segment_Start);
    }
}
```

### Multiple Fragment Playback

For applications that need to play multiple fragments sequentially, you can implement a fragment queue:

```csharp
public class MediaFragment
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}

private Queue<MediaFragment> fragmentQueue = new Queue<MediaFragment>();
private bool isProcessingQueue = false;

// Add fragments to the queue
public void EnqueueFragment(TimeSpan start, TimeSpan end)
{
    fragmentQueue.Enqueue(new MediaFragment { StartTime = start, EndTime = end });
    
    if (!isProcessingQueue && MediaPlayer1 != null)
    {
        PlayNextFragment();
    }
}

// Process the fragment queue
private async void PlayNextFragment()
{
    if (fragmentQueue.Count == 0)
    {
        isProcessingQueue = false;
        return;
    }
    
    isProcessingQueue = true;
    var fragment = fragmentQueue.Dequeue();
    
    // Set the fragment boundaries
    MediaPlayer1.Segment_Start = fragment.StartTime;
    MediaPlayer1.Segment_Stop = fragment.EndTime;
    
    // Subscribe to completion event for this fragment
    MediaPlayer1.OnStop += (s, e) => PlayNextFragment();
    
    // Start playback
    await MediaPlayer1.PlayAsync();
}
```

### Performance Considerations

For optimal performance when using fragment playback, consider the following tips:

1. For frequent seeking between fragments, use formats with good keyframe density
2. MP4 and MOV files generally perform better for fragment-heavy applications
3. Setting fragments at keyframe boundaries improves seeking performance
4. Consider preloading files before setting fragment boundaries
5. On mobile platforms, keep fragments reasonably sized to avoid memory pressure

## Conclusion

Implementing fragment playback in your .NET media applications provides substantial flexibility and enhanced user experience. Whether you're developing for Windows only or targeting multiple platforms, the Media Player SDK .NET offers robust solutions for precise media segment playback.

By leveraging the techniques demonstrated in this guide, you can create sophisticated media experiences that allow users to focus on exactly the content they need, without the overhead of editing or splitting source files.

For more code samples and implementations, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) where you'll find comprehensive examples of media player implementations, including fragment playback and other advanced media features.
