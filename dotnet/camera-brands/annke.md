---
title: How to Connect to Annke IP Camera in C# .NET
description: Connect to Annke cameras in C# .NET with RTSP URL patterns and code samples for C500, C800, CZ400, NC400 and NVR models.
---

# How to Connect to Annke IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Annke** (Annke Innovation Co., Ltd.) is a consumer and prosumer security camera brand based in Hong Kong, primarily selling through Amazon and direct-to-consumer channels. Annke cameras are manufactured using **Hikvision** OEM hardware, and most models use Hikvision-compatible firmware and RTSP URL patterns. Annke offers competitive pricing on PoE cameras, NVRs, and complete surveillance kits.

**Key facts:**

- **Product lines:** C-series (IP cameras), CZ-series (PTZ), NC-series (NVRs), I-series (turret/dome)
- **Protocol support:** RTSP, ONVIF Profile S, HTTP/CGI
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during initial setup; some models: admin / admin)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265 (4MP and above)
- **OEM base:** Hikvision (most models use Hikvision-compatible firmware)

!!! info "Annke Uses Hikvision Firmware"
    Most Annke cameras use Hikvision OEM firmware. The RTSP URL format (`/Streaming/Channels/`) is identical to Hikvision. See our [Hikvision connection guide](hikvision.md) for additional details and troubleshooting.

## RTSP URL Patterns

### Standard URL Format

Annke cameras use the Hikvision `Streaming/Channels` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| Channel ID | Stream | Description |
|-----------|--------|-------------|
| 101 | Main stream | Full resolution |
| 102 | Sub stream | Lower resolution |
| 103 | Third stream | Mobile-optimized (if supported) |

### Camera Models

| Model | Resolution | Main Stream URL | Audio |
|-------|-----------|----------------|-------|
| C500 (5MP bullet) | 2592x1944 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| C800 (4K bullet) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| C1200 (12MP bullet) | 4000x3000 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| CZ400 (4MP PTZ) | 2560x1440 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| I91BN (4K turret) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| I91BM (4K dome) | 3840x2160 | `rtsp://IP:554/Streaming/Channels/101` | Yes |
| NC400 (4ch NVR) | N/A | See NVR section | N/A |
| N48PAW (8ch PoE NVR) | N/A | See NVR section | N/A |

### NVR Channel URLs

For Annke NVRs (NC400, N48PAW, N46PCK, etc.):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Camera 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Camera N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Alternative URL Formats

Some Annke models (especially non-Hikvision OEM variants) use different URL patterns:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Hikvision-style (most models) |
| `rtsp://IP:554/h264/ch1/main/av_stream` | Older Hikvision firmware |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-style (some older models) |

## Connecting with VisioForge SDK

Use your Annke camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Annke C800 (4K bullet), main stream
var uri = new Uri("rtsp://192.168.1.90:554/Streaming/Channels/101");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/Streaming/Channels/102` instead.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/ISAPI/Streaming/channels/101/picture` | Requires digest auth |
| MJPEG Stream | `http://IP/ISAPI/Streaming/channels/102/httpPreview` | Sub stream as MJPEG |
| Legacy Snapshot | `http://IP/Streaming/channels/1/picture` | Older firmware |

## Troubleshooting

### Camera requires activation

Annke cameras with newer firmware require initial activation (password setup) before RTSP access works. Use the camera's web interface at `http://CAMERA_IP` or Annke's SADP-compatible discovery tool.

### Hikvision URL format not working

Some Annke models use different OEM firmware. If `/Streaming/Channels/101` does not work, try:

1. `/h264/ch1/main/av_stream` (older Hikvision firmware)
2. `/cam/realmonitor?channel=1&subtype=0` (Dahua-style)
3. Use ONVIF discovery to automatically retrieve the correct stream URL

### H.265 stream issues

Annke 4K cameras (C800, I91BN) default to H.265 encoding. If playback fails, switch the camera to H.264 in the web interface or install the HEVC decoder redistributable.

## FAQ

**What is the default RTSP URL for Annke cameras?**

Most Annke cameras use `rtsp://admin:password@CAMERA_IP:554/Streaming/Channels/101` for the main stream. Use channel `102` for the sub stream. This is the same format as Hikvision cameras.

**Are Annke cameras Hikvision OEMs?**

Most Annke cameras use Hikvision OEM hardware and firmware. The RTSP URL format, web interface, and API are typically identical to Hikvision. Some Annke models may use different OEM bases.

**Do Annke cameras support ONVIF?**

Yes. All current Annke cameras support ONVIF Profile S, providing standardized discovery and stream access.

**Can I mix Annke cameras with Hikvision NVRs?**

Yes. Since Annke cameras use Hikvision-compatible protocols, they work natively with Hikvision NVRs and vice versa. You can also mix Annke cameras into any ONVIF-compatible NVR or VMS.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hikvision Connection Guide](hikvision.md) — Same URL format (OEM base)
- [LTS Connection Guide](lts.md) — Another Hikvision OEM
- [Dahua Connection Guide](dahua.md) — Alternative OEM ecosystem
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
