---
title: Set Individual Audio Track Volume Level in C# .NET Editor
description: Control individual audio track volumes in video editing apps with VisioForge Video Edit SDK .NET. Per-track mixing with C# code examples.
tags:
  - Video Edit SDK
  - .NET
  - VideoEditCore
  - Windows
  - Editing
  - Effects
  - C#
  - NuGet
primary_api_classes:
  - AudioVolumeEnvelopeEffect
  - AudioSource

---

# Setting Custom Volume Levels for Audio Tracks in C# Applications

[Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoEditCore](#){ .md-button }

## Overview

Managing audio volume levels is a critical aspect of video production and editing applications. This guide demonstrates how to implement individual volume controls for separate audio tracks in your .NET application.

## Implementation Details

Setting custom volume levels for audio tracks gives your users more precise control over their audio mix. Each track can have its own independent volume setting, allowing for professional-quality audio balancing.

## Sample Code Implementation

The following C# example shows how to apply a volume envelope effect to an audio track:

```cs
// AudioVolumeEnvelopeEffect(level) sets a constant volume for the track.
// Optional StartTime / StopTime (TimeSpan) restrict the effect to a time window.
var volume = new AudioVolumeEnvelopeEffect(level: 10);

// The 5-arg Input_AddAudioFile overload takes an AudioSource (not a string),
// so wrap the file path before passing it alongside the effect array.
// Signature: Input_AddAudioFile(AudioSource, TimeSpan? timelineInsertTime = null,
//   int targetStreamIndex = 0, AudioTrackEffect[] effects = null,
//   TimelineAudioTrackCustomSettings = null)
var audioSource = new AudioSource(audioFile);
VideoEdit1.Input_AddAudioFile(audioSource, null, 0, new[] { volume });
```

## Understanding the Parameters

- `AudioVolumeEnvelopeEffect(level: 10)`: constant-volume envelope. Level is an `int`; use `StartTime`/`StopTime` properties (both `TimeSpan`) to scope the effect to a time window.
- `audioFile` (string) must be wrapped in a `new AudioSource(...)` — the effects-array overload of `Input_AddAudioFile` only accepts an `AudioSource`, not a raw filename.
- `Input_AddAudioFile`: adds the audio file to the timeline with the given effects applied to the chosen audio stream.

## Required Dependencies

To implement this functionality, you'll need the following redistributable packages:

- Video Edit SDK redistributables:
  - [x86 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x86/)
  - [x64 package](https://www.nuget.org/packages/VisioForge.DotNet.Core.Redist.VideoEdit.x64/)

## Deployment Information

For information about installing or deploying the required components to your end users' systems, please refer to our [deployment guide](../deployment.md).

---
## Additional Resources
For more code examples and implementation techniques, visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) with complete sample projects.