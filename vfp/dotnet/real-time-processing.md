---
title: Real-Time Video Fingerprinting Guide
description: Process live video streams and generate fingerprints in real-time using fragment-based processing with the VisioForge Video Fingerprinting SDK.
---

# Real-Time Video Fingerprinting Guide

This guide demonstrates how to process live video streams and generate fingerprints in real-time using the VisioForge Video Fingerprinting SDK. The approach uses fragment-based processing to build fingerprints from continuous video streams.

## Overview

Real-time fingerprinting works by:
1. Capturing frames from a live source (camera, IP stream, etc.)
2. Accumulating frames into time-based fragments
3. Building fingerprints from completed fragments
4. Searching for matches against reference fingerprints
5. Using overlapping fragments for better detection accuracy

## Core Components

### VFPSearch Class

The `VFPSearch` class provides static methods for real-time frame processing:

- `Process()` - Processes individual frames and adds them to search data
- `Build()` - Builds a fingerprint from accumulated search data

### VFPSearchData Class

Container for accumulating frame data during real-time processing:

```csharp
// Create search data container for a specific duration
var searchData = new VFPSearchData(TimeSpan.FromSeconds(10));
```

## Complete Example: Real-time Fingerprint Processing

Here's a complete example based on actual production code that processes live video and detects content matches:

```csharp
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using VisioForge.Core.VideoFingerPrinting;
using VisioForge.Core.VideoCaptureX;
using VisioForge.Core.Types.Events;

public class RealTimeFingerprintProcessor
{
    // Fragment-based live data container
    public class FingerprintLiveData : IDisposable
    {
        public VFPSearchData Data { get; private set; }
        public DateTime StartTime { get; private set; }

        public FingerprintLiveData(TimeSpan duration, DateTime startTime)
        {
            Data = new VFPSearchData(duration);
            StartTime = startTime;
        }

        public void Dispose()
        {
            Data?.Dispose();
        }
    }

    // Main processor class
    private FingerprintLiveData _searchLiveData;
    private FingerprintLiveData _searchLiveOverlapData;
    private ConcurrentQueue<FingerprintLiveData> _fingerprintQueue;
    private List<VFPFingerPrint> _referenceFingerprints;
    private VideoCaptureCoreX _videoCapture;
    
    private IntPtr _tempBuffer;
    private long _fragmentDuration;
    private int _fragmentCount;
    private int _overlapFragmentCount;
    private readonly object _processingLock = new object();

    public async Task InitializeAsync()
    {
        // Initialize video capture
        _videoCapture = new VideoCaptureCoreX();
        _videoCapture.OnVideoFrameBuffer += OnVideoFrameReceived;
        
        // Initialize processing queue
        _fingerprintQueue = new ConcurrentQueue<FingerprintLiveData>();
        
        // Load reference fingerprints (ads, copyrighted content, etc.)
        _referenceFingerprints = await LoadReferenceFingerprintsAsync();
        
        // Calculate optimal fragment duration
        // Should be at least 2x the longest reference content duration
        var maxReferenceDuration = GetMaxReferenceDuration();
        _fragmentDuration = ((maxReferenceDuration + 1000) / 1000 + 1) * 1000 * 2;
    }

    private async Task<List<VFPFingerPrint>> LoadReferenceFingerprintsAsync()
    {
        var fingerprints = new List<VFPFingerPrint>();
        
        // Load or generate reference fingerprints
        foreach (var videoFile in GetReferenceVideos())
        {
            VFPFingerPrint fp;
            
            // Check if fingerprint already exists
            var fpFile = videoFile + ".vfsigx";
            if (File.Exists(fpFile))
            {
                fp = VFPFingerPrint.Load(fpFile);
            }
            else
            {
                // Generate and save fingerprint
                var source = new VFPFingerprintSource(videoFile);
                fp = await VFPAnalyzer.GetSearchFingerprintForVideoFileAsync(
                    source, 
                    progressCallback: null,
                    cancellationToken: default);
                
                fp.Save(fpFile);
            }
            
            fingerprints.Add(fp);
        }
        
        return fingerprints;
    }

    private void OnVideoFrameReceived(object sender, VideoFrameXBufferEventArgs e)
    {
        // Allocate temporary buffer if needed
        if (_tempBuffer == IntPtr.Zero)
        {
            var stride = ((e.Frame.Width * 3 + 3) / 4) * 4; // RGB24 stride
            _tempBuffer = Marshal.AllocCoTaskMem(stride * e.Frame.Height);
        }

        // Process main fragment
        ProcessMainFragment(e);
        
        // Process overlapping fragment for better detection
        ProcessOverlappingFragment(e);
    }

    private void ProcessMainFragment(VideoFrameXBufferEventArgs e)
    {
        // Initialize new fragment if needed
        if (_searchLiveData == null)
        {
            _searchLiveData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration), 
                DateTime.Now);
            _fragmentCount++;
        }

        // Calculate timestamp relative to fragment start
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Check if we're still within current fragment duration
        if (timestamp < _fragmentDuration * _fragmentCount)
        {
            // Copy frame data to temporary buffer
            CopyMemory(_tempBuffer, e.Frame.Data, e.Frame.DataSize);
            
            // Calculate corrected timestamp (relative to fragment start)
            var correctedTimestamp = timestamp - _fragmentDuration * (_fragmentCount - 1);
            
            // Process frame and add to search data
            VFPSearch.Process(
                _tempBuffer,                                    // Frame data
                e.Frame.Width,                                   // Width
                e.Frame.Height,                                  // Height
                ((e.Frame.Width * 3 + 3) / 4) * 4,             // Stride
                TimeSpan.FromMilliseconds(correctedTimestamp),  // Timestamp
                ref _searchLiveData.Data);                      // Search data
        }
        else
        {
            // Fragment complete, queue for processing
            _fingerprintQueue.Enqueue(_searchLiveData);
            _searchLiveData = null;
            
            // Process queued fragments
            ProcessQueuedFragments();
        }
    }

    private void ProcessOverlappingFragment(VideoFrameXBufferEventArgs e)
    {
        long timestamp = (long)(e.Frame.Timestamp.TotalMilliseconds * 1000);
        
        // Start overlap processing after half fragment duration
        if (timestamp < _fragmentDuration / 2)
            return;

        // Initialize overlapping fragment if needed
        if (_searchLiveOverlapData == null)
        {
            _searchLiveOverlapData = new FingerprintLiveData(
                TimeSpan.FromMilliseconds(_fragmentDuration),
                DateTime.Now);
            _overlapFragmentCount++;
        }

        // Check if we're within overlap fragment duration
        if (timestamp < _fragmentDuration * _overlapFragmentCount + _fragmentDuration / 2)
        {
            CopyMemory(_tempBuffer, e.Frame.Data, e.Frame.DataSize);
            
            VFPSearch.Process(
                _tempBuffer,
                e.Frame.Width,
                e.Frame.Height,
                ((e.Frame.Width * 3 + 3) / 4) * 4,
                TimeSpan.FromMilliseconds(timestamp),
                ref _searchLiveOverlapData.Data);
        }
        else
        {
            // Overlap fragment complete
            _fingerprintQueue.Enqueue(_searchLiveOverlapData);
            _searchLiveOverlapData = null;
            
            ProcessQueuedFragments();
        }
    }

    private void ProcessQueuedFragments()
    {
        lock (_processingLock)
        {
            if (_fingerprintQueue.TryDequeue(out var fragmentData))
            {
                // Build fingerprint from fragment data
                long dataSize;
                IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref fragmentData.Data);
                
                // Create fingerprint object
                var fingerprint = new VFPFingerPrint
                {
                    Data = new byte[dataSize],
                    OriginalFilename = string.Empty
                };
                
                Marshal.Copy(fingerprintPtr, fingerprint.Data, 0, (int)dataSize);
                
                // Search for matches against reference fingerprints
                SearchForMatches(fingerprint, fragmentData.StartTime);
                
                // Clean up
                fragmentData.Data.Free();
                fragmentData.Dispose();
            }
        }
    }

    private void SearchForMatches(VFPFingerPrint liveFingerprint, DateTime fragmentStartTime)
    {
        foreach (var referenceFingerprint in _referenceFingerprints)
        {
            // Search for matches with configurable difference threshold
            var matches = VFPAnalyzer.Search(
                referenceFingerprint,           // Reference fingerprint
                liveFingerprint,                // Live fingerprint to search
                referenceFingerprint.Duration,  // Duration to search
                differenceLevel: 10,             // Similarity threshold (0-100)
                multipleSearch: true);           // Find all matches
            
            if (matches.Count > 0)
            {
                foreach (var match in matches)
                {
                    // Calculate actual timestamp of match
                    var matchTime = fragmentStartTime.AddMilliseconds(match.TotalMilliseconds);
                    
                    OnMatchFound(new MatchResult
                    {
                        ReferenceFile = referenceFingerprint.OriginalFilename,
                        Timestamp = matchTime,
                        Position = match,
                        Confidence = CalculateConfidence(differenceLevel: 10)
                    });
                }
            }
        }
    }

    public async Task StartCameraStreamAsync(string deviceName, string format, double frameRate)
    {
        // Configure camera source
        var devices = await _videoCapture.Video_SourcesAsync();
        var device = devices.FirstOrDefault(d => d.Name == deviceName);
        var videoFormat = device?.VideoFormats.FirstOrDefault(f => f.Name == format);
        
        var settings = new VideoCaptureDeviceSourceSettings(device)
        {
            Format = videoFormat.ToFormat()
        };
        settings.Format.FrameRate = new VideoFrameRate(frameRate);
        
        _videoCapture.Video_Source = settings;
        _videoCapture.Start();
    }

    public async Task StartNetworkStreamAsync(string streamUrl)
    {
        // Configure network stream source
        var sourceSettings = await UniversalSourceSettings.CreateAsync(streamUrl);
        
        // For IP cameras with authentication
        if (requiresAuth)
        {
            sourceSettings.Login = "username";
            sourceSettings.Password = "password";
        }
        
        await _videoCapture.StartAsync(sourceSettings);
    }

    public void Stop()
    {
        _videoCapture?.Stop();
        
        // Process any remaining fragments
        while (_fingerprintQueue.TryDequeue(out var fragment))
        {
            ProcessQueuedFragments();
        }
        
        // Clean up
        if (_tempBuffer != IntPtr.Zero)
        {
            Marshal.FreeCoTaskMem(_tempBuffer);
            _tempBuffer = IntPtr.Zero;
        }
        
        _searchLiveData?.Dispose();
        _searchLiveOverlapData?.Dispose();
    }

    // Helper methods
    private long GetMaxReferenceDuration()
    {
        return _referenceFingerprints.Max(fp => (long)fp.Duration.TotalMilliseconds);
    }

    private List<string> GetReferenceVideos()
    {
        // Return list of reference video files
        return new List<string> { "ad1.mp4", "ad2.mp4", "copyrighted_content.mp4" };
    }

    private double CalculateConfidence(int differenceLevel)
    {
        return (100.0 - differenceLevel) / 100.0;
    }

    private void OnMatchFound(MatchResult result)
    {
        Console.WriteLine($"Match found: {result.ReferenceFile} at {result.Timestamp:HH:mm:ss.fff}");
    }

    [DllImport("msvcrt.dll", EntryPoint = "memcpy")]
    private static extern void CopyMemory(IntPtr dest, IntPtr src, int length);
}

// Supporting classes
public class MatchResult
{
    public string ReferenceFile { get; set; }
    public DateTime Timestamp { get; set; }
    public TimeSpan Position { get; set; }
    public double Confidence { get; set; }
}
```

## Key Concepts

### Fragment Duration

The fragment duration determines how much video data is accumulated before building a fingerprint:

```csharp
// Calculate optimal fragment duration
// Should be at least 2x the longest content you want to detect
var maxContentDuration = 30000; // 30 seconds in milliseconds
var fragmentDuration = ((maxContentDuration + 1000) / 1000 + 1) * 1000 * 2;
// Result: 62000ms (62 seconds) fragments
```

### Overlapping Fragments

Using overlapping fragments improves detection accuracy by ensuring content isn't missed at fragment boundaries:

```csharp
// Main fragment: 0-60 seconds
// Overlap fragment 1: 30-90 seconds (starts at 50% of main)
// Overlap fragment 2: 60-120 seconds
// This ensures any content is fully captured in at least one fragment
```

### Frame Processing

Each frame is processed individually and added to the current fragment's search data:

```csharp
VFPSearch.Process(
    frameData,      // RGB24 frame data pointer
    width,          // Frame width
    height,         // Frame height  
    stride,         // Row stride in bytes
    timestamp,      // Frame timestamp
    ref searchData  // Accumulator for this fragment
);
```

### Building Fingerprints

Once a fragment is complete, build the fingerprint:

```csharp
long dataSize;
IntPtr fingerprintPtr = VFPSearch.Build(out dataSize, ref searchData);

// Copy to managed array
var fingerprintData = new byte[dataSize];
Marshal.Copy(fingerprintPtr, fingerprintData, 0, (int)dataSize);
```

## Performance Considerations

### Memory Management

- Pre-allocate buffers to avoid repeated allocations
- Use `Marshal.AllocCoTaskMem` for unmanaged buffers
- Properly dispose of `VFPSearchData` objects after use
- Free search data with `searchData.Free()` after building

### Processing Strategy

- Use a queue to decouple frame capture from fingerprint processing
- Process fragments in a separate thread to avoid blocking capture
- Adjust fragment duration based on reference content length
- Use overlapping fragments for better detection accuracy

### Optimization Tips

1. **Fragment Duration**: Longer fragments = better accuracy but higher latency
2. **Overlap Percentage**: 50% overlap is typical, adjust based on needs
3. **Difference Threshold**: Lower values = stricter matching (0-100 scale)
4. **Buffer Size**: Pre-allocate based on maximum expected frame size

## Common Use Cases

### Live Broadcast Monitoring

Monitor live TV broadcasts for copyrighted content or advertisements:

```csharp
// Initialize with broadcast content library
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("ads_library/");

// Start processing broadcast stream
await processor.StartNetworkStreamAsync("rtsp://broadcast-server/stream1");
```

### Security Camera Analysis

Detect specific events or objects in security camera feeds:

```csharp
// Process multiple camera streams
var processors = new List<RealTimeFingerprintProcessor>();

foreach (var camera in GetSecurityCameras())
{
    var processor = new RealTimeFingerprintProcessor();
    await processor.StartNetworkStreamAsync(camera.StreamUrl);
    processors.Add(processor);
}
```

### Content Compliance

Ensure streaming platforms comply with content restrictions:

```csharp
// Monitor user-generated streams
var processor = new RealTimeFingerprintProcessor();
await processor.LoadReferenceFingerprintsAsync("prohibited_content/");

// Process incoming stream
await processor.StartNetworkStreamAsync(userStreamUrl);
```

## Troubleshooting

### No Matches Found

- Verify fragment duration is appropriate for reference content length
- Check difference threshold isn't too strict
- Ensure overlapping fragments are enabled
- Verify reference fingerprints were generated correctly

### High Memory Usage

- Reduce fragment duration if possible
- Process and dispose fragments more frequently
- Use smaller overlap percentage
- Free unmanaged buffers properly

### Processing Lag

- Use separate threads for capture and processing
- Implement frame dropping if processing can't keep up
- Optimize reference fingerprint search (index, parallel search)
- Consider GPU acceleration if available

## Summary

Real-time fingerprinting with the VisioForge SDK uses a fragment-based approach that:
- Accumulates frames into time-based fragments using `VFPSearchData`
- Processes frames in real-time with `VFPSearch.Process()`
- Builds fingerprints from fragments with `VFPSearch.Build()`
- Uses overlapping fragments for robust detection
- Enables live content matching against reference fingerprints

This approach is proven in production environments for broadcast monitoring, content compliance, and security applications.

## Complete Sample Application

For a complete working example of real-time video fingerprinting, see the **MMT Live** sample application:
- [MMT Live Sample on GitHub](https://github.com/visioforge/.Net-SDK-s-samples/tree/master/Video%20Fingerprinting%20SDK/MMT%20Live)

This sample demonstrates all the concepts covered in this guide with a full Windows Forms application for live commercial detection.