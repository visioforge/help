---
title: How to Connect to Uniview (UNV) IP Camera in C# .NET
description: Uniview IPC-B, IPC-T, IPC-D, IPC-E, and NVR RTSP URL patterns for C# .NET. ONVIF-compatible integration with VisioForge SDK code samples.
---

# How to Connect to Uniview (UNV) IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Uniview** (Zhejiang Uniview Technologies Co., Ltd.), also known as **UNV**, is the world's third-largest video surveillance manufacturer by market share, behind Hikvision and Dahua. Founded in 2005 and headquartered in Hangzhou, China, Uniview pioneered IP video surveillance in China and offers a full range of IP cameras, NVRs, VMS software, and access control products for enterprise and government markets.

**Key facts:**

- **Product lines:** IPC-B (bullet), IPC-T (turret), IPC-D (dome), IPC-E (eyeball), IPC-P (PTZ), NVR30x/50x (NVRs)
- **Protocol support:** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SDK (EZStation)
- **Default RTSP port:** 554
- **Default credentials:** admin / 123456 (must be changed on first login with newer firmware)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265 (U-Code Smart Codec), MJPEG
- **Market position:** #3 globally in video surveillance

!!! info "Uniview vs UNV Branding"
    Uniview markets under both the **Uniview** and **UNV** brand names depending on the region. The RTSP URL patterns and firmware are identical regardless of branding. Some OEM partners rebrand Uniview hardware under their own labels.

## RTSP URL Patterns

### Standard URL Format

Uniview cameras use a media profile-based URL structure:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/media/video[STREAM]
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `video1` | Main stream | Highest resolution (4K/5MP/4MP/2MP) |
| `video2` | Sub stream | Lower resolution, reduced bandwidth |
| `video3` | Third stream | Mobile-optimized (if supported) |

### Alternative URL Formats

Uniview cameras support multiple RTSP URL patterns:

| URL Pattern | Description |
|-------------|-------------|
| `rtsp://IP:554/media/video1` | Main stream (recommended) |
| `rtsp://IP:554/media/video2` | Sub stream |
| `rtsp://IP:554/media/video3` | Third stream |
| `rtsp://IP:554/unicast/c1/s0/live` | Unicast main stream (alternative) |
| `rtsp://IP:554/unicast/c1/s1/live` | Unicast sub stream (alternative) |
| `rtsp://IP:554/live/ch00_0` | Legacy format (older firmware) |
| `rtsp://IP:554/live/ch00_1` | Legacy sub stream |

### IP Camera Models

| Model Series | Resolution | Main Stream URL | Audio |
|-------------|-----------|----------------|-------|
| IPC-B112-PF28 (2MP bullet) | 1920x1080 | `rtsp://IP:554/media/video1` | No |
| IPC-B314-APKZ (4MP bullet) | 2688x1520 | `rtsp://IP:554/media/video1` | Yes |
| IPC-B315-APKZ (5MP bullet) | 2880x1620 | `rtsp://IP:554/media/video1` | Yes |
| IPC-T112-PF28 (2MP turret) | 1920x1080 | `rtsp://IP:554/media/video1` | No |
| IPC-T314-APKZ (4MP turret) | 2688x1520 | `rtsp://IP:554/media/video1` | Yes |
| IPC-D312-APKZ (4MP dome) | 2688x1520 | `rtsp://IP:554/media/video1` | Yes |
| IPC-D314-APKZ (4MP dome) | 2688x1520 | `rtsp://IP:554/media/video1` | Yes |
| IPC-E312-APKZ (4MP eyeball) | 2688x1520 | `rtsp://IP:554/media/video1` | Yes |
| IPC-P1E2-I (2MP PTZ) | 1920x1080 | `rtsp://IP:554/media/video1` | Yes |
| IPC-B182-PF28 (4K bullet) | 3840x2160 | `rtsp://IP:554/media/video1` | Yes |

### NVR Channel URLs

For Uniview NVRs (NVR301, NVR302, NVR304, NVR501, NVR516):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/media/video1` | `rtsp://IP:554/media/video2` |
| Camera 2 | `rtsp://IP:554/media/video3` | `rtsp://IP:554/media/video4` |
| Camera 3 | `rtsp://IP:554/media/video5` | `rtsp://IP:554/media/video6` |
| Camera N | `rtsp://IP:554/media/video[2N-1]` | `rtsp://IP:554/media/video[2N]` |

!!! tip "NVR Channel Numbering"
    On Uniview NVRs, the video stream number encodes both the channel and stream type. Each channel uses two consecutive numbers: odd for main stream, even for sub stream. Camera 1 = video1/video2, Camera 2 = video3/video4, and so on.

## Connecting with VisioForge SDK

Use your Uniview camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Uniview IPC-B314-APKZ, main stream
var uri = new Uri("rtsp://192.168.1.90:554/media/video1");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/media/video2` instead of `/media/video1`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Requires digest authentication |
| ONVIF Snapshot | `http://IP/onvif-http/snapshot?channel=1` | ONVIF HTTP snapshot |

## Troubleshooting

### Default password must be changed

Uniview cameras with current firmware require the default password (`123456`) to be changed during initial setup. If you haven't configured the camera yet:

1. Access the camera at `http://CAMERA_IP` in a browser
2. Complete the activation wizard
3. Set a strong password
4. Use those credentials in your RTSP URL

### "unicast" vs "media" URL format

If `/media/video1` does not work on your camera, try the unicast format: `rtsp://IP:554/unicast/c1/s0/live`. Older Uniview firmware versions may only support the unicast path. Newer firmware supports both formats.

### H.265 stream not playing

Uniview's U-Code smart codec produces standard H.265/HEVC streams. If H.265 playback fails:

1. Install the HEVC decoder redistributable
2. Or switch the camera to H.264 encoding in the web interface: **Setup > Video > Video**
3. Use `rtspSettings.UseGPUDecoder = true` for hardware-accelerated H.265 decoding

### ONVIF discovery issues

ONVIF is enabled by default on Uniview cameras but may require a separate ONVIF password. Check **Setup > Network > ONVIF** in the web interface and ensure the ONVIF user account is configured.

## FAQ

**What is the default RTSP URL for Uniview cameras?**

The standard URL is `rtsp://admin:password@CAMERA_IP:554/media/video1` for the main stream. Use `/media/video2` for the sub stream. Some older models use `rtsp://IP:554/unicast/c1/s0/live` instead.

**Is Uniview the same as UNV?**

Yes. Uniview and UNV are the same company (Zhejiang Uniview Technologies). The branding varies by region. All cameras use identical firmware, RTSP URL formats, and web interfaces regardless of whether they carry the Uniview or UNV label.

**Do Uniview cameras support ONVIF?**

Yes. All current Uniview cameras support ONVIF Profile S and Profile T. ONVIF allows automatic camera discovery and standardized stream access without using brand-specific RTSP URLs.

**How do I access multiple channels on a Uniview NVR?**

Uniview NVRs use sequential video stream numbers: Camera 1 = video1 (main) / video2 (sub), Camera 2 = video3 (main) / video4 (sub), and so on. The formula is: main stream = video[2N-1], sub stream = video[2N] where N is the camera channel number.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hikvision Connection Guide](hikvision.md) — Global market leader, different URL format
- [Dahua Connection Guide](dahua.md) — Another major Chinese surveillance brand
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
