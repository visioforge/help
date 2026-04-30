---
title: Build RTSP Server in C# .NET — H.264/HEVC + GPU Encoding
description: Build an RTSP server with H.264/AAC encoding, GPU acceleration (NVENC, QSV, AMF), authentication, and latency tuning. VisioForge SDK cross-platform examples.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - Video Edit SDK
  - .NET
  - DirectShow
  - MediaBlocksPipeline
  - VideoCaptureCoreX
  - VideoEditCoreX
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - GStreamer
  - Capture
  - Streaming
  - Encoding
  - Editing
  - IP Camera
  - RTSP
  - ONVIF
  - UDP
  - WebRTC
  - MP4
  - H.264
  - H.265
  - AAC
  - C#
primary_api_classes:
  - VideoCaptureCoreX
  - VideoEditCoreX
  - RTSPServerOutput
  - RTSPServerSettings
  - RTSPServerBlock

---

# Mastering RTSP Streaming with VisioForge SDKs

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Video Edit SDK .Net](https://www.visioforge.com/video-edit-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

!!! info "Cross-platform support"
    The `VideoCaptureCoreX` engine and the Media Blocks SDK run on **Windows, macOS, Linux, Android, and iOS** via GStreamer. See the [platform support matrix](../../platform-matrix.md) for codec and hardware-acceleration details, and the [Linux deployment guide](../../deployment-x/Ubuntu.md) for Ubuntu / NVIDIA Jetson / Raspberry Pi setup.

!!! tip "AI coding agents: use the VisioForge MCP server"

    Building this with **Claude Code**, **Cursor**, or another AI coding agent?
    Connect to the public [VisioForge MCP server](../mcp-server-usage.md)
    at `https://mcp.visioforge.com/mcp` for structured API lookups, runnable
    code samples, and deployment guides — more accurate than grepping
    `llms.txt`. No authentication required.

    Claude Code: `claude mcp add --transport http visioforge-sdk https://mcp.visioforge.com/mcp`

## Introduction to RTSP

The Real-Time Streaming Protocol (RTSP) is a network control protocol designed for use in entertainment and communications systems to control streaming media servers. It acts like a "network remote control," allowing users to play, pause, and stop media streams. VisioForge SDKs harness the power of RTSP to provide robust video and audio streaming capabilities.

Our SDKs integrate RTSP with industry-standard codecs like **H.264 (AVC)** for video and **Advanced Audio Coding (AAC)** for audio. H.264 offers excellent video quality at relatively low bitrates, making it ideal for streaming over various network conditions. AAC provides efficient and high-fidelity audio compression. This powerful combination ensures reliable, high-definition audiovisual streaming suitable for demanding applications such as:

*   **Security and Surveillance:** Delivering clear, real-time video feeds from IP cameras.
*   **Live Broadcasting:** Streaming events, webinars, or performances to a wide audience.
*   **Video Conferencing:** Enabling smooth, high-quality communication.
*   **Remote Monitoring:** Observing industrial processes or environments remotely.

This guide delves into the specifics of implementing RTSP streaming using VisioForge SDKs, covering both modern cross-platform approaches and legacy Windows-specific methods.

## Cross-Platform RTSP Output (Recommended)

[VideoCaptureCoreX](#){ .md-button } [VideoEditCoreX](#){ .md-button } [MediaBlocksPipeline](#){ .md-button }

The modern VisioForge SDKs (`CoreX` versions and Media Blocks) provide a flexible and powerful cross-platform RTSP server implementation built upon the robust GStreamer framework. This approach offers greater control, wider codec support, and compatibility across Windows, Linux, macOS, and other platforms.

### Core Component: `RTSPServerOutput`

The `RTSPServerOutput` class is the central configuration point for establishing an RTSP stream within the Video Capture or Video Edit SDKs (`CoreX` versions). It acts as a bridge between your capture/edit pipeline and the underlying RTSP server logic.

**Key Responsibilities:**

*   **Interface Implementation:** Implements `IVideoEditXBaseOutput` and `IVideoCaptureXBaseOutput`, allowing seamless integration as an output format in both editing and capture scenarios.
*   **Settings Management:** Holds the `RTSPServerSettings` object, which contains all the detailed configuration parameters for the server instance.
*   **Codec Specification:** Defines the video and audio encoders that will be used to compress the media before streaming.

**Supported Encoders:**

VisioForge provides access to a wide array of encoders, allowing optimization based on hardware capabilities and target platforms:

*   **Video Encoders:**
    *   **Hardware-Accelerated (Recommended for performance):**
        *   `NVENC` (NVIDIA): Leverages dedicated encoding hardware on NVIDIA GPUs.
        *   `QSV` (Intel Quick Sync Video): Utilizes integrated GPU capabilities on Intel processors.
        *   `AMF` (AMD Advanced Media Framework): Uses encoding hardware on AMD GPUs/APUs.
    *   **Software-Based (Platform-independent, higher CPU usage):**
        *   `OpenH264`: A widely compatible H.264 software encoder.
        *   `VP8` / `VP9`: Royalty-free video codecs developed by Google, offering good compression (often used with WebRTC, but available here).
    *   **Platform-Specific:**
        *   `MF HEVC` (Media Foundation HEVC): Windows-specific H.265/HEVC encoder for higher efficiency compression.
*   **Audio Encoders:**
    *   **AAC Variants:**
        *   `VO-AAC`: A versatile, cross-platform AAC encoder.
        *   `AVENC AAC`: Utilizes FFmpeg's AAC encoder.
        *   `MF AAC`: Windows Media Foundation AAC encoder.
    *   **Other Formats:**
        *   `MP3`: Widely compatible but less efficient than AAC.
        *   `OPUS`: Excellent low-latency codec, ideal for interactive applications.

### Configuring the Stream: `RTSPServerSettings`

This class encapsulates all the parameters needed to define the behavior and properties of your RTSP server.

**Detailed Properties:**

*   **Network Configuration:**
    *   `Port` (int): The TCP port the server listens on for incoming RTSP connections. The default is `8554`, a common alternative to the standard (often restricted) port 554. Ensure this port is open in firewalls.
    *   `Address` (string): The IP address the server binds to.
        *   `"127.0.0.1"` (Default): Listens only for connections from the local machine.
        *   `"0.0.0.0"`: Listens on all available network interfaces (use for public access).
        *   Specific IP (e.g., `"192.168.1.100"`): Binds only to that specific network interface.
    *   `Point` (string): The path component of the RTSP URL (e.g., `/live`, `/stream1`). Clients will connect to `rtsp://<Address>:<Port><Point>`. Default is `"/live"`.
*   **Stream Configuration:**
    *   `VideoEncoder` (IVideoEncoder): An `IVideoEncoder` instance — typically an encoder-settings object (e.g., `OpenH264EncoderSettings`) or the result of `H264EncoderBlock.GetDefaultSettings()`. Defines codec, bitrate (Kbit/s), quality, etc.
    *   `AudioEncoder` (IAudioEncoder): An `IAudioEncoder` instance (e.g., `VOAACEncoderSettings`). Defines audio codec parameters.
    *   `Latency` (TimeSpan): Controls the buffering delay introduced by the server to smooth out network jitter. Default is 250 milliseconds. Higher values increase stability but also delay.
*   **Authentication:**
    *   `Username` (string): If set, clients must provide this username for basic authentication.
    *   `Password` (string): If set, clients must provide this password along with the username.
*   **Server Identity:**
    *   `Name` (string): A friendly name for the server, sometimes displayed by client applications.
    *   `Description` (string): A more detailed description of the stream content or server purpose.
*   **Convenience Property:**
    *   `URL` (string, read-only): Automatically composes the full RTSP connection URL from `Address`, `Port`, and `Point`. With `RTSPServerSettings` defaults (`Port = 8554`), the composed URL is `rtsp://127.0.0.1:8554/live`.

### The Engine: `RTSPServerBlock` (Media Blocks SDK)

When using the Media Blocks SDK, the `RTSPServerBlock` represents the actual GStreamer-based element that performs the streaming.

**Functionality:**

*   **Media Sink:** Acts as a terminal point (sink) in a media pipeline, receiving encoded video and audio data.
*   **Input Pads:** Provides distinct `VideoInput` and `AudioInput` pads for connecting upstream video and audio sources/encoders.
*   **GStreamer Integration:** Manages the underlying GStreamer `rtspserver` and related elements necessary for handling client connections and streaming RTP packets.
*   **Availability Check:** The static `IsAvailable()` method allows checking if the necessary GStreamer plugins for RTSP streaming are present on the system.
*   **Resource Management:** Implements `IDisposable` for proper cleanup of network sockets and GStreamer resources when the block is no longer needed.

### Practical Usage Examples

#### Example 1: Basic Server Setup (VideoCaptureCoreX / VideoEditCoreX)

```csharp
// 1. Choose and configure encoders

// Use hardware acceleration if available, otherwise fallback to software
var videoEncoder = H264EncoderBlock.GetDefaultSettings();

var audioEncoder = new VOAACEncoderSettings(); // Reliable cross-platform AAC

// 2. Configure server network settings
var settings = new RTSPServerSettings(videoEncoder, audioEncoder)
{
    Port = 8554,
    Address = "0.0.0.0",  // Accessible from other machines on the network
    Point = "/livefeed"
};

// 3. Create the output object
var rtspOutput = new RTSPServerOutput(settings);

// 4. Integrate with the SDK engine
// For VideoCaptureCoreX:
// videoCapture is an initialized instance of VideoCaptureCoreX
videoCapture.Outputs_Add(rtspOutput); 

// For VideoEditCoreX:
// videoEdit is an initialized instance of VideoEditCoreX
// videoEdit.Output_Format = rtspOutput; // Set before starting editing/playback
```

#### Example 2: Media Blocks Pipeline

```csharp
// Assume 'pipeline' is an initialized MediaBlocksPipeline
// Assume 'videoSource' and 'audioSource' provide unencoded media streams

// 1. Create video and audio encoder settings
var videoEncoder = H264EncoderBlock.GetDefaultSettings();

var audioEncoder = new VOAACEncoderSettings();

// 2. Create RTSP server settings with a specific URL
var serverUri = new Uri("rtsp://192.168.1.50:8554/cam1"); 
var rtspSettings = new RTSPServerSettings(serverUri, videoEncoder, audioEncoder)
{
    Description = "Camera Feed 1 - Warehouse"
};

// 3. Create the RTSP Server Block
if (!RTSPServerBlock.IsAvailable())
{
    Console.WriteLine("RTSP Server components not available. Check GStreamer installation.");
    return; 
}
var rtspSink = new RTSPServerBlock(rtspSettings);

// Connect source directly to RTSP server block, because server block will use its own encoders
pipeline.Connect(videoSource.Output, rtspSink.VideoInput); // Connect source directly to video input of RTSP server block
pipeline.Connect(audioSource.Output, rtspSink.AudioInput); // Connect source directly to audio input of RTSP server block

// Start the pipeline
await pipeline.StartAsync();
```

#### Example 3: Advanced Configuration with Authentication

```csharp
// Using settings from Example 1...
var secureSettings = new RTSPServerSettings(videoEncoder, audioEncoder)
{
    Port = 8555, // Use a different port
    Address = "192.168.1.100", // Bind to a specific internal IP
    Point = "/secure",
    Username = "viewer",
    Password = "VerySecretPassword!",
    Latency = TimeSpan.FromMilliseconds(400), // Slightly higher latency
    Name = "SecureStream",
    Description = "Authorized access only"
};

var secureRtspOutput = new RTSPServerOutput(secureSettings);

// Add to VideoCaptureCoreX or set for VideoEditCoreX as before
// videoCapture.Outputs_Add(secureRtspOutput); 
```

### Streaming Best Practices

1.  **Encoder Selection Strategy:**
    *   **Prioritize Hardware:** Always prefer hardware encoders (NVENC, QSV, AMF) when available on the target system. They drastically reduce CPU load, allowing for higher resolutions, frame rates, or more simultaneous streams.
    *   **Software Fallback:** Use `OpenH264` as a reliable software fallback for broad compatibility when hardware acceleration isn't present or suitable.
    *   **Codec Choice:** H.264 remains the most widely compatible codec for RTSP clients. HEVC offers better compression but client support might be less universal.
2.  **Latency Tuning:**
    *   **Interactivity vs. Stability:** Lower latency (e.g., 100-200ms) is crucial for applications like video conferencing but makes the stream more susceptible to network hiccups.
    *   **Broadcast/Surveillance:** Higher latency (e.g., 500ms-1000ms+) provides larger buffers, improving stream resilience over unstable networks (like Wi-Fi or the internet) at the cost of increased delay. Start with the default (250ms) and adjust based on observed stream quality and requirements.
3.  **Network Configuration:**
    *   **Security First:** Implement `Username` and `Password` authentication for any stream not intended for public anonymous access.
    *   **Binding Address:** Use `"0.0.0.0"` cautiously. For enhanced security, bind explicitly to the network interface (`Address`) intended for client connections if possible.
    *   **Firewall Rules:** Meticulously configure system and network firewalls to allow incoming TCP connections on the chosen RTSP `Port`. Also, remember that RTP/RTCP (used for the actual media data) often use dynamic UDP ports; firewalls might need helper modules (like `nf_conntrack_rtsp` on Linux) or broad UDP port ranges opened (less secure).
4.  **Resource Management:**
    *   **Dispose Pattern:** RTSP server instances hold network resources (sockets) and potentially complex GStreamer pipelines. *Always* ensure they are disposed of correctly using `using` statements or explicit `.Dispose()` calls in `finally` blocks to prevent resource leaks.
    *   **Graceful Shutdown:** When stopping the capture or edit process, ensure the output is properly removed or the pipeline is stopped cleanly to allow the RTSP server to shut down gracefully.

### Performance Considerations

Optimizing RTSP streaming involves balancing quality, latency, and resource usage:

1.  **Encoder Impact:** This is often the biggest factor.
    *   **Hardware:** Significantly lower CPU usage, higher potential throughput. Requires compatible hardware and drivers.
    *   **Software:** High CPU load, especially at higher resolutions/framerates. Limits the number of concurrent streams on a single machine but works universally.
2.  **Latency vs. Bandwidth:** Lower latency settings can sometimes lead to increased peak bandwidth usage as the system has less time to smooth out data transmission.
3.  **Resource Monitoring:**
    *   **CPU:** Keep a close eye on CPU usage, particularly with software encoders. Overload leads to dropped frames and stuttering.
    *   **Memory:** Monitor RAM usage, especially if handling multiple streams or complex Media Blocks pipelines.
    *   **Network:** Ensure the server's network interface has sufficient bandwidth for the configured bitrate, resolution, and number of connected clients. Calculate required bandwidth (Video Bitrate + Audio Bitrate) * Number of Clients.

## Windows-Only RTSP Output (Legacy)

[VideoCaptureCore](#){ .md-button } [VideoEditCore](#){ .md-button }

The implementation includes several error handling mechanisms:

Older versions of the SDK (`VideoCaptureCore`, `VideoEditCore`) included a simpler, Windows-specific RTSP output mechanism. While functional, it offers less flexibility and codec support compared to the cross-platform `RTSPServerOutput`. **It is generally recommended to use the `CoreX` / Media Blocks approach for new projects.**

### How it Works

This method leverages built-in Windows components or specific bundled filters. Configuration is done directly via properties on the `VideoCaptureCore` or `VideoEditCore` object.

### Sample Configuration Code

```csharp
using VisioForge.Core.Types.Output;           // MP4Output, NetworkStreamingFormat, H264Profile
using VisioForge.Core.Types.VideoCapture;
using VisioForge.Core.VideoCapture;           // VideoCaptureCore

// Assuming VideoCapture1 is an instance of VideoCaptureCore already bound to a VideoView.

// 1. Enable network streaming globally for the component
VideoCapture1.Network_Streaming_Enabled = true;

// 2. Enable audio streaming (optional)
VideoCapture1.Network_Streaming_Audio_Enabled = true;

// 3. Select the RTSP format.
//    RTSP_H264_AAC_SW = software H.264 + AAC (DirectShow-based Windows RTSP).
VideoCapture1.Network_Streaming_Format = NetworkStreamingFormat.RTSP_H264_AAC_SW;

// 4. Configure encoder settings. MP4Output holds the H.264 and AAC parameters
//    even though the stream is sent over RTSP rather than written to a file.
var mp4Output = new MP4Output();

//    H.264 settings live on MP4Output.Video (MP4OutputH264Settings).
mp4Output.Video = new MP4OutputH264Settings
{
    Bitrate = 2000,                // kbps
    Profile = H264Profile.ProfileMain,
    Level = H264Level.Level4,
    RateControl = H264RateControl.VBR
};

//    AAC settings live on MP4Output.Audio_AAC (M4AOutput — already initialised in the constructor).
mp4Output.Audio_AAC.Bitrate = 128;      // kbps
mp4Output.Audio_AAC.Object = AACObject.Low;
mp4Output.Audio_AAC.Version = AACVersion.MPEG4;

// 5. Assign the settings container to the network streaming output
VideoCapture1.Network_Streaming_Output = mp4Output;

// 6. Define the RTSP URL clients will use.
//    The server listens on the port specified in the URL (5554 here).
VideoCapture1.Network_Streaming_URL = "rtsp://localhost:5554/vfstream";
//    Use the machine's actual IP instead of localhost for external access.

// 7. Start as usual (OnError fires if the port is busy or encoders are unavailable).
await VideoCapture1.StartAsync();
```

**Note:** This legacy method often relies on DirectShow filters or Media Foundation transforms available on the specific Windows system, making it less predictable and portable than the GStreamer-based cross-platform solution.

## See also

* [RTSP reconnection and fallback switch](../network-sources/reconnection-and-fallback.md) — handle camera disconnects with reconnect events, `DisconnectEventInterval`, and the declarative `FallbackSwitch` (image / text / media fallback) across all VisioForge SDKs
* [RTSP camera source configuration in C#](../../videocapture/video-sources/ip-cameras/rtsp.md) — `IPCameraSourceSettings` and `RTSPSourceSettings` reference with UDP/TCP transport and buffer tuning
* [ONVIF IP camera integration](../../videocapture/video-sources/ip-cameras/onvif.md) — WS-Discovery, profile selection, and PTZ control for standards-based cameras
* [IP camera live preview tutorial](../../videocapture/video-tutorials/ip-camera-preview.md) — video walkthrough for a minimal preview implementation
* [Record RTSP stream to MP4](../../videocapture/video-tutorials/ip-camera-capture-mp4.md) — capture any IP camera to MP4 file in .NET
* [Media Blocks RTSP player](../../mediablocks/Guides/rtsp-player-csharp.md) — pipeline-based RTSP playback with the Media Blocks SDK
* [RTSP server output block](../../mediablocks/RTSPServer/index.md) — broadcast your own video and audio stream over RTSP
* [Multi-camera RTSP grid (NVR wall)](../../mediablocks/Guides/multi-camera-rtsp-grid.md) — build a 4×4 live preview wall with WPF or MAUI, with synchronized start
* [Pre-Event Recording with IP Camera](../../mediablocks/Guides/pre-event-recording.md) — record RTSP streams with circular buffer and motion detection triggers
* [Save Original RTSP Stream](../../mediablocks/Guides/rtsp-save-original-stream.md) — save RTSP video without re-encoding
* [Barcode & QR Code Scanner](../../mediablocks/Guides/barcode-qr-reader-guide.md) — scan barcodes from RTSP IP camera streams in real time

---
For more detailed examples and advanced use cases, explore the code samples provided in our [GitHub repository](https://github.com/visioforge/.Net-SDK-s-samples). For brand-specific RTSP URLs, see our [IP camera brands directory](../../camera-brands/index.md).
