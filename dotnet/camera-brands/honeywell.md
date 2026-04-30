---
title: Honeywell IP Camera RTSP URL Guide for C# .NET Apps
description: Connect to Honeywell Performance Series and equIP cameras in C# .NET with RTSP URL patterns and code samples for HD, HDZ, HBD, HBW, and PSIA models.
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
  - H.265
  - MJPEG
  - C#

---

# How to Connect to Honeywell IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Honeywell Commercial Security** (part of Honeywell Building Technologies) is a major manufacturer of enterprise video surveillance equipment. Honeywell cameras are widely deployed in commercial buildings, critical infrastructure, government facilities, and transportation systems worldwide. Honeywell acquired several camera brands over the years including **Samsung Techwin** (briefly) and markets cameras under the **Performance Series**, **30 Series**, and **60 Series** product lines.

**Key facts:**

- **Product lines:** Performance Series (equIP, H-series), 30 Series (HC30W, HC35W), 60 Series (HC60W), HDZ/HD (legacy equIP), HBD/HBW (bullet/dome), IPCAM (consumer)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), PSIA, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / 1234 (Performance Series); admin / admin (legacy models); varies by model and firmware
- **ONVIF support:** Yes (all current Performance Series and 30/60 Series models)
- **Video codecs:** H.264, H.265 (current models), MPEG-4 (legacy)

## RTSP URL Patterns

### Current Models (Performance Series, 30/60 Series)

| Stream | RTSP URL | Codec | Notes |
|--------|----------|-------|-------|
| Main stream (H.264) | `rtsp://IP:554/h264` | H.264 | Primary stream |
| Main stream (H.265) | `rtsp://IP:554/h265` | H.265 | Current models only |
| Channel stream (H.264) | `rtsp://IP:554/cam1/h264` | H.264 | Channel-specific |
| Channel stream (MPEG-4) | `rtsp://IP:554/cam1/mpeg4` | MPEG-4 | Legacy fallback |
| PSIA stream | `rtsp://IP:554/PSIA/Streaming/channels/1` | H.264 | PSIA-compatible |

### Model-Specific URLs

| Model Series | RTSP URL | Resolution | Notes |
|-------------|----------|------------|-------|
| HC30W/HC35W (30 Series) | `rtsp://IP:554/h264` | Up to 5MP | Current Wi-Fi |
| HC60W (60 Series) | `rtsp://IP:554/h264` | Up to 4K | Current wired |
| HD45IP | `rtsp://IP:554/h264` | 1080p | equIP dome |
| HD54IP | `rtsp://IP:554/h264` | 1080p | equIP box |
| HD55IPX | `rtsp://IP:554/h264` | 1080p+ | equIP box |
| HDZ20HDEX/HDZ20HDX | `rtsp://IP:554/h264` | 1080p | equIP PTZ |
| HD4MDIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Multi-channel |
| HDM3DIP | `rtsp://IP:554/cam1/mpeg4` | 720p | Mini dome |
| HBD/HBW series | `rtsp://IP:554/h264` | Up to 4MP | Bullet/dome |

### PSIA Streaming

Honeywell cameras that support **PSIA (Physical Security Interoperability Alliance)** use a different URL format:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Channel 1 | `rtsp://IP:554/PSIA/Streaming/channels/1` | First channel |
| Channel 2 | `rtsp://IP:554/PSIA/Streaming/channels/2` | Second channel |

### Legacy Models (HTTP Only)

Older Honeywell consumer cameras (IPCAM series) use HTTP:

| Model | URL | Notes |
|-------|-----|-------|
| IPCAM / IPCAM-PT | `http://IP/img/snapshot.cgi?size=3` | JPEG snapshot |
| IPCAM-PT | `http://IP/img/video.mjpeg` | MJPEG stream |
| IPCAM-PT | `http://IP/img/video.asf` | ASF stream (audio) |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.mjpeg` | MJPEG stream |
| IPCAM-OD / IPCAM-W12 | `http://IP/img/video.asf` | ASF stream (audio) |

## Connecting with VisioForge SDK

Use your Honeywell camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Honeywell Performance Series camera, main stream
var uri = new Uri("rtsp://192.168.1.75:554/h264");
var username = "admin";
var password = "YourPassword";
```

For PSIA channel streams, use `/PSIA/Streaming/channels/1` instead. For multi-channel models, use `/cam1/h264` format.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/img/snapshot.cgi?size=3` | Most models |
| MJPEG Stream | `http://IP/img/video.mjpeg` | Continuous MJPEG |
| ASF Stream | `http://IP/img/video.asf` | ASF with audio |
| HREP Snapshot | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=1` | NVR channel snapshot |

## Troubleshooting

### RTSP URL format

Honeywell cameras use a simple RTSP URL format compared to other brands:

- Primary: `rtsp://IP:554/h264` (no complex paths)
- Multi-channel: `rtsp://IP:554/cam1/h264` (channel number in path)
- PSIA: `rtsp://IP:554/PSIA/Streaming/channels/1` (PSIA standard)

If `/h264` doesn't work, try `/cam1/h264` or the PSIA URL.

### Default credentials vary

Honeywell has used different default credentials across product lines:

- **Performance Series:** admin / 1234 (must be changed on first login)
- **30/60 Series:** Set during initial setup (no default)
- **Legacy equIP:** admin / admin
- **IPCAM series:** admin / (empty) or admin / admin

### PSIA vs ONVIF

Honeywell cameras support both PSIA and ONVIF protocols:

- **ONVIF** is recommended for new integrations (wider compatibility)
- **PSIA** is Honeywell's legacy interoperability standard, still supported on most models
- Both provide the same video streams via different discovery and configuration mechanisms

## FAQ

**What is the default RTSP URL for Honeywell cameras?**

For most current Honeywell cameras, use `rtsp://admin:password@CAMERA_IP:554/h264`. For multi-channel models, use `rtsp://admin:password@CAMERA_IP:554/cam1/h264`. PSIA-compatible cameras also respond to `/PSIA/Streaming/channels/1`.

**Do Honeywell cameras support ONVIF?**

Yes. All current Honeywell Performance Series, 30 Series, and 60 Series cameras support ONVIF Profile S (streaming), Profile G (recording), and Profile T (advanced streaming). Legacy equIP models may only support ONVIF Profile S.

**What is PSIA on Honeywell cameras?**

PSIA (Physical Security Interoperability Alliance) is an alternative to ONVIF for device interoperability. Honeywell has historically supported PSIA alongside ONVIF. PSIA streams use the URL format `rtsp://IP:554/PSIA/Streaming/channels/1`.

**Are Honeywell IPCAM models still supported?**

The IPCAM consumer series is discontinued. These cameras only support HTTP MJPEG/JPEG and do not have RTSP. For IPCAM models, use the HTTP snapshot URL `http://IP/img/snapshot.cgi?size=3` or MJPEG stream `http://IP/img/video.mjpeg`.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Bosch Connection Guide](bosch.md) — Enterprise / commercial segment peer
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
