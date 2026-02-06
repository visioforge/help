---
title: .NET Cross-Platform Deployment Guide for Ubuntu
description: Deploy .NET multimedia apps on Ubuntu Linux with GStreamer setup, hardware configuration, and cross-platform performance optimization.
---

# Ubuntu Deployment Guide for VisioForge SDK Applications

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction

Deploying .NET applications with VisioForge SDKs on Ubuntu Linux offers multiple benefits, including cross-platform compatibility, access to Linux-specific hardware, and the ability to run your multimedia applications on environments ranging from server infrastructure to edge devices. This comprehensive guide will walk you through the complete process of configuring your Ubuntu environment, installing the necessary dependencies, and deploying your VisioForge-powered .NET application.

The VisioForge SDK family works on Ubuntu and other Linux distributions that support `GStreamer` libraries. Additional supported platforms include `Nvidia Jetson` devices and `Raspberry Pi`, making it perfect for a wide range of applications from desktop multimedia software to IoT solutions.

## System Requirements

Before deploying your application, ensure your Ubuntu environment meets these minimum requirements:

- Ubuntu 20.04 LTS or later (22.04 LTS and later recommended)
- .NET 7.0 or later runtime
- At least 4GB RAM (8GB recommended for video processing)
- x86_64 or ARM64 architecture
- Internet connection for package installation

## Installation and Setup

### Installing .NET

Download the latest [.NET installer](https://dotnet.microsoft.com/en-us/download/dotnet) package from the Microsoft website and follow the installation instructions.

## GStreamer Installation

GStreamer forms the multimedia backbone for VisioForge SDKs on Linux platforms. It provides essential functionality for audio and video capture, processing, and playback.

### Required GStreamer Packages

Install the following GStreamer packages using apt-get. We require v1.22.0 or later, though v1.24.0+ is highly recommended for access to the latest features and optimizations:

- `gstreamer1.0-plugins-base`: Essential baseline plugins
- `gstreamer1.0-plugins-good`: High-quality, well-tested plugins
- `gstreamer1.0-plugins-bad`: Newer plugins of varying quality
- `gstreamer1.0-alsa`: ALSA audio support
- `gstreamer1.0-gl`: OpenGL rendering support
- `gstreamer1.0-pulseaudio`: PulseAudio integration
- `libges-1.0-0`: GStreamer Editing Services
- `gstreamer1.0-libav`: FFMPEG integration (OPTIONAL but recommended for broader format support)

### Complete Installation Script

The following commands will update your package repositories and install all required GStreamer components:

```bash
sudo apt update
```

```bash
sudo apt install gstreamer1.0-plugins-base gstreamer1.0-plugins-good gstreamer1.0-plugins-bad gstreamer1.0-alsa gstreamer1.0-gl gstreamer1.0-pulseaudio gstreamer1.0-libav libges-1.0-0
```

### Raspberry Pi Additional Requirements

For Raspberry Pi, additionally, you need to install the following packages:

```bash
sudo apt install gstreamer1.0-libcamera
```

### Verifying GStreamer Installation

After installation, verify your GStreamer setup by running:

```bash
gst-inspect-1.0 --version
```

This should display the installed GStreamer version. Ensure it meets the minimum requirement (1.22.0+) or ideally shows 1.24.0 or later.

## Required NuGet Packages

When deploying your .NET application to Ubuntu, you'll need to include additional platform-specific NuGet packages that provide the necessary native libraries and bindings.

### Additional Core Linux Package

The [VisioForge.CrossPlatform.Core.Linux.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Linux.x64) package contains essential native libraries and bindings for the .NET Linux platform. This package is mandatory for all VisioForge SDK deployments on Ubuntu.

### Development Environment

You can use Rider to develop your project in Linux. Please check the [Rider](../install/rider.md) installation page for more information.

## Application Deployment

Follow these steps to deploy your application on Ubuntu:

### Publishing Your Application

To create a self-contained deployment that includes all .NET runtime dependencies:

```bash
dotnet publish -c Release -r linux-x64 --self-contained true
```

For smaller deployments where the target machine already has .NET installed:

```bash
dotnet publish -c Release -r linux-x64 --self-contained false
```

### Deployment Structure

Your deployment folder should contain:

- Your application executable
- Application DLLs
- VisioForge SDK assemblies
- Native Linux libraries from the VisioForge NuGet packages

### Setting Execution Permissions

Ensure your application executable has the proper permissions:

```bash
chmod +x ./YourApplicationName
```

## Hardware Considerations

### Camera Support

Ubuntu supports various camera types:

- **USB Webcams**: Most USB webcams work out of the box
- **IP Cameras**: Supported via RTSP, HTTP streams
- **Professional Cameras**: Many professional cameras with Linux drivers are supported
- **Virtual Devices**: v4l2loopback can be used for virtual camera creation

To list available camera devices:

```bash
v4l2-ctl --list-devices
```

### Audio Devices

Audio capture and playback is supported through:

- ALSA (Advanced Linux Sound Architecture)
- PulseAudio

To list available audio devices:

```bash
arecord -L  # For recording devices
aplay -L    # For playback devices
```

## Troubleshooting

### Permission Issues

Camera or audio device access issues can often be resolved by adding your user to the appropriate groups:

```bash
sudo usermod -a -G video,audio $USER
```

Remember to log out and back in for group changes to take effect.

### Performance Optimization

For optimal performance on Ubuntu:

- Use the latest GStreamer version (1.24.0+)
- Enable hardware acceleration where available
- For NVIDIA GPUs, install the appropriate CUDA and nvcodec packages
- Adjust process priority using `nice` for resource-intensive applications

## Conclusion

Deploying VisioForge SDK applications on Ubuntu provides a powerful, flexible environment for multimedia applications. By following this guide, you can ensure that your .NET application leverages the full capabilities of the VisioForge SDK ecosystem on Linux platforms.

For specific deployment scenarios or troubleshooting assistance, refer to the comprehensive documentation available on the VisioForge website or contact our technical support team.

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.