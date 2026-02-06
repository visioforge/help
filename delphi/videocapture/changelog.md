---
title: TVFVideoCapture Library Version History
description: TVFVideoCapture version history - GPU acceleration, streaming capabilities, format updates from version 4.1 to 11.0 with detailed release notes.
---

# TVFVideoCapture Version History

## Release 11.00 - Enhanced GPU Encoding & Modern Delphi Support

- **Expanded Framework Compatibility**: Added support for Delphi 10.4 and 11.0 development environments
- **Advanced AMD GPU Acceleration**: Implemented MP4 (H264/AAC) video encoding utilizing AMD graphics processing units
- **Intel GPU Hardware Encoding**: Added MP4 (H264/AAC) video encoding through Intel integrated and discrete GPUs
- **NVIDIA CUDA Acceleration**: Introduced MP4 (H264/AAC) video encoding powered by NVIDIA graphics hardware
- **Container Format Improvements**: Enhanced MKV output with optimized performance and reliability
- **New Output Format**: Added MOV container format support for Apple ecosystem compatibility

## Release 10.0 - Performance Optimizations & Multi-Platform Support

- **MP4 Enhancement**: Thoroughly updated and improved MP4 output capabilities
- **Streaming Improvements**: Updated VLC source filter with enhanced RTMP and HTTPS support
- **Memory Management**: Fixed critical CUDA encoder memory leak for stable long-duration encoding
- **Resource Optimization**: Resolved FFMPEG source memory leak for improved application stability
- **Audio Capture**: Enhanced What You Hear filter for superior system audio recording
- **64-bit Architecture**: Added x64 VLC source for TVFMediaPlayer and TVFVideoCapture (both Delphi and ActiveX)
- **Extended Format Support**: Enhanced YUV2RGB filter with HDYC format support
- **Audio Encoding**: Updated LAME encoder with fix for low bitrate mono audio issues
- **Development Environment**: Added Delphi 10, 10.1 support for modern development workflows

## Release 8.7 - Core Engine Updates

- **VLC Integration**: Updated VLC engine to libVLC 2.2.1.0 for improved streaming capabilities
- **Decoder Enhancement**: Updated FFMPEG engine for better format compatibility and performance

## Release 8.6 - Reliability Improvements & Format Support

- **Resource Management**: Fixed critical memory leak for improved application stability
- **File Handling**: Resolved issues with improperly closed input and output files
- **New Format Support**: Added custom WebM filters based on the WebM project specifications

## Release 8.4 - Architecture Expansion

- **Modern Delphi**: Added Delphi XE8 support for latest development environments
- **64-bit Architecture**: Introduced Delphi and ActiveX x64 versions for performance on modern systems

## Release 8.31 - Development Environment Update

- **Framework Compatibility**: Added Delphi XE7 support for expanded development options

## Release 8.3 - API and Performance Improvements

- **Interface Enhancement**: Updated ActiveX API for improved developer experience
- **Decoder Optimization**: Enhanced FFMPEG decoder for better performance and format support
- **Stability**: Implemented several critical bug fixes and performance improvements

## Release 8.0 - Streaming Capabilities

- **Network Streaming**: Introduced VLC engine for IP video capture capabilities
- **Reliability**: Fixed several bugs for improved stability across all components

## Release 7.15 - Advanced Output Options & Security

- **Network Capture**: Improved IP capture engine for better connection stability and performance
- **Modern Format Support**: Added MP4 with H264/AAC output for industry-standard compatibility
- **Security Feature**: Implemented video encryption for protected content workflows
- **System Integration**: Added Virtual Camera output for software integration scenarios
- **Stability**: Multiple small bug fixes for improved reliability

## Release 7.0 - Capture Engine Improvements

- **Network Performance**: Enhanced IP capture engine with improved throughput and reliability
- **Desktop Capture**: Updated screen capture engine for better performance and quality
- **Output Options**: Enhanced FFMPEG output for expanded format support
- **Visual Effects**: Added Pan/zoom video effect for advanced video manipulation
- **Reliability**: Implemented multiple small bug fixes for improved stability

## Release 6.0 - Multi-Source & Windows 8 Compatibility

- **Advanced Compositing**: Improved Picture-In-Picture with support for any video source including screen capture and IP cameras
- **Streaming Protocol**: Enhanced RTSP sources support for better network video integration
- **Special Capture Mode**: Added layered windows screen capture support for complex UI recording
- **Hardware Support**: Implemented iCube cameras support for specialized imaging applications
- **OS Compatibility**: Added Windows 8 Developer Preview support for forward compatibility
- **Visual Processing**: Enhanced video effects with new options and improved performance
- **Audio Management**: Introduced multiple audio stream support for AVI and WMV outputs

## Release 5.5 - Stability & Feature Enhancements

- **Visual Processing**: Enhanced video effects with improved quality and performance
- **Network Video**: Improved IP cameras support for better connectivity and compatibility
- **Reliability**: Fixed several bugs for improved overall stability

## Release 5.4 - Modern Delphi Support

- **Development Environment**: Added Delphi XE2 support for modern application development
- **Stability**: Implemented several bug fixes for improved reliability

## Release 5.3 - Video Processing Improvements

- **Visual Effects**: Enhanced video effects with additional options and better performance
- **Network Video**: Improved IP cameras support for wider device compatibility
- **Reliability**: Fixed multiple bugs for more stable operation

## Release 5.2 - Frame Processing Enhancements

- **Visual Effects**: Improved video effects and video frame grabber functionality
- **Stability**: Fixed several bugs for enhanced reliability

## Release 5.1 - Network Video & Effects Improvements

- **IP Camera Integration**: Enhanced IP camera support for improved connectivity
- **Visual Processing**: Improved video effect quality and performance
- **Reliability**: Fixed various issues for better stability

## Release 5.0 - Major Format Support Expansion

- **Network Video**: Added RTSP/HTTP IP camera support (MJPEG/MPEG-4/H264 with or without audio)
- **Modern Format**: Implemented WebM output for open web standards compatibility
- **Format Flexibility**: Added MPEG-1/2/4 and FLV output using FFMPEG integration

## Release 4.22 - Screen Capture Improvements

- **Desktop Recording**: Fixed bugs in screen capture filter for improved recording quality

## Release 4.21 - Screen Capture Enhancements

- **Desktop Recording**: Implemented multiple bug fixes and improvements in screen capture filter

## Release 4.2 - Audio Processing Improvement

- **Sound Effects**: Enhanced audio effects filter with improved quality and performance

## Release 4.1 - Modern Delphi Integration

- **Development Environment**: Added Delphi 2010 support for the Delphi edition
- **Stability**: Fixed several bugs for improved reliability
