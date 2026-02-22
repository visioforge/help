---
title: Tenda IP Camera RTSP URL and C# .NET Connection Guide
description: Connect to Tenda cameras in C# .NET with RTSP URL patterns and code samples for CP, CT, IT series security cameras and pan/tilt models.
---

# How to Connect to Tenda IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Tenda Technology** is a Chinese networking equipment manufacturer headquartered in Shenzhen, China. Founded in 1999, Tenda is primarily known for routers and networking gear but has expanded into the security camera market with a growing line of affordable IP cameras targeting the consumer and small business segments. Tenda cameras are gaining traction in emerging markets across Asia, South America, and Africa.

**Key facts:**

- **Product lines:** CP (pan/tilt), CT (outdoor bullet/turret), IT (indoor)
- **Protocol support:** RTSP, ONVIF (select models), HTTP
- **Default RTSP port:** 554
- **Default credentials:** admin / admin (varies by model)
- **ONVIF support:** Yes (newer models)
- **Video codecs:** H.264, H.265 (select models)
- **Companion app:** Tenda Security app (for setup and remote viewing)

## RTSP URL Patterns

### Standard URL Format

Tenda cameras use a stream-number-based RTSP URL:

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:554/stream1
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| Main stream | `rtsp://IP:554/stream1` | Full resolution |
| Sub stream | `rtsp://IP:554/stream2` | Lower resolution |

### Camera Models

| Model | Type | Resolution | Main Stream URL | Audio |
|-------|------|-----------|----------------|-------|
| CP3 (pan/tilt indoor) | Indoor PTZ | 1920x1080 | `rtsp://IP:554/stream1` | Yes |
| CP6 (2K pan/tilt) | Indoor PTZ | 2304x1296 | `rtsp://IP:554/stream1` | Yes |
| CP7 (4MP pan/tilt) | Indoor PTZ | 2560x1440 | `rtsp://IP:554/stream1` | Yes |
| CT3 (outdoor bullet) | Outdoor | 1920x1080 | `rtsp://IP:554/stream1` | Yes |
| CT6 (2K outdoor) | Outdoor | 2304x1296 | `rtsp://IP:554/stream1` | Yes |
| CT7 (4MP outdoor) | Outdoor | 2560x1440 | `rtsp://IP:554/stream1` | Yes |
| IT6 (indoor) | Indoor | 1920x1080 | `rtsp://IP:554/stream1` | Yes |
| IT7 (2K indoor) | Indoor | 2304x1296 | `rtsp://IP:554/stream1` | Yes |

### Alternative URL Formats

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/stream1` | Main stream (recommended) |
| `rtsp://IP:554/stream2` | Sub stream |
| `rtsp://IP:554/live/ch0` | Alternative format (some models) |
| `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` | Dahua-compatible (some OEM firmware) |

## Connecting with VisioForge SDK

Use your Tenda camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Tenda CP7 (4MP pan/tilt), main stream
var uri = new Uri("rtsp://192.168.1.90:554/stream1");
var username = "admin";
var password = "YourPassword";
```

For sub-stream access, use `/stream2` instead of `/stream1`.

## Snapshot URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Requires basic auth |

## Troubleshooting

### Camera requires app setup first

Tenda cameras must be initially configured through the Tenda Security app. The camera needs WiFi credentials and account setup before RTSP is accessible. After setup, you can connect directly via RTSP on the local network.

### Multiple URL formats

Some Tenda cameras use different firmware bases. If `/stream1` does not work, try:

1. `rtsp://IP:554/live/ch0` (alternative format)
2. `rtsp://IP:554/cam/realmonitor?channel=1&subtype=0` (Dahua-compatible)
3. Use ONVIF discovery to retrieve the correct URL automatically

### Finding the camera IP

Tenda WiFi cameras get their IP via DHCP. Find it in:

1. The Tenda Security app (Device Info)
2. Your router's DHCP client list
3. ONVIF discovery (if supported)

## FAQ

**What is the default RTSP URL for Tenda cameras?**

Most Tenda cameras use `rtsp://admin:password@CAMERA_IP:554/stream1` for the main stream and `/stream2` for the sub stream. Some models use alternative URL paths.

**Do Tenda cameras support ONVIF?**

Newer Tenda camera models support ONVIF for standardized discovery and streaming. Older or budget models may not. Check your camera's specifications in the Tenda Security app.

**Are Tenda cameras good for development integration?**

Tenda cameras offer competitive pricing and standard RTSP support, making them suitable for development and prototyping. For production deployments requiring guaranteed RTSP/ONVIF compatibility, consider established surveillance brands like [Hikvision](hikvision.md), [Dahua](dahua.md), or [Reolink](reolink.md).

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [TP-Link Connection Guide](tp-link.md) — Similar consumer segment
- [Mercusys Connection Guide](mercusys.md) — Budget camera alternative
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
