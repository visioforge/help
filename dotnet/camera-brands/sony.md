---
title: Sony SNC IP Camera RTSP URL Patterns and C# .NET Setup
description: Sony SNC CH, DH, EB, CX, and IPELA camera RTSP URL patterns for C# .NET. Stream and record with VisioForge Video Capture SDK integration code.
---

# How to Connect to Sony IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Sony** (Sony Corporation, Security Systems Division) was a major manufacturer of professional IP surveillance cameras under the **IPELA** brand and later the **SNC** (Sony Network Camera) product line. Sony exited the security camera market in 2020, selling its security business to **Bosch**. However, a large installed base of Sony cameras remains in use worldwide, particularly in enterprise and government installations.

**Key facts:**

- **Product lines:** SNC-CH (box, H.264), SNC-DH (dome, H.264), SNC-EB/ER (E-series), SNC-CX (compact), SNC-VB/VM/WR/XM (current-gen before exit), SNC-DF/RX/RZ/CS (legacy IPELA), SNT (video encoders)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI, Sony proprietary (DEPA)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (must be changed on setup)
- **ONVIF support:** Yes (SNC-CH/DH and newer models)
- **Video codecs:** H.264, H.265 (late models), MPEG-4 (legacy), MJPEG
- **Status:** Sony exited security camera market in 2020

!!! warning "End of life"
    Sony exited the security camera market in 2020. While existing cameras continue to work, no new firmware updates or models are being released. The RTSP URLs documented here remain valid for existing installations.

## RTSP URL Patterns

### Current-Gen Models (SNC-CH/DH/EB/ER/CX/VB/VM/WR/XM)

| Stream | RTSP URL | Codec | Notes |
|--------|----------|-------|-------|
| Video 1 (main) | `rtsp://IP:554/media/video1` | H.264 | Main stream |
| Video 2 (sub) | `rtsp://IP:554/media/video2` | H.264 | Sub stream |
| ONVIF profile | `rtsp://IP//profile` | H.264 | ONVIF-based (note double slash) |
| Direct | `rtsp://IP//media/video1` | H.264 | Alternative (double slash) |

### Model-Specific URLs

| Model Series | RTSP URL | Resolution | Notes |
|-------------|----------|------------|-------|
| SNC-CH110 | `rtsp://IP/media/video1` | 1280x1024 | Box camera |
| SNC-CH120/CH140 | `rtsp://IP/media/video1` | 1280x1024 / 1920x1080 | Box camera |
| SNC-CH160/CH180 | `rtsp://IP/media/video1` | 1920x1080 | Box camera |
| SNC-CH210/CH260/CH280 | `rtsp://IP/media/video1` | 1920x1080 / 2MP | Box camera |
| SNC-DH110/DH120/DH140 | `rtsp://IP/media/video1` | Up to 1080p | Fixed dome |
| SNC-DH160/DH180 | `rtsp://IP/media/video1` | 1920x1080 | Fixed dome |
| SNC-DH210/DH260 | `rtsp://IP/media/video1` | 1920x1080 | Fixed dome |
| SNC-EB600B | `rtsp://IP/media/video1` | 1080p | E-series |
| SNC-CX600W | `rtsp://IP:554//media/video1` | 1080p | Compact |
| SNC-VB630/WR630/XM632 | `rtsp://IP//profile` | 1080p+ | Latest gen |
| SNC-DM110 | `rtsp://IP:554//media/video1` | 720p | Mini dome |

### Legacy IPELA Models (SNC-RX/RZ/DF/CS/EP)

Older Sony IPELA cameras typically don't support RTSP and use HTTP-based streaming:

| Model Series | URL | Notes |
|-------------|-----|-------|
| SNC-RX530/RX550 | `http://IP/jpeg/vga.jpg` | JPEG snapshot |
| SNC-RZ25/RZ30/RZ50 | `http://IP/oneshotimage.jpg` | JPEG snapshot |
| SNC-DF40/DF50/DF70/DF80 | `http://IP/image` | JPEG snapshot |
| SNC-CS11/CS3P/CS50P | `http://IP/oneshotimage.jpg` | JPEG snapshot |
| SNC-EP520/EP580 | `http://IP/jpeg/vga.jpg` | JPEG snapshot |
| SNC-M1/M3 | `http://IP/image` | Very old MPEG-4 |

### Video Encoder URLs

| Encoder | URL | Notes |
|---------|-----|-------|
| SNT-EX101/EX104 | `http://IP/oneshotimage.jpg` | Snapshot per channel |
| SNT-EX104 (channel) | `http://IP/CH1/oneshotimage.jpg` | Channel-specific |
| SNT-V704 | `http://IP/CH1/oneshotimage.jpg` | 4-channel encoder |

## Connecting with VisioForge SDK

Use your Sony camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Sony SNC camera, main stream
var uri = new Uri("rtsp://192.168.1.55:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/media/video2` instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/oneshotimage.jpg` | Most SNC models |
| JPEG (VGA) | `http://IP/jpeg/vga.jpg` | VGA resolution |
| JPEG (QVGA) | `http://IP/jpeg/qvga.jpg` | QVGA resolution |
| MJPEG Stream | `http://IP/img/mjpeg.cgi` | Continuous MJPEG |
| MJPEG (alt) | `http://IP/mjpeg` | Alternative MJPEG path |
| H.264 over HTTP | `http://IP/h264` | H.264 stream via HTTP |
| Image | `http://IP/image` | Generic snapshot |
| Channel snapshot | `http://IP/oneshotimage1` | Channel-specific |

## Troubleshooting

### Double slash in URLs

Some Sony models use a double slash before the path in RTSP URLs:

- `rtsp://IP//media/video1` (double slash)
- `rtsp://IP:554/media/video1` (single slash with port)

Both formats usually work, but try the double-slash variant if the standard URL fails.

### ONVIF vs direct RTSP

Sony cameras support both direct RTSP and ONVIF-based connections:

- Direct RTSP: `rtsp://IP:554/media/video1` (recommended)
- ONVIF: `rtsp://IP//profile` (ONVIF-discovered URL)

### Legacy cameras without RTSP

Older Sony IPELA cameras (SNC-RX, SNC-RZ, SNC-DF, SNC-CS, SNC-M series) often don't support RTSP and only offer HTTP JPEG/MJPEG. For these cameras, use HTTP snapshot integration.

### Sony exited the market

Sony sold its security camera business in 2020. Existing cameras continue to function but receive no new firmware updates. Plan for eventual migration when deploying new integrations.

## FAQ

**What is the default RTSP URL for Sony SNC cameras?**

For current Sony SNC cameras, use `rtsp://admin:password@CAMERA_IP:554/media/video1` for the main stream and `media/video2` for the sub stream.

**Does Sony still make IP cameras?**

No. Sony exited the security camera market in 2020. Existing Sony SNC cameras remain in use and their RTSP streams continue to work, but no new models or firmware updates are being released.

**Do Sony cameras support ONVIF?**

Yes. Sony SNC-CH, SNC-DH, and newer series support ONVIF Profile S. Use `rtsp://IP//profile` for ONVIF-based connections.

**What about Sony IPELA cameras?**

IPELA was Sony's earlier camera brand. Many IPELA models (SNC-RX, SNC-RZ, SNC-DF series) only support HTTP JPEG/MJPEG, not RTSP. Later IPELA models (SNC-CH/DH series) do support RTSP via `media/video1`.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Canon Connection Guide](canon.md) — Japanese enterprise cameras
- [Axis Connection Guide](axis.md) — Enterprise surveillance peer
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
