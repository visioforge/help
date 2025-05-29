---
title: Audio Volume Envelope Effects for .NET Video Editing
description: Learn how to implement professional audio volume envelope effects in your .NET applications with this complete tutorial. Step-by-step code samples show you how to control audio levels precisely in your video editing projects.
sidebar_label: How to Make an Audio Volume Envelope Effect?

---

# Implementing Audio Volume Envelope Effects in .NET

[!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge variant="dark" size="xl" text="VideoEditCore"]

Audio volume envelopes are essential tools for professional video production, allowing developers to precisely control audio levels throughout a timeline. This tutorial demonstrates how to implement these effects in your .NET applications.

## What is an Audio Volume Envelope?

An audio volume envelope lets you adjust the volume levels of your audio track. Rather than manually adjusting volume throughout the editing process, envelopes provide a programmatic way to set consistent volume levels. This is particularly useful when working with multiple audio tracks that need to maintain specific volume relationships.

## Implementation Overview

The implementation process involves three key steps:

1. Creating an audio source from your file
2. Creating the volume envelope effect with your desired level
3. Adding the audio with the effect to your timeline

Each step requires specific code components that we'll explore in detail below.

## Understanding the AudioVolumeEnvelopeEffect Class

The `AudioVolumeEnvelopeEffect` class is the core component for implementing volume control:

```cs
public class AudioVolumeEnvelopeEffect : AudioTrackEffect
{
    /// <summary>
    /// Gets or sets level (in percents), range is [0-100].
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Initializes a new instance of the AudioVolumeEnvelopeEffect class. 
    /// </summary>
    /// <param name="level">
    /// Level (in percents), range is [0-100].
    /// </param>
    public AudioVolumeEnvelopeEffect(int level) 
    {
        Level = level;
    }
}
```

As you can see, this class:

- Inherits from `AudioTrackEffect`
- Has a `Level` property that accepts values from 0-100 (representing volume percentage)
- Provides a constructor for setting the initial level

## Detailed Implementation Steps

### 1. Creating Your Audio Source

The first step involves initializing an audio source object that references your audio file. This object serves as the foundation for applying effects.

```cs
var audioFile = new AudioSource(file, segments, null);
```

In this code:

- `file` is the path to your audio file
- `segments` defines time segments if you're only using portions of the audio
- The final parameter can contain additional options (null in this basic example)

### 2. Configuring the Volume Envelope Effect

Next, create and configure the volume envelope effect by specifying your desired volume level:

```cs
var envelope = new AudioVolumeEnvelopeEffect(70);
```

This creates a volume envelope effect set to 70%. The parameter accepts values from 0 to 100:

- 0 = completely silent
- 50 = half volume
- 100 = full volume

You can also adjust the level after creation:

```cs
var envelope = new AudioVolumeEnvelopeEffect(50);
envelope.Level = 75; // Changed to 75% volume
```

### 3. Adding Audio with Envelope Effect to Timeline

The final step is to add your audio source with the envelope effect applied to your project timeline:

```cs
VideoEdit1.Input_AddAudioFile(
    audioFile,                        // Your configured audio source
    TimeSpan.FromMilliseconds(0),     // Starting position on timeline
    0,                                // Track index
    new []{ envelope }                // Array of effects to apply
);
```

This positions your audio at the beginning of the timeline (0ms) and applies the envelope effect we configured earlier.

## Common Use Cases

### Normalizing Audio Levels

When working with audio from different sources, normalization ensures consistent volume levels:

```cs
// Main interview audio at full volume
var interviewAudio = new AudioSource("interview.mp3", null, null);
VideoEdit1.Input_AddAudioFile(interviewAudio, TimeSpan.Zero, 0, null);

// Background music at 30% volume to avoid overpowering speech
var backgroundMusic = new AudioSource("background.mp3", null, null);
var musicEnvelope = new AudioVolumeEnvelopeEffect(30);
VideoEdit1.Input_AddAudioFile(backgroundMusic, TimeSpan.Zero, 1, new[] { musicEnvelope });
```

### Muting Specific Sections

If you need to mute sections of audio in your timeline, you can create and apply different envelope effects:

```cs
// Create audio sources for different segments
var segment1 = new AudioSource("audio.mp3", GetSegment(0, 10000), null); // 0-10s
var segment2 = new AudioSource("audio.mp3", GetSegment(10000, 15000), null); // 10-15s
var segment3 = new AudioSource("audio.mp3", GetSegment(15000, 30000), null); // 15-30s

// Apply different volume levels
VideoEdit1.Input_AddAudioFile(segment1, TimeSpan.Zero, 0, new[] { new AudioVolumeEnvelopeEffect(100) });
// Mute middle segment
VideoEdit1.Input_AddAudioFile(segment2, TimeSpan.FromMilliseconds(10000), 0, new[] { new AudioVolumeEnvelopeEffect(0) });
VideoEdit1.Input_AddAudioFile(segment3, TimeSpan.FromMilliseconds(15000), 0, new[] { new AudioVolumeEnvelopeEffect(100) });
```

## Required Dependencies

To implement audio envelope effects, you'll need:

- Video Edit SDK .NET redistributable packages:
  - [x86 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [x64 version](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

You can install these packages via NuGet Package Manager:

```nuget
Install-Package VisioForge.DotNet.Core.Redist.VideoEdit.x64
```

For more information on deploying these dependencies to your users' systems, refer to our [deployment documentation](../deployment.md).

## Performance Considerations

When implementing audio volume effects, consider these performance tips:

- Apply envelope effects during the editing/rendering phase rather than at runtime
- When working with multiple tracks, consider the cumulative effect of all audio processing
- Test on your target hardware to ensure smooth playback

## Troubleshooting Common Issues

If you encounter problems with your audio envelope implementation:

- Verify audio file paths and formats are supported
- Check that volume percentages are within the 0-100 range
- Ensure the audio effect is correctly added to the effects array
- Verify timeline positioning doesn't create conflicts between audio segments

## Conclusion

Audio volume envelope effects provide essential control over your application's audio experience. By following this guide, you've learned how to implement volume control in your .NET video editing projects, balancing different audio sources for professional results.

---

For more code samples and advanced techniques, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).
