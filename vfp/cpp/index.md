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
  - VFPFingerprintSource
  - VFPFingerPrint
  - VFPSearch
  - VFPCompare

---

# Video Fingerprinting SDK for C++

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

#### Generate Search Fingerprint (high-level API)

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

VFPSetLicenseKey(L"your-license-key");

// Fill source info
VFPFingerprintSource src{};
VFPFillSource(L"C:\\video.mp4", &src);
src.StartTime = 10000;   // start at 10s
src.StopTime = 60000;    // stop at 60s

// Generate search fingerprint
VFPFingerPrint fp{};
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(src, &fp);
if (error == nullptr)
{
    printf("Fingerprint: %dx%d, %.1fs, %d bytes\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"output.vfpsig");
}
```

#### Compare Two Fingerprints

```cpp
VFPFingerPrint fp1{}, fp2{};
VFPFingerprintLoad(&fp1, L"video1.vfpsig");
VFPFingerprintLoad(&fp2, L"video2.vfpsig");

double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                 fp2.Data, fp2.DataSize, 10);
printf("Difference: %.2f\n", diff);
```

#### Search Fragment in Longer Video

```cpp
VFPFingerPrint needle{}, haystack{};
VFPFingerprintLoad(&needle, L"fragment.vfpsig");
VFPFingerprintLoad(&haystack, L"full.vfpsig");

double diff = 0;
int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
if (pos != INT_MAX)
    printf("Found at %d seconds (diff: %.2f)\n", pos, diff);
```

#### Low-Level Per-Frame API (for live streams / custom decoders)

```cpp
// Allocate accumulator for ~60s of video at 30fps
void* pData = VFPSearch_Init2(30 * 60);

while (hasMoreFrames)
{
    // Decode frame into RGB buffer...
    VFPSearch_Process(rgbData, width, height, stride, timestampSec, pData);
}

int len = 0;
char* data = VFPSearch_Build(&len, pData);

// Use data/len as VFPFingerPrint.Data / .DataSize
VFPFingerPrint fp{};
fp.Data = data;
fp.DataSize = len;
// ... set Duration, Width, Height, FrameRate manually ...

VFPSearch_Clear(pData);
```

**Important:** The SDK provides only the low-level `_Init` / `_Process` / `_Build` / `_Search` / `_Compare` primitives. The host application is responsible for decoding video frames (FFmpeg, GStreamer, DirectShow, etc.) and feeding them frame-by-frame to `*_Process`.

## Integration Patterns

### Batch Processing

```cpp
void ProcessBatch(const std::vector<std::wstring>& videos)
{
    for (const auto& path : videos)
    {
        VFPFingerprintSource src{};
        VFPFillSource(path.c_str(), &src);

        VFPFingerPrint fp{};
        VFPSearch_GetFingerprintForVideoFile(src, &fp);
        // Store fp in database...
    }
}
```

### Real-Time Stream Analysis (low-level API)

```cpp
void* pData = VFPSearch_Init2(30 * 60); // 30fps, 60s buffer

void OnFrame(unsigned char* rgb, int w, int h, int stride, double timestampSec)
{
    VFPSearch_Process(rgb, w, h, stride, timestampSec, pData);
}

void OnStreamEnd()
{
    int len;
    char* data = VFPSearch_Build(&len, pData);
    // Compare with known fingerprints...
    VFPSearch_Clear(pData);
}
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

```cpp
#include <VisioForge_VFP.h>

VFPSetLicenseKey(L"your-license-key");
// or for narrow-char: VFPSetLicenseKeyA("your-license-key");
```

## Next Steps

1. [Install and Setup](getting-started.md) - Get started with the C++ SDK
2. [Review the API](api.md) - Understand available classes and methods
3. [Explore Examples](getting-started.md#your-first-application) - See working code
