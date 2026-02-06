---
title: C++ Video Fingerprinting SDK Code Samples
description: Native C++ code examples and command-line samples for video fingerprint generation, comparison, and search with VisioForge SDK implementation.
sidebar_label: C++ Samples
---

# Video Fingerprinting SDK C++ Code Samples

## Available Samples

The C++ SDK includes command-line samples demonstrating core functionality. These samples are included in the SDK package under the `/samples/cpp/` directory.

### Core Functionality Examples

#### Generate Fingerprints

```cpp
// vfp_generate.cpp - Generate fingerprints from video files
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Usage: vfp_generate <input_video> <output_fingerprint>" << std::endl;
        return 1;
    }
    
    // Set license
    VFPSetLicenseKey(L"TRIAL");
    
    // Generate fingerprint
    VFP_SearchFingerprintGenerateSettings settings;
    settings.Mode = VFP_Mode::Search;
    settings.FrameStep = 10;
    
    auto result = VFPSearchFingerprintGenerate(
        argv[1],  // Input video
        argv[2],  // Output fingerprint
        &settings,
        nullptr   // Progress callback
    );
    
    return result == VFP_ErrorCode::Ok ? 0 : 1;
}
```

#### Compare Videos

```cpp
// vfp_compare.cpp - Compare two video fingerprints
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Usage: vfp_compare <fingerprint1> <fingerprint2>" << std::endl;
        return 1;
    }
    
    VFPSetLicenseKey(L"TRIAL");
    
    VFP_CompareResult result;
    auto status = VFPCompareFingerprints(
        argv[1],
        argv[2],
        &result
    );
    
    if (status == VFP_ErrorCode::Ok) {
        std::cout << "Similarity: " << result.Similarity << "%" << std::endl;
        std::cout << "Match: " << (result.IsMatch ? "Yes" : "No") << std::endl;
    }
    
    return status == VFP_ErrorCode::Ok ? 0 : 1;
}
```

#### Search for Fragments

```cpp
// vfp_search.cpp - Search for video fragments
#include <VisioForge_VFP.h>

int main(int argc, char* argv[]) {
    if (argc < 3) {
        std::cerr << "Usage: vfp_search <source_fingerprint> <target_fingerprint>" << std::endl;
        return 1;
    }
    
    VFPSetLicenseKey(L"TRIAL");
    
    VFP_SearchResult* results = nullptr;
    int count = 0;
    
    auto status = VFPSearchFingerprint(
        argv[1],  // Source (fragment)
        argv[2],  // Target (full video)
        &results,
        &count
    );
    
    if (status == VFP_ErrorCode::Ok) {
        std::cout << "Found " << count << " matches" << std::endl;
        for (int i = 0; i < count; i++) {
            std::cout << "Match " << i << ": Position " 
                     << results[i].Position << "ms, Similarity " 
                     << results[i].Similarity << "%" << std::endl;
        }
        VFPFreeSearchResults(results);
    }
    
    return status == VFP_ErrorCode::Ok ? 0 : 1;
}
```

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
    
    void ProcessVideo(const std::string& video) {
        VFP_SearchFingerprintGenerateSettings settings;
        settings.Mode = VFP_Mode::Search;
        
        std::string output = video + ".vfp";
        VFPSearchFingerprintGenerate(
            video.c_str(),
            output.c_str(),
            &settings,
            nullptr
        );
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
