---
title: JVC IP Camera RTSP URL and C# .NET Integration Guide
description: JVC VN-H, VN-T, VN-C, and VN-X network camera RTSP URL patterns for C# .NET. Stream and record with VisioForge Video Capture SDK integration code.
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
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# How to Connect to JVC IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**JVC** (JVCKENWOOD Corporation) is a Japanese electronics company headquartered in Yokohama, Japan. JVC's Professional Systems Division produced the VN-series IP cameras for surveillance applications. JVC exited the standalone IP camera market around 2015, but many VN-series cameras remain in active service across enterprise and government installations. These cameras are known for their robust PSIA protocol support and reliable performance.

**Key facts:**

- **Product lines:** VN-H Series (VN-H37, VN-H137, VN-H237, VN-H657), VN-T Series (VN-T216U), VN-X Series (VN-X35U, VN-X235U), VN-C Series (VN-C20U)
- **Protocol support:** RTSP, ONVIF (VN-H/VN-T series), PSIA, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / jvc (varies by model)
- **ONVIF support:** Yes (VN-H and VN-T series)
- **Video codecs:** H.264 (VN-H/VN-T series), MPEG-4 (older VN-C models)

!!! warning "Discontinued Product Line"
    JVC exited the IP camera market around 2015. While VN-series cameras remain widely deployed, firmware updates are no longer available. Consider network segmentation and firewall rules to protect these cameras, as they will not receive security patches.

## RTSP URL Patterns

### Standard URL Formats

JVC cameras support multiple RTSP URL patterns depending on the model series:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/PSIA/Streaming/channels/0?videoCodecType=H.264
```

| URL Pattern | Protocol | Description |
|-------------|----------|-------------|
| `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | PSIA | Primary H.264 stream (most VN-H models) |
| `rtsp://IP:554/PSIA/Streaming/channels/CHANNEL` | PSIA | PSIA stream by channel number |
| `rtsp://IP:554/video.h264` | H.264 | Direct H.264 stream (VN Series general) |
| `rtsp://IP:554/1/stream1` | H.264 | Alternative stream URL (VN-T216U) |
| `rtsp://IP:554//livestream` | H.264 | Live stream URL (VN-H57) |

!!! note "PSIA Channel Numbering"
    JVC cameras use zero-based channel numbering for PSIA URLs. Channel 0 is the first (and usually only) video channel. This differs from most other brands that start channel numbering at 1.

### PSIA Channel URLs

| Channel | URL | Description |
|---------|-----|-------------|
| Channel 0 (primary) | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | First video channel (main stream) |
| Channel 1 | `rtsp://IP:554/PSIA/Streaming/channels/1?videoCodecType=H.264` | Second video channel (sub stream, if available) |

### Camera Models

| Model Series | Resolution | Main Stream URL | Codec |
|-------------|-----------|----------------|-------|
| VN-H37 (HD dome) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H137 (HD bullet) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H237 (HD dome) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-H657 (HD PTZ) | 1920x1080 | `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` | H.264 |
| VN-T216U (HD box) | 1920x1080 | `rtsp://IP:554/1/stream1` | H.264 |
| VN-X35U (network camera) | 1280x960 | `rtsp://IP:554/video.h264` | H.264 |
| VN-X235U (network camera) | 1920x1080 | `rtsp://IP:554/video.h264` | H.264 |
| VN-C20U (legacy network) | 640x480 | N/A (HTTP snapshot only) | MJPEG |

## Connecting with VisioForge SDK

Use your JVC camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// JVC VN-H Series, PSIA H.264 main stream
var uri = new Uri("rtsp://192.168.1.90:554/PSIA/Streaming/channels/0?videoCodecType=H.264");
var username = "admin";
var password = "jvc";
```

For VN-T series cameras using the alternative URL format:

```csharp
// JVC VN-T216U, alternative stream URL
var uri = new Uri("rtsp://192.168.1.90:554/1/stream1");
```

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/video.jpg` | Standard snapshot (most models) |
| Java Applet Snapshot | `http://IP/java.jpg` | Java-based snapshot (legacy) |
| API Snapshot | `http://IP/api/video?encode=jpeg` | API-based JPEG capture (VN-X series) |
| MJPEG Stream | `http://IP/api/video?encode=jpeg&framerate=15&boundary=on` | Continuous MJPEG via API |

### Model-Specific Snapshot URLs

| Model Series | Snapshot URL | Notes |
|-------------|-------------|-------|
| VN-H Series | `http://IP/cgi-bin/video.jpg` | CGI-based snapshot |
| VN-T Series | `http://IP/cgi-bin/video.jpg` | CGI-based snapshot |
| VN-C Series (VN-C20U) | `http://IP/cgi-bin/video.jpg` | CGI-based snapshot |
| VN-X Series (VN-X35U/X235U) | `http://IP/api/video?encode=jpeg` | API-based snapshot |

## Troubleshooting

### PSIA channel numbering starts at 0

Unlike most camera brands where channel numbering starts at 1, JVC uses **zero-based** PSIA channel numbering. If you are porting code from another brand:

- Channel 0 = First video channel (equivalent to Channel 1 on other brands)
- Channel 1 = Second video channel (sub stream or secondary sensor)

### Default credentials not working

JVC cameras ship with different default credentials depending on the model and firmware version:

1. Try `admin` / `jvc` (most common)
2. Try `admin` / `admin`
3. Try accessing the web interface at `http://CAMERA_IP` to reset or verify credentials
4. Some models ship with no default password - access the web interface first to set one

### Firmware updates unavailable

Since JVC discontinued its IP camera line around 2015, firmware updates are no longer available. To mitigate security risks:

- Place cameras on an isolated VLAN or network segment
- Use firewall rules to restrict access to camera ports
- Disable UPnP and any cloud connectivity features
- Consider replacing end-of-life cameras with currently supported models

### VN-C series HTTP-only access

The older VN-C series cameras (such as VN-C20U) do not support RTSP streaming and only provide HTTP-based MJPEG access. Use the HTTP snapshot or MJPEG stream URLs for these models instead of RTSP.

### Multiple URL formats on VN-T series

The VN-T216U supports multiple RTSP URL formats. If one does not work, try alternatives:

1. `rtsp://IP:554/1/stream1` (preferred)
2. `rtsp://IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264` (PSIA)
3. `rtsp://IP:554/video.h264` (direct H.264)

## FAQ

**What is the default RTSP URL for JVC cameras?**

For most VN-H series cameras, the primary RTSP URL is `rtsp://admin:jvc@CAMERA_IP:554/PSIA/Streaming/channels/0?videoCodecType=H.264`. The VN-T series uses `rtsp://IP:554/1/stream1` as an alternative. VN-X series models use `rtsp://IP:554/video.h264`.

**Are JVC IP cameras still supported?**

JVC exited the standalone IP camera market around 2015. The cameras remain functional but no longer receive firmware updates or official support. Many VN-series cameras are still actively deployed in surveillance systems worldwide.

**Do JVC cameras support ONVIF?**

The VN-H and VN-T series cameras support ONVIF Profile S. Older VN-C and some VN-X models do not support ONVIF and rely on PSIA or proprietary CGI interfaces instead.

**Why does PSIA channel numbering start at 0?**

JVC implements zero-based PSIA channel numbering, meaning the first video channel is channel 0 rather than channel 1. This is specific to JVC's PSIA implementation and differs from most other camera manufacturers. When migrating from another brand, adjust your channel numbers accordingly.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Sony Connection Guide](sony.md) — Japanese enterprise cameras
- [Canon Connection Guide](canon.md) — Japanese professional cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
