---
title: How to Connect to EZVIZ IP Camera in C# .NET
description: Connect to EZVIZ cameras in C# .NET with RTSP URL patterns for C1C, C3W, C6N, BC1C and other models. Enable RTSP on cloud-first cameras.
---

# How to Connect to EZVIZ IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**EZVIZ** is a consumer smart home and security camera brand owned by **Hikvision**. Originally launched as Hikvision's consumer division, EZVIZ became an independent brand focused on home cameras, doorbells, smart locks, and IoT devices. EZVIZ cameras are designed primarily for cloud-based use via the EZVIZ app, but many models support local RTSP streaming when enabled.

**Key facts:**

- **Product lines:** C-series (indoor/outdoor), BC-series (battery), DB-series (doorbells), H-series (pan/tilt)
- **Protocol support:** RTSP (must be enabled), ONVIF (limited models), EZVIZ Cloud (default)
- **Default RTSP port:** 554
- **Default credentials:** admin / verification code (printed on camera label)
- **ONVIF support:** Limited (some newer models only)
- **Video codecs:** H.264, H.265 (select models)
- **Parent company:** Hikvision

!!! warning "RTSP Must Be Enabled Manually"
    EZVIZ cameras are cloud-first devices. **RTSP is disabled by default** on most models. You must enable RTSP through the EZVIZ mobile app or web portal before connecting with the VisioForge SDK. Some budget and battery-powered models do not support RTSP at all.

## Enabling RTSP on EZVIZ Cameras

Before connecting, enable RTSP access:

1. Open the **EZVIZ app** on your phone
2. Select your camera → **Settings** (gear icon)
3. Navigate to **Local Network** or **LAN access**
4. Enable **RTSP** or **Third-party access**
5. Note the verification code (usually printed on the camera label or shown in app)

Alternatively, use the EZVIZ web portal at `https://www.ezvizlife.com` to manage camera settings.

## RTSP URL Patterns

### Standard URL Format

EZVIZ cameras use Hikvision-derived RTSP URL patterns:

```
rtsp://admin:[VERIFICATION_CODE]@[IP]:554/h264/ch1/main/av_stream
```

| Stream | URL Pattern | Description |
|--------|-------------|-------------|
| Main stream | `rtsp://IP:554/h264/ch1/main/av_stream` | Full resolution |
| Sub stream | `rtsp://IP:554/h264/ch1/sub/av_stream` | Lower resolution |

!!! info "Verification Code as Password"
    EZVIZ cameras use the **verification code** (printed on the camera label) as the RTSP password. The username is always `admin`. This is different from the EZVIZ cloud account password.

### Alternative URL Formats

Some EZVIZ models support additional URL patterns:

| URL Pattern | Notes |
|-------------|-------|
| `rtsp://IP:554/h264/ch1/main/av_stream` | Standard (recommended) |
| `rtsp://IP:554/h264/ch1/sub/av_stream` | Sub stream |
| `rtsp://IP:554/Streaming/Channels/101` | Hikvision-style (some models) |
| `rtsp://IP:554/Streaming/Channels/102` | Hikvision-style sub stream |
| `rtsp://IP:554/live` | Simple path (older models) |

### Camera Models

| Model | Type | RTSP Support | Main Stream URL |
|-------|------|-------------|----------------|
| C6N (indoor pan/tilt) | Indoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C6W (4MP indoor PT) | Indoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C1C (1080p indoor) | Indoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H6c (pan/tilt) | Indoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| H8c (outdoor PT) | Outdoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3W (outdoor bullet) | Outdoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3WN (outdoor bullet) | Outdoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| C3X (dual-lens outdoor) | Outdoor | Yes | `rtsp://IP:554/h264/ch1/main/av_stream` |
| BC1C (battery cam) | Battery | No | N/A — cloud-only |
| DB1C (doorbell) | Doorbell | No | N/A — cloud-only |

!!! warning "Battery and Doorbell Models"
    EZVIZ battery-powered cameras (BC series) and video doorbells (DB series) generally do **not** support RTSP. These devices only stream through the EZVIZ cloud. Only AC-powered cameras with wired network or stable WiFi connections support RTSP.

## Connecting with VisioForge SDK

Use your EZVIZ camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// EZVIZ C6N, main stream (verification code from label)
var uri = new Uri("rtsp://192.168.1.90:554/h264/ch1/main/av_stream");
var username = "admin";
var password = "ABCDEF"; // verification code from camera label
```

For sub-stream access, use `/h264/ch1/sub/av_stream` instead.

## Snapshot URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| JPEG Snapshot | `http://IP/cgi-bin/snapshot.cgi` | Requires basic auth with verification code |

## Troubleshooting

### "Connection refused" or no response

RTSP is disabled by default on EZVIZ cameras. You must enable it through the EZVIZ app first. Check **Settings > Local Network > Third-party access**.

### Wrong password

EZVIZ cameras use the **verification code** (6 uppercase letters printed on the camera label) as the RTSP password, **not** your EZVIZ cloud account password. The username is always `admin`.

### Camera not on local network

EZVIZ cameras connect to the cloud via WiFi. To use RTSP, the camera and your application must be on the same local network. The camera's local IP can be found in the EZVIZ app under **Device Info** or in your router's DHCP client list.

### RTSP option not available in app

Some EZVIZ models and firmware versions do not expose RTSP settings. In this case:

1. Update the camera firmware through the EZVIZ app
2. If RTSP still does not appear, the model may not support local streaming
3. Battery-powered and doorbell models typically do not support RTSP

### Hikvision URL format works on some models

Since EZVIZ cameras use Hikvision firmware, some models also accept the Hikvision URL format (`/Streaming/Channels/101`). Try this if the standard EZVIZ URL does not work.

## FAQ

**What is the default RTSP URL for EZVIZ cameras?**

The standard URL is `rtsp://admin:VERIFICATION_CODE@CAMERA_IP:554/h264/ch1/main/av_stream`. The VERIFICATION_CODE is the 6-character code printed on your camera's label. RTSP must be enabled in the EZVIZ app first.

**Is EZVIZ related to Hikvision?**

Yes. EZVIZ is a brand owned by Hikvision, focused on the consumer smart home market. EZVIZ cameras use Hikvision-derived firmware, which is why similar RTSP URL patterns work. However, EZVIZ cameras are designed primarily for cloud-based use.

**Can I use EZVIZ cameras without the cloud?**

Partially. You can access RTSP streams locally without the EZVIZ cloud for live viewing and recording. However, initial camera setup, firmware updates, and enabling RTSP require the EZVIZ app (which uses the cloud). Features like motion alerts and clip storage require an EZVIZ cloud subscription.

**Do EZVIZ cameras support ONVIF?**

Some newer EZVIZ models support ONVIF, but it is not available on all cameras. Check your camera's specifications or the EZVIZ app settings for ONVIF support. For most EZVIZ cameras, direct RTSP connection is more reliable than ONVIF.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Hikvision Connection Guide](hikvision.md) — Parent company, similar URL format
- [Imou Connection Guide](imou.md) — Dahua consumer brand, similar market
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
