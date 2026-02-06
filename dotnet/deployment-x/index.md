---
title: Cross-Platform .NET SDK Deployment Guide
description: Deploy .NET applications on Windows, macOS, iOS, Android, and Linux with native libraries, platform dependencies, and UI framework integration.
sidebar_label: Deployment
order: 17
---

# Cross-Platform Deployment Guide for VisioForge .NET SDK

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Introduction to VisioForge SDK Deployment

The VisioForge SDK suite provides powerful multimedia capabilities for .NET applications, supporting video capture, editing, playback, and advanced media processing across multiple platforms. Proper deployment is critical to ensure your applications function correctly and leverage the full potential of these SDKs.

This comprehensive guide outlines the deployment process for applications built with VisioForge's cross-platform .NET SDKs, helping you navigate the specific requirements of each supported operating system.

## Deployment Overview

Deploying applications built with VisioForge SDKs requires careful consideration of platform-specific dependencies and configurations. The deployment process varies significantly depending on your target platform due to differences in:

- Native library requirements
- Media framework dependencies
- Hardware access mechanisms
- Package distribution methods

### Key Deployment Considerations

Before beginning the deployment process, consider these critical factors:

1. **Target Platform Architecture**: Ensure you select the appropriate architecture (x86, x64, ARM64) for your deployment platform
2. **Required Dependencies**: Some platforms require additional libraries that aren't included in NuGet packages
3. **Framework Compatibility**: Verify compatibility between your .NET version and the target operating system
4. **Native Library Integration**: Understand how native libraries are integrated and loaded on each platform
5. **UI Framework Selection**: Choose the appropriate UI integration package for your selected framework

## Platform-Specific Deployment

### Windows Deployment

Windows deployment is the most straightforward, with comprehensive NuGet package support covering all dependencies:

- **Package Distribution**: All components available via NuGet
- **Architecture Support**: Both x86 and x64 architectures fully supported
- **Native Libraries**: Automatically deployed alongside your application
- **UI Framework Options**: Windows Forms, WPF, WinUI, Avalonia, Uno, and MAUI supported

For detailed Windows deployment instructions, see the [Windows deployment guide](Windows.md).

### Android Deployment

Android deployment requires specific configuration for native library extraction and permissions:

- **Package Distribution**: Core components available via NuGet
- **Architecture Support**: ARM64, ARMv7, and x86_64 architectures supported
- **Native Libraries**: Requires proper configuration for extraction to the correct location
- **Permissions**: Camera, microphone, and storage permissions must be explicitly requested
- **UI Integration**: Android-specific video view controls required

Android applications use a single native library that must be correctly deployed. Review the [Android deployment guide](Android.md) for complete instructions.

### macOS Deployment

macOS deployment requires additional GStreamer library installation:

- **Package Distribution**: Core components available via NuGet, GStreamer requires manual installation
- **Architecture Support**: Intel (x64) and Apple Silicon (ARM64) architectures supported
- **Native Libraries**: Multiple unmanaged libraries required
- **Framework Options**: Native macOS, MAUI, and Avalonia supported
- **Bundle Integration**: Special attention needed for proper app bundle structure

macOS deployments may require specific entitlements and permissions configurations. See the [macOS deployment guide](macOS.md) for detailed instructions.

### iOS Deployment

iOS deployment involves unique challenges related to Apple's platform restrictions:

- **Package Distribution**: Core components available via NuGet
- **Architecture Support**: ARM64 architecture supported
- **App Store Guidelines**: Special considerations for App Store submissions
- **Native Libraries**: Single unmanaged binary library to deploy
- **UI Integration**: iOS-specific video view controls required

iOS applications require proper provisioning profiles and entitlements. The [iOS deployment guide](iOS.md) provides comprehensive instructions.

### Ubuntu/Linux Deployment

Linux deployment requires manual installation of GStreamer dependencies:

- **Package Distribution**: Core components available via NuGet, GStreamer requires system packages
- **Architecture Support**: x64 architecture primarily supported
- **System Dependencies**: Required packages must be installed on the target system
- **Distribution Considerations**: Different Linux distributions may require different dependency packages
- **UI Options**: Primarily Avalonia UI framework supported

Linux deployment often involves distribution-specific package management. The [Ubuntu deployment guide](Ubuntu.md) provides instructions for Ubuntu-based distributions.

### Uno Platform Deployment

Uno Platform enables deploying applications from a single codebase to multiple platforms:

- **Package Distribution**: Core components available via NuGet with platform-specific redistributables
- **Supported Platforms**: Windows, Android, iOS, macOS (Catalyst), WebAssembly, and Linux Desktop
- **Architecture Support**: Varies by target platform
- **Native Libraries**: Platform-specific redistributables required for each target
- **UI Integration**: Uno-specific VideoView control adapts to each platform

Uno Platform simplifies multi-platform development while leveraging native capabilities. See the [Uno Platform deployment guide](uno.md) for comprehensive instructions.

### Runtime Requirements

Target devices must meet these minimum requirements:

- **Windows**: Windows 7 or later (x86 or x64)
- **macOS**: macOS 10.15 (Catalina) or later (x64 or ARM64)
- **iOS**: iOS 14.0 or later (ARM64)
- **Android**: Android 7.0 (API level 24) or later
- **Linux**: Ubuntu 20.04 LTS or later (x64 or ARM64)

## Common Deployment Challenges

### Native Library Loading Issues

One of the most common deployment problems involves native library loading failures:

- **Symptoms**: Runtime exceptions mentioning DllNotFoundException or similar
- **Causes**: Incorrect architecture, missing dependencies, or improper extraction
- **Solutions**: Verify package references, check deployment configuration, ensure libraries are in the correct location

### Permission and Security Constraints

Modern operating systems enforce strict security policies:

- **Camera Access**: Requires explicit permission on all mobile platforms
- **Storage Access**: File system restrictions vary by platform
- **Network Usage**: May require specific entitlements or manifest entries
- **Background Operation**: Platform-specific rules for background media processing

### Performance Considerations

Media processing can be resource-intensive:

- **CPU Usage**: Implement appropriate threading to avoid UI freezing
- **Memory Management**: Monitor and optimize memory usage for large media files
- **Power Consumption**: Balance quality settings with battery life considerations

## Deployment Checklist

Use this checklist to ensure a successful deployment:

- ✅ Correct NuGet packages selected for target platform and architecture
- ✅ Platform-specific dependencies installed and configured
- ✅ SDK properly initialized and cleaned up
- ✅ Appropriate video view controls integrated
- ✅ Required permissions requested and justified
- ✅ Application tested on target platform under realistic conditions
- ✅ Performance metrics validated for acceptable user experience
- ✅ Error handling implemented for graceful recovery

## Computer Vision Deployment

Computer Vision SDK is a separate NuGet package. Check the [Computer Vision deployment guide](computer-vision.md) for more information.

## Additional Resources

- [VisioForge GitHub Repository](https://github.com/visioforge/.Net-SDK-s-samples) - Code samples and example projects
- [API Documentation](https://api.visioforge.org/dotnet/) - Comprehensive API reference
- [Support Portal](https://support.visioforge.com/) - Technical support and knowledge base
