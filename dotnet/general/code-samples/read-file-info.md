---
title: Reading Media File Information in C# for Developers
description: Learn how to extract detailed information from video and audio files in C# with step-by-step code examples. Discover how to access codecs, resolution, frame rate, bitrate, and metadata tags for building robust media applications.
sidebar_label: Reading Media File Information
---

# Reading Media File Information in C#

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Player SDK .Net"](https://www.visioforge.com/media-player-sdk-net)

## Introduction

Accessing detailed information embedded within media files is essential for developing sophisticated applications like media players, video editors, content management systems, and file analysis tools. Understanding properties such as codecs, resolution, frame rate, bitrate, duration, and embedded tags allows developers to build more intelligent and user-friendly software.

This guide demonstrates how to read comprehensive information from video and audio files using C# and the `MediaInfoReader` class. The techniques shown are applicable across various .NET projects and provide a foundation for handling media files programmatically.

## Why Extract Media File Information?

Media file information serves multiple purposes in application development:

- **User Experience**: Display technical details to users in media players
- **Compatibility Checks**: Verify if files meet required specifications
- **Automated Processing**: Configure encoding parameters based on source properties
- **Content Organization**: Catalog media libraries with accurate metadata
- **Quality Assessment**: Evaluate media files for potential issues

## Implementation Guide

Let's explore the process of extracting media file information in a step-by-step approach. The examples assume a WinForms application with a `TextBox` control named `mmInfo` for displaying the extracted information.

### Step 1: Initialize the Media Information Reader

The first step involves creating an instance of the `MediaInfoReader` class:

```csharp
// Import the necessary namespace
using VisioForge.Core.MediaInfo; // Namespace for MediaInfoReader
using VisioForge.Core.Helpers;  // Namespace for TagLibHelper (optional)

// Create an instance of MediaInfoReader
var infoReader = new MediaInfoReader();
```

This initialization prepares the reader to process media files.

### Step 2: Verify File Playability (Optional)

Before diving into detailed analysis, it's often useful to check if the file is supported:

```csharp
// Define variables to hold potential error information
FilePlaybackError errorCode;
string errorText;

// Specify the path to the media file
string filename = @"C:\path\to\your\mediafile.mp4"; // Replace with your actual file path

// Check if the file is playable
if (MediaInfoReader.IsFilePlayable(filename, out errorCode, out errorText))
{
    // Display success message
    mmInfo.Text += "Status: This file appears to be playable." + Environment.NewLine;
}
else
{
    // Display error message including the error code and description
    mmInfo.Text += $"Status: This file might not be playable. Error: {errorCode} - {errorText}" + Environment.NewLine;
}

mmInfo.Text += "------------------------------------" + Environment.NewLine;
```

This verification provides early feedback on file integrity and compatibility.

### Step 3: Extract Detailed Stream Information

Now we can extract the rich metadata from the file:

```csharp
try
{
    // Assign the filename to the reader
    infoReader.Filename = filename;

    // Read the file information (true for full analysis)
    infoReader.ReadFileInfo(true);

    // Process Video Streams
    mmInfo.Text += $"Found {infoReader.VideoStreams.Count} video stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.VideoStreams.Count; i++)
    {
        var stream = infoReader.VideoStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Video Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Duration: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Dimensions: {stream.Width}x{stream.Height}" + Environment.NewLine;
        mmInfo.Text += $"  FOURCC: {stream.FourCC}" + Environment.NewLine;
        
        if (stream.AspectRatio != null && stream.AspectRatio.Item1 > 0 && stream.AspectRatio.Item2 > 0)
        {
             mmInfo.Text += $"  Aspect Ratio: {stream.AspectRatio.Item1}:{stream.AspectRatio.Item2}" + Environment.NewLine;
        }
        
        mmInfo.Text += $"  Frame Rate: {stream.FrameRate:F2} fps" + Environment.NewLine;
        mmInfo.Text += $"  Bitrate: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Frames Count: {stream.FramesCount}" + Environment.NewLine;
    }

    // Process Audio Streams
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Found {infoReader.AudioStreams.Count} audio stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.AudioStreams.Count; i++)
    {
        var stream = infoReader.AudioStreams[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Audio Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Codec Info: {stream.CodecInfo}" + Environment.NewLine;
        mmInfo.Text += $"  Duration: {stream.Duration}" + Environment.NewLine;
        mmInfo.Text += $"  Bitrate: {stream.Bitrate / 1000.0:F0} kbps" + Environment.NewLine;
        mmInfo.Text += $"  Channels: {stream.Channels}" + Environment.NewLine;
        mmInfo.Text += $"  Sample Rate: {stream.SampleRate} Hz" + Environment.NewLine;
        mmInfo.Text += $"  Bits Per Sample (BPS): {stream.BPS}" + Environment.NewLine;
        mmInfo.Text += $"  Language: {stream.Language}" + Environment.NewLine;
    }

    // Process Subtitle Streams
    mmInfo.Text += Environment.NewLine;
    mmInfo.Text += $"Found {infoReader.Subtitles.Count} subtitle stream(s)." + Environment.NewLine;
    
    for (int i = 0; i < infoReader.Subtitles.Count; i++)
    {
        var stream = infoReader.Subtitles[i];

        mmInfo.Text += Environment.NewLine;
        mmInfo.Text += $"--- Subtitle Stream #{i + 1} ---" + Environment.NewLine;
        mmInfo.Text += $"  Codec/Format: {stream.Codec}" + Environment.NewLine;
        mmInfo.Text += $"  Name: {stream.Name}" + Environment.NewLine;
        mmInfo.Text += $"  Language: {stream.Language}" + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Handle potential errors during file reading
    mmInfo.Text += $"{Environment.NewLine}Error reading file info: {ex.Message}{Environment.NewLine}";
}
finally
{
    // Important: Dispose the reader to release file handles and resources
    infoReader.Dispose();
}
```

The code iterates through each collection (`VideoStreams`, `AudioStreams`, and `Subtitles`), extracting and displaying relevant information for every stream found.

### Step 4: Extract Metadata Tags

Beyond technical stream information, media files often contain metadata tags:

```csharp
// Read Metadata Tags
mmInfo.Text += Environment.NewLine + "--- Metadata Tags ---" + Environment.NewLine;
try
{
    // Use TagLibHelper to read tags from the file
    var tags = TagLibHelper.ReadTags(filename);

    // Check if tags were successfully read
    if (tags != null)
    {
        mmInfo.Text += $"Title: {tags.Title}" + Environment.NewLine;
        mmInfo.Text += $"Artist(s): {string.Join(", ", tags.Performers ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Album: {tags.Album}" + Environment.NewLine;
        mmInfo.Text += $"Year: {tags.Year}" + Environment.NewLine;
        mmInfo.Text += $"Genre: {string.Join(", ", tags.Genres ?? new string[0])}" + Environment.NewLine;
        mmInfo.Text += $"Comment: {tags.Comment}" + Environment.NewLine;
    }
    else
    {
        mmInfo.Text += "No standard metadata tags found or readable." + Environment.NewLine;
    }
}
catch (Exception ex)
{
    // Handle errors during tag reading
    mmInfo.Text += $"Error reading tags: {ex.Message}" + Environment.NewLine;
}
```

## Best Practices for Media File Analysis

When implementing media file analysis in your applications, consider these best practices:

### Error Handling

Always wrap file operations in appropriate try-catch blocks. Media files can be corrupted, inaccessible, or in unexpected formats, which might cause exceptions.

```csharp
try {
    // Media file operations
}
catch (Exception ex) {
    // Log error and provide user feedback
}
```

### Resource Management

Properly dispose of objects that access file resources to prevent file locking issues:

```csharp
using (var infoReader = new MediaInfoReader())
{
    // Use the reader
}
// Or manually in a finally block
try {
    // Operations
}
finally {
    infoReader.Dispose();
}
```

### Performance Considerations

For large media libraries, consider:

1. Implementing caching mechanisms for repeated analysis
2. Using background threads for processing to keep UI responsive
3. Limiting the depth of analysis for initial quick scans

## Required Components

For successful implementation, ensure your project includes the necessary dependencies as specified in the SDK documentation.

## Conclusion

Extracting information from media files is a powerful capability for developers building applications that work with audio and video content. With the techniques outlined in this guide, you can access detailed technical properties and metadata tags to enhance your application's functionality.

The `MediaInfoReader` class provides a convenient and efficient way to extract the necessary metadata, allowing you to build more sophisticated media handling features in your C# applications.

For more advanced scenarios, explore the full capabilities of the SDK and consult the detailed documentation. You can find additional code samples and examples on GitHub to further expand your media file processing capabilities.
