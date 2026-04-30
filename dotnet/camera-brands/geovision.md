---
title: GeoVision IP Camera RTSP URL Setup and C# .NET Guide
description: GeoVision GV-BL, GV-FD, GV-VD, GV-FE, and GV-DVR RTSP URL patterns for C# .NET. Integrate with VisioForge SDK for multi-channel surveillance apps.
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

# How to Connect to GeoVision IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**GeoVision** (GeoVision Inc.) is a Taiwanese manufacturer of IP cameras, network video recorders, and video management software, headquartered in Taipei, Taiwan. GeoVision is a well-established brand in the enterprise and professional surveillance market, known for their GV-series IP cameras and the GeoVision VMS platform.

**Key facts:**

- **Product lines:** GV-BL (bullet), GV-FD (fixed dome), GV-VD (vandal dome), GV-FE (fisheye), GV-CB (cube), GV-CA (camera), GV-DVR (digital video recorder), GV-NVR
- **Protocol support:** RTSP, ONVIF, PSIA, HTTP/CGI
- **Default RTSP port:** 8554 (IP cameras), 554 (DVR/Server)
- **Default credentials:** admin / admin
- **ONVIF support:** Yes (current models)
- **Video codecs:** H.264, H.265 (current models), MPEG-4 (legacy)

!!! warning "Non-standard RTSP port"
    GeoVision IP cameras use **port 8554** by default, not the standard 554. Make sure to specify the correct port when constructing your RTSP URL. GeoVision DVR/Server software uses the standard port 554.

## RTSP URL Patterns

### IP Camera Standard Format

GeoVision IP cameras use a channel-based SDP URL pattern on port 8554:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:8554//CH001.sdp
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| Port | 8554 | Default for GeoVision IP cameras |
| `CH001` | CH001, CH002... | Channel number (zero-padded 3 digits) |
| `.sdp` | Required | SDP session descriptor suffix |

!!! note "Double slash"
    Some GeoVision models require a double slash (`//`) before the channel identifier. If a single slash does not work, try `//CH001.sdp`.

### IP Camera Streams

| Stream | URL | Notes |
|--------|-----|-------|
| Main stream | `rtsp://IP:8554//CH001.sdp` | Full resolution, port 8554 |
| Sub stream | `rtsp://IP:8554//CH002.sdp` | Lower resolution |

### DVR / GeoVision Server

GeoVision DVR and GV-Server software use port 554:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/CH001.sdp
```

| Channel | Main Stream URL | Notes |
|---------|----------------|-------|
| Channel 1 | `rtsp://IP:554/CH001.sdp` | Port 554 on DVR/Server |
| Channel 2 | `rtsp://IP:554/CH002.sdp` | Port 554 on DVR/Server |
| Channel N | `rtsp://IP:554/CH00N.sdp` | Zero-pad to 3 digits |

### PSIA Streaming

GeoVision also supports PSIA-compatible RTSP URLs:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/PSIA/Streaming/channels/1?videoCodecType=MPEG4
```

### URL Summary Table

| Device Type | Main Stream URL | Default Port | Notes |
|-------------|----------------|--------------|-------|
| GV-BL (bullet) | `rtsp://IP:8554//CH001.sdp` | 8554 | Standard IP camera |
| GV-FD (fixed dome) | `rtsp://IP:8554//CH001.sdp` | 8554 | Standard IP camera |
| GV-VD (vandal dome) | `rtsp://IP:8554//CH001.sdp` | 8554 | Standard IP camera |
| GV-FE (fisheye) | `rtsp://IP:8554//CH001.sdp` | 8554 | Standard IP camera |
| GV-CB (cube) | `rtsp://IP:8554//CH001.sdp` | 8554 | Standard IP camera |
| GV-DVR | `rtsp://IP:554/CH001.sdp` | 554 | DVR software |
| GV-NVR | `rtsp://IP:554/CH001.sdp` | 554 | NVR software |
| PSIA stream | `rtsp://IP:554/PSIA/Streaming/channels/1` | 554 | PSIA compatible |

## Connecting with VisioForge SDK

Use your GeoVision camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// GeoVision GV-BL2702, main stream (note port 8554)
var uri = new Uri("rtsp://192.168.1.90:8554//CH001.sdp");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `CH002.sdp` instead of `CH001.sdp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Requires basic auth |
| JPEG Snapshot (alt) | `http://IP/GetImage.cgi` | Some models |
| JPEG Snapshot (alt) | `http://IP/cgi-bin/getimage` | Some models |
| JPEG Snapshot (viewer) | `http://IP/cgi-bin/viewer/video.jpg` | Web viewer interface |
| Static image (alt) | `http://IP/cgi-bin/jpg/image.cgi` | Some models |
| Legacy snapshot | `http://IP/cam1.jpg` | Firmware 6.0-8.x, channel 1 |
| Legacy snapshot (ch N) | `http://IP/camN.jpg` | Firmware 6.0-8.x, channel N |

## Troubleshooting

### Wrong port — 554 vs 8554

The most common connection issue with GeoVision cameras is using the wrong port:

- **IP cameras** (GV-BL, GV-FD, GV-VD, GV-FE, GV-CB): Use **port 8554**
- **DVR / GV-Server software**: Use **port 554**

If you get a connection timeout, check that you are using the correct port for your device type.

### Double slash in URL path

Some GeoVision IP camera models require a double slash before the channel identifier (`//CH001.sdp`). If a single slash (`/CH001.sdp`) returns an error, add the extra slash.

### Channel numbering format

GeoVision uses zero-padded three-digit channel numbers: `CH001`, `CH002`, `CH003`, etc. Using `CH1` instead of `CH001` will not work.

### Firmware version differences

Older GeoVision firmware versions (6.x-8.x) may use different snapshot URL formats. If the CGI-based snapshot URL does not work, try the legacy format (`http://IP/cam1.jpg`).

## FAQ

**What port does GeoVision use for RTSP?**

GeoVision IP cameras use **port 8554** by default, which differs from the industry-standard port 554. GeoVision DVR and GV-Server software use the standard port 554.

**What is the default RTSP URL for GeoVision IP cameras?**

The URL is `rtsp://admin:password@CAMERA_IP:8554//CH001.sdp` for the main stream. Use `CH002.sdp` for the sub stream. Note the double slash before `CH001` and port 8554.

**Do GeoVision cameras support ONVIF?**

Yes. All current GeoVision IP camera models support ONVIF Profile S. ONVIF discovery can be used as an alternative to manual RTSP URL configuration.

**Can I connect to a GeoVision DVR and IP camera at the same time?**

Yes. Connect to the DVR on port 554 and individual IP cameras on port 8554. Each device has its own IP address and RTSP endpoint.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Vivotek Connection Guide](vivotek.md) — Taiwanese enterprise cameras
- [ACTi Connection Guide](acti.md) — Taiwanese professional cameras
- [RTSP Camera Integration Guide](../videocapture/video-sources/ip-cameras/rtsp.md) — GeoVision RTSP stream setup
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
