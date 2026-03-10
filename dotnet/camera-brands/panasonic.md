---
title: How to Connect to Panasonic (i-PRO) IP Camera in C# .NET
description: Panasonic i-PRO and legacy WV/BL/BB camera RTSP URL patterns for C# .NET. ONVIF-compatible integration with VisioForge SDK for all generations.
---

# How to Connect to Panasonic (i-PRO) IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Panasonic i-PRO** (formerly Panasonic Security Systems, now operating as i-PRO Co., Ltd.) is a Japanese manufacturer of professional video surveillance equipment. Originally part of Panasonic Corporation, the security division was spun off as **i-PRO** in 2019. Panasonic/i-PRO cameras are widely deployed in enterprise, government, transportation, and retail environments worldwide.

**Key facts:**

- **Product lines:** WV-S (current S-series), WV-X (X-series AI), WV-SF/SC/SP/SW (mid-generation), WV-NP/NS/NW (legacy professional), BL (consumer/SMB), BB/KX-HCM (legacy consumer)
- **Protocol support:** RTSP, ONVIF (Profile S/G/T), HTTP/CGI, MJPEG, Panasonic proprietary
- **Default RTSP port:** 554
- **Default credentials:** admin / 12345 (current models require password change on first login); legacy BB/BL models often had no default password
- **ONVIF support:** Yes (all current WV-S/WV-X models)
- **Video codecs:** H.264, H.265 (current models), MPEG-4 (legacy), MJPEG

## RTSP URL Patterns

### Current Models (WV-S/WV-X Series, i-PRO)

Current Panasonic i-PRO cameras use the `MediaInput` URL format:

| Stream | RTSP URL | Codec | Notes |
|--------|----------|-------|-------|
| H.264 stream | `rtsp://IP:554/MediaInput/h264` | H.264 | Primary RTSP stream |
| H.265 stream | `rtsp://IP:554/MediaInput/h265` | H.265 | Current models only |
| MPEG-4 stream | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | Legacy fallback |
| ONVIF stream | `rtsp://IP//ONVIF/MediaInput` | H.264 | ONVIF-compatible (note double slash) |

!!! warning "Double slash in ONVIF URLs"
    Panasonic ONVIF URLs use a double slash before `ONVIF`: `rtsp://IP//ONVIF/MediaInput`. This is intentional and required for ONVIF-based connections.

### Model-Specific URLs

| Model Series | RTSP URL | Generation |
|-------------|----------|------------|
| WV-S1131/S1132 | `rtsp://IP:554/MediaInput/h264` | Current (i-PRO) |
| WV-S2131L/S2231L | `rtsp://IP:554/MediaInput/h264` | Current (i-PRO) |
| WV-X1551L/X2251L | `rtsp://IP:554/MediaInput/h264` | Current AI series |
| WV-SF132/SF135/SF138 | `rtsp://IP:554/MediaInput/h264` | Mid-generation |
| WV-SF332/SF335/SF346 | `rtsp://IP:554/MediaInput/h264` | Mid-generation |
| WV-SC384/SC385/SC386 | `rtsp://IP:554/MediaInput/h264` | Mid-generation |
| WV-SP105/SP306/SP508 | `rtsp://IP:554/MediaInput/h264` | Mid-generation |
| WV-SW115/SW155/SW175 | `rtsp://IP:554/MediaInput/h264` | Mid-generation outdoor |
| WV-SW316/SW352/SW355 | `rtsp://IP:554/MediaInput/h264` | Mid-generation outdoor |
| WV-SW395/SW396/SW458 | `rtsp://IP:554/MediaInput/h264` | Mid-generation outdoor |
| WV-SW558/SW559/SW598 | `rtsp://IP:554/MediaInput/h264` | Mid-generation outdoor |
| WV-ST162/ST165 | `rtsp://IP:554/MediaInput/h264` | Mid-generation PTZ |
| WV-NP240/NP244/NP304 | `rtsp://IP:554/MediaInput/mpeg4` | Legacy professional |
| WV-NP502/NP1000/NP1004 | `rtsp://IP:554/MediaInput/mpeg4` | Legacy professional |
| WV-NS202/NS324/NS954 | `rtsp://IP:554/MediaInput/mpeg4` | Legacy PTZ |
| WV-NW484/NW502/NW960/NW964 | `rtsp://IP:554/MediaInput/mpeg4` | Legacy outdoor |

### Legacy Consumer Models (BB/BL/KX Series)

Older Panasonic consumer cameras used different URL patterns:

| Model Series | RTSP URL | Codec | Notes |
|-------------|----------|-------|-------|
| BL-C210/C230 | `rtsp://IP:554/MediaInput/h264` | H.264 | Late consumer models |
| BL-C210/C230 | `rtsp://IP:554/MediaInput/mpeg4` | MPEG-4 | MPEG-4 fallback |
| BL-VP101/VP104 | `rtsp://IP:554/MediaInput/h264` | H.264 | Compact |
| BB-HCM531A/735 | `rtsp://IP/nphMpeg4/g726-640x48` | MPEG-4 | Very old format |
| BB/BL/KX (HTTP only) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | MJPEG | HTTP MJPEG stream |

!!! info "Legacy BB/BL cameras"
    Many older Panasonic BB and BL series cameras do not support RTSP at all. They only provide HTTP-based MJPEG and JPEG snapshot streams. Current i-PRO cameras fully support RTSP.

### Encoder/DVR URLs

| Device | RTSP URL | Notes |
|--------|----------|-------|
| WJ-GXE500 encoder | `http://IP/cgi-bin/camera` | MJPEG via HTTP |
| WJ-HD220 DVR | `http://IP/cgi-bin/jpeg` | Snapshot from DVR |
| WJ-ND400 NVR | `http://IP/cgi-bin/jpeg` | Snapshot from NVR |
| WJ-NV200 NVR | `http://IP/cgi-bin/checkimage.cgi?UID=USER&CAM=CHANNEL` | Channel snapshot |

## Connecting with VisioForge SDK

Use your Panasonic camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Panasonic i-PRO camera, H.264 stream
var uri = new Uri("rtsp://192.168.1.80:554/MediaInput/h264");
var username = "admin";
var password = "YourPassword";
```

For H.265, use `/MediaInput/h265` instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| MJPEG Stream (current) | `http://IP/cgi-bin/mjpeg?stream=1` | Current WV-S/WV-X models |
| JPEG Snapshot | `http://IP/cgi-bin/camera` | Current models |
| Snapshot (sized) | `http://IP/SnapshotJPEG?Resolution=640x480` | Mid-generation models |
| Snapshot (quality) | `http://IP/SnapShotJPEG?Resolution=320x240&Quality=Motion` | Legacy models |
| MJPEG Stream (legacy) | `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` | BB/BL/KX models |
| Server push | `http://IP/cgi-bin/nphContinuousServerPush` | Continuous JPEG push |

## Troubleshooting

### Brand name confusion

The Panasonic security camera brand has evolved:

- **Panasonic** (before 2019): Full Panasonic branding
- **i-PRO** (2019-present): Spun off from Panasonic as i-PRO Co., Ltd.
- Current products are branded **i-PRO** but many users still search for "Panasonic camera"

All use compatible RTSP URL patterns within their generation.

### MediaInput/h264 vs ONVIF/MediaInput

- Use `rtsp://IP:554/MediaInput/h264` for direct RTSP connections (recommended)
- Use `rtsp://IP//ONVIF/MediaInput` for ONVIF-compatible connections (note the double slash)
- Both provide the same video stream but use different authentication mechanisms

### Legacy cameras without RTSP

Many older Panasonic BB-series and BL-series cameras (particularly BL-C1, BL-C10, BL-C30, BL-C101, BL-C111, BL-C131 and earlier) do not support RTSP. These cameras only provide:

- HTTP MJPEG: `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard`
- HTTP Snapshot: `http://IP/SnapshotJPEG?Resolution=320x240`

### MPEG-4 vs H.264

Legacy WV-NP/NS/NW series cameras may only support MPEG-4 over RTSP. Try `MediaInput/mpeg4` if `MediaInput/h264` fails on older models.

## FAQ

**What is the default RTSP URL for Panasonic/i-PRO cameras?**

For current i-PRO cameras, use `rtsp://admin:password@CAMERA_IP:554/MediaInput/h264`. For ONVIF connections, use `rtsp://CAMERA_IP//ONVIF/MediaInput`. Legacy models may need `MediaInput/mpeg4`.

**Is Panasonic the same as i-PRO?**

Yes. Panasonic's security camera division was spun off as i-PRO Co., Ltd. in 2019. Current cameras are branded i-PRO, but use the same RTSP URL patterns as late-generation Panasonic WV-series cameras.

**Do Panasonic cameras support H.265?**

Current i-PRO cameras (WV-S and WV-X series) support H.265. Use `rtsp://IP:554/MediaInput/h265` for the H.265 stream. Mid-generation and older models support H.264 and MPEG-4 only.

**Can I connect to legacy Panasonic BB/BL cameras?**

Many older BB and BL series cameras don't support RTSP and only provide HTTP MJPEG streams. Use the HTTP MJPEG URL `http://IP/nphMotionJpeg?Resolution=640x480&Quality=Standard` with an HTTP source instead of RTSP.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Sanyo Connection Guide](sanyo.md) — Acquired by Panasonic, predecessor product line
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
