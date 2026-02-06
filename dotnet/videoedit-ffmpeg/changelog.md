---
title: Video Edit SDK FFMPEG .Net Release History
description: Detailed version history and feature updates for Video Edit SDK FFMPEG .Net with performance improvements and API changes.
---

# Video Edit SDK FFMPEG .Net: Complete Version History

## What's New in Version 12.1

Our latest release brings significant improvements to deployment flexibility and framework compatibility:

### .Net Framework Upgrade

* Full migration to .Net 4.6 framework ensuring better performance and compatibility with modern systems
* Enhanced runtime reliability with updated core components

### Streamlined Distribution Model

* Unified installer package for both TRIAL and FULL versions, simplifying the deployment process
* Identical NuGet packages across licensing tiers, eliminating confusion between versions

### Cross-Platform Development

* Consolidated .Net Core and .Net Framework packages into a single unified distribution
* Simplified dependency management across different target platforms

### Deployment Improvements

* Added NuGet redists packages for easier dependency management
* Streamlined deployment process with automatic reference handling
* Reduced setup complexity for enterprise applications

## Version 11.3 Release Highlights

This version focused on core audio capabilities and cross-platform support:

### Audio Enhancement

* Completely redesigned audio fade-in/fade-out effects for smoother transitions
* Improved algorithm performance on multi-core processors
* Enhanced audio processing pipeline stability

### Framework Updates

* Added comprehensive .Net Core support for cross-platform development
* Backward compatibility maintained with existing .Net Framework implementations
* Performance optimizations for both runtime environments

### Technical Improvements

* Updated integrated JSON serializer with better handling of complex objects
* Improved memory management for large media processing tasks
* Fixed threading issues in multi-processor environments

## Version 10.0 Major Update

A significant update with many new features and architectural improvements:

### Advanced Media Handling

* Completely redesigned media information reader with better format support
* `MediaInfoNV` component renamed to more intuitive `MediaInfoReader`
* Enhanced metadata extraction capabilities for a wider range of formats

### Media Tagging System

* Added comprehensive standard tags support for various formats:
  * Video files: MP4, WMV, and other container formats
  * Audio files: MP3, AAC, M4A, Ogg Vorbis, and additional audio formats
* Tag reading support in Media Player SDK
* Tag writing capabilities in both Video Capture SDK and Video Edit SDK

### Synchronization Enhancements

* Implemented delayed start functionality across all SDK components
* New `Start_DelayEnabled` property allowing near-simultaneous initialization of multiple SDK controls
* Improved synchronization between audio and video processing pipelines

### Audio Processing Architecture

* Audio effects rewritten in C# for x64 application compatibility
* Legacy effects API maintained for backward compatibility
* Improved performance and reduced latency in real-time processing

### Developer Experience

* Added error tracking in Visual Studio Output window
* Real-time error monitoring from OnError events
* JSON-based settings serialization for easier configuration management

### Output Format Additions

* GIF output support in both Video Edit SDK .Net and Video Capture SDK .Net
* Custom MP3 splitter addressing playback issues with problematic MP3 files

### Structural Changes

* `VisioForge.Controls.WinForms` and `VisioForge.Controls.WPF` assemblies consolidated into unified `VisioForge.Controls.UI` assembly
* Added `ExecutableFilename` property to `VFFFMPEGEXEOutput` for custom FFMPEG executable specification
* Significant optimization of video effects for latest Intel CPU architectures
* Improved multithreading support for better multicore utilization

## Version 9.0 Features

This release introduced several new capabilities to enhance media presentation:

### Visual Enhancements

* Added animated GIF support as image logo overlays
* Improved rendering pipeline for smoother animations
* Better alpha channel handling for transparent overlays

### SDK Information Access

* New `SDK_Version` property to programmatically access assembly versions
* Added `SDK_State` property to check registration and licensing information
* Enhanced diagnostic capabilities for troubleshooting

### Licensing Improvements

* Implemented dedicated licensing event system to verify required SDK edition
* Clearer error messages for licensing issues
* Improved license validation process

## Version 8.6 Update

A maintenance release focusing on stability:

### Stability Improvements

* Fixed memory leaks in long-running processing operations
* Addressed threading issues with concurrent media operations
* Improved exception handling in core components

## Version 8.5 Release

This update provided core engine improvements:

### FFMPEG Updates

* Updated FFMPEG core components to latest stable version
* Enhanced codec support for newer media formats
* Performance improvements in transcoding operations

### Bug Fixes

* Resolved issues with audio/video synchronization in specific formats
* Fixed container format compatibility problems
* Improved stability during format conversion operations

## Version 7.0 Initial Release

The foundation release that established the core functionality:

### Key Features

* High-performance video editing capabilities
* Comprehensive format support for professional workflows
* Flexible API design for integration into various applications
* Cross-platform compatibility considerations
* Foundation for future development and enhancement

## Compatibility and Requirements

When upgrading between versions, please consider the following:

* Version 12.1 requires .Net Framework 4.6 or higher
* Version 11.3 and above support both .Net Core and .Net Framework
* Version 10.0 introduced breaking changes in assembly structure
* NuGet packages provide the simplest upgrade path between versions

Our ongoing development aims to enhance functionality while maintaining compatibility where possible. API changes are documented in detail to assist with migration planning.

## Getting Started

For developers new to the SDK, we recommend starting with the latest version to benefit from all improvements and optimizations. The unified installer and NuGet packages make integration straightforward into both new and existing projects.
