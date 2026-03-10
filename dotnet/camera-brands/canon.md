---
title: Canon IP Camera RTSP URL and C# .NET Connection Guide
description: Canon VB-H, VB-M, VB-S, VB-R camera RTSP URL patterns for C# .NET. Integrate with VisioForge SDK for streaming and recording in WPF, WinForms, MAUI.
---

# How to Connect to Canon IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Canon Inc.** is a Japanese multinational corporation headquartered in Tokyo. Canon's IP camera division produces the **VB series** of network cameras targeting professional and enterprise surveillance markets. Canon cameras are known for their optical quality, leveraging Canon's expertise in lens manufacturing. Canon has been reducing its IP camera lineup in recent years, focusing on higher-end models.

**Key facts:**

- **Product lines:** VB-H series (box/PTZ, current), VB-M series (PTZ/compact), VB-S series (compact), VB-R series (PTZ), VB-C series (legacy PTZ)
- **Protocol support:** RTSP, ONVIF (VB-H and VB-M series), HTTP/CGI with proprietary `-wvhttp-01-` path
- **Default RTSP port:** 554
- **Default credentials:** root / (camera-specific) or admin / admin (varies by model)
- **ONVIF support:** Yes (VB-H and VB-M series)
- **Video codecs:** H.264, H.265 (VB-H47, VB-H761 series), MJPEG

## RTSP URL Patterns

Canon cameras use profile-based streaming with channel and profile identifiers in the URL path.

### Current Models (VB-H / VB-M / VB-S / VB-R Series)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Channel-based | `rtsp://IP:554/cam1/h264` | Channel 1, H.264 |
| Profile stream | `rtsp://IP:554//stream/profile1=r` | Profile 1, read mode (note double slash) |
| Profile (short) | `rtsp://IP:554/profile1=r` | Profile 1, shorter variant |
| Unicast profile | `rtsp://IP/profile1=u` | Unicast mode, no port |

!!! info "Profile-based streaming"
    Canon cameras use profile identifiers with access modes: `profile1=r` for **read** (multicast-capable) and `profile1=u` for **unicast** (direct connection). Use `=r` for general access and `=u` when connecting directly without multicast.

### Model-Specific URLs

| Model | RTSP URL | Type | Notes |
|-------|----------|------|-------|
| VB-H41 | `rtsp://IP:554//stream/profile1=r` | Fixed box | Profile with double slash |
| VB-H43 / VB-H45 | `rtsp://IP:554/cam1/h264` | Fixed box | Channel-based |
| VB-H47 | `rtsp://IP:554/cam1/h264` | Fixed box | H.265 capable |
| VB-H610D / VB-H610VE | `rtsp://IP:554/cam1/h264` | Fixed dome | Current |
| VB-H730F | `rtsp://IP:554/cam1/h264` | Fixed dome | Fisheye |
| VB-H751LE | `rtsp://IP:554/cam1/h264` | Fixed bullet | Outdoor |
| VB-H761LVE | `rtsp://IP:554/cam1/h264` | Fixed bullet | H.265 capable |
| VB-M40 | `rtsp://IP/profile1=u` | Compact PTZ | Unicast, no port specified |
| VB-M42 / VB-M44 | `rtsp://IP:554/cam1/h264` | Compact PTZ | Channel-based |
| VB-M600D | `rtsp://IP/profile1=r` | Compact dome | Read mode |
| VB-M620D / VB-M640V | `rtsp://IP:554/cam1/h264` | Compact dome | Current |
| VB-M741LE | `rtsp://IP:554/cam1/h264` | Compact PTZ | Outdoor |
| VB-S30D / VB-S31D | `rtsp://IP:554/cam1/h264` | Compact | Indoor |
| VB-S800D / VB-S900F | `rtsp://IP:554/cam1/h264` | Compact | Indoor |
| VB-R11 / VB-R11VE | `rtsp://IP:554/cam1/h264` | PTZ dome | Current |
| VB-R12VE | `rtsp://IP:554/cam1/h264` | PTZ dome | Outdoor |

### Legacy Models (VB-C Series -- HTTP Only)

Legacy VB-C cameras do not support RTSP. They use Canon's proprietary `-wvhttp-01-` HTTP URLs:

| Model | HTTP URL | Type | Notes |
|-------|----------|------|-------|
| VB-C300 | `http://IP/-wvhttp-01-/GetLiveImage` | PTZ dome | HTTP only |
| VB-C10 | `http://IP/-wvhttp-01-/GetLiveImage` | Compact | HTTP only |
| VB-C50i | `http://IP/-wvhttp-01-/GetLiveImage` | PTZ dome | HTTP only |
| VB-610 | `http://IP/-wvhttp-01-/video.cgi` | Fixed | HTTP only |

## Connecting with VisioForge SDK

Use your Canon camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Canon VB-H series camera, channel 1
var uri = new Uri("rtsp://192.168.1.70:554/cam1/h264");
var username = "root";
var password = "YourPassword";
```

For profile-based access on older VB models, use `rtsp://IP:554/profile1=r` or `rtsp://IP/profile1=u` depending on the model.

## Snapshot and MJPEG URLs

Canon uses a distinctive `-wvhttp-01-` path prefix for all HTTP-based image and video access:

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Live Image | `http://IP/-wvhttp-01-/GetLiveImage` | Current snapshot |
| MJPEG Stream | `http://IP/-wvhttp-01-/video.cgi` | Continuous MJPEG |
| Snapshot (sized) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT` | Custom resolution |
| Snapshot (continuous) | `http://IP/-wvhttp-01-/GetOneShot?image_size=WIDTHxHEIGHT&frame_count=0` | Continuous capture |

!!! info "Canon `-wvhttp-01-` prefix"
    The `-wvhttp-01-` path prefix is unique to Canon network cameras. All HTTP-based image and video URLs use this prefix. This distinctive path can help identify Canon cameras on a network.

## Troubleshooting

### RTSP must be enabled in the web interface

Canon cameras may not have RTSP enabled by default. Access the camera's web interface and navigate to the streaming settings to enable RTSP. Without this, the camera will only respond to HTTP requests.

### Legacy VB-C series are HTTP only

The VB-C series (VB-C300, VB-C10, VB-C50i) and VB-610 do not support RTSP at all. Use Canon's `-wvhttp-01-` HTTP URLs for video access from these models:

- `http://IP/-wvhttp-01-/GetLiveImage` for snapshots
- `http://IP/-wvhttp-01-/video.cgi` for MJPEG streaming

### Profile read vs unicast modes

Canon profile URLs use two access modes:

- `profile1=r` -- **Read mode**: Allows multicast distribution, suitable for multiple viewers
- `profile1=u` -- **Unicast mode**: Direct connection, one viewer per stream

If multicast is not configured on your network, use `profile1=u` for a direct unicast connection.

### Double slash in some URLs

Some Canon models (notably VB-H41) require a **double forward slash** before the stream path:

- VB-H41: `rtsp://IP:554//stream/profile1=r` (double slash)
- Most others: `rtsp://IP:554/cam1/h264` (single slash)

### Default credentials vary

Canon cameras do not have a universal default credential:

- **Current models:** Often `root` with a password set during initial setup
- **Older models:** May use `admin` / `admin` or `root` / `camera`
- Check the camera label or setup guide for model-specific defaults

## FAQ

**What is the default RTSP URL for Canon cameras?**

For current Canon VB-H and VB-M series cameras, use `rtsp://root:password@CAMERA_IP:554/cam1/h264`. For older models, try `rtsp://CAMERA_IP:554/profile1=r` or `rtsp://CAMERA_IP/profile1=u`.

**Do Canon cameras support H.265?**

Select Canon models support H.265, including the VB-H47 and VB-H761 series. Most other VB-series cameras use H.264. Legacy VB-C models support only MJPEG over HTTP.

**What is the `-wvhttp-01-` path in Canon URLs?**

The `-wvhttp-01-` prefix is Canon's proprietary HTTP path used for all web-based image and video access on their network cameras. It is used for snapshots (`GetOneShot`, `GetLiveImage`), MJPEG streaming (`video.cgi`), and camera control. This path is unique to Canon cameras.

**Can I connect to legacy Canon VB-C cameras?**

Legacy VB-C cameras (VB-C300, VB-C10, VB-C50i) are HTTP-only and do not support RTSP. You can access their video using the HTTP URL `http://CAMERA_IP/-wvhttp-01-/GetLiveImage` for snapshots or `http://CAMERA_IP/-wvhttp-01-/video.cgi` for MJPEG streaming.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Sony Connection Guide](sony.md) — Japanese enterprise cameras
- [Axis Connection Guide](axis.md) — Enterprise surveillance leader
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
