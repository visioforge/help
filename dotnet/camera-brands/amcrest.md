---
title: Amcrest IP Camera RTSP URL Format and C# .NET Setup
description: Amcrest IP2M, IP4M, IP5M, IP8M, and NVR RTSP URL patterns for C# .NET. Stream and record using VisioForge SDK with ONVIF auto-discovery support.
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
  - MJPEG
  - C#

---

# How to Connect to Amcrest IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Amcrest** (Amcrest Technologies LLC) is an American consumer security camera brand based in Houston, Texas. Amcrest cameras are manufactured by **Dahua Technology** and use Dahua firmware and protocols. This means Amcrest cameras share identical RTSP URL patterns, web interfaces, and API endpoints with Dahua cameras. Amcrest has become one of the best-selling IP camera brands on Amazon in North America.

**Key facts:**

- **Product lines:** IP2M (1080p), IP4M (4MP), IP5M (5MP), IP8M (4K/8MP), ASH (smart home), NV (NVRs)
- **Protocol support:** RTSP, ONVIF, HTTP/CGI, Amcrest Cloud, RTMP
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (must be changed on first login with newer firmware)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264 (all models), H.265 (IP4M and newer)
- **OEM base:** Dahua (identical RTSP URL format)

!!! info "Amcrest = Dahua"
    Amcrest cameras use Dahua firmware and the exact same RTSP URL format as Dahua cameras. If you're familiar with Dahua integration, Amcrest works identically. See our [Dahua connection guide](dahua.md) for additional details.

## RTSP URL Patterns

### Standard URL Format

Amcrest uses the Dahua `cam/realmonitor` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel` | 1, 2, 3... | Camera channel (1 for standalone cameras) |
| `subtype` | 0 | Main stream (highest resolution) |
| `subtype` | 1 | Sub stream (lower resolution, less bandwidth) |
| `subtype` | 2 | Third stream (if supported, mobile-optimized) |

### Camera Models

| Model | Resolution | Main Stream URL | Audio |
|-------|-----------|----------------|-------|
| IP2M-841 (1080p bullet) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| IP2M-844 (1080p dome) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| IP4M-1051 (4MP bullet) | 2688x1520 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| IP5M-T1179E (5MP turret) | 2592x1944 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| IP8M-2493E (4K bullet) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| IP8M-T2599E (4K turret) | 3840x2160 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| ASH-41 (pan/tilt) | 2560x1440 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| ASH-42 (indoor) | 1920x1080 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |

### NVR Channel URLs

For Amcrest NVRs (NV4108E, NV4216E, NV5216E, etc.):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=1` |
| Camera 2 | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=2&subtype=1` |
| Camera N | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=0` | `rtsp://IP:554/cam/realmonitor?channel=N&subtype=1` |

### Alternative URL Formats

Some older Amcrest models or firmware versions support these alternative URLs:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Standard (recommended) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Force unicast |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&proto=Onvif` | ONVIF-compatible |

## Connecting with VisioForge SDK

Use your Amcrest camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Amcrest IP4M-1051, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `subtype=1` instead of `subtype=0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requires basic auth |
| JPEG Snapshot (legacy) | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | URL-based auth |
| MJPEG Stream | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | Continuous MJPEG |
| Current image | `http://IP/onvif-http/snapshot?channel=1` | ONVIF HTTP snapshot |

## Troubleshooting

### "401 Unauthorized" error

Amcrest cameras with newer firmware require the password to be changed from the default on first login. If you haven't set up the camera via the web interface or Amcrest app yet:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Complete the initial setup wizard
3. Set a strong password
4. Use those credentials in your RTSP URL

### Port 554 vs custom port

Some Amcrest firmware versions allow changing the RTSP port. Check the port setting at:

- Web interface: **Setup > Network > Port > RTSP Port**
- Default is 554

### Stream type confusion

- `subtype=0` = Main stream (full resolution, higher bandwidth)
- `subtype=1` = Sub stream (reduced resolution, lower bandwidth)
- `subtype=2` = Third stream (if available, typically for mobile)

### Amcrest SmartHome (ASH) cameras

The ASH series cameras (like ASH-41, ASH-42) use the same RTSP URL format but some models require enabling RTSP in the Amcrest Smart Home app first.

## FAQ

**Are Amcrest and Dahua cameras the same?**

Amcrest cameras are manufactured by Dahua and use Dahua firmware. The RTSP URL format (`cam/realmonitor?channel=1&subtype=0`) is identical. Any code written for Dahua cameras works with Amcrest and vice versa. The main differences are branding, warranty, and North American support.

**What is the default RTSP URL for Amcrest cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` for the main stream. Replace `channel=1` with the appropriate channel for NVR setups and `subtype=0` with `subtype=1` for the sub stream.

**Do Amcrest cameras support ONVIF?**

Yes. All current Amcrest cameras support ONVIF Profile S and Profile T. ONVIF is enabled by default on most models.

**Can I use Amcrest cameras without the Amcrest cloud?**

Yes. RTSP, ONVIF, and the web interface all work locally without any cloud dependency. The Amcrest cloud service is optional and only needed for remote viewing through Amcrest's apps.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Same URL format (OEM base)
- [Lorex Connection Guide](lorex.md) — Also uses Dahua URL format
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
