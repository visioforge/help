---
title: Getting Started with Video Fingerprinting SDK C++
description: Install and configure Video Fingerprinting SDK C++ with Visual Studio setup, project configuration, and step-by-step implementation guide.
---

# Getting Started with Video Fingerprinting SDK C++

Welcome to the VisioForge Video Fingerprinting SDK for C++! This comprehensive guide will walk you through everything you need to get started, from installation to your first working application. By the end of this guide, you'll have a solid foundation for building high-performance video fingerprinting applications in C++.

> **Note:** If you're already familiar with the .NET SDK, you'll find the C++ SDK follows similar concepts with native performance benefits. See our [SDK Comparison](../index.md#sdk-comparison) for details.

## Quick Start Summary

If you're looking to get up and running quickly:

1. Download the SDK package from VisioForge
2. Extract the files to your project directory
3. Include the headers: `#include <VisioForge_VFP.h>` and `#include <VisioForge_VFP_Types.h>`
4. Link the appropriate library: `VisioForge_VideoFingerprinting.lib` (x86) or `VisioForge_VideoFingerprinting_x64.lib` (x64)
5. Copy runtime DLLs to your output directory
6. Set your license key (if purchased): `VFPSetLicenseKey(L"license-key");`
7. Generate your first fingerprint using the examples below

## Prerequisites and System Requirements

For detailed system requirements including supported platforms, hardware specifications, and performance considerations, please see our comprehensive [System Requirements](../system-requirements.md) guide.

### C++-Specific Requirements

- **Windows Compiler**: Visual Studio 2019+ (recommended) or MinGW-w64
- **Linux Compiler**: GCC 7+ or Clang 6+
- **macOS Compiler**: Xcode 12+ with Command Line Tools
- **Build Tools**: CMake 3.10+ (optional but recommended for Linux/macOS)

## SDK Package Contents

After downloading the SDK, you'll find the following structure:

```
VideoFingerprinting_CPP_SDK/
├── include/
│   ├── VisioForge_VFP.h           # Main API header
│   └── VisioForge_VFP_Types.h     # Type definitions
├── lib/
│   ├── VisioForge_VideoFingerprinting.lib      # x86 import library
│   └── VisioForge_VideoFingerprinting_x64.lib  # x64 import library
├── redist/
│   ├── VisioForge_VideoFingerprinting.dll      # x86 runtime DLL
│   ├── VisioForge_VideoFingerprinting_x64.dll  # x64 runtime DLL
│   ├── VisioForge_FFMPEG_Source.dll           # x86 media support
│   └── VisioForge_FFMPEG_Source_x64.dll       # x64 media support
├── demos/
│   ├── vfp_gen/        # Fingerprint generation demo
│   ├── vfp_compare/    # Video comparison demo
│   └── vfp_search/     # Fragment search demo
└── README.txt
```

## Setting Up Your Development Environment

### Visual Studio Setup (Windows)

#### Step 1: Create a New Project

1. Open Visual Studio 2019 or later
2. Click "Create a new project"
3. Select "Console App" (C++)
4. Name your project (e.g., "VFPExample")
5. Choose a location and click "Create"

#### Step 2: Configure Project Properties

1. Right-click your project in Solution Explorer
2. Select "Properties"
3. Configure the following settings:

**Configuration Properties → C/C++ → General:**

```
Additional Include Directories: $(ProjectDir)include
```

**Configuration Properties → Linker → General:**

```
Additional Library Directories: $(ProjectDir)lib
```

**Configuration Properties → Linker → Input:**

```
Additional Dependencies (x86): VisioForge_VideoFingerprinting.lib
Additional Dependencies (x64): VisioForge_VideoFingerprinting_x64.lib
```

#### Step 3: Add SDK Files

1. Copy the `include` folder to your project directory
2. Copy the `lib` folder to your project directory
3. Copy DLL files from `redist` to your output directory (e.g., `Debug` or `Release`)

#### Step 4: Configure Post-Build Events

Add a post-build event to automatically copy DLLs:

```batch
xcopy /y "$(ProjectDir)redist\*.dll" "$(OutDir)"
```

### CMake Setup (Cross-Platform)

Create a `CMakeLists.txt` file:

```cmake
cmake_minimum_required(VERSION 3.10)
project(VFPExample)

set(CMAKE_CXX_STANDARD 11)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

# Find SDK files
set(VFP_SDK_PATH "${CMAKE_CURRENT_SOURCE_DIR}/sdk")

# Include directories
include_directories(${VFP_SDK_PATH}/include)

# Platform-specific configuration
if(WIN32)
    # Windows configuration
    link_directories(${VFP_SDK_PATH}/lib)
    
    if(CMAKE_SIZEOF_VOID_P EQUAL 8)
        set(VFP_LIBRARIES VisioForge_VideoFingerprinting_x64)
        set(VFP_RUNTIME_LIBS 
            ${VFP_SDK_PATH}/redist/VisioForge_VideoFingerprinting_x64.dll
            ${VFP_SDK_PATH}/redist/VisioForge_FFMPEG_Source_x64.dll)
    else()
        set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
        set(VFP_RUNTIME_LIBS 
            ${VFP_SDK_PATH}/redist/VisioForge_VideoFingerprinting.dll
            ${VFP_SDK_PATH}/redist/VisioForge_FFMPEG_Source.dll)
    endif()
elseif(APPLE)
    # macOS configuration
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
elseif(UNIX)
    # Linux configuration
    link_directories(${VFP_SDK_PATH}/lib)
    set(VFP_LIBRARIES VisioForge_VideoFingerprinting)
endif()

# Add executable
add_executable(vfp_example main.cpp)

# Link libraries
target_link_libraries(vfp_example ${VFP_LIBRARIES})

# Copy runtime libraries on Windows
if(WIN32)
    foreach(DLL ${VFP_RUNTIME_LIBS})
        add_custom_command(TARGET vfp_example POST_BUILD
            COMMAND ${CMAKE_COMMAND} -E copy_if_different
            ${DLL} $<TARGET_FILE_DIR:vfp_example>)
    endforeach()
endif()
```

Build the project:

```bash
mkdir build
cd build
cmake ..
cmake --build .
```

### Linux Setup

#### Install Dependencies

Ubuntu/Debian:

```bash
sudo apt-get update
sudo apt-get install build-essential cmake
sudo apt-get install libgstreamer1.0-dev libgstreamer-plugins-base1.0-dev
sudo apt-get install gstreamer1.0-plugins-good gstreamer1.0-plugins-bad
sudo apt-get install gstreamer1.0-libav
```

Fedora/RHEL:

```bash
sudo dnf install gcc-c++ cmake
sudo dnf install gstreamer1-devel gstreamer1-plugins-base-devel
sudo dnf install gstreamer1-plugins-good gstreamer1-plugins-bad-free
sudo dnf install gstreamer1-libav
```

#### Build Configuration

Create a simple Makefile:

```makefile
CXX = g++
CXXFLAGS = -std=c++11 -Wall -I./include
LDFLAGS = -L./lib -lVisioForge_VideoFingerprinting -Wl,-rpath,'$$ORIGIN/lib'

TARGET = vfp_example
SOURCES = main.cpp
OBJECTS = $(SOURCES:.cpp=.o)

all: $(TARGET)

$(TARGET): $(OBJECTS)
 $(CXX) $(OBJECTS) $(LDFLAGS) -o $(TARGET)

%.o: %.cpp
 $(CXX) $(CXXFLAGS) -c $< -o $@

clean:
 rm -f $(OBJECTS) $(TARGET)

.PHONY: all clean
```

### macOS Setup

#### Install Xcode Command Line Tools

```bash
xcode-select --install
```

#### Build Configuration

Create a build script `build.sh`:

```bash
#!/bin/bash

# Compiler settings
CXX=clang++
CXXFLAGS="-std=c++11 -Wall -I./include"
LDFLAGS="-L./lib -lVisioForge_VideoFingerprinting"

# Set library path
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH

# Build
$CXX $CXXFLAGS main.cpp $LDFLAGS -o vfp_example

echo "Build complete. Run with: ./vfp_example"
```

Make it executable and run:

```bash
chmod +x build.sh
./build.sh
```

## Your First Application

Let's create a simple application that generates a fingerprint from a video file.

### Step 1: Create main.cpp

```cpp
#include <iostream>
#include <string>
#include <cstring>

#ifdef _WIN32
#include <Windows.h>
#endif

#include <VisioForge_VFP.h>
#include <VisioForge_VFP_Types.h>

int main(int argc, char* argv[])
{
    std::cout << "VisioForge Video Fingerprinting SDK - First Application\n";
    std::cout << "========================================================\n\n";
    
    // Check command line arguments
    if (argc != 2) {
        std::cout << "Usage: " << argv[0] << " <video_file>\n";
        std::cout << "Example: " << argv[0] << " sample.mp4\n";
        return 1;
    }
    
    // Convert filename to wide character (for Windows compatibility)
    wchar_t videoFile[256];
#ifdef _WIN32
    size_t converted;
    mbstowcs_s(&converted, videoFile, argv[1], 256);
#else
    mbstowcs(videoFile, argv[1], 256);
#endif
    
    // Step 1: Set license key
    std::cout << "Setting license key...\n";
    VFPSetLicenseKey(L"TRIAL");  // Use "TRIAL" for evaluation
    
    // Step 2: Configure the source
    std::cout << "Configuring source for: " << argv[1] << "\n";
    VFPFingerprintSource source{};
    VFPFillSource(videoFile, &source);
    
    // Optional: Configure processing parameters
    // source.StartTime = 0;      // Start from beginning
    // source.StopTime = 60000;   // Process first 60 seconds
    
    // Step 3: Generate fingerprint
    std::cout << "Generating fingerprint...\n";
    VFPFingerPrint fingerprint{};
    
    wchar_t* error = VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
    
    if (error != nullptr) {
        std::wcerr << L"Error: " << error << L"\n";
        return 1;
    }
    
    // Step 4: Display results
    std::cout << "\nFingerprint generated successfully!\n";
    std::cout << "=====================================\n";
    std::cout << "Video Information:\n";
    std::cout << "  Duration: " << (fingerprint.Duration / 1000.0) << " seconds\n";
    std::cout << "  Resolution: " << fingerprint.Width << "x" << fingerprint.Height << "\n";
    std::cout << "  Frame Rate: " << fingerprint.FrameRate << " fps\n";
    std::cout << "  Data Size: " << fingerprint.DataSize << " bytes\n";
    
    // Step 5: Save fingerprint to file
    wchar_t outputFile[256];
#ifdef _WIN32
    wcscpy_s(outputFile, videoFile);
    wcscat_s(outputFile, L".vfpsig");
#else
    wcscpy(outputFile, videoFile);
    wcscat(outputFile, L".vfpsig");
#endif
    
    std::cout << "\nSaving fingerprint...\n";
    VFPFingerprintSave(&fingerprint, outputFile);
    
    char outputFileAnsi[256];
#ifdef _WIN32
    size_t convertedOut;
    wcstombs_s(&convertedOut, outputFileAnsi, outputFile, 256);
#else
    wcstombs(outputFileAnsi, outputFile, 256);
#endif
    
    std::cout << "Fingerprint saved to: " << outputFileAnsi << "\n";
    
    std::cout << "\nSuccess! You can now use this fingerprint for:\n";
    std::cout << "  - Comparing with other videos for similarity\n";
    std::cout << "  - Searching for this video in longer broadcasts\n";
    std::cout << "  - Building a database of video fingerprints\n";
    
    return 0;
}
```

### Step 2: Build and Run

#### Windows (Visual Studio)

1. Press F7 to build the solution
2. Copy a test video file to your output directory
3. Open Command Prompt in the output directory
4. Run: `VFPExample.exe testvideo.mp4`

#### Windows (Command Line)

```batch
cl /EHsc /I.\include main.cpp /link /LIBPATH:.\lib VisioForge_VideoFingerprinting_x64.lib
copy redist\*.dll .
VFPExample.exe testvideo.mp4
```

#### Linux

```bash
g++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example
export LD_LIBRARY_PATH=./lib:$LD_LIBRARY_PATH
./vfp_example testvideo.mp4
```

#### macOS

```bash
clang++ -std=c++11 -I./include main.cpp -L./lib -lVisioForge_VideoFingerprinting -o vfp_example
export DYLD_LIBRARY_PATH=./lib:$DYLD_LIBRARY_PATH
./vfp_example testvideo.mp4
```

## Understanding Fingerprint Types

The SDK provides two types of fingerprints optimized for different use cases. For a comprehensive explanation including technical details, performance characteristics, and decision guidance, see our [Fingerprint Types Guide](../fingerprint-types.md).

**Quick Reference:**
- **Search Fingerprints** (`VFPSearch_GetFingerprintForVideoFile()`): For finding video fragments within larger videos (commercial detection, content monitoring)
- **Compare Fingerprints** (`VFPCompare_GetFingerprintForVideoFile()`): For comparing entire videos for similarity (duplicate detection, version comparison)

## Common Use Cases

### Use Case 1: Duplicate Detection

```cpp
// Generate fingerprints for two videos
VFPFingerPrint fp1{}, fp2{};
// ... generate fingerprints ...

// Compare them
double difference = VFPCompare_Compare(
    fp1.Data, fp1.DataSize,
    fp2.Data, fp2.DataSize,
    10  // Allow up to 10 seconds shift
);

if (difference < 100) {
    std::cout << "Videos are duplicates\n";
}
```

### Use Case 2: Commercial Detection

```cpp
// Load commercial and broadcast fingerprints
VFPFingerPrint commercial{}, broadcast{};
VFPFingerprintLoad(&commercial, L"commercial.vfpsig");
VFPFingerprintLoad(&broadcast, L"broadcast.vfpsig");

// Search for commercial
double diff;
int position = VFPSearch_Search2(
    &commercial, 0,
    &broadcast, 0,
    &diff, 300  // threshold
);

if (position != INT_MAX) {
    std::cout << "Commercial found at: " << position << " seconds\n";
}
```

### Use Case 3: Content Verification

```cpp
// Generate fingerprint with ignored areas for logos
VFPFingerprintSource source{};
VFPFillSource(L"video.mp4", &source);

// Ignore logo areas
source.IgnoredAreas[0] = {0, 0, 200, 100};        // Top-left logo
source.IgnoredAreas[1] = {1720, 980, 1920, 1080}; // Bottom-right watermark

VFPFingerPrint fingerprint{};
VFPSearch_GetFingerprintForVideoFile(source, &fingerprint);
```

## Licensing

### Trial Mode

For evaluation, use the trial license:

```cpp
VFPSetLicenseKey(L"TRIAL");
```

Trial mode limitations:

- Adds watermark to processed frames
- Limited to 60 seconds of processing per session
- Full functionality otherwise available

### Commercial License

For production use, purchase a license from VisioForge:

```cpp
VFPSetLicenseKey(L"YOUR-LICENSE-KEY-HERE");
```

License types:

- **Developer License**: For development and testing
- **Deployment License**: For distribution with your application
- **Server License**: For server-based deployments

## Troubleshooting

### Common Issues and Solutions

#### Issue: DLL Not Found

**Error**: "The program can't start because VisioForge_VideoFingerprinting.dll is missing"

**Solution**:

- Ensure DLLs are in the same directory as your executable
- Or add the DLL directory to your PATH environment variable
- Check you're using the correct architecture (x86 vs x64)

#### Issue: Unsupported Video Format

**Error**: "Unable to process video file"

**Solution**:

- Ensure video codec is supported (H.264, H.265, VP8, VP9, AV1)
- Install additional codec packs if needed
- Try converting the video to a standard format

#### Issue: License Key Not Working

**Error**: "Invalid license key"

**Solution**:

- Verify the license key is entered correctly
- Ensure VFPSetLicenseKey() is called before any other SDK functions
- Check that the license hasn't expired
- Verify you're using the correct license for your platform

#### Issue: Memory Access Violation

**Error**: Access violation reading location

**Solution**:

- Initialize all structures with {} before use
- Check that pointers are valid before use
- Ensure proper string buffer sizes
- Don't free SDK-allocated memory manually

#### Issue: Poor Performance

**Symptom**: Processing is slower than expected

**Solution**:

- Use Release build configuration, not Debug
- Enable compiler optimizations (/O2 or -O2)
- Process videos from local SSD, not network drives
- Consider using multiple threads for batch processing
- Reduce video resolution if quality permits

### Debug Tips

1. **Enable debug output**: Set `VFPAnalyzer.DebugDir` to save intermediate results
2. **Check return values**: Always check for NULL/error returns
3. **Use debug builds**: Initially develop with debug symbols
4. **Log operations**: Add logging to track processing flow
5. **Test with known files**: Use reference videos to verify setup

## Next Steps

Now that you have a working setup, explore these advanced topics:

1. **Batch Processing**: Process multiple files efficiently
2. **Database Integration**: Store fingerprints in a database
3. **Real-time Processing**: Generate fingerprints from live streams
4. **Custom UI**: Build graphical interfaces for your applications
5. **Performance Optimization**: Tune for your specific use case

### Recommended Reading

- [C++ API Documentation](api.md) - Complete API reference
- [Understanding Video Fingerprinting](../understanding-video-fingerprinting.md) - Technical background
- [Use Cases](../use-cases.md) - Real-world applications

## Sample Projects

The SDK includes three complete sample projects:

### vfp_gen - Fingerprint Generation

Demonstrates how to generate fingerprints with various options:

```bash
vfp_gen source.mp4 output.sig 0 0 0
```

### vfp_compare - Video Comparison

Shows how to compare two videos for similarity:

```bash
vfp_compare video1.sig video2.sig 100 10
```

### vfp_search - Fragment Search

Illustrates searching for video fragments:

```bash
vfp_search needle.sig haystack.sig 300
```

Study these examples to understand best practices and common patterns.

## Support and Resources

### Getting Help

- **Support Email**: <support@visioforge.com>
- **Discord Community**: Join for real-time help and discussions
- **GitHub Examples**: Additional code samples and integrations

### Reporting Issues

When reporting issues, please provide:

1. SDK version and platform
2. Minimal code example reproducing the issue
3. Error messages and stack traces
4. Sample video files (if applicable)
5. System specifications

## Conclusion

You now have everything needed to start building video fingerprinting applications with the C++ SDK. The SDK provides powerful functionality with excellent performance, suitable for both desktop applications and server deployments.

Remember to:

- Start with the provided examples
- Test thoroughly with your target content
- Optimize parameters for your use case
- Reach out for support when needed

Happy coding with VisioForge Video Fingerprinting SDK!
