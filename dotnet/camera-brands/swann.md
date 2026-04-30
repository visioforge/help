---
title: Swann IP Camera RTSP URL and C# .NET Connection Guide
description: Swann NHD, SWNHD, DVR/NVR, and ADS camera RTSP URL patterns for C# .NET. Stream and record using VisioForge Video Capture SDK integration code.
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

# How to Connect to Swann IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Swann** (Swann Communications) is an Australian consumer security brand headquartered in Melbourne, Australia, now owned by **Infinova**. Swann is one of the best-known consumer and prosumer security brands, popular for their bundled DVR/NVR camera systems sold through major retailers. Swann offers a range of standalone IP cameras, analog-over-coax (BNC) camera systems, and network video recorders.

**Key facts:**

- **Product lines:** NHD (current network HD cameras), SWNHD (HD IP cameras), SWPRO (analog-over-coax), DVR/NVR systems, ADS (legacy IP cameras)
- **Protocol support:** RTSP, ONVIF (current NHD models), HTTP/MJPEG (legacy)
- **Default RTSP port:** 554
- **Default credentials:** admin / admin or admin / (empty) on older models
- **ONVIF support:** Yes (current NHD-series cameras)
- **Video codecs:** H.264, H.265 (current models), MPEG-4 (legacy DVRs)
- **OEM base:** Many newer Swann NVRs are Hikvision OEM and use Hikvision RTSP URL patterns

!!! info "Swann NVRs and Hikvision"
    Many current Swann NVRs are manufactured by Hikvision and use Hikvision firmware. If the standard Swann RTSP URL does not work on your NVR, try the Hikvision URL format (`/Streaming/Channels/`). See our [Hikvision connection guide](hikvision.md) for details.

## RTSP URL Patterns

### Current NHD-Series IP Cameras

Standalone Swann NHD-series IP cameras (SWNHD-820CAM, SWNHD-830CAM, NHD-866, etc.) use the following URL:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/live/h264
```

### NVR Systems (Hikvision-Based)

Most current Swann NVRs use Hikvision-style RTSP paths:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554//Streaming/Channels/[CHANNEL_ID]
```

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554//Streaming/Channels/1` | `rtsp://IP:554//Streaming/Channels/102` |
| Camera 2 | `rtsp://IP:554//Streaming/Channels/2` | `rtsp://IP:554//Streaming/Channels/202` |
| Camera N | `rtsp://IP:554//Streaming/Channels/N` | `rtsp://IP:554//Streaming/Channels/N02` |

!!! note "Channel numbering"
    For Hikvision-based NVRs, the main stream channel ID matches the camera number (1, 2, 3...). The sub stream uses the format `N02` where N is the camera number (102, 202, 302...).

### Legacy DVR Models

Older Swann DVR systems (DVR4-PRO-NET, etc.) and standalone cameras use MPEG-4:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/mpeg4
```

### URL Summary Table

| Model / Series | Main Stream URL | Notes |
|----------------|----------------|-------|
| NHD-series cameras (SWNHD-820/830) | `rtsp://IP:554/live/h264` | Standalone IP cameras |
| IP-3G ConnectCam | `rtsp://IP:554/mpeg4` | Legacy standalone |
| Max-IP-Cam | `rtsp://IP:554/mpeg4` | Legacy standalone |
| Current NVR (channel 1) | `rtsp://IP:554//Streaming/Channels/1` | Hikvision OEM |
| Current NVR (channel 1, sub) | `rtsp://IP:554//Streaming/Channels/102` | Hikvision OEM |
| DVR4-PRO-NET | `rtsp://IP:554/mpeg4` | Legacy DVR |
| Generic Swann IP cameras | `rtsp://IP:554/live/h264` | Try this first |

## Connecting with VisioForge SDK

Use your Swann camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Swann NHD-series camera, main stream
var uri = new Uri("rtsp://192.168.1.90:554/live/h264");
var username = "admin";
var password = "YourPassword";
```

For NVR sub-stream access, use `/Streaming/Channels/102` instead of `/Streaming/Channels/1`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| HTTP stream (ADS-440 legacy) | `http://IP/videostream.asf?user=USER&pwd=PASS` | ASF format, no RTSP |
| MJPEG stream (legacy) | `http://IP/videostream.cgi?user=USER&pwd=PASS` | Older models |
| ONVIF snapshot | `http://IP/onvif-http/snapshot` | NHD-series with ONVIF |

!!! warning "Legacy HTTP-only cameras"
    The ADS-440 series and some other older Swann models only support HTTP streaming (ASF or MJPEG) and do not support RTSP at all. Use the HTTP URL directly for these cameras.

## Troubleshooting

### Identify your NVR firmware type

Many Swann NVRs are Hikvision OEM. To determine which URL format to use:

1. Access the NVR web interface at `http://NVR_IP`
2. Check the login page — Hikvision-based NVRs often show a Hikvision-style interface
3. Try the Hikvision URL first (`/Streaming/Channels/1`), then fall back to Swann URLs (`/live/h264` or `/mpeg4`)

### "Connection refused" on legacy cameras

Older Swann cameras (ADS-440 series, early DVR models) may not support RTSP at all. These cameras use HTTP-based streaming only. Try the HTTP ASF or MJPEG URL instead of RTSP.

### Default credentials not working

- Current models typically ship with admin / admin but require password change on first setup
- Some older models use admin with an empty password
- Always complete initial setup via the Swann web interface or SwannView app before attempting RTSP access

### SwannView vs local RTSP access

SwannView (Swann's cloud service) is separate from local RTSP access. You do not need a SwannView account to use RTSP streaming on your local network. RTSP works purely over the local network connection.

## FAQ

**What is the default RTSP URL for Swann cameras?**

For current NHD-series cameras, use `rtsp://admin:password@CAMERA_IP:554/live/h264`. For Swann NVRs (Hikvision-based), use `rtsp://admin:password@NVR_IP:554//Streaming/Channels/1` for channel 1 main stream.

**Are Swann NVRs compatible with Hikvision RTSP URLs?**

Yes. Many current Swann NVRs are manufactured by Hikvision and use identical firmware. The Hikvision RTSP URL format (`/Streaming/Channels/`) works on these systems. If the standard Swann URL fails, try the Hikvision format.

**Do all Swann cameras support RTSP?**

No. Some legacy Swann models (particularly the ADS-440 series) only support HTTP-based streaming in ASF or MJPEG format. All current NHD-series cameras and NVRs support RTSP.

**Do Swann cameras support ONVIF?**

Yes, current NHD-series cameras support ONVIF. Legacy models (SWPRO, ADS series) generally do not support ONVIF.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Lorex Connection Guide](lorex.md) — Consumer/prosumer segment peer
- [Hikvision Connection Guide](hikvision.md) — Swann NVRs with Hikvision firmware
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
