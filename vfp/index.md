---
title: Video Fingerprinting Technology for Developers
description: Implement video identification algorithms with SDK to detect duplicates, identify fragments, and match transformed videos on multiple platforms.
sidebar_label: Video Fingerprinting SDK
order: 17
---

# Video Fingerprinting SDK

## What is Video Fingerprinting?

Our state-of-the-art video fingerprinting technology creates unique digital signatures of video content by analyzing multiple dimensions of visual data. The system employs sophisticated algorithms that focus on:

- **Scene analysis** - Detecting transitions, cuts, and composition
- **Object recognition** - Identifying and tracking key visual elements
- **Motion detection** - Analyzing movement patterns and trajectories
- **Color distribution** - Mapping visual palettes and tonal variations
- **Temporal patterns** - Examining how visual elements change over time

These elements combine to form a distinctive fingerprint that uniquely identifies each video in your database.

## Key Capabilities and Benefits

The SDK can accurately match videos despite significant transformations, including:

- Changes in resolution (from SD to 4K and beyond)
- Variations in encoding bitrate and quality
- Different compression techniques
- Conversion between file formats (MP4, AVI, MOV, etc.)
- Partial content matching (identifying segments)
- Videos embedded within other content
- Presence of overlays, watermarks, or subtitles

This robustness makes the technology ideal for content verification, copyright protection, and media monitoring applications.

## Platform Support and Integration

The SDK offers cross-platform compatibility with:

- **Windows** - Full support for Windows 10/11 and server environments
- **Linux** - Compatible with major distributions
- **macOS** - Full support for recent versions

Developers can integrate using multiple programming languages:

- [C# and .NET](#net-sdk-documentation) - Managed code with rich features
- [C++](#c-sdk-documentation) - Native performance and control
- VB.NET - Full .NET compatibility
- Delphi - Via COM interop
- Other languages via bindings

Read more about the SDK on the [product page](https://www.visioforge.com/video-fingerprinting-sdk).

## Sample Applications

We provide two powerful sample applications built with our SDK:

### Media Monitoring Tool

A Windows application designed to detect advertisements and specific content segments in recorded or live video streams. Ideal for:

- TV and DVB channel monitoring
- Advertisement tracking
- Broadcast compliance verification
- Content analysis for media companies

### Duplicates Video Finder

A specialized Windows tool for identifying duplicate video content across large collections. The application can detect matches even when videos have:

- Different resolutions and aspect ratios
- Varying bitrates and quality levels
- Different file formats and codecs
- Added watermarks or subtitles
- Minor edits or trimming

## Choose Your SDK

### .NET SDK Documentation

The .NET SDK provides a managed code solution with rich features and rapid development:

- [Getting Started with .NET](dotnet/getting-started.md) - Complete installation and setup
- [.NET API Reference](dotnet/api.md) - Comprehensive managed API documentation
- [Database Integration](dotnet/database-integration.md) - Built-in MongoDB support
- [Sample Applications](dotnet/samples/index.md) - GUI and CLI tools

### C++ SDK Documentation  

The C++ SDK offers native performance and fine-grained control:

- [Getting Started with C++](cpp/getting-started.md) - Platform-specific setup guides
- [C++ API Reference](cpp/api.md) - Native API documentation
- [C++ SDK Overview](cpp/index.md) - Features and capabilities

### Core Concepts (Both SDKs)

- [System Requirements](system-requirements.md) - Platform and hardware requirements for both SDKs
- [Understanding Video Fingerprinting](understanding-video-fingerprinting.md) - How the technology works
- [Fingerprint Types Explained](fingerprint-types.md) - Compare vs Search fingerprints (applies to both .NET and C++)

## SDK Comparison

### Quick Comparison Table

| Feature | .NET SDK | C++ SDK |
|---------|----------|---------|
| **Performance** | Excellent managed performance | Maximum native performance |
| **Development Speed** | Fast development, simple API | More complex, full control |
| **Memory Management** | Automatic (GC) | Manual (RAII) |
| **GUI Support** | WPF, WinForms, MAUI | Qt, MFC, wxWidgets |
| **Database Integration** | Built-in MongoDB | Custom implementation |
| **Sample Applications** | Extensive GUI & CLI | Command-line focused |
| **Learning Curve** | Easier for .NET developers | Steeper, more control |
| **Deployment** | .NET runtime required | Self-contained binaries |

### Choosing the Right SDK

**Choose .NET SDK if you:**

- Need rapid application development
- Want built-in database integration
- Prefer automatic memory management
- Are building GUI applications
- Have existing .NET infrastructure

**Choose C++ SDK if you:**

- Require maximum performance
- Need fine-grained memory control
- Are integrating with native code
- Deploy to embedded systems
- Want minimal dependencies

## Tutorials and Guides

### Step-by-Step Tutorials

- [How to Compare Two Video Files](dotnet/samples/how-to-compare-two-video-files.md) - Video comparison guide (.NET)
- [How to Find One Video Fragment in Another](dotnet/samples/how-to-search-one-video-fragment-in-another.md) - Fragment search guide (.NET)

### Integration Guides

- [.NET Database Integration](dotnet/database-integration.md) - MongoDB with .NET SDK
- [.NET Command-Line Samples](dotnet/samples/index.md) - CLI utilities and examples
- [C++ Command-Line Samples](cpp/samples/index.md) - Native CLI examples
- [C++ Integration Patterns](cpp/index.md#integration-patterns) - Native integration examples

## Use Cases and Applications

- [Real-World Use Cases](use-cases.md) - Industry applications and scenarios

## Sample Applications

### .NET Windows Applications

- [Media Monitoring Tool (MMT)](dotnet/samples/mmt.md) - TV and stream monitoring
- [MMT Live Edition](dotnet/samples/mmt-live.md) - Real-time stream analysis
- [Duplicate Video Scanner (DVS)](dotnet/samples/dvs.md) - Find duplicate videos

### Command-Line Tools

- [.NET CLI Tools](dotnet/samples/index.md) - VFP Generator, Compare, Search
- [C++ Samples](cpp/samples/index.md) - Native command-line utilities

### Code Examples

- [.NET Code Samples](dotnet/samples/index.md) - Comprehensive .NET examples
- [C++ Code Samples](cpp/samples/index.md) - Native C++ examples

## Help and Support

### Essential Resources

- **[FAQ](faq.md)** - Frequently asked questions with detailed answers

### Reference Documentation

- [Complete .NET API Reference](https://api.visioforge.org/vfpnet/)
- [SDK Changelog](changelog.md)

## Additional Resources

- [Complete .NET API Reference](https://api.visioforge.org/vfpnet/)
- [SDK Changelog](changelog.md)
- [End User License Agreement](../eula.md)
- [Product Information](https://www.visioforge.com/video-fingerprinting-sdk)
