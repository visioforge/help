---
title: Display First Frame in Video Files with .NET
description: Show the first video frame in WinForms, WPF, and Console applications with C# implementation examples using Media Player SDK.
---

# Displaying the First Frame of Video Files in .NET Applications

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [VideoCaptureCore](#){ .md-button }

## Overview

When developing media applications, it's often necessary to preview video content without playing the entire file. This technique is particularly useful for creating thumbnail galleries, video selection screens, or providing users with a visual preview before committing to watching a video.

## Implementation Guide

The Media Player SDK .NET provides a simple yet powerful way to display the first frame of any video file. This is achieved through the `Play_PauseAtFirstFrame` property, which when set to `true`, instructs the player to pause immediately after loading the first frame.

### How It Works

When the `Play_PauseAtFirstFrame` property is enabled:

1. The player loads the video file
2. Renders the first frame to the video display surface
3. Automatically pauses playback
4. Maintains the first frame on screen until further user action

If this property is not enabled (set to `false`), the player will proceed with normal playback after loading.

## Code Implementation

### Basic Example

```cs
// create player and configure the file name
// ...

// set the property to true
MediaPlayer1.Play_PauseAtFirstFrame = true;

// play the file
await MediaPlayer1.PlayAsync();
```

Resume playback from the first frame:

```cs
// resume playback
await MediaPlayer1.ResumeAsync();
```

## Practical Applications

This feature is particularly useful for:

- Providing preview capabilities in video editing applications
- Generating video poster frames for streaming applications
- Implementing "hover to preview" functionality in media browsers

## Required Components

To implement this functionality in your application, you'll need:

- Base redist package
- SDK redist package

For more information on distributing these components with your application, see: [How can the required redists be installed or deployed to the user's PC?](../deployment.md)

## Additional Resources

Find more code samples and implementation examples in our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples).

## Technical Considerations

When implementing this feature, keep in mind:

- First frame display is nearly instantaneous for most video formats
- Resource usage is minimal as the player doesn't buffer beyond the first frame
- Works with all supported video formats including MP4, MOV, AVI, and more
