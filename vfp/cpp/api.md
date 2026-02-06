---
title: Video Fingerprinting SDK C++ API Documentation
description: Complete API documentation for VisioForge Video Fingerprinting SDK C++ to generate, compare, and search video fingerprints with examples.
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

### VFPSearch_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPSearch_GetFingerprintForVideoFile(
    VFPFingerprintSource source, 
    VFPFingerPrint* vfp);
```

**Description:** Generates a search-optimized fingerprint from a video file.

**Parameters:**

- `source`: Source video configuration
- `vfp`: Pointer to fingerprint structure to receive the result

**Returns:** Error message if failed, NULL if successful

**Example:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\commercial.mp4", &source);

VFPFingerPrint fingerprint{};
wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);

if (error == nullptr) {
    printf("Fingerprint generated successfully\n");
    printf("Duration: %lld ms\n", fingerprint.Duration);
    printf("Size: %d bytes\n", fingerprint.DataSize);
} else {
    wprintf(L"Error: %s\n", error);
}
```

### VFPSearch_Search2

```cpp
extern "C" VFP_EXPORT int VFP_EXPORT_CALL VFPSearch_Search2(
    VFPFingerPrint* vfp1, 
    int iSkip1,
    VFPFingerPrint* vfp2, 
    int iSkip2, 
    double* pDiff,
    int maxDiff);
```

**Description:** Searches for one fingerprint within another, returning the position where found.

**Parameters:**

- `vfp1`: Fingerprint to search for (needle)
- `iSkip1`: Starting position in vfp1 (in seconds)
- `vfp2`: Fingerprint to search within (haystack)
- `iSkip2`: Starting position in vfp2 (in seconds)
- `pDiff`: Pointer to receive the difference value
- `maxDiff`: Maximum allowed difference threshold

**Returns:** Position in seconds where match was found, or INT_MAX if not found

**Example:**

```cpp
// Search for commercial in broadcast
VFPFingerPrint commercial{};
VFPFingerPrint broadcast{};

// Load fingerprints
VFPFingerprintLoad(&commercial, L"commercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"broadcast.vfpsig");

double diff = 0;
int position = VFPSearch_Search2(&commercial, 0, &broadcast, 0, &diff, 300);

if (position != INT_MAX) {
    printf("Commercial found at position: %d seconds\n", position);
    printf("Difference score: %.2f\n", diff);
} else {
    printf("Commercial not found in broadcast\n");
}
```

### VFPSearch_SearchOneSignatureFileInAnother

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPSearch_SearchOneSignatureFileInAnother(
    wchar_t* file1, 
    wchar_t* file2, 
    LONG threshold,
    LONG* position);
```

**Description:** Searches for one signature file within another directly from disk.

**Parameters:**

- `file1`: Path to fingerprint file to search for
- `file2`: Path to fingerprint file to search within
- `threshold`: Maximum allowed difference threshold
- `position`: Pointer to receive the position where found

**Example:**

```cpp
LONG position = 0;
VFPSearch_SearchOneSignatureFileInAnother(
    L"needle.vfpsig", 
    L"haystack.vfpsig", 
    300,  // threshold
    &position);

if (position != INT_MAX) {
    printf("Match found at: %ld seconds\n", position);
}
```

## Compare Functions

### VFPCompare_GetFingerprintForVideoFile

```cpp
extern "C" VFP_EXPORT wchar_t* VFP_EXPORT_CALL VFPCompare_GetFingerprintForVideoFile(
    VFPFingerprintSource source,
    VFPFingerPrint* vfp);
```

**Description:** Generates a comparison-optimized fingerprint from a video file.

**Parameters:**

- `source`: Source video configuration
- `vfp`: Pointer to fingerprint structure to receive the result

**Returns:** Error message if failed, NULL if successful

**Example:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\video1.mp4", &source);

VFPFingerPrint fingerprint{};
wchar_t* error = VFPCompare_GetFingerprintForVideoFile(source, &fingerprint);

if (error == nullptr) {
    // Save fingerprint for later comparison
    VFPFingerprintSave(&fingerprint, L"video1.vfpsig");
}
```

### VFPCompare_Compare

```cpp
extern "C" VFP_EXPORT double VFP_EXPORT_CALL VFPCompare_Compare(
    const char* pData1,
    int iLen1,
    const char* pData2,
    int iLen2,
    int MaxS);
```

**Description:** Compares two fingerprints and returns a difference score.

**Parameters:**

- `pData1`: First fingerprint data
- `iLen1`: Size of first fingerprint
- `pData2`: Second fingerprint data
- `iLen2`: Size of second fingerprint
- `MaxS`: Maximum time shift in seconds to check

**Returns:** Difference score (lower values indicate more similarity)

**Example:**

```cpp
VFPFingerPrint fp1{}, fp2{};
VFPFingerprintLoad(&fp1, L"video1.vfpsig");
VFPFingerprintLoad(&fp2, L"video2.vfpsig");

double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Check up to 10 seconds shift
);

printf("Difference score: %.2f\n", difference);
if (difference < 100) {
    printf("Videos are very similar (likely duplicates)\n");
} else if (difference < 500) {
    printf("Videos have significant similarities\n");
} else {
    printf("Videos are different\n");
}
```

## Utility Functions

### VFPFillSource

```cpp
extern "C" VFP_EXPORT void VFP_EXPORT_CALL VFPFillSource(
    wchar_t* filename,
    VFPFingerprintSource* source);
```

**Description:** Initializes a VFPFingerprintSource structure with default values for a video file.

**Parameters:**

- `filename`: Path to the video file
- `source`: Pointer to source structure to initialize

**Example:**

```cpp
VFPFingerprintSource source{};
VFPFillSource(L"C:\\Videos\\sample.mp4", &source);

// Optionally modify settings after initialization
source.StartTime = 5000;  // Start at 5 seconds
source.StopTime = 30000;  // Stop at 30 seconds
```

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

### Example 1: Generate Fingerprint

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main()
{
    // Set license key
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");
    
    // Configure source
    VFPFingerprintSource source{};
    VFPFillSource(L"C:\\Videos\\sample.mp4", &source);
    
    // Optional: Process only a segment
    source.StartTime = 10000;  // Start at 10 seconds
    source.StopTime = 60000;   // Stop at 60 seconds
    
    // Optional: Ignore logo area
    source.IgnoredAreas[0] = {1800, 0, 1920, 100};
    
    // Generate search fingerprint
    VFPFingerPrint fingerprint{};
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error == nullptr) {
        printf("Fingerprint generated successfully\n");
        printf("Duration: %lld ms\n", fingerprint.Duration);
        printf("Data size: %d bytes\n", fingerprint.DataSize);
        
        // Save to file
        VFPFingerprintSave(&fingerprint, L"sample.vfpsig");
        printf("Fingerprint saved to sample.vfpsig\n");
    } else {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    return 0;
}
```

### Example 2: Compare Two Videos

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main(int argc, char* argv[])
{
    if (argc != 3) {
        printf("Usage: compare video1.mp4 video2.mp4\n");
        return 1;
    }
    
    // Set license key
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");
    
    // Generate fingerprints for both videos
    VFPFingerprintSource source1{}, source2{};
    VFPFingerPrint fp1{}, fp2{};
    
    // Convert filenames to wide char
    wchar_t file1[256], file2[256];
    mbstowcs(file1, argv[1], 256);
    mbstowcs(file2, argv[2], 256);
    
    // Generate first fingerprint
    VFPFillSource(file1, &source1);
    wchar_t* error = VFPCompare_GetFingerprintForVideoFile(source1, &fp1);
    if (error != nullptr) {
        wprintf(L"Error processing first video: %s\n", error);
        return 1;
    }
    
    // Generate second fingerprint
    VFPFillSource(file2, &source2);
    error = VFPCompare_GetFingerprintForVideoFile(source2, &fp2);
    if (error != nullptr) {
        wprintf(L"Error processing second video: %s\n", error);
        return 1;
    }
    
    // Compare fingerprints
    double difference = VFPCompare_Compare(
        fp1.Data, fp1.DataSize,
        fp2.Data, fp2.DataSize,
        10  // Check up to 10 seconds shift
    );
    
    printf("Comparison Results:\n");
    printf("  Video 1: %ls (%.2f seconds)\n", file1, fp1.Duration / 1000.0);
    printf("  Video 2: %ls (%.2f seconds)\n", file2, fp2.Duration / 1000.0);
    printf("  Difference score: %.2f\n", difference);
    
    if (difference < 100) {
        printf("  Result: Videos are very similar (likely duplicates)\n");
    } else if (difference < 500) {
        printf("  Result: Videos have significant similarities\n");
    } else {
        printf("  Result: Videos are different\n");
    }
    
    return 0;
}
```

### Example 3: Search for Commercial in Broadcast

```cpp
#include <iostream>
#include <Windows.h>
#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>
#include <ctime>

std::string timeToString(time_t tm)
{
    char buff[20];
    strftime(buff, 20, "%H:%M:%S", gmtime(&tm));
    return std::string(buff);
}

int main(int argc, char* argv[])
{
    if (argc != 3) {
        printf("Usage: search commercial.mp4 broadcast.mp4\n");
        return 1;
    }
    
    // Set license key
    VFPSetLicenseKey(L"YOUR-LICENSE-KEY");
    
    wchar_t commercial[256], broadcast[256];
    mbstowcs(commercial, argv[1], 256);
    mbstowcs(broadcast, argv[2], 256);
    
    // Generate fingerprints
    VFPFingerprintSource src1{}, src2{};
    VFPFingerPrint fp_commercial{}, fp_broadcast{};
    
    printf("Generating fingerprint for commercial...\n");
    VFPFillSource(commercial, &src1);
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(src1, &fp_commercial);
    if (error != nullptr) {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    printf("Generating fingerprint for broadcast...\n");
    VFPFillSource(broadcast, &src2);
    error = VFPSearch_GetFingerprintForVideoFile(src2, &fp_broadcast);
    if (error != nullptr) {
        wprintf(L"Error: %s\n", error);
        return 1;
    }
    
    // Search for all occurrences
    printf("\nSearching for commercial in broadcast...\n");
    printf("Commercial duration: %.2f seconds\n", fp_commercial.Duration / 1000.0);
    printf("Broadcast duration: %.2f seconds\n\n", fp_broadcast.Duration / 1000.0);
    
    const int threshold = 300;
    double diff = 0;
    int position = 0;
    int occurrences = 0;
    
    const int commercial_duration = (int)(fp_commercial.Duration / 1000);
    const int broadcast_duration = (int)(fp_broadcast.Duration / 1000);
    
    while (position < broadcast_duration) {
        position = VFPSearch_Search2(
            &fp_commercial, 0,
            &fp_broadcast, position,
            &diff, threshold
        );
        
        if (position == INT_MAX) {
            break;
        }
        
        occurrences++;
        printf("Match #%d found at %s (difference: %.2f)\n",
               occurrences, timeToString(position).c_str(), diff);
        
        // Skip past this occurrence
        position += commercial_duration;
    }
    
    if (occurrences == 0) {
        printf("Commercial not found in broadcast.\n");
    } else {
        printf("\nTotal occurrences found: %d\n", occurrences);
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
