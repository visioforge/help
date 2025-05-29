---
title: NDI Network Video Streaming Integration Guide
description: Learn how to implement high-performance NDI streaming in .NET applications. Step-by-step guide for developers to set up low-latency video/audio transmission over IP networks with code examples and best practices.
sidebar_label: NDI

---

# Network Device Interface (NDI) Streaming Integration

[!badge size="xl" target="blank" variant="info" text="Video Capture SDK .Net"](https://www.visioforge.com/video-capture-sdk-net) [!badge size="xl" target="blank" variant="info" text="Media Blocks SDK .Net"](https://www.visioforge.com/media-blocks-sdk-net)

## What is NDI and Why Use It?

The VisioForge SDK's integration of Network Device Interface (NDI) technology provides a transformative solution for professional video production and broadcasting workflows. NDI has emerged as a leading industry standard for live production, enabling high-quality, ultra-low-latency video streaming over conventional Ethernet networks.

NDI significantly simplifies the process of sharing and managing multiple video streams across diverse devices and platforms. When implemented within the VisioForge SDK, it facilitates seamless transmission of high-definition video and audio content from servers to clients with exceptional performance characteristics. This makes the technology particularly valuable for applications including:

- Live broadcasting and streaming
- Professional video conferencing
- Multi-camera production setups
- Remote production workflows
- Educational and corporate presentation environments

The inherent flexibility and efficiency of NDI streaming technology substantially reduces dependency on specialized hardware configurations, delivering a cost-effective alternative to traditional SDI-based systems for professional-grade live video production.

## Installation Requirements

### Prerequisites for NDI Implementation

To successfully implement NDI streaming functionality within your application, you must install one of the following official NDI software packages:

1. **[NDI SDK](https://ndi.video/download-ndi-sdk/)** - Recommended for developers who need comprehensive access to NDI functionality
2. **[NDI Tools](https://ndi.video/tools/)** - Suitable for basic implementation and testing scenarios

These packages provide the necessary runtime components that enable NDI communication across your network infrastructure.

## Cross-Platform NDI Output Implementation

[!badge variant="dark" size="xl" text="VideoCaptureCoreX"] [!badge variant="dark" size="xl" text="VideoEditCoreX"] [!badge variant="dark" size="xl" text="MediaBlocksPipeline"]

### Understanding the NDIOutput Class Architecture

The `NDIOutput` class serves as the core implementation framework for NDI functionality within the VisioForge SDK ecosystem. This class encapsulates configuration properties and processing logic required for high-performance video-over-IP transmission using the NDI protocol. The architecture enables broadcast-quality video and audio transmission across standard network infrastructure without specialized hardware requirements.

#### Class Definition and Interface Implementation

```csharp
public class NDIOutput : IVideoEditXBaseOutput, IVideoCaptureXBaseOutput, IOutputVideoProcessor, IOutputAudioProcessor
```

The class implements several interfaces that provide comprehensive functionality for different output scenarios:

- `IVideoEditXBaseOutput` - Provides integration with video editing workflows
- `IVideoCaptureXBaseOutput` - Enables direct capture-to-NDI streaming capabilities
- `IOutputVideoProcessor` - Allows for advanced video processing during output
- `IOutputAudioProcessor` - Facilitates audio processing and manipulation in the NDI pipeline

### Configuration Properties

#### Video Processing Pipeline

```csharp
public MediaBlock CustomVideoProcessor { get; set; }
```

This property allows developers to extend the NDI streaming pipeline with custom video processing functionality. By assigning a custom `MediaBlock` implementation, you can integrate specialized video filters, transformations, or analysis algorithms before content is transmitted via NDI.

#### Audio Processing Pipeline

```csharp
public MediaBlock CustomAudioProcessor { get; set; }
```

Similar to the video processor property, this allows for insertion of custom audio processing logic. Common applications include dynamic audio level adjustment, noise reduction, or specialized audio effects that enhance the streaming experience.

#### NDI Sink Configuration

```csharp
public NDISinkSettings Sink { get; set; }
```

This property contains the comprehensive configuration parameters for the NDI output sink, including essential settings such as stream identification, compression options, and network transmission parameters.

### Constructor Overloads

#### Basic Constructor with Stream Name

```csharp
public NDIOutput(string name)
```

Creates a new NDI output instance with the specified stream name, which will identify this NDI source on the network.

**Parameters:**

- `name`: String identifier for the NDI stream visible to receivers on the network

#### Advanced Constructor with Pre-configured Settings

```csharp
public NDIOutput(NDISinkSettings settings)
```

Creates a new NDI output instance with comprehensive pre-configured sink settings for advanced implementation scenarios.

**Parameters:**

- `settings`: A fully configured `NDISinkSettings` object containing all required NDI configuration parameters

### Core Methods

#### Stream Identification

```csharp
public string GetFilename()
```

Returns the configured name of the NDI stream. This method maintains compatibility with file-based output interfaces in the SDK architecture.

**Returns:** The current NDI stream identifier

```csharp
public void SetFilename(string filename)
```

Updates the NDI stream identifier. This method is primarily used for compatibility with other output types that use filename-based identification.

**Parameters:**

- `filename`: The updated name for the NDI stream

#### Encoder Management

```csharp
public Tuple<string, Type>[] GetVideoEncoders()
```

Returns an empty array as NDI handles video encoding internally through its proprietary technology.

**Returns:** Empty array of encoder tuples

```csharp
public Tuple<string, Type>[] GetAudioEncoders()
```

Returns an empty array as NDI handles audio encoding internally through its proprietary technology.

**Returns:** Empty array of encoder tuples

## Implementation Examples

### Media Blocks SDK Implementation

The following example demonstrates how to configure an NDI output using the Media Blocks SDK architecture:

```cs
// Create an NDI output block with a descriptive stream name
var ndiSink = new NDISinkBlock("VisioForge Production Stream");

// Connect video source to the NDI output
// CreateNewInput method establishes a video input channel for the NDI sink
pipeline.Connect(videoSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Video));

// Connect audio source to the NDI output
// CreateNewInput method establishes an audio input channel for the NDI sink
pipeline.Connect(audioSource.Output, ndiSink.CreateNewInput(MediaBlockPadMediaType.Audio));
```

### Video Capture SDK Implementation

This example shows how to integrate NDI streaming within the Video Capture SDK framework:

```cs
// Initialize NDI output with a network-friendly stream name
var ndiOutput = new NDIOutput("VisioForge_Studio_Output");

// Add the configured NDI output to the video capture pipeline
core.Outputs_Add(ndiOutput); // core represents the VideoCaptureCoreX instance
```

## Windows-Specific NDI Implementation

[!badge variant="dark" size="xl" text="VideoCaptureCore"] [!badge variant="dark" size="xl" text="VideoEditCore"]

For Windows-specific implementations, the SDK provides additional configuration options through the VideoCaptureCore or VideoEditCore components.

### Step-by-Step Implementation Guide

#### 1. Enable Network Streaming

First, activate the network streaming functionality:

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

#### 2. Configure Audio Streaming

Enable audio transmission alongside video content:

```cs
VideoCapture1.Network_Streaming_Audio_Enabled = true;
```

#### 3. Select NDI Protocol

Specify NDI as the preferred streaming format:

```csharp
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.NDI;
```

#### 4. Create and Configure NDI Output

Initialize the NDI output with a descriptive name:

```cs
var streamName = "VisioForge NDI Streamer";
var ndiOutput = new NDIOutput(streamName); 
```

#### 5. Assign the Output

Connect the configured NDI output to the video capture pipeline:

```cs
VideoCapture1.Network_Streaming_Output = ndiOutput;
```

#### 6. Generate the NDI URL (Optional)

For debugging or sharing purposes, you can generate the standard NDI protocol URL:

```cs
string ndiUrl = $"ndi://{System.Net.Dns.GetHostName()}/{streamName}";
Debug.WriteLine(ndiUrl);
```

## Advanced Integration Considerations

When implementing NDI streaming in production environments, consider the following factors:

- **Network bandwidth requirements** - NDI streams can consume significant bandwidth depending on resolution and framerate
- **Quality vs. latency tradeoffs** - Configure appropriate compression settings based on your specific use case
- **Multicast vs. unicast distribution** - Determine the optimal network transmission method based on your infrastructure
- **Hardware acceleration options** - Leverage GPU acceleration where available for improved performance
- **Discovery mechanism** - Consider how NDI sources will be discovered across network segments

## Related Components

- **NDISinkSettings** - Provides detailed configuration options for the NDI output sink
- **NDISinkBlock** - Implements the core NDI output functionality referenced in NDISinkSettings
- **MediaBlockPadMediaType** - Enum used to specify the type of media (video or audio) for input connections

---

Visit our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples) for additional code samples and implementation examples.
