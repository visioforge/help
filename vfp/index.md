---
title: Video Fingerprinting Technology for Developers
description: Learn how to implement powerful video identification algorithms with our SDK. Detect duplicates, identify fragments, and match transformed videos across Windows, Linux, and macOS platforms. Perfect for media analysis and content verification.
sidebar_label: Video Fingerprinting SDK
order: 17
icon: ../static/fingerprint.svg
route: /docs/vfp/
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

- C# and .NET
- C++
- VB.NET
- Delphi
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

## .NET API Documentation

- [How to find one video fragment in another](how-to-search-one-video-fragment-in-another.md)
- [How do I compare two video files?](how-to-compare-two-video-files.md)

## SDK Registration Instructions

To register the SDK in your application, call one of the following methods depending on your implementation:

```csharp
VFPAnalyzer.SetLicenseKey("your-license-key");
VFPCompare.SetLicenseKey("your-license-key"); 
VFPImageCompare.SetLicenseKey("your-license-key");
```

## Additional Resources

- [Complete .NET API Reference](https://api.visioforge.com/vfpnet/)
- [SDK Changelog](changelog.md)
- [End User License Agreement](../eula.md)
- [Product Information](https://www.visioforge.com/video-fingerprinting-sdk)
