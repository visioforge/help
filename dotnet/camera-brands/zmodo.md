---
title: Zmodo RTSP URL Guide: Connect IP Cameras in C# .NET
description: Zmodo ZH Wi-Fi, ZP PoE, and DVR/NVR RTSP URL patterns for C# .NET. Stream and record Zmodo cameras with VisioForge Video Capture SDK integration.
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

# How to Connect to Zmodo IP Camera in C# .NET

[Video Capture SDK .Net](https://www.visioforge.com/video-capture-sdk-net){ .md-button .md-button--primary target="_blank" } [Media Blocks SDK .Net](https://www.visioforge.com/media-blocks-sdk-net){ .md-button .md-button--primary target="_blank" }

## Brand Overview

**Zmodo Technology** is a consumer security camera brand headquartered in Shenzhen, China. Zmodo is known for affordable Wi-Fi and wired IP cameras, DVR/NVR systems, and smart home security products. The brand targets the budget consumer market and is widely available through online retailers.

**Key facts:**

- **Product lines:** ZH-IXx (Wi-Fi cameras), ZP-IBH/IBI (PoE cameras), CM-I (legacy IP cameras), ZMD-ISV (DVR systems), Greet (smart doorbell)
- **Protocol support:** RTSP, HTTP/MJPEG (legacy), Zmodo Zink (proprietary), ONVIF (limited, some ZP models)
- **Default RTSP port:** 10554 (Wi-Fi cameras), 554 (standard/DVR models)
- **Default credentials:** admin / admin or admin / (empty password)
- **ONVIF support:** Limited (some newer ZP-series PoE models only)
- **Video codecs:** H.264, MPEG-4 (legacy DVR)

!!! warning "Zmodo Zink cameras"
    Zmodo cameras that use the proprietary **Zink** protocol do **not** support RTSP at all. These cameras can only be accessed through the Zmodo app. Check your camera's specifications before attempting RTSP connections.

## RTSP URL Patterns

Zmodo cameras use different RTSP ports and URL formats depending on the product line.

### Wi-Fi Cameras (ZH-Series) -- Port 10554

```
rtsp://[USERNAME]:[PASSWORD]@[IP]:10554//tcp/av0_0
```

!!! warning "Non-standard port 10554"
    Zmodo Wi-Fi cameras (ZH-series) use **port 10554**, not the standard 554. This is the most common connection issue with Zmodo cameras.

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| Main stream | `rtsp://IP:10554//tcp/av0_0` | Full resolution |
| Sub stream | `rtsp://IP:10554//tcp/av0_1` | Lower resolution |

### Model-Specific URLs (Wi-Fi / PoE)

| Model | RTSP URL | Type |
|-------|----------|------|
| ZH-IXA15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi indoor |
| ZH-IXB15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi indoor |
| ZH-IXC15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi indoor |
| ZH-IXD15-WC | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi indoor |
| ZH-IBH13-W | `rtsp://IP:10554//tcp/av0_0` | Wi-Fi bullet |
| ZP-IBH13-P | `rtsp://IP:10554//tcp/av0_0` | PoE bullet |
| ZP-IBI13-W | `rtsp://IP:10554//tcp/av0_0` | PoE indoor |

### Standard H.264 Cameras -- Port 554

Some Zmodo cameras use the standard RTSP port:

| Stream | RTSP URL | Notes |
|--------|----------|-------|
| H.264 direct | `rtsp://IP:554/h264` | Standard port |
| Channel stream | `rtsp://IP:554/VideoInput/1/h264/1` | Channel-based |
| Channel number | `rtsp://IP:554/[CHANNEL]` | Direct channel |

### Legacy CM-I Series

| Model | RTSP URL | Alt URL | Notes |
|-------|----------|---------|-------|
| CM-I11123BK | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | HTTP fallback |
| CM-I12316GY | `rtsp://IP:554/VideoInput/1/h264/1` | `http://IP/videostream.asf` | HTTP fallback |

### DVR/NVR Systems

| Model | RTSP URL | Notes |
|-------|----------|-------|
| ZMD-ISV-BFS23NM | `rtsp://IP:554/VideoInput/1/h264/1` | Channel 1 |
| DVR (MPEG-4) | `rtsp://IP:554/mpeg4` | Legacy format |
| DVR (auth in URL) | `rtsp://IP:554/0/USERNAME:PASSWORD/main` | Auth in path |

## Connecting with VisioForge SDK

Use your Zmodo camera's RTSP URL with any of the three SDK approaches shown in the [Quick Start Guide](index.md#quick-start-code):

```csharp
// Zmodo ZH-series Wi-Fi camera, main stream -- note port 10554!
var uri = new Uri("rtsp://192.168.1.60:10554//tcp/av0_0");
var username = "admin";
var password = "admin";
```

For sub-stream access, use `//tcp/av0_1` instead of `//tcp/av0_0`.

## Snapshot and MJPEG URLs

| Type | URL Pattern | Notes |
|------|-------------|-------|
| Snapshot | `http://IP/snapshot.cgi?user=USER&pwd=PASS` | Standard models |
| Snapshot (camera) | `http://IP/snapshot.cgi?camera=1` | Camera selection |
| DVR Snapshot | `http://IP/cgi-bin/snapshot.cgi?loginuse=USER&loginpas=PASS` | DVR systems |
| ASF Stream | `http://IP/videostream.asf?user=USER&pwd=PASS&resolution=64&rate=0` | Legacy CM-I |
| MJPEG Stream | `http://IP/videostream.cgi?rate=11` | Legacy models |

## Troubleshooting

### Must use port 10554 for Wi-Fi cameras

The most common Zmodo connection issue is using port 554 when the camera requires **port 10554**. All ZH-series Wi-Fi cameras and many ZP-series PoE cameras use port 10554. If your connection times out on port 554, switch to 10554.

### TCP transport in URL path

The `//tcp/av0_0` path explicitly specifies TCP transport. This is built into the Zmodo URL format and is not optional. Do not remove the `//tcp/` prefix from the path.

### Zmodo app required for initial setup

Some Zmodo cameras require the Zmodo mobile app for initial Wi-Fi setup and activation. RTSP access may not be available until the camera has been set up through the app at least once. Complete the initial setup before attempting RTSP connections.

### Zink protocol cameras do not support RTSP

Zmodo cameras that use the proprietary **Zink** protocol are designed exclusively for the Zmodo ecosystem and do not support RTSP, ONVIF, or any third-party streaming protocol. Check the camera specifications or packaging for "Zink" branding. If your camera uses Zink, it cannot be accessed via RTSP.

### Legacy CM-I cameras use HTTP streaming

Older CM-I series cameras may have limited or unreliable RTSP support. If RTSP fails on a CM-I model, fall back to the HTTP ASF or MJPEG streaming URLs: `http://IP/videostream.asf?user=USER&pwd=PASS`.

### DVR authentication format

Some Zmodo DVRs embed credentials in the RTSP path rather than using standard RTSP authentication: `rtsp://IP:554/0/USERNAME:PASSWORD/main`. If standard authentication fails, try this URL format.

## FAQ

**What is the default RTSP URL for Zmodo Wi-Fi cameras?**

For ZH-series Wi-Fi cameras, the URL is `rtsp://admin:admin@CAMERA_IP:10554//tcp/av0_0`. Note the non-standard port 10554 and the `//tcp/` prefix in the path.

**Why does my Zmodo camera use port 10554 instead of 554?**

Zmodo chose port 10554 for their Wi-Fi camera line. This is a fixed port in the camera firmware. Some standard (non-Wi-Fi) Zmodo cameras and DVR systems use the standard port 554.

**Do all Zmodo cameras support RTSP?**

No. Zmodo cameras that use the proprietary Zink protocol do not support RTSP. These cameras are limited to the Zmodo app and cloud service. Most ZH-series, ZP-series, and CM-I series cameras do support RTSP.

**Does Zmodo support ONVIF?**

ONVIF support on Zmodo cameras is limited. Some newer ZP-series PoE models include ONVIF support, but most consumer Wi-Fi models (ZH-series) do not. Check your specific model's specifications for ONVIF compatibility.

**What is the difference between av0_0 and av0_1?**

In the Zmodo RTSP URL, `av0_0` is the main (highest quality) stream and `av0_1` is the sub (lower resolution) stream. Use `av0_1` when you need lower bandwidth consumption for remote viewing.

## Related Resources

- [All Camera Brands — RTSP URL Directory](index.md)
- [Foscam Connection Guide](foscam.md) — Budget consumer IP cameras
- [IP Camera Preview Tutorial](../videocapture/video-tutorials/ip-camera-preview.md)
- [SDK Installation & Samples](index.md#get-started)
