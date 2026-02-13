---
title: How to Connect to TP-Link IP Camera in C# .NET
description: Connect to TP-Link cameras and Tapo cameras in C# .NET with RTSP URL patterns and code samples for TL-SC, NC, and Tapo C series models.
---

# How to Connect to TP-Link IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**TP-Link** is a global networking equipment manufacturer headquartered in Shenzhen, China. While primarily known for routers and networking gear, TP-Link produces IP cameras under both the **TP-Link** brand (TL-SC series, now discontinued) and the **Tapo** smart home brand (Tapo C series, currently active). The Tapo line has become one of the best-selling consumer camera brands globally due to aggressive pricing and app-based setup.

**Key facts:**

- **Product lines:** TL-SC series (legacy, discontinued), NC series (cloud cameras, discontinued), Tapo C series (current smart home cameras)
- **Protocol support:** RTSP, HTTP/MJPEG, ONVIF (Tapo models with firmware update), proprietary cloud protocol
- **Default RTSP port:** 554
- **Default credentials:** Varies by generation (see below)
- **ONVIF support:** Tapo C series (requires enabling in Tapo app); TL-SC series has no ONVIF
- **Video codecs:** H.264 (all models), H.265 (Tapo C320WS and newer)

### Credentials by Product Line

| Product Line | Default Username | Default Password | Notes |
|-------------|-----------------|-----------------|-------|
| TL-SC series | admin | admin | Legacy, fixed |
| NC series | admin | admin | Cloud-managed |
| Tapo C series | (set in app) | (set in app) | Must create RTSP credentials in Tapo app |

!!! info "Tapo camera credentials"
    Tapo cameras require you to create a separate **camera account** in the Tapo app (Advanced Settings > Camera Account) before RTSP access works. This username/password is different from your TP-Link cloud account.

## RTSP URL Patterns

### Tapo C Series (Current Models)

The Tapo camera line uses a straightforward RTSP URL format:

| Model | RTSP URL | Stream | Audio |
|-------|----------|--------|-------|
| Tapo C100 (indoor) | `rtsp://IP:554/stream1` | Main (1080p) | Yes |
| Tapo C100 (indoor) | `rtsp://IP:554/stream2` | Sub (360p) | Yes |
| Tapo C110 (indoor 3MP) | `rtsp://IP:554/stream1` | Main (2304x1296) | Yes |
| Tapo C110 (indoor 3MP) | `rtsp://IP:554/stream2` | Sub | Yes |
| Tapo C200 (pan/tilt) | `rtsp://IP:554/stream1` | Main (1080p) | Yes |
| Tapo C200 (pan/tilt) | `rtsp://IP:554/stream2` | Sub (360p) | Yes |
| Tapo C210 (pan/tilt 3MP) | `rtsp://IP:554/stream1` | Main (2304x1296) | Yes |
| Tapo C310 (outdoor) | `rtsp://IP:554/stream1` | Main (2048x1296) | Yes |
| Tapo C320WS (outdoor 2K) | `rtsp://IP:554/stream1` | Main (2560x1440) | Yes |
| Tapo C500 (outdoor PTZ) | `rtsp://IP:554/stream1` | Main (1080p) | Yes |
| Tapo C520WS (outdoor 2K PTZ) | `rtsp://IP:554/stream1` | Main (2560x1440) | Yes |

### TL-SC Series (Legacy Models)

The discontinued TL-SC series used different URL formats depending on the model:

| Model | RTSP URL | Codec | Audio |
|-------|----------|-------|-------|
| TL-SC3130 | `rtsp://IP:554/video.mp4` | MPEG-4 | Yes |
| TL-SC3130G | `rtsp://IP:554/video.mp4` | MPEG-4 | Yes |
| TL-SC3171 | `rtsp://IP:554/video.mp4` | MPEG-4 | Yes |
| TL-SC3171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Yes |
| TL-SC3230 | `rtsp://IP:554/video.h264` | H.264 | Yes |
| TL-SC3230N | `rtsp://IP:554/video.h264` | H.264 | Yes |
| TL-SC3430 | `rtsp://IP:554/video.h264` | H.264 | Yes |
| TL-SC3430N | `rtsp://IP:554/video.h264` | H.264 | Yes |
| TL-SC4171G | `rtsp://IP:554/video.mp4` | MPEG-4 | Yes |

### TL-SC Alternative URL Formats

| URL Pattern | Codec | Notes |
|-------------|-------|-------|
| `rtsp://IP:554/video.mp4` | MPEG-4 | Primary for SC3xxx models |
| `rtsp://IP:554/video.h264` | H.264 | Primary for newer SC models |
| `rtsp://IP:554/video.mjpg` | MJPEG | Lower quality, wider compatibility |
| `rtsp://IP:554/video.pro2` | MPEG-4 | Alternative profile |
| `rtsp://IP:554/live.sdp` | H.264 | SDP-based stream |
| `rtsp://IP:554/cam1/h264` | H.264 | Channel-based format |
| `rtsp://IP:554/media.amp` | Auto | Axis-compatible firmware |

## Connecting with VisioForge SDK

Use your TP-Link camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// TP-Link Tapo C200, main stream
var uri = new Uri("rtsp://192.168.1.100:554/stream1");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/stream2` instead.

## Snapshot and MJPEG URLs

### Tapo C Series

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Snapshot | `http://IP/snapshot.jpg` | May require authentication |

### TL-SC Series

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/jpg/image.jpg` | Basic snapshot |
| Sized Snapshot | `http://IP/jpg/image.jpg?size=3` | Predefined size |
| CGI Snapshot | `http://IP/cgi-bin/jpg/image` | CGI-based |
| MJPEG Stream | `http://IP/video.mjpg` | Continuous MJPEG |
| MJPEG (quality) | `http://IP/video.mjpg?q=30&fps=33&id=0.5` | Quality/FPS control |
| Video CGI | `http://IP/video.cgi?resolution=VGA` | Resolution-specific |
| Net Video CGI | `http://IP/cgi-bin/net_video.cgi?channel=1` | Channel-based |
| Axis-compatible | `http://IP/axis-cgi/mjpg/video.cgi` | Emulated Axis API |

## Troubleshooting

### Tapo camera: "Connection refused" or "Unauthorized"

The most common issue with Tapo cameras is not setting up RTSP credentials:

1. Open the **Tapo app** on your phone
2. Go to your camera's settings
3. Navigate to **Advanced Settings > Camera Account**
4. Create a username and password
5. Use these credentials (not your TP-Link account) in RTSP URLs

### Tapo camera: ONVIF not working

ONVIF is disabled by default on Tapo cameras. To enable it:

1. Open the Tapo app
2. Go to camera settings > Advanced Settings
3. Enable **ONVIF** toggle
4. Camera will reboot

### TL-SC models: wrong codec URL

TL-SC cameras are codec-specific in their URLs:

- **SC3130/3171 series:** Use `/video.mp4` (MPEG-4)
- **SC3230/3430 series:** Use `/video.h264` (H.264)
- Using the wrong codec in the URL path will result in no stream

### Stream2 on Tapo cameras shows low resolution

This is by design. `stream2` is the sub stream intended for lower bandwidth. Use `stream1` for full resolution. You can adjust the sub stream resolution in the Tapo app under camera settings.

### TL-SC models: videostream.asf not working

The `videostream.asf` URL format requires URL-embedded credentials:
`http://IP/videostream.asf?user=admin&pwd=admin&resolution=64&rate=0`

The `resolution` parameter values: 32 = 320x240, 64 = 640x480.

## FAQ

**What is the default RTSP URL for Tapo cameras?**

The URL is `rtsp://username:password@CAMERA_IP:554/stream1` for the main stream and `stream2` for the sub stream. You must first create RTSP credentials in the Tapo app under Advanced Settings > Camera Account.

**Can I use Tapo cameras without the Tapo cloud service?**

Yes. Once you set up RTSP credentials via the Tapo app, you can access the camera's RTSP stream directly over your local network without any cloud dependency. The Tapo app is only needed for initial setup and credential configuration.

**What's the difference between TL-SC and Tapo cameras?**

The TL-SC series was TP-Link's older IP camera line (discontinued) with traditional web-based management. Tapo is the current smart home camera brand with app-based setup. Both support RTSP but use different URL patterns and authentication methods.

**Do Tapo cameras support H.265?**

Select models like the Tapo C320WS and C520WS support H.265 encoding. Most Tapo cameras use H.264. Check your specific model's specifications for H.265 support.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Reolink Connection Guide](reolink.md) — Consumer alternative with RTSP
- [Mercusys Connection Guide](mercusys.md) — TP-Link sub-brand, same firmware
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
