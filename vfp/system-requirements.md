---
title: System Requirements for Video Fingerprinting SDK
description: Hardware and software requirements for running VisioForge Video Fingerprinting SDK on Windows, Linux, and macOS platforms
---

# System Requirements

This page outlines the system requirements for running the Video Fingerprinting SDK on supported platforms. These requirements apply to both the .NET and C++ versions of the SDK.

## Supported Platforms

### Windows

- **Operating System**: Windows 10 version 1903+ or Windows 11
- **Architecture**: x86, x64, or ARM64 (ARM64 for .NET SDK only)
- **Runtime**: Microsoft Visual C++ Redistributables 2019 or later
- **Additional Libraries**: DirectShow filters for enhanced codec support (optional)

### Linux

- **Distributions**: Ubuntu 20.04+, Debian 11+, RHEL 8+, CentOS 8+, Fedora 34+ (Fedora for C++ SDK)
- **Architecture**: x64 or ARM64
- **Dependencies**: GStreamer 1.18+ with base and good plugins
- **Display Server**: X11 or Wayland (for .NET SDK with GUI)

### macOS

- **Operating System**: macOS 12 (Monterey) or later
- **Architecture**: Intel x64 or Apple Silicon (M1/M2/M3)
- **Dependencies**: No additional runtime dependencies required

## Hardware Requirements

### Minimum Requirements

- **Processor**: Dual-core CPU (Intel Core i3 or AMD equivalent)
- **Memory**: 
  - .NET SDK: 4 GB RAM
  - C++ SDK: 2 GB RAM available for application
- **Storage**: 
  - .NET SDK: 500 MB free disk space for SDK
  - C++ SDK: 100 MB for SDK files + space for video processing
- **GPU**: Any DirectX 9 compatible graphics card (Windows only)

### Recommended Requirements

- **Processor**: Quad-core CPU (Intel Core i5 or AMD Ryzen 5)
- **Memory**: 
  - .NET SDK: 8 GB RAM or more
  - C++ SDK: 4 GB RAM or more available for application
- **Storage**: 
  - .NET SDK: 2 GB free disk space for SDK and temporary files
  - C++ SDK: 500 MB for SDK + temporary processing space
- **GPU**: Dedicated graphics card with hardware acceleration support

### Performance Considerations

- **Processing Speed**: Scales linearly with video duration and CPU cores
- **Memory Usage**: Increases with video resolution
- **Storage**: SSD storage significantly improves processing speed (2-3x faster I/O operations)
- **Parallelization**: Multiple CPU cores enable parallel processing
- **Hardware Acceleration**: Hardware video decoding can provide 3-5x speedup (C++ SDK)

## Development Environment Requirements

### .NET SDK

- **.NET Version**: 
  - Windows: .NET Framework 4.6.1+ or .NET 6.0+
  - Linux/macOS: .NET 6.0+
- **IDE**: Visual Studio 2019+ (Windows), Visual Studio Code, or JetBrains Rider

### C++ SDK

- **Windows Compiler**: Visual Studio 2019+ (recommended) or MinGW-w64
- **Linux Compiler**: GCC 7+ or Clang 6+
- **macOS Compiler**: Xcode 12+ with Command Line Tools
- **Build Tools**: CMake 3.10+ (optional but recommended for Linux/macOS)

## Additional Platform-Specific Notes

### Windows
- DirectShow filters can enhance codec support for legacy formats
- Windows Media Foundation is used for hardware acceleration when available

### Linux
- Ensure GStreamer plugins are properly installed: `sudo apt-get install gstreamer1.0-plugins-base gstreamer1.0-plugins-good`
- For GUI applications, X11 or Wayland display server is required

### macOS
- Apple Silicon (M1/M2/M3) processors are fully supported with native performance
- Rosetta 2 translation is supported for Intel binaries on Apple Silicon

## Network Requirements

For cloud-based fingerprint storage and comparison:
- **Bandwidth**: Minimum 1 Mbps for fingerprint upload/download
- **Latency**: < 100ms for real-time processing scenarios
- **Protocols**: HTTPS for API communication, MongoDB wire protocol for database operations

## Virtualization and Container Support

- **Docker**: Fully supported on Linux hosts
- **Virtual Machines**: Supported with performance overhead (20-30% slower)
- **WSL2**: Supported for Linux SDK on Windows
- **Cloud Platforms**: Compatible with AWS EC2, Azure VMs, Google Cloud Compute

## Next Steps

- [.NET SDK Getting Started](dotnet/getting-started.md) - Set up the .NET SDK
- [C++ SDK Getting Started](cpp/getting-started.md) - Set up the C++ SDK
- [Understanding Video Fingerprinting](understanding-video-fingerprinting.md) - Learn how the technology works