---
title: .NET SDK Platform Requirements & Compatibility Guide
description: .NET SDK platform support and system requirements for Windows, macOS, Linux, iOS, and Android with framework compatibility details.
---

# System Requirements for .NET SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

This guide details the system requirements and platform compatibility for VisioForge's suite of .NET SDKs, designed for high-performance video processing and playback applications.

## Overview

Unlock powerful cross-platform video capabilities with VisioForge .NET SDKs, fully compatible with Windows, Linux, macOS, Android, and iOS. Our SDKs provide robust support for .NET Framework, .NET Core, and modern .NET 5+ (including .NET 8 LTS & .NET 9), enabling seamless integration with WinForms, WPF, WinUI 3, Avalonia, .NET MAUI, and Xamarin. Develop high-performance video applications with familiar C# paradigms across all major operating systems and UI frameworks.

> **Important Note**: While Windows users benefit from our dedicated installer package, developers working on other platforms should utilize the NuGet package distribution method for implementation.

## Development Environment Requirements

The following sections outline the specific requirements for setting up your development environment when working with our SDKs.

### Operating Systems for Development

Development of applications using our SDKs is supported on the following platforms:

#### Windows

* Windows 10 (all editions)
* Windows 11 (all editions)
* Recommended: Latest feature update with current security patches

#### Linux

* Ubuntu 22.04 LTS or newer
* Debian 11 or newer
* Other distributions with equivalent libraries may work but are not officially supported

#### macOS

* macOS 12 (Monterey) or newer
* Apple Silicon (M1/M2/M3) and Intel processors supported

### Hardware Requirements

For optimal development experience, we recommend:

* Processor: 4+ cores, 2.5 GHz or faster
* RAM: 8 GB minimum, 16 GB recommended for complex projects
* Storage: SSD with at least 10 GB free space
* Graphics: DirectX 11 compatible GPU (Windows) or Metal-compatible GPU (macOS)

## Target Deployment Platforms

Our SDKs can be deployed to a variety of platforms, enabling wide-reaching distribution of your applications.

### Desktop Platforms

#### Windows

* Windows 10 (version 1809 or newer)
* Windows 11 (all versions)
* Both x86 and x64 architectures supported
* ARM64 support for Windows on ARM devices

#### Linux

* Ubuntu 22.04 LTS or newer
* Other distributions require equivalent libraries and dependencies
* x64 and ARM64 architectures supported

#### macOS

* macOS 12 (Monterey) or newer
* Both Intel and Apple Silicon architectures supported natively
* Rosetta 2 not required for Apple Silicon devices

### Mobile Platforms

#### Android

* Android 10 (API level 29) or newer
* ARM, ARM64, and x86 architectures supported
* Google Play Store compatible
* Hardware-accelerated rendering recommended

#### iOS

* iOS 12 or newer versions
* Compatible with iPhone, iPad, and iPod Touch
* Supports both ARMv7 and ARM64 architectures
* App Store distribution compatible

## .NET Framework Compatibility

Our SDKs provide extensive compatibility with various .NET implementations:

### .NET Framework

* .NET Framework 4.6.1
* .NET Framework 4.7.x
* .NET Framework 4.8
* .NET Framework 4.8.1

### Modern .NET

* .NET Core 3.1 (LTS)
* .NET 5.0
* .NET 6.0 (LTS)
* .NET 7.0
* .NET 8.0 (LTS)
* .NET 9.0

### Xamarin (Legacy)

* Xamarin.iOS 12.0+
* Xamarin.Android 9.0+
* Xamarin.Mac 5.0+

## UI Framework Integration

The SDKs integrate with a wide array of UI frameworks, enabling flexible application development:

### Windows-Specific Frameworks

* Windows Forms (WinForms)
  * .NET Framework 4.6.1+ and .NET Core 3.1+
  * High-performance rendering options
  * Supports designer integration

* Windows Presentation Foundation (WPF)
  * .NET Framework 4.6.1+ and .NET Core 3.1+
  * Hardware-accelerated rendering
  * XAML-based layout with binding support

* Windows UI Library 3 (WinUI 3)
  * Desktop applications only
  * Modern Fluent Design components
  * Windows App SDK integration

### Cross-Platform Frameworks

* .NET MAUI
  * Unified development for Windows, macOS, iOS, and Android
  * Shared UI code across platforms
  * Native performance with shared codebase

* Avalonia UI
  * Truly cross-platform UI framework
  * XAML-based with familiar paradigms
  * Windows, Linux, macOS compatible

### Mobile-Specific Frameworks

* iOS Native UI
  * UIKit integration
  * SwiftUI compatibility layer
  * Storyboard and XIB support

* macOS / Mac Catalyst
  * AppKit and UIKit integration
  * Mac Catalyst for iPad app adaptation
  * Native macOS UI elements

* Android Native UI
  * Integration with Android UI toolkit
  * Support for Activities and Fragments
  * Material Design components compatibility

## Distribution Methods

### NuGet Packages

Our SDKs are available as NuGet packages, simplifying integration with your development workflow.

### Windows Setup

For Windows developers, we offer a dedicated installer package that includes:

* SDK binaries and dependencies
* Documentation and example projects
* Visual Studio integration components
* Developer tools and utilities

## Performance Considerations

### Memory Requirements

* Base memory footprint: ~50MB
* Video processing: Additional 100-500MB depending on resolution and complexity
* 4K video processing: 1GB+ recommended

### CPU Utilization

* Video capture: 10-30% on a modern quad-core CPU
* Real-time effects: Additional 10-40% depending on complexity
* Hardware acceleration recommended for production environments

### Storage Requirements

* SDK installation: ~250MB
* Runtime cache: ~100MB
* Temporary processing files: Up to several GB depending on workload

## Licensing and Deployment

Check out our [Licensing](../../licensing.md) page for more information on the different licensing options available for our SDKs.

## Technical Support Resources

We provide extensive resources to ensure successful implementation:

* API documentation with code examples
* Implementation guides for various platforms
* Troubleshooting and optimization tips
* Direct support channels for licensed developers

## Code Samples and Examples

Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for an extensive collection of code samples demonstrating SDK features and implementation patterns across supported platforms.

## Updates and Maintenance

* Regular SDK updates with new features and optimizations
* Security patches and bug fixes
* Backward compatibility considerations
* Migration guides for version transitions

---
This technical specification document outlines the system requirements and compatibility matrix for our Video Capture SDK .Net and related products. For specific implementation details or custom integration scenarios, please contact our developer support team.