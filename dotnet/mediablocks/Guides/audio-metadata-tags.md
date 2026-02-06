---
title: Write Audio Tags with Media Blocks SDK
description: Write audio metadata tags (ID3, Vorbis Comments, MP4 metadata, ASF metadata) to audio files with practical code examples for MP3, OGG, M4A.
---

# Write Audio Tags with Media Blocks SDK

[Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Table of Contents

- [Write Audio Tags with Media Blocks SDK](#write-audio-tags-with-media-blocks-sdk)
  - [Table of Contents](#table-of-contents)
  - [Overview](#overview)
  - [Core Features](#core-features)
  - [Supported Audio Formats](#supported-audio-formats)
  - [Prerequisites](#prerequisites)
  - [MediaFileTags: The Unified Interface](#mediafiletags-the-unified-interface)
  - [Code Examples by Format](#code-examples-by-format)
    - [MP3 Output with ID3 Tags](#mp3-output-with-id3-tags)
    - [OGG Vorbis Output with Vorbis Comments](#ogg-vorbis-output-with-vorbis-comments)
    - [M4A Output with MP4 Metadata](#m4a-output-with-mp4-metadata)
    - [WMV/WMA Output with ASF Metadata](#wmvwma-output-with-asf-metadata)
  - [Complete Audio Recording Example](#complete-audio-recording-example)
  - [Advanced Tag Scenarios](#advanced-tag-scenarios)
    - [Album Artwork Support](#album-artwork-support)
    - [Runtime Tag Modification](#runtime-tag-modification)
    - [Multi-Track Albums](#multi-track-albums)
  - [Best Practices](#best-practices)
    - [Tag Data Quality](#tag-data-quality)
    - [Performance Considerations](#performance-considerations)
    - [Format-Specific Guidelines](#format-specific-guidelines)
  - [Troubleshooting](#troubleshooting)
    - [Common Issues and Solutions](#common-issues-and-solutions)
    - [Debug Tag Writing](#debug-tag-writing)
  - [Tag Format Specifications](#tag-format-specifications)
    - [ID3 Tags (MP3)](#id3-tags-mp3)
    - [Vorbis Comments (OGG)](#vorbis-comments-ogg)
    - [MP4 Metadata (M4A)](#mp4-metadata-m4a)
    - [ASF Attributes (WMV/WMA)](#asf-attributes-wmvwma)

## Overview

The VisioForge Media Blocks SDK provides comprehensive support for writing audio metadata tags to output files across all major audio formats. Whether you're building a music production application, podcast recorder, or audio content management system, you can easily embed rich metadata into your audio files using a unified programming interface.

This guide demonstrates how to add metadata tags like artist, album, title, year, genre, and more to MP3, OGG Vorbis, M4A, and WMV/WMA audio files using format-appropriate tagging mechanisms while maintaining industry standards compliance.

## Core Features

- **Universal Tag Support**: Write metadata to MP3 (ID3), OGG (Vorbis Comments), M4A (MP4 atoms), and WMV (ASF attributes)
- **Comprehensive Metadata**: Support for 20+ tag fields including title, artist, album, year, track numbers, lyrics, and album artwork
- **Standards Compliant**: Uses native container tag mechanisms for optimal compatibility
- **Unified API**: Single `MediaFileTags` interface works across all output formats
- **Professional Quality**: Industry-standard tag writing with proper encoding and structure
- **Runtime Flexibility**: Modify tags before and during pipeline execution

## Supported Audio Formats

| Format | Container | Tag System | Standards |
|--------|-----------|------------|-----------|
| **MP3** | MPEG-1 Layer 3 | ID3v1/ID3v2 | ISO/IEC 11172-3, ID3v2.4 |
| **OGG Vorbis** | OGG | Vorbis Comments | Xiph.Org specification |
| **M4A** | MP4/MPEG-4 | MP4 metadata atoms | ISO Base Media File Format |
| **WMV/WMA** | ASF | ASF metadata attributes | Microsoft ASF specification |

## Prerequisites

- VisioForge Media Blocks SDK .NET
- .NET Framework 4.7.2+ or .NET Core 3.1+ or .NET 5+
- Basic understanding of audio processing pipelines

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.MediaBlocks;
```

## MediaFileTags: The Unified Interface

The `MediaFileTags` class provides a unified interface for audio metadata across all supported formats. This class contains all common audio metadata fields and automatically maps them to the appropriate tag format for each output container.

```csharp
// Create comprehensive audio metadata
var audioTags = new MediaFileTags
{
    // Basic metadata
    Title = "Bohemian Rhapsody",
    Performers = new[] { "Queen" },
    Album = "A Night at the Opera",
    Year = 1975,
    
    // Track information
    Track = 11,
    TrackCount = 12,
    Disc = 1,
    DiscCount = 1,
    
    // Genre and categorization
    Genres = new[] { "Progressive Rock", "Opera Rock" },
    
    // Extended metadata
    Composers = new[] { "Freddie Mercury" },
    Conductor = "Roy Thomas Baker",
    Comment = "6-minute epic masterpiece",
    Copyright = "© 1975 Queen Productions Ltd.",
    
    // Technical metadata
    BeatsPerMinute = 72,
    Grouping = "Epic Songs",
    
    // Lyrics (for supported formats)
    Lyrics = @"Is this the real life?
Is this just fantasy?
Caught in a landslide
No escape from reality..."
};
```

## Code Examples by Format

### MP3 Output with ID3 Tags

MP3 files use ID3 tags (both v1 and v2) for metadata storage. The SDK uses GStreamer's `id3mux` element to write standards-compliant ID3 tags.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;

public async Task CreateMP3WithTags()
{
    // Configure MP3 encoder settings
    var mp3Settings = new MP3EncoderSettings
    {
        Bitrate = 320, // High quality 320 kbps
        BitrateMode = MP3BitrateMode.CBR
    };
    
    // Create metadata tags
    var tags = new MediaFileTags
    {
        Title = "Summer Vibes",
        Performers = new[] { "Indie Artist" },
        Album = "Seasonal Collection",
        Year = 2025,
        Track = 3,
        TrackCount = 10,
        Genres = new[] { "Indie Pop", "Electronic" },
        Comment = "Recorded in home studio",
        Copyright = "© 2025 Independent Label"
    };
    
    // Create MP3 output block with tags
    var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings, tags);
    
    // Alternative: Set tags after creation
    // var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
    // mp3Output.Tags = tags;
    
    // Build your complete pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Add audio source (microphone, file, etc.)
    var audioSource = new AudioCaptureSourceBlock();
    
    // Connect and build pipeline
    pipeline.Connect(audioSource.Output, mp3Output.Input);
    
    // Start recording with metadata
    await pipeline.StartAsync();
    
    // Recording will include ID3 tags in the MP3 file
    await Task.Delay(30000); // Record for 30 seconds
    
    await pipeline.StopAsync();
}
```

### OGG Vorbis Output with Vorbis Comments

OGG Vorbis files use Vorbis Comments for metadata, which are embedded directly in the audio stream by the Vorbis encoder.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;

public async Task CreateOGGWithTags()
{
    // Configure Vorbis encoder settings
    var vorbisSettings = new VorbisEncoderSettings
    {
        Quality = 0.8f, // High quality (0.0 to 1.0 scale)
        BitrateMode = VorbisBitrateMode.VBR
    };
    
    // Create comprehensive metadata
    var tags = new MediaFileTags
    {
        Title = "Acoustic Session",
        Performers = new[] { "Folk Artist", "Guest Vocalist" },
        AlbumArtists = new[] { "Folk Artist" },
        Album = "Live Sessions",
        Year = 2025,
        Track = 1,
        Genres = new[] { "Folk", "Acoustic" },
        Composers = new[] { "Folk Artist", "Traditional" },
        Comment = "Recorded live at Studio A",
        
        // Vorbis Comments support extensive metadata
        Conductor = "Sound Engineer",
        Grouping = "Live Recordings",
        Lyrics = @"In the quiet of the morning
When the world begins to wake
There's a song within the silence..."
    };
    
    // Create OGG output block with Vorbis comments
    var oggOutput = new OGGVorbisOutputBlock("output.ogg", vorbisSettings, tags);
    
    // Build and execute pipeline
    var pipeline = new MediaBlocksPipeline();
    var audioSource = new AudioFileSourceBlock("input.wav");
    
    pipeline.Connect(audioSource.Output, oggOutput.Input);
    
    await pipeline.StartAsync();
    await pipeline.WaitForCompletionAsync();
}
```

### M4A Output with MP4 Metadata

M4A files use MP4 metadata atoms for storing information, compatible with iTunes and most media players.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;

public async Task CreateM4AWithTags()
{
    // Configure AAC encoder for M4A
    var aacSettings = new AACEncoderSettings
    {
        Bitrate = 256,
        Profile = AACProfile.LC, // Low Complexity for broad compatibility
        Channels = 2
    };
    
    // Create podcast metadata
    var tags = new MediaFileTags
    {
        Title = "Episode 42: The Future of AI",
        Performers = new[] { "Tech Podcast Host" },
        Album = "Weekly Tech Talk",
        Year = 2025,
        Track = 42,
        Genres = new[] { "Technology", "Podcast" },
        Comment = "Special guest interview with AI researcher",
        Copyright = "© 2025 Tech Media Network",
        
        // Podcast-specific metadata
        Subtitle = "Exploring artificial intelligence trends",
        Grouping = "Season 3"
    };
    
    // Create M4A output with MP4 metadata
    var m4aOutput = new M4AOutputBlock("podcast_episode_42.m4a", tags);
    
    // Pipeline setup for podcast recording
    var pipeline = new MediaBlocksPipeline();
    var micSource = new AudioCaptureSourceBlock();
    
    // Optional: Add audio processing
    var volumeFilter = new VolumeFilterBlock { Volume = 1.2f };
    var noiseGate = new NoiseGateBlock { Threshold = -40.0f };
    
    // Connect processing chain
    pipeline.Connect(micSource.Output, volumeFilter.Input);
    pipeline.Connect(volumeFilter.Output, noiseGate.Input);
    pipeline.Connect(noiseGate.Output, m4aOutput.Input);
    
    await pipeline.StartAsync();
}
```

### WMV/WMA Output with ASF Metadata

Windows Media formats use ASF (Advanced Systems Format) metadata attributes for storing information.

```csharp
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.VideoEncoders;
using VisioForge.Core.Types.X.Sinks;

public async Task CreateWMVWithTags()
{
    // Configure Windows Media encoders
    var wmaSettings = new WMAEncoderSettings
    {
        Bitrate = 192,
        SampleRate = 44100,
        Channels = 2
    };
    
    var wmvSettings = new WMVEncoderSettings
    {
        Bitrate = 2000000, // 2 Mbps
        Width = 1920,
        Height = 1080,
        FrameRate = 30
    };
    
    var asfSettings = new ASFSinkSettings("presentation.wmv");
    
    // Create presentation metadata
    var tags = new MediaFileTags
    {
        Title = "Q4 Business Review",
        Performers = new[] { "CEO", "CFO", "VP Sales" },
        Album = "Corporate Presentations 2025",
        Year = 2025,
        Genres = new[] { "Business", "Corporate" },
        Comment = "Quarterly financial review and outlook",
        Copyright = "© 2025 Business Corp. Confidential",
        
        // Corporate metadata
        Conductor = "Meeting Organizer",
        Grouping = "Executive Presentations"
    };
    
    // Create WMV output with ASF metadata
    var wmvOutput = new WMVOutputBlock(asfSettings, wmvSettings, wmaSettings, tags);
    
    // Setup for video + audio recording
    var pipeline = new MediaBlocksPipeline();
    
    // Add video and audio sources
    var videoSource = new VideoCaptureSourceBlock();
    var audioSource = new AudioCaptureSourceBlock();
    
    // Create input pads for the WMV output
    var videoPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Video);
    var audioPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Audio);
    
    // Connect sources to WMV output
    pipeline.Connect(videoSource.Output, videoPad);
    pipeline.Connect(audioSource.Output, audioPad);
    
    await pipeline.StartAsync();
}
```

## Complete Audio Recording Example

Here's a comprehensive example that demonstrates recording audio with different output formats and metadata:

```csharp
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.Types;

public class AudioRecorderWithTags
{
    public async Task RecordAudioWithMetadata()
    {
        // Create rich metadata for the recording
        var sessionTags = new MediaFileTags
        {
            Title = "Studio Session #1",
            Performers = new[] { "John Doe", "Jane Smith" },
            Album = "Demo Recordings",
            Year = 2025,
            Track = 1,
            Genres = new[] { "Rock", "Alternative" },
            Composers = new[] { "John Doe" },
            Comment = "First studio recording session",
            Copyright = "© 2025 Demo Productions",
            BeatsPerMinute = 120,
            Grouping = "Demo Sessions"
        };
        
        // Create multiple output formats with the same metadata
        var outputs = new IMediaBlockSink[]
        {
            // High-quality MP3
            new MP3OutputBlock("session1.mp3", new MP3EncoderSettings 
            { 
                Bitrate = 320, 
                BitrateMode = MP3BitrateMode.CBR 
            }, sessionTags),
            
            // Lossless-quality OGG Vorbis
            new OGGVorbisOutputBlock("session1.ogg", new VorbisEncoderSettings 
            { 
                Quality = 1.0f 
            }, sessionTags),
            
            // Professional M4A
            new M4AOutputBlock("session1.m4a", sessionTags),
            
            // Windows Media format
            new WMVOutputBlock("session1.wma", sessionTags)
        };
        
        // Setup recording pipeline
        var pipeline = new MediaBlocksPipeline();
        var audioSource = new AudioCaptureSourceBlock();
        
        // Connect source to all outputs (splitter will be created automatically)
        foreach (var output in outputs)
        {
            pipeline.Connect(audioSource.Output, output.Input);
        }
        
        // Start recording
        Console.WriteLine("Starting recording with metadata...");
        await pipeline.StartAsync();
        
        // Record for specified duration
        await Task.Delay(TimeSpan.FromMinutes(3));
        
        // Stop recording
        Console.WriteLine("Stopping recording...");
        await pipeline.StopAsync();
        
        Console.WriteLine("Recording complete! Files created with metadata:");
        Console.WriteLine("- session1.mp3 (ID3 tags)");
        Console.WriteLine("- session1.ogg (Vorbis comments)");
        Console.WriteLine("- session1.m4a (MP4 metadata)");
        Console.WriteLine("- session1.wma (ASF metadata)");
    }
}
```

## Advanced Tag Scenarios

### Album Artwork Support

Add album artwork to your audio files (supported by MP3, M4A, and WMV formats):

```csharp
var tags = new MediaFileTags
{
    Title = "Album Title Track",
    Performers = new[] { "Artist Name" },
    Album = "Album Name"
};

// Add album artwork (Windows platforms)
#if NET_WINDOWS
if (File.Exists("album_cover.jpg"))
{
    var albumArt = new System.Drawing.Bitmap("album_cover.jpg");
    tags.Pictures = new[] { albumArt };
    tags.Pictures_Descriptions = new[] { "Front Cover" };
}
#endif

var mp3Output = new MP3OutputBlock("track.mp3", mp3Settings, tags);
```

### Runtime Tag Modification

Modify tags during pipeline execution:

```csharp
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);

// Initial tags
mp3Output.Tags = new MediaFileTags
{
    Title = "Live Recording",
    Performers = new[] { "Artist" }
};

// Update tags before starting (for example, based on user input)
mp3Output.Tags.Comment = $"Recorded on {DateTime.Now:yyyy-MM-dd}";
mp3Output.Tags.Year = (uint)DateTime.Now.Year;

await pipeline.StartAsync();
```

### Multi-Track Albums

Create consistent metadata across album tracks:

```csharp
public class AlbumRecorder
{
    private readonly MediaFileTags _baseAlbumTags;
    
    public AlbumRecorder()
    {
        _baseAlbumTags = new MediaFileTags
        {
            Album = "My Album",
            AlbumArtists = new[] { "Main Artist" },
            Year = 2025,
            Genres = new[] { "Pop", "Electronic" },
            TrackCount = 12,
            Copyright = "© 2025 Record Label"
        };
    }
    
    public void RecordTrack(int trackNumber, string title, string[] performers)
    {
        var trackTags = new MediaFileTags
        {
            // Copy base album information
            Album = _baseAlbumTags.Album,
            AlbumArtists = _baseAlbumTags.AlbumArtists,
            Year = _baseAlbumTags.Year,
            Genres = _baseAlbumTags.Genres,
            TrackCount = _baseAlbumTags.TrackCount,
            Copyright = _baseAlbumTags.Copyright,
            
            // Track-specific information
            Track = (uint)trackNumber,
            Title = title,
            Performers = performers
        };
        
        var output = new MP3OutputBlock($"track_{trackNumber:D2}.mp3", mp3Settings, trackTags);
        
        // Continue with pipeline setup...
    }
}
```

## Best Practices

### Tag Data Quality

- **Consistent Encoding**: Use UTF-8 encoding for international characters
- **Complete Information**: Fill in as many relevant tag fields as possible
- **Standardized Genres**: Use recognized genre names for better compatibility
- **Proper Copyright**: Include appropriate copyright notices

### Performance Considerations

- **Tag Size**: Keep text fields reasonable in length to avoid bloating files
- **Image Compression**: Compress album artwork appropriately (JPEG recommended)
- **Batch Processing**: When creating multiple files, reuse tag objects when possible

### Format-Specific Guidelines

```csharp
// MP3: ID3v2 supports extensive metadata
var mp3Tags = new MediaFileTags
{
    // Rich metadata fully supported
    Title = "Song Title",
    Subtitle = "Song Subtitle", // ID3v2.4 TIT3 frame
    Lyrics = "Full lyrics text", // USLT frame
    BeatsPerMinute = 128 // TBPM frame
};

// OGG: Vorbis comments are very flexible
var oggTags = new MediaFileTags
{
    // All fields map well to Vorbis comment fields
    Composers = new[] { "Composer 1", "Composer 2" }, // Multiple values supported
    Performers = new[] { "Artist 1", "Artist 2" }
};

// M4A: iTunes-compatible metadata
var m4aTagsForPodcast = new MediaFileTags
{
    Title = "Episode Title",
    Album = "Podcast Series Name", // Shows as "Album" in iTunes
    Performers = new[] { "Host Name" }, // Shows as "Artist"
    Genres = new[] { "Podcast" }, // Use "Podcast" genre for podcasts
    Comment = "Episode description"
};
```

## Troubleshooting

### Common Issues and Solutions

**Tags not appearing in media players:**

- Ensure the output format supports the specific tag fields you're using
- Verify that the media player supports the tag format (some players prefer ID3v2.3 over ID3v2.4)
- Check that text encoding is correct (UTF-8 recommended)

**File size unexpectedly large:**

- Reduce album artwork resolution (recommended: 600x600 pixels maximum)
- Avoid extremely long text fields in comments or lyrics
- Use appropriate image compression for artwork

**Encoding errors:**

- Validate that special characters are properly encoded
- Ensure file paths are accessible and writable
- Check that encoder settings are compatible with your system

### Debug Tag Writing

```csharp
var pipeline = new MediaBlocksPipeline();

// Enable detailed logging to see tag processing
pipeline.OnMessage += (sender, e) => 
{
    if (e.Message.Contains("tag") || e.Message.Contains("metadata"))
    {
        Console.WriteLine($"Tag Debug: {e.Message}");
    }
};

// Continue with pipeline setup...
```

## Tag Format Specifications

### ID3 Tags (MP3)

- **ID3v1**: Basic 128-byte structure with limited fields
- **ID3v2**: Extensible format supporting Unicode, multiple values, and custom frames
- **Common Frames**: TIT2 (Title), TPE1 (Artist), TALB (Album), TDRC (Year), TCON (Genre)

### Vorbis Comments (OGG)

- **Format**: UTF-8 text in NAME=VALUE format
- **Standard Fields**: TITLE, ARTIST, ALBUM, DATE, GENRE, TRACKNUMBER
- **Flexible**: Supports arbitrary field names and multiple values

### MP4 Metadata (M4A)

- **Atoms**: iTunes-style metadata stored in MP4 atoms
- **Common Atoms**: ©nam (Title), ©ART (Artist), ©alb (Album), ©day (Year)
- **Binary Data**: Supports embedded artwork in covr atom

### ASF Attributes (WMV/WMA)

- **Structure**: Key-value pairs in ASF header
- **Standard Attributes**: Title, Author, Copyright, Description
- **Extended**: Custom attributes supported

---
This comprehensive guide demonstrates how the VisioForge Media Blocks SDK provides professional-grade audio metadata writing capabilities across all major audio formats. The unified `MediaFileTags` interface simplifies the development process while ensuring standards compliance and optimal compatibility with media players and applications.
For more advanced audio processing scenarios and additional SDK features, explore the complete [VisioForge Media Blocks SDK documentation](../index.md).