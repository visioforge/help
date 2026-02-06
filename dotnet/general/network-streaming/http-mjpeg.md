---
title: HTTP MJPEG Video Streaming Implementation Guide
description: Implement HTTP MJPEG streaming in .NET for real-time video feeds with client connection handling and adjustable frame rates for web streaming.
---

# HTTP MJPEG streaming

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

The SDK's feature of streaming video encoded as Motion JPEG (MJPEG) over HTTP is advantageous for its simplicity and broad compatibility. MJPEG encodes each video frame individually as a JPEG image, which simplifies decoding and is ideal for applications like web streaming and surveillance. The use of HTTP ensures easy integration and high compatibility across different platforms and devices and is effective even in networks with strict configurations. This method is particularly suitable for real-time video feeds and applications requiring straightforward frame-by-frame analysis. With adjustable frame rates and resolutions, the SDK offers flexibility for various network conditions and quality requirements, making it a versatile choice for developers implementing video streaming in their applications.

## Cross-platform MJPEG output

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

The streaming functionality is implemented through two main classes:

1. `HTTPMJPEGLiveOutput`: The high-level configuration class that sets up the streaming output
2. `HTTPMJPEGLiveSinkBlock`: The underlying implementation block that handles the actual streaming process

### HTTPMJPEGLiveOutput Class

This class serves as the configuration entry point for setting up an MJPEG HTTP stream. It implements the `IVideoCaptureXBaseOutput` interface, making it compatible with the video capture pipeline system.

#### Key Properties

- `Port`: Gets the network port number on which the MJPEG stream will be served

#### Usage

```csharp
// Create a new MJPEG streaming output on port 8080
var mjpegOutput = new HTTPMJPEGLiveOutput(8080);

// Add the MJPEG output to the VideoCaptureCoreX engine
core.Outputs_Add(mjpegOutput, true);
```

#### Implementation Details

- The class is designed to be immutable, with the port being set only through the constructor
- It does not support video or audio encoders, as MJPEG uses direct JPEG encoding
- The filename-related methods return null or are no-ops, as this is a streaming-only implementation

### HTTPMJPEGLiveSinkBlock Class

This class handles the actual implementation of the MJPEG streaming functionality. It's responsible for:

- Setting up the pipeline for video processing
- Managing the HTTP server for streaming
- Handling input video data and converting it to MJPEG format
- Managing client connections and stream delivery

#### Key Features

- Implements multiple interfaces for integration with the media pipeline:
  - `IMediaBlockInternals`: For pipeline integration
  - `IMediaBlockDynamicInputs`: For handling dynamic input connections
  - `IMediaBlockSink`: For sink functionality
  - `IDisposable`: For proper resource cleanup

#### Input/Output Configuration

- Accepts a single video input through the `Input` pad
- No output pads (as it's a sink block)
- Input pad configured for video media type only

### Implementation Notes

#### Initialization

```csharp
// The block must be initialized with a port number
var mjpegSink = new HTTPMJPEGLiveSinkBlock(8080);
pipeline.Connect(videoSource.Output, mjpegSink.Input);

// "IMG tag URL is http://127.0.0.1:8090";
```

#### Resource Management

- The class implements proper resource cleanup through the `IDisposable` pattern
- The `CleanUp` method ensures all resources are properly released
- Event handlers are properly connected and disconnected during the pipeline lifecycle

#### Pipeline Integration

The `Build` method handles the critical setup process:

1. Creates the underlying HTTP MJPEG sink element
2. Initializes the sink with the specified port
3. Sets up the necessary GStreamer pad connections
4. Connects pipeline event handlers

### Error Handling

- The implementation includes comprehensive error checking during the build process
- Failed initialization is properly reported through the context error system
- Resource cleanup is handled even in error cases

### Technical Considerations

#### Performance

- The implementation uses GStreamer's native elements for optimal performance
- Direct pad connections minimize copying and overhead
- The sink block is designed to handle multiple client connections efficiently

#### Memory Management

- Proper disposal patterns ensure no memory leaks
- Resources are cleaned up when the pipeline stops or the block is disposed
- The implementation handles GStreamer element lifecycle correctly

#### Threading

- The implementation is thread-safe for pipeline operations
- Event handlers are properly synchronized with pipeline state changes
- Client connections are handled asynchronously

#### Client Usage

To consume the MJPEG stream:

1. Initialize the streaming output with desired port
2. Connect it to your video pipeline
3. Access the stream through a web browser or HTTP client at:
   ```
   http://[server-address]:[port]
   ```

#### Example Client HTML

```html
<img src="http://localhost:8080" />
```

### Limitations and Considerations

1. Bandwidth Usage
   - MJPEG streams can use significant bandwidth as each frame is a complete JPEG
   - Consider frame rate and resolution settings for optimal performance

2. Browser Support
   - While MJPEG is widely supported, some modern browsers may have limitations
   - Mobile devices may handle MJPEG streams differently

3. Latency
   - While MJPEG provides relatively low latency, it's not suitable for ultra-low-latency requirements
   - Network conditions can affect frame delivery timing

### Best Practices

1. Port Selection
   - Choose ports that don't conflict with other services
   - Consider firewall implications when selecting ports

2. Resource Management
   - Always dispose of the sink block properly
   - Monitor client connections and resource usage

3. Error Handling
   - Implement proper error handling for network and pipeline issues
   - Monitor the pipeline status for potential issues

### Security Considerations

1. Network Security
   - The MJPEG stream is unencrypted by default
   - Consider implementing additional security measures for sensitive content

2. Access Control
   - No built-in authentication mechanism
   - Consider implementing application-level access control if needed

3. Port Security
   - Ensure proper firewall rules are in place
   - Consider network isolation for internal streams

## Windows-only MJPEG output

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

Set the `Network_Streaming_Enabled` property to true to enable network streaming.

```cs
VideoCapture1.Network_Streaming_Enabled = true;
```

Set the HTTP MJPEG output.

```cs
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.HTTP_MJPEG;
```

Create the settings object and set the port.

```cs
VideoCapture1.Network_Streaming_Output = new MJPEGOutput(8080);
```

---
Visit our [GitHub](https://github.com/visioforge/.Net-SDK-s-samples) page to get more code samples.