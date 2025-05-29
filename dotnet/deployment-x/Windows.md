---
title: Cross-platform SDK .Net deployment for Windows
description: Comprehensive guide for installing and deploying VisioForge SDK for .Net applications on Windows. Learn how to set up development environments, manage dependencies, and troubleshoot common issues for multimedia applications.
sidebar_label: Windows
---

# Windows Installation and Deployment Guide for VisioForge Cross-Platform SDK

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## Introduction to VisioForge SDK Installation and Deployment

The VisioForge SDK suite provides powerful multimedia capabilities for your .NET applications, supporting video capture, editing, playback, and advanced media processing across multiple platforms. This comprehensive guide covers both installation and deployment for Windows applications.

## Installation

SDKs are accessible in two forms: a setup file and NuGet packages. The setup file provides a straightforward installation process, ensuring that all necessary components are correctly configured. On the other hand, NuGet packages offer a flexible and modular approach to incorporating SDKs into your projects, allowing for easy updates and dependency management. We highly recommend utilizing NuGet packages due to their convenience and efficiency in managing project dependencies and updates.

When building your application, you have the option to create both x86 and x64 versions. This allows your application to run on a wider range of systems, accommodating different hardware architectures. However, it's important to note that the setup file is exclusively available for the x64 architecture. This means that while you can develop and compile x86 builds, the initial setup and installation process will require an x64 system.

### IDEs

For development, you can use powerful integrated development environments (IDEs) like JetBrains Rider or Visual Studio. Both IDEs offer robust tools and features to streamline the development process on Windows. To ensure a smooth setup, please refer to the respective installation guides. The [Rider installation page](../install/rider.md) provides detailed instructions for setting up JetBrains Rider, while the [Visual Studio installation page](../install/visual-studio.md) offers comprehensive guidance on installing and configuring Visual Studio. These resources will help you get started quickly and effectively, leveraging the full capabilities of these development environments.

## Distribution and Package Management

VisioForge SDK components for Windows are distributed as NuGet packages, making integration straightforward with modern .NET development environments. You can add these packages to your project using any of the following tools:

- Visual Studio Package Manager
- JetBrains Rider NuGet Manager
- Visual Studio Code with NuGet extensions
- Direct command-line integration using the .NET CLI

## Required Base Packages

Every Windows application built with VisioForge SDK requires the appropriate base package according to your application's target architecture. These packages contain the essential components for SDK functionality.

### Core Platform Packages

- [VisioForge.CrossPlatform.Core.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x86) - For 32-bit Windows applications
- [VisioForge.CrossPlatform.Core.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Core.Windows.x64) - For 64-bit Windows applications

> **Note**: For applications targeting multiple architectures, you should include both packages and implement appropriate runtime selection logic.

## Optional Component Packages

Depending on your application's requirements, you may need to include additional packages for specialized functionality. These optional components extend the SDK's capabilities in various domains.

### FFMPEG Media Processing (Recommended)

These packages provide comprehensive codec support for a wide range of media formats through the FFMPEG library integration:

- [VisioForge.CrossPlatform.Libav.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86) - 32-bit FFMPEG support
- [VisioForge.CrossPlatform.Libav.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64) - 64-bit FFMPEG support

For applications with size constraints, compressed versions of these packages utilizing UPX compression are available:

- [VisioForge.CrossPlatform.Libav.Windows.x86.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x86.UPX) - Compressed 32-bit FFMPEG support
- [VisioForge.CrossPlatform.Libav.Windows.x64.UPX](https://www.nuget.org/packages/VisioForge.CrossPlatform.Libav.Windows.x64.UPX) - Compressed 64-bit FFMPEG support

### Cloud Integration - Amazon Web Services

For applications requiring cloud storage integration with AWS S3:

- [VisioForge.CrossPlatform.AWS.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x86) - 32-bit AWS support
- [VisioForge.CrossPlatform.AWS.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AWS.Windows.x64) - 64-bit AWS support

When using these packages, the following Media Blocks become available:

- `AWSS3SourceBlock` - For retrieving media from S3 buckets
- `AWSS3SinkBlock` - For storing media in S3 buckets

### Computer Vision with OpenCV

For applications requiring advanced image processing and computer vision capabilities:

- [VisioForge.CrossPlatform.OpenCV.Windows.x86](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x86) - 32-bit OpenCV support
- [VisioForge.CrossPlatform.OpenCV.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.OpenCV.Windows.x64) - 64-bit OpenCV support

The OpenCV integration provides access to Media Blocks in the `VisioForge.Core.MediaBlocks.OpenCV` namespace, including:

- Image transformation: `CVDewarpBlock`, `CVDilateBlock`, `CVErodeBlock`
- Edge and feature detection: `CVEdgeDetectBlock`, `CVLaplaceBlock`, `CVSobelBlock`
- Face processing: `CVFaceBlurBlock`, `CVFaceDetectBlock`
- Motion detection: `CVMotionCellsBlock`
- Object recognition: `CVTemplateMatchBlock`, `CVHandDetectBlock`
- Image enhancement: `CVEqualizeHistogramBlock`, `CVSmoothBlock`
- Tracking and overlay: `CVTrackerBlock`, `CVTextOverlayBlock`

## Specialized Hardware Support Packages

VisioForge SDK provides integration with professional camera systems and specialized hardware. Include the appropriate package when working with specific device types.

### Allied Vision Cameras

For integrating with professional Allied Vision camera hardware:

- [VisioForge.CrossPlatform.AlliedVision.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.AlliedVision.Windows.x64)

### Basler Cameras

For applications working with Basler industrial cameras:

- [VisioForge.CrossPlatform.Basler.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Basler.Windows.x64)

### Teledyne/FLIR Cameras (Spinnaker SDK)

For thermal imaging and specialized FLIR cameras:

- [VisioForge.CrossPlatform.Spinnaker.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.Spinnaker.Windows.x64)

### GenICam Protocol Support (GigE/USB3 Vision)

For cameras utilizing the standardized GenICam protocol:

- [VisioForge.CrossPlatform.GenICam.Windows.x64](https://www.nuget.org/packages/VisioForge.CrossPlatform.GenICam.Windows.x64)

## Deployment Best Practices

When deploying VisioForge-based applications for Windows, consider these recommendations:

1. Choose the appropriate architecture packages (x86 or x64) based on your target platform
2. Include the FFMPEG packages for comprehensive media format support
3. Only include specialized hardware packages when needed to minimize deployment size
4. For security-sensitive applications, consider using the UPX compressed versions to obfuscate native libraries
5. Always test your deployment on a clean system to ensure all dependencies are properly resolved

## Troubleshooting Common Issues

### Deployment Issues

If you encounter issues after deployment:

1. Verify all required NuGet packages are properly included
2. Check that the architecture (x86/x64) matches your application target
3. Ensure native libraries are being extracted to the correct locations
4. Review Windows security and permission settings that might restrict media functionality

### WinForms RESX Files Build Issue

Sometimes you can get the following error:

`Error MSB3821: Couldn't process file Form1.resx due to its being in the Internet or Restricted zone or having the mark of the web on the file. Remove the mark of the web if you want to process these files.`

Error MSB3821 occurs when Visual Studio or MSBuild cannot process a `.resx` resource file because it is marked as untrusted. This happens when the file has the "Mark of the Web" (MOTW), a security feature that flags files downloaded from the internet or received from untrusted sources. The MOTW places the file in the Internet or Restricted security zone, preventing it from being processed during a build.

#### How to Fix It

To resolve this error, you need to remove the MOTW from the affected file:

##### Unblock the File Manually

- Right-click on Form1.resx in File Explorer.
- Select Properties.
- In the General tab, check for an Unblock button or checkbox at the bottom.
- Click Unblock, then click OK.

##### Unblock via PowerShell (for multiple files)

- Open PowerShell.
- Navigate to your project directory.
- Run the command: Get-ChildItem -Path . -Recurse | Unblock-File

##### Unblock the ZIP Before Extraction

- If you downloaded the project as a ZIP file, right-click the ZIP file.
- Select Properties.
- Click Unblock, then extract the files.

By unblocking the file, you remove the MOTW, allowing Visual Studio to process it normally during the build.

For additional assistance, visit the [VisioForge support site](https://support.visioforge.com/) or consult the [API documentation](https://api.visioforge.org/dotnet/api/index.html).

---

Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.
