---
title: Video Fingerprinting SDK .NET API Documentation
description: Complete API documentation for VisioForge Video Fingerprinting SDK to generate, compare, and search video fingerprints with code examples.
---

# Video Fingerprinting SDK .NET API Documentation

## Overview

The VisioForge Video Fingerprinting namespace provides powerful functionality for video content identification, comparison, and search operations. It enables applications to:

- Generate unique fingerprints from video files for content identification
- Compare videos to determine similarity and detect duplicates
- Search for video fragments within larger videos (e.g., finding commercials, intros, or specific scenes)
- Compare individual images for similarity detection (Windows only)
- Process video frames directly to generate fingerprints from streams or generated content

## Table of Contents

- [VFPAnalyzer Class](#vfpanalyzer-class)
- [VFPFingerPrint Class](#vfpfingerprint-class)
- [VFPFingerprintSource Class](#vfpfingerprintsource-class)
- [VFPCompare Class](#vfpcompare-class)
- [VFPSearch Class](#vfpsearch-class)
- [VFPFingerPrintDB Class](#vfpfingerprintdb-class)
- [VFPFingerprintFromFrames Class](#vfpfingerprintfromframes-class)
- [Supporting Types](#supporting-types)
- [Delegates](#delegates)

## VFPAnalyzer Class

The main entry point for video fingerprinting operations, providing high-level static methods for analysis, comparison, and search.

### Properties

#### DebugDir

```csharp
public static string DebugDir { get; set; }
```

**Description:** Directory path for debug output. When set, intermediate processing results may be saved for troubleshooting.

**Default:** `null` (debug output disabled)

**Example:**

```csharp
// Enable debug output
VFPAnalyzer.DebugDir = @"C:\Temp\VFP_Debug";

// Disable debug output
VFPAnalyzer.DebugDir = null;
```

### Methods

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string vfpLicense)
```

**Description:** Sets the license key for Video Fingerprinting SDK. Must be called before using any fingerprinting features.

**Parameters:**

- `vfpLicense` (string): Your VisioForge license key

**Example:**

```csharp
// Set license key at application startup
VFPAnalyzer.SetLicenseKey("YOUR-LICENSE-KEY-HERE");
```

#### GetComparingFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetComparingFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Description:** Generates a fingerprint optimized for whole-video comparison operations.

**Parameters:**

- `source` (VFPFingerprintSource): Video source configuration including file path, time range, and processing options
- `errorDelegate` (VFPErrorCallback): Optional callback for error messages
- `progressDelegate` (VFPProgressCallback): Optional callback for progress updates (0-100)

**Returns:** `Task<VFPFingerPrint>` - Generated fingerprint or `null` if an error occurred

**Use Case:** Comparing entire videos or large segments to determine overall similarity

**Example:**

```csharp
// Basic usage
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");
var fingerprint = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);

if (fingerprint != null)
{
    fingerprint.Save(@"C:\Fingerprints\movie.vsigx");
    Console.WriteLine($"Fingerprint created for {fingerprint.Duration} duration");
}

// With error handling and progress reporting
var source2 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    StartTime = TimeSpan.FromMinutes(5),
    StopTime = TimeSpan.FromMinutes(10)
};

var fingerprint2 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(
    source2,
    error => Console.WriteLine($"Error: {error}"),
    progress => Console.WriteLine($"Progress: {progress}%"));

// With ignored areas (e.g., logos, watermarks)
var source3 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source3.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Top-right logo
source3.IgnoredAreas.Add(new Rect(100, 950, 300, 1000)); // Bottom timestamp

var fingerprint3 = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source3);
```

#### GetSearchFingerprintForVideoFileAsync

```csharp
public static async Task<VFPFingerPrint> GetSearchFingerprintForVideoFileAsync(
    VFPFingerprintSource source,
    VFPErrorCallback errorDelegate = null,
    VFPProgressCallback progressDelegate = null)
```

**Description:** Generates a fingerprint optimized for fragment search operations.

**Parameters:**

- `source` (VFPFingerprintSource): Video source configuration
- `errorDelegate` (VFPErrorCallback): Optional error callback
- `progressDelegate` (VFPProgressCallback): Optional progress callback

**Returns:** `Task<VFPFingerPrint>` - Generated fingerprint or `null` if an error occurred

**Use Case:** Creating fingerprints of short clips to locate within full-length videos

**Example:**

```csharp
// Create fingerprint for a commercial
var commercialSource = new VFPFingerprintSource(@"C:\Videos\commercial.mp4");
var commercialFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
    commercialSource,
    error => Console.WriteLine($"Error: {error}"),
    progress => Console.WriteLine($"Processing: {progress}%"));

// Create fingerprint for a specific scene
var sceneSource = new VFPFingerprintSource(@"C:\Videos\movie.mp4")
{
    StartTime = TimeSpan.FromMinutes(42),
    StopTime = TimeSpan.FromMinutes(43)
};

var sceneFp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(sceneSource);

if (sceneFp != null)
{
    sceneFp.Tag = "Action scene at bridge";
    sceneFp.Save(@"C:\Fingerprints\scene.vsigx");
}
```

#### Compare

```csharp
public static int Compare(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan shift)
```

**Description:** Compares two video fingerprints to determine similarity.

**Parameters:**

- `fp1` (VFPFingerPrint): First fingerprint
- `fp2` (VFPFingerPrint): Second fingerprint
- `shift` (TimeSpan): Maximum time shift allowed during comparison

**Returns:** `int` - Difference score (lower = more similar), or `Int32.MaxValue` if either fingerprint is null

**Example:**

```csharp
// Load two fingerprints
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

// Compare with 5-second shift tolerance
int difference = VFPAnalyzer.Compare(fp1, fp2, TimeSpan.FromSeconds(5));

// Interpret results
if (difference < 5)
{
    Console.WriteLine("Videos are nearly identical");
}
else if (difference < 15)
{
    Console.WriteLine("Videos are very similar");
}
else if (difference < 30)
{
    Console.WriteLine("Videos have similar content with differences");
}
else if (difference < 100)
{
    Console.WriteLine("Videos are related but significantly different");
}
else
{
    Console.WriteLine("Videos are completely different");
}

// Batch comparison
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    fingerprints.Add(VFPFingerPrint.Load(file));
}

var referenceFp = fingerprints[0];
foreach (var fp in fingerprints.Skip(1))
{
    int diff = VFPAnalyzer.Compare(referenceFp, fp, TimeSpan.FromSeconds(3));
    Console.WriteLine($"{fp.OriginalFilename}: Difference = {diff}");
}
```

#### Search / SearchAsync

```csharp
public static List<TimeSpan> Search(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan duration,
    int maxDifference,
    bool allowMultipleFragments)

public static Task<List<TimeSpan>> SearchAsync(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    TimeSpan duration,
    int maxDifference,
    bool allowMultipleFragments)
```

**Description:** Searches for occurrences of a video fragment within a larger video.

**Parameters:**

- `fp1` (VFPFingerPrint): Fragment fingerprint (needle)
- `fp2` (VFPFingerPrint): Video to search within (haystack)
- `duration` (TimeSpan): Fragment duration (prevents overlapping matches)
- `maxDifference` (int): Maximum allowed difference (typical: 5-20)
- `allowMultipleFragments` (bool): Find all occurrences vs. first match only

**Returns:** `List<TimeSpan>` - Timestamps where matches were found

**Example:**

```csharp
// Search for a commercial in a recording
var commercialFp = VFPFingerPrint.Load(@"C:\Fingerprints\commercial.vsigx");
var recordingFp = VFPFingerPrint.Load(@"C:\Fingerprints\tv_recording.vsigx");

// Find all occurrences
var matches = await VFPAnalyzer.SearchAsync(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30), // Commercial duration
    maxDifference: 10,
    allowMultipleFragments: true);

foreach (var timestamp in matches)
{
    Console.WriteLine($"Commercial found at: {timestamp:hh\\:mm\\:ss}");
}

// Find first occurrence only
var firstMatch = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 15,
    allowMultipleFragments: false);

if (firstMatch.Any())
{
    Console.WriteLine($"First occurrence at: {firstMatch[0]}");
}

// Search with stricter matching
var exactMatches = VFPAnalyzer.Search(
    commercialFp,
    recordingFp,
    TimeSpan.FromSeconds(30),
    maxDifference: 5, // Very strict
    allowMultipleFragments: true);
```

#### CompareVideoFilesAsync

```csharp
public static async Task<bool> CompareVideoFilesAsync(
    VFPFingerprintSource file1,
    VFPFingerprintSource file2,
    TimeSpan shift,
    VFPErrorCallback errorCallback,
    int threshold = 500)
```

**Description:** Convenience method that generates fingerprints and compares two video files in one operation.

**Parameters:**

- `file1` (VFPFingerprintSource): First video configuration
- `file2` (VFPFingerprintSource): Second video configuration
- `shift` (TimeSpan): Maximum time shift allowed
- `errorCallback` (VFPErrorCallback): Error callback
- `threshold` (int): Maximum difference to consider as matching (default: 500)

**Returns:** `Task<bool>` - `true` if videos match (difference < threshold), otherwise `false`

**Example:**

```csharp
// Compare two video files directly
var file1 = new VFPFingerprintSource(@"C:\Videos\original.mp4");
var file2 = new VFPFingerprintSource(@"C:\Videos\copy.mp4");

bool areIdentical = await VFPAnalyzer.CompareVideoFilesAsync(
    file1,
    file2,
    TimeSpan.FromSeconds(5),
    error => Console.WriteLine($"Error: {error}"),
    threshold: 20);

if (areIdentical)
{
    Console.WriteLine("Videos are identical or very similar");
}

// Compare with custom processing
var source1 = new VFPFingerprintSource(@"C:\Videos\video1.mp4")
{
    CustomResolution = new Size(640, 480),
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromMinutes(5)
};

var source2 = new VFPFingerprintSource(@"C:\Videos\video2.mp4")
{
    CustomResolution = new Size(640, 480),
    StartTime = TimeSpan.FromSeconds(10),
    StopTime = TimeSpan.FromMinutes(5)
};

bool match = await VFPAnalyzer.CompareVideoFilesAsync(
    source1,
    source2,
    TimeSpan.FromSeconds(3),
    null,
    threshold: 50);
```

## VFPFingerPrint Class

Represents a video fingerprint with metadata and serialization support.

### Properties

```csharp
public byte[] Data { get; set; }
public TimeSpan Duration { get; set; }
public Guid ID { get; set; }
public string OriginalFilename { get; set; }
public TimeSpan OriginalDuration { get; set; }
public string Tag { get; set; }
public int Width { get; set; }
public int Height { get; set; }
public double FrameRate { get; set; }
public List<Rect> IgnoredAreas { get; set; }
```

### Methods

#### Load (Static)

```csharp
public static VFPFingerPrint Load(string filename)
public static VFPFingerPrint Load(byte[] data)
```

**Description:** Loads a fingerprint from file or memory.

**Parameters:**

- `filename` (string): Path to fingerprint file
- `data` (byte[]): Fingerprint data in memory

**Returns:** `VFPFingerPrint` - Loaded fingerprint object

**Example:**

```csharp
// Load from file
var fingerprint = VFPFingerPrint.Load(@"C:\Fingerprints\video.vsigx");
Console.WriteLine($"Loaded: {fingerprint.OriginalFilename}");
Console.WriteLine($"Duration: {fingerprint.Duration}");
Console.WriteLine($"ID: {fingerprint.ID}");

// Load from memory
byte[] fpData = File.ReadAllBytes(@"C:\Fingerprints\video.vsigx");
var fingerprint2 = VFPFingerPrint.Load(fpData);

// Load multiple fingerprints
var fingerprints = new List<VFPFingerPrint>();
foreach (var file in Directory.GetFiles(@"C:\Fingerprints", "*.vsigx"))
{
    try
    {
        var fp = VFPFingerPrint.Load(file);
        fingerprints.Add(fp);
        Console.WriteLine($"Loaded: {fp.OriginalFilename} ({fp.Duration})");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Failed to load {file}: {ex.Message}");
    }
}
```

#### Save

```csharp
public void Save(string filename)
public byte[] Save()
```

**Description:** Saves fingerprint to file or memory. Default extension: `.vsigx`

**Parameters:**

- `filename` (string): Output file path

**Returns:** `byte[]` - Serialized fingerprint data (memory version)

**Example:**

```csharp
// Save to file
fingerprint.Save(@"C:\Fingerprints\output.vsigx");

// Save to memory
byte[] data = fingerprint.Save();
File.WriteAllBytes(@"C:\Backup\fingerprint.vsigx", data);

// Save with metadata
fingerprint.Tag = "Important scene at 00:42:00";
fingerprint.Save(@"C:\Fingerprints\tagged.vsigx");

// Batch save with organized naming
foreach (var fp in fingerprints)
{
    string safeName = Path.GetFileNameWithoutExtension(fp.OriginalFilename)
        .Replace(" ", "_")
        .Replace(".", "_");
    string outputPath = Path.Combine(
        @"C:\Fingerprints",
        $"{safeName}_{fp.ID}.vsigx");
    fp.Save(outputPath);
}
```

## VFPFingerprintSource Class

Configuration for video fingerprinting operations.

### Constructor

```csharp
public VFPFingerprintSource(string filename)
```

**Description:** Creates a new source configuration.

**Parameters:**

- `filename` (string): Path to video file

**Throws:** `FileNotFoundException` if file doesn't exist

### Properties

```csharp
public string Filename { get; }
public TimeSpan StartTime { get; set; }
public TimeSpan StopTime { get; set; }
public Rect CustomCropSize { get; set; }
public Size CustomResolution { get; set; }
public List<Rect> IgnoredAreas { get; }
public TimeSpan OriginalDuration { get; set; }
```

- **`Filename`** (`string`): Path to source video file
- **`StartTime`** (`TimeSpan`): Start time for fingerprinting (default: 0)
- **`StopTime`** (`TimeSpan`): Stop time for fingerprinting (default: video duration)
- **`CustomCropSize`** (`Rect`): Crop rectangle (Left, Top, Right, Bottom distances)
- **`CustomResolution`** (`Size`): Target resolution (Empty = no resize)
- **`IgnoredAreas`** (`List<Rect>`): Regions to exclude (e.g., logos, timestamps)
- **`OriginalDuration`** (`TimeSpan`): Total video duration (auto-populated)

### Examples

```csharp
// Basic configuration
var source = new VFPFingerprintSource(@"C:\Videos\movie.mp4");

// Process specific time range
var source2 = new VFPFingerprintSource(@"C:\Videos\long_video.mp4")
{
    StartTime = TimeSpan.FromMinutes(10),
    StopTime = TimeSpan.FromMinutes(20)
};

// Crop to region of interest
var source3 = new VFPFingerprintSource(@"C:\Videos\video.mp4")
{
    CustomCropSize = new Rect(100, 100, 1820, 980) // Remove borders
};

// Resize for faster processing
var source4 = new VFPFingerprintSource(@"C:\Videos\4k_video.mp4")
{
    CustomResolution = new Size(1280, 720) // Downscale from 4K
};

// Ignore overlays and logos
var source5 = new VFPFingerprintSource(@"C:\Videos\broadcast.mp4");
source5.IgnoredAreas.Add(new Rect(1700, 50, 1870, 150)); // Channel logo
source5.IgnoredAreas.Add(new Rect(50, 50, 250, 100)); // Network bug
source5.IgnoredAreas.Add(new Rect(100, 950, 400, 1000)); // Ticker

// Combined processing options
var source6 = new VFPFingerprintSource(@"C:\Videos\tv_show.mp4")
{
    StartTime = TimeSpan.FromSeconds(90), // Skip intro
    StopTime = TimeSpan.FromMinutes(42), // Before credits
    CustomResolution = new Size(640, 480),
    CustomCropSize = new Rect(60, 0, 1860, 1080) // Remove pillarboxing
};
source6.IgnoredAreas.Add(new Rect(1600, 100, 1800, 200));
```

## VFPCompare Class

Low-level fingerprint comparison functionality.

### Methods

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Description:** Sets SDK license key.

**Example:**

```csharp
VFPCompare.SetLicenseKey("YOUR-LICENSE-KEY");
```

#### Process

```csharp
public static int Process(
    IntPtr ptr,
    int w,
    int h,
    int s,
    TimeSpan dTime,
    ref VFPCompareData data)
```

**Description:** Processes RGB24 frame for comparison fingerprint.

**Parameters:**

- `ptr` (IntPtr): Pointer to RGB24 frame data
- `w` (int): Frame width
- `h` (int): Frame height
- `s` (int): Stride (bytes per row)
- `dTime` (TimeSpan): Frame timestamp
- `data` (ref VFPCompareData): Comparison data structure

**Returns:** `int` - Status code (0 = success)

**Example:**

```csharp
// Initialize comparison data
var compareData = new VFPCompareData(durationInSeconds: 120);

// Process frames (usually done internally by VFPAnalyzer)
IntPtr frameData = GetRGB24Frame(); // Your frame source
int result = VFPCompare.Process(
    frameData,
    1920, // width
    1080, // height
    5760, // stride for 1920x3 RGB24
    TimeSpan.FromSeconds(1.5),
    ref compareData);

if (result == 0)
{
    Console.WriteLine("Frame processed successfully");
}

// Build fingerprint after processing all frames
IntPtr fpData = VFPCompare.Build(out long length, ref compareData);

// Clean up
compareData.Free();
```

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPCompareData video)
```

**Description:** Builds fingerprint from processed frames.

**Parameters:**

- `length` (out long): Size of fingerprint data
- `video` (ref VFPCompareData): Processed frame data

**Returns:** `IntPtr` - Pointer to fingerprint data

#### Compare

```csharp
public static double Compare(
    VFPFingerPrint fp1,
    VFPFingerPrint fp2,
    int maxDifference)
```

**Description:** Compares two fingerprints.

**Parameters:**

- `fp1` (VFPFingerPrint): First fingerprint
- `fp2` (VFPFingerPrint): Second fingerprint
- `maxDifference` (int): Maximum allowed difference

**Returns:** `double` - Similarity score (0-100, higher = more similar)

**Example:**

```csharp
var fp1 = VFPFingerPrint.Load(@"C:\Fingerprints\video1.vsigx");
var fp2 = VFPFingerPrint.Load(@"C:\Fingerprints\video2.vsigx");

double similarity = VFPCompare.Compare(fp1, fp2, maxDifference: 50);
Console.WriteLine($"Similarity: {similarity:F2}%");

if (similarity > 90)
{
    Console.WriteLine("Videos are very similar");
}
else if (similarity > 70)
{
    Console.WriteLine("Videos have significant similarities");
}
else
{
    Console.WriteLine("Videos are different");
}
```

## VFPSearch Class

Low-level fingerprint search functionality.

### Methods

#### SetLicenseKey

```csharp
public static void SetLicenseKey(string licenseKey)
```

**Description:** Sets SDK license key.

#### Process

```csharp
public static int Process(
    IntPtr ptr,
    int w,
    int h,
    int s,
    TimeSpan dTime,
    ref VFPSearchData data)
```

**Description:** Processes video frame for search fingerprint.

**Parameters:**

- `ptr` (IntPtr): RGB24 frame data pointer
- `w` (int): Frame width
- `h` (int): Frame height
- `s` (int): Stride
- `dTime` (TimeSpan): Frame timestamp
- `data` (ref VFPSearchData): Search data structure

**Returns:** `int` - Status code

#### Build

```csharp
public static IntPtr Build(
    out long length,
    ref VFPSearchData data)
```

**Description:** Builds search fingerprint.

**Parameters:**

- `length` (out long): Fingerprint data size
- `data` (ref VFPSearchData): Processed frames

**Returns:** `IntPtr` - Fingerprint data pointer

#### Search

```csharp
public static int Search(
    VFPFingerPrint fp1,
    int startPos1,
    VFPFingerPrint fp2,
    int startPos2,
    out int difference,
    int maxDifference)
```

**Description:** Searches for fragment in video.

**Parameters:**

- `fp1` (VFPFingerPrint): Fragment fingerprint
- `startPos1` (int): Start position in fragment (seconds)
- `fp2` (VFPFingerPrint): Main video fingerprint
- `startPos2` (int): Start search position (seconds)
- `difference` (out int): Match difference score
- `maxDifference` (int): Maximum allowed difference

**Returns:** `int` - Position where found (seconds) or Int32.MaxValue if not found

**Example:**

```csharp
// Search for fragment
var fragmentFp = VFPFingerPrint.Load(@"C:\Fingerprints\fragment.vsigx");
var videoFp = VFPFingerPrint.Load(@"C:\Fingerprints\full_video.vsigx");

int position = VFPSearch.Search(
    fragmentFp,
    startPos1: 0,
    videoFp,
    startPos2: 0,
    out int matchDifference,
    maxDifference: 20);

if (position != Int32.MaxValue)
{
    Console.WriteLine($"Fragment found at {position} seconds");
    Console.WriteLine($"Match quality: {matchDifference}");
}
else
{
    Console.WriteLine("Fragment not found");
}

// Search from specific position
int nextPosition = VFPSearch.Search(
    fragmentFp,
    0,
    videoFp,
    position + 30, // Skip past first match
    out matchDifference,
    maxDifference: 20);
```

## VFPFingerPrintDB Class

Database for managing collections of fingerprints.

### Properties

```csharp
public List<VFPFingerPrint> Items { get; }
```

### Methods

#### Save

```csharp
public void Save(string filename)
```

**Description:** Saves database to file.

**Example:**

```csharp
var db = new VFPFingerPrintDB();

// Add fingerprints
foreach (var videoFile in Directory.GetFiles(@"C:\Videos", "*.mp4"))
{
    var source = new VFPFingerprintSource(videoFile);
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    if (fp != null)
    {
        db.Items.Add(fp);
    }
}

// Save database
db.Save(@"C:\Database\fingerprints.db");
Console.WriteLine($"Saved {db.Items.Count} fingerprints to database");
```

#### Load (Static)

```csharp
public static VFPFingerPrintDB Load(string filename)
```

**Description:** Loads database from file.

**Example:**

```csharp
// Load existing database
var db = VFPFingerPrintDB.Load(@"C:\Database\fingerprints.db");
Console.WriteLine($"Loaded {db.Items.Count} fingerprints");

// Query database
var recentVideos = db.Items
    .Where(fp => fp.OriginalDuration > TimeSpan.FromMinutes(30))
    .OrderBy(fp => fp.OriginalFilename)
    .ToList();

foreach (var fp in recentVideos)
{
    Console.WriteLine($"{fp.OriginalFilename}: {fp.Duration}");
}
```

#### ContainsFile

```csharp
public bool ContainsFile(VFPFingerprintSource source)
```

**Description:** Checks if database contains fingerprint for source.

**Example:**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\new_video.mp4");

if (!db.ContainsFile(source))
{
    // Generate and add new fingerprint
    var fp = await VFPAnalyzer.GetComparingFingerprintForVideoFileAsync(source);
    db.Items.Add(fp);
    db.Save(@"C:\Database\fingerprints.db");
}
else
{
    Console.WriteLine("Fingerprint already exists in database");
}
```

#### GetFingerprint

```csharp
public VFPFingerPrint GetFingerprint(VFPFingerprintSource source)
```

**Description:** Retrieves fingerprint matching source.

**Example:**

```csharp
var source = new VFPFingerprintSource(@"C:\Videos\video.mp4");
var fp = db.GetFingerprint(source);

if (fp != null)
{
    Console.WriteLine($"Found fingerprint: {fp.ID}");
    Console.WriteLine($"Duration: {fp.Duration}");
    Console.WriteLine($"Tag: {fp.Tag}");
}
```

## VFPFingerprintFromFrames Class

Creates fingerprints from individual image frames. Available on all platforms.

### Constructor

```csharp
public VFPFingerprintFromFrames(
    double frameRate,
    int width,
    int height,
    TimeSpan totalDuration)
```

**Description:** Initializes frame-based fingerprint builder.

**Parameters:**

- `frameRate` (double): Video frame rate
- `width` (int): Frame width
- `height` (int): Frame height
- `totalDuration` (TimeSpan): Total video duration

### Methods

#### Push

```csharp
public void Push(byte[] rgb24frame)           // All platforms
public void Push(Bitmap frame)                // Windows only
public void Push(SKBitmap frame)              // All platforms (new)
public void Push(IntPtr rgb24frame, int rgb24frameSize)  // All platforms
```

**Description:** Adds frames to the fingerprint generation process. Frames must match configured dimensions.

- **byte[]**: Raw RGB24 frame data (cross-platform)
- **Bitmap**: System.Drawing.Bitmap (Windows only)
- **SKBitmap**: SkiaSharp bitmap for cross-platform support
- **IntPtr**: Pointer to RGB24 frame data (cross-platform)

**Example:**

```csharp
// Create builder
var builder = new VFPFingerprintFromFrames(
    frameRate: 30.0,
    width: 1920,
    height: 1080,
    totalDuration: TimeSpan.FromMinutes(5));

// Cross-platform: Add frames as byte arrays
foreach (var frameData in videoStream.GetFrames())
{
    builder.Push(frameData); // byte[] RGB24
}

// Cross-platform: Add SkiaSharp bitmaps
using (var skBitmap = SKBitmap.Decode(imageData))
{
    builder.Push(skBitmap);
}

// Windows-only: Add System.Drawing.Bitmap frames
#if NET_WINDOWS
for (int i = 0; i < frameCount; i++)
{
    Bitmap frame = GetFrameAsBitmap(i);
    builder.Push(frame);
}
#endif

// Cross-platform: Add frames via IntPtr
unsafe
{
    fixed (byte* ptr = frameData)
    {
        builder.Push(new IntPtr(ptr), frameData.Length);
    }
}

// Build final fingerprint
var fingerprint = builder.Build();
fingerprint.OriginalFilename = "stream_capture.mp4";
fingerprint.Save(@"C:\Fingerprints\stream.vsigx");
```

#### Build

```csharp
public VFPFingerPrint Build()
```

**Description:** Generates fingerprint from processed frames.

**Returns:** `VFPFingerPrint` - Generated fingerprint

## Supporting Types

### VFPCompareData

```csharp
public struct VFPCompareData
{
    public IntPtr Data { get; set; }
    public VFPCompareData(int duration)
    public void Free()
}
```

**Description:** Manages native comparison data.

**Example:**

```csharp
// Create and use comparison data
var data = new VFPCompareData(durationInSeconds: 60);
try
{
    // Process frames...
    // Build fingerprint...
}
finally
{
    data.Free(); // Always free native memory
}
```

### VFPSearchData

```csharp
public class VFPSearchData : IDisposable
{
    public IntPtr Data { get; set; }
    public VFPSearchData(TimeSpan duration)
    public void Free()
    public void Dispose()
}
```

**Description:** Manages native search data with automatic disposal.

**Example:**

```csharp
// Using statement ensures proper cleanup
using (var searchData = new VFPSearchData(TimeSpan.FromMinutes(2)))
{
    // Process frames for search fingerprint
    // Build fingerprint
} // Automatically disposed
```

## Delegates

### VFPProgressCallback

```csharp
public delegate void VFPProgressCallback(int percent)
```

**Description:** Reports progress during fingerprinting operations (0-100).

**Example:**

```csharp
// Simple progress display
VFPProgressCallback progressCallback = (percent) =>
{
    Console.Write($"\rProgress: {percent}%");
    if (percent == 100) Console.WriteLine();
};

// Progress with UI update
VFPProgressCallback uiProgress = (percent) =>
{
    progressBar.Value = percent;
    labelStatus.Text = $"Processing: {percent}%";
    Application.DoEvents();
};

// Progress with cancellation check
CancellationToken token = GetCancellationToken();
VFPProgressCallback cancellableProgress = (percent) =>
{
    if (token.IsCancellationRequested)
        throw new OperationCanceledException();
    UpdateProgress(percent);
};
```

### VFPErrorCallback

```csharp
public delegate void VFPErrorCallback(string error)
```

**Description:** Reports errors during fingerprinting operations.

**Example:**

```csharp
// Log errors
VFPErrorCallback errorCallback = (error) =>
{
    logger.Error($"VFP Error: {error}");
    File.AppendAllText(@"C:\Logs\vfp_errors.log", 
        $"{DateTime.Now}: {error}{Environment.NewLine}");
};

// Display errors to user
VFPErrorCallback userErrorCallback = (error) =>
{
    MessageBox.Show($"Error processing video: {error}", 
        "Fingerprinting Error", 
        MessageBoxButtons.OK, 
        MessageBoxIcon.Error);
};

// Collect errors for batch processing
var errors = new List<string>();
VFPErrorCallback collectErrors = (error) => errors.Add(error);
```
