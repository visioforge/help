---
title: How to Connect to Samsung (Hanwha) IP Camera in C# .NET
description: Connect to Samsung Wisenet and Hanwha Vision cameras in C# .NET with RTSP URL patterns and code samples for SNO, SND, XNO, XND, and PNO models.
---

# How to Connect to Samsung (Hanwha) IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Hanwha Vision** (formerly Samsung Techwin, then Hanwha Techwin) is a South Korean manufacturer of professional and enterprise-grade video surveillance equipment. The Samsung security camera brand was rebranded to **Wisenet** after Hanwha Group acquired Samsung Techwin in 2015. Hanwha Vision cameras are widely deployed in enterprise, government, and critical infrastructure installations worldwide.

**Key facts:**

- **Product lines:** XNO/XND/XNV (X-series, current flagship), PNO/PND/PNV (P-series, mainstream), QNO/QND/QNV (Q-series, value), SNO/SND/SNV/SNB (S-series, legacy Samsung)
- **Naming convention:** First letter = series, N = network, O = outdoor, D = dome, V = vandal-resistant, B = box, P = PTZ
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, Wisenet WAVE VMS
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during initial setup); legacy Samsung models: admin / 4321
- **ONVIF support:** Yes (all current and most legacy models)
- **Video codecs:** H.264, H.265, MJPEG

## RTSP URL Patterns

### Current Models (Wisenet X/P/Q Series)

Current Hanwha Vision cameras use a profile-based URL format:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Profile 1 (main) | `rtsp://IP:554/profile2/media.smp` | Main stream, H.264/H.265 |
| Profile 2 (sub) | `rtsp://IP:554/profile3/media.smp` | Sub stream |
| ONVIF Profile 1 | `rtsp://IP:554//onvif/profile1/media.smp` | ONVIF-compatible (note double slash) |
| ONVIF Profile 2 | `rtsp://IP:554//onvif/profile2/media.smp` | ONVIF sub stream |

!!! warning "Double slash in ONVIF URLs"
    Samsung/Hanwha ONVIF URLs use a double slash (`//onvif/`). This is intentional and required. Using a single slash will fail.

### Legacy Samsung S-Series

| Model Series | RTSP URL | Codec |
|-------------|----------|-------|
| SNB-xxxx (box) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SND-xxxx (dome) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNO-xxxx (outdoor) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNV-xxxx (vandal) | `rtsp://IP:554/profile2/media.smp` | H.264 |
| SNP-xxxx (PTZ) | `rtsp://IP:554/profile2/media.smp` | H.264 |

### Older Samsung Models (Pre-Wisenet)

Older Samsung cameras used different URL formats:

| URL Pattern | Models | Codec |
|-------------|--------|-------|
| `rtsp://IP:554/mpeg4unicast` | SNB-2000, SNC-1300, SNP-3301/3370 | MPEG-4 |
| `rtsp://IP:554/h264unicast` | SNP-3301/H, SNP-3370/TH | H.264 |
| `rtsp://IP:554/mjpegunicast` | SNP-3301/H, SNP-3370/TH | MJPEG |
| `rtsp://IP:554/H264/media.smp` | SNB-3000, SND-3080, SNV-3080/3081 | H.264 |
| `rtsp://IP:554/MPEG4/media.smp` | SNV-3080/3081 | MPEG-4 |
| `rtsp://IP:554/MJPEG/media.smp` | SNB-3000, SNV-3081, SNV-6084R | MJPEG |
| `rtsp://IP:554/MediaInput/h264` | Misc. Samsung | H.264 |

### DVR/NVR URLs

| Device | RTSP URL | Notes |
|--------|----------|-------|
| SRD-165 DVR | `rtsp://IP:558/` | Non-standard port 558 |
| SME DVR | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |
| SMT DVR | `rtsp://IP:554/mpeg4unicast` | MPEG-4 |

## Connecting with VisioForge SDK

Use your Samsung (Hanwha Wisenet) camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Hanwha Wisenet X-series camera, main stream
var uri = new Uri("rtsp://192.168.1.70:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `profile3/media.smp` instead of `profile2/media.smp`. For legacy Samsung S-series models, use the default password `4321` and URL path `/mpeg4unicast` or `/H264/media.smp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/video.cgi?msubmenu=jpg` | Current models |
| MJPEG Stream | `http://IP/cgi-bin/video.cgi?msubmenu=mjpg` | Current models |
| Legacy Snapshot | `http://IP/video?submenu=jpg` | Pre-Wisenet models |
| Legacy MJPEG | `http://IP/video?submenu=mjpg` | Pre-Wisenet models |
| CGI Snapshot | `http://IP/cgi-bin/webra_fcgi.fcgi?api=get_jpeg_raw&chno=CHANNEL` | DVR models |
| Snap (sized) | `http://IP/snap.jpg?JpegSize=XL` | Some Bosch-OEM firmware |

## Troubleshooting

### Default password differences

- **Current Hanwha Vision models:** Password must be set during initial setup via web browser
- **Legacy Samsung S-series:** Default password is `4321`
- **Very old Samsung models:** Some used `admin` / `admin`

### profile2 vs profile1 in URL

Samsung/Hanwha cameras use `profile2/media.smp` for the main stream (not `profile1`). This is a common source of confusion:

- `profile2/media.smp` = Main stream (typically H.264 at full resolution)
- `profile3/media.smp` = Sub stream
- The profile numbers may differ based on camera configuration

### ONVIF double-slash issue

The ONVIF URL format requires a double slash before `onvif`:
- Correct: `rtsp://IP:554//onvif/profile1/media.smp`
- Incorrect: `rtsp://IP:554/onvif/profile1/media.smp`

### Brand name confusion

Samsung Techwin was acquired by Hanwha in 2015. The brand has been called:
- **Samsung Techwin** (before 2015)
- **Hanwha Techwin** (2015-2022)
- **Hanwha Vision** (2022-present)
- **Wisenet** (product brand name, used throughout)

All use the same RTSP URL patterns within their respective generation.

## FAQ

**What is the default RTSP URL for Samsung/Hanwha cameras?**

For current Wisenet models, the URL is `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp`. For legacy Samsung models, try `rtsp://admin:4321@CAMERA_IP:554/mpeg4unicast` or `rtsp://CAMERA_IP:554/H264/media.smp`.

**Is Samsung the same as Hanwha Vision?**

Yes. Samsung's security camera division was acquired by Hanwha Group in 2015. The product brand is **Wisenet**. Legacy Samsung cameras (SNB, SND, SNO series) and current Hanwha Vision cameras (XNO, XND, PNO series) use similar RTSP patterns.

**Do Samsung/Hanwha cameras support H.265?**

Yes. Current X-series and P-series cameras support H.265 (HEVC). Legacy S-series cameras typically support H.264 and MPEG-4 only.

**What VMS works with Hanwha cameras?**

Hanwha's own VMS is **Wisenet WAVE**. However, all Hanwha cameras support standard RTSP and ONVIF, making them compatible with any third-party software including VisioForge SDK applications.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hanwha Vision Connection Guide](hanwha.md) — Current brand name, same URLs
- [Wisenet Connection Guide](wisenet.md) — Hanwha Vision product family
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
