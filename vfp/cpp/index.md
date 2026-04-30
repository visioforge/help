---
title: C++ Video Fingerprinting Library and API Documentation
description: Native C++ implementation of Video Fingerprinting SDK with high performance and cross-platform support for robust video fingerprints.
sidebar_label: C++ SDK Documentation
order: 50
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - Fingerprinting
primary_api_classes:
  - VFPAnalyzer
  - VFPAnalyzerSettings
  - VideoProcessor
  - StoreFingerprint
  - StreamAnalyzer

---

# Video Fingerprinting SDK for C++

!!! danger "Code samples on this page do not match the public C API — treat as illustrative pseudocode"

    The C++ class API documented below (`VFPAnalyzer` class with
    `Initialize` / `ProcessVideo`, `VFPAnalyzerSettings`, `VFPAnalyzerMode`
    enum, `VFPCompare` class with `CompareVideos`, etc.) does **not** exist
    in the shipped SDK. The real Video Fingerprinting SDK exposes a
    **C-style flat API** (`extern "C"`), not a class hierarchy.

    **Real canonical C API (per `VisioForge_VFP.h`):**

    Search-fingerprint workflow (per-frame):

    ```c
    VFPSearchData searchData;
    VFPSearch_Init(&searchData);
    // For each decoded frame:
    VFPSearch_Process(&searchData, frameData, width, height, stride, timestamp);
    // After all frames:
    VFPSearch_Build(&searchData, &resultFingerprint);
    // Search one fingerprint inside another:
    VFPSearch_Search(&fpA, &fpB, duration, maxDifference, allowMultipleFragments,
                     &outResults, &outCount);
    ```

    Compare-fingerprint workflow (per-frame):

    ```c
    VFPCompareData compareData;
    VFPCompare_Init(&compareData);
    // For each decoded frame:
    VFPCompare_Process(&compareData, frameData, width, height, stride, timestamp);
    // After all frames:
    VFPCompare_Build(&compareData, &resultFingerprint);
    // Compare two fingerprints (returns difference; 0 = identical):
    int diff = VFPCompare_Compare(&fpA, &fpB, durationMs);
    ```

    Convenience higher-level helpers like
    `VFPSearch_GetFingerprintForVideoFile`,
    `VFPCompare_GetFingerprintForVideoFile`, and `VFPFillSource` shown
    elsewhere on the C++ pages are **not** real exports — only the
    low-level `_Init` / `_Process` / `_Build` / `_Search` / `_Compare`
    primitives ship in `VisioForge_VFP.dll`. The host application is
    responsible for decoding video frames (e.g., via FFmpeg or GStreamer)
    and feeding them frame-by-frame to `*_Process`.

    Tracked as defects #084-#090 in the help audit. A full rewrite of
    the C++ doc set is queued. For working low-level samples, see
    `samples/` in the SDK distribution.

## Overview

The Video Fingerprinting SDK for C++ provides a native implementation with direct access to high-performance video analysis and fingerprinting capabilities. This SDK is ideal for applications requiring:

- Maximum performance and minimal overhead
- Direct integration with native applications
- Custom memory management
- Real-time processing pipelines
- Embedded systems deployment

## Key Features

### Performance Advantages

- **Native Performance** - Direct memory access and optimized algorithms
- **Zero Overhead** - No managed runtime or garbage collection
- **SIMD Optimization** - Leverages CPU vectorization capabilities
- **Parallel Processing** - Multi-threaded fingerprint generation
- **Custom Memory Management** - Fine-grained control over memory allocation

### Platform Support

- **Windows** - Visual Studio 2019+ (x64)
- **Linux** - GCC 9+ or Clang 10+
- **macOS** - Xcode 12+ (Intel and Apple Silicon)

## Documentation

### Getting Started

- [Installation and Setup](getting-started.md) - Complete setup guide for all platforms
- [API Reference](api.md) - Comprehensive C++ API documentation

### Core Concepts

- [Understanding Video Fingerprinting](../understanding-video-fingerprinting.md) - How the technology works
- [Fingerprint Types](../fingerprint-types.md) - Compare vs Search fingerprints

### Code Examples

#### Basic Fingerprint Generation

```cpp
#include <VFPAnalyzer.h>

// Create analyzer instance
auto analyzer = std::make_unique<VFPAnalyzer>();

// Configure analysis parameters
VFPAnalyzerSettings settings;
settings.Mode = VFPAnalyzerMode::Search;
settings.FrameStep = 10;

// Set license key
analyzer->SetLicenseKey("your-license-key");

// Process video file
analyzer->StartAsync("input_video.mp4", "output.vfp", settings);
```

#### Comparing Two Videos

```cpp
#include <VFPCompare.h>

// Create comparison instance
auto compare = std::make_unique<VFPCompare>();

// Set license
compare->SetLicenseKey("your-license-key");

// Load fingerprints
compare->LoadFingerprint("video1.vfp");
compare->LoadFingerprint("video2.vfp");

// Perform comparison
auto result = compare->Compare();

// Check similarity
std::cout << "Similarity: " << result.Similarity << "%" << std::endl;
if (result.IsMatch) {
    std::cout << "Videos match!" << std::endl;
}
```

## Integration Patterns

### Memory-Efficient Processing

```cpp
// Process large video collections with minimal memory
class VideoProcessor {
public:
    void ProcessBatch(const std::vector<std::string>& videos) {
        VFPAnalyzer analyzer;
        analyzer.SetLicenseKey(m_licenseKey);
        
        for (const auto& video : videos) {
            // Process and immediately store/transmit fingerprint
            analyzer.StartAsync(video, 
                [this](const std::string& fingerprint) {
                    // Store in database or send to server
                    StoreFingerprint(fingerprint);
                });
        }
    }
};
```

### Real-Time Stream Analysis

```cpp
// Analyze live video streams
class StreamAnalyzer {
public:
    void AnalyzeStream(const std::string& streamUrl) {
        VFPAnalyzer analyzer;
        VFPAnalyzerSettings settings;
        settings.Mode = VFPAnalyzerMode::RealTime;
        settings.BufferSize = 30; // 30 second buffer
        
        analyzer.SetLicenseKey(m_licenseKey);
        analyzer.StartStreamAnalysis(streamUrl, settings,
            [](const VFPSegment& segment) {
                // Process detected segments in real-time
                ProcessSegment(segment);
            });
    }
};
```

## Support and Resources

### Documentation

- [C++ API Reference](api.md)
- [Getting Started Guide](getting-started.md)
- [C++ Code Samples](samples/index.md)
- [Common Use Cases](../use-cases.md)

### Sample Code

- [Complete Examples](samples/index.md) - Working code samples
- Command-line tools in SDK package `/samples/cpp/`

### Community and Support

- [GitHub Issues](https://github.com/visioforge/.Net-SDK-s-samples/issues)
- [Support Portal](https://support.visioforge.com)

## License Registration

Register the SDK in your C++ application:

```cpp
// In your initialization code
VFPAnalyzer analyzer;
analyzer.SetLicenseKey("your-license-key");

// Or globally for all instances
VFPLicense::SetGlobalKey("your-license-key");
```

## Next Steps

1. [Install and Setup](getting-started.md) - Get started with the C++ SDK
2. [Review the API](api.md) - Understand available classes and methods
3. [Explore Examples](getting-started.md#your-first-application) - See working code
