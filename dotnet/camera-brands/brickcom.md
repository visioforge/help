---
title: How to Connect to BrickCom IP Camera in C# .NET
description: Connect to BrickCom cameras in C# .NET with RTSP URL patterns and code samples for CB, MB, OB, VD, WCB, WOB, and MD series models.
---

# How to Connect to BrickCom IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**BrickCom** (Brickcom Corporation) is a Taiwanese professional IP camera manufacturer headquartered in Taipei, Taiwan. Founded in 2004, BrickCom targets professional security and industrial surveillance markets with a wide range of form factors including bullet, dome, cube, and specialty cameras. The brand is known for its straightforward channel-based RTSP URL pattern across its product lines.

**Key facts:**

- **Product lines:** CB (cube), MB (mini bullet), OB (outdoor bullet), VD (vandal dome), FD (fixed dome), MD (multi-directional), WCB/WOB (wireless)
- **Channel-based URL pattern:** `/channel1` for main stream, `/channel2` for sub stream
- **Default RTSP port:** 554
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (most models)
- **Video codecs:** H.264, MJPEG
- **Primary RTSP URL:** `rtsp://IP:554/channel1`

!!! tip "Channel numbering"
    BrickCom uses simple channel-based URLs. Use `/channel1` for the primary (high-quality) stream and `/channel2` for the secondary (lower bandwidth) stream.

## RTSP URL Patterns

### Standard URL Format

BrickCom cameras use a simple channel-based RTSP URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/channel1
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel1` | Main stream | Primary stream (highest resolution) |
| `channel2` | Sub stream | Secondary stream (lower resolution, less bandwidth) |

### Camera Models

| Model | Type | Main Stream URL | Notes |
|-------|------|----------------|-------|
| CB-100 (cube) | Cube | `rtsp://IP:554/channel1` | Indoor cube camera |
| MB-300Ap (mini bullet) | Mini Bullet | `rtsp://IP:554/channel1` | Compact bullet form factor |
| OB-100Ap (outdoor bullet) | Outdoor Bullet | `rtsp://IP:554/channel1` | Weatherproof bullet |
| OB-300Af (outdoor bullet) | Outdoor Bullet | `rtsp://IP:554/channel1` | Auto-focus bullet |
| VD-130Ae (vandal dome) | Vandal Dome | `rtsp://IP:554/channel1` | IK10-rated dome |
| VD-301AF (vandal dome) | Vandal Dome | `rtsp://IP:554/channel1` | Auto-focus vandal dome |
| VD-500Af (vandal dome) | Vandal Dome | `rtsp://IP:554/channel1` | 5MP vandal dome |
| WCB-100Ap (wireless cube) | Wireless Cube | `rtsp://IP:554/channel1` | Wi-Fi cube camera |
| WCB-300AP (wireless cube) | Wireless Cube | `rtsp://IP:554/channel1` | Wi-Fi cube, 3MP |
| WOB-100Ae (wireless bullet) | Wireless Bullet | `rtsp://IP:554/channel1` | Wi-Fi outdoor bullet |
| MD-500AP-360-A1 (multi-dome) | Multi-Directional | `rtsp://IP:554/channel1` | 360-degree multi-sensor |

### Alternative URL Formats

Some BrickCom models and firmware versions support these additional RTSP URLs:

| URL Pattern | Supported Models | Notes |
|-------------|-----------------|-------|
| `rtsp://IP:554/channel1` | Most models | Standard (recommended) |
| `rtsp://IP:554/h264` | Various | Direct H.264 stream |
| `rtsp://IP//ONVIF/channel2` | VD-500Af, WCB-100Ap | ONVIF sub stream |
| `rtsp://IP/stream/bidirect/channel1` | Select models | Bidirectional stream with audio |

## Connecting with VisioForge SDK

Use your BrickCom camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// BrickCom OB-300Af, main stream
var uri = new Uri("rtsp://192.168.1.90:554/channel1");
var username = "admin";
var password = "admin";
```

For sub-stream access, use `/channel2` instead of `/channel1`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snapshot.jpg` | No authentication required |
| JPEG Snapshot (auth) | `http://IP/snapshot.jpg?user=USER&pwd=PASS` | URL-based authentication |
| Channel Snapshot | `http://IP/snapshot.jpg?user=USER&pwd=PASS&strm=1` | Specific channel with auth |
| CGI Snapshot | `http://IP/cgi-bin/media.cgi?action=getSnapshot` | CGI-based snapshot |
| HTTP Channel Stream | `http://IP/channel2` | HTTP sub stream |

## Troubleshooting

### "401 Unauthorized" error

BrickCom cameras ship with default credentials of **admin / admin**. If you have changed the password via the web interface:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Navigate to **Configuration > User Management**
3. Verify your credentials
4. Use those credentials in your RTSP URL

### Channel URL not connecting

If `rtsp://IP:554/channel1` does not work, try the alternative H.264 URL:

- `rtsp://IP:554/h264` -- direct H.264 stream without channel specification
- Some older firmware versions may require the ONVIF format: `rtsp://IP//ONVIF/channel2`

### ONVIF discovery issues

BrickCom cameras support ONVIF on most models. If ONVIF discovery fails:

1. Access the web interface at `http://CAMERA_IP`
2. Navigate to **Configuration > Network > ONVIF**
3. Ensure ONVIF is enabled
4. Verify the ONVIF port (default: 80 or 8080)

### Wireless models (WCB/WOB) connection drops

Wireless BrickCom cameras (WCB and WOB series) may experience intermittent RTSP disconnections on congested Wi-Fi networks. Use the sub stream (`/channel2`) for lower bandwidth requirements, or connect via Ethernet for maximum reliability.

## FAQ

**What is the default RTSP URL for BrickCom cameras?**

The URL is `rtsp://admin:admin@CAMERA_IP:554/channel1` for the main stream. Use `/channel2` for the sub stream with lower resolution and bandwidth.

**Do BrickCom cameras support ONVIF?**

Yes. Most current BrickCom models support ONVIF. Some models also expose an ONVIF-specific RTSP path at `rtsp://IP//ONVIF/channel2`.

**What is the difference between channel1 and channel2?**

`/channel1` provides the primary high-resolution stream and `/channel2` provides a secondary lower-resolution stream suitable for thumbnails, mobile viewing, or bandwidth-constrained scenarios.

**Can I access multiple streams simultaneously?**

Yes. BrickCom cameras support concurrent connections to both `/channel1` and `/channel2`. The maximum number of simultaneous connections depends on the specific model.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [AVTech Connection Guide](avtech.md) — Taiwanese industrial cameras
- [LILIN Connection Guide](lilin.md) — Taiwanese professional cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
