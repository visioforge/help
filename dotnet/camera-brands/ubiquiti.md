---
title: How to Connect to Ubiquiti (UniFi) IP Camera in C# .NET
description: Ubiquiti UniFi Protect G3, G4, G5, and AI series RTSP URL patterns for C# .NET. Enable RTSP in UniFi and integrate with VisioForge SDK code.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
  - VideoCaptureCoreX
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
  - C#

---

# How to Connect to Ubiquiti (UniFi) IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Ubiquiti Inc.** is an American technology company headquartered in New York City, known for networking equipment under the **UniFi** brand. Ubiquiti's camera line is part of the **UniFi Protect** ecosystem, which includes cameras, NVRs (Network Video Recorders), doorbells, and sensors. UniFi Protect cameras are managed through a central console (Dream Machine, Cloud Key, or NVR) and are popular in prosumer and SMB environments.

**Key facts:**

- **Product lines:** UniFi Protect G3 (1080p), G4 (2K/4MP), G5 (2K/4MP updated), AI series (with onboard AI), UVC (legacy AirCam)
- **Protocol support:** RTSP (must be enabled per camera), ONVIF (limited), proprietary UniFi Protect protocol
- **Default RTSP port:** 7447 (UniFi Protect) or 554 (legacy AirCam)
- **Default credentials:** Set during UniFi Protect setup (RTSP uses separate per-camera credentials)
- **ONVIF support:** Not natively supported; RTSP is the third-party integration method
- **Video codecs:** H.264 (all models)

!!! warning "RTSP must be enabled"
    UniFi Protect cameras do **not** have RTSP enabled by default. You must enable RTSP for each camera individually through the UniFi Protect web interface or app. Without enabling it, the camera will not respond to RTSP connections.

### Enabling RTSP on UniFi Protect Cameras

1. Open the **UniFi Protect** web interface (via your Dream Machine, Cloud Key, or NVR)
2. Go to **Devices** and select the camera
3. Open **Settings** tab
4. Scroll to **Advanced** section
5. Enable **RTSP** toggle
6. Note the RTSP URL displayed (includes unique token)

## RTSP URL Patterns

### UniFi Protect Cameras (Current)

UniFi Protect cameras expose RTSP on **port 7447** with stream quality selection:

| Stream | RTSP URL | Resolution | Notes |
|--------|----------|------------|-------|
| High quality | `rtsp://IP:7447/STREAM_TOKEN` | Full (up to 2688x1512) | Main stream |
| Medium quality | `rtsp://IP:7447/STREAM_TOKEN` | Reduced | Medium stream |
| Low quality | `rtsp://IP:7447/STREAM_TOKEN` | Low (640x360) | Bandwidth-optimized |

!!! info "Stream tokens"
    UniFi Protect generates unique RTSP URLs per camera when you enable RTSP. The URL contains a unique token. You can find the exact URL in the UniFi Protect interface under each camera's Advanced settings.

The RTSP URL format is typically:

```
rtsp://CAMERA_IP:7447/UNIQUE_TOKEN_STRING
```

Where the token is auto-generated and displayed in the UniFi Protect UI.

### UniFi Protect Camera Models

| Model | Resolution | Streams | Form Factor |
|-------|-----------|---------|-------------|
| G3 Instant | 1920x1080 | High/Low | Indoor mini |
| G3 Flex | 1920x1080 | High/Medium/Low | Indoor/outdoor flex |
| G3 Bullet | 1920x1080 | High/Medium/Low | Outdoor bullet |
| G3 Dome | 1920x1080 | High/Medium/Low | Outdoor dome |
| G4 Instant | 2688x1512 | High/Medium/Low | Indoor mini |
| G4 Bullet | 2688x1512 | High/Medium/Low | Outdoor bullet |
| G4 Dome | 2688x1512 | High/Medium/Low | Outdoor dome |
| G4 Pro | 3840x2160 | High/Medium/Low | Outdoor pro |
| G4 PTZ | 3840x2160 | High/Medium/Low | PTZ |
| G5 Bullet | 2688x1512 | High/Medium/Low | Outdoor bullet |
| G5 Dome | 2688x1512 | High/Medium/Low | Outdoor dome |
| G5 Turret Ultra | 3840x2160 | High/Medium/Low | Outdoor turret |
| AI 360 | 3840x2160 | High/Medium/Low | Fisheye |
| AI Bullet | 3840x2160 | High/Medium/Low | Outdoor bullet |
| AI Pro | 3840x2160 | High/Medium/Low | Outdoor pro |

### Legacy AirCam/AirVision URLs

Older Ubiquiti cameras (AirCam series, before UniFi Protect) used standard port 554:

| Model | RTSP URL | Notes |
|-------|----------|-------|
| AirCam | `rtsp://IP:554/live/ch00_0` | Main stream |
| AirCam Dome | `rtsp://IP:554/live/ch00_0` | Dome variant |
| AirCam Mini | `rtsp://IP:554/live/ch00_0` | Mini variant |
| AirCam (channel) | `rtsp://IP:554/ch0N_0` | N = channel number |

## Connecting with VisioForge SDK

Use your UniFi Protect camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// UniFi Protect camera, token-based auth (no username/password needed)
var uri = new Uri("rtsp://192.168.1.40:7447/YOUR_STREAM_TOKEN");
```

UniFi Protect cameras use token-based authentication -- the unique stream token is provided in the UniFi Protect UI when you enable RTSP. No separate username or password is required. For different quality streams (high/medium/low), select the corresponding stream in the Protect interface to get its token.

For legacy AirCam models, use port 554 with credentials `ubnt`/`ubnt` and path `/live/ch00_0`.

## Snapshot URLs

### Legacy AirCam

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Snapshot | `http://IP/snapshot.cgi` | Basic snapshot |
| Snapshot (auth) | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | With credentials |
| Snapshot (alt) | `http://IP:554/snapshot.cgi?user=USER&pwd=PASS&count=0` | Via RTSP port |

### UniFi Protect

UniFi Protect cameras do not expose HTTP snapshot endpoints directly. Snapshots are accessed through the UniFi Protect API or by capturing frames from the RTSP stream in your application.

## Troubleshooting

### "Connection refused" on port 554

UniFi Protect cameras use **port 7447** for RTSP, not the standard port 554. Port 554 only applies to legacy AirCam models. Make sure you're using the correct port:

- **UniFi Protect cameras:** Port 7447
- **Legacy AirCam:** Port 554

### RTSP not enabled

RTSP is disabled by default on UniFi Protect cameras. You must enable it in the UniFi Protect interface:

1. UniFi Protect > Devices > Select Camera > Settings > Advanced > Enable RTSP

### Stream token changed

The RTSP stream token can change if you:
- Disable and re-enable RTSP on the camera
- Reset the camera
- Update firmware

Always verify the current RTSP URL in the UniFi Protect interface if your connection stops working.

### High latency

UniFi Protect cameras can exhibit 2-5 second latency by default. To reduce latency:

- Set `LowLatencyMode = true` on the `RTSPSourceSettings` passed to VideoCaptureCoreX
- Select the low-quality stream (lower resolution = less buffering)
- Use TCP transport for more reliable delivery

### No ONVIF support

UniFi Protect cameras do not support ONVIF. Use RTSP for third-party integration. If you need ONVIF discovery, it won't work with these cameras.

## FAQ

**What is the default RTSP URL for UniFi Protect cameras?**

The RTSP URL format is `rtsp://CAMERA_IP:7447/UNIQUE_TOKEN`. RTSP must be enabled per-camera in the UniFi Protect interface, which will display the unique URL. There is no universal default URL -- each camera gets a unique stream token.

**Can I use UniFi cameras without UniFi Protect?**

Current UniFi cameras require a UniFi Protect controller (Dream Machine, Cloud Key, or NVR) for initial setup and management. Once RTSP is enabled, you can stream to third-party software. Legacy AirCam models work standalone.

**Do UniFi cameras support H.265?**

As of current firmware, UniFi Protect cameras stream H.264 over RTSP. H.265 support may be available for internal recording but is not typically exposed via RTSP.

**What are the default credentials for AirCam?**

Legacy AirCam cameras use `ubnt` / `ubnt` as default credentials. Current UniFi Protect cameras use token-based RTSP authentication.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Reolink Connection Guide](reolink.md) — Prosumer alternative with RTSP
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
