---
title: VisioForge Computer Vision NuGet Packages for .NET
description: Deploy VisioForge CV and CVD NuGet packages for .NET computer vision on Windows, Linux, and macOS. Face detection, object recognition, and more.
tags:
  - Video Capture SDK
  - Media Player SDK
  - Video Edit SDK
  - .NET
  - Windows

---

# Computer Vision Implementation Guide

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" }, [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" }, [Media Player SDK .Net](https://www.visioforge.com/media-player-sdk-net){ .md-button .md-button--primary target="_blank" }

## Overview of Available Packages

Our SDK provides two NuGet packages that deliver computer-vision capabilities for your applications:

1. **[VisioForge.DotNet.Core.CV](https://www.nuget.org/packages/VisioForge.DotNet.Core.CV/)** — Windows-only CV package.
2. **[VisioForge.DotNet.Core.CVD](https://www.nuget.org/packages/VisioForge.DotNet.Core.CVD/)** — Cross-platform CV package for Windows, Linux, and macOS.

Both packages provide a consistent API for integrating computer-vision features directly into your .NET applications.

## Deployment Requirements

### Windows-Specific CV Package

#### Installation Process

The Windows-specific CV package is designed for seamless integration:

- Simply install the NuGet package through your preferred package manager
- No additional deployment steps are necessary
- Ready to use immediately after installation

### Cross-Platform CVD Package

Our cross-platform CVD package requires specific configurations based on your operating system:

#### Windows Environment Setup

When deploying on Windows systems:

- Install the NuGet package through Visual Studio or the .NET CLI
- No additional dependencies or configurations are required
- Works out of the box with standard Windows installations

#### Ubuntu Linux Configuration

For Ubuntu Linux systems, install the following dependencies:

```bash
sudo apt-get install libgdiplus libopenblas-dev libx11-6
```

These packages provide essential functionalities:

- `libgdiplus`: Provides System.Drawing compatibility
- `libopenblas-dev`: Optimizes matrix operations for computer vision algorithms
- `libx11-6`: Handles X Window System protocol client library

#### macOS Setup Instructions

For macOS environments, use Homebrew to install the required dependencies:

```bash
brew install --cask xquartz
brew install mono-libgdiplus
```

These components enable:

- XQuartz: Provides X11 functionality on macOS
- mono-libgdiplus: Ensures compatibility with System.Drawing

## Additional Resources

For implementation examples and technical guidance:

- Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for extensive code samples
- Explore practical implementations across various use cases
- Access community-contributed examples and solutions

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.