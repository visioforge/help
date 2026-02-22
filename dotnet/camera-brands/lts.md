---
title: LTS IP Camera RTSP URL and C# .NET Connection Guide
description: Connect to LTS (LT Security) cameras in C# .NET with RTSP URL patterns and code samples for CMIP, CMHR series and NVR models. LTS uses Hikvision firmware.
---

# How to Connect to LTS IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**LTS (LT Security Inc.)** is an American security company based in City of Industry, California. LTS cameras are manufactured by **Hikvision** and use Hikvision firmware, protocols, and web interfaces. LTS rebrands Hikvision hardware with US-based technical support and competitive pricing, making it a popular choice in the professional installation market.

Because LTS cameras run Hikvision firmware, they use the same RTSP URL format, ONVIF implementation, and API endpoints as Hikvision cameras. Any integration code written for Hikvision works with LTS and vice versa.

**Key facts:**

- **Product lines:** CMIP (IP cameras), CMHR (HD-TVI analog), LTD (DVRs), LTN (NVRs)
- **Protocol support:** RTSP, ONVIF Profile S/G/T, HTTP/ISAPI
- **Default RTSP port:** 554
- **Default credentials:** admin / 123456 (some models: admin / admin)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265 (CMIP4xxx and newer)
- **OEM base:** Hikvision (identical RTSP URL format)

!!! info "LTS = Hikvision OEM"
    LTS cameras use Hikvision firmware and the exact same RTSP URL format as Hikvision cameras. Any code written for Hikvision cameras works with LTS. See our [Hikvision connection guide](hikvision.md) for additional details.

## RTSP URL Patterns

### Standard URL Format (Hikvision)

Most current LTS cameras use the standard Hikvision `Streaming/Channels` URL pattern:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/Streaming/Channels/[CHANNEL_ID]
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `CHANNEL_ID` | 101 | Channel 1, main stream |
| `CHANNEL_ID` | 102 | Channel 1, sub stream |
| `CHANNEL_ID` | 201 | Channel 2, main stream (NVR) |
| `CHANNEL_ID` | 202 | Channel 2, sub stream (NVR) |

!!! note "Double-slash in URL"
    Some LTS/Hikvision cameras use `//Streaming/Channels/1` (with a double forward slash before `Streaming`). Both single-slash and double-slash variants typically work, but try the double-slash version if the single-slash URL fails.

### RTSP URLs by Model

| Model | RTSP URL | Resolution | Notes |
|-------|----------|------------|-------|
| CMIP3122 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Hikvision standard format |
| CMIP3132-28 | `rtsp://IP:554/Streaming/Channels/101` | 3MP | Hikvision standard format |
| CMIP3432 | `rtsp://IP:554/Streaming/Channels/101` | 4MP | Hikvision standard format |
| CMIP3243 | `rtsp://IP:554/live.h264` | 3MP | Alternative H.264 stream |
| CMIP3412-28 | `rtsp://IP:554/live.h264` | 4MP | Alternative H.264 stream |
| CMIP8232 | `rtsp://IP:554/live.sdp` | 8MP/4K | SDP live stream |
| CMIP8232 (alt) | `rtsp://IP:554/HighResolutionVideo` | 8MP/4K | High resolution stream |
| CMIP8232 (sub) | `rtsp://IP:554/h264/ch1/sub/` | 8MP/4K | H.264 sub stream |
| CMIP series (low-res) | `rtsp://IP:554/LowResolutionVideo` | Varies | Low resolution sub stream |

### Alternative URL Formats

Some older LTS models or specific firmware versions support these alternative URLs:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/Streaming/Channels/101` | Standard Hikvision (recommended) |
| `rtsp://IP:554//Streaming/Channels/1` | Double-slash variant |
| `rtsp://IP:554/live.h264` | H.264 live stream (older CMIP3xxx) |
| `rtsp://IP:554/live.sdp` | SDP live stream (CMIP8xxx) |
| `rtsp://IP:554/HighResolutionVideo` | Named high-resolution stream |
| `rtsp://IP:554/LowResolutionVideo` | Named low-resolution stream |
| `rtsp://IP:554/h264/ch1/sub/` | H.264 sub stream by channel |
| `rtsp://IP:554/cam1/mpeg4?user=USER&pwd=PASS` | MPEG-4 with URL-based auth |

### NVR Channel URLs (LTN Series)

For LTS NVRs (LTN8704, LTN8708, LTN8716, etc.):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/Streaming/Channels/101` | `rtsp://IP:554/Streaming/Channels/102` |
| Camera 2 | `rtsp://IP:554/Streaming/Channels/201` | `rtsp://IP:554/Streaming/Channels/202` |
| Camera N | `rtsp://IP:554/Streaming/Channels/N01` | `rtsp://IP:554/Streaming/Channels/N02` |

### Model Series Summary

| Model Series | Primary RTSP URL | Alternative URLs |
|-------------|------------------|-----------------|
| CMIP3xxx (3MP) | `/Streaming/Channels/101` | `/live.h264` (some models) |
| CMIP4xxx (4MP) | `/Streaming/Channels/101` | `/live.h264` (some models) |
| CMIP8xxx (8MP/4K) | `/Streaming/Channels/101` | `/live.sdp`, `/HighResolutionVideo`, `/h264/ch1/sub/` |
| LTN NVRs | `/Streaming/Channels/N01` | Channel-based |
| LTD DVRs | `/Streaming/Channels/N01` | Channel-based |

## Connecting with VisioForge SDK

Use your LTS camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// LTS CMIP3432, main stream (Hikvision format)
var uri = new Uri("rtsp://192.168.1.80:554/Streaming/Channels/101");
var username = "admin";
var password = "123456";
```

For sub-stream access, use channel ID `102` instead of `101`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/snapshot.jpg` | Standard snapshot |
| 3GP Snapshot | `http://IP/snapshot_3gp.jpg` | 3GP format (mobile-optimized) |
| Stream Snapshot | `http://IP/stream.jpg` | Stream-based snapshot |
| DVR Channel Snapshot | `http://IP/stillimg[CHANNEL].jpg` | Replace `[CHANNEL]` with channel number (LTD DVRs) |
| ISAPI Snapshot | `http://IP/ISAPI/Streaming/channels/101/picture` | Hikvision ISAPI (requires auth) |

## Troubleshooting

### Identifying the correct URL format

LTS cameras span multiple generations with different URL formats. To determine which URL your camera uses:

1. Try the standard Hikvision format first: `rtsp://IP:554/Streaming/Channels/101`
2. If that fails, try the double-slash variant: `rtsp://IP:554//Streaming/Channels/1`
3. For older CMIP3xxx models, try `rtsp://IP:554/live.h264`
4. For CMIP8xxx (4K) models, try `rtsp://IP:554/live.sdp`

### Default credentials and activation

- Older LTS cameras: default password is `123456` or `admin`
- Newer LTS cameras (with Hikvision 5.3+ firmware): require password activation on first use, similar to Hikvision
- If you cannot log in with default credentials, the camera may need to be activated via the LTS Discovery Tool or Hikvision SADP Tool

### Using Hikvision tools with LTS cameras

Since LTS cameras run Hikvision firmware, you can use Hikvision utilities for network discovery and configuration:

- **Hikvision SADP Tool** -- discovers LTS cameras on the local network and can activate/reset them
- **LTS Discovery Tool** -- LTS-branded version of SADP with identical functionality
- **iVMS-4200** -- Hikvision's free VMS software works with LTS cameras

### "401 Unauthorized" error

1. Verify your credentials are correct (default: admin / 123456)
2. On newer firmware, ensure the camera has been activated and you are using the password set during activation
3. Check if the camera has a lockout policy -- too many failed login attempts may temporarily block access
4. Some models require digest authentication rather than basic authentication for RTSP

### Double-slash URL issue

The `//Streaming/Channels/1` URL with a double forward slash at the beginning is a known Hikvision pattern. Some HTTP clients or RTSP libraries may normalize this to a single slash. If your connection fails:

- Ensure your URL string preserves the double slash
- Try both `//Streaming/Channels/1` and `/Streaming/Channels/101` variants

## FAQ

**Are LTS cameras the same as Hikvision?**

LTS cameras are manufactured by Hikvision and run Hikvision firmware. The RTSP URL format (`/Streaming/Channels/101`), ONVIF implementation, and ISAPI interface are identical. The main differences are branding, pricing, and US-based technical support from LTS. Any code written for Hikvision cameras works with LTS cameras.

**What is the default RTSP URL for LTS cameras?**

For most current LTS cameras, use `rtsp://admin:123456@CAMERA_IP:554/Streaming/Channels/101` for the main stream. Use channel ID `102` for the sub stream. Older models may use `/live.h264` or `/live.sdp` instead.

**Do LTS cameras support ONVIF?**

Yes. All current LTS IP cameras (CMIP series) support ONVIF Profile S and Profile T. ONVIF can be used for automatic discovery and configuration alongside direct RTSP URLs.

**What is the difference between CMIP and CMHR series?**

CMIP cameras are IP (network) cameras that support RTSP streaming. CMHR cameras are HD-TVI analog cameras that connect directly to DVRs via coaxial cable and do not have network RTSP capability. Only CMIP series cameras can be connected to via RTSP URLs in software.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hikvision Connection Guide](hikvision.md) — Same URL format (OEM base)
- [Annke Connection Guide](annke.md) — Another Hikvision OEM
- [RTSP Camera Integration Guide](../videocapture/video-sources/ip-cameras/rtsp.md) — LTS RTSP stream configuration
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
