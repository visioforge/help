---
title: ACTi IP Camera RTSP URL and C# .NET Connection Guide
description: ACTi A, B, D, E series and legacy ACM/KCM/TCM camera RTSP URL patterns for C# .NET. Integrate with VisioForge Video Capture SDK code samples.
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
  - Encoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.264
  - H.265
  - MJPEG
  - C#

---

# How to Connect to ACTi IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**ACTi Corporation** is a Taiwanese manufacturer of IP surveillance cameras and video management solutions. Headquartered in Taipei, Taiwan, ACTi targets professional and enterprise markets with a wide range of fixed, dome, bullet, and PTZ cameras. ACTi is known for its current A/B/D/E series cameras and legacy ACM, KCM, and TCM product lines.

**Key facts:**

- **Product lines:** A-series (box), B-series (bullet/zoom), D-series (dome), E-series (hemispheric dome), KCM (legacy dome), ACM (legacy box/dome), TCM (legacy box)
- **Protocol support:** RTSP, ONVIF (current A/B/D/E series), HTTP/CGI
- **Default RTSP port:** 7070 (most models), 554 (some legacy models)
- **Default credentials:** Admin / 123456 (current models), admin / admin (legacy)
- **ONVIF support:** Yes (current A/B/D/E series)
- **Video codecs:** H.264, H.265 (E-series), MJPEG

!!! warning "Non-standard port"
    ACTi cameras use **port 7070** by default for RTSP, not the standard port 554. This is the most common connection issue when integrating ACTi cameras.

## RTSP URL Patterns

### Current Models (A/B/D/E Series)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:7070//stream1` | Primary stream (note double slash) |
| Root stream | `rtsp://IP:7070/` | Fallback |
| H.264 direct | `rtsp://IP:7070/h264` | Explicit codec selection |
| ONVIF stream | `rtsp://IP:7070//onvif-stream1` | ONVIF variant |

!!! info "Double slash before stream1"
    ACTi cameras use a **double forward slash** before `stream1` in their RTSP URLs: `rtsp://IP:7070//stream1`. This is intentional and required for most current models.

### Model-Specific URLs

| Model Series | RTSP URL | Type | Notes |
|-------------|----------|------|-------|
| D11, D21, D31, D32 | `rtsp://IP:7070//stream1` | Dome | Current |
| D42, D51, D52, D55, D72 | `rtsp://IP:7070//stream1` | Dome | Current |
| E12, E32, E33, E43, E46 | `rtsp://IP:7070//stream1` | Hemispheric | Current, H.265 capable |
| E51, E52, E63, E65, E73 | `rtsp://IP:7070//stream1` | Hemispheric | Current, H.265 capable |
| E82, E84, E96 | `rtsp://IP:7070//stream1` | Hemispheric | Current, H.265 capable |
| B53, B87, B95 | `rtsp://IP:7070//stream1` | Bullet/Zoom | Current |
| A-series (box) | `rtsp://IP:7070//stream1` | Box | Current |

### Legacy Models

Legacy ACTi cameras may use port 554 or 7070 depending on the model and firmware version:

| Model Series | RTSP URL | Type | Notes |
|-------------|----------|------|-------|
| ACM-1011 | `rtsp://IP:554/` or `rtsp://IP:7070/` | Box | Legacy |
| ACM-3401 | `rtsp://IP:554/` or `rtsp://IP:7070/` | Dome | Legacy |
| ACM-5601 | `rtsp://IP:554/` or `rtsp://IP:7070/` | Box | Legacy |
| ACM-7411 | `rtsp://IP:554/` or `rtsp://IP:7070/` | Dome | Legacy |
| KCM-3311 | `rtsp://IP:7070/` | Dome | Legacy |
| KCM-5611 | `rtsp://IP:7070/` | Dome | Legacy |
| KCM-7211 | `rtsp://IP:7070/` | Dome | Legacy |
| TCM-1231 | `rtsp://IP:7070/` | Box | Legacy |
| TCM-3511 | `rtsp://IP:7070/` | Box | Legacy |
| TCM-5111 | `rtsp://IP:7070/` | Box | Legacy |
| TCM-5311 | `rtsp://IP:7070/` | Box | Legacy |

## Connecting with VisioForge SDK

Use your ACTi camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// ACTi D/E series camera, main stream -- note port 7070, not 554!
var uri = new Uri("rtsp://192.168.1.50:7070//stream1");
var username = "Admin";
var password = "123456";
```

For legacy ACM models that use port 554, change the port accordingly. For a simpler root stream, use `rtsp://IP:7070/` as the URL.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| CGI Snapshot | `http://IP/cgi-bin/encoder?USER=USERNAME&PWD=PASSWORD&SNAPSHOT` | Authenticated snapshot |
| HTTP Streaming | `http://IP/cgi-bin/cmd/system?GET_STREAM&USER=USERNAME&PWD=PASSWORD` | Continuous stream |
| JPEG Image | `http://IP/jpg/image.jpg` | Direct JPEG |
| JPEG (alt) | `http://IP/now.jpg` | Alternative snapshot path |

## Troubleshooting

### Port 7070, not 554

The most common ACTi connection issue is using the standard port 554. ACTi cameras default to **port 7070** for RTSP. If your connection times out or is refused, verify you are using the correct port.

- Correct: `rtsp://IP:7070//stream1`
- Likely incorrect: `rtsp://IP:554//stream1` (unless using a legacy ACM model)

### Double slash before stream1

ACTi current-generation cameras use a **double forward slash** before `stream1`:

- Correct: `rtsp://IP:7070//stream1`
- May not work: `rtsp://IP:7070/stream1`

### Default credentials differ by generation

- **Current models (A/B/D/E series):** Username `Admin` (capital A), password `123456`
- **Legacy models (ACM/KCM/TCM):** Username `admin` (lowercase), password `admin`

Always change default credentials before deploying cameras on a production network.

### Legacy ACM models and port 554

Some older ACM-series cameras (ACM-1011, ACM-3401, ACM-5601, ACM-7411) may use port 554 instead of 7070. If port 7070 fails on a legacy model, try port 554 with the root URL `rtsp://IP:554/`.

### ONVIF availability

ONVIF is only supported on current-generation cameras (A, B, D, and E series). Legacy ACM, KCM, and TCM cameras do not support ONVIF. For legacy models, use direct RTSP or HTTP URLs.

## FAQ

**What is the default RTSP URL for ACTi cameras?**

For current ACTi cameras (A/B/D/E series), use `rtsp://Admin:123456@CAMERA_IP:7070//stream1`. Note the non-standard port 7070 and double slash before `stream1`. For legacy models, try `rtsp://admin:admin@CAMERA_IP:7070/` or `rtsp://admin:admin@CAMERA_IP:554/`.

**Why does ACTi use port 7070 instead of 554?**

ACTi chose port 7070 as their default RTSP port. This can be changed in the camera's web interface, but the factory default is 7070 for most models. Some legacy ACM-series cameras default to port 554.

**Does ACTi support H.265?**

Current E-series cameras (hemispheric dome models) support H.265 encoding. Other current series (A, B, D) primarily use H.264. Legacy models (ACM, KCM, TCM) support H.264 and MJPEG only.

**What is the difference between ACTi product series?**

ACTi organizes cameras by letter: **A** = box cameras, **B** = bullet and zoom cameras, **D** = dome cameras, **E** = hemispheric dome cameras. Legacy product lines include ACM (box/dome), KCM (dome), and TCM (box).

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Vivotek Connection Guide](vivotek.md) — Taiwanese enterprise cameras
- [GeoVision Connection Guide](geovision.md) — Taiwanese professional cameras
- [ONVIF IP Camera Integration](../videocapture/video-sources/ip-cameras/onvif.md) — ACTi ONVIF device setup
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
