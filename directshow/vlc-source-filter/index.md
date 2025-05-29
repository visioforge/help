---
title: VLC Source DirectShow Filter for Media Playback
description: Integrate powerful VLC media capabilities into DirectShow applications with our robust filter component. Enable playback of diverse video files and network streams with hardware acceleration, 4K support, and advanced seeking capabilities.
sidebar_label: VLC Source DirectShow Filter
---

# VLC Source DirectShow Filter

## Overview

The VLC Source DirectShow filter empowers developers to seamlessly integrate advanced media playback capabilities into any DirectShow-based application. This powerful component enables smooth playback of various video files and network streams across multiple formats and protocols. 

Our SDK package delivers a complete solution with all necessary VLC player DLLs bundled alongside a flexible DirectShow filter. The package provides both standard file-selection interfaces and extensive options for custom filter configurations to match your specific development requirements.

For complete product details and licensing options, visit the [product page](https://www.visioforge.com/vlc-source-directshow-filter).

## Technical Specifications

### Supported DirectShow Interfaces

The filter implements these standard DirectShow interfaces for maximum compatibility:

- **IAMStreamSelect** - Comprehensive video and audio stream selection capabilities
- **IAMStreamConfig** - Advanced video and audio configuration settings
- **IFileSourceFilter** - Flexible specification of filename or URL sources
- **IMediaSeeking** - Robust timeline seeking and positioning support

### Key Features

- Hardware-accelerated decoding for optimal performance
- Support for 4K and 8K video playback
- Extensive format compatibility including modern codecs
- Network stream handling (RTSP, HLS, DASH, etc.)
- Subtitle rendering and management
- Multi-language audio track support
- 360° video playback capabilities
- HDR content support

## Version History

### Version 15.0

- Enhanced playback quality across numerous formats
- Improved subtitle rendering engine
- Updated codec implementations including dav1d, ffmpeg, and libvpx
- Added Super Resolution scaling with nVidia and Intel GPU acceleration

### Version 14.0

- Updated to VLC v3.0.18 core
- Fixed DxVA/D3D11 compatibility issues with HEVC content
- Resolved OpenGL resizing problems for smoother playback

### Version 12.0

- Upgraded to VLC v3.0.16 engine
- Added support for new Fourcc formats (E-AC3 and AV1)
- Fixed stability issues with VP9 streams

### Version 11.1

- Incorporated VLC v3.0.11
- Optimized HLS playlist update mechanism
- Enhanced WebVTT subtitle handling and display

### Version 11.0

- Built on VLC v3.0.10 foundation
- Fixed critical regression issues with HLS streams

### Version 10.4

- Major update to VLC 3.0 architecture
- Enabled hardware decoding by default for 4K and 8K content
- Added 10-bit color depth and HDR support
- Implemented 360-degree video and 3D audio capabilities
- Introduced Blu-Ray Java menu support

### Version 10.0

- Initial release as a standalone DirectShow filter
- For earlier version history, please refer to Video Capture SDK .Net changelog

## Additional Resources

- [End User License Agreement](../../eula.md)
- [Code Samples](https://github.com/visioforge/)
