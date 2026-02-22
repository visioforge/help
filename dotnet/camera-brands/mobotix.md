---
title: Mobotix RTSP URL Patterns for IP Camera Access in C#
description: Connect to MOBOTIX cameras in C# .NET with RTSP URL patterns for classic Mx and MOVE series. Includes MxPEG, MJPEG, and H.264 stream options.
---

# How to Connect to Mobotix IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**MOBOTIX** (MOBOTIX AG) is a German IP camera manufacturer headquartered in Langmeil, Germany, founded in 1999. MOBOTIX pioneered the concept of decentralized IP video systems where intelligent processing, recording, and analytics happen directly inside the camera rather than on a central server. The company was acquired by **Konica Minolta** in 2016. MOBOTIX cameras are known for their rugged construction, long operational lifespan, and suitability for industrial, outdoor, and critical infrastructure environments.

**Key facts:**

- **Product lines:** M-series (outdoor), D-series (dome), S-series (hemispheric), Q-series (panoramic), T-series (door station), MOVE (newer ONVIF line)
- **Protocol support:** RTSP, HTTP/CGI, MxPEG (proprietary); ONVIF (MOVE series only)
- **Default RTSP port:** 554
- **Default credentials:** admin / meinsm
- **ONVIF support:** MOVE series only (classic Mx cameras do not support ONVIF)
- **Video codecs:** MxPEG (proprietary), MJPEG, H.264 (newer models)
- **Architecture:** Decentralized, in-camera recording and processing

!!! warning "Classic vs MOVE Series"
    Classic Mobotix cameras (M/D/S/Q series) primarily use the proprietary MxPEG codec and do not support ONVIF. For ONVIF and standard H.264/H.265 RTSP, use the newer MOBOTIX MOVE series.

!!! note "About MxPEG"
    MxPEG is a proprietary video codec developed by MOBOTIX for efficient bandwidth usage with their decentralized architecture. If your application cannot decode MxPEG natively, use the MJPEG fallback stream via HTTP (`/cgi-bin/faststream.jpg`) or configure the camera to output standard MJPEG or H.264 where supported. VisioForge SDK can connect to MOBOTIX cameras using the MJPEG HTTP stream or the H.264 RTSP stream on supported models.

## RTSP URL Patterns

### Standard URL Format

MOBOTIX cameras use branded path-based RTSP URLs:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/mobotix.h264
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| H.264 main stream | `rtsp://IP:554/mobotix.h264` | Primary H.264 stream (newer models) |
| MJPEG stream | `rtsp://IP:554/mobotix.mjpeg` | MJPEG over RTSP |

### Camera Series and URLs

| Series | Type | Recommended URL | Codec |
|--------|------|----------------|-------|
| MOVE Bullet | IP bullet | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Dome | IP dome | `rtsp://IP:554/mobotix.h264` | H.264 |
| MOVE Vandal Dome | IP vandal-proof | `rtsp://IP:554/mobotix.h264` | H.264 |
| M-series (M73, M16) | Outdoor | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| D-series (D16, D26) | Dome | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| S-series (S16, S26) | Hemispheric | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| Q-series (Q26) | Panoramic 360 | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |
| T-series (T26) | Door station | `rtsp://IP:554/mobotix.mjpeg` | MJPEG |

### MOVE Series ONVIF URLs

MOBOTIX MOVE cameras support standard ONVIF and provide conventional RTSP streams:

| Stream | URL Pattern | Notes |
|--------|-------------|-------|
| Main stream | `rtsp://IP:554/mobotix.h264` | H.264 primary stream |
| Sub stream | `rtsp://IP:554/mobotix.mjpeg` | MJPEG secondary stream |

## Connecting with VisioForge SDK

Use your MOBOTIX camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// MOBOTIX MOVE or classic Mx camera, H.264 stream
var uri = new Uri("rtsp://192.168.1.90:554/mobotix.h264");
var username = "admin";
var password = "meinsm";
```

For classic Mx cameras that only support MxPEG, use the MJPEG HTTP stream URL instead (see below).

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| MJPEG Full Resolution | `http://IP/cgi-bin/faststream.jpg?stream=full` | Full resolution continuous MJPEG |
| MxPEG Stream | `http://IP/cgi-bin/faststream.jpg?stream=MxPEG&needlength&fps=6` | Proprietary MxPEG at 6 fps |
| Controlled FPS MJPEG | `http://IP/control/faststream.jpg?stream=full&fps=10` | MJPEG capped at 10 fps |
| Current Snapshot | `http://IP/record/current.jpg` | Single JPEG snapshot |

## Troubleshooting

### Classic Mx camera not connecting via RTSP

Classic MOBOTIX cameras (M, D, S, Q, T series) primarily use the proprietary MxPEG codec. If the RTSP stream fails:

1. Try the MJPEG RTSP URL: `rtsp://IP:554/mobotix.mjpeg`
2. If RTSP is not available, use the HTTP MJPEG stream: `http://IP/cgi-bin/faststream.jpg?stream=full`
3. Check that RTSP is enabled in the camera's web interface under **Admin Menu > Network Setup > RTSP Server**

### "401 Unauthorized" error

MOBOTIX cameras use the default credentials `admin` / `meinsm`. If authentication fails:

1. Access the camera web interface at `http://CAMERA_IP`
2. Log in with the default or configured credentials
3. Verify the user account has streaming access permissions
4. Use the correct credentials in your RTSP URL

### MxPEG stream not decoding

MxPEG is a proprietary codec that standard media players and libraries may not support. Workarounds:

- Use the MJPEG fallback stream via `http://IP/cgi-bin/faststream.jpg?stream=full`
- Configure the camera to output H.264 if the model and firmware support it
- For MOVE series cameras, H.264 RTSP is natively supported

### ONVIF discovery not finding the camera

Only MOBOTIX MOVE series cameras support ONVIF. Classic Mx cameras (M, D, S, Q, T series) do not implement the ONVIF protocol. For classic cameras, connect directly using the RTSP or HTTP URLs listed above.

### Low frame rate on MJPEG streams

MOBOTIX classic cameras may default to low frame rates to conserve bandwidth. To adjust:

1. Open the camera web interface
2. Navigate to **Admin Menu > Image Control > Frame Rate**
3. Increase the maximum frame rate
4. For HTTP streams, specify the desired fps in the URL: `http://IP/control/faststream.jpg?stream=full&fps=15`

## FAQ

**What is the default RTSP URL for MOBOTIX cameras?**

For newer MOBOTIX MOVE cameras, the default URL is `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.h264`. For classic Mx cameras, use `rtsp://admin:meinsm@CAMERA_IP:554/mobotix.mjpeg` or the HTTP MJPEG stream at `http://CAMERA_IP/cgi-bin/faststream.jpg?stream=full`.

**What is MxPEG and do I need it?**

MxPEG is a proprietary video codec developed by MOBOTIX for bandwidth-efficient streaming in their decentralized camera architecture. You do not need MxPEG support to use MOBOTIX cameras with VisioForge SDK. Instead, use the standard MJPEG HTTP stream or H.264 RTSP stream (on supported models) as described on this page.

**Do MOBOTIX cameras support ONVIF?**

Only the MOBOTIX MOVE series supports ONVIF. Classic MOBOTIX cameras (M, D, S, Q, T series) use a proprietary web interface and do not support ONVIF discovery or profiles.

**What is the difference between MOBOTIX classic and MOVE cameras?**

Classic MOBOTIX cameras (M, D, S, Q, T series) use a decentralized architecture with in-camera recording and the proprietary MxPEG codec. MOVE series cameras are MOBOTIX's newer product line that follows industry-standard protocols including ONVIF, H.264, and H.265, making them easier to integrate with third-party VMS and SDK solutions.

**Can I connect to MOBOTIX cameras without ONVIF?**

Yes. All MOBOTIX cameras support direct RTSP or HTTP connections using the URLs listed on this page. ONVIF is not required for basic video streaming.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Basler Connection Guide](basler.md) — Industrial / machine vision cameras
- [FLIR Connection Guide](flir.md) — Industrial and thermal imaging
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
