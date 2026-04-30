---
title: Tiandy IP Camera in C# .NET — RTSP, ONVIF Setup Guide
description: Connect Tiandy IP cameras (TC-C, TC-NC, TC-A, TC-R NVR) to C# / .NET apps via RTSP and ONVIF. Default stream URLs, credentials, H.265 configs. Code sample.
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
  - Decoding
  - IP Camera
  - RTSP
  - ONVIF
  - H.265
  - MJPEG
  - C#

---

# How to Connect to Tiandy IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Tiandy Technologies** (Tiandy Technologies Co., Ltd.) is a Chinese video surveillance manufacturer headquartered in Tianjin, China. Founded in 1994, Tiandy is one of China's largest security equipment manufacturers and has been expanding rapidly into international markets across Asia, the Middle East, Africa, and Latin America. Tiandy specializes in AI-powered IP cameras, NVRs, and integrated video management solutions.

**Key facts:**

- **Product lines:** TC-C (current IP cameras), TC-NC (legacy IP), TC-A (AI analytics), TC-R (NVRs), TC-NR (network recorders)
- **Protocol support:** RTSP, ONVIF Profile S/T, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / 1111 (older models) or admin / admin123 (varies by region)
- **ONVIF support:** Yes (current models)
- **Video codecs:** H.264, H.265 (SuperH.265), MJPEG
- **AI features:** Smart H.265+, face detection, perimeter protection, people counting

## RTSP URL Patterns

### Standard URL Format

Tiandy cameras use a channel and stream-based RTSP URL structure:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Camera channel (1 for standalone cameras) |
| `subtype` | 0 | Main stream (highest resolution) |
| `subtype` | 1 | Sub stream (lower resolution) |

!!! info "Dahua-Compatible URL Format"
    Many Tiandy cameras use the same `cam/realmonitor` RTSP URL format as Dahua cameras. If you are familiar with Dahua integration, the same URL patterns may work with Tiandy. See our [Dahua connection guide](dahua.md) for additional details.

### Alternative URL Formats

| URL Pattern | Description |
|-------------|-------------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-compatible (many models) |
| `rtsp://IP:554/live/ch0` | Main stream (legacy format) |
| `rtsp://IP:554/live/ch1` | Sub stream (legacy format) |
| `rtsp://IP:554/media/video1` | Uniview-compatible (some models) |
| `rtsp://IP:554/Streaming/Channels/101` | Hikvision-compatible (some OEM models) |
| `rtsp://IP:554/h264` | Simple H.264 stream path |

### Camera Models

| Model Series | Resolution | Main Stream URL | Audio |
|-------------|-----------|----------------|-------|
| TC-C32JN (2MP bullet) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C34JN (4MP bullet) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C35JN (5MP bullet) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C38JN (4K bullet) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| TC-C32DN (2MP dome) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | No |
| TC-C34DN (4MP dome) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| TC-C32EP (2MP turret) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| TC-C34EP (4MP turret) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| TC-A32E2T (2MP AI) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| TC-C32WP (2MP WiFi) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |

### NVR Channel URLs

For Tiandy NVRs (TC-R3100, TC-R3200, TC-NR series):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Camera 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Camera N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

## Connecting with VisioForge SDK

Use your Tiandy camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Tiandy TC-C34JN (4MP bullet), main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `subtype=1` instead of `subtype=0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requires digest authentication |
| MJPEG Stream | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | Continuous MJPEG |

## Troubleshooting

### Multiple URL formats

Tiandy cameras may use different RTSP URL formats depending on firmware version and model. If one format does not work, try alternatives in this order:

1. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (Dahua-compatible, most common)
2. `rtsp://IP:554/live/ch0` (legacy Tiandy format)
3. `rtsp://IP:554/h264` (simple path)

### Default credentials vary

Tiandy default passwords differ by model and region. Common defaults include:

- `admin` / `1111`
- `admin` / `admin123`
- `admin` / `123456`

If none of these work, the camera may require initial activation via the web interface or Tiandy's EasyLive utility.

### SuperH.265 codec

Tiandy's SuperH.265 is a proprietary optimization that produces standard H.265/HEVC streams. No special decoder is required. The VisioForge SDK handles H.265 streams natively.

## FAQ

**What is the default RTSP URL for Tiandy cameras?**

Most Tiandy cameras use `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` for the main stream, which is the same format as Dahua cameras. Some older models use `rtsp://IP:554/live/ch0` instead.

**Are Tiandy cameras Dahua OEMs?**

No. Tiandy is an independent manufacturer with its own hardware and firmware. However, some Tiandy firmware uses the same RTSP URL format as Dahua (`cam/realmonitor`), which is common across several Chinese surveillance manufacturers.

**Do Tiandy cameras support ONVIF?**

Yes. Current Tiandy models support ONVIF Profile S and Profile T. ONVIF must be enabled in the camera's web interface under network settings. Some models require creating a separate ONVIF user account.

**Which Tiandy camera series should I choose?**

**TC-C** is the current mainstream series. The number after "TC-C3" indicates resolution: **2** = 2MP, **4** = 4MP, **5** = 5MP, **8** = 4K. The suffix letters indicate form factor: **JN** = bullet, **DN** = dome, **EP** = turret/eyeball, **WP** = WiFi.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Similar URL format
- [Uniview Connection Guide](uniview.md) — Another major Chinese surveillance brand
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
