---
title: Getting Started with Video Fingerprinting SDK .NET
description: Complete installation and setup guide for VisioForge Video Fingerprinting SDK with configuration, licenses, and step-by-step instructions.
---

# Getting Started with Video Fingerprinting SDK

Welcome to the VisioForge Video Fingerprinting SDK! This comprehensive guide will walk you through everything you need to get started, from installation to your first working application. By the end of this guide, you'll have a solid foundation for building video fingerprinting applications.

## Quick Start Summary

If you're looking to get up and running quickly:

1. Install the SDK via NuGet: `Install-Package VisioForge.DotNet.Core`
2. Add the redistribution package: `Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting`
   - This single package supports Windows (x86/x64), Linux (x64/ARM64), and macOS
3. Set your license key: `VFPAnalyzer.SetLicenseKey("TRIAL");`
4. Generate your first fingerprint using the examples below

## Prerequisites and System Requirements

For detailed system requirements including supported platforms, hardware specifications, and performance considerations, please see our comprehensive [System Requirements](../system-requirements.md) guide.

### .NET-Specific Requirements

- **.NET Version**: 
  - Windows: .NET Framework 4.6.1+ or .NET 6.0+
  - Linux/macOS: .NET 6.0+
- **IDE**: Visual Studio 2019+ (Windows), Visual Studio Code, or JetBrains Rider
- **NuGet Package Manager**: For easy installation and updates

## Installation Methods

### Method 1: NuGet Package Manager (Recommended)

The easiest way to install the SDK is through NuGet Package Manager in Visual Studio.

#### Via Package Manager UI

1. Right-click on your project in Solution Explorer
2. Select "Manage NuGet Packages"
3. Click "Browse" and search for "VisioForge.DotNet.Core"
4. Select the package and click "Install"
5. Accept the license agreement

#### Via Package Manager Console

```powershell
# Install the main SDK package
Install-Package VisioForge.DotNet.Core

# Install the redistribution package with native libraries (required)
# This package includes support for Windows (x86/x64), Linux (x64/arm64), and macOS
Install-Package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# For MongoDB integration (optional)
Install-Package VisioForge.DotNet.VideoFingerprinting.MongoDB
```

#### Via .NET CLI

```bash
# Add the main SDK package
dotnet add package VisioForge.DotNet.Core

# Add the redistribution package with native libraries (required)
# This package includes support for Windows (x86/x64), Linux (x64/arm64), and macOS
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting

# For MongoDB integration (optional)
dotnet add package VisioForge.DotNet.VideoFingerprinting.MongoDB

# Restore packages
dotnet restore
```

#### Via PackageReference (in .csproj)

```xml
<ItemGroup>
  <PackageReference Include="VisioForge.DotNet.Core" Version="2025.8.7" />
  <PackageReference Include="VisioForge.DotNet.Core.Redist.VideoFingerprinting" Version="2025.8.7" />
  
  <!-- Optional: MongoDB integration -->
  <PackageReference Include="VisioForge.DotNet.VideoFingerprinting.MongoDB" Version="2025.8.7" />
</ItemGroup>
```

!!! important "NuGet Package Requirements"

    The SDK requires two packages:

    1. **VisioForge.DotNet.Core** - The main SDK library with C# API
    2. **VisioForge.DotNet.Core.Redist.VideoFingerprinting** - Native libraries for video fingerprinting functionality (supports Windows x86/x64, Linux x64/arm64, and macOS)

    Both packages must be installed for the SDK to function properly. The redistribution package contains platform-specific native libraries that are automatically deployed to your output directory.

    **Optional packages:**

    - **VisioForge.DotNet.VideoFingerprinting.MongoDB** - MongoDB integration for storing fingerprints in a database

    **Platform Support:**
    The `VisioForge.DotNet.Core.Redist.VideoFingerprinting` package includes native libraries for:

    - Windows (x86 and x64)
    - Linux (x64 and ARM64)
    - macOS (Intel and Apple Silicon)

    The correct platform-specific libraries are automatically selected and deployed based on your target runtime.

### Method 2: Manual Installation

For environments where NuGet isn't available or for custom deployment scenarios:

1. **Download the SDK**
   - Visit the [product page](https://www.visioforge.com/video-fingerprinting-sdk)
   - Choose your platform and architecture

2. **Run the installer**

### Additional Platform-Specific Packages

While the `VisioForge.DotNet.Core.Redist.VideoFingerprinting` package includes all necessary native libraries for video fingerprinting, you may need additional packages for extended functionality:

#### For Windows Applications

```powershell
# Additional Windows-specific packages (optional, based on your needs)
Install-Package VisioForge.DotNet.Core.Redist.Base.x64  # Extended Windows x64 support
Install-Package VisioForge.DotNet.Core.Redist.Base.x86  # Extended Windows x86 support
```

#### For Mobile Applications

```powershell
# iOS/macOS/tvOS UI support
Install-Package VisioForge.DotNet.Core.UI.Apple

# Android UI support
Install-Package VisioForge.DotNet.Core.UI.Android
```

### Platform-Specific Setup

#### Windows Setup

Not required.

#### Linux Setup

1. **Install GStreamer Dependencies**

   ```bash
   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   
   # RHEL/CentOS
   sudo yum install -y \
     gstreamer1.0-plugins-base \
     gstreamer1.0-plugins-good \
     gstreamer1.0-plugins-bad \
     gstreamer1.0-plugins-ugly \
     gstreamer1.0-libav
   ```

#### macOS Setup

Not required.

## License Key Activation

### Obtaining a License Key

1. **Trial License**
   - Use empty string for evaluation
   - Full functionality with watermark
   - 30-day evaluation period

2. **Commercial License**
   - Purchase from [Product Page](https://www.visioforge.com/video-fingerprinting-sdk)
   - Receive license key via email
   - Use license key in your application

### Activating Your License

```csharp
using VisioForge.Core.VideoFingerPrinting;

// At application startup
public static void InitializeSDK()
{
    // For trial evaluation - do nothing
    
    // For commercial license
    VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");    
}
```

### License Validation

```csharp
// Verify license status
public static bool ValidateLicense()
{
    try
    {
        // Attempt a simple operation to verify license
        var testSource = new VFPFingerprintSource("test.mp4");
        testSource.StopTime = TimeSpan.FromSeconds(1);
        
        // This will fail if license is invalid
        var fp = VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(testSource).Result;
        
        return fp != null;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"License validation failed: {ex.Message}");
        return false;
    }
}
```

### License Types

| Feature | Trial | Commercial |
|---------|-------|------------|
| Basic Fingerprinting | ✅ | ✅ |
| Video Comparison | ✅ | ✅ |
| Fragment Search | ✅ | ✅ |
| Database Support | ✅ | ✅ |
| Cross-platform Support | ✅ | ✅ |
| Watermark | Yes | No |
| Technical Support | Forum | Email/Priority |
| Updates | 30 days | 1 year |

## Your First Fingerprint Generation

Let's create a simple console application that generates a video fingerprint:

### Step 1: Create a New Project

```bash
# Create a new console application
dotnet new console -n VideoFingerprintingDemo
cd VideoFingerprintingDemo

# Add the SDK packages
dotnet add package VisioForge.DotNet.Core
dotnet add package VisioForge.DotNet.Core.Redist.VideoFingerprinting
```

### Step 2: Basic Implementation

```csharp
using System;
using System.IO;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize the SDK with your license
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Specify the video file to process
            string videoPath = @"C:\Videos\sample.mp4";
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine($"Error: Video file not found at {videoPath}");
                return;
            }
            
            try
            {
                // Generate the fingerprint
                await GenerateFingerprint(videoPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        
        static async Task GenerateFingerprint(string videoPath)
        {
            Console.WriteLine($"Processing: {Path.GetFileName(videoPath)}");
            Console.WriteLine("----------------------------------------");
            
            // Create source configuration
            var source = new VFPFingerprintSource(videoPath);
            
            // Optional: Process only first 30 seconds for testing
            source.StopTime = TimeSpan.FromSeconds(30);
            
            // Optional: Downscale for faster processing
            source.CustomResolution = new VisioForge.Core.Types.Size(640, 480);
            
            // Generate fingerprint with progress tracking
            var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (error) => {
                    Console.WriteLine($"Error: {error}");
                },
                progressDelegate: (progress) => {
                    Console.Write($"\rProgress: {progress}%");
                }
            );
            
            Console.WriteLine(); // New line after progress
            
            if (fingerprint != null)
            {
                // Display fingerprint information
                Console.WriteLine("\nFingerprint Generated Successfully!");
                Console.WriteLine($"  Duration: {fingerprint.Duration}");
                Console.WriteLine($"  Resolution: {fingerprint.Width}x{fingerprint.Height}");
                Console.WriteLine($"  Frame Rate: {fingerprint.FrameRate:F2} fps");
                Console.WriteLine($"  Data Size: {fingerprint.Data?.Length ?? 0} bytes");
                
                // Save fingerprint to file
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fingerprint.Save(outputPath);
                
                Console.WriteLine($"\nFingerprint saved to: {outputPath}");
                Console.WriteLine($"File size: {new FileInfo(outputPath).Length / 1024} KB");
            }
            else
            {
                Console.WriteLine("Failed to generate fingerprint.");
            }
        }
    }
}
```

### Step 3: Run the Application

```bash
# Build and run
dotnet build
dotnet run

# Expected output:
# Processing: sample.mp4
# ----------------------------------------
# Progress: 100%
# 
# Fingerprint Generated Successfully!
#   Duration: 00:00:30
#   Resolution: 1920x1080
#   Frame Rate: 29.97 fps
#   Data Size: 125440 bytes
# 
# Fingerprint saved to: C:\Videos\sample.vfp
# File size: 122 KB
```

## Basic Comparison Example

Now let's compare two videos to determine their similarity:

```csharp
using System;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

class VideoComparisonDemo
{
    static async Task Main(string[] args)
    {
        VFPAnalyzer.SetLicenseKey("TRIAL");
        
        string video1 = @"C:\Videos\original.mp4";
        string video2 = @"C:\Videos\copy.mp4";
        
        await CompareVideos(video1, video2);
    }
    
    static async Task CompareVideos(string path1, string path2)
    {
        Console.WriteLine("Comparing videos...");
        Console.WriteLine($"Video 1: {Path.GetFileName(path1)}");
        Console.WriteLine($"Video 2: {Path.GetFileName(path2)}");
        Console.WriteLine("----------------------------------------");
        
        // Create sources with time limits for quick comparison
        var source1 = new VFPFingerprintSource(path1)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        var source2 = new VFPFingerprintSource(path2)
        {
            StopTime = TimeSpan.FromSeconds(30),
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        // Generate fingerprints
        Console.Write("Generating fingerprint 1...");
        var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
        Console.WriteLine(" Done");
        
        Console.Write("Generating fingerprint 2...");
        var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
        Console.WriteLine(" Done");
        
        if (fp1 != null && fp2 != null)
        {
            // Compare fingerprints
            int difference = VFPAnalyzer.Compare(
                fp1, 
                fp2, 
                TimeSpan.FromMilliseconds(500)
            );
            
            Console.WriteLine($"\nComparison Results:");
            Console.WriteLine($"  Difference Score: {difference}");
            
            // Interpret the results
            string interpretation = GetInterpretation(difference);
            Console.WriteLine($"  Interpretation: {interpretation}");
            
            // Provide detailed analysis
            if (difference < 100)
            {
                double similarity = Math.Max(0, 100 - (difference / 3.0));
                Console.WriteLine($"  Similarity: {similarity:F1}%");
            }
        }
        else
        {
            Console.WriteLine("Error: Failed to generate one or both fingerprints");
        }
    }
    
    static string GetInterpretation(int difference)
    {
        if (difference < 5)
            return "IDENTICAL - Same video, possibly different encoding";
        else if (difference < 15)
            return "NEARLY IDENTICAL - Same video with minor quality differences";
        else if (difference < 30)
            return "VERY SIMILAR - Same content with slight modifications";
        else if (difference < 50)
            return "SIMILAR - Same content with noticeable changes (watermark, logo, etc.)";
        else if (difference < 100)
            return "RELATED - Significant similarities, likely same source material";
        else if (difference < 300)
            return "SOMEWHAT RELATED - Some common scenes or content";
        else
            return "DIFFERENT - Completely different videos";
    }
}
```

## Common Pitfalls and Solutions

### Issue 1: DllNotFoundException

**Problem**: Application crashes with "Unable to load DLL 'VisioForge_VideoFingerprinting'"

**Solution**:

Add NuGet package `VisioForge.DotNet.Core.Redist.VideoFingerprinting` to your project.

### Issue 2: Out of Memory Exception

**Problem**: "System.OutOfMemoryException" when processing large videos

**Solutions**:

```csharp
// Solution 1: Use 64-bit process and increase memory
// Add to .csproj:
<PropertyGroup>
  <PlatformTarget>x64</PlatformTarget>
  <LargeAddressAware>true</LargeAddressAware>
</PropertyGroup>

// Solution 2: Reduce video resolution
var source = new VFPFingerprintSource(videoPath)
{
    CustomResolution = new VisioForge.Core.Types.Size(320, 240), // Very low resolution
    FrameRate = 5 // Process fewer frames per second
};
```

### Issue 3: Slow Processing Speed

**Problem**: Fingerprint generation takes too long

**Solutions**:

```csharp
// Solution 1: Use parallel processing for multiple videos
static async Task ProcessMultipleVideos(string[] videoPaths)
{
    var tasks = videoPaths.Select(path => Task.Run(async () =>
    {
        var source = new VFPFingerprintSource(path)
        {
            CustomResolution = new VisioForge.Core.Types.Size(640, 480)
        };
        
        return await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    }));
    
    var fingerprints = await Task.WhenAll(tasks);
}

// Solution 3: Cache fingerprints
class FingerprintCache
{
    private static Dictionary<string, VFPFingerPrint> cache = new();
    
    public static async Task<VFPFingerPrint> GetOrGenerate(string videoPath)
    {
        string cacheKey = GetCacheKey(videoPath);
        
        if (cache.ContainsKey(cacheKey))
            return cache[cacheKey];
        
        string cachePath = $"{videoPath}.vfp";
        
        if (File.Exists(cachePath))
        {
            var fp = VFPFingerPrint.Load(cachePath);
            cache[cacheKey] = fp;
            return fp;
        }
        
        // Generate new fingerprint
        var source = new VFPFingerprintSource(videoPath);
        var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
        
        if (fingerprint != null)
        {
            fingerprint.Save(cachePath);
            cache[cacheKey] = fingerprint;
        }
        
        return fingerprint;
    }
    
    private static string GetCacheKey(string path)
    {
        var info = new FileInfo(path);
        return $"{path}_{info.Length}_{info.LastWriteTimeUtc.Ticks}";
    }
}
```

### Issue 4: Incorrect Similarity Results

**Problem**: Videos that should match show as different

**Solutions**:

```csharp
// Solution 1: Adjust comparison parameters
static int CompareWithTolerance(VFPFingerPrint fp1, VFPFingerPrint fp2)
{
    // Try different time shifts
    int[] shifts = { 100, 500, 1000, 2000 }; // milliseconds
    int minDifference = int.MaxValue;
    
    foreach (int shift in shifts)
    {
        int diff = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(shift));
        minDifference = Math.Min(minDifference, diff);
    }
    
    return minDifference;
}

// Solution 2: Handle videos with different aspect ratios
var source = new VFPFingerprintSource(videoFilePath);
{
    // Ignore letterbox/pillarbox areas
    source.IgnoredAreas.AddRange(new[]
    {
        new Rect(0, 0, 1920, 140),      // Top letterbox
        new Rect(0, 940, 1920, 140)     // Bottom letterbox
    });
};
```

## Best Practices Summary

### Do's

- ✅ Always set license key before any SDK operations
- ✅ Use try-catch blocks around SDK calls
- ✅ Process videos at lower resolution for faster analysis
- ✅ Cache fingerprints to avoid reprocessing
- ✅ Use appropriate fingerprint type (Search vs Compare)
- ✅ Test with small video segments first
- ✅ Implement progress callbacks for user feedback
- ✅ Dispose of fingerprint objects when done

### Don'ts

- ❌ Don't ignore error callbacks
- ❌ Don't compare fingerprints of different types
- ❌ Don't process multiple large videos simultaneously without memory management

## Next Steps

Now that you have the SDK installed and working, explore these resources:

1. **[API Documentation](api.md)** - Complete reference for all classes and methods
2. **[Use Cases and Applications](../use-cases.md)** - Real-world implementation scenarios
3. **[Understanding the Technology](../understanding-video-fingerprinting.md)** - Deep technical dive

## Getting Help

### Resources

- **API Reference**: [https://api.visioforge.org/dotnet/](https://api.visioforge.org/dotnet/)
- **GitHub Samples**: [https://github.com/visioforge/.Net-SDK-s-samples/](https://github.com/visioforge/.Net-SDK-s-samples/)
- **Support Forum**: [https://support.visioforge.com/](https://support.visioforge.com/)
- **Discord Community**: [https://discord.com/invite/yvXUG56WCH](https://discord.com/invite/yvXUG56WCH)

### Common Questions

- **Q: Can I use the SDK in a web application?**
  A: Yes, the SDK can be used in ASP.NET Core applications for server-side processing.

- **Q: What video formats are supported?**
  A: MP4, AVI, MKV, MOV, WMV, FLV, WebM, and many more through GStreamer.

- **Q: How accurate is the fingerprinting?**
  A: Typically 95-99% accurate for content identification, depending on transformations.

- **Q: Can it detect videos with added watermarks?**
  A: Yes, the SDK can identify videos even with watermarks, logos, or subtitles added.

## Complete Working Example

Here's a comprehensive console application that demonstrates all basic operations:

```csharp
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.Types.X.Sources;

namespace VideoFingerprintingDemo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Initialize SDK
            VFPAnalyzer.SetLicenseKey("TRIAL");
            
            // Configure paths
            string videosDir = @"C:\Videos";
            string dbDir = @"C:\FingerprintDB";
            Directory.CreateDirectory(dbDir);
            
            var app = new FingerprintingApp(videosDir, dbDir);
            
            while (true)
            {
                Console.WriteLine("\n=== Video Fingerprinting Demo ===");
                Console.WriteLine("1. Generate fingerprint for a video");
                Console.WriteLine("2. Compare two videos");
                Console.WriteLine("3. Find fragment in video");
                Console.WriteLine("4. Build fingerprint database");
                Console.WriteLine("5. Search database for similar videos");
                Console.WriteLine("0. Exit");
                Console.Write("\nSelect option: ");
                
                var choice = Console.ReadLine();
                Console.WriteLine();
                
                try
                {
                    switch (choice)
                    {
                        case "1":
                            await app.GenerateFingerprint();
                            break;
                        case "2":
                            await app.CompareTwoVideos();
                            break;
                        case "3":
                            await app.FindFragment();
                            break;
                        case "4":
                            await app.BuildDatabase();
                            break;
                        case "5":
                            await app.SearchDatabase();
                            break;
                        case "0":
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
                
                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }
    }
    
    class FingerprintingApp
    {
        private string videosDir;
        private string dbDir;
        private Dictionary<string, VFPFingerPrint> database = new Dictionary<string, VFPFingerPrint>();
        
        public FingerprintingApp(string videosDir, string dbDir)
        {
            this.videosDir = videosDir;
            this.dbDir = dbDir;
            LoadDatabase();
        }
        
        public async Task GenerateFingerprint()
        {
            Console.Write("Enter video filename: ");
            string filename = Console.ReadLine();
            string videoPath = Path.Combine(videosDir, filename);
            
            if (!File.Exists(videoPath))
            {
                Console.WriteLine("File not found!");
                return;
            }
            
            var source = new VFPFingerprintSource(videoPath);
            
            Console.Write("Process full video? (y/n): ");
            if (Console.ReadLine().ToLower() != "y")
            {
                Console.Write("Enter duration in seconds: ");
                if (int.TryParse(Console.ReadLine(), out int seconds))
                {
                    source.StopTime = TimeSpan.FromSeconds(seconds);
                }
            }
            
            Console.WriteLine("Generating fingerprint...");
            var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                source,
                errorDelegate: (msg) => Console.WriteLine($"Error: {msg}"),
                progressDelegate: (p) => Console.Write($"\rProgress: {p}%")
            );
            
            if (fp != null)
            {
                string outputPath = Path.ChangeExtension(videoPath, ".vfp");
                fp.Save(outputPath);
                Console.WriteLine($"\n✓ Fingerprint saved to: {outputPath}");
                Console.WriteLine($"  Duration: {fp.Duration}");
                Console.WriteLine($"  Resolution: {fp.Width}x{fp.Height}");
                Console.WriteLine($"  Frame Rate: {fp.FrameRate:F2} fps");
            }
        }
        
        public async Task CompareTwoVideos()
        {
            Console.Write("Enter first video filename: ");
            string file1 = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Enter second video filename: ");
            string file2 = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(file1) || !File.Exists(file2))
            {
                Console.WriteLine("One or both files not found!");
                return;
            }
            
            Console.WriteLine("Generating fingerprints...");
            
            var source1 = new VFPFingerprintSource(file1);
            source1.StopTime = TimeSpan.FromSeconds(30); // Quick comparison
            
            var source2 = new VFPFingerprintSource(file2);
            source2.StopTime = TimeSpan.FromSeconds(30);
            
            var fp1 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source1);
            var fp2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source2);
            
            if (fp1 != null && fp2 != null)
            {
                int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromMilliseconds(500));
                
                Console.WriteLine($"\nDifference Score: {difference}");
                
                if (difference < 5)
                    Console.WriteLine("✓ Videos are IDENTICAL");
                else if (difference < 30)
                    Console.WriteLine("✓ Videos are VERY SIMILAR");
                else if (difference < 100)
                    Console.WriteLine("✓ Videos are SIMILAR");
                else if (difference < 300)
                    Console.WriteLine("⚠ Videos have SOME SIMILARITIES");
                else
                    Console.WriteLine("✗ Videos are DIFFERENT");
            }
        }
        
        public async Task FindFragment()
        {
            Console.Write("Enter fragment video filename: ");
            string fragmentFile = Path.Combine(videosDir, Console.ReadLine());
            
            Console.Write("Enter full video filename: ");
            string fullFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(fragmentFile) || !File.Exists(fullFile))
            {
                Console.WriteLine("One or both files not found!");
                return;
            }
            
            Console.WriteLine("Processing fragment...");
            var fragmentFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fragmentFile),
                progressDelegate: (p) => Console.Write($"\rFragment: {p}%")
            );
            
            Console.WriteLine("\nProcessing full video...");
            var fullFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(fullFile),
                progressDelegate: (p) => Console.Write($"\rFull video: {p}%")
            );
            
            if (fragmentFp != null && fullFp != null)
            {
                Console.WriteLine("\n\nSearching...");
                var positions = await VFPAnalyzer.SearchAsync(
                    fragmentFp, fullFp, fragmentFp.Duration, 50, true
                );
                
                if (positions.Count > 0)
                {
                    Console.WriteLine($"✓ Found {positions.Count} occurrence(s):");
                    foreach (var pos in positions)
                    {
                        Console.WriteLine($"  - At {pos:hh\\:mm\\:ss}");
                    }
                }
                else
                {
                    Console.WriteLine("✗ Fragment not found");
                }
            }
        }
        
        public async Task BuildDatabase()
        {
            var videoFiles = Directory.GetFiles(videosDir, "*.mp4")
                .Concat(Directory.GetFiles(videosDir, "*.avi"))
                .Concat(Directory.GetFiles(videosDir, "*.mkv"))
                .ToList();
            
            Console.WriteLine($"Found {videoFiles.Count} video files");
            
            int processed = 0;
            foreach (var videoFile in videoFiles)
            {
                string id = Path.GetFileNameWithoutExtension(videoFile);
                string fpPath = Path.Combine(dbDir, $"{id}.vfp");
                
                if (File.Exists(fpPath))
                {
                    Console.WriteLine($"Skipping {id} (already exists)");
                    continue;
                }
                
                Console.WriteLine($"Processing {id}...");
                
                var source = new VFPFingerprintSource(videoFile);
                source.StopTime = TimeSpan.FromSeconds(60); // First minute only
                
                var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                    source,
                    progressDelegate: (p) => Console.Write($"\r  Progress: {p}%")
                );
                
                if (fp != null)
                {
                    fp.ID = Guid.NewGuid();
                    fp.OriginalFilename = Path.GetFileName(videoFile);
                    fp.Save(fpPath);
                    processed++;
                    Console.WriteLine($"\r  ✓ Saved fingerprint for {id}");
                }
            }
            
            Console.WriteLine($"\n✓ Processed {processed} videos");
            LoadDatabase();
        }
        
        public async Task SearchDatabase()
        {
            Console.Write("Enter query video filename: ");
            string queryFile = Path.Combine(videosDir, Console.ReadLine());
            
            if (!File.Exists(queryFile))
            {
                Console.WriteLine("File not found!");
                return;
            }
            
            Console.Write("Enter similarity threshold (default 30): ");
            if (!int.TryParse(Console.ReadLine(), out int threshold))
                threshold = 30;
            
            Console.WriteLine("Generating query fingerprint...");
            var queryFp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
                new VFPFingerprintSource(queryFile) { StopTime = TimeSpan.FromSeconds(60) }
            );
            
            if (queryFp == null) return;
            
            Console.WriteLine($"Searching {database.Count} fingerprints...");
            
            var matches = new List<(string id, int score)>();
            
            foreach (var entry in database)
            {
                int score = VFPAnalyzer.Compare(queryFp, entry.Value, TimeSpan.FromMilliseconds(500));
                if (score < threshold)
                {
                    matches.Add((entry.Key, score));
                }
            }
            
            if (matches.Count > 0)
            {
                Console.WriteLine($"\n✓ Found {matches.Count} similar video(s):");
                foreach (var match in matches.OrderBy(m => m.score))
                {
                    var fp = database[match.id];
                    Console.WriteLine($"  - {fp.OriginalFilename} (score: {match.score})");
                }
            }
            else
            {
                Console.WriteLine("\n✗ No similar videos found");
            }
        }
        
        private void LoadDatabase()
        {
            database.Clear();
            
            if (!Directory.Exists(dbDir))
                return;
            
            var files = Directory.GetFiles(dbDir, "*.vfp");
            foreach (var file in files)
            {
                try
                {
                    var fp = VFPFingerPrint.Load(file);
                    string id = Path.GetFileNameWithoutExtension(file);
                    database[id] = fp;
                }
                catch { }
            }
            
            Console.WriteLine($"Loaded {database.Count} fingerprints from database");
        }
    }
}
```

## Performance Benchmarks

| Operation | Duration | File Size | Processing Time | Memory Usage |
|-----------|----------|-----------|----------------|------------|
| Generate fingerprint | 1 minute | 100 MB | ~5 seconds | 200 MB |
| Generate fingerprint | 10 minutes | 1 GB | ~45 seconds | 400 MB |
| Compare fingerprints | N/A | N/A | <1 ms | Minimal |
| Search fragment | 30 sec in 1 hour | N/A | ~100 ms | 100 MB |
| Database query | N/A | 1000 videos | ~50 ms | 250 MB |

## Summary

You've now learned how to:

- ✅ Install and configure the Video Fingerprinting SDK with proper NuGet packages
- ✅ Generate fingerprints from video files
- ✅ Compare videos for similarity
- ✅ Search for fragments within videos
- ✅ Build and query a fingerprint database
- ✅ Handle common issues and optimize performance

The Video Fingerprinting SDK provides a powerful foundation for content identification, duplicate detection, and media monitoring applications. Start with the simple examples and gradually incorporate more advanced features as your needs grow.

Congratulations! You're now ready to build powerful video fingerprinting applications with the VisioForge SDK.
