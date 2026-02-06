---
title: Media Player SDK for Delphi and ActiveX Development
description: Comprehensive media playback SDK for Delphi and ActiveX with format support, advanced controls, video processing, and network streaming.
sidebar_label: TVFMediaPlayer
---

# TVFMediaPlayer: Feature-Rich Media Playback for Delphi & ActiveX

## Introduction to TVFMediaPlayer

The VisioForge TVFMediaPlayer library stands as a powerful and versatile solution designed for developers working with Delphi (VCL) and ActiveX-compatible environments (like .NET WinForms/WPF, VB6). It provides a robust framework for integrating sophisticated multimedia playback capabilities directly into custom applications. Whether you're building a simple video viewer, a complex media center application, a surveillance system interface, or interactive training software, TVFMediaPlayer offers the tools needed to handle a diverse range of audio and video requirements.

At its core, the library abstracts the complexities of various media codecs and streaming protocols, presenting a unified and relatively straightforward API. This allows developers to focus on application logic rather than low-level multimedia handling. The library emphasizes performance, stability, and extensive format support, making it a reliable choice for demanding playback scenarios.

## Core Features and Capabilities

TVFMediaPlayer is packed with features designed to address common and advanced media playback needs.

### Extensive Format and Codec Support

One of the library's most significant strengths is its ability to play back a vast array of media formats. This is achieved through flexible backend support:

* **System Codecs:** Leverages codecs already installed on the Windows operating system (DirectShow/Media Foundation). Ideal for common formats like AVI, WMV, and MP3 when appropriate decoders are present.
* **FFmpeg:** Integrates the renowned FFmpeg libraries, providing built-in support for a huge number of video and audio codecs and container formats without requiring external installations. This ensures broad compatibility out-of-the-box.
* **VLC Engine (libVLC):** Option to utilize the VLC engine, known for its excellent handling of various stream types and potentially problematic files.

This multi-pronged approach ensures that your application can handle almost any media file or stream thrown at it, minimizing compatibility issues for end-users.

### Advanced Playback Control

Beyond basic Play, Pause, Stop, and Seek operations, TVFMediaPlayer offers fine-grained control:

* **Variable Playback Rate:** Adjust playback speed (faster or slower) while optionally maintaining audio pitch.
* **Frame-Stepping:** Navigate video content frame by frame, essential for analysis or precise editing tasks.
* **Volume and Audio Control:** Adjust volume, mute audio, and potentially select specific audio tracks if multiple are available.
* **Seamless Looping:** Configure specific segments or the entire media file to loop continuously.

### Video Processing and Enhancement

Enhance the visual experience and extract information from video streams:

* **Overlays:** Easily add text, images (with transparency), or even graphical elements on top of the video playback. Useful for watermarking, displaying information, or custom controls.
* **Video Effects:** Apply real-time video effects such as brightness, contrast, saturation, hue adjustments, grayscale, inversion, and potentially more complex filters.
* **Frame Capture:** Capture snapshots of the currently playing video frame and save them to various image formats (BMP, JPG, PNG). This is useful for thumbnail generation, analysis, or documentation.
* **Zoom and Pan:** Allow users to digitally zoom into specific areas of the video and pan the view.

### Audio Processing and Enhancements

Refine the audio output:

* **Audio Equalizer:** Provide users with a multi-band equalizer to tailor the audio output to their preferences or environment.
* **Audio Enhancements:** Features like volume boosting beyond standard levels might be available.
* **Track Selection:** Explicitly select from multiple available audio tracks within a media file.

### Network Stream Playback

Effortlessly play streams from network sources:

* **Supported Protocols:** Handles common streaming protocols like HTTP, HTTPS, HLS (HTTP Live Streaming), RTSP, RTMP, and MMS.
* **Buffering Control:** Manage buffering settings to balance startup latency and playback smoothness, crucial for varying network conditions.

### Specialized Playback Features

* **Multi-Stream Files:** Uniquely handles video files containing multiple video streams (e.g., different camera angles), allowing seamless switching between them during playback.
* **DVD and Blu-ray:** Supports playback from DVD and Blu-ray discs, including menu navigation and chapter selection (requires appropriate system support and potentially decryption libraries for commercial discs).
* **Subtitle Integration:** Load and display subtitles from external files (like SRT, ASS, SSA, VobSub) or embedded subtitle tracks. Customize font, size, color, and position.

## Integration and Development

TVFMediaPlayer is designed for ease of integration into Delphi (VCL) and ActiveX host applications.

### Delphi Integration (VCL)

For Delphi developers, the library typically provides native VCL components. These components can be dropped onto a form in the IDE, and their properties and events can be configured visually and programmatically. This component-based approach significantly speeds up development compared to using raw APIs. 

### ActiveX Integration

The ActiveX control allows the media player to be embedded in any environment supporting ActiveX technology. This includes older platforms like Visual Basic 6, as well as .NET applications (Windows Forms, WPF) and even some web pages (though ActiveX in browsers is largely deprecated for security reasons). The ActiveX control exposes properties, methods, and events similar to the native Delphi components.

## Licensing Model

VisioForge typically offers flexible licensing:

* **Trial Version:** A fully functional trial version is usually available, allowing developers to evaluate the library thoroughly. Trial versions often overlay a watermark or display a nag screen.
* **Full License:** Purchasing a full license removes trial limitations. Full licenses offer free updates and priority support for one year. This ensures that developers have ongoing access to improvements and technical assistance.

It's crucial to consult the official VisioForge website or licensing documentation for precise terms and conditions.

## Resources and Further Information

To delve deeper into the capabilities and usage of the TVFMediaPlayer library, explore the following official resources:

* **Product Page:** [VisioForge Media Player SDK](https://www.visioforge.com/all-in-one-media-framework)
* **API Documentation:** [Delphi Media Player API Reference](https://api.visioforge.org/delphi/media_player_sdk/index.html)
* **Changelog:** [View recent updates and fixes](changelog.md)
* **Installation Guide:** [Steps for setting up the library](install/index.md)
* **Deployment:** [Information on distributing your application](deployment.md)
* **License Agreement:** [End User License Agreement](../../eula.md)

## Tutorials and Code Samples

Practical examples demonstrate how to implement specific features:

* [How to play a video file with several video streams?](file-multiple-video-streams.md)
* *(More tutorials can be added here as they become available)*

By leveraging the extensive features and flexible integration options of TVFMediaPlayer, developers can create compelling multimedia applications with rich playback experiences across various Windows platforms.
