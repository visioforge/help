---
title: Advanced Video Encryption SDK for Developers
description: Integrate powerful video encryption capabilities into your DirectShow applications. Securely encrypt video files or streams with AES-256, support H264/AAC formats, and leverage GPU acceleration for optimal performance. Complete developer toolkit with code samples.
sidebar_label: Video Encryption SDK
---

# Video Encryption SDK

## Introduction to Video Encryption

The [Video Encryption SDK](https://www.visioforge.com/video-encryption-sdk) provides robust tools for encoding video files into MP4 H264/AAC format with advanced encryption capabilities. Developers can secure their media content using custom passwords or binary data encryption methods.

The SDK integrates seamlessly with any DirectShow application through a complete set of filters. These filters come with extensive interfaces allowing developers to fine-tune settings according to specific security requirements and implementation needs.

## Integration Flexibility

You can implement the SDK in various DirectShow applications as filters for both encryption and decryption processes. The system works effectively with:

- Live video sources
- File-based video sources
- Software video encoders
- GPU-accelerated video encoders from the [DirectShow Encoding Filters pack](https://www.visioforge.com/encoding-filters-pack) (available separately)
- Third-party DirectShow filters for additional video encoding options

## Key Features and Capabilities

### Core Functionality

- **Secure Encryption/Decryption**: Process video files or capture streams with robust security algorithms
- **Format Support**: Full H264 encoder support for video content
- **Audio Handling**: Complete AAC encoder support for audio streams
- **Flexible Security Options**: Implement encryption using either binary data or string passwords

### Performance Optimization

- AES-256 encryption engine for maximum security
- CPU hardware acceleration support
- GPU acceleration compatibility
- Optimized for high-speed encryption processes

## Development Resources

### Code Samples and Documentation

The SDK includes comprehensive code samples for multiple programming languages:

- C# implementation examples
- C++ reference code
- Delphi sample projects

These samples provide practical implementation guidance for developers building secure video applications.

### Demo Application

Explore the included Video Encryptor application for a hands-on demonstration of the SDK's capabilities in a working environment.

## Licensing Information

- [End User License Agreement](../../eula.md)

## Version History

### Version 11.4

- Full compatibility with VisioForge .Net SDKs 11.4
- Enhanced Nvidia NVENC support for H264 and H265 video encoders
- Improved Intel QuickSync support for H264 video encoder
- Added NV12 colorspace support for enhanced performance

### Version 11.0

- Complete compatibility with VisioForge .Net SDKs 11.0
- Enhanced GPU encoders support
- Upgraded AAC encoder functionality
  
### Version 10.0

- Full compatibility with VisioForge .Net SDKs 10.0
- Enhanced compatibility with H264 and H265 video formats
- Integrated AMD AMF acceleration support
- Added Intel QuickSync technology support

### Version 9.0

- Significantly improved encryption processing speed
- Added CPU hardware acceleration capabilities
- Implemented new engine based on AES-256 encryption
- Added file usage as a key (with binary array support)
- Integrated NVENC support for GPU acceleration
- Enhanced AAC HE encoder support

### Version 8.0

- Updated video and audio encoders
- Improved filter encryption performance

### Version 7.0

- Initial release as a standalone product
- Previously integrated within Video Capture SDK, Video Edit SDK, and Media Player SDK
- Compatible with any DirectShow application without requiring additional VisioForge SDKs
