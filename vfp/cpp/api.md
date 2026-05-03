---
title: Video Fingerprinting C++ API for Search and Comparison
description: Complete API documentation for VisioForge Video Fingerprinting SDK C++ to generate, compare, and search video fingerprints with examples.
tags:
  - Video Fingerprinting SDK
  - C++
  - Windows
  - macOS
  - Linux
  - GStreamer
  - Fingerprinting
  - MP4
primary_api_classes:
  - VFPFingerprintSource
  - VFPFingerPrint

---

# Video Fingerprinting SDK C++ API Documentation

## Overview

The VisioForge Video Fingerprinting C++ SDK provides a high-performance native library for video content identification, comparison, and search operations. It enables applications to:

- Generate unique fingerprints from video files for content identification
- Compare videos to determine similarity and detect duplicates
- Search for video fragments within larger videos (e.g., finding commercials, intros, or specific scenes)
- Compare individual images for similarity detection
- Process video frames directly to generate fingerprints from streams or generated content

The C++ SDK offers optimal performance for high-throughput applications and can be integrated into existing C++ applications or used through P/Invoke from other languages.

> **Related Documentation:**
>
> - [.NET API Reference](../dotnet/api.md) - For managed code developers
> - [Understanding Video Fingerprinting](../understanding-video-fingerprinting.md) - Core concepts
> - [Fingerprint Types](../fingerprint-types.md) - Compare vs Search modes

## Table of Contents

- [Header Files](#header-files)
- [License Management](#license-management)
- [Core Types and Structures](#core-types-and-structures)
- [Search Functions](#search-functions)
- [Compare Functions](#compare-functions)
- [Utility Functions](#utility-functions)
- [Image Comparison](#image-comparison)
- [Complete Working Examples](#complete-working-examples)
- [Platform Support](#platform-support)
- [Building and Linking](#building-and-linking)
- [Performance Considerations](#performance-considerations)
- [Error Handling](#error-handling)

## Header Files

The SDK provides two main header files:

### VisioForge_VFP.h

Main API header containing all function declarations and exports.

### VisioForge_VFP_Types.h

Type definitions and data structures used by the SDK.

```cpp
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
```

## License Management

### VFPSetLicenseKey

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKey(wchar_t* licenseKey);
```

**Description:** Sets the license key for Video Fingerprinting SDK. Must be called before using any fingerprinting features.

**Parameters:**

- `licenseKey` (wchar_t*): Your VisioForge license key as a wide character string

**Example:**

```cpp
// Set license key at application startup
VFPSetLicenseKey(L"YOUR-LICENSE-KEY-HERE");

// For trial mode
VFPSetLicenseKey(L"TRIAL");
```

### VFPSetLicenseKeyA

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSetLicenseKeyA(char* licenseKey);
```

**Description:** Sets the license key using ANSI string (alternative to VFPSetLicenseKey).

**Parameters:**

- `licenseKey` (char*): Your VisioForge license key as an ANSI string

**Example:**

```cpp
// Set license key using ANSI string
VFPSetLicenseKeyA("YOUR-LICENSE-KEY-HERE");
```

## Core Types and Structures

### VFPFingerprintSource

```cpp
struct VFPFingerprintSource
{
    wchar_t Filename[256];     // Video file path
    INT64 StartTime;            // Start time in milliseconds
    INT64 StopTime;             // Stop time in milliseconds
    RECT CustomCropSize;        // Custom crop area
    SIZE CustomResolution;      // Custom resolution for processing
    RECT IgnoredAreas[10];      // Areas to ignore (e.g., logos, tickers)
    INT64 OriginalDuration;     // Original file duration
};
```

**Description:** Configuration structure for fingerprint generation from video files.

**Fields:**

- `Filename`: Path to the video file (maximum 256 characters)
- `StartTime`: Starting position in milliseconds (0 for beginning)
- `StopTime`: Ending position in milliseconds (0 for end of file)
- `CustomCropSize`: Optional cropping rectangle (left, top, right, bottom)
- `CustomResolution`: Optional custom resolution for processing
- `IgnoredAreas`: Up to 10 rectangular areas to ignore during fingerprint generation
- `OriginalDuration`: Duration of the original file in milliseconds

**Example:**

```cpp
VFPFingerprintSource source{};
wcscpy_s(source.Filename, L"C:\\Videos\\sample.mp4");
source.StartTime = 10000;  // Start at 10 seconds
source.StopTime = 60000;   // Stop at 60 seconds

// Ignore logo in top-right corner
source.IgnoredAreas[0] = {1800, 0, 1920, 100};
```

### VFPFingerPrint

```cpp
struct VFPFingerPrint
{
    char* Data;                    // Fingerprint data
    INT32 DataSize;                // Size of fingerprint data
    INT64 Duration;                // Duration in milliseconds
    GUID ID;                       // Unique identifier
    wchar_t OriginalFilename[256]; // Original file name
    INT64 OriginalDuration;        // Original file duration
    wchar_t Tag[100];              // Optional tag
    INT32 Width;                   // Source video width
    INT32 Height;                  // Source video height
    double FrameRate;              // Frame rate
};
```

**Description:** Structure containing generated fingerprint data and metadata.

**Fields:**

- `Data`: Binary fingerprint data
- `DataSize`: Size of the fingerprint data in bytes
- `Duration`: Duration of the fingerprinted content in milliseconds
- `ID`: Unique GUID identifier for the fingerprint
- `OriginalFilename`: Path to the original video file
- `OriginalDuration`: Duration of the original file
- `Tag`: Optional user-defined tag (up to 100 characters)
- `Width`: Width of the source video
- `Height`: Height of the source video
- `FrameRate`: Frame rate of the source video

## Search Functions

The Search API provides both a high-level convenience API (generate fingerprint from video file) and a low-level per-frame API (for live streams / custom decoders).

### High-Level API

#### VFPSearch_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Generates a search fingerprint directly from a video file. Returns `NULL` on success, or an error message string.

#### VFPSearch_GetFingerprintForVideoFileAndSave

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFileAndSave(
    wchar_t* sourceFilename,
    wchar_t* destFilename);
```

Generates and saves a fingerprint in one call.

#### VFPSearch_SearchOneSignatureFileInAnother

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_SearchOneSignatureFileInAnother(
    wchar_t* file1, wchar_t* file2,
    LONG threshold, LONG* position);
```

Searches for one signature file inside another directly from disk.

#### VFPSearch_Search2

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, int iSkip1,
    VFPFingerPrint* vfp2, int iSkip2,
    double* pDiff, int maxDiff);
```

Searches `vfp1` inside `vfp2`. Returns position in seconds, or `INT_MAX` if not found.

### Low-Level Per-Frame API

#### VFPSearch_Init

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Init(int count, void* pDataTmp);
```

Initializes a per-frame accumulator. `count` is the expected number of frames.

#### VFPSearch_Init2

```cpp
extern "C" VFP_EXPORT void* VFP_EXPORT_CALL VFPSearch_Init2(int count);
```

Allocates and returns a new accumulator. Use `VFPSearch_Clear` to free it.

#### VFPSearch_Process

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Process(
    unsigned char* p, int w, int h, int s,
    double dTime, void* pDataTmp);
```

Feeds one decoded RGB frame. `dTime` is the timestamp in seconds. Returns 0 on success.

#### VFPSearch_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPSearch_Build(int* pLen, void* pDataTmp);
```

Finalizes the fingerprint. Returns a `char*` buffer; `*pLen` receives its size in bytes.

#### VFPSearch_Search

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search(
    const char* pData1, int iLen1, int iSkip1,
    const char* pData2, int iLen2, int iSkip2,
    double* pDiff, int maxDiff);
```

Low-level search using raw fingerprint data. Returns position in seconds.

#### VFPSearch_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_Clear(void* pDataTmp);
```

Frees resources allocated by `VFPSearch_Init` or `VFPSearch_Init2`.

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPSearch_Build(
    int* pLen,
    void* pDataTmp);
```

Finalizes the fingerprint. Returns a `char*` buffer with the fingerprint data;
`*pLen` receives its size in bytes. The caller owns the returned buffer.

### VFPSearch_Search

Low-level search using raw fingerprint data:

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search(
    const char* pData1, int iLen1, int iSkip1,
    const char* pData2, int iLen2, int iSkip2,
    double* pDiff, int maxDiff);
```

Returns the position in seconds where `pData1` was found inside `pData2`,
or `INT_MAX` if not found.

### VFPSearch_Search2

Higher-level variant using `VFPFingerPrint` structs:

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, int iSkip1,
    VFPFingerPrint* vfp2, int iSkip2,
    double* pDiff, int maxDiff);
```

### VFPSearch_GetFingerprintForVideoFile

High-level convenience: generates a search fingerprint directly from a video file.

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Returns `NULL` on success, or an error message string on failure.

**Example — high-level search workflow:**

```cpp
VFPFingerprintSource src{};
VFPFillSource(L"video.mp4", &src);

VFPFingerPrint fp{};
VFPSearch_GetFingerprintForVideoFile(src, &fp);
// fp.Data / fp.DataSize now contain the fingerprint
```

## Compare Functions

The Compare API provides both high-level convenience and low-level per-frame access.

### High-Level API

#### VFPCompare_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

Generates a comparison fingerprint directly from a video file.

#### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1, int iLen1,
    const char* pData2, int iLen2,
    int MaxS);
```

Compares two raw fingerprint buffers. `MaxS` is maximum time shift in seconds. Returns a difference score (lower = more similar).

### Low-Level Per-Frame API

#### VFPCompare_Init

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPCompare_Init(int count, void* pDataTmp);
```

#### VFPCompare_Process

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPCompare_Process(
    unsigned char* p, int w, int h, int s,
    double dTime, void* pDataTmp);
```

Feeds one decoded RGB frame. `dTime` is timestamp in seconds.

#### VFPCompare_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPCompare_Build(int* pLen, void* pDataTmp);
```

Finalizes the fingerprint. Returns a `char*` buffer; `*pLen` receives its size.

#### VFPCompare_Clear

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPCompare_Clear(void* pDataTmp);
```

Frees accumulator resources.

Feeds one decoded video frame into the accumulator.

### VFPCompare_Build

```cpp
extern "C" VFP_EXPORT char* VFP_EXPORT_CALL VFPCompare_Build(
    int* pLen,
    void* pDataTmp);
```

Finalizes the fingerprint. Returns a `char*` buffer; `*pLen` receives its size.

### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1, int iLen1,
    const char* pData2, int iLen2,
    int MaxS);
```

Returns a difference score (lower = more similar). `MaxS` is the maximum time shift in seconds.

### VFPCompare_GetFingerprintForVideoFile

High-level convenience for comparison fingerprints:

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

**Example — high-level compare workflow:**

```cpp
VFPFingerprintSource src{};
VFPFillSource(L"video.mp4", &src);

VFPFingerPrint fp{};
VFPCompare_GetFingerprintForVideoFile(src, &fp);

// Compare with another fingerprint:
double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                 fp2.Data, fp2.DataSize, 10);
```

## Utility Functions

### VFPFingerprintSave

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSave(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description:** Saves a fingerprint to a file in the current format.

**Parameters:**

- `vfp`: Pointer to fingerprint to save
- `filename`: Path to the output file

**Example:**

```cpp
VFPFingerPrint fingerprint{};
// ... generate fingerprint ...
VFPFingerprintSave(&fingerprint, L"output.vfpsig");
```

### VFPFingerprintLoad

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintLoad(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description:** Loads a fingerprint from a file.

**Parameters:**

- `vfp`: Pointer to fingerprint structure to receive the data
- `filename`: Path to the fingerprint file

**Example:**

```cpp
VFPFingerPrint fingerprint{};
VFPFingerprintLoad(&fingerprint, L"saved.vfpsig");

printf("Loaded fingerprint:\n");
printf("  Duration: %lld ms\n", fingerprint.Duration);
printf("  Original file: %ls\n", fingerprint.OriginalFilename);
printf("  Resolution: %dx%d\n", fingerprint.Width, fingerprint.Height);
```

### VFPFingerprintSaveLegacy / VFPFingerprintLoadLegacy

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintSaveLegacy(
    VFPFingerPrint* vfp,
    wchar_t* filename);

extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFingerprintLoadLegacy(
    VFPFingerPrint* vfp,
    wchar_t* filename);
```

**Description:** Save and load fingerprints in legacy format for backward compatibility.

## Image Comparison

### VFPImageCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPImageCompare_Compare(
    BYTE* image1,
    int image1width,
    int image1height,
    BYTE* image2,
    int image2width,
    int image2height);
```

**Description:** Compares two images and returns a similarity score.

**Parameters:**

- `image1`: First image data (RGB24 format)
- `image1width`: Width of first image
- `image1height`: Height of first image
- `image2`: Second image data (RGB24 format)
- `image2width`: Width of second image
- `image2height`: Height of second image

**Returns:** Similarity score (0-100, higher values indicate more similarity)

**Example:**

```cpp
// Assume we have two RGB24 images loaded
BYTE* img1 = LoadImage("image1.bmp", &width1, &height1);
BYTE* img2 = LoadImage("image2.bmp", &width2, &height2);

double similarity = VFPImageCompare_Compare(
    img1, width1, height1,
    img2, width2, height2
);

printf("Image similarity: %.2f%%\n", similarity);
```

## Complete Working Examples

### Example 1: Generate Search Fingerprint (high-level API)

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    VFPFingerprintSource src{};
    VFPFillSource(L"sample.mp4", &src);
    src.StartTime = 10000;   // start at 10s
    src.StopTime = 60000;    // stop at 60s

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    printf("Fingerprint: %dx%d, %.1fs, %d bytes\n",
           fp.Width, fp.Height, fp.Duration / 1000.0, fp.DataSize);
    VFPFingerprintSave(&fp, L"sample.vfpsig");
    return 0;
}
```

### Example 2: Compare Two Videos

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    // Generate fingerprint for video 1
    VFPFingerprintSource src1{};
    VFPFillSource(L"video1.mp4", &src1);
    VFPFingerPrint fp1{};
    VFPCompare_GetFingerprintForVideoFile(src1, &fp1);

    // Generate fingerprint for video 2
    VFPFingerprintSource src2{};
    VFPFillSource(L"video2.mp4", &src2);
    VFPFingerPrint fp2{};
    VFPCompare_GetFingerprintForVideoFile(src2, &fp2);

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);
    printf("Difference: %.2f\n", diff);
    if (diff < 100)       printf("Very similar\n");
    else if (diff < 500)  printf("Some similarity\n");
    else                  printf("Different\n");

    return 0;
}
```

### Example 3: Search Fragment in Longer Video

```cpp
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");

    // Build needle fingerprint (short fragment)
    VFPFingerprintSource needleSrc{};
    VFPFillSource(L"fragment.mp4", &needleSrc);
    VFPFingerPrint needle{};
    VFPSearch_GetFingerprintForVideoFile(needleSrc, &needle);

    // Build haystack fingerprint (long video)
    VFPFingerprintSource haystackSrc{};
    VFPFillSource(L"broadcast.mp4", &haystackSrc);
    VFPFingerPrint haystack{};
    VFPSearch_GetFingerprintForVideoFile(haystackSrc, &haystack);

    // Search for all occurrences
    double diff = 0;
    int pos = 0;
    const int needleSec = (int)(needle.Duration / 1000);

    while (pos < (int)(haystack.Duration / 1000))
    {
        pos = VFPSearch_Search2(&needle, 0, &haystack, pos, &diff, 300);
        if (pos == INT_MAX) break;

        printf("Match at %d seconds (diff: %.2f)\n", pos, diff);
        pos += needleSec;
    }

    return 0;
}
```

## Platform Support

### Windows

- **Architectures**: x86, x64
- **Compilers**: Visual Studio 2019 or later, MinGW-w64
- **Libraries**: VisioForge_VideoFingerprinting.dll (x86/x64)

### Linux

- **Architectures**: x64, ARM64
- **Compilers**: GCC 7+, Clang 6+
- **Dependencies**: GStreamer 1.18+

### macOS

- **Architectures**: x64, Apple Silicon (M1/M2)
- **Compilers**: Xcode 12+, Clang
- **Frameworks**: No additional dependencies

## Building and Linking

### Visual Studio (Windows)

1. Add include directory:

   ```
   $(SolutionDir)include
   ```

2. Add library directory:

   ```
   $(SolutionDir)lib
   ```

3. Link libraries:
   - For x86: `VisioForge_VideoFingerprinting.lib`
   - For x64: `VisioForge_VideoFingerprinting_x64.lib`

4. Copy runtime DLLs to output directory:
   - `VisioForge_VideoFingerprinting.dll` or `VisioForge_VideoFingerprinting_x64.dll`
   - `VisioForge_FFMPEG_Source.dll` or `VisioForge_FFMPEG_Source_x64.dll`

### CMake

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)

# Include directories
include_directories(${CMAKE_CURRENT_SOURCE_DIR}/include)

# Link directories
link_directories(${CMAKE_CURRENT_SOURCE_DIR}/lib)

# Add executable
add_executable(vfp_example main.cpp)

# Link libraries
if(WIN32)
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting_x64)
    else()
        target_link_libraries(vfp_example VisioForge_VideoFingerprinting)
    endif()
endif()

# Copy DLLs on Windows
if(WIN32)
    add_custom_command(TARGET vfp_example POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E copy_if_different
        "${CMAKE_CURRENT_SOURCE_DIR}/redist/*.dll"
        $<TARGET_FILE_DIR:vfp_example>)
endif()
```

### Linux/macOS

```bash
# Compile
g++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example

# Set library path (Linux)
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH

# Set library path (macOS)
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Run
./vfp_example
```

## Performance Considerations

### Memory Management

- Fingerprint data is allocated internally by the SDK
- Always check return values for errors
- Free resources properly when done

### Processing Speed

- Search fingerprints process approximately 30x real-time on modern CPUs
- Compare fingerprints process approximately 100x real-time
- Performance scales with CPU cores for batch processing

### Optimization Tips

1. **Use appropriate fingerprint type**: Search for fragment detection, Compare for whole-video comparison
2. **Set time ranges**: Process only required segments to reduce processing time
3. **Configure ignored areas**: Exclude logos and tickers to improve accuracy
4. **Adjust thresholds**: Balance between false positives and false negatives
5. **Cache fingerprints**: Save generated fingerprints to avoid reprocessing

## Error Handling

All functions that return `wchar_t*` return NULL on success and an error message on failure:

```cpp
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
if (error != nullptr) {
    wprintf(L"Error occurred: %s\n", error);
    // Handle error appropriately
    return -1;
}
```

Common error scenarios:

- File not found or inaccessible
- Unsupported video format
- Insufficient memory
- Invalid license key
- Corrupted fingerprint file

## Threshold Guidelines

### Search Operations

- **100-200**: Very strict matching (exact or near-exact copies)
- **200-400**: Normal matching (minor encoding differences allowed)
- **400-600**: Loose matching (significant quality differences allowed)
- **600+**: Very loose matching (may produce false positives)

### Compare Operations

- **< 100**: Videos are nearly identical
- **100-300**: Videos are very similar (likely same content)
- **300-500**: Videos have significant similarities
- **500-1000**: Videos have some similarities
- **> 1000**: Videos are different

## Best Practices

1. **Always set a license key** before calling any SDK functions
2. **Check return values** for all operations
3. **Use appropriate fingerprint types** for your use case
4. **Save fingerprints** to avoid reprocessing large video files
5. **Configure ignored areas** for content with overlays or logos
6. **Test threshold values** with your specific content type
7. **Handle errors gracefully** and provide meaningful feedback
8. **Free resources** when no longer needed
9. **Use batch processing** for multiple files
10. **Monitor memory usage** when processing many files

## Comparison with .NET SDK

The C++ SDK provides the same core functionality as the .NET SDK with these differences:

### Advantages

- Direct native performance without managed overhead
- Lower memory footprint
- Easier integration with existing C++ applications
- No .NET runtime requirement

### Differences

- Manual memory management required
- Uses wide character strings for Windows compatibility
- Function-based API instead of object-oriented
- Direct access to low-level processing functions

### Feature Parity

Both SDKs support:

- Search and Compare fingerprint generation
- Fragment detection within larger videos
- Similarity comparison between videos
- Image comparison (Windows only for C++)
- Custom crop and ignored areas
- Time range specification
- Fingerprint save/load operations

## Support and Resources

For additional support and resources:

- **Samples**: Available in the SDK package under `Demos/` directory
- **Support**: <support@visioforge.com>
- **License**: <https://www.visioforge.com/>
