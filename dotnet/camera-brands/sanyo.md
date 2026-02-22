---
title: Sanyo IP Camera RTSP URL Patterns and C# .NET Integration
description: Connect Sanyo VCC, VDC, and VCC-HD series IP cameras in C# .NET using RTSP URL patterns, snapshot endpoints, and VisioForge SDK code examples.
---

# How to Connect to Sanyo IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Sanyo** (Sanyo Electric Co., Ltd.) was a Japanese electronics manufacturer headquartered in Osaka, Japan. Sanyo's security camera division produced the well-regarded VCC and VDC camera lines for professional surveillance installations. In 2009-2011, Panasonic acquired Sanyo Electric, and the camera technology was integrated into Panasonic's i-PRO product line. While Sanyo cameras are no longer manufactured, many units remain deployed in legacy installations worldwide.

**Key facts:**

- **Product lines:** VCC (box cameras), VDC (dome cameras), VCC-HD (HD series)
- **Status:** Discontinued (acquired by Panasonic 2009-2011)
- **Protocol support:** RTSP, HTTP/CGI, limited ONVIF (newer firmware)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Limited (older firmware only)
- **Video codecs:** H.264, MJPEG
- **Successor:** Panasonic i-PRO

!!! warning "Sanyo Cameras Are Discontinued"
    Sanyo security cameras are discontinued. Sanyo Electric was acquired by Panasonic, and the camera technology was integrated into Panasonic's i-PRO product line. See our [Panasonic/i-PRO connection guide](panasonic.md) for current products.

## RTSP URL Patterns

### Standard URL Format

Sanyo cameras use the `VideoInput` RTSP path:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/VideoInput/1/h264/1
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `VideoInput` | 1, 2, 3... | Camera channel (1 for standalone cameras) |
| `h264` | h264 | H.264 video codec |
| Trailing `1` | 1 | Stream index |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| VCC-HD2300P | HD box camera | `rtsp://IP:554/VideoInput/1/h264/1` | H.264 main stream |
| VCC-HD series | HD cameras | `rtsp://IP:554/VideoInput/1/h264/1` | H.264 main stream |
| VCC-9574N | Box camera | `rtsp://IP:554/VideoInput/1/h264/1` | H.264 main stream |
| VCC-P9574N | PTZ camera | `rtsp://IP:554/VideoInput/1/h264/1` | H.264 main stream |
| VDC series | Dome cameras | `rtsp://IP:554/VideoInput/1/h264/1` | H.264 main stream |

### DVR Channel URLs

For Sanyo DVR systems with multiple channels:

| Channel | Main Stream URL |
|---------|----------------|
| Camera 1 | `rtsp://IP:554/VideoInput/1/h264/1` |
| Camera 2 | `rtsp://IP:554/VideoInput/2/h264/1` |
| Camera N | `rtsp://IP:554/VideoInput/N/h264/1` |

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/VideoInput/1/h264/1` | Standard H.264 stream (recommended) |
| `rtsp://IP:554/VideoInput/CHANNEL/h264/1` | Multi-channel DVR access |

## Connecting with VisioForge SDK

Use your Sanyo camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Sanyo VCC-HD2300P, main stream
var uri = new Uri("rtsp://192.168.1.90:554/VideoInput/1/h264/1");
var username = "admin";
var password = "YourPassword";
```

For multi-channel DVR access, replace `VideoInput/1` with the appropriate channel number.

## Snapshot and MJPEG URLs

!!! note "Sanyo's liveimg.cgi Endpoint"
    Sanyo cameras use a distinctive `/liveimg.cgi` endpoint for HTTP snapshots and MJPEG streams. The `serverpush=1` parameter enables continuous MJPEG streaming.

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Live Snapshot | `http://IP/liveimg.cgi` | Single JPEG frame |
| MJPEG Stream | `http://IP/liveimg.cgi?serverpush=1` | Continuous server-push MJPEG |
| MJPEG with Channel | `http://IP/liveimg.cgi?serverpush=1&jpeg=1&stream=CHANNEL` | Channel-specific MJPEG stream |
| Channel Snapshot (DVR) | `http://IP/liveimg.cgi?ch=CHANNEL` | Channel-specific snapshot for DVR |

## Troubleshooting

### "401 Unauthorized" error

Sanyo cameras use basic authentication by default. Ensure you are providing correct credentials:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Log in with your credentials (default: admin / admin)
3. Verify the RTSP service is enabled in network settings
4. Use those credentials in your RTSP URL

### H.264 stream not available

Older Sanyo models may only support MJPEG. If the H.264 URL does not work, try using the MJPEG HTTP stream instead:

```
http://CAMERA_IP/liveimg.cgi?serverpush=1
```

### Firmware and compatibility

Since Sanyo cameras are discontinued, firmware updates are no longer available. If you encounter compatibility issues:

- Ensure the camera firmware is the latest available version
- Try using ONVIF discovery if direct URL connection fails
- Consider migrating to Panasonic i-PRO cameras, which inherit Sanyo's technology

### MJPEG server-push not working

The `serverpush=1` parameter requires the camera's HTTP server to support chunked transfer encoding. Some older firmware versions may not support this reliably. Try the single snapshot endpoint (`/liveimg.cgi` without parameters) and poll at your desired frame rate instead.

## FAQ

**Are Sanyo cameras still supported?**

Sanyo security cameras are discontinued. Sanyo Electric was fully acquired by Panasonic, and the surveillance camera technology was merged into Panasonic's i-PRO product line. No new firmware updates or support are available for Sanyo-branded cameras.

**What is the default RTSP URL for Sanyo cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/VideoInput/1/h264/1` for the main H.264 stream. For DVR setups, replace `VideoInput/1` with the appropriate channel number (e.g., `VideoInput/2` for channel 2).

**Do Sanyo cameras support ONVIF?**

Only some Sanyo cameras with newer firmware versions have limited ONVIF support. Most older models do not support ONVIF and require direct RTSP URL configuration.

**What should I use instead of Sanyo cameras?**

Panasonic's i-PRO product line is the direct successor to Sanyo's security camera division. The i-PRO cameras use similar VideoInput RTSP paths and offer modern features like H.265, advanced analytics, and full ONVIF support. See our [Panasonic/i-PRO connection guide](panasonic.md).

**How do I get snapshots from a Sanyo camera?**

Use the `/liveimg.cgi` HTTP endpoint: `http://CAMERA_IP/liveimg.cgi` returns a single JPEG frame. Add `?serverpush=1` for a continuous MJPEG stream, or `?ch=CHANNEL` for a specific DVR channel.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Panasonic/i-PRO Connection Guide](panasonic.md) — Successor product line
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
