---
title: LG IP Camera RTSP URL Setup and Streaming in C# .NET
description: LG SmartIP, LW, and LV series camera RTSP URL patterns for C# .NET. Stream wireless and wired models using VisioForge Video Capture SDK integration.
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
  - Webcam
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - MJPEG
  - C#

---

# How to Connect to LG IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**LG Electronics** is a South Korean multinational electronics company headquartered in Seoul, South Korea. LG produced IP cameras under the **SmartIP** brand and the **LW/LV series** for the professional security market. LG has since largely exited the IP camera business and sold its security division. A limited number of LG cameras remain deployed in commercial and enterprise installations.

**Key facts:**

- **Product lines:** LW Series (wireless dome/bullet), LV Series (wired), SmartIP (enterprise)
- **Protocol support:** RTSP, HTTP/CGI, ONVIF (SmartIP series), PSIA (select models)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (SmartIP series), limited or absent on LW/LV series
- **Video codecs:** H.264 (LW130W, LW332, SmartIP series), MJPEG (older models)

!!! warning "Discontinued Product Line"
    LG has exited the IP camera market and sold its security division. No new firmware updates or official support are available. Many database entries labeled "LG" are actually LG smartphones used as IP cameras via third-party apps -- only actual LG camera models (LW, LV, SmartIP, 7210R) are covered here.

## RTSP URL Patterns

### Standard URL Formats

LG cameras use several different RTSP URL patterns depending on the model series:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/video1+audio1
```

| URL Pattern | Description |
|-------------|-------------|
| `video1+audio1` | H.264 video with audio (LW series, 7210R) |
| `/` (root) | Root stream (LV series) |
| `//Master-0` | Master stream (LW130W alternate) |
| `camera.stm` | Camera stream (LW332) |
| `live1.sdp` | Live SDP stream (LW332 alternate) |
| PSIA channel URL | Enterprise PSIA streaming (SmartIP) |

### Camera Models - RTSP Streams

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| LW130W | Wireless Dome | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| LW130W | Wireless Dome | `rtsp://IP//Master-0` | Alternate Master stream |
| LW332 | Wireless Bullet | `rtsp://IP:554/camera.stm` | Camera stream |
| LW332 | Wireless Bullet | `rtsp://IP:554/live1.sdp` | Alternate SDP stream |
| LVW700 | Wired Dome | `rtsp://IP:554/` | Root stream |
| LVW701 | Wired Dome | `rtsp://IP:554/` | Root stream |
| 7210R | IP Camera | `rtsp://IP:554/video1+audio1` | H.264 + audio |
| SmartIP | Enterprise | `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | PSIA H.264 stream |

### Models by Series

#### LW Series (Wireless Cameras)

| Model | Streaming URLs | Protocol |
|-------|---------------|----------|
| LW130W | `video1+audio1` or `//Master-0` | RTSP + HTTP |
| LW332 | `camera.stm` or `live1.sdp` | RTSP + HTTP |

#### LV Series (Wired Cameras)

| Model | Streaming URLs | Protocol |
|-------|---------------|----------|
| LVW700 | Root stream (`rtsp://IP:554/`) | RTSP |
| LVW701 | Root stream (`rtsp://IP:554/`) | RTSP |

#### SmartIP Series (Enterprise Cameras)

| Model | Streaming URLs | Protocol |
|-------|---------------|----------|
| SmartIP models | PSIA channel URL | RTSP + PSIA + ONVIF |

#### Standalone Models

| Model | Streaming URLs | Protocol |
|-------|---------------|----------|
| 7210R | `video1+audio1` | RTSP |

### Alternative URL Formats

| URL Pattern | Models | Notes |
|-------------|--------|-------|
| `rtsp://IP:554/video1+audio1` | LW130W, 7210R | Standard (recommended for these models) |
| `rtsp://IP//Master-0` | LW130W | Alternate; note double-slash, no port |
| `rtsp://IP:554/camera.stm` | LW332 | Standard for LW332 |
| `rtsp://IP:554/live1.sdp` | LW332 | Alternate SDP format |
| `rtsp://IP:554/` | LVW700, LVW701 | Root stream (unusual but valid) |
| `rtsp://IP:554/PSIA/Streaming/channels/2?videoCodecType=H.264` | SmartIP | PSIA enterprise streaming |

!!! tip "Root Stream URL"
    The LVW700 and LVW701 use a root RTSP URL (`rtsp://IP:554/`) with no path component. This is unusual but valid. Make sure your RTSP client does not strip the trailing slash or add a default path.

## Connecting with VisioForge SDK

Use your LG camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// LG LW130W, H.264 + audio stream
var uri = new Uri("rtsp://192.168.1.90:554/video1+audio1");
var username = "admin";
var password = "admin";
```

For LW332 cameras, use `camera.stm` or `live1.sdp` as the stream path instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| CGI Snapshot | `http://IP/snapshot.cgi` | Standard CGI snapshot |
| JPEG Snapshot | `http://IP/snapshot.jpg` | Direct JPEG (LW130W) |
| Video Feed | `http://IP/videofeed` | Live video feed |
| MJPEG Stream | `http://IP/video?submenu=mjpg` | Continuous MJPEG stream |
| Profile Video | `http://IP/video?profile=CHANNEL` | Profile-based video selection |

## Troubleshooting

### Confusing LG cameras with LG smartphones

Many RTSP camera databases contain entries labeled "LG" that are actually **LG smartphones** (P350, P509, P970, Nexus 4, Optimus V, LS670) running third-party IP camera apps like "IP Webcam." These are not actual LG IP cameras. Look for model numbers starting with **LW**, **LV**, **SmartIP**, or **7210R** to identify genuine LG security cameras.

### Root stream URL not connecting

The LVW700 and LVW701 cameras use a bare root URL (`rtsp://IP:554/`) with no stream path. Some RTSP client libraries may not handle this correctly. If you experience connection issues:

1. Ensure the trailing slash is included
2. Try specifying the URL as `rtsp://admin:admin@192.168.1.90:554/`
3. Verify the camera is responding on port 554 using a network scanner

### Multiple URL formats per model

Some LG cameras (particularly the LW130W and LW332) support multiple RTSP URL formats. If one format fails, try the alternate:

- **LW130W:** Try `video1+audio1` first, then `//Master-0`
- **LW332:** Try `camera.stm` first, then `live1.sdp`

### PSIA streaming on SmartIP models

SmartIP enterprise cameras support PSIA (Physical Security Interoperability Alliance) streaming. The PSIA URL format is:

```
rtsp://admin:admin@192.168.1.90:554/PSIA/Streaming/channels/2?videoCodecType=H.264
```

Change the channel number to select different streams. PSIA requires authentication via the URL or HTTP digest.

### No firmware updates available

LG has exited the security camera market. No new firmware, patches, or official support channels are available. If you encounter bugs or security vulnerabilities, consider replacing the camera with a currently supported model.

## FAQ

**What is the default RTSP URL for LG IP cameras?**

It depends on the model series. For LW130W and 7210R cameras, use `rtsp://admin:admin@CAMERA_IP:554/video1+audio1`. For LW332, use `rtsp://admin:admin@CAMERA_IP:554/camera.stm`. For LVW700/LVW701, use `rtsp://admin:admin@CAMERA_IP:554/`. Each model series has a different URL pattern.

**Are LG IP cameras still supported?**

No. LG sold its security division and exited the IP camera market. No firmware updates, new models, or official technical support are available. Existing cameras continue to function but will not receive security patches or feature updates.

**Do LG cameras support ONVIF?**

Only the SmartIP enterprise series supports ONVIF. The consumer LW and LV series cameras have limited or no ONVIF support. SmartIP cameras also support PSIA as an alternative interoperability protocol.

**Why do I see LG phone models in IP camera databases?**

Many RTSP URL databases list LG smartphone models (Nexus 4, Optimus V, P509, etc.) as "LG cameras." These are actually phones running third-party apps like "IP Webcam" that turn the phone into a makeshift security camera. They are not actual LG IP camera products and use completely different URL patterns determined by the app.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Samsung Connection Guide](samsung.md) — Korean enterprise cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
