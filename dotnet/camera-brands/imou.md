---
title: Imou RTSP URL Guide: Connect to IP Cameras in C# .NET
description: Stream Imou Cruiser, Ranger, Bullet, and Cell cameras via RTSP in your C# .NET app. Includes URL format, ONVIF tips, and VisioForge SDK code samples.
---

# How to Connect to Imou IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Imou** (pronounced "ee-moo") is a consumer smart home and security camera brand owned by **Dahua Technology**. Launched in 2019, Imou targets the consumer and small business market with WiFi cameras, battery cameras, doorbells, and home security kits. Imou cameras use Dahua firmware and RTSP URL patterns.

**Key facts:**

- **Product lines:** Cruiser (outdoor PT), Ranger (indoor PT), Bullet (fixed outdoor), Cell (battery), Versa (versatile), Rex (indoor)
- **Protocol support:** RTSP (must be enabled on some models), ONVIF (select models), Imou Life cloud
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (or admin / imou + serial number suffix)
- **ONVIF support:** Yes (most wired models)
- **Video codecs:** H.264, H.265 (select models)
- **Parent company:** Dahua Technology

!!! info "Imou = Dahua Consumer Brand"
    Imou cameras use Dahua firmware and the same `cam/realmonitor` RTSP URL format as Dahua cameras. See our [Dahua connection guide](dahua.md) for additional details.

## RTSP URL Patterns

### Standard URL Format

Imou cameras use the Dahua `cam/realmonitor` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/cam/realmonitor?channel=1&subtype=0
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `channel` | 1 | Camera channel (always 1 for standalone cameras) |
| `subtype` | 0 | Main stream (highest resolution) |
| `subtype` | 1 | Sub stream (lower resolution, less bandwidth) |

### Camera Models

| Model | Type | Main Stream URL | Audio |
|-------|------|----------------|-------|
| Cruiser SE+ 4MP | Outdoor PTZ | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Cruiser 2E 4MP | Outdoor PTZ | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Ranger 2 (IPC-A22EP) | Indoor PTZ | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Ranger SE 4MP | Indoor PTZ | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Rex 3D (IPC-GS7EP) | Indoor PTZ | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Bullet 2E (IPC-F22FP) | Outdoor fixed | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Bullet 2S (IPC-F26FP) | Outdoor fixed | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Versa 4MP | Indoor/outdoor | `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Yes |
| Cell 2 | Battery outdoor | Limited — see note | Yes |
| Cell Go | Battery mini | No RTSP | No |

!!! warning "Battery Models"
    Imou battery-powered cameras (Cell series) have limited or no RTSP support. The Cell 2 may support RTSP when connected to the Imou Base Station, but the Cell Go and other battery mini cameras are cloud-only devices.

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Standard (recommended) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0&unicast=true` | Force unicast |
| `rtsp://IP:554/live` | Simple path (some models) |

## Connecting with VisioForge SDK

Use your Imou camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Imou Cruiser SE+ 4MP, main stream
var uri = new Uri("rtsp://192.168.1.90:554/cam/realmonitor?channel=1&subtype=0");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `subtype=1` instead of `subtype=0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi?channel=1` | Requires basic auth |
| MJPEG Stream | `http://IP/cgi-bin/mjpg/video.cgi?channel=1&subtype=1` | Continuous MJPEG |

## Troubleshooting

### RTSP not accessible

Some Imou cameras require RTSP to be enabled through the Imou Life app:

1. Open **Imou Life** app → select your camera
2. Go to **Settings > Advanced Settings > RTSP**
3. Enable RTSP and note the password (may differ from app password)

### Default credentials

Imou password defaults vary by model and firmware:

- `admin` / `admin` (common on older models)
- `admin` / specific code (check camera label)
- Custom password set during Imou Life app setup

If RTSP login fails, check the RTSP password in the Imou Life app settings.

### WiFi camera IP address

Imou WiFi cameras get their IP from your router via DHCP. Find the camera's local IP in:

1. The Imou Life app → Device Info
2. Your router's DHCP client list
3. ONVIF discovery (if supported)

### Dahua web interface

Some Imou cameras expose the Dahua web interface at `http://CAMERA_IP`. This provides additional configuration options beyond the Imou Life app, including RTSP settings, video encoding, and network configuration.

## FAQ

**What is the default RTSP URL for Imou cameras?**

The standard URL is `rtsp://admin:password@CAMERA_IP:554/cam/realmonitor?channel=1&subtype=0` for the main stream. This is the same format as Dahua cameras. RTSP may need to be enabled in the Imou Life app first.

**Is Imou the same as Dahua?**

Imou is a consumer brand owned by Dahua Technology. Imou cameras use Dahua firmware and the same RTSP URL format (`cam/realmonitor`). The main differences are branding, consumer-focused features, and cloud service integration.

**Can I use Imou cameras without the cloud?**

Partially. You can access RTSP streams locally without the Imou cloud for live viewing and recording. However, initial camera setup requires the Imou Life app. Cloud-dependent features like smart alerts, cloud storage, and remote access require an Imou subscription.

**Do Imou cameras support ONVIF?**

Most wired and WiFi-connected Imou cameras support ONVIF. Battery-powered models generally do not. Check your camera's specifications in the Imou Life app.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Dahua Connection Guide](dahua.md) — Parent company, identical URL format
- [Amcrest Connection Guide](amcrest.md) — Another Dahua OEM brand
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
