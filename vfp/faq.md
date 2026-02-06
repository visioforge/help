---
title: Video Fingerprinting SDK FAQ
description: Find answers about VisioForge Video Fingerprinting SDK including licensing, performance, accuracy, formats, and platform compatibility.
---

# Video Fingerprinting SDK FAQ

This comprehensive FAQ addresses the most common questions about the VisioForge Video Fingerprinting SDK. If you can't find your answer here, please visit our [support forum](https://support.visioforge.com/) or [Discord community](https://discord.com/invite/yvXUG56WCH).

## Table of Contents

- [Licensing Questions](#licensing-questions)
- [Performance and Optimization](#performance-and-optimization)
- [Accuracy and Detection](#accuracy-and-detection)
- [Supported Formats and Codecs](#supported-formats-and-codecs)
- [Integration and Development](#integration-and-development)
- [Database and Storage](#database-and-storage)

## Licensing Questions

### Q: What's the difference between Trial and Commercial licenses?

**A:** The key differences are:

| Feature | Trial | Commercial |
|---------|-------|------------|
| Duration | 30 days | Perpetual |
| Watermark | Yes (visual overlay) | No |
| All Features | Yes | Yes |
| Production Use | No | Yes |
| Technical Support | Forum only | Email/Priority/Phone |
| Updates | Trial period only | 1 year |

### Q: Can I use the Trial license for development?

**A:** Yes! The trial license is perfect for development and testing. It includes all features with a watermark. You can develop your entire application with the trial license and purchase a commercial license when ready for production.

```csharp
// Development environment
VFPAnalyzer.SetLicenseKey("TRIAL");

// Production environment
VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE_KEY"));
```

### Q: What happens when my trial expires?

**A:** After 30 days, the SDK will stop processing videos and return an error. Your code remains intact - simply purchase a license and update the key to continue.

### Q: Are there any feature limitations with different licenses?

**A:** No, the commercial license includes all features without any limitations. The only difference between trial and commercial licenses is the watermark in trial mode.

### Q: Can I use one license on multiple machines?

**A:** License terms depend on your purchase:
- **Single Developer License**: One developer, unlimited development machines
- **Site License**: Unlimited developers at one physical location
- **Enterprise License**: Unlimited developers across multiple locations

For deployment, you need a runtime license for each production server or distributed application.

### Q: How do I handle licensing in a distributed application?

**A:** For distributed applications (installed on customer machines), you need:

```csharp
public class LicenseManager
{
    private const string EncryptedLicense = "YOUR_ENCRYPTED_LICENSE";
    
    public static void Initialize()
    {
        // Decrypt license at runtime
        string licenseKey = DecryptLicense(EncryptedLicense);
        VFPAnalyzer.SetLicenseKey(licenseKey);
    }
    
    private static string DecryptLicense(string encrypted)
    {
        // Implement your decryption logic
        // Never store plain text licenses in distributed apps
        return Decrypt(encrypted);
    }
}
```

## Performance and Optimization

### Q: How fast can the SDK process videos?

**A:** Processing speed depends on several factors:

| Factor | Impact on Speed |
|--------|----------------|
| Video Resolution | 4K: ~0.5x realtime, 1080p: ~2x realtime, 480p: ~5x realtime |
| CPU Cores | Linear scaling up to 8 cores |
| Hardware Acceleration | 2-5x faster with GPU support |
| Storage Type | SSD provides 30-50% speed improvement |
| Fingerprint Type | Search fingerprints are 2x slower than Compare |

Typical benchmarks on modern hardware (Intel i7, 16GB RAM, SSD):
- **1080p video**: 60-120 seconds per hour of content
- **720p video**: 30-60 seconds per hour of content
- **480p video**: 15-30 seconds per hour of content

### Q: How much memory does fingerprint generation require?

**A:** Memory usage scales with video resolution and duration:

```csharp
// Approximate memory usage calculation
long EstimateMemoryUsage(int width, int height, int durationSeconds)
{
    // Base memory for decoder and buffers
    long baseMemory = 100 * 1024 * 1024; // 100 MB
    
    // Frame buffer memory (3-5 frames typically buffered)
    long frameSize = width * height * 3; // RGB
    long frameBuffers = frameSize * 5;
    
    // Fingerprint data (approximately 1KB per second)
    long fingerprintSize = durationSeconds * 1024;
    
    return baseMemory + frameBuffers + fingerprintSize;
}

// Example: 1080p video, 10 minutes
// Memory ≈ 100MB + (1920*1080*3*5) + (600*1KB) ≈ 131MB
```

### Q: Can I process multiple videos simultaneously?

**A:** Yes, but with considerations:

```csharp
public class ParallelProcessor
{
    private readonly SemaphoreSlim _semaphore;
    
    public ParallelProcessor(int maxConcurrency = 4)
    {
        // Limit based on CPU cores and available memory
        _semaphore = new SemaphoreSlim(maxConcurrency);
    }
    
    public async Task ProcessMultipleVideos(string[] videoPaths)
    {
        var tasks = videoPaths.Select(ProcessVideoAsync);
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessVideoAsync(string videoPath)
    {
        await _semaphore.WaitAsync();
        try
        {
            var source = new VFPFingerprintSource(videoPath)
            {
                // Reduce resolution for parallel processing
                CustomResolution = new Size(640, 480)
            };
            
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
            // Process fingerprint...
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
```

Recommended concurrency:
- **8GB RAM**: 2-3 concurrent videos
- **16GB RAM**: 4-6 concurrent videos
- **32GB RAM**: 8-12 concurrent videos

### Q: How can I optimize for real-time processing?

**A:** For real-time or near real-time processing:

1. **Use hardware acceleration**:
```csharp
var source = new VFPFingerprintSource(videoPath)
{
    UseHardwareAcceleration = true,
    HardwareDevice = "cuda" // or "qsv", "d3d11va"
};
```

2. **Process in segments**:
```csharp
// Process 30-second segments for live streams
var source = new VFPFingerprintSource(streamUrl)
{
    StartTime = TimeSpan.FromSeconds(segmentIndex * 30),
    StopTime = TimeSpan.FromSeconds((segmentIndex + 1) * 30),
    CustomResolution = new Size(480, 360) // Lower resolution
};
```

3. **Use frame skipping**:
```csharp
var source = new VFPFingerprintSource(videoPath)
{
    FrameRate = 5 // Process 5 fps instead of full framerate
};
```

## Accuracy and Detection

### Q: How accurate is the video matching?

**A:** Accuracy depends on the transformation type:

| Transformation | Detection Rate | False Positive Rate |
|----------------|---------------|-------------------|
| Re-encoding | 99.9% | <0.1% |
| Resolution change | 99.5% | <0.1% |
| Watermark/Logo overlay | 98% | <0.5% |
| Cropping (< 20%) | 95% | <1% |
| Color adjustment | 93% | <2% |
| Heavy compression | 90% | <3% |
| Combined transformations | 85-95% | <5% |

### Q: What similarity threshold should I use?

**A:** Threshold recommendations based on use case:

```csharp
public enum MatchingThreshold
{
    Identical = 5,        // Same file, different encoding
    NearDuplicate = 15,   // Minor quality differences
    Similar = 30,         // Same content, some modifications
    Related = 50,         // Significant modifications (watermarks, etc.)
    PossiblyRelated = 100 // Heavy transformations
}

public bool IsMatch(int difference, MatchingThreshold threshold)
{
    return difference <= (int)threshold;
}
```

Use cases:
- **Copyright detection**: Use `Similar` (30) or stricter
- **Duplicate finding**: Use `NearDuplicate` (15)
- **Content monitoring**: Use `Related` (50) for flexibility
- **Scene detection**: Use `PossiblyRelated` (100)

### Q: Can the SDK detect partial matches?

**A:** Yes! The SDK excels at finding video fragments:

```csharp
// Search for a 30-second clip in a 2-hour movie
var searchFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("clip.mp4")
);

var mainFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    new VFPFingerprintSource("movie.mp4")
);

var results = VFPAnalyzer.Search(mainFp, searchFp, searchFp.Duration);

foreach (var result in results)
{
    Console.WriteLine($"Found match at {result.Position} with score {result.Score}");
}
```

### Q: How does the SDK handle different aspect ratios?

**A:** The SDK normalizes aspect ratios automatically, but you can improve accuracy:

```csharp
// For videos with letterboxing/pillarboxing
var source = new VFPFingerprintSource(videoPath)
{
    // Ignore black bars
    IgnoredAreas = new List<Rectangle>
    {
        new Rectangle(0, 0, 1920, 140),    // Top letterbox
        new Rectangle(0, 940, 1920, 140)   // Bottom letterbox
    }
};

// Or use smart cropping
var source = new VFPFingerprintSource(videoPath)
{
    AutoCropBlackBars = true
};
```

### Q: Can it detect mirrored or rotated videos?

**A:** The SDK can detect:
- ✅ Horizontal mirroring (with preprocessing)
- ✅ Minor rotations (< 5 degrees)
- ⚠️ 90/180/270 degree rotations (requires manual preprocessing)

```csharp
// For mirrored video detection
public async Task<bool> CheckMirroredMatch(string video1, string video2)
{
    var fp1 = await GenerateFingerprint(video1);
    var fp2 = await GenerateFingerprint(video2);
    
    // Check normal orientation
    int normalDiff = VFPAnalyzer.Compare(fp1, fp2);
    if (normalDiff < 30) return true;
    
    // Check mirrored version
    var fp2Mirrored = await GenerateFingerprint(video2, mirror: true);
    int mirroredDiff = VFPAnalyzer.Compare(fp1, fp2Mirrored);
    
    return mirroredDiff < 30;
}
```

## Supported Formats and Codecs

### Q: What video formats are supported?

**A:** The SDK supports virtually all common formats through GStreamer:

**Container Formats**:
- MP4, M4V, MOV
- AVI, WMV, ASF
- MKV, WebM
- FLV, F4V
- MPEG, MPG, M2TS, TS
- 3GP, 3G2
- OGV, OGG
- And many more...

**Video Codecs**:
- H.264/AVC
- H.265/HEVC
- VP8, VP9
- MPEG-1, MPEG-2, MPEG-4
- WMV7, WMV8, WMV9
- ProRes, DNxHD
- AV1 (with plugin)

### Q: Are there any format limitations?

**A:** Some limitations exist:

1. **DRM-protected content**: Cannot process encrypted videos
2. **Rare codecs**: May require additional GStreamer plugins
3. **Corrupted files**: Partial processing possible with error handling
4. **Live streams**: Supported but requires segmented processing
```

### Q: What about audio-only files?

**A:** The Video Fingerprinting SDK requires video content.

## Platform Compatibility

### Q: Which platforms are supported?

**A:** Full platform support matrix:

| Platform | Architecture | .NET Version | Status |
|----------|-------------|-------------|---------|
| Windows 10/11 | x86, x64, ARM64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Full Support |
| Windows Server 2016+ | x64 | .NET Framework 4.6.1+, .NET 6+ | ✅ Full Support |
| Ubuntu 20.04+ | x64, ARM64 | .NET 6+ | ✅ Full Support |
| Debian 11+ | x64, ARM64 | .NET 6+ | ✅ Full Support |
| RHEL/CentOS 8+ | x64 | .NET 6+ | ✅ Full Support |
| macOS 12+ | x64, ARM64 | .NET 6+ | ✅ Full Support |
| Docker/Kubernetes | Linux-based | .NET 6+ | ✅ Full Support |

### Q: Can I use the SDK in Docker containers?

**A:** Yes! Here's a sample Dockerfile:

```dockerfile
FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

# Install GStreamer and dependencies
RUN apt-get update && apt-get install -y \
    libgstreamer1.0-0 \
    gstreamer1.0-plugins-base \
    gstreamer1.0-plugins-good \
    gstreamer1.0-plugins-bad \
    gstreamer1.0-plugins-ugly \
    gstreamer1.0-libav \
    && rm -rf /var/lib/apt/lists/*

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["YourProject.csproj", "."]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "YourProject.dll"]
```

### Q: Are there platform-specific performance differences?

**A:** Yes, performance varies by platform:

| Platform | Relative Performance | Hardware Acceleration |
|----------|---------------------|----------------------|
| Windows x64 | 100% (baseline) | NVENC, Quick Sync, D3D11 |
| Linux x64 | 95-100% | NVENC, VAAPI |
| macOS Intel | 90-95% | VideoToolbox |
| macOS ARM64 | 85-90% | VideoToolbox |
| Windows ARM64 | 70-80% | Limited |

### Q: Can I use the SDK in cloud environments?

**A:** Yes, the SDK works in all major cloud platforms:

**AWS**:
```csharp
// Use GPU instances for better performance
// Recommended: g4dn.xlarge or p3.2xlarge
```

**Azure**:
```csharp
// Use NCv3-series or NVv4-series VMs
// Enable GPU acceleration in container instances
```

**Google Cloud**:
```csharp
// Use N1 with NVIDIA Tesla GPUs
// Or A2 machine series for best performance
```

## Integration and Development

### Q: Can I use the SDK in a web application?

**A:** Yes, for server-side processing in ASP.NET Core:

```csharp
public class VideoFingerprintService
{
    private readonly ILogger<VideoFingerprintService> _logger;
    
    public VideoFingerprintService(ILogger<VideoFingerprintService> logger)
    {
        _logger = logger;
        VFPAnalyzer.SetLicenseKey(Environment.GetEnvironmentVariable("VFP_LICENSE"));
    }
    
    public async Task<IActionResult> ProcessUpload(IFormFile file)
    {
        var tempPath = Path.GetTempFileName();
        try
        {
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var fingerprint = await GenerateFingerprint(tempPath);
            // Store fingerprint in database
            
            return Ok(new { Success = true, FingerprintId = fingerprint.Id });
        }
        finally
        {
            File.Delete(tempPath);
        }
    }
}
```

### Q: How do I implement a progress bar?

**A:** Use the progress callback:

```csharp
public class ProgressTracker
{
    public event EventHandler<int> ProgressChanged;
    
    public async Task<VFPFingerPrint> ProcessWithProgress(string videoPath)
    {
        var source = new VFPFingerprintSource(videoPath);
        
        return await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
            source,
            errorDelegate: (error) => {
                Console.WriteLine($"Error: {error}");
            },
            progressDelegate: (progress) => {
                ProgressChanged?.Invoke(this, progress);
            }
        );
    }
}

// Usage in WPF/WinForms
tracker.ProgressChanged += (s, progress) => {
    Dispatcher.Invoke(() => {
        progressBar.Value = progress;
        labelStatus.Text = $"Processing: {progress}%";
    });
};
```

## Database and Storage

### Q: How should I store fingerprints in a database?

**A:** Best practices for database storage:

```csharp
// SQL Server example
public class FingerprintRepository
{
    public async Task SaveFingerprint(VFPFingerPrint fp, string videoId)
    {
        // Option 1: Store as binary
        byte[] data = fp.Save();
        
        using var connection = new SqlConnection(connectionString);
        await connection.ExecuteAsync(
            @"INSERT INTO Fingerprints (VideoId, Data, Duration, Width, Height, Created)
              VALUES (@VideoId, @Data, @Duration, @Width, @Height, @Created)",
            new {
                VideoId = videoId,
                Data = data,
                Duration = fp.Duration.TotalSeconds,
                Width = fp.Width,
                Height = fp.Height,
                Created = DateTime.UtcNow
            });
    }
    
    public async Task<VFPFingerPrint> LoadFingerprint(string videoId)
    {
        using var connection = new SqlConnection(connectionString);
        var data = await connection.QuerySingleAsync<byte[]>(
            "SELECT Data FROM Fingerprints WHERE VideoId = @VideoId",
            new { VideoId = videoId });
        
        return VFPFingerPrint.Load(data);
    }
}

// MongoDB example
public class MongoFingerprintRepository
{
    private readonly IMongoCollection<FingerprintDocument> _collection;
    
    public async Task SaveFingerprint(VFPFingerPrint fp, string videoId)
    {
        var document = new FingerprintDocument
        {
            VideoId = videoId,
            Data = Convert.ToBase64String(fp.Save()),
            Metadata = new FingerprintMetadata
            {
                Duration = fp.Duration,
                Width = fp.Width,
                Height = fp.Height,
                FrameRate = fp.FrameRate
            }
        };
        
        await _collection.InsertOneAsync(document);
    }
}
```

### Q: How much storage space do fingerprints require?

**A:** Fingerprint sizes are predictable:

| Fingerprint Type | Size Formula | Example (10 min video) |
|-----------------|--------------|------------------------|
| Compare | ~100 bytes/second | ~60 KB |
| Search | ~1 KB/second | ~600 KB |
| With thumbnails | +10 KB/minute | ~660 KB |

Storage planning:
```csharp
public long EstimateStorageNeeded(int videoCount, double avgDurationMinutes)
{
    // Search fingerprints (worst case)
    long bytesPerMinute = 60 * 1024; // 60 KB
    long fingerprintSize = (long)(bytesPerMinute * avgDurationMinutes);
    
    // Add 20% overhead for metadata
    long totalPerVideo = (long)(fingerprintSize * 1.2);
    
    // Total storage needed
    return videoCount * totalPerVideo;
}

// Example: 10,000 videos, 30 minutes average
// Storage ≈ 10,000 * (60KB * 30 * 1.2) = ~21 GB
```

### Q: Should I use file system or database storage?

**A:** Depends on your requirements:

| Storage Type | Pros | Cons | Best For |
|-------------|------|------|----------|
| File System | Fast, simple, easy backup | Hard to query, no transactions | < 10,000 videos |
| SQL Database | ACID, queryable, metadata | Slower, size limits | 10,000 - 100,000 videos |
| NoSQL Database | Scalable, flexible | Complex setup | > 100,000 videos |
| Object Storage (S3) | Unlimited scale, cheap | Network latency | Archive/backup |

Hybrid approach (recommended for large scale):
```csharp
public class HybridStorage
{
    // Metadata in database for fast queries
    private readonly SqlConnection _db;
    
    // Fingerprint data in object storage
    private readonly IS3Client _s3;
    
    public async Task SaveFingerprint(VFPFingerPrint fp, VideoMetadata metadata)
    {
        // Save fingerprint to S3
        string s3Key = $"fingerprints/{metadata.VideoId}.vfp";
        await _s3.PutObjectAsync(s3Key, fp.Save());
        
        // Save metadata to database
        await _db.ExecuteAsync(
            @"INSERT INTO Videos (Id, Title, Duration, S3Key)
              VALUES (@Id, @Title, @Duration, @S3Key)",
            new {
                metadata.Id,
                metadata.Title,
                metadata.Duration,
                S3Key = s3Key
            });
    }
}
```

### Q: How do I debug performance issues?

**A:** Use profiling and metrics:

```csharp
public class PerformanceMonitor
{
    public async Task<FingerprintMetrics> ProcessWithMetrics(string videoPath)
    {
        var metrics = new FingerprintMetrics();
        var sw = Stopwatch.StartNew();
        
        // Check file size
        var fileInfo = new FileInfo(videoPath);
        metrics.FileSize = fileInfo.Length;
        
        // Monitor memory before
        long memoryBefore = GC.GetTotalMemory(false);
        
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
            source,
            progressDelegate: (progress) => {
                if (progress % 10 == 0)
                {
                    long currentMemory = GC.GetTotalMemory(false);
                    Console.WriteLine($"Progress: {progress}%, Memory: {currentMemory / 1024 / 1024} MB");
                }
            }
        );
        
        sw.Stop();
        
        metrics.ProcessingTime = sw.Elapsed;
        metrics.MemoryUsed = GC.GetTotalMemory(false) - memoryBefore;
        metrics.ProcessingSpeed = fileInfo.Length / sw.Elapsed.TotalSeconds;
        
        Console.WriteLine($"Performance Report:");
        Console.WriteLine($"  File Size: {metrics.FileSize / 1024 / 1024} MB");
        Console.WriteLine($"  Processing Time: {metrics.ProcessingTime}");
        Console.WriteLine($"  Memory Used: {metrics.MemoryUsed / 1024 / 1024} MB");
        Console.WriteLine($"  Speed: {metrics.ProcessingSpeed / 1024 / 1024} MB/s");
        
        return metrics;
    }
}
```

## Additional Resources

- **[API Reference](https://api.visioforge.org/dotnet/)** - Complete API documentation
- **[GitHub Samples](https://github.com/visioforge/.Net-SDK-s-samples/)** - Code examples
- **[Discord Community](https://discord.com/invite/yvXUG56WCH)** - Get help from the community

## Contact Support

If you can't find an answer in this FAQ:

1. **Search the forum**: [https://support.visioforge.com/](https://support.visioforge.com/)
2. **Join Discord**: [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)
3. **Email support**: support@visioforge.com (commercial licenses)
4. **Report bugs**: [GitHub Issues](https://github.com/visioforge/.Net-SDK-s-samples/issues)

When contacting support, please include:
- SDK version
- Platform and .NET version
- License type
- Error messages and stack traces
- Sample code reproducing the issue
- Debug logs if available