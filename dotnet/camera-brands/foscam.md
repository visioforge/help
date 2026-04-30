---
title: Foscam IP Camera RTSP URL Guide and C# .NET Integration
description: Connect to Foscam cameras in C# .NET with RTSP and HTTP URL patterns, CGI API access, and code samples for FI, C1, C2, R2, and R4 models.
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

# How to Connect to Foscam IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Foscam** (Shenzhen Foscam Intelligent Technology Co., Ltd.) is a Chinese manufacturer specializing in consumer and small business IP cameras. Founded in 2007 and headquartered in Shenzhen, China, Foscam gained popularity for affordable Wi-Fi cameras and was one of the first brands to bring low-cost IP cameras to the consumer market.

**Key facts:**

- **Product lines:** FI-series (legacy pan/tilt), C1/C2 (indoor HD), R2/R4 (indoor pan/tilt), SD (outdoor), G-series (battery), VZ-series (doorbell)
- **Protocol support:** RTSP, HTTP/CGI, ONVIF (newer models), P2P
- **Default RTSP port:** 88 (not 554 -- this is unique to Foscam)
- **Default HTTP port:** 88
- **Default credentials:** admin / (blank password on older models); admin / (set during setup on newer models)
- **ONVIF support:** Partial (newer HD models only, e.g., C1, C2, R2, R4)
- **Video codecs:** H.264 (HD models), MJPEG (legacy models)

## RTSP URL Patterns

Foscam cameras use a non-standard port (88) and simple stream path names.

### URL Format

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:88/videoMain
```

!!! warning "Non-standard port"
    Foscam cameras typically use **port 88** for both RTSP and HTTP, not the standard port 554. This is the most common connection issue.

### HD Models (H.264)

| Model Series | RTSP URL | Stream | Audio |
|-------------|----------|--------|-------|
| C1 / C1 Lite (indoor) | `rtsp://IP:88/videoMain` | Main (720p) | Yes |
| C1 / C1 Lite (indoor) | `rtsp://IP:88/videoSub` | Sub (VGA) | Yes |
| C2 (indoor 1080p) | `rtsp://IP:88/videoMain` | Main (1080p) | Yes |
| C2 (indoor 1080p) | `rtsp://IP:88/videoSub` | Sub (VGA) | Yes |
| R2 (pan/tilt 1080p) | `rtsp://IP:88/videoMain` | Main (1080p) | Yes |
| R4 (pan/tilt 1440p) | `rtsp://IP:88/videoMain` | Main (2560x1440) | Yes |
| FI9821W V2 (pan/tilt) | `rtsp://IP:88/videoMain` | Main (720p) | Yes |
| FI9826W (pan/tilt/zoom) | `rtsp://IP:88/videoMain` | Main (960p) | Yes |
| FI9828P (outdoor PTZ) | `rtsp://IP:88/videoMain` | Main (960p) | Yes |
| FI9900P (outdoor bullet) | `rtsp://IP:88/videoMain` | Main (1080p) | Yes |
| SD2 (outdoor pan/tilt) | `rtsp://IP:88/videoMain` | Main (1080p) | Yes |

### Legacy Models (MJPEG only)

Older Foscam models (FI8904W, FI8910W, FI8918W, FI8919W) do not support RTSP. They use HTTP streaming only:

| Model | HTTP URL | Type | Audio |
|-------|----------|------|-------|
| FI8904W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | ASF stream | Yes |
| FI8910W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | ASF stream | Yes |
| FI8918W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | ASF stream | Yes |
| FI8919W | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | ASF stream | Yes |
| FI8904W | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | MJPEG | No |

### Alternative RTSP Ports

Some Foscam models can be configured for alternative ports:

| URL Pattern | Port | Notes |
|-------------|------|-------|
| `rtsp://IP:88/videoMain` | 88 | Default for most models |
| `rtsp://IP:554/videoMain` | 554 | If reconfigured in settings |
| `rtsp://IP:554/cam1/mpeg4` | 554 | Some OEM variants |
| `rtsp://IP:554/live1.sdp` | 554 | DCS-compatible firmware |

## Connecting with VisioForge SDK

Use your Foscam camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Foscam R2, main stream -- note port 88, not 554!
var uri = new Uri("rtsp://192.168.1.30:88/videoMain");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/videoSub` instead.

## Snapshot and MJPEG URLs

Foscam provides a CGI API for snapshots and control:

| Type | URL Pattern | Notes |
|------|-------------|-------|
| CGI Snapshot (HD) | `http://IP:88/cgi-bin/CGIProxy.fcgi?cmd=snapPicture2&usr=USER&pwd=PASS` | HD models |
| Legacy Snapshot | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS` | Legacy models |
| Snapshot (count) | `http://IP:88/snapshot.cgi?user=USER&pwd=PASS&count=0` | Single frame |
| MJPEG Stream (legacy) | `http://IP:88/videostream.cgi?user=USER&pwd=PASS&resolution=32` | VGA MJPEG |
| ASF Stream (legacy) | `http://IP:88/videostream.asf?user=USER&pwd=PASS` | ASF container |
| Video CGI | `http://IP:88/video.cgi?resolution=VGA` | Direct video |

## Troubleshooting

### Wrong port -- must use 88, not 554

The most common Foscam connection issue is using port 554. Foscam cameras default to **port 88** for all services (RTSP, HTTP, and CGI). If your connection times out, check the port number first.

### Legacy vs HD models

Foscam has two fundamentally different product generations:

- **Legacy (FI89xx):** MJPEG only, HTTP streaming via `videostream.asf` or `videostream.cgi`, no RTSP
- **HD (C1, C2, R2, R4, FI99xx):** H.264, RTSP via `videoMain`/`videoSub`, ONVIF support

If `rtsp://IP:88/videoMain` doesn't work, your camera is likely a legacy model -- use the HTTP streaming URLs instead.

### Blank/empty password

Older Foscam cameras ship with a blank password (username: `admin`, password: empty string). Newer firmware requires setting a password during initial setup. If authentication fails with a password, try an empty password for legacy models.

### Wi-Fi connection instability

Foscam Wi-Fi cameras can experience stream dropouts. Recommendations:

- Use TCP transport mode for reliability
- Position camera closer to the Wi-Fi router
- Use 2.4GHz Wi-Fi (better range) instead of 5GHz
- Reduce stream resolution to sub stream: `rtsp://IP:88/videoSub`

### ONVIF not available

ONVIF is only supported on newer HD models (C1, C2, R2, R4, FI99xx). Legacy FI89xx cameras do not support ONVIF. For legacy models, use direct HTTP/RTSP URLs instead.

## FAQ

**What is the default RTSP URL for Foscam cameras?**

For HD models, the URL is `rtsp://admin:password@CAMERA_IP:88/videoMain`. Note the non-standard port 88 (not 554). For legacy models (FI89xx series), use HTTP: `http://CAMERA_IP:88/videostream.asf?user=admin&pwd=password`.

**Why does Foscam use port 88 instead of the standard 554?**

Foscam chose port 88 as their default for all camera services to avoid conflicts with other network devices. You can change this in the camera's web interface under Settings > Network > Port, but the default is 88.

**Can I change the Foscam RTSP port to 554?**

Yes. Access the camera's web interface at `http://CAMERA_IP:88`, go to Settings > Network > Port, and change the RTSP port to 554. After saving and rebooting, you can use the standard port 554 in your RTSP URLs.

**Does Foscam support pan/tilt/zoom control via the SDK?**

Foscam PTZ models (R2, R4, FI9821, FI9826) support pan/tilt via their CGI API and ONVIF (HD models). You can send PTZ commands through ONVIF using the VisioForge SDK's PTZ control features.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [TP-Link Connection Guide](tp-link.md) — Consumer cameras with RTSP
- [D-Link Connection Guide](dlink.md) — Consumer segment peer
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
