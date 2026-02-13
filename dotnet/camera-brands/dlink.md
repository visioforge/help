---
title: How to Connect to D-Link IP Camera in C# .NET
description: Connect to D-Link DCS cameras in C# .NET with RTSP URL patterns and code samples for DCS-930, DCS-2130, DCS-5222, and other DCS models.
---

# How to Connect to D-Link IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**D-Link Corporation** is a Taiwanese networking equipment manufacturer headquartered in Taipei. D-Link produces IP cameras under the **DCS (D-Link Cloud Security)** product line, targeting consumer and small business markets. D-Link cameras are widely available through retail channels and are popular for home security and small office deployments.

**Key facts:**

- **Product lines:** DCS-930/932/933/934 (consumer Wi-Fi), DCS-2130/2132/2230/2310/2330/2332 (prosumer), DCS-5020/5222/5615 (PTZ), DCS-6010/6113/6818 (enterprise), DCS-7010/7110/7410 (outdoor professional)
- **Protocol support:** RTSP, ONVIF (some models), HTTP/CGI, MJPEG, D-Link mydlink cloud
- **Default RTSP port:** 554
- **Default credentials:** admin / (empty password); some models: admin / admin
- **ONVIF support:** Select models only (typically DCS-2xxx and higher)
- **Video codecs:** H.264, MJPEG, MPEG-4 (legacy)

## RTSP URL Patterns

### Current and Recent Models

D-Link cameras use the `live.sdp` or `play.sdp` URL format:

| Stream | RTSP URL | Quality | Notes |
|--------|----------|---------|-------|
| Main stream (H.264) | `rtsp://IP:554/live1.sdp` | High | Main H.264 stream |
| Sub stream (H.264) | `rtsp://IP:554/live2.sdp` | Medium | Second stream |
| Third stream | `rtsp://IP:554/live3.sdp` | Low | Third stream (some models) |
| Main stream (alt) | `rtsp://IP:554/play1.sdp` | High | Alternative URL |
| Sub stream (alt) | `rtsp://IP:554/play2.sdp` | Medium | Alternative URL |

### Model-Specific URLs

| Model | RTSP URL | Resolution | Type |
|-------|----------|------------|------|
| DCS-930L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumer Wi-Fi |
| DCS-932L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumer Wi-Fi IR |
| DCS-933L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumer Wi-Fi |
| DCS-934L | `rtsp://IP:554/play1.sdp` | 1280x720 | Consumer HD |
| DCS-942L | `rtsp://IP:554/play1.sdp` | 640x480 | Consumer IR |
| DCS-2100+ | `rtsp://IP:554/live.sdp` | 640x480 | Legacy |
| DCS-2121 | `rtsp://IP:554/play1.sdp` | 640x480 | Prosumer |
| DCS-2130 | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumer HD |
| DCS-2132L | `rtsp://IP:554//live1.sdp` | 1280x720 | Prosumer HD |
| DCS-2230 | `rtsp://IP:554//live1.sdp` | 1920x1080 | Prosumer FHD |
| DCS-2310L | `rtsp://IP:554/live1.sdp` | 1280x720 | Outdoor HD |
| DCS-2332L | `rtsp://IP:554//live1.sdp` | 1280x720 | Outdoor HD |
| DCS-5020L | `rtsp://IP:554/play1.sdp` | 640x480 | PTZ consumer |
| DCS-5222L | `rtsp://IP:554//live1.sdp` | 1280x720 | PTZ HD |
| DCS-6010L | `rtsp://IP:554/live1.sdp` | 1600x1200 | Panoramic |
| DCS-6113 | `rtsp://IP:554/live1.sdp` | 1920x1080 | Box camera |
| DCS-6818 | `rtsp://IP:554/live3.sdp` | 1920x1080 | Enterprise |
| DCS-7010L | `rtsp://IP:554/live1.sdp` | 1280x720 | Outdoor PoE |
| DCS-7110 | `rtsp://IP:554/live1.sdp` | 1280x800 | Outdoor HD |
| DCS-7410 | `rtsp://IP:554/live1.sdp` | 1280x720 | Outdoor enterprise |

!!! info "Double slash in some URLs"
    Some D-Link models use a double slash before the path: `rtsp://IP:554//live1.sdp`. This is common on DCS-2130, DCS-2132L, DCS-2230, DCS-2332L, and DCS-5222L models. Try both single and double slash if one format doesn't work.

### Legacy Models (HTTP Only)

Very old D-Link DCS cameras only support HTTP:

| Model | URL | Notes |
|-------|-----|-------|
| DCS-900 | `http://IP/cgi-bin/video.jpg` | JPEG only |
| DCS-910 | `http://IP/video.cgi` | MJPEG |
| DCS-920 | `http://IP/video.cgi` | MJPEG |
| DCS-2100 | `http://IP/cgi-bin/video.jpg?size=2` | JPEG only |

## Connecting with VisioForge SDK

Use your D-Link camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// D-Link DCS camera, main stream
var uri = new Uri("rtsp://192.168.1.45:554/live1.sdp");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/live2.sdp` instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/image/jpeg.cgi` | Most current DCS models |
| MJPEG Stream | `http://IP/video/mjpg.cgi` | Continuous MJPEG |
| MJPEG (alt) | `http://IP/video.cgi` | Older models |
| MJPEG (auth) | `http://IP/mjpeg.cgi?user=USER&password=PASS&channel=1` | With authentication |
| DMS Snapshot | `http://IP/dms.jpg` | DCS-2130/2132/2230/2310/2332 |
| DMS Stream | `http://IP/dms?nowprofileid=2` | Profile-based |
| ipcam stream | `http://IP/ipcam/stream.cgi?nowprofileid=2` | Some models |
| Legacy JPEG | `http://IP/cgi-bin/video.jpg` | Very old DCS models |

## Troubleshooting

### live vs play URL format

D-Link cameras use two URL naming conventions:

- **Current models (DCS-2xxx+):** `live1.sdp`, `live2.sdp`, `live3.sdp`
- **Consumer models (DCS-930/932/933/942):** `play1.sdp`, `play2.sdp`, `play3.sdp`

If one format doesn't work, try the other.

### Default password is empty

Many D-Link cameras ship with `admin` as the username and an **empty password**. You may need to set a password through the web interface or D-Link Setup Wizard before RTSP works properly.

### mydlink cloud cameras

Some newer D-Link cameras are designed primarily for the mydlink cloud ecosystem and may have limited or no RTSP support. Check the camera's specifications for "RTSP" or "third-party integration" support.

### Port configuration

D-Link cameras use port 554 by default for RTSP. The HTTP interface is typically on port 80. Both can be changed in the camera's web interface under Network Settings.

## FAQ

**What is the default RTSP URL for D-Link cameras?**

For most D-Link DCS cameras, try `rtsp://admin:password@CAMERA_IP:554/live1.sdp` or `rtsp://admin:password@CAMERA_IP:554/play1.sdp`. The `live` format is used by newer models, while `play` is used by consumer models.

**Do D-Link cameras support ONVIF?**

Select models support ONVIF (typically DCS-2xxx and higher-end models). Consumer cameras like the DCS-930L and DCS-932L generally do not support ONVIF.

**What is the difference between live1.sdp and play1.sdp?**

Both serve the same purpose (main video stream) but are used by different D-Link camera generations. `live1.sdp` is more common on newer prosumer/professional models, while `play1.sdp` is used on older consumer models.

**Can I connect to D-Link cameras without the mydlink app?**

Yes. D-Link cameras with RTSP support can be accessed directly via their IP address without the mydlink cloud service. The mydlink cloud is optional for remote access.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Foscam Connection Guide](foscam.md) — Consumer IP camera peer
- [TP-Link Connection Guide](tp-link.md) — Consumer cameras with RTSP
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
