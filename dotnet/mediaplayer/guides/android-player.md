---
title: Android Media Player SDK: Video Player for Apps
description: Build Android player apps with video playback, streaming, hardware acceleration, and multiple format support using Media Player SDK.
---

# Android Media Player SDK - Professional Video Playback Solution

[Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview

The VisioForge Android Player SDK enables developers to integrate professional video playback, streaming, and editing into native Android apps. Built on GStreamer, it provides a comprehensive API for feature-rich applications.

The SDK supports extensive video formats, codecs, and streaming protocols.

## Key Features

### Video Playback and Streaming

Our Android player SDK delivers powerful playback with hardware acceleration, ensuring optimal performance for high-resolution content. Developers integrate the player using an intuitive API with support for MP4, MKV, AVI, WebM, and other formats.

The player provides precise control with play, pause, seek, and navigation. Variable playback speeds and frame-by-frame navigation give complete control over the viewing experience.

Stream content from various sources including HTTP Live Streaming (HLS), RTSP, and RTMP. Adaptive bitrate streaming adjusts quality based on bandwidth for mobile users.

### Video Editing and Effects

The SDK includes video editing capabilities for creating editor applications. Apply real-time effects including brightness, contrast, and saturation adjustments.

Overlay text, images, and SVG graphics with control over positioning and transparency for picture-in-picture, watermarks, and interactive elements.

### Native Android and Cross-Platform Support

The SDK integrates seamlessly with Android Studio, supporting Java and Kotlin development. The VideoView component embeds into any Android layout.

The SDK also supports .NET MAUI and Avalonia for cross-platform development, enabling code sharing across Android, iOS, Windows, macOS, and Linux.

## Technical Capabilities

### Codec and Format Support

The SDK supports extensive video codecs with hardware-accelerated decoding for H.264, H.265/HEVC, VP8, and VP9. Audio playback supports AAC, MP3, Opus, and Vorbis.

### API and Performance

Our API reference provides detailed documentation. Code samples demonstrate common use cases. Events and callbacks provide real-time notifications.

The SDK is optimized for mobile with attention to battery and memory. Hardware acceleration ensures smooth playback.

## Getting Started

### Installation and Setup

Integrate the VisioForge Android Player SDK using NuGet. Add the package reference to your project. For .NET MAUI, configure to use the VideoView control.

Setup instructions are in our documentation.

### Quick Start Code Example

Here's how to create a basic media player:

#### Add VideoView to Layout

```xml
<VisioForge.Core.UI.Android.VideoViewTX
    android:layout_width="fill_parent"
    android:layout_height="fill_parent"
    android:id="@+id/videoView" />
```

#### Initialize Player

```csharp
using VisioForge.Core.MediaPlayerX;

public class MainActivity : Activity
{
    private MediaPlayerCoreX _player;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        SetContentView(Resource.Layout.activity_main);

        var videoView = FindViewById<VisioForge.Core.UI.Android.VideoViewTX>(Resource.Id.videoView);
        _player = new MediaPlayerCoreX(videoView);
    }

    protected override void OnDestroy()
    {
        VisioForgeX.DestroySDK();
        base.OnDestroy();
    }
}
```

#### Playback Controls

```csharp
private async void PlayVideo()
{
    await _player.OpenAsync(new Uri("https://example.com/video.mp4"));
    await _player.PlayAsync();
}

private async void PauseVideo() => await _player.PauseAsync();
private async void ResumeVideo() => await _player.ResumeAsync();
private async void StopVideo() => await _player.StopAsync();
```

### Sample Applications

GitHub samples demonstrate SDK capabilities: [Media Player sample](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Player%20SDK%20X/Android/MediaPlayer) with playback, streaming, and editing.

### Alternative: Media Blocks SDK

The [Media Blocks SDK](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Media%20Blocks%20SDK/Android/MediaPlayer) provides lower-level API for custom pipelines.

## Use Cases

The Android Player SDK is ideal for:

- **Video Streaming Apps**: Adaptive streaming support
- **Educational Platforms**: Video lessons and e-learning
- **Media Players**: Native mediaplayer apps with subtitle support
- **Social Media**: User-generated content playback
- **Video Editors**: Mobile editing with real-time preview
- **Security**: Surveillance apps with live streaming

The SDK supports Android TV and picture-in-picture mode on Android 8.0+.

## Licensing

The Android Player SDK is available under a commercial license. A single license covers all supported platforms. Trial versions are available.

## Conclusion

The VisioForge Android Player SDK provides professional video playback for Android applications. With streaming, editing, and advanced features, developers can create powerful media apps quickly.

For more information, visit our [product page](https://www.visioforge.com/media-player-sdk-net) or [API documentation](https://api.visioforge.org/dotnet/api/index.html).

## Related Resources

- [Android Implementation Guide](../../deployment-x/Android.md) - Detailed deployment instructions for Android
- [Code Samples](../code-samples/index.md) - Working examples and snippets
- [Cross-Platform Avalonia Player Guide](../guides/avalonia-player.md) - Building cross-platform video apps
- [Changelog](../../changelog.md) - Latest updates and releases
- [End User License Agreement](../../../eula.md) - Licensing information
