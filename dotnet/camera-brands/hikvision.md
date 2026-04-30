---
title: Hikvision RTSP URL in C# .NET — IP Camera and NVR Guide
description: Hikvision RTSP URL format for DS-2CD, DS-2DE, and NVR models in C# .NET. ONVIF discovery, multi-channel streams, and VisioForge SDK integration guide.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - Windows
  - macOS
  - Linux
  - Android
  - iOS
  - Capture
  - Streaming
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#
primary_api_classes:
  - RTSPSourceProtocol

---

# How to Connect to Hikvision IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Hikvision** (Hangzhou Hikvision Digital Technology Co., Ltd.) is the world's largest manufacturer of video surveillance equipment by market share. Founded in 2001 and headquartered in Hangzhou, China, Hikvision produces IP cameras, DVRs, NVRs, and video management software used across enterprise, government, and consumer markets.

**Key facts:**

- **Product lines:** DS-2CD (fixed cameras), DS-2DE (PTZ cameras), DS-76/77/96 (NVRs), DS-7200/7300/7600 (DVRs)
- **Protocol support:** ONVIF Profile S/G/T, RTSP, HTTP, ISAPI
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during initial setup; older firmware: admin / 12345)
- **ONVIF support:** Full -- recommended for automatic discovery and configuration
- **Video codecs:** H.264, H.265 (Smart Codec), MJPEG

## RTSP URL Patterns

Hikvision cameras use a channel-based URL structure. Channel numbers encode both the camera channel and the stream type.

### URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:[PORT]/Streaming/Channels/[CHANNEL_ID]
```

**Channel ID encoding:**

- Channel ID = (camera_number * 100) + stream_number
- Stream 1 = main stream, Stream 2 = sub stream, Stream 3 = third stream
- Example: Camera 1, main stream = **101**; Camera 1, sub stream = **102**

### IP Cameras (Single Channel)

| Model Series | RTSP URL | Stream | Audio |
|-------------|----------|--------|-------|
| DS-2CD2xx2 (2MP fixed) | `rtsp://IP:554/Streaming/Channels/101` | Main (1080p) | Yes |
| DS-2CD2xx2 (2MP fixed) | `rtsp://IP:554/Streaming/Channels/102` | Sub (CIF/D1) | Yes |
| DS-2CD2x32 (3MP fixed) | `rtsp://IP:554/Streaming/Channels/101` | Main (2048x1536) | Yes |
| DS-2CD2x32 (3MP fixed) | `rtsp://IP:554/Streaming/Channels/102` | Sub | Yes |
| DS-2CD21xx-I (value series) | `rtsp://IP:554/Streaming/Channels/1` | Main | Yes |
| DS-2CD21xx-I (value series) | `rtsp://IP:554/Streaming/Channels/2` | Sub | Yes |
| DS-2DE series (PTZ) | `rtsp://IP:554/Streaming/Channels/101` | Main | Yes |
| DS-2CD6362F (fisheye) | `rtsp://IP:554/Streaming/Channels/101` | Main (3072x2048) | Yes |

### NVR / DVR Channels

For NVR and DVR devices, change the camera number in the channel ID:

| Device | Channel | RTSP URL | Stream |
|--------|---------|----------|--------|
| NVR Camera 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Main |
| NVR Camera 1 | 1 | `rtsp://IP:554/Streaming/Channels/102` | Sub |
| NVR Camera 2 | 2 | `rtsp://IP:554/Streaming/Channels/201` | Main |
| NVR Camera 2 | 2 | `rtsp://IP:554/Streaming/Channels/202` | Sub |
| NVR Camera 8 | 8 | `rtsp://IP:554/Streaming/Channels/801` | Main |
| DVR Channel 1 | 1 | `rtsp://IP:554/Streaming/Channels/101` | Main |

### Alternative URL Formats

Some older Hikvision models and OEM variants use different URL patterns:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Older firmware versions |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Older firmware, sub stream |
| `rtsp://IP:554/PSIA/Streaming/channels/101` | PSIA protocol (legacy) |
| `rtsp://IP:554/video.h264` | Some OEM models |
| `rtsp://IP:554/live.sdp` | Some older models |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` | Dahua-compatible OEM |
| `rtsp://IP:554/mpeg4` | MPEG4 stream (legacy) |

## Connecting with VisioForge SDK

Use your Hikvision camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Hikvision DS-2CD2032-I, main stream
var uri = new Uri("rtsp://192.168.1.64:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/Streaming/Channels/102` instead.

### ONVIF Discovery

Hikvision cameras have excellent ONVIF support. Use ONVIF to automatically discover cameras on your network and retrieve their stream URIs without manually constructing RTSP URLs. See the [ONVIF integration guide](../mediablocks/Sources/index.md) for discovery code examples.

## Snapshot and MJPEG URLs

Hikvision cameras also provide HTTP endpoints for snapshots and MJPEG streams:

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/ISAPI/Streaming/channels/101/picture` | Requires authentication |
| MJPEG Stream | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Sub stream as MJPEG |
| Legacy Snapshot | `http://IP/Streaming/channels/1/picture` | Older firmware |
| CGI Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Basic authentication |

## Troubleshooting

### "Double slash" in URL path

Hikvision RTSP URLs use a path starting with `/Streaming/Channels/`. Some tools or code generate `//Streaming/Channels/` (double slash). Both work with Hikvision cameras, but use a single slash for correctness.

### Connection refused on port 554

- Verify RTSP is enabled in the camera's web interface: **Configuration > Network > Advanced Settings > RTSP**
- Check that the RTSP port hasn't been changed from the default (554)
- Ensure no firewall is blocking the port between your application and the camera

### Authentication failures

- Hikvision cameras require **digest authentication** by default. VisioForge SDK handles this automatically.
- On newer firmware, the default `admin/12345` credentials are disabled. You must set a strong password during initial setup via the Hikvision SADP tool or web interface.
- If connecting to an NVR, use the NVR credentials, not the individual camera credentials.

### H.265 stream not playing

- Ensure you have the HEVC decoder redistributable installed
- Alternatively, configure the camera to use H.264 encoding in its video settings
- Use `rtspSettings.UseGPUDecoder = true` for hardware-accelerated H.265 decoding

### High latency

- Use TCP transport: `rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP`
- Reduce buffer latency: `rtspSettings.Latency = TimeSpan.FromMilliseconds(200)`
- Switch to the sub stream (channel 102) for lower bandwidth requirements
- Disable audio if not needed: `audioEnabled: false`

Combined on a single `RTSPSourceSettings` instance (constructed via the async factory):

```csharp
var rtspSettings = await RTSPSourceSettings.CreateAsync(
    uri: new Uri("rtsp://192.168.1.100:554/Streaming/Channels/102"),  // sub stream
    login: "admin",
    password: "password",
    audioEnabled: false);

rtspSettings.UseGPUDecoder    = true;                        // hardware-accelerated H.265
rtspSettings.AllowedProtocols = RTSPSourceProtocol.TCP;      // TCP transport
rtspSettings.Latency          = TimeSpan.FromMilliseconds(200);
```

## FAQ

**What is the default RTSP URL for Hikvision cameras?**

The standard RTSP URL for Hikvision cameras is `rtsp://admin:password@CAMERA_IP:554/Streaming/Channels/101` for the main stream. Replace `admin` and `password` with your camera credentials, and `CAMERA_IP` with the camera's IP address. Use channel `102` for the sub stream.

**How do I find my Hikvision camera's IP address?**

Use the Hikvision SADP (Search Active Devices Protocol) tool, which is a free utility that discovers all Hikvision devices on your local network. Alternatively, check your router's DHCP client list or use ONVIF device discovery with the VisioForge SDK.

**Can I connect to a Hikvision NVR and view individual camera channels?**

Yes. Use the same RTSP URL format but change the channel number. Camera 1 is channel 101 (main) or 102 (sub), camera 2 is channel 201/202, and so on. The formula is: channel ID = (camera_number x 100) + stream_number.

**Does VisioForge SDK support Hikvision's H.265+ (Smart Codec)?**

Yes. The SDK supports standard H.265/HEVC decoding. Hikvision's H.265+ is a proprietary compression optimization that produces standard H.265 streams, so it works with any H.265-capable decoder.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [LTS Connection Guide](lts.md) — Hikvision OEM, uses same URL format
- [EZVIZ Connection Guide](ezviz.md) — Hikvision consumer brand
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
