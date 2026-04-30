---
title: Toshiba IP Camera RTSP URL and Streaming in C# .NET
description: Toshiba IK-WB, IK-WD, IK-WR, and IK-WP camera RTSP URL patterns for C# .NET. Stream and record with VisioForge Video Capture SDK integration.
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

# How to Connect to Toshiba IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Toshiba** (Toshiba Corporation) is a Japanese multinational conglomerate headquartered in Tokyo, Japan. Toshiba's security division produced the **IK-W series** of IP cameras, covering box, dome, bullet, and PTZ form factors. Toshiba has since exited the standalone security camera market and sold its surveillance business. Despite discontinuation, many IK-W series cameras remain deployed in commercial and industrial installations worldwide.

**Key facts:**

- **Product lines:** IK-WB (box cameras), IK-WD (dome cameras), IK-WR (bullet/rugged cameras), IK-WP (PTZ cameras)
- **Protocol support:** RTSP, HTTP/CGI, ONVIF (limited, newer models only)
- **Default RTSP port:** 554
- **Default credentials:** admin / 1234
- **ONVIF support:** Limited (newer IK-W14/16/30/70/80 models only)
- **Video codecs:** H.264 (IK-W14/16/30/70/80 series), MJPEG (older models)

!!! warning "Discontinued Product Line"
    Toshiba has exited the IP camera market and sold its surveillance business. No new firmware updates or official support are available. Many early IK-WB models (01A, 02A, 11A) support HTTP snapshot only and do not provide RTSP streaming.

## RTSP URL Patterns

### Standard URL Format

Toshiba IK-W series cameras use the `live.sdp` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live.sdp
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `live.sdp` | Primary stream | Main H.264 stream (highest resolution) |
| `live2.sdp` | Sub stream | Secondary stream (lower resolution) |
| `live3.sdp` | Third stream | Third stream (mobile-optimized, select models) |

### Camera Models - RTSP Streams

| Model | Type | Main Stream URL | Sub Stream URL | Notes |
|-------|------|----------------|----------------|-------|
| IK-WB16A | Box | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WB80A | Box | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD01A | Dome | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WD12A | Dome | `rtsp://IP:554//live.sdp` | -- | Double-slash path |
| IK-WD14A | Dome | `rtsp://IP:554/live.sdp` | `rtsp://IP:554/live2.sdp` | Also supports `live3.sdp` |
| IK-WR04A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR12A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WR14A | Bullet | `rtsp://IP:554/live.sdp` | -- | H.264 |
| IK-WP41A | PTZ | `rtsp://IP:554/live.sdp` | -- | H.264 |

### Models by Series

#### IK-WB Series (Box Cameras)

| Model | Streaming | Protocol |
|-------|-----------|----------|
| IK-WB01A | HTTP snapshot only | HTTP |
| IK-WB02A | HTTP snapshot only | HTTP |
| IK-WB11A | HTTP snapshot only | HTTP |
| IK-WB15A | HTTP snapshot + CGI | HTTP |
| IK-WB16A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB16A-W | RTSP `live.sdp`, `live3.sdp` | RTSP + HTTP |
| IK-WB21A | HTTP CGI only | HTTP |
| IK-WB30A | RTSP `live.sdp` | RTSP + HTTP |
| IK-WB70A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |
| IK-WB80A | RTSP `live.sdp` | RTSP + HTTP + MJPEG |

#### IK-WD Series (Dome Cameras)

| Model | Streaming | Protocol |
|-------|-----------|----------|
| IK-WD01A | RTSP `live.sdp` | RTSP |
| IK-WD12A | RTSP `//live.sdp` | RTSP (double-slash) |
| IK-WD14A | RTSP `live.sdp`, `live2.sdp`, `live3.sdp` | RTSP (multi-stream) |

#### IK-WR Series (Bullet/Rugged Cameras)

| Model | Streaming | Protocol |
|-------|-----------|----------|
| IK-WR01A | HTTP snapshot only | HTTP |
| IK-WR02A | HTTP snapshot only | HTTP |
| IK-WR04A | RTSP `live.sdp` | RTSP |
| IK-WR12A | RTSP `live.sdp` | RTSP + MJPEG |
| IK-WR14A | RTSP `live.sdp` | RTSP + HTTP |

#### IK-WP Series (PTZ Cameras)

| Model | Streaming | Protocol |
|-------|-----------|----------|
| IK-WP41A | RTSP `live.sdp` | RTSP |

### Alternative URL Formats

Some Toshiba models use a double-slash in the RTSP path:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/live.sdp` | Standard (recommended) |
| `rtsp://IP:554//live.sdp` | Double-slash variant (IK-WD12A, some IK-WD14A units) |
| `rtsp://IP:554/live2.sdp` | Sub stream (IK-WD14A) |
| `rtsp://IP:554/live3.sdp` | Third stream (IK-WB16A-W, IK-WD14A) |

!!! tip "Double-Slash Path"
    If `rtsp://IP:554/live.sdp` does not work on your Toshiba camera, try the double-slash variant `rtsp://IP:554//live.sdp`. Some IK-WD models require this format.

## Connecting with VisioForge SDK

Use your Toshiba camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Toshiba IK-WD14A, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live.sdp");
var username = "admin";
var password = "1234";
```

For sub-stream access on supported models, use `live2.sdp` instead of `live.sdp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Compatible Models | Notes |
|------|-------------|-------------------|-------|
| JPEG Snapshot | `http://IP/__live.jpg?&&&` | IK-WB01A, WB11A, WB15A, WB16A-W, WB21A | Note underscore prefix |
| CGI Snapshot | `http://IP/GetData.cgi` | IK-WB01A, WB11A, WB15A, WB21A, WR01A | Basic snapshot |
| Configurable Snapshot | `http://IP/GetData.cgi?CH=CHANNEL&Codec=jpeg&Size=WIDTHxHEIGHT` | IK-WB series | Set resolution and channel |
| Resolution Snapshot | `http://IP/cgi-bin/viewer/video.jpg?resolution=WIDTHxHEIGHT` | IK-WB15A, WB16A, WB30A, WB70A, WR12A, WR14A | Specify output resolution |
| Simple Snapshot | `http://IP/Jpeg/CamImg.jpg` | IK-WB02A, WR01A | Basic JPEG capture |
| MJPEG Stream | `http://IP/video.mjpg` | IK-WB70A, WB80A, WR12A | Continuous MJPEG stream |

!!! note "HTTP-Only Models"
    Early Toshiba models (IK-WB01A, WB02A, WB11A, WR01A, WR02A) do not support RTSP. For these cameras, use HTTP snapshot URLs or MJPEG streams. You can capture these via the VisioForge SDK's HTTP source or MJPEG source modes.

## Troubleshooting

### Camera is HTTP-only (no RTSP)

Many early IK-WB models (01A, 02A, 11A) and IK-WR models (01A, 02A) do not support RTSP streaming at all. These cameras only provide HTTP snapshot and CGI endpoints. If your camera does not respond on port 554, check whether it is an HTTP-only model from the tables above.

### Underscore prefix in snapshot URL

The `__live.jpg` snapshot URL uses a **double underscore prefix**, which is unusual. Make sure to include both underscores:

```
http://192.168.1.90/__live.jpg?&&&
```

The trailing `&&&` characters are also required on some firmware versions.

### Double-slash in RTSP path

Some IK-WD series cameras (WD12A, certain WD14A units) require a double forward slash in the RTSP path:

```
rtsp://admin:1234@192.168.1.90:554//live.sdp
```

If the standard single-slash URL does not connect, try this variant.

### No firmware updates available

Toshiba has exited the security camera market. No new firmware, patches, or official support channels are available. If you encounter bugs or security vulnerabilities, consider replacing the camera with a currently supported model.

### Default credentials not working

The factory default credentials are **admin / 1234**. If these do not work, the password may have been changed by a previous administrator. A hardware factory reset (usually a pinhole reset button) will restore defaults on most models.

## FAQ

**What is the default RTSP URL for Toshiba IP cameras?**

The primary RTSP URL is `rtsp://admin:1234@CAMERA_IP:554/live.sdp` for models that support RTSP streaming. Use `live2.sdp` for the sub stream on models like the IK-WD14A. Note that older IK-WB01A/02A/11A and IK-WR01A/02A models do not support RTSP at all.

**Are Toshiba IP cameras still supported?**

No. Toshiba sold its surveillance business and exited the IP camera market. No firmware updates, new models, or official technical support are available. Existing cameras continue to function but will not receive security patches or feature updates.

**Do Toshiba cameras support ONVIF?**

Only newer models in the IK-W14/16/30/70/80 range have limited ONVIF support. Older models (IK-WB01A through WB11A, IK-WR01A/02A) do not support ONVIF. For ONVIF discovery and configuration, use only the supported models.

**Why does my Toshiba camera only provide snapshots, not video streams?**

Early Toshiba IK-W models were designed as network snapshot cameras and do not include an RTSP server. These models (IK-WB01A, WB02A, WB11A, WR01A, WR02A) only support HTTP-based JPEG snapshots and CGI endpoints. To get continuous video, you need a newer model from the IK-W14/16/30/70/80 series.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Sony Connection Guide](sony.md) — Japanese enterprise cameras
- [JVC Connection Guide](jvc.md) — Japanese legacy surveillance brand
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
