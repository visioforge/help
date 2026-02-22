---
title: Mercusys IP Camera RTSP URL and C# .NET Integration
description: Connect to Mercusys cameras in C# .NET with RTSP URL patterns and code samples for MC, MB series indoor and outdoor security cameras.
---

# How to Connect to Mercusys IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Mercusys** is a networking and smart home brand owned by **TP-Link**. Mercusys targets the budget-conscious segment with affordable routers, mesh systems, and security cameras. Mercusys cameras share design and firmware similarities with TP-Link Tapo cameras, offering standard RTSP support at lower price points.

**Key facts:**

- **Product lines:** MC (indoor cameras), MB (outdoor cameras)
- **Protocol support:** RTSP, ONVIF (select models), HTTP
- **Default RTSP port:** 554
- **Default credentials:** Set during app setup (no factory defaults)
- **ONVIF support:** Yes (newer models, must be enabled)
- **Video codecs:** H.264
- **Parent company:** TP-Link
- **Companion app:** Mercusys Security app

!!! info "Mercusys and TP-Link Tapo"
    Mercusys cameras share firmware architecture with TP-Link Tapo cameras. The RTSP URL format (`/stream1`, `/stream2`) is similar. If you are familiar with Tapo integration, the same approach works with Mercusys. See our [TP-Link connection guide](tp-link.md) for additional details.

## RTSP URL Patterns

### Standard URL Format

Mercusys cameras use a stream-number-based RTSP URL:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| Main stream | `rtsp://IP:554/stream1` | Full resolution |
| Sub stream | `rtsp://IP:554/stream2` | Lower resolution, less bandwidth |

### Camera Models

| Model | Type | Resolution | Main Stream URL | Audio |
|-------|------|-----------|----------------|-------|
| MC50 (indoor PT) | Indoor pan/tilt | 1920x1080 | `rtsp://IP:554/stream1` | Yes |
| MC60 (2K indoor PT) | Indoor pan/tilt | 2304x1296 | `rtsp://IP:554/stream1` | Yes |
| MC70 (4MP indoor PT) | Indoor pan/tilt | 2560x1440 | `rtsp://IP:554/stream1` | Yes |
| MB50 (outdoor bullet) | Outdoor | 1920x1080 | `rtsp://IP:554/stream1` | Yes |
| MB60 (2K outdoor) | Outdoor | 2304x1296 | `rtsp://IP:554/stream1` | Yes |
| MB70 (4MP outdoor) | Outdoor | 2560x1440 | `rtsp://IP:554/stream1` | Yes |

### Enabling RTSP / ONVIF

RTSP and ONVIF may need to be enabled in the camera settings:

1. Open the **Mercusys Security** app
2. Select your camera → **Settings**
3. Navigate to **Advanced Settings**
4. Enable **RTSP** and/or **ONVIF**
5. Set a username and password for RTSP access

## Connecting with VisioForge SDK

Use your Mercusys camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Mercusys MC70 (4MP indoor pan/tilt), main stream
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "rtsp_user"; // set in Mercusys Security app
var password = "rtsp_pass";
```

For sub-stream access, use `/stream2` instead of `/stream1`.

## Snapshot URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Requires basic auth |

## Troubleshooting

### RTSP not accessible

Mercusys cameras require initial setup through the Mercusys Security app. RTSP may also need to be explicitly enabled in the advanced settings. After enabling RTSP, the credentials set in the app must be used for RTSP authentication.

### Camera IP discovery

Find your Mercusys camera's IP address in:

1. The Mercusys Security app → Device Info
2. Your router's DHCP client list
3. ONVIF discovery (if enabled)

### Similar to TP-Link Tapo

If standard Mercusys troubleshooting does not resolve your issue, check our [TP-Link Tapo guide](tp-link.md) for additional troubleshooting steps, as the firmware is similar.

## FAQ

**What is the default RTSP URL for Mercusys cameras?**

Mercusys cameras use `rtsp://username:password@CAMERA_IP:554/stream1` for the main stream and `/stream2` for the sub stream. The username and password are set during RTSP enablement in the Mercusys Security app.

**Is Mercusys the same as TP-Link?**

Mercusys is a brand owned by TP-Link that targets the budget segment. Mercusys cameras share firmware architecture with TP-Link Tapo cameras and use similar RTSP URL formats.

**Do Mercusys cameras support ONVIF?**

Newer Mercusys camera models support ONVIF, but it must be enabled through the Mercusys Security app. Older models may not include ONVIF support.

**How do Mercusys cameras compare to TP-Link Tapo?**

Mercusys cameras are positioned as a more affordable alternative to Tapo. They share similar firmware and RTSP URL patterns. Tapo cameras generally have more model options and broader community support.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [TP-Link Connection Guide](tp-link.md) — Parent company, similar firmware
- [Tenda Connection Guide](tenda.md) — Budget camera alternative
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
