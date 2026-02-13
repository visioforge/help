---
title: How to Connect to Bosch IP Camera in C# .NET
description: Connect to Bosch Security cameras in C# .NET with RTSP URL patterns and code samples for Dinion, Flexidome, Autodome, and VIP encoder models.
---

# How to Connect to Bosch IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Bosch Security and Safety Systems** (a division of Robert Bosch GmbH) is a German manufacturer of professional and enterprise video surveillance equipment. Headquartered in Grasbrunn near Munich, Bosch produces IP cameras, encoders, recording solutions, and video analytics primarily for critical infrastructure, transportation, and enterprise security markets.

**Key facts:**

- **Product lines:** Dinion (bullet/box), Flexidome (dome), Autodome (PTZ), MIC (ruggedized), NBN/NDN/NTC (legacy network), NWC (compact), VideoJet/VIP (encoders)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Bosch VMS (BVMS), iSCSI direct recording
- **Default RTSP port:** 554
- **Default credentials:** Varies by model and firmware version; many require setup via Bosch Configuration Manager
- **ONVIF support:** Yes (all current IP cameras)
- **Video codecs:** H.264, H.265, MJPEG
- **Unique feature:** RTSP tunnel mode for firewall traversal

## RTSP URL Patterns

Bosch cameras use several URL patterns depending on the model generation. The most common are the `/rtsp_tunnel` and `/video` paths.

### Current Models (Bosch CPP series firmware)

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Video stream 1 | `rtsp://IP:554/video?inst=1` | Main stream |
| Video stream 2 | `rtsp://IP:554/video?inst=2` | Sub stream |
| RTSP tunnel | `rtsp://IP:554//rtsp_tunnel` | Firewall-friendly (note double slash) |
| H.264 direct | `rtsp://IP:554/h264` | Direct H.264 stream |

!!! info "RTSP tunnel mode"
    The `//rtsp_tunnel` URL (with double slash) is Bosch's proprietary RTSP tunneling mode that works better through firewalls and NAT. It encapsulates RTP data within the RTSP TCP connection. Use the standard `/video` URL for most integrations.

### Model-Specific URLs

| Model Series | RTSP URL | Codec | Notes |
|-------------|----------|-------|-------|
| Dinion IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Current |
| Flexidome IP 4000/5000/7000/8000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Current |
| Autodome IP 4000/5000/7000 | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Current PTZ |
| MIC IP fusion/starlight/ultra | `rtsp://IP:554/video?inst=1` | H.264/H.265 | Ruggedized |
| NDC-225-PI | `rtsp://IP:554//rtsp_tunnel` | H.264 | Legacy |
| NDC-255-P | `rtsp://IP:554//rtsp_tunnel` | H.264 | Legacy |
| NDC-265-P | `rtsp://IP:554/h264` | H.264 | Legacy |
| NDN-832v | `rtsp://IP:554//rtsp_tunnel` | H.264 | Legacy dome |
| NTC-255-PI | `rtsp://IP:554/video` | H.264 | Legacy thermal |
| NTC-265-PI | `rtsp://IP:554/h264` | H.264 | Legacy thermal |
| NTI-50022-V3 | `rtsp://IP:554/h264` | H.264 | IP bullet |
| NWC-0455-20P | `rtsp://IP:554/h264` | H.264 | Compact |

### Encoder URLs

Bosch video encoders (VideoJet, VIP series) allow connecting analog cameras to IP networks:

| Encoder | RTSP URL | Notes |
|---------|----------|-------|
| VideoJet 10 | `rtsp://IP:554/video?inst=1` | Single channel |
| VIP X1 | `rtsp://IP:554//rtsp_tunnel` | Single channel |
| VIP X1600 | `rtsp://IP:554/video?inst=1` | Multi-channel |
| VIP X2 | `rtsp://IP:554/video?inst=1` | Dual channel |

### DVR RTSP URLs

| DVR Model | RTSP URL | Notes |
|-----------|----------|-------|
| DVR 440/480/600 | `rtsp://IP:554/rtsp_tunnel` | Single slash |
| DVR 440/480/600 | `rtsp://IP:554/video` | Alternative |
| DVR (channel) | `rtsp://IP:554/cgi-bin/rtspStream/CHANNEL` | Channel-specific |
| DVR (SDP) | `rtsp://IP:554/user=USER&password=PASS&channel=1&stream=0.sdp?` | SDP-based |

## Connecting with VisioForge SDK

Use your Bosch camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Bosch Dinion/Flexidome, main stream
var uri = new Uri("rtsp://192.168.1.60:554/video?inst=1");
var username = "service";
var password = "YourPassword";
```

For sub-stream access, use `?inst=2` instead of `?inst=1`. For legacy Bosch models, use the RTSP tunnel URL `//rtsp_tunnel` (note the double slash).

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snap.jpg` | Basic snapshot |
| Snapshot (sized) | `http://IP/snap.jpg?JpegSize=XL` | XL, M sizes available |
| Snapshot (channel) | `http://IP/snap.jpg?JpegCam=CHANNEL` | Multi-channel encoders |
| Snapshot (auth) | `http://IP/snap.jpg?usr=USER&pwd=PASS` | URL-based auth |
| MJPEG Stream | `http://IP/img/mjpeg.jpg` | Continuous MJPEG |
| Image | `http://IP/img.jpg` | Single frame |
| Image (alt) | `http://IP/image.jpg` | Alternative path |

## Troubleshooting

### Double slash in rtsp_tunnel URL

The `//rtsp_tunnel` URL (with double slash before `rtsp_tunnel`) is intentional for legacy Bosch cameras. This is not a typo:

- Correct: `rtsp://IP:554//rtsp_tunnel`
- Incorrect: `rtsp://IP:554/rtsp_tunnel` (may work on some models but not all)

### Bosch Configuration Manager required

Many Bosch cameras require initial setup through the **Bosch Configuration Manager** desktop application before RTSP access works. The camera may not respond to RTSP connections until initial configuration is complete.

### Default username varies

- **Current models:** `service` user with password set during setup
- **Legacy models:** May use `admin`, `user`, or `service` depending on firmware
- Check the Bosch Configuration Manager or camera's web interface for user settings

### inst parameter

The `?inst=1` parameter selects the video stream instance:
- `inst=1` = First video stream (main)
- `inst=2` = Second video stream (sub)
- Not all models support multiple instances

### Encoder channel selection

For multi-channel encoders (VIP X1600, VideoJet X-series), use the `inst` parameter to select the channel:
- `rtsp://IP:554/video?inst=1` = Channel 1
- `rtsp://IP:554/video?inst=2` = Channel 2

## FAQ

**What is the default RTSP URL for Bosch cameras?**

For current Bosch cameras, the URL is `rtsp://service:password@CAMERA_IP:554/video?inst=1`. For legacy models, try `rtsp://CAMERA_IP:554//rtsp_tunnel` or `rtsp://CAMERA_IP:554/h264`.

**What is Bosch RTSP tunnel mode?**

RTSP tunnel (`//rtsp_tunnel`) is Bosch's proprietary mode that encapsulates RTP data within the RTSP TCP connection, making it easier to traverse firewalls. It's the default streaming mode on many legacy Bosch cameras.

**Do Bosch cameras support H.265?**

Current Bosch IP cameras (CPP13/CPP14 platform, including Dinion/Flexidome 7000/8000 series) support H.265. Legacy cameras support H.264 and MPEG-4. Check your specific model's datasheet for codec support.

**Can I use Bosch encoders to connect analog cameras?**

Yes. Bosch VideoJet and VIP encoders convert analog camera signals to IP streams accessible via RTSP. Use the same URL format (`/video?inst=1` or `//rtsp_tunnel`) as IP cameras.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Axis Connection Guide](axis.md) — Enterprise surveillance peer
- [Honeywell Connection Guide](honeywell.md) — Enterprise / commercial segment
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
