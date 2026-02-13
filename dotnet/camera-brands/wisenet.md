---
title: How to Connect to Wisenet IP Camera in C# .NET
description: Connect to Wisenet cameras in C# .NET with RTSP URL patterns for Wisenet X, P, Q, L series by Hanwha Vision. Complete code samples and NVR guide.
---

# How to Connect to Wisenet IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Wisenet** is the product brand name used by **Hanwha Vision** (formerly Hanwha Techwin / Samsung Techwin) for all IP cameras, NVRs, and video management systems. Wisenet is not a separate company but rather the product family name used across Hanwha Vision's full surveillance lineup.

**Key facts:**

- **Manufacturer:** Hanwha Vision (South Korea)
- **Product tiers:** X (premium), P (AI), Q (mainstream), Q mini (compact), L (value), T (thermal)
- **Protocol support:** RTSP, ONVIF Profile S/G/T, SUNAPI (proprietary)
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during activation)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265, WiseStream III, MJPEG

!!! info "Wisenet = Hanwha Vision Products"
    Wisenet is the **product brand**, Hanwha Vision is the **company**. All Wisenet cameras use the same RTSP URL patterns. For detailed connection instructions including NVR channel access and troubleshooting, see our [Hanwha Vision connection guide](hanwha.md). For legacy Samsung-branded cameras, see the [Samsung/Hanwha guide](samsung.md).

## RTSP URL Patterns

### Standard URL Format

All Wisenet cameras share the same profile-based URL structure:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

### By Product Tier

| Wisenet Tier | Example Models | Main Stream URL | Key Feature |
|-------------|---------------|----------------|-------------|
| **X series** (premium) | XNO-6080R, XND-8080RV, XNP-6120H | `rtsp://IP:554/profile2/media.smp` | Best WDR, low-light |
| **P series** (AI) | PNO-A9081R, PND-A9081RV | `rtsp://IP:554/profile2/media.smp` | Deep learning analytics |
| **Q series** (mainstream) | QNO-8080R, QND-8080R, QNE-8021R | `rtsp://IP:554/profile2/media.smp` | Balanced features/price |
| **Q mini** (compact) | QND-8021 | `rtsp://IP:554/profile2/media.smp` | Discreet form factor |
| **L series** (value) | LNO-6032R, LND-6032R | `rtsp://IP:554/profile2/media.smp` | Entry-level |
| **T series** (thermal) | TNO-4030T, TNO-4050T | `rtsp://IP:554/profile2/media.smp` | Thermal + visible |

### Multi-Sensor and Multi-Directional Models

| Model Type | Stream URL | Notes |
|-----------|-----------|-------|
| Single sensor | `rtsp://IP:554/profile2/media.smp` | Standard |
| Multi-sensor channel 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | First sensor |
| Multi-sensor channel 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | Second sensor |
| Stitched panoramic | `rtsp://IP:554/profile2/media.smp/trackID=channel5` | Combined view |

### NVR / WAVE VMS Access

For Wisenet NVRs (XRN, QRN, LRN series):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Camera 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Camera N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

## Connecting with VisioForge SDK

Use your Wisenet camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Wisenet QNO-8080R (Q series 5MP), main stream
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/profile3/media.smp` instead of `/profile2/media.smp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Requires digest auth |
| MJPEG Stream | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | Continuous MJPEG |

## Troubleshooting

### Which profile number is the main stream?

Wisenet cameras typically use `profile2` as the main (highest quality) stream. This is different from most other brands. If you get unexpected results, check the profile configuration in the camera's web interface (**Setup > Video/Audio > Video Profile**).

### WiseStream III bandwidth savings

WiseStream III dynamically adjusts encoding per region in the frame. The output is standard H.265 or H.264 -- no special decoder is needed. WiseStream settings can be configured in the camera's web interface.

### Camera activation

New Wisenet cameras require activation (setting a password) before use. Use the Wisenet Installation Wizard utility, web browser, or Wisenet mobile app for initial setup.

## FAQ

**What is the default RTSP URL for Wisenet cameras?**

All Wisenet cameras use `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` for the main stream. Use `profile3` for the sub stream.

**What is the difference between Wisenet X, P, Q, and L?**

**X** = premium enterprise. **P** = AI-powered analytics. **Q** = mainstream business. **L** = value/entry-level. **T** = thermal. All tiers use the same RTSP URL format.

**Is Wisenet the same as Samsung security cameras?**

Wisenet is the current product brand from Hanwha Vision, which acquired Samsung's security division in 2015. Legacy Samsung Techwin cameras may use different URL formats. See our [Samsung/Hanwha guide](samsung.md) for older models.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hanwha Vision Connection Guide](hanwha.md) — Detailed Hanwha Vision integration
- [Samsung/Hanwha Legacy Guide](samsung.md) — Older Samsung Techwin models
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
