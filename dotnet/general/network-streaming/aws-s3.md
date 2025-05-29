---
title: Stream Video and Audio to Amazon S3 Storage
description: Learn how to implement AWS S3 video and audio streaming in .NET applications. Step-by-step guide for developers covering configuration, encoding settings, error handling, and best practices for S3 media output integration.
sidebar_label: AWS S3

---

# AWS S3 Output

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Video Edit SDK .Net"](https://www.visioforge.com/video-edit-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

The AWS S3 Output functionality in VisioForge SDKs enables direct video and audio output streaming to Amazon S3 storage. This guide will walk you through setting up and using AWS S3 output in your applications.

## Overview

The `AWSS3Output` class is a specialized output handler within the VisioForge SDKs that facilitates video and audio output streaming to Amazon Web Services (AWS) S3 storage. This class implements multiple interfaces to support both video editing (`IVideoEditXBaseOutput`) and video capture (`IVideoCaptureXBaseOutput`) scenarios, along with processing capabilities for both video and audio content.

## Class Implementation

```csharp
public class AWSS3Output : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

## Key Features

The `AWSS3Output` class provides a comprehensive solution for streaming media content to AWS S3 by managing:

- Video encoding configuration
- Audio encoding configuration
- Custom media processing
- AWS S3-specific settings

## Properties

### Video Encoder Settings

```csharp
public IVideoEncoder Video { get; set; }
```

Controls the video encoding process. The selected video encoder must be compatible with the configured sink settings. This property allows you to specify compression methods, quality settings, and other video-specific parameters.

### Audio Encoder Settings

```csharp
public IAudioEncoder Audio { get; set; }
```

Manages audio encoding configuration. Like the video encoder, the audio encoder must be compatible with the sink settings. This property enables control over audio quality, compression, and format settings.

### Sink Settings

```csharp
public IMediaBlockSettings Sink { get; set; }
```

Defines the output destination configuration. In this context, it contains AWS S3-specific settings for the media output stream.

### Custom Processing Blocks

```csharp
public MediaBlock CustomVideoProcessor { get; set; }
```

```csharp
public MediaBlock CustomAudioProcessor { get; set; }
```

Allow for additional processing of video and audio streams before they are encoded and uploaded to S3. These blocks can be used for implementing custom filters, transformations, or analysis of the media content.

### AWS S3 Configuration

```csharp
public AWSS3SinkSettings Settings { get; set; }
```

Contains all AWS S3-specific configuration options, including:

- Access credentials (Access Key, Secret Access Key)
- Bucket and object key information
- Region configuration
- Upload behavior settings
- Error handling preferences

## Constructor

```csharp
public AWSS3Output(AWSS3SinkSettings settings, 
                   IVideoEncoder videoEnc, 
                   IAudioEncoder audioEnc, 
                   IMediaBlockSettings sink)
```

Creates a new instance of the `AWSS3Output` class with the specified configuration:

- `settings`: AWS S3-specific configuration
- `videoEnc`: Video encoder settings
- `audioEnc`: Audio encoder settings
- `sink`: Media sink configuration

## Methods

### File Management

```csharp
public string GetFilename()
```

```csharp
public void SetFilename(string filename)
```

These methods manage the URI of the S3 object:

- `GetFilename()`: Returns the current S3 URI
- `SetFilename(string filename)`: Sets the S3 URI for the output

### Encoder Support

All encoders are supported. Be sure that encoder settings are compatible with the sink settings.

## Usage Example

```csharp
// Create AWS S3 sink settings
var s3Settings = new AWSS3SinkSettings
{
    AccessKey = "your-access-key",
    SecretAccessKey = "your-secret-key",
    Bucket = "your-bucket-name",
    Key = "output-file-key",
    Region = "us-west-1"
};

// Configure encoders
IVideoEncoder videoEncoder = /* your video encoder configuration */;
IAudioEncoder audioEncoder = /* your audio encoder configuration */;
IMediaBlockSettings sinkSettings = /* your sink settings */;

// Create the AWS S3 output
var s3Output = new AWSS3Output(s3Settings, videoEncoder, audioEncoder, sinkSettings);

// Optional: Configure custom processors
s3Output.CustomVideoProcessor = /* your custom video processor */;
s3Output.CustomAudioProcessor = /* your custom audio processor */;
```

## Best Practices

1. Always ensure your AWS credentials are properly secured and not hard-coded in the application.
2. Configure appropriate retry attempts and request timeouts based on your network conditions and file sizes.
3. Select compatible video and audio encoders for your target use case.
4. Consider implementing custom processors for specific requirements like watermarking or audio normalization.

## Error Handling

The class works in conjunction with the `S3SinkOnError` enumeration defined in `AWSS3SinkSettings`, which provides three error handling strategies:

- Abort: Stops the upload process on error
- Complete: Attempts to complete the upload despite errors
- DoNothing: Ignores errors during upload

## Related Components

- AWSS3SinkSettings: Contains detailed configuration for AWS S3 connectivity
- IVideoEncoder: Interface for video encoding configuration
- IAudioEncoder: Interface for audio encoding configuration
- IMediaBlockSettings: Interface for media output configuration
