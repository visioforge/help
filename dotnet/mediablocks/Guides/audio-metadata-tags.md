---
title: Write Audio Metadata Tags in C# .NET - ID3, Vorbis
description: Add ID3, Vorbis Comments, and MP4 metadata to audio files using VisioForge Media Blocks SDK. Code examples for MP3, OGG, M4A, and WMA tagging.
tags:
  - Media Blocks SDK
  - .NET
  - MediaBlocksPipeline
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Encoding
  - Metadata
  - MP4
  - WMV
  - OGG
  - AAC
  - MP3
  - Vorbis
  - WMA
  - C#
primary_api_classes:
  - MediaFileTags
  - MP3OutputBlock
  - OGGVorbisOutputBlock
  - M4AOutputBlock
  - WMVOutputBlock
  - MediaBlocksPipeline
  - SystemAudioSourceBlock

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

The VisioForge Media Blocks SDK supports writing audio metadata tags to output files across all major audio formats. Whether you're building a music production application, podcast recorder, or audio content management system, you can embed rich metadata into your audio files using a unified programming interface.

This guide demonstrates how to add metadata tags like artist, album, title, year, genre, and more to MP3, OGG Vorbis, M4A, and WMV/WMA audio files using format-appropriate tagging mechanisms while maintaining industry standards compliance.

## Core Features

- **Universal tag support**: Write metadata to MP3 (ID3), OGG (Vorbis Comments), M4A (MP4 atoms), and WMV (ASF attributes)
- **Comprehensive metadata**: 20+ tag fields including title, artist, album, year, track numbers, lyrics, and album artwork
- **Standards compliant**: Uses native container tag mechanisms for optimal compatibility
- **Unified API**: Single `MediaFileTags` instance works across all output formats
- **Runtime flexibility**: Modify tags before pipeline execution

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
using VisioForge.Core.MediaBlocks;
using VisioForge.Core.MediaBlocks.Outputs;
using VisioForge.Core.MediaBlocks.Sinks;
using VisioForge.Core.MediaBlocks.Sources;
using VisioForge.Core.Types;
using VisioForge.Core.Types.X.AudioEncoders;
using VisioForge.Core.Types.X.Sources;
```

## MediaFileTags: The Unified Interface

The `MediaFileTags` class provides a unified interface for audio metadata across all supported formats. It contains common metadata fields and is mapped to the appropriate tag format for each output container by the specific output block.

```csharp
// Create audio metadata
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

All string-array fields (`Performers`, `Composers`, `Genres`, `AlbumArtists`) accept multiple values and are written as repeated frames on formats that support them (Vorbis Comments, ID3v2). Numeric fields (`Year`, `Track`, `TrackCount`, `Disc`, `DiscCount`, `BeatsPerMinute`) are `uint`.

## Code Examples by Format

### MP3 Output with ID3 Tags

MP3 files use ID3 tags (both v1 and v2) for metadata storage. `MP3OutputBlock` writes standards-compliant ID3 tags via the GStreamer `id3mux` element.

```csharp
public async Task CreateMP3WithTags()
{
    // Configure MP3 encoder settings
    var mp3Settings = new MP3EncoderSettings
    {
        Bitrate = 320,                              // Kbit/s
        RateControl = MP3EncoderRateControl.CBR    // CBR / ABR / VBR
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
    
    // Alternative: set tags after creation via the Tags property
    // var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);
    // mp3Output.Tags = tags;
    
    // Build the pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Add an audio source (microphone, file, etc.)
    var audioSource = new SystemAudioSourceBlock();
    
    // Connect source directly to MP3 output
    pipeline.Connect(audioSource.Output, mp3Output.Input);
    
    // Start recording with metadata
    await pipeline.StartAsync();
    
    // Recording writes ID3 tags into the MP3 file
    await Task.Delay(TimeSpan.FromSeconds(30));
    
    await pipeline.StopAsync();
}
```

### OGG Vorbis Output with Vorbis Comments

OGG Vorbis files use Vorbis Comments for metadata, which are embedded directly in the audio stream by the Vorbis encoder.

```csharp
public async Task CreateOGGWithTags()
{
    // Configure Vorbis encoder settings.
    // Quality is int in the range [-1..10] (default 4). Used when RateControl = Quality.
    var vorbisSettings = new VorbisEncoderSettings
    {
        Quality = 8,
        RateControl = VorbisEncoderRateControl.Quality
    };
    
    // Create metadata
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
        Conductor = "Sound Engineer",
        Grouping = "Live Recordings",
        Lyrics = @"In the quiet of the morning
When the world begins to wake
There's a song within the silence..."
    };
    
    // Create OGG output block with Vorbis comments
    var oggOutput = new OGGVorbisOutputBlock("output.ogg", vorbisSettings, tags);
    
    // Build and execute the pipeline
    var pipeline = new MediaBlocksPipeline();
    
    // Use UniversalSourceBlock to decode any file format into raw audio
    var sourceSettings = await UniversalSourceSettings.CreateAsync(new Uri("input.wav"));
    var fileSource = new UniversalSourceBlock(sourceSettings);
    
    pipeline.Connect(fileSource.AudioOutput, oggOutput.Input);
    
    await pipeline.StartAsync();
    await pipeline.WaitForStopAsync();   // Wait for EOS
}
```

### M4A Output with MP4 Metadata

M4A files use MP4 metadata atoms, compatible with iTunes and most media players. The default `M4AOutputBlock` ctor picks a default AAC encoder; use the 3-arg overload to pick a specific AAC implementation (`AVENCAACEncoderSettings`, `VOAACEncoderSettings`, or `MFAACEncoderSettings` on Windows).

```csharp
public async Task CreateM4AWithTags()
{
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
        Subtitle = "Exploring artificial intelligence trends",
        Grouping = "Season 3"
    };
    
    // Option A: simplest — default AAC encoder picked internally
    var m4aOutput = new M4AOutputBlock("podcast_episode_42.m4a", tags);
    
    // Option B: pick a specific AAC encoder and sink settings
    // var sinkSettings = new MP4SinkSettings("podcast_episode_42.m4a");
    // var aacSettings = new AVENCAACEncoderSettings { Bitrate = 256 }; // 256 Kbit/s
    // var m4aOutput = new M4AOutputBlock(sinkSettings, aacSettings, tags);
    
    // Pipeline setup for podcast recording
    var pipeline = new MediaBlocksPipeline();
    var micSource = new SystemAudioSourceBlock();
    
    pipeline.Connect(micSource.Output, m4aOutput.Input);
    
    await pipeline.StartAsync();
}
```

### WMV/WMA Output with ASF Metadata

Windows Media formats use ASF (Advanced Systems Format) metadata attributes. `WMVOutputBlock` accepts a `MediaFileTags` parameter and handles both audio-only and audio+video outputs.

```csharp
public async Task CreateWMVWithTags()
{
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
        Conductor = "Meeting Organizer",
        Grouping = "Executive Presentations"
    };
    
    // Simplest form — default encoders, just filename + tags.
    // WMVOutputBlock will use the default WMV/WMA encoder settings internally.
    var wmvOutput = new WMVOutputBlock("presentation.wmv", tags);
    
    // Alternative: pass explicit sink/video/audio settings objects
    // var asfSettings = new ASFSinkSettings("presentation.wmv");
    // var wmvSettings = WMVEncoderBlock.GetDefaultSettings();
    // var wmaSettings = WMAEncoderBlock.GetDefaultSettings();
    // var wmvOutput = new WMVOutputBlock(asfSettings, wmvSettings, wmaSettings, tags);
    
    // Setup for video + audio recording
    var pipeline = new MediaBlocksPipeline();
    
    // Video source — pick the first available device
    var videoDevice = (await DeviceEnumerator.Shared.VideoSourcesAsync())[0];
    var videoSource = new SystemVideoSourceBlock(new VideoCaptureDeviceSourceSettings(videoDevice));
    
    // Audio source
    var audioSource = new SystemAudioSourceBlock();
    
    // Create dynamic input pads on the WMV output
    var videoPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Video);
    var audioPad = wmvOutput.CreateNewInput(MediaBlockPadMediaType.Audio);
    
    pipeline.Connect(videoSource.Output, videoPad);
    pipeline.Connect(audioSource.Output, audioPad);
    
    await pipeline.StartAsync();
}
```

## Complete Audio Recording Example

Record the same audio source to multiple tagged output formats simultaneously:

```csharp
public class AudioRecorderWithTags
{
    public async Task RecordAudioWithMetadata()
    {
        // Rich metadata shared across every output file
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
        
        // MP3 output (CBR 320 Kbit/s + ID3 tags)
        var mp3Output = new MP3OutputBlock(
            "session1.mp3",
            new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
            sessionTags);
        
        // OGG Vorbis output (highest quality + Vorbis Comments)
        var oggOutput = new OGGVorbisOutputBlock(
            "session1.ogg",
            new VorbisEncoderSettings { Quality = 10, RateControl = VorbisEncoderRateControl.Quality },
            sessionTags);
        
        // M4A output (default AAC + MP4 atoms)
        var m4aOutput = new M4AOutputBlock("session1.m4a", sessionTags);
        
        // Pipeline with a single audio source fanned out to all three files
        var pipeline = new MediaBlocksPipeline();
        var audioSource = new SystemAudioSourceBlock();
        
        // Connecting the same source pad to multiple sinks auto-inserts a tee
        pipeline.Connect(audioSource.Output, mp3Output.Input);
        pipeline.Connect(audioSource.Output, oggOutput.Input);
        pipeline.Connect(audioSource.Output, m4aOutput.Input);
        
        Console.WriteLine("Starting recording with metadata...");
        await pipeline.StartAsync();
        
        await Task.Delay(TimeSpan.FromMinutes(3));
        
        Console.WriteLine("Stopping recording...");
        await pipeline.StopAsync();
        
        Console.WriteLine("Recording complete — files written with metadata:");
        Console.WriteLine("- session1.mp3 (ID3 tags)");
        Console.WriteLine("- session1.ogg (Vorbis comments)");
        Console.WriteLine("- session1.m4a (MP4 metadata)");
    }
}
```

## Advanced Tag Scenarios

### Album Artwork Support

Attach album artwork to supported formats (MP3, M4A, WMV). On Windows, `MediaFileTags.Pictures` accepts `System.Drawing.Bitmap[]`; cross-platform builds use `IBitmap[]`.

```csharp
var tags = new MediaFileTags
{
    Title = "Album Title Track",
    Performers = new[] { "Artist Name" },
    Album = "Album Name"
};

// Attach album artwork (Windows — System.Drawing)
#if NET_WINDOWS
if (File.Exists("album_cover.jpg"))
{
    var albumArt = new System.Drawing.Bitmap("album_cover.jpg");
    tags.Pictures = new[] { albumArt };
    tags.Pictures_Descriptions = new[] { "Front Cover" };
}
#endif

var mp3Output = new MP3OutputBlock(
    "track.mp3",
    new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR },
    tags);
```

### Runtime Tag Modification

Set or modify tags before starting the pipeline — once a pipeline has started, the tag payload for that output is already being emitted.

```csharp
var mp3Settings = new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };
var mp3Output = new MP3OutputBlock("output.mp3", mp3Settings);

// Assign tags via the Tags property before StartAsync
mp3Output.Tags = new MediaFileTags
{
    Title = "Live Recording",
    Performers = new[] { "Artist" }
};

// Tweak fields right up until start
mp3Output.Tags.Comment = $"Recorded on {DateTime.Now:yyyy-MM-dd}";
mp3Output.Tags.Year = (uint)DateTime.Now.Year;

await pipeline.StartAsync();
```

### Multi-Track Albums

Create consistent metadata across album tracks by using a shared base tag object:

```csharp
public class AlbumRecorder
{
    private readonly MediaFileTags _baseAlbumTags;
    private readonly MP3EncoderSettings _mp3Settings =
        new MP3EncoderSettings { Bitrate = 320, RateControl = MP3EncoderRateControl.CBR };

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

    public MP3OutputBlock CreateTrackOutput(int trackNumber, string title, string[] performers)
    {
        var trackTags = new MediaFileTags
        {
            // Inherit album-level metadata
            Album = _baseAlbumTags.Album,
            AlbumArtists = _baseAlbumTags.AlbumArtists,
            Year = _baseAlbumTags.Year,
            Genres = _baseAlbumTags.Genres,
            TrackCount = _baseAlbumTags.TrackCount,
            Copyright = _baseAlbumTags.Copyright,

            // Track-specific metadata
            Track = (uint)trackNumber,
            Title = title,
            Performers = performers
        };

        return new MP3OutputBlock($"track_{trackNumber:D2}.mp3", _mp3Settings, trackTags);
    }
}
```

## Best Practices

### Tag Data Quality

- **Consistent encoding**: Use UTF-8 encoding for international characters
- **Complete information**: Fill in as many relevant tag fields as possible
- **Standardized genres**: Use recognized genre names for better compatibility
- **Proper copyright**: Include appropriate copyright notices

### Performance Considerations

- **Tag size**: Keep text fields reasonable in length to avoid bloating files
- **Image compression**: Compress album artwork appropriately (JPEG recommended)
- **Reuse instances**: When creating multiple files, share base tag objects and only override per-track fields

### Format-Specific Guidelines

```csharp
// MP3: ID3v2 supports extensive metadata
var mp3Tags = new MediaFileTags
{
    Title = "Song Title",
    Subtitle = "Song Subtitle",     // ID3v2 TIT3 frame
    Lyrics = "Full lyrics text",    // USLT frame
    BeatsPerMinute = 128            // TBPM frame
};

// OGG: Vorbis comments are flexible and handle multi-value fields natively
var oggTags = new MediaFileTags
{
    Composers = new[] { "Composer 1", "Composer 2" },
    Performers = new[] { "Artist 1", "Artist 2" }
};

// M4A: iTunes-compatible metadata
var m4aTagsForPodcast = new MediaFileTags
{
    Title = "Episode Title",
    Album = "Podcast Series Name",  // Shows as "Album" in iTunes
    Performers = new[] { "Host Name" }, // Shows as "Artist"
    Genres = new[] { "Podcast" },
    Comment = "Episode description"
};
```

## Troubleshooting

### Common Issues and Solutions

**Tags not appearing in media players:**

- Ensure the output format supports the specific tag fields you're using
- Verify the media player supports the tag format (some players prefer ID3v2.3 over ID3v2.4)
- Check that text encoding is correct (UTF-8 recommended)

**File size unexpectedly large:**

- Reduce album artwork resolution (600×600 is usually enough)
- Avoid extremely long text fields in comments or lyrics
- Use appropriate image compression for artwork

**Encoding errors:**

- Validate that special characters are properly encoded
- Ensure file paths are accessible and writable
- Check that encoder settings are compatible with your system

### Debug Tag Writing

Subscribe to the pipeline's `OnError` event to see encoder/muxer failures during tag writing. There is no "tag messages only" stream — inspect the produced file with a tag reader (TagLib, MediaInfo, or the SDK's own `MediaInfoReader`) to confirm what was written.

```csharp
var pipeline = new MediaBlocksPipeline();

pipeline.OnError += (sender, e) =>
{
    Console.WriteLine($"Pipeline error: {e.Message}");
};

// Continue with pipeline setup...
```

## Tag Format Specifications

### ID3 Tags (MP3)

- **ID3v1**: Basic 128-byte structure with limited fields
- **ID3v2**: Extensible format supporting Unicode, multiple values, and custom frames
- **Common frames**: TIT2 (Title), TPE1 (Artist), TALB (Album), TDRC (Year), TCON (Genre)

### Vorbis Comments (OGG)

- **Format**: UTF-8 text in NAME=VALUE format
- **Standard fields**: TITLE, ARTIST, ALBUM, DATE, GENRE, TRACKNUMBER
- **Flexible**: Arbitrary field names and multiple values are allowed

### MP4 Metadata (M4A)

- **Atoms**: iTunes-style metadata stored in MP4 atoms
- **Common atoms**: ©nam (Title), ©ART (Artist), ©alb (Album), ©day (Year)
- **Binary data**: Embedded artwork goes in the `covr` atom

### ASF Attributes (WMV/WMA)

- **Structure**: Key-value pairs in the ASF header
- **Standard attributes**: Title, Author, Copyright, Description
- **Extended**: Custom attributes are supported

---

This guide covers writing audio metadata tags with the VisioForge Media Blocks SDK. The unified `MediaFileTags` class simplifies the code while keeping each container's native tag format (ID3, Vorbis Comments, MP4 atoms, ASF) intact. For more advanced audio-processing scenarios, explore the complete [VisioForge Media Blocks SDK documentation](../index.md).
