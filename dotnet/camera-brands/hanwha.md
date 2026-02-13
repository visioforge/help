---
title: How to Connect to Hanwha Vision IP Camera in C# .NET
description: Connect to Hanwha Vision cameras in C# .NET with RTSP URL patterns and code samples for X, Q, P, L series and Wisenet NVR models.
---

# How to Connect to Hanwha Vision IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Hanwha Vision** (formerly Hanwha Techwin, formerly Samsung Techwin) is a South Korean video surveillance manufacturer and a subsidiary of Hanwha Group. Hanwha acquired Samsung's security division in 2015 and rebranded to Hanwha Vision in 2023. All cameras are sold under the **Wisenet** product brand. Hanwha Vision is a top-5 global surveillance manufacturer with strong enterprise and government market presence.

**Key facts:**

- **Product lines:** Wisenet X (premium), Wisenet P (AI/4K), Wisenet Q (mainstream), Wisenet L (value), Wisenet T (thermal)
- **Protocol support:** RTSP, ONVIF Profile S/G/T, HTTP/CGI, SUNAPI (proprietary)
- **Default RTSP port:** 554
- **Default credentials:** admin / (set during initial setup; older models: admin / 4321)
- **ONVIF support:** Yes (all current models)
- **Video codecs:** H.264, H.265 (WiseStream II), MJPEG
- **Product brand:** Wisenet (see also our [Samsung/Hanwha guide](samsung.md) for legacy Samsung Techwin URLs)

!!! info "Hanwha Vision vs Samsung vs Wisenet"
    **Hanwha Vision** is the company name (since 2023). **Wisenet** is the product brand for all cameras and NVRs. **Samsung Techwin** was the previous company name (before 2015). Our [Samsung/Hanwha guide](samsung.md) covers legacy Samsung-branded models. This page covers current Hanwha Vision / Wisenet products.

## RTSP URL Patterns

### Standard URL Format

Hanwha Vision cameras use a profile-based RTSP URL structure:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/profile[N]/media.smp
```

| Parameter | Value | Description |
|-----------|-------|-------------|
| `profile1` | Profile 1 | Typically configured as main stream |
| `profile2` | Profile 2 | Typically configured as sub stream |
| `profile3` | Profile 3 | Third stream (if configured) |

### Camera Models

| Model Series | Resolution | Main Stream URL | Audio |
|-------------|-----------|----------------|-------|
| XNO-6080R (X 2MP bullet) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Yes |
| XNO-8080R (X 5MP bullet) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Yes |
| XNO-9080R (X 4K bullet) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Yes |
| XND-6080 (X 2MP dome) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Yes |
| XND-8080RV (X 5MP dome) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Yes |
| XNP-6120H (X 2MP PTZ) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | Yes |
| PNO-A9081R (P 4K AI bullet) | 3840x2160 | `rtsp://IP:554/profile2/media.smp` | Yes |
| QNO-8080R (Q 5MP bullet) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Yes |
| QND-8080R (Q 5MP dome) | 2560x1920 | `rtsp://IP:554/profile2/media.smp` | Yes |
| LNO-6032R (L 2MP bullet) | 1920x1080 | `rtsp://IP:554/profile2/media.smp` | No |

!!! tip "Profile Numbering"
    On most Hanwha Vision cameras, `profile2` is the main stream and `profile1` is reserved for internal use. If `profile2` does not work, try `profile1` or `profile3`. You can verify profile assignments in the camera's web interface under **Video Profile**.

### NVR Channel URLs

For Wisenet NVRs (XRN, QRN, LRN series):

| Channel | Main Stream | Sub Stream |
|---------|-------------|------------|
| Camera 1 | `rtsp://IP:554/profile2/media.smp/trackID=channel1` | `rtsp://IP:554/profile3/media.smp/trackID=channel1` |
| Camera 2 | `rtsp://IP:554/profile2/media.smp/trackID=channel2` | `rtsp://IP:554/profile3/media.smp/trackID=channel2` |
| Camera N | `rtsp://IP:554/profile2/media.smp/trackID=channelN` | `rtsp://IP:554/profile3/media.smp/trackID=channelN` |

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/profile2/media.smp` | Standard (recommended) |
| `rtsp://IP:554/profile1/media.smp` | First profile |
| `rtsp://IP:554/onvif-media/media.amp` | ONVIF media service |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Some OEM variants |

## Connecting with VisioForge SDK

Use your Hanwha Vision camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Hanwha Vision XNO-8080R (Wisenet X 5MP), main stream
var uri = new Uri("rtsp://192.168.1.90:554/profile2/media.smp");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/profile3/media.smp` instead of `/profile2/media.smp`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/video.cgi?msubmenu=jpg&action=view&Resolution=1920x1080&Quality=5&Channel=0` | Full resolution snapshot |
| JPEG Snapshot (simple) | `http://IP/cgi-bin/snapshot.cgi` | Requires digest auth |
| MJPEG Stream | `http://IP/cgi-bin/video.cgi?msubmenu=mjpeg&action=view&Channel=0&Stream=0` | Continuous MJPEG |

## Troubleshooting

### profile2 vs profile1 confusion

Hanwha Vision cameras typically assign `profile2` as the main (highest quality) stream, which differs from most other brands that use profile/channel 1. If you get no video or low resolution from `profile2`, check the profile configuration in the camera's web interface under **Video Profile**.

### Password activation required

Current Hanwha Vision cameras ship without a default password. You must activate the camera and set a password through:

1. Wisenet Installation Wizard (IP Installer tool)
2. Web browser at `http://CAMERA_IP`
3. Wisenet mobile app

Older Samsung Techwin models used `admin` / `4321` as defaults.

### WiseStream II codec

WiseStream II is Hanwha's dynamic encoding technology that adjusts compression per-region in the frame. It produces standard H.265 or H.264 streams that are compatible with any decoder. No special codec is required.

### SUNAPI vs ONVIF

Hanwha Vision cameras support both their proprietary SUNAPI and standard ONVIF. For VisioForge SDK integration, use either the RTSP URLs above or ONVIF discovery. SUNAPI is primarily used by Hanwha's own VMS (SSM/Wisenet WAVE).

## FAQ

**What is the default RTSP URL for Hanwha Vision (Wisenet) cameras?**

The standard URL is `rtsp://admin:password@CAMERA_IP:554/profile2/media.smp` for the main stream. Use `profile3` for the sub stream. The profile numbers can be customized in the camera's web interface.

**Are Hanwha Vision and Samsung cameras the same?**

Hanwha Vision acquired Samsung's security camera division in 2015 (then called Samsung Techwin, later Hanwha Techwin, now Hanwha Vision). Current cameras are sold under the **Wisenet** brand. Legacy Samsung-branded cameras may use different URL patterns -- see our [Samsung/Hanwha guide](samsung.md).

**What is the difference between Wisenet X, P, Q, and L series?**

**X** = premium enterprise (best low-light, WDR). **P** = AI-powered (deep learning analytics). **Q** = mainstream business (good balance of features and price). **L** = value/entry-level (basic features, competitive pricing). **T** = thermal imaging.

**Do Hanwha Vision cameras support ONVIF?**

Yes. All current Hanwha Vision cameras support ONVIF Profile S, G, and T. ONVIF provides standardized discovery, streaming, and PTZ control.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Samsung/Hanwha Legacy Guide](samsung.md) — Older Samsung Techwin models
- [Wisenet Product Guide](wisenet.md) — Wisenet product family details
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
