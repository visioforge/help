---
title: Video Fingerprinting SDK Release Notes
description: Track the evolution of our Video Fingerprinting SDK through detailed version history. Discover new features, performance updates, and technical improvements across multiple releases, from version 3.0 to the latest 12.1 update.
sidebar_label: Changelog

---

# Video Fingerprinting SDK Version History

## Version 12.1 - Performance and Feature Enhancements

### .NET Framework Improvements

* **New Fingerprinting Capability**: Introduced `VFPFingerprintFromFrames` class enabling developers to generate video fingerprints directly from sequences of RGB24 frames
* **API Modernization**: Completely revamped async/await API implementation for better asynchronous processing
* **Engine Optimization**: Significantly improved performance of the core fingerprinting engine with enhanced processing algorithms

## Version 12.0 - Database Integration and Hardware Acceleration

### .NET Framework Updates

* **Multi-fingerprint Storage**: Added new `VFPFingerPrintDB` class for efficiently storing multiple fingerprints in a single binary file format
* **Media Monitoring Tool Integration**: Updated the Media Monitoring Tool application to leverage the new database capabilities
* **Updated Dependencies**: Integrated the latest FFMPEG version for improved video handling capabilities
* **Framework Requirement Change**: Increased minimum .NET Framework requirement to version 4.7.2
* **External Logging**: Added NLog as an external dependency for enhanced logging capabilities
* **GPU Acceleration**: Enhanced support for hardware acceleration via Nvidia, Intel and AMD GPU video decoders

## Version 11.0 - Engine Modernization

### .NET Implementation

* **Standalone Installation**: Released as an independent installer package without requiring other .NET SDK installations
* **Video Source Engine**: Implemented new engine for processing video from files and network sources
* **Capture Device Support**: Developed new engine for handling video from capture devices
* **Core Improvements**: Updated fingerprinting engine with latest algorithms

### C++ Linux Support

* **Bug Resolution**: Fixed multiple issues affecting Linux implementations
* **Engine Updates**: Improved fingerprinting engine with platform-specific optimizations

## Version 10.0 - Customization Features

### .NET Enhancements

* **Resolution Control**: Added custom resolution options for source video
* **Cropping Functionality**: Implemented custom crop capabilities for source material
* **Engine Updates**: Upgraded both decoding and fingerprinting engines

### C++ Linux Improvements

* **Demo Application**: Updated Media Monitoring Tool demo with latest FFMPEG compatibility
* **Stability Improvements**: Resolved various bugs affecting performance

## Version 3.1 - Optimization Release

### General Improvements

* **Bug Fixes**: Addressed minor issues affecting overall stability
* **Engine Updates**: Enhanced processing engine for .NET implementation
* **Licensing Change**: Media Monitoring Tool (Live) and Duplicates Video Search tools are now available for free commercial usage

## Version 3.0 - Initial Public Release

### Key Features

* First public release of the Video Fingerprinting SDK
* Introduced core fingerprinting capabilities for video content identification
* Established foundation for cross-platform development
