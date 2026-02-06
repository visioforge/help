---
title: Delphi Media Framework for Video Processing
description: Powerful Delphi/ActiveX libraries for video playback, capture, and editing with x64 support for professional media applications.
sidebar_label: All-in-One Media Framework (Delphi/ActiveX)
order: 18
---

# All-in-One Media Framework

A set of Delphi/ActiveX libraries for video processing, playback, and capture called All-in-One Media Framework. These libraries help developers create professional video editing, playback, and capture applications with minimal effort and maximum performance.

The framework provides a comprehensive solution for media handling in Delphi applications, offering high-performance video processing capabilities that would otherwise require extensive low-level programming. Developers can implement complex video workflows with simple component-based architecture.

You can find the following library documentation here:

## Libraries

- [TVFMediaPlayer](mediaplayer/index.md) - Full-featured media player component with playlist support, frame-accurate seeking, and advanced playback controls
- [TVFVideoCapture](videocapture/index.md) - Powerful video capture component supporting webcams, capture cards, IP cameras, and screen recording
- [TVFVideoEdit](videoedit/index.md) - Professional video editing component with timeline support, transitions, filters, and output to multiple formats

## Implementation Examples

The framework includes numerous examples demonstrating how to implement common media tasks:

- Video players with custom controls and visualizations
- Multi-camera recording applications
- Video editing software with timeline support
- Format conversion utilities
- Streaming media applications

## General Information

ActiveX packages can be used in multiple programming languages and development environments including Visual C++, Visual Basic, and C++ Builder. These components extend your software capabilities, accelerating development and improving performance. With ActiveX integration, you can incorporate existing software components into your projects, boosting efficiency and functionality.

Our framework is compatible with all Delphi versions from Delphi 6 to Delphi 11 and beyond, making it suitable for both legacy projects and new development. The components maintain a consistent API across different Delphi versions, simplifying migration between different IDE versions.

## Technical Specifications

- **Supported Media Formats**: MP4, AVI, MOV, MKV, MPEG, WMV and many others
- **Audio Support**: AAC, MP3, PCM, WMA and other popular audio codecs
- **Video Codecs**: H.264, H.265/HEVC, MPEG-4, VP9, AV1 and more
- **Capture Sources**: Webcams, HDMI capture cards, IP cameras, screen capture
- **Hardware Acceleration**: NVIDIA NVENC, Intel Quick Sync, AMD AMF

## x64 Support Limitations

With Delphi XE2 and later, you can develop 64-bit applications. Our framework fully supports these 64-bit applications, allowing you to leverage modern computing power and handle larger memory requirements. 64-bit support enables processing of higher resolution videos and more complex editing operations that would be impossible in 32-bit environments.

Microsoft Visual Basic 6 does not support 64-bit applications. If you're using Visual Basic 6, you'll need to use the 32-bit version of our framework due to VB6's inherent limitations. While 32-bit applications can access up to 4GB of memory with proper configuration, for demanding video applications, we recommend using Delphi or other development environments with 64-bit support.

## Development Best Practices

When integrating the framework into your applications, consider these best practices:

- Initialize components at design time when possible for better IDE integration
- Use hardware acceleration for demanding operations like encoding and decoding
- Implement proper error handling for media operations
- Consider memory management for large media files
- Test with various media sources to ensure compatibility

---

For more information about the framework, visit the [All-in-One Media Framework (Delphi/ActiveX)](https://www.visioforge.com/all-in-one-media-framework) product page.
