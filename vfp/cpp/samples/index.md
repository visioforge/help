---
title: Video Fingerprinting SDK C++ Code Samples and Examples
description: Native C++ code examples and command-line samples for video fingerprint generation, comparison, and search with VisioForge SDK implementation.
sidebar_label: C++ Samples
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

# Video Fingerprinting SDK C++ Code Samples

## Available Samples

The C++ SDK includes command-line samples demonstrating core functionality. These samples are included in the SDK package under the `/samples/cpp/` directory.

### Core Functionality Examples

#### Generate Fingerprints

```cpp
// vfp_gen.cpp - Generate fingerprint from video file
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerprintSource src{};
    VFPFillSource(L"input.mp4", &src);

    VFPFingerPrint fp{};
    VFPSearch_GetFingerprintForVideoFile(src, &fp);

    VFPFingerprintSave(&fp, L"output.vfpsig");
    printf("Fingerprint saved: %d bytes\n", fp.DataSize);
    return 0;
}
```

#### Compare Videos

```cpp
// vfp_compare.cpp - Compare two fingerprints
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint fp1{}, fp2{};
    VFPFingerprintLoad(&fp1, L"video1.vfpsig");
    VFPFingerprintLoad(&fp2, L"video2.vfpsig");

    double diff = VFPCompare_Compare(fp1.Data, fp1.DataSize,
                                     fp2.Data, fp2.DataSize, 10);

    printf("Difference: %.2f\n", diff);
    if (diff < 100)       printf("Very similar\n");
    else if (diff < 500)  printf("Some similarity\n");
    else                  printf("Different\n");

    return 0;
}
```

#### Search for Fragments

```cpp
// vfp_search.cpp - Search for fragment in longer video
#include <VisioForge_VFP.h>

int main()
{
    VFPSetLicenseKey(L"TRIAL");

    VFPFingerPrint needle{}, haystack{};
    VFPFingerprintLoad(&needle, L"fragment.vfpsig");
    VFPFingerprintLoad(&haystack, L"full_video.vfpsig");

    double diff = 0;
    int pos = VFPSearch_Search2(&needle, 0, &haystack, 0, &diff, 300);
    if (pos != INT_MAX)
        printf("Found at %d seconds (diff: %.2f)\n", pos, diff);

    return 0;
}
               results[i].startMs, results[i].endMs, results[i].difference);

    return 0;
}

### Building the Samples

#### Windows (Visual Studio)

```bash
# Open the Visual Studio solution
samples/cpp/VFPSamples.sln

# Or build from command line
msbuild VFPSamples.sln /p:Configuration=Release /p:Platform=x64
```

#### Linux/macOS (CMake)

```bash
cd samples/cpp
mkdir build && cd build
cmake ..
make
```

## Integration Examples

### Multi-threaded Processing

```cpp
#include <thread>
#include <vector>
#include <queue>
#include <mutex>

class FingerprintProcessor {
private:
    std::queue<std::string> videoQueue;
    std::mutex queueMutex;
    
public:
    void ProcessVideos(const std::vector<std::string>& videos) {
        const int numThreads = std::thread::hardware_concurrency();
        std::vector<std::thread> workers;
        
        // Fill queue
        for (const auto& video : videos) {
            videoQueue.push(video);
        }
        
        // Start worker threads
        for (int i = 0; i < numThreads; i++) {
            workers.emplace_back(&FingerprintProcessor::Worker, this);
        }
        
        // Wait for completion
        for (auto& worker : workers) {
            worker.join();
        }
    }
    
private:
    void Worker() {
        while (true) {
            std::string video;
            {
                std::lock_guard<std::mutex> lock(queueMutex);
                if (videoQueue.empty()) break;
                video = videoQueue.front();
                videoQueue.pop();
            }
            
            ProcessVideo(video);
        }
    }
    
    void ProcessVideo(const std::string& video)
    {
        VFPFingerprintSource src{};
        std::wstring wpath(video.begin(), video.end());
        VFPFillSource(wpath.c_str(), &src);

        VFPFingerPrint fp{};
        VFPSearch_GetFingerprintForVideoFile(src, &fp);
        // Store fp.Data / fp.DataSize
    }
};
```

### Database Integration

```cpp
// Example using SQLite for fingerprint storage
#include <sqlite3.h>

class FingerprintDatabase {
private:
    sqlite3* db;
    
public:
    void StoreFingerprint(const std::string& videoPath, 
                         const std::vector<uint8_t>& fingerprint) {
        const char* sql = "INSERT INTO fingerprints (path, data) VALUES (?, ?)";
        sqlite3_stmt* stmt;
        sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        sqlite3_bind_blob(stmt, 2, fingerprint.data(), 
                         fingerprint.size(), SQLITE_STATIC);
        
        sqlite3_step(stmt);
        sqlite3_finalize(stmt);
    }
    
    std::vector<uint8_t> LoadFingerprint(const std::string& videoPath) {
        const char* sql = "SELECT data FROM fingerprints WHERE path = ?";
        sqlite3_stmt* stmt;
        sqlite3_prepare_v2(db, sql, -1, &stmt, nullptr);
        
        sqlite3_bind_text(stmt, 1, videoPath.c_str(), -1, SQLITE_STATIC);
        
        std::vector<uint8_t> fingerprint;
        if (sqlite3_step(stmt) == SQLITE_ROW) {
            const void* data = sqlite3_column_blob(stmt, 0);
            int size = sqlite3_column_bytes(stmt, 0);
            
            fingerprint.resize(size);
            memcpy(fingerprint.data(), data, size);
        }
        
        sqlite3_finalize(stmt);
        return fingerprint;
    }
};
```

## Comparison with .NET Samples

| Feature | C++ Implementation | .NET Implementation |
|---------|-------------------|---------------------|
| **GUI Applications** | Qt/MFC examples available separately | WPF/WinForms samples available |
| **CLI Tools** | Included in SDK | [Comprehensive tools](../../dotnet/samples/index.md) |
| **Database Integration** | Manual implementation | Built-in MongoDB support |
| **Progress Callbacks** | Function pointers | Events and delegates |
| **Error Handling** | Return codes | Exceptions |

## Performance Optimization Tips

1. **Use appropriate frame steps** - Higher values process faster but may miss short segments
2. **Enable multi-threading** - Process multiple videos in parallel
3. **Reuse analyzer instances** - Avoid initialization overhead
4. **Batch operations** - Process multiple files before cleanup
5. **Use native paths** - Avoid string conversions

## Additional Resources

- [C++ API Reference](../api.md) - Complete API documentation
- [Getting Started Guide](../getting-started.md) - Setup and configuration
- [.NET Samples](../../dotnet/samples/index.md) - For comparison with managed code

## Support

For questions about these samples:

- Check the [FAQ](../../faq.md) for common issues
- Visit our [Support Portal](https://support.visioforge.com)
- Join our [Discord Community](https://discord.com/invite/yvXUG56WCH)
