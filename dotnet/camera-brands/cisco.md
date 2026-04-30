---
title: Cisco IP Camera RTSP URL Setup and C# .NET Integration
description: Cisco CIVS and Meraki camera RTSP integration for C# .NET. URL patterns for enterprise PVC, VC models with VisioForge SDK code and ONVIF examples.
tags:
  - Video Capture SDK
  - Media Blocks SDK
  - .NET
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
  - H.264
  - MJPEG
  - C#

---

# How to Connect to Cisco IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Cisco Systems** is the world's largest networking company, headquartered in San Jose, California, USA. Cisco produced IP cameras under both the **Cisco** and **Linksys** brands. The main camera lines were **Cisco Video Surveillance (CIVS/VC/PVC)** for enterprise deployments and the former **Linksys-branded (WVC)** series for consumer and small business markets. Cisco sold their video surveillance business to Verkada and discontinued most camera products, but many units remain deployed in the field. The **Meraki MV** line (cloud-managed) is the only current Cisco camera product.

**Key facts:**

- **Product lines:** CIVS (enterprise video surveillance), PVC (small business), VC (video camera), WVC (wireless video camera, Linksys legacy), WCS (wireless camera server), Meraki MV (cloud-managed, current)
- **Protocol support:** RTSP, HTTP/CGI, MJPEG, ASF (WVC models); Meraki MV is cloud-only
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (varies by model)
- **ONVIF support:** Limited (some newer CIVS models only; WVC series does not support ONVIF)
- **Video codecs:** H.264, MPEG-4 (WVC series), MJPEG

!!! warning "Meraki MV cameras"
    Cisco Meraki MV cameras use cloud-only access and do **not** support direct RTSP streaming. They cannot be connected via local RTSP URLs. The information on this page applies to the legacy Cisco and Linksys camera product lines.

## RTSP URL Patterns

### Enterprise Cameras (CIVS / VC / PVC Series)

Most Cisco enterprise IP cameras use the `/img/media.sav` path:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:554/img/media.sav` | Most CIVS and WVC cameras |
| Live SDP | `rtsp://IP:554/live.sdp` | VC240, PVC300, VC220 series |
| Video SAV | `rtsp://IP:554/img/video.sav` | PVC2300 |
| Access code | `rtsp://IP:554/[ACCESS_CODE]` | Code configured in camera web UI |
| Root stream | `rtsp://IP:554/` | Fallback for PVC2300 and others |

### Model-Specific URLs

| Model | RTSP URL | Type |
|-------|----------|------|
| CIVS 2500 / 2521 / 2531 | `rtsp://IP:554/img/media.sav` | Enterprise dome/bullet |
| PVC300 | `rtsp://IP:554/live.sdp` | Small business PTZ |
| PVC2300 | `rtsp://IP:554/img/video.sav` | Small business box |
| PVC2300 (alt) | `rtsp://IP:554/` | Fallback |
| VC220 | `rtsp://IP:554/live.sdp` | Dome camera |
| VC240 | `rtsp://IP:554/live.sdp` | Dome camera |
| WVC80N | `rtsp://IP:554/img/media.sav` | Linksys wireless |
| WVC210 | `rtsp://IP:554/img/media.sav` | Linksys wireless |
| WVC54GCA | `rtsp://IP:554/img/media.sav` | Linksys wireless |
| WCS-1130 | `rtsp://IP:554/play1.sdp` | Wireless camera server |

!!! info "Unusual file extension"
    Cisco and Linksys cameras use the `/img/media.sav` path, which has an unusual `.sav` file extension. This is not a standard media format -- it is a Cisco-specific RTSP endpoint. Do not confuse it with a file download URL.

### WVC Series (Linksys Legacy)

The Linksys WVC (Wireless Video Camera) series was a popular consumer camera line before Cisco rebranded and eventually discontinued it:

| Model | RTSP URL | Resolution | Notes |
|-------|----------|------------|-------|
| WVC54GCA | `rtsp://IP:554/img/media.sav` | 640x480 | Wi-Fi, MPEG-4 |
| WVC80N | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-N |
| WVC210 | `rtsp://IP:554/img/media.sav` | 640x480 | Wireless-G PTZ |

## Connecting with VisioForge SDK

Use your Cisco camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Cisco enterprise camera (CIVS/WVC), main stream
var uri = new Uri("rtsp://192.168.1.50:554/img/media.sav");
var username = "admin";
var password = "YourPassword";
```

For VC240 or PVC300 cameras, use `/live.sdp` instead of `/img/media.sav`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/img/snapshot.cgi?size=3` | Most Cisco cameras (size: 1=QQVGA, 2=QVGA, 3=VGA) |
| JPEG Snapshot (VGA) | `http://IP/img/snapshot.cgi?img=vga` | Named resolution |
| MJPEG Stream | `http://IP/img/video.mjpeg` | Continuous MJPEG stream |
| MJPEG (alt) | `http://IP/img/mjpeg.jpg` | Alternative MJPEG endpoint |
| ASF Stream | `http://IP/img/video.asf` | WVC series ASF stream |
| PVC300 Snapshot | `http://IP/cgi-bin/viewer/snapshot.jpg?resolution=640x480` | Resolution parameter required |

## Troubleshooting

### `/img/media.sav` not responding

The `.sav` extension is a Cisco-specific RTSP endpoint. If the URL does not work:

1. Verify the camera IP address and that port 554 is open
2. Confirm RTSP is enabled in the camera's web UI
3. Some models require an access code to be configured before RTSP access works -- check the camera's streaming settings
4. Try the fallback URL `rtsp://IP:554/` if the specific path does not respond

### WVC series returns MPEG-4 only

The Linksys WVC cameras (WVC54GCA, WVC80N, WVC210) support MPEG-4 but not H.264. The VisioForge SDK handles MPEG-4 streams automatically. If you see artifacts, ensure you are not forcing H.264 decoding.

### Access code authentication

Some Cisco cameras use an access code instead of traditional username/password for RTSP. The access code is configured in the camera's web interface under streaming settings and is appended to the URL:

```
rtsp://IP:554/[YOUR_ACCESS_CODE]
```

### HTTP streams for legacy cameras

Older Cisco/Linksys cameras may work better with HTTP-based ASF or MJPEG streams than RTSP. Use the ASF URL (`http://IP/img/video.asf`) as a fallback if RTSP is unreliable.

### Meraki MV cameras not accessible

Meraki MV cameras are cloud-managed only and do not support local RTSP access. There is no local RTSP URL available for these cameras. Video can only be accessed through the Meraki Dashboard cloud interface.

## FAQ

**What is the default RTSP URL for Cisco cameras?**

For most Cisco and Linksys IP cameras, use `rtsp://admin:password@CAMERA_IP:554/img/media.sav`. For VC/PVC series cameras, try `rtsp://admin:password@CAMERA_IP:554/live.sdp` instead.

**Do Cisco cameras support ONVIF?**

Only some newer CIVS-series enterprise cameras have limited ONVIF support. The Linksys WVC consumer cameras and Meraki MV cameras do not support ONVIF.

**Are Cisco cameras still being manufactured?**

Cisco sold their video surveillance business (CIVS line) to Verkada and discontinued the Linksys WVC cameras. The only current Cisco camera product is the Meraki MV series, which is cloud-managed and does not support RTSP. However, many legacy Cisco cameras remain deployed and operational.

**What codecs do Cisco cameras use?**

Newer CIVS enterprise cameras support H.264. The Linksys WVC series primarily uses MPEG-4 and MJPEG. The codec depends on the model and firmware version.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Linksys Connection Guide](linksys.md) — Same URL patterns, Cisco subsidiary
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
