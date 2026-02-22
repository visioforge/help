---
title: Vivotek IP Camera RTSP URL and C# .NET Connection Guide
description: Connect to Vivotek IP cameras in C# .NET with RTSP URL patterns and code samples for FD, IP, SD, FE fisheye, and video server models.
---

# How to Connect to Vivotek IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Vivotek Inc.** is a Taiwanese manufacturer of network surveillance solutions headquartered in New Taipei City. Founded in 2000, Vivotek is one of the world's leading IP camera brands, widely deployed in enterprise, retail, transportation, and city surveillance. Vivotek is known for its wide range of form factors including fisheye, panoramic, speed dome, and specialty cameras.

**Key facts:**

- **Product lines:** FD (fixed dome), IP (box/bullet), IB (bullet), SD (speed dome), FE (fisheye), MD (mobile dome), CC (compact), VS (video servers/encoders)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, MJPEG
- **Default RTSP port:** 554
- **Default credentials:** root / (empty or set during setup); legacy models: root / root
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265, MJPEG

## RTSP URL Patterns

### Current Models

All current Vivotek cameras use the `live.sdp` URL pattern for RTSP streaming:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Stream 1 (main) | `rtsp://IP:554/live.sdp` | Main stream, H.264/H.265 |
| Stream 2 (sub) | `rtsp://IP:554/live2.sdp` | Sub stream |
| Stream 3 | `rtsp://IP:554/live3.sdp` | Third stream (if supported) |
| Stream 4 | `rtsp://IP:554/live4.sdp` | Fourth stream (some models) |

### Model-Specific URLs

| Model Series | RTSP URL | Form Factor |
|-------------|----------|-------------|
| FD81xx (fixed dome) | `rtsp://IP:554/live.sdp` | Fixed dome |
| FD83xx (fixed dome) | `rtsp://IP:554/live.sdp` | Fixed dome |
| FD8134/FD8136 | `rtsp://IP:554/live.sdp` | Mini dome |
| FD8161/FD8162/FD8166 | `rtsp://IP:554/live.sdp` | Fixed dome |
| FD8335H | `rtsp://IP:554/live.sdp` | Fixed dome |
| FD8361/FD8362E/FD8372 | `rtsp://IP:554/live.sdp` | Fixed dome |
| FE8171V/FE8172V/FE8174 | `rtsp://IP:554/live.sdp` | Fisheye |
| IP7130/IP7131/IP7132 | `rtsp://IP:554/live.sdp` | Box camera |
| IP7160/IP7161 | `rtsp://IP:554/live.sdp` | Box camera |
| IP7330/IP7361 | `rtsp://IP:554/live.sdp` | Bullet |
| IP8130/IP8133/IP8152 | `rtsp://IP:554/live.sdp` | Box camera |
| IP8331/IP8332/IP8335H | `rtsp://IP:554/live.sdp` | Box camera |
| IP8362/IP8364 | `rtsp://IP:554/live.sdp` | Box camera |
| SD8362E | `rtsp://IP:554/live.sdp` | Speed dome |
| CC8130 | `rtsp://IP:554/live.sdp` | Compact |
| MD7560/MD8562 | `rtsp://IP:554/live.sdp` | Mobile dome |

### Legacy Models

Older Vivotek models (IP3xxx, IP6xxx, PT3xxx, PZ6xxx series) used HTTP-only streaming:

| Model Series | URL | Notes |
|-------------|-----|-------|
| IP3121/IP3122/IP3133/IP3135 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG only |
| IP6127 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG only |
| PT3112/PT3122 | `http://IP/cgi-bin/video.jpg?size=2` | Pan/tilt, JPEG |
| PZ6114/PZ6122 | `http://IP/cgi-bin/video.jpg?size=2` | Pan/zoom, JPEG |

### Video Server URLs

Vivotek video servers encode analog camera feeds for IP streaming:

| Model | RTSP URL | Notes |
|-------|----------|-------|
| VS2403 | `rtsp://IP:554/live.sdp` | Video server, multi-channel |
| VS3100P | `http://IP/cgi-bin/video.jpg?size=2` | Legacy encoder |
| VS7100 | `rtsp://IP:554/live.sdp` | Video server |
| VS8102 | `rtsp://IP:554/live.sdp` | Video server |
| VS8401 | `rtsp://IP:554/live.sdp` | 4-channel server |
| VS8801 | `rtsp://IP:554/live.sdp` | 8-channel server |

### NVR URLs

| Model | RTSP URL | Notes |
|-------|----------|-------|
| NR8x01 NVR | `rtsp://IP:554/live.sdp` | Via NVR |

## Connecting with VisioForge SDK

Use your Vivotek camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Vivotek camera, main stream
var uri = new Uri("rtsp://192.168.1.50:554/live.sdp");
var username = "root";
var password = "YourPassword";
```

For sub-stream access, use `/live2.sdp` instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/viewer/video.jpg?resolution=640x480` | Current models |
| JPEG Snapshot (channel) | `http://IP/cgi-bin/viewer/video.jpg?channel=1&resolution=640x480` | Multi-channel |
| MJPEG Stream | `http://IP/video.mjpg` | Continuous MJPEG |
| MJPEG Stream (alt) | `http://IP/video2.mjpg` | Second stream |
| MJPEG (params) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | With quality/fps params |
| Legacy Snapshot | `http://IP/cgi-bin/video.jpg` | Older models |
| Legacy Snapshot (sized) | `http://IP/cgi-bin/video.jpg?size=2` | Older models, VGA |
| Snapshot CGI | `http://IP/snapshot.cgi` | Some models |

## Troubleshooting

### Consistent URL pattern

Unlike many brands, Vivotek uses the same `live.sdp` RTSP URL pattern across virtually all their RTSP-capable models. If `rtsp://IP:554/live.sdp` doesn't work, try:

- `rtsp://IP:554/live2.sdp` (sub stream)
- `rtsp://IP:554/live3.sdp` (third stream)

### Default credentials

- **Current models:** `root` with password set during initial setup
- **Legacy models:** `root` / (empty password) or `root` / `root`
- Some models require setup via the web interface before RTSP is accessible

### Non-standard ports on some models

Some Vivotek cameras may use non-standard RTSP ports (e.g., 1025, 1032) if configured. Check the camera's web interface under Network > RTSP settings if port 554 doesn't respond.

### Legacy HTTP-only cameras

Very old Vivotek cameras (IP31xx, IP61xx, PT31xx, PZ61xx series) only support HTTP JPEG and MJPEG streams, not RTSP. These cameras cannot use the RTSP source -- use HTTP snapshot or MJPEG integration instead.

## FAQ

**What is the default RTSP URL for Vivotek cameras?**

The standard URL is `rtsp://root:password@CAMERA_IP:554/live.sdp` for the main stream. Use `live2.sdp` for the sub stream and `live3.sdp` for the third stream. This pattern works across virtually all RTSP-capable Vivotek models.

**Do Vivotek cameras support H.265?**

Yes. Current Vivotek cameras support H.265 (HEVC). Use the same `live.sdp` URL -- the codec is configured in the camera's web interface, not in the URL.

**What is the difference between live.sdp and live2.sdp?**

`live.sdp` is the main (highest quality) stream, `live2.sdp` is typically a lower-resolution sub stream for bandwidth-constrained viewing, and `live3.sdp` is a third stream often used for mobile viewing.

**Do Vivotek video servers support RTSP?**

Yes. Current Vivotek video servers (VS2403, VS7100, VS8102, VS8401, VS8801) support RTSP using the same `live.sdp` URL pattern as cameras. Legacy servers (VS3100P) only support HTTP JPEG.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [GeoVision Connection Guide](geovision.md) — Taiwanese enterprise cameras
- [ACTi Connection Guide](acti.md) — Taiwanese professional cameras
- [IP Camera Capture to MP4](../videocapture/video-tutorials/ip-camera-capture-mp4.md) — Record Vivotek streams to file
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
